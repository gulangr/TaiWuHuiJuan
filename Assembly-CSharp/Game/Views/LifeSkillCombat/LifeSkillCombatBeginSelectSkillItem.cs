using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.LifeSkillCombat
{
	// Token: 0x02000984 RID: 2436
	public class LifeSkillCombatBeginSelectSkillItem : MonoBehaviour
	{
		// Token: 0x17000D33 RID: 3379
		// (get) Token: 0x06007513 RID: 29971 RVA: 0x00368C9E File Offset: 0x00366E9E
		private ViewLifeSkillCombatBegin Parent
		{
			get
			{
				return UIElement.LifeSkillCombatBegin.UiBaseAs<ViewLifeSkillCombatBegin>();
			}
		}

		// Token: 0x06007514 RID: 29972 RVA: 0x00368CAC File Offset: 0x00366EAC
		private void Awake()
		{
			this.skillBtn.ClearAndAddListener(delegate
			{
				this.Parent.OnSelectSkillItemClicked(this._index);
			});
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(this.skillBtn.interactable);
				bool flag = this._selectSkillType == ESelectSkillType.Normal;
				if (flag)
				{
					this.Parent.ShowAttainment(this._skillId);
				}
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
				this.Parent.ShowAttainment(-1);
			});
		}

		// Token: 0x06007515 RID: 29973 RVA: 0x00368D0C File Offset: 0x00366F0C
		public void Set(sbyte index, sbyte skillId, ESelectSkillType selectSkillType, bool isGreater, int value, int cost, bool canInteract, bool isObserved = false)
		{
			this._index = index;
			this._skillId = skillId;
			this._selectSkillType = selectSkillType;
			this.skillIcon.SetSprite(string.Format("{0}{1}_{2}", "ui9_icon_life_skill_combat_select_skill_icon_", isObserved ? 1 : 0, this._skillId), false, null);
			TMP_Text tmp_Text = this.skillName;
			ESelectSkillType selectSkillType2 = this._selectSkillType;
			tmp_Text.text = ((selectSkillType2 == ESelectSkillType.UnKnown || selectSkillType2 == ESelectSkillType.Observe) ? LanguageKey.UI_LifeSkillBattle_Prepare_Unknown.Tr() : LifeSkillType.Instance[this._skillId].Name);
			this.compareIcon.gameObject.SetActive(this._selectSkillType == ESelectSkillType.Normal);
			this.compareIcon.SetSprite(isGreater ? "ui9_icon_property_inc" : "ui9_icon_property_dec", false, null);
			this.compareValue.gameObject.SetActive(this._selectSkillType == ESelectSkillType.Normal);
			this.compareValue.text = value.ToString().SetColor(isGreater ? "brightblue" : "brightred");
			this.wisdomCost.text = cost.ToString();
			this.hover.gameObject.SetActive(false);
			(base.transform as RectTransform).sizeDelta = new Vector2((base.transform as RectTransform).sizeDelta.x, (float)((this._selectSkillType == ESelectSkillType.Observe) ? 170 : 124));
			Selectable selectable = this.skillBtn;
			bool interactable;
			if (this._selectSkillType != ESelectSkillType.Observe || !canInteract)
			{
				selectSkillType2 = this._selectSkillType;
				interactable = ((selectSkillType2 == ESelectSkillType.Normal || selectSkillType2 <= ESelectSkillType.UnKnown) && canInteract);
			}
			else
			{
				interactable = true;
			}
			selectable.interactable = interactable;
			GameObject gameObject = this.UnknownIcon.gameObject;
			selectSkillType2 = this._selectSkillType;
			gameObject.SetActive(selectSkillType2 == ESelectSkillType.UnKnown || selectSkillType2 == ESelectSkillType.Observe);
			this.wisdomCost.transform.parent.gameObject.SetActive(this._selectSkillType == ESelectSkillType.Observe);
			GameObject gameObject2 = this.skillIcon.gameObject;
			selectSkillType2 = this._selectSkillType;
			gameObject2.SetActive(selectSkillType2 != ESelectSkillType.UnKnown && selectSkillType2 != ESelectSkillType.Observe);
			this.BannedMask.gameObject.SetActive(this._selectSkillType == ESelectSkillType.Banned);
		}

		// Token: 0x040057C8 RID: 22472
		[SerializeField]
		private CButton skillBtn;

		// Token: 0x040057C9 RID: 22473
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x040057CA RID: 22474
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x040057CB RID: 22475
		[SerializeField]
		private CImage compareIcon;

		// Token: 0x040057CC RID: 22476
		[SerializeField]
		private TextMeshProUGUI compareValue;

		// Token: 0x040057CD RID: 22477
		[SerializeField]
		private TextMeshProUGUI wisdomCost;

		// Token: 0x040057CE RID: 22478
		[SerializeField]
		private CImage hover;

		// Token: 0x040057CF RID: 22479
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x040057D0 RID: 22480
		[SerializeField]
		private CImage UnknownIcon;

		// Token: 0x040057D1 RID: 22481
		[SerializeField]
		private CImage BannedMask;

		// Token: 0x040057D2 RID: 22482
		private ESelectSkillType _selectSkillType = ESelectSkillType.UnKnown;

		// Token: 0x040057D3 RID: 22483
		private sbyte _skillId;

		// Token: 0x040057D4 RID: 22484
		private sbyte _index;

		// Token: 0x040057D5 RID: 22485
		private const int NormalHeight = 124;

		// Token: 0x040057D6 RID: 22486
		private const int Observr = 170;
	}
}
