using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWindowCoroutines : MonoBehaviour {
	// This class only helps us start coroutines
	// so it's empty
}

class UICoroutine : IEnumerator 
{
	private bool stop;

	IEnumerator enumerator;
	MonoBehaviour behaviour;

	public readonly Coroutine coroutine;

	public UICoroutine(MonoBehaviour behaviour, IEnumerator enumerator)
	{
		this.behaviour = behaviour;
		this.enumerator = enumerator;
		this.coroutine = this.behaviour.StartCoroutine(this);
	}

	public object Current { get { return enumerator.Current; } }
	public bool MoveNext() { return !stop && enumerator.MoveNext(); }
	public void Reset() { enumerator.Reset(); }
	public void Stop() { stop = true; }
}