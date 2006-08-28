using System;
using System.Collections;
using System.Reflection;

using PMS.Metadata;

using PMS.NUnit.Model;

using NUnit.Framework;

namespace PMS.NUnit
{

    [TestFixture(Description="Create, Read, and Compare repository.xml files")]
    public class A_RepositoryManagerTest
    {
        private Repository r1 = null;
        private Repository r2 = null;

        [TestFixtureSetUp]
        public void Constructor()
        {
            Assert.IsNotNull(PMS.Broker.PersistenceBrokerFactory.CreateBroker());

            r1 = new Repository();
            r1.Assemblies.Add("Npgsql");

            string sConn = "Server=10.5.4.20;Database=jupiter;User ID=granny;Password=all_your_base;Pooling=false";

            r1.Connections.Add(new Connection("1", sConn, typeof(Npgsql.NpgsqlConnection), true, 2));

            FieldCollection fields = new FieldCollection();
            fields.Add(new Field("mID", "id", "serial4", true, true));
            fields.Add(new Field("mFirstName", "first_name", "varchar"));
            fields.Add(new Field("mLastName", "last_name", "varchar"));
            fields.Add(new Field("mEmail", "email", "varchar"));
            fields.Add(new Field("mCreationDate", "creation_date", "timestamp", true));
            r1.Classes.Add(new Class(typeof(Person), "person", fields, typeof(PersonCollection)));

            Reference pRef = new Reference("mPerson", typeof(Person));
            pRef.Retrieve = "SELECT * FROM person WHERE id = :mPersonId LIMIT 1";
            pRef.Auto = Auto.Create | Auto.Retrieve | Auto.Update | Auto.Delete;

            fields = new FieldCollection();
            fields.Add(new Field("mID", "id", "int4", true, true));
            fields.Add(new Field("mUsername", "username", "varchar"));
            fields.Add(new Field("mPassword", "password", "varchar"));
            fields.Add(new Field("mPersonId", "person_id", "int4", true, pRef));
            fields.Add(new Field("mCreationDate", "creation_date", "timestamp", true));
            r1.Classes.Add(new Class(typeof(Member), "member", fields, typeof(MemberCollection)));
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

            r2 = RepositoryManager.Repository;

            Assert.IsNotNull(r1);
            Assert.IsNotNull(r2);

            Assert.IsInstanceOfType(typeof(Repository), r1);
            Assert.IsInstanceOfType(typeof(Repository), r2);

            Assert.AreEqual(r1, r2);
        }

        [TestFixtureTearDown]
        public void Destructor()
        {
            r1 = null;
        }
    }
}
