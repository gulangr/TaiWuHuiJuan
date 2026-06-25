using System;
using Redzen.Random;

namespace FrameWork.Tools.Random
{
	// Token: 0x02001034 RID: 4148
	public class CustomRandom : IRandomSource
	{
		// Token: 0x0600BD81 RID: 48513 RVA: 0x005613B8 File Offset: 0x0055F5B8
		public CustomRandom() : this((ulong)DateTime.Now.Ticks)
		{
		}

		// Token: 0x0600BD82 RID: 48514 RVA: 0x005613DA File Offset: 0x0055F5DA
		public CustomRandom(ulong seed)
		{
			this.Reinitialise(seed);
		}

		// Token: 0x0600BD83 RID: 48515 RVA: 0x005613FC File Offset: 0x0055F5FC
		public void Reinitialise(ulong seed)
		{
			int ii = 0;
			int subtraction = ((int)seed == int.MinValue) ? int.MaxValue : Math.Abs((int)seed);
			int mj = 161803398 - subtraction;
			int mk = 1;
			this._seedArray[55] = mj;
			for (int i = 1; i < 55; i++)
			{
				bool flag = (ii += 21) >= 55;
				if (flag)
				{
					ii -= 55;
				}
				this._seedArray[ii] = mk;
				mk = mj - mk;
				bool flag2 = mk < 0;
				if (flag2)
				{
					mk += int.MaxValue;
				}
				mj = this._seedArray[ii];
			}
			for (int j = 1; j < 5; j++)
			{
				for (int k = 1; k < 56; k++)
				{
					int l = k + 30;
					bool flag3 = l >= 55;
					if (flag3)
					{
						l -= 55;
					}
					this._seedArray[k] -= this._seedArray[1 + l];
					bool flag4 = this._seedArray[k] < 0;
					if (flag4)
					{
						this._seedArray[k] += int.MaxValue;
					}
				}
			}
			this._inext = 0;
			this._inextp = 21;
		}

		// Token: 0x0600BD84 RID: 48516 RVA: 0x00561538 File Offset: 0x0055F738
		public int Next()
		{
			int locINext = this._inext;
			int locINextp = this._inextp;
			bool flag = ++locINext >= 56;
			if (flag)
			{
				locINext = 1;
			}
			bool flag2 = ++locINextp >= 56;
			if (flag2)
			{
				locINextp = 1;
			}
			int retVal = this._seedArray[locINext] - this._seedArray[locINextp];
			bool flag3 = retVal == int.MaxValue;
			if (flag3)
			{
				retVal--;
			}
			bool flag4 = retVal < 0;
			if (flag4)
			{
				retVal += int.MaxValue;
			}
			this._seedArray[locINext] = retVal;
			this._inext = locINext;
			this._inextp = locINextp;
			return retVal;
		}

		// Token: 0x0600BD85 RID: 48517 RVA: 0x005615D0 File Offset: 0x0055F7D0
		public int Next(int maxValue)
		{
			bool flag = maxValue < 0;
			if (flag)
			{
				throw new Exception("Random next: maxValue must greater than 0!");
			}
			return (int)(this.NextDouble() * (double)maxValue);
		}

		// Token: 0x0600BD86 RID: 48518 RVA: 0x00561600 File Offset: 0x0055F800
		public int Next(int minValue, int maxValue)
		{
			bool flag = minValue > maxValue;
			if (flag)
			{
				throw new Exception("Random next: minValue cannot greater than maxValue!");
			}
			long range = (long)maxValue - (long)minValue;
			bool flag2 = range <= 2147483647L;
			int result;
			if (flag2)
			{
				result = (int)(this.NextDouble() * (double)range) + minValue;
			}
			else
			{
				result = (int)((long)(this.NextDoubleForLargeRange() * (double)range) + (long)minValue);
			}
			return result;
		}

		// Token: 0x0600BD87 RID: 48519 RVA: 0x00561658 File Offset: 0x0055F858
		public double NextDouble()
		{
			return (double)this.Next() * 4.656612875245797E-10;
		}

		// Token: 0x0600BD88 RID: 48520 RVA: 0x0056167B File Offset: 0x0055F87B
		public int NextInt()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD89 RID: 48521 RVA: 0x00561683 File Offset: 0x0055F883
		public uint NextUInt()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD8A RID: 48522 RVA: 0x0056168B File Offset: 0x0055F88B
		public ulong NextULong()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD8B RID: 48523 RVA: 0x00561693 File Offset: 0x0055F893
		public bool NextBool()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD8C RID: 48524 RVA: 0x0056169B File Offset: 0x0055F89B
		public byte NextByte()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD8D RID: 48525 RVA: 0x005616A3 File Offset: 0x0055F8A3
		public void NextBytes(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD8E RID: 48526 RVA: 0x005616AB File Offset: 0x0055F8AB
		public float NextFloat()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD8F RID: 48527 RVA: 0x005616B3 File Offset: 0x0055F8B3
		public float NextFloatNonZero()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD90 RID: 48528 RVA: 0x005616BB File Offset: 0x0055F8BB
		public double NextDoubleNonZero()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD91 RID: 48529 RVA: 0x005616C3 File Offset: 0x0055F8C3
		public double NextDoubleHighRes()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD92 RID: 48530 RVA: 0x005616CC File Offset: 0x0055F8CC
		private double NextDoubleForLargeRange()
		{
			int result = this.Next();
			bool negative = this.Next() % 2 == 0;
			bool flag = negative;
			if (flag)
			{
				result = -result;
			}
			double d = (double)result;
			d += 2147483646.0;
			return d / 4294967293.0;
		}

		// Token: 0x040091CF RID: 37327
		private const int MBIG = 2147483647;

		// Token: 0x040091D0 RID: 37328
		private const int MSEED = 161803398;

		// Token: 0x040091D1 RID: 37329
		private int _inext;

		// Token: 0x040091D2 RID: 37330
		private int _inextp;

		// Token: 0x040091D3 RID: 37331
		private readonly int[] _seedArray = new int[56];
	}
}
