using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B21 RID: 2849
	public class CombatNeiliType : MonoBehaviour
	{
		// Token: 0x06008BC1 RID: 35777 RVA: 0x00408980 File Offset: 0x00406B80
		public void Set(sbyte neiliType)
		{
			NeiliTypeItem typeConfig = NeiliType.Instance[neiliType];
			bool isBuff = typeConfig.ColorType == 1;
			this.icon.SetSprite(string.Format("ui9_icon_neili_{0}", neiliType), false, null);
			TMP_Text tmp_Text = this.text;
			string name = typeConfig.Name;
			tmp_Text.text = name.Substring(3, name.Length - 3).SetColor(isBuff ? "lightblue" : "pinkyellow");
			this.mouseTip.PresetParam[0] = typeConfig.Name;
			this.mouseTip.PresetParam[1] = typeConfig.Desc.ColorReplace();
		}

		// Token: 0x04006AFF RID: 27391
		[SerializeField]
		private CImage icon;

		// Token: 0x04006B00 RID: 27392
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04006B01 RID: 27393
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}
