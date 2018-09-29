// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ManageSales
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField AdditionalInterestLabel { get; set; }

		[Outlet]
		AppKit.NSComboBox AddressComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField CashflowDateLabel { get; set; }

		[Outlet]
		AppKit.NSPopUpButton ChooseActionPopUp { get; set; }

		[Outlet]
		AppKit.NSButton ConractReceivedCheckBox { get; set; }

		[Outlet]
		AppKit.NSTextField ExpectedAdditionalInterestTextField { get; set; }

		[Outlet]
		AppKit.NSTextField ExpectedSalePriceTextField { get; set; }

		[Outlet]
		AppKit.NSComboBox LenderComboBox { get; set; }

		[Outlet]
		AppKit.NSDatePicker RecordDatePicker { get; set; }

		[Outlet]
		AppKit.NSTextField RepaymentAmountLabel { get; set; }

		[Outlet]
		AppKit.NSTextField RepaymentAmountTextField { get; set; }

		[Outlet]
		AppKit.NSDatePicker SaleDatePicker { get; set; }

		[Outlet]
		AppKit.NSTextField SalePriceLabel { get; set; }

		[Outlet]
		AppKit.NSComboBox StatusComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField StatusMessageTextField { get; set; }

		[Outlet]
		AppKit.NSTextField StatusTextField { get; set; }

		[Action ("AddressChosen:")]
		partial void AddressChosen (AppKit.NSComboBox sender);

		[Action ("ContractReceivedToggled:")]
		partial void ContractReceivedToggled (AppKit.NSButton sender);

		[Action ("GoButtonPressed:")]
		partial void GoButtonPressed (AppKit.NSButton sender);

		[Action ("LenderChosen:")]
		partial void LenderChosen (AppKit.NSComboBox sender);

		[Action ("StatusChosen:")]
		partial void StatusChosen (AppKit.NSComboBox sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AdditionalInterestLabel != null) {
				AdditionalInterestLabel.Dispose ();
				AdditionalInterestLabel = null;
			}

			if (AddressComboBox != null) {
				AddressComboBox.Dispose ();
				AddressComboBox = null;
			}

			if (CashflowDateLabel != null) {
				CashflowDateLabel.Dispose ();
				CashflowDateLabel = null;
			}

			if (ChooseActionPopUp != null) {
				ChooseActionPopUp.Dispose ();
				ChooseActionPopUp = null;
			}

			if (ConractReceivedCheckBox != null) {
				ConractReceivedCheckBox.Dispose ();
				ConractReceivedCheckBox = null;
			}

			if (ExpectedAdditionalInterestTextField != null) {
				ExpectedAdditionalInterestTextField.Dispose ();
				ExpectedAdditionalInterestTextField = null;
			}

			if (ExpectedSalePriceTextField != null) {
				ExpectedSalePriceTextField.Dispose ();
				ExpectedSalePriceTextField = null;
			}

			if (RecordDatePicker != null) {
				RecordDatePicker.Dispose ();
				RecordDatePicker = null;
			}

			if (RepaymentAmountLabel != null) {
				RepaymentAmountLabel.Dispose ();
				RepaymentAmountLabel = null;
			}

			if (RepaymentAmountTextField != null) {
				RepaymentAmountTextField.Dispose ();
				RepaymentAmountTextField = null;
			}

			if (SaleDatePicker != null) {
				SaleDatePicker.Dispose ();
				SaleDatePicker = null;
			}

			if (SalePriceLabel != null) {
				SalePriceLabel.Dispose ();
				SalePriceLabel = null;
			}

			if (StatusMessageTextField != null) {
				StatusMessageTextField.Dispose ();
				StatusMessageTextField = null;
			}

			if (StatusTextField != null) {
				StatusTextField.Dispose ();
				StatusTextField = null;
			}

			if (StatusComboBox != null) {
				StatusComboBox.Dispose ();
				StatusComboBox = null;
			}

			if (LenderComboBox != null) {
				LenderComboBox.Dispose ();
				LenderComboBox = null;
			}
		}
	}
}
