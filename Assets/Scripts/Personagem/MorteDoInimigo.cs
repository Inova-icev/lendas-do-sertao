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
        statusBase.vidaAtualEscudo = statusBase.vidaEscudoMaxima;
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Mapinguari!");
        }
      
    //statusBase.AtivarEscudo();
         
    
    }

    public bool IsDead(){
        if (statusBase.vidaAtual<=0){
            return true;
        }else{
            return false;
        }
    }

    public void DanoNoinimigo(float dano){
        if(statusBase.temEscudo ==false){
            statusBase.EscudoPai.SetActive(false);
            statusBase.barravidaPai.SetActive(true);
            
             statusBase.vidaAtual-=dano;
            statusBase.UpdateBarraVida();
        if(statusBase.vidaAtual<=0){
            OnInimigoEliminado?.Invoke();
            Destroy(this.gameObject);
        }

        }else{
            statusBase.barravidaPai.SetActive(false);
            statusBase.vidaAtualEscudo-=dano;
            if(statusBase.vidaAtualEscudo <= 0){
                statusBase.temEscudo = false;
            }
        }
    }

    public void Curando(float cura){
        statusBase.UpdateBarraVida();
        if(statusBase.vidaAtual>=statusBase.vidaEscudoMaxima){
        statusBase.vidaAtual+=0;
        }else{
            statusBase.vidaAtual+=cura;
        }

    }
}
