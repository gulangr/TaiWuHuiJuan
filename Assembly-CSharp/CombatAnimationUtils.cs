using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x0200033D RID: 829
public static class CombatAnimationUtils
{
	// Token: 0x060030B5 RID: 12469 RVA: 0x0017DEBE File Offset: 0x0017C0BE
	public static void UpdateSkeleton(SkeletonAnimation skeletonAnimation, CharacterDisplayData charData, List<ItemDisplayData> equipments)
	{
		CombatAnimationUtils.UpdateSkeleton(skeletonAnimation, charData, equipments, null);
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x0017DECB File Offset: 0x0017C0CB
	public static void UpdateSkeleton(SkeletonGraphic skeletonGraphic, CharacterDisplayData charData, List<ItemDisplayData> equipments)
	{
		CombatAnimationUtils.UpdateSkeleton(skeletonGraphic, charData, equipments, null);
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x0017DED8 File Offset: 0x0017C0D8
	public static void UpdateSkeleton(SkeletonAnimation skeletonAnimation, CharacterDisplayData charData, List<ItemDisplayData> equipments, List<sbyte> hideEquipSlots)
	{
		bool isKid = charData.AvatarRelatedData.DisplayAge < 16;
		Skeleton skeleton = skeletonAnimation.Skeleton;
		Transform boneRoot = skeletonAnimation.GetComponent<SkeletonUtility>().GetBoneRoot();
		Transform root = boneRoot.Find("root");
		Transform head = boneRoot.Find("root/ROLL/waist/body/head");
		skeletonAnimation.transform.localScale = Vector3.one;
		skeleton.SetSkin(isKid ? "kid" : ((charData.AvatarRelatedData.AvatarData.Gender == 0) ? "female" : "doll"));
		root.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
		root.localScale = Vector3.one * (isKid ? 0.85f : 1f);
		head.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
		head.localScale = Vector3.one * (isKid ? 1.2f : 1f);
		AvatarData avatarData = charData.AvatarRelatedData.AvatarData;
		ValueTuple<string[], string[]> hairData = avatarData.GetSkeletonSlotAndAttachment();
		Dictionary<string, string> attachmentDict = EasyPool.Get<Dictionary<string, string>>();
		bool hideHair = false;
		bool showHair = avatarData.GetGrowableElementShowingState(0) && avatarData.GetGrowableElementShowingAbility(0);
		attachmentDict.Clear();
		bool flag = showHair;
		if (flag)
		{
			bool flag2 = hairData.Item1 != null;
			if (flag2)
			{
				for (int i = 0; i < hairData.Item1.Length; i += 2)
				{
					attachmentDict.Add(hairData.Item1[i], hairData.Item1[i + 1]);
				}
			}
			bool flag3 = hairData.Item2 != null;
			if (flag3)
			{
				for (int j = 0; j < hairData.Item2.Length; j += 2)
				{
					attachmentDict.Add(hairData.Item2[j], hairData.Item2[j + 1]);
				}
			}
		}
		sbyte k = 0;
		while ((int)k < equipments.Count)
		{
			ItemKey equipKey = equipments[(int)k].Key;
			bool flag4 = !equipKey.IsValid() || equipKey.ItemType != 1 || (hideEquipSlots != null && hideEquipSlots.Contains(k));
			if (!flag4)
			{
				List<string> equipData = Armor.Instance[equipKey.TemplateId].SkeletonSlotAndAttachment;
				bool flag5 = equipData != null;
				if (flag5)
				{
					for (int l = 0; l < equipData.Count; l += 2)
					{
						attachmentDict.Add(equipData[l], equipData[l + 1]);
					}
					bool flag6 = k == 3 && CombatAnimationUtils.HideHairEquipAttachments.Exist((string attachName) => attachName.Equals(equipData[1]));
					if (flag6)
					{
						hideHair = true;
					}
				}
			}
			k += 1;
		}
		string[] changeDisplaySlots = CombatAnimationUtils.ChangeDisplaySlots;
		for (int m = 0; m < changeDisplaySlots.Length; m++)
		{
			string slotName = changeDisplaySlots[m];
			string attachmentName = (attachmentDict.ContainsKey(slotName) && (!hideHair || !CombatAnimationUtils.NeedHideHairSlots.Exist((string hairSlot) => hairSlot.Equals(slotName)))) ? attachmentDict[slotName] : null;
			skeleton.SetAttachment(slotName, attachmentName);
			Attachment externAttachment = skeletonAnimation.GetExternAttachment(slotName, attachmentName);
			bool flag7 = externAttachment != null;
			if (flag7)
			{
				skeleton.FindSlot(slotName).Attachment = externAttachment;
			}
		}
		EasyPool.Free<Dictionary<string, string>>(attachmentDict);
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x0017E26C File Offset: 0x0017C46C
	public static void UpdateSkeleton(SkeletonGraphic skeletonGraphic, CharacterDisplayData charData, List<ItemDisplayData> equipments, List<sbyte> hideEquipSlots)
	{
		bool isKid = charData.AvatarRelatedData.DisplayAge < 16;
		Skeleton skeleton = skeletonGraphic.Skeleton;
		Transform boneRoot = skeletonGraphic.GetComponent<SkeletonUtility>().GetBoneRoot();
		Transform root = boneRoot.Find("root");
		Transform head = boneRoot.Find("root/ROLL/waist/body/head");
		skeleton.SetSkin(isKid ? "kid" : ((charData.AvatarRelatedData.AvatarData.Gender == 0) ? "female" : "doll"));
		root.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
		root.localScale = Vector3.one * (isKid ? 0.85f : 1f);
		head.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
		head.localScale = Vector3.one * (isKid ? 1.2f : 1f);
		AvatarData avatarData = charData.AvatarRelatedData.AvatarData;
		ValueTuple<string[], string[]> hairData = avatarData.GetSkeletonSlotAndAttachment();
		Dictionary<string, string> attachmentDict = EasyPool.Get<Dictionary<string, string>>();
		bool hideHair = false;
		bool showHair = avatarData.GetGrowableElementShowingState(0) && avatarData.GetGrowableElementShowingAbility(0);
		attachmentDict.Clear();
		bool flag = showHair;
		if (flag)
		{
			bool flag2 = hairData.Item1 != null;
			if (flag2)
			{
				for (int i = 0; i < hairData.Item1.Length; i += 2)
				{
					attachmentDict.Add(hairData.Item1[i], hairData.Item1[i + 1]);
				}
			}
			bool flag3 = hairData.Item2 != null;
			if (flag3)
			{
				for (int j = 0; j < hairData.Item2.Length; j += 2)
				{
					attachmentDict.Add(hairData.Item2[j], hairData.Item2[j + 1]);
				}
			}
		}
		sbyte k = 0;
		while ((int)k < equipments.Count)
		{
			ItemKey equipKey = equipments[(int)k].Key;
			bool flag4 = !equipKey.IsValid() || equipKey.ItemType != 1 || (hideEquipSlots != null && hideEquipSlots.Contains(k));
			if (!flag4)
			{
				List<string> equipData = Armor.Instance[equipKey.TemplateId].SkeletonSlotAndAttachment;
				bool flag5 = equipData != null;
				if (flag5)
				{
					for (int l = 0; l < equipData.Count; l += 2)
					{
						attachmentDict.Add(equipData[l], equipData[l + 1]);
					}
					bool flag6 = k == 3 && CombatAnimationUtils.HideHairEquipAttachments.Exist((string attachName) => attachName.Equals(equipData[1]));
					if (flag6)
					{
						hideHair = true;
					}
				}
			}
			k += 1;
		}
		string[] changeDisplaySlots = CombatAnimationUtils.ChangeDisplaySlots;
		for (int m = 0; m < changeDisplaySlots.Length; m++)
		{
			string slotName = changeDisplaySlots[m];
			string attachmentName = (attachmentDict.ContainsKey(slotName) && (!hideHair || !CombatAnimationUtils.NeedHideHairSlots.Exist((string hairSlot) => hairSlot.Equals(slotName)))) ? attachmentDict[slotName] : null;
			skeleton.SetAttachment(slotName, attachmentName);
			Attachment externAttachment = skeletonGraphic.GetExternAttachment(slotName, attachmentName);
			bool flag7 = externAttachment != null;
			if (flag7)
			{
				skeleton.FindSlot(slotName).Attachment = externAttachment;
			}
		}
		EasyPool.Free<Dictionary<string, string>>(attachmentDict);
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x0017E5F0 File Offset: 0x0017C7F0
	public static void UpdateSkeletonWeapon(ISkeletonAnimation skeleton, int weaponTemplateId)
	{
		WeaponItem configData = Weapon.Instance[weaponTemplateId];
		skeleton.Skeleton.SetAttachment("Weapons_L", configData.CombatPictureL);
		skeleton.Skeleton.SetAttachment("Weapons_R", configData.CombatPictureR);
		Attachment externWeaponL = skeleton.GetExternAttachment("Weapons_L", configData.CombatPictureL);
		bool flag = externWeaponL != null;
		if (flag)
		{
			skeleton.Skeleton.FindSlot("Weapons_L").Attachment = externWeaponL;
		}
		Attachment externWeaponR = skeleton.GetExternAttachment("Weapons_R", configData.CombatPictureR);
		bool flag2 = externWeaponR != null;
		if (flag2)
		{
			skeleton.Skeleton.FindSlot("Weapons_R").Attachment = externWeaponR;
		}
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x0017E69C File Offset: 0x0017C89C
	public static void ClearSkeletonWeaponSlot(ISkeletonAnimation skeleton)
	{
		skeleton.Skeleton.SetAttachment("Weapons_L", null);
		skeleton.Skeleton.SetAttachment("Weapons_R", null);
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x0017E6C4 File Offset: 0x0017C8C4
	public static void UpdateSkeletonSpecial(SkeletonAnimation skeletonAnimation, CombatSkeletonItem combatSkeleton)
	{
		Skeleton skeleton = skeletonAnimation.Skeleton;
		skeletonAnimation.transform.localScale = Vector3.one * combatSkeleton.GlobalScale;
		skeleton.SetSkin(combatSkeleton.SkinName);
		SkeletonUtility skeletonUtility = skeletonAnimation.GetComponent<SkeletonUtility>();
		bool flag = skeletonUtility != null;
		if (flag)
		{
			Transform boneRoot = skeletonUtility.GetBoneRoot();
			Transform root = boneRoot.Find("root");
			Transform head = boneRoot.Find("root/ROLL/waist/body/head");
			root.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
			root.localScale = Vector3.one * combatSkeleton.RootScale;
			head.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
			head.localScale = Vector3.one * combatSkeleton.HeadScale;
		}
		bool isNotStandard = combatSkeleton.IsNotStandard;
		if (!isNotStandard)
		{
			Dictionary<string, string> slot2Attachments = EasyPool.Get<Dictionary<string, string>>();
			slot2Attachments.Clear();
			int slot2AttachmentsCount = Math.Min(combatSkeleton.Slots.Length, combatSkeleton.Attachments.Length);
			for (int i = 0; i < slot2AttachmentsCount; i++)
			{
				slot2Attachments[combatSkeleton.Slots[i]] = combatSkeleton.Attachments[i];
			}
			foreach (string slotName in CombatAnimationUtils.ChangeDisplaySlots)
			{
				string attachmentName;
				slot2Attachments.TryGetValue(slotName, out attachmentName);
				skeleton.SetAttachment(slotName, attachmentName);
			}
			EasyPool.Free<Dictionary<string, string>>(slot2Attachments);
			bool flag2 = !combatSkeleton.SpecialRightWeapon.IsNullOrEmpty();
			if (flag2)
			{
				skeleton.SetAttachment("Weapons_R", combatSkeleton.SpecialRightWeapon);
			}
		}
	}

	// Token: 0x0400237F RID: 9087
	public static readonly string[] ChangeDisplaySlots = new string[]
	{
		"hair/hairtop",
		"hair/female_double_hairtop_l_front",
		"hair/female_double_hairtop_r_front",
		"hair/female_double_hairtop_l",
		"hair/female_double_hairtop_r",
		"hairup",
		"hairup_double_l_front",
		"hairup_double_r_front",
		"hairup_double_l",
		"hairup_double_r",
		"hairdown",
		"hairdown_double_l_front",
		"hairdown_double_r_front",
		"hairdown_double_l",
		"hairdown_double_r",
		"hair_front",
		"hair",
		"bangs",
		"hair/hair_hat",
		"headwear/headwear",
		"equip_hel/equip_hel",
		"collar/collar",
		"peplum/peplum",
		"equip_arm_l/equip_arm_l",
		"equip_arm_r/equip_arm_r",
		"equip_elbow_l",
		"equip_elbow_r",
		"sleeve_l/sleeve_l_weapon",
		"sleeve_r/sleeve_r_weapon",
		"sleeve_l/sleeve_l",
		"sleeve_r/sleeve_r",
		"equip_bw_l/equip_bw_l",
		"equip_bw_r/equip_bw_r",
		"equip_waist/equip_waist",
		"equip_leg_l/equip_leg_l",
		"equip_leg_r/equip_leg_r",
		"skirt/skirt",
		"skirt_back/skirt_back",
		"equip_shank_l",
		"equip_shank_r"
	};

	// Token: 0x04002380 RID: 9088
	public static readonly string[] HideHairEquipAttachments = new string[]
	{
		"equip_hel/equip_hel_111",
		"equip_hel/equip_hel_115",
		"equip_hel/equip_hel_119",
		"equip_hel/equip_hel_121",
		"equip_hel/equip_hel_125",
		"equip_hel/equip_hel_129",
		"equip_hel/equip_hel_131",
		"equip_hel/equip_hel_135",
		"equip_hel/equip_hel_139",
		"equip_hel/equip_hel_211",
		"equip_hel/equip_hel_215",
		"equip_hel/equip_hel_219",
		"equip_hel/equip_hel_221",
		"equip_hel/equip_hel_225",
		"equip_hel/equip_hel_229",
		"equip_hel/equip_hel_231",
		"equip_hel/equip_hel_235",
		"equip_hel/equip_hel_239",
		"equip_hel/equip_hel_311",
		"equip_hel/equip_hel_315",
		"equip_hel/equip_hel_319",
		"equip_hel/equip_hel_321",
		"equip_hel/equip_hel_325",
		"equip_hel/equip_hel_329",
		"equip_hel/equip_hel_331",
		"equip_hel/equip_hel_335",
		"equip_hel/equip_hel_339",
		"headwear/headwear_341",
		"headwear/headwear_345",
		"headwear/headwear_349",
		"headwear/headwear_351",
		"headwear/headwear_355",
		"headwear/headwear_359",
		"headwear/headwear_361",
		"headwear/headwear_365",
		"headwear/headwear_369"
	};

	// Token: 0x04002381 RID: 9089
	public static readonly string[] NeedHideHairSlots = new string[]
	{
		"hair/hairtop",
		"hair/female_double_hairtop_l_front",
		"hair/female_double_hairtop_r_front",
		"hair/female_double_hairtop_l",
		"hair/female_double_hairtop_r",
		"hairup",
		"hairup_double_l_front",
		"hairup_double_r_front",
		"hairup_double_l",
		"hairup_double_r"
	};
}
