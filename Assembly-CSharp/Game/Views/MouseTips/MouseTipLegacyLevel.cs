using System;
using FrameWork;
using Game.Components.Common;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200086A RID: 2154
	public class MouseTipLegacyLevel : MouseTipBase
	{
		// Token: 0x17000C74 RID: 3188
		// (get) Token: 0x060067F7 RID: 26615 RVA: 0x002F7FA0 File Offset: 0x002F61A0
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060067F8 RID: 26616 RVA: 0x002F7FA3 File Offset: 0x002F61A3
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			this._showDetail = false;
			this.detail.gameObject.SetActive(false);
			this.detailHotkey.Refresh(EHotKeyDisplayType.Detail);
		}

		// Token: 0x060067F9 RID: 26617 RVA: 0x002F7FDC File Offset: 0x002F61DC
		private void Update()
		{
			bool flag = this.Element == null;
			if (!flag)
			{
				bool flag2 = this._showDetail == CommonCommandKit.Alt.Check(this.Element, true, false, false, true, false);
				if (!flag2)
				{
					bool flag3 = this._showDetail = !this._showDetail;
					if (flag3)
					{
						this.detail.gameObject.SetActive(true);
						this.detailHotkey.Refresh(EHotKeyDisplayType.CancelDetail);
					}
					else
					{
						this.detail.gameObject.SetActive(false);
						this.detailHotkey.Refresh(EHotKeyDisplayType.Detail);
					}
					base.SetAllowOverlapLayout(this._showDetail);
				}
			}
		}

		// Token: 0x0400496E RID: 18798
		[SerializeField]
		private TMPTextSpriteHelper desc;

		// Token: 0x0400496F RID: 18799
		[SerializeField]
		private TMPTextSpriteHelper contentText;

		// Token: 0x04004970 RID: 18800
		[SerializeField]
		private TextMeshProUGUI moreInfoTitle;

		// Token: 0x04004971 RID: 18801
		[SerializeField]
		private TextMeshProUGUI[] levelRangeTexts;

		// Token: 0x04004972 RID: 18802
		[SerializeField]
		private GameObject detail;

		// Token: 0x04004973 RID: 18803
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay detailHotkey;

		// Token: 0x04004974 RID: 18804
		private bool _showDetail;
	}
}
