using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BetterSearch.Contracts;
using Config;
using GameData.Common;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;

namespace BetterSearchBackend
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class CharacterSearchPatch : BaseBackendPatch
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002336 File Offset: 0x00000536
		public override void OnModSettingUpdate(string modIdStr)
		{
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002338 File Offset: 0x00000538
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CharacterDomain), "FindNameInCurrentSortFilter")]
		public static bool CharacterDomain_FindNameInCurrentSortFilter_Prefix(CharacterDomain __instance, string name, ref CharacterList __result)
		{
			string text = (name ?? string.Empty).Trim();
			if (text.Length == 0)
			{
				return true;
			}
			SearchRequest request;
			if (SearchRequestCodec.TryDecode(text, out request))
			{
				return CharacterSearchPatch.RunScopedSearch(__instance, request, ref __result);
			}
			FieldInfo characterSortFilterField = CharacterSearchPatch.CharacterSortFilterField;
			CharacterSortFilter characterSortFilter = ((characterSortFilterField != null) ? characterSortFilterField.GetValue(__instance) : null) as CharacterSortFilter;
			if (characterSortFilter == null)
			{
				__result = default(CharacterList);
				return false;
			}
			CharacterList characterList = default(CharacterList);
			CharacterList characterList2 = default(CharacterList);
			bool result;
			try
			{
				characterList = characterSortFilter.FindByName(text);
				characterList2 = characterSortFilter.GetCharacterList(characterSortFilter.Settings.FilterSubId);
				HashSet<int> hashSet = new HashSet<int>(characterList.GetCollection());
				CharacterList characterList3 = default(CharacterList);
				foreach (int num in characterList2.GetCollection())
				{
					if (hashSet.Contains(num) || CharacterSearchPatch.CharacterMatchesKeyword(num, text))
					{
						characterList3.Add(num);
					}
				}
				__result = characterList3;
				result = false;
			}
			catch (Exception ex)
			{
				string str = "BetterSearch item search failed, fallback to vanilla name search: ";
				Exception ex2 = ex;
				AdaptableLog.Warning(str + ((ex2 != null) ? ex2.ToString() : null), false);
				result = true;
			}
			finally
			{
				characterList.Clear();
				characterList2.Clear();
			}
			return result;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002498 File Offset: 0x00000698
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CharacterDomain), "CallMethod")]
		public static bool CharacterDomain_CallMethod_Prefix(CharacterDomain __instance, Operation operation, RawDataPool argDataPool, RawDataPool returnDataPool, DataContext context, ref int __result)
		{
			bool flag = operation.MethodId == 65002;
			if (operation.MethodId != 65001 && !flag)
			{
				return true;
			}
			bool result;
			try
			{
				int num = operation.ArgsOffset;
				List<int> candidateIds = null;
				num += Serializer.Deserialize(argDataPool, num, ref candidateIds);
				string text = null;
				Serializer.Deserialize(argDataPool, num, ref text);
				SearchRequest request;
				bool flag2 = SearchRequestCodec.TryDecode(text, out request);
				if (!flag2)
				{
					AdaptableLog.Warning("BetterSearch backend request decode failed. encodedLength=" + ((text != null) ? text.Length : 0).ToString(), false);
				}
				List<int> list = flag2 ? CharacterSearchPatch.FindScopedMatches(__instance, candidateIds, request) : new List<int>();
				if (flag)
				{
					__result = Serializer.Serialize(list, returnDataPool);
					result = false;
				}
				else
				{
					List<VillagerCharDisplayData> charDisplayDataListAsVillager = __instance.GetCharDisplayDataListAsVillager(context, list);
					__result = Serializer.Serialize(charDisplayDataListAsVillager, returnDataPool);
					result = false;
				}
			}
			catch (Exception ex)
			{
				string str = "BetterSearch character table search failed: ";
				Exception ex2 = ex;
				AdaptableLog.Warning(str + ((ex2 != null) ? ex2.ToString() : null), false);
				__result = (flag ? Serializer.Serialize(new List<int>(), returnDataPool) : Serializer.Serialize(new List<VillagerCharDisplayData>(), returnDataPool));
				result = false;
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000025B0 File Offset: 0x000007B0
		private static List<int> FindScopedMatches(CharacterDomain domain, [Nullable(2)] List<int> candidateIds, SearchRequest request)
		{
			List<int> result = new List<int>();
			string keyword = (request.Keyword ?? string.Empty).Trim();
			if (request.Scope == SearchScope.TaiwuVillagers)
			{
				CharacterSearchPatch.AppendVillagerMatches(result, keyword);
				return result;
			}
			if (candidateIds != null && candidateIds.Count > 0)
			{
				foreach (int num in candidateIds)
				{
					Character character;
					if (DomainManager.Character.TryGetElement_Objects(num, ref character) && character != null)
					{
						CharacterSearchPatch.AddMatchIfNeeded(result, num, character, request, keyword);
					}
				}
				return result;
			}
			FieldInfo characterObjectsField = CharacterSearchPatch.CharacterObjectsField;
			Dictionary<int, Character> dictionary = ((characterObjectsField != null) ? characterObjectsField.GetValue(domain) : null) as Dictionary<int, Character>;
			if (dictionary == null)
			{
				return result;
			}
			foreach (KeyValuePair<int, Character> keyValuePair in dictionary)
			{
				CharacterSearchPatch.AddMatchIfNeeded(result, keyValuePair.Key, keyValuePair.Value, request, keyword);
			}
			return result;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000026C0 File Offset: 0x000008C0
		private static void AppendVillagerMatches(List<int> result, string keyword)
		{
			try
			{
				List<int> list = new List<int>();
				DomainManager.Taiwu.TaiwuVillage.GetMembers().GetAllMembers(list);
				int taiwuCharId = DomainManager.Taiwu.GetTaiwuCharId();
				foreach (int num in list)
				{
					Character character;
					if (num != taiwuCharId && DomainManager.Character.TryGetElement_Objects(num, ref character) && character != null && (keyword.Length == 0 || CharacterSearchPatch.CharacterMatchesKeyword(num, character, keyword)))
					{
						result.Add(num);
					}
				}
			}
			catch (Exception ex)
			{
				AdaptableLog.Warning("BetterSearch villager-roster enumeration failed: " + ex.Message, false);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002788 File Offset: 0x00000988
		private static void AddMatchIfNeeded(List<int> result, int charId, Character character, SearchRequest request, string keyword)
		{
			try
			{
				if (character != null && CharacterSearchPatch.IsInScope(character, request))
				{
					if (keyword.Length == 0 || CharacterSearchPatch.CharacterMatchesKeyword(charId, character, keyword))
					{
						result.Add(charId);
					}
				}
			}
			catch (Exception ex)
			{
				AdaptableLog.Warning("BetterSearch skipped character " + charId.ToString() + ": " + ex.Message, false);
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000027F8 File Offset: 0x000009F8
		private static bool RunScopedSearch(CharacterDomain domain, SearchRequest request, ref CharacterList result)
		{
			string text = (request.Keyword ?? string.Empty).Trim();
			if (text.Length == 0)
			{
				result = default(CharacterList);
				return false;
			}
			bool result2;
			try
			{
				if (request.Scope == SearchScope.TaiwuVillagers)
				{
					List<int> list = new List<int>();
					CharacterSearchPatch.AppendVillagerMatches(list, text);
					list.Sort();
					CharacterList characterList = default(CharacterList);
					foreach (int num in list)
					{
						characterList.Add(num);
					}
					result = characterList;
					result2 = false;
				}
				else
				{
					FieldInfo characterObjectsField = CharacterSearchPatch.CharacterObjectsField;
					Dictionary<int, Character> dictionary = ((characterObjectsField != null) ? characterObjectsField.GetValue(domain) : null) as Dictionary<int, Character>;
					if (dictionary == null)
					{
						FieldInfo characterSortFilterField = CharacterSearchPatch.CharacterSortFilterField;
						CharacterSortFilter characterSortFilter = ((characterSortFilterField != null) ? characterSortFilterField.GetValue(domain) : null) as CharacterSortFilter;
						result = ((characterSortFilter != null) ? CharacterSearchPatch.SearchCurrentSortFilter(characterSortFilter, text) : default(CharacterList));
						result2 = false;
					}
					else
					{
						List<int> list2 = new List<int>();
						foreach (KeyValuePair<int, Character> keyValuePair in dictionary)
						{
							Character value = keyValuePair.Value;
							if (value != null && CharacterSearchPatch.IsInScope(value, request))
							{
								int key = keyValuePair.Key;
								if (CharacterSearchPatch.CharacterMatchesKeyword(key, value, text))
								{
									list2.Add(key);
								}
							}
						}
						list2.Sort();
						CharacterList characterList2 = default(CharacterList);
						foreach (int num2 in list2)
						{
							characterList2.Add(num2);
						}
						result = characterList2;
						result2 = false;
					}
				}
			}
			catch (Exception ex)
			{
				string str = "BetterSearch scoped search failed, fallback to vanilla name search: ";
				Exception ex2 = ex;
				AdaptableLog.Warning(str + ((ex2 != null) ? ex2.ToString() : null), false);
				result2 = true;
			}
			return result2;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002A30 File Offset: 0x00000C30
		private static CharacterList SearchCurrentSortFilter(CharacterSortFilter sortFilter, string keyword)
		{
			CharacterList characterList = default(CharacterList);
			CharacterList characterList2 = default(CharacterList);
			CharacterList result;
			try
			{
				characterList = sortFilter.FindByName(keyword);
				characterList2 = sortFilter.GetCharacterList(sortFilter.Settings.FilterSubId);
				HashSet<int> hashSet = new HashSet<int>(characterList.GetCollection());
				CharacterList characterList3 = default(CharacterList);
				foreach (int num in characterList2.GetCollection())
				{
					if (hashSet.Contains(num) || CharacterSearchPatch.CharacterMatchesKeyword(num, keyword))
					{
						characterList3.Add(num);
					}
				}
				result = characterList3;
			}
			finally
			{
				characterList.Clear();
				characterList2.Clear();
			}
			return result;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002B00 File Offset: 0x00000D00
		private static bool IsInScope(Character character, SearchRequest request)
		{
			Location location = character.GetLocation();
			if (!location.IsValid())
			{
				location = character.GetValidLocation();
			}
			if (!location.IsValid())
			{
				return false;
			}
			if (request.Scope == SearchScope.World)
			{
				return true;
			}
			if (request.Scope == SearchScope.Area)
			{
				return location.AreaId == request.AreaId;
			}
			return location.AreaId == request.AreaId && location.BlockId == request.BlockId;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002B70 File Offset: 0x00000D70
		private static bool CharacterNameContains(int charId, string keyword)
		{
			bool result;
			try
			{
				string name = DomainManager.Character.GetName(charId, false);
				result = (!string.IsNullOrEmpty(name) && name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002BBC File Offset: 0x00000DBC
		private static bool CharacterMatchesKeyword(int charId, string keyword)
		{
			Character character;
			return CharacterSearchPatch.CharacterNameContains(charId, keyword) || (DomainManager.Character.TryGetElement_Objects(charId, ref character) && character != null && (CharacterSearchPatch.CharacterHasMatchingItem(character, keyword) || CharacterSearchPatch.CharacterHasMatchingFeature(character, keyword)));
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002BFA File Offset: 0x00000DFA
		private static bool CharacterMatchesKeyword(int charId, Character character, string keyword)
		{
			return CharacterSearchPatch.CharacterNameContains(charId, keyword) || CharacterSearchPatch.CharacterHasMatchingItem(character, keyword) || CharacterSearchPatch.CharacterHasMatchingFeature(character, keyword);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002C18 File Offset: 0x00000E18
		private static bool CharacterHasMatchingItem(int charId, string keyword)
		{
			Character character;
			if (!DomainManager.Character.TryGetElement_Objects(charId, ref character) || character == null)
			{
				return false;
			}
			Inventory inventory = character.GetInventory();
			if (((inventory != null) ? inventory.Items : null) != null)
			{
				using (Dictionary<ItemKey, int>.KeyCollection.Enumerator enumerator = inventory.Items.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CharacterSearchPatch.ItemNameContains(enumerator.Current, keyword))
						{
							return true;
						}
					}
				}
			}
			ItemKey[] equipment = character.GetEquipment();
			if (equipment == null)
			{
				return false;
			}
			ItemKey[] array = equipment;
			for (int i = 0; i < array.Length; i++)
			{
				if (CharacterSearchPatch.ItemNameContains(array[i], keyword))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002CD8 File Offset: 0x00000ED8
		private static bool CharacterHasMatchingItem(Character character, string keyword)
		{
			Inventory inventory = character.GetInventory();
			if (((inventory != null) ? inventory.Items : null) != null)
			{
				using (Dictionary<ItemKey, int>.KeyCollection.Enumerator enumerator = inventory.Items.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CharacterSearchPatch.ItemNameContains(enumerator.Current, keyword))
						{
							return true;
						}
					}
				}
			}
			ItemKey[] equipment = character.GetEquipment();
			if (equipment == null)
			{
				return false;
			}
			ItemKey[] array = equipment;
			for (int i = 0; i < array.Length; i++)
			{
				if (CharacterSearchPatch.ItemNameContains(array[i], keyword))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002D84 File Offset: 0x00000F84
		private static bool CharacterHasMatchingFeature(Character character, string keyword)
		{
			List<short> featureIds = character.GetFeatureIds();
			if (featureIds == null)
			{
				return false;
			}
			foreach (short featureId in featureIds)
			{
				if (CharacterSearchPatch.FeatureNameContains(character, featureId, keyword))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002DE8 File Offset: 0x00000FE8
		private static bool FeatureNameContains(Character character, short featureId, string keyword)
		{
			if (featureId < 0)
			{
				return false;
			}
			bool result;
			try
			{
				if (character.HideAndDisableFeature(featureId))
				{
					result = false;
				}
				else
				{
					CharacterFeatureItem characterFeatureItem = CharacterFeature.Instance[featureId];
					result = (characterFeatureItem != null && !characterFeatureItem.Hidden && (CharacterSearchPatch.TextContains(characterFeatureItem.Name, keyword) || CharacterSearchPatch.TextContains(characterFeatureItem.SmallVillageName, keyword)));
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002E5C File Offset: 0x0000105C
		private static bool TextContains([Nullable(2)] string text, string keyword)
		{
			return !string.IsNullOrEmpty(text) && text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002E78 File Offset: 0x00001078
		private static bool ItemNameContains(ItemKey itemKey, string keyword)
		{
			if (itemKey.ItemType < 0 || itemKey.TemplateId < 0)
			{
				return false;
			}
			bool result;
			try
			{
				string text = null;
				if (itemKey.IsValid())
				{
					ItemBase itemBase = DomainManager.Item.TryGetBaseItem(itemKey);
					text = ((itemBase != null) ? itemBase.GetName() : null);
				}
				if (string.IsNullOrEmpty(text))
				{
					text = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId);
				}
				result = (!string.IsNullOrEmpty(text) && text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0400000F RID: 15
		[Nullable(2)]
		private static readonly FieldInfo CharacterSortFilterField = AccessTools.Field(typeof(CharacterDomain), "_characterSortFilter");

		// Token: 0x04000010 RID: 16
		[Nullable(2)]
		private static readonly FieldInfo CharacterObjectsField = AccessTools.Field(typeof(CharacterDomain), "_objects");
	}
}
