using System;

namespace PiGameSharp.TileWorld
{
	[Flags]
	public enum Direction : byte
	{
		None = 0,
		Top = 1,
		Bottom = 2,
		Left = 4,
		Right = 8,
	}

	public static class DirectionHelpers
	{
		public static bool HasLeft(this Direction dir) => (dir & Direction.Left) == Direction.Left;
		public static bool HasRight(this Direction dir) => (dir & Direction.Right) == Direction.Right;
		public static bool HasTop(this Direction dir) => (dir & Direction.Top) == Direction.Top;
		public static bool HasBottom(this Direction dir) => (dir & Direction.Bottom) == Direction.Bottom;
		public static bool HasTopLeft(this Direction dir) => (dir & (Direction.Top | Direction.Left)) == (Direction.Top | Direction.Left);
		public static bool HasTopRight(this Direction dir) => (dir & (Direction.Top | Direction.Right)) == (Direction.Top | Direction.Right);
		public static bool HasBottomLeft(this Direction dir) => (dir & (Direction.Bottom | Direction.Left)) == (Direction.Bottom | Direction.Left);
		public static bool HasBottomRight(this Direction dir) => (dir & (Direction.Bottom | Direction.Right)) == (Direction.Bottom | Direction.Right);
	}
}
