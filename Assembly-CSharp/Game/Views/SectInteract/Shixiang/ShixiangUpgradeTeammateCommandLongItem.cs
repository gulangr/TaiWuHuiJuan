using System;
using Coffee.UIExtensions;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using UnityEngine;

namespace Game.Views.SectInteract.Shixiang
{
	// Token: 0x020009E2 RID: 2530
	public class ShixiangUpgradeTeammateCommandLongItem : MonoBehaviour
	{
		// Token: 0x06007C22 RID: 31778 RVA: 0x0039B664 File Offset: 0x00399864
		public void Set(ViewShixiangUpgradeTeammateCommand parent, sbyte commandId, bool isEquipped, bool canUpgrade)
		{
			this._parent = parent;
			this._commandId = commandId;
			this._isEquipped = isEquipped;
			this.particles.gameObject.SetActive(false);
			bool isAdvance = this._parent.IsCommandAdvance(commandId);
			if (canUpgrade)
			{
				this.particles.gameObject.SetActive(true);
				this.particles.GetChild(0).GetComponent<UIParticle>().Play();
			}
			this.btnSelect.ClearAndAddListener(new Action(this.OnSelectCommand));
			this.btnCancel.gameObject.SetActive(isAdvance);
			this.btnCancel.ClearAndAddListener(new Action(this.OnClickCancel));
			this.teammateCommand.GetComponent<RectTransform>().SetWidth((float)(isAdvance ? 1250 : 1330));
			this.teammateCommand.Set((short)commandId, true);
		}

		// Token: 0x06007C23 RID: 31779 RVA: 0x0039B748 File Offset: 0x00399948
		public void SetSelected(ViewShixiangUpgradeTeammateCommand parent, bool selected)
		{
			bool isAdvance = parent.IsCommandAdvance(this._commandId);
			if (selected)
			{
				this.teammateCommand.GetComponent<CImage>().sprite = (isAdvance ? (this._isEquipped ? this.advanceEquippedSelected : this.advanceUnequippedSelected) : (this._isEquipped ? this.normalEquippedSelected : this.normalUnequippedSelected));
			}
			else
			{
				this.teammateCommand.GetComponent<CImage>().sprite = (isAdvance ? (this._isEquipped ? this.advanceEquippedBack : this.advanceUnequippedBack) : (this._isEquipped ? this.normalEquippedBack : this.normalUnequippedBack));
			}
		}

		// Token: 0x06007C24 RID: 31780 RVA: 0x0039B7F2 File Offset: 0x003999F2
		public void PlayParticle()
		{
			this.particle.gameObject.SetActive(true);
			this.particle.Play();
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, delegate
			{
				this.particle.Stop();
				this.particle.gameObject.SetActive(false);
			});
		}

		// Token: 0x06007C25 RID: 31781 RVA: 0x0039B82F File Offset: 0x00399A2F
		private void OnClickCancel()
		{
			this._parent.OnClickDowngrade(-1, this._commandId);
		}

		// Token: 0x06007C26 RID: 31782 RVA: 0x0039B845 File Offset: 0x00399A45
		private void OnSelectCommand()
		{
			this._parent.OnCommandSelect(this._commandId);
		}

		// Token: 0x04005E49 RID: 24137
		private const int NormalWidth = 1330;

		// Token: 0x04005E4A RID: 24138
		private const int AdvanceWidth = 1250;

		// Token: 0x04005E4B RID: 24139
		private const float Duration = 1.5f;

		// Token: 0x04005E4C RID: 24140
		public Sprite normalEquippedBack;

		// Token: 0x04005E4D RID: 24141
		public Sprite advanceEquippedBack;

		// Token: 0x04005E4E RID: 24142
		public Sprite normalUnequippedBack;

		// Token: 0x04005E4F RID: 24143
		public Sprite advanceUnequippedBack;

		// Token: 0x04005E50 RID: 24144
		public Sprite normalEquippedSelected;

		// Token: 0x04005E51 RID: 24145
		public Sprite advanceEquippedSelected;

		// Token: 0x04005E52 RID: 24146
		public Sprite normalUnequippedSelected;

		// Token: 0x04005E53 RID: 24147
		public Sprite advanceUnequippedSelected;

		// Token: 0x04005E54 RID: 24148
		public CButton btnCancel;

		// Token: 0x04005E55 RID: 24149
		public CButton btnSelect;

		// Token: 0x04005E56 RID: 24150
		public Game.Components.Character.TeammateCommandLongItem teammateCommand;

		// Token: 0x04005E57 RID: 24151
		public ParticleSystem particle;

		// Token: 0x04005E58 RID: 24152
		public Transform particles;

		// Token: 0x04005E59 RID: 24153
		private ViewShixiangUpgradeTeammateCommand _parent;

		// Token: 0x04005E5A RID: 24154
		private sbyte _commandId;

		// Token: 0x04005E5B RID: 24155
		private bool _isEquipped;
	}
}
