using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000703 RID: 1795
	public class ViewConfirmProfessionSkillDialog : UIBase
	{
		// Token: 0x060054D8 RID: 21720 RVA: 0x0027599C File Offset: 0x00273B9C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._onPostConfirm = null;
			argsBox.Get<ProfessionSkillArg>("ProfessionSkillArg", out this._professionSkillArg);
			argsBox.Get<Action>("OnConfirm", out this._onConfirm);
			argsBox.Get<Action>("OnPostConfirm", out this._onPostConfirm);
			argsBox.Get("BeggarMoneyCount", out this._beggarMoneyCount);
			argsBox.Get<ItemDisplayData>("RebirthCricketItemData", out this._rebirthCricketItemData);
			this._instantlyConfirm = true;
			bool instantlyConfirm;
			bool flag = argsBox.Get("InstantlyConfirm", out instantlyConfirm);
			if (flag)
			{
				this._instantlyConfirm = instantlyConfirm;
			}
			bool flag2 = !argsBox.Get<ResourceInts>("CostResources", out this._costResources);
			if (flag2)
			{
				this._costResources.Initialize();
			}
			ProfessionData profession = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(this._professionSkillArg.ProfessionId);
			this._skillIndex = profession.GetSkillIndex(this._professionSkillArg.SkillId);
		}

		// Token: 0x060054D9 RID: 21721 RVA: 0x00275A7C File Offset: 0x00273C7C
		private void Awake()
		{
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirmClick));
			this.btnCancel.ClearAndAddListener(new Action(this.OnCancelClick));
			GEvent.Add(UiEvents.RealConfirmExecuteProfessionSkill, new GEvent.Callback(this.RealConfirmExecuteProfessionSkill));
		}

		// Token: 0x060054DA RID: 21722 RVA: 0x00275AD6 File Offset: 0x00273CD6
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.RealConfirmExecuteProfessionSkill, new GEvent.Callback(this.RealConfirmExecuteProfessionSkill));
		}

		// Token: 0x060054DB RID: 21723 RVA: 0x00275AF5 File Offset: 0x00273CF5
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
			{
				34U,
				66U
			}));
		}

		// Token: 0x060054DC RID: 21724 RVA: 0x00275B28 File Offset: 0x00273D28
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					bool flag = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0;
					if (flag)
					{
						uint subId = notification.Uid.SubId1;
						uint num = subId;
						if (num != 34U)
						{
							if (num == 66U)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
								this.Refresh();
							}
						}
						else
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
							this.Refresh();
						}
					}
				}
			}
		}

		// Token: 0x060054DD RID: 21725 RVA: 0x00275C24 File Offset: 0x00273E24
		private void Refresh()
		{
			this.Element.ShowAfterRefresh();
			ProfessionSkillItem professionSkillConfig = ProfessionSkill.Instance[this._professionSkillArg.SkillId];
			bool hsaCustom = this._costResources.GetSum() > 0;
			int remainActionPoint = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			List<ConfirmDialogCost> costList = new List<ConfirmDialogCost>();
			sbyte i;
			sbyte j;
			for (i = 0; i < 8; i = j + 1)
			{
				ResourceInfo resourceInfo = professionSkillConfig.ResourcesCost.Find((ResourceInfo r) => r.ResourceType == i && r.ResourceCount > 0);
				int resourceCount = hsaCustom ? this._costResources.Get((int)i) : resourceInfo.ResourceCount;
				bool flag = resourceCount > 0;
				if (flag)
				{
					costList.Add(new ConfirmDialogCost
					{
						Type = ViewConfirmDialog.ResourceTypeToEnum[i],
						ValueCost = resourceCount,
						ValueHave = this._resources.Get((int)i)
					});
				}
				j = i;
			}
			bool flag2 = professionSkillConfig.ExpCost > 0;
			if (flag2)
			{
				costList.Add(new ConfirmDialogCost
				{
					Type = EConfirmDialogCostType.Exp,
					ValueCost = professionSkillConfig.ExpCost,
					ValueHave = this._exp
				});
			}
			bool flag3 = professionSkillConfig.TimeCost > 0;
			if (flag3)
			{
				costList.Add(new ConfirmDialogCost
				{
					Type = EConfirmDialogCostType.ActionPoint,
					ValueCost = (int)professionSkillConfig.TimeCost,
					ValueHave = remainActionPoint
				});
			}
			bool enough = true;
			for (int index = 0; index < costList.Count; index++)
			{
				ConfirmDialogCost cost = costList[index];
				bool flag4 = cost.ValueCost > cost.ValueHave;
				if (flag4)
				{
					enough = false;
				}
				bool flag5 = index >= this.costs.childCount;
				if (flag5)
				{
					Object.Instantiate<GameObject>(this.propertyTemplate, this.costs);
				}
				Transform obj = this.costs.GetChild(index);
				obj.GetComponent<PropertyItem>().Set(ViewConfirmDialog.CostIcon[cost.Type], ViewConfirmDialog.CostName[cost.Type].Tr(), LanguageKey.LK_Make_Resource_Require_Meet.TrFormat(CommonUtils.GetDisplayStringForNum(cost.ValueHave, 100000).SetColor((cost.ValueHave >= cost.ValueCost) ? "brightblue" : "brightred"), CommonUtils.GetDisplayStringForNum(cost.ValueCost, 100000)), null, false);
				obj.gameObject.SetActive(true);
			}
			for (int index2 = costList.Count; index2 < this.costs.childCount; index2++)
			{
				this.costs.GetChild(index2).gameObject.SetActive(false);
			}
			this.content.text = LocalStringManager.GetFormat((costList.Count == 0) ? LanguageKey.LK_ProfessionSkill_Confirm_NoCost : LanguageKey.LK_ProfessionSkill_Confirm_Cost, professionSkillConfig.Name);
			this.cooldown.text = professionSkillConfig.SkillCoolDown.ToString();
			this.btnConfirm.interactable = enough;
		}

		// Token: 0x060054DE RID: 21726 RVA: 0x00275F6C File Offset: 0x0027416C
		private void OnCancelClick()
		{
			ExtraDomainMethod.Call.ConfirmExecuteSkill(this._professionSkillArg, false);
			UIManager.Instance.HideUI(UIElement.ProfessionSkillConfirm);
			GEvent.OnEvent(UiEvents.ProfessionSkillConfirmSelectCancel, null);
		}

		// Token: 0x060054DF RID: 21727 RVA: 0x00275FA0 File Offset: 0x002741A0
		private void OnConfirmClick()
		{
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
			UIManager.Instance.HideUI(UIElement.ProfessionSkillConfirm);
			bool flag = !this._instantlyConfirm;
			if (!flag)
			{
				this.Confirm();
			}
		}

		// Token: 0x060054E0 RID: 21728 RVA: 0x00275FE6 File Offset: 0x002741E6
		private void Confirm()
		{
			ProfessionSkillController.ExecuteSkillDirect(this._professionSkillArg, this._skillIndex, this._beggarMoneyCount, this._rebirthCricketItemData, this._onPostConfirm);
		}

		// Token: 0x060054E1 RID: 21729 RVA: 0x0027600C File Offset: 0x0027420C
		public override void QuickHide()
		{
			base.QuickHide();
			this.OnCancelClick();
		}

		// Token: 0x060054E2 RID: 21730 RVA: 0x00276020 File Offset: 0x00274220
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.btnConfirm.interactable;
			if (flag)
			{
				this.OnConfirmClick();
			}
			else
			{
				bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					this.QuickHide();
				}
			}
		}

		// Token: 0x060054E3 RID: 21731 RVA: 0x00276084 File Offset: 0x00274284
		private void RealConfirmExecuteProfessionSkill(ArgumentBox _)
		{
			this.Confirm();
		}

		// Token: 0x040039B2 RID: 14770
		public Transform costs;

		// Token: 0x040039B3 RID: 14771
		public GameObject propertyTemplate;

		// Token: 0x040039B4 RID: 14772
		public TextMeshProUGUI content;

		// Token: 0x040039B5 RID: 14773
		public TextMeshProUGUI cooldown;

		// Token: 0x040039B6 RID: 14774
		public CButton btnConfirm;

		// Token: 0x040039B7 RID: 14775
		public CButton btnCancel;

		// Token: 0x040039B8 RID: 14776
		private int _skillIndex;

		// Token: 0x040039B9 RID: 14777
		private int _beggarMoneyCount;

		// Token: 0x040039BA RID: 14778
		private ItemDisplayData _rebirthCricketItemData;

		// Token: 0x040039BB RID: 14779
		private ProfessionSkillArg _professionSkillArg;

		// Token: 0x040039BC RID: 14780
		private Action _onConfirm;

		// Token: 0x040039BD RID: 14781
		private Action _onPostConfirm;

		// Token: 0x040039BE RID: 14782
		private ResourceInts _resources;

		// Token: 0x040039BF RID: 14783
		private ResourceInts _costResources;

		// Token: 0x040039C0 RID: 14784
		private int _exp;

		// Token: 0x040039C1 RID: 14785
		private bool _instantlyConfirm = true;
	}
}
