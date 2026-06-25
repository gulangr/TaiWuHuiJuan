using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000322 RID: 802
[ExecuteAlways]
public class CommonCharacterNameFrame : UIBehaviour, ILanguage
{
	// Token: 0x17000523 RID: 1315
	// (get) Token: 0x06002EE2 RID: 12002 RVA: 0x00171496 File Offset: 0x0016F696
	public TextMeshProUGUI NameLabel
	{
		get
		{
			return this.nameLabel;
		}
	}

	// Token: 0x17000524 RID: 1316
	// (get) Token: 0x06002EE4 RID: 12004 RVA: 0x001714EA File Offset: 0x0016F6EA
	// (set) Token: 0x06002EE3 RID: 12003 RVA: 0x001714A0 File Offset: 0x0016F6A0
	public CommonCharacterNameFrame.HeightMode Height
	{
		get
		{
			return this._heightMode;
		}
		set
		{
			this._heightMode = value;
			RectTransform rect = base.GetComponent<RectTransform>();
			rect.sizeDelta = rect.sizeDelta.SetY(this.heightSettings.First((CommonCharacterNameFrame.HeightSetting s) => s.Mode == this._heightMode).Height);
		}
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x001714F2 File Offset: 0x0016F6F2
	public void SetName(string text)
	{
		this.nameLabel.text = text;
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x00171501 File Offset: 0x0016F701
	protected override void Awake()
	{
		GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x00171520 File Offset: 0x0016F720
	protected override void OnDestroy()
	{
		GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x0017153F File Offset: 0x0016F73F
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		this.Height = ((languageType != LocalStringManager.LanguageType.CN) ? CommonCharacterNameFrame.HeightMode.Latin : CommonCharacterNameFrame.HeightMode.Normal);
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x00171550 File Offset: 0x0016F750
	private void OnLanguageChange(ArgumentBox argumentBox)
	{
		this.OnLanguageChange(LocalStringManager.CurLanguageType);
	}

	// Token: 0x04002208 RID: 8712
	[SerializeField]
	private CImage backGround;

	// Token: 0x04002209 RID: 8713
	[SerializeField]
	private TextMeshProUGUI nameLabel;

	// Token: 0x0400220A RID: 8714
	[SerializeField]
	private List<CommonCharacterNameFrame.HeightSetting> heightSettings;

	// Token: 0x0400220B RID: 8715
	private CommonCharacterNameFrame.HeightMode _heightMode;

	// Token: 0x020016A6 RID: 5798
	[Serializable]
	public struct HeightSetting
	{
		// Token: 0x0400A897 RID: 43159
		public CommonCharacterNameFrame.HeightMode Mode;

		// Token: 0x0400A898 RID: 43160
		public float Height;
	}

	// Token: 0x020016A7 RID: 5799
	public enum HeightMode
	{
		// Token: 0x0400A89A RID: 43162
		Normal,
		// Token: 0x0400A89B RID: 43163
		Latin
	}
}
