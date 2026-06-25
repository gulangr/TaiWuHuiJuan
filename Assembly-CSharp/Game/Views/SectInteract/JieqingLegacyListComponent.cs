using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A1 RID: 2465
	public class JieqingLegacyListComponent : Refers
	{
		// Token: 0x17000D52 RID: 3410
		// (get) Token: 0x06007694 RID: 30356 RVA: 0x00373FC1 File Offset: 0x003721C1
		public int CurrentTotalLegacyCount
		{
			get
			{
				return this._availableLegacyListCache.Count;
			}
		}

		// Token: 0x17000D53 RID: 3411
		// (get) Token: 0x06007695 RID: 30357 RVA: 0x00373FCE File Offset: 0x003721CE
		public int SelectedCount
		{
			get
			{
				return this._selectedLegacyIndices.Count;
			}
		}

		// Token: 0x17000D54 RID: 3412
		// (get) Token: 0x06007696 RID: 30358 RVA: 0x00373FDB File Offset: 0x003721DB
		private int _currentAvailableLegacyPointTotal
		{
			get
			{
				return this._isStarFortune ? this._starFortune : this._availableLegacyPointTotal;
			}
		}

		// Token: 0x17000D55 RID: 3413
		// (get) Token: 0x06007697 RID: 30359 RVA: 0x00373FF3 File Offset: 0x003721F3
		public bool AllDataInitialized
		{
			get
			{
				return this._emptyBuildingBlockCount >= 0 && this._availableLegacyDic.Count > 0;
			}
		}

		// Token: 0x17000D56 RID: 3414
		// (get) Token: 0x06007698 RID: 30360 RVA: 0x00374010 File Offset: 0x00372210
		public int AvailableLegacyPointTotal
		{
			get
			{
				int used = this.GetUsedAmount();
				return this._currentAvailableLegacyPointTotal - used;
			}
		}

		// Token: 0x06007699 RID: 30361 RVA: 0x00374034 File Offset: 0x00372234
		private int GetUsedAmount()
		{
			int usedAmount = 0;
			foreach (int selectedIndex in this._selectedLegacyIndices)
			{
				short templateId = this._availableLegacyList[selectedIndex];
				LegacyItem legacyConfig = Legacy.Instance[templateId];
				usedAmount += (int)(this._isStarFortune ? legacyConfig.ExtraCost : legacyConfig.Cost);
			}
			for (int i = 0; i < this._createRandomLegacyTimes; i++)
			{
				int cost = this.GetCreateRandomLegacyCost(i);
				usedAmount += cost;
			}
			return usedAmount;
		}

		// Token: 0x0600769A RID: 30362 RVA: 0x003740F0 File Offset: 0x003722F0
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600769B RID: 30363 RVA: 0x003740FA File Offset: 0x003722FA
		private void UpdateData()
		{
			this._availableLegacyListCache = this._availableLegacyDic[this._selectedWorldDetailId];
			this.legacyScroll.UpdateData(this.CurrentTotalLegacyCount);
		}

		// Token: 0x0600769C RID: 30364 RVA: 0x00374126 File Offset: 0x00372326
		public void CreateRandomLegacy()
		{
			this._createRandomLegacyTimes++;
			this.RefreshTotalAvailableLegacyPointLabel();
		}

		// Token: 0x0600769D RID: 30365 RVA: 0x00374140 File Offset: 0x00372340
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.legacyScroll.OnItemRender += this.OnRenderLegacy;
				this._inited = true;
			}
		}

		// Token: 0x0600769E RID: 30366 RVA: 0x00374179 File Offset: 0x00372379
		private void OnDestroy()
		{
			this.legacyScroll.OnItemRender -= this.OnRenderLegacy;
		}

		// Token: 0x0600769F RID: 30367 RVA: 0x00374194 File Offset: 0x00372394
		public void OnInit(bool inherit, bool isStarFortune, Action onReady)
		{
			this.Init();
			this._onDataReady = onReady;
			this._inherit = inherit;
			this._isStarFortune = isStarFortune;
			this._emptyBuildingBlockCount = -1;
			Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
			BuildingDomainMethod.AsyncCall.GetEmptyBlockCount(null, taiwuVillageLocation.AreaId, taiwuVillageLocation.BlockId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._emptyBuildingBlockCount);
				bool allDataInitialized = this.AllDataInitialized;
				if (allDataInitialized)
				{
					Action onDataReady = this._onDataReady;
					if (onDataReady != null)
					{
						onDataReady();
					}
				}
			});
		}

		// Token: 0x060076A0 RID: 30368 RVA: 0x003741F0 File Offset: 0x003723F0
		public void SetListByWorldDetailId(int id)
		{
			this._selectedWorldDetailId = id - 1;
			bool allDataInitialized = this.AllDataInitialized;
			if (allDataInitialized)
			{
				this.UpdateData();
			}
		}

		// Token: 0x060076A1 RID: 30369 RVA: 0x0037421A File Offset: 0x0037241A
		public void ClearSelected()
		{
			this._selectedLegacyIndices.Clear();
		}

		// Token: 0x060076A2 RID: 30370 RVA: 0x0037422C File Offset: 0x0037242C
		public bool ApplySelectAllLegacy()
		{
			bool hasLegacy = this._selectedLegacyIndices.Count > 0;
			foreach (int selectedLegacyIndex in this._selectedLegacyIndices)
			{
				short templateId = this._availableLegacyList[selectedLegacyIndex];
				bool isStarFortune = this._isStarFortune;
				if (isStarFortune)
				{
					ExtraDomainMethod.Call.ConsumeExtraLegacyPoint(templateId);
				}
				else
				{
					TaiwuDomainMethod.Call.SelectLegacy(templateId);
				}
			}
			bool flag = !this._isStarFortune;
			if (flag)
			{
				GameDataBridge.AddDataModification<int>(5, 35, ulong.MaxValue, uint.MaxValue, this.AvailableLegacyPointTotal);
			}
			this._selectedLegacyIndices.Clear();
			this._createRandomLegacyTimes = 0;
			return hasLegacy;
		}

		// Token: 0x060076A3 RID: 30371 RVA: 0x003742F0 File Offset: 0x003724F0
		public void HandleTaiwuFeatureId(List<short> taiwuFeatureIds)
		{
			this._taiwuFeatureIds = taiwuFeatureIds;
			bool allDataInitialized = this.AllDataInitialized;
			if (allDataInitialized)
			{
				this.UpdateData();
			}
		}

		// Token: 0x060076A4 RID: 30372 RVA: 0x00374318 File Offset: 0x00372518
		public void HandleAvaliableLegacyPoint(int availableLegacyPointTotal)
		{
			this._availableLegacyPointTotal = availableLegacyPointTotal;
			this.RefreshTotalAvailableLegacyPointLabel();
			bool allDataInitialized = this.AllDataInitialized;
			if (allDataInitialized)
			{
				this.UpdateData();
			}
		}

		// Token: 0x060076A5 RID: 30373 RVA: 0x00374348 File Offset: 0x00372548
		public void HandleStarFortunePoint(int availableStarFortunePointTotal)
		{
			this._starFortune = availableStarFortunePointTotal;
			this.RefreshTotalAvailableLegacyPointLabel();
			bool allDataInitialized = this.AllDataInitialized;
			if (allDataInitialized)
			{
				this.UpdateData();
			}
		}

		// Token: 0x060076A6 RID: 30374 RVA: 0x00374377 File Offset: 0x00372577
		public void HandleAvailableLegacyList(List<short> availableLegacyList)
		{
			this._availableLegacyList = availableLegacyList;
			this._availableLegacyList.Sort(new Comparison<short>(this.CompareLegacy));
			this.CollectInfo();
			this.UpdateData();
		}

		// Token: 0x060076A7 RID: 30375 RVA: 0x003743A8 File Offset: 0x003725A8
		private void CollectInfo()
		{
			this._availableLegacyDic.Clear();
			this._availableLegacyDic[-1] = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				this._availableLegacyDic[i] = new List<int>();
			}
			for (int j = 0; j < this._availableLegacyList.Count; j++)
			{
				List<int> indexList;
				bool flag = this._availableLegacyDic.TryGetValue((int)Legacy.Instance[this._availableLegacyList[j]].WorldCreationGroup, out indexList);
				if (flag)
				{
					indexList.Add(j);
				}
				this._availableLegacyDic[-1].Add(j);
			}
		}

		// Token: 0x060076A8 RID: 30376 RVA: 0x00374464 File Offset: 0x00372664
		private int CompareLegacy(short templateIdA, short templateIdB)
		{
			LegacyItem configA = Legacy.Instance[templateIdA];
			LegacyItem configB = Legacy.Instance[templateIdB];
			int compareGrade = configA.Grade.CompareTo(configB.Grade);
			bool flag = compareGrade != 0;
			int result;
			if (flag)
			{
				result = compareGrade;
			}
			else
			{
				result = templateIdA.CompareTo(templateIdB);
			}
			return result;
		}

		// Token: 0x060076A9 RID: 30377 RVA: 0x003744B8 File Offset: 0x003726B8
		private void OnRenderLegacy(int index, GameObject obj)
		{
			JieqingLegacyListComponent.<>c__DisplayClass47_0 CS$<>8__locals1 = new JieqingLegacyListComponent.<>c__DisplayClass47_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			CS$<>8__locals1.obj = obj;
			short templateId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, CS$<>8__locals1.index);
			LegacyItem configData = Legacy.Instance[templateId];
			short cost = this._isStarFortune ? configData.ExtraCost : configData.Cost;
			int realIndex = this._availableLegacyDic[this._selectedWorldDetailId][CS$<>8__locals1.index];
			CS$<>8__locals1.isSelected = this._selectedLegacyIndices.Contains(realIndex);
			bool isExtra = CS$<>8__locals1.index >= this._availableLegacyDic[this._selectedWorldDetailId].Count;
			bool cannotCancelSelect = CS$<>8__locals1.isSelected && cost < 0 && this.AvailableLegacyPointTotal < Mathf.Abs((int)cost);
			bool hasEnoughPoints = (isExtra || this.AvailableLegacyPointTotal >= (int)cost) && !cannotCancelSelect;
			bool interactable = (this._inherit || this._isStarFortune) && (CS$<>8__locals1.isSelected || (hasEnoughPoints && this.IsLegacyValid(templateId)));
			bool disabled = false;
			StringBuilder descSb = new StringBuilder();
			descSb.AppendLine(configData.Desc);
			CS$<>8__locals1.conflictType = this.GetFeatureConflictType(templateId, out CS$<>8__locals1.conflictingFeatureId);
			switch (CS$<>8__locals1.conflictType)
			{
			case JieqingLegacyListComponent.ConflictType.OppositeSign:
				descSb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Legacy_ConflictFeature_Hint_1, CharacterFeature.Instance[CS$<>8__locals1.conflictingFeatureId].Name));
				disabled = (this._inherit || this._isStarFortune);
				break;
			case JieqingLegacyListComponent.ConflictType.LowerLevel:
				descSb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Legacy_ConflictFeature_Hint_0, CharacterFeature.Instance[CS$<>8__locals1.conflictingFeatureId].Name));
				disabled = (this._inherit || this._isStarFortune);
				break;
			}
			bool flag = this.InheritCharacterBehaviorType >= 0 && this.InheritCharacterBehaviorType == Legacy.Instance[templateId].TargetBehaviorType;
			if (flag)
			{
				disabled = (this._inherit || this._isStarFortune);
				descSb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Legacy_BehaviorTypeIsSame));
			}
			bool flag2 = !disabled;
			if (flag2)
			{
				int conflictLegacyIndex = this.GetSelectedConflictLegacyIndex(this._selectedWorldDetailId, CS$<>8__locals1.index);
				bool flag3 = conflictLegacyIndex >= 0;
				if (flag3)
				{
					short conflictLegacyId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, conflictLegacyIndex);
					descSb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Legacy_ConflictFeature_Hint_2, Legacy.Instance[conflictLegacyId].Name).SetColor("brightred"));
				}
			}
			bool flag4 = cannotCancelSelect;
			if (flag4)
			{
				disabled = (this._inherit || this._isStarFortune);
				descSb.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Legacy_CancelChoose_PointNotEnough), Array.Empty<object>());
			}
			JieqingLegacyView view = CS$<>8__locals1.obj.GetComponent<JieqingLegacyView>();
			view.RefreshBasicInfo(configData);
			view.RefreshCostInfo(configData, this._inherit || this._isStarFortune, CS$<>8__locals1.isSelected, hasEnoughPoints, isExtra, this._isStarFortune);
			view.RefreshMouseTip(configData, descSb.ToString());
			view.RefreshInteraction(interactable, CS$<>8__locals1.isSelected, disabled);
			view.SetOnToggleValueChanged((this._inherit || this._isStarFortune) ? new Action<bool>(CS$<>8__locals1.<OnRenderLegacy>g__OnToggleValueChanged|0) : null);
			view.SetSelected(CS$<>8__locals1.isSelected, false);
		}

		// Token: 0x060076AA RID: 30378 RVA: 0x00374824 File Offset: 0x00372A24
		private void OnSelectLegacy(int index)
		{
			short templateId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, index);
			this.DeselectAllConflictLegacies(index);
			LegacyItem configData = Legacy.Instance[templateId];
			int realIndex = this._availableLegacyDic[this._selectedWorldDetailId][index];
			this._selectedLegacyIndices.Add(realIndex);
			bool flag = configData.AddBuildingBlock >= 0;
			if (flag)
			{
				this._emptyBuildingBlockCount--;
			}
			this.legacyScroll.UpdateData(this.CurrentTotalLegacyCount);
			this.RefreshTotalAvailableLegacyPointLabel();
			Action onSelectedLegacyChange = this.OnSelectedLegacyChange;
			if (onSelectedLegacyChange != null)
			{
				onSelectedLegacyChange();
			}
		}

		// Token: 0x060076AB RID: 30379 RVA: 0x003748C4 File Offset: 0x00372AC4
		private void DeSelectLegacy(int index)
		{
			short templateId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, index);
			LegacyItem configData = Legacy.Instance[templateId];
			int realIndex = this._availableLegacyDic[this._selectedWorldDetailId][index];
			this._selectedLegacyIndices.Remove(realIndex);
			bool flag = configData.AddBuildingBlock >= 0;
			if (flag)
			{
				this._emptyBuildingBlockCount++;
			}
			this.legacyScroll.UpdateData(this.CurrentTotalLegacyCount);
			this.RefreshTotalAvailableLegacyPointLabel();
			Action onSelectedLegacyChange = this.OnSelectedLegacyChange;
			if (onSelectedLegacyChange != null)
			{
				onSelectedLegacyChange();
			}
		}

		// Token: 0x060076AC RID: 30380 RVA: 0x0037495C File Offset: 0x00372B5C
		private void DeselectAllConflictLegacies(int index)
		{
			short legacyTemplateId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, index);
			for (int i = 0; i < this.CurrentTotalLegacyCount; i++)
			{
				bool flag = i == index;
				if (!flag)
				{
					short currTemplateId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, i);
					int realIndex = this._availableLegacyDic[this._selectedWorldDetailId][i];
					bool flag2 = this._selectedLegacyIndices.Contains(realIndex) && this.CheckLegacyConflict(legacyTemplateId, currTemplateId);
					if (flag2)
					{
						this.DeSelectLegacy(i);
					}
				}
			}
		}

		// Token: 0x060076AD RID: 30381 RVA: 0x003749EC File Offset: 0x00372BEC
		private bool CheckLegacyConflict(short templateIdA, short templateIdB)
		{
			LegacyItem configA = Legacy.Instance[templateIdA];
			LegacyItem configB = Legacy.Instance[templateIdB];
			bool flag = configA.GroupId >= 0 && configA.GroupId == configB.GroupId;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = configA.AddFeature >= 0 && configB.AddFeature >= 0;
				if (flag2)
				{
					bool flag3 = configA.AddFeature == configB.AddFeature;
					if (flag3)
					{
						result = true;
					}
					else
					{
						short mutexGroupA = CharacterFeature.Instance[configA.AddFeature].MutexGroupId;
						short mutexGroupB = CharacterFeature.Instance[configB.AddFeature].MutexGroupId;
						result = (mutexGroupA == mutexGroupB && mutexGroupA >= 0);
					}
				}
				else
				{
					bool flag4 = configA.TargetBehaviorType >= 0 && configB.TargetBehaviorType >= 0;
					result = flag4;
				}
			}
			return result;
		}

		// Token: 0x060076AE RID: 30382 RVA: 0x00374AD8 File Offset: 0x00372CD8
		private void RefreshTotalAvailableLegacyPointLabel()
		{
			bool isStarFortune = this._isStarFortune;
			if (isStarFortune)
			{
				this._sb.Clear();
				this._sb.Append(this._starFortune);
				bool flag = this._selectedLegacyIndices.Count > 0;
				if (flag)
				{
					int amount = this.GetUsedAmount();
					bool flag2 = amount < 100;
					if (flag2)
					{
						this._sb.Append('+');
						this._sb.Append(-amount);
					}
					else
					{
						this._sb.Append('-');
						this._sb.Append(amount);
					}
				}
				this.txtTotalAvailableLegacyPointLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_SectMainStory_JieQing_AvailableStarFortune, this._sb.ToString() ?? "").ColorReplace();
			}
			else
			{
				bool inherit = this._inherit;
				if (inherit)
				{
					this.txtTotalAvailableLegacyPointLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Available_Legacy_Point, this.AvailableLegacyPointTotal).ColorReplace();
				}
				else
				{
					this.txtTotalAvailableLegacyPointLabel.text = LocalStringManager.Get(LanguageKey.LK_Legacy);
				}
			}
		}

		// Token: 0x060076AF RID: 30383 RVA: 0x00374BF0 File Offset: 0x00372DF0
		private short GetTemplateIdAtIndex(int togIndex, int index)
		{
			bool flag = this._availableLegacyDic.ContainsKey(togIndex);
			short result;
			if (flag)
			{
				int originalIndex = this._availableLegacyDic[togIndex][index];
				result = this._availableLegacyList[originalIndex];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x060076B0 RID: 30384 RVA: 0x00374C36 File Offset: 0x00372E36
		public int GetCreateRandomLegacyCost(int times)
		{
			return (int)Math.Pow(2.0, (double)times) * GlobalConfig.Instance.SelectRandomLegacyCost;
		}

		// Token: 0x060076B1 RID: 30385 RVA: 0x00374C54 File Offset: 0x00372E54
		private bool IsLegacyValid(short legacyTemplateId)
		{
			LegacyItem legacyConfig = Legacy.Instance[legacyTemplateId];
			bool flag = legacyConfig.AddBuildingBlock >= 0;
			return !flag || this._emptyBuildingBlockCount > 0;
		}

		// Token: 0x060076B2 RID: 30386 RVA: 0x00374C90 File Offset: 0x00372E90
		private JieqingLegacyListComponent.ConflictType GetFeatureConflictType(short legacyTemplateId, out short conflictingFeature)
		{
			conflictingFeature = -1;
			LegacyItem legacyConfig = Legacy.Instance[legacyTemplateId];
			bool flag = legacyConfig.AddFeature < 0;
			JieqingLegacyListComponent.ConflictType result;
			if (flag)
			{
				result = JieqingLegacyListComponent.ConflictType.None;
			}
			else
			{
				CharacterFeatureItem featureConfig = CharacterFeature.Instance[legacyConfig.AddFeature];
				bool flag2 = featureConfig.MutexGroupId < 0;
				if (flag2)
				{
					result = JieqingLegacyListComponent.ConflictType.None;
				}
				else
				{
					foreach (short taiwuFeatureId in this._taiwuFeatureIds)
					{
						conflictingFeature = taiwuFeatureId;
						bool flag3 = featureConfig.TemplateId == taiwuFeatureId;
						if (flag3)
						{
							return JieqingLegacyListComponent.ConflictType.LowerLevel;
						}
						CharacterFeatureItem taiwuFeature = CharacterFeature.Instance[taiwuFeatureId];
						bool flag4 = taiwuFeature.MutexGroupId != featureConfig.MutexGroupId;
						if (!flag4)
						{
							bool flag5 = featureConfig.Type != taiwuFeature.Type;
							if (flag5)
							{
								return JieqingLegacyListComponent.ConflictType.OppositeSign;
							}
							bool flag6 = featureConfig.Level <= taiwuFeature.Level;
							if (flag6)
							{
								return JieqingLegacyListComponent.ConflictType.LowerLevel;
							}
							return JieqingLegacyListComponent.ConflictType.HigherLevel;
						}
					}
					result = JieqingLegacyListComponent.ConflictType.None;
				}
			}
			return result;
		}

		// Token: 0x060076B3 RID: 30387 RVA: 0x00374DB8 File Offset: 0x00372FB8
		private int GetSelectedConflictLegacyIndex(int togIndex, int index)
		{
			short templateId = this.GetTemplateIdAtIndex(togIndex, index);
			for (int i = 0; i < this.CurrentTotalLegacyCount; i++)
			{
				bool flag = i == index;
				if (!flag)
				{
					short currTemplateId = this.GetTemplateIdAtIndex(this._selectedWorldDetailId, i);
					int realIndex = this._availableLegacyDic[this._selectedWorldDetailId][i];
					bool flag2 = this._selectedLegacyIndices.Contains(realIndex) && this.CheckLegacyConflict(currTemplateId, templateId);
					if (flag2)
					{
						return i;
					}
				}
			}
			return -1;
		}

		// Token: 0x060076B4 RID: 30388 RVA: 0x00374E48 File Offset: 0x00373048
		public void GetRandomLegacy()
		{
			int cost = this.GetCreateRandomLegacyCost(this._createRandomLegacyTimes);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("LegacyPoint", this.AvailableLegacyPointTotal).Set("Cost", cost).SetObject("WorldCreationInfo", this._worldCreationInfo).SetObject("OnCreateRandomLegacy", new Action(this.CreateRandomLegacy));
			UIElement.SelectLegacyRewardGroup.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SelectLegacyRewardGroup, true);
		}

		// Token: 0x04005972 RID: 22898
		private const ushort TaiwuDomainId = 5;

		// Token: 0x04005973 RID: 22899
		[SerializeField]
		private InfinityScroll legacyScroll;

		// Token: 0x04005974 RID: 22900
		[SerializeField]
		private TextMeshProUGUI txtTotalAvailableLegacyPointLabel;

		// Token: 0x04005975 RID: 22901
		private List<short> _availableLegacyList = new List<short>();

		// Token: 0x04005976 RID: 22902
		private readonly HashSet<int> _selectedLegacyIndices = new HashSet<int>();

		// Token: 0x04005977 RID: 22903
		private int _createRandomLegacyTimes;

		// Token: 0x04005978 RID: 22904
		private int _availableLegacyPointTotal;

		// Token: 0x04005979 RID: 22905
		private int _starFortune;

		// Token: 0x0400597A RID: 22906
		private bool _inherit;

		// Token: 0x0400597B RID: 22907
		private bool _isStarFortune;

		// Token: 0x0400597C RID: 22908
		public sbyte InheritCharacterBehaviorType = -1;

		// Token: 0x0400597D RID: 22909
		public WorldCreationInfo _worldCreationInfo;

		// Token: 0x0400597E RID: 22910
		private int _emptyBuildingBlockCount = -1;

		// Token: 0x0400597F RID: 22911
		private List<short> _taiwuFeatureIds = new List<short>();

		// Token: 0x04005980 RID: 22912
		private int _selectedWorldDetailId = -1;

		// Token: 0x04005981 RID: 22913
		private List<int> _availableLegacyListCache = new List<int>();

		// Token: 0x04005982 RID: 22914
		private Dictionary<int, List<int>> _availableLegacyDic = new Dictionary<int, List<int>>();

		// Token: 0x04005983 RID: 22915
		private StringBuilder _sb = new StringBuilder();

		// Token: 0x04005984 RID: 22916
		private bool _inited = false;

		// Token: 0x04005985 RID: 22917
		public Action OnSelectedLegacyChange;

		// Token: 0x04005986 RID: 22918
		private Action _onDataReady;

		// Token: 0x02001EC6 RID: 7878
		private enum ConflictType
		{
			// Token: 0x0400CB0E RID: 51982
			None,
			// Token: 0x0400CB0F RID: 51983
			OppositeSign,
			// Token: 0x0400CB10 RID: 51984
			LowerLevel,
			// Token: 0x0400CB11 RID: 51985
			HigherLevel
		}
	}
}
