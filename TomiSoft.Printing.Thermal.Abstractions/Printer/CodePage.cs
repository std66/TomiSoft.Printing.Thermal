using System.Diagnostics;

namespace TomiSoft.Printing.Thermal.Abstractions.Printer;

[DebuggerDisplay("{DebugDisplay}")]
public class CodePage {
    public CodePage(string name, string description, byte code, IStringEncoder encoder) {
        Name = name;
        Description = description;
        Code = code;
        Encoder = encoder;
    }

    public string Name { get; }
    public string Description { get; }
    public byte Code { get; }
    public IStringEncoder Encoder { get; }

    private string DebugDisplay => $"{nameof(CodePage)}: {Name} ({Code})";
}
