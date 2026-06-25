using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F5E RID: 3934
	public class RecordSimple : MonoBehaviour, ILifeRecord
	{
		// Token: 0x17001472 RID: 5234
		// (get) Token: 0x0600B40A RID: 46090 RVA: 0x0051E881 File Offset: 0x0051CA81
		// (set) Token: 0x0600B40B RID: 46091 RVA: 0x0051E889 File Offset: 0x0051CA89
		public int CurrDate { get; set; }

		// Token: 0x0600B40C RID: 46092 RVA: 0x0051E892 File Offset: 0x0051CA92
		public void Init(Action<int, bool, bool> triggerByDateSelector)
		{
			this._action = triggerByDateSelector;
		}

		// Token: 0x0600B40D RID: 46093 RVA: 0x0051E89C File Offset: 0x0051CA9C
		public void Set(int value)
		{
			this.CurrDate = value;
		}

		// Token: 0x0600B40E RID: 46094 RVA: 0x0051E8A7 File Offset: 0x0051CAA7
		public void ScrollToMonth(int month, bool smooth, bool requestFill, ESelectDateDirection direction = ESelectDateDirection.SelectDefault)
		{
			this._action(month, smooth, requestFill);
		}

		// Token: 0x04008C09 RID: 35849
		[SerializeField]
		protected CScrollRect scrollRect;

		// Token: 0x04008C0B RID: 35851
		private Action<int, bool, bool> _action;
	}
}
