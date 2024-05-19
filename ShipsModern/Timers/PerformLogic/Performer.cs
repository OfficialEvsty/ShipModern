
using System;
using System.Collections.Generic;
using static ShipsForm.Timers.PerformLogic.Performance;

namespace ShipsForm.Timers.PerformLogic
{
    class Performer
    {
        private TimerData m_time;
        private List<Performance> m_performances = new List<Performance>();
        public List<Performance> Performances { get { return m_performances; } }

        public Performer(TimerData srcTime)
        {
            m_time = srcTime;
        }
        public void AddPerformance(Time time, PerformDelegate act, params object[] args)
        {
            time += m_time.Time;
            Performance p = new Performance(time, act, args);
            m_performances.Add(p);
        }

        public void Check()
        {
            for (int i = m_performances.Count - 1; i >= 0; i--)
                if (m_time.Time >= m_performances[i].Schedule && m_performances[i] != null)
                {
                    try
                    {
                        m_performances[i].Perform();
                        m_performances.RemoveAt(i);
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                }
        }
    }
}
