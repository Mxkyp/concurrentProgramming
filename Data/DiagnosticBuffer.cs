using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticBuffer
  {
    private readonly ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();
    private readonly int capacity;
    private readonly object lockObj = new object();
    private bool dataAvailable = false;

    public DiagnosticBuffer(int capacity)
    {
      this.capacity = capacity;
    }

    public bool TryAdd(string data)
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

    public string? WaitAndTake()
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
