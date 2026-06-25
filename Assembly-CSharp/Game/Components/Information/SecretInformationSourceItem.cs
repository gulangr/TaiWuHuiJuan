using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EFB RID: 3835
	public class SecretInformationSourceItem : MonoBehaviour
	{
		// Token: 0x0600B0AE RID: 45230 RVA: 0x00508C6C File Offset: 0x00506E6C
		public void Set(CharacterDisplayData data, int count = 0)
		{
			this.avatar.Refresh(data, true);
			this.nameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(data, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			this.identityLabel.text = CommonUtils.GetIdentityString(data.OrgInfo, data.Gender, data.PhysiologicalAge, false);
			this.identityIcon.SetSprite(CommonUtils.GetIdentityIconName(data.OrgInfo.Grade), false, null);
			this.orgLabel.text = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.OrgInfo);
			this.orgIcon.SetSprite(string.Format("ui9_icon_faction_behavior_big_{0}", Math.Max(0, (int)Organization.Instance[data.OrgInfo.OrgTemplateId].FiveElementsType)), false, null);
			this.holderCountLabel.text = count.ToString();
		}

		// Token: 0x0600B0AF RID: 45231 RVA: 0x00508D5A File Offset: 0x00506F5A
		public void SetSelected(bool value)
		{
			this.check.SetActive(value);
		}

		// Token: 0x0600B0B0 RID: 45232 RVA: 0x00508D6C File Offset: 0x00506F6C
		public void Init(int index, Action<int> callback)
		{
			this._index = index;
			this._callback = callback;
			this.button.ClearAndAddListener(new Action(this.Select));
			this.pointerTrigger.EnterEvent.ResetListener(new Action(this.HoverOn));
			this.pointerTrigger.ExitEvent.ResetListener(new Action(this.HoverOff));
		}

		// Token: 0x0600B0B1 RID: 45233 RVA: 0x00508DDA File Offset: 0x00506FDA
		private void HoverOn()
		{
			this.hover.SetActive(true);
		}

		// Token: 0x0600B0B2 RID: 45234 RVA: 0x00508DEA File Offset: 0x00506FEA
		private void HoverOff()
		{
			this.hover.SetActive(false);
		}

		// Token: 0x0600B0B3 RID: 45235 RVA: 0x00508DFA File Offset: 0x00506FFA
		private void Select()
		{
			this.SetSelected(true);
			this._callback(this._index);
		}

		// Token: 0x040088B6 RID: 34998
		[SerializeField]
		protected Game.Components.Avatar.Avatar avatar;

		// Token: 0x040088B7 RID: 34999
		[SerializeField]
		protected TextMeshProUGUI nameLabel;

		// Token: 0x040088B8 RID: 35000
		[SerializeField]
		protected TextMeshProUGUI typeLabel;

		// Token: 0x040088B9 RID: 35001
		[SerializeField]
		protected TextMeshProUGUI identityLabel;

		// Token: 0x040088BA RID: 35002
		[SerializeField]
		protected CImage identityIcon;

		// Token: 0x040088BB RID: 35003
		[SerializeField]
		protected TextMeshProUGUI orgLabel;

		// Token: 0x040088BC RID: 35004
		[SerializeField]
		protected CImage orgIcon;

		// Token: 0x040088BD RID: 35005
		[SerializeField]
		protected TextMeshProUGUI holderCountLabel;

		// Token: 0x040088BE RID: 35006
		[SerializeField]
		protected CButton button;

		// Token: 0x040088BF RID: 35007
		[SerializeField]
		protected PointerTrigger pointerTrigger;

		// Token: 0x040088C0 RID: 35008
		[SerializeField]
		protected GameObject hover;

		// Token: 0x040088C1 RID: 35009
		[SerializeField]
		protected GameObject check;

		// Token: 0x040088C2 RID: 35010
		private int _index;

		// Token: 0x040088C3 RID: 35011
		private Action<int> _callback;
	}
}
