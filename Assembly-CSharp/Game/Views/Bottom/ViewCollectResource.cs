using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UI;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C4E RID: 3150
	public class ViewCollectResource : UIBase
	{
		// Token: 0x0600A06E RID: 41070 RVA: 0x004AE424 File Offset: 0x004AC624
		public override void OnInit(ArgumentBox argsBox)
		{
			int type;
			argsBox.Get("CollectType", out type);
			argsBox.Get<List<CollectResourceResult>>("CollectInfo", out this._collectResults);
			argsBox.Get("CollectResourceIsMax", out this._collectResourceIsMax);
			argsBox.Get("IsDigSeries", out this._isDigSeries);
			this.CurrentCollectType = (ViewCollectResource.CollectResourceType)type;
			this.SetIsCollecting(true);
		}

		// Token: 0x0600A06F RID: 41071 RVA: 0x004AE488 File Offset: 0x004AC688
		private void Awake()
		{
			for (int index = 0; index < this.ShowingResIconCacheList.Length; index++)
			{
				this.ShowingResIconCacheList[index] = new List<GameObject>();
			}
			this._resourceIconPool = new PoolItem("CollectResource_ResourceIcon", this.resourceIcon);
			this._itemIconPool = new PoolItem("CollectResource_ItemIcon", this.itemIcon);
			this.singleObj.Init(this);
			for (int i = 0; i < this.multipleObj.transform.childCount; i++)
			{
				this.multipleObj.transform.GetChild(i).GetComponent<CollectResourceItem>().Init(this);
			}
		}

		// Token: 0x0600A070 RID: 41072 RVA: 0x004AE530 File Offset: 0x004AC730
		private void OnEnable()
		{
			this.bottomRoot.gameObject.SetActive(false);
			switch (this.CurrentCollectType)
			{
			case ViewCollectResource.CollectResourceType.Choosy:
				return;
			case ViewCollectResource.CollectResourceType.SavageSkill:
				this.StartMultipleCollect();
				goto IL_92;
			case ViewCollectResource.CollectResourceType.Treasure:
				this.StartDig();
				GEvent.Add(UiEvents.ResponseBottomTimeDisk, new GEvent.Callback(this.ResponseBottomTimeDisk));
				GEvent.OnEvent(UiEvents.RequestBottomTimeDisk, null);
				this.bottomRoot.gameObject.SetActive(true);
				goto IL_92;
			}
			this.StartSingleCollect();
			IL_92:
			this.skipAnimHotKeyDisplay.SetActive(true);
			this.mask.SetValueFactor(0.5f);
		}

		// Token: 0x0600A071 RID: 41073 RVA: 0x004AE5F0 File Offset: 0x004AC7F0
		private void OnDisable()
		{
			this.Clear();
			this.KillSeq(this._hintRedFadeOutSeq);
			this.hintBlue.SetActive(false);
			this.hintRed.SetActive(false);
			bool flag = (this.CurrentCollectType == ViewCollectResource.CollectResourceType.Normal || this.CurrentCollectType == ViewCollectResource.CollectResourceType.SavageSkill) && SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag)
			{
				TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialFinishCollectResource, false);
			}
			bool flag2 = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Treasure;
			if (flag2)
			{
				GEvent.Remove(UiEvents.ResponseBottomTimeDisk, new GEvent.Callback(this.ResponseBottomTimeDisk));
				this.TryReturnTimeDisk();
			}
			bool flag3 = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Treasure;
			if (flag3)
			{
				bool anyMaterial = this._digResult.AnyMaterial;
				if (anyMaterial)
				{
					TaiwuEventDomainMethod.Call.TriggerListener("FindTreasureMaterial", true);
				}
			}
			bool flag4 = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Treasure;
			if (flag4)
			{
				TreasureFindResult digResult = this._digResult;
				bool flag5;
				if (digResult.Success && !digResult.AnyMaterial)
				{
					List<ItemDisplayData> digItemDataList = this._digItemDataList;
					flag5 = (digItemDataList != null && digItemDataList.Count > 0);
				}
				else
				{
					flag5 = false;
				}
				bool flag6 = flag5;
				if (flag6)
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("ItemList", this._digItemDataList);
					UIElement.GetItem.SetOnInitArgs(argBox);
					bool anyExtraItem = this._digResult.AnyExtraItem;
					if (anyExtraItem)
					{
						UIElement getItem = UIElement.GetItem;
						getItem.OnHide = (Action)Delegate.Combine(getItem.OnHide, new Action(delegate()
						{
							ExtraDomainMethod.Call.InvokeFindExtraTreasureEvent(this._digResult);
						}));
					}
					UIManager.Instance.MaskUI(UIElement.GetItem);
				}
			}
			GEvent.OnEvent(UiEvents.UpdateMapBlockData, null);
		}

		// Token: 0x0600A072 RID: 41074 RVA: 0x004AE78A File Offset: 0x004AC98A
		private void OnDestroy()
		{
			PoolItem resourceIconPool = this._resourceIconPool;
			if (resourceIconPool != null)
			{
				resourceIconPool.Destroy();
			}
			this._resourceIconPool = null;
			PoolItem itemIconPool = this._itemIconPool;
			if (itemIconPool != null)
			{
				itemIconPool.Destroy();
			}
			this._itemIconPool = null;
		}

		// Token: 0x0600A073 RID: 41075 RVA: 0x004AE7C0 File Offset: 0x004AC9C0
		private void Update()
		{
			ViewCollectResource.CollectResourceType currentCollectType = this.CurrentCollectType;
			ViewCollectResource.CollectResourceType collectResourceType = currentCollectType;
			if (collectResourceType != ViewCollectResource.CollectResourceType.Choosy)
			{
				if (collectResourceType != ViewCollectResource.CollectResourceType.Treasure)
				{
					bool collecting = this._collecting;
					if (collecting)
					{
						bool flag = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
						if (flag)
						{
							this.SetIsCollecting(false);
							this.Stop();
						}
					}
					else
					{
						bool flag2 = HotKeyCommand.CheckAnyKeyDown();
						if (flag2)
						{
							base.StartCoroutine(this.ExitCollectAction());
						}
					}
				}
				else
				{
					this.UpdateTreasure();
				}
			}
		}

		// Token: 0x0600A074 RID: 41076 RVA: 0x004AE860 File Offset: 0x004ACA60
		private void ClearResourceIcon(sbyte resourceType)
		{
			List<GameObject> list = this.ShowingResIconCacheList[(int)resourceType];
			list.ForEach(new Action<GameObject>(this._resourceIconPool.DestroyObject));
			list.Clear();
		}

		// Token: 0x0600A075 RID: 41077 RVA: 0x004AE898 File Offset: 0x004ACA98
		private void Clear()
		{
			this._showingItemIconCacheList.ForEach(new Action<GameObject>(this._itemIconPool.DestroyObject));
			this._showingItemIconCacheList.Clear();
			sbyte index = 0;
			while ((int)index < this.ShowingResIconCacheList.Length)
			{
				this.ClearResourceIcon(index);
				index += 1;
			}
			this._showingPracticeIconCacheList.ForEach(new Action<GameObject>(this._resourceIconPool.DestroyObject));
			this._showingPracticeIconCacheList.Clear();
			this.singleObj.Destroy();
			for (int i = 0; i < this.multipleObj.transform.childCount; i++)
			{
				this.multipleObj.transform.GetChild(i).GetComponent<CollectResourceItem>().Destroy();
			}
			this.successObj.SetActive(false);
			this._collectResults = null;
			this.SetIsCollecting(false);
		}

		// Token: 0x0600A076 RID: 41078 RVA: 0x004AE97B File Offset: 0x004ACB7B
		public void SetIsCollecting(bool value)
		{
			this._collecting = value;
			this.skipAnimHotKeyDisplay.SetActive(value);
			this.anyKeyContinue.SetActive(!value);
			this.mask.SetValueFactor(1f);
		}

		// Token: 0x0600A077 RID: 41079 RVA: 0x004AE9B4 File Offset: 0x004ACBB4
		public GameObject GetDropIcon(bool isItem, sbyte type)
		{
			GameObject res;
			if (isItem)
			{
				res = this._itemIconPool.GetObject();
				this._showingItemIconCacheList.Add(res);
				bool flag = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Normal;
				if (flag)
				{
					this.PlayDigSuccessEffect();
				}
			}
			else
			{
				string resourceSprite = CommonUtils.GetResourceSpriteName(type, true);
				res = this._resourceIconPool.GetObject();
				res.GetComponent<CImage>().SetSprite(resourceSprite, false, null);
				bool flag2 = this.ShowingResIconCacheList.CheckIndex((int)type);
				if (flag2)
				{
					this.ShowingResIconCacheList[(int)type].Add(res);
				}
				else
				{
					this._showingPracticeIconCacheList.Add(res);
				}
			}
			return res;
		}

		// Token: 0x0600A078 RID: 41080 RVA: 0x004AEA58 File Offset: 0x004ACC58
		private void Stop()
		{
			bool flag = this.CurrentCollectType != ViewCollectResource.CollectResourceType.SavageSkill;
			if (flag)
			{
				this.singleObj.Stop();
			}
			else
			{
				for (int i = 0; i < this.multipleObj.transform.childCount; i++)
				{
					this.multipleObj.transform.GetChild(i).GetComponent<CollectResourceItem>().Stop();
				}
			}
		}

		// Token: 0x0600A079 RID: 41081 RVA: 0x004AEAC3 File Offset: 0x004ACCC3
		private IEnumerator ExitCollectAction()
		{
			bool flag = this.CurrentCollectType != ViewCollectResource.CollectResourceType.SavageSkill;
			if (flag)
			{
				this.singleObj.Hide();
			}
			else
			{
				int num;
				for (int i = 0; i < this.multipleObj.transform.childCount; i = num + 1)
				{
					this.multipleObj.transform.GetChild(i).GetComponent<CollectResourceItem>().Hide();
					num = i;
				}
				this.multipleObj.rectTransform.DOLocalMoveY(-400f, 0.3f, false).SetAutoKill(true);
				DOVirtual.Float(1f, 0f, 0.3f, delegate(float stepValue)
				{
					this.multipleObj.color = this.multipleObj.color.SetAlpha(stepValue);
				}).SetAutoKill(true).SetUpdate(true);
			}
			this.successObj.SetActive(false);
			yield return new WaitForSeconds(0.3f);
			this.ShowGetItemInfo();
			this.QuickHide();
			yield break;
		}

		// Token: 0x0600A07A RID: 41082 RVA: 0x004AEAD4 File Offset: 0x004ACCD4
		private void ShowGetItemInfo()
		{
			bool flag = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Practice;
			if (!flag)
			{
				List<ItemDisplayData> list = new List<ItemDisplayData>();
				foreach (CollectResourceResult result in this._collectResults)
				{
					bool flag2 = result.ItemDisplayData != null;
					if (flag2)
					{
						result.ItemDisplayData.Amount = 1;
						list.Add(result.ItemDisplayData);
					}
				}
				Action closeAction = this.GetCloseAction();
				bool flag3 = list.Count > 0;
				if (flag3)
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("ItemList", list);
					argBox.SetObject("CloseAction", closeAction);
					UIElement.GetItem.SetOnInitArgs(argBox);
					UIManager.Instance.MaskUI(UIElement.GetItem);
				}
				else if (closeAction != null)
				{
					closeAction();
				}
			}
		}

		// Token: 0x0600A07B RID: 41083 RVA: 0x004AEBD4 File Offset: 0x004ACDD4
		private unsafe Action GetCloseAction()
		{
			bool flag = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Choosy;
			Action result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = !SingletonObject.getInstance<TaiwuCharacterModel>().HasInheritedTaiwu;
				if (flag2)
				{
					result = null;
				}
				else
				{
					this._wudangResourceType = -1;
					MapBlockData mapBlockData = SingletonObject.getInstance<WorldMapModel>().PlayerAtBlock;
					int maxResourceCount = 0;
					bool flag3 = this.CurrentCollectType == ViewCollectResource.CollectResourceType.Normal;
					if (flag3)
					{
						this._wudangResourceType = this._collectResults[0].ResourceType;
						maxResourceCount = (int)(this._collectResourceIsMax ? (*mapBlockData.MaxResources[(int)this._wudangResourceType]) : 0);
					}
					else
					{
						for (int i = 0; i < 6; i++)
						{
							bool flag4 = *mapBlockData.CurrResources[i] == *mapBlockData.MaxResources[i];
							if (flag4)
							{
								maxResourceCount += (int)(*mapBlockData.MaxResources[i]);
							}
						}
					}
					result = (GameApp.Random.CheckPercentProb(maxResourceCount / 10) ? new Action(this.WudangAction) : null);
				}
			}
			return result;
		}

		// Token: 0x0600A07C RID: 41084 RVA: 0x004AECDE File Offset: 0x004ACEDE
		private void WudangAction()
		{
			TaiwuEventDomainMethod.Call.TaiwuCollectWudangHeavenlyTreeSeed(this._wudangResourceType);
		}

		// Token: 0x0600A07D RID: 41085 RVA: 0x004AECF0 File Offset: 0x004ACEF0
		private void StartSingleCollect()
		{
			this.hotKeyRoot.GetComponent<RectTransform>().anchoredPosition = this._singleCollectPos;
			this.singleObj.gameObject.SetActive(true);
			this.multipleObj.gameObject.SetActive(false);
			this.singleObj.Set(this._collectResults[0]);
			base.StartCoroutine(this.singleObj.DropItem());
		}

		// Token: 0x0600A07E RID: 41086 RVA: 0x004AED68 File Offset: 0x004ACF68
		private void StartMultipleCollect()
		{
			this.hotKeyRoot.GetComponent<RectTransform>().anchoredPosition = this._multipleCollectPos;
			this.singleObj.gameObject.SetActive(false);
			this.multipleObj.gameObject.SetActive(true);
			this.multipleObj.color = this.multipleObj.color.SetAlpha(0f);
			this.multipleObj.rectTransform.anchoredPosition = Vector2.down * 400f;
			this.multipleObj.rectTransform.DOAnchorPosY(0f, 0.5f, false).SetAutoKill(true);
			DOVirtual.Float(0f, 1f, 0.5f, delegate(float stepValue)
			{
				this.multipleObj.color = this.multipleObj.color.SetAlpha(stepValue);
			}).SetAutoKill(true);
			for (int i = 0; i < this.multipleObj.transform.childCount; i++)
			{
				bool hasData = this._collectResults.CheckIndex(i);
				Transform obj = this.multipleObj.transform.GetChild(i);
				CollectResourceItem item = obj.GetComponent<CollectResourceItem>();
				obj.gameObject.SetActive(hasData);
				bool flag = hasData;
				if (flag)
				{
					CollectResourceResult current = this._collectResults[i];
					for (int j = 0; j < this._collectResults.Count; j++)
					{
						bool flag2 = i == j;
						if (!flag2)
						{
							CollectResourceResult merging = this._collectResults[j];
							bool flag3 = merging.ResourceType != current.ResourceType;
							if (!flag3)
							{
								current.ResourceCount += merging.ResourceCount;
							}
						}
					}
					item.Set(current);
					base.StartCoroutine(item.DropItem());
				}
			}
		}

		// Token: 0x0600A07F RID: 41087 RVA: 0x004AEF38 File Offset: 0x004AD138
		private void StartDig()
		{
			this.hotKeyRoot.GetComponent<RectTransform>().anchoredPosition = this._digCollectPos;
			this._isDigCompleted = false;
			this.singleObj.gameObject.SetActive(true);
			this.multipleObj.gameObject.SetActive(false);
			CanvasGroup canvasGroup = this.singleObj.GetComponent<CanvasGroup>();
			canvasGroup.DOKill(false);
			canvasGroup.alpha = 0f;
			this.DoDig();
		}

		// Token: 0x0600A080 RID: 41088 RVA: 0x004AEFB4 File Offset: 0x004AD1B4
		private void DoDig()
		{
			bool flag = !SingletonObject.getInstance<TimeManager>().IsActionDayEnough(3);
			if (flag)
			{
				this._isDigCompleted = true;
				this.anyKeyContinue.SetActive(true);
				this.seriesContinue.SetActive(false);
			}
			else
			{
				ExtraDomainMethod.AsyncCall.FindTreasure(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._digResult);
					bool requestInvalid = this._digResult.RequestInvalid;
					if (requestInvalid)
					{
						this._isDigCompleted = true;
					}
					else
					{
						bool anyMaterial = this._digResult.AnyMaterial;
						if (anyMaterial)
						{
							GEvent.AddOneShot(UiEvents.OnBrokenMaterialEventInvoked, new GEvent.Callback(this.OnBrokenMaterialEventInvoked));
						}
						else
						{
							this.AnimEntry();
						}
					}
				});
			}
		}

		// Token: 0x0600A081 RID: 41089 RVA: 0x004AF016 File Offset: 0x004AD216
		private void OnBrokenMaterialEventInvoked(ArgumentBox _)
		{
			this.AnimEntry();
		}

		// Token: 0x0600A082 RID: 41090 RVA: 0x004AF020 File Offset: 0x004AD220
		private void AnimEntry()
		{
			bool flag = this._digResult.AnyItem || this._digResult.AnyExtraItem;
			if (flag)
			{
				this._digItemKeys.Clear();
				this._digItemDataList.Clear();
				bool anyItem = this._digResult.AnyItem;
				if (anyItem)
				{
					this._digItemKeys.Add(this._digResult.ItemKey);
				}
				bool anyExtraItem = this._digResult.AnyExtraItem;
				if (anyExtraItem)
				{
					this._digItemKeys.AddRange(this._digResult.ExtraItems);
				}
				ItemDomainMethod.AsyncCall.GetItemDisplayDataList(null, this._digItemKeys, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._digItemDataList);
					foreach (ItemDisplayData displayData in this._digItemDataList)
					{
						bool flag2 = displayData.Key.Equals(this._digResult.ItemKey);
						if (flag2)
						{
							displayData.Amount = (int)this._digResult.ItemCount;
						}
					}
				});
			}
			this._canSkipDigAnimation = false;
			this._skipDigAnimation = false;
			this.singleObj.Destroy();
			this.singleObj.Set(this._digResult);
			this.hintRed.SetActive(false);
			this.hintBlue.SetActive(SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() == 3);
			this.seriesContinue.gameObject.SetActive(this._isDigSeries);
			this.anyKeyContinue.SetActive(false);
			this.successObj.SetActive(false);
			this.singleObj.PlayTreasureAnim(this._isDigSeries);
			base.DelayCall(new Action(this.SetCanSkipDigAnimation), 0.3f);
			base.DelayCall(new Action(this.DigAnimFailed), ViewCollectResource.FindAnimTimeBase / 2f);
			base.DelayCall(new Action(this.AnimFailedFadeOut), ViewCollectResource.FindAnimTimeBase - 0.3f);
			base.StartCoroutine(this.DigWaitOrSkipToEnd());
		}

		// Token: 0x0600A083 RID: 41091 RVA: 0x004AF1C2 File Offset: 0x004AD3C2
		private IEnumerator DigWaitOrSkipToEnd()
		{
			float curTime = 0f;
			while (curTime < ViewCollectResource.FindAnimTimeBase && !this._skipDigAnimation)
			{
				curTime += Time.deltaTime;
				yield return null;
			}
			this.AnimFinalCall();
			yield break;
		}

		// Token: 0x0600A084 RID: 41092 RVA: 0x004AF1D1 File Offset: 0x004AD3D1
		private void SetCanSkipDigAnimation()
		{
			this.skipAnimHotKeyDisplay.SetActive(true);
			this._canSkipDigAnimation = true;
		}

		// Token: 0x0600A085 RID: 41093 RVA: 0x004AF1E8 File Offset: 0x004AD3E8
		private void DigAnimFailed()
		{
			bool flag = !base.gameObject.activeInHierarchy;
			if (!flag)
			{
				bool flag2 = !this._digResult.Success;
				if (flag2)
				{
					this.hintRed.SetActive(true);
					this.hintRed.GetComponent<CanvasGroup>().alpha = 1f;
					this.hintRedText.text = (this._digResult.AnyResource ? LocalStringManager.GetFormat(LanguageKey.LK_Treasure_Find_Failed_Material_Resource, ResourceType.Instance[this._digResult.ResourceType].Icon, this._digResult.ResourceCount).ColorReplace() : LocalStringManager.Get(LanguageKey.LK_Treasure_Find_Failed));
					this.hintRedText.GetComponent<TMPTextSpriteHelper>().Parse();
				}
				else
				{
					bool isDigSeries = this._isDigSeries;
					if (isDigSeries)
					{
						this.InvokeStop();
					}
					this.PlayDigSuccessEffect();
				}
			}
		}

		// Token: 0x0600A086 RID: 41094 RVA: 0x004AF2D4 File Offset: 0x004AD4D4
		private void AnimFailedFadeOut()
		{
			CanvasGroup failedCanvas = this.hintRed.GetComponent<CanvasGroup>();
			bool flag = !this._digResult.Success;
			if (flag)
			{
				this.KillSeq(this._hintRedFadeOutSeq);
				this._hintRedFadeOutSeq = DOTween.Sequence();
				this._hintRedFadeOutSeq.Insert(0f, failedCanvas.DOFade(0f, 0.3f));
				this._hintRedFadeOutSeq.OnComplete(delegate
				{
					this.hintRed.gameObject.SetActive(false);
				});
				this._hintRedFadeOutSeq.Restart(true, -1f);
			}
		}

		// Token: 0x0600A087 RID: 41095 RVA: 0x004AF368 File Offset: 0x004AD568
		private void KillSeq(Sequence seq)
		{
			bool flag = seq == null;
			if (!flag)
			{
				seq.Pause<Sequence>();
				seq.Kill(false);
				seq = null;
			}
		}

		// Token: 0x0600A088 RID: 41096 RVA: 0x004AF392 File Offset: 0x004AD592
		private void InvokeStop()
		{
			this._isDigSeries = false;
			this.hintBlue.SetActive(true);
			this.seriesContinue.gameObject.SetActive(false);
		}

		// Token: 0x0600A089 RID: 41097 RVA: 0x004AF3BC File Offset: 0x004AD5BC
		private void AnimFinalCall()
		{
			bool success = this._digResult.Success;
			if (success)
			{
				this._isDigSeries = false;
			}
			this.skipAnimHotKeyDisplay.SetActive(false);
			bool isDigSeries = this._isDigSeries;
			if (isDigSeries)
			{
				this.DoDig();
			}
			else
			{
				this._isDigCompleted = true;
				this.anyKeyContinue.SetActive(true);
				this.seriesContinue.gameObject.SetActive(false);
				this.mask.SetValueFactor(1f);
				this.Stop();
			}
		}

		// Token: 0x0600A08A RID: 41098 RVA: 0x004AF440 File Offset: 0x004AD640
		private void UpdateTreasure()
		{
			bool flag = this._isDigSeries && CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.InvokeStop();
			}
			bool flag2 = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) && this._canSkipDigAnimation;
			if (flag2)
			{
				this._skipDigAnimation = true;
			}
			bool flag3 = this._isDigCompleted && HotKeyCommand.CheckAnyKeyDown();
			if (flag3)
			{
				base.DelayCall(new Action(this.AnimFinalCall), 0.3f);
				this.singleObj.Destroy();
				this.successObj.SetActive(false);
				CanvasGroup canvasGroup = this.singleObj.GetComponent<CanvasGroup>();
				canvasGroup.DOKill(false);
				canvasGroup.DOFade(0f, 0.3f).OnComplete(new TweenCallback(this.QuickHide));
			}
		}

		// Token: 0x0600A08B RID: 41099 RVA: 0x004AF524 File Offset: 0x004AD724
		private void PlayDigSuccessEffect()
		{
			bool flag = !this._digResult.Success;
			if (!flag)
			{
				bool anyMaterial = this._digResult.AnyMaterial;
				this.getItemEff.SetActive(!anyMaterial);
				this.getMaterialEff.SetActive(anyMaterial);
				this.successObj.SetActive(true);
			}
		}

		// Token: 0x0600A08C RID: 41100 RVA: 0x004AF57C File Offset: 0x004AD77C
		public void InitChoosy()
		{
			this.CurrentCollectType = ViewCollectResource.CollectResourceType.Choosy;
		}

		// Token: 0x0600A08D RID: 41101 RVA: 0x004AF588 File Offset: 0x004AD788
		public void SetChoosy(sbyte resourceType, int resourceAmount)
		{
			ref Coroutine cor = ref this._showingResIconCoroutineList[(int)resourceType];
			bool flag = cor != null;
			if (flag)
			{
				base.StopCoroutine(cor);
			}
			List<GameObject> list = this.ShowingResIconCacheList[(int)resourceType];
			list.ForEach(new Action<GameObject>(this._resourceIconPool.DestroyObject));
			list.Clear();
			Transform obj = this.multipleObj.transform.GetChild((int)resourceType);
			bool flag2 = resourceAmount == 0;
			if (flag2)
			{
				obj.gameObject.SetActive(false);
			}
			else
			{
				obj.gameObject.SetActive(true);
				CollectResourceResult collectResult = new CollectResourceResult
				{
					ResourceType = resourceType,
					ResourceCount = resourceAmount
				};
				CollectResourceItem item = obj.GetComponent<CollectResourceItem>();
				item.Set(collectResult);
				item.SetCountText();
				cor = base.StartCoroutine(item.DropItem());
			}
		}

		// Token: 0x0600A08E RID: 41102 RVA: 0x004AF660 File Offset: 0x004AD860
		public void PlayChoosy(sbyte resourceType, out float duration)
		{
			CollectResourceItem item = this.multipleObj.transform.GetChild((int)resourceType).GetComponent<CollectResourceItem>();
			duration = item.PlayChoosyAnim();
		}

		// Token: 0x0600A08F RID: 41103 RVA: 0x004AF690 File Offset: 0x004AD890
		public void ClearChoosy()
		{
			this.Clear();
			for (int index = 0; index < this._showingResIconCoroutineList.Length; index++)
			{
				Coroutine coroutine = this._showingResIconCoroutineList[index];
				bool flag = coroutine != null;
				if (flag)
				{
					base.StopCoroutine(coroutine);
					this._showingResIconCoroutineList[index] = null;
				}
			}
			for (int i = 0; i < this.multipleObj.transform.childCount; i++)
			{
				this.multipleObj.transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A090 RID: 41104 RVA: 0x004AF728 File Offset: 0x004AD928
		private void ResponseBottomTimeDisk(ArgumentBox argBox)
		{
			Transform timeText;
			bool flag = argBox.Get<Transform>("timeText", out timeText);
			if (flag)
			{
				this._timeBall = timeText;
				this._timeBall.SetParent(this.bottomRoot, true);
				sbyte currentSeason = TimeKit.GetCurrSeason();
				this.seasonIcon.sprite = this.seasonBgSprites[(int)currentSeason];
				this.seasonIcon.transform.SetAsLastSibling();
				this.seasonIcon.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600A091 RID: 41105 RVA: 0x004AF7A0 File Offset: 0x004AD9A0
		private void TryReturnTimeDisk()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			bool flag = this._timeBall != null;
			if (flag)
			{
				bool flag2 = this._timeBall != null;
				if (flag2)
				{
					args.SetObject("timeText", this._timeBall);
				}
				GEvent.OnEvent(UiEvents.ReturnBottomTimeDisk, args);
			}
			this.seasonIcon.gameObject.SetActive(false);
		}

		// Token: 0x04007C45 RID: 31813
		[SerializeField]
		private GameObject resourceIcon;

		// Token: 0x04007C46 RID: 31814
		[SerializeField]
		private GameObject itemIcon;

		// Token: 0x04007C47 RID: 31815
		[SerializeField]
		private GameObject successObj;

		// Token: 0x04007C48 RID: 31816
		[SerializeField]
		private CollectResourceItem singleObj;

		// Token: 0x04007C49 RID: 31817
		[SerializeField]
		private CRawImage multipleObj;

		// Token: 0x04007C4A RID: 31818
		[SerializeField]
		private GameObject skipAnimHotKeyDisplay;

		// Token: 0x04007C4B RID: 31819
		[SerializeField]
		private GameObject anyKeyContinue;

		// Token: 0x04007C4C RID: 31820
		[SerializeField]
		private GameObject seriesContinue;

		// Token: 0x04007C4D RID: 31821
		[SerializeField]
		private GameObject hintRed;

		// Token: 0x04007C4E RID: 31822
		[SerializeField]
		private GameObject hintBlue;

		// Token: 0x04007C4F RID: 31823
		[SerializeField]
		private TextMeshProUGUI hintRedText;

		// Token: 0x04007C50 RID: 31824
		[SerializeField]
		private Transform bottomRoot;

		// Token: 0x04007C51 RID: 31825
		[SerializeField]
		private UIMask mask;

		// Token: 0x04007C52 RID: 31826
		[SerializeField]
		private GameObject getItemEff;

		// Token: 0x04007C53 RID: 31827
		[SerializeField]
		private GameObject getMaterialEff;

		// Token: 0x04007C54 RID: 31828
		[SerializeField]
		private GameObject hotKeyRoot;

		// Token: 0x04007C55 RID: 31829
		[SerializeField]
		private Sprite[] seasonBgSprites;

		// Token: 0x04007C56 RID: 31830
		[SerializeField]
		private CImage seasonIcon;

		// Token: 0x04007C57 RID: 31831
		private const int WudangActionFactor = 10;

		// Token: 0x04007C58 RID: 31832
		private const float TextChangeDelayTime = 0.5f;

		// Token: 0x04007C59 RID: 31833
		public const float FadeTime = 0.3f;

		// Token: 0x04007C5A RID: 31834
		private readonly Vector3 _multipleCollectPos = new Vector3(0f, -212f, 0f);

		// Token: 0x04007C5B RID: 31835
		private readonly Vector3 _singleCollectPos = new Vector3(0f, -10f, 0f);

		// Token: 0x04007C5C RID: 31836
		private readonly Vector3 _digCollectPos = new Vector3(0f, -64f, 0f);

		// Token: 0x04007C5D RID: 31837
		private Sequence _hintRedFadeOutSeq;

		// Token: 0x04007C5E RID: 31838
		private static readonly float FindAnimTimeBase = 3f;

		// Token: 0x04007C5F RID: 31839
		private PoolItem _resourceIconPool;

		// Token: 0x04007C60 RID: 31840
		private PoolItem _itemIconPool;

		// Token: 0x04007C61 RID: 31841
		[NonSerialized]
		public readonly List<GameObject>[] ShowingResIconCacheList = new List<GameObject>[6];

		// Token: 0x04007C62 RID: 31842
		private readonly List<GameObject> _showingItemIconCacheList = new List<GameObject>();

		// Token: 0x04007C63 RID: 31843
		private readonly List<GameObject> _showingPracticeIconCacheList = new List<GameObject>();

		// Token: 0x04007C64 RID: 31844
		[NonSerialized]
		public ViewCollectResource.CollectResourceType CurrentCollectType;

		// Token: 0x04007C65 RID: 31845
		private List<CollectResourceResult> _collectResults;

		// Token: 0x04007C66 RID: 31846
		private bool _collectResourceIsMax;

		// Token: 0x04007C67 RID: 31847
		private sbyte _wudangResourceType = -1;

		// Token: 0x04007C68 RID: 31848
		private bool _collecting;

		// Token: 0x04007C69 RID: 31849
		private bool _isDigCompleted;

		// Token: 0x04007C6A RID: 31850
		private bool _isDigSeries;

		// Token: 0x04007C6B RID: 31851
		private bool _canSkipDigAnimation;

		// Token: 0x04007C6C RID: 31852
		private bool _skipDigAnimation;

		// Token: 0x04007C6D RID: 31853
		private TreasureFindResult _digResult;

		// Token: 0x04007C6E RID: 31854
		private List<ItemKey> _digItemKeys = new List<ItemKey>();

		// Token: 0x04007C6F RID: 31855
		private List<ItemDisplayData> _digItemDataList = new List<ItemDisplayData>();

		// Token: 0x04007C70 RID: 31856
		private readonly Coroutine[] _showingResIconCoroutineList = new Coroutine[6];

		// Token: 0x04007C71 RID: 31857
		private Transform _timeBall;

		// Token: 0x02002375 RID: 9077
		public enum CollectResourceType
		{
			// Token: 0x0400DEE1 RID: 57057
			Normal,
			// Token: 0x0400DEE2 RID: 57058
			Choosy,
			// Token: 0x0400DEE3 RID: 57059
			SavageSkill,
			// Token: 0x0400DEE4 RID: 57060
			Practice,
			// Token: 0x0400DEE5 RID: 57061
			Treasure
		}
	}
}
