using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticBuffer
  {
    private readonly BlockingCollection<string> buffer;

    internal DiagnosticBuffer(int capacity)
    {
      buffer = new BlockingCollection<string>(capacity);
    }

    internal bool TryAdd(string data)
    {
      return buffer.TryAdd(data, 0);
    }

    internal string? WaitAndTake()
    {
     
      while(buffer.Count == 0)
      {
          Thread.Sleep(20);
      }

      if(!buffer.TryTake(out string logData1))
      {
        throw new Exception("reading should be deterministic");
      }

        return logData1;
    }
  }
}
