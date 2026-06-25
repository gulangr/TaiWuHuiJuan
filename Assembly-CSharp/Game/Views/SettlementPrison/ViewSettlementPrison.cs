using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Prison;
using Game.Views.CharacterMenu.Kidnap;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Global;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.Enum;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SettlementPrison
{
	// Token: 0x0200078A RID: 1930
	public class ViewSettlementPrison : UIBase
	{
		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x06005D6E RID: 23918 RVA: 0x002AFC98 File Offset: 0x002ADE98
		public CharacterDisplayData GuardDisplayData
		{
			get
			{
				sbyte b = Math.Clamp(this._currPage, 0, 2);
				if (!true)
				{
				}
				CharacterDisplayData result;
				if (b != 0)
				{
					if (b != 1)
					{
						result = this._settlementPrisonDisplayData.GuardianCharacterDisplayDataHigh[0];
					}
					else
					{
						result = this._settlementPrisonDisplayData.GuardianCharacterDisplayDataMid[0];
					}
				}
				else
				{
					result = this._settlementPrisonDisplayData.GuardianCharacterDisplayDataLow[0];
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x06005D6F RID: 23919 RVA: 0x002AFCF7 File Offset: 0x002ADEF7
		private bool IsNormal
		{
			get
			{
				return this._currPage != TreasuryOrPrisonPage.Infected.ToSbyte();
			}
		}

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x06005D70 RID: 23920 RVA: 0x002AFD0F File Offset: 0x002ADF0F
		// (set) Token: 0x06005D71 RID: 23921 RVA: 0x002AFD18 File Offset: 0x002ADF18
		public sbyte CurrPage
		{
			get
			{
				return this._currPage;
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					sbyte tmp = this._currPage;
					this._currPage = -1;
					this.toggleGroupTab.Set((int)tmp, false);
					this._currPage = tmp;
					this.RefreshGuardian();
				}
				else
				{
					bool flag2 = value < 4;
					if (flag2)
					{
						this._currPage = -1;
						this.toggleGroupTab.Set((int)value, false);
						this._currPage = value;
						this.RefreshGuardian();
						this.RefreshPrisoners();
					}
					else
					{
						this.ForceQuickHide();
					}
				}
			}
		}

		// Token: 0x06005D72 RID: 23922 RVA: 0x002AFD9C File Offset: 0x002ADF9C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.EnterState = 0;
			this._currPage = 0;
			bool flag = argsBox == null;
			if (flag)
			{
				base.QuickHide();
			}
			else
			{
				bool flag2 = !argsBox.Get("SettlementId", out this._settlementId);
				if (flag2)
				{
					this._settlementId = -1;
				}
				argsBox.Get("IsBreaking", out this._isBreaking);
				this.NeedDataListenerId = true;
				UIElement element = this.Element;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
				bool isBreaking = this._isBreaking;
				if (isBreaking)
				{
					this._currPage = ViewSettlementPrison._breakJailTarget;
					this.CurrPage = (ViewSettlementPrison._breakJailTarget = -1);
					this._breakingConfirmed = false;
				}
				else
				{
					this.CurrPage = (ViewSettlementPrison._breakJailTarget = -1);
				}
				bool flag3 = !this._firstEnter;
				if (flag3)
				{
					this._firstEnter = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(122);
				}
			}
		}

		// Token: 0x06005D73 RID: 23923 RVA: 0x002AFE88 File Offset: 0x002AE088
		private void Awake()
		{
			this.InitSubPageToggles();
			this.InitSortAndFilter();
			this.InitModeDropdown();
			this.InitGridScroll();
			this.InitFocusMode();
			this.searchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchValueChanged));
			this.toggleGroupTab.Init(-1);
			this.toggleGroupTab.OnActiveIndexChange += this.ToggleGroupTabOnActiveIndexChange;
			this._toggleGroupAnim = this.toggleGroupTab.GetComponent<CommonSecondToggleContentRefreshAnim>();
			this._toggleGroupAnim.SetWaitCallParam(new List<RectTransform>
			{
				this.gridScroll.GetComponent<CScrollRect>().Content,
				this.listScroll.InfiniteScroll.GetComponent<CScrollRect>().Content
			}, null);
		}

		// Token: 0x06005D74 RID: 23924 RVA: 0x002AFF51 File Offset: 0x002AE151
		private void OnListenerIdReady()
		{
			base.RemoveMonitorFieldId(3, 14);
			base.AppendMonitorFieldId(new UIBase.MonitorDataField(3, 14, ulong.MaxValue, null));
		}

		// Token: 0x06005D75 RID: 23925 RVA: 0x002AFF70 File Offset: 0x002AE170
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
						bool flag = notification.DomainId == 3;
						if (flag)
						{
							bool flag2 = notification.MethodId == 20;
							if (flag2)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementPrisonDisplayData);
								this.Refresh();
							}
						}
					}
				}
				else
				{
					bool flag3 = notification.Uid.DomainId == 3;
					if (flag3)
					{
						bool flag4 = notification.Uid.DataId == 14;
						if (flag4)
						{
							OrganizationDomainMethod.Call.GetSettlementPrisonDisplayData(this.Element.GameDataListenerId, this._settlementId);
						}
					}
				}
			}
		}

		// Token: 0x06005D76 RID: 23926 RVA: 0x002B006C File Offset: 0x002AE26C
		private void Refresh()
		{
			this.RefreshTitle();
			this.RefreshTogAndTips();
			this.RefreshGuardian();
			this.RefreshPrisoners();
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06005D77 RID: 23927 RVA: 0x002B0098 File Offset: 0x002AE298
		private void RefreshTitle()
		{
			OrganizationItem orgConfig = Config.Organization.Instance[this._settlementPrisonDisplayData.OrgTemplateId];
			BuildingBlockItem blockConfig = BuildingBlock.Instance[orgConfig.PrisonBuilding];
			bool flag = blockConfig != null;
			if (flag)
			{
				this.textTitle.text = blockConfig.Name;
			}
			else
			{
				this.textTitle.text = LanguageKey.LK_SettlementPrison.Tr();
			}
		}

		// Token: 0x06005D78 RID: 23928 RVA: 0x002B0100 File Offset: 0x002AE300
		private void RefreshTogAndTips()
		{
			bool mid = FavorabilityType.GetFavorabilityType(this._settlementPrisonDisplayData.GuardianCharacterDisplayDataMid[0].FavorabilityToTaiwu) >= 4;
			bool mid2 = this._settlementPrisonDisplayData.DebtOrSupport >= GlobalConfig.Instance.PrisonRequireApprovingMid;
			bool high = FavorabilityType.GetFavorabilityType(this._settlementPrisonDisplayData.GuardianCharacterDisplayDataHigh[0].FavorabilityToTaiwu) >= 4;
			bool high2 = this._settlementPrisonDisplayData.DebtOrSupport >= GlobalConfig.Instance.PrisonRequireApprovingHigh;
			List<CToggle> all = this.toggleGroupTab.GetAll();
			for (int index = 0; index < all.Count; index++)
			{
				CToggle tog = all[index];
				TooltipInvoker displayer = tog.gameObject.GetComponentInChildren<TooltipInvoker>();
				bool flag = displayer == null;
				if (flag)
				{
					throw new Exception(string.Format("cannot find TooltipInvoker of {0}", index));
				}
				tog.interactable = !this._isBreaking;
				if (!true)
				{
				}
				ValueTuple<bool, bool, CharacterDisplayData> valueTuple;
				switch (index)
				{
				case 0:
					valueTuple = new ValueTuple<bool, bool, CharacterDisplayData>(true, true, null);
					break;
				case 1:
					valueTuple = new ValueTuple<bool, bool, CharacterDisplayData>(mid, mid2, this._settlementPrisonDisplayData.GuardianCharacterDisplayDataMid.FirstOrDefault<CharacterDisplayData>());
					break;
				case 2:
					valueTuple = new ValueTuple<bool, bool, CharacterDisplayData>(high, high2, this._settlementPrisonDisplayData.GuardianCharacterDisplayDataHigh.FirstOrDefault<CharacterDisplayData>());
					break;
				default:
					valueTuple = new ValueTuple<bool, bool, CharacterDisplayData>(true, true, null);
					break;
				}
				if (!true)
				{
				}
				ValueTuple<bool, bool, CharacterDisplayData> valueTuple2 = valueTuple;
				bool isFavor = valueTuple2.Item1;
				bool isDebtOrSupportEnough = valueTuple2.Item2;
				CharacterDisplayData guardianCharacterDisplayData = valueTuple2.Item3;
				displayer.Type = TipType.SettlementTreasuryOrPrisonLayer;
				TooltipInvoker tooltipInvoker = displayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				displayer.RuntimeParam.Set("layerIndex", index);
				displayer.RuntimeParam.Set("isFavor", isFavor);
				displayer.RuntimeParam.Set("isSect", true);
				displayer.RuntimeParam.Set("isPrison", true);
				displayer.RuntimeParam.Set("isDebtOrSupportEnough", isDebtOrSupportEnough);
				displayer.RuntimeParam.Set<CharacterDisplayData>("guardianCharacterDisplayData", guardianCharacterDisplayData);
				int prisonerCount = 0;
				bool flag2 = this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict != null;
				if (flag2)
				{
					bool flag3 = index != 3;
					if (flag3)
					{
						foreach (KeyValuePair<int, CharacterDisplayDataForSettlementPrisoner> keyValuePair in this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict)
						{
							int num;
							CharacterDisplayDataForSettlementPrisoner characterDisplayDataForSettlementPrisoner;
							keyValuePair.Deconstruct(out num, out characterDisplayDataForSettlementPrisoner);
							CharacterDisplayDataForSettlementPrisoner prisoner = characterDisplayDataForSettlementPrisoner;
							CharacterDisplayDataForSettlementPrisoner data = this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict[prisoner.SettlementPrisoner.CharId];
							bool completelyInfected = data.CompletelyInfected;
							if (!completelyInfected)
							{
								bool flag4 = data.PrisonType != (PrisonType)index;
								if (!flag4)
								{
									prisonerCount++;
								}
							}
						}
					}
					else
					{
						foreach (KeyValuePair<int, CharacterDisplayDataForSettlementPrisoner> keyValuePair in this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict)
						{
							int num;
							CharacterDisplayDataForSettlementPrisoner characterDisplayDataForSettlementPrisoner;
							keyValuePair.Deconstruct(out num, out characterDisplayDataForSettlementPrisoner);
							CharacterDisplayDataForSettlementPrisoner prisoner2 = characterDisplayDataForSettlementPrisoner;
							bool completelyInfected2 = this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict[prisoner2.SettlementPrisoner.CharId].CompletelyInfected;
							if (completelyInfected2)
							{
								prisonerCount++;
							}
						}
					}
				}
				displayer.RuntimeParam.Set("PrisonerCount", prisonerCount);
			}
		}

		// Token: 0x06005D79 RID: 23929 RVA: 0x002B049C File Offset: 0x002AE69C
		private void RefreshGuardian()
		{
			bool flag = this._settlementPrisonDisplayData == null;
			if (!flag)
			{
				CharacterDisplayData displayData = this.GuardDisplayData;
				bool showCharacter = displayData != null && !this._isBreaking;
				this.characterInfoRoot.gameObject.SetActive(showCharacter);
				bool flag2 = !showCharacter;
				if (!flag2)
				{
					this.avatar.Refresh(displayData, true);
					this.textCharName.text = NameCenter.GetMonasticTitleOrDisplayName(displayData, false);
					double text = GameData.Domains.World.SharedMethods.GetApproveTaiwuDisplayData((short)this._settlementPrisonDisplayData.DebtOrSupport);
					this.propertyItem.SetValue(string.Format("{0}%", text));
				}
			}
		}

		// Token: 0x06005D7A RID: 23930 RVA: 0x002B0540 File Offset: 0x002AE740
		private void RefreshPrisoners()
		{
			bool flag = this._settlementPrisonDisplayData == null;
			if (!flag)
			{
				this._dataList.Clear();
				bool flag2 = this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict != null;
				if (flag2)
				{
					bool isNormal = this.IsNormal;
					if (isNormal)
					{
						foreach (KeyValuePair<int, CharacterDisplayDataForSettlementPrisoner> keyValuePair in this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict)
						{
							int num;
							CharacterDisplayDataForSettlementPrisoner characterDisplayDataForSettlementPrisoner;
							keyValuePair.Deconstruct(out num, out characterDisplayDataForSettlementPrisoner);
							CharacterDisplayDataForSettlementPrisoner prisoner = characterDisplayDataForSettlementPrisoner;
							CharacterDisplayDataForSettlementPrisoner data3 = this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict[prisoner.SettlementPrisoner.CharId];
							bool completelyInfected = data3.CompletelyInfected;
							if (!completelyInfected)
							{
								bool flag3 = data3.PrisonType != (PrisonType)this._currPage;
								if (!flag3)
								{
									this._dataList.Add(prisoner);
								}
							}
						}
					}
					else
					{
						foreach (KeyValuePair<int, CharacterDisplayDataForSettlementPrisoner> keyValuePair in this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict)
						{
							int num;
							CharacterDisplayDataForSettlementPrisoner characterDisplayDataForSettlementPrisoner;
							keyValuePair.Deconstruct(out num, out characterDisplayDataForSettlementPrisoner);
							CharacterDisplayDataForSettlementPrisoner prisoner2 = characterDisplayDataForSettlementPrisoner;
							CharacterDisplayDataForSettlementPrisoner data2 = this._settlementPrisonDisplayData.PrisonerCharacterDisplayDataDict[prisoner2.SettlementPrisoner.CharId];
							bool completelyInfected2 = data2.CompletelyInfected;
							if (completelyInfected2)
							{
								this._dataList.Add(prisoner2);
							}
						}
					}
				}
				bool flag4 = !this.searchInputField.text.IsNullOrEmpty();
				if (flag4)
				{
					this._dataList.RemoveAll(delegate(CharacterDisplayDataForSettlementPrisoner data)
					{
						ITradeableContent tradeableContent = data.KidnapCharDisplayData;
						NameRelatedData nameRelatedData = tradeableContent.NameRelatedData;
						string charName = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, false, false);
						return !charName.Contains(this.searchInputField.text);
					});
				}
				this.RefreshListStructure();
				this.RefreshAll();
			}
		}

		// Token: 0x06005D7B RID: 23931 RVA: 0x002B0720 File Offset: 0x002AE920
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "Avatar_BigSize"))
			{
				if (!(a == "ButtonClosePopup"))
				{
					if (a == "ButtonRecord")
					{
						this.OnClickButtonRecord();
					}
				}
				else
				{
					this.QuickHide();
				}
			}
			else
			{
				CharacterDisplayData displayData = this.GuardDisplayData;
				bool showCharacter = displayData != null;
				bool flag = showCharacter;
				if (flag)
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("CharacterId", displayData.CharacterId);
					argBox.Set("PreviousView", 11);
					UIElement.CharacterMenu.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				}
			}
		}

		// Token: 0x06005D7C RID: 23932 RVA: 0x002B07D0 File Offset: 0x002AE9D0
		private void OnSearchValueChanged(string value)
		{
			string inputValue = this.searchInputField.text;
			CommonUtils.FixToShowAbleString(ref inputValue, this.searchInputField.textComponent.font);
			this.searchInputField.FixAndSetInputFieldText(ref value, this.searchInputField.textComponent.font);
			this.searchInputField.SetTextWithoutNotify(inputValue);
			this.RefreshPrisoners();
		}

		// Token: 0x06005D7D RID: 23933 RVA: 0x002B0834 File Offset: 0x002AEA34
		public static string GetOrgName(short orgTemplateId, short randomNameId)
		{
			bool flag = randomNameId >= 0;
			string name;
			if (flag)
			{
				name = LocalTownNames.Instance.TownNameCore[(int)randomNameId].Name;
			}
			else
			{
				name = Config.Organization.Instance[(int)orgTemplateId].Name;
			}
			return name;
		}

		// Token: 0x06005D7E RID: 23934 RVA: 0x002B0878 File Offset: 0x002AEA78
		public override void QuickHide()
		{
			bool flag = this._isBreaking && !this._breakingConfirmed;
			if (flag)
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = LanguageKey.LK_Building_LeavePrison.Tr();
				dialogCmd.Content = LanguageKey.LK_Building_LeavePrison_Desc.Tr();
				dialogCmd.Yes = new Action(this.ForceQuickHide);
				dialogCmd.No = delegate()
				{
				};
				dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				ViewSettlementPrison._breakJailTarget = (sbyte)this.toggleGroupTab.GetActiveIndex();
				base.QuickHide();
				TaiwuEventDomainMethod.Call.TriggerListener("SettlementPrisonComplete", true);
			}
		}

		// Token: 0x06005D7F RID: 23935 RVA: 0x002B0955 File Offset: 0x002AEB55
		private void ForceQuickHide()
		{
			this._breakingConfirmed = true;
			this.QuickHide();
		}

		// Token: 0x06005D80 RID: 23936 RVA: 0x002B0968 File Offset: 0x002AEB68
		private void InteractPrisoner(int charId, InteractPrisonerType type)
		{
			bool flag = !this._isBreaking;
			if (flag)
			{
				TaiwuEventDomainMethod.Call.InteractPrisoner(charId, type.ToInt());
			}
			else
			{
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SettlementPrisonComplete", "SelectCharacterId", charId);
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SettlementPrisonComplete", "InteractPrisonerType", type.ToInt());
				this.ForceQuickHide();
			}
		}

		// Token: 0x06005D81 RID: 23937 RVA: 0x002B09CC File Offset: 0x002AEBCC
		private void OnClickButtonRecord()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SettlementId", this._settlementId);
			UIElement.SettlementPrisonRecords.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.SettlementPrisonRecords);
		}

		// Token: 0x06005D82 RID: 23938 RVA: 0x002B0A0C File Offset: 0x002AEC0C
		public static string GetPrisonDurationDisplayStr(CharacterDisplayDataForSettlementPrisoner data)
		{
			string infinity = LocalStringManager.Get(LanguageKey.LK_Infinity);
			int curData = SingletonObject.getInstance<BasicGameData>().CurrDate;
			int maxTime = data.SettlementPrisoner.Duration;
			int remainTime = maxTime - (curData - data.SettlementPrisoner.KidnapBeginDate);
			PunishmentTypeItem punishmentTypeItem = PunishmentType.Instance[data.SettlementPrisoner.PunishmentType];
			return (punishmentTypeItem.TemplateId == 40) ? (infinity + "/" + infinity) : string.Format("{0}/{1}", remainTime, maxTime);
		}

		// Token: 0x06005D83 RID: 23939 RVA: 0x002B0A9C File Offset: 0x002AEC9C
		private void InitSubPageToggles()
		{
			this.subPageToggleGroup.Init(-1);
			this.subPageToggleGroup.OnActiveIndexChange += delegate(int newIndex, int _)
			{
				this.OnSubPageChanged((ViewSettlementPrison.PrisonSubPage)newIndex);
			};
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				CToggle toggle = toggles[i];
				TextMeshProUGUI label = toggle.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				label.text = ViewSettlementPrison.ToggleGroupNameKeys[i].Tr();
			}
		}

		// Token: 0x06005D84 RID: 23940 RVA: 0x002B0B2C File Offset: 0x002AED2C
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new PrisonSortAndFilterController(this.sortAndFilter);
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), "KidnapSort");
				bool flag2 = this.listScroll != null;
				if (flag2)
				{
					this.listScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<ViewSettlementPrison.PrisonSubPage, CharacterDisplayDataForSettlementPrisoner>(this._sortAndFilterController);
			}
		}

		// Token: 0x06005D85 RID: 23941 RVA: 0x002B0BB0 File Offset: 0x002AEDB0
		private void InitModeDropdown()
		{
			this.switchToggleGroup.Init(1);
			this.switchToggleGroup.SetWithoutNotify(1);
			this.switchToggleGroup.OnActiveIndexChange += this.OnChangeMode;
			this._isGridMode = true;
			this.UpdateModeDisplay();
		}

		// Token: 0x06005D86 RID: 23942 RVA: 0x002B0C00 File Offset: 0x002AEE00
		private void InitGridScroll()
		{
			bool flag = this.gridScroll == null;
			if (!flag)
			{
				this.gridScroll.OnItemRender += this.OnRenderGridItem;
			}
		}

		// Token: 0x06005D87 RID: 23943 RVA: 0x002B0C38 File Offset: 0x002AEE38
		private void InitFocusMode()
		{
			this.focusMask.ClearAndAddListener(new Action(this.OnClickMask));
			this.btnPlead.ClearAndAddListener(delegate
			{
				this.OnClickButton(InteractPrisonerType.Plead);
			});
			this.btnKidnap.ClearAndAddListener(delegate
			{
				this.OnClickButton(InteractPrisonerType.Kidnap);
			});
			this.btnRescue.ClearAndAddListener(delegate
			{
				this.OnClickButton(InteractPrisonerType.Rescue);
			});
			this.btnKidnap.GetComponent<TooltipInvoker>().PresetParam = new string[]
			{
				LanguageKey.LK_SettlementPrison_Tip_StoneRoomFull.Tr().SetColor("brightred")
			};
		}

		// Token: 0x06005D88 RID: 23944 RVA: 0x002B0CD4 File Offset: 0x002AEED4
		private void RefreshAll()
		{
			PrisonSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			if (sortAndFilterController != null)
			{
				sortAndFilterController.NotifyDataChanged(this._dataList);
			}
			PrisonSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Func<CharacterDisplayDataForSettlementPrisoner, bool> func;
			if ((func = ((sortAndFilterController2 != null) ? sortAndFilterController2.GenerateFilter() : null)) == null && (func = ViewSettlementPrison.<>c.<>9__75_0) == null)
			{
				func = (ViewSettlementPrison.<>c.<>9__75_0 = ((CharacterDisplayDataForSettlementPrisoner _) => true));
			}
			Func<CharacterDisplayDataForSettlementPrisoner, bool> filter = func;
			PrisonSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			Comparison<CharacterDisplayDataForSettlementPrisoner> comparer = (sortAndFilterController3 != null) ? sortAndFilterController3.GenerateComparer(this._dataList) : null;
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<ViewSettlementPrison.PrisonSubPage, CharacterDisplayDataForSettlementPrisoner> tabSortStateManager = this._tabSortStateManager;
				flag = (tabSortStateManager != null && tabSortStateManager.ShouldSort());
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._dataList.Sort(comparer);
			}
			this._filteredDataList = this._dataList.Where(filter).ToList<CharacterDisplayDataForSettlementPrisoner>();
			bool isGridMode = this._isGridMode;
			if (isGridMode)
			{
				this.RefreshGridMode();
			}
			else
			{
				this.RefreshListMode();
			}
			PrisonSortAndFilterController sortAndFilterController4 = this._sortAndFilterController;
			if (sortAndFilterController4 != null)
			{
				sortAndFilterController4.AfterFilter(this._dataList);
			}
		}

		// Token: 0x06005D89 RID: 23945 RVA: 0x002B0DC4 File Offset: 0x002AEFC4
		private void RefreshListMode()
		{
			this.RefreshListData();
			bool needPlayToggleGroupAnim = this._needPlayToggleGroupAnim;
			if (needPlayToggleGroupAnim)
			{
				this._toggleGroupAnim.CallAnim();
				this._needPlayToggleGroupAnim = false;
			}
		}

		// Token: 0x06005D8A RID: 23946 RVA: 0x002B0DF8 File Offset: 0x002AEFF8
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this._currentSubPage);
			this.PrepareRowTemplateContainers(this._currentSubPage);
			this.listScroll.ClearInfinityScrollCache();
			this.listScroll.Init<KidnapCharDisplayData>(columnDefinitions, true, null, new Action<int, RowItem>(this.OnClickRow));
		}

		// Token: 0x06005D8B RID: 23947 RVA: 0x002B0E47 File Offset: 0x002AF047
		private void RefreshListData()
		{
			this.listScroll.SetData<CharacterDisplayDataForSettlementPrisoner>(this._filteredDataList, -1);
		}

		// Token: 0x06005D8C RID: 23948 RVA: 0x002B0E60 File Offset: 0x002AF060
		private void RefreshGridMode()
		{
			bool flag = this.gridScroll == null;
			if (!flag)
			{
				this.gridScroll.UpdateData(this._filteredDataList.Count);
				bool needPlayToggleGroupAnim = this._needPlayToggleGroupAnim;
				if (needPlayToggleGroupAnim)
				{
					this._toggleGroupAnim.CallAnim();
					this._needPlayToggleGroupAnim = false;
				}
			}
		}

		// Token: 0x06005D8D RID: 23949 RVA: 0x002B0EB8 File Offset: 0x002AF0B8
		private void OnRenderGridItem(int index, GameObject obj)
		{
			bool flag = index < 0 || index >= this._filteredDataList.Count;
			if (!flag)
			{
				SettlementPrisonerCardItem settlementPrisoner = obj.GetComponent<SettlementPrisonerCardItem>();
				CharacterDisplayDataForSettlementPrisoner data = this._filteredDataList[index];
				settlementPrisoner.Set(data, this._settlementPrisonDisplayData.IsStoneRoomFull, this._isBreaking, new Action(this.ForceQuickHide));
			}
		}

		// Token: 0x06005D8E RID: 23950 RVA: 0x002B0F20 File Offset: 0x002AF120
		private void UpdateModeDisplay()
		{
			this.subPageToggleGroup.gameObject.SetActive(!this._isGridMode);
			this.listModeRoot.SetActive(!this._isGridMode);
			this.gridModeRoot.SetActive(this._isGridMode);
		}

		// Token: 0x06005D8F RID: 23951 RVA: 0x002B0F70 File Offset: 0x002AF170
		private void OnChangeMode(int currIndex, int prevIndex)
		{
			this._isGridMode = (currIndex == 1);
			this.UpdateModeDisplay();
			this.RefreshListStructure();
			this.RefreshAll();
			bool flag = this._isGridMode && this.subPageToggleGroup.GetActiveIndex() != 0;
			if (flag)
			{
				this.subPageToggleGroup.Set(0, false);
			}
		}

		// Token: 0x06005D90 RID: 23952 RVA: 0x002B0FC8 File Offset: 0x002AF1C8
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ViewSettlementPrison.PrisonSubPage subPage)
		{
			yield return this.CreateAvatarWithNameColumn();
			switch (subPage)
			{
			case ViewSettlementPrison.PrisonSubPage.Prison:
			{
				foreach (ColumnDefinition col in this.GeneratePrisonColumns())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.State:
			{
				foreach (ColumnDefinition col2 in this.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.Property:
			{
				foreach (ColumnDefinition col3 in this.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in this.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in this.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in this.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in this.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.Item:
			{
				foreach (ColumnDefinition col8 in this.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case ViewSettlementPrison.PrisonSubPage.Command:
			{
				foreach (ColumnDefinition col9 in this.GenerateCommandColumns())
				{
					yield return col9;
					col9 = null;
				}
				IEnumerator<ColumnDefinition> enumerator9 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x06005D91 RID: 23953 RVA: 0x002B0FDF File Offset: 0x002AF1DF
		private IEnumerable<RowCellContainer> GetCellContainerTemplates(ViewSettlementPrison.PrisonSubPage subPage)
		{
			yield return this.avatarAndNameCellContainer;
			if (subPage != ViewSettlementPrison.PrisonSubPage.Prison)
			{
				if (subPage != ViewSettlementPrison.PrisonSubPage.Command)
				{
					int columnCount = this.GetColumnCount(subPage);
					int num;
					for (int i = 0; i < columnCount; i = num + 1)
					{
						yield return this.singleTextCellContainer;
						num = i;
					}
				}
				else
				{
					int num;
					for (int j = 0; j < 6; j = num + 1)
					{
						yield return this.iconAndTextCellContainer;
						num = j;
					}
				}
			}
			else
			{
				yield return this.singleTextCellContainer;
				yield return this.iconAndTextCellContainer;
				yield return this.textWithTipCellContainer;
				yield return this.ropeCellContainer;
				yield return this.singleTextCellContainer;
				yield return this.singleTextCellContainer;
			}
			yield break;
		}

		// Token: 0x06005D92 RID: 23954 RVA: 0x002B0FF8 File Offset: 0x002AF1F8
		private int GetColumnCount(ViewSettlementPrison.PrisonSubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case ViewSettlementPrison.PrisonSubPage.Prison:
				result = 4;
				break;
			case ViewSettlementPrison.PrisonSubPage.State:
				result = 10;
				break;
			case ViewSettlementPrison.PrisonSubPage.Property:
				result = 10;
				break;
			case ViewSettlementPrison.PrisonSubPage.Property2:
				result = 9;
				break;
			case ViewSettlementPrison.PrisonSubPage.LifeSkill:
				result = 17;
				break;
			case ViewSettlementPrison.PrisonSubPage.CombatSkill:
				result = 15;
				break;
			case ViewSettlementPrison.PrisonSubPage.Personality:
				result = 7;
				break;
			case ViewSettlementPrison.PrisonSubPage.Item:
				result = 10;
				break;
			case ViewSettlementPrison.PrisonSubPage.Command:
				result = 6;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06005D93 RID: 23955 RVA: 0x002B1070 File Offset: 0x002AF270
		private void PrepareRowTemplateContainers(ViewSettlementPrison.PrisonSubPage subPage)
		{
			Transform containerRoot = this.rowTemplate.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			foreach (RowCellContainer containerTemplate in this.GetCellContainerTemplates(subPage))
			{
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
			this.rowTemplate.ResetSibling();
		}

		// Token: 0x06005D94 RID: 23956 RVA: 0x002B1138 File Offset: 0x002AF338
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((CharacterDisplayDataForSettlementPrisoner data) => AvatarWithNameCellData.FromKidnapCharDisplayData(data.KidnapCharDisplayData, false, null, null));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x002B11DC File Offset: 0x002AF3DC
		private ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<CharacterDisplayDataForSettlementPrisoner, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x002B1240 File Offset: 0x002AF440
		private ColumnDefinition CreateTextWithTipColumn(Func<string> headerKey, Func<CharacterDisplayDataForSettlementPrisoner, string> valueGetter, Func<CharacterDisplayDataForSettlementPrisoner, Action<TooltipInvoker>> tipRefresherGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, TextWithTipCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((CharacterDisplayDataForSettlementPrisoner data) => new TextWithTipCellData
				{
					Text = valueGetter(data),
					TipRefresher = tipRefresherGetter(data)
				}),
				SortId = sortId
			};
		}

		// Token: 0x06005D97 RID: 23959 RVA: 0x002B12C4 File Offset: 0x002AF4C4
		private ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<CharacterDisplayDataForSettlementPrisoner, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, IconAndTextCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x06005D98 RID: 23960 RVA: 0x002B1328 File Offset: 0x002AF528
		private ColumnDefinition CreateRopeColumn()
		{
			ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, RopeCellData> columnDefinition = new ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, RopeCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 120f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_ItemSubType_1206.Tr());
			columnDefinition.CellDataGenerator = delegate(CharacterDisplayDataForSettlementPrisoner data)
			{
				data.KidnapCharDisplayData.RopeItemKey = data.SettlementPrisoner.RopeItemKey;
				return new RopeCellData
				{
					KidnapCharDisplayData = data.KidnapCharDisplayData
				};
			};
			columnDefinition.SortId = 1;
			return columnDefinition;
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x002B13CC File Offset: 0x002AF5CC
		private ColumnDefinition CreateInteractButtonColumn()
		{
			ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, MultiplyButtonCellData> columnDefinition = new ColumnDefinition<CharacterDisplayDataForSettlementPrisoner, MultiplyButtonCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 120f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SettlementPrison_Interact.Tr());
			columnDefinition.CellDataGenerator = delegate(CharacterDisplayDataForSettlementPrisoner data)
			{
				MultiplyButtonCellData cellData = new MultiplyButtonCellData
				{
					SingleButtonCellDataList = new List<SingleButtonCellData>()
				};
				bool flag = !this._isBreaking && !data.CompletelyInfected;
				if (flag)
				{
					cellData.SingleButtonCellDataList.Add(new SingleButtonCellData
					{
						LabelText = LanguageKey.LK_SettlementPrison_ButtonPlead.Tr(),
						OnClick = delegate()
						{
							this.InteractPrisoner(data.KidnapCharDisplayData.CharacterId, InteractPrisonerType.Plead);
						}
					});
				}
				bool interactable = !data.CompletelyInfected || (data.CompletelyInfected && !this._settlementPrisonDisplayData.IsStoneRoomFull);
				bool showTip = data.CompletelyInfected && this._settlementPrisonDisplayData.IsStoneRoomFull;
				cellData.SingleButtonCellDataList.Add(new SingleButtonCellData
				{
					LabelText = LanguageKey.LK_SettlementPrison_ButtonKidnap.Tr(),
					OnClick = delegate()
					{
						this.InteractPrisoner(data.KidnapCharDisplayData.CharacterId, InteractPrisonerType.Kidnap);
					},
					SingleButtonCellStatus = (interactable ? SingleButtonCellStatus.EnableInteractable : SingleButtonCellStatus.DisableInteractable),
					MouseTipText = (showTip ? LanguageKey.LK_SettlementPrison_Tip_StoneRoomFull.Tr().SetColor("brightred") : string.Empty)
				});
				bool flag2 = !data.CompletelyInfected;
				if (flag2)
				{
					cellData.SingleButtonCellDataList.Add(new SingleButtonCellData
					{
						LabelText = LanguageKey.LK_SettlementPrison_ButtonRescue.Tr(),
						OnClick = delegate()
						{
							this.InteractPrisoner(data.KidnapCharDisplayData.CharacterId, InteractPrisonerType.Rescue);
						}
					});
				}
				return cellData;
			};
			return columnDefinition;
		}

		// Token: 0x06005D9A RID: 23962 RVA: 0x002B1455 File Offset: 0x002AF655
		private IEnumerable<ColumnDefinition> GeneratePrisonColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_PunishmentType.Tr(), delegate(CharacterDisplayDataForSettlementPrisoner data)
			{
				PunishmentTypeItem punishmentTypeItem = PunishmentType.Instance[data.SettlementPrisoner.PunishmentType];
				PunishmentSeverityItem punishmentSeverityItem = PunishmentSeverity.Instance[data.SettlementPrisoner.PunishmentSeverity];
				return punishmentTypeItem.ShortName.SetColor(punishmentSeverityItem.NameColor);
			}, 138, 30f, 90f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_PrisonTime.Tr(), delegate(CharacterDisplayDataForSettlementPrisoner data)
			{
				string str = ViewSettlementPrison.GetPrisonDurationDisplayStr(data);
				return new IconAndTextCellData("ui9_icon_month", str, true, false, false, false);
			}, 14, 80f, 120f);
			yield return this.CreateTextWithTipColumn(() => LanguageKey.LK_Kidnap_Resistance_Value.Tr(), delegate(CharacterDisplayDataForSettlementPrisoner data)
			{
				string resistanceColor = (data.Resistance > 100) ? "brightred" : "brightblue";
				return data.Resistance.ToString().SetColor(resistanceColor);
			}, (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateResistanceTipRefresher(data), 120, 30f, 90f);
			yield return this.CreateRopeColumn();
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => CommonUtils.GetIdentityString(data.KidnapCharDisplayData.OrganizationInfo, data.KidnapCharDisplayData.Gender, data.KidnapCharDisplayData.PhysiologicalAge, false), -1, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.GetOrgName((short)data.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId, data.RandomNameId), -1, 30f, 90f);
			yield break;
		}

		// Token: 0x06005D9B RID: 23963 RVA: 0x002B1465 File Offset: 0x002AF665
		private IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.PhysiologicalAge.ToString(), 8, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => CommonUtils.GetCharacterHealthInfo(data.KidnapCharDisplayData.Health, data.KidnapCharDisplayData.MaxLeftHealth, data.KidnapCharDisplayData.CharacterId).Item1, 10, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), new Func<CharacterDisplayDataForSettlementPrisoner, string>(ViewSettlementPrison.GetCharmDisplayString), 9, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => CommonUtils.GetBehaviorString(data.KidnapCharDisplayData.BehaviorType), 57, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.KidnapCharDisplayData.Happiness)), 12, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<CharacterDisplayDataForSettlementPrisoner, string>(ViewSettlementPrison.GetFavorDisplayString), 11, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => CommonUtils.GetAlertnessNameByValue(data.KidnapCharDisplayData.Alertness), 130, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.PreexistenceCharCount.ToString(), 58, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => CommonUtils.GetFameString(FameType.GetFameType(data.KidnapCharDisplayData.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06005D9C RID: 23964 RVA: 0x002B1475 File Offset: 0x002AF675
		private IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Penetrations.Outer.ToString(), 22, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Penetrations.Inner.ToString(), 23, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.PenetrationResists.Outer.ToString(), 29, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.PenetrationResists.Inner.ToString(), 30, 30f, 90f);
			yield break;
		}

		// Token: 0x06005D9D RID: 23965 RVA: 0x002B1485 File Offset: 0x002AF685
		private IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.HitValues[0].ToString(), 24, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.HitValues[1].ToString(), 25, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.HitValues[2].ToString(), 26, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.HitValues[3].ToString(), 27, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ((int)(data.KidnapCharDisplayData.DisorderOfQi / 10)).ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06005D9E RID: 23966 RVA: 0x002B1495 File Offset: 0x002AF695
		private IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewSettlementPrison.<>c__DisplayClass97_0 CS$<>8__locals1 = new ViewSettlementPrison.<>c__DisplayClass97_0();
				CS$<>8__locals1.index = i;
				yield return this.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return this.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.GetSkillGrowthString(data.KidnapCharDisplayData.ActualAge, data.KidnapCharDisplayData.LifeSkillGrowthType), 118, 30f, 90f);
			yield break;
		}

		// Token: 0x06005D9F RID: 23967 RVA: 0x002B14A5 File Offset: 0x002AF6A5
		private IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewSettlementPrison.<>c__DisplayClass98_0 CS$<>8__locals1 = new ViewSettlementPrison.<>c__DisplayClass98_0();
				CS$<>8__locals1.index = i;
				yield return this.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return this.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.GetSkillGrowthString(data.KidnapCharDisplayData.ActualAge, data.KidnapCharDisplayData.CombatSkillGrowthType), 119, 30f, 90f);
			yield break;
		}

		// Token: 0x06005DA0 RID: 23968 RVA: 0x002B14B5 File Offset: 0x002AF6B5
		private IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[0].ToString(), 96, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[1].ToString(), 97, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[2].ToString(), 98, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[3].ToString(), 99, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[4].ToString(), 100, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[5].ToString(), 101, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x06005DA1 RID: 23969 RVA: 0x002B14C5 File Offset: 0x002AF6C5
		private IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[0].ToString(), 103, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[1].ToString(), 104, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[2].ToString(), 105, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[3].ToString(), 106, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[4].ToString(), 107, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[5].ToString(), 108, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[6].ToString(), 109, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.Resources[7].ToString(), 110, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.GetInventoryLoadString(data.KidnapCharDisplayData.CurrInventoryLoad, data.KidnapCharDisplayData.MaxInventoryLoad), 37, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Kidnap.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => data.KidnapCharDisplayData.KidnapCount.ToString(), 111, 30f, 90f);
			yield break;
		}

		// Token: 0x06005DA2 RID: 23970 RVA: 0x002B14D5 File Offset: 0x002AF6D5
		private IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateMedalCellData(data.KidnapCharDisplayData.AttackMedal, 0), 112, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateMedalCellData(data.KidnapCharDisplayData.DefenceMedal, 1), 113, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateMedalCellData(data.KidnapCharDisplayData.WisdomMedal, 2), 114, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateCommandCellData(data, 0), 115, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateCommandCellData(data, 1), 116, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (CharacterDisplayDataForSettlementPrisoner data) => ViewSettlementPrison.CreateCommandCellData(data, 2), 117, 80f, 120f);
			yield break;
		}

		// Token: 0x06005DA3 RID: 23971 RVA: 0x002B14E8 File Offset: 0x002AF6E8
		private static IconAndTextCellData CreateMedalCellData(int medalCount, int medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewSettlementPrison.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06005DA4 RID: 23972 RVA: 0x002B1538 File Offset: 0x002AF738
		private static string GetMedalIconName(int medalCount, int medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case 0:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case 1:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case 2:
				text = MedalSummary.WisdomMedalIconConfig[signKey];
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string iconNumber = text;
			return "ui9_icon_strategy_big_" + iconNumber;
		}

		// Token: 0x06005DA5 RID: 23973 RVA: 0x002B15B8 File Offset: 0x002AF7B8
		private static IconAndTextCellData CreateCommandCellData(CharacterDisplayDataForSettlementPrisoner data, int commandIndex)
		{
			bool flag = data.KidnapCharDisplayData.Command.Items == null || !data.KidnapCharDisplayData.Command.Items.CheckIndex(commandIndex);
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				sbyte commandId = data.KidnapCharDisplayData.Command.Items[commandIndex];
				bool flag2 = commandId < 0;
				if (flag2)
				{
					result = IconAndTextCellData.TextOnly("-");
				}
				else
				{
					TeammateCommandItem cmdConfig = Config.TeammateCommand.Instance[commandId];
					result = IconAndTextCellData.TextOnly(cmdConfig.Name);
				}
			}
			return result;
		}

		// Token: 0x06005DA6 RID: 23974 RVA: 0x002B1650 File Offset: 0x002AF850
		private static string GetCharmDisplayString(CharacterDisplayDataForSettlementPrisoner prisoner)
		{
			KidnapCharDisplayData data = prisoner.KidnapCharDisplayData;
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06005DA7 RID: 23975 RVA: 0x002B1698 File Offset: 0x002AF898
		private static string GetFavorDisplayString(CharacterDisplayDataForSettlementPrisoner prisoner)
		{
			KidnapCharDisplayData data = prisoner.KidnapCharDisplayData;
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005DA8 RID: 23976 RVA: 0x002B16C4 File Offset: 0x002AF8C4
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = ViewSettlementPrison.GetSkillGrowthAddValue(actualAge, (int)growthType);
			string growthName = (growthType == 0) ? LocalStringManager.Get("LK_Qualification_Growth_Average") : ((growthType == 1) ? LocalStringManager.Get("LK_Qualification_Growth_Precocious") : LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
			bool flag = addValue > 0;
			string addValueStr;
			if (flag)
			{
				addValueStr = string.Format("+{0}", addValue).SetColor("lightblue");
			}
			else
			{
				bool flag2 = addValue == 0;
				if (flag2)
				{
					addValueStr = "+0".SetColor("lightgrey");
				}
				else
				{
					addValueStr = string.Format("{0}", addValue).SetColor("red");
				}
			}
			return growthName + addValueStr;
		}

		// Token: 0x06005DA9 RID: 23977 RVA: 0x002B1770 File Offset: 0x002AF970
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x06005DAA RID: 23978 RVA: 0x002B17BC File Offset: 0x002AF9BC
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x06005DAB RID: 23979 RVA: 0x002B1808 File Offset: 0x002AFA08
		private static Action<TooltipInvoker> CreateResistanceTipRefresher(CharacterDisplayDataForSettlementPrisoner prisoner)
		{
			KidnapCharDisplayData data = prisoner.KidnapCharDisplayData;
			return delegate(TooltipInvoker tip)
			{
				bool flag = tip == null;
				if (!flag)
				{
					tip.Type = TipType.PrisonerResistance;
					if (tip.RuntimeParam == null)
					{
						tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tip.RuntimeParam.Set("IsPrivate", true).Set("Resistance", data.TotalResistance).Set("EscapeRate", data.EscapeRate).Set("RopeEffect", data.RopeEffect).Set("CompletelyInfected", data.CompletelyInfected).Set("OwningBook", data.OwningBook);
				}
			};
		}

		// Token: 0x06005DAC RID: 23980 RVA: 0x002B1838 File Offset: 0x002AFA38
		private void OnSubPageChanged(ViewSettlementPrison.PrisonSubPage subPage)
		{
			bool flag = this._currentSubPage == subPage;
			if (!flag)
			{
				this._currentSubPage = subPage;
				TabSortStateManager<ViewSettlementPrison.PrisonSubPage, CharacterDisplayDataForSettlementPrisoner> tabSortStateManager = this._tabSortStateManager;
				if (tabSortStateManager != null)
				{
					tabSortStateManager.OnTabChange(subPage);
				}
				this.RefreshListStructure();
				this.RefreshAll();
			}
		}

		// Token: 0x06005DAD RID: 23981 RVA: 0x002B187D File Offset: 0x002AFA7D
		private void OnSortOrFilterChanged()
		{
			this.RefreshAll();
		}

		// Token: 0x06005DAE RID: 23982 RVA: 0x002B1888 File Offset: 0x002AFA88
		private void OnClickRow(int index, RowItem row)
		{
			CharacterDisplayDataForSettlementPrisoner data = this._filteredDataList[index];
			this._currRowIndex = index;
			this.btnPlead.gameObject.SetActive(!this._isBreaking && !data.CompletelyInfected);
			this.btnKidnap.interactable = (!data.CompletelyInfected || (data.CompletelyInfected && !this._settlementPrisonDisplayData.IsStoneRoomFull));
			this.btnKidnap.GetComponent<TooltipInvoker>().enabled = (data.CompletelyInfected && this._settlementPrisonDisplayData.IsStoneRoomFull);
			this.btnRescue.gameObject.SetActive(!data.CompletelyInfected);
			this.focusPosition.Target = row.transform;
			this.focusMask.gameObject.SetActive(true);
		}

		// Token: 0x06005DAF RID: 23983 RVA: 0x002B1963 File Offset: 0x002AFB63
		private void OnClickMask()
		{
			this._currRowIndex = -1;
			this.listScroll.SetSelectedRow(-1);
			this.focusMask.gameObject.SetActive(false);
			this.focusPosition.Target = null;
		}

		// Token: 0x06005DB0 RID: 23984 RVA: 0x002B1998 File Offset: 0x002AFB98
		private void OnClickButton(InteractPrisonerType type)
		{
			bool flag = this._currRowIndex >= 0;
			if (flag)
			{
				this.InteractPrisoner(this._filteredDataList[this._currRowIndex].KidnapCharDisplayData.CharacterId, type);
			}
			this.OnClickMask();
		}

		// Token: 0x06005DB1 RID: 23985 RVA: 0x002B19E0 File Offset: 0x002AFBE0
		private void ToggleGroupTabOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool flag = this._currPage != -1;
			if (flag)
			{
				TaiwuEventDomainMethod.Call.OnSwitchToGuardedPage(this.EnterState, (sbyte)newIndex);
				this._needPlayToggleGroupAnim = true;
			}
		}

		// Token: 0x0400405A RID: 16474
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x0400405B RID: 16475
		[SerializeField]
		private CToggleGroup toggleGroupTab;

		// Token: 0x0400405C RID: 16476
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x0400405D RID: 16477
		[SerializeField]
		private TMP_InputField searchInputField;

		// Token: 0x0400405E RID: 16478
		[Header("模式切换")]
		[SerializeField]
		private CToggleGroup switchToggleGroup;

		// Token: 0x0400405F RID: 16479
		[SerializeField]
		private GameObject listModeRoot;

		// Token: 0x04004060 RID: 16480
		[SerializeField]
		private GameObject gridModeRoot;

		// Token: 0x04004061 RID: 16481
		[Header("列表模式")]
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04004062 RID: 16482
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x04004063 RID: 16483
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04004064 RID: 16484
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04004065 RID: 16485
		[SerializeField]
		private RowCellContainer textWithTipCellContainer;

		// Token: 0x04004066 RID: 16486
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04004067 RID: 16487
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04004068 RID: 16488
		[SerializeField]
		private RowCellContainer ropeCellContainer;

		// Token: 0x04004069 RID: 16489
		[SerializeField]
		private RowCellContainer multiplyButtonCellContainer;

		// Token: 0x0400406A RID: 16490
		[Header("专注模式")]
		[SerializeField]
		private PositionFollower focusPosition;

		// Token: 0x0400406B RID: 16491
		[SerializeField]
		private CButton focusMask;

		// Token: 0x0400406C RID: 16492
		[SerializeField]
		private CButton btnPlead;

		// Token: 0x0400406D RID: 16493
		[SerializeField]
		private CButton btnKidnap;

		// Token: 0x0400406E RID: 16494
		[SerializeField]
		private CButton btnRescue;

		// Token: 0x0400406F RID: 16495
		[Header("平铺模式")]
		[SerializeField]
		private InfinityScroll gridScroll;

		// Token: 0x04004070 RID: 16496
		[Header("守卫")]
		[SerializeField]
		private GameObject characterInfoRoot;

		// Token: 0x04004071 RID: 16497
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004072 RID: 16498
		[SerializeField]
		private TextMeshProUGUI textCharName;

		// Token: 0x04004073 RID: 16499
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04004074 RID: 16500
		private short _settlementId;

		// Token: 0x04004075 RID: 16501
		private bool _isBreaking;

		// Token: 0x04004076 RID: 16502
		private bool _breakingConfirmed;

		// Token: 0x04004077 RID: 16503
		private SettlementPrisonDisplayData _settlementPrisonDisplayData;

		// Token: 0x04004078 RID: 16504
		[NonSerialized]
		public byte EnterState;

		// Token: 0x04004079 RID: 16505
		private sbyte _currPage = 0;

		// Token: 0x0400407A RID: 16506
		private static sbyte _breakJailTarget = -1;

		// Token: 0x0400407B RID: 16507
		private ViewSettlementPrison.PrisonSubPage _currentSubPage = ViewSettlementPrison.PrisonSubPage.Prison;

		// Token: 0x0400407C RID: 16508
		private KidnapMenuDisplayData _kidnapMenuData;

		// Token: 0x0400407D RID: 16509
		private List<CharacterDisplayDataForSettlementPrisoner> _dataList = new List<CharacterDisplayDataForSettlementPrisoner>();

		// Token: 0x0400407E RID: 16510
		private List<CharacterDisplayDataForSettlementPrisoner> _filteredDataList = new List<CharacterDisplayDataForSettlementPrisoner>();

		// Token: 0x0400407F RID: 16511
		private PrisonSortAndFilterController _sortAndFilterController;

		// Token: 0x04004080 RID: 16512
		private TabSortStateManager<ViewSettlementPrison.PrisonSubPage, CharacterDisplayDataForSettlementPrisoner> _tabSortStateManager;

		// Token: 0x04004081 RID: 16513
		private bool _isGridMode = true;

		// Token: 0x04004082 RID: 16514
		private CommonSecondToggleContentRefreshAnim _toggleGroupAnim;

		// Token: 0x04004083 RID: 16515
		private bool _needPlayToggleGroupAnim;

		// Token: 0x04004084 RID: 16516
		private int _currRowIndex;

		// Token: 0x04004085 RID: 16517
		private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_SettlementPrison,
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x04004086 RID: 16518
		private bool _firstEnter;

		// Token: 0x02001C6A RID: 7274
		private enum PrisonSubPage
		{
			// Token: 0x0400C076 RID: 49270
			Prison,
			// Token: 0x0400C077 RID: 49271
			State,
			// Token: 0x0400C078 RID: 49272
			Property,
			// Token: 0x0400C079 RID: 49273
			Property2,
			// Token: 0x0400C07A RID: 49274
			LifeSkill,
			// Token: 0x0400C07B RID: 49275
			CombatSkill,
			// Token: 0x0400C07C RID: 49276
			Personality,
			// Token: 0x0400C07D RID: 49277
			Item,
			// Token: 0x0400C07E RID: 49278
			Command
		}
	}
}
