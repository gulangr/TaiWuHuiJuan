using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class AdventureMajorEventPointEditor : MonoBehaviour
{
	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06001638 RID: 5688 RVA: 0x000896DC File Offset: 0x000878DC
	public AdventureMajorEventNodeSnapshot Data
	{
		get
		{
			return this.parent.Snapshot.Nodes.CheckIndex(this._editingNodeIndex) ? this.parent.Snapshot.Nodes[this._editingNodeIndex] : null;
		}
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00089719 File Offset: 0x00087919
	public void SwitchEditingPoint(AdventureMajorEventEditorPoint point)
	{
		this._point = point;
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00089724 File Offset: 0x00087924
	public void SwitchOpen(bool open)
	{
		bool flag = open && this._point;
		if (flag)
		{
			AdventureMajorEventEditorPoint oldPoint = this.parent.PointDict.GetValueOrDefault(this._editingNodeIndex);
			bool flag2 = oldPoint != null && oldPoint.Editing;
			if (flag2)
			{
				oldPoint.Editing = false;
			}
			bool flag3 = !this._point.Editing;
			if (!flag3)
			{
				this._editingNodeIndex = this._point.Index;
				base.gameObject.SetActive(true);
				this.Refresh();
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x000897CB File Offset: 0x000879CB
	private void Awake()
	{
		this.addRewardGroup.onClick.ResetListener(new Action(this.AddRewardGroup));
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x000897EC File Offset: 0x000879EC
	private void LateUpdate()
	{
		bool flag = !this._point || !this._point.isActiveAndEnabled;
		if (flag)
		{
			this.SwitchEditingPoint(null);
			this.SwitchOpen(false);
		}
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x00089830 File Offset: 0x00087A30
	public void OnDisable()
	{
		AdventureMajorEventEditorPoint oldPoint = this.parent.PointDict.GetValueOrDefault(this._editingNodeIndex);
		bool flag = oldPoint != null && oldPoint.Editing;
		if (flag)
		{
			oldPoint.Editing = false;
		}
		this._editingNodeIndex = -1;
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x00089874 File Offset: 0x00087A74
	public void Refresh()
	{
		AdventureMajorEventEditorPoint prefab;
		bool flag = !this.parent.PointDict.TryGetValue(this._editingNodeIndex, out prefab);
		if (!flag)
		{
			AdventureMajorEventNodeSnapshot node = this.parent.Snapshot.Nodes[prefab.Index];
			this.key.SetTextWithoutNotify(node.Key);
			this.name.SetTextWithoutNotify(node.Name);
			this.desc.SetTextWithoutNotify(node.Desc);
			this.guid.SetTextWithoutNotify(node.EventGuid);
			this.texture.SetTextWithoutNotify(node.EventTexture);
			prefab.SetImg(node);
			this.nextnodes.SetTextWithoutNotify(string.Join(", ", from x in node.NextNodes
			select this.parent.Snapshot.Nodes[x].Key));
			this.SetPointStyleDropdown(node);
			this.SetAtmosphereTypeDropdown(node);
			this.requirementEditor.Refresh();
			this.RefreshRewardGroup();
		}
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x00089980 File Offset: 0x00087B80
	public void EditRequiredItem()
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
			if (flag2)
			{
				Debug.LogError("Internal state error: old index [" + this.Data.Key + "] does not exist, skip modifying Name");
			}
			else
			{
				this.Refresh();
			}
		}
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x00089A00 File Offset: 0x00087C00
	public void EditKey(string newKey)
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = this.Data.Key == newKey;
			if (!flag2)
			{
				this.Data.Key = newKey;
				this.parent.RefreshShow();
				this.Refresh();
			}
		}
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x00089A70 File Offset: 0x00087C70
	public void EditName(string str)
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
			if (flag2)
			{
				Debug.LogError("Internal state error: old index [" + this.Data.Key + "] does not exist, skip modifying Name");
			}
			else
			{
				this.Data.Name = str;
				this.parent.RefreshShow();
				this.Refresh();
			}
		}
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x00089B10 File Offset: 0x00087D10
	public void EditDesc(string str)
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
			if (flag2)
			{
				Debug.LogError("Internal state error: old key [" + this.Data.Key + "] does not exist, skip modifying Desc");
			}
			else
			{
				this.Data.Desc = str;
				this.parent.RefreshShow();
				this.Refresh();
			}
		}
	}

	// Token: 0x06001643 RID: 5699 RVA: 0x00089BB0 File Offset: 0x00087DB0
	public void EditEventGuid(string str)
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
			if (flag2)
			{
				Debug.LogError("Internal state error: old index [" + this.Data.Key + "] does not exist, skip modifying EventGuid");
			}
			else
			{
				this.Data.EventGuid = str;
				this.parent.RefreshShow();
				this.Refresh();
			}
		}
	}

	// Token: 0x06001644 RID: 5700 RVA: 0x00089C48 File Offset: 0x00087E48
	public void EditEventTexture(string str)
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
			if (flag2)
			{
				Debug.LogError("Internal state error: old index [" + this.Data.Key + "] does not exist, skip modifying EventTexture");
			}
			else
			{
				this.Data.EventTexture = str;
				this.parent.RefreshShow();
				this.Refresh();
			}
		}
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x00089CE0 File Offset: 0x00087EE0
	public void AddNextNodes(string str)
	{
		bool flag = this.Data == null;
		if (flag)
		{
			Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
		}
		else
		{
			bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
			if (flag2)
			{
				Debug.LogError(string.Format("Internal state error: Index [{0}] does not exist, skip AddNextNodes", this._editingNodeIndex));
			}
			else
			{
				this.Data.NextNodes.Clear();
				foreach (string node in str.Split(new char[]
				{
					',',
					'，'
				}))
				{
					this.parent.PointDict[this._editingNodeIndex].AddNextNode(node.Trim());
				}
				this.nextnodes.SetTextWithoutNotify(string.Join(", ", from x in this.Data.NextNodes
				select this.parent.Snapshot.Nodes[x].Key));
				this.Data.NextNodes.Sort();
				this.parent.RefreshShow();
				this.Refresh();
			}
		}
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x00089E12 File Offset: 0x00088012
	public void BeginAddingNodes()
	{
		this.parent.PointDict[this._editingNodeIndex].BeginAddSubNodes();
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x00089E30 File Offset: 0x00088030
	public void AddRewardGroup()
	{
		this.Data.Rewards.Add(new AdventureMajorEventRewardData
		{
			Item = new AdventureCostItem(),
			Resource = new AdventureResourceGroup()
		});
		this.RefreshRewardGroup();
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x00089E68 File Offset: 0x00088068
	private void SetPointStyleDropdown(AdventureMajorEventNodeSnapshot node)
	{
		this.pointStyleDropdown.gameObject.SetActive(node.Type == EAdventureMajorEventNodeType.Check);
		bool flag = !this.pointStyleDropdown.gameObject.activeSelf;
		if (!flag)
		{
			this.pointStyleDropdown.onValueChanged.RemoveAllListeners();
			this.pointStyleDropdown.AddOptions(AdventureMajorEventTool.CheckNodeStyleDict.Values.ToList<string>());
			this.pointStyleDropdown.value = node.Style;
			this.pointStyleDropdown.onValueChanged.AddListener(delegate(int value)
			{
				bool flag2 = this.Data == null;
				if (flag2)
				{
					Debug.LogError(string.Format("Data is null, editing key [{0}] does not exists", this._editingNodeIndex));
				}
				else
				{
					bool flag3 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
					if (flag3)
					{
						Debug.LogError("Internal state error: old key [" + this.Data.Key + "] does not exist, skip modifying EventTexture");
					}
					else
					{
						this.Data.Style = value;
						this.parent.RefreshShow();
					}
				}
			});
		}
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x00089F04 File Offset: 0x00088104
	private string GetAtmosphereTypeName(int type)
	{
		return LocalStringManager.Get(string.Format("LK_MajorEventEditor_Atmosphere_Type_{0}", type));
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x00089F2C File Offset: 0x0008812C
	private void SetAtmosphereTypeDropdown(AdventureMajorEventNodeSnapshot node)
	{
		this.atmosphereTypeDropdown.onValueChanged.RemoveAllListeners();
		this.atmosphereTypeDropdown.ClearOptions();
		List<string> options = EasyPool.Get<List<string>>();
		for (int i = 0; i < 3; i++)
		{
			options.Add(this.GetAtmosphereTypeName(i));
		}
		this.atmosphereTypeDropdown.AddOptions(options);
		EasyPool.Free<List<string>>(options);
		this.atmosphereTypeDropdown.value = node.AtmosphereType;
		this.atmosphereTypeDropdown.onValueChanged.AddListener(delegate(int value)
		{
			bool flag = this.Data == null;
			if (flag)
			{
				Debug.LogError(string.Format("Data is null, editing index [{0}] does not exists", this._editingNodeIndex));
			}
			else
			{
				bool flag2 = !this.parent.PointDict.ContainsKey(this._editingNodeIndex);
				if (flag2)
				{
					Debug.LogError("Internal state error: old index [" + this.Data.Key + "] does not exist, skip modifying AtmosphereType");
				}
				else
				{
					this.Data.AtmosphereType = value;
					this.parent.RefreshShow();
				}
			}
		});
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00089FC0 File Offset: 0x000881C0
	public void RefreshRewardGroup()
	{
		this.rewardContainer.Rebuild<RewardGroupEditor>(this.Data.Rewards.Count, delegate(RewardGroupEditor refer, int index)
		{
			refer.SetParent(this, index);
		});
	}

	// Token: 0x04001229 RID: 4649
	[SerializeField]
	private UI_AdventureMajorEventEditor parent;

	// Token: 0x0400122A RID: 4650
	[SerializeField]
	private TMP_InputField key;

	// Token: 0x0400122B RID: 4651
	[SerializeField]
	private new TMP_InputField name;

	// Token: 0x0400122C RID: 4652
	[SerializeField]
	private TMP_InputField desc;

	// Token: 0x0400122D RID: 4653
	[SerializeField]
	private TMP_InputField guid;

	// Token: 0x0400122E RID: 4654
	[SerializeField]
	private TMP_InputField texture;

	// Token: 0x0400122F RID: 4655
	[SerializeField]
	private TMP_InputField nextnodes;

	// Token: 0x04001230 RID: 4656
	[SerializeField]
	private CButton addingNextNodes;

	// Token: 0x04001231 RID: 4657
	[SerializeField]
	private CButton eventEditorBtn;

	// Token: 0x04001232 RID: 4658
	[SerializeField]
	private CDropdown pointStyleDropdown;

	// Token: 0x04001233 RID: 4659
	[SerializeField]
	private CDropdown atmosphereTypeDropdown;

	// Token: 0x04001234 RID: 4660
	[SerializeField]
	internal RectTransform rewardGroupContainer;

	// Token: 0x04001235 RID: 4661
	[SerializeField]
	internal AdventureMajorEventPointEnterRequirementEditor requirementEditor;

	// Token: 0x04001236 RID: 4662
	[SerializeField]
	private CButton addRewardGroup;

	// Token: 0x04001237 RID: 4663
	[SerializeField]
	private TemplatedContainerAssemblyNew rewardContainer;

	// Token: 0x04001238 RID: 4664
	private const int AtmosphereTypeCount = 3;

	// Token: 0x04001239 RID: 4665
	private AdventureMajorEventEditorPoint _point;

	// Token: 0x0400123A RID: 4666
	private int _editingNodeIndex;
}
