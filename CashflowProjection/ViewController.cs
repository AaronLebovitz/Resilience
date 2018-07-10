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

        #endregion

        #region Private Methods

        private void RefreshTable()
        {
            // rebuild data source based on radio selection, date range
            double dStartingBalance = 0D;
            List<clsCashflow> includedCashflows = new List<clsCashflow>();
            foreach (clsCashflow cf in this.activeCashflows)
            {
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
            includedCashflows.Sort();
            this.dataSource.StartingBalance = dStartingBalance;
            this.dataSource.Cashflows = includedCashflows;

            // call refresh table
            this.CashflowTableView.ReloadData();
        }

        #endregion

    }
}
