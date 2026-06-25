using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000841 RID: 2113
	public class InteractionPanel : MonoBehaviour
	{
		// Token: 0x17000C5E RID: 3166
		// (get) Token: 0x060066E1 RID: 26337 RVA: 0x002EEFBE File Offset: 0x002ED1BE
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x060066E2 RID: 26338 RVA: 0x002EEFCC File Offset: 0x002ED1CC
		public void Set(CharacterDisplayDataForMapBlock data)
		{
			Dictionary<short, bool> visibleCharacterInteractionEventOptionDict = data.VisibleCharacterInteractionEventOptionDict;
			bool flag = visibleCharacterInteractionEventOptionDict == null || visibleCharacterInteractionEventOptionDict.Count <= 0;
			if (!flag)
			{
				Dictionary<EInteractionEventOptionInteractionType, List<ValueTuple<string, bool>>> itemDict = new Dictionary<EInteractionEventOptionInteractionType, List<ValueTuple<string, bool>>>();
				foreach (KeyValuePair<short, bool> keyValuePair in data.VisibleCharacterInteractionEventOptionDict)
				{
					short num;
					bool flag2;
					keyValuePair.Deconstruct(out num, out flag2);
					short templateId = num;
					bool available = flag2;
					InteractionEventOptionItem config = InteractionEventOption.Instance[templateId];
					List<ValueTuple<string, bool>> list;
					bool flag3 = !itemDict.TryGetValue(config.InteractionType, out list);
					if (flag3)
					{
						list = new List<ValueTuple<string, bool>>();
						itemDict[config.InteractionType] = list;
					}
					bool flag4 = !list.Exists(([TupleElementNames(new string[]
					{
						"name",
						"available"
					})] ValueTuple<string, bool> i) => i.Item1 == config.Name);
					if (flag4)
					{
						list.Add(new ValueTuple<string, bool>(config.Name, available));
					}
				}
				for (int j = 0; j < this.contents.Length; j++)
				{
					this.contents[j].Set(itemDict.GetValueOrDefault((EInteractionEventOptionInteractionType)j));
				}
			}
		}

		// Token: 0x060066E4 RID: 26340 RVA: 0x002EF130 File Offset: 0x002ED330
		[CompilerGenerated]
		internal static string <Set>g__GetColor|3_0(EInteractionEventOptionInteractionType type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EInteractionEventOptionInteractionType.Talk:
				result = "GradeColor_2";
				break;
			case EInteractionEventOptionInteractionType.Competition:
				result = "GradeColor_6";
				break;
			case EInteractionEventOptionInteractionType.Practice:
				result = "brightblue";
				break;
			case EInteractionEventOptionInteractionType.Intimate:
				result = "normaladventure";
				break;
			case EInteractionEventOptionInteractionType.Enemy:
				result = "darkred";
				break;
			case EInteractionEventOptionInteractionType.Identity:
				result = "GradeColor_8";
				break;
			case EInteractionEventOptionInteractionType.Organization:
				result = "defense";
				break;
			case EInteractionEventOptionInteractionType.Profession:
				result = "enemyround";
				break;
			case EInteractionEventOptionInteractionType.LegendaryBook:
				result = "assist";
				break;
			case EInteractionEventOptionInteractionType.Special:
				result = "orange";
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04004863 RID: 18531
		[SerializeField]
		private InteractionPanelLine[] contents;
	}
}
