using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class CallbackCoroutine{
    Coroutine coroutine;
    UnityAction onFinish;
    UnityAction onStop;
    MonoBehaviour origin;
    public CallbackCoroutine(MonoBehaviour origin, IEnumerator routine, UnityAction onFinish, UnityAction onStop){
        this.origin = origin;
        this.onFinish = onFinish;
        this.onStop = onStop;
        origin.StartCoroutine(Start(routine));
    }
    IEnumerator Start(IEnumerator routine){
        yield return coroutine = origin.StartCoroutine(routine);
        onFinish();
    }

    public void Stop(){
        origin.StopCoroutine(coroutine);
        onStop();       
    }
}

public static class CallbackCoroutineExtension{
    public static CallbackCoroutine StartCallbackCoroutine(this MonoBehaviour origin, IEnumerator routine, UnityAction onFinish, UnityAction onStop){
        return new CallbackCoroutine(origin, routine, onFinish, onStop);
    }
}