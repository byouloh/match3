﻿using UnityEngine;
using System.Collections;

/**
 * Информация об уровне
 */
public class Level
{
    /** Номер уровня. */
    public int levelId;
    
    /** Максимальное количество ходов(целое положительное значение, 0-неограничено). */
    public int maxMoves;
    
    /** Количество ходов, сделанные в текущем уровне. */
    public int moves;
    
    /** Количество очков на текущем уровне. */
    public int points;
    
    /**
     * Типы фишек, которые используются на уровне.
     * 
     * Битовая маска типов [00000000 00POYBGR]
     * R = RED
     * G = GREEN
     * B = BLUE
     * Y = YELLOW
     * O = ORANGE
     * P = PURPLE
     */
    public uint chipTypes;
    
    /** Количество очков для получения первой звезды. */
    public int needPointsFirstStar;
    
    /** Количество очков для получения второй звезды. */
    public int needPointsSecondStar;
    
    /** Количество очков для получения третьей звезды. */
    public int needPointsThirdStar;
}
