using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB4 RID: 2740
	public class CricketCombatRewardCharacterContent : ITradeableContent, ISerializableGameData
	{
		// Token: 0x0600866C RID: 34412 RVA: 0x003E8EE8 File Offset: 0x003E70E8
		public CricketCombatRewardCharacterContent(CharacterDisplayData displayData)
		{
			this._displayData = displayData;
		}

		// Token: 0x17000EC4 RID: 3780
		// (get) Token: 0x0600866D RID: 34413 RVA: 0x003E8F00 File Offset: 0x003E7100
		public CharacterDisplayData DisplayData
		{
			get
			{
				return this._displayData;
			}
		}

		// Token: 0x17000EC5 RID: 3781
		// (get) Token: 0x0600866E RID: 34414 RVA: 0x003E8F08 File Offset: 0x003E7108
		// (set) Token: 0x0600866F RID: 34415 RVA: 0x003E8F10 File Offset: 0x003E7110
		public bool Interactable
		{
			get
			{
				return this._interactable;
			}
			set
			{
				this._interactable = value;
			}
		}

		// Token: 0x17000EC6 RID: 3782
		// (get) Token: 0x06008670 RID: 34416 RVA: 0x003E8F1C File Offset: 0x003E711C
		// (set) Token: 0x06008671 RID: 34417 RVA: 0x003E8F7E File Offset: 0x003E717E
		public long Value
		{
			get
			{
				return (long)((int)Math.Clamp(Wager.CharacterValue((int)this._displayData.FameType, (int)this._displayData.Charm, (int)this._displayData.OrgInfo.Grade, (int)this._displayData.Gender, (int)this._displayData.AvatarRelatedData.DisplayAge), 0L, 2147483647L));
			}
			set
			{
			}
		}

		// Token: 0x17000EC7 RID: 3783
		// (get) Token: 0x06008672 RID: 34418 RVA: 0x003E8F81 File Offset: 0x003E7181
		// (set) Token: 0x06008673 RID: 34419 RVA: 0x003E8F84 File Offset: 0x003E7184
		public int Amount
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		// Token: 0x17000EC8 RID: 3784
		// (get) Token: 0x06008674 RID: 34420 RVA: 0x003E8F87 File Offset: 0x003E7187
		public ItemKey Key
		{
			get
			{
				return ItemKey.Invalid;
			}
		}

		// Token: 0x17000EC9 RID: 3785
		// (get) Token: 0x06008675 RID: 34421 RVA: 0x003E8F8E File Offset: 0x003E718E
		public ItemKey RealKey
		{
			get
			{
				return ItemKey.Invalid;
			}
		}

		// Token: 0x17000ECA RID: 3786
		// (get) Token: 0x06008676 RID: 34422 RVA: 0x003E8F95 File Offset: 0x003E7195
		public sbyte Grade
		{
			get
			{
				return this._displayData.OrgInfo.Grade;
			}
		}

		// Token: 0x17000ECB RID: 3787
		// (get) Token: 0x06008677 RID: 34423 RVA: 0x003E8FA7 File Offset: 0x003E71A7
		public sbyte Gender
		{
			get
			{
				return this._displayData.Gender;
			}
		}

		// Token: 0x17000ECC RID: 3788
		// (get) Token: 0x06008678 RID: 34424 RVA: 0x003E8FB4 File Offset: 0x003E71B4
		public int CharacterId
		{
			get
			{
				return this._displayData.CharacterId;
			}
		}

		// Token: 0x17000ECD RID: 3789
		// (get) Token: 0x06008679 RID: 34425 RVA: 0x003E8FC1 File Offset: 0x003E71C1
		public OrganizationInfo OrganizationInfo
		{
			get
			{
				return this._displayData.OrgInfo;
			}
		}

		// Token: 0x17000ECE RID: 3790
		// (get) Token: 0x0600867A RID: 34426 RVA: 0x003E8FCE File Offset: 0x003E71CE
		public NameRelatedData NameRelatedData
		{
			get
			{
				return NameCenter.GetNameRelatedData(this._displayData);
			}
		}

		// Token: 0x17000ECF RID: 3791
		// (get) Token: 0x0600867B RID: 34427 RVA: 0x003E8FDB File Offset: 0x003E71DB
		public AvatarRelatedData AvatarRelatedData
		{
			get
			{
				return this._displayData.AvatarRelatedData;
			}
		}

		// Token: 0x0600867C RID: 34428 RVA: 0x003E8FE8 File Offset: 0x003E71E8
		public ITradeableContent Clone(int amount = -1)
		{
			return new CricketCombatRewardCharacterContent(this._displayData)
			{
				Interactable = this.Interactable
			};
		}

		// Token: 0x0600867D RID: 34429 RVA: 0x003E9012 File Offset: 0x003E7212
		public unsafe int Deserialize(byte* pData)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600867E RID: 34430 RVA: 0x003E901C File Offset: 0x003E721C
		public sbyte GetContentType()
		{
			return -1;
		}

		// Token: 0x0600867F RID: 34431 RVA: 0x003E902F File Offset: 0x003E722F
		public int GetSerializedSize()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06008680 RID: 34432 RVA: 0x003E9037 File Offset: 0x003E7237
		public bool IsSerializedSizeFixed()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06008681 RID: 34433 RVA: 0x003E903F File Offset: 0x003E723F
		public unsafe int Serialize(byte* pData)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04006744 RID: 26436
		private readonly CharacterDisplayData _displayData;

		// Token: 0x04006745 RID: 26437
		private bool _interactable = true;
	}
}
