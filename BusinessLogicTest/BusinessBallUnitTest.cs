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
      DataLayerFixcure layerBellow = new DataLayerFixcure();
      List<Ball> balls = new List<Ball>();
      Ball newInstance = new(dataBallFixture, balls, new object(), new Dimensions(400.0, 400.0, 5.0), dataBallFixture.Position, layerBellow.GetLogger());
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
      dataBallFixture.Start();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
      public DataBallFixture() {
        Position = new VectorFixture(0, 0);
        Velocity = new VectorFixture(0, 0);
      }

      private VectorFixture position;
      private VectorFixture velocity;
      public Data.IVector Velocity { get; }
      public Data.IVector Position { get; }

      public void setVelocity(double x, double y)
      {
      }
      public Double Mass { get; init; }
      public Double Diameter { get; init; }
      public void Start()
      {
        Move();
      }

      public void Stop()
      {
        throw new NotImplementedException();
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

      public double x { get; init; }
      public double y { get; init; }
    }

    }

    private class DataLayerFixcure : Data.DataAbstractAPI
    {
      public override void Dispose()
      { }

      public override void Start(int numberOfBalls, Action<Data.IVector, Data.IBall> upperLayerHandler, double diameter, double tableWidth, double tableHeight)
      {
        throw new NotImplementedException();
      }
      public override Data.ILogger GetLogger()
      {
        return new LoggerFixture();
      }

      private class LoggerFixture : Data.ILogger {
        public void Log(int threadId, string message, Data.IVector position, Data.IVector velocity)
        {

        }

        public void Stop()
        {

        }
      }
    }

    
    #endregion testing instrumentation
  }
}