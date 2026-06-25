using System;
using TMPro;
using UnityEngine;

// Token: 0x02000096 RID: 150
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextStyle : MonoBehaviour, ILanguage
{
	// Token: 0x06000547 RID: 1351 RVA: 0x00023E30 File Offset: 0x00022030
	private void Awake()
	{
		if (this._textComponent == null)
		{
			this._textComponent = base.GetComponent<TextMeshProUGUI>();
		}
		bool flag = TextStyle.CustomTextStyleHandler != null;
		if (flag)
		{
			TextStyle.CustomTextStyleHandler(this._textComponent);
			this._textComponent.SetAllDirty();
		}
		bool isPlaying = Application.isPlaying;
		if (isPlaying)
		{
			bool flag2 = this._initialFontSize == 0f;
			if (flag2)
			{
				this._initialFontSize = this._textComponent.fontSize;
			}
			this.UpdateFontSizeByLanguage(LocalStringManager.CurLanguageType);
		}
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00023EB6 File Offset: 0x000220B6
	public void OnLanguageChange(LocalStringManager.LanguageType type)
	{
		this.UpdateFontSizeByLanguage(type);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00023EC4 File Offset: 0x000220C4
	private void UpdateFontSizeByLanguage(LocalStringManager.LanguageType type)
	{
		bool flag = type == LocalStringManager.LanguageType.EN;
		if (flag)
		{
			base.GetComponent<TextMeshProUGUI>().fontSize = this._initialFontSize - 2f;
		}
	}

	// Token: 0x04000448 RID: 1096
	private TextMeshProUGUI _textComponent;

	// Token: 0x04000449 RID: 1097
	[SerializeField]
	private float _initialFontSize;

	// Token: 0x0400044A RID: 1098
	public static Action<TextMeshProUGUI> CustomTextStyleHandler;
}
