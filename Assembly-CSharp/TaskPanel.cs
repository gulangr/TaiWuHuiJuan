using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameData.Domains.Map;
using GameData.Domains.World.Task;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200030B RID: 779
public class TaskPanel : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerExitHandler
{
	// Token: 0x06002DEC RID: 11756 RVA: 0x0016BA1C File Offset: 0x00169C1C
	public void PlayAnimationAndEffect()
	{
		this.iconGo.SetActive(false);
		this.SkeletonGraphicGo.SetActive(true);
		this._skeletonGraphic.Initialize(true);
		this._skeletonGraphic.AnimationState.SetAnimation(0, this._skeletonGraphic.SkeletonData.Animations.Items[0].Name, true);
		this.effect.SetActive(true);
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x0016BA8D File Offset: 0x00169C8D
	private void StopPlayAnimationAndEffect()
	{
		this.SkeletonGraphicGo.SetActive(false);
		this.effect.SetActive(false);
		this.iconGo.SetActive(true);
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x0016BAB7 File Offset: 0x00169CB7
	public void KillParticle()
	{
		this.effect.SetActive(false);
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x0016BAC7 File Offset: 0x00169CC7
	public void SetData(List<TaskDisplayData> data, bool isAnimation = true)
	{
		if (this._poolItem == null)
		{
			this._poolItem = new PoolItem("TaskPreviewPool", this._taskPreview);
		}
		this.Clear(isAnimation);
		this.Init(data);
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x0016BAF8 File Offset: 0x00169CF8
	public void Clear(bool isAnimation = true)
	{
		if (isAnimation)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = this._titleText.DOFade(0f, 0.5f);
			tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
			{
				this._titleText.DOFade(1f, 0.5f);
			}));
			TweenerCore<Color, Color, ColorOptions> tweenerCore2 = this._previewTitleTMP.DOFade(0f, 0.5f);
			tweenerCore2.onComplete = (TweenCallback)Delegate.Combine(tweenerCore2.onComplete, new TweenCallback(delegate()
			{
				this._previewTitleTMP.DOFade(1f, 0.5f);
			}));
			TweenerCore<float, float, FloatOptions> tweenerCore3 = this.previewsCanvasGroup.DOFade(0f, 0.5f);
			tweenerCore3.onComplete = (TweenCallback)Delegate.Combine(tweenerCore3.onComplete, new TweenCallback(delegate()
			{
				this.previewsCanvasGroup.DOFade(1f, 0.5f);
			}));
		}
		foreach (TaskPreview t in this._taskPreviews)
		{
			this._poolItem.DestroyObject(t.gameObject);
		}
		this._taskPreviews.Clear();
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x0016BC14 File Offset: 0x00169E14
	public void Open()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x0016BC24 File Offset: 0x00169E24
	public void Close()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x0016BC34 File Offset: 0x00169E34
	private void Init(List<TaskDisplayData> dataList)
	{
		bool flag = dataList.Count == 0;
		if (!flag)
		{
			this.taskDisplayData = dataList;
			TaskData taskData = dataList[0].InnerTaskData;
			short mapAreaId = dataList[0].TargetLocation.AreaId;
			MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
			string invalid = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid);
			switch (this._taskChain.GetItem(taskData.TaskChainId).Group)
			{
			case ETaskChainGroup.MainStory:
				this._taskPanelTitle.SetSprite("task_anniu_0", false, null);
				break;
			case ETaskChainGroup.OptionalTasks:
				this._taskPanelTitle.SetSprite("task_anniu_2", false, null);
				break;
			case ETaskChainGroup.SectMainStory:
				this._taskPanelTitle.SetSprite("task_anniu_1", false, null);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this._titleText.text = ((dataList[0].DisplayType != 0) ? TaskPanelHelper.GetTitleString(dataList[0].DisplayType, mapAreaId, dataList[0], areas, invalid) : this._taskInfo.GetItem(taskData.TaskInfoId).TaskOverview.ColorReplace());
			bool flag2 = !taskData.IsParallel;
			if (flag2)
			{
				string taskDescription = (dataList[0].DisplayType != 0) ? TaskPanelHelper.GetTaskDescription(dataList[0].DisplayType, dataList[0], areas, invalid) : this._taskInfo.GetItem(taskData.TaskInfoId).TaskDescription;
				this.GenerateTaskPreview("", taskDescription);
			}
			else
			{
				foreach (TaskDisplayData item in dataList)
				{
					string taskDescription = (item.DisplayType != 0) ? TaskPanelHelper.GetTaskDescription(item.DisplayType, item, areas, invalid) : this._taskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
					this.GenerateTaskPreview(this._taskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskTitle, taskDescription);
				}
			}
		}
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x0016BE68 File Offset: 0x0016A068
	private void GenerateTaskPreview(string title, string content)
	{
		this._taskPreviewClass = this._poolItem.GetObject().GetComponent<TaskPreview>();
		this._taskPreviewClass.SetData(title, content);
		this._taskPreviewClass.transform.SetParent(this._taskPreviewParent, false);
		this._taskPreviews.Add(this._taskPreviewClass);
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x0016BEC4 File Offset: 0x0016A0C4
	private void OnDestroy()
	{
		bool flag = this._poolItem == null;
		if (!flag)
		{
			this._poolItem.Destroy();
			this._poolItem = null;
		}
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x0016BEF4 File Offset: 0x0016A0F4
	public void SetPanelInteractionState(TaskPanel.MainTaskPanelState state)
	{
		this._state = state;
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x0016BF00 File Offset: 0x0016A100
	public void OnPointerEnter(PointerEventData eventData)
	{
		base.StopAllCoroutines();
		bool flag = this._tween != null;
		if (flag)
		{
			CommonUtils.TryKillTween(this._tween, false);
			this._tween = null;
		}
		this._dropDownBox.sizeDelta = new Vector2(this._dropDownBox.rect.width, 0f);
		this._taskTitleHighlight.SetAlpha(1f);
		this.StopPlayAnimationAndEffect();
		base.StartCoroutine(this.Expand());
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x0016BF88 File Offset: 0x0016A188
	public void OnPointerDown(PointerEventData eventData)
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!inGuiding)
		{
			bool flag = !Input.GetMouseButtonDown(0) || this._state > TaskPanel.MainTaskPanelState.Interaction;
			if (!flag)
			{
				UIManager.Instance.ShowUI(UIElement.TaskPopPanel, true);
			}
		}
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x0016BFD4 File Offset: 0x0016A1D4
	public void OnPointerExit(PointerEventData eventData)
	{
		base.StopAllCoroutines();
		bool flag = this._tween != null;
		if (flag)
		{
			CommonUtils.TryKillTween(this._tween, false);
			this._tween = null;
		}
		this._taskTitleHighlight.SetAlpha(0f);
		this.Collapse();
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x0016C024 File Offset: 0x0016A224
	private IEnumerator Expand()
	{
		this._dropDownBox.gameObject.SetActive(true);
		this._content.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(this._content.GetComponent<RectTransform>());
		yield return null;
		LayoutRebuilder.ForceRebuildLayoutImmediate(this._content.GetComponent<RectTransform>());
		yield return null;
		float height = this._contentRect.height;
		float maskAndPanelHeight = (height >= 370f) ? 370f : (height + 35f);
		this._maskRect.SetHeight(maskAndPanelHeight);
		this._thisRect.SetHeight(maskAndPanelHeight);
		float targetHeight = (height >= 370f) ? 370f : (height + 30f);
		this._tween = this._dropDownBox.DOSizeDelta(new Vector2(this._dropDownBox.rect.width, targetHeight), 0.25f, false);
		Tween tween = this._tween;
		tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, new TweenCallback(delegate()
		{
			this._verticalScrollView.enabled = true;
			this._dropDownBox.Find("Icon").gameObject.SetActive(true);
			bool flag = Math.Abs(targetHeight - 370f) < 0.1f;
			if (flag)
			{
				this._slider.transform.localPosition = new Vector3(200f, this._slider.transform.position.y);
				this._content.GetComponent<CanvasGroup>().alpha = 1f;
				this._dropDownBox.sizeDelta = new Vector2(this._dropDownBox.rect.width, 370f);
				this._slider.SetActive(true);
			}
			else
			{
				this._slider.SetActive(false);
				this._slider.transform.localPosition = new Vector3(500f, this._slider.transform.position.y);
				this._content.SetActive(true);
				this._content.GetComponent<CanvasGroup>().alpha = 1f;
			}
		}));
		yield break;
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x0016C034 File Offset: 0x0016A234
	private void Collapse()
	{
		this._slider.SetActive(false);
		this._content.GetComponent<CanvasGroup>().alpha = 0f;
		this._content.SetActive(false);
		this._dropDownBox.Find("Icon").gameObject.SetActive(false);
		TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = this._dropDownBox.DOSizeDelta(new Vector2(this._dropDownBox.rect.width, 0f), 0.15f, false);
		tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
		{
			this._verticalScrollView.enabled = false;
		}));
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x0016C0E0 File Offset: 0x0016A2E0
	private void Update()
	{
		bool flag = this._dropDownBox.rect.height < 150f;
		if (flag)
		{
			this._slider.SetActive(false);
		}
	}

	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x06002DFD RID: 11773 RVA: 0x0016C11B File Offset: 0x0016A31B
	private TaskInfo _taskInfo
	{
		get
		{
			return TaskInfo.Instance;
		}
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x06002DFE RID: 11774 RVA: 0x0016C122 File Offset: 0x0016A322
	private TaskChain _taskChain
	{
		get
		{
			return TaskChain.Instance;
		}
	}

	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x06002DFF RID: 11775 RVA: 0x0016C129 File Offset: 0x0016A329
	private Rect _contentRect
	{
		get
		{
			return this._content.GetComponent<RectTransform>().rect;
		}
	}

	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x06002E00 RID: 11776 RVA: 0x0016C13B File Offset: 0x0016A33B
	private GameObject SkeletonGraphicGo
	{
		get
		{
			return this._skeletonGraphic.gameObject;
		}
	}

	// Token: 0x0400214D RID: 8525
	[SerializeField]
	private TextMeshProUGUI _titleText;

	// Token: 0x0400214E RID: 8526
	[SerializeField]
	private RectTransform _taskPreviewParent;

	// Token: 0x0400214F RID: 8527
	[SerializeField]
	private CImage _taskPanelTitle;

	// Token: 0x04002150 RID: 8528
	[SerializeField]
	private CImage _taskTitleHighlight;

	// Token: 0x04002151 RID: 8529
	[SerializeField]
	private CEmptyGraphic _verticalScrollView;

	// Token: 0x04002152 RID: 8530
	private List<TaskPreview> _taskPreviews = new List<TaskPreview>();

	// Token: 0x04002153 RID: 8531
	public GameObject _taskPreview;

	// Token: 0x04002154 RID: 8532
	private PoolItem _poolItem;

	// Token: 0x04002155 RID: 8533
	private TaskPreview _taskPreviewClass;

	// Token: 0x04002156 RID: 8534
	[SerializeField]
	private RectTransform _thisRect;

	// Token: 0x04002157 RID: 8535
	[SerializeField]
	private RectTransform _maskRect;

	// Token: 0x04002158 RID: 8536
	[SerializeField]
	private RectTransform _dropDownBox;

	// Token: 0x04002159 RID: 8537
	[SerializeField]
	private GameObject _slider;

	// Token: 0x0400215A RID: 8538
	[SerializeField]
	private GameObject _content;

	// Token: 0x0400215B RID: 8539
	[SerializeField]
	private TextMeshProUGUI _previewTitleTMP;

	// Token: 0x0400215C RID: 8540
	[SerializeField]
	private SkeletonGraphic _skeletonGraphic;

	// Token: 0x0400215D RID: 8541
	[SerializeField]
	private GameObject iconGo;

	// Token: 0x0400215E RID: 8542
	[SerializeField]
	private GameObject effect;

	// Token: 0x0400215F RID: 8543
	[SerializeField]
	private CanvasGroup previewsCanvasGroup;

	// Token: 0x04002160 RID: 8544
	private TaskPanel.MainTaskPanelState _state = TaskPanel.MainTaskPanelState.Interaction;

	// Token: 0x04002161 RID: 8545
	private const int MaxHeight = 370;

	// Token: 0x04002162 RID: 8546
	private List<TaskDisplayData> taskDisplayData = new List<TaskDisplayData>();

	// Token: 0x04002163 RID: 8547
	private const string MainTaskSpritePath = "task_anniu_0";

	// Token: 0x04002164 RID: 8548
	private const string SectMainTaskSpritePath = "task_anniu_1";

	// Token: 0x04002165 RID: 8549
	private const string OptionalTaskSpritePath = "task_anniu_2";

	// Token: 0x04002166 RID: 8550
	private Tween _tween;

	// Token: 0x02001694 RID: 5780
	public enum MainTaskPanelState
	{
		// Token: 0x0400A860 RID: 43104
		Interaction,
		// Token: 0x0400A861 RID: 43105
		NoInteraction
	}
}
