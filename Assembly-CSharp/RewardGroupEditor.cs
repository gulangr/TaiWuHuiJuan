using System;
using System.Collections.Generic;
using System.Linq;
using AdventureEditor.Beta;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class RewardGroupEditor : MonoBehaviour
{
	// Token: 0x17000281 RID: 641
	// (get) Token: 0x060016B1 RID: 5809 RVA: 0x0008B18E File Offset: 0x0008938E
	public AdventureMajorEventRewardData Data
	{
		get
		{
			return this.parent.Data.Rewards.CheckIndex(this.index) ? this.parent.Data.Rewards[this.index] : null;
		}
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x0008B1CC File Offset: 0x000893CC
	private void Awake()
	{
		this.RefreshLabels();
		this.exp.characterValidation = TMP_InputField.CharacterValidation.Integer;
		this.exp.onValueChanged.ResetListener(new Action<string>(this.EditExp));
		this.exp.onEndEdit.ResetListener(delegate(string _)
		{
			this.RefreshExpValue();
		});
		for (int i = 0; i < this.resources.Length; i++)
		{
			int resourceTemplateId = i;
			this.resources[i].characterValidation = TMP_InputField.CharacterValidation.Integer;
			this.resources[i].onValueChanged.ResetListener(delegate(string str)
			{
				this.EditResource(str, resourceTemplateId);
			});
			this.resources[i].onEndEdit.ResetListener(delegate(string _)
			{
				this.RefreshResourceValue(resourceTemplateId);
			});
		}
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x0008B2A4 File Offset: 0x000894A4
	public void Delete()
	{
		bool flag = this.parent == null || this.parent.Data == null || !this.parent.Data.Rewards.CheckIndex(this.index);
		if (!flag)
		{
			this.parent.Data.Rewards.RemoveAt(this.index);
			this.parent.RefreshRewardGroup();
		}
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x0008B31C File Offset: 0x0008951C
	public void RefreshLabels()
	{
		this.expLabel.text = LanguageKey.LK_MajorEventEditor_EditPoint_Reward_Label.TrFormat(LanguageKey.LK_Exp.Tr());
		for (int i = 0; i < this.resourceLabels.Length; i++)
		{
			this.resourceLabels[i].text = LanguageKey.LK_MajorEventEditor_EditPoint_Reward_Label.TrFormat(ResourceType.Instance[i].Name);
		}
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x0008B389 File Offset: 0x00089589
	public void SetParent(AdventureMajorEventPointEditor newParent, int currIndex)
	{
		this.parent = newParent;
		this.Refresh(currIndex);
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x0008B39B File Offset: 0x0008959B
	public void Refresh()
	{
		this.Refresh(this.index);
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x0008B3AC File Offset: 0x000895AC
	public void Refresh(int currIndex)
	{
		TMP_Text tmp_Text = this.deleteBtnText;
		LanguageKey languageKey = LanguageKey.LK_MajorEventEditor_EditPoint_Reward_Delete;
		this.index = currIndex;
		tmp_Text.text = languageKey.TrFormat(currIndex);
		this.RefreshTemplateIds();
		this.RefreshValues();
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x0008B3F0 File Offset: 0x000895F0
	public void EditItem()
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("MultipleChoice", true).Set("SelectType", UI_ItemTemplateSelector.ESelectType.ItemTemplate);
		string key = "InitialSelection";
		RepeatedField<AdventureItemReference> availableItems = this.Data.Item.AvailableItems;
		object obj;
		if (availableItems == null)
		{
			obj = null;
		}
		else
		{
			obj = (from data in availableItems
			select new EditingAdventureData.ItemCostItem
			{
				Item1 = (sbyte)data.Type,
				Item2 = (short)data.TemplateId
			}).ToArray<EditingAdventureData.ItemCostItem>();
		}
		ArgumentBox argBox = argumentBox.SetObject(key, obj ?? Array.Empty<EditingAdventureData.ItemCostItem>()).SetObject("OnConfirm", new UI_ItemTemplateSelector.OnConfirmHandler(delegate(List<EditingAdventureData.ItemCostItem> list)
		{
			this.Data.Item.AvailableItems.Clear();
			foreach (EditingAdventureData.ItemCostItem item in list)
			{
				this.Data.Item.AvailableItems.Add(new AdventureItemReference
				{
					Type = (int)item.Item1,
					TemplateId = (int)item.Item2
				});
			}
			this.RefreshTemplateIds();
		}));
		UIElement.ItemTemplateSelector.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.ItemTemplateSelector, true);
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x0008B4A8 File Offset: 0x000896A8
	public void RefreshTemplateIds()
	{
		this.items.text = string.Join("|", this.Data.Item.AvailableItems.Select(delegate(AdventureItemReference x)
		{
			IItemConfig item = ItemConfigHelper.GetConfig((sbyte)x.Type, (short)x.TemplateId);
			return string.Format("<color=#GradeColor_{0}>{1}</color>", item.Grade, item.Name);
		})).ColorReplace();
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0008B504 File Offset: 0x00089704
	public void RefreshValues()
	{
		this.RefreshExpValue();
		for (int i = 0; i < this.resources.Length; i++)
		{
			this.RefreshResourceValue(i);
		}
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x0008B538 File Offset: 0x00089738
	public void EditExp(string str)
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			int val;
			bool flag2 = int.TryParse(str, out val) && val > 0;
			if (flag2)
			{
				this.Data.Exp = val;
			}
			else
			{
				this.Data.Exp = 0;
			}
		}
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0008B588 File Offset: 0x00089788
	public void EditResource(string str, int resourceTemplateId)
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			int val;
			bool flag2 = int.TryParse(str, out val) && val > 0;
			if (flag2)
			{
				int i = this.Data.Resource.Resources.Count;
				while (i-- > 0)
				{
					bool flag3 = this.Data.Resource.Resources[i].Type == resourceTemplateId;
					if (flag3)
					{
						this.Data.Resource.Resources[i].Value = val;
						return;
					}
				}
				this.Data.Resource.Resources.Add(new AdventureCostResource
				{
					Type = resourceTemplateId,
					Value = val
				});
			}
			else
			{
				int j = this.Data.Resource.Resources.Count;
				while (j-- > 0)
				{
					bool flag4 = this.Data.Resource.Resources[j].Type == resourceTemplateId;
					if (flag4)
					{
						this.Data.Resource.Resources.RemoveAt(j);
						break;
					}
				}
			}
		}
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x0008B6C8 File Offset: 0x000898C8
	private void RefreshExpValue()
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			this.exp.SetTextWithoutNotify(this.Data.Exp.ToString());
		}
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0008B704 File Offset: 0x00089904
	private void RefreshResourceValue(int resourceTemplateId)
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			this.resources[resourceTemplateId].SetTextWithoutNotify(this.GetResourceValue(resourceTemplateId).ToString());
		}
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x0008B740 File Offset: 0x00089940
	private int GetResourceValue(int resourceTemplateId)
	{
		bool flag = this.Data == null;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			for (int i = 0; i < this.Data.Resource.Resources.Count; i++)
			{
				AdventureCostResource resource = this.Data.Resource.Resources[i];
				bool flag2 = resource.Type == resourceTemplateId;
				if (flag2)
				{
					return resource.Value;
				}
			}
			result = 0;
		}
		return result;
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x0008B7BB File Offset: 0x000899BB
	public void EditResource0(string str)
	{
		this.EditResource(str, 0);
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0008B7C6 File Offset: 0x000899C6
	public void EditResource1(string str)
	{
		this.EditResource(str, 1);
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x0008B7D1 File Offset: 0x000899D1
	public void EditResource2(string str)
	{
		this.EditResource(str, 2);
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x0008B7DC File Offset: 0x000899DC
	public void EditResource3(string str)
	{
		this.EditResource(str, 3);
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x0008B7E7 File Offset: 0x000899E7
	public void EditResource4(string str)
	{
		this.EditResource(str, 4);
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0008B7F2 File Offset: 0x000899F2
	public void EditResource5(string str)
	{
		this.EditResource(str, 5);
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x0008B7FD File Offset: 0x000899FD
	public void EditResource6(string str)
	{
		this.EditResource(str, 6);
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x0008B808 File Offset: 0x00089A08
	public void EditResource7(string str)
	{
		this.EditResource(str, 7);
	}

	// Token: 0x04001265 RID: 4709
	[SerializeField]
	private int index;

	// Token: 0x04001266 RID: 4710
	[SerializeField]
	private AdventureMajorEventPointEditor parent;

	// Token: 0x04001267 RID: 4711
	[SerializeField]
	private TMP_InputField exp;

	// Token: 0x04001268 RID: 4712
	[SerializeField]
	private TMP_InputField[] resources;

	// Token: 0x04001269 RID: 4713
	[SerializeField]
	private TMP_Text items;

	// Token: 0x0400126A RID: 4714
	[SerializeField]
	private TMP_Text expLabel;

	// Token: 0x0400126B RID: 4715
	[SerializeField]
	private TMP_Text[] resourceLabels;

	// Token: 0x0400126C RID: 4716
	[SerializeField]
	private CButton deleteBtn;

	// Token: 0x0400126D RID: 4717
	[SerializeField]
	private TMP_Text deleteBtnText;
}
