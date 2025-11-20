using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.StateCopyClasses;

namespace NS
{
    public class UnitInfo
    {
        public string UnitName { get; private set; }
        public PersistentID persistentID { get; private set; }

        public List<NS.StateCopyClasses.WeaponInfo> weaponsInfo = new List<StateCopyClasses.WeaponInfo>();

        public bool IsParachuted { get; private set; }
        public virtual void CopyUnitInfo(Unit unit)
        {
            UnitName = unit.unitName;
            persistentID = unit.persistentID;

            foreach (WeaponStation station in unit.weaponStations)
            {
                StateCopyClasses.WeaponInfo weaponInfo = new StateCopyClasses.WeaponInfo();
                weaponInfo.count = station.Ammo;
                weaponInfo.name = station.WeaponInfo.weaponName;
                weaponInfo.jammer = station.WeaponInfo.jammer;
                weaponsInfo.Add(weaponInfo);
            }


        }

        public override string ToString()
        {
            return $"UnitName: {UnitName}";
        }
    }
}
