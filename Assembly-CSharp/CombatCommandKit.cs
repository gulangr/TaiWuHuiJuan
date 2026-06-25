using System;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class CombatCommandKit : CommandKitBase
{
	// Token: 0x0600084B RID: 2123 RVA: 0x000385D4 File Offset: 0x000367D4
	public CombatCommandKit()
	{
		base.Id = 3;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_CombatSystem;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::CombatCommandKit.Pause,
			global::CombatCommandKit.BulletTime,
			global::CombatCommandKit.SpeedUp,
			global::CombatCommandKit.SpeedDown,
			global::CombatCommandKit.PureMode,
			global::CombatCommandKit.ShowHideTips,
			global::CombatCommandKit.AutoFight,
			global::CombatCommandKit.OpenQuickWheel,
			global::CombatCommandKit.OpenUseItemPanel,
			global::CombatCommandKit.MoveBackward,
			global::CombatCommandKit.MoveForward,
			global::CombatCommandKit.TargetBackward,
			global::CombatCommandKit.TargetForward,
			global::CombatCommandKit.TargetBackwardRegion,
			global::CombatCommandKit.TargetForwardRegion,
			global::CombatCommandKit.ToggleMoveTarget,
			global::CombatCommandKit.NormalAttack,
			global::CombatCommandKit.CancelReserveNormalAttack,
			global::CombatCommandKit.ChangeTrickAttack,
			global::CombatCommandKit.AmplifyCast,
			global::CombatCommandKit.ChangeToWeapon0,
			global::CombatCommandKit.ChangeToWeapon1,
			global::CombatCommandKit.ChangeToWeapon2,
			global::CombatCommandKit.ChangeToWeapon3,
			global::CombatCommandKit.ChangeToWeapon4,
			global::CombatCommandKit.ChangeToWeapon5,
			global::CombatCommandKit.ChangeToWeapon6,
			global::CombatCommandKit.UnlockWeapon0,
			global::CombatCommandKit.UnlockWeapon1,
			global::CombatCommandKit.UnlockWeapon2,
			global::CombatCommandKit.UseSkill0,
			global::CombatCommandKit.UseSkill1,
			global::CombatCommandKit.UseSkill2,
			global::CombatCommandKit.UseSkill3,
			global::CombatCommandKit.UseSkill4,
			global::CombatCommandKit.UseSkill5,
			global::CombatCommandKit.UseSkill6,
			global::CombatCommandKit.UseSkill7,
			global::CombatCommandKit.UseSkill8,
			global::CombatCommandKit.UseSkill9,
			global::CombatCommandKit.UseSkill10,
			global::CombatCommandKit.UseSkill11,
			global::CombatCommandKit.UseSkill12,
			global::CombatCommandKit.UseSkill13,
			global::CombatCommandKit.UseSkill14,
			global::CombatCommandKit.UseSkill15,
			global::CombatCommandKit.UseSkill16,
			global::CombatCommandKit.UseSkill17,
			global::CombatCommandKit.UseSkill18,
			global::CombatCommandKit.UseSkill19,
			global::CombatCommandKit.UseSkill20,
			global::CombatCommandKit.UseSkill21,
			global::CombatCommandKit.UseSkill22,
			global::CombatCommandKit.UseSkill23,
			global::CombatCommandKit.UseSkill24,
			global::CombatCommandKit.UseSkill25,
			global::CombatCommandKit.UseSkill26,
			global::CombatCommandKit.UseTeammateCommand0,
			global::CombatCommandKit.UseTeammateCommand1,
			global::CombatCommandKit.UseTeammateCommand2,
			global::CombatCommandKit.UseTeammateCommand3,
			global::CombatCommandKit.UseTeammateCommand4,
			global::CombatCommandKit.UseTeammateCommand5,
			global::CombatCommandKit.UseTeammateCommand6,
			global::CombatCommandKit.UseTeammateCommand7,
			global::CombatCommandKit.UseTeammateCommand8,
			global::CombatCommandKit.ClearAttack,
			global::CombatCommandKit.ClearAgile,
			global::CombatCommandKit.ClearDefend
		};
		this.TipDisplayCommandArray = new HotKeyCommand[]
		{
			global::CombatCommandKit.NormalAttack
		};
	}

	// Token: 0x04000AAE RID: 2734
	public static HotKeyCommand Pause = ECombatCommandType.Pause.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Pause_Resume, KeyCode.Space, KeyCode.None, false);

	// Token: 0x04000AAF RID: 2735
	public static HotKeyCommand BulletTime = ECombatCommandType.BulletTime.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Bullet_Time, KeyCode.LeftAlt, KeyCode.None, true);

	// Token: 0x04000AB0 RID: 2736
	public static HotKeyCommand ShowHideTips = ECombatCommandType.ShowHideTips.Create(LanguageKey.LK_HotKeyGroup_Combat_ShowHideTips, KeyCode.F, KeyCode.LeftShift, true);

	// Token: 0x04000AB1 RID: 2737
	public static HotKeyCommand SpeedUp = ECombatCommandType.SpeedUp.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Speed_Up, KeyCode.E, KeyCode.None, true);

	// Token: 0x04000AB2 RID: 2738
	public static HotKeyCommand SpeedDown = ECombatCommandType.SpeedDown.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Speed_Down, KeyCode.Q, KeyCode.None, true);

	// Token: 0x04000AB3 RID: 2739
	public static HotKeyCommand PureMode = ECombatCommandType.PureMode.Create(LanguageKey.LK_CombatPure_HotKey, KeyCode.G, KeyCode.LeftShift, true);

	// Token: 0x04000AB4 RID: 2740
	public static HotKeyCommand AutoFight = ECombatCommandType.AutoFight.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_AutoFight, KeyCode.R, KeyCode.None, true);

	// Token: 0x04000AB5 RID: 2741
	public static HotKeyCommand OpenQuickWheel = ECombatCommandType.OpenQuickWheel.Create(LanguageKey.LK_HotKeyGroup_Combat_OpenQuickWheel, KeyCode.Tab, KeyCode.None, true);

	// Token: 0x04000AB6 RID: 2742
	public static HotKeyCommand MoveBackward = ECombatCommandType.MoveBackward.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_CombatSystem_MoveLeft, KeyCode.A, KeyCode.None, true);

	// Token: 0x04000AB7 RID: 2743
	public static HotKeyCommand MoveForward = ECombatCommandType.MoveForward.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_CombatSystem_MoveRight, KeyCode.D, KeyCode.None, true);

	// Token: 0x04000AB8 RID: 2744
	public static HotKeyCommand TargetBackward = ECombatCommandType.TargetBackward.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_TargetLeft, (KeyCode)1000, KeyCode.None, true);

	// Token: 0x04000AB9 RID: 2745
	public static HotKeyCommand TargetForward = ECombatCommandType.TargetForward.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_TargetRight, (KeyCode)1001, KeyCode.None, true);

	// Token: 0x04000ABA RID: 2746
	public static HotKeyCommand TargetBackwardRegion = ECombatCommandType.TargetBackwardRegion.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_TargetLeftRegion, (KeyCode)1000, KeyCode.LeftShift, true);

	// Token: 0x04000ABB RID: 2747
	public static HotKeyCommand TargetForwardRegion = ECombatCommandType.TargetForwardRegion.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_TargetRightRegion, (KeyCode)1001, KeyCode.LeftShift, true);

	// Token: 0x04000ABC RID: 2748
	public static HotKeyCommand ToggleMoveTarget = ECombatCommandType.ToggleMoveTarget.Create(LanguageKey.LK_HotKeyGroup_Combat_ToggleMoveTarget, KeyCode.Mouse2, KeyCode.None, true);

	// Token: 0x04000ABD RID: 2749
	public static HotKeyCommand NormalAttack = ECombatCommandType.NormalAttack.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Normal_Attack, KeyCode.J, KeyCode.None, true);

	// Token: 0x04000ABE RID: 2750
	public static HotKeyCommand CancelReserveNormalAttack = ECombatCommandType.CancelReserveNormalAttack.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Normal_CancelReserveNormalAttack, KeyCode.S, KeyCode.None, true);

	// Token: 0x04000ABF RID: 2751
	public static HotKeyCommand ChangeTrickAttack = ECombatCommandType.ChangeTrickAttack.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Trick, KeyCode.C, KeyCode.None, true);

	// Token: 0x04000AC0 RID: 2752
	public static HotKeyCommand AmplifyCast = ECombatCommandType.AmplifyCast.Create(LanguageKey.LK_HotKeyGroup_Combat_AmplifyCast, KeyCode.Z, KeyCode.None, true);

	// Token: 0x04000AC1 RID: 2753
	public static HotKeyCommand ChangeToWeapon0 = ECombatCommandType.ChangeToWeapon0.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_0, KeyCode.Alpha1, KeyCode.None, true);

	// Token: 0x04000AC2 RID: 2754
	public static HotKeyCommand ChangeToWeapon1 = ECombatCommandType.ChangeToWeapon1.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_1, KeyCode.Alpha2, KeyCode.None, true);

	// Token: 0x04000AC3 RID: 2755
	public static HotKeyCommand ChangeToWeapon2 = ECombatCommandType.ChangeToWeapon2.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_2, KeyCode.Alpha3, KeyCode.None, true);

	// Token: 0x04000AC4 RID: 2756
	public static HotKeyCommand ChangeToWeapon3 = ECombatCommandType.ChangeToWeapon3.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_3, KeyCode.Alpha4, KeyCode.None, true);

	// Token: 0x04000AC5 RID: 2757
	public static HotKeyCommand ChangeToWeapon4 = ECombatCommandType.ChangeToWeapon4.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_4, KeyCode.Alpha5, KeyCode.None, true);

	// Token: 0x04000AC6 RID: 2758
	public static HotKeyCommand ChangeToWeapon5 = ECombatCommandType.ChangeToWeapon5.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_5, KeyCode.Alpha6, KeyCode.None, true);

	// Token: 0x04000AC7 RID: 2759
	public static HotKeyCommand ChangeToWeapon6 = ECombatCommandType.ChangeToWeapon6.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Weapon_6, KeyCode.Alpha7, KeyCode.None, true);

	// Token: 0x04000AC8 RID: 2760
	public static HotKeyCommand UnlockWeapon0 = ECombatCommandType.UnlockWeapon0.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Unlock_0, KeyCode.Alpha1, KeyCode.LeftControl, true);

	// Token: 0x04000AC9 RID: 2761
	public static HotKeyCommand UnlockWeapon1 = ECombatCommandType.UnlockWeapon1.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Unlock_1, KeyCode.Alpha2, KeyCode.LeftControl, true);

	// Token: 0x04000ACA RID: 2762
	public static HotKeyCommand UnlockWeapon2 = ECombatCommandType.UnlockWeapon2.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Change_Unlock_2, KeyCode.Alpha3, KeyCode.LeftControl, true);

	// Token: 0x04000ACB RID: 2763
	public static HotKeyCommand UseSkill0 = ECombatCommandType.UseSkill0.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_0, KeyCode.F1, KeyCode.None, true);

	// Token: 0x04000ACC RID: 2764
	public static HotKeyCommand UseSkill1 = ECombatCommandType.UseSkill1.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_1, KeyCode.F2, KeyCode.None, true);

	// Token: 0x04000ACD RID: 2765
	public static HotKeyCommand UseSkill2 = ECombatCommandType.UseSkill2.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_2, KeyCode.F3, KeyCode.None, true);

	// Token: 0x04000ACE RID: 2766
	public static HotKeyCommand UseSkill3 = ECombatCommandType.UseSkill3.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_3, KeyCode.F4, KeyCode.None, true);

	// Token: 0x04000ACF RID: 2767
	public static HotKeyCommand UseSkill4 = ECombatCommandType.UseSkill4.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_4, KeyCode.F5, KeyCode.None, true);

	// Token: 0x04000AD0 RID: 2768
	public static HotKeyCommand UseSkill5 = ECombatCommandType.UseSkill5.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_5, KeyCode.F6, KeyCode.None, true);

	// Token: 0x04000AD1 RID: 2769
	public static HotKeyCommand UseSkill6 = ECombatCommandType.UseSkill6.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_6, KeyCode.F7, KeyCode.None, true);

	// Token: 0x04000AD2 RID: 2770
	public static HotKeyCommand UseSkill7 = ECombatCommandType.UseSkill7.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_7, KeyCode.F8, KeyCode.None, true);

	// Token: 0x04000AD3 RID: 2771
	public static HotKeyCommand UseSkill8 = ECombatCommandType.UseSkill8.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_8, KeyCode.F9, KeyCode.None, true);

	// Token: 0x04000AD4 RID: 2772
	public static HotKeyCommand UseSkill9 = ECombatCommandType.UseSkill9.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_9, KeyCode.F1, KeyCode.LeftShift, true);

	// Token: 0x04000AD5 RID: 2773
	public static HotKeyCommand UseSkill10 = ECombatCommandType.UseSkill10.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_10, KeyCode.F2, KeyCode.LeftShift, true);

	// Token: 0x04000AD6 RID: 2774
	public static HotKeyCommand UseSkill11 = ECombatCommandType.UseSkill11.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_11, KeyCode.F3, KeyCode.LeftShift, true);

	// Token: 0x04000AD7 RID: 2775
	public static HotKeyCommand UseSkill12 = ECombatCommandType.UseSkill12.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_12, KeyCode.F4, KeyCode.LeftShift, true);

	// Token: 0x04000AD8 RID: 2776
	public static HotKeyCommand UseSkill13 = ECombatCommandType.UseSkill13.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_13, KeyCode.F5, KeyCode.LeftShift, true);

	// Token: 0x04000AD9 RID: 2777
	public static HotKeyCommand UseSkill14 = ECombatCommandType.UseSkill14.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_14, KeyCode.F6, KeyCode.LeftShift, true);

	// Token: 0x04000ADA RID: 2778
	public static HotKeyCommand UseSkill15 = ECombatCommandType.UseSkill15.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_15, KeyCode.F7, KeyCode.LeftShift, true);

	// Token: 0x04000ADB RID: 2779
	public static HotKeyCommand UseSkill16 = ECombatCommandType.UseSkill16.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_16, KeyCode.F8, KeyCode.LeftShift, true);

	// Token: 0x04000ADC RID: 2780
	public static HotKeyCommand UseSkill17 = ECombatCommandType.UseSkill17.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_17, KeyCode.F9, KeyCode.LeftShift, true);

	// Token: 0x04000ADD RID: 2781
	public static HotKeyCommand UseSkill18 = ECombatCommandType.UseSkill18.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_18, KeyCode.F1, KeyCode.LeftControl, true);

	// Token: 0x04000ADE RID: 2782
	public static HotKeyCommand UseSkill19 = ECombatCommandType.UseSkill19.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_19, KeyCode.F2, KeyCode.LeftControl, true);

	// Token: 0x04000ADF RID: 2783
	public static HotKeyCommand UseSkill20 = ECombatCommandType.UseSkill20.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_20, KeyCode.F3, KeyCode.LeftControl, true);

	// Token: 0x04000AE0 RID: 2784
	public static HotKeyCommand UseSkill21 = ECombatCommandType.UseSkill21.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_21, KeyCode.F4, KeyCode.LeftControl, true);

	// Token: 0x04000AE1 RID: 2785
	public static HotKeyCommand UseSkill22 = ECombatCommandType.UseSkill22.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_22, KeyCode.F5, KeyCode.LeftControl, true);

	// Token: 0x04000AE2 RID: 2786
	public static HotKeyCommand UseSkill23 = ECombatCommandType.UseSkill23.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_23, KeyCode.F6, KeyCode.LeftControl, true);

	// Token: 0x04000AE3 RID: 2787
	public static HotKeyCommand UseSkill24 = ECombatCommandType.UseSkill24.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_24, KeyCode.F7, KeyCode.LeftControl, true);

	// Token: 0x04000AE4 RID: 2788
	public static HotKeyCommand UseSkill25 = ECombatCommandType.UseSkill25.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_25, KeyCode.F8, KeyCode.LeftControl, true);

	// Token: 0x04000AE5 RID: 2789
	public static HotKeyCommand UseSkill26 = ECombatCommandType.UseSkill26.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_Skill_26, KeyCode.F9, KeyCode.LeftControl, true);

	// Token: 0x04000AE6 RID: 2790
	public static HotKeyCommand UseTeammateCommand0 = ECombatCommandType.UseTeammateCommand0.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_0, KeyCode.Alpha1, KeyCode.LeftShift, true);

	// Token: 0x04000AE7 RID: 2791
	public static HotKeyCommand UseTeammateCommand1 = ECombatCommandType.UseTeammateCommand1.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_1, KeyCode.Alpha2, KeyCode.LeftShift, true);

	// Token: 0x04000AE8 RID: 2792
	public static HotKeyCommand UseTeammateCommand2 = ECombatCommandType.UseTeammateCommand2.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_2, KeyCode.Alpha3, KeyCode.LeftShift, true);

	// Token: 0x04000AE9 RID: 2793
	public static HotKeyCommand UseTeammateCommand3 = ECombatCommandType.UseTeammateCommand3.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_3, KeyCode.Alpha4, KeyCode.LeftShift, true);

	// Token: 0x04000AEA RID: 2794
	public static HotKeyCommand UseTeammateCommand4 = ECombatCommandType.UseTeammateCommand4.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_4, KeyCode.Alpha5, KeyCode.LeftShift, true);

	// Token: 0x04000AEB RID: 2795
	public static HotKeyCommand UseTeammateCommand5 = ECombatCommandType.UseTeammateCommand5.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_5, KeyCode.Alpha6, KeyCode.LeftShift, true);

	// Token: 0x04000AEC RID: 2796
	public static HotKeyCommand UseTeammateCommand6 = ECombatCommandType.UseTeammateCommand6.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_6, KeyCode.Alpha7, KeyCode.LeftShift, true);

	// Token: 0x04000AED RID: 2797
	public static HotKeyCommand UseTeammateCommand7 = ECombatCommandType.UseTeammateCommand7.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_7, KeyCode.Alpha8, KeyCode.LeftShift, true);

	// Token: 0x04000AEE RID: 2798
	public static HotKeyCommand UseTeammateCommand8 = ECombatCommandType.UseTeammateCommand8.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Use_TeammateCommand_8, KeyCode.Alpha9, KeyCode.LeftShift, true);

	// Token: 0x04000AEF RID: 2799
	public static HotKeyCommand ClearDefend = ECombatCommandType.ClearDefend.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Clear_Defend, KeyCode.C, KeyCode.LeftControl, true);

	// Token: 0x04000AF0 RID: 2800
	public static HotKeyCommand ClearAgile = ECombatCommandType.ClearAgile.Create(LanguageKey.LK_SystemSetting_HotKeyGroup_Combat_Clear_Agile, KeyCode.X, KeyCode.LeftControl, true);

	// Token: 0x04000AF1 RID: 2801
	public static HotKeyCommand ClearAttack = ECombatCommandType.ClearAttack.Create(LanguageKey.LK_HotKeyGroup_Combat_InterruptDestroy, KeyCode.Z, KeyCode.LeftControl, true);

	// Token: 0x04000AF2 RID: 2802
	public static HotKeyCommand OpenUseItemPanel = ECombatCommandType.OpenUseItemPanel.Create(LanguageKey.LK_HotKeyGroup_CombatBehavior_UseItem, KeyCode.F, KeyCode.None, true);
}
