using System.Collections;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private float rotationSpeed = 1f;

    [SerializeField] private float exposureDuration = 2f;
    [SerializeField] private Vector2 exposureThresholds;

    private void Start()
    {
        StartCoroutine(RotateSkybox());
        StartCoroutine(SkyboxExposure());
    }

    private IEnumerator RotateSkybox()
    {
        float duration = 0;

        while (true)
        {
            skyboxMaterial.SetFloat("_Rotation", duration * rotationSpeed);
            duration += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SkyboxExposure()
    {
        float nextExposure = Random.Range(exposureThresholds.x, exposureThresholds.y);
        float duration = 0f;

        while (skyboxMaterial.GetFloat("_Exposure") != nextExposure)
        {
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(skyboxMaterial.GetFloat("_Exposure"), nextExposure, duration / exposureDuration));
            duration += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(exposureDuration);

        StartCoroutine(SkyboxExposure());
    }
}