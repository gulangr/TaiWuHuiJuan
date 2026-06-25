using System;
using FrameWork;
using Game.Views.Alertness;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000838 RID: 2104
	public class MouseTipAlertness : MouseTipBase
	{
		// Token: 0x060066A8 RID: 26280 RVA: 0x002ECCB0 File Offset: 0x002EAEB0
		protected override void Init(ArgumentBox argsBox)
		{
			this.NeedWaitData = true;
			this._alertnessData = null;
			argsBox.Get<CharacterAlertnessData>("alertnessData", out this._alertnessData);
			int charId;
			bool flag = !argsBox.Get("charId", out charId);
			if (flag)
			{
				charId = -1;
			}
			bool flag2 = charId >= 0;
			if (flag2)
			{
				CharacterDomainMethod.AsyncCall.GetAlertnessData(this, charId, false, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._alertnessData);
					this.RefreshData();
				});
			}
			else
			{
				bool flag3 = this._alertnessData != null;
				if (flag3)
				{
					this.RefreshData();
				}
			}
		}

		// Token: 0x060066A9 RID: 26281 RVA: 0x002ECD34 File Offset: 0x002EAF34
		private void RefreshData()
		{
			string levelName = (this._alertnessData == null) ? "-" : CommonUtils.GetAlertnessName((int)this._alertnessData.Level);
			this.textLevel.text = levelName;
			string changeFavorContent = ViewAlertness.GetTextEffectChangeFavor(this._alertnessData);
			this.textEffectChangeFavor.text = changeFavorContent;
			string interactContent = ViewAlertness.GetTextEffectInteract(this._alertnessData);
			this.textEffectInteract.text = interactContent;
			string exchangeContent = ViewAlertness.GetTextEffectExchange(this._alertnessData);
			this.textEffectExchange.text = exchangeContent;
			string maxFavorName = ViewAlertness.GetTextEffectMaxFavor(this._alertnessData);
			this.textEffectMaxFavor.text = maxFavorName;
			for (int i = 0; i < this.textLevelArray.Length; i++)
			{
				string alertnessIcon = CommonUtils.GetAlertnessIcon(i);
				Color color = Colors.Instance.AlertnessColors[i];
				string alertnessName = CommonUtils.GetAlertnessName(i).SetColor(color);
				TextMeshProUGUI text = this.textLevelArray[i];
				text.text = "<SpName=" + alertnessIcon + ">" + alertnessName;
				text.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x040047E8 RID: 18408
		[SerializeField]
		private TextMeshProUGUI textLevel;

		// Token: 0x040047E9 RID: 18409
		[SerializeField]
		private TextMeshProUGUI textEffectChangeFavor;

		// Token: 0x040047EA RID: 18410
		[SerializeField]
		private TextMeshProUGUI textEffectMaxFavor;

		// Token: 0x040047EB RID: 18411
		[SerializeField]
		private TextMeshProUGUI textEffectInteract;

		// Token: 0x040047EC RID: 18412
		[SerializeField]
		private TextMeshProUGUI textEffectExchange;

		// Token: 0x040047ED RID: 18413
		[SerializeField]
		private TextMeshProUGUI[] textLevelArray;

		// Token: 0x040047EE RID: 18414
		private CharacterAlertnessData _alertnessData;
	}
}
