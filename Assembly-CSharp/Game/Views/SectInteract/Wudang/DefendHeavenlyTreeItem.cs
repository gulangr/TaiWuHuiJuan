using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009DB RID: 2523
	public class DefendHeavenlyTreeItem : MonoBehaviour
	{
		// Token: 0x17000D9C RID: 3484
		// (get) Token: 0x06007B60 RID: 31584 RVA: 0x00394D6F File Offset: 0x00392F6F
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x06007B61 RID: 31585 RVA: 0x00394D78 File Offset: 0x00392F78
		public void SetData(string charName, int growPoint, int enemyCount, bool isSelected)
		{
			this.textName.text = charName;
			float rate = Mathf.Min(1f, (float)growPoint / 900f);
			int progress = (int)Math.Round((double)(100f * rate), MidpointRounding.AwayFromZero);
			this.textGrowPoint.text = string.Format("{0}%", progress);
			this.textEnemyCount.text = enemyCount.ToString();
			this.selectedObj.SetActive(isSelected);
		}

		// Token: 0x04005DAB RID: 23979
		[SerializeField]
		private CButton button;

		// Token: 0x04005DAC RID: 23980
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005DAD RID: 23981
		[SerializeField]
		private TextMeshProUGUI textGrowPoint;

		// Token: 0x04005DAE RID: 23982
		[SerializeField]
		private TextMeshProUGUI textEnemyCount;

		// Token: 0x04005DAF RID: 23983
		[SerializeField]
		private GameObject selectedObj;
	}
}
