using System;
using FrameWork;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class TooltipInvoker : MonoBehaviour
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060004A3 RID: 1187 RVA: 0x0001F430 File Offset: 0x0001D630
	// (set) Token: 0x060004A4 RID: 1188 RVA: 0x0001F448 File Offset: 0x0001D648
	public ArgumentBox RuntimeParam
	{
		get
		{
			return this._runtimeParam;
		}
		set
		{
			this._runtimeParam = value;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0001F454 File Offset: 0x0001D654
	public bool Showing
	{
		get
		{
			bool flag = base.gameObject;
			return flag && base.enabled && SingletonObject.getInstance<TooltipManager>().IsCurrMouseOverObj(base.gameObject);
		}
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001F494 File Offset: 0x0001D694
	public void Refresh(bool forceUseHideAndShow = false, int encyclopediaType = -1)
	{
		bool flag = !this.Showing;
		if (!flag)
		{
			MouseTipBase tip = SingletonObject.getInstance<TooltipManager>().GetTipsUi(this.Type);
			bool flag2 = !forceUseHideAndShow;
			if (flag2)
			{
				ArgumentBox argsBox = this.GetArgumentBox();
				bool flag3 = encyclopediaType >= 0;
				if (flag3)
				{
					argsBox.Set("EncyclopediaLink", encyclopediaType);
				}
				if (tip != null)
				{
					tip.Refresh(argsBox);
				}
			}
			else
			{
				this.HideTips();
				this.ShowTips();
				bool flag4 = tip;
				if (flag4)
				{
					tip.transform.localScale = Vector3.one;
				}
			}
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001F52C File Offset: 0x0001D72C
	public bool ShowTips()
	{
		bool globalTipsHide = SingletonObject.getInstance<GlobalSettings>().GlobalTipsHide;
		bool result;
		if (globalTipsHide)
		{
			result = false;
		}
		else
		{
			ArgumentBox argsBox = this.GetArgumentBox();
			bool flag = this.encyclopediaConfigTypeId > 0;
			if (flag)
			{
				argsBox.Set("EncyclopediaLink", this.encyclopediaConfigTypeId);
			}
			this._offsetPosArray[0] = this.RightDownOffsetPos;
			this._offsetPosArray[1] = this.RightUpOffsetPos;
			this._offsetPosArray[2] = this.LeftUpOffsetPos;
			this._offsetPosArray[3] = this.LeftDownOffsetPos;
			bool useCustomBounds = this.CustomBoundsTransform != null;
			argsBox.Set("_useCustomBounds", useCustomBounds);
			bool flag2 = useCustomBounds;
			if (flag2)
			{
				Canvas tipCanvas = this.FindRootCanvas(this.CustomBoundsTransform);
				bool flag3 = tipCanvas != null;
				if (flag3)
				{
					Transform rootCanvas = tipCanvas.transform;
					RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();
					Vector2 canvasSize = canvasRect.sizeDelta;
					Vector3[] corners = new Vector3[4];
					this.CustomBoundsTransform.GetWorldCorners(corners);
					Vector2 bottomLeftCorner = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, corners[0]);
					Vector2 bottomLeftCornerLocalPos;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, bottomLeftCorner, UIManager.Instance.UiCamera, out bottomLeftCornerLocalPos);
					Vector2 bottomLeftResult = new Vector2(bottomLeftCornerLocalPos.x + canvasSize.x * 0.5f, canvasSize.y * 0.5f - bottomLeftCornerLocalPos.y);
					Vector2 topRightCorner = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, corners[2]);
					Vector2 topRightCornerLocalPos;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, topRightCorner, UIManager.Instance.UiCamera, out topRightCornerLocalPos);
					Vector2 topRightResult = new Vector2(topRightCornerLocalPos.x + canvasSize.x * 0.5f, canvasSize.y * 0.5f - topRightCornerLocalPos.y);
					Rect rect = new Rect(bottomLeftResult.x, bottomLeftResult.y, topRightResult.x - bottomLeftResult.x, topRightResult.y - bottomLeftResult.y);
					argsBox.SetObject("_customScreenBounds", rect);
					argsBox.Set("_considerCustomRight", this._considerCustomRight);
					argsBox.Set("_considerCustomTop", this._considerCustomTop);
					argsBox.Set("_considerCustomLeft", this._considerCustomLeft);
					argsBox.Set("_considerCustomBottom", this._considerCustomBottom);
				}
				else
				{
					argsBox.Set("_useCustomBounds", false);
				}
			}
			SingletonObject.getInstance<TooltipManager>().ShowTips(this.Type, argsBox, this.NeedRefresh, this.ShowOnLeft, this.ShowOnTop, this._offsetPosArray);
			result = true;
		}
		return result;
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001F7C5 File Offset: 0x0001D9C5
	public void HideTips()
	{
		SingletonObject.getInstance<TooltipManager>().HideTips(this.Type, true);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001F7DC File Offset: 0x0001D9DC
	private ArgumentBox GetArgumentBox()
	{
		ArgumentBox argsBox = this.RuntimeParam;
		bool flag = argsBox == null;
		if (flag)
		{
			argsBox = EasyPool.Get<ArgumentBox>();
			for (int i = 0; i < this.PresetParam.Length; i++)
			{
				argsBox.Set(string.Format("arg{0}", i), this.IsLanguageKey ? LocalStringManager.Get(this.PresetParam[i]).ColorReplace() : this.PresetParam[i]);
			}
		}
		return argsBox;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001F85C File Offset: 0x0001DA5C
	private Canvas FindRootCanvas(RectTransform rectTransform)
	{
		Transform current = rectTransform;
		Canvas rootCanvas = null;
		while (current != null)
		{
			Canvas canvas = current.GetComponent<Canvas>();
			bool flag = canvas != null;
			if (flag)
			{
				bool isRootCanvas = canvas.isRootCanvas;
				if (isRootCanvas)
				{
					rootCanvas = canvas;
					break;
				}
				bool flag2 = rootCanvas == null;
				if (flag2)
				{
					rootCanvas = canvas;
				}
			}
			current = current.parent;
		}
		return rootCanvas;
	}

	// Token: 0x040002F3 RID: 755
	public TipType Type;

	// Token: 0x040002F4 RID: 756
	public bool IsLanguageKey;

	// Token: 0x040002F5 RID: 757
	public bool NeedRefresh;

	// Token: 0x040002F6 RID: 758
	public bool ShowOnLeft = false;

	// Token: 0x040002F7 RID: 759
	public bool ShowOnTop = false;

	// Token: 0x040002F8 RID: 760
	public string[] PresetParam;

	// Token: 0x040002F9 RID: 761
	[Tooltip("勾选的话，子物体的射线检测会触发显示Tip")]
	public bool triggerByChildRaycast = false;

	// Token: 0x040002FA RID: 762
	public Vector2 RightDownOffsetPos;

	// Token: 0x040002FB RID: 763
	public Vector2 RightUpOffsetPos;

	// Token: 0x040002FC RID: 764
	public Vector2 LeftUpOffsetPos;

	// Token: 0x040002FD RID: 765
	public Vector2 LeftDownOffsetPos;

	// Token: 0x040002FE RID: 766
	[Header("Custom Screen Bounds")]
	[Tooltip("如果不为空，则将此物体的屏幕范围作为Tips的边界")]
	public RectTransform CustomBoundsTransform;

	// Token: 0x040002FF RID: 767
	public bool _considerCustomRight;

	// Token: 0x04000300 RID: 768
	public bool _considerCustomTop;

	// Token: 0x04000301 RID: 769
	public bool _considerCustomLeft;

	// Token: 0x04000302 RID: 770
	public bool _considerCustomBottom;

	// Token: 0x04000303 RID: 771
	public int encyclopediaConfigTypeId = -1;

	// Token: 0x04000304 RID: 772
	private readonly Vector2[] _offsetPosArray = new Vector2[4];

	// Token: 0x04000305 RID: 773
	private ArgumentBox _runtimeParam;
}
