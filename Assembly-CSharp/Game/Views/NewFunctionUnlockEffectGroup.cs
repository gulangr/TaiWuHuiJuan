using System;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006F1 RID: 1777
	public class NewFunctionUnlockEffectGroup : MonoBehaviour
	{
		// Token: 0x06005457 RID: 21591 RVA: 0x00271EAC File Offset: 0x002700AC
		private void Awake()
		{
			bool flag = this.items == null || this.items.Length == 0;
			if (flag)
			{
				this.items = base.GetComponentsInChildren<NewFunctionUnlockEffectItem>(true);
			}
		}

		// Token: 0x06005458 RID: 21592 RVA: 0x00271EE0 File Offset: 0x002700E0
		public bool TryPlay(int unlockType)
		{
			this.StopAll();
			bool flag = this.items == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					NewFunctionUnlockEffectItem item = this.items[i];
					bool flag2 = item != null && item.UnlockType == unlockType;
					if (flag2)
					{
						item.Play();
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06005459 RID: 21593 RVA: 0x00271F58 File Offset: 0x00270158
		public void StopAll()
		{
			bool flag = this.items == null;
			if (!flag)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					bool flag2 = this.items[i] != null;
					if (flag2)
					{
						this.items[i].Stop();
					}
				}
			}
		}

		// Token: 0x04003920 RID: 14624
		[SerializeField]
		private NewFunctionUnlockEffectItem[] items;
	}
}
