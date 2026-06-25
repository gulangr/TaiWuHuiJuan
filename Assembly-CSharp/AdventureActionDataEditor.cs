using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameData.Adventure.Editor;
using SubEditor;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class AdventureActionDataEditor : AdventureAbstractListEditor<AdventureActionSnapshot, AdventureActionDataEditorItem>, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x060016CB RID: 5835 RVA: 0x0008B8C4 File Offset: 0x00089AC4
	protected override void OnEnable()
	{
		this.Refresh(AdventureEditorKit.BlackBoard.Editing.Actions);
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x0008B8DD File Offset: 0x00089ADD
	protected override bool CheckEmpty()
	{
		return false;
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x0008B8E0 File Offset: 0x00089AE0
	protected override void AddItem(IList<AdventureActionSnapshot> _)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			base.AddItem(snapshot.Actions);
		}, EAdventureEditType.Actions);
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x0008B8FC File Offset: 0x00089AFC
	protected override AdventureActionSnapshot ItemCreator(IList<AdventureActionSnapshot> list)
	{
		return AdventureActionDataEditor.GenerateNewItem(list);
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x0008B904 File Offset: 0x00089B04
	protected override void RefreshItem(IList<AdventureActionSnapshot> list, AdventureActionDataEditorItem editorItem, int index)
	{
		if (list == null)
		{
			list = AdventureEditorKit.BlackBoard.Editing.Actions;
		}
		AdventureActionDataEditorItem item = editorItem.GetComponent<AdventureActionDataEditorItem>();
		DisableStyleRoot disableRoot = editorItem.gameObject.GetOrAddComponent<DisableStyleRoot>();
		disableRoot.SetStyleEffect(list[index] == null, false);
		for (int i = 0; i < this.columnsHeader.childCount; i++)
		{
			RectTransform colRect = this.columnsHeader.GetChild(i).GetComponent<RectTransform>();
			RectTransform curRect = item.transform.GetChild(i).GetComponent<RectTransform>();
			curRect.sizeDelta = new Vector2(colRect.rect.size.x, curRect.rect.size.y);
		}
		item.Refresh(index, list, new Action<Action<IList<AdventureActionSnapshot>>>(AdventureActionDataEditor.<RefreshItem>g__ProcessEdit|4_0), new Action<IList<AdventureActionSnapshot>>(this.Refresh));
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x0008B9E8 File Offset: 0x00089BE8
	protected override void RefreshItem(IList<AdventureActionSnapshot> list, AdventureActionDataEditorItem editorItem, int index, bool setDisableStyle)
	{
		this.RefreshItem(list, editorItem, index);
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x0008B9F4 File Offset: 0x00089BF4
	public void Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.Actions);
		if (flag)
		{
			this.Refresh(AdventureEditorKit.BlackBoard.Editing.Actions);
		}
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x0008BA28 File Offset: 0x00089C28
	internal static AdventureActionSnapshot GenerateNewItem(IList<AdventureActionSnapshot> list)
	{
		int newKeyIndex = 0;
		bool flag;
		do
		{
			newKeyIndex++;
			string newKey = string.Format("new_var_{0}", newKeyIndex);
			flag = list.Any((AdventureActionSnapshot u) => u.Key == newKey);
		}
		while (flag);
		return new AdventureActionSnapshot
		{
			Key = string.Format("new_var_{0}", newKeyIndex),
			Name = string.Format("新建行为 # {0}", newKeyIndex),
			Desc = string.Format("这个就是新建行为{0}喔", newKeyIndex)
		};
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x0008BAE8 File Offset: 0x00089CE8
	[CompilerGenerated]
	internal static void <RefreshItem>g__ProcessEdit|4_0(Action<IList<AdventureActionSnapshot>> proc)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			proc(snapshot.Actions);
		}, EAdventureEditType.Actions);
	}
}
