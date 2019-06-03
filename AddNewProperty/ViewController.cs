using System;

using AppKit;
using Foundation;
using ResilienceClasses;

namespace AddNewProperty
{
    public class CostEstimate
    {
        #region properties
        private double processingCost;
        private double recordingCost;
        private double acquisitionTaxes;
        private double titlePolicyCost;
        private double hOICost;
        private double propertyTaxes;
        private double transferTax;
        private double proRatedPropertyTax;
        #endregion

        public CostEstimate(string state, double price, double bpo, double months, bool sellerExemption = false)
        {
            switch (state)
            {

                case "NJ":

                    this.processingCost = -1150D;
                    this.recordingCost = -415D;
                    this.acquisitionTaxes = 0D;
                    this.titlePolicyCost = Math.Round(-620 - 0.0044 * price,2);
                    this.hOICost = Math.Round(-450 - 0.0054 * price,2);
                    this.propertyTaxes = Math.Round(-0.015 * price,2);
                    this.proRatedPropertyTax = Math.Round(bpo * 0.002 * months,2);  // roughly 2.4% per year in NJ
                    this.transferTax = Math.Round(bpo * 0.01,2);
                    break;

                case "MD":

                    this.processingCost = -875D;
                    this.recordingCost = Math.Round(-1090D - 0.0025 * price,2);
                    this.acquisitionTaxes = Math.Round(-190D - 0.0066 * price, 2);
                    this.titlePolicyCost = Math.Round(-463D - 0.0042 * price, 2);
                    this.hOICost = Math.Round(-1017D - 0.0025 * price, 2);
                    this.propertyTaxes = Math.Round(-0.009 * price, 2);
                    this.proRatedPropertyTax = Math.Round(bpo * 0.0009 * months, 2);  // roughly 1.1% per year in MD
                    this.transferTax = Math.Round(bpo * 0.01, 2);
                    break;

                case "PA":

                    this.processingCost = -250D;
                    this.recordingCost = Math.Round(-425D - 0.0025 * price, 2);
                    this.acquisitionTaxes = Math.Round(-450D - 0.02 * price, 2);
                    this.titlePolicyCost = Math.Round(-1330D - 0.005 * price, 2);
                    this.hOICost = Math.Round(-417D - 0.0144 * price, 2);
                    this.propertyTaxes = Math.Round(-0.017 * price, 2);
                    this.proRatedPropertyTax = Math.Round(bpo * 0.00125 * months, 2);  // roughly 1.5% per year in PA
                    this.transferTax = Math.Round(bpo * 0.01, 2);
                    break;

                case "GA":
                default:

                    this.processingCost = -250D;
                    this.recordingCost = Math.Round(-425D - 0.0025 * price, 2);
                    this.acquisitionTaxes = Math.Round(-450D - 0.02 * price, 2);
                    this.titlePolicyCost = Math.Round(-1330D - 0.005 * price, 2);
                    this.hOICost = Math.Round(-417D - 0.0144 * price, 2);
                    this.propertyTaxes = Math.Round(-0.017 * price, 2);
                    this.proRatedPropertyTax = Math.Round(bpo * 0.00125 * months, 2);  // roughly 1.5% per year in PA
                    this.transferTax = Math.Round(bpo * 0.01, 2);
                    break;
            }
        }

        #region Accessors
        public Double ProcessingCost { get { return this.processingCost; } }
        public Double RecordingCost { get { return this.recordingCost; } }
        public Double AcquisitionTaxes { get { return this.acquisitionTaxes; } }
        public Double TitlePolicyCost { get { return this.titlePolicyCost; } }
        public Double HOICost { get { return this.hOICost; } }
        public Double PropertyTaxes { get { return this.propertyTaxes; } }
        public Double TransferTax {  get { return this.transferTax; } }
        public Double ProratedPropertyTax { get { return this.proRatedPropertyTax; } }
        public Double AcquisitionClosingCosts {
            get {
                return this.processingCost
                      + this.recordingCost
                      + this.acquisitionTaxes
                      + this.titlePolicyCost
                      + this.hOICost
                      + this.propertyTaxes;
            }
        }
        public Double SaleTaxes
        {
            get { return this.transferTax + this.ProratedPropertyTax; }
        }
        #endregion
    }

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
            this.TitleHolderPopUp.SelectItem(4);
            this.CoBorrowerPopUp.SelectItem(0);
            this.LenderPopUp.SelectItem(17);
            this.PointsBox.DoubleValue = 0D;
            this.DefaultRateBox.DoubleValue = 0.05;
            this.LoanRateBox.DoubleValue = 0.09;
            this.ProfitSplitBox.DoubleValue = 0.50;
            this.AcquisitionOnlyCheckBox.State = NSCellStateValue.Off;
            this.InitialLoanPercentBox.DoubleValue = 1D;
            this.InitialLoanPercentBox.Editable = false;
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

