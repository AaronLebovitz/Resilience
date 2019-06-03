// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AddNewProperty
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton AcquisitionOnlyCheckBox { get; set; }

		[Outlet]
		AppKit.NSTextField AddressBox { get; set; }

		[Outlet]
		AppKit.NSTextField BPOBox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton CoBorrowerPopUp { get; set; }

		[Outlet]
		AppKit.NSTextField CountyBox { get; set; }

		[Outlet]
		AppKit.NSTextField DefaultRateBox { get; set; }

		[Outlet]
		AppKit.NSButton FullAcquisitionCostCheckBox { get; set; }

		[Outlet]
		AppKit.NSTextField InitialLoanPercentBox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton LenderPopUp { get; set; }

		[Outlet]
		AppKit.NSTextField LoanRateBox { get; set; }

		[Outlet]
		AppKit.NSTextField MonthsToCompletionBox { get; set; }

		[Outlet]
		AppKit.NSTextField PnLBox { get; set; }

		[Outlet]
		AppKit.NSTextField PointsBox { get; set; }

		[Outlet]
		AppKit.NSButton ProfitSplit { get; set; }

		[Outlet]
		AppKit.NSTextField ProfitSplitBox { get; set; }

		[Outlet]
		AppKit.NSDatePicker PurchaseDatePicker { get; set; }

		[Outlet]
		AppKit.NSTextField PurchasePriceBox { get; set; }

		[Outlet]
		AppKit.NSTextField RehabCostBox { get; set; }

		[Outlet]
		AppKit.NSTextField StateBox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton TitleHolderPopUp { get; set; }

		[Outlet]
		AppKit.NSPopUpButton TitlePopUp { get; set; }

		[Outlet]
		AppKit.NSTextField TownBox { get; set; }

		[Outlet]
		AppKit.NSTextField UpdateMessage { get; set; }

		[Action ("AcquisitionOnlyToggled:")]
		partial void AcquisitionOnlyToggled (AppKit.NSButton sender);

		[Action ("AddButtonPushed:")]
		partial void AddButtonPushed (AppKit.NSButton sender);

		[Action ("FullAcquisitionCostToggled:")]
		partial void FullAcquisitionCostToggled (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddressBox != null) {
				AddressBox.Dispose ();
				AddressBox = null;
			}

			if (BPOBox != null) {
				BPOBox.Dispose ();
				BPOBox = null;
			}

			if (CoBorrowerPopUp != null) {
				CoBorrowerPopUp.Dispose ();
				CoBorrowerPopUp = null;
			}

			if (CountyBox != null) {
				CountyBox.Dispose ();
				CountyBox = null;
			}

			if (DefaultRateBox != null) {
				DefaultRateBox.Dispose ();
				DefaultRateBox = null;
			}

			if (LenderPopUp != null) {
				LenderPopUp.Dispose ();
				LenderPopUp = null;
			}

			if (LoanRateBox != null) {
				LoanRateBox.Dispose ();
				LoanRateBox = null;
			}

			if (MonthsToCompletionBox != null) {
				MonthsToCompletionBox.Dispose ();
				MonthsToCompletionBox = null;
			}

			if (PnLBox != null) {
				PnLBox.Dispose ();
				PnLBox = null;
			}

			if (PointsBox != null) {
				PointsBox.Dispose ();
				PointsBox = null;
			}

			if (PurchaseDatePicker != null) {
				PurchaseDatePicker.Dispose ();
				PurchaseDatePicker = null;
			}

			if (PurchasePriceBox != null) {
				PurchasePriceBox.Dispose ();
				PurchasePriceBox = null;
			}

			if (RehabCostBox != null) {
				RehabCostBox.Dispose ();
				RehabCostBox = null;
			}

			if (StateBox != null) {
				StateBox.Dispose ();
				StateBox = null;
			}

			if (TitleHolderPopUp != null) {
				TitleHolderPopUp.Dispose ();
				TitleHolderPopUp = null;
			}

			if (TitlePopUp != null) {
				TitlePopUp.Dispose ();
				TitlePopUp = null;
			}

			if (TownBox != null) {
				TownBox.Dispose ();
				TownBox = null;
			}

			if (UpdateMessage != null) {
				UpdateMessage.Dispose ();
				UpdateMessage = null;
			}

			if (ProfitSplit != null) {
				ProfitSplit.Dispose ();
				ProfitSplit = null;
			}

			if (ProfitSplitBox != null) {
				ProfitSplitBox.Dispose ();
				ProfitSplitBox = null;
			}

			if (AcquisitionOnlyCheckBox != null) {
				AcquisitionOnlyCheckBox.Dispose ();
				AcquisitionOnlyCheckBox = null;
			}

			if (InitialLoanPercentBox != null) {
				InitialLoanPercentBox.Dispose ();
				InitialLoanPercentBox = null;
			}

			if (FullAcquisitionCostCheckBox != null) {
				FullAcquisitionCostCheckBox.Dispose ();
				FullAcquisitionCostCheckBox = null;
			}
		}
	}
}
