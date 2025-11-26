using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using NS.Handlers;
using NS.Utils;
using NS.Utils.Signals;

namespace NS.Readers
{
    public static class ParachuteSystem
    {
        public static List<Unit> killedUnits = new List<Unit>();
        public static void Awake()
        {
            CargoDeploymentSystemHandler.onUnitLanded += HandleLanding;
            CargoDeploymentSystemHandler.onUnitDisabled += HandleKilled;
            HandleMissionLoad.OnMissionLoad += Reset;
        }

        private static void Reset()
        {
            killedUnits.Clear();
        }

        private static void HandleLanding(Unit unit)
        {

        }

        private static void HandleKilled(Unit unit) 
        {
            if (!killedUnits.Contains(unit))
            {
                EventBus.Instance.Invoke<ParachutingUnitKilledSignal>(new ParachutingUnitKilledSignal(unit.persistentID));
            }
        }
    }
}
