// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DocumentHistory
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSScrollView DocRecTable { get; set; }

		[Outlet]
		AppKit.NSTableView DocRecTableView { get; set; }

		[Outlet]
		AppKit.NSComboBox PropertyChooser { get; set; }

		[Outlet]
		AppKit.NSComboBox ReceiverFilter { get; set; }

		[Outlet]
		AppKit.NSComboBox SenderFilter { get; set; }

		[Outlet]
		AppKit.NSComboBox StatusFilter { get; set; }

		[Outlet]
		AppKit.NSComboBox TypeFilter { get; set; }

		[Action ("PropertyChosen:")]
		partial void PropertyChosen (AppKit.NSComboBox sender);

		[Action ("ReceiverChosen:")]
		partial void ReceiverChosen (AppKit.NSComboBox sender);

		[Action ("SenderChosen:")]
		partial void SenderChosen (AppKit.NSComboBox sender);

		[Action ("StatusChosen:")]
		partial void StatusChosen (AppKit.NSComboBox sender);

		[Action ("TypeChhosen:")]
		partial void TypeChhosen (AppKit.NSComboBox sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DocRecTable != null) {
				DocRecTable.Dispose ();
				DocRecTable = null;
			}

			if (PropertyChooser != null) {
				PropertyChooser.Dispose ();
				PropertyChooser = null;
			}

			if (DocRecTableView != null) {
				DocRecTableView.Dispose ();
				DocRecTableView = null;
			}

			if (TypeFilter != null) {
				TypeFilter.Dispose ();
				TypeFilter = null;
			}

			if (StatusFilter != null) {
				StatusFilter.Dispose ();
				StatusFilter = null;
			}

			if (SenderFilter != null) {
				SenderFilter.Dispose ();
				SenderFilter = null;
			}

			if (ReceiverFilter != null) {
				ReceiverFilter.Dispose ();
				ReceiverFilter = null;
			}
		}
	}
}
