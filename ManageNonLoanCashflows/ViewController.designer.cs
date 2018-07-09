// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ManageNonLoanCashflows
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        AppKit.NSTextField ActualTextField { get; set; }

        [Outlet]
        AppKit.NSButton AddButton { get; set; }

        [Outlet]
        AppKit.NSTextField AmountTextField { get; set; }

        [Outlet]
        AppKit.NSTextField CashflowIDTextField { get; set; }

        [Outlet]
        AppKit.NSTableView CashflowsTableView { get; set; }

        [Outlet]
        AppKit.NSTextField CommentTextField { get; set; }

        [Outlet]
        AppKit.NSDatePicker DatePicker { get; set; }

        [Outlet]
        AppKit.NSScrollView ExistingCashflowsScrollView { get; set; }

        [Outlet]
        AppKit.NSButton ExpireButton { get; set; }

        [Outlet]
        AppKit.NSDatePicker RecordDateOverridePicker { get; set; }

        [Outlet]
        AppKit.NSButton ShowExpiredButton { get; set; }

        [Outlet]
        AppKit.NSPopUpButton TypePopUpButton { get; set; }

        [Action ("ActualCashflowPressed:")]
        partial void ActualCashflowPressed (AppKit.NSButton sender);

        [Action ("AddButtonPressed:")]
        partial void AddButtonPressed (AppKit.NSButton sender);

        [Action ("AmountEntered:")]
        partial void AmountEntered (AppKit.NSTextField sender);

        [Action ("CashflowIDEntered:")]
        partial void CashflowIDEntered (AppKit.NSTextField sender);

        [Action ("DateChosen:")]
        partial void DateChosen (AppKit.NSDatePicker sender);

        [Action ("ExpireButtonPressed:")]
        partial void ExpireButtonPressed (AppKit.NSButton sender);

        [Action ("RecordDateOverriden:")]
        partial void RecordDateOverriden (AppKit.NSDatePicker sender);

        [Action ("ShowExpiredCheckBox:")]
        partial void ShowExpiredCheckBox (AppKit.NSButton sender);

        [Action ("ShowExpiredCheckBoxPressed:")]
        partial void ShowExpiredCheckBoxPressed (AppKit.NSButton sender);

        [Action ("TypeChosen:")]
        partial void TypeChosen (AppKit.NSPopUpButton sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (AddButton != null) {
                AddButton.Dispose ();
                AddButton = null;
            }

            if (AmountTextField != null) {
                AmountTextField.Dispose ();
                AmountTextField = null;
            }

            if (CashflowIDTextField != null) {
                CashflowIDTextField.Dispose ();
                CashflowIDTextField = null;
            }

            if (CashflowsTableView != null) {
                CashflowsTableView.Dispose ();
                CashflowsTableView = null;
            }

            if (CommentTextField != null) {
                CommentTextField.Dispose ();
                CommentTextField = null;
            }

            if (DatePicker != null) {
                DatePicker.Dispose ();
                DatePicker = null;
            }

            if (ExistingCashflowsScrollView != null) {
                ExistingCashflowsScrollView.Dispose ();
                ExistingCashflowsScrollView = null;
            }

            if (ExpireButton != null) {
                ExpireButton.Dispose ();
                ExpireButton = null;
            }

            if (RecordDateOverridePicker != null) {
                RecordDateOverridePicker.Dispose ();
                RecordDateOverridePicker = null;
            }

            if (ShowExpiredButton != null) {
                ShowExpiredButton.Dispose ();
                ShowExpiredButton = null;
            }

            if (TypePopUpButton != null) {
                TypePopUpButton.Dispose ();
                TypePopUpButton = null;
            }

            if (ActualTextField != null) {
                ActualTextField.Dispose ();
                ActualTextField = null;
            }
        }
    }
}
