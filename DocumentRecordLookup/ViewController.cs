using System;
using ResilienceClasses;
using AppKit;
using Foundation;

namespace DocumentRecordLookup
{
    public partial class ViewController : NSViewController
    {
        System.Collections.Generic.Dictionary<string, int> propertyIDs = new System.Collections.Generic.Dictionary<string, int>();
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
                        propertyMenu.RemoveItem("Item 2");
                        propertyMenu.RemoveItem("Item 3");

            //            clsCSVTable tbl = new clsCSVTable(clsProperty.strPropertyPath);
            System.Collections.Generic.List<string> addressList = new System.Collections.Generic.List<string>();


            //            for (int i = 0; i < tbl.Length(); i++)
            //            {
            // clsProperty property = new clsProperty(i);
            //  propertyIDs.Add(property.Address(), i);
            //  int streetNumber = Int32.Parse(System.Text.RegularExpressions.Regex.Match(property.Address(), @"\d+").Value);
            //  string streetName = System.Text.RegularExpressions.Regex.Match(property.Address(), @"[a-zA-Z\s]+").Value.Trim();
            //  addressList.Add(streetName + " " + streetNumber.ToString("00000000"));
            //            }
            //            addressList.Sort();

            //            foreach (string s in addressList)
            //            {
            //   int streetNumber = Int32.Parse(System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value);
            //  string streetName = System.Text.RegularExpressions.Regex.Match(s, @"[a-zA-Z\s]+").Value.Trim();
            //  propertyMenu.AddItem(streetNumber.ToString() + " " + streetName);
            // Do any additional setup after loading the view.
            //            }

            addressList = clsProperty.AddressList();
            foreach (string s in addressList) propertyMenu.AddItem(s);
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

        partial void propertyMenuChosen(AppKit.NSPopUpButton sender)
        {

            documentRecordHistory.StringValue = " ";
            documentRecordHistory.StringValue += "\t" + "\t" + "\t" + "Sender           " + "Reciever         " + " Action Date     " + "Record Date     " + "\n";

            clsCSVTable tbl = new clsCSVTable(clsDocument.strDocumentPath);
            int chosenID = clsLoan.LoanID(propertyMenu.TitleOfSelectedItem);
            Console.WriteLine(chosenID.ToString());

            System.Collections.Generic.List <clsDocument> documentList = new System.Collections.Generic.List<clsDocument>();
            System.Collections.Generic.List<int> documentlistIDs = new System.Collections.Generic.List<int>();
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (chosenID.ToString()==tbl.Value(i,clsDocument.PropertyColumn))
                {
                    clsDocument document = new clsDocument(i);
                    documentList.Add(document);
                    Console.Write(tbl.Value(i, clsDocument.PropertyColumn));
                    documentlistIDs.Add(i);
                }
               
            }

            clsCSVTable documentRecordTable = new clsCSVTable(clsDocumentRecord.strDocumentRecordPath);
            clsCSVTable tblEntities = new clsCSVTable(clsEntity.strEntityPath);
            for (int iDocIndex = 0; iDocIndex < documentlistIDs.Count; iDocIndex++)
            {
                int docID = documentlistIDs[iDocIndex];
                documentRecordHistory.StringValue += documentList[iDocIndex].Name() + "\n";
                for (int i = 0; i < documentRecordTable.Length(); i++)
                {
                    if (documentRecordTable.Value(i, clsDocumentRecord.DocumentColumn) == docID.ToString())
                    {
                        int senderID = Int32.Parse(documentRecordTable.Value(i, clsDocumentRecord.SenderColumn));
                        int receiverID = Int32.Parse(documentRecordTable.Value(i, clsDocumentRecord.ReceiverColumn));
                        int status = Int32.Parse(documentRecordTable.Value(i, clsDocumentRecord.StatusColumn));
                        int transmission = Int32.Parse(documentRecordTable.Value(i, clsDocumentRecord.TransmissionColumn));
                        Console.WriteLine(documentRecordTable.Value(i, clsDocumentRecord.DocumentColumn));
                        //documentRecordHistory.StringValue += "Doc record ID: " + i + ", Action date: " + (documentRecordTable.Value(i, clsDocumentRecord.ActionDateColumn)) + ", Record date: "+ (documentRecordTable.Value(i, clsDocumentRecord.RecordDateColumn)) + ", Sender ID: " + (documentRecordTable.Value(i, clsDocumentRecord.SenderColumn)) + ", Reciever ID: " + (documentRecordTable.Value(i, clsDocumentRecord.ReceiverColumn)) + ", Status: " + (documentRecordTable.Value(i, clsDocumentRecord.StatusColumn)) + ", Transmission method: " + (documentRecordTable.Value(i, clsDocumentRecord.TransmissionColumn)) + "\n";

                        string Sender = tblEntities.Value(senderID, clsEntity.NameColumn);
                        string Reciever = tblEntities.Value(receiverID, clsEntity.NameColumn);
                        clsDocumentRecord.Status Status = (clsDocumentRecord.Status)(status);
                        string StatusString = Status.ToString();
                        //Titles
                       
                         
                        if (Sender.Length>15)
                        {
                            Sender = Sender.Substring(0, 15);
                        }
                        else
                        {
                            while (Sender.Length<15)
                            {
                                Sender = Sender + " ";
                            }
                        }
                        documentRecordHistory.StringValue += "\t" + "\t" + "\t" + Sender;
                        // Now it is the reciever length
                        if (Reciever.Length > 15)
                        {
                            Reciever= Reciever.Substring(0, 15);
                        }
                        else
                        {
                            while (Reciever.Length < 15)
                            {
                                Reciever = Reciever + " ";
                            }
                        }
                        documentRecordHistory.StringValue += ", " + Reciever;

                        documentRecordHistory.StringValue +=   ",  " + DateTime.Parse(documentRecordTable.Value(i, clsDocumentRecord.ActionDateColumn)).ToString("MM/dd/yy hh:mm");
                        documentRecordHistory.StringValue +=  ", " +  DateTime.Parse(documentRecordTable.Value(i, clsDocumentRecord.RecordDateColumn)).ToString("MM/dd/yy hh:mm");
                        while (StatusString.Length < 12)
                        {
                            StatusString = StatusString + " ";
                        }
                        documentRecordHistory.StringValue +=  ", " + StatusString;
                        documentRecordHistory.StringValue +=  ", " + (clsDocumentRecord.Transmission)(transmission) +"\n";

                       


                    }
                }
            }
        }
    }
}
