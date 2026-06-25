using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class SecretInformationSearch : MonoBehaviour, ISetInformationSortOrFilterStyle
{
	// Token: 0x1400001C RID: 28
	// (add) Token: 0x060022A3 RID: 8867 RVA: 0x001007D8 File Offset: 0x000FE9D8
	// (remove) Token: 0x060022A4 RID: 8868 RVA: 0x00100810 File Offset: 0x000FEA10
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnSearchSettingsChangeEvent;

	// Token: 0x060022A5 RID: 8869 RVA: 0x00100848 File Offset: 0x000FEA48
	private void Awake()
	{
		this.InputField.text = string.Empty;
		this.InputField.onEndEdit.RemoveAllListeners();
		this.InputField.onEndEdit.AddListener(delegate(string str)
		{
			Action onSearchSettingsChangeEvent = this.OnSearchSettingsChangeEvent;
			if (onSearchSettingsChangeEvent != null)
			{
				onSearchSettingsChangeEvent();
			}
		});
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x00100895 File Offset: 0x000FEA95
	private void OnEnable()
	{
		this.InputField.SetTextWithoutNotify(string.Empty);
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x001008A9 File Offset: 0x000FEAA9
	public void SetStyle(bool isSecret)
	{
		base.gameObject.SetActive(isSecret);
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x001008B9 File Offset: 0x000FEAB9
	public void RegisterOnSearchSettingsChangeHandler(Action handler)
	{
		this.OnSearchSettingsChangeEvent -= handler;
		this.OnSearchSettingsChangeEvent += handler;
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x001008CC File Offset: 0x000FEACC
	public void FilterSecretInformationBySearchSettings(List<SecretInformationDisplayData> secretList)
	{
		bool flag = secretList == null || secretList.Count == 0;
		if (flag)
		{
		}
	}

	// Token: 0x04001A9D RID: 6813
	public TMP_InputField InputField;
}
