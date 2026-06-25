using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.Cricket.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000ABF RID: 2751
	public class ViewCricketWishing : UIBase
	{
		// Token: 0x0600877E RID: 34686 RVA: 0x003F04F0 File Offset: 0x003EE6F0
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("LuckPoint", out this._luckPoint);
			argsBox.Get<Action<Location>>("OnWishSuccess", out this._onWishSuccess);
			this._cost = GlobalConfig.Instance.CricketWishingCostLuckPoint;
			this._kingCricketItems.Clear();
			this._kingCricketIds.Clear();
			this._selectedIndex = -1;
			this._isCalling = false;
			foreach (CricketPartsItem item in ((IEnumerable<CricketPartsItem>)CricketParts.Instance))
			{
				bool flag = item.Type == ECricketPartsType.King;
				if (flag)
				{
					this._kingCricketItems.Add(new ItemDisplayData
					{
						Key = new ItemKey(11, 0, -1, -1),
						SpecialArg = (int)item.TemplateId << 16
					});
					this._kingCricketIds.Add(item.TemplateId);
				}
			}
			this.RefreshView();
		}

		// Token: 0x0600877F RID: 34687 RVA: 0x003F05F0 File Offset: 0x003EE7F0
		private void Awake()
		{
			this.scroll.InitPageCount();
			this.scroll.OnItemRender += this.OnRenderCard;
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
		}

		// Token: 0x06008780 RID: 34688 RVA: 0x003F0653 File Offset: 0x003EE853
		private void OnEnable()
		{
			this.scroll.SetDataCount(this._kingCricketItems.Count);
			this.RefreshView();
		}

		// Token: 0x06008781 RID: 34689 RVA: 0x003F0674 File Offset: 0x003EE874
		private void OnRenderCard(int index, GameObject obj)
		{
			bool flag = index < 0 || index >= this._kingCricketItems.Count;
			if (!flag)
			{
				ItemDisplayData data = this._kingCricketItems[index];
				CardItem cardItem = obj.GetComponent<CardItem>();
				RowItemMain rowItemMain = obj.GetComponent<RowItemMain>();
				bool flag2 = cardItem == null || rowItemMain == null;
				if (!flag2)
				{
					rowItemMain.SetData(data);
					rowItemMain.SetName(CricketCombatKit.GetCricketName(data));
					cardItem.Set(rowItemMain, false);
					cardItem.SetSelected(index == this._selectedIndex);
					int capturedIndex = index;
					cardItem.SetClickEvent(delegate
					{
						this.SelectCard(capturedIndex);
					});
				}
			}
		}

		// Token: 0x06008782 RID: 34690 RVA: 0x003F0730 File Offset: 0x003EE930
		private void SelectCard(int index)
		{
			bool flag = index < 0 || index >= this._kingCricketItems.Count;
			if (!flag)
			{
				this._selectedIndex = index;
				this.scroll.ReRender();
				this.RefreshConfirmButton();
			}
		}

		// Token: 0x06008783 RID: 34691 RVA: 0x003F0778 File Offset: 0x003EE978
		private void OnClickConfirm()
		{
			bool flag = this._isCalling || this._selectedIndex < 0 || this._luckPoint < this._cost;
			if (!flag)
			{
				this._isCalling = true;
				TaiwuDomainMethod.AsyncCall.CricketRoomWishingCricket(this, this._kingCricketIds[this._selectedIndex], delegate(int offset, RawDataPool pool)
				{
					Location location = Location.Invalid;
					Serializer.Deserialize(pool, offset, ref location);
					this._isCalling = false;
					this.QuickHide();
					Action<Location> onWishSuccess = this._onWishSuccess;
					if (onWishSuccess != null)
					{
						onWishSuccess(location);
					}
				});
			}
		}

		// Token: 0x06008784 RID: 34692 RVA: 0x003F07DC File Offset: 0x003EE9DC
		private void RefreshLuckPointDisplay()
		{
			string color = (this._luckPoint >= this._cost) ? "brightblue" : "brightred";
			this.luckPointText.text = LanguageKey.LK_CricketCollection_LuckPoint.TrFormat(this._luckPoint.ToString().SetColor(color), this._cost);
		}

		// Token: 0x06008785 RID: 34693 RVA: 0x003F0837 File Offset: 0x003EEA37
		private void RefreshConfirmButton()
		{
			this.btnConfirm.interactable = (!this._isCalling && this._selectedIndex >= 0 && this._luckPoint >= this._cost);
		}

		// Token: 0x06008786 RID: 34694 RVA: 0x003F086C File Offset: 0x003EEA6C
		private void RefreshView()
		{
			this.RefreshLuckPointDisplay();
			this.costTipText.text = LanguageKey.LK_CricketWish_Confirm_Cost_Tip.TrFormat(this._cost);
			this.RefreshConfirmButton();
			bool flag = this.scroll != null && this.scroll.gameObject.activeInHierarchy;
			if (flag)
			{
				this.scroll.ReRender();
			}
		}

		// Token: 0x0400680F RID: 26639
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04006810 RID: 26640
		[SerializeField]
		private TextMeshProUGUI luckPointText;

		// Token: 0x04006811 RID: 26641
		[SerializeField]
		private TextMeshProUGUI costTipText;

		// Token: 0x04006812 RID: 26642
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04006813 RID: 26643
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04006814 RID: 26644
		private readonly List<ItemDisplayData> _kingCricketItems = new List<ItemDisplayData>();

		// Token: 0x04006815 RID: 26645
		private readonly List<short> _kingCricketIds = new List<short>();

		// Token: 0x04006816 RID: 26646
		private int _selectedIndex = -1;

		// Token: 0x04006817 RID: 26647
		private int _luckPoint;

		// Token: 0x04006818 RID: 26648
		private int _cost;

		// Token: 0x04006819 RID: 26649
		private bool _isCalling;

		// Token: 0x0400681A RID: 26650
		private Action<Location> _onWishSuccess;
	}
}
