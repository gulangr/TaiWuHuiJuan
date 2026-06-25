using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000078 RID: 120
[ExecuteInEditMode]
public class MaskImage : CImage
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000453 RID: 1107 RVA: 0x0001C8C4 File Offset: 0x0001AAC4
	public override Material materialForRendering
	{
		get
		{
			bool flag = null == this.MaskTexture || null == base.sprite;
			Material result;
			if (flag)
			{
				result = base.materialForRendering;
			}
			else
			{
				Material curMat = this.material;
				bool flag2 = null == curMat || curMat.name != "MaskMat_" + base.GetInstanceID().ToString();
				if (flag2)
				{
					curMat = new Material(Shader.Find(this.maskShaderName));
					curMat = this.GetModifiedMaterial(curMat);
					curMat.name = "MaskMat_" + base.GetInstanceID().ToString();
				}
				result = curMat;
			}
			return result;
		}
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0001C978 File Offset: 0x0001AB78
	protected override void OnEnable()
	{
		this.material = this.materialForRendering;
		this.UpdateMask();
		base.OnEnable();
		RectMask2D mask2D = base.GetComponentInParent<RectMask2D>();
		bool flag = mask2D;
		if (flag)
		{
			mask2D.AddClippable(this);
		}
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001C9BA File Offset: 0x0001ABBA
	protected override void OnRectTransformDimensionsChange()
	{
		this.UpdateRect();
		base.OnRectTransformDimensionsChange();
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0001C9CC File Offset: 0x0001ABCC
	public override void Cull(Rect clipRect, bool validRect)
	{
		base.Cull(clipRect, validRect);
		bool flag = this.material.shader.name == this.maskShaderName;
		if (flag)
		{
			this.material.SetVector("_ClipRect", new Vector4(clipRect.xMin, clipRect.yMin, clipRect.xMax, clipRect.yMax));
		}
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0001CA38 File Offset: 0x0001AC38
	public void UpdateMask()
	{
		bool flag = null == base.sprite;
		if (!flag)
		{
			bool flag2 = this.material.shader.name == this.maskShaderName;
			if (flag2)
			{
				this.material.SetFloat("_TexWidth", (float)base.sprite.texture.width);
				this.material.SetFloat("_TexHeight", (float)base.sprite.texture.height);
				this.material.SetFloat("xOffset", this.Offset.x);
				this.material.SetFloat("yOffset", this.Offset.y);
				this.material.SetVector("_SpriteInfo", new Vector4(base.sprite.rect.x, base.sprite.rect.y, base.sprite.rect.width, base.sprite.rect.height));
				this.material.SetTexture("_MaskTex", this.MaskTexture);
			}
			this.UpdateRect();
		}
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x0001CB7C File Offset: 0x0001AD7C
	public void UpdateRect()
	{
		bool flag = this.material.shader.name == this.maskShaderName;
		if (flag)
		{
			Vector2 intent = this.MaskSize;
			bool flag2 = intent.x == 0f;
			if (flag2)
			{
				intent.x = base.rectTransform.rect.size.x;
			}
			bool flag3 = intent.y == 0f;
			if (flag3)
			{
				intent.y = base.rectTransform.rect.size.y;
			}
			this.material.SetVector("_RectInfo", new Vector4(base.rectTransform.rect.width, base.rectTransform.rect.height, intent.x, intent.y));
		}
	}

	// Token: 0x040002BB RID: 699
	public Texture2D MaskTexture;

	// Token: 0x040002BC RID: 700
	public Vector2 MaskSize;

	// Token: 0x040002BD RID: 701
	public Vector2 Offset;

	// Token: 0x040002BE RID: 702
	private readonly string maskShaderName = "ConchShip/UIEffect/SpriteMask";
}
