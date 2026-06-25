using System;
using System.Collections.Generic;
using EventEditor;
using GameData.Adventure;
using GameData.Adventure.Editor;
using UnityEngine;

namespace SubEditor
{
	// Token: 0x02000694 RID: 1684
	public class AdventureAutoEventEditor : AdventureAbstractListEditor<AdventureAutoEventSnapshot, AdventureAutoEventEditorItem>, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
	{
		// Token: 0x06004F1B RID: 20251 RVA: 0x00250874 File Offset: 0x0024EA74
		protected override void Start()
		{
			bool awaken = this._awaken;
			if (!awaken)
			{
				base.Start();
				EventEditorScript.Init(this._editorScript);
				this._awaken = true;
			}
		}

		// Token: 0x06004F1C RID: 20252 RVA: 0x002508A8 File Offset: 0x0024EAA8
		protected override bool CheckEmpty()
		{
			return false;
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x002508AB File Offset: 0x0024EAAB
		protected override void OnEnable()
		{
			this.Start();
			this.Refresh(AdventureEditorKit.BlackBoard.Editing.AutoEvents);
			AdventureEditorKit.FixLayerSortingOrder(base.gameObject, base.GetComponentInChildren<Canvas>());
		}

		// Token: 0x06004F1E RID: 20254 RVA: 0x002508DD File Offset: 0x0024EADD
		protected void OnDisable()
		{
			AdventureEditorKit.RestoreLayerSortingOrder(base.gameObject);
		}

		// Token: 0x06004F1F RID: 20255 RVA: 0x002508EC File Offset: 0x0024EAEC
		protected override void AddItem(IList<AdventureAutoEventSnapshot> _)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				base.AddItem(snapshot.AutoEvents);
			}, EAdventureEditType.AutoEvents);
		}

		// Token: 0x06004F20 RID: 20256 RVA: 0x00250907 File Offset: 0x0024EB07
		protected override AdventureAutoEventSnapshot ItemCreator(IList<AdventureAutoEventSnapshot> list)
		{
			return new AdventureAutoEventSnapshot
			{
				Event = new AdventureEventSnapshot
				{
					OnlyOnce = false,
					Guid = string.Empty,
					Comment = string.Empty
				},
				TriggerType = EAdventureAutoEventTriggerType.PlayerMove
			};
		}

		// Token: 0x06004F21 RID: 20257 RVA: 0x00250940 File Offset: 0x0024EB40
		protected override void RefreshItem(IList<AdventureAutoEventSnapshot> list, AdventureAutoEventEditorItem editorItem, int index)
		{
			if (list == null)
			{
				list = AdventureEditorKit.BlackBoard.Editing.AutoEvents;
			}
			DisableStyleRoot disableRoot = editorItem.gameObject.GetOrAddComponent<DisableStyleRoot>();
			disableRoot.SetStyleEffect(list[index] == null, false);
			for (int i = 0; i < this.columnsHeader.childCount; i++)
			{
				RectTransform colRect = this.columnsHeader.GetChild(i).GetComponent<RectTransform>();
				RectTransform curRect = editorItem.transform.GetChild(i).GetComponent<RectTransform>();
				curRect.sizeDelta = new Vector2(colRect.rect.size.x, curRect.rect.size.y);
			}
			editorItem.Refresh(list, index, this._editorScript, new Action<IList<AdventureAutoEventSnapshot>>(this.Refresh));
		}

		// Token: 0x06004F22 RID: 20258 RVA: 0x00250A12 File Offset: 0x0024EC12
		protected override void RefreshItem(IList<AdventureAutoEventSnapshot> list, AdventureAutoEventEditorItem editorItem, int index, bool setDisableStyle)
		{
			this.RefreshItem(list, editorItem, index);
		}

		// Token: 0x06004F23 RID: 20259 RVA: 0x00250A20 File Offset: 0x0024EC20
		void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
		{
			bool flag = editType.Contains(EAdventureEditType.AutoEvents);
			if (flag)
			{
				this.Refresh(AdventureEditorKit.BlackBoard.Editing.AutoEvents);
			}
		}

		// Token: 0x04003667 RID: 13927
		[SerializeField]
		private EventEditorScript _editorScript;

		// Token: 0x04003668 RID: 13928
		private int _topLayerSort;

		// Token: 0x04003669 RID: 13929
		private bool _awaken;
	}
}
