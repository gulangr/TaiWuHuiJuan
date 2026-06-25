using System;
using Config;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F6 RID: 1526
	public class ItemResourceButton : Refers
	{
		// Token: 0x060047F2 RID: 18418 RVA: 0x0021B708 File Offset: 0x00219908
		public void SetButtonFunc(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetItemRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, false);
			this.SetBtnFunc(add, change, receive);
			this._itemViewRefer.SetData(itemDisplayData, false, -1, false, false, this._btnViewMouseTip, false, true);
			this._btnViewMouseTip.enabled = (itemDisplayData.Key.TemplateId >= 0);
			this._itemViewRefer.gameObject.SetActive(true);
			this._specialView.gameObject.SetActive(false);
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x060047F3 RID: 18419 RVA: 0x0021B79C File Offset: 0x0021999C
		public void SetButtonMouseTip(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None)
		{
			this.SetItemRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, false);
			this._itemViewRefer.SetData(itemDisplayData, false, -1, false, false, null, false, true);
			this._itemViewRefer.gameObject.SetActive(true);
			this._specialView.gameObject.SetActive(false);
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x060047F4 RID: 18420 RVA: 0x0021B804 File Offset: 0x00219A04
		private void SetItemName(ItemDisplayData itemDisplayData)
		{
			TextMeshProUGUI itemName = base.CGet<TextMeshProUGUI>("ItemName");
			bool flag = itemDisplayData == null || !itemDisplayData.Key.IsValid();
			if (flag)
			{
				itemName.gameObject.SetActive(false);
			}
			else
			{
				string displayName = (itemDisplayData.Key.ItemType == 11) ? this._itemViewRefer.CricketView.Name : ItemTemplateHelper.GetName(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
				sbyte grade = ItemTemplateHelper.GetGrade(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
				itemName.gameObject.SetActive(true);
				itemName.text = displayName.SetColor(Colors.Instance.GradeColors[(int)grade]);
			}
		}

		// Token: 0x060047F5 RID: 18421 RVA: 0x0021B8D0 File Offset: 0x00219AD0
		private void SetButtonState(ItemResourceButton.ItemResourceButtonState btnState)
		{
			this._currentState = btnState;
			this._receiveOperateBtn.GetComponent<TooltipInvoker>().enabled = false;
			bool flag = this._btnViewMouseTip != null;
			if (flag)
			{
				this._btnViewMouseTip.enabled = false;
			}
			this._receiveOperateBtn.interactable = true;
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.None:
				this._addOperateBtn.gameObject.SetActive(false);
				this._receiveOperateBtn.gameObject.SetActive(false);
				this._changeOperateBtn.gameObject.SetActive(false);
				break;
			case ItemResourceButton.ItemResourceButtonState.Reveive:
				this._addOperateBtn.gameObject.SetActive(false);
				this._receiveOperateBtn.gameObject.SetActive(true);
				this._changeOperateBtn.gameObject.SetActive(false);
				break;
			case ItemResourceButton.ItemResourceButtonState.Change:
				this._addOperateBtn.gameObject.SetActive(false);
				this._receiveOperateBtn.gameObject.SetActive(false);
				this._changeOperateBtn.gameObject.SetActive(true);
				break;
			case ItemResourceButton.ItemResourceButtonState.Add:
				this._addOperateBtn.gameObject.SetActive(true);
				this._receiveOperateBtn.gameObject.SetActive(false);
				this._changeOperateBtn.gameObject.SetActive(false);
				break;
			case ItemResourceButton.ItemResourceButtonState.LackOfMoney:
				this._addOperateBtn.gameObject.SetActive(false);
				this._receiveOperateBtn.gameObject.SetActive(true);
				this._changeOperateBtn.gameObject.SetActive(false);
				this._receiveOperateBtn.interactable = false;
				this._receiveOperateBtn.GetComponent<TooltipInvoker>().enabled = true;
				break;
			}
			this.RefreshButtonHolder();
		}

		// Token: 0x060047F6 RID: 18422 RVA: 0x0021BA90 File Offset: 0x00219C90
		private void SetItemRefers()
		{
			bool flag = this._addOperateBtn == null;
			if (flag)
			{
				this._addOperateBtn = base.CGet<CButtonObsolete>("AddOperateBtn");
			}
			bool flag2 = this._changeOperateBtn == null;
			if (flag2)
			{
				this._changeOperateBtn = base.CGet<CButtonObsolete>("ChangeOperateBtn");
			}
			bool flag3 = this._receiveOperateBtn == null;
			if (flag3)
			{
				this._receiveOperateBtn = base.CGet<CButtonObsolete>("ReceiveOperateBtn");
			}
			bool flag4 = this._itemViewRefer == null;
			if (flag4)
			{
				this._itemViewRefer = base.CGet<ItemView>("ItemView");
				this._itemViewRefer.SetPointTriggerEnabled(false);
			}
			bool flag5 = this._specialView == null;
			if (flag5)
			{
				this._specialView = base.CGet<CImage>("SpecialView");
			}
			bool flag6 = this._normal == null;
			if (flag6)
			{
				this._normal = base.CGet<CImage>("Normal");
			}
			bool flag7 = this._hover == null;
			if (flag7)
			{
				this._hover = base.CGet<CImage>("Hover");
			}
			bool flag8 = this._resourceCount == null;
			if (flag8)
			{
				this._resourceCount = base.CGet<TextMeshProUGUI>("ResourceCount");
			}
			bool flag9 = this._btnView == null;
			if (flag9)
			{
				this._btnView = base.CGet<CButtonObsolete>("BtnView");
			}
			bool flag10 = this._btnViewMouseTip == null;
			if (flag10)
			{
				this._btnViewMouseTip = base.CGet<TooltipInvoker>("BtnViewMouseTip");
			}
			bool flag11 = this._btnHolder == null;
			if (flag11)
			{
				this._btnHolder = base.CGet<GameObject>("BtnHolder");
			}
		}

		// Token: 0x060047F7 RID: 18423 RVA: 0x0021BC28 File Offset: 0x00219E28
		public void SetResourceFunc(sbyte resourceType, int resourceAmount, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetItemRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, false);
			base.CGet<TextMeshProUGUI>("ItemName").gameObject.SetActive(false);
			this.SetBtnFunc(add, change, receive);
			ResourceTypeItem config = ResourceType.Instance[resourceType];
			this._specialView.SetSprite(config.Icon, false, null);
			this._resourceCount.text = resourceAmount.ToString();
			this._itemViewRefer.gameObject.SetActive(false);
			this._specialView.gameObject.SetActive(true);
		}

		// Token: 0x060047F8 RID: 18424 RVA: 0x0021BCCC File Offset: 0x00219ECC
		private void SetRecruitRefers()
		{
			bool flag = this._addOperateBtn == null;
			if (flag)
			{
				this._addOperateBtn = base.CGet<CButtonObsolete>("AddOperateBtn");
			}
			bool flag2 = this._changeOperateBtn == null;
			if (flag2)
			{
				this._changeOperateBtn = base.CGet<CButtonObsolete>("ChangeOperateBtn");
			}
			bool flag3 = this._receiveOperateBtn == null;
			if (flag3)
			{
				this._receiveOperateBtn = base.CGet<CButtonObsolete>("ReceiveOperateBtn");
			}
			bool flag4 = this._normal == null;
			if (flag4)
			{
				this._normal = base.CGet<CImage>("Normal");
			}
			bool flag5 = this._hover == null;
			if (flag5)
			{
				this._hover = base.CGet<CImage>("Hover");
			}
			bool flag6 = this._hasPeopleImg == null;
			if (flag6)
			{
				this._hasPeopleImg = base.CGet<CImage>("HasPeopleImg");
			}
			bool flag7 = this._noneImg == null;
			if (flag7)
			{
				this._noneImg = base.CGet<CImage>("NoneImg");
			}
			bool flag8 = this._btnHolder == null;
			if (flag8)
			{
				this._btnHolder = base.CGet<GameObject>("BtnHolder");
			}
		}

		// Token: 0x060047F9 RID: 18425 RVA: 0x0021BDEC File Offset: 0x00219FEC
		public void SetRecruitFunc(ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetRecruitRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, true);
			this._addOperateBtn.ClearAndAddListener(delegate
			{
				Action add2 = add;
				if (add2 != null)
				{
					add2();
				}
			});
			this._receiveOperateBtn.ClearAndAddListener(delegate
			{
				Action receive2 = receive;
				if (receive2 != null)
				{
					receive2();
				}
			});
			this._changeOperateBtn.ClearAndAddListener(delegate
			{
				Action change2 = change;
				if (change2 != null)
				{
					change2();
				}
			});
			this._noneImg.gameObject.SetActive(btnState == ItemResourceButton.ItemResourceButtonState.None);
			this._hasPeopleImg.gameObject.SetActive(btnState != ItemResourceButton.ItemResourceButtonState.None);
		}

		// Token: 0x060047FA RID: 18426 RVA: 0x0021BEA4 File Offset: 0x0021A0A4
		public void SetSoldItemFunc(ItemDisplayData itemDisplayData, IntPair receiveData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetItemRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, false);
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
						this._specialView.SetSprite("sp_icon_resource_money", false, null);
					}
					else
					{
						bool flag3 = receiveData.First == 7;
						if (flag3)
						{
							this._specialView.SetSprite("sp_icon_resource_authority", false, null);
						}
					}
					this._resourceCount.text = receiveData.Second.ToString();
					this._itemViewRefer.gameObject.SetActive(false);
					this._specialView.gameObject.SetActive(true);
				}
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				bool flag4 = itemDisplayData != null;
				if (flag4)
				{
					this._itemViewRefer.SetData(itemDisplayData, false, -1, false, true, this._btnViewMouseTip, false, true);
					this._btnViewMouseTip.enabled = true;
				}
				this._itemViewRefer.gameObject.SetActive(true);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
				this._itemViewRefer.gameObject.SetActive(false);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x060047FB RID: 18427 RVA: 0x0021C018 File Offset: 0x0021A218
		public void SetFixBookFunc(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetItemRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, false);
			this.SetBtnFunc(add, change, receive);
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.Reveive:
			{
				bool flag = itemDisplayData != null;
				if (flag)
				{
					this._itemViewRefer.SetData(itemDisplayData, false, -1, false, true, this._btnViewMouseTip, false, true);
					this._btnViewMouseTip.enabled = true;
				}
				this._itemViewRefer.gameObject.SetActive(true);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				bool flag2 = itemDisplayData != null;
				if (flag2)
				{
					this._itemViewRefer.SetData(itemDisplayData, false, -1, false, true, this._btnViewMouseTip, false, true);
					this._btnViewMouseTip.enabled = true;
				}
				this._itemViewRefer.gameObject.SetActive(true);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
				this._itemViewRefer.gameObject.SetActive(false);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			this.SetItemName(itemDisplayData);
		}

		// Token: 0x060047FC RID: 18428 RVA: 0x0021C140 File Offset: 0x0021A340
		public void SetTeaHorseBtnFunc(ItemDisplayData itemDisplayData, ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.None, Action add = null, Action change = null, Action receive = null)
		{
			this.SetItemRefers();
			this.SetButtonState(btnState);
			this.SetBgImg(btnState, false);
			this.SetBtnFunc(add, change, receive);
			switch (btnState)
			{
			case ItemResourceButton.ItemResourceButtonState.None:
			{
				bool flag = itemDisplayData != null;
				if (flag)
				{
					this._itemViewRefer.SetData(itemDisplayData, false, -1, false, true, this._btnViewMouseTip, false, true);
					this._btnViewMouseTip.enabled = true;
				}
				this._itemViewRefer.gameObject.SetActive(itemDisplayData != null);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Reveive:
			{
				bool flag2 = itemDisplayData != null;
				if (flag2)
				{
					this._itemViewRefer.SetData(itemDisplayData, false, -1, false, true, this._btnViewMouseTip, false, true);
					this._btnViewMouseTip.enabled = true;
				}
				this._itemViewRefer.gameObject.SetActive(true);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Change:
			{
				bool flag3 = itemDisplayData != null;
				if (flag3)
				{
					this._itemViewRefer.SetData(itemDisplayData, false, -1, false, true, this._btnViewMouseTip, false, true);
					this._btnViewMouseTip.enabled = true;
				}
				this._itemViewRefer.gameObject.SetActive(true);
				this._specialView.gameObject.SetActive(false);
				break;
			}
			case ItemResourceButton.ItemResourceButtonState.Add:
				this._itemViewRefer.gameObject.SetActive(false);
				this._specialView.gameObject.SetActive(false);
				break;
			}
		}

		// Token: 0x060047FD RID: 18429 RVA: 0x0021C2C8 File Offset: 0x0021A4C8
		private void SetBtnFunc(Action add, Action change, Action receive)
		{
			bool flag = add != null;
			if (flag)
			{
				this._btnView.ClearAndAddListener(delegate
				{
					Action add2 = add;
					if (add2 != null)
					{
						add2();
					}
				});
			}
			bool flag2 = receive != null;
			if (flag2)
			{
				this._btnView.ClearAndAddListener(delegate
				{
					Action receive2 = receive;
					if (receive2 != null)
					{
						receive2();
					}
				});
			}
			bool flag3 = change != null;
			if (flag3)
			{
				this._btnView.ClearAndAddListener(delegate
				{
					Action change2 = change;
					if (change2 != null)
					{
						change2();
					}
				});
			}
			bool flag4 = add == null && receive == null && change == null;
			if (flag4)
			{
				this._btnView.ClearAndAddListener(delegate
				{
				});
			}
		}

		// Token: 0x060047FE RID: 18430 RVA: 0x0021C3B0 File Offset: 0x0021A5B0
		private void SetBgImg(ItemResourceButton.ItemResourceButtonState btnState, bool isRecruitStyle = false)
		{
			string prefix = isRecruitStyle ? "sp_building_touxiang" : "building_daanniu";
			bool flag = btnState == ItemResourceButton.ItemResourceButtonState.None;
			if (flag)
			{
				this._normal.SetSprite(prefix + "_0_0", false, null);
				this._hover.SetSprite(prefix + "_0_1", false, null);
			}
			else
			{
				this._normal.SetSprite(prefix + "_0_0", false, null);
				this._hover.SetSprite(prefix + "_0_1", false, null);
			}
		}

		// Token: 0x060047FF RID: 18431 RVA: 0x0021C440 File Offset: 0x0021A640
		public void OnMouseEnterEvent()
		{
			bool visible = this._currentState != ItemResourceButton.ItemResourceButtonState.None;
			bool flag = this._hover;
			if (flag)
			{
				this._hover.gameObject.SetActive(visible);
			}
			bool flag2 = this._btnHolder;
			if (flag2)
			{
				this._btnHolder.SetActive(visible);
			}
		}

		// Token: 0x06004800 RID: 18432 RVA: 0x0021C498 File Offset: 0x0021A698
		public void OnMouseExitEvent()
		{
			bool flag = this._hover;
			if (flag)
			{
				this._hover.gameObject.SetActive(false);
			}
			this.RefreshButtonHolder();
		}

		// Token: 0x06004801 RID: 18433 RVA: 0x0021C4D0 File Offset: 0x0021A6D0
		private void RefreshButtonHolder()
		{
			bool visible = this._currentState == ItemResourceButton.ItemResourceButtonState.Add;
			bool flag = this._btnHolder;
			if (flag)
			{
				this._btnHolder.SetActive(visible);
			}
		}

		// Token: 0x040031B4 RID: 12724
		private CButtonObsolete _addOperateBtn;

		// Token: 0x040031B5 RID: 12725
		private CButtonObsolete _changeOperateBtn;

		// Token: 0x040031B6 RID: 12726
		private CButtonObsolete _receiveOperateBtn;

		// Token: 0x040031B7 RID: 12727
		private CButtonObsolete _btnView;

		// Token: 0x040031B8 RID: 12728
		private ItemView _itemViewRefer;

		// Token: 0x040031B9 RID: 12729
		private CImage _specialView;

		// Token: 0x040031BA RID: 12730
		private TextMeshProUGUI _resourceCount;

		// Token: 0x040031BB RID: 12731
		private TooltipInvoker _btnViewMouseTip;

		// Token: 0x040031BC RID: 12732
		private CImage _normal;

		// Token: 0x040031BD RID: 12733
		private CImage _hover;

		// Token: 0x040031BE RID: 12734
		private CImage _hasPeopleImg;

		// Token: 0x040031BF RID: 12735
		private CImage _noneImg;

		// Token: 0x040031C0 RID: 12736
		private GameObject _btnHolder;

		// Token: 0x040031C1 RID: 12737
		private ItemResourceButton.ItemResourceButtonState _currentState;

		// Token: 0x020019A5 RID: 6565
		public enum ItemResourceButtonState
		{
			// Token: 0x0400B2F6 RID: 45814
			None = -1,
			// Token: 0x0400B2F7 RID: 45815
			Reveive,
			// Token: 0x0400B2F8 RID: 45816
			Change,
			// Token: 0x0400B2F9 RID: 45817
			Add,
			// Token: 0x0400B2FA RID: 45818
			LackOfMoney
		}
	}
}
