using System;
using System.Collections;
using System.Data;
using System.IO;

using PMS.Data;
using PMS.DataAccess;
using PMS.Broker;
using PMS.Query;

using NUnit.Framework;
using PMS.NUnit.Model;

namespace PMS.NUnit
{
    /**
    //[TestFixture]
    public class D_RemotingTest
    {
        [TestFixtureSetUp]
        public override void Constructor()
        {
            // obtain instance of PersistenceBroker
            broker = ((IPersistenceBroker)
                Activator.GetObject(Type.GetType("PMS.Broker.PersistenceBroker, PMS"),
                                    "tcp://localhost:8085/PMS.Broker.PersistenceBroker"));

            Assert.IsNotNull(broker, "PersistenceBroker not retrieved from remote service");

            if (broker.IsLoaded == false ) {
                FileInfo repo = new FileInfo("repository.xml");
                Assert.AreEqual(true, broker.Load(repo.FullName));
            }

            Assert.AreEqual(true, broker.Open()); // open database connection pool

            dao = new PersonDao();
        }

        [TestFixtureTearDown]
        public override void Destructor()
        {
            if (broker != null)
                broker.Close(); // close pool
        }
    }
    **/
}
