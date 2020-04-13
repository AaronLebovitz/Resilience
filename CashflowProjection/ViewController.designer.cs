// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CashflowProjection
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        AppKit.NSComboBox ActualFilterComboxBox { get; set; }

        [Outlet]
        AppKit.NSComboBox AddressFilterComboBox { get; set; }

        [Outlet]
        AppKit.NSPopUpButton BorrowerPopUp { get; set; }

        [Outlet]
        AppKit.NSScrollView CashflowTable { get; set; }

        [Outlet]
        AppKit.NSTableView CashflowTableView { get; set; }

        [Outlet]
        AppKit.NSDatePicker DateFilterDatePicker { get; set; }

        [Outlet]
        AppKit.NSDatePicker EndDatePicker { get; set; }

        [Outlet]
        AppKit.NSButton FullDetailCheckBox { get; set; }

        [Outlet]
        AppKit.NSPopUpButton LenderPopUp { get; set; }

        [Outlet]
        AppKit.NSButton OutflowsOnlyButton { get; set; }

        [Outlet]
        AppKit.NSButton ScenarioButton { get; set; }

        [Outlet]
        AppKit.NSButton ScheduledOnlyButton { get; set; }

        [Outlet]
        AppKit.NSButton ShowExpiredCheckBox { get; set; }

        [Outlet]
        AppKit.NSDatePicker StartDatePicker { get; set; }

        [Outlet]
        AppKit.NSComboBox TypeFilterComboBox { get; set; }

        [Action ("ActualFilterSelected:")]
        partial void ActualFilterSelected (AppKit.NSComboBox sender);

        [Action ("AddressFilterSelected:")]
        partial void AddressFilterSelected (AppKit.NSComboBox sender);

        [Action ("BorrowerSelected:")]
        partial void BorrowerSelected (AppKit.NSPopUpButton sender);

        [Action ("DateFilterPicked:")]
        partial void DateFilterPicked (AppKit.NSDatePicker sender);

        [Action ("EndDatePicked:")]
        partial void EndDatePicked (AppKit.NSDatePicker sender);

        [Action ("FullDetailCheckBoxPressed:")]
        partial void FullDetailCheckBoxPressed (AppKit.NSButton sender);

        [Action ("LenderSelected:")]
        partial void LenderSelected (AppKit.NSPopUpButton sender);

        [Action ("NAVExportPressed:")]
        partial void NAVExportPressed (AppKit.NSButton sender);

        [Action ("OutflowsOnlyPressed:")]
        partial void OutflowsOnlyPressed (AppKit.NSButton sender);

        [Action ("PastDueButtonPressed:")]
        partial void PastDueButtonPressed (AppKit.NSButton sender);

        [Action ("ReloadButtonPushed:")]
        partial void ReloadButtonPushed (AppKit.NSButton sender);

        [Action ("SaveCSVPressed:")]
        partial void SaveCSVPressed (AppKit.NSButton sender);

        [Action ("ScenarioButtonPressed:")]
        partial void ScenarioButtonPressed (AppKit.NSButton sender);

        [Action ("ScheduledOnlyPressed:")]
        partial void ScheduledOnlyPressed (AppKit.NSButton sender);

        [Action ("ShowExpiredCheckBoxToggled:")]
        partial void ShowExpiredCheckBoxToggled (AppKit.NSButton sender);

        [Action ("StartDatePicked:")]
        partial void StartDatePicked (AppKit.NSDatePicker sender);

        [Action ("TypeFilterSelected:")]
        partial void TypeFilterSelected (AppKit.NSComboBox sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (ActualFilterComboxBox != null) {
                ActualFilterComboxBox.Dispose ();
                ActualFilterComboxBox = null;
            }

            if (AddressFilterComboBox != null) {
                AddressFilterComboBox.Dispose ();
                AddressFilterComboBox = null;
            }

            if (CashflowTable != null) {
                CashflowTable.Dispose ();
                CashflowTable = null;
            }

            if (CashflowTableView != null) {
                CashflowTableView.Dispose ();
                CashflowTableView = null;
            }

            if (DateFilterDatePicker != null) {
                DateFilterDatePicker.Dispose ();
                DateFilterDatePicker = null;
            }

            if (EndDatePicker != null) {
                EndDatePicker.Dispose ();
                EndDatePicker = null;
            }

            if (FullDetailCheckBox != null) {
                FullDetailCheckBox.Dispose ();
                FullDetailCheckBox = null;
            }

            if (LenderPopUp != null) {
                LenderPopUp.Dispose ();
                LenderPopUp = null;
            }

            if (BorrowerPopUp != null) {
                BorrowerPopUp.Dispose ();
                BorrowerPopUp = null;
            }

            if (OutflowsOnlyButton != null) {
                OutflowsOnlyButton.Dispose ();
                OutflowsOnlyButton = null;
            }

            if (ScenarioButton != null) {
                ScenarioButton.Dispose ();
                ScenarioButton = null;
            }

            if (ScheduledOnlyButton != null) {
                ScheduledOnlyButton.Dispose ();
                ScheduledOnlyButton = null;
            }

            if (ShowExpiredCheckBox != null) {
                ShowExpiredCheckBox.Dispose ();
                ShowExpiredCheckBox = null;
            }

            if (StartDatePicker != null) {
                StartDatePicker.Dispose ();
                StartDatePicker = null;
            }

            if (TypeFilterComboBox != null) {
                TypeFilterComboBox.Dispose ();
                TypeFilterComboBox = null;
            }
        }
    }
}
