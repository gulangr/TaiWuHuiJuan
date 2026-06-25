using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Inscription;
using GameData.Domains.Character;
using GameData.Domains.Global;
using GameData.Domains.Global.Inscription;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Main.Inscription
{
	// Token: 0x0200097B RID: 2427
	public class ViewSelectInscriptionForEvolution : UIBase
	{
		// Token: 0x17000D28 RID: 3368
		// (get) Token: 0x06007466 RID: 29798 RVA: 0x00363628 File Offset: 0x00361828
		private int MaxSelectCount
		{
			get
			{
				return GlobalConfig.Instance.InscriptionCharForCreationMaxCount;
			}
		}

		// Token: 0x06007467 RID: 29799 RVA: 0x00363634 File Offset: 0x00361834
		public override void OnInit(ArgumentBox argsBox)
		{
			CButton cbutton = this.closeButton;
			if (cbutton != null)
			{
				cbutton.ClearAndAddListener(new Action(this.OnCloseClicked));
			}
			CButton cbutton2 = this.confirmButton;
			if (cbutton2 != null)
			{
				cbutton2.ClearAndAddListener(new Action(this.OnConfirmButtonClicked));
			}
			CButton cbutton3 = this.selectAllButton;
			if (cbutton3 != null)
			{
				cbutton3.ClearAndAddListener(new Action(this.OnSelectAllClicked));
			}
			CButton cbutton4 = this.clearAllButton;
			if (cbutton4 != null)
			{
				cbutton4.ClearAndAddListener(new Action(this.OnClearAllClicked));
			}
			CButton cbutton5 = this.ageSettingButton;
			if (cbutton5 != null)
			{
				cbutton5.ClearAndAddListener(new Action(this.OnAgeSettingClicked));
			}
			CButton cbutton6 = this.defaultAgeButton;
			if (cbutton6 != null)
			{
				cbutton6.ClearAndAddListener(new Action(this.OnDefaultAgeClicked));
			}
			CButton cbutton7 = this.ageSettingEnableButton;
			if (cbutton7 != null)
			{
				cbutton7.ClearAndAddListener(new Action(this.OnAgeSettingEnableClicked));
			}
			CButton cbutton8 = this.ageSettingExitButton;
			if (cbutton8 != null)
			{
				cbutton8.ClearAndAddListener(new Action(this.OnAgeSettingExitClicked));
			}
			bool flag = this.unifiedAgeSlider != null;
			if (flag)
			{
				this.unifiedAgeSlider.minValue = 1f;
				this.unifiedAgeSlider.maxValue = 100f;
				this.unifiedAgeSlider.wholeNumbers = true;
				this.unifiedAgeSlider.value = 20f;
				this.unifiedAgeSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnUnifiedAgeSliderChanged));
			}
			bool flag2 = this.randomRangeSlider != null;
			if (flag2)
			{
				this.randomRangeSlider.minValue = 0f;
				this.randomRangeSlider.maxValue = 50f;
				this.randomRangeSlider.wholeNumbers = true;
				this.randomRangeSlider.value = 0f;
				this.randomRangeSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnRandomRangeSliderChanged));
			}
			this.UpdateAgeSettingTexts();
			this.charScroll.OnItemRender += this.OnCharItemRender;
			this._sortAndFilterController = new InscriptionSortAndFilterController(this.sortAndFilter, false);
			this._sortAndFilterController.Init(new Action(this.OnSortFilterChanged), "InscriptionSort");
			bool flag3 = this.searchingField != null;
			if (flag3)
			{
				this.searchingField.onValueChanged.RemoveAllListeners();
				this.searchingField.onValueChanged.AddListener(delegate(string _)
				{
					this.OnSortFilterChanged();
				});
			}
			GEvent.Add(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
			bool flag4 = argsBox != null;
			if (flag4)
			{
				argsBox.Get("IsNewGameSubPageAvatar", out this._isNewGameSubPageAvatar);
			}
			this._currentSelectedKey = InscribedCharacterKey.Invalid;
			bool flag5 = !this._isNewGameSubPageAvatar && argsBox != null;
			if (flag5)
			{
				this._selectedKeys.Clear();
				this._selectedAges.Clear();
				List<InscribedCharacterKey> preSelectedKeys;
				argsBox.Get<List<InscribedCharacterKey>>("PreSelectedKeys", out preSelectedKeys);
				bool flag6 = preSelectedKeys != null && preSelectedKeys.Count > 0;
				if (flag6)
				{
					foreach (InscribedCharacterKey key in preSelectedKeys)
					{
						this._selectedKeys.Add(key);
					}
				}
				Dictionary<InscribedCharacterKey, short> preSelectedAges;
				argsBox.Get<Dictionary<InscribedCharacterKey, short>>("PreSelectedAges", out preSelectedAges);
				bool flag7 = preSelectedAges != null && preSelectedAges.Count > 0;
				if (flag7)
				{
					foreach (KeyValuePair<InscribedCharacterKey, short> kv in preSelectedAges)
					{
						this._selectedAges[kv.Key] = kv.Value;
					}
				}
				short savedUnifiedAge;
				argsBox.Get("UnifiedAge", out savedUnifiedAge);
				bool flag8 = savedUnifiedAge > 0;
				if (flag8)
				{
					this._unifiedAge = savedUnifiedAge;
					bool flag9 = this.unifiedAgeSlider != null;
					if (flag9)
					{
						this.unifiedAgeSlider.value = (float)this._unifiedAge;
					}
				}
				short savedRandomRange;
				argsBox.Get("RandomRange", out savedRandomRange);
				this._randomRange = savedRandomRange;
				bool flag10 = this.randomRangeSlider != null;
				if (flag10)
				{
					this.randomRangeSlider.value = (float)this._randomRange;
				}
				bool savedAgeSettingEnabled;
				argsBox.Get("AgeSettingEnabled", out savedAgeSettingEnabled);
				this._ageSettingEnabled = savedAgeSettingEnabled;
				this.UpdateAgeSettingTexts();
			}
			bool isNewGameSubPageAvatar = this._isNewGameSubPageAvatar;
			if (isNewGameSubPageAvatar)
			{
				foreach (GameObject obj in this.closeAgeComponents)
				{
					obj.SetActive(false);
				}
				this.charScroll.gap = new Vector2(32f, -60f);
			}
			else
			{
				foreach (GameObject obj2 in this.closeAgeComponents)
				{
					obj2.SetActive(true);
				}
				this.charScroll.gap = new Vector2(32f, 16f);
			}
			this.charScroll.ClearCache();
			this.BuildDataAndRefresh();
			bool flag11 = this._ageSettingEnabled && !this._isNewGameSubPageAvatar;
			if (flag11)
			{
				this.ApplyAgeSettingToAll();
			}
			else
			{
				this.RefreshSelectedCount();
			}
		}

		// Token: 0x06007468 RID: 29800 RVA: 0x00363B88 File Offset: 0x00361D88
		public override void NotifyUIHide()
		{
			base.NotifyUIHide();
			this.charScroll.OnItemRender -= this.OnCharItemRender;
			GEvent.Remove(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
			this.OnConfirmClicked = null;
			this.OnQuickHide = null;
			this._currentCharData = null;
		}

		// Token: 0x06007469 RID: 29801 RVA: 0x00363BE4 File Offset: 0x00361DE4
		private void BuildDataAndRefresh()
		{
			this._allCharData.Clear();
			Dictionary<InscribedCharacterKey, InscribedCharacter> inscribedChars = GlobalOperations.InscribedCharacters;
			Dictionary<InscribedCharacterKey, int> pinOrders = GlobalOperations.InscribedCharacterPinOrders;
			foreach (KeyValuePair<InscribedCharacterKey, InscribedCharacter> kv in inscribedChars)
			{
				int order;
				CheckInscriptionCharData data = new CheckInscriptionCharData
				{
					Key = kv.Key,
					Character = kv.Value,
					PinOrder = (pinOrders.TryGetValue(kv.Key, out order) ? order : -1),
					Charm = kv.Value.CalcAttraction(kv.Value.ActualAge, kv.Value.ClothingDisplayId),
					MainAttributeSum = ViewSelectInscriptionForEvolution.CalcMainAttributeSum(kv.Value),
					LifeSkillSum = ViewSelectInscriptionForEvolution.CalcSkillSum(kv.Value.BaseLifeSkillQualifications),
					CombatSkillSum = ViewSelectInscriptionForEvolution.CalcSkillSum(kv.Value.BaseCombatSkillQualifications)
				};
				this._allCharData.Add(data);
				bool flag = !this._selectedAges.ContainsKey(kv.Key);
				if (flag)
				{
					this._selectedAges[kv.Key] = kv.Value.CurrAge;
				}
			}
			this.ApplySortAndFilter();
		}

		// Token: 0x0600746A RID: 29802 RVA: 0x00363D54 File Offset: 0x00361F54
		private void ApplySortAndFilter()
		{
			Func<CheckInscriptionCharData, bool> filter = this._sortAndFilterController.GenerateFilter();
			Comparison<CheckInscriptionCharData> comparer = this._sortAndFilterController.GenerateComparer(this._allCharData);
			this._displayedCharData = this._allCharData.Where(delegate(CheckInscriptionCharData d)
			{
				bool flag6 = !filter(d);
				bool result;
				if (flag6)
				{
					result = false;
				}
				else
				{
					bool flag7 = this.searchingField != null && !string.IsNullOrEmpty(this.searchingField.text);
					if (flag7)
					{
						string fullName = d.Character.Surname + d.Character.GivenName;
						result = fullName.Contains(this.searchingField.text);
					}
					else
					{
						result = true;
					}
				}
				return result;
			}).ToList<CheckInscriptionCharData>();
			this._displayedCharData.Sort(comparer);
			this.charScroll.SetDataCount(this._displayedCharData.Count);
			bool flag = this._isNewGameSubPageAvatar && this._displayedCharData.Count > 0 && this._currentSelectedKey.Equals(InscribedCharacterKey.Invalid);
			if (flag)
			{
				this.SelectChar(0);
			}
			else
			{
				bool flag2 = !this._currentSelectedKey.Equals(InscribedCharacterKey.Invalid);
				if (flag2)
				{
					int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(this._currentSelectedKey));
					bool flag3 = index >= 0;
					if (flag3)
					{
						this.SelectChar(index);
					}
					else
					{
						bool flag4 = this.detailRoot != null;
						if (flag4)
						{
							this.detailRoot.SetActive(false);
						}
					}
				}
				else
				{
					bool flag5 = this.detailRoot != null;
					if (flag5)
					{
						this.detailRoot.SetActive(false);
					}
				}
			}
			this.emptyGo.SetActive(this._displayedCharData.Count == 0);
			this.UpdateRightEmptyGo();
			this._sortAndFilterController.AfterFilter(this._allCharData);
		}

		// Token: 0x0600746B RID: 29803 RVA: 0x00363ED8 File Offset: 0x003620D8
		private void OnCharItemRender(int index, GameObject cell)
		{
			bool flag = index < 0 || index >= this._displayedCharData.Count;
			if (!flag)
			{
				CheckInscriptionCharCardWithAge card = cell.GetComponent<CheckInscriptionCharCardWithAge>();
				bool flag2 = card == null;
				if (!flag2)
				{
					CheckInscriptionCharData data = this._displayedCharData[index];
					short age;
					short selectedAge = this._selectedAges.TryGetValue(data.Key, out age) ? age : data.Character.CurrAge;
					bool isNewGameSubPageAvatar = this._isNewGameSubPageAvatar;
					if (isNewGameSubPageAvatar)
					{
						card.AgeObj.SetActive(false);
						card.SetToggleActive(false);
						CheckInscriptionCharData currentCharData = this._currentCharData;
						card.SetData(data, this, ((currentCharData != null) ? currentCharData.Key : default(InscribedCharacterKey)).Equals(data.Key), false, selectedAge);
					}
					else
					{
						card.AgeObj.SetActive(true);
						card.SetToggleActive(true);
						bool isViewing = this._currentSelectedKey.Equals(data.Key);
						bool isSelected = this._selectedKeys.Contains(data.Key);
						card.SetData(data, this, isViewing, isSelected, selectedAge);
					}
				}
			}
		}

		// Token: 0x0600746C RID: 29804 RVA: 0x00363FF8 File Offset: 0x003621F8
		public void SelectChar(int index)
		{
			bool flag = index < 0 || index >= this._displayedCharData.Count;
			if (!flag)
			{
				this._currentSelectedKey = this._displayedCharData[index].Key;
				this.charScroll.ReRender();
				this.detailPanel.SetData(this._displayedCharData[index]);
			}
		}

		// Token: 0x0600746D RID: 29805 RVA: 0x00364060 File Offset: 0x00362260
		public void SelectCharByKey(InscribedCharacterKey key)
		{
			int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(key));
			bool flag = index >= 0;
			if (flag)
			{
				this.SelectChar(index);
			}
		}

		// Token: 0x0600746E RID: 29806 RVA: 0x003640A8 File Offset: 0x003622A8
		public void ShowCharDetail(InscribedCharacterKey key)
		{
			int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(key));
			bool flag = index >= 0;
			if (flag)
			{
				CheckInscriptionCharDetail checkInscriptionCharDetail = this.detailPanel;
				if (checkInscriptionCharDetail != null)
				{
					checkInscriptionCharDetail.SetData(this._displayedCharData[index]);
				}
			}
		}

		// Token: 0x0600746F RID: 29807 RVA: 0x00364108 File Offset: 0x00362308
		public void ViewCharInfo(InscribedCharacterKey key)
		{
			bool isNewGameSubPageAvatar = this._isNewGameSubPageAvatar;
			if (isNewGameSubPageAvatar)
			{
				int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(key));
				bool flag = index >= 0;
				if (flag)
				{
					this._currentCharData = this._displayedCharData[index];
				}
				this.ShowCharDetail(key);
				this.charScroll.ReRender();
			}
			else
			{
				int dataIndex = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(key));
				bool flag2 = dataIndex < 0;
				if (!flag2)
				{
					this._currentSelectedKey = key;
					this.detailPanel.SetData(this._displayedCharData[dataIndex]);
					this.charScroll.ReRender();
					this.UpdateRightEmptyGo();
				}
			}
		}

		// Token: 0x06007470 RID: 29808 RVA: 0x003641E0 File Offset: 0x003623E0
		public void SetToggleSelection(InscribedCharacterKey key, bool isSelected)
		{
			bool isNewGameSubPageAvatar = this._isNewGameSubPageAvatar;
			if (!isNewGameSubPageAvatar)
			{
				if (isSelected)
				{
					bool flag = this._selectedKeys.Count < this.MaxSelectCount;
					if (flag)
					{
						this._selectedKeys.Add(key);
					}
				}
				else
				{
					this._selectedKeys.Remove(key);
				}
				this.charScroll.ReRender();
				this.RefreshSelectedCount();
			}
		}

		// Token: 0x06007471 RID: 29809 RVA: 0x0036424B File Offset: 0x0036244B
		public void SetAge(InscribedCharacterKey key, short age)
		{
			this._selectedAges[key] = age;
		}

		// Token: 0x06007472 RID: 29810 RVA: 0x0036425C File Offset: 0x0036245C
		private void OnSortFilterChanged()
		{
			this.ApplySortAndFilter();
		}

		// Token: 0x06007473 RID: 29811 RVA: 0x00364266 File Offset: 0x00362466
		private void OnInscriptionChange(ArgumentBox _ = null)
		{
			this.BuildDataAndRefresh();
		}

		// Token: 0x06007474 RID: 29812 RVA: 0x00364270 File Offset: 0x00362470
		private void OnCloseClicked()
		{
			bool flag = this.ageSettingPanel != null && this.ageSettingPanel.gameObject.activeInHierarchy;
			if (flag)
			{
				UIManager.Instance.UnMaskComponent(this.ageSettingPanel);
			}
			else
			{
				Action onQuickHide = this.OnQuickHide;
				if (onQuickHide != null)
				{
					onQuickHide();
				}
				this.QuickHide();
			}
		}

		// Token: 0x06007475 RID: 29813 RVA: 0x003642D0 File Offset: 0x003624D0
		private void OnConfirmButtonClicked()
		{
			bool isNewGameSubPageAvatar = this._isNewGameSubPageAvatar;
			if (isNewGameSubPageAvatar)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				args.SetObject("PersonnelCharData", this._currentCharData);
				GEvent.OnEvent(UiEvents.ConfirmIncludedInscriptionCharMultipleChoice, args);
			}
			else
			{
				List<InscribedCharacterKey> selectedKeysList = new List<InscribedCharacterKey>(this._selectedKeys);
				List<InscribedCharacterKey> canIncludeList = new List<InscribedCharacterKey>();
				Dictionary<InscribedCharacterKey, InscribedCharacter> inscribedChars = GlobalOperations.InscribedCharacters;
				foreach (InscribedCharacterKey key in inscribedChars.Keys)
				{
					bool flag = !key.Equals(InscribedCharacterKey.Invalid) && !this._selectedKeys.Contains(key);
					if (flag)
					{
						canIncludeList.Add(key);
					}
				}
				ArgumentBox args2 = EasyPool.Get<ArgumentBox>().SetObject("IncludedInscribedCharList", selectedKeysList).SetObject("CanIncludeInscribedCharList", canIncludeList).Set("AutoIncludeInscribedChar", false).SetObject("IncludedInscribedCharAges", new Dictionary<InscribedCharacterKey, short>(this._selectedAges)).Set("UnifiedAge", this._unifiedAge).Set("RandomRange", this._randomRange).Set("AgeSettingEnabled", this._ageSettingEnabled);
				GEvent.OnEvent(UiEvents.ConfirmIncludedInscriptionChar, args2);
			}
			Action<HashSet<InscribedCharacterKey>, Dictionary<InscribedCharacterKey, short>> onConfirmClicked = this.OnConfirmClicked;
			if (onConfirmClicked != null)
			{
				onConfirmClicked(new HashSet<InscribedCharacterKey>(this._selectedKeys), new Dictionary<InscribedCharacterKey, short>(this._selectedAges));
			}
			this.QuickHide();
		}

		// Token: 0x06007476 RID: 29814 RVA: 0x0036445C File Offset: 0x0036265C
		private void OnSelectAllClicked()
		{
			this._selectedKeys.Clear();
			foreach (CheckInscriptionCharData data in this._displayedCharData)
			{
				bool flag = this._selectedKeys.Count >= this.MaxSelectCount;
				if (flag)
				{
					break;
				}
				this._selectedKeys.Add(data.Key);
			}
			this.charScroll.ReRender();
			this.RefreshSelectedCount();
		}

		// Token: 0x06007477 RID: 29815 RVA: 0x003644FC File Offset: 0x003626FC
		private void OnClearAllClicked()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.LK_NewGame_InscriptionCharForCreation_ClearAll_Confirm_Title.Tr(),
				Content = LanguageKey.LK_NewGame_InscriptionCharForCreation_ClearAll_Confirm_Content.Tr(),
				Yes = delegate()
				{
					this._selectedKeys.Clear();
					this.charScroll.ReRender();
					this.RefreshSelectedCount();
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007478 RID: 29816 RVA: 0x0036456B File Offset: 0x0036276B
		private void OnAgeSettingClicked()
		{
			UIManager.Instance.MaskComponent(this.ageSettingPanel);
		}

		// Token: 0x06007479 RID: 29817 RVA: 0x0036457F File Offset: 0x0036277F
		private void OnAgeSettingExitClicked()
		{
			UIManager.Instance.UnMaskComponent(this.ageSettingPanel);
		}

		// Token: 0x0600747A RID: 29818 RVA: 0x00364593 File Offset: 0x00362793
		private void OnAgeSettingEnableClicked()
		{
			this._ageSettingEnabled = true;
			this.ApplyAgeSettingToAll();
			UIManager.Instance.UnMaskComponent(this.ageSettingPanel);
		}

		// Token: 0x0600747B RID: 29819 RVA: 0x003645B8 File Offset: 0x003627B8
		private void ApplyAgeSettingToAll()
		{
			this._uiSetAges.Clear();
			Random random = new Random();
			foreach (CheckInscriptionCharData data in this._displayedCharData)
			{
				short baseAge = this._unifiedAge;
				short randomOffset = (short)random.Next((int)(-(int)this._randomRange), (int)(this._randomRange + 1));
				short finalAge = (short)Mathf.Clamp((int)(baseAge + randomOffset), 1, 100);
				this._uiSetAges[data.Key] = finalAge;
				this._selectedAges[data.Key] = finalAge;
			}
			this.charScroll.ReRender();
			this.RefreshSelectedCount();
		}

		// Token: 0x0600747C RID: 29820 RVA: 0x00364684 File Offset: 0x00362884
		private void OnUnifiedAgeSliderChanged(float value)
		{
			this._unifiedAge = (short)value;
			this.UpdateAgeSettingTexts();
		}

		// Token: 0x0600747D RID: 29821 RVA: 0x00364696 File Offset: 0x00362896
		private void OnRandomRangeSliderChanged(float value)
		{
			this._randomRange = (short)value;
			this.UpdateAgeSettingTexts();
		}

		// Token: 0x0600747E RID: 29822 RVA: 0x003646A8 File Offset: 0x003628A8
		private void UpdateAgeSettingTexts()
		{
			bool flag = this.unifiedAgeText != null;
			if (flag)
			{
				this.unifiedAgeText.text = this._unifiedAge.ToString();
			}
			bool flag2 = this.randomRangeText != null;
			if (flag2)
			{
				this.randomRangeText.text = string.Format("±{0}", this._randomRange);
			}
		}

		// Token: 0x0600747F RID: 29823 RVA: 0x00364714 File Offset: 0x00362914
		private void OnDefaultAgeClicked()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.LK_NewGame_InscriptionCharForCreation_DefaultAge_Confirm_Title.Tr(),
				Content = LanguageKey.LK_NewGame_InscriptionCharForCreation_DefaultAge_Confirm_Content.Tr(),
				Yes = delegate()
				{
					foreach (CheckInscriptionCharData kv in this._allCharData)
					{
						this._selectedAges[kv.Key] = kv.Character.CurrAge;
					}
					this.charScroll.ReRender();
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007480 RID: 29824 RVA: 0x00364784 File Offset: 0x00362984
		private void UpdateRightEmptyGo()
		{
			bool isViewing = !this._currentSelectedKey.Equals(InscribedCharacterKey.Invalid);
			bool flag = this.rightEmptyGo != null;
			if (flag)
			{
				this.rightEmptyGo.SetActive(!isViewing && this._displayedCharData.Count > 0);
			}
			bool flag2 = this.detailRoot != null;
			if (flag2)
			{
				this.detailRoot.SetActive(isViewing);
			}
		}

		// Token: 0x06007481 RID: 29825 RVA: 0x003647F8 File Offset: 0x003629F8
		private void RefreshSelectedCount()
		{
			bool flag = this.selectedCountText != null;
			if (flag)
			{
				this.selectedCountText.text = string.Format("已选人物：{0}/{1}", this._selectedKeys.Count, this.MaxSelectCount);
			}
			this.UpdateRightEmptyGo();
		}

		// Token: 0x06007482 RID: 29826 RVA: 0x00364850 File Offset: 0x00362A50
		public void DeleteInscribedCharacter(InscribedCharacterKey key)
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character.Tr(),
				Content = LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character_Confirm.Tr(),
				Yes = delegate()
				{
					this._selectedKeys.Remove(key);
					this._selectedAges.Remove(key);
					bool flag = this._currentSelectedKey.Equals(key);
					if (flag)
					{
						this._currentSelectedKey = InscribedCharacterKey.Invalid;
					}
					GlobalDomainMethod.Call.RemoveInscribedCharacter(key);
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007483 RID: 29827 RVA: 0x003648D4 File Offset: 0x00362AD4
		public void TogglePin(InscribedCharacterKey key)
		{
			Dictionary<InscribedCharacterKey, int> pinOrders = GlobalOperations.InscribedCharacterPinOrders;
			bool flag = pinOrders.ContainsKey(key);
			if (flag)
			{
				GlobalDomainMethod.Call.RemoveInscribedCharacterPinOrder(key);
			}
			else
			{
				int maxOrder = (pinOrders.Count > 0) ? (pinOrders.Values.Max() + 1) : 0;
				GlobalDomainMethod.Call.SetInscribedCharacterPinOrder(key, maxOrder);
			}
		}

		// Token: 0x06007484 RID: 29828 RVA: 0x00364924 File Offset: 0x00362B24
		private static int CalcMainAttributeSum(InscribedCharacter c)
		{
			MainAttributes attrs = c.BaseMainAttributes;
			return attrs.GetSum();
		}

		// Token: 0x06007485 RID: 29829 RVA: 0x00364944 File Offset: 0x00362B44
		private static int CalcSkillSum(LifeSkillShorts skills)
		{
			return skills.GetSum();
		}

		// Token: 0x06007486 RID: 29830 RVA: 0x00364960 File Offset: 0x00362B60
		private static int CalcSkillSum(CombatSkillShorts skills)
		{
			return skills.GetSum();
		}

		// Token: 0x06007487 RID: 29831 RVA: 0x0036497C File Offset: 0x00362B7C
		public override void QuickHide()
		{
			bool flag = this.ageSettingPanel != null && this.ageSettingPanel.gameObject.activeInHierarchy;
			if (flag)
			{
				UIManager.Instance.UnMaskComponent(this.ageSettingPanel);
			}
			else
			{
				this._isNewGameSubPageAvatar = false;
				this._currentCharData = null;
				Action onQuickHide = this.OnQuickHide;
				if (onQuickHide != null)
				{
					onQuickHide();
				}
				base.QuickHide();
			}
		}

		// Token: 0x040056EF RID: 22255
		public Action<HashSet<InscribedCharacterKey>, Dictionary<InscribedCharacterKey, short>> OnConfirmClicked;

		// Token: 0x040056F0 RID: 22256
		public Action OnQuickHide;

		// Token: 0x040056F1 RID: 22257
		[SerializeField]
		private InfinityScroll charScroll;

		// Token: 0x040056F2 RID: 22258
		[SerializeField]
		private CheckInscriptionCharDetail detailPanel;

		// Token: 0x040056F3 RID: 22259
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x040056F4 RID: 22260
		[SerializeField]
		private CButton closeButton;

		// Token: 0x040056F5 RID: 22261
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x040056F6 RID: 22262
		[SerializeField]
		private CButton selectAllButton;

		// Token: 0x040056F7 RID: 22263
		[SerializeField]
		private CButton clearAllButton;

		// Token: 0x040056F8 RID: 22264
		[SerializeField]
		private CButton ageSettingButton;

		// Token: 0x040056F9 RID: 22265
		[SerializeField]
		private CButton defaultAgeButton;

		// Token: 0x040056FA RID: 22266
		[SerializeField]
		private TextMeshProUGUI selectedCountText;

		// Token: 0x040056FB RID: 22267
		[SerializeField]
		private TMP_InputField searchingField;

		// Token: 0x040056FC RID: 22268
		[SerializeField]
		private GameObject emptyGo;

		// Token: 0x040056FD RID: 22269
		[SerializeField]
		private GameObject rightEmptyGo;

		// Token: 0x040056FE RID: 22270
		[SerializeField]
		private GameObject detailRoot;

		// Token: 0x040056FF RID: 22271
		[SerializeField]
		private RectTransform ageSettingPanel;

		// Token: 0x04005700 RID: 22272
		[SerializeField]
		private CSlider unifiedAgeSlider;

		// Token: 0x04005701 RID: 22273
		[SerializeField]
		private CSlider randomRangeSlider;

		// Token: 0x04005702 RID: 22274
		[SerializeField]
		private CButton ageSettingEnableButton;

		// Token: 0x04005703 RID: 22275
		[SerializeField]
		private CButton ageSettingExitButton;

		// Token: 0x04005704 RID: 22276
		[SerializeField]
		private TextMeshProUGUI unifiedAgeText;

		// Token: 0x04005705 RID: 22277
		[SerializeField]
		private TextMeshProUGUI randomRangeText;

		// Token: 0x04005706 RID: 22278
		[SerializeField]
		private GameObject[] closeAgeComponents;

		// Token: 0x04005707 RID: 22279
		private InscriptionSortAndFilterController _sortAndFilterController;

		// Token: 0x04005708 RID: 22280
		private List<CheckInscriptionCharData> _allCharData = new List<CheckInscriptionCharData>();

		// Token: 0x04005709 RID: 22281
		private List<CheckInscriptionCharData> _displayedCharData = new List<CheckInscriptionCharData>();

		// Token: 0x0400570A RID: 22282
		private InscribedCharacterKey _currentSelectedKey = InscribedCharacterKey.Invalid;

		// Token: 0x0400570B RID: 22283
		private readonly HashSet<InscribedCharacterKey> _selectedKeys = new HashSet<InscribedCharacterKey>();

		// Token: 0x0400570C RID: 22284
		private readonly Dictionary<InscribedCharacterKey, short> _selectedAges = new Dictionary<InscribedCharacterKey, short>();

		// Token: 0x0400570D RID: 22285
		private readonly Dictionary<InscribedCharacterKey, short> _uiSetAges = new Dictionary<InscribedCharacterKey, short>();

		// Token: 0x0400570E RID: 22286
		private short _unifiedAge = 20;

		// Token: 0x0400570F RID: 22287
		private short _randomRange = 0;

		// Token: 0x04005710 RID: 22288
		private bool _ageSettingEnabled = false;

		// Token: 0x04005711 RID: 22289
		private bool _isNewGameSubPageAvatar = false;

		// Token: 0x04005712 RID: 22290
		private CheckInscriptionCharData _currentCharData;

		// Token: 0x04005713 RID: 22291
		private const short UnifiedAgeMin = 1;

		// Token: 0x04005714 RID: 22292
		private const short UnifiedAgeMax = 100;

		// Token: 0x04005715 RID: 22293
		private const short RandomRangeMin = 0;

		// Token: 0x04005716 RID: 22294
		private const short RandomRangeMax = 50;

		// Token: 0x04005717 RID: 22295
		private const short DefaultUnifiedAge = 20;
	}
}
