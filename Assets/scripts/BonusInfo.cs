/** 
 * Информация для создания бонусной фишки.
 * 
 * @author Timur Bogotov timur@-emagic.org
 */
public class BonusInfo
{
    /** Ячейка в которую нужно создать бонусную фишку. */
    public Cell cell;
    
    /** Тип фишки. */
    public ChipType cType;
    
    /** Тип бонуса. */
    public BonusType bType;
    
    /**
     * Конструктор.
     * 
     * @param cell ячейка в которую нужно создать бонусную фишку
     * @param chipType тип фишки
     * @param bonusType тип бонуса
     */
    public BonusInfo(Cell cell, ChipType chipType, BonusType bonusType)
    {
        this.cell  = cell;
        this.cType = chipType;
        this.bType = bonusType;
    }
}