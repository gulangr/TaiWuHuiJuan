using System;
using Coffee.UIExtensions;
using GameData.Domains.Extra;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009DC RID: 2524
	public class DefendHeavenlyTreeView : MonoBehaviour
	{
		// Token: 0x06007B63 RID: 31587 RVA: 0x00394DFC File Offset: 0x00392FFC
		public void SetData(SectStoryHeavenlyTreeExtendable treeData, int enemyCount, int villagerCount)
		{
			this.textTreeName.text = SharedMethods.GetTreeName((int)treeData.TemplateId);
			this.imageTreeGrowProgress.fillAmount = Mathf.Min(1f, (float)treeData.GrowPoint / 900f);
			this.imageTreeGrowPreviewProgress.fillAmount = 0f;
			int progress = (int)Math.Round((double)(100f * this.imageTreeGrowProgress.fillAmount), MidpointRounding.AwayFromZero);
			this.textTreeGrowProgress.text = progress.ToString() + "%";
			this._templateIndex = treeData.GrowTemplateId - 598;
			for (int i = 0; i < this.treeAnimArray.Length; i++)
			{
				this.treeAnimArray[i].SetActive(this._templateIndex == i);
			}
			this.textEnemyCount.text = enemyCount.ToString();
			this.textVillagerCount.text = villagerCount.ToString();
			foreach (UIParticle effect in this.effectUpgradeArray)
			{
				effect.Clear();
			}
		}

		// Token: 0x06007B64 RID: 31588 RVA: 0x00394F1C File Offset: 0x0039311C
		public void PlayEffectUpgrade()
		{
			for (int index = 0; index < this.effectUpgradeArray.Length; index++)
			{
				UIParticle effect = this.effectUpgradeArray[index];
				bool flag = index == this._templateIndex;
				if (flag)
				{
					effect.Play();
				}
				else
				{
					effect.Clear();
				}
			}
		}

		// Token: 0x06007B65 RID: 31589 RVA: 0x00394F6C File Offset: 0x0039316C
		public void PreviewProgress(int cur, int delta)
		{
			short max = 900;
			float curRate = Mathf.Min(1f, (float)cur / (float)max);
			this.imageTreeGrowProgress.fillAmount = curRate;
			int curRateStr = (int)Math.Round((double)(100f * curRate), MidpointRounding.AwayFromZero);
			delta = Mathf.Clamp(delta, 0, Math.Max(0, (int)max - cur));
			bool flag = delta <= 0;
			if (flag)
			{
				this.imageTreeGrowPreviewProgress.fillAmount = 0f;
				this.textTreeGrowProgress.text = curRateStr.ToString() + "%";
			}
			else
			{
				int targetPoint = cur + delta;
				float targetRate = Mathf.Min(1f, (float)targetPoint / (float)max);
				this.imageTreeGrowPreviewProgress.fillAmount = targetRate;
				int targetRateStr = (int)Math.Round((double)(100f * targetRate), MidpointRounding.AwayFromZero);
				int deltaRateStr = targetRateStr - curRateStr;
				string deltaProgressStr = (deltaRateStr > 0) ? string.Format("+{0}%", deltaRateStr).SetColor("brightblue") : string.Empty;
				this.textTreeGrowProgress.text = curRateStr.ToString() + deltaProgressStr;
			}
		}

		// Token: 0x04005DB0 RID: 23984
		[SerializeField]
		private GameObject[] treeAnimArray;

		// Token: 0x04005DB1 RID: 23985
		[SerializeField]
		private TextMeshProUGUI textTreeName;

		// Token: 0x04005DB2 RID: 23986
		[SerializeField]
		private CImage imageTreeGrowProgress;

		// Token: 0x04005DB3 RID: 23987
		[SerializeField]
		private CImage imageTreeGrowPreviewProgress;

		// Token: 0x04005DB4 RID: 23988
		[SerializeField]
		private TextMeshProUGUI textTreeGrowProgressTitle;

		// Token: 0x04005DB5 RID: 23989
		[SerializeField]
		private TextMeshProUGUI textTreeGrowProgress;

		// Token: 0x04005DB6 RID: 23990
		[SerializeField]
		private TextMeshProUGUI textEnemyCount;

		// Token: 0x04005DB7 RID: 23991
		[SerializeField]
		private TextMeshProUGUI textVillagerCount;

		// Token: 0x04005DB8 RID: 23992
		[SerializeField]
		private UIParticle effectBack;

		// Token: 0x04005DB9 RID: 23993
		[SerializeField]
		private UIParticle[] effectUpgradeArray;

		// Token: 0x04005DBA RID: 23994
		private int _templateIndex;
	}
}
