using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Unload
{
    /// <summary>
    /// Loaded units can drop off their loadees onto adjacent tiles.
    /// </summary>
    public class UnloadAction : IUnitAction
    {
        private readonly ContextState _state;
        private readonly List<Move> _previousUnloads;
        private readonly Unit _unitToUnload;
        private Tile _unloadTile;

        public UnloadAction(ContextState state, Unit unitToUnload, IEnumerable<Move> previousUnloads)
        {
            _state = state;
            _unitToUnload = unitToUnload;
            _previousUnloads = previousUnloads.ToList();
            _unloadTile = null;
        }

        public bool IsValidTile(Tile tile)
        {
            return tile.CanMoveTo;
        }

        public List<ValidTile> GetValidTiles()
        {
            return _state.Tile.GetAdjacentTiles()
                .Where(x => x != null && _unitToUnload.CanMoveOn(x.Type))
                .Where(x => !x.HasVisibleUnit(_state.Unit.Army) || x.Unit == _state.Unit)
                .Where(x => !_previousUnloads.Any(y => y.MoveTile == x && y.UnitToMove != _unitToUnload))
                .Select(x => new ValidTile { MoveType = MoveType.Move, Tile = x}).ToList();
        }

        public void UpdateMoveSet(Tile tile)
        {
            _unloadTile = tile;
        }

        public MoveSet GetMoveSet()
        {
            return _unloadTile == null
                       ? null
                       : new MoveSet(_state.Unit, new[] {Move.CreateUnload(_unloadTile, _state.Tile, _unitToUnload)});
        }

        public void Cancel()
        {
            //
        }

        public string GetName()
        {
            return "Unload";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Unload, _state.Unit, _state.Tile, this, new UnloadActionRunner(_state.Unit, _unitToUnload, _unloadTile));
        }

        public bool IsInstantAction()
        {
            return false;
        }

        public bool IsCommittingAction()
        {
            return _previousUnloads.Count + 1 == _state.Unit.LoadedUnits.Count;
        }
    }
}