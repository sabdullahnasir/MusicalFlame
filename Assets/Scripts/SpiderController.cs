using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderController : MonoBehaviour
{
    [SerializeField] int spiderIndex;
    [SerializeField] GameManager gameManager;
    [SerializeField] Animator animator;
    [SerializeField] bool hasAppeared;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(RemoveSpiderDistraction);
    }

    public void SetSpiderDistraction()
    {
        if (hasAppeared) return;

        hasAppeared = true;
        animator.Play("SpiderIn" + spiderIndex);
    }

    public void RemoveSpiderDistraction()
    {
        hasAppeared = false;
        animator.Play("SpiderOut" + spiderIndex);

        gameManager.ScheduleDistraction();
    }
}
