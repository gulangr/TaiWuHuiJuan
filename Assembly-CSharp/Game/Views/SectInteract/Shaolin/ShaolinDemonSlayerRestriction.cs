using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Shaolin
{
	// Token: 0x020009E7 RID: 2535
	public class ShaolinDemonSlayerRestriction : MonoBehaviour
	{
		// Token: 0x06007C7F RID: 31871 RVA: 0x0039DC98 File Offset: 0x0039BE98
		public void Set(DemonSlayerTrialRestrictItem config, bool isMeet)
		{
			this.checkLabel.sprite = (isMeet ? this.checkSprite : this.uncheckSprite);
			this.checkBg.sprite = (isMeet ? this.checkBgSprite : this.uncheckBgSprite);
			this.descLabel.text = config.Desc.SetGradeColor((int)((sbyte)(config.Power - 1)));
		}

		// Token: 0x04005EA5 RID: 24229
		public CImage checkLabel;

		// Token: 0x04005EA6 RID: 24230
		public CImage checkBg;

		// Token: 0x04005EA7 RID: 24231
		public TextMeshProUGUI descLabel;

		// Token: 0x04005EA8 RID: 24232
		public Sprite checkSprite;

		// Token: 0x04005EA9 RID: 24233
		public Sprite uncheckSprite;

		// Token: 0x04005EAA RID: 24234
		public Sprite checkBgSprite;

		// Token: 0x04005EAB RID: 24235
		public Sprite uncheckBgSprite;
	}
}
