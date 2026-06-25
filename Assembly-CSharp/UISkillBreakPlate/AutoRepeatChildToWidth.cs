using System;
using UnityEngine;
using UnityEngine.UI;

namespace UISkillBreakPlate
{
	// Token: 0x02000419 RID: 1049
	public class AutoRepeatChildToWidth : MonoBehaviour
	{
		// Token: 0x06003E6C RID: 15980 RVA: 0x001F522C File Offset: 0x001F342C
		private void Update()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				float spacing = this.layout.spacing;
				float templateWidth = this.template.rect.width;
				int count = Mathf.CeilToInt((this.layout.GetComponent<RectTransform>().rect.width + spacing) / (templateWidth + spacing));
				CommonUtils.PrepareEnoughChildren(this.layout.transform, this.template.gameObject, count, null);
			}
		}

		// Token: 0x04002CFF RID: 11519
		[SerializeField]
		private HorizontalLayoutGroup layout;

		// Token: 0x04002D00 RID: 11520
		[SerializeField]
		private RectTransform template;
	}
}
