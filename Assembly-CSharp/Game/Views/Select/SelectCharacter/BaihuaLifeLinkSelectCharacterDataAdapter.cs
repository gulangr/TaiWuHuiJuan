using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select.SelectCharacter
{
	// Token: 0x020007B4 RID: 1972
	public class BaihuaLifeLinkSelectCharacterDataAdapter : ITaiwuSelectCharacterData, ISelectCharacterData
	{
		// Token: 0x0600600F RID: 24591 RVA: 0x002C1504 File Offset: 0x002BF704
		public BaihuaLifeLinkSelectCharacterDataAdapter(CharacterDisplayDataForLifeLink data)
		{
			this._data = data;
		}

		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x06006010 RID: 24592 RVA: 0x002C1518 File Offset: 0x002BF718
		public int CharacterId
		{
			get
			{
				CharacterDisplayDataForLifeLink data = this._data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList listData = data.ListData;
					num = ((listData != null) ? new int?(listData.CharacterId) : null);
				}
				return num ?? -1;
			}
		}

		// Token: 0x06006011 RID: 24593 RVA: 0x002C156C File Offset: 0x002BF76C
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			CharacterDisplayDataForLifeLink data = this._data;
			return (data != null) ? data.ListData : null;
		}

		// Token: 0x06006012 RID: 24594 RVA: 0x002C1590 File Offset: 0x002BF790
		public CharacterDisplayDataForLifeLink GetRawData()
		{
			return this._data;
		}

		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x06006013 RID: 24595 RVA: 0x002C15A8 File Offset: 0x002BF7A8
		public bool IsTaiwuTeammate
		{
			get
			{
				CharacterDisplayDataForLifeLink data = this._data;
				return data != null && data.IsTeammate;
			}
		}

		// Token: 0x04004297 RID: 17047
		private readonly CharacterDisplayDataForLifeLink _data;
	}
}
