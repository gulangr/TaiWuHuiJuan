using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using Config.ConfigCells.Character;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.SectInteract.Shaolin
{
	// Token: 0x020009E9 RID: 2537
	public class ViewShaolinDemonSlayer : UIBase
	{
		// Token: 0x17000DAA RID: 3498
		// (get) Token: 0x06007C8A RID: 31882 RVA: 0x0039E204 File Offset: 0x0039C404
		private int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x06007C8B RID: 31883 RVA: 0x0039E210 File Offset: 0x0039C410
		public override void OnInit(ArgumentBox argsBox)
		{
			this.leftBoss.btnOpenCharacterMenu.gameObject.SetActive(false);
			this.rightBoss.btnOpenCharacterMenu.gameObject.SetActive(false);
			this.leftBoss.tipsArea.gameObject.SetActive(false);
			this.rightBoss.tipsArea.gameObject.SetActive(false);
		}

		// Token: 0x06007C8C RID: 31884 RVA: 0x0039E27C File Offset: 0x0039C47C
		private void Awake()
		{
			this.btnOpenTaiwu.onClick.ResetListener(new Action(this.OnClickOpenTaiwu));
			this.btnHeal.onClick.ResetListener(new Action(this.OnClickHeal));
			this.btnBreak.onClick.ResetListener(new Action(this.OnClickBreak));
			this.btnEquipment.onClick.ResetListener(new Action(this.OnClickEquipment));
			this.btnSkill.onClick.ResetListener(new Action(this.OnClickSkill));
			this.btnRefresh.onClick.ResetListener(new Action(this.OnClickRefresh));
			this.btnGiveUp.onClick.ResetListener(new Action(this.OnClickGiveUp));
			this.leftBoss.Init(0, new Action<int>(this.OnClickStart));
			this.rightBoss.Init(1, new Action<int>(this.OnClickStart));
		}

		// Token: 0x06007C8D RID: 31885 RVA: 0x0039E388 File Offset: 0x0039C588
		private void OnEnable()
		{
			GEvent.Add(UiEvents.CharacterMenuHide, new GEvent.Callback(this.OnCharacterMenuHide));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.TaiwuId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._data);
				this.RefreshTaiwu();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06007C8E RID: 31886 RVA: 0x0039E3E0 File Offset: 0x0039C5E0
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.CharacterMenuHide, new GEvent.Callback(this.OnCharacterMenuHide));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
		}

		// Token: 0x06007C8F RID: 31887 RVA: 0x0039E414 File Offset: 0x0039C614
		private void OnTopUIChanged(ArgumentBox argBox)
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.SectShaolinDemonSlayer);
			if (flag)
			{
				foreach (ParticleSystem particle in this._particleList)
				{
					particle.gameObject.SetActive(false);
				}
			}
			else
			{
				foreach (ParticleSystem particle2 in this._particleList)
				{
					particle2.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06007C90 RID: 31888 RVA: 0x0039E4D4 File Offset: 0x0039C6D4
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonCloseView";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007C91 RID: 31889 RVA: 0x0039E4FF File Offset: 0x0039C6FF
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 171, ulong.MaxValue, null));
		}

		// Token: 0x06007C92 RID: 31890 RVA: 0x0039E520 File Offset: 0x0039C720
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						ushort domainId = notification.DomainId;
						ushort num = domainId;
						if (num == 20)
						{
							bool flag = notification.MethodId == 10;
							if (flag)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._bossIdList);
								this.Refresh();
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag2 = uid.DomainId == 19 && uid.DataId == 171;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._slayerData);
						StoryDomainMethod.Call.ShaolinGenerateTemporaryDemon(this.Element.GameDataListenerId);
					}
				}
			}
		}

		// Token: 0x06007C93 RID: 31891 RVA: 0x0039E634 File Offset: 0x0039C834
		public override void QuickHide()
		{
			base.QuickHide();
			StoryDomainMethod.Call.ShaolinClearTemporaryDemon(this._bossIdList);
			this._leftIndex = -1;
			this._rightIndex = -1;
		}

		// Token: 0x06007C94 RID: 31892 RVA: 0x0039E658 File Offset: 0x0039C858
		private void Refresh()
		{
			bool flag = !this._slayerData.Trialing;
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				bool flag2 = this._slayerData.TrialingLevel == null;
				if (!flag2)
				{
					this.RefreshTower();
					this.RefreshReward();
					this.RefreshBoss();
					this.RefreshButtons();
				}
			}
		}

		// Token: 0x06007C95 RID: 31893 RVA: 0x0039E6B4 File Offset: 0x0039C8B4
		private void RefreshTaiwu()
		{
			string taiwuName = NameCenter.GetMonasticTitleOrDisplayName(this._data, true);
			this.taiwuNameLabel.text = taiwuName;
			this.taiwuAvatar.Refresh(this._data, true);
			TooltipInvoker injuryTips = this.btnOpenTaiwu.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = injuryTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			injuryTips.RuntimeParam.Set("characterId", this.TaiwuId);
		}

		// Token: 0x06007C96 RID: 31894 RVA: 0x0039E72C File Offset: 0x0039C92C
		private void RefreshTower()
		{
			DemonSlayerTrialLevelItem config = this._slayerData.TrialingLevel;
			int levelIndex = config.TemplateId;
			RectTransform currNode = this.nodes.GetChild(levelIndex).GetComponent<RectTransform>();
			for (int i = 0; i < levelIndex; i++)
			{
				this.nodes.GetChild(i).GetComponent<CImage>().sprite = this.previousNode;
			}
			currNode.GetComponent<CImage>().sprite = this.currentNode;
			for (int j = levelIndex + 1; j < this.nodes.childCount; j++)
			{
				this.nodes.GetChild(j).GetComponent<CImage>().sprite = this.nextNode;
			}
			this.levelTitleLabel.text = config.LevelName;
			int num = levelIndex;
			int num2 = num;
			if (num2 != 0)
			{
				if (num2 != 8)
				{
					this.towerFill.fillAmount = (currNode.rect.height / 2f + currNode.anchoredPosition.y) / this.tower.rect.height;
					bool flag = levelIndex % 2 == 0;
					if (flag)
					{
						this.SetLeftArrow(currNode);
					}
					else
					{
						this.SetRightArrow(currNode);
					}
				}
				else
				{
					this.towerFill.fillAmount = 1f;
					this.SetLeftArrow(currNode);
				}
			}
			else
			{
				this.towerFill.fillAmount = 0f;
				this.SetRightArrow(currNode);
			}
		}

		// Token: 0x06007C97 RID: 31895 RVA: 0x0039E8A4 File Offset: 0x0039CAA4
		private void RefreshReward()
		{
			PresetInventoryItem currRewardConfig = this._slayerData.TrialingLevel.RewardItems[0];
			ItemDisplayData data = new ItemDisplayData(currRewardConfig.Type, currRewardConfig.TemplateId);
			this.expAmountLabel.text = string.Format("x{0}", this._slayerData.TrialingLevel.RewardExp);
			this.itemIcon.SetSprite(ItemTemplateHelper.GetIcon(currRewardConfig.Type, currRewardConfig.TemplateId), false, null);
			this.itemGrade.SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(currRewardConfig.Type, currRewardConfig.TemplateId)]);
			this.itemAmountLabel.text = string.Format("x{0}", currRewardConfig.Amount);
			this.itemAmount.SetActive(currRewardConfig.Amount > 1);
			this.itemTips.Type = TooltipManager.ItemTypeToTipType[currRewardConfig.Type];
			this.itemTips.RuntimeParam = new ArgumentBox().SetObject("ItemData", data);
			this.itemTips.RuntimeParam.Set("TemplateDataOnly", true);
			this.itemTips.Refresh(true, -1);
		}

		// Token: 0x06007C98 RID: 31896 RVA: 0x0039E9E4 File Offset: 0x0039CBE4
		private void RefreshBoss()
		{
			bool flag = !this._playAnim;
			if (flag)
			{
				this._playAnim = true;
			}
			else
			{
				DemonSlayerTrialItem leftConfig = this._slayerData.GetTrialingDemon(0);
				CharacterItem leftCharConfig = Character.Instance[leftConfig.CharacterId];
				DemonSlayerTrialItem rightConfig = this._slayerData.GetTrialingDemon(1);
				CharacterItem rightCharConfig = Character.Instance[rightConfig.CharacterId];
				bool isLeftDefeated = this._slayerData.IsDemonDefeated(leftConfig.TemplateId);
				bool isRightDefeated = this._slayerData.IsDemonDefeated(rightConfig.TemplateId);
				Sequence sequence = this._sequence;
				if (sequence != null)
				{
					sequence.Kill(false);
				}
				this._sequence = DOTween.Sequence();
				foreach (ParticleSystem particle in this._particleList)
				{
					particle.gameObject.SetActive(false);
				}
				this._particleList.Clear();
				this._sequence.AppendCallback(delegate
				{
					this.btnRefresh.interactable = false;
				});
				this._sequence.AppendCallback(delegate
				{
					this.leftBoss.btnStart.interactable = false;
				});
				this._sequence.AppendCallback(delegate
				{
					this.rightBoss.btnStart.interactable = false;
				});
				bool flag2 = this._leftIndex >= 0;
				if (flag2)
				{
					this._sequence.AppendCallback(delegate
					{
						this.leftBossAvatar.gameObject.SetActive(false);
					});
					this._sequence.AppendCallback(delegate
					{
						this.rightBossAvatar.gameObject.SetActive(false);
					});
					Transform leftOutParticle = this.outAnim.GetChild(0).GetChild(this._leftIndex);
					this._sequence.AppendCallback(delegate
					{
						leftOutParticle.gameObject.SetActive(true);
					});
					this._sequence.AppendCallback(delegate
					{
						leftOutParticle.GetComponent<UIParticle>().Play();
					});
					Transform rightOutParticle = this.outAnim.GetChild(1).GetChild(this._rightIndex);
					this._sequence.AppendCallback(delegate
					{
						rightOutParticle.gameObject.SetActive(true);
					});
					this._sequence.AppendCallback(delegate
					{
						rightOutParticle.GetComponent<UIParticle>().Play();
					});
					this._sequence.AppendInterval(1f);
					this._sequence.AppendCallback(delegate
					{
						leftOutParticle.gameObject.SetActive(false);
					});
					this._sequence.AppendCallback(delegate
					{
						rightOutParticle.gameObject.SetActive(false);
					});
					this._sequence.AppendCallback(delegate
					{
						this._leftIndex = -1;
					});
					this._sequence.AppendCallback(delegate
					{
						this._rightIndex = -1;
					});
				}
				this._sequence.AppendCallback(delegate
				{
					this.avatarCanvasGroup.alpha = 0f;
				});
				this._sequence.AppendCallback(delegate
				{
					this.particleCanvasGroup.alpha = 0f;
				});
				this._sequence.AppendCallback(delegate
				{
					this._leftIndex = (int)leftCharConfig.Gender;
				});
				this._sequence.AppendCallback(delegate
				{
					this._rightIndex = (int)rightCharConfig.Gender;
				});
				Transform leftInParticle = this.inAnim.GetChild(0).GetChild((int)leftCharConfig.Gender);
				this._sequence.AppendCallback(delegate
				{
					leftInParticle.gameObject.SetActive(true);
				});
				this._sequence.AppendCallback(delegate
				{
					leftInParticle.GetComponent<UIParticle>().Play();
				});
				Transform rightInParticle = this.inAnim.GetChild(1).GetChild((int)rightCharConfig.Gender);
				this._sequence.AppendCallback(delegate
				{
					rightInParticle.gameObject.SetActive(true);
				});
				this._sequence.AppendCallback(delegate
				{
					rightInParticle.GetComponent<UIParticle>().Play();
				});
				this._sequence.AppendInterval(0.8f);
				this._sequence.AppendCallback(delegate
				{
					this.PlayParticle(true, isLeftDefeated, (int)leftCharConfig.Gender);
				});
				this._sequence.AppendCallback(delegate
				{
					this.PlayParticle(false, isRightDefeated, (int)rightCharConfig.Gender);
				});
				Action<Texture2D> <>9__29;
				this._sequence.AppendCallback(delegate
				{
					string npcFaceResPath = CharacterAvatar.GetNpcFaceResPath("BigFace", leftCharConfig.FixedAvatarName);
					Action<Texture2D> onLoad;
					if ((onLoad = <>9__29) == null)
					{
						onLoad = (<>9__29 = delegate(Texture2D tex)
						{
							this.leftBossAvatar.Refresh(tex);
							this.leftBossAvatar.gameObject.SetActive(true);
							this.avatarCanvasGroup.DOFade(1f, 0.2f);
							this.particleCanvasGroup.DOFade(1f, 1.5f);
						});
					}
					ResLoader.LoadModOrGameResource<Texture2D>(npcFaceResPath, onLoad, null);
				});
				Action<Texture2D> <>9__30;
				this._sequence.AppendCallback(delegate
				{
					string npcFaceResPath = CharacterAvatar.GetNpcFaceResPath("BigFace", rightCharConfig.FixedAvatarName);
					Action<Texture2D> onLoad;
					if ((onLoad = <>9__30) == null)
					{
						onLoad = (<>9__30 = delegate(Texture2D tex)
						{
							this.rightBossAvatar.Refresh(tex);
							this.rightBossAvatar.gameObject.SetActive(true);
						});
					}
					ResLoader.LoadModOrGameResource<Texture2D>(npcFaceResPath, onLoad, null);
				});
				this._sequence.AppendInterval(0.2f);
				this._sequence.AppendCallback(delegate
				{
					leftInParticle.gameObject.SetActive(false);
				});
				this._sequence.AppendCallback(delegate
				{
					rightInParticle.gameObject.SetActive(false);
				});
				this._sequence.AppendCallback(delegate
				{
					this.leftBoss.Set(this._slayerData, this._bossIdList);
				});
				this._sequence.AppendCallback(delegate
				{
					this.rightBoss.Set(this._slayerData, this._bossIdList);
				});
				this._sequence.AppendCallback(new TweenCallback(this.RefreshBossRestrictions));
				this._sequence.AppendCallback(new TweenCallback(this.RefreshButtons));
				this._sequence.Play<Sequence>();
			}
		}

		// Token: 0x06007C99 RID: 31897 RVA: 0x0039EED4 File Offset: 0x0039D0D4
		private void RefreshBossRestrictions()
		{
			StoryDomainMethod.AsyncCall.ShaolinQueryRestrictsAreSatisfied(this, 0, delegate(int offset, RawDataPool dataPool)
			{
				byte restrictResult = 0;
				Serializer.Deserialize(dataPool, offset, ref restrictResult);
				this.leftBoss.SetRestrictions(restrictResult);
			});
			StoryDomainMethod.AsyncCall.ShaolinQueryRestrictsAreSatisfied(this, 1, delegate(int offset, RawDataPool dataPool)
			{
				byte restrictResult = 0;
				Serializer.Deserialize(dataPool, offset, ref restrictResult);
				this.rightBoss.SetRestrictions(restrictResult);
			});
		}

		// Token: 0x06007C9A RID: 31898 RVA: 0x0039EF00 File Offset: 0x0039D100
		private void RefreshButtons()
		{
			int leftCount = this._slayerData.GetRegenerateRestrictCount();
			this.btnRefresh.interactable = (leftCount > 0);
			this.btnRefresh.GetComponent<TooltipInvoker>().enabled = !this.btnRefresh.interactable;
			this.refreshCountLabel.text = string.Concat(new string[]
			{
				LanguageKey.LK_SectShaolinDemonSlayer_RefreshRestriction.Tr(),
				"(",
				leftCount.ToString(),
				"/",
				3.ToString(),
				")"
			});
		}

		// Token: 0x06007C9B RID: 31899 RVA: 0x0039EF9C File Offset: 0x0039D19C
		private void OnClickOpenTaiwu()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this.TaiwuId);
			argBox.Set("CanOperate", true);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06007C9C RID: 31900 RVA: 0x0039EFEC File Offset: 0x0039D1EC
		private void OnClickHeal()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			HashSet<int> patientList = EasyPool.Get<HashSet<int>>();
			HashSet<int> doctorList = EasyPool.Get<HashSet<int>>();
			BasicGameData data = SingletonObject.getInstance<BasicGameData>();
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			patientList.Add(this.TaiwuId);
			patientList.UnionWith(monitor.GetTaiwuTeamCharIds());
			doctorList.UnionWith(patientList);
			patientList.UnionWith(monitor.GetTaiwuSpecialGroup());
			doctorList.UnionWith(monitor.GetTaiwuGearMateGroup());
			argBox.SetObject("DoctorList", new List<int>(doctorList));
			argBox.SetObject("PatientList", new List<int>(patientList));
			argBox.Set("NeedPay", false);
			CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, data.TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				KidnappedCharacterList kidnappedCharacterList = null;
				Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
				bool flag = kidnappedCharacterList != null;
				if (flag)
				{
					for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
					{
						patientList.Add(kidnappedCharacterList.Get(i).CharId);
					}
				}
				UIElement.Heal.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.Heal, true);
			});
		}

		// Token: 0x06007C9D RID: 31901 RVA: 0x0039F0D8 File Offset: 0x0039D2D8
		private void OnClickBreak()
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06007C9E RID: 31902 RVA: 0x0039F13C File Offset: 0x0039D33C
		private void OnClickEquipment()
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.EquipmentBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06007C9F RID: 31903 RVA: 0x0039F1A0 File Offset: 0x0039D3A0
		private void OnClickSkill()
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.EquipCombatSkillBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06007CA0 RID: 31904 RVA: 0x0039F204 File Offset: 0x0039D404
		private void OnClickRefresh()
		{
			StoryDomainMethod.Call.ShaolinRegenerateRestricts(this.Element.GameDataListenerId);
		}

		// Token: 0x06007CA1 RID: 31905 RVA: 0x0039F218 File Offset: 0x0039D418
		private void OnClickGiveUp()
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_SectShaolinDemonSlayer_FinishChallenge),
				Content = LocalStringManager.Get(LanguageKey.LK_SectShaolinDemonSlayer_FinishChallengeConfirmContext),
				Yes = delegate()
				{
					this.QuickHide();
					StoryDomainMethod.Call.ShaolinInterruptDemonSlayerTrial(this.Element.GameDataListenerId);
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007CA2 RID: 31906 RVA: 0x0039F289 File Offset: 0x0039D489
		private void OnClickStart(int index)
		{
			StoryDomainMethod.Call.ShaolinStartDemonSlayerTrial(index);
			this._playAnim = false;
		}

		// Token: 0x06007CA3 RID: 31907 RVA: 0x0039F29C File Offset: 0x0039D49C
		private void OnCharacterMenuHide(ArgumentBox _)
		{
			this.RefreshBossRestrictions();
			foreach (ParticleSystem particle in this._particleList)
			{
				particle.gameObject.SetActive(true);
				particle.Play();
			}
		}

		// Token: 0x06007CA4 RID: 31908 RVA: 0x0039F308 File Offset: 0x0039D508
		private void PlayParticle(bool isLeft, bool isBeaten, int genderIndex)
		{
			int index = isLeft ? 0 : 1;
			ParticleSystem particle = isBeaten ? this.particles.GetChild(index).GetChild(0).GetComponent<ParticleSystem>() : this.particles.GetChild(index).GetChild(1).GetChild(genderIndex).GetComponent<ParticleSystem>();
			this._particleList.Add(particle);
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.SectShaolinDemonSlayer);
			if (!flag)
			{
				particle.gameObject.SetActive(true);
				particle.Play();
				if (!isBeaten)
				{
					Transform particleTransform = particle.transform;
					for (int i = 0; i < particleTransform.childCount; i++)
					{
						ParticleSystemRenderer particleSystemRenderer;
						bool flag2 = particleTransform.GetChild(i).TryGetComponent<ParticleSystemRenderer>(out particleSystemRenderer);
						if (flag2)
						{
							this.SetParticleAlpha(particleSystemRenderer.material);
						}
					}
				}
			}
		}

		// Token: 0x06007CA5 RID: 31909 RVA: 0x0039F3E0 File Offset: 0x0039D5E0
		private void SetParticleAlpha(UnityEngine.Material mat)
		{
			int colorName = Shader.PropertyToID("_Color");
			Color color = mat.GetColor(colorName);
			color = color.SetAlpha(0f);
			mat.SetColor(colorName, color);
			Tweener tween = DOVirtual.Float(0f, 1f, 1.5f, delegate(float stepValue)
			{
				color = color.SetAlpha(stepValue);
				mat.SetColor(colorName, color);
			});
			tween.Play<Tweener>();
		}

		// Token: 0x06007CA6 RID: 31910 RVA: 0x0039F478 File Offset: 0x0039D678
		private void SetLeftArrow(Transform currNode)
		{
			this.rightArrow.gameObject.SetActive(false);
			this.leftArrow.gameObject.SetActive(true);
			this.leftArrow.SetParent(currNode);
			this.leftArrow.anchoredPosition = ViewShaolinDemonSlayer.LeftArrowPosition;
		}

		// Token: 0x06007CA7 RID: 31911 RVA: 0x0039F4C8 File Offset: 0x0039D6C8
		private void SetRightArrow(Transform currNode)
		{
			this.leftArrow.gameObject.SetActive(false);
			this.rightArrow.gameObject.SetActive(true);
			this.rightArrow.SetParent(currNode);
			this.rightArrow.anchoredPosition = ViewShaolinDemonSlayer.RightArrowPosition;
		}

		// Token: 0x04005EB9 RID: 24249
		[SerializeField]
		private Game.Components.Avatar.Avatar taiwuAvatar;

		// Token: 0x04005EBA RID: 24250
		[SerializeField]
		private TextMeshProUGUI taiwuNameLabel;

		// Token: 0x04005EBB RID: 24251
		[SerializeField]
		private CButton btnOpenTaiwu;

		// Token: 0x04005EBC RID: 24252
		[SerializeField]
		private CButton btnHeal;

		// Token: 0x04005EBD RID: 24253
		[SerializeField]
		private CButton btnBreak;

		// Token: 0x04005EBE RID: 24254
		[SerializeField]
		private CButton btnEquipment;

		// Token: 0x04005EBF RID: 24255
		[SerializeField]
		private CButton btnSkill;

		// Token: 0x04005EC0 RID: 24256
		[SerializeField]
		private TextMeshProUGUI levelTitleLabel;

		// Token: 0x04005EC1 RID: 24257
		[SerializeField]
		private RectTransform tower;

		// Token: 0x04005EC2 RID: 24258
		[SerializeField]
		private Transform nodes;

		// Token: 0x04005EC3 RID: 24259
		[SerializeField]
		private CImage towerFill;

		// Token: 0x04005EC4 RID: 24260
		[SerializeField]
		private RectTransform leftArrow;

		// Token: 0x04005EC5 RID: 24261
		[SerializeField]
		private RectTransform rightArrow;

		// Token: 0x04005EC6 RID: 24262
		[SerializeField]
		private TextMeshProUGUI expAmountLabel;

		// Token: 0x04005EC7 RID: 24263
		[SerializeField]
		private TooltipInvoker itemTips;

		// Token: 0x04005EC8 RID: 24264
		[SerializeField]
		private CImage itemIcon;

		// Token: 0x04005EC9 RID: 24265
		[SerializeField]
		private CImage itemGrade;

		// Token: 0x04005ECA RID: 24266
		[SerializeField]
		private TextMeshProUGUI itemAmountLabel;

		// Token: 0x04005ECB RID: 24267
		[SerializeField]
		private GameObject itemAmount;

		// Token: 0x04005ECC RID: 24268
		[SerializeField]
		private CButton btnRefresh;

		// Token: 0x04005ECD RID: 24269
		[SerializeField]
		private CButton btnGiveUp;

		// Token: 0x04005ECE RID: 24270
		[SerializeField]
		private TextMeshProUGUI refreshCountLabel;

		// Token: 0x04005ECF RID: 24271
		[SerializeField]
		private Game.Components.Avatar.Avatar leftBossAvatar;

		// Token: 0x04005ED0 RID: 24272
		[SerializeField]
		private Game.Components.Avatar.Avatar rightBossAvatar;

		// Token: 0x04005ED1 RID: 24273
		[SerializeField]
		private ShaolinDemonSlayerBoss leftBoss;

		// Token: 0x04005ED2 RID: 24274
		[SerializeField]
		private ShaolinDemonSlayerBoss rightBoss;

		// Token: 0x04005ED3 RID: 24275
		[SerializeField]
		private Sprite previousNode;

		// Token: 0x04005ED4 RID: 24276
		[SerializeField]
		private Sprite currentNode;

		// Token: 0x04005ED5 RID: 24277
		[SerializeField]
		private Sprite nextNode;

		// Token: 0x04005ED6 RID: 24278
		[SerializeField]
		private Transform inAnim;

		// Token: 0x04005ED7 RID: 24279
		[SerializeField]
		private Transform outAnim;

		// Token: 0x04005ED8 RID: 24280
		[SerializeField]
		private Transform particles;

		// Token: 0x04005ED9 RID: 24281
		[SerializeField]
		private CanvasGroup avatarCanvasGroup;

		// Token: 0x04005EDA RID: 24282
		[SerializeField]
		private CanvasGroup particleCanvasGroup;

		// Token: 0x04005EDB RID: 24283
		private List<int> _bossIdList = new List<int>();

		// Token: 0x04005EDC RID: 24284
		private List<ParticleSystem> _particleList = new List<ParticleSystem>();

		// Token: 0x04005EDD RID: 24285
		private SectShaolinDemonSlayerData _slayerData;

		// Token: 0x04005EDE RID: 24286
		private CharacterDisplayData _data;

		// Token: 0x04005EDF RID: 24287
		private int _leftIndex = -1;

		// Token: 0x04005EE0 RID: 24288
		private int _rightIndex = -1;

		// Token: 0x04005EE1 RID: 24289
		private Sequence _sequence;

		// Token: 0x04005EE2 RID: 24290
		private bool _playAnim = true;

		// Token: 0x04005EE3 RID: 24291
		private static readonly Vector2 RightArrowPosition = new Vector2(30f, 0f);

		// Token: 0x04005EE4 RID: 24292
		private static readonly Vector2 LeftArrowPosition = new Vector2(-38f, 0f);

		// Token: 0x04005EE5 RID: 24293
		private const int MaxLevel = 8;
	}
}
