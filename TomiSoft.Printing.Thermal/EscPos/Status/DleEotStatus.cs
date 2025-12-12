using System;

namespace TomiSoft.Printing.Thermal.EscPos.Status;

[Flags]
public enum DleEotStatus {
    None = 0,

    // --- Connectivity / mode ---
    Offline = 1 << 0,
    WaitingForOnline = 1 << 1,

    // --- Paper ---
    PaperNearEnd = 1 << 2,
    PaperEnd = 1 << 3,

    // --- Cover / panel ---
    CoverOpen = 1 << 4,
    PanelSwitchOn = 1 << 5,

    // --- Errors ---
    Error = 1 << 6,
    RecoverableError = 1 << 7,
    UnrecoverableError = 1 << 8,
    CutterError = 1 << 9,

    // --- Consumables ---
    ConsumableNearEnd = 1 << 10,
    ConsumableEnd = 1 << 11,

    // --- Drawer ---
    CashDrawerKickOut = 1 << 12,

    // --- Input ---
    PaperFeedButtonPressed = 1 << 13,

    // --- Firmware / internal ---
    FirmwareUpdating = 1 << 14,
    FirmwareBusy = 1 << 15,
}