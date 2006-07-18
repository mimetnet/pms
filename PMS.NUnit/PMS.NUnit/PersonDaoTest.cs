using System;
using System.Data;
using System.Collections;

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
        private IPersistenceBroker broker = null;
        private Person person = null;
        private PersonDao dao = null;
        private int pid = 0;

        public B_PersonDaoTest()
        {
            //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
            //Npgsql.NpgsqlEventLog.LogName = "npgsql.log";
            //Npgsql.NpgsqlEventLog.EchoMessages = false;
        }

        [TestFixtureSetUp]
        public void Constructor()
        {
            // obtain instance of PersistenceBroker
            broker = PersistenceBroker.Instance;
            Assert.AreEqual(true, broker.Load()); // load the repository.xml found in "." directory
            Assert.AreEqual(true, broker.Open()); // open database connection pool

            //broker.BeginTransaction();

            dao = new PersonDao();
        }

        [TestFixtureTearDown]
        public void Destructor()
        {
            //broker.RollbackTransaction();
            //broker.CommitTransaction();

            if (broker != null)
                broker.Close(); // close pool
        }

        [SetUp]
        public void SetUp()
        {
            person = new Person();
            person.FirstName = "Matthew";
            person.LastName = "Metnetsky";
            person.Email = "blah@blah.com";
        }

        [TearDown]
        public void TearDown()
        {
            person = null;
        }

        [Test]
        public void A_DeleteByType()
        {
            Assert.Greater((broker.Delete(new Person()).Count), -1);
        }

        [Test]
        public void B_Insert_GetIdSequence()
        {
            IDbCommand cmd = DbEngine.GetCommand("SELECT nextval('person_id_seq')", AccessMode.Write);
            object obj = cmd.ExecuteScalar();

            Assert.IsNotNull(obj, "Object is null");
            Assert.IsInstanceOfType(typeof(Int64), obj);

            Int64 id = (Int64)obj;

            this.pid = Convert.ToInt32(id);

            Assert.Greater(this.pid, 0);
        }

        [Test]
        public void B_Insert_WithIdSequence()
        {
            this.person.ID = pid;

            Assert.AreEqual(1, broker.Insert(this.person).Count);
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
            Assert.Greater(pc.Count, 0);
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
        public void E_DeleteByIdEtc()
        {
            person.Email = null; // don't delete by old email address

           // Assert.AreEqual(1, broker.Delete(person).Count);
        }

        [Test]
        public void F_QueryByCriteriaEqualAndBetween()
        {
            DateTime now = DateTime.Now;
            DateTime three = now.Subtract(new TimeSpan(72, 0, 0));
            Criteria crit = new Criteria(typeof(Person));
            crit.AndEqualTo("first_name", person.FirstName);
            crit.AndEqualTo("first_name", person.FirstName);
            crit.Between("creation_date", three, now);

            PersonCollection persons =
                    (PersonCollection)broker.GetObjectList(new QueryByCriteria(crit));

            foreach (Person p in persons) {
                Console.WriteLine(p);
            }
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

            foreach (Person p in persons) {
                Assert.IsNull(p.Email);
                Assert.AreEqual(0, p.ID);
                Assert.AreEqual(0, p.CompanyId);
                Assert.AreEqual(new DateTime(), p.CreationDate);
                Console.WriteLine(p);
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
    }
}
