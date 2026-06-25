using System;
using System.Collections.Generic;
using Config;

namespace AdventureEditor.Beta
{
	// Token: 0x020006AA RID: 1706
	public abstract class EditingAdventureBranch
	{
		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06004FA4 RID: 20388 RVA: 0x00253C4B File Offset: 0x00251E4B
		public bool Dirty
		{
			get
			{
				return this._dirty;
			}
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x00253C54 File Offset: 0x00251E54
		protected EditingAdventureBranch()
		{
			this.SkillWeights = new List<ValueTuple<byte, short>>();
			this.PersonalityContentWeights = new AdvPersonalityEventWeights[5];
			for (int i = 0; i < this.PersonalityContentWeights.Length; i++)
			{
				this.PersonalityContentWeights[i] = new AdvPersonalityEventWeights();
			}
			this.TerrainPersonalityWeights = new List<ValueTuple<byte, short, short[]>>();
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x00253CB1 File Offset: 0x00251EB1
		public void SetDirty(bool dirty)
		{
			this._dirty = dirty;
		}

		// Token: 0x06004FA7 RID: 20391 RVA: 0x00253CBC File Offset: 0x00251EBC
		protected void ToAdventureBranch(AdventureBranch branch)
		{
			branch.Length = this.Length;
			branch.GlobalEvent = this.GlobalEvent;
			branch.SkillWeights = this.SkillWeights.ToArray();
			branch.TerrainPersonalityWeights = this.TerrainPersonalityWeights.ToArray();
			branch.PersonalityContentWeights = new AdventurePersonalityContentWeights[this.PersonalityContentWeights.Length];
			for (int i = 0; i < this.PersonalityContentWeights.Length; i++)
			{
				AdvPersonalityEventWeights weights = this.PersonalityContentWeights[i];
				branch.PersonalityContentWeights[i] = new AdventurePersonalityContentWeights(weights.EmptyBlockWeight, weights.EventWeights.ToArray(), weights.ResRewardWeights.ToArray(), weights.ItemRewardWeights.ToArray(), weights.BonusWeights.ToArray());
			}
		}

		// Token: 0x040036D3 RID: 14035
		public short Length;

		// Token: 0x040036D4 RID: 14036
		public string GlobalEvent;

		// Token: 0x040036D5 RID: 14037
		public readonly List<ValueTuple<byte, short>> SkillWeights;

		// Token: 0x040036D6 RID: 14038
		public readonly List<ValueTuple<byte, short, short[]>> TerrainPersonalityWeights;

		// Token: 0x040036D7 RID: 14039
		public readonly AdvPersonalityEventWeights[] PersonalityContentWeights;

		// Token: 0x040036D8 RID: 14040
		private bool _dirty;
	}
}
