using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.LegendaryBook
{
	// Token: 0x0200098B RID: 2443
	public class LegendaryBookEffectItem : MonoBehaviour
	{
		// Token: 0x06007598 RID: 30104 RVA: 0x0036C77E File Offset: 0x0036A97E
		public void SetCanInteract(bool interactable)
		{
			this._interactable = interactable;
			this.RefreshDisplay();
		}

		// Token: 0x06007599 RID: 30105 RVA: 0x0036C78F File Offset: 0x0036A98F
		public void SetUnlocked(bool unlocked)
		{
			this._unlocked = unlocked;
			this.RefreshDisplay();
		}

		// Token: 0x0600759A RID: 30106 RVA: 0x0036C7A0 File Offset: 0x0036A9A0
		public void SetSkill(short skillId)
		{
			this._skillId = skillId;
			this._itemKey = ItemKey.Invalid;
			this.RefreshDisplay();
		}

		// Token: 0x0600759B RID: 30107 RVA: 0x0036C7BC File Offset: 0x0036A9BC
		public void SetWeapon(ItemKey weaponKey)
		{
			this._skillId = -1;
			this._itemKey = weaponKey;
			this.RefreshDisplay();
		}

		// Token: 0x0600759C RID: 30108 RVA: 0x0036C7D4 File Offset: 0x0036A9D4
		private void RefreshDisplay()
		{
			this.BtnMain.interactable = this._interactable;
			this.ItemIcon.gameObject.SetActive(false);
			bool flag = !this._unlocked || !this._interactable;
			if (flag)
			{
				this.MainBg.SetSprite("ui9_back_legendbook_buffbtn_0_2", false, null);
				this.ActiveEff.gameObject.SetActive(false);
			}
			else
			{
				bool flag2 = !this.ActiveEff.gameObject.activeSelf;
				if (flag2)
				{
					this.effYinYu.SetActive(this.isYin);
					this.effYangyu.SetActive(!this.isYin);
					this.ActiveEff.gameObject.SetActive(true);
				}
				bool flag3 = this._skillId >= 0 || this._itemKey != ItemKey.Invalid;
				if (flag3)
				{
					this.ItemIcon.gameObject.SetActive(true);
					int grade = 0;
					bool flag4 = this._skillId >= 0;
					if (flag4)
					{
						CombatSkillItem skillConfig = CombatSkill.Instance[this._skillId];
						grade = (int)skillConfig.Grade;
					}
					bool flag5 = this._itemKey != ItemKey.Invalid;
					if (flag5)
					{
						WeaponItem itemConfig = Weapon.Instance[this._itemKey.TemplateId];
						grade = (int)itemConfig.Grade;
					}
					this.MainBg.SetSprite("ui9_back_legendbook_buffbtn_0" + (8 - (grade - 1)).ToString(), false, null);
					this.Highlight.SetSprite("ui9_back_legendbook_buffbtn_1_1", false, null);
				}
				else
				{
					this.MainBg.SetSprite("ui9_back_legendbook_buffbtn_0_0", false, null);
					this.Highlight.SetSprite("ui9_back_legendbook_buffbtn_0_1", false, null);
				}
			}
		}

		// Token: 0x04005849 RID: 22601
		public int UserInt;

		// Token: 0x0400584A RID: 22602
		public CImage MainBg;

		// Token: 0x0400584B RID: 22603
		public CImage Highlight;

		// Token: 0x0400584C RID: 22604
		public CImage ItemIcon;

		// Token: 0x0400584D RID: 22605
		public TextMeshProUGUI NameEnable;

		// Token: 0x0400584E RID: 22606
		public TextMeshProUGUI NameDisable;

		// Token: 0x0400584F RID: 22607
		public GameObject ActiveEff;

		// Token: 0x04005850 RID: 22608
		public GameObject effYinYu;

		// Token: 0x04005851 RID: 22609
		public GameObject effYangyu;

		// Token: 0x04005852 RID: 22610
		public CButton BtnMain;

		// Token: 0x04005853 RID: 22611
		public bool isYin;

		// Token: 0x04005854 RID: 22612
		private bool _interactable;

		// Token: 0x04005855 RID: 22613
		private bool _unlocked;

		// Token: 0x04005856 RID: 22614
		private short _skillId = -1;

		// Token: 0x04005857 RID: 22615
		private ItemKey _itemKey = ItemKey.Invalid;
	}
}
