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
        AppKit.NSTextField AddressBox { get; set; }

        [Outlet]
        AppKit.NSTextField BPOBox { get; set; }

        [Outlet]
        AppKit.NSPopUpButton CoBorrowerPopUp { get; set; }

        [Outlet]
        AppKit.NSTextField CountyBox { get; set; }

        [Outlet]
        AppKit.NSPopUpButton LenderPopUp { get; set; }

        [Outlet]
        AppKit.NSTextField MonthsToCompletionBox { get; set; }

        [Outlet]
        AppKit.NSTextField PnLBox { get; set; }

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

        [Action ("AddButtonPushed:")]
        partial void AddButtonPushed (AppKit.NSButton sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (AddressBox != null) {
                AddressBox.Dispose ();
                AddressBox = null;
            }

            if (TownBox != null) {
                TownBox.Dispose ();
                TownBox = null;
            }

            if (CountyBox != null) {
                CountyBox.Dispose ();
                CountyBox = null;
            }

            if (StateBox != null) {
                StateBox.Dispose ();
                StateBox = null;
            }

            if (PurchaseDatePicker != null) {
                PurchaseDatePicker.Dispose ();
                PurchaseDatePicker = null;
            }

            if (TitleHolderPopUp != null) {
                TitleHolderPopUp.Dispose ();
                TitleHolderPopUp = null;
            }

            if (CoBorrowerPopUp != null) {
                CoBorrowerPopUp.Dispose ();
                CoBorrowerPopUp = null;
            }

            if (PurchasePriceBox != null) {
                PurchasePriceBox.Dispose ();
                PurchasePriceBox = null;
            }

            if (RehabCostBox != null) {
                RehabCostBox.Dispose ();
                RehabCostBox = null;
            }

            if (BPOBox != null) {
                BPOBox.Dispose ();
                BPOBox = null;
            }

            if (PnLBox != null) {
                PnLBox.Dispose ();
                PnLBox = null;
            }

            if (MonthsToCompletionBox != null) {
                MonthsToCompletionBox.Dispose ();
                MonthsToCompletionBox = null;
            }

            if (UpdateMessage != null) {
                UpdateMessage.Dispose ();
                UpdateMessage = null;
            }

            if (LenderPopUp != null) {
                LenderPopUp.Dispose ();
                LenderPopUp = null;
            }

            if (TitlePopUp != null) {
                TitlePopUp.Dispose ();
                TitlePopUp = null;
            }
        }
    }
}
