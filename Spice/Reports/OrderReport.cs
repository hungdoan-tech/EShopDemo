using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;

namespace Spice.Reports
{
    public class OrderReport
    {
        private IWebHostEnvironment _webHostEnvironment;

        public OrderReport(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        int _maxColumn = 5;
        Document _document;
        Font _fontStyle;
        PdfPTable _pdfTable = new PdfPTable(5);

        PdfPCell _pdfCell;
        MemoryStream _memoryStream = new MemoryStream();

        List<OrderDetails> _orderDetails = new List<OrderDetails>();
        string id = "";
        string userName = "";
        string email = "";
        string phoneNumber = "";
        string address = "";
        string comment = "";
        string date = "";
        string subTotal = "";
        string discount = "";
        string total = "";
        string status = "";

    public byte[] Report(List<OrderDetails> orderDetails)
        {
            _orderDetails = orderDetails;
            foreach (var item in _orderDetails)
            {
                id = item.OrderHeader.Id.ToString();
                userName = item.OrderHeader.PickupName;
                email = item.OrderHeader.Email;
                phoneNumber = item.OrderHeader.PhoneNumber;
                address = item.OrderHeader.StreetAddress + ", " + item.OrderHeader.City;
                comment = item.OrderHeader.Comments;
                date = item.OrderHeader.OrderDate.ToString();
                subTotal = "$" + Math.Round(Convert.ToDouble(item.OrderHeader.OrderTotalOriginal), 2).ToString();
                total = "$" + Math.Round(Convert.ToDouble(item.OrderHeader.OrderTotal), 2).ToString();
                discount = "-$" + Math.Round(Convert.ToDouble(item.OrderHeader.CouponCodeDiscount), 2).ToString();
                status = item.OrderHeader.Status;
            }

            _document = new Document();
            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(50f, 50f, 50f, 50f);

            _pdfTable.WidthPercentage = 100;
            
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter docWrite = PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();

            float[] sizes = new float[_maxColumn];
            for(var i = 0; i < _maxColumn; i++)
            {
                if (i == 0) sizes[i] = 15;
                else if (i == 1) sizes[i] = 100;
                else sizes[i] = 30;
            }
            _pdfTable.SetWidths(sizes);
            this.ReportHeader();
            this.EmptyRow(2);

            this.ReportUser();
            this.EmptyRow(2);
            this.ReportTable();
            this.EmptyRow(5);
            this.EndPage();
            _pdfTable.HeaderRows = 2;
            _document.Add(_pdfTable);

            _document.Close();

            return _memoryStream.ToArray();
        }
        private void ReportHeader()
        {
            _pdfCell = new PdfPCell(this.AddLogo());
            _pdfCell.Colspan = 2;
            _pdfCell.Border = 0;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(this.SetPageTitle());
            _pdfCell.Colspan = _maxColumn - 2;
            _pdfCell.Border = 0;
            _pdfTable.AddCell(_pdfCell);

            _pdfTable.CompleteRow();
            EmptyRow(3);
            _pdfCell = new PdfPCell(this.ReportUser());
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.Border = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
        }
        private PdfPTable AddLogo()
        {
            int maxColumn = 1;
            PdfPTable pdfPTable = new PdfPTable(maxColumn);
            string path = _webHostEnvironment.WebRootPath + "/images";
            string imgCombine = Path.Combine(path, "Logo.png");
            Image img = Image.GetInstance(imgCombine);
 

            _pdfCell = new PdfPCell(img);
            _pdfCell.Colspan = maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);
            pdfPTable.CompleteRow();

            return pdfPTable;
        }

        private PdfPTable SetPageTitle()
        {
            
            int maxColumn = 3;
            PdfPTable pdfPTable = new PdfPTable(maxColumn);
            _fontStyle = FontFactory.GetFont("Tahoma", 18f, 1);
            _pdfCell = new PdfPCell(new Phrase("Order Infomation", _fontStyle));
            _pdfCell.Colspan = maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);


            
            _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);
            _pdfCell = new PdfPCell(new Phrase("Invoice ID: " + id, _fontStyle));
            _pdfCell.Colspan = maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();

