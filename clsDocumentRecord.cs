using System;
namespace ResilienceClasses
{
    public class clsDocumentRecord
    {
        public enum Status
        {
            Unkown, Preliminary, Revised, Executed, Notarized
        }

        public enum Transmission
        {
            Unknown, Electronic, Fax, Mail
        }

        public static string strDocumentRecordPath = "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/tblDocumentRecord.csv";
        public static int DocumentColumn = 1;
        public static int ActionDateColumn = 2;
        public static int RecordDateColumn = 3;
        public static int SenderColumn = 4;
        public static int ReceiverColumn = 5;
        public static int StatusColumn = 6;
        public static int TransmissionColumn = 7;
        private int iDocumentRecordID;
        private int iDocumentID;
        private DateTime dtRecord;
        private DateTime dtAction;
        private int iSenderEntityID;
        private int iReceiverEntityID;
        private clsDocumentRecord.Status eStatus;
        private clsDocumentRecord.Transmission eTransmission;

        public clsDocumentRecord(int id)
        {
            this._Load(id);
        }

        public clsDocumentRecord(int docID, DateTime recordDate, DateTime actionDate, int senderID, int receiverID, 
                                 clsDocumentRecord.Status status, clsDocumentRecord.Transmission transmission)
        {
            this.iDocumentRecordID = this._NewDocumentRecordID();
            this.iDocumentID = docID;
            this.dtAction = actionDate;
            this.dtRecord = recordDate;
            this.iSenderEntityID = senderID;
            this.iReceiverEntityID = receiverID;
            this.eStatus = status;
            this.eTransmission = transmission;
        }

        public int ID() { return this.iDocumentRecordID; }
        public int DocumentID() { return this.iDocumentID; }
        public int SenderID() { return this.iSenderEntityID; }
        public int ReceiverID() { return this.iReceiverEntityID; }
        public DateTime RecordDate() { return this.dtRecord; }
        public DateTime ActionDate() { return this.dtAction; }
        public clsDocumentRecord.Status StatusType() { return this.eStatus; }
        public clsDocumentRecord.Transmission TransmissionType() { return this.eTransmission; }

        public bool Save()
        {
            return this.Save(clsDocumentRecord.strDocumentRecordPath);
        }

        public bool Save(string path)
        {
            clsCSVTable tbl = new clsCSVTable(path);
            if (this.iDocumentRecordID == tbl.Length() + 1)
            {
                string[] strValues = new string[tbl.Width() - 1];
                strValues[clsDocumentRecord.DocumentColumn - 1] = this.iDocumentID.ToString();
                strValues[clsDocumentRecord.ActionDateColumn - 1] = this.dtAction.ToString();
                strValues[clsDocumentRecord.RecordDateColumn - 1] = this.dtRecord.ToString();
                strValues[clsDocumentRecord.SenderColumn - 1] = this.iSenderEntityID.ToString();
                strValues[clsDocumentRecord.ReceiverColumn - 1] = this.iReceiverEntityID.ToString();
                strValues[clsDocumentRecord.StatusColumn - 1] = ((int)this.eStatus).ToString();
                strValues[clsDocumentRecord.TransmissionColumn - 1] = ((int)this.eTransmission).ToString();
                tbl.New(strValues);
                return tbl.Save();
            }
            else
            {
                if ((this.iDocumentRecordID <= tbl.Length()) && (this.iDocumentRecordID > 0))
                {
                    if (
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.ActionDateColumn, this.dtAction.ToString()) &&
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.DocumentColumn, this.iDocumentID.ToString()) &&
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.ReceiverColumn, this.iReceiverEntityID.ToString()) &&
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.RecordDateColumn, this.dtRecord.ToString()) &&
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.SenderColumn, this.iSenderEntityID.ToString()) &&
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.StatusColumn, ((int)this.eStatus).ToString()) &&
                        tbl.Update(this.iDocumentRecordID, clsDocumentRecord.TransmissionColumn, ((int)this.eTransmission).ToString()))
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

        private int _NewDocumentRecordID()
        {
            clsCSVTable tbl = new clsCSVTable(clsDocumentRecord.strDocumentRecordPath);
            return tbl.Length() + 1;
        }

        private bool _Load(int id)
        {
            clsCSVTable tbl = new clsCSVTable(clsDocumentRecord.strDocumentRecordPath);
            if (id < tbl.Length())
            {
                this.iDocumentRecordID = id;
                this.dtAction = DateTime.Parse(tbl.Value(id, clsDocumentRecord.ActionDateColumn));
                this.dtRecord = DateTime.Parse(tbl.Value(id, clsDocumentRecord.RecordDateColumn));
                this.eStatus = (clsDocumentRecord.Status)Int32.Parse(tbl.Value(id, clsDocumentRecord.StatusColumn));
                this.eTransmission = (clsDocumentRecord.Transmission)Int32.Parse(tbl.Value(id, clsDocumentRecord.TransmissionColumn));
                this.iDocumentID = Int32.Parse(tbl.Value(id, clsDocumentRecord.DocumentColumn));
                this.iSenderEntityID = Int32.Parse(tbl.Value(id, clsDocumentRecord.SenderColumn));
                this.iReceiverEntityID = Int32.Parse(tbl.Value(id, clsDocumentRecord.ReceiverColumn));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
