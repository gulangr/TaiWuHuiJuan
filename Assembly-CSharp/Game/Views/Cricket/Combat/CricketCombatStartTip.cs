using System;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD5 RID: 2773
	public class CricketCombatStartTip : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F09 RID: 3849
		// (get) Token: 0x06008883 RID: 34947 RVA: 0x003F4B0C File Offset: 0x003F2D0C
		// (set) Token: 0x06008884 RID: 34948 RVA: 0x003F4B14 File Offset: 0x003F2D14
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x06008885 RID: 34949 RVA: 0x003F4B20 File Offset: 0x003F2D20
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this._isHoveringStartButton = false;
				this.HideTip();
			}
			else
			{
				bool flag2 = type == ECricketCombatGlobalEventType.SelfCricketChanged;
				if (flag2)
				{
					this.RefreshTip();
				}
				else
				{
					bool flag3 = type == ECricketCombatGlobalEventType.CombatStatusChanged;
					if (flag3)
					{
						this.RefreshTip();
					}
					else
					{
						bool flag4 = type == ECricketCombatGlobalEventType.StartButtonHoverChanged;
						if (flag4)
						{
							bool isHovering;
							this._isHoveringStartButton = (argBox != null && argBox.Get("IsHovering", out isHovering) && isHovering);
							this.RefreshTip();
						}
					}
				}
			}
		}

		// Token: 0x06008886 RID: 34950 RVA: 0x003F4B98 File Offset: 0x003F2D98
		private void RefreshTip()
		{
			bool flag = CricketCombatKit.Board.Status > ECricketCombatStatus.Preparing;
			if (flag)
			{
				this.HideTip();
			}
			else
			{
				bool flag2 = this.CanStartCombat();
				if (flag2)
				{
					this.HideTip();
				}
				else
				{
					this.ShowTip();
					this.tipText.text = this.GetTipText();
				}
			}
		}

		// Token: 0x06008887 RID: 34951 RVA: 0x003F4BF0 File Offset: 0x003F2DF0
		private bool CanStartCombat()
		{
			return CricketFairCombatHelper.CanStartCombat(CricketCombatKit.Board.SelfCrickets, CricketCombatKit.Board.EnemyWager);
		}

		// Token: 0x06008888 RID: 34952 RVA: 0x003F4C1C File Offset: 0x003F2E1C
		private string GetTipText()
		{
			bool flag = !CricketFairCombatHelper.HasEnoughCrickets(CricketCombatKit.Board.SelfCrickets);
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_CricketCombat_StartTip_NotEnoughCricket);
			}
			else
			{
				bool flag2 = CricketFairCombatHelper.IsPointExceeded(CricketCombatKit.Board.SelfCrickets, CricketCombatKit.Board.EnemyWager);
				if (flag2)
				{
					result = LocalStringManager.Get(LanguageKey.LK_CricketCombat_StartTip_PointExceeded);
				}
				else
				{
					result = string.Empty;
				}
			}
			return result;
		}

		// Token: 0x06008889 RID: 34953 RVA: 0x003F4C80 File Offset: 0x003F2E80
		private void ShowTip()
		{
			this.tipRoot.SetActive(true);
		}

		// Token: 0x0600888A RID: 34954 RVA: 0x003F4C90 File Offset: 0x003F2E90
		private void HideTip()
		{
			this.tipRoot.SetActive(false);
		}

		// Token: 0x04006892 RID: 26770
		[SerializeField]
		private GameObject tipRoot;

		// Token: 0x04006893 RID: 26771
		[SerializeField]
		private TextMeshProUGUI tipText;

		// Token: 0x04006894 RID: 26772
		private bool _isHoveringStartButton;
	}
}
