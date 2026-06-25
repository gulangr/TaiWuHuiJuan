using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000700 RID: 1792
	public class ViewConfirmDialog : UIBase
	{
		// Token: 0x060054C7 RID: 21703 RVA: 0x00274C50 File Offset: 0x00272E50
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<ConfirmDialogCmd>("Cmd", out this._showingCmd);
		}

		// Token: 0x060054C8 RID: 21704 RVA: 0x00274C68 File Offset: 0x00272E68
		private void OnEnable()
		{
			this.title.text = this._showingCmd.Title.ColorReplace();
			this.descUpper.text = this._showingCmd.ContentUpper.ColorReplace();
			this.descLower.text = this._showingCmd.ContentLower.ColorReplace();
			this.costsHolder.gameObject.SetActive(this._showingCmd.ValueStyle == 0);
			this.changeHolder.gameObject.SetActive(this._showingCmd.ValueStyle == 1);
			bool flag = this.gainHolder != null;
			if (flag)
			{
				this.gainHolder.gameObject.SetActive(this._showingCmd.ValueStyle == 2);
			}
			switch (this._showingCmd.ValueStyle)
			{
			case 0:
				this.RefreshCost();
				break;
			case 1:
				this.RefreshChange();
				break;
			case 2:
				this.RefreshGain();
				break;
			}
		}

		// Token: 0x060054C9 RID: 21705 RVA: 0x00274D74 File Offset: 0x00272F74
		private void RefreshCost()
		{
			bool enough = true;
			for (int index = 0; index < this._showingCmd.ConfirmDialogCost.Count; index++)
			{
				ConfirmDialogCost cost = this._showingCmd.ConfirmDialogCost[index];
				bool flag = cost.ValueCost > cost.ValueHave;
				if (flag)
				{
					enough = false;
				}
				bool flag2 = index >= this.costsHolder.childCount;
				if (flag2)
				{
					Object.Instantiate<GameObject>(this.propertyTemplate, this.costsHolder);
				}
				Transform obj = this.costsHolder.GetChild(index);
				obj.GetComponent<PropertyItem>().Set(ViewConfirmDialog.CostIcon[cost.Type], ViewConfirmDialog.CostName[cost.Type].Tr(), LanguageKey.LK_Make_Resource_Require_Meet.TrFormat(CommonUtils.GetDisplayStringForNum(cost.ValueHave, 100000).SetColor((cost.ValueHave >= cost.ValueCost) ? "brightblue" : "brightred"), CommonUtils.GetDisplayStringForNum(cost.ValueCost, 100000)), null, false);
				obj.gameObject.SetActive(true);
			}
			for (int index2 = this._showingCmd.ConfirmDialogCost.Count; index2 < this.costsHolder.childCount; index2++)
			{
				this.costsHolder.GetChild(index2).gameObject.SetActive(false);
			}
			this.confirmBtn.interactable = enough;
		}

		// Token: 0x060054CA RID: 21706 RVA: 0x00274EF2 File Offset: 0x002730F2
		private void RefreshChange()
		{
			this.confirmBtn.interactable = true;
			this.changeHolder.Rebuild<RectTransform>(this._showingCmd.ChangeInfos.Count, delegate(RectTransform rectTransform, int index)
			{
				ChangeInfo changeInfo = this._showingCmd.ChangeInfos[index];
				rectTransform.Find("Icon").GetComponent<CImage>().SetSprite(ViewConfirmDialog.CostIcon[changeInfo.Type], false, null);
				rectTransform.Find("Title").GetComponent<TextMeshProUGUI>().SetText(ViewConfirmDialog.CostName[changeInfo.Type].Tr(), true);
				rectTransform.Find("From").GetComponent<TextMeshProUGUI>().SetText(changeInfo.fromValue.ToString(), true);
				rectTransform.Find("To").GetComponent<TextMeshProUGUI>().SetText(changeInfo.toValue.ToString(), true);
			});
		}

		// Token: 0x060054CB RID: 21707 RVA: 0x00274F2C File Offset: 0x0027312C
		private void RefreshGain()
		{
			for (int index = 0; index < this._showingCmd.GainInfos.Count; index++)
			{
				GainInfo gain = this._showingCmd.GainInfos[index];
				bool flag = index >= this.gainHolder.childCount;
				if (flag)
				{
					Object.Instantiate<GameObject>(this.propertyTemplate, this.gainHolder);
				}
				Transform obj = this.gainHolder.GetChild(index);
				obj.GetComponent<PropertyItem>().Set(ViewConfirmDialog.CostIcon[gain.Type], ViewConfirmDialog.CostName[gain.Type].Tr(), CommonUtils.GetDisplayStringForNum(gain.Value, 100000), null, false);
				obj.gameObject.SetActive(true);
			}
			for (int index2 = this._showingCmd.GainInfos.Count; index2 < this.gainHolder.childCount; index2++)
			{
				this.gainHolder.GetChild(index2).gameObject.SetActive(false);
			}
		}

		// Token: 0x060054CC RID: 21708 RVA: 0x0027504C File Offset: 0x0027324C
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

		// Token: 0x060054CD RID: 21709 RVA: 0x002750C4 File Offset: 0x002732C4
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Enter.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				bool interactable = this.confirmBtn.interactable;
				if (interactable)
				{
					UIManager.Instance.HideUI(this.Element);
					Action yes = this._showingCmd.Yes;
					if (yes != null)
					{
						yes();
					}
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

		// Token: 0x04003995 RID: 14741
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04003996 RID: 14742
		public TextMeshProUGUI title;

		// Token: 0x04003997 RID: 14743
		public TextMeshProUGUI descUpper;

		// Token: 0x04003998 RID: 14744
		public TextMeshProUGUI descLower;

		// Token: 0x04003999 RID: 14745
		public Transform costsHolder;

		// Token: 0x0400399A RID: 14746
		public TemplatedContainerAssemblyNew changeHolder;

		// Token: 0x0400399B RID: 14747
		public Transform gainHolder;

		// Token: 0x0400399C RID: 14748
		public GameObject propertyTemplate;

		// Token: 0x0400399D RID: 14749
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

		// Token: 0x0400399E RID: 14750
		public static readonly Dictionary<EConfirmDialogCostType, string> CostIcon = new Dictionary<EConfirmDialogCostType, string>
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
				"ui9_icon_resource_bar_0_0"
			}
		};

		// Token: 0x0400399F RID: 14751
		public static readonly Dictionary<EConfirmDialogCostType, LanguageKey> CostName = new Dictionary<EConfirmDialogCostType, LanguageKey>
		{
			{
				EConfirmDialogCostType.Food,
				LanguageKey.LK_Resource_Name_Food
			},
			{
				EConfirmDialogCostType.Wood,
				LanguageKey.LK_Resource_Name_Wood
			},
			{
				EConfirmDialogCostType.Metal,
				LanguageKey.LK_Resource_Name_Metal
			},
			{
				EConfirmDialogCostType.Jade,
				LanguageKey.LK_Resource_Name_Jade
			},
			{
				EConfirmDialogCostType.Fabric,
				LanguageKey.LK_Resource_Name_Fabric
			},
			{
				EConfirmDialogCostType.Herb,
				LanguageKey.LK_Resource_Name_Herb
			},
			{
				EConfirmDialogCostType.Money,
				LanguageKey.LK_Resource_Name_Money
			},
			{
				EConfirmDialogCostType.Authority,
				LanguageKey.LK_Resource_Name_Authority
			},
			{
				EConfirmDialogCostType.Exp,
				LanguageKey.LK_Exp
			},
			{
				EConfirmDialogCostType.ActionPoint,
				LanguageKey.LK_Day_Tips
			}
		};

		// Token: 0x040039A0 RID: 14752
		private ConfirmDialogCmd _showingCmd;
	}
}
