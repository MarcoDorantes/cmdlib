using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

//TODO ¿si se activa y no es su hora configurada entonces deja que se reprograme para la siguiente ocasión?

namespace utility
{
    /// <summary>
    /// Invokes a recurring operation at configured times of day.
    /// The operation must return true for a successful invocation that must not be retried at the same time of the current day.
    /// On the other hand, if the operation catches a failure and the operation needs to be retried for the given time of day,
    /// then the operation must return false and it will be retried after the configured retry milliseconds have elapsed.
    /// If the retried operation returns false again, then it will be retried again after the configured retry milliseconds have elapsed.
    /// If the limit of retries is reached then the operation will not be retried any longer at the same time of the current day, but, instead, at the next
    /// configured time of the current day or, if there is no more configured times for the current day, at the first configured time of the next day.
    /// </summary>
    public class RetryTimeOfDayTimer : IDisposable
    {
        public const double DefaultMillisecondsPrecision = 500D;

        #region Configuration statics
        /// <summary>
        /// Utility class returned by the utility static method RetryTimeOfDayTimer.ReadTimerConfiguration.
        /// </summary>
        public class RetryTimerConfiguration
        {
            public List<TimeSpan> InvokeDayTimes { get; set; }
            public int RetryLimit { get; set; }
            public int RetryMilliseconds { get; set; }
        }

        /// <summary>
        ///  Utility method to parse the RetryTimeOfDayTimer configuration from a NameValueCollection instance, like the System.Configuration.ConfigurationManager.AppSettings property.
        /// </summary>
        /// <param name="appSettings">A NameValueCollection instance, like the System.Configuration.ConfigurationManager.AppSettings property.</param>
        /// <param name="DayTimerSpansAppSetting">Key of the entry in appSettings with the configured times at which the configured operation must be invoked at the current day.</param>
        /// <param name="RetryLimitAppSetting">Key of the entry in appSettings with the limit number of retries for a single time of day invocation.</param>
        /// <param name="RetryMillisecondsAppSetting">Key of the entry in appSettings with the retry milliseconds.</param>
        /// <returns></returns>
        public static RetryTimerConfiguration ReadTimerConfiguration(NameValueCollection appSettings, string DayTimerSpansAppSetting, string RetryLimitAppSetting, string RetryMillisecondsAppSetting)
        {
            var result = new RetryTimerConfiguration();

            //App setting: DayTimerSpans
            result.InvokeDayTimes = utility.RetryTimeOfDayTimer.ParseInvokeDayTimes(appSettings[DayTimerSpansAppSetting], DayTimerSpansAppSetting);

            //App setting: RetryLimit
            int setting;
            if (int.TryParse(appSettings[RetryLimitAppSetting], out setting))
            {
                result.RetryLimit = setting;
            }
            else
            {
                string error_message = string.Format("Config value {0} ({1}) is invalid", RetryLimitAppSetting, setting);
                throw new System.Configuration.ConfigurationErrorsException(error_message);
            }

            //App setting: RetryMillisecondsAppSetting
            if (int.TryParse(appSettings[RetryMillisecondsAppSetting], out setting))
            {
                result.RetryMilliseconds = setting;
            }
            else
            {
                string error_message = string.Format("Config value {0} ({1}) is invalid", RetryMillisecondsAppSetting, setting);
                throw new System.Configuration.ConfigurationErrorsException(error_message);
            }
            return result;
        }

        /// <summary>
        /// Parse the configured times of day for timer invocation. It is called by the utility static method RetryTimeOfDayTimer.ReadTimerConfiguration.
        /// </summary>
        /// <param name="settingtext">The configured times value at which the configured operation must be invoked at the current day. These could be one TimeSpan value or multiple TimeSpan values separated by pipe (|).</param>
        /// <param name="appSettingKey">Key of the name in appSettings with the configured times at which the configured operation must be invoked at the current day.</param>
        /// <returns></returns>
        public static List<TimeSpan> ParseInvokeDayTimes(string settingtext, string appSettingKey = "")
        {
            var invoketimes = new List<TimeSpan>();
            bool timespansOk = !string.IsNullOrWhiteSpace(settingtext);
            if (timespansOk == false)
            {
                string error_message = string.Format("Config value for day timer spans {0} ({1}) is empty", appSettingKey, settingtext);
                throw new System.Configuration.ConfigurationErrorsException(error_message);
            }
            else
            {
                string[] timespans = settingtext.Split('|');
                int k = -1;
                do
                {
                    ++k;
                    if (!timespansOk || k >= timespans.Length) break;
                    if (string.IsNullOrWhiteSpace(timespans[k])) continue;

                    TimeSpan timespan;
                    timespansOk = TimeSpan.TryParse(timespans[k], out timespan);
                    if (timespansOk)
                    {
                        invoketimes.Add(timespan);
                    }
                } while (true);
            }
            if (timespansOk == false)
            {
                string error_message = string.Format("Config value for day timer spans {0} ({1}) is invalid", appSettingKey, settingtext);
                throw new System.Configuration.ConfigurationErrorsException(error_message);
            }
            return invoketimes;
        }
        #endregion

