using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x0200079E RID: 1950
	public class TasterUltimateCharacterDataAdapter : ISelectCharacterData, ITaiwuSelectCharacterData
	{
		// Token: 0x06005E51 RID: 24145 RVA: 0x002B52C8 File Offset: 0x002B34C8
		public TasterUltimateCharacterDataAdapter(CharacterDisplayDataForTasterUltimate data)
		{
			this.Data = data;
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x06005E52 RID: 24146 RVA: 0x002B52DC File Offset: 0x002B34DC
		public int CharacterId
		{
			get
			{
				CharacterDisplayDataForTasterUltimate data = this.Data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList characterData = data.CharacterData;
					num = ((characterData != null) ? new int?(characterData.CharacterId) : null);
				}
				return num ?? -1;
			}
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x002B5330 File Offset: 0x002B3530
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			CharacterDisplayDataForTasterUltimate data = this.Data;
			return (data != null) ? data.CharacterData : null;
		}

		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x06005E54 RID: 24148 RVA: 0x002B5354 File Offset: 0x002B3554
		public bool IsTaiwuTeammate
		{
			get
			{
				CharacterDisplayDataForTasterUltimate data = this.Data;
				bool? flag;
				if (data == null)
				{
					flag = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList characterData = data.CharacterData;
					flag = ((characterData != null) ? new bool?(characterData.IsTaiwuTeammate) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		// Token: 0x04004142 RID: 16706
		public readonly CharacterDisplayDataForTasterUltimate Data;
	}
}
