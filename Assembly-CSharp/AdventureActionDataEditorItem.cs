using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class AdventureActionDataEditorItem : MonoBehaviour
{
	// Token: 0x060016D6 RID: 5846 RVA: 0x0008BB1C File Offset: 0x00089D1C
	internal void Refresh(int index, IList<AdventureActionSnapshot> currentList, Action<Action<IList<AdventureActionSnapshot>>> editContext, Action<IList<AdventureActionSnapshot>> onRefresh)
	{
		this.buttonRemove.onClick.RemoveAllListeners();
		Action<IList<AdventureActionSnapshot>> <>9__9;
		this.buttonRemove.onClick.AddListener(delegate()
		{
			Action<Action<IList<AdventureActionSnapshot>>> editContext2 = editContext;
			Action<IList<AdventureActionSnapshot>> obj;
			if ((obj = <>9__9) == null)
			{
				obj = (<>9__9 = delegate(IList<AdventureActionSnapshot> editList)
				{
					editList.RemoveAt(index);
					onRefresh(editList);
				});
			}
			editContext2(obj);
		});
		this.inputFieldKey.onValidateInput = ((string v, int _, char addChar) => currentList.Any((AdventureActionSnapshot u) => u != currentList[index] && u.Key == v + addChar.ToString()) ? '\0' : addChar);
		this.inputFieldKey.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureActionSnapshot> editList)
			{
				editList[index].Key = v;
			});
		});
		this.inputFieldKey.SetTextWithoutNotify(currentList[index].Key);
		this.inputFieldTime.onValidateInput = new TMP_InputField.OnValidateInput(AdventureActionDataEditorItem.<Refresh>g__OnValidateInputFun|8_8);
		this.inputFieldTime.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureActionSnapshot> editList)
			{
				int r;
				editList[index].Time = (int.TryParse(v, out r) ? r : editList[index].Time);
			});
		});
		this.inputFieldTime.SetTextWithoutNotify(currentList[index].Time.ToString());
		this.inputFieldName.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureActionSnapshot> editList)
			{
				editList[index].Name = v;
			});
		});
		this.inputFieldName.SetTextWithoutNotify(currentList[index].Name);
		this.inputFieldDesc.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureActionSnapshot> editList)
			{
				editList[index].Desc = v;
			});
		});
		this.inputFieldDesc.SetTextWithoutNotify(currentList[index].Desc);
		this.inputFieldIcon.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureActionSnapshot> editList)
			{
				editList[index].Icon = v;
				this.imageIcon.SetSprite(v, false, null);
			});
		});
		this.inputFieldIcon.SetTextWithoutNotify(currentList[index].Icon);
		this.imageIcon.SetSprite(currentList[index].Icon, false, null);
		this.inputFieldComment.onValueChanged.ResetListener(delegate(string v)
		{
			editContext(delegate(IList<AdventureActionSnapshot> editList)
			{
				editList[index].Comment = v;
			});
		});
		this.inputFieldComment.SetTextWithoutNotify(currentList[index].Comment);
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x0008BD6C File Offset: 0x00089F6C
	[CompilerGenerated]
	internal static char <Refresh>g__OnValidateInputFun|8_8(string text, int _, char addChar)
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

	// Token: 0x0400126E RID: 4718
	[SerializeField]
	private CButton buttonRemove;

	// Token: 0x0400126F RID: 4719
	[SerializeField]
	private TMP_InputField inputFieldKey;

	// Token: 0x04001270 RID: 4720
	[SerializeField]
	private TMP_InputField inputFieldTime;

	// Token: 0x04001271 RID: 4721
	[SerializeField]
	private TMP_InputField inputFieldName;

	// Token: 0x04001272 RID: 4722
	[SerializeField]
	private TMP_InputField inputFieldDesc;

	// Token: 0x04001273 RID: 4723
	[SerializeField]
	private TMP_InputField inputFieldIcon;

	// Token: 0x04001274 RID: 4724
	[SerializeField]
	private CImage imageIcon;

	// Token: 0x04001275 RID: 4725
	[SerializeField]
	private TMP_InputField inputFieldComment;
}
