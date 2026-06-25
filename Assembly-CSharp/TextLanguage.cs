using System;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000095 RID: 149
public class TextLanguage : MonoBehaviour
{
	// Token: 0x06000542 RID: 1346 RVA: 0x00023D30 File Offset: 0x00021F30
	private void Awake()
	{
		this._textComponent = base.GetComponent<TextMeshProUGUI>();
		this._uguiText = base.GetComponent<Text>();
		this.SetLanguage();
		GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.SetLanguage));
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x00023D6E File Offset: 0x00021F6E
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.SetLanguage));
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00023D8D File Offset: 0x00021F8D
	private void SetLanguage(ArgumentBox _)
	{
		this.SetLanguage();
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x00023D98 File Offset: 0x00021F98
	public void SetLanguage()
	{
		Enum.TryParse<LanguageKey>(this.Key, out this.LanguageId);
		string strText = LocalStringManager.Get(this.LanguageId).ColorReplace();
		bool flag = null != this._textComponent;
		if (flag)
		{
			this._textComponent.text = strText;
		}
		else
		{
			bool flag2 = null != this._uguiText;
			if (flag2)
			{
				this._uguiText.text = strText;
			}
		}
		TMPTextSpriteHelper spriteHelper;
		bool flag3 = this.autoParseSprite && base.TryGetComponent<TMPTextSpriteHelper>(out spriteHelper);
		if (flag3)
		{
			spriteHelper.Parse();
		}
	}

	// Token: 0x04000443 RID: 1091
	public string Key;

	// Token: 0x04000444 RID: 1092
	[NonSerialized]
	public LanguageKey LanguageId;

	// Token: 0x04000445 RID: 1093
	private TextMeshProUGUI _textComponent;

	// Token: 0x04000446 RID: 1094
	private Text _uguiText;

	// Token: 0x04000447 RID: 1095
	public bool autoParseSprite;
}
