using UnityEngine;

public sealed class StoryTrigger : MonoBehaviour
{
    [SerializeField] private string objective;
    [SerializeField] private string story;
    [SerializeField] private float messageSeconds = 6f;
    [SerializeField] private bool triggerOnce = true;

    private bool triggered;

    public void Configure(string objectiveText, string storyText, float seconds = 6f)
    {
        objective = objectiveText;
        story = storyText;
        messageSeconds = seconds;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && triggerOnce)
        {
            return;
        }

        if (other.GetComponent<CatController>() == null)
        {
            return;
        }

        triggered = true;
        StoryDirector.ShowHint(objective, story, messageSeconds);
    }
}
