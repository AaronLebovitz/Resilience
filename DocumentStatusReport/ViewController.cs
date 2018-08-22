using System;
using System.Collections.Generic;
using ResilienceClasses;
using AppKit;
using Foundation;

namespace DocumentStatusReport
{
    public partial class ViewController : NSViewController
    {
        private int lenderID = -1;
        private List<int> lenderIndexToID = new List<int>();
        private List<int> lenderLoanIDs = new List<int>();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            clsCSVTable tblLenders = new clsCSVTable(clsEntity.strEntityPath);
            clsCSVTable tblLoans = new clsCSVTable(clsLoan.strLoanPath);
            for (int i = 0; i < tblLenders.Length(); i++)
            {
                if (tblLoans.Matches(clsLoan.LenderColumn, i.ToString()).Count > 0)
                {
                    this.LenderPopUpButton.AddItem(tblLenders.Value(i, clsEntity.NameColumn));
                    this.lenderIndexToID.Add(i);
                }
            }
            for (int i = 0; i < tblLoans.Length(); i++)
                this.lenderLoanIDs.Add(i);

            // Do any additional setup after loading the view.
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

        partial void LenderSelected(NSPopUpButton sender)
        {
            this.lenderID = this.lenderIndexToID[(int)this.LenderPopUpButton.IndexOfSelectedItem - 1];
        }

        partial void RunReportPressed(NSButton sender)
        {
            // open file
            string fileName = "/Volumes/GoogleDrive/Team Drives/Resilience/Reports/DocumentStatus";
            fileName += System.DateTime.Today.ToString("yyyyMMdd");
            fileName += "." + this.LenderPopUpButton.TitleOfSelectedItem;
            fileName += ".htm";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);

            // write beginning tags and header
            sw.WriteLine("<!DOCTYPE html><html>");
            sw.WriteLine("<head>");
            sw.WriteLine("<style>");
            sw.WriteLine("th {text-decoration:underline;}");
            sw.WriteLine("tr {background-color:WhiteSmoke;}");
            sw.WriteLine("td#MUSTHAVE {background-color:Red;}");
            sw.WriteLine("td#SHOULDHAVE {background-color:Yellow;}");
            sw.WriteLine("td#TODO {background-color:Green;}");
            sw.WriteLine("td#COMPLETE {background-color:White;}");
            sw.WriteLine("tr#HEADER {background-color:White;}");
            sw.WriteLine("</style>");
            sw.WriteLine("</head>");

            // html body
            sw.WriteLine("<body>");
            sw.WriteLine("<h1>Document Status Report  " + System.DateTime.Today.ToString("MM/dd/yyyy") + "</h1>");
            sw.WriteLine("<table style=\"width:auto\">");
            WriteHTMLHeaderRow(sw);

            // loop through loans
            clsCSVTable tbl = new clsCSVTable(clsLoan.strLoanPath);
            clsCSVTable tblDocuments = new clsCSVTable(clsDocument.strDocumentPath);
            clsCSVTable tblDocumentRecords = new clsCSVTable(clsDocumentRecord.strDocumentRecordPath);
            for (int i = 0; i < tbl.Length(); i++)
            {
                if (this.lenderLoanIDs.Contains(i))
                {
                    // load document list
                    clsLoan loan = new clsLoan(i);
                    Dictionary<int, clsDocument> documentList = new Dictionary<int, clsDocument>();
                    List<int> documentListIDs = new List<int>();
                    documentListIDs = tblDocuments.Matches(clsDocument.PropertyColumn, loan.PropertyID().ToString());

                    // load document records
                    List<clsDocumentRecord> documentRecordList = new List<clsDocumentRecord>();
                    List<int> documentRecordIDs = new List<int>();
                    List<clsDocument.Type> documentTypes = new List<clsDocument.Type>();

                    foreach (int id in documentListIDs) 
                    { 
                        documentList.Add(id, new clsDocument(id));
                        foreach (int recID in tblDocumentRecords.Matches(clsDocumentRecord.DocumentColumn, id.ToString()))
                        {
                            documentRecordIDs.Add(recID);
                            documentRecordList.Add(new clsDocumentRecord(recID));
                            documentTypes.Add(documentList[id].DocumentType());
                        }
                    }

                    // write loan to report
                    this.WriteLoanHTML(loan.Property().Address(), loan.Status(), sw, documentRecordList, documentTypes);
                }
            }

