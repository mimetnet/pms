using System;
using System.Collections;
using System.Data;
using System.Xml.Serialization;

using PMS.Data;
using PMS.Broker;
using PMS.Query;
using PMS.DataAccess;

namespace PMS.NUnit
{
    public sealed class PersonDao
    {
        private IPersistenceBroker broker = null;

        public PersonDao()
        {
            broker = PersistenceBroker.Instance;
        }

        private int NextIdSeq() 
        {
            return (int) DbEngine.GetCommand("SELECT nextval('person_id_seq')").ExecuteScalar();
        }

        public bool Insert(Person person)
        {
            person.ID = NextIdSeq();

            IQuery query = new QueryByObject(person);

            return (broker.Insert(person).Count > 0)? true : false;
        }

        public int Delete(Person person)
        {
            return broker.Delete(person).Count;
        }

        public int Update(Person person)
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
