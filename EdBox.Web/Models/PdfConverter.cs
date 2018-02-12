using System;
using System.Drawing;
using System.IO;
using System.Web;
using HiQPdf;

namespace EdBox.Web.Models
{
    public class PdfConverter
    {
        private const string LicenseKey = "6qKDu7qOjKaDiJiLmJPfxNLK28rbytvb2N/K2dvE29jE09PT0w==";
        public static string Error = "";

        public static bool ConvertToPDF_Desktop(bool is1UseHtmlUrl0RawCodes, string htmlFileUrlOrHtmlRawCode,
            string pdfFileName)
        {
            return _convertToPDF_Desktop(is1UseHtmlUrl0RawCodes, htmlFileUrlOrHtmlRawCode, pdfFileName,
                true, true, 10, "", "", false, 0, "", "", false, 0, false);
        }

        public static bool GetPdf(bool is1url0html, string urlOrHtml, string pdfFileName, bool isPortrait, bool isA4, int margins)
        {
            return _convertToPDF_Desktop(is1url0html, urlOrHtml, pdfFileName,
                isPortrait, isA4, margins, "", "", false, 0, "", "", false, 0, false);
        }

        public static bool convertToPDF_Desktop(bool is1UseHtmlUrl0RawCodes, string htmlFileUrlOrHtmlRawCode,
            string pdfFileName,
            bool isPasswordChangeAble, string password, bool enablePrinting, bool enableCopyContent)
        {
            return _convertToPDF_Desktop(is1UseHtmlUrl0RawCodes, htmlFileUrlOrHtmlRawCode, pdfFileName, true, true, 10,
                "", "", false, 0, "", "", false, 0, isPasswordChangeAble, password, enablePrinting, enableCopyContent);
        }

        public static bool convertToPDF_Desktop(bool is1UseHtmlUrl0RawCodes, string htmlFileUrlOrHtmlRawCode,
            string pdfFileName,
            bool isPortrait, bool is_1A4_0A3, int margins,
            bool isPasswordChangeAble, string password, bool enablePrinting, bool enableCopyContent)
        {
            return _convertToPDF_Desktop(is1UseHtmlUrl0RawCodes, htmlFileUrlOrHtmlRawCode, pdfFileName, isPortrait,
                is_1A4_0A3, margins,
                "", "", false, 0, "", "", false, 0, isPasswordChangeAble, password, enablePrinting, enableCopyContent);
        }

        public static bool convertToPDF_Desktop(bool is1UseHtmlUrl0RawCodes, string htmlFileUrlOrHtmlRawCode,
            string pdfFileName,
            bool isPortrait, bool is_1A4_0A3, int margins,
            string logoHeaderPath, string headerHtml, bool enableRectangleHeaderBorder, int headerHeight,
            string logoFooterPath, string footerHtml, bool enableRectangleFooterBorder, int footerHeight)
        {
            return _convertToPDF_Desktop(is1UseHtmlUrl0RawCodes, htmlFileUrlOrHtmlRawCode, pdfFileName, isPortrait,
                is_1A4_0A3, margins,
                logoHeaderPath, headerHtml, enableRectangleHeaderBorder, headerHeight,
                logoFooterPath, footerHtml, enableRectangleFooterBorder, footerHeight, false, "", true, false);
        }

        public static bool convertToPDF_Desktop(bool is1UseHtmlUrl0RawCodes, string htmlFileUrlOrHtmlRawCode,
            string pdfFileName,
            bool isPortrait, bool is_1A4_0A3, int margins,
            string logoHeaderPath, string headerHtml, bool enableRectangleHeaderBorder, int headerHeight,
            string logoFooterPath, string footerHtml, bool enableRectangleFooterBorder, int footerHeight,
            bool isPasswordChangeAble, string password, bool enablePrinting, bool enableCopyContent)
        {
            return _convertToPDF_Desktop(is1UseHtmlUrl0RawCodes, htmlFileUrlOrHtmlRawCode, pdfFileName, isPortrait,
                is_1A4_0A3, margins,
                logoHeaderPath, headerHtml, enableRectangleHeaderBorder, headerHeight,
                logoFooterPath, footerHtml, enableRectangleFooterBorder, footerHeight,
                isPasswordChangeAble, password, enablePrinting, enableCopyContent);
        }


