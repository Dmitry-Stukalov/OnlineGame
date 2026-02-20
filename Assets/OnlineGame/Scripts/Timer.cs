using System;
using UnityEngine;

public class Timer
{
	public float MaxTime { get; private set; }
	public float CurrentTime { get; private set; }
	public bool TimerIsEnd { get; private set; }
	public bool TimerIsOnStart { get; private set; }
	public bool Pause { get; private set; }

	public event Action OnTick;
	public event Action OnTimerEnd;
	public event Action OnTimerStart;
	public event Action OnTimerReverse;

	public Timer(float maxtime, float startvalue = 0, bool pause = false)
	{
		MaxTime = maxtime;
		CurrentTime = startvalue;
		Pause = pause;

		if (CurrentTime >= MaxTime) TimerIsEnd = true;
	}

	public void ResetTimer(bool pause)
	{
		TimerIsEnd = false;
		CurrentTime = 0;
		Pause = pause;
	}

	public void SetMaxTime(float maxtime)
	{
		if (maxtime > 0) MaxTime = maxtime;
		else MaxTime = 0;
		CurrentTimeToMax();
	}

	public void SetMaxTimeAndReset(float maxtime)
	{
		TimerIsEnd = false;
		CurrentTime = 0;
		MaxTime = maxtime;
	}

	public void CurrentTimeToMax()
	{
		CurrentTime = MaxTime;
	}


	public void SetPause() => Pause = true;

	public void Continue() => Pause = false;

	public void Tick(float time)
	{
		if (TimerIsEnd || Pause) return;

		UpdateTimer(time);
		OnTick?.Invoke();
	}

	public void ReverseTick(float time)
	{
		if (Pause) return;

		ReverseUpdateTimer(time);
		OnTick?.Invoke();
		OnTimerReverse?.Invoke();
	}

	public void UpdateTimer(float time)
	{
		CurrentTime += time;
		CurrentTime = Mathf.Clamp(CurrentTime, 0, MaxTime);

		if (CurrentTime >= MaxTime)
		{
			TimerIsEnd = true;
			OnTimerEnd?.Invoke();
		}
	}

	public void ReverseUpdateTimer(float time)
	{
		CurrentTime -= time;
		CurrentTime = Mathf.Clamp(CurrentTime, 0, MaxTime);

		if (CurrentTime < MaxTime)
		{
			TimerIsEnd = false;
		}

		if (CurrentTime <= 0)
		{
			TimerIsOnStart = true;
			OnTimerStart?.Invoke();
		}
	}
}
