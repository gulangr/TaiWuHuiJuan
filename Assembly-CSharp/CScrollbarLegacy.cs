using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000CF RID: 207
[Obsolete]
public class CScrollbarLegacy : Scrollbar, IPointerClickHandler, IEventSystemHandler, IEndDragHandler
{
	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000715 RID: 1813 RVA: 0x00031785 File Offset: 0x0002F985
	// (set) Token: 0x06000716 RID: 1814 RVA: 0x0003178D File Offset: 0x0002F98D
	public float HandleSize
	{
		get
		{
			return base.size;
		}
		set
		{
			base.size = Mathf.Clamp(value, 0.06f, 1f);
			this.UpdateHandleMarkVisibility();
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000717 RID: 1815 RVA: 0x000317B0 File Offset: 0x0002F9B0
	// (set) Token: 0x06000718 RID: 1816 RVA: 0x000317C8 File Offset: 0x0002F9C8
	public new bool interactable
	{
		get
		{
			return base.interactable;
		}
		set
		{
			base.interactable = value;
			this.UpdateHandleMark();
		}
	}

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000719 RID: 1817 RVA: 0x000317DC File Offset: 0x0002F9DC
	// (remove) Token: 0x0600071A RID: 1818 RVA: 0x00031814 File Offset: 0x0002FA14
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<float> OnClickSetValueEvent;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x0600071B RID: 1819 RVA: 0x0003184C File Offset: 0x0002FA4C
	// (remove) Token: 0x0600071C RID: 1820 RVA: 0x00031884 File Offset: 0x0002FA84
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnMouseBtnDownEvent;

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x0600071D RID: 1821 RVA: 0x000318BC File Offset: 0x0002FABC
	// (remove) Token: 0x0600071E RID: 1822 RVA: 0x000318F4 File Offset: 0x0002FAF4
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnMouseBtnUpEvent;

	// Token: 0x0600071F RID: 1823 RVA: 0x0003192C File Offset: 0x0002FB2C
	protected override void Awake()
	{
		base.Awake();
		bool flag = null == this.ScrollContainer;
		if (flag)
		{
			Transform transform = base.transform.Find("ScrollContainer");
			this.ScrollContainer = ((transform != null) ? transform.GetComponent<RectTransform>() : null);
		}
		bool flag2 = null == this.ButtonMin;
		if (flag2)
		{
			this.ButtonMin = (this.ScrollContainer ?? base.transform).Find("BtnMin").GetComponent<CButtonObsolete>();
		}
		bool flag3 = null == this.ButtonMax;
		if (flag3)
		{
			this.ButtonMax = (this.ScrollContainer ?? base.transform).Find("BtnMax").GetComponent<CButtonObsolete>();
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x000319E0 File Offset: 0x0002FBE0
	protected override void Start()
	{
		base.Start();
		this.ButtonMin.ClearAndAddListener(delegate
		{
			base.value = 0f;
			Action<float> onClickSetValueEvent = this.OnClickSetValueEvent;
			if (onClickSetValueEvent != null)
			{
				onClickSetValueEvent(base.value);
			}
			this.ButtonMin.interactable = false;
		});
		this.ButtonMax.ClearAndAddListener(delegate
		{
			base.value = 1f;
			Action<float> onClickSetValueEvent = this.OnClickSetValueEvent;
			if (onClickSetValueEvent != null)
			{
				onClickSetValueEvent(base.value);
			}
			this.ButtonMax.interactable = false;
		});
		base.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollValueChanged));
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00031A40 File Offset: 0x0002FC40
	protected override void OnEnable()
	{
		bool resetValueOnStart = this.ResetValueOnStart;
		if (resetValueOnStart)
		{
			base.value = 0f;
		}
		base.OnEnable();
		this.OnScrollValueChanged(base.value);
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00031A7C File Offset: 0x0002FC7C
	private void UpdateHandleMarkVisibility()
	{
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			bool flag2 = null == this.HandleMark;
			if (!flag2)
			{
				bool flag3 = base.direction == Scrollbar.Direction.BottomToTop || base.direction == Scrollbar.Direction.TopToBottom;
				if (flag3)
				{
					this.HandleMark.gameObject.SetActive(this.HandleMark.rect.height * 2f < base.handleRect.rect.height);
				}
				bool flag4 = base.direction == Scrollbar.Direction.LeftToRight || base.direction == Scrollbar.Direction.RightToLeft;
				if (flag4)
				{
					this.HandleMark.gameObject.SetActive(this.HandleMark.rect.width * 2f < base.handleRect.rect.width);
				}
			}
		}
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00031B68 File Offset: 0x0002FD68
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.dragging || eventData.button > PointerEventData.InputButton.Left;
		if (!flag)
		{
			RectTransform handleParentRect = base.handleRect.parent as RectTransform;
			bool flag2 = null == handleParentRect;
			if (!flag2)
			{
				bool flag3 = RectTransformUtility.RectangleContainsScreenPoint(handleParentRect, Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag3)
				{
					Vector2 localPos = UIManager.Instance.MousePosToLocalPos(handleParentRect);
					float clickValue = -1f;
					bool flag4 = base.direction == Scrollbar.Direction.LeftToRight;
					if (flag4)
					{
						clickValue = Mathf.Abs(localPos.x - handleParentRect.rect.xMin) / handleParentRect.rect.width;
					}
					else
					{
						bool flag5 = base.direction == Scrollbar.Direction.RightToLeft;
						if (flag5)
						{
							clickValue = Mathf.Abs(localPos.x - handleParentRect.rect.xMax) / handleParentRect.rect.width;
						}
						else
						{
							bool flag6 = base.direction == Scrollbar.Direction.TopToBottom;
							if (flag6)
							{
								clickValue = Mathf.Abs(localPos.y - handleParentRect.rect.yMax) / handleParentRect.rect.height;
							}
							else
							{
								bool flag7 = base.direction == Scrollbar.Direction.BottomToTop;
								if (flag7)
								{
									clickValue = Mathf.Abs(localPos.y - handleParentRect.rect.yMin) / handleParentRect.rect.height;
								}
							}
						}
					}
					base.value = clickValue;
					Action<float> onClickSetValueEvent = this.OnClickSetValueEvent;
					if (onClickSetValueEvent != null)
					{
						onClickSetValueEvent(base.value);
					}
				}
			}
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00031D09 File Offset: 0x0002FF09
	public override void OnPointerDown(PointerEventData eventData)
	{
		Action onMouseBtnDownEvent = this.OnMouseBtnDownEvent;
		if (onMouseBtnDownEvent != null)
		{
			onMouseBtnDownEvent();
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00031D1E File Offset: 0x0002FF1E
	public override void OnPointerUp(PointerEventData eventData)
	{
		Action onMouseBtnUpEvent = this.OnMouseBtnUpEvent;
		if (onMouseBtnUpEvent != null)
		{
			onMouseBtnUpEvent();
		}
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00031D34 File Offset: 0x0002FF34
	public void OnScrollValueChanged(float curValue)
	{
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			this.ButtonMax.interactable = (curValue < 0.995f);
			this.ButtonMin.interactable = (curValue > 0.005f);
			MonoJoint componentInChildren = this.ButtonMax.GetComponentInChildren<MonoJoint>(true);
			if (componentInChildren != null)
			{
				componentInChildren.JointSync();
			}
			MonoJoint componentInChildren2 = this.ButtonMin.GetComponentInChildren<MonoJoint>(true);
			if (componentInChildren2 != null)
			{
				componentInChildren2.JointSync();
			}
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00031DA8 File Offset: 0x0002FFA8
	public void UpdateHandleMark()
	{
		CImage handleMarkImage = this.HandleMark.GetComponent<CImage>();
		bool flag = null != handleMarkImage;
		if (flag)
		{
			handleMarkImage.SetSprite(this.interactable ? this.NormalHandleMark : this.DisableHandleMark, false, null);
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00031DEC File Offset: 0x0002FFEC
	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		bool flag = this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		if (flag)
		{
			this._draggingHandle = true;
		}
		this.CheckHandleHover();
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00031E32 File Offset: 0x00030032
	public void OnEndDrag(PointerEventData eventData)
	{
		this._draggingHandle = false;
		this.CheckHandleHover();
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00031E43 File Offset: 0x00030043
	public void SetHandleHover(bool isHovering)
	{
		this._handleHovering = isHovering;
		this.CheckHandleHover();
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00031E54 File Offset: 0x00030054
	private void CheckHandleHover()
	{
		bool flag = this.HandleHoverObj == null;
		if (flag)
		{
			Debug.LogWarning("未绑定HandleHoverObj");
		}
		else
		{
			this.HandleHoverObj.SetActive(this._draggingHandle || this._handleHovering);
		}
	}

	// Token: 0x04000779 RID: 1913
	[SerializeField]
	public bool ResetValueOnStart;

	// Token: 0x0400077A RID: 1914
	public GameObject HandleHoverObj;

	// Token: 0x0400077E RID: 1918
	public RectTransform HandleMark;

	// Token: 0x0400077F RID: 1919
	public RectTransform ScrollContainer;

	// Token: 0x04000780 RID: 1920
	public CButtonObsolete ButtonMin;

	// Token: 0x04000781 RID: 1921
	public CButtonObsolete ButtonMax;

	// Token: 0x04000782 RID: 1922
	public string DisableHandleMark = "sp_gn_gundong_8";

	// Token: 0x04000783 RID: 1923
	public string NormalHandleMark = "sp_gn_gundong_7";

	// Token: 0x04000784 RID: 1924
	private bool _draggingHandle = false;

	// Token: 0x04000785 RID: 1925
	private bool _handleHovering = false;
}
