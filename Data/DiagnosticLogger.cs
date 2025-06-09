using System.Text.Json;
using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
  internal class DiagnosticLogger : ILogger
  {
    private static readonly Lazy<DiagnosticLogger> singletonInstance = new Lazy<DiagnosticLogger>(() => new DiagnosticLogger());
    private readonly BlockingCollection<ILogEntry> buffer;
    private readonly Thread writerThread;
    private int logsMissed = 0;
    private readonly string filePath;
    private bool Disposed = false;
    private DiagnosticLogger(int bufferCapacity = 500)
    {
      string relativePath = @"..\..\..\..\logs\";
      relativePath = Path.Combine(relativePath, $"diagnostics_{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm")}.json");
      filePath = Path.GetFullPath(relativePath); 
      Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); 

      buffer = new BlockingCollection<ILogEntry>(bufferCapacity);
      writerThread = new Thread(WriteLoop);
      writerThread.Start();
    }
    internal static DiagnosticLogger LoggerInstance
    {
      get
      {
        return singletonInstance.Value;
      }
    }

    public void Log(DateTime timestamp, Guid ballId, IVector position, IVector velocity)
    {
      if(!buffer.IsAddingCompleted)
        if(!buffer.TryAdd(new LogEntry(timestamp, ballId, position, velocity)))
        {
          Interlocked.Increment(ref logsMissed);
        }
    }

    public void LogBallCollision(DateTime timeStamp, Guid ballId, IVector position, IVector velocity, Guid ballId2, IVector position2, IVector velocity2)
    {
      if(!buffer.IsAddingCompleted)
        if(!buffer.TryAdd(new BallCollisionLogEntry(timeStamp, ballId, position, velocity, ballId2, position2, velocity2)))
        {
          Interlocked.Increment(ref logsMissed);
        }
    }
    public void LogWallCollision(DateTime timestamp, Guid ballId, IVector position, IVector velocity)
    {
      if(!buffer.IsAddingCompleted)
        if(!buffer.TryAdd(new WallCollisionEntry(timestamp, ballId, position, velocity)))
        { 
          Interlocked.Increment(ref logsMissed);
        }
    }
  

    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
    public void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          buffer.CompleteAdding();
        }
        Disposed = true;
      }
    }

    private void WriteLoop()
    {
      using (StreamWriter writer = new StreamWriter(filePath, append: true)) {
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
          else if (log is WallCollisionEntry wallCollisionEntry)
          {
            writer.WriteLine(JsonSerializer.Serialize(wallCollisionEntry));
          }
          writer.Flush(); // immediate write (real-time)
        }
        writer.WriteLine(JsonSerializer.Serialize(new FinalLog(DateTime.UtcNow, logsMissed)));
        writer.Flush();
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

   private record FinalLog(DateTime TimeStamp, int LogsMissed);
  
  }
}
