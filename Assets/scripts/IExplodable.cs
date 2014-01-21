
/**
 * Интерфейс для взрываемых объектов.
 */
public interface IExplodable
{
    /**
     * Взрывает объект.
     * 
     * @param callback обработчик события завершения анимации взрыва
     */
	bool explode(Callback callback);
    
    /**
     * Возвращает количество очков за взрыв объекта.
     * 
     * @return uint количество очков(целое, неотрицательное)
     */
    uint getExplodePoints();
    
    /**
     * Задает количество очков за взрыв объекта.
     * 
     * @param количество очков(целое, неотрицательное)
     */
    void setExplodePoints(uint explodePoints);
}
