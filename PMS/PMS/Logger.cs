//
// Log.cs: Logs server events.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace PMS
{
	[Flags]
	public enum LogLevel
	{
		None = 0x00,
		Error = 0x01,
		Warn = 0x02,
		Info = 0x04,
		Debug = 0x08,
		Standard = Error | Warn | Info | Debug,
		All = Error | Warn | Info | Debug
	}
	
	public class Log
	{
		private bool console = true;
		private StreamWriter writer;
		private LogLevel level = LogLevel.Standard;
		private Object door = new Object();
		private static Log logger = new Log();
		
		~Log()
		{
			Close();
		}
		
		/*{{{ Static Public Properties
		public static LogLevel Level {
			get { return logger.level; }
			set { logger.level = value; }
		}
		
		public static bool WriteToConsole {
			get { return logger.console; }
			set { logger.console = value; }
		}
		/*}}}*/
		
		/*{{{ Static Public Methods */
		public static void Open(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			
			lock (logger.door) {
				Close();
				Stream stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				stream.Seek(0, SeekOrigin.End);
				logger.writer = new StreamWriter(stream);
			}
		}

		public static void Close()
		{
			lock (logger.door) {
				if (logger.writer == null)
					return;
				
				logger.writer.Flush();
				logger.writer.Close();
				logger.writer = null;
			}
		}
		/*}}}*/

		/*{{{ log4net-ish */
		public static void Error(string msg)
		{
			Write(LogLevel.Error, msg);
		}

		public static void Error(string msg, Exception exe)
		{
			Write(LogLevel.Error, msg);
			Write(LogLevel.Error, "Exception => " + exe.ToString());
		}

		public static void ErrorFormat(string fmt, params object[] args)
		{
			Write(LogLevel.Error, fmt, args);
		}

		public static void Warn(string msg)
		{
			Write(LogLevel.Warn, msg);
		}

		public static void Warn(string msg, Exception exe)
		{
			Write(LogLevel.Warn, msg);
			Write(LogLevel.Warn, "Exception => " + exe.ToString());
		}

		public static void WarnFormat(string fmt, params object[] args)
		{
			Write(LogLevel.Warn, fmt, args);
		}

		public static void Info(string msg)
		{
			Write(LogLevel.Info, msg);
		}

		public static void Info(string msg, Exception exe)
		{
			Write(LogLevel.Info, msg);
			Write(LogLevel.Info, "Exception => " + exe.ToString());
		}

		public static void InfoFormat(string fmt, params object[] args)
		{
			Write(LogLevel.Info, fmt, args);
		}

		public static void Debug(string msg)
		{
			Write(LogLevel.Debug, msg);
		}

		public static void Debug(string msg, Exception exe)
		{
			Write(LogLevel.Debug, msg);
			Write(LogLevel.Debug, "Exception => " + exe.ToString());
		}

		public static void DebugFormat(string fmt, params object[] args)
		{
			Write(LogLevel.Debug, fmt, args);
		}
		/*}}}*/
		
		/*{{{ Write */
		public static void Write(LogLevel lvl, string format, params object [] args)
		{
			Write(lvl, CultureInfo.CurrentCulture, format, args);
		}

		public static void Write(LogLevel lvl, IFormatProvider provider, string format, params object [] args)
		{
			Write(lvl, String.Format(provider, format, args));
		}
		
		public static void Write(LogLevel lvl, string message)
		{
			if (logger.writer == null && !logger.console) {
				Console.WriteLine("exit 1");
				return;
			}
			
			if ((logger.level & lvl) == LogLevel.None) {
				Console.WriteLine("exit 2");
				return;
			}
			
			String text = String.Format(CultureInfo.CurrentCulture, "[{0:u}] [{1,-5}] : {2}", DateTime.Now, lvl, message);
			
			lock (logger.door) {
				if (logger.console)
					Console.WriteLine(text);
				
				if (logger.writer != null) {
					logger.writer.WriteLine(text);
					logger.writer.Flush();
				}
			}
		}
		/*}}}*/
	}
}

// vim:foldmethod=marker:foldlevel=0:
