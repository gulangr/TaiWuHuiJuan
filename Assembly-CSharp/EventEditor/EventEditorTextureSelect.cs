using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x02000651 RID: 1617
	public class EventEditorTextureSelect : EventEditorSubPageBase
	{
		// Token: 0x06004CF9 RID: 19705 RVA: 0x0024556C File Offset: 0x0024376C
		public static void Init(EventEditorTextureSelect obj)
		{
			EventEditorTextureSelect.Instance = obj;
			EventEditorTextureSelect.Instance.InternalInit();
		}

		// Token: 0x06004CFA RID: 19706 RVA: 0x00245580 File Offset: 0x00243780
		protected override void InternalInit()
		{
			base.gameObject.SetActive(false);
			this.mainWindow.anchoredPosition = Vector2.right * 1000f;
			this.searchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchInputValueChanged));
			this.textureScroll.OnItemRender += this.OnCellRender;
			this.tabButtons.OnActiveIndexChange += this.OnToggleGroupValueChanged;
			this.tabButtons.Init(-1);
		}

		// Token: 0x06004CFB RID: 19707 RVA: 0x00245610 File Offset: 0x00243810
		public override void Show()
		{
			this.InitData();
			base.gameObject.SetActive(true);
			this.textureScroll.initSuccess = false;
			this.mainWindow.DOAnchorPosX(0f, 0.25f, false).SetEase(Ease.OutBack).OnComplete(new TweenCallback(this.Refresh));
		}

		// Token: 0x06004CFC RID: 19708 RVA: 0x00245670 File Offset: 0x00243870
		public override void Hide()
		{
			Action<byte> onSelectComplete = this.OnSelectComplete;
			if (onSelectComplete != null)
			{
				onSelectComplete(this._assetCode);
			}
			this.mainWindow.DOAnchorPosX(1000f, 0.25f, false).SetEase(Ease.InBack).OnComplete(delegate
			{
				base.gameObject.SetActive(false);
			});
		}

		// Token: 0x06004CFD RID: 19709 RVA: 0x002456C5 File Offset: 0x002438C5
		private void InitData()
		{
		}

		// Token: 0x06004CFE RID: 19710 RVA: 0x002456C8 File Offset: 0x002438C8
		private void Refresh()
		{
			this.InitData();
			string searchKey = this.searchInputField.text;
			bool flag = !string.IsNullOrEmpty(searchKey);
			if (flag)
			{
				bool flag2 = this._assetCode == 0;
				if (flag2)
				{
					List<Texture2D> list = new List<Texture2D>();
					for (int i = 0; i < this._internalTextures.Count; i++)
					{
						Texture2D texture = this._internalTextures[i];
						bool flag3 = texture.name.Contains(searchKey);
						if (flag3)
						{
							list.Add(texture);
						}
					}
					this._internalTextures = list;
				}
				else
				{
					bool flag4 = this._assetCode == 1;
					if (flag4)
					{
						List<ExternalTextureInfo> list2 = new List<ExternalTextureInfo>();
						for (int j = 0; j < this._externalTextureInfos.Count; j++)
						{
							ExternalTextureInfo info = this._externalTextureInfos[j];
							bool flag5 = info.TextureName.Contains(searchKey);
							if (flag5)
							{
								list2.Add(info);
							}
						}
						this._externalTextureInfos = list2;
					}
				}
			}
			int dataCount = 0;
			bool flag6 = this._assetCode == 0;
			if (flag6)
			{
				dataCount = this._internalTextures.Count;
			}
			else
			{
				bool flag7 = this._assetCode == 1;
				if (flag7)
				{
					dataCount = this._externalTextureInfos.Count;
				}
			}
			this.textureScroll.UpdateData(dataCount);
		}

		// Token: 0x06004CFF RID: 19711 RVA: 0x00245820 File Offset: 0x00243A20
		private void OnCellRender(int index, GameObject goCell)
		{
			EventEditorTextureSelectCellPrefabInfo cellInfo = goCell.GetComponent<EventEditorTextureSelectCellPrefabInfo>();
			cellInfo.goSelected.SetActive(index == this._dataIndex);
			Texture2D texture = null;
			bool flag = this._assetCode == 0;
			if (flag)
			{
				texture = this._internalTextures[index];
			}
			else
			{
				bool flag2 = 1 == this._assetCode;
				if (flag2)
				{
					texture = this._externalTextureInfos[index].Texture;
				}
			}
			cellInfo.rawImgTexture.texture = texture;
			cellInfo.btn.ClearAndAddListener(delegate
			{
				bool flag3 = this._lastSelectGoCellItem != null;
				if (flag3)
				{
					this._lastSelectGoCellItem.GetComponent<EventEditorTextureSelectCellPrefabInfo>().goSelected.SetActive(false);
				}
				cellInfo.goSelected.SetActive(true);
				Action<Texture2D> onSelectUpdate = this.OnSelectUpdate;
				if (onSelectUpdate != null)
				{
					onSelectUpdate(texture);
				}
				this._dataIndex = index;
				this._lastSelectGoCellItem = goCell;
				bool flag4 = this._assetCode == 1;
				if (flag4)
				{
					this._lastSelectedExternalTextureInfo = this._externalTextureInfos[index];
				}
			});
		}

		// Token: 0x06004D00 RID: 19712 RVA: 0x00245903 File Offset: 0x00243B03
		private void OnSearchInputValueChanged(string searchKey)
		{
			this.Refresh();
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x00245910 File Offset: 0x00243B10
		private void OnToggleGroupValueChanged(int newIndex, int preIndex)
		{
			this._assetCode = (byte)newIndex;
			this._dataIndex = -1;
			bool flag = this._assetCode == 2;
			if (flag)
			{
				Action<Texture2D> onSelectUpdate = this.OnSelectUpdate;
				if (onSelectUpdate != null)
				{
					onSelectUpdate(null);
				}
			}
			this.Refresh();
		}

		// Token: 0x04003558 RID: 13656
		private List<Texture2D> _internalTextures;

		// Token: 0x04003559 RID: 13657
		private List<ExternalTextureInfo> _externalTextureInfos;

		// Token: 0x0400355A RID: 13658
		private byte _assetCode;

		// Token: 0x0400355B RID: 13659
		private int _dataIndex;

		// Token: 0x0400355C RID: 13660
		private GameObject _lastSelectGoCellItem;

		// Token: 0x0400355D RID: 13661
		private ExternalTextureInfo _lastSelectedExternalTextureInfo;

		// Token: 0x0400355E RID: 13662
		[NonSerialized]
		public Action<Texture2D> OnSelectUpdate;

		// Token: 0x0400355F RID: 13663
		[NonSerialized]
		public Action<byte> OnSelectComplete;

		// Token: 0x04003560 RID: 13664
		public static EventEditorTextureSelect Instance;

		// Token: 0x04003561 RID: 13665
		[SerializeField]
		private RectTransform mainWindow;

		// Token: 0x04003562 RID: 13666
		[SerializeField]
		private CToggleGroup tabButtons;

		// Token: 0x04003563 RID: 13667
		[SerializeField]
		private InfinityScroll textureScroll;

		// Token: 0x04003564 RID: 13668
		[SerializeField]
		private TMP_InputField searchInputField;
	}
}
