using System;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using TMPro;
using UILogic.MouseTip;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class MouseTipSkillBreakNormalCell : MouseTipBase
{
	// Token: 0x06002B56 RID: 11094 RVA: 0x001522D0 File Offset: 0x001504D0
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x001522DC File Offset: 0x001504DC
	public override void Refresh(ArgumentBox argsBox)
	{
		this.ReadArgs(argsBox);
		this.SetupLayout(!this._isStartPoint && !this._isEndPoint);
		bool isStartPoint = this._isStartPoint;
		if (isStartPoint)
		{
			this.RefreshStartPoint();
		}
		else
		{
			bool isEndPoint = this._isEndPoint;
			if (isEndPoint)
			{
				this.RefreshEndPoint();
				this.RefreshExpAndStep();
			}
			else
			{
				this.RefreshSuccessRate();
				this.RefreshExpAndStep();
				this.RefreshPower();
				this.RefreshDesc();
				this.RefreshTitle();
			}
		}
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x00152364 File Offset: 0x00150564
	private void RefreshStartPoint()
	{
		CombatSkillItem skillConfig = CombatSkill.Instance[this._skillId];
		this.titleLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_SkillBreak_Tips_StartPointTitle, skillConfig.BreakStart);
		this.descLabel.text = LocalStringManager.Get(LanguageKey.LK_SkillBreak_Tips_StartPointDesc);
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x001523B8 File Offset: 0x001505B8
	private void RefreshEndPoint()
	{
		CombatSkillItem skillConfig = CombatSkill.Instance[this._skillId];
		this.titleLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_SkillBreak_Tips_EndPointTitle, skillConfig.BreakEnd);
		this.descLabel.text = LocalStringManager.Get(LanguageKey.LK_SkillBreak_Tips_EndPointDesc);
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x0015240C File Offset: 0x0015060C
	private void SetupLayout(bool normalMode)
	{
		foreach (GameObject item in this.normalModeObjects)
		{
			item.SetActive(normalMode);
		}
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x00152440 File Offset: 0x00150640
	private void RefreshSuccessRate()
	{
		short successRate = this._plate.CalcSuccessRate(this._coord);
		this.successRateLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Cell_SuccessRate, string.Format("{0}%", successRate)).ColorReplace();
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x0015248C File Offset: 0x0015068C
	private void RefreshExpAndStep()
	{
		SkillBreakPlateGrid cell = this._plate.GetGridAt(this._coord);
		bool needShow = cell.State == ESkillBreakGridState.CanSelect;
		this.stepArea.gameObject.SetActive(needShow);
		bool flag = needShow;
		if (flag)
		{
			byte cost = this._plate.CalcCostStep(this._coord);
			this.stepArea.RefreshStep(this._plate.StepNormal - this._plate.StepCostedNormal, this._plate.StepGoneMad - this._plate.StepCostedGoneMad, (int)cost, SkillBreakPlateUtils.IsNormalStepExhausted(this._plate));
			this.stepArea.RefreshExp(this._currentExp, this._needExp);
		}
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x00152540 File Offset: 0x00150740
	private void RefreshPower()
	{
		int power = this._plate.CalcAddMaxPower(this._coord);
		this.powerLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Cell_Power, MouseTipSkillBreakNormalCell.GetColoredPower(power)).ColorReplace();
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x00152584 File Offset: 0x00150784
	private static string GetColoredPower(int power)
	{
		if (!true)
		{
		}
		string result;
		if (power < 3)
		{
			switch (power)
			{
			case 0:
				result = power.ToString().SetColor("lightgrey");
				break;
			case 1:
				result = power.ToString().SetColor("pinkyellow");
				break;
			case 2:
				result = power.ToString().SetColor("GradeColor_6");
				break;
			default:
				result = power.ToString().SetColor("lightgrey");
				break;
			}
		}
		else
		{
			result = power.ToString().SetColor("skillbreakyellow");
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x0015261C File Offset: 0x0015081C
	private void RefreshDesc()
	{
		SkillBreakPlateGrid cellData = this._plate.GetGridAt(this._coord);
		SkillBreakGridTypeItem config = cellData.Template;
		this.descLabel.text = config.Desc.SetColor("pinkyellow").ColorReplace();
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x00152664 File Offset: 0x00150864
	private void RefreshTitle()
	{
		SkillBreakPlateGrid cellData = this._plate.GetGridAt(this._coord);
		SkillBreakGridTypeItem config = cellData.Template;
		this.titleLabel.text = config.Name.ColorReplace();
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x001526A4 File Offset: 0x001508A4
	private void ReadArgs(ArgumentBox argsBox)
	{
		argsBox.Get("SkillId", out this._skillId);
		argsBox.Get("StartPoint", out this._isStartPoint);
		argsBox.Get("EndPoint", out this._isEndPoint);
		argsBox.Get<LifeSkillShorts>("LifeSkillAttainments", out this._lifeSkillAttainments);
		argsBox.Get<GameData.Domains.Taiwu.SkillBreakPlate>("Plate", out this._plate);
		argsBox.Get<SkillBreakPlateIndex>("Coord", out this._coord);
		argsBox.Get("NeedExp", out this._needExp);
		argsBox.Get("CurrentExp", out this._currentExp);
	}

	// Token: 0x04001F81 RID: 8065
	[SerializeField]
	private BreakTipStepArea stepArea;

	// Token: 0x04001F82 RID: 8066
	[SerializeField]
	private TextMeshProUGUI successRateLabel;

	// Token: 0x04001F83 RID: 8067
	[SerializeField]
	private TextMeshProUGUI powerLabel;

	// Token: 0x04001F84 RID: 8068
	[SerializeField]
	private TextMeshProUGUI descLabel;

	// Token: 0x04001F85 RID: 8069
	[SerializeField]
	private TextMeshProUGUI titleLabel;

	// Token: 0x04001F86 RID: 8070
	[SerializeField]
	private GameObject[] normalModeObjects;

	// Token: 0x04001F87 RID: 8071
	private short _skillId;

	// Token: 0x04001F88 RID: 8072
	private LifeSkillShorts _lifeSkillAttainments;

	// Token: 0x04001F89 RID: 8073
	private GameData.Domains.Taiwu.SkillBreakPlate _plate;

	// Token: 0x04001F8A RID: 8074
	private SkillBreakPlateIndex _coord;

	// Token: 0x04001F8B RID: 8075
	private int _needExp;

	// Token: 0x04001F8C RID: 8076
	private int _currentExp;

	// Token: 0x04001F8D RID: 8077
	private bool _isStartPoint;

	// Token: 0x04001F8E RID: 8078
	private bool _isEndPoint;
}
