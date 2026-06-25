using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C2 RID: 2498
	public class GearMateCombatSkillBookPreview : MonoBehaviour
	{
		// Token: 0x17000D77 RID: 3447
		// (get) Token: 0x06007937 RID: 31031 RVA: 0x0038593F File Offset: 0x00383B3F
		public ItemKey ItemKey
		{
			get
			{
				return this._itemData.RealKey;
			}
		}

		// Token: 0x06007938 RID: 31032 RVA: 0x0038594C File Offset: 0x00383B4C
		public void Refresh(ItemDisplayData itemData, sbyte[] lastProgressRead, sbyte[] curProgressRead, Action<ItemDisplayData> onClickCancel)
		{
			this._itemData = itemData;
			this.itemBack.Set(itemData, false);
			this.textName.text = ItemTemplateHelper.GetName(this.ItemKey.ItemType, this.ItemKey.TemplateId);
			this.buttonCancel.ClearAndAddListener(delegate
			{
				Action<ItemDisplayData> onClickCancel2 = onClickCancel;
				if (onClickCancel2 != null)
				{
					onClickCancel2(this._itemData);
				}
			});
			for (int i = 0; i < this.chapterArray.Length; i++)
			{
				sbyte curProgress = curProgressRead[i];
				sbyte lastProgress = lastProgressRead[i];
				this.chapterArray[i].Refresh(i, (int)lastProgress, (int)curProgress);
			}
		}

		// Token: 0x06007939 RID: 31033 RVA: 0x003859F7 File Offset: 0x00383BF7
		public GearMateCombatSkillBookChapter GetChapter(int index)
		{
			return this.chapterArray[index];
		}

		// Token: 0x0600793A RID: 31034 RVA: 0x00385A04 File Offset: 0x00383C04
		public void RefreshTip(List<int> defaultReadIds, List<int> bookIds)
		{
			GearMateCombatSkillBookPreview.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.bookIds = bookIds;
			CS$<>8__locals1.defaultReadIds = defaultReadIds;
			SkillBookItem configData = SkillBook.Instance[this.ItemKey.TemplateId];
			CS$<>8__locals1.disPlayer = this.tip;
			CS$<>8__locals1.disPlayer.enabled = true;
			TooltipInvoker disPlayer = CS$<>8__locals1.disPlayer;
			if (disPlayer.RuntimeParam == null)
			{
				disPlayer.RuntimeParam = new ArgumentBox();
			}
			CS$<>8__locals1.disPlayer.Type = TipType.GeneralLines;
			string title = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_0);
			CS$<>8__locals1.disPlayer.RuntimeParam.Set("Title", title);
			CS$<>8__locals1.lineCount = 0;
			string line0 = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateCombatSkillBook_1, configData.Name.SetColor(Colors.Instance.GradeColors[(int)configData.Grade]));
			ArgumentBox runtimeParam = CS$<>8__locals1.disPlayer.RuntimeParam;
			string format = "LineData{0}";
			int num = CS$<>8__locals1.lineCount + 1;
			CS$<>8__locals1.lineCount = num;
			runtimeParam.SetObject(string.Format(format, num), new GeneralLineData(5, new List<string>
			{
				line0
			}, null));
			string line = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_2);
			ArgumentBox runtimeParam2 = CS$<>8__locals1.disPlayer.RuntimeParam;
			string format2 = "LineData{0}";
			num = CS$<>8__locals1.lineCount + 1;
			CS$<>8__locals1.lineCount = num;
			runtimeParam2.SetObject(string.Format(format2, num), new GeneralLineData(1, new List<string>
			{
				line
			}, null));
			CS$<>8__locals1.stringBuilder = EasyPool.Get<StringBuilder>();
			CS$<>8__locals1.stringBuilder.Clear();
			string line2 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_4);
			GearMateCombatSkillBookPreview.<RefreshTip>g__Build|10_0(line2, "LK_CombatSkill_First_Page_Type_{0}", true, ref CS$<>8__locals1);
			string line3 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_5);
			GearMateCombatSkillBookPreview.<RefreshTip>g__Build|10_0(line3, "LK_CombatSkill_Direct_Page_{0}", true, ref CS$<>8__locals1);
			string line4 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_6);
			GearMateCombatSkillBookPreview.<RefreshTip>g__Build|10_0(line4, "LK_CombatSkill_Reverse_Page_{0}", true, ref CS$<>8__locals1);
			string line5 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_3);
			ArgumentBox runtimeParam3 = CS$<>8__locals1.disPlayer.RuntimeParam;
			string format3 = "LineData{0}";
			num = CS$<>8__locals1.lineCount + 1;
			CS$<>8__locals1.lineCount = num;
			runtimeParam3.SetObject(string.Format(format3, num), new GeneralLineData(1, new List<string>
			{
				line5
			}, null));
			GearMateCombatSkillBookPreview.<RefreshTip>g__Build|10_0(line2, "LK_CombatSkill_First_Page_Type_{0}", false, ref CS$<>8__locals1);
			GearMateCombatSkillBookPreview.<RefreshTip>g__Build|10_0(line3, "LK_CombatSkill_Direct_Page_{0}", false, ref CS$<>8__locals1);
			GearMateCombatSkillBookPreview.<RefreshTip>g__Build|10_0(line4, "LK_CombatSkill_Reverse_Page_{0}", false, ref CS$<>8__locals1);
			CS$<>8__locals1.disPlayer.RuntimeParam.Set("LineCount", CS$<>8__locals1.lineCount);
			CS$<>8__locals1.disPlayer.Refresh(false, -1);
			CS$<>8__locals1.stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(CS$<>8__locals1.stringBuilder);
		}

		// Token: 0x0600793C RID: 31036 RVA: 0x00385CB4 File Offset: 0x00383EB4
		[CompilerGenerated]
		internal static void <RefreshTip>g__Build|10_0(string label, string startKeyPattern, bool thisBook = true, ref GearMateCombatSkillBookPreview.<>c__DisplayClass10_0 A_3)
		{
			A_3.stringBuilder.Clear();
			A_3.stringBuilder.Append(label);
			A_3.stringBuilder.Append(":");
			string disableColor = "545454ff";
			string enableColor = disableColor;
			int id = -1;
			for (ushort i = 0; i < 5; i += 1)
			{
				if (!(startKeyPattern == "LK_CombatSkill_First_Page_Type_{0}"))
				{
					if (!(startKeyPattern == "LK_CombatSkill_Direct_Page_{0}"))
					{
						if (startKeyPattern == "LK_CombatSkill_Reverse_Page_{0}")
						{
							enableColor = "brightred";
							id = (int)(i + 10);
						}
					}
					else
					{
						enableColor = "lightblue";
						id = (int)(i + 5);
					}
				}
				else
				{
					enableColor = "darkpurple";
					id = (int)i;
				}
				string finalColor = disableColor;
				if (thisBook)
				{
					bool flag = A_3.bookIds.Contains(id);
					if (flag)
					{
						finalColor = enableColor;
					}
				}
				else
				{
					bool flag2 = A_3.defaultReadIds.Contains(id);
					if (flag2)
					{
						finalColor = enableColor;
					}
				}
				string s = LocalStringManager.Get(string.Format(startKeyPattern, i)).SetColor(finalColor);
				A_3.stringBuilder.Append(s);
			}
			ArgumentBox runtimeParam = A_3.disPlayer.RuntimeParam;
			string format = "LineData{0}";
			int num = A_3.lineCount + 1;
			A_3.lineCount = num;
			runtimeParam.SetObject(string.Format(format, num), new GeneralLineData(5, new List<string>
			{
				A_3.stringBuilder.ToString()
			}, null)
			{
				ExtraArgs = new List<object>
				{
					20
				}
			});
		}

		// Token: 0x04005BE4 RID: 23524
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x04005BE5 RID: 23525
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005BE6 RID: 23526
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04005BE7 RID: 23527
		[SerializeField]
		private GearMateCombatSkillBookChapter[] chapterArray;

		// Token: 0x04005BE8 RID: 23528
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04005BE9 RID: 23529
		private ItemDisplayData _itemData;
	}
}
