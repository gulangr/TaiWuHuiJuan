using System;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000868 RID: 2152
	public class MouseTipKongSangDing : MouseTipBase
	{
		// Token: 0x17000C72 RID: 3186
		// (get) Token: 0x060067E9 RID: 26601 RVA: 0x002F793C File Offset: 0x002F5B3C
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060067EA RID: 26602 RVA: 0x002F793F File Offset: 0x002F5B3F
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			this.Refresh(argsBox);
		}

		// Token: 0x060067EB RID: 26603 RVA: 0x002F7958 File Offset: 0x002F5B58
		public override void Refresh(ArgumentBox argsBox)
		{
			string arg0;
			argsBox.Get("arg0", out arg0);
			string arg;
			argsBox.Get("arg1", out arg);
			SettlementDisplayData[] settlements;
			argsBox.Get<SettlementDisplayData[]>("arg2", out settlements);
			this.title.text = arg0;
			this.desc.text = arg.ColorReplace();
			bool flag = settlements != null && settlements.Length > 0;
			if (flag)
			{
				this.points.gameObject.SetActive(true);
				this.points.Rebuild<ImagesAndTexts>(settlements.Length, delegate(ImagesAndTexts t, int i)
				{
					SettlementDisplayData settlement = settlements[i];
					t.texts[0].text = settlement.SettlementNameRelatedData.GetName();
					t.texts[1].text = settlement.DarkAshStatus.Tr().ColorReplace();
				});
			}
			else
			{
				this.points.gameObject.SetActive(false);
			}
		}

		// Token: 0x0400495F RID: 18783
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004960 RID: 18784
		[SerializeField]
		private TMP_Text desc;

		// Token: 0x04004961 RID: 18785
		[SerializeField]
		private TemplatedContainerAssemblyNew points;
	}
}
