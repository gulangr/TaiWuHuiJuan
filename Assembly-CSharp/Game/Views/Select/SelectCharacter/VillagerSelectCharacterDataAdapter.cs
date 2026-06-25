using System;
using GameData.Domains.Character.Display;

namespace Game.Views.Select.SelectCharacter
{
	// Token: 0x020007B5 RID: 1973
	public class VillagerSelectCharacterDataAdapter : IVillagerSelectCharacterData, ISelectCharacterData
	{
		// Token: 0x06006014 RID: 24596 RVA: 0x002C15BC File Offset: 0x002BF7BC
		public VillagerSelectCharacterDataAdapter(VillagerSelectCharacterDisplayData data)
		{
			this._data = data;
		}

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x06006015 RID: 24597 RVA: 0x002C15D0 File Offset: 0x002BF7D0
		public int CharacterId
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList mainData = data.MainData;
					num = ((mainData != null) ? new int?(mainData.CharacterId) : null);
				}
				return num ?? -1;
			}
		}

		// Token: 0x06006016 RID: 24598 RVA: 0x002C1624 File Offset: 0x002BF824
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			VillagerSelectCharacterDisplayData data = this._data;
			return (data != null) ? data.MainData : null;
		}

		// Token: 0x06006017 RID: 24599 RVA: 0x002C1648 File Offset: 0x002BF848
		public VillagerSelectCharacterDisplayData GetRawData()
		{
			return this._data;
		}

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x06006018 RID: 24600 RVA: 0x002C1660 File Offset: 0x002BF860
		public sbyte WorkType
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (data != null) ? data.WorkType : -1;
			}
		}

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x06006019 RID: 24601 RVA: 0x002C1674 File Offset: 0x002BF874
		public byte WorkStatus
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (data != null) ? data.WorkStatus : 0;
			}
		}

		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x0600601A RID: 24602 RVA: 0x002C1688 File Offset: 0x002BF888
		public int ArrangementTemplateId
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (data != null) ? data.ArrangementTemplateId : -1;
			}
		}

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x0600601B RID: 24603 RVA: 0x002C169C File Offset: 0x002BF89C
		public int BuildingBlockTemplateId
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (data != null) ? data.BuildingBlockTemplateId : -1;
			}
		}

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x0600601C RID: 24604 RVA: 0x002C16B0 File Offset: 0x002BF8B0
		public bool IsBuyOperation
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return data != null && data.IsBuyOperation;
			}
		}

		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x0600601D RID: 24605 RVA: 0x002C16C4 File Offset: 0x002BF8C4
		public int GraveId
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (data != null) ? data.GraveId : -1;
			}
		}

		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x0600601E RID: 24606 RVA: 0x002C16D8 File Offset: 0x002BF8D8
		public int SwordTombId
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (data != null) ? data.SwordTombId : -1;
			}
		}

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x0600601F RID: 24607 RVA: 0x002C16EC File Offset: 0x002BF8EC
		public int RoleTemplateId
		{
			get
			{
				VillagerSelectCharacterDisplayData data = this._data;
				return (int)((data != null) ? data.RoleTemplateId : -1);
			}
		}

		// Token: 0x04004298 RID: 17048
		private readonly VillagerSelectCharacterDisplayData _data;
	}
}
