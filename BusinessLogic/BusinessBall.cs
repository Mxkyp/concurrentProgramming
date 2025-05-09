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
    public Ball(Data.IBall ball)
    {
      _dataBall = ball;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      HandleWallCollision(e);
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }
    private void HandleWallCollision(Data.IVector position)
    {
        bool bounced = false;

        // Check X bounds (0 to 400)
        if (position.x <= 0 || position.x >= 372)
        {
            // Reverse X velocity (elastic bounce)
            _dataBall.Velocity = new Vector(-_dataBall.Velocity.x, _dataBall.Velocity.y);
            bounced = true;
        }

        // Check Y bounds (0 to 400)
        if (position.y <= 0 || position.y >= 392)
        {
            // Reverse Y velocity (elastic bounce)
            _dataBall.Velocity = new Vector(_dataBall.Velocity.x, -_dataBall.Velocity.y);
            bounced = true;
        }
    }

    #endregion private
  }
}