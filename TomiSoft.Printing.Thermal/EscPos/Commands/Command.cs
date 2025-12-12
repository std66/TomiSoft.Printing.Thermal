namespace TomiSoft.Printing.Thermal.EscPos.Commands;

public static class Command {
    public static readonly EscPosCommand InitializePrinter = new EscPosCommand([0x1B, 0x40], "[ESC @] Initialize Printer");
}
