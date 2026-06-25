using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Creation;
using GameData.Domains.World;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x0200080F RID: 2063
	public class NewGameSubPageWorldDetail : NewGameSubPage
	{
		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x06006540 RID: 25920 RVA: 0x002E4D49 File Offset: 0x002E2F49
		// (set) Token: 0x06006541 RID: 25921 RVA: 0x002E4D54 File Offset: 0x002E2F54
		private int CurSettingLevelIndex
		{
			get
			{
				return this._curSettingLevelIndex;
			}
			set
			{
				bool flag = this._curSettingLevelIndex == value;
				if (flag)
				{
					this.curSettingLevelItem.Init(this.GetUnlockedLevel(this._curSettingLevelIndex), false, false, null, false);
					this.RefreshToggleHelper();
					this.RefreshLevelButton();
					this.RefreshTextPoint();
				}
				else
				{
					bool isLastCustom = this._curSettingLevelIndex == WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
					this._curSettingLevelIndex = value;
					int curSettingLevel = this.GetUnlockedLevel(this._curSettingLevelIndex);
					bool curNotCustomChanged = false;
					bool flag2 = curSettingLevel != WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
					if (flag2)
					{
						curNotCustomChanged = (this._curNotCustomSettingLevelIndex != this._curSettingLevelIndex);
						this._curNotCustomSettingLevelIndex = this._curSettingLevelIndex;
					}
					this.curSettingLevelItem.Init(curSettingLevel, false, false, null, this._isCurSettingLevelItemAnim);
					this._isCurSettingLevelItemAnim = true;
					this.canvasGroupDetailGroup.DOKill(false);
					bool flag3 = curNotCustomChanged || isLastCustom;
					if (flag3)
					{
						this.canvasGroupDetailGroup.DOFade(0f, 0.1f).OnComplete(delegate
						{
							int curNotCustomSettingLevel2 = this.GetUnlockedLevel(this._curNotCustomSettingLevelIndex);
							this.RefreshDifficultyGroups(curNotCustomSettingLevel2);
							this.RefreshToggleHelper();
							this.canvasGroupDetailGroup.DOFade(1f, 0.1f);
						});
					}
					else
					{
						bool flag4 = this._curNotCustomSettingLevelIndex == -1;
						if (flag4)
						{
							int curNotCustomSettingLevel = this.GetUnlockedLevel(this._curNotCustomSettingLevelIndex);
							this.RefreshDifficultyGroups(curNotCustomSettingLevel);
						}
						this.RefreshToggleHelper();
						this.canvasGroupDetailGroup.alpha = 1f;
					}
					this.RefreshLevelButton();
					this.RefreshTextPoint();
				}
			}
		}

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x06006542 RID: 25922 RVA: 0x002E4EB5 File Offset: 0x002E30B5
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x06006543 RID: 25923 RVA: 0x002E4EB8 File Offset: 0x002E30B8
		// (set) Token: 0x06006544 RID: 25924 RVA: 0x002E4EBB File Offset: 0x002E30BB
		public override bool StartGameChecked
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		// Token: 0x06006545 RID: 25925 RVA: 0x002E4EC0 File Offset: 0x002E30C0
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			foreach (NewGameSubPageWorldDetailGroup group in this.detailGroupList)
			{
				group.SaveToWorldCreationInfo(ref worldCreationInfo);
			}
			foreach (WorldCreationItem creationCfg in ((IEnumerable<WorldCreationItem>)WorldCreation.Instance))
			{
				int value = worldCreationInfo.Get(creationCfg.TemplateId);
				base.CreationInfoMap[creationCfg.SaveFileKey] = value.ToString();
			}
			int overallDifficulty = this.toggleGroupDifficultyLevel.GetActiveIndex();
			base.CreationInfoMap["OverallDifficulty"] = overallDifficulty.ToString();
			this._curDifficulty = -1;
			worldCreationInfo.TaiwuVillageStateTemplateId = sbyte.Parse(base.CreationInfoMap["TaiwuVillageStateTemplateId"]);
			worldCreationInfo.TaiwuVillageLandFormType = sbyte.Parse(base.CreationInfoMap["TaiwuVillageLandFormType"]);
			bool flag = ViewNewGame.ChallengeModeInfo != null;
			if (flag)
			{
				protagonistCreationInfo.ChallengeModeInfo = ViewNewGame.ChallengeModeInfo;
			}
		}

		// Token: 0x06006546 RID: 25926 RVA: 0x002E4FF4 File Offset: 0x002E31F4
		protected override void Awake()
		{
			base.Awake();
			this.toggleGroupDifficultyLevel.Init(-1);
			this.toggleGroupDifficultyLevel.OnActiveIndexChange += this.ToggleGroupDifficultyLevelOnOnActiveIndexChange;
			List<CToggle> all = this.toggleGroupDifficultyLevel.GetAll();
			for (int index = 0; index < all.Count; index++)
			{
				CToggle toggle = all[index];
				TextMeshProUGUI textTitle = toggle.GetComponentInChildren<TextMeshProUGUI>();
				textTitle.text = NewGameSubPageWorldDetail.DifficultyLevelKeys[index].Tr().ColorReplace();
				toggle.interactable = (index < WorldCreationInfo.EDifficultyLevel.Custom.ToInt());
			}
			this.buttonLastLevel.ClearAndAddListener(new Action(this.OnClickButtonLastLevel));
			this.buttonNextLevel.ClearAndAddListener(new Action(this.OnClickButtonNextLevel));
			this.buttonChallenge.ClearAndAddListener(new Action(this.OnClickButtonChallenge));
			this.returnBtn.ClearAndAddListener(new Action(this.OnClickReturnButton));
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x002E50EF File Offset: 0x002E32EF
		public override void Init()
		{
			base.Init();
			this.InitDifficultyGroups();
			this.Refresh();
		}

		// Token: 0x06006548 RID: 25928 RVA: 0x002E5107 File Offset: 0x002E3307
		private void OnEnable()
		{
			this.Refresh();
		}

		// Token: 0x06006549 RID: 25929 RVA: 0x002E5114 File Offset: 0x002E3314
		private void Refresh()
		{
			this.rootSelect.SetActive(true);
			this.rootSetting.SetActive(false);
			this.rootItemInfo.SetActive(true);
			this.canvasGroupHover.alpha = 0f;
			this._hoveredItem = null;
			this._selectedItem = null;
			this.RefreshUnlockedDifficultyLevels();
			bool flag = this._curDifficulty < 0;
			if (flag)
			{
				this._curDifficulty = this.parent.GetInitDifficulty();
				this._curNotCustomSettingLevelIndex = (this._curSettingLevelIndex = -1);
				this.CurSettingLevelIndex = this._unlockedLevelList.IndexOf((int)this._curDifficulty);
				this.toggleGroupDifficultyLevel.SetWithoutNotify(this.CurSettingLevelIndex);
			}
			this.RefreshChallengeInfo();
		}

		// Token: 0x0600654A RID: 25930 RVA: 0x002E51D0 File Offset: 0x002E33D0
		private void RefreshChallengeInfo()
		{
			this.challengeOff.SetActive(ViewNewGame.ChallengeModeIds.Count <= 0);
			this.SetChallengeBg(NewGameSubPageWorldDetail.GetEnabledOptionalChallengeCount());
		}

		// Token: 0x0600654B RID: 25931 RVA: 0x002E51FC File Offset: 0x002E33FC
		private static int GetEnabledOptionalChallengeCount()
		{
			return ViewNewGame.ChallengeModeIds.Count((int id) => ChallengeMode.Instance[id].Type == EChallengeModeType.Optional);
		}

		// Token: 0x0600654C RID: 25932 RVA: 0x002E5238 File Offset: 0x002E3438
		private void SetChallengeBg(int optionalEnabledCount)
		{
			bool flag = this.challengeImageBg == null;
			if (!flag)
			{
				int index = ViewChallenge.GetInfernoBgSpriteIndex(optionalEnabledCount);
				ResLoader.Load<Sprite>(string.Format("{0}{1}", "RemakeResources/UIGraphics5.0/Ui9NewGame/ui9_btn_challenge_labelpnl_", index), delegate(Sprite sprite)
				{
					this.challengeImageBg.sprite = sprite;
				}, null, false);
			}
		}

		// Token: 0x0600654D RID: 25933 RVA: 0x002E5288 File Offset: 0x002E3488
		private void RefreshUnlockedDifficultyLevels()
		{
			this._unlockedLevelList.Clear();
			for (int index = 0; index < this.difficultyLevelItemList.Count; index++)
			{
				NewGameSubPageWorldDetailDifficultyLevelItem2 levelItem = this.difficultyLevelItemList[index];
				bool isLocked = NewGameSubPageWorldDetail.IsDifficultyLocked(index);
				levelItem.Init(index, isLocked, true, new Action(this.OnClickButtonSetting), false);
				bool flag = !isLocked;
				if (flag)
				{
					this._unlockedLevelList.Add(index);
				}
			}
		}

		// Token: 0x0600654E RID: 25934 RVA: 0x002E5300 File Offset: 0x002E3500
		private int GetUnlockedLevel(int index)
		{
			return this._unlockedLevelList.GetOrDefault(index, (int)ViewNewGame.DefaultDifficulty);
		}

		// Token: 0x0600654F RID: 25935 RVA: 0x002E5314 File Offset: 0x002E3514
		private void OnClickButtonSetting()
		{
			this.rootSelect.SetActive(false);
			this.rootSetting.SetActive(true);
			this._isCurSettingLevelItemAnim = (this.CurSettingLevelIndex == this._unlockedLevelList.Count - 1);
			this.CurSettingLevelIndex = this._unlockedLevelList.Count - 1;
			this.toggleGroupDifficultyLevel.SetWithoutNotify(WorldCreationInfo.EDifficultyLevel.Custom.ToInt());
		}

		// Token: 0x06006550 RID: 25936 RVA: 0x002E5384 File Offset: 0x002E3584
		private void ToggleGroupDifficultyLevelOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool isLocked = NewGameSubPageWorldDetail.IsDifficultyLocked(newIndex);
			bool flag = isLocked;
			if (flag)
			{
				string title = LanguageKey.LK_NewGame_WorldDetail_DifficultyLevel_ForceOpen_Title.Tr();
				string content = LanguageKey.LK_NewGame_WorldDetail_DifficultyLevel_ForceOpen_Content.Tr();
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					SingletonObject.getInstance<GlobalSettings>().ForceUnlockWorldDetailDifficultyLevel4 = true;
					SingletonObject.getInstance<GlobalSettings>().SaveSettings();
					this.CurSettingLevelIndex = newIndex;
					this.RefreshUnlockedDifficultyLevels();
				}, delegate
				{
					this.toggleGroupDifficultyLevel.SetWithoutNotify(oldIndex);
				}, EDialogType.None);
			}
			else
			{
				this.CurSettingLevelIndex = newIndex;
			}
		}

		// Token: 0x06006551 RID: 25937 RVA: 0x002E5408 File Offset: 0x002E3608
		private void RefreshTextPoint()
		{
			bool showTextPoint = this.CurSettingLevelIndex < WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
			this.rootTextPoint.SetActive(showTextPoint);
			bool flag = showTextPoint;
			if (flag)
			{
				LanguageKey key = LanguageKey.LK_NewGame_DifficultyLevel_Point_1 + this.CurSettingLevelIndex;
				this.textPoint.text = key.Tr();
			}
			else
			{
				this.textPoint.text = string.Empty;
			}
		}

		// Token: 0x06006552 RID: 25938 RVA: 0x002E5474 File Offset: 0x002E3674
		private void RefreshToggleHelper()
		{
			List<string> titleList = (from g in this.detailGroupList
			select g.GetTitle()).ToList<string>();
			this.toggleHelper.Refresh(titleList);
		}

		// Token: 0x06006553 RID: 25939 RVA: 0x002E54C0 File Offset: 0x002E36C0
		public static bool IsDifficultyLocked(int difficulty)
		{
			bool flag = difficulty == WorldCreationInfo.EDifficultyLevel.Level4.ToInt();
			bool result;
			if (flag)
			{
				bool forceUnlockWorldDetailDifficultyLevel = SingletonObject.getInstance<GlobalSettings>().ForceUnlockWorldDetailDifficultyLevel4;
				result = (!forceUnlockWorldDetailDifficultyLevel && !ViewNewGame.PastEnding);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06006554 RID: 25940 RVA: 0x002E5503 File Offset: 0x002E3703
		private void OnClickReturnButton()
		{
			this.rootSelect.SetActive(true);
			this.rootSetting.SetActive(false);
		}

		// Token: 0x06006555 RID: 25941 RVA: 0x002E5520 File Offset: 0x002E3720
		private void OnClickButtonLastLevel()
		{
			int curSettingLevelIndex = this.CurSettingLevelIndex;
			this.CurSettingLevelIndex = curSettingLevelIndex - 1;
		}

		// Token: 0x06006556 RID: 25942 RVA: 0x002E5540 File Offset: 0x002E3740
		private void OnClickButtonNextLevel()
		{
			int curSettingLevelIndex = this.CurSettingLevelIndex;
			this.CurSettingLevelIndex = curSettingLevelIndex + 1;
		}

		// Token: 0x06006557 RID: 25943 RVA: 0x002E555F File Offset: 0x002E375F
		private void RefreshLevelButton()
		{
			this.buttonLastLevel.interactable = (this.CurSettingLevelIndex > 0);
			this.buttonNextLevel.interactable = (this.CurSettingLevelIndex < this._unlockedLevelList.Count - 1);
		}

		// Token: 0x06006558 RID: 25944 RVA: 0x002E5598 File Offset: 0x002E3798
		private void InitDifficultyGroups()
		{
			sbyte i = 0;
			while ((int)i < this.detailGroupList.Count)
			{
				NewGameSubPageWorldDetailGroup group = this.detailGroupList[(int)i];
				group.Init(i, new Action<byte, byte>(this.OnWorldSettingChangedSettingView), new Action<NewGameSubPageWorldDetailItem>(this.OnEnterItem), new Action<NewGameSubPageWorldDetailItem>(this.OnExitItem));
				i += 1;
			}
		}

		// Token: 0x06006559 RID: 25945 RVA: 0x002E55FC File Offset: 0x002E37FC
		private void RefreshDifficultyGroups(int difficulty)
		{
			this._curDifficulty = this.GetClampedDifficulty(difficulty);
			this.parent.InitWorldCreationInfo(this._curDifficulty);
			bool flag = difficulty == WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
			if (flag)
			{
				foreach (WorldCreationGroupItem groupCfg in ((IEnumerable<WorldCreationGroupItem>)WorldCreationGroup.Instance))
				{
					NewGameSubPageWorldDetailGroup groupView = this.detailGroupList.GetOrDefault((int)groupCfg.TemplateId);
					bool flag2 = groupView == null;
					if (!flag2)
					{
						groupView.LoadFromWorldCreationInfo(ViewNewGame.TempWorldCreationInfo);
						groupView.RefreshInteractable(EInteractType.NewGameCustom);
					}
				}
			}
			else
			{
				foreach (NewGameSubPageWorldDetailGroup group in this.detailGroupList)
				{
					group.RefreshInteractable(EInteractType.NewGameCustom);
					group.LoadByDifficultyPreset(difficulty);
				}
			}
		}

		// Token: 0x0600655A RID: 25946 RVA: 0x002E5704 File Offset: 0x002E3904
		private sbyte GetClampedDifficulty(int difficulty)
		{
			int maxDifficulty = NewGameSubPageWorldDetail.IsDifficultyLocked(WorldCreationInfo.EDifficultyLevel.Level4.ToInt()) ? WorldCreationInfo.EDifficultyLevel.Level3.ToInt() : WorldCreationInfo.EDifficultyLevel.Level4.ToInt();
			return (sbyte)Mathf.Clamp(difficulty, WorldCreationInfo.EDifficultyLevel.Level1.ToInt(), maxDifficulty);
		}

		// Token: 0x0600655B RID: 25947 RVA: 0x002E5756 File Offset: 0x002E3956
		private void OnWorldSettingChangedSettingView(byte templateId, byte value)
		{
			this.CurSettingLevelIndex = this._unlockedLevelList.Count - 1;
			this.RefreshToggleHelper();
			this.RefreshItemInfo();
		}

		// Token: 0x0600655C RID: 25948 RVA: 0x002E577C File Offset: 0x002E397C
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
					this.textItemTitle.text = levelItem.ConfigItem.Name;
					this.textItemDesc.text = levelItem.ConfigItem.Desc.ColorReplace();
					this.canvasGroupHover.DOFade(1f, 0.1f);
				});
			}
			else
			{
				this.canvasGroupHover.DOFade(0f, 0.1f);
			}
		}

		// Token: 0x0600655D RID: 25949 RVA: 0x002E57F7 File Offset: 0x002E39F7
		private void OnEnterItem(NewGameSubPageWorldDetailItem item)
		{
			this._hoveredItem = item;
			this.RefreshItemInfo();
		}

		// Token: 0x0600655E RID: 25950 RVA: 0x002E5808 File Offset: 0x002E3A08
		private void OnExitItem(NewGameSubPageWorldDetailItem item)
		{
		}

		// Token: 0x0600655F RID: 25951 RVA: 0x002E580B File Offset: 0x002E3A0B
		private void OnClickButtonChallenge()
		{
			UIManager.Instance.ShowUI(UIElement.Challenge, true);
			UIElement.Challenge.OnDeActive = new Action(this.RefreshChallengeInfo);
		}

		// Token: 0x06006561 RID: 25953 RVA: 0x002E5850 File Offset: 0x002E3A50
		// Note: this type is marked as 'beforefieldinit'.
		static NewGameSubPageWorldDetail()
		{
			LanguageKey[] array = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.30D5AF875CA6768C922721EDBB9638D994BAD6FA3E65F348AF256CBE97EBF896).FieldHandle);
			NewGameSubPageWorldDetail.DifficultyLevelKeys = array;
		}

		// Token: 0x04004686 RID: 18054
		[SerializeField]
		private NewGameSubPageWorldDetailToggleHelper toggleHelper;

		// Token: 0x04004687 RID: 18055
		[Header("选择难度")]
		[SerializeField]
		private GameObject rootSelect;

		// Token: 0x04004688 RID: 18056
		[SerializeField]
		private CToggleGroup toggleGroupDifficultyLevel;

		// Token: 0x04004689 RID: 18057
		[SerializeField]
		private List<NewGameSubPageWorldDetailDifficultyLevelItem2> difficultyLevelItemList;

		// Token: 0x0400468A RID: 18058
		[SerializeField]
		private GameObject rootTextPoint;

		// Token: 0x0400468B RID: 18059
		[SerializeField]
		private TextMeshProUGUI textPoint;

		// Token: 0x0400468C RID: 18060
		[Header("详细设置")]
		[SerializeField]
		private GameObject rootSetting;

		// Token: 0x0400468D RID: 18061
		[SerializeField]
		private CButton returnBtn;

		// Token: 0x0400468E RID: 18062
		[SerializeField]
		private NewGameSubPageWorldDetailDifficultyLevelItem2 curSettingLevelItem;

		// Token: 0x0400468F RID: 18063
		[SerializeField]
		private CButton buttonNextLevel;

		// Token: 0x04004690 RID: 18064
		[SerializeField]
		private CButton buttonLastLevel;

		// Token: 0x04004691 RID: 18065
		[SerializeField]
		private CanvasGroup canvasGroupDetailGroup;

		// Token: 0x04004692 RID: 18066
		[SerializeField]
		private List<NewGameSubPageWorldDetailGroup> detailGroupList;

		// Token: 0x04004693 RID: 18067
		[Header("设置项的信息")]
		[SerializeField]
		private GameObject rootItemInfo;

		// Token: 0x04004694 RID: 18068
		[SerializeField]
		private CanvasGroup canvasGroupHover;

		// Token: 0x04004695 RID: 18069
		[SerializeField]
		private CRawImage imageItem;

		// Token: 0x04004696 RID: 18070
		[SerializeField]
		private TextMeshProUGUI textItemTitle;

		// Token: 0x04004697 RID: 18071
		[SerializeField]
		private TextMeshProUGUI textItemDesc;

		// Token: 0x04004698 RID: 18072
		[Header("玄狱模式")]
		[SerializeField]
		private CButton buttonChallenge;

		// Token: 0x04004699 RID: 18073
		[SerializeField]
		private GameObject challengeOff;

		// Token: 0x0400469A RID: 18074
		[SerializeField]
		private CImage challengeImageBg;

		// Token: 0x0400469B RID: 18075
		public static readonly LanguageKey[] DifficultyLevelKeys;

		// Token: 0x0400469C RID: 18076
		private int _curNotCustomSettingLevelIndex;

		// Token: 0x0400469D RID: 18077
		private int _curSettingLevelIndex;

		// Token: 0x0400469E RID: 18078
		private bool _isCurSettingLevelItemAnim;

		// Token: 0x0400469F RID: 18079
		private readonly List<int> _unlockedLevelList = new List<int>();

		// Token: 0x040046A0 RID: 18080
		private sbyte _curDifficulty = -1;

		// Token: 0x040046A1 RID: 18081
		private NewGameSubPageWorldDetailItem _hoveredItem;

		// Token: 0x040046A2 RID: 18082
		private NewGameSubPageWorldDetailItem _selectedItem;

		// Token: 0x040046A3 RID: 18083
		private const string ChallengeBgSpritePath = "RemakeResources/UIGraphics5.0/Ui9NewGame/ui9_btn_challenge_labelpnl_";
	}
}
