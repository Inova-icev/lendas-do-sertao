using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandacaruZone : MonoBehaviourPunCallbacks
{
    public float captureSpeed = 3.33f; // Velocidade de captura ajustada para 30 segundos (% por segundo)
    public float decaySpeed = 1f; // Velocidade de decadência (% por segundo)
    public string teamLeftTag = "Left";
    public string teamRightTag = "Right";

    private float teamLeftProgress = 0f; // Progresso do time Left
    private float teamRightProgress = 0f; // Progresso do time Right
    private int lastLeftProgressLog = -1; // Último progresso inteiro registrado para o time Left
    private int lastRightProgressLog = -1; // Último progresso inteiro registrado para o time Right
    private bool isCaptured = false; // Se o objetivo foi capturado
    private HashSet<GameObject> leftTeamInZone = new HashSet<GameObject>();
    private HashSet<GameObject> rightTeamInZone = new HashSet<GameObject>();

    public float buffMultiplier = 1.5f; // Multiplicador do buff do Mandacaru
    public float damageMultiplier = 2f; // Multiplicador do dano do buff
    public float buffDuration = 30f; // Duração do buff em segundos
    public float resetTime = 120f; // Tempo de reset do Mandacaru


    void Update()
    {
        if (isCaptured) return;

        // Atualiza o progresso de captura
        UpdateCaptureProgress();
        CheckForCapture();
    }

    private void UpdateCaptureProgress()
    {
        // Verifica se a zona está contestada
        if (leftTeamInZone.Count > 0 && rightTeamInZone.Count > 0)
        {
            Debug.Log("Zona contestada! Progresso pausado.");
            return; // Pausa o progresso
        }

        // Verifica se apenas o Time Left está na zona
        if (leftTeamInZone.Count > 0 && rightTeamInZone.Count == 0)
        {
            foreach (GameObject player in leftTeamInZone)
            {
                if (player.CompareTag("Left"))
                {
                    teamLeftProgress += captureSpeed * Time.deltaTime;
                    teamRightProgress = Mathf.Max(0, teamRightProgress - decaySpeed * Time.deltaTime);
                    break;
                }
            }
        }
        // Verifica se apenas o Time Right está na zona
        else if (rightTeamInZone.Count > 0 && leftTeamInZone.Count == 0)
        {
            foreach (GameObject player in rightTeamInZone)
            {
                if (player.CompareTag("Right"))
                {
                    teamRightProgress += captureSpeed * Time.deltaTime;
                    teamLeftProgress = Mathf.Max(0, teamLeftProgress - decaySpeed * Time.deltaTime);
                    break;
                }
            }
        }
        // Decadência para ambos os times se ninguém estiver na zona
        else if (leftTeamInZone.Count == 0 && rightTeamInZone.Count == 0)
        {
            teamLeftProgress = Mathf.Max(0, teamLeftProgress - decaySpeed * Time.deltaTime);
            teamRightProgress = Mathf.Max(0, teamRightProgress - decaySpeed * Time.deltaTime);
        }

        // Limita o progresso ao intervalo [0, 100]
        teamLeftProgress = Mathf.Clamp(teamLeftProgress, 0f, 100f);
        teamRightProgress = Mathf.Clamp(teamRightProgress, 0f, 100f);

        // Verifica e registra o progresso inteiro
        int currentLeftProgress = Mathf.FloorToInt(teamLeftProgress);
        int currentRightProgress = Mathf.FloorToInt(teamRightProgress);

        if (currentLeftProgress != lastLeftProgressLog)
        {
            Debug.Log($"Progresso Left: {currentLeftProgress}%");
            lastLeftProgressLog = currentLeftProgress;
        }

        if (currentRightProgress != lastRightProgressLog)
        {
            Debug.Log($"Progresso Right: {currentRightProgress}%");
            lastRightProgressLog = currentRightProgress;
        }
    }

    private void CheckForCapture()
    {
        if (teamLeftProgress >= 100f)
        {
            isCaptured = true;
            Debug.Log("Time Left capturou o objetivo!");
            GrantBuffToTeam(teamLeftTag);
            StartCoroutine(ResetAfterCapture(resetTime)); // Usando a variável resetTime
        }
        else if (teamRightProgress >= 100f)
        {
            isCaptured = true;
            Debug.Log("Time Right capturou o objetivo!");
            GrantBuffToTeam(teamRightTag);
            StartCoroutine(ResetAfterCapture(resetTime)); // Usando a variável resetTime
        }
    }

    private void GrantBuffToTeam(string teamTag)
    {
        Debug.Log($"Time {teamTag} capturou o Mandacaru e recebeu os buffs!");

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag(teamTag))
            {
                Player playerComponent = player.GetComponent<Player>();
                if (playerComponent != null)
                {
                    playerComponent.ApplyBuff(buffMultiplier);
                    playerComponent.ApplyDamageBuff(damageMultiplier);

                    StartCoroutine(RemoveBuffAfterDuration(playerComponent, buffDuration));
                }
            }
        }
    }

    private IEnumerator RemoveBuffAfterDuration(Player playerComponent, float duration)
    {
        Debug.Log($"Buff ativo no jogador {playerComponent.name} por {duration} segundos.");
        yield return new WaitForSeconds(duration);

        playerComponent.RemoveBuff();
        playerComponent.RemoveDamageBuff(damageMultiplier);

        Debug.Log($"Buffs removidos do jogador {playerComponent.name} após {duration} segundos.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent != null)
        {
            if (other.CompareTag(teamLeftTag) && !leftTeamInZone.Contains(other.gameObject))
            {
                leftTeamInZone.Add(other.gameObject);
                Debug.Log($"{other.gameObject.name} entrou na zona. Time: Left");
            }
            else if (other.CompareTag(teamRightTag) && !rightTeamInZone.Contains(other.gameObject))
            {
                rightTeamInZone.Add(other.gameObject);
                Debug.Log($"{other.gameObject.name} entrou na zona. Time: Right");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent != null)
        {
            if (other.CompareTag(teamLeftTag) && leftTeamInZone.Contains(other.gameObject))
            {
                leftTeamInZone.Remove(other.gameObject);
                Debug.Log($"{other.gameObject.name} saiu da zona. Time: Left");
            }
            else if (other.CompareTag(teamRightTag) && rightTeamInZone.Contains(other.gameObject))
            {
                rightTeamInZone.Remove(other.gameObject);
                Debug.Log($"{other.gameObject.name} saiu da zona. Time: Right");
            }
        }
    }

    private IEnumerator ResetAfterCapture(float delay)
    {
        yield return new WaitForSeconds(delay);
        teamLeftProgress = 0f;
        teamRightProgress = 0f;

        isCaptured = false;

        leftTeamInZone.Clear();
        rightTeamInZone.Clear();

        Debug.Log("Progresso de captura resetado. O objetivo pode ser capturado novamente.");
    }
    public float GetTeamLeftProgress()
    {
        return teamLeftProgress;
    }

    public float GetTeamRightProgress()
    {
        return teamRightProgress;
    }
    [PunRPC]
    private void SyncProgress(float leftProgress, float rightProgress)
    {
        teamLeftProgress = leftProgress;
        teamRightProgress = rightProgress;
    }   
}
