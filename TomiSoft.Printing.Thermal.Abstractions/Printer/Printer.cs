namespace TomiSoft.Printing.Thermal.Abstractions.Printer;

public abstract class Printer : IPrinter {
    public abstract IReadOnlyList<CodePage> SupportedCodePages { get; }
    public abstract IReadOnlyList<PrinterFont> SupportedFonts { get; }
    public abstract string Manufacturer { get; }
    public abstract string Model { get; }
    public abstract int PaperWidthMM { get; }
    public abstract int DotsPerLine { get; }

    public virtual CodePage GetCodePage(string name) {
        return SupportedCodePages
            .FirstOrDefault(cp => cp.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new NotSupportedException($"Requested code page '{name}' is not supported by the printer '{Manufacturer} {Model}'.");
    }

    public virtual PrinterFont GetFont(string name) {
        return SupportedFonts
            .FirstOrDefault(cp => cp.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new NotSupportedException($"Requested font '{name}' is not supported by the printer '{Manufacturer} {Model}'.");
    }
}
