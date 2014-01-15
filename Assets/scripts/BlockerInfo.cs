using System.Collections.Generic;

/**
 * Информация о типе блокирующего элемента.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class BlockerInfo
{
    /** Количество очков за взрыв блокирующего элемента. */
    public uint explodePoints;
    
    /**
     * Конструктор.
     */
    public BlockerInfo(uint explodePoints)
    {
        this.explodePoints = explodePoints;
    }
}