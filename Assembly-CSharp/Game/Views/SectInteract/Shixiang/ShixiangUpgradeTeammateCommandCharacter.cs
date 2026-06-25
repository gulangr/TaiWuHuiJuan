using System;
using System.Collections.Generic;
using FrameWork;
using Game.Components.Character;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Views.SectInteract.Shixiang
{
	// Token: 0x020009E1 RID: 2529
	public class ShixiangUpgradeTeammateCommandCharacter : MonoBehaviour
	{
		// Token: 0x06007C1E RID: 31774 RVA: 0x0039B494 File Offset: 0x00399694
		public void Set(ViewShixiangUpgradeTeammateCommand parent, GroupCharDisplayData data)
		{
			this._parent = parent;
			this._charId = data.CharacterId;
			CommonUtils.CalculateOwnedCommandMedals(data.Command.Items, this._equippedCommandMedalDict);
			this.basic.SetBasic(data.AvatarRelatedData, NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, false, false), data.CharacterTemplateId);
			this.button.ClearAndAddListener(new Action(this.OnSelectCharacter));
			this.medals.Set(data.AttackMedal, data.DefenceMedal, data.WisdomMedal);
			this.fightMark.SetActive(SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuCombatTeamCharIds().Contains(this._charId));
			for (int i = 0; i < this.commands.childCount; i++)
			{
				GameObject obj = this.commands.GetChild(i).gameObject;
				bool flag = data.Command.Items == null || i >= data.Command.Items.Count;
				if (flag)
				{
					obj.SetActive(false);
				}
				else
				{
					obj.GetComponent<ShixiangUpgradeTeammateCommandShortItem>().Set(this._parent, this._charId, data.Command.Items[i]);
					obj.SetActive(true);
				}
			}
			TooltipInvoker tips = this.button.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tips.RuntimeParam.Set("CharId", this._charId);
			tips.Refresh(false, -1);
		}

		// Token: 0x06007C1F RID: 31775 RVA: 0x0039B629 File Offset: 0x00399829
		public void SetSelected(bool selected)
		{
			this.button.SetSelected(selected);
		}

		// Token: 0x06007C20 RID: 31776 RVA: 0x0039B639 File Offset: 0x00399839
		private void OnSelectCharacter()
		{
			this._parent.OnCharacterSelect(this._charId);
		}

		// Token: 0x04005E41 RID: 24129
		public CharacterNormal basic;

		// Token: 0x04005E42 RID: 24130
		public CharacterMenuSelectableCharacterNormal button;

		// Token: 0x04005E43 RID: 24131
		public MedalSummary medals;

		// Token: 0x04005E44 RID: 24132
		public Transform commands;

		// Token: 0x04005E45 RID: 24133
		public GameObject fightMark;

		// Token: 0x04005E46 RID: 24134
		private ViewShixiangUpgradeTeammateCommand _parent;

		// Token: 0x04005E47 RID: 24135
		private int _charId;

		// Token: 0x04005E48 RID: 24136
		private Dictionary<int, int> _equippedCommandMedalDict = new Dictionary<int, int>();
	}
}
