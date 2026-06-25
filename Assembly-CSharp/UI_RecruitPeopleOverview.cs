using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BB RID: 443
public class UI_RecruitPeopleOverview : UIBase
{
	// Token: 0x06001B72 RID: 7026 RVA: 0x000BBB80 File Offset: 0x000B9D80
	public override void OnInit(ArgumentBox argsBox)
	{
		this.BlockTemplate.SetActive(false);
		bool flag = argsBox == null || !argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out this._specifiedBuildingBlockKey);
		if (flag)
		{
			this._specifiedBuildingBlockKey = BuildingBlockKey.Invalid;
		}
		this._buildingBlockKeys.Clear();
		this._selections.Clear();
		this._toggleGroups.Clear();
		this._recruitCount.Clear();
		this._isInConfirm = false;
		foreach (GameObject obj in this._instantiatedBlockObjects.Values.ToArray<GameObject>())
		{
			Object.Destroy(obj);
		}
		this._instantiatedBlockObjects.Clear();
		this._instantiatedBlockTemplateObjectsRecord.Clear();
		this.LoadingAnimation.SetActive(true);
		this.BlockContainer.localScale = Vector3.zero;
		this.Confirm.interactable = false;
		base.StopAllCoroutines();
		this.AcceptAll.ClearAndAddListener(delegate
		{
			foreach (CToggleGroupObsolete toggleGroup in this._toggleGroups)
			{
				toggleGroup.Set(0, true, false);
			}
		});
		this.RejectAll.ClearAndAddListener(delegate
		{
			foreach (CToggleGroupObsolete toggleGroup in this._toggleGroups)
			{
				toggleGroup.Set(1, true, false);
			}
		});
		this.Confirm.ClearAndAddListener(delegate
		{
			this._isInConfirm = true;
			int count = 0;
			int countMax = 0;
			List<int> allCharIds = new List<int>();
			AsyncMethodCallbackDelegate <>9__5;
			foreach (KeyValuePair<BuildingBlockKey, List<bool>> keyValuePair in this._selections)
			{
				BuildingBlockKey buildingBlockKey;
				List<bool> list;
				keyValuePair.Deconstruct(out buildingBlockKey, out list);
				BuildingBlockKey key = buildingBlockKey;
				List<bool> value = list;
				for (int j = value.Count - 1; j >= 0; j--)
				{
					bool flag2 = value[j];
					if (flag2)
					{
						countMax++;
						value.RemoveAt(j);
						BuildingBlockKey key2 = key;
						int earningDataIndex = j;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__5) == null)
						{
							callback = (<>9__5 = delegate(int offset, RawDataPool pool)
							{
								int charId = -1;
								Serializer.Deserialize(pool, offset, ref charId);
								count++;
								allCharIds.Add(charId);
								bool flag3 = count >= countMax;
								if (flag3)
								{
									base.<OnInit>g__OnFinished|3();
								}
							});
						}
						BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeople(this, key2, earningDataIndex, callback);
					}
				}
				BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeopleQuick(key);
			}
			BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeople(this, BuildingBlockKey.Invalid, 0, delegate(int _, RawDataPool _)
			{
				bool flag3 = count >= countMax;
				if (flag3)
				{
					this.QuickHide();
				}
			});
		});
		this.Cancel.ClearAndAddListener(new Action(this.QuickHide));
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000BBCD1 File Offset: 0x000B9ED1
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(9, 17, ulong.MaxValue, null));
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000BBCEC File Offset: 0x000B9EEC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		bool isInConfirm = this._isInConfirm;
		if (!isInConfirm)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					bool flag = 9 == notification.Uid.DomainId;
					if (flag)
					{
						bool flag2 = 17 == notification.Uid.DataId;
						if (flag2)
						{
							base.StartCoroutine(this.RefreshCoroutine(wrapper.DataPool, notification.ValueOffset));
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x000BBDBC File Offset: 0x000B9FBC
	private IEnumerator RefreshCoroutine(RawDataPool dataPool, int offset)
	{
		UI_RecruitPeopleOverview.<>c__DisplayClass19_0 CS$<>8__locals1 = new UI_RecruitPeopleOverview.<>c__DisplayClass19_0();
		CS$<>8__locals1.<>4__this = this;
		this.LoadingAnimation.SetActive(true);
		this.BlockContainer.localScale = Vector3.zero;
		this.Confirm.interactable = false;
		Serializer.DeserializeModifications<BuildingBlockKey>(dataPool, offset, this._earningsData);
		int max = 0;
		CS$<>8__locals1.current = 0;
		bool isInvalid = this._specifiedBuildingBlockKey.IsInvalid;
		if (isInvalid)
		{
			UI_RecruitPeopleOverview.<>c__DisplayClass19_1 CS$<>8__locals2 = new UI_RecruitPeopleOverview.<>c__DisplayClass19_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals2.dict = new Dictionary<BuildingBlockKey, BuildingBlockData>();
			using (Dictionary<BuildingBlockKey, BuildingEarningsData>.KeyCollection.Enumerator enumerator = this._earningsData.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UI_RecruitPeopleOverview.<>c__DisplayClass19_2 CS$<>8__locals3 = new UI_RecruitPeopleOverview.<>c__DisplayClass19_2();
					CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
					CS$<>8__locals3.key = enumerator.Current;
					BuildingDomainMethod.AsyncCall.GetBuildingBlockData(this, CS$<>8__locals3.key, delegate(int offset2, RawDataPool pool2)
					{
						BuildingBlockData blockData2 = new BuildingBlockData();
						Serializer.Deserialize(pool2, offset2, ref blockData2);
						CS$<>8__locals3.CS$<>8__locals2.dict.Add(CS$<>8__locals3.key, blockData2);
					});
					CS$<>8__locals3 = null;
				}
			}
			Dictionary<BuildingBlockKey, BuildingEarningsData>.KeyCollection.Enumerator enumerator = default(Dictionary<BuildingBlockKey, BuildingEarningsData>.KeyCollection.Enumerator);
			yield return new WaitUntil(() => CS$<>8__locals2.dict.Count == CS$<>8__locals2.CS$<>8__locals1.<>4__this._earningsData.Count);
			foreach (BuildingBlockKey key in CS$<>8__locals2.dict.Keys)
			{
				max++;
				BuildingBlockKey key2 = key;
				BuildingBlockData blockData = CS$<>8__locals2.dict[key];
				BuildingEarningsData earningsData = this._earningsData[key];
				Action onDone;
				if ((onDone = CS$<>8__locals2.CS$<>8__locals1.<>9__2) == null)
				{
					onDone = (CS$<>8__locals2.CS$<>8__locals1.<>9__2 = delegate()
					{
						CS$<>8__locals2.CS$<>8__locals1.current = CS$<>8__locals2.CS$<>8__locals1.current + 1;
					});
				}
				IEnumerator sub = this.RefreshSingleBlock(key2, blockData, earningsData, onDone);
				while (sub.MoveNext())
				{
					object obj = sub.Current;
					yield return obj;
				}
				sub = null;
				key = default(BuildingBlockKey);
			}
			Dictionary<BuildingBlockKey, BuildingBlockData>.KeyCollection.Enumerator enumerator2 = default(Dictionary<BuildingBlockKey, BuildingBlockData>.KeyCollection.Enumerator);
			CS$<>8__locals2 = null;
		}
		else
		{
			bool flag = this._earningsData.ContainsKey(this._specifiedBuildingBlockKey);
			if (flag)
			{
				UI_RecruitPeopleOverview.<>c__DisplayClass19_3 CS$<>8__locals4 = new UI_RecruitPeopleOverview.<>c__DisplayClass19_3();
				CS$<>8__locals4.blockData = null;
				BuildingDomainMethod.AsyncCall.GetBuildingBlockData(this, this._specifiedBuildingBlockKey, delegate(int offset2, RawDataPool pool2)
				{
					CS$<>8__locals4.blockData = new BuildingBlockData();
					Serializer.Deserialize(pool2, offset2, ref CS$<>8__locals4.blockData);
				});
				yield return new WaitUntil(() => CS$<>8__locals4.blockData != null);
				max++;
				IEnumerator sub2 = this.RefreshSingleBlock(this._specifiedBuildingBlockKey, CS$<>8__locals4.blockData, this._earningsData[this._specifiedBuildingBlockKey], delegate
				{
					CS$<>8__locals1.current++;
				});
				while (sub2.MoveNext())
				{
					object obj2 = sub2.Current;
					yield return obj2;
				}
				CS$<>8__locals4 = null;
				sub2 = null;
			}
		}
		while (CS$<>8__locals1.current < max)
		{
			yield return new WaitForEndOfFrame();
		}
		this.LoadingAnimation.SetActive(false);
		this.BlockContainer.localScale = Vector3.one;
		this.Confirm.interactable = true;
		yield break;
		yield break;
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x000BBDD9 File Offset: 0x000B9FD9
	private IEnumerator RefreshSingleBlock(BuildingBlockKey key, BuildingBlockData blockData, BuildingEarningsData earningsData, Action onDone = null)
	{
		UI_RecruitPeopleOverview.<>c__DisplayClass20_0 CS$<>8__locals1 = new UI_RecruitPeopleOverview.<>c__DisplayClass20_0();
		CS$<>8__locals1.earningsData = earningsData;
		CS$<>8__locals1.onDone = onDone;
		bool isRecruit = true;
		BuildingBlockItem config = BuildingBlock.Instance.GetItem(blockData.TemplateId);
		bool flag = key.IsInvalid || config == null;
		if (flag)
		{
			Action onDone2 = CS$<>8__locals1.onDone;
			if (onDone2 != null)
			{
				onDone2();
			}
			yield break;
		}
		ShopEventItem shopEventData = null;
		bool flag2 = config.SuccesEvent.Count > 0;
		if (flag2)
		{
			shopEventData = ShopEvent.Instance[config.SuccesEvent[0]];
		}
		else
		{
			isRecruit = false;
		}
		bool flag3 = shopEventData == null || shopEventData.RecruitPeopleProb.Count <= 0;
		if (flag3)
		{
			isRecruit = false;
		}
		shopEventData = null;
		CS$<>8__locals1.selections = null;
		bool flag4 = isRecruit && CS$<>8__locals1.earningsData.RecruitLevelList.Count > 0;
		if (flag4)
		{
			bool flag5 = this._instantiatedBlockTemplateObjectsRecord.TryGetValue(config.TemplateId, out CS$<>8__locals1.instance);
			if (!flag5)
			{
				bool flag6 = !this._instantiatedBlockObjects.TryGetValue(key, out CS$<>8__locals1.instance) || CS$<>8__locals1.instance == null;
				if (flag6)
				{
					CS$<>8__locals1.instance = Object.Instantiate<GameObject>(this.BlockTemplate, this.BlockContainer);
					this._instantiatedBlockObjects.Add(key, CS$<>8__locals1.instance);
					this._instantiatedBlockTemplateObjectsRecord.Add(config.TemplateId, CS$<>8__locals1.instance);
					CS$<>8__locals1.instance.SetActive(false);
				}
			}
			CS$<>8__locals1.selections = new List<bool>();
			int num;
			for (int i = 0; i < CS$<>8__locals1.earningsData.RecruitLevelList.Count; i = num + 1)
			{
				CS$<>8__locals1.selections.Add(false);
				num = i;
			}
			this._buildingBlockKeys.Add(key);
			this._recruitCount[key] = CS$<>8__locals1.earningsData.RecruitLevelList.Count;
			this._selections[key] = CS$<>8__locals1.selections;
			CS$<>8__locals1.instanceTransform = CS$<>8__locals1.instance.GetComponent<RectTransform>();
			Refers refers = CS$<>8__locals1.instance.GetComponent<Refers>();
			GameObject template = refers.CGet<GameObject>("RecruitPeopleButton");
			CS$<>8__locals1.container = refers.CGet<GameObject>("RecruitPeopleButtonHolder").GetComponent<RectTransform>();
			CS$<>8__locals1.layout = CS$<>8__locals1.container.GetComponent<GridLayoutGroup>();
			CS$<>8__locals1.spriteSize = template.GetComponent<CImage>().sprite.rect.size;
			refers.CGet<TextMeshProUGUI>("Title").text = config.Name;
			template.SetActive(false);
			CS$<>8__locals1.layout.cellSize = CS$<>8__locals1.spriteSize;
			CS$<>8__locals1.instanceTransform.localScale = Vector3.zero;
			for (int j = 0; j < CS$<>8__locals1.earningsData.RecruitLevelList.Count; j = num + 1)
			{
				UI_RecruitPeopleOverview.<>c__DisplayClass20_1 CS$<>8__locals2 = new UI_RecruitPeopleOverview.<>c__DisplayClass20_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.index = j;
				CS$<>8__locals2.person = Object.Instantiate<GameObject>(template, CS$<>8__locals2.CS$<>8__locals1.container);
				Refers currentItemRefers = CS$<>8__locals2.person.GetComponent<Refers>();
				CToggleGroupObsolete toggleGroup = currentItemRefers.CGet<CToggleGroupObsolete>("Panel");
				this._toggleGroups.Add(toggleGroup);
				CS$<>8__locals2.person.GetComponent<RectTransform>().sizeDelta = CS$<>8__locals2.CS$<>8__locals1.spriteSize;
				CS$<>8__locals2.person.SetActive(false);
				currentItemRefers.CGet<CImage>("Normal").gameObject.SetActive(true);
				currentItemRefers.CGet<GameObject>("Stay").SetActive(true);
				toggleGroup.OnActiveToggleChange = delegate(CToggleObsolete toggleNew, CToggleObsolete toggleOld)
				{
					CS$<>8__locals2.CS$<>8__locals1.selections[CS$<>8__locals2.index] = (toggleNew.Key == 0);
				};
				toggleGroup.InitPreOnToggle(-1);
				CS$<>8__locals2.tipsDp = currentItemRefers.GetComponentInChildren<TooltipInvoker>();
				CS$<>8__locals2.tipsDp.enabled = false;
				CS$<>8__locals2.peopleRefers = currentItemRefers.CGet<CImage>("HasPeopleImg").GetComponent<Refers>();
				GameObject buttonsRefers = currentItemRefers.CGet<GameObject>("BtnHolder");
				CS$<>8__locals2.remainTime = GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(blockData.TemplateId) - CS$<>8__locals2.CS$<>8__locals1.earningsData.RecruitLevelList[j].Second;
				currentItemRefers.CGet<TextMeshProUGUI>("StayText").text = CS$<>8__locals2.remainTime.ToString();
				yield return new WaitForEndOfFrame();
				ExtraDomainMethod.AsyncCall.RequestRecruitCharacterData(this, key, j, delegate(int offset2, RawDataPool pool2)
				{
					CS$<>8__locals2.CS$<>8__locals1.layout.cellSize = CS$<>8__locals2.CS$<>8__locals1.spriteSize;
					LayoutRebuilder.ForceRebuildLayoutImmediate(CS$<>8__locals2.CS$<>8__locals1.container);
					CS$<>8__locals2.person.GetComponent<RectTransform>().sizeDelta = CS$<>8__locals2.CS$<>8__locals1.spriteSize;
					CS$<>8__locals2.person.SetActive(true);
					RecruitCharacterData recruitCharacterData = null;
					Serializer.Deserialize(pool2, offset2, ref recruitCharacterData);
					bool flag8 = recruitCharacterData != null;
					if (flag8)
					{
						CS$<>8__locals2.tipsDp.enabled = true;
						CS$<>8__locals2.tipsDp.RuntimeParam = new ArgumentBox();
						CS$<>8__locals2.tipsDp.RuntimeParam.Set("RemainTime", CS$<>8__locals2.remainTime);
						CS$<>8__locals2.tipsDp.RuntimeParam.SetObject("Data", new CharacterDisplayDataForTooltip(recruitCharacterData));
						ValueTuple<string, string> name = recruitCharacterData.FullName.GetName(recruitCharacterData.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
						string surname = name.Item1;
						string givenName = name.Item2;
						CS$<>8__locals2.peopleRefers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(recruitCharacterData.GenerateAvatarRelatedData());
						CS$<>8__locals2.peopleRefers.CGet<TextMeshProUGUI>("Name").text = surname + givenName;
					}
					bool flag9 = CS$<>8__locals2.index == CS$<>8__locals2.CS$<>8__locals1.earningsData.RecruitLevelList.Count - 1;
					if (flag9)
					{
						CS$<>8__locals2.CS$<>8__locals1.layout.cellSize = CS$<>8__locals2.CS$<>8__locals1.spriteSize;
						CS$<>8__locals2.CS$<>8__locals1.instanceTransform.localScale = Vector3.one;
						UI_RecruitPeopleOverview.RefitContentSize(CS$<>8__locals2.CS$<>8__locals1.instance.transform.parent.GetComponent<RectTransform>(), CS$<>8__locals2.CS$<>8__locals1.container);
						YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
						uint frame = 1U;
						Action job;
						if ((job = CS$<>8__locals2.CS$<>8__locals1.<>9__2) == null)
						{
							job = (CS$<>8__locals2.CS$<>8__locals1.<>9__2 = delegate()
							{
								CS$<>8__locals2.CS$<>8__locals1.layout.cellSize = CS$<>8__locals2.CS$<>8__locals1.spriteSize;
								UI_RecruitPeopleOverview.RefitContentSize(CS$<>8__locals2.CS$<>8__locals1.instance.transform.parent.GetComponent<RectTransform>(), CS$<>8__locals2.CS$<>8__locals1.container);
							});
						}
						instance.DelayFrameDo(frame, job);
						Action onDone4 = CS$<>8__locals2.CS$<>8__locals1.onDone;
						if (onDone4 != null)
						{
							onDone4();
						}
					}
				});
				currentItemRefers.CGet<CImage>("HasPeopleImg").gameObject.SetActive(true);
				buttonsRefers.SetActive(false);
				LayoutRebuilder.ForceRebuildLayoutImmediate(CS$<>8__locals2.CS$<>8__locals1.container);
				LayoutRebuilder.MarkLayoutForRebuild(CS$<>8__locals2.CS$<>8__locals1.container);
				CS$<>8__locals2 = null;
				currentItemRefers = null;
				toggleGroup = null;
				buttonsRefers = null;
				num = j;
			}
			CS$<>8__locals1.instance.SetActive(true);
			yield return new WaitForEndOfFrame();
			UI_RecruitPeopleOverview.RefitContentSize(CS$<>8__locals1.instance.transform.parent.GetComponent<RectTransform>(), CS$<>8__locals1.instance.GetComponent<RectTransform>());
			yield break;
		}
		bool flag7 = this._instantiatedBlockObjects.TryGetValue(key, out CS$<>8__locals1.instance);
		if (flag7)
		{
			Object.Destroy(CS$<>8__locals1.instance);
			this._instantiatedBlockObjects.Remove(key);
		}
		this._buildingBlockKeys.Remove(key);
		this._recruitCount.Remove(key);
		this._selections.Remove(key);
		Action onDone3 = CS$<>8__locals1.onDone;
		if (onDone3 != null)
		{
			onDone3();
		}
		yield break;
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x000BBE08 File Offset: 0x000BA008
	private void ShowGetPeopleView(List<int> charIdList)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("CharIdList", charIdList);
		argBox.Set("ObtainType", 15);
		UIElement.GetItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.GetItem);
		GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000BBE64 File Offset: 0x000BA064
	public static void RefitContentSize(RectTransform root, RectTransform leaf)
	{
		RectTransform current = leaf;
		while (null != current)
		{
			ContentSizeFitter fitter = current.GetComponent<ContentSizeFitter>();
			bool flag = null != fitter;
			if (flag)
			{
				fitter.SetLayoutHorizontal();
				fitter.SetLayoutVertical();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(current);
			LayoutRebuilder.MarkLayoutForRebuild(current);
			bool flag2 = current == root;
			if (flag2)
			{
				break;
			}
			current = (current.parent as RectTransform);
		}
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000BBED0 File Offset: 0x000BA0D0
	public static void EntryFromBuildingArea(BuildingBlockKey blockKey)
	{
		ArgumentBox argsBox = new ArgumentBox();
		argsBox.Set<BuildingBlockKey>("BuildingBlockKey", blockKey);
		UIElement.RecruitPeopleOverview.SetOnInitArgs(argsBox);
		UIManager.Instance.MaskUI(UIElement.RecruitPeopleOverview);
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000BBF0D File Offset: 0x000BA10D
	public static void EntryFromBuildingArea()
	{
		UIElement.RecruitPeopleOverview.SetOnInitArgs(new ArgumentBox());
		UIManager.Instance.MaskUI(UIElement.RecruitPeopleOverview);
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x000BBF30 File Offset: 0x000BA130
	private void OnDisable()
	{
		GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
	}

	// Token: 0x04001569 RID: 5481
	public GameObject BlockTemplate;

	// Token: 0x0400156A RID: 5482
	public RectTransform BlockContainer;

	// Token: 0x0400156B RID: 5483
	public GameObject LoadingAnimation;

	// Token: 0x0400156C RID: 5484
	public CButtonObsolete AcceptAll;

	// Token: 0x0400156D RID: 5485
	public CButtonObsolete RejectAll;

	// Token: 0x0400156E RID: 5486
	public CButtonObsolete Confirm;

	// Token: 0x0400156F RID: 5487
	public CButtonObsolete Cancel;

	// Token: 0x04001570 RID: 5488
	private BuildingBlockKey _specifiedBuildingBlockKey;

	// Token: 0x04001571 RID: 5489
	private readonly HashSet<BuildingBlockKey> _buildingBlockKeys = new HashSet<BuildingBlockKey>();

	// Token: 0x04001572 RID: 5490
	private readonly Dictionary<BuildingBlockKey, List<bool>> _selections = new Dictionary<BuildingBlockKey, List<bool>>();

	// Token: 0x04001573 RID: 5491
	private readonly HashSet<CToggleGroupObsolete> _toggleGroups = new HashSet<CToggleGroupObsolete>();

	// Token: 0x04001574 RID: 5492
	private readonly Dictionary<BuildingBlockKey, int> _recruitCount = new Dictionary<BuildingBlockKey, int>();

	// Token: 0x04001575 RID: 5493
	private bool _isInConfirm;

	// Token: 0x04001576 RID: 5494
	private readonly Dictionary<BuildingBlockKey, BuildingEarningsData> _earningsData = new Dictionary<BuildingBlockKey, BuildingEarningsData>();

	// Token: 0x04001577 RID: 5495
	private readonly Dictionary<BuildingBlockKey, GameObject> _instantiatedBlockObjects = new Dictionary<BuildingBlockKey, GameObject>();

	// Token: 0x04001578 RID: 5496
	private readonly Dictionary<short, GameObject> _instantiatedBlockTemplateObjectsRecord = new Dictionary<short, GameObject>();
}
