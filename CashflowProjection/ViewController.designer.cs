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
		AppKit.NSScrollView CashflowTable { get; set; }

		[Outlet]
		AppKit.NSTableView CashflowTableView { get; set; }

		[Outlet]
		AppKit.NSDatePicker EndDatePicker { get; set; }

		[Outlet]
		AppKit.NSButton FullDetailCheckBox { get; set; }

		[Outlet]
		AppKit.NSButton OutflowsOnlyButton { get; set; }

		[Outlet]
		AppKit.NSButton ScenarioButton { get; set; }

		[Outlet]
		AppKit.NSButton ScheduledOnlyButton { get; set; }

		[Outlet]
		AppKit.NSDatePicker StartDatePicker { get; set; }

		[Action ("EndDatePicked:")]
		partial void EndDatePicked (AppKit.NSDatePicker sender);

		[Action ("FullDetailCheckBoxPressed:")]
		partial void FullDetailCheckBoxPressed (AppKit.NSButton sender);

		[Action ("NAVExportPressed:")]
		partial void NAVExportPressed (AppKit.NSButton sender);

		[Action ("OutflowsOnlyPressed:")]
		partial void OutflowsOnlyPressed (AppKit.NSButton sender);

		[Action ("SaveCSVPressed:")]
		partial void SaveCSVPressed (AppKit.NSButton sender);

		[Action ("ScenarioButtonPressed:")]
		partial void ScenarioButtonPressed (AppKit.NSButton sender);

		[Action ("ScheduledOnlyPressed:")]
		partial void ScheduledOnlyPressed (AppKit.NSButton sender);

		[Action ("StartDatePicked:")]
		partial void StartDatePicked (AppKit.NSDatePicker sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CashflowTable != null) {
				CashflowTable.Dispose ();
				CashflowTable = null;
			}

			if (CashflowTableView != null) {
				CashflowTableView.Dispose ();
				CashflowTableView = null;
			}

			if (EndDatePicker != null) {
				EndDatePicker.Dispose ();
				EndDatePicker = null;
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

			if (StartDatePicker != null) {
				StartDatePicker.Dispose ();
				StartDatePicker = null;
			}

			if (FullDetailCheckBox != null) {
				FullDetailCheckBox.Dispose ();
				FullDetailCheckBox = null;
			}
		}
	}
}
