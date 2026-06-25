using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UI;
using GameData.GameDataBridge;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class UIElement
{
	// Token: 0x0600065E RID: 1630 RVA: 0x0002A498 File Offset: 0x00028698
	private static List<UIElement> Elements(params UIElement[] elems)
	{
		return new List<UIElement>(elems);
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x0600065F RID: 1631 RVA: 0x0002A4B0 File Offset: 0x000286B0
	public bool Exist
	{
		get
		{
			return null != this.UiBase && this.UiBase.gameObject.activeInHierarchy;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000660 RID: 1632 RVA: 0x0002A4D3 File Offset: 0x000286D3
	public bool IsShowing
	{
		get
		{
			return !this._stateMachine.IsInState(EUiElementState.Sleep) && !this._stateMachine.IsInState(EUiElementState.AnimateOut) && !this._stateMachine.IsInState(EUiElementState.Hiding);
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000661 RID: 1633 RVA: 0x0002A504 File Offset: 0x00028704
	public bool Ready
	{
		get
		{
			bool flag = this.ServeGroup != null;
			bool result;
			if (flag)
			{
				result = this.ServeGroup.StateReady;
			}
			else
			{
				result = this._ready;
			}
			return result;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000662 RID: 1634 RVA: 0x0002A537 File Offset: 0x00028737
	// (set) Token: 0x06000663 RID: 1635 RVA: 0x0002A53F File Offset: 0x0002873F
	public bool CanQuickHide { get; private set; } = true;

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000664 RID: 1636 RVA: 0x0002A548 File Offset: 0x00028748
	// (set) Token: 0x06000665 RID: 1637 RVA: 0x0002A550 File Offset: 0x00028750
	public bool IsFullScreen { get; private set; } = false;

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000666 RID: 1638 RVA: 0x0002A55C File Offset: 0x0002875C
	public bool IsWaitShowing
	{
		get
		{
			UIElementStateBase state = this._stateMachine.GetCurrentState() as UIElementStateBase;
			bool flag = state == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				EUiElementState uiState = (EUiElementState)state.stateName;
				result = (uiState >= EUiElementState.ResourcePrepare && uiState <= EUiElementState.AnimateIn);
			}
			return result;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000667 RID: 1639 RVA: 0x0002A5A8 File Offset: 0x000287A8
	public string Name
	{
		get
		{
			bool flag = string.IsNullOrEmpty(this._path);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = this._path.Substring(this._path.LastIndexOf("/", StringComparison.Ordinal) + 1);
			}
			return result;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000668 RID: 1640 RVA: 0x0002A5F0 File Offset: 0x000287F0
	public UIFlag UiFlags
	{
		get
		{
			bool flag = null == this.UiBase;
			UIFlag result;
			if (flag)
			{
				result = UIFlag.None;
			}
			else
			{
				result = this.UiBase.UiFlags;
			}
			return result;
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x0002A624 File Offset: 0x00028824
	public UIElement()
	{
		this._stateMachine = new UIElementStateMachine();
		this._stateMachine.Element = this;
		this._stateMachine.TranslateState(EUiElementState.Sleep, null);
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0002A678 File Offset: 0x00028878
	public void ResetElement()
	{
		bool flag = null == this.UiBase;
		if (!flag)
		{
			bool flag2 = this.UiBase.AnimOut != null;
			if (flag2)
			{
				this.UiBase.AnimOut.autoPlay = false;
				this.UiBase.AnimOut.autoKill = false;
				bool flag3 = this.UiBase.AnimOut.tween != null;
				if (flag3)
				{
					this.UiBase.AnimOut.tween.onComplete = null;
					bool flag4 = this.UiBase.AnimOut.tween.IsPlaying();
					if (flag4)
					{
						this.UiBase.AnimOut.tween.Complete(false);
					}
				}
			}
			else
			{
				bool flag5 = this.UiBase.GetAnimSequenceOut() != null;
				if (flag5)
				{
					Sequence seqOut = this.UiBase.GetAnimSequenceOut();
					seqOut.onComplete = null;
					bool flag6 = seqOut.IsPlaying();
					if (flag6)
					{
						seqOut.Complete(false);
						seqOut.Pause<Sequence>();
					}
				}
			}
			UIBase uiBase = this.UiBase;
			if (uiBase != null)
			{
				uiBase.OnReset();
			}
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0002A798 File Offset: 0x00028998
	public void InitElement()
	{
		this.UiBase.OnInit(this._argsBox);
		EasyPool.Free<ArgumentBox>(this._argsBox);
		this._argsBox = null;
		this.SetUIVisible(true);
		Sequence sequence;
		bool flag = this.UiBase.GetAnimInGroupSequence(out sequence) && !sequence.IsPlaying();
		if (flag)
		{
			sequence.Rewind(true);
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0002A7FC File Offset: 0x000289FC
	public void MonitorData()
	{
		this.UiBase.InitMonitorFieldIds();
		bool flag = this.GameDataListenerId < 0 && this.UiBase.NeedGameDataListenerId(false);
		if (flag)
		{
			this.GameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.UiBase.OnNotifyGameData));
		}
		List<UIBase.MonitorDataField> dataFieldIdList = this.UiBase.GetMonitorFields();
		for (int i = 0; i < dataFieldIdList.Count; i++)
		{
			UIBase.MonitorDataField dataField = dataFieldIdList[i];
			bool flag2 = dataField.SubId1List != null;
			if (flag2)
			{
				GameDataBridge.AddDataMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
		}
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0002A8D0 File Offset: 0x00028AD0
	public void UnMonitorData()
	{
		this.UiBase.ClearAsyncMethodCalls();
		bool flag = this.GameDataListenerId >= 0;
		if (flag)
		{
			this.UiBase.ClearMonitorFields();
			GameDataBridge.UnregisterListener(this.GameDataListenerId);
		}
		this.GameDataListenerId = -1;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0002A91C File Offset: 0x00028B1C
	public virtual bool NeedWaitData()
	{
		return this.UiBase != null && this.UiBase.NeedGameDataListenerId(true);
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0002A94C File Offset: 0x00028B4C
	public void PlayShowAnimation(Action onAnimationComplete, bool skipLastEvent = true)
	{
		bool flag = !skipLastEvent;
		if (flag)
		{
			Action onAnimationInComplete = this._onAnimationInComplete;
			if (onAnimationInComplete != null)
			{
				onAnimationInComplete();
			}
		}
		this._onAnimationInComplete = onAnimationComplete;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			bool flag3 = this.UiBase == null;
			if (!flag3)
			{
				bool flag4 = null == this.UiBase.AnimIn && this.UiBase.GetAnimSequenceIn() == null;
				if (flag4)
				{
					this.<PlayShowAnimation>g__OnComplete|43_0();
				}
			}
		});
		Sequence sequence;
		bool animInGroupSequence = this.UiBase.GetAnimInGroupSequence(out sequence);
		if (animInGroupSequence)
		{
			sequence.SetAutoKill(false);
			sequence.OnComplete(new TweenCallback(this.<PlayShowAnimation>g__OnComplete|43_0));
			sequence.Restart(true, -1f);
		}
		else
		{
			bool flag2 = this.UiBase.GetAnimSequenceIn() != null;
			if (flag2)
			{
				Sequence seqIn = this.UiBase.GetAnimSequenceIn();
				seqIn.SetAutoKill(false);
				seqIn.OnComplete(new TweenCallback(this.<PlayShowAnimation>g__OnComplete|43_0));
				seqIn.Restart(true, -1f);
			}
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0002AA20 File Offset: 0x00028C20
	public void PlayHideAnimation(Action onAnimationComplete, bool skipLastEvent = true)
	{
		bool flag = !skipLastEvent;
		if (flag)
		{
			Action onAnimationOutComplete = this._onAnimationOutComplete;
			if (onAnimationOutComplete != null)
			{
				onAnimationOutComplete();
			}
		}
		this._onAnimationOutComplete = onAnimationComplete;
		Sequence sequence;
		bool animInGroupSequence = this.UiBase.GetAnimInGroupSequence(out sequence);
		if (animInGroupSequence)
		{
			bool flag2 = sequence.IsPlaying();
			if (flag2)
			{
				sequence.Pause<Sequence>();
				bool flag3 = !skipLastEvent;
				if (flag3)
				{
					TweenCallback onComplete = sequence.onComplete;
					if (onComplete != null)
					{
						onComplete();
					}
				}
			}
			sequence.onComplete = null;
		}
		else
		{
			bool flag4 = this.UiBase.GetAnimSequenceIn() != null;
			if (flag4)
			{
				Sequence seqIn = this.UiBase.GetAnimSequenceIn();
				bool flag5 = seqIn.IsPlaying();
				if (flag5)
				{
					seqIn.Pause<Sequence>();
					bool flag6 = !skipLastEvent;
					if (flag6)
					{
						TweenCallback onComplete2 = seqIn.onComplete;
						if (onComplete2 != null)
						{
							onComplete2();
						}
					}
				}
				seqIn.onComplete = null;
			}
		}
		bool flag7 = null != this.UiBase.AnimOut;
		if (flag7)
		{
			this.UiBase.AnimOut.tween.SetUpdate(true);
			this.UiBase.AnimOut.tween.OnComplete(new TweenCallback(this.<PlayHideAnimation>g__OnComplete|44_0));
			this.UiBase.AnimOut.hasOnComplete = true;
			this.UiBase.AnimOut.tween.Restart(true, -1f);
		}
		else
		{
			bool flag8 = this.UiBase.GetAnimSequenceOut() != null;
			if (flag8)
			{
				Sequence seqOut = this.UiBase.GetAnimSequenceOut();
				seqOut.SetAutoKill(false);
				seqOut.OnComplete(new TweenCallback(this.<PlayHideAnimation>g__OnComplete|44_0));
				seqOut.Restart(true, -1f);
			}
			else
			{
				this.<PlayHideAnimation>g__OnComplete|44_0();
			}
		}
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0002ABD4 File Offset: 0x00028DD4
	public void SetElementReady(bool isReady)
	{
		this._ready = isReady;
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0002ABDE File Offset: 0x00028DDE
	public void NotifyElementShow()
	{
		UIBase uiBase = this.UiBase;
		if (uiBase != null)
		{
			uiBase.NotifyUIShow();
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0002ABF3 File Offset: 0x00028DF3
	public void NotifyElementHide()
	{
		this.UiBase.NotifyUIHide();
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0002AC02 File Offset: 0x00028E02
	public void NotifyElementShowFinish()
	{
		UIBase uiBase = this.UiBase;
		if (uiBase != null)
		{
			uiBase.NotifyUIShowFinish();
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0002AC17 File Offset: 0x00028E17
	public void NotifyElementHideStart()
	{
		UIBase uiBase = this.UiBase;
		if (uiBase != null)
		{
			uiBase.NotifyUIHideStart();
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0002AC2C File Offset: 0x00028E2C
	public void SetUIVisible(bool visible)
	{
		bool flag = null == this.UiBase;
		if (flag)
		{
			GLog.TagWarn("UIElement", this.Name + ":SetUIVisible is called while null == UiBase!", Array.Empty<object>());
		}
		else
		{
			bool flag2 = null == this.UiBase.gameObject;
			if (flag2)
			{
				GLog.TagWarn("UIElement", this.Name + ":SetUIVisible is called while Element called Hide!", Array.Empty<object>());
				return;
			}
			bool flag3 = !visible;
			if (flag3)
			{
				UIMaskManager maskManager = SingletonObject.getInstance<UIMaskManager>();
				if (maskManager != null)
				{
					maskManager.OnUIBaseHidden(this.UiBase.transform);
				}
			}
			this.UiBase.gameObject.GetComponent<Canvas>().enabled = visible;
			this.UiBase.gameObject.SetActive(visible);
		}
		if (visible)
		{
			this.SetTransformToLastSibling();
			Action onActive = this.OnActive;
			if (onActive != null)
			{
				onActive();
			}
			this.OnActive = null;
		}
		else
		{
			Action onDeActive = this.OnDeActive;
			if (onDeActive != null)
			{
				onDeActive();
			}
			this.OnDeActive = null;
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0002AD44 File Offset: 0x00028F44
	public void PrepareRes(bool autoShow = true, Action<GameObject> onPrefabLoaded = null, bool isLoadAsyncInBackground = false)
	{
		UIElement.<>c__DisplayClass51_0 CS$<>8__locals1 = new UIElement.<>c__DisplayClass51_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.onPrefabLoaded = onPrefabLoaded;
		CS$<>8__locals1.autoShow = autoShow;
		bool flag = this._path.IsNullOrEmpty();
		if (flag)
		{
			GLog.TagError("UIElement", "Fetal Error:UIElement type=" + base.GetType().FullName + " set empty prefab path!", Array.Empty<object>());
		}
		else
		{
			bool flag2 = null != this.UiBase;
			if (flag2)
			{
				bool autoShow2 = CS$<>8__locals1.autoShow;
				if (autoShow2)
				{
					this._stateMachine.TranslateState(EUiElementState.Reset, null);
				}
			}
			else
			{
				ResLoader.Load<GameObject>(Path.Combine(UIElement.rootPrefabPath, this._path), new Action<GameObject>(CS$<>8__locals1.<PrepareRes>g__OnPrefabLoaded|0), null, isLoadAsyncInBackground);
			}
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x0002AE00 File Offset: 0x00029000
	public virtual void SetTransformToLastSibling()
	{
		bool flag = null != this.UiBase;
		if (flag)
		{
			this.UiBase.transform.SetAsLastSibling();
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0002AE2F File Offset: 0x0002902F
	public void SetOnInitArgs(ArgumentBox box)
	{
		this._argsBox = (((box != null) ? box.Clone() : null) as ArgumentBox);
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0002AE4C File Offset: 0x0002904C
	public virtual void Show()
	{
		bool isWaitShowing = this.IsWaitShowing;
		if (!isWaitShowing)
		{
			bool flag = this._stateMachine.IsInState(EUiElementState.Sleep);
			if (flag)
			{
				this._stateMachine.TranslateState(EUiElementState.ResourcePrepare, null);
			}
			else
			{
				this._stateMachine.TranslateState(EUiElementState.Reset, null);
			}
		}
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0002AEA0 File Offset: 0x000290A0
	public bool IsInState(EUiElementState state)
	{
		UIElementStateMachine stateMachine = this._stateMachine;
		return stateMachine != null && stateMachine.IsInState(state);
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0002AEC5 File Offset: 0x000290C5
	protected void TranslateState(EUiElementState state)
	{
		this._stateMachine.TranslateState(state, null);
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0002AEDC File Offset: 0x000290DC
	public virtual void ShowAfterRefresh()
	{
		bool flag = this._ready || this.IsInState(EUiElementState.AnimateIn) || this.IsInState(EUiElementState.AnimateOut) || this.IsInState(EUiElementState.Hiding);
		if (!flag)
		{
			this._stateMachine.TranslateState(EUiElementState.AnimateIn, null);
		}
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0002AF28 File Offset: 0x00029128
	public virtual void Hide(bool quickHide = false)
	{
		bool flag = null == this.UiBase;
		if (!flag)
		{
			if (quickHide)
			{
				this.NotifyElementHideStart();
				this._stateMachine.TranslateState(EUiElementState.Hiding, null);
			}
			else
			{
				this._stateMachine.TranslateState(EUiElementState.AnimateOut, null);
			}
		}
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x0002AF80 File Offset: 0x00029180
	public void Destroy()
	{
		bool flag = this.Exist && !this.IsInState(EUiElementState.Hiding);
		if (flag)
		{
			this._stateMachine.TranslateState(EUiElementState.Hiding, null);
		}
		this._stateMachine.TranslateState(EUiElementState.Sleep, null);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x0002AFD0 File Offset: 0x000291D0
	public void DestroyUIBase()
	{
		bool flag = null == this.UiBase;
		if (!flag)
		{
			bool activeInHierarchy = this.UiBase.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				GLog.TagWarn(this.Name, "Failed to destroy " + this.Name + ",call Hide() before DestroyUIBase", Array.Empty<object>());
			}
			else
			{
				this.UiBase.UnRegisterRelativeAtlases();
				Object.Destroy(this.UiBase.gameObject);
				GLog.LogWithTag(this.Name + " Destroyed", Array.Empty<object>());
				this.UiBase = null;
			}
		}
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0002B06C File Offset: 0x0002926C
	public T UiBaseAs<T>() where T : UIBase
	{
		bool flag = null != this.UiBase;
		T result;
		if (flag)
		{
			result = (this.UiBase as T);
		}
		else
		{
			result = default(T);
		}
		return result;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0002B0AC File Offset: 0x000292AC
	public override string ToString()
	{
		return base.GetType().FullName + ":" + this.Name;
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0002DFF6 File Offset: 0x0002C1F6
	[CompilerGenerated]
	private void <PlayShowAnimation>g__OnComplete|43_0()
	{
		Action onAnimationInComplete = this._onAnimationInComplete;
		if (onAnimationInComplete != null)
		{
			onAnimationInComplete();
		}
		this._onAnimationInComplete = null;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0002E063 File Offset: 0x0002C263
	[CompilerGenerated]
	private void <PlayHideAnimation>g__OnComplete|44_0()
	{
		Action onAnimationOutComplete = this._onAnimationOutComplete;
		if (onAnimationOutComplete != null)
		{
			onAnimationOutComplete();
		}
		this._onAnimationOutComplete = null;
	}

	// Token: 0x0400052F RID: 1327
	private static string rootPrefabPath = "RemakeResources/Prefab/Views/";

	// Token: 0x04000530 RID: 1328
	private string _path;

	// Token: 0x04000531 RID: 1329
	private UIElementStateMachine _stateMachine;

	// Token: 0x04000532 RID: 1330
	private ArgumentBox _argsBox;

	// Token: 0x04000533 RID: 1331
	public UIBase UiBase;

	// Token: 0x04000534 RID: 1332
	public UIGroup ServeGroup;

	// Token: 0x04000535 RID: 1333
	public Action OnListenerIdReady;

	// Token: 0x04000536 RID: 1334
	public Action OnActive;

	// Token: 0x04000537 RID: 1335
	public Action OnDeActive;

	// Token: 0x04000538 RID: 1336
	public Action OnShowed;

	// Token: 0x04000539 RID: 1337
	public Action OnHide;

	// Token: 0x0400053A RID: 1338
	public int GameDataListenerId = -1;

	// Token: 0x0400053B RID: 1339
	public bool ForceListenCommand;

	// Token: 0x0400053C RID: 1340
	private bool _ready;

	// Token: 0x0400053F RID: 1343
	private Action _onAnimationInComplete;

	// Token: 0x04000540 RID: 1344
	private Action _onAnimationOutComplete;

	// Token: 0x04000541 RID: 1345
	public static UIElement CharacterMenu = new UIElement
	{
		_path = "CharacterMenu/ViewCharacterMenu",
		IsFullScreen = true
	};

	// Token: 0x04000542 RID: 1346
	public static CharacterMenuSubPageElement CharacterMenuInfo = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuInfo",
		CanQuickHide = false
	};

	// Token: 0x04000543 RID: 1347
	public static CharacterMenuSubPageElement CharacterMenuTeam = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuTeam",
		CanQuickHide = false
	};

	// Token: 0x04000544 RID: 1348
	public static CharacterMenuSubPageElement CharacterMenuKidnap = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuKidnap",
		CanQuickHide = false
	};

	// Token: 0x04000545 RID: 1349
	public static CharacterMenuSubPageElement CharacterMenuEquip = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuEquip",
		CanQuickHide = false
	};

	// Token: 0x04000546 RID: 1350
	public static CharacterMenuSubPageElement CharacterMenuItems = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuItems",
		CanQuickHide = false,
		ForceListenCommand = true
	};

	// Token: 0x04000547 RID: 1351
	public static CharacterMenuSubPageElement CharacterMenuLifeSkill = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuLifeSkill",
		CanQuickHide = false
	};

	// Token: 0x04000548 RID: 1352
	public static CharacterMenuSubPageElement CharacterMenuCombatSkill = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuCombatSkill",
		CanQuickHide = false
	};

	// Token: 0x04000549 RID: 1353
	public static CharacterMenuSubPageElement CharacterMennAttaiment = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuAttainment",
		CanQuickHide = false
	};

	// Token: 0x0400054A RID: 1354
	public static CharacterMenuSubPageElement CharacterMenuRelationShip = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuRelationship",
		CanQuickHide = false
	};

	// Token: 0x0400054B RID: 1355
	public static CharacterMenuSubPageElement CharacterMenuGenealogy = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuGenealogy",
		CanQuickHide = false
	};

	// Token: 0x0400054C RID: 1356
	public static CharacterMenuSubPageElement CharacterMenuLifeRecords = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuLifeRecords",
		CanQuickHide = false
	};

	// Token: 0x0400054D RID: 1357
	public static UIElement LifeRecords = new UIElement
	{
		_path = "CharacterMenu/ViewLifeRecords"
	};

	// Token: 0x0400054E RID: 1358
	public static CharacterMenuSubPageElement CharacterMenuInformation = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuInformation",
		CanQuickHide = false
	};

	// Token: 0x0400054F RID: 1359
	public static CharacterMenuSubPageElement CharacterMenuSecretInformation = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuSecret",
		CanQuickHide = false
	};

	// Token: 0x04000550 RID: 1360
	public static CharacterMenuSubPageElement CharacterMenuNeili = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuNeili",
		CanQuickHide = false
	};

	// Token: 0x04000551 RID: 1361
	public static CharacterMenuSubPageElement CharacterMenuEquipCombatSkill = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuEquipCombatSkill",
		CanQuickHide = false
	};

	// Token: 0x04000552 RID: 1362
	public static UIElement Samsara = new UIElement
	{
		_path = "ViewSamsara"
	};

	// Token: 0x04000553 RID: 1363
	public static CharacterMenuSubPageElement CharacterMenuPractice = new CharacterMenuSubPageElement
	{
		_path = "CharacterMenu/ViewCharacterMenuPractice",
		CanQuickHide = false
	};

	// Token: 0x04000554 RID: 1364
	public static UIElement SkillBreakPlate = new UIElement
	{
		_path = "CharacterMenu/ViewCharacterMenuSkillBreakPlate"
	};

	// Token: 0x04000555 RID: 1365
	public static UIElement SelectItem = new UIElement
	{
		_path = "Select/ViewSelectItem"
	};

	// Token: 0x04000556 RID: 1366
	public static UIElement CricketCombat = new UIElement
	{
		_path = "Cricket/ViewCricketCombat",
		CanQuickHide = false
	};

	// Token: 0x04000557 RID: 1367
	public static UIElement CricketWishing = new UIElement
	{
		_path = "Cricket/ViewCricketWishing"
	};

	// Token: 0x04000558 RID: 1368
	public static UIElement ScreenFade = new UIElement
	{
		_path = "Cricket/ViewScreenFade"
	};

	// Token: 0x04000559 RID: 1369
	public static UIElement WorldMap = new UIElement
	{
		_path = "Map/ViewWorldMap",
		CanQuickHide = false
	};

	// Token: 0x0400055A RID: 1370
	public static UIElement MouseTipCricketEncyclopedia = new UIElement
	{
		_path = "MouseTips/MouseTipCricketEncyclopedia"
	};

	// Token: 0x0400055B RID: 1371
	public static UIElement MouseTipProfessionEncyclopedia = new UIElement
	{
		_path = "MouseTips/MouseTipProfessionEncyclopedia"
	};

	// Token: 0x0400055C RID: 1372
	public static UIElement MouseTipProfessionSkillEncyclopedia = new UIElement
	{
		_path = "MouseTips/MouseTipProfessionSkillEncyclopedia"
	};

	// Token: 0x0400055D RID: 1373
	public static UIElement MouseTipMakeTargetMaterial = new UIElement
	{
		_path = "MouseTips/MouseTipMakeTargetMaterial"
	};

	// Token: 0x0400055E RID: 1374
	public static UIElement MouseTipAiAction = new UIElement
	{
		_path = "MouseTips/MouseTipAiAction"
	};

	// Token: 0x0400055F RID: 1375
	public static UIElement MouseTipCharacterCurrentProfession = new UIElement
	{
		_path = "MouseTips/MouseTipCharacterCurrentProfession"
	};

	// Token: 0x04000560 RID: 1376
	public static UIElement MouseTipKongSangDing = new UIElement
	{
		_path = "MouseTips/MouseTipKongSangDing"
	};

	// Token: 0x04000561 RID: 1377
	public static UIElement ToolTipSoulPiece = new UIElement
	{
		_path = "MouseTips/ToolTipSoulPiece"
	};

	// Token: 0x04000562 RID: 1378
	public static UIElement LogoShow = new UIElement
	{
		_path = "Legacy/UI_Logo",
		CanQuickHide = false
	};

	// Token: 0x04000563 RID: 1379
	public static UIElement MainMenu = new UIElement
	{
		_path = "ViewMainMenu",
		CanQuickHide = false
	};

	// Token: 0x04000564 RID: 1380
	public static UIElement FirstTime = new UIElement
	{
		_path = "ViewFirstTime",
		CanQuickHide = false
	};

	// Token: 0x04000565 RID: 1381
	public static UIElement Dialog = new UIElement
	{
		_path = "ViewDialog",
		CanQuickHide = false
	};

	// Token: 0x04000566 RID: 1382
	public static UIElement DialogHuge = new UIElement
	{
		_path = "ViewDialogHuge",
		CanQuickHide = false
	};

	// Token: 0x04000567 RID: 1383
	public static UIElement Rename = new UIElement
	{
		_path = "ViewRename",
		CanQuickHide = true
	};

	// Token: 0x04000568 RID: 1384
	public static UIElement ConfirmDialog = new UIElement
	{
		_path = "ViewConfirmDialog",
		CanQuickHide = false
	};

	// Token: 0x04000569 RID: 1385
	public static UIElement ViewConfirmDialogLayoutSmall = new UIElement
	{
		_path = "ViewConfirmDialogLayoutSmall",
		CanQuickHide = false
	};

	// Token: 0x0400056A RID: 1386
	public static UIElement ConfirmDialogLayoutMerchant = new UIElement
	{
		_path = "ViewConfirmDialogLayoutMerchant",
		CanQuickHide = false
	};

	// Token: 0x0400056B RID: 1387
	public static UIElement TangramDialog = new UIElement
	{
		_path = "SectInteract/Jieqing/ViewTangramDialog",
		CanQuickHide = false
	};

	// Token: 0x0400056C RID: 1388
	public static UIElement TextureShow = new UIElement
	{
		_path = "Legacy/UI_TextureShow",
		CanQuickHide = false
	};

	// Token: 0x0400056D RID: 1389
	public static UIElement RecordSelect = new UIElement
	{
		_path = "RecordSelect/ViewRecordSelect",
		CanQuickHide = true
	};

	// Token: 0x0400056E RID: 1390
	public static UIElement Loading = new UIElement
	{
		_path = "Loading/ViewLoading",
		CanQuickHide = false,
		ForceListenCommand = true
	};

	// Token: 0x0400056F RID: 1391
	public static UIElement NewGame = new UIElement
	{
		_path = "NewGame/ViewNewGame",
		CanQuickHide = false
	};

	// Token: 0x04000570 RID: 1392
	public static UIElement Challenge = new UIElement
	{
		_path = "NewGame/ViewChallenge"
	};

	// Token: 0x04000571 RID: 1393
	public static UIElement NewGameFeatureItemSelection = new UIElement
	{
		_path = "NewGame/ViewNewGameFeatureItemSelection"
	};

	// Token: 0x04000572 RID: 1394
	public static UIElement CatchCricket = new UIElement
	{
		_path = "Cricket/ViewCatchCricket",
		CanQuickHide = false
	};

	// Token: 0x04000573 RID: 1395
	public static UIElement TaiwuVillageStoneRoom = new UIElement
	{
		_path = "Building/ViewTaiwuVillageStoneRoom"
	};

	// Token: 0x04000574 RID: 1396
	public static UIElement PartWorld = new UIElement
	{
		_path = "World/ViewPartWorldMap"
	};

	// Token: 0x04000575 RID: 1397
	public static UIElement ResourceBar = new UIElement
	{
		_path = "Bottom/ViewResourceBar",
		CanQuickHide = false
	};

	// Token: 0x04000576 RID: 1398
	public static UIElement KungfuPracticeRoomPuppet = new UIElement
	{
		_path = "Building/ViewPracticeRoomPuppet"
	};

	// Token: 0x04000577 RID: 1399
	public static UIElement NewAreaNotify = new UIElement
	{
		_path = "ViewNewAreaNotify",
		CanQuickHide = false
	};

	// Token: 0x04000578 RID: 1400
	public static UIElement LifeSkillCombatBegin = new UIElement
	{
		_path = "LifeSkillCombat/ViewLifeSkillCombatBegin",
		CanQuickHide = false
	};

	// Token: 0x04000579 RID: 1401
	public static UIElement SelectSkill = new UIElement
	{
		_path = "Select/ViewSelectSkill"
	};

	// Token: 0x0400057A RID: 1402
	public static UIElement TutorialVideoPlayer = new UIElement
	{
		_path = "Tutorial/ViewTutorialVideoPlayer",
		CanQuickHide = true,
		ForceListenCommand = true
	};

	// Token: 0x0400057B RID: 1403
	public static UIElement SkillBreakBonusSelect = new UIElement
	{
		_path = "CharacterMenu/SkillBreak/ViewSkillBreakBonusSelect"
	};

	// Token: 0x0400057C RID: 1404
	public static UIElement Reading = new UIElement
	{
		_path = "Main/Reading/ViewReading"
	};

	// Token: 0x0400057D RID: 1405
	public static UIElement SelectItemLegacy = new UIElement
	{
		_path = "Legacy/UI_SelectItem"
	};

	// Token: 0x0400057E RID: 1406
	public static UIElement SelectAreaItem = new UIElement
	{
		_path = "Legacy/UI_SelectAreaItem"
	};

	// Token: 0x0400057F RID: 1407
	public static UIElement MultiSelectItem = new UIElement
	{
		_path = "Legacy/UI_MultiSelectItem"
	};

	// Token: 0x04000580 RID: 1408
	public static UIElement SelectChicken = new UIElement
	{
		_path = "Select/ViewSelectChicken"
	};

	// Token: 0x04000581 RID: 1409
	public static UIElement AvatarPreset = new UIElement
	{
		_path = "Legacy/UI_AvatarPreset"
	};

	// Token: 0x04000582 RID: 1410
	public static UIElement Combat = new UIElement
	{
		_path = "Combat/ViewCombat",
		CanQuickHide = false
	};

	// Token: 0x04000583 RID: 1411
	public static UIElement CombatBackground = new UIElement
	{
		_path = "Legacy/UI_CombatBackground"
	};

	// Token: 0x04000584 RID: 1412
	public static UIElement CombatResult = new UIElement
	{
		_path = "Combat/ViewCombatResult"
	};

	// Token: 0x04000585 RID: 1413
	public static UIElement CombatDamageDetail = new UIElement
	{
		_path = "Combat/ViewCombatDamageDetail"
	};

	// Token: 0x04000586 RID: 1414
	public static UIElement Advance = new UIElement
	{
		_path = "Legacy/UI_Advance",
		CanQuickHide = false
	};

	// Token: 0x04000587 RID: 1415
	public static UIElement MainOperation = new UIElement
	{
		_path = "Bottom/ViewMainOperation"
	};

	// Token: 0x04000588 RID: 1416
	public static UIElement BlockOperation = new UIElement
	{
		_path = "Bottom/ViewBlockOperation"
	};

	// Token: 0x04000589 RID: 1417
	public static UIElement AdvanceDays = new UIElement
	{
		_path = "Bottom/ViewAdvanceDays"
	};

	// Token: 0x0400058A RID: 1418
	public static UIElement Bottom = new UIElement
	{
		_path = "Bottom/ViewBottom",
		CanQuickHide = false
	};

	// Token: 0x0400058B RID: 1419
	public static UIElement BottomPastTaiwuVillage = new UIElement
	{
		_path = "Bottom/ViewBottomPastTaiwuVillage",
		CanQuickHide = false
	};

	// Token: 0x0400058C RID: 1420
	public static UIElement AdvanceConfirm = new UIElement
	{
		_path = "Legacy/UI_AdvanceConfirm",
		CanQuickHide = false
	};

	// Token: 0x0400058D RID: 1421
	public static UIElement BuildingArea = new UIElement
	{
		_path = "Building/ViewBuildingArea",
		CanQuickHide = false
	};

	// Token: 0x0400058E RID: 1422
	public static UIElement BuildingQuickActionMenu = new UIElement
	{
		_path = "Building/ViewBuildingQuickActionMenu"
	};

	// Token: 0x0400058F RID: 1423
	public static UIElement BuildingManageOld = new UIElement
	{
		_path = "Legacy/Building/UI_BuildingManage"
	};

	// Token: 0x04000590 RID: 1424
	public static UIElement BuildingManage = new UIElement
	{
		_path = "Building/ViewBuildingManage"
	};

	// Token: 0x04000591 RID: 1425
	public static UIElement BuildingRepairDialog = new UIElement
	{
		_path = "Building/ViewBuildingRepairDialog"
	};

	// Token: 0x04000592 RID: 1426
	public static UIElement MapBlockCharList = new UIElement
	{
		_path = "MapBlockCharList/ViewMapBlockCharList",
		CanQuickHide = false
	};

	// Token: 0x04000593 RID: 1427
	public static UIElement MapBlockCharCustomSetting = new UIElement
	{
		_path = "Legacy/UI_MapBlockCharCustomSetting"
	};

	// Token: 0x04000594 RID: 1428
	public static UIElement ButtonSheet = new UIElement
	{
		_path = "Legacy/UI_ButtonSheet"
	};

	// Token: 0x04000595 RID: 1429
	public static UIElement DragShow = new UIElement
	{
		_path = "Legacy/UI_DragShow"
	};

	// Token: 0x04000596 RID: 1430
	public static UIElement SearchResultShow = new UIElement
	{
		_path = "Legacy/UI_SearchResultShow"
	};

	// Token: 0x04000597 RID: 1431
	public static UIElement CollectResource = new UIElement
	{
		_path = "Bottom/ViewCollectResource",
		CanQuickHide = false
	};

	// Token: 0x04000598 RID: 1432
	public static UIElement ChickenMap = new UIElement
	{
		_path = "Legacy/UI_ChickenMap",
		CanQuickHide = true
	};

	// Token: 0x04000599 RID: 1433
	public static UIElement GetItem = new UIElement
	{
		_path = "Obtain/ViewObtain",
		CanQuickHide = false
	};

	// Token: 0x0400059A RID: 1434
	public static UIElement SectMainStoryUnlock = new UIElement
	{
		_path = "ViewSectMainStoryUnlock",
		CanQuickHide = false
	};

	// Token: 0x0400059B RID: 1435
	public static UIElement NewFunctionUnlock = new UIElement
	{
		_path = "ViewNewFunctionUnlock",
		CanQuickHide = false
	};

	// Token: 0x0400059C RID: 1436
	public static UIElement SwitchDate = new UIElement
	{
		_path = "ViewSwitchDate",
		CanQuickHide = false
	};

	// Token: 0x0400059D RID: 1437
	public static UIElement SelectCharLegacy = new UIElement
	{
		_path = "Legacy/UI_SelectChar"
	};

	// Token: 0x0400059E RID: 1438
	public static UIElement SelectChar = new UIElement
	{
		_path = "Select/ViewSelectCharacter"
	};

	// Token: 0x0400059F RID: 1439
	public static UIElement CombatBegin = new UIElement
	{
		_path = "Combat/ViewCombatBegin",
		CanQuickHide = false
	};

	// Token: 0x040005A0 RID: 1440
	public static UIElement AdventurePrepareRemake = new UIElement
	{
		_path = "Adventure/ViewAdventurePrepare"
	};

	// Token: 0x040005A1 RID: 1441
	public static UIElement EventWindow = new UIElement
	{
		_path = "EventWindow/ViewEventWindow",
		CanQuickHide = false
	};

	// Token: 0x040005A2 RID: 1442
	public static UIElement MiniScene = new UIElement
	{
		_path = "MiniScene/ViewMiniScene",
		CanQuickHide = false
	};

	// Token: 0x040005A3 RID: 1443
	public static UIElement BlockInteract = new UIElement
	{
		_path = "Legacy/UI_BlockInteract",
		CanQuickHide = false
	};

	// Token: 0x040005A4 RID: 1444
	public static UIElement VillagerWork = new UIElement
	{
		_path = "Villager/ViewVillagerWork"
	};

	// Token: 0x040005A5 RID: 1445
	public static UIElement WorldState = new UIElement
	{
		_path = "Bottom/ViewWorldState",
		CanQuickHide = false
	};

	// Token: 0x040005A6 RID: 1446
	public static UIElement SystemOption = new UIElement
	{
		_path = "SystemOption/ViewSystemOption"
	};

	// Token: 0x040005A7 RID: 1447
	public static UIElement SystemSetting = new UIElement
	{
		_path = "SystemSetting/ViewSystemSetting"
	};

	// Token: 0x040005A8 RID: 1448
	public static UIElement UpdateLog = new UIElement
	{
		_path = "UpdateLog/ViewUpdateLog"
	};

	// Token: 0x040005A9 RID: 1449
	public static UIElement Legacy = new UIElement
	{
		_path = "LegacyPassing/ViewLegacy"
	};

	// Token: 0x040005AA RID: 1450
	public static UIElement SelectLegacyRewardGroup = new UIElement
	{
		_path = "LegacyPassing/ViewSelectLegacyRewardGroup"
	};

	// Token: 0x040005AB RID: 1451
	public static UIElement DisplayConfigLegacy = new UIElement
	{
		_path = "LegacyPassing/ViewDisplayConfigLegacy"
	};

	// Token: 0x040005AC RID: 1452
	public static UIElement SelectRandomLegacyReward = new UIElement
	{
		_path = "LegacyPassing/ViewSelectRandomLegacyReward"
	};

	// Token: 0x040005AD RID: 1453
	public static UIElement LegacyActivate = new UIElement
	{
		_path = "LegacyPassing/ViewLegacyActivate",
		CanQuickHide = false
	};

	// Token: 0x040005AE RID: 1454
	public static UIElement CheckInscription = new UIElement
	{
		_path = "Main/Inscription/ViewCheckInscription"
	};

	// Token: 0x040005AF RID: 1455
	public static UIElement SelectInscriptionForPlay = new UIElement
	{
		_path = "Main/Inscription/ViewSelectInscriptionForPlay"
	};

	// Token: 0x040005B0 RID: 1456
	public static UIElement SelectInscriptionForEvolution = new UIElement
	{
		_path = "Main/Inscription/ViewSelectInscriptionForEvolution"
	};

	// Token: 0x040005B1 RID: 1457
	public static UIElement TutorialChaptersMenu = new UIElement
	{
		_path = "Tutorial/ViewTutorialChaptersMenu"
	};

	// Token: 0x040005B2 RID: 1458
	public static UIElement TutorialGuidingChapter = new UIElement
	{
		_path = "Tutorial/ViewGuidingChapter"
	};

	// Token: 0x040005B3 RID: 1459
	public static UIElement TutorialGuidingChapterTip = new UIElement
	{
		_path = "Tutorial/ViewGuidingChapterTip",
		CanQuickHide = false
	};

	// Token: 0x040005B4 RID: 1460
	public static UIElement FullScreenMask = new UIElement
	{
		_path = "Legacy/UI_FullScreenMask",
		CanQuickHide = false
	};

	// Token: 0x040005B5 RID: 1461
	public static UIElement RevertArchive = new UIElement
	{
		_path = "RecordSelect/ViewRevertArchive"
	};

	// Token: 0x040005B6 RID: 1462
	public static UIElement CricketCollection = new UIElement
	{
		_path = "Building/ViewCricketCollection"
	};

	// Token: 0x040005B7 RID: 1463
	public static UIElement CricketPolymorphEffect = new UIElement
	{
		_path = "Cricket/ViewCricketPolymorphEffect"
	};

	// Token: 0x040005B8 RID: 1464
	public static UIElement NewShop = new UIElement
	{
		_path = "Exchange/ViewShop"
	};

	// Token: 0x040005B9 RID: 1465
	public static UIElement NewShopGift = new UIElement
	{
		_path = "Exchange/ViewShopGift"
	};

	// Token: 0x040005BA RID: 1466
	public static UIElement SettlementShop = new UIElement
	{
		_path = "Exchange/ViewSettlementShop"
	};

	// Token: 0x040005BB RID: 1467
	public static UIElement SettlementTreasuryReplenish = new UIElement
	{
		_path = "SettlementTreasury/ViewSettlementTreasuryReplenish"
	};

	// Token: 0x040005BC RID: 1468
	public static UIElement SettlementInformation = new UIElement
	{
		_path = "SettlementInformation/ViewSettlementInformation"
	};

	// Token: 0x040005BD RID: 1469
	public static UIElement Warehouse = new UIElement
	{
		_path = "Exchange/ViewWarehouse"
	};

	// Token: 0x040005BE RID: 1470
	public static UIElement AddCraftResource = new UIElement
	{
		_path = "Legacy/UI_AddCraftResource"
	};

	// Token: 0x040005BF RID: 1471
	public static UIElement TaiwuVillagers = new UIElement
	{
		_path = "ViewTaiwuVillagers"
	};

	// Token: 0x040005C0 RID: 1472
	public static UIElement TaiwuSelectVillagers = new UIElement
	{
		_path = "ViewTaiwuSelectVillagers"
	};

	// Token: 0x040005C1 RID: 1473
	public static UIElement TaiwuVillagerNeedItem = new UIElement
	{
		_path = "TaiwuVillagerNeed/ViewTaiwuVillagerNeedItem"
	};

	// Token: 0x040005C2 RID: 1474
	public static UIElement CombatSkillTree = new UIElement
	{
		_path = "ViewCombatSkillTree"
	};

	// Token: 0x040005C3 RID: 1475
	public static UIElement LifeSkillCombatOld = new UIElement
	{
		_path = "Legacy/LifeSkillCombat/UI_LifeSkillCombat2"
	};

	// Token: 0x040005C4 RID: 1476
	public static UIElement Debate = new UIElement
	{
		_path = "Debate/ViewDebate",
		CanQuickHide = false
	};

	// Token: 0x040005C5 RID: 1477
	public static UIElement DebateCardGroup = new UIElement
	{
		_path = "Debate/ViewDebateCardGroup"
	};

	// Token: 0x040005C6 RID: 1478
	public static UIElement BuildingOverview = new UIElement
	{
		_path = "Building/ViewBuildingOverview"
	};

	// Token: 0x040005C7 RID: 1479
	public static UIElement NumberSetter = new UIElement
	{
		_path = "Legacy/UI_NumberSetter",
		CanQuickHide = false
	};

	// Token: 0x040005C8 RID: 1480
	public static UIElement CharacterShave = new UIElement
	{
		_path = "ViewCharacterShave"
	};

	// Token: 0x040005C9 RID: 1481
	public static UIElement ReadingEvent = new UIElement
	{
		_path = "Main/Reading/ViewReadingEvent"
	};

	// Token: 0x040005CA RID: 1482
	public static UIElement MonthNotify = new UIElement
	{
		_path = "MonthNotify/ViewMonthNotify"
	};

	// Token: 0x040005CB RID: 1483
	public static UIElement MonthlyEvent = new UIElement
	{
		_path = "Legacy/UI_MonthlyEvent"
	};

	// Token: 0x040005CC RID: 1484
	public static UIElement Exchange = new UIElement
	{
		_path = "Exchange/ViewExchange"
	};

	// Token: 0x040005CD RID: 1485
	public static UIElement ExchangeResource = new UIElement
	{
		_path = "Legacy/PopUp/UI_ExchangeResource"
	};

	// Token: 0x040005CE RID: 1486
	public static UIElement TeaHorseCaravan = new UIElement
	{
		_path = "Building/ViewTeaHorseCaravan"
	};

	// Token: 0x040005CF RID: 1487
	public static UIElement CgPlayer = new UIElement
	{
		_path = "Legacy/UI_CgPlayer",
		CanQuickHide = false
	};

	// Token: 0x040005D0 RID: 1488
	public static UIElement SectStoryPopUpToggle = new UIElement
	{
		_path = "ViewSectStoryPopUpToggle"
	};

	// Token: 0x040005D1 RID: 1489
	public static UIElement InstantNotification = new UIElement
	{
		_path = "InstantNotification/ViewInstantNotification"
	};

	// Token: 0x040005D2 RID: 1490
	public static UIElement InstantNotificationEvent = new UIElement
	{
		_path = "Bottom/ViewInstantNotificationEvent",
		CanQuickHide = false
	};

	// Token: 0x040005D3 RID: 1491
	public static UIElement ModPanelOld = new UIElement
	{
		_path = "Legacy/Mod/UI_ModPanel"
	};

	// Token: 0x040005D4 RID: 1492
	public static UIElement ModInfo = new UIElement
	{
		_path = "Mod/ViewModInfo"
	};

	// Token: 0x040005D5 RID: 1493
	public static UIElement SaveChangedSettingDialog = new UIElement
	{
		_path = "Mod/ViewSaveChangedSettingDialog"
	};

	// Token: 0x040005D6 RID: 1494
	public static UIElement Mod = new UIElement
	{
		_path = "Mod/ViewMod"
	};

	// Token: 0x040005D7 RID: 1495
	public static UIElement Heal = new UIElement
	{
		_path = "Legacy/UI_Heal"
	};

	// Token: 0x040005D8 RID: 1496
	public static UIElement Encyclopedia = new UIElement
	{
		_path = "Encyclopedia/ViewEncyclopedia"
	};

	// Token: 0x040005D9 RID: 1497
	public static UIElement SelectRandomSuccessor = new UIElement
	{
		_path = "LegacyPassing/ViewSelectRandomSuccessor",
		CanQuickHide = false
	};

	// Token: 0x040005DA RID: 1498
	public static UIElement SelectItemInCombat = new UIElement
	{
		_path = "Legacy/UI_SelectItemInCombat"
	};

	// Token: 0x040005DB RID: 1499
	public static UIElement MakeOld = new UIElement
	{
		_path = "Legacy/Building/UI_Make"
	};

	// Token: 0x040005DC RID: 1500
	public static UIElement Make = new UIElement
	{
		_path = "Make/ViewMake"
	};

	// Token: 0x040005DD RID: 1501
	public static UIElement Craftsman = new UIElement
	{
		_path = "Make/ViewCraftsman"
	};

	// Token: 0x040005DE RID: 1502
	public static UIElement CraftsmanOld = new UIElement
	{
		_path = "Legacy/Building/UI_CraftsmanPanel"
	};

	// Token: 0x040005DF RID: 1503
	public static UIElement SelectProductType = new UIElement
	{
		_path = "Legacy/Building/UI_SelectProductType"
	};

	// Token: 0x040005E0 RID: 1504
	public static UIElement GameLineScroll = new UIElement
	{
		_path = "GameLineScroll/ViewGameLineScroll"
	};

	// Token: 0x040005E1 RID: 1505
	public static UIElement SetSelectCount = new UIElement
	{
		_path = "ViewSetSelectCount"
	};

	// Token: 0x040005E2 RID: 1506
	public static UIElement SetInnerRatio = new UIElement
	{
		_path = "ViewSetInnerRatio"
	};

	// Token: 0x040005E3 RID: 1507
	public static UIElement SamsaraPlatform = new UIElement
	{
		_path = "Building/ViewSamsaraPlatform"
	};

	// Token: 0x040005E4 RID: 1508
	public static UIElement SelectInformation = new UIElement
	{
		_path = "Select/ViewSelectInformation"
	};

	// Token: 0x040005E5 RID: 1509
	public static UIElement SelectSecretInformation = new UIElement
	{
		_path = "Select/ViewSelectSecret"
	};

	// Token: 0x040005E6 RID: 1510
	public static UIElement SelectInformationForShopping = new UIElement
	{
		_path = "Select/ViewPurchaseSecret"
	};

	// Token: 0x040005E7 RID: 1511
	public static UIElement BlackMask = new UIElement
	{
		_path = "Legacy/UI_BlackMask"
	};

	// Token: 0x040005E8 RID: 1512
	public static UIElement LifeSkillCombatPrepare = new UIElement
	{
		_path = "LifeSkillCombat/ViewLifeSkillCombatBegin",
		CanQuickHide = false
	};

	// Token: 0x040005E9 RID: 1513
	public static UIElement VillagerRole = new UIElement
	{
		_path = "VillagerRole/ViewVillagerRole"
	};

	// Token: 0x040005EA RID: 1514
	public static UIElement VillagerRoleSelectStorageType = new UIElement
	{
		_path = "Legacy/VillagerRole/UI_VillagerRoleSelectStorageType"
	};

	// Token: 0x040005EB RID: 1515
	public static UIElement VillagerSelectMerchantType = new UIElement
	{
		_path = "Legacy/VillagerRole/UI_VillagerSelectMerchantType"
	};

	// Token: 0x040005EC RID: 1516
	public static UIElement VillagerCraftInputMaterial = new UIElement
	{
		_path = "Legacy/VillagerRole/UI_VillagerCraftInputMaterial"
	};

	// Token: 0x040005ED RID: 1517
	public static UIElement XiangshuLevelChanged = new UIElement
	{
		_path = "World/ViewXiangshuLevelChanged"
	};

	// Token: 0x040005EE RID: 1518
	public static UIElement PopupMenu = new UIElement
	{
		_path = "ViewPopupMenu"
	};

	// Token: 0x040005EF RID: 1519
	public static UIElement ExchangeBook = new UIElement
	{
		_path = "Exchange/ViewExchangeBook"
	};

	// Token: 0x040005F0 RID: 1520
	public static UIElement SelectGrave = new UIElement
	{
		_path = "Legacy/UI_SelectGrave"
	};

	// Token: 0x040005F1 RID: 1521
	public static UIElement SucceedingSelect = new UIElement
	{
		_path = "Legacy/UI_SucceedingSelect"
	};

	// Token: 0x040005F2 RID: 1522
	public static UIElement CricketCombatResult = new UIElement
	{
		_path = "Cricket/ViewCricketCombatResult"
	};

	// Token: 0x040005F3 RID: 1523
	public static UIElement ItemMultiplyOperationOld = new UIElement
	{
		_path = "Legacy/UI_ItemMultiplyOperation"
	};

	// Token: 0x040005F4 RID: 1524
	public static UIElement ItemMultiplyOptionOld = new UIElement
	{
		_path = "Legacy/UI_ItemMultiplyOption"
	};

	// Token: 0x040005F5 RID: 1525
	public static UIElement ItemMultiplyOperation = new UIElement
	{
		_path = "Item/ViewItemMultiplyOperation"
	};

	// Token: 0x040005F6 RID: 1526
	public static UIElement ItemAutoOperation = new UIElement
	{
		_path = "Item/ViewItemAutoOperation"
	};

	// Token: 0x040005F7 RID: 1527
	public static UIElement DebateResult = new UIElement
	{
		_path = "Debate/ViewDebateResult"
	};

	// Token: 0x040005F8 RID: 1528
	public static UIElement AiForceSilenceDialog = new UIElement
	{
		_path = "LifeSkillCombat/ViewAiForceSilenceDialog"
	};

	// Token: 0x040005F9 RID: 1529
	public static UIElement LegendaryBook = new UIElement
	{
		_path = "LegendaryBook/ViewLegendaryBook"
	};

	// Token: 0x040005FA RID: 1530
	public static UIElement TaskPopPanel = new UIElement
	{
		_path = "TaskPanel/ViewTaskPopUpPanel"
	};

	// Token: 0x040005FB RID: 1531
	public static UIElement TaskPanelMain = new UIElement
	{
		_path = "Legacy/TaskPanel/UI_TaskPanelMain"
	};

	// Token: 0x040005FC RID: 1532
	public static UIElement TaskPanelMain2 = new UIElement
	{
		_path = "Bottom/TaskPanel/ViewTaskPanelMain2"
	};

	// Token: 0x040005FD RID: 1533
	public static UIElement TaskPanelTopGroup = new UIElement
	{
		_path = "TaskPanel/ViewTaskPanelTopGroup"
	};

	// Token: 0x040005FE RID: 1534
	public static UIElement ProfessionSkillConfirm = new UIElement
	{
		_path = "ViewConfirmProfessionSkillDialog"
	};

	// Token: 0x040005FF RID: 1535
	public static UIElement ResourceListCostConfirm = new UIElement
	{
		_path = "Legacy/Profession/UI_ResourceListCostConfirm"
	};

	// Token: 0x04000600 RID: 1536
	public static UIElement ProfessionSkillPreConfirm = new UIElement
	{
		_path = "Legacy/Profession/UI_ProfessionSkillPreConfirm"
	};

	// Token: 0x04000601 RID: 1537
	public static UIElement ProfessionMask = new UIElement
	{
		_path = "Profession/ViewProfessionMask",
		CanQuickHide = false
	};

	// Token: 0x04000602 RID: 1538
	public static UIElement Profession = new UIElement
	{
		_path = "Profession/ViewProfession"
	};

	// Token: 0x04000603 RID: 1539
	public static UIElement ProfessionSkillUnlocked = new UIElement
	{
		_path = "Legacy/Profession/UI_ProfessionSkillUnlocked",
		CanQuickHide = false
	};

	// Token: 0x04000604 RID: 1540
	public static UIElement TeachCombatSkillResultConfirm = new UIElement
	{
		_path = "Profession/ViewTeachCombatSkillResultConfirm"
	};

	// Token: 0x04000605 RID: 1541
	public static UIElement TeachLifeSkillResultConfirm = new UIElement
	{
		_path = "Profession/ViewTeachLifeSkillResultConfirm"
	};

	// Token: 0x04000606 RID: 1542
	public static UIElement ProfessionPreview = new UIElement
	{
		_path = "Profession/ViewProfessionPreview"
	};

	// Token: 0x04000607 RID: 1543
	public static UIElement SetEquipmentEffect = new UIElement
	{
		_path = "Legacy/UI_SetEquipmentEffect"
	};

	// Token: 0x04000608 RID: 1544
	public static UIElement UltimateSelectCharacter = new UIElement
	{
		_path = "Legacy/UI_UltimateSelectCharacter"
	};

	// Token: 0x04000609 RID: 1545
	public static UIElement AreaStoryScroll = new UIElement
	{
		_path = "GameLineScroll/ViewAreaStoryScroll"
	};

	// Token: 0x0400060A RID: 1546
	public static UIElement MapInfoOption = new UIElement
	{
		_path = "Legacy/UI_MapInfoOption"
	};

	// Token: 0x0400060B RID: 1547
	public static UIElement AudioSetting = new UIElement
	{
		_path = "Legacy/UI_AudioSetting"
	};

	// Token: 0x0400060C RID: 1548
	public static UIElement SelectKongsangArea = new UIElement
	{
		_path = "World/ViewKongsangSelectMapArea"
	};

	// Token: 0x0400060D RID: 1549
	public static UIElement CharacterPracticeConfirm = new UIElement
	{
		_path = "Legacy/UI_CharacterPracticeConfirm"
	};

	// Token: 0x0400060E RID: 1550
	public static UIElement UsingMedicineItem = new UIElement
	{
		_path = "ViewUsingMedicine"
	};

	// Token: 0x0400060F RID: 1551
	public static UIElement ModifyBook = new UIElement
	{
		_path = "SectInteract/Wudang/ViewWudangModifyBook"
	};

	// Token: 0x04000610 RID: 1552
	public static UIElement ModifyBookConfirm = new UIElement
	{
		_path = "Legacy/UI_ModifyBookConfirm"
	};

	// Token: 0x04000611 RID: 1553
	public static UIElement UpgradeTeammateCommand = new UIElement
	{
		_path = "SectInteract/Shixiang/ViewShixiangUpgradeTeammateCommand"
	};

	// Token: 0x04000612 RID: 1554
	public static UIElement ChangeTeammateCommand = new UIElement
	{
		_path = "ViewChangeTeammateCommand"
	};

	// Token: 0x04000613 RID: 1555
	public static UIElement DefendHeavenlyTree = new UIElement
	{
		_path = "SectInteract/Wudang/ViewDefendHeavenlyTree",
		IsFullScreen = true
	};

	// Token: 0x04000614 RID: 1556
	public static UIElement DefendHeavenlyTreeFeed = new UIElement
	{
		_path = "SectInteract/Wudang/ViewDefendHeavenlyTreeFeed"
	};

	// Token: 0x04000615 RID: 1557
	public static UIElement FiveElementsPanel = new UIElement
	{
		_path = "CharacterMenu/ViewFiveElementsPanel"
	};

	// Token: 0x04000616 RID: 1558
	public static UIElement ExtraordinaryCricket = new UIElement
	{
		_path = "Legacy/UI_ExtraordinaryCricket"
	};

	// Token: 0x04000617 RID: 1559
	public static UIElement MusicPlayer = new UIElement
	{
		_path = "MusicPlayer/ViewMusicPlayer"
	};

	// Token: 0x04000618 RID: 1560
	public static UIElement CombatSkillSpecialBreak = new UIElement
	{
		_path = "SectInteract/Emei/ViewEmeiCombatSkillSpecialBreak"
	};

	// Token: 0x04000619 RID: 1561
	public static UIElement CombatSkillSpecialBreakMultiplySelect = new UIElement
	{
		_path = "Legacy/CombatSkillSpecialBreak/UI_CombatSkillSpecialBreakMultiplySelect"
	};

	// Token: 0x0400061A RID: 1562
	public static UIElement EventLog = new UIElement
	{
		_path = "EventWindow/ViewEventLog"
	};

	// Token: 0x0400061B RID: 1563
	public static UIElement CombatSkillPanel = new UIElement
	{
		_path = "ViewCombatSkillPanel"
	};

	// Token: 0x0400061C RID: 1564
	public static UIElement BuildingJiaoPool = new UIElement
	{
		_path = "Legacy/UI_BuildingJiaoPool"
	};

	// Token: 0x0400061D RID: 1565
	public static UIElement JiaoPoolSelectItem = new UIElement
	{
		_path = "Legacy/UI_JiaoPoolSelectItem"
	};

	// Token: 0x0400061E RID: 1566
	public static UIElement JiaoChangeLoong = new UIElement
	{
		_path = "Legacy/UI_JiaoChangeLoong"
	};

	// Token: 0x0400061F RID: 1567
	public static UIElement JiaoPoolRecord = new UIElement
	{
		_path = "Legacy/UI_JiaoPoolRecord",
		CanQuickHide = false
	};

	// Token: 0x04000620 RID: 1568
	public static UIElement LoongDebuffAnimation = new UIElement
	{
		_path = "Legacy/UI_LoongDebuffAnimation"
	};

	// Token: 0x04000621 RID: 1569
	public static UIElement RecordContent = new UIElement
	{
		_path = "RecordSelect/ViewRecordContent"
	};

	// Token: 0x04000622 RID: 1570
	public static UIElement MakeWugKing = new UIElement
	{
		_path = "MakeWugKing/ViewMakeWugKing"
	};

	// Token: 0x04000623 RID: 1571
	public static UIElement MakeWugKingList = new UIElement
	{
		_path = "MakeWugKing/ViewMakeWugKingList"
	};

	// Token: 0x04000624 RID: 1572
	public static UIElement SectShaolinDemonSlayer = new UIElement
	{
		_path = "SectInteract/Shaolin/ViewShaolinDemonSlayer"
	};

	// Token: 0x04000625 RID: 1573
	public static UIElement SwapSoul = new UIElement
	{
		_path = "SectInteract/Jingang/ViewSwapSoul"
	};

	// Token: 0x04000626 RID: 1574
	public static UIElement EditAvatar = new UIElement
	{
		_path = "SectInteract/Jingang/ViewSwapSoulEditAvatar"
	};

	// Token: 0x04000627 RID: 1575
	public static UIElement SelectFeature = new UIElement
	{
		_path = "SectInteract/Jingang/ViewSwapSoulSelectFeature"
	};

	// Token: 0x04000628 RID: 1576
	public static UIElement RecruitPeopleOverview = new UIElement
	{
		_path = "Building/ViewRecruitPeopleOverview"
	};

	// Token: 0x04000629 RID: 1577
	public static UIElement ShopConfirm = new UIElement
	{
		_path = "Legacy/Shop/UI_ShopConfirm"
	};

	// Token: 0x0400062A RID: 1578
	public static UIElement InvestCaravanConfirm = new UIElement
	{
		_path = "Legacy/Shop/UI_InvestCaravanConfirm"
	};

	// Token: 0x0400062B RID: 1579
	public static UIElement ProtectCaravanConfirm = new UIElement
	{
		_path = "Legacy/Shop/UI_ProtectCaravanConfirm"
	};

	// Token: 0x0400062C RID: 1580
	public static UIElement LegendaryBookKeeping = new UIElement
	{
		_path = "SectInteract/Ranshan/ViewRanshanThreeCorpses"
	};

	// Token: 0x0400062D RID: 1581
	public static UIElement GiveUpLegendaryBook = new UIElement
	{
		_path = "Legacy/LegendaryBookKeeping/UI_GiveUpLegendaryBook"
	};

	// Token: 0x0400062E RID: 1582
	public static UIElement SamsaraPlatformRecords = new UIElement
	{
		_path = "Legacy/Building/UI_SamsaraPlatformRecords"
	};

	// Token: 0x0400062F RID: 1583
	public static UIElement LifeLink = new UIElement
	{
		_path = "SectInteract/Baihua/ViewBaihuaLifeLink"
	};

	// Token: 0x04000630 RID: 1584
	public static UIElement CreateMirrorCharacter = new UIElement
	{
		_path = "SectInteract/Xuannv/ViewXuannvCreateMirrorCharacter"
	};

	// Token: 0x04000631 RID: 1585
	public static UIElement SettlementTreasuryRecords = new UIElement
	{
		_path = "Record/ViewSettlementTreasuryRecords"
	};

	// Token: 0x04000632 RID: 1586
	public static UIElement Looping = new UIElement
	{
		_path = "Looping/ViewLooping"
	};

	// Token: 0x04000633 RID: 1587
	public static UIElement LoopingEvent = new UIElement
	{
		_path = "Legacy/Looping/UI_LoopingEvent"
	};

	// Token: 0x04000634 RID: 1588
	public static UIElement Following = new UIElement
	{
		_path = "Following/ViewFollowing"
	};

	// Token: 0x04000635 RID: 1589
	public static UIElement MerchantInfo = new UIElement
	{
		_path = "ViewMerchantInfo"
	};

	// Token: 0x04000636 RID: 1590
	public static UIElement MerchantCaravanDetail = new UIElement
	{
		_path = "World/ViewMerchantCaravanDetail"
	};

	// Token: 0x04000637 RID: 1591
	public static UIElement SettlementPrison = new UIElement
	{
		_path = "SettlementPrison/ViewSettlementPrison"
	};

	// Token: 0x04000638 RID: 1592
	public static UIElement SettlementBounty = new UIElement
	{
		_path = "SectBuilding/ViewSettlementBounty"
	};

	// Token: 0x04000639 RID: 1593
	public static UIElement SettlementPrisonRecords = new UIElement
	{
		_path = "Record/ViewSettlementPrisonRecords"
	};

	// Token: 0x0400063A RID: 1594
	public static UIElement TaiwuVillageStoragesRecord = new UIElement
	{
		_path = "Record/ViewTaiwuStorageRecords"
	};

	// Token: 0x0400063B RID: 1595
	public static UIElement SectLaw = new UIElement
	{
		_path = "SettlementPrison/ViewSectLaw"
	};

	// Token: 0x0400063C RID: 1596
	public static UIElement PunishmentSeverity = new UIElement
	{
		_path = "Legacy/SettlementPrison/UI_PunishmentSeverity"
	};

	// Token: 0x0400063D RID: 1597
	public static UIElement ModDependenceChangeList = new UIElement
	{
		_path = "Legacy/Mod/UI_ModDependenceChangeList"
	};

	// Token: 0x0400063E RID: 1598
	public static UIElement MakeMedicine = new UIElement
	{
		_path = "Legacy/Profession/UI_MakeMedicine"
	};

	// Token: 0x0400063F RID: 1599
	public static UIElement ProfessionTravelerStation = new UIElement
	{
		_path = "Profession/ViewProfessionTravelerStation"
	};

	// Token: 0x04000640 RID: 1600
	public static UIElement GearMate = new UIElement
	{
		_path = "SectInteract/Zhujian/ViewZhujianGearMate"
	};

	// Token: 0x04000641 RID: 1601
	public static UIElement CatchThief = new UIElement
	{
		_path = "Legacy/UI_CatchThief",
		CanQuickHide = false
	};

	// Token: 0x04000642 RID: 1602
	public static UIElement SectInteractTutorial = new UIElement
	{
		_path = "SectInteract/ViewSectInteractTutorial"
	};

	// Token: 0x04000643 RID: 1603
	public static UIElement MouseTipSingleDesc = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSingleDesc"
	};

	// Token: 0x04000644 RID: 1604
	public static UIElement MouseTipSimple = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSimple"
	};

	// Token: 0x04000645 RID: 1605
	public static UIElement MouseTipSimpleWithHotkeyDisplay = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSimpleWithHotkeyDisplay"
	};

	// Token: 0x04000646 RID: 1606
	public static UIElement MouseTipCombatSkill = new UIElement
	{
		_path = "Tooltip/TooltipCombatSkill",
		ForceListenCommand = true
	};

	// Token: 0x04000647 RID: 1607
	public static UIElement MouseTipWeapon = new UIElement
	{
		_path = "Tooltip/Item/TooltipWeapon",
		ForceListenCommand = true
	};

	// Token: 0x04000648 RID: 1608
	public static UIElement MouseTipBook = new UIElement
	{
		_path = "Tooltip/Item/TooltipBook",
		ForceListenCommand = true
	};

	// Token: 0x04000649 RID: 1609
	public static UIElement MouseTipCraftTool = new UIElement
	{
		_path = "Tooltip/Item/TooltipCraftTool",
		ForceListenCommand = true
	};

	// Token: 0x0400064A RID: 1610
	public static UIElement MouseTipMaterial = new UIElement
	{
		_path = "Tooltip/Item/TooltipMaterial",
		ForceListenCommand = true
	};

	// Token: 0x0400064B RID: 1611
	public static UIElement PermanentTips = new UIElement
	{
		ForceListenCommand = true,
		_path = "Legacy/UI_PermanentTips"
	};

	// Token: 0x0400064C RID: 1612
	public static UIElement MouseTipCricket = new UIElement
	{
		_path = "Tooltip/Item/TooltipCricket",
		ForceListenCommand = true
	};

	// Token: 0x0400064D RID: 1613
	public static UIElement MouseTipArmor = new UIElement
	{
		_path = "Tooltip/Item/TooltipArmor",
		ForceListenCommand = true
	};

	// Token: 0x0400064E RID: 1614
	public static UIElement MouseTipCarrier = new UIElement
	{
		_path = "Tooltip/Item/TooltipCarrier",
		ForceListenCommand = true
	};

	// Token: 0x0400064F RID: 1615
	public static UIElement MouseTipClothing = new UIElement
	{
		_path = "Tooltip/Item/TooltipClothing",
		ForceListenCommand = true
	};

	// Token: 0x04000650 RID: 1616
	public static UIElement MouseTipFood = new UIElement
	{
		_path = "Tooltip/Item/TooltipFood",
		ForceListenCommand = true
	};

	// Token: 0x04000651 RID: 1617
	public static UIElement MouseTipMedicine = new UIElement
	{
		_path = "Tooltip/Item/TooltipMedicine",
		ForceListenCommand = true
	};

	// Token: 0x04000652 RID: 1618
	public static UIElement MouseTipMisc = new UIElement
	{
		_path = "Tooltip/Item/TooltipMisc",
		ForceListenCommand = true
	};

	// Token: 0x04000653 RID: 1619
	public static UIElement MouseTipTeaWine = new UIElement
	{
		_path = "Tooltip/Item/TooltipTeaWine",
		ForceListenCommand = true
	};

	// Token: 0x04000654 RID: 1620
	public static UIElement MouseTipAccessory = new UIElement
	{
		_path = "Tooltip/Item/TooltipAccessory",
		ForceListenCommand = true
	};

	// Token: 0x04000655 RID: 1621
	public static UIElement MouseTipLegendaryBook = new UIElement
	{
		_path = "Tooltip/Item/TooltipLegendaryBook",
		ForceListenCommand = true
	};

	// Token: 0x04000656 RID: 1622
	public static UIElement MouseTipActiveLoopCost = new UIElement
	{
		_path = "Tooltip/Item/TooltipActiveLoopCost",
		ForceListenCommand = true
	};

	// Token: 0x04000657 RID: 1623
	public static UIElement MouseTipActiveReadCost = new UIElement
	{
		_path = "Tooltip/Item/TooltipActiveReadCost",
		ForceListenCommand = true
	};

	// Token: 0x04000658 RID: 1624
	public static UIElement TooltipChicken = new UIElement
	{
		_path = "Tooltip/TooltipChicken",
		ForceListenCommand = true
	};

	// Token: 0x04000659 RID: 1625
	public static UIElement MouseTipLifeRecords = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeRecords"
	};

	// Token: 0x0400065A RID: 1626
	public static UIElement MouseTipCharacter = new UIElement
	{
		_path = "MouseTips/MouseTipCharacter"
	};

	// Token: 0x0400065B RID: 1627
	public static UIElement MouseTipResource = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipResource"
	};

	// Token: 0x0400065C RID: 1628
	public static UIElement MouseTipResourceHolder = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipResourceHolder"
	};

	// Token: 0x0400065D RID: 1629
	public static UIElement MouseTipEatingItems = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipEatingItems"
	};

	// Token: 0x0400065E RID: 1630
	public static UIElement MouseTipMapBlock = new UIElement
	{
		_path = "MouseTips/MouseTipMapBlock"
	};

	// Token: 0x0400065F RID: 1631
	public static UIElement MouseTipFeature = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipFeature"
	};

	// Token: 0x04000660 RID: 1632
	public static UIElement MouseTipMartialArtTournament = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipMartialArtTournament"
	};

	// Token: 0x04000661 RID: 1633
	public static UIElement MouseTipSimpleWide = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSimpleWide"
	};

	// Token: 0x04000662 RID: 1634
	public static UIElement MouseTipMakeItem = new UIElement
	{
		_path = "MouseTips/MouseTipMakeItem"
	};

	// Token: 0x04000663 RID: 1635
	public static UIElement MouseTipInnateFiveElements = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipInnateFiveElements"
	};

	// Token: 0x04000664 RID: 1636
	public static UIElement MouseTipDisassembleItem = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipDisassembleItem"
	};

	// Token: 0x04000665 RID: 1637
	public static UIElement MouseTipRepairItem = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipRepairItem"
	};

	// Token: 0x04000666 RID: 1638
	public static UIElement MouseTipReading = new UIElement
	{
		_path = "MouseTips/MouseTipReading"
	};

	// Token: 0x04000667 RID: 1639
	public static UIElement MouseTipSecretInformation = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSecretInformation"
	};

	// Token: 0x04000668 RID: 1640
	public static UIElement MouseTipLifeCombatSkillValue = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeCombatSkillValue"
	};

	// Token: 0x04000669 RID: 1641
	public static UIElement MouseTipBuildingShowItem = new UIElement
	{
		_path = "MouseTips/MouseTipBuildingShowItem"
	};

	// Token: 0x0400066A RID: 1642
	public static UIElement MouseTipBuildingShowRecruitPeople = new UIElement
	{
		_path = "MouseTips/MouseTipBuildingShowRecruitPeople"
	};

	// Token: 0x0400066B RID: 1643
	public static UIElement MouseTipSecretInformationBroadcastNotify = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSecretInformationBroadcastNotifyTips"
	};

	// Token: 0x0400066C RID: 1644
	public static UIElement MouseTipLegendaryBookBonus = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLegendaryBookBonus"
	};

	// Token: 0x0400066D RID: 1645
	public static UIElement EquipCompareTips = new UIElement
	{
		_path = "Legacy/UI_EquipCompareTips",
		CanQuickHide = false
	};

	// Token: 0x0400066E RID: 1646
	public static UIElement MouseTipProfessionSkill = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipProfessionSkill"
	};

	// Token: 0x0400066F RID: 1647
	public static UIElement MouseTipGearMateUpgradeAttribute = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipGearMateUpgradeAttribute"
	};

	// Token: 0x04000670 RID: 1648
	public static UIElement MouseTipGearMateUpgradeFeature = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipGearMateUpgradeFeature"
	};

	// Token: 0x04000671 RID: 1649
	public static UIElement MouseTipProfession = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipProfession"
	};

	// Token: 0x04000672 RID: 1650
	public static UIElement MouseTipAdventureNode = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipAdventureNode"
	};

	// Token: 0x04000673 RID: 1651
	public static UIElement MouseTipInjury = new UIElement
	{
		_path = "MouseTips/MouseTipInjury",
		CanQuickHide = false
	};

	// Token: 0x04000674 RID: 1652
	public static UIElement MouseTipCombatInjuryChange = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatInjuryChange",
		CanQuickHide = false
	};

	// Token: 0x04000675 RID: 1653
	public static UIElement MouseTipMapArea = new UIElement
	{
		_path = "MouseTips/MouseTipMapArea",
		CanQuickHide = false
	};

	// Token: 0x04000676 RID: 1654
	public static UIElement MouseTipVillagerAssign = new UIElement
	{
		_path = "MouseTips/MouseTipVillagerAssign",
		CanQuickHide = false
	};

	// Token: 0x04000677 RID: 1655
	public static UIElement MouseTipAttachedPoison = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipAttachedPoison",
		CanQuickHide = false
	};

	// Token: 0x04000678 RID: 1656
	public static UIElement MouseTipMixPoison = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipMixPoison",
		CanQuickHide = false
	};

	// Token: 0x04000679 RID: 1657
	public static UIElement MouseTipAdventure = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipAdventure",
		CanQuickHide = false
	};

	// Token: 0x0400067A RID: 1658
	public static UIElement MouseTipCharacterPoison = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCharacterPoison",
		CanQuickHide = false
	};

	// Token: 0x0400067B RID: 1659
	public static UIElement MouseTipLifeSkillValue = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillValue"
	};

	// Token: 0x0400067C RID: 1660
	public static UIElement MouseTipCombatSkillValue = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatSkillValue"
	};

	// Token: 0x0400067D RID: 1661
	public static UIElement MouseTipCombatSkillPractice = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatSkillPractice"
	};

	// Token: 0x0400067E RID: 1662
	public static UIElement MouseTipBodyPart = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipBodyPart"
	};

	// Token: 0x0400067F RID: 1663
	public static UIElement MouseTipCombatSkillBanReason = new UIElement
	{
		_path = "MouseTips/MouseTipCombatSkillBanReason"
	};

	// Token: 0x04000680 RID: 1664
	public static UIElement MouseTipFold = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipFold"
	};

	// Token: 0x04000681 RID: 1665
	public static UIElement MouseTipMonthNotify = new UIElement
	{
		_path = "MouseTips/MouseTipMonthNotify"
	};

	// Token: 0x04000682 RID: 1666
	public static UIElement CombatSkillBreakout = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatSkillBreakout"
	};

	// Token: 0x04000683 RID: 1667
	public static UIElement MouseTipCombatSkillBreakInfo = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatSkillBreakInfo"
	};

	// Token: 0x04000684 RID: 1668
	public static UIElement MouseTipCombatSkillBonus = new UIElement
	{
		_path = "MouseTips/MouseTipCombatSkillBonus"
	};

	// Token: 0x04000685 RID: 1669
	public static UIElement MouseTipCombatSkillOneBonus = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatSkillOneBonus"
	};

	// Token: 0x04000686 RID: 1670
	public static UIElement MouseTipFlaw = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipFlaw"
	};

	// Token: 0x04000687 RID: 1671
	public static UIElement MouseTipCombatChangeTrick = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatChangeTrick"
	};

	// Token: 0x04000688 RID: 1672
	public static UIElement MouseTipAdvance = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipAdvance"
	};

	// Token: 0x04000689 RID: 1673
	public static UIElement MouseTipTrickType = new UIElement
	{
		_path = "MouseTips/MouseTipTrickType"
	};

	// Token: 0x0400068A RID: 1674
	public static UIElement MouseTipUpgradeTeammateCommand = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipUpgradeTeammateCommand"
	};

	// Token: 0x0400068B RID: 1675
	public static UIElement MouseTipFiveElements = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipFiveElements"
	};

	// Token: 0x0400068C RID: 1676
	public static UIElement MouseTipNeiliAllocation = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipNeiliAllocation"
	};

	// Token: 0x0400068D RID: 1677
	public static UIElement MouseTipMusic = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipMusic"
	};

	// Token: 0x0400068E RID: 1678
	public static UIElement MouseTipReadingEvent = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipReadingEvent"
	};

	// Token: 0x0400068F RID: 1679
	public static UIElement EventConfirm = new UIElement
	{
		_path = "Legacy/UI_EventConfirm",
		CanQuickHide = false
	};

	// Token: 0x04000690 RID: 1680
	public static UIElement MouseTipLegacy = new UIElement
	{
		_path = "MouseTips/MouseTipLegacy"
	};

	// Token: 0x04000691 RID: 1681
	public static UIElement MouseTipLegacyLevel = new UIElement
	{
		_path = "MouseTips/MouseTipLegacyLevel"
	};

	// Token: 0x04000692 RID: 1682
	public static UIElement MouseTipFuyu = new UIElement
	{
		_path = "MouseTips/MouseTipFuyu",
		ForceListenCommand = true
	};

	// Token: 0x04000693 RID: 1683
	public static UIElement MouseTipDynamicCondition = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipDynamicCondition"
	};

	// Token: 0x04000694 RID: 1684
	public static UIElement MouseTipJiao = new UIElement
	{
		_path = "Tooltip/TooltipJiao",
		ForceListenCommand = true
	};

	// Token: 0x04000695 RID: 1685
	public static UIElement MouseTipJiaoEgg = new UIElement
	{
		_path = "Tooltip/TooltipJiaoEgg",
		ForceListenCommand = true
	};

	// Token: 0x04000696 RID: 1686
	public static UIElement MouseTipLoongDebuff = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLoongDebuff"
	};

	// Token: 0x04000697 RID: 1687
	public static UIElement MouseTipJiaoNurturance = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipJiaoNurturance"
	};

	// Token: 0x04000698 RID: 1688
	public static UIElement MouseTipCombatSkillBuff = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatSkillBuff"
	};

	// Token: 0x04000699 RID: 1689
	public static UIElement MouseTipGeneralLines = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipGeneralLines"
	};

	// Token: 0x0400069A RID: 1690
	public static UIElement ToolTipCommon = new UIElement
	{
		_path = "Tooltip/ToolTipCommon"
	};

	// Token: 0x0400069B RID: 1691
	public static UIElement MouseTipMixPoisonEffectSimple = new UIElement
	{
		_path = "MouseTips/MouseTipMixPoisonEffectSimple",
		ForceListenCommand = true
	};

	// Token: 0x0400069C RID: 1692
	public static UIElement MouseTipMixPoisonEffectDetailed = new UIElement
	{
		_path = "MouseTips/MouseTipMixPoisonEffectDetailed",
		ForceListenCommand = true
	};

	// Token: 0x0400069D RID: 1693
	public static UIElement MouseTipDisorderOfQi = new UIElement
	{
		_path = "Tooltip/TooltipDisorderOfQi"
	};

	// Token: 0x0400069E RID: 1694
	public static UIElement MouseTipMakeWugKing = new UIElement
	{
		_path = "MouseTips/MouseTipMakeWugKing"
	};

	// Token: 0x0400069F RID: 1695
	public static UIElement MouseTipEmptyContainer = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipEmptyContainer"
	};

	// Token: 0x040006A0 RID: 1696
	public static UIElement MouseTipCharacterComplete = new UIElement
	{
		_path = "MouseTips/MouseTipCharacterComplete"
	};

	// Token: 0x040006A1 RID: 1697
	public static UIElement MouseTipBuildingProduce = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipBuildingProduce"
	};

	// Token: 0x040006A2 RID: 1698
	public static UIElement MouseTipBuildingProduceCollectResource = new UIElement
	{
		_path = "MouseTips/MouseTipBuildingProduceCollectResource"
	};

	// Token: 0x040006A3 RID: 1699
	public static UIElement MouseTipMixPoisonEffectOutCombat = new UIElement
	{
		_path = "MouseTips/MouseTipMixPoisonEffectOutCombat",
		ForceListenCommand = true
	};

	// Token: 0x040006A4 RID: 1700
	public static UIElement MouseTipEatingWug = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipEatingWug"
	};

	// Token: 0x040006A5 RID: 1701
	public static UIElement MouseTipBuildingRequireCultureSafety = new UIElement
	{
		_path = "MouseTips/MouseTipBuildingRequireCultureSafety"
	};

	// Token: 0x040006A6 RID: 1702
	public static UIElement MouseTipChangeTrick = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipChangeTrick"
	};

	// Token: 0x040006A7 RID: 1703
	public static UIElement MouseTipCombatBannedList = new UIElement
	{
		_path = "MouseTips/MouseTipCombatBannedList"
	};

	// Token: 0x040006A8 RID: 1704
	public static UIElement MouseTipCombatBlockAttack = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatBlockAttack"
	};

	// Token: 0x040006A9 RID: 1705
	public static UIElement MouseTipEquipLoad = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipEquipLoad"
	};

	// Token: 0x040006AA RID: 1706
	public static UIElement MouseTipDefeatMark = new UIElement
	{
		_path = "MouseTips/MouseTipDefeatMark",
		ForceListenCommand = true
	};

	// Token: 0x040006AB RID: 1707
	public static UIElement MouseTipDamageValue = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipDamageValue"
	};

	// Token: 0x040006AC RID: 1708
	public static UIElement MouseTipDestiny = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipDestiny"
	};

	// Token: 0x040006AD RID: 1709
	public static UIElement MouseTipSettlementTreasury = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSettlementTreasury"
	};

	// Token: 0x040006AE RID: 1710
	public static UIElement MouseTipLoopingEvent = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLoopingEvent"
	};

	// Token: 0x040006AF RID: 1711
	public static UIElement MouseTipBuildingLevel = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipBuildingLevel"
	};

	// Token: 0x040006B0 RID: 1712
	public static UIElement MouseTipLifeLinkNeiliType = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeLinkNeiliType"
	};

	// Token: 0x040006B1 RID: 1713
	public static UIElement MouseTipActiveRead = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipActiveRead"
	};

	// Token: 0x040006B2 RID: 1714
	public static UIElement MouseTipActiveLoop = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipActiveLoop"
	};

	// Token: 0x040006B3 RID: 1715
	public static UIElement MouseTipReadProgress = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipReadProgress"
	};

	// Token: 0x040006B4 RID: 1716
	public static UIElement MouseTipLoopProgress = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLoopProgress"
	};

	// Token: 0x040006B5 RID: 1717
	public static UIElement MouseTipCharacterOnMapBlock = new UIElement
	{
		_path = "MouseTips/MouseTipCharacterOnMapBlock"
	};

	// Token: 0x040006B6 RID: 1718
	public static UIElement MouseTipTeammateCommand = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipTeammateCommand"
	};

	// Token: 0x040006B7 RID: 1719
	public static UIElement MouseTipTeammateCount = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipTeammateCount"
	};

	// Token: 0x040006B8 RID: 1720
	public static UIElement MouseTipFeatureMedalLegacy = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipFeatureMedal"
	};

	// Token: 0x040006B9 RID: 1721
	public static UIElement MouseTipFeatureMedal = new UIElement
	{
		_path = "MouseTips/MouseTipFeatureMedal"
	};

	// Token: 0x040006BA RID: 1722
	public static UIElement MouseTipFame = new UIElement
	{
		_path = "MouseTips/MouseTipFame"
	};

	// Token: 0x040006BB RID: 1723
	public static UIElement MouseTipHealthInfo = new UIElement
	{
		_path = "Tooltip/TooltipHealth"
	};

	// Token: 0x040006BC RID: 1724
	public static UIElement MouseTipFulongFlame = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipFulongFlame"
	};

	// Token: 0x040006BD RID: 1725
	public static UIElement MouseTipVillagerRoleAvailableCount = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipVillagerRoleAvailableCount"
	};

	// Token: 0x040006BE RID: 1726
	public static UIElement MouseTipVillagerRoleEffect = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipVillagerRoleEffect"
	};

	// Token: 0x040006BF RID: 1727
	public static UIElement MouseTipCombatRawCreate = new UIElement
	{
		_path = "MouseTips/MouseTipCombatRawCreate"
	};

	// Token: 0x040006C0 RID: 1728
	public static UIElement MouseTipCombatUnlockProgress = new UIElement
	{
		_path = "MouseTips/MouseTipCombatUnlockProgress"
	};

	// Token: 0x040006C1 RID: 1729
	public static UIElement MouseTipCombatWeaponUnlock = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCombatWeaponUnlock"
	};

	// Token: 0x040006C2 RID: 1730
	public static UIElement MouseTipCaravanOperation = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCaravanOperation"
	};

	// Token: 0x040006C3 RID: 1731
	public static UIElement MouseTipTaiwuWanted = new UIElement
	{
		_path = "MouseTips/MouseTipTaiwuWanted"
	};

	// Token: 0x040006C4 RID: 1732
	public static UIElement MouseTipCaravanPath = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipCaravanPath"
	};

	// Token: 0x040006C5 RID: 1733
	public static UIElement MouseTipCaravanPathDetail = new UIElement
	{
		_path = "MouseTips/MouseTipCaravanPathDetail"
	};

	// Token: 0x040006C6 RID: 1734
	public static UIElement MouseTipExtraProfessionSkill = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipExtraProfessionSkill"
	};

	// Token: 0x040006C7 RID: 1735
	public static UIElement MouseTipOrganization = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipOrganization"
	};

	// Token: 0x040006C8 RID: 1736
	public static UIElement MouseTipVillagerNeedItem = new UIElement
	{
		_path = "MouseTips/MouseTipVillagerNeedItem"
	};

	// Token: 0x040006C9 RID: 1737
	public static UIElement MouseTipNormalInformationType = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipNormalInformationType"
	};

	// Token: 0x040006CA RID: 1738
	public static UIElement MouseTipDemonSlayer = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipDemonSlayer"
	};

	// Token: 0x040006CB RID: 1739
	public static UIElement MouseTipSkillBreakBonus = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSkillBreakBonus"
	};

	// Token: 0x040006CC RID: 1740
	public static UIElement MouseTipSkillBreakNormalCell = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSkillBreakNormalCell"
	};

	// Token: 0x040006CD RID: 1741
	public static UIElement MouseTipSectStory = new UIElement
	{
		_path = "MouseTips/MouseTipSectStory"
	};

	// Token: 0x040006CE RID: 1742
	public static UIElement MouseTipThreeVitals = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipThreeVitals"
	};

	// Token: 0x040006CF RID: 1743
	public static UIElement MouseTipPrisonerResistance = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipPrisonerResistance"
	};

	// Token: 0x040006D0 RID: 1744
	public static UIElement MouseTipSpecialBuild = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSpecialBuild"
	};

	// Token: 0x040006D1 RID: 1745
	public static UIElement MouseTipBuildingFeast = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipBuildingFeast"
	};

	// Token: 0x040006D2 RID: 1746
	public static UIElement MouseTipLifeSkillDetailReadProgress = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillDetailReadProgress"
	};

	// Token: 0x040006D3 RID: 1747
	public static UIElement MouseTipLifeSkillDetailUnlockBuilding = new UIElement
	{
		_path = "MouseTips/MouseTipLifeSkillDetailUnlockBuilding"
	};

	// Token: 0x040006D4 RID: 1748
	public static UIElement MouseTipLifeSkillDetailUnlockInformation = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillDetailUnlockInformation"
	};

	// Token: 0x040006D5 RID: 1749
	public static UIElement MouseTipLifeSkillDetailUnlockStrategy = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillDetailUnlockStrategy"
	};

	// Token: 0x040006D6 RID: 1750
	public static UIElement MouseTipLifeSkillCombatCardType = new UIElement
	{
		_path = "MouseTips/MouseTipDebateCardType"
	};

	// Token: 0x040006D7 RID: 1751
	public static UIElement MouseTipLifeSkillCombatUnit = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillCombatUnit"
	};

	// Token: 0x040006D8 RID: 1752
	public static UIElement MouseTipLifeSkillCombatBlock = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillCombatBlock"
	};

	// Token: 0x040006D9 RID: 1753
	public static UIElement MouseTipLifeSkillCombatStrategy = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillCombatStrategy"
	};

	// Token: 0x040006DA RID: 1754
	public static UIElement MouseTipLifeSkillCombatStress = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillCombatStress"
	};

	// Token: 0x040006DB RID: 1755
	public static UIElement MouseTipLifeSkillCombatFirstMove = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillCombatFirstMove"
	};

	// Token: 0x040006DC RID: 1756
	public static UIElement MouseTipLifeSkillCombatLastMove = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipLifeSkillCombatLastMove"
	};

	// Token: 0x040006DD RID: 1757
	public static UIElement MouseTipLifeSkillCombatAudience = new UIElement
	{
		_path = "MouseTips/MouseTipDebateAudience"
	};

	// Token: 0x040006DE RID: 1758
	public static UIElement MouseTipSimpleList = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSimpleList"
	};

	// Token: 0x040006DF RID: 1759
	public static UIElement MouseTipMatchVillagerRole = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipMatchVillagerRole"
	};

	// Token: 0x040006E0 RID: 1760
	public static UIElement BuildingTeachBook = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipBuildingTeachBook"
	};

	// Token: 0x040006E1 RID: 1761
	public static UIElement MouseTipTaiwuVillageStele = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipTaiwuVillageStele"
	};

	// Token: 0x040006E2 RID: 1762
	public static UIElement WorkingStatus = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipWorkingStatus"
	};

	// Token: 0x040006E3 RID: 1763
	public static UIElement MouseTipPracticeNotice = new UIElement
	{
		_path = "MouseTips/MouseTipPracticeNotice"
	};

	// Token: 0x040006E4 RID: 1764
	public static UIElement LegendaryBookGiveUp = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipGiveUpLegendaryBook"
	};

	// Token: 0x040006E5 RID: 1765
	public static UIElement LegendaryBookCharacters = new UIElement
	{
		_path = "LegendaryBook/ViewLegendaryBookCharacters"
	};

	// Token: 0x040006E6 RID: 1766
	public static UIElement LegendaryBookUnlockBreakPlateConfirm = new UIElement
	{
		_path = "Legacy/LegendaryBook/UI_LegendaryBookUnlockBreakPlateConfirm"
	};

	// Token: 0x040006E7 RID: 1767
	public static UIElement MouseTipEncyclopedia = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipEncyclopedia"
	};

	// Token: 0x040006E8 RID: 1768
	public static UIElement EventEditor = new UIElement
	{
		_path = "Legacy/EditorViews/UI_EventEditor",
		CanQuickHide = false
	};

	// Token: 0x040006E9 RID: 1769
	public static UIElement AdventureEditorRemake = new UIElement
	{
		_path = "Legacy/EditorViews/UI_AdventureEditorRemake",
		CanQuickHide = false
	};

	// Token: 0x040006EA RID: 1770
	public static UIElement ItemTemplateSelector = new UIElement
	{
		_path = "Legacy/EditorViews/UI_ItemTemplateSelector"
	};

	// Token: 0x040006EB RID: 1771
	public static UIElement FileExplorer = new UIElement
	{
		_path = "Legacy/UI_FileExplorer"
	};

	// Token: 0x040006EC RID: 1772
	public static UIElement AiEditor = new UIElement
	{
		_path = "Legacy/EditorViews/UI_AiEditor",
		CanQuickHide = false
	};

	// Token: 0x040006ED RID: 1773
	public static UIElement AiParamInputField = new UIElement
	{
		_path = "Legacy/EditorViews/UI_AiParamInputField"
	};

	// Token: 0x040006EE RID: 1774
	public static UIElement AiShortNumberInputField = new UIElement
	{
		_path = "Legacy/EditorViews/UI_AiShortNumberInputField"
	};

	// Token: 0x040006EF RID: 1775
	public static UIElement AiSearchInputField = new UIElement
	{
		_path = "Legacy/EditorViews/UI_AiSearchInputField"
	};

	// Token: 0x040006F0 RID: 1776
	public static UIElement MonthlyNotificationSortingGroupSettings = new UIElement
	{
		_path = "MonthNotify/ViewMonthNotifySortingGroupSettings"
	};

	// Token: 0x040006F1 RID: 1777
	public static UIElement CricketBetting = new UIElement
	{
		_path = "Cricket/ViewCricketBetting"
	};

	// Token: 0x040006F2 RID: 1778
	public static UIElement BeggarSkill3 = new UIElement
	{
		_path = "Profession/ViewBeggarUltimate"
	};

	// Token: 0x040006F3 RID: 1779
	public static UIElement TravelingTaoistMonkSkill2 = new UIElement
	{
		_path = "Profession/ViewTravelingTaoistMonkSkill2"
	};

	// Token: 0x040006F4 RID: 1780
	public static UIElement EventStyleFeatureSelect = new UIElement
	{
		_path = "Profession/ViewEventStyleFeatureSelect",
		CanQuickHide = false
	};

	// Token: 0x040006F5 RID: 1781
	public static UIElement InteractCheckResult = new UIElement
	{
		_path = "InteractCheckResult/ViewInteractCheckResult"
	};

	// Token: 0x040006F6 RID: 1782
	public static UIElement MouseTipInteractCheckResult = new UIElement
	{
		_path = "MouseTips/MouseTipInteractCheckResult"
	};

	// Token: 0x040006F7 RID: 1783
	public static UIElement MouseTipExpCheck = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipExpCheck"
	};

	// Token: 0x040006F8 RID: 1784
	public static UIElement MouseTipRecordIncompatible = new UIElement
	{
		_path = "MouseTips/MouseTipRecordIncompatible"
	};

	// Token: 0x040006F9 RID: 1785
	public static UIElement YuanshanMiniGame = new UIElement
	{
		_path = "SectInteract/Yuanshan/ViewYuanshanMiniGame"
	};

	// Token: 0x040006FA RID: 1786
	public static UIElement CostResourceConfirm = new UIElement
	{
		_path = "Legacy/UI_CostResourceConfirm"
	};

	// Token: 0x040006FB RID: 1787
	public static UIElement ThreeVitals = new UIElement
	{
		_path = "SectInteract/Yuanshan/ViewYuanshanThreeVitals"
	};

	// Token: 0x040006FC RID: 1788
	public static UIElement SelectThreeVitalsTarget = new UIElement
	{
		_path = "Legacy/Sect/UI_SelectThreeVitalsTarget"
	};

	// Token: 0x040006FD RID: 1789
	public static UIElement JieQingTangram = new UIElement
	{
		_path = "SectInteract/Jieqing/ViewJieQingTangram"
	};

	// Token: 0x040006FE RID: 1790
	public static UIElement JieQingInteract = new UIElement
	{
		_path = "SectInteract/Jieqing/ViewJieqingInteract"
	};

	// Token: 0x040006FF RID: 1791
	public static UIElement BuildingFeastMenu = new UIElement
	{
		_path = "Building/ViewFeastMenu"
	};

	// Token: 0x04000700 RID: 1792
	public static UIElement UI_FullScreenCover = new UIElement
	{
		_path = "Legacy/UI_FullScreenCover"
	};

	// Token: 0x04000701 RID: 1793
	public static UIElement MouseTipSettlementTreasuryOrPrisonLayer = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipSettlementTreasuryOrPrisonLayer"
	};

	// Token: 0x04000702 RID: 1794
	public static UIElement CombatSkillQuickEdit = new UIElement
	{
		_path = "Legacy/CombatSkill/UI_CombatSkillQuickEdit"
	};

	// Token: 0x04000703 RID: 1795
	public static UIElement AdventureRemake = new UIElement
	{
		_path = "Adventure/ViewAdventureRemake"
	};

	// Token: 0x04000704 RID: 1796
	public static UIElement AchievementPopUp = new UIElement
	{
		_path = "Achievement/ViewAchievementPopup"
	};

	// Token: 0x04000705 RID: 1797
	public static UIElement Achievement = new UIElement
	{
		_path = "Achievement/ViewAchievement"
	};

	// Token: 0x04000706 RID: 1798
	public static UIElement AdventureMajorEventEditor = new UIElement
	{
		_path = "Legacy/EditorViews/UI_AdventureMajorEventEditor",
		CanQuickHide = false
	};

	// Token: 0x04000707 RID: 1799
	public static UIElement AdventureMajorEvent = new UIElement
	{
		_path = "Legacy/AdventureRemake/UI_AdventureMajorEvent",
		CanQuickHide = false
	};

	// Token: 0x04000708 RID: 1800
	public static UIElement MouseTipDeadCharacterComplete = new UIElement
	{
		_path = "MouseTips/MouseTipDeadCharacterComplete"
	};

	// Token: 0x04000709 RID: 1801
	public static UIElement WorldStatePanel = new UIElement
	{
		_path = "ViewWorldStatePanel"
	};

	// Token: 0x0400070A RID: 1802
	public static UIElement Jixi = new UIElement
	{
		_path = "SectInteract/Xuehou/ViewJixi"
	};

	// Token: 0x0400070B RID: 1803
	public static UIElement WugKingMultiplySelect = new UIElement
	{
		_path = "Legacy/SectMain/Kongsang/UI_WugKingMultiplySelect"
	};

	// Token: 0x0400070C RID: 1804
	public static UIElement KongsangSpecialInteract = new UIElement
	{
		_path = "SectInteract/Kongsang/ViewKongsangSpecialInteract"
	};

	// Token: 0x0400070D RID: 1805
	public static UIElement MouseTipTargetStarFortune = new UIElement
	{
		_path = "Legacy/MouseTip/UI_MouseTipTargetStarFortune"
	};

	// Token: 0x0400070E RID: 1806
	public static UIElement DevelopmentTeam = new UIElement
	{
		_path = "ViewDevelopmentTeam"
	};

	// Token: 0x0400070F RID: 1807
	public static UIElement MouseTipLegendaryBookPageItem = new UIElement
	{
		_path = "MouseTips/MouseTipLegendaryBookPageItem"
	};

	// Token: 0x04000710 RID: 1808
	public static UIElement Alertness = new UIElement
	{
		_path = "Alertness/ViewAlertness"
	};

	// Token: 0x04000711 RID: 1809
	public static UIElement MouseTipAlertness = new UIElement
	{
		_path = "MouseTips/MouseTipAlertness"
	};

	// Token: 0x04000712 RID: 1810
	public static UIElement MouseTipBuildingBlock = new UIElement
	{
		_path = "MouseTips/MouseTipBuildingBlock"
	};

	// Token: 0x04000713 RID: 1811
	public static UIElement ChoosyResource = new UIElement
	{
		_path = "ViewChoosyResource"
	};

	// Token: 0x04000714 RID: 1812
	public static UIElement ChoosyResourceConfirm = new UIElement
	{
		_path = "ViewChoosyResourceConfirm"
	};

	// Token: 0x04000715 RID: 1813
	public static UIElement MapElementDisplayRule = new UIElement
	{
		_path = "MapElement/ViewMapElementDisplayRule"
	};

	// Token: 0x04000716 RID: 1814
	public static UIElement FindMapBlock = new UIElement
	{
		_path = "Bottom/ViewFindMapBlock"
	};

	// Token: 0x04000717 RID: 1815
	public static UIElement SetCombatQuickUseItemSlot = new UIElement
	{
		_path = "Combat/ViewSetCombatQuickUseItemSlot"
	};

	// Token: 0x04000718 RID: 1816
	public static UIElement ChangeWeaponTrick = new UIElement
	{
		_path = "Profession/ViewChangeWeaponTrick"
	};

	// Token: 0x04000719 RID: 1817
	public static UIElement SelectFeatureStartGame = new UIElement
	{
		_path = "NewGame/ViewSelectFeatureNewGame"
	};

	// Token: 0x0400071A RID: 1818
	public static UIElement DLCIntroduce = new UIElement
	{
		_path = "DLCIntroduce/ViewDLCIntroduce"
	};

	// Token: 0x0400071B RID: 1819
	public static UIElement TaiwuLifeSummary = new UIElement
	{
		_path = "ViewTaiwuLifeSummary"
	};

	// Token: 0x0400071C RID: 1820
	public static UIElement EatDetail = new UIElement
	{
		_path = "CharacterMenu/ViewCharacterMenuEatDetail"
	};

	// Token: 0x0400071D RID: 1821
	public static UIElement GameLineAnim = new UIElement
	{
		_path = "GameLineAnim/ViewGameLineAnim"
	};

	// Token: 0x0400071E RID: 1822
	public static UIElement TestNewUI = new UIElement
	{
		_path = "Legacy/UI_TestNewUI"
	};

	// Token: 0x0400071F RID: 1823
	public static UIGroup StateMainWorld = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.WorldMap,
			UIElement.MiniScene,
			UIElement.ResourceBar,
			UIElement.MapBlockCharList,
			UIElement.Bottom,
			UIElement.WorldState,
			UIElement.InstantNotificationEvent,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2
		}),
		CanQuickHide = false
	};

	// Token: 0x04000720 RID: 1824
	public static UIGroup StateBuilding = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.BuildingArea,
			UIElement.ResourceBar,
			UIElement.Bottom,
			UIElement.WorldState,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2
		})
	};

	// Token: 0x04000721 RID: 1825
	public static UIGroup StateCombat = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.CombatBackground,
			UIElement.Combat
		}),
		CanQuickHide = false
	};

	// Token: 0x04000722 RID: 1826
	public static UIGroup StatePartWorldMap = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.PartWorld,
			UIElement.ResourceBar,
			UIElement.Bottom,
			UIElement.WorldState,
			UIElement.InstantNotificationEvent,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2
		})
	};

	// Token: 0x04000723 RID: 1827
	public static UIGroup TaiwuCrossArchive = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.WorldMap,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2
		}),
		CanQuickHide = false
	};

	// Token: 0x04000724 RID: 1828
	public static UIGroup PastTaiwuVillage = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.WorldMap,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2,
			UIElement.WorldState,
			UIElement.BottomPastTaiwuVillage
		}),
		CanQuickHide = false
	};

	// Token: 0x04000725 RID: 1829
	public static UIGroup StateAdventureRemake = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.AdventureRemake,
			UIElement.Bottom,
			UIElement.ResourceBar,
			UIElement.WorldState,
			UIElement.InstantNotificationEvent,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2
		}),
		CanQuickHide = false
	};

	// Token: 0x04000726 RID: 1830
	public static UIGroup StateAdventureRemakeSpecialBottom = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.AdventureRemake,
			UIElement.BottomPastTaiwuVillage,
			UIElement.ResourceBar,
			UIElement.WorldState,
			UIElement.InstantNotificationEvent,
			UIElement.TaskPanelMain,
			UIElement.TaskPanelMain2
		}),
		CanQuickHide = false
	};

	// Token: 0x04000727 RID: 1831
	public static UIGroup StateMajorEvent = new UIGroup
	{
		Elements = UIElement.Elements(new UIElement[]
		{
			UIElement.AdventureMajorEvent,
			UIElement.Bottom,
			UIElement.ResourceBar,
			UIElement.WorldState,
			UIElement.InstantNotificationEvent
		}),
		CanQuickHide = false
	};

	// Token: 0x04000728 RID: 1832
	public const string GmPrefabPath = "RemakeResources/Prefab/Views/Legacy/Test/UI_GMWindow";

	// Token: 0x04000729 RID: 1833
	public const string BuildingMakeEffectPath = "RemakeResources/Prefab/Views/Legacy/Building/Make/";

	// Token: 0x0400072A RID: 1834
	public static List<UIElement> ConstantElements = new List<UIElement>
	{
		UIElement.Advance,
		UIElement.BuildingArea,
		UIElement.PermanentTips,
		UIElement.FullScreenMask,
		UIElement.CharacterMenu,
		UIElement.CharacterMenuInfo,
		UIElement.CharacterMenuTeam,
		UIElement.CharacterMenuEquip,
		UIElement.CharacterMenuItems,
		UIElement.CharacterMenuLifeSkill,
		UIElement.CharacterMennAttaiment,
		UIElement.CharacterMenuPractice,
		UIElement.CharacterMenuRelationShip,
		UIElement.CharacterMenuLifeRecords,
		UIElement.CharacterMenuInformation,
		UIElement.CharacterMenuSecretInformation,
		UIElement.CharacterMenuEquipCombatSkill,
		UIElement.CombatBackground,
		UIElement.Combat,
		UIElement.SelectSkill,
		UIElement.SelectSecretInformation,
		UIElement.SelectInformation,
		UIElement.CharacterShave,
		UIElement.AdventureRemake,
		UIElement.CharacterMenuCombatSkill
	};

	// Token: 0x0400072B RID: 1835
	public static List<UIElement> PreloadElements = new List<UIElement>
	{
		UIElement.MouseTipCombatSkill,
		UIElement.SelectInformation,
		UIElement.SelectSecretInformation,
		UIElement.BuildingArea,
		UIElement.CharacterShave
	};
}
