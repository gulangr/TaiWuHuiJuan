using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003D6 RID: 982
public class MapBlockEightDirectionEffectContainer : MonoBehaviour
{
	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x06003B0B RID: 15115 RVA: 0x001DE8E2 File Offset: 0x001DCAE2
	// (set) Token: 0x06003B0C RID: 15116 RVA: 0x001DE8EA File Offset: 0x001DCAEA
	public MapElementPosGenerator PosGenerator { get; set; }

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x06003B0D RID: 15117 RVA: 0x001DE8F3 File Offset: 0x001DCAF3
	// (set) Token: 0x06003B0E RID: 15118 RVA: 0x001DE8FB File Offset: 0x001DCAFB
	public Func<Location, short, int> GetPrefabIndexByLocation { get; set; }

	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x06003B0F RID: 15119 RVA: 0x001DE904 File Offset: 0x001DCB04
	// (set) Token: 0x06003B10 RID: 15120 RVA: 0x001DE90C File Offset: 0x001DCB0C
	public Func<Location, short, int> GetSinglePrefabIndexByLocation { get; set; }

	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x06003B11 RID: 15121 RVA: 0x001DE915 File Offset: 0x001DCB15
	private WorldMapModel MapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x06003B12 RID: 15122 RVA: 0x001DE91C File Offset: 0x001DCB1C
	public bool TryGetEffectPrefabIndexByLocation(Location location, short settlementBlockId, out int index, out bool isSingle)
	{
		isSingle = false;
		index = this.GetPrefabIndexByLocation(location, settlementBlockId);
		bool flag = this.EffectPrefabs.CheckIndex(index);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = this.GetSinglePrefabIndexByLocation == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				index = this.GetSinglePrefabIndexByLocation(location, settlementBlockId);
				bool flag3 = this.SingleEffectPrefabs.CheckIndex(index);
				if (flag3)
				{
					isSingle = true;
					result = true;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06003B13 RID: 15123 RVA: 0x001DE994 File Offset: 0x001DCB94
	public void TurnOffAll()
	{
		foreach (GameObject effect in this._placedEffects.Values)
		{
			effect.SetActive(false);
		}
	}

	// Token: 0x06003B14 RID: 15124 RVA: 0x001DE9F4 File Offset: 0x001DCBF4
	public virtual void TryRefresh(Location location, bool visible, short settlementBlockId = 0)
	{
		GameObject effect;
		bool flag = !this._placedEffects.TryGetValue(new ValueTuple<Location, short>(location, settlementBlockId), out effect);
		if (!flag)
		{
			effect.SetActive(visible);
		}
	}

	// Token: 0x06003B15 RID: 15125 RVA: 0x001DEA28 File Offset: 0x001DCC28
	public void Clear()
	{
		foreach (GameObject effect in this._placedEffects.Values)
		{
			Object.Destroy(effect);
		}
		this._placedEffects.Clear();
	}

	// Token: 0x06003B16 RID: 15126 RVA: 0x001DEA90 File Offset: 0x001DCC90
	public void RefreshAll(ICollection<Location> locations)
	{
		bool flag = locations == null;
		if (!flag)
		{
			List<Location> removedLocations = EasyPool.Get<List<Location>>();
			foreach (ValueTuple<Location, short> valueTuple in this._placedEffects.Keys)
			{
				Location location = valueTuple.Item1;
				bool flag2 = !locations.Contains(location);
				if (flag2)
				{
					removedLocations.Add(location);
				}
			}
			foreach (Location location2 in removedLocations)
			{
				this.Remove(location2, 0);
			}
			EasyPool.Free<List<Location>>(removedLocations);
			foreach (Location location3 in locations)
			{
				this.TryAppend(location3, 0);
			}
		}
	}

	// Token: 0x06003B17 RID: 15127 RVA: 0x001DEBA4 File Offset: 0x001DCDA4
	public void RefreshAll(short areaId, Dictionary<short, HashSet<short>> blockIds)
	{
		bool flag = blockIds == null;
		if (!flag)
		{
			List<Location> removedLocations = EasyPool.Get<List<Location>>();
			foreach (ValueTuple<Location, short> valueTuple in this._placedEffects.Keys)
			{
				Location location = valueTuple.Item1;
				bool flag2 = !blockIds.ContainsKey(location.BlockId);
				if (flag2)
				{
					removedLocations.Add(location);
				}
			}
			foreach (Location location2 in removedLocations)
			{
				this.Remove(location2, 0);
			}
			EasyPool.Free<List<Location>>(removedLocations);
			foreach (short settlementBlockId in blockIds.Keys)
			{
				foreach (short blockId in blockIds[settlementBlockId])
				{
					this.TryAppend(new Location(areaId, blockId), settlementBlockId);
				}
			}
		}
	}

	// Token: 0x06003B18 RID: 15128 RVA: 0x001DED0C File Offset: 0x001DCF0C
	private void TryAppend(Location location, short settlementBlockId = 0)
	{
		int index;
		bool isSingle;
		bool flag = !this._placedEffects.ContainsKey(new ValueTuple<Location, short>(location, settlementBlockId)) && location.AreaId == this.MapModel.ShowingAreaId && this.TryGetEffectPrefabIndexByLocation(location, settlementBlockId, out index, out isSingle);
		if (flag)
		{
			this.Append(location, settlementBlockId, index, isSingle);
		}
	}

	// Token: 0x06003B19 RID: 15129 RVA: 0x001DED60 File Offset: 0x001DCF60
	private void Append(Location location, short settlementBlockId, int index, bool isSingle)
	{
		MapBlockData blockData = this.MapModel.GetBlockData(location);
		RectTransform blockRoot = this.mapBlockRoot;
		GameObject template = isSingle ? this.SingleEffectPrefabs[index] : this.EffectPrefabs[index];
		GameObject effect = Object.Instantiate<GameObject>(template, blockRoot);
		effect.GetComponent<RectTransform>().anchoredPosition = this.PosGenerator(location);
		effect.SetActive(this.GetEffectVisible(blockData));
		this._placedEffects.Add(new ValueTuple<Location, short>(location, settlementBlockId), effect);
	}

	// Token: 0x06003B1A RID: 15130 RVA: 0x001DEDE3 File Offset: 0x001DCFE3
	protected virtual bool GetEffectVisible(MapBlockData blockData)
	{
		return blockData.Visible;
	}

	// Token: 0x06003B1B RID: 15131 RVA: 0x001DEDEC File Offset: 0x001DCFEC
	private void Remove(Location location, short settlementBlockId = 0)
	{
		GameObject effect;
		bool flag = !this._placedEffects.TryGetValue(new ValueTuple<Location, short>(location, settlementBlockId), out effect);
		if (!flag)
		{
			Object.Destroy(effect);
			this._placedEffects.Remove(new ValueTuple<Location, short>(location, settlementBlockId));
		}
	}

	// Token: 0x04002A7A RID: 10874
	public List<GameObject> EffectPrefabs;

	// Token: 0x04002A7B RID: 10875
	public List<GameObject> SingleEffectPrefabs;

	// Token: 0x04002A7C RID: 10876
	public RectTransform mapBlockRoot;

	// Token: 0x04002A80 RID: 10880
	protected readonly Dictionary<ValueTuple<Location, short>, GameObject> _placedEffects = new Dictionary<ValueTuple<Location, short>, GameObject>();
}