        private static bool _convertToPDF_Desktop(bool is1UseHtmlUrl0RawCodes, string htmlFileUrlOrHtmlRawCode,
            string pdfFileName,
            bool is1Portrait0Landscape, bool is_1A4_0A3, int margins,
            string logoHeaderPath, string headerHtml, bool enableRectangleHeaderBorder, int headerHeight,
            string logoFooterPath, string footerHtml, bool enableRectangleFooterBorder, int footerHeight,
            bool isPasswordChangeAble = true, string password = "", bool enablePrinting = true,
            bool enableCopyContent = true)
        {
            var objPdf = new HtmlToPdf();
            objPdf.Document.PageSize = is_1A4_0A3 ? PdfPageSize.A4 : PdfPageSize.A3;
            objPdf.Document.PageOrientation = is1Portrait0Landscape
                ? PdfPageOrientation.Portrait
                : PdfPageOrientation.Landscape;
            objPdf.Document.Margins = new PdfMargins(margins);
            objPdf.Document.PdfStandard = PdfStandard.Pdf;
            objPdf.SerialNumber = LicenseKey;
            if (password.Length > 2)
            {
                if (isPasswordChangeAble) objPdf.Document.Security.PermissionsPassword = password;
                else objPdf.Document.Security.OpenPassword = password;
            }
            objPdf.Document.Security.AllowPrinting = enablePrinting;
            objPdf.Document.Security.AllowCopyContent = enableCopyContent;
            //---------------- setting Header and Footer ---------------------
            var enableHeader = (logoHeaderPath.Length > 4 || headerHtml.Length > 4);
            var enableFooter = (logoFooterPath.Length > 4 || footerHtml.Length > 4);
            if (enableHeader)
                _setHeader(objPdf.Document, logoHeaderPath, headerHtml, enableRectangleHeaderBorder, headerHeight);
            if (enableFooter)
                _setFooter(objPdf.Document, logoFooterPath, footerHtml, enableRectangleFooterBorder, footerHeight);

            try
            {
                if (File.Exists(pdfFileName))
                    File.Delete(pdfFileName);

                if (is1UseHtmlUrl0RawCodes)
                {
                    objPdf.ConvertUrlToFile(htmlFileUrlOrHtmlRawCode, pdfFileName);
                }
                else
                {
                    const string baseUrl = "";
                    objPdf.ConvertHtmlToFile(htmlFileUrlOrHtmlRawCode, baseUrl, pdfFileName);
                }
            }
            catch (Exception ex)
            {
                Error = String.Format("Unable to Convert PDF '{0}', error: {1} ", pdfFileName, ex.Message);
                return false;
            }

            return true;
        }

        public static void OpenPdf(string pdfDirFileName)
        {
            Error = "";
            try
            {
                System.Diagnostics.Process.Start(pdfDirFileName);
            }
            catch (Exception ex)
            {
                Error = String.Format("Conversion Succeeded but cannot '{0}'. {1} ", pdfDirFileName, ex.Message);
            }
        }

        private static void _setHeader(PdfDocumentControl objPdf, string logoHeaderPath, string objHeaderHtml,
            bool enableRectangleHeaderBorder, int headerHeight = 50)
        {
            if (objHeaderHtml == null) throw new ArgumentNullException("objHeaderHtml");
            objPdf.Header.Enabled = true; //  if (!objPDF.Header.Enabled) return;
            objPdf.Header.Height = headerHeight;
            var pdfPageWidth = objPdf.PageOrientation == PdfPageOrientation.Portrait
                ? objPdf.PageSize.Width
                : objPdf.PageSize.Height;
            var headerWidth = pdfPageWidth - objPdf.Margins.Left - objPdf.Margins.Right;
            // float headerHeight = objPDF.Header.Height;
            objPdf.Header.BackgroundColor = Color.WhiteSmoke;
            if (logoHeaderPath.Length > 5)
            {
                var logoHeaderImage = new PdfImage(5, 5, 40, Image.FromFile(logoHeaderPath));
                objPdf.Header.Layout(logoHeaderImage);
            }
            /* @"<span style=""color:Navy; font-family:Times New Roman; font-style:italic"">
                        Quickly Create High Quality PDFs with </span><a href=""http://www.hiqpdf.com"">HiQPdf</a>",*/
            var headerHtml = new PdfHtml(50, 5, objHeaderHtml, null) { FitDestHeight = true };
            objPdf.Header.Layout(headerHtml); //headerHtml.FontEmbedding = true;
            if (!enableRectangleHeaderBorder) return;
            var borderRectangle = new PdfRectangle(1, 1, headerWidth - 2, headerHeight - 2)
            {
                LineStyle = { LineWidth = 0.5f },
                ForeColor = Color.Navy
            };
            objPdf.Header.Layout(borderRectangle);
        }

