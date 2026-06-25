using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Map;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class MapElementContainer
{
	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x06003C0F RID: 15375 RVA: 0x001E5871 File Offset: 0x001E3A71
	public int Count
	{
		get
		{
			return this._elements.Count;
		}
	}

	// Token: 0x06003C10 RID: 15376 RVA: 0x001E5880 File Offset: 0x001E3A80
	private MapElementBase ElementCreate(string prefabKey)
	{
		MapElementBase element = PoolManager.GetObject<MapElementBase>(prefabKey);
		element.BindAsyncMethod();
		element.BindPosGenerator(new MapElementPosGenerator(this._mapRoot.ToPos));
		this.SyncElementPos(element);
		return element;
	}

	// Token: 0x06003C11 RID: 15377 RVA: 0x001E58C2 File Offset: 0x001E3AC2
	private void ElementReturn(string prefabKey, MapElementBase element)
	{
		element.Collect();
		PoolManager.Destroy(prefabKey, element.gameObject);
	}

	// Token: 0x06003C12 RID: 15378 RVA: 0x001E58DC File Offset: 0x001E3ADC
	private void ElementCreateCache(string prefabKey)
	{
		bool flag = this._elements.ContainsKey(prefabKey);
		if (!flag)
		{
			MapElementBase element = this.ElementCreate(prefabKey);
			this._elements.Add(prefabKey, element);
			this._mapRoot.SetDirty();
		}
	}

	// Token: 0x06003C13 RID: 15379 RVA: 0x001E5920 File Offset: 0x001E3B20
	private void ElementReturnCache(string prefabKey)
	{
		MapElementBase element;
		bool flag = !this._elements.TryGetValue(prefabKey, out element);
		if (!flag)
		{
			this._elements.Remove(prefabKey);
			this.ElementReturn(prefabKey, element);
			this._mapRoot.SetDirty();
		}
	}

	// Token: 0x06003C14 RID: 15380 RVA: 0x001E5968 File Offset: 0x001E3B68
	private bool CheckExist(string prefabKey, MapElementContainer.CheckShouldExist check, Location location)
	{
		bool exist = this._elements.ContainsKey(prefabKey);
		bool shouldExist = check(location);
		bool flag = exist == shouldExist;
		bool result;
		if (flag)
		{
			result = shouldExist;
		}
		else
		{
			bool flag2 = shouldExist;
			if (flag2)
			{
				this.ElementCreateCache(prefabKey);
			}
			else
			{
				this.ElementReturnCache(prefabKey);
			}
			result = shouldExist;
		}
		return result;
	}

	// Token: 0x06003C15 RID: 15381 RVA: 0x001E59B8 File Offset: 0x001E3BB8
	private void CheckUpdate(string prefabKey, Location location)
	{
		MapElementContainer.CheckShouldExist check;
		bool flag = !MapElementContainer.ElementChecks.TryGetValue(prefabKey, out check);
		if (!flag)
		{
			bool flag2 = !this.CheckExist(prefabKey, check, location);
			if (!flag2)
			{
				MapElementBase element;
				bool flag3 = !this._elements.TryGetValue(prefabKey, out element);
				if (!flag3)
				{
					element.Refresh(location);
					element.Scale(this._scaleCache);
				}
			}
		}
	}

	// Token: 0x06003C16 RID: 15382 RVA: 0x001E5A1C File Offset: 0x001E3C1C
	private bool CheckElement(string prefabKey)
	{
		MapElementBase element;
		return this._elements.TryGetValue(prefabKey, out element) && element.gameObject.activeSelf;
	}

	// Token: 0x06003C17 RID: 15383 RVA: 0x001E5A4C File Offset: 0x001E3C4C
	private void SyncElementPos(MapElementBase element)
	{
		Transform trans = element.transform;
		RectTransform root = this._mapRoot.Layer2Root(element.Layer, this._locationCache);
		trans.SetParent(root, false);
		trans.localPosition = this._mapRoot.ToPos(this._locationCache) + element.MapOffset;
	}

	// Token: 0x06003C18 RID: 15384 RVA: 0x001E5AAC File Offset: 0x001E3CAC
	internal T ElementAs<T>() where T : MapElementBase
	{
		foreach (MapElementBase element in this._elements.Values)
		{
			T t = element as T;
			bool flag = t != null;
			if (flag)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x06003C19 RID: 15385 RVA: 0x001E5B30 File Offset: 0x001E3D30
	public void SyncSorting()
	{
		int taiwuIndex = 0;
		this.SyncSorting(ref taiwuIndex);
	}

	// Token: 0x06003C1A RID: 15386 RVA: 0x001E5B4C File Offset: 0x001E3D4C
	public void SyncSorting(ref int siblingIndex)
	{
		List<MapElementBase> elements = EasyPool.Get<List<MapElementBase>>();
		elements.Clear();
		elements.AddRange(this._elements.Values);
		CollectionUtils.Sort<MapElementBase>(elements, new Func<MapElementBase, MapElementBase, int>(this.Comparison));
		foreach (MapElementBase element in elements)
		{
			RectTransform root = this._mapRoot.Layer2Root(element.Layer, this._locationCache);
			bool flag = root != element.transform.parent;
			if (flag)
			{
				element.transform.SetParent(root, false);
			}
			Transform transform = (RectTransform)element.transform;
			int num = siblingIndex;
			siblingIndex = num + 1;
			transform.SetSiblingIndex(num);
		}
		EasyPool.Free<List<MapElementBase>>(elements);
	}

	// Token: 0x06003C1B RID: 15387 RVA: 0x001E5C2C File Offset: 0x001E3E2C
	private int Comparison(MapElementBase x, MapElementBase y)
	{
		return (x.Layer < y.Layer) ? -1 : ((x.Layer > y.Layer) ? 1 : 0);
	}

	// Token: 0x06003C1C RID: 15388 RVA: 0x001E5C51 File Offset: 0x001E3E51
	public void BindLocation(IMapRoot mapRoot, Location location)
	{
		this._mapRoot = mapRoot;
		this._locationCache = location;
	}

	// Token: 0x06003C1D RID: 15389 RVA: 0x001E5C62 File Offset: 0x001E3E62
	public void UnbindLocation()
	{
		this._mapRoot = null;
		this._locationCache = Location.Invalid;
	}

	// Token: 0x06003C1E RID: 15390 RVA: 0x001E5C78 File Offset: 0x001E3E78
	public void CollectAll()
	{
		foreach (KeyValuePair<string, MapElementBase> prefab2Element in this._elements)
		{
			this.ElementReturn(prefab2Element.Key, prefab2Element.Value);
		}
		this._elements.Clear();
		this._scaleCache = 1f;
	}

	// Token: 0x06003C1F RID: 15391 RVA: 0x001E5CF4 File Offset: 0x001E3EF4
	public void ScaleAll(float scale)
	{
		this._scaleCache = scale;
		foreach (MapElementBase element in this._elements.Values)
		{
			element.Scale(scale);
		}
	}

	// Token: 0x06003C20 RID: 15392 RVA: 0x001E5D58 File Offset: 0x001E3F58
	public void UpdateAll()
	{
		foreach (string prefabKey in MapElementContainer.ElementChecks.Keys)
		{
			this.CheckUpdate(prefabKey, this._locationCache);
		}
	}

	// Token: 0x06003C21 RID: 15393 RVA: 0x001E5DBC File Offset: 0x001E3FBC
	public void CollectCricket()
	{
		this.ElementReturnCache(MapElementContainer.CricketPrefabKey);
	}

	// Token: 0x06003C22 RID: 15394 RVA: 0x001E5DCB File Offset: 0x001E3FCB
	public void CollectCost()
	{
		this.ElementReturnCache(MapElementContainer.CostPrefabKey);
	}

	// Token: 0x06003C23 RID: 15395 RVA: 0x001E5DDA File Offset: 0x001E3FDA
	public void UpdateCricket()
	{
		this.CheckUpdate(MapElementContainer.CricketPrefabKey, this._locationCache);
	}

	// Token: 0x06003C24 RID: 15396 RVA: 0x001E5DEF File Offset: 0x001E3FEF
	public void UpdateMerchant()
	{
		this.CheckUpdate(MapElementContainer.MerchantPrefabKey, this._locationCache);
	}

	// Token: 0x06003C25 RID: 15397 RVA: 0x001E5E04 File Offset: 0x001E4004
	public void UpdateAdventure()
	{
		this.CheckUpdate(MapElementContainer.AdventureRemakePrefabKey, this._locationCache);
	}

	// Token: 0x06003C26 RID: 15398 RVA: 0x001E5E19 File Offset: 0x001E4019
	public void UpdateSettlementAndStationBtn()
	{
		this.CheckUpdate(MapElementContainer.SettlementBtnPrefabKey, this._locationCache);
		this.CheckUpdate(MapElementContainer.StationBtnPrefabKey, this._locationCache);
	}

	// Token: 0x06003C27 RID: 15399 RVA: 0x001E5E40 File Offset: 0x001E4040
	public void UpdateSettlementBtn()
	{
		this.CheckUpdate(MapElementContainer.SettlementBtnPrefabKey, this._locationCache);
	}

	// Token: 0x06003C28 RID: 15400 RVA: 0x001E5E55 File Offset: 0x001E4055
	public void UpdateStationBtn()
	{
		this.CheckUpdate(MapElementContainer.StationBtnPrefabKey, this._locationCache);
	}

	// Token: 0x06003C29 RID: 15401 RVA: 0x001E5E6A File Offset: 0x001E406A
	public void UpdateCharacter()
	{
		this.CheckUpdate(MapElementContainer.SpecialCharacterPrefabKey, this._locationCache);
	}

	// Token: 0x06003C2A RID: 15402 RVA: 0x001E5E7F File Offset: 0x001E407F
	public void UpdateInfo()
	{
		this.CheckUpdate(MapElementContainer.InfoPrefabKey, this._locationCache);
	}

	// Token: 0x06003C2B RID: 15403 RVA: 0x001E5E94 File Offset: 0x001E4094
	public void UpdateCost()
	{
		this.CheckUpdate(MapElementContainer.CostPrefabKey, this._locationCache);
	}

	// Token: 0x06003C2C RID: 15404 RVA: 0x001E5EA9 File Offset: 0x001E40A9
	public void UpdateTemporaryMark()
	{
		this.CheckUpdate(MapElementContainer.TemporaryMarkPrefabKey, this._locationCache);
	}

	// Token: 0x06003C2D RID: 15405 RVA: 0x001E5EBE File Offset: 0x001E40BE
	public void UpdateDreamBack()
	{
		this.CheckUpdate(MapElementContainer.DreamBackPrefabKey, this._locationCache);
	}

	// Token: 0x06003C2E RID: 15406 RVA: 0x001E5ED3 File Offset: 0x001E40D3
	public void UpdateFulongFlame()
	{
		this.CheckUpdate(MapElementContainer.FulongFlamePrefabKey, this._locationCache);
	}

	// Token: 0x06003C2F RID: 15407 RVA: 0x001E5EE8 File Offset: 0x001E40E8
	public void UpdateZhujianThief()
	{
		this.CheckUpdate(MapElementContainer.ZhujianThiefPrefabKey, this._locationCache);
	}

	// Token: 0x06003C30 RID: 15408 RVA: 0x001E5F00 File Offset: 0x001E4100
	public bool CheckHasCharacter()
	{
		return this.CheckElement(MapElementContainer.SpecialCharacterPrefabKey);
	}

	// Token: 0x06003C31 RID: 15409 RVA: 0x001E5F1D File Offset: 0x001E411D
	public void UpdatePickup()
	{
		this.CheckUpdate(MapElementContainer.PickupPrefabKey, this._locationCache);
	}

	// Token: 0x06003C32 RID: 15410 RVA: 0x001E5F32 File Offset: 0x001E4132
	public void UpdatePickupEffect()
	{
		this.CheckUpdate(MapElementContainer.PickupEffectPrefabKey, this._locationCache);
	}

	// Token: 0x06003C33 RID: 15411 RVA: 0x001E5F47 File Offset: 0x001E4147
	public void UpdateCricketWishEffect()
	{
		this.CheckUpdate(MapElementContainer.CricketWishEffectPrefabKey, this._locationCache);
	}

	// Token: 0x04002B34 RID: 11060
	public static readonly string InfoPrefabKey = "UI_WorldMap_MapElementContainer_InfoPrefabKey";

	// Token: 0x04002B35 RID: 11061
	public static readonly string CostPrefabKey = "UI_WorldMap_MapElementContainer_CostPrefabKey";

	// Token: 0x04002B36 RID: 11062
	public static readonly string CricketPrefabKey = "UI_WorldMap_MapElementContainer_CricketPrefabKey";

	// Token: 0x04002B37 RID: 11063
	public static readonly string MerchantPrefabKey = "UI_WorldMap_MapElementContainer_MerchantPrefabKey";

	// Token: 0x04002B38 RID: 11064
	public static readonly string AdventureRemakePrefabKey = "UI_WorldMap_MapElementContainer_AdventureRemakePrefabKey";

	// Token: 0x04002B39 RID: 11065
	public static readonly string SpecialCharacterPrefabKey = "UI_WorldMap_MapElementContainer_SpecialCharacterPrefabKey";

	// Token: 0x04002B3A RID: 11066
	public static readonly string SettlementBtnPrefabKey = "UI_WorldMap_MapElementContainer_SettlementBtnPrefabKey";

	// Token: 0x04002B3B RID: 11067
	public static readonly string StationBtnPrefabKey = "UI_WorldMap_MapElementContainer_StationBtnPrefabKey";

	// Token: 0x04002B3C RID: 11068
	public static readonly string ExpectPromptPrefabKey = "UI_WorldMap_MapElementContainer_ExpectPromptPrefabKey";

	// Token: 0x04002B3D RID: 11069
	public static readonly string TemporaryMarkPrefabKey = "UI_WorldMap_MapElementContainer_TemporaryMarkPrefabKey";

	// Token: 0x04002B3E RID: 11070
	public static readonly string DreamBackPrefabKey = "UI_WorldMap_MapElementContainer_DreamBackPrefabKey";

	// Token: 0x04002B3F RID: 11071
	public static readonly string FulongFlamePrefabKey = "UI_WorldMap_MapElementContainer_FulongFlamePrefabKey";

	// Token: 0x04002B40 RID: 11072
	public static readonly string EmeiGuidancePrefabKey = "UI_WorldMap_MapElementContainer_EmeiGuidancePrefabKey";

	// Token: 0x04002B41 RID: 11073
	public static readonly string ZhujianThiefPrefabKey = "UI_WorldMap_MapElementContainer_ZhujianThiefPrefabKey";

	// Token: 0x04002B42 RID: 11074
	public static readonly string PickupPrefabKey = "UI_WorldMap_MapElementContainer_PickupPrefabKey";

	// Token: 0x04002B43 RID: 11075
	public static readonly string PickupEffectPrefabKey = "UI_WorldMap_MapElementContainer_PickupEffectPrefabKey";

	// Token: 0x04002B44 RID: 11076
	public static readonly string CricketWishEffectPrefabKey = "UI_WorldMap_MapElementContainer_CricketWishEffectPrefabKey";

	// Token: 0x04002B45 RID: 11077
	private static readonly Dictionary<string, MapElementContainer.CheckShouldExist> ElementChecks = new Dictionary<string, MapElementContainer.CheckShouldExist>
	{
		{
			MapElementContainer.InfoPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementInfo.CheckMaybeExist)
		},
		{
			MapElementContainer.CostPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementCost.CheckMaybeExist)
		},
		{
			MapElementContainer.CricketPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementCricket.CheckMaybeExist)
		},
		{
			MapElementContainer.MerchantPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementMerchant.CheckMaybeExist)
		},
		{
			MapElementContainer.AdventureRemakePrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementAdventureRemake.CheckMaybeExist)
		},
		{
			MapElementContainer.SpecialCharacterPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementCharacter.CheckMaybeExist)
		},
		{
			MapElementContainer.SettlementBtnPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementSettlementBtn.CheckMaybeExist)
		},
		{
			MapElementContainer.StationBtnPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementStationBtn.CheckMaybeExist)
		},
		{
			MapElementContainer.ExpectPromptPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementExpectPrompt.CheckMaybeExist)
		},
		{
			MapElementContainer.TemporaryMarkPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementTemporaryMark.CheckMaybeExist)
		},
		{
			MapElementContainer.DreamBackPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementDreamBack.CheckMaybeExist)
		},
		{
			MapElementContainer.FulongFlamePrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementFulongFlame.CheckMaybeExist)
		},
		{
			MapElementContainer.ZhujianThiefPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementZhujianThief.CheckMaybeExist)
		},
		{
			MapElementContainer.PickupPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementPickup.CheckMaybeExist)
		},
		{
			MapElementContainer.PickupEffectPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementPickupEffect.CheckMaybeExist)
		},
		{
			MapElementContainer.CricketWishEffectPrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementCricketWishEffect.CheckMaybeExist)
		},
		{
			MapElementContainer.EmeiGuidancePrefabKey,
			new MapElementContainer.CheckShouldExist(MapElementEmeiGuidance.CheckMaybeExist)
		}
	};

	// Token: 0x04002B46 RID: 11078
	private readonly Dictionary<string, MapElementBase> _elements = new Dictionary<string, MapElementBase>();

	// Token: 0x04002B47 RID: 11079
	private IMapRoot _mapRoot;

	// Token: 0x04002B48 RID: 11080
	private float _scaleCache = 1f;

	// Token: 0x04002B49 RID: 11081
	private Location _locationCache = Location.Invalid;

	// Token: 0x02001877 RID: 6263
	// (Invoke) Token: 0x0600D6CC RID: 54988
	private delegate bool CheckShouldExist(Location location);
}
