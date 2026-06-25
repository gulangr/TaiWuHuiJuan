using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008BB RID: 2235
	public class MonthNotifyBookPageItem : MonoBehaviour
	{
		// Token: 0x06006AA1 RID: 27297 RVA: 0x00313B20 File Offset: 0x00311D20
		public void Set(int index, sbyte direction, sbyte behavior, int prev, int curr)
		{
			if (direction != 0)
			{
				if (direction != 1)
				{
					this.pageName.text = LanguageKey.LK_Skill_Break_Tip_Outline_Title.TrFormat(LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", behavior)).SetColor(Colors.Instance.BehaviorTypeColors[(int)behavior]));
				}
				else
				{
					this.pageName.text = LanguageKey.LK_Skill_Break_Tip_OtherPage_Title.TrFormat(LocalStringManager.Get(string.Format("LK_CombatSkill_Reverse_Page_{0}", behavior)).SetColor("ffc0c0"));
				}
			}
			else
			{
				this.pageName.text = LanguageKey.LK_Skill_Break_Tip_OtherPage_Title.TrFormat(LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", behavior)).SetColor("81ddff"));
			}
			this.pageSubName.text = LocalStringManager.Get(string.Format("LK_Book_Page_Index_{0}", index));
			this.progress.fillAmount = (float)curr / 100f;
			this.progressLabel.text = string.Format("{0}%{1}", curr, this.GetDeltaText(prev, curr));
		}

		// Token: 0x06006AA2 RID: 27298 RVA: 0x00313C4C File Offset: 0x00311E4C
		public void Set(int index, int prev, int curr)
		{
			this.pageName.text = LocalStringManager.Get(string.Format("LK_Book_Page_Index_{0}", index));
			this.pageSubName.text = "";
			this.progress.fillAmount = (float)curr / 100f;
			this.progressLabel.text = string.Format("{0}%{1}", curr, this.GetDeltaText(prev, curr));
		}

		// Token: 0x06006AA3 RID: 27299 RVA: 0x00313CC4 File Offset: 0x00311EC4
		private string GetDeltaText(int prev, int curr)
		{
			int delta = curr - prev;
			int percent = delta * 100 / 100;
			if (!true)
			{
			}
			string result;
			if (delta <= 0)
			{
				if (delta >= 0)
				{
					result = "";
				}
				else
				{
					result = string.Format("({0}%)", percent).SetColor("brightred");
				}
			}
			else
			{
				result = string.Format("(+{0}%)", percent).SetColor("brightblue");
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04004D0B RID: 19723
		[SerializeField]
		protected TextMeshProUGUI pageName;

		// Token: 0x04004D0C RID: 19724
		[SerializeField]
		protected TextMeshProUGUI pageSubName;

		// Token: 0x04004D0D RID: 19725
		[SerializeField]
		protected CImage progress;

		// Token: 0x04004D0E RID: 19726
		[SerializeField]
		protected TextMeshProUGUI progressLabel;
	}
}
