using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EF7 RID: 3831
	public class InformationTypeToggleGroup : MonoBehaviour
	{
		// Token: 0x0600B07A RID: 45178 RVA: 0x00506CFC File Offset: 0x00504EFC
		public void Init(Action onToggleChange)
		{
			this.toggleGroup.Init(-1);
			this.toggleGroup.OnActiveIndexChange += delegate(int _, int _)
			{
				onToggleChange();
			};
			foreach (InformationTypeItem config in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
			{
				bool flag = (int)config.TemplateId >= this.toggleGroup.Count();
				if (flag)
				{
					this.toggleGroup.Add(Object.Instantiate<CToggle>(this.toggleGroup.Get(0), this.toggleGroup.transform));
				}
				this.toggleGroup.gameObject.SetActive(config.InUse);
			}
			this.toggleGroup.Get(this.toggleGroup.Count() - 1).transform.GetChild(0).gameObject.SetActive(false);
		}

		// Token: 0x0600B07B RID: 45179 RVA: 0x00506E00 File Offset: 0x00505000
		public void SetVisible(sbyte type, bool value)
		{
			this.toggleGroup.Get((int)type).gameObject.SetActive(value && InformationType.Instance[type].InUse);
		}

		// Token: 0x0600B07C RID: 45180 RVA: 0x00506E30 File Offset: 0x00505030
		public void Set(sbyte type, int count)
		{
			this.toggleGroup.Get((int)type).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format(InformationType.Instance[type].Title, count);
		}

		// Token: 0x0600B07D RID: 45181 RVA: 0x00506E70 File Offset: 0x00505070
		public int Get()
		{
			return this.toggleGroup.GetActiveIndex();
		}

		// Token: 0x04008873 RID: 34931
		public CToggleGroup toggleGroup;
	}
}
