using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x0200079A RID: 1946
	public class BasicSelectCharacterDataAdapter : ISelectCharacterData
	{
		// Token: 0x06005E44 RID: 24132 RVA: 0x002B5099 File Offset: 0x002B3299
		public BasicSelectCharacterDataAdapter(CharacterDisplayDataForGeneralScrollList data)
		{
			this._data = data;
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x06005E45 RID: 24133 RVA: 0x002B50AA File Offset: 0x002B32AA
		public int CharacterId
		{
			get
			{
				CharacterDisplayDataForGeneralScrollList data = this._data;
				return (data != null) ? data.CharacterId : -1;
			}
		}

		// Token: 0x06005E46 RID: 24134 RVA: 0x002B50C0 File Offset: 0x002B32C0
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			return this._data;
		}

		// Token: 0x0400413D RID: 16701
		private readonly CharacterDisplayDataForGeneralScrollList _data;
	}
}
