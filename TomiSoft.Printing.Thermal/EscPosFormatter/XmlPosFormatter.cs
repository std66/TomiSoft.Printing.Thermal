using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TomiSoft.Printing.Thermal.Abstractions.EscPosFormatter;

namespace TomiSoft.Printing.Thermal.EscPosFormatter {
    public class XmlPosFormatter {
        private readonly string[] terminalNodes = { "t", "heading", "qr", "table", "barcode" };

        public void Format(Stream xmlStream, IEscPosXmlVisitor visitor, Action<FormatterOptions> options) {
            XmlDocument xmlDocument = new XmlDocument();

            try {
                xmlDocument.Load(xmlStream);
            }
            catch (XmlException ex) {
                throw;
            }

            FormatterOptions formatterOptions = new FormatterOptions();
            options(formatterOptions);

            Accept(xmlDocument.DocumentElement, visitor, formatterOptions);
        }

        private void Accept(XmlElement? documentElement, IEscPosXmlVisitor visitor, FormatterOptions formatterOptions) {
            if (documentElement == null)
                return;

            AcceptComplexElement(documentElement, visitor, formatterOptions);
        }

        private void AcceptComplexElement(XmlNode node, IEscPosXmlVisitor visitor, FormatterOptions formatterOptions) {
            AcceptBeginComplexNode(node, visitor, formatterOptions);

            foreach (XmlNode childNode in node.ChildNodes) {
                if (terminalNodes.Contains(childNode.Name)) {
                    AcceptTerminalNode(childNode, visitor, formatterOptions);
                }
                else {
                    AcceptComplexElement(childNode, visitor, formatterOptions);
                }
            }

            AcceptEndComplexNode(node, visitor, formatterOptions);
        }

        private static void AcceptEndComplexNode(XmlNode node, IEscPosXmlVisitor visitor, FormatterOptions formatterOptions) {
            switch (node.Name) {
                case "escpos":
                    visitor.VisitDocumentEnd(formatterOptions.LineFeedAtEnd);
                    break;

                case "para":
                    visitor.VisitParagraphEnd();
                    break;

                case "align":
                    visitor.VisitAlignmentEnd();
                    break;
            }
        }

        private static void AcceptBeginComplexNode(XmlNode node, IEscPosXmlVisitor visitor, FormatterOptions formatterOptions) {
            switch (node.Name) {
                case "escpos":
                    visitor.VisitDocumentBegin(
                        formatterOptions.CodePage,
                        formatterOptions.DefaultFont
                    );
                    break;

                case "para":
                    visitor.VisitParagraphBegin();
                    break;

                case "align":
                    XmlNode toAttr = node.Attributes?.GetNamedItem("to") ?? throw new Exception("Attribute 'to' is required but not found");
                    visitor.VisitAlignmentBegin(
                        toAttr.Value switch {
                            "left" => Alignment.Left,
                            "right" => Alignment.Right,
                            "center" => Alignment.Center,
                            _ => throw new Exception($"Value '{toAttr.InnerText}' is not a valid value for 'to' attribute of 'align' element.")
                        }
                    );
                    break;
            }
        }

        private void AcceptTerminalNode(XmlNode node, IEscPosXmlVisitor visitor, FormatterOptions formatterOptions) {
            switch (node.Name) {
                case "t":
                    visitor.VisitText(node.InnerText);
                    break;

                case "heading":
                    XmlNode levelAttr = node.Attributes?.GetNamedItem("level") ?? throw new Exception("Attribute 'level' is required but not found");
                    visitor.VisitHeading(node.InnerText, Convert.ToInt32(levelAttr.InnerText));
                    break;

                case "qr":
                    XmlNode sizeAttr = node.Attributes?.GetNamedItem("size") ?? throw new Exception("Attribute 'size' is required but not found");
                    bool asImage = Convert.ToBoolean(node.Attributes?.GetNamedItem("asimage")?.InnerText);
                    visitor.VisitQr(node.InnerText, sizeAttr.InnerText, asImage);
                    break;

                case "table":
                    AcceptTable(node, visitor);
                    break;

                case "barcode":
                    XmlNode encodingAttr = node.Attributes?.GetNamedItem("encoding") ?? throw new Exception("Attribute 'encoding' is required but not found");
                    bool barcodeAsImage = Convert.ToBoolean(node.Attributes?.GetNamedItem("asimage")?.InnerText);
                    AcceptBarcode(visitor, encodingAttr.InnerText, barcodeAsImage, node.InnerText);
                    break;
            }
        }

        private void AcceptBarcode(IEscPosXmlVisitor visitor, string encoding, bool barcodeAsImage, string value) {
            BarcodeKind kind = encoding switch {
                "UPC-A" => BarcodeKind.UPC_A,
                "UPC-E" => BarcodeKind.UPC_E,
                "EAN-8" => BarcodeKind.EAN8,
                "EAN-13" => BarcodeKind.EAN13,
                "Code39" => BarcodeKind.CODE39,
                "Code93" => BarcodeKind.CODE93,
                "Code128" => BarcodeKind.CODE128,
                "ITF" => BarcodeKind.ITF,
                "Codabar" => BarcodeKind.CODABAR,
                _ => throw new Exception($"Attribute 'encoding' has invalid value '{encoding}'.")
            };

            visitor.VisitBarcode(kind, barcodeAsImage, value);
        }

        private void AcceptTable(XmlNode node, IEscPosXmlVisitor visitor) {
            List<string[]> lines = new List<string[]>();

            foreach (XmlNode row in node.SelectNodes("row")) {
                XmlNodeList columns = row.SelectNodes("column");
                string[] data = new string[columns.Count];
                for (int i = 0; i < columns.Count; i++) {
                    data[i] = columns[i].InnerText;
                }
                lines.Add(data);
            }

            visitor.VisitTable(lines);
        }
    }
}
