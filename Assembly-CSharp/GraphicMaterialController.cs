using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000064 RID: 100
public class GraphicMaterialController : MonoBehaviour
{
	// Token: 0x06000349 RID: 841 RVA: 0x00013F44 File Offset: 0x00012144
	public void SetMaterial(bool apply)
	{
		bool flag = this.graphics == null;
		if (!flag)
		{
			Material materialToSet = apply ? this.targetMaterial : null;
			foreach (Graphic graphic in this.graphics)
			{
				bool flag2 = graphic != null;
				if (flag2)
				{
					graphic.material = materialToSet;
				}
			}
		}
	}

	// Token: 0x040001F6 RID: 502
	[SerializeField]
	private Material targetMaterial;

	// Token: 0x040001F7 RID: 503
	[SerializeField]
	private Graphic[] graphics;
}
