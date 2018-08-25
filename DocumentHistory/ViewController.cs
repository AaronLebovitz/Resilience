using System;
using System.Collections.Generic;
using ResilienceClasses;
using AppKit;
using Foundation;

namespace DocumentHistory
{
    public partial class ViewController : NSViewController
    {

        private clsCSVTable tblEntities = new clsCSVTable(clsEntity.strEntityPath);
        private clsCSVTable tblDocuments = new clsCSVTable(clsDocument.strDocumentPath);
        private Dictionary<string, int> dictLoanIDsByAddress = clsLoan.LoanIDsByAddress();
        private clsCSVTable tblDocumentRecords = new clsCSVTable(clsDocumentRecord.strDocumentRecordPath);
        private DocumentRecordTableDataSource dataSource;
        private DocumentRecordTableDataSourceDelegate dataSourceDelegate;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Initalize Property Chooser
            this.PropertyChooser.RemoveAll();
            this.PropertyChooser.Add((NSString)"Choose Property");
            List<string> addresses = clsProperty.AddressList();
            foreach (string s in addresses) this.PropertyChooser.Add((NSString)s);
            this.PropertyChooser.SelectItem(0);

            // Initialize Filters
            this.SenderFilter.RemoveAll();
            this.ReceiverFilter.RemoveAll();
            this.SenderFilter.Add((NSString)"All Senders");
            this.ReceiverFilter.Add((NSString)"All Receivers");
            for (int i = 0; i < this.tblEntities.Length(); i++)
            {
                this.SenderFilter.Add((NSString)tblEntities.Value(i, clsEntity.NameColumn));
                this.ReceiverFilter.Add((NSString)tblEntities.Value(i, clsEntity.NameColumn));
            }
            this.SenderFilter.SelectItem(0);
            this.ReceiverFilter.SelectItem(0);

            this.StatusFilter.RemoveAll();
            this.StatusFilter.Add((NSString)"All Status");
            for (int i = 0; i < Enum.GetValues(typeof(clsDocumentRecord.Status)).Length; i++)
                this.StatusFilter.Add((NSString)((clsDocumentRecord.Status)i).ToString());
            this.StatusFilter.SelectItem(0);

            this.TypeFilter.RemoveAll();
            this.TypeFilter.Add((NSString)"All Types");
            for (int i = 0; i < Enum.GetValues(typeof(clsDocument.Type)).Length; i++)
                this.TypeFilter.Add((NSString)((clsDocument.Type)i).ToString());
            this.TypeFilter.SelectItem(0);

            // Initialize Data Source
            this.dataSource= new DocumentRecordTableDataSource();
            this.dataSourceDelegate = new DocumentRecordTableDataSourceDelegate(this.dataSource);
            this.DocRecTableView.DataSource = this.dataSource;
            this.DocRecTableView.Delegate = this.dataSourceDelegate;
            for (int i = 0; i < tblEntities.Length(); i++)
                this.dataSource.entityNames.Add(tblEntities.Value(i, clsEntity.NameColumn));
            for (int i = 0; i < tblDocuments.Length(); i++)
                this.dataSource.documentNames.Add(((clsDocument.Type)(Int32.Parse(tblDocuments.Value(i, clsDocument.TypeColumn)))).ToString());
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

        partial void PropertyChosen(NSComboBox sender)
        {
            this.RedrawTable();
        }

        partial void ReceiverChosen(NSComboBox sender)
        {
            this.RedrawTable();
        }

        partial void SenderChosen(NSComboBox sender)
        {
            this.RedrawTable();
        }

        partial void StatusChosen(NSComboBox sender)
        {
            this.RedrawTable();
        }

        partial void TypeChhosen(NSComboBox sender)
        {
            this.RedrawTable();
        }

        private void RedrawTable()
        {
            // instantiate loan that matches address chosen
            if (this.PropertyChooser.SelectedIndex != 0)
            {
                clsLoan loan = new clsLoan(dictLoanIDsByAddress[this.PropertyChooser.StringValue]);

                // load all documents pertaining to that property, and all document records pertaining to each document
                List<int> documentIDs = tblDocuments.Matches(clsDocument.PropertyColumn, loan.PropertyID().ToString());
                List<clsDocument> documents = new List<clsDocument>();
                Dictionary<int, List<clsDocumentRecord>> documentRecords = new Dictionary<int, List<clsDocumentRecord>>();
                foreach (int id in documentIDs)
                {
                    documents.Add(new clsDocument(id));
                    documentRecords.Add(id, new List<clsDocumentRecord>());
                    foreach (int docrecid in tblDocumentRecords.Matches(clsDocumentRecord.DocumentColumn, id.ToString()))
                        documentRecords[id].Add(new clsDocumentRecord(docrecid));
                }

                // apply filters (date range, type, status, transmission, sender, receiver) to construct datasource
                // construct datasource
                this.dataSource.data = new List<clsDocumentRecord>();
                foreach (int id in documentIDs)
                {
                    foreach (clsDocumentRecord rec in documentRecords[id])
                    {
                        bool ok = true;
                        ok = (ok) && (((int)rec.StatusType() == this.StatusFilter.SelectedIndex - 1) || (this.StatusFilter.SelectedIndex == 0));
                        ok = (ok) && (((int)rec.SenderID() == this.SenderFilter.SelectedIndex - 1) || (this.SenderFilter.SelectedIndex == 0));
                        ok = (ok) && (((int)rec.ReceiverID() == this.ReceiverFilter.SelectedIndex - 1) || (this.ReceiverFilter.SelectedIndex == 0));
                        //                    ok = (ok) && (((int)rec.DocumentID() == this.TypeFilter.SelectedIndex - 1) || (this.TypeFilter.SelectedIndex == 0)); 
                        if (ok) this.dataSource.data.Add(rec);
                    }
                }

                // ALLOW SORTING
                this.DocRecTableView.ReloadData();
            }
        }
    }
}

