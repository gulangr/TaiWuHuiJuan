using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD4 RID: 2772
	public class CricketCombatStartButton : MonoBehaviour, ICricketCombatComponent, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000F06 RID: 3846
		// (get) Token: 0x06008876 RID: 34934 RVA: 0x003F48C3 File Offset: 0x003F2AC3
		// (set) Token: 0x06008877 RID: 34935 RVA: 0x003F48CB File Offset: 0x003F2ACB
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x17000F07 RID: 3847
		// (get) Token: 0x06008878 RID: 34936 RVA: 0x003F48D4 File Offset: 0x003F2AD4
		public bool IsInteractable
		{
			get
			{
				return base.gameObject.activeSelf && this.Button.interactable;
			}
		}

		// Token: 0x17000F08 RID: 3848
		// (get) Token: 0x06008879 RID: 34937 RVA: 0x003F48F4 File Offset: 0x003F2AF4
		private CButton Button
		{
			get
			{
				CButton result;
				if ((result = this._button) == null)
				{
					result = (this._button = base.GetComponent<CButton>());
				}
				return result;
			}
		}

		// Token: 0x0600887A RID: 34938 RVA: 0x003F491C File Offset: 0x003F2B1C
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.Refresh();
			}
			else
			{
				bool flag2 = type == ECricketCombatGlobalEventType.SelfCricketChanged;
				if (flag2)
				{
					this.Refresh();
				}
				else
				{
					bool flag3 = type == ECricketCombatGlobalEventType.CombatStatusChanged;
					if (flag3)
					{
						bool flag4 = CricketCombatKit.Board.Status == ECricketCombatStatus.Combating;
						if (flag4)
						{
							base.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0600887B RID: 34939 RVA: 0x003F4974 File Offset: 0x003F2B74
		private void Refresh()
		{
			bool canStart = this.CanStartCombat();
			this.Button.interactable = canStart;
			this.startCombatShortcutKeyTip.SetActive(canStart);
			string lang = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			this.iconImage.SetTexture(string.Format("{0}{1}_{2}", "ui9_tex_cricketcombat_title_", canStart ? 0 : 1, lang));
			this.stateImage.texture = this.GetCurrentTexture(canStart);
		}

		// Token: 0x0600887C RID: 34940 RVA: 0x003F49F0 File Offset: 0x003F2BF0
		private bool CanStartCombat()
		{
			return CricketFairCombatHelper.CanStartCombat(CricketCombatKit.Board.SelfCrickets, CricketCombatKit.Board.EnemyWager);
		}

		// Token: 0x0600887D RID: 34941 RVA: 0x003F4A1B File Offset: 0x003F2C1B
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._isHovering = true;
			this.Refresh();
			this.NotifyHoverStateChanged();
		}

		// Token: 0x0600887E RID: 34942 RVA: 0x003F4A33 File Offset: 0x003F2C33
		public void OnPointerExit(PointerEventData eventData)
		{
			this._isHovering = false;
			this.Refresh();
			this.NotifyHoverStateChanged();
		}

		// Token: 0x0600887F RID: 34943 RVA: 0x003F4A4C File Offset: 0x003F2C4C
		private void OnDisable()
		{
			bool isHovering = this._isHovering;
			if (isHovering)
			{
				this._isHovering = false;
				this.NotifyHoverStateChanged();
			}
			this._isHovering = false;
		}

		// Token: 0x06008880 RID: 34944 RVA: 0x003F4A7C File Offset: 0x003F2C7C
		private Texture2D GetCurrentTexture(bool canStart)
		{
			bool flag = !canStart;
			Texture2D result;
			if (flag)
			{
				result = this.disabledTexture;
			}
			else
			{
				result = (this._isHovering ? this.hoverTexture : this.normalTexture);
			}
			return result;
		}

		// Token: 0x06008881 RID: 34945 RVA: 0x003F4AB8 File Offset: 0x003F2CB8
		private void NotifyHoverStateChanged()
		{
			bool flag = this.Handler == null;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("IsHovering", this._isHovering);
				this.Handler.OnEvent(ECricketCombatGlobalEventType.StartButtonHoverChanged, argBox);
				EasyPool.Free<ArgumentBox>(argBox);
			}
		}

		// Token: 0x04006889 RID: 26761
		[SerializeField]
		private CRawImage iconImage;

		// Token: 0x0400688A RID: 26762
		[SerializeField]
		private CRawImage stateImage;

		// Token: 0x0400688B RID: 26763
		[SerializeField]
		private Texture2D normalTexture;

		// Token: 0x0400688C RID: 26764
		[SerializeField]
		private Texture2D hoverTexture;

		// Token: 0x0400688D RID: 26765
		[SerializeField]
		private Texture2D disabledTexture;

		// Token: 0x0400688E RID: 26766
		[SerializeField]
		private GameObject startCombatShortcutKeyTip;

		// Token: 0x04006890 RID: 26768
		private CButton _button;

		// Token: 0x04006891 RID: 26769
		private bool _isHovering;
	}
}
