using System;
using TMPro;
using UnityEngine;

// Token: 0x02000099 RID: 153
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextStyleResetter : MonoBehaviour
{
	// Token: 0x06000555 RID: 1365 RVA: 0x00024294 File Offset: 0x00022494
	public void ResetToInitialStyle()
	{
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			bool flag2 = this._textComponent == null;
			if (flag2)
			{
				this._textComponent = base.GetComponent<TextMeshProUGUI>();
			}
			this.initialStyle.ApplyTo(this._textComponent);
			TextStyle textStyle = base.GetComponent<TextStyle>();
			bool flag3 = textStyle != null;
			if (flag3)
			{
				textStyle.OnLanguageChange(LocalStringManager.CurLanguageType);
			}
		}
	}

	// Token: 0x0400045C RID: 1116
	[SerializeField]
	private TextStyleData initialStyle = new TextStyleData();

	// Token: 0x0400045D RID: 1117
	private TextMeshProUGUI _textComponent;
}
