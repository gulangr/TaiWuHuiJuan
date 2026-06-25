using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B60 RID: 2912
	internal static class CharacterMenuAttainmentUnlockParticleCache
	{
		// Token: 0x06009039 RID: 36921 RVA: 0x0043386A File Offset: 0x00431A6A
		public static bool IsValidParticleRootIndex(int particleRootIndex)
		{
			return particleRootIndex >= 0 && particleRootIndex < 2;
		}

		// Token: 0x0600903A RID: 36922 RVA: 0x00433878 File Offset: 0x00431A78
		public static void RegisterSharedParticleRoot(int particleRootIndex, UIParticle root)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex) || root == null;
			if (!flag)
			{
				CharacterMenuAttainmentUnlockParticleCache.SharedParticleRoots[particleRootIndex] = root;
			}
		}

		// Token: 0x0600903B RID: 36923 RVA: 0x004338A6 File Offset: 0x00431AA6
		public static UIParticle GetSharedParticleRoot(int particleRootIndex)
		{
			return CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex) ? CharacterMenuAttainmentUnlockParticleCache.SharedParticleRoots[particleRootIndex] : null;
		}

		// Token: 0x0600903C RID: 36924 RVA: 0x004338BC File Offset: 0x00431ABC
		public static void ApplyExclusiveParticleRoot(int activeParticleRootIndex)
		{
			for (int i = 0; i < 2; i++)
			{
				UIParticle root = CharacterMenuAttainmentUnlockParticleCache.SharedParticleRoots[i];
				bool flag = root == null;
				if (!flag)
				{
					bool shouldActive = activeParticleRootIndex >= 0 && i == activeParticleRootIndex;
					bool flag2 = root.gameObject.activeSelf == shouldActive;
					if (!flag2)
					{
						bool flag3 = !shouldActive;
						if (flag3)
						{
							foreach (ParticleSystem ps in root.GetComponentsInChildren<ParticleSystem>(true))
							{
								bool flag4 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(ps.gameObject.name);
								if (!flag4)
								{
									ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
									bool activeSelf = ps.gameObject.activeSelf;
									if (activeSelf)
									{
										ps.gameObject.SetActive(false);
									}
								}
							}
						}
						root.gameObject.SetActive(shouldActive);
					}
				}
			}
		}

		// Token: 0x0600903D RID: 36925 RVA: 0x004339A3 File Offset: 0x00431BA3
		public static void DeactivateAllSharedParticleRoots()
		{
			CharacterMenuAttainmentUnlockParticleCache.ApplyExclusiveParticleRoot(-1);
		}

		// Token: 0x0600903E RID: 36926 RVA: 0x004339AC File Offset: 0x00431BAC
		public static string BuildNodeRecordKey(string effectNodeName)
		{
			return effectNodeName;
		}

		// Token: 0x0600903F RID: 36927 RVA: 0x004339B0 File Offset: 0x00431BB0
		public static bool IsEffectNodePlayed(int particleRootIndex, string effectNodeName, bool debugReplay)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(effectNodeName);
				if (flag2)
				{
					result = CharacterMenuAttainmentUnlockParticleCache.IsShuNodePlayed(particleRootIndex, effectNodeName);
				}
				else
				{
					result = (!debugReplay && CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Contains(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey(effectNodeName)));
				}
			}
			return result;
		}

		// Token: 0x06009040 RID: 36928 RVA: 0x00433A04 File Offset: 0x00431C04
		public static bool IsShuNodePlayed(int particleRootIndex, string shuNodeName)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex) || !CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(shuNodeName);
			return !flag && CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Contains(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey(shuNodeName));
		}

		// Token: 0x06009041 RID: 36929 RVA: 0x00433A44 File Offset: 0x00431C44
		public static void MarkEffectNodePlayed(int particleRootIndex, string effectNodeName, bool debugReplay)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex);
			if (!flag)
			{
				bool flag2 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(effectNodeName);
				if (flag2)
				{
					CharacterMenuAttainmentUnlockParticleCache.MarkShuNodePlayed(particleRootIndex, effectNodeName);
				}
				else if (!debugReplay)
				{
					CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Add(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey(effectNodeName));
				}
			}
		}

		// Token: 0x06009042 RID: 36930 RVA: 0x00433A90 File Offset: 0x00431C90
		public static void MarkShuNodePlayed(int particleRootIndex, string shuNodeName)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex) || !CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(shuNodeName);
			if (!flag)
			{
				CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Add(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey(shuNodeName));
			}
		}

		// Token: 0x06009043 RID: 36931 RVA: 0x00433ACC File Offset: 0x00431CCC
		public static void MarkSlotPrimaryNodesPlayed(int particleRootIndex, List<int> slotGrades, bool debugReplay)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex) || slotGrades == null;
			if (!flag)
			{
				for (int i = 0; i < slotGrades.Count; i++)
				{
					int grade = slotGrades[i];
					bool flag2 = grade == 0;
					if (flag2)
					{
						if (!debugReplay)
						{
							CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Add(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey("xishou"));
						}
					}
					else
					{
						CharacterMenuAttainmentUnlockParticleCache.MarkShuNodePlayed(particleRootIndex, CharacterMenuAttainmentUnlockParticleCache.GetShuNodeName(grade));
					}
				}
			}
		}

		// Token: 0x06009044 RID: 36932 RVA: 0x00433B48 File Offset: 0x00431D48
		public static bool ShouldPlaySlotUnlockEffect(int particleRootIndex, int slotGrade, bool[] slotUnlocked, bool debugReplay)
		{
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = slotUnlocked == null || slotGrade < 0 || slotGrade >= slotUnlocked.Length || !slotUnlocked[slotGrade];
				if (flag2)
				{
					result = false;
				}
				else if (debugReplay)
				{
					result = true;
				}
				else
				{
					string playedNodeName = (slotGrade == 0) ? "xishou" : CharacterMenuAttainmentUnlockParticleCache.GetShuNodeName(slotGrade);
					bool flag3 = slotGrade == 0;
					if (flag3)
					{
						result = !CharacterMenuAttainmentUnlockParticleCache.IsEffectNodePlayed(particleRootIndex, playedNodeName, false);
					}
					else
					{
						result = !CharacterMenuAttainmentUnlockParticleCache.IsShuNodePlayed(particleRootIndex, playedNodeName);
					}
				}
			}
			return result;
		}

		// Token: 0x06009045 RID: 36933 RVA: 0x00433BC8 File Offset: 0x00431DC8
		public static void CollectPlayedShuNodeNames(int particleRootIndex, List<string> result)
		{
			result.Clear();
			bool flag = !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex);
			if (!flag)
			{
				foreach (string nodeName in CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex])
				{
					bool flag2 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(nodeName);
					if (flag2)
					{
						result.Add(nodeName);
					}
				}
			}
		}

		// Token: 0x06009046 RID: 36934 RVA: 0x00433C44 File Offset: 0x00431E44
		public static void ClearParticleRoot(int particleRootIndex)
		{
			bool flag = CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex);
			if (flag)
			{
				CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Clear();
			}
		}

		// Token: 0x06009047 RID: 36935 RVA: 0x00433C6C File Offset: 0x00431E6C
		public static void ClearEffectNodesForGrade(int particleRootIndex, int slotGrade, bool debugReplay)
		{
			bool flag = debugReplay || !CharacterMenuAttainmentUnlockParticleCache.IsValidParticleRootIndex(particleRootIndex);
			if (!flag)
			{
				bool flag2 = slotGrade == 0;
				if (flag2)
				{
					CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Remove(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey("xishou"));
				}
				else
				{
					CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Remove(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey(CharacterMenuAttainmentUnlockParticleCache.GetShuNodeName(slotGrade)));
					CharacterMenuAttainmentUnlockParticleCache.PlayedEffectNodes[particleRootIndex].Remove(CharacterMenuAttainmentUnlockParticleCache.BuildNodeRecordKey(CharacterMenuAttainmentUnlockParticleCache.GetXianNodeName(slotGrade)));
				}
			}
		}

		// Token: 0x06009048 RID: 36936 RVA: 0x00433CE1 File Offset: 0x00431EE1
		public static string GetShuNodeName(int slotGrade)
		{
			return (slotGrade <= 0) ? "xishou" : string.Format("shu0{0}", slotGrade);
		}

		// Token: 0x06009049 RID: 36937 RVA: 0x00433CFE File Offset: 0x00431EFE
		public static string GetXianNodeName(int slotGrade)
		{
			return (slotGrade <= 0) ? "xishou" : string.Format("xian0{0}", slotGrade);
		}

		// Token: 0x0600904A RID: 36938 RVA: 0x00433D1B File Offset: 0x00431F1B
		public static bool IsShuNodeName(string nodeName)
		{
			return !string.IsNullOrEmpty(nodeName) && nodeName.Length == 5 && nodeName.StartsWith("shu0");
		}

		// Token: 0x0600904B RID: 36939 RVA: 0x00433D3C File Offset: 0x00431F3C
		public static bool IsXianNodeName(string nodeName)
		{
			return !string.IsNullOrEmpty(nodeName) && nodeName.Length == 6 && nodeName.StartsWith("xian0");
		}

		// Token: 0x0600904C RID: 36940 RVA: 0x00433D60 File Offset: 0x00431F60
		public static bool IsUnlockEffectNodeName(string nodeName)
		{
			bool flag = nodeName == "xishou";
			return flag || CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(nodeName) || CharacterMenuAttainmentUnlockParticleCache.IsXianNodeName(nodeName);
		}

		// Token: 0x04006EE5 RID: 28389
		public const int Particle01Index = 0;

		// Token: 0x04006EE6 RID: 28390
		public const int Particle02Index = 1;

		// Token: 0x04006EE7 RID: 28391
		private const int ParticleRootCount = 2;

		// Token: 0x04006EE8 RID: 28392
		private static readonly UIParticle[] SharedParticleRoots = new UIParticle[2];

		// Token: 0x04006EE9 RID: 28393
		private static readonly HashSet<string>[] PlayedEffectNodes = new HashSet<string>[]
		{
			new HashSet<string>(),
			new HashSet<string>()
		};
	}
}
