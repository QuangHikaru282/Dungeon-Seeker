using System.Collections;
using UnityEngine;

public class FrozenEffect : MonoBehaviour
{
    [Header("References")]
    public GameObject frozenObject;         // Child GameObject "Frozen" bên trong Player
    public Animator frozenAnimator;         // Animator bên trong Frozen
    public float freezeDuration = 3f;

    private static readonly int HashFrozenDestroyed = Animator.StringToHash("Frozen_Destroyed");

    private PlayerController controller;
    private Rigidbody2D rb;
    private Coroutine freezeCoroutine;
    private Animator playerAnimator;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        // Đảm bảo Frozen tắt lúc đầu
        if (frozenObject) frozenObject.SetActive(false);
    }

    public void ApplyFreeze()
    {
        // Nếu đang đóng băng thì reset lại timer
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(FreezeRoutine());
    }

    IEnumerator FreezeRoutine()
    {
        frozenObject.SetActive(true);

        // Tắt controller và đóng băng hoàn toàn
        controller.enabled = false;

        // Dừng Animator của Player
        playerAnimator.speed = 0f;

        yield return new WaitForSeconds(freezeDuration);

        // Chạy animation Frozen_Destroyed
        if (frozenAnimator) frozenAnimator.SetTrigger(HashFrozenDestroyed);
        float animLength = GetAnimationLength(frozenAnimator, "Frozen_Destroyed");
        yield return new WaitForSeconds(animLength);

        // Khôi phục tất cả
        frozenObject.SetActive(false);
        playerAnimator.speed = 1f;
        controller.enabled = true;

        freezeCoroutine = null;
    }

    float GetAnimationLength(Animator animator, string clipName)
    {
        if (!animator) return 0.5f;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0.5f;
    }
}