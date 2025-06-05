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

    internal Ball(Data.IBall ball, List<Ball> ballsList, object lckObj, Dimensions tableDim, Data.IVector startingPosition, Data.ILogger logger)
    {
      _dataBall = ball;
      underneathBallId = ball.BallId;
      _balls = ballsList;
      lockObj = lckObj;
      currentPosition = startingPosition;
      dim = tableDim;
      ball.NewPositionNotification += RaisePositionChangeEvent;
      this.logger = logger;
    }

    internal void Start()
    {
      _dataBall.Start();
    }

    internal void Stop()
    {
      this._dataBall.NewPositionNotification -= RaisePositionChangeEvent;
      _dataBall.Stop();
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private Data.IBall _dataBall;
    private List<Ball> _balls;
    private readonly object lockObj;
    private Data.IVector currentPosition;
    private Dimensions dim;
    private Data.ILogger logger;
    private Guid underneathBallId;
    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
        currentPosition = e;
        Data.IVector myVelocity = this._dataBall.Velocity;
        logger.Log(DateTime.UtcNow, underneathBallId, "position changed", currentPosition, myVelocity);
        CheckCollisionsWithOtherBalls();
        HandleWallCollision();
        NewPositionNotification?.Invoke(this, new Position(currentPosition.x, currentPosition.y));
    }

    private void HandleWallCollision()
    {
      lock (lockObj)
      {
        Data.IVector currentVel = _dataBall.Velocity;
        double newXVel;
        double newYVel;

        if ((currentPosition.x <= 0 && currentVel.x <= 0) || (currentPosition.x >= dim.TableWidth - _dataBall.Diameter - 2 * dim.TableBorderSize) && currentVel.x >= 0)
        {
          logger.Log(DateTime.UtcNow, underneathBallId, "COLIDED with vertical wall", currentPosition, currentVel);
          // Reverse X velocity (elastic bounce)
          newXVel = -currentVel.x;
          newYVel = currentVel.y;
          _dataBall.setVelocity(newXVel, newYVel);
        }

        if ( (currentPosition.y <= 0 && currentVel.y <= 0) || (currentPosition.y >= dim.TableHeight - _dataBall.Diameter - 2 * dim.TableBorderSize) && currentVel.y >= 0)
        {
          logger.Log(DateTime.UtcNow, underneathBallId, "COLIDED with horizontal wall", currentPosition, currentVel);
          // Reverse Y velocity (elastic bounce)
          newXVel = currentVel.x;
          newYVel = -currentVel.y;
          _dataBall.setVelocity(newXVel, newYVel);
        }
      }
    }


    private void CheckCollisionsWithOtherBalls()
    {
      lock (lockObj)
      {
        foreach (var other in _balls)
        {
          if (other == this) continue; // Skip self

          Data.IVector otherPostion = other.currentPosition;
          double dx = currentPosition.x - otherPostion.x;
          double dy = currentPosition.y - otherPostion.y; ;
          double distance = Math.Sqrt(dx * dx + dy * dy);

          double collisionDistance = this._dataBall.Diameter / 2 + other._dataBall.Diameter / 2;

          if (distance <= collisionDistance)
          {
            HandleBallCollision(other, distance, dx, dy, otherPostion);
          }
        }
      }
    }

    private void HandleBallCollision(Ball other, double distance, double dx, double dy, Data.IVector otherPos)
    {
        Data.IVector otherVelocity = other._dataBall.Velocity;
        Data.IVector myVelocity = _dataBall.Velocity;

        logger.LogBallCollision(DateTime.UtcNow, underneathBallId, "colided", currentPosition, myVelocity, other._dataBall.BallId, otherPos, otherVelocity);

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