using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.Exchange;
using GameData.Domains.Organization;
using TMPro;
using UnityEngine;

// Token: 0x020002D1 RID: 721
public class MouseTipSettlementTreasuryOrPrisonLayer : MouseTipBase
{
	// Token: 0x06002B30 RID: 11056 RVA: 0x001511A8 File Offset: 0x0014F3A8
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitRefers();
		this.Refresh(argsBox);
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x001511BC File Offset: 0x0014F3BC
	public override void Refresh(ArgumentBox argsBox)
	{
		int layerIndex;
		argsBox.Get("layerIndex", out layerIndex);
		bool isSect;
		argsBox.Get("isSect", out isSect);
		bool isDebtOrSupportEnough;
		argsBox.Get("isDebtOrSupportEnough", out isDebtOrSupportEnough);
		bool isPrison;
		argsBox.Get("isPrison", out isPrison);
		SettlementTreasuryLayers settlementTreasuryLayers = (SettlementTreasuryLayers)layerIndex;
		string layer = settlementTreasuryLayers.ToString();
		if (!true)
		{
		}
		ValueTuple<string, string, string> valueTuple;
		switch (layerIndex)
		{
		case 0:
			valueTuple = new ValueTuple<string, string, string>(LanguageKey.LK_SettlementTreasury_Toggle_Shallow.Tr(), LanguageKey.LK_SettlementPrison_Tog_Low.Tr(), LanguageKey.LK_SettlementPrison_Tips_Low_Desc.Tr());
			break;
		case 1:
			valueTuple = new ValueTuple<string, string, string>(LanguageKey.LK_SettlementTreasury_Toggle_Mid.Tr(), LanguageKey.LK_SettlementPrison_Tog_Mid.Tr(), LanguageKey.LK_SettlementPrison_Tips_Mid_Desc.Tr());
			break;
		case 2:
			valueTuple = new ValueTuple<string, string, string>(LanguageKey.LK_SettlementTreasury_Toggle_Deep.Tr(), LanguageKey.LK_SettlementPrison_Tog_High.Tr(), LanguageKey.LK_SettlementPrison_Tips_High_Desc.Tr());
			break;
		default:
			valueTuple = new ValueTuple<string, string, string>(string.Empty, LanguageKey.LK_SettlementPrison_Tog_CompletelyInfected.Tr(), LanguageKey.LK_SettlementPrison_Tips_Infected_Desc.Tr());
			break;
		}
		if (!true)
		{
		}
		ValueTuple<string, string, string> valueTuple2 = valueTuple;
		string prisonPrefix = valueTuple2.Item1;
		string prisonName = valueTuple2.Item2;
		string desc = valueTuple2.Item3;
		int prisonerCount;
		bool flag = isPrison && argsBox.Get("PrisonerCount", out prisonerCount);
		if (flag)
		{
			this.prisoners.gameObject.SetActive(true);
			TMP_Text tmp_Text = this.prisoners;
			bool flag2 = layerIndex >= 0 && layerIndex <= 2;
			bool flag3 = prisonerCount > 0;
			if (!true)
			{
			}
			string text;
			if (flag2)
			{
				if (!flag3)
				{
					text = LanguageKey.LK_MouseTip_Prisoner_Empty.TrFormat(prisonPrefix).ColorReplace();
				}
				else
				{
					text = LanguageKey.LK_MouseTip_Prisoner_Count.TrFormat(prisonPrefix, prisonerCount).ColorReplace();
				}
			}
			else if (!flag3)
			{
				text = LanguageKey.LK_MouseTip_Prisoner_Empty_Infected.Tr().ColorReplace();
			}
			else
			{
				text = LanguageKey.LK_MouseTip_Prisoner_Count_Infected.TrFormat(prisonerCount).ColorReplace();
			}
			if (!true)
			{
			}
			tmp_Text.text = text;
		}
		else
		{
			this.prisoners.gameObject.SetActive(false);
		}
		this._name.text = ((!isPrison) ? LocalStringManager.Get("LK_SettlementTreasury_Title_" + layer) : prisonName);
		this._desc.text = ((!isPrison) ? LocalStringManager.Get("LK_SettlementTreasury_Desc_" + layer) : desc);
		bool flag4 = layerIndex == 0;
		if (flag4)
		{
			this._none.SetActive(true);
			this._approving.SetActive(false);
			this._spiritualDebt.SetActive(false);
			this._gradeLimit.SetActive(isPrison);
		}
		else
		{
			bool flag5 = layerIndex == 3;
			if (flag5)
			{
				this._none.SetActive(true);
				this._approving.SetActive(false);
				this._spiritualDebt.SetActive(false);
				this._gradeLimit.SetActive(true);
				for (int i = 0; i < this._gradeTextList.Count; i++)
				{
					bool flag6 = i == 0;
					if (flag6)
					{
						this._gradeTextList[i].text = LanguageKey.LK_SettlementPrison_Tog_CompletelyInfected.Tr();
					}
					else
					{
						this._gradeList[i].SetActive(false);
					}
				}
			}
			else
			{
				this._none.SetActive(false);
				this._approving.SetActive(isSect);
				this._spiritualDebt.SetActive(!isSect);
				this._gradeLimit.SetActive(isPrison);
				bool flag7 = isSect;
				if (flag7)
				{
					this._approvingText.text = LocalStringManager.GetFormat(LocalStringManager.Get("LK_SettlementTreasury_Require_Approving"), string.Format("{0}%", (layerIndex == 1) ? GlobalConfig.Instance.TreasuryRquireApprovingMid : GlobalConfig.Instance.TreasuryRquireApprovingHigh).SetColor("pinkyellow")).ColorReplace();
					this._approvingText.GetComponent<DisableStyleRoot>().SetStyleEffect(!isDebtOrSupportEnough, false);
				}
				else
				{
					this._spiritualDebtText.text = LocalStringManager.GetFormat(LocalStringManager.Get("LK_SettlementTreasury_Require_SpiritualDebt"), string.Format("{0}", (layerIndex == 1) ? GlobalConfig.Instance.TreasuryRquireSpiritualDebtMid : GlobalConfig.Instance.TreasuryRquireSpiritualDebtHigh).SetColor("pinkyellow")).ColorReplace();
					this._spiritualDebtText.GetComponent<DisableStyleRoot>().SetStyleEffect(!isDebtOrSupportEnough, false);
				}
			}
		}
		bool flag8 = isPrison && layerIndex != 3;
		if (flag8)
		{
			for (int j = 0; j < this._gradeTextList.Count; j++)
			{
				int grade = layerIndex * 3 + j;
				this._gradeTextList[j].text = LocalStringManager.Get(string.Format("LK_OrgGrade_{0}", grade)).SetGradeColor(grade);
				this._gradeList[j].SetActive(true);
			}
		}
		this._unFinishedNotice.SetActive(!isPrison && layerIndex != (int)ViewSettlementShop.SettlementTreasuryLayerIndex && ViewSettlementShop.IsExistTradeItems);
		this._unFinishedNotice.GetComponent<TextMeshProUGUI>().text.ColorReplace();
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x001516D4 File Offset: 0x0014F8D4
	private void InitRefers()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._gradeList = base.CGetList<GameObject>("Grade_");
			this._gradeTextList = base.CGetList<TextMeshProUGUI>("GradeText_");
			this._favorabilityText = base.CGet<TextMeshProUGUI>("FavorabilityText");
			this._approvingText = base.CGet<TextMeshProUGUI>("ApprovingText");
			this._spiritualDebtText = base.CGet<TextMeshProUGUI>("SpiritualDebtText");
			this._approving = base.CGet<GameObject>("Approving");
			this._spiritualDebt = base.CGet<GameObject>("SpiritualDebt");
			this._none = base.CGet<GameObject>("None");
			this._favorability = base.CGet<GameObject>("Favorability");
			this._requirementNotice = base.CGet<GameObject>("RequirementNotice");
			this._name = base.CGet<TextMeshProUGUI>("Name");
			this._desc = base.CGet<TextMeshProUGUI>("Desc");
			this._fullCharNameButton = base.CGet<GameObject>("FullCharNameButton");
			this._rightPartCharNameButton = base.CGet<GameObject>("RightPartCharNameButton");
			this._leftPartCharNameButton = base.CGet<GameObject>("LeftPartCharNameButton");
			this._favorabilityNew = base.CGet<Refers>("FavorabilityNew");
			this._gradeLimit = base.CGet<GameObject>("GradeLimit");
			this._unFinishedNotice = base.CGet<GameObject>("UnFinishedNotice");
			this._inited = true;
		}
	}

	// Token: 0x04001F4D RID: 8013
	[SerializeField]
	public TMP_Text prisoners;

	// Token: 0x04001F4E RID: 8014
	private bool _inited;

	// Token: 0x04001F4F RID: 8015
	private List<GameObject> _gradeList;

	// Token: 0x04001F50 RID: 8016
	private List<TextMeshProUGUI> _gradeTextList;

	// Token: 0x04001F51 RID: 8017
	private TextMeshProUGUI _favorabilityText;

	// Token: 0x04001F52 RID: 8018
	private TextMeshProUGUI _approvingText;

	// Token: 0x04001F53 RID: 8019
	private TextMeshProUGUI _spiritualDebtText;

	// Token: 0x04001F54 RID: 8020
	private GameObject _approving;

	// Token: 0x04001F55 RID: 8021
	private GameObject _spiritualDebt;

	// Token: 0x04001F56 RID: 8022
	private GameObject _none;

	// Token: 0x04001F57 RID: 8023
	private GameObject _favorability;

	// Token: 0x04001F58 RID: 8024
	private GameObject _requirementNotice;

	// Token: 0x04001F59 RID: 8025
	private TextMeshProUGUI _name;

	// Token: 0x04001F5A RID: 8026
	private TextMeshProUGUI _desc;

	// Token: 0x04001F5B RID: 8027
	private GameObject _fullCharNameButton;

	// Token: 0x04001F5C RID: 8028
	private GameObject _rightPartCharNameButton;

	// Token: 0x04001F5D RID: 8029
	private GameObject _leftPartCharNameButton;

	// Token: 0x04001F5E RID: 8030
	private Refers _favorabilityNew;

	// Token: 0x04001F5F RID: 8031
	private GameObject _gradeLimit;

	// Token: 0x04001F60 RID: 8032
	private GameObject _unFinishedNotice;
}
