using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Config;
using Config.Common;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using GameData.GameDataBridge;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000264 RID: 612
public class UI_ModPanel : UIBase
{
	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x06002807 RID: 10247 RVA: 0x00126D57 File Offset: 0x00124F57
	private bool IsInGame
	{
		get
		{
			return GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06002808 RID: 10248 RVA: 0x00126D66 File Offset: 0x00124F66
	private CButton CurModButtonOpenExplorer
	{
		get
		{
			return this.curModPanel.buttonOpenExplorer;
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x06002809 RID: 10249 RVA: 0x00126D73 File Offset: 0x00124F73
	private int WorkshopPageItemCurrentCount
	{
		get
		{
			return (this._workshopPageSwitch.Value == this._workshopPageSwitch.MaxValue) ? (this._workshopModIdList.Count % 48) : 48;
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x0600280A RID: 10250 RVA: 0x00126D9F File Offset: 0x00124F9F
	// (set) Token: 0x0600280B RID: 10251 RVA: 0x00126DA6 File Offset: 0x00124FA6
	public static UI_ModPanel.WorkshopTimeToggleKey CurWorkshopTimeToggleKey { get; private set; } = UI_ModPanel.WorkshopTimeToggleKey.All;

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x0600280C RID: 10252 RVA: 0x00126DAE File Offset: 0x00124FAE
	// (set) Token: 0x0600280D RID: 10253 RVA: 0x00126DB5 File Offset: 0x00124FB5
	public static UI_ModPanel.WorkshopSortToggleKey CurWorkshopSortToggleKey { get; private set; } = UI_ModPanel.WorkshopSortToggleKey.MostPopular;

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x0600280E RID: 10254 RVA: 0x00126DC0 File Offset: 0x00124FC0
	public static uint CurWorkshopTime
	{
		get
		{
			UI_ModPanel.WorkshopTimeToggleKey curWorkshopTimeToggleKey = UI_ModPanel.CurWorkshopTimeToggleKey;
			if (!true)
			{
			}
			uint result;
			switch (curWorkshopTimeToggleKey)
			{
			case UI_ModPanel.WorkshopTimeToggleKey.Year:
				result = 365U;
				break;
			case UI_ModPanel.WorkshopTimeToggleKey.Quarter:
				result = 90U;
				break;
			case UI_ModPanel.WorkshopTimeToggleKey.Month:
				result = 30U;
				break;
			case UI_ModPanel.WorkshopTimeToggleKey.Week:
				result = 7U;
				break;
			case UI_ModPanel.WorkshopTimeToggleKey.Day:
				result = 1U;
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

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x0600280F RID: 10255 RVA: 0x00126E1A File Offset: 0x0012501A
	private GameObject WorkshopDetailPanel
	{
		get
		{
			return this.workshopPanel.detail;
		}
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06002810 RID: 10256 RVA: 0x00126E27 File Offset: 0x00125027
	private GameObject WorkshopListPanel
	{
		get
		{
			return this.workshopPanel.list;
		}
	}

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06002811 RID: 10257 RVA: 0x00126E34 File Offset: 0x00125034
	private GameObject WorkshopSharePanel
	{
		get
		{
			return this.workshopPanel.share;
		}
	}

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x06002812 RID: 10258 RVA: 0x00126E41 File Offset: 0x00125041
	private CButton ButtonCreate
	{
		get
		{
			return this.uploadModPanel.buttonCreate;
		}
	}

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06002813 RID: 10259 RVA: 0x00126E4E File Offset: 0x0012504E
	private CButton ButtonCreateFormDirectory
	{
		get
		{
			return this.uploadModPanel.buttonCreateFormDirectory;
		}
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x06002814 RID: 10260 RVA: 0x00126E5B File Offset: 0x0012505B
	private CButton ButtonOpenUpload
	{
		get
		{
			return this.uploadModPanel.buttonOpenUpload;
		}
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x06002815 RID: 10261 RVA: 0x00126E68 File Offset: 0x00125068
	private CToggle ChangeConfigToggle
	{
		get
		{
			return this._uploadBasicInfoRefers.changeConfig;
		}
	}

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x06002816 RID: 10262 RVA: 0x00126E75 File Offset: 0x00125075
	private CToggle HasArchiveToggle
	{
		get
		{
			return this._uploadBasicInfoRefers.hasArchive;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x06002817 RID: 10263 RVA: 0x00126E82 File Offset: 0x00125082
	private CToggle NeedRestartToggle
	{
		get
		{
			return this._uploadBasicInfoRefers.needRestart;
		}
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06002818 RID: 10264 RVA: 0x00126E8F File Offset: 0x0012508F
	private CButton UploadModButtonOpenExplorer
	{
		get
		{
			return this.uploadModPanel.buttonOpenExplorer;
		}
	}

	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x06002819 RID: 10265 RVA: 0x00126E9C File Offset: 0x0012509C
	private bool CurEditModIsNotCreated
	{
		get
		{
			return ModManager.UploadedMods.All((ModId u) => u.FileId != this._curEditModInfo.ModId.FileId);
		}
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x00126EB4 File Offset: 0x001250B4
	public override void OnInit(ArgumentBox argsBox)
	{
		SteamManager.InitString();
		this.InitCurModPanel();
		this.InitWorkshopPanel();
		this.InitUploadPanel();
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x00126ED2 File Offset: 0x001250D2
	public void Awake()
	{
		this.toggleGroup.OnActiveIndexChange += this.OnActiveToggleChange;
		this.toggleGroup.Init(-1);
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x00126EFA File Offset: 0x001250FA
	private void OnDestroy()
	{
		PoolManager.RemoveData("ModPanel_SettingEntry_Toggle");
		PoolManager.RemoveData("ModPanel_SettingEntry_Dropdown");
		PoolManager.RemoveData("ModPanel_SettingEntry_Slider");
		PoolManager.RemoveData("ModPanel_SettingEntry_InputField");
		PoolManager.RemoveData("ModPanel_Setting_Row");
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x00126F34 File Offset: 0x00125134
	private void OnEnable()
	{
		GEvent.Add(UiEvents.WorkshopModPreviewImageHasDownloaded, new GEvent.Callback(this.OnWorkshopModPreviewImageHasDownloaded));
		GEvent.Add(UiEvents.ModEditSettings, new GEvent.Callback(this.OnModEditSettings));
		GEvent.Add(UiEvents.DisableDependenciesChangedMods, new GEvent.Callback(this.OnDisableDependenciesChangedMods));
		this._isEditingUploadMod = false;
		this.toggleGroup.Set(UI_ModPanel.PanelToggleKey.CurMod.ToInt(), true);
		this._downLoadedItemList.Clear();
		this._downLoadingItemList.Clear();
		this.UpdateModList();
		ModManager.SaveModSettings(true);
		this.ClearChangeState();
		this.RefreshApplyButton();
		bool flag = UIManager.Instance.IsElementActive(UIElement.SystemOption);
		if (flag)
		{
			AudioManager.Instance.EnableMusicVolumeRate(0.2f);
			AudioManager.Instance.SetMusicVolumeWithFade(1f, 0f);
		}
		bool isPlatformAvailable = ModManager.IsPlatformAvailable;
		List<CToggle> all = this.toggleGroup.GetAll();
		for (int i = 0; i < all.Count; i++)
		{
			bool flag2 = i > UI_ModPanel.PanelToggleKey.CurMod.ToInt();
			if (flag2)
			{
				all[i].gameObject.SetActive(isPlatformAvailable);
			}
		}
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x00127078 File Offset: 0x00125278
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.WorkshopModPreviewImageHasDownloaded, new GEvent.Callback(this.OnWorkshopModPreviewImageHasDownloaded));
		GEvent.Remove(UiEvents.ModEditSettings, new GEvent.Callback(this.OnModEditSettings));
		GEvent.Remove(UiEvents.DisableDependenciesChangedMods, new GEvent.Callback(this.OnDisableDependenciesChangedMods));
		this._dependenciesChangedList.Clear();
		ModManager.Clear();
		this._hasUpdateDetailInfo = false;
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x001270F4 File Offset: 0x001252F4
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string text = name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1748097320U)
		{
			if (num <= 969483025U)
			{
				if (num <= 605753053U)
				{
					if (num != 354874508U)
					{
						if (num != 605753053U)
						{
							return;
						}
						if (!(text == "ButtonSyncAll"))
						{
							return;
						}
						this.OnClickSync(true);
						return;
					}
					else
					{
						if (!(text == "UploadBtn"))
						{
							return;
						}
						goto IL_2FF;
					}
				}
				else if (num != 753398408U)
				{
					if (num != 969483025U)
					{
						return;
					}
					if (!(text == "CloseBtn"))
					{
						return;
					}
				}
				else
				{
					if (!(text == "RemoveLocalBtn"))
					{
						return;
					}
					goto IL_2F6;
				}
			}
			else
			{
				if (num > 1323743248U)
				{
					if (num != 1502442331U)
					{
						if (num != 1748097320U)
						{
							return;
						}
						if (!(text == "DeleteRemoteBtn"))
						{
							return;
						}
					}
					else if (!(text == "DeleteLocalBtn"))
					{
						return;
					}
					this.OnClickDelete();
					return;
				}
				if (num != 1225266778U)
				{
					if (num != 1323743248U)
					{
						return;
					}
					if (!(text == "SaveBtn"))
					{
						return;
					}
					this.OnClickSave();
					return;
				}
				else
				{
					if (!(text == "EnableBtn"))
					{
						return;
					}
					this.EnableItem(this._curModInfo, this._curIndex);
					return;
				}
			}
		}
		else if (num <= 2941182009U)
		{
			if (num <= 2292684623U)
			{
				if (num != 2166359122U)
				{
					if (num != 2292684623U)
					{
						return;
					}
					if (!(text == "ButtonClosePopup"))
					{
						return;
					}
				}
				else
				{
					if (!(text == "ButtonOpenExplorer"))
					{
						return;
					}
					this.OnClickOpenExplorer();
					return;
				}
			}
			else if (num != 2555788625U)
			{
				if (num != 2941182009U)
				{
					return;
				}
				if (!(text == "RemoveRemoteBtn"))
				{
					return;
				}
				goto IL_2F6;
			}
			else
			{
				if (!(text == "DisableBtn"))
				{
					return;
				}
				this.SetModEnabled(this._curIndex, this._curModId, false);
				return;
			}
		}
		else if (num <= 3251915448U)
		{
			if (num != 2957524969U)
			{
				if (num != 3251915448U)
				{
					return;
				}
				if (!(text == "UpdateModListBtn"))
				{
					return;
				}
				this.OnClickUpdate();
				return;
			}
			else
			{
				if (!(text == "ApplyBtn"))
				{
					return;
				}
				bool isInGame = this.IsInGame;
				if (isInGame)
				{
					this.SaveAllSettings();
				}
				else
				{
					this.Apply();
				}
				return;
			}
		}
		else if (num != 3509380516U)
		{
			if (num != 3542127896U)
			{
				if (num != 3600043662U)
				{
					return;
				}
				if (!(text == "ButtonSync"))
				{
					return;
				}
				this.OnClickSync(false);
				return;
			}
			else
			{
				if (!(text == "ButtonDependency"))
				{
					return;
				}
				this.OnClickDependency();
				return;
			}
		}
		else
		{
			if (!(text == "UpdateBtn"))
			{
				return;
			}
			goto IL_2FF;
		}
		this.QuickHide();
		return;
		IL_2F6:
		this.OnClickRemoveCurMod();
		return;
		IL_2FF:
		this.OnClickUpload();
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x0012744C File Offset: 0x0012564C
	public override void QuickHide()
	{
		bool exist = UIElement.FullScreenMask.Exist;
		if (!exist)
		{
			bool flag = this._curToggleKey == UI_ModPanel.PanelToggleKey.UploadMod;
			if (flag)
			{
				foreach (CDropdown dropdown in this._uploadModTagDropdownList)
				{
					dropdown.Hide();
				}
				this._uploadVisibilityDropdown.Hide();
				bool activeSelf = this._editSettingPanel.gameObject.activeSelf;
				if (activeSelf)
				{
					this._editSettingPanel.Hide();
				}
				else
				{
					bool activeSelf2 = this._editUpdateLogPanel.gameObject.activeSelf;
					if (activeSelf2)
					{
						this._editUpdateLogPanel.Hide();
					}
					else
					{
						bool activeSelf3 = this._setProgramPanel.gameObject.activeSelf;
						if (activeSelf3)
						{
							this._setProgramPanel.Hide();
						}
						else
						{
							bool activeSelf4 = this._modUploadConfirmDialog.gameObject.activeSelf;
							if (activeSelf4)
							{
								this._modUploadConfirmDialog.Hide();
							}
							else
							{
								bool activeSelf5 = this._setDependencePanel.gameObject.activeSelf;
								if (activeSelf5)
								{
									this._setDependencePanel.Hide();
								}
								else
								{
									bool activeSelf6 = this._modDirectlyUploadPanel.gameObject.activeSelf;
									if (activeSelf6)
									{
										this._modDirectlyUploadPanel.Hide();
									}
									else
									{
										this.CancelEditUploadMod(new Action(base.QuickHide), null);
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag2 = this._curToggleKey == UI_ModPanel.PanelToggleKey.WorkshopMod;
				if (flag2)
				{
					bool activeSelf7 = this._subscribeDependenceDialog.gameObject.activeSelf;
					if (activeSelf7)
					{
						this._subscribeDependenceDialog.Hide();
						return;
					}
					bool activeSelf8 = this.WorkshopSharePanel.activeSelf;
					if (activeSelf8)
					{
						this.ShowWorkshopModSharePanel(false);
						return;
					}
					bool activeSelf9 = this.WorkshopDetailPanel.activeSelf;
					if (activeSelf9)
					{
						this.ShowWorkshopModDetailPanel(false);
						return;
					}
				}
				bool flag3 = this._curToggleKey == UI_ModPanel.PanelToggleKey.CurMod;
				if (flag3)
				{
					bool activeSelf10 = this._enableDependenceDialog.gameObject.activeSelf;
					if (activeSelf10)
					{
						this._enableDependenceDialog.Hide();
					}
					else
					{
						this.CancelEditCurMod(delegate
						{
							SingletonObject.getInstance<YieldHelper>().StartYield(ModManager.RestoreModConfigImpl());
							AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
							base.QuickHide();
						}, null);
					}
				}
				else
				{
					AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
					base.QuickHide();
				}
			}
		}
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x001276B4 File Offset: 0x001258B4
	private void OnClickUpdate()
	{
		switch (this._curToggleKey)
		{
		case UI_ModPanel.PanelToggleKey.CurMod:
			this.CancelEditCurMod(delegate
			{
				this.ClearModInfo(false);
				this.UpdateModList();
			}, null);
			break;
		case UI_ModPanel.PanelToggleKey.WorkshopMod:
			this.RefreshPage(true);
			break;
		case UI_ModPanel.PanelToggleKey.UploadMod:
			this.CancelEditUploadMod(delegate
			{
				this.ClearUploadModInfo();
				ModManager.UpdateModList(delegate
				{
					this.RefreshUploadModList(true, true, false);
				});
			}, null);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x0012771C File Offset: 0x0012591C
	private void OnActiveToggleChange(int newTog, int oldTog)
	{
		UI_ModPanel.<>c__DisplayClass163_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass163_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.oldTog = oldTog;
		CS$<>8__locals1.newTog = newTog;
		bool flag = CS$<>8__locals1.newTog >= 0;
		if (flag)
		{
			bool flag2 = this._curToggleKey == UI_ModPanel.PanelToggleKey.UploadMod;
			if (flag2)
			{
				this.CancelEditUploadMod(new Action(CS$<>8__locals1.<OnActiveToggleChange>g__Action|1), delegate
				{
					CS$<>8__locals1.<>4__this.toggleGroup.SetWithoutNotify(CS$<>8__locals1.oldTog);
				});
			}
			else
			{
				CS$<>8__locals1.<OnActiveToggleChange>g__Action|1();
			}
		}
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x00127790 File Offset: 0x00125990
	private void InitCurModPanel()
	{
		this._curModSettingsHolder = this.curModPanel.settings;
		this._curBasicInfoRefers = this.curModPanel.basicInfo;
		this._curModScroll = this.curModPanel.curModScroll;
		this._curModScroll.srcPrefab.gameObject.SetActive(false);
		this._curModScroll.RemoveOnScrollEvent(new Action(this.OnCurModScroll));
		this._curModScroll.AddOnScrollEvent(new Action(this.OnCurModScroll));
		this._curModScroll.OnItemRender += this.OnCurModRender;
		this._curModPageMaxCount = this._curModScroll.GetPageMaxCount();
		this._curModScrollToggleGroup = this._curModScroll.GetComponent<CToggleGroup>();
		this._curModScrollToggleGroup.OnActiveIndexChange += this.OnCurModToggleChange;
		this._curModPageSwitch = this.curModPanel.curModPageSwitch;
		this._curModPageSwitch.OnValueChanged = new Action<int>(this.OnCurModPageValueChanged);
		this._curModPageSwitch.SetValueAndRefresh(0);
		this._curModSearchInputField = this.curModPanel.curModSearchInputField;
		this._curModSearchInputField.onValueChanged.RemoveAllListeners();
		this._curModSearchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnCurModSearchInputValueChange));
		this._curModSearchInputField.SetTextWithoutNotify(string.Empty);
		this._enableDependenceDialog = this.curModPanel.enableDependenceDialog;
		this.ClearModInfo(false);
		this.InitSettingPrefabs();
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x00127910 File Offset: 0x00125B10
	private void InitSettingPrefabs()
	{
		this.toggleContainer.UserString = "ModPanel_SettingEntry_Toggle";
		PoolManager.SetSrcObject("ModPanel_SettingEntry_Toggle", this.toggleContainer.gameObject);
		this.dropdownContainer.UserString = "ModPanel_SettingEntry_Dropdown";
		PoolManager.SetSrcObject("ModPanel_SettingEntry_Dropdown", this.dropdownContainer.gameObject);
		this.sliderContainer.UserString = "ModPanel_SettingEntry_Slider";
		PoolManager.SetSrcObject("ModPanel_SettingEntry_Slider", this.sliderContainer.gameObject);
		this.inputFieldContainer.UserString = "ModPanel_SettingEntry_InputField";
		PoolManager.SetSrcObject("ModPanel_SettingEntry_InputField", this.inputFieldContainer.gameObject);
		this.rowContainer.GetComponent<RectTransform>().SetSize(new Vector2(560f, 60f));
		PoolManager.SetSrcObject("ModPanel_Setting_Row", this.rowContainer);
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x001279E8 File Offset: 0x00125BE8
	private void OnCurModPageValueChanged(int index)
	{
		bool isAdd = index > this._lastCurModPageSwitchValue;
		this._lastCurModPageSwitchValue = index;
		int targetIndex = this._curModPageMaxCount * (isAdd ? index : (index - 1));
		bool flag = index < this._curModPageSwitch.MaxValue;
		if (flag)
		{
			targetIndex--;
		}
		targetIndex = Mathf.Max(0, targetIndex);
		this._curModScroll.Refresh(targetIndex);
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x00127A44 File Offset: 0x00125C44
	private void OnCurModScroll()
	{
		float curValue = this._curModScroll.Scroll.ScrollBar.value * (float)this._curModScroll.CurrentDataCount;
		float value = curValue / (float)this._curModPageMaxCount;
		this._curModPageSwitch.SetValueAndRefresh(Mathf.CeilToInt(value));
		this._lastCurModPageSwitchValue = this._curModPageSwitch.Value;
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x00127AA4 File Offset: 0x00125CA4
	private void OnCurModToggleChange(int newTog, int oldTog)
	{
		bool flag = oldTog >= 0;
		if (flag)
		{
			this._curModScroll.RefreshCell(oldTog);
		}
		bool flag2 = newTog >= 0;
		if (flag2)
		{
			this._curModScroll.RefreshCell(newTog);
		}
		bool flag3 = newTog >= 0;
		if (flag3)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(this._curModIdList[newTog]);
			this.UpdateModInfo(newTog, modInfo);
		}
		else
		{
			this.ClearModInfo(false);
		}
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x00127B14 File Offset: 0x00125D14
	private void OnCurModSearchInputValueChange(string value)
	{
		string inputValue = this.GetSearchInputValue(this._curModSearchInputField);
		bool flag = inputValue.IsNullOrEmpty();
		if (flag)
		{
			this.RefreshCurModList(true);
		}
		else
		{
			this._curModIdList.Clear();
			foreach (ModId modId in this._originCurModIdList)
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				bool flag2 = modInfo.Title.Contains(inputValue);
				if (flag2)
				{
					this._curModIdList.Add(modId);
				}
			}
			this.RefreshCurModList(false);
		}
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x00127BC4 File Offset: 0x00125DC4
	private void ClearChangeState()
	{
		this._hasChangedEnable = false;
		this._hasChangedSetting = false;
		this._needRestart = false;
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x00127BDC File Offset: 0x00125DDC
	private void UpdateModList()
	{
		ModManager.UpdateModList(delegate
		{
			bool flag = !this.IsInGame;
			if (flag)
			{
				this._hasUpdateDetailInfo = false;
			}
			bool flag2 = this.IsInGame && this._hasUpdateDetailInfo;
			if (flag2)
			{
				this.RefreshCurModList(true);
			}
			else
			{
				bool flag3 = !this._hasUpdateDetailInfo;
				if (flag3)
				{
					this.ShowMask();
					ModManager.UpdateUploadedItems(delegate(Dictionary<ModId, bool> uploadedDependencyChangeStateDict)
					{
						Action<Dictionary<ModId, bool>> <>9__3;
						ModManager.UpdateSubscribedItems(delegate(Dictionary<ModId, bool> subscribedDependencyChangeStateDict)
						{
							bool flag4 = subscribedDependencyChangeStateDict != null && subscribedDependencyChangeStateDict.Count > 0;
							if (flag4)
							{
								foreach (KeyValuePair<ModId, bool> keyValuePair in subscribedDependencyChangeStateDict)
								{
									ModId modId;
									bool flag5;
									keyValuePair.Deconstruct(out modId, out flag5);
									ModId key = modId;
									bool value = flag5;
									uploadedDependencyChangeStateDict.TryAdd(key, value);
								}
							}
							Action<Dictionary<ModId, bool>> onFinished;
							if ((onFinished = <>9__3) == null)
							{
								ModId modId;
								onFinished = (<>9__3 = delegate(Dictionary<ModId, bool> _)
								{
									this._hasUpdateDetailInfo = true;
									this._dependenciesChangedList.Clear();
									bool flag6 = uploadedDependencyChangeStateDict != null && uploadedDependencyChangeStateDict.Count > 0;
									if (flag6)
									{
										foreach (KeyValuePair<ModId, bool> keyValuePair2 in uploadedDependencyChangeStateDict)
										{
											ModId modId2;
											bool flag7;
											keyValuePair2.Deconstruct(out modId2, out flag7);
											ModId modId = modId2;
											bool changed = flag7;
											bool flag8 = changed && ModManager.EnabledMods.Exists((ModId id) => id.FileId == modId.FileId);
											if (flag8)
											{
												this._dependenciesChangedList.Add(modId);
											}
										}
									}
									this._tempEnabledModIdList.Clear();
									this._tempEnabledModIdList.AddRange(ModManager.EnabledMods);
									bool flag9 = this._dependenciesChangedList.Count > 0;
									if (flag9)
									{
										ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("DependenciesChangedList", this._dependenciesChangedList);
										UIElement.ModDependenceChangeList.SetOnInitArgs(args);
										UIManager.Instance.ShowUI(UIElement.ModDependenceChangeList, true);
									}
									this.RefreshCurModList(true);
									this.HideMask();
								});
							}
							ModManager.UpdateUploadedItems(onFinished);
						});
					});
				}
			}
		});
	}

	// Token: 0x0600282B RID: 10283 RVA: 0x00127BF4 File Offset: 0x00125DF4
	private void RefreshCurModList(bool refreshData = true)
	{
		if (refreshData)
		{
			this._originCurModIdList.Clear();
			bool isInGame = this.IsInGame;
			if (isInGame)
			{
				this._originCurModIdList.AddRange(ModManager.EnabledMods);
			}
			else
			{
				this._originCurModIdList.AddRange(ModManager.ExternalMods);
				this._originCurModIdList.AddRange(from id in ModManager.PlatformMods
				where ModManager.GetModInfo(id).IsSubscribed
				select id);
			}
			this._originCurModIdList.Sort(delegate(ModId a, ModId b)
			{
				bool aIsEnabled = ModManager.EnabledMods.Contains(a);
				return ModManager.EnabledMods.Contains(b).CompareTo(aIsEnabled);
			});
			this._curModIdList.Clear();
			this._curModIdList.AddRange(this._originCurModIdList);
		}
		bool curModPanelNeedScrollToSelectedWorkshopMod = this._curModPanelNeedScrollToSelectedWorkshopMod;
		int index;
		if (curModPanelNeedScrollToSelectedWorkshopMod)
		{
			this._curModPanelNeedScrollToSelectedWorkshopMod = false;
			index = this._curModIdList.IndexOf(this._selectedWorkshopModInfo.ModId);
		}
		else
		{
			index = this._curModIdList.IndexOf(this._curModId);
		}
		bool flag = index >= 0;
		if (flag)
		{
			this._curModScrollToggleGroup.SetWithoutNotify(index);
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(this._curModIdList[index]);
			this.UpdateModInfo(index, modInfo);
			this._curModScroll.ScrollTo(index + 1, 0.3f);
		}
		else
		{
			this._curModScrollToggleGroup.DeSelectWithoutNotify();
			this.ClearModInfo(false);
		}
		this._curModScroll.SetDataCount(this._curModIdList.Count);
		int maxValue = Mathf.CeilToInt((float)this._curModIdList.Count / (float)this._curModPageMaxCount);
		this._lastCurModPageSwitchValue = 1;
		this._curModPageSwitch.Init(1, Mathf.Max(1, maxValue), 1);
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x00127DBC File Offset: 0x00125FBC
	private void ShowRestart()
	{
		if (this._restartDialog == null)
		{
			this._restartDialog = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Apply_Restart_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Apply_Restart_Content),
				Type = 1,
				Yes = new Action(this.<ShowRestart>g__OnConfirmRestart|173_0)
			};
		}
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._restartDialog));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x00127E48 File Offset: 0x00126048
	private void SaveDependenciesChangedMod()
	{
		foreach (ModId modId in this._dependenciesChangedList)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			ModManager.SaveModInfo(modInfo);
		}
		this._dependenciesChangedList.Clear();
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x00127EB4 File Offset: 0x001260B4
	private void OnCurModRender(int index, GameObject go)
	{
		ModId modId = this._curModIdList[index];
		ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
		bool isValid = modInfo != null;
		bool flag = !isValid;
		if (flag)
		{
			GLog.TagWarn("UI_ModPanel", string.Format("OnCurModRender Not Found Mod {0}", modId), Array.Empty<object>());
		}
		ModEntryTemplate refers = go.GetComponent<ModEntryTemplate>();
		TextMeshProUGUI titleText = refers.title;
		string title = isValid ? modInfo.Title : string.Empty;
		titleText.SetText(title, true);
		TooltipInvoker titleTip = titleText.GetComponent<TooltipInvoker>();
		titleTip.enabled = isValid;
		bool flag2 = titleTip.PresetParam == null || titleTip.PresetParam.Length == 0;
		if (flag2)
		{
			titleTip.PresetParam = new string[1];
		}
		titleTip.PresetParam[0] = title;
		bool isModEnabled = this._tempEnabledModIdList.Contains(modId);
		bool isInGame = this.IsInGame;
		CToggle modToggle = refers.GetComponent<CToggle>();
		modToggle.interactable = ((isModEnabled || !isInGame) && isValid);
		List<CToggle> toggleList = this._curModScrollToggleGroup.GetAll();
		bool flag3 = !toggleList.Contains(modToggle);
		if (flag3)
		{
			this._curModScrollToggleGroup.Add(modToggle);
		}
		bool isDependenciesChanged = this._dependenciesChangedList.Exists((ModId m) => m.Equals(modId));
		CToggle isEnabledTog = refers.isModEnabledToggle;
		isEnabledTog.onValueChanged.RemoveAllListeners();
		isEnabledTog.isOn = isModEnabled;
		isEnabledTog.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				isEnabledTog.SetIsOnWithoutNotify(false);
				this.EnableItem(modInfo, index);
			}
			else
			{
				this.SetModEnabled(index, modId, false);
			}
		});
		isEnabledTog.gameObject.SetActive(!isInGame && isValid);
		refers.steam.SetActive(isValid && modId.Source == 1);
		refers.external.SetActive(isValid && modId.Source == 0);
		refers.rating.SetActive(isValid && modId.Source == 1);
		int score = isValid ? modInfo.Score : 0;
		for (int i = 0; i < refers.starList.Count; i++)
		{
			string icon = (i < score) ? UI_ModPanel.StarActiveIcon : UI_ModPanel.StarInactiveIcon;
			refers.starList[i].SetSprite(icon, false, null);
		}
		TextMeshProUGUI ratingText = refers.ratingAmountLabel;
		ratingText.gameObject.SetActive(isValid);
		bool flag4 = isValid;
		if (flag4)
		{
			ratingText.text = LocalStringManager.GetFormat("LK_Steam_Mod_Rating_Count", modInfo.VoteCount);
		}
		bool isDownLoading = isValid && modInfo.IsSubscribed && !SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateInstalled);
		refers.downloading.SetActive(isDownLoading);
		bool isModOutdated = isValid && ModManager.IsModOutdated(modInfo) && !isDownLoading;
		refers.outdatedWarning.SetActive(isModOutdated);
		bool hasNotEnabledDependency = isValid && modInfo.Dependencies.Exists((ulong d) => !this._tempEnabledModIdList.Exists((ModId e) => e.FileId == d));
		refers.dependencyWarning.SetActive(hasNotEnabledDependency);
		string spName = "popup_modpanel_anniu_0_0";
		bool flag5 = isModOutdated;
		if (flag5)
		{
			spName = "popup_modpanel_anniu_0_2";
		}
		bool flag6 = isDownLoading;
		if (flag6)
		{
			spName = "popup_modpanel_anniu_0_3";
		}
		bool flag7 = isDependenciesChanged;
		if (flag7)
		{
			spName = "popup_modpanel_anniu_0_4";
		}
		refers.back.SetSprite(spName, false, null);
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x00128260 File Offset: 0x00126460
	private void EnableItem(ModInfoWithDisplayData modInfo, int index)
	{
		bool flag = modInfo.Dependencies.Count > 0;
		if (flag)
		{
			this.ShowMask();
			Func<ModId, bool> <>9__2;
			ModManager.UpdateTargetItems(modInfo.Dependencies, delegate(Dictionary<ModId, bool> dependenciesChangeStateDict)
			{
				this.HideMask();
				List<ModId> dependenceList = new List<ModId>();
				bool flag2 = dependenciesChangeStateDict != null;
				if (flag2)
				{
					dependenceList.AddRange(dependenciesChangeStateDict.Keys);
				}
				IEnumerable<ModId> local = from modId in ModManager.ExternalMods
				where modInfo.Dependencies.Contains(modId.FileId) && (dependenciesChangeStateDict == null || dependenciesChangeStateDict.All((KeyValuePair<ModId, bool> pair) => pair.Key.FileId != modId.FileId))
				select modId;
				dependenceList.AddRange(local);
				bool flag3 = dependenceList.Count == 0;
				if (flag3)
				{
					this.SetModEnabled(index, modInfo.ModId, true);
				}
				else
				{
					IEnumerable<ModId> dependenceList2 = dependenceList;
					Func<ModId, bool> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ((ModId dependence) => this._tempEnabledModIdList.Exists((ModId modId) => modId.FileId == dependence.FileId)));
					}
					bool allEnabled = dependenceList2.All(predicate);
					bool flag4 = allEnabled;
					if (flag4)
					{
						this.SetModEnabled(index, modInfo.ModId, true);
					}
					else
					{
						this._enableDependenceDialog.Show(dependenceList, delegate
						{
							foreach (ModId id in dependenceList)
							{
								this.SubscribeItem(id, false, null);
							}
							this.RefreshCurModList(true);
						}, delegate
						{
							foreach (ModId id in dependenceList)
							{
								this.SetModEnabled(id, true);
							}
							this.SetModEnabled(index, modInfo.ModId, true);
						});
					}
				}
			});
		}
		else
		{
			this.SetModEnabled(index, modInfo.ModId, true);
		}
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x001282E4 File Offset: 0x001264E4
	private void SetSettingChangeState(bool changed, ModInfoWithDisplayData modInfo = null)
	{
		this._hasChangedSetting = changed;
		bool flag = this._hasChangedSetting && modInfo != null;
		if (flag)
		{
			bool needRestart = modInfo.NeedRestartWhenSettingChanged;
			bool flag2 = needRestart;
			if (flag2)
			{
				this._needRestart = true;
			}
		}
		this.RefreshApplyButton();
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x00128328 File Offset: 0x00126528
	private void RefreshEnableChangeState()
	{
		this._hasChangedEnable = this._tempEnabledModIdList.ContentIsDifferent(ModManager.EnabledMods);
		this.RefreshApplyButton();
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x00128348 File Offset: 0x00126548
	private void RefreshApplyButton()
	{
		this.curModPanel.applyBtn.interactable = (this._hasChangedSetting || this._hasChangedEnable);
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x00128370 File Offset: 0x00126570
	private void SetModEnabled(int index, ModId modId, bool isEnabled)
	{
		bool flag = index < 0;
		if (!flag)
		{
			this.SetModEnabled(modId, isEnabled);
			bool isInGame = this.IsInGame;
			this.curModPanel.enableBtn.gameObject.SetActive(!isInGame && !isEnabled);
			this.curModPanel.disableBtn.gameObject.SetActive(!isInGame && isEnabled);
			int curSelected = this._curModScrollToggleGroup.GetActiveIndex();
			this._curModScrollToggleGroup.Set(index, false);
			bool flag2 = curSelected == index;
			if (flag2)
			{
				this._curModScroll.RefreshCell(index);
			}
			this.RefreshEnableChangeState();
			this.RefreshCurModList(false);
		}
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x00128418 File Offset: 0x00126618
	private void SetModEnabled(ModId modId, bool isEnabled)
	{
		bool flag = isEnabled && !this._tempEnabledModIdList.Contains(modId);
		if (flag)
		{
			int existCount = this._tempEnabledModIdList.RemoveAll((ModId id) => id.FileId == modId.FileId);
			bool flag2 = existCount > 0;
			if (flag2)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Enable_Same_Tip);
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
			}
			this._tempEnabledModIdList.Add(modId);
		}
		else
		{
			bool flag3 = !isEnabled && this._tempEnabledModIdList.Contains(modId);
			if (flag3)
			{
				this._tempEnabledModIdList.Remove(modId);
			}
		}
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x001284DC File Offset: 0x001266DC
	private void UpdateModSource(RectTransform sourceHolder, sbyte modSource)
	{
		sourceHolder.GetChild(0).gameObject.SetActive(modSource == 1);
		sourceHolder.GetChild(1).gameObject.SetActive(modSource == 0);
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x0012850C File Offset: 0x0012670C
	private void ClearModInfo(bool showButtons = false)
	{
		this.CurModButtonOpenExplorer.gameObject.SetActive(false);
		this._curIndex = -1;
		this._curModId = default(ModId);
		this._curModInfo = null;
		TextMeshProUGUI nameText = this._curBasicInfoRefers.nameValue;
		nameText.SetText(string.Empty, true);
		TooltipInvoker nameTip = nameText.GetComponent<TooltipInvoker>();
		bool flag = nameTip.PresetParam == null || nameTip.PresetParam.Length == 0;
		if (flag)
		{
			nameTip.PresetParam = new string[1];
		}
		nameTip.PresetParam[0] = string.Empty;
		this._curBasicInfoRefers.authorValue.SetText(string.Empty, true);
		this._curBasicInfoRefers.versionValue.SetText(string.Empty, true);
		this.UpdateModSource(this._curBasicInfoRefers.sourceValue, -1);
		this._curBasicInfoRefers.fileIdValue.SetText(string.Empty, true);
		this._curBasicInfoRefers.fileSizeValue.SetText(string.Empty, true);
		this._curBasicInfoRefers.createDateValue.SetText(string.Empty, true);
		this._curBasicInfoRefers.updateDataValue.SetText(string.Empty, true);
		this._curBasicInfoRefers.countFavorite.SetText(string.Empty, true);
		this._curBasicInfoRefers.countSubscribe.SetText(string.Empty, true);
		this._curBasicInfoRefers.countComment.SetText(string.Empty, true);
		this._curBasicInfoRefers.tagLayout.gameObject.SetActive(false);
		this._curBasicInfoRefers.starLayout.gameObject.SetActive(false);
		this.ClearCover(this._curBasicInfoRefers.modImageInfo);
		this.RefreshCurModButtons(null, showButtons);
		this.curModPanel.description.SetText(string.Empty, true);
		this._curModSettingsHolder.gameObject.SetActive(false);
		this.UpdateSettings(null, null, null);
		this.curModPanel.versionWarningMark.SetActive(false);
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x00128708 File Offset: 0x00126908
	private void RefreshCurModButtons(ModInfoWithDisplayData modInfo, bool isShow)
	{
		bool isDownloaded = modInfo != null && (!modInfo.IsSubscribed || SteamManager.IsItemStateActive(modInfo.ModId.FileId, EItemState.k_EItemStateInstalled));
		bool isEnabled = modInfo != null && this._tempEnabledModIdList.Contains(modInfo.ModId);
		this.curModPanel.enableBtn.gameObject.SetActive(isShow && isDownloaded && !isEnabled && !this.IsInGame);
		this.curModPanel.disableBtn.gameObject.SetActive(isShow && isDownloaded && isEnabled && !this.IsInGame);
		bool isLocal = modInfo == null || modInfo.ModId.Source == 0;
		CButton removeRemoteBtn = this.curModPanel.removeRemoteBtn;
		removeRemoteBtn.gameObject.SetActive(isShow && !this.IsInGame && !isLocal);
		removeRemoteBtn.interactable = isDownloaded;
		CButton removeLocalBtn = this.curModPanel.removeLocalBtn;
		removeLocalBtn.gameObject.SetActive(isShow && !this.IsInGame && isLocal);
		removeLocalBtn.interactable = isDownloaded;
		CButton downloadingBtn = this.curModPanel.downloadingBtn;
		downloadingBtn.gameObject.SetActive(isShow && !isDownloaded && !this.IsInGame);
		downloadingBtn.interactable = false;
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x00128854 File Offset: 0x00126A54
	private void UpdateModInfo(int index, ModInfoWithDisplayData modInfo)
	{
		this._curIndex = index;
		this._curModId = modInfo.ModId;
		this._curModInfo = modInfo;
		this._tempModDetailImageFileNameList.Clear();
		this._tempModDetailImageFileNameList.AddRange(modInfo.DetailImageList);
		this._tempModDetailImageFilePathList.Clear();
		this._tempModDetailImageFilePathList.AddRange(modInfo.DetailImageList);
		this.RefreshBasicInfo(this._curBasicInfoRefers, modInfo);
		this.RefreshCurModButtons(modInfo, true);
		this.curModPanel.description.SetText(modInfo.Description, true);
		this._curModSettingsHolder.gameObject.SetActive(true);
		this.UpdateSettings(modInfo, this._curModSettingsHolder, null);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._curModSettingsHolder);
			this._curModSettingsHolder.GetComponentInParent<CScrollRect>(true).ScrollBar.value = 0f;
		});
		this.CurModButtonOpenExplorer.gameObject.SetActive(true);
		this.curModPanel.versionWarningMark.SetActive(modInfo.ModId.Version == 0UL);
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x00128954 File Offset: 0x00126B54
	private void UpdateSettings(ModInfoWithDisplayData modInfo, RectTransform settingHolder, List<SettingEntry> modSettingEntries = null)
	{
		UI_ModPanel.<>c__DisplayClass186_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass186_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.modInfo = modInfo;
		if (modSettingEntries == null)
		{
			ModInfoWithDisplayData modInfo2 = CS$<>8__locals1.modInfo;
			modSettingEntries = ((modInfo2 != null) ? modInfo2.ModSettingEntries : null);
		}
		foreach (ModSettingWidgetsContainer refers in this._settingEntriesList)
		{
			PoolManager.Destroy(refers.UserString, refers.gameObject);
		}
		this._settingEntriesList.Clear();
		foreach (Transform row in this._settingRowList)
		{
			PoolManager.Destroy("ModPanel_Setting_Row", row.gameObject);
		}
		this._settingRowList.Clear();
		this._curModSettingDropdownList.Clear();
		bool flag = CS$<>8__locals1.modInfo == null || modSettingEntries.Count == 0;
		if (!flag)
		{
			bool isUpload = this._curToggleKey == UI_ModPanel.PanelToggleKey.UploadMod;
			using (List<SettingEntry>.Enumerator enumerator3 = modSettingEntries.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					UI_ModPanel.<>c__DisplayClass186_1 CS$<>8__locals2 = new UI_ModPanel.<>c__DisplayClass186_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.entry = enumerator3.Current;
					ToggleSetting toggleSettingEntry = CS$<>8__locals2.entry as ToggleSetting;
					bool flag2 = toggleSettingEntry != null;
					ModSettingWidgetsContainer refers2;
					if (flag2)
					{
						ModSettingWidgetsToggleContainer settingWidgetsToggleContainer = PoolManager.GetObject<ModSettingWidgetsToggleContainer>("ModPanel_SettingEntry_Toggle");
						refers2 = settingWidgetsToggleContainer;
						CToggle toggle = settingWidgetsToggleContainer.toggle;
						toggle.onValueChanged.RemoveAllListeners();
						toggle.isOn = toggleSettingEntry.Value;
						toggle.onValueChanged.AddListener(delegate(bool isOn)
						{
							bool flag9 = isOn != toggleSettingEntry.Value;
							if (flag9)
							{
								CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetSettingChangeState(true, CS$<>8__locals2.CS$<>8__locals1.modInfo);
							}
							toggleSettingEntry.Value = isOn;
						});
						toggle.interactable = !isUpload;
					}
					else
					{
						SliderSetting sliderSettingEntry = CS$<>8__locals2.entry as SliderSetting;
						bool flag3 = sliderSettingEntry != null;
						if (flag3)
						{
							ModSettingWidgetsSliderContainer settingWidgetsSliderContainer = PoolManager.GetObject<ModSettingWidgetsSliderContainer>("ModPanel_SettingEntry_Slider");
							refers2 = settingWidgetsSliderContainer;
							CSlider slider = settingWidgetsSliderContainer.slider;
							slider.onValueChanged.RemoveAllListeners();
							slider.wholeNumbers = true;
							slider.maxValue = (float)sliderSettingEntry.MaxValue;
							slider.minValue = (float)sliderSettingEntry.MinValue;
							slider.value = (float)sliderSettingEntry.Value;
							TextMeshProUGUI curValue = settingWidgetsSliderContainer.curValue;
							curValue.text = sliderSettingEntry.Value.ToString();
							slider.onValueChanged.AddListener(delegate(float val)
							{
								int intVal = (int)val;
								bool flag9 = sliderSettingEntry.Value != intVal;
								if (flag9)
								{
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetSettingChangeState(true, CS$<>8__locals2.CS$<>8__locals1.modInfo);
								}
								sliderSettingEntry.Value = intVal;
								curValue.text = ((int)val).ToString();
							});
							slider.interactable = !isUpload;
						}
						else
						{
							DropdownSetting dropdownSettingEntry = CS$<>8__locals2.entry as DropdownSetting;
							bool flag4 = dropdownSettingEntry != null;
							if (flag4)
							{
								ModSettingWidgetsDropdownContainer settingWidgetsDropdownContainer = PoolManager.GetObject<ModSettingWidgetsDropdownContainer>("ModPanel_SettingEntry_Dropdown");
								refers2 = settingWidgetsDropdownContainer;
								CDropdown dropdown = settingWidgetsDropdownContainer.dropdown;
								dropdown.onValueChanged.RemoveAllListeners();
								dropdown.ClearOptions();
								dropdown.AddOptions(dropdownSettingEntry.Options);
								dropdown.value = dropdownSettingEntry.Value;
								dropdown.onValueChanged.AddListener(delegate(int val)
								{
									bool flag9 = val != dropdownSettingEntry.Value;
									if (flag9)
									{
										CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetSettingChangeState(true, CS$<>8__locals2.CS$<>8__locals1.modInfo);
									}
									dropdownSettingEntry.Value = val;
								});
								dropdown.interactable = !isUpload;
								bool flag5 = this._curToggleKey == UI_ModPanel.PanelToggleKey.CurMod;
								if (flag5)
								{
									this._curModSettingDropdownList.Add(dropdown);
								}
							}
							else
							{
								InputFieldSetting inputFieldSettingEntry = CS$<>8__locals2.entry as InputFieldSetting;
								bool flag6 = inputFieldSettingEntry != null;
								if (!flag6)
								{
									continue;
								}
								ModSettingWidgetsInputFieldContainer settingWidgetsInputFieldContainer = PoolManager.GetObject<ModSettingWidgetsInputFieldContainer>("ModPanel_SettingEntry_InputField");
								refers2 = settingWidgetsInputFieldContainer;
								TMP_InputField inputField = settingWidgetsInputFieldContainer.inputField;
								inputField.onEndEdit.RemoveAllListeners();
								inputField.text = inputFieldSettingEntry.Value;
								((TextMeshProUGUI)inputField.placeholder).text = LocalStringManager.Get(LanguageKey.LK_EventInput_StringHolderTips);
								inputField.onEndEdit.AddListener(delegate(string val)
								{
									bool flag9 = inputFieldSettingEntry.Value != val;
									if (flag9)
									{
										CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetSettingChangeState(true, CS$<>8__locals2.CS$<>8__locals1.modInfo);
									}
									inputFieldSettingEntry.Value = val;
								});
								inputField.interactable = !isUpload;
							}
						}
					}
					TextMeshProUGUI label = refers2.label;
					label.SetText(CS$<>8__locals2.entry.DisplayName, true);
					TooltipInvoker tipsDisplayer = label.GetComponent<TooltipInvoker>();
					tipsDisplayer.PresetParam[0] = CS$<>8__locals2.entry.Description;
					tipsDisplayer.Refresh(false, -1);
					CButton buttonLess = refers2.buttonLess;
					buttonLess.gameObject.SetActive(isUpload);
					bool flag7 = isUpload;
					if (flag7)
					{
						buttonLess.ClearAndAddListener(delegate
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this._isEditingUploadMod = true;
							CS$<>8__locals2.CS$<>8__locals1.<>4__this._tempModSettingEntries.Remove(CS$<>8__locals2.entry);
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.RefreshUploadModSettings();
						});
					}
					CButton buttonEdit = refers2.buttonEdit;
					buttonEdit.gameObject.SetActive(isUpload);
					bool flag8 = isUpload;
					if (flag8)
					{
						buttonEdit.ClearAndAddListener(delegate
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this._isEditingUploadMod = true;
							CS$<>8__locals2.CS$<>8__locals1.<>4__this._editSettingPanel.Show(CS$<>8__locals2.CS$<>8__locals1.<>4__this._tempModSettingEntries, CS$<>8__locals2.entry, null, null);
						});
					}
					this._settingEntriesList.Add(refers2);
					Transform curRow = PoolManager.GetObject<Transform>("ModPanel_Setting_Row");
					curRow.SetParent(settingHolder);
					curRow.SetAsLastSibling();
					curRow.localScale = Vector3.one;
					this._settingRowList.Add(curRow);
					int curRowWidth = 0;
					Transform entryTransform = refers2.transform;
					entryTransform.SetParent(curRow);
					entryTransform.SetAsLastSibling();
					entryTransform.localScale = Vector3.one;
					curRowWidth += refers2.UserInt;
				}
			}
		}
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x00128FC0 File Offset: 0x001271C0
	private void SaveAllSettings()
	{
		bool flag = !this._hasChangedSetting;
		if (!flag)
		{
			ModManager.SaveModSettings(false);
			bool needRestart = this._needRestart;
			if (needRestart)
			{
				this.ShowRestart();
			}
			else
			{
				this.SetSettingChangeState(false, null);
				foreach (ModId modId in ModManager.EnabledMods)
				{
					ModManager.UpdateModSettingsInGame(modId);
				}
			}
		}
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x0012904C File Offset: 0x0012724C
	private void Apply()
	{
		bool flag = this._hasChangedEnable || (this._hasChangedSetting && this._needRestart);
		if (flag)
		{
			this.ShowRestart();
		}
		else
		{
			ModManager.SaveModSettings(false);
			foreach (ModId modId in ModManager.EnabledMods)
			{
				ModManager.UpdateModSettingsInGame(modId);
			}
			this.ClearChangeState();
			this.RefreshApplyButton();
		}
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x001290E4 File Offset: 0x001272E4
	private IEnumerator CoroutineReloadEnabledMods()
	{
		this.ShowMask();
		yield return new WaitForEndOfFrame();
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		string langKey = settings.Language;
		LocalStringManager.Init(langKey);
		yield return new WaitUntil(() => LocalStringManager.ConfigLanguageInitReady);
		Task<ParallelLoopResult> initCfgTask = Task.Run<ParallelLoopResult>(() => Parallel.ForEach<IConfigData>(ConfigCollection.Items, delegate(IConfigData item)
		{
			item.Init();
		}));
		while (!initCfgTask.IsCompleted)
		{
			yield return null;
		}
		bool flag = initCfgTask.Exception != null;
		if (flag)
		{
			throw initCfgTask.Exception;
		}
		RefNameMap.DoQueuedLoadRequests();
		LocalStringManager.Release();
		IEnumerator loader = ModManager.LoadAllEnabledMods();
		while (loader.MoveNext())
		{
			object obj = loader.Current;
			yield return obj;
		}
		yield return new WaitForEndOfFrame();
		GameDataBridge.AllowSendingInitializationMessage();
		while (GameDataBridge.GetGameDataModuleInitializationState() != 3)
		{
			yield return null;
		}
		this.HideMask();
		yield break;
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x001290F4 File Offset: 0x001272F4
	private void OnClickRemoveCurMod()
	{
		bool flag = this._curModId.Source == 1;
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_Remove_Subscribed_Tip_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Remove_Subscribed_Tip_Content);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this.ShowMask();
				this.UnSubscribeItem(this._curModId);
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.UpdateModList));
			}, null, EDialogType.None);
		}
		else
		{
			string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Title);
			string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Content);
			CommonUtils.ShowConfirmDialog(title2, content2, delegate
			{
				ModManager.DeleteLocalMod(this._curModInfo);
				this.UpdateModList();
			}, null, EDialogType.None);
		}
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x00129174 File Offset: 0x00127374
	private void CancelEditCurMod(Action onConfirm, Action onCancel = null)
	{
		bool flag = this._hasChangedSetting || this._hasChangedEnable;
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Content);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this._hasChangedSetting = false;
				this._hasChangedEnable = false;
				this.RefreshApplyButton();
				Action onConfirm3 = onConfirm;
				if (onConfirm3 != null)
				{
					onConfirm3();
				}
			}, onCancel, EDialogType.None);
		}
		else
		{
			Action onConfirm2 = onConfirm;
			if (onConfirm2 != null)
			{
				onConfirm2();
			}
		}
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x001291F0 File Offset: 0x001273F0
	private void OnDisableDependenciesChangedMods(ArgumentBox argumentBox)
	{
		using (List<ModId>.Enumerator enumerator = this._dependenciesChangedList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ModId modId = enumerator.Current;
				int index = this._tempEnabledModIdList.FindIndex((ModId id) => id.FileId == modId.FileId);
				bool flag = index >= 0;
				if (flag)
				{
					this._tempEnabledModIdList.RemoveAt(index);
				}
			}
		}
		this.RefreshCurModList(true);
		this.RefreshEnableChangeState();
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x0012928C File Offset: 0x0012748C
	private void InitWorkshopPanel()
	{
		this._workshopScroll = this.workshopPanel.workshopScroll;
		this._workshopScroll.srcPrefab.gameObject.SetActive(false);
		this._workshopScroll.OnItemRender += this.OnWorkshopRender;
		this._workshopPageSwitch = this.workshopPanel.workshopPageSwitch;
		this._workshopPageSwitch.OnValueChanged = new Action<int>(this.OnWorkshopPageValueChanged);
		this._workshopPageSwitch.SetValueAndRefresh(1);
		this._workshopSearchInputText = string.Empty;
		this._workshopSearchInputField = this.workshopPanel.workshopSearchInputField;
		this._workshopSearchInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnWorkshopSearchInputValueChange));
		this._workshopSearchInputField.SetTextWithoutNotify(string.Empty);
		this._workshopSortToggleGroup = this.workshopPanel.workshopSortToggleGroup;
		this._workshopSortToggleGroup.Init(-1);
		this._workshopSortToggleGroup.OnActiveIndexChange += this.OnWorkshopSortToggleChange;
		this._workshopSortToggleGroup.SetWithoutNotify(0);
		UI_ModPanel.CurWorkshopSortToggleKey = UI_ModPanel.WorkshopSortToggleKey.MostPopular;
		this._workshopTimeToggleGroup = this.workshopPanel.workshopTimeToggleGroup;
		this._workshopTimeToggleGroup.Init(-1);
		this._workshopTimeToggleGroup.OnActiveIndexChange += this.OnWorkshopTimeToggleChange;
		this._workshopTimeToggleGroup.SetWithoutNotify(0);
		UI_ModPanel.CurWorkshopTimeToggleKey = UI_ModPanel.WorkshopTimeToggleKey.All;
		this._workshopDetailToggleGroup = this.workshopPanel.modDetailToggleGroup;
		this._workshopDetailToggleGroup.Init(-1);
		this._workshopDetailToggleGroup.OnActiveIndexChange += this.OnModDetailToggleChange;
		this.ShowWorkshopModDetailPanel(false);
		this.ShowWorkshopModSharePanel(false);
		this.workshopPanel.buttonClose.ClearAndAddListener(delegate
		{
			this.ShowWorkshopModDetailPanel(false);
			this.buttonClosePopup.gameObject.SetActive(true);
		});
		CButton buttonShare = this.workshopPanel.buttonShare;
		buttonShare.ClearAndAddListener(new Action(this.OnClickShareMod));
		this._workshopTagDropdown = this.workshopPanel.tagDropdown;
		this._workshopTagDropdown.ClearOptions();
		this._workshopTagDropdown.onValueChanged.RemoveAllListeners();
		this._workshopTagDropdown.onValueChanged.AddListener(delegate(int value)
		{
			List<ModId> curWorkshopList = SteamManager.CurWorkshopList;
			if (curWorkshopList != null)
			{
				curWorkshopList.Clear();
			}
			this.RefreshWorkshopModList(true, true);
		});
		this._workshopTagDropdown.AddOptions(SteamManager.AllTagList);
		this._workshopTagDropdown.SetValueWithoutNotify(0);
		this.workshopPanel.buttonOpenWeb.ClearAndAddListener(delegate
		{
			string pchURL = string.Format("https://steamcommunity.com/sharedfiles/filedetails/?id={0}", this._selectedWorkshopModInfo.ModId.FileId);
			SteamFriends.ActivateGameOverlayToWebPage(pchURL, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
		});
		this._subscribeDependenceDialog = this.workshopPanel.subscribeDependenceDialog;
		this._subscribeDependenceDialog.Init();
		this._workShopListCoverImageOriginSize = this._workshopScroll.srcPrefab.GetComponent<ModWorkshopTemplate>().coverImage.rectTransform.rect.size;
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x00129540 File Offset: 0x00127740
	private void OnClickShareMod()
	{
		string url = string.Format("https://steamcommunity.com/sharedfiles/filedetails/?id={0}", this._selectedWorkshopModInfo.ModId.FileId);
		TMP_InputField shareInputField = this.workshopPanel.shareInputField;
		shareInputField.text = url;
		shareInputField.onSelect.RemoveAllListeners();
		shareInputField.onSelect.AddListener(delegate(string str)
		{
			TextEditor te = new TextEditor();
			te.text = url;
			te.SelectAll();
			te.Copy();
			this.ShowShareTip(true);
		});
		this.ShowShareTip(false);
		this.ShowWorkshopModSharePanel(true);
		CButton buttonCloseShare = this.workshopPanel.buttonCloseShare;
		buttonCloseShare.ClearAndAddListener(delegate
		{
			this.ShowWorkshopModSharePanel(false);
		});
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x001295EC File Offset: 0x001277EC
	private void OnWorkshopRender(int index, GameObject go)
	{
		UI_ModPanel.<>c__DisplayClass195_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass195_0();
		CS$<>8__locals1.<>4__this = this;
		int realIndex = (this._workshopPageSwitch.Value - 1) * 48 + index;
		CS$<>8__locals1.modId = this._workshopModIdList[realIndex];
		CS$<>8__locals1.modInfo = ModManager.GetModInfo(CS$<>8__locals1.modId);
		CS$<>8__locals1.refers = go.GetComponent<ModWorkshopTemplate>();
		TextMeshProUGUI titleText = CS$<>8__locals1.refers.title;
		CButton subscribeButton = CS$<>8__locals1.refers.subscribeButton;
		CButton button = CS$<>8__locals1.refers.button;
		TextMeshProUGUI ratingText = CS$<>8__locals1.refers.ratingAmountLabel;
		RectTransform starLayout = CS$<>8__locals1.refers.starLayout;
		bool flag = CS$<>8__locals1.modInfo == null;
		if (flag)
		{
			titleText.SetText(string.Empty, true);
			subscribeButton.gameObject.SetActive(false);
			button.interactable = false;
			ratingText.SetText(string.Empty, true);
			CRawImage coverImg = CS$<>8__locals1.refers.coverImage;
			coverImg.texture = this.popupModPanelLoadError0;
			coverImg.enabled = true;
			this.ResetCoverSize(CS$<>8__locals1.refers);
			starLayout.gameObject.SetActive(false);
			GLog.TagWarn("UI_ModPanel", string.Format("OnWorkshopRender Not Found Mod {0}", CS$<>8__locals1.modId), Array.Empty<object>());
		}
		else
		{
			titleText.SetText(CS$<>8__locals1.modInfo.Title, true);
			TooltipInvoker titleTip = titleText.GetComponent<TooltipInvoker>();
			bool flag2 = titleTip.PresetParam == null || titleTip.PresetParam.Length == 0;
			if (flag2)
			{
				titleTip.PresetParam = new string[1];
			}
			titleTip.PresetParam[0] = CS$<>8__locals1.modInfo.Title;
			int starCount = CS$<>8__locals1.modInfo.Score;
			starLayout.gameObject.SetActive(true);
			for (int i = 0; i < starLayout.childCount; i++)
			{
				string icon = (i < starCount) ? UI_ModPanel.StarActiveIcon : UI_ModPanel.StarInactiveIcon;
				starLayout.GetChild(i).GetComponent<CImage>().SetSprite(icon, false, null);
			}
			ratingText.text = LocalStringManager.GetFormat("LK_Steam_Mod_Rating_Count", CS$<>8__locals1.modInfo.VoteCount);
			this.RefreshCoverImage(CS$<>8__locals1.refers, CS$<>8__locals1.modInfo, false);
			subscribeButton.gameObject.SetActive(true);
			subscribeButton.ClearAndAddListener(new Action(CS$<>8__locals1.<OnWorkshopRender>g__OnClickSubscribeMod|0));
			CS$<>8__locals1.refers.checkMark.SetActive(CS$<>8__locals1.modInfo.IsSubscribed);
			button.interactable = true;
			button.ClearAndAddListener(new Action(CS$<>8__locals1.<OnWorkshopRender>g__OnClickWorkshopMod|2));
		}
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x00129884 File Offset: 0x00127A84
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

	// Token: 0x06002844 RID: 10308 RVA: 0x001298D4 File Offset: 0x00127AD4
	private void SubscribeItem(ModId modId, bool checkDependencies = true, Action onConfirm = null)
	{
		UI_ModPanel.<>c__DisplayClass197_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass197_0();
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
				this.ShowMask();
				ModManager.UpdateTargetItems(modInfo.Dependencies, delegate(Dictionary<ModId, bool> dependenciesChangeStateDict)
				{
					CS$<>8__locals1.<>4__this.HideMask();
					List<ModId> dependenceList = dependenciesChangeStateDict.Keys.ToList<ModId>();
					CS$<>8__locals1.<>4__this._subscribeDependenceDialog.Show(dependenceList, delegate(bool only)
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

	// Token: 0x06002845 RID: 10309 RVA: 0x0012995B File Offset: 0x00127B5B
	private void DoSubscribeItem(ModId modId)
	{
		ModManager.SubscribeItem(modId, true);
		this.DownloadMod(modId);
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x00129970 File Offset: 0x00127B70
	private void DownloadMod(ModId modId)
	{
		PublishedFileId_t publishedFileId = new PublishedFileId_t(modId.FileId);
		SteamUGC.DownloadItem(publishedFileId, true);
		this._downLoadingItemList.Add(modId);
		base.StopCoroutine(this.CorUpdateDownloadingMod());
		base.StartCoroutine(this.CorUpdateDownloadingMod());
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x001299BA File Offset: 0x00127BBA
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
				bool flag4 = this._curToggleKey == UI_ModPanel.PanelToggleKey.CurMod;
				if (flag4)
				{
					this.UpdateModList();
				}
				else
				{
					this._curModPanelNeedUpdate = true;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x001299C9 File Offset: 0x00127BC9
	private void ShowWorkshopModSharePanel(bool isShow)
	{
		this.WorkshopSharePanel.SetActive(isShow);
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x001299D8 File Offset: 0x00127BD8
	private void ShowShareTip(bool isShow)
	{
		this.workshopPanel.shareTip.SetActive(isShow);
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x001299EC File Offset: 0x00127BEC
	private void ShowWorkshopModDetailPanel(bool isShow)
	{
		this.WorkshopDetailPanel.SetActive(isShow);
		this.WorkshopListPanel.SetActive(!isShow);
		bool flag = !isShow;
		if (!flag)
		{
			CButton buttonManage = this.workshopPanel.buttonManage;
			buttonManage.gameObject.SetActive(this._selectedWorkshopModInfo.IsSubscribed);
			buttonManage.ClearAndAddListener(delegate
			{
				this.ShowWorkshopModDetailPanel(false);
				this.toggleGroup.Set(UI_ModPanel.PanelToggleKey.CurMod.ToInt(), false);
				this._curModPanelNeedScrollToSelectedWorkshopMod = true;
				this.UpdateModList();
			});
			CButton buttonSubscribe = this.workshopPanel.buttonSubscribe;
			buttonSubscribe.ClearAndAddListener(delegate
			{
				bool isSubscribed = this._selectedWorkshopModInfo.IsSubscribed;
				if (isSubscribed)
				{
					this.UnSubscribeItem(this._selectedWorkshopModInfo.ModId);
					this.<ShowWorkshopModDetailPanel>g__OnSubscribeStateChanged|203_2();
				}
				else
				{
					this.SubscribeItem(this._selectedWorkshopModInfo.ModId, true, new Action(this.<ShowWorkshopModDetailPanel>g__OnSubscribeStateChanged|203_2));
				}
			});
			GameObject checkMark = this.workshopPanel.checkMark;
			checkMark.SetActive(this._selectedWorkshopModInfo.IsSubscribed);
		}
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x00129A98 File Offset: 0x00127C98
	private void RefreshItemVoteButton(ModId modId)
	{
		UI_ModPanel.<>c__DisplayClass204_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass204_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.fileId = new PublishedFileId_t(modId.FileId);
		CButton buttonVoteUp = this.workshopPanel.buttonVoteUp;
		CButton buttonVoteDown = this.workshopPanel.buttonVoteDown;
		buttonVoteUp.ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshItemVoteButton>g__OnClickVoteUp|2));
		buttonVoteDown.ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshItemVoteButton>g__OnClickVoteDown|1));
		CallResult<GetUserItemVoteResult_t> callResult = CallResult<GetUserItemVoteResult_t>.Create(null);
		SteamAPICall_t steamAPICall = SteamUGC.GetUserItemVote(CS$<>8__locals1.fileId);
		callResult.Set(steamAPICall, delegate(GetUserItemVoteResult_t t, bool failure)
		{
			if (!failure)
			{
				CS$<>8__locals1.<>4__this.SetButtonVoteActive(t.m_bVotedUp, t.m_bVotedDown);
			}
		});
	}

	// Token: 0x0600284C RID: 10316 RVA: 0x00129B30 File Offset: 0x00127D30
	private void SetButtonVoteActive(bool isUp, bool isDown)
	{
		CButton buttonVoteUp = this.workshopPanel.buttonVoteUp;
		CButton buttonVoteDown = this.workshopPanel.buttonVoteDown;
		Transform transform = buttonVoteUp.transform.Find("Active");
		if (transform != null)
		{
			transform.gameObject.SetActive(isUp);
		}
		Transform transform2 = buttonVoteDown.transform.Find("Active");
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(isDown);
		}
	}

	// Token: 0x0600284D RID: 10317 RVA: 0x00129B9A File Offset: 0x00127D9A
	private void OnModDetailToggleChange(int newTog, int oldTog)
	{
		this.UpdateModDescriptionOrUpdateLog((UI_ModPanel.WorkshopDetailToggleKey)newTog);
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x00129BA8 File Offset: 0x00127DA8
	private void UpdateModDescriptionOrUpdateLog(UI_ModPanel.WorkshopDetailToggleKey key)
	{
		bool flag = this._selectedWorkshopModInfo == null;
		if (!flag)
		{
			if (key != UI_ModPanel.WorkshopDetailToggleKey.Description)
			{
				if (key != UI_ModPanel.WorkshopDetailToggleKey.UpdateLog)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.workshopPanel.label.text = LocalStringManager.Get(LanguageKey.LK_Mod_UpdateLog);
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				bool flag2 = this._selectedWorkshopModInfo.UpdateLogList != null;
				if (flag2)
				{
					foreach (ModInfoWithDisplayData.UpdateLog updateLog in this._selectedWorkshopModInfo.UpdateLogList)
					{
						string time = this.GetTimeString((uint)updateLog.Timestamp);
						sb.Append(time);
						bool flag3 = !time.IsNullOrEmpty();
						if (flag3)
						{
							sb.AppendLine();
							sb.AppendLine();
						}
						string log = UI_ModPanel.GetUpdateLog(updateLog.LogList);
						sb.Append(log);
						bool flag4 = !log.IsNullOrEmpty();
						if (flag4)
						{
							sb.AppendLine();
							sb.AppendLine();
						}
						sb.Append("--------------------------------------------------");
						sb.AppendLine();
						sb.AppendLine();
					}
				}
				string content = sb.ToString();
				this.workshopPanel.content.text = content;
			}
			else
			{
				this.workshopPanel.label.text = LocalStringManager.Get(LanguageKey.LK_Mod_Info);
				this.workshopPanel.content.text = this._selectedWorkshopModInfo.Description;
			}
		}
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x00129D50 File Offset: 0x00127F50
	private void OnWorkshopSearchInputValueChange(string value)
	{
		bool flag = this._workshopSearchInputText == value;
		if (!flag)
		{
			this._workshopSearchInputText = value;
			SteamManager.Clear();
			this.RefreshWorkshopModList(true, true);
		}
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x00129D88 File Offset: 0x00127F88
	private void RefreshWorkshopModList(bool refreshData = true, bool forceUpdate = false)
	{
		if (refreshData)
		{
			this.RefreshPage(forceUpdate);
		}
		else
		{
			int sortToggleKey = this._workshopSortToggleGroup.GetActiveIndex();
			this._workshopSortToggleGroup.Set(sortToggleKey, true);
		}
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x00129DC4 File Offset: 0x00127FC4
	private void RefreshPage(bool forceUpdate = false)
	{
		UI_ModPanel.<>c__DisplayClass210_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass210_0();
		CS$<>8__locals1.<>4__this = this;
		int maxItemIndex = this._workshopPageSwitch.Value * 48;
		int minItemIndex = (this._workshopPageSwitch.Value - 1) * 48;
		CS$<>8__locals1.maxSteamPageIndex = Mathf.Clamp(Mathf.CeilToInt((float)maxItemIndex / 50f), 1, this._steamPageMaxCount);
		int minSteamPageIndex = Mathf.Clamp(Mathf.CeilToInt((float)minItemIndex / 50f), 1, this._steamPageMaxCount);
		CS$<>8__locals1.curWorkshopList = SteamManager.CurWorkshopList;
		bool minNeedUpdate = CS$<>8__locals1.<RefreshPage>g__CheckNeedUpdate|0(minItemIndex);
		bool maxNeedUpdate = CS$<>8__locals1.<RefreshPage>g__CheckNeedUpdate|0(maxItemIndex);
		bool needUpdate = CS$<>8__locals1.curWorkshopList == null || minNeedUpdate || maxNeedUpdate;
		bool flag = forceUpdate || needUpdate;
		if (flag)
		{
			UI_ModPanel.<>c__DisplayClass210_1 CS$<>8__locals2 = new UI_ModPanel.<>c__DisplayClass210_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			this.ShowMask();
			CS$<>8__locals2.searchText = this.GetSearchInputValue(this._workshopSearchInputField);
			string searchTag = SteamManager.GetTagKey(SteamManager.AllTagList[this._workshopTagDropdown.value]);
			CS$<>8__locals2.tags = new List<string>
			{
				searchTag
			};
			CS$<>8__locals2.startPage = minSteamPageIndex;
			CS$<>8__locals2.<RefreshPage>g__Send|2();
		}
		else
		{
			CS$<>8__locals1.<RefreshPage>g__Refresh|1(CS$<>8__locals1.curWorkshopList);
		}
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x00129EF1 File Offset: 0x001280F1
	private void OnWorkshopTimeToggleChange(int newTog, int oldTog)
	{
		UI_ModPanel.CurWorkshopTimeToggleKey = (UI_ModPanel.WorkshopTimeToggleKey)newTog;
		this.RefreshWorkshopModList(true, true);
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x00129F04 File Offset: 0x00128104
	private void OnWorkshopSortToggleChange(int newTog, int oldTog)
	{
		UI_ModPanel.CurWorkshopSortToggleKey = (UI_ModPanel.WorkshopSortToggleKey)newTog;
		this._workshopTimeToggleGroup.gameObject.SetActive(UI_ModPanel.CurWorkshopSortToggleKey == UI_ModPanel.WorkshopSortToggleKey.MostPopular);
		bool flag = UI_ModPanel.CurWorkshopSortToggleKey == UI_ModPanel.WorkshopSortToggleKey.MostPopular;
		if (flag)
		{
			bool flag2 = oldTog >= 0 && oldTog != newTog;
			if (flag2)
			{
				this._workshopTimeToggleGroup.Set(UI_ModPanel.WorkshopTimeToggleKey.All.ToInt(), true);
			}
		}
		else
		{
			this.RefreshWorkshopModList(true, true);
		}
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x00129F7A File Offset: 0x0012817A
	private void OnWorkshopPageValueChanged(int index)
	{
		SteamManager.Clear();
		this.RefreshPage(false);
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x00129F8C File Offset: 0x0012818C
	private void OnWorkshopModPreviewImageHasDownloaded(ArgumentBox argumentBox)
	{
		bool flag = this._curToggleKey != UI_ModPanel.PanelToggleKey.WorkshopMod;
		if (!flag)
		{
			ModInfoWithDisplayData modInfo;
			bool flag2 = argumentBox.Get<ModInfoWithDisplayData>("ModInfo", out modInfo);
			if (flag2)
			{
				int index = this._workshopModIdList.IndexOf(modInfo.ModId);
				bool flag3 = index / 48 == this._workshopPageSwitch.Value - 1;
				if (flag3)
				{
					GameObject refers = this._workshopScroll.GetActiveCell(index % 48);
					bool flag4 = refers;
					if (flag4)
					{
						this.RefreshCoverImage(refers.GetComponent<ModWorkshopTemplate>(), modInfo, false);
					}
				}
				bool flag5 = this.WorkshopDetailPanel.activeSelf && this._selectedWorkshopModInfo != null && modInfo.ModId.Equals(this._selectedWorkshopModInfo.ModId);
				if (flag5)
				{
					this.RefreshBasicInfo(this.workshopPanel.basicInfo, modInfo);
				}
			}
		}
	}

	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x06002856 RID: 10326 RVA: 0x0012A06B File Offset: 0x0012826B
	private TMP_InputField NameInputField
	{
		get
		{
			return this._uploadBasicInfoRefers.nameInputField;
		}
	}

	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x06002857 RID: 10327 RVA: 0x0012A078 File Offset: 0x00128278
	private TMP_InputField VersionInputField
	{
		get
		{
			return this._uploadBasicInfoRefers.versionInputField;
		}
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x0012A088 File Offset: 0x00128288
	private void InitUploadPanel()
	{
		this._targetDetailImageToggleKey = -1;
		this._uploadBasicInfoRefers = this.uploadModPanel.basicInfo;
		this._uploadModScroll = this.uploadModPanel.uploadModScroll;
		this._uploadModScroll.srcPrefab.gameObject.SetActive(false);
		this._uploadModScroll.RemoveOnScrollEvent(new Action(this.OnUploadModScroll));
		this._uploadModScroll.AddOnScrollEvent(new Action(this.OnUploadModScroll));
		this._uploadModScroll.OnItemRender += this.OnUploadModRender;
		this._uploadModPageMaxCount = this._uploadModScroll.GetPageMaxCount();
		this._uploadModScrollToggleGroup = this._uploadModScroll.GetComponent<CToggleGroup>();
		this._uploadModScrollToggleGroup.OnActiveIndexChange += this.OnUploadModToggleChange;
		this._uploadModPageSwitch = this.uploadModPanel.uploadModPageSwitch;
		this._uploadModPageSwitch.OnValueChanged = new Action<int>(this.OnUploadModPageValueChanged);
		this._uploadModPageSwitch.SetValueAndRefresh(1);
		this._uploadModSearchInputField = this.uploadModPanel.uploadModSearchInputField;
		this._uploadModSearchInputField.onValueChanged.RemoveAllListeners();
		this._uploadModSearchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnUploadModSearchInputValueChange));
		this._uploadModSearchInputField.SetTextWithoutNotify(string.Empty);
		this._buttonMoreTag = this._uploadBasicInfoRefers.buttonMore;
		this._buttonMoreTag.ClearAndAddListener(new Action(this.OnClickAddTag));
		this.NameInputField.onValueChanged.RemoveAllListeners();
		this.NameInputField.onValueChanged.AddListener(delegate(string value)
		{
			bool flag = this._curEditModInfo != null && value != this._curEditModInfo.Title;
			if (flag)
			{
				this._isEditingUploadMod = true;
			}
			this.RefreshUploadWarningMark();
		});
		TMP_InputField versionInputField = this._uploadBasicInfoRefers.versionInputField;
		versionInputField.onValueChanged.RemoveAllListeners();
		versionInputField.onValueChanged.AddListener(delegate(string value)
		{
			bool flag = this._curEditModInfo != null && ModManager.VersionStringToUlong(value) != this._curEditModInfo.ModId.Version;
			if (flag)
			{
				this._isEditingUploadMod = true;
			}
		});
		this._uploadVisibilityDropdown = this._uploadBasicInfoRefers.visibilityDropdown;
		this._uploadVisibilityDropdown.ClearOptions();
		this._uploadVisibilityDropdown.AddOptions(SteamManager.VisibilityOptionList);
		this._uploadVisibilityDropdown.onValueChanged.RemoveAllListeners();
		this._uploadVisibilityDropdown.onValueChanged.AddListener(delegate(int value)
		{
			bool flag = this._curEditModInfo != null && value != this._curEditModInfo.Visibility.ToInt();
			if (flag)
			{
				this._isEditingUploadMod = true;
			}
		});
		this._buttonAdd = this.uploadModPanel.buttonAdd;
		this._buttonAdd.ClearAndAddListener(new Action(this.OnClickAddMod));
		this.ButtonCreate.ClearAndAddListener(new Action(this.OnClickCreate));
		this.ButtonOpenUpload.ClearAndAddListener(new Action(this.OnClickOpenUpload));
		this.ButtonCreateFormDirectory.ClearAndAddListener(new Action(this.OnClickCreateFormDirectory));
		this._buttonProgram = this._uploadBasicInfoRefers.buttonProgram;
		this._buttonProgram.ClearAndAddListener(new Action(this.OnClickSetProgram));
		this._uploadDescriptionAreaRefers = this.uploadModPanel.descriptionArea;
		this._uploadDescriptionRoot = this._uploadDescriptionAreaRefers.descriptionRoot;
		this._uploadUpdateLogRoot = this._uploadDescriptionAreaRefers.updateLogRoot;
		this._uploadDescriptionInputField = this._uploadDescriptionAreaRefers.descriptionInputField;
		this._uploadUpdateLogContent = this._uploadDescriptionAreaRefers.updateLogContent;
		CToggleGroup modDescriptionToggleGroup = this._uploadDescriptionAreaRefers.modDescriptionToggleGroup;
		this._uploadDescriptionInputField.onValueChanged.RemoveAllListeners();
		this._uploadDescriptionInputField.onValueChanged.AddListener(delegate(string value)
		{
			bool flag = this._curEditModInfo != null && value != this._curEditModInfo.Description;
			if (flag)
			{
				this._isEditingUploadMod = true;
			}
			this.RefreshUploadWarningMark();
		});
		this._setProgramPanel = this.uploadModPanel.setProgramPanel;
		this._setProgramPanel.Init();
		this._setProgramPanel.Hide();
		this._editSettingPanel = this.uploadModPanel.editSettingPanel;
		this._editSettingPanel.Init();
		this._editSettingPanel.Hide();
		this._editUpdateLogPanel = this.uploadModPanel.editUpdateLogPanel;
		this._editUpdateLogPanel.Init();
		this._editUpdateLogPanel.Hide();
		this._modUploadConfirmDialog = this.uploadModPanel.uploadConfirmDialog;
		this._setDependencePanel = this.uploadModPanel.setDependencePanel;
		this._setDependencePanel.Init();
		this._modDirectlyUploadPanel = this.uploadModPanel.directlyUploadPanel;
		this._modDirectlyUploadPanel.Hide();
		this.uploadModPanel.buttonAddSetting.ClearAndAddListener(new Action(this.OnClickAddSetting));
		this.InitVersionInputField();
		this.ClearUploadModInfo();
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x0012A4DC File Offset: 0x001286DC
	private void InitVersionInputField()
	{
		GameObject versionInputFieldLayout = this.uploadModPanel.versionInputFieldLayout;
		this._versionInputFieldList.Clear();
		versionInputFieldLayout.GetComponentsInChildren<TMP_InputField>(true, this._versionInputFieldList);
		for (int i = 0; i < this._versionInputFieldList.Count; i++)
		{
			int index = i;
			TMP_InputField inputField = this._versionInputFieldList[i];
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(delegate(string value)
			{
				bool flag = inputField.text.Length == inputField.characterLimit;
				if (flag)
				{
					base.<InitVersionInputField>g__MoveToNext|3();
				}
				bool flag2 = this._curEditModInfo != null && this.GetVersionFromInputFieldList() != this._curEditModInfo.ModId.Version;
				if (flag2)
				{
					this._isEditingUploadMod = true;
				}
				this.RefreshUploadWarningMark();
			});
			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(delegate(string value)
			{
				bool flag = value.IsNullOrEmpty();
				if (flag)
				{
					inputField.SetTextWithoutNotify(0.ToString());
				}
			});
			inputField.onSubmit.RemoveAllListeners();
			inputField.onSubmit.AddListener(delegate(string value)
			{
				base.<InitVersionInputField>g__MoveToNext|3();
			});
		}
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x0012A5E4 File Offset: 0x001287E4
	private void Update()
	{
		bool keyDown = Input.GetKeyDown(KeyCode.Backspace);
		if (keyDown)
		{
			int focusedIndex = this._versionInputFieldList.FindIndex((TMP_InputField f) => f.isFocused);
			bool flag = focusedIndex > 0;
			if (flag)
			{
				TMP_InputField inputField = this._versionInputFieldList[focusedIndex];
				int value;
				int.TryParse(inputField.text, out value);
				bool flag2 = value == 0;
				if (flag2)
				{
					int lastIndex = focusedIndex - 1;
					GameObject lastObj = this._versionInputFieldList[lastIndex].gameObject;
					EventSystem.current.SetSelectedGameObject(lastObj);
				}
			}
		}
		else
		{
			bool flag3 = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.KeypadPeriod);
			if (flag3)
			{
				int focusedIndex2 = this._versionInputFieldList.FindIndex((TMP_InputField f) => f.isFocused);
				bool flag4 = focusedIndex2 >= 0;
				if (flag4)
				{
					int nextIndex = (focusedIndex2 + 1) % this._versionInputFieldList.Count;
					TMP_InputField nextInputField = this._versionInputFieldList[nextIndex];
					EventSystem.current.SetSelectedGameObject(nextInputField.gameObject);
				}
			}
		}
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x0012A720 File Offset: 0x00128920
	private ulong GetVersionFromInputFieldList()
	{
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		for (int i = 0; i < this._versionInputFieldList.Count; i++)
		{
			string value = this._versionInputFieldList[i].text;
			sb.Append(value);
			bool flag = i < this._versionInputFieldList.Count - 1;
			if (flag)
			{
				sb.Append(".");
			}
		}
		string versionString = sb.ToString();
		EasyPool.Free<StringBuilder>(sb);
		return ModManager.VersionStringToUlong(versionString);
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x0012A7AC File Offset: 0x001289AC
	private void OnClickAddMod()
	{
		this.CancelEditUploadMod(delegate
		{
			this.AddMod();
			this._uploadModScroll.ScrollToEnd();
		}, null);
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x0012A7C4 File Offset: 0x001289C4
	private void AddMod()
	{
		this.ClearUploadModInfo();
		ModId modId = default(ModId);
		this._uploadModIdList.Add(modId);
		this.ButtonCreate.gameObject.SetActive(true);
		this.ButtonOpenUpload.gameObject.SetActive(true);
		this.RefreshUploadModList(false, false, false);
		this._buttonAdd.interactable = false;
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x0012A82C File Offset: 0x00128A2C
	private void OnClickCreate()
	{
		this.ButtonCreate.gameObject.SetActive(false);
		this.ButtonOpenUpload.gameObject.SetActive(false);
		this._isEditingUploadMod = true;
		ModId modId = default(ModId);
		ModInfoWithDisplayData modInfo = new ModInfoWithDisplayData
		{
			ModId = modId
		};
		this.UpdateUploadModInfo(modInfo);
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x0012A884 File Offset: 0x00128A84
	private void OnClickCreateFormDirectory()
	{
		bool flag = this._curEditModInfo.DirectoryName.IsNullOrEmpty();
		if (flag)
		{
			this.<OnClickCreateFormDirectory>g__Action|226_0();
		}
		else
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_CreateFromDirectory_Warning_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_CreateFromDirectory_Warning_Content);
			CommonUtils.ShowConfirmDialog(title, content, new Action(this.<OnClickCreateFormDirectory>g__Action|226_0), null, EDialogType.None);
		}
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x0012A8E1 File Offset: 0x00128AE1
	private void OnClickOpenUpload()
	{
		this._modDirectlyUploadPanel.Show(delegate(ModInfoWithDisplayData modInfo, string changeNote)
		{
			this._curEditModInfo = modInfo;
			this._tempModUpdateLogList.Clear();
			bool flag = !changeNote.IsNullOrEmpty();
			if (flag)
			{
				this._tempModUpdateLogList.Add(changeNote);
			}
			this.UploadMod(false);
		});
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x0012A8FC File Offset: 0x00128AFC
	private void OnClickSetProgram()
	{
		this._setProgramPanel.Show(this._curEditModInfo, this._tempFrontendPlugins, this._tempBackendPlugins, new Action(this.SaveMod));
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x0012A929 File Offset: 0x00128B29
	private void OnClickAddSetting()
	{
		this._isEditingUploadMod = true;
		this._editSettingPanel.Show(this._tempModSettingEntries, null, null, null);
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x0012A948 File Offset: 0x00128B48
	private void OnClickDelete()
	{
		bool flag = ModManager.UploadedMods.Exists((ModId u) => u.FileId == this._curEditModInfo.ModId.FileId);
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Uploaded_Tip_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Uploaded_Tip_Content);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this._isEditingUploadMod = false;
				ModManager.UnSubscribeItem(this._curEditModInfo.ModId);
				ModManager.DeleteUploadedMod(this._curEditModInfo.ModId);
				ModManager.UpdateModList(delegate
				{
					this.RefreshUploadModList(true, true, false);
					this.UpdateUploadModInfo(this._curEditModInfo);
				});
			}, null, EDialogType.None);
		}
		else
		{
			string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Title);
			string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Content);
			CommonUtils.ShowConfirmDialog(title2, content2, delegate
			{
				this._isEditingUploadMod = false;
				ModManager.DeleteLocalMod(this._curEditModInfo);
				ModManager.UpdateModList(delegate
				{
					this.RefreshUploadModList(true, true, false);
					this.ClearUploadModInfo();
				});
			}, null, EDialogType.None);
		}
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x0012A9D0 File Offset: 0x00128BD0
	private void OnUploadModRender(int index, GameObject go)
	{
		CToggle toggle = go.GetComponent<CToggle>();
		List<CToggle> toggleList = this._uploadModScrollToggleGroup.GetAll();
		bool flag = !toggleList.Contains(toggle);
		if (flag)
		{
			this._uploadModScrollToggleGroup.Add(toggle);
		}
		ModUploadTemplate refers = go.GetComponent<ModUploadTemplate>();
		ModId modId = this._uploadModIdList[index];
		bool flag2 = !modId.IsValid;
		if (flag2)
		{
			refers.title.SetText(LocalStringManager.Get(LanguageKey.LK_Mod_Create), true);
			this._uploadModScrollToggleGroup.Set(index, true);
			refers.steam.SetActive(false);
			refers.external.SetActive(true);
			GLog.TagWarn("UI_ModPanel", string.Format("OnUploadModRender Not Found Mod {0}", modId), Array.Empty<object>());
		}
		else
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			bool flag3 = modInfo == null;
			if (!flag3)
			{
				bool isSteam = ModManager.UploadedMods.Exists((ModId id) => id.FileId == modId.FileId);
				refers.steam.SetActive(isSteam);
				refers.external.SetActive(!isSteam);
				TextMeshProUGUI titleText = refers.title;
				titleText.SetText(modInfo.Title, true);
				TooltipInvoker titleTip = titleText.GetComponent<TooltipInvoker>();
				bool flag4 = titleTip.PresetParam == null || titleTip.PresetParam.Length == 0;
				if (flag4)
				{
					titleTip.PresetParam = new string[1];
				}
				titleTip.PresetParam[0] = modInfo.Title;
			}
		}
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x0012AB5C File Offset: 0x00128D5C
	private void OnUploadModPageValueChanged(int index)
	{
		bool isAdd = index > this._lastUploadModPageSwitchValue;
		this._lastUploadModPageSwitchValue = index;
		int targetIndex = this._uploadModPageMaxCount * (isAdd ? index : (index - 1));
		bool flag = index < this._uploadModPageSwitch.MaxValue;
		if (flag)
		{
			targetIndex--;
		}
		targetIndex = Mathf.Max(0, targetIndex);
		this._uploadModScroll.Refresh(targetIndex);
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x0012ABB8 File Offset: 0x00128DB8
	private void OnUploadModScroll()
	{
		float curValue = this._uploadModScroll.Scroll.ScrollBar.value * (float)this._uploadModScroll.CurrentDataCount;
		float value = curValue / (float)this._uploadModPageMaxCount;
		this._uploadModPageSwitch.SetValueAndRefresh(Mathf.CeilToInt(value));
		this._lastUploadModPageSwitchValue = this._uploadModPageSwitch.Value;
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x0012AC18 File Offset: 0x00128E18
	private void OnUploadModToggleChange(int newTog, int oldTog)
	{
		bool flag = oldTog >= 0;
		if (flag)
		{
			this._uploadModScroll.RefreshCell(oldTog);
		}
		bool flag2 = newTog >= 0;
		if (flag2)
		{
			this._uploadModScroll.RefreshCell(newTog);
		}
		bool flag3 = newTog >= 0;
		if (flag3)
		{
			ModId modId = this._uploadModIdList[newTog];
			bool isValid = modId.IsValid;
			if (isValid)
			{
				this.CancelEditUploadMod(delegate
				{
					ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
					this.UpdateUploadModInfo(modInfo);
					this.RefreshUploadModList(true, false, false);
				}, delegate
				{
					bool flag4 = oldTog >= 0;
					if (flag4)
					{
						this._uploadModScrollToggleGroup.SetWithoutNotify(oldTog);
					}
				});
			}
		}
		else
		{
			this.ClearUploadModInfo();
		}
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x0012ACE8 File Offset: 0x00128EE8
	private void OnUploadModSearchInputValueChange(string value)
	{
		this.ClearUploadModInfo();
		string inputValue = this.GetSearchInputValue(this._uploadModSearchInputField);
		bool flag = inputValue.IsNullOrEmpty();
		if (flag)
		{
			this.RefreshUploadModList(true, false, false);
		}
		else
		{
			this._uploadModIdList.Clear();
			foreach (ModId modId in this._originUploadModIdList)
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				bool flag2 = modInfo.Title.Contains(inputValue);
				if (flag2)
				{
					this._uploadModIdList.Add(modId);
				}
			}
			this.RefreshUploadModList(false, false, false);
		}
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x0012ADA4 File Offset: 0x00128FA4
	private void RefreshUploadModList(bool refreshData = true, bool initData = false, bool initScrollBar = false)
	{
		UI_ModPanel.<>c__DisplayClass236_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass236_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.initScrollBar = initScrollBar;
		if (initData)
		{
			this.ShowMask();
			ModManager.UpdateUploadedItems(delegate(Dictionary<ModId, bool> _)
			{
				CS$<>8__locals1.<>4__this._originUploadModIdList.Clear();
				CS$<>8__locals1.<>4__this._originUploadModIdList.AddRange(ModManager.ExternalMods);
				CS$<>8__locals1.<>4__this._uploadModIdList.Clear();
				CS$<>8__locals1.<>4__this._uploadModIdList.AddRange(CS$<>8__locals1.<>4__this._originUploadModIdList);
				base.<RefreshUploadModList>g__Refresh|1();
				CS$<>8__locals1.<>4__this.HideMask();
			});
		}
		else
		{
			if (refreshData)
			{
				this._uploadModIdList.Clear();
				this._uploadModIdList.AddRange(this._originUploadModIdList);
			}
			CS$<>8__locals1.<RefreshUploadModList>g__Refresh|1();
		}
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x0012AE18 File Offset: 0x00129018
	private void OnUploadModScrollRenderEnd()
	{
		int index = this._uploadModScrollToggleGroup.GetActiveIndex();
		this._uploadModScroll.ScrollTo(index, 0.3f);
		this._uploadModScroll.OnRenderEnd -= this.OnUploadModScrollRenderEnd;
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x0012AE5C File Offset: 0x0012905C
	private void OnClickSave()
	{
		bool nameIsDuplicated;
		bool flag = this.CheckIsReadyToSave(out nameIsDuplicated);
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Save_Confirm_Content);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this.SaveMod();
				ModManager.UpdateModList(delegate
				{
					this.RefreshUploadModList(true, true, false);
				});
			}, null, EDialogType.None);
		}
		else
		{
			string title2 = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			LanguageKey contentKey = nameIsDuplicated ? LanguageKey.LK_Mod_Save_NameIsDuplicated_Tip : LanguageKey.LK_Mod_Save_NotReady_Tip;
			string content2 = LocalStringManager.Get(contentKey);
			CommonUtils.ShowDialog(title2, content2, null, EDialogType.None);
			this._showUploadWarningMark = true;
			this.RefreshUploadWarningMark();
		}
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x0012AEE8 File Offset: 0x001290E8
	private void SaveMod()
	{
		this._isEditingUploadMod = false;
		string titleText = this.NameInputField.text.Trim();
		bool flag = this._curEditModInfo.DirectoryName.IsNullOrEmpty();
		if (flag)
		{
			this._curEditModInfo.DirectoryName = Path.Combine(ModManager.GetModRootFolder(), titleText);
		}
		bool flag2 = !this._curEditModInfo.ModId.IsValid;
		if (flag2)
		{
			this._curEditModInfo.ModId = ModManager.CreateTempModId(Path.GetFileName(this._curEditModInfo.DirectoryName), true);
		}
		this._curEditModInfo.Title = titleText;
		this._curEditModInfo.Description = this._uploadDescriptionInputField.text;
		this._curEditModInfo.ModId.Version = this.GetVersionFromInputFieldList();
		this._curEditModInfo.Visibility = (EModVisibility)this._uploadVisibilityDropdown.value;
		List<string> tagList = (from t in this._tempModUsedTagList
		where t != SteamManager.AllTagList.First<string>()
		select t).ToList<string>();
		List<string> tagKeyList = SteamManager.GetTagKeyList(tagList);
		this._curEditModInfo.TagList = tagKeyList;
		this._curEditModInfo.ModSettingEntries.Clear();
		this._curEditModInfo.ModSettingEntries.AddRange(this._tempModSettingEntries);
		this._curEditModInfo.Dependencies.Clear();
		this._curEditModInfo.Dependencies.AddRange(this._tempModDependencyList);
		bool flag3 = this._curEditModInfo.Author.IsNullOrEmpty();
		if (flag3)
		{
			string personaName = SteamFriends.GetPersonaName();
			bool flag4 = !personaName.IsNullOrEmpty();
			if (flag4)
			{
				this._curEditModInfo.Author = personaName;
			}
		}
		string modRootFolder = Path.GetFullPath(ModManager.GetModRootFolder());
		string sourceFolderPath = this._tempModCreateFromDirectoryPath;
		string destinationFolderPath = Path.Combine(modRootFolder, Path.GetFileName(this._curEditModInfo.DirectoryName));
		bool flag5 = this._curEditModInfo.ModId.Source == 0 && !this._tempModCreateFromDirectoryPath.IsNullOrEmpty() && sourceFolderPath != destinationFolderPath;
		if (flag5)
		{
			bool flag6 = Directory.Exists(destinationFolderPath);
			if (flag6)
			{
				Directory.Delete(destinationFolderPath, true);
			}
			bool flag7 = !Directory.Exists(destinationFolderPath);
			if (flag7)
			{
				Directory.CreateDirectory(destinationFolderPath);
			}
			string[] rootFiles = Directory.GetFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories);
			foreach (string file in rootFiles)
			{
				string relativePath = file.Substring(sourceFolderPath.Length).TrimStart(Path.DirectorySeparatorChar);
				string destFile = Path.Combine(destinationFolderPath, relativePath);
				bool flag8 = !Directory.Exists(Path.GetDirectoryName(destFile));
				if (flag8)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(destFile));
				}
				bool flag9 = file != destFile;
				if (flag9)
				{
					File.Copy(file, destFile, true);
				}
			}
		}
		else
		{
			bool flag10 = !Directory.Exists(destinationFolderPath);
			if (flag10)
			{
				Directory.CreateDirectory(destinationFolderPath);
			}
		}
		this._tempModCreateFromDirectoryPath = null;
		this._curEditModInfo.Cover = this._tempModCoverFileName;
		this._curEditModInfo.WorkshopCover = this._tempModCoverFileName;
		this._curEditModInfo.DetailImageList.Clear();
		this._curEditModInfo.DetailImageList.AddRange(from s in this._tempModDetailImageFileNameList
		where !s.IsNullOrEmpty()
		select s);
		bool flag11 = !this._tempModCoverFilePath.IsNullOrEmpty() && File.Exists(this._tempModCoverFilePath);
		if (flag11)
		{
			string destCoverPath = Path.Combine(this._curEditModInfo.DirectoryName, this._tempModCoverFileName);
			bool flag12 = this._tempModCoverFilePath != destCoverPath;
			if (flag12)
			{
				File.Copy(this._tempModCoverFilePath, destCoverPath, true);
			}
		}
		for (int index = 0; index < this._tempModDetailImageFilePathList.Count; index++)
		{
			string src = this._tempModDetailImageFilePathList[index];
			bool flag13 = !src.IsNullOrEmpty() && File.Exists(src);
			if (flag13)
			{
				string destImagePath = Path.Combine(this._curEditModInfo.DirectoryName, this._tempModDetailImageFileNameList[index]);
				bool flag14 = src != destImagePath;
				if (flag14)
				{
					File.Copy(src, destImagePath, true);
				}
			}
		}
		bool needBackup = ModSetProgramPanel.NeedBackup;
		if (needBackup)
		{
			this.BackupModPlugins();
		}
		this.CopyModPlugins(this._tempFrontendPlugins);
		this.CopyModPlugins(this._tempBackendPlugins);
		List<string> originList = this._curEditModInfo.FrontendPlugins.Union(this._curEditModInfo.BackendPlugins).ToList<string>();
		List<string> tempList = this._tempFrontendPlugins.Union(this._tempBackendPlugins).ToList<string>();
		this.DeleteModPlugins(originList, tempList);
		this._curEditModInfo.FrontendPlugins.Clear();
		this._curEditModInfo.FrontendPlugins.AddRange(this._tempFrontendPlugins);
		this._curEditModInfo.BackendPlugins.Clear();
		this._curEditModInfo.BackendPlugins.AddRange(this._tempBackendPlugins);
		ModSetProgramPanel.Clear();
		this._curEditModInfo.ChangeConfig = this.ChangeConfigToggle.isOn;
		this._curEditModInfo.HasArchive = this.HasArchiveToggle.isOn;
		this._curEditModInfo.NeedRestartWhenSettingChanged = this.NeedRestartToggle.isOn;
		ModManager.SaveModInfo(this._curEditModInfo);
		ModManager.SaveModSettings(false);
		this.RefreshUploadModButtonOpenExplorer();
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x0012B44C File Offset: 0x0012964C
	private void BackupModPlugins()
	{
		UI_ModPanel.<>c__DisplayClass240_0 CS$<>8__locals1;
		CS$<>8__locals1.pluginDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "Plugins");
		CS$<>8__locals1.legacyDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "LegacyPlugins");
		bool flag = Directory.Exists(CS$<>8__locals1.legacyDirectory);
		if (flag)
		{
			Directory.Delete(CS$<>8__locals1.legacyDirectory, true);
		}
		bool flag2 = !Directory.Exists(CS$<>8__locals1.legacyDirectory);
		if (flag2)
		{
			Directory.CreateDirectory(CS$<>8__locals1.legacyDirectory);
		}
		UI_ModPanel.<BackupModPlugins>g__CopyFile|240_0(this._curEditModInfo.FrontendPlugins, ref CS$<>8__locals1);
		UI_ModPanel.<BackupModPlugins>g__CopyFile|240_0(this._curEditModInfo.BackendPlugins, ref CS$<>8__locals1);
		this._curEditModInfo.FrontendPluginsLegacy.Clear();
		this._curEditModInfo.FrontendPluginsLegacy.AddRange(this._curEditModInfo.FrontendPlugins);
		this._curEditModInfo.BackendPluginsLegacy.Clear();
		this._curEditModInfo.BackendPluginsLegacy.AddRange(this._curEditModInfo.BackendPlugins);
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x0012B550 File Offset: 0x00129750
	private void CopyModPlugins(List<string> tempList)
	{
		string pluginDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "Plugins");
		bool flag = !Directory.Exists(pluginDirectory);
		if (flag)
		{
			Directory.CreateDirectory(pluginDirectory);
		}
		for (int index = 0; index < tempList.Count; index++)
		{
			string fileName = tempList[index];
			string path;
			bool flag2 = !ModSetProgramPanel.TempFileNameToPathDict.TryGetValue(fileName, out path);
			if (!flag2)
			{
				bool flag3 = !File.Exists(path);
				if (flag3)
				{
					tempList[index] = string.Empty;
				}
				else
				{
					string newPath = Path.Combine(pluginDirectory, fileName);
					bool flag4 = path != newPath;
					if (flag4)
					{
						File.Copy(path, newPath, true);
					}
				}
			}
		}
		tempList.RemoveAll((string s) => s.IsNullOrEmpty());
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x0012B62C File Offset: 0x0012982C
	private void DeleteModPlugins(List<string> originList, List<string> tempList)
	{
		string pluginDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "Plugins");
		bool flag = !Directory.Exists(pluginDirectory);
		if (!flag)
		{
			string[] files = Directory.GetFiles(pluginDirectory);
			foreach (string filePath in files)
			{
				string fileName = Path.GetFileName(filePath);
				bool flag2 = originList.Contains(fileName) && !tempList.Contains(fileName);
				if (flag2)
				{
					File.Delete(filePath);
				}
			}
		}
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x0012B6B4 File Offset: 0x001298B4
	private void OnClickUpload()
	{
		bool nameIsDuplicated;
		bool flag = this.CheckIsReadyToSave(out nameIsDuplicated);
		if (flag)
		{
			this._modUploadConfirmDialog.Show(this.CurEditModIsNotCreated, delegate(string log)
			{
				this.SaveMod();
				this._tempModUpdateLogList.Clear();
				bool flag2 = !log.IsNullOrEmpty();
				if (flag2)
				{
					this._tempModUpdateLogList.Add(log);
				}
				this.UploadMod(true);
			});
		}
		else
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			LanguageKey contentKey = nameIsDuplicated ? LanguageKey.LK_Mod_Save_NameIsDuplicated_Tip : LanguageKey.LK_Mod_Upload_NotReady_Tip;
			string content = LocalStringManager.Get(contentKey);
			CommonUtils.ShowDialog(title, content, null, EDialogType.None);
			this._showUploadWarningMark = true;
			this.RefreshUploadWarningMark();
		}
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x0012B730 File Offset: 0x00129930
	private void UploadMod(bool isEdit)
	{
		UI_ModPanel.<>c__DisplayClass245_0 CS$<>8__locals1 = new UI_ModPanel.<>c__DisplayClass245_0();
		CS$<>8__locals1.isEdit = isEdit;
		CS$<>8__locals1.<>4__this = this;
		SteamManager.IsEditMod = CS$<>8__locals1.isEdit;
		List<string> logList = new List<string>(this._tempModUpdateLogList);
		bool isEdit2 = CS$<>8__locals1.isEdit;
		if (isEdit2)
		{
			this.ExcludeModSettingFile();
		}
		this.ShowMask();
		ERemoteStoragePublishedFileVisibility visibility = (ERemoteStoragePublishedFileVisibility)this._curEditModInfo.Visibility;
		bool curEditModIsNotCreated = this.CurEditModIsNotCreated;
		if (curEditModIsNotCreated)
		{
			SteamManager.CreateItem(this._curEditModInfo.DirectoryName, this._curEditModInfo, EWorkshopFileType.k_EWorkshopFileTypeFirst, visibility, logList, new Action<UGCUpdateHandle_t>(CS$<>8__locals1.<UploadMod>g__OnSucceed|0), new Action(CS$<>8__locals1.<UploadMod>g__OnFailed|1));
		}
		else
		{
			SteamManager.UploadItemUpdate(0UL, this._curEditModInfo.DirectoryName, this._curEditModInfo, visibility, logList, new Action<UGCUpdateHandle_t>(CS$<>8__locals1.<UploadMod>g__OnSucceed|0), new Action(CS$<>8__locals1.<UploadMod>g__OnFailed|1));
		}
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x0012B806 File Offset: 0x00129A06
	private void UpdateUploadProgress(UGCUpdateHandle_t updateHandle, bool isEdit, Action onFailed)
	{
		base.StartCoroutine(this.CoroutineUploadMod(updateHandle, isEdit, onFailed));
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x0012B819 File Offset: 0x00129A19
	private IEnumerator CoroutineUploadMod(UGCUpdateHandle_t updateHandle, bool isEdit, Action onFailed)
	{
		UI_FullScreenMask screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
		bool succeed = true;
		ulong bytesProcess;
		ulong byteTotal;
		for (;;)
		{
			EItemUpdateStatus progress = SteamUGC.GetItemUpdateProgress(updateHandle, out bytesProcess, out byteTotal);
			string progressMessage = LocalStringManager.Get(UI_ModPanel.UploadStatusMessages[progress]);
			screenMask.UpdateMessage(progressMessage);
			bool flag = progress == EItemUpdateStatus.k_EItemUpdateStatusInvalid && bytesProcess == byteTotal;
			if (flag)
			{
				break;
			}
			yield return null;
			progressMessage = null;
		}
		bool flag2 = byteTotal > 0UL;
		if (flag2)
		{
			succeed = (bytesProcess > 0UL);
		}
		bool flag3 = succeed;
		if (flag3)
		{
			this.DownloadMod(this._curEditModInfo.ModId);
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_Succeed_Title);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_Mod_Upload_Succeed_Content, this._curEditModInfo.ModId.FileId, this._curEditModInfo.Title);
			CommonUtils.ShowDialog(title, content, null, EDialogType.None);
			this._tempModUpdateLogList.Clear();
			ModManager.UpdateModList(delegate
			{
				this.RefreshUploadModList(true, true, false);
				bool flag4 = !isEdit;
				if (flag4)
				{
					this._curEditModInfo = null;
				}
			});
			title = null;
			content = null;
		}
		else
		{
			string title2 = LocalStringManager.Get(LanguageKey.LK_Steam_Fail_Title);
			string reason = LocalStringManager.Get(LanguageKey.LK_Unknow);
			string content2 = LocalStringManager.GetFormat(LanguageKey.LK_Steam_Fail_Content, reason);
			CommonUtils.ShowDialog(title2, content2, null, EDialogType.None);
			onFailed();
			title2 = null;
			reason = null;
			content2 = null;
		}
		yield break;
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x0012B840 File Offset: 0x00129A40
	private void ExcludeModSettingFile()
	{
		this._curEditModInfo.ModId.Source = 1;
		ModManager.SaveModInfo(this._curEditModInfo);
		string settingPath = Path.Combine(this._curEditModInfo.DirectoryName, "Settings.Lua");
		bool flag = File.Exists(settingPath);
		if (flag)
		{
			string tempDirectory = Path.Combine(ModManager.GetModRootFolder(), ".TempFileForUploading");
			bool flag2 = !Directory.Exists(tempDirectory);
			if (flag2)
			{
				Directory.CreateDirectory(tempDirectory);
			}
			DirectoryInfo tempDirectoryInfo = new DirectoryInfo(tempDirectory);
			tempDirectoryInfo.Attributes = (FileAttributes.Hidden | FileAttributes.Directory);
			string newSettingPath = Path.Combine(tempDirectory, "Settings.Lua");
			bool flag3 = File.Exists(newSettingPath);
			if (flag3)
			{
				File.Delete(newSettingPath);
			}
			File.Move(settingPath, newSettingPath);
		}
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x0012B8F0 File Offset: 0x00129AF0
	private void RecoverModSettingFile()
	{
		this._curEditModInfo.ModId.Source = 0;
		ModManager.SaveModInfo(this._curEditModInfo);
		string tempDirectory = Path.Combine(ModManager.GetModRootFolder(), ".TempFileForUploading");
		bool flag = Directory.Exists(tempDirectory);
		if (flag)
		{
			string tempSettingPath = Path.Combine(tempDirectory, "Settings.Lua");
			bool flag2 = File.Exists(tempSettingPath);
			if (flag2)
			{
				string originSettingsPath = Path.Combine(this._curEditModInfo.DirectoryName, "Settings.Lua");
				bool flag3 = File.Exists(originSettingsPath);
				if (flag3)
				{
					File.Delete(originSettingsPath);
				}
				File.Move(tempSettingPath, originSettingsPath);
			}
		}
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x0012B984 File Offset: 0x00129B84
	private bool CheckIsReadyToSave(out bool nameIsDuplicated)
	{
		return this.CheckModNameIsValid(out nameIsDuplicated) && this.CheckModVersionIsValid() && this.CheckModDescriptionIsValid() && this.CheckModTagIsValid();
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x0012B9B8 File Offset: 0x00129BB8
	private void RefreshUploadWarningMark()
	{
		bool flag;
		this._uploadBasicInfoRefers.nameWarningMark.SetActive(this._showUploadWarningMark && !this.CheckModNameIsValid(out flag));
		this._uploadBasicInfoRefers.versionWarningMark.SetActive(this._showUploadWarningMark && !this.CheckModVersionIsValid());
		this._uploadDescriptionAreaRefers.warningMark.SetActive(this._showUploadWarningMark && !this.CheckModDescriptionIsValid());
		this._uploadBasicInfoRefers.tagWarningMark.SetActive(this._showUploadWarningMark && !this.CheckModTagIsValid());
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x0012BA5C File Offset: 0x00129C5C
	private bool CheckModNameIsValid(out bool isDuplicated)
	{
		isDuplicated = false;
		string text = this.NameInputField.text.Trim();
		bool flag = text.IsNullOrEmpty();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = ModManager.LocalMods.Any((KeyValuePair<string, ModInfoWithDisplayData> pair) => pair.Value.Title == text && this._curEditModInfo.Title != text);
			if (flag2)
			{
				isDuplicated = true;
				result = false;
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x0012BACA File Offset: 0x00129CCA
	private bool CheckModVersionIsValid()
	{
		return this.GetVersionFromInputFieldList() > 0UL;
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x0012BAD6 File Offset: 0x00129CD6
	private bool CheckModDescriptionIsValid()
	{
		return !this._uploadDescriptionInputField.text.IsNullOrEmpty();
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x0012BAEB File Offset: 0x00129CEB
	private bool CheckModTagIsValid()
	{
		return this._tempModUsedTagList.Count > 0 && this._tempModUsedTagList.First<string>() != SteamManager.AllTagList.First<string>();
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x0012BB18 File Offset: 0x00129D18
	private void UpdateUploadModInfo(ModInfoWithDisplayData modInfo)
	{
		ModSetProgramPanel.Clear();
		this._showUploadWarningMark = false;
		this.RefreshUploadWarningMark();
		this.ClearTempUploadModTextures();
		this._curEditModInfo = modInfo;
		this._tempModDetailImageFileNameList.Clear();
		this._tempModDetailImageFileNameList.AddRange(this._curEditModInfo.DetailImageList);
		this._tempModDetailImageFilePathList.Clear();
		this._tempModDetailImageFilePathList.AddRange(this._curEditModInfo.DetailImageList);
		this.uploadModPanel.modInfo.SetActive(true);
		this._tempModCoverFileName = this._curEditModInfo.Cover;
		this._tempModSettingEntries.Clear();
		this._tempModSettingEntries.AddRange(this._curEditModInfo.ModSettingEntries);
		this._tempFrontendPlugins.Clear();
		this._tempFrontendPlugins.AddRange(this._curEditModInfo.FrontendPlugins);
		this._tempBackendPlugins.Clear();
		this._tempBackendPlugins.AddRange(this._curEditModInfo.BackendPlugins);
		this._tempModDependencyList.Clear();
		this._tempModDependencyList.AddRange(from fileId in this._curEditModInfo.RemoteDependencies
		where fileId != modInfo.ModId.FileId
		select fileId);
		this.NameInputField.SetTextWithoutNotify(modInfo.Title);
		string versionString = ModManager.VersionUlongToString(modInfo.ModId.Version);
		this.VersionInputField.SetTextWithoutNotify(versionString);
		string[] numbers = this.VersionInputField.text.Split('.', StringSplitOptions.None);
		for (int i = 0; i < this._versionInputFieldList.Count; i++)
		{
			TMP_InputField inputField = this._versionInputFieldList[i];
			inputField.SetTextWithoutNotify(numbers[i]);
		}
		string author = this._curEditModInfo.Author.IsNullOrEmpty() ? SteamFriends.GetPersonaName() : this._curEditModInfo.Author;
		this._uploadBasicInfoRefers.authorValue.text = "<u>" + author + "</u>";
		this._uploadVisibilityDropdown.value = modInfo.Visibility.ToInt();
		this._tempModUsedTagList.Clear();
		List<string> tagList = modInfo.TagList;
		int tagCount = (tagList != null) ? tagList.Count : 0;
		bool flag = tagCount == 0;
		if (flag)
		{
			this._tempModUsedTagList.Add(SteamManager.AllTagList.First<string>());
		}
		else
		{
			this._tempModUsedTagList.AddRange(SteamManager.GetTagContentList(modInfo.TagList));
		}
		this.RefreshUploadModTag();
		this.RefreshCover(this._uploadBasicInfoRefers.modImageInfo, modInfo, true);
		this._uploadDescriptionInputField.SetTextWithoutNotify(modInfo.Description);
		this.RefreshUploadModSettings();
		bool isRemote = ModManager.UploadedMods.Exists((ModId u) => u.FileId == this._curEditModInfo.ModId.FileId);
		this.uploadModPanel.deleteLocalBtn.SetActive(!isRemote);
		this.uploadModPanel.deleteRemoteBtn.SetActive(isRemote);
		this.uploadModPanel.uploadBtn.SetActive(!isRemote);
		this.uploadModPanel.updateBtn.SetActive(isRemote);
		this.uploadModPanel.buttonSync.gameObject.SetActive(isRemote);
		this.ChangeConfigToggle.isOn = modInfo.ChangeConfig;
		this.HasArchiveToggle.isOn = modInfo.HasArchive;
		this.NeedRestartToggle.isOn = modInfo.NeedRestartWhenSettingChanged;
		this.RefreshUploadModButtonOpenExplorer();
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x0012BECC File Offset: 0x0012A0CC
	private void RefreshUploadModSettings()
	{
		RectTransform settingHolder = this.uploadModPanel.settings;
		this.UpdateSettings(this._curEditModInfo, settingHolder, this._tempModSettingEntries);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(settingHolder);
			settingHolder.GetComponentInParent<CScrollRect>(true).ScrollBar.value = 0f;
		});
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x0012BF24 File Offset: 0x0012A124
	private void RefreshUploadModButtonOpenExplorer()
	{
		bool show = this._curEditModInfo != null && !this._curEditModInfo.DirectoryName.IsNullOrEmpty();
		this.UploadModButtonOpenExplorer.gameObject.SetActive(show);
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x0012BF64 File Offset: 0x0012A164
	private void ClearUploadModInfo()
	{
		this.uploadModPanel.modInfo.SetActive(false);
		this._curEditModInfo = null;
		this.NameInputField.text = string.Empty;
		this.VersionInputField.text = string.Empty;
		this._uploadDescriptionInputField.text = string.Empty;
		this._tempModUsedTagList.Clear();
		this.RefreshUploadModTag();
		this._uploadVisibilityDropdown.value = EModVisibility.Public.ToInt();
		this.ClearCover(this._uploadBasicInfoRefers.modImageInfo);
		this.ClearTempUploadModTextures();
		this.RefreshUploadModButtonOpenExplorer();
		this._versionInputFieldList.ForEach(delegate(TMP_InputField f)
		{
			f.SetTextWithoutNotify(string.Empty);
		});
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x0012C034 File Offset: 0x0012A234
	private void ClearTempUploadModTextures()
	{
		bool flag = this._tempModCoverTexture;
		if (flag)
		{
			Object.Destroy(this._tempModCoverTexture);
			this._tempModCoverTexture = null;
		}
		foreach (KeyValuePair<string, Texture2D> pair in this._tempModDetailImageDict)
		{
			bool flag2 = pair.Value;
			if (flag2)
			{
				Object.Destroy(this._tempModCoverTexture);
			}
		}
		this._tempModDetailImageDict.Clear();
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x0012C0D0 File Offset: 0x0012A2D0
	private void RefreshUploadModTag()
	{
		this._uploadModTagDropdownList.Clear();
		RectTransform tagLayout = this._uploadBasicInfoRefers.tagLayout;
		tagLayout.gameObject.SetActive(true);
		int childCount = tagLayout.childCount - 1;
		int tagCount = this._tempModUsedTagList.Count;
		for (int i = 0; i < tagCount; i++)
		{
			bool flag = i >= childCount;
			if (flag)
			{
				break;
			}
			Transform child = tagLayout.GetChild(i);
			child.gameObject.SetActive(true);
			ModUploadBasicInfoTagItem refers = child.GetComponent<ModUploadBasicInfoTagItem>();
			int index = i;
			string tagContent = this._tempModUsedTagList[index];
			List<string> options = SteamManager.AllTagList.Except(from t in this._tempModUsedTagList
			where t != tagContent
			select t).ToList<string>();
			CDropdown tagDropdown = refers.tagDropdown;
			tagDropdown.onValueChanged.RemoveAllListeners();
			tagDropdown.ClearOptions();
			tagDropdown.AddOptions(options);
			tagDropdown.value = options.IndexOf(tagContent);
			tagDropdown.onValueChanged.AddListener(delegate(int value)
			{
				this._tempModUsedTagList.RemoveAt(index);
				bool flag5 = value != 0 || index == 0;
				if (flag5)
				{
					string newContent = tagDropdown.options[value].text;
					this._tempModUsedTagList.Insert(index, newContent);
				}
				this.RefreshUploadModTag();
			});
			this._uploadModTagDropdownList.Add(tagDropdown);
			CButton buttonLess = refers.buttonLess;
			buttonLess.gameObject.SetActive(index != 0);
			buttonLess.ClearAndAddListener(delegate
			{
				this._tempModUsedTagList.RemoveAt(index);
				this.RefreshUploadModTag();
			});
			refers.space.SetActive(index == 0);
		}
		for (int j = tagCount; j < childCount; j++)
		{
			tagLayout.GetChild(j).gameObject.SetActive(false);
		}
		tagLayout.gameObject.SetActive(this._tempModUsedTagList.Count != 0);
		this._uploadBasicInfoRefers.buttonMoreLayout.SetActive(tagCount < childCount);
		bool flag2 = this._curEditModInfo != null;
		if (flag2)
		{
			List<string> contentList = SteamManager.GetTagContentList(this._curEditModInfo.TagList);
			bool flag3 = contentList.Count == 0;
			if (flag3)
			{
				contentList.Add(SteamManager.AllTagList.First<string>());
			}
			bool flag4 = this._tempModUsedTagList.ContentIsDifferent(contentList);
			if (flag4)
			{
				this._isEditingUploadMod = true;
			}
		}
		this.RefreshUploadWarningMark();
		bool hasEmpty = this._tempModUsedTagList.Contains(SteamManager.AllTagList.First<string>());
		this._buttonMoreTag.interactable = !hasEmpty;
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x0012C37E File Offset: 0x0012A57E
	private void OnClickAddTag()
	{
		this._tempModUsedTagList.Add(SteamManager.AllTagList.First<string>());
		this.RefreshUploadModTag();
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x0012C3A0 File Offset: 0x0012A5A0
	private void CancelEditUploadMod(Action onConfirm, Action onCancel = null)
	{
		this.ButtonCreate.gameObject.SetActive(false);
		this.ButtonOpenUpload.gameObject.SetActive(false);
		bool isEditingUploadMod = this._isEditingUploadMod;
		if (isEditingUploadMod)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Content);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this.ClearTempUploadModTextures();
				this._tempModCoverFileName = string.Empty;
				this._tempModCoverFilePath = string.Empty;
				this._tempModDetailImageFileNameList.Clear();
				this._tempModDetailImageFilePathList.Clear();
				this._isEditingUploadMod = false;
				this._tempModCreateFromDirectoryPath = null;
				Action onConfirm3 = onConfirm;
				if (onConfirm3 != null)
				{
					onConfirm3();
				}
			}, onCancel, EDialogType.None);
		}
		else
		{
			Action onConfirm2 = onConfirm;
			if (onConfirm2 != null)
			{
				onConfirm2();
			}
		}
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x0012C434 File Offset: 0x0012A634
	public static void RefreshButtonPath(CButton button, string path)
	{
		bool notSet = path.IsNullOrEmpty();
		GameObject notSetObj = button.transform.Find("NotSet").gameObject;
		notSetObj.SetActive(notSet);
		GameObject hasSetObj = button.transform.Find("HasSet").gameObject;
		hasSetObj.SetActive(!notSet);
		TextMeshProUGUI text = hasSetObj.GetComponent<TextMeshProUGUI>();
		text.SetText(path, true);
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x0012C498 File Offset: 0x0012A698
	private void ShowMakeDropdownMask(bool show)
	{
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x0012C49C File Offset: 0x0012A69C
	private void OnGUI()
	{
		ModDropdownUtils.HandleDropdown(this._uploadVisibilityDropdown, null);
		foreach (CDropdown dropdown in this._uploadModTagDropdownList)
		{
			ModDropdownUtils.HandleDropdown(dropdown, null);
		}
		foreach (CDropdown dropdown2 in this._curModSettingDropdownList)
		{
			ModDropdownUtils.HandleDropdown(dropdown2, null);
		}
		ModDropdownUtils.HandleDropdown(this._workshopTagDropdown, null);
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x0012C554 File Offset: 0x0012A754
	private void OnModEditSettings(ArgumentBox argumentBox)
	{
		this.RefreshUploadModSettings();
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x0012C560 File Offset: 0x0012A760
	private void OnClickSync(bool isAll)
	{
		if (isAll)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_Sync);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Sync_Confirm_Tip);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this.StartSync(true);
			}, null, EDialogType.None);
		}
		else
		{
			string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Sync);
			string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Sync_All_Confirm_Tip);
			CommonUtils.ShowConfirmDialog(title2, content2, delegate
			{
				this.StartSync(false);
			}, null, EDialogType.None);
		}
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x0012C5D4 File Offset: 0x0012A7D4
	private void StartSync(bool isAll)
	{
		this._needSyncModIdList.Clear();
		List<ModId> needSyncModIdList = this._needSyncModIdList;
		IEnumerable<ModId> collection;
		if (!isAll)
		{
			collection = from id in ModManager.UploadedMods
			where id.FileId == this._curEditModInfo.ModId.FileId
			select id;
		}
		else
		{
			IEnumerable<ModId> uploadedMods = ModManager.UploadedMods;
			collection = uploadedMods;
		}
		needSyncModIdList.AddRange(collection);
		base.StartCoroutine(this.CorStartSync());
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x0012C629 File Offset: 0x0012A829
	private IEnumerator CorStartSync()
	{
		UI_FullScreenMask screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
		screenMask.UpdateMessage("正在同步已上传的MOD到本地");
		this.ShowMask();
		foreach (ModId modId in this._needSyncModIdList)
		{
			PublishedFileId_t publishedFileId = new PublishedFileId_t(modId.FileId);
			SteamUGC.DownloadItem(publishedFileId, true);
			publishedFileId = default(PublishedFileId_t);
			modId = default(ModId);
		}
		List<ModId>.Enumerator enumerator = default(List<ModId>.Enumerator);
		for (;;)
		{
			bool finished = true;
			foreach (ModId modId2 in this._needSyncModIdList)
			{
				bool isInstalled = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateInstalled);
				bool isNeedsUpdate = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateNeedsUpdate);
				bool isDownloading = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateDownloading);
				bool isDownloadPending = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateDownloadPending);
				bool flag = !isInstalled && !isNeedsUpdate && !isDownloading && !isDownloadPending;
				if (!flag)
				{
					bool flag2 = !isInstalled || isNeedsUpdate || isDownloading || isDownloadPending;
					if (flag2)
					{
						finished = false;
					}
					modId2 = default(ModId);
				}
			}
			List<ModId>.Enumerator enumerator2 = default(List<ModId>.Enumerator);
			bool flag3 = finished;
			if (flag3)
			{
				break;
			}
			yield return null;
		}
		bool flag4 = ModManager.SyncCoverLocalMod(this._needSyncModIdList);
		if (flag4)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Notification_Sync_Success);
			CommonUtils.ShowDialog(title, content, delegate()
			{
				this.ClearUploadModInfo();
				ModManager.UpdateModList(delegate
				{
					this.RefreshUploadModList(true, true, false);
				});
			}, EDialogType.None);
			title = null;
			content = null;
		}
		else
		{
			this.HideMask();
		}
		yield break;
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x0012C638 File Offset: 0x0012A838
	private void OnClickDependency()
	{
		this._setDependencePanel.Show(new Action(this.ShowMask), new Action(this.HideMask), this._curEditModInfo.ModId.FileId, this._tempModDependencyList);
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x0012C678 File Offset: 0x0012A878
	private void RefreshBasicInfo(ModPanelBasicInfo basicInfoRefers, ModInfoWithDisplayData modInfo)
	{
		TextMeshProUGUI nameText = basicInfoRefers.nameValue;
		nameText.SetText(modInfo.Title, true);
		TooltipInvoker nameTip = nameText.GetComponent<TooltipInvoker>();
		bool flag = nameTip.PresetParam == null || nameTip.PresetParam.Length == 0;
		if (flag)
		{
			nameTip.PresetParam = new string[1];
		}
		nameTip.PresetParam[0] = modInfo.Title;
		basicInfoRefers.authorValue.SetText("<u>" + modInfo.Author + "</u>", true);
		basicInfoRefers.versionValue.SetText(modInfo.GetVersionString(), true);
		this.UpdateModSource(basicInfoRefers.sourceValue, (sbyte)modInfo.ModId.Source);
		basicInfoRefers.fileIdValue.SetText(modInfo.ModId.FileId.ToString(), true);
		double fileSize = Math.Round((double)modInfo.FileSize / 1048576.0, 3);
		basicInfoRefers.fileSizeValue.SetText(string.Format("{0} MB", fileSize), true);
		basicInfoRefers.createDateValue.SetText(this.GetTimeString(modInfo.CreateData), true);
		basicInfoRefers.updateDataValue.SetText(this.GetTimeString(modInfo.UpdateData), true);
		basicInfoRefers.countFavorite.SetText(modInfo.FavoriteCount.ToString(), true);
		basicInfoRefers.countSubscribe.SetText(modInfo.SubscribeCount.ToString(), true);
		basicInfoRefers.countComment.SetText(modInfo.VoteCount.ToString(), true);
		bool canClickTag = this.toggleGroup.Get(UI_ModPanel.PanelToggleKey.WorkshopMod.ToInt()).gameObject.activeSelf;
		RectTransform tagLayout = basicInfoRefers.tagLayout;
		tagLayout.gameObject.SetActive(true);
		List<string> tagList = modInfo.TagList;
		int tagCount = (tagList != null) ? tagList.Count : 0;
		List<string> tagContentList = SteamManager.GetTagContentList(modInfo.TagList);
		for (int i = 0; i < tagCount; i++)
		{
			bool flag2 = i >= tagLayout.childCount;
			if (flag2)
			{
				break;
			}
			string tagContent = tagContentList[i];
			bool flag3 = tagContent.IsNullOrEmpty();
			if (!flag3)
			{
				Transform child = tagLayout.GetChild(i);
				child.gameObject.SetActive(true);
				TextMeshProUGUI text = child.GetComponentInChildren<TextMeshProUGUI>();
				text.SetText(tagContent, true);
				CButton button = child.GetComponentInChildren<CButton>();
				button.interactable = canClickTag;
				button.ClearAndAddListener(delegate
				{
					this._workshopTagDropdown.value = SteamManager.AllTagList.IndexOf(tagContent);
					UI_ModPanel.PanelToggleKey curToggleKey = this._curToggleKey;
					UI_ModPanel.PanelToggleKey panelToggleKey = curToggleKey;
					if (panelToggleKey != UI_ModPanel.PanelToggleKey.CurMod)
					{
						if (panelToggleKey == UI_ModPanel.PanelToggleKey.WorkshopMod)
						{
							this.ShowWorkshopModDetailPanel(false);
						}
					}
					else
					{
						this.toggleGroup.Set(UI_ModPanel.PanelToggleKey.WorkshopMod.ToInt(), false);
					}
				});
				TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
				tip.enabled = canClickTag;
			}
		}
		for (int j = tagCount; j < tagLayout.childCount; j++)
		{
			tagLayout.GetChild(j).gameObject.SetActive(false);
		}
		int starCount = modInfo.Score;
		RectTransform starLayout = basicInfoRefers.starLayout;
		starLayout.gameObject.SetActive(false);
		for (int k = 0; k < starLayout.childCount; k++)
		{
			string icon = (k < starCount) ? UI_ModPanel.StarActiveIcon : UI_ModPanel.StarInactiveIcon;
			starLayout.GetChild(k).GetComponent<CImage>().SetSprite(icon, false, null);
		}
		this.RefreshCover(basicInfoRefers.modImageInfo, modInfo, true);
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x0012C9CC File Offset: 0x0012ABCC
	private void RefreshButtonSetImage(ModImageInfo imageInfo, bool isCover, bool hasImage)
	{
		ModImageInfoDetail detailImageBack = imageInfo.detailImageBack;
		CToggleGroup detailImageToggleGroup = detailImageBack.detailImageToggleGroup;
		bool isUploadMod = this._curToggleKey == UI_ModPanel.PanelToggleKey.UploadMod;
		bool showSetImage = isUploadMod && (isCover || detailImageToggleGroup.GetActiveIndex() >= 0);
		CButton buttonSetImage = imageInfo.buttonSetImage;
		buttonSetImage.gameObject.SetActive(showSetImage);
		bool flag = showSetImage;
		if (flag)
		{
			GameObject posGo = hasImage ? (isCover ? imageInfo.coverRightBottomPos : imageInfo.detailRightBottomPos) : imageInfo.centerPos;
			buttonSetImage.transform.localPosition = posGo.transform.localPosition;
		}
	}

	// Token: 0x0600288E RID: 10382 RVA: 0x0012CA64 File Offset: 0x0012AC64
	private void RefreshCover(ModImageInfo imageInfo, ModInfoWithDisplayData modInfo, bool init = true)
	{
		CToggleGroup imageToggleGroup = imageInfo.imageToggleGroup;
		ModImageInfoCover coverImageBack = imageInfo.coverImageBack;
		ModImageInfoDetail detailImageBack = imageInfo.detailImageBack;
		CButton buttonSetImage = imageInfo.buttonSetImage;
		CToggleGroup detailImageToggleGroup = detailImageBack.detailImageToggleGroup;
		imageToggleGroup.Init(-1);
		imageToggleGroup.OnActiveIndexChange += delegate(int newTog, int oldTog)
		{
			this._targetDetailImageToggleKey = -1;
			bool isCover = newTog == 0;
			coverImageBack.gameObject.SetActive(isCover);
			detailImageBack.gameObject.SetActive(!isCover);
			bool hasImage = isCover ? this.RefreshCoverImage(coverImageBack, modInfo, true) : this.RefreshDetailImage(imageInfo, modInfo, init);
			this.RefreshButtonSetImage(imageInfo, isCover, hasImage);
		};
		int targetKey = init ? 0 : imageToggleGroup.GetActiveIndex();
		imageToggleGroup.Set(targetKey, true);
		buttonSetImage.ClearAndAddListener(delegate
		{
			bool isCover = imageToggleGroup.GetActiveIndex() == 0;
			string path = LocalDialog.GetUnitySelectFileName("Image Files(*.PNG;*.JPG)\0*.PNG;*.JPG\0", this._curEditModInfo.DirectoryName);
			bool flag = !File.Exists(path);
			if (!flag)
			{
				this._isEditingUploadMod = true;
				string fileName = Path.GetFileName(path);
				bool flag2 = isCover;
				if (flag2)
				{
					ModManager.RemoveModCoverTexture(this._curEditModInfo.ModId);
					this._tempModCoverFilePath = path;
					this._tempModCoverFileName = fileName;
					this.SetImagePath(imageInfo, coverImageBack, path);
				}
				else
				{
					this._tempModDetailImageFilePathList[detailImageToggleGroup.GetActiveIndex()] = path;
					this._tempModDetailImageFileNameList[detailImageToggleGroup.GetActiveIndex()] = fileName;
					this.SetImagePath(imageInfo, detailImageBack, path);
					this.RefreshDetailImage(imageInfo, modInfo, false);
				}
			}
		});
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x0012CB44 File Offset: 0x0012AD44
	private bool RefreshCoverImage(ModImageInfoCover refers, ModInfoWithDisplayData modInfo, bool useTempCover = false)
	{
		CRawImage coverImg = refers.coverImage;
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
				this.ResetCoverSize(refers);
			}
			bool flag6 = !showCover && modInfo.PreviewFileHandle != UGCHandle_t.Invalid;
			if (flag6)
			{
				SteamManager.DownloadPreviewCoverImage(modInfo);
			}
			bool flag7 = this._curToggleKey == UI_ModPanel.PanelToggleKey.UploadMod && !this._tempModCoverFileName.IsNullOrEmpty() && modInfo.Cover != this._tempModCoverFileName;
			if (flag7)
			{
				this._isEditingUploadMod = true;
			}
			result = coverImg.enabled;
		}
		return result;
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x0012CC70 File Offset: 0x0012AE70
	private bool RefreshDetailImage(ModImageInfo imageInfo, ModInfoWithDisplayData modInfo, bool init)
	{
		ModImageInfoDetail detailImageBack = imageInfo.detailImageBack;
		CToggleGroup detailImageToggleGroup = detailImageBack.detailImageToggleGroup;
		CRawImage coverImage = detailImageBack.coverImage;
		for (int i = 0; i < this._tempModDetailImageFileNameList.Count; i++)
		{
			bool flag = i < detailImageToggleGroup.transform.childCount;
			if (flag)
			{
				Transform child = detailImageToggleGroup.transform.GetChild(i);
				child.gameObject.SetActive(true);
				CToggle toggle = child.GetComponent<CToggle>();
				foreach (TextMeshProUGUI label in toggle.GetComponentsInChildren<TextMeshProUGUI>())
				{
					label.text = (i + 1).ToString();
				}
			}
		}
		for (int j = this._tempModDetailImageFileNameList.Count; j < detailImageToggleGroup.transform.childCount; j++)
		{
			detailImageToggleGroup.transform.GetChild(j).gameObject.SetActive(false);
		}
		detailImageToggleGroup.Clear();
		detailImageToggleGroup.AddAllChildToggles();
		detailImageToggleGroup.Init(-1);
		detailImageToggleGroup.OnActiveIndexChange += delegate(int newTog, int oldTog)
		{
			Texture2D texture = null;
			bool flag5 = this._tempModDetailImageFileNameList.CheckIndex(newTog);
			if (flag5)
			{
				string textureName = this._tempModDetailImageFileNameList[newTog];
				bool flag6 = !textureName.IsNullOrEmpty();
				if (flag6)
				{
					bool flag7 = !this._tempModDetailImageDict.TryGetValue(textureName, out texture);
					if (flag7)
					{
						texture = ModManager.GetModDetailTexture(modInfo.ModId, textureName);
					}
				}
				bool flag8 = !texture;
				if (flag8)
				{
					IReadOnlyList<Texture2D> list = ModManager.GetDetailModCoverTexture(modInfo.ModId);
					bool flag9 = list != null && list.Count > newTog;
					if (flag9)
					{
						texture = list[newTog];
					}
				}
			}
			coverImage.texture = texture;
			bool flag10 = texture;
			if (flag10)
			{
				coverImage.enabled = true;
				this.ResetCoverSize(detailImageBack);
			}
			else
			{
				coverImage.enabled = false;
			}
			this.RefreshButtonSetImage(imageInfo, false, coverImage.enabled);
		};
		bool flag2 = init && this._tempModDetailImageFileNameList.Count > 0;
		if (flag2)
		{
			this._targetDetailImageToggleKey = 0;
		}
		detailImageToggleGroup.Set(this._targetDetailImageToggleKey, true);
		bool flag3 = !detailImageToggleGroup.AnyTogglesOn();
		if (flag3)
		{
			coverImage.enabled = false;
		}
		bool isUpload = this._curToggleKey == UI_ModPanel.PanelToggleKey.UploadMod;
		CButton buttonAddImage = detailImageBack.buttonAddImage;
		bool showAdd = isUpload && this._tempModDetailImageFileNameList.Count < detailImageToggleGroup.transform.childCount;
		buttonAddImage.gameObject.SetActive(showAdd);
		buttonAddImage.ClearAndAddListener(delegate
		{
			this._isEditingUploadMod = true;
			this._tempModDetailImageFileNameList.Add(string.Empty);
			this._tempModDetailImageFilePathList.Add(string.Empty);
			this._targetDetailImageToggleKey = this._tempModDetailImageFileNameList.Count - 1;
			this.RefreshDetailImage(imageInfo, modInfo, false);
		});
		buttonAddImage.interactable = !this._tempModDetailImageFileNameList.Contains(string.Empty);
		CButton buttonRemoveImage = detailImageBack.buttonRemoveImage;
		int curDetailImageIndex = (detailImageToggleGroup.GetActiveIndex() >= 0) ? detailImageToggleGroup.GetActiveIndex() : 0;
		bool showRemove = isUpload && this._tempModDetailImageFileNameList.CheckIndex(curDetailImageIndex);
		buttonRemoveImage.gameObject.SetActive(showRemove);
		buttonRemoveImage.ClearAndAddListener(delegate
		{
			this._isEditingUploadMod = true;
			int index = (detailImageToggleGroup.GetActiveIndex() >= 0) ? detailImageToggleGroup.GetActiveIndex() : 0;
			string fileName = this._tempModDetailImageFileNameList[index];
			this._tempModDetailImageFileNameList.RemoveAt(index);
			this._tempModDetailImageFilePathList.RemoveAt(index);
			Texture2D texture;
			bool flag5 = !this._tempModDetailImageFileNameList.Contains(fileName) && this._tempModDetailImageDict.TryGetValue(fileName, out texture);
			if (flag5)
			{
				Object.Destroy(texture);
				this._tempModDetailImageDict.Remove(fileName);
			}
			this._targetDetailImageToggleKey = index - 1;
			this.RefreshDetailImage(imageInfo, modInfo, false);
		});
		bool flag4 = isUpload && this._curEditModInfo != null && this._tempModDetailImageFileNameList.ContentIsDifferent(this._curEditModInfo.DetailImageList);
		if (flag4)
		{
			this._isEditingUploadMod = true;
		}
		return coverImage.enabled;
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x0012CF6C File Offset: 0x0012B16C
	private void SetImagePath(ModImageInfo imageInfo, ModImageInfoCover refers, string filePath)
	{
		CRawImage coverImg = refers.coverImage;
		CToggleGroup imageToggleGroup = imageInfo.imageToggleGroup;
		bool isCover = imageToggleGroup.GetActiveIndex() == 0;
		coverImg.gameObject.SetActive(true);
		byte[] buffer = File.ReadAllBytes(filePath);
		Texture2D texture = new Texture2D(0, 0);
		bool loaded = false;
		try
		{
			loaded = texture.LoadImage(buffer);
			bool flag = isCover;
			if (flag)
			{
				bool flag2 = this._tempModCoverTexture;
				if (flag2)
				{
					Object.Destroy(this._tempModCoverTexture);
				}
				this._tempModCoverTexture = texture;
			}
			else
			{
				ModImageInfoDetail detail = refers as ModImageInfoDetail;
				bool flag3 = detail != null;
				if (flag3)
				{
					CToggleGroup detailImageToggleGroup = detail.detailImageToggleGroup;
					int index = detailImageToggleGroup.GetActiveIndex();
					string name = this._tempModDetailImageFileNameList[index];
					this._tempModDetailImageDict[name] = texture;
				}
			}
			coverImg.texture = texture;
			coverImg.enabled = true;
			this.ResetCoverSize(refers);
			this.RefreshButtonSetImage(imageInfo, isCover, true);
		}
		catch (Exception e)
		{
			GLog.TagError("UI_ModPanel", e.ToString(), Array.Empty<object>());
			throw;
		}
		finally
		{
			bool flag4 = !loaded;
			if (flag4)
			{
				this.ClearCover(imageInfo);
			}
		}
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x0012D0AC File Offset: 0x0012B2AC
	private void ResetCoverSize(ModImageInfoCover refers)
	{
		CRawImage coverImg = refers.coverImage;
		Texture texture = coverImg.texture;
		bool flag = !texture || texture.width == 0 || texture.height == 0;
		if (!flag)
		{
			RectTransform coverImgRectTrans = coverImg.GetComponent<RectTransform>();
			Vector2 anchor = Vector2.one * 0.5f;
			coverImgRectTrans.SetAnchor(anchor, anchor);
			coverImgRectTrans.SetSize(new Vector2((float)texture.width, (float)texture.height));
			float widthScale = this._workShopListCoverImageOriginSize.x / (float)texture.width;
			float heightScale = this._workShopListCoverImageOriginSize.y / (float)texture.height;
			coverImgRectTrans.localScale = Vector3.one * Mathf.Min(widthScale, heightScale);
		}
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x0012D16C File Offset: 0x0012B36C
	private void ClearCover(ModImageInfo imageInfo)
	{
		this._tempModDetailImageFileNameList.Clear();
		this._tempModDetailImageFilePathList.Clear();
		CToggleGroup imageToggleGroup = imageInfo.imageToggleGroup;
		imageToggleGroup.Set(0, true);
		this.RefreshCover(imageInfo, null, true);
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x0012D1AC File Offset: 0x0012B3AC
	private void ShowMask()
	{
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("ShowBlackMask", true);
		box.Set("ShowWaitAnimation", true);
		box.Set("Message", LocalStringManager.Get(LanguageKey.LK_Waiting));
		UIElement.FullScreenMask.SetOnInitArgs(box);
		UIElement.FullScreenMask.Show();
		this.StopMaskTimer();
		this.StartMaskTimer();
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x0012D215 File Offset: 0x0012B415
	private void HideMask()
	{
		UIElement.FullScreenMask.Hide(false);
		this.StopMaskTimer();
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x0012D22B File Offset: 0x0012B42B
	private IEnumerator CorMaskTimer()
	{
		yield return new WaitForSeconds(15f);
		bool exist = UIElement.FullScreenMask.Exist;
		if (exist)
		{
			this.HideMask();
			string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Loading_Failed);
			CommonUtils.ShowDialog(title, content, null, EDialogType.None);
			title = null;
			content = null;
		}
		yield break;
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x0012D23C File Offset: 0x0012B43C
	private void StopMaskTimer()
	{
		bool flag = this._corMaskTimer == null;
		if (!flag)
		{
			base.StopCoroutine(this._corMaskTimer);
			this._corMaskTimer = null;
		}
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x0012D270 File Offset: 0x0012B470
	private void StartMaskTimer()
	{
		bool flag = !base.gameObject.activeSelf;
		if (!flag)
		{
			this._corMaskTimer = base.StartCoroutine(this.CorMaskTimer());
		}
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x0012D2A4 File Offset: 0x0012B4A4
	private string GetTimeString(uint time)
	{
		return DateTimeOffset.FromUnixTimeSeconds((long)((ulong)time)).DateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x0012D2DC File Offset: 0x0012B4DC
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

	// Token: 0x0600289B RID: 10395 RVA: 0x0012D348 File Offset: 0x0012B548
	public static string GetUpdateLog(List<string> list)
	{
		return (list.Count > 0) ? list[0] : string.Empty;
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x0012D374 File Offset: 0x0012B574
	private void OnClickOpenExplorer()
	{
		UI_ModPanel.PanelToggleKey curToggleKey = this._curToggleKey;
		UI_ModPanel.PanelToggleKey panelToggleKey = curToggleKey;
		if (panelToggleKey != UI_ModPanel.PanelToggleKey.CurMod)
		{
			if (panelToggleKey == UI_ModPanel.PanelToggleKey.UploadMod)
			{
				this.OpenExplorer(this._curEditModInfo.DirectoryName);
			}
		}
		else
		{
			this.OpenExplorer(this._curModInfo.DirectoryName);
		}
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x0012D3BE File Offset: 0x0012B5BE
	private void OpenExplorer(string path)
	{
		this.ShowMask();
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.HideMask));
		Process.Start("Explorer.exe", path);
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x0012D6D4 File Offset: 0x0012B8D4
	[CompilerGenerated]
	private void <ShowRestart>g__OnConfirmRestart|173_0()
	{
		List<ModId> needEnableModIds = this._tempEnabledModIdList.Except(ModManager.EnabledMods).ToList<ModId>();
		List<ModId> needDisableModIds = ModManager.EnabledMods.Except(this._tempEnabledModIdList).ToList<ModId>();
		foreach (ModId modId in needEnableModIds)
		{
			ModManager.SetModEnabled(modId, true);
		}
		foreach (ModId modId2 in needDisableModIds)
		{
			ModManager.SetModEnabled(modId2, false);
		}
		this._tempEnabledModIdList.Clear();
		this._tempEnabledModIdList.AddRange(ModManager.EnabledMods);
		ModManager.SaveModSettings(false);
		this.SaveDependenciesChangedMod();
		GameApp.Instance.ReStart();
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x0012D93D File Offset: 0x0012BB3D
	[CompilerGenerated]
	private void <ShowWorkshopModDetailPanel>g__OnSubscribeStateChanged|203_2()
	{
		this.ShowMask();
		this.ShowWorkshopModDetailPanel(true);
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
		{
			this.RefreshWorkshopModList(true, false);
			this.HideMask();
		});
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x0012DA90 File Offset: 0x0012BC90
	[CompilerGenerated]
	private void <OnClickCreateFormDirectory>g__Action|226_0()
	{
		string path = LocalDialog.GetUnitySaveDir("Select your Mod directory", ModManager.GetModRootFolder());
		bool flag = string.IsNullOrEmpty(path) || !Directory.Exists(path);
		if (flag)
		{
			string title = LocalStringManager.Get("LK_Mod_Upload_Invalid_Dir_Title");
			string content = LocalStringManager.Get("LK_Mod_Upload_Invalid_Dir_Content");
			CommonUtils.ShowDialog(title, content, null, EDialogType.None);
		}
		else
		{
			this._isEditingUploadMod = true;
			this._tempModCreateFromDirectoryPath = path;
			string configPath = Path.Combine(path, "Config.lua");
			bool flag2 = File.Exists(configPath);
			if (flag2)
			{
				ModId oldModId = this._curEditModInfo.ModId;
				string oldDirectoryName = this._curEditModInfo.DirectoryName;
				this._curEditModInfo = ModManager.ReadModInfo(configPath, string.Empty, true, false);
				this._tempModCoverFilePath = (this._curEditModInfo.Cover.IsNullOrEmpty() ? string.Empty : Path.Combine(this._curEditModInfo.DirectoryName, this._curEditModInfo.Cover));
				bool isValid = oldModId.IsValid;
				if (isValid)
				{
					this._curEditModInfo.ModId.FileId = oldModId.FileId;
					this._curEditModInfo.ModId.Source = oldModId.Source;
					this._curEditModInfo.DirectoryName = oldDirectoryName;
				}
				else
				{
					this._curEditModInfo.ModId.FileId = 0UL;
					this._curEditModInfo.DirectoryName = null;
				}
				bool flag3 = !this._tempModCoverFilePath.IsNullOrEmpty();
				if (flag3)
				{
					ModManager.RemoveModCoverTexture(this._curEditModInfo.ModId);
					ModManager.AddModCoverTexture(this._curEditModInfo.ModId, this._tempModCoverFilePath);
				}
			}
			this.UpdateUploadModInfo(this._curEditModInfo);
		}
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x0012DD54 File Offset: 0x0012BF54
	[CompilerGenerated]
	internal static void <BackupModPlugins>g__CopyFile|240_0(List<string> plugins, ref UI_ModPanel.<>c__DisplayClass240_0 A_1)
	{
		foreach (string fileName in plugins)
		{
			string oldPath = Path.Combine(A_1.pluginDirectory, fileName);
			bool flag = File.Exists(oldPath);
			if (flag)
			{
				string newPath = Path.Combine(A_1.legacyDirectory, fileName);
				bool flag2 = oldPath != newPath;
				if (flag2)
				{
					File.Copy(oldPath, newPath, true);
				}
			}
		}
	}

	// Token: 0x04001D4B RID: 7499
	[SerializeField]
	private ModSettingWidgetsDropdownContainer dropdownContainer;

	// Token: 0x04001D4C RID: 7500
	[SerializeField]
	private ModSettingWidgetsToggleContainer toggleContainer;

	// Token: 0x04001D4D RID: 7501
	[SerializeField]
	private ModSettingWidgetsSliderContainer sliderContainer;

	// Token: 0x04001D4E RID: 7502
	[SerializeField]
	private GameObject rowContainer;

	// Token: 0x04001D4F RID: 7503
	[SerializeField]
	private ModSettingWidgetsInputFieldContainer inputFieldContainer;

	// Token: 0x04001D50 RID: 7504
	[SerializeField]
	private CToggleGroup toggleGroup;

	// Token: 0x04001D51 RID: 7505
	[SerializeField]
	private CButton buttonClosePopup;

	// Token: 0x04001D52 RID: 7506
	[SerializeField]
	private ModCurModPanel curModPanel;

	// Token: 0x04001D53 RID: 7507
	[SerializeField]
	private ModWorkshopPanel workshopPanel;

	// Token: 0x04001D54 RID: 7508
	[SerializeField]
	private ModUploadModPanel uploadModPanel;

	// Token: 0x04001D55 RID: 7509
	[SerializeField]
	private Texture2D popupModPanelLoadError0;

	// Token: 0x04001D56 RID: 7510
	private Coroutine _corMaskTimer;

	// Token: 0x04001D57 RID: 7511
	private static readonly string StarActiveIcon = "popup_modpanel_icon_pingfen";

	// Token: 0x04001D58 RID: 7512
	private static readonly string StarInactiveIcon = "popup_modpanel_icon_pingfen_gray";

	// Token: 0x04001D59 RID: 7513
	private UI_ModPanel.PanelToggleKey _curToggleKey = UI_ModPanel.PanelToggleKey.Invalid;

	// Token: 0x04001D5A RID: 7514
	private InfinityScroll _curModScroll;

	// Token: 0x04001D5B RID: 7515
	private CToggleGroup _curModScrollToggleGroup;

	// Token: 0x04001D5C RID: 7516
	private ModIdSwitch _curModPageSwitch;

	// Token: 0x04001D5D RID: 7517
	private TMP_InputField _curModSearchInputField;

	// Token: 0x04001D5E RID: 7518
	private RectTransform _curModSettingsHolder;

	// Token: 0x04001D5F RID: 7519
	private ModPanelBasicInfo _curBasicInfoRefers;

	// Token: 0x04001D60 RID: 7520
	private int _lastCurModPageSwitchValue;

	// Token: 0x04001D61 RID: 7521
	private int _curModPageMaxCount;

	// Token: 0x04001D62 RID: 7522
	private int _curIndex;

	// Token: 0x04001D63 RID: 7523
	private ModId _curModId;

	// Token: 0x04001D64 RID: 7524
	private ModInfoWithDisplayData _curModInfo;

	// Token: 0x04001D65 RID: 7525
	private DialogCmd _restartDialog;

	// Token: 0x04001D66 RID: 7526
	private bool _hasChangedSetting;

	// Token: 0x04001D67 RID: 7527
	private bool _hasChangedEnable;

	// Token: 0x04001D68 RID: 7528
	private readonly List<ModId> _tempEnabledModIdList = new List<ModId>();

	// Token: 0x04001D69 RID: 7529
	private const string ToggleKey = "ModPanel_SettingEntry_Toggle";

	// Token: 0x04001D6A RID: 7530
	private const string SliderKey = "ModPanel_SettingEntry_Slider";

	// Token: 0x04001D6B RID: 7531
	private const string DropdownKey = "ModPanel_SettingEntry_Dropdown";

	// Token: 0x04001D6C RID: 7532
	private const string InputFieldKey = "ModPanel_SettingEntry_InputField";

	// Token: 0x04001D6D RID: 7533
	private const string RowKey = "ModPanel_Setting_Row";

	// Token: 0x04001D6E RID: 7534
	private const int MaxRowWidth = 2;

	// Token: 0x04001D6F RID: 7535
	private readonly List<ModSettingWidgetsContainer> _settingEntriesList = new List<ModSettingWidgetsContainer>();

	// Token: 0x04001D70 RID: 7536
	private readonly List<Transform> _settingRowList = new List<Transform>();

	// Token: 0x04001D71 RID: 7537
	private readonly List<ModId> _curModIdList = new List<ModId>();

	// Token: 0x04001D72 RID: 7538
	private readonly List<ModId> _originCurModIdList = new List<ModId>();

	// Token: 0x04001D73 RID: 7539
	private bool _needRestart;

	// Token: 0x04001D74 RID: 7540
	private readonly List<ModId> _dependenciesChangedList = new List<ModId>();

	// Token: 0x04001D75 RID: 7541
	private bool _hasUpdateDetailInfo;

	// Token: 0x04001D76 RID: 7542
	private const string ModEntryBackDefault = "popup_modpanel_anniu_0_0";

	// Token: 0x04001D77 RID: 7543
	private const string ModEntryBackOutdated = "popup_modpanel_anniu_0_2";

	// Token: 0x04001D78 RID: 7544
	private const string ModEntryBackDownloading = "popup_modpanel_anniu_0_3";

	// Token: 0x04001D79 RID: 7545
	private const string ModEntryBackDependencyChanged = "popup_modpanel_anniu_0_4";

	// Token: 0x04001D7A RID: 7546
	private readonly List<ModId> _downLoadingItemList = new List<ModId>();

	// Token: 0x04001D7B RID: 7547
	private readonly List<ModId> _downLoadedItemList = new List<ModId>();

	// Token: 0x04001D7C RID: 7548
	private bool _curModPanelNeedUpdate;

	// Token: 0x04001D7D RID: 7549
	private bool _curModPanelNeedScrollToSelectedWorkshopMod;

	// Token: 0x04001D7E RID: 7550
	private ModEnableDependenceDialog _enableDependenceDialog;

	// Token: 0x04001D7F RID: 7551
	private InfinityScroll _workshopScroll;

	// Token: 0x04001D80 RID: 7552
	private TMP_InputField _workshopSearchInputField;

	// Token: 0x04001D81 RID: 7553
	private CToggleGroup _workshopSortToggleGroup;

	// Token: 0x04001D82 RID: 7554
	private CToggleGroup _workshopTimeToggleGroup;

	// Token: 0x04001D83 RID: 7555
	private ModIdSwitch _workshopPageSwitch;

	// Token: 0x04001D84 RID: 7556
	private CToggleGroup _workshopDetailToggleGroup;

	// Token: 0x04001D85 RID: 7557
	private CDropdown _workshopTagDropdown;

	// Token: 0x04001D86 RID: 7558
	private string _workshopSearchInputText;

	// Token: 0x04001D87 RID: 7559
	private readonly List<ModId> _workshopModIdList = new List<ModId>();

	// Token: 0x04001D88 RID: 7560
	private readonly List<ModId> _originWorkshopModIdList = new List<ModId>();

	// Token: 0x04001D89 RID: 7561
	private const int WorkshopPageItemMaxCount = 48;

	// Token: 0x04001D8A RID: 7562
	private const float PageRate = 1.0416666f;

	// Token: 0x04001D8B RID: 7563
	private int _steamPageMaxCount = 1;

	// Token: 0x04001D8C RID: 7564
	private ModInfoWithDisplayData _selectedWorkshopModInfo;

	// Token: 0x04001D8F RID: 7567
	private ModSubscribeDependenceDialog _subscribeDependenceDialog;

	// Token: 0x04001D90 RID: 7568
	private Vector2 _workShopListCoverImageOriginSize;

	// Token: 0x04001D91 RID: 7569
	private InfinityScroll _uploadModScroll;

	// Token: 0x04001D92 RID: 7570
	private CToggleGroup _uploadModScrollToggleGroup;

	// Token: 0x04001D93 RID: 7571
	private ModIdSwitch _uploadModPageSwitch;

	// Token: 0x04001D94 RID: 7572
	private TMP_InputField _uploadModSearchInputField;

	// Token: 0x04001D95 RID: 7573
	private ModUploadBasicInfo _uploadBasicInfoRefers;

	// Token: 0x04001D96 RID: 7574
	private CDropdown _uploadVisibilityDropdown;

	// Token: 0x04001D97 RID: 7575
	private CButton _buttonProgram;

	// Token: 0x04001D98 RID: 7576
	private CButton _buttonAdd;

	// Token: 0x04001D99 RID: 7577
	private CButton _buttonMoreTag;

	// Token: 0x04001D9A RID: 7578
	private readonly List<TMP_InputField> _versionInputFieldList = new List<TMP_InputField>();

	// Token: 0x04001D9B RID: 7579
	private ModUploadDescriptionArea _uploadDescriptionAreaRefers;

	// Token: 0x04001D9C RID: 7580
	private GameObject _uploadDescriptionRoot;

	// Token: 0x04001D9D RID: 7581
	private GameObject _uploadUpdateLogRoot;

	// Token: 0x04001D9E RID: 7582
	private TMP_InputField _uploadDescriptionInputField;

	// Token: 0x04001D9F RID: 7583
	private TextMeshProUGUI _uploadUpdateLogContent;

	// Token: 0x04001DA0 RID: 7584
	private ModSetProgramPanel _setProgramPanel;

	// Token: 0x04001DA1 RID: 7585
	private ModEditSettingPanel _editSettingPanel;

	// Token: 0x04001DA2 RID: 7586
	private ModEditUpdateLogPanel _editUpdateLogPanel;

	// Token: 0x04001DA3 RID: 7587
	private ModUploadConfirmDialog _modUploadConfirmDialog;

	// Token: 0x04001DA4 RID: 7588
	private ModSetDependencePanel _setDependencePanel;

	// Token: 0x04001DA5 RID: 7589
	private ModDirectlyUploadPanel _modDirectlyUploadPanel;

	// Token: 0x04001DA6 RID: 7590
	private int _lastUploadModPageSwitchValue;

	// Token: 0x04001DA7 RID: 7591
	private int _uploadModPageMaxCount;

	// Token: 0x04001DA8 RID: 7592
	private readonly List<ModId> _uploadModIdList = new List<ModId>();

	// Token: 0x04001DA9 RID: 7593
	private readonly List<ModId> _originUploadModIdList = new List<ModId>();

	// Token: 0x04001DAA RID: 7594
	private readonly List<string> _tempModUsedTagList = new List<string>();

	// Token: 0x04001DAB RID: 7595
	private readonly List<CDropdown> _uploadModTagDropdownList = new List<CDropdown>();

	// Token: 0x04001DAC RID: 7596
	private readonly List<CDropdown> _curModSettingDropdownList = new List<CDropdown>();

	// Token: 0x04001DAD RID: 7597
	private bool _isEditingUploadMod;

	// Token: 0x04001DAE RID: 7598
	private readonly List<SettingEntry> _tempModSettingEntries = new List<SettingEntry>();

	// Token: 0x04001DAF RID: 7599
	private string _tempModCreateFromDirectoryPath;

	// Token: 0x04001DB0 RID: 7600
	private ModInfoWithDisplayData _curEditModInfo;

	// Token: 0x04001DB1 RID: 7601
	private readonly List<string> _tempFrontendPlugins = new List<string>();

	// Token: 0x04001DB2 RID: 7602
	private readonly List<string> _tempBackendPlugins = new List<string>();

	// Token: 0x04001DB3 RID: 7603
	private readonly List<string> _tempModUpdateLogList = new List<string>();

	// Token: 0x04001DB4 RID: 7604
	private readonly List<ModId> _needSyncModIdList = new List<ModId>();

	// Token: 0x04001DB5 RID: 7605
	private string _tempModCoverFileName;

	// Token: 0x04001DB6 RID: 7606
	private readonly List<string> _tempModDetailImageFileNameList = new List<string>();

	// Token: 0x04001DB7 RID: 7607
	private string _tempModCoverFilePath;

	// Token: 0x04001DB8 RID: 7608
	private readonly List<string> _tempModDetailImageFilePathList = new List<string>();

	// Token: 0x04001DB9 RID: 7609
	private Texture2D _tempModCoverTexture;

	// Token: 0x04001DBA RID: 7610
	private readonly Dictionary<string, Texture2D> _tempModDetailImageDict = new Dictionary<string, Texture2D>();

	// Token: 0x04001DBB RID: 7611
	private int _targetDetailImageToggleKey = -1;

	// Token: 0x04001DBC RID: 7612
	private readonly List<ulong> _tempModDependencyList = new List<ulong>();

	// Token: 0x04001DBD RID: 7613
	private bool _showUploadWarningMark;

	// Token: 0x04001DBE RID: 7614
	private const string TempFileForUploadingDirectoryName = ".TempFileForUploading";

	// Token: 0x04001DBF RID: 7615
	private static readonly Dictionary<EItemUpdateStatus, string> UploadStatusMessages = new Dictionary<EItemUpdateStatus, string>
	{
		{
			EItemUpdateStatus.k_EItemUpdateStatusInvalid,
			"LK_Mod_Update_Status_Invalid"
		},
		{
			EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig,
			"LK_Mod_Update_Status_Preparing_Config"
		},
		{
			EItemUpdateStatus.k_EItemUpdateStatusPreparingContent,
			"LK_Mod_Update_Status_Preparing_Content"
		},
		{
			EItemUpdateStatus.k_EItemUpdateStatusUploadingContent,
			"LK_Mod_Update_Status_Uploading_Content"
		},
		{
			EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile,
			"LK_Mod_Update_Status_Uploading_Preview_File"
		},
		{
			EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges,
			"LK_Mod_Update_Status_Committing_Changes"
		}
	};

	// Token: 0x020015B7 RID: 5559
	private enum PanelToggleKey
	{
		// Token: 0x0400A5AD RID: 42413
		Invalid = -1,
		// Token: 0x0400A5AE RID: 42414
		CurMod,
		// Token: 0x0400A5AF RID: 42415
		WorkshopMod,
		// Token: 0x0400A5B0 RID: 42416
		UploadMod
	}

	// Token: 0x020015B8 RID: 5560
	private enum WorkshopDetailToggleKey
	{
		// Token: 0x0400A5B2 RID: 42418
		Description,
		// Token: 0x0400A5B3 RID: 42419
		UpdateLog
	}

	// Token: 0x020015B9 RID: 5561
	public enum WorkshopSortToggleKey
	{
		// Token: 0x0400A5B5 RID: 42421
		MostPopular,
		// Token: 0x0400A5B6 RID: 42422
		MostSubscribed,
		// Token: 0x0400A5B7 RID: 42423
		MostRated,
		// Token: 0x0400A5B8 RID: 42424
		LatestUpdate,
		// Token: 0x0400A5B9 RID: 42425
		LatestUpload
	}

	// Token: 0x020015BA RID: 5562
	public enum WorkshopTimeToggleKey
	{
		// Token: 0x0400A5BB RID: 42427
		All,
		// Token: 0x0400A5BC RID: 42428
		Year,
		// Token: 0x0400A5BD RID: 42429
		Quarter,
		// Token: 0x0400A5BE RID: 42430
		Month,
		// Token: 0x0400A5BF RID: 42431
		Week,
		// Token: 0x0400A5C0 RID: 42432
		Day
	}
}
