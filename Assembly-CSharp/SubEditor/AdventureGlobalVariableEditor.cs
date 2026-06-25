using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Adventure.Editor;
using UnityEngine;

namespace SubEditor
{
	// Token: 0x02000698 RID: 1688
	public class AdventureGlobalVariableEditor : AdventureAbstractListEditor<AdventureParameterSnapshot, AdventureGlobalVariableEditorItem>, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
	{
		// Token: 0x06004F41 RID: 20289 RVA: 0x002515B4 File Offset: 0x0024F7B4
		protected override void OnEnable()
		{
			this.Refresh(AdventureEditorKit.BlackBoard.Editing.Parameters);
			this.resetViewButton.ClearAndAddListener(delegate
			{
				AdventureEditorKit.BlackBoard.MakeEdit(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction(this.ResetViewData), EAdventureEditType.Parameters);
			});
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x002515E5 File Offset: 0x0024F7E5
		protected override bool CheckEmpty()
		{
			return false;
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x002515E8 File Offset: 0x0024F7E8
		protected override void AddItem(IList<AdventureParameterSnapshot> _)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				base.AddItem(snapshot.Parameters);
			}, EAdventureEditType.Parameters);
		}

		// Token: 0x06004F44 RID: 20292 RVA: 0x00251603 File Offset: 0x0024F803
		protected override AdventureParameterSnapshot ItemCreator(IList<AdventureParameterSnapshot> list)
		{
			return AdventureGlobalVariableEditor.GenerateNewItem(list);
		}

		// Token: 0x06004F45 RID: 20293 RVA: 0x0025160C File Offset: 0x0024F80C
		protected override void RefreshItem(IList<AdventureParameterSnapshot> list, AdventureGlobalVariableEditorItem editorItem, int index)
		{
			if (list == null)
			{
				list = AdventureEditorKit.BlackBoard.Editing.Parameters;
			}
			AdventureGlobalVariableEditorItem item = editorItem.GetComponent<AdventureGlobalVariableEditorItem>();
			DisableStyleRoot disableRoot = editorItem.gameObject.GetOrAddComponent<DisableStyleRoot>();
			disableRoot.SetStyleEffect(list[index] == null, false);
			for (int i = 0; i < this.columnsHeader.childCount; i++)
			{
				RectTransform colRect = this.columnsHeader.GetChild(i).GetComponent<RectTransform>();
				RectTransform curRect = item.transform.GetChild(i).GetComponent<RectTransform>();
				curRect.sizeDelta = new Vector2(colRect.rect.size.x, curRect.rect.size.y);
			}
			item.Refresh(index, list, new Action<Action<IList<AdventureParameterSnapshot>>>(this.<RefreshItem>g__ProcessEdit|4_0), new Action<IList<AdventureParameterSnapshot>>(this.Refresh));
		}

		// Token: 0x06004F46 RID: 20294 RVA: 0x002516F0 File Offset: 0x0024F8F0
		protected override void RefreshItem(IList<AdventureParameterSnapshot> list, AdventureGlobalVariableEditorItem editorItem, int index, bool setDisableStyle)
		{
			this.RefreshItem(list, editorItem, index);
		}

		// Token: 0x06004F47 RID: 20295 RVA: 0x002516FC File Offset: 0x0024F8FC
		void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
		{
			bool flag = editType.Contains(EAdventureEditType.Parameters);
			if (flag)
			{
				this.Refresh(AdventureEditorKit.BlackBoard.Editing.Parameters);
			}
		}

		// Token: 0x06004F48 RID: 20296 RVA: 0x00251730 File Offset: 0x0024F930
		internal static AdventureParameterSnapshot GenerateNewItem(IList<AdventureParameterSnapshot> list)
		{
			int newKeyIndex = 0;
			bool flag;
			do
			{
				newKeyIndex++;
				string newKey = string.Format("new_var_{0}", newKeyIndex);
				flag = list.Any((AdventureParameterSnapshot u) => u.Key == newKey);
			}
			while (flag);
			return new AdventureParameterSnapshot
			{
				Key = string.Format("new_var_{0}", newKeyIndex)
			};
		}

		// Token: 0x06004F49 RID: 20297 RVA: 0x002517A4 File Offset: 0x0024F9A4
		private void ResetViewData(AdventureSnapshot snapshot)
		{
			bool hasNear = false;
			bool hasFar = false;
			foreach (AdventureParameterSnapshot parameter in snapshot.Parameters)
			{
				bool flag = parameter.Key.Equals("view_range_0");
				if (flag)
				{
					parameter.InitialValue = 1;
					parameter.Type = EAdventureParameterType.Influence;
					hasNear = true;
				}
				bool flag2 = parameter.Key.Equals("view_range_1");
				if (flag2)
				{
					parameter.InitialValue = 3;
					parameter.Type = EAdventureParameterType.Influence;
					hasFar = true;
				}
			}
			bool flag3 = !hasNear;
			if (flag3)
			{
				snapshot.Parameters.Add(AdventureSnapshotConstants.ViewTypeDefaultNear);
			}
			bool flag4 = !hasFar;
			if (flag4)
			{
				snapshot.Parameters.Add(AdventureSnapshotConstants.ViewTypeDefaultFar);
			}
		}

		// Token: 0x06004F4D RID: 20301 RVA: 0x002518B4 File Offset: 0x0024FAB4
		[CompilerGenerated]
		private void <RefreshItem>g__ProcessEdit|4_0(Action<IList<AdventureParameterSnapshot>> proc)
		{
			bool refreshEditing = this._refreshEditing;
			if (!refreshEditing)
			{
				this._refreshEditing = true;
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					proc(snapshot.Parameters);
				}, EAdventureEditType.Parameters);
				this._refreshEditing = false;
			}
		}

		// Token: 0x0400367E RID: 13950
		private bool _refreshEditing;

		// Token: 0x0400367F RID: 13951
		public CButton resetViewButton;
	}
}
