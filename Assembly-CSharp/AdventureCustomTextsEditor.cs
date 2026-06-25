using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Adventure.Editor;
using SubEditor;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class AdventureCustomTextsEditor : AdventureAbstractListEditor<AdventureTextSnapshot, AdventureCustomTextsEditorItem>, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x060016D9 RID: 5849 RVA: 0x0008BDAA File Offset: 0x00089FAA
	protected override void OnEnable()
	{
		this.Refresh(AdventureEditorKit.BlackBoard.Editing.CustomTexts);
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x0008BDC3 File Offset: 0x00089FC3
	protected override bool CheckEmpty()
	{
		return false;
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0008BDC6 File Offset: 0x00089FC6
	protected override void AddItem(IList<AdventureTextSnapshot> _)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			base.AddItem(snapshot.CustomTexts);
		}, EAdventureEditType.Actions);
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0008BDE2 File Offset: 0x00089FE2
	protected override AdventureTextSnapshot ItemCreator(IList<AdventureTextSnapshot> list)
	{
		return AdventureCustomTextsEditor.GenerateNewItem(list);
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x0008BDEC File Offset: 0x00089FEC
	protected override void RefreshItem(IList<AdventureTextSnapshot> list, AdventureCustomTextsEditorItem editorItem, int index)
	{
		if (list == null)
		{
			list = AdventureEditorKit.BlackBoard.Editing.CustomTexts;
		}
		AdventureCustomTextsEditorItem item = editorItem.GetComponent<AdventureCustomTextsEditorItem>();
		DisableStyleRoot disableRoot = editorItem.gameObject.GetOrAddComponent<DisableStyleRoot>();
		disableRoot.SetStyleEffect(list[index] == null, false);
		for (int i = 0; i < this.columnsHeader.childCount; i++)
		{
			RectTransform colRect = this.columnsHeader.GetChild(i).GetComponent<RectTransform>();
			RectTransform curRect = item.transform.GetChild(i).GetComponent<RectTransform>();
			curRect.sizeDelta = new Vector2(colRect.rect.size.x, curRect.rect.size.y);
		}
		item.Refresh(index, list, new Action<Action<IList<AdventureTextSnapshot>>>(AdventureCustomTextsEditor.<RefreshItem>g__ProcessEdit|4_0), new Action<IList<AdventureTextSnapshot>>(this.Refresh));
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x0008BED0 File Offset: 0x0008A0D0
	protected override void RefreshItem(IList<AdventureTextSnapshot> list, AdventureCustomTextsEditorItem editorItem, int index, bool setDisableStyle)
	{
		this.RefreshItem(list, editorItem, index);
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x0008BEDC File Offset: 0x0008A0DC
	public void Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.CustomTexts);
		if (flag)
		{
			this.Refresh(AdventureEditorKit.BlackBoard.Editing.CustomTexts);
		}
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x0008BF10 File Offset: 0x0008A110
	private static AdventureTextSnapshot GenerateNewItem(IList<AdventureTextSnapshot> list)
	{
		return new AdventureTextSnapshot
		{
			Key = "",
			Text = "",
			Priority = 0,
			OnlyOnce = false,
			Comment = ""
		};
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x0008BF74 File Offset: 0x0008A174
	[CompilerGenerated]
	internal static void <RefreshItem>g__ProcessEdit|4_0(Action<IList<AdventureTextSnapshot>> proc)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			proc(snapshot.CustomTexts);
		}, EAdventureEditType.CustomTexts);
	}
}
