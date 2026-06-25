using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings
{
	// Token: 0x02000BC0 RID: 3008
	public class PracticeRoomPuppetTemplate : MonoBehaviour
	{
		// Token: 0x06009786 RID: 38790 RVA: 0x004695D8 File Offset: 0x004677D8
		public void Set(short templateId)
		{
			bool isEmpty = templateId == -1;
			this.content.SetActive(!isEmpty);
			this.empty.SetActive(isEmpty);
			bool flag = isEmpty;
			if (!flag)
			{
				PuppetItem config = Puppet.Instance[templateId];
				ResLoader.LoadModOrGameResource<Sprite>("NpcFace/NormalFace/" + config.Avatar, delegate(Sprite sprite)
				{
					this.puppetIcon.sprite = sprite;
				}, null);
				this.puppetName.text = config.Name;
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
		}

		// Token: 0x06009787 RID: 38791 RVA: 0x004696AF File Offset: 0x004678AF
		public void SetSelected(bool value)
		{
			this.selected.SetActive(value);
		}

		// Token: 0x06009788 RID: 38792 RVA: 0x004696BF File Offset: 0x004678BF
		public void SetHover(bool value)
		{
			this.hover.SetActive(value);
		}

		// Token: 0x0400742E RID: 29742
		public CImage puppetIcon;

		// Token: 0x0400742F RID: 29743
		public TextMeshProUGUI puppetName;

		// Token: 0x04007430 RID: 29744
		public PointerTrigger pointerTrigger;

		// Token: 0x04007431 RID: 29745
		public GameObject selected;

		// Token: 0x04007432 RID: 29746
		public GameObject hover;

		// Token: 0x04007433 RID: 29747
		public CButton button;

		// Token: 0x04007434 RID: 29748
		public GameObject content;

		// Token: 0x04007435 RID: 29749
		public GameObject empty;
	}
}
