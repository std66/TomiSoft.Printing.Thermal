using System.Collections.Generic;

namespace TomiSoft.Printing.Thermal.EscPosFormatter {
    public interface IXmlVisitor {
        byte[] Result { get; }

        void VisitAlignmentBegin(Alignment align);
        void VisitAlignmentEnd();
        void VisitBarcode(BarcodeKind kind, bool asImage, string value);
        void VisitDocumentBegin(int? paperWidthMM, int? lineWidthChars, int? lineFeedAtEnd);
        void VisitDocumentEnd();
        void VisitHeading(string text, int level);
        void VisitParagraphBegin();
        void VisitParagraphEnd();
        void VisitQr(string text, string size, bool asImage);
        void VisitTable(IEnumerable<string[]> lines);
        void VisitText(string text);
    }
}