using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using WhereFirefliesReturn.Player;
using WhereFirefliesReturn.Narrative;

public class cutscene_controller : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage overlay;
    [SerializeField] private GameObject playerController; // drag your movement script here
    [SerializeField] private OverlayEffects overlayEffects;

    void Start()
    {
        PlayCutscene(videoPlayer.clip);
    }
    public void PlayCutscene(VideoClip clip)
    {
        overlay.gameObject.SetActive(true);
        playerController.GetComponent<PlayerController>().CanMove = false;

        videoPlayer.clip = clip;
        videoPlayer.Prepare();
        videoPlayer.loopPointReached += OnCutsceneEnd;
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnCutsceneEnd;
        if (DialogueManager.Instance != null)
        DialogueManager.Instance.canInteract = false;
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnCutsceneEnd;

        overlayEffects.TriggerFadeOut(onComplete: () =>
        {
            overlay.gameObject.SetActive(false);
            playerController.GetComponent<PlayerController>().CanMove = true;
            overlayEffects.TriggerFadeIn();
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.canInteract = true;
        });
    }
}