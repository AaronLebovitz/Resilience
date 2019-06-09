using System;
namespace ResilienceClasses
{
    public class clsEntity
    {

        #region Enums and Static Values
        public static string strEntityPath = "/Volumes/GoogleDrive/Shared Drives/Resilience/tblEntity.csv";
            // "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/tblEntity.csv";
        public static int NameColumn = 1;
        public static int AddressColumn = 2;
        public static int TownColumn = 3;
        public static int StateColumn = 4;
        public static int ZipCodeColumn = 5;
        public static int PhoneNumberColumn = 6;
        public static int ContactNameColumn = 7;
        public static int ContactEmailColumn = 8;
        public static int PathAbbreviationColumn = 9;
        #endregion

        #region Properties
        private int iEntityID;
        private string strName;
        private string strAddress;
        private string strTown;
        private string strState;
        private int iZipCode;
        private string strPhone;
        private string strContactName;
        private string strEmail;
        private string strPathAbbrev;
        #endregion

        #region Constructors
        public clsEntity(int id)
        {
            this._Load(id, new clsCSVTable(clsEntity.strEntityPath));
        }

        public clsEntity(int id, clsCSVTable tbl)
        {
            this._Load(id, tbl);
        }

        public clsEntity(string name, string address, string town, string state, int zip, string phone, string contact, string email, string path)
        {
            this.iEntityID = _NewEntityID();
            this.strName = name;
            this.strAddress = address;
            this.strTown = town;
            this.strState = state;
            this.iZipCode = zip;
            this.strPhone = phone;
            this.strContactName = contact;
            this.strEmail = email;
            this.strPathAbbrev = path;
        }
        #endregion

        #region PropertyAccessors
        public string Name() { return this.strName; }
        public int ID() { return this.iEntityID; }
        public string Address() { return this.strAddress; }
        public string Town() { return this.strTown; }
        public string State() { return this.strState; }
        public int ZipCode() { return this.iZipCode; }
        public string Phone() { return this.strPhone; }
        public string ContactName() { return this.strContactName; }
        public string ContactEmail() { return this.strEmail; }
        public string PathAbbreviation() { return this.strPathAbbrev; }
        #endregion

        #region DB Methods
        public bool Save()
        {
            return this.Save(clsEntity.strEntityPath);
        }

        public bool Save(string path)
        {
            clsCSVTable tbl = new clsCSVTable(path);
            if (this.iEntityID == tbl.Length())
            {
                string[] strValues = new string[tbl.Width() - 1];
                strValues[clsEntity.NameColumn - 1] = this.strName;
                strValues[clsEntity.AddressColumn - 1] = this.strAddress;
                strValues[clsEntity.TownColumn - 1] = this.strTown;
                strValues[clsEntity.StateColumn - 1] = this.strState;
                strValues[clsEntity.ZipCodeColumn - 1] = this.iZipCode.ToString();
                strValues[clsEntity.PhoneNumberColumn - 1] = this.strPhone;
                strValues[clsEntity.ContactNameColumn - 1] = this.strContactName;
                strValues[clsEntity.ContactEmailColumn - 1] = this.strEmail;
                strValues[clsEntity.PathAbbreviationColumn - 1] = this.strPathAbbrev;
                tbl.New(strValues);
                return tbl.Save();
            }
            else
            {
                if ((this.iEntityID < tbl.Length()) && (this.iEntityID >= 0))
                {
                    if (
                        tbl.Update(this.iEntityID, clsEntity.NameColumn, this.strName) &&
                        tbl.Update(this.iEntityID, clsEntity.AddressColumn, this.strAddress) &&
                        tbl.Update(this.iEntityID, clsEntity.TownColumn, this.strTown) &&
                        tbl.Update(this.iEntityID, clsEntity.StateColumn, this.strState) &&
                        tbl.Update(this.iEntityID, clsEntity.ZipCodeColumn, this.iZipCode.ToString()) &&
                        tbl.Update(this.iEntityID, clsEntity.PhoneNumberColumn, this.strPhone) &&
                        tbl.Update(this.iEntityID, clsEntity.ContactNameColumn, this.strContactName) &&
                        tbl.Update(this.iEntityID, clsEntity.PathAbbreviationColumn, this.strPathAbbrev) &&
                        tbl.Update(this.iEntityID, clsEntity.ContactEmailColumn, this.strEmail))
                    {
                        return tbl.Save();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Private Methods
        private int _NewEntityID()
        {
            clsCSVTable tbl = new clsCSVTable(clsEntity.strEntityPath);
            return tbl.Length();
        }

        private bool _Load(int id, clsCSVTable tbl)
        {
            if (id < tbl.Length())
            {
                this.iEntityID = id;
                this.strName = tbl.Value(id, clsEntity.NameColumn);
                this.strAddress = tbl.Value(id, clsEntity.AddressColumn);
                this.strTown = tbl.Value(id, clsEntity.TownColumn);
                this.strState = tbl.Value(id, clsEntity.StateColumn);
                Console.WriteLine(tbl.Value(id, clsEntity.ZipCodeColumn));
                this.iZipCode = Int32.Parse(tbl.Value(id, clsEntity.ZipCodeColumn));
                this.strPhone = tbl.Value(id, clsEntity.PhoneNumberColumn);
                this.strContactName = tbl.Value(id, clsEntity.ContactNameColumn);
                this.strEmail = tbl.Value(id, clsEntity.ContactEmailColumn);
                this.strPathAbbrev = tbl.Value(id, clsEntity.PathAbbreviationColumn);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
