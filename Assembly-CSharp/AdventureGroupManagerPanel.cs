using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Adventure.Editor;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class AdventureGroupManagerPanel : MonoBehaviour, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x06001620 RID: 5664 RVA: 0x000890A4 File Offset: 0x000872A4
	private void Awake()
	{
		this.addGroupBtn.ClearAndAddListener(new Action(this.OnAddGroup));
		this.closeBtn.ClearAndAddListener(new Action(this.OnClose));
		this.groupScroll.OnItemRender += this.OnGroupItemRender;
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000890FA File Offset: 0x000872FA
	private void OnEnable()
	{
		AdventureEditorKit.BlackBoard.Register(this);
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x00089109 File Offset: 0x00087309
	private void OnDisable()
	{
		AdventureEditorKit.BlackBoard.Unregister(this);
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x00089118 File Offset: 0x00087318
	public void Refresh()
	{
		this._groups = AdventureEditorKit.BlackBoard.Editing.Groups;
		this.groupScroll.SetDataCount(this._groups.Count);
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x00089147 File Offset: 0x00087347
	private void OnAddGroup()
	{
		AdventureEditorKit.BlackBoard.AddGroup();
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x00089155 File Offset: 0x00087355
	private void OnClose()
	{
		GEvent.OnEvent(UiEvents.AdventureEditorToggleGroupPanel, null);
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x0008916C File Offset: 0x0008736C
	private void OnGroupItemRender(int index, GameObject item)
	{
		AdventureGroupItem groupItem = item.GetComponent<AdventureGroupItem>();
		bool flag = groupItem != null;
		if (flag)
		{
			groupItem.Setup(this._groups[index], index, AdventureEditorKit.BlackBoard.CurrentGroupIndex, new Action(this.Refresh));
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06001627 RID: 5671 RVA: 0x000891B8 File Offset: 0x000873B8
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x000891BC File Offset: 0x000873BC
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.Groups);
		if (flag)
		{
			this.Refresh();
		}
	}

	// Token: 0x0400121B RID: 4635
	[SerializeField]
	private InfinityScroll groupScroll;

	// Token: 0x0400121C RID: 4636
	[SerializeField]
	private CButton addGroupBtn;

	// Token: 0x0400121D RID: 4637
	[SerializeField]
	private CButton closeBtn;

	// Token: 0x0400121E RID: 4638
	private List<AdventureGroupSnapshot> _groups;
}
