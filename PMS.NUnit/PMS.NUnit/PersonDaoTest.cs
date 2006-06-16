using System;

using PMS.Data;
using PMS.Broker;
using PMS.Query;

using NUnit.Framework;

namespace PMS.NUnit
{

    [TestFixture]
    public class PersonDaoTest
    {
        private IPersistenceBroker broker = null;
        private Person person = null;
        private PersonDao dao = null;

        public PersonDaoTest()
        {
            Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
            Npgsql.NpgsqlEventLog.LogName = "npgsql.log";
            Npgsql.NpgsqlEventLog.EchoMessages = false;
        }

        [TestFixtureSetUp]
        public void Constructor()
        {
            // obtain instance of PersistenceBroker
            broker = PersistenceBroker.Instance;
            broker.Load(); // load the repository.xml found in "." directory
            broker.Open(); // open database connection pool

            //broker.BeginTransaction();

            dao = new PersonDao();
        }

        [TestFixtureTearDown]
        public void Destructor()
        {
            //broker.RollbackTransaction();

            if (broker != null)
                broker.Close(); // close pool
        }

        [SetUp]
        public void SetUp()
        {
            person = new Person();
            person.FirstName = "TylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTylerTyler";
            person.LastName = "Willingham";
            person.Email = "blah@blah.com";
        }

        [TearDown]
        public void TearDown()
        {
            person = null;
        }

        [Test]
        public void B()
        {
            Assert.Greater((broker.Delete(new Person()).Count), -1);
        }

        [Test]
        public void C()
        {
            //Assert.AreEqual(1, broker.Insert(person).Count);

            Console.WriteLine("broker.Insert.Count = " + broker.Insert(person).Count);
            Console.WriteLine("broker.Insert.Ex = " + broker.Insert(person).Exception);
        }

        [Test]
        public void D()
        {
            int len = broker.GetObjectList(new QueryByType(typeof(Person))).Length;

            Assert.Greater(len, 0);
        }

        [Test]
        public void E()
        {
            person.Email = null; // don't delete by old email address

            Assert.AreEqual(1, broker.Delete(person).Count);
        }
    }
}
