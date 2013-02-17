using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public class ArmyRules
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string Colour { get; private set; }
        public bool MirrorX { get; private set; }
        public string InfantryStyle { get; private set; }

        public ArmyRules(ResourceDefinition def)
        {
            Name = def.GetData("Name");
            FullName = def.GetData("FullName");
            Colour = def.GetData("Colour");
            MirrorX = def.GetData("MirrorX") == "True";
            InfantryStyle = def.GetData("InfantryStyle");
        }
    }
}
