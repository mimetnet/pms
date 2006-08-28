using System;
using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Threading;

using PMS.Data;
using PMS.DataAccess;
using PMS.Broker;
using PMS.Query;

using NUnit.Framework;
using PMS.NUnit.Model;

namespace PMS.NUnit
{

    [TestFixture]
    public class B_PersonDaoTest
    {
        protected IPersistenceBroker broker = null;
        protected Person person = null;
        protected PersonDao dao = null;
        protected int pid = 0;

        #region Configuration
		[TestFixtureSetUp]
        public virtual void Constructor()
        {
            // obtain instance of PersistenceBroker
            broker = PersistenceBrokerFactory.CreateBroker();
            // load the repository.xml found in "." directory
            Assert.AreEqual(true, broker.Load(), "broker.Load() failure");
            // open database connection pool
            Assert.AreEqual(true, broker.Open(), "broker.Open() failure");


            dao = new PersonDao();

            person = new Person();
            person.FirstName = "Matthew";
            person.LastName = "Metnetsky";
            person.Email = "blah@blah.com";

            Thread.CurrentPrincipal = BuildPrincipal(new GenericIdentity("USER-NUnit", "generic"));
        }

        private IPrincipal BuildPrincipal(IIdentity identity)
        {
            string[] roles = {"rA", "rB"};
            return new GenericPrincipal(identity, roles);
        }

        [TestFixtureTearDown]
        public virtual void Destructor()
        {
            if (broker != null) broker.Close(); // close pool
        }

	    #endregion

        [Test]
        public void B_Insert_WithIdSequence()
        {
            DbResult result = DbEngine.ExecuteScalar("SELECT nextval('person_id_seq')");

            Assert.IsNotNull(result, "Object is null");
            Assert.IsInstanceOfType(typeof(DbResult), result);

            Int64 id = (Int64) result.Count;

            this.pid = Convert.ToInt32(id);

            Assert.Greater(this.pid, 0);

            this.person.ID = pid;

            Assert.AreEqual(1, broker.Insert(this.person).Count);
        }

        [Test]
        public void B_Update()
        {
			if (this.pid == 0) {
				return;
			}

			this.person.ID = this.pid;
            this.person.Email = "updated@aol.com";

            if (this.person.ID == 0) {
                Console.WriteLine("Relies on B_Insert_WithIdSequence(), run in full sequence");
                return;
            }

            DbResult result = broker.Update(this.person);

            Assert.IsNotNull(result, "result is null");
            Assert.Greater(result.Count, (double)0, "Result.Count: " + result.Count + " !> 0");
        }

        [Test]
        public void C_GetObject_QueryByObjectPrimaryKey()
        {
            Person ps = new Person();
            ps.ID = this.pid;

            Object obj = broker.GetObject(new QueryByObject(ps));

            Assert.IsNotNull(obj, "Person is null");
            Assert.IsInstanceOfType(typeof(Person), obj, "Object is not Person");

            Person p = obj as Person;

            Assert.Greater(p.ID, 0, "Person.ID !> 0");
        }

        [Test]
        public void C_GetObject_QueryByObjectPK_0_SQL()
        {
            Person ps = new Person();
            ps.ID = 0;

            IQuery query = new QueryByObject(ps);

            String sql = query.Select();

            Assert.AreEqual("SELECT * FROM person", 
                            sql,
                            "Generated SQL '" + sql + "' does not match 'SELECT * FROM person'");
        }


        [Test]
        public void C_GetObject_QueryByObjectFields()
        {
            Object obj = broker.GetObject(new QueryByObject(this.person));

            Assert.IsNotNull(obj, "Person is null");
            Assert.IsInstanceOfType(typeof(Person), obj, "Object is not Person");

            Person p = obj as Person;

            Assert.Greater(p.ID, 0, "Person.ID !> 0");
        }

        [Test]
        public void C_GetObject_QueryByType()
        {
            Object obj = broker.GetObject(new QueryByType(typeof(Person)));

            Assert.IsNotNull(obj, "Person is null");
            Assert.IsInstanceOfType(typeof(Person), obj, "Object is not Person");
        }

        [Test]
        public void C_GetObject_QueryByCriteriaPK_0_SQL()
        {
            Criteria criteria = new Criteria(typeof(Person));
            criteria.EqualTo("id", 0);

            IQuery query = new QueryByCriteria(criteria);

            String sql = query.Select();

            Assert.AreEqual("SELECT * FROM person WHERE id = 0",
                            sql,
                            "Generated SQL does not match");
        }

        [Test]
        public void C_GetObjectList()
        {
            IList pc = broker.GetObjectList(new QueryByType(typeof(Person)));

            Assert.IsNotNull(pc, "PersonCollection is null");
            Assert.IsInstanceOfType(typeof(PersonCollection), pc);
        }

        [Test]
        public void C_GetObjectArray()
        {
            object[] pc = broker.GetObjectArray(new QueryByType(typeof(Person)));

            Assert.IsNotNull(pc, "Person[] is null");
            Assert.IsInstanceOfType(typeof(Person[]), pc);
            Assert.Greater(pc.Length, 0);
        }

