using System;

using PMS.Broker;
using PMS.DataAccess;
using PMS.Metadata;
using PMS.Query;

using NUnit.Framework;

namespace PMS.NUnit
{

    [TestFixture(Description="Create, Read, and Compare repository.xml files")]
    public class F_DbEngineTest
    {
        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteCount()
        {
            DbEngine.ExecuteCount(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteDelete_Object()
        {
            Object query = null;

            DbEngine.ExecuteDelete(query);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteDelete_Query()
        {
            PMS.Query.IQuery obj = null;

            DbEngine.ExecuteDelete(obj);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteDelete_Type()
        {
            Type obj = null;

            DbEngine.ExecuteDelete(obj);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteInsert()
        {
            DbEngine.ExecuteInsert(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteNonQuery()
        {
            DbEngine.ExecuteNonQuery(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecutePersist()
        {
            DbEngine.ExecutePersist(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteScalar()
        {
            DbEngine.ExecuteScalar(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteSelectArray()
        {
            DbEngine.ExecuteSelectArray(null);
        }

        [Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteSelectList()
        {
            DbEngine.ExecuteSelectList(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteSelectObject()
        {
            DbEngine.ExecuteSelectObject(null);
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void ExecuteUpdate()
        {
            DbEngine.ExecuteUpdate(null);
        }

        [Test]
        public void ExecuteSelectObject_ClassNotFound()
        {
            IQuery query = new QueryByObject(new Int16());

            Assert.AreEqual(query.IsValid, false, "query.IsValid should be false!");

            Assert.IsNull(DbEngine.ExecuteSelectArray(query));
            Assert.IsNull(DbEngine.ExecuteSelectList(query));
            Assert.IsNull(DbEngine.ExecuteSelectObject(query));
        }
    }
}
