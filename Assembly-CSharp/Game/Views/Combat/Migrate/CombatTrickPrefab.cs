using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B5A RID: 2906
	public class CombatTrickPrefab : MonoBehaviour
	{
		// Token: 0x04006DFF RID: 28159
		public int UserInt;

		// Token: 0x04006E00 RID: 28160
		public TextMeshProUGUI trickName;

		// Token: 0x04006E01 RID: 28161
		public CImage currTrickMark;

		// Token: 0x04006E02 RID: 28162
		public CImage mainImage;

		// Token: 0x04006E03 RID: 28163
		public GameObject currTrickTips;

		// Token: 0x04006E04 RID: 28164
		public GameObject costPreview;

		// Token: 0x04006E05 RID: 28165
		public List<GameObject> clickableHoverList;

		// Token: 0x04006E06 RID: 28166
		public UIParticle clickableParticle;
	}
}
