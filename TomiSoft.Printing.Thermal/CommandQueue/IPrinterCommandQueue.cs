using System;
using System.Threading.Tasks;
using TomiSoft.Printing.Thermal.EscPos.Commands;
using TomiSoft.Printing.Thermal.EscPos.Status;

namespace TomiSoft.Printing.Thermal.CommandQueue;

public interface IPrinterCommandQueue {
    event EventHandler<DleEotStatus> StatusReceived;

    void CancelPrinting();
    void CompleteAdding();
    void Enqueue(EscPosCommand command);
    void Enqueue(params EscPosCommand[] commands);
    Task QueueCompletionAsync();
}