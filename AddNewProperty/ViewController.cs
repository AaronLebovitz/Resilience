using System;

using AppKit;
using Foundation;
using ResilienceClasses;

namespace AddNewProperty
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            clsCSVTable EntityTable = new clsCSVTable(clsEntity.strEntityPath);
            for (int i = 0; i < EntityTable.Length(); i++)
            {
                this.TitleHolderPopUp.AddItem(EntityTable.Value(i, clsEntity.NameColumn));
                this.CoBorrowerPopUp.AddItem(EntityTable.Value(i, clsEntity.NameColumn));
                this.TitlePopUp.AddItem(EntityTable.Value(i, clsEntity.NameColumn));
                this.LenderPopUp.AddItem(EntityTable.Value(i, clsEntity.NameColumn));
            }
            this.PurchaseDatePicker.DateValue = (NSDate)System.DateTime.Today.AddMonths(1);
            this.TitleHolderPopUp.SelectItem(6);
            this.CoBorrowerPopUp.SelectItem(5);
            this.LenderPopUp.SelectItem(4);
            this.PointsBox.DoubleValue = 0D;
            this.DefaultRateBox.DoubleValue = 0.05;
            this.LoanRateBox.DoubleValue = 0.09;
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

        partial void AddButtonPushed(AppKit.NSButton sender)
        {
            // store values
            string address = this.AddressBox.StringValue;
            string town = this.TownBox.StringValue;
            string county = this.CountyBox.StringValue;
            string state = this.StateBox.StringValue;
            int titleHolderID = (int)this.TitleHolderPopUp.IndexOfSelectedItem-3;
            int coID = (int)this.CoBorrowerPopUp.IndexOfSelectedItem-3;
            int lenderID = (int)this.LenderPopUp.IndexOfSelectedItem-3;
            int titleID = (int)this.TitlePopUp.IndexOfSelectedItem-3;
            DateTime acquisitionDate = (DateTime)this.PurchaseDatePicker.DateValue;
            double price = this.PurchasePriceBox.DoubleValue;
            double bpo = this.BPOBox.DoubleValue;
            double rehabCost = this.RehabCostBox.DoubleValue;
            double pnl = this.PnLBox.DoubleValue;
            double months = this.MonthsToCompletionBox.DoubleValue;
            double loanRate = this.LoanRateBox.DoubleValue;
            double penaltyRate = this.DefaultRateBox.DoubleValue;
            double points = this.PointsBox.DoubleValue;

            // acquisition cost estimates
            double processingCost;
            double recordingCost;
            double acquisitionTaxes;
            double titlePolicyCost;
            double HOICost;
            double propertyTaxes;
            double initialDraw = -4500D;

            double programFee = -2000D;
            double commissions;
            double transferTax;
            double proRatedPropertyTax;
            double otherBudgetedCosts = initialDraw - programFee;  // accounting, travel, utilities, inspections, etc.

            double concessionPercentage = 0.02;
            double commissionPercentage = 0.06;
            double totalBackEndCosts;

            // other stuff
            double dispostion;
            double totalCommitment;
            double hardInterestGuess;
            double pnlGuess;

            int streetNumber;
            string streetName;

            this.UpdateMessage.StringValue = "";

            // check State.length == 2
            if (state.Length != 2) 
            {
                this.UpdateMessage.StringValue = "Invalid State ID (Length must be 2)";
            }
            // check date > today
            else if (acquisitionDate < System.DateTime.Today) 
            {
                this.UpdateMessage.StringValue = "Acquisition Date can not be in the past.";
            }
            // check no duplicates in entities
            else if ((titleHolderID == coID) || (titleHolderID == lenderID) || (titleHolderID == titleID) ||
                     (coID == lenderID) || (coID == titleID) || (lenderID == titleID))
            {
                this.UpdateMessage.StringValue = "Duplicate Entities - please check TitleHolder, CoBorrower, Lender and Title Company";
            }
            // check address has numbers and letters
            else if ((!Int32.TryParse(System.Text.RegularExpressions.Regex.Match(address, @"\d+").Value, out streetNumber)) ||
                     (streetName = System.Text.RegularExpressions.Regex.Replace(address, streetNumber.ToString(), "").Trim()) == "")
            {
                this.UpdateMessage.StringValue = "Invalid Street Address, must be {streetnumber} {streetname}";
            }
            // check all values are positive
            else if (price * rehabCost * months * pnl * bpo < 0.001)
            {
                this.UpdateMessage.StringValue = "Missing Values in price, rehab, PnL, BPO, or Months";
            }
            // check things like lender elgible, borrower eligible, title company state eligible
            // Check valid address??
            // Check county??
            else
            {
                // calculate estimated acquisition costs
                switch (state)
                {

                    case "NJ":

                        processingCost = -1150D;
                        recordingCost = -415D;
                        acquisitionTaxes = 0D;
                        titlePolicyCost = -620 - 0.0044 * price;
                        HOICost = -450 - 0.0054 * price;
                        propertyTaxes = -0.015 * price;
                        proRatedPropertyTax = bpo * 0.002 * months;  // roughly 2.4% per year in NJ
                        transferTax = bpo * 0.01;
                        break;

                    case "MD":

                        processingCost = -875D;
                        recordingCost = -1090D - 0.0025 * price;
                        acquisitionTaxes = -190D - 0.0066 * price;
                        titlePolicyCost = -463D - 0.0042 * price;
                        HOICost = -1017D - 0.0025 * price;
                        propertyTaxes = -0.009 * price;
                        proRatedPropertyTax = bpo * 0.0009 * months;  // roughly 1.1% per year in MD
                        transferTax = bpo * 0.01;
                        break;

                    case "PA":

                        processingCost = -250D;
                        recordingCost = -425D - 0.0025 * price;
                        acquisitionTaxes = -450D - 0.02 * price;
                        titlePolicyCost = -1330D - 0.005 * price;
                        HOICost = -417D - 0.0144 * price;
                        propertyTaxes = -0.017 * price;
                        proRatedPropertyTax = bpo * 0.00125 * months;  // roughly 1.5% per year in PA
                        transferTax = bpo * 0.01;
                        break;

                    case "GA":
                    default:

                        processingCost = -250D;
                        recordingCost = -425D - 0.0025 * price;
                        acquisitionTaxes = -450D - 0.02 * price;
                        titlePolicyCost = -1330D - 0.005 * price;
                        HOICost = -417D - 0.0144 * price;
                        propertyTaxes = -0.017 * price;
                        proRatedPropertyTax = bpo * 0.00125 * months;  // roughly 1.5% per year in PA
                        transferTax = bpo * 0.01;
                        break;
                }

                // calculate rough estimated other costs

                totalCommitment = price + rehabCost - 
                    (initialDraw + processingCost + recordingCost + acquisitionTaxes + titlePolicyCost + HOICost + propertyTaxes);
                commissions = bpo * commissionPercentage;
                // total back end costs are Accrued Property Tax, Sale Transfer Tax (typically split with buyer), Sale Commissons,
                //   seller concession / assistance, and Hard Interest
                hardInterestGuess = (totalCommitment - 0.5 * rehabCost) * (months / 12D) * loanRate;
                totalBackEndCosts = proRatedPropertyTax + transferTax + commissions + hardInterestGuess + bpo * concessionPercentage;
                pnlGuess = bpo - (totalCommitment + totalBackEndCosts);

                // Check estimated PnL based on State, Purchase, Rehab - Message Box variance, reject if outside $X
                UpdateMessage.StringValue += "\n Estimated vs Quoted PnL: \t";
                UpdateMessage.StringValue += pnlGuess.ToString("000,000.00") + " vs " + pnl.ToString("000,000.00");

                dispostion = totalCommitment + hardInterestGuess + 0.5 * pnlGuess;

                // if valid, then create new property, new loan, new cashflows, new documents
                clsProperty newProperty = new clsProperty(address, town, county, state, bpo, streetName);
                newProperty.Save();

                clsLoan newLoan = new clsLoan(newProperty.ID(), titleHolderID, coID, titleID, lenderID,
                                              acquisitionDate, acquisitionDate.AddMonths(9), loanRate, penaltyRate, points);
                int newLoanID = newLoan.ID();
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID, 
                                                    -price, false, clsCashflow.Type.AcquisitionPrice));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    0D, false, clsCashflow.Type.AcquisitionConcession));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    processingCost, false, clsCashflow.Type.AcquisitionProcessing));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    recordingCost, false, clsCashflow.Type.AcquisitionRecording));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    acquisitionTaxes, false, clsCashflow.Type.AcquisitionTaxes));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    HOICost, false, clsCashflow.Type.HomeownersInsurance));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    initialDraw, false, clsCashflow.Type.InitialExpenseDraw));
                newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                    titlePolicyCost, false, clsCashflow.Type.TitlePolicy));
                // assume entire rehab draw comes halfway through the rehab process, and that there is a two month lag from rehab completion to sale closing
                newLoan.AddCashflow(new clsCashflow(acquisitionDate.AddMonths((int)(0.5 * (months - 2D))), System.DateTime.Today,
                                                    System.DateTime.MaxValue, newLoanID, -rehabCost, false, clsCashflow.Type.RehabDraw));
                // disposition cashflow
                newLoan.AddCashflow(new clsCashflow(acquisitionDate.AddMonths((int)months), System.DateTime.Today, System.DateTime.MaxValue,
                                                    newLoanID, dispostion, false, clsCashflow.Type.NetDispositionProj));
                newLoan.SetNewOriginationDate(acquisitionDate);
                newLoan.Save();

                // Create Documents
                for (int i = 0; i < Enum.GetValues(typeof(clsDocument.Type)).Length; i++)
                {
                    clsDocument newDoc = new clsDocument(((clsDocument.Type)i).ToString(),newProperty.ID(),(clsDocument.Type)i) ;
                    newDoc.Save();
                }

                // write summary to text label (this.UpdateMessage.StringValue)
                this.UpdateMessage.StringValue += String.Format("\n\nCompleted adding new loan ({0}), new cashflows, new property ({1}), and new docs ",
                                                             newLoan.ID().ToString(), 
                                                             newProperty.ID().ToString());
            }
        }
    }
}
