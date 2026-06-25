using System;
using System.Text;
using Config;
using FrameWork;
using Game.Views.Combat;
using Game.Views.Encyclopedia;
using GameData.Domains.Combat;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class MouseTipNeiliAllocation : MouseTipBase
{
	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x06002AEB RID: 10987 RVA: 0x0014AA44 File Offset: 0x00148C44
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x0014AA48 File Offset: 0x00148C48
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		sbyte neiliType;
		argsBox.Get("neiliType", out neiliType);
		byte type;
		argsBox.Get("type", out type);
		short current;
		argsBox.Get("current", out current);
		short origin;
		argsBox.Get("origin", out origin);
		short effectValue;
		argsBox.Get("effect", out effectValue);
		bool isNeiliLack;
		argsBox.Get("isNeiliLack", out isNeiliLack);
		GameObject effectLayout = base.CGet<GameObject>("EffectLayout");
		GameObject layout = base.CGet<GameObject>("Layout");
		GameObject effectTemplate = base.CGet<GameObject>("EffectTemplate");
		base.CGet<GameObject>("LayoutNeiliLack").SetActive(isNeiliLack);
		string effectDescStr = MouseTip_Util.GetNeiliAllocationTips(neiliType, type, origin, effectValue);
		string[] effectStrs = effectDescStr.Split('\n', StringSplitOptions.None);
		int childCount = 0;
		for (int i = 0; i < effectLayout.transform.childCount; i++)
		{
			effectLayout.transform.GetChild(i).gameObject.SetActive(false);
		}
		foreach (string effectStr in effectStrs)
		{
			bool flag = effectStr.IsNullOrEmpty();
			if (!flag)
			{
				childCount++;
				bool flag2 = childCount <= effectLayout.transform.childCount;
				GameObject effect;
				if (flag2)
				{
					effect = effectLayout.transform.GetChild(childCount - 1).gameObject;
				}
				else
				{
					effect = Object.Instantiate<GameObject>(effectTemplate, effectLayout.transform, false);
				}
				effect.SetActive(true);
				TextMeshProUGUI effectDesc = effect.GetComponentInChildren<TextMeshProUGUI>();
				effectDesc.text = effectStr.ColorReplace();
				effectDesc.GetComponent<TMPTextSpriteHelper>().Parse();
			}
		}
		layout.SetActive(!effectDescStr.IsNullOrEmpty());
		bool flag3 = origin <= 0;
		int neiliAllocationPercent;
		if (flag3)
		{
			neiliAllocationPercent = 100;
		}
		else
		{
			neiliAllocationPercent = (int)(current * 100 / origin);
		}
		string titleStr = LocalStringManager.Get(string.Format("LK_Neili_Allocation_Type_{0}", type));
		string titleStr2 = titleStr + LocalStringManager.Get(LanguageKey.LK_Neili_Allocation);
		base.CGet<TextMeshProUGUI>("Name").text = titleStr2;
		string descStr = LocalStringManager.Get(string.Format("LK_NeiliDesc_{0}", type));
		base.CGet<TextMeshProUGUI>("Desc").text = descStr;
		ENeiliAllocationStatusType eNeiliAllocationStatus = NeiliAllocationStatusHelper.GetStatus(current, origin);
		NeiliAllocationStatusItem neiliAllocationStatusItem = eNeiliAllocationStatus.GetConfig();
		string currentValue = string.Format("{0}%", neiliAllocationPercent);
		Refers currentState = base.CGet<Refers>("CurrentState");
		CImage neiliIcon = currentState.CGet<CImage>("NeiliIcon");
		TextMeshProUGUI currentStateText = currentState.CGet<TextMeshProUGUI>("CurrentStateText");
		this._sb.Clear();
		string neiliName2 = this._sb.Append("「").Append(titleStr2).Append("」").ToString().SetColor(CombatNeiliAllocation.NeiliAllocationFontColor[(int)type]);
		this._sb.Clear();
		string neiliName3 = this._sb.Append("「").Append(titleStr).Append("」").ToString().SetColor(CombatNeiliAllocation.NeiliAllocationFontColor[(int)type]);
		this._sb.Clear();
		string currentStateDesc = this._sb.Append(neiliAllocationStatusItem.Name).Append("（").Append(currentValue).Append("）").ToString();
		bool flag4 = eNeiliAllocationStatus < ENeiliAllocationStatusType.None;
		if (flag4)
		{
			currentStateDesc = currentStateDesc.SetColor("brightred");
		}
		else
		{
			bool flag5 = eNeiliAllocationStatus == ENeiliAllocationStatusType.None;
			if (flag5)
			{
				currentStateDesc = currentStateDesc.SetColor("pinkyellow");
			}
			else
			{
				currentStateDesc = currentStateDesc.SetColor("brightblue");
			}
		}
		currentStateText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Neili_Content, neiliName2, currentStateDesc);
		Refers costState = base.CGet<Refers>("CostState");
		CImage neiliIcon2 = costState.CGet<CImage>("NeiliIcon");
		TextMeshProUGUI costStateText = costState.CGet<TextMeshProUGUI>("CostStateText");
		costStateText.SetText(neiliAllocationStatusItem.Desc[0].GetFormat(neiliName2).ColorReplace(), true);
		neiliIcon.SetSprite(string.Format("mousetip_zhenqi_{0}", type), false, null);
		neiliIcon2.SetSprite(string.Format("mousetip_zhenqi_{0}", type), false, null);
		Refers powerState = base.CGet<Refers>("PowerState");
		TextMeshProUGUI powerStateText = powerState.CGet<TextMeshProUGUI>("PowerStateText");
		powerStateText.SetText(neiliAllocationStatusItem.Desc[1].GetFormat(neiliName3).ColorReplace(), true);
		Refers goneMadInjuryState = base.CGet<Refers>("GoneMadInjuryState");
		TextMeshProUGUI goneMadInjuryStateText = goneMadInjuryState.CGet<TextMeshProUGUI>("GoneMadInjuryStateText");
		goneMadInjuryStateText.SetText(neiliAllocationStatusItem.Desc[2].GetFormat(neiliName3).ColorReplace(), true);
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x0014AEE8 File Offset: 0x001490E8
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.TrueQiState);
		}
	}

	// Token: 0x04001F02 RID: 7938
	private StringBuilder _sb = new StringBuilder();
}
