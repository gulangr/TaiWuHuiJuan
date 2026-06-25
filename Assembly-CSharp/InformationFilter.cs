using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using FrameWork;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class InformationFilter : MonoBehaviour, ISetInformationSortOrFilterStyle
{
	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06002244 RID: 8772 RVA: 0x000FE044 File Offset: 0x000FC244
	// (remove) Token: 0x06002245 RID: 8773 RVA: 0x000FE07C File Offset: 0x000FC27C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action OnFilterSettingsChangeEvent;

	// Token: 0x06002246 RID: 8774 RVA: 0x000FE0B4 File Offset: 0x000FC2B4
	private void Awake()
	{
		bool flag = null != this.ToggleGroup;
		if (flag)
		{
			this.ToggleGroup.InitPreOnToggle(-1);
			this.ToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete _, CToggleObsolete _)
			{
				Action onFilterSettingsChangeEvent = this.OnFilterSettingsChangeEvent;
				if (onFilterSettingsChangeEvent != null)
				{
					onFilterSettingsChangeEvent();
				}
			};
		}
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000FE0F8 File Offset: 0x000FC2F8
	private void OnDestroy()
	{
		this.OnFilterSettingsChangeEvent = null;
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000FE102 File Offset: 0x000FC302
	public void SetStyle(bool isSecret)
	{
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000FE108 File Offset: 0x000FC308
	public void SetTogglesVisibleState(bool[] flags)
	{
		CToggleObsolete[] allToggles = new CToggleObsolete[]
		{
			this.ToggleArea,
			this.ToggleSect,
			this.ToggleLifeSkill,
			this.ToggleWestern,
			this.ToggleSwordTomb,
			this.ToggleProfession,
			this.ToggleSecret,
			this.ToggleBroadcastSecret
		};
		for (int i = 0; i < allToggles.Length; i++)
		{
			CToggleObsolete toggle = allToggles[i];
			toggle.interactable = flags[i];
			toggle.gameObject.SetActive(flags[i]);
			bool flag = !flags[i] && toggle.isOn;
			if (flag)
			{
				this.ToggleGroup.Set(toggle, false);
			}
			sbyte informationType = -1;
			bool flag2 = toggle == this.ToggleArea;
			if (flag2)
			{
				informationType = 0;
			}
			else
			{
				bool flag3 = toggle == this.ToggleSect;
				if (flag3)
				{
					informationType = 1;
				}
				else
				{
					bool flag4 = toggle == this.ToggleLifeSkill;
					if (flag4)
					{
						informationType = 2;
					}
					else
					{
						bool flag5 = toggle == this.ToggleWestern;
						if (flag5)
						{
							informationType = 3;
						}
						else
						{
							bool flag6 = toggle == this.ToggleSwordTomb;
							if (flag6)
							{
								informationType = 5;
							}
							else
							{
								bool flag7 = toggle == this.ToggleProfession;
								if (flag7)
								{
									informationType = 6;
								}
							}
						}
					}
				}
			}
			bool flag8 = informationType >= 0;
			if (flag8)
			{
				TooltipInvoker tips = allToggles[i].gameObject.GetOrAddComponent<TooltipInvoker>();
				tips.Type = TipType.NormalInformationType;
				tips.RuntimeParam = new ArgumentBox().Set("InformationType", informationType);
			}
		}
		bool flag9 = null == this.ToggleGroup.GetActive();
		if (flag9)
		{
			this.ToggleGroup.SetToFirstInteractable(false);
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000FE2AA File Offset: 0x000FC4AA
	public void RegisterOnFilterSettingsChangeHandler(Action handler)
	{
		this.OnFilterSettingsChangeEvent -= handler;
		this.OnFilterSettingsChangeEvent += handler;
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000FE2C0 File Offset: 0x000FC4C0
	public List<NormalInformation> FilterNormalInformation(List<NormalInformation> normalList, out sbyte type)
	{
		type = 0;
		bool isOn = this.ToggleSect.isOn;
		if (isOn)
		{
			type = 1;
		}
		else
		{
			bool isOn2 = this.ToggleLifeSkill.isOn;
			if (isOn2)
			{
				type = 2;
			}
			else
			{
				bool isOn3 = this.ToggleWestern.isOn;
				if (isOn3)
				{
					type = 3;
				}
				else
				{
					bool isOn4 = this.ToggleSwordTomb.isOn;
					if (isOn4)
					{
						type = 5;
					}
					else
					{
						bool isOn5 = this.ToggleProfession.isOn;
						if (isOn5)
						{
							type = 6;
						}
					}
				}
			}
		}
		return new List<NormalInformation>();
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000FE340 File Offset: 0x000FC540
	public List<SecretInformationDisplayData> FilterSecretInformation(List<SecretInformationDisplayData> secretList)
	{
		bool isBroadcast = this.ToggleBroadcastSecret.isOn;
		return this.FilterSecretInformation(secretList, isBroadcast);
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000FE368 File Offset: 0x000FC568
	private List<SecretInformationDisplayData> FilterSecretInformation(List<SecretInformationDisplayData> srcSecretList, bool isBroadCast)
	{
		List<SecretInformationDisplayData> secretList = new List<SecretInformationDisplayData>();
		bool flag = srcSecretList != null;
		if (flag)
		{
			int i = 0;
			int max = srcSecretList.Count;
			while (i < max)
			{
				SecretInformationDisplayData data = srcSecretList[i];
				bool flag2 = !isBroadCast && data.HolderCount > 0;
				if (flag2)
				{
					secretList.Add(data);
				}
				bool flag3 = isBroadCast && data.HolderCount <= 0;
				if (flag3)
				{
					secretList.Add(data);
				}
				i++;
			}
		}
		return secretList;
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000FE3F4 File Offset: 0x000FC5F4
	public void SetNormalCount(List<NormalInformation> normalList)
	{
		int count0 = 0;
		int count = 0;
		int count2 = 0;
		int count3 = 0;
		int count4 = 0;
		int count5 = 0;
		bool flag = normalList != null;
		if (flag)
		{
			int i = 0;
			int max = normalList.Count;
			while (i < max)
			{
				NormalInformation information = normalList[i];
				InformationItem config = Information.Instance.GetItem(information.TemplateId);
				bool flag2 = config.Type == 0;
				if (flag2)
				{
					count0++;
				}
				else
				{
					bool flag3 = config.Type == 1;
					if (flag3)
					{
						count++;
					}
					else
					{
						bool flag4 = config.Type == 2;
						if (flag4)
						{
							count2++;
						}
						else
						{
							bool flag5 = config.Type == 3;
							if (flag5)
							{
								count3++;
							}
							else
							{
								bool flag6 = config.Type == 5;
								if (flag6)
								{
									count4++;
								}
								else
								{
									bool flag7 = config.Type == 6;
									if (flag7)
									{
										count5++;
									}
								}
							}
						}
					}
				}
				i++;
			}
		}
		this.SetCount(this.ToggleArea, count0);
		this.SetCount(this.ToggleSect, count);
		this.SetCount(this.ToggleLifeSkill, count2);
		this.SetCount(this.ToggleWestern, count3);
		this.SetCount(this.ToggleSwordTomb, count4);
		this.SetCount(this.ToggleProfession, count5);
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000FE540 File Offset: 0x000FC740
	public void SetSecretCount(List<SecretInformationDisplayData> secretList)
	{
		int count = 0;
		int count2 = 0;
		bool flag = secretList != null;
		if (flag)
		{
			int i = 0;
			int max = secretList.Count;
			while (i < max)
			{
				SecretInformationDisplayData data = secretList[i];
				bool flag2 = data.HolderCount > 0;
				if (flag2)
				{
					count++;
				}
				bool flag3 = data.HolderCount <= 0;
				if (flag3)
				{
					count2++;
				}
				i++;
			}
			this.SetCount(this.ToggleSecret, count);
			this.SetCount(this.ToggleBroadcastSecret, count2);
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000FE5CC File Offset: 0x000FC7CC
	internal void SetCount(CToggleObsolete tog, int count)
	{
		bool flag = null == tog;
		if (flag)
		{
			Debug.LogError("can not set null toggle's count");
		}
		else
		{
			Transform labelTrans = tog.transform.Find("Amount");
			bool flag2 = null == labelTrans;
			if (flag2)
			{
				Debug.LogWarning(tog.name + "'s find no child named Amount");
			}
			else
			{
				TextMeshProUGUI label = labelTrans.GetComponent<TextMeshProUGUI>();
				bool flag3 = null == label;
				if (flag3)
				{
					Debug.LogWarning(tog.name + "'s child Amount has no component TextMeshProUGUI");
				}
				else
				{
					label.text = string.Format("({0})", count);
				}
			}
		}
	}

	// Token: 0x04001A63 RID: 6755
	public CToggleGroupObsolete ToggleGroup;

	// Token: 0x04001A64 RID: 6756
	public CToggleObsolete ToggleArea;

	// Token: 0x04001A65 RID: 6757
	public CToggleObsolete ToggleSect;

	// Token: 0x04001A66 RID: 6758
	public CToggleObsolete ToggleLifeSkill;

	// Token: 0x04001A67 RID: 6759
	public CToggleObsolete ToggleWestern;

	// Token: 0x04001A68 RID: 6760
	public CToggleObsolete ToggleSwordTomb;

	// Token: 0x04001A69 RID: 6761
	public CToggleObsolete ToggleProfession;

	// Token: 0x04001A6A RID: 6762
	public CToggleObsolete ToggleSecret;

	// Token: 0x04001A6B RID: 6763
	public CToggleObsolete ToggleBroadcastSecret;
}
