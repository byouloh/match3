/**
 * Информация о типе ячейки.
 * 
 * @author Timur Bogotov timur@e-magic.org
 */
public class ChipInfo
{
    /** Количество очков за взрыв фишки. */
    public uint explodePoints;
    
    /**
     * Конструктор.
     */
    public ChipInfo(uint explodePoints)
    {
        this.explodePoints = explodePoints;
    }
}