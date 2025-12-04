using System.Text;
using TomiSoft.Printing.Thermal.Abstractions.Printer;

namespace TomiSoft.Printing.Thermal.Printer;

public class AsciiEncoder : IStringEncoder {
    public byte[] Encode(string text) {
        return Encoding.ASCII.GetBytes(text);
    }
}
