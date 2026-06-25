using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using EventEditor;
using EventEditor.EventScript;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200019D RID: 413
public class UI_AdventureMajorEventEditor : UIBase
{
	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06001719 RID: 5913 RVA: 0x0008D7A8 File Offset: 0x0008B9A8
	// (set) Token: 0x0600171A RID: 5914 RVA: 0x0008D7BB File Offset: 0x0008B9BB
	internal AdventureMajorEventEditorPoint AddNextNodePoint
	{
		get
		{
			return this.PointDict.GetValueOrDefault(this._addNextNodePointKey);
		}
		set
		{
			this._addNextNodePointKey = ((value == null) ? -1 : value.Index);
		}
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x0008D7D0 File Offset: 0x0008B9D0
	internal void RefreshShow()
	{
		this.ResetData();
		for (int nodeIndex = 0; nodeIndex < this.Snapshot.Nodes.Count; nodeIndex++)
		{
			AdventureMajorEventNodeSnapshot node = this.Snapshot.Nodes[nodeIndex];
			AdventureMajorEventEditorPoint point = this.GetPointRefers(node, nodeIndex);
			point.SetPointInfo(node, this._sb);
			this.SetLineData(node, nodeIndex);
		}
		this.DrawLine();
		List<AdventureMajorEventDecorationData> decorations = this.Snapshot.Decorations;
		bool flag = decorations.Count > 0;
		if (flag)
		{
			this.backgroundImgName.text = decorations[0].Resource;
			for (int i = 1; i < decorations.Count; i++)
			{
				this.SetDecorations(decorations[i]);
			}
		}
		this.eventTexture.SetTextWithoutNotify(this.Snapshot.EventTexture);
		this.typeTagDropdown.SetValueWithoutNotify(this.GetTypeTags());
		this.definition.SetTextWithoutNotify(this.Snapshot.Definition);
		this.name.SetTextWithoutNotify(this.Snapshot.Name);
		this.desc.SetTextWithoutNotify(this.Snapshot.Desc);
		this.stayMonths.SetTextWithoutNotify(this.Snapshot.StayMonths.ToString());
		this.RefreshCharacter();
		this.RefreshParameter();
		this.RefreshCostData();
		this.releasedToggle.SetIsOnWithoutNotify(this.Snapshot.Released);
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x0008D962 File Offset: 0x0008BB62
	public void RefreshCostData()
	{
		this.adventureCostDataEditor.Refresh(ref this.Snapshot.Cost);
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x0008D97C File Offset: 0x0008BB7C
	public void RefreshCharacter()
	{
		this.charHolder.Rebuild<MajorEventCharacterTemplate>(this.Snapshot.Characters.Count, delegate(MajorEventCharacterTemplate refer, int index)
		{
			refer.RefreshData(index);
		});
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x0008D9BA File Offset: 0x0008BBBA
	public void RefreshParameter()
	{
		this.paramHolder.Rebuild<MajorEventParameterTemplate>(this.Snapshot.Parameters.Count, delegate(MajorEventParameterTemplate refer, int index)
		{
			refer.RefreshData(index);
		});
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x0008D9F8 File Offset: 0x0008BBF8
	private void DrawLine()
	{
		foreach (ValueTuple<int, int> valueTuple in this._lineData)
		{
			int startNodeIndex = valueTuple.Item1;
			int endNodeIndex = valueTuple.Item2;
			AdventureMajorEventEditorPoint startRefer = this.PointDict[startNodeIndex];
			AdventureMajorEventEditorPoint endRefer = this.PointDict[endNodeIndex];
			RectTransform lineRefers = this.GetLineRefers();
			CImage linSprite = lineRefers.GetComponent<CImage>();
			linSprite.SetSprite(AdventureMajorEventTool.GetLineSpriteName(0, 0), false, null);
			linSprite.type = Image.Type.Sliced;
			RectTransform lineRect = linSprite.GetComponent<RectTransform>();
			float lineWidth = AdventureMajorEventTool.GetLineWidth(startRefer.GetComponent<RectTransform>(), endRefer.GetComponent<RectTransform>());
			lineRect.SetWidth(lineWidth);
			lineRect.SetHeight(10f);
			lineRefers.localPosition = AdventureMajorEventTool.GetCenterPos(startRefer.GetComponent<RectTransform>(), endRefer.GetComponent<RectTransform>());
			lineRefers.rotation = AdventureMajorEventTool.GetQuaternion(startRefer.GetComponent<RectTransform>(), endRefer.GetComponent<RectTransform>());
			lineRefers.Rotate(0f, 0f, 90f);
		}
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x0008DB20 File Offset: 0x0008BD20
	private void ResetData()
	{
		this.DestroyLinePrefab();
		this.DestroyPointPrefab();
		this.DestroyDecorationPrefab();
		this.PointDict.Clear();
		this._lineData.Clear();
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x0008DB50 File Offset: 0x0008BD50
	private void SetLineData(AdventureMajorEventNodeSnapshot node, int nodeIndex)
	{
		foreach (int nextNodeIndex in node.NextNodes)
		{
			this._lineData.Add(new ValueTuple<int, int>(nodeIndex, nextNodeIndex));
		}
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0008DBB4 File Offset: 0x0008BDB4
	private AdventureMajorEventEditorPoint GetPointRefers(AdventureMajorEventNodeSnapshot node, int nodeIndex)
	{
		AdventureMajorEventEditorPoint nodeRefers = PoolManager.GetObject<AdventureMajorEventEditorPoint>("UI_AdventureMajorEventEditorPointPrefab");
		nodeRefers.Init(nodeIndex);
		nodeRefers.transform.SetParent(this.nodeRoot, false);
		this.PointDict.Add(nodeIndex, nodeRefers);
		this.SetEventImg(node, nodeIndex);
		return nodeRefers;
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x0008DC04 File Offset: 0x0008BE04
	private void SetEventImg(AdventureMajorEventNodeSnapshot node, int nodeIndex)
	{
		AdventureMajorEventEditorPoint refers = this.PointDict[nodeIndex];
		CRawImage tex = refers.eventImg;
		bool flag = tex == null;
		if (!flag)
		{
			bool flag2 = node.Type != EAdventureMajorEventNodeType.Check;
			if (flag2)
			{
				UI_AdventureMajorEvent.LoadEventTexture(tex, node.EventTexture, true);
			}
			else
			{
				bool flag3 = tex && tex.gameObject.activeSelf;
				if (flag3)
				{
					tex.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x0008DC7C File Offset: 0x0008BE7C
	private RectTransform GetLineRefers()
	{
		RectTransform lineRefers = PoolManager.GetObject<RectTransform>("UI_AdventureMajorEventEditorLinePrefab");
		lineRefers.gameObject.SetActive(true);
		lineRefers.transform.SetParent(this.lineRoot, false);
		this._lineRefersList.Add(lineRefers);
		return lineRefers;
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x0008DCC8 File Offset: 0x0008BEC8
	private void SetDecorations(AdventureMajorEventDecorationData adventureMajorEventDecorationData)
	{
		AdventureMajorEventEditorDecorationPrefab decorationRefers = PoolManager.GetObject<AdventureMajorEventEditorDecorationPrefab>("UI_AdventureMajorEventEditorDecorationPrefab");
		decorationRefers.gameObject.SetActive(true);
		decorationRefers.transform.SetParent(this.decorationRoot, false);
		decorationRefers.GetComponent<RectTransform>().anchoredPosition = new Vector2(adventureMajorEventDecorationData.X, adventureMajorEventDecorationData.Y);
		CRawImage image = decorationRefers.decoration;
		TMP_InputField input = decorationRefers.input;
		ResLoader.LoadModOrGameResource<Texture2D>("AdventureMajorEvent/" + adventureMajorEventDecorationData.Resource, delegate(Texture2D texture)
		{
			image.texture = texture;
			image.SetNativeSize();
		}, null);
		input.onEndEdit.RemoveAllListeners();
		input.text = (adventureMajorEventDecorationData.Resource ?? string.Empty);
		input.onEndEdit.AddListener(delegate(string str)
		{
			ResLoader.LoadModOrGameResource<Texture2D>("AdventureMajorEvent/" + str, delegate(Texture2D texture)
			{
				image.texture = texture;
				image.SetNativeSize();
				adventureMajorEventDecorationData.Resource = str;
			}, null);
		});
		decorationRefers.commonButtonCloseLevelTwo.ClearAndAddListener(delegate
		{
			PoolManager.Destroy("UI_AdventureMajorEventEditorDecorationPrefab", decorationRefers.gameObject);
			this.Snapshot.Decorations.Remove(adventureMajorEventDecorationData);
		});
		decorationRefers.dragNode.OnDragEnd = delegate()
		{
			adventureMajorEventDecorationData.X = decorationRefers.GetComponent<RectTransform>().anchoredPosition.x;
			adventureMajorEventDecorationData.Y = decorationRefers.GetComponent<RectTransform>().anchoredPosition.y;
		};
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x0008DE0D File Offset: 0x0008C00D
	internal void PointOnDragEnd()
	{
		this.RefreshNodePosData();
		this.DestroyLinePrefab();
		this.DrawLine();
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x0008DE28 File Offset: 0x0008C028
	private void RefreshNodePosData()
	{
		for (int nodeIndex = 0; nodeIndex < this.Snapshot.Nodes.Count; nodeIndex++)
		{
			AdventureMajorEventNodeSnapshot node = this.Snapshot.Nodes[nodeIndex];
			AdventureMajorEventEditorPoint refers = this.PointDict[nodeIndex];
			node.X = refers.GetComponent<RectTransform>().localPosition.x;
			node.Y = refers.GetComponent<RectTransform>().localPosition.y;
		}
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x0008DEA4 File Offset: 0x0008C0A4
	public EAdventureMajorEventNodeType GetNodeTypeByParent(AdventureMajorEventNodeSnapshot parentNode, AdventureMajorEventNodeSnapshot currNode)
	{
		bool flag = UI_AdventureMajorEventEditor.<GetNodeTypeByParent>g__IsSpecialNode|50_0(currNode.Type) || parentNode == null;
		EAdventureMajorEventNodeType result;
		if (flag)
		{
			result = currNode.Type;
		}
		else
		{
			bool flag2 = UI_AdventureMajorEventEditor.<GetNodeTypeByParent>g__IsSpecialNode|50_0(parentNode.Type);
			if (flag2)
			{
				result = EAdventureMajorEventNodeType.Check;
			}
			else
			{
				result = currNode.Type;
			}
		}
		return result;
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x0008DEF4 File Offset: 0x0008C0F4
	private void Awake()
	{
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorPointPrefab", this.pointPrefab.gameObject);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorLinePrefab", this.linePrefab);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorShortLinePrefab", this.shortLinePrefab);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorDecorationPrefab", this.decorationPrefab.gameObject);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorRewardGroupEditorPrefabKeyPrefab", this.rewardGroupEditorPrefab.gameObject);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorLifeSkillEditorPrefabKeyPrefab", this.lifeSkillEditorPrefab.gameObject);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorCombatSkillEditorPrefabKeyPrefab", this.combatSkillEditorPrefab.gameObject);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorPersonalityEditorPrefabKeyPrefab", this.personalityEditorPrefab.gameObject);
		PoolManager.SetSrcObject("UI_AdventureMajorEventEditorItemEditorPrefabKeyPrefab", this.itemEditorPrefab.gameObject);
		this.setAutoNodePos.onClick.ResetListener(new Action(this.AutoNodePos));
		this.releasedToggle.onValueChanged.ResetListener(delegate(bool isOn)
		{
			this.Snapshot.Released = isOn;
		});
		this.eventTexture.onEndEdit.ResetListener(new Action<string>(this.EditEventTexture));
		this.typeTagDropdown.ClearOptions();
		this.typeTagDropdown.AddOptions(this.GetTypeTagsStringList());
		this.typeTagDropdown.onValueChanged.ResetListener(new Action<int>(this.TypeTagsValueChanged));
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x0008E050 File Offset: 0x0008C250
	private void OnDestroy()
	{
		PoolManager.RemoveData("UI_AdventureMajorEventEditorPointPrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorLinePrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorShortLinePrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorDecorationPrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorRewardGroupEditorPrefabKeyPrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorLifeSkillEditorPrefabKeyPrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorCombatSkillEditorPrefabKeyPrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorPersonalityEditorPrefabKeyPrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventEditorItemEditorPrefabKeyPrefab");
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x0008E0C1 File Offset: 0x0008C2C1
	private void OnEnable()
	{
		AdventureEditorKit.CheckCorePath();
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x0008E0CA File Offset: 0x0008C2CA
	private void OnDisable()
	{
		this.EndAddSubNodes();
		this.DestroyPointPrefab();
		this.DestroyLinePrefab();
		this.DestroyDecorationPrefab();
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x0008E0EC File Offset: 0x0008C2EC
	private void Update()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.Dialog);
		if (!flag)
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				CommonUtils.ShowConfirmDialog(LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_Exit_Title), LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_Exit_Content), delegate
				{
					UIManager.Instance.StackBack(null);
				}, null, EDialogType.None);
			}
			else
			{
				bool flag3 = !AdventureEditorKit.GetControlKey;
				if (!flag3)
				{
					bool keyUp = Input.GetKeyUp(KeyCode.S);
					if (keyUp)
					{
						this.Save();
					}
					bool keyUp2 = Input.GetKeyUp(KeyCode.L);
					if (keyUp2)
					{
						this.Load();
					}
					bool keyUp3 = Input.GetKeyUp(KeyCode.G);
					if (keyUp3)
					{
						this.Export();
					}
				}
			}
		}
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x0008E1B1 File Offset: 0x0008C3B1
	public override void OnInit(ArgumentBox argsBox)
	{
		this._editingPath = string.Empty;
		this.RefreshShow();
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x0008E1C6 File Offset: 0x0008C3C6
	private void DestroyLinePrefab()
	{
		this._lineRefersList.ForEach(delegate(RectTransform e)
		{
			bool flag = e == null;
			if (!flag)
			{
				PoolManager.Destroy("UI_AdventureMajorEventEditorLinePrefab", e.gameObject);
			}
		});
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x0008E1F4 File Offset: 0x0008C3F4
	private void DestroyPointPrefab()
	{
		this.PointDict.Values.ToList<AdventureMajorEventEditorPoint>().ForEach(delegate(AdventureMajorEventEditorPoint e)
		{
			bool flag = e == null;
			if (!flag)
			{
				PoolManager.Destroy("UI_AdventureMajorEventEditorPointPrefab", e.gameObject);
			}
		});
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x0008E22C File Offset: 0x0008C42C
	private void DestroyDecorationPrefab()
	{
		for (int i = 0; i < this.decorationRoot.childCount; i++)
		{
			Transform child = this.decorationRoot.GetChild(i);
			PoolManager.Destroy("UI_AdventureMajorEventEditorDecorationPrefab", child.gameObject);
		}
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x0008E274 File Offset: 0x0008C474
	public void Load()
	{
		string path = LocalDialog.SelectLoadFilePath("Adventure Major Event Files(*.advme)\0*.advme\0", AdventureEditorKit.MajorEventDirectory);
		bool flag = !AdventureMajorEventSnapshot.TryLoadFromFile(path, out this.Snapshot);
		if (flag)
		{
			this.Snapshot = new AdventureMajorEventSnapshot
			{
				Cost = new AdventureCostData()
			};
		}
		else
		{
			this._editingPath = path;
			this.RefreshShow();
		}
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x0008E2CC File Offset: 0x0008C4CC
	public void Save()
	{
		foreach (AdventureMajorEventNodeSnapshot node in this.Snapshot.Nodes)
		{
			foreach (int childNodeIndex in node.NextNodes)
			{
				this.Snapshot.Nodes[childNodeIndex].Type = this.GetNodeTypeByParent(node, this.Snapshot.Nodes[childNodeIndex]);
			}
		}
		string path = this._editingPath;
		bool flag = AdventureEditorKit.GetShiftKey || string.IsNullOrEmpty(path);
		if (flag)
		{
			path = LocalDialog.SelectSaveFilePath("Adventure Major Event Files(*.advme)\0*.advme\0", AdventureEditorKit.MajorEventDirectory);
		}
		bool flag2 = string.IsNullOrEmpty(path);
		if (!flag2)
		{
			this._editingPath = path;
			bool flag3 = !path.EndsWith(".advme");
			if (flag3)
			{
				path += ".advme";
			}
			bool flag4 = string.IsNullOrEmpty(path);
			if (!flag4)
			{
				AdventureMajorEventSnapshot exist;
				this.Snapshot.Id = (AdventureMajorEventSnapshot.TryLoadFromFile(path, out exist) ? exist.Id : AdventureEditorKit.GetNewBlueprintId());
				bool flag5 = this.Snapshot.Id <= 0;
				if (flag5)
				{
					this.Snapshot.Id = AdventureEditorKit.GetNewBlueprintId();
				}
				this.Snapshot.SaveToFile(path);
				CommonUtils.ShowConfirmDialog(LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_Save_Success), LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_Save_Success), null, null, EDialogType.None);
			}
		}
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x0008E47C File Offset: 0x0008C67C
	public void Clear()
	{
		UIElement dialog = UIElement.Dialog;
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		string key = "Cmd";
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Type = 1;
		dialogCmd.Title = LanguageKey.LK_MajorEventEditor_MainMenuOps_Clear_Title.Tr();
		dialogCmd.Content = LanguageKey.LK_MajorEventEditor_MainMenuOps_Clear_Desc.Tr();
		dialogCmd.Yes = new Action(this.ClearImpl);
		dialogCmd.No = delegate()
		{
		};
		dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x0008E517 File Offset: 0x0008C717
	private void ClearImpl()
	{
		this.pointEditor.SwitchOpen(false);
		this.AddNextNodePoint = null;
		this.Snapshot = new AdventureMajorEventSnapshot
		{
			Cost = new AdventureCostData()
		};
		this.RefreshShow();
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x0008E54C File Offset: 0x0008C74C
	public void EditDefinition(string str)
	{
		this.Snapshot.Definition = str;
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x0008E55A File Offset: 0x0008C75A
	public void EditName(string str)
	{
		this.Snapshot.Name = str;
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x0008E56D File Offset: 0x0008C76D
	public void EditDesc(string str)
	{
		this.Snapshot.Desc = str;
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x0008E580 File Offset: 0x0008C780
	public void EditEventTexture(string str)
	{
		this.Snapshot.EventTexture = str;
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x0008E58E File Offset: 0x0008C78E
	public void EditStayMonths(string str)
	{
		AdventureMajorEventTool.EditUInt(ref this.Snapshot.StayMonths, str, "EditStayMonths");
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x0008E5A7 File Offset: 0x0008C7A7
	public void EditBackgroundImgName(string str)
	{
		ResLoader.LoadModOrGameResource<Texture2D>("AdventureMajorEvent/" + str, delegate(Texture2D texture)
		{
			this.backgroundImg.texture = texture;
		}, null);
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x0008E5C8 File Offset: 0x0008C7C8
	public void AddCharacter()
	{
		int index = this.Snapshot.Characters.Count;
		this.Snapshot.Characters.Add(new AdventureCharacterGroup
		{
			Data = new AdventureCharacterData
			{
				Type = EAdventureCharacterType.Invalid,
				FilterRuleTemplateId = 0,
				SearchRangeType = 0
			},
			Count = 0,
			Major = false
		});
		this.RefreshCharacter();
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x0008E638 File Offset: 0x0008C838
	public void AddParameter()
	{
		this.Snapshot.Parameters.Add(new AdventureParameterSnapshot
		{
			Comment = string.Empty,
			Desc = string.Empty,
			Icon = string.Empty,
			InitialValue = 0,
			Key = string.Empty,
			Name = string.Empty
		});
		this.RefreshParameter();
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x0008E6AC File Offset: 0x0008C8AC
	public void EditOnActiveScript()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = this.Snapshot.ActiveAction == null || this.Snapshot.ActiveAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			GameData.Serializer.CommonObjectSerializer.Deserialize<EventScriptEditorData>(this.Snapshot.ActiveAction.EventScriptJson, out script, GameData.Serializer.CommonObjectSerializer.MarshalFormat.Json);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData scriptEditorData)
		{
			AdventureMajorEventSnapshot snapshot = this.Snapshot;
			if (snapshot.ActiveAction == null)
			{
				snapshot.ActiveAction = new InstructionAdaptor();
			}
			this.Snapshot.ActiveAction.EventScriptType = 8;
			GameData.Serializer.CommonObjectSerializer.Serialize<EventScriptEditorData>(scriptEditorData, out this.Snapshot.ActiveAction.EventScriptJson, GameData.Serializer.CommonObjectSerializer.MarshalFormat.Json);
		}, 8, string.Empty, string.Empty);
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x0008E74C File Offset: 0x0008C94C
	public void EditOnRemoveScript()
	{
		this.eventEditorScript.GetComponent<RectTransform>().position = Vector3.zero;
		bool flag = this.Snapshot.RemoveAction == null || this.Snapshot.RemoveAction.EventScriptJson.IsNullOrEmpty();
		EventScriptEditorData script;
		if (flag)
		{
			script = new EventScriptEditorData();
		}
		else
		{
			GameData.Serializer.CommonObjectSerializer.Deserialize<EventScriptEditorData>(this.Snapshot.RemoveAction.EventScriptJson, out script, GameData.Serializer.CommonObjectSerializer.MarshalFormat.Json);
		}
		EventEditorScript.Init(this.eventEditorScript);
		this.eventEditorScript.Show(script, delegate(EventScriptEditorData scriptEditorData)
		{
			AdventureMajorEventSnapshot snapshot = this.Snapshot;
			if (snapshot.RemoveAction == null)
			{
				snapshot.RemoveAction = new InstructionAdaptor();
			}
			this.Snapshot.RemoveAction.EventScriptType = 9;
			GameData.Serializer.CommonObjectSerializer.Serialize<EventScriptEditorData>(scriptEditorData, out this.Snapshot.RemoveAction.EventScriptJson, GameData.Serializer.CommonObjectSerializer.MarshalFormat.Json);
		}, 9, string.Empty, string.Empty);
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x0008E7EC File Offset: 0x0008C9EC
	public void NewNode(int type)
	{
		bool flag = type < 0 || type > 3;
		if (flag)
		{
			Debug.LogError(string.Format("type [{0}] is not a valid EAdventureMajorEventNodeType", type));
		}
		else
		{
			this.Snapshot.Nodes.Add(new AdventureMajorEventNodeSnapshot
			{
				Type = (EAdventureMajorEventNodeType)type,
				Name = "",
				Desc = "",
				EventGuid = "",
				EventTexture = "",
				Key = "",
				X = -this.dragTransform.localPosition.x / this.dragTransform.localScale.x,
				Y = -this.dragTransform.localPosition.y / this.dragTransform.localScale.y
			});
			bool flag2 = type >= 3 && this.AddNextNodePoint != null;
			if (flag2)
			{
				this.AddNextNodePoint.Data.NextNodes.Add(this.Snapshot.Nodes.Count - 1);
				List<AdventureMajorEventNodeSnapshot> nodes = this.Snapshot.Nodes;
				AdventureMajorEventNodeSnapshot adventureMajorEventNodeSnapshot = nodes[nodes.Count - 1];
				AdventureMajorEventNodeSnapshot data = this.AddNextNodePoint.Data;
				List<AdventureMajorEventNodeSnapshot> nodes2 = this.Snapshot.Nodes;
				adventureMajorEventNodeSnapshot.Type = this.GetNodeTypeByParent(data, nodes2[nodes2.Count - 1]);
			}
			this.RefreshShow();
		}
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x0008E964 File Offset: 0x0008CB64
	public void NewDecoration()
	{
		AdventureMajorEventDecorationData decorationData = new AdventureMajorEventDecorationData
		{
			Resource = string.Empty,
			X = 0f,
			Y = 0f
		};
		this.Snapshot.Decorations.Add(decorationData);
		this.SetDecorations(decorationData);
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x0008E9B8 File Offset: 0x0008CBB8
	public void BeginAddSubNodes(AdventureMajorEventEditorPoint node)
	{
		this.AddNextNodePoint = node;
		string text = LanguageKey.LK_MajorEventEditor_EditPoint_CancelAddNextNode.TrFormat(node.Data.Name, node.Data.Key);
		this.cancelAddSubNodesBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
		this.cancelAddSubNodesBtn.gameObject.SetActive(true);
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x0008EA23 File Offset: 0x0008CC23
	public void EndAddSubNodes()
	{
		this.AddNextNodePoint = null;
		this.cancelAddSubNodesBtn.gameObject.SetActive(false);
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x0008EA40 File Offset: 0x0008CC40
	private void Export()
	{
		Exception exception;
		try
		{
			AdventureEditorRuntimeImporter.ExportToContext(AdventureEditorKit.CoreRoot);
			exception = null;
		}
		catch (Exception e)
		{
			exception = e;
		}
		bool flag = exception != null;
		if (flag)
		{
			Debug.LogWarning("Export failed by exception " + exception.Message + "\nstack trace: " + exception.StackTrace);
		}
		DialogCmd cmd = new DialogCmd
		{
			Title = LanguageKey.LK_Adventure_Editor_Export_Title.Tr(),
			Content = ((exception == null) ? LanguageKey.LK_Adventure_Editor_Export_Content_Successful : LanguageKey.LK_Adventure_Editor_Export_Content_Failed).Tr(),
			Type = 2
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x0008EB04 File Offset: 0x0008CD04
	public AdventureMajorEventEditorPoint GetEditorPointByKey(string nodeKey)
	{
		for (int nodeIndex = 0; nodeIndex < this.Snapshot.Nodes.Count; nodeIndex++)
		{
			AdventureMajorEventNodeSnapshot node = this.Snapshot.Nodes[nodeIndex];
			bool flag = node.Key.Equals(nodeKey);
			if (flag)
			{
				return this.PointDict[nodeIndex];
			}
		}
		return null;
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x0008EB6C File Offset: 0x0008CD6C
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

	// Token: 0x06001747 RID: 5959 RVA: 0x0008EBCC File Offset: 0x0008CDCC
	private int GetTypeTags()
	{
		bool flag = this.Snapshot.Tags == null || this.Snapshot.Tags.Count == 0;
		int result;
		if (flag)
		{
			result = 3;
		}
		else
		{
			result = (int)this.Snapshot.Tags.First<EAdventureTag>();
		}
		return result;
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x0008EC1C File Offset: 0x0008CE1C
	private void TypeTagsValueChanged(int value)
	{
		bool flag = value > 2;
		if (flag)
		{
			this.Snapshot.Tags.Clear();
		}
		else
		{
			bool flag2 = this.Snapshot.Tags.Count == 0;
			if (flag2)
			{
				this.Snapshot.Tags.Add(EAdventureTag.MainStory);
			}
			this.Snapshot.Tags[0] = (EAdventureTag)value;
		}
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x0008EC83 File Offset: 0x0008CE83
	public void AutoNodePos()
	{
		CommonUtils.ShowConfirmDialog(LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_AutoSetPos_Title), LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_AutoSetPos_content), new Action(this.SetAutoNodePos), null, EDialogType.None);
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x0008ECB0 File Offset: 0x0008CEB0
	public void SetAutoNodePos()
	{
		List<ValueTuple<double, double>>[] fanoutOffsets = UI_AdventureMajorEventEditor.BuildFanoutOffsets((double)this.SetAutoNodePosWidth, (double)this.SetAutoNodePosHeight, (double)this.SetAutoNodePosScale);
		List<UI_AdventureMajorEventEditor.NodeLayout> nodeLayouts = new List<UI_AdventureMajorEventEditor.NodeLayout>();
		for (int i = 0; i < this.Snapshot.Nodes.Count; i++)
		{
			AdventureMajorEventNodeSnapshot node = this.Snapshot.Nodes[i];
			UI_AdventureMajorEventEditor.NodeLayout nodeLayout = new UI_AdventureMajorEventEditor.NodeLayout
			{
				Index = i,
				NextNodes = node.NextNodes,
				Type = node.Type
			};
			nodeLayouts.Add(nodeLayout);
		}
		Dictionary<int, ValueTuple<double, double>> result = UI_AdventureMajorEventEditor.LayoutNodes(nodeLayouts, (double)this.SetAutoNodePosWidth, (double)this.SetAutoNodePosHeight, fanoutOffsets);
		try
		{
			for (int nodeIndex = 0; nodeIndex < this.Snapshot.Nodes.Count; nodeIndex++)
			{
				AdventureMajorEventNodeSnapshot node2 = this.Snapshot.Nodes[nodeIndex];
				AdventureMajorEventEditorPoint refers = this.PointDict[nodeIndex];
				ValueTuple<double, double> pos = result[nodeIndex];
				node2.X = (float)pos.Item1;
				node2.Y = (float)pos.Item2;
				refers.GetComponent<RectTransform>().localPosition = new Vector3((float)pos.Item1, (float)pos.Item2, 0f);
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Node Connect wrong");
			return;
		}
		this.DestroyLinePrefab();
		this.DrawLine();
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x0008EE38 File Offset: 0x0008D038
	[return: TupleElementNames(new string[]
	{
		"dx",
		"dy"
	})]
	private static List<ValueTuple<double, double>>[] BuildFanoutOffsets(double xSpacing, double ySpacing, double scale)
	{
		double dx = Math.Round(xSpacing * scale);
		double dy = Math.Round(ySpacing * scale);
		return new List<ValueTuple<double, double>>[]
		{
			new List<ValueTuple<double, double>>
			{
				new ValueTuple<double, double>(0.0, dy)
			},
			new List<ValueTuple<double, double>>
			{
				new ValueTuple<double, double>(0.0, dy),
				new ValueTuple<double, double>(0.0, -dy)
			},
			new List<ValueTuple<double, double>>
			{
				new ValueTuple<double, double>(-dx, dy),
				new ValueTuple<double, double>(dx, dy),
				new ValueTuple<double, double>(0.0, -dy)
			},
			new List<ValueTuple<double, double>>
			{
				new ValueTuple<double, double>(-dx, dy),
				new ValueTuple<double, double>(dx, dy),
				new ValueTuple<double, double>(-dx, -dy),
				new ValueTuple<double, double>(dx, -dy)
			},
			new List<ValueTuple<double, double>>
			{
				new ValueTuple<double, double>(0.0, dy),
				new ValueTuple<double, double>(-dx, dy),
				new ValueTuple<double, double>(dx, dy),
				new ValueTuple<double, double>(-dx, -dy),
				new ValueTuple<double, double>(dx, -dy)
			},
			new List<ValueTuple<double, double>>
			{
				new ValueTuple<double, double>(0.0, -dy),
				new ValueTuple<double, double>(0.0, dy),
				new ValueTuple<double, double>(-dx, -dy),
				new ValueTuple<double, double>(dx, -dy),
				new ValueTuple<double, double>(-dx, dy),
				new ValueTuple<double, double>(dx, dy)
			}
		};
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x0008F000 File Offset: 0x0008D200
	[return: TupleElementNames(new string[]
	{
		"x",
		"y"
	})]
	public static Dictionary<int, ValueTuple<double, double>> LayoutNodes(List<UI_AdventureMajorEventEditor.NodeLayout> nodes, double xSpacing, double ySpacing, [TupleElementNames(new string[]
	{
		"dx",
		"dy"
	})] List<ValueTuple<double, double>>[] fanoutOffsets)
	{
		int i = nodes.Count;
		int startIdx = nodes.FindIndex((UI_AdventureMajorEventEditor.NodeLayout n) => n.Type == EAdventureMajorEventNodeType.Start);
		bool flag = startIdx == -1;
		if (flag)
		{
			throw new ArgumentException("No start node (Type=0).");
		}
		Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
		Dictionary<int, List<int>> reverseGraph = new Dictionary<int, List<int>>();
		for (int j = 0; j < i; j++)
		{
			graph[j] = nodes[j].NextNodes;
			reverseGraph[j] = new List<int>();
		}
		foreach (int u in graph.Keys)
		{
			foreach (int v in graph[u])
			{
				bool flag2 = v >= 0 && v < i;
				if (flag2)
				{
					reverseGraph[v].Add(u);
				}
			}
		}
		bool[] isLeaf = new bool[i];
		for (int k = 0; k < i; k++)
		{
			bool[] array = isLeaf;
			int num = k;
			int num2;
			if (nodes[k].Type == EAdventureMajorEventNodeType.Check)
			{
				List<int> nextNodes = nodes[k].NextNodes;
				num2 = ((nextNodes == null || nextNodes.Count <= 0) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
			array[num] = num2;
		}
		int[] level = Enumerable.Repeat<int>(-1, i).ToArray<int>();
		Queue<int> queue = new Queue<int>();
		level[startIdx] = 0;
		queue.Enqueue(startIdx);
		while (queue.Count > 0)
		{
			int u2 = queue.Dequeue();
			bool flag3 = isLeaf[u2];
			if (!flag3)
			{
				foreach (int v2 in graph[u2])
				{
					bool flag4 = !isLeaf[v2] && level[v2] == -1;
					if (flag4)
					{
						level[v2] = level[u2] + 1;
						queue.Enqueue(v2);
					}
				}
			}
		}
		Dictionary<int, List<int>> layers = new Dictionary<int, List<int>>();
		for (int l = 0; l < i; l++)
		{
			bool flag5 = !isLeaf[l] && level[l] != -1;
			if (flag5)
			{
				bool flag6 = !layers.ContainsKey(level[l]);
				if (flag6)
				{
					layers[level[l]] = new List<int>();
				}
				layers[level[l]].Add(l);
			}
		}
		Dictionary<int, ValueTuple<double, double>> positions = new Dictionary<int, ValueTuple<double, double>>();
		bool flag7 = layers.Count > 0;
		if (flag7)
		{
			int maxLvl = layers.Keys.Max();
			for (int lvl = 0; lvl <= maxLvl; lvl++)
			{
				List<int> indices;
				bool flag8 = layers.TryGetValue(lvl, out indices);
				if (flag8)
				{
					int count = indices.Count;
					double totalHeight = (double)(count - 1) * ySpacing;
					double startY = -totalHeight / 2.0;
					for (int idx = 0; idx < count; idx++)
					{
						double x = (double)lvl * xSpacing;
						double y = startY + (double)idx * ySpacing;
						positions[indices[idx]] = new ValueTuple<double, double>(x, y);
					}
				}
			}
		}
		Dictionary<int, List<int>> leafGroups = new Dictionary<int, List<int>>();
		int m = 0;
		while (m < i)
		{
			bool flag9 = isLeaf[m];
			if (flag9)
			{
				List<int> preds = reverseGraph[m];
				bool flag10 = preds.Count == 0;
				if (!flag10)
				{
					int parent = preds[0];
					bool flag11 = !positions.ContainsKey(parent);
					if (!flag11)
					{
						bool flag12 = !leafGroups.ContainsKey(parent);
						if (flag12)
						{
							leafGroups[parent] = new List<int>();
						}
						leafGroups[parent].Add(m);
					}
				}
			}
			IL_3FC:
			m++;
			continue;
			goto IL_3FC;
		}
		foreach (KeyValuePair<int, List<int>> kvp in leafGroups)
		{
			int parentIdx = kvp.Key;
			List<int> leaves = kvp.Value;
			int count2 = Math.Min(leaves.Count, 6);
			ValueTuple<double, double> valueTuple = positions[parentIdx];
			double px = valueTuple.Item1;
			double py = valueTuple.Item2;
			List<ValueTuple<double, double>> offsets = fanoutOffsets[count2 - 1];
			int n2 = 0;
			while (n2 < leaves.Count && n2 < offsets.Count)
			{
				ValueTuple<double, double> valueTuple2 = offsets[n2];
				double dx = valueTuple2.Item1;
				double dy = valueTuple2.Item2;
				positions[leaves[n2]] = new ValueTuple<double, double>(px + dx, py + dy);
				n2++;
			}
		}
		return positions;
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x0008F538 File Offset: 0x0008D738
	public static void SwitchActiveState(GameObject obj)
	{
		obj.SetActive(!obj.activeSelf);
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0008F5CA File Offset: 0x0008D7CA
	[CompilerGenerated]
	internal static bool <GetNodeTypeByParent>g__IsSpecialNode|50_0(EAdventureMajorEventNodeType type)
	{
		return type == EAdventureMajorEventNodeType.Start || type == EAdventureMajorEventNodeType.End || type == EAdventureMajorEventNodeType.Turn;
	}

	// Token: 0x0400129B RID: 4763
	[SerializeField]
	private AdventureMajorEventPointEditor pointEditor;

	// Token: 0x0400129C RID: 4764
	[SerializeField]
	private CToggle releasedToggle;

	// Token: 0x0400129D RID: 4765
	[SerializeField]
	private GameObject linePrefab;

	// Token: 0x0400129E RID: 4766
	[SerializeField]
	private RectTransform lineRoot;

	// Token: 0x0400129F RID: 4767
	[SerializeField]
	private RectTransform nodeRoot;

	// Token: 0x040012A0 RID: 4768
	[SerializeField]
	private AdventureMajorEventEditorPoint pointPrefab;

	// Token: 0x040012A1 RID: 4769
	[SerializeField]
	private GameObject shortLinePrefab;

	// Token: 0x040012A2 RID: 4770
	[SerializeField]
	private AdventureCostDataEditor adventureCostDataEditor;

	// Token: 0x040012A3 RID: 4771
	[SerializeField]
	private TemplatedContainerAssemblyNew charHolder;

	// Token: 0x040012A4 RID: 4772
	[SerializeField]
	private TemplatedContainerAssemblyNew paramHolder;

	// Token: 0x040012A5 RID: 4773
	[SerializeField]
	private AdventureMajorEventEditorDecorationPrefab decorationPrefab;

	// Token: 0x040012A6 RID: 4774
	[SerializeField]
	private RectTransform decorationRoot;

	// Token: 0x040012A7 RID: 4775
	[SerializeField]
	private CButton setAutoNodePos;

	// Token: 0x040012A8 RID: 4776
	[SerializeField]
	private EventEditorScript eventEditorScript;

	// Token: 0x040012A9 RID: 4777
	internal AdventureMajorEventSnapshot Snapshot = new AdventureMajorEventSnapshot
	{
		Cost = new AdventureCostData()
	};

	// Token: 0x040012AA RID: 4778
	internal readonly Dictionary<int, AdventureMajorEventEditorPoint> PointDict = new Dictionary<int, AdventureMajorEventEditorPoint>();

	// Token: 0x040012AB RID: 4779
	[TupleElementNames(new string[]
	{
		"startNodeIndex",
		"endNodeIndex"
	})]
	private readonly List<ValueTuple<int, int>> _lineData = new List<ValueTuple<int, int>>();

	// Token: 0x040012AC RID: 4780
	private readonly List<RectTransform> _lineRefersList = new List<RectTransform>();

	// Token: 0x040012AD RID: 4781
	private const string PointPrefabKey = "UI_AdventureMajorEventEditorPointPrefab";

	// Token: 0x040012AE RID: 4782
	private const string LinePrefabKey = "UI_AdventureMajorEventEditorLinePrefab";

	// Token: 0x040012AF RID: 4783
	[Obsolete]
	private const string ShortLinePrefabKey = "UI_AdventureMajorEventEditorShortLinePrefab";

	// Token: 0x040012B0 RID: 4784
	private const string DecorationPrefabKey = "UI_AdventureMajorEventEditorDecorationPrefab";

	// Token: 0x040012B1 RID: 4785
	internal const string RewardGroupEditorPrefabKey = "UI_AdventureMajorEventEditorRewardGroupEditorPrefabKeyPrefab";

	// Token: 0x040012B2 RID: 4786
	[SerializeField]
	private RewardGroupEditor rewardGroupEditorPrefab;

	// Token: 0x040012B3 RID: 4787
	internal const string LifeSkillEditorPrefabKey = "UI_AdventureMajorEventEditorLifeSkillEditorPrefabKeyPrefab";

	// Token: 0x040012B4 RID: 4788
	[SerializeField]
	private ResourceEditor lifeSkillEditorPrefab;

	// Token: 0x040012B5 RID: 4789
	internal const string CombatSkillEditorPrefabKey = "UI_AdventureMajorEventEditorCombatSkillEditorPrefabKeyPrefab";

	// Token: 0x040012B6 RID: 4790
	[SerializeField]
	private ResourceEditor combatSkillEditorPrefab;

	// Token: 0x040012B7 RID: 4791
	internal const string PersonalityEditorPrefabKey = "UI_AdventureMajorEventEditorPersonalityEditorPrefabKeyPrefab";

	// Token: 0x040012B8 RID: 4792
	[SerializeField]
	private ResourceEditor personalityEditorPrefab;

	// Token: 0x040012B9 RID: 4793
	internal const string ItemEditorPrefabKey = "UI_AdventureMajorEventEditorItemEditorPrefabKeyPrefab";

	// Token: 0x040012BA RID: 4794
	[SerializeField]
	private ItemEditor itemEditorPrefab;

	// Token: 0x040012BB RID: 4795
	private int _addNextNodePointKey = -1;

	// Token: 0x040012BC RID: 4796
	private readonly StringBuilder _sb = new StringBuilder();

	// Token: 0x040012BD RID: 4797
	[SerializeField]
	private CDropdownLegacy itemOptions;

	// Token: 0x040012BE RID: 4798
	[SerializeField]
	private CDropdownLegacy resourceOptions;

	// Token: 0x040012BF RID: 4799
	private string _editingPath;

	// Token: 0x040012C0 RID: 4800
	[SerializeField]
	private RectTransform dragTransform;

	// Token: 0x040012C1 RID: 4801
	[SerializeField]
	private TMP_InputField keyPrefix;

	// Token: 0x040012C2 RID: 4802
	[SerializeField]
	private TMP_InputField keyNumber;

	// Token: 0x040012C3 RID: 4803
	[SerializeField]
	private TMP_InputField definition;

	// Token: 0x040012C4 RID: 4804
	[SerializeField]
	private new TMP_InputField name;

	// Token: 0x040012C5 RID: 4805
	[SerializeField]
	private TMP_InputField desc;

	// Token: 0x040012C6 RID: 4806
	[SerializeField]
	private TMP_InputField stayMonths;

	// Token: 0x040012C7 RID: 4807
	[SerializeField]
	private CRawImage backgroundImg;

	// Token: 0x040012C8 RID: 4808
	[SerializeField]
	private TMP_InputField backgroundImgName;

	// Token: 0x040012C9 RID: 4809
	[SerializeField]
	private TMP_InputField eventTexture;

	// Token: 0x040012CA RID: 4810
	[SerializeField]
	private CDropdown typeTagDropdown;

	// Token: 0x040012CB RID: 4811
	[SerializeField]
	private float SetAutoNodePosWidth = 600f;

	// Token: 0x040012CC RID: 4812
	[SerializeField]
	private float SetAutoNodePosHeight = 400f;

	// Token: 0x040012CD RID: 4813
	[SerializeField]
	private float SetAutoNodePosScale = 0.6f;

	// Token: 0x040012CE RID: 4814
	[SerializeField]
	private MajorEventCharacterTemplate charTemplate;

	// Token: 0x040012CF RID: 4815
	[SerializeField]
	private MajorEventParameterTemplate paramTemplate;

	// Token: 0x040012D0 RID: 4816
	[SerializeField]
	private CButton cancelAddSubNodesBtn;

	// Token: 0x020012E0 RID: 4832
	public struct NodeLayout
	{
		// Token: 0x17001634 RID: 5684
		// (get) Token: 0x0600C74B RID: 51019 RVA: 0x00584A3A File Offset: 0x00582C3A
		// (set) Token: 0x0600C74C RID: 51020 RVA: 0x00584A42 File Offset: 0x00582C42
		public int Index { readonly get; set; }

		// Token: 0x17001635 RID: 5685
		// (get) Token: 0x0600C74D RID: 51021 RVA: 0x00584A4B File Offset: 0x00582C4B
		// (set) Token: 0x0600C74E RID: 51022 RVA: 0x00584A53 File Offset: 0x00582C53
		public List<int> NextNodes { readonly get; set; }

		// Token: 0x17001636 RID: 5686
		// (get) Token: 0x0600C74F RID: 51023 RVA: 0x00584A5C File Offset: 0x00582C5C
		// (set) Token: 0x0600C750 RID: 51024 RVA: 0x00584A64 File Offset: 0x00582C64
		public EAdventureMajorEventNodeType Type { readonly get; set; }
	}
}
