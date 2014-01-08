using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Класс, в котором хранится информация о ряде(тройке фишек).
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class ThreeLine
{
    /** Номер строки на котором стоит первая фишка*/
    public int ai;
    
    /** Номер столбца на котором стоит первая фишка*/
    public int aj;
    
    /** Номер строки на котором стоит вторая фишка*/
    public int bi;
    
    /** Номер столбца на котором стоит вторая фишка*/
    public int bj;
    
    /** Номер строки на котором стоит перемещаемая фишка*/
    public int moveI;
    
    /** Номер столбца на котором стоит перемещаемая фишка*/
    public int moveJ;
    
    /**
     * Конструктор.
     * 
     * @param ai номер строки первого элемента
     * @param aj номер столбца первого элемента
     * @param bi номер строки второго элемента
     * @param bj номер столбца второго элемента
     * @param moveI номер строки перемещаемого элемента
     * @param moveJ номер столбца перемещаемого элемента
     */
    public ThreeLine(int ai, int aj, int bi, int bj, int moveI, int moveJ)
    {
        this.ai = ai;
        this.aj = aj;
        this.bi = bi;
        this.bj = bj;
        this.moveI = moveI;
        this.moveJ = moveJ;
    }
    
    /**
     * Возвращает список смещений координат, которые образуют ход.
     * 
     * @return List<ThreeLine> список смещений
     */
    public static List<ThreeLine> getOffsetList()
    {
        List<ThreeLine> offset = new List<ThreeLine>();
        
        offset.Add(new ThreeLine(-1, 0, -2, 0, 0, -1));
        offset.Add(new ThreeLine(-1, 0, -2, 0, 0, 1));
        offset.Add(new ThreeLine(-1, 0, -2, 0, -1, 0));
        
        offset.Add(new ThreeLine(1, 0, 2, 0, 0, -1));
        offset.Add(new ThreeLine(1, 0, 2, 0, 0, 1));
        offset.Add(new ThreeLine(1, 0, 2, 0, 1, 0));
        
        offset.Add(new ThreeLine(0, -1, 0, -2, -1, 0));
        offset.Add(new ThreeLine(0, -1, 0, -2, 1, 0));
        offset.Add(new ThreeLine(0, -1, 0, -2, 0, 1));
        
        offset.Add(new ThreeLine(0, 1, 0, 2, -1, 0));
        offset.Add(new ThreeLine(0, 1, 0, 2, 1, 0));
        offset.Add(new ThreeLine(0, 1, 0, 2, 0, -1));
        
        offset.Add(new ThreeLine(0, -1, 0, 1, -1, 0));
        offset.Add(new ThreeLine(0, -1, 0, 1, 1, 0));
        offset.Add(new ThreeLine(-1, 0, 1, 0, 0, -1));
        offset.Add(new ThreeLine(-1, 0, 1, 0, 0, 1));
        
        return offset;
    }
}
