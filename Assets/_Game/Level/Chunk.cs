using UnityEngine;

[System.Serializable]
public struct Chunk
{
    public GameObject prefab;
    public float sizeZ;
    public bool canRepeat; // Флаг для возможности повторения
    public int maxRepeatCount; // Максимальное количество повторений
    [Range(0f, 1f)] public float spawnChance; // Вероятность появления чанка

    // Конструктор
    public Chunk(GameObject prefab, float sizeZ, bool canRepeat, int maxRepeatCount, float spawnChance)
    {
        this.prefab = prefab;
        this.sizeZ = sizeZ;
        this.canRepeat = canRepeat;
        this.maxRepeatCount = maxRepeatCount;
        this.spawnChance = spawnChance;
    }
}