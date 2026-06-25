using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Information;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class SecretInformationFilter : MonoBehaviour, ISetInformationSortOrFilterStyle
{
	// Token: 0x1400001B RID: 27
	// (add) Token: 0x0600229A RID: 8858 RVA: 0x001006B4 File Offset: 0x000FE8B4
	// (remove) Token: 0x0600229B RID: 8859 RVA: 0x001006EC File Offset: 0x000FE8EC
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnSecretInformationFilterSettingsChangeEvent;

	// Token: 0x0600229C RID: 8860 RVA: 0x00100721 File Offset: 0x000FE921
	public void SetStyle(bool isSecret)
	{
		base.gameObject.SetActive(isSecret);
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x00100731 File Offset: 0x000FE931
	public void RegisterOnSettingsChangeHandler(Action handler)
	{
		this.OnSecretInformationFilterSettingsChangeEvent -= handler;
		this.OnSecretInformationFilterSettingsChangeEvent += handler;
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x00100744 File Offset: 0x000FE944
	public void FilterSecretInformation(List<SecretInformationDisplayData> secretList)
	{
		bool flag = secretList == null || secretList.Count == 0;
		if (!flag)
		{
			CToggleObsolete activeToggle = this.ToggleGroup.GetActive();
			bool flag2 = activeToggle != null;
			if (flag2)
			{
				int type = activeToggle.Key;
			}
		}
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x00100787 File Offset: 0x000FE987
	private void Awake()
	{
		this.ToggleGroup.InitPreOnToggle(-1);
		this.ToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete _, CToggleObsolete __)
		{
			Action onSecretInformationFilterSettingsChangeEvent = this.OnSecretInformationFilterSettingsChangeEvent;
			if (onSecretInformationFilterSettingsChangeEvent != null)
			{
				onSecretInformationFilterSettingsChangeEvent();
			}
		};
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x001007AE File Offset: 0x000FE9AE
	private void OnDestroy()
	{
		this.OnSecretInformationFilterSettingsChangeEvent = null;
	}

	// Token: 0x04001A9B RID: 6811
	public CToggleGroupObsolete ToggleGroup;
}
