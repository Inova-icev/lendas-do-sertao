using UnityEngine;

public abstract class Passive : ScriptableObject
{
    // Nome da passiva (opcional, útil para identificar passivas no editor)
    public string passiveName;

    // Método para aplicar o efeito passivo ao personagem
    public abstract void ApplyPassive(GameObject character);
}
