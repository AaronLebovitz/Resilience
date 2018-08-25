using System;
using Foundation;
using AppKit;
using System.Collections.Generic;
using ResilienceClasses;

namespace DocumentHistory
{
    public class DocumentRecordTableDataSourceDelegate : NSTableViewDelegate
    {
        private DocumentRecordTableDataSource dataSource;

        public DocumentRecordTableDataSourceDelegate(DocumentRecordTableDataSource source)
        {
            this.dataSource = source;
        }


        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTextField view;

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                
                case "Document":
                case "Sender":
                case "Receiver":
                case "Status":
                case "Transmission":
                    view = (NSTextField)tableView.MakeView("StringCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "StringCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                        view.Alignment = NSTextAlignment.Left;
                    }
                    break;

                case "Date":
                case "Record":
                    view = (NSTextField)tableView.MakeView("DateCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "DateCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;


                default:
                    view = null;
                    break;

            }

            return view;
        }

    }
}
