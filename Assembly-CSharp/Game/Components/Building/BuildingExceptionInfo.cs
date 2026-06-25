using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Building;
using GameData.Domains.Building;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

namespace Game.Components.Building
{
	// Token: 0x02000F64 RID: 3940
	public class BuildingExceptionInfo : MonoBehaviour
	{
		// Token: 0x17001475 RID: 5237
		// (get) Token: 0x0600B450 RID: 46160 RVA: 0x0052086C File Offset: 0x0051EA6C
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x0600B451 RID: 46161 RVA: 0x00520873 File Offset: 0x0051EA73
		public void RefreshInfo(BuildingExceptionData exceptionData, MapBlockData mapBlockData)
		{
			this._mapBlockData = mapBlockData;
			this.ResetData();
			this.CollectData(exceptionData);
			this.RefreshBtn();
			this.RefreshCurrentInfo();
		}

		// Token: 0x0600B452 RID: 46162 RVA: 0x0052089A File Offset: 0x0051EA9A
		public void SetBuildingArea(ViewBuildingArea buildingArea)
		{
			this._buildingArea = buildingArea;
		}

		// Token: 0x0600B453 RID: 46163 RVA: 0x005208A4 File Offset: 0x0051EAA4
		private void Awake()
		{
			this.infinityScroll.gameObject.SetActive(false);
			this.infinityScroll.OnItemRender += this.OnItemRender;
			this.collapseBtn.onClick.AddListener(delegate()
			{
				this.infinityScroll.gameObject.SetActive(false);
			});
			CButton lastBtn = this.collapseBtn;
			this.manageBtn.onClick.ResetListener(delegate()
			{
				this.infinityScroll.gameObject.SetActive(lastBtn.GetInstanceID() != this.manageBtn.GetInstanceID() || !this.infinityScroll.gameObject.activeSelf);
				lastBtn = this.manageBtn;
				this._refreshExceptionClass = BuildingExceptionClass.ManageException;
				this.RefreshCurrentInfo();
			});
			this.learnBtn.onClick.ResetListener(delegate()
			{
				this.infinityScroll.gameObject.SetActive(lastBtn.GetInstanceID() != this.learnBtn.GetInstanceID() || !this.infinityScroll.gameObject.activeSelf);
				lastBtn = this.learnBtn;
				this._refreshExceptionClass = BuildingExceptionClass.LearnException;
				this.RefreshCurrentInfo();
			});
			this.buildBtn.onClick.ResetListener(delegate()
			{
				this.infinityScroll.gameObject.SetActive(lastBtn.GetInstanceID() != this.buildBtn.GetInstanceID() || !this.infinityScroll.gameObject.activeSelf);
				lastBtn = this.buildBtn;
				this._refreshExceptionClass = BuildingExceptionClass.BuildException;
				this.RefreshCurrentInfo();
			});
			this.effectBtn.onClick.ResetListener(delegate()
			{
				this.infinityScroll.gameObject.SetActive(lastBtn.GetInstanceID() != this.effectBtn.GetInstanceID() || !this.infinityScroll.gameObject.activeSelf);
				lastBtn = this.effectBtn;
				this._refreshExceptionClass = BuildingExceptionClass.EffectException;
				this.RefreshCurrentInfo();
			});
			this.damageBtn.onClick.ResetListener(delegate()
			{
				this.infinityScroll.gameObject.SetActive(lastBtn.GetInstanceID() != this.damageBtn.GetInstanceID() || !this.infinityScroll.gameObject.activeSelf);
				lastBtn = this.damageBtn;
				this._refreshExceptionClass = BuildingExceptionClass.DamagedException;
				this.RefreshCurrentInfo();
			});
		}

		// Token: 0x0600B454 RID: 46164 RVA: 0x005209A3 File Offset: 0x0051EBA3
		private void OnEnable()
		{
			this.infinityScroll.gameObject.SetActive(false);
		}

		// Token: 0x0600B455 RID: 46165 RVA: 0x005209B8 File Offset: 0x0051EBB8
		private void RefreshCurrentInfo()
		{
			this.title.SetText(this.GetBuildingExceptionClassTitle(this._refreshExceptionClass), true);
			List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> dataList = this.GetDataListByClass(this._refreshExceptionClass);
			this.totalCount.SetText(dataList.Count.ToString(), true);
			this.infinityScroll.SetDataCount(dataList.Count);
		}

		// Token: 0x0600B456 RID: 46166 RVA: 0x00520A1C File Offset: 0x0051EC1C
		private void OnItemRender(int index, GameObject go)
		{
			List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> dataList = this.GetDataListByClass(this._refreshExceptionClass);
			ValueTuple<BuildingBlockKey, BuildingExceptionType> data = dataList[index];
			CButton btn = go.GetComponent<CButton>();
			TextMeshProUGUI[] textArray = go.GetComponentsInChildren<TextMeshProUGUI>();
			BuildingBlockKey blockKey = data.Item1;
			BuildingExceptionType exceptionType = data.Item2;
			BuildingBlockData blockData = this.BuildingModel.GetTaiwuBuildingData(blockKey);
			BuildingBlockItem config = BuildingBlock.Instance[blockData.TemplateId];
			textArray[0].SetText(ViewBuildingArea.GetBuildingName(config, blockKey, this._mapBlockData.TemplateId, false), true);
			textArray[1].SetText(ViewBuildingArea.GetBuildingExceptionString(exceptionType), true);
			btn.onClick.ResetListener(delegate()
			{
				this._buildingArea.MoveCameraCenterToBuilding(blockKey.BuildingBlockIndex);
			});
		}

