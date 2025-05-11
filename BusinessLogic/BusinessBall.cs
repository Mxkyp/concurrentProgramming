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
    Data.IBall _dataBall;
    public Position currentPosition;
    List<Ball> otherBalls;
    private Dimensions dim;

    internal Ball(Data.IBall ball, List<Ball> OtherBalls, Dimensions dim, Barrier barrier)
    {
      currentPosition = new Position(ball.Position.x, ball.Position.y);
      _syncBarrier = barrier;
      _dataBall = ball;
      otherBalls = OtherBalls;
      this.dim = dim;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    public void dispose()
    {
      _dataBall.dispose();
      _syncBarrier.RemoveParticipant();
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private Barrier _syncBarrier;    //private Vector Position;
    private static object lockObj = new object();
    private static bool barrierUpdated = false;
    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {

      _syncBarrier.SignalAndWait();
      lock (lockObj)
      {
        currentPosition = new Position(e.x, e.y);
        CheckCollisionsWithOtherBalls();
        HandleWallCollision(e);
        NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
      }
    }

    private void HandleWallCollision(Data.IVector position)
    {
      bool bounced = false;

      // Check X bounds (0 to 400)
      if (position.x <= 0 || position.x >= dim.TableWidth - dim.BallDimension - 2 * dim.TableBorderSize)
      {
        // Reverse X velocity (elastic bounce)
        _dataBall.Velocity.x = -_dataBall.Velocity.x;
        _dataBall.Velocity.y =  _dataBall.Velocity.y;
        bounced = true;
      }

      // Check Y bounds (0 to 400)
      if (position.y <= 0 || position.y >= dim.TableHeight - dim.BallDimension - 2 * dim.TableBorderSize)
      {
        // Reverse Y velocity (elastic bounce)
        _dataBall.Velocity.x = _dataBall.Velocity.x;
        _dataBall.Velocity.y = -_dataBall.Velocity.y;
        bounced = true;
      }
    }


    private void CheckCollisionsWithOtherBalls()
    {
      foreach (var other in otherBalls)
      {
        if (other == this) continue; // Skip self

        double dx = currentPosition.x - other.currentPosition.x;
        double dy = currentPosition.y - other.currentPosition.y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        double collisionDistance = dim.BallDimension; // Example: 10 radius per ball

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
      double dx = this.currentPosition.x - other.currentPosition.x;
      double dy = currentPosition.y - other.currentPosition.y;

      // Distance squared (avoid sqrt for performance)
      double distanceSquared = dx * dx + dy * dy;

      if (distanceSquared == 0)
        return; // Prevent division by zero (balls perfectly overlapping)

      // Normalize distance vector (collision normal)
      double distance = Math.Sqrt(distanceSquared);
      double nx = dx / distance;
      double ny = dy / distance;

      // Get relative velocity along the normal
      double dvx = _dataBall.Velocity.x - other._dataBall.Velocity.x;
      double dvy = _dataBall.Velocity.y - other._dataBall.Velocity.y;

      // Dot product of relative velocity and the normal (impact speed)
      double impactSpeed = dvx * nx + dvy * ny;

      // If the balls are moving away from each other (positive impact speed), no collision
      if (impactSpeed > 0)
        return;

      // Apply impulse (elastic collision, equal mass)
      double impulse = -impactSpeed;  // Impulse magnitude

      // Update velocities based on the impulse
      _dataBall.Velocity.x = _dataBall.Velocity.x+ impulse * nx;  // Velocity change for ball 1
      _dataBall.Velocity.y = _dataBall.Velocity.y + impulse * ny;
      

      other._dataBall.Velocity.x =  other._dataBall.Velocity.x - impulse * nx;  // Velocity change for ball 2
      other._dataBall.Velocity.y = other._dataBall.Velocity.y - impulse * ny;
    }


    #endregion private
  }
}