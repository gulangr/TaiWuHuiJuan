using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B03 RID: 2819
	public class CombatCirclePanelReserve : MonoBehaviour
	{
		// Token: 0x17000F4D RID: 3917
		// (get) Token: 0x06008AB7 RID: 35511 RVA: 0x00403249 File Offset: 0x00401449
		private int ReserveStyleCount
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06008AB8 RID: 35512 RVA: 0x0040324C File Offset: 0x0040144C
		public void Refresh(CombatCirclePanelReserve.EReserveType type, RectTransform target)
		{
			if (!true)
			{
			}
			int num;
			if (type != CombatCirclePanelReserve.EReserveType.OtherAction)
			{
				if (type != CombatCirclePanelReserve.EReserveType.UseItem)
				{
					throw new ArgumentOutOfRangeException("type", type, null);
				}
				num = 1;
			}
			else
			{
				num = 0;
			}
			if (!true)
			{
			}
			int index = num;
			RectTransform rt = base.GetComponent<RectTransform>();
			rt.SetParent(target);
			rt.pivot = this.defaultPivot;
			rt.anchoredPosition = Vector2.zero;
			int activateEffectIndex = index;
			for (int i = 0; i < this.ReserveStyleCount; i++)
			{
				Transform effect = base.transform.GetChild(i);
				effect.gameObject.SetActive(i == activateEffectIndex);
			}
		}

		// Token: 0x06008AB9 RID: 35513 RVA: 0x004032F4 File Offset: 0x004014F4
		public void Cancel()
		{
			RectTransform rt = base.GetComponent<RectTransform>();
			rt.SetParent(this.defaultParent);
			for (int i = 0; i < this.ReserveStyleCount; i++)
			{
				Transform effect = base.transform.GetChild(i);
				effect.gameObject.SetActive(false);
			}
		}

		// Token: 0x04006A56 RID: 27222
		[SerializeField]
		private Transform defaultParent;

		// Token: 0x04006A57 RID: 27223
		[SerializeField]
		private Vector2 defaultPivot;

		// Token: 0x020020CB RID: 8395
		public enum EReserveType
		{
			// Token: 0x0400D22B RID: 53803
			OtherAction,
			// Token: 0x0400D22C RID: 53804
			UseItem,
			// Token: 0x0400D22D RID: 53805
			Count
		}
	}
}
