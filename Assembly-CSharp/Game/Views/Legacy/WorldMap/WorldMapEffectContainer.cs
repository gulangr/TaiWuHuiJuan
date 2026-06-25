using System;
using GameData.Domains.Map;
using UnityEngine;

namespace Game.Views.Legacy.WorldMap
{
	// Token: 0x020009F8 RID: 2552
	public class WorldMapEffectContainer : SectStoryEffectContainer
	{
		// Token: 0x06007DB6 RID: 32182 RVA: 0x003A6464 File Offset: 0x003A4664
		public void Append(Location location, int prefabIndex)
		{
			MapBlockData blockData = base.MapModel.GetBlockData(location);
			RectTransform blockRoot = this.mapBlockRoot;
			GameObject effect = Object.Instantiate<GameObject>(this.EffectPrefabArray[prefabIndex], blockRoot);
			effect.GetComponent<RectTransform>().anchoredPosition = base.PosGenerator(location);
			effect.SetActive(!this.ensureVisible || blockData.Visible);
			this._placedEffects.Add(location, effect);
		}

		// Token: 0x04005FB7 RID: 24503
		public GameObject[] EffectPrefabArray;
	}
}
