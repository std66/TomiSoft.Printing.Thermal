namespace TomiSoft.Printing.Thermal.Printer;

internal sealed class InternalPrinterStatus {
    public byte Raw { get; }

    public InternalPrinterStatus(byte raw) {
        Raw = raw;
    }

    public bool PaperOut => (Raw & 0b0000_0100) != 0;
    public bool CoverOpen => (Raw & 0b0010_0000) != 0;
    public bool Error => (Raw & 0b0100_0000) != 0;
}
