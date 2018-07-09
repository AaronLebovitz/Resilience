using System;
using AppKit;
using System.Collections.Generic;
using ResilienceClasses;
using Foundation;

namespace ManageNonLoanCashflows
{
    public class NonLoanCashflowDataSource:NSTableViewDataSource
    {

        public List<clsCashflow> Cashflows;

        public NonLoanCashflowDataSource()
        {
            this.Cashflows = new List<clsCashflow>();
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return Cashflows.Count;
        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            string text = string.Empty;
            int irow = (int)row;
            switch (tableColumn.Title)
            {
                case "ID":
                    text = this.Cashflows[irow].ID().ToString("00000");
                    break;

                case "PayDate":
                    text = this.Cashflows[irow].PayDate().ToString("MM/dd/yy");
                    break;

                case "RecDate":
                    text = this.Cashflows[irow].RecordDate().ToString("MM/dd/yy");
                    break;

                case "Amount":
                    text = this.Cashflows[irow].Amount().ToString("#,##0.00");
                    break;

                case "Type":
                    text = this.Cashflows[irow].TypeID().ToString();
                    break;

                case "Actual":
                    text = this.Cashflows[irow].Actual().ToString();
                    break;

                case "DelDate":
                    text = this.Cashflows[irow].DeleteDate().ToString("MM/dd/yy");
                    break;

                case "Comment":
                    text = this.Cashflows[irow].Comment();
                    break;

                default:
                    break;
            }

            return (NSString)text;
        }

    }
}
