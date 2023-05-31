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
        string name = "";
        _playerVehicleController = target.GetComponent<PlayerVehicleController>();

        GameManager.instance.ScoreUp(5);
        this._meshRenderer.enabled = false;
        this._boxCollider.enabled = false;
        
        switch (this.itemType) {
            case ItemType.Shield :
                name = "Shield";
                StartCoroutine(nameof(Shield));
                break;
            case ItemType.SpeedUp :
                name = "Speed Up";
                StartCoroutine(nameof(SpeedUp));
                break;
        }

        GameUIManager.instance.StartCoroutine(nameof(GameUIManager.ItemGet), name);
    }

    private IEnumerator Shield() {
        _playerVehicleController.IsShield = true;
        _playerVehicleController.ItemShieldParticle.Play();
        
        yield return new WaitForSeconds(10f);
        
        _playerVehicleController.ItemShieldParticle.Stop();
        _playerVehicleController.IsShield = false;
        
        ItemSpawner.instance.ItemReSpawn(gameObject);
    }

    private IEnumerator SpeedUp() {
        _playerVehicleController.IsSpeedUp = true;
        _playerVehicleController.ItemSpeedUpParticle.Play();

        _playerVehicleController.MoveSpeedMultiplier = 1.3f;
        _playerVehicleController.TurnSpeedMultiplier = 1.3f;
        
        yield return new WaitForSeconds(5f);
        
        _playerVehicleController.MoveSpeedMultiplier = 1f;
        _playerVehicleController.TurnSpeedMultiplier = 1f;

        _playerVehicleController.ItemSpeedUpParticle.Stop();
        _playerVehicleController.IsSpeedUp = false;
        
        ItemSpawner.instance.ItemReSpawn(gameObject);
    }
}