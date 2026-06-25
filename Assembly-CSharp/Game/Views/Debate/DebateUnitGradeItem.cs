using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Debate
{
	// Token: 0x02000AA4 RID: 2724
	public class DebateUnitGradeItem : MonoBehaviour
	{
		// Token: 0x17000EB9 RID: 3769
		// (get) Token: 0x06008596 RID: 34198 RVA: 0x003E0822 File Offset: 0x003DEA22
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x17000EBA RID: 3770
		// (get) Token: 0x06008597 RID: 34199 RVA: 0x003E0829 File Offset: 0x003DEA29
		public PointerTrigger PointerTrigger
		{
			get
			{
				return this.pointerTrigger;
			}
		}

		// Token: 0x06008598 RID: 34200 RVA: 0x003E0831 File Offset: 0x003DEA31
		public void Init(sbyte grade, Vector3 angel)
		{
			base.transform.localEulerAngles = angel;
			this.textGrade.text = grade.ToString();
			this.textGrade.rectTransform.localEulerAngles = -angel;
		}

		// Token: 0x06008599 RID: 34201 RVA: 0x003E086C File Offset: 0x003DEA6C
		public void Refresh(sbyte grade, Action<int, sbyte> onEnter, Action onExit, Action onClick)
		{
			int cost = this.Model.DebateGame.GetPawnGradeToBase(this.Model.DebateGame.PlayerLeft.MaxBases, (int)grade);
			cost = Mathf.Max(0, cost);
			bool costIsMeet = this.Model.DebateGame.PlayerLeft.Bases >= cost;
			this.button.interactable = costIsMeet;
			Sprite spriteNormal = this.GetGradeUnitImage((int)grade, this.button.interactable, true);
			this.image.sprite = spriteNormal;
			Sprite spriteHighlight = this.GetGradeUnitImage((int)grade, this.button.interactable, false);
			SpriteState spriteState = this.button.spriteState;
			spriteState.highlightedSprite = spriteHighlight;
			this.button.spriteState = spriteState;
			this.tip.Type = TipType.SingleDesc;
			string[] presetParam = this.tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 1;
			if (flag)
			{
				this.tip.PresetParam = new string[1];
			}
			string color = costIsMeet ? "brightblue" : "brightred";
			string curValue = this.Model.DebateGame.PlayerLeft.Bases.ToString().SetColor(color);
			this.tip.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_CostEnergy_Tip, curValue, cost);
			PointerTrigger pointerTrigger = this.pointerTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				onEnter(cost, grade);
				this.gameObject.transform.SetAsLastSibling();
			});
			pointerTrigger = this.pointerTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				onExit();
			});
			this.button.ClearAndAddListener(onClick);
		}

		// Token: 0x0600859A RID: 34202 RVA: 0x003E0AB0 File Offset: 0x003DECB0
		private Sprite GetGradeUnitImage(int index, bool interactable, bool isNormal)
		{
			return interactable ? (isNormal ? this.spriteNormalArray[index] : this.spriteHighlightArray[index]) : this.spriteDisable;
		}

		// Token: 0x04006673 RID: 26227
		[SerializeField]
		private CButton button;

		// Token: 0x04006674 RID: 26228
		[SerializeField]
		private CImage image;

		// Token: 0x04006675 RID: 26229
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04006676 RID: 26230
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04006677 RID: 26231
		[SerializeField]
		private TextMeshProUGUI textGrade;

		// Token: 0x04006678 RID: 26232
		[SerializeField]
		private Sprite[] spriteNormalArray;

		// Token: 0x04006679 RID: 26233
		[SerializeField]
		private Sprite[] spriteHighlightArray;

		// Token: 0x0400667A RID: 26234
		[SerializeField]
		private Sprite spriteDisable;
	}
}
