//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
      Thread t = new Thread(new ThreadStart(MoveContinuously));
      t.Start();
    }
    public void Dispose()
    {
      disposed = true;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }
    public IVector Position{ get; set; }

    #endregion IBall

    #region private
    private bool disposed = false;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    private void Move()
    {
        Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);
        RaiseNewPositionChangeNotification();
    }

    private void MoveContinuously()
    {
        while (!disposed)
        {
            Move();
            Thread.Sleep(30);  // Adjust the interval (in milliseconds) as needed
        }
    }

        #endregion private
    }
}