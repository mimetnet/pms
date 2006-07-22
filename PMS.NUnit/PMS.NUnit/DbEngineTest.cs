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
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteCount()
        {
            DbEngine.ExecuteCount(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteDelete_Object()
        {
            Object query = null;

            DbEngine.ExecuteDelete(query);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteDelete_Query()
        {
            PMS.Query.IQuery obj = null;

            DbEngine.ExecuteDelete(obj);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteDelete_Type()
        {
            Type obj = null;

            DbEngine.ExecuteDelete(obj);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteInsert()
        {
            DbEngine.ExecuteInsert(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteNonQuery()
        {
            DbEngine.ExecuteNonQuery(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecutePersist()
        {
            DbEngine.ExecutePersist(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteScalar()
        {
            DbEngine.ExecuteScalar(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteSelectArray()
        {
            DbEngine.ExecuteSelectArray(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteSelectList()
        {
            DbEngine.ExecuteSelectList(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteSelectObject()
        {
            DbEngine.ExecuteSelectObject(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ExecuteUpdate()
        {
            DbEngine.ExecuteUpdate(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void GetCommand_Sql_AccessMode()
        {
            DbEngine.GetCommand(null, PMS.DataAccess.AccessMode.Read);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void GetCommand_Sql()
        {
            DbEngine.GetCommand(null);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void GetCommand_Sql_AccessMode_Empty()
        {
            DbEngine.GetCommand(String.Empty, PMS.DataAccess.AccessMode.Read);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void GetCommand_Sql_Empty()
        {
            DbEngine.GetCommand(String.Empty);
        }

        [Test]
        public void ExecuteSelectObject_ClassNotFound()
        {
            IQuery query = new QueryByObject(new Int16());

            Assert.AreEqual(query.IsValid, false, "query.IsValid should be false!");
            Assert.IsInstanceOfType(typeof(ClassNotFoundException), 
                query.ValidationException, 
                "query.ValidationException is not of type ClassNotFoundException");

            Assert.IsNull(DbEngine.ExecuteSelectArray(query));
            Assert.IsNull(DbEngine.ExecuteSelectList(query));
            Assert.IsNull(DbEngine.ExecuteSelectObject(query));
        }
    }
}
