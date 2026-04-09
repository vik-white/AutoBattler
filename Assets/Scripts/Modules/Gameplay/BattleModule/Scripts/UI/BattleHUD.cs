using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using vikwhite.ECS;
using Time = UnityEngine.Time;

public class BattleHUD : MonoBehaviour
{
    public Text FPS;

    private EntityManager _entityManager;
    private EntityQuery _createCharacterQuery;
    
    
    public static void Show()
    {
        var canvas = FindAnyObjectByType<Canvas>().transform;
        var hud = Resources.Load<GameObject>("UI/BattleHUD");
        Instantiate(hud, canvas).GetComponent<BattleHUD>().Initialize();
    }

    public static void Hide()
    {
        Destroy(FindAnyObjectByType<BattleHUD>().gameObject);
    }

    private void Initialize()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _createCharacterQuery = _entityManager.CreateEntityQuery(typeof(CreateCharacter));
    }
    
    private void Update()
    {
        FPS.text = $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";
        foreach (var createCharacter in _createCharacterQuery.ToEntityArray(Allocator.Temp))
        {
            OnCreateCharacter();
        }
    }

    private void OnCreateCharacter()
    {
        //Debug.Log("Creating Character");
    }
}