using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

namespace ResilienceClasses
{
    public class clsLoan
    {
        public static string strLoanPath = "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/tblLoan.csv";
        public static int IndexColumn = 0;
        public static int PropertyColumn = 1;
        public static int TitleHolderColumn = 2;
        public static int CoBorrowerColumn = 3;
        public static int TitleCompanyColumn = 4;
        public static int OGDateColumn = 5;
        public static int MaturityDateCoumn = 6;
        public static int RateColumn = 7;
        public static int PenaltyRateColumn = 8;

        public enum State { Unknown, Cancelled, PendingAcquisition, Rehab, Listed, PendingSale, Sold }

        // public static methods
        public static int LoanID(string address)
        {
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            clsCSVTable tblProperties = new clsCSVTable(clsProperty.strPropertyPath);
            int propertyID = -1;
            int loanID = -1;
            // Find PropertyID from Address First
            for (int i = 0; i < tblProperties.Length(); i++)
            {
                if (tblProperties.Value(i, clsProperty.AddressColumn) == address) propertyID = i;
            }
            if (propertyID == -1) throw new Exception("Address not found: " + address);
            // Now match propertyID to the loan
            for (int i = 0; i < tblLoans.Length(); i++)
            {
                if (tblLoans.Value(i, clsLoan.PropertyColumn) == propertyID.ToString()) loanID = i;
            }
            if (loanID == -1) throw new Exception("No Loan Found for property " + address);
            return loanID;
        }

        // properties
        private int iLoanID;
        private int iPropertyID;
        private int iTitleHolderEntityID;
        private int iCoBorrowerEntityID;
        private int iAcquisitionTitleCompanyEntityID;
        private List<clsCashflow> cfCashflows;
        private string strName;
        private clsProperty pProperty;
        private DateTime dtOrigination;
        private DateTime dtMaturity;
        private double dRate;
        private double dPenaltyRate;

        // Constructors and builders

        public clsLoan(int loanID)
        {
            this._Load(loanID);
        }

        public clsLoan(int propertyID, int titleHolderID, int coBorrowerID, int titleCoID,
                       DateTime orig, DateTime mature, double r, double pr, int loanID = 0)
        {
            this.iLoanID = loanID;
            this.iPropertyID = propertyID;
            this.iTitleHolderEntityID = titleHolderID;
            this.iCoBorrowerEntityID = coBorrowerID;
            this.iAcquisitionTitleCompanyEntityID = titleCoID;
            this.dtMaturity = mature;
            this.dtOrigination = orig;
            this.dRate = r;
            this.dPenaltyRate = pr;
            this.pProperty = new clsProperty(propertyID);
            this.cfCashflows = new List<clsCashflow>();
        }

        public clsLoan LoanAsOf(DateTime dt)
        {
            clsLoan newLoan = new clsLoan(this.iPropertyID, this.iTitleHolderEntityID, this.iCoBorrowerEntityID,
                                          this.iAcquisitionTitleCompanyEntityID, this.dtOrigination, this.dtMaturity,
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

        public bool AddCashflow(clsCashflow cf)
        {
            if (cf.LoanID() == this.iLoanID)
            {
                this.cfCashflows.Add(cf);
                return true;
            }
            else return false;
        }

        // private constructor and builder support

        private bool _Load(int loanID)
        {
            return this._Load(loanID, clsLoan.strLoanPath);
        }

        private bool _Load(int loanID, string path)
        {
            this.iLoanID = loanID;
            clsCSVTable tbl = new clsCSVTable(path);
            if (loanID < tbl.Length())
            {
                this.iPropertyID = Int32.Parse(tbl.Value(loanID, clsLoan.PropertyColumn));
                this.iTitleHolderEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.TitleHolderColumn));
                this.iCoBorrowerEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.CoBorrowerColumn));
                this.iAcquisitionTitleCompanyEntityID = Int32.Parse(tbl.Value(loanID, clsLoan.TitleCompanyColumn));
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

        // Get Properties
        public DateTime OriginationDate() { return this.dtOrigination; }
        public DateTime MaturityDate() { return this.dtMaturity; }
        public double Rate() { return this.dRate; }
        public double PenaltyRate() { return this.dPenaltyRate; }
        public int TitleHolderID() { return this.iTitleHolderEntityID; }
        public int CoBorrowerID() { return this.iCoBorrowerEntityID; }
        public int TitleCompanyID() { return this.iAcquisitionTitleCompanyEntityID; }
        public int PropertyID() { return this.iPropertyID; }
        public int ID() { return this.iLoanID; }
        public clsProperty Property() { return this.pProperty; }
        public List<clsCashflow> Cashflows() { return this.cfCashflows; }

        // Calculations

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
                    dExpiredDays = Math.Max(0, Math.Min((dt - this.dtMaturity).TotalDays, (dt - cf.PayDate()).TotalDays));
                    if ((cf.PayDate() < dt) && (cf.Actual()))
                    {
                        dAccrued += cf.Amount() * this.dRate * (dt - cf.PayDate()).TotalDays / 360;
                        dAccrued += cf.Amount() * this.dPenaltyRate * dExpiredDays / 360;
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
                return -l.Balance(l.FindDate(clsCashflow.Type.AcquisitionPrice, true, true));
            }
            else
            {
                clsLoan l = this.LoanAsOf(this.dtOrigination);
                return -l.Balance(this.dtOrigination);
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
        { return this.ImpliedAdditionalInterest() + this.ScheduledAdditionalInterest(); }

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

        public double ScheduledAdditionalInterest()
        { return this.ScheduledAdditionalInterest(System.DateTime.Today); }

        public double ScheduledAdditionalInterest(DateTime dt)
        {
            return this._ProjectedToBePaid(dt, clsCashflow.Type.InterestAdditional);
        }

        public double IRR(bool original)
        {
            double dPrevValue;
            double dCurrentValue;
            double dPrevIRR;
            double dCurrentIRR;
            int i=0;
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
                return (l.DispositionAmount(false, true) + l.ImpliedAdditionalInterest()) / l.LoanAsOf(FindDate(clsCashflow.Type.NetDispositionProj,true,true).AddDays(-1)).TotalInvestment() - 1D;
            }
            else 
            {
                return (l.HardInterestPaid(dtAsOf) + 2 * l.AdditionalInterestPaid(dtAsOf)) / l.Balance(dtBalanceDate);
            }
        }

        // private calculation support

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
            double dRemain = 0;
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
    }
}
