using System;
using Config;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.Cricket;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D6 RID: 2006
	public class GetItem : MonoBehaviour
	{
		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x060061DC RID: 25052 RVA: 0x002CE55C File Offset: 0x002CC75C
		// (set) Token: 0x060061DD RID: 25053 RVA: 0x002CE564 File Offset: 0x002CC764
		public CricketViewNew CricketView { get; private set; }

		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x060061DE RID: 25054 RVA: 0x002CE56D File Offset: 0x002CC76D
		// (set) Token: 0x060061DF RID: 25055 RVA: 0x002CE575 File Offset: 0x002CC775
		public JiaoEggView JiaoEggView { get; private set; }

		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x060061E0 RID: 25056 RVA: 0x002CE57E File Offset: 0x002CC77E
		// (set) Token: 0x060061E1 RID: 25057 RVA: 0x002CE586 File Offset: 0x002CC786
		public ItemDisplayData Data { get; private set; }

		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x060061E2 RID: 25058 RVA: 0x002CE58F File Offset: 0x002CC78F
		private bool IsCricket
		{
			get
			{
				return this.Data != null && this.Data.Key.ItemType == 11;
			}
		}

		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x060061E3 RID: 25059 RVA: 0x002CE5B0 File Offset: 0x002CC7B0
		private bool IsJiaoEgg
		{
			get
			{
				bool result;
				if (this.Data != null)
				{
					JiaoLoongDisplayData jiaoLoongDisplayData = this.Data.JiaoLoongDisplayData;
					result = (jiaoLoongDisplayData != null && jiaoLoongDisplayData.IsEgg);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x060061E4 RID: 25060 RVA: 0x002CE5E0 File Offset: 0x002CC7E0
		public void Set(ItemDisplayData data)
		{
			this.Data = data;
			ItemKey key = data.Key;
			bool isExp = key.ItemType == 12 && key.TemplateId == 8;
			bool isResource = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
			sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
			bool isCricket = data.Key.ItemType == 11;
			sbyte gradeValue = isCricket ? new ValueTuple<short, short>(data.CricketColorId, data.CricketPartId).CalcCricketGrade() : ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId);
			int amountValue = data.Amount;
			this.itemName.text = data.GetName(false);
			this.amount.text = amountValue.ToString();
			this.amount.gameObject.SetActive(amountValue > 1);
			bool flag = this.IsCricket || this.IsJiaoEgg;
			if (flag)
			{
				this.icon.SetSprite(string.Empty, false, null);
			}
			else
			{
				bool flag2 = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
				if (flag2)
				{
					this.icon.SetSprite(string.Format("{0}{1}", "ui9_icon_resource_card_", resourceType), false, null);
				}
				else
				{
					bool flag3 = isExp;
					if (flag3)
					{
						this.icon.SetSprite("ui9_icon_item_exp", false, null);
					}
					else
					{
						bool flag4 = isCricket;
						if (flag4)
						{
							this.icon.SetSprite(CricketParts.Instance[data.CricketColorId].Icon, false, null);
						}
						else
						{
							this.icon.SetSprite(ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId), false, null);
						}
					}
				}
			}
			this.grade.SetColor(Colors.Instance.GradeColors[(int)gradeValue]);
			this.tips.RuntimeParam = null;
			bool flag5 = isResource || isExp;
			if (flag5)
			{
				this.tips.enabled = false;
			}
			else
			{
				this.tips.enabled = true;
				RowItemLine.SetMouseTipDisplayer(true, data, this.tips);
			}
			this.RefreshCricket();
			this.RefreshJiaoEggView();
		}

		// Token: 0x060061E5 RID: 25061 RVA: 0x002CE831 File Offset: 0x002CCA31
		public void OnItemHide()
		{
			this.ReturnCricket();
			this.ReturnJiaoEggView();
		}

		// Token: 0x060061E6 RID: 25062 RVA: 0x002CE844 File Offset: 0x002CCA44
		private void RefreshCricket()
		{
			bool isCricket = this.IsCricket;
			if (isCricket)
			{
				this.GetCricket();
			}
			else
			{
				this.ReturnCricket();
			}
		}

		// Token: 0x060061E7 RID: 25063 RVA: 0x002CE86C File Offset: 0x002CCA6C
		private void GetCricket()
		{
			bool flag = !this.CricketView;
			if (flag)
			{
				this.CricketView = SingletonObject.getInstance<ItemViewPool>().Get<CricketViewNew>();
				Transform cricketTransform = this.CricketView.transform;
				cricketTransform.SetParent(this.cricketHolder);
				cricketTransform.localPosition = Vector3.zero;
				cricketTransform.localScale = Vector3.one * 2f;
				cricketTransform.localRotation = Quaternion.identity;
			}
			this.CricketView.SetCricketData(this.Data.CricketColorId, this.Data.CricketPartId, false, this.Data, false);
		}

		// Token: 0x060061E8 RID: 25064 RVA: 0x002CE910 File Offset: 0x002CCB10
		private void ReturnCricket()
		{
			bool flag = this.CricketView;
			if (flag)
			{
				bool flag2 = !SingletonObject.IsDestroying;
				if (flag2)
				{
					this.CricketView.skeletonGraphic.enabled = true;
					this.CricketView.GetComponent<TooltipInvoker>().enabled = true;
					ItemViewPool instance = SingletonObject.getInstance<ItemViewPool>();
					if (instance != null)
					{
						instance.Return<CricketViewNew>(this.CricketView);
					}
				}
				this.CricketView = null;
			}
		}

		// Token: 0x060061E9 RID: 25065 RVA: 0x002CE980 File Offset: 0x002CCB80
		private void RefreshJiaoEggView()
		{
			bool isJiaoEgg = this.IsJiaoEgg;
			if (isJiaoEgg)
			{
				this.GetJiaoEggView();
			}
			else
			{
				this.ReturnJiaoEggView();
			}
		}

		// Token: 0x060061EA RID: 25066 RVA: 0x002CE9A8 File Offset: 0x002CCBA8
		private void GetJiaoEggView()
		{
			bool flag = !this.JiaoEggView;
			if (flag)
			{
				this.JiaoEggView = SingletonObject.getInstance<ItemViewPool>().Get<JiaoEggView>();
				bool flag2 = this.JiaoEggView;
				if (flag2)
				{
					Transform trans = this.JiaoEggView.transform;
					trans.SetParent(this.jiaoEggHolder);
					trans.localPosition = Vector3.zero;
					trans.localScale = Vector3.one * 2f;
					trans.localRotation = Quaternion.identity;
				}
			}
			bool flag3 = this.JiaoEggView;
			if (flag3)
			{
				this.JiaoEggView.Refresh(this.Data.JiaoLoongDisplayData, false);
			}
		}

		// Token: 0x060061EB RID: 25067 RVA: 0x002CEA5C File Offset: 0x002CCC5C
		private void ReturnJiaoEggView()
		{
			bool flag = this.JiaoEggView;
			if (flag)
			{
				bool flag2 = !SingletonObject.IsDestroying;
				if (flag2)
				{
					ItemViewPool instance = SingletonObject.getInstance<ItemViewPool>();
					if (instance != null)
					{
						instance.Return<JiaoEggView>(this.JiaoEggView);
					}
				}
				this.JiaoEggView = null;
			}
		}

		// Token: 0x040043F8 RID: 17400
		[SerializeField]
		private TextMeshProUGUI itemName;

		// Token: 0x040043F9 RID: 17401
		[SerializeField]
		private TextMeshProUGUI amount;

		// Token: 0x040043FA RID: 17402
		[SerializeField]
		private CImage icon;

		// Token: 0x040043FB RID: 17403
		[SerializeField]
		private CImage grade;

		// Token: 0x040043FC RID: 17404
		[SerializeField]
		private TooltipInvoker tips;

		// Token: 0x040043FD RID: 17405
		[SerializeField]
		private RectTransform cricketHolder;

		// Token: 0x040043FE RID: 17406
		[SerializeField]
		private RectTransform jiaoEggHolder;
	}
}
