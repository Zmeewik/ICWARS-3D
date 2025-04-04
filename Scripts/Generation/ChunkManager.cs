using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkManager : MonoBehaviour
{
    [Header("Ссылки")]
    //Глобальные настройки игры
    [SerializeField] GameSettings gameSettings;


    [Header("ChunkConntrol")]
    //Чанки
    //Стартовые чанки
    [SerializeField] GameObject[] started_chunks;
    //Стартовый чанк
    Chunk startChunk;
    //Игровые чанки
    [SerializeField] GameObject[] chunks;
    //Интервал времени проверки расстояния
    [SerializeField] float checkInterval;

    //Пулы текущих игровых чанков
    ObjectPoolStandart<Chunk>[] chunkPools;
    //Список чанков
    List<Chunk> currentChunks = new List<Chunk>();
    //Первый и последний чанки
    [SerializeField] Chunk firstChunk;
    Vector3 firstChunkPosition;
    Chunk lastChunk;

    //Характеристики чанков
    [SerializeField] int chunkObjectNum;


    [Header("Барьеры")]
    [SerializeField] GameObject[] barriers;
    [SerializeField] float barrierPossibility;
    [SerializeField] float[] barrierPossibilities;
    ObjectPoolStandart<Barrier>[] barrierPools;


    //Игровые коллекционные предметы
    [Header("Коллекционные предметы")]
    [Header("Монеты")]
    [SerializeField] GameObject[] coins;
    //Вероятность появления монет
    [SerializeField] float coinSpawnPossibility;
    //Пулы монет
    ObjectPoolStandart<Collectable>[] coinPools;


    [Header("Бустеры")]
    [SerializeField] GameObject[] upgrades;
    //Вероятность появления бустера
    [SerializeField] float upgradeSpawnPossibility;
    //Пулы бустеров
    ObjectPoolStandart<Collectable>[] upgradePools;


    [Header("Другие коллектаблы")]
    [SerializeField] GameObject[] collectables;
    //Сккорость появления коллеккционного предмета
    [SerializeField] float collectablesSpawnPossibility;
    //Пулы коллеккционных предметов
    ObjectPoolStandart<Collectable>[] collectablePools;


    // Объединенный список пулов собираемых предметов
    private List<ObjectPoolStandart<Collectable>> allPools = new List<ObjectPoolStandart<Collectable>>();


    [Header("Декорации")]
    //Украшения дороги (зелень, тротуар, ограждение)
    [SerializeField] GameObject[] grass;
    [SerializeField] GameObject[] sidewalk;
    [SerializeField] GameObject[] railing;
    //Их пулы
    ObjectPoolStandart<Decoration>[] grassPools;
    ObjectPoolStandart<Decoration>[] sidewalkPools;
    ObjectPoolStandart<Decoration>[] railingPools;

    //Расстояние до дорожки
    [SerializeField] float distanceToSidewalk;
    //расстояние в клетках от дома, до дорожки
    [SerializeField] int distanceToHouseInCells;
    //Счёт клеток, чтобы создать дома
    int leftHouseCounter = 0;
    int rightHouseCounter = 0;

    //Украшения дома (дом, забор, мусорка)
    [SerializeField] GameObject[] houses;
    //Интервал в клетаакаъ, через который нужно создавать дома
    [SerializeField] int quantityOfCellInInterval;
    [SerializeField] GameObject[] fence;
    [SerializeField] GameObject[] trashCan;
    //Их пулы
    ObjectPoolStandart<Decoration>[] housePools;
    ObjectPoolStandart<Decoration>[] fencePools;
    ObjectPoolStandart<Decoration>[] trashCanPools;

    //Украшения природы (деревья, кусты)
    [SerializeField] GameObject[] trees;
    [SerializeField] GameObject[] bushes;
    [SerializeField] int countOfTrees;
    [SerializeField] int countOfBushes;
    //Их пулы
    ObjectPoolStandart<Decoration>[] treePools;
    ObjectPoolStandart<Decoration>[] bushPools;

    //Список всех декораций на уровне
    List<ObjectPoolStandart<Decoration>> allPoolsDeco = new List<ObjectPoolStandart<Decoration>>();


    //Стартовая начальная функция
    void Awake()
    {
        gameSettings.runnerSpeed = 0;
        CreateStartChunk();

    }

    //Сделать начальный чанк
    public void CreateStartChunk()
    {
        var obj = Instantiate(started_chunks[0], new Vector3(0, 0, 0), Quaternion.identity);
        firstChunk = obj.GetComponent<Chunk>();
        startChunk = obj.GetComponent<Chunk>();
    }

    //Начать режим игры
    public void StartRunner()
    {
        if (firstChunk == null)
            CreateStartChunk();
        gameSettings.runnerSpeed = gameSettings.minSpeed;
        firstChunk.Initialize();
        firstChunkPosition = firstChunk.EndPosition.position;
        firstChunk = null;


        SetListeners();
        SetupChunks();
        ChunkGeneration();
        while (lastChunk.EndPosition.position.z < 50)
        {
            ChunkGeneration();
        }
        UpdateSpeed(gameSettings.runnerSpeed);
        StartCoroutine(GenerationCheck());
    }

    //Остановить режим игры
    public void EndRunner()
    {

        UnsetListeners();
        ClearAllPools();
        StopAllCoroutines();
        StopPlay();
        gameSettings.runnerSpeed = 0;
        if (startChunk != null)
            DeleteStartChunk();
        if (firstChunk == null)
            CreateStartChunk();
    }

    //Удалить первый чанк
    private void DeleteStartChunk()
    {
        Destroy(startChunk.gameObject);
        startChunk = null;
    }

    //Восстановить скорость стандартную
    public void RestoreSpeed()
    {
        StartCoroutine(GenerationCheck());
        UpdateSpeed(gameSettings.runnerSpeed);
    }

    //Обнулить скорость
    public void StopPlay()
    {
        StopAllCoroutines();
        UpdateSpeed(0);
    }

    //Изменение скорости объектов
    public void UpdateSpeed(float speed)
    {
        //Изменить скорость стартового чанка
        if (startChunk != null)
            startChunk.ChangeSpeed(speed);

        //Чанки
        foreach (var pool in chunkPools)
        {
            pool.ModifyAllObjects(obj =>
            {
                //Изменение объекта
                obj.ChangeSpeed(speed);
            });
        }
        //Кколлекционные предметы
        foreach (var pool in allPools)
        {
            pool.ModifyAllObjects(obj =>
            {
                //Изменение объекта
                obj.ChangeSpeed(speed);
            });
        }
        //Барьеры
        foreach (var pool in barrierPools)
        {
            pool.ModifyAllObjects(obj =>
            {
                //Изменение объекта
                obj.ChangeSpeed(speed);
            }
            );
        }
        //Декорации
        foreach (var pool in allPoolsDeco)
        {
            pool.ModifyAllObjects(obj =>
            {
                //Изменение объекта
                obj.ChangeSpeed(speed);
            }
            );
        }
    }


    //Установка подписки для событий столкновения
    void SetListeners()
    {
        //Подписка на события сбора коллекционных предметов
        PlayerCollisionControl.OnCoinTouch.AddListener(DeactivateCollectable);
        PlayerCollisionControl.OnUpgradeTouch.AddListener(DeactivateCollectable);
        PlayerCollisionControl.OnCollectableTouch.AddListener(DeactivateCollectable);

        //Подписка на событие смерти игрока
        PlayerMovement.OnElimination.AddListener(StopPlay);
    }

    //Отмена подписки на события
    void UnsetListeners()
    {
        PlayerCollisionControl.OnCoinTouch.RemoveListener(DeactivateCollectable);
        PlayerCollisionControl.OnUpgradeTouch.RemoveListener(DeactivateCollectable);
        PlayerCollisionControl.OnCollectableTouch.RemoveListener(DeactivateCollectable);
        PlayerMovement.OnElimination.RemoveListener(StopPlay);
    }

    //Генерация чанков......................................................................................................................................................................
    //Инициализация стартовых чанков
    void SetupChunks()
    {
        //Сделать количество пулов по кличеству чанков и сделать каждому свой пул
        chunkPools = new ObjectPoolStandart<Chunk>[chunks.Length];
        for (int r = 0; r < chunks.Length; r++)
        {
            // Создаем новый экземпляр пула
            chunkPools[r] = new ObjectPoolStandart<Chunk>();
            chunkPools[r].Setup(chunks[r], 2, 4, r);
        }

        //Сделать пул монет
        coinPools = new ObjectPoolStandart<Collectable>[coins.Length];
        for (int r = 0; r < coins.Length; r++)
        {
            // Создаем новый экземпляр пула
            coinPools[r] = new ObjectPoolStandart<Collectable>();
            coinPools[r].Setup(coins[r], 30, 60, r);
        }

        //Сделать пул бустеров
        upgradePools = new ObjectPoolStandart<Collectable>[upgrades.Length];
        for (int r = 0; r < upgrades.Length; r++)
        {
            // Создаем новый экземпляр пула
            upgradePools[r] = new ObjectPoolStandart<Collectable>();
            upgradePools[r].Setup(upgrades[r], 2, 4, r);
        }

        //Сделать пул коллекционных предметов
        collectablePools = new ObjectPoolStandart<Collectable>[collectables.Length];
        for (int r = 0; r < collectables.Length; r++)
        {
            // Создаем новый экземпляр пула
            collectablePools[r] = new ObjectPoolStandart<Collectable>();
            collectablePools[r].Setup(collectables[r], 2, 4, r);
        }

        //Сделать пул барьеров
        barrierPools = new ObjectPoolStandart<Barrier>[barriers.Length];
        for (int r = 0; r < barriers.Length; r++)
        {
            // Создаем новый экземпляр пула
            barrierPools[r] = new ObjectPoolStandart<Barrier>();
            barrierPools[r].Setup(barriers[r], 10, 20, r);
        }

        //Объединяем все коллекционные предметы
        allPools.AddRange(coinPools);
        allPools.AddRange(upgradePools);
        allPools.AddRange(collectablePools);

        //Сделать пул травы
        grassPools = new ObjectPoolStandart<Decoration>[grass.Length];
        for (int r = 0; r < grass.Length; r++)
        {
            // Создаем новый экземпляр пула
            grassPools[r] = new ObjectPoolStandart<Decoration>();
            grassPools[r].Setup(grass[r], 150, 200, r);
        }

        //Сделать пул тротуаров
        sidewalkPools = new ObjectPoolStandart<Decoration>[sidewalk.Length];
        for (int r = 0; r < sidewalk.Length; r++)
        {
            // Создаем новый экземпляр пула
            sidewalkPools[r] = new ObjectPoolStandart<Decoration>();
            sidewalkPools[r].Setup(sidewalk[r], 150, 200, r);
        }

        //Сделать пул оград дороги
        railingPools = new ObjectPoolStandart<Decoration>[railing.Length];
        for (int r = 0; r < railing.Length; r++)
        {
            // Создаем новый экземпляр пула
            railingPools[r] = new ObjectPoolStandart<Decoration>();
            railingPools[r].Setup(railing[r], 300, 400, r);
        }

        //Сделать пул домов
        housePools = new ObjectPoolStandart<Decoration>[houses.Length];
        for (int r = 0; r < houses.Length; r++)
        {
            // Создаем новый экземпляр пула
            housePools[r] = new ObjectPoolStandart<Decoration>();
            housePools[r].Setup(houses[r], 150, 200, r);
        }

        //Сделать пул заборов
        fencePools = new ObjectPoolStandart<Decoration>[fence.Length];
        for (int r = 0; r < fence.Length; r++)
        {
            // Создаем новый экземпляр пула
            fencePools[r] = new ObjectPoolStandart<Decoration>();
            fencePools[r].Setup(fence[r], 80, 120, r);
        }

        //Сделать пул мусора
        trashCanPools = new ObjectPoolStandart<Decoration>[trashCan.Length];
        for (int r = 0; r < trashCan.Length; r++)
        {
            // Создаем новый экземпляр пула
            trashCanPools[r] = new ObjectPoolStandart<Decoration>();
            trashCanPools[r].Setup(trashCan[r], 150, 200, r);
        }

        //Сделать пул деревьев
        treePools = new ObjectPoolStandart<Decoration>[trees.Length];
        for (int r = 0; r < trees.Length; r++)
        {
            // Создаем новый экземпляр пула
            treePools[r] = new ObjectPoolStandart<Decoration>();
            treePools[r].Setup(trees[r], 150, 200, r);
        }

        //Сделать пул кустов
        bushPools = new ObjectPoolStandart<Decoration>[bushes.Length];
        for (int r = 0; r < bushes.Length; r++)
        {
            // Создаем новый экземпляр пула
            bushPools[r] = new ObjectPoolStandart<Decoration>();
            bushPools[r].Setup(bushes[r], 150, 200, r);
        }

        //Добавить в общий пул все декорации
        allPoolsDeco.AddRange(grassPools);
        allPoolsDeco.AddRange(sidewalkPools);
        allPoolsDeco.AddRange(railingPools);
        allPoolsDeco.AddRange(housePools);
        allPoolsDeco.AddRange(fencePools);
        allPoolsDeco.AddRange(trashCanPools);
        allPoolsDeco.AddRange(treePools);
        allPoolsDeco.AddRange(bushPools);
    }

    // Метод для очистки всех пулов
    void ClearAllPools()
    {
        firstChunk = null;
        lastChunk = null;
        currentChunks.Clear();
        ClearPools(chunkPools);
        ClearPools(coinPools);
        ClearPools(upgradePools);
        ClearPools(collectablePools);
        ClearPools(barrierPools);
        
        ClearPools(grassPools);
        ClearPools(sidewalkPools);
        ClearPools(railingPools);
        ClearPools(housePools);
        ClearPools(fencePools);
        ClearPools(trashCanPools);
        ClearPools(treePools);
        ClearPools(bushPools);

        allPoolsDeco.Clear();
        allPools.Clear();
    }


    //Метод уничтожения всех объектов пула
    private void ClearPools<T>(ObjectPoolStandart<T>[] pools) where T : Component, IPoolObject
    {
        if (pools == null) return;

        foreach (var pool in pools)
        {
            //Уничтожаем все объекты пула
            var objs = pool.GetObjects();
            foreach (var obj in objs)
            { 
                Destroy(obj.gameObject);
            }
            // Очищаем пул
            pool.ClearPool();
        }
    }


    //Остановить режим бега
    public void StopGeneration()
    {
        //остановка всех обхектов
        UpdateSpeed(0);
    }


    //Логика времени установки и удалениия чанков
    public void ChunkGeneration()
    {
        //Удалить первый чанк, если за экраном
        if (startChunk != null && startChunk.EndPosition.position.z < -10)
            DeleteStartChunk();

        //Если последняя точка первого чанка находитсяя за экраном 
        if (firstChunk != null && firstChunk.EndPosition.position.z < -10)
            DeleteOldChunk();
        //Если последняя точка последнего чанка находится менее чем через 100 юнитов от камеры, вне зоны её рендера
        if (lastChunk == null ||  lastChunk.EndPosition.position.z < 50)
            AddNewChunk();
    }


    //Установить новый чанк
    public void AddNewChunk()
    {
        //Выбрать случайный чанк
        var chunkNum = Random.Range(0, chunkPools.Length);
        Chunk nextChunk = chunkPools[chunkNum].Get();

        //Поставить случайныый чанк в конце предыдущего или примерно в начале координат
        if (currentChunks.Count > 0)
        {
            nextChunk.transform.position = currentChunks[currentChunks.Count - 1].EndPosition.position + (nextChunk.transform.position - nextChunk.StartPosition.position);
        }
        else
        {
            firstChunk = nextChunk;
            var Offset = firstChunk.EndPosition.position - firstChunk.StartPosition.position;
            nextChunk.transform.position = firstChunkPosition + Vector3.up * 0.6f + Offset / 2;
        }

        //Добавить чанк в текущие
        currentChunks.Add(nextChunk);
        lastChunk = nextChunk;

        //Добавить для нового чанка объектов
        if(currentChunks.Count > 0)
            GenerateChunkObjects(lastChunk);
    }

    //Удалить первый чанк из списка
    public void DeleteOldChunk()
    {
        if (firstChunk != null)
        {
            //Отчичтить все объекты чанка
            DeleteChunkObjects(firstChunk);
            firstChunk.chunkCollectables.Clear();

            //Вернуть объект в пул
            var poolNum = firstChunk.poolNum;
            chunkPools[poolNum].Release(firstChunk);
            //Debug.Log("Chunk deleted: " + firstChunk.name);

            //Удалить первый объект из списка
            currentChunks.RemoveAt(0);
            firstChunk = currentChunks.Count > 0 ? currentChunks[0] : null;
        }
    }

    //Удалить объекты первых двух чанков
    public void DestroyCurrentChunk()
    {
        DeleteChunkObjectsAtTrack(currentChunks[0]);
        DeleteChunkObjectsAtTrack(currentChunks[1]);
    }


    //Функция вызываемая с определённым шагом времени
    IEnumerator GenerationCheck()
    {
        while (true)
        {
            ChunkGeneration();
            yield return new WaitForSeconds(checkInterval);
        }
    }




    //Генерация объектов......................................................................................................................................................................


    //Удаление всех объхектов с пути движения
    private void DeleteChunkObjectsAtTrack(Chunk chunk)
    {
        //Освобождение коллекционных пердметов
        foreach (var chunkObj in chunk.chunkCollectables)
        {
            if (chunkObj.tag == "Coin")
            {
                coinPools[chunkObj.poolNum].Release(chunkObj);
            }
            else if (chunkObj.tag == "Collectable")
            {
                collectablePools[chunkObj.poolNum].Release(chunkObj);
            }
            else if (chunkObj.tag == "Upgrade")
            {
                upgradePools[chunkObj.poolNum].Release(chunkObj);
            }
        }
        //освобождение пула преград
        foreach (var chunkObj in chunk.chunkBarriers)
        {
            barrierPools[chunkObj.poolNum].Release(chunkObj);
        }

        //Очищаем массивы, чтобы не повторялось очищение уже очищенных объектов
        chunk.chunkCollectables.Clear();
        chunk.chunkBarriers.Clear();
    }

    //Освобождение объектов чанка
    private void DeleteChunkObjects(Chunk chunk)
    {
        //Освобождение коллекционных пердметов
        foreach(var chunkObj in chunk.chunkCollectables)
        {
            if (chunkObj.tag == "Coin")
            {
                coinPools[chunkObj.poolNum].Release(chunkObj);
            }
            else if (chunkObj.tag == "Collectable")
            {
                collectablePools[chunkObj.poolNum].Release(chunkObj);
            }
            else if (chunkObj.tag == "Upgrade")
            {
                upgradePools[chunkObj.poolNum].Release(chunkObj);
            }
        }
        //освобождение пула преград
        foreach (var chunkObj in chunk.chunkBarriers)
        {
            barrierPools[chunkObj.poolNum].Release(chunkObj);
        }

        //Освобождение пулов украшений
        foreach (var chunkDeco in chunk.chunkDecorations)
        {
            switch (chunkDeco.type)
            {
                case "grass": grassPools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
                case "sidewalk": sidewalkPools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
                case "railing": railingPools[chunkDeco.poolNum].Release(chunkDeco); 
                    break;
                case "house": housePools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
                case "fence": fencePools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
                case "trashCan": trashCanPools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
                case "tree": treePools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
                case "bush": bushPools[chunkDeco.poolNum].Release(chunkDeco);
                    break;
            }
        }

        //Очищаем массивы, чтобы не повторялось очищение уже очищенных объектов
        chunk.chunkCollectables.Clear();
        chunk.chunkBarriers.Clear();
        chunk.chunkDecorations.Clear();
    }

    //генерация разных объектов
    private void GenerateChunkObjects(Chunk chunk)
    {
        bool isCreatingBarriers = false;
        if (currentChunks.Count > 1)
            isCreatingBarriers = true;

        //Получаем расстояние между объектами
        var objectSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / (float)chunkObjectNum;
        Vector3 objectSpawnPosition = lastChunk.StartPosition.position;

        //Отчичтить пул объектов чанка
        chunk.chunkCollectables = new List<Collectable>();

        if (isCreatingBarriers)
        {
            //С шагом в расстояние создаём объекты на чанке
            for (int r = 0; r < chunkObjectNum; r++)
            {
                //Пройти по 3 линиям которые существуют
                for (int f = 0; f < 3; f++)
                {
                    //Создание ммонет
                    var currentPossibility = Random.Range(0f, 100f);
                    //Если появляется с нужной вероятностью, то выбираем случайный1 и размещаем на карте
                    if (currentPossibility < coinSpawnPossibility * 100)
                    {
                        var coinNum = Random.Range(0, coinPools.Length);
                        var coinObj = coinPools[coinNum].Get();

                        //Получаем разницу между высотами объектов
                        var yOffset = coinObj.transform.position.y - objectSpawnPosition.y;
                        //Размещаем монету на линии
                        coinObj.transform.position = objectSpawnPosition + (f - 1) * new Vector3(1, 0, 0) * gameSettings.lineRange + new Vector3(0, 1, 0) * yOffset;

                        //Добавление объектов в пул чанка
                        chunk.chunkCollectables.Add(coinObj);
                    }
                    else if(currentPossibility < barrierPossibility * 100)
                    {
                        var barrierNum = Random.Range(0, barriers.Length);
                        var barrierObj = barrierPools[barrierNum].Get();


                        //Получаем разницу между высотами объектов
                        var yOffset = barrierObj.transform.position.y - objectSpawnPosition.y;
                        //Размещаем барьер на линии
                        barrierObj.transform.position = objectSpawnPosition + (f - 1) * new Vector3(1, 0, 0) * gameSettings.lineRange + new Vector3(0, 1, 0) * yOffset;

                        //Добавление объектов в пул чанка
                        chunk.chunkBarriers.Add(barrierObj);
                    }


                    //Создание барьеров
                    
                }

                objectSpawnPosition += new Vector3(0, 0, 1) * objectSpace;
            }
        }


        var grassSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / 5;
        objectSpawnPosition = lastChunk.StartPosition.position + Vector3.left * 4.4f + Vector3.forward * 2;
        //С шагом заспавнить тайлы травы слева и справа
        int sign = -1;
        for (int y = 0; y < 2; y++)
        {
            for (int r = 0; r < 5; r++)
            {
                //Размещаем тайлы слева
                for (int m = 0; m < 3; m++)
                {
                    var grassObj = grassPools[0].Get();
                    //Получаем разницу между высотами объектов
                    var yOffset = grassObj.transform.position.y - objectSpawnPosition.y;
                    grassObj.transform.position = objectSpawnPosition + sign * m * new Vector3(1, 0, 0) * grassSpace + new Vector3(0, 1, 0) * yOffset;

                    //Добавление объектов в пул чанка
                    chunk.chunkDecorations.Add(grassObj);
                }

                objectSpawnPosition += new Vector3(0, 0, 1) * grassSpace;
            }

            sign = 1;
            objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * 4.4f + Vector3.forward * 2;
        }




        //С шагом заспавнить тайлы дороги слева и справа
        var sidewalkSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / 10;
        objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * (4.4f + distanceToSidewalk) + Vector3.forward * 1 + Vector3.up * 0.2f;
        sign = -1;

        for (int r = 0; r < 10; r++)
        {
            //Заспавнить дома
            bool lHouseSpawned = false;
            bool rHouseSpawned = false;
            leftHouseCounter++;
            rightHouseCounter++;

            if (leftHouseCounter > quantityOfCellInInterval)
            {
                lHouseSpawned = true;
                leftHouseCounter = 0;
            }

            if (rightHouseCounter > quantityOfCellInInterval)
            {
                rHouseSpawned = true;
                rightHouseCounter = 0;
            }

            //Заспавнить дорогу
            for (int f = 0; f < 2; f++)
            {
                var sidewalkObj = sidewalkPools[0].Get();

                //Создать левый дом с дорожкой
                if (lHouseSpawned && sign == -1)
                {
                    sidewalkPools[0].Release(sidewalkObj);
                    sidewalkObj = sidewalkPools[2].Get();
                    //Создать дорожку к дому
                    for (int c = 0; c < distanceToHouseInCells; c++)
                    {
                        var newCell = sidewalkPools[0].Get();
                        newCell.transform.position = objectSpawnPosition - Vector3.left * sidewalkSpace * c - Vector3.left * sidewalkSpace;
                        newCell.transform.rotation = Quaternion.Euler(-90, 180,0);
                        //Добавление объектов в пул чанка
                        chunk.chunkDecorations.Add(newCell);
                    }

                    //Создать дом
                    var houseNum = Random.Range(0, houses.Length);
                    var houseObj = housePools[houseNum].Get();
                    var housePos = objectSpawnPosition - Vector3.left * sidewalkSpace * distanceToHouseInCells - Vector3.left * sidewalkSpace;
                    houseObj.transform.position = housePos + Vector3.forward * sidewalkSpace / 2;
                    houseObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //Добавление объектов в пул чанка
                    chunk.chunkDecorations.Add(houseObj);


                    //Созать забор
                    /*var fencePos = housePos - Vector3.forward * sidewalkSpace * 1.5f + Vector3.up * 0.7f + Vector3.right * 3;
                    var fenceObj = fencePools[0].Get();
                    fenceObj.transform.position = fencePos;
                    chunk.chunkDecorations.Add(fenceObj);
*/
                }
                else if(!lHouseSpawned && sign == -1 && quantityOfCellInInterval - leftHouseCounter > 0)
                {
                    //Созать забор
/*                    for (int countFence = 0; countFence < 2; countFence++)
                    {
                        var fencePos = objectSpawnPosition - Vector3.left * sidewalkSpace * distanceToHouseInCells - Vector3.left * sidewalkSpace;
                        fencePos = fencePos + (countFence - 1) * sidewalkSpace / 2 * Vector3.forward + Vector3.up * 0.7f + Vector3.right * 3;

                        var fenceObj = fencePools[0].Get();
                        fenceObj.transform.position = fencePos;
                        chunk.chunkDecorations.Add(fenceObj);
                    }*/
                }

                //Создать правый дом с дорожкой
                if (rHouseSpawned && sign == 1)
                {
                    sidewalkPools[0].Release(sidewalkObj);
                    sidewalkObj = sidewalkPools[1].Get();
                    //Создать дорожку к дому
                    for (int c = 0; c < distanceToHouseInCells; c++)
                    {
                        var newCell = sidewalkPools[0].Get();
                        newCell.transform.position = objectSpawnPosition + Vector3.left * sidewalkSpace * c + Vector3.left * sidewalkSpace;
                        newCell.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        //Добавление объектов в пул чанка
                        chunk.chunkDecorations.Add(newCell);
                    }

                    //Создать дом
                    var houseNum = Random.Range(0, houses.Length);
                    var houseObj = housePools[houseNum].Get();
                    var housePos = objectSpawnPosition + Vector3.left * sidewalkSpace * distanceToHouseInCells + Vector3.left * sidewalkSpace;
                    housePos -= Vector3.forward * 1.5f * sidewalkSpace;
                    houseObj.transform.position = housePos + Vector3.forward * sidewalkSpace / 2;
                    houseObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                    //Добавление объектов в пул чанка
                    chunk.chunkDecorations.Add(houseObj);

                    //Созать забор
/*                    var fencePos = housePos - Vector3.forward * sidewalkSpace * 1.5f + Vector3.up * 0.7f - Vector3.right * 3 + Vector3.forward * 1.5f * sidewalkSpace;
                    var fenceObj = fencePools[0].Get();
                    fenceObj.transform.position = fencePos;
                    chunk.chunkDecorations.Add(fenceObj);*/
                }
                else if(!rHouseSpawned && sign == 1 && quantityOfCellInInterval - rightHouseCounter > 0)
                {
/*                    //Созать забор
                    for (int countFence = 0; countFence < 2; countFence++)
                    {
                        var fencePos = objectSpawnPosition + Vector3.left * sidewalkSpace * distanceToHouseInCells + Vector3.left * sidewalkSpace - Vector3.forward * 0.5f * sidewalkSpace;
                        fencePos = fencePos - (countFence - 1) * sidewalkSpace / 2 * Vector3.forward + Vector3.up * 0.7f - Vector3.right * 3;

                        var fenceObj = fencePools[0].Get();
                        fenceObj.transform.position = fencePos;
                        chunk.chunkDecorations.Add(fenceObj);
                    }*/

                }

                var yOffset = sidewalkObj.transform.position.y - objectSpawnPosition.y;
                sidewalkObj.transform.position = objectSpawnPosition;
                sidewalkObj.transform.rotation = Quaternion.Euler(-90, 90, 0);

                //Добавление объектов в пул чанка
                chunk.chunkDecorations.Add(sidewalkObj);



                //Создать кусты
                //Не на дорожках
                if (!(lHouseSpawned && sign == -1) && !(rHouseSpawned && sign == 1))
                {
                    var grassPos = objectSpawnPosition + sign * Vector3.left * sidewalkSpace;
                    //Случайное место между тротуаром и домами
                    var numberOfPlants = Random.Range(0, countOfBushes);
                    var numberOfTrees = Random.Range(0, countOfTrees);
                    //Случайные кусты
                    for (int u = 0; u < numberOfPlants; u++)
                    {
                        var xPos = Random.Range(grassPos.x, (grassPos + sign * Vector3.left * sidewalkSpace * distanceToHouseInCells).x);
                        var zPos = Random.Range((grassPos - Vector3.forward * sidewalkSpace / 2).z, (grassPos + Vector3.forward * sidewalkSpace / 2).z);

                        var bushNum = Random.Range(0, bushes.Length);
                        var bushObj = bushPools[bushNum].Get();
                        bushObj.transform.position = new Vector3(xPos, grassPos.y, zPos);
                        chunk.chunkDecorations.Add(bushObj);
                    }

                    //Случайные деревья
                    for (int u = 0; u < numberOfPlants; u++)
                    {
                        var xPos = Random.Range(grassPos.x, (grassPos + sign * Vector3.left * sidewalkSpace * distanceToHouseInCells).x);
                        var zPos = Random.Range((grassPos - Vector3.forward * sidewalkSpace / 2).z, (grassPos + Vector3.forward * sidewalkSpace / 2).z);

                        var treeNum = Random.Range(0, trees.Length);
                        var treeObj = treePools[treeNum].Get();
                        treeObj.transform.position = new Vector3(xPos, grassPos.y, zPos);
                        chunk.chunkDecorations.Add(treeObj);
                    }
                }


                //Переместить точку слева направо
                sign = -sign;
                objectSpawnPosition = objectSpawnPosition + sign * Vector3.left * (4.4f + distanceToSidewalk) * 2;
            }

            //Переместить точку спавна вперёд
            objectSpawnPosition += new Vector3(0, 0, 1) * sidewalkSpace;

        }



        //Создать траву и деревья
        objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * (4.4f + distanceToSidewalk) + Vector3.forward * 1 + Vector3.up * 0.2f - Vector3.left * sidewalkSpace;
        sign = -1;

        for (int y = 0; y < 10; y++)
        { 


            objectSpawnPosition += Vector3.forward * sidewalkSpace;
        }

        //Создание ограды слева и справа
        var railingSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / 30;
        objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * (4.4f + distanceToSidewalk - 1) + Vector3.forward * 1 - Vector3.up * 0.2f;

        for (int r = 0; r < 30; r++)
        {
            //Заспавнить ограду
            for (int f = 0; f < 2; f++)
            {
                var railingObj = railingPools[0].Get();
                var yOffset = railingObj.transform.position.y - objectSpawnPosition.y;
                railingObj.transform.position = objectSpawnPosition + new Vector3(0, 1, 0);

                //Добавление объектов в пул чанка
                chunk.chunkDecorations.Add(railingObj);

                //Переместить точку слева направо
                sign = -sign;
                objectSpawnPosition = objectSpawnPosition + sign * Vector3.left * (4.4f + distanceToSidewalk - 1) * 2;
            }

            //Переместить точку спавна вперёд
            objectSpawnPosition += new Vector3(0, 0, 1) * railingSpace;

        }
    }
    

    //Отчистить коллекционный объект из чанка и вернуть в пул при столкновении
    public void DeactivateCollectable(Collectable obj)
    {
        for (int c = 0; c < currentChunks.Count; c++)
        {
            if (currentChunks[c].chunkCollectables.Contains(obj))
            {
                currentChunks[c].chunkCollectables.Remove(obj);
            }
        }
        if (obj.isActiveAndEnabled)
        {
            switch (obj.tag)
            {
                case "Coin":
                    coinPools[obj.poolNum].Release(obj);
                    break;
                case "Collectable":
                    collectablePools[obj.poolNum].Release(obj);
                    break;
                case "Upgrade":
                    upgradePools[obj.poolNum].Release(obj);
                    break;
            }
            
        }
    }
    public void DeactivateChunk(Chunk obj)
    {
        if (obj.isActiveAndEnabled)
            chunkPools[obj.poolNum].Release(obj);
    }
}
