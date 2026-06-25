using System;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000967 RID: 2407
	public class ReadingBriefStrategy : MonoBehaviour
	{
		// Token: 0x0600736D RID: 29549 RVA: 0x00359F00 File Offset: 0x00358100
		public void SetStrategy(bool value)
		{
			this.none.SetActive(!value);
			this.active.SetActive(value);
		}

		// Token: 0x0600736E RID: 29550 RVA: 0x00359F20 File Offset: 0x00358120
		public void SetMonth(bool isShow)
		{
			this.monthBg.SetActive(isShow);
		}

		// Token: 0x0600736F RID: 29551 RVA: 0x00359F30 File Offset: 0x00358130
		public void SetTip(string name, string desc)
		{
			bool flag = this.tipDisplayer == null;
			if (!flag)
			{
				bool flag2 = this.tipDisplayer.RuntimeParam != null;
				if (flag2)
				{
					EasyPool.Free<ArgumentBox>(this.tipDisplayer.RuntimeParam);
					this.tipDisplayer.RuntimeParam = null;
				}
				this.tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", name).Set("arg1", desc);
			}
		}

		// Token: 0x06007370 RID: 29552 RVA: 0x00359FAC File Offset: 0x003581AC
		public void ClearTip()
		{
			bool flag = this.tipDisplayer == null;
			if (!flag)
			{
				bool flag2 = this.tipDisplayer.RuntimeParam != null;
				if (flag2)
				{
					EasyPool.Free<ArgumentBox>(this.tipDisplayer.RuntimeParam);
					this.tipDisplayer.RuntimeParam = null;
				}
			}
		}

		// Token: 0x040055C4 RID: 21956
		public GameObject none;

		// Token: 0x040055C5 RID: 21957
		public GameObject active;

		// Token: 0x040055C6 RID: 21958
		public TextMeshProUGUI strategy;

		// Token: 0x040055C7 RID: 21959
		public TooltipInvoker tipDisplayer;

		// Token: 0x040055C8 RID: 21960
		public GameObject monthBg;

		// Token: 0x040055C9 RID: 21961
		public TextMeshProUGUI monthText;
	}
}
