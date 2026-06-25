using System;
using System.Collections.Generic;
using System.Linq;
using EventEditor;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

namespace SubEditor
{
	// Token: 0x02000695 RID: 1685
	public class AdventureAutoEventEditorItem : MonoBehaviour
	{
		// Token: 0x06004F26 RID: 20262 RVA: 0x00250A6C File Offset: 0x0024EC6C
		internal void Refresh(IList<AdventureAutoEventSnapshot> currentList, int index, EventEditorScript editorScript, Action<IList<AdventureAutoEventSnapshot>> onRefresh)
		{
			this._buttonRemove.onClick.RemoveAllListeners();
			AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction <>9__3;
			this._buttonRemove.onClick.AddListener(delegate()
			{
				AdventureBlackBoard<AdventureSnapshot, EAdventureEditType> blackBoard = AdventureEditorKit.BlackBoard;
				AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction editAction;
				if ((editAction = <>9__3) == null)
				{
					editAction = (<>9__3 = delegate(AdventureSnapshot snapshot)
					{
						snapshot.AutoEvents.RemoveAt(index);
						onRefresh(snapshot.AutoEvents);
					});
				}
				blackBoard.MakeEdit(editAction, EAdventureEditType.AutoEvents);
			});
			AdventureAutoEventEditorItem.EventEditorRefresh(currentList, index, onRefresh, this._toggleOnlyOnce, this._buttonMoveUp, this._buttonMoveDown, this._inputFieldGuid, this._inputFieldComment);
			this._dropdownTriggerType.ClearOptions();
			this._dropdownTriggerType.AddOptions((from n in Enum.GetNames(typeof(EAdventureAutoEventTriggerType))
			select LocalStringManager.Get("LK_AdventureEditor_AdventureAutoEventTriggerType_" + n)).ToList<string>());
			this._dropdownTriggerType.onValueChanged.ResetListener(delegate(int v)
			{
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					snapshot.AutoEvents[index].TriggerType = (EAdventureAutoEventTriggerType)v;
				}, EAdventureEditType.AutoEvents);
			});
			this._dropdownTriggerType.SetValueWithoutNotify((int)currentList[index].TriggerType);
		}

		// Token: 0x06004F27 RID: 20263 RVA: 0x00250B78 File Offset: 0x0024ED78
		private static void EventEditorRefresh(IList<AdventureAutoEventSnapshot> currentList, int index, Action<IList<AdventureAutoEventSnapshot>> onRefresh, CToggle toggleOnlyOnce, CButton buttonMoveUp, CButton buttonMoveDown, TMP_InputField inputFieldGuid, TMP_InputField inputFieldComment)
		{
			AdventureAbstractListEditor<AdventureAutoEventSnapshot, Refers>.ItemAddMoveSwap(() => currentList.Count, delegate(int swapIdx, int swapIdx2)
			{
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					List<AdventureAutoEventSnapshot> autoEvents = snapshot.AutoEvents;
					int swapIdx = swapIdx;
					List<AdventureAutoEventSnapshot> autoEvents2 = snapshot.AutoEvents;
					int swapIdx2 = swapIdx2;
					AdventureAutoEventSnapshot value = snapshot.AutoEvents[swapIdx2];
					AdventureAutoEventSnapshot value2 = snapshot.AutoEvents[swapIdx];
					autoEvents[swapIdx] = value;
					autoEvents2[swapIdx2] = value2;
					onRefresh(snapshot.AutoEvents);
				}, EAdventureEditType.AutoEvents);
			}, index, buttonMoveUp, buttonMoveDown, delegate()
			{
			}, null);
			toggleOnlyOnce.onValueChanged.ResetListener(delegate(bool v)
			{
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					AdventureAutoEventSnapshot adventureAutoEventSnapshot = snapshot.AutoEvents[index];
					AdventureEventSnapshot adventureEventSnapshot;
					if ((adventureEventSnapshot = adventureAutoEventSnapshot.Event) == null)
					{
						adventureEventSnapshot = (adventureAutoEventSnapshot.Event = new AdventureEventSnapshot());
					}
					adventureEventSnapshot.OnlyOnce = v;
				}, EAdventureEditType.AutoEvents);
			});
			AdventureEventSnapshot @event = currentList[index].Event;
			toggleOnlyOnce.SetIsOnWithoutNotify(@event != null && @event.OnlyOnce);
			inputFieldGuid.onEndEdit.ResetListener(delegate(string v)
			{
				v = v.Trim('"');
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					AdventureAutoEventSnapshot adventureAutoEventSnapshot = snapshot.AutoEvents[index];
					AdventureEventSnapshot adventureEventSnapshot;
					if ((adventureEventSnapshot = adventureAutoEventSnapshot.Event) == null)
					{
						adventureEventSnapshot = (adventureAutoEventSnapshot.Event = new AdventureEventSnapshot());
					}
					adventureEventSnapshot.Guid = v;
				}, EAdventureEditType.AutoEvents);
				inputFieldGuid.SetTextWithoutNotify(v);
			});
			TMP_InputField inputFieldGuid2 = inputFieldGuid;
			AdventureEventSnapshot event2 = currentList[index].Event;
			inputFieldGuid2.SetTextWithoutNotify(((event2 != null) ? event2.Guid : null) ?? string.Empty);
			inputFieldComment.onValueChanged.ResetListener(delegate(string v)
			{
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					AdventureAutoEventSnapshot adventureAutoEventSnapshot = snapshot.AutoEvents[index];
					AdventureEventSnapshot adventureEventSnapshot;
					if ((adventureEventSnapshot = adventureAutoEventSnapshot.Event) == null)
					{
						adventureEventSnapshot = (adventureAutoEventSnapshot.Event = new AdventureEventSnapshot());
					}
					adventureEventSnapshot.Comment = v;
				}, EAdventureEditType.AutoEvents);
			});
			AdventureEventSnapshot event3 = currentList[index].Event;
			inputFieldComment.SetTextWithoutNotify(((event3 != null) ? event3.Comment : null) ?? string.Empty);
		}

		// Token: 0x06004F28 RID: 20264 RVA: 0x00250CD4 File Offset: 0x0024EED4
		private static void EventEditorRefresh<T>(IList<T> list, int index, Action onUpdate, AdventureEventSnapshot data, CToggle toggleOnlyOnce, CButton buttonMoveUp, CButton buttonMoveDown, TMP_InputField inputFieldGuid, TMP_InputField inputFieldComment)
		{
			AdventureAbstractListEditor<T, Refers>.ItemAddMoveSwap(list, index, buttonMoveUp, buttonMoveDown, onUpdate, null);
			toggleOnlyOnce.onValueChanged.ResetListener(delegate(bool v)
			{
				data.OnlyOnce = v;
			});
			toggleOnlyOnce.isOn = data.OnlyOnce;
			inputFieldGuid.onEndEdit.ResetListener(delegate(string v)
			{
				v = v.Trim('"');
				data.Guid = v;
				inputFieldGuid.SetTextWithoutNotify(v);
			});
			inputFieldGuid.SetTextWithoutNotify(data.Guid);
			inputFieldComment.onValueChanged.ResetListener(delegate(string v)
			{
				data.Comment = v;
			});
			inputFieldComment.text = data.Comment;
		}

		// Token: 0x06004F29 RID: 20265 RVA: 0x00250D94 File Offset: 0x0024EF94
		internal static void EventEditorRefresh<T>(IList<T> list, int index, Action onUpdate, AdventureElementEventSnapshot data, CToggle toggleOnlyOnce, CButton buttonMoveUp, CButton buttonMoveDown, TMP_InputField inputFieldGuid, TMP_InputField inputFieldComment, CDropdown dropdownTriggerType)
		{
			AdventureElementEventSnapshot data2 = data;
			AdventureEventSnapshot data3;
			if ((data3 = data2.Event) == null)
			{
				data3 = (data2.Event = new AdventureEventSnapshot());
			}
			AdventureAutoEventEditorItem.EventEditorRefresh<T>(list, index, onUpdate, data3, toggleOnlyOnce, buttonMoveUp, buttonMoveDown, inputFieldGuid, inputFieldComment);
			dropdownTriggerType.ClearOptions();
			dropdownTriggerType.AddOptions((from n in Enum.GetNames(typeof(EAdventureElementEventTriggerType))
			select LocalStringManager.Get("LK_AdventureEditor_AdventureElementEventTriggerType_" + n)).ToList<string>());
			dropdownTriggerType.onValueChanged.ResetListener(delegate(int v)
			{
				data.TriggerType = (EAdventureElementEventTriggerType)v;
			});
			dropdownTriggerType.value = (int)data.TriggerType;
		}

		// Token: 0x0400366A RID: 13930
		[SerializeField]
		private CButton _buttonRemove;

		// Token: 0x0400366B RID: 13931
		[SerializeField]
		private CButton _buttonMoveUp;

		// Token: 0x0400366C RID: 13932
		[SerializeField]
		private CButton _buttonMoveDown;

		// Token: 0x0400366D RID: 13933
		[SerializeField]
		private CToggle _toggleOnlyOnce;

		// Token: 0x0400366E RID: 13934
		[SerializeField]
		private CDropdown _dropdownTriggerType;

		// Token: 0x0400366F RID: 13935
		[SerializeField]
		private TMP_InputField _inputFieldGuid;

		// Token: 0x04003670 RID: 13936
		[SerializeField]
		private TMP_InputField _inputFieldComment;
	}
}
