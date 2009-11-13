using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

using PMS.Broker;
using PMS.Data;
using PMS.Query;

namespace PMS.Tester
{
    public enum Gender
    {
        Unknown,
        Male,
        Female
    }

    public class Department
    {
        private int id;
        private int oid;
        private int pid;
        private string name;

        public Department()
        {
        }

        public int ID {
		    get { return id; }
		    set { id = value; }
	    }
        
        public int OwnerID {
		    get { return oid; }
		    set { oid = value; }
	    }
        
        public int ParentID {
		    get { return pid; }
		    set { pid = value; }
	    }

	    public string Name {
		    get { return name; }
		    set { name = value; }
	    }

        public override string ToString()
        {
            return String.Format("Department({0}, {1})", id, name);
        }
    }

    public class Member
    {
	    private int id, did, rid;
	    private string email, fun, fname, lname;
        private DateTime cdate;
        private Gender gender;

        public Member()
	    {
	    }

	    public int ID {
		    get { return id; }
		    set { id = value; }
	    }

        public int DepartmentID {
		    get { return did; }
		    set { did = value; }
	    }

        public int ReportsToID {
		    get { return rid; }
		    set { rid = value; }
	    }

	    public string Email {
		    get { return email; }
		    set { email = value; }
	    }

        public string FirstName {
		    get { return fname; }
		    set { fname = value; }
	    }

        public string LastName {
		    get { return lname; }
		    set { lname = value; }
	    }

        public DateTime Creation {
            get { return cdate; }
            set { cdate = value; }
        }

        public Gender Gender {
            get { return gender; }
            set { gender = value; }
        }

	    public string Fun {
		    get { return fun; }
		    set { fun = value; }
	    }

        public override string ToString()
        {
            return String.Format("Member({0}, {1}, {2})", id, email, did);
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
		        using (DbBroker cxt = new DbBroker("repository.xml")) {
                    //TestOne(cxt);
                    TestTwo(cxt);
                    //TestThree(cxt);
                    //TestFour(cxt);
                    //TestFive(cxt);
                    //TestSix(cxt);
                }

                //TestZero();
            } catch (Exception e) {
                Console.WriteLine("TEST EXCEPTION: " + e.ToString());
            } finally {
                DbManagerFactory.Close();

                Console.ReadLine();
            }

            return 0;
        }

        //private static void TestZero()
        //{
        //    for (int x=0; x<25; x++) {
        //        (new Thread(new ThreadStart(TestThread))).Start();
        //    }
        //    Console.ReadLine();
        //}

        //private static void TestThread()
        //{
        //    try {
        //        using (DbBroker cxt = new DbBroker("peon")) {
        //            IList<Member> list = cxt.Query<Member>().Like("username", "%a%").OrderBy("id").Exec().Objects<List<Member>>();
    
        //            foreach (Member m in list) {
        //                Console.WriteLine(" : " + m);
        //            }
        //        }
        //    } catch (Exception e) {
        //        Console.WriteLine("\n\nTestThread Death: " + e.Message);
        //    }
        //}

        private static void TestOne(DbBroker cxt)
        {
            IList<Department> list = cxt.Exec<Department>().Objects<List<Department>>();

            foreach (Department o in list) {
                Console.WriteLine(" : " + o);
            }
        }

        private static void TestTwo(DbBroker cxt)
        {
            foreach (Member m in cxt.Query<Member>().Like("lname", "%m%").OrderBy("id").Exec()) {
                Console.WriteLine(" : " + m);
            }
        }

        private static void TestThree(DbBroker cxt)
        {
            foreach (Member m in cxt.Exec<Member>().Objects<List<Member>>("SELECT member.* FROM member where email LIKE '%a%' ORDER BY id")) {
                Console.WriteLine(" : " + m);
            }
        }

        private static void TestFour(DbBroker cxt)
        {
            using (IDataReader reader = cxt.Query<Member>().GreaterThan("id", 68665).Columns("id,email").Exec().Reader()) {
                while (reader.Read()) {
                    Console.WriteLine("Member(id={0}, name='{1}')", reader[0], reader[1]);
                }
            }
        }

        private static void TestFive(DbBroker cxt)
        {
            //Console.WriteLine("first: " + cxt.Query<Member>().Exec().First());
            //Console.WriteLine("first: " + cxt.Exec<Member>().First());
            //Console.WriteLine("count: " + cxt.Query<Member>().GreaterThan("id", 50000).Exec().Count<int>());
            //Console.WriteLine("scalar: " + cxt.Exec<Member>().Scalar<int>("SELECT id FROM member WHERE email='mimetnet'"));
            Console.WriteLine("between: " + cxt.Query<Member>().Between("id", 50000, 60000).Exec().Count<int>());
            Console.WriteLine("in: " + cxt.Query<Member>().In("id", 5,6,67,8,9).Exec().Count<int>());
        }

        private static void TestSix(DbBroker cxt)
        {
            Member m = new Member();
            m.Email = "matthew@kmbs";
            m.FirstName = "Matthew";
            m.LastName = "Metnetsky";

            using (IDbTransaction t = cxt.Begin()) {
                Console.WriteLine(" : " + cxt.Exec<Member>(m).Insert());

                m = cxt.Exec<Member>(m).Object();
                m.Creation = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
                Console.WriteLine(" : " + cxt.Exec<Member>(m).Update());

                m.ID = 0;
                m.Creation = DateTime.MinValue;
                Console.WriteLine(" : " + cxt.Exec<Member>(m).Delete());

            }
        }
    }
}