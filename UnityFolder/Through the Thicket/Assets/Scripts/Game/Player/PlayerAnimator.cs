using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Transform movementTransform;
    private Vector3 startPos;
    private Vector3 endPos;
    private float progress;
    private float duration;
    private float currentTime;
    public void WalkAnimation(Vector3 movement)
    {
        currentTime = 0;
        startPos = -movement;
        endPos = Vector3.zero;
        duration = 0.3f;
        progress = 0;
        StartCoroutine(WalkingAnimation());
    }
    private IEnumerator WalkingAnimation()
    {
        while (progress < 1) {
            currentTime += Time.deltaTime;
            progress = currentTime / duration;
            movementTransform.localPosition = Vector3.Lerp(startPos, endPos, progress);

            float height = Mathf.Abs(Mathf.Sin(progress * 0.5f * Mathf.PI * 2)) * 0.6f;
            movementTransform.localPosition = new Vector3(movementTransform.localPosition.x, height, movementTransform.localPosition.z);
            yield return null;
        }
        movementTransform.localPosition = endPos;
    }
}
