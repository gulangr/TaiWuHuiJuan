using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x02000726 RID: 1830
	public class AreaStateItem : MonoBehaviour
	{
		// Token: 0x0600578E RID: 22414 RVA: 0x0028AC04 File Offset: 0x00288E04
		public bool Set(int templateId, int value, string nameSuffix)
		{
			MapLegendItem cfg = MapLegend.Instance[templateId];
			ref string ptr = ref this.displayer.PresetParam[0];
			string[] presetParam = this.displayer.PresetParam;
			int num = 1;
			string name = cfg.Name;
			string desc = cfg.Desc;
			ptr = name;
			presetParam[num] = desc;
			this.img.SetSprite(cfg.Sprite + nameSuffix, false, null);
			bool flag = !this.img.enabled;
			if (flag)
			{
				this.img.SetSprite(cfg.Sprite, false, null);
			}
			int num2 = value;
			int num3 = num2;
			bool result;
			if (num3 >= 0)
			{
				if (num3 <= 0)
				{
					this.img.gameObject.SetActive(false);
					this.val.gameObject.SetActive(false);
					result = false;
				}
				else
				{
					this.img.material = null;
					this.img.gameObject.SetActive(true);
					this.val.gameObject.SetActive(true);
					this.val.text = value.ToString();
					result = true;
				}
			}
			else
			{
				this.img.material = null;
				this.img.gameObject.SetActive(true);
				this.val.gameObject.SetActive(false);
				result = true;
			}
			return result;
		}

		// Token: 0x04003C06 RID: 15366
		[SerializeField]
		private CImage img;

		// Token: 0x04003C07 RID: 15367
		[SerializeField]
		public TMP_Text val;

		// Token: 0x04003C08 RID: 15368
		[SerializeField]
		private TooltipInvoker displayer;
	}
}
