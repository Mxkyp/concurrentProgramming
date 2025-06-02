namespace TP.ConcurrentProgramming.Data
{
  internal class LogEntry
  {
    internal LogEntry(int threadId, string message, IVector position, IVector velocity)
    {
      ThreadId = threadId;
      Message = message;
      Position = position;
      Velocity = velocity;
    }

    public string Timestamp { get; set; } = DateTime.Now.ToString("O");
    public int ThreadId { get; set; }
    public IVector Position { get; set; } = new Vector(0, 0);
    public IVector Velocity { get; set; } = new Vector(0, 0);
    public string Message { get; set; } = "";
  }
}
