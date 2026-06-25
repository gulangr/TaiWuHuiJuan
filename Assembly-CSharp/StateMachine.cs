using System;
using System.Collections.Generic;
using FrameWork;

// Token: 0x0200003C RID: 60
public class StateMachine
{
	// Token: 0x17000045 RID: 69
	// (get) Token: 0x06000208 RID: 520 RVA: 0x0000C58C File Offset: 0x0000A78C
	public float currentStateTime
	{
		get
		{
			return (float)this._currentStateTime;
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000209 RID: 521 RVA: 0x0000C5A8 File Offset: 0x0000A7A8
	public double currentStateTimeDouble
	{
		get
		{
			return this._currentStateTime;
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000C5C0 File Offset: 0x0000A7C0
	protected virtual void OnStateChanged(BaseState lastState, BaseState currentState)
	{
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000C5C3 File Offset: 0x0000A7C3
	protected virtual void Init()
	{
		this.AllStates = new Dictionary<Enum, BaseState>();
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000C5D4 File Offset: 0x0000A7D4
	public BaseState GetCurrentState()
	{
		return this._currentState;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000C5EC File Offset: 0x0000A7EC
	public void TranslateState(Enum newStateName, ArgumentBox argsBox = null)
	{
		bool flag = this.AllStates == null;
		if (flag)
		{
			this.Init();
		}
		bool flag2 = !this.AllStates.ContainsKey(newStateName);
		if (flag2)
		{
			GLog.TagError("StateMachine", "No state " + ((newStateName != null) ? newStateName.ToString() : null), Array.Empty<object>());
			EasyPool.Free<ArgumentBox>(argsBox);
		}
		else
		{
			bool duringSetting = this._duringSetting;
			if (duringSetting)
			{
				throw new Exception("Shouldn't change state inside OnExit, or stateChanged event!");
			}
			this._duringSetting = true;
			BaseState currentState = this._currentState;
			if (currentState != null)
			{
				currentState.OnExit();
			}
			BaseState lastState = this._currentState;
			this._duringSetting = false;
			this._currentState = this.AllStates[newStateName];
			this._currentStateTime = 0.0;
			BaseState currentState2 = this._currentState;
			if (currentState2 != null)
			{
				currentState2.OnEnter(argsBox);
			}
			this.OnStateChanged(lastState, this._currentState);
			EasyPool.Free<ArgumentBox>(argsBox);
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000C6E0 File Offset: 0x0000A8E0
	protected void RegisterState(State state)
	{
		bool flag = state != null && !this.AllStates.ContainsKey(state.stateName);
		if (flag)
		{
			this.AllStates.Add(state.stateName, state);
		}
	}

	// Token: 0x04000102 RID: 258
	private BaseState _currentState;

	// Token: 0x04000103 RID: 259
	private double _currentStateTime;

	// Token: 0x04000104 RID: 260
	protected Dictionary<Enum, BaseState> AllStates;

	// Token: 0x04000105 RID: 261
	private bool _duringSetting = false;
}