            // end of doc tags
            sw.WriteLine("<tr><td>TOTALS</td></tr>");
            sw.Write("<tr text-decoration:underline><td></td><td></td>");
        }

        private void WriteHTMLHeaderRow(System.IO.StreamWriter sw)
        {
            string title;
            sw.WriteLine("<tr id=HEADER>");
            title = "Property";
            sw.WriteLine("<th align=\"left\">" + title + "</th>");
            foreach (clsDocument.Type t in Enum.GetValues(typeof(clsDocument.Type)))
            {
                switch (t)
                {
                    case clsDocument.Type.BPO: title = "BPO"; break;
                    case clsDocument.Type.Calculator: title = "CLC"; break;
                    case clsDocument.Type.ClosingProtectionLetter: title = "CPL"; break;
                    case clsDocument.Type.Discharge: title = "DIS"; break;
                    case clsDocument.Type.EscrowInstructionLetter: title = "EIL"; break;
                    case clsDocument.Type.HomeownersInsurance: title = "HOI"; break;
                    case clsDocument.Type.LoanPayoffLetter: title = "PAY"; break;
                    case clsDocument.Type.Mortgage: title = "MTG"; break;
                    case clsDocument.Type.ProfitStatement: title = "PNL"; break;
                    case clsDocument.Type.ProFormaLenderPolicy: title = "PRO"; break;
                    case clsDocument.Type.PurchaseStatement: title = "PST"; break;
                    case clsDocument.Type.RehabBid: title = "RHB"; break;
                    case clsDocument.Type.SaleContract: title = "SCT"; break;
                    case clsDocument.Type.SaleStatement: title = "SST"; break;
                    case clsDocument.Type.TitleCommitment: title = "TCM"; break;
                    case clsDocument.Type.TitleWork: title = "TWK"; break;
                }
                sw.WriteLine("<th>" + title + "</th>");
            }
            sw.WriteLine("</tr>");
        }

        private void WriteLoanHTML(string address, clsLoan.State status, System.IO.StreamWriter streamWriter, List<clsDocumentRecord> documentRecords, List<clsDocument.Type> docTypes)
        {
            streamWriter.WriteLine("<tr>");
            streamWriter.WriteLine("<td align=\"left\">" + address + "</td>");
            // loop through document types checking for docstatus given loan status
            foreach (clsDocument.Type t in Enum.GetValues(typeof(clsDocument.Type)))
            {
                string[] docStatus = DocStatus(status, documentRecords, docTypes, t);
                streamWriter.Write("<td id=" + docStatus[0] + ">");
                streamWriter.Write(docStatus[1]);
                streamWriter.WriteLine("</td>");
            }
            streamWriter.WriteLine("</tr>");
            streamWriter.Flush();
        }

