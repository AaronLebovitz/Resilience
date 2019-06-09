using System;
using System.Collections.Generic;
namespace ResilienceClasses
{
    public class clsCashflow:IComparable<clsCashflow>
    {

        #region Enums and Static Value
        public enum Type
        {
            Unknown, AcquisitionPrice, AcquisitionConcession, AcquisitionProcessing, AcquisitionRecording, AcquisitionTaxes,
            TitlePolicy, HomeownersInsurance, RehabDraw, PropertyTax, ManagementFee,
            DispositionPrice, DispositionConcession, DispositionProcessing, DispositionRecording, DispositionTaxes,
            PromoteFee, BankFees, LegalFees, AccountingFees, CapitalCall, BrokersFee, Misc,
            NetDispositionProj, CatchUp, Principal, InterestHard, InterestAdditional, EstimatedTotalCosts, Distribution,
            InitialExpenseDraw, Acquisition, Repayment, Points
        }

        public static string strCashflowPath = "/Volumes/GoogleDrive/Shared Drives/Resilience/tblCashflow.csv";
            // "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/tblCashflow.csv";
        public static int LoanColumn = 1;
        public static int TransactionDateColumn = 2;
        public static int AmountColumn = 3;
        public static int RecordDateColumn = 4;
        public static int DeleteDateColumn = 5;
        public static int TransactionTypeColumn = 6;
        public static int ActualColumn = 7;
        public static int CommentColumn = 8;

        public static int SaveFailedOnIO = -2;
        public static int SaveFailedOnDataInsertion = -3;
        public static int SaveFailedOnIndexOutOfOrder = -4;
        #endregion

        #region Properties
        private string strName = "";
        private string strComment;
        private DateTime dtPayDate;
        private DateTime dtRecordDate;
        private DateTime dtDeleteDate;
        private int iLoanID;
        private double dAmount;
        private bool bActual;
        private clsCashflow.Type eTypeID;
        private int iTransactionID;
        #endregion

        #region Constructors
        public clsCashflow(int transactionID)
        {
            this._Load(transactionID, new clsCSVTable(clsCashflow.strCashflowPath));
        }

        public clsCashflow(int transactionID, clsCSVTable tbl)
        {
            this._Load(transactionID, tbl);
        }

        public clsCashflow(DateTime payDate, DateTime recordDate, DateTime deleteDate, int loanID, 
                           double amount, bool actual, clsCashflow.Type typeID)
        {
            this.dtPayDate = payDate;
            this.dtRecordDate = recordDate;
            this.dtDeleteDate = deleteDate;
            this.iLoanID = loanID;
            this.dAmount = amount;
            this.bActual = actual;
            this.eTypeID = typeID;
            this.strComment = "";
            this.iTransactionID = -1; // unassigned, will be assigned if saved to DB
        }

        public clsCashflow(DateTime payDate, DateTime recordDate, DateTime deleteDate, int loanID, 
                           double amount, bool actual, clsCashflow.Type typeID, string comment)
        {
            this.dtPayDate = payDate;
            this.dtRecordDate = recordDate;
            this.dtDeleteDate = deleteDate;
            this.iLoanID = loanID;
            this.dAmount = amount;
            this.bActual = actual;
            this.eTypeID = typeID;
            this.strComment = comment;
            this.iTransactionID = -1; // unassigned, will be assigned if saved to DB
        }
        #endregion

