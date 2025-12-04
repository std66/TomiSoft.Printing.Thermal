using System.Collections.Generic;

namespace TomiSoft.Printing.Thermal.Abstractions.Printer;

public class PrinterFont {
    private readonly IReadOnlyDictionary<int, int> charsPerLine;

    public PrinterFont(string name, byte code, IReadOnlyDictionary<int, int> charsPerLine) {
        Name = name;
        Code = code;
        this.charsPerLine = charsPerLine;
    }

    public string Name { get; }
    public byte Code { get; }

    public int GetCharsPerLine(int fontSize) {
        if (charsPerLine.TryGetValue(fontSize, out int cpl))
            return cpl;

        throw new KeyNotFoundException($"Character per line information is not available for font size '{fontSize}'.");
    }
}
