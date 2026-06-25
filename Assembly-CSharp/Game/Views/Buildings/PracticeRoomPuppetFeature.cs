using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using UnityEngine;

namespace Game.Views.Buildings
{
	// Token: 0x02000BBE RID: 3006
	public class PracticeRoomPuppetFeature : MonoBehaviour
	{
		// Token: 0x0600977B RID: 38779 RVA: 0x0046902C File Offset: 0x0046722C
		public void Set(short templateId)
		{
			bool flag = templateId >= 0;
			if (flag)
			{
				this.feature.Set(templateId, -1, false, -1);
				this.feature.SetTipEnabled(true);
				this.feature.gameObject.SetActive(true);
				this.emptyContent.gameObject.SetActive(false);
				base.GetComponent<CButton>().interactable = true;
				this.pointerTrigger.enabled = true;
			}
			else
			{
				this.feature.SetTipEnabled(false);
				this.feature.gameObject.SetActive(false);
				this.emptyContent.gameObject.SetActive(true);
				base.GetComponent<CButton>().interactable = false;
				this.pointerTrigger.enabled = false;
			}
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.SetHover(true);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.SetHover(false);
			});
		}

		// Token: 0x0600977C RID: 38780 RVA: 0x0046914D File Offset: 0x0046734D
		public void SetSelected(bool value)
		{
			this.selected.SetActive(value);
		}

		// Token: 0x0600977D RID: 38781 RVA: 0x0046915D File Offset: 0x0046735D
		public void SetHover(bool value)
		{
			this.hover.SetActive(value);
		}

		// Token: 0x04007422 RID: 29730
		public Feature feature;

		// Token: 0x04007423 RID: 29731
		public GameObject emptyContent;

		// Token: 0x04007424 RID: 29732
		public GameObject selected;

		// Token: 0x04007425 RID: 29733
		public GameObject hover;

		// Token: 0x04007426 RID: 29734
		public PointerTrigger pointerTrigger;
	}
}
