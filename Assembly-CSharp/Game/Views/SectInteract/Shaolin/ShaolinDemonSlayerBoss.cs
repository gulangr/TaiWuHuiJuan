using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Story.SectMainStory;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Shaolin
{
	// Token: 0x020009E6 RID: 2534
	public class ShaolinDemonSlayerBoss : MonoBehaviour
	{
		// Token: 0x06007C7A RID: 31866 RVA: 0x0039D930 File Offset: 0x0039BB30
		public void Init(int index, Action<int> onClickStart)
		{
			this._index = index;
			this.btnOpenCharacterMenu.ClearAndAddListener(new Action(this.OnClickOpenCharacterMenu));
			this.btnStart.ClearAndAddListener(delegate
			{
				onClickStart(this._index);
			});
			this.warningBack.SetSprite(string.Format("ui9_back_demon_slayer_warning_{0}", SingletonObject.getInstance<GlobalSettings>().Language.ToLower()), false, null);
		}

		// Token: 0x06007C7B RID: 31867 RVA: 0x0039D9B0 File Offset: 0x0039BBB0
		public void Set(SectShaolinDemonSlayerData data, List<int> bossIds)
		{
			this._data = data;
			this._id = bossIds[this._index];
			DemonSlayerTrialItem config = data.GetTrialingDemon(this._index);
			CharacterItem charConfig = Character.Instance[config.CharacterId];
			bool isDefeated = this._data.IsDemonDefeated(this._data.GetTrialingDemon(this._index).TemplateId);
			this.nameLabel.text = charConfig.GivenName;
			bool flag = config.SpecialDesc.IsNullOrEmpty();
			if (flag)
			{
				this.desc.SetActive(false);
			}
			else
			{
				this.descLabel.text = config.SpecialDesc.ColorReplace();
				this.desc.SetActive(true);
			}
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(config.Desc);
			sb.AppendLine(LanguageKey.LK_SectShaolinDemonSlayer_RewardTips_Subtitle.Tr().ColorReplace());
			sb.Append(LanguageKey.LK_Dot_Symbol.Tr());
			sb.Append(Accessory.Instance[config.FirstTimeRewards].Name);
			bool flag2 = isDefeated;
			if (flag2)
			{
				sb.Append(LanguageKey.LK_SectShaolinDemonSlayer_RewardTips_Owned.Tr());
			}
			string[] tipArgs = new string[]
			{
				charConfig.GivenName,
				sb.ToString()
			};
			this.tipsArea.PresetParam = tipArgs;
			this.tipsArea.gameObject.SetActive(true);
			this.btnOpenCharacterMenu.GetComponent<TooltipInvoker>().PresetParam = tipArgs;
			this.btnOpenCharacterMenu.gameObject.SetActive(true);
		}

		// Token: 0x06007C7C RID: 31868 RVA: 0x0039DB3C File Offset: 0x0039BD3C
		public void SetRestrictions(byte restrictResult)
		{
			int restrictIndex = 0;
			BoolArray8 array = restrictResult;
			bool meetAll = true;
			foreach (DemonSlayerTrialRestrictItem restrict in this._data.GetTrialingRestricts(this._index))
			{
				bool meet = array[restrictIndex];
				ShaolinDemonSlayerRestriction obj = this.restrictions[restrictIndex++];
				obj.gameObject.SetActive(true);
				obj.Set(restrict, meet);
				bool flag = !meet;
				if (flag)
				{
					meetAll = false;
				}
			}
			for (int i = restrictIndex; i < 3; i++)
			{
				this.restrictions[i].gameObject.SetActive(false);
			}
			this.btnStart.interactable = meetAll;
			this.btnStart.GetComponent<TooltipInvoker>().enabled = !this.btnStart.interactable;
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
		}

		// Token: 0x06007C7D RID: 31869 RVA: 0x0039DC40 File Offset: 0x0039BE40
		private void OnClickOpenCharacterMenu()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this._id);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x04005E99 RID: 24217
		public TextMeshProUGUI nameLabel;

		// Token: 0x04005E9A RID: 24218
		public TextMeshProUGUI descLabel;

		// Token: 0x04005E9B RID: 24219
		public GameObject desc;

		// Token: 0x04005E9C RID: 24220
		public CButton btnStart;

		// Token: 0x04005E9D RID: 24221
		public CButton btnOpenCharacterMenu;

		// Token: 0x04005E9E RID: 24222
		public TooltipInvoker tipsArea;

		// Token: 0x04005E9F RID: 24223
		public ShaolinDemonSlayerRestriction[] restrictions = new ShaolinDemonSlayerRestriction[3];

		// Token: 0x04005EA0 RID: 24224
		public CImage warningBack;

		// Token: 0x04005EA1 RID: 24225
		public RectTransform content;

		// Token: 0x04005EA2 RID: 24226
		private int _id;

		// Token: 0x04005EA3 RID: 24227
		private int _index;

		// Token: 0x04005EA4 RID: 24228
		private SectShaolinDemonSlayerData _data;
	}
}
