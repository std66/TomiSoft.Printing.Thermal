namespace TomiSoft.Printing.Thermal.EscPos.Commands;

public sealed class DleEotStatusRequestCommand : EscPosCommand {
    public byte StatusType { get; }

    public DleEotStatusRequestCommand(byte statusType)
        : base([0x10, 0x04, statusType], $"[DLE EOT {statusType}] Query printer status") {
        StatusType = statusType;
    }
}