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
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001DD RID: 477
public class UI_LifeSkillCombatBegin : UIBase
{
	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001F41 RID: 8001 RVA: 0x000E38A7 File Offset: 0x000E1AA7
	public LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x000E38B0 File Offset: 0x000E1AB0
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = !this._awaken;
		if (flag)
		{
			this.Awake();
		}
		this.NeedDataListenerId = true;
		argsBox.Get("EnemyId", out this._enemyId);
		argsBox.Get("LifeSkillType", out this._lifeSkillType);
		this._enemyTeam.Clear();
		this._enemyTeam.Add(this._enemyId);
		this._settingData = SingletonObject.getInstance<GlobalSettings>();
		this._selfTeam.Clear();
		this._selfTeam.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		RectTransform cloudLeft = base.CGet<RectTransform>("CloudLeft");
		RectTransform cloudRight = base.CGet<RectTransform>("CloudRight");
		cloudLeft.anchoredPosition = cloudLeft.anchoredPosition.SetX(-210f);
		cloudRight.anchoredPosition = cloudRight.anchoredPosition.SetX(230f);
		base.CGet<SkeletonGraphic>("Ani1").gameObject.SetActive(false);
		base.CGet<SkeletonGraphic>("Ani2").gameObject.SetActive(false);
		base.CGet<CanvasGroup>("Back").alpha = 0f;
		base.CGet<GameObject>("PointerMask").SetActive(true);
		base.CGet<GameObject>("ClickToStartTips").SetActive(false);
		this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(this._selfTeam[0], false);
		this._toggleGroup = base.CGet<CToggleGroupObsolete>("ToggleGroup");
		this._toggleGroup.InitPreOnToggle(-1);
		this._toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		TaiwuDomainMethod.AsyncCall.DebateGameGetTaiwuSelectedCardTypes(this, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._selectedTypeList);
		});
		Refers firstMove = base.CGet<Refers>("FirstMove");
		EPrepareCombatResult result = this.Model.DebateGame.IsTaiwuFirst ? EPrepareCombatResult.SelfFirst : EPrepareCombatResult.EnemyFirst;
		UI_LifeSkillCombatBegin.RefreshFirstMove(firstMove, result, true);
		CombatBeginFirstMove allyFirstMove = firstMove.CGet<CombatBeginFirstMove>("Ally");
		CombatBeginFirstMove enemyFirstMove = firstMove.CGet<CombatBeginFirstMove>("Enemy");
		TooltipInvoker allyTip = allyFirstMove.GetComponent<TooltipInvoker>();
		allyTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		allyTip.Type = (this.Model.DebateGame.IsTaiwuFirst ? TipType.LifeSkillCombatFirstMove : TipType.LifeSkillCombatLastMove);
		TooltipInvoker enemyTip = enemyFirstMove.GetComponent<TooltipInvoker>();
		enemyTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		enemyTip.Type = ((!this.Model.DebateGame.IsTaiwuFirst) ? TipType.LifeSkillCombatFirstMove : TipType.LifeSkillCombatLastMove);
		this._prepareTog.isOn = this._settingData.CombatPrepare;
		this._prepareTog.gameObject.SetActive(true);
		this._pointerInPauseUi = false;
		this._prepareFinished = false;
		this._gettingSelfWisdom = true;
		YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
		bool flag2 = this._animRoutine != null;
		if (flag2)
		{
			yieldHelper.StopYield(this._animRoutine);
		}
		this._animRoutine = yieldHelper.StartYield(this.<OnInit>g__AnimRoutine|27_2());
		this.NeedDataListenerId = true;
		CommandKitBase.SetDisable(true);
		this.Element.OnListenerIdReady = delegate()
		{
			for (int i = 0; i < 3; i++)
			{
				Refers selfTeammate = this._selfTeammateHolder.GetChild(i).GetComponent<Refers>();
				Refers enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<Refers>();
				selfTeammate.CGet<CanvasGroup>("BetrayTextBack").gameObject.SetActive(false);
				enemyTeammate.CGet<SkeletonGraphic>("BetrayedTips0").gameObject.SetActive(false);
				enemyTeammate.CGet<SkeletonGraphic>("BetrayedTips1").gameObject.SetActive(false);
				enemyTeammate.CGet<CanvasGroup>("BetrayTextBack").gameObject.SetActive(false);
			}
			bool flag3 = !SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(this._enemyId);
			if (flag3)
			{
				for (int index = 0; index < 3; index++)
				{
					base.AppendMonitorFieldId(new UIBase.MonitorDataField(5, 32, (ulong)((long)index), null));
				}
			}
			else
			{
				this.RefreshCharactersDisplay();
				this._gettingSelfWisdom = true;
				CharacterDomainMethod.Call.GetCharacterWisdomCountById(this.Element.GameDataListenerId, this._selfTeam[0]);
				CharacterDomainMethod.Call.GetCharacterWisdomCountById(this.Element.GameDataListenerId, this._enemyTeam[0]);
			}
			this.InitSpectators();
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
			{
				CommandKitBase.SetDisable(false);
			}));
		};
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000E3BB8 File Offset: 0x000E1DB8
	internal static void RefreshFirstMove(Refers firstMove, EPrepareCombatResult prepareCombatResult, bool noTip = false)
	{
		CombatBeginFirstMove allyFirstMove = firstMove.CGet<CombatBeginFirstMove>("Ally");
		CombatBeginFirstMove enemyFirstMove = firstMove.CGet<CombatBeginFirstMove>("Enemy");
		allyFirstMove.Set(prepareCombatResult, noTip);
		enemyFirstMove.Set(prepareCombatResult, noTip);
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000E3BF0 File Offset: 0x000E1DF0
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._selfTeam[0]), new uint[]
		{
			97U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._enemyTeam[0]), new uint[]
		{
			97U
		}));
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000E3C54 File Offset: 0x000E1E54
	private void Awake()
	{
		bool awaken = this._awaken;
		if (!awaken)
		{
			this._selfCharInfo = base.CGet<Refers>("SelfInfo");
			this._enemyCharInfo = base.CGet<Refers>("EnemyInfo");
			this._selfTeammateHolder = this._selfCharInfo.CGet<RectTransform>("TeammateHolder");
			this._enemyTeammateHolder = this._enemyCharInfo.CGet<RectTransform>("TeammateHolder");
			this._noPrepareBarLeft = base.CGet<CImage>("ProgressBarLeft");
			this._noPrepareBarRight = base.CGet<CImage>("ProgressBarRight");
			CButtonObsolete selfCharBtn = this._selfCharInfo.CGet<CButtonObsolete>("OpenCharMenu");
			selfCharBtn.ClearAndAddListener(delegate
			{
				this.ShowCharMenu(true);
			});
			this.InitPointerTrigger(selfCharBtn.GetComponent<PointerTrigger>(), this._selfCharInfo.CGet<Game.Components.Avatar.Avatar>("Avatar"));
			CButtonObsolete enemyCharBtn = this._enemyCharInfo.CGet<CButtonObsolete>("OpenCharMenu");
			enemyCharBtn.ClearAndAddListener(delegate
			{
				this.ShowCharMenu(false);
			});
			this.InitPointerTrigger(enemyCharBtn.GetComponent<PointerTrigger>(), this._enemyCharInfo.CGet<Game.Components.Avatar.Avatar>("Avatar"));
			this._prepareTog = base.CGet<CToggleObsolete>("PrepareTog");
			this._prepareTog.onValueChanged.AddListener(new UnityAction<bool>(this.OnPrepareTogChange));
			this.InitPointerTrigger(this._prepareTog.GetComponent<PointerTrigger>(), null);
			this._awaken = true;
		}
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000E3DAC File Offset: 0x000E1FAC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 4 && notification.MethodId == 109;
					if (flag)
					{
						int wisdomCount = 0;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref wisdomCount);
						bool gettingSelfWisdom = this._gettingSelfWisdom;
						if (gettingSelfWisdom)
						{
							this._selfCharInfo.CGet<CImage>("WisdomIcon").SetSprite((wisdomCount < 0) ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_11", false, null);
							this._selfCharInfo.CGet<TextMeshProUGUI>("WisdomCount").text = string.Format("x{0}", Mathf.Abs(wisdomCount));
							this._gettingSelfWisdom = false;
						}
						else
						{
							this._enemyCharInfo.CGet<CImage>("WisdomIcon").SetSprite((wisdomCount < 0) ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_11", false, null);
							this._enemyCharInfo.CGet<TextMeshProUGUI>("WisdomCount").text = string.Format("x{0}", Mathf.Abs(wisdomCount));
							base.CGet<CanvasGroup>("Back").DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.PlayBeginAni));
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				DataUid dataUid = uid;
				DataUid dataUid2 = dataUid;
				ushort domainId = dataUid2.DomainId;
				if (domainId != 4)
				{
					if (domainId == 5)
					{
						ushort dataId = dataUid2.DataId;
						if (dataId == 32)
						{
							int charId = -1;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref charId);
							bool flag2 = charId >= 0 && !this._enemyTeam.Contains(charId);
							if (flag2)
							{
								this._selfTeam.Add(charId);
							}
							bool flag3 = uid.SubId0 == 2UL;
							if (flag3)
							{
								this.RefreshCharactersDisplay();
								this._gettingSelfWisdom = true;
								CharacterDomainMethod.Call.GetCharacterWisdomCountById(this.Element.GameDataListenerId, this._selfTeam[0]);
								CharacterDomainMethod.Call.GetCharacterWisdomCountById(this.Element.GameDataListenerId, this._enemyTeam[0]);
							}
						}
					}
				}
				else
				{
					ushort dataId = dataUid2.DataId;
					if (dataId == 0)
					{
						uint subId = dataUid2.SubId1;
						if (subId == 97U)
						{
							LifeSkillShorts lifeSkillShorts = default(LifeSkillShorts);
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref lifeSkillShorts);
							bool flag4 = uid.SubId0 == (ulong)((long)this._selfTeam[0]);
							if (flag4)
							{
								this._taiwuLifeSkillAttainments = lifeSkillShorts;
							}
							else
							{
								bool flag5 = uid.SubId0 == (ulong)((long)this._enemyTeam[0]);
								if (flag5)
								{
									this._enemyLifeSkillAttainments = lifeSkillShorts;
								}
							}
							this.RefreshLifeSkillInfo();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000E40E0 File Offset: 0x000E22E0
	private void OnEnable()
	{
		this._selfCharInfo.CGet<GameObject>("OpenCharMenuTips").SetActive(false);
		this._enemyCharInfo.CGet<GameObject>("OpenCharMenuTips").SetActive(false);
		this.StopNoPrepareProgress(true);
		AudioManager audioManager = AudioManager.Instance;
		this._ambienceGlobalVolume = audioManager.GetAmbienceVolume();
		audioManager.SetAmbienceVolume(1f);
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000E4144 File Offset: 0x000E2344
	private void OnDisable()
	{
		YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
		bool flag = this._animRoutine != null;
		if (flag)
		{
			yieldHelper.StopYield(this._animRoutine);
		}
		this._animRoutine = null;
		this.ClearCharactersDisplay();
		AudioManager audioManager = AudioManager.Instance;
		audioManager.SetAmbienceVolume(this._ambienceGlobalVolume);
		audioManager.PlayAmbience("", 1f, 100);
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000E41A8 File Offset: 0x000E23A8
	public void Update()
	{
		bool flag = Input.GetKeyUp(KeyCode.Space) && !UIElement.CharacterMenu.Exist && UIManager.Instance.IsFocusElement(this.Element);
		if (flag)
		{
			this._prepareTog.isOn = !this._prepareTog.isOn;
		}
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000E41FC File Offset: 0x000E23FC
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		string text = btnName;
		string a = text;
		if (a == "StartCombat")
		{
			bool flag = !UIManager.Instance.IsFocusElement(this.Element) || this._prepareFinished;
			if (!flag)
			{
				this.StopNoPrepareProgress(false);
				this._noPrepareBarLeft.fillAmount = 1f;
				this._noPrepareBarRight.fillAmount = 1f;
				this._prepareFinished = true;
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
				{
					this.AutoSelectTaiwuAudience(new Action(this.ShowCombatUi));
				});
			}
		}
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000E4298 File Offset: 0x000E2498
	private void InitPointerTrigger(PointerTrigger ptrTrigger, Game.Components.Avatar.Avatar charAvatar = null)
	{
		ptrTrigger.EnterEvent.AddListener(delegate()
		{
			this._pointerInPauseUi = true;
			this.StopNoPrepareProgress(false);
			bool flag = charAvatar != null;
			if (flag)
			{
				charAvatar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -221f);
			}
		});
		ptrTrigger.ExitEvent.AddListener(delegate()
		{
			this._pointerInPauseUi = false;
			bool flag = !this._settingData.CombatPrepare && ptrTrigger.gameObject.activeInHierarchy && (charAvatar == null || !UIElement.CharacterMenu.Exist);
			if (flag)
			{
				this.StartNoPrepareProgress();
			}
			bool flag2 = charAvatar != null;
			if (flag2)
			{
				charAvatar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -223f);
			}
		});
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x000E42FC File Offset: 0x000E24FC
	private void RefreshCharactersDisplay()
	{
		List<int> charIdList = new List<int>();
		for (int i = 0; i < 4; i++)
		{
			int selfCharId = (this._selfTeam.Count > i) ? this._selfTeam[i] : -1;
			int enemyCharId = (this._enemyTeam.Count > i) ? this._enemyTeam[i] : -1;
			bool flag = selfCharId >= 0;
			if (flag)
			{
				charIdList.Add(selfCharId);
			}
			bool flag2 = enemyCharId >= 0;
			if (flag2)
			{
				charIdList.Add(enemyCharId);
			}
			bool flag3 = i > 0;
			if (flag3)
			{
				int index = i - 1;
				this._selfTeammateHolder.GetChild(index).gameObject.SetActive(selfCharId >= 0);
				this._enemyTeammateHolder.GetChild(index).gameObject.SetActive(enemyCharId >= 0);
			}
		}
		bool flag4 = charIdList.Count > 0;
		if (flag4)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, charIdList, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> displayDataList = new List<CharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref displayDataList);
				bool flag5 = displayDataList == null;
				if (!flag5)
				{
					Dictionary<int, CharacterDisplayData> dataDict = new Dictionary<int, CharacterDisplayData>();
					foreach (CharacterDisplayData data in displayDataList)
					{
						dataDict[data.CharacterId] = data;
					}
					int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					for (int j = 0; j < 4; j++)
					{
						int selfCharId2 = (this._selfTeam.Count > j) ? this._selfTeam[j] : -1;
						CharacterDisplayData selfData;
						bool flag6 = selfCharId2 >= 0 && dataDict.TryGetValue(selfCharId2, out selfData);
						if (flag6)
						{
							this.RefreshCharacterDisplay(true, j, selfData, selfCharId2 == taiwuId);
						}
					}
					for (int k = 0; k < 4; k++)
					{
						int enemyCharId2 = (this._enemyTeam.Count > k) ? this._enemyTeam[k] : -1;
						CharacterDisplayData enemyData;
						bool flag7 = enemyCharId2 >= 0 && dataDict.TryGetValue(enemyCharId2, out enemyData);
						if (flag7)
						{
							this.RefreshCharacterDisplay(false, k, enemyData, false);
						}
					}
				}
			});
		}
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x000E4404 File Offset: 0x000E2604
	private void RefreshCharacterDisplay(bool isSelf, int index, CharacterDisplayData data, bool isTaiwu)
	{
		bool flag = index == 0;
		Game.Components.Avatar.Avatar avatar;
		TextMeshProUGUI nameLabel;
		if (flag)
		{
			Refers charInfo = isSelf ? this._selfCharInfo : this._enemyCharInfo;
			avatar = charInfo.CGet<Game.Components.Avatar.Avatar>("Avatar");
			nameLabel = charInfo.CGet<TextMeshProUGUI>("CharName");
		}
		else
		{
			RectTransform holder = isSelf ? this._selfTeammateHolder : this._enemyTeammateHolder;
			Refers refers = holder.GetChild(index - 1).GetComponent<Refers>();
			avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			nameLabel = refers.CGet<TextMeshProUGUI>("Name");
		}
		avatar.Refresh(data, true);
		nameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(data, isTaiwu);
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000E44A0 File Offset: 0x000E26A0
	private void ClearCharactersDisplay()
	{
		this._selfCharInfo.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
		this._selfCharInfo.CGet<TextMeshProUGUI>("CharName").text = string.Empty;
		this._enemyCharInfo.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
		this._enemyCharInfo.CGet<TextMeshProUGUI>("CharName").text = string.Empty;
		for (int i = 0; i < 3; i++)
		{
			Refers selfTeammate = this._selfTeammateHolder.GetChild(i).GetComponent<Refers>();
			Refers enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<Refers>();
			selfTeammate.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
			selfTeammate.CGet<TextMeshProUGUI>("Name").text = string.Empty;
			enemyTeammate.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
			enemyTeammate.CGet<TextMeshProUGUI>("Name").text = string.Empty;
		}
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000E459C File Offset: 0x000E279C
	private void PlayBeginAni()
	{
		SkeletonGraphic ani = base.CGet<SkeletonGraphic>("Ani1");
		SkeletonGraphic ani2 = base.CGet<SkeletonGraphic>("Ani2");
		RectTransform cloudLeft = base.CGet<RectTransform>("CloudLeft");
		RectTransform cloudRight = base.CGet<RectTransform>("CloudRight");
		ani.AnimationState.SetAnimation(0, "animation", true);
		ani2.AnimationState.SetAnimation(0, "animation", true);
		ani.gameObject.SetActive(true);
		ani2.gameObject.SetActive(true);
		this.UnlockUi();
		cloudLeft.DOAnchorPosX(-160f, 0.2f, false);
		cloudRight.DOAnchorPosX(180f, 0.2f, false);
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x000E4648 File Offset: 0x000E2848
	private void UnlockUi()
	{
		base.CGet<GameObject>("PointerMask").SetActive(false);
		base.CGet<GameObject>("ClickToStartTips").SetActive(true);
		this._selfCharInfo.CGet<GameObject>("OpenCharMenuTips").SetActive(true);
		this._enemyCharInfo.CGet<GameObject>("OpenCharMenuTips").SetActive(true);
		bool flag = !this._settingData.CombatPrepare;
		if (flag)
		{
			this.StartNoPrepareProgress();
		}
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000E46C4 File Offset: 0x000E28C4
	private void ShowCharMenu(bool isAlly)
	{
		bool prepareFinished = this._prepareFinished;
		if (!prepareFinished)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CharacterIdList", isAlly ? this._selfTeam : this._enemyTeam);
			argBox.Set("CanOperate", isAlly);
			argBox.Set("OpenFromCombatPrepare", true);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIElement characterMenu = UIElement.CharacterMenu;
			characterMenu.OnShowed = (Action)Delegate.Combine(characterMenu.OnShowed, new Action(delegate()
			{
				CommandKitBase.SetDisable(false);
			}));
			UIElement characterMenu2 = UIElement.CharacterMenu;
			characterMenu2.OnHide = (Action)Delegate.Combine(characterMenu2.OnHide, new Action(delegate()
			{
				bool flag2 = !this._settingData.CombatPrepare;
				if (flag2)
				{
					this.StartNoPrepareProgress();
				}
				CommandKitBase.SetDisable(true);
			}));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			bool flag = !this._prepareTog.isOn;
			if (flag)
			{
				this._prepareTog.isOn = true;
			}
		}
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000E47D4 File Offset: 0x000E29D4
	private void OnPrepareTogChange(bool isOn)
	{
		this._settingData.CombatPrepare = isOn;
		if (isOn)
		{
			this.StopNoPrepareProgress(true);
		}
		else
		{
			bool flag = !this._pointerInPauseUi && !this._settingData.CombatPrepare;
			if (flag)
			{
				this.StartNoPrepareProgress();
			}
		}
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000E4824 File Offset: 0x000E2A24
	private void StartNoPrepareProgress()
	{
		float aniTime = 1.5f * (1f - this._noPrepareBarLeft.fillAmount);
		this._noPrepareBarLeft.DOFillAmount(1f, aniTime).SetEase(Ease.Linear);
		this._noPrepareBarRight.DOFillAmount(1f, aniTime).SetEase(Ease.Linear).OnComplete(delegate
		{
			this._prepareFinished = true;
			this.AutoSelectTaiwuAudience(new Action(this.ShowCombatUi));
		});
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000E488C File Offset: 0x000E2A8C
	private void StopNoPrepareProgress(bool clearProgress = false)
	{
		this._noPrepareBarLeft.DOKill(false);
		this._noPrepareBarRight.DOKill(false);
		bool flag = !clearProgress;
		if (!flag)
		{
			this._noPrepareBarLeft.fillAmount = 0f;
			this._noPrepareBarRight.fillAmount = 0f;
		}
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x000E48E0 File Offset: 0x000E2AE0
	private void ShowCombatUi()
	{
		this.ConfirmCardType();
		base.CGet<GameObject>("PointerMask").SetActive(true);
		base.CGet<GameObject>("ClickToStartTips").SetActive(false);
		this._prepareTog.gameObject.SetActive(false);
		this._selfCharInfo.CGet<GameObject>("OpenCharMenuTips").SetActive(false);
		this._enemyCharInfo.CGet<GameObject>("OpenCharMenuTips").SetActive(false);
		UIElement debate = UIElement.Debate;
		debate.OnShowed = (Action)Delegate.Combine(debate.OnShowed, new Action(delegate()
		{
			UIManager.Instance.HideUI(UIElement.LifeSkillCombatBegin);
		}));
		UIManager.Instance.ShowUI(UIElement.Debate, true);
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x000E49A4 File Offset: 0x000E2BA4
	private void RefreshLifeSkillInfo()
	{
		this._selfCharInfo.CGet<CImage>("TypeIcon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", this._lifeSkillType), false, null);
		this._enemyCharInfo.CGet<CImage>("TypeIcon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", this._lifeSkillType), false, null);
		this._selfCharInfo.CGet<TextMeshProUGUI>("TypeText").text = LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", this._lifeSkillType)) + LocalStringManager.Get(LanguageKey.LK_Attainment);
		this._enemyCharInfo.CGet<TextMeshProUGUI>("TypeText").text = LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", this._lifeSkillType)) + LocalStringManager.Get(LanguageKey.LK_Attainment);
		this._selfCharInfo.CGet<TextMeshProUGUI>("Count").text = (ref this._taiwuLifeSkillAttainments.Items.FixedElementField + (IntPtr)this._lifeSkillType * 2).ToString();
		this._enemyCharInfo.CGet<TextMeshProUGUI>("Count").text = (ref this._enemyLifeSkillAttainments.Items.FixedElementField + (IntPtr)this._lifeSkillType * 2).ToString();
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x000E4AF4 File Offset: 0x000E2CF4
	private void InitSpectators()
	{
		this.Model.ClearAudience();
		TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._lifeSkillType, this._enemyId, false, null, delegate(int offset, RawDataPool pool)
		{
			List<int> charList = new List<int>();
			Serializer.Deserialize(pool, offset, ref charList);
			bool flag = charList == null || charList.Count <= 0;
			if (flag)
			{
				this.<InitSpectators>g__OnEnd|49_1();
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, charList, delegate(int offset2, RawDataPool pool2)
				{
					List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
					Serializer.Deserialize(pool2, offset2, ref charDataList);
					bool flag2 = charDataList != null;
					if (flag2)
					{
						List<CharacterDisplayData> enemyAudienceList = this.Model.GetAudienceList(false);
						for (int i = 0; i < charDataList.Count; i++)
						{
							bool flag3 = enemyAudienceList.CheckIndex(i);
							if (flag3)
							{
								enemyAudienceList[i] = charDataList[i];
							}
						}
					}
					this.<InitSpectators>g__OnEnd|49_1();
				});
			}
		});
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000E4B28 File Offset: 0x000E2D28
	private void RefreshAudience(bool isTaiwu)
	{
		Refers infoRoot = isTaiwu ? this._selfCharInfo : this._enemyCharInfo;
		RectTransform layout = infoRoot.CGet<RectTransform>("AudienceLayout");
		for (int i = 0; i < layout.childCount; i++)
		{
			int index = i;
			Refers refers = layout.GetChild(i).GetComponent<Refers>();
			Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(isTaiwu);
			CharacterDisplayData charData = audienceList[i];
			bool hasChar = charData != null;
			avatar.gameObject.SetActive(hasChar);
			refers.CGet<GameObject>("NameBack").SetActive(hasChar);
			refers.CGet<GameObject>("AvatarBg").SetActive(hasChar);
			bool hasChar2 = hasChar;
			if (hasChar2)
			{
				avatar.Refresh(charData, true);
				refers.CGet<TextMeshProUGUI>("Name").text = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
			}
			CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
			button.gameObject.SetActive(isTaiwu);
			button.ClearAndAddListener(delegate
			{
				this.OpenSelectChar(index, hasChar ? index : -1);
			});
			TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
			TooltipInvoker tip2 = refers.GetComponent<TooltipInvoker>();
			tip.enabled = false;
			tip2.enabled = false;
			TooltipInvoker tip3 = isTaiwu ? tip : tip2;
			UI_LifeSkillCombatBegin.RefreshAudienceTip(tip3, hasChar, charData);
		}
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000E4CAC File Offset: 0x000E2EAC
	private static void RefreshAudienceTip(TooltipInvoker tip, bool hasChar, CharacterDisplayData charData)
	{
		tip.enabled = true;
		if (hasChar)
		{
			tip.Type = TipType.LifeSkillCombatAudience;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tip.RuntimeParam.SetObject("CharData", charData);
		}
		else
		{
			tip.Type = TipType.SingleDesc;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Audience_Tips));
		}
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000E4D54 File Offset: 0x000E2F54
	private void OpenSelectChar(int index, int lastIndex)
	{
		UI_LifeSkillCombatBegin.<>c__DisplayClass52_0 CS$<>8__locals1 = new UI_LifeSkillCombatBegin.<>c__DisplayClass52_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.lastIndex = lastIndex;
		CS$<>8__locals1.index = index;
		bool prepareFinished = this._prepareFinished;
		if (!prepareFinished)
		{
			this._prepareTog.isOn = true;
			CS$<>8__locals1.selfAudienceList = this.Model.GetAudienceList(true);
			List<CharacterDisplayData> enemyAudienceList = this.Model.GetAudienceList(false);
			List<int> selectedList = (from d in CS$<>8__locals1.selfAudienceList.Union(enemyAudienceList)
			where d != null
			select d.CharacterId).ToList<int>();
			bool hideTaiwuAudience = this.Model.HideTaiwuAudience;
			if (hideTaiwuAudience)
			{
				List<int> charIdList = new List<int>();
				CS$<>8__locals1.<OpenSelectChar>g__OnEnd|3(charIdList);
			}
			else
			{
				TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._lifeSkillType, this._enemyId, true, selectedList, delegate(int offset, RawDataPool pool)
				{
					List<int> charIdList2 = new List<int>();
					Serializer.Deserialize(pool, offset, ref charIdList2);
					base.<OpenSelectChar>g__OnEnd|3(charIdList2);
				});
			}
		}
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x000E4E5C File Offset: 0x000E305C
	private void InitCardTypeToggleGroup()
	{
		List<CToggleObsolete> toggles = this._toggleGroup.GetAll();
		sbyte i = 0;
		while ((int)i < toggles.Count)
		{
			sbyte skillType = i;
			CToggleObsolete toggle = toggles[(int)skillType];
			LifeSkillTypeItem config = Config.LifeSkillType.Instance[skillType];
			Refers refers = toggle.GetComponent<Refers>();
			refers.CGet<CImage>("SelectedIcon").SetSprite(string.Format("lifeskillcombat_prepare_icon_0_{0}", skillType), false, null);
			refers.CGet<CImage>("UnselectedIcon").SetSprite(string.Format("lifeskillcombat_prepare_icon_3_{0}", skillType), false, null);
			refers.CGet<TextMeshProUGUI>("LabelOff").SetText(config.Name, true);
			refers.CGet<TextMeshProUGUI>("LabelOn").SetText(config.Name, true);
			List<Config.LifeSkillItem> unlockedSkills = (from s in this._lifeSkillMonitor.LearnedLifeSkills
			where LifeSkill.Instance[s.SkillTemplateId].Type == skillType && s.IsAllPagesRead()
			select LifeSkill.Instance[s.SkillTemplateId]).ToList<Config.LifeSkillItem>();
			bool[] unlockedStateArray = new bool[3];
			bool flag = unlockedSkills.Exists(delegate(Config.LifeSkillItem s)
			{
				sbyte grade = s.Grade;
				return grade >= 0 && grade <= 2;
			});
			if (flag)
			{
				unlockedStateArray[0] = true;
			}
			bool flag2 = unlockedSkills.Exists(delegate(Config.LifeSkillItem s)
			{
				sbyte grade = s.Grade;
				return grade >= 3 && grade <= 5;
			});
			if (flag2)
			{
				unlockedStateArray[1] = true;
			}
			bool flag3 = unlockedSkills.Exists(delegate(Config.LifeSkillItem s)
			{
				sbyte grade = s.Grade;
				return grade >= 6 && grade <= 8;
			});
			if (flag3)
			{
				unlockedStateArray[2] = true;
			}
			int unlockedStrategyCount = unlockedStateArray.Count((bool s) => s);
			refers.CGet<TextMeshProUGUI>("CardCount").SetText(unlockedStrategyCount.ToString(), true);
			bool isSelected = this._selectedTypeList.Contains(skillType);
			this._toggleGroup.Set((int)i, isSelected, true);
			TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("LifeSkillType", skillType).SetObject("UnlockedState", unlockedStateArray);
			}
			tip.Type = TipType.LifeSkillCombatCardType;
			i += 1;
		}
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x000E50EC File Offset: 0x000E32EC
	private void OnActiveToggleChange(CToggleObsolete newTOg, CToggleObsolete oldTog)
	{
		bool flag = newTOg;
		if (flag)
		{
			this._prepareTog.isOn = true;
		}
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000E5114 File Offset: 0x000E3314
	private void ConfirmCardType()
	{
		List<sbyte> typeList = (from t in this._toggleGroup.GetAllActive()
		select (sbyte)t.Key).ToList<sbyte>();
		TaiwuDomainMethod.Call.DebateGameSetTaiwuSelectedCardTypes(typeList);
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000E5160 File Offset: 0x000E3360
	private void AutoSelectTaiwuAudience(Action onEnd)
	{
		List<int> selfAudienceList = (from d in this.Model.GetAudienceList(true)
		where d != null
		select d.CharacterId).ToList<int>();
		List<int> enemyAudienceList = (from d in this.Model.GetAudienceList(false)
		where d != null
		select d.CharacterId).ToList<int>();
		bool flag = selfAudienceList.Count >= 3 || this.Model.HideTaiwuAudience;
		if (flag)
		{
			onEnd();
		}
		else
		{
			List<int> allAudienceList = selfAudienceList.Union(enemyAudienceList).ToList<int>();
			AsyncMethodCallbackDelegate <>9__5;
			TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this.Model.LifeSkillType, this.Model.EnemyCharId, true, allAudienceList, delegate(int offset, RawDataPool pool)
			{
				List<int> charList = new List<int>();
				Serializer.Deserialize(pool, offset, ref charList);
				bool flag2 = charList.Count > 0;
				if (flag2)
				{
					IAsyncMethodRequestHandler <>4__this = this;
					List<int> charIdList = charList;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__5) == null)
					{
						callback = (<>9__5 = delegate(int offset2, RawDataPool pool2)
						{
							List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
							Serializer.Deserialize(pool2, offset2, ref charDataList);
							bool flag3 = charDataList != null;
							if (flag3)
							{
								List<CharacterDisplayData> taiwuAudienceDataList = this.Model.GetAudienceList(true);
								foreach (CharacterDisplayData data in charDataList)
								{
									int emptyIndex = taiwuAudienceDataList.FindIndex((CharacterDisplayData d) => d == null);
									bool flag4 = taiwuAudienceDataList.CheckIndex(emptyIndex);
									if (flag4)
									{
										taiwuAudienceDataList[emptyIndex] = data;
									}
								}
							}
							onEnd();
						});
					}
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(<>4__this, charIdList, callback);
				}
				else
				{
					onEnd();
				}
			});
		}
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000E5450 File Offset: 0x000E3650
	[CompilerGenerated]
	private IEnumerator <OnInit>g__AnimRoutine|27_2()
	{
		UI_LifeSkillCombatBegin.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.timePoints = new float[]
		{
			0f,
			0.2f,
			1.3f,
			4f,
			5.1f,
			7.67f
		};
		ParticleSystem particle = base.CGet<ParticleSystem>("AniControl");
		AudioManager audioManager = AudioManager.Instance;
		audioManager.PlayAmbience("art_AmbienceFan", 0.1f, 100);
		do
		{
			CS$<>8__locals1.timePointIndex = 0;
			particle.Stop();
			yield return new WaitForSeconds(UI_LifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|27_4(ref CS$<>8__locals1));
			audioManager.PlaySound("art_AnimationFan", false, false);
			yield return new WaitForSeconds(UI_LifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|27_4(ref CS$<>8__locals1));
			particle.Stop();
			particle.Play();
			yield return new WaitForSeconds(UI_LifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|27_4(ref CS$<>8__locals1));
			audioManager.PlaySound("art_AnimationFan", false, false);
			yield return new WaitForSeconds(UI_LifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|27_4(ref CS$<>8__locals1));
			particle.Stop();
			particle.Play();
			yield return new WaitForSeconds(UI_LifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|27_4(ref CS$<>8__locals1));
		}
		while (particle.gameObject.activeInHierarchy);
		yield break;
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x000E5470 File Offset: 0x000E3670
	[CompilerGenerated]
	internal static float <OnInit>g__GetAndAdvanceTimePoint|27_4(ref UI_LifeSkillCombatBegin.<>c__DisplayClass27_0 A_0)
	{
		float timePoint = A_0.timePoints[A_0.timePointIndex + 1] - A_0.timePoints[A_0.timePointIndex];
		A_0.timePointIndex++;
		return timePoint;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000E5737 File Offset: 0x000E3937
	[CompilerGenerated]
	private void <InitSpectators>g__OnEnd|49_1()
	{
		this.RefreshAudience(true);
		this.RefreshAudience(false);
		this.InitCardTypeToggleGroup();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x0400178E RID: 6030
	private const byte TeamCapacity = 4;

	// Token: 0x0400178F RID: 6031
	private const float NoPrepareAniTime = 1.5f;

	// Token: 0x04001790 RID: 6032
	private sbyte _lifeSkillType;

	// Token: 0x04001791 RID: 6033
	private int _enemyId;

	// Token: 0x04001792 RID: 6034
	private readonly List<int> _selfTeam = new List<int>();

	// Token: 0x04001793 RID: 6035
	private readonly List<int> _enemyTeam = new List<int>();

	// Token: 0x04001794 RID: 6036
	private sbyte _firstMoveType;

	// Token: 0x04001795 RID: 6037
	private GlobalSettings _settingData;

	// Token: 0x04001796 RID: 6038
	private bool _gettingSelfWisdom;

	// Token: 0x04001797 RID: 6039
	private Refers _selfCharInfo;

	// Token: 0x04001798 RID: 6040
	private RectTransform _selfTeammateHolder;

	// Token: 0x04001799 RID: 6041
	private LifeSkillShorts _taiwuLifeSkillAttainments;

	// Token: 0x0400179A RID: 6042
	private LifeSkillShorts _enemyLifeSkillAttainments;

	// Token: 0x0400179B RID: 6043
	private Refers _enemyCharInfo;

	// Token: 0x0400179C RID: 6044
	private RectTransform _enemyTeammateHolder;

	// Token: 0x0400179D RID: 6045
	private CToggleObsolete _prepareTog;

	// Token: 0x0400179E RID: 6046
	private bool _pointerInPauseUi;

	// Token: 0x0400179F RID: 6047
	private bool _prepareFinished;

	// Token: 0x040017A0 RID: 6048
	private CImage _noPrepareBarLeft;

	// Token: 0x040017A1 RID: 6049
	private CImage _noPrepareBarRight;

	// Token: 0x040017A2 RID: 6050
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x040017A3 RID: 6051
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x040017A4 RID: 6052
	private List<sbyte> _selectedTypeList = new List<sbyte>();

	// Token: 0x040017A5 RID: 6053
	private Coroutine _animRoutine;

	// Token: 0x040017A6 RID: 6054
	private bool _awaken;

	// Token: 0x040017A7 RID: 6055
	private float _ambienceGlobalVolume;
}
