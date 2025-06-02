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

    internal Ball(Vector initialPosition, Vector initialVelocity, double mass, double diameter)
    {
      position = initialPosition;
      velocity = initialVelocity;
      this.mass = mass;
      this.diameter = diameter;
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

    public Double Mass
    {
      get { return mass; }
      init { mass = value; }
    }
    public Double Diameter 
    {
      get { return diameter; }
      init { mass = value; }
    }

    public void setVelocity(double x, double y)
    {
      lock (vlock)
      {
        velocity = new Vector(x, y);
      }
    }

    #endregion IBall

    #region private

    private volatile bool stopped = false;
    private Vector position;
    private Vector velocity;
    private readonly Double mass;
    private readonly Double diameter;
    private readonly object vlock = new object();
    private readonly object plock = new object();

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, position);
    }

    private void Move(double time, IVector vel)
    {
      position = new Vector(position.x + vel.x * (time / 1000), position.y + vel.y * (time / 1000));
      RaiseNewPositionChangeNotification();
    }

    private int CalculateRefresh(IVector velocity)
    {
          return (int) Math.Round(100 / (1 + Math.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y)) + 5);
    }

    private void MoveContinuously()
    {
      var stopwatch = new System.Diagnostics.Stopwatch();
      stopwatch.Start();

      long lastUpdate = stopwatch.ElapsedMilliseconds;


      while (!stopped)
        {
        long now = stopwatch.ElapsedMilliseconds;
        double deltaTime = (now - lastUpdate); // miliseconds
        lastUpdate = now;

        if(deltaTime > 1000)
        {
          throw new Exception("EXCEEDED MAXIMUM TIME CONSTRAINT");
        }

        IVector vel = Velocity;
        Move(deltaTime, vel);

        int refreshTime = CalculateRefresh(vel);
        Thread.Sleep(refreshTime);  
        }
    }

        #endregion private
    }
}