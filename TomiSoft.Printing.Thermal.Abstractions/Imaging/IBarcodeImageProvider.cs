using TomiSoft.Printing.Thermal.Abstractions.EscPosFormatter;

namespace TomiSoft.Printing.Thermal.Abstractions.Imaging {
    public interface IBarcodeImageProvider {
        byte[] GetQrCodeImage(string textContent, int size);
        byte[] GetBarcodeImage(BarcodeKind kind, string content, int size);
    }
}
