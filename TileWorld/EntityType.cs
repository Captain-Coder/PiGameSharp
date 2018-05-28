using System;
using System.Collections.Generic;
using PiGameSharp.VG;

namespace PiGameSharp.TileWorld
{
	public class EntityType
	{
		public readonly ushort Id;
		public readonly Func<World, Entity> Generator;

		internal Dictionary<ushort, List<EntityInteraction>> EntityInteractions = new Dictionary<ushort, List<EntityInteraction>>();
		internal Dictionary<ushort, List<TileInteraction>> TileInteractions = new Dictionary<ushort, List<TileInteraction>>();

		public EntityType(ushort id, Func<World, Entity> gen)
		{
			Id = id;
			Generator = gen;
		}

		public Entity CreateInstance(World world, Vector3 position)
		{
			Entity ret = Generator(world);
			ret.EntityType = this;
			ret.Parent = world;
			ret.Transform = Matrix.Translation(position + new Vector3(
				(ret.WidthInTiles - (int)ret.WidthInTiles) == 0 ? 0 : (1 - (ret.WidthInTiles - (int)ret.WidthInTiles)) / 2,
				(ret.HeightInTiles - (int)ret.HeightInTiles) == 0 ? 0 : (1 - (ret.HeightInTiles - (int)ret.HeightInTiles)) / 2));
			return ret;
		}

		public void AddInteraction(ushort other, TileInteraction item)
		{
			if (!TileInteractions.ContainsKey(other))
				TileInteractions[other] = new List<TileInteraction>();
			if (TileInteractions[other].Contains(item))
				return;
			else
				TileInteractions[other].Add(item);
		}


		public void AddInteraction(ushort other, EntityInteraction item)
		{
			if (!EntityInteractions.ContainsKey(other))
				EntityInteractions[other] = new List<EntityInteraction>();
			if (EntityInteractions[other].Contains(item))
				return;
			else
				EntityInteractions[other].Add(item);
		}
	}
}
