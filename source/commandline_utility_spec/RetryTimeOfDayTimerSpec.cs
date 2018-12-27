using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;

namespace TimeOfDayTimerSpec
{
    [TestClass]
    public class RetryTimeOfDayTimerSpec
    {

        [TestMethod, TestCategory("TimeOfDay")]
        public void Daytimes()
        {
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay,
                DateTime.Now.AddSeconds(10).TimeOfDay
            };
            var invokes = new List<TimeSpan>();
            Func<bool> operation = () => { invokes.Add(DateTime.Now.TimeOfDay); return true; };

            //Act
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
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5D).TimeOfDay,
                DateTime.Now.AddSeconds(30D).TimeOfDay
            };
            var invokes = new List<TimeSpan>();
            Func<bool> operation = () =>
            {
                invokes.Add(DateTime.Now.TimeOfDay);
                return invokes.Count % 2 == 0 ? true : false;
            };

            //Act
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
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5D).TimeOfDay
            };

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, null)) { }

            //Assert
            Assert.Fail("timer did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("TimeOfDay")]
        public void Precision_Mismatch()
        {
            //Arrange
            var daytime = DateTime.Now;
            var when = new List<TimeSpan>()
            {
                daytime.TimeOfDay,
                daytime.AddMilliseconds(10D).TimeOfDay
            };
            bool operation() => false;

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation)) { }

            //Assert
            Assert.Fail("timer did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.ArgumentOutOfRangeException)), TestCategory("TimeOfDay")]
        public void Precision_OutOfRange()
        {
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5D).TimeOfDay
            };
            bool operation() => false;

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation, milliseconds_precision: 0D)) { }

            //Assert
            Assert.Fail("timer did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("TimeOfDay")]
        public void Repeated()
        {
            //Arrange
            var repeat = DateTime.Now.AddSeconds(10).TimeOfDay;
            var when = new List<TimeSpan>()
            {
                DateTime.Now.TimeOfDay,
                DateTime.Now.AddSeconds(5).TimeOfDay,repeat,repeat
            };
            bool operation() => false;

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation)) { }

            //Assert
            Assert.Fail("timer did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("TimeOfDay")]
        public void NoDayTimes()
        {
            //Arrange
            IList<TimeSpan> when = null;
            bool operation() => false;

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation)) { }

            //Assert
            Assert.Fail("timer did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("TimeOfDay")]
        public void BadDayTimes()
        {
            //Arrange
            var when = new List<TimeSpan>()
            {
                TimeSpan.Zero
            };
            bool operation() { return false; }

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation)) { }

            //Assert
            Assert.Fail("timer did not throw");
        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void Precision_Default()
        {
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(3).TimeOfDay,
                DateTime.Now.AddSeconds(3).AddMilliseconds(900).TimeOfDay
            };
            var invokes = new List<TimeSpan>();
            bool operation()
            {
                invokes.Add(DateTime.Now.TimeOfDay);
                return true;
            };

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation, 3, 2000))
            {
                System.Threading.Thread.Sleep(10000);
            }

            //Assert
            Assert.AreEqual<int>(2, invokes.Count);
        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void GetNextInvokeTime()
        {
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay,
                DateTime.Now.AddSeconds(10).TimeOfDay
            };
            var invokes = new List<TimeSpan>();
            bool operation() { invokes.Add(DateTime.Now.TimeOfDay); return true; };

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation))
            {
                System.Threading.Thread.Sleep(17000);

                //Assert
                Assert.IsTrue(TimeSpan.Parse("00:00:05").TotalMilliseconds - timer.NextInvokeTime.TotalMilliseconds <= 5);
            }
        }

        class T : utility.RetryTimeOfDayTimer
        {
            public T(IEnumerable<TimeSpan> when, double milliseconds_precision = 1000) :base(milliseconds_precision)
            {
                InvokeDayTimes = when.ToList();
            }

            internal TimeSpan timeOfDay;
            internal DateTime dateTime;

            protected override TimeSpan GetCurrentTimeOfDay() =>timeOfDay;
            protected override DateTime GetCurrentDateTime() => dateTime;

        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void DuplicateTrigger_BasicCheck()
        {
            //Arrange
            var when = new List<TimeSpan>()
            {
                DateTime.Now.AddSeconds(5).TimeOfDay
            };
            var invokes = new List<TimeSpan>();
            bool operation() { invokes.Add(DateTime.Now.TimeOfDay); return true; };
            bool nextday = false;

            //Act
            using (var timer = new utility.RetryTimeOfDayTimer(when, operation))
            {
                System.Threading.Thread.Sleep(6000);
                nextday = timer.NextInvokeTime.TotalHours >= 23D;
            }

            //Assert
            Assert.AreEqual<int>(1, invokes.Count);
            Assert.IsTrue(nextday);
        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void DuplicateTrigger_BasicCheck2()
        {
            //Arrange
            T t = new TimeOfDayTimerSpec.RetryTimeOfDayTimerSpec.T
            (
                new List<TimeSpan>()
                {
                    TimeSpan.Parse("08:00:00"),
                    TimeSpan.Parse("08:00:01")
                }
            );

            //Act
            t.timeOfDay = TimeSpan.Parse("07:59:59.9");
            var next = t.GetNextInvokeTimeSpan();

            //Assert
            Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:01.1"), next);
        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void DuplicateTrigger_BasicCheck2b()
        {
            //Arrange
            T t = new TimeOfDayTimerSpec.RetryTimeOfDayTimerSpec.T
            (
                new List<TimeSpan>()
                {
                    TimeSpan.Parse("08:00:00"),
                    TimeSpan.Parse("08:00:01")
                },
                milliseconds_precision: 50D
            );

            //Act
            t.timeOfDay = TimeSpan.Parse("07:59:59.9");
            var next = t.GetNextInvokeTimeSpan();

            //Assert
            Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:00.1"), next);
        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void DuplicateTrigger_BasicCheck2c()
        {
            //Arrange
            T t = new TimeOfDayTimerSpec.RetryTimeOfDayTimerSpec.T
            (
                new List<TimeSpan>()
                {
                    TimeSpan.Parse("08:00:00"),
                    TimeSpan.Parse("08:00:01")
                },
                milliseconds_precision: 1000D
            );

            //Act
            t.timeOfDay = TimeSpan.Parse("07:59:59.9");
            var next = t.GetNextInvokeTimeSpan();

            //Assert
            Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:01.1"), next);
        }

        [TestMethod, TestCategory("TimeOfDay")]
        public void DuplicateTrigger_BasicCheck3()
        {
            //Arrange
            T t = new TimeOfDayTimerSpec.RetryTimeOfDayTimerSpec.T
            (
                new List<TimeSpan>()
                {
                    TimeSpan.Parse("08:00:00"),
                    TimeSpan.Parse("08:00:01"),
                    TimeSpan.Parse("08:00:02")
                }
            );
            TimeSpan current_time_of_day = TimeSpan.Parse("07:59:59.9");
            bool nextday = false;

            //Act
            t.timeOfDay = current_time_of_day;
            var next1 = t.GetNextInvokeTimeSpan();
            t.timeOfDay = current_time_of_day.Add(next1);
            var next2 = t.GetNextInvokeTimeSpan();
            t.timeOfDay = current_time_of_day.Add(next1 + next2);
            t.dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, t.timeOfDay.Hours, t.timeOfDay.Minutes, t.timeOfDay.Seconds, t.timeOfDay.Milliseconds);
            nextday = t.GetNextInvokeTimeSpan().TotalHours >= 23D;

            //Assert
            Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:01.1"), next1);
            Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:01"), next2);
            Assert.IsTrue(nextday);
        }

        [TestMethod, TestCategory("Weekend")]
        public void Daytimes_Saturday_disabled()
        {
            //Arrange
            var date = new DateTime(2018, 10, 6, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            var watch = Stopwatch.StartNew();
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                var current_now = new Func<DateTime>(() => date.AddMilliseconds(watch.ElapsedMilliseconds));
                System.Fakes.ShimDateTime.NowGet = () => current_now();
                System.Fakes.ShimDateTime.TodayGet = () => current_now();
                var when = new List<TimeSpan>()
                {
                    current_now().AddSeconds(5).TimeOfDay,
                    current_now().AddSeconds(10).TimeOfDay
                };
                var invokes = new List<TimeSpan>();
                bool operation() { invokes.Add(DateTime.Now.TimeOfDay); return true; };

                //Act
                using (var timer = new utility.RetryTimeOfDayTimer(when, operation, weekend_disable: true))
                {
                    System.Threading.Thread.Sleep(17000);
                }

                //Assert
                Assert.AreEqual(current_now().DayOfWeek, DateTime.Now.DayOfWeek);
                Assert.AreEqual(DateTime.Now.DayOfWeek, DayOfWeek.Saturday);
                Assert.AreEqual(0, invokes.Count);
            }
            watch.Stop();
        }

        [TestMethod, TestCategory("Weekend")]
        public void Daytimes_Saturday_enabled()
        {
            //Arrange
            var date = new DateTime(2018, 10, 6, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            var watch = Stopwatch.StartNew();
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                var current_now = new Func<DateTime>(() => date.AddMilliseconds(watch.ElapsedMilliseconds));
                System.Fakes.ShimDateTime.NowGet = () => current_now();
                System.Fakes.ShimDateTime.TodayGet = () => current_now();
                var when = new List<TimeSpan>()
                {
                    current_now().AddSeconds(5).TimeOfDay,
                    current_now().AddSeconds(10).TimeOfDay
                };
                var invokes = new List<TimeSpan>();
                bool operation() { invokes.Add(DateTime.Now.TimeOfDay); return true; };

                //Act
                using (var timer = new utility.RetryTimeOfDayTimer(when, operation, weekend_disable: false))
                {
                    System.Threading.Thread.Sleep(17000);
                }

                //Assert
                Assert.AreEqual(current_now().DayOfWeek, DateTime.Now.DayOfWeek);
                Assert.AreEqual(DateTime.Now.DayOfWeek, DayOfWeek.Saturday);
                Assert.AreEqual(2, invokes.Count);
            }
        }

        [TestMethod, TestCategory("Weekend")]
        public void Daytimes_Sunday_disabled()
        {
            //Arrange
            var date = new DateTime(2018, 10, 7, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            var watch = Stopwatch.StartNew();
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                var current_now = new Func<DateTime>(() => date.AddMilliseconds(watch.ElapsedMilliseconds));
                System.Fakes.ShimDateTime.NowGet = () => current_now();
                System.Fakes.ShimDateTime.TodayGet = () => current_now();
                var when = new List<TimeSpan>()
                {
                    current_now().AddSeconds(5).TimeOfDay,
                    current_now().AddSeconds(10).TimeOfDay
                };
                var invokes = new List<TimeSpan>();
                bool operation() { invokes.Add(DateTime.Now.TimeOfDay); return true; };

                //Act
                using (var timer = new utility.RetryTimeOfDayTimer(when, operation, weekend_disable: true))
                {
                    System.Threading.Thread.Sleep(17000);
                }

                //Assert
                Assert.AreEqual(current_now().DayOfWeek, DateTime.Now.DayOfWeek);
                Assert.AreEqual(DateTime.Now.DayOfWeek, DayOfWeek.Sunday);
                Assert.AreEqual(0, invokes.Count);
            }
            watch.Stop();
        }

        [TestMethod, TestCategory("Weekend")]
        public void Daytimes_Sunday_enabled()
        {
            //Arrange
            var date = new DateTime(2018, 10, 7, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            var watch = Stopwatch.StartNew();
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                var current_now = new Func<DateTime>(() => date.AddMilliseconds(watch.ElapsedMilliseconds));
                System.Fakes.ShimDateTime.NowGet = () => current_now();
                System.Fakes.ShimDateTime.TodayGet = () => current_now();
                var when = new List<TimeSpan>()
                {
                    current_now().AddSeconds(5).TimeOfDay,
                    current_now().AddSeconds(10).TimeOfDay
                };
                var invokes = new List<TimeSpan>();
                bool operation() { invokes.Add(DateTime.Now.TimeOfDay); return true; };

                //Act
                using (var timer = new utility.RetryTimeOfDayTimer(when, operation, weekend_disable: false))
                {
                    System.Threading.Thread.Sleep(17000);
                }

                //Assert
                Assert.AreEqual(current_now().DayOfWeek, DateTime.Now.DayOfWeek);
                Assert.AreEqual(DateTime.Now.DayOfWeek, DayOfWeek.Sunday);
                Assert.AreEqual(2, invokes.Count);
            }
        }
    }
}