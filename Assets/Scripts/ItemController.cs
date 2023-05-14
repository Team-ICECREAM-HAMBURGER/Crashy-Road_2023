using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    enum ItemType {
        Shield,
        SpeedUp,
    }

    [SerializeField] private ItemType itemType;

    private PlayerVehicleController _playerVehicleController;
    private BoxCollider _boxCollider;
    private MeshRenderer _meshRenderer;
    
    
    private void Init() {
        this.itemType = (ItemType)Random.Range(0, 2);
        this._meshRenderer = GetComponent<MeshRenderer>();
        this._boxCollider = GetComponent<BoxCollider>();
        this._meshRenderer.enabled = true;
        this._boxCollider.enabled = true;
    }

    private void OnEnable() {
        Init();
    }
    
    public void Use(GameObject target) {
        _playerVehicleController = target.GetComponent<PlayerVehicleController>();

        GameManager.instance.ScoreUp(5);
        this._meshRenderer.enabled = false;
        this._boxCollider.enabled = false;
        
        switch (this.itemType) {
            case ItemType.Shield :
                StartCoroutine(nameof(Shield));
                break;
            case ItemType.SpeedUp :
                StartCoroutine(nameof(SpeedUp));
                break;
        }
    }

    private IEnumerator Shield() {
        Debug.Log("Shield Item Get!");
        
        _playerVehicleController.IsShield = true;
        
        // TODO: Shield VFX/SFX Play
        
        yield return new WaitForSeconds(5f);

        // TODO: Shield VFX/SFX Stop
        
        _playerVehicleController.IsShield = false;
        ItemSpawner.instance.ItemReSpawn(gameObject);
    }

    private IEnumerator SpeedUp() {
        Debug.Log("Speed Up Item Get!");
        
        _playerVehicleController.IsSpeedUp = true;

        _playerVehicleController.MoveSpeedMultiplier = 1.3f;
        _playerVehicleController.TurnSpeedMultiplier = 1.3f;
        
        // TODO: Speed Up VFX/SFX Play
        
        yield return new WaitForSeconds(5f);
        
        // TODO: Speed Up VFX/SFX Stop
        
        _playerVehicleController.MoveSpeedMultiplier = 1f;
        _playerVehicleController.TurnSpeedMultiplier = 1f;

        _playerVehicleController.IsSpeedUp = false;
        ItemSpawner.instance.ItemReSpawn(gameObject);
    }
}