namespace TP.ConcurrentProgramming.Data
{

  internal enum LogType
  {
    MOVED,
    COLIDED_WITH,
    WALL_COLLISION
  }
  internal interface ILogEntry
  {
    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public LogType Type { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
  }

  internal class LogEntry : ILogEntry 
  {
    internal LogEntry(DateTime timeStamp, Guid ballId, IVector position, IVector velocity)
    {
      Timestamp = timeStamp;
      BallId = ballId;
      Type = LogType.MOVED;
      Position = position;
      Velocity = velocity; 
    }

    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public LogType Type { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
  }
  internal class BallCollisionLogEntry : ILogEntry 
  {

    internal BallCollisionLogEntry(DateTime timeStamp, Guid ballId, IVector position, IVector velocity, Guid ballId2, IVector position2, IVector velocity2)
    {
      Timestamp = timeStamp;
      BallId = ballId;
      Type = LogType.COLIDED_WITH;
      Position = position;
      Velocity = velocity;
      BallId2 = ballId2;
      Position2 = position2;
      Velocity2 = velocity2;
    }

    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public LogType Type { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
    public Guid BallId2 { get; init; }
    public IVector Position2 { get; init; }
    public IVector Velocity2 { get; init; }
  }
  internal class WallCollisionEntry : ILogEntry 
  {
    internal WallCollisionEntry(DateTime timeStamp, Guid ballId, IVector position, IVector velocity)
    {
      Timestamp = timeStamp;
      BallId = ballId;
      Type = LogType.WALL_COLLISION;
      Position = position;
      Velocity = velocity; 
    }

    public DateTime Timestamp { get; init; }
    public Guid BallId { get; init; }
    public LogType Type { get; init; }
    public IVector Position { get; init; }
    public IVector Velocity { get; init; }
  }
}
