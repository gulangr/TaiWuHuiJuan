using System;
using TMPro;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000993 RID: 2451
	public class QualificationAndAttainment : MonoBehaviour
	{
		// Token: 0x06007624 RID: 30244 RVA: 0x00371184 File Offset: 0x0036F384
		public void Set(string title, int qual, int att)
		{
			this.type.text = title;
			this.qualification.text = LanguageKey.LK_CharacterMenu_QualificationWithVal.TrFormat(qual);
			this.attainment.text = LanguageKey.LK_CharacterMenu_AttainmentWithVal.TrFormat(att);
		}

		// Token: 0x040058F0 RID: 22768
		[SerializeField]
		private CImage icon;

		// Token: 0x040058F1 RID: 22769
		[SerializeField]
		private TMP_Text type;

		// Token: 0x040058F2 RID: 22770
		[SerializeField]
		private TMP_Text qualification;

		// Token: 0x040058F3 RID: 22771
		[SerializeField]
		private TMP_Text attainment;
	}
}
