using UnityEngine;
using System.Collections;

enum ExplodeState
{
    ES_READY,
    ES_EXPLODING
}

/**
 * 
 */
public class LinesExploder
{
    private ExplodeState _state;
    private GameObject _uiRoot;
    private GameObject _scorePrefab;
    
    /**
     * Конструктор.
     */
    public LinesExploder(GameObject uiRoot)
    {
        _uiRoot = uiRoot;
        _state = ExplodeState.ES_READY;
        _scorePrefab = Resources.Load<GameObject>("prefabs/scoreLabel");
    }
    
    public void start(SwapResult swapResult)
    {
        int i;
        int j;
        
        if (swapResult.lines.Count > 0) {
            for (i = 0; i < swapResult.lines.Count; i++) {
                Match line = swapResult.lines[i];
                
                if (line.Count > 0) {
                    Vector3 lineCenter = getLineCenter(line);
                    
                    for (j = 0; j < line.Count; j++) {
                        line[j].explode(null);
                    }
                    
                    GameObject scoreLabel = (GameObject)UnityEngine.Object.Instantiate(_scorePrefab);
                    scoreLabel.transform.parent   = _uiRoot.transform;
                    scoreLabel.transform.position = lineCenter;
                    scoreLabel.GetComponent<UILabel>().text = "" + line.Count * 10;
                    
                    
                }
            }
            
            //_state = ExplodeState.ES_EXPLODING;
        }
    }
    
    public void step(float deltaTime)
    {
        
    }
    
    /**
     * Возвращает центр линии(списка ячеек).
     */
    private Vector3 getLineCenter(Match line)
    {
        Vector3 ltPos = new Vector3(line[0].transform.position.x, line[0].transform.position.y, 0);
        Vector3 rbPos = new Vector3(ltPos.x, ltPos.y, 0);
        
        for (int i = 0; i < line.Count; i++) {
            if (line[i].transform.position.x < ltPos.x) {
                ltPos.x = line[i].transform.position.x;
            } else
            if (line[i].transform.position.x > rbPos.x) {
                rbPos.x = line[i].transform.position.x;
            }
            
            if (line[i].transform.position.y < ltPos.y) {
                ltPos.y = line[i].transform.position.y;
            } else
            if (line[i].transform.position.y > rbPos.y) {
                rbPos.y = line[i].transform.position.y;
            }
        }
        
        ltPos.x = (ltPos.x + rbPos.x) / 2;
        ltPos.y = (ltPos.y + rbPos.y) / 2;
        
        return ltPos;
    }
    
    /**
     * Взрываются ли сейчас линии.
     * 
     * @return bool если взрываются линии, возаращается true, иначе false
     */
    public bool isExploding()
    {
        return _state == ExplodeState.ES_EXPLODING;
    }
}
