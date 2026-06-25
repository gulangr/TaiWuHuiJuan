using System;
using System.Collections.Generic;
using EasyButtons;
using FrameWork;
using GameData.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000674 RID: 1652
	public class AiEditorMainArea : Refers, ISelectAndDragHandler, IAiNodeTemplateHandler
	{
		// Token: 0x06004DF4 RID: 19956 RVA: 0x0024B8A1 File Offset: 0x00249AA1
		private void Awake()
		{
			this.selectAndDrag.Bind(this);
		}

		// Token: 0x06004DF5 RID: 19957 RVA: 0x0024B8B4 File Offset: 0x00249AB4
		private void Update()
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.AiEditor) || this._blockInput;
			if (!flag)
			{
				bool keyUp = Input.GetKeyUp(KeyCode.R);
				if (keyUp)
				{
					this.ResetArea();
				}
				bool keyUp2 = Input.GetKeyUp(KeyCode.D);
				if (keyUp2)
				{
					this.Edit();
				}
				bool keyUp3 = Input.GetKeyUp(KeyCode.J);
				if (keyUp3)
				{
					this.Jump();
				}
				bool keyUp4 = Input.GetKeyUp(KeyCode.X);
				if (keyUp4)
				{
					this.RemoveAt();
				}
				bool keyUp5 = Input.GetKeyUp(KeyCode.C);
				if (keyUp5)
				{
					this.Copy();
				}
				bool keyUp6 = Input.GetKeyUp(KeyCode.V);
				if (keyUp6)
				{
					this.Paste();
				}
				bool keyUp7 = Input.GetKeyUp(KeyCode.F);
				if (keyUp7)
				{
					this.Find();
				}
				bool flag2 = this.selectAndDrag.Dragging || this.selectAndDrag.MultiSelecting;
				if (!flag2)
				{
					bool keyUp8 = Input.GetKeyUp(KeyCode.Delete);
					if (keyUp8)
					{
						this.Clear();
					}
					bool keyUp9 = Input.GetKeyUp(KeyCode.Z);
					if (keyUp9)
					{
						this.TryRevert();
					}
					bool keyUp10 = Input.GetKeyUp(KeyCode.Y);
					if (keyUp10)
					{
						this.TryEvolve();
					}
				}
			}
		}

		// Token: 0x06004DF6 RID: 19958 RVA: 0x0024B9CC File Offset: 0x00249BCC
		public void Init()
		{
			this._selectingNode = -1;
			this._relatingNode = -1;
			this._nodeCount = 1;
			this.GetNode(0).Set(this, 0, 0);
			this.GetNode(0).RectTransform.anchoredPosition = Vector2.zero;
			this.ResetArea();
			this._commandRecord.Clear();
			this.EnsureActive();
		}

		// Token: 0x06004DF7 RID: 19959 RVA: 0x0024BA30 File Offset: 0x00249C30
		public AiBlueprintSnapshot Save()
		{
			List<AiNodeDataSnapshot> data = new List<AiNodeDataSnapshot>(this._nodeCount);
			for (int i = 0; i < this._nodeCount; i++)
			{
				data.Add(this.GetNode(i).Save());
			}
			return new AiBlueprintSnapshot(data);
		}

		// Token: 0x06004DF8 RID: 19960 RVA: 0x0024BA7C File Offset: 0x00249C7C
		public void Load(AiBlueprintSnapshot snapshot)
		{
			this._commandRecord.Clear();
			this.LoadInternal(snapshot);
			this.ResetArea();
		}

		// Token: 0x06004DF9 RID: 19961 RVA: 0x0024BA9C File Offset: 0x00249C9C
		private void LoadInternal(AiBlueprintSnapshot snapshot)
		{
			List<AiNodeDataSnapshot> data = snapshot.Data;
			this._nodeCount = data.Count;
			for (int i = this.target.childCount; i < data.Count; i++)
			{
				Object.Instantiate<Transform>(this.target.GetChild(0), this.target);
			}
			for (int j = 0; j < data.Count; j++)
			{
				this.GetNode(j).Load(this, data[j]);
			}
			this.EnsureActive();
		}

		// Token: 0x06004DFA RID: 19962 RVA: 0x0024BB27 File Offset: 0x00249D27
		private AiNodeTemplate GetNode(int index)
		{
			return this.target.GetChild(index).GetComponent<AiNodeTemplate>();
		}

		// Token: 0x06004DFB RID: 19963 RVA: 0x0024BB3C File Offset: 0x00249D3C
		private void EnsureActive()
		{
			for (int i = 0; i < this._nodeCount; i++)
			{
				bool flag = !this.target.GetChild(i).gameObject.activeSelf;
				if (flag)
				{
					this.target.GetChild(i).gameObject.SetActive(true);
				}
			}
			for (int j = this._nodeCount; j < this.target.childCount; j++)
			{
				bool activeSelf = this.target.GetChild(j).gameObject.activeSelf;
				if (activeSelf)
				{
					this.target.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06004DFC RID: 19964 RVA: 0x0024BBE9 File Offset: 0x00249DE9
		[Button]
		private void Test(EAiNodeType type)
		{
			this.GetNode(0).Set(this, 0, (int)type);
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x0024BBFC File Offset: 0x00249DFC
		public void BlockInput()
		{
			this._blockInput = true;
		}

		// Token: 0x06004DFE RID: 19966 RVA: 0x0024BC05 File Offset: 0x00249E05
		public void UnBlockInput()
		{
			this._blockInput = false;
		}

		// Token: 0x06004DFF RID: 19967 RVA: 0x0024BC0E File Offset: 0x00249E0E
		private void ResetArea()
		{
			this.target.localScale = Vector3.one;
			this.JumpNode(0);
		}

		// Token: 0x06004E00 RID: 19968 RVA: 0x0024BC2C File Offset: 0x00249E2C
		private void Edit()
		{
			bool flag = this._relatingNode < 0;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MinValue", 0).Set("MaxValue", this._nodeCount - 1);
				argBox.SetObject("SelectCallback", new Action<int>(this.ApplyNodeRelate));
				UIElement.AiShortNumberInputField.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.AiShortNumberInputField, true);
			}
		}

		// Token: 0x06004E01 RID: 19969 RVA: 0x0024BCA4 File Offset: 0x00249EA4
		private void Jump()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MinValue", 0).Set("MaxValue", this._nodeCount - 1);
			argBox.SetObject("SelectCallback", new Action<int>(this.JumpNode));
			UIElement.AiShortNumberInputField.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.AiShortNumberInputField, true);
		}

		// Token: 0x06004E02 RID: 19970 RVA: 0x0024BD0C File Offset: 0x00249F0C
		private void Clear()
		{
			bool flag = this._relatingNode >= 0;
			if (flag)
			{
				this.ApplyNodeRelate(-1);
			}
			else
			{
				bool flag2 = this._selectingNode >= 0;
				if (flag2)
				{
					this.ClearOrDeleteSelect();
				}
			}
		}

		// Token: 0x06004E03 RID: 19971 RVA: 0x0024BD4C File Offset: 0x00249F4C
		private void RemoveAt()
		{
			bool flag = this._selectingNode < 0 || this._relatingNode >= 0;
			if (!flag)
			{
				AiNodeTemplate node = this.GetNode(this._selectingNode);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MinValue", 0).Set("MaxValue", node.NextCount - 1);
				argBox.SetObject("SelectCallback", new Action<int>(node.RemoveAt));
				UIElement.AiShortNumberInputField.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.AiShortNumberInputField, true);
			}
		}

		// Token: 0x06004E04 RID: 19972 RVA: 0x0024BDE0 File Offset: 0x00249FE0
		private void Copy()
		{
			this._copyingNode = null;
			bool flag = this._selectingNode >= 0;
			if (flag)
			{
				this._copyingNode = this.GetNode(this._selectingNode).Save();
			}
		}

		// Token: 0x06004E05 RID: 19973 RVA: 0x0024BE1C File Offset: 0x0024A01C
		private void Paste()
		{
			bool flag = this._copyingNode == null;
			if (!flag)
			{
				this._copyingNode.RuntimeId = this.AppendNode(this._copyingNode.Type);
				this.GetNode(this._copyingNode.RuntimeId).Load(this, this._copyingNode);
				this._commandRecord.RecordAddNode(this._copyingNode.RuntimeId, this._copyingNode);
			}
		}

		// Token: 0x06004E06 RID: 19974 RVA: 0x0024BE90 File Offset: 0x0024A090
		private void Find()
		{
			bool flag = this._relatingNode >= 0;
			if (!flag)
			{
				int startAt = (this._selectingNode < 0) ? 0 : this._selectingNode;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("StartAt", startAt);
				argBox.SetObject("TryNext", new AiSearchTryNextDelegate(this.SearchAndJumpNext));
				UIElement.AiSearchInputField.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.AiSearchInputField, true);
			}
		}

		// Token: 0x06004E07 RID: 19975 RVA: 0x0024BF0C File Offset: 0x0024A10C
		private void TryRevert()
		{
			AiEditorCommandData? nullableData = this._commandRecord.Revert();
			AiEditorCommandData data;
			bool flag;
			if (nullableData != null)
			{
				data = nullableData.GetValueOrDefault();
				flag = (1 == 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				switch (data.Type)
				{
				case EAiEditorCommandType.AddNode:
					this.DeleteNode(data.RuntimeId);
					break;
				case EAiEditorCommandType.DeleteNode:
				{
					AiEditorDeleteNodeData deleteData = JsonConvert.DeserializeObject<AiEditorDeleteNodeData>(data.Data);
					this.LoadInternal(deleteData.PrevData);
					break;
				}
				case EAiEditorCommandType.ActionNode:
				{
					AiEditorActionNodeData actionData = JsonConvert.DeserializeObject<AiEditorActionNodeData>(data.Data);
					this.GetNode(data.RuntimeId).Load(this, actionData.PrevData);
					break;
				}
				case EAiEditorCommandType.MoveNode:
				{
					AiEditorMoveNodeData moveData = JsonConvert.DeserializeObject<AiEditorMoveNodeData>(data.Data);
					this.GetNode(data.RuntimeId).RectTransform.anchoredPosition = moveData.PrevPosition;
					break;
				}
				default:
					AdaptableLog.Warning(string.Format("Unsupported command type {0}", data.Type), false);
					break;
				}
			}
		}

		// Token: 0x06004E08 RID: 19976 RVA: 0x0024C010 File Offset: 0x0024A210
		private void TryEvolve()
		{
			AiEditorCommandData? nullableData = this._commandRecord.Evolve();
			AiEditorCommandData data;
			bool flag;
			if (nullableData != null)
			{
				data = nullableData.GetValueOrDefault();
				flag = (1 == 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				switch (data.Type)
				{
				case EAiEditorCommandType.AddNode:
				{
					Tester.Assert(data.RuntimeId == this._nodeCount, "");
					AiEditorAddNodeData addData = JsonConvert.DeserializeObject<AiEditorAddNodeData>(data.Data);
					this.AppendNode(addData.NodeData.Type);
					this.GetNode(data.RuntimeId).Load(this, addData.NodeData);
					break;
				}
				case EAiEditorCommandType.DeleteNode:
					this.DeleteNode(data.RuntimeId);
					break;
				case EAiEditorCommandType.ActionNode:
				{
					AiEditorActionNodeData actionData = JsonConvert.DeserializeObject<AiEditorActionNodeData>(data.Data);
					this.GetNode(data.RuntimeId).Load(this, actionData.CurrData);
					break;
				}
				case EAiEditorCommandType.MoveNode:
				{
					AiEditorMoveNodeData moveData = JsonConvert.DeserializeObject<AiEditorMoveNodeData>(data.Data);
					this.GetNode(data.RuntimeId).RectTransform.anchoredPosition = moveData.CurrPosition;
					break;
				}
				default:
					AdaptableLog.Warning(string.Format("Unsupported command type {0}", data.Type), false);
					break;
				}
			}
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x06004E09 RID: 19977 RVA: 0x0024C14B File Offset: 0x0024A34B
		int ISelectAndDragHandler.SelectedComponentCount
		{
			get
			{
				return (this._selectingNode < 0) ? 0 : 1;
			}
		}

		// Token: 0x06004E0A RID: 19978 RVA: 0x0024C15C File Offset: 0x0024A35C
		bool ISelectAndDragHandler.IsSelectedComponent(ISelectAndDragComponent component)
		{
			AiNodeTemplate node = (AiNodeTemplate)component;
			return node.RuntimeId == this._selectingNode;
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x06004E0B RID: 19979 RVA: 0x0024C184 File Offset: 0x0024A384
		IEnumerable<ISelectAndDragComponent> ISelectAndDragHandler.SelectedComponents
		{
			get
			{
				bool flag = this._selectingNode < 0;
				if (flag)
				{
					yield break;
				}
				yield return this.GetNode(this._selectingNode);
				yield break;
			}
		}

		// Token: 0x06004E0C RID: 19980 RVA: 0x0024C1A4 File Offset: 0x0024A3A4
		void ISelectAndDragHandler.Select(ISelectAndDragComponent component)
		{
			AiNodeTemplate node = (AiNodeTemplate)component;
			bool flag = this._relatingNode >= 0;
			if (flag)
			{
				this.ApplyNodeRelate(node.RuntimeId);
			}
			else
			{
				AiNodeRelate relate = node.TryGetSelectingRelate();
				bool flag2 = relate != null;
				if (flag2)
				{
					relate.Invoke();
				}
				else
				{
					this.SelectNode(node.RuntimeId);
				}
			}
		}

		// Token: 0x06004E0D RID: 19981 RVA: 0x0024C204 File Offset: 0x0024A404
		void ISelectAndDragHandler.Unselect(ISelectAndDragComponent component)
		{
			bool flag = this._relatingNode >= 0;
			if (!flag)
			{
				AiNodeTemplate node = (AiNodeTemplate)component;
				bool flag2 = node.RuntimeId != this._selectingNode;
				if (!flag2)
				{
					node.Unselect();
					this._selectingNode = -1;
				}
			}
		}

		// Token: 0x06004E0E RID: 19982 RVA: 0x0024C250 File Offset: 0x0024A450
		void ISelectAndDragHandler.SelectEmpty()
		{
			this.ClearRelatingAndSelectingNode();
		}

		// Token: 0x06004E0F RID: 19983 RVA: 0x0024C25A File Offset: 0x0024A45A
		void ISelectAndDragHandler.BeginMultiSelect(Vector2 startPos)
		{
		}

		// Token: 0x06004E10 RID: 19984 RVA: 0x0024C25D File Offset: 0x0024A45D
		void ISelectAndDragHandler.OnMultiSelect(Vector2 startPos, Vector2 curPos)
		{
		}

		// Token: 0x06004E11 RID: 19985 RVA: 0x0024C260 File Offset: 0x0024A460
		void ISelectAndDragHandler.EndMultiSelect()
		{
		}

		// Token: 0x06004E12 RID: 19986 RVA: 0x0024C263 File Offset: 0x0024A463
		void ISelectAndDragHandler.BeginDrag()
		{
		}

		// Token: 0x06004E13 RID: 19987 RVA: 0x0024C268 File Offset: 0x0024A468
		void ISelectAndDragHandler.EndDrag(ISelectAndDragComponent component, Vector2 startPos, Vector2 endPos)
		{
			bool flag = component == null;
			if (!flag)
			{
				AiNodeTemplate node = (AiNodeTemplate)component;
				this._commandRecord.RecordMoveNode(node.RuntimeId, startPos, endPos);
			}
		}

		// Token: 0x06004E14 RID: 19988 RVA: 0x0024C29B File Offset: 0x0024A49B
		void ISelectAndDragHandler.ActionContext()
		{
		}

		// Token: 0x06004E15 RID: 19989 RVA: 0x0024C29E File Offset: 0x0024A49E
		void ISelectAndDragHandler.ActionComponents()
		{
		}

		// Token: 0x06004E16 RID: 19990 RVA: 0x0024C2A4 File Offset: 0x0024A4A4
		void IAiNodeTemplateHandler.NodeRelate(int runtimeId)
		{
			bool flag = this._relatingNode >= 0;
			if (flag)
			{
				this.GetNode(this._relatingNode).InterruptRelate();
			}
			this._relatingNode = runtimeId;
			bool flag2 = this._selectingNode >= 0;
			if (flag2)
			{
				this.GetNode(this._selectingNode).Unselect();
			}
		}

		// Token: 0x06004E17 RID: 19991 RVA: 0x0024C300 File Offset: 0x0024A500
		void IAiNodeTemplateHandler.NodeAction(int runtimeId, Action action)
		{
			this.ActionNode(runtimeId, delegate(AiNodeTemplate _)
			{
				action();
			});
		}

		// Token: 0x06004E18 RID: 19992 RVA: 0x0024C32F File Offset: 0x0024A52F
		private void JumpNode(int runtimeId)
		{
			this.target.anchoredPosition = this.GetNode(runtimeId).RectTransform.anchoredPosition * (-1f * this.target.localScale.x);
		}

		// Token: 0x06004E19 RID: 19993 RVA: 0x0024C36C File Offset: 0x0024A56C
		private int SearchAndJumpNext(int startAt, string text)
		{
			int next = this.TrySearchNext(startAt, text);
			bool flag = next < 0;
			int result;
			if (flag)
			{
				result = startAt;
			}
			else
			{
				this.JumpNode(next);
				result = next;
			}
			return result;
		}

		// Token: 0x06004E1A RID: 19994 RVA: 0x0024C39C File Offset: 0x0024A59C
		private int TrySearchNext(int startAt, string text)
		{
			foreach (int runtimeId in this.IterAllNodes(startAt + 1))
			{
				bool flag = this.IsNodeContainsText(runtimeId, text);
				if (flag)
				{
					return runtimeId;
				}
			}
			return -1;
		}

		// Token: 0x06004E1B RID: 19995 RVA: 0x0024C400 File Offset: 0x0024A600
		private IEnumerable<int> IterAllNodes(int startAt)
		{
			int num;
			for (int i = startAt; i < this._nodeCount; i = num + 1)
			{
				yield return i;
				num = i;
			}
			for (int j = 0; j < startAt; j = num + 1)
			{
				yield return j;
				num = j;
			}
			yield break;
		}

		// Token: 0x06004E1C RID: 19996 RVA: 0x0024C418 File Offset: 0x0024A618
		private bool IsNodeContainsText(int runtimeId, string text)
		{
			AiNodeTemplate node = this.GetNode(runtimeId);
			return node.Contains(text);
		}

		// Token: 0x06004E1D RID: 19997 RVA: 0x0024C43C File Offset: 0x0024A63C
		private Vector2 CalcNewNodePosition()
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			RectTransform newNodePosition = base.CGet<RectTransform>("NewNodePosition");
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, newNodePosition.position);
			Vector2 point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.target, screenPoint, uiCamera, out point);
			return point;
		}

		// Token: 0x06004E1E RID: 19998 RVA: 0x0024C488 File Offset: 0x0024A688
		private void ClearOrDeleteSelect()
		{
			AiNodeTemplate node = this.GetNode(this._selectingNode);
			bool flag = node.NextCount > 0;
			if (flag)
			{
				this.ActionNode(this._selectingNode, delegate(AiNodeTemplate x)
				{
					x.Clear();
				});
			}
			else
			{
				bool flag2 = this._selectingNode != 0;
				if (flag2)
				{
					this._commandRecord.RecordDeleteNode(this._selectingNode, this.Save());
					this.DeleteNode(this._selectingNode);
				}
			}
		}

		// Token: 0x06004E1F RID: 19999 RVA: 0x0024C514 File Offset: 0x0024A714
		private void ClearRelatingAndSelectingNode()
		{
			bool flag = this._relatingNode >= 0;
			if (flag)
			{
				this.GetNode(this._relatingNode).InterruptRelate();
				this._relatingNode = -1;
			}
			bool flag2 = this._selectingNode >= 0;
			if (flag2)
			{
				this.GetNode(this._selectingNode).Unselect();
				this._selectingNode = -1;
			}
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x0024C578 File Offset: 0x0024A778
		private void SelectNode(int runtimeId)
		{
			bool flag = this._selectingNode >= 0;
			if (flag)
			{
				this.GetNode(this._selectingNode).Unselect();
			}
			this._selectingNode = runtimeId;
			bool flag2 = this._selectingNode >= 0;
			if (flag2)
			{
				this.GetNode(this._selectingNode).Select();
			}
		}

		// Token: 0x06004E21 RID: 20001 RVA: 0x0024C5D4 File Offset: 0x0024A7D4
		private void ApplyNodeRelate(int runtimeId)
		{
			bool flag = this._relatingNode == runtimeId;
			if (!flag)
			{
				this.ActionNode(this._relatingNode, delegate(AiNodeTemplate node)
				{
					node.ApplyRelate(runtimeId);
				});
				this._relatingNode = -1;
				bool flag2 = this._selectingNode >= 0;
				if (flag2)
				{
					this.GetNode(this._selectingNode).Select();
				}
			}
		}

		// Token: 0x06004E22 RID: 20002 RVA: 0x0024C648 File Offset: 0x0024A848
		private void ActionNode(int runtimeId, Action<AiNodeTemplate> action)
		{
			AiNodeTemplate node = this.GetNode(runtimeId);
			AiNodeDataSnapshot prevData = node.Save();
			action(node);
			AiNodeDataSnapshot currData = node.Save();
			this._commandRecord.RecordActionNode(runtimeId, prevData, currData);
		}

		// Token: 0x06004E23 RID: 20003 RVA: 0x0024C684 File Offset: 0x0024A884
		private void DeleteNode(int runtimeId)
		{
			this.ClearRelatingAndSelectingNode();
			AiNodeTemplate node = this.GetNode(runtimeId);
			node.gameObject.SetActive(false);
			node.transform.SetAsLastSibling();
			Dictionary<int, int> dict = new Dictionary<int, int>();
			dict[runtimeId] = -1;
			for (int i = runtimeId + 1; i < this._nodeCount; i++)
			{
				dict[i] = i - 1;
			}
			foreach (AiNodeRelate relate in base.GetComponentsInChildren<AiNodeRelate>())
			{
				relate.Sync(dict);
			}
			for (int j = runtimeId; j < this.target.childCount; j++)
			{
				AiNodeTemplate otherNode = this.GetNode(j);
				bool activeSelf = otherNode.gameObject.activeSelf;
				if (activeSelf)
				{
					otherNode.Sync(dict);
				}
			}
			this._nodeCount--;
		}

		// Token: 0x06004E24 RID: 20004 RVA: 0x0024C76C File Offset: 0x0024A96C
		private int AppendNode(int nodeId)
		{
			this._nodeCount++;
			bool flag = this._nodeCount > this.target.childCount;
			if (flag)
			{
				Object.Instantiate<Transform>(this.target.GetChild(0), this.target);
			}
			int runtimeId = this._nodeCount - 1;
			AiNodeTemplate node = this.GetNode(runtimeId);
			node.Set(this, runtimeId, nodeId);
			node.RectTransform.anchoredPosition = this.CalcNewNodePosition();
			this.EnsureActive();
			return runtimeId;
		}

		// Token: 0x06004E25 RID: 20005 RVA: 0x0024C7F0 File Offset: 0x0024A9F0
		public void AppendLinear(int nodeId)
		{
			bool flag = this._selectingNode >= 0;
			if (flag)
			{
				this.ActionNode(this._selectingNode, delegate(AiNodeTemplate node)
				{
					node.AppendLinear().AppendRelay();
				});
			}
			else
			{
				int runtimeId = this.AppendNode(nodeId);
				this._commandRecord.RecordAddNode(runtimeId, this.GetNode(runtimeId).Save());
			}
		}

		// Token: 0x06004E26 RID: 20006 RVA: 0x0024C860 File Offset: 0x0024AA60
		public void AppendBranch(int conditionId)
		{
			bool flag = this._selectingNode < 0;
			if (!flag)
			{
				this.ActionNode(this._selectingNode, delegate(AiNodeTemplate node)
				{
					node.AppendBranch(conditionId);
				});
			}
		}

		// Token: 0x06004E27 RID: 20007 RVA: 0x0024C8A4 File Offset: 0x0024AAA4
		public void AppendAction(int actionId)
		{
			bool flag = this._selectingNode < 0;
			if (!flag)
			{
				this.ActionNode(this._selectingNode, delegate(AiNodeTemplate node)
				{
					node.AppendAction(actionId);
				});
			}
		}

		// Token: 0x0400360A RID: 13834
		public RectTransform target;

		// Token: 0x0400360B RID: 13835
		[SerializeField]
		private AiEditorSelectAndDrag selectAndDrag;

		// Token: 0x0400360C RID: 13836
		private int _nodeCount;

		// Token: 0x0400360D RID: 13837
		private int _selectingNode;

		// Token: 0x0400360E RID: 13838
		private int _relatingNode;

		// Token: 0x0400360F RID: 13839
		private AiNodeDataSnapshot _copyingNode;

		// Token: 0x04003610 RID: 13840
		private bool _blockInput;

		// Token: 0x04003611 RID: 13841
		private readonly AiEditorCommandRecord _commandRecord = new AiEditorCommandRecord();
	}
}
