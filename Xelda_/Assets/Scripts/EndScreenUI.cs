using UnityEngine;

public class EndScreenUI : MonoBehaviour
{
    public EntityHealth playerHealth;
    public EntityHealth bossHealth;

    public GameObject winPanel;
    public GameObject losePanel;

    public float popSpeed = 8.0f;

    private bool battleEnded = false;
    private Transform activePanelTransform;

    void Start()
    {
        if (winPanel != null)
        {
            winPanel.transform.localScale = Vector3.zero;
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.transform.localScale = Vector3.zero;
            losePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!battleEnded)
        {
            if (bossHealth != null && bossHealth.currentHealth <= 0)
            {
                ShowResult(winPanel);
            }
            else if (playerHealth != null && playerHealth.currentHealth <= 0)
            {
                ShowResult(losePanel);
            }
        }
        else if (activePanelTransform != null)
        {
            activePanelTransform.localScale = Vector3.Lerp(
                activePanelTransform.localScale,
                Vector3.one,
                Time.unscaledDeltaTime * popSpeed
            );
        }
    }

    private void ShowResult(GameObject panel)
    {
        battleEnded = true;
        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            activePanelTransform = panel.transform;
        }
    }
}