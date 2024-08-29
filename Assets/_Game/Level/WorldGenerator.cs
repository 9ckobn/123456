using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Chunk[] startChunks; // Начальные чанки
    public Chunk[] singleChunks; // Односоставные чанки
    public List<CompositeChunk> compositeChunks = new List<CompositeChunk>(); // Составные чанки

    public Transform player; // Ссылка на игрока
    public float minSpawnDistance = 50f; // Минимальное расстояние до следующего чанка

    private List<GameObject> spawnedChunks = new List<GameObject>(); // Список созданных чанков
    private Vector3 nextSpawnPosition; // Позиция для следующего чанка
    private CompositeChunk currentComposite; // Текущий составной чанк
    private int currentCompositeIndex = -1; // Индекс текущего чанка в составном
    private float totalSizeZ = 0f; // Общий размер по оси Z
    private bool isCreatingComposite = false; // Флаг для отслеживания создания составного чанка
    private Chunk lastSingleChunk; // Последний созданный одиночный чанк
    private int repeatCount = 0; // Количество повторений текущего одиночного чанка

    [SerializeField] private PlatformGenerator platformGenerator;

    [SerializeField] private GameObject startCoins;
    [SerializeField] private Transform coinsRoot;

    private GameObject currentCoinsInstance = null;

    void Start()
    {
        // Начальная генерация уровня
        foreach (var chunk in startChunks)
        {
            SpawnChunk(chunk);
        }
        
        ClearStartCoins();
    }

    public void ClearLevel()
    {
        foreach (var chunk in spawnedChunks)
        {
            Destroy(chunk);
        }

        spawnedChunks = new List<GameObject>();
        platformGenerator.ClearPlatforms();
        nextSpawnPosition = Vector3.zero;
        Start();
        
        ClearStartCoins();
    }

    private void ClearStartCoins()
    {
        if (currentCoinsInstance != null)
            Destroy(currentCoinsInstance);

        currentCoinsInstance = Instantiate(startCoins, coinsRoot);
    }

    void Update()
    {
        // Генерация нового чанка, если игрок приближается к следующему чанку
        float distanceToPlayer = Vector3.Distance(player.position, nextSpawnPosition);

        if (distanceToPlayer < minSpawnDistance)
        {
            if (isCreatingComposite)
            {
                CreateCompositeChunk();
            }
            else
            {
                // Выбираем случайный одиночный чанк
                CreateSingleChunk();
            }
        }

        // Удаление старых чанков
        if (spawnedChunks.Count > 0)
        {
            float distanceFromFirstChunk = Vector3.Distance(player.position, spawnedChunks[0].transform.position);
            if (distanceFromFirstChunk > minSpawnDistance + totalSizeZ)
            {
                Destroy(spawnedChunks[0]);
                spawnedChunks.RemoveAt(0);
            }
        }
    }

    private void CreateSingleChunk()
    {
        Chunk newSingleChunk;
        do
        {
            newSingleChunk = SelectChunkWithChance(singleChunks);
        } while (newSingleChunk.Equals(lastSingleChunk) &&
                 (!newSingleChunk.canRepeat && repeatCount >= newSingleChunk.maxRepeatCount));

        SpawnChunk(newSingleChunk);

        // Обновляем состояние повторений
        if (newSingleChunk.Equals(lastSingleChunk))
        {
            repeatCount++;
        }
        else
        {
            lastSingleChunk = newSingleChunk;
            repeatCount = 1; // Сброс счетчика повторений
        }

        // Проверяем, следует ли создать новый составной чанк
        if (Random.value < 0.5f) // Можно настроить шанс на создание составного чанка
        {
            currentComposite = compositeChunks[Random.Range(0, compositeChunks.Count)];
            currentCompositeIndex = 0;
            isCreatingComposite = true;
        }
    }

    private void CreateCompositeChunk()
    {
        if (currentComposite != null && currentCompositeIndex >= 0 &&
            currentCompositeIndex < currentComposite.chunks.Length)
        {
            // Логика для повторения составных чанков
            Chunk currentChunk = currentComposite.chunks[currentCompositeIndex];
            if (currentCompositeIndex != 0 && currentCompositeIndex != currentComposite.chunks.Length - 1)
            {
                // Если текущий чанк может повторяться
                if (currentChunk.canRepeat)
                {
                    int repeatTimes = Random.Range(1, currentChunk.maxRepeatCount + 1);
                    for (int i = 0; i < repeatTimes; i++)
                    {
                        SpawnChunk(currentChunk);
                    }

                    // После создания повторяющихся чанков, переходим к следующему чанк
                    currentCompositeIndex++;
                }
                else
                {
                    // Если не может повторяться, создаем один раз
                    SpawnChunk(currentChunk);
                    currentCompositeIndex++;
                }
            }
            else
            {
                // Создаем первый или последний чанк составного уровня
                SpawnChunk(currentChunk);
                currentCompositeIndex++;
            }

            // Если это был последний чанк составного ассета, сбрасываем текущий составной чанк
            if (currentCompositeIndex >= currentComposite.chunks.Length)
            {
                currentComposite = null;
                currentCompositeIndex = -1;
                isCreatingComposite = false;
            }
        }
    }

    private Chunk SelectChunkWithChance(Chunk[] chunks)
    {
        // Выбираем чанк на основе вероятности
        float totalChance = 0f;
        foreach (var chunk in chunks)
        {
            totalChance += chunk.spawnChance;
        }

        float randomValue = Random.value * totalChance;
        float cumulativeChance = 0f;

        foreach (var chunk in chunks)
        {
            cumulativeChance += chunk.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return chunk;
            }
        }

        // По умолчанию возвращаем первый чанк, если что-то пошло не так
        return chunks[0];
    }

    void SpawnChunk(Chunk chunk)
    {
        // Создаем новый чанк уровня
        GameObject newChunk = Instantiate(chunk.prefab, nextSpawnPosition, Quaternion.identity);
        spawnedChunks.Add(newChunk);

        // Обновляем позицию для следующего чанка
        nextSpawnPosition.z += chunk.sizeZ; // Смещение по оси Z
        totalSizeZ += chunk.sizeZ; // Обновляем общий размер по оси Z
    }
}