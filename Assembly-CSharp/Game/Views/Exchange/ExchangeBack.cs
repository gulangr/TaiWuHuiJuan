using System;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A26 RID: 2598
	public class ExchangeBack : MonoBehaviour
	{
		// Token: 0x06007F67 RID: 32615 RVA: 0x003B5818 File Offset: 0x003B3A18
		public void SetBack(int index)
		{
			foreach (GameObject back in this.backs)
			{
				back.SetActive(index-- == 0);
			}
		}

		// Token: 0x06007F68 RID: 32616 RVA: 0x003B5850 File Offset: 0x003B3A50
		public void SetMerchant(int templateId)
		{
			this.SetBack(this.merchantStart + templateId);
		}

		// Token: 0x06007F69 RID: 32617 RVA: 0x003B5861 File Offset: 0x003B3A61
		public void SetTreasury(int offset = 0)
		{
			this.SetBack(this.treasuryIndex + offset);
		}

		// Token: 0x06007F6A RID: 32618 RVA: 0x003B5872 File Offset: 0x003B3A72
		public void SetNpc(int offset = 0)
		{
			this.SetBack(this.npcIndex + offset);
		}

		// Token: 0x06007F6B RID: 32619 RVA: 0x003B5883 File Offset: 0x003B3A83
		public void SetInvalid()
		{
			this.SetBack(-1);
		}

		// Token: 0x04006185 RID: 24965
		[SerializeField]
		private GameObject[] backs;

		// Token: 0x04006186 RID: 24966
		[SerializeField]
		public int treasuryIndex = 7;

		// Token: 0x04006187 RID: 24967
		[SerializeField]
		public int npcIndex = 7;

		// Token: 0x04006188 RID: 24968
		[SerializeField]
		public int merchantStart = 0;
	}
}
