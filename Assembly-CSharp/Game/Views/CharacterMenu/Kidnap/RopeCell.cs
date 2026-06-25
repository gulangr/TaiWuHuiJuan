using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu.Kidnap
{
	// Token: 0x02000BB9 RID: 3001
	public class RopeCell : MonoBehaviour, ICellContent<RopeCellData>, ICellContent
	{
		// Token: 0x060096F6 RID: 38646 RVA: 0x004665D0 File Offset: 0x004647D0
		private void Awake()
		{
			bool flag = this.changeRopeButton != null;
			if (flag)
			{
				this.changeRopeButton.ClearAndAddListener(new Action(this.OnChangeRopeClicked));
			}
			bool flag2 = this.changeRopeButtonPointerTrigger != null;
			if (flag2)
			{
				this.changeRopeButtonPointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnRopeButtonPointerEnter));
				this.changeRopeButtonPointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnRopeButtonPointerExit));
			}
			bool flag3 = this.changeRopeButtonHoverItem != null;
			if (flag3)
			{
				this.changeRopeButtonHoverItem.SetActive(false);
			}
		}

		// Token: 0x060096F7 RID: 38647 RVA: 0x00466674 File Offset: 0x00464874
		public void SetData(RopeCellData data)
		{
			this._data = data;
			bool flag = ((data != null) ? data.KidnapCharDisplayData : null) == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.RefreshRopeDisplay(data.KidnapCharDisplayData.RopeItemKey);
				this.RefreshButtonState();
			}
		}

		// Token: 0x060096F8 RID: 38648 RVA: 0x004666C0 File Offset: 0x004648C0
		private void SetEmpty()
		{
			bool flag = this.ropeItemIcon != null;
			if (flag)
			{
				this.ropeItemIcon.gameObject.SetActive(false);
			}
			bool flag2 = this.ropeItemGradeIcon != null;
			if (flag2)
			{
				this.ropeItemGradeIcon.gameObject.SetActive(false);
			}
			bool flag3 = this.changeRopeButton != null;
			if (flag3)
			{
				this.changeRopeButton.gameObject.SetActive(false);
			}
		}

		// Token: 0x060096F9 RID: 38649 RVA: 0x00466734 File Offset: 0x00464934
		private void RefreshRopeDisplay(ItemKey ropeItemKey)
		{
			bool flag = this.ropeItemIcon == null;
			if (!flag)
			{
				bool hasTemplate = ropeItemKey.HasTemplate;
				if (hasTemplate)
				{
					this.ropeItemIcon.gameObject.SetActive(true);
					string iconName = ItemTemplateHelper.GetIcon(ropeItemKey.ItemType, ropeItemKey.TemplateId);
					this.ropeItemIcon.SetSprite(iconName, false, null);
					sbyte grade = ItemTemplateHelper.GetGrade(ropeItemKey.ItemType, ropeItemKey.TemplateId);
					this.ropeItemGradeIcon.SetSprite("ui9_icon_item_grade_" + grade.ToString(), false, null);
				}
				else
				{
					this.ropeItemIcon.gameObject.SetActive(false);
					bool flag2 = this.ropeItemGradeIcon != null;
					if (flag2)
					{
						this.ropeItemGradeIcon.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060096FA RID: 38650 RVA: 0x00466804 File Offset: 0x00464A04
		private void RefreshButtonState()
		{
			bool flag = this.changeRopeButton == null;
			if (!flag)
			{
				bool canSelectRope = this._data != null && this._data.CanOperate && this._data.CurrentCharacterIsTaiwu;
				this.changeRopeButton.gameObject.SetActive(true);
				this.changeRopeButton.interactable = canSelectRope;
			}
		}

		// Token: 0x060096FB RID: 38651 RVA: 0x00466868 File Offset: 0x00464A68
		private void OnChangeRopeClicked()
		{
			RopeCellData data = this._data;
			bool flag = ((data != null) ? data.KidnapCharDisplayData : null) == null;
			if (!flag)
			{
				SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
				{
					ItemSubType = 1206,
					OnlyFromInventory = true
				}, delegate(List<SelectedItemData> selectedItems)
				{
					bool flag2 = selectedItems.Count > 0;
					if (flag2)
					{
						ItemKey ropeItemKey = selectedItems[0].ItemData.Key;
						CharacterDomainMethod.Call.ChangeKidnappedCharacterRope(this._data.KidnapperCharId, this._data.KidnapCharDisplayData.CharacterId, ropeItemKey);
						Action onRopeChanged = this._data.OnRopeChanged;
						if (onRopeChanged != null)
						{
							onRopeChanged();
						}
					}
				}, "", new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount | ESelectItemColumnType.Type | ESelectItemColumnType.Value | ESelectItemColumnType.Weight | ESelectItemColumnType.EscapeRate));
				config.VisibleMainFilterToggles = new List<int>
				{
					6
				};
				config.HideSourceToggles = true;
				config.HideSortAndFilter = true;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("SelectItemConfig", config);
				UIElement.SelectItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.SelectItem);
			}
		}

		// Token: 0x060096FC RID: 38652 RVA: 0x00466920 File Offset: 0x00464B20
		private void OnRopeButtonPointerEnter()
		{
			bool flag = this.changeRopeButtonHoverItem != null && this.changeRopeButton != null && this.changeRopeButton.interactable;
			if (flag)
			{
				this.changeRopeButtonHoverItem.SetActive(true);
			}
		}

		// Token: 0x060096FD RID: 38653 RVA: 0x0046696C File Offset: 0x00464B6C
		private void OnRopeButtonPointerExit()
		{
			bool flag = this.changeRopeButtonHoverItem != null;
			if (flag)
			{
				this.changeRopeButtonHoverItem.SetActive(false);
			}
		}

		// Token: 0x040073E2 RID: 29666
		[SerializeField]
		private CImage ropeItemIcon;

		// Token: 0x040073E3 RID: 29667
		[SerializeField]
		private CImage ropeItemGradeIcon;

		// Token: 0x040073E4 RID: 29668
		[SerializeField]
		private CButton changeRopeButton;

		// Token: 0x040073E5 RID: 29669
		[SerializeField]
		private PointerTrigger changeRopeButtonPointerTrigger;

		// Token: 0x040073E6 RID: 29670
		[SerializeField]
		private GameObject changeRopeButtonHoverItem;

		// Token: 0x040073E7 RID: 29671
		private RopeCellData _data;
	}
}
