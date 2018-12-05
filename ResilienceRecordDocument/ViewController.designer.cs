// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ResilienceRecordDocument
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSDatePicker ActionDateChooser { get; set; }

		[Outlet]
		AppKit.NSTextField BookInstrumentTextField { get; set; }

		[Outlet]
		AppKit.NSTextField ChosenDocumentLabel { get; set; }

		[Outlet]
		AppKit.NSComboBox DocumentChooser { get; set; }

		[Outlet]
		AppKit.NSTextField PageTextField { get; set; }

		[Outlet]
		AppKit.NSComboBox PropertyChooser { get; set; }

		[Outlet]
		AppKit.NSComboBox ReceiverChooser { get; set; }

		[Outlet]
		AppKit.NSDatePicker RecordDateChooser { get; set; }

		[Outlet]
		AppKit.NSTextField SaveMessage { get; set; }

		[Outlet]
		AppKit.NSComboBox SenderChooser { get; set; }

		[Outlet]
		AppKit.NSTextField SenderReceiverLabel { get; set; }

		[Outlet]
		AppKit.NSComboBox StatusChooser { get; set; }

		[Outlet]
		AppKit.NSComboBox TransmitChooser { get; set; }

		[Action ("ActionDateChosen:")]
		partial void ActionDateChosen (AppKit.NSDatePicker sender);

		[Action ("DocumentChosen:")]
		partial void DocumentChosen (AppKit.NSComboBox sender);

		[Action ("PropertyChosen:")]
		partial void PropertyChosen (AppKit.NSComboBox sender);

		[Action ("ReceiverChosen:")]
		partial void ReceiverChosen (AppKit.NSComboBox sender);

		[Action ("RecordButtonPushed:")]
		partial void RecordButtonPushed (AppKit.NSButton sender);

		[Action ("RecordDateChosen:")]
		partial void RecordDateChosen (AppKit.NSDatePicker sender);

		[Action ("SenderChosen:")]
		partial void SenderChosen (AppKit.NSComboBox sender);

		[Action ("StatusChosen:")]
		partial void StatusChosen (AppKit.NSComboBox sender);

		[Action ("TransmitChosen:")]
		partial void TransmitChosen (AppKit.NSComboBox sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ActionDateChooser != null) {
				ActionDateChooser.Dispose ();
				ActionDateChooser = null;
			}

			if (ChosenDocumentLabel != null) {
				ChosenDocumentLabel.Dispose ();
				ChosenDocumentLabel = null;
			}

			if (DocumentChooser != null) {
				DocumentChooser.Dispose ();
				DocumentChooser = null;
			}

			if (PropertyChooser != null) {
				PropertyChooser.Dispose ();
				PropertyChooser = null;
			}

			if (ReceiverChooser != null) {
				ReceiverChooser.Dispose ();
				ReceiverChooser = null;
			}

			if (RecordDateChooser != null) {
				RecordDateChooser.Dispose ();
				RecordDateChooser = null;
			}

			if (SaveMessage != null) {
				SaveMessage.Dispose ();
				SaveMessage = null;
			}

			if (SenderChooser != null) {
				SenderChooser.Dispose ();
				SenderChooser = null;
			}

			if (SenderReceiverLabel != null) {
				SenderReceiverLabel.Dispose ();
				SenderReceiverLabel = null;
			}

			if (StatusChooser != null) {
				StatusChooser.Dispose ();
				StatusChooser = null;
			}

			if (TransmitChooser != null) {
				TransmitChooser.Dispose ();
				TransmitChooser = null;
			}

			if (BookInstrumentTextField != null) {
				BookInstrumentTextField.Dispose ();
				BookInstrumentTextField = null;
			}

			if (PageTextField != null) {
				PageTextField.Dispose ();
				PageTextField = null;
			}
		}
	}
}
