using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveWall : MonoBehaviour
{
    private Tweener _shakeTweener;
    private Vector3 _firstPosition = new Vector3(0.0f, 0.0f, 0.0f);

    //TODO:調整が終わったら[serializeField]を外す
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float strength = 10.0f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float randomness = 45.0f;
    [SerializeField] private bool fadeOut = true;

    private readonly float MAX_rotate = 15.0f;
    private float _floatAngleX = 0.0f;
    private float _floatAngleZ = 0.0f;
    private float _rotationSpeed = 45.0f;
    private bool _canMove = false;

    /// <summary>
    /// 箱を揺らす
    /// </summary>
    public void ShakeWalls()
    {
        if (!_canMove) return;
        // 前の処理が残って入れば初期位置に戻す
        if (_shakeTweener != null)
        {
            _shakeTweener.Kill();
            gameObject.transform.position = _firstPosition;
        }

        // 揺らす
        _shakeTweener = this.transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
    }

    /// <summary>
    /// 箱の回転
    /// </summary>
    /// <param name="isright"></param>
    public void RotateWallX(bool isright)
    {
        if (!_canMove) return;
        float angle = _rotationSpeed * Time.deltaTime;
        // TODO:角度を直接指定するのを辞める
        // 壁の中心座標
        var wallPos = new Vector3(0,0,450);
        if (isright)
        {
            _floatAngleX += angle;
            if(_floatAngleX <= MAX_rotate)
            {
                this.transform.RotateAround(wallPos, Vector3.right, angle);
            }
            else
            {
                _floatAngleX = MAX_rotate;
            }
        }
        else if (!isright)
        {
            if (!_canMove) return;
            _floatAngleX -= angle;
            if(-MAX_rotate <= _floatAngleX)
            {
                this.transform.RotateAround(wallPos, Vector3.left, angle);
            }
            else
            {
                _floatAngleX = -MAX_rotate;
            }
        }
    }

    public void RotateWallZ(bool isright)
    {
        if (!_canMove) return;
        float angle = _rotationSpeed * Time.deltaTime;
        // TODO:角度を直接指定するのを辞める
        // 壁の中心座標
        var wallPos = new Vector3(0, 0, 450);
        if (isright)
        {
            _floatAngleZ += angle;
            if (_floatAngleZ <= MAX_rotate)
            {
                this.transform.RotateAround(wallPos, Vector3.forward, angle);
            }
            else
            {
                _floatAngleZ = MAX_rotate;
            }
        }
        else if (!isright)
        {
            _floatAngleZ -= angle;
            if (-MAX_rotate <= _floatAngleZ)
            {
                this.transform.RotateAround(wallPos, Vector3.back, angle);
            }
            else
            {
                _floatAngleZ = -MAX_rotate;
            }
        }
    }

    /// <summary>
    /// 箱を動かせるかのフラグを受け取る
    /// </summary>
    /// <param name="canMove"></param>
    public void SetCanFallFlag(bool canMove)
    {
        _canMove = canMove;
    }

    /// <summary>
    /// 箱の座標、角度の初期化
    /// </summary>
    public void InitializeWall()
    {
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
