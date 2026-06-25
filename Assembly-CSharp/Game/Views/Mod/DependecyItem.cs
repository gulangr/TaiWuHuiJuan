using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008C7 RID: 2247
	public class DependecyItem : MonoBehaviour
	{
		// Token: 0x06006B3D RID: 27453 RVA: 0x00318EAE File Offset: 0x003170AE
		private void Awake()
		{
			this.jumpBtn.ClearAndAddListener(delegate
			{
				Action jumpAction = this._jumpAction;
				if (jumpAction != null)
				{
					jumpAction();
				}
			});
		}

		// Token: 0x06006B3E RID: 27454 RVA: 0x00318EC9 File Offset: 0x003170C9
		public void Set(bool isInstalled, string modName, Action action)
		{
			this.statusIcon.sprite = this.sprites[isInstalled ? 1 : 0];
			this.modNameLabel.text = modName;
			this._jumpAction = action;
		}

		// Token: 0x06006B3F RID: 27455 RVA: 0x00318EFA File Offset: 0x003170FA
		public void Set(string modName, Action action)
		{
			this.statusIcon.gameObject.SetActive(false);
			this.modNameLabel.text = modName;
			this._jumpAction = action;
		}

		// Token: 0x04004DDB RID: 19931
		[SerializeField]
		private CImage statusIcon;

		// Token: 0x04004DDC RID: 19932
		[SerializeField]
		private TextMeshProUGUI modNameLabel;

		// Token: 0x04004DDD RID: 19933
		[SerializeField]
		private CButton jumpBtn;

		// Token: 0x04004DDE RID: 19934
		[SerializeField]
		private Sprite[] sprites;

		// Token: 0x04004DDF RID: 19935
		private Action _jumpAction;
	}
}
