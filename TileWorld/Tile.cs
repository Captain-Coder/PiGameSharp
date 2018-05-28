using System.Collections.Generic;
using PiGameSharp.VG;

namespace PiGameSharp.TileWorld
{
	public class Tile
	{
		public readonly ushort Id;
		public readonly Image Image;
		public readonly Vector2 TileOrigin;

		internal List<TileInteraction> Interactions;

		public Tile(ushort id, Image img) : this(id, Vector2.Zero, img) { }

		public Tile(ushort id, Vector2 org, Image img)
		{
			Id = id;
			TileOrigin = org;
			Image = img;
		}

		public void AddInteraction(TileInteraction item)
		{
			if (Interactions == null)
				Interactions = new List<TileInteraction>();
			else if (Interactions.Contains(item))
				return;
			Interactions.Add(item);
		}
	}
}