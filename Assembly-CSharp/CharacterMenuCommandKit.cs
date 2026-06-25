using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class CharacterMenuCommandKit : CommandKitBase
{
	// Token: 0x06000847 RID: 2119 RVA: 0x0003816C File Offset: 0x0003636C
	public CharacterMenuCommandKit()
	{
		base.Id = 12;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_CharacterMenu;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::CharacterMenuCommandKit.ShowCharacterMenuCharacter,
			global::CharacterMenuCommandKit.ShowCharacterMenuTeam,
			global::CharacterMenuCommandKit.ShowCharacterMenuKidnap,
			global::CharacterMenuCommandKit.ShowCharacterMenuEquip,
			global::CharacterMenuCommandKit.ShowCharacterMenuCarrier,
			global::CharacterMenuCommandKit.ShowCharacterMenuItems,
			global::CharacterMenuCommandKit.ShowCharacterMenuAttainments,
			global::CharacterMenuCommandKit.ShowCharacterMenuCombatAttainments,
			global::CharacterMenuCommandKit.ShowCharacterMenuLifeSkillAttainments,
			global::CharacterMenuCommandKit.ShowCharacterMenuBreakout,
			global::CharacterMenuCommandKit.ShowCharacterMenuNieli,
			global::CharacterMenuCommandKit.ShowCharacterMenuEquipCombatSkill,
			global::CharacterMenuCommandKit.ShowCharacterMenuRelationship,
			global::CharacterMenuCommandKit.ShowCharacterMenuFamilyTree,
			global::CharacterMenuCommandKit.ShowCharacterMenuLifeRecords,
			global::CharacterMenuCommandKit.ShowCharacterMenuLifeInformation,
			global::CharacterMenuCommandKit.ShowCharacterMenuSecretInformation
		};
	}

	// Token: 0x04000A8F RID: 2703
	public static HotKeyCommand ShowCharacterMenuCharacter = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_CharacterMenu_Character, KeyCode.F1, KeyCode.None, true, true);

	// Token: 0x04000A90 RID: 2704
	public static HotKeyCommand ShowCharacterMenuTeam = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_CharacterMenu_Team, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A91 RID: 2705
	public static HotKeyCommand ShowCharacterMenuKidnap = new HotKeyCommand(3, LanguageKey.LK_HotKeyGroup_CharacterMenu_Kidnap, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A92 RID: 2706
	public static HotKeyCommand ShowCharacterMenuEquip = new HotKeyCommand(4, LanguageKey.LK_HotKeyGroup_CharacterMenu_Equip, KeyCode.F2, KeyCode.None, true, true);

	// Token: 0x04000A93 RID: 2707
	public static HotKeyCommand ShowCharacterMenuCarrier = new HotKeyCommand(17, LanguageKey.LK_HotKeyGroup_CharacterMenu_Carrier, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A94 RID: 2708
	public static HotKeyCommand ShowCharacterMenuItems = new HotKeyCommand(5, LanguageKey.LK_HotKeyGroup_CharacterMenu_Items, KeyCode.F3, KeyCode.None, true, true);

	// Token: 0x04000A95 RID: 2709
	public static HotKeyCommand ShowCharacterMenuAttainments = new HotKeyCommand(6, LanguageKey.LK_HotKeyGroup_CharacterMenu_Attainments, KeyCode.F4, KeyCode.None, true, true);

	// Token: 0x04000A96 RID: 2710
	public static HotKeyCommand ShowCharacterMenuCombatAttainments = new HotKeyCommand(7, LanguageKey.LK_HotKeyGroup_CharacterMenu_CombatAttainments, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A97 RID: 2711
	public static HotKeyCommand ShowCharacterMenuLifeSkillAttainments = new HotKeyCommand(8, LanguageKey.LK_HotKeyGroup_CharacterMenu_LifeSkillAttainments, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A98 RID: 2712
	public static HotKeyCommand ShowCharacterMenuBreakout = new HotKeyCommand(9, LanguageKey.LK_HotKeyGroup_CharacterMenu_Breakout, KeyCode.F5, KeyCode.None, true, true);

	// Token: 0x04000A99 RID: 2713
	public static HotKeyCommand ShowCharacterMenuNieli = new HotKeyCommand(10, LanguageKey.LK_HotKeyGroup_CharacterMenu_Nieli, KeyCode.F6, KeyCode.None, true, true);

	// Token: 0x04000A9A RID: 2714
	public static HotKeyCommand ShowCharacterMenuEquipCombatSkill = new HotKeyCommand(11, LanguageKey.LK_HotKeyGroup_CharacterMenu_EquipCombatSkill, KeyCode.F7, KeyCode.None, true, true);

	// Token: 0x04000A9B RID: 2715
	public static HotKeyCommand ShowCharacterMenuRelationship = new HotKeyCommand(12, LanguageKey.LK_HotKeyGroup_CharacterMenu_Relationship, KeyCode.F8, KeyCode.None, true, true);

	// Token: 0x04000A9C RID: 2716
	public static HotKeyCommand ShowCharacterMenuFamilyTree = new HotKeyCommand(13, LanguageKey.LK_HotKeyGroup_CharacterMenu_FamilyTree, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A9D RID: 2717
	public static HotKeyCommand ShowCharacterMenuLifeRecords = new HotKeyCommand(14, LanguageKey.LK_HotKeyGroup_CharacterMenu_LifeRecords, KeyCode.F9, KeyCode.None, true, true);

	// Token: 0x04000A9E RID: 2718
	public static HotKeyCommand ShowCharacterMenuLifeInformation = new HotKeyCommand(15, LanguageKey.LK_HotKeyGroup_CharacterMenu_LifeInformation, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000A9F RID: 2719
	public static HotKeyCommand ShowCharacterMenuSecretInformation = new HotKeyCommand(16, LanguageKey.LK_HotKeyGroup_CharacterMenu_SecretInformation, KeyCode.F10, KeyCode.None, true, true);
}
