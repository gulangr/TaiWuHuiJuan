using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000082 RID: 130
public class MultiTextLanguage : MonoBehaviour
{
	// Token: 0x060004D9 RID: 1241 RVA: 0x00021DDF File Offset: 0x0001FFDF
	private void Awake()
	{
		this._textComponent = base.GetComponent<TextMeshProUGUI>();
		this._uguiText = base.GetComponent<Text>();
		this.SetLanguage();
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00021E04 File Offset: 0x00020004
	public void SetLanguage()
	{
		this.LanguageIds.Clear();
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		sb.Clear();
		foreach (string key in this.Keys)
		{
			LanguageKey languageId;
			this.LanguageIds.Add(Enum.TryParse<LanguageKey>(key, out languageId) ? languageId : LanguageKey.Invalid);
			string strText = LocalStringManager.Get(languageId).ColorReplace();
			sb.Append(strText);
		}
		bool flag = this._textComponent != null;
		if (flag)
		{
			this._textComponent.text = sb.ToString();
		}
		else
		{
			bool flag2 = this._uguiText != null;
			if (flag2)
			{
				this._uguiText.text = sb.ToString();
			}
		}
	}

	// Token: 0x040003E0 RID: 992
	public List<string> Keys = new List<string>();

	// Token: 0x040003E1 RID: 993
	[HideInInspector]
	public List<LanguageKey> LanguageIds = new List<LanguageKey>();

	// Token: 0x040003E2 RID: 994
	private TextMeshProUGUI _textComponent;

	// Token: 0x040003E3 RID: 995
	private Text _uguiText;
}
