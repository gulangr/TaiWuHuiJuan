using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Organization;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Shixiang
{
	// Token: 0x020009E5 RID: 2533
	public class ViewShixiangUpgradeTeammateCommand : UIBase
	{
		// Token: 0x17000DA6 RID: 3494
		// (get) Token: 0x06007C2E RID: 31790 RVA: 0x0039BC7C File Offset: 0x00399E7C
		private int AdvanceCommandCount
		{
			get
			{
				GroupCharDisplayData data;
				int result;
				if (!this._groupCharDisplayDataDict.TryGetValue(this._selectedCharId, out data))
				{
					result = 0;
				}
				else
				{
					List<sbyte> items = data.AdvancedCommand.Items;
					result = ((items != null) ? items.Count : 0);
				}
				return result;
			}
		}

		// Token: 0x17000DA7 RID: 3495
		// (get) Token: 0x06007C2F RID: 31791 RVA: 0x0039BCB8 File Offset: 0x00399EB8
		private int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000DA8 RID: 3496
		// (get) Token: 0x06007C30 RID: 31792 RVA: 0x0039BCC4 File Offset: 0x00399EC4
		private int Days
		{
			get
			{
				return SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			}
		}

		// Token: 0x17000DA9 RID: 3497
		// (get) Token: 0x06007C31 RID: 31793 RVA: 0x0039BCD0 File Offset: 0x00399ED0
		private int Authority
		{
			get
			{
				return this._resourceMonitor.Resources[7];
			}
		}

		// Token: 0x06007C32 RID: 31794 RVA: 0x0039BCDF File Offset: 0x00399EDF
		public override void OnInit(ArgumentBox argsBox)
		{
			this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this.TaiwuId, false);
		}

		// Token: 0x06007C33 RID: 31795 RVA: 0x0039BCF9 File Offset: 0x00399EF9
		private void Awake()
		{
			this.InitCharacter();
			this.InitCommand();
			this.InitDrum();
		}

		// Token: 0x06007C34 RID: 31796 RVA: 0x0039BD11 File Offset: 0x00399F11
		private void OnEnable()
		{
			this._isNeedSortCommandList = true;
			this.RequestData();
		}

		// Token: 0x06007C35 RID: 31797 RVA: 0x0039BD24 File Offset: 0x00399F24
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007C36 RID: 31798 RVA: 0x0039BD51 File Offset: 0x00399F51
		public override void QuickHide()
		{
			base.QuickHide();
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntListArg("UpgradeTeammateCommand", "Teammates", this._changedCharIdList);
			TaiwuEventDomainMethod.Call.TriggerListener("UpgradeTeammateCommand", true);
		}

		// Token: 0x06007C37 RID: 31799 RVA: 0x0039BD80 File Offset: 0x00399F80
		private void RequestData()
		{
			this._charIdList.Clear();
			this._charIdList.AddRange(SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds());
			this._charIdList.Remove(this.TaiwuId);
			CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(this, this._charIdList, delegate(int offset, RawDataPool pool)
			{
				this._groupCharDisplayDataList.Clear();
				Serializer.Deserialize(pool, offset, ref this._groupCharDisplayDataList);
				this.UpdateCharacterMedalDict();
				this.UpdateCharacterList();
				bool flag = this._filterdCharIdList.Count > 0;
				if (flag)
				{
					this.OnCharacterSelect(this._filterdCharIdList[0]);
				}
				else
				{
					this.OnCharacterSelect(-1);
				}
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06007C38 RID: 31800 RVA: 0x0039BDDC File Offset: 0x00399FDC
		public void OnClickUpgrade(int charId, sbyte commandId)
		{
			sbyte upgradeId = CommonUtils.GetAdvanceTeammateCommand(commandId);
			this._operatingCharAndCommand = new ValueTuple<int, sbyte, sbyte, bool>(charId, commandId, upgradeId, true);
			this.ChangeCacheCommandList(upgradeId, commandId);
			this._changeId = (int)upgradeId;
			ExtraDomainMethod.AsyncCall.SetAdvancedTeammateCommands(null, charId, upgradeId, new AsyncMethodCallbackDelegate(this.OnFinishUpgradeOrDownGradeCommand));
		}

		// Token: 0x06007C39 RID: 31801 RVA: 0x0039BE28 File Offset: 0x0039A028
		public void OnClickDowngrade(int charId, sbyte commandId)
		{
			bool flag = charId < 0;
			if (flag)
			{
				charId = this._selectedCharId;
			}
			sbyte downgradeId = CommonUtils.GetNormalTeammateCommand(commandId);
			this._operatingCharAndCommand = new ValueTuple<int, sbyte, sbyte, bool>(charId, commandId, downgradeId, false);
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.LK_UpgradeTeammateCommand_Revoke_Tip_Title.Tr(),
				Content = LanguageKey.LK_UpgradeTeammateCommand_Revoke_Confirm_Content.Tr(),
				Type = 1,
				Yes = new Action(this.OnConfirmDowngrade),
				No = null
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007C3A RID: 31802 RVA: 0x0039BECC File Offset: 0x0039A0CC
		public void OnCharacterSelect(int charId)
		{
			this.SetCharacterCellSelected(false);
			this._selectedCharId = charId;
			this.SetCharacterCellSelected(true);
			this.btnChange.interactable = (this._selectedCharId >= 0);
			this._isNeedSortCommandList = true;
			this.UpdateCommandList();
			bool flag = this._currentCommandList.Count == 0;
			if (flag)
			{
				this.OnCommandSelect(-1);
			}
			else
			{
				this.OnCommandSelect(this._currentCommandList[0]);
			}
		}

		// Token: 0x06007C3B RID: 31803 RVA: 0x0039BF46 File Offset: 0x0039A146
		public bool IsCommandAdvance(sbyte commandId)
		{
			return TeammateCommand.Instance[commandId].Type == ETeammateCommandType.Advance;
		}

		// Token: 0x06007C3C RID: 31804 RVA: 0x0039BF5C File Offset: 0x0039A15C
		public bool CanUpgrade(int charId, sbyte commandId)
		{
			GroupCharDisplayData data;
			bool flag = !this._groupCharDisplayDataDict.TryGetValue(charId, out data);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.IsCommandAdvance(commandId);
				if (flag2)
				{
					result = false;
				}
				else
				{
					List<sbyte> items = data.AdvancedCommand.Items;
					bool flag3 = ((items != null) ? items.Count : 0) >= this._maxUpgradeCount;
					result = (!flag3 && this.IsCostEnough(data, commandId));
				}
			}
			return result;
		}

		// Token: 0x06007C3D RID: 31805 RVA: 0x0039BFCC File Offset: 0x0039A1CC
		public List<GeneralLineData> GetUpgradeTips(int charId, sbyte commandId)
		{
			List<GeneralLineData> res = new List<GeneralLineData>();
			GroupCharDisplayData data;
			bool flag = !this._groupCharDisplayDataDict.TryGetValue(charId, out data);
			List<GeneralLineData> result;
			if (flag)
			{
				result = res;
			}
			else
			{
				TeammateCommandItem config = TeammateCommand.Instance[commandId];
				string medalText = CommonUtils.GetFeatureMedalTypeText((int)this.GetMedalType(commandId)) + " " + this.GetMedalCostString(data, commandId);
				string authorityText = LanguageKey.LK_Resource_Name_Authority.Tr() + " " + this.GetAuthorityCostString(commandId);
				string actionPointText = LanguageKey.LK_ActionPoint.Tr() + " " + this.GetActionPointCostString();
				res.Add(new GeneralLineData
				{
					Type = 3,
					Args = new List<string>
					{
						LanguageKey.LK_UpgradeTeammateCommand_Confirm_Content_Need.Tr()
					}
				});
				res.Add(new GeneralLineData
				{
					Type = 1,
					Args = new List<string>
					{
						LanguageKey.LK_UpgradeTeammateCommand_Upgrade_Tip_Cost.Tr()
					}
				});
				res.Add(new GeneralLineData
				{
					Type = 2,
					Args = new List<string>
					{
						"ui9_icon_resource_small_" + 7.ToString(),
						authorityText
					}
				});
				res.Add(new GeneralLineData
				{
					Type = 2,
					Args = new List<string>
					{
						"ui9_icon_event_action_point_0",
						actionPointText
					}
				});
				result = res;
			}
			return result;
		}

		// Token: 0x06007C3E RID: 31806 RVA: 0x0039C140 File Offset: 0x0039A340
		private void InitCharacter()
		{
			this.characterToggleGroup.Init(-1);
			this.characterToggleGroup.OnActiveIndexChange += this.OnCharacterToggleChange;
			this.characterScroll.InitPageCount();
			this.characterScroll.OnItemRender += this.OnCharacterRender;
			for (int i = 0; i <= 3; i++)
			{
				this._characterDict[i] = new List<int>();
			}
		}

		// Token: 0x06007C3F RID: 31807 RVA: 0x0039C1BC File Offset: 0x0039A3BC
		private void InitCommand()
		{
			this.commandToggleGroup.Init(-1);
			this.commandToggleGroup.OnActiveIndexChange += this.OnCommandToggleChange;
			this.btnChange.ClearAndAddListener(new Action(this.OnClickChangeCommand));
			this.commandScroll.InitPageCount();
			this.commandScroll.OnItemRender += this.OnCommandRender;
			foreach (TeammateCommandItem config in ((IEnumerable<TeammateCommandItem>)TeammateCommand.Instance))
			{
				bool flag = config.Type == ETeammateCommandType.Normal && config.MedalType != -1;
				if (flag)
				{
					this._normalCommandsInConfig.Add(config.TemplateId);
				}
			}
		}

		// Token: 0x06007C40 RID: 31808 RVA: 0x0039C294 File Offset: 0x0039A494
		private void InitDrum()
		{
			this.btnUpgrade.ClearAndAddListener(delegate
			{
				this.OnClickUpgrade(this._selectedCharId, this._selectedCommandId);
			});
			PointerTrigger trigger = this.btnUpgrade.GetComponent<PointerTrigger>();
			trigger.EnterEvent.AddListener(delegate()
			{
				this.hoverParticle.gameObject.SetActive(true);
				this.hoverParticle.Play();
				this.StartUpgradeSoundLoop();
			});
			trigger.ExitEvent.AddListener(delegate()
			{
				this.hoverParticle.gameObject.SetActive(false);
				this.hoverParticle.Stop();
				this.StopUpgradeSoundLoop();
			});
			this.btnDrum.ClearAndAddListener(delegate
			{
				this._drumClickCount++;
				bool flag = this._drumClickCount == 5;
				if (flag)
				{
					TaiwuEventDomainMethod.Call.TriggerShixiangDrumEasterEgg();
				}
				ViewShixiangUpgradeTeammateCommand.TurnDownMusicVolume(0.5f, 0f);
				ViewShixiangUpgradeTeammateCommand.RecoverMusicVolume(0.5f);
				this.drumParticle.gameObject.SetActive(true);
				this.drumParticle.Play();
				TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = this.drumScaleArea.DOScale(Vector3.one, 0.05f).SetEase(Ease.InOutQuad);
				tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
				{
					this.drumScaleArea.DOScale(Vector3.one * 1.03f, 0.1f).SetEase(Ease.InOutQuad);
				}));
			});
			this.btnDrum.GetComponent<PointerTrigger>().EnterEvent.AddListener(delegate()
			{
				this.drumScaleArea.DOScale(Vector3.one * 1.03f, 0.1f).SetEase(Ease.InOutQuad);
			});
			this.btnDrum.GetComponent<PointerTrigger>().ExitEvent.AddListener(delegate()
			{
				this.drumScaleArea.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutQuad);
			});
		}

		// Token: 0x06007C41 RID: 31809 RVA: 0x0039C354 File Offset: 0x0039A554
		private int SortData(sbyte commandIdA, sbyte commandIdB)
		{
			TeammateCommandItem configA = TeammateCommand.Instance[commandIdA];
			TeammateCommandItem configB = TeammateCommand.Instance[commandIdB];
			bool isAdvanceA = configA.Type == ETeammateCommandType.Advance;
			bool isAdvanceB = configB.Type == ETeammateCommandType.Advance;
			bool flag = isAdvanceA && isAdvanceB;
			int result;
			if (flag)
			{
				result = configB.MedalCount.CompareTo(configA.MedalCount);
			}
			else
			{
				bool flag2 = isAdvanceA;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = isAdvanceB;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06007C42 RID: 31810 RVA: 0x0039C3CC File Offset: 0x0039A5CC
		private void UpdateCharacterMedalDict()
		{
			this._groupCharDisplayDataDict.Clear();
			foreach (GroupCharDisplayData data in this._groupCharDisplayDataList)
			{
				this._groupCharDisplayDataDict[data.CharacterId] = data;
			}
			this._characterDict.Clear();
			for (int i = 0; i <= 3; i++)
			{
				int medalType = i - 1;
				List<sbyte> commandList = (i == 0) ? this._normalCommandsInConfig : this._normalCommandsInConfig.FindAll((sbyte commandTemplateId) => (int)this.GetMedalType(commandTemplateId) == medalType);
				this._characterDict[i] = new List<int>();
				this._characterDict[i].AddRange(this._charIdList.FindAll((int charId) => AgeGroup.GetAgeGroup(this._groupCharDisplayDataDict[charId].PhysiologicalAge) == 2 && commandList.Any((sbyte commandId) => this.GetMedalOwned(this._groupCharDisplayDataDict[charId], commandId) > 0)));
			}
		}

		// Token: 0x06007C43 RID: 31811 RVA: 0x0039C4DC File Offset: 0x0039A6DC
		private void UpdateCharacterList()
		{
			this.FilterCharacterIdList();
			this.characterScroll.SetDataCount(this._filterdCharIdList.Count);
			this.characterCountText.text = string.Format("{0}/{1}", this._filterdCharIdList.Count, this._charIdList.Count);
		}

		// Token: 0x06007C44 RID: 31812 RVA: 0x0039C53E File Offset: 0x0039A73E
		private void UpdateAdvanceCommandCount()
		{
			OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 6, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
			{
				bool isOpen = false;
				Serializer.Deserialize(dataPool, offset, ref isOpen);
				this._maxUpgradeCount = (isOpen ? 2 : 1);
				string color = (this.AdvanceCommandCount < this._maxUpgradeCount) ? "brightblue" : "brightred";
				this.commandCountText.text = string.Format("{0}/{1}", this.AdvanceCommandCount.ToString().SetColor(color), this._maxUpgradeCount);
			});
		}

		// Token: 0x06007C45 RID: 31813 RVA: 0x0039C558 File Offset: 0x0039A758
		private void UpdateCommandList()
		{
			GroupCharDisplayData data;
			bool flag = !this._groupCharDisplayDataDict.TryGetValue(this._selectedCharId, out data);
			if (flag)
			{
				if (this._currentCommandList == null)
				{
					this._currentCommandList = new List<sbyte>();
				}
				this._currentCommandList.Clear();
				this._cachedCommandList.Clear();
				this.CacheCommandList(this._currentCommandList);
				this.commandScroll.SetDataCount(this._currentCommandList.Count);
			}
			else
			{
				this.FilterCommandListForCharacter(this._selectedCharId);
				int typeIndex = this.commandToggleGroup.GetActiveIndex();
				List<sbyte> newList = (typeIndex == 0) ? this._availableCommandList : this._availableCommandsDict[typeIndex - 1];
				bool isNeedSortCommandList = this._isNeedSortCommandList;
				if (isNeedSortCommandList)
				{
					this._currentCommandList = newList;
					this._currentCommandList.Sort(new Comparison<sbyte>(this.SortData));
					this.CacheCommandList(this._currentCommandList);
					this._isNeedSortCommandList = false;
				}
				else
				{
					bool flag2 = this._changeId >= 0;
					if (flag2)
					{
						bool flag3 = ViewShixiangUpgradeTeammateCommand.IsCommandListEqualIgnoreOrder(this._cachedCommandList, newList);
						if (flag3)
						{
							this._currentCommandList = this._cachedCommandList;
						}
						else
						{
							this.CacheCommandList(this._currentCommandList);
							this._currentCommandList = newList;
						}
						this._changeId = -1;
					}
					else
					{
						bool flag4 = ViewShixiangUpgradeTeammateCommand.IsCommandListEqualIgnoreOrder(this._cachedCommandList, newList);
						if (flag4)
						{
							this._currentCommandList = this._cachedCommandList;
						}
						else
						{
							this._currentCommandList = newList;
							this.CacheCommandList(this._currentCommandList);
						}
					}
				}
				this.commandScroll.SetDataCount(this._currentCommandList.Count);
			}
		}

		// Token: 0x06007C46 RID: 31814 RVA: 0x0039C6F4 File Offset: 0x0039A8F4
		private void UpdateUpgradeButton()
		{
			bool canUse = this._selectedCommandId >= 0 && this.CanUpgrade(this._selectedCharId, this._selectedCommandId);
			this.btnUpgrade.interactable = canUse;
			this.btnUpgrade.GetComponent<PointerTrigger>().enabled = canUse;
		}

		// Token: 0x06007C47 RID: 31815 RVA: 0x0039C740 File Offset: 0x0039A940
		private void UpdateCommandDesc()
		{
			bool flag = this._selectedCommandId < 0;
			if (flag)
			{
				this.commandDesc.gameObject.SetActive(false);
				this.commandEffects.gameObject.SetActive(false);
			}
			else
			{
				sbyte advanceCommandId = CommonUtils.GetAdvanceTeammateCommand(this._selectedCommandId);
				sbyte normalCommandId = CommonUtils.GetNormalTeammateCommand(this._selectedCommandId);
				TeammateCommandItem advanceConfig = TeammateCommand.Instance[advanceCommandId];
				TeammateCommandItem normalConfig = TeammateCommand.Instance[normalCommandId];
				string descString = advanceConfig.Description;
				int startIndex = descString.IndexOf('<');
				string text = descString;
				int num = startIndex;
				string subDesc = text.Substring(num, text.Length - num);
				bool isUpgrade = this._selectedCommandId == advanceCommandId;
				for (int i = 0; i < this.commandEffects.childCount; i++)
				{
					bool flag2 = i >= advanceConfig.EffectDisplayTextList.Length;
					if (flag2)
					{
						this.commandEffects.GetChild(i).gameObject.SetActive(false);
					}
					else
					{
						ShixiangUpgradeTeammateCommandProperty obj = this.commandEffects.GetChild(i).GetComponent<ShixiangUpgradeTeammateCommandProperty>();
						obj.gameObject.SetActive(true);
						obj.Set(advanceConfig, normalConfig, i, isUpgrade);
						bool needPlayUpgradeParticle = this._needPlayUpgradeParticle;
						if (needPlayUpgradeParticle)
						{
							obj.PlayParticle();
						}
					}
				}
				this._needPlayUpgradeParticle = false;
				this.commandDesc.text = subDesc.ColorReplace();
				this.commandDesc.gameObject.SetActive(true);
				this.commandEffects.gameObject.SetActive(true);
			}
		}

		// Token: 0x06007C48 RID: 31816 RVA: 0x0039C8D0 File Offset: 0x0039AAD0
		private void UpdateCommandCost()
		{
			bool flag = this._selectedCommandId < 0 || this.IsCommandAdvance(this._selectedCommandId);
			if (flag)
			{
				this.authorityCost.SetActive(false);
				this.actionPointCost.SetActive(false);
			}
			else
			{
				this.authorityCostText.text = this.GetAuthorityCostString(this._selectedCommandId);
				this.actionPointCostText.text = this.GetActionPointCostString();
				this.authorityCost.SetActive(true);
				this.actionPointCost.SetActive(true);
			}
		}

		// Token: 0x06007C49 RID: 31817 RVA: 0x0039C95B File Offset: 0x0039AB5B
		private void OnCharacterToggleChange(int _, int __)
		{
			this._isNeedSortCommandList = true;
			this.UpdateCharacterList();
		}

		// Token: 0x06007C4A RID: 31818 RVA: 0x0039C96C File Offset: 0x0039AB6C
		private void OnCommandToggleChange(int _, int __)
		{
			this._isNeedSortCommandList = true;
			this.UpdateCommandList();
		}

		// Token: 0x06007C4B RID: 31819 RVA: 0x0039C980 File Offset: 0x0039AB80
		private void OnCharacterRender(int index, GameObject obj)
		{
			ShixiangUpgradeTeammateCommandCharacter item = obj.GetComponent<ShixiangUpgradeTeammateCommandCharacter>();
			int charId = this._filterdCharIdList[index];
			item.Set(this, this._groupCharDisplayDataDict[charId]);
			item.SetSelected(charId == this._selectedCharId);
		}

		// Token: 0x06007C4C RID: 31820 RVA: 0x0039C9C8 File Offset: 0x0039ABC8
		private void OnCommandRender(int index, GameObject obj)
		{
			ShixiangUpgradeTeammateCommandLongItem item = obj.GetComponent<ShixiangUpgradeTeammateCommandLongItem>();
			sbyte commandId = this._currentCommandList[index];
			bool isEquipped = this.IsCharacterEquippingCommand(this._selectedCharId, commandId) || this.IsCharacterEquippingCommand(this._selectedCharId, CommonUtils.GetAdvanceTeammateCommand(commandId)) || this.IsCharacterEquippingCommand(this._selectedCharId, CommonUtils.GetNormalTeammateCommand(commandId));
			item.Set(this, commandId, isEquipped, this.CanUpgrade(this._selectedCharId, commandId));
			item.SetSelected(this, commandId == this._selectedCommandId);
		}

		// Token: 0x06007C4D RID: 31821 RVA: 0x0039CA4B File Offset: 0x0039AC4B
		public void OnCommandSelect(sbyte commandId)
		{
			this.SetCommandCellSelected(false);
			this._selectedCommandId = commandId;
			this.SetCommandCellSelected(true);
			this.UpdateAdvanceCommandCount();
			this.UpdateCommandDesc();
			this.UpdateUpgradeButton();
			this.UpdateCommandCost();
		}

		// Token: 0x06007C4E RID: 31822 RVA: 0x0039CA84 File Offset: 0x0039AC84
		private void OnFinishUpgradeOrDownGradeCommand(int offset, RawDataPool pool)
		{
			bool success = false;
			Serializer.Deserialize(pool, offset, ref success);
			bool flag = success;
			if (flag)
			{
				int charId = this._operatingCharAndCommand.Item1;
				sbyte commandId = this._operatingCharAndCommand.Item2;
				sbyte operatedId = this._operatingCharAndCommand.Item3;
				bool isUpgrade = this._operatingCharAndCommand.Item4;
				this._operatingCharAndCommand = new ValueTuple<int, sbyte, sbyte, bool>(-1, -1, -1, false);
				bool flag2 = charId < 0 || commandId < 0;
				if (!flag2)
				{
					bool flag3 = isUpgrade && !this._changedCharIdList.Items.Contains(charId);
					if (flag3)
					{
						this._changedCharIdList.Items.Add(charId);
					}
					this.UpdateCharacterData(charId, commandId, operatedId);
					this.UpdateUpgradeButton();
					this.UpdateCharacterList();
					this.UpdateAdvanceCommandCount();
					bool flag4 = isUpgrade && charId == this._selectedCharId;
					if (flag4)
					{
						this.upgradeParticle.gameObject.SetActive(true);
						this.upgradeParticle.Play();
						for (int index = 0; index < this._currentCommandList.Count; index++)
						{
							bool flag5 = this._currentCommandList[index] == commandId;
							if (flag5)
							{
								GameObject cell = this.commandScroll.GetActiveCell(index);
								bool flag6 = cell != null;
								if (flag6)
								{
									cell.GetComponent<ShixiangUpgradeTeammateCommandLongItem>().PlayParticle();
								}
							}
						}
						this._needPlayUpgradeParticle = true;
					}
					bool flag7 = isUpgrade;
					if (flag7)
					{
						WorldDomainMethod.Call.AdvanceDaysInMonth(5);
					}
				}
			}
		}

		// Token: 0x06007C4F RID: 31823 RVA: 0x0039CC08 File Offset: 0x0039AE08
		private void OnClickChangeCommand()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this._selectedCharId);
			UIElement changeTeammateCommand = UIElement.ChangeTeammateCommand;
			changeTeammateCommand.OnHide = (Action)Delegate.Combine(changeTeammateCommand.OnHide, new Action(this.UpdateCharacterData));
			UIElement.ChangeTeammateCommand.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.ChangeTeammateCommand);
		}

		// Token: 0x06007C50 RID: 31824 RVA: 0x0039CC70 File Offset: 0x0039AE70
		private void OnConfirmDowngrade()
		{
			this.ChangeCacheCommandList(this._operatingCharAndCommand.Item3, this._operatingCharAndCommand.Item2);
			this._changeId = (int)this._operatingCharAndCommand.Item2;
			ExtraDomainMethod.AsyncCall.CancelAdvancedTeammateCommands(null, this._operatingCharAndCommand.Item1, this._operatingCharAndCommand.Item2, new AsyncMethodCallbackDelegate(this.OnFinishUpgradeOrDownGradeCommand));
		}

		// Token: 0x06007C51 RID: 31825 RVA: 0x0039CCD5 File Offset: 0x0039AED5
		private void CacheCommandList(List<sbyte> list)
		{
			this._cachedCommandList.Clear();
			this._cachedCommandList.AddRange(list);
		}

		// Token: 0x06007C52 RID: 31826 RVA: 0x0039CCF4 File Offset: 0x0039AEF4
		private void ChangeCacheCommandList(sbyte newId, sbyte oldId)
		{
			for (int i = 0; i < this._cachedCommandList.Count; i++)
			{
				sbyte commandId = this._cachedCommandList[i];
				bool flag = commandId == oldId;
				if (flag)
				{
					this._cachedCommandList[i] = newId;
					break;
				}
			}
		}

		// Token: 0x06007C53 RID: 31827 RVA: 0x0039CD44 File Offset: 0x0039AF44
		private static bool IsCommandListEqualIgnoreOrder(List<sbyte> a, List<sbyte> b)
		{
			bool flag = a.Count != b.Count;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Dictionary<sbyte, int> counts = new Dictionary<sbyte, int>(a.Count);
				foreach (sbyte id in a)
				{
					int c;
					counts.TryGetValue(id, out c);
					counts[id] = c + 1;
				}
				foreach (sbyte id2 in b)
				{
					int c2;
					bool flag2 = !counts.TryGetValue(id2, out c2) || c2 == 0;
					if (flag2)
					{
						return false;
					}
					counts[id2] = c2 - 1;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06007C54 RID: 31828 RVA: 0x0039CE40 File Offset: 0x0039B040
		private void StartUpgradeSoundLoop()
		{
			ViewShixiangUpgradeTeammateCommand.TurnDownMusicVolume(0.2f, 0.5f);
			AudioCommand cmd = new AudioCommand
			{
				AudioType = SEType.Sound,
				Loop = true,
				AudioName = "SFX_UpgradeTeammateCommand_inspire_loop",
				OnPlayUpdate = new Action<AudioCommandOnPlayeUpdateParam>(this.UpdateUpgradeSoundVolumn)
			};
			this._upgradeButtonSoundVolume = 1f;
			this.TryStopUpgradeButtonSoundFadeOut();
			AudioManager.Instance.StopSound("SFX_UpgradeTeammateCommand_inspire_loop");
			AudioManager.Instance.Play(cmd);
		}

		// Token: 0x06007C55 RID: 31829 RVA: 0x0039CEC0 File Offset: 0x0039B0C0
		private void TryStopUpgradeButtonSoundFadeOut()
		{
			bool flag = this._fadeOutUpgradeButtonSoundCoroutine != null && base.gameObject.activeSelf;
			if (flag)
			{
				base.StopCoroutine(this._fadeOutUpgradeButtonSoundCoroutine);
			}
		}

		// Token: 0x06007C56 RID: 31830 RVA: 0x0039CEF8 File Offset: 0x0039B0F8
		private static void TurnDownMusicVolume(float volume, float duration)
		{
			AudioManager.Instance.EnableMusicVolumeRate(volume);
			bool flag = duration < 0.01f;
			if (flag)
			{
				AudioManager.Instance.SetMusicVolume(volume, true);
			}
			else
			{
				AudioManager.Instance.SetMusicVolumeWithFade(duration, 0f);
			}
		}

		// Token: 0x06007C57 RID: 31831 RVA: 0x0039CF3E File Offset: 0x0039B13E
		private void StopUpgradeSoundLoop()
		{
			ViewShixiangUpgradeTeammateCommand.RecoverMusicVolume(0.5f);
			this.FadeOutUpgradeButtonSound();
		}

		// Token: 0x06007C58 RID: 31832 RVA: 0x0039CF53 File Offset: 0x0039B153
		private static void RecoverMusicVolume(float duration)
		{
			AudioManager.Instance.DisableMusicVolumeRate();
			AudioManager.Instance.SetMusicVolumeWithFade(duration, 0f);
		}

		// Token: 0x06007C59 RID: 31833 RVA: 0x0039CF74 File Offset: 0x0039B174
		private void FadeOutUpgradeButtonSound()
		{
			this.TryStopUpgradeButtonSoundFadeOut();
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				this._fadeOutUpgradeButtonSoundCoroutine = base.StartCoroutine(this.FadeOutUpgradeButtonSoundCo());
			}
			else
			{
				AudioManager.Instance.StopSound("SFX_UpgradeTeammateCommand_inspire_loop");
			}
		}

		// Token: 0x06007C5A RID: 31834 RVA: 0x0039CFBC File Offset: 0x0039B1BC
		private IEnumerator FadeOutUpgradeButtonSoundCo()
		{
			this._upgradeButtonSoundVolume = 1f;
			float elapsedTime = 0f;
			while (elapsedTime < 0.1f)
			{
				this._upgradeButtonSoundVolume = Mathf.Lerp(1f, 0f, elapsedTime / 0.1f);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			this._upgradeButtonSoundVolume = 0f;
			AudioManager.Instance.StopSound("SFX_UpgradeTeammateCommand_inspire_loop");
			yield break;
		}

		// Token: 0x06007C5B RID: 31835 RVA: 0x0039CFCB File Offset: 0x0039B1CB
		private void UpdateUpgradeSoundVolumn(AudioCommandOnPlayeUpdateParam param)
		{
			param.player.volume = this._upgradeButtonSoundVolume;
		}

		// Token: 0x06007C5C RID: 31836 RVA: 0x0039CFE0 File Offset: 0x0039B1E0
		private void FilterCharacterIdList()
		{
			this._characterDict.TryGetValue(this.characterToggleGroup.GetActiveIndex(), out this._filterdCharIdList);
		}

		// Token: 0x06007C5D RID: 31837 RVA: 0x0039D000 File Offset: 0x0039B200
		private void FilterCommandListForCharacter(int characterId)
		{
			this._availableCommandList.Clear();
			CommonUtils.MergeTeammateCommandList(this._normalCommandsInConfig, this._groupCharDisplayDataDict[characterId].AdvancedCommand.Items, this._availableCommandList);
			this._availableCommandList.Sort(new Comparison<sbyte>(this.CompareCommands));
			this._availableCommandsDict.Clear();
			for (int i = 0; i < 3; i++)
			{
				this._availableCommandsDict.Add(i, new List<sbyte>());
			}
			foreach (sbyte commandId in this._availableCommandList)
			{
				this._availableCommandsDict[(int)this.GetMedalType(commandId)].Add(commandId);
			}
		}

		// Token: 0x06007C5E RID: 31838 RVA: 0x0039D0E4 File Offset: 0x0039B2E4
		private GameObject GetSelectedCommandCell()
		{
			for (int index = 0; index < this._currentCommandList.Count; index++)
			{
				bool flag = this._currentCommandList[index] == this._selectedCommandId;
				if (flag)
				{
					return this.commandScroll.GetActiveCell(index);
				}
			}
			return null;
		}

		// Token: 0x06007C5F RID: 31839 RVA: 0x0039D138 File Offset: 0x0039B338
		private sbyte GetMedalType(sbyte commandTemplateId)
		{
			return TeammateCommand.Instance[commandTemplateId].MedalType;
		}

		// Token: 0x06007C60 RID: 31840 RVA: 0x0039D14C File Offset: 0x0039B34C
		private int GetMedalOwned(GroupCharDisplayData data, sbyte commandId)
		{
			sbyte medalType = this.GetMedalType(commandId);
			if (!true)
			{
			}
			int value;
			switch (medalType)
			{
			case 0:
				value = data.AttackMedal;
				break;
			case 1:
				value = data.DefenceMedal;
				break;
			case 2:
				value = data.WisdomMedal;
				break;
			default:
				throw new Exception("invalid medal type");
			}
			if (!true)
			{
			}
			return Math.Abs(value);
		}

		// Token: 0x06007C61 RID: 31841 RVA: 0x0039D1A9 File Offset: 0x0039B3A9
		private bool GetAuthorityEnough(sbyte commandId)
		{
			return this.Authority >= TeammateCommand.Instance[commandId].UpgradeAuthorityCost;
		}

		// Token: 0x06007C62 RID: 31842 RVA: 0x0039D1C6 File Offset: 0x0039B3C6
		private bool GetActionPointEnough()
		{
			return this.Days >= 5;
		}

		// Token: 0x06007C63 RID: 31843 RVA: 0x0039D1D4 File Offset: 0x0039B3D4
		private string GetCostString(int own, int require)
		{
			return string.Format("{0}/{1}", own.ToString().SetColor((own >= require) ? "brightblue" : "brightred"), require);
		}

		// Token: 0x06007C64 RID: 31844 RVA: 0x0039D202 File Offset: 0x0039B402
		private string GetMedalCostString(GroupCharDisplayData data, sbyte commandId)
		{
			return this.GetCostString(this.GetMedalOwned(data, commandId), (int)TeammateCommand.Instance[commandId].MedalCount);
		}

		// Token: 0x06007C65 RID: 31845 RVA: 0x0039D222 File Offset: 0x0039B422
		private string GetAuthorityCostString(sbyte commandId)
		{
			return this.GetCostString(this.Authority, TeammateCommand.Instance[commandId].UpgradeAuthorityCost);
		}

		// Token: 0x06007C66 RID: 31846 RVA: 0x0039D240 File Offset: 0x0039B440
		private string GetActionPointCostString()
		{
			return this.GetCostString(this.Days, 5);
		}

		// Token: 0x06007C67 RID: 31847 RVA: 0x0039D250 File Offset: 0x0039B450
		private bool IsCharacterEquippingCommand(int charId, sbyte commandId)
		{
			foreach (GroupCharDisplayData data in this._groupCharDisplayDataList)
			{
				bool flag = data.CharacterId == charId;
				if (flag)
				{
					List<sbyte> items = data.Command.Items;
					return items != null && items.Contains(commandId);
				}
			}
			return false;
		}

		// Token: 0x06007C68 RID: 31848 RVA: 0x0039D2CC File Offset: 0x0039B4CC
		private void UpdateCharacterData()
		{
			this.UpdateCharacterData(this._selectedCharId, -1, -1);
		}

		// Token: 0x06007C69 RID: 31849 RVA: 0x0039D2E0 File Offset: 0x0039B4E0
		private void UpdateCharacterData(int charId, sbyte commandId, sbyte operatedCommandId)
		{
			CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(this, new List<int>
			{
				charId
			}, delegate(int offset2, RawDataPool pool2)
			{
				List<GroupCharDisplayData> singleList = new List<GroupCharDisplayData>();
				Serializer.Deserialize(pool2, offset2, ref singleList);
				this._groupCharDisplayDataDict[charId] = singleList[0];
				this.UpdateCharacterList();
				this.UpdateCommandList();
				bool flag = charId == this._selectedCharId && commandId == this._selectedCommandId;
				if (flag)
				{
					for (int index = 0; index < this._currentCommandList.Count; index++)
					{
						bool flag2 = this._currentCommandList[index] == this._selectedCommandId;
						if (flag2)
						{
							GameObject cell = this.commandScroll.GetActiveCell(index);
							bool flag3 = cell != null;
							if (flag3)
							{
								this.OnCommandRender(index, cell);
							}
							break;
						}
					}
					this.OnCommandSelect(operatedCommandId);
					bool flag4 = operatedCommandId >= 0;
					if (flag4)
					{
						int index2 = this._currentCommandList.IndexOf(operatedCommandId);
						this.commandScroll.ScrollTo(index2, 0.3f);
					}
				}
			});
		}

		// Token: 0x06007C6A RID: 31850 RVA: 0x0039D338 File Offset: 0x0039B538
		private void SetCommandCellSelected(bool value)
		{
			GameObject cell = this.GetSelectedCommandCell();
			bool flag = cell != null;
			if (flag)
			{
				cell.GetComponent<ShixiangUpgradeTeammateCommandLongItem>().SetSelected(this, value);
			}
		}

		// Token: 0x06007C6B RID: 31851 RVA: 0x0039D368 File Offset: 0x0039B568
		private void SetCharacterCellSelected(bool value)
		{
			for (int index = 0; index < this._filterdCharIdList.Count; index++)
			{
				bool flag = this._filterdCharIdList[index] == this._selectedCharId;
				if (flag)
				{
					GameObject cell = this.characterScroll.GetActiveCell(index);
					bool flag2 = cell != null;
					if (flag2)
					{
						cell.GetComponent<ShixiangUpgradeTeammateCommandCharacter>().SetSelected(value);
					}
					break;
				}
			}
		}

		// Token: 0x06007C6C RID: 31852 RVA: 0x0039D3D4 File Offset: 0x0039B5D4
		private int CompareCommands(sbyte x, sbyte y)
		{
			bool xOwned = this.IsCommandOwned(this._selectedCharId, x);
			int ownedCompare = this.IsCommandOwned(this._selectedCharId, y).CompareTo(xOwned);
			bool flag = ownedCompare != 0;
			int result;
			if (flag)
			{
				result = ownedCompare;
			}
			else
			{
				GroupCharDisplayData groupDisplayData = this._groupCharDisplayDataDict[this._selectedCharId];
				List<sbyte> items = groupDisplayData.Command.Items;
				int xOwnedIndex = (items != null) ? items.IndexOf(x) : -1;
				List<sbyte> items2 = groupDisplayData.Command.Items;
				int yOwnedIndex = (items2 != null) ? items2.IndexOf(y) : -1;
				bool flag2 = xOwnedIndex != -1 && yOwnedIndex != -1;
				if (flag2)
				{
					result = xOwnedIndex.CompareTo(yOwnedIndex);
				}
				else
				{
					bool xAvailable = this.IsCommandAvailable(this._selectedCharId, x);
					int availableCompare = this.IsCommandAvailable(this._selectedCharId, y).CompareTo(xAvailable);
					bool flag3 = availableCompare != 0;
					if (flag3)
					{
						result = availableCompare;
					}
					else
					{
						bool xCanUpgrade = this.CanUpgrade(this._selectedCharId, x);
						int canUpgradeCompare = this.CanUpgrade(this._selectedCharId, y).CompareTo(xCanUpgrade);
						bool flag4 = canUpgradeCompare != 0;
						if (flag4)
						{
							result = canUpgradeCompare;
						}
						else
						{
							TeammateCommandItem xConfig = TeammateCommand.Instance[x];
							TeammateCommandItem yConfig = TeammateCommand.Instance[y];
							bool flag5 = xConfig.Type != yConfig.Type;
							if (flag5)
							{
								result = ((xConfig.Type == ETeammateCommandType.Advance) ? -1 : 1);
							}
							else
							{
								result = x.CompareTo(y);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06007C6D RID: 31853 RVA: 0x0039D550 File Offset: 0x0039B750
		private bool IsCommandOwned(int characterId, sbyte commandId)
		{
			GroupCharDisplayData groupCharDisplayData;
			bool flag = !this._groupCharDisplayDataDict.TryGetValue(characterId, out groupCharDisplayData);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<sbyte> items = groupCharDisplayData.Command.Items;
				result = (items != null && items.Contains(commandId));
			}
			return result;
		}

		// Token: 0x06007C6E RID: 31854 RVA: 0x0039D594 File Offset: 0x0039B794
		private bool IsCommandAvailable(int characterId, sbyte commandId)
		{
			GroupCharDisplayData groupCharDisplayData;
			bool flag = !this._groupCharDisplayDataDict.TryGetValue(characterId, out groupCharDisplayData);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<sbyte> items = groupCharDisplayData.Command.Items;
				bool flag2 = items != null && items.Contains(commandId);
				result = (flag2 || this.GetMedalOwned(groupCharDisplayData, commandId) >= (int)TeammateCommand.Instance[commandId].MedalCount);
			}
			return result;
		}

		// Token: 0x06007C6F RID: 31855 RVA: 0x0039D5FC File Offset: 0x0039B7FC
		private bool IsCostEnough(GroupCharDisplayData data, sbyte commandId)
		{
			return this.GetAuthorityEnough(commandId) && this.GetActionPointEnough();
		}

		// Token: 0x04005E6D RID: 24173
		[SerializeField]
		private CToggleGroup characterToggleGroup;

		// Token: 0x04005E6E RID: 24174
		[SerializeField]
		private TextMeshProUGUI characterCountText;

		// Token: 0x04005E6F RID: 24175
		[SerializeField]
		private InfinityScroll characterScroll;

		// Token: 0x04005E70 RID: 24176
		[SerializeField]
		private CToggleGroup commandToggleGroup;

		// Token: 0x04005E71 RID: 24177
		[SerializeField]
		private TextMeshProUGUI commandCountText;

		// Token: 0x04005E72 RID: 24178
		[SerializeField]
		private CButton btnChange;

		// Token: 0x04005E73 RID: 24179
		[SerializeField]
		private InfinityScroll commandScroll;

		// Token: 0x04005E74 RID: 24180
		[SerializeField]
		private CButton btnUpgrade;

		// Token: 0x04005E75 RID: 24181
		[SerializeField]
		private CButton btnDrum;

		// Token: 0x04005E76 RID: 24182
		[SerializeField]
		private TextMeshProUGUI commandDesc;

		// Token: 0x04005E77 RID: 24183
		[SerializeField]
		private Transform commandEffects;

		// Token: 0x04005E78 RID: 24184
		[SerializeField]
		private TextMeshProUGUI authorityCostText;

		// Token: 0x04005E79 RID: 24185
		[SerializeField]
		private TextMeshProUGUI actionPointCostText;

		// Token: 0x04005E7A RID: 24186
		[SerializeField]
		private GameObject authorityCost;

		// Token: 0x04005E7B RID: 24187
		[SerializeField]
		private GameObject actionPointCost;

		// Token: 0x04005E7C RID: 24188
		[SerializeField]
		private RectTransform drumScaleArea;

		// Token: 0x04005E7D RID: 24189
		[SerializeField]
		private ParticleSystem upgradeParticle;

		// Token: 0x04005E7E RID: 24190
		[SerializeField]
		private ParticleSystem hoverParticle;

		// Token: 0x04005E7F RID: 24191
		[SerializeField]
		private ParticleSystem drumParticle;

		// Token: 0x04005E80 RID: 24192
		public const int NeedTime = 5;

		// Token: 0x04005E81 RID: 24193
		private const int MedalTypeCount = 3;

		// Token: 0x04005E82 RID: 24194
		private int _selectedCharId = -1;

		// Token: 0x04005E83 RID: 24195
		private readonly List<int> _charIdList = new List<int>();

		// Token: 0x04005E84 RID: 24196
		private List<int> _filterdCharIdList = new List<int>();

		// Token: 0x04005E85 RID: 24197
		private List<GroupCharDisplayData> _groupCharDisplayDataList = new List<GroupCharDisplayData>();

		// Token: 0x04005E86 RID: 24198
		private readonly Dictionary<int, GroupCharDisplayData> _groupCharDisplayDataDict = new Dictionary<int, GroupCharDisplayData>();

		// Token: 0x04005E87 RID: 24199
		private readonly Dictionary<int, List<int>> _characterDict = new Dictionary<int, List<int>>();

		// Token: 0x04005E88 RID: 24200
		private readonly List<sbyte> _normalCommandsInConfig = new List<sbyte>();

		// Token: 0x04005E89 RID: 24201
		private List<sbyte> _currentCommandList;

		// Token: 0x04005E8A RID: 24202
		private readonly List<sbyte> _cachedCommandList = new List<sbyte>();

		// Token: 0x04005E8B RID: 24203
		private readonly IntList _changedCharIdList = IntList.Create();

		// Token: 0x04005E8C RID: 24204
		private int _maxUpgradeCount = 1;

		// Token: 0x04005E8D RID: 24205
		private List<sbyte> _availableCommandList = new List<sbyte>();

		// Token: 0x04005E8E RID: 24206
		private readonly Dictionary<int, List<sbyte>> _availableCommandsDict = new Dictionary<int, List<sbyte>>();

		// Token: 0x04005E8F RID: 24207
		private sbyte _selectedCommandId;

		// Token: 0x04005E90 RID: 24208
		[TupleElementNames(new string[]
		{
			"charId",
			"commandId",
			"operatedId",
			"isUpgrade"
		})]
		private ValueTuple<int, sbyte, sbyte, bool> _operatingCharAndCommand;

		// Token: 0x04005E91 RID: 24209
		private int _drumClickCount;

		// Token: 0x04005E92 RID: 24210
		private bool _needPlayUpgradeParticle = false;

		// Token: 0x04005E93 RID: 24211
		private int _changeId;

		// Token: 0x04005E94 RID: 24212
		private bool _isNeedSortCommandList = false;

		// Token: 0x04005E95 RID: 24213
		private ResourceMonitor _resourceMonitor;

		// Token: 0x04005E96 RID: 24214
		private float _upgradeButtonSoundVolume = 1f;

		// Token: 0x04005E97 RID: 24215
		private Coroutine _fadeOutUpgradeButtonSoundCoroutine;

		// Token: 0x04005E98 RID: 24216
		private const float UpgradeButtonFadeOutDuration = 0.1f;
	}
}
