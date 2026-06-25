using System;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F4F RID: 3919
	[RequireComponent(typeof(CImage), typeof(RectTransform))]
	public class LifeRecordSummaryYearComplexLine : MonoBehaviour
	{
		// Token: 0x0600B3C2 RID: 46018 RVA: 0x0051D0E4 File Offset: 0x0051B2E4
		public void Set(RectTransform from, RectTransform to, int spriteIndex = -1)
		{
			base.gameObject.SetActive(true);
			bool flag = this.sprites.CheckIndex(spriteIndex);
			if (flag)
			{
				this.line.sprite = this.sprites[spriteIndex];
			}
			Vector3 dir = from.position - to.position;
			this.self.sizeDelta = this.self.sizeDelta.SetY(dir.magnitude * (from.localScale + to.localScale).magnitude / (from.lossyScale + to.lossyScale).magnitude);
			this.self.SetPositionAndRotation((from.position + to.position) / 2f, Quaternion.LookRotation(Vector3.forward, dir));
		}

		// Token: 0x04008BCB RID: 35787
		[SerializeField]
		private Sprite[] sprites;

		// Token: 0x04008BCC RID: 35788
		[SerializeField]
		private CImage line;

		// Token: 0x04008BCD RID: 35789
		[SerializeField]
		private RectTransform self;
	}
}
