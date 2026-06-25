using System;
using System.Collections.Generic;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000739 RID: 1849
	public class VillagerRoleInfoBook : MonoBehaviour
	{
		// Token: 0x0600596A RID: 22890 RVA: 0x00297707 File Offset: 0x00295907
		public void Refresh(VillagerRoleItem roleConfig)
		{
			this._config = roleConfig;
			this.RefreshBooks();
		}

		// Token: 0x0600596B RID: 22891 RVA: 0x00297718 File Offset: 0x00295918
		private void RefreshBooks()
		{
			this._bookItems.Clear();
			foreach (sbyte type in this._config.LearnableLifeSkillTypes)
			{
				this._bookItems.Add(VillagerRoleInfoBook.BookItem.CreateLife(type));
			}
			sbyte[] learnableCombatSkillTypes = this._config.LearnableCombatSkillTypes;
			int num = 0;
			if (num < learnableCombatSkillTypes.Length)
			{
				sbyte type2 = learnableCombatSkillTypes[num];
				this._bookItems.Add(VillagerRoleInfoBook.BookItem.CreateCombat(0));
			}
			CommonUtils.PrepareEnoughChildren(this.bookLayoutRoot.transform, this.bookItemTemplate.gameObject, this._bookItems.Count, null);
			for (int i = 0; i < this._bookItems.Count; i++)
			{
				VillagerRoleInfoBook.BookItem item = this._bookItems[i];
				Refers refers = this.bookLayoutRoot.GetChild(i).GetComponent<Refers>();
				CImage icon = refers.CGet<CImage>("Icon");
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				item.SetIcon(icon);
				item.SetLabel(label);
			}
		}

		// Token: 0x04003D80 RID: 15744
		private VillagerRoleItem _config;

		// Token: 0x04003D81 RID: 15745
		private readonly List<VillagerRoleInfoBook.BookItem> _bookItems = new List<VillagerRoleInfoBook.BookItem>();

		// Token: 0x04003D82 RID: 15746
		[SerializeField]
		private RectTransform bookLayoutRoot;

		// Token: 0x04003D83 RID: 15747
		[SerializeField]
		private Refers bookItemTemplate;

		// Token: 0x02001C0B RID: 7179
		private struct BookItem
		{
			// Token: 0x0600E5E7 RID: 58855 RVA: 0x005EEFF8 File Offset: 0x005ED1F8
			public static VillagerRoleInfoBook.BookItem CreateCombat(sbyte type)
			{
				return new VillagerRoleInfoBook.BookItem
				{
					_isCombatSkill = true,
					_type = type
				};
			}

			// Token: 0x0600E5E8 RID: 58856 RVA: 0x005EF024 File Offset: 0x005ED224
			public static VillagerRoleInfoBook.BookItem CreateLife(sbyte type)
			{
				return new VillagerRoleInfoBook.BookItem
				{
					_isCombatSkill = false,
					_type = type
				};
			}

			// Token: 0x0600E5E9 RID: 58857 RVA: 0x005EF050 File Offset: 0x005ED250
			public void SetIcon(CImage icon)
			{
				icon.SetSprite(this._isCombatSkill ? string.Format("{0}{1}", "ui9_back_combatskill_small_1_", this._type) : string.Format("{0}{1}", "ui9_icon_craftsmanship_big_2_", this._type), false, null);
			}

			// Token: 0x0600E5EA RID: 58858 RVA: 0x005EF0A5 File Offset: 0x005ED2A5
			public void SetLabel(TextMeshProUGUI label)
			{
				label.text = (this._isCombatSkill ? LocalStringManager.Get(LanguageKey.LK_CombatSkill_2) : LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", this._type)));
			}

			// Token: 0x0400BF63 RID: 48995
			private bool _isCombatSkill;

			// Token: 0x0400BF64 RID: 48996
			private sbyte _type;
		}
	}
}
