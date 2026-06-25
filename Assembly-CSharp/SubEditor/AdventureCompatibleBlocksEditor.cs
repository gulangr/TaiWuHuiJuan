using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure.Editor;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SubEditor
{
	// Token: 0x02000697 RID: 1687
	public class AdventureCompatibleBlocksEditor : MonoBehaviour, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
	{
		// Token: 0x06004F33 RID: 20275 RVA: 0x00251004 File Offset: 0x0024F204
		private void Awake()
		{
			this.compatibleVerticalScrollView.OnItemRender += this.CompatibleItemRender;
			this.incompatibleVerticalScrollView.OnItemRender += this.IncompatibleItemRender;
			this.addBtn.ClearAndAddListener(new Action(this.OnClickAdd));
			this.removeBtn.ClearAndAddListener(new Action(this.OnClickRemove));
			this.searchByName.onValueChanged.RemoveAllListeners();
			this.searchByName.onEndEdit.RemoveAllListeners();
			this.searchByName.text = string.Empty;
			this.searchByName.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchByNameChanged));
			this.searchByName.onEndEdit.AddListener(new UnityAction<string>(this.OnSearchByNameChanged));
		}

		// Token: 0x06004F34 RID: 20276 RVA: 0x002510DF File Offset: 0x0024F2DF
		private void OnEnable()
		{
			this.UpdateCache();
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x002510E9 File Offset: 0x0024F2E9
		private void OnClickAdd()
		{
			AdventureEditorKit.BlackBoard.MakeEdit(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction(this.ApplyAdd), EAdventureEditType.CompatibleBlocks);
		}

		// Token: 0x06004F36 RID: 20278 RVA: 0x00251104 File Offset: 0x0024F304
		private void OnClickRemove()
		{
			AdventureEditorKit.BlackBoard.MakeEdit(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction(this.ApplyRemove), EAdventureEditType.CompatibleBlocks);
		}

		// Token: 0x06004F37 RID: 20279 RVA: 0x00251120 File Offset: 0x0024F320
		private void OnSearchByNameChanged(string inputValue)
		{
			CommonUtils.FixToShowAbleString(ref inputValue, this.searchByName.textComponent.font);
			inputValue = inputValue.Replace(" ", string.Empty);
			this.searchByName.SetTextWithoutNotify(inputValue);
			this.UpdateCache();
		}

		// Token: 0x06004F38 RID: 20280 RVA: 0x0025116C File Offset: 0x0024F36C
		public void Load(EAdventureEditType editType)
		{
			bool flag = !editType.Contains(EAdventureEditType.CompatibleBlocks);
			if (!flag)
			{
				this.UpdateCache();
			}
		}

		// Token: 0x06004F39 RID: 20281 RVA: 0x00251194 File Offset: 0x0024F394
		private void UpdateCache()
		{
			List<short> compatibleBlocks = AdventureEditorKit.BlackBoard.Editing.CompatibleBlocks;
			this._incompatibleBlocks.ClearAndAddRange(from x in MapBlock.Instance
			select x.TemplateId into x
			where !compatibleBlocks.Contains(x)
			select x);
			this._selectedCompatibleBlocks.RemoveAll(new Predicate<short>(this._incompatibleBlocks.Contains));
			this._selectedIncompatibleBlocks.RemoveAll(new Predicate<short>(compatibleBlocks.Contains));
			this.UpdateBtnInteractable();
			this.UpdateCacheBySearching();
			this.compatibleVerticalScrollView.UpdateData(this._searchCompatibleBlocks.Count);
			this.incompatibleVerticalScrollView.UpdateData(this._searchIncompatibleBlocks.Count);
		}

		// Token: 0x06004F3A RID: 20282 RVA: 0x0025127C File Offset: 0x0024F47C
		private void UpdateCacheBySearching()
		{
			List<short> compatibleBlocks = AdventureEditorKit.BlackBoard.Editing.CompatibleBlocks;
			string searchingText = this.searchByName.text;
			bool flag = string.IsNullOrEmpty(searchingText);
			if (flag)
			{
				this._searchCompatibleBlocks.ClearAndAddRange(compatibleBlocks);
				this._searchIncompatibleBlocks.ClearAndAddRange(this._incompatibleBlocks);
			}
			else
			{
				this._searchCompatibleBlocks.ClearAndAddRange(from x in compatibleBlocks
				where MapBlock.Instance[x].AdventureEditorName.Contains(searchingText)
				select x);
				this._searchIncompatibleBlocks.ClearAndAddRange(from x in this._incompatibleBlocks
				where MapBlock.Instance[x].AdventureEditorName.Contains(searchingText)
				select x);
			}
		}

		// Token: 0x06004F3B RID: 20283 RVA: 0x00251326 File Offset: 0x0024F526
		private void UpdateBtnInteractable()
		{
			this.addBtn.interactable = (this._selectedIncompatibleBlocks.Count > 0);
			this.removeBtn.interactable = (this._selectedCompatibleBlocks.Count > 0);
		}

		// Token: 0x06004F3C RID: 20284 RVA: 0x00251360 File Offset: 0x0024F560
		private void CompatibleItemRender(int index, GameObject refers)
		{
			short templateId = this._searchCompatibleBlocks[index];
			AdventureCompatibleBlocksEditorTemplate template = refers.GetComponent<AdventureCompatibleBlocksEditorTemplate>();
			MapBlockItem config = MapBlock.Instance[templateId];
			template.nameText.text = config.AdventureEditorName;
			CToggle toggle = template.toggle;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.isOn = this._selectedCompatibleBlocks.Contains(templateId);
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				bool flag = isOn && !this._selectedCompatibleBlocks.Contains(templateId);
				if (flag)
				{
					this._selectedCompatibleBlocks.Add(templateId);
				}
				bool flag2 = !isOn && this._selectedCompatibleBlocks.Contains(templateId);
				if (flag2)
				{
					this._selectedCompatibleBlocks.Remove(templateId);
				}
				this.UpdateBtnInteractable();
			});
		}

		// Token: 0x06004F3D RID: 20285 RVA: 0x002513FC File Offset: 0x0024F5FC
		private void IncompatibleItemRender(int index, GameObject refers)
		{
			short templateId = this._searchIncompatibleBlocks[index];
			AdventureCompatibleBlocksEditorTemplate template = refers.GetComponent<AdventureCompatibleBlocksEditorTemplate>();
			MapBlockItem config = MapBlock.Instance[templateId];
			template.nameText.text = config.AdventureEditorName;
			CToggle toggle = template.toggle;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.isOn = this._selectedIncompatibleBlocks.Contains(templateId);
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				bool flag = isOn && !this._selectedIncompatibleBlocks.Contains(templateId);
				if (flag)
				{
					this._selectedIncompatibleBlocks.Add(templateId);
				}
				bool flag2 = !isOn && this._selectedIncompatibleBlocks.Contains(templateId);
				if (flag2)
				{
					this._selectedIncompatibleBlocks.Remove(templateId);
				}
				this.UpdateBtnInteractable();
			});
		}

		// Token: 0x06004F3E RID: 20286 RVA: 0x00251498 File Offset: 0x0024F698
		private void ApplyAdd(AdventureSnapshot snapshot)
		{
			foreach (short templateId in this._selectedIncompatibleBlocks)
			{
				bool flag = !snapshot.CompatibleBlocks.Contains(templateId);
				if (flag)
				{
					snapshot.CompatibleBlocks.Add(templateId);
				}
			}
		}

		// Token: 0x06004F3F RID: 20287 RVA: 0x00251508 File Offset: 0x0024F708
		private void ApplyRemove(AdventureSnapshot snapshot)
		{
			foreach (short templateId in this._selectedCompatibleBlocks)
			{
				bool flag = snapshot.CompatibleBlocks.Contains(templateId);
				if (flag)
				{
					snapshot.CompatibleBlocks.Remove(templateId);
				}
			}
		}

		// Token: 0x04003674 RID: 13940
		[SerializeField]
		private InfinityScroll compatibleVerticalScrollView;

		// Token: 0x04003675 RID: 13941
		[SerializeField]
		private InfinityScroll incompatibleVerticalScrollView;

		// Token: 0x04003676 RID: 13942
		[SerializeField]
		private CButton addBtn;

		// Token: 0x04003677 RID: 13943
		[SerializeField]
		private CButton removeBtn;

		// Token: 0x04003678 RID: 13944
		[SerializeField]
		private TMP_InputField searchByName;

		// Token: 0x04003679 RID: 13945
		private readonly List<short> _incompatibleBlocks = new List<short>();

		// Token: 0x0400367A RID: 13946
		private readonly List<short> _selectedCompatibleBlocks = new List<short>();

		// Token: 0x0400367B RID: 13947
		private readonly List<short> _selectedIncompatibleBlocks = new List<short>();

		// Token: 0x0400367C RID: 13948
		private readonly List<short> _searchCompatibleBlocks = new List<short>();

		// Token: 0x0400367D RID: 13949
		private readonly List<short> _searchIncompatibleBlocks = new List<short>();
	}
}
