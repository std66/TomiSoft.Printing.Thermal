using System.Collections.Generic;

namespace TomiSoft.Printing.Thermal.Abstractions.EscPosFormatter {
    public interface IEscPosXmlVisitor {
        byte[] Result { get; }

        void VisitAlignmentBegin(Alignment align);
        void VisitAlignmentEnd();
        void VisitBarcode(BarcodeKind kind, bool asImage, string value, IReadOnlyDictionary<string, string> vendorAttributes);
        void VisitDocumentBegin(string codePage, string font);
        void VisitDocumentEnd(int lineFeed);
        void VisitHeading(string text, int level);
        void VisitImage(byte[] imageBytes, string mimeType, int width, int height, IReadOnlyDictionary<string, string> vendorAttributes);
        void VisitParagraphBegin();
        void VisitParagraphEnd();
        void VisitQr(string text, string size, bool asImage, IReadOnlyDictionary<string, string> vendorAttributes);
        void VisitTable(IEnumerable<string[]> lines);
        void VisitText(string text);
    }
}