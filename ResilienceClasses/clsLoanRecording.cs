using System;
using System.Collections.Generic;
namespace ResilienceClasses
{
    public class clsLoanRecording
    {
        #region Enums and Static Values
        public static string strLoanRecordingPath = "/Volumes/GoogleDrive/Team Drives/Resilience/tblLoanRecording.csv";
        public static int IndexColumn = 0;
        public static int LoanIDColumn = 1;
        public static int RecordingDateColumn = 2;
        public static int BookColumn = 3;
        public static int PageColumn = 4;
        public static int InstrumentColumn = 5;
        public static int ParcelColumn = 6;
        #endregion

        #region Properties
        private int iRecordingID;
        private int iLoanID;
        private int iBook;
        private int iPage;
        private int iInstrument;
        private int iParcel;
        private DateTime dtRecording;
        #endregion

        #region Constructors
        public clsLoanRecording()
        {
            this.init();
        }

        public clsLoanRecording(string propertyAddress)
        {
            this._LoadByAddress(propertyAddress, new clsCSVTable(clsLoanRecording.strLoanRecordingPath));
        }

        public clsLoanRecording(string propertyAddress, clsCSVTable tbl)
        {
            this._LoadByAddress(propertyAddress, tbl);
        }

        public clsLoanRecording(int loanID, int book, int page, int instrument, int parcel, DateTime dt, int loanRecordingID = -1)
        {
            if (loanRecordingID < 0) { this.iRecordingID = this._NewLoanRecordingID(new clsCSVTable(clsLoanRecording.strLoanRecordingPath)); }
            else { this.iRecordingID = loanRecordingID; }
            this.iLoanID = loanID;
            this.iBook = book;
            this.iPage = page;
            this.iInstrument = instrument;
            this.iParcel = parcel;
            this.dtRecording = dt;
        }
        #endregion

        #region PropertyAccessors
        public int ID() { return this.iRecordingID; }
        public int LoanID() { return this.iLoanID; }
        public int Book() { return this.iBook; }
        public int Page() { return this.iPage; }
        public int Instrument() { return this.iInstrument; }
        public int Parcel() { return this.iParcel; }
        public DateTime RecordingDate() { return this.dtRecording; }
        #endregion

        #region DB Methods
        public bool Save(string path)
        {
            clsCSVTable tbl = new clsCSVTable(path);
            if (this.iRecordingID == tbl.Length())
            {
                string[] strValues = new string[tbl.Width() - 1];
                strValues[clsLoanRecording.LoanIDColumn - 1] = this.iLoanID.ToString();
                strValues[clsLoanRecording.BookColumn - 1] = this.iBook.ToString();
                strValues[clsLoanRecording.PageColumn - 1] = this.iPage.ToString();
                strValues[clsLoanRecording.InstrumentColumn - 1] = this.iInstrument.ToString();
                strValues[clsLoanRecording.ParcelColumn - 1] = this.iParcel.ToString();
                strValues[clsLoanRecording.RecordingDateColumn - 1] = this.dtRecording.ToString();
                tbl.New(strValues);
                return tbl.Save();
            }
            else
            {
                if ((this.iRecordingID < tbl.Length()) && (this.iRecordingID >= 0))
                {
                    if (
                        tbl.Update(this.iRecordingID, clsLoanRecording.LoanIDColumn, this.iLoanID.ToString()) &&
                        tbl.Update(this.iRecordingID, clsLoanRecording.BookColumn, this.iBook.ToString()) &&
                        tbl.Update(this.iRecordingID, clsLoanRecording.PageColumn, this.iPage.ToString()) &&
                        tbl.Update(this.iRecordingID, clsLoanRecording.InstrumentColumn, this.iInstrument.ToString()) &&
                        tbl.Update(this.iRecordingID, clsLoanRecording.ParcelColumn, this.iParcel.ToString()) &&
                        tbl.Update(this.iRecordingID, clsLoanRecording.RecordingDateColumn, this.iRecordingID.ToString()))
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

        public bool Save()
        {
            return this.Save(clsLoanRecording.strLoanRecordingPath);
        }

        #endregion

        #region Private Methods
        private bool _LoadByAddress(string a, clsCSVTable tbl)
        {
            this.iLoanID = clsLoan.LoanID(a);
            if (this.iLoanID < 0)
            {
                this.init();
                return false;
            }
            else
            {
                List<int> matches = tbl.Matches(clsLoanRecording.LoanIDColumn, this.iLoanID.ToString());
                if (matches.Count == 0)
                {
                    this.init();
                    return false;
                }
                else
                {
                    this._Load(matches[0], tbl);
                }
            }
            return true;
        }

        private bool _Load(int loanRecordingID, clsCSVTable tbl)
        {
            this.iRecordingID = loanRecordingID;
            if (loanRecordingID < tbl.Length())
            {
                this.iRecordingID = Int32.Parse(tbl.Value(loanRecordingID, clsLoanRecording.IndexColumn));
                this.iBook = Int32.Parse(tbl.Value(loanRecordingID, clsLoanRecording.BookColumn));
                this.iPage = Int32.Parse(tbl.Value(loanRecordingID, clsLoanRecording.PageColumn));
                this.iInstrument = Int32.Parse(tbl.Value(loanRecordingID, clsLoanRecording.InstrumentColumn));
                this.iParcel = Int32.Parse(tbl.Value(loanRecordingID, clsLoanRecording.ParcelColumn));
                this.dtRecording = DateTime.Parse(tbl.Value(loanRecordingID, clsLoanRecording.RecordingDateColumn));
                return true;
            }
            else return false;
        }

        private void init()
        {
            this.iRecordingID = -1;
            this.iLoanID = -1;
            this.iBook = -1;
            this.iPage = -1;
            this.iInstrument = -1;
            this.iParcel = -1;
            this.dtRecording = System.DateTime.MaxValue;
        }

        private int _NewLoanRecordingID(clsCSVTable tbl)
        {
            return tbl.Length();
        }

        #endregion
    }
}
