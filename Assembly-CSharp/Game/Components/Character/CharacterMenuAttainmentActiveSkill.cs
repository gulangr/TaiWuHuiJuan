using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F46 RID: 3910
	public class CharacterMenuAttainmentActiveSkill : MonoBehaviour
	{
		// Token: 0x0600B379 RID: 45945 RVA: 0x0051AE29 File Offset: 0x00519029
		public void SetPanelType(bool isCombatSkill)
		{
			this._isCombatSkillPanel = isCombatSkill;
		}

		// Token: 0x0600B37A RID: 45946 RVA: 0x0051AE34 File Offset: 0x00519034
		private void OnDestroy()
		{
			bool flag = this._normalIconMat != null;
			if (flag)
			{
				Object.Destroy(this._normalIconMat);
			}
			this._normalIconMat = null;
		}

		// Token: 0x0600B37B RID: 45947 RVA: 0x0051AE68 File Offset: 0x00519068
		public void ShowUnlockMask()
		{
			bool flag = this.layoutNormal != null;
			if (flag)
			{
				this.layoutNormal.SetActive(false);
			}
			bool flag2 = this.layoutNone != null;
			if (flag2)
			{
				this.layoutNone.SetActive(false);
			}
			bool flag3 = this.layoutMask != null;
			if (flag3)
			{
				this.layoutMask.SetActive(true);
			}
		}

		// Token: 0x0600B37C RID: 45948 RVA: 0x0051AED0 File Offset: 0x005190D0
		public void HideUnlockMaskOnly()
		{
			bool flag = this.layoutMask != null;
			if (flag)
			{
				this.layoutMask.SetActive(false);
			}
		}

		// Token: 0x0600B37D RID: 45949 RVA: 0x0051AEFC File Offset: 0x005190FC
		public void FinishUnlockEffect(bool showNormalLayout)
		{
			bool flag = this.layoutMask != null;
			if (flag)
			{
				this.layoutMask.SetActive(false);
			}
			bool flag2 = this.layoutNormal != null;
			if (flag2)
			{
				this.layoutNormal.SetActive(showNormalLayout);
			}
			bool flag3 = this.layoutNone != null;
			if (flag3)
			{
				this.layoutNone.SetActive(!showNormalLayout);
			}
		}

		// Token: 0x0600B37E RID: 45950 RVA: 0x0051AF64 File Offset: 0x00519164
		public void HideUnlockMask()
		{
			this.HideUnlockMaskOnly();
		}

		// Token: 0x0600B37F RID: 45951 RVA: 0x0051AF70 File Offset: 0x00519170
		public void Set(CharacterMenuAttainmentActiveSkillData data, int slotGrade, bool isAnim = false)
		{
			bool isEmpty = data == null;
			this.layoutNormal.SetActive(!isEmpty);
			this.layoutNone.SetActive(isEmpty);
			this.ResetAnim();
			this.mouseTip.enabled = false;
			bool flag = isEmpty;
			if (flag)
			{
				this.RefreshLockedSlot(slotGrade);
			}
			else
			{
				this.mouseTip.enabled = true;
				bool flag2 = this._normalIconMat == null;
				if (flag2)
				{
					this.InitMat();
				}
				this.normalGradeIcon.material.SetTexture(CharacterMenuAttainmentActiveSkill.MainTex, this.gradeIcon[data.Grade]);
				this.ResetGradeIconDissolve();
				this.normalGradeIcon.color = Color.white;
				this.icon.SetSprite(data.Icon, false, null);
				bool isCombatSkill = data.IsCombatSkill;
				if (isCombatSkill)
				{
					CombatSkillItem combatConfig = CombatSkill.Instance[data.SkillTemplateId];
					bool flag3 = this.frame != null;
					if (flag3)
					{
						this.frame.gameObject.SetActive(true);
						this.frame.SetSprite(CharacterMenuAttainmentActiveSkill.FramePaths[(int)combatConfig.EquipType] + combatConfig.Grade.ToString(), false, null);
					}
					bool flag4 = this.fiveElementsFrame != null;
					if (flag4)
					{
						this.fiveElementsFrame.gameObject.SetActive(true);
						this.fiveElementsFrame.SetSprite("ui9_back_combat_skill_five_elements_frame_" + combatConfig.EquipType.ToString(), false, null);
						this.fiveElementsFrame.color = CommonUtils.GetFiveElementColor((int)combatConfig.FiveElements);
					}
				}
				else
				{
					bool flag5 = this.frame != null;
					if (flag5)
					{
						this.frame.gameObject.SetActive(false);
					}
					bool flag6 = this.fiveElementsFrame != null;
					if (flag6)
					{
						this.fiveElementsFrame.gameObject.SetActive(false);
					}
				}
				this.bookName.text = data.SkillName.SetGradeColor(data.Grade);
				this.addValue.text = string.Format("+{0}%", GlobalConfig.Instance.AddAttainmentPerGrade[data.Grade]);
				this.layoutBottom.gameObject.SetActive(true);
				bool flag7 = this.icon != null;
				if (flag7)
				{
					this.icon.gameObject.SetActive(true);
				}
				this.lockObj.SetActive(data.Locked);
				this.addValue.text = string.Format("{0}%", data.AddValue);
				bool isCombatSkill2 = data.IsCombatSkill;
				if (isCombatSkill2)
				{
					this.mouseTip.Type = TipType.CombatSkill;
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.Set("CombatSkillId", data.SkillTemplateId).Set("CharId", data.CharId).Set("IsSimple", true);
				}
				else
				{
					short skillbookId = LifeSkill.Instance[data.SkillTemplateId].SkillBookId;
					this.mouseTip.Type = TipType.LifeSkillDetailReadProgress;
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyBookConfig, SkillBook.Instance[skillbookId]).SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyReadProgresses, (data.ReadingProgress == null) ? CharacterMenuAttainmentActiveSkill._lifeSkillProgress : data.ReadingProgress);
				}
				bool flag8 = !isAnim;
				if (!flag8)
				{
					this.waveEffList[data.Grade].SetActive(true);
					this.layoutBottom.alpha = 0f;
					this.layoutBottom.DOFade(1f, 0.35f);
					UnityEngine.Material mat = this.normalGradeIcon.material;
					mat.SetTexture(CharacterMenuAttainmentActiveSkill.MainTex, this.gradeIcon[data.Grade]);
					mat.SetFloat(CharacterMenuAttainmentActiveSkill.DissolveProperty, 1f);
					mat.SetColor(CharacterMenuAttainmentActiveSkill.EdgeColorProperty, CharacterMenuAttainmentActiveSkill.DissolveEdgeColorList[data.Grade].HexStringToColor());
					this._dissolveTweener = DOTween.To(() => mat.GetFloat(CharacterMenuAttainmentActiveSkill.DissolveProperty), delegate(float x)
					{
						mat.SetFloat(CharacterMenuAttainmentActiveSkill.DissolveProperty, x);
					}, -0.1f, 0.35f).SetEase(Ease.OutQuart);
				}
			}
		}

		// Token: 0x0600B380 RID: 45952 RVA: 0x0051B40C File Offset: 0x0051960C
		private void RefreshLockedSlot(int slotGrade)
		{
			Transform labelRoot = (this.emptyValue != null) ? this.emptyValue.transform.parent : null;
			bool flag = labelRoot != null;
			if (flag)
			{
				labelRoot.gameObject.SetActive(true);
			}
			bool flag2 = this.emptyValue == null;
			if (!flag2)
			{
				this.emptyValue.gameObject.SetActive(true);
				string gradeNum = CharacterMenuAttainmentActiveSkill.GetUnlockRequiredGradeNumText(slotGrade);
				this.emptyValue.text = (this._isCombatSkillPanel ? LanguageKey.LK_AttainmentItems_Lock_Skill.TrFormat(gradeNum) : LanguageKey.LK_AttainmentItems_Lock_Book.TrFormat(gradeNum));
			}
		}

		// Token: 0x0600B381 RID: 45953 RVA: 0x0051B4AC File Offset: 0x005196AC
		private static string GetUnlockRequiredGradeNumText(int slotGrade)
		{
			int numKey = 9 - slotGrade;
			bool flag = numKey <= 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = LocalStringManager.Get(string.Format("LK_Num_{0}", numKey));
			}
			return result;
		}

		// Token: 0x0600B382 RID: 45954 RVA: 0x0051B4EC File Offset: 0x005196EC
		private void ResetAnim()
		{
			foreach (GameObject go in this.waveEffList)
			{
				go.SetActive(false);
			}
			this.layoutBottom.DOKill(true);
			this.layoutBottom.alpha = 1f;
			Tweener dissolveTweener = this._dissolveTweener;
			if (dissolveTweener != null)
			{
				dissolveTweener.Kill(true);
			}
			this._dissolveTweener = null;
			this.ResetGradeIconDissolve();
		}

		// Token: 0x0600B383 RID: 45955 RVA: 0x0051B584 File Offset: 0x00519784
		private void ResetGradeIconDissolve()
		{
			bool flag = this._normalIconMat != null;
			if (flag)
			{
				this._normalIconMat.SetFloat(CharacterMenuAttainmentActiveSkill.DissolveProperty, -0.1f);
			}
		}

		// Token: 0x0600B384 RID: 45956 RVA: 0x0051B5B8 File Offset: 0x005197B8
		private void InitMat()
		{
			bool flag = this._normalIconMat != null;
			if (!flag)
			{
				this._normalIconMat = new UnityEngine.Material(this.normalGradeIcon.material);
				this.normalGradeIcon.material = this._normalIconMat;
				this.normalGradeIcon.sprite = null;
			}
		}

		// Token: 0x04008B64 RID: 35684
		public TooltipInvoker mouseTip;

		// Token: 0x04008B65 RID: 35685
		public CImage normalGradeIcon;

		// Token: 0x04008B66 RID: 35686
		public CImage icon;

		// Token: 0x04008B67 RID: 35687
		public CImage frame;

		// Token: 0x04008B68 RID: 35688
		public CImage fiveElementsFrame;

		// Token: 0x04008B69 RID: 35689
		public TextMeshProUGUI bookName;

		// Token: 0x04008B6A RID: 35690
		public TextMeshProUGUI addValue;

		// Token: 0x04008B6B RID: 35691
		public TextMeshProUGUI emptyValue;

		// Token: 0x04008B6C RID: 35692
		public GameObject layoutMask;

		// Token: 0x04008B6D RID: 35693
		public GameObject layoutNormal;

		// Token: 0x04008B6E RID: 35694
		public GameObject layoutNone;

		// Token: 0x04008B6F RID: 35695
		public GameObject lockObj;

		// Token: 0x04008B70 RID: 35696
		public CanvasGroup layoutBottom;

		// Token: 0x04008B71 RID: 35697
		public List<GameObject> waveEffList;

		// Token: 0x04008B72 RID: 35698
		public List<Texture2D> gradeIcon;

		// Token: 0x04008B73 RID: 35699
		private static sbyte[] _lifeSkillProgress = new sbyte[]
		{
			100,
			100,
			100,
			100,
			100
		};

		// Token: 0x04008B74 RID: 35700
		private static readonly int DissolveProperty = Shader.PropertyToID("_rongjie");

		// Token: 0x04008B75 RID: 35701
		private static readonly int MainTex = Shader.PropertyToID("_MainTex");

		// Token: 0x04008B76 RID: 35702
		private static readonly int EdgeColorProperty = Shader.PropertyToID("_bianyuanyanse");

		// Token: 0x04008B77 RID: 35703
		private static readonly List<string> DissolveEdgeColorList = new List<string>
		{
			"#80817f",
			"#ffffff",
			"#9eb767",
			"#509296",
			"#5d92c3",
			"#84448d",
			"#be8a2f",
			"#ce5d1f",
			"#d43f38"
		};

		// Token: 0x04008B78 RID: 35704
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};

		// Token: 0x04008B79 RID: 35705
		private Tweener _dissolveTweener;

		// Token: 0x04008B7A RID: 35706
		private UnityEngine.Material _normalIconMat;

		// Token: 0x04008B7B RID: 35707
		private bool _isCombatSkillPanel;
	}
}
