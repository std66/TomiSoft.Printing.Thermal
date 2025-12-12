using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TomiSoft.Printing.Thermal.EscPos.Commands;
using TomiSoft.Printing.Thermal.EscPos.Status;
using TomiSoft.Printing.Thermal.Printer;

namespace TomiSoft.Printing.Thermal.CommandQueue;

public sealed class PrinterCommandQueue : IDisposable, IPrinterCommandQueue {
    private readonly Stream _stream;
    private readonly BlockingCollection<EscPosCommand> _queue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _writerTask;
    private readonly Task _readerTask;

    private InternalPrinterStatusQueryState _state = InternalPrinterStatusQueryState.Idle;
    private DleEotStatusRequestCommand _pendingStatus;

    public event EventHandler<DleEotStatus> StatusReceived;

    public PrinterCommandQueue(Stream stream) {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));

        _writerTask = Task.Run(WriterLoop, _cts.Token);
        _readerTask = Task.Run(ReaderLoop, _cts.Token);
    }

    public void Enqueue(EscPosCommand command)
        => _queue.Add(command);

    public void Enqueue(params EscPosCommand[] commands) {
        foreach (var cmd in commands)
            _queue.Add(cmd);
    }

    public void CompleteAdding()
        => _queue.CompleteAdding();

    public Task QueueCompletionAsync()
        => _writerTask;

    public void CancelPrinting() {
        CompleteAdding();
        _cts.Cancel();
    }

    // ---------------- Writer ----------------
    private async Task WriterLoop() {
        try {
            foreach (var cmd in _queue.GetConsumingEnumerable(_cts.Token)) {
                Console.WriteLine($"[PrinterCommandQueue] Sending command: {cmd.Description}");
                if (cmd is DleEotStatusRequestCommand statusCmd) {
                    _pendingStatus = statusCmd;
                    _state = InternalPrinterStatusQueryState.WaitingForStatus;
                }

                byte[] bytes = cmd.Bytes;
                await _stream.WriteAsync(bytes, 0, bytes.Length, _cts.Token);
                await _stream.FlushAsync(_cts.Token);

                // pacing (USB / BT sokszor igényli)
                await Task.Delay(1, _cts.Token);
            }
        }
        catch (OperationCanceledException) {
            // Ignore
        }
    }

    // ---------------- Reader ----------------
    private async Task ReaderLoop() {
        var buffer = new byte[1];

        while (!_cts.IsCancellationRequested) {
            if (_state != InternalPrinterStatusQueryState.WaitingForStatus) {
                await Task.Delay(5, _cts.Token);
                continue;
            }

            int read = await _stream.ReadAsync(buffer, 0, 1, _cts.Token);
            if (read != 1)
                continue;

            var status = new InternalPrinterStatus(buffer[0]);
            _state = InternalPrinterStatusQueryState.Idle;
            byte statusType = _pendingStatus.StatusType;
            _pendingStatus = null;

            StatusReceived?.Invoke(this, DleEotStatusConverter.Parse(statusType, buffer[0]));
        }
    }

    public void Dispose() {
        _cts.Cancel();
        _queue.CompleteAdding();
        _stream.Dispose();
    }
}

