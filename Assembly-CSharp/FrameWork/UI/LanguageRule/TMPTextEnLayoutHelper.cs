using System;
using TMPro;
using UnityEngine;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FF4 RID: 4084
	public static class TMPTextEnLayoutHelper
	{
		// Token: 0x170014FD RID: 5373
		// (get) Token: 0x0600BA4A RID: 47690 RVA: 0x0054D578 File Offset: 0x0054B778
		public static bool IsChinese
		{
			get
			{
				return LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			}
		}

		// Token: 0x0600BA4B RID: 47691 RVA: 0x0054D584 File Offset: 0x0054B784
		public static void ApplyIconAndTextListLabel(TextMeshProUGUI label)
		{
			bool flag = label == null;
			if (!flag)
			{
				bool flag2 = !TMPTextEnLayoutHelper.IsChinese;
				if (flag2)
				{
					label.enableWordWrapping = true;
					label.overflowMode = TextOverflowModes.Ellipsis;
				}
				else
				{
					label.enableWordWrapping = false;
					label.overflowMode = TextOverflowModes.Overflow;
				}
				label.enableAutoSizing = false;
			}
		}

		// Token: 0x0600BA4C RID: 47692 RVA: 0x0054D5DC File Offset: 0x0054B7DC
		public static void ApplyLegendaryBookSkillTypeName(TextMeshProUGUI text, float baseFontSize)
		{
			bool flag = text == null;
			if (!flag)
			{
				text.enableWordWrapping = false;
				bool flag2 = !TMPTextEnLayoutHelper.IsChinese;
				if (flag2)
				{
					text.characterSpacing = -3f;
					text.enableAutoSizing = true;
					text.fontSizeMax = baseFontSize;
					text.fontSizeMin = Mathf.Min(14f, baseFontSize);
					text.overflowMode = TextOverflowModes.Overflow;
				}
				else
				{
					text.characterSpacing = 0f;
					text.enableAutoSizing = false;
					text.fontSize = baseFontSize;
					text.overflowMode = TextOverflowModes.Overflow;
				}
			}
		}

		// Token: 0x0600BA4D RID: 47693 RVA: 0x0054D66C File Offset: 0x0054B86C
		public static void ApplyLegendaryBookBookName(TextMeshProUGUI text, float baseFontSize)
		{
			bool flag = text == null;
			if (!flag)
			{
				bool flag2 = !TMPTextEnLayoutHelper.IsChinese;
				if (flag2)
				{
					text.characterSpacing = -3f;
					text.enableWordWrapping = true;
					text.enableAutoSizing = true;
					text.fontSizeMax = baseFontSize;
					text.fontSizeMin = Mathf.Min(14f, baseFontSize);
					text.overflowMode = TextOverflowModes.Overflow;
				}
				else
				{
					text.characterSpacing = 0f;
					text.enableWordWrapping = false;
					text.enableAutoSizing = false;
					text.fontSize = baseFontSize;
					text.overflowMode = TextOverflowModes.Overflow;
				}
			}
		}

		// Token: 0x0600BA4E RID: 47694 RVA: 0x0054D704 File Offset: 0x0054B904
		public static void ApplySubPageToggleLabel(TextMeshProUGUI label, float maxFontSize)
		{
			bool flag = label == null;
			if (!flag)
			{
				label.enableWordWrapping = false;
				bool flag2 = !TMPTextEnLayoutHelper.IsChinese;
				if (flag2)
				{
					label.enableAutoSizing = true;
					label.fontSizeMin = 18f;
					label.fontSizeMax = maxFontSize;
					label.overflowMode = TextOverflowModes.Ellipsis;
				}
				else
				{
					label.enableAutoSizing = false;
					label.fontSize = maxFontSize;
					label.overflowMode = TextOverflowModes.Overflow;
				}
			}
		}

		// Token: 0x04009007 RID: 36871
		public const float ToggleLabelMinFontSize = 18f;

		// Token: 0x04009008 RID: 36872
		public const float LegendaryBookCellMinFontSize = 14f;

		// Token: 0x04009009 RID: 36873
		public const float NonChineseCharacterSpacing = -3f;
	}
}
