using System;
using Game.Components.Character;
using GameData.Domains.Character;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200087A RID: 2170
	public class PoisonInfo : MonoBehaviour
	{
		// Token: 0x06006860 RID: 26720 RVA: 0x002FB9FC File Offset: 0x002F9BFC
		public unsafe void Refresh(PoisonInts poisons)
		{
			for (sbyte type = 0; type < 6; type += 1)
			{
				InjuryPoisonItem item = this.poisonItems[(int)type];
				int value = *(ref poisons.Items.FixedElementField + (IntPtr)type * 4);
				item.Set(type, value, 0, false, false);
			}
		}

		// Token: 0x04004A15 RID: 18965
		[SerializeField]
		private InjuryPoisonItem[] poisonItems;
	}
}
