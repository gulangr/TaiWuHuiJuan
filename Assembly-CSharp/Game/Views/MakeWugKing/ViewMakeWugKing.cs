using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.MakeWugKing
{
	// Token: 0x02000949 RID: 2377
	public class ViewMakeWugKing : UIBase
	{
		// Token: 0x17000CD5 RID: 3285
		// (get) Token: 0x06007050 RID: 28752 RVA: 0x00340970 File Offset: 0x0033EB70
		public int DriveTargetCharId
		{
			get
			{
				return this._driveTargetCharId;
			}
		}

		// Token: 0x17000CD6 RID: 3286
		// (get) Token: 0x06007051 RID: 28753 RVA: 0x00340978 File Offset: 0x0033EB78
		public SectWuxianWugJugData WugJugData
		{
			get
			{
				return this._wugJugData;
			}
		}

		// Token: 0x17000CD7 RID: 3287
		// (get) Token: 0x06007052 RID: 28754 RVA: 0x00340980 File Offset: 0x0033EB80
		public List<ItemDisplayData> InventoryItems
		{
			get
			{
				return this._inventoryItems;
			}
		}

		// Token: 0x17000CD8 RID: 3288
		// (get) Token: 0x06007053 RID: 28755 RVA: 0x00340988 File Offset: 0x0033EB88
		public List<WugKingDriveDisplayData> WugKingDriveDisplayDatas
		{
			get
			{
				return this._wugKingDriveDisplayDatas;
			}
		}

		// Token: 0x17000CD9 RID: 3289
		// (get) Token: 0x06007054 RID: 28756 RVA: 0x00340990 File Offset: 0x0033EB90
		public ResourceInts TaiwuResources
		{
			get
			{
				return this._taiwuResources;
			}
		}

		// Token: 0x17000CDA RID: 3290
		// (get) Token: 0x06007055 RID: 28757 RVA: 0x00340998 File Offset: 0x0033EB98
		public int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x06007056 RID: 28758 RVA: 0x003409A4 File Offset: 0x0033EBA4
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06007057 RID: 28759 RVA: 0x003409B0 File Offset: 0x0033EBB0
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this._subPageToggleGroup.Init(-1);
				ToggleGroupHotkeyController.Set(this.Element, this._subPageToggleGroup, 0, null);
				this._subPageToggleGroup.OnActiveIndexChange += delegate(int newToggle, int oldToggle)
				{
					bool flag = newToggle >= 0;
					if (flag)
					{
						this.SwitchSubPage(newToggle);
					}
				};
				this.btnClose.ClearAndAddListener(delegate
				{
					this.QuickHide();
				});
			}
		}

		// Token: 0x06007058 RID: 28760 RVA: 0x00340A24 File Offset: 0x0033EC24
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox == null)
			{
				argsBox = new ArgumentBox();
			}
			this.ReadArgs(argsBox);
			this._inventoryItems.Clear();
			this.NeedDataListenerId = true;
			this.makeWugKingPanel.Setup(this);
			this.makeWugKingPanel.OnInit(argsBox);
			this.driveWugKingPanel.Setup(this);
			this.driveWugKingPanel.OnInit();
			this.InitButtonListTip();
			this.RefreshToggleState();
		}

		// Token: 0x06007059 RID: 28761 RVA: 0x00340A98 File Offset: 0x0033EC98
		private void RefreshToggleState()
		{
			OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 12, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked, delegate(int offset, RawDataPool pool)
			{
				bool unlock = false;
				Serializer.Deserialize(pool, offset, ref unlock);
				this._subPageToggleGroup.SetInteractable(unlock, 1);
			});
		}

		// Token: 0x0600705A RID: 28762 RVA: 0x00340AB4 File Offset: 0x0033ECB4
		private void InitButtonListTip()
		{
			TooltipInvoker btnListTip = this.btnList.GetComponent<TooltipInvoker>();
			btnListTip.enabled = true;
			btnListTip.Type = TipType.Simple;
			string title = LocalStringManager.Get(LanguageKey.LK_MakeWugKing_List);
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MakeWugKing_List_Tip_1).SetColor("pinkyellow"));
			sb.AppendLine(string.Empty);
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MakeWugKing_List_Tip_2).SetColor("pinkyellow"));
			sb.AppendLine(string.Empty);
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MakeWugKing_List_Tip_3).SetColor("grey"));
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MakeWugKing_List_Tip_4).SetColor("brightred"));
			bool flag = btnListTip.PresetParam == null || btnListTip.PresetParam.Length < 2;
			if (flag)
			{
				btnListTip.PresetParam = new string[2];
			}
			btnListTip.PresetParam[0] = title;
			btnListTip.PresetParam[1] = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
		}

		// Token: 0x0600705B RID: 28763 RVA: 0x00340BB6 File Offset: 0x0033EDB6
		private void SwitchToTargetSubPage()
		{
			this._subPageToggleGroup.Set(this._initialTabIndex, true);
		}

		// Token: 0x0600705C RID: 28764 RVA: 0x00340BCC File Offset: 0x0033EDCC
		private void SwitchSubPage(int key)
		{
			this.makeWugKingPanel.gameObject.SetActive(key == 0);
			this.driveWugKingPanel.gameObject.SetActive(key == 1);
		}

		// Token: 0x0600705D RID: 28765 RVA: 0x00340BFC File Offset: 0x0033EDFC
		private void RefreshDriveTabToggleInteractable()
		{
			bool flag = this._subPageToggleGroup == null;
			if (!flag)
			{
				int active = this._subPageToggleGroup.GetActiveIndex();
				bool flag2 = active >= 0 && active == 1;
				if (flag2)
				{
					this._subPageToggleGroup.Set(0, true);
				}
			}
		}

		// Token: 0x0600705E RID: 28766 RVA: 0x00340C48 File Offset: 0x0033EE48
		private void ReadArgs(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get("InitialTabIndex", out this._initialTabIndex);
			if (flag)
			{
				this._initialTabIndex = 0;
			}
			bool flag2 = !argsBox.Get("TargetCharacterId", out this._driveTargetCharId);
			if (flag2)
			{
				this._driveTargetCharId = this.TaiwuCharId;
			}
		}

		// Token: 0x0600705F RID: 28767 RVA: 0x00340CA0 File Offset: 0x0033EEA0
		private void OnEnable()
		{
			AudioManager.Instance.PlaySound("SFX_Wugjug_amb_loop", true, false);
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChanged));
			GEvent.Add(EEvents.OnActionPointInPrevMonthChange, new GEvent.Callback(this.OnActionPointChanged));
		}

		// Token: 0x06007060 RID: 28768 RVA: 0x00340CF4 File Offset: 0x0033EEF4
		private void OnDisable()
		{
			AudioManager.Instance.StopSound("SFX_Wugjug_amb_loop");
			AudioManager.Instance.StopSound("SFX_Wugjug_fly_01");
			AudioManager.Instance.StopSound("SFX_Wugjug_fly_02");
			AudioManager.Instance.StopSound("SFX_Wugjug_fly_in");
			AudioManager.Instance.StopSound("SFX_Wugjug_upgrade");
			AudioManager.Instance.StopSound("SFX_Wugjug_amb_loop");
			AudioManager.Instance.StopSound("SFX_Wugjug_tree");
			AudioManager.Instance.StopSound("SFX_Wugjug_lighting");
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChanged));
			GEvent.Remove(EEvents.OnActionPointInPrevMonthChange, new GEvent.Callback(this.OnActionPointChanged));
		}

		// Token: 0x06007061 RID: 28769 RVA: 0x00340DB3 File Offset: 0x0033EFB3
		private void OnActionPointChanged(ArgumentBox _)
		{
			DriveWugKingPanel driveWugKingPanel = this.driveWugKingPanel;
			if (driveWugKingPanel != null)
			{
				driveWugKingPanel.RefreshCostDisplay();
			}
		}

		// Token: 0x06007062 RID: 28770 RVA: 0x00340DC8 File Offset: 0x0033EFC8
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this.TaiwuCharId, new uint[]
			{
				34U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 103, ulong.MaxValue, null));
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this.TaiwuCharId);
				OrganizationDomainMethod.Call.GetSectFunctionStatus(this.Element.GameDataListenerId, 12, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked);
				bool flag = this._driveTargetCharId == -1;
				if (flag)
				{
					this._driveTargetCharId = this.TaiwuCharId;
				}
				this.RequestWugKingDriveStatuses();
			}));
		}

		// Token: 0x06007063 RID: 28771 RVA: 0x00340E3C File Offset: 0x0033F03C
		public void RequestWugKingDriveStatuses()
		{
			bool flag = this._driveTargetCharId != -1;
			if (flag)
			{
				StoryDomainMethod.Call.GetWugKingDriveStatuses(this.Element.GameDataListenerId, this._driveTargetCharId);
			}
		}

		// Token: 0x06007064 RID: 28772 RVA: 0x00340E74 File Offset: 0x0033F074
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						bool flag = notification.DomainId == 4;
						if (flag)
						{
							bool flag2 = notification.MethodId == 27;
							if (flag2)
							{
								this._inventoryItems.Clear();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
								this.makeWugKingPanel.OnGotInventoryItems();
								this.Element.ShowAfterRefresh();
							}
						}
						else
						{
							bool flag3 = notification.DomainId == 3 && notification.MethodId == 32;
							if (flag3)
							{
								bool isUnlocked = false;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref isUnlocked);
								this._wuxianUpgradeUnlocked = isUnlocked;
								this.RefreshDriveTabToggleInteractable();
								this.SwitchToTargetSubPage();
							}
							else
							{
								bool flag4 = notification.DomainId == 20 && notification.MethodId == 37;
								if (flag4)
								{
									this._wugKingDriveDisplayDatas.Clear();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wugKingDriveDisplayDatas);
									bool wuxianUpgradeUnlocked = this._wuxianUpgradeUnlocked;
									if (wuxianUpgradeUnlocked)
									{
										this.driveWugKingPanel.OnGotWugKingDriveDisplayDatas();
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag5 = notification.Uid.DomainId == 19;
					if (flag5)
					{
						bool flag6 = notification.Uid.DataId == 103;
						if (flag6)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wugJugData);
							this.makeWugKingPanel.OnGotWugJugData();
						}
					}
					else
					{
						bool flag7 = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0 && notification.Uid.SubId0 == (ulong)((long)this.TaiwuCharId) && notification.Uid.SubId1 == 34U;
						if (flag7)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuResources);
							bool wuxianUpgradeUnlocked2 = this._wuxianUpgradeUnlocked;
							if (wuxianUpgradeUnlocked2)
							{
								this.driveWugKingPanel.UpdateDriveResourceDisplay();
							}
						}
					}
				}
			}
		}

		// Token: 0x06007065 RID: 28773 RVA: 0x003410D8 File Offset: 0x0033F2D8
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "BtnList")
			{
				this.OnClickBtnList();
			}
		}

		// Token: 0x06007066 RID: 28774 RVA: 0x00341107 File Offset: 0x0033F307
		private void OnClickBtnList()
		{
			UIManager.Instance.MaskUI(UIElement.MakeWugKingList);
		}

		// Token: 0x06007067 RID: 28775 RVA: 0x0034111A File Offset: 0x0033F31A
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
			this.makeWugKingPanel.OnQuickHide();
		}

		// Token: 0x17000CDB RID: 3291
		// (get) Token: 0x06007068 RID: 28776 RVA: 0x00341142 File Offset: 0x0033F342
		public ViewMakeWugKing.RoleAnimName CurRoleAnimName
		{
			get
			{
				return this._curRoleAnimName;
			}
		}

		// Token: 0x06007069 RID: 28777 RVA: 0x0034114C File Offset: 0x0033F34C
		private void SetRoleAnimation(int trackIndex, ViewMakeWugKing.RoleAnimName animName, bool loop, Spine.AnimationState.TrackEntryDelegate onComplete = null)
		{
			this._curRoleAnimName = animName;
			if (!true)
			{
			}
			string text;
			switch (animName)
			{
			case ViewMakeWugKing.RoleAnimName.IdleLoop:
				text = "idle_loop";
				break;
			case ViewMakeWugKing.RoleAnimName.PutMaterials:
				text = "put_materials";
				break;
			case ViewMakeWugKing.RoleAnimName.MaterialsMeet:
				text = "materials_meet";
				break;
			case ViewMakeWugKing.RoleAnimName.IdleAfterMeetLoop:
				text = "idle_after_meet_loop";
				break;
			case ViewMakeWugKing.RoleAnimName.MakeWugLoop:
				text = "make_wug_loop";
				break;
			default:
				text = "";
				break;
			}
			if (!true)
			{
			}
			string animNameStr = text;
			this.roleAnimation.AnimationState.SetAnimation(trackIndex, animNameStr, loop);
			this.roleAnimation.AnimationState.Complete -= onComplete;
			this.roleAnimation.AnimationState.Complete += onComplete;
		}

		// Token: 0x0600706A RID: 28778 RVA: 0x003411F4 File Offset: 0x0033F3F4
		public void PlayRoleAnim(ViewMakeWugKing.RoleAnimName animName, Spine.AnimationState.TrackEntryDelegate onComplete = null)
		{
			this._curRoleAnimName = animName;
			bool loop = animName == ViewMakeWugKing.RoleAnimName.IdleLoop || animName == ViewMakeWugKing.RoleAnimName.IdleAfterMeetLoop || animName == ViewMakeWugKing.RoleAnimName.MakeWugLoop;
			this.SetRoleAnimation(0, animName, loop, onComplete);
		}

		// Token: 0x0600706B RID: 28779 RVA: 0x00341223 File Offset: 0x0033F423
		public void RefreshMakePageInfo()
		{
			CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this.TaiwuCharId);
		}

		// Token: 0x0400534D RID: 21325
		private int _initialTabIndex;

		// Token: 0x0400534E RID: 21326
		private int _driveTargetCharId;

		// Token: 0x0400534F RID: 21327
		private SectWuxianWugJugData _wugJugData;

		// Token: 0x04005350 RID: 21328
		private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

		// Token: 0x04005351 RID: 21329
		private List<WugKingDriveDisplayData> _wugKingDriveDisplayDatas = new List<WugKingDriveDisplayData>();

		// Token: 0x04005352 RID: 21330
		private ResourceInts _taiwuResources;

		// Token: 0x04005353 RID: 21331
		private bool _wuxianUpgradeUnlocked;

		// Token: 0x04005354 RID: 21332
		[SerializeField]
		private MakeWugKingPanel makeWugKingPanel;

		// Token: 0x04005355 RID: 21333
		[SerializeField]
		private DriveWugKingPanel driveWugKingPanel;

		// Token: 0x04005356 RID: 21334
		[SerializeField]
		private CToggleGroup _subPageToggleGroup;

		// Token: 0x04005357 RID: 21335
		[SerializeField]
		private SkeletonGraphic roleAnimation;

		// Token: 0x04005358 RID: 21336
		[SerializeField]
		private CButton btnList;

		// Token: 0x04005359 RID: 21337
		[SerializeField]
		private CButton btnClose;

		// Token: 0x0400535A RID: 21338
		private bool _inited = false;

		// Token: 0x0400535B RID: 21339
		private ViewMakeWugKing.RoleAnimName _curRoleAnimName;

		// Token: 0x02001E39 RID: 7737
		public enum RoleAnimName
		{
			// Token: 0x0400C900 RID: 51456
			IdleLoop,
			// Token: 0x0400C901 RID: 51457
			PutMaterials,
			// Token: 0x0400C902 RID: 51458
			MaterialsMeet,
			// Token: 0x0400C903 RID: 51459
			IdleAfterMeetLoop,
			// Token: 0x0400C904 RID: 51460
			MakeWugLoop
		}

		// Token: 0x02001E3A RID: 7738
		public enum ETab
		{
			// Token: 0x0400C906 RID: 51462
			MakeWugKing,
			// Token: 0x0400C907 RID: 51463
			DriveWugKing
		}
	}
}
