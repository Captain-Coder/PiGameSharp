using System.Collections.Generic;

namespace PiGameSharp.TileWorld
{
	public class TileSet
	{
		public readonly TileShape Shape;
		public readonly Vector2 UnitSize;
		public ushort DefaultTile = ushort.MaxValue;

		private Dictionary<ushort, Tile> map = new Dictionary<ushort, Tile>();
		private Dictionary<ushort, EntityType> entities = new Dictionary<ushort, EntityType>();

		public TileSet(TileShape shape, Vector2 unit, IEnumerable<Tile> tiles, IEnumerable<EntityType> types)
		{
			Shape = shape;
			UnitSize = unit;
			if (tiles != null)
				foreach (Tile t in tiles)
				{
					if (DefaultTile == ushort.MaxValue)
						DefaultTile = t.Id;
					map[t.Id] = t;
				}
			if (types != null)
				foreach (EntityType et in types)
					entities[et.Id] = et;
		}

		public Tile this[ushort type] => map.ContainsKey(type) ? map[type] : null;
		public EntityType GetEntity(ushort type) => entities.ContainsKey(type) ? entities[type] : null;
	}
}