using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
     [SerializeField] private string SelecaoPersonagem;
     [SerializeField] private GameObject painelMenuInicial;
     [SerializeField] private GameObject painelOpcoes;
    public void Jogar(){
        SceneManager.LoadScene(SelecaoPersonagem);


    }

    public void AbrirOpcoes(){
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);

    }

    public void FecharOpcoes(){
        painelMenuInicial.SetActive(true);
        painelOpcoes.SetActive(false);

    }
}
