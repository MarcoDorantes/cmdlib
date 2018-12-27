using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeOfDayTimerSpec
{
    [TestClass]
    public class TimeOfDayTimerReadConfigSpec
    {
        const string StartTimeAppSetting = "StartTime";
        const string StartRetryLimitAppSetting = "StartRetryLimit";
        const string StartRetryMillisecondsAppSetting = "StartRetryMilliseconds";
        const string MillisecondsPrecisionAppSetting = "MillisecondsPrecision";

        [TestMethod, TestCategory("Configuration")]
        public void BasicConfigurationRead()
        {
            //Arrange
            bool empty_operation() => true;

            //Act
            var startTimerConfig = utility.RetryTimeOfDayTimer.ReadTimerConfiguration(ConfigurationManager.AppSettings, StartTimeAppSetting, StartRetryLimitAppSetting, StartRetryMillisecondsAppSetting);
            using (var timer = new utility.RetryTimeOfDayTimer(startTimerConfig.InvokeDayTimes, empty_operation, startTimerConfig.RetryLimit, startTimerConfig.RetryMilliseconds, true, "StartOperationTimer"))
            {
                //Assert
                Assert.AreNotEqual(TimeSpan.Zero, timer.NextInvokeTime);
            }
        }

        [TestMethod, TestCategory("Configuration")]
        public void ReadConfiguredPrecision()
        {
            //Arrange
            bool empty_operation() => true;
            double configured_milliseconds_precision = utility.RetryTimeOfDayTimer.DefaultMillisecondsPrecision;

            //Act
            var startTimerConfig = utility.RetryTimeOfDayTimer.ReadTimerConfiguration(ConfigurationManager.AppSettings, StartTimeAppSetting, StartRetryLimitAppSetting, StartRetryMillisecondsAppSetting);
            if (double.TryParse(System.Configuration.ConfigurationManager.AppSettings[MillisecondsPrecisionAppSetting], out double precision))
            {
                configured_milliseconds_precision = precision;
            }
            using (var timer = new utility.RetryTimeOfDayTimer(startTimerConfig.InvokeDayTimes, empty_operation, startTimerConfig.RetryLimit, startTimerConfig.RetryMilliseconds, true, "StartOperationTimer", milliseconds_precision: configured_milliseconds_precision))
            {
                //Assert
                Assert.AreNotEqual(TimeSpan.Zero, timer.NextInvokeTime);
                Assert.AreEqual(100D, configured_milliseconds_precision);
            }
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("Configuration")]
        public void BadRetryLimitAppSetting()
        {
            //Arrange
            var settings = new System.Collections.Specialized.NameValueCollection();
            settings.Add(StartTimeAppSetting, "09:15");
            settings.Add(StartRetryLimitAppSetting, "09:15");
            settings.Add(StartRetryMillisecondsAppSetting, "60000");

            //Act
            utility.RetryTimeOfDayTimer.ReadTimerConfiguration(settings, StartTimeAppSetting, StartRetryLimitAppSetting, StartRetryMillisecondsAppSetting);

            //Assert
            Assert.Fail($"{nameof(utility.RetryTimeOfDayTimer.ReadTimerConfiguration)} did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("Configuration")]
        public void BadRetryMillisecondsAppSetting()
        {
            //Arrange
            var settings = new System.Collections.Specialized.NameValueCollection();
            settings.Add(StartTimeAppSetting, "09:15");
            settings.Add(StartRetryLimitAppSetting, "3");
            settings.Add(StartRetryMillisecondsAppSetting, "09:15");

            //Act
            utility.RetryTimeOfDayTimer.ReadTimerConfiguration(settings, StartTimeAppSetting, StartRetryLimitAppSetting, StartRetryMillisecondsAppSetting);

            //Assert
            Assert.Fail($"{nameof(utility.RetryTimeOfDayTimer.ReadTimerConfiguration)} did not throw");
        }

        [TestMethod, ExpectedException(typeof(System.Configuration.ConfigurationErrorsException)), TestCategory("Configuration")]
        public void BadTimeAppSetting()
        {
            //Arrange
            var settings = new System.Collections.Specialized.NameValueCollection();
            settings.Add(StartTimeAppSetting, null);

            //Act
            utility.RetryTimeOfDayTimer.ReadTimerConfiguration(settings, StartTimeAppSetting, StartRetryLimitAppSetting, StartRetryMillisecondsAppSetting);

            //Assert
            Assert.Fail($"{nameof(utility.RetryTimeOfDayTimer.ReadTimerConfiguration)} did not throw");
        }
    }
}