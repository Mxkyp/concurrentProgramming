using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticBuffer
  {
    private readonly ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();
    private readonly int capacity;
    private readonly object lockObj = new object();
    private bool dataAvailable = false;

    internal DiagnosticBuffer(int capacity)
    {
      this.capacity = capacity;
    }

    internal bool TryAdd(string data)
    {
      lock (lockObj)
      {
        if (buffer.Count >= capacity)
          return false;

        buffer.Enqueue(data);
        dataAvailable = true;
        Monitor.Pulse(lockObj); // wake up logger thread
        return true;
      }
    }

    internal string? WaitAndTake()
    {
      lock (lockObj)
      {
        while (buffer.Count == 0)
        {
          dataAvailable = false;
          Monitor.Wait(lockObj); // wait until new data arrives
        }

        if(!buffer.TryDequeue(out string logData1))
        {
          throw new Exception("reading should be deterministiic");
        }

        return logData1;
      }
    }
  }
}
