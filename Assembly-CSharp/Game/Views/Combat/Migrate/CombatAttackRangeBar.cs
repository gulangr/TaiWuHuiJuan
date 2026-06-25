using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B42 RID: 2882
	public class CombatAttackRangeBar : MonoBehaviour
	{
		// Token: 0x06008F3E RID: 36670 RVA: 0x0042C1E0 File Offset: 0x0042A3E0
		public CRawImage GetSkillRangeRawImage(int index)
		{
			bool flag = this.skillRangeList == null;
			if (flag)
			{
				this.skillRangeList = new List<CRawImage>
				{
					this.selfDefendSkillRange,
					this.selfTeammateDefendSkillRange,
					this.enemyDefendSkillRange,
					this.enemyTeammateDefendSkillRange
				};
			}
			return this.skillRangeList[index];
		}

		// Token: 0x04006D7A RID: 28026
		public RectTransform selfRange;

		// Token: 0x04006D7B RID: 28027
		public RectTransform dividerHolder;

		// Token: 0x04006D7C RID: 28028
		public RectTransform enemyRange;

		// Token: 0x04006D7D RID: 28029
		public RectTransform previewRange;

		// Token: 0x04006D7E RID: 28030
		public CRawImage selfDefendSkillRange;

		// Token: 0x04006D7F RID: 28031
		public CRawImage selfTeammateDefendSkillRange;

		// Token: 0x04006D80 RID: 28032
		public CRawImage enemyDefendSkillRange;

		// Token: 0x04006D81 RID: 28033
		public CRawImage enemyTeammateDefendSkillRange;

		// Token: 0x04006D82 RID: 28034
		public Texture2D defendInRange;

		// Token: 0x04006D83 RID: 28035
		public Texture2D defendOutRange;

		// Token: 0x04006D84 RID: 28036
		public CombatRangeText enemyRangeCanvas;

		// Token: 0x04006D85 RID: 28037
		public CombatRangeText selfRangeCanvas;

		// Token: 0x04006D86 RID: 28038
		public CombatRangeText previewRangeCanvas;

		// Token: 0x04006D87 RID: 28039
		private List<CRawImage> skillRangeList;
	}
}
