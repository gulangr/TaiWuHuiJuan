using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Save;
using GameData.Domains.Character.Creation;
using GameData.Domains.Global.Inscription;
using GameData.Domains.World;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x0200080D RID: 2061
	public class NewGameSubPageOther : NewGameSubPage
	{
		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x06006523 RID: 25891 RVA: 0x002E3D32 File Offset: 0x002E1F32
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x06006524 RID: 25892 RVA: 0x002E3D39 File Offset: 0x002E1F39
		private int SwitchOnIndex
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x06006525 RID: 25893 RVA: 0x002E3D3C File Offset: 0x002E1F3C
		private int SwitchOffIndex
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x06006526 RID: 25894 RVA: 0x002E3D3F File Offset: 0x002E1F3F
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x06006527 RID: 25895 RVA: 0x002E3D42 File Offset: 0x002E1F42
		// (set) Token: 0x06006528 RID: 25896 RVA: 0x002E3D4A File Offset: 0x002E1F4A
		public override bool StartGameChecked { get; set; }

		// Token: 0x06006529 RID: 25897 RVA: 0x002E3D54 File Offset: 0x002E1F54
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			foreach (NewGameSubPageWorldDetailItem setting in this.detailItemList)
			{
				int value = setting.GetSettingValue();
				worldCreationInfo.Set(setting.ConfigItem.TemplateId, (byte)value);
				base.CreationInfoMap[setting.ConfigItem.SaveFileKey] = value.ToString();
			}
			base.CreationInfoMap["InscriptionForEvolutionEnabled"] = (this.inscriptionChar.SwitchToggle.isOn ? "1" : "0");
			base.CreationInfoMap["InscriptionAgeUnified"] = this._savedUnifiedAge.ToString();
			base.CreationInfoMap["InscriptionAgeRange"] = this._savedRandomRange.ToString();
			base.CreationInfoMap["InscriptionAgeEnabled"] = (this._savedAgeSettingEnabled ? "1" : "0");
			List<InscribedCharacterKey> includedInscribedCharList = this._includedInscribedCharList;
			bool flag = includedInscribedCharList != null && includedInscribedCharList.Count > 0;
			if (flag)
			{
				List<string> keyStrings = new List<string>(this._includedInscribedCharList.Count);
				foreach (InscribedCharacterKey key in this._includedInscribedCharList)
				{
					keyStrings.Add(string.Format("{0}_{1}", key.WorldId, key.CharId));
				}
				base.CreationInfoMap["InscribedCharKeys_Evolution"] = string.Join(",", keyStrings);
			}
			else
			{
				base.CreationInfoMap["InscribedCharKeys_Evolution"] = string.Empty;
			}
			bool flag2 = !this.inscriptionChar.SwitchToggle.isOn;
			if (!flag2)
			{
				List<InscribedCharacterKey> list = new List<InscribedCharacterKey>();
				bool flag3 = this._includedInscribedCharList != null;
				if (flag3)
				{
					list.AddRange(this._includedInscribedCharList);
				}
				bool flag4 = this._autoIncludeInscribedChar && list.Count < GlobalConfig.Instance.InscriptionCharForCreationMaxCount;
				if (flag4)
				{
					foreach (InscribedCharacterKey key2 in this._canIncludeInscribedCharList)
					{
						bool flag5 = list.Count >= GlobalConfig.Instance.InscriptionCharForCreationMaxCount;
						if (flag5)
						{
							break;
						}
						bool flag6 = !list.Contains(key2);
						if (flag6)
						{
							list.Add(key2);
						}
					}
				}
				bool flag7 = list != null && list.Count > 0;
				if (flag7)
				{
					List<short> ageList = new List<short>(list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						InscribedCharacterKey key3 = list[i];
						short targetAge;
						short age = (this._includedInscribedCharAges != null && this._includedInscribedCharAges.TryGetValue(key3, out targetAge)) ? targetAge : -1;
						ageList.Add(age);
					}
					GameDataBridge.AddMethodCall<List<InscribedCharacterKey>, List<short>>(-1, 3, 13, list, ageList);
				}
			}
		}

		// Token: 0x0600652A RID: 25898 RVA: 0x002E40A8 File Offset: 0x002E22A8
		public override void Init()
		{
			this.canvasGroupHover.alpha = 0f;
			string[] switchOptions = new string[]
			{
				LanguageKey.LK_Option_On.Tr(),
				LanguageKey.LK_Option_Off.Tr()
			};
			int tutorialValue = SingletonObject.getInstance<GlobalSettings>().Guiding ? this.SwitchOnIndex : this.SwitchOffIndex;
			string texturePrefix = "Tutorial".ToLower();
			string title = LanguageKey.LK_NewGame_Other_Tutorial_Title.Tr();
			string desc = LanguageKey.LK_NewGame_Other_Tutorial_Desc.Tr();
			this.tutorial.Init(texturePrefix, title, desc, tutorialValue, true, switchOptions, new Action<byte, byte>(this.OnTutorialValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			bool flag = Save.SaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			byte globalLabelStatus = Save.SaveData.GlobalLabelStatus;
			if (!true)
			{
			}
			int num;
			if (globalLabelStatus != 9)
			{
				if (globalLabelStatus != 11)
				{
					if (globalLabelStatus != 15)
					{
						num = 0;
					}
					else
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
			}
			if (!true)
			{
			}
			int encyclopediaValue = num;
			string[] encyclopediaOptions = new string[]
			{
				LanguageKey.LK_Encyclopedia_LevelButton_Low.Tr(),
				LanguageKey.LK_Encyclopedia_LevelButton_Mid.Tr(),
				LanguageKey.LK_Encyclopedia_LevelButton_High.Tr()
			};
			string texturePrefix2 = "Encyclopedia".ToLower();
			string title2 = LanguageKey.LK_NewGame_Other_Encyclopedia_Title.Tr();
			string desc2 = LanguageKey.LK_NewGame_Other_Encyclopedia_Desc.Tr();
			this.encyclopedia.Init(texturePrefix2, title2, desc2, encyclopediaValue, false, encyclopediaOptions, new Action<byte, byte>(this.OnEncyclopediaValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.rootProgress.SetActive(ViewNewGame.PastPerformArea);
			bool isQuickStartGame = ViewNewGame.PastPerformArea && this.SettingData.QuickStartGame;
			int quickStartGameValue = isQuickStartGame ? this.SwitchOnIndex : this.SwitchOffIndex;
			string texturePrefix3 = "QuickStartGame".ToLower();
			string title3 = LanguageKey.LK_NewGame_QuickStartGame.Tr();
			string desc3 = LanguageKey.LK_NewGame_QuickStartGame_Tip.Tr();
			this.quickStartGame.Init(texturePrefix3, title3, desc3, quickStartGameValue, true, switchOptions, new Action<byte, byte>(this.OnQuickStartGameValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.SettingData.SetQuickStartGame(isQuickStartGame);
			string enabledStr;
			bool isInscriptionEnabled = base.CreationInfoMap.TryGetValue("InscriptionForEvolutionEnabled", out enabledStr) && enabledStr == "1";
			byte inscriptionCharValue = isInscriptionEnabled ? ((byte)this.SwitchOnIndex) : ((byte)this.SwitchOffIndex);
			string keysStr;
			bool flag2 = isInscriptionEnabled && base.CreationInfoMap.TryGetValue("InscribedCharKeys_Evolution", out keysStr) && !string.IsNullOrEmpty(keysStr);
			if (flag2)
			{
				this.LoadInscribedCharKeys(keysStr);
			}
			string unifiedAgeStr;
			short unifiedAge;
			bool flag3 = base.CreationInfoMap.TryGetValue("InscriptionAgeUnified", out unifiedAgeStr) && short.TryParse(unifiedAgeStr, out unifiedAge);
			if (flag3)
			{
				this._savedUnifiedAge = unifiedAge;
			}
			string rangeStr;
			short range;
			bool flag4 = base.CreationInfoMap.TryGetValue("InscriptionAgeRange", out rangeStr) && short.TryParse(rangeStr, out range);
			if (flag4)
			{
				this._savedRandomRange = range;
			}
			string ageEnabledStr;
			bool flag5 = base.CreationInfoMap.TryGetValue("InscriptionAgeEnabled", out ageEnabledStr);
			if (flag5)
			{
				this._savedAgeSettingEnabled = (ageEnabledStr == "1");
			}
			this.OnInscriptionCharValueChanged(0, inscriptionCharValue);
			string texturePrefix4 = "Inscription".ToLower();
			string title4 = LanguageKey.LK_NewGame_InscriptionCharForCreation.Tr();
			string desc4 = LanguageKey.LK_NewGame_InscriptionCharForCreation_Tip.Tr();
			this.inscriptionChar.Init(texturePrefix4, title4, desc4, (int)inscriptionCharValue, true, switchOptions, new Action<byte, byte>(this.OnInscriptionCharValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.buttonSelectInscriptionChar.ClearAndAddListener(new Action(this.OpenCheckInscriptionForInclude));
			this.RefreshInscriptionCharCount();
			this.restrictOptionsBehavior.Init(9, 3, new Action<byte, byte>(this.OnValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.restrictOptionsBehavior.SetInteractable(true);
			this.allowRandomTaiwuHeir.Init(10, 3, new Action<byte, byte>(this.OnValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.allowRandomTaiwuHeir.SetInteractable(true);
			this.worldPopulation.Init(8, 3, new Action<byte, byte>(this.OnValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.worldPopulation.SetInteractable(true);
			this.characterLifeSpan.Init(0, 3, new Action<byte, byte>(this.OnValueChanged), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
			this.characterLifeSpan.SetInteractable(true);
			this.RefreshToggleHelper();
			int restrictOptionsBehaviorValue = ViewNewGame.TempWorldCreationInfo.Get(this.restrictOptionsBehavior.ConfigItem.TemplateId);
			this.restrictOptionsBehavior.SetWithoutNotify(restrictOptionsBehaviorValue);
			int allowRandomTaiwuHeirValue = ViewNewGame.TempWorldCreationInfo.Get(this.allowRandomTaiwuHeir.ConfigItem.TemplateId);
			this.allowRandomTaiwuHeir.SetWithoutNotify(allowRandomTaiwuHeirValue);
			int worldPopulationValue = ViewNewGame.TempWorldCreationInfo.Get(this.worldPopulation.ConfigItem.TemplateId);
			this.worldPopulation.SetWithoutNotify(worldPopulationValue);
			int characterLifeSpanValue = ViewNewGame.TempWorldCreationInfo.Get(this.characterLifeSpan.ConfigItem.TemplateId);
			this.characterLifeSpan.SetWithoutNotify(characterLifeSpanValue);
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x002E461D File Offset: 0x002E281D
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ConfirmIncludedInscriptionChar, new GEvent.Callback(this.OnConfirmIncludedInscriptionChar));
			GEvent.Add(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x002E4655 File Offset: 0x002E2855
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ConfirmIncludedInscriptionChar, new GEvent.Callback(this.OnConfirmIncludedInscriptionChar));
			GEvent.Remove(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x002E4690 File Offset: 0x002E2890
		private void RefreshToggleHelper()
		{
			NewGameSubPageOtherToggleHelper newGameSubPageOtherToggleHelper = this.toggleHelper;
			bool isOn = this.tutorial.SwitchToggle.isOn;
			bool activeSelf = this.rootProgress.activeSelf;
			bool isOn2 = this.quickStartGame.SwitchToggle.isOn;
			bool activeSelf2 = this.rootSelectInscriptionChar.activeSelf;
			List<InscribedCharacterKey> includedInscribedCharList = this._includedInscribedCharList;
			newGameSubPageOtherToggleHelper.Refresh(isOn, activeSelf, isOn2, activeSelf2, (includedInscribedCharList != null) ? includedInscribedCharList.Count : 0);
		}

		// Token: 0x0600652E RID: 25902 RVA: 0x002E46F2 File Offset: 0x002E28F2
		private void OnValueChanged(byte templateId, byte value)
		{
			ViewNewGame.TempWorldCreationInfo.Set(templateId, value);
			this.RefreshItemInfo();
		}

		// Token: 0x0600652F RID: 25903 RVA: 0x002E4709 File Offset: 0x002E2909
		private void OnTutorialValueChanged(byte templateId, byte value)
		{
			SingletonObject.getInstance<GlobalSettings>().Guiding = ((int)value == this.SwitchOnIndex);
			this.RefreshToggleHelper();
		}

		// Token: 0x06006530 RID: 25904 RVA: 0x002E4728 File Offset: 0x002E2928
		private void OnEncyclopediaValueChanged(byte templateId, byte value)
		{
			Save.EncyclopediaSaveData saveData = Save.SaveData;
			if (!true)
			{
			}
			byte globalLabelStatus;
			switch (value)
			{
			case 0:
				globalLabelStatus = 9;
				break;
			case 1:
				globalLabelStatus = 11;
				break;
			case 2:
				globalLabelStatus = 15;
				break;
			default:
				globalLabelStatus = 9;
				break;
			}
			if (!true)
			{
			}
			saveData.GlobalLabelStatus = globalLabelStatus;
		}

		// Token: 0x06006531 RID: 25905 RVA: 0x002E4774 File Offset: 0x002E2974
		private void OnQuickStartGameValueChanged(byte templateId, byte value)
		{
			bool isOn = (int)value == this.SwitchOnIndex;
			this.SettingData.SetQuickStartGame(isOn);
			bool flag = isOn;
			if (flag)
			{
				this.SettingData.SetHasSelectedQuickStartGame();
			}
			this.RefreshToggleHelper();
		}

		// Token: 0x06006532 RID: 25906 RVA: 0x002E47B4 File Offset: 0x002E29B4
		private void OnInscriptionCharValueChanged(byte templateId, byte value)
		{
			bool isOn = value == 0;
			this.rootSelectInscriptionChar.gameObject.SetActive(isOn);
			this.RefreshToggleHelper();
		}

		// Token: 0x06006533 RID: 25907 RVA: 0x002E47E0 File Offset: 0x002E29E0
		private void OpenCheckInscriptionForInclude()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			List<InscribedCharacterKey> includedInscribedCharList = this._includedInscribedCharList;
			bool flag = includedInscribedCharList != null && includedInscribedCharList.Count > 0;
			if (flag)
			{
				args.SetObject("PreSelectedKeys", new List<InscribedCharacterKey>(this._includedInscribedCharList));
			}
			Dictionary<InscribedCharacterKey, short> includedInscribedCharAges = this._includedInscribedCharAges;
			bool flag2 = includedInscribedCharAges != null && includedInscribedCharAges.Count > 0;
			if (flag2)
			{
				args.SetObject("PreSelectedAges", new Dictionary<InscribedCharacterKey, short>(this._includedInscribedCharAges));
			}
			args.Set("UnifiedAge", this._savedUnifiedAge);
			args.Set("RandomRange", this._savedRandomRange);
			args.Set("AgeSettingEnabled", this._savedAgeSettingEnabled);
			UIElement.SelectInscriptionForEvolution.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.SelectInscriptionForEvolution, true);
		}

		// Token: 0x06006534 RID: 25908 RVA: 0x002E48B0 File Offset: 0x002E2AB0
		private void OnConfirmIncludedInscriptionChar(ArgumentBox argumentBox)
		{
			argumentBox.Get<List<InscribedCharacterKey>>("IncludedInscribedCharList", out this._includedInscribedCharList);
			argumentBox.Get<List<InscribedCharacterKey>>("CanIncludeInscribedCharList", out this._canIncludeInscribedCharList);
			argumentBox.Get("AutoIncludeInscribedChar", out this._autoIncludeInscribedChar);
			argumentBox.Get<Dictionary<InscribedCharacterKey, short>>("IncludedInscribedCharAges", out this._includedInscribedCharAges);
			argumentBox.Get("UnifiedAge", out this._savedUnifiedAge);
			argumentBox.Get("RandomRange", out this._savedRandomRange);
			argumentBox.Get("AgeSettingEnabled", out this._savedAgeSettingEnabled);
			this.RefreshInscriptionCharCount();
		}

		// Token: 0x06006535 RID: 25909 RVA: 0x002E4944 File Offset: 0x002E2B44
		private void RefreshInscriptionCharCount()
		{
			List<InscribedCharacterKey> includedInscribedCharList = this._includedInscribedCharList;
			int curCount = (includedInscribedCharList != null) ? includedInscribedCharList.Count : 0;
			Dictionary<InscribedCharacterKey, InscribedCharacter> inscribedCharacters = GlobalOperations.InscribedCharacters;
			int maxCount = (inscribedCharacters != null) ? inscribedCharacters.Count : 0;
			this.textInscriptionCharCount.text = string.Format("{0}/{1}", curCount, maxCount);
			this.RefreshToggleHelper();
		}

		// Token: 0x06006536 RID: 25910 RVA: 0x002E49A0 File Offset: 0x002E2BA0
		private void LoadInscribedCharKeys(string keysStr)
		{
			List<InscribedCharacterKey> loadedKeys = new List<InscribedCharacterKey>();
			string[] parts = keysStr.Split(',', StringSplitOptions.None);
			foreach (string part in parts)
			{
				bool flag = string.IsNullOrEmpty(part);
				if (!flag)
				{
					string[] subParts = part.Split('_', StringSplitOptions.None);
					uint worldId;
					int charId;
					bool flag2 = subParts.Length == 2 && uint.TryParse(subParts[0], out worldId) && int.TryParse(subParts[1], out charId);
					if (flag2)
					{
						InscribedCharacterKey key = new InscribedCharacterKey(worldId, charId);
						bool flag3 = GlobalOperations.InscribedCharacters.ContainsKey(key);
						if (flag3)
						{
							loadedKeys.Add(key);
						}
					}
				}
			}
			bool flag4 = loadedKeys.Count > 0;
			if (flag4)
			{
				this._includedInscribedCharList = loadedKeys;
				this._canIncludeInscribedCharList = new List<InscribedCharacterKey>();
				foreach (KeyValuePair<InscribedCharacterKey, InscribedCharacter> kv in GlobalOperations.InscribedCharacters)
				{
					bool flag5 = !loadedKeys.Contains(kv.Key);
					if (flag5)
					{
						this._canIncludeInscribedCharList.Add(kv.Key);
					}
				}
			}
		}

		// Token: 0x06006537 RID: 25911 RVA: 0x002E4AD8 File Offset: 0x002E2CD8
		private void OnInscriptionChange(ArgumentBox _ = null)
		{
			List<InscribedCharacterKey> list = this._includedInscribedCharList;
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				this._includedInscribedCharList.RemoveAll((InscribedCharacterKey key) => !GlobalOperations.InscribedCharacters.ContainsKey(key));
			}
			list = this._canIncludeInscribedCharList;
			bool flag2 = list != null && list.Count > 0;
			if (flag2)
			{
				this._canIncludeInscribedCharList.RemoveAll((InscribedCharacterKey key) => !GlobalOperations.InscribedCharacters.ContainsKey(key));
			}
			this.RefreshInscriptionCharCount();
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x002E4B7C File Offset: 0x002E2D7C
		private void RefreshItemInfo()
		{
			this.canvasGroupHover.DOKill(false);
			bool isShow = this._selectedItem != null || this._hoveredItem != null;
			bool flag = isShow;
			if (flag)
			{
				this.canvasGroupHover.DOFade(0f, 0.1f).OnComplete(delegate
				{
					NewGameSubPageWorldDetailItem levelItem = this._selectedItem ?? this._hoveredItem;
					this.imageItem.SetTexture(levelItem.Texture);
					this.textItemTitle.text = levelItem.Title;
					this.textItemDesc.text = levelItem.Desc;
					this.canvasGroupHover.DOFade(1f, 0.1f);
				});
			}
			else
			{
				this.canvasGroupHover.DOFade(0f, 0.1f);
			}
		}

		// Token: 0x06006539 RID: 25913 RVA: 0x002E4BF7 File Offset: 0x002E2DF7
		private void OnEnterItem(NewGameSubPageWorldDetailItem item)
		{
			this._hoveredItem = item;
			this.RefreshItemInfo();
		}

		// Token: 0x0600653A RID: 25914 RVA: 0x002E4C08 File Offset: 0x002E2E08
		private void OnExitItem(NewGameSubPageWorldDetailItem item)
		{
		}

		// Token: 0x04004661 RID: 18017
		[SerializeField]
		private NewGameSubPageOtherToggleHelper toggleHelper;

		// Token: 0x04004662 RID: 18018
		[Header("非配置设置")]
		[SerializeField]
		private NewGameSubPageWorldDetailItem tutorial;

		// Token: 0x04004663 RID: 18019
		[SerializeField]
		private NewGameSubPageWorldDetailItem encyclopedia;

		// Token: 0x04004664 RID: 18020
		[SerializeField]
		private GameObject rootProgress;

		// Token: 0x04004665 RID: 18021
		[SerializeField]
		private NewGameSubPageWorldDetailItem quickStartGame;

		// Token: 0x04004666 RID: 18022
		[SerializeField]
		private NewGameSubPageWorldDetailItem inscriptionChar;

		// Token: 0x04004667 RID: 18023
		[SerializeField]
		private CButton buttonSelectInscriptionChar;

		// Token: 0x04004668 RID: 18024
		[SerializeField]
		private GameObject rootSelectInscriptionChar;

		// Token: 0x04004669 RID: 18025
		[SerializeField]
		private TextMeshProUGUI textInscriptionCharCount;

		// Token: 0x0400466A RID: 18026
		[Header("配置设置")]
		[SerializeField]
		private List<NewGameSubPageWorldDetailItem> detailItemList;

		// Token: 0x0400466B RID: 18027
		[SerializeField]
		private NewGameSubPageWorldDetailItem restrictOptionsBehavior;

		// Token: 0x0400466C RID: 18028
		[SerializeField]
		private NewGameSubPageWorldDetailItem allowRandomTaiwuHeir;

		// Token: 0x0400466D RID: 18029
		[SerializeField]
		private NewGameSubPageWorldDetailItem worldPopulation;

		// Token: 0x0400466E RID: 18030
		[SerializeField]
		private NewGameSubPageWorldDetailItem characterLifeSpan;

		// Token: 0x0400466F RID: 18031
		[Header("设置项的信息")]
		[SerializeField]
		private GameObject rootItemInfo;

		// Token: 0x04004670 RID: 18032
		[SerializeField]
		private CanvasGroup canvasGroupHover;

		// Token: 0x04004671 RID: 18033
		[SerializeField]
		private CRawImage imageItem;

		// Token: 0x04004672 RID: 18034
		[SerializeField]
		private TextMeshProUGUI textItemTitle;

		// Token: 0x04004673 RID: 18035
		[SerializeField]
		private TextMeshProUGUI textItemDesc;

		// Token: 0x04004674 RID: 18036
		private List<InscribedCharacterKey> _includedInscribedCharList;

		// Token: 0x04004675 RID: 18037
		private List<InscribedCharacterKey> _canIncludeInscribedCharList;

		// Token: 0x04004676 RID: 18038
		private Dictionary<InscribedCharacterKey, short> _includedInscribedCharAges;

		// Token: 0x04004677 RID: 18039
		private bool _autoIncludeInscribedChar;

		// Token: 0x04004678 RID: 18040
		private short _savedUnifiedAge = 20;

		// Token: 0x04004679 RID: 18041
		private short _savedRandomRange = 0;

		// Token: 0x0400467A RID: 18042
		private bool _savedAgeSettingEnabled;

		// Token: 0x0400467B RID: 18043
		private const string InscriptionForEvolutionEnabledKey = "InscriptionForEvolutionEnabled";

		// Token: 0x0400467C RID: 18044
		private const string InscribedCharKeysEvolutionKey = "InscribedCharKeys_Evolution";

		// Token: 0x0400467D RID: 18045
		private const string InscriptionAgeUnifiedKey = "InscriptionAgeUnified";

		// Token: 0x0400467E RID: 18046
		private const string InscriptionAgeRangeKey = "InscriptionAgeRange";

		// Token: 0x0400467F RID: 18047
		private const string InscriptionAgeEnabledKey = "InscriptionAgeEnabled";

		// Token: 0x04004680 RID: 18048
		private NewGameSubPageWorldDetailItem _hoveredItem;

		// Token: 0x04004681 RID: 18049
		private NewGameSubPageWorldDetailItem _selectedItem;
	}
}
