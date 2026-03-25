interface ICollectable
{
    bool IsPersistent { get; }
    int Points { get; }
}

sealed class Coin : ICollectable
{
    public bool IsPersistent => false;
    public int Points { get; }

    public Coin(int points = 1)
    {
        Points = points;
    }
}

sealed class Key : ICollectable
{
    public bool IsPersistent => true;
    public int Points => 5;
    public int DoorId { get; }
    public string Name { get; }

    public Key(int doorId = 0)
    {
        DoorId = doorId;
        Name = $"Key {doorId + 1}";
    }
}
