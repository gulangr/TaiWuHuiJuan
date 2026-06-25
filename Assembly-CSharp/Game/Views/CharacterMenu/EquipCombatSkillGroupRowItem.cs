using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B75 RID: 2933
	public class EquipCombatSkillGroupRowItem : MonoBehaviour
	{
		// Token: 0x060090F4 RID: 37108 RVA: 0x00439326 File Offset: 0x00437526
		private void Awake()
		{
			this._layoutElement = base.GetComponent<LayoutElement>();
		}

		// Token: 0x060090F5 RID: 37109 RVA: 0x00439338 File Offset: 0x00437538
		public void Set(ViewCharacterMenuEquipCombatSkill parentView, RowData rowData, List<CombatSkillDisplayDataCharacterMenuListItem> list)
		{
			bool flag = this.titleRoot != null && this.titleRoot.activeSelf != rowData.IsTitle;
			if (flag)
			{
				this.titleRoot.SetActive(rowData.IsTitle);
			}
			bool flag2 = this.contentRoot != null && this.contentRoot.activeSelf == rowData.IsTitle;
			if (flag2)
			{
				this.contentRoot.SetActive(!rowData.IsTitle);
			}
			this._layoutElement.preferredHeight = (rowData.IsTitle ? this.titlePreferredHeight : this.contentPreferredHeight);
			bool isTitle = rowData.IsTitle;
			if (isTitle)
			{
				EquipCombatSkillGroupTitleView titleView = this.titleRoot.GetComponent<EquipCombatSkillGroupTitleView>();
				bool flag3 = titleView == null;
				if (flag3)
				{
					titleView = this.titleRoot.GetComponentInChildren<EquipCombatSkillGroupTitleView>(true);
				}
				bool flag4 = titleView != null;
				if (flag4)
				{
					titleView.Set(rowData.IsEquippedGroup);
				}
			}
			else
			{
				bool flag5 = this.contentRoot == null || list == null;
				if (!flag5)
				{
					for (int i = 0; i < 3; i++)
					{
						bool flag6 = i >= this.contentRoot.transform.childCount;
						if (flag6)
						{
							break;
						}
						Transform cell = this.contentRoot.transform.GetChild(i);
						int dataIndex = rowData.StartIndex + i;
						bool flag7 = dataIndex < 0 || dataIndex >= list.Count;
						if (flag7)
						{
							bool activeSelf = cell.gameObject.activeSelf;
							if (activeSelf)
							{
								cell.gameObject.SetActive(false);
							}
						}
						else
						{
							bool flag8 = !cell.gameObject.activeSelf;
							if (flag8)
							{
								cell.gameObject.SetActive(true);
							}
							EquipCombatSkillItem item = cell.GetComponent<EquipCombatSkillItem>();
							bool flag9 = item == null;
							if (flag9)
							{
								item = cell.GetComponentInChildren<EquipCombatSkillItem>(true);
							}
							bool flag10 = item == null;
							if (!flag10)
							{
								CombatSkillDisplayDataCharacterMenuListItem skillData = list[dataIndex];
								item.combatSkillItem.SetByCharacterMenuList(skillData);
								bool flag11 = !parentView.IsInFavoriteMode;
								if (flag11)
								{
									item.Set(parentView, dataIndex, skillData, rowData.IsEquippedGroup ? EEquipCombatSkillItemType.ScrollEquipped : EEquipCombatSkillItemType.ScrollUnEquipped, true);
								}
								else
								{
									item.Set(parentView, dataIndex, skillData, EEquipCombatSkillItemType.FavoriteMode, true);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04006FAD RID: 28589
		[SerializeField]
		private GameObject titleRoot;

		// Token: 0x04006FAE RID: 28590
		[SerializeField]
		private GameObject contentRoot;

		// Token: 0x04006FAF RID: 28591
		[SerializeField]
		private float titlePreferredHeight;

		// Token: 0x04006FB0 RID: 28592
		[SerializeField]
		private float contentPreferredHeight;

		// Token: 0x04006FB1 RID: 28593
		private LayoutElement _layoutElement;
	}
}
