using System;

using PMS.Metadata;

namespace PMS.Config
{
    static class Program
    {
        static void Main(string[] args)
        {
			foreach (string file in args) {
				if (System.IO.File.Exists(file)) {
					RepositoryManager.Load(file);
				}
			}

			RepositoryManager.Save("printgroove");
        }
    }
}
