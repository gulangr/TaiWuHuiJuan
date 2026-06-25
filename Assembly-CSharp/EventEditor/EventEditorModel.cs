using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Config;
using Config.Common;
using EventEditor.EventScript;
using EventScript;
using FrameWork;
using FrameWork.ExternalTexture;
using GameData.Adventure.Editor;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000628 RID: 1576
	public class EventEditorModel : ISingletonInit, IDisposable
	{
		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x06004A81 RID: 19073 RVA: 0x0022E594 File Offset: 0x0022C794
		// (set) Token: 0x06004A82 RID: 19074 RVA: 0x0022E674 File Offset: 0x0022C874
		public EventScriptRuntimeSettings ScriptRuntimeSettings
		{
			get
			{
				bool flag = this._scriptRuntimeSettings != null;
				EventScriptRuntimeSettings scriptRuntimeSettings;
				if (flag)
				{
					scriptRuntimeSettings = this._scriptRuntimeSettings;
				}
				else
				{
					bool flag2 = File.Exists(this._scriptRuntimeSettingsPath);
					if (flag2)
					{
						try
						{
							string content = File.ReadAllText(this._scriptRuntimeSettingsPath);
							this._scriptRuntimeSettings = JsonConvert.DeserializeObject<EventScriptRuntimeSettings>(content);
							bool flag3 = this._scriptRuntimeSettings.LogScriptTypes.Length != EventScriptType.Instance.Count;
							if (flag3)
							{
								Array.Resize<bool>(ref this._scriptRuntimeSettings.LogScriptTypes, EventScriptType.Instance.Count);
							}
						}
						catch (Exception e)
						{
							Debug.LogWarning(string.Format("Unable to load runtime settings.\n{0}", e));
							this._scriptRuntimeSettings = new EventScriptRuntimeSettings();
						}
					}
					else
					{
						this._scriptRuntimeSettings = new EventScriptRuntimeSettings();
					}
					scriptRuntimeSettings = this._scriptRuntimeSettings;
				}
				return scriptRuntimeSettings;
			}
			set
			{
				this._scriptRuntimeSettings = value;
				string jsonContent = JsonConvert.SerializeObject(this._scriptRuntimeSettings);
				File.WriteAllText(this._scriptRuntimeSettingsPath, jsonContent);
			}
		}

		// Token: 0x06004A83 RID: 19075 RVA: 0x0022E6A4 File Offset: 0x0022C8A4
		public void Init()
		{
			this.DataReady = false;
			this.ModAuthor = "ConchShip";
			this.ModNamespace = "Taiwu";
			this.ModAuthor = PlayerPrefs.GetString("EventAuthorSaveKey", string.Format("Modder_{0}", SteamUser.GetSteamID()));
			bool flag = !PlayerPrefs.HasKey("EventAuthorSaveKey");
			if (flag)
			{
				PlayerPrefs.SetString("EventAuthorSaveKey", this.ModAuthor);
			}
			this.SystemLanguage = "CN";
			this._configIdToRefNameMap = null;
		}

		// Token: 0x06004A84 RID: 19076 RVA: 0x0022E727 File Offset: 0x0022C927
		public void Dispose()
		{
		}

		// Token: 0x06004A85 RID: 19077 RVA: 0x0022E72A File Offset: 0x0022C92A
		public void LoadEventCore()
		{
			EnumDataUpdater.DoLoad();
			SingletonObject.getInstance<YieldHelper>().StartYield(UI_EventEditor.IsDev ? this.LoadEventsCoroutine() : this.LoadEventsForModEventEditor());
		}

		// Token: 0x06004A86 RID: 19078 RVA: 0x0022E754 File Offset: 0x0022C954
		private void InitItemTypeNameMap()
		{
			bool flag = this._itemTypeNameList != null;
			if (!flag)
			{
				this._itemTypeNameList = new List<string>();
				this._itemTypeNames = new Dictionary<string, sbyte>();
				string noneText = LocalStringManager.Get(LanguageKey.LK_None);
				this._itemTypeNameList.Add(noneText);
				this._itemTypeNames.Add(noneText, -1);
				for (sbyte itemType = 0; itemType < 13; itemType += 1)
				{
					string refName = LocalStringManager.Get(string.Format("LK_ItemType_{0}", itemType));
					this._itemTypeNameList.Add(refName);
					this._itemTypeNames.Add(refName, itemType);
				}
			}
		}

		// Token: 0x06004A87 RID: 19079 RVA: 0x0022E7F8 File Offset: 0x0022C9F8
		private void InitWorldFunctionTypeNameMap()
		{
			bool flag = this._worldFunctionTypeNameList != null;
			if (!flag)
			{
				Type type = typeof(WorldFunctionType);
				this._worldFunctionTypeNameList = new List<string>();
				this._worldFunctionTypeNames = new Dictionary<string, byte>();
				List<FieldInfo> constFields = (from field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
				where field.IsLiteral && !field.IsInitOnly
				select field).ToList<FieldInfo>();
				foreach (FieldInfo field2 in constFields)
				{
					this._worldFunctionTypeNameList.Add(field2.Name);
					this._worldFunctionTypeNames.Add(field2.Name, (byte)field2.GetValue(null));
				}
				this._worldFunctionTypeNameList.Sort((string a, string b) => this._worldFunctionTypeNames[a].CompareTo(this._worldFunctionTypeNames[b]));
			}
		}

		// Token: 0x06004A88 RID: 19080 RVA: 0x0022E8F8 File Offset: 0x0022CAF8
		private void InitConfigIdToRefNameMap()
		{
			bool flag = this._configIdToRefNameMap != null;
			if (!flag)
			{
				this._configIdToRefNameMap = new Dictionary<IConfigData, Dictionary<int, string>>();
				EventFunction funcConfigData = EventFunction.Instance;
				this._configIdToRefNameMap.Add(funcConfigData, EventEditorModel.<InitConfigIdToRefNameMap>g__CreateConfigIdToRefNameMap|46_0(funcConfigData));
				foreach (KeyValuePair<string, int> pair in EventArgument.Instance.RefNameMap)
				{
					bool flag2 = pair.Value < 0;
					if (!flag2)
					{
						EventArgumentItem eventArgConfig = EventArgument.Instance[pair.Value];
						bool flag3 = string.IsNullOrEmpty(eventArgConfig.ConfigTable);
						if (!flag3)
						{
							IConfigData configData = ConfigCollection.NameMap[eventArgConfig.ConfigTable];
							bool flag4 = this._configIdToRefNameMap.ContainsKey(configData);
							if (!flag4)
							{
								Dictionary<int, string> map = EventEditorModel.<InitConfigIdToRefNameMap>g__CreateConfigIdToRefNameMap|46_0(configData);
								this._configIdToRefNameMap.Add(configData, map);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004A89 RID: 19081 RVA: 0x0022EA00 File Offset: 0x0022CC00
		private IEnumerator LoadEventsCoroutine()
		{
			this.DataReady = false;
			this.EventGroupInfoDic = new Dictionary<string, string>();
			string eventDataPath = ModManager.GetModEventSaveCore();
			GLog.TagLog("EventEditorModel", "Start Load EventsCore From: " + eventDataPath, Array.Empty<object>());
			bool flag = string.IsNullOrEmpty(eventDataPath) || !Directory.Exists(eventDataPath);
			if (flag)
			{
				this.DataReady = true;
				Action onModelDataReady = this.OnModelDataReady;
				if (onModelDataReady != null)
				{
					onModelDataReady();
				}
				this.OnModelDataReady = null;
				yield break;
			}
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("ShowBlackMask", true);
			box.Set("ShowWaitAnimation", true);
			UIElement.FullScreenMask.SetOnInitArgs(box);
			UIElement.FullScreenMask.Show();
			yield return new WaitUntil(() => UIElement.FullScreenMask.IsInState(EUiElementState.Ready));
			UI_FullScreenMask screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
			GLog.TagLog("EventEditorModel", string.Format("Load Start:{0}", Time.realtimeSinceStartup), Array.Empty<object>());
			this._allEventGroups = new Dictionary<string, EventGroupData>();
			string[] indexFiles = Directory.GetFiles(eventDataPath, "index.lua", SearchOption.AllDirectories);
			bool flag2 = indexFiles.Length != 0;
			if (flag2)
			{
				int num;
				for (int i = 0; i < indexFiles.Length; i = num)
				{
					EventGroupData data = EventGroupData.CreateOrLoad(indexFiles[i].PathFix());
					this.EventGroupInfoDic.Add(data.Key, data.Name);
					this._allEventGroups.Add(data.Key, data);
					bool flag3 = null != screenMask;
					if (flag3)
					{
						screenMask.UpdateMessage(LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_EventGroupLoaded, data.Name));
					}
					bool flag4 = (i + 1) % 10 == 0;
					if (flag4)
					{
						yield return null;
					}
					data = null;
					num = i + 1;
				}
			}
			GLog.TagLog("EventEditorModel", string.Format("Load Complete:{0}", Time.realtimeSinceStartup), Array.Empty<object>());
			string modEventTexturePath = ModManager.GetModEventTexturesFolder();
			SingletonObject.getInstance<TextureCenter>().LoadTextureGroupFromPath<NameKeyTextureGroup>(ModManager.GetWorkingModName(), modEventTexturePath);
			this.DataReady = true;
			Action onModelDataReady2 = this.OnModelDataReady;
			if (onModelDataReady2 != null)
			{
				onModelDataReady2();
			}
			this.OnModelDataReady = null;
			UIElement.FullScreenMask.Hide(false);
			yield break;
		}

		// Token: 0x06004A8A RID: 19082 RVA: 0x0022EA10 File Offset: 0x0022CC10
		public EventGroupData GetGroupData(string groupKey)
		{
			EventGroupData data;
			this._allEventGroups.TryGetValue(groupKey, out data);
			return data;
		}

		// Token: 0x06004A8B RID: 19083 RVA: 0x0022EA34 File Offset: 0x0022CC34
		public EventGroupData AddGroupData(string groupName)
		{
			string corePath = ModManager.GetModEventSaveCore();
			string indexFilePath = Path.Combine(corePath, "index.lua");
			bool flag = !File.Exists(indexFilePath);
			EventGroupData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				EventGroupData data = EventGroupData.CreateOrLoad(indexFilePath);
				this.EventGroupInfoDic.Add(data.Key, data.Name);
				this._allEventGroups.Add(data.Key, data);
				result = data;
			}
			return result;
		}

		// Token: 0x06004A8C RID: 19084 RVA: 0x0022EAA0 File Offset: 0x0022CCA0
		public EventGroupData GetGroupDataByEventGuid(string eventGuid)
		{
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				bool flag = pair.Value.HasEvent(eventGuid);
				if (flag)
				{
					return pair.Value;
				}
			}
			return null;
		}

		// Token: 0x06004A8D RID: 19085 RVA: 0x0022EB14 File Offset: 0x0022CD14
		public List<string> GetAllGroupDataKeyList()
		{
			bool flag = this._allEventGroups != null;
			List<string> result;
			if (flag)
			{
				result = new List<string>(this._allEventGroups.Keys);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06004A8E RID: 19086 RVA: 0x0022EB48 File Offset: 0x0022CD48
		public List<EventGroupData> SearchEventGroup(string searchKey, Predicate<EventGroupData> customSearchHandler = null)
		{
			List<EventGroupData> resultList = new List<EventGroupData>();
			Regex regex = new Regex("[A-Z|a-z|0-9|_]+");
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				EventGroupData data = pair.Value;
				bool flag = customSearchHandler != null && !customSearchHandler(data);
				if (!flag)
				{
					bool flag2 = string.IsNullOrEmpty(searchKey);
					if (flag2)
					{
						resultList.Add(data);
					}
					else
					{
						bool flag3 = regex.IsMatch(data.Key) && data.Key.Contains(searchKey);
						if (flag3)
						{
							resultList.Add(data);
						}
						else
						{
							bool flag4 = data.Name.Contains(searchKey);
							if (flag4)
							{
								resultList.Add(data);
							}
						}
					}
				}
			}
			return resultList;
		}

		// Token: 0x06004A8F RID: 19087 RVA: 0x0022EC40 File Offset: 0x0022CE40
		public void GetEventToEventList(string srcEventGuid, List<ValueTuple<string, string, string>> toEventList, List<ValueTuple<string, List<ValueTuple<string, string, string>>>> optionOnSelectResult)
		{
			EventEditorModel.<>c__DisplayClass53_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.toEventList = toEventList;
			bool flag = CS$<>8__locals1.toEventList == null;
			if (flag)
			{
				CS$<>8__locals1.toEventList = new List<ValueTuple<string, string, string>>();
			}
			bool flag2 = optionOnSelectResult == null;
			if (flag2)
			{
				optionOnSelectResult = new List<ValueTuple<string, List<ValueTuple<string, string, string>>>>();
			}
			CS$<>8__locals1.toEventList.Clear();
			optionOnSelectResult.Clear();
			EventEditorData eventData = this.GetEvent(srcEventGuid);
			bool flag3 = eventData == null;
			if (flag3)
			{
				Debug.LogWarning("Unable to find event " + srcEventGuid + ".");
			}
			else
			{
				Dictionary<int, EventEditorData.Option> optionsTable = eventData.Options;
				foreach (EventEditorData.Option option in optionsTable.Values)
				{
					IReadOnlyList<string> optionToEvents = this.GetOptionToEventList(eventData, option);
					bool flag4 = optionToEvents != null;
					if (flag4)
					{
						List<ValueTuple<string, string, string>> resultList = new List<ValueTuple<string, string, string>>();
						foreach (string eventGuid in optionToEvents)
						{
							EventEditorData nextEventTable = this.GetEvent(eventGuid);
							bool flag5 = nextEventTable == null;
							if (!flag5)
							{
								string eventName = nextEventTable.EventName;
								string content = nextEventTable.EventContent;
								resultList.Add(new ValueTuple<string, string, string>(eventGuid, eventName, content));
							}
						}
						string optionKey = option.OptionKey;
						optionOnSelectResult.Add(new ValueTuple<string, List<ValueTuple<string, string, string>>>(optionKey, resultList));
					}
				}
				string saveDataDir = Save.GetEventSaveDir(eventData);
				string eventGuidString = eventData.EventGuid;
				string scriptUrl = Path.Combine(saveDataDir, eventGuidString + ".cs");
				Dictionary<string, List<string>> functionsDictionary = Export.GetFileFunctionInfos(scriptUrl);
				bool flag6 = functionsDictionary != null;
				if (flag6)
				{
					List<string> checkEventFuncLines;
					bool flag7 = functionsDictionary.TryGetValue("OnCheckEventCondition", out checkEventFuncLines);
					if (flag7)
					{
						this.<GetEventToEventList>g__GetFunctionToEventStringList|53_0(checkEventFuncLines, ref CS$<>8__locals1);
					}
					List<string> eventEnterFuncLines;
					bool flag8 = functionsDictionary.TryGetValue("OnEventEnter", out eventEnterFuncLines);
					if (flag8)
					{
						this.<GetEventToEventList>g__GetFunctionToEventStringList|53_0(eventEnterFuncLines, ref CS$<>8__locals1);
					}
				}
			}
		}

		// Token: 0x06004A90 RID: 19088 RVA: 0x0022EE4C File Offset: 0x0022D04C
		public string GetConfigRefKey(IConfigData config, int templateId)
		{
			this.InitConfigIdToRefNameMap();
			string value;
			return this._configIdToRefNameMap[config].TryGetValue(templateId, out value) ? value : string.Empty;
		}

		// Token: 0x06004A91 RID: 19089 RVA: 0x0022EE84 File Offset: 0x0022D084
		public string GetItemTypeName(sbyte itemType)
		{
			this.InitItemTypeNameMap();
			return (itemType >= 0 && itemType < 13) ? this._itemTypeNameList[(int)(itemType + 1)] : this._itemTypeNameList[0];
		}

		// Token: 0x06004A92 RID: 19090 RVA: 0x0022EEC4 File Offset: 0x0022D0C4
		public sbyte GetItemType(string name)
		{
			this.InitItemTypeNameMap();
			return this._itemTypeNames.GetValueOrDefault(name, -1);
		}

		// Token: 0x06004A93 RID: 19091 RVA: 0x0022EEEC File Offset: 0x0022D0EC
		public bool IsItemTypeNameInRange(string name)
		{
			this.InitItemTypeNameMap();
			return this._itemTypeNames.ContainsKey(name);
		}

		// Token: 0x06004A94 RID: 19092 RVA: 0x0022EF14 File Offset: 0x0022D114
		public IReadOnlyCollection<string> GetItemTypeNames()
		{
			this.InitItemTypeNameMap();
			return this._itemTypeNameList;
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x0022EF34 File Offset: 0x0022D134
		public byte GetWorldFunctionType(string name)
		{
			this.InitWorldFunctionTypeNameMap();
			return this._worldFunctionTypeNames.GetValueOrDefault(name, 0);
		}

		// Token: 0x06004A96 RID: 19094 RVA: 0x0022EF5C File Offset: 0x0022D15C
		public string GetWorldFunctionTypeName(byte worldFunctionType)
		{
			foreach (KeyValuePair<string, byte> pair in this._worldFunctionTypeNames)
			{
				bool flag = pair.Value == worldFunctionType;
				if (flag)
				{
					return pair.Key;
				}
			}
			return string.Empty;
		}

		// Token: 0x06004A97 RID: 19095 RVA: 0x0022EFCC File Offset: 0x0022D1CC
		public IReadOnlyCollection<string> GetWorldFunctionTypeNames()
		{
			this.InitWorldFunctionTypeNameMap();
			return this._worldFunctionTypeNameList;
		}

		// Token: 0x06004A98 RID: 19096 RVA: 0x0022EFEC File Offset: 0x0022D1EC
		public IReadOnlyCollection<string> GetAdventureElementTypeNames()
		{
			AdventureEditorKit.UpdateElementCache();
			return (from id in AdventureEditorKit.GetElementIds()
			select AdventureEditorKit.GetElementFromCache(id.Item1) into element
			where element != null
			select (!string.IsNullOrEmpty(element.Definition)) ? element.Definition : ((element.Name != null) ? element.Name.Value : null) into name
			where name != null
			select name).ToList<string>();
		}

		// Token: 0x06004A99 RID: 19097 RVA: 0x0022F0A0 File Offset: 0x0022D2A0
		public IReadOnlyCollection<string> GetMajorEventIdNames()
		{
			List<AdventureMajorEventSnapshot> data = AdventureEditorKit.GetMajorEvents().ToList<AdventureMajorEventSnapshot>();
			return (from e in data
			select e.Name.Value).ToList<string>();
		}

		// Token: 0x06004A9A RID: 19098 RVA: 0x0022F0EC File Offset: 0x0022D2EC
		public IReadOnlyCollection<string> GetAdventureRemakeTemplateNames()
		{
			List<AdventureSnapshot> data = AdventureEditorKit.GetAdventures().ToList<AdventureSnapshot>();
			return (from e in data
			select e.Name.Value).ToList<string>();
		}

		// Token: 0x06004A9B RID: 19099 RVA: 0x0022F138 File Offset: 0x0022D338
		public void InvalidateTransitionCache(string guid)
		{
			bool flag = string.IsNullOrEmpty(guid);
			if (!flag)
			{
				this._transitionCache.Remove(guid);
			}
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x0022F160 File Offset: 0x0022D360
		public IReadOnlyList<string> GetOptionToEventList(EventEditorData eventData, EventEditorData.Option option)
		{
			bool flag = eventData == null;
			IReadOnlyList<string> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				this._tmpGuidList.Clear();
				this.GetOptionToEventListInternal(eventData, option, this._tmpGuidList);
				result = this._tmpGuidList;
			}
			return result;
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x0022F1A0 File Offset: 0x0022D3A0
		private void GetOptionToEventListInternal(EventEditorData eventData, EventEditorData.Option option, List<string> result)
		{
			bool flag = eventData == null;
			if (!flag)
			{
				this.GetEventTransitions(eventData, this._tmpGuidList);
				bool flag2 = option != null;
				if (flag2)
				{
					this.GetOptionTransitions(eventData, option, this._tmpGuidList);
					bool flag3 = !string.IsNullOrEmpty(option.RedirectTargetOption.Item1) && !string.IsNullOrEmpty(option.RedirectTargetOption.Item2);
					if (flag3)
					{
						EventEditorData redirectEvent = this.GetEvent(option.RedirectTargetOption.Item1);
						bool flag4 = redirectEvent == null;
						if (!flag4)
						{
							foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in redirectEvent.Options)
							{
								int num;
								EventEditorData.Option option2;
								keyValuePair.Deconstruct(out num, out option2);
								EventEditorData.Option redirectOption = option2;
								bool flag5 = redirectOption.Guid == option.RedirectTargetOption.Item2;
								if (flag5)
								{
									this.GetOptionToEventListInternal(redirectEvent, redirectOption, result);
									break;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x0022F2B4 File Offset: 0x0022D4B4
		private void GetEventTransitions(EventEditorData eventData, List<string> resultList)
		{
			string eventGuid = eventData.EventGuid;
			string saveDataDir = Save.GetEventSaveDir(eventData);
			string eventScriptFilePath = Path.Combine(saveDataDir, eventGuid + ".tws");
			bool flag = File.Exists(eventScriptFilePath);
			if (flag)
			{
				JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(eventScriptFilePath)).GetTransitions(eventData.EventGroup, resultList);
			}
			Dictionary<string, List<string>> functionsDictionary = this.GetOptionFunctionDictionary(eventData, null);
			bool flag2 = functionsDictionary != null;
			if (flag2)
			{
				List<string> enterFuncLines;
				bool flag3 = functionsDictionary.TryGetValue("OnEventEnter", out enterFuncLines);
				if (flag3)
				{
					resultList.AddRange(this.GetFunctionReturnStringList(enterFuncLines, functionsDictionary, true));
				}
				List<string> exitFuncLines;
				bool flag4 = functionsDictionary.TryGetValue("OnEventExit", out exitFuncLines);
				if (flag4)
				{
					resultList.AddRange(this.GetFunctionReturnStringList(exitFuncLines, functionsDictionary, true));
				}
			}
		}

		// Token: 0x06004A9F RID: 19103 RVA: 0x0022F364 File Offset: 0x0022D564
		private void GetOptionTransitions(EventEditorData eventData, EventEditorData.Option options, List<string> resultList)
		{
			string optionGuid = options.Guid;
			string saveDataDir = Save.GetEventSaveDir(eventData);
			string eventScriptFilePath = Path.Combine(saveDataDir, optionGuid + ".tws");
			bool flag = File.Exists(eventScriptFilePath);
			if (flag)
			{
				JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(eventScriptFilePath)).GetTransitions(eventData.EventGroup, resultList);
			}
			Dictionary<string, List<string>> functionsDictionary = this.GetOptionFunctionDictionary(eventData, options);
			List<string> selectFuncLines;
			bool flag2 = functionsDictionary != null && functionsDictionary.TryGetValue("OnSelect", out selectFuncLines);
			if (flag2)
			{
				resultList.AddRange(this.GetFunctionReturnStringList(selectFuncLines, functionsDictionary, true));
			}
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x0022F3EC File Offset: 0x0022D5EC
		private Dictionary<string, List<string>> GetOptionFunctionDictionary(EventEditorData eventData, EventEditorData.Option options)
		{
			string saveDataDir = Save.GetEventSaveDir(eventData);
			string eventScriptFilePath = Path.Combine(saveDataDir, eventData.EventGuid + ".cs");
			Dictionary<string, List<string>> functionsDictionary = Export.GetFileFunctionInfos(eventScriptFilePath);
			bool flag = options != null;
			if (flag)
			{
				string optionScriptFilePath = Path.Combine(saveDataDir, options.Guid + ".cs");
				Dictionary<string, List<string>> dic = Export.GetFileFunctionInfos(optionScriptFilePath);
				bool flag2 = dic != null;
				if (flag2)
				{
					if (functionsDictionary == null)
					{
						functionsDictionary = new Dictionary<string, List<string>>();
					}
					functionsDictionary.AddRangeOverride(dic);
				}
			}
			return functionsDictionary;
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x0022F470 File Offset: 0x0022D670
		private List<string> GetFunctionReturnStringList(List<string> lineList, Dictionary<string, List<string>> funcDict, bool withEventHelperApi = false)
		{
			List<string> list = new List<string>();
			bool flag = lineList.Count <= 0;
			List<string> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < lineList.Count; i++)
				{
					MatchCollection guidMatchCollection = EventEditorModel.ReturnGuidRegex.Matches(lineList[i]);
					bool flag2 = guidMatchCollection.Count > 0;
					if (flag2)
					{
						foreach (object obj in guidMatchCollection)
						{
							Match j = (Match)obj;
							list.Add(j.Groups[1].Value);
						}
					}
					MatchCollection funcMatchCollection = EventEditorModel.ReturnFuncRegex.Matches(lineList[i]);
					bool flag3 = funcMatchCollection.Count > 0;
					if (flag3)
					{
						foreach (object obj2 in funcMatchCollection)
						{
							Match k = (Match)obj2;
							string funcName = k.Groups[1].Value;
							List<string> functionLines;
							bool flag4 = funcDict.TryGetValue(funcName, out functionLines);
							if (flag4)
							{
								list.AddRange(this.GetFunctionReturnStringList(functionLines, funcDict, false));
							}
						}
					}
					if (withEventHelperApi)
					{
						MatchCollection setListenerMatchCollection = EventEditorModel.SetEventListenRegex.Matches(lineList[i]);
						bool flag5 = setListenerMatchCollection.Count > 0;
						if (flag5)
						{
							foreach (object obj3 in setListenerMatchCollection)
							{
								Match l = (Match)obj3;
								list.Add(l.Groups[2].Value);
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x0022F67C File Offset: 0x0022D87C
		public EventEditorData GetEvent(string eventGuid)
		{
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				EventEditorData eventTable = pair.Value.GetEvent(eventGuid);
				bool flag = eventTable != null;
				if (flag)
				{
					return eventTable;
				}
			}
			return null;
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x0022F6F0 File Offset: 0x0022D8F0
		public void SaveEventScript(string guid, string path, EventScriptEditorData script)
		{
			this.InvalidateTransitionCache(guid);
			List<EventInstructionEditorData> instructions = script.Instructions;
			bool flag = instructions != null && instructions.Count > 0;
			if (flag)
			{
				File.WriteAllText(path, JsonConvert.SerializeObject(script, Formatting.Indented));
			}
			else
			{
				bool flag2 = File.Exists(path);
				if (flag2)
				{
					File.Delete(path);
				}
			}
		}

		// Token: 0x06004AA4 RID: 19108 RVA: 0x0022F744 File Offset: 0x0022D944
		public string EnsureEventScriptPath(EventEditorData eventData)
		{
			string saveDir = Save.GetEventSaveDir(eventData);
			bool flag = !Directory.Exists(saveDir);
			if (flag)
			{
				string saveInfo = SingletonObject.getInstance<Save>().SaveEvent(eventData);
				bool flag2 = string.IsNullOrEmpty(saveInfo);
				if (flag2)
				{
					string eventName = eventData.EventName;
					string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_SaveEventOK, eventName);
					EventEditorNotes.Instance.AddNote(info);
				}
				else
				{
					EventEditorNotes.Instance.AddNote(saveInfo);
				}
			}
			return saveDir;
		}

		// Token: 0x06004AA5 RID: 19109 RVA: 0x0022F7B8 File Offset: 0x0022D9B8
		public EventScriptEditorData GetEventScript(string eventGuid, string optionGuid)
		{
			EventEditorData curEvent = this.GetEvent(eventGuid);
			bool flag = curEvent == null;
			EventScriptEditorData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string saveDir = this.EnsureEventScriptPath(curEvent);
				bool flag2 = !Directory.Exists(saveDir);
				if (flag2)
				{
					result = null;
				}
				else
				{
					string scriptUrl = Path.Combine(saveDir, (optionGuid.IsNullOrEmpty() ? eventGuid : optionGuid) + ".tws");
					EventScriptEditorData scriptData = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
					result = scriptData;
				}
			}
			return result;
		}

		// Token: 0x06004AA6 RID: 19110 RVA: 0x0022F838 File Offset: 0x0022DA38
		public bool TryGetEventScript(EventScriptId id, out EventScriptEditorData script)
		{
			EventEditorData curEvent = this.GetEvent(id.EventScriptRef.Guid.ToString());
			script = null;
			bool flag = curEvent == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string saveDir = Save.GetEventSaveDir(curEvent);
				bool flag2 = !Directory.Exists(saveDir);
				if (flag2)
				{
					result = false;
				}
				else
				{
					string scriptUrl = Path.Combine(saveDir, id.GetFileName() + ".tws");
					bool flag3 = !File.Exists(scriptUrl);
					if (flag3)
					{
						result = false;
					}
					else
					{
						script = JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl));
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06004AA7 RID: 19111 RVA: 0x0022F8D4 File Offset: 0x0022DAD4
		public void SearchInstructionInGroup(string groupKey, string text, bool searchFuncName, Dictionary<EventScriptId, List<ValueTuple<int, string>>> allResults)
		{
			EventEditorModel.<>c__DisplayClass77_0 CS$<>8__locals1;
			CS$<>8__locals1.text = text;
			CS$<>8__locals1.searchFuncName = searchFuncName;
			CS$<>8__locals1.allResults = allResults;
			EventGroupData groupData;
			bool flag = !this._allEventGroups.TryGetValue(groupKey, out groupData);
			if (!flag)
			{
				CS$<>8__locals1.indexes = new List<ValueTuple<int, string>>();
				foreach (string eventGuid in groupData.AllEventContent.Keys)
				{
					EventEditorData curEvent = this.GetEvent(eventGuid);
					bool flag2 = curEvent == null;
					if (!flag2)
					{
						string saveDir = Save.GetEventSaveDir(curEvent);
						bool flag3 = !Directory.Exists(saveDir);
						if (!flag3)
						{
							EventScriptRef eventRef = new EventScriptRef(eventGuid, null);
							EventScriptId enterId = new EventScriptId(1, eventRef);
							bool flag4 = EventEditorModel.<SearchInstructionInGroup>g__FindInScript|77_0(saveDir, enterId, ref CS$<>8__locals1);
							if (flag4)
							{
								CS$<>8__locals1.indexes = new List<ValueTuple<int, string>>();
							}
							EventScriptId conditionId = new EventScriptId(2, eventRef);
							bool flag5 = EventEditorModel.<SearchInstructionInGroup>g__FindInScript|77_0(saveDir, conditionId, ref CS$<>8__locals1);
							if (flag5)
							{
								CS$<>8__locals1.indexes = new List<ValueTuple<int, string>>();
							}
							Dictionary<int, EventEditorData.Option> options = curEvent.Options;
							bool flag6 = options == null;
							if (!flag6)
							{
								foreach (EventEditorData.Option table in options.Values)
								{
									string optionGuid = table.Guid;
									EventScriptRef optionRef = new EventScriptRef(eventGuid, optionGuid);
									EventScriptId visibleId = new EventScriptId(5, optionRef);
									bool flag7 = EventEditorModel.<SearchInstructionInGroup>g__FindInScript|77_0(saveDir, visibleId, ref CS$<>8__locals1);
									if (flag7)
									{
										CS$<>8__locals1.indexes = new List<ValueTuple<int, string>>();
									}
									EventScriptId availableId = new EventScriptId(4, optionRef);
									bool flag8 = EventEditorModel.<SearchInstructionInGroup>g__FindInScript|77_0(saveDir, availableId, ref CS$<>8__locals1);
									if (flag8)
									{
										CS$<>8__locals1.indexes = new List<ValueTuple<int, string>>();
									}
									EventScriptId optionScriptId = new EventScriptId(3, optionRef);
									bool flag9 = EventEditorModel.<SearchInstructionInGroup>g__FindInScript|77_0(saveDir, optionScriptId, ref CS$<>8__locals1);
									if (flag9)
									{
										CS$<>8__locals1.indexes = new List<ValueTuple<int, string>>();
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06004AA8 RID: 19112 RVA: 0x0022FB04 File Offset: 0x0022DD04
		public string GetEventGuidByEventNamePrioritizingGroup(string groupKey, string eventName)
		{
			bool flag = string.IsNullOrEmpty(groupKey);
			string result;
			if (flag)
			{
				result = this.GetEventGuidByEventName(eventName);
			}
			else
			{
				string guid = this.GetEventGuidByEventName(groupKey, eventName);
				result = (string.IsNullOrEmpty(guid) ? this.GetEventGuidByEventName(eventName) : guid);
			}
			return result;
		}

		// Token: 0x06004AA9 RID: 19113 RVA: 0x0022FB48 File Offset: 0x0022DD48
		public string GetEventGuidByEventName(string eventName)
		{
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				string guid = pair.Value.GetEventGuidByEventName(eventName);
				bool flag = !string.IsNullOrEmpty(guid);
				if (flag)
				{
					return guid;
				}
			}
			return string.Empty;
		}

		// Token: 0x06004AAA RID: 19114 RVA: 0x0022FBC4 File Offset: 0x0022DDC4
		public string GetEventGuidByEventName(string groupKey, string eventName)
		{
			EventGroupData group = this.GetGroupData(groupKey);
			return ((group != null) ? group.GetEventGuidByEventName(eventName) : null) ?? string.Empty;
		}

		// Token: 0x06004AAB RID: 19115 RVA: 0x0022FBF4 File Offset: 0x0022DDF4
		public List<EventEditorData> GetGroupEventList(string eventGroup)
		{
			List<EventEditorData> list = new List<EventEditorData>();
			EventGroupData groupData;
			bool flag = string.IsNullOrEmpty(eventGroup) || !this._allEventGroups.TryGetValue(eventGroup, out groupData);
			List<EventEditorData> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				List<ValueTuple<string, string, string>> eventList = groupData.GetDisplayList(false);
				foreach (ValueTuple<string, string, string> tuple in eventList)
				{
					list.Add(groupData.GetEvent(tuple.Item1));
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x0022FC90 File Offset: 0x0022DE90
		public bool IsGroupKeyExist(string groupKey)
		{
			return this.EventGroupInfoDic.ContainsKey(groupKey);
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x0022FCB0 File Offset: 0x0022DEB0
		public bool IsGroupNameExist(string groupName)
		{
			return this.EventGroupInfoDic.ContainsValue(groupName);
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x0022FCD0 File Offset: 0x0022DED0
		public void SearchEventByName(string key, List<ValueTuple<string, string>> resultList = null)
		{
			bool flag = resultList == null;
			if (flag)
			{
				resultList = new List<ValueTuple<string, string>>();
			}
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				List<ValueTuple<string, string, string>> pairList = pair.Value.GetDisplayList(false);
				foreach (ValueTuple<string, string, string> tuple in pairList)
				{
					bool flag2 = tuple.Item2.Contains(key);
					if (flag2)
					{
						resultList.Add(new ValueTuple<string, string>(tuple.Item1, tuple.Item2));
					}
				}
			}
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x0022FDA8 File Offset: 0x0022DFA8
		public EventEditorData.Option EventAddNewOption(EventEditorData eventData)
		{
			bool flag = eventData == null;
			EventEditorData.Option result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Dictionary<int, EventEditorData.Option> optionsTable = eventData.Options;
				EventEditorData.Option newOptionTable = new EventEditorData.Option();
				string guidString = Guid.NewGuid().ToString();
				newOptionTable.Guid = guidString;
				newOptionTable.OptionKey = string.Format("Option_{0}", guidString.GetHashCode());
				newOptionTable.InternalContent = string.Empty;
				optionsTable.Add(optionsTable.Keys.Prepend(0).Max() + 1, newOptionTable);
				result = newOptionTable;
			}
			return result;
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x0022FE34 File Offset: 0x0022E034
		public EventEditorData CreateNewEvent()
		{
			EventEditorData eventData = new EventEditorData
			{
				EventGuid = Guid.NewGuid().ToString(),
				TmEdit = Save.GetTimeStamp(),
				Dirty = true,
				EventName = string.Empty,
				EventContent = string.Empty,
				TriggerType = "None",
				ForceSingle = false,
				EventOrder = 500,
				ControlMask = false,
				ControlMaskCode = 0,
				MaskTweenTime = 0f
			};
			Dictionary<int, EventEditorData.Option> options = new Dictionary<int, EventEditorData.Option>();
			eventData.Options = options;
			bool flag = EventEditorEventDetail.Instance.CurEvent != null;
			if (flag)
			{
				EventEditorData srcData = EventEditorEventDetail.Instance.CurEvent;
				eventData.EventOrder = srcData.EventOrder;
				eventData.EventType = srcData.EventType;
				eventData.EventGroup = srcData.EventGroup;
				eventData.ForceSingle = srcData.ForceSingle;
				eventData.DecideRole = srcData.DecideRole;
				eventData.TargetRole = srcData.TargetRole;
				eventData.InternalTexture = srcData.InternalTexture;
				eventData.EventTexture = srcData.EventTexture;
				eventData.TexturePath = srcData.TexturePath;
				string prevEventName = srcData.EventName;
				string eventNamePrefix = prevEventName.Substring(0, prevEventName.LastIndexOf('-') + 1);
				string newEventName = prevEventName + "(clone)";
				string text = prevEventName;
				int num = prevEventName.LastIndexOf('-') + 1;
				string eventIndexString = text.Substring(num, text.Length - num);
				int index;
				bool flag2 = int.TryParse(eventIndexString, out index);
				if (flag2)
				{
					newEventName = eventNamePrefix + (index + 1).ToString("D2");
				}
				eventData.EventName = newEventName;
			}
			bool flag3 = !UI_EventEditor.IsDev;
			if (flag3)
			{
				eventData.EventType = "ModEvent";
			}
			return eventData;
		}

		// Token: 0x06004AB1 RID: 19121 RVA: 0x00230008 File Offset: 0x0022E208
		public void AddEvent(EventEditorData eventData)
		{
			string eventGroup = eventData.EventGroup;
			string eventGuid = eventData.EventGuid;
			string groupName;
			bool flag = !this.EventGroupInfoDic.TryGetValue(eventGroup, out groupName);
			if (flag)
			{
				throw new Exception("EventGroupError:" + eventGuid + " can not find groupName folder of eventGroup:" + eventGroup);
			}
			EventGroupData groupData;
			bool flag2 = !this._allEventGroups.TryGetValue(eventGroup, out groupData);
			if (flag2)
			{
				string indexFilePath = Path.Combine(ModManager.GetModEventSaveCore(), groupName, "index.lua");
				groupData = EventGroupData.CreateOrLoad(indexFilePath);
				this._allEventGroups.Add(eventGroup, groupData);
			}
			groupData.AddEvent(eventData, true);
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x0023009C File Offset: 0x0022E29C
		public string SaveEvent(EventEditorData eventData)
		{
			string eventGroup = eventData.EventGroup;
			EventGroupData groupData = null;
			bool flag = !string.IsNullOrEmpty(eventGroup) && !this._allEventGroups.TryGetValue(eventGroup, out groupData);
			if (flag)
			{
				string directory = Save.GetEventSaveDir(eventData);
				DirectoryInfo parent = new DirectoryInfo(directory).Parent;
				string indexFilePath = Path.Combine(((parent != null) ? parent.FullName : null) ?? string.Empty, "index.lua");
				groupData = EventGroupData.CreateOrLoad(indexFilePath);
				groupData.Key = eventGroup;
				groupData.Name = this.EventGroupInfoDic[eventGroup];
				groupData.ExportFlag = eventData.Export;
				this._allEventGroups.Add(groupData.Key, groupData);
			}
			return (groupData != null) ? groupData.SaveEvent(eventData, true) : null;
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x0023015C File Offset: 0x0022E35C
		public void DeleteEvent(string eventGuid)
		{
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				bool flag = pair.Value.HasEvent(eventGuid);
				if (flag)
				{
					pair.Value.RemoveEvent(eventGuid, string.Empty);
					break;
				}
			}
			GEvent.OnEvent(ModEditorEvents.EventDeleted, EasyPool.Get<ArgumentBox>().Set("Guid", eventGuid));
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x002301F0 File Offset: 0x0022E3F0
		public void GenerateOptionCodeFile(EventEditorData eventData, string optionGuid)
		{
			string saveUrl = Path.Combine(Save.GetEventSaveDir(eventData), optionGuid + ".cs");
			bool flag = !string.IsNullOrEmpty(saveUrl);
			if (flag)
			{
				string scriptTemplateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventOptionTemplate.template");
				Regex regex = new Regex("\\$\\{(?<Key>[a-z|A-Z][a-z|A-Z|0-9]+)\\}");
				string scriptString = regex.Replace(File.ReadAllText(scriptTemplateFilePath), delegate(Match match)
				{
					string value = match.Groups["Key"].Value;
					string a = value;
					string result;
					if (!(a == "OptionKey"))
					{
						result = Export.GetScriptReplaceString(match.Groups["Key"].Value, eventData);
					}
					else
					{
						result = optionGuid;
					}
					return result;
				});
				File.WriteAllText(saveUrl, scriptString, new UTF8Encoding(false));
			}
		}

		// Token: 0x06004AB5 RID: 19125 RVA: 0x00230288 File Offset: 0x0022E488
		public bool IsOptionCodeValidity(EventEditorData eventData, string optionGuid)
		{
			string saveUrl = Path.Combine(Save.GetEventSaveDir(eventData), optionGuid + ".cs");
			bool flag = !File.Exists(saveUrl);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Regex regex = new Regex("\\$\\{(?<Key>[a-z|A-Z][a-z|A-Z|0-9]+)\\}");
				string scriptTemplateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventOptionTemplate.template");
				string initString = regex.Replace(File.ReadAllText(scriptTemplateFilePath), delegate(Match match)
				{
					string value = match.Groups["Key"].Value;
					if (!true)
					{
					}
					string result2;
					if (!(value == "OptionKey"))
					{
						result2 = Export.GetScriptReplaceString(match.Groups["Key"].Value, eventData);
					}
					else
					{
						result2 = optionGuid;
					}
					if (!true)
					{
					}
					return result2;
				});
				string eventScriptString = File.ReadAllText(saveUrl);
				result = (initString != eventScriptString);
			}
			return result;
		}

		// Token: 0x06004AB6 RID: 19126 RVA: 0x0023032C File Offset: 0x0022E52C
		public static void ClearDirectory(string targetDir)
		{
			bool flag = !Directory.Exists(targetDir);
			if (!flag)
			{
				string[] files = Directory.GetFiles(targetDir);
				string[] dirs = Directory.GetDirectories(targetDir);
				foreach (string dir in dirs)
				{
					EventEditorModel.ClearDirectory(dir);
					Directory.Delete(dir);
				}
				foreach (string file in files)
				{
					File.SetAttributes(file, FileAttributes.Normal);
					File.Delete(file);
				}
			}
		}

		// Token: 0x06004AB7 RID: 19127 RVA: 0x002303BC File Offset: 0x0022E5BC
		public static int CopyDirectory(string srcDir, string targetDir)
		{
			int result;
			try
			{
				bool flag = !Directory.Exists(targetDir);
				if (flag)
				{
					Directory.CreateDirectory(targetDir);
				}
				string[] files = Directory.GetFiles(srcDir);
				foreach (string file in files)
				{
					string name = Path.GetFileName(file);
					string dest = Path.Combine(targetDir, name);
					File.Copy(file, dest);
				}
				string[] folders = Directory.GetDirectories(srcDir);
				foreach (string folder in folders)
				{
					string name2 = Path.GetFileName(folder);
					string dest2 = Path.Combine(targetDir, name2);
					EventEditorModel.CopyDirectory(folder, dest2);
				}
				result = 1;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				result = -1;
			}
			return result;
		}

		// Token: 0x06004AB8 RID: 19128 RVA: 0x00230490 File Offset: 0x0022E690
		public void CreateEventCodeFile(EventEditorData eventData)
		{
			string eventGuid = eventData.EventGuid;
			string eventScriptFilePath = Path.Combine(Save.GetEventSaveDir(eventData), eventGuid + ".cs");
			bool flag = !File.Exists(eventScriptFilePath);
			if (flag)
			{
				FileInfo scriptFileInfo = new FileInfo(eventScriptFilePath);
				bool flag2 = !Directory.Exists(scriptFileInfo.DirectoryName);
				if (flag2)
				{
					Directory.CreateDirectory(scriptFileInfo.DirectoryName ?? string.Empty);
				}
				string scriptString = this.GetEventCodeText(eventData);
				File.WriteAllText(eventScriptFilePath, scriptString, new UTF8Encoding(false));
			}
		}

		// Token: 0x06004AB9 RID: 19129 RVA: 0x00230514 File Offset: 0x0022E714
		public string GetEventCodeTemplateText()
		{
			string scriptTemplateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventAPITemplate.template");
			return File.ReadAllText(scriptTemplateFilePath);
		}

		// Token: 0x06004ABA RID: 19130 RVA: 0x0023053C File Offset: 0x0022E73C
		public string GetEventCodeText(EventEditorData eventData)
		{
			string scriptTemplateString = this.GetEventCodeTemplateText();
			Regex regex = new Regex("\\$\\{(?<Key>[a-z|A-Z][a-z|A-Z|0-9]+)\\}");
			return regex.Replace(scriptTemplateString, (Match match) => Export.GetScriptReplaceString(match.Groups["Key"].Value, eventData));
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x00230580 File Offset: 0x0022E780
		public bool IsEventCodeValidity(EventEditorData eventTable)
		{
			string eventGuid = eventTable.EventGuid;
			string eventScriptFilePath = Path.Combine(Save.GetEventSaveDir(eventTable), eventGuid + ".cs");
			bool flag = !File.Exists(eventScriptFilePath);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Regex regex = new Regex("\\$\\{(?<Key>[a-z|A-Z][a-z|A-Z|0-9]+)\\}");
				string scriptTemplateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventAPITemplate.template");
				string scriptTemplateString = File.ReadAllText(scriptTemplateFilePath);
				string initString = regex.Replace(scriptTemplateString, (Match match) => Export.GetScriptReplaceString(match.Groups["Key"].Value, eventTable));
				string eventScriptString = File.ReadAllText(eventScriptFilePath);
				result = (initString != eventScriptString);
			}
			return result;
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x00230629 File Offset: 0x0022E829
		private IEnumerator CoroutineExportEvents(List<string> eventGroupList, bool autoCompile, bool ignoreMarkedForCompile = false)
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("ShowBlackMask", true);
			box.Set("ShowWaitAnimation", true);
			UIElement.FullScreenMask.SetOnInitArgs(box);
			UI_FullScreenMask screenMask = null;
			UIElement fullScreenMask = UIElement.FullScreenMask;
			fullScreenMask.OnActive = (Action)Delegate.Combine(fullScreenMask.OnActive, new Action(delegate()
			{
				screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
			}));
			UIElement.FullScreenMask.Show();
			yield return null;
			Export.MetaData metaData2 = new Export.MetaData();
			metaData2.Author = this.ModAuthor;
			metaData2.Namespace = this.ModNamespace;
			metaData2.CsExportFolder = ModManager.GetModEventExportCsFilesFolder();
			metaData2.ScriptExportFolder = ModManager.GetModEventCompileScriptFolder();
			metaData2.LanguageExportFolder = ModManager.GetModEventExportLanguageFilesFolder();
			metaData2.OnExportEvent = delegate(EventEditorData eventData)
			{
				bool flag2 = null != screenMask;
				if (flag2)
				{
					screenMask.UpdateMessage("Exporting:" + eventData.EventName);
				}
			};
			Action<EventGroupData> onExportEventGroupComplete;
			if (!autoCompile)
			{
				onExportEventGroupComplete = null;
			}
			else
			{
				onExportEventGroupComplete = delegate(EventGroupData group)
				{
					group.Compile = true;
				};
			}
			metaData2.OnExportEventGroupComplete = onExportEventGroupComplete;
			metaData2.OnExportEventFailure = new Action<Exception>(this.ShowErrorInfo);
			Export.MetaData metaData = metaData2;
			yield return Export.CoroutineExportEventGroupList(this, metaData, eventGroupList, ignoreMarkedForCompile);
			bool failed = metaData.Failed;
			if (failed)
			{
				yield break;
			}
			UIElement.FullScreenMask.Hide(false);
			bool flag = !autoCompile;
			if (flag)
			{
				TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_ExportComplete), true);
			}
			yield return null;
			this.ExportEventGroupHashset.UnionWith(eventGroupList);
			if (autoCompile)
			{
				this.CompileEventCore();
			}
			yield break;
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x0023064D File Offset: 0x0022E84D
		private void ShowErrorInfo(Exception e)
		{
			Action<Exception> onCompileError = this.OnCompileError;
			if (onCompileError != null)
			{
				onCompileError(e);
			}
			UIElement.FullScreenMask.Hide(false);
		}

		// Token: 0x06004ABE RID: 19134 RVA: 0x0023066F File Offset: 0x0022E86F
		public void QuickTestEventGroup(EventGroupData groupData)
		{
			this.ExportEventGroup(groupData.Key, true);
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x00230680 File Offset: 0x0022E880
		public void ExportEventGroup(string eventGroupKey, bool compile)
		{
			bool flag = string.IsNullOrEmpty(eventGroupKey);
			if (!flag)
			{
				SingletonObject.getInstance<YieldHelper>().StartYield(this.CoroutineExportEvents(new List<string>
				{
					eventGroupKey
				}, compile, false));
			}
		}

		// Token: 0x06004AC0 RID: 19136 RVA: 0x002306BC File Offset: 0x0022E8BC
		public void ExportEventGroups(List<string> eventGroupList, bool compile, bool ignoreMarkedForCompile = false)
		{
			bool flag = eventGroupList == null || eventGroupList.Count <= 0;
			if (!flag)
			{
				SingletonObject.getInstance<YieldHelper>().StartYield(this.CoroutineExportEvents(eventGroupList, compile, ignoreMarkedForCompile));
			}
		}

		// Token: 0x06004AC1 RID: 19137 RVA: 0x002306F8 File Offset: 0x0022E8F8
		public void ExportAllEventGroup(bool compile, bool ignoreMarkedForCompile = false)
		{
			List<string> exportList = new List<string>();
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				bool exportFlag = pair.Value.ExportFlag;
				if (exportFlag)
				{
					exportList.Add(pair.Key);
				}
				else
				{
					pair.Value.Compile = false;
				}
			}
			bool flag = exportList.Count <= 0;
			if (flag)
			{
				TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.LK_EventEditor_ExportAll_Error1_NoExportEventGroup), true);
			}
			else
			{
				if (compile)
				{
					EventEditorModel.ClearDirectory(ModManager.GetModEventCompileDllFolder());
					EventEditorModel.ClearDirectory(ModManager.GetModEventCompileScriptFolder());
				}
				SingletonObject.getInstance<YieldHelper>().StartYield(this.CoroutineExportEvents(exportList, compile, ignoreMarkedForCompile));
			}
		}

		// Token: 0x06004AC2 RID: 19138 RVA: 0x002307DC File Offset: 0x0022E9DC
		public void CompileEventCore()
		{
			DirectoryInfo dataPathDirectoryInfo = new DirectoryInfo(Application.dataPath);
			string backendDllDirectory = string.Empty;
			string targetLanguageFileDirectory = ModManager.GetModEventExportLanguageFilesFolder();
			string gameDataDllFilePath = Path.Combine(new DirectoryInfo(GameApp.GetArchiveDirPath()).Parent.FullName, "Backend/GameData.dll").PathFix();
			string compileProcessFilePath = Path.Combine(dataPathDirectoryInfo.Parent.FullName, "Event", "EventCompiler/TaiwuEventCompiler.exe");
			string exportDllDirectory = ModManager.GetModEventCompileDllFolder();
			List<string> argumentList = new List<string>();
			argumentList.Add("\"" + this.ModNamespace + "\"");
			argumentList.Add("\"" + ModManager.GetModEventExportCsFilesFolder() + "\"");
			argumentList.Add("\"" + gameDataDllFilePath + "\"");
			argumentList.Add("\"" + exportDllDirectory + "\"");
			argumentList.Add("\"" + backendDllDirectory + "\"");
			bool hasCompileTarget = false;
			List<string> compileGroupKeyList = new List<string>();
			foreach (KeyValuePair<string, EventGroupData> pair in this._allEventGroups)
			{
				bool compile = pair.Value.Compile;
				if (compile)
				{
					hasCompileTarget = true;
					argumentList.Add("\"" + pair.Value.Key + "\"");
					argumentList.Add("\"" + pair.Value.Name + "\"");
					compileGroupKeyList.Add(pair.Value.Key);
					pair.Value.Compile = false;
				}
			}
			bool flag = !hasCompileTarget;
			if (flag)
			{
				foreach (KeyValuePair<string, EventGroupData> pair2 in this._allEventGroups)
				{
					bool exportFlag = pair2.Value.ExportFlag;
					if (exportFlag)
					{
						argumentList.Add("\"" + pair2.Value.Key + "\"");
						argumentList.Add("\"" + pair2.Value.Name + "\"");
						compileGroupKeyList.Add(pair2.Value.Key);
					}
				}
			}
			Process process = Process.Start(new ProcessStartInfo
			{
				Arguments = string.Join(" ", argumentList),
				FileName = compileProcessFilePath
			});
			bool flag2 = process != null;
			if (flag2)
			{
				process.WaitForExit();
				bool flag3 = process.ExitCode != 0;
				if (flag3)
				{
					return;
				}
			}
			GEvent.OnEvent(ModEditorEvents.EventCompileComplete, null);
		}

		// Token: 0x06004AC3 RID: 19139 RVA: 0x00230AC8 File Offset: 0x0022ECC8
		public void CompileEventGroupScripts(EventGroupData groupData, string eventGroupPath)
		{
			bool flag = File.Exists(eventGroupPath);
			if (flag)
			{
				File.Delete(eventGroupPath);
			}
			using (FileStream fileStream = File.OpenWrite(eventGroupPath))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					EventEditorModel._scriptCompiler.CompileVersion(binaryWriter);
					int scriptCount = 0;
					List<ValueTuple<string, string, string>> list = groupData.GetDisplayList(true);
					foreach (ValueTuple<string, string, string> eventInfo in list)
					{
						string eventGuid = eventInfo.Item1;
						EventScriptRef eventRef = new EventScriptRef(eventGuid, null);
						EventEditorData eventTable = groupData.GetEvent(eventGuid);
						string saveDir = Save.GetEventSaveDir(eventTable);
						string eventScriptPath = Path.Combine(saveDir, eventGuid + ".tws");
						bool flag2 = File.Exists(eventScriptPath);
						if (flag2)
						{
							EventScriptId id = new EventScriptId(1, eventRef);
							this.CompileScript(eventScriptPath, binaryWriter, id);
							scriptCount++;
						}
						string eventConditionPath = Path.Combine(saveDir, eventGuid + "_condition.tws");
						bool flag3 = File.Exists(eventConditionPath);
						if (flag3)
						{
							EventScriptId id2 = new EventScriptId(2, eventRef);
							this.CompileScript(eventConditionPath, binaryWriter, id2);
							scriptCount++;
						}
						Dictionary<int, EventEditorData.Option> options = eventTable.Options;
						foreach (EventEditorData.Option option in options.Values)
						{
							string optionGuid = option.Guid;
							EventScriptRef optionRef = new EventScriptRef(eventGuid, optionGuid);
							string optionScriptPath = Path.Combine(saveDir, optionGuid + ".tws");
							bool flag4 = File.Exists(optionScriptPath);
							if (flag4)
							{
								EventScriptId id3 = new EventScriptId(3, optionRef);
								this.CompileScript(optionScriptPath, binaryWriter, id3);
								scriptCount++;
							}
							string optionAvailableConditionsPath = Path.Combine(saveDir, optionGuid + "_available.tws");
							bool flag5 = File.Exists(optionAvailableConditionsPath);
							if (flag5)
							{
								EventScriptId id4 = new EventScriptId(4, optionRef);
								this.CompileScript(optionAvailableConditionsPath, binaryWriter, id4);
								scriptCount++;
							}
							string optionVisibleConditionsPath = Path.Combine(saveDir, optionGuid + "_visible.tws");
							bool flag6 = File.Exists(optionVisibleConditionsPath);
							if (flag6)
							{
								EventScriptId id5 = new EventScriptId(5, optionRef);
								this.CompileScript(optionVisibleConditionsPath, binaryWriter, id5);
								scriptCount++;
							}
						}
					}
					bool flag7 = scriptCount <= 0;
					if (flag7)
					{
						binaryWriter.Close();
						fileStream.Close();
						File.Delete(eventGroupPath);
					}
				}
			}
		}

		// Token: 0x06004AC4 RID: 19140 RVA: 0x00230D9C File Offset: 0x0022EF9C
		public void CompileScript(string srcPath, string dstPath, EventScriptId scriptId)
		{
			using (FileStream fileStream = File.OpenWrite(dstPath))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					EventEditorModel._scriptCompiler.CompileVersion(binaryWriter);
					this.CompileScript(srcPath, binaryWriter, scriptId);
				}
			}
		}

		// Token: 0x06004AC5 RID: 19141 RVA: 0x00230E04 File Offset: 0x0022F004
		private void CompileScript(string srcPath, BinaryWriter binaryWriter, EventScriptId scriptId)
		{
			try
			{
				string text = File.ReadAllText(srcPath);
				EventScriptEditorData script = JsonConvert.DeserializeObject<EventScriptEditorData>(text);
				EventEditorModel._scriptCompiler.Compile(binaryWriter, scriptId, script);
			}
			catch (Exception e)
			{
				throw EventScriptCompilerException.CreateAtScript(scriptId, srcPath, e);
			}
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x00230E50 File Offset: 0x0022F050
		public void CompileGlobalScripts()
		{
			DirectoryInfo dataPathDirectoryInfo = new DirectoryInfo(Application.dataPath);
			string targetCompiledGlobalScriptPath = ModManager.GetModEventExportCompiledGlobalScripts();
			string folderPath = ModManager.GetModEventGlobalScriptsFolder();
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
			bool flag = Directory.Exists(targetCompiledGlobalScriptPath);
			if (flag)
			{
				Directory.Delete(targetCompiledGlobalScriptPath, true);
			}
			Directory.CreateDirectory(targetCompiledGlobalScriptPath);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				bool flag2 = fileInfo.Extension != ".tws";
				if (!flag2)
				{
					string scriptGuid = Path.GetFileNameWithoutExtension(fileInfo.Name);
					EventScriptRef scriptRef = new EventScriptRef(scriptGuid, null);
					Guid guid;
					bool flag3 = !Guid.TryParse(scriptGuid, out guid);
					if (!flag3)
					{
						string dstPath = Path.Combine(targetCompiledGlobalScriptPath, scriptGuid + ".twes");
						this.CompileScript(fileInfo.FullName, dstPath, new EventScriptId(0, scriptRef));
					}
				}
			}
			GEvent.OnEvent(ModEditorEvents.EventCompileComplete, null);
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x00230F44 File Offset: 0x0022F144
		private void CopyConchShipEventsResultToTest(List<string> eventGroupKeyList)
		{
			string srcLanguageDir = ModManager.GetModEventExportLanguageFilesFolder();
			string srcDllDirectory = ModManager.GetModEventCompileDllFolder();
			string srcScriptDirectory = ModManager.GetModEventCompileScriptFolder();
			DirectoryInfo dataPathDirectoryInfo = new DirectoryInfo(Application.dataPath);
			string rootPath = dataPathDirectoryInfo.Parent.FullName;
			string dirName = "Event";
			string targetLanguageFileDirectory = Path.Combine(rootPath, dirName, "EventLanguages");
			string targetDllDirectory = Path.Combine(rootPath, dirName, "EventLib");
			string targetScriptDirectory = Path.Combine(rootPath, dirName, "EventScript");
			bool flag = !Directory.Exists(targetDllDirectory);
			if (flag)
			{
				Directory.CreateDirectory(targetDllDirectory);
			}
			bool flag2 = !Directory.Exists(targetLanguageFileDirectory);
			if (flag2)
			{
				Directory.CreateDirectory(targetLanguageFileDirectory);
			}
			bool flag3 = !Directory.Exists(targetScriptDirectory);
			if (flag3)
			{
				Directory.CreateDirectory(targetScriptDirectory);
			}
			string dlcDirectory = string.Empty;
			for (int i = 0; i < eventGroupKeyList.Count; i++)
			{
				string groupName = eventGroupKeyList[i];
				string packageName = this.ModNamespace + "_EventPackage_" + groupName;
				string srcDllFilePath = Path.Combine(srcDllDirectory, packageName + ".dll").PathFix();
				string srcScriptPath = Path.Combine(srcScriptDirectory, packageName + ".twes").PathFix();
				string dlcName;
				bool flag4 = EventEditorModel.DlcEventGroupMap.TryGetValue(eventGroupKeyList[i], out dlcName);
				if (flag4)
				{
					dlcDirectory = Path.Combine(dataPathDirectoryInfo.Parent.FullName, string.Concat(new string[]
					{
						"Dlc/",
						dlcName,
						"/Event/",
						dlcName,
						"/Events"
					}));
				}
				bool flag5 = File.Exists(srcDllFilePath);
				if (flag5)
				{
					string targetDllFilePath = Path.Combine(targetDllDirectory, Path.GetFileName(srcDllFilePath));
					bool flag6 = !string.IsNullOrEmpty(dlcDirectory);
					if (flag6)
					{
						targetDllFilePath = Path.Combine(dlcDirectory, "EventLib", Path.GetFileName(srcDllFilePath));
						string dlcEventLibDirectory = Path.GetDirectoryName(targetDllFilePath);
						bool flag7 = !Directory.Exists(dlcEventLibDirectory);
						if (flag7)
						{
							Directory.CreateDirectory(dlcEventLibDirectory);
						}
					}
					File.Copy(srcDllFilePath, targetDllFilePath, true);
				}
				string targetScriptPath = Path.Combine(targetScriptDirectory, packageName + ".twes");
				bool flag8 = !string.IsNullOrEmpty(dlcDirectory);
				if (flag8)
				{
					targetScriptPath = Path.Combine(dlcDirectory, "EventScript", packageName);
					string dlcEventScriptDirectory = Path.GetDirectoryName(targetScriptPath);
					bool flag9 = !Directory.Exists(dlcEventScriptDirectory);
					if (flag9)
					{
						Directory.CreateDirectory(dlcEventScriptDirectory);
					}
				}
				bool flag10 = File.Exists(srcScriptPath);
				if (flag10)
				{
					File.Copy(srcScriptPath, targetScriptPath, true);
				}
				else
				{
					bool flag11 = File.Exists(targetScriptPath);
					if (flag11)
					{
						File.Delete(targetScriptPath);
					}
				}
				string srcLanguageFilePath = Path.Combine(srcLanguageDir, packageName + "_Language_" + this.SystemLanguage.ToUpper() + ".txt").PathFix();
				bool flag12 = File.Exists(srcLanguageFilePath);
				if (flag12)
				{
					string targetLanguageFilePath = Path.Combine(targetLanguageFileDirectory, Path.GetFileName(srcLanguageFilePath));
					bool flag13 = !string.IsNullOrEmpty(dlcDirectory);
					if (flag13)
					{
						targetLanguageFilePath = Path.Combine(dlcDirectory, "EventLanguages", Path.GetFileName(srcDllFilePath));
						string dlcEventLanguageDirectory = Path.GetDirectoryName(targetLanguageFilePath);
						bool flag14 = !Directory.Exists(dlcEventLanguageDirectory);
						if (flag14)
						{
							Directory.CreateDirectory(dlcEventLanguageDirectory);
						}
					}
					File.Copy(srcLanguageFilePath, targetLanguageFilePath, true);
				}
				dlcDirectory = string.Empty;
			}
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x00231269 File Offset: 0x0022F469
		public string PublishCurrentModEventCore()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06004AC9 RID: 19145 RVA: 0x00231271 File Offset: 0x0022F471
		public void CreateLocalTestMod(string publishDir)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06004ACA RID: 19146 RVA: 0x00231279 File Offset: 0x0022F479
		private static void ShowDownloadNotice()
		{
			EventEditorNotify.Instance.SetLinkDelegate(new Action<string>(EventEditorModel.<ShowDownloadNotice>g__ClickHandler|115_0));
			EventEditorNotify.Instance.SetNotifyAndShow(LocalStringManager.Get(LanguageKey.LK_EventEditor_DownloadExampleNotice));
		}

		// Token: 0x06004ACB RID: 19147 RVA: 0x002312A9 File Offset: 0x0022F4A9
		private IEnumerator LoadEventsForModEventEditor()
		{
			this.EventGroupInfoDic = new Dictionary<string, string>();
			bool flag = !PlayerPrefs.HasKey("LastWorkingModName") && !EventEditorModel._noticedDownloadExample;
			if (flag)
			{
				EventEditorModel.ShowDownloadNotice();
				yield return new WaitUntil(() => !EventEditorNotify.Instance.gameObject.activeSelf);
			}
			EventEditorModel._noticedDownloadExample = true;
			yield return new WaitUntil(() => EventEditorModel._noticedDownloadExample);
			string eventDataPath = ModManager.GetModEventSaveCore();
			GLog.TagLog("EventEditorModel", "Start Load EventsCore From: " + eventDataPath, Array.Empty<object>());
			bool flag2 = string.IsNullOrEmpty(eventDataPath) || !Directory.Exists(eventDataPath);
			if (flag2)
			{
				this.DataReady = true;
				Action onModelDataReady = this.OnModelDataReady;
				if (onModelDataReady != null)
				{
					onModelDataReady();
				}
				this.OnModelDataReady = null;
				yield break;
			}
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("ShowBlackMask", true);
			box.Set("ShowWaitAnimation", true);
			UIElement.FullScreenMask.SetOnInitArgs(box);
			UIElement.FullScreenMask.Show();
			yield return new WaitUntil(() => UIElement.FullScreenMask.IsInState(EUiElementState.Ready));
			UI_FullScreenMask screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
			GLog.TagLog("EventEditorModel", string.Format("ModEvents {0} Load Start:{1}", eventDataPath, Time.realtimeSinceStartup), Array.Empty<object>());
			string[] indexFiles = Directory.GetFiles(eventDataPath, "index.lua", SearchOption.AllDirectories);
			this._allEventGroups = new Dictionary<string, EventGroupData>();
			List<ValueTuple<EventGroupData, sbyte>> invalidEventGroups = new List<ValueTuple<EventGroupData, sbyte>>();
			bool flag3 = indexFiles.Length != 0;
			if (flag3)
			{
				int num;
				for (int i = 0; i < indexFiles.Length; i = num)
				{
					EventGroupData data = EventGroupData.CreateOrLoad(indexFiles[i].PathFix());
					bool flag4 = !this.IsValidEventGroupKey(data.Key);
					if (flag4)
					{
						invalidEventGroups.Add(new ValueTuple<EventGroupData, sbyte>(data, 0));
						screenMask.UpdateMessage(LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidKey_Used, eventDataPath + "/" + data.Name));
						yield return null;
					}
					else
					{
						bool flag5 = !this.IsValidEventGroupName(data.Name);
						if (flag5)
						{
							invalidEventGroups.Add(new ValueTuple<EventGroupData, sbyte>(data, 1));
							screenMask.UpdateMessage(LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidName_Used, eventDataPath + "/" + data.Name));
							yield return null;
						}
						else
						{
							this.EventGroupInfoDic.Add(data.Key, data.Name);
							this._allEventGroups.Add(data.Key, data);
							screenMask.UpdateMessage(LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_EventGroupLoaded, data.Name));
							bool flag6 = (i + 1) % 30 == 0;
							if (flag6)
							{
								yield return null;
							}
							data = null;
						}
					}
					num = i + 1;
				}
			}
			GLog.TagLog("EventEditorModel", string.Format("ModEvents Load Complete:{0}", Time.realtimeSinceStartup), Array.Empty<object>());
			string modEventTexturePath = ModManager.GetModEventTexturesFolder();
			SingletonObject.getInstance<TextureCenter>().LoadTextureGroupFromPath<NameKeyTextureGroup>(ModManager.GetWorkingModName(), modEventTexturePath);
			this.DataReady = true;
			Action onModelDataReady2 = this.OnModelDataReady;
			if (onModelDataReady2 != null)
			{
				onModelDataReady2();
			}
			this.OnModelDataReady = null;
			yield return null;
			bool flag7 = invalidEventGroups.Count > 0;
			if (flag7)
			{
				int j = 0;
				int max = invalidEventGroups.Count;
				while (j < max)
				{
					ValueTuple<EventGroupData, sbyte> valueTuple = invalidEventGroups[j];
					EventGroupData data2 = valueTuple.Item1;
					sbyte code = valueTuple.Item2;
					string message = string.Empty;
					bool flag8 = code == 0;
					if (flag8)
					{
						message = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_LoadEventGroupFail_Key, data2.Key, data2.Name);
					}
					else
					{
						bool flag9 = code == 1;
						if (flag9)
						{
							message = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_LoadEventGroupFail_Name, data2.Name, data2.Name);
						}
					}
					EventEditorNotes.Instance.AddNote(message);
					data2 = null;
					message = null;
					int num = j + 1;
					j = num;
				}
			}
			UIElement.FullScreenMask.Hide(false);
			yield break;
		}

		// Token: 0x06004ACC RID: 19148 RVA: 0x002312B8 File Offset: 0x0022F4B8
		public bool IsValidEventGroupKey(string key)
		{
			return EventEditorModel._forbidEventGroups == null || !EventEditorModel._forbidEventGroups.ContainsKey(key);
		}

		// Token: 0x06004ACD RID: 19149 RVA: 0x002312E4 File Offset: 0x0022F4E4
		public bool IsValidEventGroupName(string name)
		{
			return EventEditorModel._forbidEventGroups == null || !EventEditorModel._forbidEventGroups.ContainsValue(name);
		}

		// Token: 0x06004ACE RID: 19150 RVA: 0x0023130E File Offset: 0x0022F50E
		public void SetForbidEventGroupMap(Dictionary<string, string> forbidMap)
		{
			EventEditorModel._forbidEventGroups = forbidMap;
		}

		// Token: 0x06004AD2 RID: 19154 RVA: 0x00231438 File Offset: 0x0022F638
		[CompilerGenerated]
		internal static Dictionary<int, string> <InitConfigIdToRefNameMap>g__CreateConfigIdToRefNameMap|46_0(IConfigData configData)
		{
			Dictionary<int, string> dict = new Dictionary<int, string>();
			foreach (KeyValuePair<string, int> pair in configData.RefNameMap)
			{
				dict.Add(pair.Value, pair.Key);
			}
			return dict;
		}

		// Token: 0x06004AD3 RID: 19155 RVA: 0x002314A4 File Offset: 0x0022F6A4
		[CompilerGenerated]
		private void <GetEventToEventList>g__GetFunctionToEventStringList|53_0(List<string> lineList, ref EventEditorModel.<>c__DisplayClass53_0 A_2)
		{
			bool flag = lineList.Count <= 0;
			if (!flag)
			{
				for (int i = 0; i < lineList.Count; i++)
				{
					MatchCollection guidMatchCollection = EventEditorModel.ToEventRegex.Matches(lineList[i]);
					bool flag2 = guidMatchCollection.Count > 0;
					if (flag2)
					{
						foreach (object obj in guidMatchCollection)
						{
							Match j = (Match)obj;
							string eventGuid = j.Groups[1].Value;
							EventEditorData toEvent = this.GetEvent(eventGuid);
							string eventName = toEvent.EventName;
							string content = toEvent.EventContent;
							A_2.toEventList.Add(new ValueTuple<string, string, string>(eventGuid, eventName, content));
						}
					}
				}
			}
		}

		// Token: 0x06004AD4 RID: 19156 RVA: 0x002315A0 File Offset: 0x0022F7A0
		[CompilerGenerated]
		internal static bool <SearchInstructionInGroup>g__FindInScript|77_0(string dir, EventScriptId id, ref EventEditorModel.<>c__DisplayClass77_0 A_2)
		{
			string filePath = Path.Combine(dir, id.GetFileName() + ".tws");
			bool flag = File.Exists(filePath);
			if (flag)
			{
				EventScriptEditorData script = JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(filePath));
				script.FindInstructions(A_2.text, A_2.searchFuncName, A_2.indexes);
				bool flag2 = A_2.indexes.Count > 0;
				if (flag2)
				{
					A_2.allResults.Add(id, A_2.indexes);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AD5 RID: 19157 RVA: 0x0023162C File Offset: 0x0022F82C
		[CompilerGenerated]
		internal static void <ShowDownloadNotice>g__ClickHandler|115_0(string linkId)
		{
			bool flag = "Document" == linkId;
			if (flag)
			{
				Application.OpenURL("https://event.conchship.com.cn/");
			}
			else
			{
				bool flag2 = "Example" == linkId;
				if (flag2)
				{
					Application.OpenURL("https://file.conchship.com.cn/event/ConchShipEventEditorBootCamp.zip");
				}
				else
				{
					bool flag3 = "FAQ" == linkId;
					if (flag3)
					{
						Application.OpenURL("https://event.conchship.com.cn/");
					}
				}
			}
		}

		// Token: 0x040033B0 RID: 13232
		public const string SaveDataPathKey = "EventEditorSaveDataPath";

		// Token: 0x040033B1 RID: 13233
		public const string CustomBgPathKey = "EventEditorCustomBackgroudPath";

		// Token: 0x040033B2 RID: 13234
		public const string EventScriptDirectoryKey = "EventEditorScriptPath";

		// Token: 0x040033B3 RID: 13235
		public const string AvatarPresetPathKey = "EventEditorAvatarPresetPath";

		// Token: 0x040033B4 RID: 13236
		public const string AutoSaveKey = "EventEditorAutoSave";

		// Token: 0x040033B5 RID: 13237
		public const string LastAudioPathKey = "LastSelectAudioKey";

		// Token: 0x040033B6 RID: 13238
		public const string CodeEditorPath = "EventEditorVsCodeExePath";

		// Token: 0x040033B7 RID: 13239
		public const string ConchShipEventAuthor = "ConchShip";

		// Token: 0x040033B8 RID: 13240
		public const string EventAuthorSaveKey = "EventAuthorSaveKey";

		// Token: 0x040033B9 RID: 13241
		public const string ConchShipEventNamespace = "Taiwu";

		// Token: 0x040033BA RID: 13242
		public const string CompilerFilePath = "EventCompiler/TaiwuEventCompiler.exe";

		// Token: 0x040033BB RID: 13243
		public const string LastEditingEventGroupKey = "LastEditingEventGroupKey";

		// Token: 0x040033BC RID: 13244
		public const string LastEventGroupSearchKey = "LastEventGroupSearchKey";

		// Token: 0x040033BD RID: 13245
		private Dictionary<string, EventGroupData> _allEventGroups;

		// Token: 0x040033BE RID: 13246
		public Action OnModelDataReady;

		// Token: 0x040033BF RID: 13247
		public Action<Exception> OnCompileError;

		// Token: 0x040033C0 RID: 13248
		private static readonly Regex ReturnGuidRegex = new Regex("return \"([0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12})\";");

		// Token: 0x040033C1 RID: 13249
		private static readonly Regex ReturnFuncRegex = new Regex("return ([\\w]+)\\((.*)\\);");

		// Token: 0x040033C2 RID: 13250
		private static readonly Regex SetEventListenRegex = new Regex("EventHelper.[\\w]+\\(([\\s\\S]+,)?\"([0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12})\"(,[\\s\\S]+)?\\);");

		// Token: 0x040033C3 RID: 13251
		private static readonly Regex ToEventRegex = new Regex("EventHelper.ToEvent\\(\"([a-z0-9-]+)\"\\);");

		// Token: 0x040033C4 RID: 13252
		public HashSet<string> ExportEventGroupHashset = new HashSet<string>();

		// Token: 0x040033C5 RID: 13253
		public Dictionary<string, string> EventGroupInfoDic;

		// Token: 0x040033C6 RID: 13254
		public Dictionary<string, EventOptionConditionConfig> EventOptionConditionsConfig;

		// Token: 0x040033C7 RID: 13255
		public string SystemLanguage;

		// Token: 0x040033C8 RID: 13256
		public string ModAuthor;

		// Token: 0x040033C9 RID: 13257
		public string ModNamespace;

		// Token: 0x040033CA RID: 13258
		public bool DataReady;

		// Token: 0x040033CB RID: 13259
		private readonly Dictionary<string, List<string>> _transitionCache = new Dictionary<string, List<string>>();

		// Token: 0x040033CC RID: 13260
		private Dictionary<IConfigData, Dictionary<int, string>> _configIdToRefNameMap;

		// Token: 0x040033CD RID: 13261
		private List<string> _tmpGuidList = new List<string>();

		// Token: 0x040033CE RID: 13262
		private StringBuilder _stringBuilder = new StringBuilder();

		// Token: 0x040033CF RID: 13263
		private static IEventScriptCompiler _scriptCompiler = new EventScriptCompiler_0_0_2_0();

		// Token: 0x040033D0 RID: 13264
		private EventScriptRuntimeSettings _scriptRuntimeSettings;

		// Token: 0x040033D1 RID: 13265
		private readonly string _scriptRuntimeSettingsPath = Path.Combine(GameApp.GetArchiveDirPath(), "EventScriptRuntimeSettings.json");

		// Token: 0x040033D2 RID: 13266
		private List<string> _itemTypeNameList;

		// Token: 0x040033D3 RID: 13267
		private Dictionary<string, sbyte> _itemTypeNames;

		// Token: 0x040033D4 RID: 13268
		private List<string> _worldFunctionTypeNameList;

		// Token: 0x040033D5 RID: 13269
		private Dictionary<string, byte> _worldFunctionTypeNames;

		// Token: 0x040033D6 RID: 13270
		private static readonly Dictionary<string, string> DlcEventGroupMap = new Dictionary<string, string>
		{
			{
				"CharacterInteraction_LoveDlc_DateNpcOne",
				"InteractOfLove"
			},
			{
				"CharacterInteraction_LoveDlc_DateNpcTwo",
				"InteractOfLove"
			},
			{
				"loveinteract",
				"InteractOfLove"
			}
		};

		// Token: 0x040033D7 RID: 13271
		private static Dictionary<string, string> _forbidEventGroups;

		// Token: 0x040033D8 RID: 13272
		private static bool _noticedDownloadExample = false;
	}
}
