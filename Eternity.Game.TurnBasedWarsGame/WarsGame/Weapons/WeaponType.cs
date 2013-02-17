namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons
{
    public enum WeaponType
    {
        MachineGun,
        Bazooka,
        Autocannon, // Anti-Air, Carrier
        TankGun,
        ShellArtillery, // Artillery, Anti-Tank
        RocketArtillery,
        SurfaceToAirMissiles,

        AirToAirMissiles, // SRAAM: Short Range Air-Air Missiles
        Bombs,
        GatlingGun, // Duster. M61 Vulcan Cannons. Not a machine gun because they can attack air units too!
        AirToSurfaceMissiles, // Hellfires

        NavalCannon, // Battleship
        Torpedos,
        AntiSubMissiles, // Cruiser, attacks submerged ships too
        AntiShipMissiles, // Missile Boats, can't attack submerged subs
        UniversallyGoodMissiles, // Ship Planes. They can attack EVERYTHING. God I love those things.
    }
}