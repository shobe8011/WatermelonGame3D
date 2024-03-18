using UnityEngine;

public class CollisionFruit : MonoBehaviour
{
    [SerializeField] private GameManager.FruitsKinds _fruitsKinds = GameManager.FruitsKinds.none;
    public GameManager.FruitsKinds GetFruitKind { get { return _fruitsKinds; } }

    private int _collisionWallCount = 0;
    private bool _collision = false;

    public void SetFruitKind(GameManager.FruitsKinds fruitKind)
    {
        _fruitsKinds = fruitKind;
    }


    private void OnCollisionEnter(Collision collision)
    {
        var collisionFruitKind = collision.gameObject.GetComponent<CollisionFruit>();
        if(collision.gameObject.tag == "Fruits")
        {
            _collision = true;

            if (_fruitsKinds == collisionFruitKind.GetFruitKind)
            {
                // ��̃I�u�W�F�N�g�Ɏ��̃t���[�c�𐶐�����悤�ɒʒm���΂�
                var parent = this.transform.parent;
                var spawnFruit = parent.transform.parent.GetComponent<SpawnFruits>();
                spawnFruit.CalcurationHalfPoint(_fruitsKinds, this.transform.position);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���ڂ̕ǂƂ̐ڐG�̓X���[
        if(other.gameObject.tag == "Wall")
        {
            _collisionWallCount++;
            if(_collisionWallCount > 1 || _collision)
            {
                var parentObj = this.gameObject.transform.parent.gameObject;
                var gameManager = parentObj.transform.parent.gameObject.GetComponent<GameManager>();
                gameManager.GameOver();
            }
        }
    }
}
