using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000209 RID: 521
public class GMCheckAvatarLine : MonoBehaviour
{
	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06002131 RID: 8497 RVA: 0x000F1F25 File Offset: 0x000F0125
	public GMCheckAvatarComponentType ComponentType
	{
		get
		{
			return this.componentType;
		}
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000F1F30 File Offset: 0x000F0130
	public void Initialize(GMCheckAvatarComponentType newComponentType, Action<GMCheckAvatarComponentType, int> onStep, Action<GMCheckAvatarComponentType> onClear)
	{
		this.componentType = newComponentType;
		this._onStep = onStep;
		this._onClear = onClear;
		this.previousButton.onClick.RemoveAllListeners();
		this.previousButton.onClick.AddListener(delegate()
		{
			Action<GMCheckAvatarComponentType, int> onStep2 = this._onStep;
			if (onStep2 != null)
			{
				onStep2(this.componentType, -1);
			}
		});
		this.nextButton.onClick.RemoveAllListeners();
		this.nextButton.onClick.AddListener(delegate()
		{
			Action<GMCheckAvatarComponentType, int> onStep2 = this._onStep;
			if (onStep2 != null)
			{
				onStep2(this.componentType, 1);
			}
		});
		this.clearButton.onClick.RemoveAllListeners();
		this.clearButton.onClick.AddListener(delegate()
		{
			Action<GMCheckAvatarComponentType> onClear2 = this._onClear;
			if (onClear2 != null)
			{
				onClear2(this.componentType);
			}
		});
		this.RefreshTexts();
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000F1FE4 File Offset: 0x000F01E4
	public void RefreshTexts()
	{
		GMCheckAvatarLineDefinition definition = GMCheckAvatarState.GetLineDefinition(this.componentType);
		this.nameText.text = LocalStringManager.Get(definition.NameKey);
		this.clearButton.gameObject.SetActive(definition.CanClear);
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000F2030 File Offset: 0x000F0230
	public void RefreshInteractable(bool canOperate)
	{
		GMCheckAvatarLineDefinition definition = GMCheckAvatarState.GetLineDefinition(this.componentType);
		this.previousButton.interactable = canOperate;
		this.nextButton.interactable = canOperate;
		this.clearButton.interactable = (canOperate && definition.CanClear);
	}

	// Token: 0x040019A8 RID: 6568
	[SerializeField]
	private GMCheckAvatarComponentType componentType;

	// Token: 0x040019A9 RID: 6569
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x040019AA RID: 6570
	[SerializeField]
	private Button previousButton;

	// Token: 0x040019AB RID: 6571
	[SerializeField]
	private Button nextButton;

	// Token: 0x040019AC RID: 6572
	[SerializeField]
	private Button clearButton;

	// Token: 0x040019AD RID: 6573
	private Action<GMCheckAvatarComponentType, int> _onStep;

	// Token: 0x040019AE RID: 6574
	private Action<GMCheckAvatarComponentType> _onClear;
}
