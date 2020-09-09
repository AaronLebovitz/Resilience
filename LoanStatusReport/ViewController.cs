using System;
using AppKit;
using Foundation;
using ResilienceClasses;
using System.Collections.Generic;

namespace LoanStatusReport
{
    public partial class ViewController : NSViewController
    {
        DateTime dtStartDateChosen;
        private int lenderID = -1;
        private List<int> lenderIndexToID = new List<int>();
        private List<int> lenderLoanIDs = new List<int>();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            this.ReportDatePicker.DateValue = (NSDate)System.DateTime.Today.Date.ToUniversalTime();
            this.ReportDatePicker2.DateValue = (NSDate)System.DateTime.Today.Date.AddDays(91).ToUniversalTime();

            clsCSVTable tblLenders = new clsCSVTable(clsEntity.strEntityPath);
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tblLenders.Length(); i++)
            {
                if (tblLoans.Matches(clsLoan.LenderColumn, i.ToString()).Count > 0)
                {
                    this.LenderPopUpButton.AddItem(tblLenders.Value(i, clsEntity.NameColumn));
                    this.lenderIndexToID.Add(i);
                }
            }
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
            DateTime dt1 = ((DateTime)ReportDatePicker.DateValue).Date.ToUniversalTime();
            DateTime dt2 = ((DateTime)ReportDatePicker2.DateValue).Date.ToUniversalTime();
            if (dt2 == dt1) { dt2 = dt2.AddDays(1); }
            else if (dt2 < dt1)
            {
                dt2 = dt1;
                dt1 = (DateTime)ReportDatePicker2.DateValue;
            }
            //this.RunLoanStatusReport(dt1);
            //this.RunLoanStatusReport(dt2);
            this.RunLoanStatusReportHTML(dt1);
            this.RunLoanStatusReportHTML(dt2);
            this.CalculateAggregates(dt1, dt2);
            this.UpdateLabel.StringValue = "Completed.";
        }

        partial void RunAnnualReportButtonPushed(NSButton sender)
        {
            this.UpdateLabel.StringValue = "Working...";
            DateTime dt1 = ((DateTime)ReportDatePicker.DateValue).AddHours(-5).Date.ToUniversalTime();
            DateTime dt2 = ((DateTime)ReportDatePicker2.DateValue).AddHours(-5).Date.ToUniversalTime();
            if (dt2 == dt1) { dt2 = dt2.AddDays(1); }
            else if (dt2 < dt1)
            {
                dt2 = dt1;
                dt1 = (DateTime)ReportDatePicker2.DateValue;
            }
            this.RunLoanAuditReportHTML(dt1, dt2);
            this.UpdateLabel.StringValue = "Completed.";
        }

        partial void DateChosen(AppKit.NSDatePicker sender)
        {
            if (((DateTime)this.ReportDatePicker.DateValue).Date != dtStartDateChosen)
            {
                dtStartDateChosen = ((DateTime)this.ReportDatePicker.DateValue).Date;
                DateTime testDate = (DateTime)this.ReportDatePicker.DateValue;
                testDate = testDate.Date.AddDays(1);
                testDate = testDate.AddMonths(3);
                testDate = testDate.AddDays(-1);
                this.ReportDatePicker2.DateValue = (NSDate)(((DateTime)this.ReportDatePicker.DateValue).Date.AddDays(1).AddMonths(3).AddDays(-1).ToUniversalTime());
            }
        }

        partial void RunReportButtonPushed(AppKit.NSButton sender)
        {
            this.UpdateLabel.StringValue = "Working...";
            //this.RunLoanStatusReport((DateTime)(this.ReportDatePicker.DateValue));
            this.RunLoanStatusReportHTML((DateTime)(this.ReportDatePicker.DateValue));
            this.UpdateLabel.StringValue = "Completed.";
        }

        partial void LenderChosen(NSPopUpButton sender)
        {
            this.lenderLoanIDs.Clear();
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            if (this.LenderPopUpButton.IndexOfSelectedItem > 0)
            {
                this.lenderID = this.lenderIndexToID[(int)this.LenderPopUpButton.IndexOfSelectedItem - 1];
                this.lenderLoanIDs = tblLoans.Matches(clsLoan.LenderColumn, this.lenderID.ToString());
            }
            else
            {
                this.lenderID = -1;
                for (int i = 0; i < tblLoans.Length(); i++)
                    this.lenderLoanIDs.Add(i);
            }
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
            double dPartialBalance = 0D;

            // Accrued:   Total accrued FTD, less this period payments, less prior payments, (=accrued end of period), 
            //            less accrued end of prev period,  adding to net accrued this period
            //            (+) paid this period (+) additional interest paid this period (+) add'l accrued this period = Net Income Period
            double dHardInterestPaidThisPeriod = 0D;
            double dHardInterestPaidPreviously = 0D;
            double dAccruedThisPeriod = 0D;
            double dAdditionalInterestPaidThisPeriod = 0D;
            double dAdditionalInterestAccruedThisPeriod = 0D;
            double dAdditionalInterestAccruedPreviousPeriod = 0D;
            double dPointsPaidThisPeriod = 0D;
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
                if (this.lenderLoanIDs.Contains(i))
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
                        dPointsPaidThisPeriod += loan.PointsPaid(endDate) - loan.PointsPaid(startDate);
                        dAccruedThisPeriod += loan.AccruedInterest(endDate) - loan.AccruedInterest(startDate);
                        dAdditionalInterestPaidThisPeriod += loan.AdditionalInterestPaid(endDate) - loan.AdditionalInterestPaid(startDate);
                        dAdditionalInterestAccruedThisPeriod += loan.AccruedAdditionalInterest(endDate);  // needs fixing 
                        dAdditionalInterestAccruedPreviousPeriod += loan.AccruedAdditionalInterest(startDate);  // needs fixing 
                        dAccruedFTD += loan.AccruedInterest(endDate) + loan.HardInterestPaid(endDate);
                        dAccruedNetAtStartDate += loan.AccruedInterest(startDate);
                        if (loanAsOfEndDate.Status() == clsLoan.State.Listed) { dSaleListings += loan.Balance(endDate); }
                        if (loanAsOfEndDate.Status() == clsLoan.State.PendingSale) { dSaleContracts += loan.Balance(endDate); }
                        if (loanAsOfEndDate.Status() == clsLoan.State.PendingAcquisition) { dPendingAcq += loan.AcquisitionCost(false); }
                        if (loanAsOfEndDate.Status() == clsLoan.State.PartiallySold) { dPartialBalance += loan.Balance(endDate); }
                    }
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
                iUncancelledCount[i] += iLoansByStatus[(int)clsLoan.State.PartiallySold, i];
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
            this.ReportSummaryTextLabel.StringValue += "(+) Points Paid :\t" + dPointsPaidThisPeriod.ToString("000,000.00");

            this.ReportSummaryTextLabel.StringValue += "\n                    \t                   \t";
            this.ReportSummaryTextLabel.StringValue += "Net Income Per:  \t" + (dAccruedFTD - dHardInterestPaidPreviously - dAccruedNetAtStartDate + dAdditionalInterestPaidThisPeriod + dAdditionalInterestAccruedThisPeriod - dAdditionalInterestAccruedPreviousPeriod + dPointsPaidThisPeriod).ToString("000,000.00");

        }

        #region Old (csv) Reports

        private void RunAnnualLoanAuditReport(DateTime dtStart, DateTime dtEnd)
        {
            // create report file
            string fileName = "/Volumes/GoogleDrive/Shared Drives/Resilience/Reports/LoanAudit";
            fileName += dtEnd.ToString("yyyyMMdd");
            fileName += "." + this.LenderPopUpButton.TitleOfSelectedItem;
            fileName += ".txt";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);

            // write header
            sw.WriteLine("Property,Begin Balance,Additions,Repayments,End Balance,Accrued Interest,Interest Paid");

            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (this.lenderLoanIDs.Contains(i))
                    this.WriteLoanAudit(i, dtStart, dtEnd, sw);
            }
            sw.Close();

        }

        private void RunLoanStatusReport(DateTime dtReport)
        {
            // create report file
            string fileName = "/Volumes/GoogleDrive/Shared Drives/Resilience/Reports/LoanStatus";
            fileName += dtReport.ToString("yyyyMMdd");
            fileName += "." + this.LenderPopUpButton.TitleOfSelectedItem;
            fileName += ".txt";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);

            // write header
            sw.WriteLine("Property,State,Principal Balance,Accrued Interest,Proj Hard Int,Proj Additional Int,Proj Return,Proj IRR,Principal  Paid,Interest  Paid,Addl Int  Paid,Act Return,Act IRR,Rehab Remain,Rehab  Spent,Sale Date,Purchase Date,Days");

            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (this.lenderLoanIDs.Contains(i))
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
                    sw.Write(",,,");
                    if (loan.AccruedAdditionalInterest(dtAsOf) > 0D) sw.Write(loan.AccruedAdditionalInterest(dtAsOf));
                    sw.Write(",,,");
                    sw.Write(loan.PrincipalPaid(dtAsOf).ToString() + ",");
                    sw.Write(loan.InterestPaid(dtAsOf).ToString() + ",");
                    sw.Write(loan.AdditionalInterestPaid(dtAsOf).ToString() + ",");
                    sw.Write(loan.Return(false).ToString() + ",");
                    sw.Write(loan.IRR(false).ToString() + ",");
                }
                else if (loan.Status() == clsLoan.State.PartiallySold)
                {
                    sw.Write(loan.Balance(dtAsOf).ToString() + ",");
                    sw.Write(loan.AccruedInterest(dtAsOf).ToString() + ",");
                    sw.Write(loan.ProjectedHardInterest().ToString() + ",");
                    if (loan.AccruedAdditionalInterest(dtAsOf) > 0D) sw.Write(loan.AccruedAdditionalInterest(dtAsOf));
                    sw.Write(",");
                    sw.Write(loan.Return(false).ToString() + ",");
                    sw.Write(loan.IRR(false).ToString() + ",");
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

        private void WriteLoanAudit(int loanID, DateTime dtStart, DateTime dtEnd, System.IO.StreamWriter streamWriter)
        {
            clsLoan loanEnd = new clsLoan(loanID).LoanAsOf(dtEnd);
            clsLoan loanStart = new clsLoan(loanID).LoanAsOf(dtStart);

            if ((loanEnd.Status() != clsLoan.State.Cancelled) && (loanStart.Status() != clsLoan.State.Sold))
            {
                streamWriter.Write(loanEnd.Property().Address() + ",");
                streamWriter.Write(loanStart.Balance(dtStart).ToString() + ",");
                double dAdditions = loanEnd.Balance(dtEnd) - loanStart.Balance(dtStart) + loanEnd.PrincipalPaid(dtEnd);
                streamWriter.Write(dAdditions.ToString() + ",");
                streamWriter.Write(loanEnd.PrincipalPaid(dtEnd).ToString() + ",");
                streamWriter.Write(loanEnd.Balance(dtEnd).ToString() + ",");
                streamWriter.Write(loanEnd.AccruedInterest(dtEnd).ToString() + ",");
                streamWriter.WriteLine(loanEnd.InterestPaid(dtEnd).ToString());
                streamWriter.Flush();
            }
        }

        #endregion

        #region HTML Reports

        private void RunLoanAuditReportHTML(DateTime dtStart, DateTime dtEnd)
        {
            // create report file
            List<double> totals = new List<double>();
            for (int i = 0; i < 7; i++)
                totals.Add(0D);
            string fileName = "/Volumes/GoogleDrive/Shared Drives/Resilience/Reports/LoanAudit";
            fileName += dtEnd.ToString("yyyyMMdd");
            fileName += "." + this.LenderPopUpButton.TitleOfSelectedItem;
            fileName += ".htm";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);

            // write beginning tags and header
            sw.WriteLine("<!DOCTYPE html><html>");
            sw.WriteLine("<head>");
            sw.WriteLine("<style>");
            sw.WriteLine("th {text-decoration:underline;}");
            sw.WriteLine("tr {background-color:WhiteSmoke;}");
            sw.WriteLine("tr#ROW2 {background-color:LightGray;}");
            sw.WriteLine("tr#HEADER {background-color:White;}");
            sw.WriteLine("</style>");
            sw.WriteLine("</head>");

            // html body
            sw.WriteLine("<body>");
            sw.WriteLine("<h1>Loan Audit Report for the Year Ending " + dtEnd.ToString("MM/dd/yyyy") + "</h1>");
            sw.WriteLine("<table style=\"width:auto\">");
            WriteHTMLHeaderRowAudit(sw);

            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (this.lenderLoanIDs.Contains(i))
                {
                    string rowID = "";
                    if ((i % 3) == 2) rowID = "ROW2";
                    this.WriteLoanAuditHTML(i, dtStart, dtEnd, sw, totals, rowID);
                }
            }

            // end of doc tags
            sw.WriteLine("<tr><td>TOTALS</td></tr>");
            sw.Write("<tr text-decoration:underline><td></td>");
            foreach (double d in totals)
                sw.Write("<td align=\"right\"><b><u>" + d.ToString("#,##0.00") + "</u></b></td>");
            sw.WriteLine("</tr></table></html></body>");
            sw.Close();

        }

        private void RunLoanStatusReportHTML(DateTime dtReport)
        {
            // create report file
            List<double> totals = new List<double>();
            for (int i = 0; i < 14; i++)
                totals.Add(0D);
            string fileName = "/Volumes/GoogleDrive/Shared Drives/Resilience/Reports/LoanStatus";
            fileName += dtReport.ToString("yyyyMMdd");
            fileName += "." + this.LenderPopUpButton.TitleOfSelectedItem;
            fileName += ".htm";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);

            // write beginning tags and header
            sw.WriteLine("<!DOCTYPE html><html>");
            sw.WriteLine("<head>");
            sw.WriteLine("<style>");
            sw.WriteLine("th {text-decoration:underline;}");
            sw.WriteLine("tr {background-color:WhiteSmoke;}");
            sw.WriteLine("tr#ROW2 {background-color:LightGray;}");
            sw.WriteLine("tr#HEADER {background-color:White;}");
            sw.WriteLine("</style>");
            sw.WriteLine("</head>");

            // html body
            sw.WriteLine("<body>");
            sw.WriteLine("<h1>Loan Status Report  " + dtReport.ToString("MM/dd/yyyy") + "</h1>");
            sw.WriteLine("<table style=\"width:auto\">");
            WriteHTMLHeaderRow(sw);
            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            //            for (int i = 26; i < tbl.Length(); i++)
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (this.lenderLoanIDs.Contains(i))
                {
                    string rowID = "";
                    if ((i % 3) == 2) rowID = "ROW2";
                    this.WriteLoanStatusHTML(i, dtReport, sw, totals, rowID);
                }
            }
            // end of doc tags
            sw.WriteLine("<tr><td>TOTALS</td></tr>");
            sw.Write("<tr text-decoration:underline><td></td><td></td>");
            foreach (double d in totals)
                sw.Write("<td align=\"right\"><b><u>" +d.ToString("#,##0.00") +"</u></b></td>");
            sw.WriteLine("</tr></table></html></body>");
            sw.Close();
        }

        private void WriteLoanStatusHTML(int loanID, DateTime dtAsOf, System.IO.StreamWriter sw, List<double> totals, string rowIDName)
        {
            clsEntity titleHolder;
            clsEntity coBorrower;
            clsEntity titleCompany;
            int totalsIndex = 0;
            double value;

            clsLoan loan = new clsLoan(loanID).LoanAsOf(dtAsOf);
            titleHolder = new clsEntity(loan.TitleHolderID());
            titleCompany = new clsEntity(loan.TitleCompanyID());
            coBorrower = new clsEntity(loan.CoBorrowerID());
            clsLoan.State eStatus = loan.Status();

            sw.WriteLine("<tr ID=" + rowIDName + ">");

            sw.Write("<td align=\"left\">" + loan.Property().Address() + "</td>");
            if (eStatus.ToString().Length > 10)
                sw.Write("<td align=\"left\">" + eStatus.ToString().ToUpper().Substring(0,10) + "</td>");
            else
                sw.Write("<td align=\"left\">" + eStatus.ToString().ToUpper() + "</td>");

            if (eStatus == clsLoan.State.Cancelled)
            {
                for (int i = 0; i < 16; i++)
                {
                    sw.Write("<td></td>");
                    totalsIndex++;
                }
            }
            else
            {
                if (loan.Status() == clsLoan.State.Sold)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        sw.Write("<td></td>");
                        totalsIndex++;
                    }
                    if (loan.AccruedAdditionalInterest(dtAsOf) > 0D)
                    {
                        value = loan.AccruedAdditionalInterest(dtAsOf);
                        totals[totalsIndex] += value;
                        sw.Write("<td align=\"right\">" + value + "</td>");
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        sw.Write("<td></td>");
                        totalsIndex++;
                    }
                    value = loan.PrincipalPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.InterestPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.PointsPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.AdditionalInterestPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.Return(false);
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.IRR(false);
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                }
                else if (loan.Status() == clsLoan.State.PartiallySold)
                {
                    value = loan.Balance(dtAsOf);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.AccruedInterest(dtAsOf);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.ProjectedHardInterest();
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.AccruedAdditionalInterest(dtAsOf);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value + "</td>");
                    value = loan.Return(false);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    value = loan.IRR(false);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    value = loan.PrincipalPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.InterestPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.PointsPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.AdditionalInterestPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.Return(false);
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    value = loan.IRR(false);
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                }
                else
                {
                    value = loan.Balance(dtAsOf);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.AccruedInterest(dtAsOf);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.ProjectedHardInterest();
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.ProjectedAdditionalInterest(dtAsOf);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    value = loan.Return(false);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    value = loan.IRR(false);
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    sw.Write("<td align=\"right\">" + value.ToString("#0.00%") + "</td>");
                    for (int i = 0; i < 2; i++)
                    {
                        sw.Write("<td></td>");
                        totalsIndex++;
                    }
                    value = loan.PointsPaid(dtAsOf);
                    sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                    totals[totalsIndex] += value;
                    totalsIndex++;
                    for (int i = 0; i < 3; i++)
                    {
                        sw.Write("<td></td>");
                        totalsIndex++;
                    }
                }
                value = loan.RehabRemain(dtAsOf);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loan.RehabSpent(dtAsOf);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                sw.Write("<td align=\"right\">" + loan.SaleDate().ToShortDateString() + "</td>");
                sw.Write("<td align=\"right\">" + loan.OriginationDate().ToShortDateString() + "</td>");
                sw.Write("<td align=\"right\">" + (loan.SaleDate() - loan.OriginationDate()).TotalDays.ToString() + "</td>");
            }

            sw.Write("<td>" + titleHolder.Name() + "</td>");
            sw.Write("<td>" + coBorrower.Name() + "</td>");
            sw.Write("<td>" + titleCompany.Name() + "</td>");
            sw.Write("<td>" + loan.Rate().ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.PenaltyRate().ToString("#0.00%") + "</td>");
            sw.Write("<td>" + (loan.Points() * 0.01).ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.ProfitSplit().ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.OriginationDate().ToShortDateString() + "</td>");
            sw.Write("<td>" + loan.MaturityDate().ToShortDateString() + "</td>");
            sw.Write("<td>" + loan.GrossReturn(false).ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.GrossReturn(true).ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.Return(true).ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.IRR(true).ToString("#0.00%") + "</td>");
            sw.Write("<td>" + loan.FirstRehabEstimate().ToString("#,##0.00") + "</td>");
            sw.WriteLine();

            sw.WriteLine("</tr>");
            sw.Flush();
        }

        private void WriteHTMLHeaderRow(System.IO.StreamWriter sw)
        {//TODO Add points paid
            string title;
            sw.WriteLine("<tr id=HEADER>");
            title = "Property";
            sw.WriteLine("<th align=\"left\">" + title + "</th>");
            title = "State";
            sw.WriteLine("<th align=\"left\">" + title + "</th>");
            title = "Principal Balance";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Accrued Interest";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Proj Hard Int";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Proj Addl Int";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Proj Return";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Proj IRR";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Principal Paid";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Interest Paid";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Points Paid";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Addl Int Paid";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Act Return";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Act IRR";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Rehab Remain";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Rehab Spent";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Sale Date";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Purchase Date";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Days";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Borrower";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Co-Borrower";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Title (Acq)";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Rate";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Penalty";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Points";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Split";
            sw.WriteLine("<th>" + title + "</th>");
            sw.WriteLine("</tr>");
        }

        private void WriteLoanAuditHTML(int loanID, DateTime dtStart, DateTime dtEnd, System.IO.StreamWriter sw, List<double> totals, string rowIDName)
        {
            clsEntity titleHolder;
            clsEntity coBorrower;
            int totalsIndex = 0;
            double value;

            clsLoan loanStart = new clsLoan(loanID).LoanAsOf(dtStart);
            clsLoan loanEnd = new clsLoan(loanID).LoanAsOf(dtEnd);
            titleHolder = new clsEntity(loanEnd.TitleHolderID());
            coBorrower = new clsEntity(loanEnd.CoBorrowerID());

            if ((loanEnd.Status() != clsLoan.State.Cancelled) && (loanStart.Status() != clsLoan.State.Sold))
            {
                sw.WriteLine("<tr ID=" + rowIDName + ">");
                sw.Write("<td align=\"left\">" + loanEnd.Property().Address() + "</td>");
                // write values
                value = loanStart.Balance(dtStart);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loanEnd.Balance(dtEnd) - loanStart.Balance(dtStart) + loanEnd.PrincipalPaid(dtEnd);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loanEnd.PrincipalPaid(dtEnd);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loanEnd.Balance(dtEnd);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loanEnd.AccruedInterest(dtEnd);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loanEnd.InterestPaid(dtEnd);
                totals[totalsIndex] += value;
                totalsIndex++;
                sw.Write("<td align=\"right\">" + value.ToString("#,##0.00") + "</td>");
                value = loanEnd.Rate();
                sw.Write("<td align=\"right\">" + value.ToString("0.00%") + "</td>");
                sw.Write("<td>" + titleHolder.Name() + "</td>");
                sw.Write("<td>" + coBorrower.Name() + "</td>");
                sw.WriteLine();
                sw.WriteLine("</tr>");
                sw.Flush();
            }
        }

        private void WriteHTMLHeaderRowAudit(System.IO.StreamWriter sw)
        {
            string title;
            sw.WriteLine("<tr id=HEADER>");
            title = "Property";
            sw.WriteLine("<th align=\"left\">" + title + "</th>");
            title = "Starting Balance";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Additions";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Repayments";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Ending Balance";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Accrued Interest";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Interest Paid";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Interest Rate";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Mortgagor";
            sw.WriteLine("<th>" + title + "</th>");
            title = "Co-Borrower";
            sw.WriteLine("<th>" + title + "</th>");
            sw.WriteLine("</tr>");
        }

        #endregion

    }
}
