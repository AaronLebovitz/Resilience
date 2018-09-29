using System;
using ResilienceClasses;
using System.Collections.Generic;
using AppKit;
using Foundation;

namespace ManageSales
{
    public partial class ViewController : NSViewController
    {

        private clsLoan loan;
        private bool bRecordSaleContract = false;
        private clsLoan.State statusFilter = clsLoan.State.Unknown;
        private int lenderID = -1;
        private Dictionary<string, clsLoan> loansByAddress = new Dictionary<string, clsLoan>();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // load Loans By Address
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                clsLoan l = new clsLoan(i);
                loansByAddress.Add(l.Property().Address(), l);
            }

            // Do any additional setup after loading the view.
            this.UpdateAddressList();
            this.SaleDatePicker.DateValue = (NSDate)System.DateTime.Today.Date;
            this.ChooseActionPopUp.RemoveAllItems();
            this.RecordDatePicker.DateValue = (NSDate)System.DateTime.Today.Date;
            this.LenderComboBox.RemoveAll();
            clsCSVTable tblEntities = new clsCSVTable(clsEntity.strEntityPath);
            for (int i = 0; i < tblEntities.Length(); i++)
            {
                if (tbl.Matches(clsLoan.LenderColumn,i.ToString()).Count > 0)
                    this.LenderComboBox.Add((NSString)(new clsEntity(i)).Name());
            }
            this.StatusComboBox.RemoveAll();
            foreach (clsLoan.State c in Enum.GetValues(typeof(clsLoan.State)))
                this.StatusComboBox.Add((NSString)c.ToString());
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

        partial void AddressChosen(NSComboBox sender)
        {
            this.UpdateChosenAddress();
        }

        partial void GoButtonPressed(NSButton sender)
        {
            if (this.ChooseActionPopUp.SelectedItem == null)
            {
                this.StatusMessageTextField.StringValue = "No Action Chosen / Available.  Nothing was updated.";
            }
            else
            {
                if (this.ChooseActionPopUp.TitleOfSelectedItem == "Schedule")
                {
                    this.ScheduleNewSale();
                }
                else if (this.ChooseActionPopUp.TitleOfSelectedItem == "Update")
                {
                    if (StatusTextField.StringValue == clsLoan.State.PendingSale.ToString())
                        this.UpdateScheduledSale();
                    else
                        this.UpdateProjectedDisposition();
                }
                else if (this.ChooseActionPopUp.TitleOfSelectedItem == "Cancel")
                {
                    this.CancelScheduledSale();
                }
                else if (this.ChooseActionPopUp.TitleOfSelectedItem == "Mark Loan Repaid")
                {
                    this.MarkRepaid();
                }
                else if (this.ChooseActionPopUp.TitleOfSelectedItem == "Mark Addl Int Actual")
                {
                    this.MarkAdditionalInterestActual();
                }
                else if (this.ChooseActionPopUp.TitleOfSelectedItem == "Update/Add")
                {
                    this.UpdateScheduledAdditionalInterest();
                }
            }
            this.UpdateChosenAddress(false);
        }

        partial void ContractReceivedToggled(NSButton sender)
        {
            this.bRecordSaleContract = !this.bRecordSaleContract;
        }

