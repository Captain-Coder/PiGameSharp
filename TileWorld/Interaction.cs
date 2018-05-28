using System;
using PiGameSharp.VG;

namespace PiGameSharp.TileWorld
{
	/// <summary>
	/// Interaction between entity and a tile
	/// </summary>
	/// <param name="entity">The entity that causes the interaction</param>
	/// <param name="position">The position of the tile that has the interaction defined</param>
	/// <param name="dir">The direction that this tile is relative to the entity</param>
	/// <param name="covered">True if the position is or becomes covered, false if the position is or becomes uncovered by the entity</param>
	/// <param name="delta">The proposed movement of the entity that causes the interaction</param>
	/// <returns>True if previous interactions need to be reevaluated, false otherwise.</returns>
	public delegate bool TileInteraction(Entity entity, Vector2 position, Direction dir, bool covered, ref Vector3 delta);

	/// <summary>
	/// Interaction between entity and another entity
	/// </summary>
	/// <param name="entity">The entity that causes the interaction</param>
	/// <param name="target">The entity that is being interacted with</param>
	/// <param name="delta">The proposed movement of the entity that causes the interaction</param>
	/// <returns>True if previous interactions need to be reevaluated, false otherwise.</returns>
	public delegate bool EntityInteraction(Entity entity, Entity target, ref Vector3 delta);

	public static class DefaultInteractions
	{
		public static bool Wall(Entity entity, Vector2 position, Direction dir, bool covered, ref Vector3 delta)
		{
			if (!covered)
				return false;

			Vector3 entity_pos = entity.Position;
			Vector3 overlap = Vector3.Zero;
			Vector3 move = Vector3.Zero;
			bool changed = false;

			overlap = new Vector3(
				entity_pos.x - position.x + delta.x + (dir.HasRight() ? entity.WidthInTiles / 2 : 0) - (dir.HasLeft() ? entity.WidthInTiles / 2 + 1 : 0),
				entity_pos.y - position.y + delta.y + (dir.HasTop() ? entity.HeightInTiles / 2 : 0) - (dir.HasBottom() ? entity.HeightInTiles / 2 + 1: 0));

			if (dir.HasTopLeft() || dir.HasTopRight() || dir.HasBottomLeft() || dir.HasBottomRight())
			{
				if (Math.Abs(delta.x) < Math.Abs(delta.y))
					move = new Vector3(-overlap.x, 0);
				else if (Math.Abs(delta.y) < Math.Abs(delta.x))
					move = new Vector3(0, -overlap.y);
				//else
				// exactly on corner with exactly 45 deg angle heading for the tile.
				//TODO: perhaps we can use the overlap to determine "direction" in this case

				if (changed = move.LengthSq > delta.LengthSq)
					delta = move * (delta.Length / move.Length);
				else if (changed = move.LengthSq > 0 && delta.LengthSq > 0)
					if (move.x != 0)
						delta = new Vector3(move.x, delta.Length * (float)Math.Sin(Math.Acos(move.x / delta.Length)));
					else
						delta = new Vector3(delta.Length * (float)Math.Sin(Math.Acos(move.y / delta.Length)), move.y);
			}
			else if (changed = overlap.x != 0 && dir.HasLeft() || dir.HasRight())
				delta += new Vector3(-overlap.x, 0);
			else if (changed = overlap.y != 0 && dir.HasTop() || dir.HasBottom())
				delta += new Vector3(0, -overlap.y);

			return changed;
		}
	}
}