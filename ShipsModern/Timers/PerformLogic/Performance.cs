
using System;

namespace ShipsForm.Timers.PerformLogic
{

    class Performance : IDisposable
    {
        object[]? arr_args;
        private Time schedule;
        private PerformDelegate m_act;
        public object[]? Args { get { return arr_args; } }
        public Time Schedule { get { return schedule; } }
        public delegate void PerformDelegate(object[]? args);

        public Performance(Time time, PerformDelegate act, params object[]? args)
        {
            schedule = time;
            m_act = act;
            arr_args = args;
        }

        public void Perform()
        {
            m_act(arr_args);
            Dispose();
            Console.WriteLine($"Delayed task-{m_act.Method.Name} at {Schedule.Hours}:{Schedule.Minutes}:{Schedule.Seconds} had performed with args: ({arr_args}).");
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
