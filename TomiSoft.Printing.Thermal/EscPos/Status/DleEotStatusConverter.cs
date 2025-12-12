using System;

namespace TomiSoft.Printing.Thermal.EscPos.Status;

public class DleEotStatusConverter {
    public static DleEotStatus Parse(byte n, byte raw) {
        DleEotStatus flags = DleEotStatus.None;

        bool Bit(int b) => (raw & (1 << b)) != 0;

        switch (n) {
            case 1: // Printer status
                if (Bit(2))
                    flags |= DleEotStatus.CashDrawerKickOut;
                if (Bit(3))
                    flags |= DleEotStatus.Offline;
                break;

            case 2: // Offline status
                if (Bit(2))
                    flags |= DleEotStatus.CoverOpen;
                if (Bit(3))
                    flags |= DleEotStatus.PaperFeedButtonPressed;
                if (Bit(5))
                    flags |= DleEotStatus.Offline;
                break;

            case 3: // Error status
                if (Bit(3))
                    flags |= DleEotStatus.CutterError;
                if (Bit(5))
                    flags |= DleEotStatus.RecoverableError | DleEotStatus.Error;
                if (Bit(6))
                    flags |= DleEotStatus.UnrecoverableError | DleEotStatus.Error;
                break;

            case 4: // Paper roll status
                if (Bit(2))
                    flags |= DleEotStatus.PaperNearEnd;
                if (Bit(3))
                    flags |= DleEotStatus.PaperEnd;
                break;

            case 7: // Consumable / ribbon
                if (Bit(2))
                    flags |= DleEotStatus.ConsumableNearEnd;
                if (Bit(3))
                    flags |= DleEotStatus.ConsumableEnd;
                break;

            case 8: // Extended printer status
                if (Bit(0))
                    flags |= DleEotStatus.WaitingForOnline;
                if (Bit(5))
                    flags |= DleEotStatus.PanelSwitchOn;
                break;

            case 18: // Firmware / internal (vendor specific)
                if (Bit(0))
                    flags |= DleEotStatus.FirmwareUpdating;
                if (Bit(7))
                    flags |= DleEotStatus.FirmwareBusy;
                break;

            default:
                throw new NotSupportedException($"Unsupported DLE EOT n={n}");
        }

        return flags;
    }
}


