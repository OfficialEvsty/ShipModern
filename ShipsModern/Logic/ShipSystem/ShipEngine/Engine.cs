using ShipsForm.Data;
using System;

namespace ShipsForm.Logic.ShipSystem.ShipEngine
{
    interface IEngineController
    {
        void ChangeSpeed(float speed);
        void SwitchMode(bool mode);
    }

    class Engine : IEngineController
    {
        public float MaxSpeedInKM { get; init; }
        public float AverageSpeedInKM { get; private set; }
        public float CaravanSpeedInKM { get; private set; }
        public bool IsStartEngine;

        private float f_currentSpeed;
        private bool b_isConvoyMode;

        public Action<int> OnSwitchTile;

        public Engine()
        {
            MaxSpeedInKM = 40;
            AverageSpeedInKM = MaxSpeedInKM * 3 / 4;
            CaravanSpeedInKM = Data.Configuration.Instance.CaravanSpeed;
            f_currentSpeed = AverageSpeedInKM;
            OnSwitchTile += Change;
        }

        public float Running()
        {
            if (IsStartEngine)
                return f_currentSpeed * Configuration.Instance.TimeTickMS * Configuration.Instance.MultiplyTimer / 3600;
            return 0;
        }

        public void StartEngine()
        {
            IsStartEngine = true;
        }

        public void StopEngine()
        {
            IsStartEngine = false;
        }

        public void SwitchMode(bool mode)
        {
            b_isConvoyMode = mode;
            if (b_isConvoyMode)
                StartEngine();
            else
            {
                StopEngine();
                f_currentSpeed = AverageSpeedInKM;
            }           
        }

        public void ChangeSpeed(float speed)
        {
            if (!b_isConvoyMode)
                return;
            f_currentSpeed = speed;
        }

        private void Change(int tileId)
        {
            var speedTiles = Configuration.Instance.ShipSpeedOnTile[tileId];
            f_currentSpeed = speedTiles;
        }
    }
}
