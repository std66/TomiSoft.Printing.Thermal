namespace TomiSoft.Printing.Thermal.Abstractions.Printer;

public interface IPrinter {
    IReadOnlyList<CodePage> SupportedCodePages { get; }
    IReadOnlyList<PrinterFont> SupportedFonts { get; }

    string Manufacturer { get; }
    string Model { get; }
    int PaperWidthMM { get; }
    int DotsPerLine { get; }

    CodePage GetCodePage(string name);
    PrinterFont GetFont(string name);
}
