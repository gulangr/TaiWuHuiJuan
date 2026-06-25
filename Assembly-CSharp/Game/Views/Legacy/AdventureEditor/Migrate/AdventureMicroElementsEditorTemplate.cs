using System;
using UnityEngine;

namespace Game.Views.Legacy.AdventureEditor.Migrate
{
	// Token: 0x02000A0D RID: 2573
	public class AdventureMicroElementsEditorTemplate : MonoBehaviour
	{
		// Token: 0x06007DDB RID: 32219 RVA: 0x003A6A08 File Offset: 0x003A4C08
		private void Awake()
		{
			bool flag = this.icon != null;
			if (flag)
			{
				this._defaultMaterial = this.icon.material;
			}
		}

		// Token: 0x06007DDC RID: 32220 RVA: 0x003A6A38 File Offset: 0x003A4C38
		public void RestoreDefaultMaterial()
		{
			bool flag = this.icon != null && this._defaultMaterial != null;
			if (flag)
			{
				this.icon.material = this._defaultMaterial;
			}
		}

		// Token: 0x04006005 RID: 24581
		public CImage icon;

		// Token: 0x04006006 RID: 24582
		private Material _defaultMaterial;
	}
}
