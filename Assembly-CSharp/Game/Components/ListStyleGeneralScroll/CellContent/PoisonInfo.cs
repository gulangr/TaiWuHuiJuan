using System;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED0 RID: 3792
	public class PoisonInfo : MonoBehaviour
	{
		// Token: 0x0600AF0F RID: 44815 RVA: 0x004FC2A0 File Offset: 0x004FA4A0
		public void Refresh(PoisonSlot poisonSlot, bool needBack = false)
		{
			sbyte poisonType = poisonSlot.GetPoisonType();
			PoisonsAndLevels poisonsAndLevels = poisonSlot.GetPoisonsAndLevels();
			short value = poisonsAndLevels.GetValue((int)poisonType);
			this.icon.SetSprite("ui9_icon_poison_2_" + poisonType.ToString(), false, null);
			this.poisonValue.SetText(value.ToString(), true);
			this.poisonName.SetText(LocalStringManager.Get(CommonUtils.PoisonNameLStrings[(int)poisonType]), true);
			sbyte level = poisonsAndLevels.GetLevel((int)poisonType);
			string levelSprite = string.Format("{0}6_{1}", "ui9_icon_poison_Level_", level);
			this.imgPoisonLevel.SetSprite(levelSprite, false, null);
			this.imgPoisonLevel.gameObject.SetActive(true);
			if (needBack)
			{
				this.imgPoisonBack.SetSprite("ui9_back_poison_" + poisonType.ToString(), false, null);
				this.imgPoisonBack.gameObject.SetActive(true);
			}
			else
			{
				this.imgPoisonBack.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AF10 RID: 44816 RVA: 0x004FC3A4 File Offset: 0x004FA5A4
		public void SetStyleEffect(bool isGray)
		{
			bool flag = this.grayRoot;
			if (flag)
			{
				this.grayRoot.SetStyleEffect(isGray, false);
			}
		}

		// Token: 0x04008791 RID: 34705
		[SerializeField]
		private CImage icon;

		// Token: 0x04008792 RID: 34706
		[SerializeField]
		private TextMeshProUGUI poisonName;

		// Token: 0x04008793 RID: 34707
		[SerializeField]
		private TextMeshProUGUI poisonValue;

		// Token: 0x04008794 RID: 34708
		[SerializeField]
		private CImage imgPoisonBack;

		// Token: 0x04008795 RID: 34709
		[SerializeField]
		private CImage imgPoisonLevel;

		// Token: 0x04008796 RID: 34710
		[SerializeField]
		private DisableStyleRoot grayRoot;
	}
}
