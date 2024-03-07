
namespace BussinesApplication.Utils.Threads; 
public class DataUpdaterThread {
    private CancellationTokenSource _cancellationTokenSource;
    private Task _threadTask;

    public Action Action { get; set; }
    public int IntervalMilliseconds { get; set; }
    public bool IsRunning { get; private set; }

    public DataUpdaterThread() {
        IsRunning = false;
    }

    public void Start() {
        if (IsRunning) return;

        _cancellationTokenSource = new CancellationTokenSource();
        _threadTask = Task.Run(async () =>
        {
            IsRunning = true;
            while (!_cancellationTokenSource.Token.IsCancellationRequested) {
                Action?.Invoke();
                await Task.Delay(IntervalMilliseconds);
            }
            IsRunning = false;
        });
    }

    public void Stop() {
        if (!IsRunning) return;

        _cancellationTokenSource.Cancel();
    }
}
