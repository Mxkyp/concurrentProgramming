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
  public class BusinessLogicImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixcure()))
      {
        bool newInstanceDisposed = true;
        newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
        Assert.IsFalse(newInstanceDisposed);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataLayerDisposeFixcure dataLayerFixcure = new DataLayerDisposeFixcure();
      BusinessLogicImplementation newInstance = new(dataLayerFixcure);
      Assert.IsFalse(dataLayerFixcure.Disposed);
      bool newInstanceDisposed = true;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }, 10, 10, 10, 10));
      Assert.IsTrue(dataLayerFixcure.Disposed);
    }

    [TestMethod]
    public void StartTestMethod()
    {
      DataLayerStartFixcure dataLayerFixcure = new();
      using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
      {
        int called = 0;
        int numberOfBalls2Create = 1;
        newInstance.Start(
          numberOfBalls2Create,
          (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball);}, 10, 10, 10, 10);
        Assert.AreEqual<int>(1, called);
        Assert.IsTrue(dataLayerFixcure.StartCalled);
        Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
      }
    }

    #region testing instrumentation

    private class DataLayerConstructorFixcure : Data.DataAbstractAPI
    {
      public override void Dispose()
      { }

      public override void Start(int numberOfBalls, Action<Data.IVector, Data.IBall> upperLayerHandler, double diameter, double tableWidth, double tableHeight)
      {
        throw new NotImplementedException();
      }

      public override Data.ILogger GetLogger()
      {
        throw new NotImplementedException();
      }
    }

    private class DataLayerDisposeFixcure : Data.DataAbstractAPI
    {
      internal bool Disposed = false;

      public override void Dispose()
      {
        Disposed = true;
      }

      public override void Start(int numberOfBalls, Action<Data.IVector, Data.IBall> upperLayerHandler, double diameter, double tableWidth, double tableHeight)
      {
        throw new NotImplementedException();
      }

      public override Data.ILogger GetLogger()
      {
        throw new NotImplementedException();
      }
    }

    private class DataLayerStartFixcure : Data.DataAbstractAPI
    {
      internal bool StartCalled = false;
      internal int NumberOfBallseCreated = -1;

      public override void Dispose()
      { }

      public override void Start(int numberOfBalls, Action<Data.IVector, Data.IBall> upperLayerHandler, double diameter, double tableWidth, double tableHeight)
      {
        StartCalled = true;
        NumberOfBallseCreated = numberOfBalls;
        upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
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
    private record DataVectorFixture : Data.IVector
    {
      internal DataVectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }
      internal DataVectorFixture()
      {
        x = 0; y = 0;
      }

      public double x { get; init; }
      public double y { get; init; }
    }
    private class DataBallFixture : Data.IBall
    {
      internal DataBallFixture()
      {
        Position = new DataVectorFixture(0, 0);
        Velocity = new DataVectorFixture(0, 0);
        Mass = 1.0;
        Diameter = 10.0;
      }

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

      }

      public event EventHandler<Data.IVector>? NewPositionNotification;

      private void Move()
      {
        NewPositionNotification.Invoke(this, new DataVectorFixture(0.0, 0.0));
      }
    }

    #endregion testing instrumentation
  }
  }