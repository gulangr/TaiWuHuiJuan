using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x0200079B RID: 1947
	public class YuanshanSelectCharacterDataAdapter : ISelectCharacterData
	{
		// Token: 0x06005E47 RID: 24135 RVA: 0x002B50D8 File Offset: 0x002B32D8
		public YuanshanSelectCharacterDataAdapter(CharacterDisplayDataForYuanshanSelect data)
		{
			this.Data = data;
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x06005E48 RID: 24136 RVA: 0x002B50EC File Offset: 0x002B32EC
		public int CharacterId
		{
			get
			{
				CharacterDisplayDataForYuanshanSelect data = this.Data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList generalData = data.GeneralData;
					num = ((generalData != null) ? new int?(generalData.CharacterId) : null);
				}
				return num ?? -1;
			}
		}

		// Token: 0x06005E49 RID: 24137 RVA: 0x002B5140 File Offset: 0x002B3340
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			CharacterDisplayDataForYuanshanSelect data = this.Data;
			return (data != null) ? data.GeneralData : null;
		}

		// Token: 0x0400413E RID: 16702
		public readonly CharacterDisplayDataForYuanshanSelect Data;
	}
}
