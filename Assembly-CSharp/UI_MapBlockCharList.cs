using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003DC RID: 988
public class UI_MapBlockCharList : UIBase
{
	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x06003B4F RID: 15183 RVA: 0x001E0417 File Offset: 0x001DE617
	private bool HasNormal
	{
		get
		{
			return this._normalCharList.Count > 0;
		}
	}

	// Token: 0x17000601 RID: 1537
	// (get) Token: 0x06003B50 RID: 15184 RVA: 0x001E0427 File Offset: 0x001DE627
	private bool HasEnemy
	{
		get
		{
			return this._normalEnemyList.Count > 0 || this._randomEnemyList.Count > 0 || this._animalEnemyList.Count > 0;
		}
	}

	// Token: 0x17000602 RID: 1538
	// (get) Token: 0x06003B51 RID: 15185 RVA: 0x001E0456 File Offset: 0x001DE656
	private bool HasGrave
	{
		get
		{
			return this._graveList.Count > 0;
		}
	}

	// Token: 0x17000603 RID: 1539
	// (get) Token: 0x06003B52 RID: 15186 RVA: 0x001E0466 File Offset: 0x001DE666
	private bool HasSpecial
	{
		get
		{
			return this._specialCharList.Count > 0;
		}
	}

	// Token: 0x17000604 RID: 1540
	// (get) Token: 0x06003B53 RID: 15187 RVA: 0x001E0476 File Offset: 0x001DE676
	private bool HasInformation
	{
		get
		{
			return this._characterDisplayDataWithInfoList.Count > 0;
		}
	}

	// Token: 0x17000605 RID: 1541
	// (get) Token: 0x06003B54 RID: 15188 RVA: 0x001E0486 File Offset: 0x001DE686
	public static bool IsFocusOnSearchInputField
	{
		get
		{
			return UI_MapBlockCharList._staticSearchInputField && UI_MapBlockCharList._staticSearchInputField.isFocused;
		}
	}

