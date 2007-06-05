using System;
using System.IO;

namespace PMS
{
	[Flags]
	internal enum LogLevel
	{
		None    = 0x00,
		Error   = 0x01,
		Warning = 0x02,
		Notice  = 0x04,
		Info    = 0x08,
		Debug   = 0x16,
		All     = 0xFF
	}
	
	internal static class Logger
	{
		private static StreamWriter _writer;
		private const string _lock = "LOCK";
		
		public static LogLevel Level;
		
		public static void Open(string path)
		{
			lock (_lock) {
				if (_writer != null) {
					_writer.Close ();
				}
			
				FileInfo info = new FileInfo(path);
				Stream stream = info.OpenWrite();
				stream.Seek(0, SeekOrigin.End);
				_writer = new StreamWriter(stream);
			}
		}

		public static void Debug(string msg) { Write(LogLevel.Debug, msg); }
		public static void Info(string msg) { Write(LogLevel.Info, msg); }
		public static void Notice(string msg) { Write(LogLevel.Notice, msg); }
		public static void Warning(string msg) { Write(LogLevel.Warning, msg); }
		public static void Error(string msg) { Write(LogLevel.Error, msg); }
		
		public static void Write(LogLevel level, string message)
		{
			lock (_lock) {
				if (_writer == null || ((Level & level) == LogLevel.None)) {
					return;
				}
				
				_writer.WriteLine("[" + DateTime.Now.ToString () + "] " + level.ToString () + "\t" + message);
				Console.WriteLine("[" + DateTime.Now.ToString () + "] " + level.ToString () + "\t" + message);
				_writer.Flush();
			}
		}
		
		public static void Close ()
		{
			lock (_lock) {
				if (_writer == null) {
					return;
				}
				
				_writer.Close();
				_writer = null;
			}
		}
	}
}
