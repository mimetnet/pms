using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.Remoting;

using PMS.Data;
using PMS.DataAccess;
using PMS.Broker;
using PMS.Query;

using NUnit.Framework;

using PMS.NUnit.Model;

namespace PMS.NUnit
{
    //[TestFixture]
    public class D_RemotingTest
    {
        private IPersistenceBroker broker = null;
		private PersonDao dao = null;
        private Person person = null;

        [TestFixtureSetUp]
        public void Constructor()
        {
            // obtain instance of PersistenceBroker
            broker = PersistenceBrokerFactory.CreateProxiedBroker();

            Assert.IsNotNull(broker, "PersistenceBroker not retrieved from remote service");

            if (broker.IsLoaded == false ) {
                FileInfo repo = new FileInfo("repository.xml");
                Assert.AreEqual(true, broker.Load(repo.FullName));
            }

            Assert.AreEqual(true, broker.Open()); // open database connection pool

            dao = new PersonDao();
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
        public void C_GetObject_QueryByObjectPK_0_SQL()
        {
            Person ps = new Person();
            ps.ID = 0;

            IQuery query = QueryFactory.ByObjectProxied(ps);

            String sql = query.Select();

            Assert.AreEqual("SELECT * FROM person",
                            sql,
                            "Generated SQL '" + sql + "' does not match 'SELECT * FROM person'");
        }

        [TestFixtureTearDown]
        public void Destructor()
        {
            if (broker != null)
                broker.Close(); // close pool
        }
    }
}
