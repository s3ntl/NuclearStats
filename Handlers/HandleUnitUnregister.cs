using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NS
{
    [HarmonyPatch(typeof(UnitRegistry), "UnregisterUnit")]
    public class HandleUnitUnregister
    {
            public static Action<Unit> unregister;

            public static void Postfix(Unit unit)
            {

                if (unregister != null)
                {
                    unregister.Invoke(unit);
                }
            }
    }
}
