using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Relation.RelationTree;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public static class GenealogyMaker
{
	// Token: 0x06001EAC RID: 7852 RVA: 0x000DD480 File Offset: 0x000DB680
	public static Dictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> MakeNodes(this Genealogy genealogy)
	{
		Dictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> dictionary = new Dictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>>();
		GenealogyMaker.GenealogyNode self = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Self, genealogy.CoreCharId, ushort.MaxValue, null);
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.Self, new List<GenealogyMaker.GenealogyNode>
		{
			self
		});
		GenealogyMaker.GenealogyNode grandFather = null;
		GenealogyMaker.GenealogyNode grandMother = null;
		GenealogyMaker.GenealogyNode grandFatherMaternal = null;
		GenealogyMaker.GenealogyNode grandMotherMaternal = null;
		List<GenealogyMaker.GenealogyNode> list = new List<GenealogyMaker.GenealogyNode>();
		bool flag = genealogy.GrandfatherId >= 0;
		if (flag)
		{
			list.Add(grandFather = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.GrandParents, genealogy.GrandfatherId, ushort.MaxValue, null));
		}
		bool flag2 = genealogy.GrandmotherId >= 0;
		if (flag2)
		{
			list.Add(grandMother = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.GrandParents, genealogy.GrandmotherId, ushort.MaxValue, grandFather));
		}
		bool flag3 = genealogy.MaternalGrandfatherId >= 0;
		if (flag3)
		{
			list.Add(grandFatherMaternal = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.GrandParents, genealogy.MaternalGrandfatherId, ushort.MaxValue, null));
		}
		bool flag4 = genealogy.MaternalGrandmotherId >= 0;
		if (flag4)
		{
			list.Add(grandMotherMaternal = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.GrandParents, genealogy.MaternalGrandmotherId, ushort.MaxValue, grandFatherMaternal));
		}
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.GrandParents, list);
		GenealogyMaker.GenealogyNode bloodFather = null;
		GenealogyMaker.GenealogyNode bloodMother = null;
		List<GenealogyMaker.GenealogyNode> list2 = new List<GenealogyMaker.GenealogyNode>();
		bool flag5 = genealogy.BloodFatherId >= 0;
		if (flag5)
		{
			bloodFather = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Parents, genealogy.BloodFatherId, 1, null);
			bloodFather.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(grandFather, 1));
			GenealogyMaker.AddNodeToMain(grandFather, bloodFather);
			bloodFather.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(grandMother, 1));
			GenealogyMaker.AddNodeToMain(grandMother, bloodFather);
			self.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(bloodFather, 1));
			GenealogyMaker.AddNodeToMain(bloodFather, self);
			list2.Add(bloodFather);
		}
		bool flag6 = genealogy.BloodMotherId >= 0;
		if (flag6)
		{
			bloodMother = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Parents, genealogy.BloodMotherId, 1, null);
			bloodMother.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(grandFatherMaternal, 1));
			GenealogyMaker.AddNodeToMain(grandFatherMaternal, bloodMother);
			bloodMother.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(grandMotherMaternal, 1));
			GenealogyMaker.AddNodeToMain(grandMotherMaternal, bloodMother);
			self.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(bloodMother, 1));
			GenealogyMaker.AddNodeToMain(bloodMother, self);
			list2.Add(bloodMother);
		}
		bool flag7 = genealogy.Parents != null;
		if (flag7)
		{
			foreach (CharIdAndRelation charIdAndRelation in genealogy.Parents)
			{
				bool flag8 = charIdAndRelation.RelationType == 1;
				if (!flag8)
				{
					GenealogyMaker.GenealogyNode node = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Parents, charIdAndRelation.CharId, charIdAndRelation.RelationType, null);
					self.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(node, charIdAndRelation.RelationType));
					GenealogyMaker.AddNodeToMain(node, self);
					list2.Add(node);
				}
			}
		}
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.Parents, list2);
		List<GenealogyMaker.GenealogyNode> list3 = new List<GenealogyMaker.GenealogyNode>();
		bool flag9 = genealogy.BrothersAndSisters != null;
		if (flag9)
		{
			foreach (CharIdAndRelation charIdAndRelation2 in genealogy.BrothersAndSisters)
			{
				bool flag10 = charIdAndRelation2.CharId == genealogy.CoreCharId;
				if (!flag10)
				{
					bool flag11 = charIdAndRelation2.RelationType == 4;
					GenealogyMaker.GenealogyNode node2;
					if (flag11)
					{
						node2 = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Siblings, charIdAndRelation2.CharId, 4, self);
						node2.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(bloodFather, 1));
						node2.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(bloodMother, 1));
					}
					else
					{
						node2 = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Siblings, charIdAndRelation2.CharId, charIdAndRelation2.RelationType, null);
					}
					node2.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(self, charIdAndRelation2.RelationType));
					GenealogyMaker.AddNodeToMain(node2, self);
					list3.Add(node2);
				}
			}
		}
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.Siblings, list3);
		List<GenealogyMaker.GenealogyNode> listSpouses = new List<GenealogyMaker.GenealogyNode>();
		List<GenealogyMaker.GenealogyNode> listChildren = new List<GenealogyMaker.GenealogyNode>();
		List<GenealogyMaker.GenealogyNode> listGrandChildren = new List<GenealogyMaker.GenealogyNode>();
		bool flag12 = genealogy.Spouses != null;
		if (flag12)
		{
			foreach (SpouseAndChildren spouse in genealogy.Spouses)
			{
				GenealogyMaker.GenealogyNode spouseNode = (spouse.SpouseCharId >= 0) ? new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Spouses, spouse.SpouseCharId, 1024, null) : null;
				bool flag13 = spouseNode != null;
				if (flag13)
				{
					spouseNode.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(self, 1024));
					GenealogyMaker.AddNodeToMain(spouseNode, self);
					listSpouses.Add(spouseNode);
				}
				bool flag14 = spouse.Children != null;
				if (flag14)
				{
					foreach (CharIdAndRelation charIdAndRelation3 in spouse.Children)
					{
						GenealogyMaker.GenealogyNode child = new GenealogyMaker.GenealogyNode(ViewCharacterMenuGenealogy.EGeneration.Children, charIdAndRelation3.CharId, charIdAndRelation3.RelationType, null);
						bool flag15 = spouseNode == null;
						if (flag15)
						{
							child.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(self, charIdAndRelation3.RelationType));
							GenealogyMaker.AddNodeToMain(child, self);
						}
						else
						{
							child.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(spouseNode, charIdAndRelation3.RelationType));
							GenealogyMaker.AddNodeToMain(child, spouseNode);
						}
						listChildren.Add(child);
					}
				}
				GenealogyMaker.<MakeNodes>g__FillSpousesAndChildrenList|1_0(spouse.BloodChildrenSpouses, 2, spouseNode, listChildren, listGrandChildren, ViewCharacterMenuGenealogy.EGeneration.Children, ViewCharacterMenuGenealogy.EGeneration.GrandChildren);
			}
		}
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.Spouses, listSpouses);
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.Children, listChildren);
		dictionary.Add(ViewCharacterMenuGenealogy.EGeneration.GrandChildren, listGrandChildren);
		return dictionary;
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000DDA54 File Offset: 0x000DBC54
	private static void AddNodeToMain(GenealogyMaker.GenealogyNode baseNode, GenealogyMaker.GenealogyNode highlightNode)
	{
		bool flag = baseNode == null || highlightNode == null;
		if (!flag)
		{
			baseNode.TryAddNodeToMain(highlightNode);
		}
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x000DDA7A File Offset: 0x000DBC7A
	public static IEnumerator SquaredAway(IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> nodes)
	{
		Stopwatch watch = new Stopwatch();
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		watch.Start();
		bool flag = !GenealogyMaker.InitialCompactLayout(nodes, watch, 16);
		if (flag)
		{
			yield break;
		}
		int num;
		for (int iteration = 0; iteration < 2; iteration = num + 1)
		{
			bool flag2 = !GenealogyMaker.SmartBalanceAdjustment(nodes, watch, 16);
			if (flag2)
			{
				break;
			}
			bool flag3 = watch.ElapsedMilliseconds > 16L;
			if (flag3)
			{
				yield return wait;
				watch.Restart();
			}
			num = iteration;
		}
		GenealogyMaker.FinalFineTuning(nodes);
		watch.Stop();
		yield break;
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000DDA8C File Offset: 0x000DBC8C
	private static bool InitialCompactLayout(IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> nodes, Stopwatch watch, int splitInterval)
	{
		bool hasChanges = false;
		foreach (ViewCharacterMenuGenealogy.EGeneration generation in from ViewCharacterMenuGenealogy.EGeneration g in Enum.GetValues(typeof(ViewCharacterMenuGenealogy.EGeneration))
		orderby (int)g
		select g)
		{
			List<GenealogyMaker.GenealogyNode> list;
			bool flag = !nodes.TryGetValue(generation, out list) || list.Count == 0;
			if (!flag)
			{
				List<GenealogyMaker.GenealogyNode> independentNodes = list.Where((GenealogyMaker.GenealogyNode n) => n.FollowNode == null).ToList<GenealogyMaker.GenealogyNode>();
				bool flag2 = independentNodes.Count == 0;
				if (!flag2)
				{
					independentNodes.Sort(delegate(GenealogyMaker.GenealogyNode a, GenealogyMaker.GenealogyNode b)
					{
						List<GenealogyMaker.GenealogyNode.GenealogyLink> dependencies = a.Dependencies;
						int depsA = (dependencies != null) ? dependencies.Count : 0;
						List<GenealogyMaker.GenealogyNode.GenealogyLink> dependencies2 = b.Dependencies;
						return ((dependencies2 != null) ? dependencies2.Count : 0).CompareTo(depsA);
					});
					HashSet<int> usedPositions = new HashSet<int>();
					foreach (GenealogyMaker.GenealogyNode node in independentNodes)
					{
						int idealPosition = 0;
						bool flag3 = node.Dependencies.Count > 0;
						if (flag3)
						{
							List<int> dependencyPositions = node.Dependencies.Select((GenealogyMaker.GenealogyNode.GenealogyLink link) => GenealogyMaker.GetGlobalOffset(link.Node)).Where((int pos) => pos != 0).ToList<int>();
							bool flag4 = dependencyPositions.Count > 0;
							if (flag4)
							{
								idealPosition = (int)Math.Round(dependencyPositions.Average());
							}
						}
						int bestPosition = GenealogyMaker.FindNearestAvailablePosition(idealPosition, usedPositions, 1);
						node.SelfOffset = bestPosition;
						usedPositions.Add(bestPosition);
						hasChanges = true;
						bool flag5 = watch.ElapsedMilliseconds > (long)splitInterval;
						if (flag5)
						{
						}
					}
					foreach (GenealogyMaker.GenealogyNode node2 in list.Where((GenealogyMaker.GenealogyNode n) => n.FollowNode != null))
					{
						node2.SelfOffset = node2.FollowNode.SelfOffset + 1;
						hasChanges = true;
					}
				}
			}
		}
		return hasChanges;
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000DDD4C File Offset: 0x000DBF4C
	private static int FindNearestAvailablePosition(int idealPosition, HashSet<int> usedPositions, int minSpacing)
	{
		bool flag = !usedPositions.Contains(idealPosition);
		int result;
		if (flag)
		{
			result = idealPosition;
		}
		else
		{
			for (int offset = 1; offset <= 10; offset++)
			{
				int rightPosition = idealPosition + offset;
				bool flag2 = !usedPositions.Contains(rightPosition) && !usedPositions.Any((int p) => Math.Abs(p - rightPosition) < minSpacing);
				if (flag2)
				{
					return rightPosition;
				}
				int leftPosition = idealPosition - offset;
				bool flag3 = !usedPositions.Contains(leftPosition) && !usedPositions.Any((int p) => Math.Abs(p - leftPosition) < minSpacing);
				if (flag3)
				{
					return leftPosition;
				}
			}
			int maxUsed = usedPositions.Any<int>() ? usedPositions.Max() : 0;
			result = maxUsed + minSpacing;
		}
		return result;
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000DDE54 File Offset: 0x000DC054
	private static bool SmartBalanceAdjustment(IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> nodes, Stopwatch watch, int splitInterval)
	{
		bool hasChanges = false;
		foreach (ViewCharacterMenuGenealogy.EGeneration generation in Enum.GetValues(typeof(ViewCharacterMenuGenealogy.EGeneration)).Cast<ViewCharacterMenuGenealogy.EGeneration>())
		{
			List<GenealogyMaker.GenealogyNode> list;
			bool flag = !nodes.TryGetValue(generation, out list) || list.Count < 2;
			if (!flag)
			{
				List<GenealogyMaker.GenealogyNode> independentNodes = (from n in list
				where n.FollowNode == null
				select n).OrderBy(new Func<GenealogyMaker.GenealogyNode, int>(GenealogyMaker.GetGlobalOffset)).ToList<GenealogyMaker.GenealogyNode>();
				for (int i = 0; i < independentNodes.Count - 1; i++)
				{
					GenealogyMaker.GenealogyNode currentNode = independentNodes[i];
					GenealogyMaker.GenealogyNode nextNode = independentNodes[i + 1];
					int currentPos = GenealogyMaker.GetGlobalOffset(currentNode);
					int nextPos = GenealogyMaker.GetGlobalOffset(nextNode);
					int spacing = nextPos - currentPos;
					bool flag2 = spacing > 1;
					if (flag2)
					{
						bool flag3 = GenealogyMaker.CanSafelyMoveRight(currentNode, independentNodes) && GenealogyMaker.CanSafelyMoveLeft(nextNode, independentNodes);
						if (flag3)
						{
							int moveDistance = (spacing - 1) / 2;
							bool flag4 = moveDistance > 0;
							if (flag4)
							{
								currentNode.SelfOffset += moveDistance;
								nextNode.SelfOffset -= moveDistance;
								hasChanges = true;
							}
						}
					}
					bool flag5 = watch.ElapsedMilliseconds > (long)splitInterval;
					if (flag5)
					{
					}
				}
			}
		}
		return hasChanges;
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000DDFF4 File Offset: 0x000DC1F4
	private static bool CanSafelyMoveRight(GenealogyMaker.GenealogyNode node, List<GenealogyMaker.GenealogyNode> generationNodes)
	{
		int currentPos = GenealogyMaker.GetGlobalOffset(node);
		int nodeIndex = generationNodes.IndexOf(node);
		bool flag = nodeIndex < generationNodes.Count - 1;
		bool result;
		if (flag)
		{
			GenealogyMaker.GenealogyNode nextNode = generationNodes[nodeIndex + 1];
			int nextPos = GenealogyMaker.GetGlobalOffset(nextNode);
			result = (nextPos - currentPos > 1);
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000DE048 File Offset: 0x000DC248
	private static bool CanSafelyMoveLeft(GenealogyMaker.GenealogyNode node, List<GenealogyMaker.GenealogyNode> generationNodes)
	{
		int currentPos = GenealogyMaker.GetGlobalOffset(node);
		int nodeIndex = generationNodes.IndexOf(node);
		bool flag = nodeIndex > 0;
		bool result;
		if (flag)
		{
			GenealogyMaker.GenealogyNode prevNode = generationNodes[nodeIndex - 1];
			int prevPos = GenealogyMaker.GetGlobalOffset(prevNode);
			result = (currentPos - prevPos > 1);
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000DE094 File Offset: 0x000DC294
	private static void FinalFineTuning(IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> nodes)
	{
		List<int> allOffsets = nodes.Values.SelectMany((List<GenealogyMaker.GenealogyNode> list) => list.Select(new Func<GenealogyMaker.GenealogyNode, int>(GenealogyMaker.GetGlobalOffset))).ToList<int>();
		bool flag = allOffsets.Count == 0;
		if (!flag)
		{
			int minOffset = allOffsets.Min<int>();
			int maxOffset = allOffsets.Max<int>();
			bool flag2 = maxOffset - minOffset > nodes.Values.Max((List<GenealogyMaker.GenealogyNode> list) => list.Count) * 2;
			if (flag2)
			{
				GenealogyMaker.CompressLayout(nodes);
			}
			GenealogyMaker.GenealogyNode selfNode = nodes[ViewCharacterMenuGenealogy.EGeneration.Self][0];
			Func<GenealogyMaker.GenealogyNode, bool> <>9__2;
			foreach (ViewCharacterMenuGenealogy.EGeneration tackleGeneration in new ViewCharacterMenuGenealogy.EGeneration[]
			{
				ViewCharacterMenuGenealogy.EGeneration.Siblings,
				ViewCharacterMenuGenealogy.EGeneration.Spouses
			})
			{
				List<GenealogyMaker.GenealogyNode> tackles = nodes[tackleGeneration];
				for (;;)
				{
					IEnumerable<GenealogyMaker.GenealogyNode> source = tackles;
					Func<GenealogyMaker.GenealogyNode, bool> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ((GenealogyMaker.GenealogyNode n) => n.GetOffsetFromRoot() == selfNode.GetOffsetFromRoot()));
					}
					GenealogyMaker.GenealogyNode exist = source.FirstOrDefault(predicate);
					bool flag3 = exist != null;
					if (!flag3)
					{
						break;
					}
					exist.SelfOffset++;
				}
			}
		}
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000DE1D8 File Offset: 0x000DC3D8
	private static void CompressLayout(IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> nodes)
	{
		List<GenealogyMaker.GenealogyNode> allNodes = nodes.Values.SelectMany((List<GenealogyMaker.GenealogyNode> list) => list).ToList<GenealogyMaker.GenealogyNode>();
		List<int> nodePositions = (from pos in allNodes.Select(new Func<GenealogyMaker.GenealogyNode, int>(GenealogyMaker.GetGlobalOffset))
		orderby pos
		select pos).ToList<int>();
		bool flag = nodePositions.Count < 2;
		if (!flag)
		{
			Dictionary<int, int> positionMap = new Dictionary<int, int>();
			int targetPosition = 0;
			foreach (int pos2 in nodePositions)
			{
				bool flag2 = positionMap.TryAdd(pos2, targetPosition);
				if (flag2)
				{
					targetPosition++;
				}
			}
			foreach (GenealogyMaker.GenealogyNode node in allNodes)
			{
				int originalPos = GenealogyMaker.GetGlobalOffset(node);
				int newPos;
				bool flag3 = positionMap.TryGetValue(originalPos, out newPos);
				if (flag3)
				{
					int adjustment = newPos - originalPos;
					node.SelfOffset += adjustment;
				}
			}
		}
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000DE334 File Offset: 0x000DC534
	private static int GetGlobalOffset(GenealogyMaker.GenealogyNode node)
	{
		bool flag = node.FollowNode != null;
		int result;
		if (flag)
		{
			result = GenealogyMaker.GetGlobalOffset(node.FollowNode) + 1;
		}
		else
		{
			result = node.SelfOffset;
		}
		return result;
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000DE36C File Offset: 0x000DC56C
	[CompilerGenerated]
	internal static void <MakeNodes>g__FillSpousesAndChildrenList|1_0(IReadOnlyList<SpousesAndChildren> spousesAndChildrenList, ushort relationType, GenealogyMaker.GenealogyNode spouse, List<GenealogyMaker.GenealogyNode> listChildren, List<GenealogyMaker.GenealogyNode> listGrandChildren, ViewCharacterMenuGenealogy.EGeneration generationChildren, ViewCharacterMenuGenealogy.EGeneration generationGrandChildren)
	{
		bool flag = spousesAndChildrenList == null;
		if (!flag)
		{
			using (IEnumerator<SpousesAndChildren> enumerator = spousesAndChildrenList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SpousesAndChildren spousesAndChildren = enumerator.Current;
					int idx = listChildren.FindIndex((GenealogyMaker.GenealogyNode n) => n.CharacterId == spousesAndChildren.CoreCharId);
					bool flag2 = idx >= 0;
					GenealogyMaker.GenealogyNode child;
					if (flag2)
					{
						child = listChildren[idx];
					}
					else
					{
						child = new GenealogyMaker.GenealogyNode(generationChildren, spousesAndChildren.CoreCharId, relationType, null);
						child.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(spouse, relationType));
						GenealogyMaker.AddNodeToMain(child, spouse);
						listChildren.Add(child);
					}
					bool flag3 = relationType != 2 || spousesAndChildren.Spouses == null;
					if (!flag3)
					{
						foreach (SpouseAndChildren childSpouseAndChildren in spousesAndChildren.Spouses)
						{
							bool flag4 = childSpouseAndChildren.Children == null;
							if (!flag4)
							{
								foreach (CharIdAndRelation grandChildCharIdAndRelation in childSpouseAndChildren.Children)
								{
									GenealogyMaker.GenealogyNode grandChild = new GenealogyMaker.GenealogyNode(generationGrandChildren, grandChildCharIdAndRelation.CharId, grandChildCharIdAndRelation.RelationType, null);
									grandChild.AddDependencyExceptInvalid(new GenealogyMaker.GenealogyNode.GenealogyLink(child, grandChildCharIdAndRelation.RelationType));
									GenealogyMaker.AddNodeToMain(grandChild, child);
									listGrandChildren.Add(grandChild);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x02001449 RID: 5193
	public class GenealogyNode
	{
		// Token: 0x1700165B RID: 5723
		// (get) Token: 0x0600CB5E RID: 52062 RVA: 0x00593E9D File Offset: 0x0059209D
		public ViewCharacterMenuGenealogy.EGeneration Generation
		{
			get
			{
				return this._generation;
			}
		}

		// Token: 0x0600CB5F RID: 52063 RVA: 0x00593EA8 File Offset: 0x005920A8
		public GenealogyNode(ViewCharacterMenuGenealogy.EGeneration generation, int characterId, ushort relationType, GenealogyMaker.GenealogyNode followNode = null)
		{
			this._generation = generation;
			this.CharacterId = characterId;
			this.RelationType = relationType;
			bool flag = followNode != null && followNode.CharacterId >= 0;
			if (flag)
			{
				this.FollowNode = this.GetTrulyFollowNode(followNode);
			}
		}

		// Token: 0x0600CB60 RID: 52064 RVA: 0x00593F08 File Offset: 0x00592108
		private GenealogyMaker.GenealogyNode GetTrulyFollowNode(GenealogyMaker.GenealogyNode followNode)
		{
			while (followNode._beFollowed != null)
			{
				followNode = followNode._beFollowed;
			}
			followNode._beFollowed = this;
			return followNode;
		}

		// Token: 0x0600CB61 RID: 52065 RVA: 0x00593F3C File Offset: 0x0059213C
		public void AddDependencyExceptInvalid(GenealogyMaker.GenealogyNode.GenealogyLink link)
		{
			GenealogyMaker.GenealogyNode dependency = link.Node;
			bool flag = dependency == null || dependency.CharacterId < 0 || this._generation == dependency._generation;
			if (!flag)
			{
				Tester.Assert(dependency != this._beFollowed, "");
				this.Dependencies.Add(link);
			}
		}

		// Token: 0x0600CB62 RID: 52066 RVA: 0x00593F98 File Offset: 0x00592198
		public int GetOffsetFromRoot()
		{
			bool flag = this.FollowNode != null;
			int result;
			if (flag)
			{
				result = this.FollowNode.GetOffsetFromRoot() + 1;
			}
			else
			{
				int offset = this.SelfOffset;
				bool flag2 = this.Dependencies.Count > 0;
				if (flag2)
				{
					offset += (from d in this.Dependencies
					select d.Node.GetOffsetFromRoot()).Min();
				}
				result = offset;
			}
			return result;
		}

		// Token: 0x0600CB63 RID: 52067 RVA: 0x00594018 File Offset: 0x00592218
		public bool TryAddNodeToMain(GenealogyMaker.GenealogyNode node)
		{
			bool flag = this.NodeToMain != null || node == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Debug.Log(string.Format("Test {0} TryAddNodeToMain:{1}", this.CharacterId, node.CharacterId));
				this.NodeToMain = node;
				result = true;
			}
			return result;
		}

		// Token: 0x0400A068 RID: 41064
		private readonly ViewCharacterMenuGenealogy.EGeneration _generation;

		// Token: 0x0400A069 RID: 41065
		public readonly int CharacterId;

		// Token: 0x0400A06A RID: 41066
		public readonly ushort RelationType;

		// Token: 0x0400A06B RID: 41067
		public int SelfOffset;

		// Token: 0x0400A06C RID: 41068
		public readonly GenealogyMaker.GenealogyNode FollowNode;

		// Token: 0x0400A06D RID: 41069
		public GenealogyMaker.GenealogyNode NodeToMain;

		// Token: 0x0400A06E RID: 41070
		public readonly List<GenealogyMaker.GenealogyNode.GenealogyLink> Dependencies = new List<GenealogyMaker.GenealogyNode.GenealogyLink>();

		// Token: 0x0400A06F RID: 41071
		private GenealogyMaker.GenealogyNode _beFollowed;

		// Token: 0x020026B7 RID: 9911
		public readonly struct GenealogyLink
		{
			// Token: 0x06011C3E RID: 72766 RVA: 0x00689B68 File Offset: 0x00687D68
			public GenealogyLink(GenealogyMaker.GenealogyNode node, ushort type)
			{
				this.Node = node;
				this.Type = type;
			}

			// Token: 0x0400EB29 RID: 60201
			public readonly GenealogyMaker.GenealogyNode Node;

			// Token: 0x0400EB2A RID: 60202
			public readonly ushort Type;
		}
	}
}
