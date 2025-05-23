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
  public abstract class BusinessLogicAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region Layer API

    public static readonly Dimensions GetDimensions = new(10.0, 10.0, 10.0);

    public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler, double width, double height, double borderSize, double ballDia);

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #endregion Layer API

    #region private

    private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

    #endregion private
  }
  /// <summary>
  /// Immutable type representing table dimensions
  /// </summary>
  /// <param name="TableHeight"></param>
  /// <param name="TableWidth"></param>
  /// <param name="TableBorderSize"></param>
  /// <remarks>
  /// Must be abstract
  /// </remarks>
  public record Dimensions(double TableHeight, double TableWidth, double TableBorderSize);

  public interface IPosition
  {
    double x { get; init; }
    double y { get; init; }
  }

  public interface IBall 
  {
    event EventHandler<IPosition> NewPositionNotification;
  }
}