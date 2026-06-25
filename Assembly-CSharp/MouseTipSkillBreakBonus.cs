using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UILogic.MouseTip;
using UnityEngine;

// Token: 0x020002D7 RID: 727
public class MouseTipSkillBreakBonus : MouseTipBase
{
	// Token: 0x06002B4D RID: 11085 RVA: 0x00151D35 File Offset: 0x0014FF35
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x00151D40 File Offset: 0x0014FF40
	public override void Refresh(ArgumentBox argsBox)
	{
		this.ReadArgs(argsBox);
		this.RefreshExpAndStep();
		this.RefreshDesc();
		this.RefreshPower();
		this.RefreshBonusName();
		this.RefreshBonusEffect();
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x00151D70 File Offset: 0x0014FF70
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

	// Token: 0x06002B50 RID: 11088 RVA: 0x00151E24 File Offset: 0x00150024
	private void RefreshBonusEffect()
	{
		SkillBreakPlateGrid cell = this._plate.GetGridAt(this._coord);
		bool flag = cell.State != ESkillBreakGridState.Selected;
		if (flag)
		{
			this.bonusEffectLayout.gameObject.SetActive(false);
			this.bonusEffectTitle.SetActive(false);
		}
		else
		{
			SkillBreakPlateBonus bonus = this._plate.GetBonus(this._coord);
			bool flag2 = bonus.Type == ESkillBreakPlateBonusType.None;
			if (flag2)
			{
				this.bonusEffectLayout.gameObject.SetActive(false);
				this.bonusEffectTitle.SetActive(false);
			}
			else
			{
				this.bonusEffectLayout.gameObject.SetActive(true);
				this.bonusEffectTitle.SetActive(true);
				this._resultList.Clear();
				SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(this._skillId, bonus, this._lifeSkillAttainments, this._resultList);
				CommonUtils.PrepareEnoughChildren(this.bonusEffectLayout.transform, this.bonusEffectItemTemplate.gameObject, this._resultList.Count, null);
				for (int i = 0; i < this._resultList.Count; i++)
				{
					SkillBreakBonusEffect bonusItem = this.bonusEffectLayout.transform.GetChild(i).GetComponent<SkillBreakBonusEffect>();
					bonusItem.Refresh(this._resultList[i], SkillBreakBonusEffect.EBonusIconSize.Small);
				}
			}
		}
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x00151F84 File Offset: 0x00150184
	private void RefreshBonusName()
	{
		MouseTipSkillBreakBonus.<>c__DisplayClass6_0 CS$<>8__locals1 = new MouseTipSkillBreakBonus.<>c__DisplayClass6_0();
		CS$<>8__locals1.<>4__this = this;
		SkillBreakPlateGrid cell = this._plate.GetGridAt(this._coord);
		bool flag = cell.State != ESkillBreakGridState.Selected;
		if (flag)
		{
			this.bonusNameArea.SetActive(false);
		}
		else
		{
			SkillBreakPlateBonus bonus = this._plate.GetBonus(this._coord);
			bool flag2 = bonus.Type == ESkillBreakPlateBonusType.None;
			if (flag2)
			{
				this.bonusNameArea.SetActive(false);
			}
			else
			{
				this.bonusNameArea.SetActive(true);
				CS$<>8__locals1.grade = bonus.Grade;
				switch (bonus.Type)
				{
				case ESkillBreakPlateBonusType.Item:
					CS$<>8__locals1.<RefreshBonusName>g__SetBonusName|0(ItemTemplateHelper.GetName(bonus.ItemType, bonus.ItemTemplateId));
					break;
				case ESkillBreakPlateBonusType.Relation:
				case ESkillBreakPlateBonusType.Friend:
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, bonus.RelationCharId, delegate(int offset, RawDataPool pool)
					{
						CharacterDisplayData characterDisplayData = null;
						Serializer.Deserialize(pool, offset, ref characterDisplayData);
						base.<RefreshBonusName>g__SetBonusName|0(NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, false));
					});
					break;
				case ESkillBreakPlateBonusType.Exp:
					CS$<>8__locals1.<RefreshBonusName>g__SetBonusName|0(LocalStringManager.Get(LanguageKey.LK_Exp));
					break;
				}
			}
		}
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x00152098 File Offset: 0x00150298
	private void RefreshPower()
	{
		SkillBreakPlateGrid cell = this._plate.GetGridAt(this._coord);
		bool flag = cell.State != ESkillBreakGridState.Selected;
		if (flag)
		{
			this.powerArea.SetActive(false);
		}
		else
		{
			this.powerArea.SetActive(true);
			SkillBreakPlateBonus bonus = this._plate.GetBonus(this._coord);
			int rangeIndex = (bonus.Type == ESkillBreakPlateBonusType.None) ? -1 : (bonus.ImpactRange - 1);
			string[] coloredPowerList = this._possiblePowerList.Select(delegate(int p, int i)
			{
				string color = (i == rangeIndex) ? "brightblue" : "grayscaleblack";
				return string.Format("+{0}", p).SetColor(color);
			}).ToArray<string>();
			string powerValue = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_BonusSelect_Info_Power_Value, coloredPowerList[0], coloredPowerList[1], coloredPowerList[2]).ColorReplace();
			this.power.text = LanguageKey.LK_Skill_Break_BonusSelect_Info_Power_Desc.Tr() + ":" + powerValue;
		}
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x00152178 File Offset: 0x00150378
	private void RefreshDesc()
	{
		SkillBreakPlateGrid cell = this._plate.GetGridAt(this._coord);
		bool flag = cell.State != ESkillBreakGridState.Selected;
		if (flag)
		{
			this.desc.text = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Bonus_Tip_Not_Connected);
		}
		else
		{
			bool flag2 = this._plate.GetBonus(this._coord).Type == ESkillBreakPlateBonusType.None;
			if (flag2)
			{
				this.desc.text = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Bonus_Tip_Not_Filled);
			}
			else
			{
				this.desc.text = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Bonus_Tip_Filled);
			}
		}
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x00152210 File Offset: 0x00150410
	private void ReadArgs(ArgumentBox argsBox)
	{
		argsBox.Get<LifeSkillShorts>("LifeSkillAttainments", out this._lifeSkillAttainments);
		argsBox.Get("SkillId", out this._skillId);
		argsBox.Get<SkillBreakPlate>("Plate", out this._plate);
		argsBox.Get<SkillBreakPlateIndex>("Coord", out this._coord);
		List<int> powerList;
		argsBox.Get<List<int>>("PossiblePowerList", out powerList);
		argsBox.Get("NeedExp", out this._needExp);
		argsBox.Get("CurrentExp", out this._currentExp);
		this._possiblePowerList.Clear();
		this._possiblePowerList.AddRange(powerList);
	}

	// Token: 0x04001F70 RID: 8048
	private readonly List<int> _possiblePowerList = new List<int>();

	// Token: 0x04001F71 RID: 8049
	private readonly List<SkillBreakBonusEffectDisplay> _resultList = new List<SkillBreakBonusEffectDisplay>();

	// Token: 0x04001F72 RID: 8050
	[SerializeField]
	private TextMeshProUGUI desc;

	// Token: 0x04001F73 RID: 8051
	[SerializeField]
	private TextMeshProUGUI power;

	// Token: 0x04001F74 RID: 8052
	[SerializeField]
	private TextMeshProUGUI bonusName;

	// Token: 0x04001F75 RID: 8053
	[SerializeField]
	private Transform bonusEffectLayout;

	// Token: 0x04001F76 RID: 8054
	[SerializeField]
	private SkillBreakBonusEffect bonusEffectItemTemplate;

	// Token: 0x04001F77 RID: 8055
	[SerializeField]
	private GameObject powerArea;

	// Token: 0x04001F78 RID: 8056
	[SerializeField]
	private GameObject bonusNameArea;

	// Token: 0x04001F79 RID: 8057
	[SerializeField]
	private GameObject bonusEffectTitle;

	// Token: 0x04001F7A RID: 8058
	[SerializeField]
	private BreakTipStepArea stepArea;

	// Token: 0x04001F7B RID: 8059
	private short _skillId;

	// Token: 0x04001F7C RID: 8060
	private LifeSkillShorts _lifeSkillAttainments;

	// Token: 0x04001F7D RID: 8061
	private SkillBreakPlate _plate;

	// Token: 0x04001F7E RID: 8062
	private SkillBreakPlateIndex _coord;

	// Token: 0x04001F7F RID: 8063
	private int _needExp;

	// Token: 0x04001F80 RID: 8064
	private int _currentExp;
}
