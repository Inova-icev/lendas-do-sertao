using System.IO;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Garante que o logger persista entre cenas

        // Define o caminho do arquivo para salvar os logs
        logFilePath = Path.Combine(Application.persistentDataPath, "game_logs.txt");

        // Inicializa o arquivo
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath); // Limpa o arquivo antigo ao iniciar
        }

        Application.logMessageReceived += LogToFileHandler; // Adiciona o listener de log
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= LogToFileHandler; // Remove o listener
    }

    private void LogToFileHandler(string condition, string stackTrace, LogType type)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"[{System.DateTime.Now}] {type}: {condition}");
            if (type == LogType.Error || type == LogType.Exception)
            {
                writer.WriteLine(stackTrace); // Adiciona a stack trace para erros
            }
        }
    }
}
