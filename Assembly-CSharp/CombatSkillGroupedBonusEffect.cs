using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class CombatSkillGroupedBonusEffect : MonoBehaviour
{
	// Token: 0x06001FA3 RID: 8099 RVA: 0x000E6768 File Offset: 0x000E4968
	public void RefreshBonusGroupedLayout(IAsyncMethodRequestHandler handler, int charId, short skillId, LifeSkillShorts lifeSkillAttainments)
	{
		CombatSkillDomainMethod.AsyncCall.GetCombatSkillBreakBonuses(handler, charId, skillId, delegate(int offset, RawDataPool pool)
		{
			List<SkillBreakPlateBonus> bonuses = null;
			Serializer.Deserialize(pool, offset, ref bonuses);
			List<ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>> result = new List<ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>>();
			if (bonuses == null)
			{
				bonuses = new List<SkillBreakPlateBonus>();
			}
			foreach (SkillBreakPlateBonus bonus2 in bonuses)
			{
				bool flag = bonus2.Type == ESkillBreakPlateBonusType.None;
				if (!flag)
				{
					List<SkillBreakBonusEffectDisplay> bonusEffectList = EasyPool.Get<List<SkillBreakBonusEffectDisplay>>();
					SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(skillId, bonus2, lifeSkillAttainments, bonusEffectList);
					result.Add(new ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>(bonus2, bonusEffectList));
				}
			}
			this.bonusEffectGroupLayout.gameObject.SetActive(result.Count > 0);
			CommonUtils.PrepareEnoughChildren(this.bonusEffectGroupLayout, this.bonusEffectGroupTemplate, result.Count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
			{
				ExtraItemCount = this.bonusEffectGroupLayoutExtraItemCount,
				TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
			}));
			for (int i = 0; i < result.Count; i++)
			{
				ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>> valueTuple = result[i];
				SkillBreakPlateBonus bonus = valueTuple.Item1;
				List<SkillBreakBonusEffectDisplay> bonusEffectList2 = valueTuple.Item2;
				Transform groupItem = this.bonusEffectGroupLayout.GetChild(i + this.bonusEffectGroupLayoutExtraItemCount);
				Refers groupRefers = groupItem.GetComponent<Refers>();
				TextMeshProUGUI bonusNameLabel = groupRefers.CGet<TextMeshProUGUI>("BonusName");
				SkillBreakPlateUtils.AsyncGetBonusName(handler, bonus, delegate(string bonusName)
				{
					bonusNameLabel.text = bonusName.SetGradeColor((int)bonus.Grade);
				});
				CommonUtils.PrepareEnoughChildren(groupItem, this.bonusEffectTemplate, bonusEffectList2.Count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = this.bonusEffectLayoutExtraItemCount,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				for (int j = 0; j < bonusEffectList2.Count; j++)
				{
					Transform bonusEffectItem = groupItem.GetChild(j + this.bonusEffectLayoutExtraItemCount);
					SkillBreakBonusEffect bonusItem = bonusEffectItem.GetComponent<SkillBreakBonusEffect>();
					bonusItem.Refresh(bonusEffectList2[j], SkillBreakBonusEffect.EBonusIconSize.Small);
				}
			}
		});
	}

	// Token: 0x040017C8 RID: 6088
	[SerializeField]
	private RectTransform bonusEffectGroupLayout;

	// Token: 0x040017C9 RID: 6089
	[SerializeField]
	private GameObject bonusEffectGroupTemplate;

	// Token: 0x040017CA RID: 6090
	[SerializeField]
	private GameObject bonusEffectTemplate;

	// Token: 0x040017CB RID: 6091
	[SerializeField]
	private int bonusEffectGroupLayoutExtraItemCount = 2;

	// Token: 0x040017CC RID: 6092
	[SerializeField]
	private int bonusEffectLayoutExtraItemCount = 1;
}
