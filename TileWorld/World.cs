using System;
using PiGameSharp.VG;

namespace PiGameSharp.TileWorld
{
	/// <summary>
	/// Represents a game world made of tiles
	/// </summary>
	/// <remarks>
	/// Tile worlds are composed of two separate things, tiles (obviously) and entities. The tiles are rendered in a fixed pattern, entities can be positioned freely. When moved, entities can interact with tiles or other entities.
	/// </remarks>
	public class World : RenderNode
	{
		public TileSet Tiles;
		public ushort[,] Map;
		public bool AutoZoom = true;
		public float PixelsPerTile = 1.0f;

		public Vector2 MapSize => new Vector2((uint)Map.GetUpperBound(0) + 1, (uint)Map.GetUpperBound(1) + 1);

		public Tile this[Vector2 pos]
		{
			get
			{
				if (pos.x >= MapSize.x || pos.y >= MapSize.y)
					return null;
				return Tiles[Map[pos.x, pos.y]];
			}
		}

		public virtual void LoadMap(string map)
		{
			//TODO: remove all existing entities?
			string[] rows = map.Split('\n');
			Map = new ushort[(uint)rows[0].Length, (uint)rows.Length];

			for (uint x = 0; x < MapSize.x; x++)
				for (uint y = 0; y < MapSize.y; y++)
				{
					ushort tile = rows[MapSize.y - 1 - y][(int)x];
					if (Tiles[tile] != null)
						Map[x, y] = tile;
					else
					{
						Map[x, y] = Tiles.DefaultTile;
						if (Tiles.GetEntity(tile) != null)
							Tiles.GetEntity(tile).CreateInstance(this, new Vector3(x, y));
					}
				}
		}

		public Matrix GetTileTransform(Vector2 pos)
		{
			Tile t = this[pos];
			if (t == null || t.Image == null)
				return Matrix.Translation(new Vector3(pos.x, pos.y, 1.0f)) * Matrix.Scale(1.0f / Tiles.UnitSize.x);
			else
				return Matrix.Translation(new Vector3(pos.x - t.TileOrigin.x / (float)Tiles.UnitSize.x, pos.y - t.TileOrigin.y / (float)Tiles.UnitSize.y, 1.0f)) * Matrix.Scale(1.0f / Tiles.UnitSize.x);
		}

		public override void Draw()
		{
			// TODO: TileShapes other than square.
			// TODO: Tile Images that overlay others (skyscrapers)
			if (AutoZoom)
			{
				PixelsPerTile = Math.Min((float)VG.VG.RenderSize.x / (float)MapSize.x, (float)VG.VG.RenderSize.y / (float)MapSize.y);
				//TODO: this might only work for square tiles (or atleast it's not guaranteed that selecting this Pixels per Tile results in an integer scaling on the y-axis)
				if ((PixelsPerTile % Tiles.UnitSize.x) != 0)
					PixelsPerTile = (float)(Tiles.UnitSize.x * (int)(PixelsPerTile / Tiles.UnitSize.x));
				Transform = Matrix.Translation(new Vector3((VG.VG.RenderSize.x - PixelsPerTile * MapSize.x) / 2, (VG.VG.RenderSize.y - PixelsPerTile * MapSize.y) / 2)) * Matrix.Scale(PixelsPerTile);
			}
			// TODO: else, track entity (probably player) in a viewport bounding box?

			//TODO: determine visible ranges
			for (uint x = 0; x < MapSize.x; x++)
				for (uint y = 0; y < MapSize.y; y++)
				{
					Vector2 pos = new Vector2(x, y);
					Tile t = this[pos];
					if (t == null || t.Image == null)
						continue;

					VG.VG.ImageToSurfaceTransform = WorldTransform * GetTileTransform(pos);
					t.Image.Draw();
				}

			//draw the world first, then the entities over it. So base call goes last.
			base.Draw();
		}
	}
}
