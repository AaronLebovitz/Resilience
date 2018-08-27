﻿using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

namespace ResilienceClasses
{
    public class clsLoan
    {
        #region Enums and Static Values
        public enum State { Unknown, Cancelled, PendingAcquisition, Rehab, Listed, PendingSale, Sold }

        public static string strLoanPath = "/Volumes/GoogleDrive/Team Drives/Resilience/tblLoan.csv";
            // "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/tblLoan.csv";
        public static int IndexColumn = 0;
        public static int PropertyColumn = 1;
        public static int TitleHolderColumn = 2;
        public static int CoBorrowerColumn = 3;
        public static int TitleCompanyColumn = 4;
        public static int OGDateColumn = 5;
        public static int MaturityDateCoumn = 6;
        public static int RateColumn = 7;
        public static int PenaltyRateColumn = 8;
        public static int LenderColumn = 9;
        #endregion

        #region Static Methods
        public static int LoanID(string address)
        {
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            int loanID = -1;

            // Find PropertyID from Address First
            int propertyID = clsProperty.IDFromAddress(address);
            if (propertyID == -1) return -1;

            // Now match propertyID to the loan
            for (int i = 0; i < tblLoans.Length(); i++)
            {
                if (tblLoans.Value(i, clsLoan.PropertyColumn) == propertyID.ToString()) loanID = i;
            }
            return loanID;
        }

