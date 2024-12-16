using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandacaruZone : MonoBehaviourPunCallbacks
{
    public float captureSpeed = 1.665f; // Velocidade de captura ajustada para 30 segundos (% por segundo)
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

        if (PhotonNetwork.IsMasterClient)
        {
            UpdateCaptureProgress();
            CheckForCapture();
        }
    }

    private void UpdateCaptureProgress()
    {
        // Verifica se a zona está contestada
        if (leftTeamInZone.Count > 0 && rightTeamInZone.Count > 0)
        {
            Debug.Log("Zona contestada! Progresso pausado.");
            return; // Pausa o progresso
        }

        // Atualiza progresso para o time Left
        if (leftTeamInZone.Count > 0 && rightTeamInZone.Count == 0)
        {
            teamLeftProgress += captureSpeed * Time.deltaTime;
            teamRightProgress = Mathf.Max(0, teamRightProgress - decaySpeed * Time.deltaTime);
        }
        // Atualiza progresso para o time Right
        else if (rightTeamInZone.Count > 0 && leftTeamInZone.Count == 0)
        {
            teamRightProgress += captureSpeed * Time.deltaTime;
            teamLeftProgress = Mathf.Max(0, teamLeftProgress - decaySpeed * Time.deltaTime);
        }
        // Decadência para ambos os times se ninguém estiver na zona
        else if (leftTeamInZone.Count == 0 && rightTeamInZone.Count == 0)
        {
            teamLeftProgress = Mathf.Max(0, teamLeftProgress - decaySpeed * Time.deltaTime);
            teamRightProgress = Mathf.Max(0, teamRightProgress - decaySpeed * Time.deltaTime);
        }
        teamLeftProgress = Mathf.Clamp(teamLeftProgress, 0f, 100f);
        teamRightProgress = Mathf.Clamp(teamRightProgress, 0f, 100f);

        // Sincroniza o progresso para todos os clientes
        photonView.RPC("SyncProgress", RpcTarget.All, teamLeftProgress, teamRightProgress);
    }


    private void CheckForCapture()
    {
        if (teamLeftProgress >= 100f)
        {
            isCaptured = true;
            Debug.Log("Time Left capturou o objetivo!");
            photonView.RPC("HandleCapture", RpcTarget.All, teamLeftTag); // Sincroniza a captura
        }
        else if (teamRightProgress >= 100f)
        {
            isCaptured = true;
            Debug.Log("Time Right capturou o objetivo!");
            photonView.RPC("HandleCapture", RpcTarget.All, teamRightTag); // Sincroniza a captura
        }
    }
    [PunRPC]
    private void SyncProgress(float leftProgress, float rightProgress)
    {
        teamLeftProgress = leftProgress;
        teamRightProgress = rightProgress;
    }

    [PunRPC]
    private void SyncPresence(bool isLeftTeam, string playerId, bool isEntering)
    {
        if (isEntering)
        {
            if (isLeftTeam)
                leftTeamInZone.Add(PhotonView.Find(int.Parse(playerId)).gameObject);
            else
                rightTeamInZone.Add(PhotonView.Find(int.Parse(playerId)).gameObject);
        }
        else
        {
            if (isLeftTeam)
                leftTeamInZone.Remove(PhotonView.Find(int.Parse(playerId)).gameObject);
            else
                rightTeamInZone.Remove(PhotonView.Find(int.Parse(playerId)).gameObject);
        }
    }
    [PunRPC]
    private void HandleCapture(string capturingTeamTag)
    {
        isCaptured = true;
        if (capturingTeamTag == teamLeftTag)
        {
            teamLeftProgress = 100f;
            teamRightProgress = 0f;
        }
        else if (capturingTeamTag == teamRightTag)
        {
            teamRightProgress = 100f;
            teamLeftProgress = 0f;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            GrantBuffToTeam(capturingTeamTag);
            StartCoroutine(ResetAfterCapture(resetTime));
        }
    }


    private void GrantBuffToTeam(string teamTag)
    {
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
        yield return new WaitForSeconds(duration);
        playerComponent.RemoveBuff();
        playerComponent.RemoveDamageBuff(damageMultiplier);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent != null)
        {
            bool isLeftTeam = other.CompareTag(teamLeftTag);
            photonView.RPC("SyncPresence", RpcTarget.All, isLeftTeam, other.GetComponent<PhotonView>().ViewID.ToString(), true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent != null)
        {
            bool isLeftTeam = other.CompareTag(teamLeftTag);
            photonView.RPC("SyncPresence", RpcTarget.All, isLeftTeam, other.GetComponent<PhotonView>().ViewID.ToString(), false);
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
    }
    public float GetTeamLeftProgress() => teamLeftProgress;

    public float GetTeamRightProgress() => teamRightProgress;
}
