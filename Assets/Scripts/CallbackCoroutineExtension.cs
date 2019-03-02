using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public static class CallbackCoroutineExtension{
    public static CallbackCoroutine StartCallbackCoroutine(this MonoBehaviour origin, IEnumerator routine, UnityAction onFinish, UnityAction onStop){
        return new CallbackCoroutine(origin, routine, onFinish, onStop);
    }
}