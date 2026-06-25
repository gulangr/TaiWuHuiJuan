using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PostProcessGlow
{
	// Token: 0x020005FB RID: 1531
	public class AutoGlow : MonoBehaviour
	{
		// Token: 0x06004849 RID: 18505 RVA: 0x0021E3B0 File Offset: 0x0021C5B0
		private void Awake()
		{
			TextMeshProUGUI text = base.GetComponent<TextMeshProUGUI>();
			bool flag = text != null;
			if (flag)
			{
				GlowImageGenerator.Instance.GenerateTextGlow(text, this.GlowColor, this.Parameters, null, null, null);
			}
			else
			{
				Image image = base.GetComponent<Image>();
				bool flag2 = image != null;
				if (flag2)
				{
					GlowImageGenerator.Instance.GenerateImageGlow(image, this.GlowColor, this.Parameters, null, null);
				}
			}
		}

		// Token: 0x040031EE RID: 12782
		public Color GlowColor = Color.yellow;

		// Token: 0x040031EF RID: 12783
		[Header("Glow Parameters")]
		public GlowImageGenerator.GlowParameters Parameters = new GlowImageGenerator.GlowParameters();
	}
}
