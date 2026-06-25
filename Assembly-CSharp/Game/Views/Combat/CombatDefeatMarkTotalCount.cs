using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B20 RID: 2848
	public class CombatDefeatMarkTotalCount : MonoBehaviour
	{
		// Token: 0x06008BB8 RID: 35768 RVA: 0x00408480 File Offset: 0x00406680
		public void Set(DefeatMarkCollection marks, short oldDisorderOfQi, Injuries oldInjuries, PoisonInts oldPoison)
		{
			Dictionary<EMarkGroupType, int> groupToCountDict = this.CountMarks(marks, oldDisorderOfQi, oldInjuries, oldPoison);
			this.RefreshDefeatMarkTip(groupToCountDict);
		}

		// Token: 0x06008BB9 RID: 35769 RVA: 0x004084A2 File Offset: 0x004066A2
		public void Set(Dictionary<EMarkGroupType, int> groupToCountDict)
		{
			this.RefreshDefeatMarkTip(groupToCountDict);
		}

		// Token: 0x06008BBA RID: 35770 RVA: 0x004084B0 File Offset: 0x004066B0
		public void Set(Dictionary<short, int> groupToCountDict)
		{
			Dictionary<EMarkGroupType, int> dict = new Dictionary<EMarkGroupType, int>();
			foreach (KeyValuePair<short, int> pair in groupToCountDict)
			{
				dict.Add((EMarkGroupType)pair.Key, pair.Value);
			}
			this.RefreshDefeatMarkTip(dict);
		}

		// Token: 0x06008BBB RID: 35771 RVA: 0x00408520 File Offset: 0x00406720
		private void RefreshDefeatMarkTip(Dictionary<EMarkGroupType, int> groupToCountDict)
		{
			int totalCount = groupToCountDict.Values.Sum();
			this.defeatMarkCount.text = Mathf.Min(totalCount, this.maxDisplayCount).ToString();
			ref GeneralLineData ptr = ref this._defeatMarkTipContext.DefeatMarkTitleLine;
			if (ptr == null)
			{
				ptr = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						null
					}
				};
			}
			ref List<GeneralLineData> ptr2 = ref this._defeatMarkTipContext.DefeatMarkLines;
			if (ptr2 == null)
			{
				ptr2 = new List<GeneralLineData>();
			}
			int index = 0;
			int lineCount = 0;
			TooltipInvoker tooltipInvoker = this.defeatMarkTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			ArgumentBox tipParam = this.defeatMarkTip.RuntimeParam;
			this.defeatMarkTip.Type = TipType.GeneralLines;
			tipParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Combat_MarkTip_Title));
			this._defeatMarkTipContext.DefeatMarkTitleLine.Args[0] = LocalStringManager.GetFormat(LanguageKey.LK_Combat_MarkTip_MarkTitle, totalCount);
			CombatDefeatMarkTotalCount.AddNode(tipParam, this._defeatMarkTipContext.DefeatMarkTitleLine, ref lineCount);
			foreach (EMarkGroupType group in CombatDefeatMarkTotalCount.MarkGroupOrder)
			{
				int count;
				bool flag = !groupToCountDict.TryGetValue(group, out count);
				if (!flag)
				{
					CombatDefeatMarkTotalCount.GroupConfig markConfig = CombatDefeatMarkTotalCount.MarkConfigs[group];
					GeneralLineData line = this.GetOneLine(markConfig.IconName, markConfig.GroupLanguageKey, count, ref index);
					CombatDefeatMarkTotalCount.AddNode(tipParam, line, ref lineCount);
				}
			}
			tipParam.Set("LineCount", lineCount);
		}

		// Token: 0x06008BBC RID: 35772 RVA: 0x004086B4 File Offset: 0x004068B4
		private Dictionary<EMarkGroupType, int> CountMarks(DefeatMarkCollection marks, short oldDisorderOfQi, Injuries oldInjuries, PoisonInts oldPoison)
		{
			Dictionary<EMarkGroupType, int> dict = new Dictionary<EMarkGroupType, int>();
			bool flag = marks == null;
			Dictionary<EMarkGroupType, int> result;
			if (flag)
			{
				result = dict;
			}
			else
			{
				foreach (DefeatMarkKey key in marks.GetAllKeys(oldDisorderOfQi, oldPoison, oldInjuries))
				{
					EMarkGroupType group = key.Type.GetGroup();
					bool flag2 = !dict.TryAdd(group, 1);
					if (flag2)
					{
						Dictionary<EMarkGroupType, int> dictionary = dict;
						EMarkGroupType key2 = group;
						dictionary[key2]++;
					}
				}
				result = dict;
			}
			return result;
		}

		// Token: 0x06008BBD RID: 35773 RVA: 0x00408758 File Offset: 0x00406958
		private GeneralLineData GetOneLine(string iconName, LanguageKey markNameKey, int markCount, ref int index)
		{
			bool flag = index >= this._defeatMarkTipContext.DefeatMarkLines.Count;
			if (flag)
			{
				this._defeatMarkTipContext.DefeatMarkLines.Add(new GeneralLineData
				{
					Type = 9,
					Args = new List<string>
					{
						null,
						null,
						null
					}
				});
			}
			GeneralLineData line = this._defeatMarkTipContext.DefeatMarkLines[index];
			line.Args[0] = "-";
			line.Args[1] = iconName;
			line.Args[2] = LocalStringManager.GetFormat(LanguageKey.LK_Combat_MarkTip_MarkLine, LocalStringManager.Get(markNameKey), markCount);
			index++;
			return line;
		}

		// Token: 0x06008BBE RID: 35774 RVA: 0x00408828 File Offset: 0x00406A28
		private static void AddNode(ArgumentBox tipParam, GeneralLineData lineData, ref int lineCount)
		{
			string format = "LineData{0}";
			int num = lineCount + 1;
			lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x06008BC0 RID: 35776 RVA: 0x00408868 File Offset: 0x00406A68
		// Note: this type is marked as 'beforefieldinit'.
		static CombatDefeatMarkTotalCount()
		{
			EMarkGroupType[] array = new EMarkGroupType[10];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.4E40839D3CF21C8757E4C32F9D341740437F3D4CA0B4FA56B9BA6EE062DC72FC).FieldHandle);
			CombatDefeatMarkTotalCount.MarkGroupOrder = array;
			CombatDefeatMarkTotalCount.MarkConfigs = new Dictionary<EMarkGroupType, CombatDefeatMarkTotalCount.GroupConfig>
			{
				{
					EMarkGroupType.Injury,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_injury", LanguageKey.LK_Combat_MarkType_Injury)
				},
				{
					EMarkGroupType.Impair,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_obstacle", LanguageKey.LK_Combat_MarkType_Impair)
				},
				{
					EMarkGroupType.Fatal,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_zhongchuang_0", LanguageKey.LK_Combat_MarkType_Fatal)
				},
				{
					EMarkGroupType.Die,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_die", LanguageKey.LK_Combat_MarkType_Die)
				},
				{
					EMarkGroupType.Poison,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_poison", LanguageKey.LK_Combat_MarkType_Poison)
				},
				{
					EMarkGroupType.Wug,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_guchong", LanguageKey.LK_Combat_MarkType_Wug)
				},
				{
					EMarkGroupType.State,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_state_0", LanguageKey.LK_Combat_MarkType_State)
				},
				{
					EMarkGroupType.NeiliAllocation,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_zhenqi_4", LanguageKey.LK_Combat_MarkType_NeiliAllocation)
				},
				{
					EMarkGroupType.Health,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_jiankang", LanguageKey.LK_Combat_MarkType_Health)
				},
				{
					EMarkGroupType.QiDisorder,
					new CombatDefeatMarkTotalCount.GroupConfig("mousetip_neixiwenluan", LanguageKey.LK_Combat_MarkType_QiDisorder)
				}
			};
		}

		// Token: 0x04006AF9 RID: 27385
		private static readonly EMarkGroupType[] MarkGroupOrder;

		// Token: 0x04006AFA RID: 27386
		private static readonly Dictionary<EMarkGroupType, CombatDefeatMarkTotalCount.GroupConfig> MarkConfigs;

		// Token: 0x04006AFB RID: 27387
		private CombatDefeatMarkTotalCount.DefeatMarkTipContext _defeatMarkTipContext;

		// Token: 0x04006AFC RID: 27388
		[SerializeField]
		private TextMeshProUGUI defeatMarkCount;

		// Token: 0x04006AFD RID: 27389
		[SerializeField]
		private TooltipInvoker defeatMarkTip;

		// Token: 0x04006AFE RID: 27390
		[SerializeField]
		private int maxDisplayCount = 99;

		// Token: 0x020020E1 RID: 8417
		private struct GroupConfig
		{
			// Token: 0x0600F893 RID: 63635 RVA: 0x0062D184 File Offset: 0x0062B384
			public GroupConfig(string iconName, LanguageKey groupLanguageKey)
			{
				this.IconName = iconName;
				this.GroupLanguageKey = groupLanguageKey;
			}

			// Token: 0x0400D2D7 RID: 53975
			public readonly string IconName;

			// Token: 0x0400D2D8 RID: 53976
			public readonly LanguageKey GroupLanguageKey;
		}

		// Token: 0x020020E2 RID: 8418
		private struct DefeatMarkTipContext
		{
			// Token: 0x0400D2D9 RID: 53977
			public GeneralLineData DefeatMarkTitleLine;

			// Token: 0x0400D2DA RID: 53978
			public List<GeneralLineData> DefeatMarkLines;
		}
	}
}
