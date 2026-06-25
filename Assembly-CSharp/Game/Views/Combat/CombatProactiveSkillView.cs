using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AEA RID: 2794
	public class CombatProactiveSkillView : MonoBehaviour
	{
		// Token: 0x17000F2C RID: 3884
		// (get) Token: 0x0600894F RID: 35151 RVA: 0x003F8805 File Offset: 0x003F6A05
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008950 RID: 35152 RVA: 0x003F880C File Offset: 0x003F6A0C
		public void SetData(CombatSkillDisplayData skillData, Action onClick, Action onJumpSettingClick, Action onEnterProactiveSkillView, Action onExitProactiveSkillView)
		{
			this._skillData = skillData;
			this.UpdateMouseTip();
			CombatSkillItem configData = CombatSkill.Instance[skillData.TemplateId];
			this.txtName.text = configData.Name.SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
			this.skillIcon.SetSprite(configData.Icon, false, null);
			this.skillIcon.SetColor(CommonUtils.GetFiveElementColor((int)configData.FiveElements));
			this.frame.SetSprite(CombatProactiveSkillView.FramePaths[(int)configData.EquipType] + configData.Grade.ToString(), false, null);
			this.fiveElementsFrame.gameObject.SetActive(false);
			this.revoked.SetActive(skillData.Revoked);
			this.grayRoot.SetStyleEffect(false, false);
			this.disableMask.SetActive(false);
			this.GetHolderComponent<CButton>().ClearAndAddListener(onClick);
			this.SetChecked(false);
			this.mouseOver.gameObject.SetActive(false);
			this.jumpSetting.gameObject.SetActive(skillData.JumpThreshold > 0);
			bool flag = onJumpSettingClick != null;
			if (flag)
			{
				this.jumpSetting.ClearAndAddListener(onJumpSettingClick);
			}
			this.UpdateSkillCdAndLock(skillData.TemplateId);
			this.SetBodyPartBroken();
			PointerTrigger pointerTrigger = this.GetHolderComponent<PointerTrigger>();
			Sequence nameSequence = this._nameSequence;
			if (nameSequence != null)
			{
				nameSequence.Kill(false);
			}
			this._nameSequence = null;
			this.txtName.rectTransform.anchoredPosition = Vector2.zero;
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				onEnterProactiveSkillView();
			});
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				onExitProactiveSkillView();
			});
		}

		// Token: 0x06008951 RID: 35153 RVA: 0x003F89F8 File Offset: 0x003F6BF8
		public void UpdateSkillCdAndLock(short skillId)
		{
			CombatSkillKey combatSkillKey = new CombatSkillKey(this.Model.SelfCharId, skillId);
			CombatSubProcessorSkill combatSkillProcessor;
			bool flag = !this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out combatSkillProcessor);
			if (!flag)
			{
				short leftFrame = combatSkillProcessor.LeftCdFrame;
				short totalFrame = combatSkillProcessor.TotalCdFrame;
				bool locked = totalFrame < 0;
				bool isLastFrameLocked = this._isSkillLockedLastFrame;
				this.lockProgress.fillAmount = (locked ? 1f : ((totalFrame > 0) ? ((float)leftFrame / (float)totalFrame) : 0f));
				string countDownTextStr = CombatUtils.StyleCountDownText((float)leftFrame / 60f, locked);
				this._isSkillLockedLastFrame = (locked || (totalFrame > 0 && leftFrame > 0));
				this.countDownText.gameObject.SetActive(this._isSkillLockedLastFrame);
				this.countDownText.SetText(countDownTextStr.SetColor("red"), true);
				bool flag2 = isLastFrameLocked && !this._isSkillLockedLastFrame;
				if (flag2)
				{
					this.lockFinishParticle.gameObject.SetActive(true);
					this.lockFinishParticle.StartDelay();
				}
			}
		}

		// Token: 0x06008952 RID: 35154 RVA: 0x003F8B0C File Offset: 0x003F6D0C
		public void SetBodyPartBroken()
		{
			bool flag = this._skillData == null;
			if (!flag)
			{
				CombatSkillKey combatSkillKey = new CombatSkillKey(this.Model.SelfCharId, this._skillData.TemplateId);
				CombatSubProcessorSkill combatSkillProcessor;
				bool flag2 = !this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out combatSkillProcessor);
				if (!flag2)
				{
					IReadOnlyList<CombatSkillBanReasonData> banReasons = combatSkillProcessor.BanReason;
					bool flag3 = banReasons == null;
					if (!flag3)
					{
						bool bodyPartBroken = banReasons.Any((CombatSkillBanReasonData x) => x.Type == ECombatSkillBanReasonType.BodyPartBroken);
						GameObject brokenRoot = this.bodyPartBrokenPartRoot;
						this.bodyPartBrokenGo.SetActive(bodyPartBroken);
						bool flag4 = !bodyPartBroken;
						if (!flag4)
						{
							List<sbyte> needBodyPartTypes = CombatSkill.Instance[this._skillData.TemplateId].NeedBodyPartTypes;
							for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
							{
								bool anyContains = false;
								foreach (sbyte needBodyPartType in needBodyPartTypes)
								{
									bool flag5 = NeedBodyPartType.Contains(needBodyPartType, bodyPart);
									if (flag5)
									{
										anyContains = true;
										break;
									}
								}
								brokenRoot.transform.GetChild((int)bodyPart).gameObject.SetActive(anyContains);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008953 RID: 35155 RVA: 0x003F8C78 File Offset: 0x003F6E78
		public void OnMouseEnter()
		{
			bool flag = this._interactable && !this.checkMark.activeSelf;
			if (flag)
			{
				this.mouseOver.gameObject.SetActive(true);
			}
		}

		// Token: 0x06008954 RID: 35156 RVA: 0x003F8CB8 File Offset: 0x003F6EB8
		public void OnMouseExit()
		{
			bool interactable = this._interactable;
			if (interactable)
			{
				this.mouseOver.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008955 RID: 35157 RVA: 0x003F8CE4 File Offset: 0x003F6EE4
		public void SetInteractable(bool interactable)
		{
			CButton btn = this.GetHolderComponent<CButton>();
			this._interactable = interactable;
			bool flag = btn != null;
			if (flag)
			{
				btn.interactable = interactable;
			}
			bool flag2 = !interactable;
			if (flag2)
			{
				this.mouseOver.gameObject.SetActive(false);
			}
			this.grayRoot.SetStyleEffect(!interactable, false);
			this.disableMask.SetActive(!interactable);
		}

		// Token: 0x06008956 RID: 35158 RVA: 0x003F8D50 File Offset: 0x003F6F50
		public void UpdateMouseTip()
		{
			TooltipInvoker mouseTip = this.GetHolderComponent<TooltipInvoker>();
			mouseTip.enabled = (this._skillData != null);
			bool flag = this._skillData == null;
			if (!flag)
			{
				mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", this._skillData.TemplateId).Set("CharId", this._skillData.CharId);
				bool showing = mouseTip.Showing;
				if (showing)
				{
					mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x06008957 RID: 35159 RVA: 0x003F8DCD File Offset: 0x003F6FCD
		private void SetChecked(bool check)
		{
			this.checkMark.SetActive(check);
		}

		// Token: 0x06008958 RID: 35160 RVA: 0x003F8DE0 File Offset: 0x003F6FE0
		public T GetHolderComponent<T>() where T : Component
		{
			CImage viewHolder = this.combatSkillViewHolder;
			return viewHolder.GetComponent<T>();
		}

		// Token: 0x06008959 RID: 35161 RVA: 0x003F8DFF File Offset: 0x003F6FFF
		private void OnDisable()
		{
			this._isSkillLockedLastFrame = false;
			this.lockFinishParticle.gameObject.SetActive(false);
		}

		// Token: 0x04006932 RID: 26930
		[SerializeField]
		public CButton jumpSetting;

		// Token: 0x04006933 RID: 26931
		[SerializeField]
		private CButton masterActiveStatus;

		// Token: 0x04006934 RID: 26932
		[SerializeField]
		private CButton masterInactiveStatus;

		// Token: 0x04006935 RID: 26933
		[SerializeField]
		private CImage lockProgress;

		// Token: 0x04006936 RID: 26934
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x04006937 RID: 26935
		[SerializeField]
		private DisableStyleRoot grayRoot;

		// Token: 0x04006938 RID: 26936
		[SerializeField]
		private GameObject bodyPartBrokenGo;

		// Token: 0x04006939 RID: 26937
		[SerializeField]
		private GameObject cdIcon;

		// Token: 0x0400693A RID: 26938
		[SerializeField]
		private GameObject checkMark;

		// Token: 0x0400693B RID: 26939
		[SerializeField]
		private GameObject disableMask;

		// Token: 0x0400693C RID: 26940
		[SerializeField]
		private GameObject elementsRoot;

		// Token: 0x0400693D RID: 26941
		[SerializeField]
		private GameObject lockGo;

		// Token: 0x0400693E RID: 26942
		[SerializeField]
		private GameObject master;

		// Token: 0x0400693F RID: 26943
		[SerializeField]
		private GameObject mouseOver;

		// Token: 0x04006940 RID: 26944
		[SerializeField]
		private GameObject prevMark;

		// Token: 0x04006941 RID: 26945
		[SerializeField]
		private GameObject revoked;

		// Token: 0x04006942 RID: 26946
		[SerializeField]
		public ParticleSystem particle;

		// Token: 0x04006943 RID: 26947
		[SerializeField]
		private RectTransform nameBack;

		// Token: 0x04006944 RID: 26948
		[SerializeField]
		private TextMeshProUGUI countDownText;

		// Token: 0x04006945 RID: 26949
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x04006946 RID: 26950
		[SerializeField]
		private CImage combatSkillViewHolder;

		// Token: 0x04006947 RID: 26951
		[SerializeField]
		private CImage frame;

		// Token: 0x04006948 RID: 26952
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x04006949 RID: 26953
		[SerializeField]
		private DelayCaller lockFinishParticle;

		// Token: 0x0400694A RID: 26954
		[SerializeField]
		private GameObject bodyPartBrokenPartRoot;

		// Token: 0x0400694B RID: 26955
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};

		// Token: 0x0400694C RID: 26956
		private CombatSkillDisplayData _skillData;

		// Token: 0x0400694D RID: 26957
		private bool _interactable;

		// Token: 0x0400694E RID: 26958
		private Sequence _nameSequence;

		// Token: 0x0400694F RID: 26959
		private bool _isSkillLockedLastFrame = false;
	}
}
