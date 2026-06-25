using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.World;
using GameData.Domains.World.MonthlyEvent;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using UnityEngine;

// Token: 0x0200038F RID: 911
public class UI_MonthlyEvent : UIBase
{
	// Token: 0x170005A8 RID: 1448
	// (get) Token: 0x0600360F RID: 13839 RVA: 0x001B34CC File Offset: 0x001B16CC
	private bool HandlingMonthlyEventBlock
	{
		get
		{
			return SingletonObject.getInstance<DisplayTriggerModel>().HandlingMonthlyEventBlock;
		}
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x001B34D8 File Offset: 0x001B16D8
	private void Awake()
	{
		base.CGet<InfinityScrollLegacy>("MonthlyEventScrollView").OnItemRender = new Action<int, Refers>(this.OnMonthlyEventItemRender);
	}

	// Token: 0x06003611 RID: 13841 RVA: 0x001B34F7 File Offset: 0x001B16F7
	private void OnDisable()
	{
		base.CGet<InfinityScrollLegacy>("MonthlyEventScrollView").SetDataCount(0);
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x001B350C File Offset: 0x001B170C
	public override void OnInit(ArgumentBox argsBox)
	{
		if (argsBox != null)
		{
			argsBox.Get("NeedSave", out this._needSave);
		}
		if (argsBox != null)
		{
			argsBox.Get<List<MonthlyEventRenderInfo>>("RenderInfoList", out this._monthlyEventRenderInfoList);
		}
		if (argsBox != null)
		{
			argsBox.Get<ArgumentCollection>("Arguments", out this._monthlyEventArgumentCollection);
		}
		base.CGet<GameObject>("MonthEventArea").SetActive(false);
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x001B3574 File Offset: 0x001B1774
	private void OnEnable()
	{
		List<MonthlyEventRenderInfo> monthlyEventRenderInfoList = this._monthlyEventRenderInfoList;
		bool flag = monthlyEventRenderInfoList != null && monthlyEventRenderInfoList.Count > 0;
		if (flag)
		{
			LifeRecordDomainMethod.AsyncCall.GetRecordRenderInfoArguments(null, "UI_MonthNotify_MonthlyEvent", new RecordArgumentsRequest(this._monthlyEventArgumentCollection), delegate(int offset, RawDataPool pool)
			{
				ArgumentCollectionRenderArguments dynamicArguments = null;
				Serializer.Deserialize(pool, offset, ref dynamicArguments);
				this.UpdateMonthlyEventCollectionArguments(dynamicArguments);
			});
		}
		else
		{
			this.RefreshMonthlyEventScroll();
		}
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x001B35CC File Offset: 0x001B17CC
	public override void QuickHide()
	{
		bool flag = SingletonObject.getInstance<BasicGameData>().SavingWorld || SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
		if (!flag)
		{
			List<MonthlyEventRenderInfo> monthlyEventRenderInfoList = this._monthlyEventRenderInfoList;
			bool flag2 = monthlyEventRenderInfoList != null && monthlyEventRenderInfoList.Count > 0;
			if (!flag2)
			{
				base.QuickHide();
			}
		}
	}

	// Token: 0x06003615 RID: 13845 RVA: 0x001B3620 File Offset: 0x001B1820
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "DefaultAll" == btnName;
		if (flag)
		{
			btn.GetComponent<CButton>().interactable = false;
			bool flag2 = this.HasForbidDefaultEvent();
			if (!flag2)
			{
				WorldDomainMethod.Call.ProcessAllMonthlyEventsWithDefaultOption();
				this._monthlyEventRenderInfoList.Clear();
				this.RefreshMonthlyEventScroll();
			}
		}
	}

	// Token: 0x06003616 RID: 13846 RVA: 0x001B3678 File Offset: 0x001B1878
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 4, ulong.MaxValue, null));
	}

