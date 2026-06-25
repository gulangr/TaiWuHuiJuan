using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class AdventureCustomTextsEditorItem : MonoBehaviour
{
	// Token: 0x060016E4 RID: 5860 RVA: 0x0008BFA8 File Offset: 0x0008A1A8
	internal void Refresh(int index, IList<AdventureTextSnapshot> currentList, Action<Action<IList<AdventureTextSnapshot>>> editContext, Action<IList<AdventureTextSnapshot>> onRefresh)
	{
		this.buttonRemove.onClick.RemoveAllListeners();
		Action<IList<AdventureTextSnapshot>> <>9__8;
		this.buttonRemove.onClick.AddListener(delegate()
		{
			Action<Action<IList<AdventureTextSnapshot>>> editContext2 = editContext;
			Action<IList<AdventureTextSnapshot>> obj;
			if ((obj = <>9__8) == null)
			{
				obj = (<>9__8 = delegate(IList<AdventureTextSnapshot> editList)
				{
					editList.RemoveAt(index);
					onRefresh(editList);
				});
			}
			editContext2(obj);
		});
		this.toggleOnlyOnce.SetIsOnWithoutNotify(currentList[index].OnlyOnce);
		this.toggleOnlyOnce.onValueChanged.ResetListener(delegate(bool isOn)
		{
			editContext(delegate(IList<AdventureTextSnapshot> editList)
			{
				editList[index].OnlyOnce = isOn;
			});
		});
		this.inputFieldKey.onValidateInput = ((string v, int _, char addChar) => currentList.Any((AdventureTextSnapshot u) => u != currentList[index] && u.Key == v + addChar.ToString()) ? '\0' : addChar);
		this.inputFieldKey.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureTextSnapshot> editList)
			{
				editList[index].Key = v;
			});
		});
		this.inputFieldKey.SetTextWithoutNotify(currentList[index].Key);
		this.inputFieldText.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureTextSnapshot> editList)
			{
				editList[index].Text = v;
			});
		});
		this.inputFieldText.SetTextWithoutNotify(currentList[index].Text);
		this.inputFieldPriority.onValidateInput = new TMP_InputField.OnValidateInput(AdventureCustomTextsEditorItem.<Refresh>g__OnValidateInputFun|6_7);
		this.inputFieldPriority.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureTextSnapshot> editList)
			{
				int r;
				editList[index].Priority = (int.TryParse(v, out r) ? r : editList[index].Priority);
			});
		});
		this.inputFieldPriority.SetTextWithoutNotify(currentList[index].Priority.ToString());
		this.inputFieldComment.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureTextSnapshot> editList)
			{
				editList[index].Comment = v;
			});
		});
		this.inputFieldComment.SetTextWithoutNotify(currentList[index].Comment);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x0008C188 File Offset: 0x0008A388
	[CompilerGenerated]
	internal static char <Refresh>g__OnValidateInputFun|6_7(string text, int _, char addChar)
	{
		bool flag = addChar.Equals('-');
		char result;
		if (flag)
		{
			result = '-';
		}
		else
		{
			int r;
			result = (int.TryParse(text + addChar.ToString(), out r) ? addChar : '\0');
		}
		return result;
	}

	// Token: 0x04001276 RID: 4726
	[SerializeField]
	private CButton buttonRemove;

	// Token: 0x04001277 RID: 4727
	[SerializeField]
	private CToggle toggleOnlyOnce;

	// Token: 0x04001278 RID: 4728
	[SerializeField]
	private TMP_InputField inputFieldKey;

	// Token: 0x04001279 RID: 4729
	[SerializeField]
	private TMP_InputField inputFieldText;

	// Token: 0x0400127A RID: 4730
	[SerializeField]
	private TMP_InputField inputFieldPriority;

	// Token: 0x0400127B RID: 4731
	[SerializeField]
	private TMP_InputField inputFieldComment;
}
