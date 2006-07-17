using System;
using System.Collections;

using PMS.Metadata;

using NUnit.Framework;

namespace PMS.NUnit
{

    [TestFixture]
    public class A_RepositoryManagerTest
    {
        private Repository r1 = null;
        private Repository r2 = null;

        [TestFixtureSetUp]
        public void Constructor()
        {
            r1 = new Repository();

            string sConn = "Server=localhost;Database=jupiter;User ID=granny;Password=chinwag;Pooling=false";

            r1.Connections.Add(new Connection("1", sConn, typeof(Npgsql.NpgsqlConnection)));

            ArrayList fields = new ArrayList();
            fields.Add(new Field("mID", "id", "int4", true, true));
            fields.Add(new Field("mFirstName", "first_name", "varchar"));
            fields.Add(new Field("mLastName", "last_name", "varchar"));
            fields.Add(new Field("mEmail", "email", "varchar"));
            fields.Add(new Field("mCreationDate", "creation_date", "timestamp", true));

            r1.Classes.Add(new Class(typeof(Person), "person", fields, typeof(PersonCollection)));
            r1.Classes.Add(new Class(typeof(Person), "human", fields, typeof(PersonCollection)));
        }

        [Test(Description="Write To Xml")]
        public void A_Serialize()
        {
            RepositoryManager.SaveAs(r1, "repository.xml");
        }

        [Test(Description="Read From Xml")]
        public void B_Deserialize()
        {
            if (!RepositoryManager.Load("repository.xml"))
                return;

            Class c1 = r1.Classes[0] as Class;

            r2 = RepositoryManager.Repository;
            Class c2 = r2.Classes[0] as Class;

            Assert.AreEqual(c1.Name, c2.Name);
            Assert.AreEqual(c1.Table, c2.Table);
            Assert.AreEqual(c1.ListType, c2.ListType);

            Field f1 = c1.Fields[0];
            Field f2 = c2.Fields[0];

            Assert.AreEqual(f1.Name, f2.Name);
            Assert.AreEqual(f1.Column, f2.Column);
            Assert.AreEqual(f1.DbType, f2.DbType);
            Assert.AreEqual(f1.IgnoreDefault, f2.IgnoreDefault);
            Assert.AreEqual(f1.PrimaryKey, f2.PrimaryKey);

            Connection conn1 = r1.Connections[0] as Connection;
            Connection conn2 = r2.Connections[0] as Connection;

            Assert.AreEqual(conn1.Type, conn2.Type);
        }

        [TestFixtureTearDown]
        public void Destructor()
        {
            r1 = null;
        }
    }
}
