namespace TomiSoft.Printing.Thermal.Abstractions.Printer {
    public class UnsupportedEncoder : IStringEncoder {
        public byte[] Encode(string text) {
            throw new NotImplementedException($"{nameof(IStringEncoder)} for the selected code page is not implemented");
        }
    }
}
