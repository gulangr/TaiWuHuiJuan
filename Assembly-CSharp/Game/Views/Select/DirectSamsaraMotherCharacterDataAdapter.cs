using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x0200079D RID: 1949
	public class DirectSamsaraMotherCharacterDataAdapter : ISelectCharacterData, ITaiwuSelectCharacterData
	{
		// Token: 0x06005E4D RID: 24141 RVA: 0x002B51F4 File Offset: 0x002B33F4
		public DirectSamsaraMotherCharacterDataAdapter(CharacterDisplayDataForDirectSamsaraMother data)
		{
			this.Data = data;
		}

		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06005E4E RID: 24142 RVA: 0x002B5208 File Offset: 0x002B3408
		public int CharacterId
		{
			get
			{
				CharacterDisplayDataForDirectSamsaraMother data = this.Data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList characterDisplayDataForGeneralScrollList = data.CharacterDisplayDataForGeneralScrollList;
					num = ((characterDisplayDataForGeneralScrollList != null) ? new int?(characterDisplayDataForGeneralScrollList.CharacterId) : null);
				}
				return num ?? -1;
			}
		}

		// Token: 0x06005E4F RID: 24143 RVA: 0x002B525C File Offset: 0x002B345C
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			CharacterDisplayDataForDirectSamsaraMother data = this.Data;
			return (data != null) ? data.CharacterDisplayDataForGeneralScrollList : null;
		}

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x06005E50 RID: 24144 RVA: 0x002B5280 File Offset: 0x002B3480
		public bool IsTaiwuTeammate
		{
			get
			{
				CharacterDisplayDataForDirectSamsaraMother data = this.Data;
				bool? flag;
				if (data == null)
				{
					flag = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList characterDisplayDataForGeneralScrollList = data.CharacterDisplayDataForGeneralScrollList;
					flag = ((characterDisplayDataForGeneralScrollList != null) ? new bool?(characterDisplayDataForGeneralScrollList.IsTaiwuTeammate) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		// Token: 0x04004141 RID: 16705
		public readonly CharacterDisplayDataForDirectSamsaraMother Data;
	}
}
