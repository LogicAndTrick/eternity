namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Units
{
    public static class UnitTypeExtensions
    {
        public static bool CanCapture(this UnitType unit)
        {
            return unit == UnitType.Infantry
                   || unit == UnitType.Mech
                   || unit == UnitType.Motorbike;
        }
    }
    public enum UnitType
    {
        // Infantry
        Infantry,
        Mech,
        Motorbike,

        // Tanks
        Tank,
        HeavyTank,
        MegaTank,

        // Artillery
        Artillery,
        AntiTank,
        Rockets,
        Missiles,

        // Misc. Ground Units
        AntiAir,
        Flare,
        APC,
        Recon,

        // Air Units
        Fighter,
        Bomber,
        Duster,
        BattleHelicopter,
        TransportHelicopter,
        ShipPlane,

        // Ships
        Battleship,
        Carrier,
        Sub,
        Cruiser,
        MissileBoat,
        Lander,
    }
}
