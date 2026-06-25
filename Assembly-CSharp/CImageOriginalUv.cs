using System;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CB RID: 203
public class CImageOriginalUv : CInputEventImage, IMeshModifier
{
	// Token: 0x06000704 RID: 1796 RVA: 0x00030B80 File Offset: 0x0002ED80
	public void ModifyMesh(Mesh mesh)
	{
		using (VertexHelper vh = new VertexHelper(mesh))
		{
			this.ModifyMesh(vh);
			vh.FillMesh(mesh);
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00030BC4 File Offset: 0x0002EDC4
	public void ModifyMesh(VertexHelper verts)
	{
		CImageOriginalUv.<>c__DisplayClass3_0 CS$<>8__locals1;
		CS$<>8__locals1.verts = verts;
		CS$<>8__locals1.<>4__this = this;
		this.<ModifyMesh>g__SetUv|3_0(new Vector2(0f, 0f), 0, ref CS$<>8__locals1);
		this.<ModifyMesh>g__SetUv|3_0(new Vector2(0f, 1f), 1, ref CS$<>8__locals1);
		this.<ModifyMesh>g__SetUv|3_0(new Vector2(1f, 1f), 2, ref CS$<>8__locals1);
		this.<ModifyMesh>g__SetUv|3_0(new Vector2(1f, 0f), 3, ref CS$<>8__locals1);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00030C68 File Offset: 0x0002EE68
	[CompilerGenerated]
	private void <ModifyMesh>g__SetUv|3_0(Vector2 uv, int i, ref CImageOriginalUv.<>c__DisplayClass3_0 A_3)
	{
		UIVertex vert = default(UIVertex);
		A_3.verts.PopulateUIVertex(ref vert, i);
		vert.uv1 = uv;
		vert.uv2 = this.externalUv2Data;
		vert.uv3 = this.externalUv3Data;
		A_3.verts.SetUIVertex(vert, i);
	}

	// Token: 0x04000775 RID: 1909
	[SerializeField]
	internal Vector4 externalUv2Data = Vector4.one;

	// Token: 0x04000776 RID: 1910
	[SerializeField]
	internal Vector4 externalUv3Data = Vector4.one;
}
