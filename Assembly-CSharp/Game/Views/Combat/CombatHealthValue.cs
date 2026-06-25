using System;
using GameData.Domains.Character;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B2D RID: 2861
	public class CombatHealthValue : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F73 RID: 3955
		// (get) Token: 0x06008C2E RID: 35886 RVA: 0x0040C57B File Offset: 0x0040A77B
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008C2F RID: 35887 RVA: 0x0040C584 File Offset: 0x0040A784
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnHealthChanged = (OnDataChangedEvent)Delegate.Combine(model.OnHealthChanged, new OnDataChangedEvent(this.OnHealthChanged));
			this.Model.AddEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.RefreshCurrentCharacter();
		}

		// Token: 0x06008C30 RID: 35888 RVA: 0x0040C5F4 File Offset: 0x0040A7F4
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnHealthChanged = (OnDataChangedEvent)Delegate.Remove(model.OnHealthChanged, new OnDataChangedEvent(this.OnHealthChanged));
			this.Model.RemoveEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008C31 RID: 35889 RVA: 0x0040C65B File Offset: 0x0040A85B
		public void Set(short health, short maxHealth)
		{
			this.fillImage.fillAmount = ((maxHealth > 0) ? Mathf.Clamp01((float)health / (float)maxHealth) : 0f);
		}

		// Token: 0x06008C32 RID: 35890 RVA: 0x0040C680 File Offset: 0x0040A880
		private void OnHealthChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				this.RefreshCurrentCharacter();
			}
		}

		// Token: 0x06008C33 RID: 35891 RVA: 0x0040C6A8 File Offset: 0x0040A8A8
		private void OnChangeChar()
		{
			int changingToCharId = this.Model.ChangingToCharId;
			bool flag = changingToCharId <= 0 || this.Model.CharIsAlly(changingToCharId) != this.ally;
			if (!flag)
			{
				this._leftMaxHealth = 0;
				this.RefreshCurrentCharacter();
				this.RequestLeftMaxHealth();
			}
		}

		// Token: 0x06008C34 RID: 35892 RVA: 0x0040C6FB File Offset: 0x0040A8FB
		private void OnDataReady()
		{
			this._leftMaxHealth = 0;
			this.RefreshCurrentCharacter();
			this.RequestLeftMaxHealth();
		}

		// Token: 0x06008C35 RID: 35893 RVA: 0x0040C714 File Offset: 0x0040A914
		private void RefreshCurrentCharacter()
		{
			CombatSubProcessorCharacter processor = this.ally ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = processor == null;
			if (!flag)
			{
				this._currentHealth = processor.Health;
				this.RefreshFill();
			}
		}

		// Token: 0x06008C36 RID: 35894 RVA: 0x0040C760 File Offset: 0x0040A960
		private void RequestLeftMaxHealth()
		{
			int charId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
			bool flag = charId <= 0;
			if (!flag)
			{
				CharacterDomainMethod.AsyncCall.GetLeftMaxHealth(null, charId, delegate(int offset, RawDataPool dataPool)
				{
					int currentCharId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
					bool flag2 = currentCharId != charId;
					if (!flag2)
					{
						Serializer.Deserialize(dataPool, offset, ref this._leftMaxHealth);
						this.RefreshFill();
					}
				});
			}
		}

		// Token: 0x06008C37 RID: 35895 RVA: 0x0040C7CC File Offset: 0x0040A9CC
		private void RefreshFill()
		{
			this.Set(this._currentHealth, this._leftMaxHealth);
			this.RefreshTip();
		}

		// Token: 0x06008C38 RID: 35896 RVA: 0x0040C7EC File Offset: 0x0040A9EC
		private void RefreshTip()
		{
			bool flag = this.tip == null || this._leftMaxHealth <= 0;
			if (!flag)
			{
				int charId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
				ValueTuple<string, float, int> healthInfo = CommonUtils.GetCharacterHealthInfo(this._currentHealth, this._leftMaxHealth, charId);
				string healthLevel = healthInfo.Item1;
				string title = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Title.Tr();
				string healthLine = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Level.TrFormat(healthLevel);
				string desc = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Desc1.Tr();
				string desc2 = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Desc2.Tr();
				string tipContent = string.Concat(new string[]
				{
					healthLine,
					"\n\n",
					desc,
					"\n",
					desc2
				});
				this.tip.enabled = true;
				this.tip.Type = TipType.Simple;
				this.tip.IsLanguageKey = false;
				this.tip.PresetParam = new string[]
				{
					title,
					tipContent
				};
			}
		}

		// Token: 0x04006B4B RID: 27467
		[SerializeField]
		private bool ally;

		// Token: 0x04006B4C RID: 27468
		[SerializeField]
		private CImage fillImage;

		// Token: 0x04006B4D RID: 27469
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04006B4E RID: 27470
		private short _leftMaxHealth;

		// Token: 0x04006B4F RID: 27471
		private short _currentHealth;
	}
}
