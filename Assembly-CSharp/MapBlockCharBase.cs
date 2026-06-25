using System;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003C6 RID: 966
public abstract class MapBlockCharBase : MonoBehaviour
{
	// Token: 0x06003A4C RID: 14924 RVA: 0x001DA757 File Offset: 0x001D8957
	protected static string GetIconGrave(int level)
	{
		return string.Format("blockchar_icon_fenmu_{0}", level);
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x001DA769 File Offset: 0x001D8969
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayData displayData)
	{
		this._canInteract = canInteract;
		this.MapBlock = mapBlock;
		this.CharId = -1;
		this.DisplayData = displayData;
	}

	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x06003A4E RID: 14926 RVA: 0x001DA788 File Offset: 0x001D8988
	private WorldMapModel WorldMapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x06003A4F RID: 14927 RVA: 0x001DA78F File Offset: 0x001D898F
	protected bool IsMoving
	{
		get
		{
			return this.WorldMapModel.TaiwuMoveState > WorldMapModel.MoveState.Idle;
		}
	}

	// Token: 0x170005F0 RID: 1520
	// (get) Token: 0x06003A50 RID: 14928 RVA: 0x001DA79F File Offset: 0x001D899F
	protected int CurrentBlockId
	{
		get
		{
			return (int)this.WorldMapModel.CurrentBlockId;
		}
	}

	// Token: 0x06003A51 RID: 14929
	protected abstract void RefreshName();

	// Token: 0x06003A52 RID: 14930
	protected abstract void RefreshOrganization();

	// Token: 0x06003A53 RID: 14931 RVA: 0x001DA7AC File Offset: 0x001D89AC
	private void RefreshInteraction()
	{
		PointerTrigger pointerTrigger = this.button.GetComponent<PointerTrigger>();
		SelectableCursorTriggerObsolete selectableCursorTrigger = this.button.GetComponent<SelectableCursorTriggerObsolete>();
		this.button.interactable = (this._canInteract && this.CurrentBlockId == (int)this.MapBlock.BlockId);
		this.button.onClick.RemoveAllListeners();
		pointerTrigger.EnterEvent.RemoveAllListeners();
		pointerTrigger.ExitEvent.RemoveAllListeners();
		if (selectableCursorTrigger != null)
		{
			selectableCursorTrigger.AddCursorEvent();
		}
		bool flag = !this._canInteract;
		if (!flag)
		{
			this.button.ClearAndAddListener(new Action(this.OnClickButton));
		}
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x001DA85C File Offset: 0x001D8A5C
	protected virtual void OnClickButton()
	{
		bool flag = this.WorldMapModel.ShowingAreaId != this.WorldMapModel.CurrentAreaId;
		if (flag)
		{
			GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", false));
		}
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x001DA8A6 File Offset: 0x001D8AA6
	protected virtual void Refresh()
	{
		this.RefreshName();
		this.RefreshOrganization();
		this.RefreshInteraction();
	}

	// Token: 0x06003A56 RID: 14934 RVA: 0x001DA8C0 File Offset: 0x001D8AC0
	public virtual void OnHide()
	{
		PointerTrigger pointerTrigger = this.button.GetComponent<PointerTrigger>();
		UnityEvent enterEvent = pointerTrigger.EnterEvent;
		if (enterEvent != null)
		{
			enterEvent.RemoveAllListeners();
		}
		UnityEvent exitEvent = pointerTrigger.ExitEvent;
		if (exitEvent != null)
		{
			exitEvent.RemoveAllListeners();
		}
	}

	// Token: 0x04002A0B RID: 10763
	[SerializeField]
	protected CButtonObsolete button;

	// Token: 0x04002A0C RID: 10764
	[SerializeField]
	protected TextMeshProUGUI nameText;

	// Token: 0x04002A0D RID: 10765
	[SerializeField]
	protected TextMeshProUGUI organizationText;

	// Token: 0x04002A0E RID: 10766
	[SerializeField]
	protected CImage organizationIcon;

	// Token: 0x04002A0F RID: 10767
	protected static readonly string NpcAvatarTexturePath = "NpcFace/SmallFace";

	// Token: 0x04002A10 RID: 10768
	protected const string IconEnemy = "blockchar_icon_diren";

	// Token: 0x04002A11 RID: 10769
	protected const string IconXiangshu = "map_icon_xiangshu";

	// Token: 0x04002A12 RID: 10770
	protected const string IconZizhu = "map_icon_zizhu";

	// Token: 0x04002A13 RID: 10771
	protected const string IconSpecial = "map_icon_teshu";

	// Token: 0x04002A14 RID: 10772
	protected const string IconCaravan = "sp_icon_shanghui";

	// Token: 0x04002A15 RID: 10773
	private bool _canInteract;

	// Token: 0x04002A16 RID: 10774
	protected MapBlockData MapBlock;

	// Token: 0x04002A17 RID: 10775
	protected int CharId = -1;

	// Token: 0x04002A18 RID: 10776
	protected CharacterDisplayData DisplayData;
}
