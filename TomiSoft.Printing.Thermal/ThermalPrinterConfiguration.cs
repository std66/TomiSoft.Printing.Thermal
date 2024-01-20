namespace TomiSoft.Printing.Thermal {
    public class ThermalPrinterConfiguration {
        public ThermalPrinterConfiguration(int maxLineLength) {
            MaxLineLength = maxLineLength;
        }

        public int MaxLineLength { get; }
    }
}
