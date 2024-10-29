using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject minionCorpoACorpoAliadoPrefab; // Prefab dos minions corpo a corpo aliados
    public GameObject minionADistanciaAliadoPrefab; // Prefab dos minions à distância aliados
    public GameObject minionCorpoACorpoInimigoPrefab; // Prefab dos minions corpo a corpo inimigos
    public GameObject minionADistanciaInimigoPrefab; // Prefab dos minions à distância inimigos

    public Transform spawnPointAliados; // Ponto de spawn dos minions aliados
    public Transform spawnPointInimigos; // Ponto de spawn dos minions inimigos

    public Transform waypointAliados; // Waypoint para minions aliados
    public Transform waypointInimigos; // Waypoint para minions inimigos

    public float intervaloWaves = 10f; // Intervalo entre as waves

    private float countdown = 0f; // Temporizador para controle de spawn

    void Start()
    {
        countdown = intervaloWaves; // Inicializa o countdown com o intervalo definido
    }

    void Update()
    {
        // Reduz o temporizador a cada frame
        countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            SpawnWave(); // Spawna uma nova wave
            countdown = intervaloWaves; // Reseta o temporizador
        }
    }

    void SpawnWave()
    {
        // Spawna dois minions corpo a corpo aliados e inimigos
        for (int i = 0; i < 2; i++)
        {
            GameObject minionAliado = Instantiate(minionCorpoACorpoAliadoPrefab, spawnPointAliados.position, spawnPointAliados.rotation);
            minionAliado.GetComponent<Minion>().waypoint = waypointAliados;

            GameObject minionInimigo = Instantiate(minionCorpoACorpoInimigoPrefab, spawnPointInimigos.position, spawnPointInimigos.rotation);
            minionInimigo.GetComponent<Minion>().waypoint = waypointInimigos;
        }

        // Spawna dois minions à distância aliados e inimigos
        for (int i = 0; i < 2; i++)
        {
            GameObject minionAliado = Instantiate(minionADistanciaAliadoPrefab, spawnPointAliados.position, spawnPointAliados.rotation);
            minionAliado.GetComponent<MinionRanged>().waypoint = waypointAliados;

            GameObject minionInimigo = Instantiate(minionADistanciaInimigoPrefab, spawnPointInimigos.position, spawnPointInimigos.rotation);
            minionInimigo.GetComponent<MinionRanged>().waypoint = waypointInimigos;
        }

        Debug.Log("Nova wave spawnada.");
    }
}
