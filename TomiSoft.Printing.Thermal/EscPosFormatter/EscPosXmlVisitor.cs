using ESCPOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomiSoft.Printing.Thermal.Abstractions.EscPosFormatter;
using TomiSoft.Printing.Thermal.Abstractions.Imaging;
using TomiSoft.Printing.Thermal.Abstractions.Printer;
using TomiSoft.Printing.Thermal.Printer;

namespace TomiSoft.Printing.Thermal.EscPosFormatter {
    public class EscPosXmlVisitor : IEscPosXmlVisitor {
        private readonly List<byte> result = new List<byte>();

        private readonly Stack<Justification> currentJustification = new Stack<Justification>();
        private readonly IPrinter printer;

        private IStringEncoder encoder;
        private PrinterFont font;
        private int currentFontSize = 1;
        private bool isFirstParagraph = true;

        public IBarcodeImageProvider BarcodeImageProvider { get; set; }
        public IImageProvider ImageProvider { get; set; }

        public EscPosXmlVisitor(IPrinter printer) {
            this.printer = printer;
        }

        public void VisitDocumentBegin(string codePage, string font) {
            CodePage printerCodePage = codePage != null ? printer.GetCodePage(codePage) : new CodePage("ascii", "", 0, new AsciiEncoder());

            this.font = font == null ? printer.SupportedFonts.First() : printer.GetFont(font);
            this.encoder = printerCodePage.Encoder;

            result.AddRange(ESCPOS.Commands.InitializePrinter);
            result.AddRange(ESCPOS.Commands.SelectCodeTable((CodeTable)printerCodePage.Code));
            result.AddRange(ESCPOS.Commands.SelectCharacterFont((Font)this.font.Code));
            currentJustification.Push(Justification.Left);
        }

        public void VisitDocumentEnd(int lineFeed) {
            for (int i = 0; i < (lineFeed); i++)
                result.AddRange(ESCPOS.Commands.LineFeed);
        }

        public void VisitParagraphBegin() {
            if (!isFirstParagraph) {
                result.AddRange(ESCPOS.Commands.LineFeed);
            }
        }

        public void VisitParagraphEnd() {
            isFirstParagraph = false;
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
            result.AddRange(tw(text));
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

            int oldFontSize = currentFontSize;
            currentFontSize = level;

            result.AddRange(tw(text));
            result.AddRange(ESCPOS.Commands.LineFeed);

            currentFontSize = oldFontSize;

            result.AddRange(ESCPOS.Commands.SelectCharSize(ESCPOS.CharSizeWidth.Normal, ESCPOS.CharSizeHeight.Normal));
        }

        public void VisitQr(string text, string size, bool asImage, IReadOnlyDictionary<string, string> vendorAttributes) {
            if (asImage) {
                if (BarcodeImageProvider == null) {
                    throw new InvalidOperationException($"{nameof(BarcodeImageProvider)} is not set, cannot render QR code as image.");
                }

                result.AddRange(BarcodeImageProvider.GetQrCodeImage(text, Convert.ToInt32(size), vendorAttributes));
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
            int? lineWidthChars = font.GetCharsPerLine(1);

            foreach (string[] line in lines) {
                if (line == null || line.Length < 2) {
                    // skip or handle rows that aren't two columns
                    continue;
                }

                string left = line[0] ?? string.Empty;
                string right = line[1] ?? string.Empty;

                int spaces;
                if (lineWidthChars == null) {
                    spaces = 1;
                }
                else {
                    spaces = Math.Max(1, lineWidthChars.Value - (left.Length + right.Length));
                }

                var sb = new StringBuilder();
                sb.Append(left);
                sb.Append(' ', spaces);
                sb.Append(right);

                result.AddRange(t(sb.ToString()));
                result.AddRange(ESCPOS.Commands.LineFeed);
            }
        }

        public void VisitBarcode(BarcodeKind kind, bool asImage, string value, IReadOnlyDictionary<string, string> vendorAttributes) {
            if (asImage) {
                if (BarcodeImageProvider == null) {
                    throw new InvalidOperationException($"{nameof(BarcodeImageProvider)} is not set, cannot render QR code as image.");
                }

                result.AddRange(BarcodeImageProvider.GetBarcodeImage(kind, value, 300, vendorAttributes));
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

        private byte[] t(string text) => encoder.Encode(text);

        private byte[] tw(string text) {
            var lines = WordWrap(text.Replace(Environment.NewLine, ""), font.GetCharsPerLine(currentFontSize)).ToList();

            if (lines.Count == 1) {
                return encoder.Encode(text);
            }

            var bytes = new List<byte>();

            for (int i = 0; i < lines.Count; i++) {
                bytes.AddRange(encoder.Encode(lines[i]));

                // Append line feed between lines, but not after the last line
                if (i < lines.Count - 1) {
                    bytes.AddRange(ESCPOS.Commands.LineFeed);
                }
            }

            return bytes.ToArray();
        }

        public byte[] Result => result.ToArray();

        public static IEnumerable<string> WordWrap(string text, int maxWidth) {
            if (string.IsNullOrEmpty(text))
                yield break;

            var words = text.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            var line = new StringBuilder();

            foreach (var word in words) {
                // Ha túl hosszú a szó, akkor törni kell
                if (word.Length > maxWidth) {
                    // Ha a jelenlegi sor nem üres, előbb kiadjuk
                    if (line.Length > 0) {
                        yield return line.ToString();
                        line.Clear();
                    }

                    // Tördeli a túl hosszú szót
                    int index = 0;
                    while (index < word.Length) {
                        int take = Math.Min(maxWidth, word.Length - index);
                        yield return word.Substring(index, take);
                        index += take;
                    }

                    continue;
                }

                // Ha a szó befér a jelenlegi sorba
                if (line.Length == 0) {
                    line.Append(word);
                }
                else if (line.Length + 1 + word.Length <= maxWidth) {
                    line.Append(' ');
                    line.Append(word);
                }
                else {
                    // Sor vége → visszaad, új sort kezd
                    yield return line.ToString();
                    line.Clear();
                    line.Append(word);
                }
            }

            if (line.Length > 0)
                yield return line.ToString();
        }

    }
}
