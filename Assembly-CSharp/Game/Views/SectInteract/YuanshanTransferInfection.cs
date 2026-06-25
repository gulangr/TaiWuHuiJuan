using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UI;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Extra;
using GameData.Domains.Story.SectMainStory;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectInteract
{
	// Token: 0x020009BE RID: 2494
	public class YuanshanTransferInfection : MonoBehaviour
	{
		// Token: 0x17000D71 RID: 3441
		// (get) Token: 0x060078E4 RID: 30948 RVA: 0x00383917 File Offset: 0x00381B17
		private List<GameObject> EarlyParticles
		{
			get
			{
				return this._data.IsGoodEnd ? this.vitalParticles : this.targetParticles;
			}
		}

		// Token: 0x17000D72 RID: 3442
		// (get) Token: 0x060078E5 RID: 30949 RVA: 0x00383934 File Offset: 0x00381B34
		private List<GameObject> LateParticles
		{
			get
			{
				return this._data.IsGoodEnd ? this.targetParticles : this.vitalParticles;
			}
		}

		// Token: 0x17000D73 RID: 3443
		// (get) Token: 0x060078E6 RID: 30950 RVA: 0x00383951 File Offset: 0x00381B51
		private List<GameObject> CircleParticles
		{
			get
			{
				return this._data.IsGoodEnd ? this.goodParticles : this.badParticles;
			}
		}

		// Token: 0x17000D74 RID: 3444
		// (get) Token: 0x060078E7 RID: 30951 RVA: 0x0038396E File Offset: 0x00381B6E
		private int VitalInfection
		{
			get
			{
				return this._data.ThreeVitals[this._vitalIndex].Infection + (int)this.transferSlider.value * (this._data.IsGoodEnd ? -1 : 1);
			}
		}

		// Token: 0x17000D75 RID: 3445
		// (get) Token: 0x060078E8 RID: 30952 RVA: 0x003839AC File Offset: 0x00381BAC
		private int TargetInfection
		{
			get
			{
				return (this._targetCharId >= 0) ? ((int)this._data.PotentialTargetData[this._targetCharId].Infection + (int)this.transferSlider.value * (this._data.IsGoodEnd ? 1 : -1)) : 0;
			}
		}

		// Token: 0x17000D76 RID: 3446
		// (get) Token: 0x060078E9 RID: 30953 RVA: 0x003839FF File Offset: 0x00381BFF
		private SectStoryThreeVitalsCharacterType CurrType
		{
			get
			{
				return (SectStoryThreeVitalsCharacterType)this._vitalIndex;
			}
		}

		// Token: 0x060078EA RID: 30954 RVA: 0x00383A08 File Offset: 0x00381C08
		public void Init(Action<int, int, int> onClickConfirm)
		{
			this.btnClose.ClearAndAddListener(new Action(this.OnClickClose));
			this.btnPrev.ClearAndAddListener(new Action(this.OnClickPrev));
			this.btnNext.ClearAndAddListener(new Action(this.OnClickNext));
			this.btnSelect.ClearAndAddListener(new Action(this.OnClickSelectChar));
			this.btnDecrease.ClearAndAddListener(new Action(this.OnClickDecrease));
			this.btnIncrease.ClearAndAddListener(new Action(this.OnClickIncrease));
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.btnVital.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this._threeVitalsCharDataList[this._vitalIndex].CharacterId);
			});
			this.btnTarget.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this._targetCharId);
			});
			this.transferSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChange));
			this._onClickConfirm = onClickConfirm;
			for (int i = 0; i < this.spinesGood.childCount; i++)
			{
				SkeletonGraphic spine = this.spinesGood.GetChild(i).GetComponent<SkeletonGraphic>();
				spine.AnimationState.Complete += delegate(TrackEntry entry)
				{
					bool flag = entry.Animation.Name != "idle";
					if (flag)
					{
						spine.AnimationState.SetAnimation(0, "idle", true);
					}
				};
			}
			for (int j = 0; j < this.spinesBad.childCount; j++)
			{
				SkeletonGraphic spine = this.spinesBad.GetChild(j).GetComponent<SkeletonGraphic>();
				spine.AnimationState.Complete += delegate(TrackEntry entry)
				{
					bool flag = entry.Animation.Name != "idle";
					if (flag)
					{
						spine.AnimationState.SetAnimation(0, "idle", true);
					}
				};
			}
		}

		// Token: 0x060078EB RID: 30955 RVA: 0x00383BC4 File Offset: 0x00381DC4
		public void Set(SectYuanshanThreeVitalsData data, List<CharacterDisplayData> vitalData, int vitalIndex)
		{
			this._data = data;
			this._threeVitalsCharDataList = vitalData;
			bool isGoodEnd = this._data.IsGoodEnd;
			if (isGoodEnd)
			{
				this.goodBg.SetActive(true);
				this.goodFrame.SetActive(true);
				this.goodNameFrame.SetActive(true);
				this.badBg.SetActive(false);
				this.badFrame.SetActive(false);
				this.badNameFrame.SetActive(false);
				this.spinesGood.gameObject.SetActive(true);
				this.spinesBad.gameObject.SetActive(false);
				this.btnConfirmText.text = LanguageKey.LK_ThreeVitals_SelectTarget_Good.Tr();
				this.particles.eulerAngles = this.particles.eulerAngles.SetY(0f);
			}
			else
			{
				this.goodBg.SetActive(false);
				this.goodFrame.SetActive(false);
				this.goodNameFrame.SetActive(false);
				this.badBg.SetActive(true);
				this.badFrame.SetActive(true);
				this.badNameFrame.SetActive(true);
				this.spinesGood.gameObject.SetActive(false);
				this.spinesBad.gameObject.SetActive(true);
				this.btnConfirmText.text = LanguageKey.LK_ThreeVitals_SelectTarget_Bad.Tr();
				this.particles.eulerAngles = this.particles.eulerAngles.SetY(180f);
			}
			int reverseThreshold = data.IsGoodEnd ? GlobalConfig.Instance.ThreeVitalsThresholdHigh : GlobalConfig.Instance.ThreeVitalsThresholdLow;
			this.vitalProgress.Set(reverseThreshold, GlobalConfig.Instance.ThreeVitalsMaxInfection);
			this.targetProgress.Set(100, 200);
			this.SetVital(vitalIndex);
			this.SetTarget(-1);
		}

		// Token: 0x060078EC RID: 30956 RVA: 0x00383DA8 File Offset: 0x00381FA8
		public void SetVital(int vitalIndex)
		{
			this._vitalIndex = vitalIndex;
			Transform spines = this._data.IsGoodEnd ? this.spinesGood : this.spinesBad;
			for (int i = 0; i < spines.childCount; i++)
			{
				spines.GetChild(i).gameObject.SetActive(i == this._vitalIndex);
			}
			CharacterDisplayData sourceData = this._threeVitalsCharDataList[this._vitalIndex];
			string sourceName = NameCenter.GetMonasticTitleOrDisplayName(sourceData, false);
			this.vitalProgress.SetTips(this._data.IsGoodEnd, this._data.ThreeVitals[vitalIndex]);
			this.vitalName.text = sourceName;
		}

		// Token: 0x060078ED RID: 30957 RVA: 0x00383E5C File Offset: 0x0038205C
		public void SetTarget(int charId)
		{
			this._targetCharId = charId;
			bool flag = this._targetCharId < 0;
			if (flag)
			{
				this.targetName.text = LanguageKey.LK_None.Tr();
				this.targetAvatar.gameObject.SetActive(false);
				this.targetProgress.gameObject.SetActive(false);
				this.targetName.gameObject.SetActive(false);
				this.btnTarget.gameObject.SetActive(false);
				this.hideObj.SetActive(true);
				this.emptyFrame.SetActive(true);
				this.targetFrame.SetActive(false);
			}
			else
			{
				CharacterDisplayData displayData = this._data.CharacterDisplayData[this._targetCharId];
				this.targetName.text = NameCenter.GetMonasticTitleOrDisplayName(displayData, false);
				this.targetAvatar.Refresh(displayData, true);
				this.targetAvatar.gameObject.SetActive(true);
				this.targetProgress.gameObject.SetActive(true);
				this.targetName.gameObject.SetActive(true);
				this.btnTarget.gameObject.SetActive(true);
				this.hideObj.SetActive(false);
				this.emptyFrame.SetActive(false);
				this.targetFrame.SetActive(true);
			}
			this.RefreshTransferPanel();
		}

		// Token: 0x060078EE RID: 30958 RVA: 0x00383FBD File Offset: 0x003821BD
		private void OnDisable()
		{
			this.StopEarlyAnimation();
			this.StopLateAnimation();
		}

		// Token: 0x060078EF RID: 30959 RVA: 0x00383FD0 File Offset: 0x003821D0
		private void RefreshTransferPanel()
		{
			int vitalInfection = this._data.ThreeVitals[this._vitalIndex].Infection;
			int targetInfection = (int)((this._targetCharId >= 0) ? this._data.PotentialTargetData[this._targetCharId].Infection : 0);
			this._transferMaxValue = ((this._targetCharId < 0) ? 0 : (this._data.IsGoodEnd ? Math.Min(vitalInfection, 200 - targetInfection) : Math.Min(GlobalConfig.Instance.ThreeVitalsMaxInfection - vitalInfection, targetInfection)));
			this.transferSlider.maxValue = (float)this._transferMaxValue;
			this.transferSlider.value = 0f;
			this.transferTitle.text = (this._data.IsGoodEnd ? LanguageKey.LK_ThreeVitals_Transfer_Good.Tr() : LanguageKey.LK_ThreeVitals_Transfer_Bad.Tr());
			this.OnSliderChange(0f);
			bool canInteract = this._transferMaxValue > 0;
			this.transferSlider.interactable = canInteract;
			this.btnDecrease.interactable = (canInteract && this.transferSlider.value > 0f);
			this.btnIncrease.interactable = (canInteract && this.transferSlider.value < this.transferSlider.maxValue);
		}

		// Token: 0x060078F0 RID: 30960 RVA: 0x00384128 File Offset: 0x00382328
		public void ShowCharacterMenu(int id)
		{
			bool flag = id >= 0;
			if (flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", id);
				argBox.Set("CanOperate", false);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x060078F1 RID: 30961 RVA: 0x00384180 File Offset: 0x00382380
		private void UpdateEarlyAnimation()
		{
			bool interactable = this.btnConfirm.interactable;
			if (interactable)
			{
				bool flag = !this._isPlayingEarlyParticle;
				if (flag)
				{
					this._isPlayingEarlyParticle = true;
					this.PlayEarlyAnimation();
				}
			}
			else
			{
				bool isPlayingEarlyParticle = this._isPlayingEarlyParticle;
				if (isPlayingEarlyParticle)
				{
					this._isPlayingEarlyParticle = false;
					this.StopEarlyAnimation();
				}
			}
		}

		// Token: 0x060078F2 RID: 30962 RVA: 0x003841DC File Offset: 0x003823DC
		private void PlayEarlyAnimation()
		{
			this._earlySequence = DOTween.Sequence();
			this._earlySequence.AppendCallback(delegate
			{
				this.EarlyParticles[0].SetActive(true);
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.EarlyParticles[0].GetComponent<ParticleSystem>().Play();
			});
			this._earlySequence.AppendInterval(1f);
			this._earlySequence.AppendCallback(delegate
			{
				this.EarlyParticles[1].SetActive(true);
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.EarlyParticles[1].GetComponent<ParticleSystem>().Play();
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.EarlyParticles[0].SetActive(false);
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.CircleParticles[0].SetActive(true);
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.CircleParticles[0].GetComponent<ParticleSystem>().Play();
			});
			this._earlySequence.AppendInterval(4f);
			this._earlySequence.AppendCallback(delegate
			{
				this.CircleParticles[0].SetActive(false);
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.CircleParticles[1].SetActive(true);
			});
			this._earlySequence.AppendCallback(delegate
			{
				this.CircleParticles[1].GetComponent<ParticleSystem>().Play();
			});
			this._earlySequence.Play<DG.Tweening.Sequence>();
		}

		// Token: 0x060078F3 RID: 30963 RVA: 0x00384314 File Offset: 0x00382514
		private void StopEarlyAnimation()
		{
			this._earlySequence.Kill(false);
			this.EarlyParticles[0].SetActive(false);
			this.EarlyParticles[1].SetActive(false);
			this.CircleParticles[0].SetActive(false);
			this.CircleParticles[1].SetActive(false);
			this._isPlayingEarlyParticle = false;
		}

		// Token: 0x060078F4 RID: 30964 RVA: 0x00384384 File Offset: 0x00382584
		private void PlayLateAnimation()
		{
			this._lateSequence = DOTween.Sequence();
			this._lateSequence.AppendCallback(delegate
			{
				float delta = this.transferSlider.value;
				int vitalStart = this._data.ThreeVitals[this._vitalIndex].Infection;
				byte targetStart = this._data.PotentialTargetData[this._targetCharId].Infection;
				int textIndex = this._data.IsGoodEnd ? 0 : 1;
				this._sliderTweener = DOVirtual.Float(delta, 0f, 2f, delegate(float inter)
				{
					this.UpdateDisplayInAnimation(vitalStart, (int)targetStart, (int)inter);
				});
				this.ShowBubble(YuanshanTransferInfection.VitalBubbleConfig[this.CurrType][textIndex].Tr());
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.mask.SetActive(true);
			});
			this._lateSequence.AppendCallback(new TweenCallback(this.StopEarlyAnimation));
			this._lateSequence.AppendCallback(delegate
			{
				this.EarlyParticles[2].SetActive(true);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.EarlyParticles[2].GetComponent<ParticleSystem>().Play();
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.CircleParticles[2].SetActive(true);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.CircleParticles[2].GetComponent<ParticleSystem>().Play();
			});
			this._lateSequence.AppendInterval(1f);
			this._lateSequence.AppendCallback(delegate
			{
				this.EarlyParticles[2].SetActive(false);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[0].SetActive(true);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[0].GetComponent<ParticleSystem>().Play();
			});
			this._lateSequence.AppendInterval(1f);
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[0].SetActive(false);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[1].SetActive(true);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[1].GetComponent<ParticleSystem>().Play();
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.CircleParticles[2].SetActive(false);
			});
			this._lateSequence.AppendCallback(new TweenCallback(this.RefreshTransferPanel));
			this._lateSequence.AppendCallback(delegate
			{
				this.mask.SetActive(false);
			});
			this._lateSequence.AppendInterval(1f);
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[1].SetActive(false);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[2].SetActive(true);
			});
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[2].GetComponent<ParticleSystem>().Play();
			});
			this._lateSequence.AppendInterval(1f);
			this._lateSequence.AppendCallback(delegate
			{
				this.LateParticles[2].SetActive(false);
			});
			this._lateSequence.Play<DG.Tweening.Sequence>();
		}

		// Token: 0x060078F5 RID: 30965 RVA: 0x003845D0 File Offset: 0x003827D0
		private void StopLateAnimation()
		{
			this._lateSequence.Kill(false);
			Tweener sliderTweener = this._sliderTweener;
			if (sliderTweener != null)
			{
				sliderTweener.Kill(false);
			}
			this.mask.SetActive(false);
			this.EarlyParticles[2].SetActive(false);
			this.LateParticles[0].SetActive(false);
			this.CircleParticles[2].SetActive(false);
			this.LateParticles[1].SetActive(false);
			this.LateParticles[2].SetActive(false);
			this.HideBubble();
			this.RefreshTransferPanel();
		}

		// Token: 0x060078F6 RID: 30966 RVA: 0x00384678 File Offset: 0x00382878
		public void ShowBubble(string text)
		{
			Transform spines = this._data.IsGoodEnd ? this.spinesGood : this.spinesBad;
			SkeletonGraphic spine = spines.GetChild(this._vitalIndex).GetComponent<SkeletonGraphic>();
			this.bubbleText.text = text;
			this.bubble.SetActive(true);
			bool flag = this._closeBubbleCoroutines != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(this._closeBubbleCoroutines);
			}
			this._closeBubbleCoroutines = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(2f, new Action(this.HideBubble));
			spine.AnimationState.SetAnimation(0, "talk", false);
		}

		// Token: 0x060078F7 RID: 30967 RVA: 0x00384720 File Offset: 0x00382920
		public void HideBubble()
		{
			bool flag = this.bubble != null;
			if (flag)
			{
				this.bubble.SetActive(false);
			}
		}

		// Token: 0x060078F8 RID: 30968 RVA: 0x0038474C File Offset: 0x0038294C
		private void UpdateDisplayInAnimation(int vitalStart, int targetStart, int value)
		{
			this.transferSlider.SetValueWithoutNotify((float)value);
			this.transferValue.text = string.Format("{0} / {1}", value.ToString().SetColor((value == 0) ? "grey" : "brightyellow"), this._transferMaxValue);
			int vitalValue = vitalStart + value * (this._data.IsGoodEnd ? 1 : -1);
			int targetValue = targetStart + value * (this._data.IsGoodEnd ? -1 : 1);
			this.vitalProgress.SetProgress(vitalValue);
			this.targetProgress.SetProgress(targetValue);
		}

		// Token: 0x060078F9 RID: 30969 RVA: 0x003847EC File Offset: 0x003829EC
		private void OnSliderChange(float fv)
		{
			int value = (int)fv;
			this.transferValue.text = string.Format("{0} / {1}", value.ToString().SetColor((value == 0) ? "grey" : "brightyellow"), this._transferMaxValue);
			this.btnConfirm.interactable = (value != 0);
			this.btnIncrease.interactable = ((float)value < this.transferSlider.maxValue);
			this.btnDecrease.interactable = (value > 0);
			this.vitalProgress.SetProgress(this.VitalInfection);
			this.targetProgress.SetProgress(this.TargetInfection);
			this.UpdateEarlyAnimation();
		}

		// Token: 0x060078FA RID: 30970 RVA: 0x0038489F File Offset: 0x00382A9F
		private void OnClickDecrease()
		{
			this.transferSlider.value = Math.Clamp(this.transferSlider.value - 1f, 0f, this.transferSlider.maxValue);
		}

		// Token: 0x060078FB RID: 30971 RVA: 0x003848D4 File Offset: 0x00382AD4
		private void OnClickIncrease()
		{
			this.transferSlider.value = Math.Clamp(this.transferSlider.value + 1f, 0f, this.transferSlider.maxValue);
		}

		// Token: 0x060078FC RID: 30972 RVA: 0x0038490C File Offset: 0x00382B0C
		private void OnClickConfirm()
		{
			this.PlayLateAnimation();
			this.btnConfirm.interactable = false;
			this._data.ThreeVitals[this._vitalIndex].Infection = this.VitalInfection;
			this._data.PotentialTargetData[this._targetCharId].Infection = (byte)this.TargetInfection;
			this._onClickConfirm(this._vitalIndex, this._targetCharId, (int)this.transferSlider.value);
		}

		// Token: 0x060078FD RID: 30973 RVA: 0x00384998 File Offset: 0x00382B98
		private void OnClickSelectChar()
		{
			bool flag = this._data.PotentialTargetData == null || this._data.PotentialTargetData.Count == 0;
			if (!flag)
			{
				bool isWaitingForSelectData = this._isWaitingForSelectData;
				if (!isWaitingForSelectData)
				{
					this._isWaitingForSelectData = true;
					List<int> keys = new List<int>(this._data.PotentialTargetData.Keys);
					CharacterDomainMethod.AsyncCall.GetYuanshanSelectDataList(null, keys, delegate(int offset, RawDataPool dataPool)
					{
						this._isWaitingForSelectData = false;
						List<CharacterDisplayDataForYuanshanSelect> selectDataList = new List<CharacterDisplayDataForYuanshanSelect>();
						Serializer.Deserialize(dataPool, offset, ref selectDataList);
						this.ShowSelectCharacterUI(selectDataList);
					});
				}
			}
		}

		// Token: 0x060078FE RID: 30974 RVA: 0x00384A0C File Offset: 0x00382C0C
		private void ShowSelectCharacterUI(List<CharacterDisplayDataForYuanshanSelect> dataList)
		{
			List<ISelectCharacterData> selectList = new List<ISelectCharacterData>();
			bool flag = dataList != null;
			if (flag)
			{
				foreach (CharacterDisplayDataForYuanshanSelect item in dataList)
				{
					selectList.Add(new YuanshanSelectCharacterDataAdapter(item));
				}
			}
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.YuanshanInfection);
			config.InteractionMode = ESelectCharacterInteractionMode.Instant;
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.SetObject("SelectCharacterConfig", config);
			args.SetObject("SelectCharacterDataList", selectList);
			args.SetObject("SelectCharacterCallback", new SelectCharacterCallback(this.OnSelectCharacter));
			UIElement.SelectChar.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		}

		// Token: 0x060078FF RID: 30975 RVA: 0x00384ADC File Offset: 0x00382CDC
		private void OnSelectCharacter(List<int> selectedIds)
		{
			bool flag = selectedIds != null && selectedIds.Count > 0;
			if (flag)
			{
				this.SetTarget(selectedIds[0]);
			}
		}

		// Token: 0x06007900 RID: 30976 RVA: 0x00384B0D File Offset: 0x00382D0D
		private IEnumerable<ColumnDefinition> GenerateYuanshanColumns()
		{
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_Column_Infect.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				YuanshanSelectCharacterDataAdapter adapter = data as YuanshanSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					result = adapter.Data.Infection.ToString();
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition.SortId = 123;
			yield return columnDefinition;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition2 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_RelationShip.Tr());
			columnDefinition2.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				bool flag = generalData == null;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					bool flag2 = generalData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					if (flag2)
					{
						result = "-";
					}
					else
					{
						result = YuanshanTransferInfection.GetHighestPriorityRelationText(generalData.RelationToTaiwu, generalData.IsSameFactionWithTaiwu);
					}
				}
				return result;
			};
			yield return columnDefinition2;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition3 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Favorability.Tr());
			columnDefinition3.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetFavorStringByInteracted(generalData.FavorabilityToTaiwu, generalData.IsInteractedWithTaiwu);
			};
			columnDefinition3.SortId = 11;
			yield return columnDefinition3;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition4 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr());
			columnDefinition4.CellDataGenerator = ((ISelectCharacterData data) => CommonUtils.GetBehaviorString(data.GetGeneralScrollListData().BehaviorType));
			columnDefinition4.SortId = 57;
			yield return columnDefinition4;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition5 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr());
			columnDefinition5.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetFameString(FameType.GetFameType(generalData.Fame));
			};
			columnDefinition5.SortId = 59;
			yield return columnDefinition5;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition6 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr());
			columnDefinition6.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)generalData.CharacterTemplateId, generalData.OrgInfo, generalData.Gender, generalData.PhysiologicalAge, false);
			};
			columnDefinition6.SortId = 1;
			yield return columnDefinition6;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition7 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr());
			columnDefinition7.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				string text;
				return CommonUtils.TryGetCharacterSpecialGradeName((int)generalData.CharacterTemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(generalData.OrgInfo);
			};
			yield return columnDefinition7;
			yield break;
		}

		// Token: 0x06007901 RID: 30977 RVA: 0x00384B20 File Offset: 0x00382D20
		private static string GetHighestPriorityRelationText(ushort relationToTaiwu, bool isSameFaction)
		{
			bool flag = RelationType.ContainParentRelations(relationToTaiwu);
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Parent);
			}
			else
			{
				bool flag2 = RelationType.ContainChildRelations(relationToTaiwu);
				if (flag2)
				{
					result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Child);
				}
				else
				{
					bool flag3 = RelationType.ContainBrotherOrSisterRelations(relationToTaiwu);
					if (flag3)
					{
						result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Bro);
					}
					else
					{
						bool flag4 = RelationType.HasRelation(relationToTaiwu, 1024);
						if (flag4)
						{
							result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Wife);
						}
						else
						{
							bool flag5 = RelationType.HasRelation(relationToTaiwu, 32768);
							if (flag5)
							{
								result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy);
							}
							else
							{
								bool flag6 = RelationType.HasRelation(relationToTaiwu, 16384);
								if (flag6)
								{
									result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored);
								}
								else
								{
									bool flag7 = RelationType.HasRelation(relationToTaiwu, 2048) || RelationType.HasRelation(relationToTaiwu, 4096);
									if (flag7)
									{
										result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Mentor);
									}
									else
									{
										bool flag8 = RelationType.HasRelation(relationToTaiwu, 512);
										if (flag8)
										{
											result = LocalStringManager.Get(LanguageKey.LK_RelationShip_SwornBro);
										}
										else
										{
											bool flag9 = RelationType.HasRelation(relationToTaiwu, 8192);
											if (flag9)
											{
												result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Friend);
											}
											else if (isSameFaction)
											{
												result = LocalStringManager.Get(LanguageKey.LK_Faction);
											}
											else
											{
												result = "-";
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

		// Token: 0x06007902 RID: 30978 RVA: 0x00384C66 File Offset: 0x00382E66
		private void OnClickPrev()
		{
			this.SetVital(this.GetValidVitalIndex(-1));
			this.RefreshTransferPanel();
		}

		// Token: 0x06007903 RID: 30979 RVA: 0x00384C7E File Offset: 0x00382E7E
		private void OnClickNext()
		{
			this.SetVital(this.GetValidVitalIndex(1));
			this.RefreshTransferPanel();
		}

		// Token: 0x06007904 RID: 30980 RVA: 0x00384C98 File Offset: 0x00382E98
		private void OnClickClose()
		{
			UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
			manager.DetachMask(base.transform);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06007905 RID: 30981 RVA: 0x00384CC8 File Offset: 0x00382EC8
		private int GetValidVitalIndex(int delta)
		{
			int index = this._vitalIndex;
			for (;;)
			{
				index += delta;
				bool flag = index == this._vitalIndex;
				if (flag)
				{
					break;
				}
				bool flag2 = index < 0;
				if (flag2)
				{
					index = this._data.ThreeVitals.Count - 1;
				}
				bool flag3 = index >= this._data.ThreeVitals.Count;
				if (flag3)
				{
					index = 0;
				}
				bool flag4 = !this._data.ThreeVitals[index].IsInPrison;
				if (flag4)
				{
					goto Block_4;
				}
			}
			return index;
			Block_4:
			return index;
		}

		// Token: 0x04005B8A RID: 23434
		public GameObject goodBg;

		// Token: 0x04005B8B RID: 23435
		public GameObject badBg;

		// Token: 0x04005B8C RID: 23436
		public GameObject goodFrame;

		// Token: 0x04005B8D RID: 23437
		public GameObject badFrame;

		// Token: 0x04005B8E RID: 23438
		public GameObject targetFrame;

		// Token: 0x04005B8F RID: 23439
		public CButton btnClose;

		// Token: 0x04005B90 RID: 23440
		public CButton btnPrev;

		// Token: 0x04005B91 RID: 23441
		public CButton btnNext;

		// Token: 0x04005B92 RID: 23442
		public CButton btnSelect;

		// Token: 0x04005B93 RID: 23443
		public Transform spinesGood;

		// Token: 0x04005B94 RID: 23444
		public Transform spinesBad;

		// Token: 0x04005B95 RID: 23445
		public TextMeshProUGUI transferTitle;

		// Token: 0x04005B96 RID: 23446
		public TextMeshProUGUI transferValue;

		// Token: 0x04005B97 RID: 23447
		public CSlider transferSlider;

		// Token: 0x04005B98 RID: 23448
		public CButton btnDecrease;

		// Token: 0x04005B99 RID: 23449
		public CButton btnIncrease;

		// Token: 0x04005B9A RID: 23450
		public CButton btnConfirm;

		// Token: 0x04005B9B RID: 23451
		public CButton btnVital;

		// Token: 0x04005B9C RID: 23452
		public CButton btnTarget;

		// Token: 0x04005B9D RID: 23453
		public TextMeshProUGUI btnConfirmText;

		// Token: 0x04005B9E RID: 23454
		public TextMeshProUGUI vitalName;

		// Token: 0x04005B9F RID: 23455
		public TextMeshProUGUI targetName;

		// Token: 0x04005BA0 RID: 23456
		public GameObject goodNameFrame;

		// Token: 0x04005BA1 RID: 23457
		public GameObject badNameFrame;

		// Token: 0x04005BA2 RID: 23458
		public YuanshanVitalProgress vitalProgress;

		// Token: 0x04005BA3 RID: 23459
		public YuanshanVitalProgress targetProgress;

		// Token: 0x04005BA4 RID: 23460
		public Game.Components.Avatar.Avatar targetAvatar;

		// Token: 0x04005BA5 RID: 23461
		public GameObject hideObj;

		// Token: 0x04005BA6 RID: 23462
		public GameObject emptyFrame;

		// Token: 0x04005BA7 RID: 23463
		public RectTransform particles;

		// Token: 0x04005BA8 RID: 23464
		public List<GameObject> vitalParticles;

		// Token: 0x04005BA9 RID: 23465
		public List<GameObject> targetParticles;

		// Token: 0x04005BAA RID: 23466
		public List<GameObject> goodParticles;

		// Token: 0x04005BAB RID: 23467
		public List<GameObject> badParticles;

		// Token: 0x04005BAC RID: 23468
		public GameObject mask;

		// Token: 0x04005BAD RID: 23469
		public GameObject bubble;

		// Token: 0x04005BAE RID: 23470
		public TextMeshProUGUI bubbleText;

		// Token: 0x04005BAF RID: 23471
		private SectYuanshanThreeVitalsData _data;

		// Token: 0x04005BB0 RID: 23472
		private List<CharacterDisplayData> _threeVitalsCharDataList;

		// Token: 0x04005BB1 RID: 23473
		private Action<int, int, int> _onClickConfirm;

		// Token: 0x04005BB2 RID: 23474
		private bool _isPlayingEarlyParticle = false;

		// Token: 0x04005BB3 RID: 23475
		private DG.Tweening.Sequence _earlySequence;

		// Token: 0x04005BB4 RID: 23476
		private DG.Tweening.Sequence _lateSequence;

		// Token: 0x04005BB5 RID: 23477
		private int _transferMaxValue;

		// Token: 0x04005BB6 RID: 23478
		private int _targetCharId;

		// Token: 0x04005BB7 RID: 23479
		private int _vitalIndex;

		// Token: 0x04005BB8 RID: 23480
		private Coroutine _closeBubbleCoroutines;

		// Token: 0x04005BB9 RID: 23481
		private Tweener _sliderTweener;

		// Token: 0x04005BBA RID: 23482
		private static readonly Dictionary<SectStoryThreeVitalsCharacterType, LanguageKey[]> VitalBubbleConfig = new Dictionary<SectStoryThreeVitalsCharacterType, LanguageKey[]>
		{
			{
				SectStoryThreeVitalsCharacterType.Heaven,
				new LanguageKey[]
				{
					LanguageKey.LK_ThreeVitals_Speak_Giveaway_Sky,
					LanguageKey.LK_ThreeVitals_Speak_Drain_Sky
				}
			},
			{
				SectStoryThreeVitalsCharacterType.Earth,
				new LanguageKey[]
				{
					LanguageKey.LK_ThreeVitals_Speak_Giveaway_Earth,
					LanguageKey.LK_ThreeVitals_Speak_Drain_Earth
				}
			},
			{
				SectStoryThreeVitalsCharacterType.Human,
				new LanguageKey[]
				{
					LanguageKey.LK_ThreeVitals_Speak_Giveaway_Human,
					LanguageKey.LK_ThreeVitals_Speak_Drain_Human
				}
			}
		};

		// Token: 0x04005BBB RID: 23483
		private bool _isWaitingForSelectData;
	}
}
