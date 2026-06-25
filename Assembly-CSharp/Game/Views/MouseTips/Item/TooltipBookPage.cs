using System;
using System.Collections.Generic;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x0200089B RID: 2203
	public class TooltipBookPage : MonoBehaviour
	{
		// Token: 0x06006965 RID: 26981 RVA: 0x003071CC File Offset: 0x003053CC
		public void Refresh(bool isCombatSkill, int index, sbyte type, sbyte progress, sbyte state)
		{
			bool flag = this.textName;
			if (flag)
			{
				this.textName.text = TooltipBookPage.GetPageName(isCombatSkill, index);
			}
			bool flag2 = this.textType;
			if (flag2)
			{
				if (isCombatSkill)
				{
					bool isFirst = index == 0;
					bool isDirect = type == 0;
					string typeKey = isFirst ? LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", type)) : (isDirect ? LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", index - 1)) : LocalStringManager.Get(string.Format("LK_CombatSkill_Reverse_Page_{0}", index - 1)));
					string colorName = isFirst ? TooltipBookPage.ColorMap[(int)type] : (isDirect ? "brightblue" : "brightred");
					this.textType.text = typeKey.SetColor(colorName);
				}
				else
				{
					this.textType.text = string.Empty;
				}
			}
			bool flag3 = this.imageProgress;
			if (flag3)
			{
				this.imageProgress.fillAmount = (float)progress / 100f;
			}
			bool flag4 = this.textProgress;
			if (flag4)
			{
				this.textProgress.text = string.Format("{0}%", progress);
			}
			if (!true)
			{
			}
			string text;
			switch (state)
			{
			case 0:
				text = LanguageKey.LK_Book_Page_State_Complete_No_Symbol.Tr().SetColor("brightblue");
				break;
			case 1:
				text = LanguageKey.LK_Book_Page_State_Incomplete_No_Symbol.Tr().SetColor("lightgrey");
				break;
			case 2:
				text = LanguageKey.LK_Book_Page_State_Lost_No_Symbol.Tr().SetColor("brightred");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			string stateName = text;
			bool flag5 = this.textIncompleteState;
			if (flag5)
			{
				this.textIncompleteState.text = stateName;
			}
			bool flag6 = this.imageIncompleteState;
			if (flag6)
			{
				this.imageIncompleteState.SetSprite("ui9_icon_item_book_reading_status_{0}".GetFormat(state), false, null);
			}
		}

		// Token: 0x06006966 RID: 26982 RVA: 0x003073D4 File Offset: 0x003055D4
		public static string GetPageName(bool isCombatSkill, int index)
		{
			string result;
			if (isCombatSkill)
			{
				LanguageKey nameKey = (index == 0) ? LanguageKey.LK_CombatSkill_Book_First_Page : (LanguageKey.LK_Book_Page_Index_0 + index - 1);
				result = nameKey.Tr();
			}
			else
			{
				LanguageKey nameKey2 = LanguageKey.LK_Book_Page_Index_0 + index;
				result = nameKey2.Tr();
			}
			return result;
		}

		// Token: 0x04004BAC RID: 19372
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04004BAD RID: 19373
		[SerializeField]
		private CImage imageProgress;

		// Token: 0x04004BAE RID: 19374
		[SerializeField]
		private TextMeshProUGUI textProgress;

		// Token: 0x04004BAF RID: 19375
		[SerializeField]
		private TextMeshProUGUI textType;

		// Token: 0x04004BB0 RID: 19376
		[SerializeField]
		private CImage imageIncompleteState;

		// Token: 0x04004BB1 RID: 19377
		[SerializeField]
		private TextMeshProUGUI textIncompleteState;

		// Token: 0x04004BB2 RID: 19378
		public static readonly Dictionary<int, string> ColorMap = new Dictionary<int, string>
		{
			{
				0,
				"BehaviorType_Just"
			},
			{
				1,
				"BehaviorType_Kind"
			},
			{
				2,
				"BehaviorType_Even"
			},
			{
				3,
				"BehaviorType_Rebel"
			},
			{
				4,
				"BehaviorType_Egoistic"
			}
		};
	}
}
