using System;

// Token: 0x020000E2 RID: 226
public enum EEvents
{
	// Token: 0x040008DB RID: 2267
	Invalid,
	// Token: 0x040008DC RID: 2268
	OnGameResourceReady,
	// Token: 0x040008DD RID: 2269
	OnGameStateChange,
	// Token: 0x040008DE RID: 2270
	LoadingProgress,
	// Token: 0x040008DF RID: 2271
	ArchivesInfoReady,
	// Token: 0x040008E0 RID: 2272
	WorldDataReady,
	// Token: 0x040008E1 RID: 2273
	OnAdvancingMonthStateChange,
	// Token: 0x040008E2 RID: 2274
	OnMonthChange,
	// Token: 0x040008E3 RID: 2275
	OnActionPointInPrevMonthChange,
	// Token: 0x040008E4 RID: 2276
	OnActionPointChange,
	// Token: 0x040008E5 RID: 2277
	OnAreaSpiritualDebtChange,
	// Token: 0x040008E6 RID: 2278
	InscriptionChange,
	// Token: 0x040008E7 RID: 2279
	InscriptionCharacterRemove,
	// Token: 0x040008E8 RID: 2280
	OnScreenResolutionChange,
	// Token: 0x040008E9 RID: 2281
	OnSavingWorldStateChange,
	// Token: 0x040008EA RID: 2282
	OnFunctionLockStateChange,
	// Token: 0x040008EB RID: 2283
	OnTutorialFunctionStatusChange,
	// Token: 0x040008EC RID: 2284
	OnConfirmQuitGameState,
	// Token: 0x040008ED RID: 2285
	OnFullScreenChange,
	// Token: 0x040008EE RID: 2286
	OnTaiwuCharIdChange,
	// Token: 0x040008EF RID: 2287
	OnTaiwuResourceChange,
	// Token: 0x040008F0 RID: 2288
	TaiwuGroupChange,
	// Token: 0x040008F1 RID: 2289
	ResetWorldSettingsStateChanged,
	// Token: 0x040008F2 RID: 2290
	RequestAdvanceMonth,
	// Token: 0x040008F3 RID: 2291
	SetAdvanceMonthLock,
	// Token: 0x040008F4 RID: 2292
	GuidingChapterDataChange,
	// Token: 0x040008F5 RID: 2293
	AchievementUnlocked
}
