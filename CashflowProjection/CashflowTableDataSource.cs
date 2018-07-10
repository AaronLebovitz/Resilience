using System;
using Foundation;
using AppKit;
using System.Collections.Generic;
using ResilienceClasses;

namespace CashflowProjection
{
    public class CashflowTableDataSource:NSTableViewDataSource
    {
        private List<clsCashflow> data;
        private List<double> balance;
        private double startingBalance;
        private List<string> addresses;

        public CashflowTableDataSource()
        {
            this.data = new List<clsCashflow>();
            this.balance = new List<double>();
            this.startingBalance = 0D;
            this.addresses = new List<string>();
            clsCSVTable loanTable = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < loanTable.Length(); i++)
                this.addresses.Add((new clsLoan(i)).Property().Address());
        }

        public double StartingBalance
        {
            get
            {
                return this.startingBalance;
            }

            set
            {
                double changeBalance = (value - this.startingBalance);
                for (int i = 0; i < this.balance.Count; i++)
                {
                    this.balance[i] += changeBalance;
                }
                this.startingBalance = value;
            }
        }

        public List<double> Balances 
        {
            get
            {
                return this.balance;
            }
        }

        public List<clsCashflow> Cashflows 
        { 
            get 
            {
                return this.data;
            }

            set
            {
                this.data = value;
                this.balance.Clear();
                double runningBalance = this.startingBalance;
                foreach (clsCashflow cf in this.data)
                {
                    runningBalance += cf.Amount();
                    this.balance.Add(runningBalance);
                }
            }
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return data.Count;

        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            string text = string.Empty;
            int irow = (int)row;
            switch (tableColumn.Title)
            {
                case "ID":
                    text = this.data[irow].ID().ToString("00000");
                    break;

                case "Date":
                    text = this.data[irow].PayDate().ToString("MM/dd/yy");
                    break;

                case "Property":
                    if ((this.data[irow]).LoanID() >= 0)
                        text = this.addresses[(this.data[irow]).LoanID()];
                    else
                        text = "Fund Ops";
                    break;

                case "Amount":
                    text = this.data[irow].Amount().ToString("#,##0.00");
                    break;

                case "Type":
                    text = this.data[irow].TypeID().ToString();
                    break;

                case "Actual":
                    text = this.data[irow].Actual().ToString();
                    break;

                case "Balance":
                    text = this.balance[irow].ToString("#,##0.00");
                    break;

                case "Notes":
                    if (this.data[irow].Comment() != null)
                        text = this.data[irow].Comment();
                    else
                        text = "";
                    break;

                default:
                    break;
            }

            return (NSString)text;
        }
    }
}
