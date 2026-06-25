using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF5 RID: 3061
	public class BuildingManageSubPageNewRemove : BuildingManageSubPage
	{
		// Token: 0x1700106A RID: 4202
		// (get) Token: 0x06009B85 RID: 39813 RVA: 0x0048E268 File Offset: 0x0048C468
		private bool IsRemove
		{
			get
			{
				return this.operationType == BuildingManageSubPageNewRemove.SubPageType.Remove;
			}
		}

		// Token: 0x1700106B RID: 4203
		// (get) Token: 0x06009B86 RID: 39814 RVA: 0x0048E273 File Offset: 0x0048C473
		private BuildingBlockKey BlockKey
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x1700106C RID: 4204
		// (get) Token: 0x06009B87 RID: 39815 RVA: 0x0048E280 File Offset: 0x0048C480
		private BuildingBlockData BlockData
		{
			get
			{
				return this.ParentView.BlockData;
			}
		}

		// Token: 0x1700106D RID: 4205
		// (get) Token: 0x06009B88 RID: 39816 RVA: 0x0048E28D File Offset: 0x0048C48D
		private BuildingBlockItem ConfigData
		{
			get
			{
				return this.ParentView.ConfigData;
			}
		}

		// Token: 0x1700106E RID: 4206
		// (get) Token: 0x06009B89 RID: 39817 RVA: 0x0048E29A File Offset: 0x0048C49A
		private int MaxOperators
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700106F RID: 4207
		// (get) Token: 0x06009B8A RID: 39818 RVA: 0x0048E29D File Offset: 0x0048C49D
		private bool IsBuildingManagementUnlocked
		{
			get
			{
				return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			}
		}

		// Token: 0x06009B8B RID: 39819 RVA: 0x0048E2AC File Offset: 0x0048C4AC
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(UIElement.BuildingManage, false, false, false, true, false) && this.confirmBtn.interactable;
			if (flag)
			{
				this.confirmBtn.onClick.Invoke();
			}
		}

		// Token: 0x06009B8C RID: 39820 RVA: 0x0048E2F5 File Offset: 0x0048C4F5
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			this.resourceTemplate.SetActive(false);
		}

		// Token: 0x06009B8D RID: 39821 RVA: 0x0048E30D File Offset: 0x0048C50D
		private void OnEnable()
		{
			GEvent.Add(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.OnVillagerWorkDataChange));
		}

		// Token: 0x06009B8E RID: 39822 RVA: 0x0048E329 File Offset: 0x0048C529
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.OnVillagerWorkDataChange));
		}

		// Token: 0x06009B8F RID: 39823 RVA: 0x0048E345 File Offset: 0x0048C545
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this.LoadOperatorsFromModel();
			this.RefreshResourceArea();
			this.RefreshPersonArea();
			this.RefreshProgressArea();
		}

		// Token: 0x06009B90 RID: 39824 RVA: 0x0048E36C File Offset: 0x0048C56C
		private void OnVillagerWorkDataChange(ArgumentBox _)
		{
			bool flag = !base.gameObject.activeInHierarchy || this.ParentView == null || this.DisplayData == null;
			if (!flag)
			{
				this.RefreshPersonArea();
			}
		}

		// Token: 0x06009B91 RID: 39825 RVA: 0x0048E3B0 File Offset: 0x0048C5B0
		private void RefreshResourceArea()
		{
			this.resourceTitle.text = (this.IsRemove ? LanguageKey.LK_Building_RemoveGetResource.Tr() : LanguageKey.LK_Building_ExpandConsume.Tr());
			sbyte level = this.IsRemove ? base.Model.GetBuildingLevel(this.BlockKey, this.BlockData) : 0;
			int index = 0;
			sbyte resType = 0;
			while ((int)resType < this.ConfigData.BaseBuildCost.Length)
			{
				int amount = this.IsRemove ? GameData.Domains.Building.SharedMethods.GetResourceReturnOfRemoveBuilding(this.ConfigData, level, resType, this.BlockData) : ((int)this.ConfigData.BaseBuildCost[(int)resType]);
				bool flag = amount <= 0;
				if (!flag)
				{
					GameObject item = this.GetOrCreateResourceItem(index);
					item.SetActive(true);
					this.SetResourceItem(item, resType, amount);
					index++;
				}
				resType += 1;
			}
			for (int i = index; i < this._resourceItems.Count; i++)
			{
				this._resourceItems[i].SetActive(false);
			}
			bool isRemove = this.IsRemove;
			if (isRemove)
			{
				this.resourceTip.enabled = true;
				this.resourceTip.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = this.resourceTip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("arg0", LanguageKey.LK_Building_RemoveGetResourceTip.Tr());
			}
			else
			{
				this.resourceTip.enabled = false;
			}
		}

		// Token: 0x06009B92 RID: 39826 RVA: 0x0048E530 File Offset: 0x0048C730
		private GameObject GetOrCreateResourceItem(int index)
		{
			while (this._resourceItems.Count <= index)
			{
				GameObject go = Object.Instantiate<GameObject>(this.resourceTemplate, this.resourceRoot);
				this._resourceItems.Add(go);
			}
			return this._resourceItems[index];
		}

		// Token: 0x06009B93 RID: 39827 RVA: 0x0048E588 File Offset: 0x0048C788
		private void SetResourceItem(GameObject item, sbyte resourceType, int amount)
		{
			ResourceTypeItem config = Config.ResourceType.Instance[resourceType];
			item.transform.Find("Icon").GetComponent<CImage>().SetSprite(config.Icon, false, null);
			item.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = config.Name;
			TextMeshProUGUI valueText = item.transform.Find("Value").GetComponent<TextMeshProUGUI>();
			bool isRemove = this.IsRemove;
			if (isRemove)
			{
				valueText.text = amount.ToString();
			}
			else
			{
				int owned = base.Model.GetResourceCount(resourceType);
				string color = (owned >= amount) ? "brightblue" : "brightred";
				string ownStr = CommonUtils.GetDisplayStringForNum(owned, 100000).SetColor(color);
				valueText.text = string.Format("{0}/{1}", ownStr, amount);
			}
		}

		// Token: 0x06009B94 RID: 39828 RVA: 0x0048E668 File Offset: 0x0048C868
		private void RefreshPersonArea()
		{
			int maxOps = this.MaxOperators;
			int count = 0;
			for (int i = 0; i < maxOps; i++)
			{
				bool flag = this._operatorListCached[i] >= 0;
				if (flag)
				{
					count++;
				}
			}
			LanguageKey titleKey = this.IsRemove ? LanguageKey.LK_Building_RemoveManpower : LanguageKey.LK_Building_ArrangeManpower;
			this.personTitle.text = string.Format("{0}  ({1}/{2})", titleKey.Tr(), count, maxOps);
			this.fastAddPersonButton.interactable = this.IsBuildingManagementUnlocked;
			this.fastAddPersonButton.ClearAndAddListener(new Action(this.OnClickQuickAssign));
			bool hasOperator = count > 0;
			this.fastRemovePersonButton.interactable = (this.IsBuildingManagementUnlocked && hasOperator);
			this.fastRemovePersonButton.ClearAndAddListener(new Action(this.OnClickQuickClear));
			this.RefreshAllMemberViews();
			this.UpdateOperationLeftTime();
			this.RefreshProgressArea();
		}

		// Token: 0x06009B95 RID: 39829 RVA: 0x0048E75C File Offset: 0x0048C95C
		private void RefreshAllMemberViews()
		{
			int maxOps = this.MaxOperators;
			List<int> charIds = new List<int>();
			for (int i = 0; i < maxOps; i++)
			{
				bool flag = this._operatorListCached[i] >= 0;
				if (flag)
				{
					charIds.Add(this._operatorListCached[i]);
				}
			}
			bool flag2 = charIds.Count == 0;
			if (flag2)
			{
				for (int j = 0; j < this.memberViews.Length; j++)
				{
					this.memberViews[j].gameObject.SetActive(j < maxOps);
					bool flag3 = j < maxOps;
					if (flag3)
					{
						this.memberViews[j].SetForOperator(null, j, new Action<int>(this.OnSelectOperator), new Action<int>(this.OnCancelOperator), this.ParentView, false, null);
					}
				}
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this.ParentView, charIds, delegate(int offset, RawDataPool pool)
				{
					List<CharacterDisplayData> dataList = null;
					Serializer.Deserialize(pool, offset, ref dataList);
					Dictionary<int, CharacterDisplayData> dataDict = new Dictionary<int, CharacterDisplayData>();
					bool flag4 = dataList != null;
					if (flag4)
					{
						foreach (CharacterDisplayData data in dataList)
						{
							dataDict[data.CharacterId] = data;
						}
					}
					for (int k = 0; k < this.memberViews.Length; k++)
					{
						this.memberViews[k].gameObject.SetActive(k < maxOps);
						bool flag5 = k >= maxOps;
						if (!flag5)
						{
							int charId = this._operatorListCached[k];
							CharacterDisplayData d;
							CharacterDisplayData charData = (charId >= 0 && dataDict.TryGetValue(charId, out d)) ? d : null;
							List<int> unlockedWorkingVillagerList = this.DisplayData.UnlockedWorkingVillagerList;
							bool isUnlocked = unlockedWorkingVillagerList != null && unlockedWorkingVillagerList.Contains(charId);
							this.memberViews[k].SetForOperator(charData, k, new Action<int>(this.OnSelectOperator), new Action<int>(this.OnCancelOperator), this.ParentView, isUnlocked, new Action<int, bool>(this.SetUnlockedWorkingVillager));
						}
					}
				});
			}
		}

		// Token: 0x06009B96 RID: 39830 RVA: 0x0048E870 File Offset: 0x0048CA70
		private void SetUnlockedWorkingVillager(int charId, bool isUnlock)
		{
			BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, isUnlock);
		}

		// Token: 0x06009B97 RID: 39831 RVA: 0x0048E87C File Offset: 0x0048CA7C
		private void OnSelectOperator(int index)
		{
			this._selectingOperatorIndex = index;
			BuildingManageDisplayData displayData = this.DisplayData;
			List<int> availableWorker = (displayData != null) ? displayData.AvailableWorker : null;
			bool flag = availableWorker != null && availableWorker.Count > 0;
			if (flag)
			{
				this.OpenSelectCharPanel(availableWorker);
			}
			else
			{
				TaiwuDomainMethod.AsyncCall.GetAllVillagersAvailableForWork(this.ParentView, delegate(int offset, RawDataPool pool)
				{
					List<int> workerList = new List<int>();
					Serializer.Deserialize(pool, offset, ref workerList);
					this.OpenSelectCharPanel(workerList);
				});
			}
		}

		// Token: 0x06009B98 RID: 39832 RVA: 0x0048E8E0 File Offset: 0x0048CAE0
		private void OpenSelectCharPanel(List<int> workerList)
		{
			int currentCharId = this._operatorListCached[this._selectingOperatorIndex];
			HashSet<int> occupiedByOthers = new HashSet<int>();
			for (int i = 0; i < this.MaxOperators; i++)
			{
				bool flag = i == this._selectingOperatorIndex;
				if (!flag)
				{
					int occupiedId = this._operatorListCached[i];
					bool flag2 = occupiedId >= 0;
					if (flag2)
					{
						occupiedByOthers.Add(occupiedId);
					}
				}
			}
			List<int> filteredList = (from id in workerList
			where !occupiedByOthers.Contains(id)
			select id).ToList<int>();
			bool flag3 = currentCharId >= 0 && !filteredList.Contains(currentCharId);
			if (flag3)
			{
				filteredList.Add(currentCharId);
			}
			int curId = currentCharId;
			SelectCharacterCallback <>9__3;
			TaiwuDomainMethod.AsyncCall.GetVillagersForWorkDisplayData(this.ParentView, filteredList, delegate(int offset, RawDataPool pool)
			{
				List<VillagerSelectCharacterDisplayData> displayData = new List<VillagerSelectCharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new VillagerSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.Villager);
				config.InteractionMode = ESelectCharacterInteractionMode.Slot;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				CommonSelectCharacterConfig commonSelectCharacterConfig = config;
				object initialSelectedCharacterIds;
				if (curId < 0)
				{
					initialSelectedCharacterIds = null;
				}
				else
				{
					(initialSelectedCharacterIds = new List<int>()).Add(curId);
				}
				commonSelectCharacterConfig.InitialSelectedCharacterIds = initialSelectedCharacterIds;
				config.BannedCharacterIds = occupiedByOthers;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.WorkStatus,
					ESelectCharacterFilterMenuId.RoleArrangementWork,
					ESelectCharacterFilterMenuId.Identity
				};
				UIElement selectChar = UIElement.SelectChar;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList);
				string key = "SelectCharacterCallback";
				SelectCharacterCallback arg;
				if ((arg = <>9__3) == null)
				{
					arg = (<>9__3 = delegate(List<int> selectedIds)
					{
						int charId = (selectedIds != null && selectedIds.Count > 0) ? selectedIds[0] : -1;
						this.OnCharSelected(charId);
					});
				}
				selectChar.SetOnInitArgs(argumentBox.SetObject(key, arg));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06009B99 RID: 39833 RVA: 0x0048E9BC File Offset: 0x0048CBBC
		private void OnCharSelected(int charId)
		{
			int index = this._selectingOperatorIndex;
			bool flag = this._operatorListCached[index] == charId;
			if (!flag)
			{
				bool flag2 = this.BlockData.OperationType != -1;
				if (flag2)
				{
					this.TrySetOperatorForActiveOperation(index, charId);
				}
				this._operatorListCached[index] = charId;
				this.RefreshPersonArea();
			}
		}

		// Token: 0x06009B9A RID: 39834 RVA: 0x0048EA10 File Offset: 0x0048CC10
		private void OnCancelOperator(int index)
		{
			bool flag = this._operatorListCached[index] == -1;
			if (!flag)
			{
				bool flag2 = this.BlockData.OperationType != -1;
				if (flag2)
				{
					this.TrySetOperatorForActiveOperation(index, -1);
				}
				this._operatorListCached[index] = -1;
				this.RefreshPersonArea();
			}
		}

		// Token: 0x06009B9B RID: 39835 RVA: 0x0048EA5D File Offset: 0x0048CC5D
		private void OnClickQuickAssign()
		{
			BuildingDomainMethod.AsyncCall.QuickArrangeBuildOperator(this.ParentView, this.ConfigData.TemplateId, this.BlockKey, (sbyte)this.operationType, delegate(int offset, RawDataPool dataPool)
			{
				List<int> charIdList = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref charIdList);
				int maxOps = this.MaxOperators;
				int count = Mathf.Min(charIdList.Count, maxOps);
				for (int i = 0; i < count; i++)
				{
					int charId = charIdList[i];
					bool flag = this._operatorListCached[i] == charId;
					if (!flag)
					{
						this._operatorListCached[i] = charId;
						bool flag2 = this.BlockData.OperationType != -1;
						if (flag2)
						{
							this.TrySetOperatorForActiveOperation(i, charId);
						}
					}
				}
				for (int j = count; j < maxOps; j++)
				{
					bool flag3 = this._operatorListCached[j] == -1;
					if (!flag3)
					{
						this._operatorListCached[j] = -1;
						bool flag4 = this.BlockData.OperationType != -1;
						if (flag4)
						{
							this.TrySetOperatorForActiveOperation(j, -1);
						}
					}
				}
				this.RefreshPersonArea();
			});
		}

		// Token: 0x06009B9C RID: 39836 RVA: 0x0048EA90 File Offset: 0x0048CC90
		private void OnClickQuickClear()
		{
			for (int i = 0; i < this.MaxOperators; i++)
			{
				bool flag = this._operatorListCached[i] == -1;
				if (!flag)
				{
					this._operatorListCached[i] = -1;
					bool flag2 = this.BlockData.OperationType != -1;
					if (flag2)
					{
						this.TrySetOperatorForActiveOperation(i, -1);
					}
				}
			}
			this.RefreshPersonArea();
		}

		// Token: 0x06009B9D RID: 39837 RVA: 0x0048EAF8 File Offset: 0x0048CCF8
		private bool TrySetOperatorForActiveOperation(int index, int charId)
		{
			CharacterList operatorList;
			bool flag = base.Model.BuildingOperators.TryGetValue(this.BlockKey, out operatorList);
			if (flag)
			{
				List<int> collection = operatorList.GetCollection();
				bool flag2 = collection.CheckIndex(index) && collection[index] == charId;
				if (flag2)
				{
					return false;
				}
			}
			BuildingDomainMethod.Call.SetOperator(this.BlockKey, (sbyte)index, charId);
			return true;
		}

		// Token: 0x06009B9E RID: 39838 RVA: 0x0048EB64 File Offset: 0x0048CD64
		private void UpdateOperationLeftTime()
		{
			bool flag = this.BlockData.OperationType != -1 && this.BlockData.OperationStopping;
			if (flag)
			{
				this.needTimeLabel.text = LanguageKey.LK_Building_NewRemove_Cost.TrFormat("1");
			}
			else
			{
				BuildingDomainMethod.AsyncCall.GetOperationLeftTime(this.ParentView, this.ConfigData.TemplateId, this.BlockKey, (sbyte)this.operationType, this._operatorListCached.ToList<int>(), delegate(int offset, RawDataPool dataPool)
				{
					int leftTime = 0;
					Serializer.Deserialize(dataPool, offset, ref leftTime);
					this.needTimeLabel.text = ((leftTime > 0) ? LanguageKey.LK_Building_NewRemove_Cost.TrFormat(leftTime.ToString()) : "-");
				});
			}
		}

		// Token: 0x06009B9F RID: 39839 RVA: 0x0048EBEC File Offset: 0x0048CDEC
		private void LoadOperatorsFromModel()
		{
			for (int i = 0; i < this._operatorListCached.Length; i++)
			{
				this._operatorListCached[i] = -1;
			}
			bool flag = this.BlockData.OperationType == -1;
			if (!flag)
			{
				CharacterList charList;
				bool flag2 = base.Model.BuildingOperators.TryGetValue(this.BlockKey, out charList);
				if (flag2)
				{
					List<int> collection = charList.GetCollection();
					int j = 0;
					while (j < this._operatorListCached.Length && j < collection.Count)
					{
						this._operatorListCached[j] = collection[j];
						j++;
					}
				}
			}
		}

		// Token: 0x06009BA0 RID: 39840 RVA: 0x0048EC94 File Offset: 0x0048CE94
		private void RefreshProgressArea()
		{
			this.progressBg.SetActive(true);
			this.progressTitle.text = (this.IsRemove ? LanguageKey.LK_Building_RemoveProgress.Tr() : LanguageKey.LK_Building_BuildProgress.Tr());
			short progress = this.BlockData.OperationProgress;
			short totalProgress = this.ConfigData.OperationTotalProgress[(int)((sbyte)this.operationType)];
			bool hasOperator = this._operatorListCached.Any((int id) => id >= 0);
			int remaining = (int)(hasOperator ? (totalProgress - progress) : 0);
			this.progressFill.fillAmount = (float)progress / (float)totalProgress;
			this.progressIncrease.fillAmount = (float)((int)progress + remaining) / (float)totalProgress;
			this.progressCurrentText.text = string.Format("{0}", progress);
			this.progressIncreaseText.text = string.Format("+{0}", remaining);
			this.progressTotalText.text = string.Format("/{0}", totalProgress);
			this.progressIncreaseText.gameObject.SetActive(hasOperator);
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.progressText);
			this.UpdateButtons();
		}

		// Token: 0x06009BA1 RID: 39841 RVA: 0x0048EDCC File Offset: 0x0048CFCC
		private void UpdateButtons()
		{
			bool flag = this.IsRemove && this.BlockData.OperationType == -1;
			if (flag)
			{
				this.confirmBtn.gameObject.SetActive(true);
				this.cancelBtn.gameObject.SetActive(false);
				this.confirmBtnText.text = LanguageKey.LK_Building_Start_Demolish.Tr();
				bool hasOperator = this._operatorListCached.Any((int id) => id >= 0);
				this.confirmBtn.interactable = (hasOperator && this.IsBuildingManagementUnlocked);
				this.confirmBtn.ClearAndAddListener(new Action(this.OnClickStartRemove));
				TMP_Text child = this.confirmBtn.GetComponentInChildren<TMP_Text>();
				string color = this.confirmBtn.interactable ? "#c6c6c6" : "#858585";
				child.color = color.HexStringToColor();
				this.confirmBtnTip.enabled = true;
				this.confirmBtnTip.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = this.confirmBtnTip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				ArgumentBox argBox = argumentBox;
				argBox.Set("arg0", LanguageKey.LK_Building_Start_Remove.Tr());
				string tipContent = LanguageKey.LK_Building_Start_Remove_Tip_Desc.Tr();
				bool flag2 = !hasOperator;
				if (flag2)
				{
					tipContent = tipContent + "\n" + LanguageKey.LK_Cannot_Build_No_Operator.Tr();
				}
				argBox.Set("arg1", tipContent);
			}
			else
			{
				bool flag3 = !this.BlockData.OperationStopping;
				if (flag3)
				{
					this.confirmBtn.gameObject.SetActive(false);
					this.cancelBtn.gameObject.SetActive(true);
					this.cancelBtnText.text = (this.IsRemove ? LanguageKey.LK_Building_Stop_Demolish.Tr() : LanguageKey.LK_Building_Stop_Build.Tr());
					this.cancelBtn.ClearAndAddListener(new Action(this.OnClickStopOperation));
				}
				else
				{
					this.confirmBtn.gameObject.SetActive(true);
					this.cancelBtn.gameObject.SetActive(false);
					this.confirmBtnText.text = (this.IsRemove ? LanguageKey.LK_Building_Continue_Remove.Tr() : LanguageKey.LK_Building_Continue_Build.Tr());
					this.confirmBtn.interactable = true;
					this.confirmBtn.ClearAndAddListener(new Action(this.OnClickContinueOperation));
				}
			}
		}

		// Token: 0x06009BA2 RID: 39842 RVA: 0x0048F054 File Offset: 0x0048D254
		private void OnClickStartRemove()
		{
			AudioManager.Instance.PlaySound("ui_industry_dismantle", false, false);
			this.BlockData.OperationType = 1;
			BuildingDomainMethod.AsyncCall.Remove(this.ParentView, this.BlockKey, this._operatorListCached, delegate(int offset, RawDataPool pool)
			{
				ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
				Serializer.Deserialize(pool, offset, ref retValue);
				bool flag = retValue.Item1 == this.BlockKey.BuildingBlockIndex;
				if (flag)
				{
					UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this.BlockKey, retValue.Item2, true);
				}
				ViewBuildingArea buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
				buildingArea.MoveCameraCenterToBuilding(this.BlockKey.BuildingBlockIndex);
				this.ParentView.QuickHide();
			});
		}

		// Token: 0x06009BA3 RID: 39843 RVA: 0x0048F0A4 File Offset: 0x0048D2A4
		private void OnClickStopOperation()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			bool flag = !this.IsRemove && BuildingBlockData.IsUsefulResource(this.ConfigData.Type);
			if (flag)
			{
				ViewBuildingManage.ShowDialog(LanguageKey.LK_Building_Stop_Build.Tr(), LanguageKey.LK_Building_Stop_Build_Tip_Resource.Tr(), delegate
				{
					this.DoStopOperation();
				}, null);
			}
			else
			{
				this.DoStopOperation();
			}
		}

		// Token: 0x06009BA4 RID: 39844 RVA: 0x0048F118 File Offset: 0x0048D318
		private void DoStopOperation()
		{
			bool flag = !this.IsRemove && this.BlockData.OperationType == 0;
			if (flag)
			{
				List<ItemDisplayData> returnItems = this.BuildStopOperationReturnItemList();
				BuildingDomainMethod.Call.SetStopOperation(UIElement.BuildingArea.GameDataListenerId, this.BlockKey, true);
				this.ParentView.QuickHide();
				BuildingManageSubPageNewRemove.ShowBuildingResourceReturn(returnItems);
			}
			else
			{
				BuildingDomainMethod.AsyncCall.SetStopOperation(this.ParentView, this.BlockKey, true, delegate(int offset, RawDataPool pool)
				{
					ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
					Serializer.Deserialize(pool, offset, ref retValue);
					bool flag2 = retValue.Item1 == this.BlockKey.BuildingBlockIndex;
					if (flag2)
					{
						UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this.BlockKey, retValue.Item2, true);
					}
					this.ParentView.RequestData();
				});
			}
		}

		// Token: 0x06009BA5 RID: 39845 RVA: 0x0048F198 File Offset: 0x0048D398
		private List<ItemDisplayData> BuildStopOperationReturnItemList()
		{
			List<ItemDisplayData> result = new List<ItemDisplayData>();
			for (sbyte resourceType = 0; resourceType < 8; resourceType += 1)
			{
				ushort amount = this.ConfigData.BaseBuildCost[(int)resourceType];
				bool flag = amount <= 0;
				if (!flag)
				{
					short templateId = (short)resourceType;
					result.Add(new ItemDisplayData(12, templateId)
					{
						Amount = (int)amount
					});
				}
			}
			bool flag2 = this.ConfigData.BuildingCoreItem >= 0;
			if (flag2)
			{
				result.Add(new ItemDisplayData(12, this.ConfigData.BuildingCoreItem)
				{
					Amount = 1
				});
			}
			return result;
		}

		// Token: 0x06009BA6 RID: 39846 RVA: 0x0048F238 File Offset: 0x0048D438
		private static void ShowBuildingResourceReturn(List<ItemDisplayData> returnItems)
		{
			bool flag = returnItems == null || returnItems.Count == 0;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", returnItems);
				argBox.Set("ObtainType", 21);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
		}

		// Token: 0x06009BA7 RID: 39847 RVA: 0x0048F298 File Offset: 0x0048D498
		private void OnClickContinueOperation()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			BuildingDomainMethod.AsyncCall.SetStopOperation(this.ParentView, this.BlockKey, false, delegate(int offset, RawDataPool pool)
			{
				this.ParentView.RequestData();
			});
		}

		// Token: 0x04007887 RID: 30855
		[Tooltip("操作类型：新建或拆除")]
		[SerializeField]
		private BuildingManageSubPageNewRemove.SubPageType operationType;

		// Token: 0x04007888 RID: 30856
		[Header("区域一：资源")]
		[SerializeField]
		private TextMeshProUGUI resourceTitle;

		// Token: 0x04007889 RID: 30857
		[SerializeField]
		private Transform resourceRoot;

		// Token: 0x0400788A RID: 30858
		[SerializeField]
		private GameObject resourceTemplate;

		// Token: 0x0400788B RID: 30859
		[SerializeField]
		private TooltipInvoker resourceTip;

		// Token: 0x0400788C RID: 30860
		[Header("区域二：人手")]
		[SerializeField]
		private TextMeshProUGUI personTitle;

		// Token: 0x0400788D RID: 30861
		[SerializeField]
		private CButton fastAddPersonButton;

		// Token: 0x0400788E RID: 30862
		[SerializeField]
		private CButton fastRemovePersonButton;

		// Token: 0x0400788F RID: 30863
		[SerializeField]
		private TextMeshProUGUI needTimeLabel;

		// Token: 0x04007890 RID: 30864
		[SerializeField]
		private BuildingManagerMemberView[] memberViews;

		// Token: 0x04007891 RID: 30865
		[Header("区域三：进度")]
		[SerializeField]
		private TextMeshProUGUI progressTitle;

		// Token: 0x04007892 RID: 30866
		[SerializeField]
		private GameObject progressBg;

		// Token: 0x04007893 RID: 30867
		[SerializeField]
		private CImage progressFill;

		// Token: 0x04007894 RID: 30868
		[SerializeField]
		private CImage progressIncrease;

		// Token: 0x04007895 RID: 30869
		[SerializeField]
		private RectTransform progressText;

		// Token: 0x04007896 RID: 30870
		[SerializeField]
		private TextMeshProUGUI progressCurrentText;

		// Token: 0x04007897 RID: 30871
		[SerializeField]
		private TextMeshProUGUI progressIncreaseText;

		// Token: 0x04007898 RID: 30872
		[SerializeField]
		private TextMeshProUGUI progressTotalText;

		// Token: 0x04007899 RID: 30873
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x0400789A RID: 30874
		[SerializeField]
		private TooltipInvoker confirmBtnTip;

		// Token: 0x0400789B RID: 30875
		[SerializeField]
		private TextMeshProUGUI confirmBtnText;

		// Token: 0x0400789C RID: 30876
		[SerializeField]
		private CButton cancelBtn;

		// Token: 0x0400789D RID: 30877
		[SerializeField]
		private TextMeshProUGUI cancelBtnText;

		// Token: 0x0400789E RID: 30878
		private readonly int[] _operatorListCached = new int[3];

		// Token: 0x0400789F RID: 30879
		private int _selectingOperatorIndex = -1;

		// Token: 0x040078A0 RID: 30880
		private readonly List<GameObject> _resourceItems = new List<GameObject>();

		// Token: 0x02002309 RID: 8969
		private enum SubPageType
		{
			// Token: 0x0400DD86 RID: 56710
			New,
			// Token: 0x0400DD87 RID: 56711
			Remove
		}
	}
}
