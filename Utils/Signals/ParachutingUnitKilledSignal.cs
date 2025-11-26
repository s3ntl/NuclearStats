using NuclearOption.Networking;
namespace NS.Utils.Signals
{
    public class ParachutingUnitKilledSignal
    {
        public readonly PersistentID unit;
        public readonly PersistentID killer;

        public ParachutingUnitKilledSignal(PersistentID unit, PersistentID killer = default)
        {
            this.unit = unit;
            this.killer = killer;
        }
    }
}
