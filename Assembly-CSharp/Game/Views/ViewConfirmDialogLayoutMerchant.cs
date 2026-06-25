using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000702 RID: 1794
	public class ViewConfirmDialogLayoutMerchant : UIBase
	{
		// Token: 0x060054D2 RID: 21714 RVA: 0x0027548B File Offset: 0x0027368B
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<ConfirmDialogLayoutMerchantCmd>("Cmd", out this._showingCmd);
		}

		// Token: 0x060054D3 RID: 21715 RVA: 0x002754A0 File Offset: 0x002736A0
		private void OnEnable()
		{
			this.title.text = this._showingCmd.Title.ColorReplace();
			this.descUpper.text = this._showingCmd.ContentUpper.ColorReplace();
			this.descLower.text = this._showingCmd.ContentLower.ColorReplace();
			this.unavailableContent.gameObject.SetActive(!string.IsNullOrWhiteSpace(this._showingCmd.ContentUnavailable));
			this.unavailableContent.text = this._showingCmd.ContentUnavailable.ColorReplace();
			bool enough = true;
			ConfirmDialogCost cost = this._showingCmd.ConfirmDialogCost;
			bool flag = cost.ValueCost > cost.ValueHave;
			if (flag)
			{
				enough = false;
			}
			this.propertyCost.Set(this._costIcon[cost.Type], this._costName[cost.Type], LanguageKey.LK_Make_Resource_Require_Meet.TrFormat(CommonUtils.GetDisplayStringForNum(cost.ValueHave, 100000).SetColor((cost.ValueHave >= cost.ValueCost) ? "brightblue" : "brightred"), CommonUtils.GetDisplayStringForNum(cost.ValueCost, 100000)), null, false);
			this.confirmBtn.interactable = enough;
		}

		// Token: 0x060054D4 RID: 21716 RVA: 0x002755F4 File Offset: 0x002737F4
		protected override void OnClick(Transform btn)
		{
			UIManager.Instance.HideUI(this.Element);
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "BtnYes"))
			{
				if (a == "BtnNo")
				{
					Action no = this._showingCmd.No;
					if (no != null)
					{
						no();
					}
				}
			}
			else
			{
				Action yes = this._showingCmd.Yes;
				if (yes != null)
				{
					yes();
				}
			}
		}

		// Token: 0x060054D5 RID: 21717 RVA: 0x0027566C File Offset: 0x0027386C
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Enter.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				UIManager.Instance.HideUI(this.Element);
				Action yes = this._showingCmd.Yes;
				if (yes != null)
				{
					yes();
				}
			}
			else
			{
				bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
					UIManager.Instance.HideUI(this.Element);
					Action no = this._showingCmd.No;
					if (no != null)
					{
						no();
					}
				}
			}
		}

		// Token: 0x040039A8 RID: 14760
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x040039A9 RID: 14761
		public TextMeshProUGUI title;

		// Token: 0x040039AA RID: 14762
		public TextMeshProUGUI descUpper;

		// Token: 0x040039AB RID: 14763
		public TextMeshProUGUI descLower;

		// Token: 0x040039AC RID: 14764
		public TextMeshProUGUI unavailableContent;

		// Token: 0x040039AD RID: 14765
		public PropertyItem propertyCost;

		// Token: 0x040039AE RID: 14766
		public static Dictionary<sbyte, EConfirmDialogCostType> ResourceTypeToEnum = new Dictionary<sbyte, EConfirmDialogCostType>
		{
			{
				0,
				EConfirmDialogCostType.Food
			},
			{
				1,
				EConfirmDialogCostType.Wood
			},
			{
				2,
				EConfirmDialogCostType.Metal
			},
			{
				3,
				EConfirmDialogCostType.Jade
			},
			{
				4,
				EConfirmDialogCostType.Fabric
			},
			{
				5,
				EConfirmDialogCostType.Herb
			},
			{
				6,
				EConfirmDialogCostType.Money
			},
			{
				7,
				EConfirmDialogCostType.Authority
			}
		};

		// Token: 0x040039AF RID: 14767
		private Dictionary<EConfirmDialogCostType, string> _costIcon = new Dictionary<EConfirmDialogCostType, string>
		{
			{
				EConfirmDialogCostType.Food,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 0)
			},
			{
				EConfirmDialogCostType.Wood,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 1)
			},
			{
				EConfirmDialogCostType.Metal,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 2)
			},
			{
				EConfirmDialogCostType.Jade,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 3)
			},
			{
				EConfirmDialogCostType.Fabric,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 4)
			},
			{
				EConfirmDialogCostType.Herb,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 5)
			},
			{
				EConfirmDialogCostType.Money,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 6)
			},
			{
				EConfirmDialogCostType.Authority,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 7)
			},
			{
				EConfirmDialogCostType.Exp,
				string.Format("{0}{1}", "ui9_icon_resource_bar_", 8)
			},
			{
				EConfirmDialogCostType.ActionPoint,
				"ui9_icon_event_action_point_0"
			}
		};

		// Token: 0x040039B0 RID: 14768
		private Dictionary<EConfirmDialogCostType, string> _costName = new Dictionary<EConfirmDialogCostType, string>
		{
			{
				EConfirmDialogCostType.Food,
				LanguageKey.LK_Resource_Name_Food.Tr()
			},
			{
				EConfirmDialogCostType.Wood,
				LanguageKey.LK_Resource_Name_Wood.Tr()
			},
			{
				EConfirmDialogCostType.Metal,
				LanguageKey.LK_Resource_Name_Metal.Tr()
			},
			{
				EConfirmDialogCostType.Jade,
				LanguageKey.LK_Resource_Name_Jade.Tr()
			},
			{
				EConfirmDialogCostType.Fabric,
				LanguageKey.LK_Resource_Name_Fabric.Tr()
			},
			{
				EConfirmDialogCostType.Herb,
				LanguageKey.LK_Resource_Name_Herb.Tr()
			},
			{
				EConfirmDialogCostType.Money,
				LanguageKey.LK_Resource_Name_Money.Tr()
			},
			{
				EConfirmDialogCostType.Authority,
				LanguageKey.LK_Resource_Name_Authority.Tr()
			},
			{
				EConfirmDialogCostType.Exp,
				LanguageKey.LK_Exp.Tr()
			},
			{
				EConfirmDialogCostType.ActionPoint,
				LanguageKey.LK_Day_Tips.Tr()
			}
		};

		// Token: 0x040039B1 RID: 14769
		private ConfirmDialogLayoutMerchantCmd _showingCmd;
	}
}
