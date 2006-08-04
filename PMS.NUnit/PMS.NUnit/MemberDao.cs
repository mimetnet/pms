using System;
using System.Collections;
using System.Data;
using System.Xml.Serialization;

using PMS.Data;
using PMS.Broker;
using PMS.Query;
using PMS.DataAccess;

using PMS.NUnit.Model;

namespace PMS.NUnit
{
    public sealed class MemberDao
    {
        private IPersistenceBroker broker = null;

        public MemberDao()
        {
            broker = PersistenceBrokerFactory.CreateBroker();
        }

        public bool Insert(Member member)
        {
            IQuery query = new QueryByObject(member);

            return (broker.Insert(member).Count > 0) ? true : false;
        }

        public long Delete(Member member)
        {
            return broker.Delete(member).Count;
        }

        public long Update(Member member)
        {
            return broker.Delete(member).Count;
        }

        public Member[] RetrieveAll()
        {
            IQuery query = new QueryByCriteria(new Criteria(typeof(Member)));

            return (Member[])broker.GetObjectList(query);
        }

        public Member RetrieveByPK(int id)
        {
            Criteria crit = new Criteria(typeof(Member));
            crit.EqualTo("id", id);

            return (Member)broker.GetObject(new QueryByCriteria(crit));
        }
    }
}
