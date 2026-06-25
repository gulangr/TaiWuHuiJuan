using System;
using UnityEngine.EventSystems;

// Token: 0x02000194 RID: 404
public abstract class AdventureEditorPropertyEditorBase : UIBehaviour
{
	// Token: 0x060016AD RID: 5805
	public abstract void Refresh(string propertyName, object value, Action onValueUpdate);
}
