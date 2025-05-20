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
      position = initialPosition;
      Velocity = initialVelocity;
    }

    public void Start()
    {
      Thread t = new Thread(new ThreadStart(MoveContinuously));
      t.Start();
    }


    public void Stop()
    {
      stopped = true;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { 
      get
      {
          return velocity;
      } 
      set 
      {
          velocity = (Vector) value; 
      } 
    }

    public IVector Position{ get => new Vector(position.x, position.y);  }

    #endregion IBall

    #region private
    private volatile bool stopped = false;
    private Vector position;
    private static object lockObj = new object();
    private Vector velocity;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    private void Move(double sleepTime)
    {
        position = new Vector(position.x + Velocity.x * sleepTime/1000, position.y + Velocity.y * sleepTime/1000);
        RaiseNewPositionChangeNotification();
    }

    private void MoveContinuously()
    {
        while (!stopped)
        {
        double sleepTime;

        lock (lockObj)
        {
          sleepTime = 100 / (1 + Math.Sqrt(Velocity.x * Velocity.x + Velocity.y * Velocity.y)) + 5;
          Move(sleepTime);
        }
            Thread.Sleep((int) sleepTime);  // Adjust the interval (in milliseconds) as needed
        }
    }

        #endregion private
    }
}