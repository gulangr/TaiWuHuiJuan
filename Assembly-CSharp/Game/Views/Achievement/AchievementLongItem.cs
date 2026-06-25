using System;
using Config;
using Game.Constants;
using TMPro;
using UnityEngine;

namespace Game.Views.Achievement
{
	// Token: 0x02000C7E RID: 3198
	public class AchievementLongItem : MonoBehaviour
	{
		// Token: 0x0600A368 RID: 41832 RVA: 0x004C7968 File Offset: 0x004C5B68
		public void Set(short templateId, long timeStamp, bool isNew, int progressCurrent = 0, int progressTarget = 0, bool showProgress = false)
		{
			AchievementInfoItem config = AchievementInfo.Instance[templateId];
			string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			this.icon.SetSprite((timeStamp < 0L) ? (config.Icon + "_0") : config.Icon, false, null);
			this.title.text = config.Name;
			this.desc.text = config.Desc;
			this.icon.GetComponent<DisableStyleRoot>().SetStyleEffect(timeStamp < 0L, false);
			bool flag = timeStamp < 0L;
			if (flag)
			{
				this.achieved.SetActive(false);
				this.status.SetSprite(string.Format(PathConstants.IconAchievementStatus[0], language), false, null);
			}
			else
			{
				DateTime time = DateTimeOffset.FromUnixTimeSeconds(timeStamp).LocalDateTime;
				this.achievedDate.text = LanguageKey.LK_Achievement_Unlocked_Date_Content.TrFormat(time.Year, time.Month, time.Day);
				this.achieved.SetActive(true);
				this.status.SetSprite(isNew ? string.Format(PathConstants.IconAchievementStatus[2], language) : string.Format(PathConstants.IconAchievementStatus[1], language), false, null);
			}
			bool flag2 = showProgress && progressTarget > 0;
			if (flag2)
			{
				this.progressGo.SetActive(true);
				this.progressLabel.text = string.Format("{0}/{1}", progressCurrent, progressTarget);
				this.progressBar.fillAmount = Mathf.Clamp01((float)progressCurrent / (float)progressTarget);
			}
			else
			{
				this.progressGo.SetActive(false);
				this.progressBar.fillAmount = 0f;
			}
		}

		// Token: 0x04007F20 RID: 32544
		[SerializeField]
		private GameObject achieved;

		// Token: 0x04007F21 RID: 32545
		[SerializeField]
		private TextMeshProUGUI achievedDate;

		// Token: 0x04007F22 RID: 32546
		[SerializeField]
		private CImage icon;

		// Token: 0x04007F23 RID: 32547
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04007F24 RID: 32548
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04007F25 RID: 32549
		[SerializeField]
		private CImage status;

		// Token: 0x04007F26 RID: 32550
		[SerializeField]
		private GameObject progressGo;

		// Token: 0x04007F27 RID: 32551
		[SerializeField]
		private TextMeshProUGUI progressLabel;

		// Token: 0x04007F28 RID: 32552
		[SerializeField]
		private CImage progressBar;
	}
}
