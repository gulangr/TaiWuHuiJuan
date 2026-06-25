using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Global.Inscription;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007E0 RID: 2016
	public interface IAvatarSubPageParent
	{
		// Token: 0x06006241 RID: 25153
		AvatarData GetAvatarData();

		// Token: 0x06006242 RID: 25154
		void RefreshAvatar();

		// Token: 0x06006243 RID: 25155
		void MarkAvatarDirty();

		// Token: 0x06006244 RID: 25156
		void MarkDirtyWithoutInscriptionClear();

		// Token: 0x06006245 RID: 25157
		bool IsAvatarDirty();

		// Token: 0x06006246 RID: 25158
		void ApplyPreset(AvatarPreset preset);

		// Token: 0x06006247 RID: 25159
		void ApplyInscribedCharacter(InscribedCharacterKey key, InscribedCharacter character);

		// Token: 0x06006248 RID: 25160
		void TrySavePendingPreset();

		// Token: 0x06006249 RID: 25161
		AvatarData GetCurrentAvatarData();

		// Token: 0x0600624A RID: 25162
		bool GetIsTransGender();

		// Token: 0x0600624B RID: 25163
		sbyte GetGender();

		// Token: 0x0600624C RID: 25164
		short GetAge();

		// Token: 0x0600624D RID: 25165
		void RefreshHairTabsIfNeeded();

		// Token: 0x0600624E RID: 25166 RVA: 0x002D147C File Offset: 0x002CF67C
		bool OnlyShowShaveItems()
		{
			return false;
		}

		// Token: 0x0600624F RID: 25167 RVA: 0x002D148F File Offset: 0x002CF68F
		Func<sbyte, bool> GetBodyTypeFilter()
		{
			return null;
		}

		// Token: 0x06006250 RID: 25168 RVA: 0x002D1492 File Offset: 0x002CF692
		Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>> GetSkinColorFilter()
		{
			return null;
		}

		// Token: 0x06006251 RID: 25169 RVA: 0x002D1495 File Offset: 0x002CF695
		Func<List<HairRes>, List<HairRes>> GetFrontHairFilter()
		{
			return null;
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x002D1498 File Offset: 0x002CF698
		Func<List<HairRes>, List<HairRes>> GetBackHairFilter()
		{
			return null;
		}

		// Token: 0x06006253 RID: 25171 RVA: 0x002D149B File Offset: 0x002CF69B
		Func<List<AvatarAsset>, List<AvatarAsset>> GetEyebrowFilter()
		{
			return null;
		}

		// Token: 0x06006254 RID: 25172 RVA: 0x002D149E File Offset: 0x002CF69E
		Func<List<EyeRes>, List<EyeRes>> GetEyesFilter()
		{
			return null;
		}

		// Token: 0x06006255 RID: 25173 RVA: 0x002D14A1 File Offset: 0x002CF6A1
		Func<List<AvatarAsset>, List<AvatarAsset>> GetNoseFilter()
		{
			return null;
		}

		// Token: 0x06006256 RID: 25174 RVA: 0x002D14A4 File Offset: 0x002CF6A4
		Func<List<MouthRes>, List<MouthRes>> GetMouthFilter()
		{
			return null;
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x002D14A7 File Offset: 0x002CF6A7
		Func<List<AvatarAsset>, List<AvatarAsset>> GetBeard1Filter()
		{
			return null;
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x002D14AA File Offset: 0x002CF6AA
		Func<List<AvatarAsset>, List<AvatarAsset>> GetBeard2Filter()
		{
			return null;
		}

		// Token: 0x06006259 RID: 25177 RVA: 0x002D14AD File Offset: 0x002CF6AD
		Func<List<AvatarAsset>, List<AvatarAsset>> GetFeature1Filter()
		{
			return null;
		}

		// Token: 0x0600625A RID: 25178 RVA: 0x002D14B0 File Offset: 0x002CF6B0
		Func<List<AvatarAsset>, List<AvatarAsset>> GetFeature2Filter()
		{
			return null;
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x002D14B3 File Offset: 0x002CF6B3
		bool CanShaveHairBald()
		{
			return true;
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x002D14B6 File Offset: 0x002CF6B6
		bool CanShaveBeard1Bald()
		{
			return true;
		}

		// Token: 0x0600625D RID: 25181 RVA: 0x002D14B9 File Offset: 0x002CF6B9
		bool CanShaveBeard2Bald()
		{
			return true;
		}

		// Token: 0x0600625E RID: 25182 RVA: 0x002D14BC File Offset: 0x002CF6BC
		bool CanShaveEyebrowBald()
		{
			return true;
		}

		// Token: 0x0600625F RID: 25183 RVA: 0x002D14BF File Offset: 0x002CF6BF
		bool IsCopyCloth()
		{
			return false;
		}
	}
}
