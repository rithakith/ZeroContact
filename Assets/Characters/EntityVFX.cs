using System.Collections;
using UnityEngine;

public class EntityVFX : MonoBehaviour
{
    private Material originalMat;
    private SpriteRenderer sr;

    [SerializeField] private Material onDamageVFXMat;
    [SerializeField] private float damageVFXDuration = 0.15f;
    private Coroutine damageVFXCoroutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void TriggerOnDamageVFX()
    {
        if (damageVFXCoroutine != null)
        {
            StopCoroutine(damageVFXCoroutine);
        }
        damageVFXCoroutine = StartCoroutine(onDamageVFXCo());
    }

    private IEnumerator onDamageVFXCo()
    {
        sr.material = onDamageVFXMat;
        yield return new WaitForSeconds(damageVFXDuration);
        sr.material = originalMat;
    }
}
