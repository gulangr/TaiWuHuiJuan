using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using Game.Views.Legacy.WorldMap;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003EF RID: 1007
public class MapElementInfo : MapElementBase
{
	// Token: 0x06003C83 RID: 15491 RVA: 0x001E7CF8 File Offset: 0x001E5EF8
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = MapElementBase.MapModel.CurrentAreaId != location.AreaId;
				if (flag3)
				{
					result = false;
				}
				else
				{
					MapBlockData block = MapElementBase.MapModel.GetBlockData(location);
					bool flag4 = block == null || !block.Visible;
					if (flag4)
					{
						result = false;
					}
					else
					{
						bool flag5 = !MapElementBase.IsHideCharacterSet && MapElementInfo.CheckExistCharacter(block);
						if (flag5)
						{
							result = true;
						}
						else
						{
							bool flag6 = MapElementInfo.CheckShowWorkMark(location);
							if (flag6)
							{
								result = true;
							}
							else
							{
								bool flag7 = MapElementBase.MapModel.FindBlockSet.Contains(location);
								result = flag7;
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003C84 RID: 15492 RVA: 0x001E7DE0 File Offset: 0x001E5FE0
	public static bool CheckMaybeExistForVillager(MapBlockData block)
	{
		bool flag = MapElementInfo.CheckExistCharacter(block);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			Location location = block.GetLocation();
			result = MapElementInfo.CheckShowWorkMark(location);
		}
		return result;
	}

	// Token: 0x06003C85 RID: 15493 RVA: 0x001E7E10 File Offset: 0x001E6010
	private static bool CheckExistCharacter(MapBlockData block)
	{
		Location location = block.GetLocation();
		HashSet<int> characterSet = block.CharacterSet;
		int anyCount = (characterSet != null) ? characterSet.Count : 0;
		int num = anyCount;
		HashSet<int> fixedCharacterSet = block.FixedCharacterSet;
		anyCount = num + ((fixedCharacterSet != null) ? fixedCharacterSet.Count : 0);
		int num2 = anyCount;
		HashSet<int> enemyCharacterSet = block.EnemyCharacterSet;
		anyCount = num2 + ((enemyCharacterSet != null) ? enemyCharacterSet.Count : 0);
		int num3 = anyCount;
		HashSet<int> infectedCharacterSet = block.InfectedCharacterSet;
		anyCount = num3 + ((infectedCharacterSet != null) ? infectedCharacterSet.Count : 0);
		int num4 = anyCount;
		List<MapTemplateEnemyInfo> templateEnemyList = block.TemplateEnemyList;
		anyCount = num4 + ((templateEnemyList != null) ? templateEnemyList.Count : 0);
		int num5 = anyCount;
		HashSet<int> graveSet = block.GraveSet;
		anyCount = num5 + ((graveSet != null) ? graveSet.Count : 0);
		int num6 = anyCount;
		SortedList<ItemKeyAndDate, int> items = block.Items;
		anyCount = num6 + ((items != null) ? items.Count : 0);
		anyCount += MapElementBase.MapModel.GetAnimalCount(location);
		return anyCount > 0;
	}

	// Token: 0x06003C86 RID: 15494 RVA: 0x001E7ECC File Offset: 0x001E60CC
	private static bool CheckShowWorkMark(Location location)
	{
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		return buildingModel.CheckBlockIsMarked(location) || MapElementBase.MapModel.VillagerWorkLocations.Contains(location);
	}

	// Token: 0x17000626 RID: 1574
	// (get) Token: 0x06003C87 RID: 15495 RVA: 0x001E7F00 File Offset: 0x001E6100
	private bool ShowActor
	{
		get
		{
			return !MapElementBase.IsHideCharacterSet;
		}
	}

	// Token: 0x17000627 RID: 1575
	// (get) Token: 0x06003C88 RID: 15496 RVA: 0x001E7F0A File Offset: 0x001E610A
	protected override bool AutoSetActive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000628 RID: 1576
	// (get) Token: 0x06003C89 RID: 15497 RVA: 0x001E7F0D File Offset: 0x001E610D
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.PopulationMarkAndTaiwu;
		}
	}

	// Token: 0x06003C8A RID: 15498 RVA: 0x001E7F10 File Offset: 0x001E6110
	public override void Scale(float wheel)
	{
		for (int i = 0; i < this.itemLayout.childCount; i++)
		{
			MapElementBase.ScaleReverse(this.itemLayout.GetChild(i), wheel);
		}
	}

	// Token: 0x06003C8B RID: 15499 RVA: 0x001E7F4C File Offset: 0x001E614C
	protected override void OnRefresh()
	{
		bool flag = !this._notPlayEnterAnim;
		if (flag)
		{
			this.TryPlayEnterAnim();
		}
		this.RefreshPlaceMark();
		bool showFind = MapElementBase.MapModel.FindBlockSet.Contains(base.BlockLocation);
		this.imageFind.enabled = showFind;
		bool showActor = this.ShowActor;
		if (showActor)
		{
			this.RequestActorData();
		}
	}

	// Token: 0x06003C8C RID: 15500 RVA: 0x001E7FA9 File Offset: 0x001E61A9
	protected override void OnCollect()
	{
		base.GetComponent<CanvasGroup>().DOKill(false);
	}

	// Token: 0x06003C8D RID: 15501 RVA: 0x001E7FB9 File Offset: 0x001E61B9
	public void SetNotPlayEnterAnim(bool notPlayEnterAnim)
	{
		this._notPlayEnterAnim = notPlayEnterAnim;
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x001E7FC3 File Offset: 0x001E61C3
	private void RequestActorData()
	{
		CommonUtils.QueryJieqingSpecialInteractionUnlocked(delegate(bool unlocked)
		{
			bool flag = !unlocked;
			if (flag)
			{
				MapDomainMethod.AsyncCall.GetMapBlockCharacterCountData(this.Dispatcher, base.BlockLocation, new AsyncMethodCallbackDelegate(this.HandleMapBlockCharacterCountData));
			}
			else
			{
				List<short> sectFilter = CommonUtils.GetJieqingMurderMapOrgFilter();
				MapDomainMethod.AsyncCall.GetMapBlockCharacterCountData(this.Dispatcher, base.BlockLocation, sectFilter, new AsyncMethodCallbackDelegate(this.HandleMapBlockCharacterCountData));
			}
		}, null);
	}

	// Token: 0x06003C8F RID: 15503 RVA: 0x001E7FDC File Offset: 0x001E61DC
	private void HandleMapBlockCharacterCountData(int offset, RawDataPool pool)
	{
		MapBlockCharacterCountData mapBlockCharacterCountData = null;
		Serializer.Deserialize(pool, offset, ref mapBlockCharacterCountData);
		this.RefreshActorCount(mapBlockCharacterCountData);
	}

	// Token: 0x06003C90 RID: 15504 RVA: 0x001E8000 File Offset: 0x001E6200
	private void TryPlayEnterAnim()
	{
		bool activeSelf = base.gameObject.activeSelf;
		if (!activeSelf)
		{
			base.gameObject.SetActive(true);
			CanvasGroup canvas = base.GetComponent<CanvasGroup>();
			canvas.alpha = 0f;
			canvas.DOFade(1f, 0.5f);
		}
	}

	// Token: 0x06003C91 RID: 15505 RVA: 0x001E8050 File Offset: 0x001E6250
	private void RefreshPlaceMark()
	{
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		bool isMarked = buildingModel.CheckBlockIsMarked(base.BlockLocation);
		bool hasWork = MapElementBase.MapModel.VillagerWorkLocations.Contains(base.BlockLocation);
		isMarked = (isMarked || hasWork);
		bool isShow = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(28, true);
		this.imageMark.gameObject.SetActive(isMarked && isShow);
		bool activeSelf = this.imageMark.gameObject.activeSelf;
		if (activeSelf)
		{
			this.imageMark.SetSprite(hasWork ? "map_icon_biaoji_1" : "map_icon_biaoji_0", false, null);
		}
	}

	// Token: 0x06003C92 RID: 15506 RVA: 0x001E80E0 File Offset: 0x001E62E0
	public void RefreshPlaceMarkForDisplayRuleGroup(bool isShow)
	{
		this.imageMark.gameObject.SetActive(isShow);
	}

	// Token: 0x06003C93 RID: 15507 RVA: 0x001E80F8 File Offset: 0x001E62F8
	public void RefreshActorCount(MapBlockCharacterCountData data)
	{
		bool flag = this.Dispatcher == null;
		if (!flag)
		{
			MapBlockCharacterCountData data2 = data;
			int? num;
			if (data2 == null)
			{
				num = null;
			}
			else
			{
				Dictionary<short, int> characterCountDict = data2.CharacterCountDict;
				num = ((characterCountDict != null) ? new int?(characterCountDict.Count) : null);
			}
			int? num2 = num;
			int typeCount = num2.GetValueOrDefault();
			Vector3 startPos = this.itemLayout.localPosition + Vector3.zero.SetY(this.itemLayout.rect.yMin + this.border);
			Vector3 endPos = this.itemLayout.localPosition + Vector3.zero.SetX(this.itemLayout.rect.xMax - this.border);
			Vector3 offsetPos = endPos - startPos;
			int typeItemAmount = 0;
			bool flag2 = typeCount > 0;
			if (flag2)
			{
				Dictionary<EMapElementDisplayRuleItemPoisionType, int> positionTypeAmount = new Dictionary<EMapElementDisplayRuleItemPoisionType, int>();
				positionTypeAmount[EMapElementDisplayRuleItemPoisionType.Left] = 0;
				positionTypeAmount[EMapElementDisplayRuleItemPoisionType.Right] = 0;
				positionTypeAmount[EMapElementDisplayRuleItemPoisionType.BottomMiddle] = 0;
				List<short> typeIdList = (from id in data.CharacterCountDict.Keys
				where data.CharacterCountDict.GetOrDefault(id) > 0 && MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(id, true)
				orderby MapElementDisplayRuleItem.Instance[id].Order
				select id).ToList<short>();
				typeItemAmount = typeIdList.Count;
				Vector2 maxOffsetUnitRight = new Vector2(this.maxOffsetUnitX, this.maxOffsetUnitX / this.itemLayout.rect.width * this.itemLayout.rect.height);
				Vector2 maxOffsetUnitLeft = new Vector2(-this.maxOffsetUnitX, this.maxOffsetUnitX / this.itemLayout.rect.width * this.itemLayout.rect.height);
				int leftTypeAmount = typeIdList.Count((short t) => MapElementDisplayRuleItem.Instance[t].PoisionType == EMapElementDisplayRuleItemPoisionType.Left);
				int rightTypeAmount = typeIdList.Count((short t) => MapElementDisplayRuleItem.Instance[t].PoisionType == EMapElementDisplayRuleItemPoisionType.Right);
				int middleAmount = typeIdList.Count((short t) => MapElementDisplayRuleItem.Instance[t].PoisionType == EMapElementDisplayRuleItemPoisionType.BottomMiddle);
				bool haveMiddle = middleAmount > 0;
				bool leftInMiddle = false;
				bool rightInMiddle = false;
				bool flag3 = typeIdList.Count > 0 && !haveMiddle;
				if (flag3)
				{
					short tempId = typeIdList[0];
					MapElementDisplayRuleItemItem tempConfig = MapElementDisplayRuleItem.Instance[tempId];
					leftInMiddle = (tempConfig.PoisionType == EMapElementDisplayRuleItemPoisionType.Left);
					rightInMiddle = !leftInMiddle;
				}
				for (int i = 0; i < typeItemAmount; i++)
				{
					MapElementInfoCountItem countItem = (i < this.itemLayout.childCount) ? this.itemLayout.GetChild(i).GetComponent<MapElementInfoCountItem>() : Object.Instantiate<MapElementInfoCountItem>(this.itemTemplate, this.itemLayout);
					MapElementDisplayRuleItemItem config = MapElementDisplayRuleItem.Instance[typeIdList[i]];
					Dictionary<EMapElementDisplayRuleItemPoisionType, int> dictionary = positionTypeAmount;
					EMapElementDisplayRuleItemPoisionType poisionType = config.PoisionType;
					int num3 = dictionary[poisionType];
					dictionary[poisionType] = num3 + 1;
					int currentPositionTypeAmount = positionTypeAmount[config.PoisionType];
					Vector3 maxOffsetUnit = Vector3.zero;
					int currentTypeAmount = 1;
					bool flag4 = config.PoisionType == EMapElementDisplayRuleItemPoisionType.Left;
					if (flag4)
					{
						maxOffsetUnit = maxOffsetUnitLeft;
						currentTypeAmount = leftTypeAmount;
						bool flag5 = leftInMiddle;
						if (flag5)
						{
							currentPositionTypeAmount--;
						}
					}
					bool flag6 = config.PoisionType == EMapElementDisplayRuleItemPoisionType.Right;
					if (flag6)
					{
						maxOffsetUnit = maxOffsetUnitRight;
						currentTypeAmount = rightTypeAmount;
						bool flag7 = rightInMiddle;
						if (flag7)
						{
							currentPositionTypeAmount--;
						}
					}
					Vector3 offset = offsetPos * ((float)currentPositionTypeAmount / (float)currentTypeAmount);
					countItem.gameObject.SetActive(true);
					Vector3 maxOffset = maxOffsetUnit * (float)currentPositionTypeAmount;
					offset = Vector2.Min(offset, maxOffset);
					countItem.transform.localPosition = startPos + offset;
					short id2 = typeIdList[i];
					bool showCount = id2 != 27;
					int count = data.CharacterCountDict.GetOrDefault(id2);
					countItem.Refresh(id2, count, showCount);
					countItem.transform.SetAsFirstSibling();
				}
			}
			for (int j = typeItemAmount; j < this.itemLayout.childCount; j++)
			{
				this.itemLayout.GetChild(j).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04002B7C RID: 11132
	[SerializeField]
	private CImage imageMark;

	// Token: 0x04002B7D RID: 11133
	[SerializeField]
	private CImage imageFind;

	// Token: 0x04002B7E RID: 11134
	[SerializeField]
	private MapElementInfoCountItem itemTemplate;

	// Token: 0x04002B7F RID: 11135
	[SerializeField]
	private RectTransform itemLayout;

	// Token: 0x04002B80 RID: 11136
	[SerializeField]
	private float border = 30f;

	// Token: 0x04002B81 RID: 11137
	[SerializeField]
	private float maxOffsetUnitX = 50f;

	// Token: 0x04002B82 RID: 11138
	private bool _notPlayEnterAnim;
}
