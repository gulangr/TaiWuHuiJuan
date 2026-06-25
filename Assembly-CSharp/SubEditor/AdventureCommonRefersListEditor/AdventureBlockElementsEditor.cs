using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

namespace SubEditor.AdventureCommonRefersListEditor
{
	// Token: 0x0200069C RID: 1692
	public class AdventureBlockElementsEditor : AdventureCommonRefersListEditor<AdventureElementSnapshot>, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
	{
		// Token: 0x06004F55 RID: 20309 RVA: 0x00251E1C File Offset: 0x0025001C
		private static AdventureBlockSnapshot ExtractIndexedBlock(IList<AdventureBlockSnapshot> list, AdventureBlockIndex blockIndex)
		{
			return list.First((AdventureBlockSnapshot block) => block.Index == blockIndex);
		}

		// Token: 0x06004F56 RID: 20310 RVA: 0x00251E48 File Offset: 0x00250048
		private AdventureBlockSnapshot ExtractIndexedBlock(IList<AdventureBlockSnapshot> list)
		{
			return AdventureBlockElementsEditor.ExtractIndexedBlock(list, this.TargetIndex);
		}

		// Token: 0x06004F57 RID: 20311 RVA: 0x00251E56 File Offset: 0x00250056
		private List<AdventureElementSnapshot> MakeLookingList(IList<int> elementIds)
		{
			return elementIds.Select(new Func<int, AdventureElementSnapshot>(AdventureEditorKit.GetElementFromCache)).ToList<AdventureElementSnapshot>();
		}

