using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace TomiSoft.Printing.Thermal.EscPosFormatter {
    public class XmlPosFormatter {
        private readonly string[] terminalNodes = { "t", "heading", "qr", "table", "barcode" };

        public void Format(Stream xmlStream, IXmlVisitor visitor) {
            XmlDocument xmlDocument = new XmlDocument();

            try {
                xmlDocument.Load(xmlStream);
            }
            catch (XmlException ex) {
                throw;
            }

            Accept(xmlDocument.DocumentElement, visitor);
        }

        private void Accept(XmlElement? documentElement, IXmlVisitor visitor) {
            if (documentElement == null)
                return;

            AcceptComplexElement(documentElement, visitor);
        }

        private void AcceptComplexElement(XmlNode node, IXmlVisitor visitor) {
            AcceptBeginComplexNode(node, visitor);

            foreach (XmlNode childNode in node.ChildNodes) {
                if (terminalNodes.Contains(childNode.Name)) {
                    AcceptTerminalNode(childNode, visitor);
                }
                else {
                    AcceptComplexElement(childNode, visitor);
                }
            }

            AcceptEndComplexNode(node, visitor);
        }

        private static void AcceptEndComplexNode(XmlNode node, IXmlVisitor visitor) {
            switch (node.Name) {
                case "escpos":
                    visitor.VisitDocumentEnd();
                    break;

                case "para":
                    visitor.VisitParagraphEnd();
                    break;

                case "align":
                    visitor.VisitAlignmentEnd();
                    break;
            }
        }

        private static void AcceptBeginComplexNode(XmlNode node, IXmlVisitor visitor) {
            switch (node.Name) {
                case "escpos":
                    XmlNode? paperWidthMM = node.Attributes?.GetNamedItem("paperWidthMM");
                    XmlNode? lineWidthChars = node.Attributes?.GetNamedItem("lineWidthChars");
                    XmlNode? lineFeedAtEnd = node.Attributes?.GetNamedItem("lineFeedAtEnd");

                    visitor.VisitDocumentBegin(
                        paperWidthMM == null ? null : Convert.ToInt32(paperWidthMM.Value),
                        lineWidthChars == null ? null : Convert.ToInt32(lineWidthChars.Value),
                        lineFeedAtEnd == null ? null : Convert.ToInt32(lineFeedAtEnd.Value)
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

        private void AcceptTerminalNode(XmlNode node, IXmlVisitor visitor) {
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

        private void AcceptBarcode(IXmlVisitor visitor, string encoding, bool barcodeAsImage, string value) {
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

        private void AcceptTable(XmlNode node, IXmlVisitor visitor) {
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
