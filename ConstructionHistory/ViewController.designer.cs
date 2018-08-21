// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ConstructionHistory
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton addButton { get; set; }

		[Outlet]
		AppKit.NSTextField cashflowID { get; set; }

		[Outlet]
		AppKit.NSTextField constructionAmountNew { get; set; }

		[Outlet]
		AppKit.NSDatePicker constructionDateNew { get; set; }

		[Outlet]
		AppKit.NSButton dateChangeButton { get; set; }

		[Outlet]
		AppKit.NSDatePicker dateChanger { get; set; }

		[Outlet]
		AppKit.NSButton deleteButton { get; set; }

		[Outlet]
		AppKit.NSTextField LoanStatusTextField { get; set; }

		[Outlet]
		AppKit.NSButton markTrueButton { get; set; }

		[Outlet]
		AppKit.NSPopUpButton propertyMenu { get; set; }

		[Outlet]
		AppKit.NSTextField rehabDrawDisplayFalse { get; set; }

		[Outlet]
		AppKit.NSTextField rehabDrawDisplayTrue { get; set; }

		[Action ("addNewConstruction:")]
		partial void addNewConstruction (AppKit.NSButton sender);

		[Action ("dateChangeClicked:")]
		partial void dateChangeClicked (AppKit.NSButton sender);

		[Action ("deleteButtonClicked:")]
		partial void deleteButtonClicked (AppKit.NSButton sender);

		[Action ("markTrueClicked:")]
		partial void markTrueClicked (AppKit.NSButton sender);

		[Action ("propertyMenuChosen:")]
		partial void propertyMenuChosen (AppKit.NSPopUpButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (addButton != null) {
				addButton.Dispose ();
				addButton = null;
			}

			if (cashflowID != null) {
				cashflowID.Dispose ();
				cashflowID = null;
			}

			if (constructionAmountNew != null) {
				constructionAmountNew.Dispose ();
				constructionAmountNew = null;
			}

			if (constructionDateNew != null) {
				constructionDateNew.Dispose ();
				constructionDateNew = null;
			}

			if (dateChangeButton != null) {
				dateChangeButton.Dispose ();
				dateChangeButton = null;
			}

			if (dateChanger != null) {
				dateChanger.Dispose ();
				dateChanger = null;
			}

			if (deleteButton != null) {
				deleteButton.Dispose ();
				deleteButton = null;
			}

			if (markTrueButton != null) {
				markTrueButton.Dispose ();
				markTrueButton = null;
			}

			if (propertyMenu != null) {
				propertyMenu.Dispose ();
				propertyMenu = null;
			}

			if (rehabDrawDisplayFalse != null) {
				rehabDrawDisplayFalse.Dispose ();
				rehabDrawDisplayFalse = null;
			}

			if (rehabDrawDisplayTrue != null) {
				rehabDrawDisplayTrue.Dispose ();
				rehabDrawDisplayTrue = null;
			}

			if (LoanStatusTextField != null) {
				LoanStatusTextField.Dispose ();
				LoanStatusTextField = null;
			}
		}
	}
}
