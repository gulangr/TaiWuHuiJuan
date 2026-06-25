using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UI;
using GameData.GameDataBridge;
using UnityEngine;
using UnityEngine.U2D;

// Token: 0x020000B5 RID: 181
[RequireComponent(typeof(ConchShipGraphicRaycaster))]
public abstract class UIBase : Refers, IDisplay, IAsyncMethodRequestHandler, ILanguage
{
	// Token: 0x06000631 RID: 1585
	public abstract void OnInit(ArgumentBox argsBox);

	// Token: 0x06000632 RID: 1586 RVA: 0x00029890 File Offset: 0x00027A90
	public virtual void OnReset()
	{
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00029893 File Offset: 0x00027A93
	public virtual void InitMonitorFieldIds()
	{
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00029896 File Offset: 0x00027A96
	public virtual void NotifyUIShow()
	{
		this.PreInitializeUIMasks();
		GEvent.OnEvent(UiEvents.OnUIElementShow, EasyPool.Get<ArgumentBox>().SetObject("Element", this.Element));
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x000298C4 File Offset: 0x00027AC4
	private void PreInitializeUIMasks()
	{
		UIMask[] masks = base.GetComponentsInChildren<UIMask>(true);
		bool flag = masks == null || masks.Length == 0;
		if (!flag)
		{
			foreach (UIMask mask in masks)
			{
				bool isSharedManaged = mask.IsSharedManaged;
				if (!isSharedManaged)
				{
					bool activeInHierarchy = mask.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						mask.ForceInitialize();
					}
				}
			}
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x0002992E File Offset: 0x00027B2E
	public virtual void NotifyUIHide()
	{
		GEvent.OnEvent(UiEvents.OnUIElementHide, EasyPool.Get<ArgumentBox>().SetObject("Element", this.Element));
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00029954 File Offset: 0x00027B54
	public virtual void PlayAudioIn()
	{
		bool flag = !this.AudioIn.IsNullOrEmpty();
		string audioName;
		if (flag)
		{
			audioName = this.AudioIn;
		}
		else
		{
			audioName = this.GetDefaultAudioInByType();
			bool flag2 = audioName.IsNullOrEmpty();
			if (flag2)
			{
				audioName = "";
			}
		}
		bool flag3 = !audioName.IsNullOrEmpty();
		if (flag3)
		{
			AudioManager.Instance.PlaySound(audioName, false, false);
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x000299B4 File Offset: 0x00027BB4
	public virtual void PlayAudioOut()
	{
		bool flag = !this.AudioOut.IsNullOrEmpty();
		string audioName;
		if (flag)
		{
			audioName = this.AudioOut;
		}
		else
		{
			audioName = this.GetDefaultAudioOutByType();
			bool flag2 = audioName.IsNullOrEmpty();
			if (flag2)
			{
				audioName = "";
			}
		}
		bool flag3 = !audioName.IsNullOrEmpty();
		if (flag3)
		{
			AudioManager.Instance.PlaySound(audioName, false, false);
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00029A13 File Offset: 0x00027C13
	public virtual void NotifyUIShowFinish()
	{
		GEvent.OnEvent(UiEvents.OnUIElementShowFinish, EasyPool.Get<ArgumentBox>().SetObject("Element", this.Element));
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00029A37 File Offset: 0x00027C37
	public virtual void NotifyUIHideStart()
	{
		GEvent.OnEvent(UiEvents.OnUIElementHideStart, EasyPool.Get<ArgumentBox>().SetObject("Element", this.Element));
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00029A5C File Offset: 0x00027C5C
	private string GetDefaultAudioInByType()
	{
		UIBase.UIOpenCloseAudioType openCloseAudio = this.OpenCloseAudio;
		if (!true)
		{
		}
		string result;
		if (openCloseAudio != UIBase.UIOpenCloseAudioType.BigWindow)
		{
			if (openCloseAudio != UIBase.UIOpenCloseAudioType.SmallWindow)
			{
				result = string.Empty;
			}
			else
			{
				result = "ui_default_small_whoosh";
			}
		}
		else
		{
			result = "ui_default_whoosh";
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00029AA4 File Offset: 0x00027CA4
	private string GetDefaultAudioOutByType()
	{
		UIBase.UIOpenCloseAudioType openCloseAudio = this.OpenCloseAudio;
		if (!true)
		{
		}
		string result;
		if (openCloseAudio != UIBase.UIOpenCloseAudioType.BigWindow)
		{
			if (openCloseAudio != UIBase.UIOpenCloseAudioType.SmallWindow)
			{
				result = string.Empty;
			}
			else
			{
				result = "ui_default_small_back";
			}
		}
		else
		{
			result = "ui_default_cancel";
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00029AEC File Offset: 0x00027CEC
	public void AppendMonitorFieldId(UIBase.MonitorDataField dataField)
	{
		bool flag = this.Element.GameDataListenerId < 0;
		if (!flag)
		{
			this.MonitorFields.Add(dataField);
			bool flag2 = dataField.SubId1List != null;
			if (flag2)
			{
				GameDataBridge.AddDataMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
		}
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x00029B78 File Offset: 0x00027D78
	public void RemoveMonitorFieldId(int index)
	{
		UIBase.MonitorDataField dataField = this.MonitorFields[index];
		this.MonitorFields.RemoveAt(index);
		bool flag = dataField.SubId1List != null;
		if (flag)
		{
			GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
		}
		else
		{
			GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
		}
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00029BFC File Offset: 0x00027DFC
	public void RemoveMonitorFieldId(ushort domainId, ushort dataId)
	{
		for (int i = this.MonitorFields.Count - 1; i >= 0; i--)
		{
			UIBase.MonitorDataField dataField = this.MonitorFields[i];
			bool flag = dataField.DomainId == domainId && dataField.DataId == dataId;
			if (flag)
			{
				GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, domainId, dataId, ulong.MaxValue, uint.MaxValue);
				this.MonitorFields.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00029C78 File Offset: 0x00027E78
	public void RemoveMonitorFieldId(ushort domainId, ushort dataId, ulong subId0)
	{
		for (int i = this.MonitorFields.Count - 1; i >= 0; i--)
		{
			UIBase.MonitorDataField dataField = this.MonitorFields[i];
			bool flag = dataField.DomainId == domainId && dataField.DataId == dataId && dataField.SubId0 == subId0;
			if (flag)
			{
				bool flag2 = dataField.SubId1List != null;
				if (flag2)
				{
					GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, domainId, dataId, subId0, dataField.SubId1List);
				}
				else
				{
					GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, domainId, dataId, subId0, uint.MaxValue);
				}
				this.MonitorFields.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00029D2C File Offset: 0x00027F2C
	public List<UIBase.MonitorDataField> GetMonitorFields()
	{
		return this.MonitorFields;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00029D44 File Offset: 0x00027F44
	public void ClearMonitorFields()
	{
		for (int i = 0; i < this.MonitorFields.Count; i++)
		{
			UIBase.MonitorDataField dataField = this.MonitorFields[i];
			bool flag = dataField.SubId1List != null;
			if (flag)
			{
				GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
		}
		this.MonitorFields.Clear();
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00029DE8 File Offset: 0x00027FE8
	public bool NeedGameDataListenerId(bool checkNeedWaitData = false)
	{
		return this.MonitorFields.Count > 0 || this.NeedDataListenerId || (checkNeedWaitData && this.NeedWaitData);
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00029E1F File Offset: 0x0002801F
	public virtual void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00029E24 File Offset: 0x00028024
	public virtual void QuickHide()
	{
		bool flag = this.Element.IsInState(EUiElementState.Ready);
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00029E54 File Offset: 0x00028054
	public Sequence GetAnimSequenceIn()
	{
		return this.AnimSequenceIn;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00029E6C File Offset: 0x0002806C
	public Sequence AddAnimToDefaultSequenceIn(GameObject obj, Sequence sequence, float fadeDuration, float moveDuration, float moveDistance, bool horizontal)
	{
		RectTransform rectTransform = obj.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = obj.GetOrAddComponent<CanvasGroup>();
		if (horizontal)
		{
			float pos = rectTransform.anchoredPosition.x;
			sequence.AppendCallback(delegate
			{
				rectTransform.anchoredPosition = rectTransform.anchoredPosition.SetX(pos - moveDistance);
			});
			sequence.AppendCallback(delegate
			{
				rectTransform.DOAnchorPosX(pos, moveDuration, false).SetEase(Ease.OutExpo);
			});
		}
		else
		{
			float pos = rectTransform.anchoredPosition.y;
			sequence.AppendCallback(delegate
			{
				rectTransform.anchoredPosition = rectTransform.anchoredPosition.SetY(pos - moveDistance);
			});
			sequence.AppendCallback(delegate
			{
				rectTransform.DOAnchorPosY(pos, moveDuration, false).SetEase(Ease.OutExpo);
			});
		}
		sequence.AppendCallback(delegate
		{
			canvasGroup.alpha = 0f;
		});
		sequence.AppendCallback(delegate
		{
			canvasGroup.DOFade(1f, fadeDuration);
		});
		return sequence;
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00029F84 File Offset: 0x00028184
	public Sequence InitDefaultSequenceIn(bool withAnim)
	{
		this.DefaultAnimSequenceIn = DOTween.Sequence();
		if (withAnim)
		{
			bool flag = !this.defaultAnimSequenceInRoot;
			if (flag)
			{
				this.defaultAnimSequenceInRoot = base.gameObject;
			}
			this.AddAnimToDefaultSequenceIn(this.defaultAnimSequenceInRoot, this.DefaultAnimSequenceIn, 0.33f, 0.66f, 200f, true);
		}
		return this.DefaultAnimSequenceIn;
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00029FF0 File Offset: 0x000281F0
	public Sequence GetAnimSequenceOut()
	{
		return this.AnimSequenceOut;
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0002A008 File Offset: 0x00028208
	public bool GetAnimInGroupSequence(out Sequence sequence)
	{
		sequence = this.GetAnimInGroupSequence();
		return sequence != null;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x0002A028 File Offset: 0x00028228
	private Sequence GetAnimInGroupSequence()
	{
		bool flag = this.AnimIn == null;
		Sequence result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = this.AnimInGroupSequence == null;
			if (flag2)
			{
				Sequence sequence = DOTween.Sequence();
				List<Tween> tweens = this.AnimIn.GetTweens();
				bool flag3 = tweens.Count == 0;
				if (flag3)
				{
					foreach (DOTweenAnimation anim in this.AnimIn.GetComponents<DOTweenAnimation>())
					{
						anim.CreateTween(true, false);
					}
					tweens = this.AnimIn.GetTweens();
				}
				foreach (Tween t in tweens)
				{
					sequence.Join(t);
				}
				sequence.SetAutoKill(false);
				sequence.SetUpdate(true);
				sequence.Pause<Sequence>();
				this.AnimInGroupSequence = sequence;
			}
			result = this.AnimInGroupSequence;
		}
		return result;
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0002A138 File Offset: 0x00028338
	protected virtual void OnClick(Transform btn)
	{
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0002A13C File Offset: 0x0002833C
	protected virtual bool CanClick()
	{
		return this.Element.IsInState(EUiElementState.Ready) && !UIManager.Instance.BlockHotKey;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0002A16C File Offset: 0x0002836C
	public void HandleClick(Transform btnTrans)
	{
		bool flag = !this.CanClick() || btnTrans == null;
		if (!flag)
		{
			this.OnClick(btnTrans);
		}
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0002A19C File Offset: 0x0002839C
	public void RegisterRelativeAtlases()
	{
		bool flag = null == AtlasInfo.Instance;
		if (!flag)
		{
			bool flag2 = this.RelativeAtlases == null || this.RelativeAtlases.Length == 0;
			if (!flag2)
			{
				foreach (SpriteAtlas atlas in this.RelativeAtlases)
				{
					bool flag3 = null != atlas;
					if (flag3)
					{
						AtlasInfo.Instance.RegisterLoadedPacker(atlas.name, atlas);
					}
				}
			}
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0002A215 File Offset: 0x00028415
	public void UnRegisterRelativeAtlases()
	{
		AtlasInfo.Instance.MarkPackerUnload(this.RelativeAtlases);
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0002A22C File Offset: 0x0002842C
	public virtual void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		this.CurLanguageType = languageType;
		TextStyle[] textStyleList = base.gameObject.GetComponentsInChildren<TextStyle>(true);
		foreach (TextStyle textStyle in textStyleList)
		{
			textStyle.OnLanguageChange(languageType);
		}
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x0002A26B File Offset: 0x0002846B
	public void RegisterAsyncMethodCall(int requestId)
	{
		this._requestedAsyncMethods.Add(requestId);
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x0002A27C File Offset: 0x0002847C
	public void ClearAsyncMethodCalls()
	{
		AsyncMethodDispatcher dispatcher = SingletonObject.getInstance<AsyncMethodDispatcher>();
		foreach (int one in this._requestedAsyncMethods)
		{
			dispatcher.UnregisterAsyncMethodCall(one);
		}
		this._requestedAsyncMethods.Clear();
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x0002A2E8 File Offset: 0x000284E8
	public void AsyncMethodCall(ushort domainId, ushort methodId, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(domainId, methodId, callback));
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0002A304 File Offset: 0x00028504
	public void AsyncMethodCall<T1>(ushort domainId, ushort methodId, T1 arg1, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1>(domainId, methodId, arg1, callback));
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x0002A322 File Offset: 0x00028522
	public void AsyncMethodCall<T1, T2>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2>(domainId, methodId, arg1, arg2, callback));
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0002A342 File Offset: 0x00028542
	public void AsyncMethodCall<T1, T2, T3>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3>(domainId, methodId, arg1, arg2, arg3, callback));
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0002A364 File Offset: 0x00028564
	public void AsyncMethodCall<T1, T2, T3, T4>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4>(domainId, methodId, arg1, arg2, arg3, arg4, callback));
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0002A394 File Offset: 0x00028594
	public void AsyncMethodCall<T1, T2, T3, T4, T5>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, callback));
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0002A3C8 File Offset: 0x000285C8
	public void AsyncMethodCall<T1, T2, T3, T4, T5, T6>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, callback));
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x0002A3FC File Offset: 0x000285FC
	public void AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback));
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0002A434 File Offset: 0x00028634
	public void AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback));
	}

	// Token: 0x04000514 RID: 1300
	public DOTweenAnimation AnimIn;

	// Token: 0x04000515 RID: 1301
	public DOTweenAnimation AnimOut;

	// Token: 0x04000516 RID: 1302
	public UIBase.UIOpenCloseAudioType OpenCloseAudio;

	// Token: 0x04000517 RID: 1303
	[Tooltip("标记UI的一些特点\n\nIncludeCoverCheck：参与UIVisableHandler系统的检查\n\nFullCover：全屏界面，完全遮挡背后的界面（包含IncludeCoverCheck特性）\n\nFullCoverIgnoreAnim：全屏界面，使用ShowHide事件而不是ShowFinish/HideStart事件（包含FullCover特性）\n\nFlag31：预留一个高位的flag，不要设置，避免Unity处理everyting时直接把新加的flag自动设置了。")]
	public UIFlag UiFlags;

	// Token: 0x04000518 RID: 1304
	public string AudioIn;

	// Token: 0x04000519 RID: 1305
	public string AudioOut;

	// Token: 0x0400051A RID: 1306
	protected const string DefaultAudioIn = "";

	// Token: 0x0400051B RID: 1307
	protected const string DefaultAudioOut = "";

	// Token: 0x0400051C RID: 1308
	protected Sequence AnimSequenceIn;

	// Token: 0x0400051D RID: 1309
	protected Sequence AnimSequenceOut;

	// Token: 0x0400051E RID: 1310
	protected Sequence AnimInGroupSequence;

	// Token: 0x0400051F RID: 1311
	public UILayer UiType;

	// Token: 0x04000520 RID: 1312
	public UIElement Element;

	// Token: 0x04000521 RID: 1313
	public SpriteAtlas[] RelativeAtlases;

	// Token: 0x04000522 RID: 1314
	protected List<UIBase.MonitorDataField> MonitorFields = new List<UIBase.MonitorDataField>();

	// Token: 0x04000523 RID: 1315
	protected bool NeedDataListenerId = false;

	// Token: 0x04000524 RID: 1316
	protected bool NeedWaitData = false;

	// Token: 0x04000525 RID: 1317
	private readonly List<int> _requestedAsyncMethods = new List<int>();

	// Token: 0x04000526 RID: 1318
	private Canvas _canvas;

	// Token: 0x04000527 RID: 1319
	protected LocalStringManager.LanguageType CurLanguageType;

	// Token: 0x04000528 RID: 1320
	public GameObject defaultAnimSequenceInRoot;

	// Token: 0x04000529 RID: 1321
	public Sequence DefaultAnimSequenceIn;

	// Token: 0x0400052A RID: 1322
	public const float DefaultAnimMoveDuration = 0.66f;

	// Token: 0x0400052B RID: 1323
	public const float DefaultAnimFadeDuration = 0.33f;

	// Token: 0x0400052C RID: 1324
	public const float DefaultAnimMoveOffset = 200f;

	// Token: 0x02001115 RID: 4373
	public enum UIOpenCloseAudioType
	{
		// Token: 0x04009591 RID: 38289
		None,
		// Token: 0x04009592 RID: 38290
		BigWindow,
		// Token: 0x04009593 RID: 38291
		SmallWindow,
		// Token: 0x04009594 RID: 38292
		Custom
	}

	// Token: 0x02001116 RID: 4374
	public struct MonitorDataField : IEquatable<UIBase.MonitorDataField>
	{
		// Token: 0x0600C15C RID: 49500 RVA: 0x0056E956 File Offset: 0x0056CB56
		public MonitorDataField(ushort domainId, ushort dataId, ulong subId0 = 18446744073709551615UL, uint[] subId1List = null)
		{
			this.DomainId = domainId;
			this.DataId = dataId;
			this.SubId0 = subId0;
			this.SubId1List = subId1List;
		}

		// Token: 0x0600C15D RID: 49501 RVA: 0x0056E978 File Offset: 0x0056CB78
		public bool Equals(UIBase.MonitorDataField other)
		{
			bool flag = this.DomainId != other.DomainId;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.DataId != other.DataId;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.SubId0 != other.SubId0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.SubId1List == null && other.SubId1List == null;
						if (flag4)
						{
							result = true;
						}
						else
						{
							bool flag5 = this.SubId1List == null || other.SubId1List == null;
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = this.SubId1List.Length != other.SubId1List.Length;
								if (flag6)
								{
									result = false;
								}
								else
								{
									for (int i = 0; i < this.SubId1List.Length; i++)
									{
										bool flag7 = this.SubId1List[i] != other.SubId1List[i];
										if (flag7)
										{
											return false;
										}
									}
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04009595 RID: 38293
		public ushort DomainId;

		// Token: 0x04009596 RID: 38294
		public ushort DataId;

		// Token: 0x04009597 RID: 38295
		public ulong SubId0;

		// Token: 0x04009598 RID: 38296
		public uint[] SubId1List;
	}
}
