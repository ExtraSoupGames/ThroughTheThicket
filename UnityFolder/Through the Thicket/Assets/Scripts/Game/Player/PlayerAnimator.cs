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
            yield return null;
        }
        movementTransform.localPosition = endPos;
    }
}
