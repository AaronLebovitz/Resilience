// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DocumentRecordLookup
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        AppKit.NSTableView docHistoryTableView { get; set; }

        [Outlet]
        AppKit.NSTextField documentInfoDisplay { get; set; }

        [Outlet]
        AppKit.NSScrollView documentRecordDisplay { get; set; }

        [Outlet]
        AppKit.NSTextField documentRecordHistory { get; set; }

        [Outlet]
        AppKit.NSPopUpButton propertyMenu { get; set; }

        [Outlet]
        AppKit.NSTableColumn receiverColumn { get; set; }

        [Outlet]
        AppKit.NSTableColumn senderColumn { get; set; }

        [Action ("propertyMenuChoose:")]
        partial void propertyMenuChoose (Foundation.NSObject sender);

        [Action ("propertyMenuChosen:")]
        partial void propertyMenuChosen (AppKit.NSPopUpButton sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (documentInfoDisplay != null) {
                documentInfoDisplay.Dispose ();
                documentInfoDisplay = null;
            }

            if (documentRecordDisplay != null) {
                documentRecordDisplay.Dispose ();
                documentRecordDisplay = null;
            }

            if (documentRecordHistory != null) {
                documentRecordHistory.Dispose ();
                documentRecordHistory = null;
            }

            if (propertyMenu != null) {
                propertyMenu.Dispose ();
                propertyMenu = null;
            }

            if (docHistoryTableView != null) {
                docHistoryTableView.Dispose ();
                docHistoryTableView = null;
            }

            if (senderColumn != null) {
                senderColumn.Dispose ();
                senderColumn = null;
            }

            if (receiverColumn != null) {
                receiverColumn.Dispose ();
                receiverColumn = null;
            }
        }
    }
}
