using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x02000417 RID: 1047
public class VillagerRoleUtils
{
	// Token: 0x06003E5D RID: 15965 RVA: 0x001F4A58 File Offset: 0x001F2C58
	public static short GetRoleIdFromOrgMemberId(short orgMemberId)
	{
		return (short)(VillagerRole.Instance.Count - (int)(orgMemberId - 10));
	}

	// Token: 0x06003E5E RID: 15966 RVA: 0x001F4A7C File Offset: 0x001F2C7C
	public static bool CanGetRoleIdFromOrgMemberId(short orgMemberId)
	{
		return orgMemberId > 10 && (int)orgMemberId < 10 + VillagerRole.Instance.Count + 1;
	}

	// Token: 0x06003E5F RID: 15967 RVA: 0x001F4AA8 File Offset: 0x001F2CA8
	public unsafe static int CalculateRoleEffect(Personalities personalities, short roleTemplateId, int effectIndex)
	{
		VillagerRoleItem roleConfig = VillagerRole.Instance[roleTemplateId];
		float result = 0f;
		float[] valueList = roleConfig.EffectDisplayValueList[effectIndex];
		for (int i = 0; i < valueList.Length; i++)
		{
			result += (float)(*personalities[(int)roleConfig.NeedPersonalityList[i].PersonalityType]) * valueList[i];
		}
		return (int)Math.Floor((double)result);
	}

