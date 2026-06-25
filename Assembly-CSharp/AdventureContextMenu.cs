using System;
using System.Collections.Generic;
using Game.Views.Legacy.AdventureEditor.Migrate;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class AdventureContextMenu : MonoBehaviour
{
	// Token: 0x06001409 RID: 5129 RVA: 0x0007D58C File Offset: 0x0007B78C
	private void Awake()
	{
		this._canvas = base.GetComponentInParent<Canvas>();
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0007D59C File Offset: 0x0007B79C
	public void RefreshFirstLevelContextMenu(List<AdventureContextMenuItemData> actionList)
	{
		try
		{
			this.RefreshPosition();
			float backHeight = (float)actionList.Count * 50f;
			this.firstMenuHolder.SetHeight(backHeight);
			this.RefreshChild(this.firstMenuHolder, actionList);
			base.gameObject.SetActive(true);
			this.secondMenuHolder.gameObject.SetActive(false);
		}
		catch (Exception e)
		{
			Debug.LogError("刷新上下文菜单时发生错误: " + e.Message);
		}
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0007D628 File Offset: 0x0007B828
	private void RefreshSecondLevelContextMenu(List<AdventureContextMenuItemData> actionList)
	{
		try
		{
			float backHeight = (float)actionList.Count * 50f;
			this.secondMenuHolder.SetHeight(backHeight);
			this.RefreshChild(this.secondMenuHolder, actionList);
			this.secondMenuHolder.gameObject.SetActive(true);
		}
		catch (Exception e)
		{
			Debug.LogError("刷新上下文菜单时发生错误: " + e.Message);
		}
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x0007D6A0 File Offset: 0x0007B8A0
	public void RefreshSecondLevelItemName(int index, string itemName)
	{
		this.RefreshChildReferName(this.secondMenuHolder, index, itemName);
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x0007D6B4 File Offset: 0x0007B8B4
	private void RefreshChild(RectTransform root, List<AdventureContextMenuItemData> actionList)
	{
		try
		{
			while (root.childCount < actionList.Count)
			{
				Transform newItem = Object.Instantiate<Transform>(root.GetChild(0), root, false);
			}
			for (int i = root.childCount - 1; i >= actionList.Count; i--)
			{
				root.GetChild(i).gameObject.SetActive(false);
			}
			int index = 0;
			foreach (AdventureContextMenuItemData itemData in actionList)
			{
				this.RefreshChildRefer(root, index, itemData);
				index++;
			}
		}
		catch (Exception e)
		{
			Debug.LogError("刷新子项时发生错误: " + e.Message);
		}
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0007D79C File Offset: 0x0007B99C
	private void RefreshChildRefer(RectTransform root, int index, AdventureContextMenuItemData itemData)
	{
		Transform child = root.GetChild(index);
		child.gameObject.SetActive(true);
		AdventureContextMenuItem childRefers = child.GetComponent<AdventureContextMenuItem>();
		childRefers.contextMenuItemText.SetText(itemData.ItemName, true);
		childRefers.contextMenuItemBtn.ClearAndAddListener(delegate
		{
			Action itemAction = itemData.ItemAction;
			if (itemAction != null)
			{
				itemAction();
			}
			List<AdventureContextMenuItemData> childItems = itemData.ChildItems;
			bool flag = childItems != null && childItems.Count > 0;
			if (flag)
			{
				this.RefreshSecondLevelContextMenu(itemData.ChildItems);
			}
			bool hideContextMenu = itemData.HideContextMenu;
			if (hideContextMenu)
			{
				this.gameObject.SetActive(false);
			}
		});
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0007D80C File Offset: 0x0007BA0C
	private void RefreshChildReferName(RectTransform root, int index, string itemName)
	{
		Transform child = root.GetChild(index);
		AdventureContextMenuItem childRefers = child.GetComponent<AdventureContextMenuItem>();
		childRefers.contextMenuItemText.SetText(itemName, true);
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x0007D838 File Offset: 0x0007BA38
	private void RefreshPosition()
	{
		this._canvas = base.GetComponentInParent<Canvas>();
		Camera uiCamera = (this._canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : this._canvas.worldCamera;
		Vector3 mousePos = Input.mousePosition;
		Vector2 mouseSize = ConchShipCursor.Instance.GetCursorRectTransform().rect.size;
		Vector2 localPos;
		bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(base.GetComponent<RectTransform>().parent.GetComponent<RectTransform>(), new Vector2(mousePos.x - mouseSize.x / 2f, mousePos.y + mouseSize.y / 2f) + this.offsetWhenRefresh, uiCamera, out localPos);
		bool flag = success;
		if (flag)
		{
			base.GetComponent<RectTransform>().localPosition = localPos + this._menuOffset;
		}
	}

	// Token: 0x040010D8 RID: 4312
	private const float ItemHeight = 50f;

	// Token: 0x040010D9 RID: 4313
	[SerializeField]
	private Vector2 offsetWhenRefresh;

	// Token: 0x040010DA RID: 4314
	[SerializeField]
	private RectTransform firstMenuHolder;

	// Token: 0x040010DB RID: 4315
	[SerializeField]
	private RectTransform secondMenuHolder;

	// Token: 0x040010DC RID: 4316
	private readonly Vector2 _menuOffset = new Vector2(120f, -50f);

	// Token: 0x040010DD RID: 4317
	private Canvas _canvas;
}
