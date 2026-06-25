using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Team
{
	// Token: 0x02000CD5 RID: 3285
	public sealed class GroupCharDisplaySelectAdapter : ISelectCharacterData
	{
		// Token: 0x0600A609 RID: 42505 RVA: 0x004D49B4 File Offset: 0x004D2BB4
		public GroupCharDisplaySelectAdapter(GroupCharDisplayData data)
		{
			this._data = data;
		}

		// Token: 0x17001153 RID: 4435
		// (get) Token: 0x0600A60A RID: 42506 RVA: 0x004D49C5 File Offset: 0x004D2BC5
		public int CharacterId
		{
			get
			{
				GroupCharDisplayData data = this._data;
				return (data != null) ? data.CharacterId : -1;
			}
		}

		// Token: 0x0600A60B RID: 42507 RVA: 0x004D49DC File Offset: 0x004D2BDC
		public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
		{
			return GroupCharDisplaySelectAdapter.ToGeneralScrollListData(this._data);
		}

		// Token: 0x0600A60C RID: 42508 RVA: 0x004D49FC File Offset: 0x004D2BFC
		public static GroupCharDisplaySelectAdapter Wrap(GroupCharDisplayData data)
		{
			return new GroupCharDisplaySelectAdapter(data);
		}

		// Token: 0x0600A60D RID: 42509 RVA: 0x004D4A14 File Offset: 0x004D2C14
		public static CharacterDisplayDataForGeneralScrollList ToGeneralScrollListData(GroupCharDisplayData data)
		{
			bool flag = data == null;
			CharacterDisplayDataForGeneralScrollList result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new CharacterDisplayDataForGeneralScrollList
				{
					CharacterId = data.CharacterId,
					CharacterTemplateId = data.CharacterTemplateId,
					NameData = data.NameData,
					Gender = data.Gender,
					PhysiologicalAge = data.PhysiologicalAge,
					BehaviorType = data.BehaviorType,
					FavorabilityToTaiwu = data.FavorabilityToTaiwu,
					IsInteractedWithTaiwu = data.IsInteractedWithTaiwu,
					OrgInfo = new OrganizationInfo(data.NameData.OrgTemplateId, data.NameData.OrgGrade, true, -1)
				};
			}
			return result;
		}

		// Token: 0x04008302 RID: 33538
		private readonly GroupCharDisplayData _data;
	}
}