        private string[] DocStatus(clsLoan.State loanStatus, List<clsDocumentRecord> documentRecords, List<clsDocument.Type> docTypes, clsDocument.Type docType)
        {
            bool red = false;
            bool yellow = false;
            bool green = false;
            clsDocumentRecord.Transmission TAny = clsDocumentRecord.Transmission.Unknown;
            clsDocumentRecord.Status SAny = clsDocumentRecord.Status.Unkown;
            int EAny = -1;
            string reason = " ";

            ///////////////////////////////////////////////////////////////////////////////////
            // Pending Acquisition or Later
            ///////////////////////////////////////////////////////////////////////////////////
            if ((loanStatus == clsLoan.State.PendingAcquisition) ||
                (loanStatus == clsLoan.State.Listed) ||
                (loanStatus == clsLoan.State.Rehab) ||
                (loanStatus == clsLoan.State.PendingSale) ||
                (loanStatus == clsLoan.State.Sold))
            {
                switch (docType)
                {
                    //RED
                    case clsDocument.Type.Calculator:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, EAny, this.lenderID))
                        {
                            red = true;
                            reason = "!r";
                        }
                        break;

                    //YELLOW
                    case clsDocument.Type.BPO:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, EAny, this.lenderID))
                        {
                            reason = "!r";
                            yellow = true;
                        }
                        break;

                    //GREEN
                    case clsDocument.Type.TitleCommitment:
                    case clsDocument.Type.TitleWork:
                    case clsDocument.Type.ProFormaLenderPolicy:
                    case clsDocument.Type.RehabBid:
                    case clsDocument.Type.HomeownersInsurance:
                    case clsDocument.Type.PurchaseStatement:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, EAny, this.lenderID))
                        {
                            reason = "!r";
                            green = true;
                        }
                        break;
                    //case clsDocument.Type.Mortgage:
                        //if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, this.lenderID, EAny))
                        //    green = true;
                        //break;
                    case clsDocument.Type.ClosingProtectionLetter:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                        {
                            reason = "!R";
                            green = true;
                        }
                        break;
                    case clsDocument.Type.EscrowInstructionLetter:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, this.lenderID, EAny))
                        {
                            reason = "!s";
                            green = true;
                        }
                        else if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                        {
                            reason = "!R";
                            green = true;
                        }
                        break;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // Acquired or Later
            ///////////////////////////////////////////////////////////////////////////////////
            if ((loanStatus == clsLoan.State.Listed) ||
                (loanStatus == clsLoan.State.Rehab) ||
                (loanStatus == clsLoan.State.PendingSale) ||
                (loanStatus == clsLoan.State.Sold))
            {
                switch (docType)
                {
                    //RED
                    case clsDocument.Type.ProFormaLenderPolicy:
                    case clsDocument.Type.ClosingProtectionLetter:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, EAny, this.lenderID))
                        {
                            reason = "!r";
                            red = true;
                        }
                        break;
                    case clsDocument.Type.EscrowInstructionLetter:
                    case clsDocument.Type.Mortgage: // no requirement to send executed mortgage to fund admin
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                        {
                            reason = "!R";
                            red = true;
                        }
                        break;
                    case clsDocument.Type.PurchaseStatement:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, this.lenderID, EAny))
                        {
                            reason = "!S";
                            red = true;
                        }
                        else if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                        {
                            reason = "!R";
                            red = true;
                        }
                        break;

                    //YELLOW
                    case clsDocument.Type.RehabBid:
                    case clsDocument.Type.HomeownersInsurance:
                    case clsDocument.Type.TitleWork:
                    case clsDocument.Type.TitleCommitment:
                        if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, EAny, this.lenderID))
                        {
                            reason = "!r";
                            yellow = true;
                        }
                        break;
                }
                if ((!red) && (!yellow) && (docType == clsDocument.Type.Mortgage) &&
                    (!DocListContains(documentRecords, docTypes, docType, clsDocumentRecord.Transmission.Post, clsDocumentRecord.Status.Executed, EAny, this.lenderID)))
                {
                    reason = "!RP";
                    yellow = true;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // Pending Sale or Sold
            ///////////////////////////////////////////////////////////////////////////////////
            if ((loanStatus == clsLoan.State.PendingSale) ||
                (loanStatus == clsLoan.State.Sold))
            {
                if ((!red) && (!yellow) && (!green))
                {
                    if ((docType == clsDocument.Type.SaleContract) && (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID)))
                    {
                        reason = "!R";
                        green = true;
                    }
                    else if ((docType == clsDocument.Type.LoanPayoffLetter) && (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, this.lenderID, EAny)))
                    {
                        reason = "!s";
                        green = true;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////
            // Sold
            ///////////////////////////////////////////////////////////////////////////////////
            if (loanStatus == clsLoan.State.Sold)
            {
                if (!red)
                {
                    if ((docType == clsDocument.Type.SaleContract) && (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID)))
                    {
                        reason = "!R";
                        red = true;
                    }
                    else if ((!yellow) && (!green))
                    {
                        if ((docType == clsDocument.Type.Discharge) &&
                            (!DocListContains(documentRecords, docTypes, docType, clsDocumentRecord.Transmission.Post, clsDocumentRecord.Status.Notarized, this.lenderID, EAny)))
                        {
                            reason = "!SN";
                            green = true;
                        }
                        else if (docType == clsDocument.Type.SaleStatement)
                        {
                            if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, this.lenderID, EAny))
                            {
                                reason = "!S";
                                green = true;
                            }
                            else if (!DocListContains(documentRecords, docTypes, docType, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                            {
                                reason = "!R";
                                green = true;
                            }
                        }
                        else if (docType == clsDocument.Type.ProfitStatement)
                        {
                            if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, this.lenderID, EAny))
                            {
                                reason = "!s";
                                green = true;
                            }
                            else if (!DocListContains(documentRecords, docTypes, docType, TAny, SAny, EAny, this.lenderID))
                            {
                                reason = "!r";
                                green = true;
                            }
                        }
                    }
                }
            }

            // calculate return value
            string[] docstatus = new string[2];
            if (red) docstatus[0] = "MUSTHAVE";
            else if (yellow) docstatus[0] = "SHOULDHAVE";
            else if (green) docstatus[0] = "TODO";
            else docstatus[0] = "COMPLETE";
            docstatus[1] = reason;
            return docstatus;
        }

        private bool DocListContains(List<clsDocumentRecord> list, List<clsDocument.Type> docTypesByRecord, clsDocument.Type docType,
                                     clsDocumentRecord.Transmission transmission = clsDocumentRecord.Transmission.Unknown, 
                                     clsDocumentRecord.Status status = clsDocumentRecord.Status.Unkown, 
                                     int senderID = -1, 
                                     int receiverID = -1)
        {
            bool found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (!found)
                {
                    if ((docType == docTypesByRecord[i]) &&
                        ((transmission == clsDocumentRecord.Transmission.Unknown) || (transmission == list[i].TransmissionType())) &&
                        ((status == clsDocumentRecord.Status.Unkown) || (status == list[i].StatusType())) &&
                        ((senderID == -1) || (senderID == list[i].SenderID())) &&
                        ((receiverID == -1) || (receiverID == list[i].ReceiverID())))
                        found = true;
                }
            }
            return found;
        }

        private string DocStatus(clsLoan.State loanStatus, List<clsDocumentRecord> documentRecords, List<clsDocument.Type> docTypes)
        {
            clsDocumentRecord.Transmission TAny = clsDocumentRecord.Transmission.Unknown;
            clsDocumentRecord.Status SAny = clsDocumentRecord.Status.Unkown;
            int EAny = -1;
            bool red = false;
            bool yellow = false;
            bool green = false;

            // if Pending Acquisition or Later
            if ((loanStatus == clsLoan.State.PendingAcquisition) ||
                (loanStatus == clsLoan.State.Listed) ||
                (loanStatus == clsLoan.State.Rehab) ||
                (loanStatus == clsLoan.State.PendingSale) ||
                (loanStatus == clsLoan.State.Sold))
            {
                if (!DocListContains(documentRecords, docTypes, clsDocument.Type.Calculator, TAny, SAny, EAny, this.lenderID))
                    red = true;
                else if (!DocListContains(documentRecords, docTypes, clsDocument.Type.BPO, TAny, SAny, EAny, this.lenderID))
                    yellow = true;
                else if (!((DocListContains(documentRecords, docTypes, clsDocument.Type.Mortgage, TAny, SAny, this.lenderID, EAny))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.PurchaseStatement, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.EscrowInstructionLetter, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.EscrowInstructionLetter, TAny, clsDocumentRecord.Status.Executed, this.lenderID, EAny))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.ClosingProtectionLetter, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.TitleCommitment, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.TitleWork, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.ProFormaLenderPolicy, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.RehabBid, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.HomeownersInsurance, TAny, SAny, EAny, this.lenderID))))
                    green = true;
            }

            // if Acquired or Later
            if ((loanStatus == clsLoan.State.Listed) ||
                (loanStatus == clsLoan.State.Rehab) ||
                (loanStatus == clsLoan.State.PendingSale) ||
                (loanStatus == clsLoan.State.Sold))
            {
                if (!((DocListContains(documentRecords, docTypes, clsDocument.Type.ProFormaLenderPolicy, TAny, SAny, EAny, this.lenderID)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.ClosingProtectionLetter, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.EscrowInstructionLetter, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.PurchaseStatement, clsDocumentRecord.Transmission.Electronic, clsDocumentRecord.Status.Executed, EAny, this.lenderID)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.Mortgage, clsDocumentRecord.Transmission.Electronic, clsDocumentRecord.Status.Executed, EAny, this.lenderID)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.PurchaseStatement, clsDocumentRecord.Transmission.Electronic, clsDocumentRecord.Status.Executed, this.lenderID, EAny)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.Mortgage, clsDocumentRecord.Transmission.Electronic, clsDocumentRecord.Status.Executed, this.lenderID, EAny))))
                    red = true;
                else if (!((DocListContains(documentRecords, docTypes, clsDocument.Type.RehabBid, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.HomeownersInsurance, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.TitleCommitment, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.TitleWork, TAny, SAny, EAny, this.lenderID))
                           && (DocListContains(documentRecords, docTypes, clsDocument.Type.Mortgage, clsDocumentRecord.Transmission.Post, clsDocumentRecord.Status.Executed, EAny, this.lenderID))))
                    yellow = true;
            }

            // if Under Contract to Sell or Sold
            if ((loanStatus == clsLoan.State.PendingSale) ||
                (loanStatus == clsLoan.State.Sold))
            {
                if (!((DocListContains(documentRecords, docTypes, clsDocument.Type.SaleContract, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID)) &&
                      (DocListContains(documentRecords, docTypes, clsDocument.Type.LoanPayoffLetter, TAny, SAny, this.lenderID, EAny))))
                    green = true;
            }

            // if Sold
            if (loanStatus == clsLoan.State.Sold)
            {
                if (!DocListContains(documentRecords, docTypes, clsDocument.Type.SaleContract, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID))
                    red = true;
                else if (!((DocListContains(documentRecords, docTypes, clsDocument.Type.Discharge, clsDocumentRecord.Transmission.Post, clsDocumentRecord.Status.Notarized, this.lenderID, EAny)) &&
                           (DocListContains(documentRecords, docTypes, clsDocument.Type.SaleStatement, TAny, clsDocumentRecord.Status.Executed, EAny, this.lenderID)) &&
                           (DocListContains(documentRecords, docTypes, clsDocument.Type.SaleStatement, TAny, clsDocumentRecord.Status.Executed, this.lenderID, EAny)) &&
                           (DocListContains(documentRecords, docTypes, clsDocument.Type.ProfitStatement, TAny, SAny, EAny, this.lenderID)) &&
                           (DocListContains(documentRecords, docTypes, clsDocument.Type.ProfitStatement, TAny, SAny, this.lenderID, EAny))
                          ))
                    green = true;
            }

            string docstatus = "COMPLETE";
            if (red) docstatus = "MUSTHAVE";
            else if (yellow) docstatus = "SHOULDHAVE";
            else if (green) docstatus = "TODO";

            return docstatus;
        }

    
    }
}
