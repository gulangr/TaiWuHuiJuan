using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C1 RID: 2497
	public class GearMateCombatSkillBookChapter : MonoBehaviour
	{
		// Token: 0x06007934 RID: 31028 RVA: 0x00385688 File Offset: 0x00383888
		public void Refresh(int index, int last, int cur)
		{
			bool isActive = cur < last;
			int internalIndex = index % 5;
			if (!true)
			{
			}
			GearMateCombatSkillBookChapter.ChapterType chapterType;
			if (index <= 9)
			{
				if (index > 4)
				{
					chapterType = GearMateCombatSkillBookChapter.ChapterType.Direct;
				}
				else
				{
					chapterType = GearMateCombatSkillBookChapter.ChapterType.Outline;
				}
			}
			else
			{
				if (index > 14)
				{
					throw new ArgumentOutOfRangeException("index", index, null);
				}
				chapterType = GearMateCombatSkillBookChapter.ChapterType.Reverse;
			}
			if (!true)
			{
			}
			GearMateCombatSkillBookChapter.ChapterType type = chapterType;
			this.imageBack.sprite = this.spriteBackArray[type.ToInt()];
			this.imageBack.gameObject.SetActive(isActive);
			this.imageState.sprite = (isActive ? this.spriteStateActiveArray[type.ToInt()] : this.spriteStateNormalArray[type.ToInt()]);
			if (!true)
			{
			}
			string text;
			switch (type)
			{
			case GearMateCombatSkillBookChapter.ChapterType.Outline:
				text = GearMateCombatSkillBookChapter.OutlineNameList[internalIndex];
				break;
			case GearMateCombatSkillBookChapter.ChapterType.Direct:
				text = GearMateCombatSkillBookChapter.DirectNameList[internalIndex];
				break;
			case GearMateCombatSkillBookChapter.ChapterType.Reverse:
				text = GearMateCombatSkillBookChapter.ReverseNameList[internalIndex];
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			string nameKey = text;
			string text2;
			if (isActive)
			{
				if (!true)
				{
				}
				switch (type)
				{
				case GearMateCombatSkillBookChapter.ChapterType.Outline:
					text = "specialyellow";
					break;
				case GearMateCombatSkillBookChapter.ChapterType.Direct:
					text = "brightblue";
					break;
				case GearMateCombatSkillBookChapter.ChapterType.Reverse:
					text = "brightred";
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				text2 = text;
			}
			else
			{
				text2 = "grey";
			}
			string nameColor = text2;
			this.textName.text = nameKey.SetColor(nameColor);
			string progressColor = isActive ? "specialyellow" : ((cur >= 100) ? "brightblue" : "brightred");
			this.curProgress.text = string.Format("{0}%", cur).SetColor(progressColor);
			this.lastProgress.text = string.Format("{0}%", last);
		}

		// Token: 0x04005BD9 RID: 23513
		[SerializeField]
		private CImage imageBack;

		// Token: 0x04005BDA RID: 23514
		[SerializeField]
		private CImage imageState;

		// Token: 0x04005BDB RID: 23515
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005BDC RID: 23516
		[SerializeField]
		private TextMeshProUGUI lastProgress;

		// Token: 0x04005BDD RID: 23517
		[SerializeField]
		private TextMeshProUGUI curProgress;

		// Token: 0x04005BDE RID: 23518
		[SerializeField]
		private Sprite[] spriteBackArray;

		// Token: 0x04005BDF RID: 23519
		[SerializeField]
		private Sprite[] spriteStateNormalArray;

		// Token: 0x04005BE0 RID: 23520
		[SerializeField]
		private Sprite[] spriteStateActiveArray;

		// Token: 0x04005BE1 RID: 23521
		public static List<string> OutlineNameList = new List<string>
		{
			"承",
			"合",
			"解",
			"异",
			"独"
		};

		// Token: 0x04005BE2 RID: 23522
		public static List<string> DirectNameList = new List<string>
		{
			"修",
			"思",
			"源",
			"参",
			"藏"
		};

		// Token: 0x04005BE3 RID: 23523
		public static List<string> ReverseNameList = new List<string>
		{
			"用",
			"奇",
			"巧",
			"化",
			"绝"
		};

		// Token: 0x02001F0F RID: 7951
		private enum ChapterType
		{
			// Token: 0x0400CC34 RID: 52276
			Outline,
			// Token: 0x0400CC35 RID: 52277
			Direct,
			// Token: 0x0400CC36 RID: 52278
			Reverse
		}
	}
}
