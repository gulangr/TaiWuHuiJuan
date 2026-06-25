using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SubEditor
{
	// Token: 0x02000693 RID: 1683
	public abstract class AdventureAbstractListEditor<T, TEditorItem> : TemplatedContainerAssemblyNew where TEditorItem : MonoBehaviour
	{
		// Token: 0x06004F0B RID: 20235 RVA: 0x002505BA File Offset: 0x0024E7BA
		protected virtual bool CheckEmpty()
		{
			return this.List == null;
		}

		// Token: 0x06004F0C RID: 20236 RVA: 0x002505C5 File Offset: 0x0024E7C5
		protected virtual void Start()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				bool flag = this.CheckEmpty();
				if (flag)
				{
					base.gameObject.SetActive(false);
				}
			});
		}

		// Token: 0x06004F0D RID: 20237 RVA: 0x002505E0 File Offset: 0x0024E7E0
		protected virtual void OnEnable()
		{
			this.Refresh(this.List);
		}

		// Token: 0x06004F0E RID: 20238 RVA: 0x002505F0 File Offset: 0x0024E7F0
		protected virtual void Refresh(IList<T> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				this.buttonAdd.onClick.ResetListener(delegate()
				{
					this.AddItem(list);
				});
				base.Rebuild<TEditorItem>(list.Count, delegate(TEditorItem unit, int index)
				{
					this.RefreshItem(list, unit, index);
				});
			}
		}

		// Token: 0x06004F0F RID: 20239 RVA: 0x0025065C File Offset: 0x0024E85C
		protected virtual void AddItem(IList<T> list)
		{
			list.Add(this.ItemCreator(list));
			this.Refresh(list);
		}

		// Token: 0x06004F10 RID: 20240
		protected abstract T ItemCreator(IList<T> list);

		// Token: 0x06004F11 RID: 20241
		protected abstract void RefreshItem(IList<T> list, TEditorItem editorItem, int index);

		// Token: 0x06004F12 RID: 20242
		protected abstract void RefreshItem(IList<T> list, TEditorItem editorItem, int index, bool setDisableStyle);

		// Token: 0x06004F13 RID: 20243 RVA: 0x00250678 File Offset: 0x0024E878
		internal static void FixByColumn(RectTransform columnsHeader, RectTransform item)
		{
			for (int i = 0; i < columnsHeader.childCount; i++)
			{
				RectTransform colRect = columnsHeader.GetChild(i).GetComponent<RectTransform>();
				RectTransform curRect = item.transform.GetChild(i).GetComponent<RectTransform>();
				curRect.sizeDelta = new Vector2(colRect.rect.size.x, curRect.rect.size.y);
			}
		}

		// Token: 0x06004F14 RID: 20244 RVA: 0x002506F0 File Offset: 0x0024E8F0
		internal static void ItemAddMoveSwap(IList<T> subList, AdventureAbstractListEditorTemplate subUnit, int subIndex, Action subOnUpdate, Action extraOnUpdate = null)
		{
			AdventureAbstractListEditor<T, TEditorItem>.ItemAddMoveSwap(subList, subIndex, subUnit.moveUp, subUnit.moveDown, subOnUpdate, extraOnUpdate);
		}

		// Token: 0x06004F15 RID: 20245 RVA: 0x0025070C File Offset: 0x0024E90C
		internal static void ItemAddMoveSwap(IList<T> subList, int subIndex, CButton buttonMoveUp, CButton buttonMoveDown, Action subOnUpdate, Action extraOnUpdate = null)
		{
			AdventureAbstractListEditor<T, TEditorItem>.ItemAddMoveSwap(() => subList.Count, delegate(int swapIdx, int swapIdx2)
			{
				IList<T> subList2 = subList;
				IList<T> subList3 = subList;
				T value = subList[swapIdx2];
				T value2 = subList[swapIdx];
				subList2[swapIdx] = value;
				subList3[swapIdx2] = value2;
			}, subIndex, buttonMoveUp, buttonMoveDown, subOnUpdate, extraOnUpdate);
		}

		// Token: 0x06004F16 RID: 20246 RVA: 0x0025074C File Offset: 0x0024E94C
		internal static void ItemAddMoveSwap(Func<int> subListCountProvider, Action<int, int> subListSwapAction, int subIndex, CButton buttonMoveUp, CButton buttonMoveDown, Action subOnUpdate, Action extraOnUpdate = null)
		{
			buttonMoveUp.interactable = (subIndex > 0);
			buttonMoveUp.onClick.ResetListener(delegate()
			{
				int swapIdx = subIndex - 1;
				subListSwapAction(swapIdx, subIndex);
				subOnUpdate();
				Action extraOnUpdate2 = extraOnUpdate;
				if (extraOnUpdate2 != null)
				{
					extraOnUpdate2();
				}
			});
			buttonMoveDown.interactable = (subIndex < subListCountProvider() - 1);
			buttonMoveDown.onClick.ResetListener(delegate()
			{
				int swapIdx = subIndex + 1;
				subListSwapAction(swapIdx, subIndex);
				subOnUpdate();
				Action extraOnUpdate2 = extraOnUpdate;
				if (extraOnUpdate2 != null)
				{
					extraOnUpdate2();
				}
			});
		}

		// Token: 0x06004F17 RID: 20247 RVA: 0x002507D7 File Offset: 0x0024E9D7
		internal static void ItemAddRemove(IList<T> subList, AdventureAbstractListEditorTemplate subUnit, int subIndex, Action subOnUpdate, Action extraOnUpdate = null)
		{
			AdventureAbstractListEditor<T, TEditorItem>.ItemAddRemove(new Action<int>(subList.RemoveAt), subUnit, subIndex, subOnUpdate, extraOnUpdate);
		}

		// Token: 0x06004F18 RID: 20248 RVA: 0x002507F4 File Offset: 0x0024E9F4
		internal static void ItemAddRemove(Action<int> subListRemoveAction, AdventureAbstractListEditorTemplate subUnit, int subIndex, Action subOnUpdate, Action extraOnUpdate = null)
		{
			subUnit.remove.onClick.ResetListener(delegate()
			{
				subListRemoveAction(subIndex);
				subOnUpdate();
				Action extraOnUpdate2 = extraOnUpdate;
				if (extraOnUpdate2 != null)
				{
					extraOnUpdate2();
				}
			});
		}

		// Token: 0x04003663 RID: 13923
		[FormerlySerializedAs("_labelTitle")]
		[SerializeField]
		protected TextMeshProUGUI labelTitle;

		// Token: 0x04003664 RID: 13924
		[FormerlySerializedAs("_columnsHeader")]
		[SerializeField]
		protected RectTransform columnsHeader;

		// Token: 0x04003665 RID: 13925
		[FormerlySerializedAs("_buttonAdd")]
		[SerializeField]
		protected CButton buttonAdd;

		// Token: 0x04003666 RID: 13926
		protected List<T> List;
	}
}
