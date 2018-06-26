using System;
using System.Collections.Generic;
using ResilienceClasses;
using AppKit;
using Foundation;

namespace ResilienceRecordDocument
{
    public partial class ViewController : NSViewController
    {
        List<clsDocument> docList;
        List<clsEntity> entityList;
        string addressSelected;
        int docID;
        int senderID;
        int receiverID;
        clsDocumentRecord.Status status;
        clsDocumentRecord.Transmission transmittal;
        DateTime dtAction;
        DateTime dtRecord;
        DateTime dtMinDate = new DateTime(2017, 02, 23);

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            this.PropertyChooser.DataSource = new ComboBoxStringListDataSource(clsProperty.AddressList());

            DocumentChooser.RemoveAll();
            foreach (clsDocument.Type t in Enum.GetValues(typeof(clsDocument.Type)))
            {
                DocumentChooser.Add((NSString)t.ToString());
            }

            this.entityList = new List<clsEntity>();
            SenderChooser.RemoveAll();
            ReceiverChooser.RemoveAll();
            clsCSVTable tblEntities = new clsCSVTable(clsEntity.strEntityPath);
            for (int i = 0; i < tblEntities.Length(); i++)
            {
                SenderChooser.Add((NSString)tblEntities.Value(i, clsEntity.NameColumn));
                ReceiverChooser.Add((NSString)tblEntities.Value(i, clsEntity.NameColumn));
                this.entityList.Add(new clsEntity(i));
            }

            foreach (clsDocumentRecord.Status s in Enum.GetValues(typeof(clsDocumentRecord.Status)))
            { StatusChooser.Add((NSString)s.ToString()); }

            foreach (clsDocumentRecord.Transmission t in Enum.GetValues(typeof(clsDocumentRecord.Transmission)))
            { TransmitChooser.Add((NSString)t.ToString()); }

            ActionDateChooser.DateValue = (NSDate)System.DateTime.Today;
            RecordDateChooser.DateValue = (NSDate)System.DateTime.Today;

            ChosenDocumentLabel.StringValue = "";
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        partial void RecordButtonPushed(NSButton sender)
        {
            if ((docID >= 0) && (senderID >= 0) && (receiverID >= 0) && (dtAction > dtMinDate) && (dtRecord > dtMinDate))
            {
                clsDocumentRecord newRecord = new clsDocumentRecord(docID, dtRecord, dtAction, senderID, receiverID, status, transmittal);
                if (newRecord.Save())
                {
                    SaveMessage.StringValue = "Saved New Document Record " + newRecord.ID().ToString();
                    SaveMessage.TextColor = NSColor.Black;
                }
                else
                {
                    SaveMessage.StringValue = "Save Failed";
                    SaveMessage.TextColor = NSColor.Red;
                }
            }
            else
            {
                SaveMessage.StringValue = "Unable to Create New Record";
                SaveMessage.TextColor = NSColor.Red;
            }
        }

        partial void PropertyChosen(NSComboBox sender)
        {
            this.Update();
        }

        partial void DocumentChosen(NSComboBox sender)
        {
            this.Update();
        }

        partial void SenderChosen(NSComboBox sender)
        {
            this.Update();
        }

        partial void ReceiverChosen(NSComboBox sender)
        {
            this.Update();
        }

        partial void StatusChosen(NSComboBox sender)
        {
            this.Update();
        }

        partial void TransmitChosen(NSComboBox sender)
        {
            this.Update();
        }

        partial void ActionDateChosen(NSDatePicker sender)
        {
            this.Update();
        }

        partial void RecordDateChosen(NSDatePicker sender)
        {
            this.Update();
        }

        private void Update()
        {
            // Update Stored Values
            this.addressSelected = ((ComboBoxStringListDataSource)PropertyChooser.DataSource).Value((int)PropertyChooser.SelectedIndex);
            this.docList = clsDocument.Documents(clsProperty.IDFromAddress(this.addressSelected));
            this.docID = -1;
            if (this.docList != null)
            {
                foreach (clsDocument doc in this.docList)
                {
                    if ((int)doc.DocumentType() == DocumentChooser.SelectedIndex) this.docID = doc.ID();
                }
            }
            this.senderID = (int)SenderChooser.SelectedIndex;
            this.receiverID = (int)ReceiverChooser.SelectedIndex;
            if ((int)StatusChooser.SelectedIndex >= 0)
            {
                this.status = (clsDocumentRecord.Status)((int)StatusChooser.SelectedIndex);
            }
            else this.status = clsDocumentRecord.Status.Unkown;
            if ((int)TransmitChooser.SelectedIndex >= 0)
            {
                this.transmittal = (clsDocumentRecord.Transmission)((int)TransmitChooser.SelectedIndex);
            }
            else this.transmittal = clsDocumentRecord.Transmission.Unknown;

            // Update Labels
            ChosenDocumentLabel.StringValue = "";
            if (this.addressSelected != null)
            {
                ChosenDocumentLabel.StringValue = this.addressSelected;
            }
            if (DocumentChooser.SelectedIndex >= 0)
            {
                ChosenDocumentLabel.StringValue += " | " + ((clsDocument.Type)((int)DocumentChooser.SelectedIndex)).ToString();
            }
            ChosenDocumentLabel.StringValue += " (" + docID.ToString() + ")";

            SenderReceiverLabel.StringValue = "";
            if (senderID >= 0) SenderReceiverLabel.StringValue = entityList[senderID].Name();
            SenderReceiverLabel.StringValue += " --> ";
            if (receiverID >= 0) SenderReceiverLabel.StringValue += entityList[receiverID].Name();
            SenderReceiverLabel.StringValue += "  |  " + this.status.ToString() + "," + this.transmittal.ToString();

            dtAction = (DateTime)ActionDateChooser.DateValue;
            dtRecord = (DateTime)RecordDateChooser.DateValue;

            SaveMessage.StringValue = "";
        }
    }
}
