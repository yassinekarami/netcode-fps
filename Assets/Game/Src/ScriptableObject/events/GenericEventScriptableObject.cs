using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "GenericEventScriptableObject", menuName = "Scriptable Objects/GenericEventScriptableObject")]
public class GenericEventScriptableObject<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T parameter)
    {
        if (OnEventRaised == null)
            return;

        OnEventRaised.Invoke(parameter);
    }
}
