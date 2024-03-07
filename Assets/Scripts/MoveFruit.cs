using UnityEngine;

public class MoveFruit : MonoBehaviour
{
    private GameObject _fallFruit = null;
    private float _moveSpeed = 1.0f;


    /// <summary>
    /// Ÿ‚É—‚Æ‚·ƒtƒ‹[ƒc‚ğˆÚ“®‚³‚¹‚é
    /// </summary>
    /// <param name="isRight"></param>
    public void MoveNextFruitPositionX(bool isRight)
    {
        if (_fallFruit == null) return;
        var moveLength = new Vector3(_moveSpeed, 0.0f, 0.0f);
        _fallFruit.transform.position += isRight ? moveLength : -moveLength;
    }

    public void MoveNextFruitPositionZ(bool isfront)
    {
        if (_fallFruit == null) return;
        var moveLength = new Vector3(0.0f, 0.0f, _moveSpeed);
        _fallFruit.transform.position += isfront ? -moveLength : moveLength;
    }

    public void SetFallFruit(GameObject fruit)
    {
        _fallFruit = fruit;
    }
}
