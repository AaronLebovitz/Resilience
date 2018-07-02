using System;
using AppKit;
using Foundation;
using ResilienceClasses;

namespace LoanStatusReport
{
    public partial class ViewController : NSViewController
    {
        DateTime dtStartDateChosen;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            this.ReportDatePicker.DateValue = (NSDate)System.DateTime.Today;
            this.ReportDatePicker2.DateValue = (NSDate)System.DateTime.Today.AddDays(91);
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        #region Event Handlers

        partial void RunPeriodReportButtonPushed(AppKit.NSButton sender)
        {
            this.UpdateLabel.StringValue = "Working...";
            DateTime dt1 = (DateTime)ReportDatePicker.DateValue;
            DateTime dt2 = (DateTime)ReportDatePicker2.DateValue;
            if (dt2 == dt1) { dt2 = dt2.AddDays(1); }
            else if (dt2 < dt1)
            {
                dt2 = dt1;
                dt1 = (DateTime)ReportDatePicker2.DateValue;
            }
            this.RunLoanStatusReport(dt1);
            this.RunLoanStatusReport(dt2);
            this.CalculateAggregates(dt1, dt2);
            this.UpdateLabel.StringValue = "Completed.";
        }

        partial void DateChosen(AppKit.NSDatePicker sender)
        {
            if ((DateTime)this.ReportDatePicker.DateValue != dtStartDateChosen)
            {
                dtStartDateChosen = (DateTime)this.ReportDatePicker.DateValue;
                this.ReportDatePicker2.DateValue = (NSDate)(((DateTime)this.ReportDatePicker.DateValue).AddMonths(3));
            }
        }

        partial void RunReportButtonPushed(AppKit.NSButton sender)
        {
            this.UpdateLabel.StringValue = "Working...";
            this.RunLoanStatusReport((DateTime)(this.ReportDatePicker.DateValue));
            this.UpdateLabel.StringValue = "Completed.";
        }

        #endregion

        private void CalculateAggregates(DateTime startDate, DateTime endDate)
        {
            // Balances:  Total made, less each quarters repayment, adding to Net Balance
            double dTotalLent = 0D;
            double dRepaidPeriod = 0D;
            double dRepaidPreviously = 0D;
            double dNetBalance = 0D;
            double dTotalCommitted = 0D;
            double dTotalRehabRemain = 0D;
            double dSaleContracts = 0D;
            double dSaleListings = 0D;
            double dPendingAcq = 0D;

            // Accrued:   Total accrued FTD, less this period payments, less prior payments, (=accrued end of period), 
            //            less accrued end of prev period,  adding to net accrued this period
            //            (+) paid this period (+) additional interest paid this period (+) add'l accrued this period = Net Income Period
            double dHardInterestPaidThisPeriod = 0D;
            double dHardInterestPaidPreviously = 0D;
            double dAccruedThisPeriod = 0D;
            double dAdditionalInterestPaidThisPeriod = 0D;
            double dAdditionalInterestAccruedThisPeriod = 0D;
            double dAdditionalInterestAccruedPreviousPeriod = 0D;
            double dAccruedFTD = 0D;
            double dAccruedNetAtStartDate = 0D;

            // Loan-by-loan:
            //            (+) Total Accrual FTD (+) interest paid this period (-) interest paid previous periods
            //            (+) Additional Paid this period (+) additional accrued this period (-) additional accrued last period
            int iStateCount = Enum.GetNames(typeof(clsLoan.State)).Length;
            int[,] iLoansByStatus = new int[iStateCount,2];
            int[] iUncancelledCount = new int[2];
            for (int i = 0; i < iStateCount; i++) { iLoansByStatus[i, 0] = iLoansByStatus[i, 1] = 0; }

            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                clsLoan loan = new clsLoan(i);
                clsLoan loanAsOfEndDate = loan.LoanAsOf(endDate);
                if (loan.FindDate(clsCashflow.Type.AcquisitionPrice, true, false) <= endDate)
                {
                    iLoansByStatus[(int)loan.LoanAsOf(startDate).Status(), 0]++;
                    iLoansByStatus[(int)loanAsOfEndDate.Status(), 1]++;
                    dTotalLent += loan.Balance(endDate) + loan.PrincipalPaid(endDate);
                    dTotalRehabRemain += loanAsOfEndDate.RehabRemain(endDate);
                    dRepaidPeriod += loan.PrincipalPaid(endDate) - loan.PrincipalPaid(startDate);
                    dRepaidPreviously += loan.PrincipalPaid(startDate);
                    dHardInterestPaidPreviously += loan.HardInterestPaid(startDate);
                    dHardInterestPaidThisPeriod += loan.HardInterestPaid(endDate) - loan.HardInterestPaid(startDate);
                    dAccruedThisPeriod += loan.AccruedInterest(endDate) - loan.AccruedInterest(startDate);
                    dAdditionalInterestPaidThisPeriod += loan.AdditionalInterestPaid(endDate) - loan.AdditionalInterestPaid(startDate);
                    dAdditionalInterestAccruedThisPeriod += loan.AccruedAdditionalInterest(endDate);  // needs fixing 
                    dAdditionalInterestAccruedPreviousPeriod += loan.AccruedAdditionalInterest(startDate);  // needs fixing 
                    dAccruedFTD += loan.AccruedInterest(endDate) + loan.HardInterestPaid(endDate);
                    dAccruedNetAtStartDate += loan.AccruedInterest(startDate);
                    if (loanAsOfEndDate.Status() == clsLoan.State.Listed) { dSaleListings += loan.Balance(endDate); }
                    if (loanAsOfEndDate.Status() == clsLoan.State.PendingSale) { dSaleContracts += loan.Balance(endDate); }
                    if (loanAsOfEndDate.Status() == clsLoan.State.PendingAcquisition) { dPendingAcq += loan.AcquisitionCost(false); }
                }
            }
            dTotalCommitted = dTotalLent + dTotalRehabRemain + dPendingAcq;
            dNetBalance = dTotalLent - dRepaidPeriod - dRepaidPreviously;
            for (int i = 0; i < 2; i++)
            {
                iUncancelledCount[i] = iLoansByStatus[(int)clsLoan.State.Listed, i];
                iUncancelledCount[i] += iLoansByStatus[(int)clsLoan.State.PendingAcquisition, i];
                iUncancelledCount[i] += iLoansByStatus[(int)clsLoan.State.PendingSale, i];
                iUncancelledCount[i] += iLoansByStatus[(int)clsLoan.State.Rehab, i];
                iUncancelledCount[i] += iLoansByStatus[(int)clsLoan.State.Sold, i];
            }
            int iRepaidCount = iLoansByStatus[(int)clsLoan.State.Sold, 1] - iLoansByStatus[(int)clsLoan.State.Sold, 0];
            int iOutstandingCount = iUncancelledCount[1] - iLoansByStatus[(int)clsLoan.State.Sold, 1];

