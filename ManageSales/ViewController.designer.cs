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
        AppKit.NSComboBox AddressComboBox { get; set; }

        [Outlet]
        AppKit.NSPopUpButton ChooseActionPopUp { get; set; }

        [Outlet]
        AppKit.NSButton ConractReceivedCheckBox { get; set; }

        [Outlet]
        AppKit.NSTextField ExpectedAdditionalInterestTextField { get; set; }

        [Outlet]
        AppKit.NSTextField ExpectedSalePriceTextField { get; set; }

        [Outlet]
        AppKit.NSDatePicker RecordDatePicker { get; set; }

        [Outlet]
        AppKit.NSTextField RepaymentAmountTextField { get; set; }

        [Outlet]
        AppKit.NSDatePicker SaleDatePicker { get; set; }

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
        
        void ReleaseDesignerOutlets ()
        {
            if (AddressComboBox != null) {
                AddressComboBox.Dispose ();
                AddressComboBox = null;
            }

            if (ChooseActionPopUp != null) {
                ChooseActionPopUp.Dispose ();
                ChooseActionPopUp = null;
            }

            if (ExpectedAdditionalInterestTextField != null) {
                ExpectedAdditionalInterestTextField.Dispose ();
                ExpectedAdditionalInterestTextField = null;
            }

            if (ExpectedSalePriceTextField != null) {
                ExpectedSalePriceTextField.Dispose ();
                ExpectedSalePriceTextField = null;
            }

            if (RepaymentAmountTextField != null) {
                RepaymentAmountTextField.Dispose ();
                RepaymentAmountTextField = null;
            }

            if (SaleDatePicker != null) {
                SaleDatePicker.Dispose ();
                SaleDatePicker = null;
            }

            if (StatusMessageTextField != null) {
                StatusMessageTextField.Dispose ();
                StatusMessageTextField = null;
            }

            if (StatusTextField != null) {
                StatusTextField.Dispose ();
                StatusTextField = null;
            }

            if (RecordDatePicker != null) {
                RecordDatePicker.Dispose ();
                RecordDatePicker = null;
            }

            if (ConractReceivedCheckBox != null) {
                ConractReceivedCheckBox.Dispose ();
                ConractReceivedCheckBox = null;
            }
        }
    }
}
