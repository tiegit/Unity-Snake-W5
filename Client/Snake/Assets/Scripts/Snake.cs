using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Snake : MonoBehaviour
{
    public float Speed { get { return _speed; } }

    [SerializeField] private Tail _tailPrefab;
    [SerializeField] private Transform _head;
    [SerializeField] private float _speed = 2f;

    private Tail _tail;

    public void Init(int detailCount, Material skinMaterial)
    {
        _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        _tail.Init(_head, _speed, detailCount);

        SetHeadSkin(skinMaterial); // красим голову и передаем цвет хвосту и детялям
    }

    public void SetDetailCount(int detailCount)
    {
        _tail.SetDetailCount(detailCount);
    }

    public void Destroy()
    {
        _tail.Destroy();
        Destroy(gameObject);
    }

    public void SetRotation(Vector3 pointToLook)
    {
        _head.LookAt(pointToLook);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += _head.forward * Time.deltaTime * _speed;
    }

    public void SetHeadSkin(Material skinMaterial)
    {
        GetComponent<SetSkin>().SetMaterial(skinMaterial); // красим голову 
        _tail.SetTailAndDetailsSkin(skinMaterial); // передаем цвет хвосту и детялям
    }
}
