﻿//____________________________________________________________________________________________________________________________________
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
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region public API

    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler, double ballDiameter, double tableWidth, double tableHeight, ILogger logger);
    public abstract ILogger GetLogger(); 

    #endregion public API

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

  public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get; init; }

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get; init; }
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;
    IVector Velocity { get; }
    Double Mass { get; init; }
    Double Diameter { get; init; }
    Guid BallId { get; init; }
    void setVelocity(double x, double y);
    void Start();
    void Stop();
  }

  public interface ILogger : IDisposable
  {
    void Log(DateTime timestamp, Guid ballId, IVector position, IVector velocity);
    void LogBallCollision(DateTime timeStamp, Guid ballId, IVector position, IVector velocity, Guid ballId2, IVector position2, IVector velocity2);
    void LogWallCollision(DateTime timestamp, Guid ballId, IVector position, IVector velocity);
  }
}