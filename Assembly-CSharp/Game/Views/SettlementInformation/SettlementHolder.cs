using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementInformation
{
	// Token: 0x0200078D RID: 1933
	public class SettlementHolder : MonoBehaviour
	{
		// Token: 0x06005DCA RID: 24010 RVA: 0x002B213C File Offset: 0x002B033C
		public void SetData(SettlementDisplayData settlement)
		{
			this.SettlementId = settlement.SettlementId;
			this.title.text = ((settlement.RandomNameId != -1) ? LocalTownNames.Instance.TownNameCore[(int)settlement.RandomNameId].Name : Organization.Instance[settlement.OrgTemplateId].Name);
			OrganizationItem organizationItem = Organization.Instance[settlement.OrgTemplateId];
			bool isSect = organizationItem != null && organizationItem.IsSect;
			this.icon.SetSprite(Organization.Instance[settlement.OrgTemplateId].Icon, false, null);
			sbyte stateTaskStatus = isSect ? SingletonObject.getInstance<BuildingModel>().GetAreaTaskStatus((int)settlement.AreaTemplateId) : 0;
			this.bloom.SetActive(stateTaskStatus == 1);
			this.decay.SetActive(stateTaskStatus == 2);
		}

		// Token: 0x06005DCB RID: 24011 RVA: 0x002B2211 File Offset: 0x002B0411
		public void SetBloomDecay(bool isBloom)
		{
			this.bloom.SetActive(isBloom);
			this.decay.SetActive(!isBloom);
		}

		// Token: 0x06005DCC RID: 24012 RVA: 0x002B2231 File Offset: 0x002B0431
		public void SwitchOff(int exceptId)
		{
			this.toggle.isOn = (exceptId == this.SettlementId);
		}

		// Token: 0x06005DCD RID: 24013 RVA: 0x002B224C File Offset: 0x002B044C
		public void SwitchOn(int id)
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				this.toggle.isOn = (id == this.SettlementId);
			});
		}

		// Token: 0x04004096 RID: 16534
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004097 RID: 16535
		[SerializeField]
		internal CToggle toggle;

		// Token: 0x04004098 RID: 16536
		[SerializeField]
		internal GameObject bloom;

		// Token: 0x04004099 RID: 16537
		[SerializeField]
		internal GameObject decay;

		// Token: 0x0400409A RID: 16538
		[SerializeField]
		internal CImage icon;

		// Token: 0x0400409B RID: 16539
		internal int SettlementId;
	}
}
