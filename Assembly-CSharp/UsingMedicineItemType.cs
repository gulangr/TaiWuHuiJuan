using System;

// Token: 0x020001CA RID: 458
public static class UsingMedicineItemType
{
	// Token: 0x06001C8D RID: 7309 RVA: 0x000C7E48 File Offset: 0x000C6048
	public static bool IsHurt(short type)
	{
		return type >= UsingMedicineItemType.BodyPartTypeChest && type <= UsingMedicineItemType.BodyPartTypeRightLeg;
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x000C7E60 File Offset: 0x000C6060
	public static bool IsPoison(short type)
	{
		return type >= UsingMedicineItemType.PoisonTypeHot && type <= UsingMedicineItemType.PoisonTypeIllusory;
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x000C7E78 File Offset: 0x000C6078
	public static bool IsHealth(short type)
	{
		return type == UsingMedicineItemType.Health;
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x000C7E82 File Offset: 0x000C6082
	public static bool IsDisorderOfQi(short type)
	{
		return type == UsingMedicineItemType.DisorderOfQi;
	}

	// Token: 0x0400162D RID: 5677
	public static short Invalid = -1;

	// Token: 0x0400162E RID: 5678
	public static short BodyPartTypeChest = 0;

	// Token: 0x0400162F RID: 5679
	public static short BodyPartTypeBelly = 1;

	// Token: 0x04001630 RID: 5680
	public static short BodyPartTypeHead = 2;

	// Token: 0x04001631 RID: 5681
	public static short BodyPartTypeLeftHand = 3;

	// Token: 0x04001632 RID: 5682
	public static short BodyPartTypeRightHand = 4;

	// Token: 0x04001633 RID: 5683
	public static short BodyPartTypeLeftLeg5;

	// Token: 0x04001634 RID: 5684
	public static short BodyPartTypeRightLeg = 6;

	// Token: 0x04001635 RID: 5685
	public static short PoisonTypeHot = 7;

	// Token: 0x04001636 RID: 5686
	public static short PoisonTypeGloomy = 8;

	// Token: 0x04001637 RID: 5687
	public static short PoisonTypeCold = 9;

	// Token: 0x04001638 RID: 5688
	public static short PoisonTypeRed = 10;

	// Token: 0x04001639 RID: 5689
	public static short PoisonTypeRotten = 11;

	// Token: 0x0400163A RID: 5690
	public static short PoisonTypeIllusory = 12;

	// Token: 0x0400163B RID: 5691
	public static short Health = 13;

	// Token: 0x0400163C RID: 5692
	public static short DisorderOfQi = 14;
}
