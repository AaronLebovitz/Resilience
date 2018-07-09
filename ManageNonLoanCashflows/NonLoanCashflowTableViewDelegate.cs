using System;
using System.Collections.Generic;
using ResilienceClasses;
using Foundation;
using AppKit;

namespace ManageNonLoanCashflows
{
    public class NonLoanCashflowTableViewDelegate : NSTableViewDelegate
    {
        private NonLoanCashflowDataSource dataSource;

        public NonLoanCashflowTableViewDelegate(NonLoanCashflowDataSource dataSource)
        {
            this.dataSource = dataSource;
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

                case "PayDate":
                    view = (NSTextField)tableView.MakeView("PayDateCell", this);
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

                case "RecDate":
                    view = (NSTextField)tableView.MakeView("RecDateCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "RecDateCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    view.StringValue = dataSource.Cashflows[(int)row].RecordDate().ToShortDateString();
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

                case "DelDate":
                    view = (NSTextField)tableView.MakeView("DelDateCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "DelDateCell";
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Selectable = false;
                        view.Editable = false;
                    }
                    break;

                case "Comment":
                    view = (NSTextField)tableView.MakeView("CommentCell", this);
                    if (view == null)
                    {
                        view = new NSTextField();
                        view.Identifier = "CommentCell";
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
