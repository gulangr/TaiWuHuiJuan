using System;
using FrameWork;
using GameData.Domains.World;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000872 RID: 2162
	public class MouseTipPracticeNotice : MouseTipBase
	{
		// Token: 0x0600682B RID: 26667 RVA: 0x002F9F7D File Offset: 0x002F817D
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x0600682C RID: 26668 RVA: 0x002F9F88 File Offset: 0x002F8188
		public override void Refresh(ArgumentBox argsBox)
		{
			WorldStateData worldStateData;
			bool flag = argsBox.Get<WorldStateData>("WorldStateData", out worldStateData);
			if (flag)
			{
				this.practiceNoticeItem.Set(worldStateData);
			}
		}

		// Token: 0x040049BB RID: 18875
		[SerializeField]
		private PracticeNoticeItem practiceNoticeItem;
	}
}
