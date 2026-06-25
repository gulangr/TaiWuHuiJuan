using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CC RID: 204
public class CImagePinned : CImage, IMeshModifier
{
	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000708 RID: 1800 RVA: 0x00030CC2 File Offset: 0x0002EEC2
	// (set) Token: 0x06000709 RID: 1801 RVA: 0x00030CCA File Offset: 0x0002EECA
	public Vector2Int PinPixelOffset
	{
		get
		{
			return this.pinPixelOffset;
		}
		set
		{
			this.pinPixelOffset = value;
			this.SetVerticesDirty();
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00030CDC File Offset: 0x0002EEDC
	public void ModifyMesh(Mesh mesh)
	{
		using (VertexHelper vh = new VertexHelper(mesh))
		{
			this.ModifyMesh(vh);
			vh.FillMesh(mesh);
		}
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00030D20 File Offset: 0x0002EF20
	public void ModifyMesh(VertexHelper vh)
	{
		bool flag = base.sprite == null || base.sprite.texture == null || vh.currentVertCount == 0;
		if (!flag)
		{
			Rect rect = base.GetPixelAdjustedRect();
			float imgW = base.sprite.rect.width;
			float imgH = base.sprite.rect.height;
			float dispW = rect.width;
			float dispH = rect.height;
			bool flag2 = imgW <= 0f || imgH <= 0f || dispW <= 0f || dispH <= 0f;
			if (!flag2)
			{
				bool wStretch = dispW > imgW;
				bool hStretch = dispH > imgH;
				bool hCrop = dispH < imgH;
				bool wCrop = dispW < imgW;
				bool flag3 = !wStretch && !hStretch && !hCrop && !wCrop;
				if (!flag3)
				{
					int offX = Mathf.Clamp(this.pinPixelOffset.x, 0, (int)imgW - 1);
					int offY = Mathf.Clamp(this.pinPixelOffset.y, 0, (int)imgH - 1);
					UIVertex v0 = default(UIVertex);
					UIVertex v = default(UIVertex);
					UIVertex v2 = default(UIVertex);
					UIVertex v3 = default(UIVertex);
					vh.PopulateUIVertex(ref v0, 0);
					vh.PopulateUIVertex(ref v, 1);
					vh.PopulateUIVertex(ref v2, 2);
					vh.PopulateUIVertex(ref v3, 3);
					Vector2 uvBL = v0.uv0;
					Vector2 uvTL = v.uv0;
					Vector2 uvTR = v2.uv0;
					Vector2 uvBR = v3.uv0;
					float du = (uvTR.x - uvTL.x) / imgW;
					float dv = (uvTL.y - uvBL.y) / imgH;
					bool flag4 = wStretch;
					CImagePinned.<>c__DisplayClass5_0 CS$<>8__locals1;
					if (flag4)
					{
						float extra = dispW - imgW;
						CS$<>8__locals1.h_leftL = rect.xMin;
						CS$<>8__locals1.h_leftR = CS$<>8__locals1.h_leftL + (float)offX;
						CS$<>8__locals1.h_midL = CS$<>8__locals1.h_leftR;
						CS$<>8__locals1.h_midR = CS$<>8__locals1.h_leftR + extra;
						CS$<>8__locals1.h_rightL = CS$<>8__locals1.h_midR;
						CS$<>8__locals1.h_rightR = rect.xMax;
						CS$<>8__locals1.h_uvLeftL = uvBL.x;
						CS$<>8__locals1.h_uvLeftR = uvBL.x + du * (float)offX;
						CS$<>8__locals1.h_uvMidL = CS$<>8__locals1.h_uvLeftR;
						CS$<>8__locals1.h_uvMidR = CS$<>8__locals1.h_uvLeftR + du;
						CS$<>8__locals1.h_uvRightL = CS$<>8__locals1.h_uvLeftR;
						CS$<>8__locals1.h_uvRightR = uvBR.x;
					}
					else
					{
						bool flag5 = wCrop;
						if (flag5)
						{
							float cropU = uvBR.x - du * dispW;
							CS$<>8__locals1.h_leftL = (CS$<>8__locals1.h_midL = 0f);
							CS$<>8__locals1.h_leftR = (CS$<>8__locals1.h_midR = 0f);
							CS$<>8__locals1.h_rightL = rect.xMin;
							CS$<>8__locals1.h_rightR = rect.xMax;
							CS$<>8__locals1.h_uvLeftL = (CS$<>8__locals1.h_uvLeftR = 0f);
							CS$<>8__locals1.h_uvMidL = (CS$<>8__locals1.h_uvMidR = 0f);
							CS$<>8__locals1.h_uvRightL = cropU;
							CS$<>8__locals1.h_uvRightR = uvBR.x;
						}
						else
						{
							CS$<>8__locals1.h_leftL = (CS$<>8__locals1.h_midL = 0f);
							CS$<>8__locals1.h_leftR = (CS$<>8__locals1.h_midR = 0f);
							CS$<>8__locals1.h_rightL = rect.xMin;
							CS$<>8__locals1.h_rightR = rect.xMax;
							CS$<>8__locals1.h_uvLeftL = (CS$<>8__locals1.h_uvLeftR = 0f);
							CS$<>8__locals1.h_uvMidL = (CS$<>8__locals1.h_uvMidR = 0f);
							CS$<>8__locals1.h_uvRightL = uvBL.x;
							CS$<>8__locals1.h_uvRightR = uvBR.x;
						}
					}
					bool flag6 = hStretch;
					if (flag6)
					{
						float extra2 = dispH - imgH;
						CS$<>8__locals1.v_botB = rect.yMin;
						CS$<>8__locals1.v_botT = CS$<>8__locals1.v_botB + (float)offY;
						CS$<>8__locals1.v_midB = CS$<>8__locals1.v_botT;
						CS$<>8__locals1.v_midT = CS$<>8__locals1.v_botT + extra2;
						CS$<>8__locals1.v_topB = CS$<>8__locals1.v_midT;
						CS$<>8__locals1.v_topT = rect.yMax;
						CS$<>8__locals1.h_uvBotB = uvBL.y;
						CS$<>8__locals1.h_uvBotT = uvBL.y + dv * (float)offY;
						CS$<>8__locals1.h_uvMidB = CS$<>8__locals1.h_uvBotT;
						CS$<>8__locals1.h_uvMidT = CS$<>8__locals1.h_uvBotT + dv;
						CS$<>8__locals1.h_uvTopB = CS$<>8__locals1.h_uvBotT;
						CS$<>8__locals1.h_uvTopT = uvTL.y;
					}
					else
					{
						bool flag7 = hCrop;
						if (flag7)
						{
							float ratio = dispH / imgH;
							float cropV = Mathf.Lerp(uvTL.y, uvBL.y, ratio);
							CS$<>8__locals1.v_botB = (CS$<>8__locals1.v_midB = 0f);
							CS$<>8__locals1.v_botT = (CS$<>8__locals1.v_midT = 0f);
							CS$<>8__locals1.v_topB = rect.yMin;
							CS$<>8__locals1.v_topT = rect.yMax;
							CS$<>8__locals1.h_uvBotB = (CS$<>8__locals1.h_uvBotT = 0f);
							CS$<>8__locals1.h_uvMidB = (CS$<>8__locals1.h_uvMidT = 0f);
							CS$<>8__locals1.h_uvTopB = cropV;
							CS$<>8__locals1.h_uvTopT = uvTL.y;
						}
						else
						{
							CS$<>8__locals1.v_botB = (CS$<>8__locals1.v_midB = 0f);
							CS$<>8__locals1.v_botT = (CS$<>8__locals1.v_midT = 0f);
							CS$<>8__locals1.v_topB = rect.yMin;
							CS$<>8__locals1.v_topT = rect.yMax;
							CS$<>8__locals1.h_uvBotB = (CS$<>8__locals1.h_uvBotT = 0f);
							CS$<>8__locals1.h_uvMidB = (CS$<>8__locals1.h_uvMidT = 0f);
							CS$<>8__locals1.h_uvTopB = uvBL.y;
							CS$<>8__locals1.h_uvTopT = uvTL.y;
						}
					}
					Color32 c32 = this.color;
					CS$<>8__locals1.quads = new List<ValueTuple<float, float, float, float, float, float, float, ValueTuple<float>>>();
					bool flag8 = wStretch && hStretch;
					if (flag8)
					{
						CImagePinned.<ModifyMesh>g__AddQ|5_0(0, 0, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(1, 0, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 0, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(0, 1, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(1, 1, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 1, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(0, 2, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(1, 2, ref CS$<>8__locals1);
						CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 2, ref CS$<>8__locals1);
					}
					else
					{
						bool flag9 = wStretch;
						if (flag9)
						{
							CImagePinned.<ModifyMesh>g__AddQ|5_0(0, 2, ref CS$<>8__locals1);
							CImagePinned.<ModifyMesh>g__AddQ|5_0(1, 2, ref CS$<>8__locals1);
							CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 2, ref CS$<>8__locals1);
						}
						else
						{
							bool flag10 = hStretch;
							if (flag10)
							{
								CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 0, ref CS$<>8__locals1);
								CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 1, ref CS$<>8__locals1);
								CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 2, ref CS$<>8__locals1);
							}
							else
							{
								CImagePinned.<ModifyMesh>g__AddQ|5_0(2, 2, ref CS$<>8__locals1);
							}
						}
					}
					vh.Clear();
					for (int q = 0; q < CS$<>8__locals1.quads.Count; q++)
					{
						ValueTuple<float, float, float, float, float, float, float, ValueTuple<float>> qd = CS$<>8__locals1.quads[q];
						int idx = vh.currentVertCount;
						vh.AddVert(new Vector3(qd.Item1, qd.Item2), c32, new Vector2(qd.Item5, qd.Item6));
						vh.AddVert(new Vector3(qd.Item1, qd.Item4), c32, new Vector2(qd.Item5, qd.Rest.Item1));
						vh.AddVert(new Vector3(qd.Item3, qd.Item4), c32, new Vector2(qd.Item7, qd.Rest.Item1));
						vh.AddVert(new Vector3(qd.Item3, qd.Item2), c32, new Vector2(qd.Item7, qd.Item6));
						vh.AddTriangle(idx, idx + 1, idx + 2);
						vh.AddTriangle(idx, idx + 2, idx + 3);
					}
				}
			}
		}
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00031558 File Offset: 0x0002F758
	[CompilerGenerated]
	internal static void <ModifyMesh>g__AddQ|5_0(int col, int row, ref CImagePinned.<>c__DisplayClass5_0 A_2)
	{
		bool flag = col == 0;
		float px0;
		float px;
		float pu0;
		float pu;
		if (flag)
		{
			px0 = A_2.h_leftL;
			px = A_2.h_leftR;
			pu0 = A_2.h_uvLeftL;
			pu = A_2.h_uvLeftR;
		}
		else
		{
			bool flag2 = col == 1;
			if (flag2)
			{
				px0 = A_2.h_midL;
				px = A_2.h_midR;
				pu0 = A_2.h_uvMidL;
				pu = A_2.h_uvMidR;
			}
			else
			{
				px0 = A_2.h_rightL;
				px = A_2.h_rightR;
				pu0 = A_2.h_uvRightL;
				pu = A_2.h_uvRightR;
			}
		}
		bool flag3 = row == 0;
		float py0;
		float py;
		float pv0;
		float pv;
		if (flag3)
		{
			py0 = A_2.v_botB;
			py = A_2.v_botT;
			pv0 = A_2.h_uvBotB;
			pv = A_2.h_uvBotT;
		}
		else
		{
			bool flag4 = row == 1;
			if (flag4)
			{
				py0 = A_2.v_midB;
				py = A_2.v_midT;
				pv0 = A_2.h_uvMidB;
				pv = A_2.h_uvMidT;
			}
			else
			{
				py0 = A_2.v_topB;
				py = A_2.v_topT;
				pv0 = A_2.h_uvTopB;
				pv = A_2.h_uvTopT;
			}
		}
		bool flag5 = px - px0 < 0.0001f || py - py0 < 0.0001f;
		if (!flag5)
		{
			A_2.quads.Add(new ValueTuple<float, float, float, float, float, float, float, ValueTuple<float>>(px0, py0, px, py, pu0, pv0, pu, new ValueTuple<float>(pv)));
		}
	}

	// Token: 0x04000777 RID: 1911
	[SerializeField]
	[Tooltip("边缘固定区域（像素）。X=左边固定宽度，Y=底部固定高度。扩展时这些区域保持原图，中间拉伸")]
	private Vector2Int pinPixelOffset;
}
