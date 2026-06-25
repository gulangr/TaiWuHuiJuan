using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.NewGame;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000996 RID: 2454
	public class ViewLegacy : UIBase
	{
		// Token: 0x17000D4C RID: 3404
		// (get) Token: 0x0600762C RID: 30252 RVA: 0x00371370 File Offset: 0x0036F570
		private bool CanModifyWorldState
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.LockWorldSettings) ? (GlobalOperations.CanResetWorldSettings && !this._inherit) : (GlobalOperations.CanResetWorldSettings || this._inherit || this._crossArchive);
			}
		}

		// Token: 0x17000D4D RID: 3405
		// (get) Token: 0x0600762D RID: 30253 RVA: 0x003713BC File Offset: 0x0036F5BC
		private bool IsSimple
		{
			get
			{
				return !this._inherit && !this._crossArchive && !GlobalOperations.CanResetWorldSettings;
			}
		}

		// Token: 0x0600762E RID: 30254 RVA: 0x003713DC File Offset: 0x0036F5DC
		private void Awake()
		{
			this.legacyContainer.Selected = new Action<LegacyItem, bool>(this.OnSelected);
			this.legacyContainer.Set = new Action<LegacyItem>(this.SetConflict);
			this.getReward.onClick.ResetListener(new Action(this.GetRandomLegacy));
			this.confirm.onClick.ResetListener(new Action(this.Submit));
			this.searchOther.onClick.ResetListener(new Action(this.SearchOtherTaiwu));
			this.openJieqingInteractButton.onClick.ResetListener(new Action(this.OpenJieqingInteract));
			this.group.Init(-1);
		}

		// Token: 0x0600762F RID: 30255 RVA: 0x0037149C File Offset: 0x0036F69C
		private void Submit()
		{
			bool flag = this._inherit || this._crossArchive;
			if (flag)
			{
				this.ConfirmInherit();
			}
			else
			{
				this.ConfirmResetWorldSetting();
			}
		}

		// Token: 0x06007630 RID: 30256 RVA: 0x003714CF File Offset: 0x0036F6CF
		private void SearchOtherTaiwu()
		{
			this._selectLegacyCharacters.Show();
		}

		// Token: 0x06007631 RID: 30257 RVA: 0x003714DE File Offset: 0x0036F6DE
		private void OpenJieqingInteract()
		{
			UIManager.Instance.ShowUI(UIElement.JieQingInteract, true);
		}

		// Token: 0x06007632 RID: 30258 RVA: 0x003714F4 File Offset: 0x0036F6F4
		private void OnSelected(LegacyItem selected, bool isOn)
		{
			short conflictingFeatureId;
			bool flag = isOn && this.GetFeatureConflictType(selected.id, out conflictingFeatureId) != ConflictType.None;
			if (flag)
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Type = 1,
					Title = LanguageKey.LK_Legacy_ReplaceFeature_Dialog_Title.Tr(),
					Content = LanguageKey.LK_Legacy_ReplaceFeature_Dialog_Content.TrFormat(CharacterFeature.Instance[conflictingFeatureId].Name),
					Yes = delegate()
					{
						this.SwitchSelected(selected, true);
					},
					No = delegate()
					{
						selected.toggle.SetIsOnWithoutNotify(false);
						selected.selected.SetActive(false);
					}
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.SwitchSelected(selected, isOn);
			}
		}

		// Token: 0x06007633 RID: 30259 RVA: 0x003715D8 File Offset: 0x0036F7D8
		private void SwitchSelected(LegacyItem selected, bool isOn)
		{
			if (isOn)
			{
				this.OnSelectLegacy(selected.id);
			}
			bool flag = !selected.IsFree;
			if (flag)
			{
				this.CostPoint += (int)(isOn ? Legacy.Instance[selected.id].Cost : (-(int)Legacy.Instance[selected.id].Cost));
				this.RefreshTotalAvailableLegacyPointLabel();
			}
			GEvent.OnEvent(UiEvents.RequestLegacyItemRefresh, null);
		}

		// Token: 0x06007634 RID: 30260 RVA: 0x0037165C File Offset: 0x0036F85C
		private void SetConflict(LegacyItem item)
		{
			short conflictingFeatureId;
			switch (item.ConflictType = this.GetFeatureConflictType(item.id, out conflictingFeatureId))
			{
			case ConflictType.OppositeSign:
				item.tipDisplayer.PresetParam[0] = Legacy.Instance[item.id].Desc + "\n" + LanguageKey.LK_Legacy_ConflictFeature_Hint_1.TrFormat(CharacterFeature.Instance[conflictingFeatureId].Name);
				item.toggle.interactable = false;
				return;
			case ConflictType.LowerLevel:
				item.tipDisplayer.PresetParam[0] = Legacy.Instance[item.id].Desc + "\n" + LanguageKey.LK_Legacy_ConflictFeature_Hint_0.TrFormat(CharacterFeature.Instance[conflictingFeatureId].Name);
				item.toggle.interactable = false;
				return;
			}
			item.tipDisplayer.PresetParam[0] = ((conflictingFeatureId != -1 && item.ConflictType != ConflictType.HigherLevel && !item.SelectedIsOn) ? (Legacy.Instance[item.id].Desc + "\n" + LanguageKey.LK_Legacy_ConflictFeature_Hint_2.TrFormat(Legacy.Instance[conflictingFeatureId].Name)) : Legacy.Instance[item.id].Desc);
		}

		// Token: 0x06007635 RID: 30261 RVA: 0x003717C0 File Offset: 0x0036F9C0
		public override void OnInit(ArgumentBox argsBox)
		{
			this._cleared = false;
			argsBox.Get("Inherit", out this._inherit);
			argsBox.Get("CharacterId", out this._charId);
			argsBox.Get("CrossArchive", out this._crossArchive);
			this.close.gameObject.SetActive(!this._crossArchive);
			bool crossArchive = this._crossArchive;
			if (crossArchive)
			{
				this._charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
			Selectable selectable = this.challengeToggle;
			BasicGameData instance = SingletonObject.getInstance<BasicGameData>();
			bool interactable;
			if (instance == null)
			{
				interactable = false;
			}
			else
			{
				ChallengeModeData challengeModeData = instance.ChallengeModeData;
				bool? flag = (challengeModeData != null) ? new bool?(challengeModeData.IsEnabled()) : null;
				bool flag2 = true;
				interactable = (flag.GetValueOrDefault() == flag2 & flag != null);
			}
			selectable.interactable = interactable;
			this.canTransferText.text = (this._inherit ? (this.CanModifyWorldState ? LanguageKey.LK_Legacy_Tips.Tr() : LanguageKey.LK_Legacy_Tips_Challenge.Tr()) : (this.CanModifyWorldState ? LanguageKey.LK_Legacy_Unavailable_Tips_CanResetWorldSettings.TrFormat(SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName) : LanguageKey.LK_Legacy_Unavailable_Tips.TrFormat(SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName))).ColorReplace();
			this._enteredPassingLegacy = false;
			foreach (GameObject go in this.middleLegacyIcons)
			{
				go.SetActive(false);
			}
			bool isSimple = this.IsSimple;
			if (isSimple)
			{
				this.legacyMiddle.SetHeight(this.legacyMiddleSimpleHeight);
				this.legacyBottom.SetHeight(this.legacyBottomSimpleHeight);
			}
			else
			{
				this.legacyMiddle.SetHeight(this.legacyMiddleComplexHeight);
				this.legacyBottom.SetHeight(this.legacyBottomComplexHeight);
			}
			this.confirm.gameObject.SetActive(this._inherit || GlobalOperations.CanResetWorldSettings);
			OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
			{
				bool unlock = false;
				Serializer.Deserialize(pool, offset, ref unlock);
				this.openJieqingInteractButton.gameObject.SetActive(unlock && !this._inherit);
			});
			TaiwuDomainMethod.AsyncCall.GetTaiwuLifeSummaryDisplayData(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._taiwuSummaryDisplayData);
				this._characterDisplayData = this._taiwuSummaryDisplayData.TotalTaiwuDisplayDatas;
			});
			TooltipInvoker tooltipInvoker = this.fuyu;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("ItemData", new ItemDisplayData(12, 239));
			}
			this.getReward.gameObject.SetActive(this._inherit || this._crossArchive);
			this._createRandomLegacyTimes = this.legacyContainer.FreeCount;
			this.CostPoint = 0;
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x06007636 RID: 30262 RVA: 0x00371A6C File Offset: 0x0036FC6C
		private void RefreshTaiwuSummaryPage()
		{
			TooltipInvoker tip = this.searchOther.GetComponent<TooltipInvoker>();
			bool flag = this._characterDisplayData.Count <= 0;
			if (flag)
			{
				this.searchOther.interactable = false;
				tip.enabled = true;
				tip.Type = TipType.SingleDesc;
				tip.PresetParam = new string[]
				{
					LanguageKey.LK_View_Taiwu_NoPastDynasties.Tr().SetColor("brightred")
				};
			}
			else
			{
				tip.enabled = false;
				this.searchOther.interactable = true;
				CommonUtils.PrepareEnoughChildren(this.extractContent, this.taiwuSummaryTemplate.gameObject, this._characterDisplayData.Count, null);
				for (int i = 0; i < this.extractContent.childCount; i++)
				{
					TaiwuSummaryTemplate taiwuTemplate = this.extractContent.GetChild(i).GetComponent<TaiwuSummaryTemplate>();
					CharacterDisplayData taiwu = this._characterDisplayData[i];
					TaiwuSummaryTemplate taiwuSummaryTemplate = taiwuTemplate;
					int index = i;
					CharacterDisplayData data = taiwu;
					bool isCurrentTaiwu = false;
					AbridgedCharacter abridgedCharacter = this._taiwuSummaryDisplayData.TotalTaiwuLifeSummaries[i].AbridgedCharacter;
					taiwuSummaryTemplate.Set(index, data, isCurrentTaiwu, (abridgedCharacter != null) ? abridgedCharacter.GenerateAvatarRelatedData() : null);
				}
				this._selectLegacyCharacters.RefreshScrollButtons();
			}
		}

		// Token: 0x06007637 RID: 30263 RVA: 0x00371BA3 File Offset: 0x0036FDA3
		public static int GetCreateRandomLegacyCost(int times)
		{
			return (int)Math.Pow(2.0, (double)times) * GlobalConfig.Instance.SelectRandomLegacyCost;
		}

		// Token: 0x06007638 RID: 30264 RVA: 0x00371BC4 File Offset: 0x0036FDC4
		public void GetRandomLegacy()
		{
			int cost = ViewLegacy.GetCreateRandomLegacyCost(this._createRandomLegacyTimes);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("LegacyPoint", this._legacyDisplayData.LegacyPoint - this.CostPoint).Set("Cost", cost).SetObject("WorldCreationInfo", this._legacyDisplayData.WorldCreationInfo).SetObject("OnSelectLegacy", new Action<short>(this.legacyContainer.AddExtraLegacy)).SetObject("OnCreateRandomLegacy", new Action(this.CreateRandomLegacy));
			UIElement.SelectLegacyRewardGroup.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.SelectLegacyRewardGroup);
		}

		// Token: 0x06007639 RID: 30265 RVA: 0x00371C74 File Offset: 0x0036FE74
		public void CreateRandomLegacy()
		{
			this._legacyDisplayData.LegacyPoint -= ViewLegacy.GetCreateRandomLegacyCost(this._createRandomLegacyTimes);
			int createRandomLegacyTimes = this._createRandomLegacyTimes;
			this._createRandomLegacyTimes = createRandomLegacyTimes + 1;
			TaiwuDomainMethod.Call.ChangeLegacyPointWhilePassingLegacy(-ViewLegacy.GetCreateRandomLegacyCost(createRandomLegacyTimes));
			this.RefreshTotalAvailableLegacyPointLabel();
		}

		// Token: 0x0600763A RID: 30266 RVA: 0x00371CC3 File Offset: 0x0036FEC3
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.RequestLegacyDisplayData(this, this._charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._legacyDisplayData);
				this._characterDisplayData.RemoveAll((CharacterDisplayData data) => data.CharacterId == this._legacyDisplayData.OldTaiwuChar.CharacterId);
				this.Refresh();
				this.RefreshTaiwuSummaryPage();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x0600763B RID: 30267 RVA: 0x00371CDF File Offset: 0x0036FEDF
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 199, ulong.MaxValue, null));
		}

		// Token: 0x0600763C RID: 30268 RVA: 0x00371D00 File Offset: 0x0036FF00
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					DataUid uid = notification.Uid;
					bool flag = uid.DomainId == 19 && uid.DataId == 199;
					if (flag)
					{
						int starFortunePoint = 0;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref starFortunePoint);
						string title = LanguageKey.LK_SectMainStory_JieQing_AvailableStarFortune.Tr();
						this.startLuckyText.text = string.Format(title, starFortunePoint);
					}
				}
			}
		}

		// Token: 0x0600763D RID: 30269 RVA: 0x00371DD4 File Offset: 0x0036FFD4
		private void Refresh()
		{
			this.selfAvatar.Refresh(this._legacyDisplayData.OldTaiwuChar, false);
			this.selfName.text = NameCenter.GetNameByDisplayData(this._legacyDisplayData.OldTaiwuChar, false, true);
			int selfInheritCount = Mathf.Max(1, this._characterDisplayData.Count + 1);
			this.selfCount.text = string.Format("{0}", selfInheritCount);
			bool flag = this._inherit || this._crossArchive;
			if (flag)
			{
				this.nextAvatar.Refresh(this._legacyDisplayData.InheritChar, false);
				this.nextName.text = NameCenter.GetNameByDisplayData(this._legacyDisplayData.InheritChar, false, true);
				this.nextAvatar.transform.parent.gameObject.SetActive(true);
				int nextInheritCount = Mathf.Max(1, this._characterDisplayData.Count + 1) + 1;
				this.nextCount.text = string.Format("{0}", nextInheritCount);
				this.nextInfoGo.SetActive(true);
			}
			else
			{
				this.nextAvatar.transform.parent.gameObject.SetActive(false);
				this.nextInfoGo.SetActive(false);
			}
			LegacyContainer legacyContainer = this.legacyContainer;
			List<short> availableLegacyList = this._legacyDisplayData.AvailableLegacyList;
			IEnumerable<short> itemTemplateIds;
			if (availableLegacyList == null)
			{
				itemTemplateIds = null;
			}
			else
			{
				itemTemplateIds = from x in availableLegacyList
				orderby x
				select x;
			}
			legacyContainer.RefreshItems(itemTemplateIds, this._inherit || this._crossArchive);
			EInteractType mode = (this._inherit || this._crossArchive) ? EInteractType.Inherit : EInteractType.Legacy;
			this.worldDetailPanel.Init(ref this._legacyDisplayData.WorldCreationInfo, mode);
			this.UpdateLegacyPointText();
			this.RefreshChallenge();
			GEvent.OnEvent(UiEvents.RequestLegacyItemRefresh, null);
		}

		// Token: 0x0600763E RID: 30270 RVA: 0x00371FBF File Offset: 0x003701BF
		public void OnSelectLegacy(short templateId)
		{
			this.legacyContainer.DeselectAllConflictLegacies(templateId);
		}

		// Token: 0x0600763F RID: 30271 RVA: 0x00371FD0 File Offset: 0x003701D0
		public void RefreshTotalAvailableLegacyPointLabel()
		{
			int now = this._legacyDisplayData.LegacyPoint - this.CostPoint;
			this.legacyContainer.RefreshInteractable(now);
			this.getReward.interactable = true;
			this.legacyPoint.text = LanguageKey.LK_Legacy_Point_Summary.TrFormat(now, this._legacyPointMaxTotal).ColorReplace();
		}

		// Token: 0x06007640 RID: 30272 RVA: 0x00372038 File Offset: 0x00370238
		public ConflictType GetFeatureConflictType(short legacyTemplateId, out short conflictingFeature)
		{
			conflictingFeature = -1;
			LegacyItem legacyConfig = Legacy.Instance[legacyTemplateId];
			bool flag = legacyConfig.AddFeature < 0;
			ConflictType result;
			if (flag)
			{
				result = ConflictType.None;
			}
			else
			{
				CharacterFeatureItem featureConfig = CharacterFeature.Instance[legacyConfig.AddFeature];
				bool flag2 = featureConfig.MutexGroupId < 0;
				if (flag2)
				{
					result = ConflictType.None;
				}
				else
				{
					foreach (short taiwuFeatureId in this._legacyDisplayData.InheritChar.FeatureIds)
					{
						conflictingFeature = taiwuFeatureId;
						bool flag3 = featureConfig.TemplateId == taiwuFeatureId;
						if (flag3)
						{
							return ConflictType.LowerLevel;
						}
						CharacterFeatureItem taiwuFeature = CharacterFeature.Instance[taiwuFeatureId];
						bool flag4 = taiwuFeature.MutexGroupId != featureConfig.MutexGroupId;
						if (!flag4)
						{
							bool flag5 = featureConfig.Type != taiwuFeature.Type;
							if (flag5)
							{
								return ConflictType.OppositeSign;
							}
							bool flag6 = featureConfig.Level <= taiwuFeature.Level;
							if (flag6)
							{
								return ConflictType.LowerLevel;
							}
							return ConflictType.HigherLevel;
						}
					}
					conflictingFeature = -1;
					ValueTuple<List<short>, List<short>> selected2 = this.legacyContainer.GetSelected();
					List<short> selected = selected2.Item1;
					List<short> free = selected2.Item2;
					foreach (short item in selected.Concat(free))
					{
						bool flag7 = LegacyContainer.CheckLegacyConflict(legacyTemplateId, item);
						if (flag7)
						{
							conflictingFeature = item;
							return ConflictType.None;
						}
					}
					result = ConflictType.None;
				}
			}
			return result;
		}

		// Token: 0x06007641 RID: 30273 RVA: 0x003721EC File Offset: 0x003703EC
		private void UpdateLegacyPointText()
		{
			int[] legacyPointTypeTotal = new int[LegacyPointType.Instance.Count];
			Array.Fill<int>(legacyPointTypeTotal, 0);
			bool flag = this._legacyDisplayData.LegacyPointDict != null;
			if (flag)
			{
				foreach (short templateId in this._legacyDisplayData.LegacyPointDict.Keys)
				{
					sbyte idx = LegacyPoint.Instance[templateId].Type;
					bool flag2 = idx >= 0;
					if (flag2)
					{
						legacyPointTypeTotal[(int)idx] += (int)this._legacyDisplayData.LegacyPointDict[templateId];
					}
				}
			}
			this._legacyPointMaxTotal = 0;
			foreach (LegacyPointTypeItem legacyPointType in ((IEnumerable<LegacyPointTypeItem>)LegacyPointType.Instance))
			{
				bool flag3 = (int)legacyPointType.TemplateId > this.legacyPointNames.Length - 1;
				if (!flag3)
				{
					int typePoints = legacyPointTypeTotal[(int)legacyPointType.TemplateId];
					List<IntList> legacyMaxPointAndTimesList = GameData.Domains.Taiwu.SharedMethods.GetLegacyMaxPointAndTimesListByType(this._legacyDisplayData.WorldCreationInfo, this._legacyDisplayData.LegacyPointTimesDict, (short)legacyPointType.TemplateId);
					int totalTypePoints = 0;
					foreach (IntList list in legacyMaxPointAndTimesList)
					{
						totalTypePoints += list.Items[1];
					}
					this._legacyPointMaxTotal += totalTypePoints;
					TMP_Text nameText = this.legacyPointNames[(int)legacyPointType.TemplateId];
					TMP_Text valueText = this.legacyPointValues[(int)legacyPointType.TemplateId];
					nameText.text = legacyPointType.Name;
					bool flag4 = typePoints >= totalTypePoints;
					if (flag4)
					{
						valueText.text = string.Format("<SpName=legacy_icon_max>{0}/{1}", typePoints, totalTypePoints).SetColor("pinkyellow");
						valueText.GetComponent<TMPTextSpriteHelper>().Parse();
					}
					else
					{
						valueText.text = typePoints.ToString().SetColor("lightgrey") + "/" + totalTypePoints.ToString();
					}
					this.RefreshLegacyPointsMouseTip(this.legacyTips[(int)legacyPointType.TemplateId], legacyPointType, legacyMaxPointAndTimesList, typePoints, totalTypePoints);
				}
			}
			this.RefreshTotalAvailableLegacyPointLabel();
		}

		// Token: 0x06007642 RID: 30274 RVA: 0x00372494 File Offset: 0x00370694
		private void RefreshLegacyPointsMouseTip(TooltipInvoker tip, LegacyPointTypeItem legacyPointType, List<IntList> legacyMaxPointAndTimesList, int typePoint, int totalTypePoint)
		{
			ArgumentBox argumentBox;
			if ((argumentBox = tip.RuntimeParam) == null)
			{
				argumentBox = (tip.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			List<Vector2Int> legacyProperties;
			argumentBox.Get<List<Vector2Int>>("LegacyProperties", out legacyProperties);
			if (legacyProperties == null)
			{
				legacyProperties = EasyPool.Get<List<Vector2Int>>();
			}
			legacyProperties.Clear();
			legacyProperties.AddRange((from item in LegacyPoint.Instance
			where item.Type == legacyPointType.TemplateId
			select item).Select(delegate(LegacyPointItem item)
			{
				int templateId = (int)item.TemplateId;
				Dictionary<short, short> legacyPointDict = this._legacyDisplayData.LegacyPointDict;
				return new Vector2Int(templateId, (int)((legacyPointDict != null) ? legacyPointDict.GetValueOrDefault(item.TemplateId) : 0));
			}));
			tip.RuntimeParam.Set("Title", LocalStringManager.GetFormat(LanguageKey.LK_Legacy_TypeTitle, legacyPointType.Name)).Set("MaxTitle", LocalStringManager.GetFormat(LanguageKey.LK_Legacy_TypeMaxTitle, legacyPointType.Name)).Set("TypePoint", typePoint).SetObject("LegacyProperties", legacyProperties).Set("TotalTypePoint", totalTypePoint).SetObject("LegacyMaxPointAndTimesList", legacyMaxPointAndTimesList).Set<WorldCreationInfo>("WorldCreationInfo", this._legacyDisplayData.WorldCreationInfo).Set("LegacyPointBonusFactor", this._legacyDisplayData.LegacyPointBonusFactor);
			tip.enabled = (legacyProperties.Count > 0);
		}

		// Token: 0x06007643 RID: 30275 RVA: 0x003725C9 File Offset: 0x003707C9
		private void SetWorldCreationInfo()
		{
			this.worldDetailPanel.Save(new Dictionary<string, string>(), ref this._legacyDisplayData.WorldCreationInfo);
			WorldDomainMethod.Call.SetWorldCreationInfo(this._legacyDisplayData.WorldCreationInfo, this._inherit);
		}

		// Token: 0x06007644 RID: 30276 RVA: 0x00372600 File Offset: 0x00370800
		private void ConfirmInherit()
		{
			bool enteredPassingLegacy = this._enteredPassingLegacy;
			if (!enteredPassingLegacy)
			{
				ValueTuple<List<short>, List<short>> selected = this.legacyContainer.GetSelected();
				List<short> normalFeature = selected.Item1;
				List<short> freeFeature = selected.Item2;
				new DialogCmdHuge
				{
					Title = LanguageKey.LK_Taiwu_Inherit_Complete.Tr(),
					Content = ((normalFeature.Count + freeFeature.Count == 0) ? LanguageKey.LK_Taiwu_Inherit_Complete_Confirm_Empty : LanguageKey.LK_Taiwu_Inherit_Complete_Confirm).Tr().ColorReplace(),
					Left = delegate()
					{
						this._enteredPassingLegacy = true;
						TaiwuDomainMethod.Call.SelectLegacies(normalFeature, freeFeature);
						this.SetWorldCreationInfo();
						bool crossArchive = this._crossArchive;
						if (crossArchive)
						{
							TaiwuEventDomainMethod.Call.TriggerListener("SelectLegacyComplete", true);
						}
						else
						{
							TaiwuDomainMethod.Call.CompletePassingLegacy();
							TaiwuDomainMethod.Call.ConfirmChosenSuccessor(this._legacyDisplayData.InheritChar.CharacterId);
						}
						int cost = this.CostPoint + Enumerable.Range(0, this._createRandomLegacyTimes).Sum(new Func<int, int>(ViewLegacy.GetCreateRandomLegacyCost));
						WorldDomainMethod.Call.RequestSetStat(47, cost);
						WorldDomainMethod.Call.RequestSetStat(48, normalFeature.Count + freeFeature.Count);
						this.legacyContainer.OnSubmit();
						this.<>n__0();
					},
					Right = delegate()
					{
						this._enteredPassingLegacy = false;
					}
				}.SetDefaultText().Show();
			}
		}

		// Token: 0x06007645 RID: 30277 RVA: 0x003726C8 File Offset: 0x003708C8
		private void ConfirmResetWorldSetting()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Type = 1,
				Title = LocalStringManager.Get(LanguageKey.UI_Reset_World_Config_Title),
				Content = LocalStringManager.Get(LanguageKey.UI_Reset_World_Config_Desc).ColorReplace(),
				Yes = delegate()
				{
					this.SetWorldCreationInfo();
					base.QuickHide();
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007646 RID: 30278 RVA: 0x00372744 File Offset: 0x00370944
		public override void QuickHide()
		{
			bool crossArchive = this._crossArchive;
			if (!crossArchive)
			{
				bool inherit = this._inherit;
				if (inherit)
				{
					TaiwuDomainMethod.AsyncCall.FindSuccessorCandidates(this, true, delegate(int offset, RawDataPool pool)
					{
						base.QuickHide();
						SingletonObject.getInstance<DisplayTriggerModel>().Handle_FindSuccessorCandidates(offset, pool);
					});
				}
				else
				{
					base.QuickHide();
				}
			}
		}

		// Token: 0x06007647 RID: 30279 RVA: 0x00372788 File Offset: 0x00370988
		private void RefreshChallenge()
		{
			ChallengeModeData challengeModeData = this._legacyDisplayData.ChallengeModeData;
			bool isEnabled = challengeModeData != null && challengeModeData.IsEnabled();
			this.switchToggle.SetIsOnWithoutNotify(isEnabled);
			this.switchToggle.interactable = false;
			ChallengeModeData challengeModeData2 = this._legacyDisplayData.ChallengeModeData;
			int? num;
			if (challengeModeData2 == null)
			{
				num = null;
			}
			else
			{
				IReadOnlyList<int> challengeModeIds = challengeModeData2.ChallengeModeIds;
				num = ((challengeModeIds != null) ? new int?(challengeModeIds.Count) : null);
			}
			int? count = num;
			this.textCount.text = LanguageKey.LK_NewGame_Challenge_Count.TrFormat(count);
			List<ChallengeModeItem> essentialConfigList = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Required
			select c).ToList<ChallengeModeItem>();
			for (int i = 0; i < essentialConfigList.Count; i++)
			{
				ChallengeModeItem config = essentialConfigList[i];
				NewGameSubPageChallengeItem item = this.essentialLayout.GetChild(i).GetComponent<NewGameSubPageChallengeItem>();
				item.Init(config.TemplateId, true, this.IsItemEnable(config.Implement), null);
			}
			List<ChallengeModeItem> optionalConfigList = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Optional
			select c).ToList<ChallengeModeItem>();
			for (int j = 0; j < optionalConfigList.Count; j++)
			{
				ChallengeModeItem config2 = optionalConfigList[j];
				NewGameSubPageChallengeItem item2 = this.optionalLayout.GetChild(j).GetComponent<NewGameSubPageChallengeItem>();
				item2.Init(config2.TemplateId, true, this.IsItemEnable(config2.Implement), null);
			}
			List<ChallengeModeItem> bonusConfigList = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Bonus
			select c).ToList<ChallengeModeItem>();
			for (int k = 0; k < bonusConfigList.Count; k++)
			{
				ChallengeModeItem config3 = bonusConfigList[k];
				NewGameSubPageChallengeItem item3 = this.bonusLayout.GetChild(k).GetComponent<NewGameSubPageChallengeItem>();
				item3.Init(config3.TemplateId, true, this.IsItemEnable(config3.Implement), null);
			}
		}

		// Token: 0x06007648 RID: 30280 RVA: 0x003729BC File Offset: 0x00370BBC
		private bool IsItemEnable(EChallengeModeImplement implement)
		{
			ChallengeModeData challengeModeData = this._legacyDisplayData.ChallengeModeData;
			return challengeModeData != null && challengeModeData.IsEnabled(implement);
		}

		// Token: 0x040058FD RID: 22781
		[SerializeField]
		private RectTransform legacyMiddle;

		// Token: 0x040058FE RID: 22782
		[SerializeField]
		private RectTransform legacyBottom;

		// Token: 0x040058FF RID: 22783
		[SerializeField]
		private LegacyContainer legacyContainer;

		// Token: 0x04005900 RID: 22784
		[SerializeField]
		private Game.Components.Avatar.Avatar selfAvatar;

		// Token: 0x04005901 RID: 22785
		[SerializeField]
		private Game.Components.Avatar.Avatar nextAvatar;

		// Token: 0x04005902 RID: 22786
		[SerializeField]
		private TMP_Text selfName;

		// Token: 0x04005903 RID: 22787
		[SerializeField]
		private TMP_Text selfCount;

		// Token: 0x04005904 RID: 22788
		[SerializeField]
		private TMP_Text nextName;

		// Token: 0x04005905 RID: 22789
		[SerializeField]
		private TMP_Text nextCount;

		// Token: 0x04005906 RID: 22790
		[SerializeField]
		private GameObject nextInfoGo;

		// Token: 0x04005907 RID: 22791
		[SerializeField]
		private Sprite nowCn;

		// Token: 0x04005908 RID: 22792
		[SerializeField]
		private Sprite nowEn;

		// Token: 0x04005909 RID: 22793
		[SerializeField]
		private TooltipInvoker fuyu;

		// Token: 0x0400590A RID: 22794
		[SerializeField]
		private TMP_Text canTransferText;

		// Token: 0x0400590B RID: 22795
		[SerializeField]
		private TMP_Text legacyPoint;

		// Token: 0x0400590C RID: 22796
		[SerializeField]
		private TextMeshProUGUI startLuckyText;

		// Token: 0x0400590D RID: 22797
		[SerializeField]
		private CToggleGroup group;

		// Token: 0x0400590E RID: 22798
		[SerializeField]
		private CButton getReward;

		// Token: 0x0400590F RID: 22799
		[SerializeField]
		private CButton confirm;

		// Token: 0x04005910 RID: 22800
		[SerializeField]
		private CButton close;

		// Token: 0x04005911 RID: 22801
		[SerializeField]
		private CButton searchOther;

		// Token: 0x04005912 RID: 22802
		[SerializeField]
		private CButton openJieqingInteractButton;

		// Token: 0x04005913 RID: 22803
		[SerializeField]
		private NewGameSubPageWorldDetailPanel worldDetailPanel;

		// Token: 0x04005914 RID: 22804
		[SerializeField]
		private float legacyMiddleSimpleHeight = 328f;

		// Token: 0x04005915 RID: 22805
		[SerializeField]
		private float legacyBottomSimpleHeight = 226f;

		// Token: 0x04005916 RID: 22806
		[SerializeField]
		private float legacyMiddleComplexHeight = 168f;

		// Token: 0x04005917 RID: 22807
		[SerializeField]
		private float legacyBottomComplexHeight = 386f;

		// Token: 0x04005918 RID: 22808
		[SerializeField]
		private GameObject[] middleLegacyIcons;

		// Token: 0x04005919 RID: 22809
		[SerializeField]
		private ViewSelectLegacyCharacters _selectLegacyCharacters;

		// Token: 0x0400591A RID: 22810
		[Header("玄狱内容")]
		[SerializeField]
		private CToggle challengeToggle;

		// Token: 0x0400591B RID: 22811
		[SerializeField]
		private CToggle switchToggle;

		// Token: 0x0400591C RID: 22812
		[SerializeField]
		private TextMeshProUGUI textCount;

		// Token: 0x0400591D RID: 22813
		[SerializeField]
		private Transform essentialLayout;

		// Token: 0x0400591E RID: 22814
		[SerializeField]
		private Transform optionalLayout;

		// Token: 0x0400591F RID: 22815
		[SerializeField]
		private Transform bonusLayout;

		// Token: 0x04005920 RID: 22816
		[Header("历代太吾")]
		[SerializeField]
		private GameObject extractPanel;

		// Token: 0x04005921 RID: 22817
		[SerializeField]
		private TaiwuSummaryTemplate taiwuSummaryTemplate;

		// Token: 0x04005922 RID: 22818
		[SerializeField]
		private RectTransform extractContent;

		// Token: 0x04005923 RID: 22819
		private bool _inherit;

		// Token: 0x04005924 RID: 22820
		private bool _crossArchive;

		// Token: 0x04005925 RID: 22821
		private bool _enteredPassingLegacy;

		// Token: 0x04005926 RID: 22822
		private int _charId;

		// Token: 0x04005927 RID: 22823
		private LegacyDisplayData _legacyDisplayData;

		// Token: 0x04005928 RID: 22824
		private TaiwuLifeSummaryDisplayData _taiwuSummaryDisplayData = new TaiwuLifeSummaryDisplayData();

		// Token: 0x04005929 RID: 22825
		private List<CharacterDisplayData> _characterDisplayData;

		// Token: 0x0400592A RID: 22826
		private int _createRandomLegacyTimes;

		// Token: 0x0400592B RID: 22827
		private bool _cleared;

		// Token: 0x0400592C RID: 22828
		private List<short> _extraLegacies;

		// Token: 0x0400592D RID: 22829
		private List<short> _extraLegaciesCache;

		// Token: 0x0400592E RID: 22830
		[NonSerialized]
		public int CostPoint;

		// Token: 0x0400592F RID: 22831
		[SerializeField]
		private TMP_Text[] legacyPointNames;

		// Token: 0x04005930 RID: 22832
		[SerializeField]
		private TMP_Text[] legacyPointValues;

		// Token: 0x04005931 RID: 22833
		[SerializeField]
		private TooltipInvoker[] legacyTips;

		// Token: 0x04005932 RID: 22834
		[SerializeField]
		private CImage[] fills;

		// Token: 0x04005933 RID: 22835
		private int _legacyPointMaxTotal;
	}
}
