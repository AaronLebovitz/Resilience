﻿using System;

using AppKit;
using Foundation;
using ResilienceClasses;
using System.Collections.Generic;
using Xceed.Words.NET;

namespace UpdateAcquisition
{
    public partial class ViewController : NSViewController
    {

        #region Private Variables
        clsLoan loanToUpdate;
        #endregion

        #region Constructors
        public ViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region WiredActions
        partial void PropertyChosen(AppKit.NSComboBox sender)
        {
            if (this.AddressComboBox.StringValue != "")
            {
                this.loanToUpdate = new clsLoan(clsLoan.LoanID(this.AddressComboBox.StringValue));
                DateTime OGDate = loanToUpdate.OriginationDate();
                this.ClosingDatePicker.DateValue = (NSDate)OGDate.ToUniversalTime();

                //clear labels
                this.ConcessionLabel.StringValue = "";
                this.HOILabel.StringValue = "";
                this.PriceLabel.StringValue = "";
                this.AcqTaxLabel.StringValue = "";
                this.RecordingLabel.StringValue = "";
                this.ProcessingLabel.StringValue = "";
                this.ClosingDateLabel.StringValue = "";
                this.InitialDrawLabel.StringValue = "";
                this.PropertyTaxLabel.StringValue = "";
                this.TitlePolicyLabel.StringValue = "";
                this.PointsLabel.StringValue = "";

                //reload labels
                foreach (clsCashflow cf in this.loanToUpdate.Cashflows())
                {
                    if (cf.DeleteDate() > System.DateTime.Today.AddYears(100))
                    {
                        string amount = (Math.Abs(cf.Amount())).ToString("#,##0.00");
                        switch (cf.TypeID())
                        {
                            case clsCashflow.Type.AcquisitionConcession:
                                this.ConcessionLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.AcquisitionPrice:
                                this.PriceLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.AcquisitionProcessing:
                                this.ProcessingLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.AcquisitionRecording:
                                this.RecordingLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.AcquisitionTaxes:
                                this.AcqTaxLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.TitlePolicy:
                                this.TitlePolicyLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.HomeownersInsurance:
                                this.HOILabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.InitialExpenseDraw:
                                this.InitialDrawLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.PropertyTax:
//                                amount = (-cf.Amount()).ToString("#,##0.00");
                                this.PropertyTaxLabel.StringValue = amount;
                                break;
                            case clsCashflow.Type.Points:
                                this.PointsLabel.StringValue = amount;
                                break;
                            default:
                                break;
                        }
                    }
                }
                this.HOIField.StringValue = this.HOILabel.StringValue;
                this.PriceField.StringValue = this.PriceLabel.StringValue;
                this.AcqTaxField.StringValue = this.AcqTaxLabel.StringValue;
                this.RecordingField.StringValue = this.RecordingLabel.StringValue;
                this.ConcessionField.StringValue = this.ConcessionLabel.StringValue;
                this.ProcessingField.StringValue = this.ProcessingLabel.StringValue;
                this.ClosingDatePicker.DateValue = (NSDate)this.loanToUpdate.OriginationDate().ToUniversalTime();
                this.InitialDrawField.StringValue = this.InitialDrawLabel.StringValue;
                this.PropertyTaxField.StringValue = this.PropertyTaxLabel.StringValue;
                this.TitlePolicyField.StringValue = this.TitlePolicyLabel.StringValue;
                clsEntity lender = new clsEntity(this.loanToUpdate.LenderID);
                this.LenderComboBox.Select((NSString)lender.Name());
                clsEntity borrower = new clsEntity(this.loanToUpdate.BorrowerID);
                this.BorrowerComboBox.Select((NSString)borrower.Name()); 
                this.LenderLabel.StringValue = (NSString)this.LenderComboBox.SelectedValue;
                this.BorrowerLabel.StringValue = (NSString)this.BorrowerComboBox.SelectedValue;
                this.UpdatedPointsLabel.StringValue = this.PointsLabel.StringValue;
                this.UpdateTotalCostLabel();
            }
        }

