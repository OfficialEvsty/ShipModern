using ShipsForm.Data;
using System;

namespace ShipsForm.Timers
{
    public struct Time
    {
        public readonly int MSEC_IN_SEC = 1000;
        public readonly int SEC_IN_MINUTE = 60;
        public readonly int MINUTE_IN_HOUR = 60;
        public readonly int HOUR_IN_DAY = 24;

        private static int GetMS(Time t) { return t.MSeconds + t.Seconds * t.MSEC_IN_SEC + t.Minutes * t.SEC_IN_MINUTE * t.MSEC_IN_SEC + t.Hours * t.MINUTE_IN_HOUR * t.SEC_IN_MINUTE * t.MSEC_IN_SEC; }

        private int i_mseconds;
        public int MSeconds
        {
            get { return i_mseconds; }
            set 
            {
                if (i_mseconds > -1)
                {
                    i_mseconds = value % MSEC_IN_SEC;
                    Seconds += value / MSEC_IN_SEC;
                }
            }
        }
        private int i_seconds;
        public int Seconds
        {
            get { return i_seconds; }
            set
            {
                if (i_seconds > -1)
                {
                    i_seconds = value % SEC_IN_MINUTE;
                    Minutes += value / SEC_IN_MINUTE;
                }
                    
            }
        }
        private int i_minutes;
        public int Minutes
        {
            get
            {
                return i_minutes;
            }
            set
            {
                if (value > -1)
                {
                    i_minutes = value % MINUTE_IN_HOUR;
                    Hours += value / MINUTE_IN_HOUR;
                }
            }
        }
        private int i_hours;
        public int Hours
        {
            get
            {
                return i_hours;
            }
            set
            {
                if (value > -1)
                {
                    i_hours = value % HOUR_IN_DAY;
                }                    
            }
        }

        public Time(int ms, int s=0, int m=0, int h=0)
        {
            i_mseconds = ms;
            i_seconds = s;
            i_minutes = m;
            i_hours = h;
        }
        public static Time operator +(Time a, Time b)
        {
            int ms = a.MSeconds + b.MSeconds;
            int s = a.Seconds + b.Seconds + ms / a.MSEC_IN_SEC;
            int m = a.Minutes + b.Minutes + s / a.SEC_IN_MINUTE;
            int h = a.Hours + b.Hours + m / a.MINUTE_IN_HOUR;
            return new Time(ms % a.MSEC_IN_SEC, s % a.SEC_IN_MINUTE, m % a.MINUTE_IN_HOUR, h % a.HOUR_IN_DAY);
        } 
        public static bool operator >=(Time a, Time b)
        {
            return GetMS(a) >= GetMS(b);
        }
        public static bool operator <=(Time a, Time b)
        {
            return GetMS(a) <= GetMS(b);
        }

    }
    public class TimerData : INotifyPropertyChanged
    {
        private Time m_time;
        private DispatcherTimer m_timer;

        private int i_multipleTimer = Configuration.Instance.MultiplyTimer;
        public int Tick = Configuration.Instance.TimeTickMS;

        public static TimerData Timer = new TimerData();

        
        private string s_timeFormat;

        public Time Time { get { return m_time; } }
        public string TimeFormat
        {
            get { return s_timeFormat; }
            set { s_timeFormat = value; }
        }
        public delegate void PropertyChangedEventHandler();

        public static event PropertyChangedEventHandler? PropertyChanged;


        public TimerData()
        {
            m_time = new Time(0, 0, 0, 9);
            TimeFormat = "Время 09:00 AM";
            m_timer = new DispatcherTimer(Tick);
            m_timer.Tick += TimerTick;
        }

        public void TimerTick()
        {
            m_time.MSeconds += Tick * i_multipleTimer;
            
            OnPropertyChanged();
            UpdateTimeFormat();
        }

        public void TimerOff()
        {
            m_timer.Pause();
        }

        public void TimerOn()
        {
            m_timer.Resume();
        }

        private void UpdateTimeFormat()
        {
            TimeFormat = string.Format("Время {0:00}:{1:00} AM", Time.Hours, Time.Minutes);
            Console.WriteLine(TimeFormat);
        }

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke();
        }
    }
}
