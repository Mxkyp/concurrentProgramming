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

    internal Ball(Vector initialPosition, Vector initialVelocity, double Diameter)
    {
      position = initialPosition;
      Velocity = initialVelocity;
      this.Diameter = Diameter;
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
    public IVector Position{ get => new Vector(position.x, position.y);  }

    public Double Mass { get => 1; } 
    public Double Diameter { get; init; }

    #endregion IBall

    #region private
    private volatile bool disposed = false;
    private Vector position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    private void Move()
    {
        position = new Vector(position.x + Velocity.x, position.y + Velocity.y);
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