using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using FrameWork.Tools.Random;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Creation;
using GameData.Domains.Global;
using GameData.Domains.World;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x02000801 RID: 2049
	public class NewGameSubPageCustomPreset : NewGameSubPage
	{
		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x060063FC RID: 25596 RVA: 0x002DD2D0 File Offset: 0x002DB4D0
		private NewGameSubPageCustomPresetSelectOrganization SelectOrganizationSubPage
		{
			get
			{
				bool flag = this._selectOrganizationSubPage == null;
				if (flag)
				{
					this._selectOrganizationSubPage = base.GetComponentInChildren<NewGameSubPageCustomPresetSelectOrganization>(true);
				}
				return this._selectOrganizationSubPage;
			}
		}

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x060063FD RID: 25597 RVA: 0x002DD305 File Offset: 0x002DB505
		private int TotalPoints
		{
			get
			{
				return NewGameSubPageCustomPreset.GetTotalAllocatablePoints();
			}
		}

		// Token: 0x060063FE RID: 25598 RVA: 0x002DD30C File Offset: 0x002DB50C
		private static int GetTotalAllocatablePoints()
		{
			GlobalConfig global = GlobalConfig.Instance;
			return (int)(global.CustomProtagonistMainAttributeTotalPoint + global.CustomProtagonistLifeSkillQualificationTotalPoint + global.CustomProtagonistCombatSkillQualificationTotalPoint + global.CustomProtagonistCharacterFeatureTotalPoint);
		}

		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x060063FF RID: 25599 RVA: 0x002DD33F File Offset: 0x002DB53F
		public int RemainingPoints
		{
			get
			{
				return this.TotalPoints - this._spentPoints;
			}
		}

		// Token: 0x06006400 RID: 25600 RVA: 0x002DD34E File Offset: 0x002DB54E
		public void OnSubPagePointsChanged()
		{
			this.RecalculateSpentPointsFromSubPages();
			this.RefreshToggleDisplay();
			base.RefreshDisableEnterGameReason();
		}

		// Token: 0x06006401 RID: 25601 RVA: 0x002DD368 File Offset: 0x002DB568
		private void RecalculateSpentPointsFromSubPages()
		{
			int totalSpent = 0;
			foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
			{
				bool flag = page != null;
				if (flag)
				{
					totalSpent += page.SpentPoints;
				}
			}
			this._spentPoints = Mathf.Clamp(totalSpent, 0, this.TotalPoints);
		}

		// Token: 0x06006402 RID: 25602 RVA: 0x002DD3BD File Offset: 0x002DB5BD
		private void RefreshToggleDisplay()
		{
			NewGameSubPageCustomPresetToggleHelper newGameSubPageCustomPresetToggleHelper = this.toggleHelper;
			if (newGameSubPageCustomPresetToggleHelper != null)
			{
				newGameSubPageCustomPresetToggleHelper.RefreshPoints(this.RemainingPoints, this.TotalPoints);
			}
			this.RefreshSubPageTogglePoints();
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x002DD3E8 File Offset: 0x002DB5E8
		private void RefreshSubPageTogglePoints()
		{
			List<CToggle> toggleList = this.subPageToggleGroup.GetAll();
			int i = 0;
			while (i < this.subPages.Length && i < toggleList.Count)
			{
				SubPageCustomPresetToggle subPageToggle = toggleList[i] as SubPageCustomPresetToggle;
				bool flag = subPageToggle != null;
				if (flag)
				{
					subPageToggle.RefreshPointsDisplay(this.subPages[i].RemainingPoints);
				}
				i++;
			}
		}

		// Token: 0x06006404 RID: 25604 RVA: 0x002DD454 File Offset: 0x002DB654
		protected override void Awake()
		{
			base.Awake();
			this.useCustomPresetToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnUseCustomPresetToggleChanged));
			this.presetToggleGroup.Init(-1);
			this.presetToggleGroup.OnActiveIndexChange += this.OnPresetToggleChanged;
			this.addPresetButton.onClick.ResetListener(new Action(this.OnAddPreset));
			this.clonePresetButton.onClick.ResetListener(new Action(this.OnClonePreset));
			this.clearPresetButton.onClick.ResetListener(new Action(this.OnClearPreset));
			this.deletePresetButton.onClick.ResetListener(new Action(this.OnDeletePreset));
			this.resetButton.onClick.ResetListener(new Action(this.OnReset));
			this.randomButton.onClick.ResetListener(new Action(this.OnRandom));
			this.InitializeSubPages();
			this.InitializeSubPageToggleGroup();
		}

		// Token: 0x06006405 RID: 25605 RVA: 0x002DD568 File Offset: 0x002DB768
		private void InitializeSubPageToggleGroup()
		{
			this.subPageToggleGroup.Init(-1);
			List<CToggle> toggleList = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggleList.Count; i++)
			{
				SubPageCustomPresetToggle subPageToggle = toggleList[i] as SubPageCustomPresetToggle;
				bool flag = subPageToggle != null;
				if (flag)
				{
					subPageToggle.SetSubPageIndex(i);
				}
			}
			this.subPageToggleGroup.OnActiveIndexChange += this.OnSubPageToggleChanged;
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x002DD5E0 File Offset: 0x002DB7E0
		private void OnSubPageToggleChanged(int newIndex, int oldIndex)
		{
			bool flag = newIndex == oldIndex;
			if (!flag)
			{
				this.SwitchToSubPage((NewGameSubPageCustomPreset.ESubPageType)newIndex, false, false);
			}
		}

		// Token: 0x06006407 RID: 25607 RVA: 0x002DD604 File Offset: 0x002DB804
		public override void Init()
		{
			base.Init();
			this.SwitchToToggle(0);
			this.RefreshPresetTogglesVisibility();
			this.ResetSpentPoints();
			this.RefreshAllSubPages();
			this.RefreshCustomPresetOffOverlay();
			this.RefreshPresetButtons();
			this.SwitchToSubPage(NewGameSubPageCustomPreset.ESubPageType.Template, false, false);
			this.subPageToggleGroup.Set(0, false);
			this.RefreshToggleDisplay();
			this.warningTips.SetActive(!this.useCustomPresetToggle.isOn);
			string customTips = this.useCustomPresetToggle.isOn ? LanguageKey.LK_NewGame_CustomPreset_Toggle.Tr() : LanguageKey.LK_NewGame_CustomPreset_Off.Tr();
			this.customTipsText.SetText(customTips.SetColor(this.useCustomPresetToggle.isOn ? "pinkyellow" : "lowwarning").ColorReplace(), true);
		}

		// Token: 0x06006408 RID: 25608 RVA: 0x002DD6D4 File Offset: 0x002DB8D4
		private void OnUseCustomPresetToggleChanged(bool isOn)
		{
			bool flag = this.customToggleParticle != null;
			if (flag)
			{
				this.customToggleParticle.gameObject.SetActive(!isOn);
			}
			this._useCustomPreset = isOn;
			this.RefreshCustomPresetOffOverlay();
			base.RefreshDisableEnterGameReason();
			if (isOn)
			{
				foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
				{
					NewGameSubPageCustomPresetNeili neiliPage = page as NewGameSubPageCustomPresetNeili;
					bool flag2 = neiliPage != null;
					if (flag2)
					{
						neiliPage.ApplyRecommendedNeiliType();
					}
				}
			}
			this.warningTips.SetActive(!isOn);
			string customTips = isOn ? LanguageKey.LK_NewGame_CustomPreset_Toggle.Tr() : LanguageKey.LK_NewGame_CustomPreset_Off.Tr();
			this.customTipsText.SetText(customTips.SetColor(isOn ? "pinkyellow" : "lowwarning").ColorReplace(), true);
			Action<bool> onUseCustomPresetToggleChangedOverEvent = this.OnUseCustomPresetToggleChangedOverEvent;
			if (onUseCustomPresetToggleChangedOverEvent != null)
			{
				onUseCustomPresetToggleChangedOverEvent(isOn);
			}
		}

		// Token: 0x06006409 RID: 25609 RVA: 0x002DD7C5 File Offset: 0x002DB9C5
		private void RefreshCustomPresetOffOverlay()
		{
			this.customPresetOffOverlay.SetActive(!this._useCustomPreset);
		}

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x0600640A RID: 25610 RVA: 0x002DD7DD File Offset: 0x002DB9DD
		public sbyte CurrentGender
		{
			get
			{
				return this.parent.Gender;
			}
		}

		// Token: 0x0600640B RID: 25611 RVA: 0x002DD7EC File Offset: 0x002DB9EC
		private void SwitchToToggle(int toggleIndex)
		{
			this._currentToggleIndex = toggleIndex;
			CustomProtagonistPreset preset = NewGameCustomPresetHelper.GetCustomProtagonistPreset();
			bool flag = toggleIndex >= 0 && toggleIndex < preset.Presets.Count;
			if (flag)
			{
				this.ApplyDataToSubPages(preset.Presets[toggleIndex]);
			}
		}

		// Token: 0x0600640C RID: 25612 RVA: 0x002DD834 File Offset: 0x002DBA34
		private void OnPresetToggleChanged(int newIndex, int oldIndex)
		{
			bool flag = newIndex == oldIndex;
			if (!flag)
			{
				bool isRestoringToggle = this._isRestoringToggle;
				if (!isRestoringToggle)
				{
					bool flag2 = newIndex < 0;
					if (flag2)
					{
						this._isRestoringToggle = true;
						try
						{
							this.presetToggleGroup.SetWithoutNotify(oldIndex);
						}
						finally
						{
							this._isRestoringToggle = false;
						}
					}
					else
					{
						int presetCount = NewGameCustomPresetHelper.GetPresetCount();
						bool flag3 = newIndex < 0 || newIndex >= presetCount;
						if (flag3)
						{
							this._isRestoringToggle = true;
							try
							{
								this.presetToggleGroup.SetWithoutNotify(oldIndex);
							}
							finally
							{
								this._isRestoringToggle = false;
							}
						}
						else
						{
							NewGameCustomPresetHelper.ChangePreset(newIndex);
							this.SwitchToToggle(newIndex);
							this.RefreshPresetButtons();
							NewGameSubPageCustomPresetSelectOrganization selectOrganizationSubPage = this.SelectOrganizationSubPage;
							if (selectOrganizationSubPage != null)
							{
								selectOrganizationSubPage.SetToCustom();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600640D RID: 25613 RVA: 0x002DD910 File Offset: 0x002DBB10
		private void OnAddPreset()
		{
			CustomProtagonistPreset preset = NewGameCustomPresetHelper.GetCustomProtagonistPreset();
			bool flag = !preset.CanAdd;
			if (!flag)
			{
				NewGameCustomPresetHelper.AddPreset();
				CustomProtagonistPresetItem newItem = NewGameCustomPresetHelper.GetCurrentPresetItem();
				newItem.Clear();
				newItem.ResetMainAttributes();
				newItem.ResetQualifications();
				NewGameCustomPresetHelper.UpdateCurrentPresetItem(newItem);
				int newPresetCount = preset.Presets.Count;
				this.presetToggleGroup.Set(newPresetCount - 1, false);
				this.SwitchToToggle(newPresetCount - 1);
				this.RefreshPresetTogglesVisibility();
			}
		}

		// Token: 0x0600640E RID: 25614 RVA: 0x002DD988 File Offset: 0x002DBB88
		private void OnClonePreset()
		{
			CustomProtagonistPreset preset = NewGameCustomPresetHelper.GetCustomProtagonistPreset();
			bool flag = !preset.CanAdd;
			if (!flag)
			{
				NewGameCustomPresetHelper.ClonePreset();
				int newPresetCount = preset.Presets.Count;
				this.presetToggleGroup.Set(newPresetCount - 1, false);
				this.SwitchToToggle(newPresetCount - 1);
				this.RefreshPresetTogglesVisibility();
			}
		}

		// Token: 0x0600640F RID: 25615 RVA: 0x002DD9E0 File Offset: 0x002DBBE0
		private void OnClearPreset()
		{
			NewGameCustomPresetHelper.ClearPreset();
			CustomProtagonistPresetItem currentItem = NewGameCustomPresetHelper.GetCurrentPresetItem();
			bool flag = currentItem != null;
			if (flag)
			{
				this.ApplyDataToSubPages(currentItem);
			}
		}

		// Token: 0x06006410 RID: 25616 RVA: 0x002DDA0C File Offset: 0x002DBC0C
		private void OnDeletePreset()
		{
			CustomProtagonistPreset preset = NewGameCustomPresetHelper.GetCustomProtagonistPreset();
			bool flag = !preset.CanDelete;
			if (!flag)
			{
				NewGameCustomPresetHelper.DeletePreset();
				int newToggleIndex = Mathf.Max(0, this._currentToggleIndex - 1);
				this.presetToggleGroup.Set(newToggleIndex, false);
				this.RefreshPresetTogglesVisibility();
				this.RefreshPresetButtons();
			}
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x002DDA60 File Offset: 0x002DBC60
		private void OnReset()
		{
			CustomProtagonistPresetItem currentData = new CustomProtagonistPresetItem();
			foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
			{
				bool flag = page != null;
				if (flag)
				{
					page.ApplyToPreset(currentData);
				}
			}
			CustomProtagonistPresetItem defaultData = new CustomProtagonistPresetItem();
			bool flag2 = !currentData.Equals(defaultData);
			if (flag2)
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LanguageKey.LK_NewGame_CustomPreset_BtnReset_Confirm_Title.Tr(),
					Content = LanguageKey.LK_NewGame_CustomPreset_BtnReset_Confirm_Content.Tr(),
					Yes = delegate()
					{
						this.DoReset();
					}
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.DoReset();
			}
		}

		// Token: 0x06006412 RID: 25618 RVA: 0x002DDB2C File Offset: 0x002DBD2C
		private void DoReset()
		{
			CustomProtagonistPresetItem currentData = this.GetCurrentPresetItem();
			currentData.ResetMainAttributes();
			currentData.ResetQualifications();
			List<short> selectedFeatures = currentData.SelectedFeatures;
			if (selectedFeatures != null)
			{
				selectedFeatures.Clear();
			}
			this.ApplyDataToSubPages(currentData);
			this.SaveSubPagesToCurrentPreset();
			NewGameSubPageCustomPresetSelectOrganization selectOrganizationSubPage = this.SelectOrganizationSubPage;
			if (selectOrganizationSubPage != null)
			{
				selectOrganizationSubPage.SetToCustom();
			}
		}

		// Token: 0x06006413 RID: 25619 RVA: 0x002DDB84 File Offset: 0x002DBD84
		private void OnRandom()
		{
			CustomProtagonistPresetItem currentData = new CustomProtagonistPresetItem();
			foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
			{
				bool flag = page != null;
				if (flag)
				{
					page.ApplyToPreset(currentData);
				}
			}
			bool flag2 = this.ShouldShowRandomConfirm(currentData);
			if (flag2)
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LanguageKey.LK_NewGame_CustomPreset_BtnRandom_Confirm_Title.Tr(),
					Content = LanguageKey.LK_NewGame_CustomPreset_BtnRandom_Confirm_Content.Tr(),
					Yes = delegate()
					{
						this.DoRandom();
					}
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.DoRandom();
			}
		}

		// Token: 0x06006414 RID: 25620 RVA: 0x002DDC44 File Offset: 0x002DBE44
		private void DoRandom()
		{
			CustomProtagonistPresetItem currentData = this.GetCurrentPresetItem();
			CustomRandom random = new CustomRandom();
			NewGameSubPageCustomPresetFeature featureSubPage = this.GetFeatureSubPage();
			bool hasRemainingPoints = this.HasAnyRemainingPoints(currentData);
			sbyte randomNeiliType = (sbyte)random.Next(6);
			currentData.NeiliProportion = CustomProtagonistPresetItem.GenerateNeiliProportionByNeiliType(randomNeiliType);
			bool flag = hasRemainingPoints;
			if (flag)
			{
				this.RandomAllocateRemainingMainAttributes(currentData, random);
				this.RandomAllocateRemainingLifeSkillQualifications(currentData, random);
				this.RandomAllocateRemainingCombatSkillQualifications(currentData, random);
				if (featureSubPage != null)
				{
					featureSubPage.RandomAllocateFeatures(currentData, random, false);
				}
			}
			else
			{
				currentData.RandomMainAttributes(random);
				currentData.RandomQualifications(random);
				if (featureSubPage != null)
				{
					featureSubPage.RandomAllocateFeatures(currentData, random, true);
				}
			}
			this.ApplyDataToSubPages(currentData);
			this.SaveSubPagesToCurrentPreset();
			this.RecordLastRandomState(currentData);
		}

		// Token: 0x06006415 RID: 25621 RVA: 0x002DDCF4 File Offset: 0x002DBEF4
		private NewGameSubPageCustomPresetFeature GetFeatureSubPage()
		{
			for (int i = 0; i < this.subPages.Length; i++)
			{
				NewGameSubPageCustomPresetFeature featureSubPage = this.subPages[i] as NewGameSubPageCustomPresetFeature;
				bool flag = featureSubPage != null;
				if (flag)
				{
					return featureSubPage;
				}
			}
			return null;
		}

		// Token: 0x06006416 RID: 25622 RVA: 0x002DDD3C File Offset: 0x002DBF3C
		private bool HasAnyRemainingPoints(CustomProtagonistPresetItem currentData)
		{
			return currentData.MainAttributeRemainPoints > 0 || currentData.LifeSkillQualificationRemainPoints > 0 || currentData.CombatSkillQualificationRemainPoints > 0 || NewGameSubPageCustomPreset.GetFeatureRemainingPoints(currentData) > 0;
		}

		// Token: 0x06006417 RID: 25623 RVA: 0x002DDD78 File Offset: 0x002DBF78
		private unsafe void RandomAllocateRemainingMainAttributes(CustomProtagonistPresetItem currentData, CustomRandom random)
		{
			int remainingPoints = currentData.MainAttributeRemainPoints;
			bool flag = remainingPoints <= 0;
			if (!flag)
			{
				short maxPoint = GlobalConfig.Instance.CustomProtagonistMainAttributeMaxPoint;
				List<sbyte> candidates = new List<sbyte>(6);
				while (remainingPoints > 0)
				{
					candidates.Clear();
					for (int i = 0; i < 6; i++)
					{
						sbyte type = (sbyte)i;
						bool flag2 = *currentData.MainAttributes[(int)type] < maxPoint;
						if (flag2)
						{
							candidates.Add(type);
						}
					}
					bool flag3 = candidates.Count <= 0;
					if (flag3)
					{
						break;
					}
					sbyte selectedType = candidates[random.Next(candidates.Count)];
					*currentData.MainAttributes[(int)selectedType] = *currentData.MainAttributes[(int)selectedType] + 1;
					remainingPoints--;
				}
			}
		}

		// Token: 0x06006418 RID: 25624 RVA: 0x002DDE54 File Offset: 0x002DC054
		private unsafe void RandomAllocateRemainingLifeSkillQualifications(CustomProtagonistPresetItem currentData, CustomRandom random)
		{
			int remainingPoints = currentData.LifeSkillQualificationRemainPoints;
			bool flag = remainingPoints <= 0;
			if (!flag)
			{
				short maxPoint = GlobalConfig.Instance.CustomProtagonistLifeSkillQualificationMaxPoint;
				List<sbyte> candidates = new List<sbyte>(16);
				while (remainingPoints > 0)
				{
					candidates.Clear();
					for (int i = 0; i < 16; i++)
					{
						sbyte type = (sbyte)i;
						bool flag2 = *currentData.LifeSkillQualifications[(int)type] < maxPoint;
						if (flag2)
						{
							candidates.Add(type);
						}
					}
					bool flag3 = candidates.Count <= 0;
					if (flag3)
					{
						break;
					}
					sbyte selectedType = candidates[random.Next(candidates.Count)];
					*currentData.LifeSkillQualifications[(int)selectedType] = *currentData.LifeSkillQualifications[(int)selectedType] + 1;
					remainingPoints--;
				}
			}
		}

		// Token: 0x06006419 RID: 25625 RVA: 0x002DDF30 File Offset: 0x002DC130
		private unsafe void RandomAllocateRemainingCombatSkillQualifications(CustomProtagonistPresetItem currentData, CustomRandom random)
		{
			int remainingPoints = currentData.CombatSkillQualificationRemainPoints;
			bool flag = remainingPoints <= 0;
			if (!flag)
			{
				short maxPoint = GlobalConfig.Instance.CustomProtagonistCombatSkillQualificationMaxPoint;
				List<sbyte> candidates = new List<sbyte>(14);
				while (remainingPoints > 0)
				{
					candidates.Clear();
					for (int i = 0; i < 14; i++)
					{
						sbyte type = (sbyte)i;
						bool flag2 = *currentData.CombatSkillQualifications[(int)type] < maxPoint;
						if (flag2)
						{
							candidates.Add(type);
						}
					}
					bool flag3 = candidates.Count <= 0;
					if (flag3)
					{
						break;
					}
					sbyte selectedType = candidates[random.Next(candidates.Count)];
					*currentData.CombatSkillQualifications[(int)selectedType] = *currentData.CombatSkillQualifications[(int)selectedType] + 1;
					remainingPoints--;
				}
			}
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x002DE00C File Offset: 0x002DC20C
		private static int GetFeatureRemainingPoints(CustomProtagonistPresetItem currentData)
		{
			short totalPoint = GlobalConfig.Instance.CustomProtagonistCharacterFeatureTotalPoint;
			return Mathf.Max(0, (int)totalPoint - NewGameSubPageCustomPreset.GetFeatureSpentPoints(currentData.SelectedFeatures));
		}

		// Token: 0x0600641B RID: 25627 RVA: 0x002DE03C File Offset: 0x002DC23C
		private static int GetFeatureSpentPoints(List<short> selectedFeatures)
		{
			bool flag = selectedFeatures == null || selectedFeatures.Count <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				Dictionary<short, int> groupCostMap = new Dictionary<short, int>();
				for (int i = 0; i < selectedFeatures.Count; i++)
				{
					CharacterFeatureItem featureItem = CharacterFeature.Instance.GetItem(selectedFeatures[i]);
					bool flag2 = featureItem == null;
					if (!flag2)
					{
						short groupId = featureItem.MutexGroupId;
						int cost = NewGameSubPageCustomPreset.GetFeatureCost(featureItem.Level);
						bool flag3 = cost <= 0;
						if (!flag3)
						{
							int oldCost;
							bool flag4 = !groupCostMap.TryGetValue(groupId, out oldCost) || cost > oldCost;
							if (flag4)
							{
								groupCostMap[groupId] = cost;
							}
						}
					}
				}
				int totalCost = 0;
				foreach (KeyValuePair<short, int> pair in groupCostMap)
				{
					totalCost += pair.Value;
				}
				result = totalCost;
			}
			return result;
		}

		// Token: 0x0600641C RID: 25628 RVA: 0x002DE14C File Offset: 0x002DC34C
		private static int GetFeatureCost(sbyte level)
		{
			int absLevel = Mathf.Abs((int)level);
			return (absLevel == 1 || absLevel == 2 || absLevel == 3) ? absLevel : 0;
		}

		// Token: 0x0600641D RID: 25629 RVA: 0x002DE178 File Offset: 0x002DC378
		private CustomProtagonistPresetItem GetCurrentPresetItem()
		{
			return NewGameCustomPresetHelper.GetPresetItem(this._currentToggleIndex) ?? new CustomProtagonistPresetItem();
		}

		// Token: 0x0600641E RID: 25630 RVA: 0x002DE1A0 File Offset: 0x002DC3A0
		public void RefreshPresetTogglesVisibility()
		{
			int presetCount = NewGameCustomPresetHelper.GetPresetCount();
			List<CToggle> toggleList = this.presetToggleGroup.GetAll();
			for (int i = 0; i < toggleList.Count; i++)
			{
				toggleList[i].gameObject.SetActive(i < presetCount);
			}
			this.RefreshPresetButtons();
		}

		// Token: 0x0600641F RID: 25631 RVA: 0x002DE1F4 File Offset: 0x002DC3F4
		private void RefreshPresetButtons()
		{
			CustomProtagonistPreset preset = NewGameCustomPresetHelper.GetCustomProtagonistPreset();
			this.deletePresetButton.interactable = preset.CanDelete;
			this.clonePresetButton.interactable = preset.CanAdd;
			this.addPresetButton.interactable = preset.CanAdd;
			this.RefreshResetButton();
		}

		// Token: 0x06006420 RID: 25632 RVA: 0x002DE248 File Offset: 0x002DC448
		private unsafe void RefreshResetButton()
		{
			CustomProtagonistPresetItem currentData = new CustomProtagonistPresetItem();
			foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
			{
				bool flag = page != null;
				if (flag)
				{
					page.ApplyToPreset(currentData);
				}
			}
			GlobalConfig global = GlobalConfig.Instance;
			short attrDefault = global.CustomProtagonistMainAttributeDefaultPoint;
			for (sbyte i = 0; i < 6; i += 1)
			{
				bool flag2 = *currentData.MainAttributes[(int)i] != attrDefault;
				if (flag2)
				{
					this.resetButton.interactable = true;
					return;
				}
			}
			short lifeSkillDefault = global.CustomProtagonistLifeSkillQualificationDefaultPoint;
			for (sbyte j = 0; j < 16; j += 1)
			{
				bool flag3 = *currentData.LifeSkillQualifications[(int)j] != lifeSkillDefault;
				if (flag3)
				{
					this.resetButton.interactable = true;
					return;
				}
			}
			short combatSkillDefault = global.CustomProtagonistCombatSkillQualificationDefaultPoint;
			for (sbyte k = 0; k < 14; k += 1)
			{
				bool flag4 = *currentData.CombatSkillQualifications[(int)k] != combatSkillDefault;
				if (flag4)
				{
					this.resetButton.interactable = true;
					return;
				}
			}
			bool flag5 = currentData.LifeSkillQualificationGrowthType != 0 || currentData.CombatSkillQualificationGrowthType != 0;
			if (flag5)
			{
				this.resetButton.interactable = true;
				return;
			}
			List<short> selectedFeatures = currentData.SelectedFeatures;
			bool flag6 = selectedFeatures != null && selectedFeatures.Count > 0;
			if (flag6)
			{
				this.resetButton.interactable = true;
				return;
			}
			this.resetButton.interactable = false;
		}

		// Token: 0x06006421 RID: 25633 RVA: 0x002DE3EC File Offset: 0x002DC5EC
		public void ApplyDataToSubPages(CustomProtagonistPresetItem presetItem)
		{
			foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
			{
				bool flag = page != null;
				if (flag)
				{
					page.ApplyFromPreset(presetItem);
				}
			}
			this.RecalculateSpentPointsFromSubPages();
			this.RefreshToggleDisplay();
			this.RefreshAllSubPages();
			this.RefreshResetButton();
		}

		// Token: 0x06006422 RID: 25634 RVA: 0x002DE448 File Offset: 0x002DC648
		private void SaveSubPagesToCurrentPreset()
		{
			CustomProtagonistPresetItem item = NewGameCustomPresetHelper.GetPresetItem(this._currentToggleIndex);
			bool flag = item != null;
			if (flag)
			{
				foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
				{
					bool flag2 = page != null;
					if (flag2)
					{
						page.ApplyToPreset(item);
					}
				}
				NewGameCustomPresetHelper.UpdatePresetItem(this._currentToggleIndex, item);
			}
		}

		// Token: 0x06006423 RID: 25635 RVA: 0x002DE4AD File Offset: 0x002DC6AD
		public void ApplyOrgPresetData(CustomProtagonistPresetItem orgData)
		{
			this.ApplyDataToSubPages(orgData);
			this.SaveSubPagesToCurrentPreset();
		}

		// Token: 0x06006424 RID: 25636 RVA: 0x002DE4BF File Offset: 0x002DC6BF
		public void OnSubPageDataModified()
		{
			this.SaveSubPagesToCurrentPreset();
			this.RefreshResetButton();
		}

		// Token: 0x06006425 RID: 25637 RVA: 0x002DE4D0 File Offset: 0x002DC6D0
		public bool TryHandleFeatureSelectionEsc()
		{
			for (int i = 0; i < this.subPages.Length; i++)
			{
				NewGameSubPageCustomPresetFeature featureSubPage = this.subPages[i] as NewGameSubPageCustomPresetFeature;
				bool flag = featureSubPage != null && featureSubPage.TryCloseFeatureSelectionByEsc();
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x002DE520 File Offset: 0x002DC720
		private bool ShouldShowRandomConfirm(CustomProtagonistPresetItem currentData)
		{
			return !this.IsSameAsLastRandomState(currentData);
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x002DE53C File Offset: 0x002DC73C
		private bool IsSameAsLastRandomState(CustomProtagonistPresetItem currentData)
		{
			bool flag = this._lastRandomResult == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._lastRandomToggleIndex != this._currentToggleIndex;
				result = (!flag2 && NewGameSubPageCustomPreset.AreRandomizableDataEqual(currentData, this._lastRandomResult));
			}
			return result;
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x002DE584 File Offset: 0x002DC784
		private void RecordLastRandomState(CustomProtagonistPresetItem currentData)
		{
			this._lastRandomResult = currentData.Clone();
			this._lastRandomToggleIndex = this._currentToggleIndex;
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x002DE5A0 File Offset: 0x002DC7A0
		private unsafe static bool AreRandomizableDataEqual(CustomProtagonistPresetItem left, CustomProtagonistPresetItem right)
		{
			for (int i = 0; i < 6; i++)
			{
				bool flag = *left.MainAttributes[(int)((sbyte)i)] != *right.MainAttributes[(int)((sbyte)i)];
				if (flag)
				{
					return false;
				}
			}
			bool flag2 = left.LifeSkillQualificationGrowthType != right.LifeSkillQualificationGrowthType;
			if (flag2)
			{
				return false;
			}
			for (int j = 0; j < 16; j++)
			{
				bool flag3 = *left.LifeSkillQualifications[(int)((sbyte)j)] != *right.LifeSkillQualifications[(int)((sbyte)j)];
				if (flag3)
				{
					return false;
				}
			}
			bool flag4 = left.CombatSkillQualificationGrowthType != right.CombatSkillQualificationGrowthType;
			if (flag4)
			{
				return false;
			}
			for (int k = 0; k < 14; k++)
			{
				bool flag5 = *left.CombatSkillQualifications[(int)((sbyte)k)] != *right.CombatSkillQualifications[(int)((sbyte)k)];
				if (flag5)
				{
					return false;
				}
			}
			return NewGameSubPageCustomPreset.AreSelectedFeaturesEqual(left.SelectedFeatures, right.SelectedFeatures);
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x002DE6CC File Offset: 0x002DC8CC
		private static bool AreSelectedFeaturesEqual(List<short> left, List<short> right)
		{
			int leftCount = (left != null) ? left.Count : 0;
			int rightCount = (right != null) ? right.Count : 0;
			bool flag = leftCount != rightCount;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = leftCount == 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					HashSet<short> featureSet = new HashSet<short>(left);
					bool flag3 = featureSet.Count != leftCount;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < right.Count; i++)
						{
							bool flag4 = !featureSet.Contains(right[i]);
							if (flag4)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600642B RID: 25643 RVA: 0x002DE774 File Offset: 0x002DC974
		private void InitializeSubPages()
		{
			bool flag = this.subPages == null || this.subPages.Length == 0;
			if (!flag)
			{
				foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
				{
					bool flag2 = page != null;
					if (flag2)
					{
						page.ParentCustomPresetPage = this;
					}
				}
			}
		}

		// Token: 0x0600642C RID: 25644 RVA: 0x002DE7CD File Offset: 0x002DC9CD
		private void ResetSpentPoints()
		{
			this.RecalculateSpentPointsFromSubPages();
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x002DE7D8 File Offset: 0x002DC9D8
		private void RefreshAllSubPages()
		{
			foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
			{
				bool flag = page != null;
				if (flag)
				{
					page.RefreshUI();
				}
			}
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x002DE814 File Offset: 0x002DCA14
		protected override void OnValueChange(bool isOn)
		{
			bool flag = !isOn;
			if (flag)
			{
				this.CloseActiveFeatureSelection();
			}
			base.OnValueChange(isOn);
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x002DE83C File Offset: 0x002DCA3C
		private void CloseActiveFeatureSelection()
		{
			bool flag = this.subPages == null;
			if (!flag)
			{
				foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
				{
					NewGameSubPageCustomPresetFeature featurePage = page as NewGameSubPageCustomPresetFeature;
					bool flag2 = featurePage != null && page.gameObject.activeSelf;
					if (flag2)
					{
						featurePage.CloseFeatureSelection();
						break;
					}
				}
			}
		}

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06006430 RID: 25648 RVA: 0x002DE8A0 File Offset: 0x002DCAA0
		// (remove) Token: 0x06006431 RID: 25649 RVA: 0x002DE8D8 File Offset: 0x002DCAD8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> OnUseCustomPresetToggleChangedOverEvent;

		// Token: 0x06006432 RID: 25650 RVA: 0x002DE910 File Offset: 0x002DCB10
		public void SwitchToSubPage(NewGameSubPageCustomPreset.ESubPageType type, bool resetCheck = false, bool callFocus = false)
		{
			if (resetCheck)
			{
				this.parent.ResetChecks();
			}
			if (callFocus)
			{
				base.FocusToPage();
			}
			bool flag = this.subPages == null || type >= (NewGameSubPageCustomPreset.ESubPageType)this.subPages.Length;
			if (!flag)
			{
				this.CloseActiveFeatureSelection();
				for (int i = 0; i < this.subPages.Length; i++)
				{
					bool flag2 = this.subPages[i] != null;
					if (flag2)
					{
						this.subPages[i].gameObject.SetActive(i == (int)type);
					}
				}
				bool flag3 = type < (NewGameSubPageCustomPreset.ESubPageType)this.subPages.Length && this.subPages[(int)type] != null;
				if (flag3)
				{
					this.subPages[(int)type].RefreshUI();
				}
				this.RefreshToggleDisplay();
			}
		}

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x06006433 RID: 25651 RVA: 0x002DE9E1 File Offset: 0x002DCBE1
		public override string DisableEnterGameReason
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x06006434 RID: 25652 RVA: 0x002DE9E8 File Offset: 0x002DCBE8
		public override DialogCmd StartGameCheck
		{
			get
			{
				DialogCmd result;
				if (this._useCustomPreset)
				{
					result = (from x in this.subPages
					select x.StartGameChecked ? null : x.StartGameCheck).FirstOrDefault((DialogCmd x) => x != null);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x06006435 RID: 25653 RVA: 0x002DEA4E File Offset: 0x002DCC4E
		// (set) Token: 0x06006436 RID: 25654 RVA: 0x002DEA64 File Offset: 0x002DCC64
		public override bool StartGameChecked
		{
			get
			{
				return this._checked || !this._useCustomPreset;
			}
			set
			{
				this._checked = value;
				bool @checked = this._checked;
				if (@checked)
				{
					this.parent.ContinueStartNewGame();
				}
				else
				{
					foreach (NewGameSubPageCustomPresetItemBase item in this.subPages)
					{
						item.StartGameChecked = false;
					}
				}
			}
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x002DEAB4 File Offset: 0x002DCCB4
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			bool flag = protagonistCreationInfo == null;
			if (!flag)
			{
				bool useCustomPreset = this._useCustomPreset;
				if (useCustomPreset)
				{
					CustomProtagonistPresetItem presetItem = new CustomProtagonistPresetItem();
					foreach (NewGameSubPageCustomPresetItemBase page in this.subPages)
					{
						bool flag2 = page != null;
						if (flag2)
						{
							page.ApplyToPreset(presetItem);
						}
					}
					protagonistCreationInfo.CustomPreset = presetItem;
				}
				else
				{
					protagonistCreationInfo.CustomPreset = null;
				}
			}
		}

		// Token: 0x040045E2 RID: 17890
		[SerializeField]
		private NewGameSubPageCustomPresetToggleHelper toggleHelper;

		// Token: 0x040045E3 RID: 17891
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x040045E4 RID: 17892
		[SerializeField]
		private CToggle useCustomPresetToggle;

		// Token: 0x040045E5 RID: 17893
		[SerializeField]
		private GameObject customPresetOffOverlay;

		// Token: 0x040045E6 RID: 17894
		[SerializeField]
		private GameObject warningTips;

		// Token: 0x040045E7 RID: 17895
		[SerializeField]
		private TextMeshProUGUI customTipsText;

		// Token: 0x040045E8 RID: 17896
		[SerializeField]
		private UIParticle customToggleParticle;

		// Token: 0x040045E9 RID: 17897
		[Header("预设相关")]
		[SerializeField]
		private CToggleGroup presetToggleGroup;

		// Token: 0x040045EA RID: 17898
		[SerializeField]
		private CButton addPresetButton;

		// Token: 0x040045EB RID: 17899
		[SerializeField]
		private CButton clonePresetButton;

		// Token: 0x040045EC RID: 17900
		[SerializeField]
		private CButton clearPresetButton;

		// Token: 0x040045ED RID: 17901
		[SerializeField]
		private CButton deletePresetButton;

		// Token: 0x040045EE RID: 17902
		[SerializeField]
		private CButton resetButton;

		// Token: 0x040045EF RID: 17903
		[SerializeField]
		private CButton randomButton;

		// Token: 0x040045F0 RID: 17904
		[Header("子页面容器")]
		[SerializeField]
		private Transform subPagesContainer;

		// Token: 0x040045F1 RID: 17905
		[SerializeField]
		private NewGameSubPageCustomPresetItemBase[] subPages;

		// Token: 0x040045F2 RID: 17906
		private NewGameSubPageCustomPresetSelectOrganization _selectOrganizationSubPage;

		// Token: 0x040045F3 RID: 17907
		private bool _useCustomPreset;

		// Token: 0x040045F4 RID: 17908
		private int _currentToggleIndex;

		// Token: 0x040045F5 RID: 17909
		private bool _isRestoringToggle;

		// Token: 0x040045F6 RID: 17910
		private CustomProtagonistPresetItem _lastRandomResult;

		// Token: 0x040045F7 RID: 17911
		private int _lastRandomToggleIndex = int.MinValue;

		// Token: 0x040045F8 RID: 17912
		private int _spentPoints;

		// Token: 0x040045FA RID: 17914
		private bool _checked;

		// Token: 0x02001D3E RID: 7486
		public enum ESubPageType
		{
			// Token: 0x0400C577 RID: 50551
			Template,
			// Token: 0x0400C578 RID: 50552
			Neili,
			// Token: 0x0400C579 RID: 50553
			Attribute,
			// Token: 0x0400C57A RID: 50554
			Qualification,
			// Token: 0x0400C57B RID: 50555
			Feature
		}
	}
}
