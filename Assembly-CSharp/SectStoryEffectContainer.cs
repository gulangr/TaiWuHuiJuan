using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003D9 RID: 985
public class SectStoryEffectContainer : MonoBehaviour
{
	// Token: 0x170005FD RID: 1533
	// (get) Token: 0x06003B2F RID: 15151 RVA: 0x001DF754 File Offset: 0x001DD954
	// (set) Token: 0x06003B30 RID: 15152 RVA: 0x001DF75C File Offset: 0x001DD95C
	public MapElementPosGenerator PosGenerator { get; set; }

	// Token: 0x170005FE RID: 1534
	// (get) Token: 0x06003B31 RID: 15153 RVA: 0x001DF765 File Offset: 0x001DD965
	protected WorldMapModel MapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x06003B32 RID: 15154 RVA: 0x001DF76C File Offset: 0x001DD96C
	public void TryRefresh(Location location, bool visible)
	{
		GameObject effect;
		bool flag = !this._placedEffects.TryGetValue(location, out effect);
		if (!flag)
		{
			effect.SetActive(visible);
		}
	}

	// Token: 0x06003B33 RID: 15155 RVA: 0x001DF79C File Offset: 0x001DD99C
	public void Clear()
	{
		foreach (GameObject effect in this._placedEffects.Values)
		{
			Object.Destroy(effect);
		}
		this._placedEffects.Clear();
	}

	// Token: 0x06003B34 RID: 15156 RVA: 0x001DF804 File Offset: 0x001DDA04
	public void RefreshAll(ICollection<Location> locations)
	{
		bool flag = locations == null;
		if (!flag)
		{
			List<Location> removedLocations = EasyPool.Get<List<Location>>();
			foreach (Location location in this._placedEffects.Keys)
			{
				bool flag2 = !locations.Contains(location);
				if (flag2)
				{
					removedLocations.Add(location);
				}
			}
			foreach (Location location2 in removedLocations)
			{
				this.Remove(location2);
			}
			EasyPool.Free<List<Location>>(removedLocations);
			foreach (Location location3 in locations)
			{
				bool flag3 = !this._placedEffects.ContainsKey(location3) && location3.AreaId == this.MapModel.ShowingAreaId;
				if (flag3)
				{
					this.Append(location3);
				}
			}
		}
	}

	// Token: 0x06003B35 RID: 15157 RVA: 0x001DF93C File Offset: 0x001DDB3C
	public void Append(Location location)
	{
		MapBlockData blockData = this.MapModel.GetBlockData(location);
		RectTransform blockRoot = this.mapBlockRoot;
		GameObject effect = Object.Instantiate<GameObject>(this.EffectPrefab, blockRoot);
		effect.GetComponent<RectTransform>().anchoredPosition = this.PosGenerator(location);
		effect.SetActive(!this.ensureVisible || blockData.Visible);
		this._placedEffects.Add(location, effect);
	}

	// Token: 0x06003B36 RID: 15158 RVA: 0x001DF9AC File Offset: 0x001DDBAC
	private void Remove(Location location)
	{
		GameObject effect;
		bool flag = !this._placedEffects.TryGetValue(location, out effect);
		if (!flag)
		{
			Object.Destroy(effect);
			this._placedEffects.Remove(location);
		}
	}

	// Token: 0x04002A8E RID: 10894
	public bool ensureVisible = true;

	// Token: 0x04002A8F RID: 10895
	public GameObject EffectPrefab;

	// Token: 0x04002A91 RID: 10897
	public RectTransform mapBlockRoot;

	// Token: 0x04002A92 RID: 10898
	protected readonly Dictionary<Location, GameObject> _placedEffects = new Dictionary<Location, GameObject>();
}
