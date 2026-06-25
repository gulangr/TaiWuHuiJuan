using System;

// Token: 0x0200028B RID: 651
public class MouseTipConstant
{
	// Token: 0x060029D0 RID: 10704 RVA: 0x0013CF08 File Offset: 0x0013B108
	// Note: this type is marked as 'beforefieldinit'.
	static MouseTipConstant()
	{
		string[,] array = new string[7, 3];
		array[0, 0] = "Head";
		array[0, 1] = "LK_CombatSkill_HitParts_Head";
		array[0, 2] = "mousetip_buwei_big_0";
		array[1, 0] = "Chest";
		array[1, 1] = "LK_CombatSkill_HitParts_Chest";
		array[1, 2] = "mousetip_buwei_big_1";
		array[2, 0] = "Belly";
		array[2, 1] = "LK_CombatSkill_HitParts_Belly";
		array[2, 2] = "mousetip_buwei_big_2";
		array[3, 0] = "LeftHand";
		array[3, 1] = "LK_CombatSkill_HitParts_LeftHand";
		array[3, 2] = "mousetip_buwei_big_3";
		array[4, 0] = "RightHand";
		array[4, 1] = "LK_CombatSkill_HitParts_RightHand";
		array[4, 2] = "mousetip_buwei_big_4";
		array[5, 0] = "LeftFoot";
		array[5, 1] = "LK_CombatSkill_HitParts_LeftFoot";
		array[5, 2] = "mousetip_buwei_big_5";
		array[6, 0] = "RightFoot";
		array[6, 1] = "LK_CombatSkill_HitParts_RightFoot";
		array[6, 2] = "mousetip_buwei_big_6";
		MouseTipConstant.HitPartNames = array;
		string[,] array2 = new string[7, 3];
		array2[0, 0] = "Chest";
		array2[0, 1] = "LK_CombatSkill_HitParts_Chest";
		array2[0, 2] = "mousetip_buwei_big_1";
		array2[1, 0] = "Belly";
		array2[1, 1] = "LK_CombatSkill_HitParts_Belly";
		array2[1, 2] = "mousetip_buwei_big_2";
		array2[2, 0] = "Head";
		array2[2, 1] = "LK_CombatSkill_HitParts_Head";
		array2[2, 2] = "mousetip_buwei_big_0";
		array2[3, 0] = "LeftHand";
		array2[3, 1] = "LK_CombatSkill_HitParts_LeftHand";
		array2[3, 2] = "mousetip_buwei_big_3";
		array2[4, 0] = "RightHand";
		array2[4, 1] = "LK_CombatSkill_HitParts_RightHand";
		array2[4, 2] = "mousetip_buwei_big_4";
		array2[5, 0] = "LeftFoot";
		array2[5, 1] = "LK_CombatSkill_HitParts_LeftFoot";
		array2[5, 2] = "mousetip_buwei_big_5";
		array2[6, 0] = "RightFoot";
		array2[6, 1] = "LK_CombatSkill_HitParts_RightFoot";
		array2[6, 2] = "mousetip_buwei_big_6";
		MouseTipConstant.HitPartNamesByConfig = array2;
		string[,] array3 = new string[7, 3];
		array3[0, 0] = "Head";
		array3[0, 1] = "LK_CombatSkill_Injury_Head";
		array3[0, 2] = "mousetip_waishang_0";
		array3[1, 0] = "Chest";
		array3[1, 1] = "LK_CombatSkill_Injury_Chest";
		array3[1, 2] = "mousetip_waishang_1";
		array3[2, 0] = "Belly";
		array3[2, 1] = "LK_CombatSkill_Injury_Belly";
		array3[2, 2] = "mousetip_waishang_2";
		array3[3, 0] = "LeftHand";
		array3[3, 1] = "LK_CombatSkill_Injury_LeftHand";
		array3[3, 2] = "mousetip_waishang_3";
		array3[4, 0] = "RightHand";
		array3[4, 1] = "LK_CombatSkill_Injury_RightHand";
		array3[4, 2] = "mousetip_waishang_4";
		array3[5, 0] = "LeftFoot";
		array3[5, 1] = "LK_CombatSkill_Injury_LeftFoot";
		array3[5, 2] = "mousetip_waishang_5";
		array3[6, 0] = "RightFoot";
		array3[6, 1] = "LK_CombatSkill_Injury_RightFoot";
		array3[6, 2] = "mousetip_waishang_6";
		MouseTipConstant.OuterInjuryInfos = array3;
		string[,] array4 = new string[7, 3];
		array4[0, 0] = "Head";
		array4[0, 1] = "LK_CombatSkill_Injury_Head";
		array4[0, 2] = "mousetip_neishang_0";
		array4[1, 0] = "Chest";
		array4[1, 1] = "LK_CombatSkill_Injury_Chest";
		array4[1, 2] = "mousetip_neishang_1";
		array4[2, 0] = "Belly";
		array4[2, 1] = "LK_CombatSkill_Injury_Belly";
		array4[2, 2] = "mousetip_neishang_2";
		array4[3, 0] = "LeftHand";
		array4[3, 1] = "LK_CombatSkill_Injury_LeftHand";
		array4[3, 2] = "mousetip_neishang_3";
		array4[4, 0] = "RightHand";
		array4[4, 1] = "LK_CombatSkill_Injury_RightHand";
		array4[4, 2] = "mousetip_neishang_4";
		array4[5, 0] = "LeftFoot";
		array4[5, 1] = "LK_CombatSkill_Injury_LeftFoot";
		array4[5, 2] = "mousetip_neishang_5";
		array4[6, 0] = "RightFoot";
		array4[6, 1] = "LK_CombatSkill_Injury_RightFoot";
		array4[6, 2] = "mousetip_neishang_6";
		MouseTipConstant.InnerInjuryInfos = array4;
	}

	// Token: 0x04001E64 RID: 7780
	public static readonly string[,] HitPartNames;

	// Token: 0x04001E65 RID: 7781
	public static readonly string[,] HitPartNamesByConfig;

	// Token: 0x04001E66 RID: 7782
	public static readonly string[,] OuterInjuryInfos;

	// Token: 0x04001E67 RID: 7783
	public static readonly string[,] InnerInjuryInfos;
}
