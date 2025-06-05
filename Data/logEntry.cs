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
    internal LogEntry(DateTime timeStamp, Guid ballId, string message, IVector position, IVector velocity)
    {
      Timestamp = timeStamp;
      BallId = ballId;
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
  internal class BallCollisionLogEntry : ILogEntry 
  {

    internal BallCollisionLogEntry(DateTime timeStamp, Guid ballId, string message, IVector position, IVector velocity, Guid ballId2, IVector position2, IVector velocity2)
    {
      Timestamp = timeStamp;
      BallId = ballId;
      Message = message;
      Position = position;
      Velocity = velocity;
      BallId2 = ballId2;
      Position2 = position2;
      Velocity2 = velocity2;
    }

    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public string Message { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
    public Guid BallId2 { get; init; }
    public IVector Position2 { get; init; }
    public IVector Velocity2 { get; init; }
  }
}
