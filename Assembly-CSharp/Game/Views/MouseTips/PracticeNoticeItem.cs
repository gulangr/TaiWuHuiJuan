using System;
using System.Linq;
using System.Runtime.CompilerServices;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200087B RID: 2171
	public class PracticeNoticeItem : MonoBehaviour
	{
		// Token: 0x06006862 RID: 26722 RVA: 0x002FBA50 File Offset: 0x002F9C50
		public void Set(WorldStateData worldStateData)
		{
			byte loopingIndex = (byte)worldStateData.IterateLoopingTypes().FirstOrDefault<WorldStateData.ELoopingType>();
			byte readingIndex = (byte)worldStateData.IterateReadingTypes().FirstOrDefault<WorldStateData.EReadingType>();
			this.loopingHolder.gameObject.SetActive(worldStateData.HasLoopingTypes());
			TextMeshProUGUI[] texts = this.loopingHolder.GetComponentsInChildren<TextMeshProUGUI>();
			for (int i = 1; i < texts.Length; i++)
			{
				bool flag = PracticeNoticeItem.LoopingStateKeys[(int)loopingIndex].CheckIndex(i - 1);
				if (flag)
				{
					texts[i].gameObject.SetActive(true);
					texts[i].text = PracticeNoticeItem.LoopingStateKeys[(int)loopingIndex][i - 1].Tr().ColorReplace();
				}
				else
				{
					texts[i].gameObject.SetActive(false);
				}
			}
			this.readingHolder.gameObject.SetActive(worldStateData.HasReadingTypes());
			texts = this.readingHolder.GetComponentsInChildren<TextMeshProUGUI>();
			for (int j = 1; j < texts.Length; j++)
			{
				bool flag2 = PracticeNoticeItem.ReadingStateKeys[(int)readingIndex].CheckIndex(j - 1);
				if (flag2)
				{
					texts[j].gameObject.SetActive(true);
					texts[j].text = PracticeNoticeItem.ReadingStateKeys[(int)readingIndex][j - 1].Tr().ColorReplace();
				}
				else
				{
					texts[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006864 RID: 26724 RVA: 0x002FBBAC File Offset: 0x002F9DAC
		// Note: this type is marked as 'beforefieldinit'.
		static PracticeNoticeItem()
		{
			LanguageKey[][] array = new LanguageKey[5][];
			array[0] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Reading_NoReadingBookAllFinished,
				LanguageKey.LK_WorldState_Reading_NoReadingBookAllFinished_Desc
			};
			int num = 1;
			LanguageKey[] array2 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.62615C91A12457AC9F07B88E52B21A6CFB23A2FC064193D8AB29B585A47CCAE7).FieldHandle);
			array[num] = array2;
			array[2] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Reading_ReadingBookFinishedWithUnfinished,
				LanguageKey.LK_WorldState_Reading_ReadingBookFinishedWithUnfinished_Desc
			};
			array[3] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Reading_ReadingBookCanFillReference,
				LanguageKey.LK_WorldState_Reading_ReadingBookCanFillReference_Desc
			};
			array[4] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Reading_HasReadingEvent,
				LanguageKey.LK_WorldState_Reading_HasReadingEvent_Desc
			};
			PracticeNoticeItem.ReadingStateKeys = array;
			LanguageKey[][] array3 = new LanguageKey[6][];
			array3[0] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Looping_HasLoopingEvent,
				LanguageKey.LK_WorldState_Looping_HasLoopingEvent_Desc
			};
			array3[1] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Looping_NoLoopingNeigongWithUnfinished,
				LanguageKey.LK_WorldState_Looping_NoLoopingNeigongWithUnfinished_Desc
			};
			int num2 = 2;
			LanguageKey[] array4 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array4, fieldof(<PrivateImplementationDetails>.CBC9AE78F23F6D331C538D9E3FBB17BF5D090D3FC040CCF418A1C967A736F3C7).FieldHandle);
			array3[num2] = array4;
			array3[3] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Looping_LoopingNeigongFinishedWithUnfinished,
				LanguageKey.LK_WorldState_Looping_LoopingNeigongFinishedWithUnfinished_Desc
			};
			array3[4] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Looping_LoopingNeigongCanFillAuxiliary,
				LanguageKey.LK_WorldState_Looping_LoopingNeigongCanFillAuxiliary_Desc
			};
			array3[5] = new LanguageKey[]
			{
				LanguageKey.LK_WorldState_Looping_NeiliFiveElementsConflicting,
				LanguageKey.LK_WorldState_Looping_NeiliFiveElementsConflicting_Desc
			};
			PracticeNoticeItem.LoopingStateKeys = array3;
		}

		// Token: 0x04004A16 RID: 18966
		[SerializeField]
		private RectTransform loopingHolder;

		// Token: 0x04004A17 RID: 18967
		[SerializeField]
		private RectTransform readingHolder;

		// Token: 0x04004A18 RID: 18968
		public static readonly LanguageKey[][] ReadingStateKeys;

		// Token: 0x04004A19 RID: 18969
		public static readonly LanguageKey[][] LoopingStateKeys;
	}
}
