using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;

namespace UISkillBreakPlate
{
	// Token: 0x0200041B RID: 1051
	public static class PageEffectHelper
	{
		// Token: 0x06003E73 RID: 15987 RVA: 0x001F5348 File Offset: 0x001F3548
		public static void GenerateNormalPageEffects(short combatSkillId, int pageEffectId, List<string> resultList)
		{
			resultList.Clear();
			CombatSkillItem skillConfig = CombatSkill.Instance[combatSkillId];
			sbyte equipType = skillConfig.EquipType;
			SkillBreakPageEffectItem pageConfig = SkillBreakPageEffect.Instance[pageEffectId];
			if (!true)
			{
			}
			sbyte b;
			switch (equipType)
			{
			case 0:
				b = pageConfig.EffectNeigong;
				break;
			case 1:
				b = pageConfig.EffectAttack;
				break;
			case 2:
				b = pageConfig.EffectAgile;
				break;
			case 3:
				b = pageConfig.EffectDefense;
				break;
			case 4:
				b = pageConfig.EffectAssist;
				break;
			default:
				throw new NotImplementedException();
			}
			if (!true)
			{
			}
			sbyte implementId = b;
			SkillBreakPageEffectImplementItem implementConfig = SkillBreakPageEffectImplement.Instance[implementId];
			List<SkillBreakPageEffectDisplay> effectDisplayList = EasyPool.Get<List<SkillBreakPageEffectDisplay>>();
			PageEffectHelper.CalcPageEffect(implementConfig, effectDisplayList, skillConfig);
			resultList.AddRange(from d in effectDisplayList
			select d.ToIconEmbedString());
			effectDisplayList.Clear();
			EasyPool.Free<List<SkillBreakPageEffectDisplay>>(effectDisplayList);
		}

		// Token: 0x06003E74 RID: 15988 RVA: 0x001F5438 File Offset: 0x001F3638
		public static void GenerateNormalPageEffects(short combatSkillId, int pageEffectId, List<SkillBreakPageEffectDisplay> resultList)
		{
			resultList.Clear();
			CombatSkillItem skillConfig = CombatSkill.Instance[combatSkillId];
			sbyte equipType = skillConfig.EquipType;
			SkillBreakPageEffectItem pageConfig = SkillBreakPageEffect.Instance[pageEffectId];
			if (!true)
			{
			}
			sbyte b;
			switch (equipType)
			{
			case 0:
				b = pageConfig.EffectNeigong;
				break;
			case 1:
				b = pageConfig.EffectAttack;
				break;
			case 2:
				b = pageConfig.EffectAgile;
				break;
			case 3:
				b = pageConfig.EffectDefense;
				break;
			case 4:
				b = pageConfig.EffectAssist;
				break;
			default:
				throw new NotImplementedException();
			}
			if (!true)
			{
			}
			sbyte implementId = b;
			SkillBreakPageEffectImplementItem implementConfig = SkillBreakPageEffectImplement.Instance[implementId];
			List<SkillBreakPageEffectDisplay> effectDisplayList = EasyPool.Get<List<SkillBreakPageEffectDisplay>>();
			PageEffectHelper.CalcPageEffect(implementConfig, effectDisplayList, skillConfig);
			resultList.AddRange(effectDisplayList);
			effectDisplayList.Clear();
			EasyPool.Free<List<SkillBreakPageEffectDisplay>>(effectDisplayList);
		}

		// Token: 0x06003E75 RID: 15989 RVA: 0x001F5503 File Offset: 0x001F3703
		public static void CalcPageEffect(SkillBreakPageEffectImplementItem pageEffectItem, List<SkillBreakPageEffectDisplay> outEffectDisplayList, CombatSkillItem skillConfig)
		{
			new PageEffectGenerator(pageEffectItem, outEffectDisplayList, skillConfig).Generate();
		}
	}
}
