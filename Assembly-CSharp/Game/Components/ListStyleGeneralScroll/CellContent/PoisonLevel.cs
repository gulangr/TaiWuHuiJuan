using System;
using System.Collections.Generic;
using Game.Components.Item;
using GameData.Domains.Item;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED3 RID: 3795
	public class PoisonLevel : MonoBehaviour
	{
		// Token: 0x0600AF18 RID: 44824 RVA: 0x004FC784 File Offset: 0x004FA984
		public void Refresh(PoisonSlot poisonSlot)
		{
			this.emptyBg.SetActive(false);
			this.poisonBg.SetActive(true);
			this.icon.gameObject.SetActive(true);
			this.grade.gameObject.SetActive(true);
			sbyte poisonType = poisonSlot.GetPoisonType();
			PoisonsAndLevels poisonsAndLevels = poisonSlot.GetPoisonsAndLevels();
			List<short> poisonList = poisonSlot.GetAllMedicineTemplateId(false);
			sbyte maxGrade = this.GetRemovePoisonMaxGrade(poisonList);
			GradeBackVisual.ApplyTint(this.grade, maxGrade);
			this.icon.SetSprite("ui9_icon_poison_2_" + poisonType.ToString(), false, null);
			sbyte level = poisonsAndLevels.GetLevel((int)poisonType);
			this.SetLevel((int)level, poisonType);
		}

		// Token: 0x0600AF19 RID: 44825 RVA: 0x004FC830 File Offset: 0x004FAA30
		private sbyte GetRemovePoisonMaxGrade(List<short> medicineList)
		{
			sbyte maxMedicine = 0;
			for (int i = 0; i < medicineList.Count; i++)
			{
				short medicine = medicineList[i];
				sbyte curGrade = ItemTemplateHelper.GetGrade(8, medicine);
				maxMedicine = Math.Max(maxMedicine, curGrade);
			}
			return maxMedicine;
		}

		// Token: 0x0600AF1A RID: 44826 RVA: 0x004FC878 File Offset: 0x004FAA78
		public void RefreshEmpty()
		{
			this.icon.SetSprite("ui9_icon_item_empty_small", false, null);
			this.icon.gameObject.SetActive(false);
			this.emptyBg.SetActive(true);
			this.poisonBg.SetActive(false);
			this.grade.gameObject.SetActive(false);
			this.HideLevel();
		}

		// Token: 0x0600AF1B RID: 44827 RVA: 0x004FC8E0 File Offset: 0x004FAAE0
		public void RefreshNotIdentified()
		{
			this.icon.SetSprite("ui9_icon_poison_not_identified", false, null);
			this.icon.gameObject.SetActive(true);
			this.emptyBg.SetActive(true);
			this.poisonBg.SetActive(false);
			this.grade.gameObject.SetActive(false);
			this.HideLevel();
		}

		// Token: 0x0600AF1C RID: 44828 RVA: 0x004FC948 File Offset: 0x004FAB48
		private void HideLevel()
		{
			foreach (CImage level in this.poisonLevel)
			{
				level.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AF1D RID: 44829 RVA: 0x004FC980 File Offset: 0x004FAB80
		private void SetLevel(int level, sbyte poisonType)
		{
			for (int i = 0; i < this.poisonLevel.Length; i++)
			{
				CImage poison = this.poisonLevel[i];
				poison.gameObject.SetActive(i == level - 1);
			}
		}

		// Token: 0x04008799 RID: 34713
		[SerializeField]
		private CImage icon;

		// Token: 0x0400879A RID: 34714
		[SerializeField]
		private GameObject emptyBg;

		// Token: 0x0400879B RID: 34715
		[SerializeField]
		private GameObject poisonBg;

		// Token: 0x0400879C RID: 34716
		[SerializeField]
		private CImage grade;

		// Token: 0x0400879D RID: 34717
		[SerializeField]
		private CImage[] poisonLevel = new CImage[3];
	}
}