        private string ID;
        protected List<TimeSpan> InvokeDayTimes;
        private Func<bool> Operation;
        private System.Threading.Timer MainTimer;
        private int MaxRetries;
        private int RetryMilliseconds;
        private bool WeekendDisable;
        private double MillisecondsPrecision;
        private int retry_count;
        private TimeSpan nextinvoke;

        /// <summary>
        /// Setup an instance of the timer.
        /// </summary>
        /// <param name="when">DayTimes at which the configured operation must be invoked at the current day. The utility static method RetryTimeOfDayTimer.ReadTimerConfiguration could be useful to get this value.</param>
        /// <param name="operation">The recurring operation to invoke.</param>
        /// <param name="retry_limit">The limit number of retries for an invocation. The utility static method RetryTimeOfDayTimer.ReadTimerConfiguration could be useful to get this value. Default 3.</param>
        /// <param name="retry_milliseconds">The retry milliseconds. The utility static method RetryTimeOfDayTimer.ReadTimerConfiguration could be useful to get this value. Default 60000.</param>
        /// <param name="weekend_disable">True if the operation must not be invoked at System.DayOfWeek.Saturday or System.DayOfWeek.Sunday days. Default false.</param>
        /// <param name="id">Identification label for this instance of the timer. Default null.</param>
        /// <param name="milliseconds_precision">Minimum amount of milliseconds between DayTimes; also, between DateTime.Now and the next scheduled timer event. Default 500ms.</param>
        public RetryTimeOfDayTimer(IEnumerable<TimeSpan> when, Func<bool> operation, int retry_limit = 3, int retry_milliseconds = 60000, bool weekend_disable = false, string id = null, double milliseconds_precision = DefaultMillisecondsPrecision)
        {
            this.ID = string.IsNullOrWhiteSpace(id) ? string.Format("ID_{0}", DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff")) : id;
            Reset(when, operation, retry_limit, retry_milliseconds, weekend_disable, milliseconds_precision);
        }

        /// <summary>
        /// This ctor is intented for unit testing and it must not be used by any application but for the RetryTimeOfDayTimer's unit tests only.
        /// </summary>
        /// <param name="milliseconds_precision"></param>
        protected RetryTimeOfDayTimer(double milliseconds_precision = DefaultMillisecondsPrecision) { MillisecondsPrecision = milliseconds_precision; }

        ~RetryTimeOfDayTimer()
        {
            Dispose(false);
        }

        /// <summary>
        /// The TimeSpan value of the next invocation time.
        /// </summary>
        public TimeSpan NextInvokeTime { get { return nextinvoke; } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Reset(IEnumerable<TimeSpan> when, Func<bool> operation, int retry_limit, int retry_milliseconds, bool weekend_disable, double milliseconds_precision)
        {
            MaxRetries = retry_limit;
            RetryMilliseconds = retry_milliseconds;
            WeekendDisable = weekend_disable;
            MillisecondsPrecision = milliseconds_precision;

            #region Input validation
            if (operation == null)
            {
                throw new InvalidOperationException(ID + " Timer operation cannot be null.");
            }
            if (MillisecondsPrecision <= 0D)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds_precision), ID + $" {nameof(milliseconds_precision)} must be positive.");
            }
            if (when == null || when.Count() <= 0)
            {
                throw new System.Configuration.ConfigurationErrorsException(ID + " Daytimes are not configured");
            }
            if (when.Any(time => time == TimeSpan.Zero))
            {
                throw new System.Configuration.ConfigurationErrorsException(ID + " Zero daytimes are not supported");
            }
            if (when.Distinct().Count() != when.Count())
            {
                throw new System.Configuration.ConfigurationErrorsException(ID + " Duplicated daytimes are not supported");
            }
            #endregion

