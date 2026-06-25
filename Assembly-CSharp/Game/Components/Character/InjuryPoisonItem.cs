using System;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F34 RID: 3892
	public class InjuryPoisonItem : MonoBehaviour
	{
		// Token: 0x17001447 RID: 5191
		// (get) Token: 0x0600B30A RID: 45834 RVA: 0x005180D1 File Offset: 0x005162D1
		public TextMeshProUGUI TextValue
		{
			get
			{
				return this.textValue;
			}
		}

		// Token: 0x17001448 RID: 5192
		// (get) Token: 0x0600B30B RID: 45835 RVA: 0x005180D9 File Offset: 0x005162D9
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x17001449 RID: 5193
		// (get) Token: 0x0600B30C RID: 45836 RVA: 0x005180E1 File Offset: 0x005162E1
		public PointerTrigger PointerTrigger
		{
			get
			{
				return this.pointerTrigger;
			}
		}

		// Token: 0x1700144A RID: 5194
		// (get) Token: 0x0600B30D RID: 45837 RVA: 0x005180E9 File Offset: 0x005162E9
		public GameObject Selected
		{
			get
			{
				return this.selected;
			}
		}

		// Token: 0x1700144B RID: 5195
		// (get) Token: 0x0600B30E RID: 45838 RVA: 0x005180F1 File Offset: 0x005162F1
		public GameObject Recovery
		{
			get
			{
				return this.recovery;
			}
		}

		// Token: 0x1700144C RID: 5196
		// (get) Token: 0x0600B30F RID: 45839 RVA: 0x005180F9 File Offset: 0x005162F9
		public GameObject Aggravate
		{
			get
			{
				return this.aggravate;
			}
		}

		// Token: 0x0600B310 RID: 45840 RVA: 0x00518104 File Offset: 0x00516304
		public void Set(sbyte type, int value, int resist, bool isImmune, bool isBornImmune)
		{
			sbyte level = PoisonsAndLevels.CalcPoisonedLevel(value);
			PoisonItem config = Poison.Instance[type];
			this.textName.text = config.Name;
			this.textValue.text = value.ToString();
			this.icon.SetSprite("ui9_icon_poison_2_" + type.ToString(), false, null);
			for (int i = 0; i < this.poisonLevels.Length; i++)
			{
				this.poisonLevels[i].gameObject.SetActive(i < (int)level);
				bool flag = i < (int)level;
				if (flag)
				{
					DOTweenAnimation dotAnim = this.poisonLevels[i].GetComponent<DOTweenAnimation>();
					dotAnim.DOPause();
					dotAnim.DOGoto(0f, false);
				}
			}
			this.immuneObj.SetActive(isImmune && !isBornImmune);
			this.immuneBornObj.SetActive(isBornImmune);
			this.mouseTip.enabled = true;
			this.mouseTip.Type = TipType.CharacterPoison;
			this.mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("PoisonType", type).Set("IsBornImmune", isBornImmune).Set("PoisonResist", resist).Set("PoisonValue", value).Set("PoisonLevel", level);
			this.selected.SetActive(false);
			this.recovery.SetActive(false);
			this.aggravate.SetActive(false);
		}

		// Token: 0x04008AF7 RID: 35575
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04008AF8 RID: 35576
		[SerializeField]
		private TextMeshProUGUI textValue;

		// Token: 0x04008AF9 RID: 35577
		[SerializeField]
		private CImage icon;

		// Token: 0x04008AFA RID: 35578
		[SerializeField]
		private GameObject immuneObj;

		// Token: 0x04008AFB RID: 35579
		[SerializeField]
		private GameObject immuneBornObj;

		// Token: 0x04008AFC RID: 35580
		[SerializeField]
		private CImage[] poisonLevels;

		// Token: 0x04008AFD RID: 35581
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008AFE RID: 35582
		[SerializeField]
		private CButton button;

		// Token: 0x04008AFF RID: 35583
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04008B00 RID: 35584
		[SerializeField]
		private GameObject selected;

		// Token: 0x04008B01 RID: 35585
		[SerializeField]
		private GameObject hover;

		// Token: 0x04008B02 RID: 35586
		[SerializeField]
		private GameObject recovery;

		// Token: 0x04008B03 RID: 35587
		[SerializeField]
		private GameObject aggravate;
	}
}
