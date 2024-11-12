using UnityEngine;

public abstract class Passive : ScriptableObject
{
    // Nome da passiva (opcional, �til para identificar passivas no editor)
    public string passiveName;

    // M�todo para aplicar o efeito passivo ao personagem
    public abstract void ApplyPassive(GameObject character);
}
