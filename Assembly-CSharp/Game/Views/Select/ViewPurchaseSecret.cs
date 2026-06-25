using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Information;
using Game.Components.SortAndFilter.Secret;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using GameData.Utilities.Information;
using TMPro;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007AB RID: 1963
	public class ViewPurchaseSecret : UIBase
	{
		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x06005EE8 RID: 24296 RVA: 0x002B86EF File Offset: 0x002B68EF
		private int Own
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>().TaiwuResources.Resources[6];
			}
		}

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x06005EE9 RID: 24297 RVA: 0x002B8702 File Offset: 0x002B6902
		private int Cost
		{
			get
			{
				return (this._selected.Count == 0) ? 0 : this._selected.Sum((int index) => this._data[index].Data.ShopValue);
			}
		}

		// Token: 0x06005EEA RID: 24298 RVA: 0x002B872C File Offset: 0x002B692C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._data.Clear();
			this._selected.Clear();
			this._characterDisplayData.Clear();
			SecretInformationDisplayPackage package;
			bool flag = argsBox.Get<SecretInformationDisplayPackage>("secretInformation", out package) && package != null;
			if (flag)
			{
				foreach (KeyValuePair<int, CharacterDisplayData> keyValuePair in package.CharacterData)
				{
					int num;
					CharacterDisplayData characterDisplayData;
					keyValuePair.Deconstruct(out num, out characterDisplayData);
					int id = num;
					CharacterDisplayData data = characterDisplayData;
					this._characterDisplayData[id] = data;
				}
				foreach (SecretInformationDisplayData data2 in package.SecretInformationDisplayDataList)
				{
					this._data.Add(new SecretSortAndFilterData
					{
						Data = data2,
						Characters = new Dictionary<int, CharacterDisplayData>(),
						LevelScore = -1,
						Relation = -1
					});
				}
			}
			bool flag2 = argsBox.Get<ExchangeAdvantage>("ExchangeData", out this._exchangeData);
			if (flag2)
			{
				this._isPurchase = false;
				this.titleLabel.text = LanguageKey.LK_SecretInformation_Exchange_Title.Tr();
				this.btnAll.gameObject.SetActive(false);
				this.moneyIcon.SetActive(false);
				this.countLabel.text = LanguageKey.LK_Exchange_Advantage.Tr();
				TMP_Text tmp_Text = this.ownLabel;
				int num = this._exchangeData.TaiwuAdvantage;
				tmp_Text.text = num.ToString();
			}
			else
			{
				this._isPurchase = true;
				this.titleLabel.text = LanguageKey.LK_SecretInformation_Buy_Title.Tr();
				this.btnAll.gameObject.SetActive(true);
				this.moneyIcon.SetActive(true);
				TMP_Text tmp_Text2 = this.ownLabel;
				int num = this.Own;
				tmp_Text2.text = num.ToString();
			}
			object callback;
			bool flag3 = argsBox.Get<object>("callback", out callback);
			if (flag3)
			{
				this._onSecretInformationConfirm = (callback as Action<SecretInformationDisplayData>);
			}
			else
			{
				this._onSecretInformationConfirm = null;
			}
			argsBox.Get("characterId", out this._shopCharId);
		}

		// Token: 0x06005EEB RID: 24299 RVA: 0x002B8970 File Offset: 0x002B6B70
		private void Awake()
		{
			this.canSelectScroll.InitPageCount();
			this.canSelectScroll.OnItemRender += this.OnRenderCanSelect;
			this.selectedScroll.InitPageCount();
			this.selectedScroll.OnItemRender += this.OnRenderSelected;
			this.btnClear.ClearAndAddListener(new Action(this.OnClickClear));
			this.btnAll.ClearAndAddListener(new Action(this.OnClickAll));
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x06005EEC RID: 24300 RVA: 0x002B8A27 File Offset: 0x002B6C27
		private void OnEnable()
		{
			this.RequestData();
		}

		// Token: 0x06005EED RID: 24301 RVA: 0x002B8A31 File Offset: 0x002B6C31
		private void OnDisable()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("ShopActionComplete", true);
		}

		// Token: 0x06005EEE RID: 24302 RVA: 0x002B8A40 File Offset: 0x002B6C40
		private void RequestData()
		{
			InformationDomainMethod.AsyncCall.GetSecretInformationLevelFactor(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._secretInformationLevelFactors);
				this.RefreshData();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x002B8A58 File Offset: 0x002B6C58
		private void RefreshData()
		{
			using (List<SecretSortAndFilterData>.Enumerator enumerator = this._data.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ViewPurchaseSecret.<>c__DisplayClass29_0 CS$<>8__locals1 = new ViewPurchaseSecret.<>c__DisplayClass29_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.data = enumerator.Current;
					SecretInformationItem config = SecretInformation.Instance[CS$<>8__locals1.data.Data.SecretInformationTemplateId];
					int relationScore = 0;
					int charScore = 0;
					int itemScore = 0;
					short valueScore = config.SortValue;
					byte[] parametersPack = CS$<>8__locals1.data.Data.ParametersPack;
					if (parametersPack != null)
					{
						parametersPack.ExtractSecretParameters(config, delegate(int _, int charId)
						{
							CharacterDisplayData characterDisplayData = CS$<>8__locals1.<>4__this._characterDisplayData[charId];
							CS$<>8__locals1.data.Characters[charId] = characterDisplayData;
							charScore += (Organization.Instance[characterDisplayData.OrgInfo.OrgTemplateId].IsSect ? GlobalConfig.Instance.SecretSectFactor[(int)characterDisplayData.OrgInfo.Grade] : GlobalConfig.Instance.SecretNonSectFactor[(int)characterDisplayData.OrgInfo.Grade]);
							bool flag = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
							if (flag)
							{
								int relationScore = relationScore;
								int[] secretRelationFactor = GlobalConfig.Instance.SecretRelationFactor;
								relationScore += secretRelationFactor[secretRelationFactor.Length - 1];
								CS$<>8__locals1.data.Relation = short.MaxValue;
							}
							else
							{
								int highest = 0;
								for (short type = 0; type <= 9; type += 1)
								{
									bool flag2 = type == 4;
									if (flag2)
									{
										bool flag3 = characterDisplayData.OrgInfo.OrgTemplateId == 16;
										if (flag3)
										{
											bool flag4 = highest < GlobalConfig.Instance.SecretRelationFactor[(int)type];
											if (flag4)
											{
												highest = GlobalConfig.Instance.SecretRelationFactor[(int)type];
												CS$<>8__locals1.data.Relation = type;
											}
										}
									}
									else
									{
										foreach (sbyte typeId in RelationDisplayType.Instance[type].RelationTypeIds)
										{
											ushort relationType = RelationType.GetRelationType(typeId);
											bool flag5 = RelationType.HasRelation(characterDisplayData.RelationToTaiwu, relationType) || RelationType.HasRelation(characterDisplayData.RelationFromTaiwu, relationType);
											if (flag5)
											{
												bool flag6 = highest < GlobalConfig.Instance.SecretRelationFactor[(int)type];
												if (flag6)
												{
													highest = GlobalConfig.Instance.SecretRelationFactor[(int)type];
													CS$<>8__locals1.data.Relation = type;
												}
											}
										}
									}
								}
								int relationScore;
								relationScore += highest;
							}
						}, null, null, delegate(int _, ItemKey itemKey)
						{
							itemScore += (itemKey.IsValid() ? GlobalConfig.Instance.SecretItemFactor[(int)ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId)] : 0);
						}, null, null, null);
					}
					CS$<>8__locals1.data.LevelScore = relationScore * GlobalConfig.Instance.SecretRelationScore[this._secretInformationLevelFactors[2] + 1] + charScore * GlobalConfig.Instance.SecretCharScore[this._secretInformationLevelFactors[1] + 1] + itemScore * GlobalConfig.Instance.SecretItemScore[this._secretInformationLevelFactors[3] + 1] + (int)valueScore * GlobalConfig.Instance.SecretSortValueScore[this._secretInformationLevelFactors[0] + 1];
				}
			}
			this.Refresh();
		}

		// Token: 0x06005EF0 RID: 24304 RVA: 0x002B8BDC File Offset: 0x002B6DDC
		private void Refresh()
		{
			bool isPurchase = this._isPurchase;
			if (isPurchase)
			{
				int cost = this.Cost;
				this.canSelectScroll.SetDataCount(this._data.Count);
				this.selectedScroll.SetDataCount(this._selected.Count);
				this.countLabel.text = LanguageKey.LK_SecretInformation_Buy_Count.TrFormat(this._selected.Count);
				this.costLabel.text = ((cost == 0) ? "" : string.Format("-{0}", cost));
				this.btnClear.interactable = (this._selected.Count != 0);
				this.btnAll.interactable = (this.GetCanBuyCount() != 0);
				this.btnConfirm.interactable = (this._selected.Count != 0 && cost <= this.Own);
				bool interactable = this.btnConfirm.interactable;
				if (interactable)
				{
					this.tips.enabled = false;
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					bool flag = this._selected.Count == 0;
					if (flag)
					{
						sb.AppendLine(LanguageKey.LK_SecretInformation_Not_Selected.Tr());
					}
					bool flag2 = cost > this.Own;
					if (flag2)
					{
						sb.AppendLine(LanguageKey.LK_SecretInformation_Not_Enough_Money.Tr());
					}
					this.tips.PresetParam = new string[]
					{
						sb.ToString()
					};
					this.tips.enabled = true;
				}
			}
			else
			{
				this.canSelectScroll.SetDataCount(this._data.Count);
				this.selectedScroll.SetDataCount(this._selected.Count);
				this.costLabel.text = ((this._selected.Count == 0) ? "" : string.Format("+{0}", this._exchangeData.CalcSecretAdvantage(this._data[this._selected[0]].Data)));
				this.btnClear.interactable = (this._selected.Count != 0);
				this.btnConfirm.interactable = (this._selected.Count != 0);
			}
		}

		// Token: 0x06005EF1 RID: 24305 RVA: 0x002B8E24 File Offset: 0x002B7024
		private void OnRenderCanSelect(int index, GameObject obj)
		{
			obj.GetComponent<CToggle>().SetIsOnWithoutNotify(this._selected.Contains(index));
			obj.GetComponent<SecretInformationCardItem>().Set(index, new Action<int, bool>(this.OnSelectSecret), this._data[index]);
			obj.GetComponent<PropertyItem>().SetValue(this._isPurchase ? this._data[index].Data.ShopValue.ToString() : this._exchangeData.CalcSecretAdvantage(this._data[index].Data).ToString());
			obj.GetComponent<PropertyItem>().SetIconEnable(this._isPurchase);
		}

		// Token: 0x06005EF2 RID: 24306 RVA: 0x002B8ED8 File Offset: 0x002B70D8
		private void OnRenderSelected(int index, GameObject obj)
		{
			obj.GetComponent<CToggle>().SetIsOnWithoutNotify(false);
			obj.GetComponent<SecretInformationCardItem>().Set(index, new Action<int, bool>(this.OnClickDeselect), this._data[this._selected[index]]);
			obj.GetComponent<PropertyItem>().SetValue(this._isPurchase ? this._data[this._selected[index]].Data.ShopValue.ToString() : this._exchangeData.CalcSecretAdvantage(this._data[this._selected[index]].Data).ToString());
			obj.GetComponent<PropertyItem>().SetIconEnable(this._isPurchase);
		}

		// Token: 0x06005EF3 RID: 24307 RVA: 0x002B8FA0 File Offset: 0x002B71A0
		private void OnSelectSecret(int index, bool value)
		{
			bool isPurchase = this._isPurchase;
			if (isPurchase)
			{
				if (value)
				{
					this._selected.Add(index);
				}
				else
				{
					this._selected.Remove(index);
				}
			}
			else
			{
				this._selected.Clear();
				this._selected.Add(index);
			}
			this.Refresh();
		}

		// Token: 0x06005EF4 RID: 24308 RVA: 0x002B8FFF File Offset: 0x002B71FF
		private void OnClickDeselect(int index, bool value)
		{
			this._selected.RemoveAt(index);
			this.Refresh();
		}

		// Token: 0x06005EF5 RID: 24309 RVA: 0x002B9016 File Offset: 0x002B7216
		private void OnClickClear()
		{
			this._selected.Clear();
			this.Refresh();
		}

		// Token: 0x06005EF6 RID: 24310 RVA: 0x002B902C File Offset: 0x002B722C
		private void OnClickAll()
		{
			int cost = this.Cost;
			for (int i = 0; i < this._data.Count; i++)
			{
				bool flag = this._selected.Contains(i);
				if (!flag)
				{
					bool flag2 = cost + this._data[i].Data.ShopValue > this.Own;
					if (!flag2)
					{
						this._selected.Add(i);
						cost += this._data[i].Data.ShopValue;
					}
				}
			}
			this.Refresh();
		}

		// Token: 0x06005EF7 RID: 24311 RVA: 0x002B90C4 File Offset: 0x002B72C4
		private void OnClickConfirm()
		{
			bool flag = this._onSecretInformationConfirm != null;
			if (flag)
			{
				this._onSecretInformationConfirm(this._data[this._selected[0]].Data);
				this._selected.Clear();
				this.QuickHide();
			}
			else
			{
				List<IntPair> res = new List<IntPair>();
				this._selected.Sort((int a, int b) => -a.CompareTo(b));
				foreach (int index in this._selected)
				{
					res.Add(new IntPair((int)this._data[index].Data.SecretInformationId, this._data[index].Data.ShopValue));
					this._data.RemoveAt(index);
				}
				this._selected.Clear();
				this.Refresh();
				InformationDomainMethod.Call.SettleSecretInformationShopTrade(res, this._shopCharId);
			}
		}

		// Token: 0x06005EF8 RID: 24312 RVA: 0x002B9200 File Offset: 0x002B7400
		private int GetCanBuyCount()
		{
			int cost = this.Cost;
			int count = 0;
			for (int i = 0; i < this._data.Count; i++)
			{
				bool flag = this._selected.Contains(i);
				if (!flag)
				{
					bool flag2 = cost + this._data[i].Data.ShopValue > this.Own;
					if (!flag2)
					{
						count++;
						cost += this._data[i].Data.ShopValue;
					}
				}
			}
			return count;
		}

		// Token: 0x040041B2 RID: 16818
		[SerializeField]
		private InfinityScroll canSelectScroll;

		// Token: 0x040041B3 RID: 16819
		[SerializeField]
		private InfinityScroll selectedScroll;

		// Token: 0x040041B4 RID: 16820
		[SerializeField]
		private TextMeshProUGUI countLabel;

		// Token: 0x040041B5 RID: 16821
		[SerializeField]
		private TextMeshProUGUI ownLabel;

		// Token: 0x040041B6 RID: 16822
		[SerializeField]
		private TextMeshProUGUI costLabel;

		// Token: 0x040041B7 RID: 16823
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040041B8 RID: 16824
		[SerializeField]
		private CButton btnClear;

		// Token: 0x040041B9 RID: 16825
		[SerializeField]
		private CButton btnAll;

		// Token: 0x040041BA RID: 16826
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x040041BB RID: 16827
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040041BC RID: 16828
		[SerializeField]
		private GameObject moneyIcon;

		// Token: 0x040041BD RID: 16829
		[SerializeField]
		private TooltipInvoker tips;

		// Token: 0x040041BE RID: 16830
		private List<SecretSortAndFilterData> _data = new List<SecretSortAndFilterData>();

		// Token: 0x040041BF RID: 16831
		private List<int> _selected = new List<int>();

		// Token: 0x040041C0 RID: 16832
		private Dictionary<int, CharacterDisplayData> _characterDisplayData = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x040041C1 RID: 16833
		private int _shopCharId;

		// Token: 0x040041C2 RID: 16834
		private bool _isPurchase;

		// Token: 0x040041C3 RID: 16835
		private Action<SecretInformationDisplayData> _onSecretInformationConfirm;

		// Token: 0x040041C4 RID: 16836
		private ExchangeAdvantage _exchangeData;

		// Token: 0x040041C5 RID: 16837
		private int[] _secretInformationLevelFactors = new int[4];
	}
}
