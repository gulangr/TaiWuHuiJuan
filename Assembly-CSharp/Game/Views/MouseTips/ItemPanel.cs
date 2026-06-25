using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000844 RID: 2116
	public class ItemPanel : MonoBehaviour
	{
		// Token: 0x17000C5F RID: 3167
		// (get) Token: 0x060066EB RID: 26347 RVA: 0x002EF2CF File Offset: 0x002ED4CF
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x060066EC RID: 26348 RVA: 0x002EF2DC File Offset: 0x002ED4DC
		public void Set(CharacterDisplayDataForMapBlock data)
		{
			this.neigong.Set(data.LearnedHighestGradeNeigongCombatSkillArray);
			this.attack.SetWithIcon(data.LearnedHighestGradeAttackCombatSkillArray);
			this.agile.Set(data.LearnedHighestGradeAgileCombatSkillArray);
			this.defense.Set(data.LearnedHighestGradeDefenseCombatSkillArray);
			this.assist.Set(data.LearnedHighestGradeAssistCombatSkillArray);
			this.love.Set(data.RelationshipDict.GetValueOrDefault(16384).Data ?? Array.Empty<NameAndAvatar>(), data.RelationshipCountDict.GetValueOrDefault(16384));
			this.hate.Set(data.RelationshipDict.GetValueOrDefault(32768).Data ?? Array.Empty<NameAndAvatar>(), data.RelationshipCountDict.GetValueOrDefault(32768));
			this.sworn.Set(data.RelationshipDict.GetValueOrDefault(512).Data ?? Array.Empty<NameAndAvatar>(), data.RelationshipCountDict.GetValueOrDefault(512));
			this.spouse.Set(data.RelationshipDict.GetValueOrDefault(1024).Data ?? Array.Empty<NameAndAvatar>(), data.RelationshipCountDict.GetValueOrDefault(1024));
			this.child.Set(data.RelationshipDict.GetValueOrDefault(146).Data ?? Array.Empty<NameAndAvatar>(), data.RelationshipCountDict.GetValueOrDefault(146));
			this.skillCounter.Set(data.LearnedCombatSkillCountArray);
		}

		// Token: 0x04004869 RID: 18537
		[SerializeField]
		private ItemPanelRelation love;

		// Token: 0x0400486A RID: 18538
		[SerializeField]
		private ItemPanelRelation hate;

		// Token: 0x0400486B RID: 18539
		[SerializeField]
		private ItemPanelRelation sworn;

		// Token: 0x0400486C RID: 18540
		[SerializeField]
		private ItemPanelRelation spouse;

		// Token: 0x0400486D RID: 18541
		[SerializeField]
		private ItemPanelRelation child;

		// Token: 0x0400486E RID: 18542
		[SerializeField]
		private SkillRender neigong;

		// Token: 0x0400486F RID: 18543
		[SerializeField]
		private SkillRender attack;

		// Token: 0x04004870 RID: 18544
		[SerializeField]
		private SkillRender agile;

		// Token: 0x04004871 RID: 18545
		[SerializeField]
		private SkillRender defense;

		// Token: 0x04004872 RID: 18546
		[SerializeField]
		private SkillRender assist;

		// Token: 0x04004873 RID: 18547
		[SerializeField]
		private SkillCounter skillCounter;
	}
}
