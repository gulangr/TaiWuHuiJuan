using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using Game.Views.Adventure;
using GameData.Adventure;
using GameData.Adventure.Editor;
using Property;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SubEditor
{
	// Token: 0x02000699 RID: 1689
	public class AdventureGlobalVariableEditorItem : MonoBehaviour
	{
		// Token: 0x06004F4E RID: 20302 RVA: 0x00251904 File Offset: 0x0024FB04
		internal void Refresh(int index, IList<AdventureParameterSnapshot> currentList, Action<Action<IList<AdventureParameterSnapshot>>> editContext, Action<IList<AdventureParameterSnapshot>> onRefresh)
		{
			AdventureGlobalVariableEditorItem.<>c__DisplayClass16_0 CS$<>8__locals1 = new AdventureGlobalVariableEditorItem.<>c__DisplayClass16_0();
			CS$<>8__locals1.editContext = editContext;
			CS$<>8__locals1.index = index;
			CS$<>8__locals1.onRefresh = onRefresh;
			CS$<>8__locals1.currentList = currentList;
			CS$<>8__locals1.<>4__this = this;
			this.labelIndex.text = CS$<>8__locals1.index.ToString();
			this.buttonRemove.onClick.RemoveAllListeners();
			this.buttonRemove.onClick.AddListener(delegate()
			{
				Action<Action<IList<AdventureParameterSnapshot>>> editContext2 = CS$<>8__locals1.editContext;
				Action<IList<AdventureParameterSnapshot>> obj;
				if ((obj = CS$<>8__locals1.<>9__15) == null)
				{
					obj = (CS$<>8__locals1.<>9__15 = delegate(IList<AdventureParameterSnapshot> editList)
					{
						editList.RemoveAt(CS$<>8__locals1.index);
						CS$<>8__locals1.onRefresh(editList);
					});
				}
				editContext2(obj);
			});
			this.inputFieldKey.onValidateInput = ((string v, int _, char addChar) => CS$<>8__locals1.currentList.Any((AdventureParameterSnapshot u) => u != CS$<>8__locals1.currentList[CS$<>8__locals1.index] && u.Key == v + addChar.ToString()) ? '\0' : addChar);
			this.inputFieldKey.onValueChanged.ResetListener(delegate(string v)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Key = v;
				});
			});
			this.inputFieldKey.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].Key);
			this.inputFieldName.onValueChanged.ResetListener(delegate(string v)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Name = v;
				});
			});
			this.inputFieldName.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].Name);
			this.inputFieldDesc.onValueChanged.ResetListener(delegate(string v)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Desc = v;
				});
			});
			this.inputFieldDesc.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].Desc);
			this.inputFieldIcon.onValueChanged.ResetListener(delegate(string v)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Icon = v;
					CS$<>8__locals1.<>4__this.imageIcon.SetSprite(v, false, null);
				});
			});
			this.inputFieldIcon.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].Icon);
			string iconName = (CS$<>8__locals1.currentList[CS$<>8__locals1.index].Type == EAdventureParameterType.State) ? ViewAdventureRemake.GetElementParamStateIconName(CS$<>8__locals1.currentList[CS$<>8__locals1.index].Icon, false) : CS$<>8__locals1.currentList[CS$<>8__locals1.index].Icon;
			this.imageIcon.SetSprite(iconName, false, null);
			this.typeDropdown.SetupEditor("LK_AdventureEditor_AdventureParameterType_{0}", delegate(EAdventureParameterType type)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Type = type;
				});
			}, (EAdventureParameterType type) => type == CS$<>8__locals1.currentList[CS$<>8__locals1.index].Type, false);
			this.typeDropdown.onValueChanged.AddListener(new UnityAction<int>(CS$<>8__locals1.<Refresh>g__SetStyleDropdownOption|8));
			CS$<>8__locals1.<Refresh>g__SetStyleDropdownOption|8((int)CS$<>8__locals1.currentList[CS$<>8__locals1.index].Type);
			this.styleDropdown.onValueChanged.ResetListener(delegate(int value)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Style = value;
				});
			});
			this.inputFieldInitialValue.onValidateInput = new TMP_InputField.OnValidateInput(AdventureGlobalVariableEditorItem.<Refresh>g__OnValidateInputFun|16_14);
			this.inputFieldInitialValue.onValueChanged.ResetListener(delegate(string v)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					int r;
					editList[CS$<>8__locals1.index].InitialValue = (int.TryParse(v, out r) ? r : editList[CS$<>8__locals1.index].InitialValue);
				});
			});
			this.inputFieldInitialValue.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].InitialValue.ToString());
			this.inputFieldComment.onValueChanged.ResetListener(delegate(string v)
			{
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].Comment = v;
				});
			});
			this.inputFieldComment.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].Comment);
			this.inputFieldRangeBlockColor.onValueChanged.ResetListener(delegate(string v)
			{
				bool flag = !v.StartsWith("#");
				if (flag)
				{
					v = "#" + v;
				}
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].InfluenceBlockColorHex = v;
				});
			});
			this.inputFieldRangeBlockColor.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].InfluenceBlockColorHex);
			this.inputFieldRangeEdgeLineColor.onValueChanged.ResetListener(delegate(string v)
			{
				bool flag = !v.StartsWith("#");
				if (flag)
				{
					v = "#" + v;
				}
				CS$<>8__locals1.editContext(delegate(IList<AdventureParameterSnapshot> editList)
				{
					editList[CS$<>8__locals1.index].InfluenceEdgeColorHex = v;
				});
			});
			this.inputFieldRangeEdgeLineColor.SetTextWithoutNotify(CS$<>8__locals1.currentList[CS$<>8__locals1.index].InfluenceEdgeColorHex);
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x00251D5C File Offset: 0x0024FF5C
		[CompilerGenerated]
		internal static char <Refresh>g__OnValidateInputFun|16_14(string text, int _, char addChar)
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
				bool flag2 = int.TryParse(text + addChar.ToString(), out r);
				if (flag2)
				{
					result = addChar;
				}
				else
				{
					result = '\0';
				}
			}
			return result;
		}

		// Token: 0x04003680 RID: 13952
		[FormerlySerializedAs("_labelIndex")]
		[SerializeField]
		private TextMeshProUGUI labelIndex;

		// Token: 0x04003681 RID: 13953
		[FormerlySerializedAs("_buttonRemove")]
		[SerializeField]
		private CButton buttonRemove;

		// Token: 0x04003682 RID: 13954
		[FormerlySerializedAs("_inputFieldKey")]
		[SerializeField]
		private TMP_InputField inputFieldKey;

		// Token: 0x04003683 RID: 13955
		[FormerlySerializedAs("_inputFieldName")]
		[SerializeField]
		private TMP_InputField inputFieldName;

		// Token: 0x04003684 RID: 13956
		[FormerlySerializedAs("_inputFieldDesc")]
		[SerializeField]
		private TMP_InputField inputFieldDesc;

		// Token: 0x04003685 RID: 13957
		[FormerlySerializedAs("_inputFieldIcon")]
		[SerializeField]
		private TMP_InputField inputFieldIcon;

		// Token: 0x04003686 RID: 13958
		[FormerlySerializedAs("_imageIcon")]
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04003687 RID: 13959
		[SerializeField]
		private CDropdown typeDropdown;

		// Token: 0x04003688 RID: 13960
		[SerializeField]
		private CDropdown styleDropdown;

		// Token: 0x04003689 RID: 13961
		[SerializeField]
		private TMP_InputField inputFieldInitialValue;

		// Token: 0x0400368A RID: 13962
		[SerializeField]
		private TMP_InputField inputFieldTotalProgress;

		// Token: 0x0400368B RID: 13963
		[SerializeField]
		private TMP_InputField inputFieldRangeBlockColor;

		// Token: 0x0400368C RID: 13964
		[SerializeField]
		private TMP_InputField inputFieldRangeEdgeLineColor;

		// Token: 0x0400368D RID: 13965
		[FormerlySerializedAs("_inputFieldComment")]
		[SerializeField]
		private TMP_InputField inputFieldComment;

		// Token: 0x0400368E RID: 13966
		[SerializeField]
		private bool elementOrGlobal;

		// Token: 0x0400368F RID: 13967
		[TupleElementNames(new string[]
		{
			"style",
			"key"
		})]
		private Dictionary<EAdventureParameterType, List<ValueTuple<int, LanguageKey>>> _typeContainsStyle = new Dictionary<EAdventureParameterType, List<ValueTuple<int, LanguageKey>>>
		{
			{
				EAdventureParameterType.Normal,
				new List<ValueTuple<int, LanguageKey>>
				{
					new ValueTuple<int, LanguageKey>(0, LanguageKey.LK_Adventure_Editor_Style0),
					new ValueTuple<int, LanguageKey>(1, LanguageKey.LK_Adventure_Editor_Style1)
				}
			},
			{
				EAdventureParameterType.State,
				new List<ValueTuple<int, LanguageKey>>
				{
					new ValueTuple<int, LanguageKey>(0, LanguageKey.LK_Adventure_Editor_Style0),
					new ValueTuple<int, LanguageKey>(1, LanguageKey.LK_Adventure_Editor_Style1),
					new ValueTuple<int, LanguageKey>(2, LanguageKey.LK_Adventure_Editor_Style2)
				}
			},
			{
				EAdventureParameterType.Influence,
				new List<ValueTuple<int, LanguageKey>>
				{
					new ValueTuple<int, LanguageKey>(0, LanguageKey.LK_Adventure_Editor_Style3),
					new ValueTuple<int, LanguageKey>(1, LanguageKey.LK_Adventure_Editor_Style4)
				}
			}
		};
	}
}
