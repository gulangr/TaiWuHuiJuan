using System;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE1 RID: 3809
	public class RefineToolInfo : MonoBehaviour
	{
		// Token: 0x0600AF3C RID: 44860 RVA: 0x004FD7C1 File Offset: 0x004FB9C1
		public void SetAttainment(string lifeSkillName, string attainmentStr)
		{
			this.attainmentName.SetText(lifeSkillName, true);
			this.attainmentValue.SetText(attainmentStr, true);
		}

		// Token: 0x0600AF3D RID: 44861 RVA: 0x004FD7E0 File Offset: 0x004FB9E0
		public void SetDurability(string durabilityStr)
		{
			this.durabilityValue.SetText(durabilityStr, true);
		}

		// Token: 0x040087C4 RID: 34756
		[SerializeField]
		private TextMeshProUGUI attainmentValue;

		// Token: 0x040087C5 RID: 34757
		[SerializeField]
		private TextMeshProUGUI attainmentName;

		// Token: 0x040087C6 RID: 34758
		[SerializeField]
		private TextMeshProUGUI durabilityValue;
	}
}
