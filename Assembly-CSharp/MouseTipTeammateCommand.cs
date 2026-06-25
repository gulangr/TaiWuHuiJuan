using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class MouseTipTeammateCommand : MouseTipBase
{
	// Token: 0x06002B93 RID: 11155 RVA: 0x00153978 File Offset: 0x00151B78
	public static bool CanUse(sbyte commandId)
	{
		TeammateCommandItem teammateCommandItem = TeammateCommand.Instance[commandId];
		bool result;
		if (teammateCommandItem.MedalType == -1)
		{
			ETeammateCommandType type = teammateCommandItem.Type;
			result = (type == ETeammateCommandType.Animal || type == ETeammateCommandType.GearMate || type == ETeammateCommandType.VitalDemon);
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x001539BE File Offset: 0x00151BBE
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		this.InitRefers();
		this.Refresh(argsBox);
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x001539DC File Offset: 0x00151BDC
	public override void Refresh(ArgumentBox argsBox)
	{
		sbyte commandId;
		argsBox.Get("CommandId", out commandId);
		this._config = TeammateCommand.Instance[commandId];
		bool flag = !MouseTipTeammateCommand.CanUse(commandId);
		if (!flag)
		{
			this.PreserveEffectItems(this._config.EffectDisplayTextList.Length);
			this._titleLabel.text = this._config.Name;
			this._desc.text = this._config.Description.ColorReplace();
			this._effectArea.SetActive(this._config.EffectDisplayTextList.Length != 0);
			for (int i = 0; i < this._config.EffectDisplayTextList.Length; i++)
			{
				this.RefreshEffectItem(i);
			}
		}
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x00153AA4 File Offset: 0x00151CA4
	private void RefreshEffectItem(int index)
	{
		Transform effectItem = this._effectTemplate.transform.parent.GetChild(index + 1);
		Refers refers = effectItem.GetComponent<Refers>();
		CImage effectIcon = refers.CGet<CImage>("EffectIcon");
		TextMeshProUGUI effectTextLabel = refers.CGet<TextMeshProUGUI>("EffectTextLabel");
		TextMeshProUGUI effectValueLabel = refers.CGet<TextMeshProUGUI>("EffectValueLabel");
		bool hasIcon = UpgradeTeammateCommandEffectHelper.HasIcon(this._config, index);
		effectIcon.gameObject.SetActive(hasIcon);
		bool flag = hasIcon;
		if (flag)
		{
			effectIcon.SetSprite(UpgradeTeammateCommandEffectHelper.GetIcon(this._config, index), false, null);
		}
		effectTextLabel.text = this._config.EffectDisplayTextList[index].ColorReplace();
		effectValueLabel.text = this._config.EffectDisplayValueList[index].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor2(this._config, index));
	}

	// Token: 0x06002B97 RID: 11159 RVA: 0x00153B74 File Offset: 0x00151D74
	private void PreserveEffectItems(int count)
	{
		Transform parent = this._effectTemplate.transform.parent;
		for (int i = 0; i < count; i++)
		{
			bool flag = i >= parent.childCount - 1;
			if (flag)
			{
				Object.Instantiate<GameObject>(this._effectTemplate, parent);
			}
			parent.GetChild(i + 1).gameObject.SetActive(true);
		}
		for (int j = count; j < parent.childCount - 1; j++)
		{
			parent.GetChild(j + 1).gameObject.SetActive(false);
		}
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x00153C10 File Offset: 0x00151E10
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.SupportCommands);
		}
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x00153C44 File Offset: 0x00151E44
	private void InitRefers()
	{
		this._titleLabel = base.CGet<TextMeshProUGUI>("TitleLabel");
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._effectTemplate = base.CGet<GameObject>("EffectTemplate");
		this._effectArea = base.CGet<GameObject>("EffectArea");
	}

	// Token: 0x04001FC7 RID: 8135
	private TeammateCommandItem _config;

	// Token: 0x04001FC8 RID: 8136
	private const int OtherChildCount = 1;

	// Token: 0x04001FC9 RID: 8137
	private TextMeshProUGUI _titleLabel;

	// Token: 0x04001FCA RID: 8138
	private TextMeshProUGUI _desc;

	// Token: 0x04001FCB RID: 8139
	private GameObject _effectTemplate;

	// Token: 0x04001FCC RID: 8140
	private GameObject _effectArea;
}
