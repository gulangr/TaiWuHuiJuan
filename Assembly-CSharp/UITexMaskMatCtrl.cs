using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B0 RID: 176
[ExecuteAlways]
[RequireComponent(typeof(Graphic))]
public class UITexMaskMatCtrl : MonoBehaviour
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000605 RID: 1541 RVA: 0x000287D3 File Offset: 0x000269D3
	// (set) Token: 0x06000606 RID: 1542 RVA: 0x000287DB File Offset: 0x000269DB
	public Vector2 MaskSize
	{
		get
		{
			return this.m_MaskSize;
		}
		set
		{
			this.m_MaskSize = value;
			this.UpdateMat();
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000607 RID: 1543 RVA: 0x000287EC File Offset: 0x000269EC
	// (set) Token: 0x06000608 RID: 1544 RVA: 0x000287F4 File Offset: 0x000269F4
	public Vector2 MaskPositionInRect
	{
		get
		{
			return this.m_MaskPositionInRect;
		}
		set
		{
			this.m_MaskPositionInRect = value;
			this.UpdateMat();
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00028805 File Offset: 0x00026A05
	private void OnEnable()
	{
		this.TryInit();
		this.UpdateMat();
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00028818 File Offset: 0x00026A18
	private void TryInit()
	{
		this.m_LastMaskCenterWorldPos = null;
		this.m_IsInitCorrect = false;
		this.m_Graphic = base.GetComponent<Graphic>();
		bool flag = this.m_Graphic == null;
		if (!flag)
		{
			Material mat = this.m_Graphic.material;
			this.m_IsInitCorrect = (mat != null && mat.shader != null && mat.shader.name == "PBBox/UI/CustomTexMaskable");
		}
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x0002889C File Offset: 0x00026A9C
	private void Update()
	{
		bool flag = this.m_MaskCenterTrans == null;
		if (!flag)
		{
			bool flag2 = this.m_LastMaskCenterWorldPos != null && this.m_LastMaskCenterWorldPos.Value == this.m_MaskCenterTrans.position;
			if (!flag2)
			{
				this.SetMaskPosition(this.m_MaskCenterTrans);
				this.m_LastMaskCenterWorldPos = new Vector3?(this.m_MaskCenterTrans.position);
			}
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00028914 File Offset: 0x00026B14
	private void UpdateMat()
	{
		bool flag = !this.m_IsInitCorrect;
		if (!flag)
		{
			Rect rect = this.m_Graphic.rectTransform.rect;
			float uVTilingX = rect.width / this.m_MaskSize.x;
			float uVTilingY = rect.height / this.m_MaskSize.y;
			uVTilingX = (Mathf.Approximately(uVTilingX, 0f) ? 1f : uVTilingX);
			uVTilingY = (Mathf.Approximately(uVTilingY, 0f) ? 1f : uVTilingY);
			Vector2 uVTiling = new Vector2(uVTilingX, uVTilingY);
			float uVOffsetX = Mathf.Approximately(rect.width, 0f) ? 0f : (this.m_MaskPositionInRect.x / rect.width - 0.5f / uVTiling.x);
			float uVOffsetY = Mathf.Approximately(rect.height, 0f) ? 0f : (this.m_MaskPositionInRect.y / rect.height - 0.5f / uVTiling.y);
			Vector2 uVOffset = new Vector2(uVOffsetX, uVOffsetY);
			uVOffset.x *= -uVTiling.x;
			uVOffset.y *= -uVTiling.y;
			this.m_Graphic.material.SetVector(UITexMaskMatCtrl.ShaderPropID_MaskTex_ST, new Vector4(uVTiling.x, uVTiling.y, uVOffset.x, uVOffset.y));
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00028A80 File Offset: 0x00026C80
	public void SetMaskPosition(Transform centerTrans)
	{
		bool flag = centerTrans == null;
		if (!flag)
		{
			this.SetMaskPosition(centerTrans.position);
		}
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00028AAC File Offset: 0x00026CAC
	public void SetMaskPosition(Vector3 worldPositon)
	{
		bool flag = this.m_Graphic == null;
		if (flag)
		{
			this.TryInit();
			bool flag2 = this.m_Graphic == null;
			if (flag2)
			{
				return;
			}
		}
		RectTransform rectTrans = this.m_Graphic.rectTransform;
		Vector3 localPosition = rectTrans.InverseTransformPoint(worldPositon);
		this.MaskPositionInRect = new Vector2(localPosition.x - rectTrans.rect.x, localPosition.y - rectTrans.rect.y);
	}

	// Token: 0x040004EF RID: 1263
	private Graphic m_Graphic;

	// Token: 0x040004F0 RID: 1264
	private bool m_IsInitCorrect;

	// Token: 0x040004F1 RID: 1265
	private static readonly int ShaderPropID_MaskTex_ST = Shader.PropertyToID("_MaskTex_ST");

	// Token: 0x040004F2 RID: 1266
	[SerializeField]
	private Vector2 m_MaskSize = Vector2.one * 100f;

	// Token: 0x040004F3 RID: 1267
	[SerializeField]
	private Vector2 m_MaskPositionInRect = Vector2.zero;

	// Token: 0x040004F4 RID: 1268
	[Tooltip("遮罩中心位置的Transform，优先级高于MaskPositionInRect")]
	[SerializeField]
	private Transform m_MaskCenterTrans;

	// Token: 0x040004F5 RID: 1269
	private Vector3? m_LastMaskCenterWorldPos = null;
}
