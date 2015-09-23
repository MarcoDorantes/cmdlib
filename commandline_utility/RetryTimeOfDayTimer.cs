using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace utility
{
  //TODO ¿si se activa y no es su hora configurada entonces deja que se reprograme para la siguiente ocasión?
  public class RetryTimeOfDayTimer : IDisposable
  {
    public class RetryTimerConfiguration
    {
      public List<TimeSpan> InvokeDayTimes { get; set; }
      public int RetryLimit { get; set; }
      public int RetryMilliseconds { get; set; }
    }

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

    private string ID;
    private List<TimeSpan> InvokeDayTimes;
    private Func<bool> Operation;
    private System.Threading.Timer MainTimer;
    private int MaxRetries;
    private int RetryMilliseconds;
    private bool WeekendDisable;
    private int retry_count;
    private TimeSpan nextinvoke;

    public RetryTimeOfDayTimer(IEnumerable<TimeSpan> when, Func<bool> operation, int retry_limit = 3, int retry_milliseconds = 60000, bool weekend_disable = false, string id = null)
    {
      this.ID = string.IsNullOrWhiteSpace(id) ? string.Format("ID_{0}", DateTime.Now.ToString("MMMdd-hhmmss-fff")) : id;
      Reset(when, operation, retry_limit, retry_milliseconds, weekend_disable);
    }

    ~RetryTimeOfDayTimer()
    {
      Dispose(false);
    }

    public TimeSpan NextInvokeTime { get { return nextinvoke; } }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Reset(IEnumerable<TimeSpan> when, Func<bool> operation, int retry_limit, int retry_milliseconds, bool weekend_disable)
    {
      this.MaxRetries = retry_limit;
      this.RetryMilliseconds = retry_milliseconds;
      this.WeekendDisable = weekend_disable;

      if (operation == null)
      {
        throw new InvalidOperationException(ID + " Timer operation cannot be null.");
      }
      if (when == null || when.Count() <= 0)
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Daytimes are not configured");
      }
      if (when.Distinct().Count() != when.Count())
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Duplicated daytimes are not supported");
      }

      this.Operation = operation;
      var positivewhen = when.Aggregate(new List<TimeSpan>(), (whole, next) => { whole.Add(next.Duration()); return whole; });
      this.InvokeDayTimes = new List<TimeSpan>();
      foreach (TimeSpan daytime in positivewhen.OrderBy(time => time))
      {
        this.InvokeDayTimes.Add(daytime);
      }
      ResultLogger.LogSuccess(string.Format(ID + " InvokeDayTime: {0}", this.InvokeDayTimes.Aggregate(new StringBuilder(), (whole, next) => { whole.AppendFormat("{0},", next); return whole; })));
      StartNextInvokeTimer();
    }

    private void StartNextInvokeTimer(int retry = 0)
    {
      if (this.MainTimer != null)
      {
        DisposeTimer();
      }
      nextinvoke = retry == 0 ? GetNextInvokeTimeSpan() : TimeSpan.FromMilliseconds(retry);
      this.MainTimer = new System.Threading.Timer(this.TimerInvoke, null, nextinvoke, TimeSpan.FromMilliseconds(-1D));
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

    public TimeSpan GetNextInvokeTimeSpan()
    {
      var result = TimeSpan.Zero;
      var now = DateTime.Now.TimeOfDay;
      var nowAndFutures = this.InvokeDayTimes.Where(time => time.CompareTo(now) >= 0);
      //ResultLogger.LogSuccess(ID + " Now.TimeOfDay:" + now);
      if (nowAndFutures.Count() > 0)
      {
        TimeSpan diff = nowAndFutures.First() - now;
        if (diff == TimeSpan.Zero)
        {
          diff = TimeSpan.Parse("00:00:03");
        }
        result = diff;
      }
      else
      {
        var nextday = DateTime.Today.AddDays(1);
        TimeSpan nextspan = this.InvokeDayTimes[0];
        var nexttime = new DateTime(nextday.Year, nextday.Month, nextday.Day, nextspan.Hours, nextspan.Minutes, nextspan.Seconds);
        result = nexttime.Subtract(DateTime.Now);
      }
      return result;
    }

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