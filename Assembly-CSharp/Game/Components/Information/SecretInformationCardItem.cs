using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter.Secret;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EF9 RID: 3833
	public class SecretInformationCardItem : MonoBehaviour
	{
		// Token: 0x0600B081 RID: 45185 RVA: 0x005071D4 File Offset: 0x005053D4
		public void Set(int index, Action<int, bool> onClick, SecretSortAndFilterData data)
		{
			this.Set(data);
			base.GetComponent<CToggle>().onValueChanged.ResetListener(delegate(bool val)
			{
				onClick(index, val);
			});
		}

		// Token: 0x0600B082 RID: 45186 RVA: 0x0050721C File Offset: 0x0050541C
		public void Set(SecretSortAndFilterData data)
		{
			SecretInformationItem config = SecretInformation.Instance[data.Data.SecretInformationTemplateId];
			int maxCount = (data.Data.AuthorityCostWhenDisseminating == 0) ? GlobalConfig.Instance.SecretInformationInBroadcastMaxUseCount : GlobalConfig.Instance.SecretInformationInPrivateMaxUseCount;
			int year = CommonUtils.GetYearByDate(data.Data.OccurenceDate);
			int month = CommonUtils.GetMonthByDate(data.Data.OccurenceDate);
			this.bg.sprite = this.GetBgSprite(data.LevelScore);
			this.titleLabel.text = config.Name;
			bool flag = data.Relation >= 0;
			if (flag)
			{
				this.relationLabel.text = ((data.Relation == short.MaxValue) ? LanguageKey.LK_Taiwu.Tr() : RelationDisplayType.Instance[data.Relation].Name);
				this.relationBack.SetActive(true);
			}
			else
			{
				this.relationBack.SetActive(false);
			}
			bool flag2 = data.Data.DisseminationRate >= 0;
			if (flag2)
			{
				this.dissemination.SetSprite(string.Format("ui9_icon_dissemination_{0}", data.DisseminationLevel), false, null);
				this.dissemination.gameObject.SetActive(true);
			}
			else
			{
				this.dissemination.gameObject.SetActive(false);
			}
			this.charCountLabel.text = data.Data.HolderCount.ToString();
			this.durationLabel.text = ((data.GetConfig.Duration >= 0) ? data.LifeTime.ToString() : "∞");
			this.costLabel.text = ((data.Data.AuthorityCostWhenDisseminating == 0) ? data.Data.AuthorityCostWhenDisseminatingForBroadcast : data.Data.AuthorityCostWhenDisseminating).ToString();
			this.useCountLabel.text = string.Format("{0}/{1}", data.CanUseCount, maxCount);
			this.dateLabelCN.text = LanguageKey.LK_Game_Time.TrFormat(new object[]
			{
				year,
				month + 1,
				"-",
				Month.Instance[month].Name
			});
			this.dateLabelEN.text = LanguageKey.LK_Game_Time.TrFormat(new object[]
			{
				year,
				month + 1,
				"\n-",
				Month.Instance[month].Name
			});
			this.dateLabelCN.gameObject.SetActive(LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN);
			this.dateLabelEN.gameObject.SetActive(LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN);
			this.tips.RuntimeParam = new ArgumentBox().SetObject("SecretSortAndFilterData", data);
			this.tips.Refresh(true, -1);
		}

		// Token: 0x0600B083 RID: 45187 RVA: 0x0050751F File Offset: 0x0050571F
		private Sprite GetBgSprite(int score)
		{
			return (score < GlobalConfig.Instance.SecretLevelRange[0]) ? this.levels[0] : ((score < GlobalConfig.Instance.SecretLevelRange[1]) ? this.levels[1] : this.levels[2]);
		}

		// Token: 0x04008878 RID: 34936
		[SerializeField]
		protected CImage bg;

		// Token: 0x04008879 RID: 34937
		[SerializeField]
		protected TextMeshProUGUI titleLabel;

		// Token: 0x0400887A RID: 34938
		[SerializeField]
		protected GameObject relationBack;

		// Token: 0x0400887B RID: 34939
		[SerializeField]
		protected TextMeshProUGUI relationLabel;

		// Token: 0x0400887C RID: 34940
		[SerializeField]
		protected CImage dissemination;

		// Token: 0x0400887D RID: 34941
		[SerializeField]
		protected TextMeshProUGUI charCountLabel;

		// Token: 0x0400887E RID: 34942
		[SerializeField]
		protected TextMeshProUGUI durationLabel;

		// Token: 0x0400887F RID: 34943
		[SerializeField]
		protected TextMeshProUGUI costLabel;

		// Token: 0x04008880 RID: 34944
		[SerializeField]
		protected TextMeshProUGUI useCountLabel;

		// Token: 0x04008881 RID: 34945
		[SerializeField]
		protected TextMeshProUGUI dateLabelCN;

		// Token: 0x04008882 RID: 34946
		[SerializeField]
		protected TextMeshProUGUI dateLabelEN;

		// Token: 0x04008883 RID: 34947
		[SerializeField]
		protected TooltipInvoker tips;

		// Token: 0x04008884 RID: 34948
		[Header("秘闻等级")]
		[SerializeField]
		protected Sprite[] levels;
	}
}
