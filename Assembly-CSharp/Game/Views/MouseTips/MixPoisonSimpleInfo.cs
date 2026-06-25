using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000836 RID: 2102
	public class MixPoisonSimpleInfo : MonoBehaviour
	{
		// Token: 0x040047DC RID: 18396
		[SerializeField]
		public CImage[] poisonImages;

		// Token: 0x040047DD RID: 18397
		[SerializeField]
		public TextMeshProUGUI mixPoisonName;

		// Token: 0x040047DE RID: 18398
		[SerializeField]
		public TextMeshProUGUI affectPoisonTypes;

		// Token: 0x040047DF RID: 18399
		[SerializeField]
		public TextMeshProUGUI leftCount;

		// Token: 0x040047E0 RID: 18400
		[SerializeField]
		public DisableStyleRoot disableStyle;

		// Token: 0x040047E1 RID: 18401
		[SerializeField]
		public CImage back;
	}
}
