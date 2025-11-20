using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS
{
    public class PlayerAircraftInfo : UnitInfo
    {
        public ulong SteamID { get; private set; }
        public string PlayerName { get; private set; }
        public void CopyPlayerInfo(Aircraft aircraft)
        {
            CopyUnitInfo(aircraft);
            SteamID = aircraft.Player.SteamID;
            PlayerName = aircraft.Player.PlayerName;
        }
        public override string ToString()
        {
            if (PlayerName != null)
            {
                return base.ToString() + $", SteamID: {SteamID}";
            }
            else return base.ToString();
        }
    }
}
