using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200009B RID: 155
[ExecuteAlways]
public class TMPClipDisabler : MonoBehaviour
{
	// Token: 0x0600055D RID: 1373 RVA: 0x0002448C File Offset: 0x0002268C
	public void OnEnable()
	{
		TextMeshProUGUI tmp = base.GetComponent<TextMeshProUGUI>();
		bool flag = tmp == null || tmp.fontSharedMaterial == null || tmp.fontMaterial == null;
		if (!flag)
		{
			Material material;
			bool flag2 = !this._Cache.TryGetValue(tmp.fontMaterial, out material);
			if (flag2)
			{
				material = Object.Instantiate<Material>(tmp.fontMaterial);
				material.SetFloat(TMPClipDisabler.ClipRangeLeft, 0f);
				material.SetFloat(TMPClipDisabler.ClipRangeRight, 0f);
				material.SetFloat(TMPClipDisabler.ClipRangeTop, 0f);
				material.SetFloat(TMPClipDisabler.ClipRangeBottom, 0f);
				this._Cache.Add(tmp.fontMaterial, material);
			}
			tmp.fontMaterial = material;
		}
	}

	// Token: 0x04000462 RID: 1122
	private Dictionary<Material, Material> _Cache = new Dictionary<Material, Material>();

	// Token: 0x04000463 RID: 1123
	private static readonly int ClipRangeLeft = Shader.PropertyToID("_ClipRangeLeft");

	// Token: 0x04000464 RID: 1124
	private static readonly int ClipRangeRight = Shader.PropertyToID("_ClipRangeRight");

	// Token: 0x04000465 RID: 1125
	private static readonly int ClipRangeTop = Shader.PropertyToID("_ClipRangeTop");

	// Token: 0x04000466 RID: 1126
	private static readonly int ClipRangeBottom = Shader.PropertyToID("_ClipRangeBottom");
}
