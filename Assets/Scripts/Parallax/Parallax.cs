using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 parallayEffectMultiplier;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUniteSizeX;
    private float textureUniteSizeY;

    private void Start(){
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUniteSizeX = texture.width / sprite.pixelsPerUnit;
        textureUniteSizeY = texture.height / sprite.pixelsPerUnit;
    }

    private void LateUpdate() {
        Vector3 deltaMocement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMocement.x * parallayEffectMultiplier.x, deltaMocement.y * parallayEffectMultiplier.y);
        lastCameraPosition = cameraTransform.position;

        if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUniteSizeX){
            float offesetPositionX = (cameraTransform.position.x - transform.position.x) % textureUniteSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offesetPositionX, transform.position.y);
        }
        if(Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUniteSizeY){
            float offesetPositionY = (cameraTransform.position.y - transform.position.y) % textureUniteSizeY;
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offesetPositionY);
        }
    }
}
