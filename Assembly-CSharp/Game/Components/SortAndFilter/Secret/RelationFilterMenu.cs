using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CDD RID: 3293
	public class RelationFilterMenu : DetailedFilterMenuLogic<SecretSortAndFilterData>
	{
		// Token: 0x17001160 RID: 4448
		// (get) Token: 0x0600A63F RID: 42559 RVA: 0x004D642C File Offset: 0x004D462C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001161 RID: 4449
		// (get) Token: 0x0600A640 RID: 42560 RVA: 0x004D642F File Offset: 0x004D462F
		public override int Id
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x0600A641 RID: 42561 RVA: 0x004D6434 File Offset: 0x004D4634
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Filter;
		}

		// Token: 0x0600A642 RID: 42562 RVA: 0x004D6450 File Offset: 0x004D4650
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> res = new List<FilterDropdownItemConfig>();
			for (short type = 0; type <= 9; type += 1)
			{
				res.Add(new FilterDropdownItemConfig(RelationDisplayType.Instance[type].Name));
			}
			res.Add(new FilterDropdownItemConfig(LanguageKey.LK_SecretInformationFilter_Related_Actually));
			res.Add(new FilterDropdownItemConfig(LanguageKey.LK_SecretInformationFilter_Related_Fake));
			return res;
		}

		// Token: 0x0600A643 RID: 42563 RVA: 0x004D64CC File Offset: 0x004D46CC
		public override bool IsDataMatch(SecretSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				int num = selectedSubType;
				int num2 = num;
				if (num2 > 9)
				{
					if (num2 != 10)
					{
						if (num2 == 11)
						{
							foreach (CharacterDisplayData characterData in data.Characters.Values)
							{
								bool flag = characterData.OrgInfo.OrgTemplateId != 16 && characterData.RelationFromTaiwu == 0 && characterData.RelationToTaiwu == 0;
								if (flag)
								{
									return true;
								}
							}
						}
					}
					else
					{
						foreach (CharacterDisplayData characterData2 in data.Characters.Values)
						{
							bool flag2 = characterData2.OrgInfo.OrgTemplateId == 16 || characterData2.RelationFromTaiwu != 0 || characterData2.RelationToTaiwu > 0;
							if (flag2)
							{
								return true;
							}
						}
					}
				}
				else if (selectedSubType == 4)
				{
					if (num2 == 4)
					{
						foreach (CharacterDisplayData characterData3 in data.Characters.Values)
						{
							bool flag3 = characterData3.OrgInfo.OrgTemplateId == 16;
							if (flag3)
							{
								return true;
							}
						}
					}
				}
				else
				{
					RelationDisplayTypeItem config = RelationDisplayType.Instance[selectedSubType];
					ushort relationType = 0;
					foreach (sbyte relationTypeId in config.RelationTypeIds)
					{
						relationType |= RelationType.GetRelationType(relationTypeId);
					}
					foreach (CharacterDisplayData characterData4 in data.Characters.Values)
					{
						bool flag4 = RelationType.HasRelation(characterData4.RelationFromTaiwu, relationType) || RelationType.HasRelation(characterData4.RelationToTaiwu, relationType);
						if (flag4)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