	// Token: 0x06003E60 RID: 15968 RVA: 0x001F4B24 File Offset: 0x001F2D24
	public static void RefreshArrangementIconTips(TooltipInvoker tipDisplayer, short arrangementTemplateId)
	{
		VillagerRoleArrangementItem arrangementConfig = VillagerRoleArrangement.Instance[arrangementTemplateId];
		if (tipDisplayer.RuntimeParam == null)
		{
			tipDisplayer.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Common_Doing, arrangementConfig.Name));
	}

	// Token: 0x06003E61 RID: 15969 RVA: 0x001F4B78 File Offset: 0x001F2D78
	public static void AsyncSetRoleName(TMP_Text label, short roleTemplateId, IAsyncMethodRequestHandler requestHandler = null, bool showBoth = false, Action<string> callback = null)
	{
		VillagerRoleItem config = VillagerRole.Instance[roleTemplateId];
		OrganizationMemberItem orgConfig = OrganizationMember.Instance[config.OrganizationMember];
		VillagerRoleUtils.AsyncGetRoleName(roleTemplateId, requestHandler, delegate(string nickName)
		{
			bool flag = label.gameObject == null;
			if (!flag)
			{
				label.text = (string.IsNullOrEmpty(nickName) ? orgConfig.GradeName.SetGradeColor((int)orgConfig.Grade) : (showBoth ? (nickName.SetGradeColor((int)orgConfig.Grade) + "<color=#lightgrey>(" + orgConfig.GradeName + ")</color>").ColorReplace() : nickName.SetGradeColor((int)orgConfig.Grade)));
				Action<string> callback2 = callback;
				if (callback2 != null)
				{
					callback2(nickName);
				}
			}
		}, false);
	}

	// Token: 0x06003E62 RID: 15970 RVA: 0x001F4BDC File Offset: 0x001F2DDC
	public static void AsyncGetRoleName(short roleTemplateId, IAsyncMethodRequestHandler requestHandler = null, Action<string> callback = null, bool fallbackToConfig = true)
	{
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleNpcNickName(requestHandler, roleTemplateId, delegate(int offset, RawDataPool pool)
		{
			string nickName = null;
			Serializer.Deserialize(pool, offset, ref nickName);
			bool flag = nickName == null & fallbackToConfig;
			if (flag)
			{
				nickName = OrganizationMember.Instance[VillagerRole.Instance[roleTemplateId].OrganizationMember].GradeName;
			}
			Action<string> callback2 = callback;
			if (callback2 != null)
			{
				callback2(nickName);
			}
		});
	}

	// Token: 0x06003E63 RID: 15971 RVA: 0x001F4C20 File Offset: 0x001F2E20
	public static string GetRoleOriginalNameWithGrade(short roleTemplateId)
	{
		short orgMember = VillagerRole.Instance[roleTemplateId].OrganizationMember;
		OrganizationMemberItem organizationMemberItem = OrganizationMember.Instance[orgMember];
		return organizationMemberItem.GradeName.SetGradeColor((int)organizationMemberItem.Grade);
	}

	// Token: 0x06003E64 RID: 15972 RVA: 0x001F4C60 File Offset: 0x001F2E60
	public static void RefreshWorkingIcon(CImage icon, int charId, short arrangementTemplateId, short roleId)
	{
		TooltipInvoker mouseTipDisplayer = icon.GetComponent<TooltipInvoker>();
		bool flag = mouseTipDisplayer != null;
		if (flag)
		{
			mouseTipDisplayer.enabled = false;
		}
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		bool flag2 = roleId != -1;
		if (flag2)
		{
			icon.gameObject.SetActive(true);
			icon.SetSprite(string.Format("sp_icon_roletype_{0}", roleId), false, null);
			VillagerWorkData workingData;
			bool flag3 = mouseTipDisplayer != null && buildingModel.VillagerWork.TryGetValue(charId, out workingData);
			if (flag3)
			{
				WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
				string locationName = worldMapModel.GetBlockName(workingData.AreaId, workingData.BlockId, workingData.BlockTemplateId, -1);
				mouseTipDisplayer.enabled = true;
				TooltipInvoker tooltipInvoker = mouseTipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTipDisplayer.RuntimeParam.Set("arg0", (workingData.WorkType >= 0) ? LocalStringManager.GetFormat(LanguageKey.LK_Villager_WorkStatus_Working, locationName, LocalStringManager.Get(string.Format("LK_WorkType_{0}", workingData.WorkType))) : LocalStringManager.Get(LanguageKey.LK_Tips_Idle));
			}
		}
		else
		{
			VillagerWorkData workingData2;
			bool flag4 = buildingModel.VillagerWork.TryGetValue(charId, out workingData2) && (workingData2.WorkType == 0 || workingData2.WorkType == 1 || workingData2.WorkType >= 10);
			if (flag4)
			{
				icon.gameObject.SetActive(true);
				icon.SetSprite("sp_icon_renwuzhuangtai_0", false, null);
				WorldMapModel worldMapModel2 = SingletonObject.getInstance<WorldMapModel>();
				string locationName2 = worldMapModel2.GetBlockName(workingData2.AreaId, workingData2.BlockId, workingData2.BlockTemplateId, -1);
				string workTypeString = LocalStringManager.Get(string.Format("LK_WorkType_{0}", workingData2.WorkType));
				bool flag5 = mouseTipDisplayer != null;
				if (flag5)
				{
					mouseTipDisplayer.enabled = true;
					TooltipInvoker tooltipInvoker = mouseTipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Villager_WorkStatus_Working, locationName2, workTypeString));
				}
			}
			else
			{
				bool flag6 = roleId >= 0;
				if (flag6)
				{
					icon.gameObject.SetActive(true);
					VillagerRoleItem roleConfig = VillagerRole.Instance[roleId];
					icon.SetSprite(roleConfig.IdleIcon, false, null);
					bool flag7 = mouseTipDisplayer != null;
					if (flag7)
					{
						mouseTipDisplayer.enabled = true;
						TooltipInvoker tooltipInvoker = mouseTipDisplayer;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						mouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Tips_Idle));
					}
				}
				else
				{
					icon.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06003E65 RID: 15973 RVA: 0x001F4F14 File Offset: 0x001F3114
	public static void RefreshWorkingIcon(CImage icon, int charId, short arrangementTemplateId, VillagerRoleCharacterDisplayData displayData)
	{
		TooltipInvoker mouseTipDisplayer = icon.GetComponent<TooltipInvoker>();
		bool flag = mouseTipDisplayer != null;
		if (flag)
		{
			mouseTipDisplayer.enabled = false;
		}
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		VillagerWorkData workData;
		buildingModel.VillagerWork.TryGetValue(charId, out workData);
		bool flag2 = arrangementTemplateId <= 0 && (workData == null || workData.WorkType == 13);
		if (flag2)
		{
			icon.gameObject.SetActive(false);
		}
		else
		{
			icon.gameObject.SetActive(true);
			icon.SetSprite("sp_icon_renwuzhuangtai_0", false, null);
			bool flag3 = mouseTipDisplayer != null;
			if (flag3)
			{
				mouseTipDisplayer.Type = TipType.WorkingStatus;
				mouseTipDisplayer.enabled = true;
				TooltipInvoker tooltipInvoker = mouseTipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTipDisplayer.RuntimeParam.Set("charId", charId);
				mouseTipDisplayer.RuntimeParam.Set("arrangementTemplateId", arrangementTemplateId);
				mouseTipDisplayer.RuntimeParam.Set<VillagerRoleCharacterDisplayData>("displayData", displayData);
			}
		}
	}

	// Token: 0x06003E66 RID: 15974 RVA: 0x001F5014 File Offset: 0x001F3214
	public static void RefreshRoleIcon(CImage icon, short roleId)
	{
		TooltipInvoker mouseTipDisplayer = icon.GetComponent<TooltipInvoker>();
		bool flag = mouseTipDisplayer != null;
		if (flag)
		{
			mouseTipDisplayer.enabled = false;
		}
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		bool flag2 = roleId >= 0;
		if (flag2)
		{
			icon.gameObject.SetActive(true);
			VillagerRoleItem roleConfig = VillagerRole.Instance[roleId];
			icon.SetSprite(string.Format("sp_icon_roletype_{0}", roleId), false, null);
			bool flag3 = mouseTipDisplayer != null;
			if (flag3)
			{
				mouseTipDisplayer.enabled = true;
				mouseTipDisplayer.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = mouseTipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_VillagerSelection_Rolename, OrganizationMember.Instance[roleConfig.OrganizationMember].GradeName));
			}
		}
		else
		{
			icon.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003E67 RID: 15975 RVA: 0x001F5100 File Offset: 0x001F3300
	public static void AsyncRefreshWorkingIcon(CImage icon, int charId, IAsyncMethodRequestHandler requestHandler = null)
	{
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterSlimDisplayData(requestHandler, charId, delegate(int offset, RawDataPool pool)
		{
			VillagerRoleCharacterSlimDisplayData displayData = new VillagerRoleCharacterSlimDisplayData();
			Serializer.Deserialize(pool, offset, ref displayData);
			bool flag = displayData.RoleTemplateId >= 0;
			if (flag)
			{
				VillagerRoleUtils.RefreshRoleIcon(icon, displayData.RoleTemplateId);
			}
			else
			{
				VillagerRoleUtils.RefreshWorkingIcon(icon, charId, displayData.ArrangementTemplateId, displayData.RoleTemplateId);
			}
		});
	}

	// Token: 0x06003E68 RID: 15976 RVA: 0x001F513C File Offset: 0x001F333C
	public static void ConfirmAndAssignRole(int villagerCharId, short roleTemplateId, Action onConfirm = null, Action<bool> onAssign = null, IAsyncMethodRequestHandler requestHandler = null)
	{
		VillagerRoleUtils.<>c__DisplayClass11_0 CS$<>8__locals1 = new VillagerRoleUtils.<>c__DisplayClass11_0();
		CS$<>8__locals1.roleTemplateId = roleTemplateId;
		CS$<>8__locals1.onConfirm = onConfirm;
		CS$<>8__locals1.requestHandler = requestHandler;
		CS$<>8__locals1.villagerCharId = villagerCharId;
		CS$<>8__locals1.onAssign = onAssign;
		bool flag = CS$<>8__locals1.roleTemplateId == -1;
		if (flag)
		{
			CS$<>8__locals1.<ConfirmAndAssignRole>g__ConfirmCancelRole|1();
		}
		else
		{
			TaiwuDomainMethod.AsyncCall.GetAllVillagerRoleDisplayData(CS$<>8__locals1.requestHandler, delegate(int offset, RawDataPool pool)
			{
				List<VillagerRoleManageDisplayData> roleManageDisplayList = EasyPool.Get<List<VillagerRoleManageDisplayData>>();
				Serializer.Deserialize(pool, offset, ref roleManageDisplayList);
				List<int> characterIds = roleManageDisplayList[(int)CS$<>8__locals1.roleTemplateId].CharacterIds;
				int villagerCount = (characterIds != null) ? characterIds.Count : 0;
				base.<ConfirmAndAssignRole>g__ConfirmAssign|2(villagerCount);
			});
		}
	}
}
