using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Config;
using GameData.Serializer;
using UnityEngine;
using UnityEngine.Assertions;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A4 RID: 1700
	public class EditingAdventureData : GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x06004F80 RID: 20352 RVA: 0x002530DF File Offset: 0x002512DF
		public void InitializeOnDeserializing()
		{
			this.CombatDifficulty = -1;
			this.Malice = new short[3];
			this.ItemCost = new List<EditingAdventureData.ItemCostItem[]>();
		}

		// Token: 0x06004F81 RID: 20353 RVA: 0x00253100 File Offset: 0x00251300
		public void FinishedDeserialization()
		{
			bool flag = this.CombatDifficulty < 0;
			if (flag)
			{
				this.CombatDifficulty = this.LifeSkillDifficulty;
			}
		}

		// Token: 0x06004F82 RID: 20354 RVA: 0x00253128 File Offset: 0x00251328
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			bool result;
			if (!(name == "EventLv"))
			{
				if (!(name == "InterruptAble"))
				{
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
							this.LastMapPosition.x = (float.TryParse(v["X"], out x) ? x : 0f);
							float y;
							this.LastMapPosition.y = (float.TryParse(v["Y"], out y) ? y : 0f);
						});
						result = true;
					}
				}
				else
				{
					proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<byte>(name, delegate(byte v)
					{
						this.Interruptible = v;
					});
					result = true;
				}
			}
			else
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<sbyte>(name, delegate(sbyte v)
				{
					this.LifeSkillDifficulty = v;
				});
				result = true;
			}
			return result;
		}

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x06004F83 RID: 20355 RVA: 0x002531C4 File Offset: 0x002513C4
		public bool Dirty
		{
			get
			{
				bool selfDirty = this._selfDirty;
				bool result;
				if (selfDirty)
				{
					result = true;
				}
				else
				{
					bool flag = this.StartNodes != null;
					if (flag)
					{
						foreach (EditingAdventureStartNode nodeData in this.StartNodes)
						{
							bool dirty = nodeData.Dirty;
							if (dirty)
							{
								return true;
							}
						}
					}
					bool flag2 = this.TransferNodes != null;
					if (flag2)
					{
						foreach (EditingAdventureTransferNode nodeData2 in this.TransferNodes)
						{
							bool dirty2 = nodeData2.Dirty;
							if (dirty2)
							{
								return true;
							}
						}
					}
					bool flag3 = this.EndNodes != null;
					if (flag3)
					{
						foreach (EditingAdventureEndNode nodeData3 in this.EndNodes)
						{
							bool dirty3 = nodeData3.Dirty;
							if (dirty3)
							{
								return true;
							}
						}
					}
					bool flag4 = this.BaseBranches != null;
					if (flag4)
					{
						foreach (EditingAdventureBaseBranch branchData in this.BaseBranches)
						{
							bool dirty4 = branchData.Dirty;
							if (dirty4)
							{
								return true;
							}
						}
					}
					bool flag5 = this.AdvancedBranches != null;
					if (flag5)
					{
						foreach (EditingAdventureAdvanceBranch branchData2 in this.AdvancedBranches)
						{
							bool dirty5 = branchData2.Dirty;
							if (dirty5)
							{
								return true;
							}
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x06004F84 RID: 20356 RVA: 0x002533E4 File Offset: 0x002515E4
		public EditingAdventureData()
		{
			this.Malice = new short[3];
			this.ResCost = new int[9];
			for (int i = 0; i < this.ResCost.Length; i++)
			{
				this.ResCost[i] = 0;
			}
			this.ItemCost = new List<EditingAdventureData.ItemCostItem[]>();
			this.AdventureParams = new List<string[]>();
		}

		// Token: 0x06004F85 RID: 20357 RVA: 0x00253477 File Offset: 0x00251677
		public void SetDirty(bool dirty = true)
		{
			this._selfDirty = dirty;
		}

		// Token: 0x06004F86 RID: 20358 RVA: 0x00253484 File Offset: 0x00251684
		private static void FormatAdventureData(EditingAdventureData data)
		{
			EditingAdventureData.<>c__DisplayClass32_0 CS$<>8__locals1;
			CS$<>8__locals1.data = data;
			CS$<>8__locals1.nodeDataList = new List<EditingAdventureNode>();
			CS$<>8__locals1.nodeDataList.AddRange(CS$<>8__locals1.data.StartNodes);
			CS$<>8__locals1.nodeDataList.AddRange(CS$<>8__locals1.data.TransferNodes);
			CS$<>8__locals1.nodeDataList.AddRange(CS$<>8__locals1.data.EndNodes);
			for (int i = 0; i < CS$<>8__locals1.data.BaseBranches.Count; i++)
			{
				EditingAdventureBaseBranch branchData = CS$<>8__locals1.data.BaseBranches[i];
				branchData.PortA = EditingAdventureData.<FormatAdventureData>g__GetPortIndexByGuid|32_0(branchData.InGuid, ref CS$<>8__locals1);
				branchData.PortB = EditingAdventureData.<FormatAdventureData>g__GetPortIndexByGuid|32_0(branchData.OutGuid, ref CS$<>8__locals1);
			}
			for (int j = 0; j < CS$<>8__locals1.data.AdvancedBranches.Count; j++)
			{
				EditingAdventureAdvanceBranch branchData2 = CS$<>8__locals1.data.AdvancedBranches[j];
				branchData2.ParentBranchId = EditingAdventureData.<FormatAdventureData>g__GetParentBranchIndexByGuid|32_1(branchData2.BelongBranchGuid, ref CS$<>8__locals1);
			}
		}

		// Token: 0x06004F87 RID: 20359 RVA: 0x00253598 File Offset: 0x00251798
		public AdventureItem ToAdventureItem()
		{
			EditingAdventureData.FormatAdventureData(this);
			short[] malice = new short[3];
			Array.Copy(this.Malice, malice, 3);
			List<ValueTuple<string, string, string, string>> paramsList = new List<ValueTuple<string, string, string, string>>();
			this.AdventureParams.ForEach(delegate(string[] argInfo)
			{
				paramsList.Add(new ValueTuple<string, string, string, string>(argInfo[0], argInfo[1], argInfo[2], (argInfo.Length > 3) ? argInfo[3] : string.Empty));
			});
			int[] resCost = new int[this.ResCost.Length];
			this.ResCost.CopyTo(resCost, 0);
			List<int[]> itemCost = new List<int[]>();
			this.ItemCost.ForEach(delegate(EditingAdventureData.ItemCostItem[] e)
			{
				int[] itemGroupData = new int[e.Length * 2];
				for (int i = 0; i < e.Length; i++)
				{
					itemGroupData[i * 2] = (int)e[i].Item1;
					itemGroupData[i * 2 + 1] = (int)e[i].Item2;
				}
				bool flag = e.Length != 0;
				if (flag)
				{
					itemCost.Add(itemGroupData);
				}
			});
			List<AdventureStartNode> startNodeList = this.StartNodes.ConvertAll<AdventureStartNode>((EditingAdventureStartNode e) => e.ToAdventureStartNode());
			List<AdventureTransferNode> transferNodeList = this.TransferNodes.ConvertAll<AdventureTransferNode>((EditingAdventureTransferNode e) => e.ToAdventureTransferNode());
			List<AdventureEndNode> endNodeList = this.EndNodes.ConvertAll<AdventureEndNode>((EditingAdventureEndNode e) => e.ToAdventureEndNode());
			List<AdventureBaseBranch> baseBranchList = this.BaseBranches.ConvertAll<AdventureBaseBranch>((EditingAdventureBaseBranch e) => e.ToAdventureBaseBranch());
			List<AdventureAdvancedBranch> advanceBranchList = new List<AdventureAdvancedBranch>();
			this.AdvancedBranches.ForEach(delegate(EditingAdventureAdvanceBranch e)
			{
				int index = -1;
				int i = 0;
				for (;;)
				{
					int num = i;
					List<EditingAdventureBaseBranch> baseBranches = this.BaseBranches;
					int? num2 = (baseBranches != null) ? new int?(baseBranches.Count) : null;
					if (!(num < num2.GetValueOrDefault() & num2 != null))
					{
						goto IL_74;
					}
					bool flag = this.BaseBranches[i].BranchGuid == e.BelongBranchGuid;
					if (flag)
					{
						break;
					}
					i++;
				}
				index = i;
				IL_74:
				Assert.AreEqual(index, e.ParentBranchId);
				bool flag2 = -1 != index;
				if (flag2)
				{
					advanceBranchList.Add(e.ToAdventureAdvancedBranch());
				}
			});
			return new AdventureItem(-1, "", "", this.Type, this.CombatDifficulty, this.LifeSkillDifficulty, this.Interruptible, this.TimeCost, this.KeepTime, resCost, itemCost, this.RestrictedByWorldPopulation, malice, paramsList, this.EnterEvent, startNodeList, transferNodeList, endNodeList, baseBranchList, advanceBranchList, this.DifficultyAddXiangshuLevel);
		}

		// Token: 0x06004F8B RID: 20363 RVA: 0x002537D8 File Offset: 0x002519D8
		[CompilerGenerated]
		internal static byte <FormatAdventureData>g__GetPortIndexByGuid|32_0(string guid, ref EditingAdventureData.<>c__DisplayClass32_0 A_1)
		{
			byte i = 0;
			while ((int)i < A_1.nodeDataList.Count)
			{
				bool flag = A_1.nodeDataList[(int)i].NodeGuid == guid;
				if (flag)
				{
					return i;
				}
				i += 1;
			}
			return byte.MaxValue;
		}

		// Token: 0x06004F8C RID: 20364 RVA: 0x0025382C File Offset: 0x00251A2C
		[CompilerGenerated]
		internal static int <FormatAdventureData>g__GetParentBranchIndexByGuid|32_1(string guid, ref EditingAdventureData.<>c__DisplayClass32_0 A_1)
		{
			for (int i = 0; i < A_1.data.BaseBranches.Count; i++)
			{
				bool flag = A_1.data.BaseBranches[i].BranchGuid == guid;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x040036AA RID: 13994
		public string DefKey = string.Empty;

		// Token: 0x040036AB RID: 13995
		public string Name;

		// Token: 0x040036AC RID: 13996
		public string Desc;

		// Token: 0x040036AD RID: 13997
		public short[] Malice;

		// Token: 0x040036AE RID: 13998
		public string EnterEvent;

		// Token: 0x040036AF RID: 13999
		public sbyte Type;

		// Token: 0x040036B0 RID: 14000
		public sbyte CombatDifficulty;

		// Token: 0x040036B1 RID: 14001
		public sbyte LifeSkillDifficulty;

		// Token: 0x040036B2 RID: 14002
		public byte TimeCost;

		// Token: 0x040036B3 RID: 14003
		public byte Interruptible;

		// Token: 0x040036B4 RID: 14004
		public sbyte KeepTime;

		// Token: 0x040036B5 RID: 14005
		public int[] ResCost;

		// Token: 0x040036B6 RID: 14006
		public bool RestrictedByWorldPopulation = false;

		// Token: 0x040036B7 RID: 14007
		public bool DifficultyAddXiangshuLevel = false;

		// Token: 0x040036B8 RID: 14008
		public List<EditingAdventureData.ItemCostItem[]> ItemCost;

		// Token: 0x040036B9 RID: 14009
		public List<string[]> AdventureParams;

		// Token: 0x040036BA RID: 14010
		public List<EditingAdventureStartNode> StartNodes;

		// Token: 0x040036BB RID: 14011
		public List<EditingAdventureTransferNode> TransferNodes;

		// Token: 0x040036BC RID: 14012
		public List<EditingAdventureEndNode> EndNodes;

		// Token: 0x040036BD RID: 14013
		public List<EditingAdventureBaseBranch> BaseBranches;

		// Token: 0x040036BE RID: 14014
		public List<EditingAdventureAdvanceBranch> AdvancedBranches;

		// Token: 0x040036BF RID: 14015
		private bool _selfDirty;

		// Token: 0x040036C0 RID: 14016
		public float MapScale = 1f;

		// Token: 0x040036C1 RID: 14017
		public Vector2 LastMapPosition = Vector2.zero;

		// Token: 0x02001AEC RID: 6892
		public struct ItemCostItem : GameData.Serializer.ICommonObjectDeserializationDirectValue, GameData.Serializer.ICommonObjectSerializationAware
		{
			// Token: 0x0600DFB0 RID: 57264 RVA: 0x005D80DB File Offset: 0x005D62DB
			public static implicit operator ValueTuple<sbyte, short>(EditingAdventureData.ItemCostItem v)
			{
				return new ValueTuple<sbyte, short>(v.Item1, v.Item2);
			}

			// Token: 0x0600DFB1 RID: 57265 RVA: 0x005D80F0 File Offset: 0x005D62F0
			public static implicit operator EditingAdventureData.ItemCostItem(ValueTuple<sbyte, short> v)
			{
				return new EditingAdventureData.ItemCostItem
				{
					Item1 = v.Item1,
					Item2 = v.Item2
				};
			}

			// Token: 0x0600DFB2 RID: 57266 RVA: 0x005D8120 File Offset: 0x005D6320
			public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
			{
				int idx;
				bool flag = int.TryParse(name, out idx);
				if (flag)
				{
					int num = idx;
					int num2 = num;
					if (num2 == 1)
					{
						proc = GameData.Serializer.CommonObjectSerializationMember.MakeTypeHintOnly<sbyte>(name);
						return true;
					}
					if (num2 == 2)
					{
						proc = GameData.Serializer.CommonObjectSerializationMember.MakeTypeHintOnly<short>(name);
						return true;
					}
				}
				proc = default(GameData.Serializer.CommonObjectSerializationMember);
				return false;
			}

			// Token: 0x0600DFB3 RID: 57267 RVA: 0x005D817C File Offset: 0x005D637C
			public void OnUnknownFieldGet(string name, object value)
			{
				int idx;
				bool flag = int.TryParse(name, out idx);
				if (flag)
				{
					int num = idx;
					int num2 = num;
					if (num2 != 1)
					{
						if (num2 == 2)
						{
							this.Item2 = ((IConvertible)value).ToInt16(CultureInfo.InvariantCulture);
						}
					}
					else
					{
						this.Item1 = ((IConvertible)value).ToSByte(CultureInfo.InvariantCulture);
					}
				}
			}

			// Token: 0x0400B76B RID: 46955
			public sbyte Item1;

			// Token: 0x0400B76C RID: 46956
			public short Item2;
		}
	}
}