            Operation = operation;
            var positivewhen = when.Aggregate(new List<TimeSpan>(), (whole, next) => { whole.Add(next.Duration()); return whole; });
            InvokeDayTimes = new List<TimeSpan>();
            var previous = TimeSpan.Zero;
            foreach (TimeSpan daytime in positivewhen.OrderBy(time => time))
            {
                if (previous != TimeSpan.Zero)
                {
                    TimeSpan diff = daytime - previous;
                    if (diff.TotalMilliseconds < MillisecondsPrecision)
                    {
                        throw new System.Configuration.ConfigurationErrorsException(ID + $" Timer DayTimes ({previous} and {daytime}) are closer than default or configured milliseconds precision ({MillisecondsPrecision}).");
                    }
                }
                InvokeDayTimes.Add(daytime);
                previous = daytime;
            }
            ResultLogger.LogSuccess($"{ID} InvokeDayTime: {InvokeDayTimes.Aggregate(new StringBuilder(), (whole, next) => { whole.AppendFormat("{0},", next); return whole; })}");
            StartNextInvokeTimer();
        }

        private void StartNextInvokeTimer(int retry = 0)
        {
            if (MainTimer != null)
            {
                DisposeTimer();
            }
            nextinvoke = retry == 0 ? GetNextInvokeTimeSpan() : TimeSpan.FromMilliseconds(retry);
            MainTimer = new System.Threading.Timer(TimerInvoke, null, nextinvoke, TimeSpan.FromMilliseconds(-1D));
            ResultLogger.LogSuccess(ID + " Next invoke:" + nextinvoke);
        }

        private void TimerInvoke(object unused_state)
        {
            bool result = false;
            try
            {
                PauseTimer();
                DayOfWeek dayname = DateTime.Now.DayOfWeek;
                if (WeekendDisable && (dayname == DayOfWeek.Saturday || dayname == DayOfWeek.Sunday))
                {
                    ResultLogger.LogSuccess(string.Format("{0} Today is {1} and the execution has been disable for today.", ID, dayname));
                    result = true;
                }
                else
                {
                    result = this.Operation();
                }
            }
            finally
            {
                if (result)
                {
                    StartNextInvokeTimer();
                }
                else
                {
                    if (this.retry_count < this.MaxRetries)
                    {
                        StartNextInvokeTimer(this.RetryMilliseconds);
                        ++this.retry_count;
                    }
                    else
                    {
                        this.retry_count = 0;
                        ResultLogger.LogSuccess(ID + " Retry limit");
                        StartNextInvokeTimer();
                    }
                }
            }
        }

        private void PauseTimer()
        {
            if (this.MainTimer != null)
            {
                this.MainTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
        }

        private void DisposeTimer()
        {
            if (this.MainTimer != null)
            {
                this.MainTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                this.MainTimer.Dispose();
                this.MainTimer = null;
                ResultLogger.LogSuccess(ID + " RetryTimeOfDayTimer.DisposeMainTimer");
            }
        }

        /// <summary>
        /// This method is invoked internally and it should not be used by any application but for the RetryTimeOfDayTimer's unit tests only.
        /// </summary>
        /// <returns>TimeSpan for next scheduled timer run.</returns>
        public TimeSpan GetNextInvokeTimeSpan()
        {
            var result = TimeSpan.Zero;
            foreach (var nextDayTime in InvokeDayTimes)
            {
                var now = GetCurrentTimeOfDay();
                //ResultLogger.LogSuccess($"{ID} Now.TimeOfDay: {now}");
                TimeSpan diff = nextDayTime - now;
                if (diff.TotalMilliseconds >= MillisecondsPrecision)
                {
                    result = diff;
                    break;
                }
                else
                {
                    if (diff.TotalMilliseconds > 0D)
                    {
                        ResultLogger.LogSuccess($"WARN: {ID} TimeOfDay ({nextDayTime}) is skipped because it and current TimeOfDay ({now}) are closer ({diff}) than default or configured milliseconds precision ({MillisecondsPrecision}).");
                        //throw new InvalidOperationException($"{ID} TimeOfDay ({nextDayTime}) is not supported because it and current TimeOfDay ({now}) are closer ({diff}) than default or configured milliseconds precision ({MillisecondsPrecision}).");
                    }
                }
            }
            if(result == TimeSpan.Zero)
            {
                var nextday = DateTime.Today.AddDays(1);
                TimeSpan nextspan = this.InvokeDayTimes[0];
                var nexttime = new DateTime(nextday.Year, nextday.Month, nextday.Day, nextspan.Hours, nextspan.Minutes, nextspan.Seconds);
                result = nexttime.Subtract(GetCurrentDateTime());
            }
            return result;
        }

        protected virtual TimeSpan GetCurrentTimeOfDay() => DateTime.Now.TimeOfDay;
        protected virtual DateTime GetCurrentDateTime() => DateTime.Now;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeTimer();
                ResultLogger.LogSuccess(ID + " RetryTimeOfDayTimer disposed.");
            }
        }
    }
}