using System;
using FrameWork;
using Game.Components.Item;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.LifeSkillCombat
{
	// Token: 0x02000987 RID: 2439
	public class ViewAiForceSilenceDialog : UIBase
	{
		// Token: 0x06007529 RID: 29993 RVA: 0x003693C4 File Offset: 0x003675C4
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				argsBox.Get<CharacterDisplayData>("charData", out this._charData);
				argsBox.Get("isItem", out this._isItem);
				argsBox.Get("AiThreaten", out this._isThreaten);
				argsBox.Get<ItemDisplayData>("itemDisplayData", out this._itemDisplayData);
				argsBox.Get<SecretInformationDisplayData>("secretInformationDisplayData", out this._secretInformationDisplayData);
				argsBox.Get<SecretInformationDisplayPackage>("secretInformationDisplayPackage", out this._secretInformationDisplayPackage);
				argsBox.Get<Action>("onYes", out this._onYesCallback);
				argsBox.Get<Action>("onNo", out this._onNoCallback);
				UIElement element = this.Element;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			}
		}

		// Token: 0x0600752A RID: 29994 RVA: 0x00369498 File Offset: 0x00367698
		private void OnListenerIdReady()
		{
			this.itemHolder.gameObject.SetActive(this._isItem && !this._isThreaten);
			bool isThreaten = this._isThreaten;
			if (isThreaten)
			{
				this.contentText.text = "？？？";
			}
			else
			{
				bool isItem = this._isItem;
				if (isItem)
				{
					this.SetItemView(this._itemDisplayData);
					sbyte itemGrade = ItemTemplateHelper.GetGrade(this._itemDisplayData.Key.ItemType, this._itemDisplayData.Key.TemplateId);
					string itemName = ItemTemplateHelper.GetName(this._itemDisplayData.Key.ItemType, this._itemDisplayData.Key.TemplateId);
					string charName = NameCenter.GetMonasticTitleOrDisplayName(this._charData, false);
					this.contentText.text = LocalStringManager.GetFormat(LanguageKey.UI_LifeSkillBattle_Prepare_AiOperation_Bribery_Desc, charName, itemName.SetGradeColor((int)itemGrade));
				}
			}
		}

		// Token: 0x0600752B RID: 29995 RVA: 0x00369588 File Offset: 0x00367788
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "BtnYes" == btnName;
			if (flag)
			{
				bool isThreaten = this._isThreaten;
				if (!isThreaten)
				{
					bool isItem = this._isItem;
					if (isItem)
					{
						AdaptableLog.Info(string.Format("send TransferInventoryItemFromAToB for AiForceSilenceDialog，item is {0}", this._itemDisplayData.Key));
						CharacterDomainMethod.Call.TransferInventoryItemFromAToB(this._charData.CharacterId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, this._itemDisplayData.Key, this._itemDisplayData.Amount);
					}
					else
					{
						InformationDomainMethod.Call.DisseminateSecretInformation(this.Element.GameDataListenerId, this._secretInformationDisplayData.SecretInformationId, this._charData.CharacterId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
					}
				}
				Action onYesCallback = this._onYesCallback;
				if (onYesCallback != null)
				{
					onYesCallback();
				}
				UIManager.Instance.HideUI(this.Element);
			}
			else
			{
				bool flag2 = "BtnNo" == btnName;
				if (flag2)
				{
					Action onNoCallback = this._onNoCallback;
					if (onNoCallback != null)
					{
						onNoCallback();
					}
					UIManager.Instance.HideUI(this.Element);
				}
			}
		}

		// Token: 0x0600752C RID: 29996 RVA: 0x003696B0 File Offset: 0x003678B0
		private void SetItemView(ItemDisplayData itemDisplayData)
		{
			this.itemBack.Set(itemDisplayData, false);
			bool isInvalid = itemDisplayData.Key.Equals(ItemKey.Invalid);
			sbyte itemGrade = ItemTemplateHelper.GetGrade(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
			string itemName = ItemTemplateHelper.GetName(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
			this.itemNameLabel.text = itemName.SetGradeColor((int)itemGrade);
			this.itemValue.text = (isInvalid ? "0" : itemDisplayData.Value.ToString());
			this.itemWeight.text = (isInvalid ? "0" : NumberFormatUtils.FormatItemWeight(itemDisplayData.Weight));
		}

		// Token: 0x040057E9 RID: 22505
		[SerializeField]
		private TextMeshProUGUI contentText;

		// Token: 0x040057EA RID: 22506
		[SerializeField]
		private RectTransform itemHolder;

		// Token: 0x040057EB RID: 22507
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x040057EC RID: 22508
		[SerializeField]
		private TextMeshProUGUI itemNameLabel;

		// Token: 0x040057ED RID: 22509
		[SerializeField]
		private TextMeshProUGUI itemValue;

		// Token: 0x040057EE RID: 22510
		[SerializeField]
		private TextMeshProUGUI itemWeight;

		// Token: 0x040057EF RID: 22511
		private CharacterDisplayData _charData;

		// Token: 0x040057F0 RID: 22512
		private bool _isItem = false;

		// Token: 0x040057F1 RID: 22513
		private bool _isThreaten = false;

		// Token: 0x040057F2 RID: 22514
		private ItemDisplayData _itemDisplayData;

		// Token: 0x040057F3 RID: 22515
		private SecretInformationDisplayData _secretInformationDisplayData;

		// Token: 0x040057F4 RID: 22516
		private SecretInformationDisplayPackage _secretInformationDisplayPackage;

		// Token: 0x040057F5 RID: 22517
		private Action _onYesCallback;

		// Token: 0x040057F6 RID: 22518
		private Action _onNoCallback;
	}
}
