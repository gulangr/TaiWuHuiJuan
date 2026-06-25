using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F51 RID: 3921
	public class LifeRecordTitle : RecordTitle
	{
		// Token: 0x0600B3C6 RID: 46022 RVA: 0x0051D271 File Offset: 0x0051B471
		public override void Set(string text)
		{
			this.layoutElement.preferredWidth = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnWidth : this.defaultWidth);
			base.Set(text);
		}

		// Token: 0x04008BD3 RID: 35795
		[SerializeField]
		private float cnWidth = 218f;

		// Token: 0x04008BD4 RID: 35796
		[SerializeField]
		private float defaultWidth = 450f;

		// Token: 0x04008BD5 RID: 35797
		[SerializeField]
		private LayoutElement layoutElement;
	}
}
