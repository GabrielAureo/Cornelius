using UnityEngine;

public abstract class Affectable : MonoBehaviour{
    public abstract void SetAffectable(bool enabled);
    /// <summary>
    /// Request that the effect may be dispeled by the effect.
    /// </summary>
    /// <returns>Returns true if the effect is dispelled.abstract False, if not.</returns>
    public abstract bool Dispel();
}