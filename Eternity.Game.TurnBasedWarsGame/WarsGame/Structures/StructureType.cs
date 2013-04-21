using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Structures
{
    public enum StructureType
    {
        Capturable,
        Destroyable, // Pipe seam, meteor
        Interactive, // Silo
        Static       // Fire
    }
}
