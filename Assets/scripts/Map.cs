
using UnityEngine;
using System.Collections;

public enum MapDirection {MD_HORIZONTAL, MD_VERTICAL}

public class Map : MonoBehaviour
{
    private const float _CORNERS_OFFSET = 5;
    
    /**
     * Контейнер для всех неподвижных элементов.
     */
    public Transform staticObjectsRoot;
    private int _mapWidth;
    private int _mapHeight;
    private MapDirection _mapDirection;
    
    /**
     * 
     */
    void Start()
    {
        initialize();
    }
    
    public void initialize()
    {
        loadMapInfo();
        initBackground();
    }
    
    private void loadMapInfo()
    {
        _mapWidth     = 500;
        _mapHeight    = 600;
        _mapDirection = MapDirection.MD_VERTICAL;
    }
    
    private void initBackground()
    {
        Transform root = staticObjectsRoot;
        
        Transform obj;
        Rect size;
        
        obj = root.FindChild("background_panel/fill_background");
        obj.localPosition = new Vector3(-100, -100, 0);
        obj.localScale = new Vector3(_mapWidth + 200, _mapHeight + 200, 1);
        
        obj = root.FindChild("background_panel/background_root");
        obj.localPosition = new Vector3(_mapWidth * 0.5f, _mapHeight * 0.5f, 0);
        
        if (_mapDirection == MapDirection.MD_HORIZONTAL) {
            obj.localRotation = Quaternion.Euler(0, 0, 0);
            tiledScale(obj, obj.GetChild(0), _mapWidth, _mapHeight, 1, 5);
        } else {
            obj.localRotation = Quaternion.Euler(0, 0, -90);
            tiledScale(obj, obj.GetChild(0), _mapHeight, _mapWidth, 1, 1);
        }
        
        obj = root.FindChild("background_panel/corners");
        size = obj.GetComponent<UISprite>().sprite.outer;
        obj.localPosition = new Vector3(-size.width * 0.5f, -size.height * 0.5f, 0);
        obj.localScale    = new Vector3(_mapWidth + size.width, _mapHeight + size.height, 1);
        
        obj = root.FindChild("background_panel/stroke_b_root");
        obj.localPosition = new Vector3(_mapWidth * 0.5f, 0, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapWidth, 1);
        
        obj = root.FindChild("background_panel/stroke_t_root");
        obj.localPosition = new Vector3(_mapWidth * 0.5f, _mapHeight, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapWidth, 1);
        
        obj = root.FindChild("background_panel/stroke_l_root");
        obj.localPosition = new Vector3(0, _mapHeight * 0.5f, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapHeight, 1);
        
        obj = root.FindChild("background_panel/stroke_r_root");
        obj.localPosition = new Vector3(_mapWidth, _mapHeight * 0.5f, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapHeight, 1);
        
        // Design
        obj = root.FindChild("background_panel/design_corner_lb");
        
        Rect cornerRect = obj.GetComponent<UISprite>().sprite.outer;
        
        obj.localPosition = new Vector3(_CORNERS_OFFSET, _CORNERS_OFFSET, 0);
        
        obj = root.FindChild("background_panel/design_corner_lt");
        obj.localPosition = new Vector3(_CORNERS_OFFSET, _mapHeight - _CORNERS_OFFSET, 0);
        
        obj = root.FindChild("background_panel/design_corner_rt");
        obj.localPosition = new Vector3(_mapWidth - _CORNERS_OFFSET, _mapHeight - _CORNERS_OFFSET, 0);
        
        obj = root.FindChild("background_panel/design_corner_rb");
        obj.localPosition = new Vector3(_mapWidth - _CORNERS_OFFSET, _CORNERS_OFFSET, 0);
        
        obj = root.FindChild("background_panel/design_line_b_root");
        obj.localPosition = new Vector3(_mapWidth * 0.5f, _CORNERS_OFFSET + cornerRect.height * 0.5f - 8, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapWidth - 2 * _CORNERS_OFFSET - 2 * cornerRect.width + 3, 1);
        
        obj = root.FindChild("background_panel/design_line_t_root");
        obj.localPosition = new Vector3(_mapWidth * 0.5f, _mapHeight - _CORNERS_OFFSET - cornerRect.height * 0.5f + 8, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapWidth - 2 * _CORNERS_OFFSET - 2 * cornerRect.width + 3, 1);
        
        obj = root.FindChild("background_panel/design_line_l_root");
        obj.localPosition = new Vector3(_CORNERS_OFFSET + cornerRect.width * 0.5f - 8, _mapHeight * 0.5f, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapHeight - 2 * _CORNERS_OFFSET - 2 * cornerRect.height + 3, 1);
        
        obj = root.FindChild("background_panel/design_line_r_root");
        obj.localPosition = new Vector3(_mapWidth - _CORNERS_OFFSET - cornerRect.width * 0.5f + 8, _mapHeight * 0.5f, 0);
        tiledScaleX(obj, obj.GetChild(0), _mapHeight - 2 * _CORNERS_OFFSET - 2 * cornerRect.height + 3, 1);
        
    }
    
    private void tiledScale(Transform parent, Transform sprite, float width, float height, int tileX, int tileY)
    {
        UITiledSprite tiledSprite = sprite.GetComponent<UITiledSprite>();
        float realWidth  = tiledSprite.sprite.outer.width;
        float realHeight = tiledSprite.sprite.outer.height;
        
        sprite.localScale = new Vector3(realWidth * tileX, realHeight * tileY, 1);
        parent.localScale = new Vector3(width / sprite.localScale.x, height / sprite.localScale.y, 1);
    }
    
    private void tiledScaleX(Transform parent, Transform sprite, float width, int tileX)
    {
        UITiledSprite tiledSprite = sprite.GetComponent<UITiledSprite>();
        float realWidth  = tiledSprite.sprite.outer.width;
        
        sprite.localScale = new Vector3(realWidth * tileX, sprite.localScale.y, 1);
        parent.localScale = new Vector3(width / sprite.localScale.x, parent.localScale.y, 1);
    }
    
    private void tiledScaleY(Transform parent, Transform sprite, float height, int tileY)
    {
        UITiledSprite tiledSprite = sprite.GetComponent<UITiledSprite>();
        float realHeight = tiledSprite.sprite.outer.height;
        
        sprite.localScale = new Vector3(sprite.localScale.x, realHeight * tileY, 1);
        parent.localScale = new Vector3(parent.localScale.x, height / sprite.localScale.y, 1);
    }
}
