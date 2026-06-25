using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom.Migrate
{
	// Token: 0x02000C5D RID: 3165
	public class BottomProfessionExtraSkill : MonoBehaviour
	{
		// Token: 0x170010EC RID: 4332
		// (get) Token: 0x0600A160 RID: 41312 RVA: 0x004B6B32 File Offset: 0x004B4D32
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x0600A161 RID: 41313 RVA: 0x004B6B3A File Offset: 0x004B4D3A
		public void Refresh(bool isReady, Action onClick)
		{
			base.gameObject.SetActive(true);
			this.button.interactable = isReady;
			this.button.ClearAndAddListener(onClick);
			this.coolDown.SetActive(!isReady);
		}

		// Token: 0x0600A162 RID: 41314 RVA: 0x004B6B74 File Offset: 0x004B4D74
		public void SetCoolDownText(string text)
		{
			bool flag = this.cdLabel != null;
			if (flag)
			{
				this.cdLabel.text = text;
			}
		}

		// Token: 0x0600A163 RID: 41315 RVA: 0x004B6B9F File Offset: 0x004B4D9F
		public void RefreshTip()
		{
			this.tip.Type = TipType.ExtraProfessionSkill;
			this.tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}

		// Token: 0x04007D21 RID: 32033
		[SerializeField]
		private CButton button;

		// Token: 0x04007D22 RID: 32034
		[SerializeField]
		private GameObject coolDown;

		// Token: 0x04007D23 RID: 32035
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04007D24 RID: 32036
		[SerializeField]
		private TextMeshProUGUI cdLabel;
	}
}
