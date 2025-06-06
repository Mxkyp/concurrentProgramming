//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      LoggerFix lgr = new LoggerFix(); 
      Vector testinVector = new Vector(0.0, 0.0);
      Ball newInstance = new(testinVector, testinVector, 1.0, 10.0, lgr);
    }

    [TestMethod]
    public void MoveTestMethod()
    {
      LoggerFix lgr = new LoggerFix(); 
      Vector initialPosition = new(10.0, 10.0);
      Ball newInstance = new(initialPosition, new Vector(0.0, 0.0), 1.0, 10.0, lgr);
      IVector curentPosition = new Vector(0.0, 0.0);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
      newInstance.Start();
      while (numberOfCallBackCalled == 0)
      {
        Thread.Sleep(10);
      }
      Assert.IsTrue(numberOfCallBackCalled >= 1);
      Assert.AreEqual<IVector>(initialPosition, curentPosition);
      newInstance.Stop();
    }
    private class LoggerFix : ILogger
    {
    public void Log(DateTime timestamp, Guid ballId, IVector position, IVector velocity)
    {

    }
    public void LogBallCollision(DateTime timeStamp, Guid ballId, IVector position, IVector velocity, Guid ballId2, IVector position2, IVector velocity2)
    {

    }
    public void LogWallCollision(DateTime timestamp, Guid ballId, IVector position, IVector velocity)
    {

    }
    public void Dispose()
    {

    }
    }
  }
}