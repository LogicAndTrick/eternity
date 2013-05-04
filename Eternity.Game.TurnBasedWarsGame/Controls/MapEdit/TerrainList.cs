using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Eternity.Controls;
using Eternity.Controls.Borders;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Graphics.Sprites;
using Eternity.Resources;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapEdit
{
    public class TerrainList : VerticalScrollPanel
    {
        private class TerrainButton : LayoutControl
        {
            public TileType TileType { get; set; }

            public TerrainButton(TileType tt, string label) : base(new HorizontalStackLayout(2))
            {
                var res = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                     {
                                                                         {"Group", "Terrain"},
                                                                         {"Type", "Terrain"},
                                                                         {"TerrainType", tt.ToString()}
                                                                     }).FirstOrDefault() ??
                          ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                     {
                                                                         {"Group", "Terrain"},
                                                                         {"Type", "Terrain"},
                                                                         {"TerrainType", TileType.City.ToString()}
                                                                     }).FirstOrDefault();
                var underlay = res.GetData("Underlay");
                var card = new LayoutControl(new CardLayout());
                SpriteControl c;
                if (underlay != "")
                {
                    c = new SpriteControl("Terrain", underlay);
                    c.DrawingOptions.DockY = SpriteDrawingOptions.Dock.Bottom;
                    card.Add(c);
                }
                c = new SpriteControl("Terrain", res.GetData("Name"));
                c.DrawingOptions.DockY = SpriteDrawingOptions.Dock.Bottom;
                card.Add(c);

                Add(card);
                Add(new Label(label, Color.White));
            }

            public override Size GetPreferredSize()
            {
                return new Size(0, 24);
            }
        }

        public TerrainList() : base(new Size(120, int.MaxValue), new LayoutControl(new VerticalStackLayout(2)))
        {
            BackgroundColour = Color.FromArgb(128, Color.Black);
            foreach (TileType tt in Enum.GetValues(typeof(TileType)))
            {
                var rules = RuleSet.GetTerrainRules(tt);
                if (rules == null) continue;
                if (tt == TileType.Pipe || tt == TileType.DestroyedPipeSeam || tt == TileType.PipeSeam) continue;
                Child.Add(new TerrainButton(tt, rules.DisplayName));
            }
        }
    }
}
