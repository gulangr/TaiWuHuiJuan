using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AEF RID: 2799
	public class CombatDamageDetailLine : MonoBehaviour
	{
		// Token: 0x06008994 RID: 35220 RVA: 0x003FAB68 File Offset: 0x003F8D68
		public void SetState(CombatDamageDetailDisc.EState state)
		{
			if (!true)
			{
			}
			Sprite sprite2;
			if (state != CombatDamageDetailDisc.EState.Hit)
			{
				if (state != CombatDamageDetailDisc.EState.Miss)
				{
					sprite2 = this.notTriggeredSprite;
				}
				else
				{
					sprite2 = this.missSprite;
				}
			}
			else
			{
				sprite2 = this.successSprite;
			}
			if (!true)
			{
			}
			Sprite sprite = sprite2;
			this.lineImage.sprite = sprite;
		}

		// Token: 0x04006971 RID: 26993
		[SerializeField]
		private CImage lineImage;

		// Token: 0x04006972 RID: 26994
		[SerializeField]
		private Sprite successSprite;

		// Token: 0x04006973 RID: 26995
		[SerializeField]
		private Sprite missSprite;

		// Token: 0x04006974 RID: 26996
		[SerializeField]
		private Sprite notTriggeredSprite;
	}
}
