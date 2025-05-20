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

    internal Ball(Data.IBall ball)
    {
      _dataBall = ball;
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
    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
        HandleWallCollision(e);
        CheckCollisionsWithOtherBalls();
        NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    private void HandleWallCollision(Data.IVector position)
    {

      // Check X bounds (0 to 400)
      if (position.x <= 0 || position.x >= BusinessLogicImplementation.dim.TableWidth - BusinessLogicImplementation.dim.BallDimension - 2 * BusinessLogicImplementation.dim.TableBorderSize)
      {
        // Reverse X velocity (elastic bounce)
        _dataBall.Velocity.x = -_dataBall.Velocity.x;
        _dataBall.Velocity.y =  _dataBall.Velocity.y;
      }

      // Check Y bounds (0 to 400)
      if (position.y <= 0 || position.y >= BusinessLogicImplementation.dim.TableHeight - BusinessLogicImplementation.dim.BallDimension - 2 * BusinessLogicImplementation.dim.TableBorderSize)
      {
        // Reverse Y velocity (elastic bounce)
        _dataBall.Velocity.x = _dataBall.Velocity.x;
        _dataBall.Velocity.y = -_dataBall.Velocity.y;
      }
    }


    private void CheckCollisionsWithOtherBalls()
    {
      foreach (var other in BusinessLogicImplementation._balls)
      {
        if (other == this) continue; // Skip self

        double dx = this._dataBall.Position.x - other._dataBall.Position.x;
        double dy = this._dataBall.Position.y - other._dataBall.Position.y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        double collisionDistance = BusinessLogicImplementation.dim.BallDimension; 

        if (distance <= collisionDistance)
        {
          // Collision detected with 'other' ball
          HandleBallCollision(other);
        }
      }
    }

    private void HandleBallCollision(Ball other)
    {
      // Difference in positions
      double dx = this._dataBall.Position.x - other._dataBall.Position.x;
      double dy = this._dataBall.Position.y - other._dataBall.Position.y;

      // Distance squared (avoid sqrt for performance)
      double distanceSquared = dx * dx + dy * dy;

      if (distanceSquared == 0)
        return; // Prevent division by zero (balls perfectly overlapping)

      // Normalize distance vector (collision normal)
      double distance = Math.Sqrt(distanceSquared);
      double nx = dx / distance;
      double ny = dy / distance;

      // Relative velocity
      double dvx = _dataBall.Velocity.x - other._dataBall.Velocity.x;
      double dvy = _dataBall.Velocity.y - other._dataBall.Velocity.y;

      // Dot product (impact speed along the normal)
      double impactSpeed = dvx * nx + dvy * ny;

      // If moving away, no collision
      if (impactSpeed > 0)
        return;

      // Masses of the balls
      double m1 = BusinessLogicImplementation.dim.ballMass;
      double m2 = BusinessLogicImplementation.dim.ballMass;

      // Compute impulse scalar
      double impulse = -(2 * impactSpeed) / (m1 + m2);

      // Update velocities (elastic collision with mass)
      _dataBall.Velocity.x += impulse * m2 * nx;
      _dataBall.Velocity.y += impulse * m2 * ny;

      other._dataBall.Velocity.x -= impulse * m1 * nx;
      other._dataBall.Velocity.y -= impulse * m1 * ny;
    }


    #endregion private
  }
}