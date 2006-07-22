using System;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.NUnit.Model
{
    /// <summary>
    /// Person class 
    /// </summary>
    [Serializable]
    public class Member
    {
		private int mID;
		private string mUsername;
		private string mPassword;
		private int mPersonId;
        private Person mPerson;
		private DateTime mCreationDate;

	    ///<summary>
        ///Default Constructor
        ///</summary> 
        public Member()
        {
	    }

        #region Overloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Member obj1, Member obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.ID != obj2.ID) return false;
            if (obj1.Username != obj2.Username) return false;
            if (obj1.Password != obj2.Password) return false;
            if (obj1.PersonId != obj2.PersonId) return false;
            if (obj1.CreationDate != obj2.CreationDate) return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Member obj1, Member obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Member)) return false;

            return this == (Member)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return "[Member (ID: " + mID + ") (Username: " + mUsername + ") (PersonId: " + mPersonId + ") (CreationDate: " + mCreationDate + ") ]";
        }

        ///<summary>
        ///GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        } 
        #endregion

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
		public string Username {
            get { return mUsername; }
            set { mUsername = value; }
		}

		///<summary>
		///Required = true
		///</summary>
		public string Password {
            get { return mPassword; }
            set { mPassword = value; }
		}

		///<summary>
		///Required = true
		///</summary>
		public int PersonId {
            get { return mPersonId; }
            set { mPersonId = value; }
		}

        public Person Person {
            get { return mPerson; }
            set { mPerson = value; mPersonId = value.ID; }
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
