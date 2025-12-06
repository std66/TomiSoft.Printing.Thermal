namespace TomiSoft.Printing.Thermal.Abstractions.Imaging {
    public interface IImageProvider {
        byte[] GetImage(byte[] imageData, string mimeType, int width, int height, IReadOnlyDictionary<string, string> vendorOptions);
    }
}
