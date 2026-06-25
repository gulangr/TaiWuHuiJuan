using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Character
{
	// Token: 0x02000F18 RID: 3864
	public class CharacterAttainmentOverviewItem : MonoBehaviour
	{
		// Token: 0x0600B1FE RID: 45566 RVA: 0x00510D74 File Offset: 0x0050EF74
		private void Awake()
		{
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				this.attainmentMouseTips.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = this.attainmentMouseTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this._pointerTrigger = base.GetComponent<PointerTrigger>();
				bool flag2 = this._pointerTrigger != null;
				if (flag2)
				{
					this._pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
					this._pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
				}
				this._currentVisualState = CharacterAttainmentOverviewItem.VisualState.Normal;
				this.RefreshVisuals(true);
			}
		}

		// Token: 0x0600B1FF RID: 45567 RVA: 0x00510E24 File Offset: 0x0050F024
		private void OnDestroy()
		{
			bool flag = this._pointerTrigger != null;
			if (flag)
			{
				this._pointerTrigger.EnterEvent.RemoveListener(new UnityAction(this.OnPointerEnter));
				this._pointerTrigger.ExitEvent.RemoveListener(new UnityAction(this.OnPointerExit));
			}
		}

		// Token: 0x0600B200 RID: 45568 RVA: 0x00510E7E File Offset: 0x0050F07E
		private void OnPointerEnter()
		{
			this._isHover = true;
			this.RefreshVisuals(false);
		}

		// Token: 0x0600B201 RID: 45569 RVA: 0x00510E90 File Offset: 0x0050F090
		private void OnPointerExit()
		{
			this._isHover = false;
			this.RefreshVisuals(false);
		}

		// Token: 0x0600B202 RID: 45570 RVA: 0x00510EA4 File Offset: 0x0050F0A4
		private CharacterAttainmentOverviewItem.VisualState GetVisualState()
		{
			bool isHover = this._isHover;
			CharacterAttainmentOverviewItem.VisualState result;
			if (isHover)
			{
				result = CharacterAttainmentOverviewItem.VisualState.Hover;
			}
			else
			{
				bool isParentHover = this._isParentHover;
				if (isParentHover)
				{
					result = CharacterAttainmentOverviewItem.VisualState.ParentHover;
				}
				else
				{
					result = CharacterAttainmentOverviewItem.VisualState.Normal;
				}
			}
			return result;
		}

		// Token: 0x0600B203 RID: 45571 RVA: 0x00510ED4 File Offset: 0x0050F0D4
		private void RefreshVisuals(bool force)
		{
			CharacterAttainmentOverviewItem.VisualState newState = this.GetVisualState();
			bool flag = !force && newState == this._currentVisualState;
			if (!flag)
			{
				this._currentVisualState = newState;
				this.ApplyTextColors(newState);
				this.ApplyIconSprite(newState);
				this.ApplyOtherSprites(newState);
			}
		}

		// Token: 0x0600B204 RID: 45572 RVA: 0x00510F20 File Offset: 0x0050F120
		private void ApplyTextColors(CharacterAttainmentOverviewItem.VisualState state)
		{
			bool flag = this.txtSkillName != null;
			if (flag)
			{
				TextMeshProUGUI textMeshProUGUI = this.txtSkillName;
				if (!true)
				{
				}
				Color color;
				if (state != CharacterAttainmentOverviewItem.VisualState.Hover)
				{
					if (state != CharacterAttainmentOverviewItem.VisualState.ParentHover)
					{
						color = this.skillNameNormalColor;
					}
					else
					{
						color = this.skillNameParentHoverColor;
					}
				}
				else
				{
					color = this.skillNameHoverColor;
				}
				if (!true)
				{
				}
				textMeshProUGUI.color = color;
			}
			bool flag2 = this.txtQualification != null;
			if (flag2)
			{
				TextMeshProUGUI textMeshProUGUI2 = this.txtQualification;
				if (!true)
				{
				}
				Color color;
				if (state != CharacterAttainmentOverviewItem.VisualState.Hover)
				{
					if (state != CharacterAttainmentOverviewItem.VisualState.ParentHover)
					{
						color = this.qualificationNormalColor;
					}
					else
					{
						color = this.qualificationParentHoverColor;
					}
				}
				else
				{
					color = this.qualificationHoverColor;
				}
				if (!true)
				{
				}
				textMeshProUGUI2.color = color;
			}
			bool flag3 = this.txtAttainment != null;
			if (flag3)
			{
				TextMeshProUGUI textMeshProUGUI3 = this.txtAttainment;
				if (!true)
				{
				}
				Color color;
				if (state != CharacterAttainmentOverviewItem.VisualState.Hover)
				{
					if (state != CharacterAttainmentOverviewItem.VisualState.ParentHover)
					{
						color = this.attainmentNormalColor;
					}
					else
					{
						color = this.attainmentParentHoverColor;
					}
				}
				else
				{
					color = this.attainmentHoverColor;
				}
				if (!true)
				{
				}
				textMeshProUGUI3.color = color;
			}
		}

		// Token: 0x0600B205 RID: 45573 RVA: 0x00511020 File Offset: 0x0050F220
		private void ApplyIconSprite(CharacterAttainmentOverviewItem.VisualState state)
		{
			bool flag = this.imgIcon == null;
			if (!flag)
			{
				if (!true)
				{
				}
				string text;
				if (state != CharacterAttainmentOverviewItem.VisualState.Hover)
				{
					if (state != CharacterAttainmentOverviewItem.VisualState.ParentHover)
					{
						text = this._iconNormalSpriteName;
					}
					else
					{
						text = this._iconParentHoverSpriteName;
					}
				}
				else
				{
					text = this._iconHoverSpriteName;
				}
				if (!true)
				{
				}
				string spriteName = text;
				this.imgIcon.SetSprite(spriteName, false, null);
			}
		}

		// Token: 0x0600B206 RID: 45574 RVA: 0x00511080 File Offset: 0x0050F280
		private void ApplyOtherSprites(CharacterAttainmentOverviewItem.VisualState state)
		{
			bool flag = this.stateSpriteBindings == null;
			if (!flag)
			{
				for (int i = 0; i < this.stateSpriteBindings.Length; i++)
				{
					CharacterAttainmentOverviewItem.StateSpriteBinding binding = this.stateSpriteBindings[i];
					bool flag2 = binding.Image == null;
					if (!flag2)
					{
						if (!true)
						{
						}
						Sprite sprite2;
						if (state != CharacterAttainmentOverviewItem.VisualState.Hover)
						{
							if (state != CharacterAttainmentOverviewItem.VisualState.ParentHover)
							{
								sprite2 = binding.NormalSprite;
							}
							else
							{
								sprite2 = binding.ParentHoverSprite;
							}
						}
						else
						{
							sprite2 = binding.HoverSprite;
						}
						if (!true)
						{
						}
						Sprite sprite = sprite2;
						binding.Image.sprite = sprite;
						binding.Image.SetEnabled(sprite != null);
					}
				}
			}
		}

		// Token: 0x0600B207 RID: 45575 RVA: 0x00511134 File Offset: 0x0050F334
		public void SetParentHover(bool hover)
		{
			this._isParentHover = hover;
			this.RefreshVisuals(false);
		}

		// Token: 0x0600B208 RID: 45576 RVA: 0x00511148 File Offset: 0x0050F348
		public void SetTitle(string title)
		{
			this.txtSkillName.text = title;
			TooltipInvoker tooltipInvoker = this.attainmentMouseTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.attainmentMouseTips.RuntimeParam.Set("arg0", title);
		}

		// Token: 0x0600B209 RID: 45577 RVA: 0x00511197 File Offset: 0x0050F397
		public void SetAttainment(string content)
		{
			this.txtAttainment.text = content;
			this._attainmentContent = content;
			this.RefreshTipsArg2();
		}

		// Token: 0x0600B20A RID: 45578 RVA: 0x005111B5 File Offset: 0x0050F3B5
		public void SetQualification(string content)
		{
			this.txtQualification.text = content;
			this._qualificationsContent = content;
			this.RefreshTipsArg2();
		}

		// Token: 0x0600B20B RID: 45579 RVA: 0x005111D4 File Offset: 0x0050F3D4
		private void RefreshTipsArg2()
		{
			TooltipInvoker tooltipInvoker = this.attainmentMouseTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.attainmentMouseTips.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Attainment_QualificationAndAttainment, this._qualificationsContent, this._attainmentContent));
		}

		// Token: 0x0600B20C RID: 45580 RVA: 0x0051122B File Offset: 0x0050F42B
		public void SetIcon(string normal, string hoverIcon, string parentHighlightIcon)
		{
			this._iconNormalSpriteName = normal;
			this._iconHoverSpriteName = hoverIcon;
			this._iconParentHoverSpriteName = parentHighlightIcon;
			this.ApplyIconSprite(this._currentVisualState);
		}

		// Token: 0x0600B20D RID: 45581 RVA: 0x00511250 File Offset: 0x0050F450
		public void SetType(bool isCombatSkill)
		{
			this.skillNameNormalColor = (isCombatSkill ? this.combatSkillNameNormalColor : this.lifeSkillNameNormalColor);
			this.skillNameHoverColor = (isCombatSkill ? this.combatSkillNameHoverColor : this.lifeSkillNameHoverColor);
			this.skillNameParentHoverColor = (isCombatSkill ? this.combatSkillNameParentHoverColor : this.lifeSkillNameParentHoverColor);
		}

		// Token: 0x0600B20E RID: 45582 RVA: 0x005112A3 File Offset: 0x0050F4A3
		private void OnDisable()
		{
			this._isParentHover = false;
			this._isHover = false;
			this.RefreshVisuals(true);
		}

		// Token: 0x040089F4 RID: 35316
		[SerializeField]
		private TextMeshProUGUI txtSkillName;

		// Token: 0x040089F5 RID: 35317
		[SerializeField]
		private TooltipInvoker attainmentMouseTips;

		// Token: 0x040089F6 RID: 35318
		[SerializeField]
		private TextMeshProUGUI txtQualification;

		// Token: 0x040089F7 RID: 35319
		[SerializeField]
		private TextMeshProUGUI txtAttainment;

		// Token: 0x040089F8 RID: 35320
		[SerializeField]
		private CImage imgIcon;

		// Token: 0x040089F9 RID: 35321
		[SerializeField]
		private Color lifeSkillNameNormalColor = Color.white;

		// Token: 0x040089FA RID: 35322
		[SerializeField]
		private Color lifeSkillNameHoverColor = Color.white;

		// Token: 0x040089FB RID: 35323
		[SerializeField]
		private Color lifeSkillNameParentHoverColor = Color.white;

		// Token: 0x040089FC RID: 35324
		[SerializeField]
		private Color combatSkillNameNormalColor = Color.white;

		// Token: 0x040089FD RID: 35325
		[SerializeField]
		private Color combatSkillNameHoverColor = Color.white;

		// Token: 0x040089FE RID: 35326
		[SerializeField]
		private Color combatSkillNameParentHoverColor = Color.white;

		// Token: 0x040089FF RID: 35327
		[SerializeField]
		private Color qualificationNormalColor = Color.white;

		// Token: 0x04008A00 RID: 35328
		[SerializeField]
		private Color qualificationHoverColor = Color.white;

		// Token: 0x04008A01 RID: 35329
		[SerializeField]
		private Color qualificationParentHoverColor = Color.white;

		// Token: 0x04008A02 RID: 35330
		[SerializeField]
		private Color attainmentNormalColor = Color.white;

		// Token: 0x04008A03 RID: 35331
		[SerializeField]
		private Color attainmentHoverColor = Color.white;

		// Token: 0x04008A04 RID: 35332
		[SerializeField]
		private Color attainmentParentHoverColor = Color.white;

		// Token: 0x04008A05 RID: 35333
		[SerializeField]
		private CharacterAttainmentOverviewItem.StateSpriteBinding[] stateSpriteBindings;

		// Token: 0x04008A06 RID: 35334
		public CButton OverallSkillItem;

		// Token: 0x04008A07 RID: 35335
		private string _attainmentContent;

		// Token: 0x04008A08 RID: 35336
		private string _qualificationsContent;

		// Token: 0x04008A09 RID: 35337
		private PointerTrigger _pointerTrigger;

		// Token: 0x04008A0A RID: 35338
		private bool _isHover;

		// Token: 0x04008A0B RID: 35339
		private bool _isParentHover;

		// Token: 0x04008A0C RID: 35340
		private CharacterAttainmentOverviewItem.VisualState _currentVisualState;

		// Token: 0x04008A0D RID: 35341
		private string _iconNormalSpriteName;

		// Token: 0x04008A0E RID: 35342
		private string _iconHoverSpriteName;

		// Token: 0x04008A0F RID: 35343
		private string _iconParentHoverSpriteName;

		// Token: 0x04008A10 RID: 35344
		private Color skillNameNormalColor = Color.white;

		// Token: 0x04008A11 RID: 35345
		private Color skillNameHoverColor = Color.white;

		// Token: 0x04008A12 RID: 35346
		private Color skillNameParentHoverColor = Color.white;

		// Token: 0x0200256B RID: 9579
		[Serializable]
		private struct StateSpriteBinding
		{
			// Token: 0x0400E7E2 RID: 59362
			public CImage Image;

			// Token: 0x0400E7E3 RID: 59363
			public Sprite NormalSprite;

			// Token: 0x0400E7E4 RID: 59364
			public Sprite HoverSprite;

			// Token: 0x0400E7E5 RID: 59365
			public Sprite ParentHoverSprite;
		}

		// Token: 0x0200256C RID: 9580
		private enum VisualState
		{
			// Token: 0x0400E7E7 RID: 59367
			Normal,
			// Token: 0x0400E7E8 RID: 59368
			Hover,
			// Token: 0x0400E7E9 RID: 59369
			ParentHover
		}
	}
}
