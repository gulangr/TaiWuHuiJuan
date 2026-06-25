using System;
using System.Collections.Generic;
using FrameWork;
using GameData.DLC;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200007C RID: 124
public abstract class MouseTipBase : UIBase
{
	// Token: 0x0600046D RID: 1133 RVA: 0x0001D3BC File Offset: 0x0001B5BC
	public virtual bool CanShowWithArgumentBox(ArgumentBox argumentBox)
	{
		return true;
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x0600046E RID: 1134 RVA: 0x0001D3BF File Offset: 0x0001B5BF
	protected virtual bool CanStick
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0001D3C4 File Offset: 0x0001B5C4
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("NeedRefresh", out this.NeedRefresh);
		bool flag = !argsBox.Get("ScreenWidth", out this._screenWidth);
		if (flag)
		{
			this._screenWidth = 2560f;
		}
		bool flag2 = !argsBox.Get("TipFixedPosX", out this.TipFixedPosX);
		if (flag2)
		{
			this.TipFixedPosX = 0f;
		}
		bool flag3 = argsBox.Get("_useCustomBounds", out this._useCustomBounds) && this._useCustomBounds;
		if (flag3)
		{
			argsBox.Get<Rect>("_customScreenBounds", out this._customScreenBounds);
			argsBox.Get("_considerCustomRight", out this._considerCustomRight);
			argsBox.Get("_considerCustomTop", out this._considerCustomTop);
			argsBox.Get("_considerCustomLeft", out this._considerCustomLeft);
			argsBox.Get("_considerCustomBottom", out this._considerCustomBottom);
		}
		this.Init(argsBox);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			bool needRefresh = this.NeedRefresh;
			if (needRefresh)
			{
				this.Refresh();
				this._lastRefreshTime = Time.time;
			}
		}));
		this.HasStick = false;
		this._canRefreshPosition = false;
		this._autoFixedActive = false;
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001D4EC File Offset: 0x0001B6EC
	private void AddEmptyRacastTarget()
	{
		bool flag = this.EmptyRaycastTarget != null;
		GameObject empty;
		if (flag)
		{
			empty = this.EmptyRaycastTarget;
		}
		else
		{
			bool flag2 = base.transform.childCount > 0 && base.transform.GetChild(0).name == "__TipsEmptyRaycastTarget";
			if (flag2)
			{
				empty = base.transform.GetChild(0).gameObject;
			}
			else
			{
				empty = new GameObject("__TipsEmptyRaycastTarget");
				empty.transform.SetParent(base.transform);
				empty.AddComponent<CEmptyGraphic>();
				LayoutElement layoutElement = empty.AddComponent<LayoutElement>();
				layoutElement.ignoreLayout = true;
			}
		}
		empty.transform.SetSiblingIndex(0);
		RectTransform rectTransform = empty.GetComponent<RectTransform>();
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.sizeDelta = Vector2.zero;
		rectTransform.localScale = Vector3.one;
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0001D5D4 File Offset: 0x0001B7D4
	public override void PlayAudioIn()
	{
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0001D5D7 File Offset: 0x0001B7D7
	public override void PlayAudioOut()
	{
	}

	// Token: 0x06000473 RID: 1139
	protected abstract void Init(ArgumentBox argsBox);

	// Token: 0x06000474 RID: 1140 RVA: 0x0001D5DA File Offset: 0x0001B7DA
	public virtual void Refresh()
	{
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001D5DD File Offset: 0x0001B7DD
	public virtual void Refresh(ArgumentBox argBox)
	{
		this.Refresh();
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0001D5E7 File Offset: 0x0001B7E7
	public virtual void OnSticked()
	{
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0001D5EC File Offset: 0x0001B7EC
	private void LateUpdate()
	{
		bool globalTipsHide = SingletonObject.getInstance<GlobalSettings>().GlobalTipsHide;
		if (globalTipsHide)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			bool stickCheckPass = CommonCommandKit.StickTips.Check(UIElement.PermanentTips, true, false, false, true, false);
			bool flag = this.NeedRefresh && Time.time - this._lastRefreshTime > 1f && GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				this.Refresh();
				this._lastRefreshTime = Time.time;
			}
			bool flag2 = !this.HasStick;
			if (flag2)
			{
				bool canRefreshPosition = this._canRefreshPosition;
				if (canRefreshPosition)
				{
					this.UpdatePos();
				}
				bool canStick = this.CanStick;
				if (canStick)
				{
					bool flag3 = this._lastStickCheckPass && !stickCheckPass && Time.time - this._stickHoldTime <= 0.25f;
					if (flag3)
					{
						UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().AddPermanentTips(this);
					}
				}
				GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
				bool flag4 = settings.TipsAutoFixed && this.CanStick && this._autoFixedActive && this.Element.Ready;
				if (flag4)
				{
					bool flag5 = Time.unscaledTime - this._autoFixedStartTime >= settings.TipsAutoFixedTime;
					if (flag5)
					{
						this._autoFixedActive = false;
						this.UpdatePos();
						UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().AddPermanentTips(this);
					}
				}
			}
			bool flag6 = stickCheckPass && !this._lastStickCheckPass;
			if (flag6)
			{
				this._stickHoldTime = Time.time;
			}
			this._lastStickCheckPass = stickCheckPass;
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0001D784 File Offset: 0x0001B984
	public void StartAutoFixedTimer()
	{
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		bool flag = !settings.TipsAutoFixed || !this.CanStick;
		if (!flag)
		{
			this._autoFixedStartTime = Time.unscaledTime;
			this._autoFixedActive = true;
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0001D7C4 File Offset: 0x0001B9C4
	public void StopAutoFixedTimer()
	{
		this._autoFixedActive = false;
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0001D7CE File Offset: 0x0001B9CE
	public void EnableSelfRefreshPosition()
	{
		this._canRefreshPosition = true;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0001D7D8 File Offset: 0x0001B9D8
	protected void SetAllowOverlapLayout(bool allow)
	{
		this._allowOverlapLayout = allow;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001D7E4 File Offset: 0x0001B9E4
	private Vector2 GetBaseTipPos()
	{
		RectTransform rectTransform = base.GetComponent<RectTransform>();
		Canvas rootCanvas = this.GetRootCanvas(rectTransform);
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera, out localPoint);
		return new Vector2(localPoint.x + (float)(AspectRatioController.ViewSize.x / 2), localPoint.y - (float)(AspectRatioController.ViewSize.y / 2));
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001D85C File Offset: 0x0001BA5C
	private Canvas GetRootCanvas(Transform myTransform)
	{
		bool flag = this._rootCanvas != null;
		Canvas result;
		if (flag)
		{
			result = this._rootCanvas;
		}
		else
		{
			Transform current = myTransform;
			Canvas rootCanvas = null;
			while (current != null)
			{
				Canvas canvas = current.GetComponent<Canvas>();
				bool flag2 = canvas != null;
				if (flag2)
				{
					CanvasScaler scaler = current.GetComponent<CanvasScaler>();
					bool flag3 = scaler != null;
					if (flag3)
					{
						rootCanvas = canvas;
						break;
					}
					bool flag4 = rootCanvas == null;
					if (flag4)
					{
						rootCanvas = canvas;
					}
				}
				current = current.parent;
			}
			this._rootCanvas = rootCanvas;
			result = rootCanvas;
		}
		return result;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001D8F4 File Offset: 0x0001BAF4
	public void UpdatePos()
	{
		bool useCustomBounds = this._useCustomBounds;
		float leftBound;
		float rightBound;
		float topBound;
		float bottomBound;
		if (useCustomBounds)
		{
			leftBound = (this._considerCustomLeft ? this._customScreenBounds.x : 0f);
			rightBound = (this._considerCustomRight ? (this._customScreenBounds.x + this._customScreenBounds.width) : this._screenWidth);
			topBound = (this._considerCustomTop ? (this._customScreenBounds.y + this._customScreenBounds.height) : 0f);
			bottomBound = (this._considerCustomBottom ? this._customScreenBounds.y : -1440f);
		}
		else
		{
			leftBound = 0f;
			rightBound = this._screenWidth;
			topBound = 0f;
			bottomBound = -1440f;
		}
		Vector2 tipsPos = this.GetBaseTipPos();
		bool isFixedPos = this.TipFixedPosX != 0f;
		bool flag = isFixedPos;
		if (flag)
		{
			tipsPos.x = this.TipFixedPosX;
		}
		RectTransform tipTransform = base.GetComponent<RectTransform>();
		Vector2 tipSize = tipTransform.sizeDelta;
		tipSize.x += this.TipAdditionSizeX;
		bool spaceAbove = tipsPos.y + tipSize.y <= topBound;
		bool spaceBelow = tipsPos.y - tipSize.y >= bottomBound;
		bool showOnTop = this.ShowOnTop ? (spaceAbove || !spaceBelow) : (!spaceBelow && spaceAbove);
		bool heightOverflow = !(showOnTop ? spaceAbove : spaceBelow);
		bool spaceOnLeft = tipsPos.x - tipSize.x >= leftBound;
		bool spaceOnRight = tipsPos.x + tipSize.x + 45f <= rightBound;
		bool showOnLeft = this.ShowOnLeft ? (spaceOnLeft || !spaceOnRight) : (!spaceOnRight && spaceOnLeft);
		bool flag2 = heightOverflow;
		if (flag2)
		{
			tipsPos.y = (showOnTop ? (topBound - tipSize.y) : (bottomBound + tipSize.y));
		}
		bool flag3 = !showOnLeft && !isFixedPos;
		if (flag3)
		{
			tipsPos.x += 45f;
		}
		tipTransform.anchorMin = new Vector2(0f, 1f);
		tipTransform.anchorMax = new Vector2(0f, 1f);
		tipTransform.pivot = new Vector2((float)(showOnLeft ? 1 : 0), (float)(showOnTop ? 0 : 1));
		bool flag4 = !isFixedPos;
		if (flag4)
		{
			bool flag5 = showOnLeft;
			if (flag5)
			{
				tipsPos += (showOnTop ? this.LeftUpOffsetPos : this.LeftDownOffsetPos);
			}
			else
			{
				tipsPos += (showOnTop ? this.RightUpOffsetPos : this.RightDownOffsetPos);
			}
		}
		bool allowOverlapLayout = this._allowOverlapLayout;
		if (allowOverlapLayout)
		{
			bool flag6 = showOnLeft;
			if (flag6)
			{
				float tipLeft = tipsPos.x - tipSize.x;
				bool flag7 = tipLeft < leftBound;
				if (flag7)
				{
					tipsPos.x = leftBound + tipSize.x;
				}
			}
			else
			{
				float tipRight = tipsPos.x + tipSize.x;
				bool flag8 = tipRight > rightBound;
				if (flag8)
				{
					tipsPos.x = rightBound - tipSize.x;
				}
			}
			bool flag9 = showOnTop;
			if (flag9)
			{
				float tipTop = tipsPos.y + tipSize.y;
				bool flag10 = tipTop > topBound;
				if (flag10)
				{
					tipsPos.y = topBound - tipSize.y;
				}
			}
			else
			{
				float tipBottom = tipsPos.y - tipSize.y;
				bool flag11 = tipBottom < bottomBound;
				if (flag11)
				{
					tipsPos.y = bottomBound + tipSize.y;
				}
			}
		}
		tipTransform.anchoredPosition = tipsPos;
		Action<bool> onUpdatePos = this.OnUpdatePos;
		if (onUpdatePos != null)
		{
			onUpdatePos(showOnLeft);
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0001DC97 File Offset: 0x0001BE97
	public virtual void UpdateOffsetPos()
	{
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0001DC9C File Offset: 0x0001BE9C
	public void MoveOut()
	{
		RectTransform tipTransform = base.GetComponent<RectTransform>();
		tipTransform.anchoredPosition = new Vector2(4000f, 4000f);
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0001DCC7 File Offset: 0x0001BEC7
	public virtual void SetNewData(ArgumentBox argsBox)
	{
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0001DCCC File Offset: 0x0001BECC
	protected void SetItemDesc(string originDesc, LoveTokenDataItem loveTokenDataItem)
	{
		bool isValid = loveTokenDataItem.IsValid;
		if (isValid)
		{
			this.GetLoveTokenDesc(loveTokenDataItem, delegate(string loveTokenDesc)
			{
				MouseTip_Util.SetMultiLineAutoHeightText(this.CGet<TextMeshProUGUI>("Desc"), originDesc + "\n" + loveTokenDesc);
			});
		}
		else
		{
			MouseTip_Util.SetMultiLineAutoHeightText(base.CGet<TextMeshProUGUI>("Desc"), originDesc);
		}
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x0001DD28 File Offset: 0x0001BF28
	protected void GetLoveTokenDesc(LoveTokenDataItem loveTokenDataItem, Action<string> action)
	{
		bool flag = !loveTokenDataItem.IsValid;
		if (!flag)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>(2)
			{
				loveTokenDataItem.TaiwuCharId,
				loveTokenDataItem.LoverCharId
			}, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayData> displayDataList = null;
				Serializer.Deserialize(dataPool, offset, ref displayDataList);
				CharacterDisplayData loverCharacterDisplayData = displayDataList[1];
				string taiwuName = NameCenter.GetNameByDisplayData(displayDataList[0], true, false).SetColor("brightred");
				string loverName = NameCenter.GetNameByDisplayData(loverCharacterDisplayData, false, false).SetColor((loverCharacterDisplayData.AliveState == 0) ? "brightred" : "red");
				string timeDesc = SingletonObject.getInstance<TimeManager>().GetDateDisplayContent(loveTokenDataItem.BecomeLoverTime);
				string str = loveTokenDataItem.IsTaiwuPresent ? LocalStringManager.GetFormat(LanguageKey.LK_Item_LoveToken, timeDesc, taiwuName, loverName) : LocalStringManager.GetFormat(LanguageKey.LK_Item_LoveToken, timeDesc, loverName, taiwuName);
				str = str.SetColor("brightblue");
				action(str);
			});
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001DD9B File Offset: 0x0001BF9B
	protected static string GetPoisonBigIcon(sbyte type)
	{
		return string.Format("mousetip_duxing_big_{0}", type);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001DDAD File Offset: 0x0001BFAD
	protected static string GetPoisonLevelIcon(sbyte level)
	{
		return string.Format("mousetip_duliang_{0}", level);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001DDBF File Offset: 0x0001BFBF
	protected virtual void OnEnable()
	{
		GEvent.OnEvent(UiEvents.MouseTipBaseOnEnable, null);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001DDD3 File Offset: 0x0001BFD3
	protected virtual void OnDisable()
	{
		GEvent.OnEvent(UiEvents.MouseTipBaseOnDisable, null);
	}

	// Token: 0x040002C8 RID: 712
	private float _screenWidth = 2560f;

	// Token: 0x040002C9 RID: 713
	public bool NeedRefresh;

	// Token: 0x040002CA RID: 714
	public bool ShowOnLeft = false;

	// Token: 0x040002CB RID: 715
	public bool ShowOnTop = false;

	// Token: 0x040002CC RID: 716
	private const int TipRefreshInterval = 1;

	// Token: 0x040002CD RID: 717
	private float _lastRefreshTime;

	// Token: 0x040002CE RID: 718
	public bool HasStick;

	// Token: 0x040002CF RID: 719
	protected GameObject EmptyRaycastTarget;

	// Token: 0x040002D0 RID: 720
	private bool _canRefreshPosition = false;

	// Token: 0x040002D1 RID: 721
	protected float TipAdditionSizeX;

	// Token: 0x040002D2 RID: 722
	protected Action<bool> OnUpdatePos;

	// Token: 0x040002D3 RID: 723
	protected float TipFixedPosX;

	// Token: 0x040002D4 RID: 724
	[NonSerialized]
	public Vector2 RightDownOffsetPos;

	// Token: 0x040002D5 RID: 725
	[NonSerialized]
	public Vector2 RightUpOffsetPos;

	// Token: 0x040002D6 RID: 726
	[NonSerialized]
	public Vector2 LeftUpOffsetPos;

	// Token: 0x040002D7 RID: 727
	[NonSerialized]
	public Vector2 LeftDownOffsetPos;

	// Token: 0x040002D8 RID: 728
	private bool _useCustomBounds;

	// Token: 0x040002D9 RID: 729
	private Rect _customScreenBounds;

	// Token: 0x040002DA RID: 730
	private bool _considerCustomRight;

	// Token: 0x040002DB RID: 731
	private bool _considerCustomTop;

	// Token: 0x040002DC RID: 732
	private bool _considerCustomLeft;

	// Token: 0x040002DD RID: 733
	private bool _considerCustomBottom;

	// Token: 0x040002DE RID: 734
	private bool _allowOverlapLayout;

	// Token: 0x040002DF RID: 735
	private float _stickHoldTime = 0f;

	// Token: 0x040002E0 RID: 736
	private bool _lastStickCheckPass = false;

	// Token: 0x040002E1 RID: 737
	private float _autoFixedStartTime = float.NegativeInfinity;

	// Token: 0x040002E2 RID: 738
	private bool _autoFixedActive;

	// Token: 0x040002E3 RID: 739
	private Canvas _rootCanvas;
}
