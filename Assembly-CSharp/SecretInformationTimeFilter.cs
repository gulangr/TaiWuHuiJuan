using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class SecretInformationTimeFilter : MonoBehaviour, ISetInformationSortOrFilterStyle
{
	// Token: 0x1400001D RID: 29
	// (add) Token: 0x060022AC RID: 8876 RVA: 0x0010090C File Offset: 0x000FEB0C
	// (remove) Token: 0x060022AD RID: 8877 RVA: 0x00100944 File Offset: 0x000FEB44
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnSelectDateChangeEvent;

	// Token: 0x060022AE RID: 8878 RVA: 0x0010097C File Offset: 0x000FEB7C
	private void Awake()
	{
		bool awakeFlag = this._awakeFlag;
		if (!awakeFlag)
		{
			this.Controller.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetItemSelectState);
			this.Controller.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshItem);
			this.Controller.RegisterOnSelectIndexChangeHandler(delegate(int index)
			{
				Action onSelectDateChangeEvent = this.OnSelectDateChangeEvent;
				if (onSelectDateChangeEvent != null)
				{
					onSelectDateChangeEvent();
				}
			});
			this._awakeFlag = true;
		}
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x001009E3 File Offset: 0x000FEBE3
	public void SetStyle(bool isSecret)
	{
		base.gameObject.SetActive(isSecret);
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x001009F3 File Offset: 0x000FEBF3
	public void RegisterOnSettingsChangeHandler(Action handler)
	{
		this.OnSelectDateChangeEvent -= handler;
		this.OnSelectDateChangeEvent += handler;
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x00100A08 File Offset: 0x000FEC08
	public void UpdateDateListFromSecretInformationList(List<SecretInformationDisplayData> secretList)
	{
		bool flag = !this._awakeFlag;
		if (flag)
		{
			this.Awake();
		}
		HashSet<int> existDates = new HashSet<int>();
		bool flag2 = secretList != null;
		if (flag2)
		{
			foreach (SecretInformationDisplayData data in secretList)
			{
				bool flag3 = this.InformationFilter != null;
				if (flag3)
				{
					bool isDataBroadcast = data.HolderCount <= 0;
					bool isNeedBroadcast = this.InformationFilter != null && this.InformationFilter.ToggleBroadcastSecret.gameObject.activeSelf && this.InformationFilter.ToggleBroadcastSecret.isOn;
					bool flag4 = isDataBroadcast != isNeedBroadcast;
					if (flag4)
					{
						continue;
					}
				}
				int date = data.OccurenceDate;
				existDates.Add(date);
			}
		}
		bool flag5 = this._secretInformationDateList == null;
		if (flag5)
		{
			this._secretInformationDateList = new List<int>();
		}
		this._secretInformationDateList.Clear();
		this._secretInformationDateList.AddRange(existDates);
		this._secretInformationDateList.Sort();
		this.Controller.InitPageCount(this._secretInformationDateList.Count, this._secretInformationDateList.Count - 1, false);
		bool flag6 = this._secretInformationDateList.Count > 0;
		if (flag6)
		{
			this.Controller.SetSelect(0, true);
		}
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x00100B88 File Offset: 0x000FED88
	public void FilterSecretInformation(List<SecretInformationDisplayData> secretList)
	{
		bool flag = secretList.Count <= 0 || !this._secretInformationDateList.CheckIndex(this.Controller.CurPageIndex);
		if (!flag)
		{
			int date = this._secretInformationDateList[this.Controller.CurPageIndex];
		}
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x00100BD8 File Offset: 0x000FEDD8
	private void SetItemSelectState(Refers refers, bool isSecret)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = isSecret;
		refers.CGet<CToggleObsolete>("Toggle").interactable = !isSecret;
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x00100C04 File Offset: 0x000FEE04
	private void RefreshItem(int index, Refers refers)
	{
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.Controller.SetSelect(index, true);
			}
		});
		int date = this._secretInformationDateList[index];
		refers.CGet<TextMeshProUGUI>("Label").text = SingletonObject.getInstance<TimeManager>().GetDateDisplayContent(date);
	}

	// Token: 0x04001A9F RID: 6815
	public HorizontalPageSwitchController Controller;

	// Token: 0x04001AA0 RID: 6816
	public InformationFilter InformationFilter;

	// Token: 0x04001AA2 RID: 6818
	private List<int> _secretInformationDateList;

	// Token: 0x04001AA3 RID: 6819
	private bool _awakeFlag;
}
