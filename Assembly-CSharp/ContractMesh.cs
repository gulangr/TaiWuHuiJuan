using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000053 RID: 83
public class ContractMesh : MaskableGraphic
{
	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060002C7 RID: 711 RVA: 0x00010C28 File Offset: 0x0000EE28
	// (set) Token: 0x060002C8 RID: 712 RVA: 0x00010C30 File Offset: 0x0000EE30
	public float Progress
	{
		get
		{
			return this._Progress;
		}
		set
		{
			bool flag = Math.Abs(this._Progress - value) <= float.Epsilon;
			if (!flag)
			{
				this._Progress = value;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060002C9 RID: 713 RVA: 0x00010C70 File Offset: 0x0000EE70
	// (set) Token: 0x060002CA RID: 714 RVA: 0x00010C78 File Offset: 0x0000EE78
	public Texture Texture
	{
		get
		{
			return this._Texture;
		}
		set
		{
			bool flag = this._Texture == value;
			if (!flag)
			{
				this._Texture = value;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060002CB RID: 715 RVA: 0x00010CAD File Offset: 0x0000EEAD
	// (set) Token: 0x060002CC RID: 716 RVA: 0x00010CB5 File Offset: 0x0000EEB5
	public Vector2 NormalizedContractTarget
	{
		get
		{
			return this._NormalizedContractTarget;
		}
		set
		{
			this._NormalizedContractTarget = value;
			this.SetVerticesDirty();
			this.SetMaterialDirty();
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060002CD RID: 717 RVA: 0x00010CD0 File Offset: 0x0000EED0
	public override Texture mainTexture
	{
		get
		{
			bool flag = !(this._Texture == null);
			Texture result;
			if (flag)
			{
				result = this._Texture;
			}
			else
			{
				result = ((this.material != null && this.material.mainTexture != null) ? this.material.mainTexture : Graphic.s_WhiteTexture);
			}
			return result;
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00010D34 File Offset: 0x0000EF34
	public override void SetNativeSize()
	{
		Texture mainTexture = this.mainTexture;
		bool flag = !(mainTexture != null);
		if (!flag)
		{
			int num = Mathf.RoundToInt((float)mainTexture.width);
			int num2 = Mathf.RoundToInt((float)mainTexture.height);
			base.rectTransform.anchorMax = base.rectTransform.anchorMin;
			base.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00010DA0 File Offset: 0x0000EFA0
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		Texture mainTexture = this.mainTexture;
		vh.Clear();
		bool flag = !(mainTexture != null);
		if (!flag)
		{
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			Color color = this.color;
			int subStep = Mathf.Max(2, this._SubSplitted);
			bool flag2 = this._ContractOffsets == null || this._ContractOffsets.Length != subStep * subStep;
			if (flag2)
			{
				this._ContractOffsets = new Vector2[subStep * subStep];
			}
			float sqrt2 = Mathf.Sqrt(2f);
			float sqrt2Half = sqrt2 * 0.5f;
			for (int i = 0; i < subStep; i++)
			{
				Vector2 normalizedPoint = new Vector2(0f, 1f * (float)i / (float)(subStep - 1));
				for (int j = 0; j < subStep; j++)
				{
					normalizedPoint.x = 1f * (float)j / (float)(subStep - 1);
					Vector2 targetArrow = this._NormalizedContractTarget - normalizedPoint;
					float targetDistance = targetArrow.magnitude / sqrt2;
					this._ContractOffsets[i * subStep + j] = targetArrow.normalized * this._DistanceCurve.Evaluate(targetDistance) * this._ContractCurve.Evaluate(this._Progress) / sqrt2Half;
				}
			}
			for (int k = 0; k < subStep; k++)
			{
				float baseY = 1f * (float)k / (float)(subStep - 1);
				for (int l = 0; l < subStep; l++)
				{
					float baseX = 1f * (float)l / (float)(subStep - 1);
					Vector2 offset = this._ContractOffsets[k * subStep + l];
					Vector3 position = new Vector3(pixelAdjustedRect.x + baseX * pixelAdjustedRect.width + offset.x * pixelAdjustedRect.width, pixelAdjustedRect.y + baseY * pixelAdjustedRect.height + offset.y * pixelAdjustedRect.height);
					float scaleValue = this._ScaleCurve.Evaluate(this._Progress);
					position.x = Mathf.Lerp(position.x, pixelAdjustedRect.x + this._NormalizedContractTarget.x * pixelAdjustedRect.width, scaleValue);
					position.y = Mathf.Lerp(position.y, pixelAdjustedRect.y + this._NormalizedContractTarget.y * pixelAdjustedRect.height, scaleValue);
					position.x = Mathf.Clamp(position.x, pixelAdjustedRect.x, pixelAdjustedRect.x + pixelAdjustedRect.width);
					position.y = Mathf.Clamp(position.y, pixelAdjustedRect.y, pixelAdjustedRect.y + pixelAdjustedRect.height);
					vh.AddVert(position, color, new Vector2(baseX, baseY));
					bool flag3 = l > 0 && k > 0;
					if (flag3)
					{
						vh.AddTriangle((k - 1) * subStep + (l - 1), k * subStep + (l - 1), k * subStep + l);
						vh.AddTriangle(k * subStep + l, (k - 1) * subStep + l, (k - 1) * subStep + (l - 1));
					}
				}
			}
		}
	}

	// Token: 0x04000176 RID: 374
	[Range(0f, 1f)]
	[SerializeField]
	private float _Progress;

	// Token: 0x04000177 RID: 375
	[Range(2f, 128f)]
	[SerializeField]
	private int _SubSplitted = 8;

	// Token: 0x04000178 RID: 376
	[SerializeField]
	private Texture _Texture;

	// Token: 0x04000179 RID: 377
	[SerializeField]
	private Vector2 _NormalizedContractTarget;

	// Token: 0x0400017A RID: 378
	[SerializeField]
	private AnimationCurve _ContractCurve;

	// Token: 0x0400017B RID: 379
	[SerializeField]
	private AnimationCurve _DistanceCurve;

	// Token: 0x0400017C RID: 380
	[SerializeField]
	private AnimationCurve _ScaleCurve;

	// Token: 0x0400017D RID: 381
	private Vector2[] _ContractOffsets;
}
