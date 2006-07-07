using System;

using PMS.Data;
using PMS.Broker;
using PMS.Query;

using NUnit.Framework;

namespace PMS.NUnit
{

    [TestFixture]
    public class B_PersonDaoTest
    {
        private IPersistenceBroker broker = null;
        private Person person = null;
        private PersonDao dao = null;

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
            person.FirstName = "Tyler";
            person.LastName = "Willingham";
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
        public void B_Insert()
        {
            Assert.AreEqual(1, broker.Insert(person).Count);
        }

        [Test]
        public void C_GetObjectList()
        {
            int len = broker.GetObjectList(new QueryByType(typeof(Person))).Count;

            Assert.Greater(len, 0);
        }

        [Test]
        public void D_GetObjectArray()
        {
            int len = broker.GetObjectArray(new QueryByType(typeof(Person))).Length;

            Assert.Greater(len, 0);
        }

        [Test]
        public void E_DeleteByIdEtc()
        {
            person.Email = null; // don't delete by old email address

            Assert.AreEqual(1, broker.Delete(person).Count);
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
