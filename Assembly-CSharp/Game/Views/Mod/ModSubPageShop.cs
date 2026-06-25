using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Mod
{
	// Token: 0x020008CF RID: 2255
	public class ModSubPageShop : ModSubPage
	{
		// Token: 0x17000CA8 RID: 3240
		// (get) Token: 0x06006BA8 RID: 27560 RVA: 0x0031AF82 File Offset: 0x00319182
		private int WorkshopPageItemCurrentCount
		{
			get
			{
				return (this.workshopPageSwitch.Value == this.workshopPageSwitch.MaxValue) ? (this._workshopModIdList.Count % 25) : 25;
			}
		}

		// Token: 0x06006BA9 RID: 27561 RVA: 0x0031AFAE File Offset: 0x003191AE
		public override void Init(ViewMod parentView)
		{
			base.Init(parentView);
			this.subscribeDependenceDialog.Hide();
		}

		// Token: 0x06006BAA RID: 27562 RVA: 0x0031AFC8 File Offset: 0x003191C8
		private void Awake()
		{
			this.subscribeDependenceDialog.Init();
			this.refreshButton.onClick.ResetListener(delegate()
			{
				this.RefreshPage(true);
			});
			this.tagFilterButton.onClick.ResetListener(delegate()
			{
				this.tagToggleGroupMultiSelect.gameObject.SetActive(!this.tagToggleGroupMultiSelect.gameObject.activeSelf);
			});
			this.clearTagButton.onClick.ResetListener(delegate()
			{
				this.tagToggleGroupMultiSelect.DeSelectAll(true);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
				{
					this.RefreshPage(true);
				});
			});
			this.tagsFilterMask.onClick.ResetListener(delegate()
			{
				this.tagToggleGroupMultiSelect.gameObject.SetActive(false);
			});
			this.workshopScroll.OnItemRender += this.OnWorkshopRender;
			this.workshopPageSwitch.OnValueChanged = new Action<int>(this.OnWorkshopPageValueChanged);
			this.workshopPageSwitch.SetValueAndRefresh(1);
			this._workshopSearchInputText = string.Empty;
			this.workshopSearchInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnWorkshopSearchInputValueChange));
			this.workshopSearchInputField.SetTextWithoutNotify(string.Empty);
			this.workshopSortToggleGroup.Init(-1);
			this.workshopSortToggleGroup.OnActiveIndexChange += this.OnWorkshopSortToggleChange;
			this.workshopSortToggleGroup.SetWithoutNotify(0);
			ModSubPageShop.CurWorkshopSortToggleKey = ModSubPageShop.WorkshopSortToggleKey.MostPopular;
			this.workshopTimeDropdown.ClearOptions();
			this.workshopTimeDropdown.AddOptions(this.GetTimeStringList());
			this.workshopTimeDropdown.onValueChanged.ResetListener(delegate(int value)
			{
				ModSubPageShop.CurWorkshopTimeToggleKey = (ModSubPageShop.WorkshopTimeToggleKey)value;
				this.RefreshWorkshopModList(true);
			});
			this.workshopTimeDropdown.SetValueWithoutNotify(0);
			ModSubPageShop.CurWorkshopTimeToggleKey = ModSubPageShop.WorkshopTimeToggleKey.All;
			this.tagToggleGroupMultiSelect.Init();
			this.tagToggleGroupMultiSelect.DeSelectAll(false);
			this.tagToggleGroupMultiSelect.OnActiveIndexChange += this.TagToggleGroupMultiSelectIndexChange;
		}

		// Token: 0x06006BAB RID: 27563 RVA: 0x0031B184 File Offset: 0x00319384
		private void OnDestroy()
		{
			this.workshopScroll.OnItemRender -= this.OnWorkshopRender;
			this.workshopSortToggleGroup.OnActiveIndexChange -= this.OnWorkshopSortToggleChange;
			this.tagToggleGroupMultiSelect.OnActiveIndexChange -= this.TagToggleGroupMultiSelectIndexChange;
		}

		// Token: 0x06006BAC RID: 27564 RVA: 0x0031B1DC File Offset: 0x003193DC
		private void OnEnable()
		{
			GEvent.Add(UiEvents.WorkshopModPreviewImageHasDownloaded, new GEvent.Callback(this.OnWorkshopModPreviewImageHasDownloaded));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			this.tagToggleGroupMultiSelect.gameObject.SetActive(false);
		}

		// Token: 0x06006BAD RID: 27565 RVA: 0x0031B230 File Offset: 0x00319430
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.WorkshopModPreviewImageHasDownloaded, new GEvent.Callback(this.OnWorkshopModPreviewImageHasDownloaded));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x06006BAE RID: 27566 RVA: 0x0031B267 File Offset: 0x00319467
		private void TopUiChanged(ArgumentBox argBox)
		{
			this.RefreshPage(true);
		}

		// Token: 0x06006BAF RID: 27567 RVA: 0x0031B272 File Offset: 0x00319472
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshPage(false);
		}

		// Token: 0x06006BB0 RID: 27568 RVA: 0x0031B284 File Offset: 0x00319484
		private void RefreshPage(bool forceUpdate = false)
		{
			ModSubPageShop.<>c__DisplayClass29_0 CS$<>8__locals1 = new ModSubPageShop.<>c__DisplayClass29_0();
			CS$<>8__locals1.<>4__this = this;
			int maxItemIndex = this.workshopPageSwitch.Value * 25;
			int minItemIndex = (this.workshopPageSwitch.Value - 1) * 25;
			CS$<>8__locals1.maxSteamPageIndex = Mathf.Clamp(Mathf.CeilToInt((float)maxItemIndex / 50f), 1, this._steamPageMaxCount);
			int minSteamPageIndex = Mathf.Clamp(Mathf.CeilToInt((float)minItemIndex / 50f), 1, this._steamPageMaxCount);
			CS$<>8__locals1.curWorkshopList = SteamManager.CurWorkshopList;
			bool minNeedUpdate = CS$<>8__locals1.<RefreshPage>g__CheckNeedUpdate|0(minItemIndex);
			bool maxNeedUpdate = CS$<>8__locals1.<RefreshPage>g__CheckNeedUpdate|0(maxItemIndex);
			bool needUpdate = CS$<>8__locals1.curWorkshopList == null || minNeedUpdate || maxNeedUpdate;
			bool flag = forceUpdate || needUpdate;
			if (flag)
			{
				ModSubPageShop.<>c__DisplayClass29_1 CS$<>8__locals2 = new ModSubPageShop.<>c__DisplayClass29_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				this.ParentView.ShowMask();
				CS$<>8__locals2.searchText = this.GetSearchInputValue(this.workshopSearchInputField);
				CS$<>8__locals2.searchTags = this.GetSearchTags();
				CS$<>8__locals2.startPage = minSteamPageIndex;
				CS$<>8__locals2.<RefreshPage>g__Send|2();
			}
			else
			{
				CS$<>8__locals1.<RefreshPage>g__Refresh|1(CS$<>8__locals1.curWorkshopList);
			}
		}

		// Token: 0x06006BB1 RID: 27569 RVA: 0x0031B394 File Offset: 0x00319594
		public override bool QuickHide()
		{
			bool activeSelf = this.subscribeDependenceDialog.gameObject.activeSelf;
			bool result;
			if (activeSelf)
			{
				this.subscribeDependenceDialog.Hide();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06006BB2 RID: 27570 RVA: 0x0031B3CC File Offset: 0x003195CC
		private void OnWorkshopRender(int index, GameObject go)
		{
			ModSubPageShop.<>c__DisplayClass31_0 CS$<>8__locals1 = new ModSubPageShop.<>c__DisplayClass31_0();
			CS$<>8__locals1.<>4__this = this;
			int realIndex = (this.workshopPageSwitch.Value - 1) * 25 + index;
			CS$<>8__locals1.modId = this._workshopModIdList[realIndex];
			CS$<>8__locals1.modInfo = ModManager.GetModInfo(CS$<>8__locals1.modId);
			CS$<>8__locals1.refers = go.GetComponent<WorkshopModTemplate>();
			TextMeshProUGUI titleText = CS$<>8__locals1.refers.title;
			CToggle subscribeToggle = CS$<>8__locals1.refers.subscribeToggle;
			CButton button = CS$<>8__locals1.refers.detailButton;
			bool flag = CS$<>8__locals1.modInfo == null;
			if (flag)
			{
				titleText.SetText(string.Empty, true);
				subscribeToggle.gameObject.SetActive(false);
				button.interactable = false;
				CRawImage coverImg = CS$<>8__locals1.refers.coverImage;
				coverImg.texture = this.popupModPanelLoadError0;
				coverImg.enabled = true;
				GLog.TagWarn("UI_ModPanel", string.Format("OnWorkshopRender Not Found Mod {0}", CS$<>8__locals1.modId), Array.Empty<object>());
			}
			else
			{
				CS$<>8__locals1.refers.RefreshItem(CS$<>8__locals1.modInfo);
				this.RefreshCoverImage(CS$<>8__locals1.refers.coverImage, CS$<>8__locals1.modInfo, false);
				subscribeToggle.gameObject.SetActive(true);
				subscribeToggle.SetIsOnWithoutNotify(CS$<>8__locals1.modInfo.IsSubscribed);
				CS$<>8__locals1.refers.subscribeDecoration.enabled = CS$<>8__locals1.modInfo.IsSubscribed;
				subscribeToggle.onValueChanged.ResetListener(new Action<bool>(CS$<>8__locals1.<OnWorkshopRender>g__OnClickSubscribeMod|0));
				button.interactable = true;
				button.ClearAndAddListener(new Action(CS$<>8__locals1.<OnWorkshopRender>g__OnClickWorkshopMod|1));
			}
		}

		// Token: 0x06006BB3 RID: 27571 RVA: 0x0031B56C File Offset: 0x0031976C
		private bool RefreshCoverImage(CRawImage coverImg, ModInfoWithDisplayData modInfo, bool useTempCover = false)
		{
			bool flag = modInfo == null;
			bool result;
			if (flag)
			{
				coverImg.enabled = false;
				result = false;
			}
			else
			{
				bool hasTempCover = useTempCover && null != this._tempModCoverTexture;
				Texture2D localCover = ModManager.GetModCoverTexture(modInfo.ModId);
				bool hasLocalCover = localCover != null;
				bool hasModCoverTextureCache = ModManager.HasPreviewModCoverTexture(modInfo.ModId);
				bool showCover = hasTempCover || hasLocalCover || hasModCoverTextureCache;
				coverImg.enabled = showCover;
				bool flag2 = showCover;
				if (flag2)
				{
					bool flag3 = hasTempCover;
					if (flag3)
					{
						coverImg.texture = this._tempModCoverTexture;
					}
					else
					{
						bool flag4 = hasLocalCover;
						if (flag4)
						{
							coverImg.texture = localCover;
						}
						else
						{
							bool flag5 = hasModCoverTextureCache;
							if (flag5)
							{
								coverImg.texture = ModManager.GetPreviewModCoverTexture(modInfo.ModId);
							}
						}
					}
				}
				bool flag6 = !showCover && modInfo.PreviewFileHandle != UGCHandle_t.Invalid;
				if (flag6)
				{
					SteamManager.DownloadPreviewCoverImage(modInfo);
				}
				result = coverImg.enabled;
			}
			return result;
		}

		// Token: 0x06006BB4 RID: 27572 RVA: 0x0031B650 File Offset: 0x00319850
		private void OnWorkshopModPreviewImageHasDownloaded(ArgumentBox argumentBox)
		{
			ModInfoWithDisplayData modInfo;
			bool flag = argumentBox.Get<ModInfoWithDisplayData>("ModInfo", out modInfo);
			if (flag)
			{
				int index = this._workshopModIdList.IndexOf(modInfo.ModId);
				bool flag2 = index / 25 == this.workshopPageSwitch.Value - 1;
				if (flag2)
				{
					GameObject refers = this.workshopScroll.GetActiveCell(index % 25);
					bool flag3 = refers;
					if (flag3)
					{
						this.RefreshCoverImage(refers.GetComponent<WorkshopModTemplate>().coverImage, modInfo, false);
					}
				}
			}
		}

		// Token: 0x06006BB5 RID: 27573 RVA: 0x0031B6D1 File Offset: 0x003198D1
		private void OnWorkshopPageValueChanged(int obj)
		{
			SteamManager.Clear();
			this.RefreshPage(false);
		}

		// Token: 0x06006BB6 RID: 27574 RVA: 0x0031B6E2 File Offset: 0x003198E2
		private void TagToggleGroupMultiSelectIndexChange(int arg1, int arg2)
		{
			List<ModId> curWorkshopList = SteamManager.CurWorkshopList;
			if (curWorkshopList != null)
			{
				curWorkshopList.Clear();
			}
			this.RefreshWorkshopModList(true);
		}

		// Token: 0x06006BB7 RID: 27575 RVA: 0x0031B700 File Offset: 0x00319900
		private List<string> GetSearchTags()
		{
			List<string> result = new List<string>();
			List<int> activeIndices = this.tagToggleGroupMultiSelect.GetActiveIndices();
			foreach (int index in activeIndices)
			{
				string searchTag = SteamManager.GetTagKey(SteamManager.AllTagList[index]);
				result.Add(searchTag);
			}
			return result;
		}

		// Token: 0x06006BB8 RID: 27576 RVA: 0x0031B780 File Offset: 0x00319980
		private void OnWorkshopSearchInputValueChange(string value)
		{
			bool flag = this._workshopSearchInputText == value;
			if (!flag)
			{
				this._workshopSearchInputText = value;
				SteamManager.Clear();
				this.RefreshWorkshopModList(true);
			}
		}

		// Token: 0x06006BB9 RID: 27577 RVA: 0x0031B7B5 File Offset: 0x003199B5
		private void RefreshWorkshopModList(bool forceUpdate = true)
		{
			this.RefreshPage(forceUpdate);
		}

		// Token: 0x06006BBA RID: 27578 RVA: 0x0031B7C0 File Offset: 0x003199C0
		private string GetSearchInputValue(TMP_InputField inputField)
		{
			string inputValue = inputField.text;
			CommonUtils.FixToShowAbleString(ref inputValue, inputField.textComponent.font);
			inputValue = inputValue.Replace(" ", string.Empty);
			bool flag = string.IsNullOrEmpty(inputValue);
			if (flag)
			{
				inputValue = inputValue.Substring(0, Mathf.Min(inputValue.Length, inputField.characterLimit - 1));
			}
			inputField.SetTextWithoutNotify(inputValue);
			return inputValue;
		}

		// Token: 0x17000CA9 RID: 3241
		// (get) Token: 0x06006BBB RID: 27579 RVA: 0x0031B82B File Offset: 0x00319A2B
		// (set) Token: 0x06006BBC RID: 27580 RVA: 0x0031B832 File Offset: 0x00319A32
		public static ModSubPageShop.WorkshopTimeToggleKey CurWorkshopTimeToggleKey { get; private set; }

		// Token: 0x17000CAA RID: 3242
		// (get) Token: 0x06006BBD RID: 27581 RVA: 0x0031B83A File Offset: 0x00319A3A
		// (set) Token: 0x06006BBE RID: 27582 RVA: 0x0031B841 File Offset: 0x00319A41
		public static ModSubPageShop.WorkshopSortToggleKey CurWorkshopSortToggleKey { get; private set; }

		// Token: 0x17000CAB RID: 3243
		// (get) Token: 0x06006BBF RID: 27583 RVA: 0x0031B84C File Offset: 0x00319A4C
		public static uint CurWorkshopTime
		{
			get
			{
				ModSubPageShop.WorkshopTimeToggleKey curWorkshopTimeToggleKey = ModSubPageShop.CurWorkshopTimeToggleKey;
				if (!true)
				{
				}
				uint result;
				switch (curWorkshopTimeToggleKey)
				{
				case ModSubPageShop.WorkshopTimeToggleKey.Day:
					result = 1U;
					break;
				case ModSubPageShop.WorkshopTimeToggleKey.Week:
					result = 7U;
					break;
				case ModSubPageShop.WorkshopTimeToggleKey.Month:
					result = 30U;
					break;
				case ModSubPageShop.WorkshopTimeToggleKey.Quarter:
					result = 90U;
					break;
				case ModSubPageShop.WorkshopTimeToggleKey.Year:
					result = 365U;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x06006BC0 RID: 27584 RVA: 0x0031B8A8 File Offset: 0x00319AA8
		private void OnWorkshopSortToggleChange(int newTog, int oldTog)
		{
			ModSubPageShop.CurWorkshopSortToggleKey = (ModSubPageShop.WorkshopSortToggleKey)newTog;
			this.workshopTimeDropdown.gameObject.SetActive(ModSubPageShop.CurWorkshopSortToggleKey == ModSubPageShop.WorkshopSortToggleKey.MostPopular);
			bool flag = ModSubPageShop.CurWorkshopSortToggleKey == ModSubPageShop.WorkshopSortToggleKey.MostPopular;
			if (flag)
			{
				bool flag2 = oldTog >= 0 && oldTog != newTog;
				if (flag2)
				{
					this.workshopTimeDropdown.SetValueWithoutNotify(ModSubPageShop.WorkshopTimeToggleKey.All.ToInt());
				}
			}
			this.RefreshWorkshopModList(true);
		}

		// Token: 0x06006BC1 RID: 27585 RVA: 0x0031B918 File Offset: 0x00319B18
		private List<string> GetTimeStringList()
		{
			return new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Mod_Sort_Time_Total),
				LocalStringManager.Get(LanguageKey.LK_Mod_Sort_Time_Day),
				LocalStringManager.Get(LanguageKey.LK_Mod_Sort_Time_Week),
				LocalStringManager.Get(LanguageKey.LK_Mod_Sort_Time_Month),
				LocalStringManager.Get(LanguageKey.LK_Mod_Sort_Time_Quarter),
				LocalStringManager.Get(LanguageKey.LK_Mod_Sort_Time_Year)
			};
		}

		// Token: 0x06006BC2 RID: 27586 RVA: 0x0031B997 File Offset: 0x00319B97
		private void DoSubscribeItem(ModId modId)
		{
			ModManager.SubscribeItem(modId, true);
			this.DownloadMod(modId);
		}

		// Token: 0x06006BC3 RID: 27587 RVA: 0x0031B9AC File Offset: 0x00319BAC
		private void DownloadMod(ModId modId)
		{
			PublishedFileId_t publishedFileId = new PublishedFileId_t(modId.FileId);
			SteamUGC.DownloadItem(publishedFileId, true);
			this._downLoadingItemList.Add(modId);
			base.StopCoroutine(this.CorUpdateDownloadingMod());
			base.StartCoroutine(this.CorUpdateDownloadingMod());
		}

		// Token: 0x06006BC4 RID: 27588 RVA: 0x0031B9F6 File Offset: 0x00319BF6
		private IEnumerator CorUpdateDownloadingMod()
		{
			while (this._downLoadingItemList.Count > 0)
			{
				bool hasChanged = false;
				foreach (ModId modId in this._downLoadingItemList)
				{
					bool flag = SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateInstalled) && !SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateNeedsUpdate) && !SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateDownloading);
					if (flag)
					{
						this._downLoadedItemList.Add(modId);
						hasChanged = true;
					}
					modId = default(ModId);
				}
				List<ModId>.Enumerator enumerator = default(List<ModId>.Enumerator);
				foreach (ModId modId2 in this._downLoadedItemList)
				{
					bool flag2 = this._downLoadingItemList.Contains(modId2);
					if (flag2)
					{
						this._downLoadingItemList.Remove(modId2);
					}
					modId2 = default(ModId);
				}
				List<ModId>.Enumerator enumerator2 = default(List<ModId>.Enumerator);
				bool flag3 = hasChanged;
				if (flag3)
				{
					this._downLoadedItemList.Clear();
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06006BC5 RID: 27589 RVA: 0x0031BA08 File Offset: 0x00319C08
		private void UnSubscribeItem(ModId modId)
		{
			ModManager.UnSubscribeItem(modId);
			bool flag = this._downLoadingItemList.Contains(modId);
			if (flag)
			{
				this._downLoadingItemList.Remove(modId);
			}
			bool flag2 = this._downLoadedItemList.Contains(modId);
			if (flag2)
			{
				this._downLoadedItemList.Remove(modId);
			}
		}

		// Token: 0x06006BC6 RID: 27590 RVA: 0x0031BA58 File Offset: 0x00319C58
		private void SubscribeItem(ModId modId, bool checkDependencies = true, Action onConfirm = null)
		{
			ModSubPageShop.<>c__DisplayClass59_0 CS$<>8__locals1 = new ModSubPageShop.<>c__DisplayClass59_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.modId = modId;
			CS$<>8__locals1.onConfirm = onConfirm;
			bool flag = !checkDependencies;
			if (flag)
			{
				CS$<>8__locals1.<SubscribeItem>g__OnConfirm|1();
			}
			else
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(CS$<>8__locals1.modId);
				bool flag2 = modInfo.Dependencies.Count > 0;
				if (flag2)
				{
					this.ParentView.ShowMask();
					ModManager.UpdateTargetItems(modInfo.Dependencies, delegate(Dictionary<ModId, bool> dependenciesChangeStateDict)
					{
						CS$<>8__locals1.<>4__this.ParentView.HideMask();
						List<ModId> dependenceList = dependenciesChangeStateDict.Keys.ToList<ModId>();
						CS$<>8__locals1.<>4__this.subscribeDependenceDialog.Show(dependenceList, delegate(bool only)
						{
							CS$<>8__locals1.<SubscribeItem>g__OnConfirm|1();
							if (!only)
							{
								foreach (ModId id in dependenceList)
								{
									CS$<>8__locals1.<>4__this.SubscribeItem(id, false, null);
								}
							}
						});
					});
				}
				else
				{
					CS$<>8__locals1.<SubscribeItem>g__OnConfirm|1();
				}
			}
		}

		// Token: 0x04004E13 RID: 19987
		[SerializeField]
		private InfinityScroll workshopScroll;

		// Token: 0x04004E14 RID: 19988
		[SerializeField]
		private TMP_InputField workshopSearchInputField;

		// Token: 0x04004E15 RID: 19989
		[SerializeField]
		private Texture2D popupModPanelLoadError0;

		// Token: 0x04004E16 RID: 19990
		[SerializeField]
		private ModIdSwitch workshopPageSwitch;

		// Token: 0x04004E17 RID: 19991
		[SerializeField]
		private CToggleGroup workshopSortToggleGroup;

		// Token: 0x04004E18 RID: 19992
		[SerializeField]
		private CDropdown workshopTimeDropdown;

		// Token: 0x04004E19 RID: 19993
		[SerializeField]
		private ModSubscribeDependenceDialog subscribeDependenceDialog;

		// Token: 0x04004E1A RID: 19994
		[SerializeField]
		private TextMeshProUGUI findCount;

		// Token: 0x04004E1B RID: 19995
		[SerializeField]
		private CToggleGroupMultiSelect tagToggleGroupMultiSelect;

		// Token: 0x04004E1C RID: 19996
		[SerializeField]
		private CButton refreshButton;

		// Token: 0x04004E1D RID: 19997
		[SerializeField]
		private CButton tagFilterButton;

		// Token: 0x04004E1E RID: 19998
		[SerializeField]
		private CButton clearTagButton;

		// Token: 0x04004E1F RID: 19999
		[SerializeField]
		private CButton tagsFilterMask;

		// Token: 0x04004E20 RID: 20000
		private readonly List<ModId> _workshopModIdList = new List<ModId>();

		// Token: 0x04004E21 RID: 20001
		private readonly List<ModId> _originWorkshopModIdList = new List<ModId>();

		// Token: 0x04004E22 RID: 20002
		private readonly List<ModId> _downLoadingItemList = new List<ModId>();

		// Token: 0x04004E23 RID: 20003
		private readonly List<ModId> _downLoadedItemList = new List<ModId>();

		// Token: 0x04004E24 RID: 20004
		private Texture2D _tempModCoverTexture;

		// Token: 0x04004E25 RID: 20005
		private const int WorkshopPageItemMaxCount = 25;

		// Token: 0x04004E26 RID: 20006
		private int _steamPageMaxCount = 1;

		// Token: 0x04004E27 RID: 20007
		private string _workshopSearchInputText;

		// Token: 0x02001DB3 RID: 7603
		public enum WorkshopSortToggleKey
		{
			// Token: 0x0400C702 RID: 50946
			MostPopular,
			// Token: 0x0400C703 RID: 50947
			MostRated,
			// Token: 0x0400C704 RID: 50948
			LatestUpdate,
			// Token: 0x0400C705 RID: 50949
			LatestUpload
		}

		// Token: 0x02001DB4 RID: 7604
		public enum WorkshopTimeToggleKey
		{
			// Token: 0x0400C707 RID: 50951
			All,
			// Token: 0x0400C708 RID: 50952
			Day,
			// Token: 0x0400C709 RID: 50953
			Week,
			// Token: 0x0400C70A RID: 50954
			Month,
			// Token: 0x0400C70B RID: 50955
			Quarter,
			// Token: 0x0400C70C RID: 50956
			Year
		}
	}
}
