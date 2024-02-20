using UnityEngine;

public class CollisionFruit : MonoBehaviour
{
    [SerializeField] private GameManager.FruitsKinds _fruitsKinds = GameManager.FruitsKinds.none;
    public GameManager.FruitsKinds GetFruitKind { get { return _fruitsKinds; } }

    public void SetFruitKind(GameManager.FruitsKinds fruitKind)
    {
        _fruitsKinds = fruitKind;
    }


    private void OnCollisionEnter(Collision collision)
    {
        var collisionFruitKind = collision.gameObject.GetComponent<CollisionFruit>();
        if(collisionFruitKind == null) return;
        if (_fruitsKinds == collisionFruitKind.GetFruitKind)
        {
            // 上のオブジェクトに次のフルーツを生成するように通知を飛ばす
            var parent = this.transform.parent;
            var spawnFruit = parent.transform.parent.GetComponent<SpawnFruits>();
            spawnFruit.CalcurationHalfPoint(_fruitsKinds, this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
