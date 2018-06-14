using System;
using System.IO;
using System.Data;

namespace ResilienceClasses
{
    public class clsProperty
    {
        public static string strPropertyPath = "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/tblProperty.csv";
        public static int TownColumn = 1;
        public static int CountyColumn = 2;
        public static int StateColumn = 3;
        public static int AddressColumn = 4;
        public static int BPOColumn = 5;
        public static int NickNameColumn = 6;
        private int iPropertyID;
        private string strAddress;
        private string strTown;
        private string strCounty;
        private string strState;
        private double dBPO;
        private string strNickname;

        public clsProperty(int propertyID)
        {
            this._Load(propertyID);
        }

        public clsProperty(string address, string town, string county, string state, double bpo, string nickname)
        {
            this.iPropertyID = _NewPropertyID();
            this.strAddress = address;
            this.strTown = town;
            this.strCounty = county;
            this.strState = state;
            this.dBPO = bpo;
            this.strNickname = nickname;
        }

        public int PropertyID()
        { return this.iPropertyID; }

        public string Address()
        { return this.strAddress; }

        public string Town()
        { return this.strTown; }

        public string County()
        { return this.strCounty; }

        public string State()
        { return this.strState; }

        public double BPO()
        { return this.dBPO; }

        public string Name()
        { return this.strNickname; }

        private bool _Load(int propertyID)
        {
            clsCSVTable tbl = new clsCSVTable(clsProperty.strPropertyPath);
            if (propertyID < tbl.Length())
            {
                this.iPropertyID = propertyID;
                this.strAddress = tbl.Value(propertyID,clsProperty.AddressColumn);
                this.strTown = tbl.Value(propertyID, clsProperty.TownColumn);
                this.strCounty = tbl.Value(propertyID, clsProperty.CountyColumn);
                this.strState = tbl.Value(propertyID, clsProperty.StateColumn);
                if (!double.TryParse(tbl.Value(propertyID, clsProperty.BPOColumn), out this.dBPO)) { this.dBPO = 0; }
//                this.dBPO = Double.Parse(tbl.Value(propertyID, clsProperty.BPOColumn));
                this.strNickname = tbl.Value(propertyID, clsProperty.NickNameColumn);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Save()
        {
            return this.Save(clsProperty.strPropertyPath);
        }

        public bool Save(string path)
        {
            clsCSVTable tbl = new clsCSVTable(path);
            if (this.iPropertyID == (tbl.Length() + 1))
            {
                string[] strValues = new string[tbl.Width()-1];
                strValues[clsProperty.AddressColumn-1] = this.strAddress;
                strValues[clsProperty.BPOColumn-1] = this.dBPO.ToString();
                strValues[clsProperty.CountyColumn-1] = this.strCounty;
                strValues[clsProperty.TownColumn-1] = this.strTown;
                strValues[clsProperty.NickNameColumn-1] = this.strNickname;
                strValues[clsProperty.StateColumn-1] = this.strState;
                tbl.New(strValues);
                return tbl.Save();
            }
            else
            {
                if ((this.iPropertyID <= tbl.Length()) && (this.iPropertyID > 0))
                {
                    if (
                        tbl.Update(this.iPropertyID, clsProperty.AddressColumn, this.strAddress) &&
                        tbl.Update(this.iPropertyID, clsProperty.BPOColumn, this.dBPO.ToString()) &&
                        tbl.Update(this.iPropertyID, clsProperty.CountyColumn, this.strCounty) &&
                        tbl.Update(this.iPropertyID, clsProperty.TownColumn, this.strTown) &&
                        tbl.Update(this.iPropertyID, clsProperty.NickNameColumn, this.strNickname) &&
                        tbl.Update(this.iPropertyID, clsProperty.StateColumn, this.strState))
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

        private int _NewPropertyID()
        {
            clsCSVTable tbl = new clsCSVTable(clsProperty.strPropertyPath);
            return tbl.Length() + 1;
        }

    }
}
