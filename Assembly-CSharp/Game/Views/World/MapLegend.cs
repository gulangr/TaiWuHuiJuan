using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x02000729 RID: 1833
	public class MapLegend : MonoBehaviour
	{
		// Token: 0x060057A0 RID: 22432 RVA: 0x0028B3B7 File Offset: 0x002895B7
		private void OnEnable()
		{
			this.text.text = MapLegend.Instance[this._index].Name;
		}

		// Token: 0x060057A1 RID: 22433 RVA: 0x0028B3DC File Offset: 0x002895DC
		public void Init(MapLegendItem item, CToggleGroupMultiSelect multiSelect)
		{
			this._index = (int)item.TemplateId;
			this.text.text = item.Name;
			this.icon.SetSprite(item.Sprite, false, null);
			multiSelect.Add(this.toggle);
			base.gameObject.SetActive(true);
			ref string ptr = ref this.invoker.PresetParam[0];
			string[] presetParam = this.invoker.PresetParam;
			int num = 1;
			string name = item.Name;
			string desc = item.Desc;
			ptr = name;
			presetParam[num] = desc;
		}

		// Token: 0x04003C1D RID: 15389
		[SerializeField]
		private CImage icon;

		// Token: 0x04003C1E RID: 15390
		[SerializeField]
		private TMP_Text text;

		// Token: 0x04003C1F RID: 15391
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04003C20 RID: 15392
		[SerializeField]
		private TooltipInvoker invoker;

		// Token: 0x04003C21 RID: 15393
		private int _index;
	}
}
