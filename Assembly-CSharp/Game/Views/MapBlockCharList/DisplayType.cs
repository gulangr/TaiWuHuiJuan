using System;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x0200093D RID: 2365
	[Flags]
	public enum DisplayType
	{
		// Token: 0x040051F1 RID: 20977
		Normal = 1,
		// Token: 0x040051F2 RID: 20978
		Enemy = 2,
		// Token: 0x040051F3 RID: 20979
		Caravan = 4,
		// Token: 0x040051F4 RID: 20980
		Grave = 8,
		// Token: 0x040051F5 RID: 20981
		NormalCharacter = 17,
		// Token: 0x040051F6 RID: 20982
		NormalSpecial = 33,
		// Token: 0x040051F7 RID: 20983
		EnemyInfected = 66,
		// Token: 0x040051F8 RID: 20984
		EnemyAnimal = 130,
		// Token: 0x040051F9 RID: 20985
		EnemyTemplate = 258
	}
}
