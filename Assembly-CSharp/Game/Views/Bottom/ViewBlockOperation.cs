using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UI;
using FrameWork.UISystem.UIElements;
using Game.Views.Map;
using GameData.Domains.Building;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C4A RID: 3146
	public class ViewBlockOperation : UIBase, IBlockButtonParent, IAsyncMethodRequestHandler
	{
		// Token: 0x170010D9 RID: 4313
		// (get) Token: 0x06009FEE RID: 40942 RVA: 0x004AACC1 File Offset: 0x004A8EC1
		public MapBlockData BlockData
		{
			get
			{
				return this._selectedBlock;
			}
		}

		// Token: 0x170010DA RID: 4314
		// (get) Token: 0x06009FEF RID: 40943 RVA: 0x004AACC9 File Offset: 0x004A8EC9
		public int CharId
		{
			get
			{
				return this._charId;
			}
		}

		// Token: 0x170010DB RID: 4315
		// (get) Token: 0x06009FF0 RID: 40944 RVA: 0x004AACD1 File Offset: 0x004A8ED1
		// (set) Token: 0x06009FF1 RID: 40945 RVA: 0x004AACDC File Offset: 0x004A8EDC
		public bool MoveDoneRegistered
		{
			get
			{
				return this._moveDoneRegistered;
			}
			set
			{
				bool flag = this._moveDoneRegistered == value;
				if (!flag)
				{
					GameObject gameObject = this.animRoot.gameObject;
					this._moveDoneRegistered = value;
					gameObject.SetActive(!value);
					SingletonObject.getInstance<UIMaskManager>().SetSharedMaskInstanceActive(this.Element, !value);
					if (value)
					{
						GEvent.Add(UiEvents.OnMapBlockMoveFinished, new GEvent.Callback(this.MoveDone));
					}
					else
					{
						GEvent.Remove(UiEvents.OnMapBlockMoveFinished, new GEvent.Callback(this.MoveDone));
						this.QuickHide();
					}
				}
			}
		}

		// Token: 0x170010DC RID: 4316
		// (get) Token: 0x06009FF2 RID: 40946 RVA: 0x004AAD6E File Offset: 0x004A8F6E
		// (set) Token: 0x06009FF3 RID: 40947 RVA: 0x004AAD78 File Offset: 0x004A8F78
		public bool DisableMove
		{
			get
			{
				return this._disableMove;
			}
			set
			{
				this._disableMove = value;
				ViewWorldMap.SetDisableMoving(value);
			}
		}

		// Token: 0x06009FF4 RID: 40948 RVA: 0x004AAD98 File Offset: 0x004A8F98
		private void OnTopUiChanged(ArgumentBox _)
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.StateMainWorld) && !UIManager.Instance.IsFocusElement(UIElement.Bottom);
			if (flag)
			{
				this._topUiChangeDetectFlag = false;
			}
			bool flag2 = !this.ExtraViewOpened && !UIManager.Instance.IsFocusElement(UIElement.BlockOperation) && !UIManager.Instance.IsFocusElement(UIElement.SelectChar);
			if (flag2)
			{
				this.QuickHide();
			}
			bool flag3 = UIManager.Instance.IsFocusElement(UIElement.BlockOperation);
			if (flag3)
			{
				this.ExtraViewOpened = false;
			}
			bool flag4 = this.DisableMove && !UIManager.Instance.IsFocusElement(UIElement.CollectResource) && !UIManager.Instance.IsFocusElement(UIElement.GetItem) && !UIManager.Instance.IsFocusElement(UIElement.EventWindow);
			if (flag4)
			{
				this.DisableMove = false;
			}
		}

		// Token: 0x06009FF5 RID: 40949 RVA: 0x004AAE78 File Offset: 0x004A9078
		public override void QuickHide()
		{
			this._topUiChangeDetectFlag = (this.DisableMove = (this.MoveDoneRegistered = false));
			base.QuickHide();
		}

		// Token: 0x06009FF6 RID: 40950 RVA: 0x004AAEA8 File Offset: 0x004A90A8
		private void Awake()
		{
			this.quickHide.onClick.ResetListener(new Action(this.QuickHide));
		}

		// Token: 0x06009FF7 RID: 40951 RVA: 0x004AAECC File Offset: 0x004A90CC
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			this.timeConsume.gameObject.SetActive(this.selectingImage.enabled = false);
			this.btnName.text = (this.simple.text = (this.complex.text = ""));
			this.animRoot.gameObject.SetActive(false);
		}

		// Token: 0x06009FF8 RID: 40952 RVA: 0x004AAF54 File Offset: 0x004A9154
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x06009FF9 RID: 40953 RVA: 0x004AAF70 File Offset: 0x004A9170
		public override void OnInit(ArgumentBox argBox)
		{
			bool moveDoneRegistered = this.MoveDoneRegistered;
			if (moveDoneRegistered)
			{
				this.MoveDoneRegistered = false;
			}
			bool flag = argBox == null || !argBox.Get<Location>("Location", out this.SelectedLocation);
			if (flag)
			{
				this.SelectedLocation = SingletonObject.getInstance<WorldMapModel>().SelectedBlock.GetLocation();
			}
			this.animRoot.gameObject.SetActive(false);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.Refresh));
			this.tips.SetActive(SingletonObject.getInstance<WorldMapModel>().CurrentLocation != this.SelectedLocation);
		}

		// Token: 0x06009FFA RID: 40954 RVA: 0x004AB028 File Offset: 0x004A9228
		public void Refresh()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
				this._selectedBlock = ((model.SelectedBlock.GetLocation() != this.SelectedLocation) ? model.GetBlockData(this.SelectedLocation) : model.SelectedBlock);
				this._moveCost = ((model.CurrentBlockId == this.SelectedLocation.BlockId) ? 0 : model.GetMoveCost(this._selectedBlock.GetLocation()));
				base.StartCoroutine(this.RefreshWorkingCharacter());
			}
		}

		// Token: 0x06009FFB RID: 40955 RVA: 0x004AB0BC File Offset: 0x004A92BC
		public IEnumerator RefreshWorkingCharacter()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this._charId = -1;
			this._workData = null;
			bool flag = this._selectedBlock != null;
			if (flag)
			{
				using (IEnumerator<KeyValuePair<int, VillagerWorkData>> enumerator = (from pair in SingletonObject.getInstance<BuildingModel>().VillagerWork
				where pair.Value.AreaId == this._selectedBlock.AreaId && pair.Value.BlockId == this._selectedBlock.BlockId && VillagerWorkType.IsWorkOnMapAndNeedMark(pair.Value.WorkType)
				select pair).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<int, VillagerWorkData> pair2 = enumerator.Current;
						this._charId = pair2.Key;
						this._workData = pair2.Value;
					}
				}
				IEnumerator<KeyValuePair<int, VillagerWorkData>> enumerator = null;
			}
			byte i = 0;
			while ((int)i < this.buttons.Length)
			{
				this.buttons[(int)i].Init(this, i);
				byte b = i;
				i = b + 1;
			}
			GameObject gameObject = this.workIsIdle.gameObject;
			VillagerWorkData workData = this._workData;
			gameObject.SetActive(workData != null && workData.WorkType == 13);
			GameObject gameObject2 = this.workIsKeepingGrave.gameObject;
			workData = this._workData;
			gameObject2.SetActive(workData != null && workData.WorkType == 12);
			BuildingDomainMethod.AsyncCall.RequestUnlockedWorkingVillagers(this, delegate(int offset, RawDataPool pool)
			{
				List<int> unlockedWorkingList = new List<int>();
				Serializer.Deserialize(pool, offset, ref unlockedWorkingList);
				bool isLock = !unlockedWorkingList.Contains(this._charId);
				this.workerIsLocked.gameObject.SetActive(isLock);
				bool flag2 = isLock;
				if (flag2)
				{
					this.buttons[14].Name = LanguageKey.LK_BlockOperation_UnlockAssign_Name.Tr();
					this.buttons[14].Simple = LanguageKey.LK_BlockOperation_UnlockAssign_Summary.Tr();
				}
				else
				{
					this.buttons[14].Name = BlockButton.DefValue.LockAssign.Name;
					this.buttons[14].Simple = BlockButton.DefValue.LockAssign.Summary;
				}
				BlockButton currHoverChild = this._currHoverChild;
				bool flag3 = ((currHoverChild != null) ? new byte?(currHoverChild.templateId) : null) == 14;
				if (flag3)
				{
					this.OnChildEnter(this._currHoverChild);
				}
				this.Element.ShowAfterRefresh();
				this.animRoot.gameObject.SetActive(true);
			});
			yield break;
		}

		// Token: 0x06009FFC RID: 40956 RVA: 0x004AB0CB File Offset: 0x004A92CB
		public void Hide(BlockButton child)
		{
			this._playAudioOut = false;
			this.OnChildExit(child);
			this.MoveDoneRegistered = false;
			this.QuickHide();
		}

		// Token: 0x06009FFD RID: 40957 RVA: 0x004AB0EC File Offset: 0x004A92EC
		public void Show(BlockButton child)
		{
			UIManager.Instance.MaskUI(UIElement.BlockOperation);
		}

		// Token: 0x06009FFE RID: 40958 RVA: 0x004AB100 File Offset: 0x004A9300
		public void OnChildEnter(BlockButton child)
		{
			this._currHoverChild = child;
			child.SetText(this.selectingImage, this.btnName, this.simple, this.complex);
			bool flag = child.ConsumeTime >= 0 && this.MoveCost >= 0;
			if (flag)
			{
				this.timeConsume.gameObject.SetActive(true);
				this.timeConsumeKey.text = child.ConsumeTimeDesc;
				this.timeConsumeValue.text = ((this._moveCost > 0) ? string.Format(" {0} + {1:f1}", (double)child.ConsumeTime * 0.1, (double)this._moveCost * 0.1) : string.Format(" {0}", (double)child.ConsumeTime * 0.1));
			}
			else
			{
				this.timeConsume.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009FFF RID: 40959 RVA: 0x004AB1FC File Offset: 0x004A93FC
		public void OnChildExit(BlockButton child)
		{
			this._currHoverChild = null;
			this.timeConsume.gameObject.SetActive(this.selectingImage.enabled = false);
			this.btnName.text = (this.simple.text = (this.complex.text = ""));
		}

		// Token: 0x0600A000 RID: 40960 RVA: 0x004AB264 File Offset: 0x004A9464
		public void MoveToBlock(BlockButton child)
		{
			this._playAudioOut = false;
			this._currChild = child;
			this.DisableMove = (this.MoveDoneRegistered = true);
			ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			worldMap.MoveToBlock(this._targetLocation = this.BlockData.GetLocation());
		}

		// Token: 0x0600A001 RID: 40961 RVA: 0x004AB2B8 File Offset: 0x004A94B8
		private void MoveDone(ArgumentBox _)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool flag = !this._targetLocation.IsValid() || mapModel.CurrentLocation != this._targetLocation;
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				this._topUiChangeDetectFlag = true;
				SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.MoveDoneImpl);
			}
		}

		// Token: 0x170010DD RID: 4317
		// (get) Token: 0x0600A002 RID: 40962 RVA: 0x004AB314 File Offset: 0x004A9514
		private IEnumerator MoveDoneImpl
		{
			get
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				bool topUiChangeDetectFlag = this._topUiChangeDetectFlag;
				if (topUiChangeDetectFlag)
				{
					this._topUiChangeDetectFlag = false;
					BlockButton.Template.OnClick(this._currChild);
				}
				else
				{
					this.MoveDoneRegistered = false;
				}
				yield break;
			}
		}

		// Token: 0x170010DE RID: 4318
		// (get) Token: 0x0600A003 RID: 40963 RVA: 0x004AB332 File Offset: 0x004A9532
		// (set) Token: 0x0600A004 RID: 40964 RVA: 0x004AB33A File Offset: 0x004A953A
		public bool ExtraViewOpened { get; set; }

		// Token: 0x170010DF RID: 4319
		// (get) Token: 0x0600A005 RID: 40965 RVA: 0x004AB343 File Offset: 0x004A9543
		public int MoveCost
		{
			get
			{
				return this._moveCost;
			}
		}

		// Token: 0x0600A006 RID: 40966 RVA: 0x004AB34C File Offset: 0x004A954C
		public override void PlayAudioOut()
		{
			bool playAudioOut = this._playAudioOut;
			if (playAudioOut)
			{
				base.PlayAudioOut();
			}
			else
			{
				this._playAudioOut = true;
			}
		}

		// Token: 0x0600A007 RID: 40967 RVA: 0x004AB374 File Offset: 0x004A9574
		public void MuteOnDisable()
		{
			this._playAudioOut = false;
		}

		// Token: 0x04007BCF RID: 31695
		[SerializeField]
		private BlockButton[] buttons;

		// Token: 0x04007BD0 RID: 31696
		[SerializeField]
		private CImage selectingImage;

		// Token: 0x04007BD1 RID: 31697
		[SerializeField]
		private CImage workIsIdle;

		// Token: 0x04007BD2 RID: 31698
		[SerializeField]
		private CImage workIsKeepingGrave;

		// Token: 0x04007BD3 RID: 31699
		[SerializeField]
		private CImage workerIsLocked;

		// Token: 0x04007BD4 RID: 31700
		[SerializeField]
		private TMP_Text btnName;

		// Token: 0x04007BD5 RID: 31701
		[SerializeField]
		private TMP_Text simple;

		// Token: 0x04007BD6 RID: 31702
		[SerializeField]
		private TMP_Text complex;

		// Token: 0x04007BD7 RID: 31703
		[SerializeField]
		private TMP_Text timeConsumeKey;

		// Token: 0x04007BD8 RID: 31704
		[SerializeField]
		private TMP_Text timeConsumeValue;

		// Token: 0x04007BD9 RID: 31705
		[SerializeField]
		private RectTransform timeConsume;

		// Token: 0x04007BDA RID: 31706
		[SerializeField]
		private RectTransform animRoot;

		// Token: 0x04007BDB RID: 31707
		[SerializeField]
		private CButton quickHide;

		// Token: 0x04007BDC RID: 31708
		[SerializeField]
		private GameObject tips;

		// Token: 0x04007BDD RID: 31709
		private MapBlockData _selectedBlock;

		// Token: 0x04007BDE RID: 31710
		private int _moveCost;

		// Token: 0x04007BDF RID: 31711
		private int _charId;

		// Token: 0x04007BE0 RID: 31712
		private bool _disableMove;

		// Token: 0x04007BE1 RID: 31713
		private bool _moveDoneRegistered;

		// Token: 0x04007BE2 RID: 31714
		public Location SelectedLocation = Location.Invalid;

		// Token: 0x04007BE3 RID: 31715
		private VillagerWorkData _workData;

		// Token: 0x04007BE4 RID: 31716
		private BlockButton _currHoverChild;

		// Token: 0x04007BE5 RID: 31717
		private Location _targetLocation;

		// Token: 0x04007BE6 RID: 31718
		private bool _topUiChangeDetectFlag = false;

		// Token: 0x04007BE7 RID: 31719
		private BlockButton _currChild;

		// Token: 0x04007BE9 RID: 31721
		private bool _playAudioOut = true;
	}
}
