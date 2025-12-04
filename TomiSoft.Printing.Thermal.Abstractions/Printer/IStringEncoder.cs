namespace TomiSoft.Printing.Thermal.Abstractions.Printer;

public interface IStringEncoder {
    byte[] Encode(string text);
}
