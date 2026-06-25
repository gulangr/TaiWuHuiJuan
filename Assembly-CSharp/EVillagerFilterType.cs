using System;

// Token: 0x02000357 RID: 855
[Flags]
public enum EVillagerFilterType
{
	// Token: 0x04002489 RID: 9353
	None = 0,
	// Token: 0x0400248A RID: 9354
	All = 1,
	// Token: 0x0400248B RID: 9355
	Adult = 2,
	// Token: 0x0400248C RID: 9356
	Teenager = 4,
	// Token: 0x0400248D RID: 9357
	Learning = 8,
	// Token: 0x0400248E RID: 9358
	FinishLearning = 16,
	// Token: 0x0400248F RID: 9359
	Farmer = 32,
	// Token: 0x04002490 RID: 9360
	FilterAll = 63
}
