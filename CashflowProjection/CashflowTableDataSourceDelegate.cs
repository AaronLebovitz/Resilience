using System;
using System.Collections.Generic;
using ResilienceClasses;
using Foundation;
using AppKit;

namespace CashflowProjection
{
    public class CashflowTableDataSourceDelegate : NSTableViewDelegate
    {
        private CashflowTableDataSource dataSource;

        public CashflowTableDataSourceDelegate(CashflowTableDataSource source)
        {
            this.dataSource = source;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTextField view;

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                case "ID":
                    view = (NSTextField)tableView.MakeView("IDCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "IDCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;

                case "Date":
                    view = (NSTextField)tableView.MakeView("DateCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "PayDateCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;

                case "Balance":
                    view = (NSTextField)tableView.MakeView("BalanceCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "BalanceCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                        view.Alignment = NSTextAlignment.Right;
                    }
                    view.DoubleValue = dataSource.Cashflows[(int)row].Amount();
                    break;

                case "Amount":
                    view = (NSTextField)tableView.MakeView("AmountCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "AmountCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                        view.Alignment = NSTextAlignment.Right;
                    }
                    view.DoubleValue = dataSource.Cashflows[(int)row].Amount();
                    break;

                case "Type":
                    view = (NSTextField)tableView.MakeView("TypeCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "TypeCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;

                case "Actual":
                    view = (NSTextField)tableView.MakeView("ActualCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "ActualCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;

                case "Notes":
                    view = (NSTextField)tableView.MakeView("NotesCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "NotesCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;

                case "Property":
                    view = (NSTextField)tableView.MakeView("PropertyCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "PropertyCell";
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
