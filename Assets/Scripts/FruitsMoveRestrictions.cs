using UnityEngine;

public class FruitsMoveRestrictions : MonoBehaviour
{
    [SerializeField] private SpawnFruits _spawnFruits;

    /// <summary>
    /// フルーツが端にぶつかったら強制的に真ん中に戻す
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Fruits")
        {
            _spawnFruits.SetCenterPosition();
        }
    }
}