        partial void AcquisitionOnlyToggled(NSButton sender)
        {
            if (this.AcquisitionOnlyCheckBox.State == NSCellStateValue.On)
            {
                // handle turn on by setting rehab value to 0D and disabling it for editing
                this.RehabCostBox.DoubleValue = 0D;
                this.RehabCostBox.Editable = false;
            }
            else 
            {
                // handle turn off by enabling RehabCostBox for editing, and setting FullAcquisitionCost toggle to ON (true)
                this.RehabCostBox.Editable = true;
                this.FullAcquisitionCostCheckBox.State = NSCellStateValue.On;
                this.InitialLoanPercentBox.DoubleValue = 1D;
                this.InitialLoanPercentBox.Editable = false;
            }
        }

        partial void FullAcquisitionCostToggled(NSButton sender)
        {
            if (this.FullAcquisitionCostCheckBox.State == NSCellStateValue.On)
            {
                // handle turn on by disabling user input for InitialLoanPercent (only applies if not funding full acquisition)
                this.InitialLoanPercentBox.DoubleValue = 1D;
                this.InitialLoanPercentBox.Editable = false;
            }
            else 
            {
                // handle turn off by allowing user input of % of price to be financed;  and make AcquisitionOnly (no Rehab)
                this.InitialLoanPercentBox.Editable = true;
                this.AcquisitionOnlyCheckBox.State = NSCellStateValue.On;
                this.RehabCostBox.DoubleValue = 0D;
                this.RehabCostBox.Editable = false;
            }
        }

