using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Main.Inscription;
using Game.Views.MouseTips;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Character.Creation;
using GameData.Domains.Global.Inscription;
using GameData.Domains.Item;
using GameData.Domains.World;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007EB RID: 2027
	public class NewGameSubPageAvatar : NewGameSubPage, IAvatarSubPageParent
	{
		// Token: 0x060062C3 RID: 25283 RVA: 0x002D2FAE File Offset: 0x002D11AE
		protected override void Awake()
		{
			base.Awake();
			this.InitializeUI();
			this.primaryToggleGroup.Set(0, true);
		}

		// Token: 0x060062C4 RID: 25284 RVA: 0x002D2FD0 File Offset: 0x002D11D0
		public override void Init()
		{
			base.Init();
			this.SetAvatarData(this.parent.CreationInfoAvatarData);
			Dictionary<InscribedCharacterKey, InscribedCharacter> inscribedCharacters = GlobalOperations.InscribedCharacters;
			int maxCount = (inscribedCharacters != null) ? inscribedCharacters.Count : 0;
			this.creationToggleGroup.SetInteractable(maxCount > 0, this.creationToggleGroup.Count() - 1);
		}

		// Token: 0x060062C5 RID: 25285 RVA: 0x002D3026 File Offset: 0x002D1226
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ConfirmIncludedInscriptionCharMultipleChoice, new GEvent.Callback(this.OnInscriptionChange));
			this.RefreshAvatar();
		}

		// Token: 0x060062C6 RID: 25286 RVA: 0x002D304C File Offset: 0x002D124C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ConfirmIncludedInscriptionCharMultipleChoice, new GEvent.Callback(this.OnInscriptionChange));
		}

		// Token: 0x060062C7 RID: 25287 RVA: 0x002D306C File Offset: 0x002D126C
		private void InitializeUI()
		{
			this.randomButton.ClearAndAddListener(new Action(this.RandomAvatar));
			this.genderToggleGroup.Init(-1);
			this.genderToggleGroup.OnActiveIndexChange += this.OnGenderChanged;
			this.creationToggleGroup.Init(-1);
			this.creationToggleGroup.OnActiveIndexChange += this.OnCreationChanged;
			this.transGenderToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnTransGenderChanged));
			this.ageSlider.minValue = 16f;
			this.ageSlider.maxValue = (float)GlobalConfig.Instance.MaxAgeOfCreatingChar;
			this.ageSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnAgeChanged));
			this.InitializeAgeSliderTicks();
			this.savePresetButton.ClearAndAddListener(new Action(this.SaveToCustomPreset));
			this.primaryToggleGroup.Init(-1);
			this.primaryToggleGroup.OnActiveIndexChange += this.OnPrimaryPageChanged;
			this.secondaryToggleGroupHelper.Init();
			this.secondaryToggleGroupHelper.AddOnActiveIndexChangeListener(new Action<int, int>(this.OnSecondaryPageChanged));
			this.InitializeSubPages();
		}

		// Token: 0x060062C8 RID: 25288 RVA: 0x002D31B0 File Offset: 0x002D13B0
		private void InitializeSubPages()
		{
			this.presetPage.Init(this);
			this.bodySkinPage.Init(this);
			this.hairPage.Init(this);
			this.beardPage.Init(this);
			this.facePartPage.Init(this);
			this.featurePage.Init(this);
			this.clothPage.Init(this);
			this.clothPage.OnlyCreateRes = true;
		}

		// Token: 0x060062C9 RID: 25289 RVA: 0x002D3228 File Offset: 0x002D1428
		public void SetAvatarData(AvatarData avatarData)
		{
			this.ClearSelectedInscribedCharacter();
			this._isAvatarDirty = false;
			this._hasManuallyAdjustedAppearance = false;
			bool flag = avatarData != null;
			if (flag)
			{
				this._avatarData = new AvatarData(avatarData);
				this._avatarData.ClothDisplayId = avatarData.ClothDisplayId;
			}
			else
			{
				this._avatarData = AvatarManager.Instance.GetRandomAvatar(GameApp.Random, 1, false, -1, null, null);
			}
			this.InitAvatarDisplayState();
			string genderStr;
			sbyte gender;
			bool flag2 = base.CreationInfoMap.TryGetValue("Gender", out genderStr) && sbyte.TryParse(genderStr, out gender);
			if (flag2)
			{
				this._gender = gender;
			}
			else
			{
				this._gender = this._avatarData.Gender;
			}
			string ageStr;
			short age;
			bool flag3 = base.CreationInfoMap.TryGetValue("Age", out ageStr) && short.TryParse(ageStr, out age);
			if (flag3)
			{
				this._age = age;
			}
			else
			{
				this._age = 16;
			}
			string transGenderStr;
			bool isTransGender;
			bool flag4 = base.CreationInfoMap.TryGetValue("IsTransGender", out transGenderStr) && bool.TryParse(transGenderStr, out isTransGender);
			if (flag4)
			{
				this._isTransGender = isTransGender;
			}
			else
			{
				this._isTransGender = false;
			}
			sbyte avatarDisplayedGender = this._avatarData.Gender;
			sbyte expectedGender = this._isTransGender ? ((avatarDisplayedGender == 1) ? 0 : 1) : avatarDisplayedGender;
			bool flag5 = this._gender != expectedGender;
			if (flag5)
			{
				this._gender = expectedGender;
			}
			this.ApplyDataToUI();
			this.RefreshAvatar();
		}

		// Token: 0x060062CA RID: 25290 RVA: 0x002D3394 File Offset: 0x002D1594
		private void ApplyDataToUI()
		{
			int genderIndex = (this._gender == 1) ? 0 : 1;
			this.genderToggleGroup.SetWithoutNotify(genderIndex);
			this.creationToggleGroup.Set(0, true);
			this.transGenderToggle.SetIsOnWithoutNotify(this._isTransGender);
			this.ageSlider.SetValueWithoutNotify((float)this._age);
			this.UpdateTransGenderLabel();
			this.UpdateAgeText();
		}

		// Token: 0x060062CB RID: 25291 RVA: 0x002D3400 File Offset: 0x002D1600
		private void InitializeAgeSliderTicks()
		{
			bool flag = this.ageTickContainer == null || this.ageTickTemplate == null || this.ageSlider == null;
			if (!flag)
			{
				int minAge = (int)this.ageSlider.minValue;
				int maxAge = (int)this.ageSlider.maxValue;
				int ageRange = maxAge - minAge;
				CommonUtils.PrepareEnoughChildren(this.ageTickContainer, this.ageTickTemplate.gameObject, ageRange + 1, null);
				RectTransform sliderRect = this.ageSlider.GetComponent<RectTransform>();
				bool flag2 = sliderRect == null;
				if (!flag2)
				{
					float sliderWidth = sliderRect.rect.width;
					for (int i = 0; i <= ageRange; i++)
					{
						RectTransform tick = this.ageTickContainer.GetChild(i).GetComponent<RectTransform>();
						bool flag3 = tick == null;
						if (!flag3)
						{
							float normalizedPosition = (float)i / (float)ageRange;
							tick.anchoredPosition = new Vector2(sliderWidth * normalizedPosition, tick.anchoredPosition.y);
						}
					}
				}
			}
		}

		// Token: 0x060062CC RID: 25292 RVA: 0x002D3518 File Offset: 0x002D1718
		private void RandomAvatar()
		{
			bool hasManuallyAdjustedAppearance = this._hasManuallyAdjustedAppearance;
			if (hasManuallyAdjustedAppearance)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_NewGame_Avatar_Random_Dialog_Title.Tr(),
					Content = LanguageKey.LK_NewGame_Avatar_Random_Dialog_Content.Tr(),
					Type = 1,
					Yes = new Action(this.DoRandomAvatar)
				};
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.SetObject("Cmd", cmd);
				UIElement.Dialog.SetOnInitArgs(box);
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.DoRandomAvatar();
			}
		}

		// Token: 0x060062CD RID: 25293 RVA: 0x002D35AC File Offset: 0x002D17AC
		private void DoRandomAvatar()
		{
			this.ClearSelectedInscribedCharacter();
			this._avatarData = AvatarManager.Instance.GetRandomAvatar(GameApp.Random, this._gender, false, -1, null, null);
			this.EnsureCreatableCloth();
			this.UpdateAvatarGender();
			this.InitAvatarDisplayState();
			this.RefreshAvatar();
			this.MarkAvatarDirty();
			this.RefreshHairTabsIfNeeded();
			this._hasManuallyAdjustedAppearance = false;
		}

		// Token: 0x060062CE RID: 25294 RVA: 0x002D3614 File Offset: 0x002D1814
		private void EnsureCreatableCloth()
		{
			bool flag = this._avatarData == null;
			if (!flag)
			{
				bool flag2 = !this.clothPage.OnlyCreateRes;
				if (!flag2)
				{
					AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)this._avatarData.AvatarId);
					bool flag3 = group == null;
					if (!flag3)
					{
						BodyRes currentBodyRes = group.BodyRes.Find((BodyRes e) => e.Id == this._avatarData.ClothDisplayId);
						bool flag4;
						if (currentBodyRes == null)
						{
							flag4 = false;
						}
						else
						{
							AvatarAsset cloth = currentBodyRes.Cloth;
							bool? flag5;
							if (cloth == null)
							{
								flag5 = null;
							}
							else
							{
								AvatarElementsItem config = cloth.Config;
								flag5 = ((config != null) ? new bool?(config.CanCreate) : null);
							}
							bool? flag6 = flag5;
							bool flag7 = true;
							flag4 = (flag6.GetValueOrDefault() == flag7 & flag6 != null);
						}
						bool flag8 = flag4;
						if (!flag8)
						{
							this._avatarData.ClothDisplayId = group.GetRandomCloth(GameApp.Random, true, false);
						}
					}
				}
			}
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x002D36FC File Offset: 0x002D18FC
		private void InitAvatarDisplayState()
		{
			bool flag = this._avatarData == null;
			if (!flag)
			{
				this._avatarData.SetGrowableElementShowingState(0, true);
				this._avatarData.SetGrowableElementShowingAbility(0, true);
				this._avatarData.SetGrowableElementShowingState(6, true);
				this._avatarData.SetGrowableElementShowingAbility(6, true);
				this._avatarData.SetGrowableElementShowingState(1, !this._isTransGender);
				this._avatarData.SetGrowableElementShowingState(2, !this._isTransGender);
				this._avatarData.SetGrowableElementShowingAbility(1, !this._isTransGender && (int)this._age >= GlobalConfig.Instance.AgeShowBeard1);
				this._avatarData.SetGrowableElementShowingAbility(2, !this._isTransGender && (int)this._age >= GlobalConfig.Instance.AgeShowBeard2);
				this._avatarData.SetGrowableElementShowingState(3, (int)this._age >= GlobalConfig.Instance.AgeShowWrinkle1);
				this._avatarData.SetGrowableElementShowingAbility(3, (int)this._age >= GlobalConfig.Instance.AgeShowWrinkle1);
				this._avatarData.SetGrowableElementShowingState(4, (int)this._age >= GlobalConfig.Instance.AgeShowWrinkle2);
				this._avatarData.SetGrowableElementShowingAbility(4, (int)this._age >= GlobalConfig.Instance.AgeShowWrinkle2);
				this._avatarData.SetGrowableElementShowingState(5, (int)this._age >= GlobalConfig.Instance.AgeShowWrinkle3);
				this._avatarData.SetGrowableElementShowingAbility(5, (int)this._age >= GlobalConfig.Instance.AgeShowWrinkle3);
			}
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x002D38A8 File Offset: 0x002D1AA8
		private void OnGenderChanged(int newIndex, int oldIndex)
		{
			this.ClearSelectedInscribedCharacter();
			this._gender = ((newIndex == 0) ? 1 : 0);
			this.UpdateAvatarGender();
			this.UpdateTransGenderLabel();
			this.InitAvatarDisplayState();
			this.RefreshAvatar();
			this.MarkAvatarDirty();
			this.RefreshHairTabsIfNeeded();
			this.bodySkinPage.RefreshSwitchSex();
		}

		// Token: 0x060062D1 RID: 25297 RVA: 0x002D3900 File Offset: 0x002D1B00
		private void OnCreationChanged(int newIndex, int oldIndex)
		{
			bool flag = newIndex == 1;
			if (flag)
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("IsNewGameSubPageAvatar", true);
				UIElement.SelectInscriptionForEvolution.SetOnInitArgs(box);
				UIManager.Instance.ShowUI(UIElement.SelectInscriptionForEvolution, true);
				bool flag2 = this.isStartViewClose;
				if (flag2)
				{
					this.creationToggleGroup.SetWithoutNotify(0);
				}
			}
			else
			{
				this.isStartViewClose = true;
				this.SetInteractable(true);
				this.parent.SetInteractable(true);
			}
		}

		// Token: 0x060062D2 RID: 25298 RVA: 0x002D3981 File Offset: 0x002D1B81
		private void OnTransGenderChanged(bool isOn)
		{
			this.ClearSelectedInscribedCharacter();
			this._isTransGender = isOn;
			this.UpdateAvatarGender();
			this.InitAvatarDisplayState();
			this.RefreshAvatar();
			this.MarkAvatarDirty();
			this.RefreshHairTabsIfNeeded();
		}

		// Token: 0x060062D3 RID: 25299 RVA: 0x002D39B8 File Offset: 0x002D1BB8
		private void UpdateAvatarGender()
		{
			bool flag = this._avatarData == null;
			if (!flag)
			{
				sbyte displayGender = this._isTransGender ? ((this._gender == 1) ? 0 : 1) : this._gender;
				this._avatarData.ChangeGender(displayGender);
			}
		}

		// Token: 0x060062D4 RID: 25300 RVA: 0x002D3A00 File Offset: 0x002D1C00
		private void UpdateTransGenderLabel()
		{
			bool flag = this.transGenderLabel == null;
			if (!flag)
			{
				LanguageKey key = (this._gender == 1) ? LanguageKey.UI_NewGame_FemaleLike : LanguageKey.UI_NewGame_MaleLike;
				this.transGenderLabel.text = LocalStringManager.Get(key);
			}
		}

		// Token: 0x060062D5 RID: 25301 RVA: 0x002D3A48 File Offset: 0x002D1C48
		private void OnAgeChanged(float newAge)
		{
			this._age = (short)newAge;
			this.UpdateAgeText();
			this.InitAvatarDisplayState();
			this.RefreshAvatar();
			this.MarkDirtyWithoutInscriptionClear();
			this.RefreshHairTabsIfNeeded();
		}

		// Token: 0x060062D6 RID: 25302 RVA: 0x002D3A78 File Offset: 0x002D1C78
		private void SaveToCustomPreset()
		{
			bool flag = this._avatarData == null;
			if (!flag)
			{
				List<AvatarPreset> presets = NewGameSubPageAvatarPresetHelper.LoadCustomPresets();
				bool flag2 = presets.Count >= 21;
				if (flag2)
				{
					this._pendingPresetData = new AvatarData(this._avatarData);
					this._pendingPresetData.ClothDisplayId = this._avatarData.ClothDisplayId;
					this._pendingPresetIsTransGender = this._isTransGender;
					this._pendingPresetGender = this._gender;
					this.SwitchToCustomPresetPage();
					this.ShowSavePresetFullDialog();
				}
				else
				{
					this.DoSavePreset(presets);
				}
			}
		}

		// Token: 0x060062D7 RID: 25303 RVA: 0x002D3B04 File Offset: 0x002D1D04
		private void DoSavePreset(List<AvatarPreset> presets)
		{
			AvatarData newData = new AvatarData(this._avatarData);
			newData.ClothDisplayId = this._avatarData.ClothDisplayId;
			presets.Add(new AvatarPreset
			{
				Name = LanguageKey.LK_NewGame_Avatar_SubPage_Preset_DefaultName.TrFormat(presets.Count + 1),
				Data = newData,
				IsTransGender = this._isTransGender,
				Gender = this._gender
			});
			NewGameSubPageAvatarPresetHelper.SaveCustomPresets(presets);
			this.SwitchToCustomPresetPage();
			this.presetPage.RefreshAllItems();
			this.presetPage.ScrollToLast();
		}

		// Token: 0x060062D8 RID: 25304 RVA: 0x002D3BA5 File Offset: 0x002D1DA5
		private void SwitchToCustomPresetPage()
		{
			this.primaryToggleGroup.Set(0, false);
			this.secondaryToggleGroupHelper.SetActiveIndex(1);
		}

		// Token: 0x060062D9 RID: 25305 RVA: 0x002D3BC4 File Offset: 0x002D1DC4
		private void ShowSavePresetFullDialog()
		{
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Title = LanguageKey.LK_NewGame_SavePreset_Title_NoSpace.Tr();
			dialogCmd.Content = LanguageKey.LK_NewGame_SavePreset_Full_Content.Tr();
			dialogCmd.Type = 1;
			dialogCmd.Yes = delegate()
			{
			};
			DialogCmd cmd = dialogCmd;
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.SetObject("Cmd", cmd);
			UIElement.Dialog.SetOnInitArgs(box);
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060062DA RID: 25306 RVA: 0x002D3C54 File Offset: 0x002D1E54
		public void TrySavePendingPreset()
		{
			bool flag = this._pendingPresetData == null;
			if (!flag)
			{
				List<AvatarPreset> presets = NewGameSubPageAvatarPresetHelper.LoadCustomPresets();
				bool flag2 = presets.Count >= 21;
				if (!flag2)
				{
					AvatarData newData = new AvatarData(this._pendingPresetData);
					newData.ClothDisplayId = this._pendingPresetData.ClothDisplayId;
					presets.Add(new AvatarPreset
					{
						Name = LocalStringManager.GetFormat("LK_NewGame_Avatar_SubPage_Preset_DefaultName", presets.Count + 1),
						Data = newData,
						IsTransGender = this._pendingPresetIsTransGender,
						Gender = this._pendingPresetGender
					});
					NewGameSubPageAvatarPresetHelper.SaveCustomPresets(presets);
					this._pendingPresetData = null;
					this.presetPage.RefreshAllItems();
					this.presetPage.ScrollToLast();
					DialogCmd dialogCmd = new DialogCmd();
					dialogCmd.Title = LanguageKey.LK_NewGame_SavePreset_Title.Tr();
					dialogCmd.Content = LanguageKey.LK_NewGame_SavePreset_AutoSaved_Content.Tr();
					dialogCmd.Type = 1;
					dialogCmd.Yes = delegate()
					{
					};
					DialogCmd cmd = dialogCmd;
					ArgumentBox box = EasyPool.Get<ArgumentBox>();
					box.SetObject("Cmd", cmd);
					UIElement.Dialog.SetOnInitArgs(box);
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x002D3DAC File Offset: 0x002D1FAC
		public void ApplyPreset(AvatarPreset preset)
		{
			bool flag = preset.Data == null;
			if (!flag)
			{
				this.ClearSelectedInscribedCharacter();
				this._avatarData = new AvatarData(preset.Data);
				this._avatarData.ClothDisplayId = preset.Data.ClothDisplayId;
				this._isTransGender = preset.IsTransGender;
				this._age = Math.Clamp(this._age, (short)this.ageSlider.minValue, (short)this.ageSlider.maxValue);
				this.InitAvatarDisplayState();
				this._gender = preset.Gender;
				this.ApplyDataToUI();
				this.RefreshAvatar();
				this._isAvatarDirty = false;
				this._hasManuallyAdjustedAppearance = false;
			}
		}

		// Token: 0x060062DC RID: 25308 RVA: 0x002D3E60 File Offset: 0x002D2060
		public AvatarData GetCurrentAvatarData()
		{
			return this._avatarData;
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x002D3E78 File Offset: 0x002D2078
		public bool IsAvatarDirty()
		{
			return this._isAvatarDirty;
		}

		// Token: 0x060062DE RID: 25310 RVA: 0x002D3E80 File Offset: 0x002D2080
		public void MarkAvatarDirty()
		{
			this.ClearSelectedInscribedCharacter();
			this.MarkDirtyWithoutInscriptionClear();
		}

		// Token: 0x060062DF RID: 25311 RVA: 0x002D3E91 File Offset: 0x002D2091
		public void MarkDirtyWithoutInscriptionClear()
		{
			this._isAvatarDirty = true;
			this._hasManuallyAdjustedAppearance = true;
			this.presetPage.ClearSelection();
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x002D3EAE File Offset: 0x002D20AE
		public bool GetIsTransGender()
		{
			return this._isTransGender;
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x002D3EB8 File Offset: 0x002D20B8
		private void UpdateAgeText()
		{
			bool flag = this.ageText == null;
			if (!flag)
			{
				this.ageText.text = LocalStringManager.GetFormat(LanguageKey.LK_Age, this._age);
				LanguageRuleTips component = this.ageText.GetComponent<LanguageRuleTips>();
				if (component != null)
				{
					component.Refresh();
				}
			}
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x002D3F10 File Offset: 0x002D2110
		private void OnSecondaryPageChanged(int newIndex, int oldIndex)
		{
			int primaryIndex = this.primaryToggleGroup.GetActiveIndex();
			bool flag = primaryIndex >= 0 && newIndex >= 0;
			if (flag)
			{
				this._secondaryPageIndexByPrimaryPage[primaryIndex] = newIndex;
			}
			this.UpdateSubPageVisibility();
		}

		// Token: 0x060062E3 RID: 25315 RVA: 0x002D3F54 File Offset: 0x002D2154
		private void OnPrimaryPageChanged(int newIndex, int oldIndex)
		{
			bool flag = oldIndex >= 0;
			if (flag)
			{
				int currentSecondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
				bool flag2 = currentSecondaryIndex >= 0;
				if (flag2)
				{
					this._secondaryPageIndexByPrimaryPage[oldIndex] = currentSecondaryIndex;
				}
			}
			bool flag3 = this.secondaryToggleGroupHelper != null;
			if (flag3)
			{
				List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs(newIndex);
				this.secondaryToggleGroupHelper.Refresh(configs, true);
				this.RestoreSecondaryPageIndex(newIndex, configs);
			}
			this.UpdateSubPageVisibility();
		}

		// Token: 0x060062E4 RID: 25316 RVA: 0x002D3FD4 File Offset: 0x002D21D4
		public void RefreshHairTabsIfNeeded()
		{
			bool beardStateChanged = this.UpdateBeardShowingAbility();
			bool flag = beardStateChanged;
			if (flag)
			{
				this.RefreshAvatar();
			}
			int primaryIndex = this.primaryToggleGroup.GetActiveIndex();
			bool flag2 = primaryIndex != 2;
			if (!flag2)
			{
				int secondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
				bool isOnBeardTab = secondaryIndex >= 2;
				bool flag3 = isOnBeardTab;
				if (flag3)
				{
					sbyte beardType = (secondaryIndex == 2) ? 1 : 2;
					bool flag4 = !this.IsBeardAvailable(beardType);
					if (flag4)
					{
						this.secondaryToggleGroupHelper.SetActiveIndex(0);
					}
				}
				List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs(primaryIndex);
				this.secondaryToggleGroupHelper.Refresh(configs, true);
				this.RestoreSecondaryPageIndex(primaryIndex, configs);
			}
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x002D4080 File Offset: 0x002D2280
		private void RestoreSecondaryPageIndex(int primaryIndex, List<AvatarSecondaryToggleConfig> configs)
		{
			bool flag = configs == null || configs.Count == 0;
			if (!flag)
			{
				int targetIndex = this.GetValidSecondaryPageIndex(primaryIndex, configs);
				this.secondaryToggleGroupHelper.SetActiveIndex(targetIndex);
				this._secondaryPageIndexByPrimaryPage[primaryIndex] = targetIndex;
			}
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x002D40C8 File Offset: 0x002D22C8
		private int GetValidSecondaryPageIndex(int primaryIndex, List<AvatarSecondaryToggleConfig> configs)
		{
			int savedIndex;
			bool flag = this._secondaryPageIndexByPrimaryPage.TryGetValue(primaryIndex, out savedIndex) && savedIndex >= 0 && savedIndex < configs.Count && configs[savedIndex].Interactable;
			int result;
			if (flag)
			{
				result = savedIndex;
			}
			else
			{
				for (int i = 0; i < configs.Count; i++)
				{
					bool interactable = configs[i].Interactable;
					if (interactable)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x002D4140 File Offset: 0x002D2340
		private bool UpdateBeardShowingAbility()
		{
			bool flag = this._avatarData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool beard1Available = this.IsBeardAvailable(1);
				bool beard2Available = this.IsBeardAvailable(2);
				bool hasChange = false;
				this._avatarData.SetGrowableElementShowingAbility(1, beard1Available);
				this._avatarData.SetGrowableElementShowingAbility(2, beard2Available);
				bool flag2 = !beard1Available && this._avatarData.GetGrowableElementShowingState(1);
				if (flag2)
				{
					this._avatarData.SetGrowableElementShowingState(1, false);
					hasChange = true;
				}
				else
				{
					bool flag3 = beard1Available && !this._avatarData.GetGrowableElementShowingState(1) && this._avatarData.Beard1Id != 1;
					if (flag3)
					{
						this._avatarData.SetGrowableElementShowingState(1, true);
						hasChange = true;
					}
				}
				bool flag4 = !beard2Available && this._avatarData.GetGrowableElementShowingState(2);
				if (flag4)
				{
					this._avatarData.SetGrowableElementShowingState(2, false);
					hasChange = true;
				}
				else
				{
					bool flag5 = beard2Available && !this._avatarData.GetGrowableElementShowingState(2) && this._avatarData.Beard2Id != 1;
					if (flag5)
					{
						this._avatarData.SetGrowableElementShowingState(2, true);
						hasChange = true;
					}
				}
				result = hasChange;
			}
			return result;
		}

		// Token: 0x060062E8 RID: 25320 RVA: 0x002D426C File Offset: 0x002D246C
		private List<AvatarSecondaryToggleConfig> GetSecondaryToggleConfigs(int primaryIndex)
		{
			List<AvatarSecondaryToggleConfig> configs = new List<AvatarSecondaryToggleConfig>();
			switch (primaryIndex)
			{
			case 0:
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_preset", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_custom", true));
				break;
			case 1:
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_body_skin", true));
				break;
			case 2:
			{
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_hair_front", true));
				bool backHairEnabled = this.IsBackHairEnabled();
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_hair_back", backHairEnabled));
				bool beard1Enabled = this.IsBeardAvailable(1);
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_beard_upper", beard1Enabled));
				bool beard2Enabled = this.IsBeardAvailable(2);
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_beard_lower", beard2Enabled));
				break;
			}
			case 3:
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_eyebrow", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_eye", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_nose", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_mouth", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_feature", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_feature", true));
				break;
			case 4:
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_cloth", true));
				break;
			}
			return configs;
		}

		// Token: 0x060062E9 RID: 25321 RVA: 0x002D43D4 File Offset: 0x002D25D4
		private void UpdateSubPageVisibility()
		{
			int primaryIndex = this.primaryToggleGroup.GetActiveIndex();
			bool flag = primaryIndex < 0;
			if (!flag)
			{
				int secondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
				bool flag2 = secondaryIndex < 0;
				if (!flag2)
				{
					NewGameSubPageAvatarPageBase targetPage = null;
					switch (primaryIndex)
					{
					case 0:
						targetPage = this.presetPage;
						break;
					case 1:
						targetPage = this.bodySkinPage;
						break;
					case 2:
						targetPage = ((secondaryIndex < 2) ? this.hairPage : this.beardPage);
						break;
					case 3:
						targetPage = ((secondaryIndex < 4) ? this.facePartPage : this.featurePage);
						break;
					case 4:
						targetPage = this.clothPage;
						break;
					}
					bool needSwitchPage = targetPage != this._currentActiveSubPage;
					bool flag3 = needSwitchPage;
					if (flag3)
					{
						bool flag4 = this._currentActiveSubPage != null;
						if (flag4)
						{
							this._currentActiveSubPage.gameObject.SetActive(false);
						}
						bool flag5 = targetPage != null;
						if (flag5)
						{
							targetPage.gameObject.SetActive(true);
						}
						this._currentActiveSubPage = targetPage;
					}
					switch (primaryIndex)
					{
					case 0:
						this.presetPage.SetPresetType(secondaryIndex == 1);
						break;
					case 2:
					{
						bool flag6 = secondaryIndex < 2;
						if (flag6)
						{
							this.hairPage.SetHairType(secondaryIndex == 1);
						}
						else
						{
							this.beardPage.SetBeardType(secondaryIndex == 3);
						}
						break;
					}
					case 3:
					{
						bool flag7 = secondaryIndex < 4;
						if (flag7)
						{
							if (!true)
							{
							}
							EAvatarElementsType eavatarElementsType;
							switch (secondaryIndex)
							{
							case 0:
								eavatarElementsType = EAvatarElementsType.EyeBrow;
								break;
							case 1:
								eavatarElementsType = EAvatarElementsType.Eye;
								break;
							case 2:
								eavatarElementsType = EAvatarElementsType.Nose;
								break;
							case 3:
								eavatarElementsType = EAvatarElementsType.Mouth;
								break;
							default:
								eavatarElementsType = EAvatarElementsType.EyeBrow;
								break;
							}
							if (!true)
							{
							}
							EAvatarElementsType partType = eavatarElementsType;
							this.facePartPage.SetPartType(partType);
						}
						else
						{
							EAvatarElementsType featureType = (secondaryIndex == 4) ? EAvatarElementsType.Feature1 : EAvatarElementsType.Feature2;
							this.featurePage.SetFeatureType(featureType);
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x060062EA RID: 25322 RVA: 0x002D45C8 File Offset: 0x002D27C8
		private void SetInteractable(bool interactable)
		{
			foreach (DisableStyleRoot root in this.grayRootArray)
			{
				root.SetStyleEffect(!interactable, false);
			}
			foreach (CButton button in this.buttonArray)
			{
				button.interactable = interactable;
			}
			foreach (CToggle toggle in this.grayToggleArray)
			{
				toggle.interactable = interactable;
			}
		}

		// Token: 0x060062EB RID: 25323 RVA: 0x002D4658 File Offset: 0x002D2858
		private void OnInscriptionChange(ArgumentBox argumentBox)
		{
			CheckInscriptionCharData charData;
			argumentBox.Get<CheckInscriptionCharData>("PersonnelCharData", out charData);
			bool flag = charData == null;
			if (!flag)
			{
				AvatarData avatar = charData.Character.Avatar;
				this._gender = avatar.Gender;
				this._isTransGender = (charData.Character.Gender != avatar.Gender);
				this._age = charData.Character.CurrAge;
				this.SetInscriptionChange(new AvatarData(avatar)
				{
					ClothDisplayId = charData.Character.ClothingDisplayId
				});
				this.isStartViewClose = false;
				this.SetInteractable(false);
				this.parent.SetInteractable(false);
				this.creationToggleGroup.Set(1, false);
				this.primaryToggleGroup.Set(this.primaryToggleGroup.Count() - 1, true);
				this.genderToggleGroup.SetWithoutNotify((this._gender == 1) ? 0 : 1);
				this.ageSlider.value = (float)this._age;
				this.parent.InscribedCharacter = charData.Character;
				this._selectedInscribedCharKey = charData.Key;
				this._selectedInscribedChar = charData.Character;
				base.CreationInfoMap["Surname"] = this._selectedInscribedChar.Surname;
				base.CreationInfoMap["GivenName"] = this._selectedInscribedChar.GivenName;
				this.transGenderToggle.SetIsOnWithoutNotify(this._isTransGender);
			}
		}

		// Token: 0x060062EC RID: 25324 RVA: 0x002D47CA File Offset: 0x002D29CA
		private void SetInscriptionChange(AvatarData avatarData)
		{
			this.ClearSelectedInscribedCharacter();
			this._avatarData = avatarData;
			this.UpdateAvatarGender();
			this.RefreshAvatar();
			this.MarkAvatarDirty();
			this.RefreshHairTabsIfNeeded();
			this._hasManuallyAdjustedAppearance = false;
		}

		// Token: 0x060062ED RID: 25325 RVA: 0x002D4800 File Offset: 0x002D2A00
		public void RefreshAvatar()
		{
			bool flag = this._avatarData == null;
			if (!flag)
			{
				this.avatarDisplay.Refresh(this._avatarData, this._age);
				this.toggleHelper.Refresh(this._avatarData, this._age, this._gender);
				this.UpdateCharmText();
				NewGameSubPageAvatarPageBase currentActiveSubPage = this._currentActiveSubPage;
				if (currentActiveSubPage != null)
				{
					currentActiveSubPage.UpdateUI();
				}
			}
		}

		// Token: 0x060062EE RID: 25326 RVA: 0x002D486C File Offset: 0x002D2A6C
		private void UpdateCharmText()
		{
			bool flag = this.charmText == null || this._avatarData == null;
			if (!flag)
			{
				short charm = Math.Clamp(this._avatarData.GetCharm(this._age, this._avatarData.ClothDisplayId), 0, 900);
				this.charmText.text = CommonUtils.GetCharmLevelText(charm, this._gender, this._age, this._avatarData.ClothDisplayId, false, true);
				bool flag2 = this.charmTip != null;
				if (flag2)
				{
					this.charmTip.enabled = true;
					this.charmTip.IsLanguageKey = false;
					this.charmTip.Type = TipType.CommonTip;
					TooltipInvoker tooltipInvoker = this.charmTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.charmTip.RuntimeParam.Clear();
					CommonTipSimpleRuntime runtime = CommonTip.DefValue.Charm.BuildSimple();
					this.charmTip.RuntimeParam.SetObject("Runtime", runtime);
				}
			}
		}

		// Token: 0x060062EF RID: 25327 RVA: 0x002D497F File Offset: 0x002D2B7F
		public AvatarData GetAvatarData()
		{
			return this._avatarData;
		}

		// Token: 0x060062F0 RID: 25328 RVA: 0x002D4987 File Offset: 0x002D2B87
		public sbyte GetGender()
		{
			return this._gender;
		}

		// Token: 0x060062F1 RID: 25329 RVA: 0x002D498F File Offset: 0x002D2B8F
		public short GetAge()
		{
			return this._age;
		}

		// Token: 0x060062F2 RID: 25330 RVA: 0x002D4998 File Offset: 0x002D2B98
		public bool IsBackHairEnabled()
		{
			bool flag = this._avatarData == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)this._avatarData.AvatarId);
				bool flag2 = group == null;
				if (flag2)
				{
					result = true;
				}
				else
				{
					AvatarAsset frontHairAsset = group.Get(EAvatarElementsType.Hair1, new short[]
					{
						this._avatarData.FrontHairId
					});
					bool flag3 = frontHairAsset == null;
					result = (flag3 || !frontHairAsset.Config.DisableRelativeType);
				}
			}
			return result;
		}

		// Token: 0x060062F3 RID: 25331 RVA: 0x002D4A18 File Offset: 0x002D2C18
		public Func<List<EyeRes>, List<EyeRes>> GetEyesFilter()
		{
			return delegate(List<EyeRes> eyesList)
			{
				List<EyeRes> result;
				if (eyesList == null)
				{
					result = null;
				}
				else
				{
					result = eyesList.FindAll((EyeRes e) => e.LeftEye.SubId == 0 && e.RightEye.SubId == 0);
				}
				return result;
			};
		}

		// Token: 0x060062F4 RID: 25332 RVA: 0x002D4A4C File Offset: 0x002D2C4C
		public bool IsBeardAvailable(sbyte beardType)
		{
			bool flag = this._gender != 1 || this._isTransGender;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = beardType == 1;
				if (flag2)
				{
					result = ((int)this._age >= GlobalConfig.Instance.AgeShowBeard1);
				}
				else
				{
					bool flag3 = beardType == 2;
					result = (flag3 && (int)this._age >= GlobalConfig.Instance.AgeShowBeard2);
				}
			}
			return result;
		}

		// Token: 0x060062F5 RID: 25333 RVA: 0x002D4AB9 File Offset: 0x002D2CB9
		private void ClearSelectedInscribedCharacter()
		{
			this._selectedInscribedCharKey = InscribedCharacterKey.Invalid;
			this._selectedInscribedChar = null;
			this.parent.InscribedCharacter = null;
		}

		// Token: 0x060062F6 RID: 25334 RVA: 0x002D4ADC File Offset: 0x002D2CDC
		public void ApplyInscribedCharacter(InscribedCharacterKey key, InscribedCharacter character)
		{
			bool flag = character == null;
			if (!flag)
			{
				this._selectedInscribedCharKey = key;
				this._selectedInscribedChar = character;
				this.parent.InscribedCharacter = character;
				base.CreationInfoMap["Surname"] = character.Surname;
				base.CreationInfoMap["GivenName"] = character.GivenName;
				bool flag2 = character.Avatar != null;
				if (flag2)
				{
					this._avatarData = new AvatarData(character.Avatar);
					this._avatarData.ClothDisplayId = character.ClothingDisplayId;
				}
				this._gender = character.Gender;
				this._age = character.CurrAge;
				this._isTransGender = (character.Avatar != null && character.Gender != character.Avatar.GetGender());
				this.InitAvatarDisplayState();
				this.ApplyDataToUI();
				this._isAvatarDirty = false;
				this._hasManuallyAdjustedAppearance = false;
				this.RefreshAvatar();
				this.RefreshHairTabsIfNeeded();
			}
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x002D4BDA File Offset: 0x002D2DDA
		public bool HasSelectedInscribedChar()
		{
			return !this._selectedInscribedCharKey.Equals(InscribedCharacterKey.Invalid);
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x002D4BEF File Offset: 0x002D2DEF
		public InscribedCharacter GetSelectedInscribedChar()
		{
			return this._selectedInscribedChar;
		}

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x060062F9 RID: 25337 RVA: 0x002D4BF7 File Offset: 0x002D2DF7
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x060062FA RID: 25338 RVA: 0x002D4BFA File Offset: 0x002D2DFA
		// (set) Token: 0x060062FB RID: 25339 RVA: 0x002D4C02 File Offset: 0x002D2E02
		public override bool StartGameChecked
		{
			get
			{
				return this._checked;
			}
			set
			{
				this._checked = value;
			}
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x002D4C0C File Offset: 0x002D2E0C
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			bool flag = protagonistCreationInfo == null;
			if (!flag)
			{
				protagonistCreationInfo.InscribedChar = this._selectedInscribedChar;
				bool flag2 = this._selectedInscribedChar != null;
				if (flag2)
				{
					protagonistCreationInfo.Surname = this._selectedInscribedChar.Surname;
					base.CreationInfoMap["Surname"] = protagonistCreationInfo.Surname;
					protagonistCreationInfo.GivenName = this._selectedInscribedChar.GivenName;
					base.CreationInfoMap["GivenName"] = protagonistCreationInfo.GivenName;
					protagonistCreationInfo.Gender = this._selectedInscribedChar.Gender;
					protagonistCreationInfo.Age = this._age;
					bool flag3 = this._avatarData != null;
					if (flag3)
					{
						protagonistCreationInfo.Avatar = new AvatarData(this._avatarData).FormatDisabledElements();
					}
					else
					{
						bool flag4 = this._selectedInscribedChar.Avatar != null;
						if (flag4)
						{
							protagonistCreationInfo.Avatar = new AvatarData(this._selectedInscribedChar.Avatar).FormatDisabledElements();
							protagonistCreationInfo.Avatar.ClothDisplayId = this._selectedInscribedChar.ClothingDisplayId;
						}
					}
					short clothDisplayId = (this._avatarData != null) ? this._avatarData.ClothDisplayId : this._selectedInscribedChar.ClothingDisplayId;
					protagonistCreationInfo.ClothingTemplateId = ItemTemplateHelper.GetClothingTemplateIdByDisplayId((byte)clothDisplayId);
					base.CreationInfoMap["ClothingTemplateId"] = protagonistCreationInfo.ClothingTemplateId.ToString();
				}
				else
				{
					protagonistCreationInfo.Gender = this._gender;
					protagonistCreationInfo.Age = this._age;
					bool flag5 = this._avatarData != null;
					if (flag5)
					{
						bool flag6 = !this._avatarData.GetGrowableElementShowingState(1);
						if (flag6)
						{
							this._avatarData.Beard1Id = 1;
						}
						bool flag7 = !this._avatarData.GetGrowableElementShowingState(2);
						if (flag7)
						{
							this._avatarData.Beard2Id = 1;
						}
						protagonistCreationInfo.Avatar = this._avatarData.FormatDisabledElements();
						protagonistCreationInfo.ClothingTemplateId = ItemTemplateHelper.GetClothingTemplateIdByDisplayId((byte)this._avatarData.ClothDisplayId);
						base.CreationInfoMap["ClothingTemplateId"] = protagonistCreationInfo.ClothingTemplateId.ToString();
					}
				}
				base.CreationInfoMap["Gender"] = protagonistCreationInfo.Gender.ToString();
				base.CreationInfoMap["IsTransGender"] = ((this._selectedInscribedChar != null) ? (this._selectedInscribedChar.Avatar != null && this._selectedInscribedChar.Gender != this._selectedInscribedChar.Avatar.GetGender()).ToString() : this._isTransGender.ToString());
				base.CreationInfoMap["Age"] = protagonistCreationInfo.Age.ToString();
			}
		}

		// Token: 0x040044CE RID: 17614
		[Header("左侧功能")]
		[SerializeField]
		private CButton randomButton;

		// Token: 0x040044CF RID: 17615
		[SerializeField]
		private CToggleGroup genderToggleGroup;

		// Token: 0x040044D0 RID: 17616
		[SerializeField]
		private CToggle transGenderToggle;

		// Token: 0x040044D1 RID: 17617
		[SerializeField]
		private TextMeshProUGUI transGenderLabel;

		// Token: 0x040044D2 RID: 17618
		[SerializeField]
		private CSlider ageSlider;

		// Token: 0x040044D3 RID: 17619
		[SerializeField]
		private Transform ageTickContainer;

		// Token: 0x040044D4 RID: 17620
		[SerializeField]
		private RectTransform ageTickTemplate;

		// Token: 0x040044D5 RID: 17621
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarDisplay;

		// Token: 0x040044D6 RID: 17622
		[SerializeField]
		private CButton savePresetButton;

		// Token: 0x040044D7 RID: 17623
		[SerializeField]
		private TextMeshProUGUI ageText;

		// Token: 0x040044D8 RID: 17624
		[SerializeField]
		private TextMeshProUGUI charmText;

		// Token: 0x040044D9 RID: 17625
		[SerializeField]
		private TooltipInvoker charmTip;

		// Token: 0x040044DA RID: 17626
		[SerializeField]
		private DisableStyleRoot[] grayRootArray;

		// Token: 0x040044DB RID: 17627
		[SerializeField]
		private CButton[] buttonArray;

		// Token: 0x040044DC RID: 17628
		[SerializeField]
		private CToggle[] grayToggleArray;

		// Token: 0x040044DD RID: 17629
		[SerializeField]
		private CToggleGroup creationToggleGroup;

		// Token: 0x040044DE RID: 17630
		[Header("右侧页签")]
		[SerializeField]
		private CToggleGroup primaryToggleGroup;

		// Token: 0x040044DF RID: 17631
		[SerializeField]
		private AvatarSecondaryToggleGroupHelper secondaryToggleGroupHelper;

		// Token: 0x040044E0 RID: 17632
		[Header("子页面组件")]
		[SerializeField]
		private NewGameSubPageAvatarPresetPage presetPage;

		// Token: 0x040044E1 RID: 17633
		[SerializeField]
		private NewGameSubPageAvatarBodySkinPage bodySkinPage;

		// Token: 0x040044E2 RID: 17634
		[SerializeField]
		private NewGameSubPageAvatarHairPage hairPage;

		// Token: 0x040044E3 RID: 17635
		[SerializeField]
		private NewGameSubPageAvatarBeardPage beardPage;

		// Token: 0x040044E4 RID: 17636
		[SerializeField]
		private NewGameSubPageAvatarFacePartPage facePartPage;

		// Token: 0x040044E5 RID: 17637
		[SerializeField]
		private NewGameSubPageAvatarFeaturePage featurePage;

		// Token: 0x040044E6 RID: 17638
		[SerializeField]
		private NewGameSubPageAvatarClothPage clothPage;

		// Token: 0x040044E7 RID: 17639
		[Header("其他")]
		[SerializeField]
		private NewGameSubPageAvatarToggleHelper toggleHelper;

		// Token: 0x040044E8 RID: 17640
		private AvatarData _avatarData;

		// Token: 0x040044E9 RID: 17641
		private sbyte _gender = 1;

		// Token: 0x040044EA RID: 17642
		private bool _isTransGender;

		// Token: 0x040044EB RID: 17643
		private short _age;

		// Token: 0x040044EC RID: 17644
		private bool _checked;

		// Token: 0x040044ED RID: 17645
		private NewGameSubPageAvatarPageBase _currentActiveSubPage;

		// Token: 0x040044EE RID: 17646
		private bool _isAvatarDirty;

		// Token: 0x040044EF RID: 17647
		private bool _hasManuallyAdjustedAppearance;

		// Token: 0x040044F0 RID: 17648
		private InscribedCharacterKey _selectedInscribedCharKey = InscribedCharacterKey.Invalid;

		// Token: 0x040044F1 RID: 17649
		private InscribedCharacter _selectedInscribedChar;

		// Token: 0x040044F2 RID: 17650
		private readonly Dictionary<int, int> _secondaryPageIndexByPrimaryPage = new Dictionary<int, int>();

		// Token: 0x040044F3 RID: 17651
		private bool isStartViewClose = false;

		// Token: 0x040044F4 RID: 17652
		private AvatarData _pendingPresetData;

		// Token: 0x040044F5 RID: 17653
		private bool _pendingPresetIsTransGender;

		// Token: 0x040044F6 RID: 17654
		private sbyte _pendingPresetGender;
	}
}
