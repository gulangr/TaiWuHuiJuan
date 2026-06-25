using System;
using TMPro;
using UnityEngine;

// Token: 0x02000328 RID: 808
public class CommonConfigureBase : MonoBehaviour
{
	// Token: 0x17000527 RID: 1319
	// (get) Token: 0x06002F07 RID: 12039 RVA: 0x00171DD5 File Offset: 0x0016FFD5
	// (set) Token: 0x06002F08 RID: 12040 RVA: 0x00171DE0 File Offset: 0x0016FFE0
	public bool ShowIcon
	{
		get
		{
			return this.showIcon;
		}
		set
		{
			GameObject gameObject = this.icon.gameObject;
			this.showIcon = value;
			gameObject.SetActive(value);
			this.text.rectTransform.anchoredPosition = new Vector2(this.showIcon ? this.textLeftPaddingWithIcon : this.textLeftPaddingWithoutIcon, 0f);
		}
	}

	// Token: 0x17000528 RID: 1320
	// (set) Token: 0x06002F09 RID: 12041 RVA: 0x00171E3A File Offset: 0x0017003A
	public string Text
	{
		set
		{
			this.text.text = value;
		}
	}

	// Token: 0x17000529 RID: 1321
	// (set) Token: 0x06002F0A RID: 12042 RVA: 0x00171E49 File Offset: 0x00170049
	public LanguageKey TextKey
	{
		set
		{
			this.text.text = value.Tr();
		}
	}

	// Token: 0x04002225 RID: 8741
	[SerializeField]
	private bool showIcon;

	// Token: 0x04002226 RID: 8742
	[SerializeField]
	private CImage icon;

	// Token: 0x04002227 RID: 8743
	[SerializeField]
	private float textLeftPaddingWithIcon;

	// Token: 0x04002228 RID: 8744
	[SerializeField]
	private float textLeftPaddingWithoutIcon;

	// Token: 0x04002229 RID: 8745
	[SerializeField]
	private TMP_Text text;
}
