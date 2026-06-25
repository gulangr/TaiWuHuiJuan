using System;
using FrameWork;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ACF RID: 2767
	public class CricketCombatScore : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F02 RID: 3842
		// (get) Token: 0x06008857 RID: 34903 RVA: 0x003F41CE File Offset: 0x003F23CE
		// (set) Token: 0x06008858 RID: 34904 RVA: 0x003F41D6 File Offset: 0x003F23D6
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x06008859 RID: 34905 RVA: 0x003F41E0 File Offset: 0x003F23E0
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize || type == ECricketCombatGlobalEventType.MatchPrepare;
			if (flag)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600885A RID: 34906 RVA: 0x003F4203 File Offset: 0x003F2403
		private void Refresh()
		{
			this.leftScoreDigit.SetSprite(CricketCombatScore.GetSpriteName(CricketCombatKit.Board.MatchWinCount), true, null);
			this.rightScoreDigit.SetSprite(CricketCombatScore.GetSpriteName(CricketCombatKit.Board.MatchLoseCount), true, null);
		}

		// Token: 0x0600885B RID: 34907 RVA: 0x003F4240 File Offset: 0x003F2440
		private static string GetSpriteName(int score)
		{
			return string.Format("{0}{1}", "ui9_number_cricketcombat_score_3_", Mathf.Clamp(score, 0, 2));
		}

		// Token: 0x04006873 RID: 26739
		private const int MaxScoreDigit = 2;

		// Token: 0x04006874 RID: 26740
		[SerializeField]
		private CImage leftScoreDigit;

		// Token: 0x04006875 RID: 26741
		[SerializeField]
		private CImage rightScoreDigit;
	}
}