        partial void MarkActualPressed(NSButton sender)
        {
            this.SummaryMessageField.StringValue = "";
            if (this.loanToUpdate == null)
            {
                this.SummaryMessageField.StringValue = "No loan selected.  No updates made.";
            }
            else if (this.loanToUpdate.Status() != clsLoan.State.Cancelled)
            {
                double dTotalCost = 0D;
                foreach (clsCashflow cf in this.loanToUpdate.Cashflows())
                {
                    if ((cf.DeleteDate() > System.DateTime.Today.AddYears(100)) && (cf.PayDate() <= System.DateTime.Today))
                    {
                        if (cf.MarkActual(System.DateTime.Today))
                        {
                            this.SummaryMessageField.StringValue += "Marked actual " + cf.TypeID().ToString() +
                                " (" + cf.TransactionID().ToString("#") + ")\n";
                            dTotalCost += cf.Amount();
                        }
                        else
                            this.SummaryMessageField.StringValue += "FAILED to mark actual " + cf.TypeID().ToString() +
                                " (" + cf.TransactionID().ToString("#") + ")\n";
                    }
                }
                this.SummaryMessageField.StringValue += "TOTAL Marked Actual = " + dTotalCost.ToString("#,##0.00");
                this.SummaryMessageField.StringValue += "\nLoan Save to Files " + this.loanToUpdate.Save().ToString().ToUpper() + "  " + this.loanToUpdate.Property().Address();
            }
            else
                this.SummaryMessageField.StringValue = "Failed to mark actual - loan has already been cancelled.  " + this.loanToUpdate.Property().Address();
        }

        partial void CancelButtonPushed(NSButton sender)
        {
            if (this.loanToUpdate == null)
                this.SummaryMessageField.StringValue = "No loan selected.  No updates made.";
            else
            {
                if (this.loanToUpdate.Status() != clsLoan.State.Cancelled)
                {
                    this.loanToUpdate.Cancel();
                    if (this.loanToUpdate.Save())
                    {
                        this.SummaryMessageField.StringValue = "Cancel successful.  " + this.loanToUpdate.Property().Address();
                    }
                    else
                    {
                        this.SummaryMessageField.StringValue = "Cancel Failed on Save.  " + this.loanToUpdate.Property().Address();
                    }
                }
                else
                    this.SummaryMessageField.StringValue = "Cancel Failed:  Loan already cancelled.  " + this.loanToUpdate.Property().Address();
            }
        }

