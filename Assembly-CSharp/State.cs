using System;
using System.Diagnostics;
using FrameWork;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003B RID: 59
[Serializable]
public class State : BaseState
{
	// Token: 0x06000202 RID: 514 RVA: 0x0000C4CA File Offset: 0x0000A6CA
	public State(Enum state)
	{
		this.stateName = state;
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000203 RID: 515 RVA: 0x0000C4DC File Offset: 0x0000A6DC
	// (remove) Token: 0x06000204 RID: 516 RVA: 0x0000C514 File Offset: 0x0000A714
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<float> onUpdate;

	// Token: 0x06000205 RID: 517 RVA: 0x0000C549 File Offset: 0x0000A749
	public override void OnEnter(ArgumentBox argsBox)
	{
		UnityEvent onEnter = this._onEnter;
		if (onEnter != null)
		{
			onEnter.Invoke();
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000C55E File Offset: 0x0000A75E
	public override void OnExit()
	{
		UnityEvent onExit = this._onExit;
		if (onExit != null)
		{
			onExit.Invoke();
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000C573 File Offset: 0x0000A773
	public override void OnUpdate(float deltaTime)
	{
		Action<float> action = this.onUpdate;
		if (action != null)
		{
			action(deltaTime);
		}
	}

	// Token: 0x040000FE RID: 254
	[SerializeField]
	private UnityEvent _onEnter;

	// Token: 0x040000FF RID: 255
	[SerializeField]
	private UnityEvent _onExit;

	// Token: 0x04000100 RID: 256
	public Enum stateName;
}
