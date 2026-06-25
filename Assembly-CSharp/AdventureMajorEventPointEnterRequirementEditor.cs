using System;
using System.Collections.Generic;
using AdventureEditor.Beta;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor;
using GameData.Adventure;
using Google.Protobuf.Collections;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class AdventureMajorEventPointEnterRequirementEditor : MonoBehaviour
{
	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06001652 RID: 5714 RVA: 0x0008A161 File Offset: 0x00088361
	public AdventureMajorEventRequireData Data
	{
		get
		{
			return this.parent.Data.Requirements;
		}
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x0008A174 File Offset: 0x00088374
	private void Awake()
	{
		this.addLifeRequire.onClick.ResetListener(delegate()
		{
			this.AddResource(0);
		});
		this.addCombatRequire.onClick.ResetListener(delegate()
		{
			this.AddResource(1);
		});
		this.addPersonalityRequire.onClick.ResetListener(delegate()
		{
			this.AddResource(2);
		});
		this.addItem.onClick.ResetListener(new Action(this.AddItem));
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x0008A1F8 File Offset: 0x000883F8
	public void AddResource(int type)
	{
		switch (type)
		{
		case 0:
			this.Data.LifeSkillAttainments.Add(new AdventureCostResource
			{
				Type = 0,
				Value = 0
			});
			break;
		case 1:
			this.Data.CombatSkillAttainments.Add(new AdventureCostResource
			{
				Type = 0,
				Value = 0
			});
			break;
		case 2:
			this.Data.Personalities.Add(new AdventureCostResource
			{
				Type = 0,
				Value = 0
			});
			break;
		}
		this.Refresh();
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x0008A29D File Offset: 0x0008849D
	public void AddItem()
	{
		this.Data.Items.Add(new AdventureCostItem());
		this.Refresh();
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x0008A2BD File Offset: 0x000884BD
	public void Refresh()
	{
		this.RefreshLifeSkill();
		this.RefreshCombatSkill();
		this.RefreshPersonality();
		this.RefreshItem();
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x0008A2DC File Offset: 0x000884DC
	public void RefreshItem()
	{
		this.RefreshWithPoolKey<AdventureCostItem, ItemEditor>("UI_AdventureMajorEventEditorItemEditorPrefabKeyPrefab", this.requireItemGroupContainer, this.Data.Items, new Action<ItemEditor, AdventureCostItem, Transform, int>(this.RenderItem));
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x0008A307 File Offset: 0x00088507
	public void RefreshLifeSkill()
	{
		this.RefreshWithPoolKey<AdventureCostResource, ResourceEditor>("UI_AdventureMajorEventEditorLifeSkillEditorPrefabKeyPrefab", this.lifeContainer, this.Data.LifeSkillAttainments, new Action<ResourceEditor, AdventureCostResource, Transform, int>(this.RenderLifeSkill));
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x0008A332 File Offset: 0x00088532
	public void RefreshCombatSkill()
	{
		this.RefreshWithPoolKey<AdventureCostResource, ResourceEditor>("UI_AdventureMajorEventEditorCombatSkillEditorPrefabKeyPrefab", this.combatContainer, this.Data.CombatSkillAttainments, new Action<ResourceEditor, AdventureCostResource, Transform, int>(this.RenderCombatSkill));
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x0008A35D File Offset: 0x0008855D
	public void RefreshPersonality()
	{
		this.RefreshWithPoolKey<AdventureCostResource, ResourceEditor>("UI_AdventureMajorEventEditorPersonalityEditorPrefabKeyPrefab", this.personalityContainer, this.Data.Personalities, new Action<ResourceEditor, AdventureCostResource, Transform, int>(this.RenderPersonality));
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x0008A388 File Offset: 0x00088588
	private void RenderItem(ItemEditor itemEditor, AdventureCostItem data, Transform parentRect, int index)
	{
		itemEditor.transform.SetParent(parentRect, false);
		itemEditor.Set(index, delegate(int idx, List<EditingAdventureData.ItemCostItem> received)
		{
			this.Data.Items[idx].AvailableItems.Clear();
			foreach (EditingAdventureData.ItemCostItem item in received)
			{
				this.Data.Items[idx].AvailableItems.Add(new AdventureItemReference
				{
					Type = (int)item.Item1,
					TemplateId = (int)item.Item2
				});
			}
		}, (int idx) => this.Data.Items[idx], delegate(int idx)
		{
			this.Data.Items.RemoveAt(idx);
		}, new Action(this.Refresh));
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x0008A3E0 File Offset: 0x000885E0
	private void RenderPersonality(ResourceEditor item, AdventureCostResource data, Transform parentRect, int index)
	{
		item.transform.SetParent(parentRect, false);
		RepeatedField<AdventureCostResource> dataArray = this.Data.Personalities;
		item.Set(index, (int idx) => new ValueTuple<int, int>(dataArray[idx].Type, dataArray[idx].Value), delegate(int idx, ValueTuple<int, int> received)
		{
			AdventureCostResource adventureCostResource = dataArray[idx];
			AdventureCostResource adventureCostResource2 = dataArray[idx];
			adventureCostResource.Type = received.Item1;
			adventureCostResource2.Value = received.Item2;
		}, delegate(int idx)
		{
			dataArray.RemoveAt(idx);
		}, new Action(this.Refresh));
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x0008A44C File Offset: 0x0008864C
	private void RenderLifeSkill(ResourceEditor item, AdventureCostResource data, Transform parentRect, int index)
	{
		item.transform.SetParent(parentRect, false);
		RepeatedField<AdventureCostResource> dataArray = this.Data.LifeSkillAttainments;
		item.Set(index, (int idx) => new ValueTuple<int, int>(dataArray[idx].Type, dataArray[idx].Value), delegate(int idx, ValueTuple<int, int> received)
		{
			AdventureCostResource adventureCostResource = dataArray[idx];
			AdventureCostResource adventureCostResource2 = dataArray[idx];
			adventureCostResource.Type = received.Item1;
			adventureCostResource2.Value = received.Item2;
		}, delegate(int idx)
		{
			dataArray.RemoveAt(idx);
		}, new Action(this.Refresh));
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x0008A4B8 File Offset: 0x000886B8
	private void RenderCombatSkill(ResourceEditor item, AdventureCostResource data, Transform parentRect, int index)
	{
		item.transform.SetParent(parentRect, false);
		RepeatedField<AdventureCostResource> dataArray = this.Data.CombatSkillAttainments;
		item.Set(index, (int idx) => new ValueTuple<int, int>(dataArray[idx].Type, dataArray[idx].Value), delegate(int idx, ValueTuple<int, int> received)
		{
			AdventureCostResource adventureCostResource = dataArray[idx];
			AdventureCostResource adventureCostResource2 = dataArray[idx];
			adventureCostResource.Type = received.Item1;
			adventureCostResource2.Value = received.Item2;
		}, delegate(int idx)
		{
			dataArray.RemoveAt(idx);
		}, new Action(this.Refresh));
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x0008A524 File Offset: 0x00088724
	public void RefreshWithPoolKey<T, TComponent>(string key, Transform container, RepeatedField<T> data, Action<TComponent, T, Transform, int> render) where TComponent : Component
	{
		int i;
		for (i = 0; i < container.childCount; i++)
		{
			bool flag = data.CheckIndex(i);
			if (flag)
			{
				render(container.GetChild(i).GetComponent<TComponent>(), data[i], container, i);
			}
			else
			{
				while (i < container.childCount)
				{
					GameObject go = container.GetChild(i).gameObject;
					PoolManager.Destroy(key, go);
					go.transform.SetParent(null, false);
					i++;
				}
			}
		}
		while (i < data.Count)
		{
			render(PoolManager.GetObject<TComponent>(key), data[i], container, i);
			i++;
		}
	}

	// Token: 0x0400123B RID: 4667
	[SerializeField]
	private AdventureMajorEventPointEditor parent;

	// Token: 0x0400123C RID: 4668
	[SerializeField]
	private CButton addLifeRequire;

	// Token: 0x0400123D RID: 4669
	[SerializeField]
	private CButton addCombatRequire;

	// Token: 0x0400123E RID: 4670
	[SerializeField]
	private CButton addPersonalityRequire;

	// Token: 0x0400123F RID: 4671
	[SerializeField]
	private CButton addItem;

	// Token: 0x04001240 RID: 4672
	[SerializeField]
	internal RectTransform lifeContainer;

	// Token: 0x04001241 RID: 4673
	[SerializeField]
	internal RectTransform combatContainer;

	// Token: 0x04001242 RID: 4674
	[SerializeField]
	internal RectTransform personalityContainer;

	// Token: 0x04001243 RID: 4675
	[SerializeField]
	internal RectTransform requireItemGroupContainer;

	// Token: 0x04001244 RID: 4676
	public const int LifeSkill = 0;

	// Token: 0x04001245 RID: 4677
	public const int CombatSkill = 1;

	// Token: 0x04001246 RID: 4678
	public const int Personality = 2;
}
