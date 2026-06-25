using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class InformationSort : MonoBehaviour, ISetInformationSortOrFilterStyle
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06002253 RID: 8787 RVA: 0x000FE68C File Offset: 0x000FC88C
	// (remove) Token: 0x06002254 RID: 8788 RVA: 0x000FE6C4 File Offset: 0x000FC8C4
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnSortSettingsChangeEvent;

	// Token: 0x06002255 RID: 8789 RVA: 0x000FE6FC File Offset: 0x000FC8FC
	private void Awake()
	{
		CButtonObsolete gradeButtton = this.GradeButtton;
		if (gradeButtton != null)
		{
			gradeButtton.ClearAndAddListener(new Action(this.OnGradeButtonClick));
		}
		CButtonObsolete timeButton = this.TimeButton;
		if (timeButton != null)
		{
			timeButton.ClearAndAddListener(new Action(this.OnTimeButtonClick));
		}
		this.UpdateGradeButtonView();
		this.UpdateTimeButtonView();
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000FE754 File Offset: 0x000FC954
	private void OnDestroy()
	{
		this.OnSortSettingsChangeEvent = null;
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000FE760 File Offset: 0x000FC960
	private void OnGradeButtonClick()
	{
		bool flag = this._settings[InformationSort.SortType.Grade] == 0;
		if (flag)
		{
			this._settings[InformationSort.SortType.Grade] = 1;
		}
		else
		{
			bool flag2 = this._settings[InformationSort.SortType.Grade] == 1;
			if (flag2)
			{
				this._settings[InformationSort.SortType.Grade] = -1;
			}
			else
			{
				bool flag3 = this._settings[InformationSort.SortType.Grade] == -1;
				if (flag3)
				{
					this._settings[InformationSort.SortType.Grade] = 0;
				}
			}
		}
		this.UpdateGradeButtonView();
		Action onSortSettingsChangeEvent = this.OnSortSettingsChangeEvent;
		if (onSortSettingsChangeEvent != null)
		{
			onSortSettingsChangeEvent();
		}
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000FE7F0 File Offset: 0x000FC9F0
	private void OnTimeButtonClick()
	{
		bool flag = this._settings[InformationSort.SortType.Time] == 0;
		if (flag)
		{
			this._settings[InformationSort.SortType.Time] = 1;
		}
		else
		{
			bool flag2 = this._settings[InformationSort.SortType.Time] == 1;
			if (flag2)
			{
				this._settings[InformationSort.SortType.Time] = -1;
			}
			else
			{
				bool flag3 = this._settings[InformationSort.SortType.Time] == -1;
				if (flag3)
				{
					this._settings[InformationSort.SortType.Time] = 0;
				}
			}
		}
		this.UpdateTimeButtonView();
		Action onSortSettingsChangeEvent = this.OnSortSettingsChangeEvent;
		if (onSortSettingsChangeEvent != null)
		{
			onSortSettingsChangeEvent();
		}
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000FE880 File Offset: 0x000FCA80
	private void UpdateGradeButtonView()
	{
		int code = this._settings[InformationSort.SortType.Grade];
		this.GradeArrow.localScale = new Vector3(1f, (float)code, 1f);
		this.GradeCheckMark.SetActive(code != 0);
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000FE8C8 File Offset: 0x000FCAC8
	private void UpdateTimeButtonView()
	{
		int code = this._settings[InformationSort.SortType.Time];
		this.TimeArrow.localScale = new Vector3(1f, (float)code, 1f);
		this.TimeCheckMark.SetActive(code != 0);
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000FE910 File Offset: 0x000FCB10
	public void RegisterOnSortSettingsUpdateHandler(Action handler)
	{
		this.OnSortSettingsChangeEvent -= handler;
		this.OnSortSettingsChangeEvent += handler;
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000FE924 File Offset: 0x000FCB24
	public Dictionary<InformationSort.SortType, int> GetSortConfig()
	{
		return this._settings;
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x000FE93C File Offset: 0x000FCB3C
	public void SetStyle(bool isSecret)
	{
		this.GradeButtton.gameObject.SetActive(!isSecret);
		this.TimeButton.gameObject.SetActive(isSecret);
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000FE966 File Offset: 0x000FCB66
	public void SortNormalInformation(List<NormalInformation> normalInformationList)
	{
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000FE96C File Offset: 0x000FCB6C
	public void SortSecretInformation(List<SecretInformationDisplayData> secretList)
	{
		bool flag = secretList == null || secretList.Count == 0;
		if (flag)
		{
		}
	}

	// Token: 0x04001A6E RID: 6766
	public CButtonObsolete GradeButtton;

	// Token: 0x04001A6F RID: 6767
	public RectTransform GradeArrow;

	// Token: 0x04001A70 RID: 6768
	public TextMeshProUGUI GradeIndex;

	// Token: 0x04001A71 RID: 6769
	public GameObject GradeCheckMark;

	// Token: 0x04001A72 RID: 6770
	public CButtonObsolete TimeButton;

	// Token: 0x04001A73 RID: 6771
	public RectTransform TimeArrow;

	// Token: 0x04001A74 RID: 6772
	public TextMeshProUGUI TimeIndex;

	// Token: 0x04001A75 RID: 6773
	public GameObject TimeCheckMark;

	// Token: 0x04001A76 RID: 6774
	private Dictionary<InformationSort.SortType, int> _settings = new Dictionary<InformationSort.SortType, int>
	{
		{
			InformationSort.SortType.Time,
			0
		},
		{
			InformationSort.SortType.Grade,
			0
		}
	};

	// Token: 0x020014F0 RID: 5360
	public enum SortType
	{
		// Token: 0x0400A309 RID: 41737
		Invalid = -1,
		// Token: 0x0400A30A RID: 41738
		Time,
		// Token: 0x0400A30B RID: 41739
		Grade,
		// Token: 0x0400A30C RID: 41740
		Count
	}
}