		// Token: 0x06004F58 RID: 20312 RVA: 0x00251E70 File Offset: 0x00250070
		public AdventureBlockElementsEditor()
		{
			this.Creator = ((IList<AdventureElementSnapshot> _) => new AdventureElementSnapshot());
			this.RefreshAction = delegate(IList<AdventureElementSnapshot> _, MonoBehaviour elementRefer, int elementIndex, Action _)
			{
				AdventureBlockElementsEditorTemplate editorTemplate = elementRefer.GetComponent<AdventureBlockElementsEditorTemplate>();
				AdventureBlockSnapshot blockSnapshot = this.ExtractIndexedBlock(AdventureEditorKit.BlackBoard.Editing.Blocks);
				int elementId = this._elementCoreIdList[elementIndex];
				AdventureElementSnapshot element = AdventureEditorKit.GetElementFromCache(elementId);
				bool invalidElement = element == null;
				bool flag = invalidElement;
				if (flag)
				{
					element = new AdventureElementSnapshot
					{
						Name = LocalStringManager.Get(LanguageKey.LK_Unknow)
					};
				}
				TextMeshProUGUI nameText = editorTemplate.nameText;
				nameText.text = element.Name.Value.SetColor(invalidElement ? "red" : "pinkyellow").ColorReplace();
				nameText.fontSize = (float)(invalidElement ? 60 : 25);
				editorTemplate.icon.SetSprite(element.Icon, false, null);
				bool isFakeElement = elementIndex >= blockSnapshot.ElementCoreIds.Count;
				AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction <>9__5;
				AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction <>9__6;
				AdventureAbstractListEditor<AdventureElementSnapshot, MonoBehaviour>.ItemAddRemove(delegate(int _)
				{
					bool flag2 = !isFakeElement;
					if (flag2)
					{
						AdventureBlackBoard<AdventureSnapshot, EAdventureEditType> blackBoard = AdventureEditorKit.BlackBoard;
						AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction editAction;
						if ((editAction = <>9__5) == null)
						{
							editAction = (<>9__5 = delegate(AdventureSnapshot snapshot)
							{
								AdventureBlockSnapshot blk = this.ExtractIndexedBlock(snapshot.Blocks);
								blk.ElementCoreIds.Remove(elementId);
								this.RefreshByData(blk.ElementCoreIds, blk.FakeElementCoreIds);
							});
						}
						blackBoard.MakeEdit(editAction, EAdventureEditType.BlockVisible);
					}
					else
					{
						AdventureBlackBoard<AdventureSnapshot, EAdventureEditType> blackBoard2 = AdventureEditorKit.BlackBoard;
						AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction editAction2;
						if ((editAction2 = <>9__6) == null)
						{
							editAction2 = (<>9__6 = delegate(AdventureSnapshot snapshot)
							{
								AdventureBlockSnapshot blk = this.ExtractIndexedBlock(snapshot.Blocks);
								blk.FakeElementCoreIds.Remove(elementId);
								this.RefreshByData(blk.ElementCoreIds, blk.FakeElementCoreIds);
							});
						}
						blackBoard2.MakeEdit(editAction2, EAdventureEditType.BlockVisible);
					}
				}, editorTemplate, elementIndex, delegate()
				{
				}, null);
				Action<AdventureBlockElementsEditorTemplate, int, IList<int>> additionalItemCallback = this.AdditionalItemCallback;
				if (additionalItemCallback != null)
				{
					additionalItemCallback(editorTemplate, elementIndex, this._elementCoreIdList);
				}
				CToggle fakeToggle = editorTemplate.fakeToggle;
				fakeToggle.onValueChanged.RemoveAllListeners();
				fakeToggle.interactable = !invalidElement;
				fakeToggle.isOn = isFakeElement;
				fakeToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
					{
						AdventureBlockSnapshot blk = this.ExtractIndexedBlock(snapshot.Blocks);
						bool isOn = isOn;
						if (isOn)
						{
							blk.ElementCoreIds.Remove(elementId);
							blk.FakeElementCoreIds.Add(elementId);
						}
						else
						{
							blk.ElementCoreIds.Add(elementId);
							blk.FakeElementCoreIds.Remove(elementId);
						}
						this.RefreshByData(blk.ElementCoreIds, blk.FakeElementCoreIds);
					}, EAdventureEditType.BlockVisible);
				});
			};
		}

		// Token: 0x06004F59 RID: 20313 RVA: 0x00251EC8 File Offset: 0x002500C8
		protected override void OnEnable()
		{
			AdventureBlockSnapshot blockSnapshot = this.ExtractIndexedBlock(AdventureEditorKit.BlackBoard.Editing.Blocks);
			this.RefreshByData(blockSnapshot.ElementCoreIds, blockSnapshot.FakeElementCoreIds);
			this.title.text = string.Format("({0},{1},{2})", this.TargetIndex.X, this.TargetIndex.Y, this.TargetIndex.I);
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x00251F45 File Offset: 0x00250145
		private void OnDisable()
		{
			Action additionalResetCallback = this.AdditionalResetCallback;
			if (additionalResetCallback != null)
			{
				additionalResetCallback();
			}
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x00251F5C File Offset: 0x0025015C
		private void RefreshByData(List<int> elementCoreIds, List<int> fakeElementCoreIds)
		{
			this._elementCoreIdList.Clear();
			this._elementCoreIdList.AddRange(elementCoreIds);
			this._elementCoreIdList.AddRange(fakeElementCoreIds);
			Action additionalResetCallback = this.AdditionalResetCallback;
			if (additionalResetCallback != null)
			{
				additionalResetCallback();
			}
			this.Refresh(this.MakeLookingList(this._elementCoreIdList));
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x00251FB8 File Offset: 0x002501B8
		protected override void Refresh(IList<AdventureElementSnapshot> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				this.buttonAdd.onClick.ResetListener(delegate()
				{
					this.AddItem(list);
				});
				base.Rebuild<AdventureBlockElementsEditorTemplate>(list.Count, delegate(AdventureBlockElementsEditorTemplate unit, int index)
				{
					this.RefreshItem(list, unit, index, false);
				});
			}
		}

		// Token: 0x06004F5D RID: 20317 RVA: 0x00252024 File Offset: 0x00250224
		protected override bool CheckEmpty()
		{
			return false;
		}

		// Token: 0x06004F5E RID: 20318 RVA: 0x00252028 File Offset: 0x00250228
		void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
		{
			bool flag = editType.Contains(EAdventureEditType.BlockVisible);
			if (flag)
			{
				AdventureBlockSnapshot blockSnapshot = this.ExtractIndexedBlock(AdventureEditorKit.BlackBoard.Editing.Blocks);
				this.RefreshByData(blockSnapshot.ElementCoreIds, blockSnapshot.FakeElementCoreIds);
			}
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x0025206C File Offset: 0x0025026C
		public void ShowMultiSelectedTip(bool multiSelect)
		{
			this.multiSelectedTip.gameObject.SetActive(multiSelect);
			this.contents.gameObject.SetActive(!multiSelect);
		}

		// Token: 0x04003692 RID: 13970
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04003693 RID: 13971
		[SerializeField]
		private GameObject contents;

		// Token: 0x04003694 RID: 13972
		[SerializeField]
		private GameObject multiSelectedTip;

		// Token: 0x04003695 RID: 13973
		internal Action<AdventureBlockElementsEditorTemplate, int, IList<int>> AdditionalItemCallback;

		// Token: 0x04003696 RID: 13974
		internal Action AdditionalResetCallback;

		// Token: 0x04003697 RID: 13975
		internal AdventureBlockIndex TargetIndex;

		// Token: 0x04003698 RID: 13976
		private readonly List<int> _elementCoreIdList = new List<int>();
	}
}
