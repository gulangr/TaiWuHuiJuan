using System;
using Config;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.DLCIntroduce
{
	// Token: 0x02000A97 RID: 2711
	public class DlCIntroduceScrollItem : MonoBehaviour
	{
		// Token: 0x17000E88 RID: 3720
		// (get) Token: 0x060084A8 RID: 33960 RVA: 0x003DB3A4 File Offset: 0x003D95A4
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x17000E89 RID: 3721
		// (get) Token: 0x060084A9 RID: 33961 RVA: 0x003DB3AC File Offset: 0x003D95AC
		public PointerTrigger PointerTrigger
		{
			get
			{
				return this.pointerTrigger;
			}
		}

		// Token: 0x060084AA RID: 33962 RVA: 0x003DB3B4 File Offset: 0x003D95B4
		public void Set(ImplementedDlcItem dlcItem)
		{
			this.icon.SetSprite(dlcItem.ScrollIcon, false, null);
			this.typeIcon.SetSprite("ui9_icon_dlc_introduce_tag_type_" + ((int)dlcItem.Type).ToString(), false, null);
			this.freeFlag.gameObject.SetActive(dlcItem.IsFree);
		}

		// Token: 0x040065B8 RID: 26040
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040065B9 RID: 26041
		[SerializeField]
		private CImage icon;

		// Token: 0x040065BA RID: 26042
		[SerializeField]
		private CImage typeIcon;

		// Token: 0x040065BB RID: 26043
		[SerializeField]
		private CImage freeFlag;

		// Token: 0x040065BC RID: 26044
		[SerializeField]
		private PointerTrigger pointerTrigger;
	}
}
