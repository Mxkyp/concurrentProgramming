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

    #endregion ctor

    #region IBall

    public void Start()
    {
      Thread t = new Thread(new ThreadStart(MoveContinuously));
      t.Start();
    }


    public void Stop()
    {
      stopped = true;
    }

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { 
      get
      {
        lock (vlock)
        {
          return velocity;
        }
      } 
    }

    public void setVelocity(double x, double y)
    {
      lock (vlock)
      {
        velocity = new Vector(x, y);
      }
    }

    public IVector Position{ 
      get 
      {
        lock (plock)
        {
          return position;
        }
      } 
    } 

    #endregion IBall

    #region private
    private volatile bool stopped = false;
    private Vector position;
    private readonly object vlock = new object();
    private readonly object plock = new object();
    private Vector velocity;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    private void Move(double sleepTime, IVector vel)
    {
      IVector pos = Position;
      lock (plock)
      {
        position = new Vector(pos.x + vel.x * sleepTime / 1000, pos.y + vel.y * sleepTime / 1000);
      }
        RaiseNewPositionChangeNotification();
    }

    private void MoveContinuously()
    {
        while (!stopped)
        {
          IVector vel = Velocity;
          double sleepTime = 100 / (1 + Math.Sqrt(vel.x * vel.x + vel.y * vel.y)) + 5;

          Move(sleepTime, vel);
          Thread.Sleep((int) Math.Round(sleepTime));  
        }
    }

        #endregion private
    }
}