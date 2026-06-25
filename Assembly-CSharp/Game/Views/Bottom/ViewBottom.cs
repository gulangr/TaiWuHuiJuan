using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Bottom.Migrate;
using Game.Views.Building;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using Game.Views.SystemOption;
using Game.Views.VillagerRoleView;
using Game.Views.World;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Map;
using GameData.Domains.Map.TeammateBubble;
using GameData.Domains.Story.MainStory;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C4C RID: 3148
	public class ViewBottom : UIBase, IMainMenuButtonParent, IAsyncMethodRequestHandler
	{
		// Token: 0x170010E5 RID: 4325
		// (get) Token: 0x0600A019 RID: 40985 RVA: 0x004AB508 File Offset: 0x004A9708
		public static bool IsReady
		{
			get
			{
				UIElement bottom = UIElement.Bottom;
				bool result;
				if (bottom == null)
				{
					result = false;
				}
				else
				{
					ViewBottom viewBottom = bottom.UiBaseAs<ViewBottom>();
					bool? flag = (viewBottom != null) ? new bool?(viewBottom._isReady) : null;
					bool flag2 = true;
					result = (flag.GetValueOrDefault() == flag2 & flag != null);
				}
				return result;
			}
		}

		// Token: 0x0600A01A RID: 40986 RVA: 0x004AB554 File Offset: 0x004A9754
		public void OnMouseHover()
		{
			foreach (SkeletonGraphic anim in this.seasonAnims)
			{
				bool activeSelf = anim.gameObject.activeSelf;
				if (activeSelf)
				{
					anim.AnimationState.SetAnimation(0, "idle", false).Complete += ViewBottom.<OnMouseHover>g__OnIdleAnimationComplete|67_0;
				}
			}
			this.summaryHoverGo.SetActive(true);
		}

		// Token: 0x0600A01B RID: 40987 RVA: 0x004AB5BF File Offset: 0x004A97BF
		public void OnMouseLeave()
		{
			this.summaryHoverGo.SetActive(false);
		}

		// Token: 0x0600A01C RID: 40988 RVA: 0x004AB5D0 File Offset: 0x004A97D0
		public void RefreshSeasonAnimCall(ArgumentBox _)
		{
			bool flag = ViewBottom._lastDate == SingletonObject.getInstance<BasicGameData>().CurrDate;
			if (!flag)
			{
				ViewBottom._lastDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				this.RefreshSeasonAnim(TimeKit.GetCurrSeason());
			}
		}

		// Token: 0x0600A01D RID: 40989 RVA: 0x004AB610 File Offset: 0x004A9810
		public void OnLanguageChange(ArgumentBox _)
		{
			Sprite[] sprites = (!UIElement.MapBlockCharList.Exist) ? (this.building.interactable ? this.regionTextActive : this.regionTextInactive) : (this.building.interactable ? this.buildingTextActive : this.buildingTextInactive);
			this.buildingText.sprite = (sprites.CheckIndex((int)LocalStringManager.CurLanguageType) ? sprites[(int)LocalStringManager.CurLanguageType] : sprites[1]);
			this.buildingText.SetNativeSize();
			sprites = (UIElement.PartWorld.Exist ? (this.worldMap.interactable ? this.regionTextActive : this.regionTextInactive) : (this.worldMap.interactable ? this.worldMapTextActive : this.worldMapTextInactive));
			this.worldMapText.sprite = (sprites.CheckIndex((int)LocalStringManager.CurLanguageType) ? sprites[(int)LocalStringManager.CurLanguageType] : sprites[1]);
			this.worldMapText.SetNativeSize();
		}

		// Token: 0x0600A01E RID: 40990 RVA: 0x004AB70C File Offset: 0x004A990C
		public void RefreshSeasonAnim(sbyte currSeason)
		{
			this.seasonBg.sprite = this.seasonBgs[(int)currSeason];
			int i = this.seasonAnims.Length;
			while (i-- > 0)
			{
				this.seasonAnims[i].gameObject.SetActive(i == (int)currSeason && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(1));
			}
			this.seasonAnims[(int)currSeason].AnimationState.SetAnimation(0, "in", false).Complete += ViewBottom.<RefreshSeasonAnim>g__OnInAnimationComplete|71_0;
			this.monthlyReportInteractArea.SetActive(SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(1));
		}

		// Token: 0x0600A01F RID: 40991 RVA: 0x004AB7B0 File Offset: 0x004A99B0
		private void AwakeCustomButtons()
		{
			foreach (CustomButton btn in this.bottomShortCuts.Append(this.customButtonPrefab))
			{
				btn.Init(this);
			}
			this.RefreshCustomButtons();
			GEvent.Add(UiEvents.NotifyBottomCustomButtonChange, new GEvent.Callback(this.CustomButtonChanged));
		}

		// Token: 0x0600A020 RID: 40992 RVA: 0x004AB830 File Offset: 0x004A9A30
		private void DestroyCustomButtons()
		{
			GEvent.Remove(UiEvents.NotifyBottomCustomButtonChange, new GEvent.Callback(this.CustomButtonChanged));
		}

		// Token: 0x0600A021 RID: 40993 RVA: 0x004AB850 File Offset: 0x004A9A50
		private void CustomButtonChanged(ArgumentBox _)
		{
			bool flag = ViewBottom.TempShortCutsSettings == null;
			if (!flag)
			{
				this.containerName.text = LanguageKey.LK_Bottom_Function_Settings.TrFormat(ViewBottom.TempShortCutsSettings.Length, this.bottomShortCuts.Length);
			}
		}

		// Token: 0x0600A022 RID: 40994 RVA: 0x004AB89C File Offset: 0x004A9A9C
		private void RefreshCustomButtons()
		{
			bool show = UIManager.Instance.IsFocusElement(UIElement.StateMainWorld) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			this.shortCutConfig.gameObject.SetActive(true);
			bool flag = !show;
			if (flag)
			{
				this.container.anchoredPosition = new Vector2(0f, 0f);
			}
			else
			{
				int i = this.container.childCount;
				while (i-- > 0)
				{
					Object.Destroy(this.container.GetChild(i).gameObject);
				}
				Dictionary<int, Transform> categoryGoDict = new Dictionary<int, Transform>();
				using (IEnumerator<MainUiCustomButtonItem> enumerator = ((IEnumerable<MainUiCustomButtonItem>)Config.MainUiCustomButton.Instance).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MainUiCustomButtonItem cfg = enumerator.Current;
						bool visible = cfg.Visible;
						if (visible)
						{
							CustomButton btn = Object.Instantiate<CustomButton>(this.customButtonPrefab, this.container, false);
							btn.gameObject.SetActive(false);
							btn.TemplateId = (int)cfg.TemplateId;
							btn.RefreshToggleVisibility(delegate(bool isActive)
							{
								bool flag2 = !btn;
								if (!flag2)
								{
									btn.gameObject.SetActive(isActive);
									bool flag3 = !isActive;
									if (!flag3)
									{
										int category = cfg.Category;
										bool flag4 = !categoryGoDict.ContainsKey(category);
										if (flag4)
										{
											GameObject categoryGo = Object.Instantiate<GameObject>(this.categoryPrefab, this.container);
											if (!true)
											{
											}
											string text;
											switch (category)
											{
											case 1:
												text = LanguageKey.LK_CommonSortAndFilter_FilterPanel_Title_Character.Tr();
												break;
											case 2:
												text = LanguageKey.LK_WorldMap_Building.Tr();
												break;
											case 3:
												text = LanguageKey.LK_Bottom_ShortCutSetting_Features.Tr();
												break;
											default:
												text = "";
												break;
											}
											if (!true)
											{
											}
											string str = text;
											categoryGo.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = str;
											categoryGo.SetActive(true);
											categoryGoDict.Add(category, categoryGo.transform);
										}
										btn.transform.SetParent(categoryGoDict[category], false);
									}
								}
							});
						}
					}
				}
			}
		}

		// Token: 0x0600A023 RID: 40995 RVA: 0x004ABA38 File Offset: 0x004A9C38
		private void Awake()
		{
			this.timeBall.Init(this);
			this.AwakeCustomButtons();
			this.taiwuAvatar.gameObject.SetActive(false);
			this.teammate1Avatar.gameObject.SetActive(false);
			this.teammate2Avatar.gameObject.SetActive(false);
			this.teammate3Avatar.gameObject.SetActive(false);
			this.mapPickupWindow.Init();
			this.mapPickupWindow.gameObject.SetActive(false);
			this.opBtn.onClick.ResetListener(new Action(this.OpBtn));
			this.professionBtn.onClick.ResetListener(new Action(this.ProfessionBtn));
			this.professionBtn.GetComponent<PointerTrigger>().EnterEvent.AddListener(delegate()
			{
				bool flag = !this.professionBottomMenu.gameObject.activeSelf;
				if (flag)
				{
					this.professionBottomMenu.gameObject.SetActive(true);
				}
			});
			this.summaryBtn.onClick.ResetListener(new Action(this.SummaryBtn));
			this.building.onClick.ResetListener(new Action(this.Building));
			this.mapFilter.onClick.ResetListener(new Action(this.MapFilter));
			this.mapFind.onClick.ResetListener(new Action(this.MapFind));
			this.mapLocate.onClick.ResetListener(new Action(this.MapLocate));
			this.mapLocateTaiwuVillage.onClick.ResetListener(new Action(this.MapLocateTaiwuVillage));
			this.worldMap.onClick.ResetListener(new Action(this.WorldMap));
			this.mainCharacterButton.onClick.ResetListener(delegate()
			{
				this.ShowCharacterMenu(ECharacterSubToggleBase.None, ECharacterSubPage.None, -1);
			});
			this.teammate1.onClick.ResetListener(new Action(this.Teammate1));
			this.teammate2.onClick.ResetListener(new Action(this.Teammate2));
			this.teammate3.onClick.ResetListener(new Action(this.Teammate3));
			this.changeTeammate1.onClick.ResetListener(new Action(this.SwitchTeammates));
			this.changeTeammate2.onClick.ResetListener(new Action(this.SwitchTeammates));
			this.changeTeammate3.onClick.ResetListener(new Action(this.SwitchTeammates));
			this.shortCutConfig.onClick.ResetListener(delegate()
			{
				bool isActive = this.shortCutConfigSettings.gameObject.activeSelf;
				bool flag = isActive;
				if (flag)
				{
					this.TriggerSetEscHandler();
				}
				else
				{
					this.TriggerSetEscHandler();
					this.CustomButtonChanged(null);
					this.shortCutConfigSettings.gameObject.SetActive(true);
					UIManager.Instance.SetEscHandler(new Action(this.TriggerSetEscHandler));
				}
			});
			this.shortCutConfigHide.onClick.ResetListener(new Action(this.TriggerSetEscHandler));
			this.shortCutConfigSure.onClick.ResetListener(delegate()
			{
				CustomButton.Settings = ViewBottom.TempShortCutsSettings;
				GEvent.OnEvent(UiEvents.NotifyBottomCustomButtonChange, null);
			});
			GEvent.Add(UiEvents.PickupDisplayInfoChange, new GEvent.Callback(this.OnPickUpDisplayInfoChange));
			GEvent.Add(UiEvents.OnMapBlockMoveFinished, new GEvent.Callback(this.OnCurrentPickUpDisplayInfoChange));
			GEvent.Add(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.OnCurrentPickUpDisplayInfoChange));
			GEvent.Add(UiEvents.CombatTeammateChange, new GEvent.Callback(this.RefreshCharacterAvatar));
			GEvent.Add(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.RefreshCharacterAvatar));
			GEvent.Add(UiEvents.ReturnBottomTimeDisk, new GEvent.Callback(this.OnReturnBottomTimeDisk));
			GEvent.Add(UiEvents.OnSetBottomInteractable, new GEvent.Callback(this.OnSetBottomInteractable));
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Add(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Add(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
			GEvent.Add(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.RefreshSeasonAnimCall));
			GEvent.Add(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.UpdateTaiwuCarrier));
			this._uiViewCoveredBehaviour = base.gameObject.GetComponent<UIViewCoveredBehaviour>();
		}

		// Token: 0x0600A024 RID: 40996 RVA: 0x004ABE8C File Offset: 0x004AA08C
		private void OnDestroy()
		{
			this.DestroyCustomButtons();
			GEvent.Remove(UiEvents.PickupDisplayInfoChange, new GEvent.Callback(this.OnPickUpDisplayInfoChange));
			GEvent.Remove(UiEvents.OnMapBlockMoveFinished, new GEvent.Callback(this.OnCurrentPickUpDisplayInfoChange));
			GEvent.Remove(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.OnCurrentPickUpDisplayInfoChange));
			GEvent.Remove(UiEvents.CombatTeammateChange, new GEvent.Callback(this.RefreshCharacterAvatar));
			GEvent.Remove(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.RefreshCharacterAvatar));
			GEvent.Remove(UiEvents.ReturnBottomTimeDisk, new GEvent.Callback(this.OnReturnBottomTimeDisk));
			GEvent.Remove(UiEvents.OnSetBottomInteractable, new GEvent.Callback(this.OnSetBottomInteractable));
			GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Remove(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Remove(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
			GEvent.Remove(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.RefreshSeasonAnimCall));
			GEvent.Remove(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.UpdateTaiwuCarrier));
		}

		// Token: 0x0600A025 RID: 40997 RVA: 0x004AC000 File Offset: 0x004AA200
		private void HideShortCutConfig()
		{
			ViewBottom.TempShortCutsSettings = CustomButton.Settings;
			this.shortCutConfigSettings.gameObject.SetActive(false);
			bool flag = UIManager.Instance.CheckEscHandler(new Action(this.TriggerSetEscHandler));
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x0600A026 RID: 40998 RVA: 0x004AC050 File Offset: 0x004AA250
		private void TriggerSetEscHandler()
		{
			this.HideShortCutConfig();
			UIElement.FindMapBlock.Hide(false);
			ViewFindMapBlock findMapBlockView = UIElement.FindMapBlock.UiBaseAs<ViewFindMapBlock>();
			bool flag = findMapBlockView != null;
			if (flag)
			{
				findMapBlockView.OpenCloseAudio = UIBase.UIOpenCloseAudioType.None;
			}
		}

		// Token: 0x0600A027 RID: 40999 RVA: 0x004AC090 File Offset: 0x004AA290
		private void Building()
		{
			bool flag = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure || SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventTaiwu.InAdventure;
			if (!flag)
			{
				bool exist = UIElement.BuildingArea.Exist;
				if (exist)
				{
					ViewBuildingArea.Hide();
				}
				else
				{
					BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
					bool flag2 = basicGameData.IsDreamBack && !basicGameData.IsDreamBackStateUnlocked(4);
					if (flag2)
					{
						bool flag3 = !this._isDreamBackUnlockBuildingButtonClicked;
						if (flag3)
						{
							this._isDreamBackUnlockBuildingButtonClicked = true;
							TaiwuEventDomainMethod.Call.TaiwuCrossArchiveFindMemory(4);
						}
					}
					else
					{
						GEvent.OnEvent(UiEvents.HideMapBlockCharList, null);
						Location blockKey = SingletonObject.getInstance<TutorialChapterModel>().InGuiding ? this._mapModel.CurrentLocation : this._mapModel.GetTaiwuVillageBlock();
						ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
						argsBox.Set("AreaId", blockKey.AreaId);
						argsBox.Set("BlockId", blockKey.BlockId);
						UIElement.BuildingArea.SetOnInitArgs(argsBox);
						CommandManager.AddCommand<CommandStackUI, UIElement>(EPriority.StackUINormal, UIElement.StateBuilding);
					}
				}
			}
		}

		// Token: 0x0600A028 RID: 41000 RVA: 0x004AC1A2 File Offset: 0x004AA3A2
		private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
		{
			this.PlayAnim(false);
		}

		// Token: 0x0600A029 RID: 41001 RVA: 0x004AC1AC File Offset: 0x004AA3AC
		private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
		{
			this.PlayAnim(true);
		}

		// Token: 0x170010E6 RID: 4326
		// (get) Token: 0x0600A02A RID: 41002 RVA: 0x004AC1B6 File Offset: 0x004AA3B6
		public bool Interactable
		{
			get
			{
				return this._isShow && this._isReady;
			}
		}

		// Token: 0x0600A02B RID: 41003 RVA: 0x004AC1CC File Offset: 0x004AA3CC
		private void PlayAnim(bool isShow)
		{
			Transform transform = base.transform;
			this._isShow = isShow;
			transform.DOLocalMoveY(isShow ? -720f : -1440f, 0.3f, false);
		}

		// Token: 0x0600A02C RID: 41004 RVA: 0x004AC204 File Offset: 0x004AA404
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Add(UiEvents.OnTravelStart, new GEvent.Callback(this.OnTravelStart));
			GEvent.Add(UiEvents.TaskBubbleStart, new GEvent.Callback(this.OnTaskBubbleStart));
			GEvent.Add(UiEvents.TaskBubbleEnded, new GEvent.Callback(this.OnTaskBubbleEnded));
			GEvent.Add(UiEvents.OnEventWindowStart, new GEvent.Callback(this.OnEventWindowStart));
			GEvent.Add(UiEvents.OnEventWindowEnded, new GEvent.Callback(this.OnEventWindowEnded));
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			GEvent.Add(UiEvents.MapFocusLocationGrave, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChange));
			GEvent.Add(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Add(EEvents.OnTutorialFunctionStatusChange, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Add(UiEvents.OnIronPlateDataChanged, new GEvent.Callback(this.OnIronPlateDataChanged));
			GEvent.Add(UiEvents.OnTravelPauseStatusChanged, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x0600A02D RID: 41005 RVA: 0x004AC36C File Offset: 0x004AA56C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Remove(UiEvents.OnTravelStart, new GEvent.Callback(this.OnTravelStart));
			GEvent.Remove(UiEvents.TaskBubbleStart, new GEvent.Callback(this.OnTaskBubbleStart));
			GEvent.Remove(UiEvents.TaskBubbleEnded, new GEvent.Callback(this.OnTaskBubbleEnded));
			GEvent.Remove(UiEvents.OnEventWindowStart, new GEvent.Callback(this.OnEventWindowStart));
			GEvent.Remove(UiEvents.OnEventWindowEnded, new GEvent.Callback(this.OnEventWindowEnded));
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			GEvent.Remove(UiEvents.MapFocusLocationGrave, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChange));
			GEvent.Remove(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Remove(EEvents.OnTutorialFunctionStatusChange, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Remove(UiEvents.OnIronPlateDataChanged, new GEvent.Callback(this.OnIronPlateDataChanged));
			GEvent.Remove(UiEvents.OnTravelPauseStatusChanged, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x0600A02E RID: 41006 RVA: 0x004AC4D3 File Offset: 0x004AA6D3
		private void OnReturnBottomTimeDisk(ArgumentBox _)
		{
			this.timeBall.transform.SetParent(this.timeBallRoot);
			this.timeBall.EnableTimeBall(true);
		}

		// Token: 0x170010E7 RID: 4327
		// (get) Token: 0x0600A02F RID: 41007 RVA: 0x004AC4FA File Offset: 0x004AA6FA
		public bool IsFocus
		{
			get
			{
				return UIManager.Instance.IsFocusElement(UIElement.Bottom);
			}
		}

		// Token: 0x0600A030 RID: 41008 RVA: 0x004AC50B File Offset: 0x004AA70B
		private void OnTopUiChanged(ArgumentBox _)
		{
			GEvent.OnEvent(UiEvents.NotifyBottomCustomButtonChange, null);
			this.OnMouseLeave();
			this.TriggerSetEscHandler();
			this.Refresh();
		}

		// Token: 0x0600A031 RID: 41009 RVA: 0x004AC534 File Offset: 0x004AA734
		private void RefreshCall(ArgumentBox _)
		{
			this.Refresh();
		}

		// Token: 0x0600A032 RID: 41010 RVA: 0x004AC53D File Offset: 0x004AA73D
		private void OpBtn()
		{
			this.OpBtnImpl(this._mapModel.SelectedBlock.GetLocation());
		}

		// Token: 0x0600A033 RID: 41011 RVA: 0x004AC556 File Offset: 0x004AA756
		private void OpBtnImpl(Location loc)
		{
			UIElement.BlockOperation.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set<Location>("Location", loc));
			UIManager.Instance.MaskUI(UIElement.BlockOperation);
		}

		// Token: 0x0600A034 RID: 41012 RVA: 0x004AC584 File Offset: 0x004AA784
		public static void ClickOpBtn(Location loc)
		{
			bool flag = ViewBottom.OpBtnCanOperate();
			if (flag)
			{
				ViewBottom viewBottom = UIElement.Bottom.UiBaseAs<ViewBottom>();
				if (viewBottom != null)
				{
					viewBottom.OpBtnImpl(loc);
				}
			}
		}

		// Token: 0x0600A035 RID: 41013 RVA: 0x004AC5B4 File Offset: 0x004AA7B4
		public static bool OpBtnCanOperate()
		{
			bool result;
			if (UIElement.Bottom.Exist)
			{
				ViewBottom viewBottom = UIElement.Bottom.UiBaseAs<ViewBottom>();
				if (viewBottom != null && viewBottom._isShow)
				{
					CButton cbutton = viewBottom.opBtn;
					if (cbutton != null)
					{
						result = cbutton.interactable;
						goto IL_35;
					}
				}
				result = false;
				IL_35:;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600A036 RID: 41014 RVA: 0x004AC5F9 File Offset: 0x004AA7F9
		private void ProfessionBtn()
		{
			UIManager.Instance.ShowUI(UIElement.Profession, true);
		}

		// Token: 0x0600A037 RID: 41015 RVA: 0x004AC610 File Offset: 0x004AA810
		private void SummaryBtn()
		{
			bool flag = SingletonObject.getInstance<BasicGameData>().CurrDate == GlobalConfig.Instance.GameStartDate;
			if (!flag)
			{
				UIElement.MonthNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", false));
				UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
			}
		}

		// Token: 0x0600A038 RID: 41016 RVA: 0x004AC666 File Offset: 0x004AA866
		private void MapFilter()
		{
			UIManager.Instance.MaskUI(UIElement.MapElementDisplayRule);
		}

		// Token: 0x0600A039 RID: 41017 RVA: 0x004AC67C File Offset: 0x004AA87C
		private void MapFind()
		{
			bool isShow = UIElement.FindMapBlock.IsShowing;
			this.TriggerSetEscHandler();
			bool flag = isShow;
			if (!flag)
			{
				UIElement.FindMapBlock.SetOnInitArgs(null);
				ViewFindMapBlock findMapBlockView = UIElement.FindMapBlock.UiBaseAs<ViewFindMapBlock>();
				bool flag2 = findMapBlockView != null;
				if (flag2)
				{
					findMapBlockView.OpenCloseAudio = UIBase.UIOpenCloseAudioType.SmallWindow;
				}
				UIElement.FindMapBlock.Show();
				UIManager.Instance.SetEscHandler(new Action(this.TriggerSetEscHandler));
			}
		}

		// Token: 0x0600A03A RID: 41018 RVA: 0x004AC6F0 File Offset: 0x004AA8F0
		private void MapLocate()
		{
			bool flag = UIElement.AdventureRemake.Exist || UIElement.AdventureMajorEvent.Exist;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.AdventureResetCamera, null);
			}
			else
			{
				bool exist = UIElement.PartWorld.Exist;
				if (exist)
				{
					ViewPartWorldMap.LookAtTaiwu();
				}
				else
				{
					GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", true));
					this.Refresh();
				}
			}
		}

		// Token: 0x0600A03B RID: 41019 RVA: 0x004AC76A File Offset: 0x004AA96A
		private void MapLocateTaiwuVillage()
		{
			ViewPartWorldMap.TravelToTaiwuVillage();
		}

		// Token: 0x0600A03C RID: 41020 RVA: 0x004AC774 File Offset: 0x004AA974
		private void WorldMap()
		{
			bool exist = UIElement.PartWorld.Exist;
			if (exist)
			{
				ViewPartWorldMap.Hide();
			}
			else
			{
				WorldMapModel mapModel = this._mapModel;
				bool flag = mapModel != null && mapModel.TaiwuMoveState == WorldMapModel.MoveState.Idle;
				if (flag)
				{
					UIElement.Bottom.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("KeepAnim", false));
					UIManager.Instance.ShowUI(UIElement.StatePartWorldMap, true);
				}
			}
		}

		// Token: 0x0600A03D RID: 41021 RVA: 0x004AC7E0 File Offset: 0x004AA9E0
		private void Teammate1()
		{
			this.ClickTeammates(1);
		}

		// Token: 0x0600A03E RID: 41022 RVA: 0x004AC7EA File Offset: 0x004AA9EA
		private void Teammate2()
		{
			this.ClickTeammates(2);
		}

		// Token: 0x0600A03F RID: 41023 RVA: 0x004AC7F4 File Offset: 0x004AA9F4
		private void Teammate3()
		{
			this.ClickTeammates(3);
		}

		// Token: 0x0600A040 RID: 41024 RVA: 0x004AC800 File Offset: 0x004AAA00
		private void ClickTeammates(int index)
		{
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			List<int> combatIds = monitor.GetTaiwuCombatTeamCharIds();
			bool flag = combatIds[index] < 0;
			if (flag)
			{
				this.SwitchTeammates();
			}
			else
			{
				TaiwuEventDomainMethod.Call.OnCharacterClicked(combatIds[index]);
			}
		}

		// Token: 0x0600A041 RID: 41025 RVA: 0x004AC840 File Offset: 0x004AAA40
		private void SwitchTeammates()
		{
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			List<int> combatIds = monitor.GetTaiwuCombatTeamCharIds();
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			List<int> charIds = monitor.GetTaiwuTeamCharIds();
			IReadOnlyCollection<int> specialGroup = monitor.GetTaiwuSpecialGroup();
			foreach (int specialChar in specialGroup)
			{
				charIds.Add(specialChar);
			}
			charIds.RemoveAll((int id) => id < 0 || id == taiwuCharId);
			List<int> initialSelected = new List<int>(3);
			bool flag = combatIds.Count > 0;
			if (flag)
			{
				combatIds.RemoveAt(0);
			}
			for (int i = 0; i < combatIds.Count; i++)
			{
				int id2 = combatIds[i];
				bool flag2 = id2 >= 0;
				if (flag2)
				{
					initialSelected.Add(id2);
				}
				bool flag3 = initialSelected.Count >= 3;
				if (flag3)
				{
					break;
				}
			}
			VillagerSelectCharacterSelectionHelper.OpenDefaultSelectChar(charIds, initialSelected, delegate(List<int> selectedIds)
			{
				bool selected = false;
				for (int j = 0; j < 3; j++)
				{
					int newCharId = (selectedIds != null && j < selectedIds.Count) ? selectedIds[j] : -1;
					selected |= (newCharId != -1);
					GameDataBridge.AddDataModification<int>(5, 32, (ulong)((long)j), uint.MaxValue, newCharId);
				}
				bool flag4 = !this._teammateChanged && selected;
				if (flag4)
				{
					this._teammateChanged = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(250);
				}
			}, ESelectCharacterInteractionMode.Slot, ESelectCharacterSelectionMode.Multiple, 3, this, null, null);
		}

		// Token: 0x0600A042 RID: 41026 RVA: 0x004AC970 File Offset: 0x004AAB70
		public override void OnInit(ArgumentBox argsBox)
		{
			TaiwuDomainMethod.AsyncCall.RequestActiveShortCut(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref CustomButton.SettingsCache);
				ViewBottom.TempShortCutsSettings = CustomButton.Settings;
				GEvent.OnEvent(UiEvents.NotifyBottomCustomButtonChange, null);
			});
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			this._isDreamBackUnlockBuildingButtonClicked = false;
			this.readAndLoop.Init(this);
			this.PlayAnim(this._canOpenCharacterMenu = (this._canOperateInCharacterMenu = (this._canExchangeChar = true)));
			this.Refresh();
			this.SetInteractable();
		}

		// Token: 0x0600A043 RID: 41027 RVA: 0x004AC9F4 File Offset: 0x004AABF4
		private IEnumerator DelaySetInteractable()
		{
			int num = this._interactableVersion + 1;
			this._interactableVersion = num;
			int interactableVersion = num;
			yield return new WaitForSeconds(0.3f);
			bool flag = interactableVersion == this._interactableVersion;
			if (flag)
			{
				GameObject gameObject = this.bottomMask;
				if (gameObject != null)
				{
					gameObject.SetActive(!(this._isReady = !ViewPartWorldMap.IsBlockInteraction));
				}
			}
			yield break;
		}

		// Token: 0x0600A044 RID: 41028 RVA: 0x004ACA04 File Offset: 0x004AAC04
		public static bool ForceDisable(AdventureRemakeModel adventureModel)
		{
			bool? flag;
			if (adventureModel == null)
			{
				flag = null;
			}
			else
			{
				AdventureTaiwu adventureTaiwu = adventureModel.AdventureTaiwu;
				flag = ((adventureTaiwu != null) ? new bool?(adventureTaiwu.InAdventure) : null);
			}
			bool? flag2 = flag;
			bool result;
			if (!flag2.GetValueOrDefault())
			{
				bool? flag3;
				if (adventureModel == null)
				{
					flag3 = null;
				}
				else
				{
					AdventureMajorEventTaiwu adventureMajorEventTaiwu = adventureModel.AdventureMajorEventTaiwu;
					flag3 = ((adventureMajorEventTaiwu != null) ? new bool?(adventureMajorEventTaiwu.InAdventure) : null);
				}
				flag2 = flag3;
				result = flag2.GetValueOrDefault();
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600A045 RID: 41029 RVA: 0x004ACA80 File Offset: 0x004AAC80
		public void Refresh()
		{
			WorldMapModel map = this._mapModel;
			this.RefreshCustomButtons();
			bool flag = this._currSeason != TimeKit.GetCurrSeason();
			if (flag)
			{
				this.RefreshSeasonAnim(this._currSeason = TimeKit.GetCurrSeason());
			}
			GameObject gameObject = this.bottomMask;
			if (gameObject != null)
			{
				gameObject.SetActive(!(this._isReady = false));
			}
			TaiwuEventDomainMethod.AsyncCall.CheckIsShowingEvent(this, delegate(int offset, RawDataPool pool)
			{
				bool isShowing = false;
				Serializer.Deserialize(pool, offset, ref isShowing);
				base.StartCoroutine(this.DelaySetInteractable());
			});
			bool forceDisable = ViewBottom.ForceDisable(SingletonObject.getInstance<AdventureRemakeModel>());
			bool bottomRightInteractable = !UIElement.PartWorld.Exist && (!UIElement.BuildingArea.Exist || UIManager.Instance.IsFocusElement(UIElement.StateMainWorld));
			bool flag2 = !UIElement.PartWorld.Exist && !UIElement.BuildingArea.Exist;
			FunctionLockManager functionLockManager = SingletonObject.getInstance<FunctionLockManager>();
			bool flag3 = !UIElement.MapBlockCharList.Exist || forceDisable;
			if (flag3)
			{
				this.buildingBg.sprite = this.regionRightSp;
				this.building.spriteState = this.regionRightSps;
				this.building.interactable = !forceDisable;
			}
			else
			{
				this.buildingBg.sprite = this.buildingSp;
				this.building.spriteState = this.buildingSps;
				Selectable selectable = this.building;
				bool interactable;
				if (!forceDisable && bottomRightInteractable)
				{
					if (!functionLockManager.IsFunctionUnlock(10))
					{
						if (SingletonObject.getInstance<TutorialChapterModel>().InGuiding)
						{
							short templateId = this._mapModel.CurrentBlockData.TemplateId;
							if (templateId == 17 || templateId == 18)
							{
								interactable = SingletonObject.getInstance<TutorialChapterModel>().GetFunctionStatus(0);
								goto IL_193;
							}
						}
						interactable = false;
						IL_193:;
					}
					else
					{
						interactable = true;
					}
				}
				else
				{
					interactable = false;
				}
				selectable.interactable = interactable;
			}
			bool exist = UIElement.PartWorld.Exist;
			if (exist)
			{
				this.worldMapBg.sprite = this.regionSp;
				this.worldMap.spriteState = this.regionSps;
				this.worldMap.interactable = !forceDisable;
				this.mapLocateTaiwuVillage.gameObject.SetActive(true);
				this.mapFilter.interactable = (this.mapFind.interactable = false);
				this.mapLocateTaiwuVillage.interactable = (this._mapModel.CurrentAreaId != this._mapModel.GetTaiwuVillageAreaId());
				this.mapLocateTaiwuVillage.GetComponent<TooltipInvoker>().PresetParam[1] = (this.mapLocateTaiwuVillage.interactable ? LanguageKey.UI_Reset_Map_Camera_To_Village_Content : LanguageKey.UI_DirectTravelToTaiwuVillage_MouseTip_Disable).Tr();
			}
			else
			{
				this.worldMapBg.sprite = this.worldMapSp;
				this.worldMap.spriteState = this.worldSps;
				this.worldMap.interactable = (!forceDisable && bottomRightInteractable && functionLockManager.IsFunctionUnlock(4));
				this.mapLocateTaiwuVillage.gameObject.SetActive(false);
				this.mapFilter.interactable = (this.mapFind.interactable = !forceDisable);
			}
			this.OnLanguageChange(null);
			this.readAndLoop.RequestData();
			TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
			this.opBtn.interactable = (bottomRightInteractable && !forceDisable && map.CurrentAreaId == map.ShowingAreaId && (functionLockManager.IsFunctionUnlock(5) || (tutorialChapterModel.InGuiding && tutorialChapterModel.GetFunctionStatus(17))));
			this.UpdateTaiwuCarrier(null);
			this.mapPickupWindow.gameObject.SetActive(false);
			this.RefreshCharacterAvatar(null);
			this.mapPickupWindow.RefreshPickupItemInfos();
			bool isProfessionUnlocked = functionLockManager.IsFunctionUnlock(27) || (tutorialChapterModel.InGuiding && tutorialChapterModel.GetFunctionStatus(18));
			this.professionBtn.interactable = (this.professionBtn.GetComponent<PointerTrigger>().enabled = (!forceDisable && isProfessionUnlocked && map.CurrentAreaId == map.ShowingAreaId));
			this.professionBottomMenu.Init(this);
			this.professionBottomMenu.RefreshByEvent(null);
			IronPlateData ironPlateData = SingletonObject.getInstance<WorldMapModel>().IronPlateData;
			this.ironPlate.Refresh(ironPlateData, false);
		}

		// Token: 0x0600A046 RID: 41030 RVA: 0x004ACE9B File Offset: 0x004AB09B
		private void RefreshCharacterAvatar(ArgumentBox _ = null)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuCombatTeamCharIds(), delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> data = new List<CharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref data);
				this.RefreshChar(data, 0, this.taiwuAvatar, null, null);
				this.RefreshChar(data, 1, this.teammate1Avatar, this.teammate1Mark, this.teammateExchangeBtnBase[0]);
				this.RefreshChar(data, 2, this.teammate2Avatar, this.teammate2Mark, this.teammateExchangeBtnBase[1]);
				this.RefreshChar(data, 3, this.teammate3Avatar, this.teammate3Mark, this.teammateExchangeBtnBase[2]);
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x0600A047 RID: 41031 RVA: 0x004ACEBC File Offset: 0x004AB0BC
		private void RefreshChar(List<CharacterDisplayData> data, int index, Game.Components.Avatar.Avatar avatar, CImage image, GameObject exchangeBase)
		{
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				bool quiting = GameApp.Quiting;
				if (!quiting)
				{
					bool readyToQuit = GameApp.ReadyToQuit;
					if (!readyToQuit)
					{
						bool flag2 = data.Count > index && data[index].CharacterId != -1;
						if (flag2)
						{
							avatar.Refresh(data[index], true);
							avatar.gameObject.SetActive(true);
							if (image != null)
							{
								image.gameObject.SetActive(false);
							}
							if (exchangeBase != null)
							{
								exchangeBase.SetActive(true);
							}
						}
						else
						{
							avatar.gameObject.SetActive(false);
							if (image != null)
							{
								image.gameObject.SetActive(true);
							}
							if (exchangeBase != null)
							{
								exchangeBase.SetActive(false);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A048 RID: 41032 RVA: 0x004ACF90 File Offset: 0x004AB190
		private void Update()
		{
			bool flag = !this.Interactable;
			if (!flag)
			{
				bool flag2 = TipsCommandKit.GlobalTipsHide.Check(this.Element, false, false, false, true, true);
				if (flag2)
				{
					GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
					settings.GlobalTipsHide = !settings.GlobalTipsHide;
					settings.SaveSettings();
					ViewSystemOption viewSystemOption = UIElement.SystemOption.UiBaseAs<ViewSystemOption>();
					if (viewSystemOption != null)
					{
						viewSystemOption.UpdateHotKeyDisplay();
					}
				}
				bool isCovered = this._uiViewCoveredBehaviour.IsCovered;
				if (!isCovered)
				{
					this.UpdateMapElementPanel();
					this.UpdateMapFindBox();
					bool flag3 = !this.IsFocus;
					if (!flag3)
					{
						bool flag4 = !this.timeBall.IsProcessing && !GameApp.AdvancingMonth && MainInterfaceCommandKit.MonthPass.Check(this.Element, false, false, false, true, false);
						if (flag4)
						{
							GEvent.OnEvent(EEvents.RequestAdvanceMonth, null);
						}
						bool flag5 = UIElement.AdvanceDays.Exist && CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
						if (flag5)
						{
							UIManager.Instance.HideUI(UIElement.AdvanceDays);
						}
						bool flag6 = this.mapLocate.interactable && MapCommandKit.FocusOnTaiwuBlock.Check(this.Element, false, false, false, true, false);
						if (flag6)
						{
							this.MapLocate();
						}
						this.HandleHotKeyCommon();
						this.HandleHotKeyForShowCharacterMenuSubPage(this.Element);
					}
				}
			}
		}

		// Token: 0x0600A049 RID: 41033 RVA: 0x004AD0F4 File Offset: 0x004AB2F4
		private void OnSetBottomInteractable(ArgumentBox argumentBox)
		{
			argumentBox.Get("canOpenCharacterMenu", out this._canOpenCharacterMenu);
			argumentBox.Get("canOperateInCharacterMenu", out this._canOperateInCharacterMenu);
			argumentBox.Get("canExchangeChar", out this._canExchangeChar);
			this.SetInteractable();
		}

		// Token: 0x0600A04A RID: 41034 RVA: 0x004AD134 File Offset: 0x004AB334
		private void SetInteractable()
		{
			this.mainCharacterButton.interactable = this._canOpenCharacterMenu;
			this.changeTeammate1.interactable = (this.changeTeammate2.interactable = (this.changeTeammate3.interactable = this._canExchangeChar));
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			List<int> combatIds = monitor.GetTaiwuCombatTeamCharIds();
			this.teammate1.interactable = (this._canExchangeChar || combatIds[1] >= 0);
			this.teammate2.interactable = (this._canExchangeChar || combatIds[2] >= 0);
			this.teammate3.interactable = (this._canExchangeChar || combatIds[3] >= 0);
		}

		// Token: 0x0600A04B RID: 41035 RVA: 0x004AD1FD File Offset: 0x004AB3FD
		private void UpdateTaiwuCarrier(ArgumentBox _)
		{
			this.mapPickupWindow.RefreshCarrierInfo(this._mapModel.TaiwuExploreBonusRate);
		}

		// Token: 0x0600A04C RID: 41036 RVA: 0x004AD217 File Offset: 0x004AB417
		private void OnTravelStart(ArgumentBox _)
		{
			this.mapLocateTaiwuVillage.gameObject.SetActive(false);
		}

		// Token: 0x0600A04D RID: 41037 RVA: 0x004AD22C File Offset: 0x004AB42C
		private void OnPickUpDisplayInfoChange(ArgumentBox argBox)
		{
			MapBlockData blockData;
			argBox.Get<MapBlockData>("MapBlockData", out blockData);
			bool refreshOnly;
			argBox.Get("RefreshOnly", out refreshOnly);
			bool flag = refreshOnly;
			if (flag)
			{
				this.mapPickupWindow.RefreshPickupItemInfos();
			}
			else
			{
				this.mapPickupWindow.RefreshPickupItemInfos(blockData);
			}
		}

		// Token: 0x0600A04E RID: 41038 RVA: 0x004AD276 File Offset: 0x004AB476
		private void OnCurrentPickUpDisplayInfoChange(ArgumentBox argBox)
		{
			this.mapPickupWindow.RefreshPickupItemInfos();
		}

		// Token: 0x0600A04F RID: 41039 RVA: 0x004AD285 File Offset: 0x004AB485
		private void CancelPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x0600A050 RID: 41040 RVA: 0x004AD295 File Offset: 0x004AB495
		private void StartPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600A051 RID: 41041 RVA: 0x004AD2A8 File Offset: 0x004AB4A8
		private bool IsPointerOverGameObject(GameObject target)
		{
			RaycastAllManager manager = SingletonObject.getInstance<RaycastAllManager>();
			bool flag = manager == null || target == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<RaycastResult> _raycastResults = manager.GetCurrentFrameResults();
				bool flag2 = _raycastResults.Count == 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					GameObject firstHit = _raycastResults[0].gameObject;
					bool flag3 = firstHit == null;
					result = (!flag3 && (firstHit == target || firstHit.transform.IsChildOf(target.transform)));
				}
			}
			return result;
		}

		// Token: 0x0600A052 RID: 41042 RVA: 0x004AD33C File Offset: 0x004AB53C
		private void UpdateMapElementPanel()
		{
			bool flag = !this.mapFilter.interactable;
			if (!flag)
			{
				bool isHoverMapFilter = this.IsPointerOverGameObject(this.mapFilter.gameObject);
				bool isHoverMapElementDisplayRulePanel = this.mapElementDisplayRulePanel.gameObject.activeSelf && this.IsPointerOverGameObject(this.mapElementDisplayRulePanel.gameObject);
				bool showMapElementDisplayRulePanel = isHoverMapFilter || isHoverMapElementDisplayRulePanel;
				bool flag2 = this.mapElementDisplayRulePanel.gameObject.activeSelf != showMapElementDisplayRulePanel;
				if (flag2)
				{
					this.mapElementDisplayRulePanel.gameObject.SetActive(showMapElementDisplayRulePanel);
				}
			}
		}

		// Token: 0x0600A053 RID: 41043 RVA: 0x004AD3CC File Offset: 0x004AB5CC
		private void UpdateMapFindBox()
		{
			bool flag = !this.mapFind.interactable;
			if (!flag)
			{
				bool isHoverMapFind = this.IsPointerOverGameObject(this.mapFind.gameObject);
				bool isHoverMapFindBoxPanel = this.mapFindBoxPanel.gameObject.activeSelf && this.IsPointerOverGameObject(this.mapFindBoxPanel.gameObject);
				bool showMapFindBoxPanel = isHoverMapFind || isHoverMapFindBoxPanel;
				bool flag2 = this.mapFindBoxPanel.gameObject.activeSelf != showMapFindBoxPanel;
				if (flag2)
				{
					this.mapFindBoxPanel.gameObject.SetActive(showMapFindBoxPanel);
				}
			}
		}

		// Token: 0x0600A054 RID: 41044 RVA: 0x004AD45C File Offset: 0x004AB65C
		private void HandleHotKeyCommon()
		{
			bool flag = MainInterfaceCommandKit.ViewCharacterPanel.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.ShowCharacterMenu(ECharacterSubToggleBase.None, ECharacterSubPage.None, -1);
			}
			bool flag2 = MainInterfaceCommandKit.TaiwuMonthlyReport.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag2)
			{
				this.SummaryBtn();
			}
			bool flag3 = this.building.interactable && MainInterfaceCommandKit.IndustryView.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			if (flag3)
			{
				this.Building();
			}
			bool flag4 = MainInterfaceCommandKit.ReadBook.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag4)
			{
				UIManager.Instance.ShowUI(UIElement.Reading, true);
			}
			bool flag5 = MainInterfaceCommandKit.Looping.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag5)
			{
				TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(this, delegate(int offset, RawDataPool dataPool)
				{
					LoopingViewDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					UIElement.Looping.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("LoopingViewDisplayData", displayData));
					UIManager.Instance.ShowUI(UIElement.Looping, true);
				});
			}
			bool flag6 = MainInterfaceFunctionCommandKit.ShowInstantNotificationEvent.Check(this.Element, false, false, false, true, false);
			if (flag6)
			{
				UIManager.Instance.MaskUI(UIElement.InstantNotification);
			}
			bool flag7 = MainInterfaceFunctionCommandKit.LegendaryBook.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(21);
			if (flag7)
			{
				UIManager.Instance.ShowUI(UIElement.LegendaryBook, true);
			}
			bool flag8 = MainInterfaceFunctionCommandKit.VillagerList.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			if (flag8)
			{
				UIElement.TaiwuVillagers.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
				UIManager.Instance.MaskUI(UIElement.TaiwuVillagers);
			}
			bool flag9 = MainInterfaceFunctionCommandKit.DispatchList.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			if (flag9)
			{
				ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
				argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
				argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleAssign);
				UIElement.VillagerRole.SetOnInitArgs(argbox);
				UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
			}
			bool flag10 = MainInterfaceFunctionCommandKit.SettlementInformation.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(13);
			if (flag10)
			{
				UIElement.SettlementInformation.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SettlementId", SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId()));
				UIManager.Instance.ShowUI(UIElement.SettlementInformation, true);
			}
			bool flag11 = MainInterfaceFunctionCommandKit.FollowCharacter.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag11)
			{
				UIManager.Instance.ShowUI(UIElement.Following, true);
			}
			bool flag12 = MainInterfaceFunctionCommandKit.TaiwuLegacy.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			if (flag12)
			{
				UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", false));
				UIManager.Instance.MaskUI(UIElement.Legacy);
			}
			bool flag13 = MainInterfaceFunctionCommandKit.ViewScroll.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag13)
			{
				UIElement.GameLineScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("index", -1).Set("targetScrollIndex", 2));
				UIManager.Instance.MaskUI(UIElement.GameLineScroll);
			}
			bool flag14 = MainInterfaceFunctionCommandKit.InscriptionCharacter.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			if (flag14)
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("CanDelete", true);
				UIElement.CheckInscription.SetOnInitArgs(box);
				UIManager.Instance.ShowUI(UIElement.CheckInscription, true);
			}
			bool flag15 = MainInterfaceFunctionCommandKit.TaiwuEncyclopedia.Check(this.Element, false, false, false, true, false);
			if (flag15)
			{
				UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
			}
			bool flag16 = MainInterfaceFunctionCommandKit.Achievement.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag16)
			{
				UIManager.Instance.MaskUI(UIElement.Achievement);
			}
			bool flag17 = MainInterfaceFunctionCommandKit.Heal.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag17)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
				List<int> teamCharList = monitor.GetTaiwuTeamCharIds();
				teamCharList.AddRange(monitor.GetTaiwuSpecialGroup());
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				args.SetObject("DoctorList", teamCharList);
				List<int> patientList = new List<int>();
				patientList.AddRange(teamCharList);
				args.SetObject("PatientList", patientList);
				args.Set("NeedPay", false);
				args.Set("CurrentCharacterId", taiwuCharId);
				CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, taiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					KidnappedCharacterList kidnappedCharacterList = null;
					Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
					bool flag28 = kidnappedCharacterList != null;
					if (flag28)
					{
						for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
						{
							patientList.Add(kidnappedCharacterList.Get(i).CharId);
						}
					}
					UIElement.Heal.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.Heal, true);
				});
			}
			bool flag18 = MapCommandKit.Legend.Check(this.Element, false, false, false, true, false);
			if (flag18)
			{
				this.mapElementDisplayRulePanel.ToggleAll();
			}
			bool flag19 = MapCommandKit.SearchResult.Check(this.Element, false, false, false, true, false);
			if (flag19)
			{
				this.mapFindBoxPanel.ChangeMapFindToggleValue();
			}
			bool flag20 = MainInterfaceFunctionCommandKit.TaiwuNotes.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag20)
			{
				UIElement.TaskPopPanel.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
				UIManager.Instance.MaskUI(UIElement.TaskPopPanel);
			}
			bool flag21 = this.worldMap.interactable && MainInterfaceCommandKit.PartWorldMap.Check(this.Element, false, false, false, true, false) && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(4);
			if (flag21)
			{
				this.WorldMap();
			}
			bool flag22 = MainInterfaceCommandKit.WorldState.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag22)
			{
				UIManager.Instance.MaskUI(UIElement.WorldStatePanel);
			}
			bool flag23 = !UIElement.TutorialGuidingChapterTip.IsShowing && MainInterfaceFunctionCommandKit.Tutorial.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag23)
			{
				UIManager.Instance.MaskUI(UIElement.TutorialGuidingChapter);
			}
			bool flag24 = MainInterfaceFunctionCommandKit.Loop.Check(this.Element, false, true, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag24)
			{
				this.readAndLoop.OnActiveLoopPointerDown(null);
			}
			else
			{
				bool flag25 = MainInterfaceFunctionCommandKit.Loop.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (flag25)
				{
					this.readAndLoop.OnActiveLoopPointerUp();
				}
			}
			bool flag26 = MainInterfaceFunctionCommandKit.Read.Check(this.Element, false, true, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag26)
			{
				this.readAndLoop.OnActiveReadPointerDown(null);
			}
			else
			{
				bool flag27 = MainInterfaceFunctionCommandKit.Read.Check(this.Element, false, false, false, true, false) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (flag27)
				{
					this.readAndLoop.OnActiveReadPointerUp();
				}
			}
		}

		// Token: 0x0600A055 RID: 41045 RVA: 0x004ADC48 File Offset: 0x004ABE48
		private void HandleHotKeyForShowCharacterMenuSubPage(UIElement element)
		{
			foreach (KeyValuePair<HotKeyCommand, ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>> mapping in ViewBottom.CharacterMenuHotKeyMappings)
			{
				bool flag = mapping.Key.Check(element, false, false, false, true, false);
				if (flag)
				{
					this.ShowCharacterMenu(mapping.Value.Item1, mapping.Value.Item2, mapping.Value.Item3);
				}
			}
		}

		// Token: 0x0600A056 RID: 41046 RVA: 0x004ADCD8 File Offset: 0x004ABED8
		private void ShowCharacterMenu(ECharacterSubToggleBase targetSubToggle = ECharacterSubToggleBase.None, ECharacterSubPage targetSubPage = ECharacterSubPage.None, int baseAttributeIndex = -1)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			argBox.Set("CanOperate", this._canOperateInCharacterMenu);
			bool flag = targetSubToggle != ECharacterSubToggleBase.None;
			if (flag)
			{
				argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(targetSubToggle, targetSubPage));
			}
			AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
			bool inAdventure = model.AdventureMajorEventTaiwu.InAdventure;
			if (inAdventure)
			{
				argBox.Set("PreviousView", 2);
			}
			TaiwuEventDomainMethod.AsyncCall.CheckIsShowingEvent(null, delegate(int offset, RawDataPool dataPoll)
			{
				bool hasEvent = false;
				Serializer.Deserialize(dataPoll, offset, ref hasEvent);
				bool flag2 = hasEvent;
				if (!flag2)
				{
					UIElement.CharacterMenu.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				}
			});
		}

		// Token: 0x0600A057 RID: 41047 RVA: 0x004ADD8B File Offset: 0x004ABF8B
		private void OnTaskBubbleStart(ArgumentBox box)
		{
			this._isTaskBubbleDisplaying = true;
		}

		// Token: 0x0600A058 RID: 41048 RVA: 0x004ADD95 File Offset: 0x004ABF95
		private void OnTaskBubbleEnded(ArgumentBox box)
		{
			this._isTaskBubbleDisplaying = false;
		}

		// Token: 0x0600A059 RID: 41049 RVA: 0x004ADD9F File Offset: 0x004ABF9F
		private void OnEventWindowStart(ArgumentBox box)
		{
			this._isEventWindowDisplaying = true;
		}

		// Token: 0x0600A05A RID: 41050 RVA: 0x004ADDA9 File Offset: 0x004ABFA9
		private void OnEventWindowEnded(ArgumentBox box)
		{
			this._isEventWindowDisplaying = false;
		}

		// Token: 0x0600A05B RID: 41051 RVA: 0x004ADDB3 File Offset: 0x004ABFB3
		private void OnActionPointChange(ArgumentBox _)
		{
			this.SendTeammateBubbleRequest();
		}

		// Token: 0x0600A05C RID: 41052 RVA: 0x004ADDBC File Offset: 0x004ABFBC
		private void SendTeammateBubbleRequest()
		{
			bool isAbleToDisplayBubble = this._isAbleToDisplayBubble;
			if (isAbleToDisplayBubble)
			{
				MapDomainMethod.AsyncCall.GetReversedTeammateBubble(this, WorldMapModel.Traveling, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._data);
					List<TransferableRecord> record = this._data.Record;
					bool flag;
					if (record != null && record.Count > 0)
					{
						TransferableRecord transferableRecord = this._data.Record[0];
						if (transferableRecord != null)
						{
							List<ValueTuple<sbyte, int>> arguments = transferableRecord.Arguments;
							if (arguments != null)
							{
								flag = (arguments.Count > 2);
								goto IL_52;
							}
						}
						flag = false;
						IL_52:;
					}
					else
					{
						flag = false;
					}
					bool flag2 = flag;
					if (flag2)
					{
						PositionFollower positionFollower = this.bubbleFollower;
						List<ValueTuple<sbyte, int>> arguments2 = this._data.Record[0].Arguments;
						int item = arguments2[arguments2.Count - 2].Item2;
						if (!true)
						{
						}
						CButton cbutton;
						switch (item)
						{
						case 0:
							cbutton = this.teammate1;
							break;
						case 1:
							cbutton = this.teammate2;
							break;
						case 2:
							cbutton = this.teammate3;
							break;
						default:
							cbutton = null;
							break;
						}
						if (!true)
						{
						}
						positionFollower.Target = ((cbutton != null) ? cbutton.transform : null);
						this.bubbleText.text = this.<SendTeammateBubbleRequest>g__DataRender|161_1(this._data.Record[0], this._data);
						this.bubbleFollower.gameObject.SetActive(true);
					}
				});
			}
		}

		// Token: 0x0600A05D RID: 41053 RVA: 0x004ADDEC File Offset: 0x004ABFEC
		private void DisableTeammateBubble()
		{
			this.bubbleFollower.gameObject.SetActive(false);
		}

		// Token: 0x0600A05E RID: 41054 RVA: 0x004ADE01 File Offset: 0x004AC001
		private void EnableTeammateBubble()
		{
			this._isAbleToDisplayBubble = true;
		}

		// Token: 0x0600A05F RID: 41055 RVA: 0x004ADE0C File Offset: 0x004AC00C
		private void OnIronPlateDataChanged(ArgumentBox argumentBox)
		{
			bool isNewUnlock;
			argumentBox.Get("isNewUnlock", out isNewUnlock);
			IronPlateData ironPlateData = SingletonObject.getInstance<WorldMapModel>().IronPlateData;
			this.ironPlate.Refresh(ironPlateData, isNewUnlock);
		}

		// Token: 0x0600A062 RID: 41058 RVA: 0x004AE00D File Offset: 0x004AC20D
		[CompilerGenerated]
		internal static void <OnMouseHover>g__OnIdleAnimationComplete|67_0(TrackEntry entry)
		{
			entry.Complete -= ViewBottom.<OnMouseHover>g__OnIdleAnimationComplete|67_0;
		}

		// Token: 0x0600A063 RID: 41059 RVA: 0x004AE022 File Offset: 0x004AC222
		[CompilerGenerated]
		internal static void <RefreshSeasonAnim>g__OnInAnimationComplete|71_0(TrackEntry entry)
		{
			entry.Complete -= ViewBottom.<RefreshSeasonAnim>g__OnInAnimationComplete|71_0;
		}

		// Token: 0x0600A06A RID: 41066 RVA: 0x004AE2D4 File Offset: 0x004AC4D4
		[CompilerGenerated]
		private string <SendTeammateBubbleRequest>g__DataRender|161_1(TransferableRecord record, TransferableRecordDataBase data)
		{
			TeammateBubbleItem config = TeammateBubble.Instance[record.RecordType];
			bool flag = config != null;
			string result;
			if (flag)
			{
				YieldHelper helper = SingletonObject.getInstance<YieldHelper>();
				helper.DelaySecondsDo((float)config.Duration / 60f, new Action(this.DisableTeammateBubble));
				helper.DelaySecondsDo((float)config.Duration / 60f + 3f, new Action(this.EnableTeammateBubble));
				TeammateBubbleItem config2 = config;
				List<ValueTuple<sbyte, int>> arguments = record.Arguments;
				int item = arguments[arguments.Count - 1].Item2;
				List<ValueTuple<sbyte, int>> arguments2 = record.Arguments;
				result = string.Format(TeammateBubbleSubType.GetStringByType(config2, item, (short)arguments2[arguments2.Count - 3].Item2), (from x in record.Arguments
				select GameMessageUtils.ReadArguments(x.Item1, x.Item2, data)).ToArray<object>()).ColorReplace();
			}
			else
			{
				Debug.LogWarning(string.Format("Invalid record type: {0}", record.RecordType));
				result = "";
			}
			return result;
		}

		// Token: 0x04007BEA RID: 31722
		[SerializeField]
		private MapPickupsWindow mapPickupWindow;

		// Token: 0x04007BEB RID: 31723
		[SerializeField]
		private BottomProfessionBottomMenu professionBottomMenu;

		// Token: 0x04007BEC RID: 31724
		[SerializeField]
		private GameObject bottomMask;

		// Token: 0x04007BED RID: 31725
		[SerializeField]
		private ReadAndLoop readAndLoop;

		// Token: 0x04007BEE RID: 31726
		[SerializeField]
		private CButton opBtn;

		// Token: 0x04007BEF RID: 31727
		[SerializeField]
		private CButton professionBtn;

		// Token: 0x04007BF0 RID: 31728
		[SerializeField]
		private CButton summaryBtn;

		// Token: 0x04007BF1 RID: 31729
		[SerializeField]
		private CButton building;

		// Token: 0x04007BF2 RID: 31730
		[SerializeField]
		private CButton mapFilter;

		// Token: 0x04007BF3 RID: 31731
		[SerializeField]
		private CButton mapFind;

		// Token: 0x04007BF4 RID: 31732
		[SerializeField]
		private CButton mapLocate;

		// Token: 0x04007BF5 RID: 31733
		[SerializeField]
		private CButton mapLocateTaiwuVillage;

		// Token: 0x04007BF6 RID: 31734
		[SerializeField]
		private CButton worldMap;

		// Token: 0x04007BF7 RID: 31735
		[SerializeField]
		private CButton mainCharacterButton;

		// Token: 0x04007BF8 RID: 31736
		[SerializeField]
		private CButton teammate1;

		// Token: 0x04007BF9 RID: 31737
		[SerializeField]
		private CButton teammate2;

		// Token: 0x04007BFA RID: 31738
		[SerializeField]
		private CButton teammate3;

		// Token: 0x04007BFB RID: 31739
		[SerializeField]
		private CButton changeTeammate1;

		// Token: 0x04007BFC RID: 31740
		[SerializeField]
		private CButton changeTeammate2;

		// Token: 0x04007BFD RID: 31741
		[SerializeField]
		private CButton changeTeammate3;

		// Token: 0x04007BFE RID: 31742
		[SerializeField]
		private CButton shortCutConfig;

		// Token: 0x04007BFF RID: 31743
		[SerializeField]
		private CButton shortCutConfigHide;

		// Token: 0x04007C00 RID: 31744
		[SerializeField]
		private CButton shortCutConfigSure;

		// Token: 0x04007C01 RID: 31745
		[SerializeField]
		private TimeBall timeBall;

		// Token: 0x04007C02 RID: 31746
		[SerializeField]
		private RectTransform animRoot;

		// Token: 0x04007C03 RID: 31747
		[SerializeField]
		private RectTransform timeBallRoot;

		// Token: 0x04007C04 RID: 31748
		[SerializeField]
		private RectTransform shortCutConfigSettings;

		// Token: 0x04007C05 RID: 31749
		[SerializeField]
		private Game.Components.Avatar.Avatar taiwuAvatar;

		// Token: 0x04007C06 RID: 31750
		[SerializeField]
		private Game.Components.Avatar.Avatar teammate1Avatar;

		// Token: 0x04007C07 RID: 31751
		[SerializeField]
		private Game.Components.Avatar.Avatar teammate2Avatar;

		// Token: 0x04007C08 RID: 31752
		[SerializeField]
		private Game.Components.Avatar.Avatar teammate3Avatar;

		// Token: 0x04007C09 RID: 31753
		[SerializeField]
		private CImage teammate1Mark;

		// Token: 0x04007C0A RID: 31754
		[SerializeField]
		private CImage teammate2Mark;

		// Token: 0x04007C0B RID: 31755
		[SerializeField]
		private CImage teammate3Mark;

		// Token: 0x04007C0C RID: 31756
		[SerializeField]
		private GameObject[] teammateExchangeBtnBase;

		// Token: 0x04007C0D RID: 31757
		[SerializeField]
		private TooltipInvoker monthTipDisplayer;

		// Token: 0x04007C0E RID: 31758
		[SerializeField]
		private Sprite regionSp;

		// Token: 0x04007C0F RID: 31759
		[SerializeField]
		private Sprite worldMapSp;

		// Token: 0x04007C10 RID: 31760
		[SerializeField]
		private Sprite regionRightSp;

		// Token: 0x04007C11 RID: 31761
		[SerializeField]
		private Sprite buildingSp;

		// Token: 0x04007C12 RID: 31762
		[SerializeField]
		private Sprite[] worldMapTextActive;

		// Token: 0x04007C13 RID: 31763
		[SerializeField]
		private Sprite[] worldMapTextInactive;

		// Token: 0x04007C14 RID: 31764
		[SerializeField]
		private Sprite[] buildingTextActive;

		// Token: 0x04007C15 RID: 31765
		[SerializeField]
		private Sprite[] buildingTextInactive;

		// Token: 0x04007C16 RID: 31766
		[SerializeField]
		private Sprite[] regionTextActive;

		// Token: 0x04007C17 RID: 31767
		[SerializeField]
		private Sprite[] regionTextInactive;

		// Token: 0x04007C18 RID: 31768
		[SerializeField]
		private CImage worldMapText;

		// Token: 0x04007C19 RID: 31769
		[SerializeField]
		private CImage buildingText;

		// Token: 0x04007C1A RID: 31770
		[SerializeField]
		private CImage worldMapBg;

		// Token: 0x04007C1B RID: 31771
		[SerializeField]
		private CImage buildingBg;

		// Token: 0x04007C1C RID: 31772
		[SerializeField]
		private SpriteState worldSps;

		// Token: 0x04007C1D RID: 31773
		[SerializeField]
		private SpriteState regionSps;

		// Token: 0x04007C1E RID: 31774
		[SerializeField]
		private SpriteState buildingSps;

		// Token: 0x04007C1F RID: 31775
		[SerializeField]
		private SpriteState regionRightSps;

		// Token: 0x04007C20 RID: 31776
		[SerializeField]
		private IronPlate ironPlate;

		// Token: 0x04007C21 RID: 31777
		[SerializeField]
		private GameObject monthlyReportInteractArea;

		// Token: 0x04007C22 RID: 31778
		private sbyte _currSeason = -1;

		// Token: 0x04007C23 RID: 31779
		[SerializeField]
		private Sprite[] seasonBgs;

		// Token: 0x04007C24 RID: 31780
		[SerializeField]
		private CImage seasonBg;

		// Token: 0x04007C25 RID: 31781
		[SerializeField]
		private SkeletonGraphic[] seasonAnims;

		// Token: 0x04007C26 RID: 31782
		[SerializeField]
		private GameObject summaryHoverGo;

		// Token: 0x04007C27 RID: 31783
		[Header("图例设置面板")]
		[SerializeField]
		private MapElementDisplayRulePanel mapElementDisplayRulePanel;

		// Token: 0x04007C28 RID: 31784
		[SerializeField]
		private MapFindBoxPanel mapFindBoxPanel;

		// Token: 0x04007C29 RID: 31785
		private UIViewCoveredBehaviour _uiViewCoveredBehaviour;

		// Token: 0x04007C2A RID: 31786
		private static int _lastDate = -1;

		// Token: 0x04007C2B RID: 31787
		[SerializeField]
		private CustomButton customButtonPrefab;

		// Token: 0x04007C2C RID: 31788
		[SerializeField]
		private GameObject categoryPrefab;

		// Token: 0x04007C2D RID: 31789
		[SerializeField]
		private CustomButton[] bottomShortCuts;

		// Token: 0x04007C2E RID: 31790
		[SerializeField]
		private RectTransform container;

		// Token: 0x04007C2F RID: 31791
		[SerializeField]
		private TMP_Text containerName;

		// Token: 0x04007C30 RID: 31792
		public static int[] TempShortCutsSettings;

		// Token: 0x04007C31 RID: 31793
		private bool _isDreamBackUnlockBuildingButtonClicked;

		// Token: 0x04007C32 RID: 31794
		private bool _isShow = true;

		// Token: 0x04007C33 RID: 31795
		private bool _isReady = false;

		// Token: 0x04007C34 RID: 31796
		private bool _teammateChanged;

		// Token: 0x04007C35 RID: 31797
		private WorldMapModel _mapModel;

		// Token: 0x04007C36 RID: 31798
		private int _interactableVersion;

		// Token: 0x04007C37 RID: 31799
		private bool _canOpenCharacterMenu;

		// Token: 0x04007C38 RID: 31800
		private bool _canOperateInCharacterMenu;

		// Token: 0x04007C39 RID: 31801
		private bool _canExchangeChar;

		// Token: 0x04007C3A RID: 31802
		private static readonly Dictionary<HotKeyCommand, ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>> CharacterMenuHotKeyMappings = new Dictionary<HotKeyCommand, ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>>
		{
			{
				CharacterMenuCommandKit.ShowCharacterMenuCharacter,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuTeam,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Team, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuKidnap,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Prison, 1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuEquip,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.EquipmentBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuCarrier,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.EquipmentBase, ECharacterSubPage.Vehicle, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuItems,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuAttainments,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.Attainment, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuCombatAttainments,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentCombatSkill, 1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuLifeSkillAttainments,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill, 2)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuBreakout,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuNieli,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.NeiliBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuEquipCombatSkill,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.EquipCombatSkillBase, ECharacterSubPage.None, 1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuRelationship,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.RelationshipBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuFamilyTree,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.RelationshipBase, ECharacterSubPage.Genealogy, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuLifeRecords,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.StoryBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuLifeInformation,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.InformationBase, ECharacterSubPage.None, -1)
			},
			{
				CharacterMenuCommandKit.ShowCharacterMenuSecretInformation,
				new ValueTuple<ECharacterSubToggleBase, ECharacterSubPage, int>(ECharacterSubToggleBase.InformationBase, ECharacterSubPage.Secret, -1)
			}
		};

		// Token: 0x04007C3B RID: 31803
		private TeammateBubbleCollection _teammateBubbleCollection;

		// Token: 0x04007C3C RID: 31804
		private ArgumentCollection _teammateBubbleArgumentCollection = new ArgumentCollection();

		// Token: 0x04007C3D RID: 31805
		private TeammateBubbleRenderInfo _renderInfo;

		// Token: 0x04007C3E RID: 31806
		private bool _isAbleToDisplayBubble = true;

		// Token: 0x04007C3F RID: 31807
		private bool _isTaskBubbleDisplaying = false;

		// Token: 0x04007C40 RID: 31808
		private bool _isEventWindowDisplaying = false;

		// Token: 0x04007C41 RID: 31809
		private TeammateBubbleItem _bubbleConfig;

		// Token: 0x04007C42 RID: 31810
		[SerializeField]
		private TMP_Text bubbleText;

		// Token: 0x04007C43 RID: 31811
		[SerializeField]
		private PositionFollower bubbleFollower;

		// Token: 0x04007C44 RID: 31812
		private TransferableRecordDataBase _data = new TransferableRecordDataBase();
	}
}
