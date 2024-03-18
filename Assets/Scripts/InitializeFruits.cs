using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class InitializeFruits : MonoBehaviour
{
    // マテリアルにアクセスるときのパス
    private readonly int fruitsSpecies = 9;
    private readonly string headMaterialPass = "Material_";
    private bool EndInitializeList = false;
    private List<FruitsBase> fruitsBase = new List<FruitsBase>();


    [SerializeField] List<Material> _materials;

    /// <summary>
    /// 果物の構造体を定義
    /// </summary>
    /// <returns></returns>
    public async Task InitializeList()
    {
        if (EndInitializeList) return;

        var grape = new FruitsBase(GameManager.FruitsKinds.grape, 45.0f, 15, "Grape", _materials[0]);
        fruitsBase.Add(grape);

        var dekopon = new FruitsBase(GameManager.FruitsKinds.dekopon, 60.0f, 20, "Dekopon", _materials[1]);
        fruitsBase.Add(dekopon);

        var persimmon = new FruitsBase(GameManager.FruitsKinds.persimmon, 80.0f, 25, "Persimmon", _materials[2]);
        fruitsBase.Add(persimmon);

        var apple = new FruitsBase(GameManager.FruitsKinds.apple, 100.0f, 30, "Apple", _materials[3]);
        fruitsBase.Add(apple);

        var pear = new FruitsBase(GameManager.FruitsKinds.pear, 120.0f, 40, "Pear", _materials[4]);
        fruitsBase.Add(pear);

        var peach = new FruitsBase(GameManager.FruitsKinds.peach, 150.0f, 50, "Peach", _materials[5]);
        fruitsBase.Add(peach);

        var pineapple = new FruitsBase(GameManager.FruitsKinds.pinapple, 180.0f, 60, "Pineapple", _materials[6]);
        fruitsBase.Add(pineapple);

        var melon = new FruitsBase(GameManager.FruitsKinds.melon, 210.0f, 70, "Melon", _materials[7]);
        fruitsBase.Add(melon);

        var watermelon = new FruitsBase(GameManager.FruitsKinds.watermelon, 240.0f, 80, "Watermelon", _materials[8]);
        fruitsBase.Add(watermelon);

        EndInitializeList = true;
    }

    /// <summary>
    /// フルーツのマテリアルを取得
    /// </summary>
    /// <returns></returns>
    public async UniTask SetFruitsMaterial()
    {
        await InitializeList();
        await UniTask.WaitUntil(() => EndInitializeList);

        for (int i = 0; i < fruitsSpecies; i++)
        {
            string fruitMaterialPass = headMaterialPass + fruitsBase[i].fruitName;
            bool complete = false;
            Material material = null;

            // ロードする
            //Addressables.LoadAssetAsync<Material>(fruitMaterialPass).Completed += handle =>
            //{
            //    if (handle.Result == null)
            //    {
            //        Debug.LogError("マテリアルの取得に失敗しました");
            //        return;
            //    }
            //    material = handle.Result;
            //    complete = true;
            //};

            // ロードが完了するまで待つ
            //await UniTask.WaitUntil(() => complete == true);
            //fruitsBase[i].SetMaterial(material);
        }
    }

    // フルーツのマテリアルを返す
    public async UniTask<Material> GetFruitMaterial(int fruitNumber)
    {
        var fruitMaterial = fruitsBase[fruitNumber].fruitMaterial;
        // もしまだマテリアルをセットしていなかったらここでロードして渡す
        if (fruitMaterial == null)
        {
            bool complete = false;
            string fruitMaterialPass = headMaterialPass + fruitsBase[fruitNumber].fruitName;
            Material material = null;
            // ロードする
            //Addressables.LoadAssetAsync<Material>(fruitMaterialPass).Completed += handle =>
            //{
            //    if (handle.Result == null)
            //    {
            //        Debug.LogError(fruitsBase[fruitNumber].fruitName + " : マテリアルの取得に失敗しました");
            //        return;
            //    }
            //    material = handle.Result;
            //    fruitsBase[fruitNumber].SetMaterial(fruitMaterial);
            //    complete = true;
            //};
            //await UniTask.WaitUntil(() => complete == true);
            fruitMaterial = material;
        }
        return fruitMaterial;
    }
    
    public FruitsBase GetFruitsBase(int fruitNumber)
    {
        return fruitsBase[fruitNumber];
    }

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

    /// <summary>
    /// フルーツの種類を返す
    /// </summary>
    /// <param name="fruitNumber"></param>
    /// <returns></returns>
    public GameManager.FruitsKinds GetFruitKind(int fruitNumber)
    {
        return fruitsBase[fruitNumber].fruitsKinds;
    }

    /// <summary>
    /// フルーツの番号を返す
    /// </summary>
    /// <param name="fruitNumber"></param>
    /// <returns></returns>
    public string GetFruitName(int fruitNumber)
    {
        return fruitsBase[fruitNumber].fruitName;
    }

    /// <summary>
    /// 同じ種類のフルーツが当たったときのスコアを渡す
    /// </summary>
    /// <param name="fruitNumber"></param>
    /// <returns></returns>
    public int GetFruitScore(int fruitNumber)
    {
        return fruitsBase[fruitNumber].score;
    }
}
