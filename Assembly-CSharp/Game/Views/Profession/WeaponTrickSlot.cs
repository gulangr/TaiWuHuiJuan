using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007CF RID: 1999
	public class WeaponTrickSlot : MonoBehaviour
	{
		// Token: 0x060061C1 RID: 25025 RVA: 0x002CDD8C File Offset: 0x002CBF8C
		public void Refresh(int slotIndex, int optionIndex, List<string> optionList, Action<int, int> onChange)
		{
			this.textTrick.text = optionList[optionIndex];
			this.SetIsChanged(false);
			this.dropdown.ClearOptions();
			this.dropdown.AddOptions(optionList);
			this.dropdown.SetValueWithoutNotify(optionIndex);
			this.dropdown.onValueChanged.RemoveAllListeners();
			this.dropdown.onValueChanged.AddListener(delegate(int value)
			{
				this.textTrick.text = optionList[value];
				onChange(slotIndex, value);
			});
		}

		// Token: 0x060061C2 RID: 25026 RVA: 0x002CDE36 File Offset: 0x002CC036
		public void SetIsChanged(bool isChanged)
		{
			this.changedEffect.SetActive(isChanged);
		}

		// Token: 0x040043D8 RID: 17368
		[SerializeField]
		private TextMeshProUGUI textTrick;

		// Token: 0x040043D9 RID: 17369
		[SerializeField]
		private GameObject changedEffect;

		// Token: 0x040043DA RID: 17370
		[SerializeField]
		private CDropdown dropdown;
	}
}
