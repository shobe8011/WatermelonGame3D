using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class InitializeFruits
{
    // マテリアルにアクセスるときのパス
    private readonly int fruitsSpecies = 11;
    private readonly string headMaterialPass = "Material_";
    private bool EndInitializeList = false;
    private List<FruitsBase> fruitsBase = new List<FruitsBase>();

    async void Start()
    {
        //await SetFruitsMaterial();
    }

    // 
    public async Task InitializeList()
    {
        if (EndInitializeList) return;
        // ゲーム開始時に全てのフルーツのデータを初期化
        var cherry = new FruitsBase(GameManager.FruitsKinds.cherry, 20.0f, 5, "Cherry");
        fruitsBase.Add(cherry);

        var strawberry = new FruitsBase(GameManager.FruitsKinds.strawberry, 30.0f, 10, "StrawBerry");
        fruitsBase.Add(strawberry);

        var grape = new FruitsBase(GameManager.FruitsKinds.grape, 45.0f, 15, "Grape");
        fruitsBase.Add(grape);

        var dekopon = new FruitsBase(GameManager.FruitsKinds.dekopon, 60.0f, 20, "Dekopon");
        fruitsBase.Add(dekopon);

        var persimmon = new FruitsBase(GameManager.FruitsKinds.persimmon, 80.0f, 25, "Persimmon");
        fruitsBase.Add(persimmon);

        var apple = new FruitsBase(GameManager.FruitsKinds.apple, 100.0f, 30, "Apple");
        fruitsBase.Add(apple);

        var pear = new FruitsBase(GameManager.FruitsKinds.pear, 120.0f, 40, "Pear");
        fruitsBase.Add(pear);

        var peach = new FruitsBase(GameManager.FruitsKinds.peach, 150.0f, 50, "Peach");
        fruitsBase.Add(peach);

        var pineapple = new FruitsBase(GameManager.FruitsKinds.pinapple, 180.0f, 60, "Pineapple");
        fruitsBase.Add(pineapple);

        var melon = new FruitsBase(GameManager.FruitsKinds.melon, 210.0f, 70, "Melon");
        fruitsBase.Add(melon);

        var watermelon = new FruitsBase(GameManager.FruitsKinds.watermelon, 240.0f, 80, "Watermelon");
        fruitsBase.Add(watermelon);

        EndInitializeList = true;
    }

    // フルーツのマテリアルを取得
    public async UniTask SetFruitsMaterial()
    {
        await InitializeList();
        await UniTask.WaitUntil(() => EndInitializeList);       // フラグが立つまで待つ

        for (int i = 0; i < fruitsSpecies; i++)
        {
            string fruitMaterialPass = headMaterialPass + fruitsBase[i].fruitName;
            bool complete = false;
            Material material = null;

            // ロードする
            Addressables.LoadAssetAsync<Material>(fruitMaterialPass).Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.LogError("マテリアルの取得に失敗しました");
                    return;
                }
                material = handle.Result;
                complete = true;
            };

            await UniTask.WaitUntil(() => complete == true);
            fruitsBase[i].SetMaterial(material);
            if (fruitsBase[i].fruitMaterial == null) Debug.LogError(fruitsBase[i].fruitName +" : マテリアルの取得に失敗しました。");
        }
    }

    // フルーツのマテリアルを返す
    public async UniTask<Material> GetFruitMaterial(int fruitNumber)
    {
        var fruitMaterial = fruitsBase[fruitNumber].fruitMaterial;
        // もしまだマテリアルをセットしていなかったらここでロードして渡す
        if(fruitMaterial == null)
        {
            bool complete = false;
            string fruitMaterialPass = headMaterialPass + fruitsBase[fruitNumber].fruitName;
            Material material = null;
            // ロードする
            Addressables.LoadAssetAsync<Material>(fruitMaterialPass).Completed += handle =>
            {
                if (handle.Result == null)
                {
                    Debug.LogError(fruitsBase[fruitNumber].fruitName + " : マテリアルの取得に失敗しました");
                    return;
                }
                material = handle.Result;
                fruitsBase[fruitNumber].SetMaterial(fruitMaterial);
                complete = true;
            };
            await UniTask.WaitUntil(() => complete == true);
            fruitMaterial = material;
        }
        return fruitMaterial;                   // そのまま"Unity.Material"を返すことができない。(asyncを付けてないとき)　
        //return fruitMaterial as Material;       // 'UnityEngine.Material' を 'Cysharp.Threading.Tasks.UniTask<UnityEngine.Material>' に変換できません

        //return UniTask.FromResult(fruitMaterial);
    }
    // やってることは↑と一緒。別の書き方
    //public UniTask<Material> GetFruitmaterial(GameManager.FruitsKinds fruitskinds)
    //{
    //    var fruitMaterial = fruitsBase[(int)fruitskinds].fruitMaterial;
    //    if (fruitMaterial != null)
    //    {
    //        return UniTask.FromResult(fruitMaterial);
    //    }
    //    // Addressables.LoadAssetAsync<Material> の結果を UniTask<Material> に変換
    //    return Addressables.LoadAssetAsync<Material>(fruitsBase[(int)fruitskinds].materialPass).ToUniTask().ContinueWith(material =>
    //    {
    //        if (material == null)
    //        {
    //            Debug.LogError("マテリアルの取得に失敗しました");
    //            return null;
    //        }
    //        fruitMaterial = material;
    //        fruitsBase[(int)fruitskinds].SetMaterial(fruitMaterial);
    //        return fruitMaterial;
    //    });
    //}

    /// <summary>
    /// リストの要素を渡す
    /// </summary>
    /// <param name="fruitNumber"></param>
    /// <returns></returns>
    public async UniTask<Vector3> GetFruitSize(int fruitNumber)
    {
        if(!EndInitializeList)
        {
            await InitializeList();
        }
        Vector3 fruitSize = new Vector3(fruitsBase[fruitNumber].fruitSize, fruitsBase[fruitNumber].fruitSize, fruitsBase[fruitNumber].fruitSize);
        return fruitSize;
    }

    public GameManager.FruitsKinds GetFruitKind(int fruitNumber)
    {
        return fruitsBase[fruitNumber].fruitsKinds;
    }

    public string GetFruitName(int fruitNumber)
    {
        return fruitsBase[fruitNumber].fruitName;
    }
}
