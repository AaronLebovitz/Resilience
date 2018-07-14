using System;
using ResilienceClasses;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace CashflowProjection
{
    public partial class ViewController : NSViewController
    {

        #region Private Fields

        private List<clsCashflow> activeCashflows = new List<clsCashflow>();
        private DateTime startDate;
        private DateTime endDate;
        private bool showAll;
        private bool showScheduledOnly;
        private bool showExpensesOnly;
        private CashflowTableDataSource dataSource;
        private CashflowTableDataSourceDelegate dataSourceDelegate;
        private bool showFullDetail = true;

        #endregion


        #region Public Methods

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            this.ScenarioButton.State = NSCellStateValue.On;
            this.OutflowsOnlyButton.State = NSCellStateValue.Off;
            this.ScheduledOnlyButton.State = NSCellStateValue.Off;
            this.showAll = true;
            this.showScheduledOnly = false;
            this.showExpensesOnly = false;

            clsCSVTable cfTable = new clsCSVTable(clsCashflow.strCashflowPath);
            for (int i = 0; i < cfTable.Length(); i++)
            {
                if (DateTime.Parse(cfTable.Value(i, clsCashflow.DeleteDateColumn)) > System.DateTime.Today.AddYears(50))
                    this.activeCashflows.Add(new clsCashflow(i));
            }

            this.startDate = System.DateTime.Today.AddYears(-10);
            this.StartDatePicker.DateValue = (NSDate)this.startDate;
            this.endDate = System.DateTime.Today.AddYears(10);
            this.EndDatePicker.DateValue = (NSDate)this.endDate;
            this.dataSource = new CashflowTableDataSource();
            this.dataSourceDelegate = new CashflowTableDataSourceDelegate(this.dataSource);
            this.CashflowTableView.DataSource = this.dataSource;
            this.CashflowTableView.Delegate = this.dataSourceDelegate;
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

        #endregion

        #region Dates Picked

        partial void StartDatePicked(AppKit.NSDatePicker sender)
        {
            this.startDate = (DateTime)this.StartDatePicker.DateValue;
            this.RefreshTable();
        }

        partial void EndDatePicked(AppKit.NSDatePicker sender)
        {
            this.endDate = (DateTime)this.EndDatePicker.DateValue;
            this.RefreshTable();
        }

        #endregion

        #region Button Presses

        partial void OutflowsOnlyPressed(AppKit.NSButton sender)
        {
            this.showExpensesOnly = true;
            this.showScheduledOnly = false;
            this.showAll = false;
            this.ScenarioButton.State = NSCellStateValue.Off;
            this.ScheduledOnlyButton.State = NSCellStateValue.Off;
            this.RefreshTable();
        }

        partial void ScenarioButtonPressed(AppKit.NSButton sender)
        {
            this.showExpensesOnly = false;
            this.showScheduledOnly = false;
            this.showAll = true;
            this.OutflowsOnlyButton.State = NSCellStateValue.Off;
            this.ScheduledOnlyButton.State = NSCellStateValue.Off;
            this.RefreshTable();
        }

        partial void ScheduledOnlyPressed(AppKit.NSButton sender)
        {
            this.showExpensesOnly = false;
            this.showScheduledOnly = true;
            this.showAll = false;
            this.ScenarioButton.State = NSCellStateValue.Off;
            this.OutflowsOnlyButton.State = NSCellStateValue.Off;
            this.RefreshTable();
        }

        partial void SaveCSVPressed(NSButton sender)
        {
            string filePath = "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/CashflowProjection";
            filePath += "_" + this.startDate.ToString("yyyyMMdd");
            filePath += "_" + this.endDate.ToString("yyyyMMdd");

            if (this.showAll) filePath += "_ALL";
            else if (this.showExpensesOnly) filePath += "_ExpensesOnly";
            else if (this.showScheduledOnly) filePath += "_ScheduledOnly";

            if (this.showFullDetail) filePath += "_full";
            else filePath += "_rollup";

            filePath += ".csv";

            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(filePath);
            for (int i = 0; i < this.dataSource.Cashflows.Count; i++)
            {
                streamWriter.Write(this.dataSource.Cashflows[i].ID());
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].LoanID());
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Address(this.dataSource.Cashflows[i].LoanID()));
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].PayDate().ToString("MM/dd/yyyy"));
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].Amount().ToString("#0.00"));
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].TypeID().ToString());
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].Actual().ToString());
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].Comment());
                streamWriter.Write(",");
                streamWriter.WriteLine(this.dataSource.Balances[i].ToString("#0.00"));
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        partial void NAVExportPressed(NSButton sender)
        {
            string filePath = "/Users/" + Environment.UserName + "/Documents/Professional/Resilience/CashflowReportNAV";
            filePath += "_" + this.startDate.ToString("yyyyMMdd");
            filePath += "_" + this.endDate.ToString("yyyyMMdd");
            filePath += ".csv";

            this.showAll = true;
            this.ScenarioButton.State = NSCellStateValue.On;
            this.OutflowsOnlyButton.State = NSCellStateValue.Off;
            this.ScheduledOnlyButton.State = NSCellStateValue.Off;
            this.showFullDetail = false;
            this.FullDetailCheckBox.State = NSCellStateValue.Off;
            this.RefreshTable();

            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(filePath);

            // titles
            streamWriter.WriteLine("Date,Amount,Type,Description");

            // data
            for (int i = 0; i < this.dataSource.Cashflows.Count; i++)
            {
                int loanID = this.dataSource.Cashflows[i].LoanID();
                streamWriter.Write(this.dataSource.Cashflows[i].PayDate().ToString("MM/dd/yyyy"));
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].Amount().ToString("#0.00"));
                streamWriter.Write(",");
                streamWriter.Write(this.dataSource.Cashflows[i].TypeID().ToString());
                streamWriter.Write(",");
                if (loanID >= 0)
                    streamWriter.Write(new clsLoan(loanID).Property().Address());
                else 
                {
                    switch (this.dataSource.Cashflows[i].TypeID())
                    {
                        case clsCashflow.Type.AccountingFees:
                            streamWriter.Write("RSM");
                            break;
                        case clsCashflow.Type.BankFees:
                            streamWriter.Write("Huntington");
                            break;
                        case clsCashflow.Type.LegalFees:
                            streamWriter.Write("Dykema");
                            break;
                        case clsCashflow.Type.ManagementFee:
                        case clsCashflow.Type.PromoteFee:
                            streamWriter.Write("Adaptation");
                            break;
                        default:
                            streamWriter.Write("N/A");
                            break;
                    }
                }
                streamWriter.WriteLine();
                streamWriter.Flush();
            }
            streamWriter.Close();
        }

        partial void FullDetailCheckBoxPressed(NSButton sender)
        {
            this.showFullDetail = !this.showFullDetail;
            this.RefreshTable();
        }

        #endregion

        #region Private Methods

        private void RefreshTable()
        {
            // rebuild data source based on radio selection, date range
            Dictionary<string, bool> rollUpDict = new Dictionary<string, bool>();
            string hashCashflow;
            double dStartingBalance = 0D;
            List<clsCashflow> includedCashflows = new List<clsCashflow>();
            List<clsCashflow> rollupCashflowsList = new List<clsCashflow>();
            Dictionary<string, clsCashflow> rollupCashflowsDict = new Dictionary<string, clsCashflow>();
            foreach (clsCashflow cf in this.activeCashflows)
            {
                // Rollup Tracking
                if (cf.AcquisitionType)
                {
                    hashCashflow = cf.PayDate().ToString("yyyyMMdd") + cf.LoanID().ToString("0000") + ((int)clsCashflow.Type.Acquisition).ToString("00");
                }
                else if (cf.RepaymentType)
                {
                    hashCashflow = cf.PayDate().ToString("yyyyMMdd") + cf.LoanID().ToString("0000") + ((int)clsCashflow.Type.Repayment).ToString("00");
                                    }
                else
                    hashCashflow = cf.PayDate().ToString("yyyyMMdd") + cf.LoanID().ToString("0000") + ((int)cf.TypeID()).ToString("00");
                if (!this.showFullDetail) // roll up
                {
                    if (rollUpDict.ContainsKey(hashCashflow))
                        rollUpDict[hashCashflow] = true;
                    else
                        rollUpDict.Add(hashCashflow, false);
                }
                // Create shortened Cashflow List
                if ((cf.PayDate() >= this.startDate) && (cf.PayDate() <= this.endDate))
                {
                    if (this.showAll)
                        includedCashflows.Add(cf);
                    else if ((this.showScheduledOnly) && (cf.TypeID() != clsCashflow.Type.NetDispositionProj))
                        includedCashflows.Add(cf);
                    else if ((cf.Amount() < 0D) || (cf.Actual()) && (this.showExpensesOnly))
                        includedCashflows.Add(cf);
                }
                else if (cf.PayDate() < this.startDate)
                    dStartingBalance += cf.Amount();
            }

            // Roll up cashflows if necessary
            if (!this.showFullDetail)
            {
                foreach (clsCashflow cf in includedCashflows)
                {
                    clsCashflow.Type rollupType;
                    if (cf.AcquisitionType)
                    {
                        rollupType = clsCashflow.Type.Acquisition;
                        hashCashflow = cf.PayDate().ToString("yyyyMMdd") + cf.LoanID().ToString("0000") + ((int)clsCashflow.Type.Acquisition).ToString("00");
                    }
                    else if (cf.RepaymentType)
                    {
                        rollupType = clsCashflow.Type.Repayment;
                        hashCashflow = cf.PayDate().ToString("yyyyMMdd") + cf.LoanID().ToString("0000") + ((int)clsCashflow.Type.Repayment).ToString("00");
                    }
                    else
                    {
                        rollupType = cf.TypeID();
                        hashCashflow = cf.PayDate().ToString("yyyyMMdd") + cf.LoanID().ToString("0000") + ((int)cf.TypeID()).ToString("00");
                    }
                    if (rollUpDict[hashCashflow])
                    {
                        if (rollupCashflowsDict.ContainsKey(hashCashflow))
                        {
                            rollupCashflowsDict[hashCashflow].AddAmount(cf.Amount());
                        }
                        else
                        {
                            rollupCashflowsDict.Add(hashCashflow, new clsCashflow(cf.PayDate(), cf.RecordDate(), cf.DeleteDate(), cf.LoanID(),
                                                                              cf.Amount(), cf.Actual(), rollupType));
                        }
                    }
                    else
                        rollupCashflowsList.Add(cf);
                }
                foreach (clsCashflow cf in rollupCashflowsDict.Values) rollupCashflowsList.Add(cf);
                includedCashflows = rollupCashflowsList;
            }

            includedCashflows.Sort();
            this.dataSource.StartingBalance = dStartingBalance;
            this.dataSource.Cashflows = includedCashflows;

            // call refresh table
            this.CashflowTableView.ReloadData();
        }

        #endregion

    }
}
