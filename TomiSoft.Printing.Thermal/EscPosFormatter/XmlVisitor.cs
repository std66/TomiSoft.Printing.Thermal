using ESCPOS;
using ESCPOS.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace TomiSoft.Printing.Thermal.EscPosFormatter {
    public class XmlVisitor : IXmlVisitor {
        private readonly List<byte> result = new List<byte>();

        private readonly Stack<Justification> currentJustification = new Stack<Justification>();

        private int? paperWidthMM;
        private int? lineWidthChars;
        private int? lineFeedAtEnd;

        public void VisitDocumentBegin(int? paperWidthMM, int? lineWidthChars, int? lineFeedAtEnd) {
            this.paperWidthMM = paperWidthMM;
            this.lineWidthChars = lineWidthChars;
            this.lineFeedAtEnd = lineFeedAtEnd;

            result.AddRange(ESCPOS.Commands.InitializePrinter);
            currentJustification.Push(Justification.Left);
        }

        public void VisitDocumentEnd() {
            for (int i = 0; i < (lineFeedAtEnd ?? 0); i++)
                result.AddRange(ESCPOS.Commands.LineFeed);
        }

        public void VisitParagraphBegin() {
        }

        public void VisitParagraphEnd() {
            result.AddRange(ESCPOS.Commands.LineFeed);
            result.AddRange(ESCPOS.Commands.LineFeed);
        }

        public void VisitAlignmentBegin(Alignment align) {
            Justification j = align switch {
                Alignment.Left => Justification.Left,
                Alignment.Center => Justification.Center,
                Alignment.Right => Justification.Right,
                _ => Justification.Left
            };

            currentJustification.Push(j);

            result.AddRange(ESCPOS.Commands.SelectJustification(j));
        }

        public void VisitAlignmentEnd() {
            result.AddRange(ESCPOS.Commands.SelectJustification(
                currentJustification.Pop()
            ));
        }

        public void VisitText(string text) {
            result.AddRange(text.ToBytes());
            result.AddRange(ESCPOS.Commands.LineFeed);
        }

        public void VisitHeading(string text, int level) {
            result.AddRange(level switch {
                1 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Normal, ESCPOS.CharSizeHeight.Normal),
                2 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Double, ESCPOS.CharSizeHeight.Double),
                3 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Triple, ESCPOS.CharSizeHeight.Triple),
                4 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Quadruple, ESCPOS.CharSizeHeight.Quadruple),
                5 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Quintuple, ESCPOS.CharSizeHeight.Quintuple),
                6 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Sextuple, ESCPOS.CharSizeHeight.Sextuple),
                7 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Septuple, ESCPOS.CharSizeHeight.Septuple),
                8 => ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Octuple, ESCPOS.CharSizeHeight.Octuple),
                _ => Array.Empty<byte>(),
            });

            result.AddRange(text.ToBytes());
            result.AddRange(ESCPOS.Commands.LineFeed);

            result.AddRange(ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Normal, ESCPOS.CharSizeHeight.Normal));
        }

        public void VisitQr(string text, string size, bool asImage) {
            if (asImage) {
                result.AddRange(ZxingTools.GetQRCodeImage(text, Convert.ToInt32(size)));
            }
            else {
                QRCodeSize s = size switch {
                    "tiny" => QRCodeSize.Tiny,
                    "small" => QRCodeSize.Small,
                    "normal" => QRCodeSize.Normal,
                    "large" => QRCodeSize.Large,
                    _ => QRCodeSize.Normal
                };

                result.AddRange(ESCPOS.Commands.PrintQRCode(text, qrCodeSize: s));
            }
        }

        public void VisitTable(IEnumerable<string[]> lines) {
            foreach (string[] line in lines) {
                //for 2 columns only
                string left = line[0];
                string right = line[1];

                int spaces = lineWidthChars == null ? 1 : lineWidthChars.Value - (left.Length + right.Length);

                StringBuilder sb = new StringBuilder();
                sb.Append(left);
                sb.Append(' ', spaces);
                sb.Append(right);

                result.AddRange(sb.ToString().ToBytes());
                result.AddRange(ESCPOS.Commands.LineFeed);
            }
        }

        public void VisitBarcode(BarcodeKind kind, bool asImage, string value) {
            if (asImage) {
                result.AddRange(ZxingTools.GetBarcodeImage(kind, value, 300));
            }
            else {
                BarCodeType type = kind switch {
                    BarcodeKind.UPC_A => BarCodeType.UPC_A,
                    BarcodeKind.UPC_E => BarCodeType.UPC_E,
                    BarcodeKind.EAN13 => BarCodeType.EAN13,
                    BarcodeKind.EAN8 => BarCodeType.EAN8,
                    BarcodeKind.CODE39 => BarCodeType.CODE39,
                    BarcodeKind.ITF => BarCodeType.ITF,
                    BarcodeKind.CODABAR => BarCodeType.CODABAR,
                    BarcodeKind.CODE93 => BarCodeType.CODE93,
                    BarcodeKind.CODE128 => BarCodeType.CODE128,
                    _ => throw new ArgumentException("Invalid barcode kind was provided", nameof(kind))
                };

                result.AddRange(ESCPOS.Commands.PrintBarCode(type, value, barcodeWidth: BarcodeWidth.Thick));
            }
        }

        public byte[] Result => result.ToArray();
    }
}
