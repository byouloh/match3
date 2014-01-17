using UnityEngine;

/**
 * Направление падения.
 */
public enum FallDirection
{
    DOWN = 0,
    LEFT_DOWN,
    RIGHT_DOWN
}

/**
 * Интерфейс для падающих обьектов.
 */
interface IFallable
{
    /**
     * Запускаем анимацию падения.
     */
    void onFallingStart(FallDirection fallDirection);

    /**
     * Останавливаем анимацию падения.
     */
    void onFallingStop();

    /**
     * Функия перерасчитывает скорость падения фищек.
     */
    void onFalling(float fallSpeed);
    Transform getTransform();

}