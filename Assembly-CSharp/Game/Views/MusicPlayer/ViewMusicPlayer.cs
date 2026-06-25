using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.MusicPlayer
{
	// Token: 0x0200081E RID: 2078
	public class ViewMusicPlayer : UIBase
	{
		// Token: 0x17000C53 RID: 3155
		// (get) Token: 0x0600660C RID: 26124 RVA: 0x002E9B72 File Offset: 0x002E7D72
		private MusicPlayerModel Model
		{
			get
			{
				return SingletonObject.getInstance<MusicPlayerModel>();
			}
		}

		// Token: 0x0600660D RID: 26125 RVA: 0x002E9B7C File Offset: 0x002E7D7C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.InitVolume();
			this.RefreshMusicPlayerPlayState();
			this.toggleGroupTab.SetWithoutNotify(ViewMusicPlayer.TogTabKey.All.ToInt());
			this.toggleGroupFilter.SetWithoutNotify(ViewMusicPlayer.TogFilterKey.All.ToInt());
			this.RefreshMusicList();
			this.RefreshButtonMode();
			this.noneFavoriteObj.SetActive(false);
			this.RefreshSelectedMusicItem();
		}

		// Token: 0x0600660E RID: 26126 RVA: 0x002E9BE8 File Offset: 0x002E7DE8
		private void Awake()
		{
			this.scroll.OnItemRender += this.OnItemRender;
			this.toggleGroupTab.Init(-1);
			this.toggleGroupTab.OnActiveIndexChange += this.OnToggleGroupTabActiveIndexChange;
			this.toggleGroupFilter.Init(-1);
			this.InitFilterToggleTips();
			this.toggleGroupFilter.OnActiveIndexChange += this.OnToggleGroupFilterActiveIndexChange;
			this.searchInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnSearchEndEdit));
			this.sliderVolume.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
		}

		// Token: 0x0600660F RID: 26127 RVA: 0x002E9C9C File Offset: 0x002E7E9C
		private void OnDestroy()
		{
			this.scroll.OnItemRender -= this.OnItemRender;
			this.toggleGroupTab.OnActiveIndexChange -= this.OnToggleGroupTabActiveIndexChange;
			this.toggleGroupFilter.OnActiveIndexChange -= this.OnToggleGroupFilterActiveIndexChange;
			this._tabLeadVisibleIndex.Clear();
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x002E9CFE File Offset: 0x002E7EFE
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnMusicPlayerPlayStateChange, new GEvent.Callback(this.OnMusicPlayerPlayStateChange));
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x002E9D1D File Offset: 0x002E7F1D
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnMusicPlayerPlayStateChange, new GEvent.Callback(this.OnMusicPlayerPlayStateChange));
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x002E9D3C File Offset: 0x002E7F3C
		private void LateUpdate()
		{
			bool isPlaying = this.Model.IsPlaying;
			if (isPlaying)
			{
				this.RefreshMusicPlayerTime();
			}
		}

		// Token: 0x06006613 RID: 26131 RVA: 0x002E9D60 File Offset: 0x002E7F60
		private void OnClickButtonMode()
		{
			this.Model.PlayMode = (MusicPlayerModel.MusicPlayMode)((this.Model.PlayMode.ToInt() + 1) % MusicPlayerModel.MusicPlayMode.Count.ToInt());
			this.RefreshButtonMode();
		}

		// Token: 0x06006614 RID: 26132 RVA: 0x002E9D9C File Offset: 0x002E7F9C
		private void RefreshButtonMode()
		{
			this.buttonModeSingle.gameObject.SetActive(this.Model.PlayMode == MusicPlayerModel.MusicPlayMode.Single);
			this.buttonModeList.gameObject.SetActive(this.Model.PlayMode == MusicPlayerModel.MusicPlayMode.List);
			this.buttonModeRandom.gameObject.SetActive(this.Model.PlayMode == MusicPlayerModel.MusicPlayMode.Random);
		}

		// Token: 0x06006615 RID: 26133 RVA: 0x002E9E07 File Offset: 0x002E8007
		private void OnMusicPlayerPlayStateChange(ArgumentBox _)
		{
			this.RefreshMusicPlayerPlayState();
			this.RefreshSelectedMusicItem();
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x002E9E18 File Offset: 0x002E8018
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string text = name;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1344321573U)
			{
				if (num <= 570695658U)
				{
					if (num != 324204674U)
					{
						if (num != 348875654U)
						{
							if (num != 570695658U)
							{
								return;
							}
							if (!(text == "ButtonCloseView"))
							{
								return;
							}
							this.QuickHide();
							return;
						}
						else if (!(text == "ButtonModeSingle"))
						{
							return;
						}
					}
					else
					{
						if (!(text == "ButtonVolumeMute"))
						{
							return;
						}
						goto IL_2AF;
					}
				}
				else if (num != 643367663U)
				{
					if (num != 1037145658U)
					{
						if (num != 1344321573U)
						{
							return;
						}
						if (!(text == "ButtonLast"))
						{
							return;
						}
						this.buttonLast.interactable = false;
						this.Model.PlayLastMusic(this._unlockedMusicIdList);
						this.ScrollToCurrentItem();
						return;
					}
					else if (!(text == "ButtonModeList"))
					{
						return;
					}
				}
				else
				{
					if (!(text == "ButtonVolumeMid"))
					{
						return;
					}
					goto IL_2AF;
				}
			}
			else if (num <= 2496501337U)
			{
				if (num != 2109919223U)
				{
					if (num != 2486530779U)
					{
						if (num != 2496501337U)
						{
							return;
						}
						if (!(text == "ButtonModeRandom"))
						{
							return;
						}
					}
					else
					{
						if (!(text == "ButtonVolumeLow"))
						{
							return;
						}
						goto IL_2AF;
					}
				}
				else
				{
					if (!(text == "ButtonVolumeHigh"))
					{
						return;
					}
					goto IL_2AF;
				}
			}
			else if (num <= 3104843228U)
			{
				if (num != 3085892545U)
				{
					if (num != 3104843228U)
					{
						return;
					}
					if (!(text == "ButtonNext"))
					{
						return;
					}
					this.buttonNext.interactable = false;
					this.Model.PlayNextMusic(this._unlockedMusicIdList);
					this.ScrollToCurrentItem();
					return;
				}
				else
				{
					if (!(text == "ButtonPause"))
					{
						return;
					}
					this.buttonPause.interactable = false;
					this.Model.PauseMusic(false);
					return;
				}
			}
			else if (num != 3172102247U)
			{
				if (num != 3401623075U)
				{
					return;
				}
				if (!(text == "ButtonHideVolume"))
				{
					return;
				}
				this.ShowPanelVolume(false);
				return;
			}
			else
			{
				if (!(text == "ButtonPlay"))
				{
					return;
				}
				this.buttonPlay.interactable = false;
				bool isPaused = this.Model.IsPaused;
				if (isPaused)
				{
					this.Model.ResumeMusic();
				}
				else
				{
					this.Model.PlayMusic(0f);
				}
				return;
			}
			this.OnClickButtonMode();
			return;
			IL_2AF:
			this.ShowPanelVolume(true);
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x002EA0E8 File Offset: 0x002E82E8
		public override void QuickHide()
		{
			bool activeSelf = this.panelVolume.activeSelf;
			if (activeSelf)
			{
				this.ShowPanelVolume(false);
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
			else
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				base.QuickHide();
			}
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x002EA13C File Offset: 0x002E833C
		private void OnToggleGroupTabActiveIndexChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex != oldIndex && oldIndex >= 0 && this.scroll != null && this.scroll.initSuccess && this.scroll.CurrentDataCount > 0;
			if (flag)
			{
				this._tabLeadVisibleIndex[oldIndex] = this.GetFirstVisibleCellIndex();
			}
			this.RefreshMusicList();
			bool showNoneFavorite = newIndex == ViewMusicPlayer.TogTabKey.Favorite.ToInt() && this._musicIdList.Count == 0;
			this.noneFavoriteObj.SetActive(showNoneFavorite);
			this.RestoreTabScrollPosition(newIndex);
		}

		// Token: 0x06006619 RID: 26137 RVA: 0x002EA1D4 File Offset: 0x002E83D4
		private int GetFirstVisibleCellIndex()
		{
			bool flag = this.scroll == null || !this.scroll.initSuccess || this.scroll.CurrentDataCount <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				for (int i = 0; i < this.scroll.CurrentDataCount; i++)
				{
					bool flag2 = this.scroll.GetActiveCell(i) != null;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x0600661A RID: 26138 RVA: 0x002EA258 File Offset: 0x002E8458
		private void RestoreTabScrollPosition(int tabIndex)
		{
			bool flag = this.scroll == null;
			if (!flag)
			{
				int i;
				int savedLead = this._tabLeadVisibleIndex.TryGetValue(tabIndex, out i) ? i : 0;
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(10U, delegate
				{
					bool flag2 = this.scroll == null || !this.scroll.initSuccess || this.scroll.CurrentDataCount <= 0;
					if (!flag2)
					{
						int idx = Mathf.Clamp(savedLead, 0, this.scroll.CurrentDataCount - 1);
						this.scroll.ScrollTo(idx + 1, 0.3f);
					}
				});
			}
		}

		// Token: 0x0600661B RID: 26139 RVA: 0x002EA2B8 File Offset: 0x002E84B8
		private void OnToggleGroupFilterActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshMusicList();
		}

		// Token: 0x0600661C RID: 26140 RVA: 0x002EA2C1 File Offset: 0x002E84C1
		private void OnSearchEndEdit(string value)
		{
			this.RefreshMusicList();
		}

		// Token: 0x0600661D RID: 26141 RVA: 0x002EA2CC File Offset: 0x002E84CC
		private void RefreshMusicList()
		{
			this._musicIdList.Clear();
			this._unlockedMusicIdList.Clear();
			List<short> list = (this.toggleGroupTab.GetActiveIndex() == ViewMusicPlayer.TogTabKey.All.ToInt()) ? Music.Instance.GetAllKeys() : this.Model.FavoriteMusicList;
			list.Sort((short a, short b) => a.CompareTo(b));
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				string searchStr = this.searchInputField.text.Trim();
				this._musicIdList.AddRange(list.Where(delegate(short id)
				{
					MusicItem config = Music.Instance[id];
					ViewMusicPlayer.TogFilterKey togKey = (ViewMusicPlayer.TogFilterKey)this.toggleGroupFilter.GetActiveIndex();
					ECharacterPropertyDisplayType displayType = ViewMusicPlayer.GetEffectTypeAndValue(id).Item1;
					bool isContains = config.Name.Contains(searchStr);
					bool isSearched = searchStr.IsNullOrEmpty() || isContains;
					return this.IsFilter(togKey, displayType) && isSearched;
				}));
				this._unlockedMusicIdList.AddRange(from id in this._musicIdList
				where !this.Model.IsMusicLock(id)
				select id);
			}
			this.scroll.SetDataCount(this._musicIdList.Count);
			this.noneFavoriteObj.SetActive(this.toggleGroupTab.GetActiveIndex() == ViewMusicPlayer.TogTabKey.Favorite.ToInt() && this._musicIdList.Count == 0);
			this.RefreshSelectedMusicItem();
		}

		// Token: 0x0600661E RID: 26142 RVA: 0x002EA414 File Offset: 0x002E8614
		private void RefreshSelectedMusicItem()
		{
			bool flag = this.seledMusicItem == null;
			if (!flag)
			{
				this.seledMusicItem.RefreshSelectedSummary(this.Model.MusicId);
			}
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x002EA44C File Offset: 0x002E864C
		public static void TryRefreshMusicRowAfterFavoriteChange(short affectedMusicId)
		{
			ViewMusicPlayer view = Object.FindObjectOfType<ViewMusicPlayer>();
			bool flag = view == null;
			if (!flag)
			{
				view.RefreshMusicListRowForMusic(affectedMusicId);
				MusicPlayerModel model = SingletonObject.getInstance<MusicPlayerModel>();
				bool flag2 = model.MusicId == affectedMusicId;
				if (flag2)
				{
					view.RefreshSelectedMusicItem();
				}
			}
		}

		// Token: 0x06006620 RID: 26144 RVA: 0x002EA490 File Offset: 0x002E8690
		private void RefreshMusicListRowForMusic(short musicId)
		{
			int idx = this._musicIdList.IndexOf(musicId);
			bool flag = idx < 0 || this.scroll == null;
			if (!flag)
			{
				this.scroll.RefreshCell(idx);
			}
		}

		// Token: 0x06006621 RID: 26145 RVA: 0x002EA4D4 File Offset: 0x002E86D4
		private void OnItemRender(int index, GameObject obj)
		{
			short id = this._musicIdList[index];
			MusicItem musicItem = obj.GetComponent<MusicItem>();
			musicItem.Set(id);
		}

		// Token: 0x06006622 RID: 26146 RVA: 0x002EA500 File Offset: 0x002E8700
		private void RefreshMusicPlayerPlayState()
		{
			this.buttonPlay.gameObject.SetActive(!this.Model.IsPlaying);
			this.buttonPause.gameObject.SetActive(this.Model.IsPlaying);
			this.buttonPlay.interactable = this.Model.Interactable;
			this.buttonPause.interactable = this.Model.Interactable;
			this.buttonLast.interactable = this.Model.Interactable;
			this.buttonNext.interactable = this.Model.Interactable;
			bool isPlaying = this.Model.IsPlaying;
			if (isPlaying)
			{
				this.ScrollToCurrentItem();
			}
			this.ShowEffect(this.Model.IsPlaying);
			int maxTime = (int)this.Model.MaxTime;
			this.textMaxTime.text = ((this.Model.MaxTime == 0f) ? "-" : ViewMusicPlayer.GetTimeStr(maxTime));
			this.RefreshMusicPlayerTime();
			this.scroll.ReRender();
		}

		// Token: 0x06006623 RID: 26147 RVA: 0x002EA618 File Offset: 0x002E8818
		private void RefreshMusicPlayerTime()
		{
			this.imageProgress.fillAmount = ((this.Model.MaxTime == 0f) ? 0f : (this.Model.CurTime / this.Model.MaxTime));
			int curTime = (int)this.Model.CurTime;
			this.textCurTime.text = ViewMusicPlayer.GetTimeStr(curTime);
		}

		// Token: 0x06006624 RID: 26148 RVA: 0x002EA681 File Offset: 0x002E8881
		private void ScrollToCurrentItem()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				bool flag = this.scroll == null || !this.scroll.initSuccess || this.scroll.CurrentDataCount <= 0;
				if (!flag)
				{
					int idx = this._musicIdList.IndexOf(this.Model.MusicId);
					bool flag2 = idx < 0;
					if (flag2)
					{
						idx = 0;
					}
					idx = Mathf.Clamp(idx, 0, this.scroll.CurrentDataCount - 1);
					this.scroll.ScrollTo(idx, 0f);
				}
			});
		}

		// Token: 0x06006625 RID: 26149 RVA: 0x002EA69C File Offset: 0x002E889C
		private void ShowEffect(bool isShow)
		{
		}

		// Token: 0x06006626 RID: 26150 RVA: 0x002EA6A0 File Offset: 0x002E88A0
		public static string GetMusicAreaName(short musicId)
		{
			string nameContent = string.Empty;
			bool flag = musicId >= 0;
			if (flag)
			{
				MusicItem music = Music.Instance[musicId];
				string placeName = string.Empty;
				bool flag2 = music.MapBlock > -1;
				if (flag2)
				{
					placeName = MapBlock.Instance[music.MapBlock].Name;
				}
				else
				{
					bool flag3 = music.MapState == 0;
					if (flag3)
					{
						placeName = MapBlock.Instance[0].Name;
					}
					else
					{
						bool flag4 = music.MapState > 0;
						if (flag4)
						{
							placeName = MapState.Instance[music.MapState].Name;
						}
					}
				}
				nameContent = placeName;
			}
			return nameContent;
		}

		// Token: 0x06006627 RID: 26151 RVA: 0x002EA750 File Offset: 0x002E8950
		public static string GetMusicFullName(short musicId)
		{
			string nameContent = string.Empty;
			bool flag = musicId >= 0;
			if (flag)
			{
				string placeName = ViewMusicPlayer.GetMusicAreaName(musicId);
				MusicItem music = Music.Instance[musicId];
				nameContent = placeName + LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + music.Name;
			}
			return nameContent;
		}

		// Token: 0x06006628 RID: 26152 RVA: 0x002EA7A4 File Offset: 0x002E89A4
		[return: TupleElementNames(new string[]
		{
			"type",
			"value"
		})]
		public static ValueTuple<ECharacterPropertyDisplayType, int> GetEffectTypeAndValue(short musicId)
		{
			MusicItem musicConfig = Music.Instance[musicId];
			ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Count;
			CharacterFeatureItem featureConfig = CharacterFeature.Instance[musicConfig.TemporaryFeature];
			int value = 0;
			bool flag = featureConfig.PersonalityCalm != 0;
			if (flag)
			{
				type = ECharacterPropertyDisplayType.PersonalityCalm;
				value = (int)featureConfig.PersonalityCalm;
			}
			else
			{
				bool flag2 = featureConfig.PersonalityClever != 0;
				if (flag2)
				{
					type = ECharacterPropertyDisplayType.PersonalityClever;
					value = (int)featureConfig.PersonalityClever;
				}
				else
				{
					bool flag3 = featureConfig.PersonalityEnthusiastic != 0;
					if (flag3)
					{
						type = ECharacterPropertyDisplayType.PersonalityEnthusiastic;
						value = (int)featureConfig.PersonalityEnthusiastic;
					}
					else
					{
						bool flag4 = featureConfig.PersonalityBrave != 0;
						if (flag4)
						{
							type = ECharacterPropertyDisplayType.PersonalityBrave;
							value = (int)featureConfig.PersonalityBrave;
						}
						else
						{
							bool flag5 = featureConfig.PersonalityFirm != 0;
							if (flag5)
							{
								type = ECharacterPropertyDisplayType.PersonalityFirm;
								value = (int)featureConfig.PersonalityFirm;
							}
							else
							{
								bool flag6 = featureConfig.PersonalityLucky != 0;
								if (flag6)
								{
									type = ECharacterPropertyDisplayType.PersonalityLucky;
									value = (int)featureConfig.PersonalityLucky;
								}
								else
								{
									bool flag7 = featureConfig.PersonalityPerceptive != 0;
									if (flag7)
									{
										type = ECharacterPropertyDisplayType.PersonalityPerceptive;
										value = (int)featureConfig.PersonalityPerceptive;
									}
								}
							}
						}
					}
				}
			}
			return new ValueTuple<ECharacterPropertyDisplayType, int>(type, value);
		}

		// Token: 0x06006629 RID: 26153 RVA: 0x002EA8B4 File Offset: 0x002E8AB4
		public static string GetEffectDesc(short musicId, bool isTip)
		{
			ValueTuple<ECharacterPropertyDisplayType, int> effectTypeAndValue = ViewMusicPlayer.GetEffectTypeAndValue(musicId);
			ECharacterPropertyDisplayType type = effectTypeAndValue.Item1;
			int value = effectTypeAndValue.Item2;
			bool flag = type >= ECharacterPropertyDisplayType.PersonalityCalm && type <= ECharacterPropertyDisplayType.PersonalityPerceptive;
			string result;
			if (flag)
			{
				CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance[type.ToInt()];
				string valueText = (value > 0) ? string.Format("+{0}", value).SetColor("brightblue") : string.Format("-{0}", value).SetColor("brightred");
				string content = isTip ? LanguageKey.LK_Mousetip_Music_TempEffect_Content.TrFormat("<SpName=" + propertyConfig.TipsIcon + ">", propertyConfig.ShortName, valueText) : LanguageKey.LK_MusicPlayer_Effect.TrFormat(propertyConfig.ShortName, valueText);
				result = content;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600662A RID: 26154 RVA: 0x002EA98C File Offset: 0x002E8B8C
		private void InitFilterToggleTips()
		{
			List<CToggle> toggles = this.toggleGroupFilter.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				CToggle toggle = toggles[i];
				GameObject target = (toggle.targetGraphic != null) ? toggle.targetGraphic.gameObject : toggle.gameObject;
				TooltipInvoker tip = target.GetOrAddComponent<TooltipInvoker>();
				tip.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tip.RuntimeParam.Clear();
				tip.RuntimeParam.Set("arg0", ViewMusicPlayer.GetFilterToggleTipText((ViewMusicPlayer.TogFilterKey)i));
			}
		}

		// Token: 0x0600662B RID: 26155 RVA: 0x002EAA44 File Offset: 0x002E8C44
		private static string GetFilterToggleTipText(ViewMusicPlayer.TogFilterKey filterKey)
		{
			ECharacterPropertyDisplayType? displayType = ViewMusicPlayer.GetFilterDisplayType(filterKey);
			return (displayType == null) ? LanguageKey.LK_All.Tr() : CharacterPropertyDisplay.Instance[displayType.Value.ToInt()].ShortName;
		}

		// Token: 0x0600662C RID: 26156 RVA: 0x002EAA94 File Offset: 0x002E8C94
		private static ECharacterPropertyDisplayType? GetFilterDisplayType(ViewMusicPlayer.TogFilterKey filterKey)
		{
			if (!true)
			{
			}
			ECharacterPropertyDisplayType? result;
			switch (filterKey)
			{
			case ViewMusicPlayer.TogFilterKey.All:
				result = null;
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityCalm:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityCalm);
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityClever:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityClever);
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityEnthusiastic:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityEnthusiastic);
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityBrave:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityBrave);
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityFirm:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityFirm);
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityLucky:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityLucky);
				break;
			case ViewMusicPlayer.TogFilterKey.PersonalityPerceptive:
				result = new ECharacterPropertyDisplayType?(ECharacterPropertyDisplayType.PersonalityPerceptive);
				break;
			default:
				throw new ArgumentOutOfRangeException("filterKey", filterKey, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600662D RID: 26157 RVA: 0x002EAB3C File Offset: 0x002E8D3C
		private bool IsFilter(ViewMusicPlayer.TogFilterKey filterKey, ECharacterPropertyDisplayType displayType)
		{
			ECharacterPropertyDisplayType? keyType = ViewMusicPlayer.GetFilterDisplayType(filterKey);
			bool result;
			if (keyType != null)
			{
				ECharacterPropertyDisplayType? echaracterPropertyDisplayType = keyType;
				result = (echaracterPropertyDisplayType.GetValueOrDefault() == displayType & echaracterPropertyDisplayType != null);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600662E RID: 26158 RVA: 0x002EAB78 File Offset: 0x002E8D78
		public static string GetTimeStr(int time)
		{
			return string.Format("{0:D2}:{1:D2}", time / 60, time % 60);
		}

		// Token: 0x0600662F RID: 26159 RVA: 0x002EABA6 File Offset: 0x002E8DA6
		private void ShowPanelVolume(bool show)
		{
			this.panelVolume.SetActive(show);
		}

		// Token: 0x06006630 RID: 26160 RVA: 0x002EABB8 File Offset: 0x002E8DB8
		private void OnSliderValueChanged(float value)
		{
			value = (float)(Mathf.CeilToInt(value / (float)this._volumeStep) * this._volumeStep);
			SingletonObject.getInstance<GlobalSettings>().BgmVolume = (sbyte)value;
			this.textVolume.text = string.Format("{0}%", (sbyte)value);
			this.RefreshButtonVolume();
		}

		// Token: 0x06006631 RID: 26161 RVA: 0x002EAC10 File Offset: 0x002E8E10
		private void RefreshButtonVolume()
		{
			sbyte volume = SingletonObject.getInstance<GlobalSettings>().BgmVolume;
			bool isMute = volume == 0;
			bool isLow = volume >= 1 && volume <= 20;
			bool isMid = volume >= 21 && volume <= 60;
			bool isHigh = volume >= 61 && volume <= 100;
			this.buttonVolumeMute.gameObject.SetActive(isMute);
			this.buttonVolumeLow.gameObject.SetActive(isLow);
			this.buttonVolumeMid.gameObject.SetActive(isMid);
			this.buttonVolumeHigh.gameObject.SetActive(isHigh);
		}

		// Token: 0x06006632 RID: 26162 RVA: 0x002EACAC File Offset: 0x002E8EAC
		private void InitVolume()
		{
			this.panelVolume.gameObject.SetActive(false);
			sbyte volume = SingletonObject.getInstance<GlobalSettings>().BgmVolume;
			this.textVolume.text = string.Format("{0}%", volume);
			this.sliderVolume.SetValueWithoutNotify((float)volume);
			this.RefreshButtonVolume();
		}

		// Token: 0x04004753 RID: 18259
		[SerializeField]
		private CToggleGroup toggleGroupTab;

		// Token: 0x04004754 RID: 18260
		[SerializeField]
		private CToggleGroup toggleGroupFilter;

		// Token: 0x04004755 RID: 18261
		[SerializeField]
		private TMP_InputField searchInputField;

		// Token: 0x04004756 RID: 18262
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04004757 RID: 18263
		[SerializeField]
		private CButton buttonModeSingle;

		// Token: 0x04004758 RID: 18264
		[SerializeField]
		private CButton buttonModeList;

		// Token: 0x04004759 RID: 18265
		[SerializeField]
		private CButton buttonModeRandom;

		// Token: 0x0400475A RID: 18266
		[SerializeField]
		private CButton buttonPlay;

		// Token: 0x0400475B RID: 18267
		[SerializeField]
		private CButton buttonPause;

		// Token: 0x0400475C RID: 18268
		[SerializeField]
		private CButton buttonLast;

		// Token: 0x0400475D RID: 18269
		[SerializeField]
		private CButton buttonNext;

		// Token: 0x0400475E RID: 18270
		[SerializeField]
		private CButton buttonVolumeMute;

		// Token: 0x0400475F RID: 18271
		[SerializeField]
		private CButton buttonVolumeLow;

		// Token: 0x04004760 RID: 18272
		[SerializeField]
		private CButton buttonVolumeMid;

		// Token: 0x04004761 RID: 18273
		[SerializeField]
		private CButton buttonVolumeHigh;

		// Token: 0x04004762 RID: 18274
		[SerializeField]
		private CButton buttonHideVolume;

		// Token: 0x04004763 RID: 18275
		[SerializeField]
		private GameObject panelVolume;

		// Token: 0x04004764 RID: 18276
		[SerializeField]
		private CSlider sliderVolume;

		// Token: 0x04004765 RID: 18277
		[SerializeField]
		private TextMeshProUGUI textVolume;

		// Token: 0x04004766 RID: 18278
		[SerializeField]
		private CImage imageProgress;

		// Token: 0x04004767 RID: 18279
		[SerializeField]
		private TextMeshProUGUI textCurTime;

		// Token: 0x04004768 RID: 18280
		[SerializeField]
		private TextMeshProUGUI textMaxTime;

		// Token: 0x04004769 RID: 18281
		[SerializeField]
		private GameObject noneFavoriteObj;

		// Token: 0x0400476A RID: 18282
		[SerializeField]
		private MusicItem seledMusicItem;

		// Token: 0x0400476B RID: 18283
		private readonly List<short> _musicIdList = new List<short>();

		// Token: 0x0400476C RID: 18284
		private readonly List<short> _unlockedMusicIdList = new List<short>();

		// Token: 0x0400476D RID: 18285
		private readonly int _volumeStep = 10;

		// Token: 0x0400476E RID: 18286
		private readonly Dictionary<int, int> _tabLeadVisibleIndex = new Dictionary<int, int>();

		// Token: 0x02001D51 RID: 7505
		private enum TogTabKey
		{
			// Token: 0x0400C5C7 RID: 50631
			All,
			// Token: 0x0400C5C8 RID: 50632
			Favorite
		}

		// Token: 0x02001D52 RID: 7506
		private enum TogFilterKey
		{
			// Token: 0x0400C5CA RID: 50634
			All,
			// Token: 0x0400C5CB RID: 50635
			PersonalityCalm,
			// Token: 0x0400C5CC RID: 50636
			PersonalityClever,
			// Token: 0x0400C5CD RID: 50637
			PersonalityEnthusiastic,
			// Token: 0x0400C5CE RID: 50638
			PersonalityBrave,
			// Token: 0x0400C5CF RID: 50639
			PersonalityFirm,
			// Token: 0x0400C5D0 RID: 50640
			PersonalityLucky,
			// Token: 0x0400C5D1 RID: 50641
			PersonalityPerceptive
		}
	}
}
