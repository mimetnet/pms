using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using PMS.Data;
using PMS.Metadata;

using Mono.GetOptions;

[assembly: Mono.UsageComplement("")]
[assembly: Mono.About("Control PMS Providers")]
[assembly: Mono.Author("Matthew Metnetsky")]

namespace PMS.Providers
{
    static class Program
    {
		static POptions opt = null;

        static int Main(string[] args)
        {
			int x = 0;

			opt = new POptions(args);

			if (opt.verbose) {
				opt.ShowBanner();
				log4net.Config.BasicConfigurator.Configure();
			}

			switch (opt.RemainingArguments.Length) {
				case 0:
					ShowProviders();
					break;

				case 2:
					x = AddProvider(opt.RemainingArguments[0], opt.RemainingArguments[1]);
					break;

				case 1:
					if (opt.remove) {
						x = RemoveProvider(opt.RemainingArguments[0]);
						break;
					}
					goto default;

				default:
					opt.DoHelp();
					x = -1;
					break;
			}

			return x;
		}

		static int AddProvider(string name, string provider)
		{
			if (!opt.force && PMS.Data.ProviderFactory.Providers.ContainsKey(name)) {
				Console.WriteLine("'{0}' already exists", name);
				return 1;
			}

			Type t = null;

			try {
				t = PMS.Util.TypeLoader.Load(provider);
			} catch (Exception te) {
				Console.WriteLine(te.Message);
				return 1;
			}

			if (Array.IndexOf(t.GetInterfaces(), typeof(PMS.Data.IProvider)) == -1) {
				Console.WriteLine("'{0}' does not implement PMS.Data.IProvider", t);
				return 1;
			}

			try {
				PMS.Data.ProviderFactory.Add(name, (IProvider)Activator.CreateInstance(t), opt.local);
			} catch (Exception ae) {
				Console.WriteLine(ae.Message);
				return 1;
			}

			return 0;
		}

		static int RemoveProvider(string name)
		{
			return PMS.Data.ProviderFactory.Remove(name.Trim(), opt.local)? 0 : 1;
		}

		static void ShowProviders()
		{
			foreach (KeyValuePair<string, IProvider> kv in PMS.Data.ProviderFactory.Providers) {
				Console.WriteLine("  {0} \t=> {1}", kv.Key, kv.Value);
			}
		}
    }

	internal class POptions : Options
	{
		[Option("Local", 'l')]
		public bool local;

		[Option("Force", 'f')]
		public bool force;

		[Option("Remove", 'r')]
		public bool remove;

		[Option("Verbose", 'v')]
		public bool verbose;

		public POptions(string[] args) : base (args, OptionsParsingMode.Both, false, false, false)
		{
		}
	}
}
