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
    public sealed class PersonDao
    {
        private IPersistenceBroker broker = null;

        public PersonDao()
        {
            broker = PersistenceBrokerFactory.CreateBroker();
        }

        public bool Insert(Person person)
        {
            return (broker.Insert(person).Count > 0)? true : false;
        }

        public long Delete(Person person)
        {
            return broker.Delete(person).Count;
        }

        public long Update(Person person)
        {
            return broker.Delete(person).Count;
        }

        public Person[] Retrieve(Person person)
        {
            IQuery query = new QueryByCriteria(new Criteria(typeof(Person)));

            return (Person[])broker.GetObjectList(query);
        }

        public Person RetrieveByPK(int id)
        {
            Criteria crit = new Criteria(typeof(Person));
            crit.EqualTo("id", id);

            return (Person)broker.GetObject(new QueryByCriteria(crit));
        }
    }
}
