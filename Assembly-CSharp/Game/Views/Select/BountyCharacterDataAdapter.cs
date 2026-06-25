using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x0200079C RID: 1948
	public class BountyCharacterDataAdapter : ISelectCharacterData
	{
		// Token: 0x06005E4A RID: 24138 RVA: 0x002B5164 File Offset: 0x002B3364
		public BountyCharacterDataAdapter(CharacterDisplayDataForSettlementBounty data, sbyte currentStateId)
		{
			this.Data = data;
			this.CurrentStateTemplateId = currentStateId;
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06005E4B RID: 24139 RVA: 0x002B517C File Offset: 0x002B337C
		public int CharacterId
		{
			get
			{
				CharacterDisplayDataForSettlementBounty data = this.Data;
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

		// Token: 0x06005E4C RID: 24140 RVA: 0x002B51D0 File Offset: 0x002B33D0
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			CharacterDisplayDataForSettlementBounty data = this.Data;
			return (data != null) ? data.CharacterDisplayDataForGeneralScrollList : null;
		}

		// Token: 0x0400413F RID: 16703
		public readonly CharacterDisplayDataForSettlementBounty Data;

		// Token: 0x04004140 RID: 16704
		public sbyte CurrentStateTemplateId;
	}
}
