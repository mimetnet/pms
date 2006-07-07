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
    [XmlRoot("person")]
    public class Person
    {
        private int id;
        private string firstName, lastName, email;
        private DateTime cdate;

        public Person()
        {
        }

        [XmlAttribute("id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [XmlElement("first-name")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [XmlElement("last-name")]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [XmlElement("email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [XmlElement("creation-date")]
        public DateTime CreationDate
        {
            get { return cdate; }
            set { cdate = value; }
        }
    }

    /// <summary>
    /// SettingCollection Class
    /// </summary>
    [XmlRoot("persons")]
    public class PersonCollection : CollectionBase
    {
        ///<summary>
        /// Default constructor.
        ///</summary>
        public PersonCollection() : base()
        {
        }

        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Person this[int index]
        {
            get
            {
                return (Person)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        ///<summary>
        /// Gets the number of elements contained in the SettingCollection.
        ///</summary>
        public new int Count {
            get { return this.List.Count;  }
        }

        ///<summary>
        /// Removes all items from the SettingCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the SettingCollection.
        ///</summary>
        public void Add(Person person)
        {
            this.List.Add(person);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the SettingCollection.
        ///</summary>
        public void Remove(Person person)
        {
            this.List.Remove(person);
        }

        ///<summary>
        /// Removes the SettingCollection item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the SettingCollection at the specified index.
        ///</summary>
        public void Insert(int index, Person person)
        {
            this.List.Insert(index, person);
        }

        ///<summary>
        /// Determines the index of a specific item in the SettingCollection.
        ///</summary>
        public int IndexOf(Person person)
        {
            return this.List.IndexOf(person);
        }

        ///<summary>
        /// Determines whether the SettingCollection contains a specific value.
        ///</summary>
        public bool Contains(Person person)
        {
            return this.List.Contains(person);
        }

        ///<summary>
        /// Copies elements of the SettingCollection to a Syste.Array, starting at a particular System.Array index.
        ///</summary>
        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

    }


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
            person.Id = NextIdSeq();

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