        private void UpdateChosenAddress(bool clearMessageText = true)
        {
            this.ChooseActionPopUp.RemoveAllItems();
            if (clsLoan.LoanID(this.AddressComboBox.StringValue) >= 0)
            {
                this.loan = new clsLoan(clsLoan.LoanID(this.AddressComboBox.StringValue));
                if (clearMessageText)
                    this.StatusMessageTextField.StringValue = "";
                this.StatusTextField.StringValue = this.loan.Status().ToString();
                this.RepaymentAmountTextField.DoubleValue = 0D;
                this.SetDefaultLabels();

                switch (this.loan.Status())
                {
                    case clsLoan.State.PendingSale:
                        this.ChooseActionPopUp.AddItem("Cancel");
                        this.ChooseActionPopUp.AddItem("Update");
                        if (this.loan.SaleDate() <= System.DateTime.Today) this.ChooseActionPopUp.AddItem("Mark Loan Repaid");
                        this.SaleDatePicker.DateValue = (NSDate)this.loan.SaleDate().Date.ToUniversalTime();
                        this.ExpectedSalePriceTextField.DoubleValue = 0D;
                        this.ExpectedAdditionalInterestTextField.DoubleValue = this.loan.ScheduledAdditionalInterest(System.DateTime.Today.Date);

                        DateTime scheduledSale = this.loan.SaleDate().Date;
                        double dPrincipalRepay = this.loan.LoanAsOf(scheduledSale.AddDays(1)).PrincipalPaid(scheduledSale.AddDays(1));
                        double dHardInterest = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);
                        double perdiem = dHardInterest - this.loan.LoanAsOf(scheduledSale.AddDays(-1)).AccruedInterest(scheduledSale.AddDays(-1));
                        double dAdditionalInterest = this.ExpectedAdditionalInterestTextField.DoubleValue;
                        this.ShowLoanPayoffLetterInfo(dPrincipalRepay, dHardInterest, perdiem);

                        break;

                    case clsLoan.State.Listed:
                    case clsLoan.State.Rehab:
                        this.ChooseActionPopUp.AddItem("Schedule");
                        this.ChooseActionPopUp.AddItem("Update");
                        this.SaleDatePicker.DateValue = (NSDate)this.loan.SaleDate().Date.ToUniversalTime();
                        this.ExpectedSalePriceTextField.DoubleValue = 0D;
                        this.ExpectedAdditionalInterestTextField.DoubleValue = this.loan.ProjectedAdditionalInterest(System.DateTime.Today.Date);
                        break;

                    case clsLoan.State.PendingAcquisition:
                        this.ChooseActionPopUp.AddItem("Update");
                        this.SaleDatePicker.DateValue = (NSDate)this.loan.SaleDate().Date.ToUniversalTime();
                        this.ExpectedSalePriceTextField.DoubleValue = 0D;
                        this.ExpectedAdditionalInterestTextField.DoubleValue = this.loan.ProjectedAdditionalInterest(System.DateTime.Today.Date);
                        break;

                    case clsLoan.State.Unknown:
                    case clsLoan.State.Cancelled:
                    case clsLoan.State.Sold:
                    default:
                        this.SetSoldLabels();
                        this.ChooseActionPopUp.AddItem("Update/Add");
                        this.ChooseActionPopUp.AddItem("Mark Addl Int Actual");
                        this.ExpectedSalePriceTextField.DoubleValue = 0D;
                        this.ExpectedAdditionalInterestTextField.DoubleValue = this.loan.ScheduledAdditionalInterest() 
                                                                             + this.loan.PastDueAdditionalInterest();
                        this.SaleDatePicker.DateValue = 
                            (NSDate)this.loan.FindDate(clsCashflow.Type.InterestAdditional,false,true).ToUniversalTime();
                        break;

                }
            }
        }

        private void UpdateScheduledSale()
        {
            //   cancel principal, move to new date
            //   cancel interest, calculate new accrued to new date
            //   cancel additional, move to new date, reduce by 1/2 (new accrued - old accrued)
            DateTime scheduledSale = (DateTime)this.SaleDatePicker.DateValue;
            if (scheduledSale <= System.DateTime.Today.Date)
            {
                this.StatusMessageTextField.StringValue = "Can't reschedule to a date in the past.";
            }
            else
            {
                // check to make sure scheduled date is after all rehabs
                if (scheduledSale < this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true))
                {
                    scheduledSale = this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true).AddDays(1);
                    this.StatusMessageTextField.StringValue =
                        "Proposed Sale Date is before last Rehab Date.  \nChanging Sale Date to " + scheduledSale.ToString("MM/dd/yyyy");
                }

                // expire Scheduled Principal, Interest Payments;  Accumulate AdditionalInterest
                double dAdditionalInterest = 0D;
                double dHardInterest = 0D;
                double dAccrued = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);

                foreach (clsCashflow cf in this.loan.Cashflows())
                {
                    if (cf.DeleteDate() > System.DateTime.Today.AddYears(50))
                    {
                        if (cf.TypeID() == clsCashflow.Type.Principal)
                        {
                            if (cf.Delete(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "\nScheduled Principal Repayment Deleted as of Today.";
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestHard)
                        {
                            dHardInterest += cf.Amount();
                            if (cf.Delete(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "\nScheduled Hard Interest Deleted as of Today.";
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestAdditional)
                        {
                            dAdditionalInterest += cf.Amount();
                            if (cf.Delete(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "\nScheduled Additional Interest Deleted as of Today.";
                        }
                    }
                }

                // schedule principal repay
                double dPrincipalRepay = this.loan.LoanAsOf(scheduledSale).Balance(scheduledSale);
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dPrincipalRepay, false, clsCashflow.Type.Principal));
                this.StatusMessageTextField.StringValue += "\nPrincipal Added: " + dPrincipalRepay.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.ToString("MM/dd/yyyy");
                // schedule hard interest
                dHardInterest = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);
                double perdiem = dHardInterest - this.loan.LoanAsOf(scheduledSale.AddDays(-1)).AccruedInterest(scheduledSale.AddDays(-1));
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dHardInterest, false, clsCashflow.Type.InterestHard));
                this.StatusMessageTextField.StringValue += "\nInterest  Added: " + dHardInterest.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.ToString("MM/dd/yyyy");
                // schedule additional interest
                double dPrevAdditional = dAdditionalInterest;
                dAdditionalInterest += this.ExpectedAdditionalInterestTextField.DoubleValue;
                this.loan.AddCashflow(new clsCashflow(scheduledSale.AddDays(7), System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dAdditionalInterest, false, clsCashflow.Type.InterestAdditional));
                this.StatusMessageTextField.StringValue += "\nAddlInt   Added: " + dAdditionalInterest.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.AddDays(7).ToString("MM/dd/yyyy");
                this.StatusMessageTextField.StringValue += "\nPrev    AddlInt: " + dPrevAdditional.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "\nSave = " + this.loan.Save().ToString();

                this.ShowLoanPayoffLetterInfo(dPrincipalRepay, dHardInterest, perdiem);
            }
        }

        private void UpdateProjectedDisposition()
        {
            DateTime scheduledSale = (DateTime)this.SaleDatePicker.DateValue;
            if (scheduledSale <= System.DateTime.Today.Date)
            {
                this.StatusMessageTextField.StringValue = "Can't Reschedule to a date in the past.";
            }
            else
            {
                double dHardInterest = this.loan.ProjectedHardInterest();
                // check to make sure scheduled date is after all rehabs
                if (scheduledSale < this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true))
                {
                    scheduledSale = this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true).AddDays(1);
                    this.StatusMessageTextField.StringValue =
                        "Proposed Sale Date is before last Rehab Date.  \nChanging Sale Date to " + scheduledSale.ToString("MM/dd/yyyy");
                }
                double dImpliedAdditional = this.loan.ImpliedAdditionalInterest();

                // expire Projected Disposition
                foreach (clsCashflow cf in this.loan.Cashflows())
                {
                    if ((cf.TypeID() == clsCashflow.Type.NetDispositionProj) && (cf.DeleteDate() > System.DateTime.Today.AddYears(50)))
                    {
                        this.StatusMessageTextField.StringValue += "\nProjected Disposition Deleted as of Today.\n";
                        this.StatusMessageTextField.StringValue += cf.Amount().ToString("000,000.00") + " : " + cf.PayDate().ToString("MM/dd/yyyy");
                        cf.Delete(System.DateTime.Today);
                    }
                }
                double dAccrued = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);
                double dDispAmount = this.loan.LoanAsOf(scheduledSale).Balance(scheduledSale);
                dDispAmount += dAccrued + dImpliedAdditional + this.ExpectedAdditionalInterestTextField.DoubleValue;
                dDispAmount += 0.5 * (dHardInterest - dAccrued); // adjustment to additional interest for change in hard interest
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dDispAmount, false, clsCashflow.Type.NetDispositionProj));

                this.StatusMessageTextField.StringValue += "\nNew Projected: " + dDispAmount.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += " : " + scheduledSale.ToString("MM/dd/yyyy");
                this.StatusMessageTextField.StringValue += "\nSave = " + this.loan.Save().ToString();
            }
        }

        private void ScheduleNewSale()
        {
            DateTime scheduledSale = (DateTime)this.SaleDatePicker.DateValue;
            if (scheduledSale <= System.DateTime.Today.Date)
            {
                this.StatusMessageTextField.StringValue = "Can't schedule a new sale in the past.";
            }
            else
            {
                // check to make sure scheduled date is after all rehabs
                if (scheduledSale < this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true))
                {
                    scheduledSale = this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true).AddDays(1);
                    this.StatusMessageTextField.StringValue =
                        "Proposed Sale Date is before last Rehab Date.  \nChanging Sale Date to " + scheduledSale.ToString("MM/dd/yyyy");
                }
                double dImpliedAdditional = this.loan.ImpliedAdditionalInterest();

                // expire Projected Disposition
                foreach (clsCashflow cf in this.loan.Cashflows())
                {
                    if ((cf.TypeID() == clsCashflow.Type.NetDispositionProj) && (cf.DeleteDate() > System.DateTime.Today.AddYears(50)))
                    {
                        this.StatusMessageTextField.StringValue += "\nProjected Disposition Deleted as of Today.";
                        cf.Delete(System.DateTime.Today);
                    }
                }

                // schedule principal repay
                double dPrincipalRepay = this.loan.LoanAsOf(scheduledSale).Balance(scheduledSale);
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dPrincipalRepay, false, clsCashflow.Type.Principal));
                this.StatusMessageTextField.StringValue += "\nPrincipal Added: " + dPrincipalRepay.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.ToString("MM/dd/yyyy");
                // schedule hard interest
                double dHardInterest = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);
                double perdiem = dHardInterest - this.loan.LoanAsOf(scheduledSale.AddDays(-1)).AccruedInterest(scheduledSale.AddDays(-1));
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dHardInterest, false, clsCashflow.Type.InterestHard));
                this.StatusMessageTextField.StringValue += "\nInterest  Added: " + dHardInterest.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.ToString("MM/dd/yyyy");
                // schedule additional interest
                double dAdditionalInterest = this.ExpectedAdditionalInterestTextField.DoubleValue;
                this.loan.AddCashflow(new clsCashflow(scheduledSale.AddDays(7), System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dAdditionalInterest, false, clsCashflow.Type.InterestAdditional));
                this.StatusMessageTextField.StringValue += "\nAddlInt   Added: " + dAdditionalInterest.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.AddDays(7).ToString("MM/dd/yyyy");
                this.StatusMessageTextField.StringValue += "\nPrev    AddlInt: " + dImpliedAdditional.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "\nSave = " + this.loan.Save().ToString();

                this.ShowLoanPayoffLetterInfo(dPrincipalRepay, dHardInterest, perdiem);

                // Record Sale Contract
                if (this.bRecordSaleContract)
                {
                    clsDocument saleContract = new clsDocument(clsDocument.DocumentID(this.loan.PropertyID(), clsDocument.Type.SaleContract));
                    clsDocumentRecord saleContractRecord = new clsDocumentRecord(saleContract.ID(),
                                                                                 System.DateTime.Now,
                                                                                (DateTime)this.RecordDatePicker.DateValue,
                                                                                 this.loan.CoBorrowerID(),
                                                                                 this.loan.LenderID(),
                                                                                 clsDocumentRecord.Status.Preliminary,
                                                                                 clsDocumentRecord.Transmission.Electronic);
                    if (saleContractRecord.Save())
                        this.StatusMessageTextField.StringValue += "\nSale Contract Recorded";
                    else
                        this.StatusMessageTextField.StringValue += "\nSale Contract FAILED TO RECORD";
                }
                else
                    this.StatusMessageTextField.StringValue += "\nSale Contract NOT RECORDED";
            }
        }

        private void CancelScheduledSale()
        {
            // delete scheduled Principal, HardInterest, AdditionalInterest
            // add new ProjDisposition in amount = Principal, Accrued As Of (new proj date), 
            //    Prior Additional Interest - Change in Hard Interest
            DateTime scheduledSale = (DateTime)this.SaleDatePicker.DateValue;
            if (scheduledSale <= System.DateTime.Today.Date)
            {
                this.StatusMessageTextField.StringValue = "Can't reschedule to a date in the past.";
            }
            else
            {
                // check to make sure scheduled date is after all rehabs
                if (scheduledSale < this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true))
                {
                    scheduledSale = this.loan.FindDate(clsCashflow.Type.RehabDraw, false, true).AddDays(1);
                    this.StatusMessageTextField.StringValue =
                        "Proposed Sale Date is before last Rehab Date.  \nChanging Sale Date to " + scheduledSale.ToString("MM/dd/yyyy");
                }

                // expire Scheduled Principal, Interest Payments;  Accumulate AdditionalInterest
                double dAdditionalInterest = 0D;
                double dHardInterest = 0D;
                double dAccrued = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);

                foreach (clsCashflow cf in this.loan.Cashflows())
                {
                    if (cf.DeleteDate() > System.DateTime.Today.AddYears(50))
                    {
                        if (cf.TypeID() == clsCashflow.Type.Principal)
                        {
                            if (cf.Delete(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "\nScheduled Principal Repayment Deleted as of Today.";
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestHard)
                        {
                            dHardInterest += cf.Amount();
                            if (cf.Delete(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "\nScheduled Hard Interest Deleted as of Today.";
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestAdditional)
                        {
                            dAdditionalInterest += cf.Amount();
                            if (cf.Delete(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "\nScheduled Additional Interest Deleted as of Today.";
                        }
                    }
                }
                // new Disposition Projection = Principal + new Accrued As Of Projected Sale Date + current Projected Addl adjusted for 
                //   change in HardInterest
                double dDispAmount = this.loan.LoanAsOf(scheduledSale).Balance() + dAccrued + dAdditionalInterest;
                dDispAmount += 0.5 * (dHardInterest - dAccrued); // adjustment to additional interest for change in hard interest
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dDispAmount, false, clsCashflow.Type.NetDispositionProj));

                this.StatusMessageTextField.StringValue += "\nProjected Disposition Added: " + dDispAmount.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.ToString("MM/dd/yyyy");
                this.StatusMessageTextField.StringValue += "\nSave = " + this.loan.Save().ToString();
            }
        }

        private void MarkRepaid()
        {
            double dAdditionalInterestPaidAtClosing = this.RepaymentAmountTextField.DoubleValue;
            double dHardInterest = 0D;
            double dAdditionalInterest = 0D;

            // first check that amount is sufficient
            foreach (clsCashflow cf in this.loan.Cashflows())
            {
                if (cf.DeleteDate() > System.DateTime.Today.AddYears(50))
                {
                    if (cf.TypeID() == clsCashflow.Type.Principal)
                        dAdditionalInterestPaidAtClosing -= cf.Amount();
                    else if (cf.TypeID() == clsCashflow.Type.InterestHard)
                    {
                        dHardInterest += cf.Amount();
                        dAdditionalInterestPaidAtClosing -= cf.Amount();
                    }
                    else if (cf.TypeID() == clsCashflow.Type.InterestAdditional)
                        dAdditionalInterest += cf.Amount();
                }
            }

            if (dAdditionalInterestPaidAtClosing < 0)
            {
                this.StatusMessageTextField.StringValue += "\nRepayment Amount is too low.  No updates made.";
            }
            else
            {
                foreach (clsCashflow cf in this.loan.Cashflows())
                {
                    if (cf.DeleteDate() > System.DateTime.Today.AddYears(50))
                    {
                        if (cf.TypeID() == clsCashflow.Type.Principal)
                        {
                            this.StatusMessageTextField.StringValue += "\nScheduled Principal Payment :  ";
                            this.StatusMessageTextField.StringValue += cf.Amount().ToString("#,##0.00");
                            if (cf.MarkActual(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "  marked paid.";
                            else
                                this.StatusMessageTextField.StringValue += "  FAILED to mark paid.";
                        }
                        else if (cf.TypeID() == clsCashflow.Type.InterestHard)
                        {
                            dHardInterest += cf.Amount();
                            this.StatusMessageTextField.StringValue += "\nScheduled Hard Interest :  ";
                            this.StatusMessageTextField.StringValue += cf.Amount().ToString("#,##0.00");
                            if (cf.MarkActual(System.DateTime.Today))
                                this.StatusMessageTextField.StringValue += "  marked paid.";
                            else
                                this.StatusMessageTextField.StringValue += "  FAILED to mark paid.";
                        }
                    }
                }
                // if an extra days of interest were paid, add this cashflow as Additional Interest paid on the closing date
                //    don't adjust scheduled additional interest, this will be updated soon after closing
                if (dAdditionalInterestPaidAtClosing > 0)
                {
                    this.loan.AddCashflow(new clsCashflow(this.loan.SaleDate(), System.DateTime.Today, System.DateTime.MaxValue,
                                                         this.loan.ID(), dAdditionalInterestPaidAtClosing, true, clsCashflow.Type.InterestAdditional));
                    this.StatusMessageTextField.StringValue += "\nAddl Interest Paid at Closing added: ";
                    this.StatusMessageTextField.StringValue += dAdditionalInterestPaidAtClosing.ToString("#,##0.00");
                }
                else
                    this.StatusMessageTextField.StringValue += "\nNO Additional Interest Paid at Closing.";
            }
            this.StatusMessageTextField.StringValue += "\nLoan Save to File : " + this.loan.Save().ToString().ToUpper();
        }

        private void ShowLoanPayoffLetterInfo(double principal, double interest, double perDiem)
        {
            this.StatusMessageTextField.StringValue += "\nPrincipal: " + principal.ToString("#,##0.00");
            this.StatusMessageTextField.StringValue += "\nInterest : " + interest.ToString("#,##0.00");
            this.StatusMessageTextField.StringValue += "\nTotal    : " + (principal + interest).ToString("#,##0.00");
            this.StatusMessageTextField.StringValue += "\nPer Diem : " + perDiem.ToString("#,##0.00");
        }

        private void MarkAdditionalInterestActual()
        {
            double dAddlInterestReceived = 0D;
            this.StatusMessageTextField.StringValue = "";
            foreach (clsCashflow cf in this.loan.Cashflows())
            {
                if ((cf.TypeID() == clsCashflow.Type.InterestAdditional) 
                    && (cf.PayDate() <= System.DateTime.Today) 
                    && (!cf.Actual())
                    && (cf.DeleteDate() > System.DateTime.Today))
                {
                    if (cf.MarkActual(System.DateTime.Today))
                    {
                        this.StatusMessageTextField.StringValue += cf.Amount().ToString("#,##0.00") + "(" + cf.TransactionID().ToString() + ") Marked ACTUAL\n";
                        dAddlInterestReceived += cf.Amount();
                    }
                    else
                        this.StatusMessageTextField.StringValue += "!" + cf.TransactionID().ToString() + "! FAILED\n";
                }
            }
            this.StatusMessageTextField.StringValue += "Total Marked Actual: " + dAddlInterestReceived.ToString("#,##0.00");
            this.loan.Save();
        }

        private void UpdateScheduledAdditionalInterest()
        {
            double dAddlInterestExpired = 0D;
            this.StatusMessageTextField.StringValue = "";
            foreach (clsCashflow cf in this.loan.Cashflows())
            {
                if ((cf.TypeID() == clsCashflow.Type.InterestAdditional) && (!cf.Actual()) && (cf.DeleteDate() > System.DateTime.Today))
                {
                    if (cf.Delete(System.DateTime.Today))
                    {
                        this.StatusMessageTextField.StringValue += cf.Amount().ToString("#,##0.00") + "(" + cf.TransactionID().ToString() + ") EXPIRED\n";
                        dAddlInterestExpired += cf.Amount();
                    }
                    else
                        this.StatusMessageTextField.StringValue += "!" + cf.TransactionID().ToString() + "! FAILED\n";
                }
            }
            loan.AddCashflow(new clsCashflow((DateTime)this.SaleDatePicker.DateValue,
                                             (DateTime)this.RecordDatePicker.DateValue,
                                             System.DateTime.MaxValue,
                                             this.loan.ID(),
                                             this.ExpectedAdditionalInterestTextField.DoubleValue,
                                             false,
                                             clsCashflow.Type.InterestAdditional));
            this.StatusMessageTextField.StringValue += "\nTotal Expired: " + dAddlInterestExpired.ToString("#,##0.00");
            this.loan.Save();
        }

        private void SetDefaultLabels()
        {
            this.SalePriceLabel.StringValue = "Expected Sale Price (Net)";
            this.CashflowDateLabel.StringValue = "Expected Sale Date";
            this.RepaymentAmountLabel.StringValue = "Repayment Amount Received";
            this.AdditionalInterestLabel.StringValue = "Expected Additional Interest";
        }

        private void SetSoldLabels()
        { 
            this.SalePriceLabel.StringValue = "N/A";
            this.CashflowDateLabel.StringValue = "Addl Interest Pay Date";
            this.RepaymentAmountLabel.StringValue = "N/A";
            this.AdditionalInterestLabel.StringValue = "Addl Interest Amount";
        }

        private void UpdateAddressList()
        {
            this.AddressComboBox.RemoveAll();
            foreach (string address in clsProperty.AddressList()) 
            { 
                if (((this.lenderID < 0) || (this.lenderID == loansByAddress[address].LenderId)) &&
                    ((this.statusFilter == clsLoan.State.Unknown) || (this.statusFilter == loansByAddress[address].Status())))
                {
                    this.AddressComboBox.Add((NSString)address);
                }
            }
        }

        partial void LenderChosen(NSComboBox sender)
        {
            this.lenderID = (int)this.LenderComboBox.SelectedIndex;
            this.UpdateAddressList();
        }

        partial void StatusChosen(NSComboBox sender)
        {
            this.statusFilter = (clsLoan.State)((int)this.StatusComboBox.SelectedIndex);
            this.UpdateAddressList();
        }
    }
}
