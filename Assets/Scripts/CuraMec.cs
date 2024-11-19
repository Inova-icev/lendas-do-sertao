using UnityEngine;

public class CuraMec : MonoBehaviour
{
    public bool  curando;

    public Transform curapoint;
    public float ataqueRange = 1f;
    private StatusBase statusBase;

    public LayerMask aliado;



    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }
    }

    void Update()
    {
       curando = Input.GetKeyDown(KeyCode.J);
        if (curando)
        {
            CuraAliado();
        }
    }

    public void CuraAliado()
    {
       // animator.SetTrigger("ataque");

       // OnAtaqueRealizado?.Invoke();

        Collider2D[] curaAliado = Physics2D.OverlapCircleAll(curapoint.position, ataqueRange, aliado);

        foreach (Collider2D cura in curaAliado)
        {
            cura.GetComponent<MorteDoInimigo>().Curando(statusBase.danoMagico*0.5f);
            Debug.Log("O aliado recebeu " + statusBase.danoMagico*0.5f+ " de cura");
           // Debug.Log("O aliado vida: " + statusBase.vidaAtual );

        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(curapoint.position, ataqueRange);
    }
}
