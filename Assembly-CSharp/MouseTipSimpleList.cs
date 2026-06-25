using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class MouseTipSimpleList : MouseTipBase
{
	// Token: 0x06002B39 RID: 11065 RVA: 0x0015193C File Offset: 0x0014FB3C
	protected override void Init(ArgumentBox argumentBox)
	{
		this.Refresh(argumentBox);
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x00151947 File Offset: 0x0014FB47
	public override void Refresh(ArgumentBox argumentBox)
	{
		this.ReadArgs(argumentBox);
		this.RefreshAll();
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x0015195C File Offset: 0x0014FB5C
	private void RefreshAll()
	{
		this.titleLabel.text = this._config.TitleKey.GetString();
		CommonUtils.PrepareEnoughChildren(this.lineRoot, this.lineTemplate.gameObject, this._config.Lines.Count, null);
		for (int i = 0; i < this._config.Lines.Count; i++)
		{
			this.RefreshLine(i);
		}
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x001519E0 File Offset: 0x0014FBE0
	private void RefreshLine(int i)
	{
		Transform line = this.lineRoot.GetChild(i);
		MouseTipSimpleList.LineConfig lineConfig = this._config.Lines[i];
		Refers refers = line.GetComponent<Refers>();
		CImage icon = refers.CGet<CImage>("Icon");
		TextMeshProUGUI text = refers.CGet<TextMeshProUGUI>("Text");
		icon.SetSprite(lineConfig.Icon, false, null);
		text.text = lineConfig.Key.GetString();
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x00151A50 File Offset: 0x0014FC50
	private void ReadArgs(ArgumentBox argumentBox)
	{
		argumentBox.Get<MouseTipSimpleList.Config>("Config", out this._config);
	}

	// Token: 0x04001F62 RID: 8034
	private MouseTipSimpleList.Config _config;

	// Token: 0x04001F63 RID: 8035
	[SerializeField]
	private TextMeshProUGUI titleLabel;

	// Token: 0x04001F64 RID: 8036
	[SerializeField]
	private RectTransform lineRoot;

	// Token: 0x04001F65 RID: 8037
	[SerializeField]
	private Refers lineTemplate;

	// Token: 0x02001628 RID: 5672
	public struct LineConfig
	{
		// Token: 0x0600D109 RID: 53513 RVA: 0x005A9E8D File Offset: 0x005A808D
		public LineConfig(string icon, StringKey key)
		{
			this.Icon = icon;
			this.Key = key;
		}

		// Token: 0x0400A71E RID: 42782
		public string Icon;

		// Token: 0x0400A71F RID: 42783
		public StringKey Key;
	}

	// Token: 0x02001629 RID: 5673
	public struct Config
	{
		// Token: 0x0400A720 RID: 42784
		public StringKey TitleKey;

		// Token: 0x0400A721 RID: 42785
		public List<MouseTipSimpleList.LineConfig> Lines;
	}
}
