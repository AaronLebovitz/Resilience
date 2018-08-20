using System;

using AppKit;
using Foundation;
using ResilienceClasses;
using System.Collections.Generic;

namespace ManageNonLoanCashflows
{
    public partial class ViewController : NSViewController
    {
        public clsCashflow.Type typeChosen;
        public NonLoanCashflowDataSource dataSource = new NonLoanCashflowDataSource();
        public bool showExpired = false;
        public bool showActualOnly = true;
        public bool validIDEntered = false;
        private List<int> entityIndexToID = new List<int>();
        private int entityID = -1;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            this.InitializeValues();
            this.TypePopUpButton.RemoveAllItems();
            List<string> typeList = new List<string>();
            foreach (clsCashflow.Type t in Enum.GetValues(typeof(clsCashflow.Type)))
            {
                if ((t == clsCashflow.Type.ManagementFee) ||
                    (t == clsCashflow.Type.AccountingFees) ||
                    (t == clsCashflow.Type.BankFees) ||
                    (t == clsCashflow.Type.CapitalCall) ||
                    (t == clsCashflow.Type.CatchUp) ||
                    (t == clsCashflow.Type.Distribution) ||
//                    (t == clsCashflow.Type.InterestAdditional) ||
                    (t == clsCashflow.Type.LegalFees) ||
                    (t == clsCashflow.Type.ManagementFee) ||
                    (t == clsCashflow.Type.Misc) ||
                    (t == clsCashflow.Type.PromoteFee))
                    typeList.Add(t.ToString());
            }
            typeList.Sort();
            foreach (string s in typeList) this.TypePopUpButton.AddItem(s);
            this.CashflowsTableView.Delegate = new NonLoanCashflowTableViewDelegate(this.dataSource);
            this.CashflowsTableView.TableColumns()[0].Title = "ID";
            this.CashflowsTableView.TableColumns()[0].Width = 60;
            this.CashflowsTableView.TableColumns()[1].Title = "PayDate";
            this.CashflowsTableView.TableColumns()[1].Width = 60;
            this.CashflowsTableView.TableColumns()[2].Title = "RecDate";
            this.CashflowsTableView.TableColumns()[2].Width = 60;
            this.CashflowsTableView.TableColumns()[3].Title = "Amount";
            this.CashflowsTableView.TableColumns()[3].Width = 80;
            this.CashflowsTableView.TableColumns()[4].Title = "Type";
            this.CashflowsTableView.TableColumns()[4].Width = 80;
            this.CashflowsTableView.TableColumns()[5].Title = "Actual";
            this.CashflowsTableView.TableColumns()[5].Width = 40;
            this.CashflowsTableView.TableColumns()[6].Title = "DelDate";
            this.CashflowsTableView.TableColumns()[6].Width = 60;
            this.CashflowsTableView.TableColumns()[7].Title = "Comment";
            this.CashflowsTableView.TableColumns()[7].Width = 200;
//            this.CashflowsTableView.RemoveColumn(this.CashflowsTableView.TableColumns()[0]);
            this.CashflowsTableView.DataSource = this.dataSource;

