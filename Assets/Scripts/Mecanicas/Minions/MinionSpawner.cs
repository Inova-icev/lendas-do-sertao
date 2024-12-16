using System.Collections;
using UnityEngine;
using Photon.Pun;

public class MinionSpawner : MonoBehaviour
{
    public GameObject minionPrefab; // Prefab do minion
    public float spawnInterval = 30f; // Intervalo entre waves (30 segundos)
    public Transform spawnPoint; // Ponto de spawn dos minions
    public int minionsPerWave = 3; // Número de minions por wave
    public string spawnTag; // Tag que este spawner aplica aos minions (ex: "Left" ou "Right")
    public string enemyTag; // Tag dos inimigos que os minions deste spawner devem atacar
    public float minionSpacing = 1f; // Raio de verificação para evitar sobreposição

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnWave());
        }
    }

    public IEnumerator SpawnWave()
    {
        while (true)
        {
            int minionsSpawned = 0;

            while (minionsSpawned < minionsPerWave)
            {
                // Verifica se o ponto de spawn está livre
                if (!IsSpawnPointOccupied())
                {
                    SpawnMinion();
                    minionsSpawned++;
                    yield return new WaitForSeconds(1.0f); // Espera 1 segundo antes de tentar gerar o próximo minion
                }
                else
                {
                    // Se o ponto de spawn estiver ocupado, espera um pouco antes de tentar novamente
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // Espera o intervalo completo antes de iniciar a próxima wave
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void SpawnMinion()
    {
        // Instancia o minion na posição de spawn com a rotação padrão
        GameObject minion = PhotonNetwork.Instantiate(minionPrefab.name, spawnPoint.position, Quaternion.identity);
        Minions minionScript = minion.GetComponent<Minions>();

        if (minionScript != null)
        {
            minionScript.enemyTag = enemyTag; // Define o inimigo diretamente no spawner

            // Define a tag do minion com base na configuração do spawner
            minion.tag = spawnTag;
        }
    }

    bool IsSpawnPointOccupied()
    {
        // Verifica se há algum objeto com o layer dos minions (ou qualquer outro filtro) na área do spawn
        Collider2D overlap = Physics2D.OverlapCircle(spawnPoint.position, minionSpacing, LayerMask.GetMask("Minion"));
        return overlap != null; // Retorna verdadeiro se houver colisão, ou falso se estiver livre
    }

    void OnDrawGizmosSelected()
    {
        // Desenha o raio de verificação para ajudar a visualizar o espaçamento dos minions
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPoint.position, minionSpacing);
    }
}
