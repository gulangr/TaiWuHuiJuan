using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BetterSearch.Contracts;
using BetterSearchFrontend.Ui;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterSearchFrontend
{
	// Token: 0x02000009 RID: 9
	internal sealed class TaiwuVillagersSearchScopePatch : BaseFrontPatch
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002356 File Offset: 0x00000556
		public override void OnModSettingUpdate(string modIdStr)
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002358 File Offset: 0x00000558
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewTaiwuVillagers), "Awake")]
		private static void ViewTaiwuVillagers_Awake_Postfix(ViewTaiwuVillagers __instance)
		{
			TaiwuVillagersSearchScopePatch.ScheduleEnsureScopeDropdown(__instance);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002360 File Offset: 0x00000560
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewTaiwuVillagers), "OnEnable")]
		private static void ViewTaiwuVillagers_OnEnable_Postfix(ViewTaiwuVillagers __instance)
		{
			TaiwuVillagersSearchScopePatch.ScheduleEnsureScopeDropdown(__instance);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002368 File Offset: 0x00000568
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewTaiwuVillagers), "OnInit")]
		private static void ViewTaiwuVillagers_OnInit_Postfix(ViewTaiwuVillagers __instance)
		{
			TaiwuVillagersSearchScopePatch.ViewState state = TaiwuVillagersSearchScopePatch.GetState(__instance);
			List<int> list = TaiwuVillagersSearchScopePatch.IdsField.GetValue(__instance) as List<int>;
			state.Scope = ((list == null) ? SearchScope.TaiwuVillagers : SearchScope.CurrentBlock);
			state.ScopeSelected = false;
			state.CachedQueryKey = string.Empty;
			state.CachedResult = null;
			state.PendingQueryKey = string.Empty;
			CDropdown cdropdown = TaiwuVillagersSearchScopePatch.FindScopeDropdown(__instance);
			if (cdropdown != null)
			{
				cdropdown.SetValueWithoutNotify((int)state.Scope);
			}
			VillagerPanelEarlyOpenPatch.ApplyExpelControlsVisibility(__instance, state.Scope == SearchScope.TaiwuVillagers);
			TaiwuVillagersSearchScopePatch.ScheduleEnsureScopeDropdown(__instance);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000023EF File Offset: 0x000005EF
		[HarmonyPrefix]
		[HarmonyPatch(typeof(ViewTaiwuVillagers), "ShowDropDownMenu")]
		private static void ViewTaiwuVillagers_ShowDropDownMenu_Prefix(ViewTaiwuVillagers __instance, out bool __state)
		{
			__state = false;
			if (TaiwuVillagersSearchScopePatch.IdsField.GetValue(__instance) != null)
			{
				return;
			}
			if (TaiwuVillagersSearchScopePatch.GetState(__instance).Scope == SearchScope.TaiwuVillagers)
			{
				return;
			}
			TaiwuVillagersSearchScopePatch.IdsField.SetValue(__instance, TaiwuVillagersSearchScopePatch.ExpelSuppressSentinel);
			__state = true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002424 File Offset: 0x00000624
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewTaiwuVillagers), "ShowDropDownMenu")]
		private static void ViewTaiwuVillagers_ShowDropDownMenu_Postfix(ViewTaiwuVillagers __instance, bool __state)
		{
			if (__state)
			{
				TaiwuVillagersSearchScopePatch.IdsField.SetValue(__instance, null);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002438 File Offset: 0x00000638
		[HarmonyPrefix]
		[HarmonyPatch(typeof(ViewTaiwuVillagers), "RefreshListData")]
		private static bool ViewTaiwuVillagers_RefreshListData_Prefix(ViewTaiwuVillagers __instance)
		{
			TaiwuVillagersSearchScopePatch.ScheduleEnsureScopeDropdown(__instance);
			TaiwuVillagersSearchScopePatch.ViewState state = TaiwuVillagersSearchScopePatch.GetState(__instance);
			TMP_InputField searchingField = TaiwuVillagersSearchScopePatch.GetSearchingField(__instance);
			string text = ((searchingField != null) ? searchingField.text : string.Empty).Trim();
			if (text.Length == 0 && !state.ScopeSelected)
			{
				state.CachedQueryKey = string.Empty;
				state.CachedResult = null;
				state.PendingQueryKey = string.Empty;
				return true;
			}
			Location searchAnchorLocation = TaiwuVillagersSearchScopePatch.GetSearchAnchorLocation(__instance);
			string text2 = TaiwuVillagersSearchScopePatch.BuildQueryKey(state.Scope, searchAnchorLocation, text);
			TaiwuVillagersSearchScopePatch.IssueScopedSearch(__instance, state, text2, searchAnchorLocation, text);
			if (state.CachedResult != null && state.CachedQueryKey == text2)
			{
				TaiwuVillagersSearchScopePatch.ApplyMatchedCharacters(__instance, TaiwuVillagersSearchScopePatch.BuildSearchResultDisplayList(__instance, state.CachedResult));
				return false;
			}
			List<int> list = TaiwuVillagersSearchScopePatch.IdsField.GetValue(__instance) as List<int>;
			if (state.Scope == SearchScope.CurrentBlock && list != null)
			{
				return true;
			}
			TaiwuVillagersSearchScopePatch.ApplyMatchedCharacters(__instance, (state.CachedResult != null) ? TaiwuVillagersSearchScopePatch.BuildSearchResultDisplayList(__instance, state.CachedResult) : new List<VillagerCharDisplayData>());
			return false;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002534 File Offset: 0x00000734
		private static void IssueScopedSearch(ViewTaiwuVillagers view, TaiwuVillagersSearchScopePatch.ViewState state, string queryKey, Location anchor, string keyword)
		{
			if (state.PendingQueryKey == queryKey)
			{
				return;
			}
			state.PendingQueryKey = queryKey;
			string text = SearchRequestCodec.Encode(state.Scope, anchor.AreaId, anchor.BlockId, keyword);
			SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, string>(4, 65001, new List<int>(), text, delegate(int offset, RawDataPool pool)
			{
				List<VillagerCharDisplayData> list = new List<VillagerCharDisplayData>();
				Serializer.Deserialize(pool, offset, ref list);
				if (state.PendingQueryKey == queryKey)
				{
					state.PendingQueryKey = string.Empty;
				}
				if (!TaiwuVillagersSearchScopePatch.IsViewAlive(view) || TaiwuVillagersSearchScopePatch.CurrentQueryKey(view, state) != queryKey)
				{
					return;
				}
				state.CachedQueryKey = queryKey;
				state.CachedResult = list;
				TaiwuVillagersSearchScopePatch.ApplyMatchedCharacters(view, TaiwuVillagersSearchScopePatch.BuildSearchResultDisplayList(view, list));
			});
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000025C8 File Offset: 0x000007C8
		private static string CurrentQueryKey(ViewTaiwuVillagers view, TaiwuVillagersSearchScopePatch.ViewState state)
		{
			TMP_InputField searchingField = TaiwuVillagersSearchScopePatch.GetSearchingField(view);
			string text = ((searchingField != null) ? searchingField.text : string.Empty).Trim();
			if (text.Length == 0 && !state.ScopeSelected)
			{
				return string.Empty;
			}
			return TaiwuVillagersSearchScopePatch.BuildQueryKey(state.Scope, TaiwuVillagersSearchScopePatch.GetSearchAnchorLocation(view), text);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002620 File Offset: 0x00000820
		private static string BuildQueryKey(SearchScope scope, Location location, string keyword)
		{
			switch (scope)
			{
			case SearchScope.Area:
				return "A|" + location.AreaId.ToString() + "|" + keyword;
			case SearchScope.World:
				return "W|" + keyword;
			case SearchScope.TaiwuVillagers:
				return "V|" + keyword;
			default:
				return string.Concat(new string[]
				{
					"B|",
					location.AreaId.ToString(),
					"|",
					location.BlockId.ToString(),
					"|",
					keyword
				});
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000026C0 File Offset: 0x000008C0
		private static void ScheduleEnsureScopeDropdown(ViewTaiwuVillagers view)
		{
			if (!TaiwuVillagersSearchScopePatch.IsViewAlive(view))
			{
				return;
			}
			TaiwuVillagersSearchScopePatch.EnsureScopeDropdown(view);
			if (!view.isActiveAndEnabled || !view.gameObject.activeInHierarchy)
			{
				return;
			}
			TaiwuVillagersSearchScopePatch.ViewState state = TaiwuVillagersSearchScopePatch.GetState(view);
			if (state.EnsureDropdownRunning)
			{
				return;
			}
			state.EnsureDropdownRunning = true;
			view.StartCoroutine(TaiwuVillagersSearchScopePatch.EnsureScopeDropdownWhenReady(view, state));
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002717 File Offset: 0x00000917
		private static IEnumerator EnsureScopeDropdownWhenReady(ViewTaiwuVillagers view, TaiwuVillagersSearchScopePatch.ViewState state)
		{
			try
			{
				int num;
				for (int i = 0; i < 20; i = num + 1)
				{
					if (!TaiwuVillagersSearchScopePatch.IsViewAlive(view) || !view.isActiveAndEnabled || !view.gameObject.activeInHierarchy)
					{
						yield break;
					}
					TaiwuVillagersSearchScopePatch.EnsureScopeDropdown(view);
					TMP_InputField searchingField = TaiwuVillagersSearchScopePatch.GetSearchingField(view);
					CDropdown cdropdown = TaiwuVillagersSearchScopePatch.FindScopeDropdown(view);
					if (searchingField != null && cdropdown != null)
					{
						cdropdown.gameObject.SetActive(true);
						cdropdown.transform.SetAsLastSibling();
						TaiwuVillagersSearchScopePatch.AlignDropdownToInput(searchingField, cdropdown);
					}
					yield return null;
					num = i;
				}
			}
			finally
			{
				state.EnsureDropdownRunning = false;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002730 File Offset: 0x00000930
		private static void EnsureScopeDropdown(ViewTaiwuVillagers view)
		{
			if (!TaiwuVillagersSearchScopePatch.IsViewAlive(view))
			{
				return;
			}
			TMP_InputField searchingField = TaiwuVillagersSearchScopePatch.GetSearchingField(view);
			if (searchingField == null || searchingField.transform == null)
			{
				return;
			}
			Transform transform = TaiwuVillagersSearchScopePatch.ResolveDropdownParent(view, searchingField);
			CDropdown cdropdown = TaiwuVillagersSearchScopePatch.FindScopeDropdown(view);
			if (cdropdown != null)
			{
				if (transform != null && cdropdown.transform.parent != transform)
				{
					cdropdown.transform.SetParent(transform, false);
				}
				cdropdown.gameObject.SetActive(true);
				TaiwuVillagersSearchScopePatch.BindDropdown(view, cdropdown);
				BetterSearchDropdownFactory.NormalizeScopeDropdown(cdropdown);
				TaiwuVillagersSearchScopePatch.AlignDropdownToInput(searchingField, cdropdown);
				TaiwuVillagersSearchScopePatch.ScheduleRealignDropdown(view);
				return;
			}
			CDropdown cdropdown2 = BetterSearchDropdownFactory.CreateScopeDropdown(transform, TaiwuVillagersSearchScopePatch.ScopeLabels);
			cdropdown2.name = "BetterSearchTaiwuVillagersScopeDropdown";
			cdropdown2.gameObject.name = "BetterSearchTaiwuVillagersScopeDropdown";
			cdropdown2.gameObject.SetActive(true);
			cdropdown2.SetValueWithoutNotify((int)TaiwuVillagersSearchScopePatch.GetState(view).Scope);
			TaiwuVillagersSearchScopePatch.BindDropdown(view, cdropdown2);
			TaiwuVillagersSearchScopePatch.AlignDropdownToInput(searchingField, cdropdown2);
			LayoutElement layoutElement = cdropdown2.gameObject.GetComponent<LayoutElement>();
			if (layoutElement == null)
			{
				layoutElement = cdropdown2.gameObject.AddComponent<LayoutElement>();
			}
			layoutElement.ignoreLayout = true;
			cdropdown2.transform.SetAsLastSibling();
			TaiwuVillagersSearchScopePatch.ScheduleRealignDropdown(view);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000285C File Offset: 0x00000A5C
		private static void ScheduleRealignDropdown(ViewTaiwuVillagers view)
		{
			if (view != null && view.isActiveAndEnabled && view.gameObject.activeInHierarchy)
			{
				view.StartCoroutine(TaiwuVillagersSearchScopePatch.RealignDropdownAfterLayout(view));
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000288C File Offset: 0x00000A8C
		private static void BindDropdown(ViewTaiwuVillagers view, CDropdown dropdown)
		{
			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onSelect.RemoveAllListeners();
			dropdown.onSelect.AddListener(delegate(int value)
			{
				TaiwuVillagersSearchScopePatch.ApplyDropdownSelection(view, value);
			});
			dropdown.onValueChanged.AddListener(delegate(int value)
			{
				TaiwuVillagersSearchScopePatch.ApplyDropdownSelection(view, value);
			});
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000028EC File Offset: 0x00000AEC
		private static void ApplyDropdownSelection(ViewTaiwuVillagers view, int value)
		{
			TaiwuVillagersSearchScopePatch.ViewState state = TaiwuVillagersSearchScopePatch.GetState(view);
			SearchScope searchScope = (SearchScope)Mathf.Clamp(value, 0, TaiwuVillagersSearchScopePatch.ScopeLabels.Count - 1);
			int frameCount = Time.frameCount;
			if (state.LastDropdownFrame == frameCount && state.LastDropdownValue == (int)searchScope)
			{
				return;
			}
			state.LastDropdownFrame = frameCount;
			state.LastDropdownValue = (int)searchScope;
			state.Scope = searchScope;
			state.ScopeSelected = true;
			VillagerPanelEarlyOpenPatch.ApplyExpelControlsVisibility(view, searchScope == SearchScope.TaiwuVillagers);
			TaiwuVillagersSearchScopePatch.RefreshListDataMethod.Invoke(view, null);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002960 File Offset: 0x00000B60
		private static Transform ResolveDropdownParent(ViewTaiwuVillagers view, TMP_InputField input)
		{
			RectTransform rectTransform = TaiwuVillagersSearchScopePatch.ResolveSearchBoxRect(input);
			if (rectTransform != null)
			{
				return rectTransform;
			}
			if (!(view != null) || !(view.transform != null))
			{
				return input.transform.parent;
			}
			return view.transform;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000029A8 File Offset: 0x00000BA8
		private static IEnumerator RealignDropdownAfterLayout(ViewTaiwuVillagers view)
		{
			yield return null;
			TaiwuVillagersSearchScopePatch.AlignExistingDropdown(view);
			yield return null;
			TaiwuVillagersSearchScopePatch.AlignExistingDropdown(view);
			yield return null;
			TaiwuVillagersSearchScopePatch.AlignExistingDropdown(view);
			yield break;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000029B8 File Offset: 0x00000BB8
		private static void AlignExistingDropdown(ViewTaiwuVillagers view)
		{
			TMP_InputField searchingField = TaiwuVillagersSearchScopePatch.GetSearchingField(view);
			CDropdown cdropdown = TaiwuVillagersSearchScopePatch.FindScopeDropdown(view);
			if (searchingField != null && cdropdown != null)
			{
				TaiwuVillagersSearchScopePatch.AlignDropdownToInput(searchingField, cdropdown);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000029EC File Offset: 0x00000BEC
		private static void AlignDropdownToInput(TMP_InputField input, CDropdown dropdown)
		{
			if (input == null || dropdown == null)
			{
				return;
			}
			RectTransform inputRect = TaiwuVillagersSearchScopePatch.ResolveSearchBoxRect(input);
			RectTransform dropdownRect = dropdown.transform as RectTransform;
			TaiwuVillagersSearchScopePatch.AlignDropdown(inputRect, dropdownRect);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A24 File Offset: 0x00000C24
		private static RectTransform ResolveSearchBoxRect(TMP_InputField input)
		{
			if (input == null)
			{
				return null;
			}
			if (input.image != null && input.image.rectTransform != null)
			{
				return input.image.rectTransform;
			}
			return input.GetComponent<RectTransform>();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002A64 File Offset: 0x00000C64
		private static void AlignDropdown(RectTransform inputRect, RectTransform dropdownRect)
		{
			if (inputRect == null || dropdownRect == null)
			{
				return;
			}
			float num = TaiwuVillagersSearchScopePatch.ResolveHeight(inputRect, 42f);
			dropdownRect.pivot = new Vector2(0f, 0.5f);
			dropdownRect.localScale = Vector3.one;
			dropdownRect.localRotation = Quaternion.identity;
			dropdownRect.sizeDelta = new Vector2(128f, Mathf.Max(34f, num));
			if (dropdownRect.parent == inputRect)
			{
				dropdownRect.anchorMin = new Vector2(1f, 0.5f);
				dropdownRect.anchorMax = new Vector2(1f, 0.5f);
				dropdownRect.anchoredPosition = new Vector2(8f, 0f);
				return;
			}
			RectTransform rectTransform = dropdownRect.parent as RectTransform;
			if (rectTransform != null)
			{
				dropdownRect.anchorMin = rectTransform.pivot;
				dropdownRect.anchorMax = rectTransform.pivot;
				Camera uiCamera = UIManager.Instance.UiCamera;
				Vector3[] array = new Vector3[4];
				inputRect.GetWorldCorners(array);
				Vector3 vector = (array[2] + array[3]) * 0.5f;
				Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(uiCamera, vector + inputRect.right * 8f);
				Vector2 vector3;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, vector2, uiCamera, ref vector3))
				{
					if (vector3.x + 128f > rectTransform.rect.xMax)
					{
						Vector3 vector4 = (array[0] + array[1]) * 0.5f;
						Vector2 vector5 = RectTransformUtility.WorldToScreenPoint(uiCamera, vector4 - inputRect.right * 8f);
						Vector2 anchoredPosition;
						if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, vector5, uiCamera, ref anchoredPosition))
						{
							dropdownRect.pivot = new Vector2(1f, 0.5f);
							dropdownRect.anchoredPosition = anchoredPosition;
							return;
						}
					}
					dropdownRect.anchoredPosition = vector3;
				}
				return;
			}
			dropdownRect.anchorMin = new Vector2(1f, 0.5f);
			dropdownRect.anchorMax = new Vector2(1f, 0.5f);
			float num2 = TaiwuVillagersSearchScopePatch.ResolveWidth(inputRect, 390f);
			dropdownRect.anchoredPosition = inputRect.anchoredPosition + new Vector2(num2 * (1f - inputRect.pivot.x) + 8f, 0f);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002CBC File Offset: 0x00000EBC
		private static float ResolveWidth(RectTransform rectTransform, float fallback)
		{
			if (rectTransform == null)
			{
				return fallback;
			}
			float num = rectTransform.rect.width;
			if (num > 1f)
			{
				return num;
			}
			num = rectTransform.sizeDelta.x;
			if (num <= 1f)
			{
				return fallback;
			}
			return num;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002D04 File Offset: 0x00000F04
		private static float ResolveHeight(RectTransform rectTransform, float fallback)
		{
			if (rectTransform == null)
			{
				return fallback;
			}
			float num = rectTransform.rect.height;
			if (num > 1f)
			{
				return num;
			}
			num = rectTransform.sizeDelta.y;
			if (num <= 1f)
			{
				return fallback;
			}
			return num;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002D4C File Offset: 0x00000F4C
		private static List<VillagerCharDisplayData> BuildSearchResultDisplayList(ViewTaiwuVillagers view, List<VillagerCharDisplayData> displayDataList)
		{
			TaiwuVillagersSearchScopePatch.<>c__DisplayClass38_0 CS$<>8__locals1 = new TaiwuVillagersSearchScopePatch.<>c__DisplayClass38_0();
			SelectCharacterSortAndFilterController selectCharacterSortAndFilterController = TaiwuVillagersSearchScopePatch.SortAndFilterControllerField.GetValue(view) as SelectCharacterSortAndFilterController;
			Func<ISelectCharacterData, bool> func;
			if (selectCharacterSortAndFilterController == null)
			{
				func = ((ISelectCharacterData _) => true);
			}
			else
			{
				func = selectCharacterSortAndFilterController.GenerateFilter();
			}
			Func<ISelectCharacterData, bool> func2 = func;
			List<VillagerCharDisplayData> list = new List<VillagerCharDisplayData>();
			if (displayDataList == null)
			{
				return list;
			}
			foreach (VillagerCharDisplayData villagerCharDisplayData in displayDataList)
			{
				if (villagerCharDisplayData != null && func2(villagerCharDisplayData))
				{
					list.Add(villagerCharDisplayData);
				}
			}
			if (list.Count == 0 && displayDataList.Count > 0)
			{
				list.AddRange(displayDataList);
			}
			TaiwuVillagersSearchScopePatch.<>c__DisplayClass38_0 CS$<>8__locals2 = CS$<>8__locals1;
			Comparison<ISelectCharacterData> comparison;
			if (selectCharacterSortAndFilterController == null)
			{
				comparison = null;
			}
			else
			{
				comparison = selectCharacterSortAndFilterController.GenerateComparer(list.ConvertAll<ISelectCharacterData>((VillagerCharDisplayData data) => data));
			}
			CS$<>8__locals2.comparison = comparison;
			if (CS$<>8__locals1.comparison != null)
			{
				list.Sort((VillagerCharDisplayData x, VillagerCharDisplayData y) => CS$<>8__locals1.comparison(x, y));
			}
			else
			{
				list.Sort((VillagerCharDisplayData x, VillagerCharDisplayData y) => x.CharacterId.CompareTo(y.CharacterId));
			}
			return list;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002E8C File Offset: 0x0000108C
		private static void ApplyMatchedCharacters(ViewTaiwuVillagers view, List<VillagerCharDisplayData> result)
		{
			TaiwuVillagersSearchScopePatch.FilteredDataListField.SetValue(view, result);
			ListStyleGeneralScroll listStyleGeneralScroll = TaiwuVillagersSearchScopePatch.ListScrollField.GetValue(view) as ListStyleGeneralScroll;
			if (listStyleGeneralScroll != null)
			{
				listStyleGeneralScroll.SetData<VillagerCharDisplayData>(result, -1);
			}
			SelectCharacterSortAndFilterController selectCharacterSortAndFilterController = TaiwuVillagersSearchScopePatch.SortAndFilterControllerField.GetValue(view) as SelectCharacterSortAndFilterController;
			if (selectCharacterSortAndFilterController != null)
			{
				selectCharacterSortAndFilterController.SetFilteredCount(result.Count);
			}
			GameObject gameObject = TaiwuVillagersSearchScopePatch.NoContentField.GetValue(view) as GameObject;
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(result.Count == 0);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002F08 File Offset: 0x00001108
		private static CDropdown FindScopeDropdown(ViewTaiwuVillagers view)
		{
			foreach (CDropdown cdropdown in view.GetComponentsInChildren<CDropdown>(true))
			{
				if (cdropdown != null && cdropdown.gameObject.name == "BetterSearchTaiwuVillagersScopeDropdown")
				{
					return cdropdown;
				}
			}
			return null;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002F52 File Offset: 0x00001152
		private static TMP_InputField GetSearchingField(ViewTaiwuVillagers view)
		{
			return TaiwuVillagersSearchScopePatch.SearchingFieldField.GetValue(view) as TMP_InputField;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002F64 File Offset: 0x00001164
		private static Location GetCurrentLocation()
		{
			Location result;
			try
			{
				WorldMapModel instance = SingletonObject.getInstance<WorldMapModel>();
				result = ((instance != null) ? instance.CurrentLocation : Location.Invalid);
			}
			catch
			{
				result = Location.Invalid;
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002FA4 File Offset: 0x000011A4
		private static Location GetSearchAnchorLocation(ViewTaiwuVillagers view)
		{
			bool flag = TaiwuVillagersSearchScopePatch.IdsField.GetValue(view) is List<int>;
			List<VillagerCharDisplayData> list = TaiwuVillagersSearchScopePatch.DataListField.GetValue(view) as List<VillagerCharDisplayData>;
			if (flag && list != null && list.Count > 0 && list[0].Location.IsValid())
			{
				return list[0].Location;
			}
			return TaiwuVillagersSearchScopePatch.GetCurrentLocation();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003005 File Offset: 0x00001205
		private static TaiwuVillagersSearchScopePatch.ViewState GetState(ViewTaiwuVillagers view)
		{
			return TaiwuVillagersSearchScopePatch.StateByView.GetValue(view, (ViewTaiwuVillagers _) => new TaiwuVillagersSearchScopePatch.ViewState());
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003031 File Offset: 0x00001231
		private static bool IsViewAlive(ViewTaiwuVillagers view)
		{
			return view != null && view.gameObject != null;
		}

		// Token: 0x04000010 RID: 16
		private const string DropdownName = "BetterSearchTaiwuVillagersScopeDropdown";

		// Token: 0x04000011 RID: 17
		private const float DropdownGap = 8f;

		// Token: 0x04000012 RID: 18
		private const int EnsureRetryFrames = 20;

		// Token: 0x04000013 RID: 19
		private static readonly List<string> ScopeLabels = new List<string>
		{
			"当前格子",
			"当前区域",
			"全世界",
			"太吾村民"
		};

		// Token: 0x04000014 RID: 20
		private static readonly ConditionalWeakTable<ViewTaiwuVillagers, TaiwuVillagersSearchScopePatch.ViewState> StateByView = new ConditionalWeakTable<ViewTaiwuVillagers, TaiwuVillagersSearchScopePatch.ViewState>();

		// Token: 0x04000015 RID: 21
		private static readonly FieldInfo SearchingFieldField = AccessTools.Field(typeof(ViewTaiwuVillagers), "searchingField");

		// Token: 0x04000016 RID: 22
		private static readonly FieldInfo IdsField = AccessTools.Field(typeof(ViewTaiwuVillagers), "_ids");

		// Token: 0x04000017 RID: 23
		private static readonly FieldInfo DataListField = AccessTools.Field(typeof(ViewTaiwuVillagers), "_dataList");

		// Token: 0x04000018 RID: 24
		private static readonly FieldInfo FilteredDataListField = AccessTools.Field(typeof(ViewTaiwuVillagers), "_filteredDataList");

		// Token: 0x04000019 RID: 25
		private static readonly FieldInfo SortAndFilterControllerField = AccessTools.Field(typeof(ViewTaiwuVillagers), "_sortAndFilterController");

		// Token: 0x0400001A RID: 26
		private static readonly FieldInfo ListScrollField = AccessTools.Field(typeof(ViewTaiwuVillagers), "listScroll");

		// Token: 0x0400001B RID: 27
		private static readonly FieldInfo NoContentField = AccessTools.Field(typeof(ViewTaiwuVillagers), "noContent");

		// Token: 0x0400001C RID: 28
		private static readonly MethodInfo RefreshListDataMethod = AccessTools.Method(typeof(ViewTaiwuVillagers), "RefreshListData", null, null);

		// Token: 0x0400001D RID: 29
		private static readonly List<int> ExpelSuppressSentinel = new List<int>();

		// Token: 0x02000010 RID: 16
		private sealed class ViewState
		{
			// Token: 0x04000046 RID: 70
			public SearchScope Scope;

			// Token: 0x04000047 RID: 71
			public bool ScopeSelected;

			// Token: 0x04000048 RID: 72
			public int LastDropdownFrame = -1;

			// Token: 0x04000049 RID: 73
			public int LastDropdownValue = -1;

			// Token: 0x0400004A RID: 74
			public bool EnsureDropdownRunning;

			// Token: 0x0400004B RID: 75
			public string CachedQueryKey = string.Empty;

			// Token: 0x0400004C RID: 76
			public string PendingQueryKey = string.Empty;

			// Token: 0x0400004D RID: 77
			public List<VillagerCharDisplayData> CachedResult;
		}
	}
}
