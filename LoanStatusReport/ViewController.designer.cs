// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LoanStatusReport
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        AppKit.NSDatePicker ReportDatePicker { get; set; }

        [Outlet]
        AppKit.NSDatePicker ReportDatePicker2 { get; set; }

        [Outlet]
        AppKit.NSTextField ReportSummaryTextLabel { get; set; }

        [Outlet]
        AppKit.NSTextFieldCell ReportSummaryTextLabelCell { get; set; }

        [Outlet]
        AppKit.NSButton RunReportButton { get; set; }

        [Outlet]
        AppKit.NSButtonCell RunReportButtonCell { get; set; }

        [Outlet]
        AppKit.NSTextField UpdateLabel { get; set; }

        [Action ("DateChosen:")]
        partial void DateChosen (AppKit.NSDatePicker sender);

        [Action ("RunPeriodReportButtonPushed:")]
        partial void RunPeriodReportButtonPushed (AppKit.NSButton sender);

        [Action ("RunReportButtonPushed:")]
        partial void RunReportButtonPushed (AppKit.NSButton sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (ReportDatePicker != null) {
                ReportDatePicker.Dispose ();
                ReportDatePicker = null;
            }

            if (RunReportButton != null) {
                RunReportButton.Dispose ();
                RunReportButton = null;
            }

            if (RunReportButtonCell != null) {
                RunReportButtonCell.Dispose ();
                RunReportButtonCell = null;
            }

            if (UpdateLabel != null) {
                UpdateLabel.Dispose ();
                UpdateLabel = null;
            }

            if (ReportDatePicker2 != null) {
                ReportDatePicker2.Dispose ();
                ReportDatePicker2 = null;
            }

            if (ReportSummaryTextLabel != null) {
                ReportSummaryTextLabel.Dispose ();
                ReportSummaryTextLabel = null;
            }

            if (ReportSummaryTextLabelCell != null) {
                ReportSummaryTextLabelCell.Dispose ();
                ReportSummaryTextLabelCell = null;
            }
        }
    }
}
