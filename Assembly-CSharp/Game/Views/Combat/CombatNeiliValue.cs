using System;
using FrameWork;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B32 RID: 2866
	public class CombatNeiliValue : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F76 RID: 3958
		// (get) Token: 0x06008C58 RID: 35928 RVA: 0x0040D63B File Offset: 0x0040B83B
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008C59 RID: 35929 RVA: 0x0040D644 File Offset: 0x0040B844
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnCurrNeiliChanged = (OnDataChangedEvent)Delegate.Combine(model.OnCurrNeiliChanged, new OnDataChangedEvent(this.OnNeiliChanged));
			CombatModel model2 = this.Model;
			model2.OnMaxNeiliChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnMaxNeiliChanged, new OnDataChangedEvent(this.OnNeiliChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.RefreshCurrentCharacter();
		}

		// Token: 0x06008C5A RID: 35930 RVA: 0x0040D6C0 File Offset: 0x0040B8C0
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnCurrNeiliChanged = (OnDataChangedEvent)Delegate.Remove(model.OnCurrNeiliChanged, new OnDataChangedEvent(this.OnNeiliChanged));
			CombatModel model2 = this.Model;
			model2.OnMaxNeiliChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnMaxNeiliChanged, new OnDataChangedEvent(this.OnNeiliChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008C5B RID: 35931 RVA: 0x0040D738 File Offset: 0x0040B938
		public void Set(int currNeili, int maxNeili)
		{
			this.bar.fillAmount = ((maxNeili > 0) ? ((float)currNeili / (float)maxNeili) : 0f);
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get("LK_Combat_Neili_Tip_Title"));
				this.mouseTip.RuntimeParam.Set("arg1", LocalStringManager.GetFormat("LK_Combat_Neili_Tip_Content", currNeili, maxNeili));
				this.mouseTip.Refresh(false, -1);
			}
		}

		// Token: 0x06008C5C RID: 35932 RVA: 0x0040D7FC File Offset: 0x0040B9FC
		private void OnNeiliChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				this.RefreshCurrentCharacter();
			}
		}

		// Token: 0x06008C5D RID: 35933 RVA: 0x0040D824 File Offset: 0x0040BA24
		private void OnChangeChar()
		{
			int changingToCharId = this.Model.ChangingToCharId;
			bool flag = changingToCharId <= 0 || this.Model.CharIsAlly(changingToCharId) != this.ally;
			if (!flag)
			{
				this.RefreshCurrentCharacter();
			}
		}

		// Token: 0x06008C5E RID: 35934 RVA: 0x0040D86C File Offset: 0x0040BA6C
		private void RefreshCurrentCharacter()
		{
			CombatSubProcessorCharacter processor = this.ally ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = processor == null;
			if (!flag)
			{
				this.Set(processor.CurrNeili, processor.MaxNeili);
			}
		}

		// Token: 0x04006B64 RID: 27492
		[SerializeField]
		private bool ally;

		// Token: 0x04006B65 RID: 27493
		[SerializeField]
		private CImage bar;

		// Token: 0x04006B66 RID: 27494
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}