		// Token: 0x0600B457 RID: 46167 RVA: 0x00520AE4 File Offset: 0x0051ECE4
		private void RefreshBtn()
		{
			this.manageBtn.gameObject.SetActive(this._manageExceptionBlockList.Count > 0);
			this.learnBtn.gameObject.SetActive(this._learnExceptionBlockList.Count > 0);
			this.buildBtn.gameObject.SetActive(this._buildExceptionBlockList.Count > 0);
			this.effectBtn.gameObject.SetActive(this._effectExceptionBlockList.Count > 0);
			this.damageBtn.gameObject.SetActive(this._damageExceptionBlockList.Count > 0);
			this.manageCount.SetText(this._manageExceptionBlockList.Count.ToString(), true);
			this.learnCount.SetText(this._learnExceptionBlockList.Count.ToString(), true);
			this.buildCount.SetText(this._buildExceptionBlockList.Count.ToString(), true);
			this.effectCount.SetText(this._effectExceptionBlockList.Count.ToString(), true);
			this.damageCount.SetText(this._damageExceptionBlockList.Count.ToString(), true);
			bool flag = this._manageExceptionBlockList.Count <= 0 && this._learnExceptionBlockList.Count <= 0 && this._buildExceptionBlockList.Count <= 0 && this._effectExceptionBlockList.Count <= 0 && this._damageExceptionBlockList.Count <= 0;
			if (flag)
			{
				this.infinityScroll.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B458 RID: 46168 RVA: 0x00520C8F File Offset: 0x0051EE8F
		private void ResetData()
		{
			this._manageExceptionBlockList.Clear();
			this._learnExceptionBlockList.Clear();
			this._buildExceptionBlockList.Clear();
			this._effectExceptionBlockList.Clear();
			this._damageExceptionBlockList.Clear();
		}

		// Token: 0x0600B459 RID: 46169 RVA: 0x00520CD0 File Offset: 0x0051EED0
		private void CollectData(BuildingExceptionData exceptionData)
		{
			foreach (KeyValuePair<BuildingBlockKey, BuildingExceptionItem> keyValuePair in exceptionData.BuildingExceptionDict)
			{
				BuildingBlockKey buildingBlockKey;
				BuildingExceptionItem buildingExceptionItem;
				keyValuePair.Deconstruct(out buildingBlockKey, out buildingExceptionItem);
				BuildingBlockKey blockKey = buildingBlockKey;
				BuildingExceptionItem exception = buildingExceptionItem;
				foreach (sbyte exceptionType in exception.ExceptionTypeList)
				{
					switch (BuildingExceptionInfo.BuildingExceptionTypeToClass((BuildingExceptionType)exceptionType))
					{
					case BuildingExceptionClass.ManageException:
						this._manageExceptionBlockList.Add(new ValueTuple<BuildingBlockKey, BuildingExceptionType>(blockKey, (BuildingExceptionType)exceptionType));
						break;
					case BuildingExceptionClass.LearnException:
						this._learnExceptionBlockList.Add(new ValueTuple<BuildingBlockKey, BuildingExceptionType>(blockKey, (BuildingExceptionType)exceptionType));
						break;
					case BuildingExceptionClass.BuildException:
						this._buildExceptionBlockList.Add(new ValueTuple<BuildingBlockKey, BuildingExceptionType>(blockKey, (BuildingExceptionType)exceptionType));
						break;
					case BuildingExceptionClass.EffectException:
						this._effectExceptionBlockList.Add(new ValueTuple<BuildingBlockKey, BuildingExceptionType>(blockKey, (BuildingExceptionType)exceptionType));
						break;
					case BuildingExceptionClass.DamagedException:
						this._damageExceptionBlockList.Add(new ValueTuple<BuildingBlockKey, BuildingExceptionType>(blockKey, (BuildingExceptionType)exceptionType));
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		// Token: 0x0600B45A RID: 46170 RVA: 0x00520E3C File Offset: 0x0051F03C
		private static BuildingExceptionClass BuildingExceptionTypeToClass(BuildingExceptionType type)
		{
			if (!true)
			{
			}
			BuildingExceptionClass result;
			switch (type)
			{
			case BuildingExceptionType.ManageStoppedForDependency:
			case BuildingExceptionType.ManageStoppedForNoLeader:
			case BuildingExceptionType.ComfortableHouseEntertainNoFood:
				result = BuildingExceptionClass.ManageException;
				break;
			case BuildingExceptionType.LearnException:
				result = BuildingExceptionClass.LearnException;
				break;
			case BuildingExceptionType.BuildStoppedForWorkerShortage:
			case BuildingExceptionType.DemolishStoppedForWorkerShortage:
				result = BuildingExceptionClass.BuildException;
				break;
			case BuildingExceptionType.EffectStoppedForDependency:
				result = BuildingExceptionClass.EffectException;
				break;
			default:
				result = BuildingExceptionClass.DamagedException;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B45B RID: 46171 RVA: 0x00520E90 File Offset: 0x0051F090
		private List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> GetDataListByClass(BuildingExceptionClass exceptionClass)
		{
			if (!true)
			{
			}
			List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> result;
			switch (exceptionClass)
			{
			case BuildingExceptionClass.ManageException:
				result = this._manageExceptionBlockList;
				break;
			case BuildingExceptionClass.LearnException:
				result = this._learnExceptionBlockList;
				break;
			case BuildingExceptionClass.BuildException:
				result = this._buildExceptionBlockList;
				break;
			case BuildingExceptionClass.EffectException:
				result = this._effectExceptionBlockList;
				break;
			default:
				result = this._damageExceptionBlockList;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B45C RID: 46172 RVA: 0x00520EF0 File Offset: 0x0051F0F0
		private string GetBuildingExceptionClassTitle(BuildingExceptionClass exceptionClass)
		{
			if (!true)
			{
			}
			string result;
			switch (exceptionClass)
			{
			case BuildingExceptionClass.ManageException:
				result = LanguageKey.Lk_Building_Exception_Manage.Tr();
				break;
			case BuildingExceptionClass.LearnException:
				result = LanguageKey.Lk_Building_Exception_Learn.Tr();
				break;
			case BuildingExceptionClass.BuildException:
				result = LanguageKey.Lk_Building_Exception_Build.Tr();
				break;
			case BuildingExceptionClass.EffectException:
				result = LanguageKey.Lk_Building_Exception_Effect.Tr();
				break;
			default:
				result = LanguageKey.Lk_Building_Exception_Damage.Tr();
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04008C72 RID: 35954
		[SerializeField]
		private CButton manageBtn;

		// Token: 0x04008C73 RID: 35955
		[SerializeField]
		private CButton learnBtn;

		// Token: 0x04008C74 RID: 35956
		[SerializeField]
		private CButton buildBtn;

		// Token: 0x04008C75 RID: 35957
		[SerializeField]
		private CButton effectBtn;

		// Token: 0x04008C76 RID: 35958
		[SerializeField]
		private CButton damageBtn;

		// Token: 0x04008C77 RID: 35959
		[SerializeField]
		private CButton collapseBtn;

		// Token: 0x04008C78 RID: 35960
		[SerializeField]
		private TextMeshProUGUI manageCount;

		// Token: 0x04008C79 RID: 35961
		[SerializeField]
		private TextMeshProUGUI learnCount;

		// Token: 0x04008C7A RID: 35962
		[SerializeField]
		private TextMeshProUGUI buildCount;

		// Token: 0x04008C7B RID: 35963
		[SerializeField]
		private TextMeshProUGUI effectCount;

		// Token: 0x04008C7C RID: 35964
		[SerializeField]
		private TextMeshProUGUI damageCount;

		// Token: 0x04008C7D RID: 35965
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x04008C7E RID: 35966
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04008C7F RID: 35967
		[SerializeField]
		private TextMeshProUGUI totalCount;

		// Token: 0x04008C80 RID: 35968
		private ViewBuildingArea _buildingArea;

		// Token: 0x04008C81 RID: 35969
		[TupleElementNames(new string[]
		{
			"blockKey",
			"exceptionType"
		})]
		private List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> _manageExceptionBlockList = new List<ValueTuple<BuildingBlockKey, BuildingExceptionType>>();

		// Token: 0x04008C82 RID: 35970
		[TupleElementNames(new string[]
		{
			"blockKey",
			"exceptionType"
		})]
		private List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> _learnExceptionBlockList = new List<ValueTuple<BuildingBlockKey, BuildingExceptionType>>();

		// Token: 0x04008C83 RID: 35971
		[TupleElementNames(new string[]
		{
			"blockKey",
			"exceptionType"
		})]
		private List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> _buildExceptionBlockList = new List<ValueTuple<BuildingBlockKey, BuildingExceptionType>>();

		// Token: 0x04008C84 RID: 35972
		[TupleElementNames(new string[]
		{
			"blockKey",
			"exceptionType"
		})]
		private List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> _effectExceptionBlockList = new List<ValueTuple<BuildingBlockKey, BuildingExceptionType>>();

		// Token: 0x04008C85 RID: 35973
		[TupleElementNames(new string[]
		{
			"blockKey",
			"exceptionType"
		})]
		private List<ValueTuple<BuildingBlockKey, BuildingExceptionType>> _damageExceptionBlockList = new List<ValueTuple<BuildingBlockKey, BuildingExceptionType>>();

		// Token: 0x04008C86 RID: 35974
		private BuildingExceptionClass _refreshExceptionClass;

		// Token: 0x04008C87 RID: 35975
		private MapBlockData _mapBlockData;
	}
}
