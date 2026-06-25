using System;
using Coffee.UIExtensions;
using UnityEngine;

namespace Game.Views.Debate.DebateResult
{
	// Token: 0x02000AAB RID: 2731
	public class DebateResultWinLose : MonoBehaviour
	{
		// Token: 0x06008629 RID: 34345 RVA: 0x003E7220 File Offset: 0x003E5420
		public void Set(bool isWin)
		{
			if (isWin)
			{
				this.lose.gameObject.SetActive(false);
				this.win.gameObject.SetActive(true);
				this.win.Play();
			}
			else
			{
				this.win.gameObject.SetActive(false);
				this.lose.gameObject.SetActive(true);
				this.lose.Play();
			}
		}

		// Token: 0x040066FD RID: 26365
		[SerializeField]
		private UIParticle win;

		// Token: 0x040066FE RID: 26366
		[SerializeField]
		private UIParticle lose;
	}
}