        partial void UpdateButtonPushed(AppKit.NSButton sender)
        {
            this.SummaryMessageField.StringValue = "";
            if (this.loanToUpdate == null)
                this.SummaryMessageField.StringValue = "No loan selected.  No updates made.";
            else if (this.loanToUpdate.Status() != clsLoan.State.Cancelled)
            {
                // only validaton is:  are any cashflows ACTUAL
                bool bAnyActuals = false;
                double oldAcqCost = this.loanToUpdate.AcquisitionCost(false);
                foreach (clsCashflow cf in this.loanToUpdate.Cashflows())
                {
                    if (cf.Actual()) bAnyActuals = true;
                }
                if (bAnyActuals)
                {
                    this.SummaryMessageField.StringValue = "Can't update - some cashflows are marked Actual";
                }
                else
                {
                    int delayDays = ((DateTime)this.ClosingDatePicker.DateValue - this.loanToUpdate.OriginationDate()).Days;
                    List<clsCashflow> newCashflows = new List<clsCashflow>();
                    // delete all existing scheduled cashflows
                    foreach (clsCashflow cf in this.loanToUpdate.Cashflows())
                    {
                        if (cf.DeleteDate() > System.DateTime.Today.AddYears(50))
                        {
                            if ((cf.TypeID() == clsCashflow.Type.NetDispositionProj) ||
                                (cf.TypeID() == clsCashflow.Type.RehabDraw))
                            {
                                newCashflows.Add(new clsCashflow(cf.PayDate().AddDays(delayDays),
                                                 System.DateTime.Today, System.DateTime.MaxValue,
                                                 this.loanToUpdate.ID(), cf.Amount(), false, cf.TypeID()));
                            }
                            cf.Delete(System.DateTime.Today);
                        }
                    }
                    // create all the new cashflows
                    this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.PriceField.DoubleValue,
                                                                  false, clsCashflow.Type.AcquisitionPrice));
                    if (this.ConcessionField.DoubleValue > 0)
                    this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), this.ConcessionField.DoubleValue,
                                                                  false, clsCashflow.Type.AcquisitionConcession));
                    if (this.HOIField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.HOIField.DoubleValue,
                                                                  false, clsCashflow.Type.HomeownersInsurance));
                    if (this.AcqTaxField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.AcqTaxField.DoubleValue,
                                                                  false, clsCashflow.Type.AcquisitionTaxes));
                    if (this.RecordingField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.RecordingField.DoubleValue,
                                                                  false, clsCashflow.Type.AcquisitionRecording));
                    if (this.ProcessingField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.ProcessingField.DoubleValue,
                                                                  false, clsCashflow.Type.AcquisitionProcessing));
                    if (this.TitlePolicyField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.TitlePolicyField.DoubleValue,
                                                                  false, clsCashflow.Type.TitlePolicy));
                    if (this.InitialDrawField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.InitialDrawField.DoubleValue,
                                                                  false, clsCashflow.Type.InitialExpenseDraw));
                    if (this.PropertyTaxField.DoubleValue > 0)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  System.DateTime.Now, System.DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), -this.PropertyTaxField.DoubleValue,
                                                                  false, clsCashflow.Type.PropertyTax));
                    if (loanToUpdate.Points() > 0)
                    {
                        if (this.loanToUpdate.PointsCapitalized()) // if points were capitalized, capitalize them on update
                            this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                      DateTime.Now, DateTime.MaxValue,
                                                                      this.loanToUpdate.ID(), -this.UpdatedPointsLabel.DoubleValue,
                                                                      false, clsCashflow.Type.Points));
                        // either way, add points paid at closing (to lender)
                        this.loanToUpdate.AddCashflow(new clsCashflow((DateTime)this.ClosingDatePicker.DateValue,
                                                                  DateTime.Now, DateTime.MaxValue,
                                                                  this.loanToUpdate.ID(), this.UpdatedPointsLabel.DoubleValue,
                                                                  false, clsCashflow.Type.Points));
                    }
                    foreach (clsCashflow cf in newCashflows)
                    {
                        if (cf.TypeID() == clsCashflow.Type.NetDispositionProj)
                        {
                            DateTime newDate = (DateTime)this.ClosingDatePicker.DateValue;
                            clsLoan l = loanToUpdate.LoanAsOf(newDate.AddDays(1), true);
                            this.loanToUpdate.AddCashflow(new clsCashflow(cf.PayDate(), cf.RecordDate(), cf.DeleteDate(), cf.LoanID(),
                                cf.Amount() + loanToUpdate.LoanAsOf(newDate.AddDays(1), true).Balance(newDate.AddDays(1)) - oldAcqCost,
                                false, clsCashflow.Type.NetDispositionProj));
                            //                                cf.Amount() + loanToUpdate.AcquisitionCost(false) - oldAcqCost, false, clsCashflow.Type.NetDispositionProj));
                        }
                        else
                            this.loanToUpdate.AddCashflow(cf);
                    }
                    // Update origination Date and Save
                    this.loanToUpdate.SetNewOriginationDate((DateTime)this.ClosingDatePicker.DateValue);
                    this.loanToUpdate.LenderID = clsEntity.EntityID((String)(NSString)this.LenderComboBox.SelectedValue);
                    this.loanToUpdate.BorrowerID = clsEntity.EntityID((String)(NSString)this.BorrowerComboBox.SelectedValue);
                    if (this.loanToUpdate.Save())
                    {
                        this.SummaryMessageField.StringValue += "\nSave successful.  " + this.loanToUpdate.Property().Address();
                        this.SummaryMessageField.StringValue += "\nOld / New Acquisition Cost = ";
                        this.SummaryMessageField.StringValue += oldAcqCost.ToString("#,##0.00") + " / ";
                        this.SummaryMessageField.StringValue += this.loanToUpdate.AcquisitionCost(false).ToString("#,##0.00");
                        this.UpdateTotalCostLabel();
                    }
                    else
                    {
                        this.SummaryMessageField.StringValue += "\nSave Failed.";
                    }
                }
            }
            else
                this.SummaryMessageField.StringValue = "Update failed.  Loan has already been cancelled.  " + this.loanToUpdate.Property().Address();

        }

        partial void AcqTaxUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void ConcessionUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void DrawUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void HOIUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void PriceUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void ProcessingUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void PropertyTaxUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void RecordingUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void TitlePolicyUpdated(NSTextField sender)
        {
            this.UpdateTotalCostLabel();
        }

        partial void GenerateDocsPressed(NSButton sender)
        {
//            GenerateDocs();
        }
        #endregion

        #region Encapsulation Methods
        private void UpdateTotalCostLabel()
        {
            double total = 0D;
            total = this.HOIField.DoubleValue;
            total += this.PriceField.DoubleValue;
            total += this.AcqTaxField.DoubleValue;
            total += this.RecordingField.DoubleValue;
            total += -Math.Abs(this.ConcessionField.DoubleValue);
            total += this.ProcessingField.DoubleValue;
            total += this.InitialDrawField.DoubleValue;
            total += this.PropertyTaxField.DoubleValue;
            total += this.TitlePolicyField.DoubleValue;
            if (this.loanToUpdate != null)
            {
                if (this.loanToUpdate.PointsCapitalized())
                {
                    total += this.UpdatedPointsLabel.DoubleValue;
                    this.UpdatedPointsLabel.DoubleValue = total / (1D - this.loanToUpdate.Points() * 0.01) * this.loanToUpdate.Points() * 0.01;
                }
                else
                    this.UpdatedPointsLabel.DoubleValue = total * this.loanToUpdate.Points() * 0.01;

            }
            this.TotalCostLabel.DoubleValue = total;
        }

        public void GenerateDocs(string Field0Value, string Field1Value, string Field2Value, string Field3Value, string Field4Value)
        {
            string prefix = "/Volumes/GoogleDrive/Shared Drives/Resilience/Document Templates/";
            string destPrefix = "/Volumes/GoogleDrive/Shared Drives/Resilience/Documents/";
            string mtgPath = prefix;
            string destMtgPath = destPrefix;
            string disclosurePath = prefix + "Disclosure for Confession of Judgment ";
            string destDisclosurePath = destPrefix + "Disclosure for Confession of Judgment ";
            string escrowPath = prefix + "Escrow Instructions ";
            string destEscrowPath = destPrefix + "EIL ";
            string stateAbbrev = this.loanToUpdate.Property().State();
            string lenderAbbrev = (new clsEntity(this.loanToUpdate.LenderID)).PathAbbreviation();
            string borrowerAbrev = (new clsEntity(this.loanToUpdate.BorrowerID)).PathAbbreviation();
            string titleAbrev = (new clsEntity(this.loanToUpdate.TitleCompanyID())).PathAbbreviation();

            // find and copy template(s) (Mortgage, Disclosure, Escrow Instruction Letter)
            destEscrowPath += this.loanToUpdate.Property().Address() + ".docx";
            switch (stateAbbrev)
            {
                case "GA":
                    mtgPath += "Security Deed (MTG) " + lenderAbbrev + " " + borrowerAbrev + " " + stateAbbrev + ".docx";
                    destMtgPath += "Deed " + this.loanToUpdate.Property().Address() + ".docx";
                    break;
                case "MD":
                    mtgPath += "DOT " + lenderAbbrev + " " + borrowerAbrev + " " + stateAbbrev + ".docx";
                    destMtgPath += "DOT " + this.loanToUpdate.Property().Address() + ".docx";
                    break;
                case "IL":
                    mtgPath += "MTG " + lenderAbbrev + " " + borrowerAbrev + " " + stateAbbrev + ".docx";
                    destMtgPath += "Mortgage " + this.loanToUpdate.Property().Address() + ".docx";
                    break;
                case "NJ":
                    mtgPath += "MTG " + lenderAbbrev + " " + borrowerAbrev + " " + stateAbbrev + ".docx";
                    destMtgPath += "Mortgage " + this.loanToUpdate.Property().Address() + ".docx";
                    disclosurePath += lenderAbbrev + " " + borrowerAbrev + ".docx";
                    destDisclosurePath += this.loanToUpdate.Property().Address() + ".docx";
                    System.IO.File.Copy(disclosurePath, destDisclosurePath, true);
                    break;
                case "PA":
                    mtgPath += "MTG " + lenderAbbrev + " " + borrowerAbrev + " " + stateAbbrev + ".docx";
                    destMtgPath += "Mortgage " + this.loanToUpdate.Property().Address() + ".docx";
                    disclosurePath += lenderAbbrev + " " + borrowerAbrev + ".docx";
                    destDisclosurePath += this.loanToUpdate.Property().Address() + ".docx";
                    System.IO.File.Copy(disclosurePath, destDisclosurePath, true);
                    break;
                default:
                    break;
            }
            escrowPath += lenderAbbrev + " " + borrowerAbrev + " " + titleAbrev + ".docx";
            System.IO.File.Copy(mtgPath, destMtgPath, true);
            System.IO.File.Copy(escrowPath, destEscrowPath, true);

            // get replacement values
            // - EIL:  LETTERDATE, ACQUISITIONDATE, FILENUMBER, WIREAMOUNT, EXPIRATIONDATE (= ACQ + 3d), COUNTY
            string LETTERDATE = System.DateTime.Today.Date.ToString("MMMM d, yyyy");
            string ACQUISITIONDATE = this.loanToUpdate.OriginationDate().Date.ToString("MMMM d, yyyy");
            string FILENUMBER = Field0Value; // from custom dialog
            string WIREAMOUNT = this.loanToUpdate.AcquisitionCost(false,true).ToString("#,##0.00");
            string EXPIRATIONDATE = this.loanToUpdate.OriginationDate().Date.AddDays(5).ToString("MMMM d, yyyy");
            string COUNTY = this.loanToUpdate.Property().County();
            // - MTG:  DATEDDATE, DUEDATE, COUNTY, ADDRESS (full address with city, state, zip)
            // - DISC: ADDRESS (full address with city, state, zip)
            string DATEDDATE = ACQUISITIONDATE;
            string DUEDATE = this.loanToUpdate.MaturityDate().ToString("MMMM d, yyyy");
            string ADDRESS = this.loanToUpdate.Property().Address() + ", " + this.loanToUpdate.Property().Town() + ", ";
            ADDRESS += this.loanToUpdate.Property().State();
            //   -     MD:  STREETADDRESS, CITY, MAXAMOUNT, MAXAMOUNTWORDS, PURCHASEAMOUNT, PURCHASEAMOUNTWORDS
            string STREETADDRESS = "";
            string CITY = "";
            string MAXAMOUNT = "";
            string MAXAMOUNTWORDS = "";
            string PURCHASEAMOUNT = "";
            string PURCHASEAMOUNTWORDS="";
            //   -     NJ:  LOT, BLOCK, MAP
            string LOT = "";
            string BLOCK = "";
            string MAP = "";
            //   -     PA, GA:  PARCEL
            string PARCEL = "";  // from custom dialog
            DocX doc;

            switch (stateAbbrev)
            {
                case "GA":
                    PARCEL = Field1Value;  // from custom dialog
                    break;
                case "NJ":
                    LOT = Field1Value;  // from custom dialog
                    BLOCK = Field2Value;  // from custom dialog
                    MAP = Field3Value;  // from custom dialog
                    break;
                case "MD":
                    STREETADDRESS = this.loanToUpdate.Property().Address();
                    CITY = this.loanToUpdate.Property().Town();
                    MAXAMOUNT = Convert.ToDouble(Field1Value).ToString("#,##0.00"); // from custom dialog
                    MAXAMOUNTWORDS = Field2Value;  // from custom dialog
                    PURCHASEAMOUNT = Convert.ToDouble(Field3Value).ToString("#,##0.00");  // from custom dialog
                    PURCHASEAMOUNTWORDS = Field4Value;  // from custom dialog
                    break;
                case "PA":
                    PARCEL = Field1Value;  // from custom dialog
                    break;
                default:
                    break;
            }

            // do find and replaces
            SummaryMessageField.StringValue = "";

            // Disclosure
            if ((stateAbbrev == "NJ") || (stateAbbrev == "PA"))
            {
                doc = DocX.Load(destDisclosurePath);
                doc.ReplaceText("[ADDRESS]", ADDRESS);
                doc.ReplaceText("[DATEDDATE]", DATEDDATE);
                doc.Save();
                SummaryMessageField.StringValue += "Updated " + destDisclosurePath;
            }

            // Escrow
            doc = DocX.Load(destEscrowPath);
            doc.ReplaceText("[LETTERDATE]", LETTERDATE);
            //doc.Headers.Odd.ReplaceText("[LETTERDATE]", LETTERDATE);
            //doc.Headers.Even.ReplaceText("[LETTERDATE]", LETTERDATE);
            //doc.Headers.First.ReplaceText("[LETTERDATE]", LETTERDATE);
            doc.ReplaceText("[ACQUISITIONDATE]", ACQUISITIONDATE);
            doc.ReplaceText("[FILENUMBER]", FILENUMBER);
            doc.ReplaceText("[WIREAMOUNT]", WIREAMOUNT);
            doc.ReplaceText("[EXPIRATIONDATE]", EXPIRATIONDATE);
            doc.ReplaceText("[COUNTY]", COUNTY);
            doc.Save();
            SummaryMessageField.StringValue += "\nUpdated " + destEscrowPath;

            // Mortgage/Deed of Trust
            doc = DocX.Load(destMtgPath);
            doc.ReplaceText("[DATEDDATE]", DATEDDATE);
            doc.ReplaceText("[DUEDATE]", DUEDATE);
            doc.ReplaceText("[COUNTY]", COUNTY);
            doc.ReplaceText("[ADDRESS]", ADDRESS);
            if (stateAbbrev == "MD")
            {
                doc.ReplaceText("[STREET]", STREETADDRESS);
                doc.ReplaceText("[CITY]", CITY);
                doc.ReplaceText("[MAXAMOUNT]", MAXAMOUNT);
                doc.ReplaceText("[MAXAMOUNTWORDS]", MAXAMOUNTWORDS.ToUpper());
                doc.ReplaceText("[PURCHASEAMOUNT]", PURCHASEAMOUNT);
                doc.ReplaceText("[PURCHASEAMOUNTWORDS]", PURCHASEAMOUNTWORDS.ToUpper());

            }
            else if (stateAbbrev == "NJ")
            {
                doc.ReplaceText("[LOT]", LOT);
                doc.ReplaceText("[BLOCK]", BLOCK);
                doc.ReplaceText("[MAP]", MAP);

            }
            else if ((stateAbbrev == "PA") || (stateAbbrev == "GA"))
            {
                doc.ReplaceText("[PARCEL]", PARCEL);
            }
            else if (stateAbbrev == "IL") // nothing to do
            { }
            doc.Save();
            SummaryMessageField.StringValue = "\nUpdated " + destMtgPath;
            SummaryMessageField.StringValue = "\n*** REMEMBER TO UPDATE LEGAL ADDRESS MANUALLY ***";
        }
        #endregion

        #region Overrides
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            List<string> addressList = clsProperty.AddressList();
            this.AddressComboBox.RemoveAll();
            foreach (string address in addressList) { this.AddressComboBox.Add((NSString)address); }

            this.HOILabel.StringValue = "0";
            this.PriceLabel.StringValue = "0";
            this.AcqTaxLabel.StringValue = "0";
            this.RecordingLabel.StringValue = "0";
            this.ConcessionLabel.StringValue = "0";
            this.ProcessingLabel.StringValue = "0";
            this.ClosingDateLabel.StringValue = "--/--/--";
            this.InitialDrawLabel.StringValue = "0";
            this.PropertyTaxLabel.StringValue = "0";
            this.TitlePolicyLabel.StringValue = "0";
            this.LenderLabel.StringValue = "---";
            this.BorrowerLabel.StringValue = "---";
            this.PointsLabel.StringValue = "0";

            this.HOIField.StringValue = this.HOILabel.StringValue;
            this.PriceField.StringValue = this.PriceLabel.StringValue;
            this.AcqTaxField.StringValue = this.AcqTaxLabel.StringValue;
            this.RecordingField.StringValue = this.RecordingLabel.StringValue;
            this.ConcessionField.StringValue = this.ConcessionLabel.StringValue;
            this.ProcessingField.StringValue = this.ProcessingLabel.StringValue;
            this.ClosingDatePicker.DateValue = (NSDate)System.DateTime.Today;
            this.InitialDrawField.StringValue = this.InitialDrawLabel.StringValue;
            this.PropertyTaxField.StringValue = this.PropertyTaxLabel.StringValue;
            this.TitlePolicyField.StringValue = this.TitlePolicyLabel.StringValue;
            this.UpdatedPointsLabel.StringValue = this.PointsLabel.StringValue;

            clsCSVTable tblEntity = new clsCSVTable(clsEntity.strEntityPath);
            for (int i = 0; i < tblEntity.Length(); i++)
            {
                if ((clsEntity.Type)Int32.Parse(tblEntity.Value(i, clsEntity.EntityTypeColumn)) == clsEntity.Type.Lender)
                    this.LenderComboBox.Add((NSString)tblEntity.Value(i, clsEntity.NameColumn));
                else if ((clsEntity.Type)Int32.Parse(tblEntity.Value(i, clsEntity.EntityTypeColumn)) == clsEntity.Type.Borrower)
                    this.BorrowerComboBox.Add((NSString)tblEntity.Value(i, clsEntity.NameColumn));
            }
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

        public override void PrepareForSegue(NSStoryboardSegue segue, NSObject sender)
        {
            switch (segue.Identifier)
            {
                case "ModelSegue": // I wanted this to be ModalSegue, but had a typo...
                    var dialog = segue.DestinationController as CustomDialogController;
                    dialog.Title = "Additional Information for the State of " + this.loanToUpdate.Property().State();
                    dialog.Label0Title = "FILENUMBER";
                    if (this.loanToUpdate.Property().State() == "MD")
                    {
                        dialog.Label1Title = "MAXAMOUNT";
                        dialog.Label2Title = "...WORDS";
                        dialog.Label3Title = "PURCHASEAMOUNT";
                        dialog.Label4Title = "...WORDS";
                    }
                    else if (this.loanToUpdate.Property().State() == "NJ")
                    {
                        dialog.Label1Title = "LOT";
                        dialog.Label2Title = "BLOCK";
                        dialog.Label3Title = "MAP";
                        dialog.Label4Title = "N/A";
                    }
                    else if (this.loanToUpdate.Property().State() == "PA")
                    {
                        dialog.Label1Title = "PARCEL";
                        dialog.Label2Title = "N/A";
                        dialog.Label3Title = "N/A";
                        dialog.Label4Title = "N/A";
                    }
                    else if (this.loanToUpdate.Property().State() == "GA")
                    {
                        dialog.Label1Title = "PARCEL/PIN";
                        dialog.Label2Title = "N/A";
                        dialog.Label3Title = "N/A";
                        dialog.Label4Title = "N/A";
                    }
                    else
                    {
                        dialog.Label1Title = "N/A";
                        dialog.Label2Title = "N/A";
                        dialog.Label3Title = "N/A";
                        dialog.Label4Title = "N/A";
                    }
                    dialog.Presentor = this;
                    break;
            }
        }
        #endregion
    }
}
