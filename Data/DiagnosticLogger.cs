using System.Text.Json;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticLogger : ILogger
  {
    private readonly DiagnosticBuffer buffer;
    private readonly Thread writerThread;
    private readonly string filePath;
    internal DiagnosticLogger(string relativePath = @"..\..\..\..\logs\diagnostic.json", int bufferCapacity = 500)
    {
      filePath = Path.GetFullPath(relativePath); // resolves relative path correctly
      Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); // ensures logs/ exists

      buffer = new DiagnosticBuffer(bufferCapacity);
      writerThread = new Thread(WriteLoop);
      writerThread.Start();
    }
    public void Log(int threadId, string message, IVector position, IVector velocity)
    {
      buffer.TryAdd(new LogEntry(threadId,  message, position, velocity));
    }

    public void Stop()
    {
      buffer.Stop();
    }

    private void WriteLoop()
    {
      using StreamWriter writer = new StreamWriter(filePath, append: true);
      while (true)
      {
        LogEntry? log = buffer.WaitAndTake();
        if (log == null) { break; }
        writer.WriteLine(JsonSerializer.Serialize(log));
        writer.Flush(); // immediate write (real-time)
      }
    }
  }
}