	// Token: 0x06003617 RID: 13847 RVA: 0x001B3694 File Offset: 0x001B1894
	private void OnMonthlyEventItemRender(int index, Refers refers)
	{
		MonthlyEventRenderInfo info = this._monthlyEventRenderInfoList[index];
		MonthlyEventItem config = MonthlyEvent.Instance.GetItem(info.RecordType);
		this.HandleMonthlyEventItemRender(refers, config, info);
		GameObject gameObject = refers.CGet<GameObject>("CannotSkipMark");
		EMonthlyEventType type = config.Type;
		gameObject.SetActive(type == EMonthlyEventType.SpecialEvent || type == EMonthlyEventType.LockedEvent);
		refers.CGet<CButtonObsolete>("Button").interactable = true;
		refers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
		{
			bool ignoreClick = this._ignoreClick;
			if (!ignoreClick)
			{
				this._ignoreClick = true;
				this.<>n__0();
				UIManager.Instance.HideUI(UIElement.MonthNotify);
				refers.CGet<CButtonObsolete>("Button").interactable = false;
				WorldDomainMethod.Call.HandleMonthlyEvent(info.Offset);
			}
		});
	}

	// Token: 0x06003618 RID: 13848 RVA: 0x001B375C File Offset: 0x001B195C
	private void OnFinishHandlingMonthlyEvent(ArgumentBox argBox)
	{
		this._ignoreClick = false;
		bool handlingMonthlyEventBlock = this.HandlingMonthlyEventBlock;
		if (!handlingMonthlyEventBlock)
		{
			bool flag = UIManager.Instance.IsFocusElement(UIElement.MainMenu);
			if (flag)
			{
				this.QuickHide();
				GEvent.Remove(UiEvents.OnHandlingMonthlyEventBlockChange, new GEvent.Callback(this.OnFinishHandlingMonthlyEvent));
			}
			else
			{
				bool needSave = !SingletonObject.getInstance<GlobalSettings>().SkipSaving && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				UIElement.MonthNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", needSave));
				UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
			}
		}
	}

	// Token: 0x06003619 RID: 13849 RVA: 0x001B37FC File Offset: 0x001B19FC
	private void UpdateMonthlyEventCollectionArguments(ArgumentCollectionRenderArguments dynamicArguments)
	{
		this._monthlyEventRenderedArgumentCollection.Clear();
		GameMessageUtils.RenderDynamicArguments(dynamicArguments, this._monthlyEventArgumentCollection, this._monthlyEventRenderedArgumentCollection, false, false);
		GameMessageUtils.RenderFixedArguments(this._monthlyEventArgumentCollection, this._monthlyEventRenderedArgumentCollection, false);
		this.RefreshMonthlyEventScroll();
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x001B383C File Offset: 0x001B1A3C
	private void HideMonthEventArea()
	{
		GameObject monthEventAreaObj = base.CGet<GameObject>("MonthEventArea");
		SkeletonGraphic spineAnimGraphic = base.CGet<SkeletonGraphic>("LetterAnimation");
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("EventCanvas");
		Sequence sequence = DOTween.Sequence();
		sequence.Append(canvasGroup.DOFade(0f, 0.3f));
		sequence.AppendInterval(0.3f);
		sequence.AppendCallback(delegate
		{
			spineAnimGraphic.AnimationState.SetAnimation(0, "close", false);
			AudioManager.Instance.PlaySound("ui_default_letter", false, false);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			monthEventAreaObj.SetActive(false);
		});
		base.CGet<GameObject>("EventClickMask").SetActive(true);
		this.QuickHide();
	}

	// Token: 0x0600361B RID: 13851 RVA: 0x001B38F4 File Offset: 0x001B1AF4
	private void RefreshMonthlyEventScroll()
	{
		GameObject monthEventAreaObj = base.CGet<GameObject>("MonthEventArea");
		SkeletonGraphic spineAnimGraphic = base.CGet<SkeletonGraphic>("LetterAnimation");
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("EventCanvas");
		GameObject eventClickMask = base.CGet<GameObject>("EventClickMask");
		eventClickMask.SetActive(this._monthlyEventRenderInfoList.Count > 0);
		bool flag = this._monthlyEventRenderInfoList.Count > 0;
		if (flag)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			bool flag2 = this._monthlyEventRenderInfoList.Count > 1;
			if (flag2)
			{
				this._monthlyEventRenderInfoList.Sort(delegate(MonthlyEventRenderInfo left, MonthlyEventRenderInfo right)
				{
					MonthlyEventItem configLeft = MonthlyEvent.Instance.GetItem(left.RecordType);
					MonthlyEventItem configRight = MonthlyEvent.Instance.GetItem(right.RecordType);
					bool flag6 = configLeft.Type != configRight.Type;
					int result;
					if (flag6)
					{
						result = configRight.Type - configLeft.Type;
					}
					else
					{
						bool flag7 = left.RecordType != right.RecordType;
						if (flag7)
						{
							result = right.RecordType.CompareTo(left.RecordType);
						}
						else
						{
							result = right.Offset.CompareTo(left.Offset);
						}
					}
					return result;
				});
			}
			base.CGet<InfinityScrollLegacy>("MonthlyEventScrollView").SetDataCount(this._monthlyEventRenderInfoList.Count);
			bool hasForbidDefaultAllEvent = this.HasForbidDefaultEvent();
			base.CGet<CButton>("DefaultAll").interactable = !hasForbidDefaultAllEvent;
			base.CGet<CButton>("DefaultAll").GetComponent<TooltipInvoker>().enabled = hasForbidDefaultAllEvent;
			base.CGet<GameObject>("CanDefaultAll").SetActive(!hasForbidDefaultAllEvent);
			spineAnimGraphic.AnimationState.SetAnimation(0, "open", false);
			AudioManager.Instance.PlaySound("ui_default_letter", false, false);
			canvasGroup.DOFade(1f, 0.3f).SetDelay(1f).OnComplete(delegate
			{
				canvasGroup.interactable = true;
				eventClickMask.SetActive(false);
			});
			monthEventAreaObj.SetActive(true);
		}
		else
		{
			bool activeSelf = monthEventAreaObj.activeSelf;
			if (activeSelf)
			{
				bool flag3 = !this.Element.Ready;
				if (flag3)
				{
					UIElement element = this.Element;
					element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.HideMonthEventArea));
				}
				else
				{
					this.HideMonthEventArea();
				}
			}
		}
		this.Element.ShowAfterRefresh();
		bool flag4 = GMFunc.DisableAutoSaving && this._needSave;
		if (flag4)
		{
			WorldDomainMethod.Call.ProcessAllMonthlyEventsWithDefaultOption();
			this._monthlyEventRenderInfoList.Clear();
			base.CGet<InfinityScrollLegacy>("MonthlyEventScrollView").SetDataCount(0);
			this.TryEndAdvancingMonth();
			this.QuickHide();
		}
		else
		{
			bool flag5 = this._monthlyEventRenderInfoList.Count <= 0;
			if (flag5)
			{
				GEvent.Remove(UiEvents.OnHandlingMonthlyEventBlockChange, new GEvent.Callback(this.OnFinishHandlingMonthlyEvent));
				this.TryEndAdvancingMonth();
			}
			else
			{
				GEvent.Add(UiEvents.OnHandlingMonthlyEventBlockChange, new GEvent.Callback(this.OnFinishHandlingMonthlyEvent));
			}
		}
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x001B3B9C File Offset: 0x001B1D9C
	private void TryEndAdvancingMonth()
	{
		bool flag = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 14;
		if (!flag)
		{
			bool flag2 = this._monthlyEventRenderInfoList.Count > 0;
			if (!flag2)
			{
				bool needSave = this._needSave && !GMFunc.DisableAutoSaving;
				WorldDomainMethod.Call.AdvanceMonth_DisplayedMonthlyNotifications(needSave);
				this._needSave = false;
			}
		}
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x001B3BF8 File Offset: 0x001B1DF8
	private void HandleMonthlyEventItemRender(Refers refers, MonthlyEventItem config, MonthlyEventRenderInfo info)
	{
		refers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
		TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
		mouseTip.PresetParam = new string[]
		{
			config.Name,
			config.Desc
		};
		mouseTip.PresetParam[1] = info.GetText(this._monthlyEventRenderedArgumentCollection).ColorReplace();
	}

	// Token: 0x0600361E RID: 13854 RVA: 0x001B3C64 File Offset: 0x001B1E64
	private bool HasForbidDefaultEvent()
	{
		bool flag = this._monthlyEventRenderInfoList == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (MonthlyEventRenderInfo renderInfo in this._monthlyEventRenderInfoList)
			{
				MonthlyEventItem config = MonthlyEvent.Instance.GetItem(renderInfo.RecordType);
				bool flag2 = config.Type > EMonthlyEventType.NormalEvent;
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x04002739 RID: 10041
	private bool _needSave;

	// Token: 0x0400273A RID: 10042
	private List<MonthlyEventRenderInfo> _monthlyEventRenderInfoList = new List<MonthlyEventRenderInfo>();

	// Token: 0x0400273B RID: 10043
	private ArgumentCollection _monthlyEventArgumentCollection = new ArgumentCollection();

	// Token: 0x0400273C RID: 10044
	private readonly RenderedArgumentCollection _monthlyEventRenderedArgumentCollection = new RenderedArgumentCollection();

	// Token: 0x0400273D RID: 10045
	private const string MonthlyEventArgumentKey = "UI_MonthNotify_MonthlyEvent";

	// Token: 0x0400273E RID: 10046
	private bool _ignoreClick;
}
