using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;         // Nome da habilidade
    public float cooldown;             // Tempo de recarga da habilidade
    public float range;                // Alcance da habilidade (se aplicável)
    public Sprite icon;                // Ícone da habilidade (para interface)

    // Método que ativa a habilidade
    public abstract void Activate(GameObject user);
}
