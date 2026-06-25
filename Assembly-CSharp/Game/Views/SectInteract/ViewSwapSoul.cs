using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using EasyButtons;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Character.Display;
using GameData.Domains.Organization;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009AC RID: 2476
	public class ViewSwapSoul : UIBase
	{
		// Token: 0x17000D62 RID: 3426
		// (get) Token: 0x0600778D RID: 30605 RVA: 0x0037A089 File Offset: 0x00378289
		// (set) Token: 0x0600778E RID: 30606 RVA: 0x0037A091 File Offset: 0x00378291
		private bool IsShowingSwapSoul
		{
			get
			{
				return this._isShowingSwapSoul;
			}
			set
			{
				this._isShowingSwapSoul = value;
				this.mask.gameObject.SetActive(value);
			}
		}

		// Token: 0x0600778F RID: 30607 RVA: 0x0037A0AD File Offset: 0x003782AD
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ResetData();
			this.IsShowingSwapSoul = false;
			this.NeedDataListenerId = true;
			this.Element.OnListenerIdReady = new Action(this.RequestAllCharacters);
		}

		// Token: 0x06007790 RID: 30608 RVA: 0x0037A0E0 File Offset: 0x003782E0
		private void ResetData()
		{
			for (int i = 0; i < this._selectedSoulCharIdList.Count; i++)
			{
				this._selectedSoulCharIdList[i] = -1;
				this._lastSelectedSoulCharIdList[i] = -1;
			}
			this._selectedBodyCharId = -1;
			this._lastSelectedBodyCharId = -1;
			this._selectedFeatureIds.Clear();
			this.characterDisplayDataDict.Clear();
		}

		// Token: 0x06007791 RID: 30609 RVA: 0x0037A148 File Offset: 0x00378348
		private void CacheLastSelectedSoulCharIds()
		{
			for (int i = 0; i < this._lastSelectedSoulCharIdList.Length; i++)
			{
				this._lastSelectedSoulCharIdList[i] = ((i < this._selectedSoulCharIdList.Count) ? this._selectedSoulCharIdList[i] : -1);
			}
		}

		// Token: 0x06007792 RID: 30610 RVA: 0x0037A194 File Offset: 0x00378394
		private bool CheckSelectedSoulCharacters()
		{
			for (int i = 0; i < this._soulLimit; i++)
			{
				bool flag = this._selectedSoulCharIdList[i] >= 0;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007793 RID: 30611 RVA: 0x0037A1DC File Offset: 0x003783DC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 9 && notification.MethodId == 126;
					if (flag2)
					{
						List<SamsaraPlatformCharDisplayData> list = null;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list);
						bool flag3 = list != null;
						if (flag3)
						{
							this._soulCharIdList = list.ConvertAll<int>((SamsaraPlatformCharDisplayData e) => e.Id);
						}
						else
						{
							bool flag4 = this._soulCharIdList != null;
							if (flag4)
							{
								this._soulCharIdList.Clear();
							}
						}
					}
					else
					{
						bool flag5 = notification.DomainId == 9 && notification.MethodId == 117;
						if (flag5)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._bodyCharIdList);
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x06007794 RID: 30612 RVA: 0x0037A324 File Offset: 0x00378524
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "EditAvatarBtn" == btnName;
			if (flag)
			{
				this.EditAvatar();
			}
			else
			{
				bool flag2 = "CloseBtn" == btnName;
				if (flag2)
				{
					this.QuickHide();
				}
				else
				{
					bool flag3 = "ConfirmBtn" == btnName;
					if (flag3)
					{
						this.StartSwapSoul();
					}
					else
					{
						bool flag4 = "SelectFeatureBtn" == btnName;
						if (flag4)
						{
							this.SelectFeature();
						}
						else
						{
							bool flag5 = "ResultCharacterAvtar" == btnName;
							if (flag5)
							{
								bool flag6 = this._resultCharacterCharId != -1;
								if (flag6)
								{
									this.OpenCharacterMenu(this._resultCharacterCharId);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06007795 RID: 30613 RVA: 0x0037A3DC File Offset: 0x003785DC
		private void OpenCharacterMenu(int charId)
		{
			bool isShowingSwapSoul = this.IsShowingSwapSoul;
			if (!isShowingSwapSoul)
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("CanOperate", false);
				box.Set("CharacterId", charId);
				box.Set("PreviousView", 10);
				UIElement.CharacterMenu.SetOnInitArgs(box);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06007796 RID: 30614 RVA: 0x0037A444 File Offset: 0x00378644
		public override void QuickHide()
		{
			bool flag = this._selectedBodyCharId > -1 || this.CheckSelectedSoulCharacters();
			if (flag)
			{
				bool isShowingSwapSoul = this.IsShowingSwapSoul;
				if (!isShowingSwapSoul)
				{
					DialogCmd cmd = new DialogCmd();
					cmd.Type = 1;
					cmd.Title = LocalStringManager.Get(LanguageKey.UI_SoulSwap_CancelSwap);
					cmd.Content = LocalStringManager.Get(LanguageKey.UI_SoulSwap_CancelSwap_ConfirmTips);
					cmd.Yes = new Action(base.QuickHide);
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x06007797 RID: 30615 RVA: 0x0037A4E8 File Offset: 0x003786E8
		private void Awake()
		{
			this.PiecesStartPositions = new Vector2[this.PropertyPieces.Length];
			for (int i = 0; i < this.PropertyPieces.Length; i++)
			{
				this.PiecesStartPositions[i] = this.PropertyPieces[i].rectTransform.localPosition;
			}
			this.InitAllPiecesTips();
		}

		// Token: 0x06007798 RID: 30616 RVA: 0x0037A54C File Offset: 0x0037874C
		private void InitAllPiecesTips()
		{
			this.ReincarnationPieceTip.Type = TipType.SoulPiece;
			TooltipInvoker tooltipInvoker = this.ReincarnationPieceTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.ReincarnationPieceTip.RuntimeParam.SetObject("GetArgsBoxFunc", new Func<ArgumentBox>(delegate
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("Type", 0);
				box.SetObject("CharacterSamsaraData", this._resultPreview.CharacterSamsaraData);
				return box;
			}));
			this.FeaturePieceTip.Type = TipType.SoulPiece;
			tooltipInvoker = this.FeaturePieceTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.FeaturePieceTip.RuntimeParam.SetObject("GetArgsBoxFunc", new Func<ArgumentBox>(delegate
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("Type", 1);
				box.SetObject("DisplayFeatureIds", this.GetPreviewFeatures());
				return box;
			}));
			this.MainAttributePieceTip.Type = TipType.SoulPiece;
			tooltipInvoker = this.MainAttributePieceTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.MainAttributePieceTip.RuntimeParam.SetObject("GetArgsBoxFunc", new Func<ArgumentBox>(delegate
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("Type", 2);
				box.SetObject("BaseMainAttributes", this._resultPreview.BaseMainAttributes);
				return box;
			}));
			this.LifeAndCombatSkillsValuePieceTip.Type = TipType.SoulPiece;
			tooltipInvoker = this.LifeAndCombatSkillsValuePieceTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.LifeAndCombatSkillsValuePieceTip.RuntimeParam.SetObject("GetArgsBoxFunc", new Func<ArgumentBox>(delegate
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("Type", 3);
				box.SetObject("LifeSkillShorts", this._resultPreview.BaseLifeSkillQualifications);
				box.SetObject("CombatSkillShorts", this._resultPreview.BaseCombatSkillQualifications);
				return box;
			}));
			this.BehaviorAndHappinessPieceTip.Type = TipType.SoulPiece;
			tooltipInvoker = this.BehaviorAndHappinessPieceTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.BehaviorAndHappinessPieceTip.RuntimeParam.SetObject("GetArgsBoxFunc", new Func<ArgumentBox>(delegate
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("Type", 4);
				box.Set("BehaviorType", GameData.Domains.Character.BehaviorType.GetBehaviorType(this._resultPreview.BaseMorality));
				box.Set("HappinessLevel", HappinessType.GetHappinessType(this._resultPreview.Happiness));
				return box;
			}));
		}

		// Token: 0x06007799 RID: 30617 RVA: 0x0037A6E8 File Offset: 0x003788E8
		private void OnEnable()
		{
			AudioManager.Instance.PlayAmbience("SFX_Swapsoul_amb_loop", 1.5f, 100);
			this.ResetPageEffects();
			this.RefreshSoulCharacters(null);
			this.RefreshBodyCharacter(-1);
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
		}

		// Token: 0x0600779A RID: 30618 RVA: 0x0037A73C File Offset: 0x0037893C
		private void OnDisable()
		{
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1.5f, 100);
			AudioManager.Instance.StopSound("SFX_Swapsoul_beam_loop");
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
			base.StopAllCoroutines();
			for (int i = 0; i < this._soulCharacterEffectCoroutine.Length; i++)
			{
				this._soulCharacterEffectCoroutine[i] = null;
			}
			this._bodyCharacterEffectCoroutine = null;
		}

		// Token: 0x0600779B RID: 30619 RVA: 0x0037A7BC File Offset: 0x003789BC
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, true, false, true, false);
			if (flag)
			{
				this.StartSwapSoul();
			}
		}

		// Token: 0x0600779C RID: 30620 RVA: 0x0037A7EC File Offset: 0x003789EC
		private void RequestAllCharacters()
		{
			BuildingDomainMethod.Call.GetSwapSoulCeremonySoulCharIdList(this.Element.GameDataListenerId);
			OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 11, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked, delegate(int offset, RawDataPool pool)
			{
				bool unlock = false;
				Serializer.Deserialize(pool, offset, ref unlock);
				this._soulLimit = (unlock ? 3 : 1);
				for (int i = 0; i < this.soulCharacterItems.Count; i++)
				{
					this.soulCharacterItems[i].Set(null, this._soulLimit);
				}
			});
			BuildingDomainMethod.Call.GetSwapSoulCeremonyBodyCharIdList(this.Element.GameDataListenerId);
		}

		// Token: 0x0600779D RID: 30621 RVA: 0x0037A828 File Offset: 0x00378A28
		[Button("切换升级互动")]
		public void Set(bool unlock)
		{
			this._soulLimit = (unlock ? 3 : 1);
			for (int i = 0; i < this.soulCharacterItems.Count; i++)
			{
				this.soulCharacterItems[i].Set(null, this._soulLimit);
			}
			for (int j = 0; j < this._selectedSoulCharIdList.Count; j++)
			{
				this._selectedSoulCharIdList[j] = -1;
			}
			this.bodyCharacterItem.Set(null, 1);
			this._selectedBodyCharId = -1;
			this._selectedFeatureIds.Clear();
			this.characterDisplayDataDict.Clear();
		}

		// Token: 0x0600779E RID: 30622 RVA: 0x0037A8D0 File Offset: 0x00378AD0
		private void OnTopUIChanged(ArgumentBox argBox)
		{
			bool flag = UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
			if (flag)
			{
				base.transform.localPosition = Vector3.up * 5000f;
			}
			else
			{
				base.transform.localPosition = Vector3.zero;
			}
		}

		// Token: 0x0600779F RID: 30623 RVA: 0x0037A91F File Offset: 0x00378B1F
		private void RefreshSoulCharacters(List<int> selectedCharacterIds = null)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, selectedCharacterIds, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> data = null;
				Serializer.Deserialize(pool, offset, ref data);
				for (int i = 0; i < this.soulCharacterItems.Count; i++)
				{
					bool flag = data.CheckIndex(i) && data[i].CharacterId >= 0;
					if (flag)
					{
						bool IsEmpty = this.soulCharacterItems[i].IsEmpty;
						this.soulCharacterItems[i].Set(data[i], this._soulLimit);
						this.characterDisplayDataDict[data[i].CharacterId] = data[i];
						this._selectedSoulCharIdList[i] = data[i].CharacterId;
					}
					else
					{
						this.soulCharacterItems[i].Set(null, this._soulLimit);
						this._selectedSoulCharIdList[i] = -1;
					}
					bool flag2 = this._soulCharacterEffectCoroutine[i] != null;
					if (flag2)
					{
						base.StopCoroutine(this._soulCharacterEffectCoroutine[i]);
					}
					this._soulCharacterEffectCoroutine[i] = base.StartCoroutine(this.UpdateSoulCharacterEffect(i));
				}
				this.RefreshResultCharacter();
			});
		}

		// Token: 0x060077A0 RID: 30624 RVA: 0x0037A938 File Offset: 0x00378B38
		private void RefreshBodyCharacter(int charId)
		{
			this._selectedBodyCharId = charId;
			bool flag = this._selectedBodyCharId >= 0;
			if (flag)
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, charId, delegate(int offset, RawDataPool pool)
				{
					CharacterDisplayData data = null;
					Serializer.Deserialize(pool, offset, ref data);
					this.bodyCharacterItem.Set(data, 1);
					this.characterDisplayDataDict[data.CharacterId] = data;
					this.RefreshResultCharacter();
				});
			}
			else
			{
				this.bodyCharacterItem.Set(null, 1);
				this.RefreshResultCharacter();
			}
			CImage image = this.fameIcon;
			TooltipInvoker mouseTip = image.GetComponent<TooltipInvoker>();
			bool flag2 = charId != -1;
			if (flag2)
			{
				CharacterDomainMethod.AsyncCall.GetFameType(this, charId, delegate(int offset, RawDataPool dataPool)
				{
					sbyte fameType = 3;
					Serializer.Deserialize(dataPool, offset, ref fameType);
					bool flag4 = fameType > 3;
					if (flag4)
					{
						image.enabled = true;
						image.sprite = this.fameIconSprites[0];
						TooltipInvoker mouseTip;
						mouseTip.enabled = true;
						mouseTip = mouseTip;
						if (mouseTip.RuntimeParam == null)
						{
							mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
						}
						mouseTip.RuntimeParam.Set("FeatureId", 740);
					}
					else
					{
						bool flag5 = fameType < 3;
						if (flag5)
						{
							image.enabled = true;
							image.sprite = this.fameIconSprites[1];
							TooltipInvoker mouseTip;
							mouseTip.enabled = true;
							mouseTip = mouseTip;
							if (mouseTip.RuntimeParam == null)
							{
								mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
							}
							mouseTip.RuntimeParam.Set("FeatureId", 741);
						}
						else
						{
							image.SetSprite(string.Empty, false, null);
							TooltipInvoker mouseTip;
							mouseTip.enabled = false;
						}
					}
				});
			}
			else
			{
				image.SetSprite(string.Empty, false, null);
				mouseTip.enabled = false;
			}
			bool flag3 = this._bodyCharacterEffectCoroutine != null;
			if (flag3)
			{
				base.StopCoroutine(this._bodyCharacterEffectCoroutine);
			}
			this._bodyCharacterEffectCoroutine = base.StartCoroutine(this.UpdateBodyCharacterEffect());
		}

		// Token: 0x060077A1 RID: 30625 RVA: 0x0037AA2C File Offset: 0x00378C2C
		private void RefreshResultCharacter()
		{
			this.soulLackHolder.gameObject.SetActive(!this.CheckSelectedSoulCharacters());
			this.bodyLackHolder.gameObject.SetActive(this._selectedBodyCharId == -1);
			bool flag = !this.CheckSelectedSoulCharacters() || this._selectedBodyCharId == -1;
			if (flag)
			{
				int i = 0;
				int max = this.PropertyPieces.Length;
				while (i < max)
				{
					IrregularClickableImage elem = this.PropertyPieces[i];
					elem.rectTransform.DOKill(false);
					elem.gameObject.SetActive(false);
					i++;
				}
				this._resultCharacterCharId = -1;
				this.resultCharacterAvatar.gameObject.SetActive(false);
				this.resultEmpty.gameObject.SetActive(true);
				this.resultCharName.text = "? ? ?";
			}
			else
			{
				this.resultCharacterAvatar.gameObject.SetActive(true);
				this.resultEmpty.gameObject.SetActive(false);
				this.GetPossessionPreview();
				this._selectedFeatureIds.Clear();
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, this._selectedBodyCharId, delegate(int offset, RawDataPool pool)
				{
					CharacterDisplayData data = null;
					Serializer.Deserialize(pool, offset, ref data);
					this.resultCharacterAvatar.Refresh(data, true);
				});
			}
			bool flag2 = !this.CheckSelectedSoulCharacters() && this._selectedBodyCharId == -1;
			if (flag2)
			{
				AudioManager.Instance.StopSound("SFX_Swapsoul_beam_loop");
			}
			else
			{
				bool flag3 = !AudioManager.Instance.IsPlayingSound("SFX_Swapsoul_beam_loop");
				if (flag3)
				{
					AudioManager.Instance.PlaySound("SFX_Swapsoul_beam_loop", true, false);
				}
			}
			this.RefreshEditAvatarButton();
			this.RefreshSelectFeatureButton();
			this.RefreshConfirmButton();
		}

		// Token: 0x060077A2 RID: 30626 RVA: 0x0037ABC8 File Offset: 0x00378DC8
		private void GetPossessionPreview()
		{
			BuildingDomainMethod.AsyncCall.GetPossessionPreview(this, this._selectedSoulCharIdList, this._selectedBodyCharId, this._selectedFeatureIds.ToList<short>(), this._resultCharacterCharId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._resultPreview);
				this._resultCharacterCharId = this._resultPreview.Id;
				int i = 0;
				int max = this.PropertyPieces.Length;
				while (i < max)
				{
					IrregularClickableImage elem = this.PropertyPieces[i];
					elem.enabled = true;
					elem.gameObject.SetActive(true);
					elem.rectTransform.DOKill(false);
					elem.rectTransform.anchoredPosition = this.PiecesStartPositions[i];
					elem.rectTransform.DOLocalMoveY(20f, 1.5f, false).SetRelative(true).SetLoops(-1, LoopType.Yoyo).SetDelay(Random.Range(0.3f, 1.2f));
					i++;
				}
				TooltipInvoker lifeAndCombatSkillsValuePieceTip = this.LifeAndCombatSkillsValuePieceTip;
				if (lifeAndCombatSkillsValuePieceTip.RuntimeParam == null)
				{
					lifeAndCombatSkillsValuePieceTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.LifeAndCombatSkillsValuePieceTip.RuntimeParam.Set("charId", this._resultPreview.Id);
			});
		}

		// Token: 0x060077A3 RID: 30627 RVA: 0x0037ABFC File Offset: 0x00378DFC
		private void SetMouseTipDisableReason(TooltipInvoker mouseTip)
		{
			List<string> reasonList = new List<string>();
			bool flag = this._selectedBodyCharId == -1;
			if (flag)
			{
				reasonList.Add(LocalStringManager.Get(LanguageKey.UI_SoulSwap_Confirm_Tip_LackBody));
			}
			bool flag2 = !this.CheckSelectedSoulCharacters();
			if (flag2)
			{
				reasonList.Add(LocalStringManager.Get(LanguageKey.UI_SoulSwap_Confirm_Tip_LackSoul));
			}
			mouseTip.PresetParam[0] = string.Join("\n", reasonList);
			mouseTip.enabled = true;
		}

		// Token: 0x060077A4 RID: 30628 RVA: 0x0037AC68 File Offset: 0x00378E68
		private void RefreshEditAvatarButton()
		{
			CButton btn = this.editAvatarBtn;
			bool interactive = this.CheckSelectedSoulCharacters() && this._selectedBodyCharId != -1;
			btn.interactable = interactive;
		}

		// Token: 0x060077A5 RID: 30629 RVA: 0x0037ACA0 File Offset: 0x00378EA0
		private void RefreshSelectFeatureButton()
		{
			CButton btn = this.selectFeatureBtn;
			this.CalcFeatureIds();
			bool hasConflict = this._featureIdGroupList.Count > 0 && this._selectedBodyCharId != -1;
			bool allResolved = this.CheckAllConflictGroupsResolved();
			this.selectFeatureIcon.gameObject.SetActive(hasConflict && !allResolved);
			this.selectFeatureBtn.interactable = hasConflict;
		}

		// Token: 0x060077A6 RID: 30630 RVA: 0x0037AD0C File Offset: 0x00378F0C
		private bool CheckAllConflictGroupsResolved()
		{
			foreach (ValueTuple<int, List<short>> valueTuple in this._featureIdGroupList)
			{
				List<short> featureIds = valueTuple.Item2;
				bool hasSelected = false;
				foreach (short id in featureIds)
				{
					bool flag = this._selectedFeatureIds.Contains(id);
					if (flag)
					{
						hasSelected = true;
						break;
					}
				}
				bool flag2 = !hasSelected;
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060077A7 RID: 30631 RVA: 0x0037ADD0 File Offset: 0x00378FD0
		private void RefreshConfirmButton()
		{
			CButton btn = this.confirmBtn;
			TooltipInvoker mouseTip = btn.GetComponent<TooltipInvoker>();
			bool interactive = this.CheckSelectedSoulCharacters() && this._selectedBodyCharId != -1;
			bool flag = !interactive;
			if (flag)
			{
				btn.interactable = false;
				this.SetMouseTipDisableReason(mouseTip);
			}
			else
			{
				btn.interactable = true;
				mouseTip.enabled = false;
			}
		}

		// Token: 0x060077A8 RID: 30632 RVA: 0x0037AE30 File Offset: 0x00379030
		private void EditAvatar()
		{
			AvatarData avatarData = this.resultCharacterAvatar.Data;
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			bool transGender = avatarData.AvatarId % 2 != (byte)this._resultPreview.Gender;
			box.Set("Gender", this._resultPreview.Gender);
			box.Set("TransGender", transGender);
			box.Set("Age", this._resultPreview.Age);
			box.Set("BornMonth", SingletonObject.getInstance<TimeManager>().GetMonthInYear(this._resultPreview.BirthDate));
			box.SetObject("AvatarData", avatarData);
			box.SetObject("RandomAvatarHandler", new Func<AvatarData>(this.RandomAvatarHandler));
			box.SetObject("OnEditComplete", new Action<AvatarData>(delegate(AvatarData v)
			{
				BuildingDomainMethod.Call.SetTemporaryPossessionCharacterAvatar(v);
				this.resultCharacterAvatar.Refresh(v, this._resultPreview.Age);
			}));
			box.SetObject("BodyTypeFilter", new Func<sbyte, bool>(this.BodyTypeFilter));
			box.SetObject("SkinColorFilter", new Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>>(this.SkinColorFilter));
			box.SetObject("FrontHairFilter", new Func<List<HairRes>, List<HairRes>>(this.FrontHairResFilter));
			box.SetObject("BackHairFilter", new Func<List<HairRes>, List<HairRes>>(this.BackHairResFilter));
			box.SetObject("EyeBrowFilter", new Func<List<AvatarAsset>, List<AvatarAsset>>(this.EyebrowFilter));
			box.SetObject("EyesFilter", new Func<List<EyeRes>, List<EyeRes>>(this.EyesFilter));
			box.SetObject("NoseFilter", new Func<List<AvatarAsset>, List<AvatarAsset>>(this.NoseFilter));
			box.SetObject("MouthFilter", new Func<List<MouthRes>, List<MouthRes>>(this.MouthFilter));
			box.SetObject("Beard1Filter", new Func<List<AvatarAsset>, List<AvatarAsset>>(this.Beard1Filter));
			box.SetObject("Beard2Filter", new Func<List<AvatarAsset>, List<AvatarAsset>>(this.Beard2Filter));
			box.SetObject("Feature1Filter", new Func<List<AvatarAsset>, List<AvatarAsset>>(this.Feature1Filter));
			box.SetObject("Feature2Filter", new Func<List<AvatarAsset>, List<AvatarAsset>>(this.Feature2Filter));
			box.SetObject("CanShaveHairBald", new Func<bool>(this.CanShaveHairBald));
			box.SetObject("CanShaveBeard1Bald", new Func<bool>(this.CanShaveBeard1Bald));
			box.SetObject("CanShaveBeard2Bald", new Func<bool>(this.CanShaveBeard2Bald));
			box.SetObject("CanShaveEyebrowBald", new Func<bool>(this.CanShaveEyebrowBald));
			UIElement.EditAvatar.SetOnInitArgs(box);
			UIManager.Instance.MaskUI(UIElement.EditAvatar);
		}

		// Token: 0x060077A9 RID: 30633 RVA: 0x0037B0A0 File Offset: 0x003792A0
		private AvatarData RandomAvatarHandler()
		{
			List<AvatarData> availableSoulCharacters = new List<AvatarData>();
			for (int i = 0; i < this._soulLimit; i++)
			{
				bool flag = this.soulCharacterItems[i] != null && this.soulCharacterItems[i].AvatarData != null;
				if (flag)
				{
					availableSoulCharacters.Add(this.soulCharacterItems[i].AvatarData);
				}
			}
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			AvatarData avatarData = new AvatarData();
			avatarData.Copy(bodyAvatarData);
			bool flag2 = availableSoulCharacters.Count == 0;
			AvatarData result;
			if (flag2)
			{
				result = avatarData;
			}
			else
			{
				AvatarData soulAvatarData = availableSoulCharacters[Random.Range(0, availableSoulCharacters.Count)];
				bool flag3 = Utils_Random.RandomCheck(50, 100);
				if (flag3)
				{
					avatarData.ChangeBodyType(soulAvatarData.GetBodyType());
				}
				bool flag4 = Utils_Random.RandomCheck(50, 100);
				if (flag4)
				{
					avatarData.ColorSkinId = soulAvatarData.ColorSkinId;
				}
				bool flag5 = Utils_Random.RandomCheck(50, 100);
				if (flag5)
				{
					avatarData.FrontHairId = soulAvatarData.FrontHairId;
				}
				bool flag6 = Utils_Random.RandomCheck(50, 100);
				if (flag6)
				{
					avatarData.BackHairId = soulAvatarData.BackHairId;
				}
				bool flag7 = Utils_Random.RandomCheck(50, 100);
				if (flag7)
				{
					avatarData.EyebrowId = soulAvatarData.EyebrowId;
				}
				bool flag8 = Utils_Random.RandomCheck(50, 100);
				if (flag8)
				{
					avatarData.EyesMainId = soulAvatarData.EyesMainId;
					AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)avatarData.AvatarId);
					bool flag9 = avatarData.EyesLeftId != 0;
					if (flag9)
					{
						AvatarAsset asset = group.Get(EAvatarElementsType.Eye, new short[]
						{
							avatarData.EyesMainId,
							avatarData.EyesLeftId
						});
						bool flag10 = asset == null;
						if (flag10)
						{
							avatarData.EyesLeftId = 0;
						}
					}
					bool flag11 = avatarData.EyesRightId != 0;
					if (flag11)
					{
						AvatarAsset asset2 = group.Get(EAvatarElementsType.Eye, new short[]
						{
							avatarData.EyesMainId,
							avatarData.EyesRightId
						});
						bool flag12 = asset2 == null;
						if (flag12)
						{
							avatarData.EyesRightId = 0;
						}
					}
				}
				bool flag13 = Utils_Random.RandomCheck(50, 100);
				if (flag13)
				{
					avatarData.NoseId = soulAvatarData.NoseId;
				}
				bool flag14 = Utils_Random.RandomCheck(50, 100);
				if (flag14)
				{
					avatarData.MouthId = soulAvatarData.MouthId;
				}
				bool flag15 = Utils_Random.RandomCheck(50, 100);
				if (flag15)
				{
					avatarData.Beard1Id = soulAvatarData.Beard1Id;
				}
				bool flag16 = Utils_Random.RandomCheck(50, 100);
				if (flag16)
				{
					avatarData.Beard2Id = soulAvatarData.Beard2Id;
				}
				bool flag17 = Utils_Random.RandomCheck(50, 100);
				if (flag17)
				{
					avatarData.Feature1Id = soulAvatarData.Feature1Id;
				}
				bool flag18 = Utils_Random.RandomCheck(50, 100);
				if (flag18)
				{
					avatarData.Feature2Id = soulAvatarData.Feature2Id;
				}
				result = avatarData;
			}
			return result;
		}

		// Token: 0x060077AA RID: 30634 RVA: 0x0037B354 File Offset: 0x00379554
		private bool BodyTypeFilter(sbyte bodyType)
		{
			bool flag = bodyType == this.bodyCharacterItem.AvatarData.GetBodyType();
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && bodyType == soulAvatarData.GetBodyType();
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060077AB RID: 30635 RVA: 0x0037B3D0 File Offset: 0x003795D0
		private List<ValueTuple<byte, Color>> SkinColorFilter(List<ValueTuple<byte, Color>> colorList)
		{
			List<ValueTuple<byte, Color>> result = new List<ValueTuple<byte, Color>>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			colorList.ForEach(delegate(ValueTuple<byte, Color> e)
			{
				bool flag = e.Item1 == bodyAvatarData.ColorSkinId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Item1 == soulAvatarData.ColorSkinId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077AC RID: 30636 RVA: 0x0037B424 File Offset: 0x00379624
		private List<HairRes> FrontHairResFilter(List<HairRes> assetsList)
		{
			List<HairRes> result = new List<HairRes>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(HairRes e)
			{
				bool flag = e.Id == bodyAvatarData.FrontHairId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.FrontHairId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077AD RID: 30637 RVA: 0x0037B478 File Offset: 0x00379678
		private List<HairRes> BackHairResFilter(List<HairRes> assetsList)
		{
			List<HairRes> result = new List<HairRes>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(HairRes e)
			{
				bool flag = e.Id == bodyAvatarData.BackHairId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.BackHairId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077AE RID: 30638 RVA: 0x0037B4CC File Offset: 0x003796CC
		private List<AvatarAsset> EyebrowFilter(List<AvatarAsset> assetsList)
		{
			List<AvatarAsset> result = new List<AvatarAsset>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(AvatarAsset e)
			{
				bool flag = e.Id == bodyAvatarData.EyebrowId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.EyebrowId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077AF RID: 30639 RVA: 0x0037B520 File Offset: 0x00379720
		private List<EyeRes> EyesFilter(List<EyeRes> assetsList)
		{
			List<EyeRes> result = new List<EyeRes>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(EyeRes e)
			{
				bool flag = e.Id == bodyAvatarData.EyesMainId && e.LeftEye.SubId == bodyAvatarData.EyesLeftId && e.RightEye.SubId == bodyAvatarData.EyesRightId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.EyesMainId && e.LeftEye.SubId == soulAvatarData.EyesLeftId && e.RightEye.SubId == soulAvatarData.EyesRightId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B0 RID: 30640 RVA: 0x0037B574 File Offset: 0x00379774
		private List<AvatarAsset> NoseFilter(List<AvatarAsset> assetsList)
		{
			List<AvatarAsset> result = new List<AvatarAsset>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(AvatarAsset e)
			{
				bool flag = e.Id == bodyAvatarData.NoseId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.NoseId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B1 RID: 30641 RVA: 0x0037B5C8 File Offset: 0x003797C8
		private List<MouthRes> MouthFilter(List<MouthRes> assetsList)
		{
			List<MouthRes> result = new List<MouthRes>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(MouthRes e)
			{
				bool flag = e.Id == bodyAvatarData.MouthId && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.MouthId && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B2 RID: 30642 RVA: 0x0037B61C File Offset: 0x0037981C
		private List<AvatarAsset> Beard1Filter(List<AvatarAsset> assetsList)
		{
			List<AvatarAsset> result = new List<AvatarAsset>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(AvatarAsset e)
			{
				bool flag = e.Id == bodyAvatarData.Beard1Id && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.Beard1Id && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B3 RID: 30643 RVA: 0x0037B670 File Offset: 0x00379870
		private List<AvatarAsset> Beard2Filter(List<AvatarAsset> assetsList)
		{
			List<AvatarAsset> result = new List<AvatarAsset>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(AvatarAsset e)
			{
				bool flag = e.Id == bodyAvatarData.Beard2Id && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.Beard2Id && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B4 RID: 30644 RVA: 0x0037B6C4 File Offset: 0x003798C4
		private List<AvatarAsset> Feature1Filter(List<AvatarAsset> assetsList)
		{
			List<AvatarAsset> result = new List<AvatarAsset>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(AvatarAsset e)
			{
				bool flag = e.Id == bodyAvatarData.Feature1Id && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.Feature1Id && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B5 RID: 30645 RVA: 0x0037B718 File Offset: 0x00379918
		private List<AvatarAsset> Feature2Filter(List<AvatarAsset> assetsList)
		{
			List<AvatarAsset> result = new List<AvatarAsset>();
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			assetsList.ForEach(delegate(AvatarAsset e)
			{
				bool flag = e.Id == bodyAvatarData.Feature2Id && !result.Contains(e);
				if (flag)
				{
					result.Add(e);
				}
				for (int i = 0; i < this._soulLimit; i++)
				{
					SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
					AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
					bool flag2 = soulAvatarData != null && e.Id == soulAvatarData.Feature2Id && !result.Contains(e);
					if (flag2)
					{
						result.Add(e);
					}
				}
			});
			return result;
		}

		// Token: 0x060077B6 RID: 30646 RVA: 0x0037B76C File Offset: 0x0037996C
		private bool CanShaveHairBald()
		{
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			bool result = !bodyAvatarData.GetGrowableElementShowingState(0) || !bodyAvatarData.GetGrowableElementShowingAbility(0);
			for (int i = 0; i < this._soulLimit; i++)
			{
				SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
				AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
				bool flag = soulAvatarData != null;
				if (flag)
				{
					result = (result || !soulAvatarData.GetGrowableElementShowingState(0) || !soulAvatarData.GetGrowableElementShowingAbility(0));
				}
			}
			return result;
		}

		// Token: 0x060077B7 RID: 30647 RVA: 0x0037B7FC File Offset: 0x003799FC
		private bool CanShaveBeard1Bald()
		{
			return true;
		}

		// Token: 0x060077B8 RID: 30648 RVA: 0x0037B810 File Offset: 0x00379A10
		private bool CanShaveBeard2Bald()
		{
			return true;
		}

		// Token: 0x060077B9 RID: 30649 RVA: 0x0037B824 File Offset: 0x00379A24
		private bool CanShaveEyebrowBald()
		{
			AvatarData bodyAvatarData = this.bodyCharacterItem.AvatarData;
			bool result = !bodyAvatarData.GetGrowableElementShowingState(6) || !bodyAvatarData.GetGrowableElementShowingAbility(6);
			for (int i = 0; i < this._soulLimit; i++)
			{
				SwapSoulCharacterItem swapSoulCharacterItem = this.soulCharacterItems[i];
				AvatarData soulAvatarData = (swapSoulCharacterItem != null) ? swapSoulCharacterItem.AvatarData : null;
				bool flag = soulAvatarData != null;
				if (flag)
				{
					result = (result || !soulAvatarData.GetGrowableElementShowingState(6) || !soulAvatarData.GetGrowableElementShowingAbility(6));
				}
			}
			return result;
		}

		// Token: 0x060077BA RID: 30650 RVA: 0x0037B8B4 File Offset: 0x00379AB4
		private void StartSwapSoul()
		{
			bool isShowingSwapSoul = this.IsShowingSwapSoul;
			if (!isShowingSwapSoul)
			{
				bool flag = !this.CheckAllConflictGroupsResolved();
				if (flag)
				{
					DialogCmd conflictCmd = new DialogCmd();
					conflictCmd.Type = 1;
					conflictCmd.Title = LanguageKey.UI_SoulSwap_FeatureConflict.Tr();
					conflictCmd.Content = LanguageKey.UI_SoulSwap_FeatureConflict_Content.Tr();
					conflictCmd.Yes = delegate()
					{
						this.SelectFeature();
					};
					conflictCmd.No = delegate()
					{
						this.ShowSwapSoulConfirmDialog();
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", conflictCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					this.ShowSwapSoulConfirmDialog();
				}
			}
		}

		// Token: 0x060077BB RID: 30651 RVA: 0x0037B96C File Offset: 0x00379B6C
		private void ShowSwapSoulConfirmDialog()
		{
			DialogCmd cmd = new DialogCmd();
			cmd.Type = 1;
			cmd.Title = LocalStringManager.Get(LanguageKey.UI_SoulSwap_CompleteSwap);
			cmd.Content = LocalStringManager.Get(LanguageKey.UI_SoulSwap_SwapConfirmTips);
			cmd.Yes = delegate()
			{
				base.StartCoroutine(this.CoroutineSwapSoul());
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060077BC RID: 30652 RVA: 0x0037B9E4 File Offset: 0x00379BE4
		private IEnumerator CoroutineSwapSoul()
		{
			AudioManager.Instance.PlayAmbience("SFX_Swapsoul_change_amb_loop", 1.5f, 100);
			this.IsShowingSwapSoul = true;
			this.confirmBtn.interactable = false;
			this.MonkAnim.AnimationState.SetAnimation(0, "SwapSoul_action", false);
			this.MonkHandAnim.AnimationState.SetAnimation(0, "SwapSoul_action", false);
			AudioManager.Instance.PlaySound("SFX_Swapsoul_hand", false, false);
			this.DoorAnim.timeScale = 1f;
			this.BehindDoorEffect.SetActive(true);
			this.DoorAnim.AnimationState.SetAnimation(0, "click", false);
			AudioManager.Instance.PlaySound("SFX_Swapsoul_door", false, false);
			yield return new WaitForSeconds(1f);
			this.BlueSceneRoot.SetActive(false);
			this.RedSceneRoot.SetActive(true);
			yield return new WaitForSeconds(0.3f);
			this.soulCharacterItems.ForEach(delegate(SwapSoulCharacterItem v)
			{
				v.Set(null, this._soulLimit);
			});
			this.bodyCharacterItem.Set(null, 1);
			Array.ForEach<RectTransform>(this.SoulCharacterRectTransform, delegate(RectTransform e)
			{
				e.gameObject.SetActive(false);
			});
			this.BodyCharacterRectTransform.gameObject.SetActive(false);
			AudioManager.Instance.PlaySound("SFX_Swapsoul_lighting", false, false);
			Array.ForEach<GameObject>(this.StartSwapSoulLineEffects, delegate(GameObject e)
			{
				e.SetActive(true);
				e.GetComponent<ParticleSystem>().Play(true);
			});
			yield return new WaitForSeconds(0.5f);
			List<IrregularClickableImage> propertyPieces = new List<IrregularClickableImage>();
			propertyPieces.AddRange(this.PropertyPieces);
			propertyPieces.Shuffle(1);
			float flyDelay = 0.13f;
			float waitTm = 0.25f + flyDelay;
			int i = 0;
			int max = propertyPieces.Count;
			while (i < max)
			{
				ParticleSystem effect = propertyPieces[i].GetComponentInChildren<ParticleSystem>(true);
				effect.gameObject.SetActive(true);
				effect.Play(true);
				propertyPieces[i].enabled = false;
				TweenerCore<Vector3, Vector3, VectorOptions> tween = propertyPieces[i].rectTransform.DOLocalMove(Vector3.zero, 0.25f, false).SetDelay(flyDelay);
				bool flag = i == 0;
				if (flag)
				{
					tween.OnComplete(delegate
					{
						this.PieceFlyEndEffect.gameObject.SetActive(true);
						this.PieceFlyEndEffect.GetComponent<ParticleSystem>().Play(true);
					}).OnStart(delegate
					{
						AudioManager.Instance.PlaySound("SFX_Swapsoul_absorb", false, false);
					});
				}
				yield return new WaitForSeconds(0.03f);
				effect = null;
				tween = null;
				int num = i + 1;
				i = num;
			}
			yield return new WaitForSeconds(waitTm + 0.03f * (float)(propertyPieces.Count - 1));
			BuildingDomainMethod.Call.TrySwapSoulCeremony(this.Element.GameDataListenerId, this._selectedSoulCharIdList, this._selectedBodyCharId, this._selectedFeatureIds.ToList<short>());
			GEvent.OnEvent(UiEvents.SamsaraPlatformRecordDataChange, null);
			GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
			WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
			yield return waitFrame;
			yield return waitFrame;
			this.MonkAnim.AnimationState.SetAnimation(0, "SwapSoul_finish_action", true);
			this.MonkHandAnim.AnimationState.SetAnimation(0, "SwapSoul_finish_action", true);
			yield return waitFrame;
			yield return new WaitUntil(() => UIManager.Instance.IsFocusElement(this.Element));
			this.ResetData();
			this.RequestAllCharacters();
			this.RefreshBodyCharacter(-1);
			this.RefreshSoulCharacters(null);
			this.ResetPageEffects();
			this.CheckMonkAnimation();
			AudioManager.Instance.PlayAmbience("SFX_Swapsoul_amb_loop", 1.5f, 100);
			this.IsShowingSwapSoul = false;
			yield break;
		}

		// Token: 0x060077BD RID: 30653 RVA: 0x0037B9F4 File Offset: 0x00379BF4
		private void ResetPageEffects()
		{
			Array.ForEach<GameObject>(this.StaticSceneEffects, delegate(GameObject e)
			{
				e.SetActive(true);
			});
			Array.ForEach<GameObject>(this.StartSwapSoulLineEffects, delegate(GameObject e)
			{
				e.SetActive(false);
			});
			Array.ForEach<GameObject>(this.PropertyPieceDissolveEffects, delegate(GameObject e)
			{
				e.SetActive(false);
			});
			Array.ForEach<IrregularClickableImage>(this.PropertyPieces, delegate(IrregularClickableImage e)
			{
				e.gameObject.SetActive(false);
			});
			this.PieceFlyEndEffect.gameObject.SetActive(false);
			Array.ForEach<RectTransform>(this.SoulCharacterRectTransform, delegate(RectTransform e)
			{
				e.gameObject.SetActive(false);
			});
			this.BodyCharacterRectTransform.gameObject.SetActive(false);
			this.RedSceneRoot.SetActive(false);
			this.BlueSceneRoot.SetActive(true);
			this.DoorAnim.AnimationState.SetAnimation(0, "idle", true).Start += delegate(TrackEntry entry)
			{
				this.BehindDoorEffect.SetActive(false);
			};
		}

		// Token: 0x060077BE RID: 30654 RVA: 0x0037BB40 File Offset: 0x00379D40
		private IEnumerator UpdateSoulCharacterEffect(int index)
		{
			int curCharId = this._selectedSoulCharIdList[index];
			int lastCharId = this._lastSelectedSoulCharIdList[index];
			bool flag = curCharId < 0 || lastCharId < 0;
			if (flag)
			{
				this.SoulCharacterRectTransform[index].gameObject.SetActive(false);
			}
			bool flag2 = curCharId < 0;
			if (flag2)
			{
				this.CheckMonkAnimation();
				this._soulCharacterEffectCoroutine[index] = null;
				yield break;
			}
			yield return new WaitUntil(() => UIManager.Instance.IsFocusElement(this.Element));
			AudioManager.Instance.PlaySound("SFX_Swapsoul_beam_appear", false, false);
			yield return new WaitForSeconds(0.45f);
			bool flag3 = this._selectedSoulCharIdList[index] < 0;
			if (flag3)
			{
				this._soulCharacterEffectCoroutine[index] = null;
				yield break;
			}
			RectTransform soulRectTrans = this.SoulCharacterRectTransform[index];
			bool isNewlyAdded = curCharId >= 0 && lastCharId < 0;
			soulRectTrans.gameObject.SetActive(true);
			bool flag4 = isNewlyAdded;
			if (flag4)
			{
				soulRectTrans.DOKill(false);
				soulRectTrans.localScale = new Vector3(0f, 1f, 1f);
				soulRectTrans.DOScaleX(1f, 0.3f);
			}
			this.CheckMonkAnimation();
			Transform soulSubEffect = (soulRectTrans.childCount > 0) ? soulRectTrans.GetChild(0) : null;
			bool flag5 = soulSubEffect != null && isNewlyAdded;
			if (flag5)
			{
				soulSubEffect.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(0.25f);
			bool flag6 = soulSubEffect != null && isNewlyAdded;
			if (flag6)
			{
				soulSubEffect.gameObject.SetActive(true);
			}
			this._soulCharacterEffectCoroutine[index] = null;
			yield break;
		}

		// Token: 0x060077BF RID: 30655 RVA: 0x0037BB56 File Offset: 0x00379D56
		private IEnumerator UpdateBodyCharacterEffect()
		{
			bool flag = this._selectedBodyCharId < 0 || this._lastSelectedBodyCharId < 0;
			if (flag)
			{
				this.BodyCharacterRectTransform.gameObject.SetActive(false);
			}
			bool flag2 = this._selectedBodyCharId < 0;
			if (flag2)
			{
				this.CheckMonkAnimation();
				this._bodyCharacterEffectCoroutine = null;
				yield break;
			}
			yield return new WaitUntil(() => UIManager.Instance.IsFocusElement(this.Element));
			AudioManager.Instance.PlaySound("SFX_Swapsoul_beam_appear", false, false);
			yield return new WaitForSeconds(0.45f);
			bool flag3 = this._selectedBodyCharId < 0;
			if (flag3)
			{
				this._bodyCharacterEffectCoroutine = null;
				yield break;
			}
			bool isNewlyAdded = this._selectedBodyCharId >= 0 && this._lastSelectedBodyCharId < 0;
			this.BodyCharacterRectTransform.gameObject.SetActive(true);
			bool flag4 = isNewlyAdded;
			if (flag4)
			{
				this.BodyCharacterRectTransform.DOKill(false);
				this.BodyCharacterRectTransform.localScale = new Vector3(0f, 1f, 1f);
				this.BodyCharacterRectTransform.DOScaleX(1f, 0.3f);
			}
			this.CheckMonkAnimation();
			Transform soulSubEffect = (this.BodyCharacterRectTransform.childCount > 0) ? this.BodyCharacterRectTransform.GetChild(0) : null;
			bool flag5 = soulSubEffect != null && isNewlyAdded;
			if (flag5)
			{
				soulSubEffect.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(0.25f);
			bool flag6 = soulSubEffect != null && isNewlyAdded;
			if (flag6)
			{
				soulSubEffect.gameObject.SetActive(true);
			}
			this._bodyCharacterEffectCoroutine = null;
			yield break;
		}

		// Token: 0x060077C0 RID: 30656 RVA: 0x0037BB68 File Offset: 0x00379D68
		private void CheckMonkAnimation()
		{
			bool flag = null == this.MonkAnim || null == this.MonkHandAnim;
			if (flag)
			{
				throw new Exception("monk animation is null!");
			}
			string animationName = "SwapSoul_idle";
			bool flag2 = this.CheckSelectedSoulCharacters() || this._selectedBodyCharId != -1;
			if (flag2)
			{
				animationName = "SwapSoul_thinking";
			}
			this.MonkAnim.AnimationState.SetAnimation(0, animationName, true);
			this.MonkHandAnim.AnimationState.SetAnimation(0, animationName, true);
		}

		// Token: 0x060077C1 RID: 30657 RVA: 0x0037BBF4 File Offset: 0x00379DF4
		private void CollectTipItems(List<RectTransform> items)
		{
			bool flag = items != null && items.Count > 0;
			if (flag)
			{
				for (int i = items.Count - 1; i >= 0; i--)
				{
					bool flag2 = items[i] == null;
					if (flag2)
					{
						items.RemoveAt(i);
					}
					else
					{
						items[i].SetParent(this.MouseTipItemsContainer);
					}
				}
			}
		}

		// Token: 0x060077C2 RID: 30658 RVA: 0x0037BC64 File Offset: 0x00379E64
		private List<RectTransform> GetOrRefreshMainAttributeTip()
		{
			bool flag = this._mainAttributeItems == null || this._mainAttributeItems.Count != 6;
			if (flag)
			{
				if (this._mainAttributeItems == null)
				{
					this._mainAttributeItems = new List<RectTransform>();
				}
				for (int i = this._mainAttributeItems.Count; i < 6; i++)
				{
					GameObject obj = Object.Instantiate<GameObject>(this.MainAttributeTipItem.gameObject, this.MainAttributeTipItem.transform.parent);
					this._mainAttributeItems.Add(obj.transform as RectTransform);
				}
			}
			bool flag2 = this._resultPreview != null;
			if (flag2)
			{
				short attributeStartId = 0;
				for (int j = 0; j < 6; j++)
				{
					CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[j + (int)attributeStartId];
					bool flag3 = config == null;
					if (!flag3)
					{
						Refers refers = this._mainAttributeItems[j].GetComponent<Refers>();
						refers.CGet<TextMeshProUGUI>("Name").text = config.Name;
						refers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
						refers.CGet<TextMeshProUGUI>("Value").text = (ref this._resultPreview.BaseMainAttributes.Items.FixedElementField + (IntPtr)j * 2).ToString();
					}
				}
			}
			return this._mainAttributeItems;
		}

		// Token: 0x060077C3 RID: 30659 RVA: 0x0037BDD4 File Offset: 0x00379FD4
		private List<RectTransform> GetOrRefreshFeatureTip()
		{
			if (this._featureItems == null)
			{
				this._featureItems = new List<RectTransform>();
			}
			bool flag = this._resultPreview != null;
			if (flag)
			{
				List<short> displayFeatureIds = this.GetPreviewFeatures();
				int i = 0;
				int max = displayFeatureIds.Count;
				while (i < max)
				{
					bool flag2 = this._featureItems.CheckIndex(i);
					Refers refers;
					if (flag2)
					{
						refers = this._featureItems[i].GetComponent<Refers>();
					}
					else
					{
						GameObject obj = Object.Instantiate<GameObject>(this.FeatureTipItem.gameObject, this.FeatureTipItem.transform.parent);
						this._featureItems.Add(obj.transform as RectTransform);
						refers = obj.GetComponent<Refers>();
					}
					FeatureItem item = refers.UserObject as FeatureItem;
					bool flag3 = item == null;
					if (flag3)
					{
						item = new FeatureItem(refers, displayFeatureIds[i], -1);
						refers.UserObject = item;
					}
					else
					{
						item.Refresh(displayFeatureIds[i], -1);
					}
					i++;
				}
				for (int j = this._featureItems.Count - 1; j >= displayFeatureIds.Count; j--)
				{
					Object.Destroy(this._featureItems[j].gameObject);
					this._featureItems.RemoveAt(j);
				}
			}
			return this._featureItems;
		}

		// Token: 0x060077C4 RID: 30660 RVA: 0x0037BF44 File Offset: 0x0037A144
		private List<short> GetPreviewFeatures()
		{
			List<short> displayFeatureIds = new List<short>
			{
				this._resultPreview.BirthFeatureId
			};
			for (int i = 0; i < this._resultPreview.FeatureIds.Count; i++)
			{
				CharacterFeatureItem config = CharacterFeature.Instance.GetItem(this._resultPreview.FeatureIds[i]);
				bool hidden = config.Hidden;
				if (!hidden)
				{
					displayFeatureIds.Add(this._resultPreview.FeatureIds[i]);
				}
			}
			return displayFeatureIds;
		}

		// Token: 0x060077C5 RID: 30661 RVA: 0x0037BFD4 File Offset: 0x0037A1D4
		private List<RectTransform> GetOrRefreshBehaviorAndHappinessTip()
		{
			bool flag = this._behaviorAndHappinessItems == null;
			if (flag)
			{
				this._behaviorAndHappinessItems = new List<RectTransform>
				{
					this.BehaviorAndHappinessTipItem.transform as RectTransform
				};
				GameObject obj = Object.Instantiate<GameObject>(this.BehaviorAndHappinessTipItem.gameObject, this.BehaviorAndHappinessTipItem.transform.parent);
				this._behaviorAndHappinessItems.Add(obj.transform as RectTransform);
			}
			Refers behaviorRefers = this._behaviorAndHappinessItems[0].GetComponent<Refers>();
			sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(this._resultPreview.BaseMorality);
			BehaviorTypeItem config = Config.BehaviorType.Instance.GetItem((short)behaviorType);
			behaviorRefers.CGet<TextMeshProUGUI>("Name").text = config.Name;
			behaviorRefers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
			behaviorRefers.CGet<TextMeshProUGUI>("Value").text = CommonUtils.GetBehaviorString(behaviorType);
			Refers happinessRefers = this._behaviorAndHappinessItems[1].GetComponent<Refers>();
			sbyte happinessLevel = HappinessType.GetHappinessType(this._resultPreview.Happiness);
			happinessRefers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness);
			happinessRefers.CGet<CImage>("Icon").SetSprite(CommonUtils.GetHappinessIconName(happinessLevel), false, null);
			happinessRefers.CGet<TextMeshProUGUI>("Value").text = CommonUtils.GetHappinessString(happinessLevel);
			return this._behaviorAndHappinessItems;
		}

		// Token: 0x060077C6 RID: 30662 RVA: 0x0037C148 File Offset: 0x0037A348
		private unsafe List<RectTransform> GetOrRefreshReincarnationTip()
		{
			bool flag = this._reincarnationItems == null;
			if (flag)
			{
				this._reincarnationItems = new List<RectTransform>
				{
					this.ReincarnationTipItem.transform as RectTransform
				};
			}
			Refers tableRefers = this._reincarnationItems[0].GetComponent<Refers>();
			PreexistenceCharIds preexistenceCharIds = this._resultPreview.CharacterSamsaraData.PreexistenceCharIds;
			Dictionary<int, ValueTuple<int, DeadCharacter>> deadCharactersMap = new Dictionary<int, ValueTuple<int, DeadCharacter>>();
			int i = 0;
			int max = preexistenceCharIds.Count;
			while (i < max)
			{
				deadCharactersMap.Add(PreexistenceCharIds.Positions[i], new ValueTuple<int, DeadCharacter>(*(ref preexistenceCharIds.CharIds.FixedElementField + (IntPtr)i * 4), this._resultPreview.CharacterSamsaraData.DeadCharacters[i]));
				i++;
			}
			tableRefers.GetComponent<CImage>().enabled = (preexistenceCharIds.Count > 0);
			for (int j = 0; j < 9; j++)
			{
				Refers refers = tableRefers.CGet<Refers>(string.Format("Character_{0}", j));
				bool flag2 = preexistenceCharIds.Count <= 0;
				if (flag2)
				{
					refers.gameObject.SetActive(false);
				}
				else
				{
					refers.gameObject.SetActive(true);
					bool hasPreLife = deadCharactersMap.ContainsKey(j);
					bool flag3 = !hasPreLife;
					if (flag3)
					{
						refers.CGet<GameObject>("CharacterInfo").SetActive(false);
						refers.GetComponent<CImage>().SetSprite("sp_03_mh_renwukong", false, null);
					}
					else
					{
						ValueTuple<int, DeadCharacter> tuple;
						deadCharactersMap.TryGetValue(j, out tuple);
						refers.GetComponent<CImage>().SetSprite("sp_03_mh_touxiang_0", false, null);
						SingletonObject.getInstance<CharacterMonitorModel>().AddDeadCharacterCache(tuple.Item1, tuple.Item2);
						Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
						CharacterAvatar characterAvatar = avatar.UserObject as CharacterAvatar;
						bool flag4 = characterAvatar == null;
						if (flag4)
						{
							characterAvatar = new CharacterAvatar(avatar, false);
							characterAvatar.SetIsDead(true);
							avatar.UserObject = characterAvatar;
						}
						characterAvatar.CharacterId = tuple.Item1;
						CharacterName characterName = refers.UserObject as CharacterName;
						bool flag5 = characterName == null;
						if (flag5)
						{
							characterName = new CharacterName(refers.CGet<TextMeshProUGUI>("Name"), null, null);
							characterName.SetIsDead(true);
							refers.UserObject = characterName;
						}
						characterName.CharacterId = tuple.Item1;
						int index = preexistenceCharIds.GetIndexByPos(j);
						refers.CGet<GameObject>("CharacterInfo").SetActive(true);
						refers.CGet<TextMeshProUGUI>("Order").text = LocalStringManager.Get(CommonUtils.DigitLanguageKeys[index + 1]);
					}
				}
			}
			tableRefers.CGet<GameObject>("NoReincarnation").SetActive(preexistenceCharIds.Count <= 0);
			return this._reincarnationItems;
		}

		// Token: 0x060077C7 RID: 30663 RVA: 0x0037C41C File Offset: 0x0037A61C
		public void OnSelectCharBtnClicked(int index, bool isBody)
		{
			List<int> list = EasyPool.Get<List<int>>();
			bool flag = !isBody;
			if (flag)
			{
				bool flag2 = this._selectedSoulCharIdList[index] > 0;
				if (flag2)
				{
					list.Add(this._selectedSoulCharIdList[index]);
					this.ShowSelectCharacterUI(1, list, delegate(List<int> v)
					{
						this.CacheLastSelectedSoulCharIds();
						bool flag5 = v.Count == 0;
						if (flag5)
						{
							this._selectedSoulCharIdList[index] = -1;
							this.RefreshSoulCharacters(this._selectedSoulCharIdList);
						}
						else
						{
							int newCharId = v[0];
							for (int i = 0; i < this._selectedSoulCharIdList.Count; i++)
							{
								bool flag6 = i != index && this._selectedSoulCharIdList[i] == newCharId;
								if (flag6)
								{
									this._selectedSoulCharIdList[i] = -1;
								}
							}
							this._selectedSoulCharIdList[index] = newCharId;
							this.RefreshSoulCharacters(this._selectedSoulCharIdList);
						}
					}, this._soulCharIdList, isBody);
				}
				else
				{
					foreach (int charId in this._selectedSoulCharIdList)
					{
						bool flag3 = charId >= 0;
						if (flag3)
						{
							list.Add(charId);
						}
					}
					this.ShowSelectCharacterUI(this._soulLimit, list, delegate(List<int> v)
					{
						this.CacheLastSelectedSoulCharIds();
						this.RefreshSoulCharacters(v);
					}, this._soulCharIdList, isBody);
				}
			}
			else
			{
				this._lastSelectedBodyCharId = this._selectedBodyCharId;
				bool flag4 = this._selectedBodyCharId > 0;
				if (flag4)
				{
					list.Add(this._selectedBodyCharId);
				}
				this.ShowSelectCharacterUI(1, list, delegate(List<int> v)
				{
					this._lastSelectedBodyCharId = this._selectedBodyCharId;
					this.RefreshBodyCharacter((v.Count > 0) ? v[0] : -1);
				}, this._bodyCharIdList, isBody);
			}
		}

		// Token: 0x060077C8 RID: 30664 RVA: 0x0037C56C File Offset: 0x0037A76C
		public void OnInfoBtnClicked(int index, bool isBody)
		{
			int charId = isBody ? this._selectedBodyCharId : this._selectedSoulCharIdList[index];
			bool flag = charId < 0;
			if (!flag)
			{
				this.OpenCharacterMenu(charId);
			}
		}

		// Token: 0x060077C9 RID: 30665 RVA: 0x0037C5A4 File Offset: 0x0037A7A4
		public void OnDeleteBtnClicked(int index, bool isBody)
		{
			bool flag = !isBody;
			if (flag)
			{
				this.CacheLastSelectedSoulCharIds();
				this.characterDisplayDataDict.Remove(this._selectedSoulCharIdList[index]);
				this._selectedSoulCharIdList[index] = -1;
				this.RefreshSoulCharacters(this._selectedSoulCharIdList);
			}
			else
			{
				this.characterDisplayDataDict.Remove(this._selectedBodyCharId);
				this._lastSelectedBodyCharId = this._selectedBodyCharId;
				this._selectedBodyCharId = -1;
				this.RefreshBodyCharacter(-1);
			}
		}

		// Token: 0x060077CA RID: 30666 RVA: 0x0037C628 File Offset: 0x0037A828
		private void ShowSelectCharacterUI(int targetCount, List<int> selectedList, SelectCharacterCallback callback, List<int> charIds, bool isBody)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this, charIds, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new BasicSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = ESelectCharacterInteractionMode.Slot;
				config.SelectionMode = ((targetCount != 1) ? ESelectCharacterSelectionMode.Multiple : ESelectCharacterSelectionMode.Single);
				config.TargetCount = targetCount;
				config.InitialSelectedCharacterIds = selectedList;
				CommonSelectCharacterConfig commonSelectCharacterConfig = config;
				IEnumerable<CharacterDisplayDataForGeneralScrollList> source;
				if (!isBody)
				{
					source = from x in displayData
					where !GameData.Domains.Character.SharedMethods.CanBePossessionSoul(x.CharacterTemplateId)
					select x;
				}
				else
				{
					source = from x in displayData
					where !GameData.Domains.Character.SharedMethods.CanBePossessionBody(x.CharacterTemplateId)
					select x;
				}
				commonSelectCharacterConfig.BannedCharacterIds = (from x in source
				select x.CharacterId into x
				where x != -1
				select x).ToHashSet<int>();
				config.RefreshDeadAsAlive = !isBody;
				config.CanShowCharacterMenu = isBody;
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
				EasyPool.Free<List<int>>(selectedList);
			});
		}

		// Token: 0x060077CB RID: 30667 RVA: 0x0037C670 File Offset: 0x0037A870
		private void SelectFeature()
		{
			List<short> allFeatureIds = this.CalcFeatureIds();
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.SetObject("SelectedFeatureIds", this._selectedFeatureIds);
			box.SetObject("AllFeatureIds", allFeatureIds);
			box.SetObject("OnConfirm", new Action(this.OnFeatureSelectComplete));
			box.SetObject("FeatureIdDict", this._featureIdDict);
			box.SetObject("FeatureIdGroupList", this._featureIdGroupList);
			UIElement.SelectFeature.SetOnInitArgs(box);
			UIManager.Instance.MaskUI(UIElement.SelectFeature);
		}

		// Token: 0x060077CC RID: 30668 RVA: 0x0037C702 File Offset: 0x0037A902
		private void OnFeatureSelectComplete()
		{
			this.GetPossessionPreview();
			this.RefreshSelectFeatureButton();
		}

		// Token: 0x060077CD RID: 30669 RVA: 0x0037C714 File Offset: 0x0037A914
		private List<short> CalcFeatureIds()
		{
			List<short> allFeatureIds = new List<short>();
			CharacterDisplayData bodyDisplayData;
			bool flag = this._selectedBodyCharId >= 0 && this.characterDisplayDataDict.TryGetValue(this._selectedBodyCharId, out bodyDisplayData);
			if (flag)
			{
				List<short> featureIds = bodyDisplayData.FeatureIds;
				for (int i = 0; i < featureIds.Count; i++)
				{
					bool bodyTransform = CharacterFeature.Instance[featureIds[i]].BodyTransform;
					if (bodyTransform)
					{
						allFeatureIds.Add(featureIds[i]);
					}
				}
			}
			for (int j = 0; j < this._soulLimit; j++)
			{
				int charId = this._selectedSoulCharIdList[j];
				CharacterDisplayData soulDisplayData;
				bool flag2 = charId < 0 || !this.characterDisplayDataDict.TryGetValue(charId, out soulDisplayData);
				if (!flag2)
				{
					List<short> featureIds2 = soulDisplayData.FeatureIds;
					for (int k = 0; k < featureIds2.Count; k++)
					{
						short mutexGroupId = CharacterFeature.Instance[featureIds2[k]].MutexGroupId;
						bool flag3 = mutexGroupId != 590 && CharacterFeature.Instance[featureIds2[k]].SoulTransform;
						if (flag3)
						{
							allFeatureIds.Add(featureIds2[k]);
						}
					}
				}
			}
			this._featureIdDict.Clear();
			HashSet<short> hashSet = EasyPool.Get<HashSet<short>>();
			foreach (short id5 in allFeatureIds)
			{
				bool flag4 = hashSet.Contains(id5);
				if (!flag4)
				{
					CharacterFeatureItem featureConfig = CharacterFeature.Instance[id5];
					hashSet.Add(id5);
					bool flag5 = !this._featureIdDict.ContainsKey((int)featureConfig.MutexGroupId);
					if (flag5)
					{
						this._featureIdDict.Add((int)featureConfig.MutexGroupId, new List<short>());
					}
					this._featureIdDict[(int)featureConfig.MutexGroupId].Add(id5);
				}
			}
			List<short> toRemoveFromAll = EasyPool.Get<List<short>>();
			foreach (KeyValuePair<int, List<short>> keyValuePair in this._featureIdDict)
			{
				int num;
				List<short> list;
				keyValuePair.Deconstruct(out num, out list);
				List<short> featureList = list;
				bool flag6 = featureList.Count <= 1;
				if (!flag6)
				{
					bool hasVisible = false;
					bool hasInvisible = false;
					foreach (short id2 in featureList)
					{
						bool hidden = CharacterFeature.Instance[id2].Hidden;
						if (hidden)
						{
							hasInvisible = true;
						}
						else
						{
							hasVisible = true;
						}
					}
					bool flag7 = hasVisible && hasInvisible;
					if (flag7)
					{
						for (int l = featureList.Count - 1; l >= 0; l--)
						{
							short id3 = featureList[l];
							bool hidden2 = CharacterFeature.Instance[id3].Hidden;
							if (hidden2)
							{
								featureList.RemoveAt(l);
								toRemoveFromAll.Add(id3);
							}
						}
					}
					else
					{
						bool flag8 = !hasVisible && hasInvisible && featureList.Count > 1;
						if (flag8)
						{
							for (int m = featureList.Count - 1; m >= 1; m--)
							{
								short id4 = featureList[m];
								featureList.RemoveAt(m);
								toRemoveFromAll.Add(id4);
							}
						}
					}
				}
			}
			bool flag9 = toRemoveFromAll.Count > 0;
			if (flag9)
			{
				allFeatureIds.RemoveAll((short id) => toRemoveFromAll.Contains(id));
			}
			EasyPool.Free<List<short>>(toRemoveFromAll);
			this._featureIdGroupList.Clear();
			foreach (KeyValuePair<int, List<short>> keyValuePair in this._featureIdDict)
			{
				int num;
				List<short> list;
				keyValuePair.Deconstruct(out num, out list);
				int n = num;
				List<short> v = list;
				bool flag10 = v.Count <= 1;
				if (flag10)
				{
					this._selectedFeatureIds.Add(v[0]);
				}
				else
				{
					this._featureIdGroupList.Add(new ValueTuple<int, List<short>>(n, v));
				}
			}
			EasyPool.Free<HashSet<short>>(hashSet);
			return allFeatureIds;
		}

		// Token: 0x04005A58 RID: 23128
		public GameObject[] StaticSceneEffects;

		// Token: 0x04005A59 RID: 23129
		public IrregularClickableImage[] PropertyPieces;

		// Token: 0x04005A5A RID: 23130
		public GameObject[] PropertyPieceDissolveEffects;

		// Token: 0x04005A5B RID: 23131
		public GameObject PieceFlyEndEffect;

		// Token: 0x04005A5C RID: 23132
		private Vector2[] PiecesStartPositions;

		// Token: 0x04005A5D RID: 23133
		public GameObject[] StartSwapSoulLineEffects;

		// Token: 0x04005A5E RID: 23134
		public GameObject BlueSceneRoot;

		// Token: 0x04005A5F RID: 23135
		public GameObject RedSceneRoot;

		// Token: 0x04005A60 RID: 23136
		public RectTransform[] SoulCharacterRectTransform;

		// Token: 0x04005A61 RID: 23137
		public RectTransform BodyCharacterRectTransform;

		// Token: 0x04005A62 RID: 23138
		public SkeletonGraphic MonkAnim;

		// Token: 0x04005A63 RID: 23139
		public SkeletonGraphic MonkHandAnim;

		// Token: 0x04005A64 RID: 23140
		public SkeletonGraphic DoorAnim;

		// Token: 0x04005A65 RID: 23141
		public GameObject BehindDoorEffect;

		// Token: 0x04005A66 RID: 23142
		[SerializeField]
		private List<SwapSoulCharacterItem> soulCharacterItems;

		// Token: 0x04005A67 RID: 23143
		[SerializeField]
		private SwapSoulCharacterItem bodyCharacterItem;

		// Token: 0x04005A68 RID: 23144
		[SerializeField]
		private Game.Components.Avatar.Avatar resultCharacterAvatar;

		// Token: 0x04005A69 RID: 23145
		[SerializeField]
		private TextMeshProUGUI resultCharacterName;

		// Token: 0x04005A6A RID: 23146
		[SerializeField]
		private TooltipInvoker MainAttributePieceTip;

		// Token: 0x04005A6B RID: 23147
		[SerializeField]
		private TooltipInvoker BehaviorAndHappinessPieceTip;

		// Token: 0x04005A6C RID: 23148
		[SerializeField]
		private TooltipInvoker FeaturePieceTip;

		// Token: 0x04005A6D RID: 23149
		[SerializeField]
		private TooltipInvoker ReincarnationPieceTip;

		// Token: 0x04005A6E RID: 23150
		[SerializeField]
		private TooltipInvoker LifeAndCombatSkillsValuePieceTip;

		// Token: 0x04005A6F RID: 23151
		[SerializeField]
		private CImage fameIcon;

		// Token: 0x04005A70 RID: 23152
		[SerializeField]
		private Sprite[] fameIconSprites;

		// Token: 0x04005A71 RID: 23153
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04005A72 RID: 23154
		[SerializeField]
		private CButton editAvatarBtn;

		// Token: 0x04005A73 RID: 23155
		[SerializeField]
		private CButton selectFeatureBtn;

		// Token: 0x04005A74 RID: 23156
		[SerializeField]
		private CImage selectFeatureIcon;

		// Token: 0x04005A75 RID: 23157
		[SerializeField]
		private CImage resultEmpty;

		// Token: 0x04005A76 RID: 23158
		[SerializeField]
		private TextMeshProUGUI resultCharName;

		// Token: 0x04005A77 RID: 23159
		[SerializeField]
		private RectTransform soulLackHolder;

		// Token: 0x04005A78 RID: 23160
		[SerializeField]
		private RectTransform bodyLackHolder;

		// Token: 0x04005A79 RID: 23161
		[SerializeField]
		private RectTransform mask;

		// Token: 0x04005A7A RID: 23162
		private readonly Dictionary<int, CharacterDisplayData> characterDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x04005A7B RID: 23163
		private PossessionPreview _resultPreview;

		// Token: 0x04005A7C RID: 23164
		private List<int> _soulCharIdList;

		// Token: 0x04005A7D RID: 23165
		private List<int> _bodyCharIdList;

		// Token: 0x04005A7E RID: 23166
		private int _soulLimit = 3;

		// Token: 0x04005A7F RID: 23167
		private readonly List<int> _selectedSoulCharIdList = new List<int>
		{
			-1,
			-1,
			-1
		};

		// Token: 0x04005A80 RID: 23168
		private int[] _lastSelectedSoulCharIdList = new int[]
		{
			-1,
			-1,
			-1
		};

		// Token: 0x04005A81 RID: 23169
		private int _selectedBodyCharId = -1;

		// Token: 0x04005A82 RID: 23170
		private int _lastSelectedBodyCharId = -1;

		// Token: 0x04005A83 RID: 23171
		private int _resultCharacterCharId = -1;

		// Token: 0x04005A84 RID: 23172
		private readonly HashSet<short> _selectedFeatureIds = new HashSet<short>();

		// Token: 0x04005A85 RID: 23173
		private readonly Dictionary<int, List<short>> _featureIdDict = new Dictionary<int, List<short>>();

		// Token: 0x04005A86 RID: 23174
		private readonly List<ValueTuple<int, List<short>>> _featureIdGroupList = new List<ValueTuple<int, List<short>>>();

		// Token: 0x04005A87 RID: 23175
		private bool _isShowingSwapSoul;

		// Token: 0x04005A88 RID: 23176
		private Coroutine[] _soulCharacterEffectCoroutine = new Coroutine[3];

		// Token: 0x04005A89 RID: 23177
		private Coroutine _bodyCharacterEffectCoroutine;

		// Token: 0x04005A8A RID: 23178
		private List<RectTransform> _mainAttributeItems;

		// Token: 0x04005A8B RID: 23179
		private List<RectTransform> _featureItems;

		// Token: 0x04005A8C RID: 23180
		private List<RectTransform> _behaviorAndHappinessItems;

		// Token: 0x04005A8D RID: 23181
		private List<RectTransform> _reincarnationItems;

		// Token: 0x04005A8E RID: 23182
		public Refers MainAttributeTipItem;

		// Token: 0x04005A8F RID: 23183
		public Refers FeatureTipItem;

		// Token: 0x04005A90 RID: 23184
		public Refers BehaviorAndHappinessTipItem;

		// Token: 0x04005A91 RID: 23185
		public Refers ReincarnationTipItem;

		// Token: 0x04005A92 RID: 23186
		public Transform MouseTipItemsContainer;
	}
}
