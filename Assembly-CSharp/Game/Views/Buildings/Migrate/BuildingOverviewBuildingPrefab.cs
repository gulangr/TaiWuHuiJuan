using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BC6 RID: 3014
	public class BuildingOverviewBuildingPrefab : MonoBehaviour
	{
		// Token: 0x17001046 RID: 4166
		// (get) Token: 0x0600980A RID: 38922 RVA: 0x0046EA51 File Offset: 0x0046CC51
		// (set) Token: 0x0600980B RID: 38923 RVA: 0x0046EA59 File Offset: 0x0046CC59
		public short TemplateId { get; set; }

		// Token: 0x17001047 RID: 4167
		// (get) Token: 0x0600980C RID: 38924 RVA: 0x0046EA62 File Offset: 0x0046CC62
		// (set) Token: 0x0600980D RID: 38925 RVA: 0x0046EA6A File Offset: 0x0046CC6A
		public BuildingOverviewBuildingPrefab.EBuildingStatus Status { get; set; }

		// Token: 0x17001048 RID: 4168
		// (get) Token: 0x0600980E RID: 38926 RVA: 0x0046EA73 File Offset: 0x0046CC73
		// (set) Token: 0x0600980F RID: 38927 RVA: 0x0046EA7B File Offset: 0x0046CC7B
		public float HeightNormalResource { get; set; }

		// Token: 0x17001049 RID: 4169
		// (get) Token: 0x06009810 RID: 38928 RVA: 0x0046EA84 File Offset: 0x0046CC84
		// (set) Token: 0x06009811 RID: 38929 RVA: 0x0046EA8C File Offset: 0x0046CC8C
		public float HeightLegacy { get; set; }

		// Token: 0x06009812 RID: 38930 RVA: 0x0046EA98 File Offset: 0x0046CC98
		public void ResetLockDisplay()
		{
			bool flag = this.lockGo != null;
			if (flag)
			{
				this.lockGo.SetActive(false);
			}
			this.HideBookLockIcons();
			bool flag2 = this.lockTips != null;
			if (flag2)
			{
				this.lockTips.gameObject.SetActive(false);
			}
			bool flag3 = this.mouseTip != null;
			if (flag3)
			{
				this.mouseTip.RuntimeParam = null;
			}
		}

		// Token: 0x06009813 RID: 38931 RVA: 0x0046EB0C File Offset: 0x0046CD0C
		public void ApplyMainProgressLock(string tipsText)
		{
			bool flag = !this.ShowLockGo();
			if (!flag)
			{
				this.HideBookLockIcons();
				this.SetLockTipsVisible(tipsText);
			}
		}

		// Token: 0x06009814 RID: 38932 RVA: 0x0046EB38 File Offset: 0x0046CD38
		public void ApplyLifeSkillBookLock(LifeSkillItem lifeSkillItem, SkillBookItem skillBookConfig, sbyte[] readingProgress)
		{
			bool flag = lifeSkillItem == null || this.lockGo == null;
			if (flag)
			{
				this.ResetLockDisplay();
			}
			else
			{
				bool flag2 = !this.ShowLockGo();
				if (!flag2)
				{
					string grade = lifeSkillItem.Grade.ToString();
					bool flag3 = this.gradeIcon != null;
					if (flag3)
					{
						this.gradeIcon.gameObject.SetActive(true);
						this.gradeIcon.SetSprite("ui9_btn_lifeskill_0_{0}".GetFormat(grade), false, null);
					}
					bool flag4 = this.lockIcon != null;
					if (flag4)
					{
						this.lockIcon.gameObject.SetActive(true);
						this.lockIcon.SetSprite(skillBookConfig.Icon, false, null);
					}
					this.SetLockTipsVisible((this.lockTips != null) ? this.lockTips.text : null);
					this.SetupLifeSkillBookMouseTip(skillBookConfig, readingProgress);
				}
			}
		}

		// Token: 0x06009815 RID: 38933 RVA: 0x0046EC2C File Offset: 0x0046CE2C
		private bool ShowLockGo()
		{
			bool flag = this.lockGo == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.lockGo.SetActive(true);
				CImage lockBackground = this.lockGo.GetComponent<CImage>();
				bool flag2 = lockBackground != null;
				if (flag2)
				{
					lockBackground.enabled = true;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06009816 RID: 38934 RVA: 0x0046EC80 File Offset: 0x0046CE80
		private void HideBookLockIcons()
		{
			bool flag = this.gradeIcon != null;
			if (flag)
			{
				this.gradeIcon.gameObject.SetActive(false);
			}
			bool flag2 = this.lockIcon != null;
			if (flag2)
			{
				this.lockIcon.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009817 RID: 38935 RVA: 0x0046ECD4 File Offset: 0x0046CED4
		private void SetLockTipsVisible(string tipsText)
		{
			bool flag = this.lockTips == null;
			if (!flag)
			{
				this.lockTips.gameObject.SetActive(true);
				bool flag2 = !string.IsNullOrEmpty(tipsText);
				if (flag2)
				{
					this.lockTips.text = tipsText;
				}
				Transform tipsBack = this.lockTips.transform.parent;
				bool flag3 = tipsBack != null;
				if (flag3)
				{
					tipsBack.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06009818 RID: 38936 RVA: 0x0046ED4C File Offset: 0x0046CF4C
		private void SetupLifeSkillBookMouseTip(SkillBookItem skillBookConfig, sbyte[] readingProgress)
		{
			bool flag = this.mouseTip == null || !this.mouseTip.enabled || readingProgress == null;
			if (!flag)
			{
				this.mouseTip.Type = TipType.LifeSkillDetailReadProgress;
				this.mouseTip.RuntimeParam = new ArgumentBox().SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyBookConfig, skillBookConfig).SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyReadProgresses, readingProgress);
				bool showing = this.mouseTip.Showing;
				if (showing)
				{
					this.mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x040074DC RID: 29916
		public CToggle toggle;

		// Token: 0x040074DD RID: 29917
		public CImage icon;

		// Token: 0x040074DE RID: 29918
		public TextMeshProUGUI buildingName;

		// Token: 0x040074DF RID: 29919
		public TooltipInvoker mouseTip;

		// Token: 0x040074E0 RID: 29920
		public CImage back;

		// Token: 0x040074E1 RID: 29921
		public TextMeshProUGUI desc1;

		// Token: 0x040074E2 RID: 29922
		public TextMeshProUGUI desc2;

		// Token: 0x040074E3 RID: 29923
		public TextMeshProUGUI needBlock;

		// Token: 0x040074E4 RID: 29924
		public CImage needBlockImage;

		// Token: 0x040074E5 RID: 29925
		public GameObject mask;

		// Token: 0x040074E6 RID: 29926
		public TextMeshProUGUI lockTips;

		// Token: 0x040074E7 RID: 29927
		public GameObject lockGo;

		// Token: 0x040074E8 RID: 29928
		public CImage lockIcon;

		// Token: 0x040074E9 RID: 29929
		public CImage gradeIcon;

		// Token: 0x040074EA RID: 29930
		public GameObject buildingCountInArea;

		// Token: 0x040074EB RID: 29931
		public TextMeshProUGUI countTip;

		// Token: 0x040074EC RID: 29932
		public GameObject warningInfo;

		// Token: 0x040074ED RID: 29933
		public GameObject infoArea;

		// Token: 0x040074EE RID: 29934
		public TextMeshProUGUI warningTip;

		// Token: 0x02002299 RID: 8857
		[Flags]
		public enum EBuildingStatus
		{
			// Token: 0x0400DB85 RID: 56197
			None = 0,
			// Token: 0x0400DB86 RID: 56198
			CanBuild = 1,
			// Token: 0x0400DB87 RID: 56199
			NotBuild = 2,
			// Token: 0x0400DB88 RID: 56200
			AlreadyBuild = 4,
			// Token: 0x0400DB89 RID: 56201
			Unlocked = 8,
			// Token: 0x0400DB8A RID: 56202
			Locked = 16
		}
	}
}
