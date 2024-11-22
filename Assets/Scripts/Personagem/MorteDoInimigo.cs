using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorteDoInimigo : MonoBehaviour
{
    private StatusBase statusBase;
    public delegate void InimigoEliminadoHandler();
    public static event InimigoEliminadoHandler OnInimigoEliminado;
    
    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Mapinguari!");
        }
    }

    public bool IsDead(){
        if (statusBase.vidaAtual<=0){
            return true;
        }else{
            return false;
        }
    }

    public void DanoNoinimigo(float dano){
        statusBase.vidaAtual-=dano;
        if(statusBase.vidaAtual<=0){
            OnInimigoEliminado?.Invoke();
            Destroy(this.gameObject);
        }
    }
}
