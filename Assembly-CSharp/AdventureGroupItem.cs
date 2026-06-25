using System;
using FrameWork.UISystem.UIElements;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000188 RID: 392
public class AdventureGroupItem : MonoBehaviour
{
	// Token: 0x06001617 RID: 5655 RVA: 0x00088D74 File Offset: 0x00086F74
	public void Setup(AdventureGroupSnapshot group, int index, int currentGroupIndex, Action onRefresh)
	{
		this._group = group;
		this._index = index;
		this._onRefresh = onRefresh;
		this.indexText.text = index.ToString();
		this.weightInput.SetTextWithoutNotify(group.Weight.ToString());
		this.weightInput.onEndEdit.RemoveAllListeners();
		this.weightInput.onEndEdit.AddListener(new UnityAction<string>(this.OnWeightChanged));
		this.commentInput.SetTextWithoutNotify(group.Comment ?? string.Empty);
		this.commentInput.onEndEdit.RemoveAllListeners();
		this.commentInput.onEndEdit.AddListener(new UnityAction<string>(this.OnCommentChanged));
		this.deleteBtn.ClearAndAddListener(new Action(this.OnDeleteClicked));
		this.switchBtn.ClearAndAddListener(new Action(this.OnSwitchClicked));
		this.moveUpBtn.ClearAndAddListener(new Action(this.OnMoveUpClicked));
		this.moveDownBtn.ClearAndAddListener(new Action(this.OnMoveDownClicked));
		this.cloneBtn.ClearAndAddListener(new Action(this.OnCloneClicked));
		int groupCount = AdventureEditorKit.BlackBoard.GroupCount;
		this.deleteBtn.interactable = (groupCount > 1);
		this.moveUpBtn.interactable = (index > 0);
		this.moveDownBtn.interactable = (index < groupCount - 1);
		this.selectedObj.SetActive(index == currentGroupIndex);
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x00088F00 File Offset: 0x00087100
	private void OnWeightChanged(string value)
	{
		uint weight;
		bool flag = uint.TryParse(value, out weight);
		if (flag)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				bool flag2 = this._index < snapshot.Groups.Count;
				if (flag2)
				{
					snapshot.Groups[this._index].Weight = weight;
				}
			}, EAdventureEditType.Groups);
		}
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x00088F48 File Offset: 0x00087148
	private void OnCommentChanged(string value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			bool flag = this._index < snapshot.Groups.Count;
			if (flag)
			{
				snapshot.Groups[this._index].Comment = value;
			}
		}, EAdventureEditType.Groups);
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x00088F83 File Offset: 0x00087183
	private void OnDeleteClicked()
	{
		AdventureEditorKit.BlackBoard.DeleteGroup(this._index);
		Action onRefresh = this._onRefresh;
		if (onRefresh != null)
		{
			onRefresh();
		}
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x00088FA9 File Offset: 0x000871A9
	private void OnSwitchClicked()
	{
		AdventureEditorKit.BlackBoard.SwitchGroup(this._index);
		Action onRefresh = this._onRefresh;
		if (onRefresh != null)
		{
			onRefresh();
		}
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x00088FD0 File Offset: 0x000871D0
	private void OnMoveUpClicked()
	{
		bool flag = this._index <= 0;
		if (!flag)
		{
			AdventureEditorKit.BlackBoard.MoveGroup(this._index, this._index - 1);
			Action onRefresh = this._onRefresh;
			if (onRefresh != null)
			{
				onRefresh();
			}
		}
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x0008901C File Offset: 0x0008721C
	private void OnMoveDownClicked()
	{
		bool flag = this._index >= AdventureEditorKit.BlackBoard.GroupCount - 1;
		if (!flag)
		{
			AdventureEditorKit.BlackBoard.MoveGroup(this._index, this._index + 1);
			Action onRefresh = this._onRefresh;
			if (onRefresh != null)
			{
				onRefresh();
			}
		}
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x00089072 File Offset: 0x00087272
	private void OnCloneClicked()
	{
		AdventureEditorKit.BlackBoard.CloneGroup(this._index);
		Action onRefresh = this._onRefresh;
		if (onRefresh != null)
		{
			onRefresh();
		}
	}

	// Token: 0x0400120F RID: 4623
	[SerializeField]
	private TextMeshProUGUI indexText;

	// Token: 0x04001210 RID: 4624
	[SerializeField]
	private TMP_InputField weightInput;

	// Token: 0x04001211 RID: 4625
	[SerializeField]
	private TMP_InputField commentInput;

	// Token: 0x04001212 RID: 4626
	[SerializeField]
	private CButton deleteBtn;

	// Token: 0x04001213 RID: 4627
	[SerializeField]
	private CButton switchBtn;

	// Token: 0x04001214 RID: 4628
	[SerializeField]
	private CButton moveUpBtn;

	// Token: 0x04001215 RID: 4629
	[SerializeField]
	private CButton moveDownBtn;

	// Token: 0x04001216 RID: 4630
	[SerializeField]
	private CButton cloneBtn;

	// Token: 0x04001217 RID: 4631
	[SerializeField]
	private GameObject selectedObj;

	// Token: 0x04001218 RID: 4632
	private AdventureGroupSnapshot _group;

	// Token: 0x04001219 RID: 4633
	private int _index;

	// Token: 0x0400121A RID: 4634
	private Action _onRefresh;
}
