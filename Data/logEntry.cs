namespace TP.ConcurrentProgramming.Data
{
  internal class LogEntry
  {
    internal LogEntry(string timeStamp, int threadId, string message, IVector position, IVector velocity)
    {
      Timestamp = timeStamp;
      ThreadId = threadId;
      Message = message;
      Position = position;
      Velocity = velocity;
    }

    public string Timestamp { get; init; }
    public int ThreadId { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
    public string Message { get; init; }
  }
}
