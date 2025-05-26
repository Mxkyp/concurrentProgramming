//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________





namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {

    internal Ball(Data.IBall ball, List<Ball> ballsList, object lckObj, Dimensions tableDim)
    {
      _dataBall = ball;
      _balls = ballsList;
      dim = tableDim;
      lockObj = lckObj;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    internal void Start()
    {
      _dataBall.Start();
    }

    internal void Stop()
    {
      _dataBall.Stop();
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private Data.IBall _dataBall;
    private List<Ball> _balls;
    private readonly object lockObj;
    private Dimensions dim;
    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
        Data.IVector newPosition = e;
        CheckCollisionsWithOtherBalls(newPosition);
        HandleWallCollision(newPosition);
        NewPositionNotification?.Invoke(this, new Position(newPosition.x, newPosition.y));
    }

    private void HandleWallCollision(Data.IVector position)
    {

      lock (lockObj)
      {
        Data.IVector currentVel = _dataBall.Velocity;
        double newXVel;
        double newYVel;

        if (position.x <= 0 || position.x >= dim.TableWidth - _dataBall.Diameter - 2 * dim.TableBorderSize)
        {
          // Reverse X velocity (elastic bounce)
          newXVel = -currentVel.x;
          newYVel = currentVel.y;
          _dataBall.setVelocity(newXVel, newYVel);
        }

        if (position.y <= 0 || position.y >= dim.TableHeight - _dataBall.Diameter - 2 * dim.TableBorderSize)
        {
          // Reverse Y velocity (elastic bounce)
          newXVel = currentVel.x;
          newYVel = -currentVel.y;
          _dataBall.setVelocity(newXVel, newYVel);
        }
      }
    }


    private void CheckCollisionsWithOtherBalls(Data.IVector myPosition)
    {
      lock (lockObj)
      {
        foreach (var other in _balls)
        {
          if (other == this) continue; // Skip self

          Data.IVector otherPostion = other._dataBall.Position;

          double dx = myPosition.x - otherPostion.x;
          double dy = myPosition.y - otherPostion.y; ;
          double distance = Math.Sqrt(dx * dx + dy * dy);

          double collisionDistance = this._dataBall.Diameter / 2 + other._dataBall.Diameter / 2;

          if (distance <= collisionDistance)
          {
            Data.IVector otherVel = other._dataBall.Velocity;
            Data.IVector myVel = _dataBall.Velocity;
            // Collision detected with 'other' ball
            HandleBallCollision(other, distance, dx, dy, myVel, otherVel);
          }
        }
      }
    }

    private void HandleBallCollision(Ball other, double distance, double dx, double dy, Data.IVector myVelocity, Data.IVector otherVelocity)
    {
      if (distance == 0)
        return; // Prevent division by zero (balls perfectly overlapping)

      // Normalize distance vector (collision normal)
      double nx = dx / distance;
      double ny = dy / distance;

      // Relative velocity
      double dvx = myVelocity.x - otherVelocity.x;
      double dvy = myVelocity.y - otherVelocity.y;

      // Dot product (impact speed along the normal)
      double impactSpeed = dvx * nx + dvy * ny;

      // If moving away, no collision
      if (impactSpeed > 0)
        return;

      // Masses of the balls
      double m1 = _dataBall.Mass;
      double m2 = other._dataBall.Mass;

      // Compute impulse scalar
      double impulse = -(2 * impactSpeed) / (m1 + m2);

      // Update velocities (elastic collision with mass)
      double newXVel = myVelocity.x + impulse * m2 * nx;
      double newYVel = myVelocity.y + impulse * m2 * ny;

      double newOtherXVel = otherVelocity.x - impulse * m1 * nx;
      double newOtherYVel = otherVelocity.y - impulse * m1 * ny;

      _dataBall.setVelocity(newXVel, newYVel);
      other._dataBall.setVelocity(newOtherXVel, newOtherYVel);
    }


    #endregion private
  }
}