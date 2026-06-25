using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class MouseTipFeatureMedalLegacy : MouseTipBase
{
	// Token: 0x06002A12 RID: 10770 RVA: 0x00140999 File Offset: 0x0013EB99
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitRefers();
		this.Refresh(argsBox);
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x001409AC File Offset: 0x0013EBAC
	public override void Refresh(ArgumentBox argBox)
	{
		string medalTypeStr;
		argBox.Get("arg0", out medalTypeStr);
		sbyte medalType = Convert.ToSByte(medalTypeStr);
		bool flag = medalType < 0 || medalType > 2;
		if (!flag)
		{
			string medalName = CommonUtils.GetFeatureMedalTypeText((int)medalType);
			this._desc.text = LocalStringManager.Get(string.Format("LK_MouseTip_FeatureMedal_Desc_{0}", medalType)).ColorReplace();
			this._titleLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_Title, medalName);
			this._descIcon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)medalType, 0), false, null);
			this._commandTitleLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_SubTitle, medalName);
			List<TeammateCommandItem> commands = this.GetCommands(medalType);
			this.PreserveCommandItems(commands.Count);
			for (int i = 0; i < commands.Count; i++)
			{
				Refers commandRefers = this._commandArea.GetChild(i + 1).GetComponent<Refers>();
				TextMeshProUGUI nameLabel = commandRefers.CGet<TextMeshProUGUI>("NameLabel");
				CImage icon = commandRefers.CGet<CImage>("Icon");
				TextMeshProUGUI medalCountLabel = commandRefers.CGet<TextMeshProUGUI>("MedalCountLabel");
				nameLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_Command, commands[i].Name);
				icon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)medalType, 0), false, null);
				medalCountLabel.text = string.Format("x{0}", commands[i].MedalCount);
			}
			if (!true)
			{
			}
			int num;
			switch (medalType)
			{
			case 0:
				num = 7;
				break;
			case 1:
				num = 6;
				break;
			case 2:
				num = 8;
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(medalType);
				break;
			}
			if (!true)
			{
			}
			int iconIndex = num;
			this._extraSubTitle.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_ExtraTitle, iconIndex, medalName).ColorReplace();
			this._extraSubTitle.GetComponent<TMPTextSpriteHelper>().Parse();
			this._effectTextLabelList[0].text = LocalStringManager.GetFormat("LK_MouseTip_FeatureMedal_ExtraContent_0", iconIndex, medalName).ColorReplace();
			this._effectTextLabelList[0].GetComponent<TMPTextSpriteHelper>().Parse();
			for (int j = 1; j < this._effectTextLabelList.Count; j++)
			{
				this._effectTextLabelList[j].transform.parent.gameObject.SetActive(medalType == 2);
				bool flag2 = medalType != 2;
				if (!flag2)
				{
					this._effectTextLabelList[j].text = LocalStringManager.Get(string.Format("LK_MouseTip_FeatureMedal_ExtraContent_{0}", j)).ColorReplace();
					this._effectTextLabelList[j].GetComponent<TMPTextSpriteHelper>().Parse();
				}
			}
		}
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x00140C78 File Offset: 0x0013EE78
	private List<TeammateCommandItem> GetCommands(sbyte medalType)
	{
		this._cachedCommands.Clear();
		foreach (TeammateCommandItem configItem in ((IEnumerable<TeammateCommandItem>)TeammateCommand.Instance))
		{
			bool flag = configItem.MedalType == medalType && configItem.Type == ETeammateCommandType.Normal;
			if (flag)
			{
				this._cachedCommands.Add(configItem);
			}
		}
		return this._cachedCommands;
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x00140CFC File Offset: 0x0013EEFC
	private void PreserveCommandItems(int count)
	{
		Transform parent = this._commandTemplate.transform.parent;
		for (int i = 0; i < count; i++)
		{
			bool flag = i >= parent.childCount - 1;
			if (flag)
			{
				Object.Instantiate<Refers>(this._commandTemplate, parent);
			}
			parent.GetChild(i + 1).gameObject.SetActive(true);
		}
		for (int j = count; j < parent.childCount - 1; j++)
		{
			parent.GetChild(j + 1).gameObject.SetActive(false);
		}
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x00140D98 File Offset: 0x0013EF98
	private void InitRefers()
	{
		this._effectTextLabelList = base.CGetList<TextMeshProUGUI>("EffectTextLabel_");
		this._titleLabel = base.CGet<TextMeshProUGUI>("TitleLabel");
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._commandArea = base.CGet<RectTransform>("CommandArea");
		this._commandTemplate = base.CGet<Refers>("CommandTemplate");
		this._extraSubTitle = base.CGet<TextMeshProUGUI>("ExtraSubTitle");
		this._descIcon = base.CGet<CImage>("DescIcon");
		this._commandTitleLabel = base.CGet<TextMeshProUGUI>("CommandTitleLabel");
		this._extraArea = base.CGet<GameObject>("ExtraArea");
	}

	// Token: 0x04001E8D RID: 7821
	private const int OtherChildCount = 1;

	// Token: 0x04001E8E RID: 7822
	private List<TeammateCommandItem> _cachedCommands = new List<TeammateCommandItem>();

	// Token: 0x04001E8F RID: 7823
	private List<TextMeshProUGUI> _effectTextLabelList;

	// Token: 0x04001E90 RID: 7824
	private TextMeshProUGUI _titleLabel;

	// Token: 0x04001E91 RID: 7825
	private TextMeshProUGUI _desc;

	// Token: 0x04001E92 RID: 7826
	private RectTransform _commandArea;

	// Token: 0x04001E93 RID: 7827
	private Refers _commandTemplate;

	// Token: 0x04001E94 RID: 7828
	private TextMeshProUGUI _extraSubTitle;

	// Token: 0x04001E95 RID: 7829
	private CImage _descIcon;

	// Token: 0x04001E96 RID: 7830
	private TextMeshProUGUI _commandTitleLabel;

	// Token: 0x04001E97 RID: 7831
	private GameObject _extraArea;
}