            return pdfPTable;
        }

        private void EmptyRow(int nCount)
        {
            for(int i = 0; i <= nCount; i++)
            {
                _pdfCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfCell.Colspan = _maxColumn;
                _pdfCell.Border = 0;
                _pdfCell.ExtraParagraphSpace = 10;
                _pdfTable.AddCell(_pdfCell);
                _pdfTable.CompleteRow();
            }    
        }
        private void EndPage()
        {
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 0);
            _pdfCell = new PdfPCell(new Phrase("__________If you have any questions about this invoice_________\n" +
                "Please contact: Time Zone, 990-337-100, tz@gmail.com", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 10;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
        }
        private PdfPTable ReportUser()
        {
            
            int maxColumn = 3;
            PdfPTable pdfPTable = new PdfPTable(maxColumn);
            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _pdfCell = new PdfPCell(new Phrase(
                "From: \n\n" +
                "Time Zone, Ho Chi Minh City \n\n\n" +
                "To:\n\n", _fontStyle));
            _pdfCell.Colspan = maxColumn;
            _pdfCell.Border = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);
            #region User name
            _pdfCell = new PdfPCell(new Phrase("User Name: ", _fontStyle));
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);


            _pdfCell = new PdfPCell(new Phrase(userName, _fontStyle));
            _pdfCell.Colspan = maxColumn - 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();
            #endregion

            #region Email
            _pdfCell = new PdfPCell(new Phrase("Email: ", _fontStyle));
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(email, _fontStyle));
            _pdfCell.Colspan = maxColumn -1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();

            #endregion

            #region Phone number
            _pdfCell = new PdfPCell(new Phrase("Phone Number: ", _fontStyle));
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(phoneNumber, _fontStyle));
            _pdfCell.Colspan = maxColumn - 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();

            #endregion

            #region Address
            _pdfCell = new PdfPCell(new Phrase("Address: ", _fontStyle));
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(address, _fontStyle));
            _pdfCell.Colspan = maxColumn - 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();

            #endregion

            #region Comment
            _pdfCell = new PdfPCell(new Phrase("Comment: ", _fontStyle));
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(comment, _fontStyle));
            _pdfCell.Colspan = maxColumn -1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();

            #endregion

            #region Date
            _pdfCell = new PdfPCell(new Phrase("Order date: ", _fontStyle));
            _pdfCell.Colspan = 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(date, _fontStyle));
            _pdfCell.Colspan = maxColumn - 1;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfCell);

            pdfPTable.CompleteRow();
            
            #endregion

            return pdfPTable;
        }
        private void ReportTable()
        {
            var fontStyleBold = FontFactory.GetFont("Tahoma", 10f, 1);
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            #region Detail table header

            _pdfCell = new PdfPCell(new Phrase("#", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase("Item", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase("Count", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase("Price ($)", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase("Total Price ($)", fontStyleBold));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            #endregion

            #region Detail table body
            int i = 1;
            foreach (var item in _orderDetails)
            {
                _pdfCell = new PdfPCell(new Phrase(i.ToString(), _fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfCell.ExtraParagraphSpace = 5;
                _pdfTable.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(item.Name, _fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfCell.ExtraParagraphSpace = 5;
                _pdfTable.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(item.Count.ToString(), _fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfCell.ExtraParagraphSpace = 5;
                _pdfTable.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(item.MenuItem.Price.ToString(), _fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfCell.ExtraParagraphSpace = 5;
                _pdfTable.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(item.Price.ToString(), _fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfCell.ExtraParagraphSpace = 5;
                _pdfTable.AddCell(_pdfCell);

                _pdfTable.CompleteRow();
                i++;
            }
            #endregion


            EmptyRow(3);

            #region Sub Total price
            _pdfCell = new PdfPCell(new Phrase("Sub Total: ", _fontStyle));
            _pdfCell.Colspan = 4;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.White;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);


            _pdfCell = new PdfPCell(new Phrase(subTotal, _fontStyle));
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.White;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            #endregion

            #region Discount
            _pdfCell = new PdfPCell(new Phrase("Discount: ", _fontStyle));
            _pdfCell.Colspan = 4;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.White;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);


            _pdfCell = new PdfPCell(new Phrase(discount, _fontStyle));
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.White;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            #endregion

            #region Total price
            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _pdfCell = new PdfPCell(new Phrase("Total: ", _fontStyle));
            _pdfCell.Colspan = 4;
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.White;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);


            _pdfCell = new PdfPCell(new Phrase(total, _fontStyle));
            _pdfCell.Border = 0;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.White;
            _pdfCell.ExtraParagraphSpace = 5;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            #endregion

            #region Status
            EmptyRow(3);
            if (status == "Completed")
            {
                string path = _webHostEnvironment.WebRootPath + "/images";
                string imgCombine = Path.Combine(path, "complete.png");
                Image img = Image.GetInstance(imgCombine);
                img.ScaleAbsolute(120f, 80f);
                _pdfCell = new PdfPCell(img);
                _pdfCell.Colspan = _maxColumn;
                _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfCell.Border = 0;
                _pdfCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfCell);
            }
            #endregion
        }

    }
}
