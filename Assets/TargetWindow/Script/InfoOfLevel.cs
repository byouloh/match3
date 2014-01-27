using UnityEngine;
using System.Collections;

/** Тип уровня. */
public enum levelTarget
{
    SCORE_TARGET = 0,
    JELLY_TARGET,
    INGREDIENT_TARGET,
    TIME_TARGET
}
/**
 * Информация уровня.
 */
public class InfoOfLevel : MonoBehaviour {

    int LevelNumber;    // Номер уровня.
    int record;         // Рекорд.
    int oneStar;        // Количество очков которые надо набрать для одной звезды.
    int twoStar;        // Количество очков которые надо набрать для двух звезд.
    int threeStar;      // Количество очков которые надо набрать для трех звезд.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
