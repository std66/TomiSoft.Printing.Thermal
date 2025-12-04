using TomiSoft.Printing.Thermal.Abstractions.Printer;

namespace TomiSoft.Printing.Thermal.Printer.Netum;

public class NT_1809DD : Abstractions.Printer.Printer {
    public override IReadOnlyList<CodePage> SupportedCodePages { get; } = [
        // ---------- 0–32 és 255 ----------
    new CodePage("OEM437", "Std. Europe", 0, new CodePageEncoder("ibm437")),
    new CodePage("Katakana", "", 1, new CodePageEncoder("shift_jis")), // Legközelebbi megfelelés
    new CodePage("OEM850", "Multilingual", 2, new CodePageEncoder("ibm850")),
    new CodePage("OEM860", "Portuguese", 3, new CodePageEncoder("ibm860")),
    new CodePage("OEM863", "Canadian", 4, new CodePageEncoder("ibm863")),
    new CodePage("OEM865", "Nordic", 5, new CodePageEncoder("ibm865")),
    new CodePage("West Europe", "", 6, new CodePageEncoder("windows-1252")),
    new CodePage("Greek", "", 7, new CodePageEncoder("windows-1253")),
    new CodePage("Hebrew", "", 8, new CodePageEncoder("windows-1255")),
    new CodePage("East Europe", "", 9, new CodePageEncoder("windows-1250")),
    new CodePage("Iran", "", 10, new CodePageEncoder("iso-8859-6")), // approximated
    new CodePage("WPC1252", "", 16, new CodePageEncoder("windows-1252")),
    new CodePage("OEM866", "Cyrillic#2", 17, new CodePageEncoder("ibm866")),
    new CodePage("OEM852", "Latin-II", 18, new CodePageEncoder("ibm852")),
    new CodePage("OEM858", "", 19, new UnsupportedEncoder()),
    new CodePage("IranII", "", 20, new CodePageEncoder("iso-8859-6")),
    new CodePage("Latvian", "", 21, new CodePageEncoder("windows-1257")),
    new CodePage("ISO-8859-6", "Arabic", 22, new CodePageEncoder("iso-8859-6")),
    new CodePage("PT151,1251", "", 23, new CodePageEncoder("windows-1251")),
    new CodePage("OEM747", "", 24, new CodePageEncoder("ibm737")),
    new CodePage("WPC1257", "", 25, new CodePageEncoder("windows-1257")),
    new CodePage("Vietnam", "", 27, new CodePageEncoder("windows-1258")),
    new CodePage("OEM864", "", 28, new CodePageEncoder("ibm864")),
    new CodePage("Hebrew", "", 31, new CodePageEncoder("windows-1255")),
    new CodePage("WPC1255", "Israel", 32, new CodePageEncoder("windows-1255")),
    new CodePage("Thai", "", 255, new CodePageEncoder("windows-874")),

    // ---------- 50–96 ----------
    new CodePage("OEM437", "Std. Europe", 50, new CodePageEncoder("ibm437")),
    new CodePage("OEM437", "Std. Europe", 52, new CodePageEncoder("ibm437")),
    new CodePage("OEM858", "Multilingual", 53, new UnsupportedEncoder()),
    new CodePage("OEM852", "Latin-2", 54, new CodePageEncoder("ibm852")),
    new CodePage("OEM860", "Portuguese", 55, new CodePageEncoder("ibm860")),
    new CodePage("OEM861", "Icelandic", 56, new CodePageEncoder("ibm861")),
    new CodePage("OEM863", "Canadian", 57, new CodePageEncoder("ibm863")),
    new CodePage("OEM865", "Nordic", 58, new CodePageEncoder("ibm865")),
    new CodePage("OEM866", "Russian", 59, new CodePageEncoder("ibm866")),
    new CodePage("OEM855", "Bulgarian", 60, new CodePageEncoder("ibm855")),
    new CodePage("OEM857", "Turkey", 61, new CodePageEncoder("ibm857")),
    new CodePage("OEM862", "Hebrew", 62, new CodePageEncoder("ibm862")),
    new CodePage("OEM864", "Arabic", 63, new CodePageEncoder("ibm864")),
    new CodePage("OEM737", "Greek", 64, new CodePageEncoder("ibm737")),
    new CodePage("OEM851", "Greek", 65, new UnsupportedEncoder()),
    new CodePage("OEM869", "Greek", 66, new CodePageEncoder("ibm869")),
    new CodePage("OEM772", "Lithuanian", 68, new CodePageEncoder("ibm775")), // nincs 772, legközelebbi 775
    new CodePage("OEM774", "Lithuanian", 69, new CodePageEncoder("ibm775")), // approximated
    new CodePage("WPC1252", "Latin–1", 71, new CodePageEncoder("windows-1252")),
    new CodePage("WPC1250", "Latin–2", 72, new CodePageEncoder("windows-1250")),
    new CodePage("WPC1251", "Cyrillic", 73, new CodePageEncoder("windows-1251")),

    // --------- SPECIAL, NOT SUPPORTED IN .NET → ASCII FALLBACK ---------
    new CodePage("PC3840", "IBM–Russian", 74, new AsciiEncoder()),
    new CodePage("PC3843", "Polish", 76, new AsciiEncoder()),
    new CodePage("PC3844", "CS2", 77, new AsciiEncoder()),
    new CodePage("PC3845", "Hungarian", 78, new AsciiEncoder()),
    new CodePage("PC3846", "Turkish", 79, new AsciiEncoder()),
    new CodePage("PC3847", "Brazil–ABNT", 80, new AsciiEncoder()),
    new CodePage("PC3848", "Brazil–ABICOMP", 81, new AsciiEncoder()),
    new CodePage("PC2001", "Lithuanian–KBL", 83, new AsciiEncoder()),
    new CodePage("PC3001", "Estonian–1", 84, new AsciiEncoder()),
    new CodePage("PC3002", "Estonian–2", 85, new AsciiEncoder()),
    new CodePage("PC3011", "Latvian–1", 86, new AsciiEncoder()),
    new CodePage("PC3021", "Latvian–2", 87, new AsciiEncoder()),
    new CodePage("PC3002", "Bulgarian", 88, new AsciiEncoder()),
    new CodePage("PC3041", "Maltese", 89, new AsciiEncoder()),

    // --------- Back to supported encodings ---------
    new CodePage("WPC1253", "Greek", 90, new CodePageEncoder("windows-1253")),
    new CodePage("WPC1254", "Turkish", 91, new CodePageEncoder("windows-1254")),
    new CodePage("WPC1256", "Arabic", 92, new CodePageEncoder("windows-1256")),
    new CodePage("OEM720", "Arabic", 93, new UnsupportedEncoder()),
    new CodePage("WPC1258", "Vietnam", 94, new CodePageEncoder("windows-1258")),
    new CodePage("OEM775", "Latvian", 95, new CodePageEncoder("ibm775")),
    new CodePage("Thai", "", 96, new CodePageEncoder("windows-874"))
    ];

    public override string Manufacturer => "NETUM";

    public override string Model => "NT-1809DD";

    public override int PaperWidthMM => 57;

    public override IReadOnlyList<PrinterFont> SupportedFonts { get; } = [
        new PrinterFont("Font A", 0, new Dictionary<int, int>() { [1] = 32, [2] = 16, [3] = 10, [4] = 8, [5] = 32, [6] = 32, [7] = 32 }),
        new PrinterFont("Font B", 1, new Dictionary<int, int>() { [1] = 42, [2] = 21, [3] = 14, [4] = 10, [5] = 42, [6] = 42, [7] = 42 }),
        new PrinterFont("Font C", 2, new Dictionary<int, int>() { [1] = 42, [2] = 21, [3] = 14, [4] = 10, [5] = 42, [6] = 42, [7] = 42 }),
    ];
}
