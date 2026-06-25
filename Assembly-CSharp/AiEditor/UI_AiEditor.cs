using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AiEditor
{
	// Token: 0x0200068E RID: 1678
	public class UI_AiEditor : UIBase
	{
		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x06004EDA RID: 20186 RVA: 0x0024FAAA File Offset: 0x0024DCAA
		private bool ForcedOperation
		{
			get
			{
				return Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt);
			}
		}

		// Token: 0x06004EDB RID: 20187 RVA: 0x0024FAD4 File Offset: 0x0024DCD4
		private UI_AiEditor.EOptionType NameToType(string optionName)
		{
			UI_AiEditor.EOptionType type;
			return Enum.TryParse<UI_AiEditor.EOptionType>(optionName, out type) ? type : UI_AiEditor.EOptionType.Invalid;
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x06004EDC RID: 20188 RVA: 0x0024FAEF File Offset: 0x0024DCEF
		private AiEditorMainArea MainArea
		{
			get
			{
				return base.CGet<AiEditorMainArea>("MainArea");
			}
		}

		// Token: 0x06004EDD RID: 20189 RVA: 0x0024FAFC File Offset: 0x0024DCFC
		public override void OnInit(ArgumentBox argBox)
		{
			bool flag = !argBox.Get("GroupId", out this._expectGroupId);
			if (flag)
			{
				this._expectGroupId = 1;
			}
			foreach (Toggle toggle in base.CGet<ToggleGroup>("Toggles").GetComponentsInChildren<Toggle>())
			{
				toggle.onValueChanged.RemoveAllListeners();
				toggle.isOn = (this._activateOption == this.NameToType(toggle.name));
				toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			}
			TMP_InputField search = base.CGet<TMP_InputField>("Search");
			search.onValueChanged.RemoveAllListeners();
			search.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchChanged));
			search.onSelect.RemoveAllListeners();
			search.onSelect.AddListener(delegate(string _)
			{
				this.MainArea.BlockInput();
			});
			search.onDeselect.RemoveAllListeners();
			search.onDeselect.AddListener(delegate(string _)
			{
				this.MainArea.UnBlockInput();
			});
			this.UpdateOptions();
			this.UpdateHint();
			this.MainArea.Init();
			Application.targetFrameRate = -1;
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
			{
				Application.targetFrameRate = 60;
			}));
		}

		// Token: 0x06004EDE RID: 20190 RVA: 0x0024FC68 File Offset: 0x0024DE68
		private void Update()
		{
			bool flag = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				UIManager.Instance.StackBack(null);
			}
			bool flag2 = !UIManager.Instance.IsFocusElement(this.Element) && !this.ForcedOperation;
			if (!flag2)
			{
				bool keyUp = Input.GetKeyUp(KeyCode.S);
				if (keyUp)
				{
					this.SaveToFile();
				}
				bool keyUp2 = Input.GetKeyUp(KeyCode.L);
				if (keyUp2)
				{
					this.LoadFromFile();
				}
				bool keyUp3 = Input.GetKeyUp(KeyCode.E);
				if (keyUp3)
				{
					this.ExportToFile();
				}
			}
		}

		// Token: 0x06004EDF RID: 20191 RVA: 0x0024FCF7 File Offset: 0x0024DEF7
		private void OnToggleChanged(bool arg0)
		{
			this.UpdateOptions();
		}

		// Token: 0x06004EE0 RID: 20192 RVA: 0x0024FD00 File Offset: 0x0024DF00
		private void OnSearchChanged(string arg0)
		{
			this.UpdateOptions();
		}

		// Token: 0x06004EE1 RID: 20193 RVA: 0x0024FD0C File Offset: 0x0024DF0C
		private void UpdateOptions()
		{
			LoopVerticalScrollRect scroll = base.CGet<LoopVerticalScrollRect>("Options");
			Toggle toggle = base.CGet<ToggleGroup>("Toggles").GetComponentsInChildren<Toggle>().FirstOrDefault((Toggle x) => x.isOn);
			UI_AiEditor.EOptionType newActivateOption = this.NameToType((toggle != null) ? toggle.name : null);
			string newSearchingText = base.CGet<TMP_InputField>("Search").text;
			bool flag = newActivateOption == this._activateOption && newSearchingText == this._searchingText;
			if (!flag)
			{
				this._activateOption = newActivateOption;
				this._searchingText = newSearchingText;
				this._searchedTemplateIds.Clear();
				UI_AiEditor.EOptionType activateOption = this._activateOption;
				if (!true)
				{
				}
				IEnumerable<IAiConfigTuple> enumerable;
				switch (activateOption)
				{
				case UI_AiEditor.EOptionType.Node:
					enumerable = AiNode.Instance;
					break;
				case UI_AiEditor.EOptionType.Condition:
					enumerable = AiCondition.Instance;
					break;
				case UI_AiEditor.EOptionType.Action:
					enumerable = AiAction.Instance;
					break;
				default:
					enumerable = null;
					break;
				}
				if (!true)
				{
				}
				IEnumerable<IAiConfigTuple> tuples = enumerable;
				tuples = ((tuples != null) ? (from x in tuples
				where AiGroup.Instance[x.GroupId].GroupIds.Contains(this._expectGroupId)
				select x) : null);
				bool flag2 = tuples != null && !string.IsNullOrEmpty(this._searchingText);
				if (flag2)
				{
					tuples = from x in tuples
					where x.Name.Contains(this._searchingText) || x.Desc.Contains(this._searchingText)
					select x;
				}
				bool flag3 = tuples != null;
				if (flag3)
				{
					this._searchedTemplateIds.AddRange(from x in tuples
					select x.TemplateId);
				}
				bool flag4 = this._searchedTemplateIds.Count == 0;
				if (flag4)
				{
					scroll.ClearCells();
				}
				else
				{
					LoopVerticalScrollRect loopVerticalScrollRect = scroll;
					GameObject item = base.CGet<Refers>("Templates").CGet<GameObject>("TemplateIdItem");
					int count = this._searchedTemplateIds.Count;
					UI_AiEditor.EOptionType activateOption2 = this._activateOption;
					if (!true)
					{
					}
					Action<Transform, int> callback;
					switch (activateOption2)
					{
					case UI_AiEditor.EOptionType.Node:
						callback = new Action<Transform, int>(this.OnRenderNode);
						break;
					case UI_AiEditor.EOptionType.Condition:
						callback = new Action<Transform, int>(this.OnRenderCondition);
						break;
					case UI_AiEditor.EOptionType.Action:
						callback = new Action<Transform, int>(this.OnRenderAction);
						break;
					default:
						callback = null;
						break;
					}
					if (!true)
					{
					}
					loopVerticalScrollRect.InitLoop(item, count, callback, null);
				}
			}
		}

		// Token: 0x06004EE2 RID: 20194 RVA: 0x0024FF35 File Offset: 0x0024E135
		private void UpdateHint()
		{
			base.CGet<TextMeshProUGUI>("Hint").text = string.Empty;
		}

		// Token: 0x06004EE3 RID: 20195 RVA: 0x0024FF50 File Offset: 0x0024E150
		private void OnRenderNode(Transform item, int index)
		{
			int templateId = this._searchedTemplateIds[index];
			AiNodeItem config = AiNode.Instance[templateId];
			item.GetComponent<AiTemplateIdItem>().Refresh(new Action<int>(this.MainArea.AppendLinear), templateId, config.Name, config.Desc);
		}

		// Token: 0x06004EE4 RID: 20196 RVA: 0x0024FFA4 File Offset: 0x0024E1A4
		private void OnRenderCondition(Transform item, int index)
		{
			int templateId = this._searchedTemplateIds[index];
			AiConditionItem config = AiCondition.Instance[templateId];
			item.GetComponent<AiTemplateIdItem>().Refresh(new Action<int>(this.MainArea.AppendBranch), templateId, config.Name, config.Desc);
		}

		// Token: 0x06004EE5 RID: 20197 RVA: 0x0024FFF8 File Offset: 0x0024E1F8
		private void OnRenderAction(Transform item, int index)
		{
			int templateId = this._searchedTemplateIds[index];
			AiActionItem config = AiAction.Instance[templateId];
			item.GetComponent<AiTemplateIdItem>().Refresh(new Action<int>(this.MainArea.AppendAction), templateId, config.Name, config.Desc);
		}

		// Token: 0x06004EE6 RID: 20198 RVA: 0x0025004C File Offset: 0x0024E24C
		private void SaveToFile()
		{
			string path = LocalDialog.SelectSaveFilePath("Ai Blueprint Files(*.aibp)\0*.aibp\0", Application.persistentDataPath);
			bool flag = path.IsNullOrEmpty();
			if (!flag)
			{
				bool flag2 = !path.EndsWith(".aibp");
				if (flag2)
				{
					path += ".aibp";
				}
				AiBlueprintSnapshot data = this.MainArea.Save();
				data.Save(path);
				Debug.Log("save success: " + path);
			}
		}

		// Token: 0x06004EE7 RID: 20199 RVA: 0x002500BC File Offset: 0x0024E2BC
		private void LoadFromFile()
		{
			string path = LocalDialog.SelectLoadFilePath("Ai Blueprint Files(*.aibp)\0*.aibp\0", Application.persistentDataPath);
			bool flag = path.IsNullOrEmpty();
			if (!flag)
			{
				AiBlueprintSnapshot data = new AiBlueprintSnapshot();
				bool flag2 = !data.Load(path);
				if (!flag2)
				{
					this.MainArea.Load(data);
					Debug.Log("load success: " + path);
				}
			}
		}

		// Token: 0x06004EE8 RID: 20200 RVA: 0x0025011C File Offset: 0x0024E31C
		private void ExportToFile()
		{
			string path = LocalDialog.SelectLoadFilePath("Ai Data Files(*.aid)\0*.aid\0", Application.persistentDataPath);
			bool flag = path.IsNullOrEmpty();
			if (!flag)
			{
				try
				{
					AiBlueprintSnapshot blueprint = this.MainArea.Save();
					blueprint.Export(path);
				}
				catch (AiParamInvalidException invalid)
				{
					AdaptableLog.TagWarning("AiEditor", "Cannot export by missing parameters, please check blue print\n" + invalid.Message, true);
					return;
				}
				catch (Exception e)
				{
					AdaptableLog.TagWarning("AiEditor", "Export failed with exception: " + e.Message + ", stacktrace:\n" + e.StackTrace, true);
					return;
				}
				Debug.Log("export success: " + path);
			}
		}

		// Token: 0x04003658 RID: 13912
		private int _expectGroupId;

		// Token: 0x04003659 RID: 13913
		private UI_AiEditor.EOptionType _activateOption = UI_AiEditor.EOptionType.Invalid;

		// Token: 0x0400365A RID: 13914
		private string _searchingText;

		// Token: 0x0400365B RID: 13915
		private readonly List<int> _searchedTemplateIds = new List<int>();

		// Token: 0x02001AB6 RID: 6838
		public enum EOptionType
		{
			// Token: 0x0400B6DE RID: 46814
			Invalid = -1,
			// Token: 0x0400B6DF RID: 46815
			Node,
			// Token: 0x0400B6E0 RID: 46816
			Condition,
			// Token: 0x0400B6E1 RID: 46817
			Action
		}
	}
}
