using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E64 RID: 3684
	public class CharacterLocationDisplayDataMainTypeFilterLineDetail : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001345 RID: 4933
		// (get) Token: 0x0600AC47 RID: 44103 RVA: 0x004EDEA8 File Offset: 0x004EC0A8
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001346 RID: 4934
		// (get) Token: 0x0600AC48 RID: 44104 RVA: 0x004EDEAB File Offset: 0x004EC0AB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC49 RID: 44105 RVA: 0x004EDEAE File Offset: 0x004EC0AE
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_Mark_Location_Manage_Block);
		}

		// Token: 0x0600AC4A RID: 44106 RVA: 0x004EDEBC File Offset: 0x004EC0BC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_0)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_1)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_2)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_3)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_4)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_5)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_6)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_7))
			};
		}

		// Token: 0x0600AC4B RID: 44107 RVA: 0x004EDF80 File Offset: 0x004EC180
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				bool flag3;
				if (index == 7)
				{
					CharacterLocationDisplayData data2 = data;
					bool? flag;
					if (data2 == null)
					{
						flag = null;
					}
					else
					{
						MapBlockData blockData = data2.BlockData;
						flag = ((blockData != null) ? new bool?(blockData.Destroyed) : null);
					}
					bool? flag2 = flag;
					flag3 = flag2.GetValueOrDefault();
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				bool flag5 = flag4;
				if (!flag5)
				{
					CharacterLocationDisplayData data3 = data;
					EMapBlockType? emapBlockType;
					if (data3 == null)
					{
						emapBlockType = null;
					}
					else
					{
						MapBlockData blockData2 = data3.BlockData;
						if (blockData2 == null)
						{
							emapBlockType = null;
						}
						else
						{
							MapBlockItem config = blockData2.GetConfig();
							emapBlockType = ((config != null) ? new EMapBlockType?(config.Type) : null);
						}
					}
					EMapBlockType? emapBlockType2 = emapBlockType;
					switch (index)
					{
					case 0:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.City)
						{
							goto IL_1AB;
						}
						break;
					}
					case 1:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Sect)
						{
							goto IL_1AB;
						}
						break;
					}
					case 2:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Town)
						{
							goto IL_1AB;
						}
						break;
					}
					case 3:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Station)
						{
							goto IL_1AB;
						}
						break;
					}
					case 4:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Developed)
						{
							goto IL_1AB;
						}
						break;
					}
					case 5:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Normal)
						{
							goto IL_1AB;
						}
						break;
					}
					case 6:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Wild)
						{
							goto IL_1AB;
						}
						break;
					}
					case 7:
					{
						if (emapBlockType2 == null)
						{
							goto IL_1AB;
						}
						EMapBlockType valueOrDefault = emapBlockType2.GetValueOrDefault();
						if (valueOrDefault != EMapBlockType.Bad)
						{
							goto IL_1AB;
						}
						break;
					}
					default:
						goto IL_1AB;
					}
					bool flag6 = true;
					goto IL_1AE;
					IL_1AB:
					flag6 = false;
					IL_1AE:
					flag5 = flag6;
				}
				return flag5;
			});
		}
	}
}
