using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Migrate;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020003E6 RID: 998
public class MapElementCharacter : MapElementBase
{
	// Token: 0x06003BD7 RID: 15319 RVA: 0x001E3F88 File Offset: 0x001E2188
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
				bool flag3 = location.AreaId != MapElementBase.MapModel.CurrentAreaId;
				if (flag3)
				{
					result = false;
				}
				else
				{
					MapBlockData blockData = MapElementBase.MapModel.GetBlockData(location);
					bool flag4 = blockData == null || !blockData.Visible;
					if (flag4)
					{
						result = false;
					}
					else
					{
						HashSet<int> fixedCharacterSet = blockData.FixedCharacterSet;
						bool hasFixed = fixedCharacterSet != null && fixedCharacterSet.Count > 0;
						bool flag5 = hasFixed;
						result = (flag5 || MapElementBase.MapModel.IsHunterAnimalExist(location) || MapElementBase.MapModel.IsFleeAnimalExist(location) || MapElementBase.MapModel.IsCaravanExist(location) || MapElementBase.MapModel.IsHeavenlyTreeExist(location) || MapElementBase.MapModel.IsLoongExist(location));
					}
				}
			}
		}
		return result;
	}

	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x06003BD8 RID: 15320 RVA: 0x001E4093 File Offset: 0x001E2293
	private int GroupCount
	{
		get
		{
			return this.GetGroupCount(this._currentType);
		}
	}

	// Token: 0x17000616 RID: 1558
	// (get) Token: 0x06003BD9 RID: 15321 RVA: 0x001E40A1 File Offset: 0x001E22A1
	private Game.Components.Avatar.Avatar AvatarNormal
	{
		get
		{
			return this.avatar;
		}
	}

	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x06003BDA RID: 15322 RVA: 0x001E40A9 File Offset: 0x001E22A9
	private CImage AvatarSprite
	{
		get
		{
			return this.avatarSprite;
		}
	}

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x06003BDB RID: 15323 RVA: 0x001E40B1 File Offset: 0x001E22B1
	private CButton ButtonAvatar
	{
		get
		{
			return this.btnAvatarFrame;
		}
	}

	// Token: 0x17000619 RID: 1561
	// (get) Token: 0x06003BDC RID: 15324 RVA: 0x001E40B9 File Offset: 0x001E22B9
	private GameObject RobbedEffect
	{
		get
		{
			return this.goRobbedEffect;
		}
	}

	// Token: 0x1700061A RID: 1562
	// (get) Token: 0x06003BDD RID: 15325 RVA: 0x001E40C1 File Offset: 0x001E22C1
	private int _currentTotalIndex
	{
		get
		{
			return this.GetStartIndex(this._currentType) + this._currentIndex;
		}
	}

	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x06003BDE RID: 15326 RVA: 0x001E40D6 File Offset: 0x001E22D6
	protected override bool AutoSetActive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x06003BDF RID: 15327 RVA: 0x001E40D9 File Offset: 0x001E22D9
	public override Vector2 MapOffset
	{
		get
		{
			return new Vector2(0f, 52f);
		}
	}

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x06003BE0 RID: 15328 RVA: 0x001E40EA File Offset: 0x001E22EA
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.SpecialCharacter;
		}
	}

	// Token: 0x06003BE1 RID: 15329 RVA: 0x001E40ED File Offset: 0x001E22ED
	public override void Scale(float wheel)
	{
		base.ScaleReverse(wheel);
	}

	// Token: 0x06003BE2 RID: 15330 RVA: 0x001E40F8 File Offset: 0x001E22F8
	protected override void OnCreate()
	{
		this.ButtonAvatar.ClearAndAddListener(new Action(this.OnClickAvatar));
		foreach (KeyValuePair<MapElementCharacter.ECharacterType, string> typeNames in MapElementCharacter.MappingTypeNames)
		{
			MapElementCharacter.ECharacterType type = typeNames.Key;
			CButton targetBtn = this.GetBtnByCharacterType(type);
			bool flag = targetBtn == null;
			if (!flag)
			{
				UnityEvent enterEvent = targetBtn.GetComponent<PointerTrigger>().EnterEvent;
				enterEvent.RemoveAllListeners();
				enterEvent.AddListener(delegate()
				{
					this.UpdateGroup(type, false);
				});
				targetBtn.ClearAndAddListener(delegate
				{
					this.UpdateGroup(type, true);
				});
			}
		}
	}

	// Token: 0x06003BE3 RID: 15331 RVA: 0x001E41D8 File Offset: 0x001E23D8
	private CButton GetBtnByCharacterType(MapElementCharacter.ECharacterType targetType)
	{
		CButton targetBtn = null;
		switch (targetType)
		{
		case MapElementCharacter.ECharacterType.Xiangshu:
			return this.btnXiangshu;
		case MapElementCharacter.ECharacterType.Zizhu:
			return this.btnZiZhu;
		case MapElementCharacter.ECharacterType.Special:
			return this.btnSpecial;
		case MapElementCharacter.ECharacterType.Caravan:
			return this.btnCaravan;
		case MapElementCharacter.ECharacterType.Animal:
			return this.btnAnimal;
		case MapElementCharacter.ECharacterType.Beast:
			return this.btnBeast;
		case MapElementCharacter.ECharacterType.Loong:
			return this.btnLoong;
		}
		Debug.LogError(string.Format("Can't Find Target Button ! {0}", targetType));
		return targetBtn;
	}

	// Token: 0x06003BE4 RID: 15332 RVA: 0x001E4270 File Offset: 0x001E2470
	protected override void OnRefresh()
	{
		bool ignoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		this.ButtonAvatar.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnXiangshu.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnZiZhu.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnSpecial.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnCaravan.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnAnimal.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnBeast.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this.btnLoong.GetComponent<CInputEventImage>().IgnoreDrag = ignoreDrag;
		this._fixedGroupData.Clear();
		this._caravanDatas = (from d in MapElementBase.MapModel.CaravanData
		where d.PathInArea.GetCurrLocation() == base.BlockLocation
		select d).Where(delegate(CaravanDisplayData d)
		{
			CaravanExtraData extraData = d.ExtraData;
			bool isRobbed = ((extraData != null) ? new CaravanState?(extraData.StateEnum) : null) == CaravanState.Robbed;
			bool isShowRobbed = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(21, true);
			bool flag6 = isRobbed && isShowRobbed;
			bool result;
			if (flag6)
			{
				result = true;
			}
			else
			{
				MerchantTypeItem merchantTypeConfig = this.GetMerchantTypeConfig(d);
				short key = MapElementDisplayRuleItem.Instance.First((MapElementDisplayRuleItemItem item) => item.Group == 1 && item.MerchantType == merchantTypeConfig.TemplateId).TemplateId;
				bool settingState = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(key, true);
				result = settingState;
			}
			return result;
		}).OrderByDescending(delegate(CaravanDisplayData d)
		{
			CaravanExtraData extraData = d.ExtraData;
			return ((extraData != null) ? new CaravanState?(extraData.StateEnum) : null) == CaravanState.Robbed;
		}).ToArray<CaravanDisplayData>();
		bool showBeast = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(13, true);
		bool showJiao = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(24, true);
		bool showLoong = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(25, true);
		this._animalExist = (MapElementBase.MapModel.IsHunterAnimalExist(base.BlockLocation) && showBeast);
		this._fleeAnimalAvatars.Clear();
		bool flag = MapElementBase.MapModel.IsFleeBeastExist(base.BlockLocation) && showBeast;
		if (flag)
		{
			this._fleeAnimalAvatars.Add("bottom_beast");
		}
		bool flag2 = MapElementBase.MapModel.IsFleeLoongExist(base.BlockLocation) && showJiao;
		if (flag2)
		{
			this._fleeAnimalAvatars.Add("fiveloong_beast");
		}
		this._loongLocations.Clear();
		bool flag3 = showLoong;
		if (flag3)
		{
			this._loongLocations.AddRange(MapElementBase.MapModel.GetLoongLocationData(base.BlockLocation));
		}
		bool flag4;
		if (!MapElementBase.MapModel.IsHeavenlyTreeExist(base.BlockLocation))
		{
			HashSet<int> fixedCharacterSet = base.BlockData.FixedCharacterSet;
			flag4 = (fixedCharacterSet != null && fixedCharacterSet.Count > 0);
		}
		else
		{
			flag4 = true;
		}
		bool flag5 = flag4;
		if (flag5)
		{
			this.DoRequestCharacterDisplayData();
		}
		else
		{
			this.UpdateGroup();
		}
	}

	// Token: 0x06003BE5 RID: 15333 RVA: 0x001E44A3 File Offset: 0x001E26A3
	protected override void OnCollect()
	{
		this._fixedGroupData.Clear();
		this._currentIndex = -1;
		this._currentType = MapElementCharacter.ECharacterType.None;
		this._isNumberBackHover = false;
		this.RefreshGoNumberBack(false);
	}

	// Token: 0x06003BE6 RID: 15334 RVA: 0x001E44CF File Offset: 0x001E26CF
	public void OnNumberBackPointerEnter()
	{
		this._isNumberBackHover = true;
		this.RefreshGoNumberBack(this.ShouldShowNumberBack());
	}

	// Token: 0x06003BE7 RID: 15335 RVA: 0x001E44E6 File Offset: 0x001E26E6
	public void OnNumberBackPointerExit()
	{
		this._isNumberBackHover = false;
		this.RefreshGoNumberBack(false);
	}

	// Token: 0x06003BE8 RID: 15336 RVA: 0x001E44F8 File Offset: 0x001E26F8
	public void OnHoverAvatar(bool isHover)
	{
		this._isHovering = isHover;
		bool flag = this._currentType == MapElementCharacter.ECharacterType.Caravan;
		if (!flag)
		{
			bool flag2 = this._currentType == MapElementCharacter.ECharacterType.Animal;
			if (!flag2)
			{
				bool flag3 = this._currentType == MapElementCharacter.ECharacterType.Beast;
				if (!flag3)
				{
					bool flag4 = this._currentType == MapElementCharacter.ECharacterType.Loong;
					if (!flag4)
					{
						CharacterDisplayData[] group;
						bool flag5 = this._fixedGroupData.TryGetValue(this._currentType, out group);
						if (flag5)
						{
							bool flag6 = group != null && group.CheckIndex(this._currentIndex);
							if (flag6)
							{
								GEvent.OnEvent(UiEvents.OnWorldMapCharacterImpactRangeChanged, EasyPool.Get<ArgumentBox>().Set("charId", group[this._currentIndex].CharacterId).Set("visible", isHover));
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003BE9 RID: 15337 RVA: 0x001E45D0 File Offset: 0x001E27D0
	public void Preview(short itemId)
	{
		this._currentIndex = 0;
		this._fixedGroupData.Clear();
		this._caravanDatas = null;
		this._animalExist = false;
		this._fleeAnimalAvatars.Clear();
		this._loongLocations.Clear();
		switch (itemId)
		{
		case 12:
			this._fixedGroupData[MapElementCharacter.ECharacterType.Special] = new CharacterDisplayData[]
			{
				new CharacterDisplayData
				{
					TemplateId = 919
				}
			};
			break;
		case 13:
			this._animalExist = true;
			this._fleeAnimalAvatars.Add("bottom_beast");
			break;
		case 14:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 6
				}
			};
			break;
		case 15:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 13
				}
			};
			break;
		case 16:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 20
				}
			};
			break;
		case 17:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 27
				}
			};
			break;
		case 18:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 34
				}
			};
			break;
		case 19:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 41
				}
			};
			break;
		case 20:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 48
				}
			};
			break;
		case 21:
			this._caravanDatas = new CaravanDisplayData[]
			{
				new CaravanDisplayData
				{
					MerchantTemplateId = 6,
					ExtraData = new CaravanExtraData
					{
						State = CaravanState.Robbed.ToSbyte()
					}
				}
			};
			break;
		case 22:
			this._fixedGroupData[MapElementCharacter.ECharacterType.Zizhu] = new CharacterDisplayData[]
			{
				new CharacterDisplayData
				{
					TemplateId = 201
				}
			};
			break;
		case 23:
			this._fixedGroupData[MapElementCharacter.ECharacterType.Xiangshu] = new CharacterDisplayData[]
			{
				new CharacterDisplayData
				{
					TemplateId = 39
				}
			};
			break;
		case 24:
			this._fleeAnimalAvatars.Add("fiveloong_beast");
			break;
		case 25:
			this._loongLocations.Add(new LoongLocationData
			{
				TemplateId = 246
			});
			break;
		}
		bool flag = this._fixedGroupData.Count > 0;
		if (flag)
		{
			CharacterDisplayData charData = this._fixedGroupData.Values.First<CharacterDisplayData[]>().First<CharacterDisplayData>();
			charData.AvatarRelatedData = new AvatarRelatedData
			{
				AvatarData = new AvatarData()
			};
			charData.NickNameId = -1;
			charData.ExtraNameTextTemplateId = -1;
		}
		this.UpdateGroup();
		this.RefreshGroup();
	}

	// Token: 0x06003BEA RID: 15338 RVA: 0x001E48B0 File Offset: 0x001E2AB0
	private void DoRequestCharacterDisplayData()
	{
		List<int> charIdList = EasyPool.Get<List<int>>();
		bool flag = base.BlockData.FixedCharacterSet != null;
		if (flag)
		{
			charIdList.AddRange(base.BlockData.FixedCharacterSet);
		}
		SectStoryHeavenlyTreeExtendable heavenlyTreeData = MapElementBase.MapModel.TryGetHeavenlyTreeData(base.BlockLocation);
		bool flag2 = heavenlyTreeData != null;
		if (flag2)
		{
			charIdList.Add(heavenlyTreeData.Id);
		}
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this.Dispatcher, charIdList, new AsyncMethodCallbackDelegate(this.HandlerCharacterDisplayData));
		EasyPool.Free<List<int>>(charIdList);
	}

	// Token: 0x06003BEB RID: 15339 RVA: 0x001E4930 File Offset: 0x001E2B30
	private void HandlerCharacterDisplayData(int offset, RawDataPool pool)
	{
		List<CharacterDisplayData> ret = null;
		Serializer.Deserialize(pool, offset, ref ret);
		bool locationMatch = ret.Any((CharacterDisplayData c) => c.Location.AreaId == base.BlockLocation.AreaId && c.Location.BlockId == base.BlockLocation.BlockId);
		bool flag = !locationMatch;
		if (!flag)
		{
			this._fixedGroupData[MapElementCharacter.ECharacterType.Xiangshu] = ret.Where(new Func<CharacterDisplayData, bool>(this.FilterXiangshuAvatar)).ToArray<CharacterDisplayData>();
			this._fixedGroupData[MapElementCharacter.ECharacterType.Zizhu] = ret.Where(new Func<CharacterDisplayData, bool>(this.FilterPurpleBambooAvatar)).ToArray<CharacterDisplayData>();
			this._fixedGroupData[MapElementCharacter.ECharacterType.Special] = ret.Where(new Func<CharacterDisplayData, bool>(this.FilterNormal)).ToArray<CharacterDisplayData>();
			this.UpdateGroup();
		}
	}

	// Token: 0x06003BEC RID: 15340 RVA: 0x001E49DC File Offset: 0x001E2BDC
	private bool FilterXiangshuAvatar(CharacterDisplayData displayData)
	{
		bool flag = !SingletonObject.getInstance<GlobalSettings>().GetMapElementDisplayRuleItemState(23, true);
		return !flag && Character.Instance.GetItem(displayData.TemplateId).XiangshuType == 1;
	}

	// Token: 0x06003BED RID: 15341 RVA: 0x001E4A20 File Offset: 0x001E2C20
	private bool FilterPurpleBambooAvatar(CharacterDisplayData displayData)
	{
		bool flag = !SingletonObject.getInstance<GlobalSettings>().GetMapElementDisplayRuleItemState(22, true);
		return !flag && Character.Instance.GetItem(displayData.TemplateId).XiangshuType == 3;
	}

	// Token: 0x06003BEE RID: 15342 RVA: 0x001E4A64 File Offset: 0x001E2C64
	private bool FilterNormal(CharacterDisplayData displayData)
	{
		CharacterItem config = Character.Instance.GetItem(displayData.TemplateId);
		bool flag = !SingletonObject.getInstance<GlobalSettings>().GetMapElementDisplayRuleItemState(12, true);
		return !flag && (config.XiangshuType == 0 || config.CreatingType > 0);
	}

	// Token: 0x06003BEF RID: 15343 RVA: 0x001E4AB4 File Offset: 0x001E2CB4
	private MerchantTypeItem GetMerchantTypeConfig(CaravanDisplayData displayData)
	{
		return Config.MerchantType.Instance[Merchant.Instance[(int)displayData.MerchantTemplateId].MerchantType];
	}

	// Token: 0x06003BF0 RID: 15344 RVA: 0x001E4AE8 File Offset: 0x001E2CE8
	private int GetAllGroupCount()
	{
		return MapElementCharacter.MappingTypeNames.Keys.Sum(new Func<MapElementCharacter.ECharacterType, int>(this.GetGroupCount));
	}

	// Token: 0x06003BF1 RID: 15345 RVA: 0x001E4B18 File Offset: 0x001E2D18
	private int GetGroupCount(MapElementCharacter.ECharacterType type)
	{
		if (!true)
		{
		}
		int result;
		switch (type)
		{
		case MapElementCharacter.ECharacterType.Caravan:
		{
			CaravanDisplayData[] caravanDatas = this._caravanDatas;
			result = ((caravanDatas != null) ? caravanDatas.Length : 0);
			break;
		}
		case MapElementCharacter.ECharacterType.Animal:
			result = (this._animalExist ? 1 : 0);
			break;
		case MapElementCharacter.ECharacterType.Beast:
			result = this._fleeAnimalAvatars.Count;
			break;
		case MapElementCharacter.ECharacterType.Loong:
			result = this._loongLocations.Count;
			break;
		default:
		{
			CharacterDisplayData[] group;
			result = (this._fixedGroupData.TryGetValue(type, out group) ? group.Length : 0);
			break;
		}
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003BF2 RID: 15346 RVA: 0x001E4BA4 File Offset: 0x001E2DA4
	private int GetStartIndex(MapElementCharacter.ECharacterType cType)
	{
		int result = 0;
		for (int i = 0; i < MapElementCharacter.CharacterTypeOrder.Length; i++)
		{
			bool flag = MapElementCharacter.CharacterTypeOrder[i] == cType;
			if (flag)
			{
				break;
			}
			result += this.GetGroupCount(MapElementCharacter.CharacterTypeOrder[i]);
		}
		return result;
	}

	// Token: 0x06003BF3 RID: 15347 RVA: 0x001E4BF4 File Offset: 0x001E2DF4
	private void UpdateGroup()
	{
		this.CollectCharacterAmount();
		MapElementCharacter.ECharacterType type = MapElementCharacter.ECharacterType.None;
		bool onlyOneElement = this.GetAllGroupCount() == 1;
		foreach (KeyValuePair<MapElementCharacter.ECharacterType, string> typeName in MapElementCharacter.MappingTypeNames)
		{
			bool exist = this.GetGroupCount(typeName.Key) > 0;
			CButton button = this.GetBtnByCharacterType(typeName.Key);
			bool flag = button == null;
			if (!flag)
			{
				button.gameObject.SetActive(exist && !onlyOneElement);
			}
		}
		for (int i = MapElementCharacter.CharacterTypeOrder.Length - 1; i >= 0; i--)
		{
			bool exist2 = this.GetGroupCount(MapElementCharacter.CharacterTypeOrder[i]) > 0;
			bool flag2 = exist2;
			if (flag2)
			{
				type = MapElementCharacter.CharacterTypeOrder[i];
				break;
			}
		}
		base.gameObject.SetActive(type > MapElementCharacter.ECharacterType.None);
		bool activeSelf = base.gameObject.activeSelf;
		if (activeSelf)
		{
			this.UpdateGroupDefault(type);
		}
		bool flag3 = base.BlockLocation == MapElementBase.MapModel.CurrentLocation;
		if (flag3)
		{
			GEvent.OnEvent(UiEvents.MapCurrentLocationFixedCharacterDataReceived, null);
		}
		this.UpdateAutoMove();
	}

	// Token: 0x06003BF4 RID: 15348 RVA: 0x001E4D48 File Offset: 0x001E2F48
	private void CollectCharacterAmount()
	{
		this._totalAmount = 0;
		foreach (MapElementCharacter.ECharacterType item in MapElementCharacter.CharacterTypeOrder)
		{
			this._totalAmount += this.GetGroupCount(item);
		}
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x001E4D8C File Offset: 0x001E2F8C
	private void UpdateGroup(MapElementCharacter.ECharacterType type, bool autoIncrease = false)
	{
		int groupStartIndex = this.GetStartIndex(type);
		int groupCount = this.GetGroupCount(type);
		bool flag = type != this._currentType;
		if (flag)
		{
			this._currentIndex = 0;
			this._currentType = type;
		}
		else if (autoIncrease)
		{
			this._currentIndex = (this._currentIndex + 1) % this.GroupCount;
		}
		else
		{
			this._currentIndex = Mathf.Clamp(this._currentIndex, 0, this.GroupCount - 1);
		}
		this.RefreshGroup();
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x001E4E09 File Offset: 0x001E3009
	private void UpdateGroupDefault(MapElementCharacter.ECharacterType type)
	{
		this._currentType = type;
		this._currentIndex = this.GroupCount - 1;
		this.RefreshGroup();
	}

	// Token: 0x06003BF7 RID: 15351 RVA: 0x001E4E28 File Offset: 0x001E3028
	private void RefreshGroup()
	{
		CharacterPrefabNumberBackInfo numberBack = this.goNumberBack.GetComponent<CharacterPrefabNumberBackInfo>();
		numberBack.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
		this.RefreshAvatar();
		this.RefreshTextCount(this._totalAmount);
		this.DOKill(false);
		bool flag = this._totalAmount <= 1;
		if (flag)
		{
			this.RefreshTextName();
		}
		this.SyncNumberBackVisibility();
		CharacterPrefabCaravanBackInfo caravan = this.goCaravanBack.GetComponent<CharacterPrefabCaravanBackInfo>();
		caravan.gameObject.SetActive(this._currentType == MapElementCharacter.ECharacterType.Caravan);
		bool activeSelf = caravan.gameObject.activeSelf;
		if (activeSelf)
		{
			CaravanDisplayData caravanData = this._caravanDatas[this._currentIndex];
			List<SettlementDisplayData> settlementList = caravanData.SettlementDisplayDataList;
			int settlementCount = (settlementList != null) ? settlementList.Count : 0;
			string settlementName = (settlementCount > 0) ? MapElementBase.MapModel.GetSettlementName(settlementList.First<SettlementDisplayData>()) : ((caravanData.PathInArea.FullPath.Count > 0) ? MapElementBase.MapModel.GetBlockName(caravanData.PathInArea.GetNextLocation()) : string.Empty);
			MerchantTypeItem merchantTypeConfig = this.GetMerchantTypeConfig(caravanData);
			string merchantName = merchantTypeConfig.Name.SetColor("caravan");
			caravan.txtMeshTarget.text = (settlementName.IsNullOrEmpty() ? merchantName : (merchantName + "->" + settlementName));
		}
		this.imgRing.enabled = (MapElementBase.MapModel.CurrentLocation == base.BlockLocation);
	}

	// Token: 0x06003BF8 RID: 15352 RVA: 0x001E4F96 File Offset: 0x001E3196
	private void RefreshGoNumberBack(bool enable)
	{
		this.goNumberBack.SetActive(enable);
	}

	// Token: 0x06003BF9 RID: 15353 RVA: 0x001E4FA6 File Offset: 0x001E31A6
	private void SyncNumberBackVisibility()
	{
		this.RefreshGoNumberBack(this._isNumberBackHover && this.ShouldShowNumberBack());
	}

	// Token: 0x06003BFA RID: 15354 RVA: 0x001E4FC4 File Offset: 0x001E31C4
	private bool ShouldShowNumberBack()
	{
		bool flag = this._totalAmount > 1;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			MapElementCharacter.ECharacterType currentType = this._currentType;
			bool flag2 = currentType == MapElementCharacter.ECharacterType.Animal || currentType == MapElementCharacter.ECharacterType.Beast || currentType == MapElementCharacter.ECharacterType.Loong || currentType == MapElementCharacter.ECharacterType.Caravan;
			if (flag2)
			{
				result = false;
			}
			else
			{
				CharacterDisplayData[] group;
				bool flag3 = !this._fixedGroupData.TryGetValue(this._currentType, out group) || !group.CheckIndex(this._currentIndex);
				if (flag3)
				{
					result = false;
				}
				else
				{
					CharacterDisplayData displayData = group[this._currentIndex];
					bool flag4 = !Character.Instance[displayData.TemplateId].FixedCharacterShowNameOnMap;
					result = (!flag4 && !NameCenter.GetNameByDisplayData(displayData, false, false).IsNullOrEmpty());
				}
			}
		}
		return result;
	}

	// Token: 0x06003BFB RID: 15355 RVA: 0x001E5088 File Offset: 0x001E3288
	private void RefreshAvatar()
	{
		bool flag = this._currentType == MapElementCharacter.ECharacterType.Caravan;
		if (flag)
		{
			this.SetAvatar(this._caravanDatas[this._currentIndex]);
		}
		else
		{
			this.RobbedEffect.SetActive(false);
			bool flag2 = this._currentType == MapElementCharacter.ECharacterType.Animal;
			if (flag2)
			{
				this.SetAvatar("bottom_profession_liehulaohu");
			}
			else
			{
				bool flag3 = this._currentType == MapElementCharacter.ECharacterType.Beast;
				if (flag3)
				{
					this.SetAvatar(this._fleeAnimalAvatars[this._currentIndex]);
				}
				else
				{
					bool flag4 = this._currentType == MapElementCharacter.ECharacterType.Loong;
					if (flag4)
					{
						this.SetAvatar(this._loongLocations[this._currentIndex]);
					}
					else
					{
						CharacterDisplayData[] group;
						bool flag5 = this._fixedGroupData.TryGetValue(this._currentType, out group);
						if (flag5)
						{
							this.SetAvatar(group[this._currentIndex]);
						}
						else
						{
							this.AvatarNormal.ResetToBlank(false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003BFC RID: 15356 RVA: 0x001E5170 File Offset: 0x001E3370
	private void SetAvatar(CharacterDisplayData displayData)
	{
		bool flag = displayData.TemplateId >= 598 && displayData.TemplateId <= 602;
		if (flag)
		{
			this.SetAvatar("ui9_back_sectpopup_3_bigtree_0");
		}
		else
		{
			this.SetAvatarType(MapElementCharacter.EAvatarType.Normal);
			this.AvatarNormal.Refresh(displayData, true);
		}
	}

	// Token: 0x06003BFD RID: 15357 RVA: 0x001E51CC File Offset: 0x001E33CC
	private void SetAvatar(CaravanDisplayData displayData)
	{
		this.SetAvatarType(MapElementCharacter.EAvatarType.Normal);
		this.AvatarNormal.ResetToBlank(false);
		CaravanExtraData extraData = displayData.ExtraData;
		sbyte state = (extraData != null) ? extraData.State : CaravanState.Normal.ToSbyte();
		MerchantItem merchantConfig = Merchant.Instance[(int)displayData.MerchantTemplateId];
		bool isRobbed = state == CaravanState.Robbed.ToSbyte() && merchantConfig.Enemy >= 0;
		this.RobbedEffect.SetActive(isRobbed);
		string texture = this.GetMerchantTypeConfig(displayData).CaravanAvatar;
		ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/NpcFace/SmallFace/", texture), new Action<Sprite>(this.AvatarNormal.Refresh), null, false);
	}

	// Token: 0x06003BFE RID: 15358 RVA: 0x001E527C File Offset: 0x001E347C
	private void SetAvatar(LoongLocationData locationData)
	{
		this.SetAvatarType(MapElementCharacter.EAvatarType.Normal);
		this.AvatarNormal.ResetToBlank(false);
		CharacterItem characterConfig = Character.Instance[locationData.TemplateId];
		bool flag = characterConfig == null || characterConfig.FixedAvatarName.IsNullOrEmpty();
		if (flag)
		{
			PredefinedLog.Show(11, string.Format("Unexpect loong template {0}", locationData.TemplateId));
		}
		else
		{
			ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/NpcFace/SmallFace/", characterConfig.FixedAvatarName), new Action<Sprite>(this.AvatarNormal.Refresh), null, false);
		}
	}

	// Token: 0x06003BFF RID: 15359 RVA: 0x001E530D File Offset: 0x001E350D
	private void SetAvatar(string spriteName)
	{
		this.SetAvatarType(MapElementCharacter.EAvatarType.Sprite);
		this.AvatarSprite.SetSprite(spriteName, true, null);
	}

	// Token: 0x06003C00 RID: 15360 RVA: 0x001E5327 File Offset: 0x001E3527
	private void SetAvatarType(MapElementCharacter.EAvatarType avatarType)
	{
		this.AvatarNormal.gameObject.SetActive(avatarType == MapElementCharacter.EAvatarType.Normal);
		this.AvatarSprite.gameObject.SetActive(avatarType == MapElementCharacter.EAvatarType.Sprite);
	}

	// Token: 0x06003C01 RID: 15361 RVA: 0x001E5354 File Offset: 0x001E3554
	private void OnClickAvatar()
	{
		WorldMapModel mapModel = MapElementBase.MapModel;
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			bool flag = mapModel.CurrentAreaId != base.BlockLocation.AreaId || mapModel.CurrentBlockId != base.BlockLocation.BlockId;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
			}
		}
	}

	// Token: 0x06003C02 RID: 15362 RVA: 0x001E53C8 File Offset: 0x001E35C8
	private void RefreshTextName()
	{
		bool flag = this._currentType == MapElementCharacter.ECharacterType.Animal;
		if (flag)
		{
			this.RefreshTextName(string.Empty);
		}
		else
		{
			bool flag2 = this._currentType == MapElementCharacter.ECharacterType.Beast;
			if (flag2)
			{
				this.RefreshTextName(string.Empty);
			}
			else
			{
				bool flag3 = this._currentType == MapElementCharacter.ECharacterType.Loong;
				if (flag3)
				{
					this.RefreshTextName(string.Empty);
				}
				else
				{
					CharacterDisplayData[] group;
					bool flag4 = this._fixedGroupData.TryGetValue(this._currentType, out group);
					if (flag4)
					{
						this.RefreshTextName(group[this._currentIndex]);
					}
					else
					{
						this.RefreshTextName(string.Empty);
					}
				}
			}
		}
	}

	// Token: 0x06003C03 RID: 15363 RVA: 0x001E5460 File Offset: 0x001E3660
	private void RefreshTextName(CharacterDisplayData displayData)
	{
		bool fixedCharacterShowNameOnMap = Character.Instance[displayData.TemplateId].FixedCharacterShowNameOnMap;
		if (fixedCharacterShowNameOnMap)
		{
			this.RefreshTextName(NameCenter.GetNameByDisplayData(displayData, false, false));
		}
		else
		{
			this.RefreshTextEmpty();
		}
	}

	// Token: 0x06003C04 RID: 15364 RVA: 0x001E54A0 File Offset: 0x001E36A0
	private void RefreshTextName(string nameText)
	{
		bool flag = nameText.IsNullOrEmpty();
		if (flag)
		{
			this.RefreshTextEmpty();
		}
		else
		{
			CharacterPrefabNumberBackInfo numberBack = this.goNumberBack.GetComponent<CharacterPrefabNumberBackInfo>();
			TextMeshProUGUI current = numberBack.txtMeshCurrent;
			TextMeshProUGUI line = numberBack.txtMeshSplitter;
			TextMeshProUGUI max = numberBack.txtMeshMax;
			current.text = nameText;
			line.enabled = false;
			max.enabled = false;
		}
		this.SyncNumberBackVisibility();
	}

	// Token: 0x06003C05 RID: 15365 RVA: 0x001E5505 File Offset: 0x001E3705
	private void RefreshTextEmpty()
	{
		this.SyncNumberBackVisibility();
	}

	// Token: 0x06003C06 RID: 15366 RVA: 0x001E5510 File Offset: 0x001E3710
	private void RefreshTextCount(int count)
	{
		CharacterPrefabNumberBackInfo numberBack = this.goNumberBack.GetComponent<CharacterPrefabNumberBackInfo>();
		TextMeshProUGUI current = numberBack.txtMeshCurrent;
		TextMeshProUGUI line = numberBack.txtMeshSplitter;
		TextMeshProUGUI max = numberBack.txtMeshMax;
		current.enabled = true;
		line.enabled = true;
		max.enabled = true;
		current.text = string.Format("{0}", this._currentTotalIndex + 1);
		max.text = string.Format("{0}", count);
	}

	// Token: 0x06003C07 RID: 15367 RVA: 0x001E558C File Offset: 0x001E378C
	private void UpdateAutoMove()
	{
		bool flag = this.IsFuyuHilt();
		if (flag)
		{
			this.SmoothMove();
		}
	}

	// Token: 0x06003C08 RID: 15368 RVA: 0x001E55AC File Offset: 0x001E37AC
	private bool IsFuyuHilt()
	{
		CharacterDisplayData[] group;
		bool flag = this._fixedGroupData.TryGetValue(MapElementCharacter.ECharacterType.Special, out group) && group.Length == 1;
		return flag && group[0].TemplateId == 922 && this.GetAllGroupCount() == 1;
	}

	// Token: 0x06003C09 RID: 15369 RVA: 0x001E55FC File Offset: 0x001E37FC
	private void SmoothMove()
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		bool flag = mapModel.FuyuHiltMovePath == null;
		if (!flag)
		{
			int curIndex = mapModel.FuyuHiltMovePath.IndexOf(base.BlockLocation.BlockId);
			bool flag2 = curIndex < 1;
			if (!flag2)
			{
				Location location = new Location(base.BlockLocation.AreaId, mapModel.FuyuHiltMovePath[curIndex - 1]);
				Vector2 srcPos = this.PosGenerator(location);
				base.transform.localPosition = srcPos;
				Vector2 dstPos = this.PosGenerator(base.BlockLocation);
				base.transform.DOLocalMove(dstPos, 0.4f, false);
			}
		}
	}

	// Token: 0x04002B17 RID: 11031
	private const string HunterAnimalAvatar = "bottom_profession_liehulaohu";

	// Token: 0x04002B18 RID: 11032
	private const string FleeBeastAvatar = "bottom_beast";

	// Token: 0x04002B19 RID: 11033
	private const string FleeLoongAvatar = "fiveloong_beast";

	// Token: 0x04002B1A RID: 11034
	private static readonly Dictionary<MapElementCharacter.ECharacterType, string> MappingTypeNames = new Dictionary<MapElementCharacter.ECharacterType, string>
	{
		{
			MapElementCharacter.ECharacterType.Xiangshu,
			"Xiangshu"
		},
		{
			MapElementCharacter.ECharacterType.Zizhu,
			"Zizhu"
		},
		{
			MapElementCharacter.ECharacterType.Special,
			"Special"
		},
		{
			MapElementCharacter.ECharacterType.Caravan,
			"Caravan"
		},
		{
			MapElementCharacter.ECharacterType.Animal,
			"Animal"
		},
		{
			MapElementCharacter.ECharacterType.Beast,
			"Beast"
		},
		{
			MapElementCharacter.ECharacterType.Loong,
			"Loong"
		}
	};

	// Token: 0x04002B1B RID: 11035
	private static readonly MapElementCharacter.ECharacterType[] CharacterTypeOrder = new MapElementCharacter.ECharacterType[]
	{
		MapElementCharacter.ECharacterType.Xiangshu,
		MapElementCharacter.ECharacterType.Zizhu,
		MapElementCharacter.ECharacterType.Caravan,
		MapElementCharacter.ECharacterType.Animal,
		MapElementCharacter.ECharacterType.Beast,
		MapElementCharacter.ECharacterType.Loong,
		MapElementCharacter.ECharacterType.Special
	};

	// Token: 0x04002B1C RID: 11036
	[SerializeField]
	private CImage imgRing;

	// Token: 0x04002B1D RID: 11037
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x04002B1E RID: 11038
	[SerializeField]
	private GameObject goNumberBack;

	// Token: 0x04002B1F RID: 11039
	[SerializeField]
	private GameObject goCaravanBack;

	// Token: 0x04002B20 RID: 11040
	[SerializeField]
	private CButton btnXiangshu;

	// Token: 0x04002B21 RID: 11041
	[SerializeField]
	private CButton btnZiZhu;

	// Token: 0x04002B22 RID: 11042
	[SerializeField]
	private CButton btnSpecial;

	// Token: 0x04002B23 RID: 11043
	[SerializeField]
	private CButton btnCaravan;

	// Token: 0x04002B24 RID: 11044
	[SerializeField]
	private CButton btnAnimal;

	// Token: 0x04002B25 RID: 11045
	[SerializeField]
	private CImage avatarSprite;

	// Token: 0x04002B26 RID: 11046
	[SerializeField]
	private CButton btnBeast;

	// Token: 0x04002B27 RID: 11047
	[SerializeField]
	private CButton btnLoong;

	// Token: 0x04002B28 RID: 11048
	[SerializeField]
	private GameObject goRobbedEffect;

	// Token: 0x04002B29 RID: 11049
	[SerializeField]
	private CButton btnAvatarFrame;

	// Token: 0x04002B2A RID: 11050
	private readonly Dictionary<MapElementCharacter.ECharacterType, CharacterDisplayData[]> _fixedGroupData = new Dictionary<MapElementCharacter.ECharacterType, CharacterDisplayData[]>();

	// Token: 0x04002B2B RID: 11051
	private CaravanDisplayData[] _caravanDatas;

	// Token: 0x04002B2C RID: 11052
	private bool _animalExist;

	// Token: 0x04002B2D RID: 11053
	private readonly List<string> _fleeAnimalAvatars = new List<string>();

	// Token: 0x04002B2E RID: 11054
	private readonly List<LoongLocationData> _loongLocations = new List<LoongLocationData>();

	// Token: 0x04002B2F RID: 11055
	private int _currentIndex = 0;

	// Token: 0x04002B30 RID: 11056
	private int _totalAmount = 0;

	// Token: 0x04002B31 RID: 11057
	private MapElementCharacter.ECharacterType _currentType;

	// Token: 0x04002B32 RID: 11058
	private bool _isNumberBackHover;

	// Token: 0x04002B33 RID: 11059
	private bool _isHovering = false;

	// Token: 0x02001872 RID: 6258
	private enum ECharacterType
	{
		// Token: 0x0400AE99 RID: 44697
		None,
		// Token: 0x0400AE9A RID: 44698
		Xiangshu,
		// Token: 0x0400AE9B RID: 44699
		Zizhu,
		// Token: 0x0400AE9C RID: 44700
		Special,
		// Token: 0x0400AE9D RID: 44701
		Caravan,
		// Token: 0x0400AE9E RID: 44702
		Animal,
		// Token: 0x0400AE9F RID: 44703
		Beast,
		// Token: 0x0400AEA0 RID: 44704
		Loong
	}

	// Token: 0x02001873 RID: 6259
	private enum EAvatarType
	{
		// Token: 0x0400AEA2 RID: 44706
		Normal,
		// Token: 0x0400AEA3 RID: 44707
		Sprite
	}
}
