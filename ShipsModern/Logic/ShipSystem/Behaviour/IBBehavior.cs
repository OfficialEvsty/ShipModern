﻿using ShipsForm.Logic.ShipSystem.IceBreakerSystem.ConvoySystem;
using ShipsForm.Logic.ShipSystem.Behaviour.IBBStates;
using ShipsForm.Logic.ShipSystem.IceBreakerSystem;
using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.SupportEntities.PatternObserver;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.NodeSystem;
using System;
using System.Linq;

namespace ShipsForm.Logic.ShipSystem.Behaviour
{
    class IBBehavior : ShipBehavior
    {
        private Shell m_shell;
        private Convoy m_convoy;
        private EskortFraght[] m_fraghts;
        private EskortFraght? m_currentActiveFraght;
        private int i_fraghtsAwaits;

        public bool IsLastFraghtShipArrived { get { return i_fraghtsAwaits == 0; } }
        public bool IsWaitingTimeUp;

        public EskortFraght? CurrentActiveFraght { get { return m_currentActiveFraght; } }
        public Shell Shell { get { return m_shell; } }
        public Convoy Convoy { get { return m_convoy; } }

        public IBBehavior(Ship ship, Navigation nav, Engine eng, Shell shell, Convoy convoy, EskortFraght[] fraghts)
        {
            m_ship = ship;
            m_navigation = nav;
            m_engine = eng;
            m_shell = shell;
            m_convoy = convoy;
            m_fraghts = fraghts;
            State = new LookingForAnyShipWantsToJoinConvoy();
            EventObservable.AddEventObserver(this);
        }

        private bool AssignFraght(EskortFraght ef)
        {
            bool IsFirstFraghtRegistered = State is LookingForAnyShipWantsToJoinConvoy;
            bool IsWaiting = State is WaitingForFulFillmentConvoyState;
            if (ef is null || IsFraghtsRecruited())
                return false;
            if (ef.RequiredIceResistLevel <= m_shell.IceResistLevel)
            {
                m_fraghts[m_fraghts.Length - 1] = ef;
                ef.Charter(m_ship);
                SortFraghts();
                i_fraghtsAwaits++;
                if (IsFirstFraghtRegistered) this.GoNextState();
                return true;
            }
            return false;
        }

        public void UpdateEskortFraghts()
        {
            RemoveCompletedEscortFraghts();
            GoNextState();
        }

        public void ChecksIsConvoyCompleteRoute()
        {
            var isLastShipArrived = Convoy.Controller.IsLastShipInConvoyEndRoute();
            if (isLastShipArrived) UpdateEskortFraghts();
        }

        private void RemoveCompletedEscortFraghts()
        {
            if (m_currentActiveFraght is null)
                return;
            EskortFraght[] similarEskorts = m_fraghts.Where(fraght => !(fraght is null) && fraght.ToNode == m_currentActiveFraght.ToNode).ToArray();
            //Removes fraghts whiches have done with active fraght.
            m_fraghts = m_fraghts.Except(similarEskorts).ToArray();
            m_convoy.ShipBehaviors.ToList().ForEach(x => x.OnArrived -= ChecksIsConvoyCompleteRoute);
            foreach (var completedEskort in similarEskorts)
            {                
                m_convoy.ReleaseShip(completedEskort);
            }
        }

        private void SortFraghts()
        {
            Array.Sort(m_fraghts);
            Array.Reverse(m_fraghts);
        }

        private bool IsFraghtsRecruited()
        {
            if (m_fraghts.Last() == null) return false;
            else return true;
        }

        public EskortFraght[] GetFraghts()
        {
            if (m_ship is IceBreaker iceBreaker)
                return iceBreaker.Fraghts;
            throw new Exception("This ship can't provide Eskort fraght type information.");
        }

        public override EskortFraght? GetFraghtInfo()
        {
            SortFraghts();
            if (GetFraghts()[0] is null)
                return null;
            m_currentActiveFraght = GetFraghts()[0];
            return m_currentActiveFraght;

        }

        public override void SetStartNode(GeneralNode nd)
        {
            m_navigation.FromNode = nd;
        }

        public void FraghtShipArrived()
        {
            i_fraghtsAwaits--;
            if (IsLastFraghtShipArrived && IsWaitingTimeUp)
                GoNextState();
        }

        public override void Update(Fraght ev)
        {
            bool isSuitableFromNode = ev.FromNode == m_navigation.FromNode;
            bool isSuitableStates = State is LookingForAnyShipWantsToJoinConvoy || State is WaitingForFulFillmentConvoyState;
            if (ev is EskortFraght eskort)
                if (isSuitableFromNode && isSuitableStates)
                    AssignFraght(eskort);
        }
    }
}
