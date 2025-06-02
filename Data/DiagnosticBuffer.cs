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
     return buffer.TryAdd(data);
   }

   internal LogEntry? WaitAndTake()
   {
      LogEntry? result = null;

      while (!buffer.IsCompleted)
      {
        if(buffer.TryTake(out LogEntry itemRead, 50))
        {
          result = itemRead;
          break;
        }
      }

      return result;
   }
    internal void Stop()
    {
      buffer.CompleteAdding();
    }

    }
}
