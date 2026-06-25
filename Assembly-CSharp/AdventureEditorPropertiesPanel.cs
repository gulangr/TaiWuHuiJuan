using System;
using System.Collections.Generic;
using System.Linq;
using EventEditor;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class AdventureEditorPropertiesPanel : MonoBehaviour
{
	// Token: 0x060015DA RID: 5594 RVA: 0x00087078 File Offset: 0x00085278
	private void Awake()
	{
		bool awaken = this._awaken;
		if (!awaken)
		{
			EventEditorScript.Init(this.eventEditorScript);
			this._awaken = true;
		}
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x000870A5 File Offset: 0x000852A5
	protected void OnEnable()
	{
		this.Awake();
		this.decoratesEditorPanel.gameObject.SetActive(false);
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x000870C1 File Offset: 0x000852C1
	private void OnDisable()
	{
		AdventureEditorKit.RestoreLayerSortingOrder(base.gameObject);
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x000870D0 File Offset: 0x000852D0
	public void Refresh(List<AdventureBlockIndex> blocks)
	{
		this.Awake();
		this._blocks = blocks;
		this.RefreshFields(blocks);
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x000870EC File Offset: 0x000852EC
	private void RefreshFields(List<AdventureBlockIndex> indexes)
	{
		AdventureEditorPropertiesPanel.<>c__DisplayClass20_0 CS$<>8__locals1 = new AdventureEditorPropertiesPanel.<>c__DisplayClass20_0();
		CS$<>8__locals1.indexes = indexes;
		CS$<>8__locals1.<>4__this = this;
		IReadOnlyList<AdventureBlockSnapshot> currentBlocks = AdventureEditorKit.BlackBoard.CurrentGroupBlocks;
		CS$<>8__locals1.blockFirst = currentBlocks.First((AdventureBlockSnapshot x) => x.Index == CS$<>8__locals1.indexes[0]);
		List<AdventureBlockSnapshot> multiSelectBlocks = (from b in currentBlocks
		where CS$<>8__locals1.indexes.Contains(b.Index)
		select b).ToList<AdventureBlockSnapshot>();
		this.inputFieldOffsetHeight.onValidateInput = delegate(string text, int charIndex, char addedChar)
		{
			bool flag = char.IsDigit(addedChar) || addedChar == '-' || addedChar == '.';
			char result;
			if (flag)
			{
				bool flag2 = addedChar == '.' && text.Contains(".");
				if (flag2)
				{
					result = '\0';
				}
				else
				{
					bool flag3 = addedChar == '-' && (text.Length > 0 || charIndex != 0);
					if (flag3)
					{
						result = '\0';
					}
					else
					{
						result = addedChar;
					}
				}
			}
			else
			{
				result = '\0';
			}
			return result;
		};
		this.inputFieldOffsetHeight.onValueChanged.ResetListener(delegate(string v)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				List<AdventureBlockSnapshot> blocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
				IEnumerable<AdventureBlockSnapshot> source = blocks;
				Func<AdventureBlockSnapshot, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__19) == null)
				{
					predicate = (CS$<>8__locals1.<>9__19 = ((AdventureBlockSnapshot x) => CS$<>8__locals1.indexes.Contains(x.Index)));
				}
				foreach (AdventureBlockSnapshot block in source.Where(predicate))
				{
					float newValue;
					block.Height = (float.TryParse(v, out newValue) ? newValue : ((float)block.TimeCost));
				}
			}, EAdventureEditType.BlockProperties);
		});
		bool heightDiff = multiSelectBlocks.Any((AdventureBlockSnapshot b) => Math.Abs(b.Height - CS$<>8__locals1.blockFirst.Height) > 1E-06f);
		this.inputFieldOffsetHeight.SetTextWithoutNotify(CS$<>8__locals1.blockFirst.Height.ToString() + (heightDiff ? "*" : ""));
		this.inputFieldCost.characterValidation = TMP_InputField.CharacterValidation.Integer;
		this.inputFieldCost.onValueChanged.ResetListener(delegate(string v)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				List<AdventureBlockSnapshot> blocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
				IEnumerable<AdventureBlockSnapshot> source = blocks;
				Func<AdventureBlockSnapshot, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__21) == null)
				{
					predicate = (CS$<>8__locals1.<>9__21 = ((AdventureBlockSnapshot x) => CS$<>8__locals1.indexes.Contains(x.Index)));
				}
				foreach (AdventureBlockSnapshot block in source.Where(predicate))
				{
					int newValue;
					block.TimeCost = (int.TryParse(v, out newValue) ? newValue : block.TimeCost);
				}
			}, EAdventureEditType.BlockProperties);
		});
		bool timeCostDiff = multiSelectBlocks.Any((AdventureBlockSnapshot b) => b.TimeCost != CS$<>8__locals1.blockFirst.TimeCost);
		this.inputFieldCost.SetTextWithoutNotify(CS$<>8__locals1.blockFirst.TimeCost.ToString() + (timeCostDiff ? "*" : ""));
		this.entryPriority.characterValidation = TMP_InputField.CharacterValidation.Integer;
		this.entryPriority.onValueChanged.ResetListener(delegate(string v)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				List<AdventureBlockSnapshot> blocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
				IEnumerable<AdventureBlockSnapshot> source = blocks;
				Func<AdventureBlockSnapshot, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__23) == null)
				{
					predicate = (CS$<>8__locals1.<>9__23 = ((AdventureBlockSnapshot x) => CS$<>8__locals1.indexes.Contains(x.Index)));
				}
				foreach (AdventureBlockSnapshot block in source.Where(predicate))
				{
					bool flag = block.BlockType.Contains(EAdventureBlockType.In);
					if (flag)
					{
						int newValue;
						block.EntryPriority = (int.TryParse(v, out newValue) ? newValue : block.EntryPriority);
					}
				}
			}, EAdventureEditType.BlockProperties);
		});
		bool entryPriorityDiff = multiSelectBlocks.Any((AdventureBlockSnapshot b) => b.EntryPriority != CS$<>8__locals1.blockFirst.EntryPriority);
		this.entryPriority.SetTextWithoutNotify(CS$<>8__locals1.blockFirst.EntryPriority.ToString() + (entryPriorityDiff ? "*" : ""));
		this.inputFieldBlockIcon.onValueChanged.ResetListener(delegate(string v)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				List<AdventureBlockSnapshot> blocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
				IEnumerable<AdventureBlockSnapshot> source = blocks;
				Func<AdventureBlockSnapshot, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__25) == null)
				{
					predicate = (CS$<>8__locals1.<>9__25 = ((AdventureBlockSnapshot x) => CS$<>8__locals1.indexes.Contains(x.Index)));
				}
				foreach (AdventureBlockSnapshot block in source.Where(predicate))
				{
					block.Icon = v;
				}
			}, EAdventureEditType.BlockProperties);
		});
		this.inputFieldBlockIcon.SetTextWithoutNotify(CS$<>8__locals1.blockFirst.Icon);
		this.inputFieldBlockDecorates.onEndEdit.ResetListener(delegate(string v)
		{
			string[] decorates = v.Split(',', StringSplitOptions.None);
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				List<AdventureBlockSnapshot> blocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
				IEnumerable<AdventureBlockSnapshot> source = blocks;
				Func<AdventureBlockSnapshot, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__27) == null)
				{
					predicate = (CS$<>8__locals1.<>9__27 = ((AdventureBlockSnapshot x) => CS$<>8__locals1.indexes.Contains(x.Index)));
				}
				foreach (AdventureBlockSnapshot block in source.Where(predicate))
				{
					block.Decorates.Clear();
					block.Decorates.AddRange(decorates);
				}
			}, EAdventureEditType.BlockProperties);
		});
		this.inputFieldBlockDecorates.SetTextWithoutNotify(string.Join(",", CS$<>8__locals1.blockFirst.Decorates));
		this.decoratesToggle.onValueChanged.ResetListener(delegate(bool isOn)
		{
			CS$<>8__locals1.<>4__this.decoratesEditorPanel.gameObject.SetActive(isOn);
			bool activeInHierarchy2 = CS$<>8__locals1.<>4__this.decoratesEditorPanel.gameObject.activeInHierarchy;
			if (activeInHierarchy2)
			{
				base.<RefreshFields>g__RebuildDecoratesEditorPanel|12();
			}
		});
		bool activeInHierarchy = this.decoratesEditorPanel.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			CS$<>8__locals1.<RefreshFields>g__RebuildDecoratesEditorPanel|12();
		}
		this.cloudToggle.SetIsOnWithoutNotify(CS$<>8__locals1.blockFirst.InCloud);
		this.cloudToggle.onValueChanged.ResetListener(delegate(bool isOn)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				IEnumerable<AdventureBlockSnapshot> blocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
				Func<AdventureBlockSnapshot, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__39) == null)
				{
					predicate = (CS$<>8__locals1.<>9__39 = ((AdventureBlockSnapshot x) => CS$<>8__locals1.indexes.Contains(x.Index)));
				}
				foreach (AdventureBlockSnapshot block in blocks.Where(predicate))
				{
					block.InCloud = isOn;
				}
			}, EAdventureEditType.BlockProperties);
		});
		this.enterCondition.onClick.ResetListener(delegate()
		{
			base.<RefreshFields>g__ShowConditionEditor|17((AdventureBlockSnapshot x) => x.EnterCondition, delegate(AdventureBlockSnapshot x, string json)
			{
				x.EnterCondition = new InstructionAdaptor
				{
					EventScriptJson = json,
					EventScriptType = 6
				};
			});
		});
		this.exitCondition.onClick.ResetListener(delegate()
		{
			base.<RefreshFields>g__ShowConditionEditor|17((AdventureBlockSnapshot x) => x.ExitCondition, delegate(AdventureBlockSnapshot x, string json)
			{
				x.ExitCondition = new InstructionAdaptor
				{
					EventScriptJson = json,
					EventScriptType = 6
				};
			});
		});
		this.passCondition.onClick.ResetListener(delegate()
		{
			base.<RefreshFields>g__ShowConditionEditor|17((AdventureBlockSnapshot x) => x.PassableCondition, delegate(AdventureBlockSnapshot x, string json)
			{
				x.PassableCondition = new InstructionAdaptor
				{
					EventScriptJson = json,
					EventScriptType = 6
				};
			});
		});
	}

	// Token: 0x040011E6 RID: 4582
	public const int RandomHeight = -2147483648;

	// Token: 0x040011E7 RID: 4583
	[SerializeField]
	private TMP_InputField inputFieldOffsetHeight;

	// Token: 0x040011E8 RID: 4584
	[SerializeField]
	private CButton enterCondition;

	// Token: 0x040011E9 RID: 4585
	[SerializeField]
	private CButton exitCondition;

	// Token: 0x040011EA RID: 4586
	[SerializeField]
	private CButton passCondition;

	// Token: 0x040011EB RID: 4587
	[SerializeField]
	private TMP_InputField inputFieldCost;

	// Token: 0x040011EC RID: 4588
	[SerializeField]
	private TMP_InputField inputFieldBlockIcon;

	// Token: 0x040011ED RID: 4589
	[SerializeField]
	private TMP_InputField inputFieldBlockDecorates;

	// Token: 0x040011EE RID: 4590
	[SerializeField]
	private TMP_InputField entryPriority;

	// Token: 0x040011EF RID: 4591
	[SerializeField]
	private CToggle decoratesToggle;

	// Token: 0x040011F0 RID: 4592
	[SerializeField]
	private TemplatedContainerAssemblyNew decoratesEditorPanel;

	// Token: 0x040011F1 RID: 4593
	[SerializeField]
	private EventEditorScript eventEditorScript;

	// Token: 0x040011F2 RID: 4594
	[SerializeField]
	private CToggle cloudToggle;

	// Token: 0x040011F3 RID: 4595
	private bool _awaken;

	// Token: 0x040011F4 RID: 4596
	private int _topLayerSort;

	// Token: 0x040011F5 RID: 4597
	private List<AdventureBlockIndex> _blocks;
}
