using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Bottom;
using Game.Views.CharacterMenu;
using Game.Views.WorldStatePanel;
using GameData.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Adventure;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	// Token: 0x0200071E RID: 1822
	public class ViewWorldStatePanel : UIBase
	{
		// Token: 0x060056D5 RID: 22229 RVA: 0x002847C0 File Offset: 0x002829C0
		public override void OnInit(ArgumentBox argsBox)
		{
			this.SetWorldInfo();
			this.sectImage.gameObject.SetActive(false);
			this._isInfocusMode = false;
			this.dropdownMask.gameObject.SetActive(false);
		}

		// Token: 0x060056D6 RID: 22230 RVA: 0x002847F8 File Offset: 0x002829F8
		private void SetWorldInfo()
		{
			sbyte xiangshuProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
			sbyte xiangshuLevel = GameData.Domains.World.SharedMethods.GetXiangshuLevel(xiangshuProgress);
			this.worldProgressLevel.text = LanguageKey.LK_WorldState_WorldProgress.Tr() + LocalStringManager.Get(string.Format("LK_Number{0}", xiangshuLevel)).SetColor("cdb149");
			this.approvingRate.text = LanguageKey.LK_WorldState_SupportLimit.Tr() + string.Format("{0}%", GlobalConfig.Instance.SectApprovingRateUpperLimits[(int)xiangshuLevel]).SetColor("8dc3c3");
			int[] requirements = GlobalConfig.Instance.MerchantFavorabilityXiangshuLevelRequirements;
			int overIndex = 0;
			for (int i = 0; i < requirements.Length; i++)
			{
				bool flag = GlobalConfig.Instance.MerchantFavorabilityXiangshuLevelRequirements[i] > (int)xiangshuProgress;
				if (flag)
				{
					overIndex = i;
					break;
				}
			}
			int num = (int)xiangshuProgress;
			int[] array = requirements;
			bool flag2 = num >= array[array.Length - 1];
			if (flag2)
			{
				overIndex = requirements.Length;
			}
			int worldProgressLimitedFavor = overIndex * 10;
			int worldProgressLimitedLevel = GameData.Domains.Merchant.SharedMethods.GetFavorLevel(worldProgressLimitedFavor);
			this.merchantFavorability.text = LanguageKey.LK_WorldState_MerchantLevel.Tr() + LocalStringManager.Get(string.Format("LK_TraditionalChineseNumber_{0}", worldProgressLimitedLevel + 1)).SetColor(Colors.Instance.GradeColors[worldProgressLimitedLevel + 2]);
			sbyte maxGrade = GlobalConfig.Instance.XiangshuInfectionGradeUpperLimits[(int)xiangshuLevel];
			Color color = Colors.Instance.GradeColors[(int)maxGrade];
			this.xiangshuInfectLevel.text = LanguageKey.LK_WorldState_XiangshuInfectLevel.Tr() + LocalStringManager.Get(string.Format("LK_OrgGrade_Short_{0}", maxGrade)).SetColor(color);
			this.teammateLevel.text = LanguageKey.LK_WorldState_TeammateLevel.Tr() + LocalStringManager.Get(string.Format("LK_OrgGrade_Short_{0}", maxGrade)).SetColor(color);
		}

		// Token: 0x060056D7 RID: 22231 RVA: 0x002849D9 File Offset: 0x00282BD9
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.UpdateTime));
			this.UpdateTime(null);
		}

		// Token: 0x060056D8 RID: 22232 RVA: 0x002849FC File Offset: 0x00282BFC
		private void Update()
		{
			bool flag = CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) && this.dropdownMask.gameObject.activeSelf;
			if (flag)
			{
				this.IsInFocusMode = false;
			}
		}

		// Token: 0x060056D9 RID: 22233 RVA: 0x00284A44 File Offset: 0x00282C44
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCloseView"))
			{
				if (a == "DropdownMask")
				{
					this.IsInFocusMode = false;
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x060056DA RID: 22234 RVA: 0x00284A8C File Offset: 0x00282C8C
		private void OnDisable()
		{
			bool isInFocusMode = this.IsInFocusMode;
			if (isInFocusMode)
			{
				this.IsInFocusMode = false;
			}
			this.ClearInstances();
			PoolManager.RemoveData("ViewWorldStatePanel_CharTemplate");
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.UpdateTime));
		}

		// Token: 0x060056DB RID: 22235 RVA: 0x00284AD8 File Offset: 0x00282CD8
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(1, 31, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 62, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 22, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(3, 7, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(3, 18, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 95, ulong.MaxValue, null));
		}

		// Token: 0x060056DC RID: 22236 RVA: 0x00284B70 File Offset: 0x00282D70
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						this.HandlerMethodReturn(notification, wrapper);
					}
				}
				else
				{
					this.HandlerDataModification(notification, wrapper);
				}
			}
		}

		// Token: 0x060056DD RID: 22237 RVA: 0x00284BF0 File Offset: 0x00282DF0
		private void HandlerDataModification(Notification notification, NotificationWrapper wrapper)
		{
			DataUid uid = notification.Uid;
			ushort domainId = uid.DomainId;
			ushort num = domainId;
			switch (num)
			{
			case 1:
				if (uid.DataId == 31)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._worldStateData);
				}
				break;
			case 2:
			case 4:
				break;
			case 3:
				if (uid.DataId != 7)
				{
					if (uid.DataId == 18)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._tournamentPreparationEndDate);
					}
				}
				else
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._martialArtTournamentPreparationInfoList);
				}
				break;
			case 5:
				if (uid.DataId != 62)
				{
					if (uid.DataId == 22)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._moveTimeCostPercent);
					}
				}
				else
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._overweightSanctionPercent);
				}
				break;
			default:
				if (num == 19)
				{
					if (uid.DataId == 95)
					{
						Dictionary<short, LoongInfo> fiveLoongDict = new Dictionary<short, LoongInfo>();
						Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, fiveLoongDict);
						int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
						foreach (KeyValuePair<short, LoongInfo> pair in fiveLoongDict)
						{
							ushort count;
							this._loongDebuff[pair.Key] = ((pair.Value.CharacterDebuffCounts != null && pair.Value.CharacterDebuffCounts.TryGetValue(taiwuCharId, out count)) ? count : 0);
						}
						this.RefreshWorldState();
						this.Element.ShowAfterRefresh();
					}
				}
				break;
			}
		}

		// Token: 0x060056DE RID: 22238 RVA: 0x00284DD4 File Offset: 0x00282FD4
		private void HandlerMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
		}

		// Token: 0x060056DF RID: 22239 RVA: 0x00284DD8 File Offset: 0x00282FD8
		private void RefreshWorldState()
		{
			this._worldStateItems.Clear();
			foreach (WorldStateItem worldStateItem in ((IEnumerable<WorldStateItem>)WorldState.Instance))
			{
				bool worldState = this._worldStateData.GetWorldState((short)worldStateItem.TemplateId);
				bool flag = this._invasionWorldStateTemplateIds.Contains(worldStateItem.TemplateId);
				if (flag)
				{
					this.UpdateXiangshuProcess(worldStateItem, worldState);
				}
				else
				{
					bool flag2 = worldState;
					if (flag2)
					{
						bool flag3 = worldStateItem.TemplateId >= 25 && worldStateItem.TemplateId <= 39;
						if (flag3)
						{
							this.sectImage.gameObject.SetActive(true);
							this.UpdateSectTask(worldStateItem);
						}
						else
						{
							this._worldStateItems.Add(worldStateItem);
						}
					}
				}
			}
			CommonUtils.PrepareEnoughChildren(this.scrollContent, this.panelItemTemplate.gameObject, this._worldStateItems.Count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
			{
				TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems,
				ExtraItemCount = 1
			}));
			for (int i = 0; i < this._worldStateItems.Count; i++)
			{
				WorldStateItem worldStateItem2 = this._worldStateItems[i];
				this.SetData(worldStateItem2, this.scrollContent.GetChild(i + 1).GetComponent<WorldStatePanelItem>());
			}
			this.emptyIcon.SetActive(this._worldStateItems.Count == 0);
		}

		// Token: 0x060056E0 RID: 22240 RVA: 0x00284F68 File Offset: 0x00283168
		private void SetData(WorldStateItem worldStateItem, WorldStatePanelItem panelItem)
		{
			CImage icon = panelItem.icon;
			TextMeshProUGUI desc = panelItem.desc;
			CButton jumpButton = panelItem.jumpButton;
			RectTransform contentHolder = panelItem.contentHolder;
			TextMeshProUGUI subTitle = panelItem.subTitle;
			GameObject conditionPrefab = panelItem.conditionPrefab;
			TextMeshProUGUI extraDescLabel = panelItem.extraDescLabel;
			RectTransform conditionRoot = panelItem.conditionRoot;
			TextMeshProUGUI specialText = panelItem.specialText;
			icon.SetSprite(worldStateItem.Icon, false, null);
			this.SetWorldStateItemData(worldStateItem, panelItem);
			bool enableInteract = this.CheckInteractEnable(worldStateItem);
			jumpButton.gameObject.SetActive(enableInteract);
			bool flag = enableInteract;
			if (flag)
			{
				jumpButton.ClearAndAddListener(delegate
				{
					this.OnJumpButtonClicked(worldStateItem, panelItem);
				});
			}
		}

		// Token: 0x060056E1 RID: 22241 RVA: 0x00285064 File Offset: 0x00283264
		private void SetWorldStateItemData(WorldStateItem worldStateItem, WorldStatePanelItem panelItem)
		{
			ViewWorldStatePanel.<>c__DisplayClass45_0 CS$<>8__locals1 = new ViewWorldStatePanel.<>c__DisplayClass45_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.worldStateItem = worldStateItem;
			CS$<>8__locals1.panelItem = panelItem;
			CS$<>8__locals1.desc = CS$<>8__locals1.panelItem.desc;
			foreach (object obj in CS$<>8__locals1.desc.transform.parent)
			{
				Transform child = (Transform)obj;
				child.gameObject.SetActive(false);
			}
			sbyte templateId2 = CS$<>8__locals1.worldStateItem.TemplateId;
			sbyte b = templateId2;
			if (b >= 42)
			{
				if (b <= 46)
				{
					foreach (KeyValuePair<short, ushort> pair in this._loongDebuff)
					{
						sbyte templateId = (sbyte)Loong.Instance[(int)(pair.Key - 246)].WorldState;
						CS$<>8__locals1.worldStateItem = WorldState.Instance.GetItem(templateId);
						bool needShow = this._worldStateData.GetWorldState((short)templateId);
						bool flag = pair.Value > 0 && needShow;
						if (flag)
						{
							CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(pair.Value.ToString()).ColorReplace();
							CS$<>8__locals1.desc.gameObject.SetActive(true);
							break;
						}
					}
					return;
				}
				switch (b)
				{
				case 49:
					OrganizationDomainMethod.AsyncCall.GetBountyCharacterDisplayDataFromCharacterList(null, SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds(), delegate(int offset, RawDataPool pool)
					{
						SettlementBountyDisplayData data = new SettlementBountyDisplayData();
						Serializer.Deserialize(pool, offset, ref data);
						CS$<>8__locals1.<>4__this.StartCoroutine(CS$<>8__locals1.<>4__this.UpdateTaiwuWanted(CS$<>8__locals1.panelItem.taiwuWantedTemplate, data));
					});
					return;
				case 51:
					TaiwuDomainMethod.AsyncCall.GetDyingGroupCharNames(this, true, delegate(int offset, RawDataPool dataPool)
					{
						List<CharNameRelatedData> charNames = new List<CharNameRelatedData>();
						Serializer.Deserialize(dataPool, offset, ref charNames);
						bool includeTaiwu = false;
						int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
						CS$<>8__locals1.<>4__this._dyingCharIds.Clear();
						List<NameRelatedData> nameList = EasyPool.Get<List<NameRelatedData>>();
						nameList.Clear();
						foreach (CharNameRelatedData charName in charNames)
						{
							CS$<>8__locals1.<>4__this._dyingCharIds.Add(charName.CharId);
							nameList.Add(charName.NameData);
							bool flag15 = charName.CharId == taiwuCharId;
							if (flag15)
							{
								includeTaiwu = true;
							}
						}
						CS$<>8__locals1.desc.text = string.Format(CS$<>8__locals1.worldStateItem.Desc, NameCenter.GetNameSequenceStringByNameRelatedDataList(nameList, includeTaiwu).SetColor("ec5f68")).ColorReplace();
						CS$<>8__locals1.desc.gameObject.SetActive(true);
						EasyPool.Free<List<NameRelatedData>>(nameList);
					});
					return;
				case 52:
					BuildingDomainMethod.AsyncCall.GetResidenceInfo(this, delegate(int offset, RawDataPool dataPool)
					{
						ValueTuple<int, int, int> info = default(ValueTuple<int, int, int>);
						Serializer.Deserialize(dataPool, offset, ref info);
						CS$<>8__locals1.desc.text = string.Format(CS$<>8__locals1.worldStateItem.Desc, info.Item1, info.Item2, info.Item3);
						CS$<>8__locals1.desc.GetComponent<TMPTextSpriteHelper>().Parse();
						CS$<>8__locals1.desc.gameObject.SetActive(true);
					});
					return;
				case 53:
					TaiwuDomainMethod.AsyncCall.GetGroupNeiliConflictingCharDataList(this, delegate(int offset, RawDataPool dataPool)
					{
						List<CharacterDisplayData> list = new List<CharacterDisplayData>();
						Serializer.Deserialize(dataPool, offset, ref list);
						StringBuilder stringBuilder2 = EasyPool.Get<StringBuilder>();
						int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
						stringBuilder2.Clear();
						for (int k = 0; k < list.Count; k++)
						{
							CharacterDisplayData charData = list[k];
							bool isTaiwu = charData.CharacterId == taiwuCharId;
							string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, isTaiwu).SetColor("orange");
							stringBuilder2.Append(charName);
							bool flag15 = k < list.Count - 1;
							if (flag15)
							{
								string separator = LocalStringManager.Get(LanguageKey.LK_Separator);
								stringBuilder2.Append(separator);
							}
						}
						CS$<>8__locals1.desc.text = string.Format(CS$<>8__locals1.worldStateItem.Desc, stringBuilder2);
						CS$<>8__locals1.desc.gameObject.SetActive(true);
						EasyPool.Free<StringBuilder>(stringBuilder2);
					});
					return;
				case 54:
					CS$<>8__locals1.panelItem.practiceNoticeItem.gameObject.SetActive(true);
					CS$<>8__locals1.panelItem.practiceNoticeItem.Set(this._worldStateData);
					return;
				}
			}
			else if (b >= 25)
			{
				if (b <= 39)
				{
					return;
				}
				if (b == 40)
				{
					TaiwuDomainMethod.AsyncCall.GetSeverelyInjuredGroupCharNames(this, delegate(int offset, RawDataPool dataPool)
					{
						List<CharNameRelatedData> charNames = null;
						Serializer.Deserialize(dataPool, offset, ref charNames);
						bool flag15 = charNames != null && charNames.Count > 0;
						if (flag15)
						{
							CS$<>8__locals1.<>4__this._severelyInjuredCharIds.Clear();
							List<NameRelatedData> nameList = EasyPool.Get<List<NameRelatedData>>();
							nameList.Clear();
							foreach (CharNameRelatedData charName in charNames)
							{
								CS$<>8__locals1.<>4__this._severelyInjuredCharIds.Add(charName.CharId);
								nameList.Add(charName.NameData);
							}
							CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(NameCenter.GetNameSequenceStringByNameRelatedDataList(nameList, false).SetColor("ec5f68")).ColorReplace();
							CS$<>8__locals1.desc.gameObject.SetActive(true);
							EasyPool.Free<List<NameRelatedData>>(nameList);
						}
					});
					return;
				}
			}
			else
			{
				switch (b)
				{
				case 11:
					this.UpdateInventoryOverflow(CS$<>8__locals1.worldStateItem, CS$<>8__locals1.panelItem);
					return;
				case 12:
				{
					StringBuilder sb = EasyPool.Get<StringBuilder>();
					sb.Clear();
					foreach (WorldStateData.EStorageType t in this._worldStateData.IterateOverloadStorageTypes())
					{
						bool flag2 = sb.Length > 0;
						if (flag2)
						{
							sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
						}
						StringBuilder stringBuilder = sb;
						if (!true)
						{
						}
						LanguageKey id;
						if (t != WorldStateData.EStorageType.Warehouse)
						{
							if (t != WorldStateData.EStorageType.Trough)
							{
								throw new ArgumentOutOfRangeException();
							}
							id = LanguageKey.LK_Trough;
						}
						else
						{
							id = LanguageKey.LK_Storage;
						}
						if (!true)
						{
						}
						stringBuilder.Append(LocalStringManager.Get(id));
					}
					CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(sb.ToString()).ColorReplace();
					CS$<>8__locals1.desc.gameObject.SetActive(true);
					EasyPool.Free<StringBuilder>(sb);
					return;
				}
				case 13:
				{
					StringBuilder resourceNameBuilder = EasyPool.Get<StringBuilder>();
					resourceNameBuilder.Clear();
					for (sbyte type = 0; type < 6; type += 1)
					{
						bool flag3 = this._worldStateData.IsResourceOverloaded(type);
						if (flag3)
						{
							bool flag4 = resourceNameBuilder.Length > 0;
							if (flag4)
							{
								resourceNameBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
							}
							resourceNameBuilder.Append(Config.ResourceType.Instance[type].Name);
						}
					}
					bool flag5 = resourceNameBuilder.Length > 0;
					if (flag5)
					{
						CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(resourceNameBuilder.ToString()).ColorReplace();
						CS$<>8__locals1.desc.gameObject.SetActive(true);
					}
					EasyPool.Free<StringBuilder>(resourceNameBuilder);
					return;
				}
				case 14:
				case 15:
				{
					StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
					strBuilder.Clear();
					for (sbyte type2 = 0; type2 < 7; type2 += 1)
					{
						bool isInjury = (CS$<>8__locals1.worldStateItem.TemplateId == 14) ? this._worldStateData.BodyPartHasOuterInjury(type2) : this._worldStateData.BodyPartHasInnerInjury(type2);
						bool flag6 = isInjury;
						if (flag6)
						{
							bool flag7 = strBuilder.Length > 0;
							if (flag7)
							{
								strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
							}
							strBuilder.Append(BodyPart.Instance[type2].Name);
						}
					}
					bool flag8 = strBuilder.Length > 0;
					if (flag8)
					{
						CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(strBuilder.ToString()).ColorReplace();
						CS$<>8__locals1.desc.gameObject.SetActive(true);
					}
					EasyPool.Free<StringBuilder>(strBuilder);
					return;
				}
				case 17:
				{
					StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
					strBuilder.Clear();
					for (sbyte order = 0; order < 6; order += 1)
					{
						sbyte poisonType = PoisonType.GetTypeBySortingOrder(order);
						bool isPoisoned = this._worldStateData.IsPoisonedWithType(poisonType);
						bool flag9 = isPoisoned;
						if (flag9)
						{
							PoisonItem configData = Poison.Instance[poisonType];
							bool flag10 = strBuilder.Length > 0;
							if (flag10)
							{
								strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
							}
							strBuilder.Append(configData.Name.SetColor(configData.FontColor));
						}
					}
					bool flag11 = strBuilder.Length > 0;
					if (flag11)
					{
						CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(strBuilder.ToString()).ColorReplace();
						CS$<>8__locals1.desc.gameObject.SetActive(true);
					}
					EasyPool.Free<StringBuilder>(strBuilder);
					return;
				}
				case 20:
					for (sbyte i = 0; i < 9; i += 1)
					{
						bool isAwakening = this._worldStateData.IsXiangshuAvatarAwakening(i);
						string swordTombName = AdventureRemakeModel.Core.GetAdventureAny(SwordTomb.Instance[i].AdventureCoreId).Name;
						bool flag12 = !isAwakening;
						if (!flag12)
						{
							CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(swordTombName, this._worldStateData.GetMinAwakeSwordTombRemainMonths()).ColorReplace();
							CS$<>8__locals1.desc.gameObject.SetActive(true);
							break;
						}
					}
					return;
				case 21:
					for (sbyte j = 0; j < 9; j += 1)
					{
						bool isAwakening2 = this._worldStateData.IsXiangshuAvatarAwakening(j);
						string swordTombName2 = AdventureRemakeModel.Core.GetAdventureAny(SwordTomb.Instance[j].AdventureCoreId).Name;
						bool flag13 = !isAwakening2;
						if (!flag13)
						{
							CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(swordTombName2).ColorReplace();
							CS$<>8__locals1.desc.gameObject.SetActive(true);
							break;
						}
					}
					return;
				case 22:
				{
					List<MartialArtTournamentPreparationInfo> martialArtTournamentPreparationInfoList = this._martialArtTournamentPreparationInfoList;
					bool flag14 = martialArtTournamentPreparationInfoList == null || martialArtTournamentPreparationInfoList.Count <= 0;
					if (flag14)
					{
						return;
					}
					this._martialArtTournamentPreparationInfoList.Sort();
					this._martialArtTournamentPreparationInfoList.Reverse();
					List<short> settlementIdList = this._martialArtTournamentPreparationInfoList.ConvertAll<short>((MartialArtTournamentPreparationInfo e) => e.SettlementId);
					int remainingMonth;
					OrganizationDomainMethod.AsyncCall.GetSettlementNameRelatedData(this, settlementIdList, delegate(int offset, RawDataPool dataPool)
					{
						List<SettlementNameRelatedData> nameRelatedDataList = new List<SettlementNameRelatedData>();
						Serializer.Deserialize(dataPool, offset, ref nameRelatedDataList);
						List<string> prepareDescList = new List<string>
						{
							LocalStringManager.Get(LanguageKey.UI_WorldState_MartialArtTournament_State_PrepairPower)
						};
						string baseStr = LocalStringManager.Get(LanguageKey.UI_WorldState_MartialArtTournament_Prepair_SectDesc);
						for (int k = 0; k < CS$<>8__locals1.<>4__this._martialArtTournamentPreparationInfoList.Count; k++)
						{
							SettlementNameRelatedData nameRelatedData = nameRelatedDataList[k];
							MartialArtTournamentPreparationInfo info = CS$<>8__locals1.<>4__this._martialArtTournamentPreparationInfoList[k];
							string settlementName = LocalStringManager.Get(string.Format("LK_Sect_Name_Short_{0}", (int)(nameRelatedData.MapBlockTemplateId - 19 + 1)));
							prepareDescList.Add(baseStr.GetFormat(new object[]
							{
								settlementName,
								info.TotalScore,
								info.CombatPowerPreparation,
								info.ResourcePreparation,
								info.AuthorityPreparation
							}));
							CS$<>8__locals1.<>4__this._settlementNameRelatedDataCache.Remove(settlementIdList[k]);
							CS$<>8__locals1.<>4__this._settlementNameRelatedDataCache.Add(settlementIdList[k], nameRelatedDataList[k]);
						}
						prepareDescList.Add(string.Empty);
						int remainingMonth = CS$<>8__locals1.<>4__this._tournamentPreparationEndDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
						prepareDescList.Add(LocalStringManager.GetFormat(LanguageKey.UI_WorldState_MartialArtTournament_Prepair_MonthCountToOpen, remainingMonth));
						CS$<>8__locals1.desc.text = string.Join("\n", prepareDescList);
						CS$<>8__locals1.desc.gameObject.SetActive(true);
					});
					return;
				}
				case 24:
				{
					IAdventureRuntime adventure;
					int remainingMonth = SingletonObject.getInstance<AdventureRemakeModel>().TryGetAnyByCoreId(114668976, out adventure) ? adventure.RemainMonths : 0;
					OrganizationDomainMethod.AsyncCall.GetMartialArtTournamentCurrentHostSettlementId(this, delegate(int offset, RawDataPool dataPool)
					{
						short settlementId = -1;
						Serializer.Deserialize(dataPool, offset, ref settlementId);
						SettlementNameRelatedData nameRelatedData;
						bool flag15 = CS$<>8__locals1.<>4__this._settlementNameRelatedDataCache.TryGetValue(settlementId, out nameRelatedData);
						if (flag15)
						{
							string sectName = CommonUtils.GetSettlementString(nameRelatedData.RandomNameId, nameRelatedData.MapBlockTemplateId);
							CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(sectName, remainingMonth.ToString()).ColorReplace();
							CS$<>8__locals1.desc.gameObject.SetActive(true);
						}
						else
						{
							List<short> list = new List<short>
							{
								settlementId
							};
							OrganizationDomainMethod.AsyncCall.GetSettlementNameRelatedData(CS$<>8__locals1.<>4__this, list, delegate(int offsetSingle, RawDataPool dataPoolSingle)
							{
								List<SettlementNameRelatedData> nameRelatedDataList = new List<SettlementNameRelatedData>();
								Serializer.Deserialize(dataPoolSingle, offsetSingle, ref nameRelatedDataList);
								nameRelatedData = nameRelatedDataList[0];
								CS$<>8__locals1.<>4__this._settlementNameRelatedDataCache.Remove(settlementId);
								CS$<>8__locals1.<>4__this._settlementNameRelatedDataCache.Add(settlementId, nameRelatedDataList[0]);
								string sectName2 = CommonUtils.GetSettlementString(nameRelatedData.RandomNameId, nameRelatedData.MapBlockTemplateId);
								CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.GetFormat(sectName2, remainingMonth.ToString()).ColorReplace();
								CS$<>8__locals1.desc.gameObject.SetActive(true);
							});
						}
					});
					return;
				}
				}
			}
			CS$<>8__locals1.desc.text = CS$<>8__locals1.worldStateItem.Desc.ColorReplace();
			CS$<>8__locals1.desc.gameObject.SetActive(true);
		}

		// Token: 0x060056E2 RID: 22242 RVA: 0x00285A4C File Offset: 0x00283C4C
		private void UpdateInventoryOverflow(WorldStateItem worldStateItem, WorldStatePanelItem panelItem)
		{
			ViewWorldStatePanel.<>c__DisplayClass46_0 CS$<>8__locals1 = new ViewWorldStatePanel.<>c__DisplayClass46_0();
			CS$<>8__locals1.panelItem = panelItem;
			bool flag = this._overweightSanctionPercent == null;
			if (!flag)
			{
				int finalSpeed = this._moveTimeCostPercent - 100;
				int sanction = 0;
				foreach (IntPair pair in this._overweightSanctionPercent)
				{
					sanction = Math.Max(pair.Second, sanction);
				}
				CS$<>8__locals1.panelItem.desc.gameObject.SetActive(true);
				CS$<>8__locals1.panelItem.desc.text = worldStateItem.Desc.ColorReplace();
				CS$<>8__locals1.panelItem.subTitle.gameObject.SetActive(true);
				CS$<>8__locals1.panelItem.subTitle.text = LocalStringManager.Get(LanguageKey.UI_WorldState_InventoryOverload_MoveTimeCost);
				CS$<>8__locals1.conditionList = new List<MouseTipDynamicCondition.ConditionData>();
				bool flag2 = sanction != 0;
				if (flag2)
				{
					MouseTipDynamicCondition.ConditionData burdenPunishmentCondition = new MouseTipDynamicCondition.ConditionData
					{
						Name = LocalStringManager.Get(LanguageKey.UI_WorldState_InventoryOverload_BurdenPunishment)
					};
					bool flag3 = sanction > 0;
					if (flag3)
					{
						burdenPunishmentCondition.ReduceValueString = string.Format("{0}%", sanction);
					}
					else
					{
						burdenPunishmentCondition.AddValueString = string.Format("{0}%", sanction);
					}
					CS$<>8__locals1.conditionList.Add(burdenPunishmentCondition);
				}
				bool flag4 = finalSpeed != 0;
				if (flag4)
				{
					MouseTipDynamicCondition.ConditionData moveTimeCostPunishmentCondition = new MouseTipDynamicCondition.ConditionData
					{
						Name = LocalStringManager.Get(LanguageKey.UI_WorldState_InventoryOverload_MoveTimeCostPunishment)
					};
					bool flag5 = finalSpeed > 0;
					if (flag5)
					{
						moveTimeCostPunishmentCondition.ReduceValueString = string.Format("+{0}%", finalSpeed);
					}
					else
					{
						moveTimeCostPunishmentCondition.AddValueString = string.Format("{0}%", finalSpeed);
					}
					CS$<>8__locals1.conditionList.Add(moveTimeCostPunishmentCondition);
				}
				CS$<>8__locals1.<UpdateInventoryOverflow>g__SetConditionList|1();
				TaiwuDomainMethod.AsyncCall.GetInventoryOverloadedGroupCharNames(null, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayData> charDataList = null;
					Serializer.Deserialize(dataPool, offset, ref charDataList);
					int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					bool flag6 = charDataList != null;
					if (flag6)
					{
						string extraDesc = string.Empty;
						foreach (CharacterDisplayData data in charDataList)
						{
							bool isTaiwu = taiwuCharId == data.CharacterId;
							string name = NameCenter.GetMonasticTitleOrDisplayName(data, isTaiwu);
							extraDesc = extraDesc + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Char_Inventory_Overflow, name).ColorReplace();
						}
						CS$<>8__locals1.panelItem.extraDescLabel.text = extraDesc;
						CS$<>8__locals1.panelItem.extraDescLabel.gameObject.SetActive(true);
					}
				});
			}
		}

		// Token: 0x060056E3 RID: 22243 RVA: 0x00285C48 File Offset: 0x00283E48
		private void UpdateSectTask(WorldStateItem worldStateItem)
		{
			Action<Texture2D> <>9__1;
			StoryDomainMethod.AsyncCall.GetSectMainStoryTriggerConditions(this, (short)worldStateItem.Sect, delegate(int offset, RawDataPool dataPool)
			{
				int status = 0;
				Serializer.Deserialize(dataPool, offset, ref status);
				WorldStateItem config = WorldState.Instance[worldStateItem.TemplateId];
				sbyte sectTemplateId = config.Sect;
				string assetPath = "RemakeResources/Textures/RemakeTextures/ui9_tex_world_state_panel_" + ((int)(sectTemplateId - 1)).ToString();
				Action<Texture2D> onLoad;
				if ((onLoad = <>9__1) == null)
				{
					onLoad = (<>9__1 = delegate(Texture2D texture)
					{
						this.sectImage.texture = texture;
					});
				}
				ResLoader.Load<Texture2D>(assetPath, onLoad, null, false);
				this.sectStoryDesc.text = config.Desc;
				bool flag = config.SectStoryCondition == null;
				if (!flag)
				{
					this.UpdateStatus(config, status, false);
				}
			});
		}

		// Token: 0x060056E4 RID: 22244 RVA: 0x00285C88 File Offset: 0x00283E88
		private void UpdateStatus(WorldStateItem config, int status, bool paused)
		{
			CommonUtils.PrepareEnoughChildren(this.sectStoryConditionHolder, this.sectStoryConditionHolder.GetChild(0).gameObject, config.SectStoryCondition.Length, null);
			for (int i = 0; i < config.SectStoryCondition.Length; i++)
			{
				TextMeshProUGUI obj = this.sectStoryConditionHolder.GetChild(i).GetComponent<TextMeshProUGUI>();
				bool isEnable = (status & 1 << i) != 0;
				obj.text = config.SectStoryCondition[i].SetColor(isEnable ? "brightblue" : "6c6c6c");
			}
		}

		// Token: 0x060056E5 RID: 22245 RVA: 0x00285D20 File Offset: 0x00283F20
		private IEnumerator UpdateTaiwuWanted(Refers refers, SettlementBountyDisplayData data)
		{
			ViewWorldStatePanel.<>c__DisplayClass52_0 CS$<>8__locals1 = new ViewWorldStatePanel.<>c__DisplayClass52_0();
			CS$<>8__locals1.data = data;
			CS$<>8__locals1.taiWuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = !PoolManager.HasData("ViewWorldStatePanel_CharTemplate");
			if (flag)
			{
				PoolManager.SetSrcObject("ViewWorldStatePanel_CharTemplate", this.CharTemplate);
			}
			foreach (int charId in SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds())
			{
				NameRelatedData nameRelatedData = default(NameRelatedData);
				Refers instance = null;
				RectTransform view = null;
				Refers unit = null;
				bool flag2 = charId == CS$<>8__locals1.taiWuId;
				if (flag2)
				{
					int index = 0;
					IEnumerable<int> source = from key in CS$<>8__locals1.data.BountyCharacterDisplayDataDict.Keys
					where key < 0
					select key;
					Func<int, CharacterDisplayDataForSettlementBounty> selector;
					if ((selector = CS$<>8__locals1.<>9__2) == null)
					{
						selector = (CS$<>8__locals1.<>9__2 = ((int key) => CS$<>8__locals1.data.BountyCharacterDisplayDataDict[key]));
					}
					foreach (CharacterDisplayDataForSettlementBounty charData3 in from charData in source.Select(selector)
					where ((charData != null) ? charData.SettlementBounty : null) != null
					select charData)
					{
						nameRelatedData = charData3.NameRelatedData;
						bool flag3 = null == instance;
						if (flag3)
						{
							instance = PoolManager.GetObject<Refers>("ViewWorldStatePanel_CharTemplate");
							Transform instanceTransform = instance.gameObject.transform;
							instanceTransform.SetParent(refers.transform.parent, true);
							instanceTransform.localScale = Vector3.one;
							view = instance.CGet<RectTransform>("View");
							unit = instance.CGet<Refers>("Unit");
							instanceTransform = null;
						}
						Refers child = (index < view.childCount) ? view.GetChild(index).GetComponent<Refers>() : Object.Instantiate<GameObject>(unit.gameObject, view).GetComponent<Refers>();
						CS$<>8__locals1.<UpdateTaiwuWanted>g__RefreshBountyUnit|0(child, charData3, index);
						index++;
						child = null;
						charData3 = null;
					}
					IEnumerator<CharacterDisplayDataForSettlementBounty> enumerator2 = null;
					bool flag4 = null == view;
					if (flag4)
					{
						continue;
					}
					int num;
					for (int i = view.childCount - 1; i >= index; i = num - 1)
					{
						Transform child2 = view.GetChild(i);
						bool flag5 = child2.gameObject != unit.gameObject;
						if (flag5)
						{
							Object.Destroy(child2.gameObject);
						}
						child2 = null;
						num = i;
					}
				}
				else
				{
					CharacterDisplayDataForSettlementBounty charData2;
					bool flag6 = !CS$<>8__locals1.data.BountyCharacterDisplayDataDict.TryGetValue(charId, out charData2) || charData2.SettlementBounty == null;
					if (flag6)
					{
						continue;
					}
					nameRelatedData = charData2.NameRelatedData;
					instance = PoolManager.GetObject<Refers>("ViewWorldStatePanel_CharTemplate");
					Transform instanceTransform2 = instance.gameObject.transform;
					instanceTransform2.SetParent(refers.transform.parent, true);
					instanceTransform2.localScale = Vector3.one;
					view = instance.CGet<RectTransform>("View");
					unit = instance.CGet<Refers>("Unit");
					int num;
					for (int j = view.childCount - 1; j >= 0; j = num - 1)
					{
						Transform child3 = view.GetChild(j);
						bool flag7 = child3.gameObject != unit.gameObject;
						if (flag7)
						{
							Object.Destroy(child3.gameObject);
						}
						child3 = null;
						num = j;
					}
					CS$<>8__locals1.<UpdateTaiwuWanted>g__RefreshBountyUnit|0(unit, charData2, 0);
					charData2 = null;
					instanceTransform2 = null;
				}
				this._charInstances.Add(instance);
				view.localScale = Vector3.zero;
				TextMeshProUGUI label = instance.CGet<TextMeshProUGUI>("Name");
				label.text = "【" + NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, charId == CS$<>8__locals1.taiWuId, false) + "】";
				LayoutRebuilder.ForceRebuildLayoutImmediate(view);
				LayoutRebuilder.MarkLayoutForRebuild(view);
				yield return new WaitForEndOfFrame();
				RectTransform targetRectTransform = instance.GetComponent<RectTransform>();
				Vector2 offsetMax = targetRectTransform.offsetMax;
				Vector2 offsetMin = targetRectTransform.offsetMin;
				offsetMax.y = label.GetComponent<RectTransform>().offsetMax.y;
				offsetMin.y = view.GetComponent<RectTransform>().offsetMin.y;
				targetRectTransform.offsetMax = offsetMax;
				targetRectTransform.offsetMin = offsetMin;
				yield return new WaitForEndOfFrame();
				view.localScale = Vector3.one;
				nameRelatedData = default(NameRelatedData);
				instance = null;
				view = null;
				unit = null;
				label = null;
				targetRectTransform = null;
				offsetMax = default(Vector2);
				offsetMin = default(Vector2);
			}
			List<int>.Enumerator enumerator = default(List<int>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060056E6 RID: 22246 RVA: 0x00285D40 File Offset: 0x00283F40
		private void ClearInstances()
		{
			foreach (Refers instance in this._charInstances)
			{
				PoolManager.Destroy("ViewWorldStatePanel_CharTemplate", instance.gameObject);
			}
			this._charInstances.Clear();
		}

		// Token: 0x060056E7 RID: 22247 RVA: 0x00285DB0 File Offset: 0x00283FB0
		private void OnJumpButtonClicked(WorldStateItem worldStateItem, WorldStatePanelItem panelItem)
		{
			ViewWorldStatePanel.<>c__DisplayClass54_0 CS$<>8__locals1 = new ViewWorldStatePanel.<>c__DisplayClass54_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.worldStateItem = worldStateItem;
			this._dropdownButtonConfigs.Clear();
			sbyte templateId = CS$<>8__locals1.worldStateItem.TemplateId;
			sbyte b = templateId;
			ArgumentBox args;
			if (b >= 25)
			{
				if (b <= 39)
				{
					goto IL_372;
				}
				if (b <= 41)
				{
					if (b != 40)
					{
						if (b != 41)
						{
							goto IL_370;
						}
						UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", false));
						UIManager.Instance.MaskUI(UIElement.Legacy);
						goto IL_372;
					}
				}
				else if (b != 51)
				{
					if (b != 54)
					{
						goto IL_370;
					}
					this._dropdownButtonConfigs.Add(new ValueTuple<string, Action<CButton>, Action>(LocalStringManager.Get(LanguageKey.LK_Bottom_Reading), delegate(CButton button)
					{
						bool hasReadingTypes = CS$<>8__locals1.<>4__this._worldStateData.HasReadingTypes();
						button.interactable = hasReadingTypes;
						TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
						tip.enabled = !hasReadingTypes;
						bool flag2 = !hasReadingTypes;
						if (flag2)
						{
							TooltipInvoker tooltipInvoker = tip;
							if (tooltipInvoker.PresetParam == null)
							{
								tooltipInvoker.PresetParam = new string[1];
							}
							tip.PresetParam[0] = LanguageKey.LK_WorldState_PracticeNotice_UnenableNotice.Tr();
						}
					}, delegate()
					{
						CS$<>8__locals1.<>4__this.ExitFocusMode();
						ViewWorldState.ShowReadingView(CS$<>8__locals1.<>4__this._worldStateData);
						bool isShowing = UIElement.Reading.IsShowing;
						if (isShowing)
						{
							UIElement reading = UIElement.Reading;
							Delegate onHide = reading.OnHide;
							Action b2;
							if ((b2 = CS$<>8__locals1.<>9__17) == null)
							{
								b2 = (CS$<>8__locals1.<>9__17 = delegate()
								{
									CS$<>8__locals1.<>4__this.RefreshWorldState();
								});
							}
							reading.OnHide = (Action)Delegate.Combine(onHide, b2);
						}
						else
						{
							bool isShowing2 = UIElement.ReadingEvent.IsShowing;
							if (isShowing2)
							{
								UIElement readingEvent = UIElement.ReadingEvent;
								Delegate onHide2 = readingEvent.OnHide;
								Action b3;
								if ((b3 = CS$<>8__locals1.<>9__18) == null)
								{
									b3 = (CS$<>8__locals1.<>9__18 = delegate()
									{
										CS$<>8__locals1.<>4__this.RefreshWorldState();
									});
								}
								readingEvent.OnHide = (Action)Delegate.Combine(onHide2, b3);
							}
						}
					}));
					this._dropdownButtonConfigs.Add(new ValueTuple<string, Action<CButton>, Action>(LocalStringManager.Get(LanguageKey.LK_Bottom_Looping), delegate(CButton button)
					{
						bool hasLoopingTypes = CS$<>8__locals1.<>4__this._worldStateData.HasLoopingTypes();
						button.interactable = hasLoopingTypes;
						TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
						tip.enabled = !hasLoopingTypes;
						bool flag2 = !hasLoopingTypes;
						if (flag2)
						{
							TooltipInvoker tooltipInvoker = tip;
							if (tooltipInvoker.PresetParam == null)
							{
								tooltipInvoker.PresetParam = new string[1];
							}
							tip.PresetParam[0] = LanguageKey.LK_WorldState_PracticeNotice_UnenableNotice.Tr();
						}
					}, delegate()
					{
						ViewWorldState.PrepareLoopingView(CS$<>8__locals1.<>4__this._worldStateData);
						IAsyncMethodRequestHandler requestHandler = null;
						AsyncMethodCallbackDelegate callback;
						if ((callback = CS$<>8__locals1.<>9__19) == null)
						{
							callback = (CS$<>8__locals1.<>9__19 = delegate(int offset, RawDataPool dataPool)
							{
								LoopingViewDisplayData displayData = null;
								Serializer.Deserialize(dataPool, offset, ref displayData);
								CS$<>8__locals1.<>4__this.ExitFocusMode();
								UIElement.Looping.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("LoopingViewDisplayData", displayData));
								UIManager.Instance.ShowUI(UIElement.Looping, true);
								UIElement looping = UIElement.Looping;
								Delegate onHide = looping.OnHide;
								Action b2;
								if ((b2 = CS$<>8__locals1.<>9__20) == null)
								{
									b2 = (CS$<>8__locals1.<>9__20 = delegate()
									{
										CS$<>8__locals1.<>4__this.RefreshWorldState();
									});
								}
								looping.OnHide = (Action)Delegate.Combine(onHide, b2);
							});
						}
						TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(requestHandler, callback);
					}));
					goto IL_372;
				}
			}
			else
			{
				switch (b)
				{
				case 10:
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowCharacterMenuSubPage|0(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None, -1);
					goto IL_372;
				case 11:
					this._dropdownButtonConfigs.Add(new ValueTuple<string, Action<CButton>, Action>(LocalStringManager.Get(LanguageKey.LK_Inventory), null, delegate()
					{
						base.<OnJumpButtonClicked>g__ShowCharacterMenuSubPage|0(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None, -1);
					}));
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowWarehouse|1();
					goto IL_372;
				case 12:
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowWarehouse|1();
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowBuildingArea|2();
					goto IL_372;
				case 13:
				{
					ResourceInts targetResources = default(ResourceInts);
					targetResources.Initialize();
					BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
					BasicGameData baseGameData = SingletonObject.getInstance<BasicGameData>();
					for (sbyte i = 0; i < 6; i += 1)
					{
						int target = Math.Max(buildingModel.GetResourceCount(i) - baseGameData.MaterialResourceMaxCount, GlobalConfig.Instance.ChoosyResourceBaseCost);
						targetResources.Set((int)i, target);
					}
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowCharacterMenuSubPage|0(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None, -1);
					args = EasyPool.Get<ArgumentBox>();
					args.SetObject("targetResources", targetResources);
					UIElement.ChoosyResource.SetOnInitArgs(args);
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
					{
						UIManager.Instance.MaskUI(UIElement.ChoosyResource);
					});
					goto IL_372;
				}
				case 14:
				case 15:
				case 16:
				case 17:
					break;
				default:
					goto IL_370;
				}
			}
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			sbyte templateId2 = CS$<>8__locals1.worldStateItem.TemplateId;
			if (!true)
			{
			}
			int currentId2;
			if (templateId2 != 40)
			{
				if (templateId2 != 51)
				{
					currentId2 = taiwuCharId;
				}
				else
				{
					currentId2 = this._dyingCharIds[0];
				}
			}
			else
			{
				currentId2 = this._severelyInjuredCharIds[0];
			}
			if (!true)
			{
			}
			int currentId = currentId2;
			this._dropdownButtonConfigs.Add(new ValueTuple<string, Action<CButton>, Action>(LocalStringManager.Get(LanguageKey.LK_Heal_Entry), null, delegate()
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
				List<int> teamCharList = monitor.GetTaiwuTeamCharIds();
				teamCharList.AddRange(monitor.GetTaiwuSpecialGroup());
				args.SetObject("DoctorList", teamCharList);
				List<int> patientList = new List<int>();
				patientList.AddRange(teamCharList);
				args.SetObject("PatientList", patientList);
				args.Set("NeedPay", false);
				args.Set("CurrentCharacterId", currentId);
				CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, taiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					KidnappedCharacterList kidnappedCharacterList = null;
					Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
					bool flag2 = kidnappedCharacterList != null;
					if (flag2)
					{
						for (int j = 0; j < kidnappedCharacterList.GetCount(); j++)
						{
							patientList.Add(kidnappedCharacterList.Get(j).CharId);
						}
					}
					CS$<>8__locals1.<>4__this.ExitFocusMode();
					UIElement.Heal.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.Heal, true);
					UIElement heal = UIElement.Heal;
					Delegate onHide = heal.OnHide;
					Action b2;
					if ((b2 = CS$<>8__locals1.<>9__13) == null)
					{
						b2 = (CS$<>8__locals1.<>9__13 = delegate()
						{
							CS$<>8__locals1.<>4__this.RefreshWorldState();
						});
					}
					heal.OnHide = (Action)Delegate.Combine(onHide, b2);
				});
			}));
			Action <>9__16;
			this._dropdownButtonConfigs.Add(new ValueTuple<string, Action<CButton>, Action>(LocalStringManager.Get(LanguageKey.LK_UsingMedicine), delegate(CButton button)
			{
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				sbyte worldStateItemTmeplateId = CS$<>8__locals1.worldStateItem.TemplateId;
				Func<ItemDisplayData, bool> <>9__15;
				CharacterDomainMethod.AsyncCall.GetInventoryItemsByItemType(null, currentId, 8, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> medicineItemDisplayDatas = null;
					Serializer.Deserialize(dataPool, offset, ref medicineItemDisplayDatas);
					bool flag2;
					if (medicineItemDisplayDatas == null)
					{
						flag2 = false;
					}
					else
					{
						IEnumerable<ItemDisplayData> source = medicineItemDisplayDatas;
						Func<ItemDisplayData, bool> predicate;
						if ((predicate = <>9__15) == null)
						{
							predicate = (<>9__15 = ((ItemDisplayData x) => ViewWorldState.IsMedicineEffectTypeMatchWorldState(worldStateItemTmeplateId, Medicine.Instance[x.Key.TemplateId].EffectType)));
						}
						flag2 = (source.Count(predicate) > 0);
					}
					bool res = flag2;
					button.interactable = res;
					TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
					tip.enabled = !res;
					bool flag3 = !res;
					if (flag3)
					{
						TooltipInvoker tooltipInvoker = tip;
						if (tooltipInvoker.PresetParam == null)
						{
							tooltipInvoker.PresetParam = new string[1];
						}
						tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_WorldState_NoMedicine).SetColor("ec5f68");
					}
				});
			}, delegate()
			{
				CS$<>8__locals1.<OnJumpButtonClicked>g__ShowCharacterMenuSubPage|0(ECharacterSubToggleBase.None, ECharacterSubPage.Character, currentId);
				UIElement characterMenu = UIElement.CharacterMenu;
				Delegate onShowed = characterMenu.OnShowed;
				Action b2;
				if ((b2 = <>9__16) == null)
				{
					b2 = (<>9__16 = delegate()
					{
						UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().Injury.OnClickShowMedicineItem(UsingMedicineItemType.Invalid, currentId);
					});
				}
				characterMenu.OnShowed = (Action)Delegate.Combine(onShowed, b2);
			}));
			IL_370:
			IL_372:
			bool flag = this._dropdownButtonConfigs.Count > 0;
			if (flag)
			{
				this._focusPanelItem = panelItem;
				this.IsInFocusMode = true;
			}
		}

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x060056E8 RID: 22248 RVA: 0x00286157 File Offset: 0x00284357
		// (set) Token: 0x060056E9 RID: 22249 RVA: 0x00286160 File Offset: 0x00284360
		private bool IsInFocusMode
		{
			get
			{
				return this._isInfocusMode;
			}
			set
			{
				this._isInfocusMode = value;
				bool isInfocusMode = this._isInfocusMode;
				if (isInfocusMode)
				{
					this.EnterFocusMode();
				}
				else
				{
					this.ExitFocusMode();
				}
			}
		}

		// Token: 0x060056EA RID: 22250 RVA: 0x00286194 File Offset: 0x00284394
		private void SetDropdownButtons()
		{
			CommonUtils.PrepareEnoughChildren(this.dropdownButtonHolder, this.dropdownButtonHolder.GetChild(1).gameObject, this._dropdownButtonConfigs.Count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
			{
				TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems,
				ExtraItemCount = 1
			}));
			for (int i = 0; i < this._dropdownButtonConfigs.Count; i++)
			{
				ValueTuple<string, Action<CButton>, Action> valueTuple = this._dropdownButtonConfigs[i];
				string buttonName = valueTuple.Item1;
				Action<CButton> checkAction = valueTuple.Item2;
				Action clickAction = valueTuple.Item3;
				CButton button = this.dropdownButtonHolder.GetChild(i + 1).GetComponent<CButton>();
				button.interactable = true;
				button.GetComponent<TooltipInvoker>().enabled = false;
				button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonName;
				if (checkAction != null)
				{
					checkAction(button);
				}
				button.ClearAndAddListener(clickAction);
			}
			CButton jumpButton = this._focusPanelItem.jumpButton;
			RectTransform rect = this.dropdownButtonHolder.transform as RectTransform;
			rect.SetParent(jumpButton.transform);
			rect.anchorMin = new Vector2(rect.anchorMin.x, 0f);
			rect.anchorMax = new Vector2(rect.anchorMax.x, 0f);
			rect.pivot = new Vector2(rect.pivot.x, 1f);
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0f);
			rect.SetParent(this.dropdownMask.transform, true);
			this.dropdownMask.gameObject.SetActive(true);
			this.dropdownMask.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.IsInFocusMode = false;
			});
		}

		// Token: 0x060056EB RID: 22251 RVA: 0x0028636C File Offset: 0x0028456C
		private void EnterFocusMode()
		{
			WorldStatePanelItem item = this._focusPanelItem;
			item.selected.gameObject.SetActive(true);
			this.dropdownMask.gameObject.SetActive(true);
			UIManager.Instance.SetEscHandler(new Action(this.ExitFocusMode));
			this.SetDropdownButtons();
		}

		// Token: 0x060056EC RID: 22252 RVA: 0x002863C4 File Offset: 0x002845C4
		private void ExitFocusMode()
		{
			WorldStatePanelItem item = this._focusPanelItem;
			bool flag = item;
			if (flag)
			{
				item.selected.gameObject.SetActive(false);
			}
			this._focusPanelItem = null;
			UIManager.Instance.SetEscHandler(null);
			this.dropdownMask.gameObject.SetActive(false);
		}

		// Token: 0x060056ED RID: 22253 RVA: 0x0028641C File Offset: 0x0028461C
		private bool CheckInteractEnable(WorldStateItem worldStateItem)
		{
			sbyte templateId = worldStateItem.TemplateId;
			if (!true)
			{
			}
			bool result;
			if (templateId <= 40)
			{
				switch (templateId)
				{
				case 10:
					result = true;
					goto IL_8C;
				case 11:
					result = true;
					goto IL_8C;
				case 12:
					result = true;
					goto IL_8C;
				case 13:
					result = true;
					goto IL_8C;
				case 14:
					result = true;
					goto IL_8C;
				case 15:
					result = true;
					goto IL_8C;
				case 16:
					result = true;
					goto IL_8C;
				case 17:
					result = true;
					goto IL_8C;
				default:
					if (templateId == 40)
					{
						result = true;
						goto IL_8C;
					}
					break;
				}
			}
			else
			{
				if (templateId == 41)
				{
					result = true;
					goto IL_8C;
				}
				if (templateId == 51)
				{
					result = true;
					goto IL_8C;
				}
				if (templateId == 54)
				{
					result = true;
					goto IL_8C;
				}
			}
			result = false;
			IL_8C:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060056EE RID: 22254 RVA: 0x002864C0 File Offset: 0x002846C0
		private void UpdateTime(ArgumentBox argBox = null)
		{
			TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
			this.time.text = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
			{
				timeManager.GetYear(),
				(int)(timeManager.GetMonthInCurrYear() + 1),
				Month.Instance[timeManager.GetMonthInCurrYear()].Name + LocalStringManager.Get(LanguageKey.LK_Dot_Symbol),
				LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetCurrSeason()))
			});
			this.seasonBg.SetSprite(string.Format("ui9_back_world_state_panel_season_{0}", TimeKit.GetCurrSeason()), false, null);
		}

		// Token: 0x060056EF RID: 22255 RVA: 0x00286574 File Offset: 0x00284774
		private void UpdateXiangshuProcess(WorldStateItem worldStateItem, bool isTrue)
		{
			sbyte xiangshuProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
			sbyte xiangshuLevel = GameData.Domains.World.SharedMethods.GetXiangshuLevel(xiangshuProgress);
			float fillAmount = (float)(xiangshuLevel - 1) * (1f / (float)(this.levelIconHolder.childCount - 1));
			this.xiangshuLevelBar.fillAmount = fillAmount;
			for (int i = 0; i < this.levelIconHolder.childCount; i++)
			{
				CImage image = this.levelIconHolder.GetChild(i).GetComponent<CImage>();
				bool flag = (int)xiangshuLevel > i;
				if (flag)
				{
					image.SetSprite("ui9_icon_world_state_progress_1", false, null);
					TooltipInvoker mouseTip = this.levelTipsHolder.GetChild(i).GetComponent<TooltipInvoker>();
					mouseTip.NeedRefresh = true;
					mouseTip.PresetParam = new string[]
					{
						WorldState.Instance[this._invasionWorldStateTemplateIds[i + 1]].Name
					};
					mouseTip.enabled = true;
				}
				else
				{
					image.SetSprite("ui9_icon_world_state_progress_0", false, null);
				}
			}
		}

		// Token: 0x04003B46 RID: 15174
		[SerializeField]
		private TextMeshProUGUI time;

		// Token: 0x04003B47 RID: 15175
		[SerializeField]
		private CImage xiangshuLevelBar;

		// Token: 0x04003B48 RID: 15176
		[SerializeField]
		private RectTransform levelIconHolder;

		// Token: 0x04003B49 RID: 15177
		[SerializeField]
		private RectTransform levelTipsHolder;

		// Token: 0x04003B4A RID: 15178
		[SerializeField]
		private TextMeshProUGUI worldProgressLevel;

		// Token: 0x04003B4B RID: 15179
		[SerializeField]
		private TextMeshProUGUI approvingRate;

		// Token: 0x04003B4C RID: 15180
		[SerializeField]
		private TextMeshProUGUI merchantFavorability;

		// Token: 0x04003B4D RID: 15181
		[SerializeField]
		private TextMeshProUGUI xiangshuInfectLevel;

		// Token: 0x04003B4E RID: 15182
		[SerializeField]
		private TextMeshProUGUI teammateLevel;

		// Token: 0x04003B4F RID: 15183
		[SerializeField]
		private CButton dropdownMask;

		// Token: 0x04003B50 RID: 15184
		[SerializeField]
		private RectTransform dropdownButtonHolder;

		// Token: 0x04003B51 RID: 15185
		[SerializeField]
		private RectTransform scrollContent;

		// Token: 0x04003B52 RID: 15186
		[SerializeField]
		private WorldStatePanelItem panelItemTemplate;

		// Token: 0x04003B53 RID: 15187
		[SerializeField]
		private CImage seasonBg;

		// Token: 0x04003B54 RID: 15188
		[SerializeField]
		private GameObject emptyIcon;

		// Token: 0x04003B55 RID: 15189
		[SerializeField]
		private CRawImage sectImage;

		// Token: 0x04003B56 RID: 15190
		[SerializeField]
		private TextMeshProUGUI sectStoryDesc;

		// Token: 0x04003B57 RID: 15191
		[SerializeField]
		private RectTransform sectStoryConditionHolder;

		// Token: 0x04003B58 RID: 15192
		private WorldStateData _worldStateData;

		// Token: 0x04003B59 RID: 15193
		private readonly List<WorldStateItem> _worldStateItems = new List<WorldStateItem>();

		// Token: 0x04003B5A RID: 15194
		[TupleElementNames(new string[]
		{
			"buttonName",
			"checkAction",
			"clickAction"
		})]
		private readonly List<ValueTuple<string, Action<CButton>, Action>> _dropdownButtonConfigs = new List<ValueTuple<string, Action<CButton>, Action>>();

		// Token: 0x04003B5B RID: 15195
		private readonly Dictionary<short, ushort> _loongDebuff = new Dictionary<short, ushort>();

		// Token: 0x04003B5C RID: 15196
		private List<int> _severelyInjuredCharIds = new List<int>();

		// Token: 0x04003B5D RID: 15197
		private List<int> _dyingCharIds = new List<int>();

		// Token: 0x04003B5E RID: 15198
		private List<MartialArtTournamentPreparationInfo> _martialArtTournamentPreparationInfoList;

		// Token: 0x04003B5F RID: 15199
		private int _tournamentPreparationEndDate;

		// Token: 0x04003B60 RID: 15200
		private readonly Dictionary<short, SettlementNameRelatedData> _settlementNameRelatedDataCache = new Dictionary<short, SettlementNameRelatedData>();

		// Token: 0x04003B61 RID: 15201
		private List<IntPair> _overweightSanctionPercent;

		// Token: 0x04003B62 RID: 15202
		private int _moveTimeCostPercent;

		// Token: 0x04003B63 RID: 15203
		private const string BrightBlue = "8dc3c3";

		// Token: 0x04003B64 RID: 15204
		private const string BrightYellow = "cdb149";

		// Token: 0x04003B65 RID: 15205
		private const string Grey = "6c6c6c";

		// Token: 0x04003B66 RID: 15206
		private const string BrightRed = "ec5f68";

		// Token: 0x04003B67 RID: 15207
		private const string CharItemPrefabKey = "ViewWorldStatePanel_CharTemplate";

		// Token: 0x04003B68 RID: 15208
		public GameObject CharTemplate;

		// Token: 0x04003B69 RID: 15209
		private readonly List<Refers> _charInstances = new List<Refers>();

		// Token: 0x04003B6A RID: 15210
		private WorldStatePanelItem _focusPanelItem;

		// Token: 0x04003B6B RID: 15211
		private const int DropdownButtonHolderOffset = -546;

		// Token: 0x04003B6C RID: 15212
		private bool _isInfocusMode;

		// Token: 0x04003B6D RID: 15213
		public const string SeasonBgPath = "ui9_back_world_state_panel_season_{0}";

		// Token: 0x04003B6E RID: 15214
		private readonly List<sbyte> _invasionWorldStateTemplateIds = new List<sbyte>
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9
		};
	}
}
