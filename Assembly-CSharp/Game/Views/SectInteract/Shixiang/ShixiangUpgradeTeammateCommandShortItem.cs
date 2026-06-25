using System;
using Game.Components.Character;
using UnityEngine;

namespace Game.Views.SectInteract.Shixiang
{
	// Token: 0x020009E4 RID: 2532
	public class ShixiangUpgradeTeammateCommandShortItem : MonoBehaviour
	{
		// Token: 0x06007C2C RID: 31788 RVA: 0x0039BC47 File Offset: 0x00399E47
		public void Set(ViewShixiangUpgradeTeammateCommand parent, int charId, sbyte commandId)
		{
			this._parent = parent;
			this._charId = charId;
			this._commandId = commandId;
			this.teammateCommand.Set((int)this._commandId, true);
		}

		// Token: 0x04005E69 RID: 24169
		public TeammateCommand teammateCommand;

		// Token: 0x04005E6A RID: 24170
		private ViewShixiangUpgradeTeammateCommand _parent;

		// Token: 0x04005E6B RID: 24171
		private int _charId;

		// Token: 0x04005E6C RID: 24172
		private sbyte _commandId;
	}
}
