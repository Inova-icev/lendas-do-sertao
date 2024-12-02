using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandacaruZone : MonoBehaviourPunCallbacks
{
    public float captureSpeed = 10f; // Velocidade de captura (% por segundo)
    public string teamLeftTag = "Left";
    public string teamRightTag = "Right";

    private float teamLeftProgress = 0f; // Progresso do time Left
    private float teamRightProgress = 0f; // Progresso do time Right
    private bool isCaptured = false; // Se o objetivo foi capturado
    private HashSet<GameObject> leftTeamInZone = new HashSet<GameObject>();
    private HashSet<GameObject> rightTeamInZone = new HashSet<GameObject>();

    public float buffMultiplier = 1.5f; // Multiplicador do buff do Mandacaru
    public float damageMultiplier = 2f; // Multiplicador do dano do buff

    void Update()
    {
        if (isCaptured) return;

        // Atualiza o progresso de captura
        UpdateCaptureProgress();
        CheckForCapture();
    }

    private void UpdateCaptureProgress()
    {
        // Verifica se apenas o Time Left está na zona
        if (leftTeamInZone.Count > 0 && rightTeamInZone.Count == 0)
        {
            // Itera pelos objetos no HashSet para verificar a tag
            foreach (GameObject player in leftTeamInZone)
            {
                if (player.CompareTag("Left")) // Confirma que é do time Left
                {
                    teamLeftProgress += captureSpeed * Time.deltaTime;
                    teamRightProgress = Mathf.Max(0, teamRightProgress - captureSpeed * Time.deltaTime);
                    break; // Sai do loop após encontrar um jogador válido
                }
            }
        }
        // Verifica se apenas o Time Right está na zona
        else if (rightTeamInZone.Count > 0 && leftTeamInZone.Count == 0)
        {
            foreach (GameObject player in rightTeamInZone)
            {
                if (player.CompareTag("Right")) // Confirma que é do time Right
                {
                    teamRightProgress += captureSpeed * Time.deltaTime;
                    teamLeftProgress = Mathf.Max(0, teamLeftProgress - captureSpeed * Time.deltaTime);
                    break; // Sai do loop após encontrar um jogador válido
                }
            }
        }

        // Limita o progresso ao intervalo [0, 100]
        teamLeftProgress = Mathf.Clamp(teamLeftProgress, 0f, 100f);
        teamRightProgress = Mathf.Clamp(teamRightProgress, 0f, 100f);

        // Atualiza visualmente o progresso (ou exibe no console)
        Debug.Log($"Progresso Left: {teamLeftProgress}% | Progresso Right: {teamRightProgress}%");
    }


    private void CheckForCapture()
    {
        if (teamLeftProgress >= 100f)
        {
            isCaptured = true;
            Debug.Log("Time Left capturou o objetivo!");
            GrantBuffToTeam(teamLeftTag);
        }
        else if (teamRightProgress >= 100f)
        {
            isCaptured = true;
            Debug.Log("Time Right capturou o objetivo!");
            GrantBuffToTeam(teamRightTag);
        }
    }


    private void GrantBuffToTeam(string teamTag)
    {
        Debug.Log($"Time {teamTag} capturou o Mandacaru e recebeu os buffs!");

        // Apenas o Master Client aplica os buffs
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag(teamTag))
            {
                Player playerComponent = player.GetComponent<Player>();
                if (playerComponent != null)
                {
                    // Aplica os buffs de velocidade e dano
                    playerComponent.ApplyBuff(buffMultiplier);
                    playerComponent.ApplyDamageBuff(damageMultiplier);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(teamLeftTag))
        {
            leftTeamInZone.Add(other.gameObject);
            Debug.Log($"{other.gameObject.name} entrou na zona. Time: Left");
        }
        else if (other.CompareTag(teamRightTag))
        {
            rightTeamInZone.Add(other.gameObject);
            Debug.Log($"{other.gameObject.name} entrou na zona. Time: Right");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(teamLeftTag))
        {
            leftTeamInZone.Remove(other.gameObject);
            Debug.Log($"{other.gameObject.name} saiu da zona. Time: Left");
        }
        else if (other.CompareTag(teamRightTag))
        {
            rightTeamInZone.Remove(other.gameObject);
            Debug.Log($"{other.gameObject.name} saiu da zona. Time: Right");
        }
    }
}
