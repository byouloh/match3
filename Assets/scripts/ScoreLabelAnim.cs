using UnityEngine;
using System.Collections;

/**
 * Скрипт для анимации и плавного исчезновения текста.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class ScoreLabelAnim : MonoBehaviour
{
    /** Скорость анимации. */
    public float speed = 1f;
    
    /** Время анимации(в секундах). */
    public float time = 1f;
    
    /** Скрипт для отображения текста. */
    private UILabel _uiLabelScript;
    
    /** Текущее время выполнения анимации. */
    private float _currentTime;
    
    /** Инициализация. */
    void Start () {
        _uiLabelScript = GetComponent<UILabel>();
        
        if (_uiLabelScript == null) {
            this.enabled = false;
        } else {
            _currentTime = 0;
        }
	}
    
    /** Событие при включении(активации) скрипта. */
    void OnEnable()
    {
        _uiLabelScript = GetComponent<UILabel>();
        
        if (_uiLabelScript == null) {
            this.enabled = false;
        }
    }
	
    /** Обновление позиции в каждом кадре. */
	void Update () {
        _currentTime += Time.deltaTime;
        
        if (_currentTime >= time) {
            Destroy(gameObject);
        } else {
            transform.Translate(0, speed * Time.deltaTime, 0);
            
            if (_currentTime < time * 0.15) {
                _uiLabelScript.alpha = _currentTime / (time * 0.15f);
            } else
            if (_currentTime < time * 0.7) {
                _uiLabelScript.alpha = 1;
            } else {
                _uiLabelScript.alpha = 1 - (_currentTime / time - 0.7f) / 0.3f;
            }
        }
	}
}
