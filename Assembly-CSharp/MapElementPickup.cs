using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020003F1 RID: 1009
public class MapElementPickup : MapElementBase
{
	// Token: 0x1700062B RID: 1579
	// (get) Token: 0x06003CAA RID: 15530 RVA: 0x001E8B87 File Offset: 0x001E6D87
	private MapElementPickupDisplayData FirstPickup
	{
		get
		{
			Dictionary<Location, List<MapElementPickupDisplayData>> visibleMapPickupDict = MapElementBase.MapModel.VisibleMapPickupDict;
			MapElementPickupDisplayData result;
			if (visibleMapPickupDict == null)
			{
				result = null;
			}
			else
			{
				List<MapElementPickupDisplayData> valueOrDefault = visibleMapPickupDict.GetValueOrDefault(base.BlockLocation);
				result = ((valueOrDefault != null) ? valueOrDefault.First<MapElementPickupDisplayData>() : null);
			}
			return result;
		}
	}

	// Token: 0x06003CAB RID: 15531 RVA: 0x001E8BB4 File Offset: 0x001E6DB4
	public static bool CheckMaybeExist(Location location)
	{
		bool crossArchiveLockMoveTime = MapElementBase.MapModel.CrossArchiveLockMoveTime;
		bool result;
		if (crossArchiveLockMoveTime)
		{
			result = false;
		}
		else
		{
			bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = MapElementBase.MapModel.CurrentAreaId != location.AreaId;
					if (flag3)
					{
						result = false;
					}
					else
					{
						MapBlockData block = MapElementBase.MapModel.GetBlockData(location);
						bool flag4 = block == null || !block.Visible;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool isLocationShouldInSight = MapElementBase.MapModel.IsLocationShouldInSight(location);
							bool inSightState = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(40, true);
							bool flag5 = isLocationShouldInSight && !inSightState;
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool outSightState = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(41, true);
								bool flag6 = !isLocationShouldInSight && !outSightState;
								if (flag6)
								{
									result = false;
								}
								else
								{
									Dictionary<Location, List<MapElementPickupDisplayData>> visibleMapPickupDict = MapElementBase.MapModel.VisibleMapPickupDict;
									result = (visibleMapPickupDict != null && visibleMapPickupDict.ContainsKey(location));
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x1700062C RID: 1580
	// (get) Token: 0x06003CAC RID: 15532 RVA: 0x001E8CE5 File Offset: 0x001E6EE5
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Cricket;
		}
	}

	// Token: 0x06003CAD RID: 15533 RVA: 0x001E8CE8 File Offset: 0x001E6EE8
	public override void Scale(float wheel)
	{
		bool flag = this.content.alpha > 0.9f;
		if (flag)
		{
			float scale = Mathf.Pow(1f / wheel, 1.5f) * wheel;
			this.scaleRoot.localScale = Vector3.one * scale;
		}
		else
		{
			this.outSightScaleRoot.localScale = Vector3.one / wheel;
		}
	}

	// Token: 0x06003CAE RID: 15534 RVA: 0x001E8D54 File Offset: 0x001E6F54
	protected override void OnCreate()
	{
		this.triggerButton.ClearAndAddListener(new Action(this.OnClickTriggerButton));
		RectTransform normalRectTransform = this.normalIcon.GetComponent<RectTransform>();
		normalRectTransform.anchoredPosition = new Vector2(0f, normalRectTransform.anchoredPosition.y);
		PointerTrigger pointerTrigger = this.triggerButton.GetComponent<PointerTrigger>();
		bool flag = pointerTrigger != null;
		if (flag)
		{
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
		}
	}

	// Token: 0x06003CAF RID: 15535 RVA: 0x001E8DEC File Offset: 0x001E6FEC
	protected override void OnRefresh()
	{
		this.background.IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		bool isLocationShouldInSight = MapElementBase.MapModel.IsLocationShouldInSight(base.BlockLocation);
		MapElementPickupDisplayData normalPickup = this.FirstPickup;
		Dictionary<Location, List<MapElementPickupDisplayData>> visibleMapPickupDict = MapElementBase.MapModel.VisibleMapPickupDict;
		int? num;
		if (visibleMapPickupDict == null)
		{
			num = null;
		}
		else
		{
			List<MapElementPickupDisplayData> valueOrDefault = visibleMapPickupDict.GetValueOrDefault(base.BlockLocation);
			num = ((valueOrDefault != null) ? new int?(valueOrDefault.Count) : null);
		}
		int? num2 = num;
		int count = num2.GetValueOrDefault();
		this.Refresh(normalPickup, isLocationShouldInSight, count);
	}

	// Token: 0x06003CB0 RID: 15536 RVA: 0x001E8E8C File Offset: 0x001E708C
	public void Refresh(MapElementPickupDisplayData normalPickup, bool isInsight, int count)
	{
		MapElementPickup.<>c__DisplayClass11_0 CS$<>8__locals1;
		CS$<>8__locals1.normalPickup = normalPickup;
		CS$<>8__locals1.<>4__this = this;
		bool flag = !isInsight;
		if (flag)
		{
			this.outSightContent.alpha = 1f;
			this.content.alpha = 0f;
			this.background.GetComponent<TooltipInvoker>().enabled = false;
		}
		else
		{
			this.outSightContent.alpha = 0f;
			this.content.alpha = 1f;
			this.normalCount.text = count.ToString();
			this.normalCountBg.alpha = ((count > 1) ? 1f : 0f);
			MapElementPickup.SetImageWithCache(this.normalIcon, CS$<>8__locals1.normalPickup.Pickup.Template.Icon, ref this._normalIconCache);
			this.<Refresh>g__Action|11_0(ref CS$<>8__locals1);
		}
	}

	// Token: 0x06003CB1 RID: 15537 RVA: 0x001E8F70 File Offset: 0x001E7170
	private void RefreshNormalPickupDisableAndTip(bool enabledByReadAndLoop, MapPickup normalPickup)
	{
		this.triggerButton.interactable = true;
		this.RefreshHoverStyleEnable(enabledByReadAndLoop);
		TooltipInvoker tip = this.background.GetComponent<TooltipInvoker>();
		this.background.GetComponent<DisableStyleRoot>().SetStyleEffect(!enabledByReadAndLoop, false);
		TooltipInvoker tooltipInvoker = tip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		bool flag = !enabledByReadAndLoop;
		if (flag)
		{
			LanguageKey key = (normalPickup.Type == MapPickup.EMapPickupType.LoopEffect) ? LanguageKey.LK_Taiwu_No_LoopingNeigong : LanguageKey.LK_Taiwu_No_ReadingBook;
			tip.RuntimeParam.Set("arg0", LocalStringManager.Get(key));
		}
		else
		{
			string tipContent = normalPickup.Template.TipsContent;
			tip.RuntimeParam.Set("arg0", tipContent);
		}
	}

	// Token: 0x06003CB2 RID: 15538 RVA: 0x001E902C File Offset: 0x001E722C
	private void RefreshBgAndHover(bool hasNormal, bool hasEvent, MapPickup normalPickup, bool canAutoBeatXiangshuMinion)
	{
		bool flag = hasNormal && hasEvent;
		if (flag)
		{
			this.<RefreshBgAndHover>g__Set|13_0("map_eventbutton_0_0", "map_eventbutton_0_1");
		}
		else if (hasNormal)
		{
			bool hasXiangshuMinion = normalPickup.HasXiangshuMinion;
			if (hasXiangshuMinion)
			{
				if (canAutoBeatXiangshuMinion)
				{
					this.<RefreshBgAndHover>g__Set|13_0("map_eventbutton_4_0", "map_eventbutton_4_1");
				}
				else
				{
					this.<RefreshBgAndHover>g__Set|13_0("map_eventbutton_3_0", "map_eventbutton_3_1");
				}
			}
			else
			{
				this.<RefreshBgAndHover>g__Set|13_0("map_eventbutton_2_0", "map_eventbutton_2_1");
			}
		}
		else if (hasEvent)
		{
			this.<RefreshBgAndHover>g__Set|13_0("map_eventbutton_1_0", "map_eventbutton_1_1");
		}
	}

	// Token: 0x06003CB3 RID: 15539 RVA: 0x001E90BE File Offset: 0x001E72BE
	private void RefreshHoverStyleEnable(bool enabledByReadAndLoop)
	{
		this._hoverStyleEnabled = (this.IsOnThisBlock() && enabledByReadAndLoop);
	}

	// Token: 0x06003CB4 RID: 15540 RVA: 0x001E90D0 File Offset: 0x001E72D0
	private void OnPointerEnter()
	{
		bool hoverStyleEnabled = this._hoverStyleEnabled;
		if (hoverStyleEnabled)
		{
			this.hover.gameObject.SetActive(true);
		}
		GEvent.OnEvent(UiEvents.OnHoverMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
	}

	// Token: 0x06003CB5 RID: 15541 RVA: 0x001E911C File Offset: 0x001E731C
	private void OnPointerExit()
	{
		this.hover.gameObject.SetActive(false);
		GEvent.OnEvent(UiEvents.OnHoverExitMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
	}

	// Token: 0x06003CB6 RID: 15542 RVA: 0x001E9154 File Offset: 0x001E7354
	private bool IsOnThisBlock()
	{
		WorldMapModel mapModel = MapElementBase.MapModel;
		return mapModel.CurrentAreaId == base.BlockLocation.AreaId && mapModel.CurrentBlockId == base.BlockLocation.BlockId;
	}

	// Token: 0x06003CB7 RID: 15543 RVA: 0x001E9198 File Offset: 0x001E7398
	protected override void OnCollect()
	{
		CButtonObsolete cbuttonObsolete = this.triggerButton;
		if (cbuttonObsolete != null)
		{
			Button.ButtonClickedEvent onClick = cbuttonObsolete.onClick;
			if (onClick != null)
			{
				onClick.RemoveAllListeners();
			}
		}
		CButtonObsolete cbuttonObsolete2 = this.triggerButton;
		PointerTrigger pointerTrigger = (cbuttonObsolete2 != null) ? cbuttonObsolete2.GetComponent<PointerTrigger>() : null;
		bool flag = pointerTrigger != null;
		if (flag)
		{
			pointerTrigger.EnterEvent.RemoveListener(new UnityAction(this.OnPointerEnter));
			pointerTrigger.ExitEvent.RemoveListener(new UnityAction(this.OnPointerExit));
		}
	}

	// Token: 0x06003CB8 RID: 15544 RVA: 0x001E9214 File Offset: 0x001E7414
	private void OnClickTriggerButton()
	{
		WorldMapModel mapModel = MapElementBase.MapModel;
		bool flag = mapModel.TaiwuMoveState != WorldMapModel.MoveState.Idle && mapModel.TaiwuMoveState != WorldMapModel.MoveState.ProfessionTravelerSkill;
		if (!flag)
		{
			MapElementPickupDisplayData normalPickup = this.FirstPickup;
			bool flag2 = normalPickup == null;
			if (flag2)
			{
				Debug.LogError(string.Format("Clicked pickup but no event or normal pickup found! Location: {0}", base.BlockLocation));
			}
			else
			{
				bool flag3 = normalPickup.BanReason > 0U;
				if (flag3)
				{
					GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
				}
				else
				{
					bool canClickThrough = mapModel.CurrentAreaId != base.BlockLocation.AreaId || mapModel.CurrentBlockId != base.BlockLocation.BlockId || mapModel.TaiwuMoveState == WorldMapModel.MoveState.ProfessionTravelerSkill;
					bool flag4 = canClickThrough;
					if (flag4)
					{
						GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
					}
					else
					{
						this.triggerButton.interactable = false;
						this.TriggerNormalPickupWithoutCheck();
					}
				}
			}
		}
	}

	// Token: 0x06003CB9 RID: 15545 RVA: 0x001E9324 File Offset: 0x001E7524
	private void TriggerNormalPickupWithoutCheck()
	{
		bool flag = base.gameObject == null;
		if (!flag)
		{
			TaiwuEventDomainMethod.Call.OnClickMapPickupNormalEvent(base.BlockLocation);
		}
	}

	// Token: 0x06003CBA RID: 15546 RVA: 0x001E9350 File Offset: 0x001E7550
	private static void SetImageWithCache(CImage icon, string iconName, ref string cache)
	{
		bool flag = iconName == cache;
		if (!flag)
		{
			icon.SetSprite(iconName, true, null);
			cache = iconName;
		}
	}

	// Token: 0x06003CBB RID: 15547 RVA: 0x001E937C File Offset: 0x001E757C
	private void Update()
	{
		bool flag = MapCommandKit.PickupMapItems.Check(UIElement.WorldMap, false, false, false, true, false) && this.IsOnThisBlock();
		if (flag)
		{
			Button.ButtonClickedEvent onClick = this.triggerButton.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
		}
	}

	// Token: 0x06003CBD RID: 15549 RVA: 0x001E93D0 File Offset: 0x001E75D0
	[CompilerGenerated]
	private void <Refresh>g__Action|11_0(ref MapElementPickup.<>c__DisplayClass11_0 A_1)
	{
		bool enabledByReadAndLoop = A_1.normalPickup.BanReason == 0U;
		this.RefreshHoverStyleEnable(enabledByReadAndLoop);
		TooltipInvoker tip = this.background.GetComponent<TooltipInvoker>();
		tip.enabled = true;
		this.RefreshNormalPickupDisableAndTip(enabledByReadAndLoop, A_1.normalPickup.Pickup);
		this.RefreshBgAndHover(true, false, A_1.normalPickup.Pickup, A_1.normalPickup.CanAutoBeatXiangshuMinion);
	}

	// Token: 0x06003CBE RID: 15550 RVA: 0x001E943B File Offset: 0x001E763B
	[CompilerGenerated]
	private void <RefreshBgAndHover>g__Set|13_0(string bgPath, string hoverPath)
	{
		MapElementPickup.SetImageWithCache(this.background, bgPath, ref this._bgIconCache);
		MapElementPickup.SetImageWithCache(this.hover, hoverPath, ref this._hoverIconCache);
	}

	// Token: 0x04002B86 RID: 11142
	private string _normalIconCache;

	// Token: 0x04002B87 RID: 11143
	private string _bgIconCache;

	// Token: 0x04002B88 RID: 11144
	private string _hoverIconCache;

	// Token: 0x04002B89 RID: 11145
	private bool _hoverStyleEnabled;

	// Token: 0x04002B8A RID: 11146
	public CButtonObsolete triggerButton;

	// Token: 0x04002B8B RID: 11147
	public CImage normalIcon;

	// Token: 0x04002B8C RID: 11148
	public RectTransform scaleRoot;

	// Token: 0x04002B8D RID: 11149
	public RectTransform outSightScaleRoot;

	// Token: 0x04002B8E RID: 11150
	public CInputEventImage background;

	// Token: 0x04002B8F RID: 11151
	public CImage hover;

	// Token: 0x04002B90 RID: 11152
	public CanvasGroup content;

	// Token: 0x04002B91 RID: 11153
	public CanvasGroup outSightContent;

	// Token: 0x04002B92 RID: 11154
	public TextMeshProUGUI normalCount;

	// Token: 0x04002B93 RID: 11155
	public CanvasGroup normalCountBg;
}
