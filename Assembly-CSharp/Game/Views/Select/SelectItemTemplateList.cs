using System;
using Config;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007AA RID: 1962
	public class SelectItemTemplateList : MonoBehaviour
	{
		// Token: 0x06005EE6 RID: 24294 RVA: 0x002B8608 File Offset: 0x002B6808
		public void Set(ItemDisplayData data, bool isSelected)
		{
			bool flag = this.selected.activeSelf != isSelected;
			if (flag)
			{
				this.selected.SetActive(isSelected);
			}
			IItemConfig config = data.Key.GetConfig();
			this.icon.SetSprite(config.Icon, false, null);
			this.nameText.text = config.Name;
			this.count.text = data.Amount.ToString();
			this.value.text = data.Value.ToString();
			this.weight.text = data.Weight.ToString();
			TMP_Text tmp_Text = this.escapeOdds;
			MiscItem misc = config as MiscItem;
			tmp_Text.text = ((misc != null) ? ("-" + misc.ReduceEscapeRate.ToString() + "%") : "-0%");
		}

		// Token: 0x040041AB RID: 16811
		[SerializeField]
		private GameObject selected;

		// Token: 0x040041AC RID: 16812
		[SerializeField]
		private CImage icon;

		// Token: 0x040041AD RID: 16813
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040041AE RID: 16814
		[SerializeField]
		private TextMeshProUGUI count;

		// Token: 0x040041AF RID: 16815
		[SerializeField]
		private TextMeshProUGUI value;

		// Token: 0x040041B0 RID: 16816
		[SerializeField]
		private TextMeshProUGUI weight;

		// Token: 0x040041B1 RID: 16817
		[SerializeField]
		private TextMeshProUGUI escapeOdds;
	}
}
