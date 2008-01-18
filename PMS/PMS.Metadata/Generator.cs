using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace PMS.Metadata
{
	internal static class Generator
	{
		public static string CamelToCString(string name)
		{
			StringBuilder str = new StringBuilder(name.Length + 2);

			for (int i=0; i<name.Length; i++) {
				if (Char.IsLower(name[i])) {
					str.Append(name[i]);
				} else {
					if (i > 0) str.Append('_');
					str.Append(Char.ToLower(name[i]));
				}
			}

			return str.ToString();
		}

		public static FieldCollection GenerateFields(Type type)
		{
			String name = null;
			String dbType = null;
			FieldCollection list = new FieldCollection();

            foreach (FieldInfo finfo in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)) {
				name = CamelToCString(finfo.Name);
				dbType = null;

				if (finfo.FieldType.IsPrimitive) {
					dbType = finfo.FieldType.Name.ToLower();
				} else if (finfo.FieldType == typeof(String)) {
					dbType = "varchar(255)";
				}

				if (dbType != null)
					list.Add(new Field(name, name, dbType, finfo.FieldType.IsPrimitive, name.Contains("id")));
				else
					Console.WriteLine("Failed to generate field for FieldType: " + finfo.FieldType);
			}

			return list;
		}

		public static Type GenerateListType(Type type)
		{
			string name = type.FullName + "Collection";

			AssemblyName an = new AssemblyName("PMS." + name);

			AssemblyBuilder assBuild = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);

			ModuleBuilder modBuild = assBuild.DefineDynamicModule("Module"); 

			TypeBuilder tb = modBuild.DefineType(name, TypeAttributes.Public | TypeAttributes.Serializable, typeof(CollectionBase));

			return tb.CreateType();
		}
	}
}
