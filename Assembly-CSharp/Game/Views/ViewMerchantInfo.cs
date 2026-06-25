using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Views.Exchange;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	// Token: 0x02000709 RID: 1801
	public class ViewMerchantInfo : UIBase
	{
		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x0600550E RID: 21774 RVA: 0x002775A0 File Offset: 0x002757A0
		private static string LanguageSuffix
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			}
		}

		// Token: 0x0600550F RID: 21775 RVA: 0x002775B4 File Offset: 0x002757B4
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get("MerchantType", out this._initialMerchantType);
			if (flag)
			{
				this._initialMerchantType = -1;
			}
			bool flag2 = !argsBox.Get("IsInvest", out this._isInvest);
			if (flag2)
			{
				this._isInvest = false;
			}
			int toggle;
			bool flag3 = argsBox.Get("Toggle", out toggle);
			if (flag3)
			{
				this._initialToggleIndex = toggle;
			}
			else
			{
				this._initialToggleIndex = -1;
			}
		}

		// Token: 0x06005510 RID: 21776 RVA: 0x00277621 File Offset: 0x00275821
		public static int GetToggle(OpenShopEventArguments arg)
		{
			return (arg.IsFromBuilding || arg.IsSpecialBuilding) ? 0 : (arg.IsCaravan ? 1 : 2);
		}

		// Token: 0x06005511 RID: 21777 RVA: 0x00277642 File Offset: 0x00275842
		private void Awake()
		{
			this.InitDropDown();
			this.InitButtons();
			this.InitToggle();
		}

		// Token: 0x06005512 RID: 21778 RVA: 0x0027765A File Offset: 0x0027585A
		private void OnEnable()
		{
			this.UpdateDropdown();
			this.UpdateTitle();
			this.UpdateButtons();
			this.RequestData();
		}

		// Token: 0x06005513 RID: 21779 RVA: 0x0027767C File Offset: 0x0027587C
		public override void InitMonitorFieldIds()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 158, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)taiwuCharId, new uint[]
			{
				34U
			}));
		}

		// Token: 0x06005514 RID: 21780 RVA: 0x002776D0 File Offset: 0x002758D0
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					bool flag = notification.Uid.DomainId == 19 && notification.Uid.DataId == 158;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._lastProtectTime);
						this.UpdateButtons();
					}
					DataUid uid = notification.Uid;
					bool flag2 = uid.DomainId == 4 && uid.SubId1 == 34U;
					if (flag2)
					{
						this._selfResources.Initialize();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._selfResources);
						this.UpdateButtons();
					}
				}
			}
		}

		// Token: 0x06005515 RID: 21781 RVA: 0x002777E0 File Offset: 0x002759E0
		private void RequestFavorData()
		{
			MerchantDomainMethod.AsyncCall.GetAllFavorability(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._allMerchantFavorability);
				this.UpdateFavor((sbyte)this.togGroupGuild.GetActiveIndex());
			});
		}

		// Token: 0x06005516 RID: 21782 RVA: 0x002777F8 File Offset: 0x002759F8
		private void RequestData()
		{
			this.RequestFavorData();
			for (sbyte i = 0; i < 7; i += 1)
			{
				sbyte index = i;
				MerchantDomainMethod.AsyncCall.GetMerchantInfoAreaDataList(null, index, delegate(int offset, RawDataPool pool)
				{
					List<MerchantInfoAreaData> data = null;
					Serializer.Deserialize(pool, offset, ref data);
					this._areaData[index] = (data ?? new List<MerchantInfoAreaData>());
					bool flag = this.listToggleGroup.GetActiveIndex() == 0 && index == 6;
					if (flag)
					{
						this.UpdateScroll();
					}
				});
			}
			MerchantDomainMethod.AsyncCall.GetMerchantInfoCaravanDataList(null, -1, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._caravanData);
				bool flag = this.listToggleGroup.GetActiveIndex() == 1;
				if (flag)
				{
					this.UpdateScroll();
				}
			});
			MerchantDomainMethod.AsyncCall.GetMerchantInfoMerchantDataList(null, -1, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._merchantData);
				bool flag = this.listToggleGroup.GetActiveIndex() == 2;
				if (flag)
				{
					this.UpdateScroll();
				}
			});
		}

		// Token: 0x06005517 RID: 21783 RVA: 0x00277875 File Offset: 0x00275A75
		private void RequestCurrentCaravanData()
		{
			MerchantDomainMethod.AsyncCall.GetMerchantInfoCaravanDataSingle(null, this._selected.CaravanId, delegate(int offset, RawDataPool pool)
			{
				MerchantInfoCaravanData tempData = null;
				Serializer.Deserialize(pool, offset, ref tempData);
				for (int i = 0; i < this._caravanData.Count; i++)
				{
					bool flag = this._caravanData[i].CaravanId == tempData.CaravanId;
					if (flag)
					{
						this._caravanData[i] = tempData;
						break;
					}
				}
				int index = -1;
				for (int j = 0; j < this._filteredCaravanData.Count; j++)
				{
					bool flag2 = this._filteredCaravanData[j].CaravanId == tempData.CaravanId;
					if (flag2)
					{
						this._filteredCaravanData[j] = tempData;
						index = j;
						break;
					}
				}
				this._selected = tempData;
				this.scroll.SetData<MerchantInfoCaravanData>(this._filteredCaravanData, index);
				this.UpdateBtnProtect();
				this.UpdateBtnInvest();
			});
		}

		// Token: 0x06005518 RID: 21784 RVA: 0x00277898 File Offset: 0x00275A98
		private void InitDropDown()
		{
			List<string> options = new List<string>();
			for (int i = 0; i < 7; i++)
			{
				options.Add(Config.MerchantType.Instance[i].Name);
			}
			this.togGroupGuild.Clear();
			for (int j = 0; j < 7; j++)
			{
				CToggle tog = this.togGroupGuild.transform.GetChild(j).GetComponent<CToggle>();
				this.togGroupGuild.Add(tog);
				Refers refers = tog.GetComponent<Refers>();
				refers.CGet<TextMeshProUGUI>("Label").text = Config.MerchantType.Instance[j].Name;
				refers.CGet<GameObject>("RightLine").SetActive(j != 6);
			}
			this.togGroupGuild.Init(-1);
			this.togGroupGuild.OnActiveIndexChange += this.OnGuildTogValueChanged;
		}

		// Token: 0x06005519 RID: 21785 RVA: 0x00277984 File Offset: 0x00275B84
		private void InitButtons()
		{
			this.btnInvest.ClearAndAddListener(new Action(this.OnClickButtonInvest));
			this.btnProtect.ClearAndAddListener(new Action(this.OnClickButtonProtect));
			this.btnRoute.ClearAndAddListener(new Action(this.OnClickButtonRoute));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x0600551A RID: 21786 RVA: 0x002779F3 File Offset: 0x00275BF3
		private void InitToggle()
		{
			this.listToggleGroup.Init(-1);
			this.listToggleGroup.OnActiveIndexChange += this.OnToggleChange;
		}

		// Token: 0x0600551B RID: 21787 RVA: 0x00277A1C File Offset: 0x00275C1C
		private void UpdateDropdown()
		{
			this.togGroupGuild.SetWithoutNotify((int)((this._initialMerchantType >= 0) ? this._initialMerchantType : 0));
			bool flag = this._initialToggleIndex >= 0;
			if (flag)
			{
				this.listToggleGroup.SetWithoutNotify(this._initialToggleIndex);
			}
			else
			{
				this.listToggleGroup.SetWithoutNotify(this._isInvest ? 1 : 0);
			}
		}

		// Token: 0x0600551C RID: 21788 RVA: 0x00277A84 File Offset: 0x00275C84
		private void UpdateTitle()
		{
			this.title.text = (this._isInvest ? ProfessionSkill.Instance[62].Name : LanguageKey.LK_MerchantInfo_Title.Tr());
		}

		// Token: 0x0600551D RID: 21789 RVA: 0x00277AB8 File Offset: 0x00275CB8
		private void UpdateFavor(sbyte type)
		{
			int value = this._allMerchantFavorability[(int)type];
			this.shopProgressBar.Set(value, 0);
			this.progressLabel.text = string.Format("{0}/{1}", value, 100);
		}

		// Token: 0x0600551E RID: 21790 RVA: 0x00277B00 File Offset: 0x00275D00
		private void UpdateScroll()
		{
			int type = this.listToggleGroup.GetActiveIndex();
			this.scroll.SetRowTemplate(this.rowTemplates[type]);
			this.scroll.Init<ITradeableContent>(this.GetCurrentColumnDefinitions(), type == 1, null, new Action<int, RowItem>(this.OnClickRow));
			this.SetFilterData(type);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x0600551F RID: 21791 RVA: 0x00277B68 File Offset: 0x00275D68
		private void UpdateButtons()
		{
			bool flag = this.listToggleGroup.GetActiveIndex() != 1;
			if (flag)
			{
				this.buttons.SetActive(false);
			}
			else
			{
				bool flag2 = SingletonObject.getInstance<ProfessionModel>().IsProfessionalSkillUnlockedAndEquipped(62);
				if (flag2)
				{
					this.UpdateBtnInvest();
					this.UpdateBtnProtect();
					this.btnInvest.gameObject.SetActive(true);
					this.btnProtect.gameObject.SetActive(true);
				}
				else
				{
					this.btnInvest.gameObject.SetActive(false);
					this.btnProtect.gameObject.SetActive(false);
				}
				this.btnRoute.interactable = (this._selected != null);
				this.buttons.SetActive(true);
			}
		}

		// Token: 0x06005520 RID: 21792 RVA: 0x00277C2C File Offset: 0x00275E2C
		private void OnGuildTogValueChanged(int newTog, int oldTog)
		{
			this.RequestFavorData();
			this.UpdateScroll();
		}

		// Token: 0x06005521 RID: 21793 RVA: 0x00277C40 File Offset: 0x00275E40
		private void OnClickRow(int index, RowItem item)
		{
			bool flag = this.listToggleGroup.GetActiveIndex() != 1;
			if (!flag)
			{
				bool flag2 = this._selected == null || this._selectedIndex != index;
				if (flag2)
				{
					this._selectedIndex = index;
					this._selected = this._filteredCaravanData[index];
				}
				else
				{
					this._selectedIndex = -1;
					this._selected = null;
				}
				this.scroll.SetSelectedRow(this._selectedIndex);
				this.UpdateButtons();
			}
		}

		// Token: 0x06005522 RID: 21794 RVA: 0x00277CC8 File Offset: 0x00275EC8
		private void OnClickButtonInvest()
		{
			int cost;
			int have;
			this.GetInvestCost(out cost, out have);
			ConfirmDialogLayoutMerchantCmd cmd = new ConfirmDialogLayoutMerchantCmd
			{
				Title = LanguageKey.LK_MerchantInfo_Invest.Tr(),
				ContentUpper = LanguageKey.LK_MerchantInfo_Invest_Confirm_Cost.Tr(),
				ContentLower = LanguageKey.LK_MerchantInfo_Invest_Confirm_Content.Tr(),
				ConfirmDialogCost = new ConfirmDialogCost
				{
					Type = EConfirmDialogCostType.Money,
					ValueCost = cost,
					ValueHave = have
				},
				Yes = new Action(this.OnConfirmInvest)
			};
			UIElement.ConfirmDialogLayoutMerchant.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.ConfirmDialogLayoutMerchant);
		}

		// Token: 0x06005523 RID: 21795 RVA: 0x00277D7C File Offset: 0x00275F7C
		private void OnClickButtonProtect()
		{
			int cost;
			int have;
			this.GetProtectCost(out cost, out have);
			ConfirmDialogLayoutMerchantCmd cmd = new ConfirmDialogLayoutMerchantCmd
			{
				Title = LanguageKey.LK_MerchantInfo_Protect.Tr(),
				ContentUpper = LanguageKey.LK_MerchantInfo_Protect_Confirm_Cost.Tr(),
				ContentLower = LanguageKey.LK_MerchantInfo_Protect_Confirm_Content.Tr(),
				ContentUnavailable = LanguageKey.LK_MerchantInfo_Protect_Confirm_Tip.Tr(),
				ConfirmDialogCost = new ConfirmDialogCost
				{
					Type = EConfirmDialogCostType.Authority,
					ValueCost = cost,
					ValueHave = have
				},
				Yes = new Action(this.OnConfirmProtect)
			};
			UIElement.ConfirmDialogLayoutMerchant.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.ConfirmDialogLayoutMerchant);
		}

		// Token: 0x06005524 RID: 21796 RVA: 0x00277E40 File Offset: 0x00276040
		private void OnClickButtonRoute()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CaravanData", this._selected);
			UIElement.MerchantCaravanDetail.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.MerchantCaravanDetail);
		}

		// Token: 0x06005525 RID: 21797 RVA: 0x00277E82 File Offset: 0x00276082
		private void OnToggleChange(int _, int __)
		{
			this.UpdateButtons();
			this.UpdateScroll();
		}

		// Token: 0x06005526 RID: 21798 RVA: 0x00277E93 File Offset: 0x00276093
		private void OnConfirmInvest()
		{
			MerchantDomainMethod.Call.InvestCaravan(this._selected.CaravanId);
			this.RequestCurrentCaravanData();
		}

		// Token: 0x06005527 RID: 21799 RVA: 0x00277EAE File Offset: 0x002760AE
		private void OnConfirmProtect()
		{
			MerchantDomainMethod.Call.ProtectCaravan(this._selected.CaravanId);
			this.RequestCurrentCaravanData();
		}

		// Token: 0x06005528 RID: 21800 RVA: 0x00277ECC File Offset: 0x002760CC
		private void UpdateBtnInvest()
		{
			TooltipInvoker tip = this.btnInvest.GetComponent<TooltipInvoker>();
			bool flag = this._selected == null;
			if (flag)
			{
				tip.enabled = false;
				this.btnInvest.interactable = false;
			}
			else
			{
				tip.enabled = true;
				tip.Type = TipType.CaravanOperation;
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				bool meet = true;
				bool isInvested = this._selected.ExtraData.IsInvested;
				if (isInvested)
				{
					meet = false;
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Invest_Tip_Done).SetColor("brightred"));
				}
				bool flag2 = !this._selected.IsInStartArea;
				if (flag2)
				{
					meet = false;
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Invest_Tip_AreaNotMeet).SetColor("brightred"));
				}
				int cost;
				int resource;
				this.GetInvestCost(out cost, out resource);
				bool flag3 = resource < cost;
				if (flag3)
				{
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Invest_Tip_ResourceNotMeet).SetColor("brightred"));
					meet = false;
				}
				tip.RuntimeParam.Set("Tip", sb.ToString()).Set("IsInvest", true).Set("ResourceNeed", cost).Set("ResourceAmount", resource).Set("ResourceType", 6);
				EasyPool.Free<StringBuilder>(sb);
				this.btnInvest.interactable = meet;
			}
		}

		// Token: 0x06005529 RID: 21801 RVA: 0x0027803C File Offset: 0x0027623C
		private void UpdateBtnProtect()
		{
			TooltipInvoker tip = this.btnProtect.GetComponent<TooltipInvoker>();
			bool flag = this._selected == null;
			if (flag)
			{
				tip.enabled = false;
				this.btnProtect.interactable = false;
			}
			else
			{
				tip.Type = TipType.CaravanOperation;
				tip.enabled = true;
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				bool meet = true;
				bool flag2 = this._selected.ExtraData.RobbedRate <= 0;
				if (flag2)
				{
					meet = false;
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Protect_Tip_NotNeed).SetColor("brightred"));
				}
				int curTime = SingletonObject.getInstance<BasicGameData>().CurrDate;
				bool flag3 = curTime <= this._lastProtectTime;
				if (flag3)
				{
					meet = false;
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Protect_Tip_NotCooldown).SetColor("brightred"));
				}
				int cost;
				int resource;
				this.GetProtectCost(out cost, out resource);
				bool flag4 = resource < cost;
				if (flag4)
				{
					meet = false;
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Protect_Tip_ResourceNotMeet).SetColor("brightred"));
				}
				tip.RuntimeParam.Set("Tip", sb.ToString()).Set("IsInvest", false).Set("ResourceNeed", cost).Set("ResourceAmount", resource).Set("ResourceType", 7);
				EasyPool.Free<StringBuilder>(sb);
				this.btnProtect.interactable = meet;
			}
		}

		// Token: 0x0600552A RID: 21802 RVA: 0x002781BD File Offset: 0x002763BD
		private void GetInvestCost(out int cost, out int resource)
		{
			cost = GlobalConfig.Instance.InvestCaravanNeedMoney[(int)this._selected.MerchantConfig.Level];
			resource = this._selfResources.Get(6);
		}

		// Token: 0x0600552B RID: 21803 RVA: 0x002781EC File Offset: 0x002763EC
		private void GetProtectCost(out int cost, out int resource)
		{
			cost = GlobalConfig.Instance.InvestedCaravanAvoidRobbedNeedAuthorityFactor[(int)this._selected.MerchantConfig.Level] * (int)this._selected.ExtraData.RobbedRate / 2;
			resource = this._selfResources.Get(7);
		}

		// Token: 0x0600552C RID: 21804 RVA: 0x00278238 File Offset: 0x00276438
		private void SetFilterData(int type)
		{
			switch (type)
			{
			case 0:
				this.scroll.SetData<MerchantInfoAreaData>(this._areaData[(sbyte)this.togGroupGuild.GetActiveIndex()], this.GetSelectedIndex());
				this.noContent.SetActive(this._areaData[(sbyte)this.togGroupGuild.GetActiveIndex()].Count == 0);
				break;
			case 1:
				this._filteredCaravanData.Clear();
				foreach (MerchantInfoCaravanData data in this._caravanData)
				{
					bool flag = (int)Merchant.Instance[(int)data.MerchantTemplateId].MerchantType == this.togGroupGuild.GetActiveIndex();
					if (flag)
					{
						this._filteredCaravanData.Add(data);
					}
				}
				this.scroll.SetData<MerchantInfoCaravanData>(this._filteredCaravanData, this.GetSelectedIndex());
				this.noContent.SetActive(this._filteredCaravanData.Count == 0);
				break;
			case 2:
				this._filteredMerchantData.Clear();
				foreach (MerchantInfoMerchantData data2 in this._merchantData)
				{
					bool flag2 = data2.MerchantTemplateId >= 0 && (int)Merchant.Instance[(int)data2.MerchantTemplateId].MerchantType == this.togGroupGuild.GetActiveIndex();
					if (flag2)
					{
						this._filteredMerchantData.Add(data2);
					}
				}
				this.scroll.SetData<MerchantInfoMerchantData>(this._filteredMerchantData, this.GetSelectedIndex());
				this.noContent.SetActive(this._filteredMerchantData.Count == 0);
				break;
			}
		}

		// Token: 0x0600552D RID: 21805 RVA: 0x0027843C File Offset: 0x0027663C
		private int GetSelectedIndex()
		{
			bool flag = this.listToggleGroup.GetActiveIndex() != 1;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = this._selected == null;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					for (int index = 0; index < this._filteredCaravanData.Count; index++)
					{
						bool flag3 = this._filteredCaravanData[index].CaravanId == this._selected.CaravanId;
						if (flag3)
						{
							return index;
						}
					}
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x0600552E RID: 21806 RVA: 0x002784C0 File Offset: 0x002766C0
		private IEnumerable<ColumnDefinition> GetCurrentColumnDefinitions()
		{
			int activeIndex = this.listToggleGroup.GetActiveIndex();
			if (!true)
			{
			}
			IEnumerable<ColumnDefinition> result;
			switch (activeIndex)
			{
			case 0:
				result = this.GetAreaColumnDefinitions();
				break;
			case 1:
				result = this.GetCaravanColumnDefinitions();
				break;
			case 2:
				result = this.GetMerchantColumnDefinitions();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600552F RID: 21807 RVA: 0x00278517 File Offset: 0x00276717
		private IEnumerable<ColumnDefinition> GetAreaColumnDefinitions()
		{
			ColumnDefinition<MerchantInfoAreaData, string> columnDefinition = new ColumnDefinition<MerchantInfoAreaData, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 555f,
				FlexibleWidth = 1f,
				PreferredWidth = 555f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_CurrentTog_Area.Tr());
			columnDefinition.CellDataGenerator = ((MerchantInfoAreaData data) => MapArea.Instance[data.AreaTemplateId].Name);
			columnDefinition.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<MerchantInfoAreaData, string> columnDefinition2 = new ColumnDefinition<MerchantInfoAreaData, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_CaravanCount.Tr());
			columnDefinition2.CellDataGenerator = ((MerchantInfoAreaData data) => data.CaravanCount.ToString());
			columnDefinition2.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition2.SortId = 0;
			yield return columnDefinition2;
			ColumnDefinition<MerchantInfoAreaData, string> columnDefinition3 = new ColumnDefinition<MerchantInfoAreaData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_MerchantCount.Tr());
			columnDefinition3.CellDataGenerator = ((MerchantInfoAreaData data) => data.MerchantCount.ToString());
			columnDefinition3.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition3.SortId = 0;
			yield return columnDefinition3;
			ColumnDefinition<MerchantInfoAreaData, string> columnDefinition4 = new ColumnDefinition<MerchantInfoAreaData, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 288f,
				FlexibleWidth = 100f,
				PreferredWidth = 288f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_LocationState.Tr());
			columnDefinition4.CellDataGenerator = ((MerchantInfoAreaData data) => MapState.Instance[MapArea.Instance[data.AreaTemplateId].StateID].Name);
			columnDefinition4.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition4.SortId = 0;
			yield return columnDefinition4;
			yield break;
		}

		// Token: 0x06005530 RID: 21808 RVA: 0x00278527 File Offset: 0x00276727
		private IEnumerable<ColumnDefinition> GetCaravanColumnDefinitions()
		{
			ColumnDefinition<MerchantInfoCaravanData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<MerchantInfoCaravanData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 555f,
				FlexibleWidth = 1f,
				PreferredWidth = 555f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_CurrentTog_Caravan.Tr());
			columnDefinition.CellDataGenerator = ((MerchantInfoCaravanData data) => new AvatarWithNameCellData(data.MerchantTemplateId));
			columnDefinition.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<MerchantInfoCaravanData, IconAndTextCellData> columnDefinition2 = new ColumnDefinition<MerchantInfoCaravanData, IconAndTextCellData>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_MerchantLevel.Tr());
			columnDefinition2.CellDataGenerator = ((MerchantInfoCaravanData data) => new IconAndTextCellData(string.Format("ui9_text_merchant_level_{0}_0_{1}", ((int)(Merchant.Instance[(int)data.MerchantTemplateId].Level + 1)).ToString(), ViewMerchantInfo.LanguageSuffix), null, true, false, false, false));
			columnDefinition2.RefreshTips = new Action<TooltipInvoker>(this.RefreshLevelTips);
			columnDefinition2.SortId = 0;
			yield return columnDefinition2;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition3 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_Location.Tr());
			columnDefinition3.CellDataGenerator = delegate(MerchantInfoCaravanData data)
			{
				MapAreaItem config = MapArea.Instance[data.CurrentAreaTemplateId];
				return MapState.Instance[config.StateID].Name + LanguageKey.LK_Dot_Symbol.Tr() + config.Name;
			};
			columnDefinition3.RefreshTips = new Action<TooltipInvoker>(this.RefreshLocationTips);
			columnDefinition3.SortId = 0;
			yield return columnDefinition3;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition4 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_TargetLocationArea.Tr());
			columnDefinition4.CellDataGenerator = delegate(MerchantInfoCaravanData data)
			{
				MapAreaItem config = MapArea.Instance[data.TargetAreaTemplateId];
				return MapState.Instance[config.StateID].Name + LanguageKey.LK_Dot_Symbol.Tr() + config.Name;
			};
			columnDefinition4.RefreshTips = new Action<TooltipInvoker>(this.RefreshTargetLocationTips);
			columnDefinition4.SortId = 0;
			yield return columnDefinition4;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition5 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_RemainPath.Tr());
			columnDefinition5.CellDataGenerator = ((MerchantInfoCaravanData data) => data.RemainSettlementCount.ToString());
			columnDefinition5.RefreshTips = new Action<TooltipInvoker>(this.RefreshRemainPathTips);
			columnDefinition5.SortId = 0;
			yield return columnDefinition5;
			ColumnDefinition<MerchantInfoCaravanData, IconAndTextCellData> columnDefinition6 = new ColumnDefinition<MerchantInfoCaravanData, IconAndTextCellData>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_CostTime.Tr());
			columnDefinition6.CellDataGenerator = ((MerchantInfoCaravanData data) => new IconAndTextCellData(null, data.RemainNodeCount.ToString(), true, false, false, false));
			columnDefinition6.RefreshTips = new Action<TooltipInvoker>(this.RefreshCostTimeTips);
			columnDefinition6.SortId = 0;
			yield return columnDefinition6;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition7 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_RobbedRate.Tr());
			columnDefinition7.CellDataGenerator = ((MerchantInfoCaravanData data) => ((GameData.Domains.Merchant.SharedMethods.GetCaravanRobbedRate((int)data.ExtraData.RobbedRate, data.IsInBrokenArea) / 10).ToString() + "%").SetColor(data.IsInBrokenArea ? "brightred" : "brightyellow"));
			columnDefinition7.RefreshTips = new Action<TooltipInvoker>(this.RefreshRobbedRateTips);
			columnDefinition7.SortId = 0;
			yield return columnDefinition7;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition8 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition8.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition8.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_IncomeBonus.Tr());
			columnDefinition8.CellDataGenerator = delegate(MerchantInfoCaravanData data)
			{
				int bonus = (int)(data.ExtraData.IncomeBonus / 10);
				return (bonus.ToString() + "%").SetColor((bonus > 100) ? "brightblue" : ((bonus < 100) ? "brightred" : "brightyellow"));
			};
			columnDefinition8.RefreshTips = new Action<TooltipInvoker>(this.RefreshIncomeBonusTips);
			columnDefinition8.SortId = 0;
			yield return columnDefinition8;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition9 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition9.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition9.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_InvestIncome.Tr());
			columnDefinition9.CellDataGenerator = delegate(MerchantInfoCaravanData data)
			{
				string result;
				if (data.ExtraData.IsInvested)
				{
					string displayStringForNum = CommonUtils.GetDisplayStringForNum(data.GetInvestIncome(), 100000);
					short incomeBonus = data.ExtraData.IncomeBonus;
					if (!true)
					{
					}
					string color;
					if (incomeBonus <= 1000)
					{
						if (incomeBonus >= 1000)
						{
							color = "brightyellow";
						}
						else
						{
							color = "brightred";
						}
					}
					else
					{
						color = "brightblue";
					}
					if (!true)
					{
					}
					result = displayStringForNum.SetColor(color);
				}
				else
				{
					result = 0.ToString();
				}
				return result;
			};
			columnDefinition9.RefreshTips = new Action<TooltipInvoker>(this.RefreshInvestIncomeTips);
			columnDefinition9.SortId = 0;
			yield return columnDefinition9;
			ColumnDefinition<MerchantInfoCaravanData, string> columnDefinition10 = new ColumnDefinition<MerchantInfoCaravanData, string>();
			columnDefinition10.LayoutOption = new LayoutOption
			{
				MinWidth = 288f,
				FlexibleWidth = 100f,
				PreferredWidth = 288f,
				Priority = 1
			};
			columnDefinition10.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_CriticalInfo.Tr());
			columnDefinition10.CellDataGenerator = ((MerchantInfoCaravanData data) => LanguageKey.LK_MerchantInfo_Invest_Critial.TrFormat(((int)(data.ExtraData.IncomeCriticalRate / 10)).ToString(), (float)Math.Round((double)((float)data.ExtraData.IncomeCriticalResult / 100f), 1)));
			columnDefinition10.RefreshTips = new Action<TooltipInvoker>(this.RefreshCriticalInfoTips);
			columnDefinition10.SortId = 0;
			yield return columnDefinition10;
			yield break;
		}

		// Token: 0x06005531 RID: 21809 RVA: 0x00278537 File Offset: 0x00276737
		private IEnumerable<ColumnDefinition> GetMerchantColumnDefinitions()
		{
			ColumnDefinition<MerchantInfoMerchantData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<MerchantInfoMerchantData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 555f,
				FlexibleWidth = 1f,
				PreferredWidth = 555f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_CurrentTog_Merchant.Tr());
			columnDefinition.CellDataGenerator = new Func<MerchantInfoMerchantData, AvatarWithNameCellData>(AvatarWithNameCellData.FromMerchantInfoMerchantData);
			columnDefinition.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<MerchantInfoMerchantData, IconAndTextCellData> columnDefinition2 = new ColumnDefinition<MerchantInfoMerchantData, IconAndTextCellData>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_BehaviorType.Tr());
			columnDefinition2.CellDataGenerator = ((MerchantInfoMerchantData data) => new IconAndTextCellData(string.Format("{0}{1}", "ui9_icon_behavior_type_", data.BehaviorType), CommonUtils.GetBehaviorString(data.BehaviorType), true, false, false, false));
			columnDefinition2.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition2.SortId = 0;
			yield return columnDefinition2;
			ColumnDefinition<MerchantInfoMerchantData, IconAndTextCellData> columnDefinition3 = new ColumnDefinition<MerchantInfoMerchantData, IconAndTextCellData>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_Favorability.Tr());
			columnDefinition3.CellDataGenerator = ((MerchantInfoMerchantData data) => new IconAndTextCellData(CommonUtils.GetFavorabilityIconName(data.Favorability, true), CommonUtils.GetFavorString(data.Favorability), true, false, false, false));
			columnDefinition3.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition3.SortId = 0;
			yield return columnDefinition3;
			ColumnDefinition<MerchantInfoMerchantData, IconAndTextCellData> columnDefinition4 = new ColumnDefinition<MerchantInfoMerchantData, IconAndTextCellData>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_MerchantLevel.Tr());
			columnDefinition4.CellDataGenerator = ((MerchantInfoMerchantData data) => new IconAndTextCellData(string.Format("ui9_text_merchant_level_{0}_0_{1}", ((int)(Merchant.Instance[(int)data.MerchantTemplateId].Level + 1)).ToString(), ViewMerchantInfo.LanguageSuffix), null, true, false, false, false));
			columnDefinition4.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition4.SortId = 0;
			yield return columnDefinition4;
			ColumnDefinition<MerchantInfoMerchantData, string> columnDefinition5 = new ColumnDefinition<MerchantInfoMerchantData, string>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_LocationArea.Tr());
			columnDefinition5.CellDataGenerator = ((MerchantInfoMerchantData data) => MapArea.Instance[data.CurrentAreaTemplateId].Name);
			columnDefinition5.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition5.SortId = 0;
			yield return columnDefinition5;
			ColumnDefinition<MerchantInfoMerchantData, string> columnDefinition6 = new ColumnDefinition<MerchantInfoMerchantData, string>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 160f,
				FlexibleWidth = 100f,
				PreferredWidth = 160f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_LocationState.Tr());
			columnDefinition6.CellDataGenerator = ((MerchantInfoMerchantData data) => MapState.Instance[MapArea.Instance[data.CurrentAreaTemplateId].StateID].Name);
			columnDefinition6.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition6.SortId = 0;
			yield return columnDefinition6;
			ColumnDefinition<MerchantInfoMerchantData, string> columnDefinition7 = new ColumnDefinition<MerchantInfoMerchantData, string>();
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 288f,
				FlexibleWidth = 100f,
				PreferredWidth = 288f,
				Priority = 1
			};
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_MerchantInfo_Organization.Tr());
			columnDefinition7.CellDataGenerator = ((MerchantInfoMerchantData data) => (data.OrgTemplateId < 36) ? Organization.Instance[(int)data.OrgTemplateId].Name : SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(data.FullBlockName, false, false, false, true));
			columnDefinition7.RefreshTips = new Action<TooltipInvoker>(this.TurnOffTips);
			columnDefinition7.SortId = 0;
			yield return columnDefinition7;
			yield break;
		}

		// Token: 0x06005532 RID: 21810 RVA: 0x00278547 File Offset: 0x00276747
		private void RefreshLevelTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_MerchantLevel_Tip.Tr()
			};
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x0027856B File Offset: 0x0027676B
		private void RefreshLocationTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_LocationArea_Tip.Tr()
			};
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x0027858F File Offset: 0x0027678F
		private void RefreshTargetLocationTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_TargetLocation_Tip.Tr()
			};
		}

		// Token: 0x06005535 RID: 21813 RVA: 0x002785B3 File Offset: 0x002767B3
		private void RefreshRemainPathTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_RemainPath_Tip.Tr()
			};
		}

		// Token: 0x06005536 RID: 21814 RVA: 0x002785D7 File Offset: 0x002767D7
		private void RefreshCostTimeTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_CostTime_Tip.Tr()
			};
		}

		// Token: 0x06005537 RID: 21815 RVA: 0x002785FB File Offset: 0x002767FB
		private void RefreshRobbedRateTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_RobbedRate_Tip.Tr()
			};
		}

		// Token: 0x06005538 RID: 21816 RVA: 0x0027861F File Offset: 0x0027681F
		private void RefreshIncomeBonusTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_IncomeBonus_Tip.Tr()
			};
		}

		// Token: 0x06005539 RID: 21817 RVA: 0x00278643 File Offset: 0x00276843
		private void RefreshInvestIncomeTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_InvestIncome_Tip.Tr()
			};
		}

		// Token: 0x0600553A RID: 21818 RVA: 0x00278667 File Offset: 0x00276867
		private void RefreshCriticalInfoTips(TooltipInvoker tips)
		{
			tips.enabled = true;
			tips.PresetParam = new string[]
			{
				LanguageKey.LK_MerchantInfo_CriticalInfo_Tip.Tr()
			};
		}

		// Token: 0x0600553B RID: 21819 RVA: 0x0027868B File Offset: 0x0027688B
		private void TurnOffTips(TooltipInvoker tips)
		{
			tips.enabled = false;
		}

		// Token: 0x040039F3 RID: 14835
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040039F4 RID: 14836
		[SerializeField]
		private CToggleGroup togGroupGuild;

		// Token: 0x040039F5 RID: 14837
		[SerializeField]
		private CToggle guildToggleTemplate;

		// Token: 0x040039F6 RID: 14838
		[SerializeField]
		private RectMask2D progressBar;

		// Token: 0x040039F7 RID: 14839
		[SerializeField]
		private TextMeshProUGUI progressLabel;

		// Token: 0x040039F8 RID: 14840
		[SerializeField]
		private GameObject buttons;

		// Token: 0x040039F9 RID: 14841
		[SerializeField]
		private CButton btnProtect;

		// Token: 0x040039FA RID: 14842
		[SerializeField]
		private CButton btnInvest;

		// Token: 0x040039FB RID: 14843
		[SerializeField]
		private CButton btnRoute;

		// Token: 0x040039FC RID: 14844
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040039FD RID: 14845
		[SerializeField]
		private CToggleGroup listToggleGroup;

		// Token: 0x040039FE RID: 14846
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x040039FF RID: 14847
		[SerializeField]
		private RowItem[] rowTemplates = new RowItem[3];

		// Token: 0x04003A00 RID: 14848
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04003A01 RID: 14849
		[SerializeField]
		private Game.Views.Exchange.ShopProgressBar shopProgressBar;

		// Token: 0x04003A02 RID: 14850
		private const int MaxFavor = 100;

		// Token: 0x04003A03 RID: 14851
		private const int AreaToggle = 0;

		// Token: 0x04003A04 RID: 14852
		private const int CaravanToggle = 1;

		// Token: 0x04003A05 RID: 14853
		private const int MerchantToggle = 2;

		// Token: 0x04003A06 RID: 14854
		private const sbyte InvestResourceType = 6;

		// Token: 0x04003A07 RID: 14855
		private const sbyte ProtectResourceType = 7;

		// Token: 0x04003A08 RID: 14856
		private int _selectedIndex;

		// Token: 0x04003A09 RID: 14857
		private MerchantInfoCaravanData _selected = null;

		// Token: 0x04003A0A RID: 14858
		private int _initialToggleIndex = -1;

		// Token: 0x04003A0B RID: 14859
		private sbyte _initialMerchantType;

		// Token: 0x04003A0C RID: 14860
		private bool _isInvest;

		// Token: 0x04003A0D RID: 14861
		private int[] _allMerchantFavorability;

		// Token: 0x04003A0E RID: 14862
		private Dictionary<sbyte, List<MerchantInfoAreaData>> _areaData = new Dictionary<sbyte, List<MerchantInfoAreaData>>();

		// Token: 0x04003A0F RID: 14863
		private List<MerchantInfoCaravanData> _caravanData = new List<MerchantInfoCaravanData>();

		// Token: 0x04003A10 RID: 14864
		private List<MerchantInfoMerchantData> _merchantData = new List<MerchantInfoMerchantData>();

		// Token: 0x04003A11 RID: 14865
		private List<MerchantInfoCaravanData> _filteredCaravanData = new List<MerchantInfoCaravanData>();

		// Token: 0x04003A12 RID: 14866
		private List<MerchantInfoMerchantData> _filteredMerchantData = new List<MerchantInfoMerchantData>();

		// Token: 0x04003A13 RID: 14867
		private int _lastProtectTime;

		// Token: 0x04003A14 RID: 14868
		private ResourceInts _selfResources;
	}
}
