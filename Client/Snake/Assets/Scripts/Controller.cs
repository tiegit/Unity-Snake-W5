using Colyseus.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _cameraOffsetY = 30f;
    [SerializeField] private Transform _cursor;

    private MultiplayerManager _multiplayerManager;
    private PlayerAim _playerAim;
    private Player _player;
    private Snake _snake;
    private Camera _camera;
    private Plane _plane;
    private byte _skinIndex;

    public void Init(PlayerAim aim, Player player, Snake snake)
    {
        _multiplayerManager = MultiplayerManager.Instance;
        
        _playerAim = aim;
        _player = player;
        _snake = snake;
        _camera = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
        _skinIndex = _player.skin;

        _snake.gameObject.AddComponent<CameraManager>().Init(_cameraOffsetY);

        _player.OnChange += OnChange;
    }

    public void Destroy()
    {
        _player.OnChange -= OnChange;
        _snake.Destroy();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveCursor();
            _playerAim.SetTargetDirection(_cursor.position);
        }

        SendMove();
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _skinIndex++;
            if (_skinIndex >= _multiplayerManager.Skins.Length) _skinIndex = 0;
            
            ChangeSkin(_skinIndex);
            SendSckinChengeMessage(_skinIndex); // отпаравл€ем сообщение о смене индекса скина
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _skinIndex--;
            if (_skinIndex < 1) _skinIndex = (byte)_multiplayerManager.Skins.Length;
            
            ChangeSkin(_skinIndex);
            SendSckinChengeMessage(_skinIndex); // отпаравл€ем сообщение о смене индекса скина
        }
    }

    private void SendSckinChengeMessage(byte skinIndex)
    {
        _multiplayerManager.SendMessage("skin", _skinIndex);
    }

    private void SendMove() // подготовка сообщени€ и отправка данных координат игрока через менеджер
    {
        _playerAim.GetMoveInfo(out Vector3 position);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"x", position.x},
            {"z", position.z}
        };

        _multiplayerManager.SendMessage("move", data);
    }

    private void MoveCursor()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out float distance);
        Vector3 point = ray.GetPoint(distance);

        _cursor.position = point;
    }
    
    private void OnChange(List<DataChange> changes)
    {
        Vector3 position = _snake.transform.position;
        for (int i = 0; i < changes.Count; i++)
        {
            switch (changes[i].Field)
            {
                case "x":
                    position.x = (float)changes[i].Value;
                    break;
                case "z":
                    position.z = (float)changes[i].Value;
                    break;
                case "d":
                    _snake.SetDetailCount((byte)changes[i].Value);
                    break;
                case "skin": // принимаем от сервера индекс скина и примен€ем к врагу материал
                    _snake.SetHeadSkin(MultiplayerManager.Instance.Skins.GetMaterial((byte)changes[i].Value));
                    break;
                default:
                    Debug.LogWarning("Ќе обрабатываетс€ изменение пол€ " + changes[i].Field);
                    break;
            }
        }

        _snake.SetRotation(position);
    }

    private void ChangeSkin(byte skinIndex)
    {
        _snake.SetHeadSkin(_multiplayerManager.Skins.GetMaterial(skinIndex));
    }

}
