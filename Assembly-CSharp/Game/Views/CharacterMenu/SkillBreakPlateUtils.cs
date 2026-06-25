using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B93 RID: 2963
	public static class SkillBreakPlateUtils
	{
		// Token: 0x0600924B RID: 37451 RVA: 0x00442994 File Offset: 0x00440B94
		public static void AsyncGetBonusName(IAsyncMethodRequestHandler requestHandler, SkillBreakPlateBonus bonus, Action<string> callback)
		{
			bool flag = bonus.Type == ESkillBreakPlateBonusType.None;
			if (flag)
			{
				callback(null);
			}
			else
			{
				bool flag2 = bonus.Type == ESkillBreakPlateBonusType.Exp;
				if (flag2)
				{
					callback(LocalStringManager.Get(LanguageKey.LK_Exp));
				}
				else
				{
					bool flag3 = bonus.Type == ESkillBreakPlateBonusType.Item;
					if (flag3)
					{
						string itemName = ItemTemplateHelper.GetName(bonus.ItemType, bonus.ItemTemplateId);
						callback(itemName);
					}
					else
					{
						CharacterDomainMethod.AsyncCall.GetNameRelatedData(requestHandler, bonus.RelationCharId, delegate(int offset, RawDataPool dataPool)
						{
							NameRelatedData nameData = new NameRelatedData();
							Serializer.Deserialize(dataPool, offset, ref nameData);
							string name = NameCenter.GetMonasticTitleOrDisplayName(ref nameData, false, false);
							callback(name);
						});
					}
				}
			}
		}

		// Token: 0x0600924C RID: 37452 RVA: 0x00442A44 File Offset: 0x00440C44
		public static void CalcBonusTypes(List<short> bonusTypes, SkillBreakBonusCollection bonusCollection)
		{
			bonusTypes.Clear();
			bool flag = bonusCollection.CharacterPropertyBonusDict.Count > 0;
			if (flag)
			{
				foreach (short templateId in bonusCollection.CharacterPropertyBonusDict.Keys)
				{
					bool flag2 = !bonusTypes.Contains(templateId);
					if (flag2)
					{
						bonusTypes.Add(templateId);
					}
				}
			}
			int totalCharPropertyCount = CharacterPropertyReferenced.Instance.Count;
			bool flag3 = bonusCollection.CombatSkillPropertyBonusDict.Count > 0;
			if (flag3)
			{
				foreach (short templateId2 in bonusCollection.CombatSkillPropertyBonusDict.Keys)
				{
					short resultId = (short)((int)templateId2 + totalCharPropertyCount);
					bool flag4 = !bonusTypes.Contains(resultId);
					if (flag4)
					{
						bonusTypes.Add(resultId);
					}
				}
			}
		}

		// Token: 0x0600924D RID: 37453 RVA: 0x00442B58 File Offset: 0x00440D58
		public static int GetNeedExp(short skillId, GameData.Domains.Taiwu.SkillBreakPlate plate, SkillBreakPlateIndex coordinate, int baseNeedExp)
		{
			return plate.CalcCostExp(baseNeedExp, coordinate);
		}

		// Token: 0x0600924E RID: 37454 RVA: 0x00442B74 File Offset: 0x00440D74
		public static bool IsNormalStepExhausted(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			return plate.StepCostedNormal >= plate.StepNormal && plate.StepCostedGoneMad > 0;
		}
	}
}
