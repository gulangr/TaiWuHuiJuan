using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CA RID: 202
public class CImage : Image
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x060006F4 RID: 1780 RVA: 0x0003088C File Offset: 0x0002EA8C
	// (remove) Token: 0x060006F5 RID: 1781 RVA: 0x000308C4 File Offset: 0x0002EAC4
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Graphic> OnUpdateMaterial;

	// Token: 0x060006F6 RID: 1782 RVA: 0x000308FC File Offset: 0x0002EAFC
	public void SetSprite(string spriteName = "", bool autoNativeSize = false, Action onSpriteChange = null)
	{
		bool destroyState = this._destroyState;
		if (!destroyState)
		{
			this.AutoSize = autoNativeSize;
			this.OnSpriteChange = onSpriteChange;
			bool flag = string.IsNullOrEmpty(spriteName);
			if (flag)
			{
				base.sprite = null;
				this.SetEnabled(false);
				Action onSpriteChange2 = this.OnSpriteChange;
				if (onSpriteChange2 != null)
				{
					onSpriteChange2();
				}
			}
			else
			{
				bool flag2 = SingletonObject.getInstance<DlcManager>().SetImageSprite(this, spriteName, true);
				if (flag2)
				{
					this.SetEnabled(true);
				}
				else
				{
					bool flag3 = AtlasInfo.Instance.SetImageSpriteOnly(this, spriteName);
					if (flag3)
					{
						this.SetEnabled(true);
					}
					else
					{
						this.SetEnabled(false);
					}
				}
			}
		}
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00030998 File Offset: 0x0002EB98
	public void SetSpriteOnly(string spriteName = "", bool autoNativeSize = false, Action onSpriteChange = null)
	{
		bool destroyState = this._destroyState;
		if (!destroyState)
		{
			this.AutoSize = autoNativeSize;
			this.OnSpriteChange = onSpriteChange;
			bool flag = string.IsNullOrEmpty(spriteName);
			if (flag)
			{
				base.sprite = null;
				Action onSpriteChange2 = this.OnSpriteChange;
				if (onSpriteChange2 != null)
				{
					onSpriteChange2();
				}
			}
			else
			{
				bool flag2 = SingletonObject.getInstance<DlcManager>().SetImageSprite(this, spriteName, true);
				if (!flag2)
				{
					bool flag3 = !AtlasInfo.Instance.SetImageSpriteOnly(this, spriteName);
					if (flag3)
					{
					}
				}
			}
		}
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00030A10 File Offset: 0x0002EC10
	public void Clear()
	{
		bool destroyState = this._destroyState;
		if (!destroyState)
		{
			base.sprite = null;
			this.SetEnabled(false);
		}
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00030A3C File Offset: 0x0002EC3C
	public CImage SetColor(Color newColor)
	{
		this.color = newColor;
		return this;
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00030A58 File Offset: 0x0002EC58
	public void SetAlpha(float alpha)
	{
		Color c = this.color;
		c.a = alpha;
		this.color = c;
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00030A7D File Offset: 0x0002EC7D
	protected override void Awake()
	{
		base.Awake();
		this._destroyState = false;
		this.canvasRenderer = base.GetComponent<CanvasRenderer>();
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00030A9A File Offset: 0x0002EC9A
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this._destroyState = true;
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x060006FD RID: 1789 RVA: 0x00030AAB File Offset: 0x0002ECAB
	public new int depth
	{
		get
		{
			return this.canvasRenderer.absoluteDepth;
		}
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00030AB8 File Offset: 0x0002ECB8
	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		bool flag = !this.raycastTarget;
		if (flag)
		{
			GraphicRegistry.UnregisterGraphicForCanvas(base.canvas, this);
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00030AE8 File Offset: 0x0002ECE8
	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		bool flag = !this.raycastTarget;
		if (flag)
		{
			GraphicRegistry.UnregisterGraphicForCanvas(base.canvas, this);
		}
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00030B18 File Offset: 0x0002ED18
	protected override void OnEnable()
	{
		base.OnEnable();
		bool flag = !this.raycastTarget;
		if (flag)
		{
			GraphicRegistry.UnregisterGraphicForCanvas(base.canvas, this);
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00030B47 File Offset: 0x0002ED47
	protected override void UpdateMaterial()
	{
		base.UpdateMaterial();
		Action<Graphic> onUpdateMaterial = this.OnUpdateMaterial;
		if (onUpdateMaterial != null)
		{
			onUpdateMaterial(this);
		}
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00030B64 File Offset: 0x0002ED64
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetEnabled(bool shouldBeEnabled)
	{
		base.enabled = shouldBeEnabled;
	}

	// Token: 0x04000770 RID: 1904
	[NonSerialized]
	public Action OnSpriteChange;

	// Token: 0x04000772 RID: 1906
	public bool AutoSize = false;

	// Token: 0x04000773 RID: 1907
	private bool _destroyState;

	// Token: 0x04000774 RID: 1908
	public new CanvasRenderer canvasRenderer;
}
