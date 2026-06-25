using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AiEditor;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Adventure;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Utilities;
using SubEditor.AdventureCommonRefersListEditor;
using TMPro;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class AdventureEditorMainArea : MonoBehaviour, ISelectAndDragHandler, IAdventureEditorElementHandler, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>, IAdventureEditorBlockElementHandler
{
	// Token: 0x17000265 RID: 613
	// (get) Token: 0x0600154A RID: 5450 RVA: 0x00083A6B File Offset: 0x00081C6B
	private bool IsSimulateMode
	{
		get
		{
			return AdventureEditorKit.BlackBoard.ViewMode == EBlockViewMode.Simulate;
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x0600154B RID: 5451 RVA: 0x00083A7A File Offset: 0x00081C7A
	int ISelectAndDragHandler.SelectedComponentCount
	{
		get
		{
			return this.IsSimulateMode ? this._selectedMicros.Count : this._selectedBlocks.Count;
		}
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00083A9C File Offset: 0x00081C9C
	bool ISelectAndDragHandler.IsSelectedComponent(ISelectAndDragComponent component)
	{
		bool isSimulateMode = this.IsSimulateMode;
		bool result;
		if (isSimulateMode)
		{
			result = this._selectedMicros.Contains(component as AdventureEditorMicro);
		}
		else
		{
			result = this._selectedBlocks.Contains(component as AdventureEditorBlock);
		}
		return result;
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x0600154D RID: 5453 RVA: 0x00083AE0 File Offset: 0x00081CE0
	IEnumerable<ISelectAndDragComponent> ISelectAndDragHandler.SelectedComponents
	{
		get
		{
			IEnumerable<ISelectAndDragComponent> result;
			if (!this.IsSimulateMode)
			{
				IEnumerable<ISelectAndDragComponent> enumerable = this._selectedBlocks;
				result = enumerable;
			}
			else
			{
				IEnumerable<ISelectAndDragComponent> enumerable = this._selectedMicros;
				result = enumerable;
			}
			return result;
		}
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00083B08 File Offset: 0x00081D08
	void ISelectAndDragHandler.Select(ISelectAndDragComponent component)
	{
		bool isSimulateMode = this.IsSimulateMode;
		if (isSimulateMode)
		{
			AdventureEditorMicro micro = component as AdventureEditorMicro;
			bool flag = micro != null;
			if (flag)
			{
				bool getControlKeyDirect = AdventureEditorKit.GetControlKeyDirect;
				if (getControlKeyDirect)
				{
					this.SelectVolumeByMicro(micro);
				}
				else
				{
					this.SelectMicro(micro);
				}
			}
		}
		else
		{
			AdventureEditorBlock block = component as AdventureEditorBlock;
			bool flag2 = block == null;
			if (flag2)
			{
				Debug.LogWarning(string.Format("{0}", block));
			}
			else
			{
				bool getControlKeyDirect2 = AdventureEditorKit.GetControlKeyDirect;
				if (getControlKeyDirect2)
				{
					for (int i = 0; i < 9; i++)
					{
						this.SelectBlock(this._blockMappings[block.Index.SetI(i)]);
					}
				}
				else
				{
					this.SelectBlock(block);
				}
			}
		}
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x00083BDC File Offset: 0x00081DDC
	void ISelectAndDragHandler.Unselect(ISelectAndDragComponent component)
	{
		bool isSimulateMode = this.IsSimulateMode;
		if (isSimulateMode)
		{
			AdventureEditorMicro micro = component as AdventureEditorMicro;
			bool flag = micro != null;
			if (flag)
			{
				bool getControlKeyDirect = AdventureEditorKit.GetControlKeyDirect;
				if (getControlKeyDirect)
				{
					this.UnselectVolumeByMicro(micro);
				}
				else
				{
					this.UnselectMicro(micro);
				}
			}
		}
		else
		{
			AdventureEditorBlock block = (AdventureEditorBlock)component;
			bool getControlKeyDirect2 = AdventureEditorKit.GetControlKeyDirect;
			if (getControlKeyDirect2)
			{
				for (int i = 0; i < 9; i++)
				{
					this.UnselectBlock(this._blockMappings[block.Index.SetI(i)]);
				}
			}
			else
			{
				this.UnselectBlock(block);
			}
		}
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x00083C8C File Offset: 0x00081E8C
	void ISelectAndDragHandler.SelectEmpty()
	{
		bool isSimulateMode = this.IsSimulateMode;
		if (isSimulateMode)
		{
			foreach (AdventureEditorMicro micro in this._selectedMicros)
			{
				micro.Unselect();
			}
			this._selectedMicros.Clear();
		}
		else
		{
			foreach (AdventureEditorBlock block in this._selectedBlocks)
			{
				block.Unselect();
			}
			this._selectedBlocks.Clear();
		}
		this.HideContextMenu();
		this.HidePropertyPanel();
		this.HideElementsPanel();
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x00083D68 File Offset: 0x00081F68
	void ISelectAndDragHandler.BeginMultiSelect(Vector2 startPos)
	{
		this.multiSelectRect.gameObject.SetActive(true);
		this.multiSelectRect.Set(startPos, startPos);
		this.HideContextMenu();
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x00083D92 File Offset: 0x00081F92
	void ISelectAndDragHandler.OnMultiSelect(Vector2 startPos, Vector2 curPos)
	{
		this.multiSelectRect.Set(startPos, curPos);
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00083DA3 File Offset: 0x00081FA3
	void ISelectAndDragHandler.EndMultiSelect()
	{
		this.multiSelectRect.gameObject.SetActive(false);
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x00083DB8 File Offset: 0x00081FB8
	void ISelectAndDragHandler.BeginDrag()
	{
		this.HideContextMenu();
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x00083DC2 File Offset: 0x00081FC2
	void ISelectAndDragHandler.EndDrag(ISelectAndDragComponent component, Vector2 startPos, Vector2 endPos)
	{
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x00083DC8 File Offset: 0x00081FC8
	void ISelectAndDragHandler.ActionContext()
	{
		List<AdventureContextMenuItemData> items = new List<AdventureContextMenuItemData>();
		AdventureContextMenuItemData adventureContextMenuItemData = new AdventureContextMenuItemData();
		adventureContextMenuItemData.ItemName = LanguageKey.LK_Adventure_CtxMenu_Redo.Tr();
		adventureContextMenuItemData.ItemAction = delegate()
		{
			bool flag = AdventureEditorKit.BlackBoard.Evolve();
			if (flag)
			{
				GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusEvolved, null);
			}
		};
		adventureContextMenuItemData.GroupIndex = 0;
		AdventureContextMenuItemData redo = adventureContextMenuItemData;
		items.Add(redo);
		AdventureContextMenuItemData adventureContextMenuItemData2 = new AdventureContextMenuItemData();
		adventureContextMenuItemData2.ItemName = LanguageKey.LK_Adventure_CtxMenu_Undo.Tr();
		adventureContextMenuItemData2.ItemAction = delegate()
		{
			bool flag = AdventureEditorKit.BlackBoard.Revert();
			if (flag)
			{
				GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusReverted, null);
			}
		};
		adventureContextMenuItemData2.GroupIndex = 0;
		AdventureContextMenuItemData undo = adventureContextMenuItemData2;
		items.Add(undo);
		this.adventureContextMenu.RefreshFirstLevelContextMenu(items);
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x00083E80 File Offset: 0x00082080
	void ISelectAndDragHandler.ActionComponents()
	{
		List<AdventureContextMenuItemData> items = new List<AdventureContextMenuItemData>();
		bool hasSelection = this.IsSimulateMode ? (this._selectedMicros.Count > 0) : (this._selectedBlocks.Count > 0);
		bool flag = hasSelection;
		if (flag)
		{
			AdventureContextMenuItemData blockTypeSetData = this.GetBlockTypeContextMenuItemData();
			items.Add(blockTypeSetData);
			AdventureContextMenuItemData groupSetData = this.GetGroupContextMenuItemData();
			items.Add(groupSetData);
		}
		this.adventureContextMenu.RefreshFirstLevelContextMenu(items);
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001558 RID: 5464 RVA: 0x00083EEF File Offset: 0x000820EF
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x00083EF4 File Offset: 0x000820F4
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.All);
		if (flag)
		{
			this.ResetToEmpty();
		}
		bool flag2 = editType.Contains(EAdventureEditType.Size);
		if (flag2)
		{
			this.ReloadSize();
		}
		bool flag3 = editType.Contains(EAdventureEditType.BlockViewMode);
		if (flag3)
		{
			this.ReloadViewMode();
		}
		bool flag4 = editType.Contains(EAdventureEditType.Groups);
		if (flag4)
		{
			this.ReloadGroups();
		}
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x00083F4C File Offset: 0x0008214C
	void IAdventureEditorElementHandler.OnClick(FileInfo file, DirectoryInfo directory)
	{
		AdventureElementSnapshot data;
		bool flag = file == null || !file.Exists || !AdventureElementSnapshot.TryLoadFromFile(file.FullName, out data);
		if (!flag)
		{
			bool flag2 = !AdventureEditorKit.GetControlKeyDirect;
			if (flag2)
			{
				AdventureEditorKit.BlackBoard.MakeEdit<AdventureElementSnapshot>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<AdventureElementSnapshot>(this.AddElementToSelectedBlocks), EAdventureEditType.BlockVisible, data);
			}
			else
			{
				this.elementArea.gameObject.SetActive(true);
				this.elementArea.OnClick(file, directory);
				AdventureEditorElement.CurrentSelected = file;
			}
		}
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x00083FD0 File Offset: 0x000821D0
	void IAdventureEditorElementHandler.OnDoubleClick(FileInfo file, DirectoryInfo directory)
	{
		AdventureElementSnapshot data;
		bool flag = file != null && file.Exists && AdventureElementSnapshot.TryLoadFromFile(file.FullName, out data);
		if (flag)
		{
			AdventureEditorKit.BlackBoard.MakeEdit<AdventureElementSnapshot>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<AdventureElementSnapshot>(this.AddElementToSelectedBlocks), EAdventureEditType.BlockVisible, data);
		}
		bool flag2 = directory != null;
		if (flag2)
		{
			this.container.ChangeDirectory(directory);
		}
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x0008402B File Offset: 0x0008222B
	void IAdventureEditorElementHandler.OnRightClick(FileInfo file, DirectoryInfo directory)
	{
		AdventureEditorElementArea.RightClickMenuProcess(file, directory, this.adventureContextMenu, null, this.elementInfoEditor, this.elementInfoPanel);
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x00084049 File Offset: 0x00082249
	void IAdventureEditorElementHandler.OnSelectEmpty()
	{
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x0008404C File Offset: 0x0008224C
	public IAdventureEditorElementHandler.DragPostProcess OnDragValidate(GameObject dropTarget, FileInfo file, DirectoryInfo directory)
	{
		AdventureEditorBlock targetBlock = dropTarget ? dropTarget.GetComponentInChildren<AdventureEditorBlock>() : null;
		AdventureElementSnapshot data;
		bool flag = targetBlock != null && file != null && file.Exists && AdventureElementSnapshot.TryLoadFromFile(file.FullName, out data);
		IAdventureEditorElementHandler.DragPostProcess result;
		if (flag)
		{
			AdventureEditorKit.BlackBoard.MakeEdit<AdventureBlockIndex, AdventureElementSnapshot>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<AdventureBlockIndex, AdventureElementSnapshot>(this.AddElementToBlock), EAdventureEditType.BlockVisible, targetBlock.Index, data);
			result = IAdventureEditorElementHandler.DragPostProcess.Stay;
		}
		else
		{
			result = IAdventureEditorElementHandler.DragPostProcess.Failed;
		}
		return result;
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x000840C0 File Offset: 0x000822C0
	void IAdventureEditorBlockElementHandler.OnClick(AdventureEditorBlockElementPayload payload, bool isDecorate)
	{
		bool flag = payload.Kind == AdventureEditorBlockElementPayload.PayloadKind.Icon && !string.IsNullOrEmpty(payload.IconName);
		if (flag)
		{
			if (isDecorate)
			{
				AdventureEditorKit.BlackBoard.MakeEdit<string>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<string>(this.AddDecorateIconToSelectedBlocks), EAdventureEditType.BlockViewMode, payload.IconName);
			}
			else
			{
				AdventureEditorKit.BlackBoard.MakeEdit<string>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<string>(this.SetIconToSelectedBlocks), EAdventureEditType.BlockViewMode, payload.IconName);
			}
			this.RefreshPropertyPanel();
		}
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x0008413C File Offset: 0x0008233C
	void IAdventureEditorBlockElementHandler.OnDoubleClick(AdventureEditorBlockElementPayload payload, bool isDecorate)
	{
		bool flag = payload.Kind == AdventureEditorBlockElementPayload.PayloadKind.Icon && !string.IsNullOrEmpty(payload.IconName);
		if (flag)
		{
			if (isDecorate)
			{
				AdventureEditorKit.BlackBoard.MakeEdit<string>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<string>(this.AddDecorateIconToSelectedBlocks), EAdventureEditType.BlockViewMode, payload.IconName);
			}
			else
			{
				AdventureEditorKit.BlackBoard.MakeEdit<string>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<string>(this.SetIconToSelectedBlocks), EAdventureEditType.BlockViewMode, payload.IconName);
			}
		}
		bool flag2 = payload.Kind == AdventureEditorBlockElementPayload.PayloadKind.Directory;
		if (flag2)
		{
			this.blockContainer.ChangeVirtualDirectory(payload.VirtualPathSegments);
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000841D4 File Offset: 0x000823D4
	void IAdventureEditorBlockElementHandler.OnRightClick(AdventureEditorBlockElementPayload payload, bool isDecorate)
	{
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000841D7 File Offset: 0x000823D7
	void IAdventureEditorBlockElementHandler.OnSelectEmpty()
	{
		AdventureEditorBlockElement.CurrentSelectedIconName = null;
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000841E0 File Offset: 0x000823E0
	private void SetIconToSelectedBlocks(AdventureSnapshot snapshot, string iconName)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			block.Icon = iconName;
		}
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00084234 File Offset: 0x00082434
	private void AddDecorateIconToSelectedBlocks(AdventureSnapshot snapshot, string iconName)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			bool flag = !block.Decorates.Contains(iconName);
			if (flag)
			{
				block.Decorates.Add(iconName);
			}
		}
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x000842A0 File Offset: 0x000824A0
	private void Awake()
	{
		this.container.Bind(this);
		this.blockContainer.Bind(this);
		this.selectAndDrag.Bind(this);
		AiEditorSelectAndDrag simulateSelectAndDrag = this.simulateTarget.GetComponent<AiEditorSelectAndDrag>();
		simulateSelectAndDrag.Bind(this);
		this.elementInfoPanel.SetActive(false);
		this.switchToBlockContainerButton.ClearAndAddListener(delegate
		{
			this.blockContainer.gameObject.SetActive(true);
			this.container.gameObject.SetActive(false);
		});
		this.switchToElementContainerButton.ClearAndAddListener(delegate
		{
			this.blockContainer.gameObject.SetActive(false);
			this.container.gameObject.SetActive(true);
		});
		this.pure.onClick.ResetListener(delegate()
		{
			this._adventureEditorPureMode = !this._adventureEditorPureMode;
			GEvent.OnEvent(UiEvents.AdventureEditorPureModeSwitch, EasyPool.Get<ArgumentBox>().Set("PureMode", this._adventureEditorPureMode));
		});
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x00084344 File Offset: 0x00082544
	private void OnEnable()
	{
		this.blockContainer.gameObject.SetActive(false);
		this.container.gameObject.SetActive(true);
		GEvent.Add(UiEvents.AdventureEditorRemakeRefreshPropertyPanel, new GEvent.Callback(this.RefreshPropertyPanel));
		GEvent.Add(UiEvents.AdventureEditorToggleGroupPanel, new GEvent.Callback(this.ToggleGroupManagePanel));
		this.UpdateGroupIndexText();
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x000843B5 File Offset: 0x000825B5
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.AdventureEditorRemakeRefreshPropertyPanel, new GEvent.Callback(this.RefreshPropertyPanel));
		GEvent.Remove(UiEvents.AdventureEditorToggleGroupPanel, new GEvent.Callback(this.ToggleGroupManagePanel));
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x000843F0 File Offset: 0x000825F0
	private void ReloadGroups()
	{
		foreach (AdventureEditorBlock block in this._blockMappings.Values)
		{
			block.Reload();
		}
		bool flag = this.IsSimulateMode && this.simulateTarget != null;
		if (flag)
		{
			foreach (object obj in this.simulateTarget)
			{
				Transform child = (Transform)obj;
				AdventureEditorMicro micro;
				bool flag2 = child.TryGetComponent<AdventureEditorMicro>(out micro);
				if (flag2)
				{
					micro.Reload();
				}
			}
		}
		this.UpdateGroupIndexText();
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000844D0 File Offset: 0x000826D0
	private void Update()
	{
		bool flag = this.selectAndDrag.Dragging || this.selectAndDrag.MultiSelecting;
		if (!flag)
		{
			bool keyUp = Input.GetKeyUp(KeyCode.Delete);
			if (keyUp)
			{
				AdventureEditorKit.BlackBoard.MakeEdit(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction(this.ClearSelectedBlockElements), EAdventureEditType.BlockVisible);
			}
			bool flag2 = !AdventureEditorKit.GetControlKey;
			if (!flag2)
			{
				bool flag3;
				if (Input.GetKeyUp(KeyCode.C))
				{
					List<AdventureEditorBlock> selectedBlocks = this._selectedBlocks;
					flag3 = (selectedBlocks != null && selectedBlocks.Count == 1);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				if (flag4)
				{
					this._clipBoardBlocks.ClearAndAddRange(from blk in this._selectedBlocks
					select blk.Index);
				}
				AdventureBlockSnapshot srcData;
				bool flag5;
				if (Input.GetKeyUp(KeyCode.V))
				{
					List<AdventureBlockIndex> clipBoardBlocks = this._clipBoardBlocks;
					if (clipBoardBlocks != null && clipBoardBlocks.Count == 1 && this._selectedBlocks.Count((AdventureEditorBlock blk) => !this._clipBoardBlocks.Contains(blk.Index)) > 0)
					{
						srcData = AdventureEditorKit.BlackBoard.CurrentGroupBlocks.FirstOrDefault((AdventureBlockSnapshot blk) => blk.Index == this._clipBoardBlocks[0]);
						flag5 = (srcData != null);
						goto IL_12F;
					}
				}
				flag5 = false;
				IL_12F:
				bool flag6 = flag5;
				if (flag6)
				{
					AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
					{
						List<AdventureBlockSnapshot> currentBlocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
						using (List<AdventureEditorBlock>.Enumerator enumerator = this._selectedBlocks.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								AdventureEditorBlock dstClipBlock = enumerator.Current;
								bool flag9 = dstClipBlock.Index == this._clipBoardBlocks[0];
								if (!flag9)
								{
									AdventureBlockSnapshot dst = currentBlocks.FirstOrDefault((AdventureBlockSnapshot blk) => blk.Index == dstClipBlock.Index);
									bool flag10 = dst == null;
									if (!flag10)
									{
										dst.ElementCoreIds.ClearAndAddRange(srcData.ElementCoreIds);
										dstClipBlock.ReloadElements();
									}
								}
							}
						}
					}, EAdventureEditType.BlockVisible);
				}
				bool keyUp2 = Input.GetKeyUp(KeyCode.R);
				if (keyUp2)
				{
					this.ResetArea();
				}
				bool keyUp3 = Input.GetKeyUp(KeyCode.E);
				if (keyUp3)
				{
					this.elementArea.gameObject.SetActive(!this.elementArea.gameObject.activeSelf);
				}
				bool keyUp4 = Input.GetKeyUp(KeyCode.Z);
				if (keyUp4)
				{
					bool flag7 = AdventureEditorKit.BlackBoard.Revert();
					if (flag7)
					{
						GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusReverted, null);
					}
				}
				bool keyUp5 = Input.GetKeyUp(KeyCode.Y);
				if (keyUp5)
				{
					bool flag8 = AdventureEditorKit.BlackBoard.Evolve();
					if (flag8)
					{
						GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusEvolved, null);
					}
				}
			}
		}
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000846CD File Offset: 0x000828CD
	private void LateUpdate()
	{
		this.LaterUpdatePropertiesPanel();
		this.LaterUpdateElementsPanel();
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x000846E0 File Offset: 0x000828E0
	public void Reload()
	{
		foreach (AdventureEditorBlock block in this._blockMappings.Values)
		{
			block.ReloadElements();
		}
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x0008473C File Offset: 0x0008293C
	private void ResetArea()
	{
		this.target.localScale = Vector3.one;
		this.target.anchoredPosition = Vector2.zero;
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00084764 File Offset: 0x00082964
	private void ResetToEmpty()
	{
		this._selectedBlocks.Clear();
		this._clipBoardBlocks.Clear();
		this.HideContextMenu();
		this.propertyPanel.gameObject.SetActive(false);
		this.blockElementsEditor.gameObject.SetActive(false);
		this.switchGridModeBtn.ClearAndAddListener(delegate
		{
			EBlockViewMode newMode = (AdventureEditorKit.BlackBoard.ViewMode == EBlockViewMode.Simulate) ? EBlockViewMode.Default : EBlockViewMode.Simulate;
			this.selectAndDrag = ((newMode == EBlockViewMode.Default) ? this.target.GetComponent<AiEditorSelectAndDrag>() : this.simulateTarget.GetComponent<AiEditorSelectAndDrag>());
			AdventureEditorKit.BlackBoard.ChangeViewMode(newMode);
			this.ResetSimulateHeightData();
			this.ReloadSize();
		});
		this.focusBtn.ClearAndAddListener(delegate
		{
			this.target.anchoredPosition = Vector2.zero;
			this.target.localScale = Vector3.one;
			bool flag = this.simulateTarget != null;
			if (flag)
			{
				this.simulateTarget.anchoredPosition = Vector2.zero;
				this.simulateTarget.localScale = Vector3.one;
			}
		});
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x000847E8 File Offset: 0x000829E8
	private void ReloadSize()
	{
		bool isSimulateMode = AdventureEditorKit.BlackBoard.ViewMode == EBlockViewMode.Simulate;
		this.target.gameObject.SetActive(!isSimulateMode);
		bool flag = isSimulateMode;
		if (flag)
		{
			this.ReloadSizeForSimulateMode();
			bool flag2 = this.simulateTarget != null;
			if (flag2)
			{
				this.simulateTarget.gameObject.SetActive(true);
			}
		}
		else
		{
			bool flag3 = this.simulateTarget != null;
			if (flag3)
			{
				this.simulateTarget.gameObject.SetActive(false);
			}
			this.ReloadSizeForDefaultMode();
		}
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x00084878 File Offset: 0x00082A78
	private void ReloadSizeForDefaultMode()
	{
		this._blockMappings.Clear();
		this._selectedBlocks.Clear();
		this._selectedMicros.Clear();
		int childIndex = 0;
		int size = AdventureEditorKit.BlackBoard.Editing.Size;
		IEnumerable<AdventureBlockIndex> blocks = AdventureBlockIndex.GetIndexes(size);
		foreach (AdventureBlockIndex index in blocks)
		{
			bool flag = childIndex == this.target.childCount;
			if (flag)
			{
				Object.Instantiate<GameObject>(this.nodeTemplate, this.target);
			}
			AdventureEditorBlock block = this.target.GetChild(childIndex++).GetComponent<AdventureEditorBlock>();
			block.Set(index);
			this._blockMappings[block.Index] = block;
		}
		for (int i = childIndex; i < this.target.childCount; i++)
		{
			Transform child = this.target.GetChild(i);
			bool activeSelf = child.gameObject.activeSelf;
			if (activeSelf)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x000849AC File Offset: 0x00082BAC
	private void ReloadSizeForSimulateMode()
	{
		bool flag = this.simulateTarget == null || this.microTemplate == null;
		if (flag)
		{
			Debug.LogWarning("Simulate mode requires simulateTarget and microTemplate to be assigned.");
		}
		else
		{
			this._selectedBlocks.Clear();
			this._selectedMicros.Clear();
			int childIndex = 0;
			int size = AdventureEditorKit.BlackBoard.Editing.Size;
			List<AdventureBlockIndex> blocks = (from idx in AdventureBlockIndex.GetIndexes(size)
			orderby idx.Y - idx.X descending, idx.X + idx.Y, AdventureEditorMainArea.GetMicroRenderOrder(idx.I)
			select idx).ToList<AdventureBlockIndex>();
			foreach (AdventureBlockIndex index in blocks)
			{
				bool flag2 = childIndex == this.simulateTarget.childCount;
				if (flag2)
				{
					Object.Instantiate<GameObject>(this.microTemplate, this.simulateTarget);
				}
				Transform microObj = this.simulateTarget.GetChild(childIndex++);
				AdventureEditorMicro micro = microObj.GetComponent<AdventureEditorMicro>();
				bool flag3 = micro == null;
				if (flag3)
				{
					Debug.LogWarning("Micro template doesn't have AdventureEditorMicro component.");
				}
				else
				{
					micro.Set(index, index.I == 2);
				}
			}
			for (int i = childIndex; i < this.simulateTarget.childCount; i++)
			{
				Transform child = this.simulateTarget.GetChild(i);
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x00084B98 File Offset: 0x00082D98
	private static int GetMicroRenderOrder(int microIndex)
	{
		bool flag = microIndex < 0 || microIndex >= AdventureEditorMicro.DataIndexToRenderIndex.Length;
		int result;
		if (flag)
		{
			result = microIndex;
		}
		else
		{
			result = AdventureEditorMicro.DataIndexToRenderIndex[microIndex];
		}
		return result;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x00084BD0 File Offset: 0x00082DD0
	private void ReloadViewMode()
	{
		this.target.eulerAngles = this.target.eulerAngles.SetZ(0f);
		this.simulateTarget.eulerAngles = this.simulateTarget.eulerAngles.SetZ(0f);
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x00084C20 File Offset: 0x00082E20
	private void UpdateGroupIndexText()
	{
		this.groupIndexText.text = string.Format("Group Index: {0}", AdventureEditorKit.BlackBoard.CurrentGroupIndex);
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x00084C48 File Offset: 0x00082E48
	private List<AdventureBlockIndex> GetSelectedBlockIndexes()
	{
		bool isSimulateMode = this.IsSimulateMode;
		List<AdventureBlockIndex> result;
		if (isSimulateMode)
		{
			result = (from x in this._selectedMicros
			select x.Index).ToList<AdventureBlockIndex>();
		}
		else
		{
			result = (from x in this._selectedBlocks
			select x.Index).ToList<AdventureBlockIndex>();
		}
		return result;
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00084CC8 File Offset: 0x00082EC8
	private IEnumerable<AdventureBlockSnapshot> GetSelectedBlocks(AdventureSnapshot snapshot)
	{
		List<AdventureBlockIndex> selectedBlockIndexes = this.GetSelectedBlockIndexes();
		List<AdventureBlockSnapshot> currentBlocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
		return from x in currentBlocks
		where selectedBlockIndexes.Contains(x.Index)
		select x;
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x00084D1C File Offset: 0x00082F1C
	private void AddElementToBlock(AdventureSnapshot snapshot, AdventureBlockIndex index, AdventureElementSnapshot data)
	{
		List<AdventureBlockSnapshot> currentBlocks = snapshot.Groups[AdventureEditorKit.BlackBoard.CurrentGroupIndex].Blocks;
		foreach (AdventureBlockSnapshot block in currentBlocks)
		{
			bool flag = block.Index == index;
			if (flag)
			{
				block.ElementCoreIds.Add(data.Id);
			}
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x00084DA4 File Offset: 0x00082FA4
	private void AddElementToSelectedBlocks(AdventureSnapshot snapshot, AdventureElementSnapshot data)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			block.ElementCoreIds.Add(data.Id);
		}
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x00084E00 File Offset: 0x00083000
	private void ClearSelectedBlockElements(AdventureSnapshot snapshot)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			block.ElementCoreIds.Clear();
		}
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x00084E58 File Offset: 0x00083058
	private void AddBlockType(AdventureSnapshot snapshot, EAdventureBlockType blockType)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			block.BlockType = block.BlockType.Add(blockType);
		}
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x00084EB4 File Offset: 0x000830B4
	private void RemoveBlockType(AdventureSnapshot snapshot, EAdventureBlockType blockType)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			block.BlockType = block.BlockType.Remove(blockType);
		}
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x00084F10 File Offset: 0x00083110
	private void AddGroupById(AdventureSnapshot snapshot, int groupId)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			bool flag = !block.GroupIds.Contains(groupId);
			if (flag)
			{
				block.GroupIds.Add(groupId);
			}
		}
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x00084F7C File Offset: 0x0008317C
	private void RemoveGroupById(AdventureSnapshot snapshot, int groupId)
	{
		foreach (AdventureBlockSnapshot block in this.GetSelectedBlocks(snapshot))
		{
			bool flag = block.GroupIds.Contains(groupId);
			if (flag)
			{
				block.GroupIds.Remove(groupId);
			}
		}
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x00084FE4 File Offset: 0x000831E4
	private void SelectBlock(AdventureEditorBlock block)
	{
		this.HideContextMenu();
		bool flag = this._selectedBlocks.Contains(block);
		if (!flag)
		{
			block.Select();
			this._selectedBlocks.Add(block);
		}
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x00085020 File Offset: 0x00083220
	private void UnselectBlock(AdventureEditorBlock block)
	{
		this.HideContextMenu();
		bool flag = this._selectedBlocks.Remove(block);
		if (flag)
		{
			block.Unselect();
		}
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x0008504C File Offset: 0x0008324C
	private void SelectMicro(AdventureEditorMicro micro)
	{
		this.HideContextMenu();
		bool flag = this._selectedMicros.Contains(micro);
		if (!flag)
		{
			micro.Select();
			this._selectedMicros.Add(micro);
		}
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x00085088 File Offset: 0x00083288
	private void UnselectMicro(AdventureEditorMicro micro)
	{
		this.HideContextMenu();
		bool flag = this._selectedMicros.Remove(micro);
		if (flag)
		{
			micro.Unselect();
		}
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x000850B4 File Offset: 0x000832B4
	private void SelectVolumeByMicro(AdventureEditorMicro micro)
	{
		int volumeX = micro.Index.X;
		int volumeY = micro.Index.Y;
		foreach (object obj in this.simulateTarget)
		{
			Transform microTransform = (Transform)obj;
			AdventureEditorMicro i = microTransform.GetComponent<AdventureEditorMicro>();
			bool flag = i == null || i.Index.X != volumeX || i.Index.Y != volumeY;
			if (!flag)
			{
				this.SelectMicro(i);
			}
		}
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x00085170 File Offset: 0x00083370
	private void UnselectVolumeByMicro(AdventureEditorMicro micro)
	{
		int volumeX = micro.Index.X;
		int volumeY = micro.Index.Y;
		foreach (object obj in this.simulateTarget)
		{
			Transform microTransform = (Transform)obj;
			AdventureEditorMicro i = microTransform.GetComponent<AdventureEditorMicro>();
			bool flag = i == null || i.Index.X != volumeX || i.Index.Y != volumeY;
			if (!flag)
			{
				this.UnselectMicro(i);
			}
		}
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x0008522C File Offset: 0x0008342C
	private void HideContextMenu()
	{
		bool activeSelf = this.adventureContextMenu.gameObject.activeSelf;
		if (activeSelf)
		{
			this.adventureContextMenu.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x00085260 File Offset: 0x00083460
	private void LaterUpdateElementsPanel()
	{
		List<AdventureBlockIndex> validSelectedIndexes = this.GetSelectedBlockIndexes().Where(new Func<AdventureBlockIndex, bool>(this.BlockValid)).ToList<AdventureBlockIndex>();
		bool flag = validSelectedIndexes != null && validSelectedIndexes.Count == 1 && !this.blockElementsEditor.gameObject.activeSelf;
		if (flag)
		{
			AdventureBlockIndex selectedIndex = validSelectedIndexes[0];
			this.blockElementsEditor.AdditionalResetCallback = delegate()
			{
				this.elementInfoEditor.Set(null);
				this.elementInfoPanel.gameObject.SetActive(false);
			};
			this.blockElementsEditor.AdditionalItemCallback = delegate(AdventureBlockElementsEditorTemplate refers, int index, IList<int> elements)
			{
				bool flag4 = !elements.CheckIndex(index);
				if (!flag4)
				{
					GameObject cursor = refers.selected;
					FileInfo editing = this.elementInfoEditor.EditingFile;
					int editingId;
					cursor.gameObject.SetActive(editing != null && AdventureElementSnapshot.LoadHeaderFromFile(editing.FullName, out editingId) && editingId == elements[index]);
					refers.GetComponent<CButton>().ClearAndAddListener(delegate
					{
						AdventureEditorKit.UpdateElementCache();
						string path = AdventureEditorKit.GetElementPathFromCache(elements[index]);
						bool flag5 = string.IsNullOrEmpty(path);
						if (!flag5)
						{
							this.elementInfoEditor.Set(new FileInfo(path));
							this.elementInfoPanel.gameObject.SetActive(true);
							RectTransform ctn = this.blockElementsEditor.container;
							for (int i = 0; i < ctn.childCount; i++)
							{
								AdventureBlockElementsEditorTemplate subRef = ctn.GetChild(i).GetComponent<AdventureBlockElementsEditorTemplate>();
								bool flag6 = subRef;
								if (flag6)
								{
									Action<AdventureBlockElementsEditorTemplate, int, IList<int>> additionalItemCallback = this.blockElementsEditor.AdditionalItemCallback;
									if (additionalItemCallback != null)
									{
										additionalItemCallback(subRef, i, elements);
									}
								}
							}
						}
					});
				}
			};
			bool isSimulateMode = this.IsSimulateMode;
			if (isSimulateMode)
			{
				AdventureEditorMicro micro = this._selectedMicros.FirstOrDefault((AdventureEditorMicro x) => x.Index == selectedIndex);
				if (micro != null)
				{
					micro.EditElements(this.blockElementsEditor);
				}
			}
			else
			{
				AdventureEditorBlock block = this._selectedBlocks.FirstOrDefault((AdventureEditorBlock x) => x.Index == selectedIndex);
				if (block != null)
				{
					block.EditElements(this.blockElementsEditor);
				}
			}
			this.HideContextMenu();
		}
		bool flag2 = validSelectedIndexes != null && validSelectedIndexes.Count > 0 && !this.blockElementsEditor.gameObject.activeSelf;
		if (flag2)
		{
			this.blockElementsEditor.gameObject.SetActive(true);
		}
		bool flag3 = validSelectedIndexes != null && validSelectedIndexes.Count > 0 && this.blockElementsEditor.gameObject.activeSelf;
		if (flag3)
		{
			this.blockElementsEditor.ShowMultiSelectedTip(validSelectedIndexes.Count > 1);
		}
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000853E3 File Offset: 0x000835E3
	private void HideElementsPanel()
	{
		this.blockElementsEditor.gameObject.SetActive(false);
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x000853F8 File Offset: 0x000835F8
	private AdventureContextMenuItemData GetBlockTypeContextMenuItemData()
	{
		List<AdventureBlockSnapshot> blockSnapshots = new List<AdventureBlockSnapshot>(this.GetSelectedBlocks(AdventureEditorKit.BlackBoard.Editing));
		AdventureContextMenuItemData blockTypeData = new AdventureContextMenuItemData
		{
			ItemName = LocalStringManager.Get(LanguageKey.LK_Adventure_CtxMenu_BlockType_Set),
			ChildItems = new List<AdventureContextMenuItemData>(),
			GroupIndex = 1,
			HideContextMenu = false
		};
		for (int i = 1; i <= 2; i++)
		{
			int index = i;
			EAdventureBlockType tValue = (EAdventureBlockType)i;
			bool full = blockSnapshots.All((AdventureBlockSnapshot b) => b.BlockType.Contains(tValue));
			bool exist = blockSnapshots.Any((AdventureBlockSnapshot b) => b.BlockType.Contains(tValue));
			AdventureContextMenuItemData secondLevelData = new AdventureContextMenuItemData
			{
				ItemName = LocalStringManager.Get((tValue == EAdventureBlockType.In) ? LanguageKey.LK_Adventure_CtxMenu_BlockType_In : LanguageKey.LK_Adventure_CtxMenu_BlockType_Out),
				ItemAction = delegate()
				{
					AdventureEditorKit.BlackBoard.MakeEdit<EAdventureBlockType>(full ? new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<EAdventureBlockType>(this.RemoveBlockType) : new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<EAdventureBlockType>(this.AddBlockType), EAdventureEditType.BlockVisible, tValue);
					full = !full;
					string itemName = (full ? "√ " : "  ") + LocalStringManager.Get((tValue == EAdventureBlockType.In) ? LanguageKey.LK_Adventure_CtxMenu_BlockType_In : LanguageKey.LK_Adventure_CtxMenu_BlockType_Out);
					this.adventureContextMenu.RefreshSecondLevelItemName(index - 1, itemName);
				},
				HideContextMenu = false
			};
			secondLevelData.ItemName = this.FormatName(secondLevelData.ItemName, full, exist);
			blockTypeData.ChildItems.Add(secondLevelData);
		}
		return blockTypeData;
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x00085520 File Offset: 0x00083720
	private AdventureContextMenuItemData GetGroupContextMenuItemData()
	{
		List<AdventureBlockSnapshot> blockSnapshots = new List<AdventureBlockSnapshot>(this.GetSelectedBlocks(AdventureEditorKit.BlackBoard.Editing));
		AdventureContextMenuItemData groupSetData = new AdventureContextMenuItemData
		{
			ItemName = LocalStringManager.Get(LanguageKey.LK_Adventure_CtxMenu_GroupIds_Set),
			ChildItems = new List<AdventureContextMenuItemData>(),
			GroupIndex = 1,
			HideContextMenu = false
		};
		for (int i = 0; i < this.groupPalette.GetCurrent().Length - 1; i++)
		{
			int groupId = i + 1;
			bool full = blockSnapshots.All((AdventureBlockSnapshot b) => b.GroupIds.Contains(groupId));
			bool exist = blockSnapshots.Any((AdventureBlockSnapshot b) => b.GroupIds.Contains(groupId));
			string itemText = LocalStringManager.GetFormat(LanguageKey.LK_Adventure_CtxMenu_GroupIds_SetId, groupId) + " ■".SetColor(this.groupPalette[groupId]);
			AdventureContextMenuItemData secondData = new AdventureContextMenuItemData
			{
				ItemName = itemText,
				ItemAction = delegate()
				{
					AdventureEditorKit.BlackBoard.MakeEdit<int>(full ? new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<int>(this.RemoveGroupById) : new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<int>(this.AddGroupById), EAdventureEditType.BlockVisible, groupId);
					full = !full;
					this.adventureContextMenu.RefreshSecondLevelItemName(groupId - 1, (full ? "√ " : "  ") + itemText);
				},
				HideContextMenu = false
			};
			secondData.ItemName = this.FormatName(secondData.ItemName, full, exist);
			groupSetData.ChildItems.Add(secondData);
		}
		return groupSetData;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x00085678 File Offset: 0x00083878
	private string FormatName(string nameStr, bool full, bool exist)
	{
		if (full)
		{
			nameStr = "√ " + nameStr;
		}
		else if (exist)
		{
			nameStr = "○ " + nameStr;
		}
		else
		{
			nameStr = "  " + nameStr;
		}
		return nameStr;
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x000856C0 File Offset: 0x000838C0
	private void LaterUpdatePropertiesPanel()
	{
		List<AdventureBlockIndex> validSelectedIndexes = this.GetSelectedBlockIndexes().Where(new Func<AdventureBlockIndex, bool>(this.BlockValid)).ToList<AdventureBlockIndex>();
		bool flag = validSelectedIndexes != null && validSelectedIndexes.Count > 0 && !this.propertyPanel.gameObject.activeSelf;
		if (flag)
		{
			this.propertyPanel.gameObject.SetActive(true);
			this.propertyPanel.Refresh(validSelectedIndexes);
			this.HideContextMenu();
		}
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x00085739 File Offset: 0x00083939
	private void RefreshPropertyPanel(ArgumentBox box)
	{
		this.RefreshPropertyPanel();
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x00085744 File Offset: 0x00083944
	private void RefreshPropertyPanel()
	{
		List<AdventureBlockIndex> validSelectedIndexes = this.GetSelectedBlockIndexes().Where(new Func<AdventureBlockIndex, bool>(this.BlockValid)).ToList<AdventureBlockIndex>();
		bool flag = validSelectedIndexes != null && validSelectedIndexes.Count > 0 && this.propertyPanel.gameObject.activeSelf;
		if (flag)
		{
			this.propertyPanel.Refresh(validSelectedIndexes);
			this.HideContextMenu();
		}
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x000857A8 File Offset: 0x000839A8
	private bool BlockValid(AdventureBlockIndex index)
	{
		return Math.Abs(index.X) + Math.Abs(index.Y) <= AdventureEditorKit.BlackBoard.Editing.Size;
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x000857E8 File Offset: 0x000839E8
	private void HidePropertyPanel()
	{
		bool activeSelf = this.propertyPanel.gameObject.activeSelf;
		if (activeSelf)
		{
			this.propertyPanel.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x0008581C File Offset: 0x00083A1C
	private void ToggleGroupManagePanel(ArgumentBox _)
	{
		bool isActive = this.groupManagePanel.gameObject.activeSelf;
		this.groupManagePanel.gameObject.SetActive(!isActive);
		bool flag = !isActive;
		if (flag)
		{
			this.groupManagePanel.Refresh();
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x0600158F RID: 5519 RVA: 0x00085864 File Offset: 0x00083A64
	private AdventureRemakeMapBlockItem BlockConfig
	{
		get
		{
			return AdventureRemakeMapBlock.Instance[0];
		}
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001590 RID: 5520 RVA: 0x00085871 File Offset: 0x00083A71
	private byte CenterCoord
	{
		get
		{
			return (byte)(AdventureEditorKit.BlackBoard.Editing.Size + this.BlockConfig.CircleCount);
		}
	}

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001591 RID: 5521 RVA: 0x0008588F File Offset: 0x00083A8F
	private int MapSize
	{
		get
		{
			return (int)(this.CenterCoord * 2 + 1);
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x0008589B File Offset: 0x00083A9B
	private void ResetSimulateHeightData()
	{
		AdventureEditorMainArea.SimulateHeightDict.Clear();
		this.InitUnitFlatData();
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x000858B0 File Offset: 0x00083AB0
	private void InitUnitFlatData()
	{
		bool flag = this.BlockConfig.FlatPercentage > 0;
		if (flag)
		{
			this.ClearUnitFlatData();
			this._outerUnitCount = this.GetOuterUnitCount();
			this._flatUnitCount = this.GetFlatUnitCount(this._outerUnitCount);
			this.GetAllFlatStartPoint();
			this.GetSelectedFlatStartPoint();
			this.InitFlatDict();
		}
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0008590C File Offset: 0x00083B0C
	private void InitFlatDict()
	{
		int growCount = 0;
		while (AdventureEditorMainArea.FlatDict.Count < this._flatUnitCount && growCount < 20)
		{
			growCount++;
			for (int groupIndex = 0; groupIndex < this._selectedFlatStartPoint.Count; groupIndex++)
			{
				ByteCoordinate startUnit = this._selectedFlatStartPoint[groupIndex];
				bool flag = !AdventureEditorMainArea.FlatDict.Keys.Contains(startUnit);
				if (flag)
				{
					AdventureEditorMainArea.FlatDict.Add(startUnit, this._flatHeights[groupIndex]);
				}
				ByteCoordinate nextUnit = this.GetNextFlatUnit(startUnit);
				bool flag2 = !AdventureEditorMainArea.FlatDict.Keys.Contains(nextUnit);
				if (flag2)
				{
					AdventureEditorMainArea.FlatDict.Add(nextUnit, this._flatHeights[groupIndex]);
				}
				bool flag3 = AdventureEditorMainArea.FlatDict.Count >= this._flatUnitCount;
				if (flag3)
				{
					break;
				}
			}
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x00085A04 File Offset: 0x00083C04
	private ByteCoordinate GetNextFlatUnit(ByteCoordinate startUnit)
	{
		int loopCount = 0;
		ByteCoordinate nextUnit;
		do
		{
			EAdventureDirection direction = ViewAdventureRemake.GetRandomDirection();
			nextUnit = ViewAdventureRemake.GetNextFlatUnitByDirection(startUnit, direction);
			startUnit = nextUnit;
			loopCount++;
		}
		while (!this.CheckNextFlatUnitValid(nextUnit) && loopCount < 21);
		return nextUnit;
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x00085A48 File Offset: 0x00083C48
	private bool CheckNextFlatUnitValid(ByteCoordinate coordinate)
	{
		return !AdventureEditorMainArea.FlatDict.Keys.Contains(coordinate) && (int)coordinate.X < this.MapSize && (int)coordinate.Y < this.MapSize && !this.CheckRenderIndexValid(coordinate);
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x00085A98 File Offset: 0x00083C98
	private bool CheckRenderIndexValid(ByteCoordinate coordinate)
	{
		int distance = Math.Abs((int)(coordinate.X - this.CenterCoord)) + Math.Abs((int)(coordinate.Y - this.CenterCoord));
		return distance <= AdventureEditorKit.BlackBoard.Editing.Size;
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x00085AE8 File Offset: 0x00083CE8
	private void ClearUnitFlatData()
	{
		if (AdventureEditorMainArea.FlatDict == null)
		{
			AdventureEditorMainArea.FlatDict = new Dictionary<ByteCoordinate, float>();
		}
		AdventureEditorMainArea.FlatDict.Clear();
		if (this._allFlatStartPoint == null)
		{
			this._allFlatStartPoint = new List<ByteCoordinate>();
		}
		this._allFlatStartPoint.Clear();
		if (this._selectedFlatStartPoint == null)
		{
			this._selectedFlatStartPoint = new List<ByteCoordinate>();
		}
		this._selectedFlatStartPoint.Clear();
		if (this._flatHeights == null)
		{
			this._flatHeights = new List<float>();
		}
		this._flatHeights.Clear();
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x00085B70 File Offset: 0x00083D70
	private int GetOuterUnitCount()
	{
		int validSize = AdventureEditorKit.BlackBoard.Editing.Size;
		int validCount = 2 * validSize * validSize + 2 * validSize + 1;
		return this.MapSize * this.MapSize - validCount;
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x00085BB0 File Offset: 0x00083DB0
	private int GetFlatUnitCount(int outerCount)
	{
		return (int)((float)(outerCount * this.BlockConfig.FlatPercentage) / 100f);
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x00085BD8 File Offset: 0x00083DD8
	private void GetAllFlatStartPoint()
	{
		this._allFlatStartPoint.Add(new ByteCoordinate(0, 0));
		this._allFlatStartPoint.Add(new ByteCoordinate(0, this.CenterCoord));
		this._allFlatStartPoint.Add(new ByteCoordinate(0, this.CenterCoord * 2));
		this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord, 0));
		this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord * 2, 0));
		this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord, this.CenterCoord * 2));
		this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord * 2, this.CenterCoord));
		this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord * 2, this.CenterCoord * 2));
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x00085CC4 File Offset: 0x00083EC4
	private void GetSelectedFlatStartPoint()
	{
		int sumCount = (int)Random.Range(2f, MathF.Min(5f, (float)this._allFlatStartPoint.Count));
		for (int i = 0; i < sumCount; i++)
		{
			ByteCoordinate startPoint = this._allFlatStartPoint.GetRandom<ByteCoordinate>();
			this._selectedFlatStartPoint.Add(startPoint);
			this._allFlatStartPoint.Remove(startPoint);
			this._flatHeights.Add(this.BlockConfig.FlatHeight.GetRandom<float>());
		}
	}

	// Token: 0x040011AD RID: 4525
	[SerializeField]
	private RectTransform target;

	// Token: 0x040011AE RID: 4526
	[SerializeField]
	private AiEditorSelectAndDrag selectAndDrag;

	// Token: 0x040011AF RID: 4527
	[SerializeField]
	private AdventureContextMenu adventureContextMenu;

	// Token: 0x040011B0 RID: 4528
	[SerializeField]
	private MultiSelectArea multiSelectRect;

	// Token: 0x040011B1 RID: 4529
	[SerializeField]
	private AdventureEditorElementContainer container;

	// Token: 0x040011B2 RID: 4530
	[SerializeField]
	private AdventureEditorBlockContainer blockContainer;

	// Token: 0x040011B3 RID: 4531
	[SerializeField]
	private AdventureEditorPropertiesPanel propertyPanel;

	// Token: 0x040011B4 RID: 4532
	[SerializeField]
	private AdventureGroupManagerPanel groupManagePanel;

	// Token: 0x040011B5 RID: 4533
	[SerializeField]
	private AdventureBlockElementsEditor blockElementsEditor;

	// Token: 0x040011B6 RID: 4534
	[SerializeField]
	private AdventureEditorElementArea elementArea;

	// Token: 0x040011B7 RID: 4535
	[SerializeField]
	private GameObject nodeTemplate;

	// Token: 0x040011B8 RID: 4536
	[SerializeField]
	private CButton switchGridModeBtn;

	// Token: 0x040011B9 RID: 4537
	[SerializeField]
	private CButton focusBtn;

	// Token: 0x040011BA RID: 4538
	[SerializeField]
	private AdventureEditorGroupPalette groupPalette;

	// Token: 0x040011BB RID: 4539
	[SerializeField]
	private AdventureEditorElementInfo elementInfoEditor;

	// Token: 0x040011BC RID: 4540
	[SerializeField]
	private GameObject elementInfoPanel;

	// Token: 0x040011BD RID: 4541
	[SerializeField]
	private CButton switchToElementContainerButton;

	// Token: 0x040011BE RID: 4542
	[SerializeField]
	private CButton switchToBlockContainerButton;

	// Token: 0x040011BF RID: 4543
	[SerializeField]
	private RectTransform simulateTarget;

	// Token: 0x040011C0 RID: 4544
	[SerializeField]
	private GameObject microTemplate;

	// Token: 0x040011C1 RID: 4545
	[SerializeField]
	private TextMeshProUGUI groupIndexText;

	// Token: 0x040011C2 RID: 4546
	[SerializeField]
	private CButton pure;

	// Token: 0x040011C3 RID: 4547
	private readonly Dictionary<AdventureBlockIndex, AdventureEditorBlock> _blockMappings = new Dictionary<AdventureBlockIndex, AdventureEditorBlock>();

	// Token: 0x040011C4 RID: 4548
	private readonly List<AdventureEditorBlock> _selectedBlocks = new List<AdventureEditorBlock>();

	// Token: 0x040011C5 RID: 4549
	private readonly List<AdventureEditorMicro> _selectedMicros = new List<AdventureEditorMicro>();

	// Token: 0x040011C6 RID: 4550
	private readonly List<AdventureBlockIndex> _clipBoardBlocks = new List<AdventureBlockIndex>();

	// Token: 0x040011C7 RID: 4551
	private bool _adventureEditorPureMode;

	// Token: 0x040011C8 RID: 4552
	[TupleElementNames(new string[]
	{
		"x",
		"y"
	})]
	public static Dictionary<ValueTuple<int, int>, float> SimulateHeightDict = new Dictionary<ValueTuple<int, int>, float>();

	// Token: 0x040011C9 RID: 4553
	public static Dictionary<ByteCoordinate, float> FlatDict;

	// Token: 0x040011CA RID: 4554
	private List<ByteCoordinate> _allFlatStartPoint;

	// Token: 0x040011CB RID: 4555
	private List<ByteCoordinate> _selectedFlatStartPoint;

	// Token: 0x040011CC RID: 4556
	private List<float> _flatHeights;

	// Token: 0x040011CD RID: 4557
	private int _outerUnitCount;

	// Token: 0x040011CE RID: 4558
	private int _flatUnitCount;
}