        #region DB Methods
        private bool _Load(int transactionID, clsCSVTable tbl)
        {
            if (transactionID < tbl.Length())
            {
                this.iTransactionID = transactionID;
                this.dtPayDate = DateTime.Parse(tbl.Value(transactionID, clsCashflow.TransactionDateColumn));
                this.dtRecordDate = DateTime.Parse(tbl.Value(transactionID, clsCashflow.RecordDateColumn));
                this.dtDeleteDate = DateTime.Parse(tbl.Value(transactionID, clsCashflow.DeleteDateColumn));
                if (this.dtDeleteDate > new DateTime(2999, 12, 31)) 
                { 
                    this.dtDeleteDate = DateTime.MaxValue;
                }
                this.dAmount = Double.Parse(tbl.Value(transactionID, clsCashflow.AmountColumn));
                this.iLoanID = Int32.Parse(tbl.Value(transactionID, clsCashflow.LoanColumn));
                this.eTypeID = (clsCashflow.Type)Int32.Parse(tbl.Value(transactionID, clsCashflow.TransactionTypeColumn));
                this.bActual = Boolean.Parse(tbl.Value(transactionID, clsCashflow.ActualColumn));
                this.strComment = tbl.Value(transactionID, clsCashflow.CommentColumn);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Save()
        {
            return this.Save(clsCashflow.strCashflowPath);
        }

        public int Save(string path)
        {
            clsCSVTable tbl = new clsCSVTable(path);
            if (this.iTransactionID == -1) // new cashflow, unassigned id
            {
                string[] strValues = new string[tbl.Width() - 1];
                this.iTransactionID = tbl.Length();
                strValues[clsCashflow.ActualColumn - 1] = this.bActual.ToString().ToUpper();
                strValues[clsCashflow.TransactionTypeColumn - 1] = ((int)this.eTypeID).ToString();
                strValues[clsCashflow.LoanColumn - 1] = this.iLoanID.ToString();
                strValues[clsCashflow.AmountColumn - 1] = this.dAmount.ToString();
                strValues[clsCashflow.TransactionDateColumn - 1] = this.dtPayDate.ToString("MM/dd/yyyy");
                strValues[clsCashflow.RecordDateColumn - 1] = this.dtRecordDate.ToString("MM/dd/yyyy");
                strValues[clsCashflow.DeleteDateColumn - 1] = this.dtDeleteDate.ToString("MM/dd/yyyy");
                strValues[clsCashflow.CommentColumn - 1] = this.strComment;
                tbl.New(strValues);
                if (tbl.Save())
                {
                    return this.iTransactionID;
                }
                else
                {
                    return clsCashflow.SaveFailedOnIO;
                }
            }
            else if (this.iTransactionID < tbl.Length()) // existing cashflow
            {
                if (
                    tbl.Update(this.iTransactionID, clsCashflow.ActualColumn, this.bActual.ToString()) &&
                    tbl.Update(this.iTransactionID, clsCashflow.TransactionTypeColumn, ((int)this.eTypeID).ToString()) &&
                    tbl.Update(this.iTransactionID, clsCashflow.LoanColumn, this.iLoanID.ToString()) &&
                    tbl.Update(this.iTransactionID, clsCashflow.AmountColumn, this.dAmount.ToString()) &&
                    tbl.Update(this.iTransactionID, clsCashflow.TransactionDateColumn, this.dtPayDate.ToString("MM/dd/yyyy")) &&
                    tbl.Update(this.iTransactionID, clsCashflow.RecordDateColumn, this.dtRecordDate.ToString("MM/dd/yyyy")) &&
                    tbl.Update(this.iTransactionID, clsCashflow.DeleteDateColumn, this.dtDeleteDate.ToString("MM/dd/yyyy")) &&
                    tbl.Update(this.iTransactionID, clsCashflow.CommentColumn, this.strComment)
                )
                {
                    if (tbl.Save())
                    {
                        return this.iTransactionID;
                    }
                    else
                    {
                        return clsCashflow.SaveFailedOnIO;
                    }
                }
                else
                {
                    return clsCashflow.SaveFailedOnDataInsertion;
                }
            }
            else // new cashflow, transactionID out of order past end of existing table
            {
                return clsCashflow.SaveFailedOnIndexOutOfOrder;
            }
        }
        #endregion

        #region Property Accessors
        public string Name() { return this.strName; }
        public DateTime PayDate() { return this.dtPayDate; }
        public DateTime RecordDate() { return this.dtRecordDate; }
        public DateTime DeleteDate() { return this.dtDeleteDate; }
        public int LoanID() { return this.iLoanID; }
        public double Amount() { return this.dAmount; }
        public bool Actual() { return this.bActual; }
        public clsCashflow.Type TypeID() { return this.eTypeID; }
        public int TransactionID() { return this.iTransactionID; }
        public int ID() { return this.iTransactionID; }
        public string Comment() { return this.strComment; }
        public bool AcquisitionType
        { 
            get 
            {
                return ((this.eTypeID == clsCashflow.Type.PropertyTax) ||
                        (this.eTypeID == clsCashflow.Type.AcquisitionPrice) ||
                        (this.eTypeID == clsCashflow.Type.AcquisitionTaxes) ||
                        (this.eTypeID == clsCashflow.Type.AcquisitionRecording) ||
                        (this.eTypeID == clsCashflow.Type.AcquisitionConcession) ||
                        (this.eTypeID == clsCashflow.Type.AcquisitionProcessing) ||
                        (this.eTypeID == clsCashflow.Type.HomeownersInsurance) ||
                        (this.eTypeID == clsCashflow.Type.TitlePolicy) ||
                        (this.eTypeID == clsCashflow.Type.InitialExpenseDraw) ||
                        (this.eTypeID == clsCashflow.Type.Acquisition));
            }
        }
        public bool RepaymentType
        {
            get
            {
                return ((this.eTypeID == clsCashflow.Type.Principal) ||
                        (this.eTypeID == clsCashflow.Type.InterestHard) ||
                        (this.eTypeID == clsCashflow.Type.InterestAdditional) ||
                        (this.eTypeID == clsCashflow.Type.NetDispositionProj) ||
                        (this.eTypeID == clsCashflow.Type.Repayment));
            }
        }
        #endregion

        public double AddAmount(double addAmount)
        {
            this.dAmount += addAmount; 
            return this.dAmount;
        }

        public bool Delete(DateTime dt, Boolean hardDelete = false) 
        {
            if ((this.Actual()) && (!hardDelete)) return false;
            else
            {
                this.dtDeleteDate = dt;
                return true;
            }
        }

        public bool MarkActual(DateTime dt)
        {
            if (this.dtDeleteDate > dt) { this.bActual = true; }
            return this.bActual;
        }

        public int CompareTo(clsCashflow other)
        {
            // if IDs match, they are equal
            // otherwise, sort first by PayDate, then Amount, then Type, then ID

            int iReturnValue = 0;
            if ((this.iTransactionID == other.iTransactionID) && (this.iTransactionID >= 0)) return 0;

            iReturnValue = dtPayDate.CompareTo(other.dtPayDate);
            if (iReturnValue == 0) iReturnValue = dAmount.CompareTo(other.dAmount);
            if (iReturnValue == 0) iReturnValue = dtRecordDate.CompareTo(other.dtRecordDate);
            if (iReturnValue == 0) iReturnValue = eTypeID.CompareTo(other.eTypeID);
            if (iReturnValue == 0) iReturnValue = iTransactionID.CompareTo(other.iTransactionID);

            return iReturnValue;
        }
    }
}
