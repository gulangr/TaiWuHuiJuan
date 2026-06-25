using System;
using GameData.Combat.Cricket;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ACA RID: 2762
	public class CricketCombatBridge : ICricketExternalBridge
	{
		// Token: 0x06008827 RID: 34855 RVA: 0x003F3254 File Offset: 0x003F1454
		public bool CheckPercentProb(int percentProb)
		{
			return Random.Range(0, 100) < percentProb;
		}

		// Token: 0x06008828 RID: 34856 RVA: 0x003F3274 File Offset: 0x003F1474
		public int Next(int maxValue)
		{
			return Random.Range(0, maxValue);
		}
	}
}