        partial void AddButtonPushed(AppKit.NSButton sender)
        {
            // store values
            string address = this.AddressBox.StringValue;
            string town = this.TownBox.StringValue;
            string county = this.CountyBox.StringValue;
            string state = this.StateBox.StringValue;
            int titleHolderID = (int)this.TitleHolderPopUp.IndexOfSelectedItem-1;
            int coID = (int)this.CoBorrowerPopUp.IndexOfSelectedItem-1;
            int lenderID = (int)this.LenderPopUp.IndexOfSelectedItem-1;
            int titleID = (int)this.TitlePopUp.IndexOfSelectedItem-1;
            DateTime acquisitionDate = (DateTime)this.PurchaseDatePicker.DateValue;
            double price = this.PurchasePriceBox.DoubleValue;
            double bpo = this.BPOBox.DoubleValue;
            double rehabCost = this.RehabCostBox.DoubleValue;
            double pnl = this.PnLBox.DoubleValue;
            double months = this.MonthsToCompletionBox.DoubleValue;
            double loanRate = this.LoanRateBox.DoubleValue;
            double penaltyRate = this.DefaultRateBox.DoubleValue;
            double points = this.PointsBox.DoubleValue;
            double profitSplit = this.ProfitSplitBox.DoubleValue;
            bool acqOnly = (this.AcquisitionOnlyCheckBox.State == NSCellStateValue.On);
            bool bfullAcqCostFunded = (this.FullAcquisitionCostCheckBox.State == NSCellStateValue.On);
            double initialFundingMult = this.InitialLoanPercentBox.DoubleValue;

            // acquisition cost estimates
            CostEstimate costEstimateForState;
            double initialDraw = -4500D;
            double programFee = -2000D;
            double otherBudgetedCosts = initialDraw - programFee;  // accounting, travel, utilities, inspections, etc.
            double concessionPercentage = 0.02;
            double commissionPercentage = 0.06;
            double totalBackEndCosts;

            // other stuff
            double dispostion;
            double totalCommitment;
            double upfrontCommitment;
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
            else if ((titleHolderID == lenderID) || (titleHolderID == titleID) ||
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
            else if (price *  months *  bpo < 0.001)
            {
                this.UpdateMessage.StringValue = "Missing Values in price, BPO, or Months";
            }
            else if ((rehabCost <= 0) && (!acqOnly))
            {
                this.UpdateMessage.StringValue = "Rehab cost missing for non-acquisition-only loan";
            }
            else if ((pnl <= 0) && (profitSplit > 0))
            {
                this.UpdateMessage.StringValue = "Esimtated PnL Missing or Negative for loan with profit split";
            }
            // check things like lender elgible, borrower eligible, title company state eligible
            // Check valid address??
            // Check county??
            else
            {
                // Create new Property, Loan and Documents in tables
                if (coID == -1) { coID = titleHolderID; }
                clsProperty newProperty = new clsProperty(address, town, county, state, bpo, streetName);
                newProperty.Save();
                clsLoan newLoan = new clsLoan(newProperty.ID(), titleHolderID, coID, titleID, lenderID, acquisitionDate,
                                              acquisitionDate.AddMonths(9), loanRate, penaltyRate, points, profitSplit, acqOnly);
                int newLoanID = newLoan.ID();
                for (int i = 0; i < Enum.GetValues(typeof(clsDocument.Type)).Length; i++)
                {
                    clsDocument newDoc = new clsDocument(((clsDocument.Type)i).ToString(), newProperty.ID(), (clsDocument.Type)i);
                    newDoc.Save();
                }

                #region Create Cashflows, contingent on Loan Parameters

                costEstimateForState = new CostEstimate(state, price, bpo, months);

                if (bfullAcqCostFunded)
                {
                    totalCommitment = (price + rehabCost - initialDraw - costEstimateForState.AcquisitionClosingCosts) / (1D - points * 0.01);
                    upfrontCommitment = (totalCommitment - rehabCost / (1D - points * 0.01));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        -price, false, clsCashflow.Type.AcquisitionPrice));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        0D, false, clsCashflow.Type.AcquisitionConcession));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        costEstimateForState.ProcessingCost, false, clsCashflow.Type.AcquisitionProcessing));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        costEstimateForState.RecordingCost, false, clsCashflow.Type.AcquisitionRecording));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        costEstimateForState.AcquisitionTaxes, false, clsCashflow.Type.AcquisitionTaxes));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        costEstimateForState.HOICost, false, clsCashflow.Type.HomeownersInsurance));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        initialDraw, false, clsCashflow.Type.InitialExpenseDraw));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                        costEstimateForState.TitlePolicyCost, false, clsCashflow.Type.TitlePolicy));
                    if (points > 0)
                    {
                        // upfront points
                        newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                            0.01 * points * upfrontCommitment, false, clsCashflow.Type.Points));
                        newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                            -0.01 * points * upfrontCommitment, false, clsCashflow.Type.Points));
                    }
                    if (!acqOnly)
                    {
                        // assume entire rehab draw comes halfway through the rehab process, and that there is a two month lag from rehab completion to sale closing
                        newLoan.AddCashflow(new clsCashflow(acquisitionDate.AddMonths((int)(0.5 * (months - 2D))), System.DateTime.Today,
                                                            System.DateTime.MaxValue, newLoanID, -rehabCost, false, clsCashflow.Type.RehabDraw));
                        if (points > 0)
                        {
                            // points on construction draws
                            newLoan.AddCashflow(new clsCashflow(acquisitionDate.AddMonths((int)(0.5 * (months - 2D))), System.DateTime.Today, 
                                                                System.DateTime.MaxValue, newLoanID,
                                                                0.01 * points * rehabCost / (1D - points * 0.01), 
                                                                false, clsCashflow.Type.Points));
                            newLoan.AddCashflow(new clsCashflow(acquisitionDate.AddMonths((int)(0.5 * (months - 2D))), System.DateTime.Today, 
                                                                System.DateTime.MaxValue, newLoanID,
                                                                -0.01 * points * rehabCost / (1D - points * 0.01), 
                                                                false, clsCashflow.Type.Points));
                        }
                    }
                }
                else
                {
                    totalCommitment = price * initialFundingMult;
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, DateTime.Today, DateTime.MaxValue, newLoanID,
                                                -price, false, clsCashflow.Type.AcquisitionPrice));
                    newLoan.AddCashflow(new clsCashflow(acquisitionDate, DateTime.Today, DateTime.MaxValue, newLoanID,
                                                        -price * (initialFundingMult - 1D), false, clsCashflow.Type.InitialExpenseDraw));
                    if (points > 0)
                        newLoan.AddCashflow(new clsCashflow(acquisitionDate, System.DateTime.Today, System.DateTime.MaxValue, newLoanID,
                                                            0.01 * points * totalCommitment, false, clsCashflow.Type.Points));
                }

                // disposition cashflow
                hardInterestGuess = (totalCommitment - 0.5 * rehabCost) * (months / 12D) * loanRate;
                if (profitSplit > 0)
                {
                    totalBackEndCosts = costEstimateForState.SaleTaxes + hardInterestGuess + bpo * (concessionPercentage + commissionPercentage);
                    pnlGuess = bpo - (totalCommitment + totalBackEndCosts);
                    if (!bfullAcqCostFunded) { pnlGuess += -totalCommitment * points * 0.01; } // if points were payed upfront, subtract from PnL
                    UpdateMessage.StringValue += "\n Estimated vs Quoted PnL: \t";
                    UpdateMessage.StringValue += pnlGuess.ToString("000,000.00") + " vs " + pnl.ToString("000,000.00");
                }
                else
                    pnlGuess = 0D;
                dispostion = totalCommitment + hardInterestGuess + profitSplit * pnl;  // user-given pnl rather than system estimate
                newLoan.AddCashflow(new clsCashflow(acquisitionDate.AddMonths((int)months), System.DateTime.Today, System.DateTime.MaxValue,
                                                    newLoanID, dispostion, false, clsCashflow.Type.NetDispositionProj));
                #endregion

                newLoan.SetNewOriginationDate(acquisitionDate);
                newLoan.Save();

                // write summary to text label (this.UpdateMessage.StringValue)
                this.UpdateMessage.StringValue += String.Format("\n\nCompleted adding new loan ({0}), new cashflows, new property ({1}), and new docs ",
                                                             newLoan.ID().ToString(), 
                                                             newProperty.ID().ToString());
            }
        }
    }
}
