using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x0200093E RID: 2366
	public class ViewMapBlockCharList : UIBase, IMapBlockCharHolder, IMapBlockCharShortCutsParent, IMapBlockCharDataSource
	{
		// Token: 0x06006E38 RID: 28216 RVA: 0x0032EE8C File Offset: 0x0032D08C
		public void SetDataCount(int count, bool refresh = false)
		{
			this.charScroll.SetDataCount(count, refresh);
		}

		// Token: 0x17000CBA RID: 3258
		// (get) Token: 0x06006E39 RID: 28217 RVA: 0x0032EE9D File Offset: 0x0032D09D
		public int ActiveKey
		{
			get
			{
				return this.selector.GetActiveIndex();
			}
		}

		// Token: 0x06006E3A RID: 28218 RVA: 0x0032EEAC File Offset: 0x0032D0AC
		private void Awake()
		{
			this.selector.Init(0);
			this.selector.OnActiveIndexChange += delegate(int togNew, int _)
			{
				this.normalCharacterFilter.gameObject.SetActive(togNew == 0);
				bool flag = togNew == 0;
				if (flag)
				{
					this.ReapplyFilter(false, true);
				}
				else
				{
					bool flag2 = this._blockData != null;
					if (flag2)
					{
						this.SetDataCount(this.GetCount(togNew), true);
					}
					else
					{
						this.SetDataCount(0, false);
					}
				}
			};
			this.SetDataCount(0, false);
			this.characterFilter.onSelect.ResetListener(new Action<string>(this.BeginEdit));
			this.characterFilter.onDeselect.ResetListener(new Action<string>(this.EndEdit));
			this.characterFilter.onValueChanged.ResetListener(new Action<string>(this.SetSearchResult));
			this.characterFilter.onEndEdit.ResetListener(new Action<string>(this.SetSearchResult));
			this.characterFilterClearButton.onClick.ResetListener(new Action(this.ClearSearch));
			this._normalCharacterFilterController = new CharacterDisplayDataSortAndFilterController(this.normalCharacterFilter, LanguageKey.LK_CommonSortAndFilter_FilterPanel_Title_Character);
			this._normalCharacterFilterController.Init(new Action(this.RefreshOrg), "MapBlockFilter");
			this.AwakeDetails();
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.HideMapBlockCharList));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.ShowMapBlockCharList));
			GEvent.Add(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.ClearSearch));
		}

		// Token: 0x06006E3B RID: 28219 RVA: 0x0032F004 File Offset: 0x0032D204
		private void ClearSearch()
		{
			this.characterFilter.text = "";
			this.RefreshSelectedIndex(true);
		}

		// Token: 0x06006E3C RID: 28220 RVA: 0x0032F020 File Offset: 0x0032D220
		private void ClearSearch(ArgumentBox _)
		{
			this.ClearSearch();
		}

		// Token: 0x06006E3D RID: 28221 RVA: 0x0032F02C File Offset: 0x0032D22C
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.HideMapBlockCharList));
			GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.ShowMapBlockCharList));
			GEvent.Remove(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.ClearSearch));
		}

		// Token: 0x06006E3E RID: 28222 RVA: 0x0032F08C File Offset: 0x0032D28C
		private int GetCount(int active)
		{
			int result;
			if (this._isHideCharacterSet)
			{
				result = 0;
			}
			else
			{
				if (!true)
				{
				}
				int num2;
				switch (active)
				{
				case 0:
				{
					List<CharacterDisplayData> specialCharacters = this._blockData.SpecialCharacters;
					int num = (specialCharacters != null) ? specialCharacters.Count : 0;
					List<CharacterDisplayData> normalCharacters = this._blockData.NormalCharacters;
					num2 = num + ((normalCharacters != null) ? normalCharacters.Count : 0);
					break;
				}
				case 1:
				{
					List<CharacterDisplayData> infectedCharacters = this._blockData.InfectedCharacters;
					int num3 = (infectedCharacters != null) ? infectedCharacters.Count : 0;
					List<CharacterDisplayData> enemyCharacters = this._blockData.EnemyCharacters;
					int num4 = num3 + ((enemyCharacters != null) ? enemyCharacters.Count : 0);
					List<Animal> animals = this._blockData.Animals;
					int num5 = num4 + ((animals != null) ? animals.Count : 0);
					List<MapTemplateEnemyInfo> randomEnemies = this._blockData.RandomEnemies;
					num2 = num5 + ((randomEnemies != null) ? randomEnemies.Count : 0);
					break;
				}
				case 2:
				{
					List<CaravanDisplayData> caravans = this._blockData.Caravans;
					num2 = ((caravans != null) ? caravans.Count : 0);
					break;
				}
				case 3:
				{
					List<GraveDisplayData> graves = this._blockData.Graves;
					num2 = ((graves != null) ? graves.Count : 0);
					break;
				}
				default:
					num2 = this._normal.Count;
					break;
				}
				if (!true)
				{
				}
				result = num2;
			}
			return result;
		}

		// Token: 0x06006E3F RID: 28223 RVA: 0x0032F1A8 File Offset: 0x0032D3A8
		private void InitFilter(bool clearSearch = true)
		{
			this._normal.Clear();
			bool canSeeDetail = this._canSeeDetail;
			if (canSeeDetail)
			{
				bool flag = this._blockData.SpecialCharacters != null;
				if (flag)
				{
					this._normal.AddRange(this._blockData.SpecialCharacters.Select((CharacterDisplayData _, int index) => new ValueTuple<int, int>(0, index)));
				}
				bool flag2 = this._blockData.NormalCharacters != null;
				if (flag2)
				{
					this._normal.AddRange(this._blockData.NormalCharacters.Select((CharacterDisplayData _, int index) => new ValueTuple<int, int>(1, index)));
				}
				bool flag3 = this._blockData.InfectedCharacters != null;
				if (flag3)
				{
					this._normal.AddRange(this._blockData.InfectedCharacters.Select((CharacterDisplayData _, int index) => new ValueTuple<int, int>(2, index)));
				}
				bool flag4 = this._blockData.EnemyCharacters != null;
				if (flag4)
				{
					this._normal.AddRange(this._blockData.EnemyCharacters.Select((CharacterDisplayData _, int index) => new ValueTuple<int, int>(4, index)));
				}
				bool flag5 = this._blockData.Animals != null;
				if (flag5)
				{
					this._normal.AddRange(this._blockData.Animals.Select((Animal _, int index) => new ValueTuple<int, int>(3, index)));
				}
				bool flag6 = this._blockData.RandomEnemies != null;
				if (flag6)
				{
					this._normal.AddRange(this._blockData.RandomEnemies.Select((MapTemplateEnemyInfo _, int index) => new ValueTuple<int, int>(5, index)));
				}
				bool flag7 = this._blockData.Caravans != null;
				if (flag7)
				{
					this._normal.AddRange(this._blockData.Caravans.Select((CaravanDisplayData _, int index) => new ValueTuple<int, int>(6, index)));
				}
				bool flag8 = this._blockData.Graves != null;
				if (flag8)
				{
					this._normal.AddRange(this._blockData.Graves.Select((GraveDisplayData _, int index) => new ValueTuple<int, int>(7, index)));
				}
				bool flag9 = this._normal.Count > 0 || !string.IsNullOrEmpty(this.characterFilter.text);
				if (flag9)
				{
					this.characterFilter.gameObject.SetActive(true);
				}
			}
			else
			{
				this.normalCharacterFilter.gameObject.SetActive(false);
				this.characterFilter.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006E40 RID: 28224 RVA: 0x0032F4C0 File Offset: 0x0032D6C0
		public void ReapplyFilter(bool clearSearch = false, bool scrollToTop = true)
		{
			this.InitFilter(clearSearch);
			string searching = this.characterFilter.text;
			bool flag = this._inSearchMode = !string.IsNullOrWhiteSpace(this.characterFilter.text);
			if (flag)
			{
				this._normal.RemoveAll(([TupleElementNames(new string[]
				{
					"type",
					"index"
				})] ValueTuple<int, int> x) => !string.IsNullOrWhiteSpace(searching) && !this.GetNameStr(x).Contains(searching));
				this.selector.SetInteractable(false);
				this.selector.DeSelectWithoutNotify();
				this.SetDataCount(this._normal.Count, true);
			}
			else
			{
				this.RefreshSelectedIndex(scrollToTop);
			}
		}

		// Token: 0x06006E41 RID: 28225 RVA: 0x0032F564 File Offset: 0x0032D764
		private int GetOrg([TupleElementNames(new string[]
		{
			"type",
			"index"
		})] ValueTuple<int, int> x)
		{
			return this.GetNameData(x).Item3;
		}

		// Token: 0x06006E42 RID: 28226 RVA: 0x0032F574 File Offset: 0x0032D774
		private string GetNameStr([TupleElementNames(new string[]
		{
			"type",
			"index"
		})] ValueTuple<int, int> x)
		{
			ValueTuple<string, string, int> nameData = this.GetNameData(x);
			string nameText = nameData.Item1;
			if (nameText != null)
			{
				string gradeText = nameData.Item2;
				if (gradeText != null)
				{
					return ViewMapBlockCharList.HtmlTagRemover.Replace(nameText + "\n" + gradeText, "");
				}
			}
			return "";
		}

		// Token: 0x06006E43 RID: 28227 RVA: 0x0032F5C0 File Offset: 0x0032D7C0
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		private ValueTuple<string, string, int> GetNameData([TupleElementNames(new string[]
		{
			"type",
			"index"
		})] ValueTuple<int, int> x)
		{
			int item = x.Item1;
			if (!true)
			{
			}
			ValueTuple<string, string, int> result;
			switch (item)
			{
			case 0:
				result = MapBlockChar.GetTextData(this._blockData.SpecialCharacters[x.Item2]);
				break;
			case 1:
				result = MapBlockChar.GetTextData(this._blockData.NormalCharacters[x.Item2]);
				break;
			case 2:
				result = MapBlockChar.GetTextData(this._blockData.InfectedCharacters[x.Item2]);
				break;
			case 3:
				result = MapBlockChar.GetTextData(this._blockData.Animals[x.Item2]);
				break;
			case 4:
				result = MapBlockChar.GetTextData(this._blockData.EnemyCharacters[x.Item2]);
				break;
			case 5:
				result = MapBlockChar.GetTextData(this._blockData.RandomEnemies[x.Item2]);
				break;
			case 6:
				result = MapBlockChar.GetTextData(this._blockData.Caravans[x.Item2]);
				break;
			case 7:
				result = MapBlockChar.GetTextData(this._blockData.Graves[x.Item2]);
				break;
			default:
				result = new ValueTuple<string, string, int>("", "", 0);
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006E44 RID: 28228 RVA: 0x0032F717 File Offset: 0x0032D917
		public void RefreshOrg()
		{
			this.ReapplyFilter(false, false);
		}

		// Token: 0x06006E45 RID: 28229 RVA: 0x0032F722 File Offset: 0x0032D922
		public void SetSearchResult(string _)
		{
			this.ReapplyFilter(false, true);
		}

		// Token: 0x06006E46 RID: 28230 RVA: 0x0032F730 File Offset: 0x0032D930
		public void BeginEdit(string _)
		{
			ViewMapBlockCharList.IsFocusOnSearchInputField = true;
			bool flag = !string.IsNullOrWhiteSpace(this.characterFilter.text) && this.selector.GetActiveIndex() >= 0;
			if (flag)
			{
				this.ReapplyFilter(false, true);
			}
		}

		// Token: 0x06006E47 RID: 28231 RVA: 0x0032F777 File Offset: 0x0032D977
		public void EndEdit(string _)
		{
			this.SetSearchResult(_);
			ViewMapBlockCharList.IsFocusOnSearchInputField = false;
		}

		// Token: 0x06006E48 RID: 28232 RVA: 0x0032F788 File Offset: 0x0032D988
		public void OnItemRender(int index, GameObject obj)
		{
			bool flag = GameApp.Quiting || GameApp.ReadyToQuit;
			if (!flag)
			{
				MapBlockChar component = obj.GetComponent<MapBlockChar>();
				bool hasGuard = false;
				component.SetGuard(false, false);
				bool flag2 = !this._canSeeDetail;
				if (flag2)
				{
					component.SetUnknown();
				}
				else
				{
					switch (this.ActiveKey)
					{
					case 0:
					{
						CharacterDisplayData data = this._filtered[index];
						List<CharacterDisplayData> specialCharacters = this._blockData.SpecialCharacters;
						bool isSpecialNpc = specialCharacters != null && specialCharacters.Any((CharacterDisplayData character) => character.CharacterId == data.CharacterId);
						Dictionary<int, bool> hasGuardInfo = this._blockData.HasGuardInfo;
						bool hasInfo = hasGuardInfo != null && hasGuardInfo.TryGetValue(data.CharacterId, out hasGuard);
						component.Set(this, data, isSpecialNpc, true);
						component.SetGuard(hasInfo, hasGuard);
						break;
					}
					case 1:
					{
						List<CharacterDisplayData> infectedCharacters = this._blockData.InfectedCharacters;
						int count = (infectedCharacters != null) ? infectedCharacters.Count : 0;
						bool flag3 = index < count;
						if (flag3)
						{
							Dictionary<int, bool> hasGuardInfo2 = this._blockData.HasGuardInfo;
							bool hasInfo2 = hasGuardInfo2 != null && hasGuardInfo2.TryGetValue(this._blockData.InfectedCharacters[index].CharacterId, out hasGuard);
							component.Set(this, this._blockData.InfectedCharacters[index], false, true);
							component.SetGuard(hasInfo2, hasGuard);
						}
						else
						{
							index -= count;
							List<CharacterDisplayData> enemyCharacters = this._blockData.EnemyCharacters;
							count = ((enemyCharacters != null) ? enemyCharacters.Count : 0);
							bool flag4 = index < count;
							if (flag4)
							{
								Dictionary<int, bool> hasGuardInfo3 = this._blockData.HasGuardInfo;
								bool hasInfo3 = hasGuardInfo3 != null && hasGuardInfo3.TryGetValue(this._blockData.EnemyCharacters[index].CharacterId, out hasGuard);
								component.Set(this, this._blockData.EnemyCharacters[index], false, true);
								component.SetGuard(hasInfo3, hasGuard);
							}
							else
							{
								index -= count;
								List<MapTemplateEnemyInfo> randomEnemies = this._blockData.RandomEnemies;
								count = ((randomEnemies != null) ? randomEnemies.Count : 0);
								bool flag5 = index < count;
								if (flag5)
								{
									component.Set(this, this._blockData.RandomEnemies[index]);
								}
								else
								{
									index -= count;
									component.Set(this, this._blockData.Animals[index]);
								}
							}
						}
						break;
					}
					case 2:
						component.Set(this, this._blockData.Caravans[index]);
						break;
					case 3:
						component.Set(this, this._blockData.Graves[index]);
						break;
					default:
					{
						ValueTuple<int, int> curr = this._normal[index];
						index = curr.Item2;
						switch (curr.Item1)
						{
						case 0:
						{
							Dictionary<int, bool> hasGuardInfo4 = this._blockData.HasGuardInfo;
							bool hasInfo4 = hasGuardInfo4 != null && hasGuardInfo4.TryGetValue(this._blockData.SpecialCharacters[index].CharacterId, out hasGuard);
							component.Set(this, this._blockData.SpecialCharacters[index], true, true);
							component.SetGuard(hasInfo4, hasGuard);
							break;
						}
						case 1:
						{
							Dictionary<int, bool> hasGuardInfo5 = this._blockData.HasGuardInfo;
							bool hasInfo5 = hasGuardInfo5 != null && hasGuardInfo5.TryGetValue(this._blockData.NormalCharacters[index].CharacterId, out hasGuard);
							component.Set(this, this._blockData.NormalCharacters[index], false, true);
							component.SetGuard(hasInfo5, hasGuard);
							break;
						}
						case 2:
						{
							Dictionary<int, bool> hasGuardInfo6 = this._blockData.HasGuardInfo;
							bool hasInfo6 = hasGuardInfo6 != null && hasGuardInfo6.TryGetValue(this._blockData.InfectedCharacters[index].CharacterId, out hasGuard);
							component.Set(this, this._blockData.InfectedCharacters[index], false, true);
							component.SetGuard(hasInfo6, hasGuard);
							break;
						}
						case 3:
							component.Set(this, this._blockData.Animals[index]);
							break;
						case 4:
						{
							Dictionary<int, bool> hasGuardInfo7 = this._blockData.HasGuardInfo;
							bool hasInfo7 = hasGuardInfo7 != null && hasGuardInfo7.TryGetValue(this._blockData.EnemyCharacters[index].CharacterId, out hasGuard);
							component.Set(this, this._blockData.EnemyCharacters[index], false, true);
							component.SetGuard(hasInfo7, hasGuard);
							break;
						}
						case 5:
							component.Set(this, this._blockData.RandomEnemies[index]);
							break;
						case 6:
							component.Set(this, this._blockData.Caravans[index]);
							break;
						case 7:
							component.Set(this, this._blockData.Graves[index]);
							break;
						default:
							Debug.LogError(string.Format("Invalid type: {0} (with index = {1})", curr.Item1, index));
							break;
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x06006E49 RID: 28233 RVA: 0x0032FC90 File Offset: 0x0032DE90
		private void OnEnable()
		{
			GEvent.Add(UiEvents.WorldMapShowInfoBlockChange, new GEvent.Callback(this.RefreshAll));
			GEvent.Add(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.OnUpdateCaravanBlockCharData, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.HideMapBlockCharList, new GEvent.Callback(this.HideMapBlockCharList));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.AnimalPlaceDataChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.OnSetMapBlockCharListTog, new GEvent.Callback(this.RefreshCallWithTogKey));
			GEvent.Add(UiEvents.NickNameChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.OnMapBlockCharCustomInfoChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.OnMapBlockCharCustomButtonChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.MapFocusLocationGrave, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.MapClearLocationTemporaryMark, new GEvent.Callback(this.RefreshCall));
			this.shortcuts.gameObject.SetActive(false);
		}

		// Token: 0x06006E4A RID: 28234 RVA: 0x0032FE1C File Offset: 0x0032E01C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.WorldMapShowInfoBlockChange, new GEvent.Callback(this.RefreshAll));
			GEvent.Remove(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.OnUpdateCaravanBlockCharData, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.HideMapBlockCharList, new GEvent.Callback(this.HideMapBlockCharList));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.AnimalPlaceDataChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.OnSetMapBlockCharListTog, new GEvent.Callback(this.RefreshCallWithTogKey));
			GEvent.Remove(UiEvents.NickNameChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.OnMapBlockCharCustomInfoChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.OnMapBlockCharCustomButtonChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.MapFocusLocationGrave, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.MapClearLocationTemporaryMark, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x06006E4B RID: 28235 RVA: 0x0032FF98 File Offset: 0x0032E198
		private void HideMapBlockCharList(ArgumentBox _)
		{
			this.PlayAnim(this._isShow = false);
		}

		// Token: 0x06006E4C RID: 28236 RVA: 0x0032FFB7 File Offset: 0x0032E1B7
		private void ShowMapBlockCharList(ArgumentBox _)
		{
			this._isShow = true;
			this.Refresh(false);
		}

		// Token: 0x06006E4D RID: 28237 RVA: 0x0032FFCC File Offset: 0x0032E1CC
		private void RefreshCallWithTogKey(ArgumentBox ab)
		{
			int togKey;
			bool flag = ab != null && ab.Get("TogKey", out togKey);
			if (flag)
			{
				this.selector.Set(togKey, false);
			}
			this.Refresh(false);
		}

		// Token: 0x06006E4E RID: 28238 RVA: 0x00330007 File Offset: 0x0032E207
		private void RefreshCall(ArgumentBox _)
		{
			this.Refresh(false);
		}

		// Token: 0x06006E4F RID: 28239 RVA: 0x00330011 File Offset: 0x0032E211
		private void RefreshAll(ArgumentBox _)
		{
			this.Refresh(true);
		}

		// Token: 0x06006E50 RID: 28240 RVA: 0x0033001B File Offset: 0x0032E21B
		public override void OnInit(ArgumentBox _)
		{
			this.charScroll.Init(this);
			this._isShow = true;
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
		}

		// Token: 0x06006E51 RID: 28241 RVA: 0x00330040 File Offset: 0x0032E240
		public void Refresh(bool scrollToTop = false)
		{
			this.Refresh(scrollToTop, (!this._mapModel.TemporaryMarkLocation.IsValid() || this._mapModel.TemporaryMarkLocation.BlockId != this._mapModel.SelectedBlockId) ? this._mapModel.SelectedBlock : this._mapModel.GetBlockData(new Location(this._mapModel.TemporaryMarkLocation.AreaId, this._mapModel.SelectedBlockId)));
		}

		// Token: 0x06006E52 RID: 28242 RVA: 0x003300C0 File Offset: 0x0032E2C0
		public void Refresh(bool scrollToTop, MapBlockData blockData)
		{
			bool flag = GameApp.AdvancingMonth || !this._isShow || WorldMapModel.Traveling;
			if (!flag)
			{
				short? num = (blockData != null) ? new short?(blockData.BlockId) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				int currentBlockId = (int)this._mapModel.CurrentBlockId;
				this._canInteract = (num2.GetValueOrDefault() == currentBlockId & num2 != null);
				this._canSeeDetail = (this._canInteract || this._mapModel.IsLocationShouldInSight((blockData != null) ? blockData.GetLocation() : Location.Invalid));
				this._isHideCharacterSet = this._mapModel.IsHideCharacterSet();
				MapDomainMethod.AsyncCall.RequestMapBlockCharacterList(this, blockData, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._blockData);
					this._normalCharacterFilterController.SetData(this._blockData);
					this._normalCharIds.Clear();
					bool flag2 = this._blockData.NormalCharacters != null;
					if (flag2)
					{
						foreach (CharacterDisplayData character in this._blockData.NormalCharacters)
						{
							bool flag3 = (int)character.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby;
							if (flag3)
							{
								this._normalCharIds.Add(character.CharacterId);
							}
						}
					}
					WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
					bool flag4 = this._oldLocation != model.CurrentLocation;
					if (flag4)
					{
						this._oldLocation = model.CurrentLocation;
						this.normalCharacterFilter.ClearAllFilter();
					}
					this.ReapplyFilter(true, scrollToTop);
					bool inSearchMode = this._inSearchMode;
					if (inSearchMode)
					{
						this.selector.SetInteractable(false);
						this.PlayAnim(true);
					}
					else
					{
						this.RefreshSelectedIndex(scrollToTop);
					}
				});
			}
		}

		// Token: 0x06006E53 RID: 28243 RVA: 0x003301B8 File Offset: 0x0032E3B8
		private void RefreshSelectedIndex(bool scrollToTop)
		{
			bool flag2 = this._blockData == null;
			if (flag2)
			{
				this.PlayAnim(false);
			}
			else
			{
				int selectorIndex = this.selector.GetActiveIndex();
				this.normalCharacterFilter.gameObject.SetActive(this._canSeeDetail && selectorIndex == 0);
				bool flag3 = this._needClear && selectorIndex == 0;
				if (flag3)
				{
					this._needClear = false;
					this.normalCharacterFilter.ClearAllFilter();
				}
				else
				{
					this._needClear |= (selectorIndex != 0);
				}
				bool flag4 = selectorIndex >= 0 && this.GetCount(selectorIndex) > 0;
				if (flag4)
				{
					for (int i = 0; i < 4; i++)
					{
						this.selector.SetInteractable(this.GetCount(i) > 0, i);
					}
				}
				else
				{
					int j = 0;
					bool flag = true;
					while (j < 4)
					{
						bool flag5 = this.GetCount(j) > 0;
						if (flag5)
						{
							bool flag6 = flag;
							if (flag6)
							{
								flag = false;
								this.selector.Set(j, false);
							}
							this.selector.SetInteractable(true, j);
						}
						else
						{
							this.selector.SetInteractable(false, j);
						}
						j++;
					}
				}
				int count = this.GetCount(this.selector.GetActiveIndex());
				this.PlayAnim(count > 0);
				bool flag7 = this.selector.GetActiveIndex() == 0;
				if (flag7)
				{
					IEnumerable<CharacterDisplayData> enumerable = this._blockData.SpecialCharacters;
					IEnumerable<CharacterDisplayData> first = enumerable ?? Enumerable.Empty<CharacterDisplayData>();
					enumerable = this._blockData.NormalCharacters;
					this._filtered = first.Concat(enumerable ?? Enumerable.Empty<CharacterDisplayData>()).Where(this._normalCharacterFilterController.GenerateFilter()).ToList<CharacterDisplayData>();
					this._normalCharacterFilterController.SetFilteredCount(count = this._filtered.Count);
					SortAndFilterController<CharacterDisplayData> normalCharacterFilterController = this._normalCharacterFilterController;
					enumerable = this._blockData.SpecialCharacters;
					IEnumerable<CharacterDisplayData> first2 = enumerable ?? Enumerable.Empty<CharacterDisplayData>();
					enumerable = this._blockData.NormalCharacters;
					normalCharacterFilterController.AfterFilter(first2.Concat(enumerable ?? Enumerable.Empty<CharacterDisplayData>()));
				}
				this.SetDataCount(count, scrollToTop);
			}
		}

		// Token: 0x06006E54 RID: 28244 RVA: 0x003303E4 File Offset: 0x0032E5E4
		private void PlayAnim(bool isShow)
		{
			this.shortcuts.gameObject.SetActive(false);
			bool isShow2 = isShow;
			if (isShow2)
			{
				this.selector.gameObject.SetActive(true);
				this.RefreshDetails();
			}
			Tweener tween = this._tween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			bool flag = !isShow;
			if (flag)
			{
				this.details.gameObject.SetActive(false);
			}
			(this._tween = base.transform.DOLocalMoveX(isShow ? -1280f : -1650f, 0.3f, false)).OnComplete(delegate
			{
				bool flag2 = !isShow;
				if (flag2)
				{
					this.selector.gameObject.SetActive(false);
				}
				this._tween = null;
			});
		}

		// Token: 0x06006E55 RID: 28245 RVA: 0x003304B0 File Offset: 0x0032E6B0
		public bool CanClick(DisplayType type, int id)
		{
			WorldMapModel instance = SingletonObject.getInstance<WorldMapModel>();
			return instance != null && instance.TaiwuMoveState == WorldMapModel.MoveState.Idle && this._canInteract;
		}

		// Token: 0x06006E56 RID: 28246 RVA: 0x003304D8 File Offset: 0x0032E6D8
		public void OnClick(DisplayType type, int id)
		{
			bool clickLocked = this._clickLocked;
			if (!clickLocked)
			{
				this._clickLocked = true;
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
				{
					this._clickLocked = false;
				});
				bool flag = (type & DisplayType.Normal) > (DisplayType)0;
				if (flag)
				{
					TaiwuEventDomainMethod.Call.OnCharacterClicked(id);
				}
				else
				{
					bool flag2 = (type & DisplayType.Caravan) > (DisplayType)0;
					if (flag2)
					{
						TaiwuEventDomainMethod.Call.OnInteractCaravan(id);
					}
					else
					{
						bool flag3 = type == DisplayType.EnemyAnimal;
						if (flag3)
						{
							TaiwuEventDomainMethod.Call.AnimalAvatarClicked(id);
						}
						else
						{
							bool flag4 = type == DisplayType.EnemyInfected;
							if (flag4)
							{
								TaiwuEventDomainMethod.Call.OnCharacterClicked(id);
							}
							else
							{
								bool flag5 = type == DisplayType.EnemyTemplate && id >= 296 && id <= 383;
								if (flag5)
								{
									TaiwuEventDomainMethod.Call.OnCharacterTemplateClicked((short)id);
								}
								else
								{
									bool flag6 = (type & DisplayType.Grave) > (DisplayType)0;
									if (flag6)
									{
										TaiwuEventDomainMethod.Call.NpcTombClicked(id);
									}
									else
									{
										Debug.LogWarning(string.Format("invalid displayType: {0}", type));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x17000CBB RID: 3259
		// (get) Token: 0x06006E57 RID: 28247 RVA: 0x003305C9 File Offset: 0x0032E7C9
		// (set) Token: 0x06006E58 RID: 28248 RVA: 0x003305D4 File Offset: 0x0032E7D4
		bool IMapBlockCharDataSource.CanClick
		{
			get
			{
				return !this._clickLocked;
			}
			set
			{
				this._clickLocked = !value;
			}
		}

		// Token: 0x06006E59 RID: 28249 RVA: 0x003305E0 File Offset: 0x0032E7E0
		public void OnHover(RectTransform trans, MapBlockChar mbc)
		{
			GEvent.OnEvent(UiEvents.OnWorldMapCharacterImpactRangeChanged, EasyPool.Get<ArgumentBox>().Set("charId", mbc.CharId).Set("visible", true));
			bool flag = !this._normalCharIds.Contains(mbc.CharId);
			if (flag)
			{
				this.OnHoverEnd(mbc.CharId);
			}
		}

		// Token: 0x06006E5A RID: 28250 RVA: 0x00330646 File Offset: 0x0032E846
		public void OnHoverEnd(int charId)
		{
			this.shortcuts.gameObject.SetActive(false);
		}

		// Token: 0x06006E5B RID: 28251 RVA: 0x0033065A File Offset: 0x0032E85A
		public void OnChildHoverEnd(MapBlockChar charObj)
		{
			GEvent.OnEvent(UiEvents.OnWorldMapCharacterImpactRangeChanged, EasyPool.Get<ArgumentBox>().Set("charId", charObj.CharId).Set("visible", false));
		}

		// Token: 0x06006E5C RID: 28252 RVA: 0x00330690 File Offset: 0x0032E890
		public bool CanClick(int id, int charId)
		{
			WorldMapModel instance = SingletonObject.getInstance<WorldMapModel>();
			return instance != null && instance.TaiwuMoveState == WorldMapModel.MoveState.Idle && this._canInteract;
		}

		// Token: 0x06006E5D RID: 28253 RVA: 0x003306B8 File Offset: 0x0032E8B8
		public void OnClick(int id, int charId)
		{
			switch (id)
			{
			case 1:
				TaiwuDomainMethod.Call.TaiwuUnfollowNpc(charId);
				break;
			case 2:
				Debug.LogError("互动逻辑未完成");
				break;
			}
		}

		// Token: 0x06006E5E RID: 28254 RVA: 0x003306F6 File Offset: 0x0032E8F6
		private void AwakeDetails()
		{
			this.details.onClick.ResetListener(new Action(this.OpenDetails));
		}

		// Token: 0x17000CBC RID: 3260
		// (get) Token: 0x06006E5F RID: 28255 RVA: 0x00330718 File Offset: 0x0032E918
		private IEnumerable<int> Characters
		{
			get
			{
				MapBlockCharacterList blockData = this._blockData;
				IEnumerable<CharacterDisplayData> enumerable = (blockData != null) ? blockData.SpecialCharacters : null;
				IEnumerable<CharacterDisplayData> first = enumerable ?? Enumerable.Empty<CharacterDisplayData>();
				MapBlockCharacterList blockData2 = this._blockData;
				enumerable = ((blockData2 != null) ? blockData2.NormalCharacters : null);
				IEnumerable<CharacterDisplayData> first2 = first.Concat(enumerable ?? Enumerable.Empty<CharacterDisplayData>());
				MapBlockCharacterList blockData3 = this._blockData;
				enumerable = ((blockData3 != null) ? blockData3.EnemyCharacters : null);
				IEnumerable<CharacterDisplayData> first3 = first2.Concat(enumerable ?? Enumerable.Empty<CharacterDisplayData>());
				MapBlockCharacterList blockData4 = this._blockData;
				enumerable = ((blockData4 != null) ? blockData4.InfectedCharacters : null);
				return from x in first3.Concat(enumerable ?? Enumerable.Empty<CharacterDisplayData>())
				select x.CharacterId;
			}
		}

		// Token: 0x06006E60 RID: 28256 RVA: 0x003307CC File Offset: 0x0032E9CC
		private void RefreshDetails()
		{
			this.details.gameObject.SetActive(this._canSeeDetail);
			this.details.interactable = this.Characters.Any((int _) => true);
		}

		// Token: 0x06006E61 RID: 28257 RVA: 0x00330827 File Offset: 0x0032EA27
		private void OpenDetails()
		{
			UIElement.TaiwuVillagers.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("CharIds", this.Characters.ToList<int>()));
			UIManager.Instance.MaskUI(UIElement.TaiwuVillagers);
		}

		// Token: 0x040051FA RID: 20986
		public static bool IsFocusOnSearchInputField;

		// Token: 0x040051FB RID: 20987
		[SerializeField]
		private UIAnim uiAnim;

		// Token: 0x040051FC RID: 20988
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x040051FD RID: 20989
		[SerializeField]
		private Vector2 originPosition;

		// Token: 0x040051FE RID: 20990
		[SerializeField]
		private Vector2 targetPosition;

		// Token: 0x040051FF RID: 20991
		[SerializeField]
		private CToggleGroup selector;

		// Token: 0x04005200 RID: 20992
		[SerializeField]
		private SortAndFilter normalCharacterFilter;

		// Token: 0x04005201 RID: 20993
		private CharacterDisplayDataSortAndFilterController _normalCharacterFilterController;

		// Token: 0x04005202 RID: 20994
		[SerializeField]
		private TMP_InputField characterFilter;

		// Token: 0x04005203 RID: 20995
		[SerializeField]
		private CButton characterFilterClearButton;

		// Token: 0x04005204 RID: 20996
		[SerializeField]
		private CImage bottomImg;

		// Token: 0x04005205 RID: 20997
		[SerializeField]
		private MapBlockCharScroll charScroll;

		// Token: 0x04005206 RID: 20998
		private WorldMapModel _mapModel;

		// Token: 0x04005207 RID: 20999
		private List<CharacterDisplayData> _characterDisplayData;

		// Token: 0x04005208 RID: 21000
		private MapBlockCharacterList _blockData;

		// Token: 0x04005209 RID: 21001
		[TupleElementNames(new string[]
		{
			"type",
			"index"
		})]
		private readonly List<ValueTuple<int, int>> _normal = new List<ValueTuple<int, int>>();

		// Token: 0x0400520A RID: 21002
		private const int Special = 0;

		// Token: 0x0400520B RID: 21003
		private const int Normal = 1;

		// Token: 0x0400520C RID: 21004
		private const int Infected = 2;

		// Token: 0x0400520D RID: 21005
		private const int Animal = 3;

		// Token: 0x0400520E RID: 21006
		private const int Enemy = 4;

		// Token: 0x0400520F RID: 21007
		private const int RandomEnemy = 5;

		// Token: 0x04005210 RID: 21008
		private const int Caravan = 6;

		// Token: 0x04005211 RID: 21009
		private const int Grave = 7;

		// Token: 0x04005212 RID: 21010
		private readonly List<int> _organizations = new List<int>();

		// Token: 0x04005213 RID: 21011
		private bool _inSearchMode;

		// Token: 0x04005214 RID: 21012
		private static readonly Regex HtmlTagRemover = new Regex("<[^>]*>", RegexOptions.Compiled);

		// Token: 0x04005215 RID: 21013
		private bool _isShow = true;

		// Token: 0x04005216 RID: 21014
		private bool _canInteract;

		// Token: 0x04005217 RID: 21015
		private bool _canSeeDetail;

		// Token: 0x04005218 RID: 21016
		private bool _isHideCharacterSet;

		// Token: 0x04005219 RID: 21017
		private readonly HashSet<int> _normalCharIds = new HashSet<int>();

		// Token: 0x0400521A RID: 21018
		private Location _oldLocation = Location.Invalid;

		// Token: 0x0400521B RID: 21019
		private List<CharacterDisplayData> _filtered;

		// Token: 0x0400521C RID: 21020
		private bool _needClear = true;

		// Token: 0x0400521D RID: 21021
		private Tweener _tween;

		// Token: 0x0400521E RID: 21022
		private bool _clickLocked;

		// Token: 0x0400521F RID: 21023
		[SerializeField]
		private MapBlockCharShortCuts shortcuts;

		// Token: 0x04005220 RID: 21024
		[SerializeField]
		private CButton details;
	}
}
