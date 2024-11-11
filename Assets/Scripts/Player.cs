using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;
    public int gold = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            anim.SetBool("Correndo", true);
        }

        if(Input.GetKeyDown(KeyCode.S)){
            anim.SetBool("Correndo", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)){
            anim.SetBool("Atacando", true);
        }

        if(Input.GetKeyUp(KeyCode.Mouse0)){
            anim.SetBool("Atacando", false);
        }
    }

    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Jogador ganhou {amount} de ouro. Total: {gold}");
    }
}
