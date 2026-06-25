using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F97 RID: 3991
	public class QuickEncyclopedia : MonoBehaviour
	{
		// Token: 0x0600B78A RID: 46986 RVA: 0x0053A1E5 File Offset: 0x005383E5
		private void Awake()
		{
			this._button = base.GetComponentInChildren<CButton>(true);
			this._button.ClearAndAddListener(new Action(this.EnterEncyclopedia));
		}

		// Token: 0x0600B78B RID: 46987 RVA: 0x0053A20D File Offset: 0x0053840D
		private void EnterEncyclopedia()
		{
			ViewEncyclopediaPanel.OpenLink(this.GetEncyclopediaTipLinkItem(this.encyclopediaLink));
		}

		// Token: 0x0600B78C RID: 46988 RVA: 0x0053A224 File Offset: 0x00538424
		private EncyclopediaTipLinkItem GetEncyclopediaTipLinkItem(EEncyclopediaTipLinkType linkType)
		{
			return EncyclopediaTipLink.Instance[(int)linkType];
		}

		// Token: 0x04008E85 RID: 36485
		public EEncyclopediaTipLinkType encyclopediaLink;

		// Token: 0x04008E86 RID: 36486
		private CButton _button;
	}
}
