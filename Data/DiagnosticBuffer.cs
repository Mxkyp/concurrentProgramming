using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticBuffer
  {
    private readonly BlockingCollection<LogEntry> buffer;

    internal DiagnosticBuffer(int capacity)
    {
      buffer = new BlockingCollection<LogEntry>(capacity);
    }

    internal bool TryAdd(LogEntry data)
    {
      return buffer.TryAdd(data, 0);
    }

    internal LogEntry? WaitAndTake()
    {
     
      while(buffer.Count == 0)
      {
          Thread.Sleep(20);
      }

      if(!buffer.TryTake(out LogEntry logData1))
      {
        throw new Exception("reading should be deterministic");
      }

        return logData1;
    }
  }
}
