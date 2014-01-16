
enum FallDirection
{
    DOWN = 0,
    LEFT_DOWN,
    RIGHT_DOWN
}

interface IFallable
{
    void animateFalling(FallDirection fallDirection);
}