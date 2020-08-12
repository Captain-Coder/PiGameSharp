using System.Collections.Generic;
using PiGameSharp.VG;

namespace PiGameSharp.TileWorld
{
	public class Entity : RenderNode
	{
		public Image Image;
#if DEBUG
		public Image ImageCover;
#endif

		public EntityType EntityType;

		public World World => (World)Parent;
		public Vector2 Size => Image != null ? Image.Size : World.Tiles.UnitSize;
		public float WidthInTiles => Size.x / (float)World.Tiles.UnitSize.x;
		public float HeightInTiles => Size.y / (float)World.Tiles.UnitSize.y;

		public Vector3 Position => Transform.z + new Vector3(WidthInTiles / 2, HeightInTiles / 2);

		public override void Draw()
		{
			if (Image != null)
			{
				VG.VG.ImageToSurfaceTransform = WorldTransform * Matrix.Scale(1.0f / World.Tiles.UnitSize.x);
				Image.Draw();
			}
			base.Draw();
		}

		public override void DrawDebug()
		{
			base.DrawDebug();
			if (ImageCover == null)
				return;
			foreach (Vector2 pos in DetermineCover(Position))
			{
				VG.VG.ImageToSurfaceTransform = Parent.WorldTransform * World.GetTileTransform(pos);
				ImageCover.Draw();
			}
		}

		public void Move(Vector3 delta)
		{
			bool change = true;
			int changecount = 0;
			while (change && changecount < 8)
			{
				change = false;

				List<Vector2> covered_pre = DetermineCover(Position);
				List<Vector2> covered_post = DetermineCover(Position + delta);

				foreach (Vector2 uncovered in covered_pre.FindAll(x => !covered_post.Contains(x)))
					InteractTile(this, uncovered, DetermineDirection(uncovered), false, ref delta, ref change);
				foreach (Vector2 covered in covered_post)
					InteractTile(this, covered, DetermineDirection(covered), true, ref delta, ref change);
				if (change)
					changecount++;
			}
			if (changecount >= 8)
				delta = Vector3.Zero;
			Transform *= Matrix.Translation(new Vector3(delta.x, delta.y));
		}

		protected List<Vector2> DetermineCover(Vector3 position)
		{
			List<Vector2> ret = new List<Vector2>();
			int x = (int)(position.x - WidthInTiles / 2);
			int y = (int)(position.y - HeightInTiles / 2);
			float right = position.x + WidthInTiles / 2;
			float top = position.y + HeightInTiles / 2;
			// edge exclusion if the entity fits exactly in a tile
			int width = (int)right - x + (right - (int)right == 0 ? 0 : 1);
			int height = (int)top - y + (top - (int)top == 0 ? 0 : 1);
			for (int u = 0; u < width; u++)
				for (int v = 0; v < height; v++)
				{
					if (0 <= x + u && x + u < (int)World.MapSize.x && 
						0 <= y + v && y + v < (int)World.MapSize.y)
						ret.Add(new Vector2((uint)(x + u), (uint)(y + v)));
				}
			return ret;
		}

		private Direction DetermineDirection(Vector2 target)
		{
			Vector3 p = Position;
			Vector2 pos = new Vector2((uint)p.x, (uint)p.y);
			Direction ret = Direction.None;
			if (target.x < pos.x)
				ret |= Direction.Left;
			else if (target.x > pos.x)
				ret |= Direction.Right;
			if (target.y < pos.y)
				ret |= Direction.Bottom;
			else if (target.y > pos.y)
				ret |= Direction.Top;
			return ret;
		}

		private void InteractTile(Entity entity, Vector2 position, Direction dir, bool covered, ref Vector3 delta, ref bool change)
		{
			ushort tile = World[position].Id;
			if (EntityType.TileInteractions.ContainsKey(tile))
				foreach (TileInteraction i in EntityType.TileInteractions[tile])
					change = i(this, position, dir, covered, ref delta) || change;
			if (World.Tiles[tile].Interactions != null)
				foreach (TileInteraction i in World.Tiles[tile].Interactions)
					change = i(this, position, dir, covered, ref delta) || change;
		}

		private void InteractEntity(Entity other, ref bool change, ref Vector3 delta)
		{
			if (EntityType.EntityInteractions.ContainsKey(other.EntityType.Id))
				foreach(EntityInteraction i in EntityType.EntityInteractions[other.EntityType.Id])
					change = i(this, other, ref delta) || change;
		}
	}
}