        private static void _setFooter(PdfDocumentControl objPdf, string logoFooterPath, string objFooterHtml,
            bool enableRectangleFooterBorder, int footerHeight = 50)
        {
            if (objFooterHtml == null) throw new ArgumentNullException("objFooterHtml");
            objPdf.Footer.Enabled = true; //if (!objPDF.Footer.Enabled) return;
            objPdf.Footer.Height = footerHeight;
            objPdf.Footer.BackgroundColor = Color.WhiteSmoke;
            var pdfPageWidth = objPdf.PageOrientation == PdfPageOrientation.Portrait
                ? objPdf.PageSize.Width
                : objPdf.PageSize.Height;
            var footerWidth = pdfPageWidth - objPdf.Margins.Left - objPdf.Margins.Right;

            var footerHtml = new PdfHtml(5, 5, objFooterHtml, null) { FitDestHeight = true };
            objPdf.Footer.Layout(footerHtml);
            //footerHtml.FontEmbedding = enableFontEmbedding;
            var pageNumberingFont = new Font(new FontFamily("Times New Roman"), 8, GraphicsUnit.Point);
            var pageNumberingText = new PdfText(5, footerHeight - 12, "Page {CrtPage} of {PageCount}", pageNumberingFont)
            {
                HorizontalAlign = PdfTextHAlign.Right,
                EmbedSystemFont = true,
                ForeColor = Color.DarkGreen
            };
            objPdf.Footer.Layout(pageNumberingText);
            if (logoFooterPath.Length > 5)
            {
                var logoFooterImage = new PdfImage(footerWidth - 40 - 5, 5, 40, Image.FromFile(logoFooterPath));
                objPdf.Footer.Layout(logoFooterImage);
            }

            if (!enableRectangleFooterBorder) return;

            var borderRectangle = new PdfRectangle(1, 1, footerWidth - 2, footerHeight - 2)
            {
                LineStyle = { LineWidth = 0.5f },
                ForeColor = Color.DarkGreen
            };
            objPdf.Footer.Layout(borderRectangle);
        }
        
        public static void ConvertToPDF_Web(string htmlCode, bool is1Portrait0Landscape, bool is_1A4_0A3, int margin,
            bool isOpenPdfInLine)
        {
            var objPdf = new HtmlToPdf();
            objPdf.Document.PageSize = is_1A4_0A3 ? PdfPageSize.A4 : PdfPageSize.A3;
            objPdf.Document.PageOrientation = is1Portrait0Landscape
                ? PdfPageOrientation.Portrait
                : PdfPageOrientation.Landscape;
            objPdf.Document.Margins = new PdfMargins(margin);
            objPdf.SerialNumber = LicenseKey;

            const string baseUrl = ""; // convert HTML code to a PDF memory buffer
            var pdfBuffer = objPdf.ConvertHtmlToMemory(htmlCode, baseUrl);
            HttpContext.Current.Response.AddHeader("Content-Type", "application/pdf");

            HttpContext.Current.Response.AddHeader("Content-Disposition",
                String.Format("{0}; filename=HtmlToPdf.pdf;size={1}", isOpenPdfInLine ? "inline" : "attachment",
                    pdfBuffer.Length));
            HttpContext.Current.Response.BinaryWrite(pdfBuffer);
            HttpContext.Current.Response.End();
        }

        public static string PageBreak
        {
            get { return "<div style='page-break-after:always; width:100%;'><br /></div>"; }
        }
    }
}
