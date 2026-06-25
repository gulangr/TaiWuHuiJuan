using System;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x020001ED RID: 493
public class CombatSkillViewMasteredHelper
{
	// Token: 0x1700033E RID: 830
	// (get) Token: 0x0600204A RID: 8266 RVA: 0x000EB5D3 File Offset: 0x000E97D3
	// (set) Token: 0x0600204B RID: 8267 RVA: 0x000EB5DB File Offset: 0x000E97DB
	public CombatSkillDisplayData DisplayData { get; private set; }

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x0600204C RID: 8268 RVA: 0x000EB5E4 File Offset: 0x000E97E4
	public bool CanOperate
	{
		get
		{
			return this.DisplayData != null && (this.DisplayData.Mastered ? (this.DisplayData.GridCount >= 1) : (this.DisplayData.GridCount >= 2)) && (SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(this.DisplayData.CharId) || SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this.DisplayData.CharId)) && this.DisplayData.TemplateId >= 0;
		}
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x000EB66E File Offset: 0x000E986E
	public void SetData(CombatSkillDisplayData displayData)
	{
		this.DisplayData = displayData;
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000EB67C File Offset: 0x000E987C
	public void DoRequestMasteredAppend()
	{
		bool flag = !this.CanOperate || CombatSkillViewMasteredHelper.DisableMethodCall;
		if (!flag)
		{
			GEvent.OnEvent(UiEvents.SentChangeCharacterMasteredCombatSkill, null);
			CommandManager.AddCommandMethodCall<int, short>(EPriority.CallMasteredAddRemove, 19, 37, this.DisplayData.CharId, this.DisplayData.TemplateId, new CallMethodRespHandler(this.HandlerAddCharacterMasteredCombatSkill), null);
		}
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000EB6E0 File Offset: 0x000E98E0
	public void DoRequestMasteredRemove()
	{
		bool flag = !this.CanOperate || CombatSkillViewMasteredHelper.DisableMethodCall;
		if (!flag)
		{
			GEvent.OnEvent(UiEvents.SentChangeCharacterMasteredCombatSkill, null);
			CommandManager.AddCommandMethodCall<int, short>(EPriority.CallMasteredAddRemove, 19, 38, this.DisplayData.CharId, this.DisplayData.TemplateId, new CallMethodRespHandler(this.HandlerRemoveCharacterMasteredCombatSkill), null);
		}
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x000EB744 File Offset: 0x000E9944
	public void OnClickChangeMastered()
	{
		bool flag = !this.CanOperate || CombatSkillViewMasteredHelper.DisableMethodCall;
		if (!flag)
		{
			bool isRemove = this.DisplayData.Mastered;
			bool previewMastered = this.DisplayData.PreviewMastered;
			if (previewMastered)
			{
				isRemove = !isRemove;
			}
			bool flag2 = isRemove;
			if (flag2)
			{
				this.DoRequestMasteredRemove();
			}
			else
			{
				bool flag3 = CombatSkill.Instance[this.DisplayData.TemplateId].EquipType == 0;
				if (flag3)
				{
					CommandManager.AddCommandMethodCall<short>(EPriority.CallMasteredAddRemove, 5, 74, this.DisplayData.TemplateId, new CallMethodRespHandler(this.HandlerMasteredSkillWillChangePlan), null);
				}
				else
				{
					this.DoRequestMasteredAppend();
				}
			}
		}
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x000EB7E8 File Offset: 0x000E99E8
	private void ChangeMasteredStatus(bool mastered)
	{
		bool flag = this.DisplayData == null;
		if (!flag)
		{
			bool flag2 = !this.DisplayData.PreviewMastered;
			if (flag2)
			{
				CombatSkillDisplayData displayData = this.DisplayData;
				displayData.GridCount += (mastered ? -1 : 1);
				this.DisplayData.Mastered = mastered;
			}
			Action<CombatSkillDisplayData> onMasteredStatusChanged = this.OnMasteredStatusChanged;
			if (onMasteredStatusChanged != null)
			{
				onMasteredStatusChanged(this.DisplayData);
			}
			GEvent.OnEvent(UiEvents.UpdateCombatSkillDisplayDataMastered, EasyPool.Get<ArgumentBox>().SetObject("displayData", this.DisplayData));
		}
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000EB880 File Offset: 0x000E9A80
	private void HandlerAddCharacterMasteredCombatSkill(int offset, RawDataPool pool)
	{
		bool success = false;
		Serializer.Deserialize(pool, offset, ref success);
		bool flag = !success;
		if (!flag)
		{
			this.ChangeMasteredStatus(true);
		}
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000EB8AC File Offset: 0x000E9AAC
	private void HandlerRemoveCharacterMasteredCombatSkill(int offset, RawDataPool pool)
	{
		bool success = false;
		Serializer.Deserialize(pool, offset, ref success);
		bool flag = !success;
		if (!flag)
		{
			this.ChangeMasteredStatus(false);
		}
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000EB8D8 File Offset: 0x000E9AD8
	private void HandlerMasteredSkillWillChangePlan(int offset, RawDataPool pool)
	{
		bool willChange = false;
		Serializer.Deserialize(pool, offset, ref willChange);
		bool flag = willChange;
		if (flag)
		{
			this.DoRequestMasteredAppend();
		}
		else
		{
			this.DoRequestMasteredAppend();
		}
	}

	// Token: 0x0400184C RID: 6220
	public Action<CombatSkillDisplayData> OnMasteredStatusChanged;

	// Token: 0x0400184E RID: 6222
	public static bool DisableMethodCall;
}
