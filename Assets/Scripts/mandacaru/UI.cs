using UnityEngine;
using UnityEngine.UI;

public class CaptureUIManager : MonoBehaviour
{
    public Slider TeamLeftProgressBar; // Referência para a barra do Time Left
    public Slider TeamRightProgressBar; // Referência para a barra do Time Right

    private MandacaruZone mandacaruZone; // Referência ao script MandacaruZone

    void Start()
    {
        // Procura o objeto MandacaruZone na cena
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
            // Atualiza os sliders com o progresso atual do Mandacaru
            TeamLeftProgressBar.value = mandacaruZone.GetTeamLeftProgress() / 100f;
            TeamRightProgressBar.value = mandacaruZone.GetTeamRightProgress() / 100f;
        }
    }
}