        public static Dictionary<string, int> LoanIDsByAddress()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tblLoans.Length(); i++)
            {
                dict.Add((new clsLoan(i)).Property().Address(), i);
            }
            return dict;
        }
        #endregion

        #region Properties
        private int iLoanID;
        private int iPropertyID;
        private int iTitleHolderEntityID;
        private int iCoBorrowerEntityID;
        private int iAcquisitionTitleCompanyEntityID;
        private int iLenderEntityID;
        private List<clsCashflow> cfCashflows;
        private string strName;
        private clsProperty pProperty;
        private DateTime dtOrigination;
        private DateTime dtMaturity;
        private double dRate;
        private double dPenaltyRate;
        #endregion

        #region Constructors
        public clsLoan(int loanID)
        {
            if (loanID < 0)
            {
                this.iLoanID = loanID;
                this.iPropertyID = -1;
                this.iTitleHolderEntityID = -1;
                this.iAcquisitionTitleCompanyEntityID = -1;
                this.iCoBorrowerEntityID = -1;
                this.iLenderEntityID = -1;
                this.cfCashflows = new List<clsCashflow>();
                this.pProperty = new clsProperty("N/A","N/A","N/A","NA",0,"FundOps");
                this.dtMaturity = System.DateTime.MaxValue;
                this.dtOrigination = System.DateTime.MinValue;
                this.dRate = 0D;
                this.dPenaltyRate = 0D;
            }
            else
                this._Load(loanID, new clsCSVTable(clsLoan.strLoanPath));
        }

        public clsLoan(int loanID, clsCSVTable tbl)
        {
            if (loanID < 0)
            {
                this.iLoanID = loanID;
                this.iPropertyID = -1;
                this.iTitleHolderEntityID = -1;
                this.iAcquisitionTitleCompanyEntityID = -1;
                this.iCoBorrowerEntityID = -1;
                this.iLenderEntityID = -1;
                this.cfCashflows = new List<clsCashflow>();
                this.pProperty = new clsProperty("N/A", "N/A", "N/A", "NA", 0, "FundOps");
                this.dtMaturity = System.DateTime.MaxValue;
                this.dtOrigination = System.DateTime.MinValue;
                this.dRate = 0D;
                this.dPenaltyRate = 0D;
            }
            else
                this._Load(loanID, tbl);
        }

        public clsLoan(int propertyID, int titleHolderID, int coBorrowerID, int titleCoID, int lenderID,
                       DateTime orig, DateTime mature, double r, double pr, int loanID = -1)
        {
            if (loanID < 0) { this.iLoanID = this._NewLoanID(); }
            else { this.iLoanID = loanID; }
            this.iPropertyID = propertyID;
            this.iTitleHolderEntityID = titleHolderID;
            this.iCoBorrowerEntityID = coBorrowerID;
            this.iAcquisitionTitleCompanyEntityID = titleCoID;
            this.iLenderEntityID = lenderID;
            this.dtMaturity = mature;

            this.dtOrigination = orig;
            this.dRate = r;
            this.dPenaltyRate = pr;
            this.pProperty = new clsProperty(propertyID);
            this.cfCashflows = new List<clsCashflow>();
        }
        #endregion

        #region Public Methods
        public int LenderId
        {
            get { return this.iLenderEntityID; }
            set { this.iLenderEntityID = value; }
        }

        public int BorrowerId
        {
            get { return this.iTitleHolderEntityID; }
            set { this.iTitleHolderEntityID = value; }
        }

        public clsLoan LoanAsOf(DateTime dt)
        {
            clsLoan newLoan = new clsLoan(this.iPropertyID, this.iTitleHolderEntityID, this.iCoBorrowerEntityID,
                                          this.iAcquisitionTitleCompanyEntityID, this.iLenderEntityID, this.dtOrigination, this.dtMaturity,
                                          this.dRate, this.dPenaltyRate, this.iLoanID);
            if (this.cfCashflows.Count != 0)
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if (cf.RecordDate() <= dt)
                    {
                        if (cf.PayDate() <= dt)
                        {
                            if (cf.Actual())
                            {
                                newLoan.AddCashflow(cf);
                            }
                            else if (cf.DeleteDate() > dt)
                            {
                                newLoan.AddCashflow(new clsCashflow(cf.PayDate(), cf.RecordDate(), DateTime.MaxValue, cf.LoanID(), cf.Amount(), true, cf.TypeID()));
                            }
                        }
                        else if ((cf.PayDate() > dt) && (cf.DeleteDate() > dt))
                        {
                            newLoan.AddCashflow(new clsCashflow(cf.PayDate(), cf.RecordDate(), DateTime.MaxValue, cf.LoanID(), cf.Amount(), false, cf.TypeID()));
                        }
                    }
                }
            }
            return newLoan;
        }

        public void SetNewOriginationDate(DateTime dt, bool updateExpiration = true)
        {
            if (updateExpiration) { this.dtMaturity = this.dtMaturity + (dt - this.dtOrigination); }
            this.dtOrigination = dt;
        }

        public bool AddCashflow(clsCashflow cf)
        {
            if (cf.LoanID() == this.iLoanID)
            {
                this.cfCashflows.Add(cf);
                return true;
            }
            else return false;
        }

        public void Cancel() { this.Cancel(System.DateTime.Today); }

        public void Cancel(DateTime dt)
        {
            this.dtMaturity = System.DateTime.MinValue;
            this.dtOrigination = System.DateTime.MinValue;
            foreach (clsCashflow cf in this.Cashflows())
                cf.Delete(dt);
        }

        public bool Save() { return this.Save(clsLoan.strLoanPath, true, clsCashflow.strCashflowPath); }

        public bool Save(string path, bool saveCashflows, string cfPath)
        {

            clsCSVTable tbl = new clsCSVTable(path);
            if (this.iLoanID < 0) this.iLoanID = tbl.Length();
            if (this.iLoanID == tbl.Length())
            {
                string[] strValues = new string[tbl.Width() - 1];
                strValues[clsLoan.CoBorrowerColumn - 1] = this.iCoBorrowerEntityID.ToString();
                strValues[clsLoan.MaturityDateCoumn - 1] = this.dtMaturity.ToShortDateString();
                strValues[clsLoan.OGDateColumn - 1] = this.dtOrigination.ToShortDateString();
                strValues[clsLoan.PenaltyRateColumn - 1] = this.dPenaltyRate.ToString();
                strValues[clsLoan.PropertyColumn - 1] = this.iPropertyID.ToString();
                strValues[clsLoan.RateColumn - 1] = this.dRate.ToString();
                strValues[clsLoan.TitleCompanyColumn - 1] = this.iAcquisitionTitleCompanyEntityID.ToString();
                strValues[clsLoan.TitleHolderColumn - 1] = this.iTitleHolderEntityID.ToString();
                strValues[clsLoan.LenderColumn - 1] = this.iLenderEntityID.ToString();
                tbl.New(strValues);
                if (tbl.Save())
                {
                    if (saveCashflows)
                    {
                        bool bCashflowsSaved = true;
                        foreach (clsCashflow cf in this.cfCashflows)
                        {
                            if (cf.Save(cfPath) < 0) { bCashflowsSaved = false; }
                        }
                        return bCashflowsSaved;
                    }
                    else return true;
                }
                else return false;
            }
            else
            {
                if ((this.iLoanID < tbl.Length()) && (this.iLoanID >= 0))
                {
                    if (
                        tbl.Update(this.iLoanID, clsLoan.CoBorrowerColumn, this.iCoBorrowerEntityID.ToString()) &&
                        tbl.Update(this.iLoanID, clsLoan.MaturityDateCoumn, this.dtMaturity.ToShortDateString()) &&
                        tbl.Update(this.iLoanID, clsLoan.OGDateColumn, this.dtOrigination.ToShortDateString()) &&
                        tbl.Update(this.iLoanID, clsLoan.PenaltyRateColumn, this.dPenaltyRate.ToString()) &&
                        tbl.Update(this.iLoanID, clsLoan.PropertyColumn, this.iPropertyID.ToString()) &&
                        tbl.Update(this.iLoanID, clsLoan.RateColumn, this.dRate.ToString()) &&
                        tbl.Update(this.iLoanID, clsLoan.TitleCompanyColumn, this.iAcquisitionTitleCompanyEntityID.ToString()) &&
                        tbl.Update(this.iLoanID, clsLoan.LenderColumn, this.iLenderEntityID.ToString()) &&
                        tbl.Update(this.iLoanID, clsLoan.TitleHolderColumn, this.iTitleHolderEntityID.ToString()))
                    {
                        if (tbl.Save())
                        {
                            if (saveCashflows)
                            {
                                bool bCashflowsSaved = true;
                                foreach (clsCashflow cf in this.cfCashflows)
                                {
                                    if (cf.Save(cfPath) < 0) { bCashflowsSaved = false; }
                                }
                                return bCashflowsSaved;
                            }
                            else return true;
                        }
                        else return false;
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
        private bool _Load(int loanID)
        {
            return this._Load(loanID, new clsCSVTable(clsLoan.strLoanPath));
        }

        private bool _Load(int loanID, clsCSVTable tbl)
        {
            this.iLoanID = loanID;
            if (loanID < tbl.Length())
            {
                this.iPropertyID = Int32.Parse(tbl.Value(loanID, clsLoan.PropertyColumn));
                this.iTitleHolderEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.TitleHolderColumn));
                this.iCoBorrowerEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.CoBorrowerColumn));
                this.iAcquisitionTitleCompanyEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.TitleCompanyColumn));
                this.iLenderEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.LenderColumn));
                this.dtMaturity = DateTime.Parse(tbl.Value(loanID, clsLoan.MaturityDateCoumn));
                this.dtOrigination = DateTime.Parse(tbl.Value(loanID, clsLoan.OGDateColumn));
                this.dRate = Double.Parse(tbl.Value(loanID, clsLoan.RateColumn));
                this.dPenaltyRate = Double.Parse(tbl.Value(loanID, clsLoan.PenaltyRateColumn));
                this.pProperty = new clsProperty(this.iPropertyID);
                this._LoadCashflows();
                return true;
            }
            else return false;
        }

        private void _LoadCashflows()
        {
            this._LoadCashflows(clsCashflow.strCashflowPath);
        }

        private void _LoadCashflows(string path)
        {
            clsCSVTable tbl = new clsCSVTable(path);
            this.cfCashflows = new List<clsCashflow>();
            int iCFLoanID;
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (Int32.TryParse(tbl.Value(i, clsCashflow.LoanColumn), out iCFLoanID))
                {
                    if (iCFLoanID == this.iLoanID)
                    {
                        this.cfCashflows.Add(new clsCashflow(i, tbl));
                    }
                }
            }
        }
        #endregion

        #region Property Accessors
        public DateTime OriginationDate() { return this.dtOrigination; }
        public DateTime MaturityDate() { return this.dtMaturity; }
        public double Rate() { return this.dRate; }
        public double PenaltyRate() { return this.dPenaltyRate; }
        public int TitleHolderID() { return this.iTitleHolderEntityID; }
        public int CoBorrowerID() { return this.iCoBorrowerEntityID; }
        public int TitleCompanyID() { return this.iAcquisitionTitleCompanyEntityID; }
        public int PropertyID() { return this.iPropertyID; }
        public int LenderID() { return this.iLenderEntityID; }
        public int ID() { return this.iLoanID; }
        public clsProperty Property() { return this.pProperty; }
        public List<clsCashflow> Cashflows() { return this.cfCashflows; }
        #endregion

        #region Calculation Methods
        public clsLoan.State Status()
        {
            // takes the current status of a Loan - does not look forward or back
            // Use LoanAsOf.Status() to do that
            // public enum State { Unknown, Cancelled, PendingAcquisition, Rehab, Listed, PendingSale, Sold }

            clsLoan.State s = clsLoan.State.Unknown;
            bool bAllExpired = true;
            bool bRehabRemains = false;
            bool bSaleScheduled = false;

            if (this.cfCashflows.Count != 0)
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if (cf.Actual())
                    {
                        bAllExpired = false;
                        if ((cf.TypeID() == clsCashflow.Type.AcquisitionPrice) && (s == clsLoan.State.Unknown))
                            s = clsLoan.State.Rehab;
                        else if (cf.TypeID() == clsCashflow.Type.Principal)
                        {
                            s = clsLoan.State.Sold;
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestHard)
                        {
                            s = clsLoan.State.Sold;
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestAdditional)
                        {
                            s = clsLoan.State.Sold;
                        }
                        else if (cf.TypeID() == clsCashflow.Type.NetDispositionProj)
                        {
                            // iff we do a loan as of a future date, after it's projected disposition, and no current sale is scheduled
                            s = clsLoan.State.Sold;
                        }
                    }
                    else if (cf.DeleteDate().Year == DateTime.MaxValue.Year) // not deleted, but not yet actual
                    {
                        bAllExpired = false;
                        if (cf.TypeID() == clsCashflow.Type.RehabDraw) bRehabRemains = true;
                        else if (cf.TypeID() == clsCashflow.Type.Principal) bSaleScheduled = true;
                    }
                }
                if (bAllExpired) s = clsLoan.State.Cancelled;
                else if (bSaleScheduled) s = clsLoan.State.PendingSale;
                else if ((!bRehabRemains) && (s != clsLoan.State.Sold)) s = clsLoan.State.Listed;
                else if (s == clsLoan.State.Unknown) s = clsLoan.State.PendingAcquisition;
                return s;
            }
            else return clsLoan.State.Cancelled;
        }

        public double AccruedInterest(DateTime dt)
        {
            // calculates accured interest only for actual payments that took place before the calculation date
            // this does not account for any future projected cashflows!
            // TO Account for future projected cashflows, use this.LoanAsOf(dt).AccruedInterest(dt)
            // TO look back and project forward use this.LoanAsOf(dtLookBack).LoanAsOf(dtLookAhead).AccruedInterest(dtLookAhead)
            double dAccrued = 0;
            double dExpiredDays;
            bool bSold = false;
            if (this.cfCashflows.Count == 0)
            {
                return 0;
            }
            else
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    dExpiredDays = Math.Max(0, Math.Min((dt - this.dtMaturity).Days, (dt - cf.PayDate()).Days));
                    if ((cf.PayDate() < dt) && (cf.Actual()))
                    {
                        dAccrued += cf.Amount() * this.dRate * (dt - cf.PayDate()).Days / 360D;
                        dAccrued += cf.Amount() * this.dPenaltyRate * dExpiredDays / 360D;
                        if (cf.TypeID() == clsCashflow.Type.Principal) { bSold = true; }
                    }
                }
                if (bSold) { dAccrued = 0D; }
                return -dAccrued;
            }
        }

        public double AccruedInterest()
        { return this.AccruedInterest(System.DateTime.Today); }

        public double Balance(DateTime dt)
        {
            // this takes just the balance based on actual payments that occurred before the given date
            // there is no projected balance, nor does this account for any projected future cashflows!
            // TO Account for future projected cashflows, use this.LoanAsOf(dt).Balance(dt)
            // TO look back and project forward use this.LoanAsOf(dtLookBack).LoanAsOf(dtLookAhead).AccruedInterest(dtLookAhead)
            double dBalance = 0;
            if (this.cfCashflows.Count == 0)
            {
                return 0;
            }
            else
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if ((cf.PayDate() <= dt) && (cf.Actual()) &&
                        (cf.TypeID() != clsCashflow.Type.InterestHard) &&
                        (cf.TypeID() != clsCashflow.Type.InterestAdditional))
                    {
                        dBalance += cf.Amount();
                    }
                }
                return -dBalance;
            }
        }

        public double Balance()
        { return this.Balance(System.DateTime.Today); }

        public double RehabSpent(DateTime dt)
        {
            // TO Account for future projected cashflows, use this.LoanAsOf(dt).RehabSpent(dt)
            // TO look back and project forward use this.LoanAsOf(dtLookBack).LoanAsOf(dtLookAhead).RehabSpent(dtLookAhead)
            return -this._Paid(dt, clsCashflow.Type.RehabDraw);
        }

        public double RehabSpent()
        { return this.RehabSpent(System.DateTime.Today); }

        public double RehabRemain(DateTime dt)
        {
            // TO Account for future projected cashflows, use this.LoanAsOf(dt).RehabRemain(dt)
            // TO look back and project forward use this.LoanAsOf(dtLookBack).LoanAsOf(dtLookAhead).RehabRemain(dtLookAhead)
            return -this._ProjectedToBePaid(dt, clsCashflow.Type.RehabDraw);
        }

        public double RehabRemain()
        { return this.RehabRemain(System.DateTime.Today); }

        public double PrincipalPaid(DateTime dt)
        { return this._Paid(dt, clsCashflow.Type.Principal); }

        public double PrincipalPaid()
        { return this.PrincipalPaid(System.DateTime.Today); }

        public double InterestPaid(DateTime dt)
        { return (this._Paid(dt, clsCashflow.Type.InterestHard) + this._Paid(dt, clsCashflow.Type.InterestAdditional)); }

        public double InterestPaid()
        { return this.InterestPaid(System.DateTime.Today); }

        public double AdditionalInterestPaid(DateTime dt)
        { return this._Paid(dt, clsCashflow.Type.InterestAdditional); }

        public double AdditionalInterestPaid()
        { return this.AdditionalInterestPaid(System.DateTime.Today); }

        public double HardInterestPaid(DateTime dt)
        { return this._Paid(dt, clsCashflow.Type.InterestHard); }

        public double HardInterestPaid()
        { return this.HardInterestPaid(System.DateTime.Today); }

        public DateTime SaleDate()
        {
            DateTime dt = this.FindDate(clsCashflow.Type.Principal, false, true);
            if (dt == DateTime.MinValue) { dt = this.FindDate(clsCashflow.Type.NetDispositionProj, false, true); }
            return dt;
        }

        public DateTime FindDate(clsCashflow.Type cashflowType, bool firstFound, bool paydate)
        {
            // for a given type of cashflow, this will return either the paydate or the recorddate
            // if firstFound, then it finds the first cashflow of that type
            // if not firstFound, then it finds the latest undeleted cashflow of that type
            DateTime cfDate;
            DateTime dtFound;
            if (firstFound) dtFound = DateTime.MaxValue;
            else dtFound = DateTime.MinValue;

            if (this.cfCashflows.Count == 0)
            {
                return dtFound;
            }
            else
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if ((cf.TypeID() == cashflowType))
                    {
                        if (paydate) cfDate = cf.PayDate();
                        else cfDate = cf.RecordDate();

                        if (firstFound)
                        {
                            if (cfDate < dtFound) dtFound = cfDate;
                        }
                        else
                        {
                            if ((cfDate > dtFound) && (cf.DeleteDate().Year == DateTime.MaxValue.Year)) dtFound = cfDate;

                        }
                    }
                }
            }
            return dtFound;
        }

        public double AcquisitionCost(bool original)
        {
            // returns the loan balance as of the origination date of the loan (projected or past)
            // if (original), uses the first record date found;  if (!original) uses the loan origination date
            if (original)
            {
                // take the loan as of the first record date
                clsLoan l = this.LoanAsOf(this.FindDate(clsCashflow.Type.AcquisitionPrice, true, false));
                // calc balance as of the first pay date for the as of loan
                return l.Balance(l.FindDate(clsCashflow.Type.AcquisitionPrice, true, true));
            }
            else
            {
                clsLoan l = this.LoanAsOf(this.dtOrigination);
                return l.Balance(this.dtOrigination);
            }
        }

        public double OriginalAcquisitionCost()
        {
            return this.AcquisitionCost(true);
        }

        public double DispositionAmount(bool actual, bool current)
        {
            // actual = 0 if not sold
            // !actual && current = 0 if sale closed; otherwise most recent record date for either (prin + int + addl) or (dispo)
            // !actual && !current = 0 if cancelled or sale closed; otherwise original est
            double dCost = 0;
            if (this.cfCashflows.Count == 0)
            {
                return 0;
            }
            else
            {
                if (actual)
                {
                    foreach (clsCashflow cf in this.cfCashflows)
                    {
                        if ((cf.PayDate() <= System.DateTime.Today) && (cf.Actual()) &&
                            ((cf.TypeID() == clsCashflow.Type.Principal) ||
                             (cf.TypeID() == clsCashflow.Type.InterestHard) ||
                             (cf.TypeID() == clsCashflow.Type.InterestAdditional)))
                        {
                            dCost += cf.Amount();
                        }
                    }
                }
                else if (current)
                {
                    foreach (clsCashflow cf in this.cfCashflows)
                    {
                        if ((cf.DeleteDate().Year == DateTime.MaxValue.Year) &&
                           ((cf.TypeID() == clsCashflow.Type.Principal) ||
                            (cf.TypeID() == clsCashflow.Type.InterestHard) ||
                            (cf.TypeID() == clsCashflow.Type.InterestAdditional) ||
                            (cf.TypeID() == clsCashflow.Type.NetDispositionProj)))
                        {
                            dCost += cf.Amount();
                        }
                    }
                }
                else //!current , i.e. original estimate
                {
                    DateTime dtFirstDispRecord = this.FindDate(clsCashflow.Type.NetDispositionProj, true, false);
                    clsLoan l = this.LoanAsOf(dtFirstDispRecord);
                    foreach (clsCashflow cf in l.cfCashflows)
                    {
                        if (cf.TypeID() == clsCashflow.Type.NetDispositionProj)
                        {
                            dCost = cf.Amount();
                        }
                    }
                }
                return dCost;
            }
        }

        public double OriginalDispositionAmount()
        { return this.DispositionAmount(false, false); }

        public double TotalInvestment()
        {
            return this.Balance() + this.RehabRemain();
        }

        public double FirstRehabEstimate()
        {
            DateTime dtFirstRecording = this.FindDate(clsCashflow.Type.RehabDraw, true, false);
            if (dtFirstRecording == DateTime.MaxValue)
                return 0;
            else
                return this.LoanAsOf(dtFirstRecording).RehabRemain(dtFirstRecording);
        }

        public double ProjectedHardInterest()
        {
            // if there is a principal repayment scheduled, use its scheduled pay date to take loan as of and then accrued to that date
            // if not, check for a projected disposition
            // if neither, return 0
            DateTime dtSale = this.FindDate(clsCashflow.Type.Principal, false, true);
            if (dtSale == DateTime.MinValue) dtSale = this.FindDate(clsCashflow.Type.NetDispositionProj, false, true);
            if (dtSale == DateTime.MinValue)
            {
                return 0;
            }
            else
            {
                clsLoan l = this.LoanAsOf(dtSale);
                return l.AccruedInterest(dtSale);
            }
        }

        public double ProjectedAdditionalInterest()
        { return this.ProjectedAdditionalInterest(System.DateTime.Today); }

        public double ProjectedAdditionalInterest(DateTime dt)
        { return this.ImpliedAdditionalInterest() + this.ScheduledAdditionalInterest(dt); }

        public double ImpliedAdditionalInterest()
        {
            DateTime dtProjDisp = this.FindDate(clsCashflow.Type.NetDispositionProj, false, true);
            if (dtProjDisp == DateTime.MinValue) { return 0; }
            else
            {
                clsLoan l = this.LoanAsOf(dtProjDisp);
                return l.DispositionAmount(true, true) - l.AccruedInterest(dtProjDisp) - l.Balance(dtProjDisp);
            }
        }

        public double AccruedAdditionalInterest(DateTime dt)
        {
            // the idea here is that if principal had been repaid, but additional interest not yet received, 
            // then we can accrue for it 
            // we only count it if it's booked within 30 days of the sale, to filter out escrowed amounts (see property 21 e.g.)
            double dReturnValue = 0;
            if (this.LoanAsOf(dt).Status() == State.Sold)
            {
                DateTime dtSaleDate = this.SaleDate();
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if ((cf.PayDate() > dt)
                        && ((cf.PayDate() - dtSaleDate).Days <= 30)
                        && (cf.TypeID() == clsCashflow.Type.InterestAdditional)
                        && (cf.Actual()))
                    {
                        dReturnValue += cf.Amount();
                    }
                }
            }
            return dReturnValue;
        }

        public double ScheduledAdditionalInterest()
        { return this.ScheduledAdditionalInterest(System.DateTime.Today); }

        public double ScheduledAdditionalInterest(DateTime dt)
        {
            return this._ProjectedToBePaid(dt, clsCashflow.Type.InterestAdditional);
        }

        public double PastDueAdditionalInterest()
        { return this.PastDueAdditionalInterest(System.DateTime.Today); }

        public double PastDueAdditionalInterest(DateTime dt)
        {
            return this._PastDue(dt, clsCashflow.Type.InterestAdditional);
        }

        public double IRR(bool original)
        {
            double dPrevValue;
            double dCurrentValue;
            double dPrevIRR;
            double dCurrentIRR;
            int i = 0;
            double dTolerance = 0.0001;
            int iMaxIterations = 100;
            dPrevIRR = 0.05;
            dCurrentIRR = 0.20;

            if (this.cfCashflows.Count == 0) { return double.NaN; }

            if (!original)
            {
                // GOTTA CHECK FOR WHETHER IT"S BEEN SOLD ALREADY
                dPrevValue = this._NPV(dPrevIRR);
                dCurrentValue = this._NPV(dCurrentIRR);
                while ((i < iMaxIterations) && (Math.Abs(dPrevIRR - dCurrentIRR) > dTolerance))
                {
                    double dNewR = (dPrevValue * dCurrentIRR - dCurrentValue * dPrevIRR) / (dPrevValue - dCurrentValue);
                    dPrevIRR = dCurrentIRR;
                    dPrevValue = dCurrentValue;
                    dCurrentIRR = dNewR;
                    dCurrentValue = this._NPV(dCurrentIRR);
                    i++;
                }
                if (i >= iMaxIterations) { return double.NaN; }
                else { return dCurrentIRR; }
            }
            else
            {
                clsLoan l = this.LoanAsOf(this.FindDate(clsCashflow.Type.NetDispositionProj, true, false));
                dPrevValue = l._NPV(dPrevIRR);
                dCurrentValue = l._NPV(dCurrentIRR);
                while ((i < iMaxIterations) && (Math.Abs(dPrevIRR - dCurrentIRR) > dTolerance))
                {
                    double dNewR = (dPrevValue * dCurrentIRR - dCurrentValue * dPrevIRR) / (dPrevValue - dCurrentValue);
                    dPrevIRR = dCurrentIRR;
                    dPrevValue = dCurrentValue;
                    dCurrentIRR = dNewR;
                    dCurrentValue = l._NPV(dCurrentIRR);
                    i++;
                }
                if (i >= iMaxIterations) { return double.NaN; }
                else { return dCurrentIRR; }

            }
        }

        public double Return(bool original)
        {
            // just (HardInt + AddlInt) / (Balance)
            if (this.cfCashflows.Count == 0) { return double.NaN; }
            clsLoan l;
            DateTime dtAsOf;
            DateTime dtBalanceDate = DateTime.MinValue;  // the day before the principal is repaid
            bool bDisp;
            if (original)
            {
                bDisp = true;
                dtAsOf = this.FindDate(clsCashflow.Type.NetDispositionProj, true, false);
                dtBalanceDate = this.FindDate(clsCashflow.Type.NetDispositionProj, true, true).AddDays(-1);
                l = this.LoanAsOf(dtAsOf);
            }
            else
            {
                dtAsOf = this.FindDate(clsCashflow.Type.NetDispositionProj, false, true);
                if (bDisp = (dtAsOf != DateTime.MinValue))
                {
                    dtAsOf = dtAsOf.AddDays(-1);
                    dtBalanceDate = dtAsOf;
                }
                else
                {
                    dtAsOf = FindDate(clsCashflow.Type.Principal, false, true);
                    if (dtAsOf == DateTime.MinValue)
                    {
                        return double.NaN;
                    }
                    else
                    {
                        dtBalanceDate = dtAsOf.AddDays(-1);
                        if (FindDate(clsCashflow.Type.InterestAdditional, false, true) > dtAsOf)
                        {
                            dtAsOf = FindDate(clsCashflow.Type.InterestAdditional, false, true);
                        }
                    }
                }
                l = this.LoanAsOf(dtAsOf);
            }
            if (bDisp)
            {
                return (l.DispositionAmount(false, !original)) / l.LoanAsOf(dtBalanceDate).TotalInvestment() - 1;
            }
            else
            {
                return l.InterestPaid(dtAsOf) / l.Balance(dtBalanceDate);
            }
        }

        public double GrossReturn(bool original)
        {
            if (this.cfCashflows.Count == 0) { return double.NaN; }
            clsLoan l;
            DateTime dtAsOf;
            bool bDisp;
            DateTime dtBalanceDate = DateTime.MaxValue;
            if (original)
            {
                // find the record date of the first projected disposition amount, take the loan as of that date
                dtAsOf = this.FindDate(clsCashflow.Type.NetDispositionProj, true, false);
                bDisp = true;
                l = this.LoanAsOf(dtAsOf);
            }
            else
            {
                // find the pay date of the last disposition date, or projected principal payment
                dtAsOf = this.FindDate(clsCashflow.Type.NetDispositionProj, false, true);
                if (bDisp = (dtAsOf != DateTime.MinValue))
                {
                    dtAsOf = dtAsOf.AddDays(-1);
                    dtBalanceDate = dtAsOf;
                }
                else
                {
                    dtAsOf = FindDate(clsCashflow.Type.Principal, false, true);
                    if (dtAsOf == DateTime.MinValue)
                    {
                        return double.NaN;
                    }
                    else
                    {
                        dtBalanceDate = dtAsOf.AddDays(-1);
                        if (FindDate(clsCashflow.Type.InterestAdditional, false, true) > dtAsOf)
                        {
                            dtAsOf = FindDate(clsCashflow.Type.InterestAdditional, false, true);
                        }
                    }
                }
                l = this.LoanAsOf(dtAsOf);
            }
            if (bDisp)
            {
                return (l.DispositionAmount(false, true) + l.ImpliedAdditionalInterest()) / l.LoanAsOf(FindDate(clsCashflow.Type.NetDispositionProj, true, true).AddDays(-1)).TotalInvestment() - 1D;
            }
            else
            {
                return (l.HardInterestPaid(dtAsOf) + 2 * l.AdditionalInterestPaid(dtAsOf)) / l.Balance(dtBalanceDate);
            }
        }
        #endregion

        #region Private Calculations
        private double _Paid(DateTime dt, clsCashflow.Type t)
        {
            double dPaid = 0;
            if (this.cfCashflows.Count == 0)
            {
                return 0;
            }
            else
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if ((cf.PayDate() <= dt) && (cf.Actual()) && (cf.TypeID() == t))
                    {
                        dPaid += cf.Amount();
                    }
                }
                return dPaid;
            }
        }

        private double _ProjectedToBePaid(DateTime dt, clsCashflow.Type t)
        {
            double dRemain = 0D;
            if (this.cfCashflows.Count == 0)
            {
                return 0;
            }
            else
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if ((cf.PayDate() > dt) && (cf.DeleteDate() > dt) && (cf.TypeID() == t))
                    {
                        dRemain += cf.Amount();
                    }
                }
                return dRemain;
            }
        }

        private double _PastDue(DateTime dt, clsCashflow.Type t)
        {
            double dPastDue = 0D;
            if (this.cfCashflows.Count == 0)
                return 0;
            else
            {
                foreach (clsCashflow cf in this.cfCashflows)
                {
                    if ((cf.PayDate() <= dt) && (cf.DeleteDate() > dt.AddYears(100)) && (cf.TypeID() == t) && (!cf.Actual()))
                        dPastDue += cf.Amount();
                }
                return dPastDue;
            }
        }

        private double _NPV(double r)
        {
            double dRetVal = 0;
            double dMonths;
            DateTime dtSale = FindDate(clsCashflow.Type.Principal, false, true);
            if (dtSale == DateTime.MinValue) { dtSale = FindDate(clsCashflow.Type.NetDispositionProj, false, true); }
            if (dtSale == DateTime.MinValue) { return double.NaN; }
            foreach (clsCashflow cf in this.cfCashflows)
            {
                if (cf.DeleteDate().Year == DateTime.MaxValue.Year)
                {
                    dMonths = (dtSale - cf.PayDate()).Days * (12D / 365D);
                    dRetVal += cf.Amount() * Math.Pow((1D + r / 12D), dMonths);
                }
            }
            return dRetVal;
        }

        private int _NewLoanID()
        {
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            return tbl.Length();
        }
        #endregion
    }
}
