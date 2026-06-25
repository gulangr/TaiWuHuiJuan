using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EED RID: 3821
	public class WeaponTrickCell : MonoBehaviour, ICellContent<WeaponTrickData>, ICellContent
	{
		// Token: 0x0600AF51 RID: 44881 RVA: 0x004FDDB0 File Offset: 0x004FBFB0
		public void SetData(WeaponTrickData data)
		{
			for (int i = 0; i < this.trickArray.Length; i++)
			{
				GameObject trick = this.trickArray[i];
				sbyte templateId = data.TrickTemplateIdList[i];
				TrickTypeItem config = TrickType.Instance[templateId];
				TextMeshProUGUI text = trick.GetComponentInChildren<TextMeshProUGUI>(true);
				if (text != null)
				{
					text.SetText(config.ChineseName.SetColor(config.FontColor), true);
				}
			}
		}

		// Token: 0x040087EA RID: 34794
		[SerializeField]
		private GameObject[] trickArray;
	}
}
