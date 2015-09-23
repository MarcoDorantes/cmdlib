using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace commandline_utility_spec
{
  [TestClass]
  public class RetryTimeOfDayTimerSpec
  {

    [TestMethod, TestCategory("TimeOfDay")]
    public void Daytimes()
    {
      //Prepare
      var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay,
                DateTime.Now.AddSeconds(10).TimeOfDay
            };
      var invokes = new List<TimeSpan>();
      Func<bool> operation = () => { invokes.Add(DateTime.Now.TimeOfDay); return true; };

      //Ack
      using (var timer = new utility.RetryTimeOfDayTimer(when, operation))
      {
        System.Threading.Thread.Sleep(17000);
      }

      //Assert
      Assert.AreEqual<int>(2, invokes.Count);
    }

    [TestMethod, TestCategory("TimeOfDay")]
    public void DaytimesRetry()
    {
      //Prepare
      var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay,
                DateTime.Now.AddSeconds(30).TimeOfDay
            };
      var invokes = new List<TimeSpan>();
      Func<bool> operation = () =>
      {
        invokes.Add(DateTime.Now.TimeOfDay);
        return invokes.Count % 2 == 0 ? true : false;
      };

      //Ack
      using (var timer = new utility.RetryTimeOfDayTimer(when, operation, 3, 2000))
      {
        System.Threading.Thread.Sleep(50000);
      }

      //Assert
      Assert.AreEqual<int>(4, invokes.Count);
    }

    [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("TimeOfDay")]
    public void NullOperation()
    {
      //Prepare
      var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay
            };

      //Ack
      using (var timer = new utility.RetryTimeOfDayTimer(when, null)) { }

      //Assert
      Assert.Fail("timer did not throw");
    }

    [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("TimeOfDay")]
    public void Repeated()
    {
      //Prepare
      var repeat = DateTime.Now.AddSeconds(10).TimeOfDay;
      var when = new List<TimeSpan>()
            {
                DateTime.Now.TimeOfDay,
                DateTime.Now.AddSeconds(5).TimeOfDay,repeat,repeat
            };
      Func<bool> operation = () => { return false; };

      //Ack
      using (var timer = new utility.RetryTimeOfDayTimer(when, operation)) { }

      //Assert
      Assert.Fail("timer did not throw");
    }

    [TestMethod, TestCategory("TimeOfDay")]
    public void GetNextInvokeTime()
    {
      //Prepare
      var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay,
                DateTime.Now.AddSeconds(10).TimeOfDay
            };
      var invokes = new List<TimeSpan>();
      Func<bool> operation = () => { invokes.Add(DateTime.Now.TimeOfDay); return true; };

      //Ack
      using (var timer = new utility.RetryTimeOfDayTimer(when, operation))
      {
        //Assert
        Assert.IsTrue(TimeSpan.Parse("00:00:05").TotalMilliseconds - timer.NextInvokeTime.TotalMilliseconds <= 5);
      }
    }
  }
}