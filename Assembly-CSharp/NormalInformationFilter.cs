using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200021B RID: 539
public class NormalInformationFilter : MonoBehaviour, ISetInformationSortOrFilterStyle
{
	// Token: 0x1400001A RID: 26
	// (add) Token: 0x0600227E RID: 8830 RVA: 0x000FFA34 File Offset: 0x000FDC34
	// (remove) Token: 0x0600227F RID: 8831 RVA: 0x000FFA6C File Offset: 0x000FDC6C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnNormalInformationFilterSettingsChangeEvent;

	// Token: 0x06002280 RID: 8832 RVA: 0x000FFAA1 File Offset: 0x000FDCA1
	public void SetStyle(bool isSecret)
	{
		base.gameObject.SetActive(!isSecret);
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000FFAB4 File Offset: 0x000FDCB4
	public void RegisterOnSettingsChangeHandler(Action handler)
	{
		this.OnNormalInformationFilterSettingsChangeEvent -= handler;
		this.OnNormalInformationFilterSettingsChangeEvent += handler;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000FFAC8 File Offset: 0x000FDCC8
	public void FilterNormalInformation(List<NormalInformation> normalInformationList, sbyte informationType)
	{
		bool flag = this._lastInformationType != informationType;
		if (flag)
		{
			this.ToggleGroup.Clear();
			Transform container = this.LabelTemplate.transform.parent;
			this.LabelTemplate.SetActive(false);
			for (int i = container.childCount - 1; i >= 0; i--)
			{
				GameObject child = container.GetChild(i).gameObject;
				bool flag2 = child == this.LabelTemplate;
				if (!flag2)
				{
					Object.Destroy(child);
				}
			}
			GameObject all = Object.Instantiate<GameObject>(this.LabelTemplate, container);
			CToggleObsolete allToggle = all.GetComponent<CToggleObsolete>();
			all.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.Get(LanguageKey.LK_SecretInformationFilter_All);
			all.SetActive(true);
			allToggle.Key = -1;
			this.ToggleGroup.Add(allToggle);
			switch (informationType)
			{
			case 0:
			case 1:
				foreach (InformationInfoItem info in ((IEnumerable<InformationInfoItem>)InformationInfo.Instance))
				{
					CToggleObsolete toggle = this.ToggleGroup.Get((int)info.Oraganization);
					OrganizationItem dependency = Organization.Instance.GetItem(info.Oraganization);
					bool flag3 = toggle == null && dependency != null;
					if (flag3)
					{
						bool flag4 = (informationType == 0 && dependency.IsSect) || (informationType == 1 && !dependency.IsSect);
						if (!flag4)
						{
							GameObject toggleObject = Object.Instantiate<GameObject>(this.LabelTemplate, container);
							toggle = toggleObject.GetComponent<CToggleObsolete>();
							toggleObject.GetComponentInChildren<TextMeshProUGUI>().text = dependency.Name;
							toggleObject.SetActive(true);
							toggle.Key = (int)info.Oraganization;
							this.ToggleGroup.Add(toggle);
						}
					}
				}
				break;
			case 2:
				foreach (InformationInfoItem info2 in ((IEnumerable<InformationInfoItem>)InformationInfo.Instance))
				{
					bool flag5 = NormalInformationFilter.PresetFilterMode.LiteratiSkill3 == this.PresetFilter;
					if (flag5)
					{
						sbyte lifeSkillType = info2.LifeSkillType;
						bool flag6 = lifeSkillType == 12 || lifeSkillType == 13;
						if (flag6)
						{
							continue;
						}
					}
					CToggleObsolete toggle2 = this.ToggleGroup.Get((int)info2.LifeSkillType);
					LifeSkillTypeItem dependency2 = LifeSkillType.Instance.GetItem(info2.LifeSkillType);
					bool flag7 = toggle2 == null && dependency2 != null;
					if (flag7)
					{
						GameObject toggleObject2 = Object.Instantiate<GameObject>(this.LabelTemplate, container);
						toggle2 = toggleObject2.GetComponent<CToggleObsolete>();
						toggleObject2.GetComponentInChildren<TextMeshProUGUI>().text = dependency2.Name;
						toggleObject2.SetActive(true);
						toggle2.Key = (int)info2.LifeSkillType;
						this.ToggleGroup.Add(toggle2);
					}
				}
				break;
			case 3:
				foreach (InformationInfoItem info3 in ((IEnumerable<InformationInfoItem>)InformationInfo.Instance))
				{
					CToggleObsolete toggle3 = this.ToggleGroup.Get((int)info3.WesternRegionId);
					WesternRegionItem dependency3 = WesternRegion.Instance.GetItem(info3.WesternRegionId);
					bool flag8 = toggle3 == null && dependency3 != null;
					if (flag8)
					{
						GameObject toggleObject3 = Object.Instantiate<GameObject>(this.LabelTemplate, container);
						toggle3 = toggleObject3.GetComponent<CToggleObsolete>();
						toggleObject3.GetComponentInChildren<TextMeshProUGUI>().text = dependency3.Name;
						toggleObject3.SetActive(true);
						toggle3.Key = (int)info3.WesternRegionId;
						this.ToggleGroup.Add(toggle3);
					}
				}
				break;
			case 6:
				foreach (InformationInfoItem info4 in ((IEnumerable<InformationInfoItem>)InformationInfo.Instance))
				{
					CToggleObsolete toggle4 = this.ToggleGroup.Get((int)info4.Profession);
					ProfessionItem dependency4 = Profession.Instance.GetItem((int)info4.Profession);
					bool flag9 = toggle4 == null && dependency4 != null;
					if (flag9)
					{
						GameObject toggleObject4 = Object.Instantiate<GameObject>(this.LabelTemplate, container);
						toggle4 = toggleObject4.GetComponent<CToggleObsolete>();
						toggleObject4.GetComponentInChildren<TextMeshProUGUI>().text = dependency4.Name;
						toggleObject4.SetActive(true);
						toggle4.Key = (int)info4.Profession;
						this.ToggleGroup.Add(toggle4);
					}
				}
				break;
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				bool flag11 = container != null;
				if (flag11)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
					container.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
				}
			});
			this.ToggleGroup.Set(allToggle, true);
			this._lastInformationType = ((this.ToggleGroup.GetAll().Count > 1) ? informationType : -1);
		}
		CToggleObsolete activeToggle = this.ToggleGroup.GetActive();
		bool flag10 = null == activeToggle || activeToggle.Key < 0;
		if (!flag10)
		{
			switch (informationType)
			{
			case 0:
			case 1:
				normalInformationList.RemoveAll(delegate(NormalInformation item)
				{
					InformationItem config = Information.Instance.GetItem(item.TemplateId);
					InformationInfoItem info5 = InformationInfo.Instance.GetItem(config.InfoIds[(int)item.Level]);
					return (int)info5.Oraganization != activeToggle.Key;
				});
				break;
			case 2:
				normalInformationList.RemoveAll(delegate(NormalInformation item)
				{
					InformationItem config = Information.Instance.GetItem(item.TemplateId);
					InformationInfoItem info5 = InformationInfo.Instance.GetItem(config.InfoIds[(int)item.Level]);
					return (int)info5.LifeSkillType != activeToggle.Key;
				});
				break;
			case 3:
				normalInformationList.RemoveAll(delegate(NormalInformation item)
				{
					InformationItem config = Information.Instance.GetItem(item.TemplateId);
					InformationInfoItem info5 = InformationInfo.Instance.GetItem(config.InfoIds[(int)item.Level]);
					return (int)info5.WesternRegionId != activeToggle.Key;
				});
				break;
			case 5:
				normalInformationList.RemoveAll(delegate(NormalInformation item)
				{
					InformationItem config = Information.Instance.GetItem(item.TemplateId);
					InformationInfoItem info5 = InformationInfo.Instance.GetItem(config.InfoIds[(int)item.Level]);
					return info5.SwordInformationType != (EInformationInfoSwordInformationType)activeToggle.Key;
				});
				break;
			case 6:
				normalInformationList.RemoveAll(delegate(NormalInformation item)
				{
					InformationItem config = Information.Instance.GetItem(item.TemplateId);
					InformationInfoItem info5 = InformationInfo.Instance.GetItem(config.InfoIds[(int)item.Level]);
					return (int)info5.Profession != activeToggle.Key;
				});
				break;
			}
		}
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x001000FC File Offset: 0x000FE2FC
	private void Awake()
	{
		this._lastInformationType = -1;
		this.ToggleGroup.InitPreOnToggle(-1);
		this.ToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete _, CToggleObsolete __)
		{
			Action onNormalInformationFilterSettingsChangeEvent = this.OnNormalInformationFilterSettingsChangeEvent;
			if (onNormalInformationFilterSettingsChangeEvent != null)
			{
				onNormalInformationFilterSettingsChangeEvent();
			}
		};
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x0010012A File Offset: 0x000FE32A
	private void OnDestroy()
	{
		this.OnNormalInformationFilterSettingsChangeEvent = null;
	}

	// Token: 0x04001A88 RID: 6792
	public CToggleGroupObsolete ToggleGroup;

	// Token: 0x04001A89 RID: 6793
	public GameObject LabelTemplate;

	// Token: 0x04001A8A RID: 6794
	public NormalInformationFilter.PresetFilterMode PresetFilter = NormalInformationFilter.PresetFilterMode.None;

	// Token: 0x04001A8C RID: 6796
	private sbyte _lastInformationType = -1;

	// Token: 0x020014F5 RID: 5365
	public enum PresetFilterMode
	{
		// Token: 0x0400A31D RID: 41757
		None,
		// Token: 0x0400A31E RID: 41758
		LiteratiSkill3
	}
}
