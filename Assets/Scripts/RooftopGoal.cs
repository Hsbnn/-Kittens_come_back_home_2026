using UnityEngine;

public sealed class RooftopGoal : MonoBehaviour
{
    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed || other.GetComponent<CatController>() == null)
        {
            return;
        }

        completed = true;
        ProceduralAudio.PlaySuccess(transform.position);
        GameUI.ShowMessage("ПОБЕДА! Наконец-то все дома...", 8f);
        StoryDirector.CompleteStory();
    }
}
