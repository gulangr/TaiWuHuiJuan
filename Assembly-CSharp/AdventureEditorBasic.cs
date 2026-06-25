using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EventEditor;
using EventEditor.EventScript;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200016A RID: 362
public class AdventureEditorBasic : MonoBehaviour, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x0600141E RID: 5150 RVA: 0x0007DB54 File Offset: 0x0007BD54
	private void Awake()
	{
		this.definition.onEndEdit.AddListener(new UnityAction<string>(this.OnDefinitionChanged));
		this.minorVersion.onEndEdit.AddListener(new UnityAction<string>(this.OnMinorVersionChanged));
		this.nameComponent.onEndEdit.AddListener(new UnityAction<string>(this.OnNameChanged));
		this.descComponent.onEndEdit.AddListener(new UnityAction<string>(this.OnDescChanged));
		this.commentComponent.onEndEdit.AddListener(new UnityAction<string>(this.OnCommentChanged));
		this.eventTexture.onEndEdit.AddListener(new UnityAction<string>(this.OnEventTextureChanged));
		this.target.onEndEdit.AddListener(new UnityAction<string>(this.OnTargetChanged));
		this.reward.onEndEdit.AddListener(new UnityAction<string>(this.OnRewardChanged));
		List<string> options = new List<string>();
		for (int i = 0; i < 9; i++)
		{
			options.Add(LocalStringManager.Get(string.Format("LK_Grade_{0}", i)));
		}
		this.difficultyDropdown.AddOptions(options);
		this.difficultyDropdown.onSelect.ResetListener(new Action<int>(this.OnDifficultyChanged));
		this.sizeComponent.OnValueChanged += this.OnSizeValueChanged;
		this.stayMonthsComponent.onEndEdit.AddListener(new UnityAction<string>(this.OnStayMonthsChanged));
		this.cloudStyle.onEndEdit.AddListener(new UnityAction<string>(this.OnCloudStyleChanged));
		this.releasedToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnReleasedChanged));
		this.bornConditionBtn.ClearAndAddListener(new Action(this.EditorAutoGenerateCondition));
		this.advanceMonthBtn.ClearAndAddListener(new Action(this.EditorAdvanceMonthCondition));
		this.preAdvanceMonthBtn.ClearAndAddListener(new Action(this.EditorPreAdvanceMonthCondition));
		this.activeActionBtn.ClearAndAddListener(new Action(this.EditorActiveAction));
		this.removeActionBtn.ClearAndAddListener(new Action(this.EditorRemoveAction));
		this.fixAbnormalActionBtn.ClearAndAddListener(new Action(this.EditorFixAbnormalAction));
		this.adventureCostData.ClearAndAddListener(new Action(this.EditorAdventureCostData));
		this.groupManageBtn.ClearAndAddListener(new Action(this.OnGroupManageBtnClick));
		this.lightingBtn.ClearAndAddListener(new Action(this.OnLightingBtnClick));
		this.typeTagDropdown.ClearOptions();
		this.typeTagDropdown.AddOptions(this.GetTypeTagsStringList());
		this.typeTagDropdown.onValueChanged.ResetListener(new Action<int>(this.TypeTagsValueChanged));
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x0007DE2C File Offset: 0x0007C02C
	private List<string> GetTypeTagsStringList()
	{
		return new List<string>
		{
			LocalStringManager.Get(LanguageKey.Lk_Adventure_Editor_Tag_MainStory),
			LocalStringManager.Get(LanguageKey.Lk_Adventure_Editor_Tag_SectStory),
			LocalStringManager.Get(LanguageKey.Lk_Adventure_Editor_Tag_SectCompetition),
			LocalStringManager.Get(LanguageKey.LK_Other)
		};
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0007DE8C File Offset: 0x0007C08C
	private int GetTypeTags(AdventureSnapshot snapshot)
	{
		bool flag = snapshot.Tags == null || snapshot.Tags.Count == 0;
		int result;
		if (flag)
		{
			result = 3;
		}
		else
		{
			result = (int)snapshot.Tags.First<EAdventureTag>();
		}
		return result;
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0007DECC File Offset: 0x0007C0CC
	private void TypeTagsValueChanged(int value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			bool flag = value > 2;
			if (flag)
			{
				snapshot.Tags.Clear();
			}
			else
			{
				bool flag2 = snapshot.Tags.Count == 0;
				if (flag2)
				{
					snapshot.Tags.Add(EAdventureTag.MainStory);
				}
				snapshot.Tags[0] = (EAdventureTag)value;
			}
		}, EAdventureEditType.Basic);
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06001422 RID: 5154 RVA: 0x0007DEFF File Offset: 0x0007C0FF
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnEdit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001423 RID: 5155 RVA: 0x0007DF02 File Offset: 0x0007C102
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0007DF08 File Offset: 0x0007C108
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		AdventureSnapshot snapshot = AdventureEditorKit.BlackBoard.Editing;
		bool flag = editType.Contains(EAdventureEditType.Basic);
		if (flag)
		{
			this.nameComponent.SetTextWithoutNotify(snapshot.Name);
			this.descComponent.SetTextWithoutNotify(snapshot.Desc);
			this.commentComponent.SetTextWithoutNotify(snapshot.Comment);
			this.definition.SetTextWithoutNotify(snapshot.Definition);
			this.minorVersion.SetTextWithoutNotify(snapshot.MinorVersion.ToString());
			this.eventTexture.SetTextWithoutNotify(snapshot.EventTexture);
			this.target.SetTextWithoutNotify(snapshot.DescTarget);
			this.reward.SetTextWithoutNotify(snapshot.DescReward);
			this.difficultyDropdown.SetValueWithoutNotify(snapshot.Grade);
			this.stayMonthsComponent.SetTextWithoutNotify(snapshot.StayMonths.ToString());
			this.releasedToggle.isOn = snapshot.Released;
			this.cloudStyle.SetTextWithoutNotify(AdventureEditorKit.BlackBoard.Editing.Style.ToString());
			this.typeTagDropdown.SetValueWithoutNotify(this.GetTypeTags(snapshot));
		}
		bool flag2 = editType.Contains(EAdventureEditType.Size);
		if (flag2)
		{
			this.sizeComponent.ValueInt = snapshot.Size;
		}
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0007E068 File Offset: 0x0007C268
	private void OnNameChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Name = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0007E09C File Offset: 0x0007C29C
	private void OnDescChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Desc = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0007E0D0 File Offset: 0x0007C2D0
	private void OnCommentChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Comment = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0007E104 File Offset: 0x0007C304
	private void OnDefinitionChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Definition = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0007E138 File Offset: 0x0007C338
	private void OnMinorVersionChanged(string arg0)
	{
		int value;
		bool flag = int.TryParse(arg0, out value);
		if (flag)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.MinorVersion = value;
			}, EAdventureEditType.Basic);
		}
		else
		{
			this.minorVersion.SetTextWithoutNotify(AdventureEditorKit.BlackBoard.Editing.MinorVersion.ToString());
		}
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0007E198 File Offset: 0x0007C398
	private void OnEventTextureChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.EventTexture = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0007E1CC File Offset: 0x0007C3CC
	private void OnTargetChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.DescTarget = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x0007E200 File Offset: 0x0007C400
	private void OnRewardChanged(string arg0)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.DescReward = arg0;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x0007E234 File Offset: 0x0007C434
	private void OnDifficultyChanged(int value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Grade = value;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0007E267 File Offset: 0x0007C467
	private void OnSizeValueChanged()
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Size = this.sizeComponent.ValueInt;
			snapshot.FixBlocks();
		}, EAdventureEditType.Size);
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x0007E284 File Offset: 0x0007C484
	private void OnStayMonthsChanged(string arg0)
	{
		uint value;
		bool flag = uint.TryParse(arg0, out value);
		if (flag)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.StayMonths = value;
			}, EAdventureEditType.Basic);
		}
		else
		{
			this.stayMonthsComponent.text = AdventureEditorKit.BlackBoard.Editing.StayMonths.ToString();
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x0007E2E4 File Offset: 0x0007C4E4
	private void OnCloudStyleChanged(string arg0)
	{
		uint value;
		bool flag = uint.TryParse(arg0, out value);
		if (flag)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.Style = value;
			}, EAdventureEditType.Basic);
		}
		else
		{
			this.cloudStyle.text = AdventureEditorKit.BlackBoard.Editing.Style.ToString();
		}
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x0007E344 File Offset: 0x0007C544
	private void OnReleasedChanged(bool released)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
		{
			snapshot.Released = released;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x0007E378 File Offset: 0x0007C578
	private void EditorAutoGenerateCondition()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = AdventureEditorKit.BlackBoard.Editing.AutoGenerateCondition == null || AdventureEditorKit.BlackBoard.Editing.AutoGenerateCondition.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			script = JsonConvert.DeserializeObject<EventScriptEditorData>(AdventureEditorKit.BlackBoard.Editing.AutoGenerateCondition.EventScriptJson);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData jD)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot interSnapshot)
			{
				if (interSnapshot.AutoGenerateCondition == null)
				{
					interSnapshot.AutoGenerateCondition = new InstructionAdaptor();
				}
				interSnapshot.AutoGenerateCondition.EventScriptJson = JsonConvert.SerializeObject(jD);
				interSnapshot.AutoGenerateCondition.EventScriptType = 6;
			}, EAdventureEditType.Basic);
		}, 6, string.Empty, string.Empty);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x0007E434 File Offset: 0x0007C634
	private void EditorAdvanceMonthCondition()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = AdventureEditorKit.BlackBoard.Editing.AdvanceMonthAction == null || AdventureEditorKit.BlackBoard.Editing.AdvanceMonthAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			script = JsonConvert.DeserializeObject<EventScriptEditorData>(AdventureEditorKit.BlackBoard.Editing.AdvanceMonthAction.EventScriptJson);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData jD)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot interSnapshot)
			{
				if (interSnapshot.AdvanceMonthAction == null)
				{
					interSnapshot.AdvanceMonthAction = new InstructionAdaptor();
				}
				interSnapshot.AdvanceMonthAction.EventScriptJson = JsonConvert.SerializeObject(jD);
				interSnapshot.AdvanceMonthAction.EventScriptType = 7;
			}, EAdventureEditType.Basic);
		}, 7, string.Empty, string.Empty);
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x0007E4F0 File Offset: 0x0007C6F0
	private void EditorPreAdvanceMonthCondition()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = AdventureEditorKit.BlackBoard.Editing.PreAdvanceMonthAction == null || AdventureEditorKit.BlackBoard.Editing.PreAdvanceMonthAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			script = JsonConvert.DeserializeObject<EventScriptEditorData>(AdventureEditorKit.BlackBoard.Editing.PreAdvanceMonthAction.EventScriptJson);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData jD)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot interSnapshot)
			{
				if (interSnapshot.PreAdvanceMonthAction == null)
				{
					interSnapshot.PreAdvanceMonthAction = new InstructionAdaptor();
				}
				interSnapshot.PreAdvanceMonthAction.EventScriptJson = JsonConvert.SerializeObject(jD);
				interSnapshot.PreAdvanceMonthAction.EventScriptType = 7;
			}, EAdventureEditType.Basic);
		}, 7, string.Empty, string.Empty);
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0007E5AC File Offset: 0x0007C7AC
	private void EditorAdventureCostData()
	{
		this.adventureCostDataEditor.gameObject.SetActive(true);
		AdventureSnapshot editing = AdventureEditorKit.BlackBoard.Editing;
		if (editing.Cost == null)
		{
			editing.Cost = new AdventureCostData();
		}
		this.adventureCostDataEditor.Refresh(ref AdventureEditorKit.BlackBoard.Editing.Cost);
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x0007E608 File Offset: 0x0007C808
	private void EditorActiveAction()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = AdventureEditorKit.BlackBoard.Editing.ActiveAction == null || AdventureEditorKit.BlackBoard.Editing.ActiveAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			script = JsonConvert.DeserializeObject<EventScriptEditorData>(AdventureEditorKit.BlackBoard.Editing.ActiveAction.EventScriptJson);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData jD)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot interSnapshot)
			{
				if (interSnapshot.ActiveAction == null)
				{
					interSnapshot.ActiveAction = new InstructionAdaptor();
				}
				interSnapshot.ActiveAction.EventScriptJson = JsonConvert.SerializeObject(jD);
				interSnapshot.ActiveAction.EventScriptType = 8;
			}, EAdventureEditType.Basic);
		}, 8, string.Empty, string.Empty);
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x0007E6C4 File Offset: 0x0007C8C4
	private void EditorRemoveAction()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = AdventureEditorKit.BlackBoard.Editing.RemoveAction == null || AdventureEditorKit.BlackBoard.Editing.RemoveAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			script = JsonConvert.DeserializeObject<EventScriptEditorData>(AdventureEditorKit.BlackBoard.Editing.RemoveAction.EventScriptJson);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData jD)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot interSnapshot)
			{
				if (interSnapshot.RemoveAction == null)
				{
					interSnapshot.RemoveAction = new InstructionAdaptor();
				}
				interSnapshot.RemoveAction.EventScriptJson = JsonConvert.SerializeObject(jD);
				interSnapshot.RemoveAction.EventScriptType = 9;
			}, EAdventureEditType.Basic);
		}, 9, string.Empty, string.Empty);
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0007E780 File Offset: 0x0007C980
	private void EditorFixAbnormalAction()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = AdventureEditorKit.BlackBoard.Editing.FixAbnormalAction == null || AdventureEditorKit.BlackBoard.Editing.FixAbnormalAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			script = JsonConvert.DeserializeObject<EventScriptEditorData>(AdventureEditorKit.BlackBoard.Editing.FixAbnormalAction.EventScriptJson);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData jD)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot interSnapshot)
			{
				if (interSnapshot.FixAbnormalAction == null)
				{
					interSnapshot.FixAbnormalAction = new InstructionAdaptor();
				}
				interSnapshot.FixAbnormalAction.EventScriptJson = JsonConvert.SerializeObject(jD);
				interSnapshot.FixAbnormalAction.EventScriptType = 10;
			}, EAdventureEditType.Basic);
		}, 10, string.Empty, string.Empty);
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0007E83B File Offset: 0x0007CA3B
	private void OnGroupManageBtnClick()
	{
		GEvent.OnEvent(UiEvents.AdventureEditorToggleGroupPanel, null);
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0007E84F File Offset: 0x0007CA4F
	private void OnLightingBtnClick()
	{
		this.lightingPanel.TogglePanel();
	}

	// Token: 0x040010EB RID: 4331
	[SerializeField]
	private TMP_InputField nameComponent;

	// Token: 0x040010EC RID: 4332
	[SerializeField]
	private TMP_InputField descComponent;

	// Token: 0x040010ED RID: 4333
	[SerializeField]
	private TMP_InputField commentComponent;

	// Token: 0x040010EE RID: 4334
	[SerializeField]
	private TMP_InputField definition;

	// Token: 0x040010EF RID: 4335
	[SerializeField]
	private TMP_InputField minorVersion;

	// Token: 0x040010F0 RID: 4336
	[SerializeField]
	private TMP_InputField target;

	// Token: 0x040010F1 RID: 4337
	[SerializeField]
	private TMP_InputField reward;

	// Token: 0x040010F2 RID: 4338
	[SerializeField]
	private TMP_InputField eventTexture;

	// Token: 0x040010F3 RID: 4339
	[SerializeField]
	private CDropdown difficultyDropdown;

	// Token: 0x040010F4 RID: 4340
	[SerializeField]
	private InputCSlider sizeComponent;

	// Token: 0x040010F5 RID: 4341
	[SerializeField]
	private TMP_InputField stayMonthsComponent;

	// Token: 0x040010F6 RID: 4342
	[SerializeField]
	private TMP_InputField cloudStyle;

	// Token: 0x040010F7 RID: 4343
	[SerializeField]
	private CButton groupManageBtn;

	// Token: 0x040010F8 RID: 4344
	[SerializeField]
	private CToggle releasedToggle;

	// Token: 0x040010F9 RID: 4345
	[SerializeField]
	private CButton bornConditionBtn;

	// Token: 0x040010FA RID: 4346
	[SerializeField]
	private EventEditorScript eventEditorScript;

	// Token: 0x040010FB RID: 4347
	[SerializeField]
	private CButton advanceMonthBtn;

	// Token: 0x040010FC RID: 4348
	[SerializeField]
	private CButton preAdvanceMonthBtn;

	// Token: 0x040010FD RID: 4349
	[SerializeField]
	private CButton adventureCostData;

	// Token: 0x040010FE RID: 4350
	[SerializeField]
	private AdventureCostDataEditor adventureCostDataEditor;

	// Token: 0x040010FF RID: 4351
	[SerializeField]
	private CButton activeActionBtn;

	// Token: 0x04001100 RID: 4352
	[SerializeField]
	private CButton removeActionBtn;

	// Token: 0x04001101 RID: 4353
	[SerializeField]
	private CButton fixAbnormalActionBtn;

	// Token: 0x04001102 RID: 4354
	[SerializeField]
	private CButton lightingBtn;

	// Token: 0x04001103 RID: 4355
	[SerializeField]
	private AdventureEditorLightingPanel lightingPanel;

	// Token: 0x04001104 RID: 4356
	[SerializeField]
	private CDropdown typeTagDropdown;

	// Token: 0x04001105 RID: 4357
	[TupleElementNames(new string[]
	{
		"Min",
		"Max"
	})]
	private static readonly ValueTuple<int, int> TimeRange = new ValueTuple<int, int>(0, 500);
}
