using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Config;
using FrameWork;
using Game.Views.Adventure;
using Game.Views.CharacterMenu;
using GameData.Adventure;
using GameData.Common;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Merchant;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GM;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000213 RID: 531
public class UI_GMWindow : MonoBehaviour
{
	// Token: 0x17000360 RID: 864
	// (get) Token: 0x060021C2 RID: 8642 RVA: 0x000F6CBE File Offset: 0x000F4EBE
	// (set) Token: 0x060021C3 RID: 8643 RVA: 0x000F6CD0 File Offset: 0x000F4ED0
	private bool Opened
	{
		get
		{
			return this.WindowRoot.gameObject.activeSelf;
		}
		set
		{
			this.WindowRoot.gameObject.SetActive(value);
			this.SetCurPage(this._curPage);
			CommandKitBase.SetDisable(value);
		}
	}

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x060021C4 RID: 8644 RVA: 0x000F6CF9 File Offset: 0x000F4EF9
	// (set) Token: 0x060021C5 RID: 8645 RVA: 0x000F6D0C File Offset: 0x000F4F0C
	private bool ConsoleOpened
	{
		get
		{
			return this.ConsoleRoot.gameObject.activeSelf;
		}
		set
		{
			this.ConsoleRoot.gameObject.SetActive(value);
			if (value)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, new Action(this.RefreshConsoleContent));
			}
			this.ConsoleOpenedTip.text = (value ? ">" : "<");
		}
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x000F6D66 File Offset: 0x000F4F66
	public void OnDestroy()
	{
		this.OnLeaveWorld();
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x000F6D70 File Offset: 0x000F4F70
	public int? GetDataListenerId()
	{
		return this._listenerId;
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x000F6D88 File Offset: 0x000F4F88
	public bool IsGameDataReceiving()
	{
		return this._gameDataRecevier != null;
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000F6DA3 File Offset: 0x000F4FA3
	public void StopGameDataReceiving()
	{
		this._gameDataRecevier = null;
		this._gameDataReceivingList.Clear();
		this._gameDataReceivingTypes.Clear();
		this._gameDataReceivingStorage.Clear();
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000F6DD4 File Offset: 0x000F4FD4
	public void RequestGameDataReceiving(Action<Dictionary<DataUid, object>> callback, params ValueTuple<DataUid, Type>[] dataIds)
	{
		bool flag = this._listenerId == null;
		if (!flag)
		{
			this.StopGameDataReceiving();
			this._gameDataRecevier = callback;
			this._gameDataReceivingList.AddRange(from a in dataIds
			select a.Item1);
			this._gameDataReceivingTypes.AddRange(from a in dataIds
			select a.Item2);
			foreach (DataUid uid in this._gameDataReceivingList)
			{
				GameDataBridge.AddDataMonitor(this._listenerId.Value, uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1);
			}
		}
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000F6ED8 File Offset: 0x000F50D8
	public static void EnsureInstanceExist(bool isEnabled)
	{
		bool flag = !isEnabled;
		if (!flag)
		{
			bool flag2 = UI_GMWindow.Instance == null && !string.IsNullOrEmpty("RemakeResources/Prefab/Views/Legacy/Test/UI_GMWindow");
			if (flag2)
			{
				ResLoader.Load<GameObject>("RemakeResources/Prefab/Views/Legacy/Test/UI_GMWindow", delegate(GameObject gmPrefab)
				{
					GameObject instance = Object.Instantiate<GameObject>(gmPrefab);
					instance.GetComponent<UI_GMWindow>().OnWorldDataReady();
				}, delegate(string path)
				{
					Debug.LogWarning("Load failed - GM Prefab");
				}, false);
			}
		}
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000F6F60 File Offset: 0x000F5160
	public void Awake()
	{
		UI_GMWindow.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		this._AllowSetValue = false;
		this.Opened = false;
		this.ConsoleOpened = false;
		this.WidgetTemplateRoot.gameObject.SetActive(false);
		this.GMObjRoot.gameObject.SetActive(false);
		this.SelectCharRoot.gameObject.SetActive(false);
		GMCharacterEditor editor = this.CharacterEditor.GetComponent<GMCharacterEditor>();
		this.OnWorldDataReadyChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(this.OnWorldDataReadyChild, new UI_GMWindow.WorldStateCallback(editor.OnWorldDataReady));
		this.OnLeaveWorldChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(this.OnLeaveWorldChild, new UI_GMWindow.WorldStateCallback(editor.OnLeaveWorld));
		bool flag = this.CheckAvatarPanel != null;
		if (flag)
		{
			this.OnWorldDataReadyChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(this.OnWorldDataReadyChild, new UI_GMWindow.WorldStateCallback(this.CheckAvatarPanel.OnWorldDataReady));
			this.OnLeaveWorldChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(this.OnLeaveWorldChild, new UI_GMWindow.WorldStateCallback(this.CheckAvatarPanel.OnLeaveWorld));
		}
		this.CommandLine.OnGmWindowAwake(this);
		this.SearchHelper.Init(this.PageToggleRoot, this.CharacterEditor);
		base.StartCoroutine(this.WaitForGameResReady());
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000F70B1 File Offset: 0x000F52B1
	private IEnumerator WaitForGameResReady()
	{
		WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
		base.transform.SetParent(UIManager.Instance.transform.Find("Canvas/LayerTips"), false);
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;
		base.gameObject.AddComponent<GraphicRaycaster>();
		while (!GameApp.GameResReady)
		{
			yield return waitFrame;
		}
		this.LoadLayout();
		this.CommandLine.Prepare(this._pages);
		yield break;
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000F70C0 File Offset: 0x000F52C0
	public void OnWorldDataReady()
	{
		this.OnLeaveWorld();
		this._AllowSetValue = true;
		this._listenerId = new int?(GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData)));
		bool flag = GMFunc.AdvanceMonthCoroutine != null;
		if (flag)
		{
			SingletonObject.getInstance<YieldHelper>().StopCoroutine(GMFunc.AdvanceMonthCoroutine);
			GMFunc.AdvanceMonthCoroutine = null;
		}
		this.OnWorldDataReadyChild();
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000F7128 File Offset: 0x000F5328
	public void OnLeaveWorld()
	{
		this.StopGameDataReceiving();
		bool flag = this._listenerId != null;
		if (flag)
		{
			GameDataBridge.UnregisterListener(this._listenerId.Value);
			this._listenerId = null;
		}
		bool flag2 = GMFunc.AdvanceMonthCoroutine != null;
		if (flag2)
		{
			SingletonObject.getInstance<YieldHelper>().StopCoroutine(GMFunc.AdvanceMonthCoroutine);
			GMFunc.AdvanceMonthCoroutine = null;
		}
		this.OnLeaveWorldChild();
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000F719C File Offset: 0x000F539C
	private void LoadLayout()
	{
		UI_GMWindow.<>c__DisplayClass75_0 CS$<>8__locals1 = new UI_GMWindow.<>c__DisplayClass75_0();
		CS$<>8__locals1.<>4__this = this;
		this._pages = new List<MemberInfo>[10];
		this._pageToggles = new Transform[10];
		for (int i = 0; i < this._pages.Length; i++)
		{
			this._pages[i] = new List<MemberInfo>();
			GameObject page = Object.Instantiate<GameObject>(this.PageToggle, this.PageToggleRoot.transform, true);
			page.transform.localScale = Vector3.one;
			page.transform.localPosition.SetZ(0f);
			string toggleLabelText = LocalStringManager.Get(string.Format("GM_PageName_{0}", i));
			page.GetComponentInChildren<TextMeshProUGUI>().text = toggleLabelText;
			this._pageToggles[i] = page.transform;
			this.SearchHelper.RegisterPageToggle(i, page, toggleLabelText);
		}
		Type[] types = typeof(UI_GMWindow).Assembly.GetTypes();
		CS$<>8__locals1.memberName = new List<string>();
		foreach (Type type in types)
		{
			CS$<>8__locals1.<LoadLayout>g__LoadType|1(type);
		}
		foreach (Type type2 in UI_GMWindow.GMFuncTypes)
		{
			CS$<>8__locals1.<LoadLayout>g__LoadType|1(type2);
		}
		this.BuildPages();
		this.PageToggleRoot.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete _)
		{
			bool flag = togNew != null;
			if (flag)
			{
				CS$<>8__locals1.<>4__this.SetCurPage(togNew.Key);
			}
		};
		this.PageToggleRoot.AddAllChildToggles();
		this.PageToggleRoot.Set(0, true, false);
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000F7350 File Offset: 0x000F5550
	public void Reset()
	{
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x000F7354 File Offset: 0x000F5554
	private void BuildPages()
	{
		foreach (object obj in this.PageRoot)
		{
			Transform page = (Transform)obj;
			Object.Destroy(page.gameObject);
		}
		for (int i = 0; i < this._pages.Length; i++)
		{
			GameObject page2 = Object.Instantiate<GameObject>(this.Page);
			UI_GMWindow.SetParent(page2.transform, this.PageRoot);
			List<IGrouping<EGMGroup, MemberInfo>> groups = (from a in this._pages[i]
			group a by a.GetCustomAttribute<GMMemberAttribute>().Group).ToList<IGrouping<EGMGroup, MemberInfo>>();
			groups.Sort((IGrouping<EGMGroup, MemberInfo> a, IGrouping<EGMGroup, MemberInfo> b) => a.Key.CompareTo(b.Key));
			foreach (IGrouping<EGMGroup, MemberInfo> one in groups)
			{
				GameObject groupObj = Object.Instantiate<GameObject>(this.Group);
				this.SearchHelper.RegisterGroup(groupObj.transform);
				groupObj.name = string.Format("group_{0}_{1}", i, one.Key.ToString());
				UI_GMWindow.SetParent(groupObj.transform, page2.transform);
				List<MemberInfo> group = one.ToList<MemberInfo>();
				group.Sort((MemberInfo a, MemberInfo b) => a.GetCustomAttribute<GMMemberAttribute>().Priority.CompareTo(b.GetCustomAttribute<GMMemberAttribute>().Priority));
				foreach (MemberInfo member in group)
				{
					Transform parent = groupObj.transform;
					bool flag = member is MethodInfo;
					GameObject gMLine;
					if (flag)
					{
						GMFuncAttribute info = member.GetCustomAttribute<GMFuncAttribute>();
						bool flag2 = info != null;
						if (flag2)
						{
							MethodInfo methodInfo = member as MethodInfo;
							gMLine = this.GetGMFunc(methodInfo, i, this._pages[i].IndexOf(member), 1644, info.RunMode);
							bool flag3 = GMCharacterEditor.IsCharacterEditorFunc(methodInfo);
							if (flag3)
							{
								parent = this.CharacterEditor.List;
							}
						}
						else
						{
							gMLine = this.GetGMObject(member as MethodInfo, i, this._pages[i].IndexOf(member));
						}
					}
					else
					{
						gMLine = this.GetGMProperty(member as PropertyInfo, i, this._pages[i].IndexOf(member));
					}
					this._member2GO.Add(member, gMLine);
					UI_GMWindow.SetParent(gMLine.transform, parent);
				}
			}
		}
		this.SelectCharRoot.Find("MainWindow/Name/TipBack/Tip").GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get("GM_Name");
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000F769C File Offset: 0x000F589C
	private GameObject GetGMFunc(MethodInfo methodInfo, int pageIdx, int memberIdx, int marianAndBillyCount, GmRunMode runMode)
	{
		GameObject line = Object.Instantiate<GameObject>(this.Line);
		ParameterInfo[] parameters = methodInfo.GetParameters();
		bool isCharacterFunc = GMCharacterEditor.IsCharacterEditorFunc(methodInfo);
		string buttonText = this.GetMemberName(methodInfo.Name);
		bool isCharacterFunc2 = isCharacterFunc;
		if (isCharacterFunc2)
		{
			this.SearchHelper.RegisterCharacterEditorLine(pageIdx, line, buttonText + methodInfo.Name);
		}
		else
		{
			this.SearchHelper.RegisterLine(pageIdx, line, buttonText + methodInfo.Name);
		}
		line.name = string.Format("func_{0}_{1}", pageIdx, buttonText);
		GameObject btn = this.GetBtn(new UI_GMWindow.WidgetParams
		{
			Comment = buttonText,
			Width = methodInfo.GetCustomAttribute<GMMemberAttribute>().Width,
			OnClick = delegate()
			{
				bool flag6 = WorldMapModel.Traveling && !SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure;
				if (flag6)
				{
					this.Log(LocalStringManager.Get("GM_Message_CallFailedSpecial"));
				}
				else
				{
					bool flag7 = this.IsGameDataReceiving();
					if (!flag7)
					{
						object[] args2 = this.GetParams(pageIdx, memberIdx, parameters.Length);
						bool flag8 = isCharacterFunc && args2.Length != 0;
						if (flag8)
						{
							args2[0] = this.CharacterEditor.GetCharacterId();
						}
						string argStr = (args2.Length == 0) ? "" : (", " + LocalStringManager.Get("GM_Message_Args") + " :");
						int i = 0;
						int cnt = args2.Length;
						while (i < cnt)
						{
							argStr += ((i == 0) ? string.Format(" {0} ", args2[i]) : string.Format(", {0} ", args2[i]));
							i++;
						}
						this.Log(LocalStringManager.GetFormat(LanguageKey.GM_Message_Call, methodInfo.Name, argStr).SetColor(Color.yellow));
						GmRunMode runMode2 = runMode;
						GmRunMode gmRunMode = runMode2;
						if (gmRunMode != GmRunMode.Default)
						{
							if (gmRunMode == GmRunMode.NoTry)
							{
								methodInfo.Invoke(null, args2);
								this.CommandLine.GenerateConsoleHistoryFromUi(methodInfo, args2);
							}
						}
						else
						{
							try
							{
								methodInfo.Invoke(null, args2);
								this.CommandLine.GenerateConsoleHistoryFromUi(methodInfo, args2);
							}
							catch (Exception e)
							{
								this.Log(LocalStringManager.GetFormat(LanguageKey.GM_Message_CallFailed, methodInfo.Name, e).SetColor(Color.red));
							}
						}
					}
				}
			}
		});
		UI_GMWindow.SetParent(btn.transform, line.transform);
		string tip = this.GetMemberTips(methodInfo.Name);
		bool flag = tip != null;
		if (flag)
		{
			TooltipInvoker displayer = btn.AddComponent<TooltipInvoker>();
			displayer.PresetParam = new string[]
			{
				tip
			};
		}
		this.AppendGmCommandTips(btn, methodInfo);
		bool flag2 = parameters.Length != 0;
		if (flag2)
		{
			GMFuncArgAttribute[] args = new GMFuncArgAttribute[parameters.Length];
			foreach (GMFuncArgAttribute one in methodInfo.GetCustomAttributes<GMFuncArgAttribute>())
			{
				args[one.Index] = one;
			}
			for (int argIdx = 0; argIdx < parameters.Length; argIdx++)
			{
				ParameterInfo parameter = parameters[argIdx];
				GMFuncArgAttribute argInfo = args[argIdx] ?? new GMFuncArgAttribute(0, this.GetDefaultWidgetType(parameter.ParameterType), 0.1f);
				int key = this.GetArgKey(pageIdx, memberIdx, argIdx);
				this._argIsNullable[key] = (parameter.ParameterType.Name == "Nullable`1");
				UI_GMWindow.WidgetParams widgetParams = new UI_GMWindow.WidgetParams
				{
					Width = argInfo.Width,
					Comment = this.GetArgName(methodInfo.Name, argIdx, parameter.Name),
					Set2Arg = true
				};
				bool hasDefaultValue = parameter.HasDefaultValue;
				if (hasDefaultValue)
				{
					widgetParams.DefaultValue = parameter.DefaultValue;
					this.SetValue(key, parameter.DefaultValue);
				}
				GameObject widget = this.GetWidget(argInfo.WidgetType, widgetParams, key, parameters[argIdx].ParameterType);
				bool flag3 = argIdx == 0 & isCharacterFunc;
				if (flag3)
				{
					this.SetValue(key, -1);
					widget.SetActive(false);
				}
				UI_GMWindow.SetParent(widget.transform, line.transform);
				tip = this.GetArgTips(methodInfo.Name, argIdx);
				bool flag4 = tip == null;
				if (flag4)
				{
					EWidgetType widgetType = argInfo.WidgetType;
					EWidgetType ewidgetType = widgetType;
					if (ewidgetType == EWidgetType.CharIdField)
					{
						tip = LocalStringManager.Get("GM_Message_Args_CharId_Tips");
					}
				}
				bool flag5 = tip != null;
				if (flag5)
				{
					TooltipInvoker displayer2 = widget.AddComponent<TooltipInvoker>();
					displayer2.PresetParam = new string[]
					{
						tip
					};
				}
			}
		}
		return line;
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000F7A60 File Offset: 0x000F5C60
	private void AppendGmCommandTips(GameObject btn, MethodInfo methodInfo)
	{
		string originTipsText = null;
		TooltipInvoker tipDisplayer = btn.GetComponent<TooltipInvoker>();
		bool flag = tipDisplayer != null;
		if (flag)
		{
			bool flag2 = tipDisplayer.PresetParam != null;
			if (flag2)
			{
				originTipsText = tipDisplayer.PresetParam[0];
			}
			else
			{
				bool flag3 = tipDisplayer.RuntimeParam != null;
				if (flag3)
				{
					string runtimeTips;
					tipDisplayer.RuntimeParam.Get("arg0", out runtimeTips);
					originTipsText = runtimeTips;
				}
			}
		}
		string gmHelpString = GMCommandLine.GenerateGmHelpString(methodInfo);
		bool flag4 = tipDisplayer == null;
		if (flag4)
		{
			tipDisplayer = btn.AddComponent<TooltipInvoker>();
		}
		string tips = originTipsText + "\n" + gmHelpString;
		bool flag5 = tipDisplayer.PresetParam != null;
		if (flag5)
		{
			tipDisplayer.PresetParam[0] = tips;
		}
		else
		{
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Set("arg0", tips);
		}
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000F7B40 File Offset: 0x000F5D40
	private GameObject GetGMObject(MethodInfo methodInfo, int pageIdx, int memberIdx)
	{
		return methodInfo.Invoke(null, new object[0]) as GameObject;
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000F7B64 File Offset: 0x000F5D64
	private GameObject GetGMProperty(PropertyInfo propertyInfo, int pageIdx, int memberIdx)
	{
		GameObject line = Object.Instantiate<GameObject>(this.Line);
		string propertyText = this.GetMemberName(propertyInfo.Name);
		GameObject label = this.GetLabel(new UI_GMWindow.WidgetParams
		{
			Comment = propertyText,
			Width = propertyInfo.GetCustomAttribute<GMMemberAttribute>().Width
		});
		line.name = string.Format("prop_{0}_{1}", pageIdx, propertyText);
		this.SearchHelper.RegisterLine(pageIdx, line, propertyText + propertyInfo.Name);
		UI_GMWindow.SetParent(label.transform, line.transform);
		string tip = this.GetMemberTips(propertyInfo.Name);
		bool flag = tip != null;
		if (flag)
		{
			TooltipInvoker displayer = label.AddComponent<TooltipInvoker>();
			displayer.PresetParam = new string[]
			{
				tip
			};
		}
		EWidgetType widgetType = propertyInfo.GetCustomAttribute<GMPropertyAttribute>().WidgetType;
		bool flag2 = widgetType == EWidgetType.Auto;
		if (flag2)
		{
			widgetType = this.GetDefaultWidgetType(propertyInfo.PropertyType);
		}
		int key = this.GetArgKey(pageIdx, memberIdx, 0);
		this._key2Property.Add(key, propertyInfo);
		GameObject widget = this.GetWidget(widgetType, new UI_GMWindow.WidgetParams
		{
			Width = propertyInfo.GetCustomAttribute<GMPropertyAttribute>().ValueWidth,
			Set2Arg = false
		}, key, propertyInfo.PropertyType);
		UI_GMWindow.SetParent(widget.transform, line.transform);
		return line;
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000F7CB0 File Offset: 0x000F5EB0
	private void Update()
	{
		bool flag = HotKeyCommandSpecial.EditorCheckKeyGM();
		if (flag)
		{
			this.SwitchWindow();
		}
		this.SelfRoot.gameObject.SetActive(this.ValidGMWindow());
		bool flag2 = this.SelfRoot.gameObject.activeSelf && this.Opened;
		if (flag2)
		{
			this.RefreshCurPage();
		}
		bool flag3 = this._pageToggles != null;
		if (flag3)
		{
			foreach (Transform button in this._pageToggles)
			{
				button.localPosition.SetZ(0f);
			}
		}
		this.CommandLine.OnGmWindowUpdate();
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000F7D60 File Offset: 0x000F5F60
	public void RefreshPage(int page)
	{
		float nowTime = Time.realtimeSinceStartup;
		bool needRefresh = nowTime - this._lastRefreshPropertyTime > 0.5f;
		bool flag = needRefresh;
		if (flag)
		{
			this._lastRefreshPropertyTime = nowTime;
		}
		int i = 0;
		while (i < this._pages[page].Count)
		{
			MemberInfo member = this._pages[page][i];
			bool flag2 = member is MethodInfo;
			if (flag2)
			{
				bool flag3 = member.GetCustomAttribute<GMFuncAttribute>() != null;
				if (flag3)
				{
					this._member2GO[member].transform.GetChild(0).GetComponent<CButtonObsolete>().interactable = this.ParamsSetted(page, i, (member as MethodInfo).GetParameters().Length);
				}
			}
			else
			{
				bool flag4 = member is PropertyInfo;
				if (flag4)
				{
					bool flag5 = !needRefresh;
					if (!flag5)
					{
						Transform obj = this._member2GO[member].transform.GetChild(1);
						int key = this.GetArgKey(page, i, 0);
						object value = (member as PropertyInfo).GetValue(null);
						bool flag6 = this._dicParams.ContainsKey(key) && this._dicParams[key].Equals(value);
						if (!flag6)
						{
							bool flag7 = obj.GetComponentInChildren<TMP_InputField>() && !obj.GetComponentInChildren<TMP_InputField>().isFocused;
							if (flag7)
							{
								obj.GetComponentInChildren<TMP_InputField>().text = value.ToString();
							}
							bool flag8 = obj.GetComponentInChildren<CToggleObsolete>();
							if (flag8)
							{
								obj.GetComponentInChildren<CToggleObsolete>().isOn = (bool)value;
							}
							this.SetValue(key, value);
						}
					}
				}
			}
			IL_18D:
			i++;
			continue;
			goto IL_18D;
		}
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000F7F17 File Offset: 0x000F6117
	private void RefreshCurPage()
	{
		this.RefreshPage(this._curPage);
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x000F7F28 File Offset: 0x000F6128
	private GameObject GetWidget(EWidgetType widgetType, UI_GMWindow.WidgetParams widgetParams, int key, Type argType = null)
	{
		switch (widgetType)
		{
		case EWidgetType.IntField:
			return this.GetValueField(widgetParams, key, argType, UI_GMWindow.EValueType.Integer);
		case EWidgetType.FloatField:
			return this.GetValueField(widgetParams, key, argType, UI_GMWindow.EValueType.Float);
		case EWidgetType.StringField:
			return this.GetValueField(widgetParams, key, argType, UI_GMWindow.EValueType.String);
		case EWidgetType.BoolField:
			return this.GetToggle(widgetParams, key);
		case EWidgetType.CharIdField:
			return this.GetCharIdField(widgetParams, key);
		case EWidgetType.InformationTypeIdField:
			return this.GetInformationTypeIdField(widgetParams, key);
		case EWidgetType.ItemTypeIdField:
			return this.GetItemTypeIdField(widgetParams, key);
		case EWidgetType.ItemIdField:
			return this.GetItemIdField(widgetParams, key);
		case EWidgetType.ItemSelectorExField:
			return this.GetItemSelectorExField(widgetParams, key);
		case EWidgetType.XiangshuAvatarIdField:
			return this.GetXiangshuAvatarIdField(widgetParams, key);
		case EWidgetType.TwelveImmortalsIndexField:
			return this.GetTwelveImmortalsIndexField(widgetParams, key);
		case EWidgetType.BossCharIdField:
			return this.GetBossCharIdField(widgetParams, key);
		case EWidgetType.AnimalCharIdField:
			return this.GetAnimalCharIdField(widgetParams, key);
		case EWidgetType.OrgMemberCharIdField:
			return this.GetOrgMemberCharIdField(widgetParams, key);
		case EWidgetType.RandomEnemyCharIdField:
			return this.GetRandomEnemyCharIdField(widgetParams, key);
		case EWidgetType.CombatResultTypeField:
			return this.GetCombatResultTypeField(widgetParams, key);
		case EWidgetType.LegacyTemplateIdField:
			return this.GetLegacyTemplateIdField(widgetParams, key);
		case EWidgetType.LegacyPointTemplateIdField:
			return this.GetLegacyPointTemplateIdField(widgetParams, key);
		case EWidgetType.LifeSkillCombatResultTypeField:
			return this.GetLifeSkillCombatResultTypeField(widgetParams, key);
		case EWidgetType.LifeSkillCombatTalkIdField:
			return this.GetLifeSkillCombatTalkIdField(widgetParams, key);
		case EWidgetType.LifeSkillCombatStrategyIdField:
			return this.GetLifeSkillCombatStrategyIdField(widgetParams, key);
		case EWidgetType.LifeSkillCombatNodeEffectIdField:
			return this.GetLifeSkillCombatNodeEffectIdField(widgetParams, key);
		case EWidgetType.StateIdField:
			return this.GetStateIdField(widgetParams, key);
		case EWidgetType.StoryProgressField:
			return this.GetStoryProgressField(widgetParams, key);
		case EWidgetType.AdventureIdField:
			return this.GetAdventureIdField(widgetParams, key);
		case EWidgetType.AdventureRemakeIdField:
			return this.GetAdventureRemakeIdField(widgetParams, key);
		case EWidgetType.AdventureMajorEventIdField:
			return this.GetAdventureMajorEventIdField(widgetParams, key);
		case EWidgetType.BattleSceneIdField:
			return this.GetBattleSceneIdField(widgetParams, key);
		case EWidgetType.LifeSkillIdField:
			return this.GetLifeSkillIdField(widgetParams, key);
		case EWidgetType.LifeSkillTypeField:
			return this.GetLifeSkillTypeField(widgetParams, key);
		case EWidgetType.CombatSkillIdField:
			return this.GetCombatSkillIdField(widgetParams, key);
		case EWidgetType.CombatSkillTypeField:
			return this.GetCombatSkillTypeField(widgetParams, key);
		case EWidgetType.CombatTypeField:
			return this.GetCombatTypeField(widgetParams, key);
		case EWidgetType.CombatConfigField:
			return this.GetCombatConfigField(widgetParams, key);
		case EWidgetType.CharacterRelationshipTypeField:
			return this.GetCharacterRelationshipTypeField(widgetParams, key);
		case EWidgetType.ProfessionIdField:
			return this.GetProfessionField(widgetParams, key);
		case EWidgetType.ProfessionSkillIdField:
			return this.GetProfessionSkillIdField(widgetParams, key);
		case EWidgetType.TaskInfoIdField:
			return this.GetTaskInfoIdField(widgetParams, key);
		case EWidgetType.TeaHorseCaravanEventIdField:
			return this.GetTeaHorseCaravanEventIdField(widgetParams, key);
		case EWidgetType.SectIdField:
			return this.GetSectIdField(widgetParams, key);
		case EWidgetType.PoisonItemIdField:
			return this.GetPoisonItemIdField(widgetParams, key);
		case EWidgetType.SkillBreakPlateGridBonusTypeIdField:
			return this.GetSkillBreakPlateGridBonusTypeIdField(widgetParams, key);
		case EWidgetType.JiaoField:
			return this.GetJiaoTemplateId(widgetParams, key);
		case EWidgetType.ChildrenOfLoongField:
			return this.GetChildrenOfLoongTemplateId(widgetParams, key);
		case EWidgetType.CaravanStateField:
			return this.GetCaravanState(widgetParams, key);
		case EWidgetType.WorldCreationInfoTypeField:
			return this.GetWorldCreationType(widgetParams, key);
		case EWidgetType.VillagerRoleField:
			return this.GetVillagerRoleTemplateId(widgetParams, key);
		case EWidgetType.MapBlockIdField:
			return this.GetMapBlockTemplateId(widgetParams, key);
		case EWidgetType.BuildingBlockTemplateIdCollectField:
			return this.GetConfigurationTemplateId<BuildingBlockItem, short>(widgetParams, key, BuildingBlock.Instance.GetAllKeys(), (short id) => BuildingBlock.Instance[id], (BuildingBlockItem c) => c.Name, (BuildingBlockItem c) => c.Class == EBuildingBlockClass.Resource && c.Type == EBuildingBlockType.Building);
		case EWidgetType.TravelEventIdField:
			return this.GetConfigurationTemplateId<TravelingEventItem, short>(widgetParams, key, TravelingEvent.Instance.GetAllKeys(), (short id) => TravelingEvent.Instance[id], (TravelingEventItem c) => c.Name, (TravelingEventItem _) => true);
		case EWidgetType.LanguageTypeField:
			return this.GetEnumValue<LocalStringManager.LanguageType>(widgetParams, key, (LocalStringManager.LanguageType eV) => eV.ToString(), (LocalStringManager.LanguageType _) => true);
		case EWidgetType.CombatWeatherField:
			return this.GetCombatWeatherField(widgetParams, key);
		case EWidgetType.MerchantTypeField:
			return this.GetMerchantTypeFieldField(widgetParams, key);
		}
		throw new Exception(string.Format("Error WidgetType:{0}", widgetType));
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x000F842C File Offset: 0x000F662C
	private GameObject GetCombatWeatherField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject widget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = widget.GetComponentInChildren<TMP_Dropdown>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(widget, widgetParams.Width);
		input.options.Clear();
		ValueTuple<string, string>[] options = new ValueTuple<string, string>[]
		{
			new ValueTuple<string, string>("无", ""),
			new ValueTuple<string, string>("随机", "__random__"),
			new ValueTuple<string, string>("小雨", "xiaoyu"),
			new ValueTuple<string, string>("大雨", "dayu"),
			new ValueTuple<string, string>("小雪", "xiaoxue"),
			new ValueTuple<string, string>("大雪", "daxue")
		};
		for (int i = 0; i < options.Length; i++)
		{
			input.options.Add(new TMP_Dropdown.OptionData(options[i].Item1));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index < 0 || index >= options.Length;
			if (!flag)
			{
				this.SetValue(key, options[index].Item2);
			}
		});
		int defaultIndex = Mathf.Clamp(input.value, 0, options.Length - 1);
		this.SetValue(key, options[defaultIndex].Item2);
		return widget;
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x000F85B8 File Offset: 0x000F67B8
	private GameObject GetBtn(UI_GMWindow.WidgetParams widgetParams)
	{
		GameObject btn = Object.Instantiate<GameObject>(this.Btn);
		this.SetWidth(btn, widgetParams.Width);
		btn.GetComponentInChildren<TextMeshProUGUI>().text = widgetParams.Comment;
		btn.GetComponent<CButtonObsolete>().interactable = widgetParams.Interactable;
		btn.GetComponent<CButtonObsolete>().onClick.AddListener(widgetParams.OnClick);
		return btn;
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x000F8620 File Offset: 0x000F6820
	private GameObject GetLabel(UI_GMWindow.WidgetParams widgetParams)
	{
		GameObject label = Object.Instantiate<GameObject>(this.Label);
		this.SetWidth(label, widgetParams.Width);
		label.GetComponentInChildren<TextMeshProUGUI>().text = widgetParams.Comment;
		return label;
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000F8660 File Offset: 0x000F6860
	private GameObject GetValueField(UI_GMWindow.WidgetParams widgetParams, int key, Type argType, UI_GMWindow.EValueType valueType)
	{
		GameObject input = Object.Instantiate<GameObject>(this.InputField);
		this.SetWidth(input, widgetParams.Width);
		TMP_InputField inputField = input.GetComponent<TMP_InputField>();
		(inputField.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		switch (valueType)
		{
		case UI_GMWindow.EValueType.Integer:
			inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
			break;
		case UI_GMWindow.EValueType.Float:
			inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			break;
		}
		bool flag = widgetParams.DefaultValue != null;
		if (flag)
		{
			inputField.text = widgetParams.DefaultValue.ToString();
		}
		inputField.onEndEdit.AddListener(delegate(string str)
		{
			this.SetValueOnEndEdit(str, valueType, argType, key, widgetParams.Set2Arg);
		});
		return input;
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000F875C File Offset: 0x000F695C
	private GameObject GetToggle(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject toggle = Object.Instantiate<GameObject>(this.Toggle);
		toggle.GetComponentInChildren<TextMeshProUGUI>().text = widgetParams.Comment;
		this.SetWidth(toggle, widgetParams.Width);
		bool flag = widgetParams.DefaultValue != null;
		if (flag)
		{
			toggle.GetComponentInChildren<CToggleObsolete>().isOn = (bool)widgetParams.DefaultValue;
		}
		else
		{
			this.SetValue(key, toggle.GetComponentInChildren<CToggleObsolete>().isOn);
		}
		toggle.GetComponentInChildren<CToggleObsolete>().onValueChanged.AddListener(delegate(bool isOn)
		{
			this.SetValue(key, isOn);
		});
		return toggle;
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000F8814 File Offset: 0x000F6A14
	private GameObject GetCharIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject charIdWidget = Object.Instantiate<GameObject>(this.CharIdField);
		TMP_InputField input = charIdWidget.GetComponentInChildren<TMP_InputField>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(charIdWidget, widgetParams.Width);
		bool flag = widgetParams.DefaultValue != null;
		if (flag)
		{
			input.text = widgetParams.DefaultValue.ToString();
		}
		charIdWidget.transform.GetChild(1).GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			this.OnClickGetCharId(widgetParams, key, charIdWidget.GetComponentInChildren<TMP_InputField>());
		});
		charIdWidget.transform.GetChild(2).GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			this.OnClickOpenSelectChar(widgetParams, key, charIdWidget.GetComponentInChildren<TMP_InputField>());
		});
		charIdWidget.GetComponentInChildren<TMP_InputField>().onEndEdit.AddListener(delegate(string str)
		{
			bool flag2 = string.IsNullOrEmpty(str);
			if (flag2)
			{
				this._dicParams.Remove(key);
			}
			else
			{
				int value;
				bool flag3 = int.TryParse(Regex.Replace(str, "\\(.*\\)", ""), out value);
				if (flag3)
				{
					this.SetValue(key, value);
				}
				else
				{
					this.Log(LocalStringManager.Get("GM_Message_Func_GetCharIdField_Exception"));
				}
			}
		});
		return charIdWidget;
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000F8934 File Offset: 0x000F6B34
	private GameObject GetInformationTypeIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		for (int i = 0; i < InformationType.Instance.Count; i++)
		{
			input.options.Add(new TMP_Dropdown.OptionData(InformationType.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < InformationType.Instance.Count;
			if (flag)
			{
				this.SetValue(key, InformationType.Instance[index].TemplateId);
			}
		});
		this.SetValue(key, (sbyte)input.value);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000F8A10 File Offset: 0x000F6C10
	private GameObject GetItemTypeIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		for (int i = 0; i < 13; i++)
		{
			input.options.Add(new TMP_Dropdown.OptionData(LocalStringManager.Get(string.Format("LK_ItemType_{0}", i))));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < 13;
			if (flag)
			{
				this.SetValue(key, (sbyte)index);
			}
		});
		this.SetValue(key, (sbyte)input.value);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000F8AEC File Offset: 0x000F6CEC
	private GameObject GetXiangshuAvatarIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] xiangshuIds = new short[]
		{
			39,
			48,
			57,
			66,
			75,
			84,
			93,
			102,
			111
		};
		for (int i = 0; i < xiangshuIds.Length; i++)
		{
			CharacterItem xiangshu = Character.Instance[xiangshuIds[i]];
			input.options.Add(new TMP_Dropdown.OptionData(xiangshu.Surname + xiangshu.GivenName));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < xiangshuIds.Length;
			if (flag)
			{
				this.SetValue(key, (sbyte)index);
			}
		});
		this.SetValue(key, (sbyte)input.value);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000F8BF8 File Offset: 0x000F6DF8
	private GameObject GetTwelveImmortalsIndexField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		for (short i = 1075; i <= 1086; i += 1)
		{
			CharacterItem config = Character.Instance[i];
			input.options.Add(new TMP_Dropdown.OptionData(config.Surname + config.GivenName));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < input.options.Count;
			if (flag)
			{
				this.SetValue(key, index);
			}
		});
		this.SetValue(key, input.value);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000F8D04 File Offset: 0x000F6F04
	private GameObject GetBossCharIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] bossIds = new short[]
		{
			904,
			905,
			918,
			910,
			913,
			914,
			201,
			202,
			203,
			204,
			205,
			206,
			207,
			208,
			209,
			1113
		};
		for (sbyte isWeakened = 0; isWeakened <= 1; isWeakened += 1)
		{
			string weakenedStr = (isWeakened == 1) ? LocalStringManager.Get("GM_WeakenedXiangshu_Prefix") : string.Empty;
			for (sbyte xiangshuAvatarId = 0; xiangshuAvatarId < 9; xiangshuAvatarId += 1)
			{
				for (sbyte xiangshuLevel = 0; xiangshuLevel < 9; xiangshuLevel += 1)
				{
					short templateId = XiangshuAvatarIds.GetCurrentLevelXiangshuTemplateId(xiangshuAvatarId, xiangshuLevel, isWeakened == 1);
					CharacterItem xiangshu = Character.Instance[templateId];
					input.options.Add(new TMP_Dropdown.OptionData(string.Format("{0}{1}{2}[{3}]", new object[]
					{
						weakenedStr,
						xiangshu.Surname,
						xiangshu.GivenName,
						(int)(xiangshuLevel + 1)
					})));
				}
			}
		}
		for (int i = 0; i < bossIds.Length; i++)
		{
			CharacterItem boss = Character.Instance[bossIds[i]];
			input.options.Add(new TMP_Dropdown.OptionData(boss.Surname + boss.GivenName));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0;
			if (flag)
			{
				int xiangshuSplitLine = 162;
				bool flag2 = index >= xiangshuSplitLine;
				if (flag2)
				{
					this.SetValue(key, bossIds[index - xiangshuSplitLine]);
				}
				else
				{
					bool isWeakened2 = index >= 81;
					int internalIndex = index % 81;
					int id = internalIndex / 9;
					int level = internalIndex % 9;
					short templateId2 = XiangshuAvatarIds.GetCurrentLevelXiangshuTemplateId((sbyte)id, (sbyte)level, isWeakened2);
					this.SetValue(key, templateId2);
				}
			}
		});
		this.SetValue(key, 39);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000F8EE0 File Offset: 0x000F70E0
	private GameObject GetAnimalCharIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] bossIds = GameData.Domains.Combat.SharedConstValue.CharId2AnimalId.Keys.ToArray<short>();
		for (int i = 0; i < bossIds.Length; i++)
		{
			CharacterItem boss = Character.Instance[bossIds[i]];
			input.options.Add(new TMP_Dropdown.OptionData(boss.Surname + boss.GivenName));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0;
			if (flag)
			{
				this.SetValue(key, bossIds[index]);
			}
		});
		this.SetValue(key, bossIds[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x000F8FE8 File Offset: 0x000F71E8
	private GameObject GetOrgMemberCharIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<short> charIdList = new List<short>();
		for (short i = 384; i < 518; i += 1)
		{
			charIdList.Add(i);
		}
		short[] charIds = charIdList.ToArray();
		for (int j = 0; j < charIds.Length; j++)
		{
			CharacterItem charCfg = Character.Instance[charIds[j]];
			OrganizationItem org = Organization.Instance[charCfg.OrganizationInfo.OrgTemplateId];
			input.options.Add(new TMP_Dropdown.OptionData(org.Name + charCfg.GivenName));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0;
			if (flag)
			{
				this.SetValue(key, charIds[index]);
			}
		});
		this.SetValue(key, charIds[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x000F9134 File Offset: 0x000F7334
	private GameObject GetRandomEnemyCharIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] charIds = (from characterItem in Character.Instance
		where characterItem.TemplateId >= 0 && characterItem.CreatingType == 2
		select characterItem.TemplateId).ToArray<short>();
		for (int i = 0; i < charIds.Length; i++)
		{
			CharacterItem charCfg = Character.Instance[charIds[i]];
			OrganizationItem org = Organization.Instance[charCfg.OrganizationInfo.OrgTemplateId];
			input.options.Add(new TMP_Dropdown.OptionData(org.Name + charCfg.GivenName));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0;
			if (flag)
			{
				this.SetValue(key, charIds[index]);
			}
		});
		this.SetValue(key, charIds[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x000F9298 File Offset: 0x000F7498
	private GameObject GetStateIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		for (int i = 0; i < MapState.Instance.Count; i++)
		{
			input.options.Add(new TMP_Dropdown.OptionData(MapState.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < MapState.Instance.Count;
			if (flag)
			{
				this.SetValue(key, (sbyte)index);
			}
		});
		this.SetValue(key, (sbyte)input.value);
		return itemTypeIdWidget;
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x000F9374 File Offset: 0x000F7574
	private GameObject GetItemIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		UI_GMWindow.<>c__DisplayClass102_0 CS$<>8__locals1 = new UI_GMWindow.<>c__DisplayClass102_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.key = key;
		GameObject itemIdWidget = Object.Instantiate<GameObject>(this.ItemIdField);
		CS$<>8__locals1.input = itemIdWidget.GetComponentInChildren<TMP_InputField>();
		(CS$<>8__locals1.input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemIdWidget, widgetParams.Width);
		bool flag = widgetParams.DefaultValue != null;
		if (flag)
		{
			CS$<>8__locals1.input.text = widgetParams.DefaultValue.ToString();
		}
		itemIdWidget.transform.GetChild(1).GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			sbyte itemTypeId = CS$<>8__locals1.<>4__this.TryGetItemTypeIdByKey(CS$<>8__locals1.key);
			bool flag2 = itemTypeId >= 0;
			if (flag2)
			{
				UI_GMWindow.<>c__DisplayClass102_1 CS$<>8__locals2 = new UI_GMWindow.<>c__DisplayClass102_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals1.<>4__this.SelectCharRoot.gameObject.SetActive(true);
				CS$<>8__locals1.<>4__this.SelectCharRoot.GetChild(0).gameObject.SetActive(false);
				CS$<>8__locals2.itemNameInput = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/Name/Input").GetComponent<TMP_InputField>();
				CS$<>8__locals2.scroll = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/CharScroll").GetComponent<InfinityScrollLegacy>();
				CS$<>8__locals2.btn = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/ConfirmCharacter").GetComponent<CButtonObsolete>();
				CS$<>8__locals2.itemTogGroup = CS$<>8__locals2.scroll.GetComponent<CToggleGroupObsolete>();
				CS$<>8__locals2.scroll.SetTogGroup(CS$<>8__locals2.itemTogGroup, false, false);
				CS$<>8__locals2.itemTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete _)
				{
					bool flag3 = togNew != null;
					if (flag3)
					{
						CS$<>8__locals2.scroll.SelectedTogKey = togNew.Key;
					}
				};
				CS$<>8__locals2.itemNameDict = new Dictionary<short, string>();
				CS$<>8__locals2.itemIdList = new List<short>();
				CS$<>8__locals2.itemNameInput.onEndEdit.RemoveAllListeners();
				CS$<>8__locals2.itemNameInput.onEndEdit.AddListener(delegate(string str)
				{
					base.<GetItemIdField>g__UpdateItemList|6();
				});
				CS$<>8__locals2.scroll.OnItemRender = delegate(int index, Refers refer)
				{
					refer.CGet<TextMeshProUGUI>("NameText").text = CS$<>8__locals2.itemNameDict[CS$<>8__locals2.itemIdList[index]];
				};
				CS$<>8__locals2.btn.onClick.RemoveAllListeners();
				CS$<>8__locals2.btn.onClick.AddListener(delegate()
				{
					CToggleObsolete selectedTog = CS$<>8__locals2.itemTogGroup.GetActive();
					bool flag3 = selectedTog != null;
					if (flag3)
					{
						short value = CS$<>8__locals2.itemIdList[selectedTog.Key];
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetValue(CS$<>8__locals2.CS$<>8__locals1.key, value);
						CS$<>8__locals2.CS$<>8__locals1.input.text = string.Format("{0}", value);
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.SelectCharRoot.gameObject.SetActive(false);
					}
				});
				List<ValueTuple<string, ValueTuple<int, short>>> typedItemList = new List<ValueTuple<string, ValueTuple<int, short>>>();
				switch (itemTypeId)
				{
				case 0:
					typedItemList.AddRange(from a in Weapon.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(0, a.TemplateId)));
					break;
				case 1:
					typedItemList.AddRange(from a in Armor.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(1, a.TemplateId)));
					break;
				case 2:
					typedItemList.AddRange(from a in Accessory.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(2, a.TemplateId)));
					break;
				case 3:
					typedItemList.AddRange(from a in Clothing.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(3, a.TemplateId)));
					break;
				case 4:
					typedItemList.AddRange(from a in Carrier.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(4, a.TemplateId)));
					break;
				case 5:
					typedItemList.AddRange(from a in Config.Material.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(5, a.TemplateId)));
					break;
				case 6:
					typedItemList.AddRange(from a in CraftTool.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(6, a.TemplateId)));
					break;
				case 7:
					typedItemList.AddRange(from a in Food.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(7, a.TemplateId)));
					break;
				case 8:
					typedItemList.AddRange(from a in Medicine.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(8, a.TemplateId)));
					break;
				case 9:
					typedItemList.AddRange(from a in TeaWine.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(9, a.TemplateId)));
					break;
				case 10:
					typedItemList.AddRange(from a in SkillBook.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(10, a.TemplateId)));
					break;
				case 11:
					typedItemList.AddRange(from a in Cricket.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(11, a.TemplateId)));
					break;
				case 12:
					typedItemList.AddRange(from a in Misc.Instance
					select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(12, a.TemplateId)));
					break;
				}
				CS$<>8__locals2.itemNameDict.Clear();
				foreach (ValueTuple<string, ValueTuple<int, short>> item in typedItemList)
				{
					CS$<>8__locals2.itemNameDict[item.Item2.Item2] = item.Item1;
				}
				CS$<>8__locals2.<GetItemIdField>g__UpdateItemList|6();
				CS$<>8__locals1.<>4__this.SelectCharRoot.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				CS$<>8__locals1.<>4__this.Log(LocalStringManager.Get("GM_Message_Func_GetItemIdField_NoType_Exception"));
			}
		});
		itemIdWidget.GetComponentInChildren<TMP_InputField>().onEndEdit.AddListener(delegate(string str)
		{
			bool flag2 = string.IsNullOrEmpty(str);
			if (flag2)
			{
				CS$<>8__locals1.<>4__this._dicParams.Remove(CS$<>8__locals1.key);
			}
			else
			{
				short value;
				bool flag3 = short.TryParse(Regex.Replace(str, "\\(.*\\)", ""), out value);
				if (flag3)
				{
					CS$<>8__locals1.<>4__this.SetValue(CS$<>8__locals1.key, value);
				}
				else
				{
					CS$<>8__locals1.<>4__this.Log(LocalStringManager.Get("GM_Message_Func_GetItemIdField_Exception"));
				}
			}
		});
		return itemIdWidget;
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x000F9444 File Offset: 0x000F7644
	private sbyte TryGetItemTypeIdByKey(int key)
	{
		int pageId = key / 1000000;
		int memberId = (key - pageId * 1000000) / 1000;
		bool flag = pageId < 0 || pageId >= this._pages.Length;
		sbyte result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			List<MemberInfo> page = this._pages[pageId];
			bool flag2 = memberId < 0 || memberId >= page.Count;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				MemberInfo memberInfo = page[memberId];
				bool flag3 = memberInfo == null;
				if (flag3)
				{
					result = -1;
				}
				else
				{
					foreach (GMFuncArgAttribute argAttribute in memberInfo.GetCustomAttributes<GMFuncArgAttribute>())
					{
						bool flag4 = argAttribute.WidgetType == EWidgetType.ItemTypeIdField;
						if (flag4)
						{
							int itemTypeIdKey = this.GetArgKey(pageId, memberId, argAttribute.Index);
							bool flag5 = this._dicParams.ContainsKey(itemTypeIdKey);
							if (flag5)
							{
								return (sbyte)this._dicParams[itemTypeIdKey];
							}
						}
					}
					result = -1;
				}
			}
		}
		return result;
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x000F956C File Offset: 0x000F776C
	private GameObject GetItemSelectorExField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		UI_GMWindow.<>c__DisplayClass104_0 CS$<>8__locals1 = new UI_GMWindow.<>c__DisplayClass104_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.key = key;
		GameObject itemIdWidget = Object.Instantiate<GameObject>(this.ItemIdField);
		CS$<>8__locals1.input = itemIdWidget.GetComponentInChildren<TMP_InputField>();
		(CS$<>8__locals1.input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemIdWidget, widgetParams.Width);
		bool flag = widgetParams.DefaultValue != null;
		if (flag)
		{
			CS$<>8__locals1.input.text = widgetParams.DefaultValue.ToString();
		}
		CS$<>8__locals1.selectedStorage = new HashSet<int>();
		CS$<>8__locals1.amountStorage = new Dictionary<int, int>();
		itemIdWidget.transform.GetChild(1).GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			UI_GMWindow.<>c__DisplayClass104_1 CS$<>8__locals2 = new UI_GMWindow.<>c__DisplayClass104_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals1.<>4__this.SelectItemRoot.gameObject.SetActive(true);
			CS$<>8__locals1.<>4__this.SelectItemRoot.GetChild(0).gameObject.SetActive(false);
			CS$<>8__locals2.scroll = CS$<>8__locals1.<>4__this.SelectItemRoot.Find("MainWindow/ItemScroll").GetComponent<InfinityScrollLegacy>();
			CS$<>8__locals2.btn = CS$<>8__locals1.<>4__this.SelectItemRoot.Find("MainWindow/ConfirmCharacter").GetComponent<CButtonObsolete>();
			CS$<>8__locals1.selectedStorage.Clear();
			CS$<>8__locals1.amountStorage.Clear();
			CS$<>8__locals2.itemTogGroup = CS$<>8__locals2.scroll.GetComponent<CToggleGroupObsolete>();
			CS$<>8__locals2.itemTogGroup.Clear();
			CS$<>8__locals2.scroll.SetTogGroup(CS$<>8__locals2.itemTogGroup, false, false);
			CS$<>8__locals2.itemTogGroup.AllowUncheck = true;
			CToggleGroupObsolete itemTogGroup = CS$<>8__locals2.itemTogGroup;
			Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange;
			if ((onActiveToggleChange = CS$<>8__locals1.<>9__1) == null)
			{
				onActiveToggleChange = (CS$<>8__locals1.<>9__1 = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
				{
					bool flag2 = togNew != null;
					if (flag2)
					{
						CS$<>8__locals1.selectedStorage.Add(togNew.Key);
					}
					bool flag3 = togOld != null;
					if (flag3)
					{
						CS$<>8__locals1.selectedStorage.Remove(togOld.Key);
					}
				});
			}
			itemTogGroup.OnActiveToggleChange = onActiveToggleChange;
			CS$<>8__locals2.itemNameDict = new Dictionary<ValueTuple<sbyte, short>, string>();
			CS$<>8__locals2.itemIdList = new List<ValueTuple<sbyte, short>>();
			CS$<>8__locals2.scroll.OnItemRender = delegate(int index, Refers refer)
			{
				CToggleObsolete toggle = refer.GetComponent<CToggleObsolete>();
				TextMeshProUGUI nameLabel = refer.CGet<TextMeshProUGUI>("NameText");
				CButtonObsolete amountMinus = refer.transform.Find("Amount/-").GetComponent<CButtonObsolete>();
				CButtonObsolete amountAdd = refer.transform.Find("Amount/+").GetComponent<CButtonObsolete>();
				TMP_Text amountLabel = refer.transform.Find("Amount/Text").GetComponent<TMP_Text>();
				CS$<>8__locals2.itemTogGroup.Set(index, CS$<>8__locals2.CS$<>8__locals1.selectedStorage.Contains(index), false);
				toggle.interactable = true;
				nameLabel.text = CS$<>8__locals2.itemNameDict[CS$<>8__locals2.itemIdList[index]];
				amountMinus.ClearAndAddListener(delegate
				{
					int amount = int.Parse(amountLabel.text);
					bool flag2 = amount > 0;
					if (flag2)
					{
						amountLabel.text = string.Format("{0}", amount - 1);
					}
				});
				amountAdd.ClearAndAddListener(delegate
				{
					int amount = int.Parse(amountLabel.text);
					bool flag2 = amount < 99;
					if (flag2)
					{
						amountLabel.text = string.Format("{0}", amount + 1);
					}
				});
				CS$<>8__locals2.CS$<>8__locals1.amountStorage[index] = int.Parse(amountLabel.text);
			};
			CS$<>8__locals2.btn.onClick.RemoveAllListeners();
			CS$<>8__locals2.btn.onClick.AddListener(delegate()
			{
				List<ValueTuple<sbyte, short, int>> itemPairs = new List<ValueTuple<sbyte, short, int>>();
				foreach (int index in CS$<>8__locals2.CS$<>8__locals1.selectedStorage)
				{
					ValueTuple<sbyte, short> valueTuple = CS$<>8__locals2.itemIdList[index];
					sbyte typeId2 = valueTuple.Item1;
					short templateId2 = valueTuple.Item2;
					int amount;
					bool flag2 = !CS$<>8__locals2.CS$<>8__locals1.amountStorage.TryGetValue(index, out amount);
					if (flag2)
					{
						amount = 1;
					}
					itemPairs.Add(new ValueTuple<sbyte, short, int>(typeId2, templateId2, amount));
				}
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetValue(CS$<>8__locals2.CS$<>8__locals1.key, itemPairs);
				CS$<>8__locals2.CS$<>8__locals1.input.text = "Clipped.";
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.SelectItemRoot.gameObject.SetActive(false);
			});
			for (sbyte typeId = 0; typeId < 13; typeId += 1)
			{
				try
				{
					foreach (int templateId in ItemTemplateHelper.GetTemplateDataAllKeys(typeId))
					{
						ValueTuple<sbyte, short> item = new ValueTuple<sbyte, short>(typeId, (short)templateId);
						CS$<>8__locals2.itemNameDict[item] = ItemTemplateHelper.GetName(typeId, (short)templateId);
						CS$<>8__locals2.itemIdList.Add(item);
					}
				}
				catch (ArgumentOutOfRangeException)
				{
				}
			}
			CS$<>8__locals2.<GetItemSelectorExField>g__UpdateItemList|4();
			CS$<>8__locals1.<>4__this.SelectItemRoot.GetChild(0).gameObject.SetActive(true);
		});
		return itemIdWidget;
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x000F9632 File Offset: 0x000F7832
	public void LogCombatOvercome(sbyte ret)
	{
		this.Log("Combat skiped: " + UI_GMWindow._CombatResultType2Name[ret]);
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x000F9651 File Offset: 0x000F7851
	public void LogLifeSkillCombatOvercome(sbyte ret)
	{
		this.Log("Combat skiped: " + UI_GMWindow._LifeSkillCombatResultType2Name[ret]);
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x000F9670 File Offset: 0x000F7870
	private GameObject GetCombatResultTypeField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		sbyte[] keys = UI_GMWindow._CombatResultType2Name.Keys.ToArray<sbyte>();
		Array.Sort<sbyte>(keys);
		foreach (sbyte i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(UI_GMWindow._CombatResultType2Name[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, -1);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x000F9768 File Offset: 0x000F7968
	private GameObject GetLegacyTemplateIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		foreach (LegacyItem item in ((IEnumerable<LegacyItem>)Legacy.Instance))
		{
			input.options.Add(new TMP_Dropdown.OptionData(item.Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < Legacy.Instance.Count;
			if (flag)
			{
				this.SetValue(key, Legacy.Instance[index].TemplateId);
			}
		});
		this.SetValue(key, Legacy.Instance[0].TemplateId);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000F9864 File Offset: 0x000F7A64
	private GameObject GetLegacyPointTemplateIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		foreach (LegacyPointItem item in ((IEnumerable<LegacyPointItem>)LegacyPoint.Instance))
		{
			input.options.Add(new TMP_Dropdown.OptionData(item.Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < LegacyPoint.Instance.Count;
			if (flag)
			{
				this.SetValue(key, LegacyPoint.Instance[index].TemplateId);
			}
		});
		this.SetValue(key, LegacyPoint.Instance[0].TemplateId);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x000F9960 File Offset: 0x000F7B60
	private GameObject GetCombatTypeField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		sbyte[] keys = UI_GMWindow._CombatType2Name.Keys.ToArray<sbyte>();
		Array.Sort<sbyte>(keys);
		foreach (sbyte i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(UI_GMWindow._CombatType2Name[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000F9A60 File Offset: 0x000F7C60
	private GameObject GetCombatConfigField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] templateIds = (from combatConfigItem in CombatConfig.Instance
		select combatConfigItem.TemplateId).ToArray<short>();
		Dictionary<short, string> refMap = new Dictionary<short, string>();
		string refMapFilePath = Path.Combine(Application.dataPath, "StreamingAssets/ConfigRefNameMapping/CombatConfig.ref.txt");
		string[] refMapLines = (from x in File.ReadAllText(refMapFilePath).Replace('\r', '\n').Replace("\n\n", "\n").Split('\n', StringSplitOptions.None)
		where !x.IsNullOrEmpty()
		select x).ToArray<string>();
		for (int i = 0; i < refMapLines.Length; i += 2)
		{
			bool flag = i + 1 >= refMapLines.Length;
			if (flag)
			{
				break;
			}
			string refName = refMapLines[i];
			short id;
			bool flag2 = short.TryParse(refMapLines[i + 1], out id);
			if (flag2)
			{
				refMap.Add(id, refName);
			}
		}
		for (int j = 0; j < templateIds.Length; j++)
		{
			CombatConfigItem config = CombatConfig.Instance[templateIds[j]];
			input.options.Add(new TMP_Dropdown.OptionData(refMap[config.TemplateId] ?? ""));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag3 = index >= 0;
			if (flag3)
			{
				this.SetValue(key, templateIds[index]);
			}
		});
		this.SetValue(key, templateIds[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000F9C50 File Offset: 0x000F7E50
	private GameObject GetLifeSkillCombatResultTypeField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		sbyte[] keys = UI_GMWindow._LifeSkillCombatResultType2Name.Keys.ToArray<sbyte>();
		Array.Sort<sbyte>(keys);
		foreach (sbyte i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(UI_GMWindow._LifeSkillCombatResultType2Name[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, -1);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000F9D48 File Offset: 0x000F7F48
	private GameObject GetLifeSkillCombatTalkIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<short> keys = LifeSkillCombatTalk.Instance.GetAllKeys();
		foreach (short i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(LifeSkillCombatTalk.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < LifeSkillCombatTalk.Instance.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000F9E64 File Offset: 0x000F8064
	private GameObject GetConfigurationTemplateId<TItem, TTemplateId>(UI_GMWindow.WidgetParams widgetParams, int key, IEnumerable<TTemplateId> allKey, Func<TTemplateId, TItem> itemGetter, Func<TItem, string> nameGetter, Func<TItem, bool> filter)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		TTemplateId[] keys = (from id in allKey
		where filter(itemGetter(id))
		select id).ToArray<TTemplateId>();
		foreach (TTemplateId i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(nameGetter(itemGetter(i))));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = keys.CheckIndex(index);
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000F9F80 File Offset: 0x000F8180
	private GameObject GetEnumValue<TEnum>(UI_GMWindow.WidgetParams widgetParams, int key, Func<TEnum, string> nameGetter, Func<TEnum, bool> filter)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		TEnum[] values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Where(filter).ToArray<TEnum>();
		foreach (TEnum value in values)
		{
			input.options.Add(new TMP_Dropdown.OptionData(nameGetter(value)));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = values.CheckIndex(index);
			if (flag)
			{
				this.SetValue(key, values[index]);
			}
		});
		this.SetValue(key, values[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000FA088 File Offset: 0x000F8288
	private GameObject GetLifeSkillCombatNodeEffectIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<short> keys = DebateNodeEffect.Instance.GetAllKeys();
		foreach (short i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(DebateNodeEffect.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < DebateNodeEffect.Instance.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000FA1A4 File Offset: 0x000F83A4
	private GameObject GetStoryProgressField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] keys = UI_GMWindow.StoryProgress2Name.Keys.ToArray<short>();
		Array.Sort<short>(keys);
		foreach (short i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(UI_GMWindow.StoryProgress2Name[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000FA2A4 File Offset: 0x000F84A4
	private GameObject GetAdventureIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		Dictionary<short, string> adventures = new Dictionary<short, string>();
		short id = 0;
		short len = (short)Adventure.Instance.Count;
		while (id < len)
		{
			adventures.Add(id, Adventure.Instance[id].Name);
			id += 1;
		}
		short[] keys = adventures.Keys.ToArray<short>();
		Array.Sort<short>(keys);
		foreach (short i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(adventures[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < adventures.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000FA3F4 File Offset: 0x000F85F4
	private GameObject GetAdventureRemakeIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		UI_GMWindow.<>c__DisplayClass123_0 CS$<>8__locals1 = new UI_GMWindow.<>c__DisplayClass123_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.key = key;
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemIdField);
		CS$<>8__locals1.input = itemTypeIdWidget.GetComponentInChildren<TMP_InputField>();
		((TextMeshProUGUI)CS$<>8__locals1.input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		CS$<>8__locals1.adventures = new Dictionary<int, string>();
		foreach (AdventureData adventure in AdventureRemakeModel.Core.AllAdventures)
		{
			CS$<>8__locals1.adventures.Add(adventure.Id, adventure.Name);
		}
		itemTypeIdWidget.transform.GetChild(1).GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			UI_GMWindow.<>c__DisplayClass123_1 CS$<>8__locals2 = new UI_GMWindow.<>c__DisplayClass123_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals1.<>4__this.SelectCharRoot.gameObject.SetActive(true);
			CS$<>8__locals1.<>4__this.SelectCharRoot.GetChild(0).gameObject.SetActive(false);
			CS$<>8__locals2.itemNameInput = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/Name/Input").GetComponent<TMP_InputField>();
			CS$<>8__locals2.scroll = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/CharScroll").GetComponent<InfinityScrollLegacy>();
			CS$<>8__locals2.btn = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/ConfirmCharacter").GetComponent<CButtonObsolete>();
			CS$<>8__locals2.itemTogGroup = CS$<>8__locals2.scroll.GetComponent<CToggleGroupObsolete>();
			CS$<>8__locals2.scroll.SetTogGroup(CS$<>8__locals2.itemTogGroup, false, false);
			CS$<>8__locals2.itemTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete _)
			{
				bool flag = togNew != null;
				if (flag)
				{
					CS$<>8__locals2.scroll.SelectedTogKey = togNew.Key;
				}
			};
			CS$<>8__locals2.itemNameDict = CS$<>8__locals1.adventures;
			CS$<>8__locals2.itemIdList = new List<int>();
			CS$<>8__locals2.itemNameInput.onEndEdit.RemoveAllListeners();
			CS$<>8__locals2.itemNameInput.onEndEdit.AddListener(delegate(string str)
			{
				base.<GetAdventureRemakeIdField>g__UpdateItemList|5();
			});
			CS$<>8__locals2.scroll.OnItemRender = delegate(int index, Refers refer)
			{
				refer.CGet<TextMeshProUGUI>("NameText").text = CS$<>8__locals2.itemNameDict[CS$<>8__locals2.itemIdList[index]];
			};
			CS$<>8__locals2.btn.onClick.RemoveAllListeners();
			CS$<>8__locals2.btn.onClick.AddListener(delegate()
			{
				CToggleObsolete selectedTog = CS$<>8__locals2.itemTogGroup.GetActive();
				bool flag = selectedTog != null;
				if (flag)
				{
					int value = CS$<>8__locals2.itemIdList[selectedTog.Key];
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetValue(CS$<>8__locals2.CS$<>8__locals1.key, value);
					CS$<>8__locals2.CS$<>8__locals1.input.text = string.Format("{0}", value);
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.SelectCharRoot.gameObject.SetActive(false);
				}
			});
			CS$<>8__locals2.<GetAdventureRemakeIdField>g__UpdateItemList|5();
			CS$<>8__locals1.<>4__this.SelectCharRoot.GetChild(0).gameObject.SetActive(true);
			UnityEvent<string> onValueChanged = CS$<>8__locals1.input.onValueChanged;
			UnityAction<string> call;
			if ((call = CS$<>8__locals1.<>9__6) == null)
			{
				call = (CS$<>8__locals1.<>9__6 = delegate(string content)
				{
					int index;
					bool flag = int.TryParse(content, out index) && index >= 0 && index < CS$<>8__locals1.adventures.Count;
					if (flag)
					{
						CS$<>8__locals1.<>4__this.SetValue(CS$<>8__locals1.key, index);
					}
					else
					{
						CS$<>8__locals1.<>4__this.Log(LanguageKey.GM_Message_Func_GetAdventureRemakeIdField_Exception.Tr());
					}
				});
			}
			onValueChanged.AddListener(call);
		});
		return itemTypeIdWidget;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000FA4E8 File Offset: 0x000F86E8
	private GameObject GetAdventureMajorEventIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		Dictionary<int, string> adventures = new Dictionary<int, string>();
		foreach (AdventureMajorEventData majorEvent in AdventureRemakeModel.Core.AllAdventureMajorEvents)
		{
			adventures.Add(majorEvent.Id, majorEvent.Name);
		}
		int[] keys = adventures.Keys.ToArray<int>();
		Array.Sort<int>(keys);
		foreach (int i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(adventures[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < adventures.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000FA654 File Offset: 0x000F8854
	public void ShowGMGlobalParameterPanel()
	{
		AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
		bool notInAdventure = model.AdventureTaiwu.NotInAdventure;
		if (!notInAdventure)
		{
			GameObject gmGlobalParameterPanelHolder = this.GMGlobalParameterPanel.transform.parent.gameObject;
			bool flag = !gmGlobalParameterPanelHolder.activeSelf;
			if (flag)
			{
				this.RefreshGMGlobalParameterPanel();
			}
			gmGlobalParameterPanelHolder.SetActive(!gmGlobalParameterPanelHolder.activeSelf);
		}
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x000FA6B8 File Offset: 0x000F88B8
	private void RefreshGMGlobalParameterPanel()
	{
		InfinityScrollLegacy gmglobalParameterPanel = this.GMGlobalParameterPanel;
		if (gmglobalParameterPanel.OnItemRender == null)
		{
			gmglobalParameterPanel.OnItemRender = new Action<int, Refers>(this.OnRenderGMGlobalParameter);
		}
		this._gmGlobalParameterList.Clear();
		AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
		AdventureRuntime adventureRuntime = model.AdventureRemakeDict[model.AdventureTaiwu.AdventureId];
		AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(adventureRuntime.CoreId);
		foreach (AdventureParameterData adventureParameterData in adventureData.Parameters)
		{
			bool flag = adventureParameterData == null;
			if (!flag)
			{
				AdventureParameterValue parameterValue = adventureRuntime.GetParameter(adventureParameterData.Key);
				this._gmGlobalParameterList.Add(new ValueTuple<AdventureParameterData, AdventureParameterValue>(adventureParameterData, parameterValue));
			}
		}
		this._gmGlobalParameterList.Sort(([TupleElementNames(new string[]
		{
			"parameterData",
			"parameterValue"
		})] ValueTuple<AdventureParameterData, AdventureParameterValue> l, [TupleElementNames(new string[]
		{
			"parameterData",
			"parameterValue"
		})] ValueTuple<AdventureParameterData, AdventureParameterValue> r) => l.Item1.Type - r.Item1.Type);
		this.GMGlobalParameterPanel.SetDataCount(this._gmGlobalParameterList.Count);
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x000FA7D8 File Offset: 0x000F89D8
	private void OnRenderGMGlobalParameter(int index, Refers refers)
	{
		ValueTuple<AdventureParameterData, AdventureParameterValue> valueTuple = this._gmGlobalParameterList[index];
		AdventureParameterData parameterData = valueTuple.Item1;
		AdventureParameterValue parameterValue = valueTuple.Item2;
		bool isStateOrProgress = parameterData.Type == EAdventureParameterType.State;
		TextMeshProUGUI infoText = refers.CGet<TextMeshProUGUI>("ParameterInfo");
		string text;
		if (!isStateOrProgress)
		{
			int num = parameterValue.Current;
			text = num.ToString();
		}
		else
		{
			text = ((parameterData.Style == 2) ? (parameterValue.Max - parameterValue.Current) : parameterValue.Current).ToString() + "/" + parameterValue.Max.ToString();
		}
		string valueStr = text;
		int style2 = parameterData.Style;
		if (!true)
		{
		}
		string text2;
		switch (style2)
		{
		case 0:
			text2 = "Taiwu";
			break;
		case 1:
			text2 = "Global";
			break;
		case 2:
			text2 = "GlobalReverse";
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(style2);
			break;
		}
		if (!true)
		{
		}
		string style = text2;
		infoText.SetText(string.Format("{0} {1}[{2}][{3}]:{4}", new object[]
		{
			parameterData.Key,
			parameterData.Name,
			parameterData.Type,
			style,
			valueStr
		}), true);
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000FA908 File Offset: 0x000F8B08
	public void ShowGMElementParameterPanel()
	{
		AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
		bool notInAdventure = model.AdventureTaiwu.NotInAdventure;
		if (!notInAdventure)
		{
			GameObject gmElementParameterPanelHolder = this.GMElementParameterPanel.transform.parent.gameObject;
			InfinityScrollLegacy gmelementParameterPanel = this.GMElementParameterPanel;
			if (gmelementParameterPanel.OnItemRender == null)
			{
				gmelementParameterPanel.OnItemRender = new Action<int, Refers>(this.OnRenderGMElementParameter);
			}
			List<ViewAdventureRemake.ElementDisplayItem> displayItems = UIElement.AdventureRemake.UiBaseAs<ViewAdventureRemake>().DisplayItems;
			this.GMElementParameterPanel.SetDataCount(displayItems.Count);
			gmElementParameterPanelHolder.SetActive(!gmElementParameterPanelHolder.activeSelf);
		}
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000FA99C File Offset: 0x000F8B9C
	private void OnRenderGMElementParameter(int index, Refers refers)
	{
		AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
		AdventureRuntime adventureRuntime = model.AdventureRemakeDict[model.AdventureTaiwu.AdventureId];
		List<ViewAdventureRemake.ElementDisplayItem> displayItems = UIElement.AdventureRemake.UiBaseAs<ViewAdventureRemake>().DisplayItems;
		ViewAdventureRemake.ElementDisplayItem item = displayItems[index];
		TextMeshProUGUI elementInfo = refers.CGet<TextMeshProUGUI>("ElementInfo");
		RectTransform parameterHolder = refers.CGet<RectTransform>("ParameterHolder");
		parameterHolder.gameObject.SetActive(!item.IsExitItem);
		bool isExitItem = item.IsExitItem;
		if (isExitItem)
		{
			elementInfo.text = string.Format("{0}-ExitItem", index);
		}
		else
		{
			AdventureElementData data = AdventureRemakeModel.Core.GetAdventureElementData(item.Element.CoreId);
			elementInfo.text = string.Format("{0}-{1}{2}", index, item.Element.Id, data.Name);
			int count = 0;
			int remain;
			AdventureActionData elementActionData = adventureRuntime.QueryElementActionData(item.Element, out remain);
			bool flag = elementActionData != null;
			if (flag)
			{
				parameterHolder.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}:{1}", elementActionData.Key, item.Element.GetParameter(elementActionData.Key));
				count++;
			}
			foreach (AdventureParameterData parameter in item.Element.Core.Parameters)
			{
				AdventureParameterValue value = item.Element.GetParameter(parameter.Key);
				bool flag2 = count >= parameterHolder.childCount;
				if (flag2)
				{
					Object.Instantiate<Transform>(parameterHolder.GetChild(0), parameterHolder);
				}
				Transform parameterObj = parameterHolder.GetChild(count);
				parameterObj.gameObject.SetActive(true);
				parameterObj.GetComponent<TextMeshProUGUI>().text = string.Format("{0}:{1}", parameter.Key, value);
				count++;
			}
			for (int i = parameterHolder.childCount - 1; i >= count; i--)
			{
				parameterHolder.GetChild(i).gameObject.SetActive(false);
			}
			parameterHolder.gameObject.SetActive(count > 0);
		}
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x000FAC00 File Offset: 0x000F8E00
	private GameObject GetBattleSceneIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		int count = 0;
		foreach (CombatSceneItem c in ((IEnumerable<CombatSceneItem>)Config.CombatScene.Instance))
		{
			input.options.Add(new TMP_Dropdown.OptionData(c.Name));
			count++;
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < count;
			if (flag)
			{
				this.SetValue(key, Config.CombatScene.Instance[index].TemplateId);
			}
		});
		this.SetValue(key, Config.CombatScene.Instance[0].TemplateId);
		return itemTypeIdWidget;
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000FAD14 File Offset: 0x000F8F14
	private GameObject GetLifeSkillIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		Dictionary<short, string> lifeSkills = new Dictionary<short, string>
		{
			{
				-1,
				LocalStringManager.Get("LK_Common_All")
			}
		};
		short i = 0;
		while ((int)i < LifeSkill.Instance.Count)
		{
			Config.LifeSkillItem item = LifeSkill.Instance.GetItem(i);
			lifeSkills.Add(item.TemplateId, item.Name);
			i += 1;
		}
		short[] keys = lifeSkills.Keys.ToArray<short>();
		Array.Sort<short>(keys);
		foreach (short j in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(lifeSkills[j]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < lifeSkills.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000FAE7C File Offset: 0x000F907C
	private GameObject GetLifeSkillTypeField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		Dictionary<sbyte, string> lifeSkillTypes = new Dictionary<sbyte, string>
		{
			{
				-1,
				LocalStringManager.Get("GM_CombatResult_Randomize")
			}
		};
		sbyte i = 0;
		while ((int)i < Config.LifeSkillType.Instance.Count)
		{
			LifeSkillTypeItem item = Config.LifeSkillType.Instance.GetItem(i);
			lifeSkillTypes.Add(item.TemplateId, item.Name);
			i += 1;
		}
		sbyte[] keys = lifeSkillTypes.Keys.ToArray<sbyte>();
		Array.Sort<sbyte>(keys);
		foreach (sbyte j in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(lifeSkillTypes[j]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < lifeSkillTypes.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x000FAFE4 File Offset: 0x000F91E4
	private GameObject GetCombatSkillIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		Dictionary<short, string> combatSkills = new Dictionary<short, string>
		{
			{
				-1,
				LocalStringManager.Get("LK_Common_All")
			}
		};
		short i = 0;
		while ((int)i < CombatSkill.Instance.Count)
		{
			CombatSkillItem item = CombatSkill.Instance.GetItem(i);
			combatSkills.Add(item.TemplateId, item.Name);
			i += 1;
		}
		short[] keys = combatSkills.Keys.ToArray<short>();
		Array.Sort<short>(keys);
		foreach (short j in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(combatSkills[j]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < combatSkills.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000FB14C File Offset: 0x000F934C
	private GameObject GetCombatSkillTypeField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		Dictionary<sbyte, string> skillTypes = new Dictionary<sbyte, string>
		{
			{
				-1,
				LocalStringManager.Get("GM_CombatResult_Randomize")
			}
		};
		sbyte i = 0;
		while ((int)i < CombatSkillType.Instance.Count)
		{
			CombatSkillTypeItem item = CombatSkillType.Instance.GetItem(i);
			skillTypes.Add(item.TemplateId, item.Name);
			i += 1;
		}
		sbyte[] keys = skillTypes.Keys.ToArray<sbyte>();
		Array.Sort<sbyte>(keys);
		foreach (sbyte j in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(skillTypes[j]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < skillTypes.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x000FB2B4 File Offset: 0x000F94B4
	private GameObject GetCharacterRelationshipTypeField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		sbyte[] keys = UI_GMWindow._CharacterRelationshipTypeType2Name.Keys.ToArray<sbyte>();
		Array.Sort<sbyte>(keys);
		foreach (sbyte i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(UI_GMWindow._CharacterRelationshipTypeType2Name[i]));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x000FB3B4 File Offset: 0x000F95B4
	private GameObject GetProfessionField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		foreach (ProfessionItem professionConfig in ((IEnumerable<ProfessionItem>)Profession.Instance))
		{
			input.options.Add(new TMP_Dropdown.OptionData(professionConfig.Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < Profession.Instance.Count;
			if (flag)
			{
				this.SetValue(key, index);
			}
		});
		this.SetValue(key, 0);
		return itemTypeIdWidget;
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000FB4A4 File Offset: 0x000F96A4
	private GameObject GetProfessionSkillIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		foreach (ProfessionSkillItem professionConfig in ((IEnumerable<ProfessionSkillItem>)ProfessionSkill.Instance))
		{
			input.options.Add(new TMP_Dropdown.OptionData(professionConfig.Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < ProfessionSkill.Instance.Count;
			if (flag)
			{
				this.SetValue(key, index);
			}
		});
		this.SetValue(key, 0);
		return itemTypeIdWidget;
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000FB594 File Offset: 0x000F9794
	private GameObject GetTeaHorseCaravanEventIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		foreach (TeaHorseCaravanEventItem config in ((IEnumerable<TeaHorseCaravanEventItem>)TeaHorseCaravanEvent.Instance))
		{
			input.options.Add(new TMP_Dropdown.OptionData(config.Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < TeaHorseCaravanEvent.Instance.Count;
			if (flag)
			{
				this.SetValue(key, (short)index);
			}
		});
		this.SetValue(key, 0);
		return itemTypeIdWidget;
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000FB684 File Offset: 0x000F9884
	private GameObject GetTaskInfoIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<int> keys = new List<int>();
		foreach (TaskInfoItem taskInfoConfig in ((IEnumerable<TaskInfoItem>)TaskInfo.Instance))
		{
			bool flag = taskInfoConfig != null && taskInfoConfig.IsTriggeredTask;
			if (flag)
			{
				this._sb.Clear();
				this._sb.Append(taskInfoConfig.TaskTitle);
				this._sb.Append("(");
				this._sb.Append(taskInfoConfig.TemplateId);
				this._sb.Append(")");
				keys.Add(taskInfoConfig.TemplateId);
				input.options.Add(new TMP_Dropdown.OptionData(this._sb.ToString()));
			}
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag2 = index >= 0 && index < TaskInfo.Instance.Count;
			if (flag2)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000FB814 File Offset: 0x000F9A14
	private GameObject GetSectIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		(input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<int> keys = new List<int>();
		foreach (OrganizationItem org in ((IEnumerable<OrganizationItem>)Organization.Instance))
		{
			bool isSect = org.IsSect;
			if (isSect)
			{
				keys.Add((int)(org.TemplateId - 1));
				input.options.Add(new TMP_Dropdown.OptionData(org.Name));
			}
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < MapState.Instance.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[input.value]);
		return itemTypeIdWidget;
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x000FB940 File Offset: 0x000F9B40
	private GameObject GetPoisonItemIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		UI_GMWindow.<>c__DisplayClass143_0 CS$<>8__locals1 = new UI_GMWindow.<>c__DisplayClass143_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.key = key;
		GameObject itemIdWidget = Object.Instantiate<GameObject>(this.ItemIdField);
		CS$<>8__locals1.input = itemIdWidget.GetComponentInChildren<TMP_InputField>();
		(CS$<>8__locals1.input.placeholder as TextMeshProUGUI).text = widgetParams.Comment;
		this.SetWidth(itemIdWidget, widgetParams.Width);
		bool flag = widgetParams.DefaultValue != null;
		if (flag)
		{
			CS$<>8__locals1.input.text = widgetParams.DefaultValue.ToString();
		}
		itemIdWidget.transform.GetChild(1).GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			UI_GMWindow.<>c__DisplayClass143_1 CS$<>8__locals2 = new UI_GMWindow.<>c__DisplayClass143_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals1.<>4__this.SelectCharRoot.gameObject.SetActive(true);
			CS$<>8__locals1.<>4__this.SelectCharRoot.GetChild(0).gameObject.SetActive(false);
			CS$<>8__locals2.itemNameInput = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/Name/Input").GetComponent<TMP_InputField>();
			CS$<>8__locals2.scroll = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/CharScroll").GetComponent<InfinityScrollLegacy>();
			CS$<>8__locals2.btn = CS$<>8__locals1.<>4__this.SelectCharRoot.Find("MainWindow/ConfirmCharacter").GetComponent<CButtonObsolete>();
			CS$<>8__locals2.itemTogGroup = CS$<>8__locals2.scroll.GetComponent<CToggleGroupObsolete>();
			CS$<>8__locals2.scroll.SetTogGroup(CS$<>8__locals2.itemTogGroup, false, false);
			CS$<>8__locals2.itemTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete _)
			{
				bool flag2 = togNew != null;
				if (flag2)
				{
					CS$<>8__locals2.scroll.SelectedTogKey = togNew.Key;
				}
			};
			CS$<>8__locals2.itemNameDict = new Dictionary<short, string>();
			CS$<>8__locals2.itemIdList = new List<short>();
			CS$<>8__locals2.itemNameInput.onEndEdit.RemoveAllListeners();
			CS$<>8__locals2.itemNameInput.onEndEdit.AddListener(delegate(string str)
			{
				base.<GetPoisonItemIdField>g__UpdateItemList|7();
			});
			CS$<>8__locals2.scroll.OnItemRender = delegate(int index, Refers refer)
			{
				refer.CGet<TextMeshProUGUI>("NameText").text = CS$<>8__locals2.itemNameDict[CS$<>8__locals2.itemIdList[index]];
			};
			CS$<>8__locals2.btn.onClick.RemoveAllListeners();
			CS$<>8__locals2.btn.onClick.AddListener(delegate()
			{
				CToggleObsolete selectedTog = CS$<>8__locals2.itemTogGroup.GetActive();
				bool flag2 = selectedTog != null;
				if (flag2)
				{
					short value = CS$<>8__locals2.itemIdList[selectedTog.Key];
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.SetValue(CS$<>8__locals2.CS$<>8__locals1.key, value);
					CS$<>8__locals2.CS$<>8__locals1.input.text = string.Format("{0}", value);
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.SelectCharRoot.gameObject.SetActive(false);
				}
			});
			List<ValueTuple<string, ValueTuple<int, short>>> typedItemList = new List<ValueTuple<string, ValueTuple<int, short>>>();
			typedItemList.AddRange(from a in Medicine.Instance
			where a.ItemSubType == 801
			select new ValueTuple<string, ValueTuple<int, short>>(a.Name, new ValueTuple<int, short>(8, a.TemplateId)));
			CS$<>8__locals2.itemNameDict.Clear();
			foreach (ValueTuple<string, ValueTuple<int, short>> item in typedItemList)
			{
				CS$<>8__locals2.itemNameDict[item.Item2.Item2] = item.Item1;
			}
			CS$<>8__locals2.<GetPoisonItemIdField>g__UpdateItemList|7();
			CS$<>8__locals1.<>4__this.SelectCharRoot.GetChild(0).gameObject.SetActive(true);
		});
		return itemIdWidget;
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000FB9F0 File Offset: 0x000F9BF0
	private GameObject GetSkillBreakPlateGridBonusTypeIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] templateIds = (from configItem in SkillBreakPlateGridBonusType.Instance
		select configItem.TemplateId).ToArray<short>();
		Dictionary<short, string> refMap = new Dictionary<short, string>();
		string refMapFilePath = Path.Combine(Application.dataPath, "StreamingAssets/ConfigRefNameMapping/SkillBreakPlateGridBonusType.ref.txt");
		string[] refMapLines = (from x in File.ReadAllText(refMapFilePath).Replace('\r', '\n').Replace("\n\n", "\n").Split('\n', StringSplitOptions.None)
		where !x.IsNullOrEmpty()
		select x).ToArray<string>();
		for (int i = 0; i < refMapLines.Length; i += 2)
		{
			bool flag = i + 1 >= refMapLines.Length;
			if (flag)
			{
				break;
			}
			string refName = refMapLines[i];
			short id;
			bool flag2 = short.TryParse(refMapLines[i + 1], out id);
			if (flag2)
			{
				refMap.Add(id, refName);
			}
		}
		for (int j = 0; j < templateIds.Length; j++)
		{
			SkillBreakPlateGridBonusTypeItem config = SkillBreakPlateGridBonusType.Instance[templateIds[j]];
			input.options.Add(new TMP_Dropdown.OptionData(refMap[config.TemplateId] ?? ""));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag3 = index >= 0;
			if (flag3)
			{
				this.SetValue(key, templateIds[index]);
			}
		});
		this.SetValue(key, templateIds[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x000FBBE0 File Offset: 0x000F9DE0
	private GameObject GetJiaoTemplateId(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<int> keys = new List<int>();
		foreach (JiaoItem JiaoConfig in ((IEnumerable<JiaoItem>)Jiao.Instance))
		{
			bool flag = JiaoConfig.TemplateId >= 0 && JiaoConfig.TemplateId <= 30;
			if (flag)
			{
				this._sb.Clear();
				this._sb.Append(JiaoConfig.Name);
				this._sb.Append("(");
				this._sb.Append(JiaoConfig.TemplateId);
				this._sb.Append(")");
				keys.Add((int)JiaoConfig.TemplateId);
				input.options.Add(new TMP_Dropdown.OptionData(this._sb.ToString()));
			}
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag2 = index >= 0 && index <= 30;
			if (flag2)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000FBD7C File Offset: 0x000F9F7C
	private GameObject GetChildrenOfLoongTemplateId(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<int> keys = new List<int>();
		foreach (JiaoItem JiaoConfig in ((IEnumerable<JiaoItem>)Jiao.Instance))
		{
			bool flag = JiaoConfig.TemplateId >= 31 && JiaoConfig.TemplateId <= 39;
			if (flag)
			{
				this._sb.Clear();
				this._sb.Append(JiaoConfig.Name);
				this._sb.Append("(");
				this._sb.Append(JiaoConfig.TemplateId);
				this._sb.Append(")");
				keys.Add((int)JiaoConfig.TemplateId);
				input.options.Add(new TMP_Dropdown.OptionData(this._sb.ToString()));
			}
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag2 = index >= 0 && index <= 8;
			if (flag2)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000FBF18 File Offset: 0x000FA118
	private GameObject GetCaravanState(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<int> keys = new List<int>();
		for (sbyte i = CaravanState.Normal.ToSbyte(); i <= CaravanState.RobEnd.ToSbyte(); i += 1)
		{
			keys.Add((int)i);
			CaravanState state = (CaravanState)i;
			input.options.Add(new TMP_Dropdown.OptionData(state.ToString()));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			this.SetValue(key, keys[index]);
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x000FC020 File Offset: 0x000FA220
	private GameObject GetWorldCreationType(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		byte[] keys = WorldCreation.Instance.GetAllKeys().ToArray();
		foreach (byte i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(WorldCreation.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x000FC118 File Offset: 0x000FA318
	private GameObject GetVillagerRoleTemplateId(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] keys = VillagerRole.Instance.GetAllKeys().ToArray();
		VillagerRole collection = VillagerRole.Instance;
		short[] keys2 = keys;
		for (int i = 0; i < keys2.Length; i++)
		{
			short k = keys2[i];
			input.options.Add(new TMP_Dropdown.OptionData(collection.RefNameMap.Keys.FirstOrDefault((string id) => collection.RefNameMap[id] == (int)k)));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002214 RID: 8724 RVA: 0x000FC248 File Offset: 0x000FA448
	private GameObject GetMapBlockTemplateId(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		short[] keys = MapBlock.Instance.GetAllKeys().ToArray();
		MapBlock collection = MapBlock.Instance;
		short[] keys2 = keys;
		for (int i = 0; i < keys2.Length; i++)
		{
			short k = keys2[i];
			input.options.Add(new TMP_Dropdown.OptionData(collection.RefNameMap.Keys.FirstOrDefault((string id) => collection.RefNameMap[id] == (int)k)));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < keys.Length;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x000FC378 File Offset: 0x000FA578
	private GameObject GetLifeSkillCombatStrategyIdField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<short> keys = DebateStrategy.Instance.GetAllKeys();
		foreach (short i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(DebateStrategy.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < DebateStrategy.Instance.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x000FC494 File Offset: 0x000FA694
	private GameObject GetMerchantTypeFieldField(UI_GMWindow.WidgetParams widgetParams, int key)
	{
		GameObject itemTypeIdWidget = Object.Instantiate<GameObject>(this.ItemTypeIdField);
		TMP_Dropdown input = itemTypeIdWidget.GetComponentInChildren<TMP_Dropdown>();
		((TextMeshProUGUI)input.placeholder).text = widgetParams.Comment;
		this.SetWidth(itemTypeIdWidget, widgetParams.Width);
		input.options.Clear();
		List<sbyte> keys = Config.MerchantType.Instance.GetAllKeys();
		foreach (sbyte i in keys)
		{
			input.options.Add(new TMP_Dropdown.OptionData(Config.MerchantType.Instance[i].Name));
		}
		input.onValueChanged.AddListener(delegate(int index)
		{
			bool flag = index >= 0 && index < Config.MerchantType.Instance.Count;
			if (flag)
			{
				this.SetValue(key, keys[index]);
			}
		});
		this.SetValue(key, keys[0]);
		return itemTypeIdWidget;
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000FC5B0 File Offset: 0x000FA7B0
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		bool flag = this._listenerId == null;
		if (!flag)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					DataUid uid = notification.Uid;
					bool flag2 = this._gameDataRecevier != null;
					if (flag2)
					{
						bool flag3 = this._listenerId != null;
						if (flag3)
						{
							GameDataBridge.AddDataUnMonitor(this._listenerId.Value, uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1);
						}
						int index = this._gameDataReceivingList.IndexOf(uid);
						bool flag4 = index >= 0 && !this._gameDataReceivingStorage.ContainsKey(uid);
						if (flag4)
						{
							Type dataType = this._gameDataReceivingTypes[index];
							MethodInfo methodInfo = typeof(Serializer).GetMethod("Deserialize", new Type[]
							{
								typeof(RawDataPool),
								typeof(int),
								dataType.MakeByRefType()
							});
							object methodResult = null;
							bool flag5 = methodInfo == null;
							if (flag5)
							{
								Debug.LogError("deserializer not supported for type |" + dataType.FullName + "|");
							}
							else
							{
								object[] methodArgs = new object[]
								{
									wrapper.DataPool,
									notification.ValueOffset,
									Activator.CreateInstance(dataType)
								};
								methodInfo.Invoke(null, methodArgs);
								methodResult = methodArgs[2];
							}
							this._gameDataReceivingStorage[uid] = methodResult;
							bool flag6 = this._gameDataReceivingStorage.Count == this._gameDataReceivingList.Count;
							if (flag6)
							{
								this._gameDataRecevier(this._gameDataReceivingStorage);
								this.StopGameDataReceiving();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000FC7DC File Offset: 0x000FA9DC
	private void OnClickGetCharId(UI_GMWindow.WidgetParams widgetParams, int key, TMP_InputField input)
	{
		string tip = "";
		int value = -1;
		bool flag = UIElement.CharacterMenu.Exist && UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
		if (flag)
		{
			value = UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().CurCharacterId;
			tip = LocalStringManager.Get("GM_Message_Func_OnClickGetCharId_Tips_1") + ":";
		}
		else
		{
			bool exist = UIElement.EventWindow.Exist;
			if (exist)
			{
				CharacterDisplayData targetCharacter = SingletonObject.getInstance<EventModel>().DisplayingEventData.TargetCharacter;
				bool flag2 = targetCharacter != null;
				if (flag2)
				{
					value = targetCharacter.CharacterId;
					tip = LocalStringManager.Get("GM_Message_Func_OnClickGetCharId_Tips_0") + ":";
				}
			}
		}
		bool flag3 = value < 0;
		if (flag3)
		{
			value = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			tip = LocalStringManager.Get("GM_Message_Func_OnClickGetCharId_Tips_2") + ":";
		}
		this.SetValue(key, value);
		input.text = value.ToString();
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(null, new List<int>
		{
			value
		}, delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayData> displays = null;
			Serializer.Deserialize(dataPool, offset, ref displays);
			string name = NameCenter.GetCharMonasticTitleOrNameByDisplayData(displays[0], false, false);
			this.Log(string.Format("{0}{1}-{2}", tip, name, displays[0].CharacterId));
			input.text = string.Format("{0}({1})", value, name);
		});
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x000FC944 File Offset: 0x000FAB44
	private void OnClickOpenSelectChar(UI_GMWindow.WidgetParams widgetParams, int key, TMP_InputField input)
	{
		this.SelectCharRoot.gameObject.SetActive(true);
		this.SelectCharRoot.GetChild(0).gameObject.SetActive(false);
		TMP_InputField charNameInput = this.SelectCharRoot.Find("MainWindow/Name/Input").GetComponent<TMP_InputField>();
		InfinityScrollLegacy scroll = this.SelectCharRoot.Find("MainWindow/CharScroll").GetComponent<InfinityScrollLegacy>();
		CButtonObsolete btn = this.SelectCharRoot.Find("MainWindow/ConfirmCharacter").GetComponent<CButtonObsolete>();
		CToggleGroupObsolete charTogGroup = scroll.GetComponent<CToggleGroupObsolete>();
		scroll.SetTogGroup(charTogGroup, false, false);
		charTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete _)
		{
			bool flag = togNew != null;
			if (flag)
			{
				scroll.SelectedTogKey = togNew.Key;
			}
		};
		Dictionary<int, string> charNameDict = new Dictionary<int, string>();
		List<int> charIdList = new List<int>();
		charNameInput.onEndEdit.RemoveAllListeners();
		charNameInput.onEndEdit.AddListener(delegate(string str)
		{
			base.<OnClickOpenSelectChar>g__UpdateCharacterList|5();
		});
		scroll.OnItemRender = delegate(int index, Refers refer)
		{
			refer.CGet<TextMeshProUGUI>("NameText").text = charNameDict[charIdList[index]];
		};
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(delegate()
		{
			CToggleObsolete selectedTog = charTogGroup.GetActive();
			bool flag = selectedTog != null;
			if (flag)
			{
				int value = charIdList[selectedTog.Key];
				this.SetValue(key, value);
				input.text = string.Format("{0}({1})", value, charNameDict[value]);
				this.SelectCharRoot.gameObject.SetActive(false);
			}
		});
		CharacterDomainMethod.AsyncCall.GmCmd_GetAllCharacterName(null, delegate(int offset, RawDataPool dataPool)
		{
			List<CharIdAndName> ret = null;
			Serializer.Deserialize(dataPool, offset, ref ret);
			charNameDict = ret.ToDictionary((CharIdAndName a) => a.CharId, (CharIdAndName a) => a.Name);
			base.<OnClickOpenSelectChar>g__UpdateCharacterList|5();
			this.SelectCharRoot.GetChild(0).gameObject.SetActive(true);
		});
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000FCAC1 File Offset: 0x000FACC1
	public void CloseSelectChar()
	{
		this.SelectCharRoot.gameObject.SetActive(false);
		this.SelectItemRoot.gameObject.SetActive(false);
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000FCAE8 File Offset: 0x000FACE8
	private void SetValueOnEndEdit(string str, UI_GMWindow.EValueType valueType, Type argType, int key, bool set2Arg)
	{
		if (valueType > UI_GMWindow.EValueType.Float)
		{
			if (valueType == UI_GMWindow.EValueType.String)
			{
				this.SetValue(key, str);
			}
		}
		else
		{
			bool flag = string.IsNullOrEmpty(str);
			if (flag)
			{
				this._dicParams.Remove(key);
			}
			else
			{
				try
				{
					object value = this.ParseArgValue(argType, str, false);
					this.SetValue(key, value);
				}
				catch (Exception e)
				{
					this.Log(LocalStringManager.GetFormat(LanguageKey.GM_Message_Func_SetValueOnEndEdit_Exception, argType, str, e).SetColor(Color.red));
				}
			}
		}
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000FCB80 File Offset: 0x000FAD80
	private void SetValue(int key, object value)
	{
		this._dicParams[key] = value;
		bool flag = !this._AllowSetValue;
		if (!flag)
		{
			bool flag2 = this._key2Property.ContainsKey(key);
			if (flag2)
			{
				this._key2Property[key].SetValue(null, value);
			}
		}
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000FCBD0 File Offset: 0x000FADD0
	public void SwitchWindow()
	{
		this.Opened = !this.Opened;
		bool opened = this.Opened;
		if (opened)
		{
			GMFunc.RefreshWorldFunctionsStatus();
			this.CommandLine.OnGmWindowOpen();
		}
		else
		{
			GMFunc.ModifyWorldFunctionsStatus();
		}
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x000FCC16 File Offset: 0x000FAE16
	public void CloseWithoutSave()
	{
		this.Opened = false;
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000FCC21 File Offset: 0x000FAE21
	public void SwitchConsoleWindow()
	{
		this.ConsoleOpened = !this.ConsoleOpened;
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000FCC34 File Offset: 0x000FAE34
	public void Log(string str)
	{
		bool flag = this._consoleStrs.Count > 49;
		if (flag)
		{
			this._consoleStrs.RemoveAt(0);
		}
		this._consoleStrs.Add(str);
		this.ConsoleContent.text = str;
		this.RefreshConsoleContent();
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000FCC88 File Offset: 0x000FAE88
	private void RefreshConsoleContent()
	{
		bool flag = !this.ConsoleOpened;
		if (!flag)
		{
			this._sb.Clear();
			foreach (string one in this._consoleStrs)
			{
				this._sb.AppendLine(one);
				this._sb.AppendLine();
			}
			this.TotalConsoleContent.text = this._sb.ToString();
			RectTransform rectTrans = this.TotalConsoleContent.transform.parent as RectTransform;
			rectTrans.sizeDelta = rectTrans.sizeDelta.SetY(this.TotalConsoleContent.preferredHeight);
		}
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x000FCD5C File Offset: 0x000FAF5C
	private string GetMemberName(string memberName)
	{
		LanguageKey key;
		return (Enum.TryParse<LanguageKey>("GM_" + memberName + "_Name", out key) && key != LanguageKey.Invalid) ? LocalStringManager.Get(key) : memberName;
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000FCD94 File Offset: 0x000FAF94
	private string GetMemberTips(string memberName)
	{
		LanguageKey key;
		return (Enum.TryParse<LanguageKey>("GM_" + memberName + "_Tips", out key) && key != LanguageKey.Invalid) ? LocalStringManager.Get(key) : memberName;
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000FCDCC File Offset: 0x000FAFCC
	private string GetArgName(string memberName, int idx, string argName)
	{
		LanguageKey key;
		return (Enum.TryParse<LanguageKey>(string.Format("GM_{0}_Arg{1}_Name", memberName, idx), out key) && key != LanguageKey.Invalid) ? LocalStringManager.Get(key) : memberName;
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000FCE08 File Offset: 0x000FB008
	private string GetArgTips(string memberName, int idx)
	{
		LanguageKey key;
		return (Enum.TryParse<LanguageKey>(string.Format("GM_{0}_Arg{1}_Tips", memberName, idx), out key) && key != LanguageKey.Invalid) ? LocalStringManager.Get(key) : memberName;
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x000FCE44 File Offset: 0x000FB044
	private bool ValidGMWindow()
	{
		bool flag = GameApp.Instance == null;
		return !flag && GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x000FCE78 File Offset: 0x000FB078
	private void SetCurPage(int pageIdx)
	{
		this._curPage = pageIdx;
		this.ContentScrollRect.SetScrollEnable(UI_GMWindow.NeedScrollSet.Contains((EGMPage)pageIdx));
		foreach (object obj in this.PageRoot)
		{
			Transform one = (Transform)obj;
			one.gameObject.SetActive(one.GetSiblingIndex() == pageIdx);
		}
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x000FCF04 File Offset: 0x000FB104
	private static void SetParent(Transform child, Transform parent)
	{
		child.SetParent(parent, false);
		child.localScale = Vector3.one;
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x000FCF1C File Offset: 0x000FB11C
	private void SetWidth(GameObject obj, float width)
	{
		RectTransform rectTransform = obj.transform as RectTransform;
		rectTransform.sizeDelta = new Vector2(width * 1200f, 0f);
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x000FCF50 File Offset: 0x000FB150
	private EWidgetType GetDefaultWidgetType(Type type)
	{
		bool flag = type.Name == "Nullable`1";
		if (flag)
		{
			type = type.GetGenericArguments()[0];
		}
		bool flag2 = type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(long) || type == typeof(ulong);
		EWidgetType result;
		if (flag2)
		{
			result = EWidgetType.IntField;
		}
		else
		{
			bool flag3 = type == typeof(float) || type == typeof(double);
			if (flag3)
			{
				result = EWidgetType.FloatField;
			}
			else
			{
				bool flag4 = type == typeof(string);
				if (flag4)
				{
					result = EWidgetType.StringField;
				}
				else
				{
					bool flag5 = type == typeof(bool);
					if (flag5)
					{
						result = EWidgetType.BoolField;
					}
					else
					{
						result = EWidgetType.Auto;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x000FD084 File Offset: 0x000FB284
	private object[] GetParams(int pageIdx, int memberIdx, int argCnt)
	{
		object[] ret = new object[argCnt];
		for (int i = 0; i < argCnt; i++)
		{
			this._dicParams.TryGetValue(this.GetArgKey(pageIdx, memberIdx, i), out ret[i]);
		}
		return ret;
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000FD0CC File Offset: 0x000FB2CC
	private bool ParamsSetted(int pageIdx, int memberIdx, int argCnt)
	{
		for (int i = 0; i < argCnt; i++)
		{
			int key = this.GetArgKey(pageIdx, memberIdx, i);
			bool flag = !this._dicParams.ContainsKey(key) && !this._argIsNullable[key];
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000FD128 File Offset: 0x000FB328
	private int GetArgKey(int pageIdx, int memberIdx, int argIdx)
	{
		return pageIdx * 1000000 + memberIdx * 1000 + argIdx;
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000FD14C File Offset: 0x000FB34C
	public object ParseArgValue(Type argType, string value, bool usingQuoatedString = false)
	{
		bool flag = argType.Name == "Nullable`1";
		if (flag)
		{
			argType = argType.GenericTypeArguments[0];
		}
		bool isEnum = argType.IsEnum;
		if (isEnum)
		{
			object parsed;
			bool flag2 = Enum.TryParse(argType, value, out parsed);
			if (flag2)
			{
				return parsed;
			}
		}
		bool flag3 = argType == typeof(int);
		object result;
		if (flag3)
		{
			result = int.Parse(value);
		}
		else
		{
			bool flag4 = argType == typeof(uint);
			if (flag4)
			{
				result = uint.Parse(value);
			}
			else
			{
				bool flag5 = argType == typeof(ushort);
				if (flag5)
				{
					result = ushort.Parse(value);
				}
				else
				{
					bool flag6 = argType == typeof(short);
					if (flag6)
					{
						result = short.Parse(value);
					}
					else
					{
						bool flag7 = argType == typeof(byte);
						if (flag7)
						{
							result = byte.Parse(value);
						}
						else
						{
							bool flag8 = argType == typeof(sbyte);
							if (flag8)
							{
								result = sbyte.Parse(value);
							}
							else
							{
								bool flag9 = argType == typeof(bool);
								if (flag9)
								{
									result = bool.Parse(value);
								}
								else
								{
									bool flag10 = argType == typeof(long);
									if (flag10)
									{
										result = long.Parse(value);
									}
									else
									{
										bool flag11 = argType == typeof(ulong);
										if (flag11)
										{
											result = ulong.Parse(value);
										}
										else
										{
											bool flag12 = argType == typeof(float);
											if (flag12)
											{
												result = float.Parse(value);
											}
											else
											{
												bool flag13 = argType == typeof(double);
												if (flag13)
												{
													result = double.Parse(value);
												}
												else
												{
													bool flag14 = argType == typeof(string);
													if (!flag14)
													{
														throw new Exception(string.Format("Error Arg Type:{0}", argType));
													}
													if (usingQuoatedString)
													{
														bool flag15 = value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"';
														if (!flag15)
														{
															throw new Exception(string.Format("Error Arg Type:{0}", argType));
														}
														result = value.Substring(1, value.Length - 2);
													}
													else
													{
														result = value;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x04001A16 RID: 6678
	public static readonly List<Type> GMFuncTypes = new List<Type>();

	// Token: 0x04001A17 RID: 6679
	private static readonly HashSet<EGMPage> NeedScrollSet = new HashSet<EGMPage>
	{
		EGMPage.Character,
		EGMPage.Map,
		EGMPage.Combat,
		EGMPage.Build,
		EGMPage.CombatSkill,
		EGMPage.LifeSkill,
		EGMPage.WorldFunction
	};

	// Token: 0x04001A18 RID: 6680
	public CToggleGroupObsolete PageToggleRoot;

	// Token: 0x04001A19 RID: 6681
	public Transform PageRoot;

	// Token: 0x04001A1A RID: 6682
	public Transform SelfRoot;

	// Token: 0x04001A1B RID: 6683
	public Transform WindowRoot;

	// Token: 0x04001A1C RID: 6684
	public Transform OpenBtn;

	// Token: 0x04001A1D RID: 6685
	public Transform ConsoleRoot;

	// Token: 0x04001A1E RID: 6686
	public Transform WidgetTemplateRoot;

	// Token: 0x04001A1F RID: 6687
	public Transform GMObjRoot;

	// Token: 0x04001A20 RID: 6688
	public Transform SelectCharRoot;

	// Token: 0x04001A21 RID: 6689
	public Transform SelectItemRoot;

	// Token: 0x04001A22 RID: 6690
	public GameObject PageToggle;

	// Token: 0x04001A23 RID: 6691
	public GameObject Page;

	// Token: 0x04001A24 RID: 6692
	public GameObject Group;

	// Token: 0x04001A25 RID: 6693
	public GameObject Line;

	// Token: 0x04001A26 RID: 6694
	public GameObject Btn;

	// Token: 0x04001A27 RID: 6695
	public GameObject InputField;

	// Token: 0x04001A28 RID: 6696
	public GameObject Toggle;

	// Token: 0x04001A29 RID: 6697
	public GameObject CharIdField;

	// Token: 0x04001A2A RID: 6698
	public GameObject ItemTypeIdField;

	// Token: 0x04001A2B RID: 6699
	public GameObject ItemIdField;

	// Token: 0x04001A2C RID: 6700
	public GameObject Label;

	// Token: 0x04001A2D RID: 6701
	public TMP_InputField TotalConsoleContent;

	// Token: 0x04001A2E RID: 6702
	public TextMeshProUGUI ConsoleOpenedTip;

	// Token: 0x04001A2F RID: 6703
	public TMP_InputField ConsoleContent;

	// Token: 0x04001A30 RID: 6704
	public CScrollRectLegacy ContentScrollRect;

	// Token: 0x04001A31 RID: 6705
	public CScrollRectLegacy ConsoleScrollRect;

	// Token: 0x04001A32 RID: 6706
	[Space]
	public GameObject CombatSkillEditor;

	// Token: 0x04001A33 RID: 6707
	public GameObject LifeSkillEditor;

	// Token: 0x04001A34 RID: 6708
	public GameObject CombatEditor;

	// Token: 0x04001A35 RID: 6709
	public GMCharacterEditor CharacterEditor;

	// Token: 0x04001A36 RID: 6710
	public GMCheckAvatarPanel CheckAvatarPanel;

	// Token: 0x04001A37 RID: 6711
	public GameObject CricketPreview;

	// Token: 0x04001A38 RID: 6712
	public InfinityScrollLegacy GMGlobalParameterPanel;

	// Token: 0x04001A39 RID: 6713
	public InfinityScrollLegacy GMElementParameterPanel;

	// Token: 0x04001A3A RID: 6714
	public GMCommandLine CommandLine;

	// Token: 0x04001A3B RID: 6715
	public GMSearchHelper SearchHelper;

	// Token: 0x04001A3C RID: 6716
	public UI_GMWindow.WorldStateCallback OnWorldDataReadyChild;

	// Token: 0x04001A3D RID: 6717
	public UI_GMWindow.WorldStateCallback OnLeaveWorldChild;

	// Token: 0x04001A3E RID: 6718
	private int _curPage;

	// Token: 0x04001A3F RID: 6719
	private bool _AllowSetValue;

	// Token: 0x04001A40 RID: 6720
	private List<MemberInfo>[] _pages;

	// Token: 0x04001A41 RID: 6721
	private Transform[] _pageToggles;

	// Token: 0x04001A42 RID: 6722
	private List<string> _consoleStrs = new List<string>();

	// Token: 0x04001A43 RID: 6723
	private Dictionary<int, object> _dicParams = new Dictionary<int, object>();

	// Token: 0x04001A44 RID: 6724
	public static UI_GMWindow Instance;

	// Token: 0x04001A45 RID: 6725
	private Dictionary<MemberInfo, GameObject> _member2GO = new Dictionary<MemberInfo, GameObject>();

	// Token: 0x04001A46 RID: 6726
	private Dictionary<int, PropertyInfo> _key2Property = new Dictionary<int, PropertyInfo>();

	// Token: 0x04001A47 RID: 6727
	private Dictionary<int, bool> _argIsNullable = new Dictionary<int, bool>();

	// Token: 0x04001A48 RID: 6728
	private StringBuilder _sb = new StringBuilder();

	// Token: 0x04001A49 RID: 6729
	private int? _listenerId = null;

	// Token: 0x04001A4A RID: 6730
	private List<DataUid> _gameDataReceivingList = new List<DataUid>();

	// Token: 0x04001A4B RID: 6731
	private List<Type> _gameDataReceivingTypes = new List<Type>();

	// Token: 0x04001A4C RID: 6732
	private Dictionary<DataUid, object> _gameDataReceivingStorage = new Dictionary<DataUid, object>();

	// Token: 0x04001A4D RID: 6733
	private Action<Dictionary<DataUid, object>> _gameDataRecevier = null;

	// Token: 0x04001A4E RID: 6734
	private float _lastRefreshPropertyTime = 0f;

	// Token: 0x04001A4F RID: 6735
	private static readonly Dictionary<sbyte, string> _CombatType2Name = new Dictionary<sbyte, string>
	{
		{
			0,
			LocalStringManager.Get("LK_Combat_Type_0")
		},
		{
			1,
			LocalStringManager.Get("LK_Combat_Type_1")
		},
		{
			2,
			LocalStringManager.Get("LK_Combat_Type_2")
		},
		{
			3,
			LocalStringManager.Get("LK_Combat_Type_3")
		}
	};

	// Token: 0x04001A50 RID: 6736
	private static readonly Dictionary<sbyte, string> _CombatResultType2Name = new Dictionary<sbyte, string>
	{
		{
			-1,
			LocalStringManager.Get("GM_CombatResult_NoSkip")
		},
		{
			5,
			LocalStringManager.Get("GM_CombatResult_EnemyDie")
		},
		{
			3,
			LocalStringManager.Get("GM_CombatResult_EnemyFlee")
		},
		{
			1,
			LocalStringManager.Get("GM_CombatResult_EnemyWin")
		},
		{
			4,
			LocalStringManager.Get("GM_CombatResult_PlayerDie")
		},
		{
			2,
			LocalStringManager.Get("GM_CombatResult_PlayerFlee")
		},
		{
			0,
			LocalStringManager.Get("GM_CombatResult_PlayerWin")
		},
		{
			sbyte.MaxValue,
			LocalStringManager.Get("GM_CombatResult_Randomize")
		}
	};

	// Token: 0x04001A51 RID: 6737
	private static readonly Dictionary<sbyte, string> _LifeSkillCombatResultType2Name = new Dictionary<sbyte, string>
	{
		{
			-1,
			LocalStringManager.Get("GM_CombatResult_NoSkip")
		},
		{
			0,
			LocalStringManager.Get("GM_CombatResult_EnemyWin")
		},
		{
			1,
			LocalStringManager.Get("GM_CombatResult_PlayerWin")
		},
		{
			2,
			"Full AI"
		},
		{
			3,
			"Full Manual"
		},
		{
			sbyte.MaxValue,
			LocalStringManager.Get("GM_CombatResult_Randomize")
		}
	};

	// Token: 0x04001A52 RID: 6738
	public static readonly Dictionary<short, string> StoryProgress2Name = new Dictionary<short, string>
	{
		{
			0,
			LocalStringManager.Get("GM_MainStoryLine_Beginning")
		},
		{
			1,
			LocalStringManager.Get("GM_MainStoryLine_ExploringValley")
		},
		{
			2,
			LocalStringManager.Get("GM_MainStoryLine_LeavingValley")
		},
		{
			3,
			LocalStringManager.Get("GM_MainStoryLine_EnteringSmallVillage")
		},
		{
			4,
			LocalStringManager.Get("GM_MainStoryLine_ExploringSmallVillage")
		},
		{
			5,
			LocalStringManager.Get("GM_MainStoryLine_LeavingSmallVillage")
		},
		{
			7,
			LocalStringManager.Get("GM_MainStoryLine_EnteringTaiwuVillage")
		},
		{
			8,
			LocalStringManager.Get("GM_MainStoryLine_InheritingTaiwu")
		},
		{
			9,
			LocalStringManager.Get("GM_MainStoryLine_DevelopingTaiwuVillage")
		},
		{
			10,
			LocalStringManager.Get("GM_MainStoryLine_MeetingImmortalXu")
		},
		{
			11,
			LocalStringManager.Get("GM_MainStoryLine_LeavingAncientTomb")
		},
		{
			12,
			LocalStringManager.Get("GM_MainStoryLine_FirstAppearanceOfXiangshuAvatar")
		},
		{
			13,
			LocalStringManager.Get("GM_MainStoryLine_DefeatOfImmortalXu")
		},
		{
			14,
			LocalStringManager.Get("GM_MainStoryLine_VisitOfOldMonk")
		},
		{
			15,
			LocalStringManager.Get("GM_MainStoryLine_LeavingOfOldMonk")
		},
		{
			16,
			LocalStringManager.Get("GM_MainStoryLine_ExploringTheState")
		},
		{
			17,
			LocalStringManager.Get("GM_MainStoryLine_LearningCombatSkill")
		},
		{
			18,
			LocalStringManager.Get("GM_MainStoryLine_ExploringTheWorld")
		},
		{
			19,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar1")
		},
		{
			20,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar2")
		},
		{
			21,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar3")
		},
		{
			22,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar4")
		},
		{
			23,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar5")
		},
		{
			24,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar6")
		},
		{
			25,
			LocalStringManager.Get("GM_MainStoryLine_DefeatingXiangshuAvatar7")
		},
		{
			26,
			LocalStringManager.Get("GM_MainStoryLine_ReturnOfImmortalXu")
		},
		{
			27,
			LocalStringManager.Get("GM_MainStoryLine_SpiritualWanderPlace")
		},
		{
			28,
			LocalStringManager.Get("GM_MainStoryLine_LeaveOfImmortalXu")
		},
		{
			29,
			LocalStringManager.Get("GM_MainStoryLine_FinalRanChenDemon")
		},
		{
			30,
			LocalStringManager.Get("GM_MainStoryLine_FinalRanChenReincarnate")
		},
		{
			31,
			LocalStringManager.Get("GM_MainStoryLine_FinalXiangShuDormant")
		},
		{
			99,
			LocalStringManager.Get("GM_MainStoryLine_GameOver")
		}
	};

	// Token: 0x04001A53 RID: 6739
	[TupleElementNames(new string[]
	{
		"parameterData",
		"parameterValue"
	})]
	private readonly List<ValueTuple<AdventureParameterData, AdventureParameterValue>> _gmGlobalParameterList = new List<ValueTuple<AdventureParameterData, AdventureParameterValue>>();

	// Token: 0x04001A54 RID: 6740
	private static readonly Dictionary<sbyte, string> _CharacterRelationshipTypeType2Name = new Dictionary<sbyte, string>
	{
		{
			RelationType.GetTypeId(16384),
			LocalStringManager.Get("LK_RelationShip_Adored")
		},
		{
			RelationType.GetTypeId(32768),
			LocalStringManager.Get("LK_RelationShip_Enemy")
		},
		{
			RelationType.GetTypeId(8192),
			LocalStringManager.Get("LK_RelationShip_Friend")
		},
		{
			RelationType.GetTypeId(2048),
			LocalStringManager.Get("LK_RelationShip_Mentor")
		},
		{
			RelationType.GetTypeId(1024),
			LocalStringManager.Get("LK_RelationShip_Wife")
		},
		{
			RelationType.GetTypeId(512),
			LocalStringManager.Get("LK_RelationShip_SwornBro")
		},
		{
			RelationType.GetTypeId(1),
			LocalStringManager.Get("LK_RelationShipSpec_Blood")
		},
		{
			RelationType.GetTypeId(8),
			LocalStringManager.Get("LK_RelationShipSpec_Step")
		},
		{
			RelationType.GetTypeId(64),
			LocalStringManager.Get("LK_RelationShipSpec_Adoptive")
		}
	};

	// Token: 0x020014A7 RID: 5287
	private enum EValueType
	{
		// Token: 0x0400A1EF RID: 41455
		Integer,
		// Token: 0x0400A1F0 RID: 41456
		Float,
		// Token: 0x0400A1F1 RID: 41457
		String
	}

	// Token: 0x020014A8 RID: 5288
	private class WidgetParams
	{
		// Token: 0x0600CC86 RID: 52358 RVA: 0x005988E8 File Offset: 0x00596AE8
		public WidgetParams()
		{
		}

		// Token: 0x0600CC87 RID: 52359 RVA: 0x00598914 File Offset: 0x00596B14
		public WidgetParams(UI_GMWindow.WidgetParams src)
		{
			this.Width = src.Width;
			this.OnClick = src.OnClick;
			this.Set2Arg = src.Set2Arg;
			this.Interactable = src.Interactable;
			this.Comment = src.Comment;
		}

		// Token: 0x0400A1F2 RID: 41458
		public float Width;

		// Token: 0x0400A1F3 RID: 41459
		public UnityAction OnClick;

		// Token: 0x0400A1F4 RID: 41460
		public bool Set2Arg = true;

		// Token: 0x0400A1F5 RID: 41461
		public bool Interactable = true;

		// Token: 0x0400A1F6 RID: 41462
		public string Comment = "";

		// Token: 0x0400A1F7 RID: 41463
		public object DefaultValue = null;
	}

	// Token: 0x020014A9 RID: 5289
	// (Invoke) Token: 0x0600CC89 RID: 52361
	public delegate void WorldStateCallback();
}
