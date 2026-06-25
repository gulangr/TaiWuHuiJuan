using System;

// Token: 0x020000F1 RID: 241
public enum ECombatCommandType : byte
{
	// Token: 0x04000AF4 RID: 2804
	MoveBackward = 1,
	// Token: 0x04000AF5 RID: 2805
	MoveForward,
	// Token: 0x04000AF6 RID: 2806
	NormalAttack,
	// Token: 0x04000AF7 RID: 2807
	ChangeTrickAttack,
	// Token: 0x04000AF8 RID: 2808
	Pause,
	// Token: 0x04000AF9 RID: 2809
	BulletTime,
	// Token: 0x04000AFA RID: 2810
	ChangeToWeapon0,
	// Token: 0x04000AFB RID: 2811
	ChangeToWeapon1,
	// Token: 0x04000AFC RID: 2812
	ChangeToWeapon2,
	// Token: 0x04000AFD RID: 2813
	ChangeToWeapon3,
	// Token: 0x04000AFE RID: 2814
	ChangeToWeapon4,
	// Token: 0x04000AFF RID: 2815
	ChangeToWeapon5,
	// Token: 0x04000B00 RID: 2816
	UseSkill0 = 18,
	// Token: 0x04000B01 RID: 2817
	UseSkill1,
	// Token: 0x04000B02 RID: 2818
	UseSkill2,
	// Token: 0x04000B03 RID: 2819
	UseSkill3,
	// Token: 0x04000B04 RID: 2820
	UseSkill4,
	// Token: 0x04000B05 RID: 2821
	UseSkill5,
	// Token: 0x04000B06 RID: 2822
	UseSkill6,
	// Token: 0x04000B07 RID: 2823
	UseSkill7,
	// Token: 0x04000B08 RID: 2824
	UseSkill8,
	// Token: 0x04000B09 RID: 2825
	UseSkill9,
	// Token: 0x04000B0A RID: 2826
	UseSkill10,
	// Token: 0x04000B0B RID: 2827
	UseSkill11,
	// Token: 0x04000B0C RID: 2828
	UseSkill12,
	// Token: 0x04000B0D RID: 2829
	UseSkill13,
	// Token: 0x04000B0E RID: 2830
	UseSkill14,
	// Token: 0x04000B0F RID: 2831
	UseSkill15,
	// Token: 0x04000B10 RID: 2832
	UseSkill16,
	// Token: 0x04000B11 RID: 2833
	UseSkill17,
	// Token: 0x04000B12 RID: 2834
	UseSkill18,
	// Token: 0x04000B13 RID: 2835
	UseSkill19,
	// Token: 0x04000B14 RID: 2836
	UseSkill20,
	// Token: 0x04000B15 RID: 2837
	UseSkill21,
	// Token: 0x04000B16 RID: 2838
	UseSkill22,
	// Token: 0x04000B17 RID: 2839
	UseSkill23,
	// Token: 0x04000B18 RID: 2840
	UseSkill24,
	// Token: 0x04000B19 RID: 2841
	UseSkill25,
	// Token: 0x04000B1A RID: 2842
	UseSkill26,
	// Token: 0x04000B1B RID: 2843
	HealInjury,
	// Token: 0x04000B1C RID: 2844
	HealPoison,
	// Token: 0x04000B1D RID: 2845
	Flee,
	// Token: 0x04000B1E RID: 2846
	SpeedUp,
	// Token: 0x04000B1F RID: 2847
	SpeedDown,
	// Token: 0x04000B20 RID: 2848
	ChangeToWeapon6,
	// Token: 0x04000B21 RID: 2849
	TargetBackward,
	// Token: 0x04000B22 RID: 2850
	TargetForward,
	// Token: 0x04000B23 RID: 2851
	TargetCancel,
	// Token: 0x04000B24 RID: 2852
	TargetBackwardRegion,
	// Token: 0x04000B25 RID: 2853
	TargetForwardRegion,
	// Token: 0x04000B26 RID: 2854
	ClearDefend,
	// Token: 0x04000B27 RID: 2855
	AutoFight,
	// Token: 0x04000B28 RID: 2856
	ClearAgile,
	// Token: 0x04000B29 RID: 2857
	UnlockWeapon0,
	// Token: 0x04000B2A RID: 2858
	UnlockWeapon1,
	// Token: 0x04000B2B RID: 2859
	UnlockWeapon2,
	// Token: 0x04000B2C RID: 2860
	AnimalAttack,
	// Token: 0x04000B2D RID: 2861
	Surrender,
	// Token: 0x04000B2E RID: 2862
	UseTeammateCommand0,
	// Token: 0x04000B2F RID: 2863
	UseTeammateCommand1,
	// Token: 0x04000B30 RID: 2864
	UseTeammateCommand2,
	// Token: 0x04000B31 RID: 2865
	UseTeammateCommand3,
	// Token: 0x04000B32 RID: 2866
	UseTeammateCommand4,
	// Token: 0x04000B33 RID: 2867
	UseTeammateCommand5,
	// Token: 0x04000B34 RID: 2868
	UseTeammateCommand6,
	// Token: 0x04000B35 RID: 2869
	UseTeammateCommand7,
	// Token: 0x04000B36 RID: 2870
	UseTeammateCommand8,
	// Token: 0x04000B37 RID: 2871
	CancelReserveNormalAttack,
	// Token: 0x04000B38 RID: 2872
	PureMode,
	// Token: 0x04000B39 RID: 2873
	ShowHideTips,
	// Token: 0x04000B3A RID: 2874
	OpenQuickWheel,
	// Token: 0x04000B3B RID: 2875
	OpenSkillWheel,
	// Token: 0x04000B3C RID: 2876
	ViewAllyCharacter,
	// Token: 0x04000B3D RID: 2877
	ViewEnemyCharacter,
	// Token: 0x04000B3E RID: 2878
	ToggleMoveTarget,
	// Token: 0x04000B3F RID: 2879
	AmplifyCast,
	// Token: 0x04000B40 RID: 2880
	InterruptDestroy,
	// Token: 0x04000B41 RID: 2881
	InterruptAgile,
	// Token: 0x04000B42 RID: 2882
	InterruptDefend,
	// Token: 0x04000B43 RID: 2883
	IncreaseExternalDamageRatio,
	// Token: 0x04000B44 RID: 2884
	IncreaseInternalDamageRatio,
	// Token: 0x04000B45 RID: 2885
	ClearAttack,
	// Token: 0x04000B46 RID: 2886
	OpenUseItemPanel
}
