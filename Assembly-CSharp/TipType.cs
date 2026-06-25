using System;

// Token: 0x0200007F RID: 127
public enum TipType
{
	// Token: 0x04000307 RID: 775
	SingleDesc,
	// Token: 0x04000308 RID: 776
	Simple,
	// Token: 0x04000309 RID: 777
	CombatSkill,
	// Token: 0x0400030A RID: 778
	Weapon = 4,
	// Token: 0x0400030B RID: 779
	SkillBook,
	// Token: 0x0400030C RID: 780
	CraftTool,
	// Token: 0x0400030D RID: 781
	Material,
	// Token: 0x0400030E RID: 782
	Cricket,
	// Token: 0x0400030F RID: 783
	Armor = 10,
	// Token: 0x04000310 RID: 784
	Carrier,
	// Token: 0x04000311 RID: 785
	Clothing,
	// Token: 0x04000312 RID: 786
	Food,
	// Token: 0x04000313 RID: 787
	Medicine,
	// Token: 0x04000314 RID: 788
	Misc,
	// Token: 0x04000315 RID: 789
	TeaWine,
	// Token: 0x04000316 RID: 790
	Accessory,
	// Token: 0x04000317 RID: 791
	LifeRecords,
	// Token: 0x04000318 RID: 792
	Character,
	// Token: 0x04000319 RID: 793
	Resource,
	// Token: 0x0400031A RID: 794
	ResourceHolder,
	// Token: 0x0400031B RID: 795
	EatingItems,
	// Token: 0x0400031C RID: 796
	MapBlock,
	// Token: 0x0400031D RID: 797
	Feature,
	// Token: 0x0400031E RID: 798
	MartialArtTournament,
	// Token: 0x0400031F RID: 799
	SimpleWide,
	// Token: 0x04000320 RID: 800
	MakeItem,
	// Token: 0x04000321 RID: 801
	InnateFiveElements,
	// Token: 0x04000322 RID: 802
	DisassembleItem,
	// Token: 0x04000323 RID: 803
	RepairItem,
	// Token: 0x04000324 RID: 804
	ReadingBook,
	// Token: 0x04000325 RID: 805
	SecretInformation,
	// Token: 0x04000326 RID: 806
	LifeCombatSkillValue,
	// Token: 0x04000327 RID: 807
	BuildingShowItem,
	// Token: 0x04000328 RID: 808
	BuildingShowRecruitPeople,
	// Token: 0x04000329 RID: 809
	SecretInformationBroadcastNotify,
	// Token: 0x0400032A RID: 810
	DebtChange,
	// Token: 0x0400032B RID: 811
	LegendaryBookBonus,
	// Token: 0x0400032C RID: 812
	ProfessionSkill,
	// Token: 0x0400032D RID: 813
	AdventureNode,
	// Token: 0x0400032E RID: 814
	Injury = 42,
	// Token: 0x0400032F RID: 815
	MapArea,
	// Token: 0x04000330 RID: 816
	AttachedPoison,
	// Token: 0x04000331 RID: 817
	MixPoison,
	// Token: 0x04000332 RID: 818
	Adventure,
	// Token: 0x04000333 RID: 819
	CharacterPoison,
	// Token: 0x04000334 RID: 820
	LifeSkillValue,
	// Token: 0x04000335 RID: 821
	CombatSkillValue,
	// Token: 0x04000336 RID: 822
	BodyPart = 51,
	// Token: 0x04000337 RID: 823
	CombatSkillPractice,
	// Token: 0x04000338 RID: 824
	CombatSkillBanReason,
	// Token: 0x04000339 RID: 825
	Fold,
	// Token: 0x0400033A RID: 826
	MonthNotify,
	// Token: 0x0400033B RID: 827
	CombatSkillBreakout,
	// Token: 0x0400033C RID: 828
	Flaw,
	// Token: 0x0400033D RID: 829
	CombatChangeTrick,
	// Token: 0x0400033E RID: 830
	Advance,
	// Token: 0x0400033F RID: 831
	TrickType,
	// Token: 0x04000340 RID: 832
	UpgradeTeammateCommand,
	// Token: 0x04000341 RID: 833
	FiveElements,
	// Token: 0x04000342 RID: 834
	NeiliAllocation,
	// Token: 0x04000343 RID: 835
	Music,
	// Token: 0x04000344 RID: 836
	ReadingEvent,
	// Token: 0x04000345 RID: 837
	Legacy,
	// Token: 0x04000346 RID: 838
	Fuyu,
	// Token: 0x04000347 RID: 839
	DynamicCondition,
	// Token: 0x04000348 RID: 840
	Jiao,
	// Token: 0x04000349 RID: 841
	JiaoEgg,
	// Token: 0x0400034A RID: 842
	loongDebuff,
	// Token: 0x0400034B RID: 843
	JiaoNurturance,
	// Token: 0x0400034C RID: 844
	CombatSkillBuff,
	// Token: 0x0400034D RID: 845
	GeneralLines,
	// Token: 0x0400034E RID: 846
	MixPoisonEffectSimple,
	// Token: 0x0400034F RID: 847
	MixPoisonEffectDetailed,
	// Token: 0x04000350 RID: 848
	DisorderOfQi,
	// Token: 0x04000351 RID: 849
	MakeWugKing = 79,
	// Token: 0x04000352 RID: 850
	EmptyContainer,
	// Token: 0x04000353 RID: 851
	CharacterComplete,
	// Token: 0x04000354 RID: 852
	BuildingProduce,
	// Token: 0x04000355 RID: 853
	BuildingProduceCollectResource,
	// Token: 0x04000356 RID: 854
	MixPoisonEffectOutCombat,
	// Token: 0x04000357 RID: 855
	EatingWug,
	// Token: 0x04000358 RID: 856
	BuildingRequireCultureSafety,
	// Token: 0x04000359 RID: 857
	ChangeTrick,
	// Token: 0x0400035A RID: 858
	CombatBannedList,
	// Token: 0x0400035B RID: 859
	CombatBlockAttack,
	// Token: 0x0400035C RID: 860
	Encyclopedia = 100,
	// Token: 0x0400035D RID: 861
	EquipLoad,
	// Token: 0x0400035E RID: 862
	DefeatMark,
	// Token: 0x0400035F RID: 863
	DamageValue,
	// Token: 0x04000360 RID: 864
	Destiny,
	// Token: 0x04000361 RID: 865
	SettlementTreasury,
	// Token: 0x04000362 RID: 866
	LoopingEvent,
	// Token: 0x04000363 RID: 867
	BuildingLevel,
	// Token: 0x04000364 RID: 868
	LegendaryBookGiveUp = 109,
	// Token: 0x04000365 RID: 869
	LifeLinkNeiliType,
	// Token: 0x04000366 RID: 870
	ActiveRead,
	// Token: 0x04000367 RID: 871
	ActiveLoop,
	// Token: 0x04000368 RID: 872
	ReadProgress,
	// Token: 0x04000369 RID: 873
	LoopProgress,
	// Token: 0x0400036A RID: 874
	CharacterOnMapBlock,
	// Token: 0x0400036B RID: 875
	TeammateCommand,
	// Token: 0x0400036C RID: 876
	FeatureMedalLegacy,
	// Token: 0x0400036D RID: 877
	SimpleWithHotkeyDisplay,
	// Token: 0x0400036E RID: 878
	FulongFlame,
	// Token: 0x0400036F RID: 879
	VillagerRoleAvailableCount,
	// Token: 0x04000370 RID: 880
	VillagerRoleEffect,
	// Token: 0x04000371 RID: 881
	CombatRawCreate,
	// Token: 0x04000372 RID: 882
	CombatUnlockProgress,
	// Token: 0x04000373 RID: 883
	CombatWeaponUnlock,
	// Token: 0x04000374 RID: 884
	Profession,
	// Token: 0x04000375 RID: 885
	MouseTipGearMateUpgradeAttribute,
	// Token: 0x04000376 RID: 886
	MouseTipGearMateUpgradeFeature,
	// Token: 0x04000377 RID: 887
	CaravanOperation,
	// Token: 0x04000378 RID: 888
	TaiwuWanted,
	// Token: 0x04000379 RID: 889
	CaravanPath,
	// Token: 0x0400037A RID: 890
	ExtraProfessionSkill,
	// Token: 0x0400037B RID: 891
	Organization,
	// Token: 0x0400037C RID: 892
	VillagerNeedItem,
	// Token: 0x0400037D RID: 893
	InteractCheckResultPhase,
	// Token: 0x0400037E RID: 894
	NormalInformationType,
	// Token: 0x0400037F RID: 895
	ExpCheck,
	// Token: 0x04000380 RID: 896
	CombatSkillBreakInfo,
	// Token: 0x04000381 RID: 897
	DemonSlayer,
	// Token: 0x04000382 RID: 898
	LegacyLevel,
	// Token: 0x04000383 RID: 899
	SkillBreakBonus,
	// Token: 0x04000384 RID: 900
	SectStory,
	// Token: 0x04000385 RID: 901
	RecordIncompatible,
	// Token: 0x04000386 RID: 902
	TeammateCount,
	// Token: 0x04000387 RID: 903
	SkillBreakNormalCell,
	// Token: 0x04000388 RID: 904
	CombatInjuryChange,
	// Token: 0x04000389 RID: 905
	LifeSkillDetailReadProgress,
	// Token: 0x0400038A RID: 906
	LifeSkillDetailUnlockBuilding,
	// Token: 0x0400038B RID: 907
	LifeSkillDetailUnlockInformation,
	// Token: 0x0400038C RID: 908
	LifeSkillDetailUnlockStrategy,
	// Token: 0x0400038D RID: 909
	LifeSkillCombatCardType,
	// Token: 0x0400038E RID: 910
	LifeSkillCombatUnit,
	// Token: 0x0400038F RID: 911
	LifeSkillCombatStrategy,
	// Token: 0x04000390 RID: 912
	LifeSkillCombatStress,
	// Token: 0x04000391 RID: 913
	SimpleList,
	// Token: 0x04000392 RID: 914
	LifeSkillCombatBlock,
	// Token: 0x04000393 RID: 915
	MatchVillagerRole,
	// Token: 0x04000394 RID: 916
	BuildingTeachBook,
	// Token: 0x04000395 RID: 917
	TaiwuVillageStele,
	// Token: 0x04000396 RID: 918
	WorkingStatus,
	// Token: 0x04000397 RID: 919
	LifeSkillCombatFirstMove,
	// Token: 0x04000398 RID: 920
	LifeSkillCombatLastMove,
	// Token: 0x04000399 RID: 921
	LifeSkillCombatAudience,
	// Token: 0x0400039A RID: 922
	CaravanPathDetail,
	// Token: 0x0400039B RID: 923
	ThreeVitals,
	// Token: 0x0400039C RID: 924
	PrisonerResistance,
	// Token: 0x0400039D RID: 925
	BuildingFeast,
	// Token: 0x0400039E RID: 926
	SpecialBuild,
	// Token: 0x0400039F RID: 927
	SettlementTreasuryOrPrisonLayer,
	// Token: 0x040003A0 RID: 928
	CombatSkillBonus,
	// Token: 0x040003A1 RID: 929
	CombatSkillOneBonus,
	// Token: 0x040003A2 RID: 930
	DeadCharacterComplete,
	// Token: 0x040003A3 RID: 931
	JieqingInteractCharTips,
	// Token: 0x040003A4 RID: 932
	CricketEncyclopedia,
	// Token: 0x040003A5 RID: 933
	ProfessionEncyclopedia,
	// Token: 0x040003A6 RID: 934
	ProfessionSkillEncyclopedia,
	// Token: 0x040003A7 RID: 935
	FeatureMedal,
	// Token: 0x040003A8 RID: 936
	Alertness,
	// Token: 0x040003A9 RID: 937
	BuildingBlock,
	// Token: 0x040003AA RID: 938
	MakeTargetMaterial,
	// Token: 0x040003AB RID: 939
	CommonTip,
	// Token: 0x040003AC RID: 940
	LegendaryBook,
	// Token: 0x040003AD RID: 941
	LegendaryBookPageItem,
	// Token: 0x040003AE RID: 942
	Fame,
	// Token: 0x040003AF RID: 943
	HealthInfo,
	// Token: 0x040003B0 RID: 944
	PracticeNotice,
	// Token: 0x040003B1 RID: 945
	ActiveLoopCost,
	// Token: 0x040003B2 RID: 946
	AiAction,
	// Token: 0x040003B3 RID: 947
	CharacterCurrentProfession,
	// Token: 0x040003B4 RID: 948
	ActiveReadCost,
	// Token: 0x040003B5 RID: 949
	KongSangDing,
	// Token: 0x040003B6 RID: 950
	SoulPiece,
	// Token: 0x040003B7 RID: 951
	Chicken,
	// Token: 0x040003B8 RID: 952
	VillagerAssign,
	// Token: 0x040003B9 RID: 953
	Count
}