	// Token: 0x06003B55 RID: 15189 RVA: 0x001E04A4 File Offset: 0x001DE6A4
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitRefers();
		this._isWartingData = false;
		this._rectTrans = (RectTransform)base.transform;
		bool flag = this._originPosition == Vector2.zero;
		if (flag)
		{
			this._originPosition = this._rectTrans.anchoredPosition;
		}
		this._mapModel = SingletonObject.getInstance<WorldMapModel>();
		this.NeedDataListenerId = true;
		UI_MapBlockCharList._staticSearchInputField = this._searchInputField;
		UI_MapBlockCharList._staticSearchInputField.onValueChanged.RemoveAllListeners();
		UI_MapBlockCharList._staticSearchInputField.onValueChanged.AddListener(delegate(string value)
		{
			bool flag2 = CommonUtils.FixToShowAbleString(ref value, UI_MapBlockCharList._staticSearchInputField.textComponent.font);
			if (flag2)
			{
				UI_MapBlockCharList._staticSearchInputField.SetTextWithoutNotify(value);
			}
			this.UpdateCharList(true);
		});
		UI_MapBlockCharList._staticSearchInputField.SetTextWithoutNotify(string.Empty);
		if (argsBox != null)
		{
			argsBox.Get("skipRefreshData", out this._skipRefreshData);
		}
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			bool skipRefreshData = this._skipRefreshData;
			if (skipRefreshData)
			{
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
				{
					this._skipRefreshData = false;
					this.<OnInit>g__InitRefresh|63_1();
				});
			}
			else
			{
				this.<OnInit>g__InitRefresh|63_1();
			}
		}));
		this.Element.ShowAfterRefresh();
		this._uiAnim = base.GetComponent<UIAnim>();
		this._uiAnim.Init(this._originPosition.SetX(0f), this._originPosition);
	}

	// Token: 0x06003B56 RID: 15190 RVA: 0x001E05D4 File Offset: 0x001DE7D4
	private void Awake()
	{
		this.InitRefers();
		this._charScroll.SrcPrefab.gameObject.SetActive(false);
		this._charScroll.OnItemHide = new Action<Refers>(this.OnItemHide);
		this._togGroup.InitPreOnToggle(-1);
		this._togGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
		{
			this.UpdateCharList(togOld != null);
			bool flag = togNew;
			if (flag)
			{
				this._lastTogKey = togNew.Key;
			}
		};
		this._needUpdateOnFocus = true;
		GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
	}

	// Token: 0x06003B57 RID: 15191 RVA: 0x001E065A File Offset: 0x001DE85A
	private void OnDestroy()
	{
		GEvent.Remove(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
	}

	// Token: 0x06003B58 RID: 15192 RVA: 0x001E0678 File Offset: 0x001DE878
	private void OnEnable()
	{
		GEvent.Add(UiEvents.WorldMapShowInfoBlockChange, new GEvent.Callback(this.OnSelectedBlockChange));
		GEvent.Add(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.OnVillagerWorkDataChange));
		GEvent.Add(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.OnSavingWorldStateChange));
		GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		GEvent.Add(UiEvents.OnUpdateCaravanBlockCharData, new GEvent.Callback(this.OnUpdateCaravanBlockCharData));
		GEvent.Add(UiEvents.HideMapBlockCharList, new GEvent.Callback(this.HideMapBlockCharList));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Add(UiEvents.AnimalPlaceDataChange, new GEvent.Callback(this.OnAnimalPlaceDataChange));
		GEvent.Add(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.OnMapBlockDataChange));
		GEvent.Add(UiEvents.OnSetMapBlockCharListTog, new GEvent.Callback(this.OnSetMapBlockCharListTog));
		GEvent.Add(UiEvents.NickNameChanged, new GEvent.Callback(this.OnSomeoneNickNameChanged));
		GEvent.Add(UiEvents.OnMapBlockCharCustomInfoChanged, new GEvent.Callback(this.OnMapBlockCharCustomInfoChanged));
		GEvent.Add(UiEvents.OnMapBlockCharCustomButtonChanged, new GEvent.Callback(this.OnMapBlockCharCustomButtonChanged));
		this._uiAnim.PlayHideAnimation(null, false);
	}

	// Token: 0x06003B59 RID: 15193 RVA: 0x001E0800 File Offset: 0x001DEA00
	private void OnDisable()
	{
		this._charScroll.SetDataCount(0);
		GEvent.Remove(UiEvents.WorldMapShowInfoBlockChange, new GEvent.Callback(this.OnSelectedBlockChange));
		GEvent.Remove(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.OnVillagerWorkDataChange));
		GEvent.Remove(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.OnSavingWorldStateChange));
		GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		GEvent.Remove(UiEvents.OnUpdateCaravanBlockCharData, new GEvent.Callback(this.OnUpdateCaravanBlockCharData));
		GEvent.Remove(UiEvents.HideMapBlockCharList, new GEvent.Callback(this.HideMapBlockCharList));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Remove(UiEvents.AnimalPlaceDataChange, new GEvent.Callback(this.OnAnimalPlaceDataChange));
		GEvent.Remove(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.OnMapBlockDataChange));
		GEvent.Remove(UiEvents.OnSetMapBlockCharListTog, new GEvent.Callback(this.OnSetMapBlockCharListTog));
		GEvent.Remove(UiEvents.NickNameChanged, new GEvent.Callback(this.OnSomeoneNickNameChanged));
		GEvent.Remove(UiEvents.OnMapBlockCharCustomInfoChanged, new GEvent.Callback(this.OnMapBlockCharCustomInfoChanged));
		GEvent.Remove(UiEvents.OnMapBlockCharCustomButtonChanged, new GEvent.Callback(this.OnMapBlockCharCustomButtonChanged));
	}

	// Token: 0x06003B5A RID: 15194 RVA: 0x001E0987 File Offset: 0x001DEB87
	private void HideMapBlockCharList(ArgumentBox argbox)
	{
		this._charScroll.SetDataCount(0);
		this.PlayHideAnim();
		this.Element.Hide(true);
		this.UpdateCountLabel(0);
	}

	// Token: 0x06003B5B RID: 15195 RVA: 0x001E09B4 File Offset: 0x001DEBB4
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b2 = type;
			if (b2 == 1)
			{
				bool flag = notification.DomainId == 19;
				if (flag)
				{
					bool flag2 = notification.MethodId == 69;
					if (flag2)
					{
						this._loongInfos = new List<LoongInfo>();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._loongInfos);
					}
				}
				else
				{
					bool flag3 = notification.DomainId == 4;
					if (flag3)
					{
						bool flag4 = notification.MethodId == 51;
						if (flag4)
						{
							List<GraveDisplayData> displayDataList = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
							foreach (GraveDisplayData data in displayDataList)
							{
								this._graveDataDict[data.Id] = data;
							}
							this._graveList.Sort(new Comparison<int>(this.GraveCompare));
							this.InitTogGroup();
						}
						else
						{
							bool flag5 = notification.MethodId == 48;
							if (flag5)
							{
								List<CharacterDisplayData> displayDataList2 = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList2);
								foreach (CharacterDisplayData data2 in displayDataList2)
								{
									this._charDataDict[data2.CharacterId] = data2;
									this.HandleLegendaryBookOwner(data2);
								}
								this.OnCharacterDataChanged();
							}
						}
					}
					else
					{
						bool flag6 = notification.DomainId == 18;
						if (flag6)
						{
							bool flag7 = notification.MethodId == 12;
							if (flag7)
							{
								List<CharacterDisplayDataWithInfo> displayDataList3 = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList3);
								this._characterDisplayDataWithInfoList.Clear();
								bool flag8 = displayDataList3 != null;
								if (flag8)
								{
									foreach (CharacterDisplayDataWithInfo characterDisplayDataWithInfo in displayDataList3)
									{
										this._characterDisplayDataWithInfoList.Add(characterDisplayDataWithInfo);
										CharacterDisplayData data3 = characterDisplayDataWithInfo.CharacterDisplayData;
										this._charDataDict[data3.CharacterId] = data3;
										this.HandleLegendaryBookOwner(data3);
									}
								}
								this._characterDisplayDataWithInfoList.Sort((CharacterDisplayDataWithInfo a, CharacterDisplayDataWithInfo b) => this.CharCompare(a.CharacterDisplayData.CharacterId, b.CharacterDisplayData.CharacterId));
								this.OnCharacterDataChanged();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003B5C RID: 15196 RVA: 0x001E0CC8 File Offset: 0x001DEEC8
	private void HandleLegendaryBookOwner(CharacterDisplayData data)
	{
		bool flag = data.LegendaryBookOwnerState == 3 && this._normalCharList.Contains(data.CharacterId);
		if (flag)
		{
			this._normalCharList.Remove(data.CharacterId);
			this._legendaryBookOwnerEnemyList.Add(data.CharacterId);
		}
	}

	// Token: 0x06003B5D RID: 15197 RVA: 0x001E0D20 File Offset: 0x001DEF20
	private void OnCharacterDataChanged()
	{
		bool flag = this._normalCharList.All((int id) => this._charDataDict.ContainsKey(id));
		if (flag)
		{
			this._normalCharList.Sort(new Comparison<int>(this.CharCompare));
		}
		bool flag2 = this._infectedList.All((int id) => this._charDataDict.ContainsKey(id));
		if (flag2)
		{
			this._infectedList.Sort(new Comparison<int>(this.CharCompare));
		}
		this._normalEnemyList.Clear();
		this._normalEnemyList.AddRange(this._infectedList);
		this._normalEnemyList.AddRange(this._legendaryBookOwnerEnemyList);
		bool flag3 = this._graveList.Count == 0;
		if (flag3)
		{
			this.InitTogGroup();
		}
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x001E0DE0 File Offset: 0x001DEFE0
	private void UpdateDataFromSvr(bool forceUpdate = false, Action onFinish = null)
	{
		bool flag = !forceUpdate && (this._skipRefreshData || this._isUpdatingData);
		if (!flag)
		{
			this._isUpdatingData = true;
			bool flag2 = this._block == null;
			if (flag2)
			{
				this.InitDisplay(false);
			}
			else
			{
				AsyncMethodCallbackDelegate <>9__1;
				MapDomainMethod.AsyncCall.GetBlockData(this, this._block.AreaId, this._block.BlockId, delegate(int offset, RawDataPool dataPool)
				{
					MapBlockData block = null;
					Serializer.Deserialize(dataPool, offset, ref block);
					this._block = block;
					IAsyncMethodRequestHandler <>4__this = this;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate(int offset2, RawDataPool dataPool2)
						{
							Serializer.Deserialize(dataPool2, offset2, ref this._treeList);
							this.InitDisplay(false);
							Action onFinish2 = onFinish;
							if (onFinish2 != null)
							{
								onFinish2();
							}
						});
					}
					ExtraDomainMethod.AsyncCall.GetAllHeavenlyTrees(<>4__this, callback);
				});
			}
		}
	}

	// Token: 0x06003B5F RID: 15199 RVA: 0x001E0E68 File Offset: 0x001DF068
	private void InitDisplay(bool forceHide = false)
	{
		this._isUpdatingData = false;
		this._normalCharList.Clear();
		this._infectedList.Clear();
		this._legendaryBookOwnerEnemyList.Clear();
		this._normalEnemyList.Clear();
		this._randomEnemyList.Clear();
		this._animalEnemyList.Clear();
		this._graveList.Clear();
		this._specialCharList.Clear();
		this._needUpdateOnFocus = false;
		bool isWartingData = this._isWartingData;
		if (isWartingData)
		{
			this._isWartingData = false;
		}
		else
		{
			this._charDataDict.Clear();
			this._graveDataDict.Clear();
			this._caravanCharDict.Clear();
		}
		bool flag = !forceHide;
		if (flag)
		{
			bool isHiding = this._mapModel.IsHideCharacterSet();
			bool flag2 = this._block != null && !isHiding && this._treeList != null && this._treeList.Count > 0;
			if (flag2)
			{
				this._normalCharList.AddRange(from t in this._treeList
				where t.Location == this._block.GetLocation()
				select t.Id);
			}
			MapBlockData block = this._block;
			bool flag3 = ((block != null) ? block.FixedCharacterSet : null) != null && !isHiding;
			if (flag3)
			{
				this._normalCharList.AddRange(this._block.FixedCharacterSet);
			}
			MapBlockData block2 = this._block;
			bool flag4 = ((block2 != null) ? block2.InfectedCharacterSet : null) != null && !isHiding;
			if (flag4)
			{
				this._infectedList.AddRange(this._block.InfectedCharacterSet);
			}
			MapBlockData block3 = this._block;
			bool flag5 = ((block3 != null) ? block3.EnemyCharacterSet : null) != null && !isHiding;
			if (flag5)
			{
				this._infectedList.AddRange(this._block.EnemyCharacterSet);
			}
			MapBlockData block4 = this._block;
			bool flag6 = ((block4 != null) ? block4.CharacterSet : null) != null && !isHiding;
			if (flag6)
			{
				this._normalCharList.AddRange(this._block.CharacterSet);
			}
			MapBlockData block5 = this._block;
			bool flag7 = ((block5 != null) ? block5.TemplateEnemyList : null) != null && !isHiding;
			if (flag7)
			{
				this._randomEnemyList.AddRange(this._block.TemplateEnemyList);
			}
			MapBlockData block6 = this._block;
			bool flag8 = ((block6 != null) ? block6.GraveSet : null) != null && !isHiding;
			if (flag8)
			{
				this._graveList.AddRange(this._block.GraveSet);
			}
			Dictionary<short, List<int>> animalAreaData;
			List<int> animalIds;
			bool flag9 = this._block != null && !isHiding && this._mapModel.LocationAnimalMap.TryGetValue(this._block.AreaId, out animalAreaData) && animalAreaData.TryGetValue(this._block.BlockId, out animalIds);
			if (flag9)
			{
				this._animalEnemyList.AddRange(animalIds);
			}
			this._randomEnemyList.Sort(delegate(MapTemplateEnemyInfo a, MapTemplateEnemyInfo b)
			{
				CharacterItem configData = Character.Instance.GetItem(a.TemplateId);
				CharacterItem configData2 = Character.Instance.GetItem(b.TemplateId);
				bool flag18 = configData.OrganizationInfo.OrgTemplateId != configData2.OrganizationInfo.OrgTemplateId;
				int result;
				if (flag18)
				{
					result = ((configData.OrganizationInfo.OrgTemplateId != 0) ? 1 : -1);
				}
				else
				{
					bool flag19 = configData.OrganizationInfo.Grade != configData2.OrganizationInfo.Grade;
					if (flag19)
					{
						result = (int)(configData2.OrganizationInfo.Grade - configData.OrganizationInfo.Grade);
					}
					else
					{
						result = (int)(a.TemplateId - b.TemplateId);
					}
				}
				return result;
			});
		}
		foreach (CaravanDisplayData caravan in this._caravanList)
		{
			Location location = caravan.PathInArea.GetCurrLocation();
			bool flag10 = this._block != null && location.BlockId == this._block.BlockId && location.AreaId == this._block.AreaId;
			if (flag10)
			{
				this._specialCharList.Add(caravan.CaravanId);
				this._caravanCharDict[caravan.CaravanId] = caravan;
			}
		}
		bool needShow = this._normalCharList.Count + this._infectedList.Count + this._randomEnemyList.Count + this._animalEnemyList.Count + this._graveList.Count + this._specialCharList.Count > 0;
		bool flag11 = needShow;
		if (flag11)
		{
			this.PlayShowAnim();
		}
		else
		{
			this.PlayHideAnim();
		}
		bool advancingMonth = GameApp.AdvancingMonth;
		if (advancingMonth)
		{
			needShow = false;
		}
		bool flag12 = needShow;
		if (flag12)
		{
			GameDataBridge.AddMethodCall(this.Element.GameDataListenerId, 19, 69);
			List<int> list = EasyPool.Get<List<int>>();
			list.Clear();
			bool flag13 = this._normalCharList.Count > 0;
			if (flag13)
			{
				list.AddRange(this._normalCharList);
			}
			bool flag14 = this._infectedList.Count > 0;
			if (flag14)
			{
				list.AddRange(this._infectedList);
			}
			bool flag15 = list.Count > 0;
			if (flag15)
			{
				this._isWartingData = true;
				GameDataBridge.AddMethodCall<List<int>>(this.Element.GameDataListenerId, 18, 12, list);
				GameDataBridge.AddMethodCall<List<int>>(this.Element.GameDataListenerId, 4, 48, list);
			}
			EasyPool.Free<List<int>>(list);
			bool flag16 = this._graveList.Count > 0;
			if (flag16)
			{
				this._isWartingData = true;
				GameDataBridge.AddMethodCall<List<int>>(this.Element.GameDataListenerId, 4, 51, this._graveList);
			}
			bool flag17 = this._normalCharList.Count == 0 && this._infectedList.Count == 0 && this._graveList.Count == 0;
			if (flag17)
			{
				this.InitTogGroup();
			}
		}
	}

	// Token: 0x06003B60 RID: 15200 RVA: 0x001E13D8 File Offset: 0x001DF5D8
	private void InitTogGroup()
	{
		int firstActiveKey = this.HasNormal ? 0 : (this.HasEnemy ? 1 : (this.HasGrave ? 3 : (this.HasSpecial ? 2 : 4)));
		bool flag = this._targetTogKey > -1;
		if (flag)
		{
			bool flag2 = this.HasGrave && this._targetTogKey == 3;
			if (flag2)
			{
				firstActiveKey = 3;
			}
			else
			{
				bool flag3 = this.HasNormal && this._targetTogKey == 0;
				if (flag3)
				{
					firstActiveKey = 0;
				}
				else
				{
					bool flag4 = this.HasEnemy && this._targetTogKey == 1;
					if (flag4)
					{
						firstActiveKey = 1;
					}
					else
					{
						bool flag5 = this.HasSpecial && this._targetTogKey == 2;
						if (flag5)
						{
							firstActiveKey = 2;
						}
						else
						{
							bool flag6 = this.HasInformation && this._targetTogKey == 4;
							if (flag6)
							{
								firstActiveKey = 4;
							}
						}
					}
				}
			}
		}
		bool isSameToLast = this._lastTogKey == firstActiveKey;
		this._togGroup.SetInteractable(this.HasNormal, 0);
		this._togGroup.SetInteractable(this.HasEnemy, 1);
		this._togGroup.SetInteractable(this.HasGrave, 3);
		this._togGroup.SetInteractable(this.HasSpecial, 2);
		this._togGroup.SetInteractable(this.HasInformation, 4);
		CToggleObsolete active = this._togGroup.GetActive();
		bool flag7 = active != null && (this._needResetTog || !active.interactable) && active.Key != firstActiveKey && !isSameToLast;
		if (flag7)
		{
			this._togGroup.Set(firstActiveKey, true, false);
			this._lastTogKey = firstActiveKey;
		}
		else
		{
			this.UpdateCharList(this._needResetTog);
		}
		this._needResetTog = false;
	}

	// Token: 0x06003B61 RID: 15201 RVA: 0x001E158E File Offset: 0x001DF78E
	private void SetTargetTogKey(int targetTogKey)
	{
		this._targetTogKey = targetTogKey;
	}

	// Token: 0x06003B62 RID: 15202 RVA: 0x001E1598 File Offset: 0x001DF798
	private void UpdateCharList(bool scrollToTop)
	{
		bool flag = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
		if (!flag)
		{
			bool flag2 = this._togGroup == null || this._togGroup.GetActive() == null;
			if (!flag2)
			{
				int togKey = this._togGroup.GetActive().Key;
				bool flag3 = (togKey == 0 && this._normalCharList.Any((int id) => !this._charDataDict.ContainsKey(id))) || (togKey == 3 && this._graveList.Count > this._graveDataDict.Count);
				if (!flag3)
				{
					if (!true)
					{
					}
					Action<int, Refers> action;
					switch (togKey)
					{
					case 0:
						action = new Action<int, Refers>(this.OnRenderCharNormal);
						break;
					case 1:
						action = new Action<int, Refers>(this.OnRenderCharEnemy);
						break;
					case 2:
						action = new Action<int, Refers>(this.OnRenderCharCaravan);
						break;
					case 3:
						action = new Action<int, Refers>(this.OnRenderCharGrave);
						break;
					case 4:
						action = new Action<int, Refers>(this.OnRenderCharInformation);
						break;
					default:
						if (!true)
						{
						}
						<PrivateImplementationDetails>.ThrowSwitchExpressionException(togKey);
						break;
					}
					if (!true)
					{
					}
					Action<int, Refers> onRenderer = action;
					this._charScroll.OnItemRender = onRenderer;
					if (!true)
					{
					}
					int num;
					switch (togKey)
					{
					case 0:
						num = this.RefreshSearchedNormalCharacterData();
						break;
					case 1:
						num = this.RefreshEnemyCharacterData();
						break;
					case 2:
						num = this.RefreshSearchedCaravanCharacterData();
						break;
					case 3:
						num = this.RefreshSearchedGraveCharacterData();
						break;
					case 4:
						num = this.RefreshSearchedInformationCharacterData();
						break;
					default:
						if (!true)
						{
						}
						<PrivateImplementationDetails>.ThrowSwitchExpressionException(togKey);
						break;
					}
					if (!true)
					{
					}
					int actorAmount = num;
					string prefabName = this.GetCharPrefabName(togKey, this._canSeeDetail);
					Refers prefab = base.CGet<Refers>(prefabName);
					bool flag4 = this._charScroll.SrcPrefab.name != prefabName;
					if (flag4)
					{
						this._charScroll.UpdateStyle(prefab, actorAmount);
					}
					else
					{
						this._charScroll.SetDataCount(actorAmount);
					}
					if (scrollToTop)
					{
						this._charScroll.ScrollTo(0, 0.3f);
					}
					this.UpdateCountLabel(actorAmount);
				}
			}
		}
	}

	// Token: 0x06003B63 RID: 15203 RVA: 0x001E17B4 File Offset: 0x001DF9B4
	private void UpdateCountLabel(int actorAmount)
	{
		base.CGet<TextMeshProUGUI>("CountLabel").text = actorAmount.ToString();
	}

	// Token: 0x06003B64 RID: 15204 RVA: 0x001E17D0 File Offset: 0x001DF9D0
	private int RefreshSearchedNormalCharacterData()
	{
		this._searchedNormalCharList.Clear();
		bool flag = UI_MapBlockCharList._staticSearchInputField.text.IsNullOrEmpty();
		if (flag)
		{
			this._searchedNormalCharList.AddRange(this._normalCharList);
		}
		else
		{
			this._searchedNormalCharList.AddRange(this._normalCharList.Where(delegate(int charId)
			{
				CharacterDisplayData characterDisplayData;
				bool flag2 = this._charDataDict.TryGetValue(charId, out characterDisplayData);
				bool result;
				if (flag2)
				{
					string nameContent = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, false, false);
					string org = CommonUtils.GetOrganizationGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge, -1);
					result = (nameContent.Contains(UI_MapBlockCharList._staticSearchInputField.text) || org.Contains(UI_MapBlockCharList._staticSearchInputField.text));
				}
				else
				{
					result = false;
				}
				return result;
			}));
		}
		return this._searchedNormalCharList.Count;
	}

	// Token: 0x06003B65 RID: 15205 RVA: 0x001E1844 File Offset: 0x001DFA44
	private int RefreshEnemyCharacterData()
	{
		this._searchedNormalEnemyList.Clear();
		bool flag = UI_MapBlockCharList._staticSearchInputField.text.IsNullOrEmpty();
		if (flag)
		{
			this._searchedNormalEnemyList.AddRange(this._normalEnemyList);
		}
		else
		{
			this._searchedNormalEnemyList.AddRange(this._normalEnemyList.Where(delegate(int charId)
			{
				CharacterDisplayData characterDisplayData;
				bool flag6 = this._charDataDict.TryGetValue(charId, out characterDisplayData);
				bool result;
				if (flag6)
				{
					string nameContent = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, false, false);
					string org2 = CommonUtils.GetOrganizationGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge, -1);
					result = (nameContent.Contains(UI_MapBlockCharList._staticSearchInputField.text) || org2.Contains(UI_MapBlockCharList._staticSearchInputField.text));
				}
				else
				{
					result = false;
				}
				return result;
			}));
		}
		this._searchedRandomEnemyList.Clear();
		this._searchedRandomEnemyListIndexToAnimalId.Clear();
		bool flag2 = UI_MapBlockCharList._staticSearchInputField.text.IsNullOrEmpty();
		if (flag2)
		{
			this._searchedRandomEnemyList.AddRange(this._randomEnemyList);
			foreach (int animalId in this._animalEnemyList)
			{
				GameData.Domains.Character.Animal animal;
				bool flag3 = this._mapModel.Animals.TryGetValue(animalId, out animal);
				if (flag3)
				{
					this._searchedRandomEnemyList.Add(MapTemplateEnemyInfo.CreateDefault(animal.CharacterTemplateId, this._block.BlockId, -1));
					this._searchedRandomEnemyListIndexToAnimalId.Add(this._searchedRandomEnemyList.Count - 1, animalId);
				}
			}
		}
		else
		{
			this._searchedRandomEnemyList.AddRange(this._randomEnemyList.Where(delegate(MapTemplateEnemyInfo enemy)
			{
				CharacterItem config2 = Character.Instance[enemy.TemplateId];
				string name = config2.Surname + config2.GivenName;
				string org2 = CommonUtils.GetOrganizationGradeString(config2.OrganizationInfo, -1, -1, -1);
				return name.Contains(UI_MapBlockCharList._staticSearchInputField.text) || org2.Contains(UI_MapBlockCharList._staticSearchInputField.text);
			}));
			foreach (int animalId2 in this._animalEnemyList)
			{
				GameData.Domains.Character.Animal animal2;
				bool flag4 = this._mapModel.Animals.TryGetValue(animalId2, out animal2);
				if (flag4)
				{
					CharacterItem config = Character.Instance[animal2.CharacterTemplateId];
					string animalName = config.Surname + config.GivenName;
					string org = CommonUtils.GetOrganizationGradeString(config.OrganizationInfo, -1, -1, -1);
					bool flag5 = animalName.Contains(UI_MapBlockCharList._staticSearchInputField.text) || org.Contains(UI_MapBlockCharList._staticSearchInputField.text);
					if (flag5)
					{
						this._searchedRandomEnemyList.Add(MapTemplateEnemyInfo.CreateDefault(animal2.CharacterTemplateId, this._block.BlockId, -1));
						this._searchedRandomEnemyListIndexToAnimalId.Add(this._searchedRandomEnemyList.Count - 1, animalId2);
					}
				}
			}
		}
		return this._searchedNormalEnemyList.Count + this._searchedRandomEnemyList.Count;
	}

	// Token: 0x06003B66 RID: 15206 RVA: 0x001E1AF0 File Offset: 0x001DFCF0
	private int RefreshSearchedGraveCharacterData()
	{
		this._searchedGraveList.Clear();
		bool flag = UI_MapBlockCharList._staticSearchInputField.text.IsNullOrEmpty();
		if (flag)
		{
			this._searchedGraveList.AddRange(this._graveList);
		}
		else
		{
			this._searchedGraveList.AddRange(this._graveList.Where(delegate(int id)
			{
				GraveDisplayData graveDisplayData;
				bool flag2 = this._graveDataDict.TryGetValue(id, out graveDisplayData);
				bool result;
				if (flag2)
				{
					string nameContent = NameCenter.GetCharMonasticTitleOrNameByGraveData(graveDisplayData, false, false);
					OrganizationInfo organizationInfo = new OrganizationInfo(graveDisplayData.NameData.OrgTemplateId, graveDisplayData.NameData.OrgGrade, graveDisplayData.Principal, graveDisplayData.OrgSettlementId);
					string org = CommonUtils.GetOrganizationGradeString(organizationInfo, graveDisplayData.NameData.Gender, -1, -1);
					result = (nameContent.Contains(UI_MapBlockCharList._staticSearchInputField.text) || org.Contains(UI_MapBlockCharList._staticSearchInputField.text));
				}
				else
				{
					result = false;
				}
				return result;
			}));
		}
		return this._searchedGraveList.Count;
	}

	// Token: 0x06003B67 RID: 15207 RVA: 0x001E1B64 File Offset: 0x001DFD64
	private int RefreshSearchedCaravanCharacterData()
	{
		this._searchedSpecialCharList.Clear();
		bool flag = UI_MapBlockCharList._staticSearchInputField.text.IsNullOrEmpty();
		if (flag)
		{
			this._searchedSpecialCharList.AddRange(this._specialCharList);
		}
		else
		{
			this._searchedSpecialCharList.AddRange(this._specialCharList.Where(delegate(int id)
			{
				CaravanDisplayData caravanDisplayData;
				bool flag2 = this._caravanCharDict.TryGetValue(id, out caravanDisplayData);
				bool result;
				if (flag2)
				{
					MerchantTypeItem merchantTypeConfig = Config.MerchantType.Instance[Merchant.Instance[(int)caravanDisplayData.MerchantTemplateId].MerchantType];
					result = merchantTypeConfig.Name.Contains(UI_MapBlockCharList._staticSearchInputField.text);
				}
				else
				{
					result = false;
				}
				return result;
			}));
		}
		return this._searchedSpecialCharList.Count;
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x001E1BD8 File Offset: 0x001DFDD8
	private int RefreshSearchedInformationCharacterData()
	{
		this._searchedInfoList.Clear();
		bool flag = UI_MapBlockCharList._staticSearchInputField.text.IsNullOrEmpty();
		if (flag)
		{
			this._searchedInfoList.AddRange(this._characterDisplayDataWithInfoList);
		}
		else
		{
			this._searchedInfoList.AddRange(this._characterDisplayDataWithInfoList.Where(delegate(CharacterDisplayDataWithInfo info)
			{
				CharacterDisplayData characterDisplayData = info.CharacterDisplayData;
				string nameContent = NameCenter.GetCharMonasticTitleOrNameByDisplayData(info.CharacterDisplayData, false, false);
				string org = CommonUtils.GetOrganizationGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge, -1);
				return nameContent.Contains(UI_MapBlockCharList._staticSearchInputField.text) || org.Contains(UI_MapBlockCharList._staticSearchInputField.text);
			}));
		}
		return this._searchedInfoList.Count;
	}

	// Token: 0x06003B69 RID: 15209 RVA: 0x001E1C60 File Offset: 0x001DFE60
	private string GetCharPrefabName(int togKey, bool canSeeDetail)
	{
		string prefabName = "MapBlockCharUnknownWrapper";
		if (canSeeDetail)
		{
			if (!true)
			{
			}
			string text;
			switch (togKey)
			{
			case 0:
				text = "MapBlockCharNormalWrapper";
				break;
			case 1:
				text = "MapBlockCharEnemyWrapper";
				break;
			case 2:
				text = "MapBlockCharCaravanWrapper";
				break;
			case 3:
				text = "MapBlockCharGraveWrapper";
				break;
			case 4:
				text = "MapBlockCharInformationWrapper";
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(togKey);
				break;
			}
			if (!true)
			{
			}
			prefabName = text;
		}
		return prefabName;
	}

	// Token: 0x06003B6A RID: 15210 RVA: 0x001E1CE0 File Offset: 0x001DFEE0
	private void OnRenderCharNormal(int index, Refers charRefers)
	{
		bool canSeeDetail = this._canSeeDetail;
		int charId = this._searchedNormalCharList.CheckIndex(index) ? this._searchedNormalCharList[index] : -1;
		CharacterDisplayData characterDisplayData;
		bool flag = !this._charDataDict.TryGetValue(charId, out characterDisplayData);
		if (flag)
		{
			canSeeDetail = false;
		}
		bool flag2 = canSeeDetail;
		if (flag2)
		{
			GameObject inner = charRefers.CGet<GameObject>("Content");
			MapBlockCharNormal2 normal;
			bool flag3 = !inner.TryGetComponent<MapBlockCharNormal2>(out normal);
			if (!flag3)
			{
				normal.Init(this._canInteract, this._block, characterDisplayData, this._loongInfos);
			}
		}
		else
		{
			GameObject inner2 = charRefers.CGet<GameObject>("Content");
			MapBlockCharUnknown2 unkown;
			bool flag4 = !inner2.TryGetComponent<MapBlockCharUnknown2>(out unkown);
			if (!flag4)
			{
				unkown.Init(this._canInteract, this._block);
			}
		}
	}

	// Token: 0x06003B6B RID: 15211 RVA: 0x001E1DA8 File Offset: 0x001DFFA8
	private void OnRenderCharEnemy(int index, Refers charRefers)
	{
		int togKey = this._togGroup.GetActive().Key;
		bool canSeeDetail = this._canSeeDetail;
		int randomEnemyIndex = index - this._searchedNormalEnemyList.Count;
		bool isRandomEnemy = togKey == 1 && this._searchedRandomEnemyList.CheckIndex(randomEnemyIndex);
		int animalId = this._searchedRandomEnemyListIndexToAnimalId.GetValueOrDefault(randomEnemyIndex, -1);
		CharacterDisplayData characterDisplayData = null;
		CharacterItem randomEnemyCharConfig = null;
		bool flag = isRandomEnemy;
		if (flag)
		{
			bool flag2 = this._searchedRandomEnemyList.CheckIndex(randomEnemyIndex);
			if (flag2)
			{
				MapTemplateEnemyInfo enemy = this._searchedRandomEnemyList[randomEnemyIndex];
				randomEnemyCharConfig = Character.Instance[enemy.TemplateId];
			}
			else
			{
				canSeeDetail = false;
			}
		}
		else
		{
			int charId = this._searchedNormalEnemyList.CheckIndex(index) ? this._searchedNormalEnemyList[index] : -1;
			bool flag3 = !this._charDataDict.TryGetValue(charId, out characterDisplayData);
			if (flag3)
			{
				canSeeDetail = false;
			}
		}
		bool flag4 = canSeeDetail;
		if (flag4)
		{
			GameObject inner = charRefers.CGet<GameObject>("Content");
			MapBlockCharEnemy2 mapBlockCharEnemy;
			bool flag5 = !inner.TryGetComponent<MapBlockCharEnemy2>(out mapBlockCharEnemy);
			if (!flag5)
			{
				bool flag6 = isRandomEnemy;
				if (flag6)
				{
					mapBlockCharEnemy.InitRandomEnemy(this._canInteract, this._block, randomEnemyCharConfig, animalId, (this._searchedRandomEnemyList.Count > randomEnemyIndex) ? this._searchedRandomEnemyList[randomEnemyIndex].Duration : -1);
				}
				else
				{
					mapBlockCharEnemy.InitNormal(this._canInteract, this._block, characterDisplayData, this._loongInfos);
				}
			}
		}
		else
		{
			GameObject inner2 = charRefers.CGet<GameObject>("Content");
			MapBlockCharUnknown2 unkown;
			bool flag7 = !inner2.TryGetComponent<MapBlockCharUnknown2>(out unkown);
			if (!flag7)
			{
				unkown.Init(this._canInteract, this._block);
			}
		}
	}

	// Token: 0x06003B6C RID: 15212 RVA: 0x001E1F4C File Offset: 0x001E014C
	private void OnRenderCharCaravan(int index, Refers charRefers)
	{
		bool canSeeDetail = this._canSeeDetail;
		int charId = this._searchedSpecialCharList.CheckIndex(index) ? this._searchedSpecialCharList[index] : -1;
		CaravanDisplayData caravanData;
		bool flag = !this._caravanCharDict.TryGetValue(charId, out caravanData);
		if (flag)
		{
			canSeeDetail = false;
		}
		bool flag2 = canSeeDetail;
		if (flag2)
		{
			GameObject inner = charRefers.CGet<GameObject>("Content");
			MapBlockCharCaravan2 caravan;
			bool flag3 = !inner.TryGetComponent<MapBlockCharCaravan2>(out caravan);
			if (!flag3)
			{
				caravan.Init(this._canInteract, this._block, caravanData);
			}
		}
		else
		{
			GameObject inner2 = charRefers.CGet<GameObject>("Content");
			MapBlockCharUnknown2 unkown;
			bool flag4 = !inner2.TryGetComponent<MapBlockCharUnknown2>(out unkown);
			if (!flag4)
			{
				unkown.Init(this._canInteract, this._block);
			}
		}
	}

	// Token: 0x06003B6D RID: 15213 RVA: 0x001E2010 File Offset: 0x001E0210
	private void OnRenderCharGrave(int index, Refers charRefers)
	{
		bool canSeeDetail = this._canSeeDetail;
		int graveId = this._searchedGraveList.CheckIndex(index) ? this._searchedGraveList[index] : -1;
		GraveDisplayData graveDisplayData;
		bool flag = !this._graveDataDict.TryGetValue(graveId, out graveDisplayData);
		if (flag)
		{
			canSeeDetail = false;
		}
		bool flag2 = canSeeDetail;
		if (flag2)
		{
			GameObject inner = charRefers.CGet<GameObject>("Content");
			MapBlockCharGrave2 grave;
			bool flag3 = !inner.TryGetComponent<MapBlockCharGrave2>(out grave);
			if (!flag3)
			{
				grave.Init(this._canInteract, this._block, graveDisplayData);
			}
		}
		else
		{
			GameObject inner2 = charRefers.CGet<GameObject>("Content");
			MapBlockCharUnknown2 unkown;
			bool flag4 = !inner2.TryGetComponent<MapBlockCharUnknown2>(out unkown);
			if (!flag4)
			{
				unkown.Init(this._canInteract, this._block);
			}
		}
	}

	// Token: 0x06003B6E RID: 15214 RVA: 0x001E20D4 File Offset: 0x001E02D4
	private void OnRenderCharInformation(int index, Refers charRefers)
	{
		bool canSeeDetail = this._canSeeDetail;
		bool flag = !this._searchedInfoList.CheckIndex(index);
		if (flag)
		{
			canSeeDetail = false;
		}
		bool flag2 = canSeeDetail;
		if (flag2)
		{
			CharacterDisplayDataWithInfo characterDisplayDataWithInfo = this._searchedInfoList[index];
			GameObject inner = charRefers.CGet<GameObject>("Content");
			MapBlockCharInformation2 information;
			bool flag3 = !inner.TryGetComponent<MapBlockCharInformation2>(out information);
			if (!flag3)
			{
				information.Init(this._canInteract, this._block, characterDisplayDataWithInfo);
			}
		}
		else
		{
			GameObject inner2 = charRefers.CGet<GameObject>("Content");
			MapBlockCharUnknown2 unkown;
			bool flag4 = !inner2.TryGetComponent<MapBlockCharUnknown2>(out unkown);
			if (!flag4)
			{
				unkown.Init(this._canInteract, this._block);
			}
		}
	}

	// Token: 0x06003B6F RID: 15215 RVA: 0x001E2184 File Offset: 0x001E0384
	private void OnItemHide(Refers charRefers)
	{
		MapBlockCharNormal2 mapBlockCharNormal;
		bool flag = charRefers.CTryGet<MapBlockCharNormal2>("MapBlockCharNormalWrapper", out mapBlockCharNormal);
		if (flag)
		{
			mapBlockCharNormal.OnHide();
		}
		else
		{
			MapBlockCharUnknown2 mapBlockCharUnknown;
			bool flag2 = charRefers.CTryGet<MapBlockCharUnknown2>("MapBlockCharUnknownWrapper", out mapBlockCharUnknown);
			if (flag2)
			{
				mapBlockCharUnknown.OnHide();
			}
			else
			{
				MapBlockCharEnemy2 mapBlockCharEnemy;
				bool flag3 = charRefers.CTryGet<MapBlockCharEnemy2>("MapBlockCharEnemyWrapper", out mapBlockCharEnemy);
				if (flag3)
				{
					mapBlockCharEnemy.OnHide();
				}
				else
				{
					MapBlockCharGrave2 mapBlockCharGrave;
					bool flag4 = charRefers.CTryGet<MapBlockCharGrave2>("MapBlockCharGraveWrapper", out mapBlockCharGrave);
					if (flag4)
					{
						mapBlockCharGrave.OnHide();
					}
					else
					{
						MapBlockCharCaravan2 mapBlockCharCaravan;
						bool flag5 = charRefers.CTryGet<MapBlockCharCaravan2>("MapBlockCharCaravanWrapper", out mapBlockCharCaravan);
						if (flag5)
						{
							mapBlockCharCaravan.OnHide();
						}
						else
						{
							MapBlockCharInformation2 mapBlockCharInformation;
							bool flag6 = charRefers.CTryGet<MapBlockCharInformation2>("MapBlockCharInformationWrapper", out mapBlockCharInformation);
							if (flag6)
							{
								mapBlockCharInformation.OnHide();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003B70 RID: 15216 RVA: 0x001E223C File Offset: 0x001E043C
	private void OnGameStateChange(ArgumentBox argBox)
	{
		Enum state;
		argBox.Get("newState", out state);
		bool flag = (EGameState)state == EGameState.Login;
		if (flag)
		{
			this._block = null;
		}
	}

	// Token: 0x06003B71 RID: 15217 RVA: 0x001E226C File Offset: 0x001E046C
	private void OnSelectedBlockChange(ArgumentBox argBox)
	{
		UI_MapBlockCharList._staticSearchInputField.SetTextWithoutNotify(string.Empty);
		this.SetTargetTogKey(-1);
		MapBlockData block;
		argBox.Get<MapBlockData>("block", out block);
		this._needResetTog = (this._block == null || block == null || block.BlockId != this._block.BlockId || block.AreaId != this._block.AreaId);
		bool flag = block != null && this._mapModel != null;
		if (flag)
		{
			MapBlockData currentBlock = this._mapModel.GetBlockData(this._mapModel.CurrentBlockId);
			bool flag2 = currentBlock != null;
			if (flag2)
			{
				ByteCoordinate pos = block.GetBlockPos();
				ByteCoordinate currentPos = currentBlock.GetBlockPos();
				this._block = block;
				this._canInteract = (this._block != null && this._block.BlockId == this._mapModel.CurrentBlockId);
				this._canSeeDetail = (this._canInteract || Mathf.Abs((int)(currentPos.X - pos.X)) + Mathf.Abs((int)(currentPos.Y - pos.Y)) <= 1 || SingletonObject.getInstance<WorldMapModel>().IsLocationShouldInSight(block.GetLocation()));
				this.UpdateDataFromSvr(true, null);
			}
		}
		else
		{
			this.PlayHideAnim();
		}
	}

	// Token: 0x06003B72 RID: 15218 RVA: 0x001E23BA File Offset: 0x001E05BA
	private void OnAnimalPlaceDataChange(ArgumentBox argBox)
	{
		this.OnMapBlockDataChange(new ArgumentBox().SetObject("Data", this._block));
	}

	// Token: 0x06003B73 RID: 15219 RVA: 0x001E23DC File Offset: 0x001E05DC
	private void OnMapBlockDataChange(ArgumentBox argBox)
	{
		MapBlockData data;
		argBox.Get<MapBlockData>("Data", out data);
		bool flag = this._block == null || (data.AreaId == this._block.AreaId && data.BlockId == this._block.BlockId);
		if (flag)
		{
			this._block = data;
			this.UpdateDataFromSvr(false, null);
		}
	}

	// Token: 0x06003B74 RID: 15220 RVA: 0x001E2444 File Offset: 0x001E0644
	private void OnVillagerWorkDataChange(ArgumentBox argBox)
	{
		int togKey = this._togGroup.GetActive().Key;
		bool flag = (togKey == 0 || togKey == 2) && this._canSeeDetail && this._charDataDict.Count > 0;
		if (flag)
		{
			this.UpdateCharList(false);
		}
	}

	// Token: 0x06003B75 RID: 15221 RVA: 0x001E2490 File Offset: 0x001E0690
	private void OnSavingWorldStateChange(ArgumentBox argBox)
	{
		bool flag = !SingletonObject.getInstance<BasicGameData>().SavingWorld;
		if (flag)
		{
			this.UpdateDataFromSvr(false, null);
		}
	}

	// Token: 0x06003B76 RID: 15222 RVA: 0x001E24B8 File Offset: 0x001E06B8
	private void OnTopUiChanged(ArgumentBox argBox)
	{
		bool flag = UIManager.Instance.IsFocusElement(this.Element);
		if (flag)
		{
			bool flag2 = this._needUpdateOnFocus && this._block != null;
			if (flag2)
			{
				this.UpdateDataFromSvr(false, null);
			}
		}
		else
		{
			bool flag3 = UIManager.Instance.IsFocusElement(UIElement.EventWindow) || UIManager.Instance.IsFocusElement(UIElement.Combat) || UIManager.Instance.IsFocusElement(UIElement.PartWorld) || UIManager.Instance.IsFocusElement(UIElement.MainMenu) || UIManager.Instance.IsFocusElement(UIElement.AdvanceConfirm) || UIManager.Instance.IsFocusElement(UIElement.Advance) || UIManager.Instance.IsFocusElement(UIElement.MonthNotify) || UIManager.Instance.IsFocusElement(UIElement.TaiwuVillagers);
			if (flag3)
			{
				bool flag4 = this._block != null;
				if (flag4)
				{
					this.UpdateDataFromSvr(false, delegate
					{
						this._needUpdateOnFocus = true;
					});
				}
			}
		}
	}

	// Token: 0x06003B77 RID: 15223 RVA: 0x001E25B4 File Offset: 0x001E07B4
	private void OnSetMapBlockCharListTog(ArgumentBox argBox)
	{
		int togKey = -1;
		if (argBox != null)
		{
			argBox.Get("TogKey", out togKey);
		}
		this.SetTargetTogKey(togKey);
	}

	// Token: 0x06003B78 RID: 15224 RVA: 0x001E25E0 File Offset: 0x001E07E0
	private void OnSomeoneNickNameChanged(ArgumentBox argbox)
	{
		List<int> list = new List<int>();
		int charId;
		argbox.Get("CharacterId", out charId);
		list.Add(charId);
		GameDataBridge.AddMethodCall<List<int>>(this.Element.GameDataListenerId, 4, 48, list);
	}

	// Token: 0x06003B79 RID: 15225 RVA: 0x001E261F File Offset: 0x001E081F
	private void OnMapBlockCharCustomInfoChanged(ArgumentBox _)
	{
		this._charScroll.ReRender();
	}

	// Token: 0x06003B7A RID: 15226 RVA: 0x001E262E File Offset: 0x001E082E
	private void OnMapBlockCharCustomButtonChanged(ArgumentBox _)
	{
		this._charScroll.ReRender();
	}

	// Token: 0x06003B7B RID: 15227 RVA: 0x001E2640 File Offset: 0x001E0840
	private int CharCompare(int charId1, int charId2)
	{
		CharacterDisplayData data = this._charDataDict[charId1];
		CharacterDisplayData data2 = this._charDataDict[charId2];
		MapBlockData block = this._block;
		bool? flag;
		if (block == null)
		{
			flag = null;
		}
		else
		{
			HashSet<int> fixedCharacterSet = block.FixedCharacterSet;
			flag = ((fixedCharacterSet != null) ? new bool?(fixedCharacterSet.Contains(charId1)) : null);
		}
		bool? flag2 = flag;
		bool isSpecial = flag2.GetValueOrDefault();
		MapBlockData block2 = this._block;
		bool? flag3;
		if (block2 == null)
		{
			flag3 = null;
		}
		else
		{
			HashSet<int> fixedCharacterSet2 = block2.FixedCharacterSet;
			flag3 = ((fixedCharacterSet2 != null) ? new bool?(fixedCharacterSet2.Contains(charId2)) : null);
		}
		flag2 = flag3;
		bool isSpecial2 = flag2.GetValueOrDefault();
		bool flag4 = isSpecial != isSpecial2;
		int result;
		if (flag4)
		{
			result = isSpecial2.CompareTo(isSpecial);
		}
		else
		{
			bool isTree = data.TemplateId >= 598 && data.TemplateId <= 602;
			bool isTree2 = data2.TemplateId >= 598 && data2.TemplateId <= 602;
			bool flag5 = isTree != isTree2;
			if (flag5)
			{
				result = isTree2.CompareTo(isTree);
			}
			else
			{
				bool flag6 = data2.OrgInfo.Grade != data.OrgInfo.Grade;
				if (flag6)
				{
					result = data2.OrgInfo.Grade.CompareTo(data.OrgInfo.Grade);
				}
				else
				{
					bool flag7 = data2.OrgInfo.Principal != data.OrgInfo.Principal;
					if (flag7)
					{
						result = data2.OrgInfo.Principal.CompareTo(data.OrgInfo.Principal);
					}
					else
					{
						bool flag8 = data2.PhysiologicalAge != data.PhysiologicalAge;
						if (flag8)
						{
							result = data2.PhysiologicalAge.CompareTo(data.PhysiologicalAge);
						}
						else
						{
							result = charId1.CompareTo(charId2);
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003B7C RID: 15228 RVA: 0x001E2824 File Offset: 0x001E0A24
	private int GraveCompare(int id1, int id2)
	{
		GraveDisplayData data;
		GraveDisplayData data2;
		bool flag = this._graveDataDict.TryGetValue(id1, out data) && this._graveDataDict.TryGetValue(id2, out data2);
		if (flag)
		{
			bool flag2 = data.NameData.OrgGrade != data2.NameData.OrgGrade;
			if (flag2)
			{
				return (int)(data2.NameData.OrgGrade - data.NameData.OrgGrade);
			}
			bool flag3 = data.Principal != data2.Principal;
			if (flag3)
			{
				return data.Principal ? -1 : 1;
			}
			bool flag4 = data.NameData.Gender != data2.NameData.Gender;
			if (flag4)
			{
				return (int)(data2.NameData.Gender - data.NameData.Gender);
			}
		}
		return id1 - id2;
	}

	// Token: 0x06003B7D RID: 15229 RVA: 0x001E2902 File Offset: 0x001E0B02
	private void UpdateCaravanBlockCharData(List<CaravanDisplayData> caravanList)
	{
		this._caravanList.Clear();
		this._caravanList.AddRange(caravanList);
		this.UpdateDataFromSvr(false, null);
	}

	// Token: 0x06003B7E RID: 15230 RVA: 0x001E2928 File Offset: 0x001E0B28
	private void OnUpdateCaravanBlockCharData(ArgumentBox argumentBox)
	{
		List<CaravanDisplayData> caravanList;
		argumentBox.Get<List<CaravanDisplayData>>("caravanList", out caravanList);
		this.UpdateCaravanBlockCharData(caravanList);
	}

	// Token: 0x06003B7F RID: 15231 RVA: 0x001E294C File Offset: 0x001E0B4C
	private void PlayHideAnim()
	{
		bool isPlayAnimForProfession = this._isPlayAnimForProfession;
		if (!isPlayAnimForProfession)
		{
			bool flag = !this._uiAnim.AnimSequenceOut.IsPlaying() && Math.Abs(this._rectTrans.anchoredPosition.x - this._originPosition.x) > 0.1f;
			if (flag)
			{
				this._uiAnim.PlayHideAnimation(null, false);
			}
		}
	}

	// Token: 0x06003B80 RID: 15232 RVA: 0x001E29B8 File Offset: 0x001E0BB8
	private void PlayShowAnim()
	{
		bool isPlayAnimForProfession = this._isPlayAnimForProfession;
		if (!isPlayAnimForProfession)
		{
			bool flag = !this._uiAnim.AnimSequenceIn.IsPlaying() && Math.Abs(this._rectTrans.anchoredPosition.x - 0f) > 0.1f;
			if (flag)
			{
				this._uiAnim.PlayShowAnimation(null, false);
			}
		}
	}

	// Token: 0x06003B81 RID: 15233 RVA: 0x001E2A1C File Offset: 0x001E0C1C
	protected override void OnClick(Transform btn)
	{
		base.OnClick(btn);
		bool flag = btn.name == "SettingButton";
		if (flag)
		{
			UIManager.Instance.ShowUI(UIElement.MapBlockCharCustomSetting, true);
		}
	}

	// Token: 0x06003B82 RID: 15234 RVA: 0x001E2A59 File Offset: 0x001E0C59
	private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
	{
		this.PlayHideAnim();
		this._isPlayAnimForProfession = true;
	}

	// Token: 0x06003B83 RID: 15235 RVA: 0x001E2A6C File Offset: 0x001E0C6C
	private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
	{
		bool flag = argumentBox == null;
		if (flag)
		{
			argumentBox = EasyPool.Get<ArgumentBox>();
		}
		this._isPlayAnimForProfession = false;
		this.OnSelectedBlockChange(argumentBox.SetObject("block", this._mapModel.GetBlockData(this._mapModel.CurrentBlockId)));
	}

	// Token: 0x06003B84 RID: 15236 RVA: 0x001E2AB8 File Offset: 0x001E0CB8
	private void InitRefers()
	{
		this._charScroll = base.CGet<InfinityScrollLegacy>("CharScroll");
		this._togGroup = base.CGet<CToggleGroupObsolete>("TogGroup");
		this._mapBlockCharNormalWrapper = base.CGet<Refers>("MapBlockCharNormalWrapper");
		this._mapBlockCharEnemyWrapper = base.CGet<Refers>("MapBlockCharEnemyWrapper");
		this._mapBlockCharCaravanWrapper = base.CGet<Refers>("MapBlockCharCaravanWrapper");
		this._mapBlockCharGraveWrapper = base.CGet<Refers>("MapBlockCharGraveWrapper");
		this._mapBlockCharUnknownWrapper = base.CGet<Refers>("MapBlockCharUnknownWrapper");
		this._mapBlockCharInformationWrapper = base.CGet<Refers>("MapBlockCharInformationWrapper");
		this._searchInputField = base.CGet<TMP_InputField>("SearchInputField");
		this._countLabel = base.CGet<TextMeshProUGUI>("CountLabel");
	}

	// Token: 0x06003B89 RID: 15241 RVA: 0x001E2D04 File Offset: 0x001E0F04
	[CompilerGenerated]
	private void <OnInit>g__InitRefresh|63_1()
	{
		MapBlockData block = this._block ?? this._mapModel.GetBlockData(this._mapModel.CurrentBlockId);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("block", block);
		this.OnSelectedBlockChange(argumentBox);
	}

	// Token: 0x04002AAF RID: 10927
	public const int TogKeyInvalid = -1;

	// Token: 0x04002AB0 RID: 10928
	public const int TogKeyNormal = 0;

	// Token: 0x04002AB1 RID: 10929
	public const int TogKeyEnemy = 1;

	// Token: 0x04002AB2 RID: 10930
	public const int TogKeyCaravan = 2;

	// Token: 0x04002AB3 RID: 10931
	public const int TogKeyGrave = 3;

	// Token: 0x04002AB4 RID: 10932
	public const int TogKeyInformation = 4;

	// Token: 0x04002AB5 RID: 10933
	public const string NpcAvatarTexturePath = "RemakeResources/Textures/NpcFace/SmallFace/";

	// Token: 0x04002AB6 RID: 10934
	private WorldMapModel _mapModel;

	// Token: 0x04002AB7 RID: 10935
	private MapBlockData _block;

	// Token: 0x04002AB8 RID: 10936
	private bool _canSeeDetail;

	// Token: 0x04002AB9 RID: 10937
	private bool _canInteract;

	// Token: 0x04002ABA RID: 10938
	private bool _needResetTog = false;

	// Token: 0x04002ABB RID: 10939
	private bool _needUpdateOnFocus;

	// Token: 0x04002ABC RID: 10940
	private readonly List<int> _normalCharList = new List<int>();

	// Token: 0x04002ABD RID: 10941
	private readonly List<int> _infectedList = new List<int>();

	// Token: 0x04002ABE RID: 10942
	private readonly List<int> _legendaryBookOwnerEnemyList = new List<int>();

	// Token: 0x04002ABF RID: 10943
	private readonly List<int> _normalEnemyList = new List<int>();

	// Token: 0x04002AC0 RID: 10944
	private readonly List<MapTemplateEnemyInfo> _randomEnemyList = new List<MapTemplateEnemyInfo>();

	// Token: 0x04002AC1 RID: 10945
	private readonly List<int> _animalEnemyList = new List<int>();

	// Token: 0x04002AC2 RID: 10946
	private readonly List<int> _graveList = new List<int>();

	// Token: 0x04002AC3 RID: 10947
	private readonly List<int> _specialCharList = new List<int>();

	// Token: 0x04002AC4 RID: 10948
	private readonly Dictionary<int, CharacterDisplayData> _charDataDict = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x04002AC5 RID: 10949
	private readonly Dictionary<int, GraveDisplayData> _graveDataDict = new Dictionary<int, GraveDisplayData>();

	// Token: 0x04002AC6 RID: 10950
	private readonly List<CaravanDisplayData> _caravanList = new List<CaravanDisplayData>();

	// Token: 0x04002AC7 RID: 10951
	private readonly Dictionary<int, CaravanDisplayData> _caravanCharDict = new Dictionary<int, CaravanDisplayData>();

	// Token: 0x04002AC8 RID: 10952
	private List<SectStoryHeavenlyTreeExtendable> _treeList;

	// Token: 0x04002AC9 RID: 10953
	private readonly List<CharacterDisplayDataWithInfo> _characterDisplayDataWithInfoList = new List<CharacterDisplayDataWithInfo>();

	// Token: 0x04002ACA RID: 10954
	private readonly List<int> _searchedNormalCharList = new List<int>();

	// Token: 0x04002ACB RID: 10955
	private readonly List<int> _searchedNormalEnemyList = new List<int>();

	// Token: 0x04002ACC RID: 10956
	private readonly List<MapTemplateEnemyInfo> _searchedRandomEnemyList = new List<MapTemplateEnemyInfo>();

	// Token: 0x04002ACD RID: 10957
	private readonly List<int> _searchedGraveList = new List<int>();

	// Token: 0x04002ACE RID: 10958
	private readonly List<int> _searchedSpecialCharList = new List<int>();

	// Token: 0x04002ACF RID: 10959
	private readonly List<CharacterDisplayDataWithInfo> _searchedInfoList = new List<CharacterDisplayDataWithInfo>();

	// Token: 0x04002AD0 RID: 10960
	private readonly Dictionary<int, int> _searchedRandomEnemyListIndexToAnimalId = new Dictionary<int, int>();

	// Token: 0x04002AD1 RID: 10961
	private int _lastTogKey;

	// Token: 0x04002AD2 RID: 10962
	private Vector2 _originPosition;

	// Token: 0x04002AD3 RID: 10963
	private RectTransform _rectTrans;

	// Token: 0x04002AD4 RID: 10964
	private const float _targetPositionX = 0f;

	// Token: 0x04002AD5 RID: 10965
	private UIAnim _uiAnim;

	// Token: 0x04002AD6 RID: 10966
	private bool _skipRefreshData;

	// Token: 0x04002AD7 RID: 10967
	private bool _isUpdatingData;

	// Token: 0x04002AD8 RID: 10968
	private int _targetTogKey = -1;

	// Token: 0x04002AD9 RID: 10969
	private const string MapBlockCharUnknownKey = "MapBlockCharUnknownWrapper";

	// Token: 0x04002ADA RID: 10970
	private const string MapBlockCharNormalKey = "MapBlockCharNormalWrapper";

	// Token: 0x04002ADB RID: 10971
	private const string MapBlockCharEnemyKey = "MapBlockCharEnemyWrapper";

	// Token: 0x04002ADC RID: 10972
	private const string MapBlockCharGraveKey = "MapBlockCharGraveWrapper";

	// Token: 0x04002ADD RID: 10973
	private const string MapBlockCharCaravanKey = "MapBlockCharCaravanWrapper";

	// Token: 0x04002ADE RID: 10974
	private const string MapBlockCharInformationKey = "MapBlockCharInformationWrapper";

	// Token: 0x04002ADF RID: 10975
	private bool _isWartingData;

	// Token: 0x04002AE0 RID: 10976
	private static TMP_InputField _staticSearchInputField;

	// Token: 0x04002AE1 RID: 10977
	private List<LoongInfo> _loongInfos;

	// Token: 0x04002AE2 RID: 10978
	private bool _isPlayAnimForProfession;

	// Token: 0x04002AE3 RID: 10979
	private InfinityScrollLegacy _charScroll;

	// Token: 0x04002AE4 RID: 10980
	private CToggleGroupObsolete _togGroup;

	// Token: 0x04002AE5 RID: 10981
	private Refers _mapBlockCharNormalWrapper;

	// Token: 0x04002AE6 RID: 10982
	private Refers _mapBlockCharEnemyWrapper;

	// Token: 0x04002AE7 RID: 10983
	private Refers _mapBlockCharCaravanWrapper;

	// Token: 0x04002AE8 RID: 10984
	private Refers _mapBlockCharGraveWrapper;

	// Token: 0x04002AE9 RID: 10985
	private Refers _mapBlockCharUnknownWrapper;

	// Token: 0x04002AEA RID: 10986
	private Refers _mapBlockCharInformationWrapper;

	// Token: 0x04002AEB RID: 10987
	private TMP_InputField _searchInputField;

	// Token: 0x04002AEC RID: 10988
	private TextMeshProUGUI _countLabel;
}
