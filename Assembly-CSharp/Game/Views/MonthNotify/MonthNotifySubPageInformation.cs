using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Information;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Information;
using Game.Components.SortAndFilter.Secret;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.World.Notification;
using GameData.Serializer;
using GameData.Utilities;
using GameData.Utilities.Information;
using TMPro;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C1 RID: 2241
	public class MonthNotifySubPageInformation : MonoBehaviour
	{
		// Token: 0x06006AC0 RID: 27328 RVA: 0x003146DC File Offset: 0x003128DC
		public void Init(Action closeAction)
		{
			this.typeToggleGroup.Init(-1);
			this.typeToggleGroup.OnActiveIndexChange += this.OnTypeToggleChange;
			this._informationSortAndFilterController = new InformationSortAndFilterController(this.informationSortAndFilter, true);
			this._informationSortAndFilterController.Init(new Action(this.OnInformationChange), "InformationSortAndFilter");
			this.informationScroll.InitPageCount();
			this.informationScroll.OnItemRender += this.OnInformationRender;
			this._secretSortAndFilterController = new SecretSortAndFilterController(this.secretSortAndFilter);
			this._secretSortAndFilterController.Init(new Action(this.OnSecretChange), "SecretSortAndFilter");
			this._secretSortAndFilterController.SetVisibleSortIds(InformationUtils.TaiwuSortId);
			this.secretScroll.InitPageCount();
			this.secretScroll.OnItemRender += this.OnSecretRender;
			this.btnClose.ClearAndAddListener(closeAction);
		}

		// Token: 0x06006AC1 RID: 27329 RVA: 0x003147D2 File Offset: 0x003129D2
		public void Set(MonthNotify data, Dictionary<int, FullBlockName> location)
		{
			this._data = data;
			this._secretInformationLocation = location;
			this.RequestSortingData();
		}

		// Token: 0x06006AC2 RID: 27330 RVA: 0x003147EA File Offset: 0x003129EA
		private void RequestSortingData()
		{
			InformationDomainMethod.AsyncCall.GetSecretInformationLevelFactor(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._secretInformationLevelFactors);
				this.Refresh();
			});
		}

		// Token: 0x06006AC3 RID: 27331 RVA: 0x00314800 File Offset: 0x00312A00
		private void Refresh()
		{
			this.GenerateSecretSortAndFilterData();
			this.OnInformationChange();
			this.typeToggleNameLabels[0].text = this._typeToggleNames[0].TrFormat(this._data.Information.Count);
			bool flag = this.typeToggleGroup.GetActiveIndex() > 0;
			if (flag)
			{
				this.OnSecretChange();
			}
		}

		// Token: 0x06006AC4 RID: 27332 RVA: 0x0031486C File Offset: 0x00312A6C
		private void GenerateSecretSortAndFilterData()
		{
			for (int i = 0; i < this.typeToggleNameLabels.Length; i++)
			{
				this._secretData[i].Clear();
			}
			foreach (SecretInformationSnapshot snapshot in this._data.SecretInformationSnapshots.Values)
			{
				Dictionary<int, SecretSortAndFilterData> dataSet = this._secretData[snapshot.Type];
				int type = snapshot.Type;
				bool flag = type == 0 || type == 1;
				if (flag)
				{
					dataSet.Add(snapshot.SecretInformationId, this.GenerateSecretSortAndFilterData(snapshot));
				}
				else
				{
					SecretSortAndFilterData data;
					bool flag2 = dataSet.TryGetValue((int)snapshot.OccurenceId, out data);
					if (flag2)
					{
						data.Data.HolderCount += snapshot.HolderCount;
					}
					else
					{
						dataSet.Add((int)snapshot.OccurenceId, this.GenerateSecretSortAndFilterData(snapshot));
					}
				}
			}
			for (int j = 1; j < this.typeToggleNameLabels.Length; j++)
			{
				int count = 0;
				foreach (SecretSortAndFilterData data2 in this._secretData[j - 1].Values)
				{
					count += Math.Max(data2.Data.HolderCount, 1);
				}
				this.typeToggleNameLabels[j].text = this._typeToggleNames[j].TrFormat(count);
			}
		}

		// Token: 0x06006AC5 RID: 27333 RVA: 0x00314A44 File Offset: 0x00312C44
		private SecretSortAndFilterData GenerateSecretSortAndFilterData(SecretInformationSnapshot snapshot)
		{
			SecretInformationItem config = SecretInformation.Instance[snapshot.SecretInformationTemplateId];
			int relationScore = 0;
			int charScore = 0;
			int itemScore = 0;
			int highestRelationScore = int.MinValue;
			short valueScore = config.SortValue;
			SecretSortAndFilterData data = new SecretSortAndFilterData
			{
				Data = new SecretInformationDisplayData
				{
					SecretInformationId = (SecretInformationId)snapshot.SecretInformationId,
					SecretInformationTemplateId = snapshot.SecretInformationTemplateId,
					HolderCount = snapshot.HolderCount,
					SourceCharacterId = snapshot.SourceCharacterId,
					IsInBroadcast = snapshot.IsInBroadcast,
					AuthorityCostWhenDisseminating = snapshot.AuthorityCost,
					UsedCount = snapshot.UsedCount,
					Location = this._secretInformationLocation[snapshot.SecretInformationId],
					OccurenceDate = snapshot.OccurenceDate,
					DisseminationRate = -1,
					ParametersPack = snapshot.ParametersPack
				},
				Characters = new Dictionary<int, CharacterDisplayData>(),
				Relation = -1,
				LevelScore = -1,
				Date = this._data.Date
			};
			byte[] parametersPack = snapshot.ParametersPack;
			if (parametersPack != null)
			{
				parametersPack.ExtractSecretParameters(config, delegate(int _, int charId)
				{
					NameRelatedData nameData = this._data.CharacterNames[charId].NameRelatedData;
					short relation = this._data.TaiwuRelations[charId];
					if (!true)
					{
					}
					int num;
					if (relation != -1)
					{
						if (relation != 32767)
						{
							num = GlobalConfig.Instance.SecretRelationFactor[(int)relation];
						}
						else
						{
							int[] secretRelationFactor = GlobalConfig.Instance.SecretRelationFactor;
							num = secretRelationFactor[secretRelationFactor.Length - 1];
						}
					}
					else
					{
						num = 0;
					}
					if (!true)
					{
					}
					int score = num;
					data.Characters[charId] = new CharacterDisplayData
					{
						AvatarRelatedData = this._data.Avatars.GetValueOrDefault(charId),
						TemplateId = nameData.CharTemplateId,
						Gender = nameData.Gender,
						MonkType = nameData.MonkType,
						FullName = nameData.FullName,
						OrgInfo = new OrganizationInfo(nameData.OrgTemplateId, nameData.OrgGrade, true, -1),
						MonasticTitle = nameData.MonasticTitle,
						CustomDisplayNameId = nameData.CustomDisplayNameId,
						NickNameId = nameData.NickNameId,
						ExtraNameTextTemplateId = nameData.ExtraNameTextTemplateId
					};
					charScore += this._data.CharacterScore[charId];
					relationScore += score;
					bool flag = score > highestRelationScore;
					if (flag)
					{
						highestRelationScore = score;
						data.Relation = relation;
					}
				}, null, null, delegate(int _, ItemKey itemKey)
				{
					itemScore += (itemKey.IsValid() ? GlobalConfig.Instance.SecretItemFactor[(int)ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId)] : 0);
				}, null, null, null);
			}
			data.LevelScore = relationScore * GlobalConfig.Instance.SecretRelationScore[this._secretInformationLevelFactors[2] + 1] + charScore * GlobalConfig.Instance.SecretCharScore[this._secretInformationLevelFactors[1] + 1] + itemScore * GlobalConfig.Instance.SecretItemScore[this._secretInformationLevelFactors[3] + 1] + (int)valueScore * GlobalConfig.Instance.SecretSortValueScore[this._secretInformationLevelFactors[0] + 1];
			return data;
		}

		// Token: 0x06006AC6 RID: 27334 RVA: 0x00314C1C File Offset: 0x00312E1C
		private void OnTypeToggleChange(int _, int __)
		{
			bool flag = this.typeToggleGroup.GetActiveIndex() == 0;
			if (flag)
			{
				this.OnInformationChange();
			}
			else
			{
				this.OnSecretChange();
			}
		}

		// Token: 0x06006AC7 RID: 27335 RVA: 0x00314C4C File Offset: 0x00312E4C
		private void OnInformationChange()
		{
			this._filteredInformation.Clear();
			Func<InformationSortAndFilterData, bool> filter = this._informationSortAndFilterController.GenerateFilter();
			Comparison<InformationSortAndFilterData> comparer = this._informationSortAndFilterController.GenerateComparer(this._filteredInformation);
			List<InformationSortAndFilterData> allData = new List<InformationSortAndFilterData>();
			foreach (NormalInformation info in this._data.Information)
			{
				InformationSortAndFilterData item = new InformationSortAndFilterData
				{
					TemplateId = info.TemplateId,
					Level = info.Level
				};
				allData.Add(item);
				bool flag = filter(item);
				if (flag)
				{
					this._filteredInformation.Add(item);
				}
			}
			bool flag2 = comparer != null;
			if (flag2)
			{
				this._filteredInformation.Sort(comparer);
			}
			this._informationSortAndFilterController.AfterFilter(allData);
			this.informationScroll.SetDataCount(this._filteredInformation.Count);
			bool flag3 = this._data.Information.Count == 0;
			if (flag3)
			{
				this.noContent.SetActive(true);
				this.informationContent.SetActive(false);
			}
			else
			{
				this.noContent.SetActive(false);
				this.informationContent.SetActive(true);
			}
			this.secretContent.SetActive(false);
		}

		// Token: 0x06006AC8 RID: 27336 RVA: 0x00314DB4 File Offset: 0x00312FB4
		private void OnInformationRender(int index, GameObject obj)
		{
			obj.GetComponent<InformationCardItem>().Set(this._filteredInformation[index], false);
		}

		// Token: 0x06006AC9 RID: 27337 RVA: 0x00314DD0 File Offset: 0x00312FD0
		private void OnSecretChange()
		{
			bool flag = this.typeToggleGroup.GetActiveIndex() == 0;
			if (!flag)
			{
				this._filteredSecret.Clear();
				Func<SecretSortAndFilterData, bool> filter = this._secretSortAndFilterController.GenerateFilter();
				Comparison<SecretSortAndFilterData> comparer = this._secretSortAndFilterController.GenerateComparer(this._filteredSecret);
				Dictionary<int, SecretSortAndFilterData> rawData = this._secretData[this.typeToggleGroup.GetActiveIndex() - 1];
				foreach (SecretSortAndFilterData data in rawData.Values)
				{
					bool flag2 = filter(data);
					if (flag2)
					{
						this._filteredSecret.Add(data);
					}
				}
				bool flag3 = comparer != null;
				if (flag3)
				{
					this._filteredSecret.Sort(comparer);
				}
				this._secretSortAndFilterController.AfterFilter(this._secretData[this.typeToggleGroup.GetActiveIndex() - 1].Values);
				this.secretScroll.SetDataCount(this._filteredSecret.Count);
				bool flag4 = rawData.Count == 0;
				if (flag4)
				{
					this.noContent.SetActive(true);
					this.secretContent.SetActive(false);
				}
				else
				{
					this.noContent.SetActive(false);
					this.secretContent.SetActive(true);
				}
				this.informationContent.SetActive(false);
			}
		}

		// Token: 0x06006ACA RID: 27338 RVA: 0x00314F44 File Offset: 0x00313144
		private void OnSecretRender(int index, GameObject obj)
		{
			obj.GetComponent<SecretInformationCardItem>().Set(this._filteredSecret[index]);
			string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			GameObject item = obj.transform.GetChild(obj.transform.childCount - 1).gameObject;
			switch (this.typeToggleGroup.GetActiveIndex())
			{
			case 1:
			case 2:
				item.SetActive(false);
				break;
			case 3:
				item.GetComponent<CImage>().SetSprite(string.Format("ui9_back_monthnotify_grey_{0}", language), false, null);
				item.SetActive(true);
				break;
			case 4:
				item.GetComponent<CImage>().SetSprite(string.Format("ui9_back_monthnotify_red_{0}", language), false, null);
				item.SetActive(true);
				break;
			}
		}

		// Token: 0x04004D2C RID: 19756
		[SerializeField]
		protected CToggleGroup typeToggleGroup;

		// Token: 0x04004D2D RID: 19757
		[SerializeField]
		protected TextMeshProUGUI[] typeToggleNameLabels;

		// Token: 0x04004D2E RID: 19758
		[SerializeField]
		protected GameObject noContent;

		// Token: 0x04004D2F RID: 19759
		[SerializeField]
		protected GameObject informationContent;

		// Token: 0x04004D30 RID: 19760
		[SerializeField]
		protected GameObject secretContent;

		// Token: 0x04004D31 RID: 19761
		[SerializeField]
		protected SortAndFilter informationSortAndFilter;

		// Token: 0x04004D32 RID: 19762
		[SerializeField]
		protected SortAndFilter secretSortAndFilter;

		// Token: 0x04004D33 RID: 19763
		[SerializeField]
		protected InfinityScroll informationScroll;

		// Token: 0x04004D34 RID: 19764
		[SerializeField]
		protected InfinityScroll secretScroll;

		// Token: 0x04004D35 RID: 19765
		[SerializeField]
		protected CButton btnClose;

		// Token: 0x04004D36 RID: 19766
		private readonly List<LanguageKey> _typeToggleNames = new List<LanguageKey>
		{
			LanguageKey.UI_MonthNotify_Information_Get_Information,
			LanguageKey.UI_MonthNotify_Information_Get_Secret,
			LanguageKey.UI_MonthNotify_Information_Public,
			LanguageKey.UI_MonthNotify_Information_Expired,
			LanguageKey.UI_MonthNotify_Information_Expiring
		};

		// Token: 0x04004D37 RID: 19767
		private InformationSortAndFilterController _informationSortAndFilterController;

		// Token: 0x04004D38 RID: 19768
		private SecretSortAndFilterController _secretSortAndFilterController;

		// Token: 0x04004D39 RID: 19769
		private List<InformationSortAndFilterData> _filteredInformation = new List<InformationSortAndFilterData>();

		// Token: 0x04004D3A RID: 19770
		private List<SecretSortAndFilterData> _filteredSecret = new List<SecretSortAndFilterData>();

		// Token: 0x04004D3B RID: 19771
		private List<Dictionary<int, SecretSortAndFilterData>> _secretData = new List<Dictionary<int, SecretSortAndFilterData>>
		{
			new Dictionary<int, SecretSortAndFilterData>(),
			new Dictionary<int, SecretSortAndFilterData>(),
			new Dictionary<int, SecretSortAndFilterData>(),
			new Dictionary<int, SecretSortAndFilterData>(),
			new Dictionary<int, SecretSortAndFilterData>()
		};

		// Token: 0x04004D3C RID: 19772
		private int[] _secretInformationLevelFactors = new int[4];

		// Token: 0x04004D3D RID: 19773
		private MonthNotify _data;

		// Token: 0x04004D3E RID: 19774
		private Dictionary<int, FullBlockName> _secretInformationLocation;
	}
}
