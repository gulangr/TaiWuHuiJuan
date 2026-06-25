using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B8F RID: 2959
	public class SkillBreakPlateBonusGridLayout : MonoBehaviour
	{
		// Token: 0x060091F2 RID: 37362 RVA: 0x0043F32C File Offset: 0x0043D52C
		public void Refresh(IEnumerable<SkillBreakPlateBonus> bonusList, short skillId, LifeSkillShorts lifeSkillAttainments, IAsyncMethodRequestHandler requestHandler)
		{
			this.RefreshWithExtraBonus(bonusList, null, skillId, lifeSkillAttainments, requestHandler);
		}

		// Token: 0x060091F3 RID: 37363 RVA: 0x0043F33C File Offset: 0x0043D53C
		public void RefreshWithExtraBonus(IEnumerable<SkillBreakPlateBonus> bonusList, SectStoryBonusDisplayData emeiData, short skillId, LifeSkillShorts lifeSkillAttainments, IAsyncMethodRequestHandler requestHandler)
		{
			List<SkillBreakPlateBonus> list;
			if (bonusList != null)
			{
				list = (from b in bonusList
				where b.Type > ESkillBreakPlateBonusType.None
				select b).ToList<SkillBreakPlateBonus>();
			}
			else
			{
				list = new List<SkillBreakPlateBonus>();
			}
			List<SkillBreakPlateBonus> validBonusList = list;
			List<short> extraBonusIds = (emeiData != null) ? emeiData.BreakBonusTemplateIds : null;
			int extraCount = (extraBonusIds != null) ? extraBonusIds.Count : 0;
			int totalCount = validBonusList.Count + extraCount;
			bool flag = this.titleObj;
			if (flag)
			{
				this.titleObj.SetActive(totalCount > 0);
			}
			bool flag2 = totalCount == 0;
			if (flag2)
			{
				this.Clean();
			}
			else
			{
				CommonUtils.PrepareEnoughChildren(this.layout.transform, this.gridTemplate.gameObject, totalCount, null);
				List<SkillBreakBonusEffectDisplay> effectList = EasyPool.Get<List<SkillBreakBonusEffectDisplay>>();
				int gridIndex = 0;
				int i = 0;
				while (i < validBonusList.Count)
				{
					SkillBreakPlateBonusGrid grid = this.layout.GetChild(gridIndex).GetComponent<SkillBreakPlateBonusGrid>();
					grid.gameObject.SetActive(true);
					SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(skillId, validBonusList[i], lifeSkillAttainments, effectList);
					grid.SetTitle(validBonusList[i]);
					grid.SetContent(effectList);
					i++;
					gridIndex++;
				}
				bool flag3 = extraBonusIds != null && ((emeiData != null) ? emeiData.ExtraBonus : null) != null;
				if (flag3)
				{
					int j = 0;
					while (j < extraBonusIds.Count)
					{
						short bonusTypeId = extraBonusIds[j];
						SkillBreakPlateBonusGrid grid2 = this.layout.GetChild(gridIndex).GetComponent<SkillBreakPlateBonusGrid>();
						grid2.gameObject.SetActive(true);
						SkillBreakPlateGridBonusTypeItem bonusConfig = SkillBreakPlateGridBonusType.Instance[bonusTypeId];
						grid2.SetTitleText(bonusConfig.Name, 0);
						SkillBreakBonusEffectHelper.GenerateExtraBonusEffectDisplays(bonusTypeId, emeiData.ExtraBonus, effectList);
						grid2.SetContent(effectList);
						j++;
						gridIndex++;
					}
				}
				EasyPool.Free<List<SkillBreakBonusEffectDisplay>>(effectList);
			}
		}

		// Token: 0x060091F4 RID: 37364 RVA: 0x0043F530 File Offset: 0x0043D730
		public void Clean()
		{
			for (int i = this.layout.childCount - 1; i >= 0; i--)
			{
				Transform child = this.layout.GetChild(i);
				child.gameObject.SetActive(false);
			}
		}

		// Token: 0x04007077 RID: 28791
		[SerializeField]
		private Transform layout;

		// Token: 0x04007078 RID: 28792
		[SerializeField]
		private SkillBreakPlateBonusGrid gridTemplate;

		// Token: 0x04007079 RID: 28793
		[SerializeField]
		private GameObject titleObj;
	}
}
