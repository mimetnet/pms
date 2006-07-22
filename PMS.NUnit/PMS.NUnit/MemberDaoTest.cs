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
    public class C_MemberDaoTest
    {
        protected IPersistenceBroker broker = null;
        protected Member member = null;
        protected MemberDao dao = null;
        protected int mid = 0;

        [TestFixtureSetUp]
        public virtual void Constructor()
        {
            // obtain instance of PersistenceBroker
            broker = PersistenceBroker.Instance;
            Assert.AreEqual(true, broker.Load()); // load the repository.xml found in "." directory
            Assert.AreEqual(true, broker.Open()); // open database connection pool

            dao = new MemberDao();
        }

        [TestFixtureTearDown]
        public virtual void Destructor()
        {
            //broker.RollbackTransaction();
            //broker.CommitTransaction();

            if (broker != null)
                broker.Close(); // close pool
        }

        [SetUp]
        public void SetUp()
        {
            member = new Member();
            member.Username = "mimetnet";
            member.Password = "passwd";
        }

        [TearDown]
        public void TearDown()
        {
            member = null;
        }

        [Test]
        public void A_DeleteByType()
        {
            Assert.Greater((broker.Delete(new Member()).Count), -1);
        }

        [Test]
        public void B_Insert_WithIdSequence()
        {
            IDbCommand cmd =
                DbEngine.GetCommand("SELECT nextval('member_id_seq')", AccessMode.Write);

            object obj = cmd.ExecuteScalar();

            Assert.IsNotNull(obj, "Object is null");
            Assert.IsInstanceOfType(typeof(Int64), obj);

            this.mid = Convert.ToInt32((Int64)obj);
            this.member.ID = mid;

            Assert.Greater(this.mid, 0, "mid !> 0");

            cmd = DbEngine.GetCommand("SELECT id FROM person LIMIT 1", AccessMode.Read);
            IDataReader read = cmd.ExecuteReader();
            Assert.IsTrue(read.Read(), "Cannot read person.id");

            int pid = read.GetInt32(0);
            
            Assert.IsNotNull(obj, "pid is null");

            this.member.PersonId = pid;

            Assert.Greater(this.member.PersonId, 0, "member.PersonId !> 0");
            Assert.AreEqual(1, broker.Insert(this.member).Count, "Member did not insert");
        }

        [Test]
        public void C_GetObject_QueryByObjectPrimaryKey()
        {
            Member m = new Member();
            m.ID = this.mid;

            Object obj = broker.GetObject(new QueryByObject(m));

            Assert.IsNotNull(obj, "Member is null");
            Assert.IsInstanceOfType(typeof(Member), obj, "Object is not Member");

            m = obj as Member;

            Assert.Greater(m.ID, 0, "Member.ID !> 0");
        }

        [Test]
        public void C_GetObject_QueryByObjectPK_0_SQL()
        {
            Member m = new Member();
            m.ID = 0;

            IQuery query = new QueryByObject(m);

            String sql = query.Select();

            Assert.AreEqual("SELECT * FROM member", 
                            sql,
                            "Generated SQL '" + sql + "' does not match 'SELECT * FROM member'");
        }


        [Test]
        public void C_GetObject_QueryByObjectFields()
        {
            Object obj = broker.GetObject(new QueryByObject(this.member));

            Assert.IsNotNull(obj, "Member is null");
            Assert.IsInstanceOfType(typeof(Member), obj, "Object is not Member");

            Member m = obj as Member;

            Assert.Greater(m.ID, 0, "Member.ID !> 0");
        }

        [Test]
        public void C_GetObject_QueryByType()
        {
            Object obj = broker.GetObject(new QueryByType(typeof(Member)));

            Assert.IsNotNull(obj, "Member is null");
            Assert.IsInstanceOfType(typeof(Member), obj, "Object is not Member");
        }

        [Test]
        public void C_GetObject_QueryByCriteriaPK_0_SQL()
        {
            Criteria criteria = new Criteria(typeof(Member));
            criteria.EqualTo("id", 0);

            IQuery query = new QueryByCriteria(criteria);

            String sql = query.Select();

            Assert.AreEqual("SELECT * FROM member WHERE id = 0",
                            sql,
                            "Generated SQL does not match");
        }

        [Test]
        public void C_GetObjectList()
        {
            IList pc = broker.GetObjectList(new QueryByType(typeof(Member)));

            Assert.IsNotNull(pc, "MemberCollection is null");
            Assert.IsInstanceOfType(typeof(MemberCollection), pc);
            Assert.Greater(pc.Count, 0);
        }

        [Test]
        public void C_GetObjectArray()
        {
            object[] pc = broker.GetObjectArray(new QueryByType(typeof(Member)));

            Assert.IsNotNull(pc, "Member[] is null");
            Assert.IsInstanceOfType(typeof(Member[]), pc);
            Assert.Greater(pc.Length, 0);
        }

        [Test]
        public void E_DeleteByIdEtc()
        {
            member.Password = null; // don't delete by old email address

            Assert.AreEqual(1, broker.Delete(member).Count);
        }

        [Test]
        public void F_QueryByCriteriaEqualAndBetween()
        {
            DateTime now = DateTime.Now;
            DateTime three = now.Subtract(new TimeSpan(72, 0, 0));
            Criteria crit = new Criteria(typeof(Member));
            crit.AndEqualTo("username", member.Username);
            crit.Between("creation_date", three, now);

            MemberCollection members =
                    (MemberCollection)broker.GetObjectList(new QueryByCriteria(crit));

            foreach (Member m in members) {
                Console.WriteLine(m);
            }
        }

        [Test]
        public void F_QueryByCriteriaColumns()
        {
            DateTime now = DateTime.Now;
            DateTime three = now.Subtract(new TimeSpan(72, 0, 0));
            Criteria crit = new Criteria(typeof(Member));
            IQuery query = new QueryByCriteria(crit);
            query.Selection = "username, password";

            Assert.AreEqual("SELECT username, password FROM member", query.Select());

            MemberCollection members =
                    (MemberCollection)broker.GetObjectList(query);

            foreach (Member m in members) {
                Console.WriteLine("TODOTODOTODO Asserts are NEEDED");
                Console.WriteLine(m);
            }
        }
    }
}
