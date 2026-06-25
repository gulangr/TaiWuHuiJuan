using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Alertness
{
	// Token: 0x02000C5F RID: 3167
	public class ViewAlertness : UIBase
	{
		// Token: 0x0600A168 RID: 41320 RVA: 0x004B6D0C File Offset: 0x004B4F0C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("charId", out this._charId);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.CallRefresh));
		}

		// Token: 0x0600A169 RID: 41321 RVA: 0x004B6D5A File Offset: 0x004B4F5A
		private void Awake()
		{
			PoolManager.SetSrcObjectWithTurnOff("ViewAlertnessRecordItemTemplate", this.recordItemTemplate);
		}

		// Token: 0x0600A16A RID: 41322 RVA: 0x004B6D6E File Offset: 0x004B4F6E
		private void OnDestroy()
		{
			PoolManager.RemoveData("ViewAlertnessRecordItemTemplate");
		}

		// Token: 0x0600A16B RID: 41323 RVA: 0x004B6D7C File Offset: 0x004B4F7C
		private void CallRefresh()
		{
			CharacterDomainMethod.Call.GetAlertnessData(this.Element.GameDataListenerId, this._charId, false);
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._charId);
		}

		// Token: 0x0600A16C RID: 41324 RVA: 0x004B6DB0 File Offset: 0x004B4FB0
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 215;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._alertnessData);
							this.Refresh();
						}
						else
						{
							bool flag3 = notification.MethodId == 131;
							if (flag3)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._charData);
								this.RefreshCharacterInfo();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A16D RID: 41325 RVA: 0x004B6E98 File Offset: 0x004B5098
		private void Refresh()
		{
			string levelSprite = CommonUtils.GetAlertnessIcon((int)this._alertnessData.Level);
			this.imageLevel.SetSprite(levelSprite, false, null);
			this.textLevel.text = CommonUtils.GetAlertnessName((int)this._alertnessData.Level);
			this.textValue.text = ViewAlertness.GetTextValue(this._alertnessData);
			this.textEffectChangeFavor.text = ViewAlertness.GetTextEffectChangeFavor(this._alertnessData);
			this.textEffectInteract.text = ViewAlertness.GetTextEffectInteract(this._alertnessData);
			this.textEffectExchange.text = ViewAlertness.GetTextEffectExchange(this._alertnessData);
			this.textEffectMaxFavor.text = ViewAlertness.GetTextEffectMaxFavor(this._alertnessData);
			this.RefreshRecord();
		}

		// Token: 0x0600A16E RID: 41326 RVA: 0x004B6F5C File Offset: 0x004B515C
		public static string GetTextValue(CharacterAlertnessData alertnessData)
		{
			int value = (alertnessData != null) ? alertnessData.Value : 0;
			return string.Format("{0:+0;-0;0}", value).SetColor((value <= 0) ? "8dc3c3" : "ec5f68");
		}

		// Token: 0x0600A16F RID: 41327 RVA: 0x004B6FA0 File Offset: 0x004B51A0
		public static string GetTextEffectChangeFavor(CharacterAlertnessData alertnessData)
		{
			bool flag = alertnessData == null;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				int effect = CharacterAlertnessData.GetEffectChangeFavor((int)alertnessData.Level);
				int defaultEffect = CharacterAlertnessData.GetEffectChangeFavor((int)CharacterAlertnessData.LevelNormal);
				string color = (effect >= defaultEffect) ? "8dc3c3" : "ec5f68";
				result = string.Format("{0:+0;-0}%", effect).SetColor(color);
			}
			return result;
		}

		// Token: 0x0600A170 RID: 41328 RVA: 0x004B7004 File Offset: 0x004B5204
		public static string GetTextEffectInteract(CharacterAlertnessData alertnessData)
		{
			bool flag = alertnessData == null;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				int effect = CharacterAlertnessData.GetEffectInteract((int)alertnessData.Level);
				int defaultEffect = CharacterAlertnessData.GetEffectInteract((int)CharacterAlertnessData.LevelNormal);
				string color = (effect >= defaultEffect) ? "8dc3c3" : "ec5f68";
				result = string.Format("{0}%", effect).SetColor(color);
			}
			return result;
		}

		// Token: 0x0600A171 RID: 41329 RVA: 0x004B7068 File Offset: 0x004B5268
		public static string GetTextEffectExchange(CharacterAlertnessData alertnessData)
		{
			bool flag = alertnessData == null;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				int effect = CharacterAlertnessData.GetEffectExchange((int)alertnessData.Level);
				int defaultEffect = CharacterAlertnessData.GetEffectExchange((int)CharacterAlertnessData.LevelNormal);
				string color = (effect <= defaultEffect) ? "8dc3c3" : "ec5f68";
				result = string.Format("{0:+0;-0;}%", effect).SetColor(color);
			}
			return result;
		}

		// Token: 0x0600A172 RID: 41330 RVA: 0x004B70CC File Offset: 0x004B52CC
		public static string GetTextEffectMaxFavor(CharacterAlertnessData alertnessData)
		{
			bool flag = alertnessData == null;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				short maxFavor = alertnessData.MaxFavor;
				sbyte maxFavorLevel = FavorabilityType.GetFavorabilityType(maxFavor);
				string maxFavorName = CommonUtils.GetFavorStringByLevel(maxFavorLevel);
				result = maxFavorName;
			}
			return result;
		}

		// Token: 0x0600A173 RID: 41331 RVA: 0x004B7108 File Offset: 0x004B5308
		private void RefreshRecord()
		{
			this._renderInfos.Clear();
			this._argumentCollection.Clear();
			this._renderedArgumentCollection.Clear();
			for (int index = 0; index < this._recordDataArray.Length; index++)
			{
				List<string> list = this._recordDataArray[index];
				bool flag = list == null;
				if (flag)
				{
					list = new List<string>();
					this._recordDataArray[index] = list;
				}
				list.Clear();
			}
			bool flag2 = this._alertnessData.RecordCollection == null || this._alertnessData.RecordCollection.Count == 0;
			if (flag2)
			{
				this.RefreshRecordGroup();
				this.Element.ShowAfterRefresh();
			}
			else
			{
				this._alertnessData.RecordCollection.GetRenderInfos(this._renderInfos, this._argumentCollection);
				string key = "ViewAlertness";
				this._charIdList.Clear();
				this._charIdList.AddRange(this._argumentCollection.Characters);
				this._charIdList.AddRange(this._argumentCollection.CharacterRealNames);
				RecordArgumentsRequest argRequest = new RecordArgumentsRequest(this._argumentCollection)
				{
					Characters = this._charIdList
				};
				LifeRecordDomainMethod.AsyncCall.GetRecordRenderInfoArguments(this, key, argRequest, delegate(int offset, RawDataPool dataPool)
				{
					ArgumentCollectionRenderArguments dynamicArguments = null;
					Serializer.Deserialize(dataPool, offset, ref dynamicArguments);
					GameMessageUtils.RenderDynamicArguments(dynamicArguments, this._argumentCollection, this._renderedArgumentCollection, true, true);
					GameMessageUtils.RenderFixedArguments(this._argumentCollection, this._renderedArgumentCollection, true);
					foreach (CharacterAlertnessRecordRenderInfo renderInfo in this._renderInfos)
					{
						object[] fillParams = new object[renderInfo.Arguments.Count];
						sbyte i = 0;
						while ((int)i < fillParams.Length)
						{
							string paramStr = this._renderedArgumentCollection.Get(renderInfo.Arguments[(int)i].Item1, renderInfo.Arguments[(int)i].Item2);
							bool flag3 = (int)i == fillParams.Length - 1;
							if (flag3)
							{
								int value = int.Parse(paramStr.RemoveColorTags());
								string color = (value > 0) ? "ec5f68" : ((value < 0) ? "8dc3c3" : "brightyellow");
								paramStr = string.Format("{0:+0;-0;0}", value).SetColor(color);
							}
							fillParams[(int)i] = paramStr;
							bool flag4 = string.IsNullOrEmpty(paramStr);
							if (flag4)
							{
								AdaptableLog.Error("Failed to render samsara record " + renderInfo.Text + " because of insufficient argument count.");
							}
							i += 1;
						}
						string desc = string.Format(renderInfo.Text, fillParams).ColorReplace();
						CharacterAlertnessRecordItem config = CharacterAlertnessRecord.Instance[renderInfo.RecordType];
						int type = config.Type.ToInt();
						List<string> list2 = this._recordDataArray[type];
						list2.Add(desc);
					}
					this.RefreshRecordGroup();
					this.Element.ShowAfterRefresh();
				});
			}
		}

		// Token: 0x0600A174 RID: 41332 RVA: 0x004B7254 File Offset: 0x004B5454
		private void RefreshRecordGroup()
		{
			for (int i = 0; i < this._recordDataArray.Length; i++)
			{
				List<string> data = this._recordDataArray[i];
				AlertnessRecordGroup group = this.recordGroupArray[i];
				ECharacterAlertnessRecordType type = (ECharacterAlertnessRecordType)i;
				if (!true)
				{
				}
				string text;
				switch (type)
				{
				case ECharacterAlertnessRecordType.Initial:
					text = LanguageKey.LK_Alertness_Effect_Record_Initial.Tr();
					break;
				case ECharacterAlertnessRecordType.Interact:
					text = LanguageKey.LK_Alertness_Effect_Record_Interact.Tr();
					break;
				case ECharacterAlertnessRecordType.Trade:
					text = LanguageKey.LK_Alertness_Effect_Record_Trade.Tr();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				string title = text;
				group.SetData(title, data, "ViewAlertnessRecordItemTemplate");
			}
		}

		// Token: 0x0600A175 RID: 41333 RVA: 0x004B72F4 File Offset: 0x004B54F4
		private void RefreshCharacterInfo()
		{
			string nameStr = NameCenter.GetMonasticTitleOrDisplayName(this._charData, this._charData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			this.textTitle.text = LocalStringManager.GetFormat(LanguageKey.LK_Character_Character_Alertness, nameStr);
		}

		// Token: 0x0600A176 RID: 41334 RVA: 0x004B733C File Offset: 0x004B553C
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonCloseView";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x04007D29 RID: 32041
		[SerializeField]
		private CImage imageLevel;

		// Token: 0x04007D2A RID: 32042
		[SerializeField]
		private TextMeshProUGUI textLevel;

		// Token: 0x04007D2B RID: 32043
		[SerializeField]
		private TextMeshProUGUI textValue;

		// Token: 0x04007D2C RID: 32044
		[SerializeField]
		private TextMeshProUGUI textEffectChangeFavor;

		// Token: 0x04007D2D RID: 32045
		[SerializeField]
		private TextMeshProUGUI textEffectMaxFavor;

		// Token: 0x04007D2E RID: 32046
		[SerializeField]
		private TextMeshProUGUI textEffectInteract;

		// Token: 0x04007D2F RID: 32047
		[SerializeField]
		private TextMeshProUGUI textEffectExchange;

		// Token: 0x04007D30 RID: 32048
		[SerializeField]
		private AlertnessRecordGroup[] recordGroupArray;

		// Token: 0x04007D31 RID: 32049
		[SerializeField]
		private GameObject recordItemTemplate;

		// Token: 0x04007D32 RID: 32050
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04007D33 RID: 32051
		private const string RecordItemTemplateKey = "ViewAlertnessRecordItemTemplate";

		// Token: 0x04007D34 RID: 32052
		private int _charId;

		// Token: 0x04007D35 RID: 32053
		private CharacterDisplayData _charData;

		// Token: 0x04007D36 RID: 32054
		private CharacterAlertnessData _alertnessData;

		// Token: 0x04007D37 RID: 32055
		private readonly ArgumentCollection _argumentCollection = new ArgumentCollection();

		// Token: 0x04007D38 RID: 32056
		private readonly RenderedArgumentCollection _renderedArgumentCollection = new RenderedArgumentCollection();

		// Token: 0x04007D39 RID: 32057
		private readonly List<CharacterAlertnessRecordRenderInfo> _renderInfos = new List<CharacterAlertnessRecordRenderInfo>();

		// Token: 0x04007D3A RID: 32058
		private readonly List<string>[] _recordDataArray = new List<string>[ECharacterAlertnessRecordType.Count.ToInt()];

		// Token: 0x04007D3B RID: 32059
		private readonly List<int> _charIdList = new List<int>();

		// Token: 0x04007D3C RID: 32060
		private const string Blue = "8dc3c3";

		// Token: 0x04007D3D RID: 32061
		private const string Red = "ec5f68";
	}
}
