

using ShipsForm.SupportEntities.PatternObserver;
using System.Collections.Generic;

namespace ShipsForm.Logic.FraghtSystem
{
    static class FraghtMarket
    {
        private static List<Fraght> m_fraghts = new List<Fraght>();
        public static List<Fraght> Fraghts { get { return m_fraghts; } }

        public static void AddFraght(Fraght fr)
        {
            m_fraghts.Add(fr);
            EventObservable.NotifyObservers(fr);
        }
    }
}
