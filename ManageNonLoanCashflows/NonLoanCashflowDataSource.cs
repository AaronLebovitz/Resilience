using System;
using AppKit;
using System.Collections.Generic;
using ResilienceClasses;
using Foundation;

namespace ManageNonLoanCashflows
{
    public class NonLoanCashflowDataSource : NSTableViewDataSource
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
                    if (this.Cashflows[irow].Comment() != null)
                        text = this.Cashflows[irow].Comment();
                    else
                        text = "";
                    break;

                default:
                    break;
            }

            return (NSString)text;
        }

        public void Sort(string key, bool ascending)
        {

            // Take action based on key
            switch (key)
            {
                case "ID":
                    if (ascending)
                    {
                        Cashflows.Sort((x, y) => x.ID().CompareTo(y.ID()));
                    }
                    else
                    {
                        Cashflows.Sort((x, y) => -1 * x.ID().CompareTo(y.ID()));
                    }
                    break;
                case "PayDate":
                    if (ascending)
                    {
                        Cashflows.Sort((x, y) => x.PayDate().CompareTo(y.PayDate()));
                    }
                    else
                    {
                        Cashflows.Sort((x, y) => -1 * x.PayDate().CompareTo(y.PayDate()));
                    }
                    break;
                case "Amount":
                    if (ascending)
                    {
                        Cashflows.Sort((x, y) => x.Amount().CompareTo(y.Amount()));
                    }
                    else
                    {
                        Cashflows.Sort((x, y) => -1 * x.Amount().CompareTo(y.Amount()));
                    }
                    break;
                case "Type":
                    if (ascending)
                    {
                        Cashflows.Sort((x, y) => x.TypeID().CompareTo(y.TypeID()));
                    }
                    else
                    {
                        Cashflows.Sort((x, y) => -1 * x.TypeID().CompareTo(y.TypeID()));
                    }
                    break;

                default:
                    break;
            }
        }

        public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors)
        {
            // Sort the data
            //if (oldDescriptors.Length > 0)
            //{
            //    // Update sort
            //    Sort(oldDescriptors[0].Key, oldDescriptors[0].Ascending);
            //}
            //else
            //{
                // Grab current descriptors and update sort
                NSSortDescriptor[] tbSort = tableView.SortDescriptors;
                Sort(tbSort[0].Key, tbSort[0].Ascending);
            //}

            // Refresh table
            tableView.ReloadData();
        }

    }
}
