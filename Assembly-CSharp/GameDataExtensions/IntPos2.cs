using System;
using System.Runtime.CompilerServices;
using GameData.Common.Algorithm;

namespace GameDataExtensions
{
	// Token: 0x020006D4 RID: 1748
	public struct IntPos2 : IAStarPos<IntPos2>, IEquatable<IntPos2>
	{
		// Token: 0x0600535C RID: 21340 RVA: 0x00269F30 File Offset: 0x00268130
		public static implicit operator IntPos2([TupleElementNames(new string[]
		{
			"x",
			"y"
		})] ValueTuple<int, int> pos)
		{
			return new IntPos2
			{
				X = pos.Item1,
				Y = pos.Item2
			};
		}

		// Token: 0x0600535D RID: 21341 RVA: 0x00269F60 File Offset: 0x00268160
		public static IntPos2 operator +(IntPos2 left, IntPos2 right)
		{
			return new IntPos2
			{
				X = left.X + right.X,
				Y = left.Y + right.Y
			};
		}

		// Token: 0x0600535E RID: 21342 RVA: 0x00269FA4 File Offset: 0x002681A4
		public bool Equals(IntPos2 other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		// Token: 0x0600535F RID: 21343 RVA: 0x00269FD8 File Offset: 0x002681D8
		public int GetManhattanDistance(IntPos2 other)
		{
			return Math.Abs(this.X - other.X) + Math.Abs(this.Y - other.Y);
		}

		// Token: 0x06005360 RID: 21344 RVA: 0x0026A010 File Offset: 0x00268210
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is IntPos2)
			{
				IntPos2 pos = (IntPos2)obj;
				result = this.Equals(pos);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06005361 RID: 21345 RVA: 0x0026A03C File Offset: 0x0026823C
		public override int GetHashCode()
		{
			return this.X * 397 ^ this.Y;
		}

		// Token: 0x06005362 RID: 21346 RVA: 0x0026A064 File Offset: 0x00268264
		public static bool operator ==(IntPos2 left, IntPos2 right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x0026A080 File Offset: 0x00268280
		public static bool operator !=(IntPos2 left, IntPos2 right)
		{
			return !left.Equals(right);
		}

		// Token: 0x04003872 RID: 14450
		public int X;

		// Token: 0x04003873 RID: 14451
		public int Y;

		// Token: 0x04003874 RID: 14452
		public static readonly IntPos2[] Directions = new IntPos2[]
		{
			new ValueTuple<int, int>(0, 1),
			new ValueTuple<int, int>(0, -1),
			new ValueTuple<int, int>(-1, 0),
			new ValueTuple<int, int>(1, 0)
		};
	}
}
