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
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField AcqTaxField { get; set; }

		[Outlet]
		AppKit.NSTextField AcqTaxLabel { get; set; }

		[Outlet]
		AppKit.NSComboBox AddressComboBox { get; set; }

		[Outlet]
		AppKit.NSComboBox BorrowerComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField BorrowerLabel { get; set; }

		[Outlet]
		AppKit.NSTextField ClosingDateLabel { get; set; }

		[Outlet]
		AppKit.NSDatePicker ClosingDatePicker { get; set; }

		[Outlet]
		AppKit.NSTextField ConcessionField { get; set; }

		[Outlet]
		AppKit.NSTextField ConcessionLabel { get; set; }

		[Outlet]
		AppKit.NSTextField HOIField { get; set; }

		[Outlet]
		AppKit.NSTextField HOILabel { get; set; }

		[Outlet]
		AppKit.NSTextField InitialDrawField { get; set; }

		[Outlet]
		AppKit.NSTextField InitialDrawLabel { get; set; }

		[Outlet]
		AppKit.NSComboBox LenderComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField LenderLabel { get; set; }

		[Outlet]
		AppKit.NSTextField PointsLabel { get; set; }

		[Outlet]
		AppKit.NSTextField PriceField { get; set; }

		[Outlet]
		AppKit.NSTextField PriceLabel { get; set; }

		[Outlet]
		AppKit.NSTextField ProcessingField { get; set; }

		[Outlet]
		AppKit.NSTextField ProcessingLabel { get; set; }

		[Outlet]
		AppKit.NSTextField PropertyTaxField { get; set; }

		[Outlet]
		AppKit.NSTextField PropertyTaxLabel { get; set; }

		[Outlet]
		AppKit.NSTextField RecordingField { get; set; }

		[Outlet]
		AppKit.NSTextField RecordingLabel { get; set; }

		[Outlet]
		AppKit.NSTextField SummaryMessageField { get; set; }

		[Outlet]
		AppKit.NSTextField TitlePolicyField { get; set; }

		[Outlet]
		AppKit.NSTextField TitlePolicyLabel { get; set; }

		[Outlet]
		AppKit.NSTextField TotalCostLabel { get; set; }

		[Outlet]
		AppKit.NSButton UpdateAcquisitionButton { get; set; }

		[Outlet]
		AppKit.NSTextField UpdatedPointsLabel { get; set; }

		[Action ("AcqTaxUpdated:")]
		partial void AcqTaxUpdated (AppKit.NSTextField sender);

		[Action ("CancelButtonPushed:")]
		partial void CancelButtonPushed (AppKit.NSButton sender);

		[Action ("ConcessionUpdated:")]
		partial void ConcessionUpdated (AppKit.NSTextField sender);

		[Action ("DrawUpdated:")]
		partial void DrawUpdated (AppKit.NSTextField sender);

		[Action ("GenerateDocsPressed:")]
		partial void GenerateDocsPressed (AppKit.NSButton sender);

		[Action ("HOIUpdated:")]
		partial void HOIUpdated (AppKit.NSTextField sender);

		[Action ("MarkActualPressed:")]
		partial void MarkActualPressed (AppKit.NSButton sender);

		[Action ("PriceUpdated:")]
		partial void PriceUpdated (AppKit.NSTextField sender);

		[Action ("ProcessingUpdated:")]
		partial void ProcessingUpdated (AppKit.NSTextField sender);

		[Action ("PropertyChosen:")]
		partial void PropertyChosen (AppKit.NSComboBox sender);

		[Action ("PropertyTaxUpdated:")]
		partial void PropertyTaxUpdated (AppKit.NSTextField sender);

		[Action ("RecordingUpdated:")]
		partial void RecordingUpdated (AppKit.NSTextField sender);

		[Action ("TitlePolicyUpdated:")]
		partial void TitlePolicyUpdated (AppKit.NSTextField sender);

		[Action ("UpdateButtonPushed:")]
		partial void UpdateButtonPushed (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AcqTaxField != null) {
				AcqTaxField.Dispose ();
				AcqTaxField = null;
			}

			if (AcqTaxLabel != null) {
				AcqTaxLabel.Dispose ();
				AcqTaxLabel = null;
			}

			if (AddressComboBox != null) {
				AddressComboBox.Dispose ();
				AddressComboBox = null;
			}

			if (BorrowerComboBox != null) {
				BorrowerComboBox.Dispose ();
				BorrowerComboBox = null;
			}

			if (BorrowerLabel != null) {
				BorrowerLabel.Dispose ();
				BorrowerLabel = null;
			}

			if (ClosingDateLabel != null) {
				ClosingDateLabel.Dispose ();
				ClosingDateLabel = null;
			}

			if (ClosingDatePicker != null) {
				ClosingDatePicker.Dispose ();
				ClosingDatePicker = null;
			}

			if (ConcessionField != null) {
				ConcessionField.Dispose ();
				ConcessionField = null;
			}

			if (ConcessionLabel != null) {
				ConcessionLabel.Dispose ();
				ConcessionLabel = null;
			}

			if (HOIField != null) {
				HOIField.Dispose ();
				HOIField = null;
			}

			if (HOILabel != null) {
				HOILabel.Dispose ();
				HOILabel = null;
			}

			if (InitialDrawField != null) {
				InitialDrawField.Dispose ();
				InitialDrawField = null;
			}

			if (InitialDrawLabel != null) {
				InitialDrawLabel.Dispose ();
				InitialDrawLabel = null;
			}

			if (LenderComboBox != null) {
				LenderComboBox.Dispose ();
				LenderComboBox = null;
			}

			if (LenderLabel != null) {
				LenderLabel.Dispose ();
				LenderLabel = null;
			}

			if (PriceField != null) {
				PriceField.Dispose ();
				PriceField = null;
			}

			if (PriceLabel != null) {
				PriceLabel.Dispose ();
				PriceLabel = null;
			}

			if (ProcessingField != null) {
				ProcessingField.Dispose ();
				ProcessingField = null;
			}

			if (ProcessingLabel != null) {
				ProcessingLabel.Dispose ();
				ProcessingLabel = null;
			}

			if (PropertyTaxField != null) {
				PropertyTaxField.Dispose ();
				PropertyTaxField = null;
			}

			if (PropertyTaxLabel != null) {
				PropertyTaxLabel.Dispose ();
				PropertyTaxLabel = null;
			}

			if (RecordingField != null) {
				RecordingField.Dispose ();
				RecordingField = null;
			}

			if (RecordingLabel != null) {
				RecordingLabel.Dispose ();
				RecordingLabel = null;
			}

			if (SummaryMessageField != null) {
				SummaryMessageField.Dispose ();
				SummaryMessageField = null;
			}

			if (TitlePolicyField != null) {
				TitlePolicyField.Dispose ();
				TitlePolicyField = null;
			}

			if (TitlePolicyLabel != null) {
				TitlePolicyLabel.Dispose ();
				TitlePolicyLabel = null;
			}

			if (TotalCostLabel != null) {
				TotalCostLabel.Dispose ();
				TotalCostLabel = null;
			}

			if (UpdateAcquisitionButton != null) {
				UpdateAcquisitionButton.Dispose ();
				UpdateAcquisitionButton = null;
			}

			if (PointsLabel != null) {
				PointsLabel.Dispose ();
				PointsLabel = null;
			}

			if (UpdatedPointsLabel != null) {
				UpdatedPointsLabel.Dispose ();
				UpdatedPointsLabel = null;
			}
		}
	}
}
