using System;

using PMS.Metadata;

namespace PMS.Config
{
    static class Program
    {
        static int Main(string[] args)
        {
			if (args.Length != 2) {
				Console.WriteLine("pms-repo [file] [package]");
				return -1;
			}

			if (System.IO.File.Exists(args[0])) {
				Console.WriteLine("Load: " + args[0] + " save " + args[1]);
				RepositoryManager.Load(args[0]);
				RepositoryManager.Save(args[1]);
			}

			return 0;
        }
    }
}
