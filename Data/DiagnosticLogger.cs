using System.Text.Json;
using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticLogger : ILogger
  {
   private readonly BlockingCollection<ILogEntry> buffer;
    private readonly Thread writerThread;
    private readonly string filePath;
    internal DiagnosticLogger(string relativePath = @"..\..\..\..\logs\diagnostic.json", int bufferCapacity = 500)
    {
      filePath = Path.GetFullPath(relativePath); 
      Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); 

      buffer = new BlockingCollection<ILogEntry>(bufferCapacity);
      writerThread = new Thread(WriteLoop);
      writerThread.Start();
    }
    public void Log(DateTime timestamp, Guid ballId, string message, IVector position, IVector velocity)
    {
      buffer.TryAdd(new LogEntry(timestamp, ballId,  message, position, velocity));
    }

    public void LogBallCollision(DateTime timeStamp, Guid ballId, string message, IVector position, IVector velocity, Guid ballId2, IVector position2, IVector velocity2)
    {
      buffer.TryAdd(new BallCollisionLogEntry(timeStamp, ballId, message, position, velocity, ballId2, position2, velocity2));
    }

    public void Dispose()
    {
      buffer.CompleteAdding();
    }

    private void WriteLoop()
    {
      using StreamWriter writer = new StreamWriter(filePath, append: true);
      while (true)
      {
        ILogEntry? log = WaitAndTake();
        if (log == null) { break; }
        if (log is LogEntry logEntry)
        {
          writer.WriteLine(JsonSerializer.Serialize(logEntry));
        }
        else if (log is BallCollisionLogEntry collisionLogEntry)
        {
          writer.WriteLine(JsonSerializer.Serialize(collisionLogEntry));
        }
        writer.WriteLine(JsonSerializer.Serialize(log));
        writer.Flush(); // immediate write (real-time)
      }
    }
   private ILogEntry? WaitAndTake()
   {
      ILogEntry? result = null;

      while (!buffer.IsCompleted)
      {
        if(buffer.TryTake(out ILogEntry itemRead, 50))
        {
          result = itemRead;
          break;
        }
      }

      return result;
   }
  
  }
}
