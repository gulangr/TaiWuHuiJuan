using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.GameLineAnim
{
	// Token: 0x02000A20 RID: 2592
	public class UnlockSkillSlotAnimItem : MonoBehaviour
	{
		// Token: 0x06007F36 RID: 32566 RVA: 0x003B4450 File Offset: 0x003B2650
		private void OnEnable()
		{
			this.jiesuo1.gameObject.SetActive(true);
			this.jiesuo2.gameObject.SetActive(false);
			this.contentRoot.gameObject.SetActive(false);
			this.contentCanvasGroup.alpha = 0f;
		}

		// Token: 0x06007F37 RID: 32567 RVA: 0x003B44A8 File Offset: 0x003B26A8
		public void Set(ArgumentBox argsBox, Action showHideTips)
		{
			int combatSkillEquipType;
			argsBox.Get("CombatSkillEquipType", out combatSkillEquipType);
			int slotCount;
			argsBox.Get("SlotCount", out slotCount);
			int neiliCount;
			argsBox.Get("NeiliCount", out neiliCount);
			this.combatSkillEquipTypeText.text = LanguageKey.LK_UnlockSkillSlot_CombatSkillEquipType.TrFormat(this._combatSkillEquipTypeLanguageKeys[combatSkillEquipType].Tr());
			this.slotText.text = string.Format("+{0}", slotCount);
			this.neiliText.text = string.Format("+{0}", neiliCount);
			this.jiesuo1.Play();
			float duration = this.jiesuo1["eff_equipcombatskill_jiesuo_001"].length;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(duration, delegate
			{
				this.jiesuo2.gameObject.SetActive(true);
				this.contentRoot.gameObject.SetActive(true);
				this.contentCanvasGroup.DOFade(1f, this._delaySeconds);
				Action showHideTips2 = showHideTips;
				if (showHideTips2 != null)
				{
					showHideTips2();
				}
			});
		}

		// Token: 0x06007F38 RID: 32568 RVA: 0x003B458B File Offset: 0x003B278B
		public UnlockSkillSlotAnimItem()
		{
			LanguageKey[] array = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.9C16881CD4B200790A23BB337925EFB7A26AF352C0B3DA25E36FC07CB6AE2AD9).FieldHandle);
			this._combatSkillEquipTypeLanguageKeys = array;
			base..ctor();
		}

		// Token: 0x0400614B RID: 24907
		[SerializeField]
		private Animation jiesuo1;

		// Token: 0x0400614C RID: 24908
		[SerializeField]
		private Transform jiesuo2;

		// Token: 0x0400614D RID: 24909
		[SerializeField]
		private RectTransform contentRoot;

		// Token: 0x0400614E RID: 24910
		[SerializeField]
		private CanvasGroup contentCanvasGroup;

		// Token: 0x0400614F RID: 24911
		[SerializeField]
		private TextMeshProUGUI combatSkillEquipTypeText;

		// Token: 0x04006150 RID: 24912
		[SerializeField]
		private TextMeshProUGUI slotText;

		// Token: 0x04006151 RID: 24913
		[SerializeField]
		private TextMeshProUGUI neiliTitleText;

		// Token: 0x04006152 RID: 24914
		[SerializeField]
		private TextMeshProUGUI neiliText;

		// Token: 0x04006153 RID: 24915
		[SerializeField]
		private float _delaySeconds = 0.6f;

		// Token: 0x04006154 RID: 24916
		private readonly LanguageKey[] _combatSkillEquipTypeLanguageKeys;
	}
}
