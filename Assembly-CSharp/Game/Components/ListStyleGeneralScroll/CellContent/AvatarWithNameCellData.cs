using System;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.LegendaryBook;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EAF RID: 3759
	public class AvatarWithNameCellData
	{
		// Token: 0x0600AEB9 RID: 44729 RVA: 0x004F9ECC File Offset: 0x004F80CC
		public AvatarWithNameCellData(short templateId)
		{
			this.AsMerchant = true;
			this.TemplateId = templateId;
		}

		// Token: 0x0600AEBA RID: 44730 RVA: 0x004F9EE4 File Offset: 0x004F80E4
		public AvatarWithNameCellData(AvatarRelatedData avatarRelatedData, short templateId, string displayName, int characterId, bool asGrave, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null, bool isCompanion = false, bool followed = false)
		{
			this.AvatarRelatedData = avatarRelatedData;
			this.TemplateId = templateId;
			this.DisplayName = displayName;
			this.CharacterId = characterId;
			this.AsGrave = asGrave;
			this.OnClickCallback = onClickCallback;
			this.MouseTipModifier = mouseTipModifier;
			this.IsCompanion = isCompanion;
			this.Followed = followed;
		}

		// Token: 0x0600AEBB RID: 44731 RVA: 0x004F9F40 File Offset: 0x004F8140
		public static AvatarWithNameCellData FromGroupCharDisplayData(GroupCharDisplayData data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null, bool isCompanion = false, bool followed = false)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.CharacterId, false, onClickCallback, mouseTipModifier, isCompanion, followed);
		}

		// Token: 0x0600AEBC RID: 44732 RVA: 0x004F9F80 File Offset: 0x004F8180
		public static AvatarWithNameCellData FromLegendaryBookCharacterRelatedData(LegendaryBookCharacterRelatedData data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameRelatedData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.Id, false, onClickCallback, mouseTipModifier, data.IsCompanion, false);
		}

		// Token: 0x0600AEBD RID: 44733 RVA: 0x004F9FC4 File Offset: 0x004F81C4
		public static AvatarWithNameCellData FromVillagerCharDisplayData(VillagerCharDisplayData data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.CharacterId, false, onClickCallback, mouseTipModifier, data.IsCompanion, data.Followed);
		}

		// Token: 0x0600AEBE RID: 44734 RVA: 0x004FA00C File Offset: 0x004F820C
		public static AvatarWithNameCellData FromKidnapCharDisplayData(KidnapCharDisplayData data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.CharacterId, false, onClickCallback, mouseTipModifier, false, false);
		}

		// Token: 0x0600AEBF RID: 44735 RVA: 0x004FA04C File Offset: 0x004F824C
		public static AvatarWithNameCellData FromCharacterDisplayDataForGeneralScrollList(CharacterDisplayDataForGeneralScrollList data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.CharacterId, data.Health < 0, onClickCallback, mouseTipModifier, data.IsCompanion, false);
		}

		// Token: 0x0600AEC0 RID: 44736 RVA: 0x004FA098 File Offset: 0x004F8298
		public static AvatarWithNameCellData FromItemNeedCharacterDisplayData(ItemNeedCharacterDisplayData data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, (short)data.CharacterTemplateId, displayName, data.CharacterId, data.Health < 0, onClickCallback, mouseTipModifier, data.IsCompanion, false);
		}

		// Token: 0x0600AEC1 RID: 44737 RVA: 0x004FA0E4 File Offset: 0x004F82E4
		public static AvatarWithNameCellData FromVillagerDisplayData(VillagerCharDisplayData data, bool isTaiwu, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			return new AvatarWithNameCellData(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.CharacterId, data.Health < 0, onClickCallback, mouseTipModifier, data.IsCompanion, false);
		}

		// Token: 0x0600AEC2 RID: 44738 RVA: 0x004FA130 File Offset: 0x004F8330
		public static AvatarWithNameCellData FromMerchantInfoMerchantData(MerchantInfoMerchantData data)
		{
			return new AvatarWithNameCellData(data.AvatarData, data.NameRelatedData.CharTemplateId, NameCenter.GetMonasticTitleOrDisplayName(ref data.NameRelatedData, false, false), data.CharId, false, null, null, false, false);
		}

		// Token: 0x0600AEC3 RID: 44739 RVA: 0x004FA170 File Offset: 0x004F8370
		public static AvatarWithNameCellData FromRecruitData(BuildingRecruitCharacterData data, Action<int> onClickCallback = null, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			ValueTuple<string, string> name = data.CharacterData.FullName.GetName(data.CharacterData.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
			string surname = name.Item1;
			string givenName = name.Item2;
			string displayName = surname + givenName;
			return new AvatarWithNameCellData(data.CharacterData.GenerateAvatarRelatedData(), data.CharacterData.TemplateId, displayName, -1, false, onClickCallback, mouseTipModifier, false, false);
		}

		// Token: 0x04008716 RID: 34582
		public AvatarRelatedData AvatarRelatedData;

		// Token: 0x04008717 RID: 34583
		public short TemplateId;

		// Token: 0x04008718 RID: 34584
		public string DisplayName;

		// Token: 0x04008719 RID: 34585
		public int CharacterId;

		// Token: 0x0400871A RID: 34586
		public bool AsGrave;

		// Token: 0x0400871B RID: 34587
		public bool IsCompanion;

		// Token: 0x0400871C RID: 34588
		public bool AsMerchant;

		// Token: 0x0400871D RID: 34589
		public bool Followed;

		// Token: 0x0400871E RID: 34590
		public Action<int> OnClickCallback;

		// Token: 0x0400871F RID: 34591
		public Action<TooltipInvoker, int> MouseTipModifier;
	}
}
