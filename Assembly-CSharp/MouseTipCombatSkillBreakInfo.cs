using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using Game.Views.SkillBreak;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UISkillBreakPlate;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class MouseTipCombatSkillBreakInfo : MouseTipBase
{
	// Token: 0x06002993 RID: 10643 RVA: 0x0013ADF4 File Offset: 0x00138FF4
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x0013ADFF File Offset: 0x00138FFF
	public override void Refresh(ArgumentBox argsBox)
	{
		this.ReadArgs(argsBox);
		this.RefreshPageEffects();
		this.RefreshBonusEffects();
		this.RefreshPowerDesc();
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x0013AE20 File Offset: 0x00139020
	private void RefreshPowerDesc()
	{
		CombatSkillDomainMethod.AsyncCall.GetCombatSkillBreakoutStepsMaxPower(this, this._charId, this._skillId, delegate(int offset, RawDataPool pool)
		{
			int maxPower = -1;
			Serializer.Deserialize(pool, offset, ref maxPower);
			this.powerDesc.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_InfoTip_Power_Desc, maxPower).ColorReplace() + ViewCharacterMenuSkillBreakPlate.GetBreakoutMaxPowerName(this._skillId, maxPower, true);
		});
		this.combatSkillPowerDesc.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_InfoTip_CombatSkill_Power_Desc, this._combatSkillPower).ColorReplace();
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x0013AE74 File Offset: 0x00139074
	private void RefreshPageEffects()
	{
		int activateCount = 0;
		for (int i = 0; i < 10; i++)
		{
			bool flag = CombatSkillStateHelper.IsPageActive(this._activateState, (byte)(i + 5));
			if (flag)
			{
				activateCount++;
			}
		}
		List<string> allEffectList = EasyPool.Get<List<string>>();
		allEffectList.Clear();
		List<string> pageEffectList = EasyPool.Get<List<string>>();
		StringBuilder pageNameSb = EasyPool.Get<StringBuilder>();
		pageNameSb.Clear();
		for (int j = 0; j < activateCount; j++)
		{
			bool isDirect = CombatSkillStateHelper.GetPageActiveDirection(this._activateState, (byte)(j + 1)) == 0;
			int pageEffectId = isDirect ? j : (j + 5);
			PageEffectHelper.GenerateNormalPageEffects(this._skillId, pageEffectId, pageEffectList);
			allEffectList.AddRange(pageEffectList);
			int indexInDirection = j % 5;
			string otherPageName = LocalStringManager.Get(string.Format("LK_CombatSkill_{0}_Page_{1}", isDirect ? "Direct" : "Reverse", indexInDirection)).SetColor(isDirect ? "brightblue" : "brightred");
			pageNameSb.Append(otherPageName);
		}
		this.pageName.text = pageNameSb.ToString().ColorReplace();
		EasyPool.Free<StringBuilder>(pageNameSb);
		EasyPool.Free<List<string>>(pageEffectList);
		CommonUtils.PrepareEnoughChildren(this.pageEffectLayout, this.pageEffectTemplate, allEffectList.Count, null);
		for (int k = 0; k < allEffectList.Count; k++)
		{
			Transform pageEffectItem = this.pageEffectLayout.GetChild(k);
			TextMeshProUGUI pageEffect = pageEffectItem.GetComponent<TextMeshProUGUI>();
			pageEffect.text = allEffectList[k].ColorReplace();
			pageEffect.GetComponent<TMPTextSpriteHelper>().Parse();
		}
		EasyPool.Free<List<string>>(allEffectList);
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x0013B022 File Offset: 0x00139222
	private void RefreshBonusEffects()
	{
		this.bonusEffectGroup.RefreshBonusGroupedLayout(this, this._charId, this._skillId, this._lifeSkillAttainments);
	}

	// Token: 0x06002998 RID: 10648 RVA: 0x0013B044 File Offset: 0x00139244
	private void ReadArgs(ArgumentBox argsBox)
	{
		argsBox.Get("SkillId", out this._skillId);
		argsBox.Get("CharId", out this._charId);
		argsBox.Get("ActivateState", out this._activateState);
		argsBox.Get<LifeSkillShorts>("LifeSkillAttainments", out this._lifeSkillAttainments);
		argsBox.Get("CombatSkillPower", out this._combatSkillPower);
	}

	// Token: 0x04001E22 RID: 7714
	private short _skillId;

	// Token: 0x04001E23 RID: 7715
	private int _charId;

	// Token: 0x04001E24 RID: 7716
	private ushort _activateState;

	// Token: 0x04001E25 RID: 7717
	private LifeSkillShorts _lifeSkillAttainments;

	// Token: 0x04001E26 RID: 7718
	private short _combatSkillPower;

	// Token: 0x04001E27 RID: 7719
	[SerializeField]
	private RectTransform pageEffectLayout;

	// Token: 0x04001E28 RID: 7720
	[SerializeField]
	private GameObject pageEffectTemplate;

	// Token: 0x04001E29 RID: 7721
	[SerializeField]
	private TextMeshProUGUI pageName;

	// Token: 0x04001E2A RID: 7722
	[SerializeField]
	private CombatSkillGroupedBonusEffect bonusEffectGroup;

	// Token: 0x04001E2B RID: 7723
	[SerializeField]
	private TextMeshProUGUI powerDesc;

	// Token: 0x04001E2C RID: 7724
	[SerializeField]
	private TextMeshProUGUI combatSkillPowerDesc;

	// Token: 0x04001E2D RID: 7725
	[SerializeField]
	private int pageEffectGroupLayoutExtraItemCount = 1;

	// Token: 0x04001E2E RID: 7726
	[SerializeField]
	private int pageEffectLayoutExtraItemCount = 1;
}
