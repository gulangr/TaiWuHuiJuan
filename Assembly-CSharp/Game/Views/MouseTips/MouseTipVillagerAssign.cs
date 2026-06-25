using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Domains.Map;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000879 RID: 2169
	public class MouseTipVillagerAssign : MouseTipBase
	{
		// Token: 0x0600685D RID: 26717 RVA: 0x002FB828 File Offset: 0x002F9A28
		protected override void Init(ArgumentBox argsBox)
		{
			this._model = SingletonObject.getInstance<WorldMapModel>();
			argsBox.Get("areaId", out this._areaId);
			argsBox.Get<AreaDisplayData>("displayData", out this._displayData);
			this._cfg = this._model.Areas[(int)this._areaId].GetConfig();
			this.title.text = this._cfg.Name.SetColor(this._displayData.IsBroken ? "brokenarea" : "");
			this.desc.text = (this._displayData.HasSectZhujianSpecialMerchant ? (this._cfg.Desc + LanguageKey.LK_SectZhujian_SpecialMerchant_AreaTip.Tr()) : this._cfg.Desc).ColorReplace();
			bool isFarmer;
			bool flag = argsBox.Get("farmer", out isFarmer) && isFarmer;
			if (flag)
			{
				this.settlements.gameObject.SetActive(false);
				int i = MouseTipVillagerAssign.ResourceKeys.Length;
				while (i-- > 0)
				{
					this.farmer.texts[i].text = MouseTipVillagerAssign.ResourceKeys[i].TrFormat(this._displayData.MigratableBlocks[i]);
				}
				this.farmer.gameObject.SetActive(true);
			}
			else
			{
				this.farmer.gameObject.SetActive(false);
				this.settlements.gameObject.SetActive(true);
				this.settlements.Rebuild<ImagesAndTexts, SettlementDisplayData>(this._displayData.SettlementDisplayData, delegate(ImagesAndTexts it, SettlementDisplayData d)
				{
					it.texts[0].text = LanguageKey.LK_VillagerRole_Summary_Tips_7.TrFormat(d.Safety, d.MaxSafety);
					it.texts[1].text = LanguageKey.LK_VillagerRole_Summary_Tips_6.TrFormat(d.Culture, d.MaxCulture);
					it.texts[2].text = d.SettlementNameRelatedData.GetName();
				});
			}
		}

		// Token: 0x0600685F RID: 26719 RVA: 0x002FB9E3 File Offset: 0x002F9BE3
		// Note: this type is marked as 'beforefieldinit'.
		static MouseTipVillagerAssign()
		{
			LanguageKey[] array = new LanguageKey[6];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.BCD7665C7CCDACE7236F8FFE970B1CF794E889B1F836BB74E53B3710952FC70D).FieldHandle);
			MouseTipVillagerAssign.ResourceKeys = array;
		}

		// Token: 0x04004A0C RID: 18956
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004A0D RID: 18957
		[SerializeField]
		private TMP_Text desc;

		// Token: 0x04004A0E RID: 18958
		[SerializeField]
		private ImagesAndTexts farmer;

		// Token: 0x04004A0F RID: 18959
		[SerializeField]
		private TemplatedContainerAssemblyNew settlements;

		// Token: 0x04004A10 RID: 18960
		private short _areaId;

		// Token: 0x04004A11 RID: 18961
		private AreaDisplayData _displayData;

		// Token: 0x04004A12 RID: 18962
		private MapAreaItem _cfg;

		// Token: 0x04004A13 RID: 18963
		private WorldMapModel _model;

		// Token: 0x04004A14 RID: 18964
		private static readonly LanguageKey[] ResourceKeys;
	}
}
