using System;
using System.Collections.Generic;
using Config;
using FrameWork.Tools.Random;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000804 RID: 2052
	public class NewGameSubPageCustomPresetFeature : NewGameSubPageCustomPresetItemBase
	{
		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x0600645A RID: 25690 RVA: 0x002DF17C File Offset: 0x002DD37C
		public override DialogCmd StartGameCheck
		{
			get
			{
				DialogCmd result;
				if (this.RemainingPoints <= 0)
				{
					result = null;
				}
				else
				{
					DialogCmd dialogCmd = new DialogCmd();
					dialogCmd.Title = LanguageKey.LK_NewGame_StartGameCheck_NewGameSubPageCustomPreset_Title.Tr();
					dialogCmd.Content = LanguageKey.LK_NewGame_StartGameCheck_NewGameSubPageCustomPreset_Desc.Tr();
					dialogCmd.Yes = delegate()
					{
						this.ParentCustomPresetPage.StartGameChecked = true;
					};
					result = dialogCmd;
					dialogCmd.No = delegate()
					{
						this.ParentCustomPresetPage.SwitchToSubPage(NewGameSubPageCustomPreset.ESubPageType.Feature, true, true);
					};
				}
				return result;
			}
		}

		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x0600645B RID: 25691 RVA: 0x002DF1DE File Offset: 0x002DD3DE
		// (set) Token: 0x0600645C RID: 25692 RVA: 0x002DF1E6 File Offset: 0x002DD3E6
		public override bool StartGameChecked { get; set; }

		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x0600645D RID: 25693 RVA: 0x002DF1EF File Offset: 0x002DD3EF
		public override int SpentPoints
		{
			get
			{
				return this._spentPoints;
			}
		}

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x0600645E RID: 25694 RVA: 0x002DF1F7 File Offset: 0x002DD3F7
		public override int RemainingPoints
		{
			get
			{
				return (int)GlobalConfig.Instance.CustomProtagonistCharacterFeatureTotalPoint - this._spentPoints;
			}
		}

		// Token: 0x0600645F RID: 25695 RVA: 0x002DF20A File Offset: 0x002DD40A
		private void Awake()
		{
			this.EnsureUiInitialized();
		}

		// Token: 0x06006460 RID: 25696 RVA: 0x002DF214 File Offset: 0x002DD414
		public override void RefreshUI()
		{
			this.EnsureUiInitialized();
			this.RefreshFeatureUI();
		}

		// Token: 0x06006461 RID: 25697 RVA: 0x002DF228 File Offset: 0x002DD428
		public override void ApplyToPreset(CustomProtagonistPresetItem presetItem)
		{
			bool flag = this._selectedFeatureByGroup.Count > 0;
			if (flag)
			{
				presetItem.SelectedFeatures = new List<short>(this._selectedFeatureByGroup.Values);
			}
			else
			{
				presetItem.SelectedFeatures = null;
			}
		}

		// Token: 0x06006462 RID: 25698 RVA: 0x002DF268 File Offset: 0x002DD468
		public override void ApplyFromPreset(CustomProtagonistPresetItem presetItem)
		{
			this.CloseFeatureSelection();
			this._selectedFeatureByGroup.Clear();
			bool flag = presetItem.SelectedFeatures != null;
			if (flag)
			{
				foreach (short featureId in presetItem.SelectedFeatures)
				{
					CharacterFeatureItem featureItem;
					bool flag2 = !this.TryGetDisplayableFeature(featureId, out featureItem);
					if (!flag2)
					{
						short groupId = NewGameSubPageCustomPresetFeature.ResolveGroupId(featureItem);
						bool flag3 = !this._selectedFeatureByGroup.ContainsKey(groupId);
						if (flag3)
						{
							this._selectedFeatureByGroup[groupId] = featureId;
						}
					}
				}
			}
			this.RecalculateSpentPoints();
			this.RefreshFeatureResetButton();
		}

		// Token: 0x06006463 RID: 25699 RVA: 0x002DF32C File Offset: 0x002DD52C
		public void ResetFeatures()
		{
			bool flag = this._selectedFeatureByGroup.Count <= 0;
			if (!flag)
			{
				this.CloseFeatureSelection();
				this._selectedFeatureByGroup.Clear();
				this.OnFeatureSelectionChanged();
			}
		}

		// Token: 0x06006464 RID: 25700 RVA: 0x002DF36C File Offset: 0x002DD56C
		public void RandomAllocateFeatures(CustomProtagonistPresetItem presetItem, CustomRandom random, bool resetBeforeRandom = true)
		{
			this.EnsureUiInitialized();
			this.ApplyFromPreset(presetItem);
			this.CloseFeatureSelection();
			if (resetBeforeRandom)
			{
				this._selectedFeatureByGroup.Clear();
				this._spentPoints = 0;
			}
			List<NewGameSubPageCustomPresetFeature.FeatureRandomCandidate> candidates = new List<NewGameSubPageCustomPresetFeature.FeatureRandomCandidate>();
			while (this.RemainingPoints > 0)
			{
				this.BuildRandomCandidates(candidates, this.RemainingPoints);
				bool flag = candidates.Count <= 0;
				if (flag)
				{
					break;
				}
				NewGameSubPageCustomPresetFeature.FeatureRandomCandidate selectedCandidate = candidates[random.Next(candidates.Count)];
				this._selectedFeatureByGroup[selectedCandidate.GroupId] = selectedCandidate.FeatureId;
				this._spentPoints += selectedCandidate.CostDelta;
			}
			this.RecalculateSpentPoints();
			this.ApplyToPreset(presetItem);
		}

		// Token: 0x06006465 RID: 25701 RVA: 0x002DF434 File Offset: 0x002DD634
		public bool TryCloseFeatureSelectionByEsc()
		{
			bool flag = this._activeSelectionPanel == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.CloseFeatureSelection();
				result = true;
			}
			return result;
		}

		// Token: 0x06006466 RID: 25702 RVA: 0x002DF464 File Offset: 0x002DD664
		public void AddFeature(short featureId)
		{
			CharacterFeatureItem featureItem;
			bool flag = !this.TryGetDisplayableFeature(featureId, out featureItem);
			if (!flag)
			{
				short groupId = NewGameSubPageCustomPresetFeature.ResolveGroupId(featureItem);
				short oldSelectedFeatureId = this.GetSelectedFeatureId(groupId);
				bool flag2 = oldSelectedFeatureId == featureId;
				if (!flag2)
				{
					int newFeatureCost = NewGameSubPageCustomPresetFeature.GetFeatureCost(featureItem.Level);
					short maxPoint = GlobalConfig.Instance.CustomProtagonistCharacterFeatureTotalPoint;
					bool flag3 = oldSelectedFeatureId >= 0;
					if (flag3)
					{
						int oldFeatureCost = NewGameSubPageCustomPresetFeature.GetFeatureCost(CharacterFeature.Instance[oldSelectedFeatureId].Level);
						bool flag4 = this._spentPoints - oldFeatureCost + newFeatureCost > (int)maxPoint;
						if (flag4)
						{
							return;
						}
					}
					else
					{
						bool flag5 = this._spentPoints + newFeatureCost > (int)maxPoint;
						if (flag5)
						{
							return;
						}
					}
					this._selectedFeatureByGroup[groupId] = featureId;
					this.OnFeatureSelectionChanged();
				}
			}
		}

		// Token: 0x06006467 RID: 25703 RVA: 0x002DF52C File Offset: 0x002DD72C
		public void RemoveFeature(short featureId)
		{
			CharacterFeatureItem featureItem;
			bool flag = !this.TryGetDisplayableFeature(featureId, out featureItem);
			if (!flag)
			{
				short groupId = NewGameSubPageCustomPresetFeature.ResolveGroupId(featureItem);
				bool flag2 = !this._selectedFeatureByGroup.Remove(groupId);
				if (!flag2)
				{
					this.OnFeatureSelectionChanged();
				}
			}
		}

		// Token: 0x06006468 RID: 25704 RVA: 0x002DF570 File Offset: 0x002DD770
		public void ToggleFeatureGroup(short groupId)
		{
			NewGameSubPageCustomPresetFeature.FeatureGroupData groupData;
			bool flag = !this._featureGroupMap.TryGetValue(groupId, out groupData);
			if (!flag)
			{
				this.OpenFeatureSelection(groupData);
			}
		}

		// Token: 0x06006469 RID: 25705 RVA: 0x002DF5A0 File Offset: 0x002DD7A0
		private void EnsureUiInitialized()
		{
			bool uiInitialized = this._uiInitialized;
			if (!uiInitialized)
			{
				this.BuildFeatureGroups();
				this.featureResetButton.onClick.ResetListener(new Action(this.ResetFeatures));
				this.featureScroll.OnItemRender += this.OnFeatureItemRender;
				this.featureSelectionPanelCompact.gameObject.SetActive(false);
				this.featureSelectionPanelExpanded.gameObject.SetActive(false);
				this._uiInitialized = true;
			}
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x002DF624 File Offset: 0x002DD824
		private void BuildRandomCandidates(List<NewGameSubPageCustomPresetFeature.FeatureRandomCandidate> candidates, int remainingPoints)
		{
			candidates.Clear();
			sbyte currentGender = this.ParentCustomPresetPage.CurrentGender;
			for (int i = 0; i < this._featureGroups.Count; i++)
			{
				NewGameSubPageCustomPresetFeature.FeatureGroupData groupData = this._featureGroups[i];
				short currentSelectedFeatureId = this.GetSelectedFeatureId(groupData.GroupId);
				int currentCost = (currentSelectedFeatureId >= 0) ? NewGameSubPageCustomPresetFeature.GetFeatureCost(CharacterFeature.Instance[currentSelectedFeatureId].Level) : 0;
				for (int j = 0; j < groupData.Features.Count; j++)
				{
					CharacterFeatureItem feature = groupData.Features[j];
					bool flag = feature.TemplateId == currentSelectedFeatureId;
					if (!flag)
					{
						bool flag2 = feature.Gender != -1 && feature.Gender != currentGender;
						if (!flag2)
						{
							int costDelta = NewGameSubPageCustomPresetFeature.GetFeatureCost(feature.Level) - currentCost;
							bool flag3 = costDelta <= 0 || costDelta > remainingPoints;
							if (!flag3)
							{
								candidates.Add(new NewGameSubPageCustomPresetFeature.FeatureRandomCandidate(groupData.GroupId, feature.TemplateId, costDelta));
							}
						}
					}
				}
			}
		}

		// Token: 0x0600646B RID: 25707 RVA: 0x002DF750 File Offset: 0x002DD950
		private void BuildFeatureGroups()
		{
			this._featureGroups.Clear();
			this._featureGroupMap.Clear();
			Dictionary<short, List<CharacterFeatureItem>> groupDict = new Dictionary<short, List<CharacterFeatureItem>>();
			List<short> allKeys = CharacterFeature.Instance.GetAllKeys();
			foreach (short key in allKeys)
			{
				CharacterFeatureItem featureItem = CharacterFeature.Instance[key];
				bool flag = !NewGameSubPageCustomPresetFeature.ShouldDisplayFeature(featureItem);
				if (!flag)
				{
					short groupId = NewGameSubPageCustomPresetFeature.ResolveGroupId(featureItem);
					List<CharacterFeatureItem> featureList;
					bool flag2 = !groupDict.TryGetValue(groupId, out featureList);
					if (flag2)
					{
						featureList = new List<CharacterFeatureItem>();
						groupDict[groupId] = featureList;
					}
					featureList.Add(featureItem);
				}
			}
			foreach (KeyValuePair<short, List<CharacterFeatureItem>> pair in groupDict)
			{
				pair.Value.Sort(new Comparison<CharacterFeatureItem>(NewGameSubPageCustomPresetFeature.CompareFeatureByLevel));
				NewGameSubPageCustomPresetFeature.FeatureGroupData groupData = new NewGameSubPageCustomPresetFeature.FeatureGroupData(pair.Key, pair.Value);
				this._featureGroups.Add(groupData);
				this._featureGroupMap[pair.Key] = groupData;
			}
			this._featureGroups.Sort(new Comparison<NewGameSubPageCustomPresetFeature.FeatureGroupData>(NewGameSubPageCustomPresetFeature.CompareFeatureGroup));
		}

		// Token: 0x0600646C RID: 25708 RVA: 0x002DF8C4 File Offset: 0x002DDAC4
		private static int CompareFeatureByLevel(CharacterFeatureItem left, CharacterFeatureItem right)
		{
			int compareByLevel = left.Level.CompareTo(right.Level);
			bool flag = compareByLevel != 0;
			int result;
			if (flag)
			{
				result = compareByLevel;
			}
			else
			{
				result = left.TemplateId.CompareTo(right.TemplateId);
			}
			return result;
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x002DF908 File Offset: 0x002DDB08
		private static CharacterFeatureItem GetDefaultFeatureByMinAbsLevel(List<CharacterFeatureItem> features)
		{
			CharacterFeatureItem defaultFeature = features[0];
			int minAbsLevel = Mathf.Abs((int)defaultFeature.Level);
			for (int i = 1; i < features.Count; i++)
			{
				CharacterFeatureItem currentFeature = features[i];
				int currentAbsLevel = Mathf.Abs((int)currentFeature.Level);
				bool flag = currentAbsLevel > minAbsLevel;
				if (!flag)
				{
					bool flag2 = currentAbsLevel == minAbsLevel && currentFeature.TemplateId > defaultFeature.TemplateId;
					if (!flag2)
					{
						defaultFeature = currentFeature;
						minAbsLevel = currentAbsLevel;
					}
				}
			}
			return defaultFeature;
		}

		// Token: 0x0600646E RID: 25710 RVA: 0x002DF990 File Offset: 0x002DDB90
		private static int CompareFeatureGroup(NewGameSubPageCustomPresetFeature.FeatureGroupData left, NewGameSubPageCustomPresetFeature.FeatureGroupData right)
		{
			int leftExtraIndex = NewGameSubPageCustomPresetFeature.GetExtraGroupIndex(left.GroupId);
			int rightExtraIndex = NewGameSubPageCustomPresetFeature.GetExtraGroupIndex(right.GroupId);
			bool flag = leftExtraIndex >= 0 && rightExtraIndex >= 0;
			int result;
			if (flag)
			{
				result = leftExtraIndex.CompareTo(rightExtraIndex);
			}
			else
			{
				bool flag2 = leftExtraIndex >= 0;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = rightExtraIndex >= 0;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						int leftTypeOrder = NewGameSubPageCustomPresetFeature.GetTypeOrder(left.DefaultFeature.Type);
						int rightTypeOrder = NewGameSubPageCustomPresetFeature.GetTypeOrder(right.DefaultFeature.Type);
						bool flag4 = leftTypeOrder != rightTypeOrder;
						if (flag4)
						{
							result = leftTypeOrder.CompareTo(rightTypeOrder);
						}
						else
						{
							int compareByPriority = left.DefaultFeature.DisplayPriority.CompareTo(right.DefaultFeature.DisplayPriority);
							bool flag5 = compareByPriority != 0;
							if (flag5)
							{
								result = compareByPriority;
							}
							else
							{
								result = left.GroupId.CompareTo(right.GroupId);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600646F RID: 25711 RVA: 0x002DFA88 File Offset: 0x002DDC88
		private static int GetTypeOrder(ECharacterFeatureType featureType)
		{
			if (!true)
			{
			}
			int result;
			if (featureType != ECharacterFeatureType.Good)
			{
				if (featureType != ECharacterFeatureType.Bad)
				{
					result = 2;
				}
				else
				{
					result = 1;
				}
			}
			else
			{
				result = 0;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006470 RID: 25712 RVA: 0x002DFABC File Offset: 0x002DDCBC
		private static int GetExtraGroupIndex(short groupId)
		{
			for (int i = 0; i < NewGameSubPageCustomPresetFeature.ExtraAllowedMutexGroupIds.Length; i++)
			{
				bool flag = NewGameSubPageCustomPresetFeature.ExtraAllowedMutexGroupIds[i] == groupId;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06006471 RID: 25713 RVA: 0x002DFAFC File Offset: 0x002DDCFC
		private static bool IsExtraAllowedMutexGroup(short groupId)
		{
			return NewGameSubPageCustomPresetFeature.GetExtraGroupIndex(groupId) >= 0;
		}

		// Token: 0x06006472 RID: 25714 RVA: 0x002DFB1C File Offset: 0x002DDD1C
		private static bool ShouldDisplayFeature(CharacterFeatureItem featureItem)
		{
			bool hidden = featureItem.Hidden;
			bool result;
			if (hidden)
			{
				result = false;
			}
			else
			{
				ECharacterFeatureType type = featureItem.Type;
				bool flag = type == ECharacterFeatureType.Good || type == ECharacterFeatureType.Bad;
				result = (flag || NewGameSubPageCustomPresetFeature.IsExtraAllowedMutexGroup(featureItem.MutexGroupId));
			}
			return result;
		}

		// Token: 0x06006473 RID: 25715 RVA: 0x002DFB68 File Offset: 0x002DDD68
		private static short ResolveGroupId(CharacterFeatureItem featureItem)
		{
			return featureItem.MutexGroupId;
		}

		// Token: 0x06006474 RID: 25716 RVA: 0x002DFB80 File Offset: 0x002DDD80
		private static int GetFeatureCost(sbyte level)
		{
			int absLevel = Mathf.Abs((int)level);
			return (absLevel == 1 || absLevel == 2 || absLevel == 3) ? absLevel : 0;
		}

		// Token: 0x06006475 RID: 25717 RVA: 0x002DFBAC File Offset: 0x002DDDAC
		private bool TryGetDisplayableFeature(short featureId, out CharacterFeatureItem featureItem)
		{
			featureItem = CharacterFeature.Instance.GetItem(featureId);
			bool flag = featureItem == null;
			return !flag && NewGameSubPageCustomPresetFeature.ShouldDisplayFeature(featureItem);
		}

		// Token: 0x06006476 RID: 25718 RVA: 0x002DFBE0 File Offset: 0x002DDDE0
		private short GetSelectedFeatureId(short groupId)
		{
			short featureId;
			return this._selectedFeatureByGroup.TryGetValue(groupId, out featureId) ? featureId : -1;
		}

		// Token: 0x06006477 RID: 25719 RVA: 0x002DFC08 File Offset: 0x002DDE08
		private void RecalculateSpentPoints()
		{
			int total = 0;
			foreach (short featureId in this._selectedFeatureByGroup.Values)
			{
				CharacterFeatureItem item = CharacterFeature.Instance.GetItem(featureId);
				bool flag = item == null;
				if (!flag)
				{
					total += NewGameSubPageCustomPresetFeature.GetFeatureCost(item.Level);
				}
			}
			this._spentPoints = total;
		}

		// Token: 0x06006478 RID: 25720 RVA: 0x002DFC90 File Offset: 0x002DDE90
		private void RefreshFeatureUI()
		{
			short totalPoint = GlobalConfig.Instance.CustomProtagonistCharacterFeatureTotalPoint;
			this.featurePointText.text = string.Format("{0}/{1}", ((int)totalPoint - this._spentPoints).ToString().SetColor("brightblue"), totalPoint);
			this.featureScroll.UpdateData(this._featureGroups.Count);
		}

		// Token: 0x06006479 RID: 25721 RVA: 0x002DFCF8 File Offset: 0x002DDEF8
		private void OpenFeatureSelection(NewGameSubPageCustomPresetFeature.FeatureGroupData groupData)
		{
			this._activeSelectionGroupId = groupData.GroupId;
			short currentSelectedFeatureId = this.GetSelectedFeatureId(groupData.GroupId);
			int currentSelectedCost = (currentSelectedFeatureId >= 0) ? NewGameSubPageCustomPresetFeature.GetFeatureCost(CharacterFeature.Instance[currentSelectedFeatureId].Level) : 0;
			int availablePoints = this.RemainingPoints + currentSelectedCost;
			short totalPoints = GlobalConfig.Instance.CustomProtagonistCharacterFeatureTotalPoint;
			NewGameCustomPresetFeatureSelectionPanel targetPanel = (groupData.Features.Count <= 6) ? this.featureSelectionPanelCompact : this.featureSelectionPanelExpanded;
			bool flag = this._activeSelectionPanel != null && this._activeSelectionPanel != targetPanel;
			if (flag)
			{
				UIManager.Instance.UnMaskComponent(NewGameSubPageCustomPresetFeature.GetPanelRectTransform(this._activeSelectionPanel));
			}
			this._activeSelectionPanel = targetPanel;
			this._activeSelectionPanel.Open(groupData.Features, currentSelectedFeatureId, availablePoints, (int)totalPoints, new Action<short>(this.OnConfirmFeatureSelection), new Action(this.OnCancelFeatureSelection), this.ParentCustomPresetPage.CurrentGender);
			UIManager.Instance.MaskComponent(NewGameSubPageCustomPresetFeature.GetPanelRectTransform(this._activeSelectionPanel));
		}

		// Token: 0x0600647A RID: 25722 RVA: 0x002DFE00 File Offset: 0x002DE000
		internal void CloseFeatureSelection()
		{
			bool flag = this._activeSelectionPanel != null;
			if (flag)
			{
				UIManager.Instance.UnMaskComponent(NewGameSubPageCustomPresetFeature.GetPanelRectTransform(this._activeSelectionPanel));
			}
			this._activeSelectionPanel = null;
			this._activeSelectionGroupId = -1;
			AudioManager.Instance.PlaySound("ui_default_small_back", false, false);
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x002DFE54 File Offset: 0x002DE054
		private void OnCancelFeatureSelection()
		{
			this.CloseFeatureSelection();
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x002DFE60 File Offset: 0x002DE060
		private static RectTransform GetPanelRectTransform(NewGameCustomPresetFeatureSelectionPanel panel)
		{
			return (RectTransform)panel.transform;
		}

		// Token: 0x0600647D RID: 25725 RVA: 0x002DFE80 File Offset: 0x002DE080
		private void OnConfirmFeatureSelection(short selectedFeatureId)
		{
			short groupId = this._activeSelectionGroupId;
			bool flag = groupId < 0;
			if (flag)
			{
				this.CloseFeatureSelection();
			}
			else
			{
				short oldSelectedFeatureId = this.GetSelectedFeatureId(groupId);
				this.CloseFeatureSelection();
				bool flag2 = selectedFeatureId == oldSelectedFeatureId;
				if (!flag2)
				{
					bool flag3 = selectedFeatureId >= 0;
					if (flag3)
					{
						this.AddFeature(selectedFeatureId);
					}
					else
					{
						bool flag4 = oldSelectedFeatureId >= 0;
						if (flag4)
						{
							this.RemoveFeature(oldSelectedFeatureId);
						}
					}
				}
			}
		}

		// Token: 0x0600647E RID: 25726 RVA: 0x002DFEEC File Offset: 0x002DE0EC
		private void OnFeatureItemRender(int index, GameObject itemObj)
		{
			bool flag = index < 0 || index >= this._featureGroups.Count;
			if (!flag)
			{
				NewGameCustomPresetFeatureItem itemView = itemObj.GetComponent<NewGameCustomPresetFeatureItem>();
				NewGameSubPageCustomPresetFeature.FeatureGroupData groupData = this._featureGroups[index];
				short selectedFeatureId = this.GetSelectedFeatureId(groupData.GroupId);
				bool isSelected = selectedFeatureId >= 0;
				CharacterFeatureItem displayFeature = isSelected ? CharacterFeature.Instance[selectedFeatureId] : groupData.DefaultFeature;
				sbyte currentGender = this.ParentCustomPresetPage.CurrentGender;
				int displayCost = NewGameSubPageCustomPresetFeature.GetFeatureCost(displayFeature.Level);
				bool canSelectByGender = NewGameSubPageCustomPresetFeature.HasGenderMatchFeature(groupData.Features, currentGender);
				bool canSelectByCost = isSelected || NewGameSubPageCustomPresetFeature.CanSelectAnyFeature(groupData.Features, this.RemainingPoints);
				bool canSelect = canSelectByGender && canSelectByCost;
				itemView.Initialize(groupData.GroupId, new Action<short>(this.ToggleFeatureGroup));
				itemView.RefreshItem(displayFeature, displayCost, isSelected, !canSelect);
			}
		}

		// Token: 0x0600647F RID: 25727 RVA: 0x002DFFD4 File Offset: 0x002DE1D4
		private static bool HasGenderMatchFeature(List<CharacterFeatureItem> features, sbyte currentGender)
		{
			for (int i = 0; i < features.Count; i++)
			{
				CharacterFeatureItem feature = features[i];
				bool flag = feature.Gender == -1 || feature.Gender == currentGender;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006480 RID: 25728 RVA: 0x002E0028 File Offset: 0x002DE228
		private static bool CanSelectAnyFeature(List<CharacterFeatureItem> features, int remainingPoints)
		{
			for (int i = 0; i < features.Count; i++)
			{
				bool flag = NewGameSubPageCustomPresetFeature.GetFeatureCost(features[i].Level) <= remainingPoints;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006481 RID: 25729 RVA: 0x002E0074 File Offset: 0x002DE274
		private void OnFeatureSelectionChanged()
		{
			int oldSpent = this._spentPoints;
			this.RecalculateSpentPoints();
			bool flag = oldSpent != this._spentPoints;
			if (flag)
			{
				this.ParentCustomPresetPage.OnSubPagePointsChanged();
			}
			base.NotifyDataModified();
			this.RefreshUI();
			this.RefreshFeatureResetButton();
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x002E00C1 File Offset: 0x002DE2C1
		private void RefreshFeatureResetButton()
		{
			this.featureResetButton.interactable = (this._selectedFeatureByGroup.Count > 0);
		}

		// Token: 0x04004606 RID: 17926
		[Header("特性UI")]
		[SerializeField]
		private TextMeshProUGUI featurePointText;

		// Token: 0x04004607 RID: 17927
		[SerializeField]
		private InfinityScroll featureScroll;

		// Token: 0x04004608 RID: 17928
		[SerializeField]
		private CButton featureResetButton;

		// Token: 0x04004609 RID: 17929
		[Header("特性选择内嵌界面")]
		[SerializeField]
		private NewGameCustomPresetFeatureSelectionPanel featureSelectionPanelCompact;

		// Token: 0x0400460A RID: 17930
		[SerializeField]
		private NewGameCustomPresetFeatureSelectionPanel featureSelectionPanelExpanded;

		// Token: 0x0400460B RID: 17931
		private readonly Dictionary<short, short> _selectedFeatureByGroup = new Dictionary<short, short>();

		// Token: 0x0400460C RID: 17932
		private readonly List<NewGameSubPageCustomPresetFeature.FeatureGroupData> _featureGroups = new List<NewGameSubPageCustomPresetFeature.FeatureGroupData>();

		// Token: 0x0400460D RID: 17933
		private readonly Dictionary<short, NewGameSubPageCustomPresetFeature.FeatureGroupData> _featureGroupMap = new Dictionary<short, NewGameSubPageCustomPresetFeature.FeatureGroupData>();

		// Token: 0x0400460E RID: 17934
		private int _spentPoints;

		// Token: 0x0400460F RID: 17935
		private bool _uiInitialized;

		// Token: 0x04004610 RID: 17936
		private short _activeSelectionGroupId = -1;

		// Token: 0x04004611 RID: 17937
		private NewGameCustomPresetFeatureSelectionPanel _activeSelectionPanel;

		// Token: 0x04004612 RID: 17938
		private static readonly short[] ExtraAllowedMutexGroupIds = new short[]
		{
			172,
			168
		};

		// Token: 0x02001D40 RID: 7488
		private readonly struct FeatureRandomCandidate
		{
			// Token: 0x170018AC RID: 6316
			// (get) Token: 0x0600ECA5 RID: 60581 RVA: 0x006062DC File Offset: 0x006044DC
			public short GroupId { get; }

			// Token: 0x170018AD RID: 6317
			// (get) Token: 0x0600ECA6 RID: 60582 RVA: 0x006062E4 File Offset: 0x006044E4
			public short FeatureId { get; }

			// Token: 0x170018AE RID: 6318
			// (get) Token: 0x0600ECA7 RID: 60583 RVA: 0x006062EC File Offset: 0x006044EC
			public int CostDelta { get; }

			// Token: 0x0600ECA8 RID: 60584 RVA: 0x006062F4 File Offset: 0x006044F4
			public FeatureRandomCandidate(short groupId, short featureId, int costDelta)
			{
				this.GroupId = groupId;
				this.FeatureId = featureId;
				this.CostDelta = costDelta;
			}
		}

		// Token: 0x02001D41 RID: 7489
		private sealed class FeatureGroupData
		{
			// Token: 0x170018AF RID: 6319
			// (get) Token: 0x0600ECA9 RID: 60585 RVA: 0x0060630C File Offset: 0x0060450C
			public short GroupId { get; }

			// Token: 0x170018B0 RID: 6320
			// (get) Token: 0x0600ECAA RID: 60586 RVA: 0x00606314 File Offset: 0x00604514
			public CharacterFeatureItem DefaultFeature { get; }

			// Token: 0x170018B1 RID: 6321
			// (get) Token: 0x0600ECAB RID: 60587 RVA: 0x0060631C File Offset: 0x0060451C
			public List<CharacterFeatureItem> Features { get; }

			// Token: 0x0600ECAC RID: 60588 RVA: 0x00606324 File Offset: 0x00604524
			public FeatureGroupData(short groupId, List<CharacterFeatureItem> features)
			{
				this.GroupId = groupId;
				this.Features = features;
				this.DefaultFeature = NewGameSubPageCustomPresetFeature.GetDefaultFeatureByMinAbsLevel(features);
			}
		}
	}
}
