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
        private Dictionary<string, int> addressToLoanID = clsLoan.LoanIDsByAddress();
        private DateTime startDate;
        private DateTime endDate;
        private bool showAll;
        private bool showScheduledOnly;
        private bool showExpensesOnly;
        private CashflowTableDataSource dataSource;
        private CashflowTableDataSourceDelegate dataSourceDelegate;
        private bool showFullDetail = true;
        private bool includeActual = true;
        private bool includeNonActual = true;
        private string addressFilter = "All";
        private clsCashflow.Type typeFilter = clsCashflow.Type.Unknown;
        private int lenderID = -1;
        private List<int> lenderIndexToID = new List<int>();
        private List<int> lenderLoanIDs = new List<int>();

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

            this.DateFilterDatePicker.DateValue = (NSDate)System.DateTime.Today;
            this.startDate = System.DateTime.Today.AddYears(-10);
            this.StartDatePicker.DateValue = (NSDate)this.startDate;
            this.endDate = System.DateTime.Today.AddYears(10);
            this.EndDatePicker.DateValue = (NSDate)this.endDate;

            this.ActualFilterComboxBox.RemoveAll();
            this.ActualFilterComboxBox.Add((NSString)"All");
            this.ActualFilterComboxBox.Add((NSString)"True");
            this.ActualFilterComboxBox.Add((NSString)"False");
            this.ActualFilterComboxBox.SelectItem(0);

            this.AddressFilterComboBox.RemoveAll();
            this.AddressFilterComboBox.Add((NSString)"All");
            List<string> addresses = clsProperty.AddressList();
            foreach (string s in addresses) this.AddressFilterComboBox.Add((NSString)s);
            this.AddressFilterComboBox.SelectItem(0);

            this.TypeFilterComboBox.RemoveAll();
            this.TypeFilterComboBox.Add((NSString)"All");
            foreach (clsCashflow.Type t in Enum.GetValues(typeof(clsCashflow.Type)))
                this.TypeFilterComboBox.Add((NSString)t.ToString());
            this.TypeFilterComboBox.Remove((NSString)"Unknown");
            this.TypeFilterComboBox.SelectItem(0);

            this.dataSource = new CashflowTableDataSource();
            this.dataSourceDelegate = new CashflowTableDataSourceDelegate(this.dataSource);
            this.CashflowTableView.DataSource = this.dataSource;
            this.CashflowTableView.Delegate = this.dataSourceDelegate;

            this.LenderPopUp.RemoveItem("Item 2");
            this.LenderPopUp.RemoveItem("Item 3");
            clsCSVTable tblLenders = new clsCSVTable(clsEntity.strEntityPath);
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tblLenders.Length(); i++)
            {
                if (tblLoans.Matches(clsLoan.LenderColumn, i.ToString()).Count > 0)
                {
                    this.LenderPopUp.AddItem(tblLenders.Value(i, clsEntity.NameColumn));
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

        partial void PastDueButtonPressed(NSButton sender)
        {
            List<clsCashflow> includedCashflows = new List<clsCashflow>();
            foreach (clsCashflow cf in this.activeCashflows)
            {
                if ((!cf.Actual()) && (cf.PayDate() <= System.DateTime.Today))
                    includedCashflows.Add(cf);
            }
            this.dataSource.Cashflows = includedCashflows;
            this.CashflowTableView.ReloadData();
        }

        partial void DateFilterPicked(NSDatePicker sender)
        {
            this.EndDatePicker.DateValue = this.DateFilterDatePicker.DateValue;
            this.endDate = (DateTime)this.EndDatePicker.DateValue;
            this.StartDatePicker.DateValue = this.DateFilterDatePicker.DateValue;
            this.startDate = (DateTime)this.StartDatePicker.DateValue;
            this.RefreshTable();
        }

        partial void ActualFilterSelected(NSComboBox sender)
        {
            if ((String)(NSString)ActualFilterComboxBox.SelectedValue == "All")
            {
                includeActual = true;
                includeNonActual = true;
            }
            else if ((String)(NSString)ActualFilterComboxBox.SelectedValue == "True")
            {
                includeActual = true;
                includeNonActual = false;
            }
            else
            {
                includeActual = false;
                includeNonActual = true;
            }
            this.RefreshTable();
        }

        partial void AddressFilterSelected(NSComboBox sender)
        {
            this.addressFilter = (string)(NSString)this.AddressFilterComboBox.SelectedValue;
            this.RefreshTable();
        }

        partial void TypeFilterSelected(NSComboBox sender)
        {
            this.typeFilter = (clsCashflow.Type)((int)this.TypeFilterComboBox.SelectedIndex);
            this.RefreshTable();
        }

        partial void ReloadButtonPushed(NSButton sender)
        {
            this.ReloadCashflows();
        }

        partial void LenderSelected(NSPopUpButton sender)
        {
            this.lenderLoanIDs.Clear();
            if (this.LenderPopUp.IndexOfSelectedItem > 0)
            {
                this.lenderID = this.lenderIndexToID[(int)this.LenderPopUp.IndexOfSelectedItem - 1];
                clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
                this.lenderLoanIDs = tblLoans.Matches(clsLoan.LenderColumn, this.lenderID.ToString());
            }
            else
            {
                this.lenderID = -1;
            }
            this.RefreshTable();
        }

        #endregion

        #region Private Methods

        private void ReloadCashflows()
        {
            clsCSVTable cfTable = new clsCSVTable(clsCashflow.strCashflowPath);
            this.activeCashflows.Clear();
            for (int i = 0; i < cfTable.Length(); i++)
            {
                if (DateTime.Parse(cfTable.Value(i, clsCashflow.DeleteDateColumn)) > System.DateTime.Today.AddYears(50))
                    this.activeCashflows.Add(new clsCashflow(i));
            }
            this.RefreshTable();
        }

        private void RefreshTable()
        {
            // rebuild data source based on radio selection, date range
            Dictionary<string, bool> rollUpDict = new Dictionary<string, bool>();
            string hashCashflow;
            double dStartingBalance = 0D;
            List<clsCashflow> includedCashflows = new List<clsCashflow>();
            List<clsCashflow> rollupCashflowsList = new List<clsCashflow>();
            Dictionary<string, clsCashflow> rollupCashflowsDict = new Dictionary<string, clsCashflow>();

            // compile included cashflows
            foreach (clsCashflow cf in this.activeCashflows)
            {
                // cashflow must be part of a loan that belongs to the selected Lender
                bool bInclude = this.lenderLoanIDs.Contains(cf.LoanID()) || (cf.LoanID() < 0);
                // cashflow must either (be actual if user wants actual) OR (not be actual if user wants non actual);
                bInclude = bInclude && ((((includeActual) && (cf.Actual())) || ((includeNonActual) && (!cf.Actual()))));
                // cashflow must belong to selected address OR no address must be selected ("All")
                if (bInclude)
                    bInclude = ((this.addressFilter == "All") || (this.addressToLoanID[this.addressFilter] == cf.LoanID()));
                // cashflow must be of type chosen, or no type must be chosen
                if (bInclude) bInclude = ((this.typeFilter == clsCashflow.Type.Unknown) || (this.typeFilter == cf.TypeID()));
                if (bInclude)
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
                    if ((cf.PayDate() >= this.startDate.Date) && (cf.PayDate() <= this.endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
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
