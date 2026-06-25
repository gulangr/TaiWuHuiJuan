using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Map;
using Game.Views.VillagerRoleView;
using GameData.Domains.Building;
using GameData.Domains.Information;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C32 RID: 3122
	[RequireComponent(typeof(CButton), typeof(CImage))]
	public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x170010C3 RID: 4291
		// (get) Token: 0x06009EA6 RID: 40614 RVA: 0x004A37E5 File Offset: 0x004A19E5
		public bool IsThisActive
		{
			get
			{
				return this._isThisActive;
			}
		}

		// Token: 0x06009EA7 RID: 40615 RVA: 0x004A37ED File Offset: 0x004A19ED
		public void Awake()
		{
			this.button.onClick.ResetListener(new Action(this.OnClick));
		}

		// Token: 0x06009EA8 RID: 40616 RVA: 0x004A380D File Offset: 0x004A1A0D
		public void OnPointerEnter(PointerEventData eventData)
		{
			IMainMenuButtonParent parent = this._parent;
			if (parent != null)
			{
				parent.OnChildEnter(this);
			}
		}

		// Token: 0x06009EA9 RID: 40617 RVA: 0x004A3822 File Offset: 0x004A1A22
		public void OnPointerExit(PointerEventData eventData)
		{
			IMainMenuButtonParent parent = this._parent;
			if (parent != null)
			{
				parent.OnChildExit(this);
			}
		}

		// Token: 0x06009EAA RID: 40618 RVA: 0x004A3838 File Offset: 0x004A1A38
		public void Init(IMainMenuButtonParent parent, byte buttonTemplateId)
		{
			this._isThisActive = false;
			this._parent = parent;
			this.templateId = buttonTemplateId;
			MainMenuButtonItem cfg = MainMenuButton.Instance[this.templateId];
			bool isThisActive;
			if (buttonTemplateId != 0)
			{
				sbyte item = cfg.WorldFunction;
				if ((item < 0 || SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock((byte)item)) && (!SingletonObject.getInstance<TutorialChapterModel>().InGuiding || cfg.AllowInGuiding))
				{
					BuildingBlockData buildingBlockData;
					isThisActive = (buttonTemplateId != 2 || SingletonObject.getInstance<BuildingModel>().GetBuilding(45, out buildingBlockData));
					goto IL_70;
				}
			}
			isThisActive = false;
			IL_70:
			this._isThisActive = isThisActive;
			this.buttonText.text = cfg.Name;
			SpriteState state = this.button.spriteState;
			this.image.SetSprite(cfg.IconPrefix + "_3", false, null);
			state.disabledSprite = this.image.sprite;
			this.image.SetSprite(cfg.IconPrefix + "_4", false, null);
			state.pressedSprite = (state.selectedSprite = this.image.sprite);
			this.image.SetSprite(cfg.IconPrefix + "_1", false, null);
			state.highlightedSprite = this.image.sprite;
			this.button.spriteState = state;
			this.image.SetSprite(cfg.IconPrefix + "_0", false, null);
		}

		// Token: 0x06009EAB RID: 40619 RVA: 0x004A39A4 File Offset: 0x004A1BA4
		public void SetText(CImage btnImage, TMP_Text btnName, TMP_Text btnSummary, TMP_Text btnDescription)
		{
			MainMenuButtonItem cfg = MainMenuButton.Instance[this.templateId];
			btnImage.sprite = this.image.sprite;
			btnImage.enabled = true;
			btnName.text = cfg.Name;
			btnSummary.text = cfg.Summary;
			btnDescription.text = cfg.Desc;
		}

		// Token: 0x06009EAC RID: 40620 RVA: 0x004A3A08 File Offset: 0x004A1C08
		public void OnClick()
		{
			bool flag;
			if (UIElement.WorldMap.Exist)
			{
				ViewWorldMap viewWorldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
				flag = (viewWorldMap != null && viewWorldMap.IsMoving);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				switch (this.templateId)
				{
				case 0:
				{
					bool flag3 = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure || SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventTaiwu.InAdventure;
					if (flag3)
					{
						return;
					}
					BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
					bool flag4 = basicGameData.IsDreamBack && !basicGameData.IsDreamBackStateUnlocked(4);
					if (flag4)
					{
						bool flag5 = !this._isDreamBackUnlockBuildingButtonClicked;
						if (flag5)
						{
							this._isDreamBackUnlockBuildingButtonClicked = true;
							TaiwuEventDomainMethod.Call.TaiwuCrossArchiveFindMemory(4);
						}
						return;
					}
					GEvent.OnEvent(UiEvents.HideMapBlockCharList, null);
					Location blockKey = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
					ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
					argsBox.Set("AreaId", blockKey.AreaId);
					argsBox.Set("BlockId", blockKey.BlockId);
					UIElement.BuildingArea.SetOnInitArgs(argsBox);
					CommandManager.AddCommand<CommandStackUI, UIElement>(EPriority.StackUINormal, UIElement.StateBuilding);
					break;
				}
				case 1:
					UIElement.TaiwuVillagers.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
					UIManager.Instance.MaskUI(UIElement.TaiwuVillagers);
					break;
				case 2:
				{
					ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
					argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
					argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleAssign);
					UIElement.VillagerRole.SetOnInitArgs(argbox);
					UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
					break;
				}
				case 3:
					GEvent.OnEvent(UiEvents.OnRefreshWorkPanel, EasyPool.Get<ArgumentBox>().Set("show", false));
					UIManager.Instance.ShowUI(UIElement.VillagerWork, true);
					break;
				case 4:
				{
					bool isSecretInformationButtonClicked = this._isSecretInformationButtonClicked;
					if (isSecretInformationButtonClicked)
					{
						return;
					}
					InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackageFromBroadcast(this._parent, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool pool)
					{
						this._isSecretInformationButtonClicked = false;
						bool exist = UIElement.SelectSecretInformation.Exist;
						if (!exist)
						{
							this._parent.OnChildClicked(this);
							SecretInformationDisplayPackage secretInformationDisplayData = null;
							Serializer.Deserialize(pool, offset, ref secretInformationDisplayData);
							if (secretInformationDisplayData == null)
							{
								secretInformationDisplayData = new SecretInformationDisplayPackage();
							}
							MainMenuButton.<OnClick>g__ShowBroadcast|15_0(secretInformationDisplayData);
						}
					});
					return;
				}
				case 5:
					UIElement.SettlementInformation.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SettlementId", SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId()));
					UIManager.Instance.ShowUI(UIElement.SettlementInformation, true);
					break;
				case 6:
					UIElement.GameLineScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("index", -1).Set("targetScrollIndex", 2));
					UIManager.Instance.MaskUI(UIElement.GameLineScroll);
					break;
				case 7:
					UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", false));
					UIManager.Instance.MaskUI(UIElement.Legacy);
					break;
				case 8:
				{
					ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("CanDelete", true);
					UIElement.CheckInscription.SetOnInitArgs(box);
					UIManager.Instance.ShowUI(UIElement.CheckInscription, true);
					break;
				}
				case 9:
					UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
					break;
				case 10:
					UIManager.Instance.ShowUI(UIElement.LegendaryBook, true);
					break;
				case 11:
					UIManager.Instance.MaskUI(UIElement.InstantNotification);
					break;
				case 12:
					UIManager.Instance.MaskUI(UIElement.Following);
					break;
				case 13:
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					UIElement.TaiwuLifeSummary.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.TaiwuLifeSummary, true);
					break;
				}
				default:
					Debug.LogError(string.Format("MainMenuButton {0} invalid", this.templateId));
					return;
				}
				this._parent.OnChildClicked(this);
			}
		}

		// Token: 0x06009EAE RID: 40622 RVA: 0x004A3DDC File Offset: 0x004A1FDC
		[CompilerGenerated]
		internal static void <OnClick>g__ShowBroadcast|15_0(SecretInformationDisplayPackage secretInformationDisplayData)
		{
			ArgumentBox argsBox = new ArgumentBox();
			argsBox.Set("SelectType", 4);
			argsBox.Set("title", LocalStringManager.Get(LanguageKey.LK_BroadcastSecretInformation));
			argsBox.SetObject("secretInformation", secretInformationDisplayData);
			UIElement.SelectSecretInformation.SetOnInitArgs(argsBox);
			UIManager.Instance.MaskUI(UIElement.SelectSecretInformation);
		}

		// Token: 0x04007ABF RID: 31423
		private IMainMenuButtonParent _parent;

		// Token: 0x04007AC0 RID: 31424
		[SerializeField]
		private byte templateId;

		// Token: 0x04007AC1 RID: 31425
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x04007AC2 RID: 31426
		[SerializeField]
		private CButton button;

		// Token: 0x04007AC3 RID: 31427
		[SerializeField]
		private CImage image;

		// Token: 0x04007AC4 RID: 31428
		private bool _isThisActive = false;

		// Token: 0x04007AC5 RID: 31429
		private bool _isDreamBackUnlockBuildingButtonClicked;

		// Token: 0x04007AC6 RID: 31430
		private bool _isSecretInformationButtonClicked;
	}
}
