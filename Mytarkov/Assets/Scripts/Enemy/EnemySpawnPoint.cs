using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed = 4;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    //활성화 될때
    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect");
    }
    //비활성화 될때
    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect");
    }

    private IEnumerator OnFadeEffect()
    {
        while(true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
            //PingPong 0 ~ 1값이 왔다 갔다함
            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
