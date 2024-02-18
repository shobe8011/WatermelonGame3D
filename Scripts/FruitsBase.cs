using UnityEngine;

public class FruitsBase
{
    // フルーツの種類
    public GameManager.FruitsKinds fruitsKinds { get; private set; }

    // フルーツを拡大したときの大きさ
    public float fruitSize { get; private set; }

    // score
    public int score { get; private set; }

    // フルーツの名前
    public string fruitName { get; private set; }

    // フルーツのマテリアル
    public Material fruitMaterial { get; private set; } = null;

    public FruitsBase
    (
        GameManager.FruitsKinds fruitsKinds,
        float fruitSize,
        int score,
        string fruitName
    )
    {
        this.fruitsKinds = fruitsKinds;
        this.fruitSize = fruitSize;
        this.score = score;
        this.fruitName = fruitName;
    }

    public void SetMaterial(Material material)
    {
        this.fruitMaterial = material;
    }
}
