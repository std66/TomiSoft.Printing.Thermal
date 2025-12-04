using System.Text;
using TomiSoft.Printing.Thermal.Abstractions.Printer;

namespace TomiSoft.Printing.Thermal.Printer;

public class CodePageEncoder : IStringEncoder {
    private readonly Encoding _encoding;
    private static bool IsRegistered = false;
    private static readonly object LockObject = new object();

    public CodePageEncoder(string codePage) {
        EnsureProviderRegistered();
        _encoding = Encoding.GetEncoding(codePage);
    }

    public CodePageEncoder(int codePage) {
        EnsureProviderRegistered();
        _encoding = Encoding.GetEncoding(codePage);
    }

    private static void EnsureProviderRegistered() {
        if (IsRegistered)
            return;

        lock (LockObject) {
            if (IsRegistered)
                return;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IsRegistered = true;
        }
    }

    public byte[] Encode(string text) {
        return _encoding.GetBytes(text);
    }
}
