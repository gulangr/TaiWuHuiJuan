using System;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class UI_AiForceSilenceDialog : UIBase
{
	// Token: 0x06002550 RID: 9552 RVA: 0x00111B88 File Offset: 0x0010FD88
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

	// Token: 0x06002551 RID: 9553 RVA: 0x00111C5C File Offset: 0x0010FE5C
	private void OnListenerIdReady()
	{
		this._secretInformationTemplate.gameObject.SetActive(!this._isItem && !this._isThreaten);
		this._itemView.gameObject.SetActive(this._isItem && !this._isThreaten);
		bool isThreaten = this._isThreaten;
		if (isThreaten)
		{
			this._contentText.text = "？？？";
		}
		else
		{
			bool isItem = this._isItem;
			if (isItem)
			{
				this._itemView.SetData(this._itemDisplayData, true, -1, false, true, null, false, true);
				sbyte itemGrade = ItemTemplateHelper.GetGrade(this._itemDisplayData.Key.ItemType, this._itemDisplayData.Key.TemplateId);
				string itemName = ItemTemplateHelper.GetName(this._itemDisplayData.Key.ItemType, this._itemDisplayData.Key.TemplateId);
				string charName = NameCenter.GetMonasticTitleOrDisplayName(this._charData, false);
				this._contentText.text = LocalStringManager.GetFormat(LanguageKey.UI_LifeSkillBattle_Prepare_AiOperation_Bribery_Desc, charName, itemName.SetGradeColor((int)itemGrade));
			}
			else
			{
				InformationUtils.RefreshSecretInformationView(this._secretInformationTemplate, this._secretInformationDisplayData, this._secretInformationDisplayPackage);
				this._contentText.text = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_Tips_EnemyForceSilent_SecretInformation);
			}
		}
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x00111DA9 File Offset: 0x0010FFA9
	private void Awake()
	{
		this._secretInformationTemplate = base.CGet<Refers>("SecretInformationTemplate");
		this._itemView = base.CGet<ItemView>("ItemDetailView");
		this._contentText = base.CGet<TextMeshProUGUI>("ContentText");
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x00111DE0 File Offset: 0x0010FFE0
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

	// Token: 0x04001BC3 RID: 7107
	private CharacterDisplayData _charData;

	// Token: 0x04001BC4 RID: 7108
	private bool _isItem = false;

	// Token: 0x04001BC5 RID: 7109
	private bool _isThreaten = false;

	// Token: 0x04001BC6 RID: 7110
	private ItemDisplayData _itemDisplayData;

	// Token: 0x04001BC7 RID: 7111
	private SecretInformationDisplayData _secretInformationDisplayData;

	// Token: 0x04001BC8 RID: 7112
	private SecretInformationDisplayPackage _secretInformationDisplayPackage;

	// Token: 0x04001BC9 RID: 7113
	private Refers _secretInformationTemplate;

	// Token: 0x04001BCA RID: 7114
	private ItemView _itemView;

	// Token: 0x04001BCB RID: 7115
	private TextMeshProUGUI _contentText;

	// Token: 0x04001BCC RID: 7116
	private Action _onYesCallback;

	// Token: 0x04001BCD RID: 7117
	private Action _onNoCallback;
}
