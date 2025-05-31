namespace TP.ConcurrentProgramming.Data
{
  internal class LogEntry
  {
    public string Timestamp { get; set; } = DateTime.Now.ToString("O");
    public int ThreadId { get; set; }
    public string Message { get; set; } = "";
    public IVector Position { get; set; } = new Vector(0, 0);
    public IVector Velocity { get; set; } = new Vector(0, 0);
  }
}
