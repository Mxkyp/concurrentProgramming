namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticLogger : ILogger
  {
    private readonly DiagnosticBuffer buffer;
    private readonly Thread writerThread;
    private readonly string filePath = "./diagnostic.log";
    internal DiagnosticLogger(string filePath = "./diagnostic.log", int bufferCapacity = 100)
    {
      this.filePath = filePath;
      buffer = new DiagnosticBuffer(bufferCapacity);

      writerThread = new Thread(WriteLoop)
      {
        IsBackground = true
      };
      writerThread.Start();
    }
    public void Log(string message)
    {
      buffer.TryAdd($"{DateTime.Now:O} - {message}");
    }

    private void WriteLoop()
    {
      using StreamWriter writer = new StreamWriter(filePath, append: true);
      while (true)
      {
        string? log = buffer.WaitAndTake();
        writer.WriteLine(log);
        writer.Flush(); // immediate write (real-time)
      }
    }
  }
}
