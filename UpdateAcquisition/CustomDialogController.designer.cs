// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UpdateAcquisition
{
	[Register ("CustomDialogController")]
	partial class CustomDialogController
	{
		[Outlet]
		AppKit.NSTextField Field0 { get; set; }

		[Outlet]
		AppKit.NSTextField Field1 { get; set; }

		[Outlet]
		AppKit.NSTextField Field2 { get; set; }

		[Outlet]
		AppKit.NSTextField Field3 { get; set; }

		[Outlet]
		AppKit.NSTextField Field4 { get; set; }

		[Outlet]
		AppKit.NSTextField Label0 { get; set; }

		[Outlet]
		AppKit.NSTextField Label1 { get; set; }

		[Outlet]
		AppKit.NSTextField Label2 { get; set; }

		[Outlet]
		AppKit.NSTextField Label3 { get; set; }

		[Outlet]
		AppKit.NSTextField Label4 { get; set; }

		[Action ("Field0Changed:")]
		partial void Field0Changed (AppKit.NSTextField sender);

		[Action ("Field1Changed:")]
		partial void Field1Changed (AppKit.NSTextField sender);

		[Action ("Field2Changed:")]
		partial void Field2Changed (AppKit.NSTextField sender);

		[Action ("Field3Changed:")]
		partial void Field3Changed (AppKit.NSTextField sender);

		[Action ("Field4Changed:")]
		partial void Field4Changed (AppKit.NSTextField sender);

		[Action ("OKPressed:")]
		partial void OKPressed (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Field0 != null) {
				Field0.Dispose ();
				Field0 = null;
			}

			if (Field1 != null) {
				Field1.Dispose ();
				Field1 = null;
			}

			if (Field2 != null) {
				Field2.Dispose ();
				Field2 = null;
			}

			if (Field3 != null) {
				Field3.Dispose ();
				Field3 = null;
			}

			if (Field4 != null) {
				Field4.Dispose ();
				Field4 = null;
			}

			if (Label0 != null) {
				Label0.Dispose ();
				Label0 = null;
			}

			if (Label1 != null) {
				Label1.Dispose ();
				Label1 = null;
			}

			if (Label2 != null) {
				Label2.Dispose ();
				Label2 = null;
			}

			if (Label3 != null) {
				Label3.Dispose ();
				Label3 = null;
			}

			if (Label4 != null) {
				Label4.Dispose ();
				Label4 = null;
			}
		}
	}
}
