using System.ComponentModel;

namespace TomiSoft.Printing.Thermal.EscPos.Commands;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
public class EscPosCommand {
    public EscPosCommand(byte[] bytes, string description) {
        Bytes = bytes;
        Description = description;
    }

    public byte[] Bytes { get; }
    public string Description { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    private string DebuggerDisplay => $"{nameof(EscPosCommand)} ({Bytes.Length}B): {Description}";
}
