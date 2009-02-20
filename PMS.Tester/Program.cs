using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using PMS.Broker;
using PMS.Data;
using PMS.Query;

namespace PMS.Tester
{
    public class Member
    {
	    private int id;
	    private string username, fun;

	    public int ID {
		    get { return id; }
		    set { id = value; }
	    }

	    public string Name {
		    get { return username; }
		    set { username = value; }
	    }

	    public string Fun {
		    get { return fun; }
		    set { fun = value; }
	    }

	    public Member()
	    {
	    }

        public override string ToString()
        {
            return String.Format("Member({0}, {1})", id, username);
        }
    }

    public class Test
    {
	    public static int Main(String[] args)
	    {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("PMS.Tester.exe.config"));

		    try {
                // - Pull IDbConnection from pool based on unique name (from config file)
                // - This is useful if you have multiple databases to chat with
                // - Not specifying a name will load the one marked default or the first one found
		        using (DbBroker cxt = new DbBroker("peon")) {
                    TestOne(cxt);
                    TestTwo(cxt);
                    TestThree(cxt);
                    TestFour(cxt);

                    //Console.WriteLine("first: " + cxt.Query<Member>().Exec().First());
                    //Console.WriteLine("first: " + cxt.Exec<Member>().First());
                    //Console.WriteLine("count: " + cxt.Query<Member>().GreaterThan("id", 50000).Exec().Count<int>());
                    //Console.WriteLine("scalar: " + cxt.Exec<Member>().Scalar<int>("SELECT id FROM member WHERE username='mimetnet'"));

                    Console.WriteLine("between: " + cxt.Query<Member>().Between("id", 50000, 60000).Exec().Count<int>());
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            } finally {
                DbManagerFactory.Close();

                Console.ReadLine();
            }

		    return 0;
	    }

        private static void TestOne(DbBroker cxt)
        {
            IList<Member> list = cxt.Query<Member>().Like("username", "%a%").OrderBy("id").Exec().Objects<List<Member>>();

            foreach (Member m in list) {
                Console.WriteLine(" : " + m);
            }
        }

        private static void TestTwo(DbBroker cxt)
        {
            foreach (Member m in cxt.Query<Member>().Like("username", "%a%").OrderBy("id").Exec()) {
                Console.WriteLine(" : " + m);
            }
        }

        private static void TestThree(DbBroker cxt)
        {
            foreach (Member m in cxt.Exec<Member>().Objects<List<Member>>("SELECT member.* FROM member where username LIKE '%a%' ORDER BY id")) {
                Console.WriteLine(" : " + m);
            }
        }

        private static void TestFour(DbBroker cxt)
        {
            using (IDataReader reader = cxt.Query<Member>().GreaterThan("id", 68665).SetColumns("id,username").Exec().Reader()) {
                while (reader.Read()) {
                    Console.WriteLine("Member(id={0}, name='{1}')", reader[0], reader[1]);
                }
            }
        }
    }
}