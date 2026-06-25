using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Information;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class InformationSortAndFilter : MonoBehaviour
{
	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06002261 RID: 8801 RVA: 0x000FE9B8 File Offset: 0x000FCBB8
	// (remove) Token: 0x06002262 RID: 8802 RVA: 0x000FE9F0 File Offset: 0x000FCBF0
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnFilterNormalInformationDataChanged;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06002263 RID: 8803 RVA: 0x000FEA28 File Offset: 0x000FCC28
	// (remove) Token: 0x06002264 RID: 8804 RVA: 0x000FEA60 File Offset: 0x000FCC60
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnFilterSecretInformationDataChanged;

	// Token: 0x06002265 RID: 8805 RVA: 0x000FEA95 File Offset: 0x000FCC95
	public void RegisterNormalInformationHandler(Action handler)
	{
		this.OnFilterNormalInformationDataChanged -= handler;
		this.OnFilterNormalInformationDataChanged += handler;
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000FEAA8 File Offset: 0x000FCCA8
	public void RegisterSecretInformationHandler(Action handler)
	{
		this.OnFilterSecretInformationDataChanged -= handler;
		this.OnFilterSecretInformationDataChanged += handler;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000FEABC File Offset: 0x000FCCBC
	public void SetSrcNormalInformationList(List<NormalInformation> list)
	{
		bool flag = !this._awakeFlag;
		if (flag)
		{
			this.Awake();
		}
		this._srcNormalInformationList = list;
		bool flag2 = null != this.InformationFilter;
		if (flag2)
		{
			this.InformationFilter.SetNormalCount(list);
		}
		this.UpdateShowingNormalInformation();
		this.SetDirty();
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000FEB10 File Offset: 0x000FCD10
	public void SetSrcSecretInformationPackage(SecretInformationDisplayPackage package)
	{
		bool flag = !this._awakeFlag;
		if (flag)
		{
			this.Awake();
		}
		this._srcSecretInformationPackage = package;
		bool flag2 = null != this.InformationFilter;
		if (flag2)
		{
			this.InformationFilter.SetSecretCount(package.SecretInformationDisplayDataList);
		}
		this.RefreshSecretInformationByDate();
		this.UpdateShowingSecretInformation();
		this.SetDirty();
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000FEB70 File Offset: 0x000FCD70
	public void RefreshSecretInformationByDate()
	{
		bool flag = null != this.SecretInformationTimeFilter && this._srcSecretInformationPackage != null;
		if (flag)
		{
			this.SecretInformationTimeFilter.UpdateDateListFromSecretInformationList(this._srcSecretInformationPackage.SecretInformationDisplayDataList);
		}
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000FEBB4 File Offset: 0x000FCDB4
	public void SetIsShowSecret(bool flag)
	{
		this._isShowingSecret = flag;
		bool flag2 = null != this.InformationSort;
		if (flag2)
		{
			this.InformationSort.SetStyle(flag);
		}
		bool flag3 = null != this.InformationFilter;
		if (flag3)
		{
			this.InformationFilter.SetStyle(flag);
		}
		bool flag4 = null != this.NormalInformationFilter;
		if (flag4)
		{
			this.NormalInformationFilter.SetStyle(flag);
		}
		bool flag5 = null != this.SecretInformationFilter;
		if (flag5)
		{
			this.SecretInformationFilter.SetStyle(flag);
		}
		bool flag6 = null != this.SecretInformationTimeFilter;
		if (flag6)
		{
			this.SecretInformationTimeFilter.SetStyle(flag);
		}
		bool flag7 = null != this.SecretInformationSearch;
		if (flag7)
		{
			this.SecretInformationSearch.SetStyle(flag);
		}
		this.SetDirty();
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000FEC84 File Offset: 0x000FCE84
	public void SetInformationFilterStyle(bool[] flags)
	{
		bool flag = null != this.NormalInformationFilter && NormalInformationFilter.PresetFilterMode.LiteratiSkill3 == this.NormalInformationFilter.PresetFilter;
		if (flag)
		{
			flags[4] = false;
			flags[5] = false;
		}
		bool flag2 = null != this.InformationFilter;
		if (flag2)
		{
			this.InformationFilter.SetTogglesVisibleState(flags);
		}
		this.RefreshSecretInformationByDate();
		this.SetDirty();
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000FECEC File Offset: 0x000FCEEC
	public int GetShowingNormalCount()
	{
		return this._showingNormalList.Count;
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000FED0C File Offset: 0x000FCF0C
	public int GetShowingSecretCount()
	{
		return this._showingSecretList.Count;
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000FED2C File Offset: 0x000FCF2C
	public NormalInformation GetNormalInformationAtIndex(int index)
	{
		bool flag = this._showingNormalList.CheckIndex(index);
		if (flag)
		{
			return this._showingNormalList[index];
		}
		throw new IndexOutOfRangeException();
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000FED60 File Offset: 0x000FCF60
	public SecretInformationDisplayData GetSecretInformationAtIndex(int index)
	{
		bool flag = this._showingSecretList.CheckIndex(index);
		if (flag)
		{
			return this._showingSecretList[index];
		}
		throw new IndexOutOfRangeException();
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000FED94 File Offset: 0x000FCF94
	public SecretInformationDisplayPackage GetSecretInformationDisplayPackage()
	{
		return this._srcSecretInformationPackage;
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000FEDAC File Offset: 0x000FCFAC
	public void SetDirty()
	{
		this._dataDirtyFlag = true;
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000FEDB8 File Offset: 0x000FCFB8
	private void UpdateShowingNormalInformation()
	{
		bool isShowingSecret = this._isShowingSecret;
		if (isShowingSecret)
		{
			List<NormalInformation> showingNormalList = this._showingNormalList;
			if (showingNormalList != null)
			{
				showingNormalList.Clear();
			}
		}
		else
		{
			sbyte informationType = -1;
			bool flag = null != this.InformationFilter;
			if (flag)
			{
				this._showingNormalList = this.InformationFilter.FilterNormalInformation(this._srcNormalInformationList, out informationType);
			}
			else
			{
				bool flag2 = this._showingNormalList == null;
				if (flag2)
				{
					this._showingNormalList = new List<NormalInformation>();
				}
				this._showingNormalList.AddRange(this._srcNormalInformationList);
			}
			bool flag3 = null != this.NormalInformationFilter;
			if (flag3)
			{
				this.NormalInformationFilter.FilterNormalInformation(this._showingNormalList, informationType);
			}
			bool flag4 = null != this.InformationSort;
			if (flag4)
			{
				this.InformationSort.SortNormalInformation(this._showingNormalList);
			}
			Action onFilterNormalInformationDataChanged = this.OnFilterNormalInformationDataChanged;
			if (onFilterNormalInformationDataChanged != null)
			{
				onFilterNormalInformationDataChanged();
			}
		}
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x000FEE9C File Offset: 0x000FD09C
	internal void UpdateShowingSecretInformation()
	{
		bool flag = !this._isShowingSecret;
		if (flag)
		{
			List<SecretInformationDisplayData> showingSecretList = this._showingSecretList;
			if (showingSecretList != null)
			{
				showingSecretList.Clear();
			}
		}
		else
		{
			bool flag2 = null != this.InformationFilter;
			if (flag2)
			{
				this._showingSecretList = this.InformationFilter.FilterSecretInformation(this._srcSecretInformationPackage.SecretInformationDisplayDataList);
			}
			else
			{
				bool flag3 = this._showingSecretList == null;
				if (flag3)
				{
					this._showingSecretList = new List<SecretInformationDisplayData>();
				}
				else
				{
					this._showingSecretList.Clear();
				}
				this._showingSecretList.AddRange(this._srcSecretInformationPackage.SecretInformationDisplayDataList);
			}
			bool flag4 = null != this.SecretInformationFilter;
			if (flag4)
			{
				this.SecretInformationFilter.FilterSecretInformation(this._showingSecretList);
			}
			bool flag5 = null != this.SecretInformationTimeFilter;
			if (flag5)
			{
				this.SecretInformationTimeFilter.FilterSecretInformation(this._showingSecretList);
			}
			bool flag6 = null != this.SecretInformationSearch;
			if (flag6)
			{
				this.SecretInformationSearch.FilterSecretInformationBySearchSettings(this._showingSecretList);
			}
			bool flag7 = null != this.InformationSort;
			if (flag7)
			{
				this.InformationSort.SortSecretInformation(this._showingSecretList);
			}
			Action onFilterSecretInformationDataChanged = this.OnFilterSecretInformationDataChanged;
			if (onFilterSecretInformationDataChanged != null)
			{
				onFilterSecretInformationDataChanged();
			}
		}
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000FEFDC File Offset: 0x000FD1DC
	private void Awake()
	{
		bool awakeFlag = this._awakeFlag;
		if (!awakeFlag)
		{
			bool flag = null != this.InformationFilter;
			if (flag)
			{
				this.InformationFilter.RegisterOnFilterSettingsChangeHandler(new Action(this.RefreshSecretInformationByDate));
				this.InformationFilter.RegisterOnFilterSettingsChangeHandler(new Action(this.SetDirty));
			}
			bool flag2 = null != this.InformationSort;
			if (flag2)
			{
				this.InformationSort.RegisterOnSortSettingsUpdateHandler(new Action(this.SetDirty));
			}
			bool flag3 = null != this.SecretInformationFilter;
			if (flag3)
			{
				this.SecretInformationFilter.RegisterOnSettingsChangeHandler(new Action(this.SetDirty));
			}
			bool flag4 = null != this.NormalInformationFilter;
			if (flag4)
			{
				this.NormalInformationFilter.RegisterOnSettingsChangeHandler(new Action(this.SetDirty));
			}
			bool flag5 = null != this.SecretInformationSearch;
			if (flag5)
			{
				this.SecretInformationSearch.RegisterOnSearchSettingsChangeHandler(new Action(this.SetDirty));
			}
			bool flag6 = null != this.SecretInformationTimeFilter;
			if (flag6)
			{
				SecretInformationTimeFilter secretInformationTimeFilter = this.SecretInformationTimeFilter;
				if (secretInformationTimeFilter.InformationFilter == null)
				{
					secretInformationTimeFilter.InformationFilter = this.InformationFilter;
				}
				this.SecretInformationTimeFilter.RegisterOnSettingsChangeHandler(new Action(this.SetDirty));
			}
			this._awakeFlag = true;
		}
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000FF130 File Offset: 0x000FD330
	private void LateUpdate()
	{
		bool dataDirtyFlag = this._dataDirtyFlag;
		if (dataDirtyFlag)
		{
			this.UpdateShowingNormalInformation();
			this.UpdateShowingSecretInformation();
			this._dataDirtyFlag = false;
		}
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x000FF15F File Offset: 0x000FD35F
	private void OnDestroy()
	{
		this._srcNormalInformationList = null;
		this._srcSecretInformationPackage = null;
		this._showingNormalList = null;
		this._showingSecretList = null;
		this.OnFilterNormalInformationDataChanged = null;
		this.OnFilterSecretInformationDataChanged = null;
	}

	// Token: 0x04001A77 RID: 6775
	public InformationFilter InformationFilter;

	// Token: 0x04001A78 RID: 6776
	public InformationSort InformationSort;

	// Token: 0x04001A79 RID: 6777
	public NormalInformationFilter NormalInformationFilter;

	// Token: 0x04001A7A RID: 6778
	public SecretInformationFilter SecretInformationFilter;

	// Token: 0x04001A7B RID: 6779
	public SecretInformationTimeFilter SecretInformationTimeFilter;

	// Token: 0x04001A7C RID: 6780
	public SecretInformationSearch SecretInformationSearch;

	// Token: 0x04001A7D RID: 6781
	private List<NormalInformation> _srcNormalInformationList;

	// Token: 0x04001A7E RID: 6782
	private List<NormalInformation> _showingNormalList;

	// Token: 0x04001A7F RID: 6783
	private SecretInformationDisplayPackage _srcSecretInformationPackage;

	// Token: 0x04001A80 RID: 6784
	private List<SecretInformationDisplayData> _showingSecretList;

	// Token: 0x04001A81 RID: 6785
	private bool _isShowingSecret;

	// Token: 0x04001A82 RID: 6786
	private bool _dataDirtyFlag;

	// Token: 0x04001A83 RID: 6787
	private bool _awakeFlag;
}
