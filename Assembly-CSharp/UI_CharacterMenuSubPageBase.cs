using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Views.CharacterMenu;
using GameData.GameDataBridge;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public abstract class UI_CharacterMenuSubPageBase : UIBase
{
	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001EB8 RID: 7864 RVA: 0x000DE558 File Offset: 0x000DC758
	// (set) Token: 0x06001EB9 RID: 7865 RVA: 0x000DE560 File Offset: 0x000DC760
	private protected bool IsLoading { protected get; private set; }

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06001EBA RID: 7866
	public abstract LanguageKey TitleKey { get; }

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06001EBB RID: 7867 RVA: 0x000DE569 File Offset: 0x000DC769
	public virtual bool ShowCharacterList
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06001EBC RID: 7868 RVA: 0x000DE56C File Offset: 0x000DC76C
	public virtual bool CharacterListItemShowFavor
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06001EBD RID: 7869 RVA: 0x000DE56F File Offset: 0x000DC76F
	public virtual bool CharacterListItemShowHappiness
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001EBE RID: 7870 RVA: 0x000DE572 File Offset: 0x000DC772
	public virtual bool ShowBaseAttribute
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06001EBF RID: 7871 RVA: 0x000DE575 File Offset: 0x000DC775
	public ViewCharacterMenu CharacterMenu
	{
		get
		{
			return UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
		}
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000DE584 File Offset: 0x000DC784
	public bool SetCharId(int charId)
	{
		int prevCharId = this._charId;
		bool charIdChange = charId != this._charId && (charId >= 0 || this._charId < 0);
		bool flag = charIdChange;
		if (flag)
		{
			base.ClearAsyncMethodCalls();
			this._expectedCharId = charId;
		}
		this._charId = charId;
		this.OnCurrentCharacterChange(prevCharId);
		return charIdChange;
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000DE5E0 File Offset: 0x000DC7E0
	public sealed override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		bool flag = this._charId != this._expectedCharId;
		if (!flag)
		{
			this.OnNotifyGameDataFiltered(notifications);
		}
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x000DE60D File Offset: 0x000DC80D
	protected virtual void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
	{
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x000DE610 File Offset: 0x000DC810
	public override void NotifyUIHide()
	{
		this.ClearLoadingStateOnHide();
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000DE61A File Offset: 0x000DC81A
	public override void PlayAudioOut()
	{
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000DE61D File Offset: 0x000DC81D
	public virtual void OnCurrentCharacterChange(int prevCharacterId)
	{
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000DE620 File Offset: 0x000DC820
	public virtual bool CanItemDropToCharacter(int characterId, object data)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		return this.CharacterMenu.CurCharacterId == taiwuCharId || characterId == taiwuCharId;
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000DE654 File Offset: 0x000DC854
	public virtual bool CanSubpageShow(ECharacterSubPage subPageIndex)
	{
		return true;
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x000DE667 File Offset: 0x000DC867
	public virtual void OnSwitchToSubpage(int subPageIndex)
	{
		this.CurTabIndex = subPageIndex;
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000DE671 File Offset: 0x000DC871
	public virtual void OnSubpageVisible()
	{
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000DE680 File Offset: 0x000DC880
	public virtual void OnSubpageInVisible()
	{
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000DE683 File Offset: 0x000DC883
	public virtual void OnEnterFocusMode(Transform maskTrans)
	{
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000DE686 File Offset: 0x000DC886
	public virtual void OnExitFocusMode()
	{
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x000DE68C File Offset: 0x000DC88C
	public virtual bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
	{
		return false;
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x000DE6A0 File Offset: 0x000DC8A0
	protected virtual void SetLoadingState(bool loading)
	{
		if (loading)
		{
			bool flag = this._delayHideLoadingCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._delayHideLoadingCoroutine);
				this._delayHideLoadingCoroutine = null;
			}
			this._loadingShowStartTime = Time.unscaledTime;
			bool isLoading = this.IsLoading;
			if (!isLoading)
			{
				this.IsLoading = true;
				this.ApplyLoadingState(true);
			}
		}
		else
		{
			bool flag2 = !this.IsLoading;
			if (!flag2)
			{
				bool flag3 = this._delayHideLoadingCoroutine != null;
				if (!flag3)
				{
					float minDuration = Mathf.Max(0f, this.loadingMinVisibleDuration);
					float elapsed = Time.unscaledTime - this._loadingShowStartTime;
					float remain = minDuration - elapsed;
					bool flag4 = remain <= 0f;
					if (flag4)
					{
						this.SetLoadingStateImmediateFalse();
					}
					else
					{
						bool flag5 = !base.isActiveAndEnabled || !base.gameObject.activeInHierarchy;
						if (flag5)
						{
							this.SetLoadingStateImmediateFalse();
						}
						else
						{
							this._delayHideLoadingCoroutine = base.StartCoroutine(this.DelayHideLoading(remain));
						}
					}
				}
			}
		}
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x000DE7AE File Offset: 0x000DC9AE
	protected virtual void OnDisable()
	{
		this.ClearLoadingStateOnHide();
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x000DE7B8 File Offset: 0x000DC9B8
	private void ClearLoadingStateOnHide()
	{
		bool flag = this._delayHideLoadingCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._delayHideLoadingCoroutine);
			this._delayHideLoadingCoroutine = null;
		}
		this.SetLoadingStateImmediateFalse();
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000DE7F0 File Offset: 0x000DC9F0
	private IEnumerator DelayHideLoading(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		this._delayHideLoadingCoroutine = null;
		bool isLoading = this.IsLoading;
		if (isLoading)
		{
			this.SetLoadingStateImmediateFalse();
		}
		yield break;
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000DE806 File Offset: 0x000DCA06
	private void SetLoadingStateImmediateFalse()
	{
		this.IsLoading = false;
		this.ApplyLoadingState(false);
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000DE81C File Offset: 0x000DCA1C
	private void ApplyLoadingState(bool loading)
	{
		bool flag = this.loadingAnimation != null;
		if (flag)
		{
			this.loadingAnimation.gameObject.SetActive(loading);
		}
		bool flag2 = this.contentRoot != null;
		if (flag2)
		{
			if (loading)
			{
				this._contentRootOriginalPosition = this.contentRoot.anchoredPosition;
				this.contentRoot.anchoredPosition = new Vector2(10000f, 10000f);
			}
			else
			{
				this.contentRoot.anchoredPosition = this._contentRootOriginalPosition;
			}
		}
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x000DE8A8 File Offset: 0x000DCAA8
	public void ResetMoveInAnim()
	{
		bool flag = this.moveIn == null;
		if (!flag)
		{
			DOTweenAnimation[] animations = this.moveIn.GetComponents<DOTweenAnimation>();
			foreach (DOTweenAnimation t in animations)
			{
				t.DOComplete();
			}
		}
	}

	// Token: 0x04001744 RID: 5956
	[NonSerialized]
	public int Key;

	// Token: 0x04001745 RID: 5957
	[Header("Loading 状态")]
	[SerializeField]
	protected LoadingAnimation loadingAnimation;

	// Token: 0x04001746 RID: 5958
	[SerializeField]
	protected RectTransform contentRoot;

	// Token: 0x04001747 RID: 5959
	[SerializeField]
	protected float loadingMinVisibleDuration = 0.1f;

	// Token: 0x04001748 RID: 5960
	[SerializeField]
	public DOTweenAnimation moveIn;

	// Token: 0x0400174A RID: 5962
	private Vector2 _contentRootOriginalPosition;

	// Token: 0x0400174B RID: 5963
	private float _loadingShowStartTime;

	// Token: 0x0400174C RID: 5964
	private Coroutine _delayHideLoadingCoroutine;

	// Token: 0x0400174D RID: 5965
	[Header("子页签名称列表")]
	public List<string> SubPageDisplayNameList;

	// Token: 0x0400174E RID: 5966
	public int CurTabIndex;

	// Token: 0x0400174F RID: 5967
	private int _charId = -2;

	// Token: 0x04001750 RID: 5968
	private int _expectedCharId = int.MinValue;

	// Token: 0x04001751 RID: 5969
	private const float HiddenOffset = 10000f;
}
