using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NS
{
    [HarmonyPatch(typeof(UnitRegistry), "RegisterUnit")]
    public class HandleUnitRegister
    {
        public static Action<Unit, PersistentID> register;

        public static void Postfix(Unit unit, PersistentID id)
        {
            if (register != null)
            {
                register.Invoke(unit, id);
            }
           /* if (unit is Aircraft)
            {
               Aircraft aircraft = (Aircraft)unit;
                if (DeathNote.deathNotePlayers.ContainsKey(aircraft.Player.PlayerName))
                {
                    DeathNote.aircraftsInDeathNote.Add(aircraft);
                }
            }
           */
;        }

    }
}
