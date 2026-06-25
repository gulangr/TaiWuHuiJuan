using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coffee.UIExtensions;
using Config;
using FrameWork;
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
using UnityEngine.Events;

// Token: 0x02000236 RID: 566
public class LegacyListComponent : Refers
{
	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x060024D2 RID: 9426 RVA: 0x0010F476 File Offset: 0x0010D676
	private InfinityScrollLegacy _legacyScroll
	{
		get
		{
			return base.CGet<InfinityScrollLegacy>("LegacyScroll");
		}
	}

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x060024D3 RID: 9427 RVA: 0x0010F483 File Offset: 0x0010D683
	public int TotalLegacyCount
	{
		get
		{
			return this.currentLegacyList.Count + this.currentLegacieExtraList.Count;
		}
	}

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x060024D4 RID: 9428 RVA: 0x0010F49C File Offset: 0x0010D69C
	public int SelectedCount
	{
		get
		{
			return this._selectedLegacyIndices.Count;
		}
	}

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x060024D5 RID: 9429 RVA: 0x0010F4A9 File Offset: 0x0010D6A9
	private int _currentAvailableLegacyPointTotal
	{
		get
		{
			return this._isStarFortune ? this._starFortune : this._availableLegacyPointTotal;
		}
	}

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x060024D6 RID: 9430 RVA: 0x0010F4C1 File Offset: 0x0010D6C1
	public bool AllDataInitialized
	{
		get
		{
			return this._emptyBuildingBlockCount >= 0;
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x060024D7 RID: 9431 RVA: 0x0010F4CF File Offset: 0x0010D6CF
	private List<short> currentLegacyList
	{
		get
		{
			return (this._selectedWorldDetailId >= 0) ? this._availableLegacyListCache : this._availableLegacyList;
		}
	}

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x060024D8 RID: 9432 RVA: 0x0010F4E8 File Offset: 0x0010D6E8
	private List<short> currentLegacieExtraList
	{
		get
		{
			return (this._selectedWorldDetailId >= 0) ? this._extraLegaciesCache : this._extraLegacies;
		}
	}

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x060024D9 RID: 9433 RVA: 0x0010F504 File Offset: 0x0010D704
	public int AvailableLegacyPointTotal
	{
		get
		{
			int used = 0;
			foreach (int selectedIndex in this._selectedLegacyIndices)
			{
				short templateId = this.GetTemplateIdAtIndex(selectedIndex);
				LegacyItem legacyConfig = Legacy.Instance[templateId];
				bool flag = selectedIndex >= this.currentLegacyList.Count && legacyConfig.Cost >= 0;
				if (!flag)
				{
					used += (int)legacyConfig.Cost;
				}
			}
			for (int i = 0; i < this._createRandomLegacyTimes; i++)
			{
				int cost = this.GetCreateRandomLegacyCost(i);
				used += cost;
			}
			return this._currentAvailableLegacyPointTotal - used;
		}
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x0010F5D8 File Offset: 0x0010D7D8
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x0010F5E4 File Offset: 0x0010D7E4
	private void UpdateData()
	{
		bool flag = this._selectedWorldDetailId >= 0;
		if (flag)
		{
			this._availableLegacyListCache = (from t in this._availableLegacyList
			where (int)Legacy.Instance[t].WorldCreationGroup == this._selectedWorldDetailId
			select t).ToList<short>();
			this._extraLegaciesCache = (from t in this._extraLegacies
			where (int)Legacy.Instance[t].WorldCreationGroup == this._selectedWorldDetailId
			select t).ToList<short>();
		}
		this._legacyScroll.UpdateData(this.TotalLegacyCount);
		base.CGet<GameObject>("NoContent").SetActive(this.TotalLegacyCount == 0);
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x0010F674 File Offset: 0x0010D874
	public void CreateRandomLegacy()
	{
		this._createRandomLegacyTimes++;
		this.RefreshTotalAvailableLegacyPointLabel();
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x0010F68C File Offset: 0x0010D88C
	private void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._legacyScroll.OnItemRender = new Action<int, Refers>(this.OnRenderLegacy);
			this._inited = true;
		}
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x0010F6C4 File Offset: 0x0010D8C4
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

	// Token: 0x060024DF RID: 9439 RVA: 0x0010F71F File Offset: 0x0010D91F
	public void SetListByWorldDetailId(int id)
	{
		this._selectedWorldDetailId = id;
		this.ClearSelected();
		this.UpdateData();
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x0010F738 File Offset: 0x0010D938
	private void AddExtraLegacy(short templateId)
	{
		this._selectedLegacyIndices.Add(this.currentLegacyList.Count + this._extraLegaciesCache.Count);
		this._extraLegacies.Add(templateId);
		this._extraLegaciesCache.Add(templateId);
		this.RefreshTotalAvailableLegacyPointLabel();
		this._legacyScroll.UpdateData(this.TotalLegacyCount);
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x0010F79C File Offset: 0x0010D99C
	public void ClearSelected()
	{
		this._selectedLegacyIndices.Clear();
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x0010F7AC File Offset: 0x0010D9AC
	public bool ApplySelectAllLegacy()
	{
		bool hasLegacy = this._selectedLegacyIndices.Count > 0;
		foreach (int selectedLegacyIndex in this._selectedLegacyIndices)
		{
			Refers legacyRefer = this._legacyScroll.GetActiveCell(selectedLegacyIndex);
			bool flag = legacyRefer != null;
			if (flag)
			{
				UIParticle uIParticle;
				legacyRefer.CTryGet<UIParticle>("UIParticle", out uIParticle);
				if (uIParticle != null)
				{
					uIParticle.Play();
				}
			}
			short templateId = this.GetTemplateIdAtIndex(selectedLegacyIndex);
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
		bool flag2 = !this._isStarFortune;
		if (flag2)
		{
			GameDataBridge.AddDataModification<int>(5, 35, ulong.MaxValue, uint.MaxValue, this.AvailableLegacyPointTotal);
		}
		this._selectedLegacyIndices.Clear();
		this._extraLegacies.Clear();
		this._extraLegaciesCache.Clear();
		this._createRandomLegacyTimes = 0;
		return hasLegacy;
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x0010F8C0 File Offset: 0x0010DAC0
	public void HandleTaiwuFeatureId(List<short> taiwuFeatureIds)
	{
		this._taiwuFeatureIds = taiwuFeatureIds;
		bool allDataInitialized = this.AllDataInitialized;
		if (allDataInitialized)
		{
			this.UpdateData();
		}
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x0010F8E8 File Offset: 0x0010DAE8
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

	// Token: 0x060024E5 RID: 9445 RVA: 0x0010F918 File Offset: 0x0010DB18
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

	// Token: 0x060024E6 RID: 9446 RVA: 0x0010F947 File Offset: 0x0010DB47
	public void HandleAvailableLegacyList(List<short> availableLegacyList)
	{
		this._availableLegacyList = availableLegacyList;
		this._availableLegacyList.Sort(new Comparison<short>(this.CompareLegacy));
		this.UpdateData();
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x0010F970 File Offset: 0x0010DB70
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

	// Token: 0x060024E8 RID: 9448 RVA: 0x0010F9C4 File Offset: 0x0010DBC4
	private void OnRenderLegacy(int index, Refers refers)
	{
		LegacyListComponent.<>c__DisplayClass49_0 CS$<>8__locals1 = new LegacyListComponent.<>c__DisplayClass49_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.index = index;
		CS$<>8__locals1.refers = refers;
		short templateId = this.GetTemplateIdAtIndex(CS$<>8__locals1.index);
		LegacyItem configData = Legacy.Instance[templateId];
		short cost = this._isStarFortune ? configData.ExtraCost : configData.Cost;
		CS$<>8__locals1.isSelected = this._selectedLegacyIndices.Contains(CS$<>8__locals1.index);
		bool isExtra = CS$<>8__locals1.index >= this.currentLegacyList.Count;
		bool cannotCancelSelect = CS$<>8__locals1.isSelected && cost < 0 && this.AvailableLegacyPointTotal < Mathf.Abs((int)cost);
		bool hasEnoughPoints = (isExtra || this.AvailableLegacyPointTotal >= (int)cost) && !cannotCancelSelect;
		bool interactable = (this._inherit || this._isStarFortune) && (CS$<>8__locals1.isSelected || (hasEnoughPoints && this.IsLegacyValid(templateId)));
		bool disabled = false;
		StringBuilder descSb = new StringBuilder();
		descSb.AppendLine(configData.Desc);
		CS$<>8__locals1.conflictType = this.GetFeatureConflictType(templateId, out CS$<>8__locals1.conflictingFeatureId);
		switch (CS$<>8__locals1.conflictType)
		{
		case LegacyListComponent.ConflictType.OppositeSign:
			descSb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Legacy_ConflictFeature_Hint_1, CharacterFeature.Instance[CS$<>8__locals1.conflictingFeatureId].Name));
			disabled = (this._inherit || this._isStarFortune);
			break;
		case LegacyListComponent.ConflictType.LowerLevel:
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
			int conflictLegacyIndex = this.GetSelectedConflictLegacyIndex(CS$<>8__locals1.index);
			bool flag3 = conflictLegacyIndex >= 0;
			if (flag3)
			{
				short conflictLegacyId = this.GetTemplateIdAtIndex(conflictLegacyIndex);
				descSb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Legacy_ConflictFeature_Hint_2, Legacy.Instance[conflictLegacyId].Name).SetColor("brightred"));
			}
		}
		bool flag4 = cannotCancelSelect;
		if (flag4)
		{
			disabled = (this._inherit || this._isStarFortune);
			descSb.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Legacy_CancelChoose_PointNotEnough), Array.Empty<object>());
		}
		LegacyView view = (LegacyView)CS$<>8__locals1.refers;
		view.RefreshBasicInfo(configData);
		view.RefreshCostInfo(configData, this._inherit || this._isStarFortune, CS$<>8__locals1.isSelected, hasEnoughPoints, isExtra, this._isStarFortune);
		view.RefreshMouseTip(configData, descSb.ToString());
		view.RefreshHighlight(this._inherit || this._isStarFortune, isExtra, isExtra);
		view.RefreshInteraction(interactable, CS$<>8__locals1.isSelected, disabled);
		view.SetOnToggleValueChanged((this._inherit || this._isStarFortune) ? new UnityAction<bool>(CS$<>8__locals1.<OnRenderLegacy>g__OnToggleValueChanged|0) : null);
		CS$<>8__locals1.refers.CGet<CToggleObsolete>("Toggle").SetIsOnWithoutNotify(CS$<>8__locals1.isSelected);
		CS$<>8__locals1.refers.CGet<GameObject>("Selected").SetActive(CS$<>8__locals1.isSelected);
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x0010FD40 File Offset: 0x0010DF40
	private void OnSelectLegacy(int index)
	{
		short templateId = this.GetTemplateIdAtIndex(index);
		this.DeselectAllConflictLegacies(index);
		LegacyItem configData = Legacy.Instance[templateId];
		this._selectedLegacyIndices.Add(index);
		bool flag = configData.AddBuildingBlock >= 0;
		if (flag)
		{
			this._emptyBuildingBlockCount--;
		}
		this._legacyScroll.UpdateData(this.TotalLegacyCount);
		this.RefreshTotalAvailableLegacyPointLabel();
		Action onSelectedLegacyChange = this.OnSelectedLegacyChange;
		if (onSelectedLegacyChange != null)
		{
			onSelectedLegacyChange();
		}
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x0010FDC0 File Offset: 0x0010DFC0
	private void DeSelectLegacy(int index)
	{
		short templateId = this.GetTemplateIdAtIndex(index);
		LegacyItem configData = Legacy.Instance[templateId];
		this._selectedLegacyIndices.Remove(index);
		bool flag = configData.AddBuildingBlock >= 0;
		if (flag)
		{
			this._emptyBuildingBlockCount++;
		}
		this._legacyScroll.UpdateData(this.TotalLegacyCount);
		this.RefreshTotalAvailableLegacyPointLabel();
		Action onSelectedLegacyChange = this.OnSelectedLegacyChange;
		if (onSelectedLegacyChange != null)
		{
			onSelectedLegacyChange();
		}
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x0010FE38 File Offset: 0x0010E038
	private void DeselectAllConflictLegacies(int index)
	{
		short legacyTemplateId = this.GetTemplateIdAtIndex(index);
		for (int i = 0; i < this.TotalLegacyCount; i++)
		{
			bool flag = i == index;
			if (!flag)
			{
				short currTemplateId = this.GetTemplateIdAtIndex(i);
				bool flag2 = this._selectedLegacyIndices.Contains(i) && this.CheckLegacyConflict(legacyTemplateId, currTemplateId);
				if (flag2)
				{
					this.DeSelectLegacy(i);
				}
			}
		}
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x0010FEA4 File Offset: 0x0010E0A4
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

	// Token: 0x060024ED RID: 9453 RVA: 0x0010FF90 File Offset: 0x0010E190
	private void RefreshTotalAvailableLegacyPointLabel()
	{
		bool isStarFortune = this._isStarFortune;
		if (isStarFortune)
		{
			base.CGet<TextMeshProUGUI>("TotalAvailableLegacyPointLabel").text = LocalStringManager.GetFormat(LanguageKey.LK_SectMainStory_JieQing_AvailableStarFortune, this.AvailableLegacyPointTotal).ColorReplace();
		}
		else
		{
			bool inherit = this._inherit;
			if (inherit)
			{
				base.CGet<TextMeshProUGUI>("TotalAvailableLegacyPointLabel").text = LocalStringManager.GetFormat(LanguageKey.LK_Available_Legacy_Point, this.AvailableLegacyPointTotal).ColorReplace();
			}
			else
			{
				base.CGet<TextMeshProUGUI>("TotalAvailableLegacyPointLabel").text = LocalStringManager.Get(LanguageKey.LK_Legacy);
			}
		}
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x00110028 File Offset: 0x0010E228
	private short GetTemplateIdAtIndex(int index)
	{
		return (index >= this.currentLegacyList.Count) ? this._extraLegaciesCache[index - this.currentLegacyList.Count] : this.currentLegacyList[index];
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x0011006E File Offset: 0x0010E26E
	public int GetCreateRandomLegacyCost(int times)
	{
		return (int)Math.Pow(2.0, (double)times) * GlobalConfig.Instance.SelectRandomLegacyCost;
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x0011008C File Offset: 0x0010E28C
	private bool IsLegacyValid(short legacyTemplateId)
	{
		LegacyItem legacyConfig = Legacy.Instance[legacyTemplateId];
		bool flag = legacyConfig.AddBuildingBlock >= 0;
		return !flag || this._emptyBuildingBlockCount > 0;
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x001100C8 File Offset: 0x0010E2C8
	private LegacyListComponent.ConflictType GetFeatureConflictType(short legacyTemplateId, out short conflictingFeature)
	{
		conflictingFeature = -1;
		LegacyItem legacyConfig = Legacy.Instance[legacyTemplateId];
		bool flag = legacyConfig.AddFeature < 0;
		LegacyListComponent.ConflictType result;
		if (flag)
		{
			result = LegacyListComponent.ConflictType.None;
		}
		else
		{
			CharacterFeatureItem featureConfig = CharacterFeature.Instance[legacyConfig.AddFeature];
			bool flag2 = featureConfig.MutexGroupId < 0;
			if (flag2)
			{
				result = LegacyListComponent.ConflictType.None;
			}
			else
			{
				foreach (short taiwuFeatureId in this._taiwuFeatureIds)
				{
					conflictingFeature = taiwuFeatureId;
					bool flag3 = featureConfig.TemplateId == taiwuFeatureId;
					if (flag3)
					{
						return LegacyListComponent.ConflictType.LowerLevel;
					}
					CharacterFeatureItem taiwuFeature = CharacterFeature.Instance[taiwuFeatureId];
					bool flag4 = taiwuFeature.MutexGroupId != featureConfig.MutexGroupId;
					if (!flag4)
					{
						bool flag5 = featureConfig.Type != taiwuFeature.Type;
						if (flag5)
						{
							return LegacyListComponent.ConflictType.OppositeSign;
						}
						bool flag6 = featureConfig.Level <= taiwuFeature.Level;
						if (flag6)
						{
							return LegacyListComponent.ConflictType.LowerLevel;
						}
						return LegacyListComponent.ConflictType.HigherLevel;
					}
				}
				result = LegacyListComponent.ConflictType.None;
			}
		}
		return result;
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x001101F0 File Offset: 0x0010E3F0
	private int GetSelectedConflictLegacyIndex(int index)
	{
		short templateId = this.GetTemplateIdAtIndex(index);
		for (int i = 0; i < this.TotalLegacyCount; i++)
		{
			bool flag = i == index;
			if (!flag)
			{
				short currTemplateId = this.GetTemplateIdAtIndex(i);
				bool flag2 = this._selectedLegacyIndices.Contains(i) && this.CheckLegacyConflict(currTemplateId, templateId);
				if (flag2)
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x0011025C File Offset: 0x0010E45C
	public void GetRandomLegacy()
	{
		int cost = this.GetCreateRandomLegacyCost(this._createRandomLegacyTimes);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("LegacyPoint", this.AvailableLegacyPointTotal).Set("Cost", cost).SetObject("WorldCreationInfo", this._worldCreationInfo).SetObject("OnSelectLegacy", new Action<short>(this.AddExtraLegacy)).SetObject("OnCreateRandomLegacy", new Action(this.CreateRandomLegacy));
		UIElement.SelectLegacyRewardGroup.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectLegacyRewardGroup, true);
	}

	// Token: 0x04001B7F RID: 7039
	private const ushort TaiwuDomainId = 5;

	// Token: 0x04001B80 RID: 7040
	private List<short> _availableLegacyList = new List<short>();

	// Token: 0x04001B81 RID: 7041
	private readonly List<short> _extraLegacies = new List<short>();

	// Token: 0x04001B82 RID: 7042
	private readonly HashSet<int> _selectedLegacyIndices = new HashSet<int>();

	// Token: 0x04001B83 RID: 7043
	private int _createRandomLegacyTimes;

	// Token: 0x04001B84 RID: 7044
	private int _availableLegacyPointTotal;

	// Token: 0x04001B85 RID: 7045
	private int _starFortune;

	// Token: 0x04001B86 RID: 7046
	private bool _inherit;

	// Token: 0x04001B87 RID: 7047
	private bool _isStarFortune;

	// Token: 0x04001B88 RID: 7048
	public sbyte InheritCharacterBehaviorType = -1;

	// Token: 0x04001B89 RID: 7049
	public WorldCreationInfo _worldCreationInfo;

	// Token: 0x04001B8A RID: 7050
	private int _emptyBuildingBlockCount = -1;

	// Token: 0x04001B8B RID: 7051
	private List<short> _taiwuFeatureIds = new List<short>();

	// Token: 0x04001B8C RID: 7052
	private int _selectedWorldDetailId = -1;

	// Token: 0x04001B8D RID: 7053
	private List<short> _availableLegacyListCache = new List<short>();

	// Token: 0x04001B8E RID: 7054
	private List<short> _extraLegaciesCache = new List<short>();

	// Token: 0x04001B8F RID: 7055
	private bool _inited = false;

	// Token: 0x04001B90 RID: 7056
	public Action OnSelectedLegacyChange;

	// Token: 0x04001B91 RID: 7057
	private Action _onDataReady;

	// Token: 0x0200153C RID: 5436
	private enum ConflictType
	{
		// Token: 0x0400A3E7 RID: 41959
		None,
		// Token: 0x0400A3E8 RID: 41960
		OppositeSign,
		// Token: 0x0400A3E9 RID: 41961
		LowerLevel,
		// Token: 0x0400A3EA RID: 41962
		HigherLevel
	}
}
