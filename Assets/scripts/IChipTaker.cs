
interface IChipTaker
{
    /**
     * Получает и возвращает фишку из соседней ячейки сверху.
     * 
     * @param caller Ячейка, запрашивающая фишку.
     * 
     * @return Возвращает фишку, если она создана или найдена, иначе null
     */
    Chip takeChip(Cell caller);
}