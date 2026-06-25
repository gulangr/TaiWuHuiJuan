using System;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF3 RID: 3315
	public abstract class SelectCharacterFilterMenuBase<TData> : DetailedFilterMenuLogic<TData> where TData : ISelectCharacterData
	{
		// Token: 0x0600A692 RID: 42642 RVA: 0x004D7B3C File Offset: 0x004D5D3C
		protected CharacterDisplayDataForGeneralScrollList GetGeneralData(TData data)
		{
			return data.GetGeneralScrollListData();
		}
	}
}
