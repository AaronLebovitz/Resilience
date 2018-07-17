using System;
using ResilienceClasses;
using AppKit;
using Foundation;

namespace ConstructionHistory
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }
        // This is so I can use the same property chosen from before, in order to add a new cashflow for the chosen property
        int chosenID = 0;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
                propertyMenu.RemoveItem("Item 2");
                propertyMenu.RemoveItem("Item 3");
            System.Collections.Generic.List<string> addressList = new System.Collections.Generic.List<string>();

            addressList = clsProperty.AddressList();
            foreach (string s in addressList) propertyMenu.AddItem(s);

            // Do any additional setup after loading the view.

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

        partial void propertyMenuChosen(AppKit.NSPopUpButton sender)
        {
           
            loadRehabInfo();
        }

        partial void addNewConstruction(AppKit.NSButton sender)
        {
            clsLoan loan = new clsLoan(chosenID);
           
            DateTime payDate = (DateTime)(constructionDateNew.DateValue);
            DateTime recordDate = System.DateTime.Today;
            DateTime deleteDate = System.DateTime.MaxValue;
            int loanID = chosenID;
            double amount = constructionAmountNew.DoubleValue;
            if (amount > 0)
                amount = -amount;

            bool payed = false;
            if(payDate>System.DateTime.Today)
            {
                payed = false;
            }
            else
            {
                payed = true;
            }
           
            clsCashflow.Type typeID = new clsCashflow.Type();
            typeID = clsCashflow.Type.RehabDraw;

            clsCashflow newCashflow = new clsCashflow(payDate, recordDate, deleteDate, loanID, amount, payed, typeID);
            loan.Cashflows().Add(newCashflow);
            newCashflow.Save();
            loadRehabInfo();
//            if (loan.Save())
//                loadRehabInfo();
//            else
//                rehabDrawDisplayTrue.StringValue += "\nSave Failed, New Cashflow not Saved.";
                
        }
       
        partial void deleteButtonClicked(AppKit.NSButton sender)
        {
            int cfID = 0;
            cfID = Int32.Parse(cashflowID.StringValue);

            clsCashflow deletedCashflow = new clsCashflow(cfID);
            if (deletedCashflow.Actual() == true)
                rehabDrawDisplayFalse.StringValue += "Error: Could not delete cashflow, already payed.";
            else
                deletedCashflow.Delete(System.DateTime.Today);
            deletedCashflow.Save();
            loadRehabInfo();
        }

        partial void markTrueClicked(AppKit.NSButton sender)
        {
            int cfID = 0;
            cfID = Int32.Parse(cashflowID.StringValue);

            clsCashflow cf = new clsCashflow(cfID);
            cf.MarkActual(System.DateTime.Today);
            cf.Save();
            loadRehabInfo();
        }

        partial void dateChangeClicked(AppKit.NSButton sender)
        {
            clsLoan loan = new clsLoan(chosenID);
            int cfID = 0;
            cfID = Int32.Parse(cashflowID.StringValue);
            DateTime newDate = (DateTime)(dateChanger.DateValue);

            clsCashflow cf = new clsCashflow(cfID);
            cf.Delete(System.DateTime.Today);
            cf.Save();

            clsCashflow updatedCF = new clsCashflow(newDate, System.DateTime.Today, System.DateTime.MaxValue, cf.LoanID(), cf.Amount(), false, cf.TypeID());
            loan.AddCashflow(updatedCF);
            updatedCF.Save();

            loadRehabInfo();


        }

        private void loadRehabInfo()
        {
            chosenID = clsLoan.LoanID(propertyMenu.TitleOfSelectedItem);
            clsLoan l = new clsLoan(chosenID);
            rehabDrawDisplayTrue.StringValue = "Payed draws:" + "\n";
            rehabDrawDisplayFalse.StringValue = "Not payed draws:" + "\n";

            double trueTotal = 0;
            double falseTotal = 0;
            foreach (clsCashflow cashFlow in l.Cashflows())
            {
                if (cashFlow.TypeID() == clsCashflow.Type.RehabDraw && cashFlow.DeleteDate() > System.DateTime.Today)
                {

                    if (cashFlow.Actual() == true)
                    {
                        trueTotal += cashFlow.Amount();

                        rehabDrawDisplayTrue.StringValue += "Date: " + cashFlow.PayDate().ToString("MM/dd/yyyy hh:mm") + ", ";
                        rehabDrawDisplayTrue.StringValue += "Amount: " + "$" + (-cashFlow.Amount()).ToString("00,000.00") + ", ";
                        rehabDrawDisplayTrue.StringValue += "Payed: " + cashFlow.Actual() + "\n";
                    }
                    else
                    {
                        falseTotal += cashFlow.Amount();

                        rehabDrawDisplayFalse.StringValue += "Date: " + cashFlow.PayDate().ToString("MM/dd/yyyy hh:mm") + ", ";
                        rehabDrawDisplayFalse.StringValue += "Amount: " + "$" + (-cashFlow.Amount()).ToString("00,000.00") + ", ";
                        rehabDrawDisplayFalse.StringValue += "Payed: " + cashFlow.Actual() + ", ";
                        rehabDrawDisplayFalse.StringValue += "Cashflow ID: " + cashFlow.ID() + "\n";

                    }
                }
            }
            //After each cashflows are filtered, display the total
            trueTotal = -trueTotal;
            falseTotal = -falseTotal;

            rehabDrawDisplayTrue.StringValue += "Total amount: " + "$" + trueTotal.ToString("00,000.00");
            rehabDrawDisplayFalse.StringValue += "Total amount: " + "$" + falseTotal.ToString("00,000.00");
        }

   
    
    }
}
