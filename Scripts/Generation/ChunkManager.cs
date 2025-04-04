using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkManager : MonoBehaviour
{
    [Header("������")]
    //���������� ��������� ����
    [SerializeField] GameSettings gameSettings;


    [Header("ChunkConntrol")]
    //�����
    //��������� �����
    [SerializeField] GameObject[] started_chunks;
    //��������� ����
    Chunk startChunk;
    //������� �����
    [SerializeField] GameObject[] chunks;
    //�������� ������� �������� ����������
    [SerializeField] float checkInterval;

    //���� ������� ������� ������
    ObjectPoolStandart<Chunk>[] chunkPools;
    //������ ������
    List<Chunk> currentChunks = new List<Chunk>();
    //������ � ��������� �����
    [SerializeField] Chunk firstChunk;
    Vector3 firstChunkPosition;
    Chunk lastChunk;

    //�������������� ������
    [SerializeField] int chunkObjectNum;


    [Header("�������")]
    [SerializeField] GameObject[] barriers;
    [SerializeField] float barrierPossibility;
    [SerializeField] float[] barrierPossibilities;
    ObjectPoolStandart<Barrier>[] barrierPools;


    //������� ������������� ��������
    [Header("������������� ��������")]
    [Header("������")]
    [SerializeField] GameObject[] coins;
    //����������� ��������� �����
    [SerializeField] float coinSpawnPossibility;
    //���� �����
    ObjectPoolStandart<Collectable>[] coinPools;


    [Header("�������")]
    [SerializeField] GameObject[] upgrades;
    //����������� ��������� �������
    [SerializeField] float upgradeSpawnPossibility;
    //���� ��������
    ObjectPoolStandart<Collectable>[] upgradePools;


    [Header("������ �����������")]
    [SerializeField] GameObject[] collectables;
    //��������� ��������� ��������������� ��������
    [SerializeField] float collectablesSpawnPossibility;
    //���� �������������� ���������
    ObjectPoolStandart<Collectable>[] collectablePools;


    // ������������ ������ ����� ���������� ���������
    private List<ObjectPoolStandart<Collectable>> allPools = new List<ObjectPoolStandart<Collectable>>();


    [Header("���������")]
    //��������� ������ (������, �������, ����������)
    [SerializeField] GameObject[] grass;
    [SerializeField] GameObject[] sidewalk;
    [SerializeField] GameObject[] railing;
    //�� ����
    ObjectPoolStandart<Decoration>[] grassPools;
    ObjectPoolStandart<Decoration>[] sidewalkPools;
    ObjectPoolStandart<Decoration>[] railingPools;

    //���������� �� �������
    [SerializeField] float distanceToSidewalk;
    //���������� � ������� �� ����, �� �������
    [SerializeField] int distanceToHouseInCells;
    //���� ������, ����� ������� ����
    int leftHouseCounter = 0;
    int rightHouseCounter = 0;

    //��������� ���� (���, �����, �������)
    [SerializeField] GameObject[] houses;
    //�������� � ���������, ����� ������� ����� ��������� ����
    [SerializeField] int quantityOfCellInInterval;
    [SerializeField] GameObject[] fence;
    [SerializeField] GameObject[] trashCan;
    //�� ����
    ObjectPoolStandart<Decoration>[] housePools;
    ObjectPoolStandart<Decoration>[] fencePools;
    ObjectPoolStandart<Decoration>[] trashCanPools;

    //��������� ������� (�������, �����)
    [SerializeField] GameObject[] trees;
    [SerializeField] GameObject[] bushes;
    [SerializeField] int countOfTrees;
    [SerializeField] int countOfBushes;
    //�� ����
    ObjectPoolStandart<Decoration>[] treePools;
    ObjectPoolStandart<Decoration>[] bushPools;

    //������ ���� ��������� �� ������
    List<ObjectPoolStandart<Decoration>> allPoolsDeco = new List<ObjectPoolStandart<Decoration>>();


    //��������� ��������� �������
    void Awake()
    {
        gameSettings.runnerSpeed = 0;
        CreateStartChunk();

    }

    //������� ��������� ����
    public void CreateStartChunk()
    {
        var obj = Instantiate(started_chunks[0], new Vector3(0, 0, 0), Quaternion.identity);
        firstChunk = obj.GetComponent<Chunk>();
        startChunk = obj.GetComponent<Chunk>();
    }

    //������ ����� ����
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

    //���������� ����� ����
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

    //������� ������ ����
    private void DeleteStartChunk()
    {
        Destroy(startChunk.gameObject);
        startChunk = null;
    }

    //������������ �������� �����������
    public void RestoreSpeed()
    {
        StartCoroutine(GenerationCheck());
        UpdateSpeed(gameSettings.runnerSpeed);
    }

    //�������� ��������
    public void StopPlay()
    {
        StopAllCoroutines();
        UpdateSpeed(0);
    }

    //��������� �������� ��������
    public void UpdateSpeed(float speed)
    {
        //�������� �������� ���������� �����
        if (startChunk != null)
            startChunk.ChangeSpeed(speed);

        //�����
        foreach (var pool in chunkPools)
        {
            pool.ModifyAllObjects(obj =>
            {
                //��������� �������
                obj.ChangeSpeed(speed);
            });
        }
        //�������������� ��������
        foreach (var pool in allPools)
        {
            pool.ModifyAllObjects(obj =>
            {
                //��������� �������
                obj.ChangeSpeed(speed);
            });
        }
        //�������
        foreach (var pool in barrierPools)
        {
            pool.ModifyAllObjects(obj =>
            {
                //��������� �������
                obj.ChangeSpeed(speed);
            }
            );
        }
        //���������
        foreach (var pool in allPoolsDeco)
        {
            pool.ModifyAllObjects(obj =>
            {
                //��������� �������
                obj.ChangeSpeed(speed);
            }
            );
        }
    }


    //��������� �������� ��� ������� ������������
    void SetListeners()
    {
        //�������� �� ������� ����� ������������� ���������
        PlayerCollisionControl.OnCoinTouch.AddListener(DeactivateCollectable);
        PlayerCollisionControl.OnUpgradeTouch.AddListener(DeactivateCollectable);
        PlayerCollisionControl.OnCollectableTouch.AddListener(DeactivateCollectable);

        //�������� �� ������� ������ ������
        PlayerMovement.OnElimination.AddListener(StopPlay);
    }

    //������ �������� �� �������
    void UnsetListeners()
    {
        PlayerCollisionControl.OnCoinTouch.RemoveListener(DeactivateCollectable);
        PlayerCollisionControl.OnUpgradeTouch.RemoveListener(DeactivateCollectable);
        PlayerCollisionControl.OnCollectableTouch.RemoveListener(DeactivateCollectable);
        PlayerMovement.OnElimination.RemoveListener(StopPlay);
    }

    //��������� ������......................................................................................................................................................................
    //������������� ��������� ������
    void SetupChunks()
    {
        //������� ���������� ����� �� ��������� ������ � ������� ������� ���� ���
        chunkPools = new ObjectPoolStandart<Chunk>[chunks.Length];
        for (int r = 0; r < chunks.Length; r++)
        {
            // ������� ����� ��������� ����
            chunkPools[r] = new ObjectPoolStandart<Chunk>();
            chunkPools[r].Setup(chunks[r], 2, 4, r);
        }

        //������� ��� �����
        coinPools = new ObjectPoolStandart<Collectable>[coins.Length];
        for (int r = 0; r < coins.Length; r++)
        {
            // ������� ����� ��������� ����
            coinPools[r] = new ObjectPoolStandart<Collectable>();
            coinPools[r].Setup(coins[r], 30, 60, r);
        }

        //������� ��� ��������
        upgradePools = new ObjectPoolStandart<Collectable>[upgrades.Length];
        for (int r = 0; r < upgrades.Length; r++)
        {
            // ������� ����� ��������� ����
            upgradePools[r] = new ObjectPoolStandart<Collectable>();
            upgradePools[r].Setup(upgrades[r], 2, 4, r);
        }

        //������� ��� ������������� ���������
        collectablePools = new ObjectPoolStandart<Collectable>[collectables.Length];
        for (int r = 0; r < collectables.Length; r++)
        {
            // ������� ����� ��������� ����
            collectablePools[r] = new ObjectPoolStandart<Collectable>();
            collectablePools[r].Setup(collectables[r], 2, 4, r);
        }

        //������� ��� ��������
        barrierPools = new ObjectPoolStandart<Barrier>[barriers.Length];
        for (int r = 0; r < barriers.Length; r++)
        {
            // ������� ����� ��������� ����
            barrierPools[r] = new ObjectPoolStandart<Barrier>();
            barrierPools[r].Setup(barriers[r], 10, 20, r);
        }

        //���������� ��� ������������� ��������
        allPools.AddRange(coinPools);
        allPools.AddRange(upgradePools);
        allPools.AddRange(collectablePools);

        //������� ��� �����
        grassPools = new ObjectPoolStandart<Decoration>[grass.Length];
        for (int r = 0; r < grass.Length; r++)
        {
            // ������� ����� ��������� ����
            grassPools[r] = new ObjectPoolStandart<Decoration>();
            grassPools[r].Setup(grass[r], 150, 200, r);
        }

        //������� ��� ���������
        sidewalkPools = new ObjectPoolStandart<Decoration>[sidewalk.Length];
        for (int r = 0; r < sidewalk.Length; r++)
        {
            // ������� ����� ��������� ����
            sidewalkPools[r] = new ObjectPoolStandart<Decoration>();
            sidewalkPools[r].Setup(sidewalk[r], 150, 200, r);
        }

        //������� ��� ����� ������
        railingPools = new ObjectPoolStandart<Decoration>[railing.Length];
        for (int r = 0; r < railing.Length; r++)
        {
            // ������� ����� ��������� ����
            railingPools[r] = new ObjectPoolStandart<Decoration>();
            railingPools[r].Setup(railing[r], 300, 400, r);
        }

        //������� ��� �����
        housePools = new ObjectPoolStandart<Decoration>[houses.Length];
        for (int r = 0; r < houses.Length; r++)
        {
            // ������� ����� ��������� ����
            housePools[r] = new ObjectPoolStandart<Decoration>();
            housePools[r].Setup(houses[r], 150, 200, r);
        }

        //������� ��� �������
        fencePools = new ObjectPoolStandart<Decoration>[fence.Length];
        for (int r = 0; r < fence.Length; r++)
        {
            // ������� ����� ��������� ����
            fencePools[r] = new ObjectPoolStandart<Decoration>();
            fencePools[r].Setup(fence[r], 80, 120, r);
        }

        //������� ��� ������
        trashCanPools = new ObjectPoolStandart<Decoration>[trashCan.Length];
        for (int r = 0; r < trashCan.Length; r++)
        {
            // ������� ����� ��������� ����
            trashCanPools[r] = new ObjectPoolStandart<Decoration>();
            trashCanPools[r].Setup(trashCan[r], 150, 200, r);
        }

        //������� ��� ��������
        treePools = new ObjectPoolStandart<Decoration>[trees.Length];
        for (int r = 0; r < trees.Length; r++)
        {
            // ������� ����� ��������� ����
            treePools[r] = new ObjectPoolStandart<Decoration>();
            treePools[r].Setup(trees[r], 150, 200, r);
        }

        //������� ��� ������
        bushPools = new ObjectPoolStandart<Decoration>[bushes.Length];
        for (int r = 0; r < bushes.Length; r++)
        {
            // ������� ����� ��������� ����
            bushPools[r] = new ObjectPoolStandart<Decoration>();
            bushPools[r].Setup(bushes[r], 150, 200, r);
        }

        //�������� � ����� ��� ��� ���������
        allPoolsDeco.AddRange(grassPools);
        allPoolsDeco.AddRange(sidewalkPools);
        allPoolsDeco.AddRange(railingPools);
        allPoolsDeco.AddRange(housePools);
        allPoolsDeco.AddRange(fencePools);
        allPoolsDeco.AddRange(trashCanPools);
        allPoolsDeco.AddRange(treePools);
        allPoolsDeco.AddRange(bushPools);
    }

    // ����� ��� ������� ���� �����
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


    //����� ����������� ���� �������� ����
    private void ClearPools<T>(ObjectPoolStandart<T>[] pools) where T : Component, IPoolObject
    {
        if (pools == null) return;

        foreach (var pool in pools)
        {
            //���������� ��� ������� ����
            var objs = pool.GetObjects();
            foreach (var obj in objs)
            { 
                Destroy(obj.gameObject);
            }
            // ������� ���
            pool.ClearPool();
        }
    }


    //���������� ����� ����
    public void StopGeneration()
    {
        //��������� ���� ��������
        UpdateSpeed(0);
    }


    //������ ������� ��������� � ��������� ������
    public void ChunkGeneration()
    {
        //������� ������ ����, ���� �� �������
        if (startChunk != null && startChunk.EndPosition.position.z < -10)
            DeleteStartChunk();

        //���� ��������� ����� ������� ����� ���������� �� ������� 
        if (firstChunk != null && firstChunk.EndPosition.position.z < -10)
            DeleteOldChunk();
        //���� ��������� ����� ���������� ����� ��������� ����� ��� ����� 100 ������ �� ������, ��� ���� � �������
        if (lastChunk == null ||  lastChunk.EndPosition.position.z < 50)
            AddNewChunk();
    }


    //���������� ����� ����
    public void AddNewChunk()
    {
        //������� ��������� ����
        var chunkNum = Random.Range(0, chunkPools.Length);
        Chunk nextChunk = chunkPools[chunkNum].Get();

        //��������� ���������� ���� � ����� ����������� ��� �������� � ������ ���������
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

        //�������� ���� � �������
        currentChunks.Add(nextChunk);
        lastChunk = nextChunk;

        //�������� ��� ������ ����� ��������
        if(currentChunks.Count > 0)
            GenerateChunkObjects(lastChunk);
    }

    //������� ������ ���� �� ������
    public void DeleteOldChunk()
    {
        if (firstChunk != null)
        {
            //��������� ��� ������� �����
            DeleteChunkObjects(firstChunk);
            firstChunk.chunkCollectables.Clear();

            //������� ������ � ���
            var poolNum = firstChunk.poolNum;
            chunkPools[poolNum].Release(firstChunk);
            //Debug.Log("Chunk deleted: " + firstChunk.name);

            //������� ������ ������ �� ������
            currentChunks.RemoveAt(0);
            firstChunk = currentChunks.Count > 0 ? currentChunks[0] : null;
        }
    }

    //������� ������� ������ ���� ������
    public void DestroyCurrentChunk()
    {
        DeleteChunkObjectsAtTrack(currentChunks[0]);
        DeleteChunkObjectsAtTrack(currentChunks[1]);
    }


    //������� ���������� � ����������� ����� �������
    IEnumerator GenerationCheck()
    {
        while (true)
        {
            ChunkGeneration();
            yield return new WaitForSeconds(checkInterval);
        }
    }




    //��������� ��������......................................................................................................................................................................


    //�������� ���� ��������� � ���� ��������
    private void DeleteChunkObjectsAtTrack(Chunk chunk)
    {
        //������������ ������������� ���������
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
        //������������ ���� �������
        foreach (var chunkObj in chunk.chunkBarriers)
        {
            barrierPools[chunkObj.poolNum].Release(chunkObj);
        }

        //������� �������, ����� �� ����������� �������� ��� ��������� ��������
        chunk.chunkCollectables.Clear();
        chunk.chunkBarriers.Clear();
    }

    //������������ �������� �����
    private void DeleteChunkObjects(Chunk chunk)
    {
        //������������ ������������� ���������
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
        //������������ ���� �������
        foreach (var chunkObj in chunk.chunkBarriers)
        {
            barrierPools[chunkObj.poolNum].Release(chunkObj);
        }

        //������������ ����� ���������
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

        //������� �������, ����� �� ����������� �������� ��� ��������� ��������
        chunk.chunkCollectables.Clear();
        chunk.chunkBarriers.Clear();
        chunk.chunkDecorations.Clear();
    }

    //��������� ������ ��������
    private void GenerateChunkObjects(Chunk chunk)
    {
        bool isCreatingBarriers = false;
        if (currentChunks.Count > 1)
            isCreatingBarriers = true;

        //�������� ���������� ����� ���������
        var objectSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / (float)chunkObjectNum;
        Vector3 objectSpawnPosition = lastChunk.StartPosition.position;

        //��������� ��� �������� �����
        chunk.chunkCollectables = new List<Collectable>();

        if (isCreatingBarriers)
        {
            //� ����� � ���������� ������ ������� �� �����
            for (int r = 0; r < chunkObjectNum; r++)
            {
                //������ �� 3 ������ ������� ����������
                for (int f = 0; f < 3; f++)
                {
                    //�������� ������
                    var currentPossibility = Random.Range(0f, 100f);
                    //���� ���������� � ������ ������������, �� �������� ���������1 � ��������� �� �����
                    if (currentPossibility < coinSpawnPossibility * 100)
                    {
                        var coinNum = Random.Range(0, coinPools.Length);
                        var coinObj = coinPools[coinNum].Get();

                        //�������� ������� ����� �������� ��������
                        var yOffset = coinObj.transform.position.y - objectSpawnPosition.y;
                        //��������� ������ �� �����
                        coinObj.transform.position = objectSpawnPosition + (f - 1) * new Vector3(1, 0, 0) * gameSettings.lineRange + new Vector3(0, 1, 0) * yOffset;

                        //���������� �������� � ��� �����
                        chunk.chunkCollectables.Add(coinObj);
                    }
                    else if(currentPossibility < barrierPossibility * 100)
                    {
                        var barrierNum = Random.Range(0, barriers.Length);
                        var barrierObj = barrierPools[barrierNum].Get();


                        //�������� ������� ����� �������� ��������
                        var yOffset = barrierObj.transform.position.y - objectSpawnPosition.y;
                        //��������� ������ �� �����
                        barrierObj.transform.position = objectSpawnPosition + (f - 1) * new Vector3(1, 0, 0) * gameSettings.lineRange + new Vector3(0, 1, 0) * yOffset;

                        //���������� �������� � ��� �����
                        chunk.chunkBarriers.Add(barrierObj);
                    }


                    //�������� ��������
                    
                }

                objectSpawnPosition += new Vector3(0, 0, 1) * objectSpace;
            }
        }


        var grassSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / 5;
        objectSpawnPosition = lastChunk.StartPosition.position + Vector3.left * 4.4f + Vector3.forward * 2;
        //� ����� ���������� ����� ����� ����� � ������
        int sign = -1;
        for (int y = 0; y < 2; y++)
        {
            for (int r = 0; r < 5; r++)
            {
                //��������� ����� �����
                for (int m = 0; m < 3; m++)
                {
                    var grassObj = grassPools[0].Get();
                    //�������� ������� ����� �������� ��������
                    var yOffset = grassObj.transform.position.y - objectSpawnPosition.y;
                    grassObj.transform.position = objectSpawnPosition + sign * m * new Vector3(1, 0, 0) * grassSpace + new Vector3(0, 1, 0) * yOffset;

                    //���������� �������� � ��� �����
                    chunk.chunkDecorations.Add(grassObj);
                }

                objectSpawnPosition += new Vector3(0, 0, 1) * grassSpace;
            }

            sign = 1;
            objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * 4.4f + Vector3.forward * 2;
        }




        //� ����� ���������� ����� ������ ����� � ������
        var sidewalkSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / 10;
        objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * (4.4f + distanceToSidewalk) + Vector3.forward * 1 + Vector3.up * 0.2f;
        sign = -1;

        for (int r = 0; r < 10; r++)
        {
            //���������� ����
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

            //���������� ������
            for (int f = 0; f < 2; f++)
            {
                var sidewalkObj = sidewalkPools[0].Get();

                //������� ����� ��� � ��������
                if (lHouseSpawned && sign == -1)
                {
                    sidewalkPools[0].Release(sidewalkObj);
                    sidewalkObj = sidewalkPools[2].Get();
                    //������� ������� � ����
                    for (int c = 0; c < distanceToHouseInCells; c++)
                    {
                        var newCell = sidewalkPools[0].Get();
                        newCell.transform.position = objectSpawnPosition - Vector3.left * sidewalkSpace * c - Vector3.left * sidewalkSpace;
                        newCell.transform.rotation = Quaternion.Euler(-90, 180,0);
                        //���������� �������� � ��� �����
                        chunk.chunkDecorations.Add(newCell);
                    }

                    //������� ���
                    var houseNum = Random.Range(0, houses.Length);
                    var houseObj = housePools[houseNum].Get();
                    var housePos = objectSpawnPosition - Vector3.left * sidewalkSpace * distanceToHouseInCells - Vector3.left * sidewalkSpace;
                    houseObj.transform.position = housePos + Vector3.forward * sidewalkSpace / 2;
                    houseObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //���������� �������� � ��� �����
                    chunk.chunkDecorations.Add(houseObj);


                    //������ �����
                    /*var fencePos = housePos - Vector3.forward * sidewalkSpace * 1.5f + Vector3.up * 0.7f + Vector3.right * 3;
                    var fenceObj = fencePools[0].Get();
                    fenceObj.transform.position = fencePos;
                    chunk.chunkDecorations.Add(fenceObj);
*/
                }
                else if(!lHouseSpawned && sign == -1 && quantityOfCellInInterval - leftHouseCounter > 0)
                {
                    //������ �����
/*                    for (int countFence = 0; countFence < 2; countFence++)
                    {
                        var fencePos = objectSpawnPosition - Vector3.left * sidewalkSpace * distanceToHouseInCells - Vector3.left * sidewalkSpace;
                        fencePos = fencePos + (countFence - 1) * sidewalkSpace / 2 * Vector3.forward + Vector3.up * 0.7f + Vector3.right * 3;

                        var fenceObj = fencePools[0].Get();
                        fenceObj.transform.position = fencePos;
                        chunk.chunkDecorations.Add(fenceObj);
                    }*/
                }

                //������� ������ ��� � ��������
                if (rHouseSpawned && sign == 1)
                {
                    sidewalkPools[0].Release(sidewalkObj);
                    sidewalkObj = sidewalkPools[1].Get();
                    //������� ������� � ����
                    for (int c = 0; c < distanceToHouseInCells; c++)
                    {
                        var newCell = sidewalkPools[0].Get();
                        newCell.transform.position = objectSpawnPosition + Vector3.left * sidewalkSpace * c + Vector3.left * sidewalkSpace;
                        newCell.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        //���������� �������� � ��� �����
                        chunk.chunkDecorations.Add(newCell);
                    }

                    //������� ���
                    var houseNum = Random.Range(0, houses.Length);
                    var houseObj = housePools[houseNum].Get();
                    var housePos = objectSpawnPosition + Vector3.left * sidewalkSpace * distanceToHouseInCells + Vector3.left * sidewalkSpace;
                    housePos -= Vector3.forward * 1.5f * sidewalkSpace;
                    houseObj.transform.position = housePos + Vector3.forward * sidewalkSpace / 2;
                    houseObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                    //���������� �������� � ��� �����
                    chunk.chunkDecorations.Add(houseObj);

                    //������ �����
/*                    var fencePos = housePos - Vector3.forward * sidewalkSpace * 1.5f + Vector3.up * 0.7f - Vector3.right * 3 + Vector3.forward * 1.5f * sidewalkSpace;
                    var fenceObj = fencePools[0].Get();
                    fenceObj.transform.position = fencePos;
                    chunk.chunkDecorations.Add(fenceObj);*/
                }
                else if(!rHouseSpawned && sign == 1 && quantityOfCellInInterval - rightHouseCounter > 0)
                {
/*                    //������ �����
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

                //���������� �������� � ��� �����
                chunk.chunkDecorations.Add(sidewalkObj);



                //������� �����
                //�� �� ��������
                if (!(lHouseSpawned && sign == -1) && !(rHouseSpawned && sign == 1))
                {
                    var grassPos = objectSpawnPosition + sign * Vector3.left * sidewalkSpace;
                    //��������� ����� ����� ��������� � ������
                    var numberOfPlants = Random.Range(0, countOfBushes);
                    var numberOfTrees = Random.Range(0, countOfTrees);
                    //��������� �����
                    for (int u = 0; u < numberOfPlants; u++)
                    {
                        var xPos = Random.Range(grassPos.x, (grassPos + sign * Vector3.left * sidewalkSpace * distanceToHouseInCells).x);
                        var zPos = Random.Range((grassPos - Vector3.forward * sidewalkSpace / 2).z, (grassPos + Vector3.forward * sidewalkSpace / 2).z);

                        var bushNum = Random.Range(0, bushes.Length);
                        var bushObj = bushPools[bushNum].Get();
                        bushObj.transform.position = new Vector3(xPos, grassPos.y, zPos);
                        chunk.chunkDecorations.Add(bushObj);
                    }

                    //��������� �������
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


                //����������� ����� ����� �������
                sign = -sign;
                objectSpawnPosition = objectSpawnPosition + sign * Vector3.left * (4.4f + distanceToSidewalk) * 2;
            }

            //����������� ����� ������ �����
            objectSpawnPosition += new Vector3(0, 0, 1) * sidewalkSpace;

        }



        //������� ����� � �������
        objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * (4.4f + distanceToSidewalk) + Vector3.forward * 1 + Vector3.up * 0.2f - Vector3.left * sidewalkSpace;
        sign = -1;

        for (int y = 0; y < 10; y++)
        { 


            objectSpawnPosition += Vector3.forward * sidewalkSpace;
        }

        //�������� ������ ����� � ������
        var railingSpace = Mathf.Abs(lastChunk.EndPosition.position.z - lastChunk.StartPosition.position.z) / 30;
        objectSpawnPosition = lastChunk.StartPosition.position - Vector3.left * (4.4f + distanceToSidewalk - 1) + Vector3.forward * 1 - Vector3.up * 0.2f;

        for (int r = 0; r < 30; r++)
        {
            //���������� ������
            for (int f = 0; f < 2; f++)
            {
                var railingObj = railingPools[0].Get();
                var yOffset = railingObj.transform.position.y - objectSpawnPosition.y;
                railingObj.transform.position = objectSpawnPosition + new Vector3(0, 1, 0);

                //���������� �������� � ��� �����
                chunk.chunkDecorations.Add(railingObj);

                //����������� ����� ����� �������
                sign = -sign;
                objectSpawnPosition = objectSpawnPosition + sign * Vector3.left * (4.4f + distanceToSidewalk - 1) * 2;
            }

            //����������� ����� ������ �����
            objectSpawnPosition += new Vector3(0, 0, 1) * railingSpace;

        }
    }
    

    //��������� ������������� ������ �� ����� � ������� � ��� ��� ������������
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
