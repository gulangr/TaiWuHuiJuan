using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using EasyButtons;
using FrameWork;
using TMPro;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x0200067F RID: 1663
	public class AiNodeTemplate : Refers, ISelectAndDragComponent, IAiNodeRelateHandler, IAiNodeHyperlinkHandler
	{
		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06004E8B RID: 20107 RVA: 0x0024DD30 File Offset: 0x0024BF30
		private TextMeshProUGUI Id
		{
			get
			{
				return base.CGet<TextMeshProUGUI>("Id");
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06004E8C RID: 20108 RVA: 0x0024DD3D File Offset: 0x0024BF3D
		private GameObject Selecting
		{
			get
			{
				return base.CGet<GameObject>("Selecting");
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06004E8D RID: 20109 RVA: 0x0024DD4A File Offset: 0x0024BF4A
		private RectTransform Linear
		{
			get
			{
				return base.CGet<RectTransform>("Linear");
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06004E8E RID: 20110 RVA: 0x0024DD57 File Offset: 0x0024BF57
		private RectTransform Branch
		{
			get
			{
				return base.CGet<RectTransform>("Branch");
			}
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06004E8F RID: 20111 RVA: 0x0024DD64 File Offset: 0x0024BF64
		private RectTransform Action
		{
			get
			{
				return base.CGet<RectTransform>("Action");
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06004E90 RID: 20112 RVA: 0x0024DD71 File Offset: 0x0024BF71
		private RectTransform Relay
		{
			get
			{
				return base.CGet<RectTransform>("Relay");
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06004E91 RID: 20113 RVA: 0x0024DD7E File Offset: 0x0024BF7E
		// (set) Token: 0x06004E92 RID: 20114 RVA: 0x0024DD86 File Offset: 0x0024BF86
		public int NextCount { get; private set; }

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06004E93 RID: 20115 RVA: 0x0024DD8F File Offset: 0x0024BF8F
		// (set) Token: 0x06004E94 RID: 20116 RVA: 0x0024DD97 File Offset: 0x0024BF97
		public int RuntimeId { get; private set; }

		// Token: 0x06004E95 RID: 20117 RVA: 0x0024DDA0 File Offset: 0x0024BFA0
		public void Set(IAiNodeTemplateHandler handler, int runtimeId, int templateId)
		{
			this._handler = handler;
			this._config = AiNode.Instance[templateId];
			this.RuntimeId = runtimeId;
			this.Id.text = runtimeId.ToString();
			this.Unselect();
			base.CGet<TextMeshProUGUI>("Name").text = this._config.Name;
			this.Clear();
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x0024DE0C File Offset: 0x0024C00C
		public void Clear()
		{
			this.NextCount = 0;
			this.Linear.gameObject.SetActive(false);
			this.Branch.gameObject.SetActive(false);
			this.Action.gameObject.SetActive(false);
			this.Relay.gameObject.SetActive(false);
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x0024DE6A File Offset: 0x0024C06A
		public void Select()
		{
			this.Selecting.SetActive(true);
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x0024DE7A File Offset: 0x0024C07A
		public void Unselect()
		{
			this.Selecting.SetActive(false);
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x0024DE8C File Offset: 0x0024C08C
		public AiNodeRelate TryGetSelectingRelate()
		{
			List<AiNodeRelate> relates = EasyPool.Get<List<AiNodeRelate>>();
			base.transform.GetComponentsInChildren<AiNodeRelate>(relates);
			Camera uiCamera = UIManager.Instance.UiCamera;
			Vector3 mousePos = Input.mousePosition;
			AiNodeRelate relate2 = relates.FirstOrDefault((AiNodeRelate relate) => RectTransformUtility.RectangleContainsScreenPoint(relate.RectTransform, mousePos, uiCamera));
			EasyPool.Free<List<AiNodeRelate>>(relates);
			return relate2;
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x0024DEF0 File Offset: 0x0024C0F0
		public AiNodeTemplate AppendLinear()
		{
			bool flag = this._config.Type > EAiNodeType.Linear;
			AiNodeTemplate result;
			if (flag)
			{
				result = this;
			}
			else
			{
				AiNodeRelate relate = this.AppendObject<AiNodeRelate>(this.Linear);
				relate.Bind(this);
				relate.Set(-1);
				this.EnsureActive(this.Linear);
				result = this;
			}
			return result;
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x0024DF44 File Offset: 0x0024C144
		public void AppendBranch(int conditionTemplateId)
		{
			bool flag = this._config.Type != EAiNodeType.Branch;
			if (!flag)
			{
				AiNodeBranch branch = this.AppendObject<AiNodeBranch>(this.Branch);
				branch.Bind(this, this);
				branch.Set(conditionTemplateId);
				this.EnsureActive(this.Branch);
			}
		}

		// Token: 0x06004E9C RID: 20124 RVA: 0x0024DF94 File Offset: 0x0024C194
		public void AppendAction(int actionTemplateId)
		{
			bool flag = this._config.Type != EAiNodeType.Action;
			if (!flag)
			{
				AiNodeHyperlink action = this.AppendObject<AiNodeHyperlink>(this.Action);
				action.Bind(this);
				action.SetAction(actionTemplateId);
				this.EnsureActive(this.Action);
			}
		}

		// Token: 0x06004E9D RID: 20125 RVA: 0x0024DFE4 File Offset: 0x0024C1E4
		public AiNodeTemplate AppendRelay()
		{
			bool flag = this._config.Type != EAiNodeType.Relay;
			AiNodeTemplate result;
			if (flag)
			{
				result = this;
			}
			else
			{
				AiNodeRelay relay = this.AppendObject<AiNodeRelay>(this.Relay);
				relay.Bind(this);
				relay.Set();
				this.EnsureActive(this.Relay);
				result = this;
			}
			return result;
		}

		// Token: 0x06004E9E RID: 20126 RVA: 0x0024E03C File Offset: 0x0024C23C
		private T AppendObject<T>(Transform root) where T : Component
		{
			bool flag = this.NextCount == 0;
			if (flag)
			{
				root.gameObject.SetActive(true);
			}
			this.NextCount++;
			Transform template = root.GetChild(0);
			bool flag2 = this.NextCount > root.childCount;
			T result;
			if (flag2)
			{
				Debug.LogWarning("Cannot append multi object");
				result = default(T);
			}
			else
			{
				bool flag3 = this.NextCount == root.childCount;
				if (flag3)
				{
					Object.Instantiate<Transform>(template, root);
				}
				result = root.GetChild(this.NextCount - 1).GetComponent<T>();
			}
			return result;
		}

		// Token: 0x06004E9F RID: 20127 RVA: 0x0024E0DC File Offset: 0x0024C2DC
		[Button]
		public void RemoveAt(int index)
		{
			bool flag = this.NextCount <= index;
			if (!flag)
			{
				EAiNodeType type = this._config.Type;
				if (!true)
				{
				}
				RectTransform root2;
				switch (type)
				{
				case EAiNodeType.Linear:
					root2 = this.Linear;
					break;
				case EAiNodeType.Branch:
					root2 = this.Branch;
					break;
				case EAiNodeType.Action:
					root2 = this.Action;
					break;
				case EAiNodeType.Relay:
					root2 = this.Relay;
					break;
				default:
					root2 = null;
					break;
				}
				if (!true)
				{
				}
				RectTransform root = root2;
				bool flag2 = root == null;
				if (!flag2)
				{
					this._handler.NodeAction(this.RuntimeId, delegate
					{
						Transform target = root.GetChild(index);
						target.SetAsLastSibling();
						target.gameObject.SetActive(false);
						this.NextCount--;
					});
				}
			}
		}

		// Token: 0x06004EA0 RID: 20128 RVA: 0x0024E1A8 File Offset: 0x0024C3A8
		private void EnsureActive(Transform root)
		{
			for (int i = 0; i < this.NextCount; i++)
			{
				bool flag = !root.GetChild(i).gameObject.activeSelf;
				if (flag)
				{
					root.GetChild(i).gameObject.SetActive(true);
				}
			}
			for (int j = this.NextCount; j < root.childCount; j++)
			{
				bool activeSelf = root.GetChild(j).gameObject.activeSelf;
				if (activeSelf)
				{
					root.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x0024E23C File Offset: 0x0024C43C
		public void InvokeRelate()
		{
			IAiNodeTemplateHandler handler = this._handler;
			if (handler != null)
			{
				handler.NodeRelate(this.RuntimeId);
			}
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x0024E257 File Offset: 0x0024C457
		public void SetParam(Action action)
		{
			IAiNodeTemplateHandler handler = this._handler;
			if (handler != null)
			{
				handler.NodeAction(this.RuntimeId, action);
			}
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x0024E274 File Offset: 0x0024C474
		public void ApplyRelate(int relateId)
		{
			foreach (AiNodeRelate relate in base.GetComponentsInChildren<AiNodeRelate>())
			{
				relate.Relate(relateId);
			}
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x0024E2A4 File Offset: 0x0024C4A4
		public void InterruptRelate()
		{
			foreach (AiNodeRelate relate in base.GetComponentsInChildren<AiNodeRelate>())
			{
				relate.Interrupt();
			}
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x0024E2D4 File Offset: 0x0024C4D4
		public void Sync(Dictionary<int, int> old2NewIds)
		{
			int newRuntimeId;
			bool flag = !old2NewIds.TryGetValue(this.RuntimeId, out newRuntimeId);
			if (!flag)
			{
				this.RuntimeId = newRuntimeId;
				this.Id.text = this.RuntimeId.ToString();
			}
		}

		// Token: 0x06004EA6 RID: 20134 RVA: 0x0024E31C File Offset: 0x0024C51C
		public AiNodeDataSnapshot Save()
		{
			AiNodeDataSnapshot data = new AiNodeDataSnapshot
			{
				Position = ((RectTransform)base.transform).anchoredPosition,
				RuntimeId = this.RuntimeId,
				Type = this._config.TemplateId
			};
			switch (this._config.Type)
			{
			case EAiNodeType.Linear:
				for (int i = 0; i < this.NextCount; i++)
				{
					data.Ids.Add(this.Linear.GetChild(i).GetComponent<AiNodeRelate>().RelateId);
				}
				return data;
			case EAiNodeType.Branch:
				for (int j = 0; j < this.NextCount; j++)
				{
					AiNodeBranch branch = this.Branch.GetChild(j).GetComponent<AiNodeBranch>();
					data.Ids.Add(j);
					data.Ids.Add(branch.True.RelateId);
					data.Ids.Add(branch.False.RelateId);
					data.SubTypes.Add(branch.Condition.TemplateId);
					data.Params.Add(branch.Condition.Save());
				}
				return data;
			case EAiNodeType.Action:
				for (int k = 0; k < this.NextCount; k++)
				{
					AiNodeHyperlink action = this.Action.GetChild(k).GetComponent<AiNodeHyperlink>();
					data.Ids.Add(k);
					data.SubTypes.Add(action.TemplateId);
					data.Params.Add(action.Save());
				}
				return data;
			case EAiNodeType.Relay:
				for (int l = 0; l < this.NextCount; l++)
				{
					AiNodeRelay relay = this.Relay.GetChild(l).GetComponent<AiNodeRelay>();
					data.Ids.Add(relay.Next.RelateId);
					data.Ids.Add(relay.Relay.RelateId);
				}
				return data;
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x0024E550 File Offset: 0x0024C750
		public void Load(IAiNodeTemplateHandler handler, AiNodeDataSnapshot data)
		{
			this.Clear();
			((RectTransform)base.transform).anchoredPosition = data.Position;
			this.Set(handler, data.RuntimeId, data.Type);
			switch (this._config.Type)
			{
			case EAiNodeType.Linear:
				for (int i = 0; i < data.Ids.Count; i++)
				{
					int id = data.Ids[i];
					this.AppendLinear();
					this.Linear.GetChild(i).GetComponent<AiNodeRelate>().Set(id);
				}
				break;
			case EAiNodeType.Branch:
				for (int j = 0; j < data.SubTypes.Count; j++)
				{
					int subType = data.SubTypes[j];
					this.AppendBranch(subType);
					AiNodeBranch branch = this.Branch.GetChild(j).GetComponent<AiNodeBranch>();
					branch.Condition.Load(data.Params[j]);
					branch.True.Set(data.Ids[j * 3 + 1]);
					branch.False.Set(data.Ids[j * 3 + 2]);
				}
				break;
			case EAiNodeType.Action:
				for (int k = 0; k < data.SubTypes.Count; k++)
				{
					int subType2 = data.SubTypes[k];
					this.AppendAction(subType2);
					AiNodeHyperlink action = this.Action.GetChild(k).GetComponent<AiNodeHyperlink>();
					action.Load(data.Params[k]);
				}
				break;
			case EAiNodeType.Relay:
				for (int l = 0; l < data.Ids.Count; l += 2)
				{
					this.AppendRelay();
					AiNodeRelay relay = this.Relay.GetChild(l / 2).GetComponent<AiNodeRelay>();
					relay.Next.Set(data.Ids[l]);
					relay.Relay.Set(data.Ids[l + 1]);
				}
				break;
			case EAiNodeType.Count:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x0024E7A0 File Offset: 0x0024C9A0
		public bool Contains(string text)
		{
			return base.GetComponentsInChildren<TMP_Text>().Any(delegate(TMP_Text x)
			{
				string text2 = x.text;
				return text2 != null && text2.Contains(text);
			});
		}

		// Token: 0x04003644 RID: 13892
		private IAiNodeTemplateHandler _handler;

		// Token: 0x04003645 RID: 13893
		private AiNodeItem _config;
	}
}
