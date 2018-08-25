using System;
using Foundation;
using AppKit;
using System.Collections.Generic;
using ResilienceClasses;

namespace DocumentHistory
{
    public class DocumentRecordTableDataSource : NSTableViewDataSource
    {
        public List<clsDocumentRecord> data;
        public List<string> documentNames = new List<string>();
        public List<string> entityNames = new List<string>();

        public DocumentRecordTableDataSource()
        {
            this.data = new List<clsDocumentRecord>();
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return data.Count;
        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            string text = string.Empty;
            int irow = (int)row;
            switch (tableColumn.Title)
            {
                case "Document":
                    text = this.documentNames[this.data[irow].DocumentID()];
                    break;

                case "Date":
                    text = this.data[irow].ActionDate().ToString("MM/dd/yy");
                    break;

                case "Record":
                    text = this.data[irow].RecordDate().ToString("MM/dd/yy");
                    break;

                case "Sender":
                    text = this.entityNames[this.data[irow].SenderID()];
                    break;

                case "Receiver":
                    text = this.entityNames[this.data[irow].ReceiverID()];
                    break;

                case "Status":
                    text = this.data[irow].StatusType().ToString();
                    break;

                case "Transmission":
                    text = this.data[irow].TransmissionType().ToString();
                    break;

                default:
                    break;
            }

            return (NSString)text;
        }
        public void Sort(string key, bool ascending)
        {

            // Take action based on key
            switch (key)
            {
                case "Document":
                    if (ascending)
                    {
                        data.Sort((x, y) => this.documentNames[x.DocumentID()].CompareTo(this.documentNames[y.DocumentID()]));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * this.documentNames[x.DocumentID()].CompareTo(this.documentNames[y.DocumentID()]));
                    }
                    break;
                case "Date":
                    if (ascending)
                    {
                        data.Sort((x, y) => x.ActionDate().CompareTo(y.ActionDate()));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * x.ActionDate().CompareTo(y.ActionDate()));
                    }
                    break;
                case "Record":
                    if (ascending)
                    {
                        data.Sort((x, y) => x.RecordDate().CompareTo(y.RecordDate()));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * x.RecordDate().CompareTo(y.RecordDate()));
                    }
                    break;
                case "Status":
                    if (ascending)
                    {
                        data.Sort((x, y) => x.StatusType().CompareTo(y.StatusType()));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * x.StatusType().CompareTo(y.StatusType()));
                    }
                    break;
                case "Transmission":
                    if (ascending)
                    {
                        data.Sort((x, y) => x.TransmissionType().CompareTo(y.TransmissionType()));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * x.TransmissionType().CompareTo(y.TransmissionType()));
                    }
                    break;
                case "Sender":
                    if (ascending)
                    {
                        data.Sort((x, y) => this.entityNames[x.SenderID()].CompareTo(this.entityNames[y.SenderID()]));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * this.entityNames[x.SenderID()].CompareTo(this.entityNames[y.SenderID()]));
                    }
                    break;
                case "Receiver":
                    if (ascending)
                    {
                        data.Sort((x, y) => this.entityNames[x.ReceiverID()].CompareTo(this.entityNames[y.ReceiverID()]));
                    }
                    else
                    {
                        data.Sort((x, y) => -1 * this.entityNames[x.ReceiverID()].CompareTo(this.entityNames[y.ReceiverID()]));
                    }
                    break;
            }

        }

        public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors)
        {
            //// Sort the data
            //if (oldDescriptors.Length > 0)
            //{
            //    // Update sort
            //    Sort(oldDescriptors[0].Key, oldDescriptors[0].Ascending);
            //}
            //else
            //{
            // Grab current descriptors and update sort
            NSSortDescriptor[] tbSort = tableView.SortDescriptors;
            Sort(tbSort[0].Key, tbSort[0].Ascending);
            //}

            // Refresh table
            tableView.ReloadData();
        }

    }
}
