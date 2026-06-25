using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B24 RID: 2852
	public class CombatCastSkill : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F70 RID: 3952
		// (get) Token: 0x06008BEA RID: 35818 RVA: 0x0040A63B File Offset: 0x0040883B
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008BEB RID: 35819 RVA: 0x0040A644 File Offset: 0x00408844
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.SetPreparingSkill));
			CombatModel model2 = this.Model;
			model2.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnPerformingSkillIdChanged));
			CombatModel model3 = this.Model;
			model3.OnSkillPreparePercentChanged = (OnDataChangedEvent)Delegate.Combine(model3.OnSkillPreparePercentChanged, new OnDataChangedEvent(this.OnSkillPreparePercentChanged));
			CombatModel model4 = this.Model;
			model4.OnAutoCastingSkillChanged = (OnDataChangedEvent)Delegate.Combine(model4.OnAutoCastingSkillChanged, new OnDataChangedEvent(this.OnAutoCastingSkillChanged));
			this.Model.AddEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008BEC RID: 35820 RVA: 0x0040A720 File Offset: 0x00408920
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(model.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.SetPreparingSkill));
			CombatModel model2 = this.Model;
			model2.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnPerformingSkillIdChanged));
			CombatModel model3 = this.Model;
			model3.OnSkillPreparePercentChanged = (OnDataChangedEvent)Delegate.Remove(model3.OnSkillPreparePercentChanged, new OnDataChangedEvent(this.OnSkillPreparePercentChanged));
			CombatModel model4 = this.Model;
			model4.OnAutoCastingSkillChanged = (OnDataChangedEvent)Delegate.Remove(model4.OnAutoCastingSkillChanged, new OnDataChangedEvent(this.OnAutoCastingSkillChanged));
			this.Model.RemoveEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008BED RID: 35821 RVA: 0x0040A7FC File Offset: 0x004089FC
		private void Awake()
		{
			this.InitRefers();
			base.gameObject.SetActive(false);
		}

		// Token: 0x06008BEE RID: 35822 RVA: 0x0040A814 File Offset: 0x00408A14
		private void SetProgressDisplay(float p)
		{
			float _amount = Mathf.Lerp(-0.1f, 1f, 1f - p);
			this.progress.material.SetFloat(CombatCastSkill.ShaderPropId_DissolveAmount, _amount);
		}

		// Token: 0x06008BEF RID: 35823 RVA: 0x0040A850 File Offset: 0x00408A50
		private void SetPreparingSkill(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					short skillId = processor.PreparingSkillId;
					short performingSkillId = processor.PerformingSkillId;
					base.gameObject.SetActive(skillId >= 0 || performingSkillId >= 0);
					TooltipInvoker mouseTip = this.progressBack;
					bool flag3 = skillId >= 0;
					if (flag3)
					{
						CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
						this.txtName.text = skillConfig.Name;
						this.skillIcon.SetSprite(skillConfig.Icon, false, null);
						this.skillIcon.SetColor(Color.white);
						this.frame.SetSprite(CombatCastSkill.FramePaths[(int)skillConfig.EquipType] + skillConfig.Grade.ToString(), false, null);
						this.fiveElementsFrame.SetSprite(string.Format("{0}{1}", "ui9_back_combat_skill_five_elements_frame_", skillConfig.EquipType), false, null);
						this.fiveElementsFrame.color = CommonUtils.GetFiveElementColor((int)skillConfig.FiveElements);
						this.SetProgressDisplay(0f);
						this.flashProgress.fillAmount = 0f;
						for (int i = 0; i < this._flashPointStatus.Count; i++)
						{
							this._flashPointStatus[i] = false;
						}
						bool flag4 = !isAlly;
						if (flag4)
						{
							GlobalDomainMethod.Call.InvokeGuidingTrigger(172);
						}
						int randomSoundIndex = Random.Range(0, 3);
						if (!true)
						{
						}
						string text;
						switch (randomSoundIndex)
						{
						case 0:
							text = (isAlly ? "combat_prepare_skill_our_01" : "combat_prepare_skill_enemy_01");
							break;
						case 1:
							text = (isAlly ? "combat_prepare_skill_our_02" : "combat_prepare_skill_enemy_02");
							break;
						case 2:
							text = (isAlly ? "combat_prepare_skill_our_03" : "combat_prepare_skill_enemy_03");
							break;
						default:
							throw new ArgumentOutOfRangeException("randomSoundIndex", randomSoundIndex, null);
						}
						if (!true)
						{
						}
						string soundName = text;
						AudioManager.Instance.PlaySound(soundName, false, false);
						AudioManager.Instance.PlaySound("battle_ChargeUp", true, false);
						bool flag5 = !isAlly && SingletonObject.getInstance<GlobalSettings>().AutoPauseInCastSkill && skillConfig.EquipType == 1 && !this.Model.IsPausing && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
						if (flag5)
						{
							this.Model.RaiseEvent(ECombatEvents.SwitchPauseState);
						}
						bool flag6 = isAlly && SingletonObject.getInstance<GlobalSettings>().AutoPauseInAllyCastSkill && skillConfig.EquipType == 1 && !this.Model.IsPausing && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
						if (flag6)
						{
							this.Model.RaiseEvent(ECombatEvents.SwitchPauseState);
						}
						CombatUtils.SetCombatSkillTips(mouseTip, charId, skillId);
					}
					else
					{
						AudioManager.Instance.StopSound("battle_ChargeUp");
					}
					bool flag7 = isAlly && skillId >= 0;
					if (flag7)
					{
						this.Model.DoRequestGetAllCostNeiliEffectData();
					}
					if (isAlly)
					{
						this.Model.DoRequestCanCostTrickDuringPreparingSkill();
					}
					if (isAlly)
					{
						GEvent.OnEvent((skillId >= 0) ? UiEvents.OnSkillCasting : UiEvents.OnSkillCasted, null);
					}
				}
			}
		}

		// Token: 0x06008BF0 RID: 35824 RVA: 0x0040ABAC File Offset: 0x00408DAC
		private void OnPerformingSkillIdChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.OnPerformingSkillIdChangedByCharId(charId);
			}
		}

		// Token: 0x06008BF1 RID: 35825 RVA: 0x0040ABF0 File Offset: 0x00408DF0
		private void OnPerformingSkillIdChangedByCharId(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				short skillId = processor.PerformingSkillId;
				short preparingSkillId = processor.PreparingSkillId;
				base.gameObject.SetActive(skillId >= 0 || preparingSkillId >= 0);
				TooltipInvoker mouseTip = this.progressBack;
				bool flag2 = skillId >= 0;
				if (flag2)
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
					this.txtName.text = skillConfig.Name;
					this.skillIcon.SetSprite(skillConfig.Icon, false, null);
					this.skillIcon.SetColor(Color.white);
					this.frame.SetSprite(CombatCastSkill.FramePaths[(int)skillConfig.EquipType] + skillConfig.Grade.ToString(), false, null);
					this.fiveElementsFrame.SetSprite(string.Format("{0}{1}", "ui9_back_combat_skill_five_elements_frame_", skillConfig.EquipType), false, null);
					this.fiveElementsFrame.color = CommonUtils.GetFiveElementColor((int)skillConfig.FiveElements);
					CombatUtils.SetCombatSkillTips(mouseTip, charId, skillId);
				}
				CombatCastSkill.LowerBgmForCombatMusic((int)skillId);
			}
		}

		// Token: 0x06008BF2 RID: 35826 RVA: 0x0040AD24 File Offset: 0x00408F24
		private void OnSkillPreparePercentChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					byte skillPreparePercent = processor.SkillPreparePercent;
					float fillAmount = (float)skillPreparePercent / 100f;
					this.SetProgressDisplay(fillAmount);
					this.flashProgress.fillAmount = fillAmount;
					for (int i = 0; i < this._flashPointList.Count; i++)
					{
						bool isCanFlash = fillAmount >= (float)(i + 1) * 0.25f;
						bool flag3 = isCanFlash && !this._flashPointStatus[i];
						if (flag3)
						{
							this._flashPointStatus[i] = true;
							this._flashPointList[i].SetActive(true);
							int i1 = i;
							DOVirtual.DelayedCall(0.9f, delegate
							{
								this._flashPointList[i1].SetActive(false);
							}, false);
						}
					}
				}
			}
		}

		// Token: 0x06008BF3 RID: 35827 RVA: 0x0040AE5C File Offset: 0x0040905C
		private void OnAutoCastingSkillChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					bool isAutoCastingSkill = processor.AutoCastingSkill;
					this.progress.SetSprite(isAutoCastingSkill ? "combat_bifen_shizhan_4" : "combat_bifen_shizhan_0", false, null);
				}
			}
		}

		// Token: 0x06008BF4 RID: 35828 RVA: 0x0040AEDA File Offset: 0x004090DA
		private void OnCombatEnd()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06008BF5 RID: 35829 RVA: 0x0040AEEC File Offset: 0x004090EC
		private static void LowerBgmForCombatMusic(int skillId)
		{
			bool flag = skillId >= 0 && CombatSkill.Instance[skillId].Type == 13;
			if (flag)
			{
				AudioManager.Instance.SetMusicVolume(0.6f * (float)SingletonObject.getInstance<GlobalSettings>().BgmVolume / 100f, true);
			}
			else
			{
				AudioManager.Instance.SetMusicVolume(1f * (float)SingletonObject.getInstance<GlobalSettings>().BgmVolume / 100f, true);
			}
		}

		// Token: 0x06008BF6 RID: 35830 RVA: 0x0040AF64 File Offset: 0x00409164
		private void OnChangeChar()
		{
			int charId = this.Model.ChangingToCharId;
			bool flag = this.ally != this.Model.CharIsAlly(charId);
			if (!flag)
			{
				bool flag2 = this.ally;
				if (flag2)
				{
					this.interruptButton.gameObject.SetActive(this.Model.CanOperateCharacter(charId));
				}
				this.OnPerformingSkillIdChangedByCharId(charId);
			}
		}

		// Token: 0x06008BF7 RID: 35831 RVA: 0x0040AFCC File Offset: 0x004091CC
		private void InitRefers()
		{
			bool flag = this.ally;
			if (flag)
			{
				PointerTrigger pointerTrigger = this.progressBack.GetComponent<PointerTrigger>();
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.interruptButton.ShowHint();
				});
				pointerTrigger.ExitEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					this.interruptButton.HideHint();
				});
				this.progressBack.GetComponent<CButton>().ClearAndAddListener(delegate
				{
					this.interruptButton.DoInterrupt();
					Action onMainButtonClick = this.OnMainButtonClick;
					if (onMainButtonClick != null)
					{
						onMainButtonClick();
					}
				});
			}
			this._flashPointList.Clear();
			this._flashPointStatus.Clear();
			for (int i = 0; i < this.flashPointParent.transform.childCount; i++)
			{
				GameObject go = this.flashPointParent.transform.GetChild(i).gameObject;
				go.SetActive(false);
				this._flashPointList.Add(go);
				this._flashPointStatus.Add(false);
			}
		}

		// Token: 0x04006B1A RID: 27418
		public Action OnMainButtonClick;

		// Token: 0x04006B1B RID: 27419
		[SerializeField]
		private bool ally;

		// Token: 0x04006B1C RID: 27420
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x04006B1D RID: 27421
		[SerializeField]
		private CImage progress;

		// Token: 0x04006B1E RID: 27422
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x04006B1F RID: 27423
		[SerializeField]
		private CImage frame;

		// Token: 0x04006B20 RID: 27424
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x04006B21 RID: 27425
		[SerializeField]
		private TooltipInvoker progressBack;

		// Token: 0x04006B22 RID: 27426
		[SerializeField]
		private CImage flashProgress;

		// Token: 0x04006B23 RID: 27427
		[SerializeField]
		private GameObject flashPointParent;

		// Token: 0x04006B24 RID: 27428
		[SerializeField]
		private CombatSkillInterruptCasting interruptButton;

		// Token: 0x04006B25 RID: 27429
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};

		// Token: 0x04006B26 RID: 27430
		private CanvasRenderer progressRender;

		// Token: 0x04006B27 RID: 27431
		private static readonly int ShaderPropId_DissolveAmount = Shader.PropertyToID("_DissolveAmount");

		// Token: 0x04006B28 RID: 27432
		private readonly List<GameObject> _flashPointList = new List<GameObject>();

		// Token: 0x04006B29 RID: 27433
		private readonly List<bool> _flashPointStatus = new List<bool>();

		// Token: 0x04006B2A RID: 27434
		private const float FlashPerAmount = 0.25f;
	}
}
