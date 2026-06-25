using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A42 RID: 2626
	public class EventWindowSelectBookCount : MonoBehaviour
	{
		// Token: 0x17000E34 RID: 3636
		// (get) Token: 0x060081A1 RID: 33185 RVA: 0x003C6743 File Offset: 0x003C4943
		// (set) Token: 0x060081A2 RID: 33186 RVA: 0x003C674B File Offset: 0x003C494B
		public ItemKey TargetBookKey { get; private set; }

		// Token: 0x060081A3 RID: 33187 RVA: 0x003C6754 File Offset: 0x003C4954
		private void Awake()
		{
			this.btnDeselect.ClearAndAddListener(new Action(this.OpenSelectPage));
			this.btnSelectSkill.ClearAndAddListener(new Action(this.SelectSkill));
		}

		// Token: 0x060081A4 RID: 33188 RVA: 0x003C6788 File Offset: 0x003C4988
		private void SelectSkill()
		{
			SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
			{
				ItemSubType = 1001,
				OnlyFromInventory = true
			}, delegate(List<SelectedItemData> selectedItems)
			{
				bool flag = selectedItems.Count > 0;
				if (flag)
				{
					this.TargetBookKey = selectedItems[0].ItemData.Key;
					this._targetBook = ((this.TargetBookKey == ItemKey.Invalid) ? null : this._bookDatas.SingleOrDefault((ItemDisplayData t) => t.Key == this.TargetBookKey));
					this.GetReadPageInformation();
				}
			}, "", null);
			config.ExternalItems = this._bookDatas;
			config.HideSourceToggles = true;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("DisplayBg", true);
			argBox.SetObject("SelectItemConfig", config);
			UIElement.SelectItem.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SelectItem, true);
		}

		// Token: 0x060081A5 RID: 33189 RVA: 0x003C681E File Offset: 0x003C4A1E
		private void GetReadPageInformation()
		{
			TaiwuDomainMethod.AsyncCall.GetReadingResultPreview(null, this.TargetBookKey, this.CurAmount, delegate(int offset, RawDataPool dataPool)
			{
				RanshanReadingDisplayData displayData = new RanshanReadingDisplayData();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				this.OpenInformationtPage();
				this.txtDurability.text = string.Format("{0}/{1}", this._targetBook.Durability, this._targetBook.MaxDurability);
				this.SetupReadingPages(displayData);
				this.combatSkillItem.Init(this._targetBook, new Action(this.SelectSkill));
				int maxAmount = Math.Min(this._maxAmount, (int)this._targetBook.Durability);
				this.selectAmount.Rerfresh(maxAmount, 1, false, false, 1);
			});
		}

		// Token: 0x060081A6 RID: 33190 RVA: 0x003C6840 File Offset: 0x003C4A40
		private void SetupReadingPages(RanshanReadingDisplayData displayData)
		{
			CommonUtils.PrepareEnoughChildren(this.readingProgressLayout, this.readingProgressTemplate.gameObject, displayData.ReadingProgress.Length, null);
			for (int i = 0; i < displayData.ReadingProgress.Length; i++)
			{
				Refers refers = this.readingProgressLayout.GetChild(i).GetComponent<Refers>();
				refers.CGet<TextMeshProUGUI>("PageNumber").text = (i + 1).ToString();
				bool isReadfinish = this._targetBook.IsReadingFinished;
				sbyte currentProgress = displayData.ReadingProgress[i];
				int previewProgress = displayData.PreviewProgress[i];
				int displayProgress = (int)currentProgress + previewProgress;
				previewProgress = ((!isReadfinish) ? Math.Min((int)(100 - currentProgress), previewProgress) : previewProgress);
				refers.CGet<CImage>("ProgressBar").fillAmount = (float)displayProgress / 100f;
				string str = string.Format("{0}%", Mathf.FloorToInt((float)currentProgress));
				bool flag = previewProgress == 0;
				if (flag)
				{
					refers.CGet<TextMeshProUGUI>("ReadingProgress").text = string.Format("{0}%", currentProgress);
				}
				else
				{
					int value = Mathf.FloorToInt((float)previewProgress);
					string str2 = (previewProgress > 0) ? string.Format("+{0}%", value).SetColor("brightblue") : string.Format("-{0}%", value).SetColor("brightred");
					refers.CGet<TextMeshProUGUI>("ReadingProgress").text = str + str2;
				}
			}
		}

		// Token: 0x060081A7 RID: 33191 RVA: 0x003C69C8 File Offset: 0x003C4BC8
		private void OnDeselectAll()
		{
			this.infomationPage.gameObject.SetActive(false);
			this.selectPage.gameObject.SetActive(true);
			this.selectAmount.gameObject.SetActive(false);
			this.btnDeselect.gameObject.SetActive(false);
			this.txtSelectedAmount.text = "0/1";
		}

		// Token: 0x17000E35 RID: 3637
		// (get) Token: 0x060081A8 RID: 33192 RVA: 0x003C6A2F File Offset: 0x003C4C2F
		public int CurAmount
		{
			get
			{
				return this.selectAmount.CurCount;
			}
		}

		// Token: 0x060081A9 RID: 33193 RVA: 0x003C6A3C File Offset: 0x003C4C3C
		public void Refresh(int maxReadAmount, int initAmount = 1)
		{
			this._maxAmount = maxReadAmount;
			this.GetRanshanReadableBooks();
			this.OpenSelectPage();
			this.TargetBookKey = ItemKey.Invalid;
			this._targetBook = null;
			this.selectAmount.Rerfresh(maxReadAmount, 1, initAmount, false, false, 1, delegate(int newValue)
			{
				bool flag = this.TargetBookKey != ItemKey.Invalid;
				if (flag)
				{
					this.GetReadPageInformation();
				}
			});
		}

		// Token: 0x060081AA RID: 33194 RVA: 0x003C6A90 File Offset: 0x003C4C90
		private void GetRanshanReadableBooks()
		{
			CharacterDomainMethod.AsyncCall.GetAllRanshanReadBooksData(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPoll)
			{
				Serializer.Deserialize(dataPoll, offset, ref this._bookDatas);
			});
		}

		// Token: 0x060081AB RID: 33195 RVA: 0x003C6AB0 File Offset: 0x003C4CB0
		private void OpenSelectPage()
		{
			this.selectPage.gameObject.SetActive(true);
			this.infomationPage.gameObject.SetActive(false);
			this.selectAmount.gameObject.SetActive(false);
			this.btnDeselect.gameObject.SetActive(false);
			this.txtSelectedAmount.text = "0/1";
		}

		// Token: 0x060081AC RID: 33196 RVA: 0x003C6B18 File Offset: 0x003C4D18
		private void OpenInformationtPage()
		{
			this.selectPage.gameObject.SetActive(false);
			this.infomationPage.gameObject.SetActive(true);
			this.selectAmount.gameObject.SetActive(true);
			this.btnDeselect.gameObject.SetActive(true);
			this.txtSelectedAmount.text = "1/1";
		}

		// Token: 0x0400631C RID: 25372
		[SerializeField]
		private GameObject selectPage;

		// Token: 0x0400631D RID: 25373
		[SerializeField]
		private GameObject infomationPage;

		// Token: 0x0400631E RID: 25374
		[Header("titleLayout")]
		[SerializeField]
		private TextMeshProUGUI txtSelectedAmount;

		// Token: 0x0400631F RID: 25375
		[SerializeField]
		private CButton btnDeselect;

		// Token: 0x04006320 RID: 25376
		[Header("selectPage")]
		[SerializeField]
		private CButton btnSelectSkill;

		// Token: 0x04006321 RID: 25377
		[Header("infomationPage")]
		[SerializeField]
		private EventWindowItemView combatSkillItem;

		// Token: 0x04006322 RID: 25378
		[SerializeField]
		private EventWindowSetSelectAmount selectAmount;

		// Token: 0x04006323 RID: 25379
		[SerializeField]
		private RectTransform readingProgressLayout;

		// Token: 0x04006324 RID: 25380
		[SerializeField]
		private Refers readingProgressTemplate;

		// Token: 0x04006325 RID: 25381
		[SerializeField]
		private TextMeshProUGUI txtDurability;

		// Token: 0x04006326 RID: 25382
		private List<ItemDisplayData> _bookDatas;

		// Token: 0x04006327 RID: 25383
		private int _maxAmount;

		// Token: 0x04006329 RID: 25385
		private ItemDisplayData _targetBook;
	}
}
