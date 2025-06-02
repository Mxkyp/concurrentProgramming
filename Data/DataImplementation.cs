//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region DataAbstractAPI

    public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler, double diameter, double tableWidth, double tableHeight)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
      Random random = new Random();
      for (int i = 0; i < numberOfBalls; i++)
      {
        Vector startingPosition = SpawnIndependently(numberOfBalls, tableWidth, tableHeight, diameter);
        Vector moveVector = new(random.Next(-80, 80), random.Next(-80, 80));
        Ball newBall = new(startingPosition, moveVector, 1.0, diameter);
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }

    private Vector SpawnIndependently(int numberOfBalls, double tableWidth, double tableHeight, double diameter)
    {
      Random random = new Random();

        Vector startingPosition;
        bool positionIsValid;

        do
        {
          positionIsValid = true;

        do
        {
          startingPosition = new Vector(
              random.Next(100, (int)tableWidth - 100),
              random.Next(100, (int)tableHeight - 100)
          );
        } while(invalidPositions.Contains(startingPosition));

          foreach (Vector existingPosition in positionsTaken)
          {
            double distance = Math.Sqrt(
                Math.Pow(startingPosition.x - existingPosition.x, 2) +
                Math.Pow(startingPosition.y - existingPosition.y, 2)
            );

            if (distance < diameter)
            {
              invalidPositions.Add(startingPosition);
              positionIsValid = false;
              break;
            }
          }
        }
        while (!positionIsValid);

        positionsTaken.Add(startingPosition);
        return startingPosition;
      }

    public override ILogger GetLogger()
    {
      return new DiagnosticLogger();
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    //private bool disposedValue;
    private bool Disposed = false;

    private List<Ball> BallsList = [];
    private List<Vector> positionsTaken = [];
    private HashSet<Vector> invalidPositions = [];

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}