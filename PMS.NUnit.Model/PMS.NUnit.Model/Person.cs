using System;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.NUnit.Model
{
    /// <summary>
    /// Person class 
    /// </summary>
    [Serializable]
    public class Person
    {
		private int mID;				// id
		private string mFirstName;				// first_name
		private string mLastName;				// last_name
		private string mEmail;				// email
		private int mCompanyId;				// company_id
		private DateTime mCreationDate;				// creation_date


	    ///<summary>
        ///Default Constructor
        ///</summary> 
        public Person()
        {
	    }

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Person obj1, Person obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;
            
			if (obj1.ID != obj2.ID) return false;
			if (obj1.FirstName != obj2.FirstName) return false;
			if (obj1.LastName != obj2.LastName) return false;
			if (obj1.Email != obj2.Email) return false;
			if (obj1.CompanyId != obj2.CompanyId) return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Person obj1, Person obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Person)) return false;

            return this == (Person)obj;
        }
        
        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return "[Person (ID: " + mID + ") (FirstName: " + mFirstName + ") (LastName: " + mLastName + ") (Email: " + mEmail + ") (CompanyId: " + mCompanyId + ") (CreationDate: " + mCreationDate + ") ]";
        }

        ///<summary>
        ///GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

		///<summary>
		///Required = true
		///</summary>
		public int ID {
			get { return mID; }
			set { mID = value;}
		}

		///<summary>
		///Required = false
		///</summary>
		public string FirstName {
			get { return mFirstName; }
			set { mFirstName = value;}
		}

		///<summary>
		///Required = true
		///</summary>
		public string LastName {
			get { return mLastName; }
			set { mLastName = value;}
		}

		///<summary>
		///Required = true
		///</summary>
		public string Email {
			get { return mEmail; }
			set { mEmail = value;}
		}

		///<summary>
		///Required = true
		///</summary>
		public int CompanyId {
			get { return mCompanyId; }
			set { mCompanyId = value;}
		}

		///<summary>
		///Required = false
		///</summary>
		public DateTime CreationDate {
			get { return mCreationDate; }
			set { mCreationDate = value;}
		}
    }
}
