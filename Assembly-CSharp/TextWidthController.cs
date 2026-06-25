using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200009A RID: 154
public class TextWidthController : MonoBehaviour
{
	// Token: 0x06000557 RID: 1367 RVA: 0x00024310 File Offset: 0x00022510
	private void Start()
	{
		bool flag = this.textMeshPro != null;
		if (flag)
		{
			this.layoutElement = this.textMeshPro.GetComponent<LayoutElement>();
			bool flag2 = this.layoutElement == null;
			if (flag2)
			{
				this.layoutElement = this.textMeshPro.gameObject.AddComponent<LayoutElement>();
			}
			this.textMeshPro.RegisterDirtyVerticesCallback(new UnityAction(this.OnTextUpdated));
			this.textMeshPro.RegisterDirtyLayoutCallback(new UnityAction(this.OnTextUpdated));
			this.UpdateLayoutWidth();
			this.isListening = true;
		}
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x000243A8 File Offset: 0x000225A8
	private void OnTextUpdated()
	{
		bool flag = this.textMeshPro == null || this.layoutElement == null || !this.textMeshPro.gameObject.activeInHierarchy || !this.textMeshPro.isActiveAndEnabled;
		if (!flag)
		{
			base.StartCoroutine(this.UpdateLayoutWidth());
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x00024409 File Offset: 0x00022609
	private IEnumerator UpdateLayoutWidth()
	{
		yield return null;
		this.textMeshPro.ForceMeshUpdate(false, false);
		float textWidth = this.textMeshPro.preferredWidth;
		bool flag = textWidth > this.maxWidth;
		if (flag)
		{
			this.layoutElement.preferredWidth = this.maxWidth;
		}
		else
		{
			this.layoutElement.preferredWidth = -1f;
		}
		yield break;
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x00024418 File Offset: 0x00022618
	private void OnDestroy()
	{
		bool flag = this.isListening && this.textMeshPro != null;
		if (flag)
		{
			this.textMeshPro.UnregisterDirtyVerticesCallback(new UnityAction(this.OnTextUpdated));
			this.textMeshPro.UnregisterDirtyLayoutCallback(new UnityAction(this.OnTextUpdated));
		}
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x00024473 File Offset: 0x00022673
	public void ForceUpdate()
	{
		base.StartCoroutine(this.UpdateLayoutWidth());
	}

	// Token: 0x0400045E RID: 1118
	[SerializeField]
	private TMP_Text textMeshPro;

	// Token: 0x0400045F RID: 1119
	[SerializeField]
	private float maxWidth;

	// Token: 0x04000460 RID: 1120
	[SerializeField]
	private LayoutElement layoutElement;

	// Token: 0x04000461 RID: 1121
	private bool isListening;
}
