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

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            List<string> addressList = clsProperty.AddressList();
            this.AddressComboBox.RemoveAll();
            foreach (string address in addressList) { this.AddressComboBox.Add((NSString)address); }
            this.SaleDatePicker.DateValue = (NSDate)System.DateTime.Today.Date;
            this.ChooseActionPopUp.RemoveAllItems();
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
                else if (this.ChooseActionPopUp.TitleOfSelectedItem == "Mark Repaid")
                {
                    this.MarkRepaid();
                }
            }
            this.UpdateChosenAddress(false);
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

                switch (this.loan.Status())
                {
                    case clsLoan.State.PendingSale:
                        this.ChooseActionPopUp.AddItem("Cancel");
                        this.ChooseActionPopUp.AddItem("Update");
                        if (this.loan.SaleDate() <= System.DateTime.Today) this.ChooseActionPopUp.AddItem("Mark Repaid");
                        this.SaleDatePicker.DateValue = (NSDate)this.loan.SaleDate().Date.ToUniversalTime();
                        this.ExpectedSalePriceTextField.DoubleValue = 0D;
                        this.ExpectedAdditionalInterestTextField.DoubleValue = this.loan.ScheduledAdditionalInterest(System.DateTime.Today.Date);
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
                        this.ExpectedSalePriceTextField.DoubleValue = 0D;
                        this.ExpectedAdditionalInterestTextField.DoubleValue = 0D;
                        this.SaleDatePicker.DateValue = (NSDate)System.DateTime.Today.Date;
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
                double dHardInterst = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dHardInterst, false, clsCashflow.Type.InterestHard));
                this.StatusMessageTextField.StringValue += "\nInterest  Added: " + dHardInterst.ToString("000,000.00");
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
                double dHardInterst = this.loan.LoanAsOf(scheduledSale).AccruedInterest(scheduledSale);
                this.loan.AddCashflow(new clsCashflow(scheduledSale, System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dHardInterst, false, clsCashflow.Type.InterestHard));
                this.StatusMessageTextField.StringValue += "\nInterest  Added: " + dHardInterst.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.ToString("MM/dd/yyyy");
                // schedule additional interest
                double dAdditionalInterest = this.ExpectedAdditionalInterestTextField.DoubleValue;
                this.loan.AddCashflow(new clsCashflow(scheduledSale.AddDays(7), System.DateTime.Today, System.DateTime.MaxValue,
                                                      this.loan.ID(), dAdditionalInterest, false, clsCashflow.Type.InterestAdditional));
                this.StatusMessageTextField.StringValue += "\nAddlInt   Added: " + dAdditionalInterest.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "," + scheduledSale.AddDays(7).ToString("MM/dd/yyyy");
                this.StatusMessageTextField.StringValue += "\nPrev    AddlInt: " + dImpliedAdditional.ToString("000,000.00");
                this.StatusMessageTextField.StringValue += "\nSave = " + this.loan.Save().ToString();
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
                this.loan.AddCashflow(new clsCashflow(this.loan.SaleDate(), System.DateTime.Today, System.DateTime.MaxValue,
                                                     this.loan.ID(), dAdditionalInterestPaidAtClosing, true, clsCashflow.Type.InterestAdditional));
            }
        }
    }
}
