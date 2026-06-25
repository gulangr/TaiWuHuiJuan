using System;
using System.Collections.Generic;
using Config;
using GameData.Serializer;
using UnityEngine;

namespace AdventureEditor.Beta
{
	// Token: 0x020006AB RID: 1707
	public class EditingAdventureBaseBranch : EditingAdventureBranch, GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06004FA8 RID: 20392 RVA: 0x00253D78 File Offset: 0x00251F78
		public string Name
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this.BranchKey);
				string result;
				if (flag)
				{
					result = "BaseBranch_" + this.SortIndex.ToString();
				}
				else
				{
					result = "BaseBranch_" + this.BranchKey;
				}
				return result;
			}
		}

		// Token: 0x06004FA9 RID: 20393 RVA: 0x00253DC4 File Offset: 0x00251FC4
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			bool result;
			if (!(name == "MapPosition"))
			{
				proc = default(GameData.Serializer.CommonObjectSerializationMember);
				result = false;
			}
			else
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<Dictionary<string, string>>(name, delegate(Dictionary<string, string> v)
				{
					float x;
					this.BranchPosition.x = (float.TryParse(v["X"], out x) ? x : 0f);
					float y;
					this.BranchPosition.y = (float.TryParse(v["Y"], out y) ? y : 0f);
				});
				result = true;
			}
			return result;
		}

		// Token: 0x06004FAA RID: 20394 RVA: 0x00253E10 File Offset: 0x00252010
		public AdventureBaseBranch ToAdventureBaseBranch()
		{
			AdventureBaseBranch branch = new AdventureBaseBranch
			{
				PortA = this.PortA,
				PortB = this.PortB,
				BranchKey = this.BranchKey,
				LuckStoreCount = this.LuckStoreCount
			};
			base.ToAdventureBranch(branch);
			return branch;
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x00253E64 File Offset: 0x00252064
		public void CloneTo(EditingAdventureBaseBranch branchData)
		{
			bool flag = branchData == null;
			if (!flag)
			{
				branchData.GlobalEvent = this.GlobalEvent;
				branchData.Length = this.Length;
				branchData.LuckStoreCount = this.LuckStoreCount;
				for (int i = 0; i < this.PersonalityContentWeights.Length; i++)
				{
					branchData.PersonalityContentWeights[i].EventWeights.Clear();
					branchData.PersonalityContentWeights[i].EventWeights.AddRange(this.PersonalityContentWeights[i].EventWeights);
					branchData.PersonalityContentWeights[i].ItemRewardWeights.Clear();
					branchData.PersonalityContentWeights[i].ItemRewardWeights.AddRange(this.PersonalityContentWeights[i].ItemRewardWeights);
					branchData.PersonalityContentWeights[i].ResRewardWeights.Clear();
					branchData.PersonalityContentWeights[i].ResRewardWeights.AddRange(this.PersonalityContentWeights[i].ResRewardWeights);
					branchData.PersonalityContentWeights[i].BonusWeights.Clear();
					branchData.PersonalityContentWeights[i].BonusWeights.AddRange(this.PersonalityContentWeights[i].BonusWeights);
					branchData.PersonalityContentWeights[i].EmptyBlockWeight = this.PersonalityContentWeights[i].EmptyBlockWeight;
				}
				branchData.SkillWeights.Clear();
				branchData.SkillWeights.AddRange(this.SkillWeights);
				branchData.TerrainPersonalityWeights.Clear();
				branchData.TerrainPersonalityWeights.AddRange(this.TerrainPersonalityWeights);
			}
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x00253FE4 File Offset: 0x002521E4
		public override string ToString()
		{
			bool flag = string.IsNullOrEmpty(this.BranchKey);
			string result;
			if (flag)
			{
				result = "BaseBranch_" + this.SortIndex.ToString();
			}
			else
			{
				result = "BaseBranch_" + this.BranchKey;
			}
			return result;
		}

		// Token: 0x040036D9 RID: 14041
		private const string KeyPrefix = "BaseBranch_";

		// Token: 0x040036DA RID: 14042
		public string BranchKey;

		// Token: 0x040036DB RID: 14043
		public short LuckStoreCount = 1;

		// Token: 0x040036DC RID: 14044
		public byte PortA = byte.MaxValue;

		// Token: 0x040036DD RID: 14045
		public byte PortB = byte.MaxValue;

		// Token: 0x040036DE RID: 14046
		public Vector2 BranchPosition;

		// Token: 0x040036DF RID: 14047
		public ushort SortIndex;

		// Token: 0x040036E0 RID: 14048
		public string InGuid;

		// Token: 0x040036E1 RID: 14049
		public string OutGuid;

		// Token: 0x040036E2 RID: 14050
		public string BranchGuid = Guid.NewGuid().ToString("N");

		// Token: 0x040036E3 RID: 14051
		public bool HasAdvanceBranch;
	}
}
