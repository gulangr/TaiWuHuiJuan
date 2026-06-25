using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Creation;
using GameData.Domains.World;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007EA RID: 2026
	public abstract class NewGameSubPage : MonoBehaviour
	{
		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x060062B4 RID: 25268 RVA: 0x002D2EA3 File Offset: 0x002D10A3
		public Dictionary<string, string> CreationInfoMap
		{
			get
			{
				return this.parent.CreationInfoMap;
			}
		}

		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x060062B5 RID: 25269
		public abstract DialogCmd StartGameCheck { get; }

		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x060062B6 RID: 25270
		// (set) Token: 0x060062B7 RID: 25271
		public abstract bool StartGameChecked { get; set; }

		// Token: 0x060062B8 RID: 25272 RVA: 0x002D2EB0 File Offset: 0x002D10B0
		public void InitSetPageIndex(int index)
		{
			this.pageIndex = index;
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x002D2EB9 File Offset: 0x002D10B9
		public void FocusToPage()
		{
			this.parent.CurrentPage = this.pageIndex;
		}

		// Token: 0x060062BA RID: 25274
		public abstract void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo);

		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x060062BB RID: 25275 RVA: 0x002D2ECD File Offset: 0x002D10CD
		public virtual string DisableEnterGameReason
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x002D2ED4 File Offset: 0x002D10D4
		public void RefreshDisableEnterGameReason()
		{
			this.parent.RefreshDisableEnterGameReason();
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x002D2EE2 File Offset: 0x002D10E2
		protected virtual void Awake()
		{
			this.toggle.onValueChanged.ResetListener(new Action<bool>(this.OnValueChange));
		}

		// Token: 0x060062BE RID: 25278 RVA: 0x002D2F03 File Offset: 0x002D1103
		public virtual void Init()
		{
		}

		// Token: 0x060062BF RID: 25279 RVA: 0x002D2F06 File Offset: 0x002D1106
		protected virtual void OnValueChange(bool isOn)
		{
			base.gameObject.SetActive(isOn);
		}

		// Token: 0x060062C0 RID: 25280 RVA: 0x002D2F18 File Offset: 0x002D1118
		public T GetTargetSubPage<T>(ViewNewGame.ENewGameSubType subType) where T : NewGameSubPage
		{
			bool flag = this.parent == null;
			T result;
			if (flag)
			{
				Debug.LogError("Parent is null !");
				result = default(T);
			}
			else
			{
				result = this.parent.GetTargetSubPage<T>(subType);
			}
			return result;
		}

		// Token: 0x060062C1 RID: 25281 RVA: 0x002D2F60 File Offset: 0x002D1160
		public CToggle GetTargetSubToggle(ViewNewGame.ENewGameSubType subType)
		{
			bool flag = this.parent == null;
			CToggle result;
			if (flag)
			{
				Debug.LogError("Parent is null !");
				result = null;
			}
			else
			{
				result = this.parent.GetTargetSubToggle(subType);
			}
			return result;
		}

		// Token: 0x040044CB RID: 17611
		[SerializeField]
		protected CToggle toggle;

		// Token: 0x040044CC RID: 17612
		[SerializeField]
		protected ViewNewGame parent;

		// Token: 0x040044CD RID: 17613
		[SerializeField]
		private int pageIndex = -1;
	}
}
