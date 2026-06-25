using System;
using GameData.Domains.Character.Display;

namespace Game.Views.CharacterMenu.Kidnap
{
	// Token: 0x02000BB8 RID: 3000
	public class RopeCellData
	{
		// Token: 0x040073DD RID: 29661
		public KidnapCharDisplayData KidnapCharDisplayData;

		// Token: 0x040073DE RID: 29662
		public int KidnapperCharId;

		// Token: 0x040073DF RID: 29663
		public bool CanOperate;

		// Token: 0x040073E0 RID: 29664
		public bool CurrentCharacterIsTaiwu;

		// Token: 0x040073E1 RID: 29665
		public Action OnRopeChanged;
	}
}