            clsCSVTable tblLenders = new clsCSVTable(clsEntity.strEntityPath);
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tblLenders.Length(); i++)
            {
                if (tblLoans.Matches(clsLoan.LenderColumn, i.ToString()).Count > 0)
                {
                    this.EntityPopUpButton.AddItem(tblLenders.Value(i, clsEntity.NameColumn));
                    this.entityIndexToID.Add(i);
                }
            }
        }

        partial void TypeChosen(AppKit.NSPopUpButton sender)
        {
            this.InitializeValues();
            this.RedrawTable();
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

        partial void ShowExpiredCheckBoxPressed(AppKit.NSButton sender)
        {
            this.showExpired = !this.showExpired;
            this.RedrawTable();
        }

        partial void AddButtonPressed(AppKit.NSButton sender)
        {
            DateTime dtPay = ((DateTime)this.DatePicker.DateValue).Date;
            DateTime dtRecord = ((DateTime)this.RecordDateOverridePicker.DateValue).Date;
            double dAmount = this.AmountTextField.DoubleValue;
            // VALIDATE ENTRIES
            if (this.typeChosen == clsCashflow.Type.Unknown)
            {
                this.SystemMessageTextField.StringValue = "INVALID CASHFLOW TYPE, UNABLE TO ADD";
            }
            else if (this.entityID < 0)
            {
                this.SystemMessageTextField.StringValue = "NO ENTITY SELECTED, UNABLE TO ADD";
            }
            else
            {
                // CREATE NEW CASHFLOW
                clsCashflow cashflow = new clsCashflow(dtPay, dtRecord, System.DateTime.MaxValue, -this.entityID, dAmount, 
                                                       false, this.typeChosen, this.CommentTextField.StringValue);
                if (dtPay <= System.DateTime.Today.Date) cashflow.MarkActual(System.DateTime.Today.Date);
                // SAVE TO TABLE
                cashflow.Save();
                this.CashflowIDTextField.IntValue = cashflow.ID();
                // UPDATE COMMENT BOX
                this.SystemMessageTextField.StringValue = "CASHFLOW SAVED.";
                if ((cashflow.TypeID() == clsCashflow.Type.InterestAdditional) && (cashflow.Amount() < 0D))
                    this.SystemMessageTextField.StringValue += "\nCheck Amount.  Additional Interest is usually >= 0.";
                else if ((cashflow.TypeID() == clsCashflow.Type.CapitalCall) && (cashflow.Amount() < 0D))
                    this.SystemMessageTextField.StringValue += "\nCheck Amount.  Capital Calls are usually >= 0.";
                else if ((cashflow.TypeID() == clsCashflow.Type.CatchUp) && (cashflow.Amount() < 0D))
                    this.SystemMessageTextField.StringValue += "\nCheck Amount.  Catchup Payments are usually >= 0.";
                else if (cashflow.Amount() > 0)
                    this.SystemMessageTextField.StringValue += "\nCheck Amount.  Expenses are normally <0.";
            }
            RedrawTable();
        }

        partial void CashflowIDEntered(AppKit.NSTextField sender)
        {
            clsCashflow cf = new clsCashflow(this.CashflowIDTextField.IntValue);
            this.DatePicker.DateValue = (NSDate)cf.PayDate().ToUniversalTime();
            this.RecordDateOverridePicker.DateValue = (NSDate)cf.RecordDate().ToUniversalTime();
            this.AmountTextField.DoubleValue = cf.Amount();

            this.TypePopUpButton.SelectItem(cf.TypeID().ToString());
            if (this.TypePopUpButton.TitleOfSelectedItem == null)
            {
                this.typeChosen = clsCashflow.Type.Unknown;
                this.SystemMessageTextField.StringValue = "INVALID CASHFLOW TYPE (" + cf.TypeID().ToString() + ")";
                this.validIDEntered = false;
            }
            else
            {
                this.validIDEntered = true;
                if (cf.Comment() != null)
                    this.CommentTextField.StringValue = cf.Comment();
                else
                    this.CommentTextField.StringValue = "";
                this.RedrawTable();
            }
            this.ActualTextField.StringValue = "Actual : " + cf.Actual().ToString();
            this.RedrawTable();
        }

        partial void ExpireButtonPressed(AppKit.NSButton sender)
        {
            if (this.validIDEntered)
            {
                clsCashflow cashflow = new clsCashflow(this.CashflowIDTextField.IntValue);
                if (cashflow.Delete(System.DateTime.Now))
                {
                    this.SystemMessageTextField.StringValue = "CASHFLOW EXPIRED";
                    cashflow.Save();
                }
                else
                {
                    this.SystemMessageTextField.StringValue = "CASHFLOW IS ALREADY ACTUAL, EXPIRE FAILED";
                }
            }
            RedrawTable();
        }

        partial void RecordDateOverriden(AppKit.NSDatePicker sender)
        {
            
        }

        partial void ActualCashflowPressed(NSButton sender)
        {
            if (this.validIDEntered)
            {
                clsCashflow cashflow = new clsCashflow(this.CashflowIDTextField.IntValue);
                if (cashflow.Comment() != null)
                    this.CommentTextField.StringValue = cashflow.Comment();
                else
                    this.CommentTextField.StringValue = "";
                if (cashflow.Actual())
                {
                    this.SystemMessageTextField.StringValue = "CASHFLOW IS ALREADY ACTUAL";
                }
                else if (cashflow.MarkActual(System.DateTime.Today))
                {
                    this.SystemMessageTextField.StringValue = "CASHFLOW MADE ACTUAL";
                    this.ActualTextField.StringValue = "Actual : " + true.ToString();
                    cashflow.Save();
                }
                else
                {
                    this.SystemMessageTextField.StringValue = "CASHFLOW EXPIRED, CAN'T MAKE ACTUAL";
                }
            }
            RedrawTable();
        }

        partial void EntityChosen(NSPopUpButton sender)
        {
            this.entityID = this.entityIndexToID[(int)this.EntityPopUpButton.IndexOfSelectedItem - 1];
            RedrawTable();
        }

        private void RedrawTable()
        {
            clsCSVTable tbl = new clsCSVTable(clsCashflow.strCashflowPath);
            string selected = this.TypePopUpButton.TitleOfSelectedItem;
            this.typeChosen = clsCashflow.Type.Unknown;
            foreach (clsCashflow.Type t in Enum.GetValues(typeof(clsCashflow.Type)))
            {
                if (t.ToString() == selected)
                    this.typeChosen = t;
            }
            List<int> IDs = tbl.Matches(clsCashflow.TransactionTypeColumn, ((int)this.typeChosen).ToString());
            this.dataSource.Cashflows.Clear();
            foreach (int id in IDs)
            {
                clsCashflow newCf = new clsCashflow(id);
                if (((this.showExpired) || (newCf.DeleteDate() > System.DateTime.Today.AddYears(50))) && (newCf.LoanID() == -this.entityID))
                    this.dataSource.Cashflows.Add(newCf);
            }
            this.CashflowsTableView.ReloadData();
        }

        private void InitializeValues()
        {
            this.DatePicker.DateValue = (NSDate)System.DateTime.Today;
            this.RecordDateOverridePicker.DateValue = (NSDate)System.DateTime.Today;
            this.AmountTextField.StringValue = "Amount";
            this.CashflowIDTextField.StringValue = "Cashflow ID";
            this.CommentTextField.StringValue = "Comment";
            this.ActualTextField.StringValue = "";
            this.SystemMessageTextField.StringValue = "";
            this.validIDEntered = false;
        }

    }
}
