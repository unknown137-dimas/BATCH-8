interface IBoard
{
    int Width {get;}
    int Height {get;}
    public bool IsPositionEmpty(Player player, Position position);
    public bool AddHeroPosition(Player player, Hero hero, Position position);
    // TODO
    // 1. Add Board Collection? Nested List?
    // 2. Add GetBoard Method
    // 3. Add UpdateHeroPosition Method
    // 4. ADd RemoveHero Method
}