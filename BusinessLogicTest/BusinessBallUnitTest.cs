//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
      DataBallFixture dataBallFixture = new DataBallFixture();
      List<Ball> balls = new List<Ball>();
      Ball newInstance = new(dataBallFixture, balls, new Dimensions(10.0, 100.0, 100.0, 10.0), new Barrier(1));
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
      dataBallFixture.start();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
      public DataBallFixture() {
        Position = new VectorFixture(0, 0);
        Velocity = new VectorFixture(0, 0);
      }

      public Data.IVector Velocity { get; set; }
      public Data.IVector Position { get; set; }
      public Double Mass { get => 1; }
      public Double Diameter { get; init; }
      public void start()
      {
        Move();
      }

      public void Dispose()
      {

      }

      public event EventHandler<Data.IVector>? NewPositionNotification;

      private void Move()
      {
        NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
      }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; set; }
      public double y { get; set; }
    }

    }
    #endregion testing instrumentation
  }
}