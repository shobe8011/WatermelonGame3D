using UnityEngine;

public class FruitsBase
{
    // �t���[�c�̎��
    public GameManager.FruitsKinds fruitsKinds { get; private set; }

    // �t���[�c���g�債���Ƃ��̑傫��
    public float fruitSize { get; private set; }

    // score
    public int score { get; private set; }

    // �t���[�c�̖��O
    public string fruitName { get; private set; }

    // �t���[�c�̃}�e���A��
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