            // Display Results
            this.ReportSummaryTextLabel.StringValue = "";

            this.ReportSummaryTextLabel.StringValue += " Total Committed:    \t" + dTotalCommitted.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + iUncancelledCount[1].ToString("000") + ")" + "\t";

            this.ReportSummaryTextLabel.StringValue += "\n Total Loan Given:   \t" + dTotalLent.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + (iUncancelledCount[1] - iLoansByStatus[(int)clsLoan.State.PendingAcquisition, 1]).ToString("000") + ")" + "\t";
            this.ReportSummaryTextLabel.StringValue += "FTD Accrual:    \t" + dAccruedFTD.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n Repaid Period:     \t" + dRepaidPeriod.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + iRepaidCount.ToString("000") + ")" + "\t";
            this.ReportSummaryTextLabel.StringValue += "(-)Paid Period: \t" + dHardInterestPaidThisPeriod.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n Repaid Previously: \t" + dRepaidPreviously.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + iLoansByStatus[(int)clsLoan.State.Sold, 0].ToString("000") + ")" + "\t";
            this.ReportSummaryTextLabel.StringValue += "(-)Paid Prev:   \t" + dHardInterestPaidPreviously.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n Loans Outstanding: \t" + dNetBalance.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + iOutstandingCount.ToString("000") + ")" + "\t";
            this.ReportSummaryTextLabel.StringValue += "Net Accrued:    \t" + (dAccruedFTD - dHardInterestPaidThisPeriod - dHardInterestPaidPreviously).ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t";
            this.ReportSummaryTextLabel.StringValue += "(-)Start Accr:  \t" + dAccruedNetAtStartDate.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n Sale Contracts     \t" + dSaleContracts.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + iLoansByStatus[(int)clsLoan.State.PendingSale, 1].ToString("000") + ")" + "\t";
            this.ReportSummaryTextLabel.StringValue += "Period Accr:    \t" + (dAccruedFTD - dHardInterestPaidThisPeriod - dHardInterestPaidPreviously - dAccruedNetAtStartDate).ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n Sale Listings      \t" + dSaleListings.ToString("00,000,000.00");
            this.ReportSummaryTextLabel.StringValue += "(" + iLoansByStatus[(int)clsLoan.State.Listed, 1].ToString("000") + ")" + "\t";
            this.ReportSummaryTextLabel.StringValue += "                           \t";
            this.ReportSummaryTextLabel.StringValue += "(+) Addl Paid  : \t" + dAdditionalInterestPaidThisPeriod.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t                           \t";
            this.ReportSummaryTextLabel.StringValue += "(+) Addl Accrue: \t" + dAdditionalInterestAccruedThisPeriod.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t                           \t";
            this.ReportSummaryTextLabel.StringValue += "(-) Prior Accru: \t" + dAdditionalInterestAccruedPreviousPeriod.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t";
            this.ReportSummaryTextLabel.StringValue += "(+) Additional:  \t" + (dAdditionalInterestPaidThisPeriod+dAdditionalInterestAccruedThisPeriod-dAdditionalInterestAccruedPreviousPeriod).ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t";
            this.ReportSummaryTextLabel.StringValue += "(+) Hard Paid :  \t" + dHardInterestPaidThisPeriod.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t";
            this.ReportSummaryTextLabel.StringValue += "Net Income Per:  \t" + (dAccruedFTD - dHardInterestPaidPreviously - dAccruedNetAtStartDate + dAdditionalInterestPaidThisPeriod + dAdditionalInterestAccruedThisPeriod - dAdditionalInterestAccruedPreviousPeriod).ToString("000,000.00");

        }

        private void RunLoanStatusReport(DateTime dtReport)
        {
            // create report file
            string fileName = "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/LoanStatus";
            fileName += dtReport.ToString("yyyyMMdd");
            fileName += ".txt";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);

            // write header
            sw.WriteLine("Property,State,Principal Balance,Accrued Interest,Proj Hard Int,Proj Additional Int,Proj Return,Proj IRR,Principal  Paid,Interest  Paid,Addl Int  Paid,Act Return,Act IRR,Rehab Remain,Rehab  Spent,Sale Date,Purchase Date,Days");

            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                this.WriteLoan(i, dtReport, sw);
            }
            sw.Close();
        }

        private void WriteLoan(int loanID, DateTime dtAsOf, System.IO.StreamWriter sw)
        {
            clsEntity titleHolder;
            clsEntity coBorrower;
            clsEntity titleCompany;

            clsLoan loan = new clsLoan(loanID).LoanAsOf(dtAsOf);
            titleHolder = new clsEntity(loan.TitleHolderID());
            titleCompany = new clsEntity(loan.TitleCompanyID());
            coBorrower = new clsEntity(loan.CoBorrowerID());
            clsLoan.State eStatus = loan.Status();

            sw.Write(loan.Property().Address() + ",");
            sw.Write(eStatus.ToString().ToUpper() + ",");

            if (eStatus == clsLoan.State.Cancelled)
            {
                sw.Write(",,,,,,,,,,,,,,,,");
            }
            else
            {
                if (loan.Status() == clsLoan.State.Sold)
                {
                    sw.Write(",,,,,,");
                    sw.Write(loan.PrincipalPaid(dtAsOf).ToString() + ",");
                    sw.Write(loan.InterestPaid(dtAsOf).ToString() + ",");
                    sw.Write(loan.AdditionalInterestPaid(dtAsOf).ToString() + ",");
                    sw.Write(loan.Return(false).ToString() + ",");
                    sw.Write(loan.IRR(false).ToString() + ",");
                }
                else
                {
                    sw.Write(loan.Balance(dtAsOf).ToString() + ",");
                    sw.Write(loan.AccruedInterest(dtAsOf).ToString() + ",");
                    sw.Write(loan.ProjectedHardInterest().ToString() + ",");
                    sw.Write(loan.ProjectedAdditionalInterest(dtAsOf).ToString() + ",");
                    sw.Write(loan.Return(false).ToString() + ",");
                    sw.Write(loan.IRR(false).ToString() + ",");
                    sw.Write(",,,,,");
                }
                sw.Write(loan.RehabRemain(dtAsOf).ToString() + ",");
                sw.Write(loan.RehabSpent(dtAsOf).ToString() + ",");
                sw.Write(loan.SaleDate().ToShortDateString() + ",");
                sw.Write(loan.OriginationDate().ToShortDateString() + ",");
                sw.Write((loan.SaleDate() - loan.OriginationDate()).TotalDays.ToString() + ",");
            }

            sw.Write(titleHolder.Name() + ",");
            sw.Write(coBorrower.Name() + ",");
            sw.Write(titleCompany.Name() + ",");
            sw.Write(loan.Rate().ToString() + ",");
            sw.Write(loan.PenaltyRate().ToString() + ",");
            sw.Write(loan.OriginationDate().ToShortDateString() + ",");
            sw.Write(loan.MaturityDate().ToShortDateString() + ",");
            sw.Write(loan.GrossReturn(false).ToString() + ",");
            sw.Write(loan.GrossReturn(true).ToString() + ",");
            sw.Write(loan.Return(true).ToString() + ",");
            sw.Write(loan.IRR(true).ToString() + ",");
            sw.Write(loan.FirstRehabEstimate().ToString() + ",");

            sw.WriteLine();
            sw.Flush();

        }

    }
}
