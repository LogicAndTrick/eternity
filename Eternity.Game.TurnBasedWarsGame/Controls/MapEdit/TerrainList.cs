using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Borders;
using Eternity.Controls.Controls;
using Eternity.Controls.Easings;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Graphics.Sprites;
using Eternity.Messaging;
using Eternity.Resources;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapEdit
{
    public class TerrainList : VerticalScrollPanel
    {
        private string _currentArmy;

        private class TerrainButton : LayoutControl
        {
            public TileType TileType { get; set; }

            public TerrainButton(TileType tt, string label, string army) : base(new HorizontalStackLayout(2))
            {
                TileType = tt;
                ResourceDefinition res = null;
                
                var all = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                     {
                                                                         {"Group", "Terrain"},
                                                                         {"Type", "Terrain"},
                                                                         {"TerrainType", tt.ToString()}
                                                                     }).ToList();
                if (!all.Any())
                {
                    res = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                     {
                                                                         {"Group", "Terrain"},
                                                                         {"Type", "Terrain"},
                                                                         {"TerrainType", TileType.City.ToString()}
                                                                     }).First();
                }
                else
                {
                    if (tt.IsCapturable())
                    {
                        var ar = RuleSet.GetArmyRules(army);
                        res = all.FirstOrDefault(x => x.GetData("Name") == ar.Colour + tt);
                    }
                    if (res == null) res = all.First();
                }

                var underlay = res.GetData("Underlay");
                var card = new LayoutControl(new CardLayout());
                SpriteControl c;
                if (underlay != "")
                {
                    c = new SpriteControl("Terrain", underlay)
                            {DrawingOptions = {DockY = SpriteDrawingOptions.Dock.Bottom}};
                    card.Add(c);
                }
                c = new SpriteControl("Terrain", res.GetData("Name"))
                        {DrawingOptions = {DockY = SpriteDrawingOptions.Dock.Bottom}};
                card.Add(c);

                Add(card);
                Add(new Label(label, Color.White));
            }

            public override Size GetPreferredSize()
            {
                return new Size(0, 24);
            }

            public override void OnMouseDown(Input.EternityEvent e)
            {
                Mediator.Message(MapEditMessages.ChangeTerrain, TileType);
                Mediator.Message(MapEditMessages.FocusCursor, this);
            }

            public override void OnMouseEnter(Input.EternityEvent e)
            {
                Mediator.Message(MapEditMessages.HighlightCursor, this);
            }

            public override void OnMouseLeave(Input.EternityEvent e)
            {
                Mediator.Message(MapEditMessages.UnhighlightCursor, this);
            }
        }

        public string CurrentArmy
        {
            get { return _currentArmy; }
            set
            {
                _currentArmy = value;
                AddTiles();
            }
        }

        public TerrainList() : base(new Size(120, int.MaxValue), new LayoutControl(new VerticalStackLayout(2)))
        {
            Mediator.Subscribe(this);
            CurrentArmy = RuleSet.GetAllArmyRules().First().Name;
            BackgroundColour = Color.FromArgb(128, Color.Black);
            AddTiles();
        }

        private void AddTiles()
        {
            TileType? sel = null;
            var cursor = Child.GetChildren().SelectMany(x => x.GetAllChildren()).FirstOrDefault(x => x is IOverlayControl);
            if (cursor != null && cursor.Parent is TerrainButton)
            {
                var selected = cursor.Parent;
                sel = ((TerrainButton) selected).TileType;
            }
            Child.Remove(x => true);
            foreach (TileType tt in Enum.GetValues(typeof (TileType)))
            {
                var rules = RuleSet.GetTerrainRules(tt);
                if (rules == null) continue;
                if (tt == TileType.Pipe || tt == TileType.DestroyedPipeSeam || tt == TileType.PipeSeam) continue;
                var button = new TerrainButton(tt, rules.DisplayName, CurrentArmy);
                Child.Add(button);
                if (tt == sel) Mediator.Message(MapEditMessages.FocusCursor, button);
            }
        }

        [Subscribe(MapEditMessages.ChangeArmy)]
        private void ChangeArmy(params object[] parameters)
        {
            CurrentArmy = parameters[0] as string;
        }
    }
}
