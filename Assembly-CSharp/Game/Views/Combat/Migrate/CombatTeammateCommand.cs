using System;
using Coffee.UIExtensions;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B59 RID: 2905
	public class CombatTeammateCommand : MonoBehaviour
	{
		// Token: 0x04006DF4 RID: 28148
		public int UserInt;

		// Token: 0x04006DF5 RID: 28149
		public CImage highLight;

		// Token: 0x04006DF6 RID: 28150
		public TextMeshProUGUI commandName;

		// Token: 0x04006DF7 RID: 28151
		public CImage mask;

		// Token: 0x04006DF8 RID: 28152
		public CImage medalIcon;

		// Token: 0x04006DF9 RID: 28153
		public RectTransform maskLine;

		// Token: 0x04006DFA RID: 28154
		public TextMeshProUGUI maskTimeCountDown;

		// Token: 0x04006DFB RID: 28155
		public GameObject lockObj;

		// Token: 0x04006DFC RID: 28156
		public CButton button;

		// Token: 0x04006DFD RID: 28157
		public UIParticle activeEffect;

		// Token: 0x04006DFE RID: 28158
		public UIParticle activeEffect2;
	}
}
