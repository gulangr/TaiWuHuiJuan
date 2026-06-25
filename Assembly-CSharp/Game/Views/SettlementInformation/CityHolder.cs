using System;
using Config;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementInformation
{
	// Token: 0x0200078B RID: 1931
	public class CityHolder : MonoBehaviour
	{
		// Token: 0x06005DBA RID: 23994 RVA: 0x002B1CC4 File Offset: 0x002AFEC4
		public void SetLocation(sbyte stateTemplateId, short areaTemplateId)
		{
			this.title.text = MapState.Instance[stateTemplateId].Name + " " + MapArea.Instance[areaTemplateId].Name;
			this._activeChildCount = 0;
			foreach (object obj in this.cities)
			{
				Transform child = (Transform)obj;
				child.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005DBB RID: 23995 RVA: 0x002B1D64 File Offset: 0x002AFF64
		public SettlementHolder AddSettlement(SettlementDisplayData settlement)
		{
			SettlementHolder item = (this._activeChildCount < this.cities.childCount) ? this.cities.GetChild(this._activeChildCount).GetComponent<SettlementHolder>() : Object.Instantiate<SettlementHolder>(this.template, this.cities);
			this._activeChildCount++;
			item.SetData(settlement);
			return item;
		}

		// Token: 0x06005DBC RID: 23996 RVA: 0x002B1DCC File Offset: 0x002AFFCC
		public void SwitchOff(int exceptSettlementId)
		{
			foreach (SettlementHolder var in this.cities.GetComponentsInChildren<SettlementHolder>(true))
			{
				var.SwitchOff(exceptSettlementId);
			}
		}

		// Token: 0x06005DBD RID: 23997 RVA: 0x002B1E04 File Offset: 0x002B0004
		public void SwitchOn(int settlementId)
		{
			foreach (SettlementHolder var in this.cities.GetComponentsInChildren<SettlementHolder>(true))
			{
				var.SwitchOn(settlementId);
			}
		}

		// Token: 0x04004087 RID: 16519
		[SerializeField]
		private SettlementHolder template;

		// Token: 0x04004088 RID: 16520
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004089 RID: 16521
		[SerializeField]
		private RectTransform cities;

		// Token: 0x0400408A RID: 16522
		private int _activeChildCount;
	}
}
