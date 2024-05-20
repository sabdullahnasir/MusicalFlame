using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform InteractableContainer;
    [SerializeField] Transform FlamesSpriteContainer;
    [Space]
    [SerializeField] Animator animator;
    [Space]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip levelCompleteAudio;
    [SerializeField] AudioClip levelFailedAudio;
    [Space]
    [SerializeField] List<SpiderController> SpiderControllers = new List<SpiderController>();
    [Space]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject GameOverPanel;
    [Space]
    List<Button> allFlameItems = new List<Button>();
    List<Image> allFlameImages = new List<Image>();
    int currentLevel;
    List<int> currentLevelSequence = new List<int>();
    int currentSequenceIndex;
    public int minDistractionCooldown, maxDistractionCooldown;

    private void Start()
    {
        for (int i = 0; i < InteractableContainer.childCount; i++)
        {
            int index = i;
            Button tempInteractable = InteractableContainer.GetChild(index).GetComponent<Button>();
            Image tempImage = FlamesSpriteContainer.GetChild(index).GetComponent<Image>();
            allFlameItems.Add(tempInteractable);
            allFlameImages.Add(tempImage);
        }

        for (int i = 0; i < allFlameItems.Count; i++)
        {
            int index = i;
            allFlameItems[i].onClick.AddListener(() => CheckCorrectSequence(index));
        }

        StartNextLevel();
    }

    public void StartNextLevel()
    {
        currentLevel++;
        levelText.text = currentLevel.ToString();
        currentSequenceIndex = 0;
        int nextSequence = Random.Range(0, allFlameItems.Count);
        if (currentLevelSequence.Count != 0)
        {
            while (nextSequence == currentLevelSequence.Last())
            {
                nextSequence = Random.Range(0, allFlameItems.Count);
            }
        }
        currentLevelSequence.Add(nextSequence);

        StartCoroutine(ShowButtonHighlight());
    }

    public void OnLevelComplete()
    {
        PlayLevelAudio(LevelResult.Complete);
        StartNextLevel();
    }

    public void OnLevelFailed()
    {
        PlayLevelAudio(LevelResult.Fail);

        GameOverPanel.SetActive(true);

        currentLevelSequence.RemoveRange(0, currentLevelSequence.Count);
        currentLevel = 0;
    }

    void CheckCorrectSequence(int index)
    {
        if (currentLevelSequence[currentSequenceIndex] == index)
        {
            animator.Play("Flame" + index);
            currentSequenceIndex++;

            if(currentSequenceIndex == currentLevelSequence.Count)
            {
                OnLevelComplete();
            }
        }
        else
        {
            OnLevelFailed();
        }
    }

    IEnumerator ResetButtons(int timer)
    {
        yield return new WaitForSeconds(timer);
    }

    IEnumerator ShowButtonHighlight()
    {
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < allFlameImages.Count; i++)
        {
            int index = i;
            allFlameImages[index].color = new Color(255, 255, 255, 255); //Enables All Flames
            allFlameItems[index].interactable = false; //Restricts interaction with Flames
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < allFlameImages.Count; i++)
        {
            int index = i;
            allFlameImages[index].color = new Color(255, 255, 255, 0); //Disables all Flames
        }

        yield return new WaitForSeconds(1.5f);

        //Shows Memory Sequence
        for (int i = 0; i < currentLevelSequence.Count; i++)
        {
            int index = currentLevelSequence[i];
            animator.Play("Flame" + index);
            allFlameItems[index].gameObject.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(1);
        }

        for (int i = 0; i < allFlameItems.Count; i++)
        {
            int index = i;
            allFlameItems[index].interactable = true; //Enables interaction with Flames
        }

        yield return null;
    }

    void PlayLevelAudio(LevelResult result)
    {
        audioSource.clip = (result == LevelResult.Complete) ? levelCompleteAudio : levelFailedAudio;
        audioSource.Play();
    }

    public void ScheduleDistraction()
    {
        Invoke(nameof(InvokeSpiderDistraction),
            Random.Range(minDistractionCooldown, maxDistractionCooldown));
    }

    void InvokeSpiderDistraction()
    {
        int randomNum = Random.Range(0, SpiderControllers.Count);
        SpiderControllers[randomNum].SetSpiderDistraction();
    }
}

public enum LevelResult
{
    Complete,
    Fail
}
