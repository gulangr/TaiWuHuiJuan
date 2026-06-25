using System;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C7 RID: 2503
	public class ZhujianGearMateCombatSkillItem : MonoBehaviour
	{
		// Token: 0x0600796D RID: 31085 RVA: 0x00386D3C File Offset: 0x00384F3C
		private void Awake()
		{
			this.btn.ClearAndAddListener(delegate
			{
				Action onSelect = this._onSelect;
				if (onSelect != null)
				{
					onSelect();
				}
			});
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(new UnityAction(this.TurnOnHover));
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(new UnityAction(this.TurnOffHover));
		}

		// Token: 0x0600796E RID: 31086 RVA: 0x00386DC0 File Offset: 0x00384FC0
		public void Set(CombatSkillDisplayData data, Action onSelect, string tipContent, GearMateBreakoutBanReason gearMateBreakoutBanReason, SByteList progress)
		{
			bool isEnable = string.IsNullOrEmpty(tipContent);
			this.mouseTip.Type = (isEnable ? TipType.CombatSkill : ((gearMateBreakoutBanReason == GearMateBreakoutBanReason.SameBreakResult) ? TipType.SingleDesc : TipType.Simple));
			this.item.Set(data);
			this._onSelect = (isEnable ? delegate()
			{
				onSelect();
			} : null);
			this.btn.interactable = isEnable;
			this.pointerTrigger.enabled = isEnable;
			bool flag = !isEnable;
			if (flag)
			{
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				bool flag2 = gearMateBreakoutBanReason == GearMateBreakoutBanReason.SameBreakResult;
				if (flag2)
				{
					this.mouseTip.RuntimeParam.Set("arg0", tipContent);
				}
				else
				{
					this.mouseTip.RuntimeParam.Set("arg0", CombatSkill.Instance[data.TemplateId].Name);
					StringBuilder sb = EasyPool.Get<StringBuilder>();
					sb.Append(tipContent + "\n");
					sb.Append((LanguageKey.LK_Lingxing_Sprite.Tr() + LanguageKey.UI_NewGame_BornDateInfo.TrFormat(LanguageKey.LK_MouseTipReadProgress_Title.Tr())).SetColor("pinkyellow") + "\n");
					for (int i = 0; i < 15; i++)
					{
						string readingPoregress = string.Format("{0}%", (int)((progress.Items != null) ? progress.Items[i] : 0));
						bool flag3 = i < 5;
						if (flag3)
						{
							sb.Append(string.Concat(new string[]
							{
								LanguageKey.LK_Dot_Symbol.Tr(),
								LanguageKey.LK_CombatSkill_Book_First_Page.Tr(),
								LanguageKey.LK_Dot_Symbol.Tr(),
								LocalStringManager.Get("LK_CombatSkill_First_Page_Type_" + i.ToString()).SetColor(PracticeSkillPlatePageUtils.OutlinePageColorMap[i]),
								LanguageKey.LK_Colon_Symbol.Tr(),
								readingPoregress
							}));
						}
						else
						{
							bool flag4 = i < 10;
							if (flag4)
							{
								int directIndex = i - 5;
								sb.Append(string.Concat(new string[]
								{
									LanguageKey.LK_Dot_Symbol.Tr(),
									LocalStringManager.Get("LK_Book_Page_Index_" + directIndex.ToString()),
									LanguageKey.LK_Dot_Symbol.Tr(),
									LocalStringManager.Get("LK_CombatSkill_Direct_Page_" + directIndex.ToString()).SetColor("brightblue"),
									LanguageKey.LK_Colon_Symbol.Tr(),
									readingPoregress
								}));
							}
							else
							{
								int reverseIndex = i - 5 - 5;
								sb.Append(string.Concat(new string[]
								{
									LanguageKey.LK_Dot_Symbol.Tr(),
									LocalStringManager.Get("LK_Book_Page_Index_" + reverseIndex.ToString()),
									LanguageKey.LK_Dot_Symbol.Tr(),
									LocalStringManager.Get("LK_CombatSkill_Reverse_Page_" + reverseIndex.ToString()).SetColor("brightred"),
									LanguageKey.LK_Colon_Symbol.Tr(),
									readingPoregress
								}));
							}
						}
						sb.Append("\n");
					}
					this.mouseTip.RuntimeParam.Set("arg1", sb.ToString());
				}
				this.hSVStyleRoot.SetDefaultBlack();
			}
			else
			{
				this.hSVStyleRoot.SetDefault();
			}
		}

		// Token: 0x0600796F RID: 31087 RVA: 0x00387149 File Offset: 0x00385349
		public void SetSelected(bool value)
		{
			this.selected.SetActive(value);
		}

		// Token: 0x06007970 RID: 31088 RVA: 0x00387159 File Offset: 0x00385359
		private void TurnOnHover()
		{
			this.hover.SetActive(true);
		}

		// Token: 0x06007971 RID: 31089 RVA: 0x00387169 File Offset: 0x00385369
		private void TurnOffHover()
		{
			this.hover.SetActive(false);
		}

		// Token: 0x04005C15 RID: 23573
		public GameObject selected;

		// Token: 0x04005C16 RID: 23574
		public GameObject hover;

		// Token: 0x04005C17 RID: 23575
		public CharacterMenuCombatSkillItem item;

		// Token: 0x04005C18 RID: 23576
		public CButton btn;

		// Token: 0x04005C19 RID: 23577
		public PointerTrigger pointerTrigger;

		// Token: 0x04005C1A RID: 23578
		public TooltipInvoker mouseTip;

		// Token: 0x04005C1B RID: 23579
		public HSVStyleRoot hSVStyleRoot;

		// Token: 0x04005C1C RID: 23580
		private Action _onSelect;
	}
}
