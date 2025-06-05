namespace TP.ConcurrentProgramming.Data
{
  internal interface ILogEntry
  {
    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public string Message { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
  }

  internal class LogEntry : ILogEntry 
  {
    internal LogEntry(DateTime timeStamp, int threadId, string message, IVector position, IVector velocity)
    {
      Timestamp = timeStamp;
      BallId = threadId;
      Message = message;
      Position = position;
      Velocity = velocity;
    }

    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public string Message { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
  }
}
