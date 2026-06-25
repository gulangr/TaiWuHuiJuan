using System;
using TMPro;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x020005FD RID: 1533
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class GlowCopyTmp : MonoBehaviour, IGlowCopy
	{
		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06004853 RID: 18515 RVA: 0x0021E5AD File Offset: 0x0021C7AD
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06004854 RID: 18516 RVA: 0x0021E5B5 File Offset: 0x0021C7B5
		public GameObject GameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06004855 RID: 18517 RVA: 0x0021E5BD File Offset: 0x0021C7BD
		// (set) Token: 0x06004856 RID: 18518 RVA: 0x0021E5C5 File Offset: 0x0021C7C5
		public Color GlowColor { get; private set; }

		// Token: 0x06004857 RID: 18519 RVA: 0x0021E5D0 File Offset: 0x0021C7D0
		public void SetAsGlowColor()
		{
			TextMeshProUGUI textComponent = base.GetComponent<TextMeshProUGUI>();
			textComponent.color = this.GlowColor;
		}

		// Token: 0x06004858 RID: 18520 RVA: 0x0021E5F4 File Offset: 0x0021C7F4
		public void SetAsOriginalColor()
		{
			TextMeshProUGUI textComponent = base.GetComponent<TextMeshProUGUI>();
			textComponent.color = this._originalColor;
		}

		// Token: 0x06004859 RID: 18521 RVA: 0x0021E618 File Offset: 0x0021C818
		public void Sync(IGlowSource source)
		{
			GlowSourceTmp sourceTmp = source as GlowSourceTmp;
			bool flag = sourceTmp == null;
			if (flag)
			{
				Debug.LogError("GlowCopyTmp can only sync with GlowSourceTmp.");
			}
			else
			{
				TextMeshProUGUI textComponent = base.GetComponent<TextMeshProUGUI>();
				TextMeshProUGUI sourceTextComponent = sourceTmp.TextComponent;
				textComponent.text = sourceTextComponent.text;
				textComponent.font = sourceTextComponent.font;
				textComponent.fontSize = sourceTextComponent.fontSize;
				textComponent.fontStyle = sourceTextComponent.fontStyle;
				textComponent.alignment = sourceTextComponent.alignment;
				textComponent.color = sourceTextComponent.color;
				this._originalColor = sourceTextComponent.color;
				this.GlowColor = sourceTmp.GlowColor;
			}
		}

		// Token: 0x040031F3 RID: 12787
		private Color _originalColor;
	}
}
