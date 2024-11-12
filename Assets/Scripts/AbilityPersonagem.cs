using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;         // Nome da habilidade
    public float cooldown;             // Tempo de recarga da habilidade
    public float range;                // Alcance da habilidade (se aplic�vel)
    public Sprite icon;                // �cone da habilidade (para interface)

    // M�todo que ativa a habilidade
    public abstract void Activate(GameObject user);
}
