using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C0E RID: 3086
	public class ItemResourceButton : MonoBehaviour
	{
		// Token: 0x06009C94 RID: 40084 RVA: 0x0049525C File Offset: 0x0049345C
		public void SetButtonFunc(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetButtonState(btnState);
			this.SetBtnFunc(add, change, receive);
			this.itemBack.Set(itemDisplayData, false);
			this.itemBack.gameObject.SetActive(true);
			this.specialView.gameObject.SetActive(false);
			this.SetItemName(itemDisplayData);
			bool flag = this.tip.enabled = (((itemDisplayData != null) ? itemDisplayData.Key.TemplateId : -1) >= 0);
			if (flag)
			{
				RowItemLine.SetMouseTipDisplayer(true, itemDisplayData, this.tip);
			}
		}

		// Token: 0x06009C95 RID: 40085 RVA: 0x004952F0 File Offset: 0x004934F0
		public void SetAsEmpty()
		{
			this.SetButtonState(ItemResourceButton.ItemResourceButtonState.None);
			this.itemBack.Set(null, false);
			this.itemBack.SetBack(-1);
			this.tip.Type = TipType.SingleDesc;
			this.tip.enabled = false;
			ArgumentBox runtimeParam = this.tip.RuntimeParam;
			if (runtimeParam != null)
			{
				runtimeParam.Clear();
			}
			this.itemBack.gameObject.SetActive(true);
			this.avatar.gameObject.SetActive(false);
			this.specialView.gameObject.SetActive(false);
			this.costResourceHolder.gameObject.SetActive(false);
			this.remainTimeHolder.gameObject.SetActive(false);
			this.SetItemName(null);
		}

		// Token: 0x06009C96 RID: 40086 RVA: 0x004953B4 File Offset: 0x004935B4
		public void SetButtonMouseTip(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None)
		{
			this.SetButtonState(btnState);
			this.itemBack.Set(itemDisplayData, false);
			this.itemBack.gameObject.SetActive(true);
			this.specialView.gameObject.SetActive(false);
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x06009C97 RID: 40087 RVA: 0x00495404 File Offset: 0x00493604
		private void SetItemName(ItemDisplayData itemDisplayData)
		{
			bool flag = itemDisplayData == null || !itemDisplayData.Key.IsValid();
			if (flag)
			{
				this.itemName.gameObject.SetActive(false);
			}
			else
			{
				this.itemName.gameObject.SetActive(true);
				this.itemName.text = itemDisplayData.GetName(true);
			}
		}

		// Token: 0x06009C98 RID: 40088 RVA: 0x00495468 File Offset: 0x00493668
		private void SetButtonState(ItemResourceButton.ItemResourceButtonState btnState)
		{
			this._currentState = btnState;
			this.receiveOperateBtn.GetComponent<TooltipInvoker>().enabled = false;
			bool flag = this.tip;
			if (flag)
			{
				this.tip.enabled = false;
			}
			this.receiveOperateBtn.interactable = true;
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.None:
			{
				this.addOperateBtn.gameObject.SetActive(false);
				this.receiveOperateBtn.gameObject.SetActive(false);
				bool flag2 = this.rejectOperateBtn;
				if (flag2)
				{
					this.rejectOperateBtn.gameObject.SetActive(false);
				}
				this.changeOperateBtn.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Reveive:
			{
				this.addOperateBtn.gameObject.SetActive(false);
				this.receiveOperateBtn.gameObject.SetActive(true);
				bool flag3 = this.rejectOperateBtn;
				if (flag3)
				{
					this.rejectOperateBtn.gameObject.SetActive(false);
				}
				this.changeOperateBtn.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				this.addOperateBtn.gameObject.SetActive(false);
				this.receiveOperateBtn.gameObject.SetActive(false);
				bool flag4 = this.rejectOperateBtn;
				if (flag4)
				{
					this.rejectOperateBtn.gameObject.SetActive(false);
				}
				this.changeOperateBtn.gameObject.SetActive(true);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
			{
				this.addOperateBtn.gameObject.SetActive(true);
				this.receiveOperateBtn.gameObject.SetActive(false);
				bool flag5 = this.rejectOperateBtn;
				if (flag5)
				{
					this.rejectOperateBtn.gameObject.SetActive(false);
				}
				this.changeOperateBtn.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.LackOfMoney:
			{
				this.addOperateBtn.gameObject.SetActive(false);
				this.receiveOperateBtn.gameObject.SetActive(true);
				bool flag6 = this.rejectOperateBtn;
				if (flag6)
				{
					this.rejectOperateBtn.gameObject.SetActive(false);
				}
				this.receiveOperateBtn.interactable = false;
				this.receiveOperateBtn.GetComponent<TooltipInvoker>().enabled = true;
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.ReveiveRecruit:
			{
				this.addOperateBtn.gameObject.SetActive(false);
				this.receiveOperateBtn.gameObject.SetActive(true);
				bool flag7 = this.rejectOperateBtn;
				if (flag7)
				{
					this.rejectOperateBtn.gameObject.SetActive(true);
				}
				this.changeOperateBtn.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.LackOfMoneyRecruit:
			{
				this.addOperateBtn.gameObject.SetActive(false);
				this.receiveOperateBtn.gameObject.SetActive(true);
				bool flag8 = this.rejectOperateBtn;
				if (flag8)
				{
					this.rejectOperateBtn.gameObject.SetActive(true);
				}
				this.receiveOperateBtn.interactable = false;
				this.receiveOperateBtn.GetComponent<TooltipInvoker>().enabled = true;
				break;
			}
			}
			this.RefreshButtonHolder();
		}

		// Token: 0x06009C99 RID: 40089 RVA: 0x00495790 File Offset: 0x00493990
		public void SetResourceFunc(sbyte resourceType, int resourceAmount, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetButtonState(btnState);
			this.itemName.gameObject.SetActive(false);
			this.SetBtnFunc(add, change, receive);
			ResourceTypeItem config = ResourceType.Instance[resourceType];
			this.specialView.SetSprite(config.Icon, false, null);
			this.resourceCount.text = resourceAmount.ToString();
			this.itemBack.gameObject.SetActive(false);
			this.specialView.gameObject.SetActive(true);
			this.tip.enabled = true;
			RowItemLine.SetResourceTip(ItemDisplayData.CreateResource(resourceType, resourceAmount, -1), this.tip, SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName, false, false);
		}

		// Token: 0x06009C9A RID: 40090 RVA: 0x0049584C File Offset: 0x00493A4C
		public void SetRecruitFunc(RecruitCharacterData data, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null, Action reject = null)
		{
			bool flag = data == null;
			if (!flag)
			{
				this.SetButtonState(btnState);
				this.addOperateBtn.ClearAndAddListener(delegate
				{
					Action add2 = add;
					if (add2 != null)
					{
						add2();
					}
				});
				this.receiveOperateBtn.ClearAndAddListener(delegate
				{
					Action receive2 = receive;
					if (receive2 != null)
					{
						receive2();
					}
				});
				this.rejectOperateBtn.ClearAndAddListener(delegate
				{
					Action reject2 = reject;
					if (reject2 != null)
					{
						reject2();
					}
				});
				this.changeOperateBtn.ClearAndAddListener(delegate
				{
					Action change2 = change;
					if (change2 != null)
					{
						change2();
					}
				});
				this.avatar.Refresh(data.GenerateAvatarRelatedData(), data.TemplateId);
				this.avatar.gameObject.SetActive(true);
				this.itemBack.gameObject.SetActive(false);
				NameRelatedData charName = ((ITradeableContent)data).NameRelatedData;
				this.itemName.text = NameCenter.GetMonasticTitleOrDisplayName(ref charName, false, false);
				this.itemName.gameObject.SetActive(true);
				this.tip.Type = TipType.CharacterComplete;
				TooltipInvoker tooltipInvoker = this.tip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.SetObject("Data", new CharacterDisplayDataForTooltip(data));
				this.tip.triggerByChildRaycast = true;
				this.tip.enabled = true;
			}
		}

		// Token: 0x06009C9B RID: 40091 RVA: 0x004959B8 File Offset: 0x00493BB8
		public void SetSoldItemFunc(ItemDisplayData itemDisplayData, IntPair receiveData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetButtonState(btnState);
			this.SetBtnFunc(add, change, receive);
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.Reveive:
			{
				bool flag = receiveData.First != -1;
				if (flag)
				{
					bool flag2 = receiveData.First == 6;
					if (flag2)
					{
						this.specialView.SetSprite("sp_icon_resource_money", false, null);
					}
					else
					{
						bool flag3 = receiveData.First == 7;
						if (flag3)
						{
							this.specialView.SetSprite("sp_icon_resource_authority", false, null);
						}
					}
					this.resourceCount.text = receiveData.Second.ToString();
					this.itemBack.gameObject.SetActive(false);
					this.specialView.gameObject.SetActive(true);
				}
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				bool flag4 = itemDisplayData != null;
				if (flag4)
				{
					this.itemBack.Set(itemDisplayData, false);
					this.tip.enabled = true;
				}
				this.itemBack.gameObject.SetActive(true);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
				this.itemBack.gameObject.SetActive(false);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x06009C9C RID: 40092 RVA: 0x00495B10 File Offset: 0x00493D10
		public void SetFixBookFunc(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetButtonState(btnState);
			this.SetBtnFunc(add, change, receive);
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.Reveive:
			{
				bool flag = itemDisplayData != null;
				if (flag)
				{
					this.itemBack.Set(itemDisplayData, false);
					this.tip.enabled = true;
				}
				this.itemBack.gameObject.SetActive(true);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				bool flag2 = itemDisplayData != null;
				if (flag2)
				{
					this.itemBack.Set(itemDisplayData, false);
					this.tip.enabled = true;
				}
				this.itemBack.gameObject.SetActive(true);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
				this.itemBack.gameObject.SetActive(false);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x06009C9D RID: 40093 RVA: 0x00495C14 File Offset: 0x00493E14
		public void SetTeaHorseBtnFunc(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetButtonState(btnState);
			this.SetBtnFunc(add, change, receive);
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.None:
			{
				bool flag = itemDisplayData != null;
				if (flag)
				{
					this.itemBack.Set(itemDisplayData, false);
					this.tip.enabled = true;
				}
				this.itemBack.gameObject.SetActive(itemDisplayData != null);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Reveive:
			{
				bool flag2 = itemDisplayData != null;
				if (flag2)
				{
					this.itemBack.Set(itemDisplayData, false);
					this.tip.enabled = true;
				}
				this.itemBack.gameObject.SetActive(true);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				bool flag3 = itemDisplayData != null;
				if (flag3)
				{
					this.itemBack.Set(itemDisplayData, false);
					this.tip.enabled = true;
				}
				this.itemBack.gameObject.SetActive(true);
				this.specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
				this.itemBack.gameObject.SetActive(false);
				this.specialView.gameObject.SetActive(false);
				break;
			}
		}

		// Token: 0x06009C9E RID: 40094 RVA: 0x00495D68 File Offset: 0x00493F68
		private void SetBtnFunc(Action add, Action change, Action receive)
		{
			this.addOperateBtn.onClick.RemoveAllListeners();
			bool flag = add != null;
			if (flag)
			{
				this.addOperateBtn.onClick.AddListener(new UnityAction(add.Invoke));
			}
			this.changeOperateBtn.onClick.RemoveAllListeners();
			bool flag2 = change != null;
			if (flag2)
			{
				this.changeOperateBtn.onClick.AddListener(new UnityAction(change.Invoke));
			}
			this.receiveOperateBtn.onClick.RemoveAllListeners();
			bool flag3 = receive != null;
			if (flag3)
			{
				this.receiveOperateBtn.onClick.AddListener(new UnityAction(receive.Invoke));
			}
		}

		// Token: 0x06009C9F RID: 40095 RVA: 0x00495E18 File Offset: 0x00494018
		public void AppendResourceCount(sbyte resourceType = -1, int count = 0)
		{
			bool flag = !this.resourceIcons.CheckIndex((int)resourceType);
			if (!flag)
			{
				this.costResourceHolder.SetActive(true);
				this.costResourceIcon.sprite = this.resourceIcons[(int)resourceType];
				this.costResource.text = count.ToString();
			}
		}

		// Token: 0x06009CA0 RID: 40096 RVA: 0x00495E70 File Offset: 0x00494070
		public void AppendRemainTime(int value = -1)
		{
			bool flag = value == -1;
			if (!flag)
			{
				this.remainTimeHolder.SetActive(true);
				this.remainTime.text = value.ToString();
				ArgumentBox runtimeParam = this.tip.RuntimeParam;
				if (runtimeParam != null)
				{
					runtimeParam.Set("RemainTime", value);
				}
			}
		}

		// Token: 0x06009CA1 RID: 40097 RVA: 0x00495EC8 File Offset: 0x004940C8
		public void OnMouseEnterEvent()
		{
			bool visible = this._currentState != ItemResourceButton.ItemResourceButtonState.None;
			bool flag = this.hover;
			if (flag)
			{
				this.hover.gameObject.SetActive(visible);
			}
			bool flag2 = this.btnHolder;
			if (flag2)
			{
				this.btnHolder.SetActive(visible);
			}
		}

		// Token: 0x06009CA2 RID: 40098 RVA: 0x00495F20 File Offset: 0x00494120
		public void OnMouseExitEvent()
		{
			bool flag = this.hover;
			if (flag)
			{
				this.hover.gameObject.SetActive(false);
			}
			this.RefreshButtonHolder();
		}

		// Token: 0x06009CA3 RID: 40099 RVA: 0x00495F58 File Offset: 0x00494158
		public void ChangeHoverActive(bool active)
		{
			bool flag = this.hover;
			if (flag)
			{
				this.hover.gameObject.SetActive(active);
			}
		}

		// Token: 0x06009CA4 RID: 40100 RVA: 0x00495F88 File Offset: 0x00494188
		private void RefreshButtonHolder()
		{
			bool visible = this._currentState == ItemResourceButton.ItemResourceButtonState.Add;
			bool flag = this.btnHolder;
			if (flag)
			{
				this.btnHolder.SetActive(visible);
			}
		}

		// Token: 0x04007971 RID: 31089
		[SerializeField]
		private CButton addOperateBtn;

		// Token: 0x04007972 RID: 31090
		[SerializeField]
		private CButton changeOperateBtn;

		// Token: 0x04007973 RID: 31091
		[SerializeField]
		private CButton receiveOperateBtn;

		// Token: 0x04007974 RID: 31092
		[SerializeField]
		private CButton rejectOperateBtn;

		// Token: 0x04007975 RID: 31093
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x04007976 RID: 31094
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007977 RID: 31095
		[SerializeField]
		private GameObject remainTimeHolder;

		// Token: 0x04007978 RID: 31096
		[SerializeField]
		private TMP_Text remainTime;

		// Token: 0x04007979 RID: 31097
		[SerializeField]
		private CImage specialView;

		// Token: 0x0400797A RID: 31098
		[SerializeField]
		private TMP_Text resourceCount;

		// Token: 0x0400797B RID: 31099
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x0400797C RID: 31100
		[SerializeField]
		private CImage hover;

		// Token: 0x0400797D RID: 31101
		[SerializeField]
		private GameObject btnHolder;

		// Token: 0x0400797E RID: 31102
		[SerializeField]
		private GameObject costResourceHolder;

		// Token: 0x0400797F RID: 31103
		[SerializeField]
		private CImage costResourceIcon;

		// Token: 0x04007980 RID: 31104
		[SerializeField]
		private TMP_Text costResource;

		// Token: 0x04007981 RID: 31105
		[SerializeField]
		private Sprite[] resourceIcons;

		// Token: 0x04007982 RID: 31106
		[SerializeField]
		private TMP_Text itemName;

		// Token: 0x04007983 RID: 31107
		private ItemResourceButton.ItemResourceButtonState _currentState;

		// Token: 0x02002321 RID: 8993
		public enum ItemResourceButtonState
		{
			// Token: 0x0400DDD3 RID: 56787
			None = -1,
			// Token: 0x0400DDD4 RID: 56788
			Reveive,
			// Token: 0x0400DDD5 RID: 56789
			Change,
			// Token: 0x0400DDD6 RID: 56790
			Add,
			// Token: 0x0400DDD7 RID: 56791
			LackOfMoney,
			// Token: 0x0400DDD8 RID: 56792
			ReveiveRecruit,
			// Token: 0x0400DDD9 RID: 56793
			LackOfMoneyRecruit
		}
	}
}
