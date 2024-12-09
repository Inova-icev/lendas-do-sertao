using TMPro; // Certifique-se de incluir esta biblioteca para usar TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class CaptureUIManager : MonoBehaviour
{
    public Slider teamLeftProgressBar;  // Referência ao Slider do time Left
    public Slider teamRightProgressBar; // Referência ao Slider do time Right
    public TMP_Text teamLeftPercentageText; // Referência ao texto da porcentagem do time Left
    public TMP_Text teamRightPercentageText; // Referência ao texto da porcentagem do time Right

    private MandacaruZone mandacaruZone; // Referência ao script MandacaruZone

    void Start()
    {
        // Procura o MandacaruZone na cena
        mandacaruZone = FindObjectOfType<MandacaruZone>();

        if (mandacaruZone == null)
        {
            Debug.LogError("MandacaruZone não foi encontrado na cena!");
        }
    }

    void Update()
    {
        if (mandacaruZone != null)
        {
            // Atualiza os sliders com base no progresso atual
            float leftProgress = mandacaruZone.GetTeamLeftProgress();
            float rightProgress = mandacaruZone.GetTeamRightProgress();

            teamLeftProgressBar.value = leftProgress / 100f;
            teamRightProgressBar.value = rightProgress / 100f;

            // Atualiza os textos com a porcentagem
            teamLeftPercentageText.text = Mathf.FloorToInt(leftProgress) + "%";
            teamRightPercentageText.text = Mathf.FloorToInt(rightProgress) + "%";
        }
    }
}