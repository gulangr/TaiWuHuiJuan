using System;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class CombatBehaviorCommandKit : CommandKitBase
{
	// Token: 0x06000849 RID: 2121 RVA: 0x000383C8 File Offset: 0x000365C8
	public CombatBehaviorCommandKit()
	{
		base.Id = 13;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_CombatBehavior;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::CombatBehaviorCommandKit.HealInjury,
			global::CombatBehaviorCommandKit.HealPoison,
			global::CombatBehaviorCommandKit.AnimalAttack,
			global::CombatBehaviorCommandKit.Flee,
			global::CombatBehaviorCommandKit.Surrender,
			global::CombatBehaviorCommandKit.UseQuickItem1,
			global::CombatBehaviorCommandKit.UseQuickItem2,
			global::CombatBehaviorCommandKit.UseQuickItem3,
			global::CombatBehaviorCommandKit.UseQuickItem4,
			global::CombatBehaviorCommandKit.UseQuickItem5,
			global::CombatBehaviorCommandKit.UseQuickItem6,
			global::CombatBehaviorCommandKit.UseQuickItem7,
			global::CombatBehaviorCommandKit.UseQuickItem8,
			global::CombatBehaviorCommandKit.UseQuickItem9
		};
	}

	// Token: 0x04000AA0 RID: 2720
	public static HotKeyCommand HealInjury = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_CombatBehavior_HealInjury, KeyCode.H, KeyCode.None, true, true);

	// Token: 0x04000AA1 RID: 2721
	public static HotKeyCommand HealPoison = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_CombatBehavior_HealPoison, KeyCode.P, KeyCode.None, true, true);

	// Token: 0x04000AA2 RID: 2722
	public static HotKeyCommand AnimalAttack = new HotKeyCommand(4, LanguageKey.LK_HotKeyGroup_CombatBehavior_AnimalAttack, KeyCode.G, KeyCode.None, true, true);

	// Token: 0x04000AA3 RID: 2723
	public static HotKeyCommand Flee = new HotKeyCommand(5, LanguageKey.LK_HotKeyGroup_CombatBehavior_Flee, KeyCode.X, KeyCode.None, true, true);

	// Token: 0x04000AA4 RID: 2724
	public static HotKeyCommand Surrender = new HotKeyCommand(6, LanguageKey.LK_HotKeyGroup_CombatBehavior_Surrender, KeyCode.None, KeyCode.None, true, true);

	// Token: 0x04000AA5 RID: 2725
	public static HotKeyCommand UseQuickItem1 = new HotKeyCommand(7, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem1, KeyCode.Alpha1, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AA6 RID: 2726
	public static HotKeyCommand UseQuickItem2 = new HotKeyCommand(8, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem2, KeyCode.Alpha2, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AA7 RID: 2727
	public static HotKeyCommand UseQuickItem3 = new HotKeyCommand(9, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem3, KeyCode.Alpha3, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AA8 RID: 2728
	public static HotKeyCommand UseQuickItem4 = new HotKeyCommand(10, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem4, KeyCode.Alpha4, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AA9 RID: 2729
	public static HotKeyCommand UseQuickItem5 = new HotKeyCommand(11, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem5, KeyCode.Alpha5, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AAA RID: 2730
	public static HotKeyCommand UseQuickItem6 = new HotKeyCommand(12, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem6, KeyCode.Alpha6, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AAB RID: 2731
	public static HotKeyCommand UseQuickItem7 = new HotKeyCommand(13, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem7, KeyCode.Alpha7, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AAC RID: 2732
	public static HotKeyCommand UseQuickItem8 = new HotKeyCommand(14, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem8, KeyCode.Alpha8, KeyCode.LeftAlt, true, true);

	// Token: 0x04000AAD RID: 2733
	public static HotKeyCommand UseQuickItem9 = new HotKeyCommand(15, LanguageKey.LK_HotKeyGroup_CombatBehavior_UseQuickItem9, KeyCode.Alpha9, KeyCode.LeftAlt, true, true);
}
