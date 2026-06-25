using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class CharacterTeammateCommand : Refers
{
	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001C03 RID: 7171 RVA: 0x000C1BC4 File Offset: 0x000BFDC4
	// (set) Token: 0x06001C04 RID: 7172 RVA: 0x000C1BCC File Offset: 0x000BFDCC
	public int LastCmdType { get; private set; } = -1;

	// Token: 0x06001C05 RID: 7173 RVA: 0x000C1BD5 File Offset: 0x000BFDD5
	private void OnDisable()
	{
		this.ApplyToEnd();
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x000C1BE0 File Offset: 0x000BFDE0
	public void Set(int cmdType, bool isAlly = true)
	{
		this.LastCmdType = cmdType;
		bool invalid = cmdType < 0;
		CanvasGroup canvas = base.GetComponent<CanvasGroup>();
		bool flag = canvas != null;
		if (flag)
		{
			canvas.alpha = (float)(invalid ? 0 : 1);
		}
		else
		{
			base.gameObject.SetActive(!invalid);
		}
		bool flag2 = invalid;
		if (!flag2)
		{
			TeammateCommandItem config = TeammateCommand.Instance[cmdType];
			bool flag3 = config.Type == ETeammateCommandType.Negative;
			if (flag3)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(145);
			}
			ETeammateCommandType type = config.Type;
			if (!true)
			{
			}
			string text;
			if (type != ETeammateCommandType.Advance)
			{
				if (type != ETeammateCommandType.Negative)
				{
					text = "pinkyellow";
				}
				else
				{
					text = "negativecommand";
				}
			}
			else
			{
				text = "upgradeteammatecommand";
			}
			if (!true)
			{
			}
			string color = text;
			base.CGet<TextMeshProUGUI>("Name").SetText(config.Name.SetColor(Colors.Instance[color]), true);
			switch (this.SizeType)
			{
			case CharacterTeammateCommand.ESizeType.Small:
			{
				CImage component = base.GetComponent<CImage>();
				ETeammateCommandType type2 = config.Type;
				if (!true)
				{
				}
				if (type2 != ETeammateCommandType.Advance)
				{
					if (type2 != ETeammateCommandType.Negative)
					{
						text = "sp_instructions_2";
					}
					else
					{
						text = "sp_instructions_4";
					}
				}
				else
				{
					text = "sp_instructions_3";
				}
				if (!true)
				{
				}
				component.SetSprite(text, false, null);
				break;
			}
			case CharacterTeammateCommand.ESizeType.Large:
				base.GetComponent<CImage>().SetSprite((config.Type == ETeammateCommandType.Advance) ? "sp_instructions_1" : "sp_instructions_0", false, null);
				break;
			case CharacterTeammateCommand.ESizeType.CombatBegin:
				base.GetComponent<CImage>().SetSprite((config.Type == ETeammateCommandType.Advance) ? "combat_bottom_tongdaozhiling_5" : ((config.Type == ETeammateCommandType.Negative) ? "combat_bottom_tongdaozhiling_3" : (isAlly ? "combat_bottom_tongdaozhiling_0" : "combat_bottom_tongdaozhiling_4")), false, null);
				break;
			}
			TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
			bool flag4 = mouseTip == null;
			if (!flag4)
			{
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.enabled = MouseTipTeammateCommand.CanUse((sbyte)cmdType);
				mouseTip.RuntimeParam.Set("CommandId", (sbyte)cmdType);
			}
		}
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x000C1DFC File Offset: 0x000BFFFC
	public bool AnimationTo(int cmdType)
	{
		bool invalid = cmdType < 0;
		base.gameObject.SetActive(!invalid);
		bool flag = invalid;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.LastCmdType == cmdType;
			if (flag2)
			{
				result = false;
			}
			else
			{
				this.ApplyToEnd();
				ParticleSystem particle = base.CGet<ParticleSystem>("ToBad");
				GameObject particleGo = particle.gameObject;
				particleGo.SetActive(true);
				this._particles.Add(particleGo);
				this._settingCmdType = cmdType;
				base.DelayCall(new Action(this.ApplyCmdType), 0.56f);
				base.DelayCall(new Action(this.ApplyToEnd), particle.main.duration);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001C08 RID: 7176 RVA: 0x000C1EB8 File Offset: 0x000C00B8
	public void SetDisable(bool disabled)
	{
		DisableStyleRoot disableStyleRoot;
		bool flag = base.TryGetComponent<DisableStyleRoot>(out disableStyleRoot);
		if (flag)
		{
			disableStyleRoot.EffectTextColor = Colors.Instance["lightgrey"];
			disableStyleRoot.SetStyleEffect(disabled, false);
		}
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x000C1EF4 File Offset: 0x000C00F4
	private void ApplyCmdType()
	{
		bool flag = this._settingCmdType >= 0;
		if (flag)
		{
			this.Set(this._settingCmdType, true);
		}
		this._settingCmdType = -1;
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x000C1F28 File Offset: 0x000C0128
	private void ApplyToEnd()
	{
		this.ApplyCmdType();
		foreach (GameObject particle in this._particles)
		{
			particle.SetActive(false);
		}
		this._particles.Clear();
	}

	// Token: 0x040015D9 RID: 5593
	private int _settingCmdType = -1;

	// Token: 0x040015DA RID: 5594
	private readonly List<GameObject> _particles = new List<GameObject>();

	// Token: 0x040015DB RID: 5595
	[Tooltip("尺寸类型，选择后编辑器模式会即时预览")]
	[SerializeField]
	private CharacterTeammateCommand.ESizeType SizeType = CharacterTeammateCommand.ESizeType.Large;

	// Token: 0x020013C6 RID: 5062
	public enum ESizeType
	{
		// Token: 0x04009EE7 RID: 40679
		Small,
		// Token: 0x04009EE8 RID: 40680
		Large,
		// Token: 0x04009EE9 RID: 40681
		CombatBegin
	}
}
