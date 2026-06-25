using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B55 RID: 2901
	public class CombatOtherActionHolder : MonoBehaviour
	{
		// Token: 0x04006DDD RID: 28125
		public CButton surrender;

		// Token: 0x04006DDE RID: 28126
		public CButton useItem;

		// Token: 0x04006DDF RID: 28127
		public List<CButton> otherActionTypeList;

		// Token: 0x04006DE0 RID: 28128
		public RectTransform reserveParent;
	}
}
