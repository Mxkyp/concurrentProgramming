//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    #region ctor

    public BusinessLogicImplementation() : this(null)
    { }

    internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
    {
      layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
    }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      KillBalls();
      layerBellow.Dispose();
      Disposed = true;
    }

    public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
        layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
        {
            var newBall = new Ball(databall, _balls);
            _balls.Add(newBall); // Save it
            upperLayerHandler(new Position(startingPosition.x, startingPosition.y), newBall);
        });

        foreach (var ball in _balls)
        {
            ball.start();
        }
    }

    #endregion BusinessLogicAbstractAPI

    #region private

    private bool Disposed = false;
    private List<Ball> _balls = new List<Ball>();
    private readonly UnderneathLayerAPI layerBellow;

    private void KillBalls()
    {
        foreach (var ball in _balls)
        {
            ball.dispose();
        }
    }


    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}