        [Test]
        public void F_QueryByCriteriaEqualAndBetween()
        {
            DateTime nowMinusThree = DateTime.Now.Subtract(new TimeSpan(72, 0, 0));
            DateTime nowPlusFour = nowMinusThree.Add(new TimeSpan(144, 0, 0));

            Criteria crit = new Criteria(typeof(Person));
            crit.EqualTo("first_name", "Matthew");
            crit.AndBetween("creation_date", nowMinusThree, nowPlusFour);

            PersonCollection persons =
                    (PersonCollection)broker.GetObjectList(new QueryByCriteria(crit));

            Assert.IsNotNull(persons, "persons is null");
            Assert.IsInstanceOfType(typeof(PersonCollection), persons, "persons type mismatch");
        }

        [Test]
        public void F_QueryByCriteria_GreaterOrEqual_AndLessOrEqual()
        {
            DateTime nowMinusThree = DateTime.Now.Subtract(new TimeSpan(72, 0, 0));
            DateTime nowPlusFour = nowMinusThree.Add(new TimeSpan(144, 0, 0));

            Criteria crit = new Criteria(typeof(Person));
            crit.GreaterOrEqual("creation_date", nowMinusThree);
            crit.AndLessOrEqual("creation_date", nowPlusFour);

            PersonCollection persons =
                    (PersonCollection)broker.GetObjectList(new QueryByCriteria(crit));

            Assert.IsNotNull(persons, "persons is null");
            Assert.IsInstanceOfType(typeof(PersonCollection), persons, "persons type mismatch");
        }

        [Test]
        public void F_QueryByCriteriaColumns()
        {
            DateTime now = DateTime.Now;
            DateTime three = now.Subtract(new TimeSpan(72, 0, 0));
            Criteria crit = new Criteria(typeof(Person));
            IQuery query = new QueryByCriteria(crit);
            query.Selection = "first_name, last_name";

            Assert.AreEqual("SELECT first_name, last_name FROM person", query.Select());
            
            PersonCollection persons =
                    (PersonCollection)broker.GetObjectList(query);

            Assert.IsNotNull(persons, "persons is null");
            Assert.IsInstanceOfType(typeof(PersonCollection), persons, "persons type mismatch");

            foreach (Person p in persons) {
                Assert.IsNull(p.Email);
                Assert.AreEqual(0, p.ID);
                Assert.AreEqual(0, p.CompanyId);
                Assert.AreEqual(new DateTime(), p.CreationDate);
                //Console.WriteLine(p);
            }
        }

        [Test]
        public void G_GetObjectList_Bad()
        {
            Criteria crit = new Criteria(typeof(Person));
            crit.AndEqualTo("does_not_exist", "blah");

            Assert.AreEqual(broker.GetObjectList(new QueryByCriteria(crit)), null);
        }

        [Test]
        public void G_GetOblistArray_Bad()
        {
            Criteria crit = new Criteria(typeof(Person));
            crit.AndEqualTo("does_not_exist", "blah");

            Assert.AreEqual(broker.GetObjectArray(new QueryByCriteria(crit)), null);
        }

        [Test]
        public void H_QueryBySql_Object()
        {
            IQuery query = new QueryBySql(typeof(Person), "SELECT * FROM person");

            Person person = (Person) broker.GetObject(query);

            Assert.IsNotNull(person, "person is null");
            Assert.Greater(person.ID, 0);
        }

        private int threadCnt = 0;
        private object threadLock = new object();

        public void T_Thread_X_Private_Query()
        {
            Thread.Sleep(1000);

            lock (threadLock) {
                Thread.CurrentPrincipal = 
                    BuildPrincipal(new GenericIdentity("USER-" + (++threadCnt), "generic"));
            }

            Object objs = broker.GetObjectList(new QueryByType(typeof(Person)));

            Thread.Sleep(0);

            Assert.IsNotNull(objs, "Object is null");
            Assert.IsInstanceOfType(typeof(PersonCollection), objs, "Not PersonCollection");
        }

        [Test]
        public void T_Thread_10_Private()
        {
            #region ThreadStarters
            Thread[] threads = {
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query)),
                new Thread(new ThreadStart(T_Thread_X_Private_Query))
            }; 
            #endregion

            #region Start and Join
            for (int x = 0; x < threads.Length; x++) {
                threads[x].Start();
            }

            Console.WriteLine();

            Thread.Sleep(new TimeSpan(0, 0, 4));

            for (int x = 0; x < threads.Length; x++) {
                threads[x].Join();
            }

            Console.WriteLine(); 
            #endregion
        }

        public void T_Thread_X_Shared_Query()
        {
            Thread.Sleep(1000);

            lock (threadLock) {
                if (((++threadCnt) % 2) == 1) {
                    Thread.CurrentPrincipal = 
                        BuildPrincipal(new GenericIdentity("USER-Shared", "generic"));
                }
            }

            Object objs = broker.GetObjectList(new QueryByType(typeof(Person)));

            Thread.Sleep(0);

            Assert.IsNotNull(objs, "Object is null");
            Assert.IsInstanceOfType(typeof(PersonCollection), objs, "Not PersonCollection");
        }

        [Test]
        public void T_Thread_20_Shared()
        {
            #region ThreadStarters
            Thread[] threads = {
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query)),
                new Thread(new ThreadStart(T_Thread_X_Shared_Query))
            };
            #endregion

            #region Start and Join
            for (int x = 0; x < threads.Length; x++) {
                threads[x].Start();
            }

            Console.WriteLine();

            Thread.Sleep(new TimeSpan(0, 0, 4));

            for (int x = 0; x < threads.Length; x++) {
                threads[x].Join();
            }

            Console.WriteLine(); 
            #endregion
        }
    }
}
