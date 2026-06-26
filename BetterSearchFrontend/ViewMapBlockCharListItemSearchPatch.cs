using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BetterSearch.Contracts;
using Game.Views.MapBlockCharList;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TMPro;

namespace BetterSearchFrontend
{
	// Token: 0x0200000A RID: 10
	internal sealed class ViewMapBlockCharListItemSearchPatch : BaseFrontPatch
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00003175 File Offset: 0x00001375
		public override void OnModSettingUpdate(string modIdStr)
		{
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003177 File Offset: 0x00001377
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewMapBlockCharList), "Awake")]
		private static void ViewMapBlockCharList_Awake_Postfix(ViewMapBlockCharList __instance)
		{
			ViewMapBlockCharListItemSearchPatch.AttachInputListener(__instance);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000317F File Offset: 0x0000137F
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewMapBlockCharList), "OnInit")]
		private static void ViewMapBlockCharList_OnInit_Postfix(ViewMapBlockCharList __instance)
		{
			ViewMapBlockCharListItemSearchPatch.AttachInputListener(__instance);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003188 File Offset: 0x00001388
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewMapBlockCharList), "ReapplyFilter")]
		private static void ViewMapBlockCharList_ReapplyFilter_Postfix(ViewMapBlockCharList __instance)
		{
			try
			{
				ViewMapBlockCharListItemSearchPatch.ApplyReadyItemMatches(__instance);
			}
			catch
			{
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000031B0 File Offset: 0x000013B0
		private static void AttachInputListener(ViewMapBlockCharList view)
		{
			if (view == null)
			{
				return;
			}
			ViewMapBlockCharListItemSearchPatch.ViewState state = ViewMapBlockCharListItemSearchPatch.GetState(view);
			TMP_InputField searchInput = ViewMapBlockCharListItemSearchPatch.GetSearchInput(view);
			if (searchInput == null || state.ListenerAttached)
			{
				return;
			}
			state.ListenerAttached = true;
			searchInput.onValueChanged.AddListener(delegate(string value)
			{
				ViewMapBlockCharListItemSearchPatch.EnsureItemSearch(view);
			});
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003224 File Offset: 0x00001424
		private static void ApplyReadyItemMatches(ViewMapBlockCharList view)
		{
			if (view == null)
			{
				return;
			}
			string keyword = ViewMapBlockCharListItemSearchPatch.GetKeyword(view);
			ViewMapBlockCharListItemSearchPatch.ViewState state = ViewMapBlockCharListItemSearchPatch.GetState(view);
			MapBlockCharacterList blockData = ViewMapBlockCharListItemSearchPatch.GetBlockData(view);
			List<ValueTuple<int, int>> normalEntries = ViewMapBlockCharListItemSearchPatch.GetNormalEntries(view);
			if (keyword.Length == 0)
			{
				ViewMapBlockCharListItemSearchPatch.ClearState(state);
				return;
			}
			ViewMapBlockCharListItemSearchPatch.EnsureItemSearch(view);
			if (!state.HasResult || state.MatchedIds.Count == 0 || blockData == null || normalEntries == null)
			{
				return;
			}
			if (0 + ViewMapBlockCharListItemSearchPatch.AppendMatchesFromList(normalEntries, state.MatchedIds, 0, blockData.SpecialCharacters) + ViewMapBlockCharListItemSearchPatch.AppendMatchesFromList(normalEntries, state.MatchedIds, 1, blockData.NormalCharacters) + ViewMapBlockCharListItemSearchPatch.AppendMatchesFromList(normalEntries, state.MatchedIds, 2, blockData.InfectedCharacters) + ViewMapBlockCharListItemSearchPatch.AppendMatchesFromList(normalEntries, state.MatchedIds, 4, blockData.EnemyCharacters) > 0)
			{
				view.SetDataCount(normalEntries.Count, true);
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000032E8 File Offset: 0x000014E8
		private static void EnsureItemSearch(ViewMapBlockCharList view)
		{
			ViewMapBlockCharListItemSearchPatch.<>c__DisplayClass15_0 CS$<>8__locals1 = new ViewMapBlockCharListItemSearchPatch.<>c__DisplayClass15_0();
			CS$<>8__locals1.view = view;
			if (CS$<>8__locals1.view == null)
			{
				return;
			}
			string keyword = ViewMapBlockCharListItemSearchPatch.GetKeyword(CS$<>8__locals1.view);
			CS$<>8__locals1.state = ViewMapBlockCharListItemSearchPatch.GetState(CS$<>8__locals1.view);
			if (keyword.Length == 0)
			{
				ViewMapBlockCharListItemSearchPatch.ClearState(CS$<>8__locals1.state);
				return;
			}
			List<int> list = ViewMapBlockCharListItemSearchPatch.BuildCandidateIds(ViewMapBlockCharListItemSearchPatch.GetBlockData(CS$<>8__locals1.view));
			if (list.Count == 0)
			{
				ViewMapBlockCharListItemSearchPatch.ClearState(CS$<>8__locals1.state);
				return;
			}
			CS$<>8__locals1.queryKey = ViewMapBlockCharListItemSearchPatch.BuildQueryKey(keyword, list);
			if (CS$<>8__locals1.state.QueryKey == CS$<>8__locals1.queryKey && (CS$<>8__locals1.state.Pending || CS$<>8__locals1.state.HasResult))
			{
				return;
			}
			CS$<>8__locals1.state.QueryKey = CS$<>8__locals1.queryKey;
			CS$<>8__locals1.state.Pending = true;
			CS$<>8__locals1.state.HasResult = false;
			CS$<>8__locals1.state.MatchedIds.Clear();
			ViewMapBlockCharListItemSearchPatch.<>c__DisplayClass15_0 CS$<>8__locals2 = CS$<>8__locals1;
			ViewMapBlockCharListItemSearchPatch.ViewState state = CS$<>8__locals1.state;
			int serial = state.Serial + 1;
			state.Serial = serial;
			CS$<>8__locals2.serial = serial;
			string text = SearchRequestCodec.Encode(SearchScope.World, 0, 0, keyword);
			SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, string>(4, 65002, list, text, delegate(int offset, RawDataPool pool)
			{
				try
				{
					if (CS$<>8__locals1.serial == CS$<>8__locals1.state.Serial && !(CS$<>8__locals1.state.QueryKey != CS$<>8__locals1.queryKey))
					{
						List<int> list2 = new List<int>();
						Serializer.Deserialize(pool, offset, ref list2);
						CS$<>8__locals1.state.MatchedIds.Clear();
						foreach (int num in list2)
						{
							if (num > 0)
							{
								CS$<>8__locals1.state.MatchedIds.Add(num);
							}
						}
						CS$<>8__locals1.state.Pending = false;
						CS$<>8__locals1.state.HasResult = true;
						if (ViewMapBlockCharListItemSearchPatch.IsAlive(CS$<>8__locals1.view))
						{
							ViewMapBlockCharListItemSearchPatch.ReapplyFilterMethod.Invoke(CS$<>8__locals1.view, new object[]
							{
								false,
								false
							});
						}
					}
				}
				catch
				{
					CS$<>8__locals1.state.Pending = false;
					CS$<>8__locals1.state.HasResult = true;
					CS$<>8__locals1.state.MatchedIds.Clear();
				}
			});
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003428 File Offset: 0x00001628
		private static int AppendMatchesFromList(List<ValueTuple<int, int>> normalEntries, HashSet<int> matchedIds, int type, List<CharacterDisplayData> source)
		{
			if (source == null || source.Count == 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < source.Count; i++)
			{
				CharacterDisplayData characterDisplayData = source[i];
				if (characterDisplayData != null && matchedIds.Contains(characterDisplayData.CharacterId) && !ViewMapBlockCharListItemSearchPatch.ContainsEntry(normalEntries, type, i))
				{
					normalEntries.Add(new ValueTuple<int, int>(type, i));
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000348C File Offset: 0x0000168C
		private static bool ContainsEntry(List<ValueTuple<int, int>> normalEntries, int type, int index)
		{
			foreach (ValueTuple<int, int> valueTuple in normalEntries)
			{
				if (valueTuple.Item1 == type && valueTuple.Item2 == index)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000034EC File Offset: 0x000016EC
		private static List<int> BuildCandidateIds(MapBlockCharacterList blockData)
		{
			HashSet<int> hashSet = new HashSet<int>();
			ViewMapBlockCharListItemSearchPatch.AddCharacterIds(hashSet, (blockData != null) ? blockData.SpecialCharacters : null);
			ViewMapBlockCharListItemSearchPatch.AddCharacterIds(hashSet, (blockData != null) ? blockData.NormalCharacters : null);
			ViewMapBlockCharListItemSearchPatch.AddCharacterIds(hashSet, (blockData != null) ? blockData.InfectedCharacters : null);
			ViewMapBlockCharListItemSearchPatch.AddCharacterIds(hashSet, (blockData != null) ? blockData.EnemyCharacters : null);
			List<int> list = new List<int>(hashSet);
			list.Sort();
			return list;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003554 File Offset: 0x00001754
		private static void AddCharacterIds(HashSet<int> ids, List<CharacterDisplayData> characters)
		{
			if (characters == null)
			{
				return;
			}
			foreach (CharacterDisplayData characterDisplayData in characters)
			{
				if (characterDisplayData != null && characterDisplayData.CharacterId > 0)
				{
					ids.Add(characterDisplayData.CharacterId);
				}
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000035B8 File Offset: 0x000017B8
		private static void ClearState(ViewMapBlockCharListItemSearchPatch.ViewState state)
		{
			state.QueryKey = string.Empty;
			state.Pending = false;
			state.HasResult = false;
			state.MatchedIds.Clear();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000035DE File Offset: 0x000017DE
		private static string BuildQueryKey(string keyword, List<int> candidateIds)
		{
			return keyword + "|" + string.Join<int>(",", candidateIds);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000035F6 File Offset: 0x000017F6
		private static TMP_InputField GetSearchInput(ViewMapBlockCharList view)
		{
			return ViewMapBlockCharListItemSearchPatch.CharacterFilterField.GetValue(view) as TMP_InputField;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003608 File Offset: 0x00001808
		private static MapBlockCharacterList GetBlockData(ViewMapBlockCharList view)
		{
			return ViewMapBlockCharListItemSearchPatch.BlockDataField.GetValue(view) as MapBlockCharacterList;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000361A File Offset: 0x0000181A
		private static List<ValueTuple<int, int>> GetNormalEntries(ViewMapBlockCharList view)
		{
			return ViewMapBlockCharListItemSearchPatch.NormalEntriesField.GetValue(view) as List<ValueTuple<int, int>>;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000362C File Offset: 0x0000182C
		private static string GetKeyword(ViewMapBlockCharList view)
		{
			TMP_InputField searchInput = ViewMapBlockCharListItemSearchPatch.GetSearchInput(view);
			return ((searchInput != null) ? searchInput.text : string.Empty).Trim();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000365B File Offset: 0x0000185B
		private static ViewMapBlockCharListItemSearchPatch.ViewState GetState(ViewMapBlockCharList view)
		{
			return ViewMapBlockCharListItemSearchPatch.StateByView.GetValue(view, (ViewMapBlockCharList _) => new ViewMapBlockCharListItemSearchPatch.ViewState());
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003687 File Offset: 0x00001887
		private static bool IsAlive(ViewMapBlockCharList view)
		{
			return view != null && view.gameObject != null && view.gameObject.activeInHierarchy;
		}

		// Token: 0x0400001E RID: 30
		private const int Special = 0;

		// Token: 0x0400001F RID: 31
		private const int Normal = 1;

		// Token: 0x04000020 RID: 32
		private const int Infected = 2;

		// Token: 0x04000021 RID: 33
		private const int Enemy = 4;

		// Token: 0x04000022 RID: 34
		private static readonly ConditionalWeakTable<ViewMapBlockCharList, ViewMapBlockCharListItemSearchPatch.ViewState> StateByView = new ConditionalWeakTable<ViewMapBlockCharList, ViewMapBlockCharListItemSearchPatch.ViewState>();

		// Token: 0x04000023 RID: 35
		private static readonly FieldInfo CharacterFilterField = AccessTools.Field(typeof(ViewMapBlockCharList), "characterFilter");

		// Token: 0x04000024 RID: 36
		private static readonly FieldInfo BlockDataField = AccessTools.Field(typeof(ViewMapBlockCharList), "_blockData");

		// Token: 0x04000025 RID: 37
		private static readonly FieldInfo NormalEntriesField = AccessTools.Field(typeof(ViewMapBlockCharList), "_normal");

		// Token: 0x04000026 RID: 38
		private static readonly MethodInfo ReapplyFilterMethod = AccessTools.Method(typeof(ViewMapBlockCharList), "ReapplyFilter", null, null);

		// Token: 0x02000017 RID: 23
		private sealed class ViewState
		{
			// Token: 0x04000060 RID: 96
			public bool ListenerAttached;

			// Token: 0x04000061 RID: 97
			public string QueryKey = string.Empty;

			// Token: 0x04000062 RID: 98
			public int Serial;

			// Token: 0x04000063 RID: 99
			public bool Pending;

			// Token: 0x04000064 RID: 100
			public bool HasResult;

			// Token: 0x04000065 RID: 101
			public readonly HashSet<int> MatchedIds = new HashSet<int>();
		}
	}
}
