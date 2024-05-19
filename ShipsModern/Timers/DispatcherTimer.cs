
using System;
using System.Threading;

namespace ShipsForm.Timers
{
    interface INotifyPropertyChanged
    {
        void OnPropertyChanged();

    }

    class DispatcherTimer
    {
        System.Threading.Timer m_timer;
        int i_tick;
        private bool b_isRunning;
        public bool IsRunning { get { return b_isRunning; } }
        public delegate void TickEventHandler();
        public event TickEventHandler? Tick;
        public DispatcherTimer(int tick)
        {
            TimerCallback tm = new TimerCallback(Running);
            i_tick = tick;
            m_timer = new Timer(tm, null, 0, i_tick);
            b_isRunning = true;
        }

        private void Running(object tick)
        {
            if (m_timer != null)
            {
                Tick?.Invoke();
            }
        }

        public void Pause()
        {
            m_timer.Change(0, 0);
            b_isRunning = false;
            Console.WriteLine("Таймер был остановлен.");
        }

        public void Resume()
        {
            m_timer.Change(0, i_tick);
            b_isRunning = true;
            Console.WriteLine("Таймер запущен.");
        }
    }
}
