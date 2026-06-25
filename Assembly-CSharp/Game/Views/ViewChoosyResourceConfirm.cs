using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006F7 RID: 1783
	public class ViewChoosyResourceConfirm : UIBase
	{
		// Token: 0x060054A3 RID: 21667 RVA: 0x00273B2C File Offset: 0x00271D2C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<int[]>("needResources", out this._needResources);
			argsBox.Get<Action>("onConfirm", out this._onConfirm);
			this.NeedWaitData = true;
		}

		// Token: 0x060054A4 RID: 21668 RVA: 0x00273B5A File Offset: 0x00271D5A
		private void Awake()
		{
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnConfirmClick));
			this.buttonCancel.ClearAndAddListener(new Action(this.OnCancelClick));
		}

		// Token: 0x060054A5 RID: 21669 RVA: 0x00273B8D File Offset: 0x00271D8D
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
			{
				34U
			}));
		}

		// Token: 0x060054A6 RID: 21670 RVA: 0x00273BBC File Offset: 0x00271DBC
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
						if (num == 34U)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
							this.Refresh();
						}
					}
				}
			}
		}

		// Token: 0x060054A7 RID: 21671 RVA: 0x00273C8C File Offset: 0x00271E8C
		private void Refresh()
		{
			int resultCount = 0;
			for (int i = 0; i < 6; i++)
			{
				PropertyItem child = this.propertyItemList[i];
				int needResource = this._needResources[i];
				ResourceTypeItem config = Config.ResourceType.Instance[i];
				bool flag = needResource > 0;
				if (flag)
				{
					resultCount += needResource / GlobalConfig.Instance.ChoosyResourceBaseCost;
					child.gameObject.SetActive(true);
					string value = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._resources.Get(i), 100000).SetColor("brightblue"), CommonUtils.GetDisplayStringForNum(needResource, 100000));
					child.Set(config.Icon, config.Name, value, null, false);
				}
				else
				{
					child.gameObject.SetActive(false);
				}
			}
			this.textConfirm.text = LanguageKey.LK_Resource_Choosy_Confirm_Result.TrFormat(resultCount.ToString().SetColor("brightblue"));
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060054A8 RID: 21672 RVA: 0x00273D9E File Offset: 0x00271F9E
		private void OnCancelClick()
		{
			this.QuickHide();
		}

		// Token: 0x060054A9 RID: 21673 RVA: 0x00273DA8 File Offset: 0x00271FA8
		private void OnConfirmClick()
		{
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
			this.QuickHide();
		}

		// Token: 0x060054AA RID: 21674 RVA: 0x00273DC4 File Offset: 0x00271FC4
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
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

		// Token: 0x04003951 RID: 14673
		[SerializeField]
		private TextMeshProUGUI textConfirm;

		// Token: 0x04003952 RID: 14674
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04003953 RID: 14675
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04003954 RID: 14676
		[SerializeField]
		private List<PropertyItem> propertyItemList;

		// Token: 0x04003955 RID: 14677
		private ResourceInts _resources;

		// Token: 0x04003956 RID: 14678
		private int[] _needResources;

		// Token: 0x04003957 RID: 14679
		private Action _onConfirm;
	}
}
