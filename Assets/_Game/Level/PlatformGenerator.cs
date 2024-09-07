using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab; // Префаб платформы
    public int initialPlatformCount = 5; // Количество начальных платформ
    public float platformLength = 50f; // Длина платформы по оси Z
    public Transform player; // Ссылка на игрока
    public float spawnDistance = 20f; // Расстояние до игрока, при котором платформы перемещаются вперед

    private List<GameObject> platforms = new List<GameObject>(); // Список платформ

    void Start()
    {
        // Создаем начальные платформы
        for (int i = 0; i < initialPlatformCount; i++)
        {
            CreatePlatform(i * platformLength);
        }
    }

    public void ClearPlatforms()
    {
        foreach (var platform in platforms)
        {
            Destroy(platform);
        }

        platforms = new List<GameObject>();
        Start();
    }

    void Update()
    {
        // Проверяем, нужно ли переместить платформы вперед
        if (player.position.z > platforms[0].transform.position.z + spawnDistance)
        {
            RepositionPlatforms();
        }
    }

    private void CreatePlatform(float zPosition)
    {
        GameObject newPlatform = Instantiate(platformPrefab, new Vector3(0f, 0.21f, zPosition), Quaternion.identity);
        platforms.Add(newPlatform);
    }

    private void RepositionPlatforms()
    {
        // Перемещаем платформы вперед
        GameObject firstPlatform = platforms[0];
        platforms.RemoveAt(0);

        // Перемещаем первую платформу вперед и добавляем её в конец списка
        float newZPosition = platforms[platforms.Count - 1].transform.position.z + platformLength;
        firstPlatform.transform.position = new Vector3(0f, 0.21f, newZPosition);
        platforms.Add(firstPlatform);
    }
}