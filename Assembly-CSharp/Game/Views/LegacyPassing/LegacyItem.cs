using System;
using Config;
using Config.Common;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000992 RID: 2450
	public class LegacyItem : MonoBehaviour
	{
		// Token: 0x17000D48 RID: 3400
		// (get) Token: 0x06007617 RID: 30231 RVA: 0x00370F67 File Offset: 0x0036F167
		public short TemplateId
		{
			get
			{
				return this.toggle.isOn ? this.id : -1;
			}
		}

		// Token: 0x17000D49 RID: 3401
		// (get) Token: 0x06007618 RID: 30232 RVA: 0x00370F7F File Offset: 0x0036F17F
		public int Cost
		{
			get
			{
				return (int)Legacy.Instance[this.id].Cost;
			}
		}

		// Token: 0x17000D4A RID: 3402
		// (get) Token: 0x06007619 RID: 30233 RVA: 0x00370F96 File Offset: 0x0036F196
		// (set) Token: 0x0600761A RID: 30234 RVA: 0x00370FA3 File Offset: 0x0036F1A3
		public bool Interactable
		{
			get
			{
				return this.toggle.interactable;
			}
			set
			{
				this.toggle.interactable = value;
			}
		}

		// Token: 0x17000D4B RID: 3403
		// (get) Token: 0x0600761B RID: 30235 RVA: 0x00370FB2 File Offset: 0x0036F1B2
		// (set) Token: 0x0600761C RID: 30236 RVA: 0x00370FBF File Offset: 0x0036F1BF
		public bool SelectedIsOn
		{
			get
			{
				return this.toggle.isOn;
			}
			set
			{
				this.toggle.isOn = value;
			}
		}

		// Token: 0x0600761D RID: 30237 RVA: 0x00370FCE File Offset: 0x0036F1CE
		public void Awake()
		{
			this.selected.gameObject.SetActive(this.SelectedIsOn);
			this.toggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this.parent.Selected(this, isOn);
				this.toggle.OnDeselect(null);
			});
		}

		// Token: 0x0600761E RID: 30238 RVA: 0x00371005 File Offset: 0x0036F205
		private void OnEnable()
		{
			GEvent.Add(UiEvents.RequestLegacyItemRefresh, new GEvent.Callback(this.Refresh));
		}

		// Token: 0x0600761F RID: 30239 RVA: 0x00371024 File Offset: 0x0036F224
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.RequestLegacyItemRefresh, new GEvent.Callback(this.Refresh));
		}

		// Token: 0x06007620 RID: 30240 RVA: 0x00371044 File Offset: 0x0036F244
		private void Refresh(ArgumentBox _)
		{
			bool flag = this.parent;
			if (flag)
			{
				this.parent.Set(this);
			}
		}

		// Token: 0x06007621 RID: 30241 RVA: 0x00371074 File Offset: 0x0036F274
		public void Set(short templateId, bool isFree = false, bool canSelect = true)
		{
			ConfigData<LegacyItem, short> instance = Legacy.Instance;
			this.id = templateId;
			LegacyItem cfg = instance[templateId];
			this.itemName.text = cfg.Name;
			TMP_Text tmp_Text = this.itemCost;
			this.IsFree = isFree;
			tmp_Text.text = (isFree ? "0".SetColor("lightgrey") : cfg.Cost.ToString());
			this.grade.SetColor(Colors.Instance.GradeColors[(int)cfg.Grade]);
			this.icon.SetSprite(cfg.Icon, false, null);
			base.gameObject.SetActive(true);
			this.toggle.interactable = canSelect;
			this.toggle.isOn = false;
			this.tipDisplayer.Type = TipType.SingleDesc;
			this.parent.Set(this);
		}

		// Token: 0x040058E5 RID: 22757
		[NonSerialized]
		public bool IsFree;

		// Token: 0x040058E6 RID: 22758
		[NonSerialized]
		public ConflictType ConflictType;

		// Token: 0x040058E7 RID: 22759
		[SerializeField]
		internal short id;

		// Token: 0x040058E8 RID: 22760
		[SerializeField]
		internal LegacyContainer parent;

		// Token: 0x040058E9 RID: 22761
		[SerializeField]
		internal CToggle toggle;

		// Token: 0x040058EA RID: 22762
		[SerializeField]
		internal GameObject selected;

		// Token: 0x040058EB RID: 22763
		[SerializeField]
		internal TooltipInvoker tipDisplayer;

		// Token: 0x040058EC RID: 22764
		[SerializeField]
		private CImage grade;

		// Token: 0x040058ED RID: 22765
		[SerializeField]
		private CImage icon;

		// Token: 0x040058EE RID: 22766
		[SerializeField]
		private TMP_Text itemName;

		// Token: 0x040058EF RID: 22767
		[SerializeField]
		private TMP_Text itemCost;
	}
}
