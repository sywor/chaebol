public static class NullPlaceableStack
{
    private static PlaceableStack instance;

    public static PlaceableStack Instance
    {
        get
        {
            if (instance == null)
            {
                instance = PlaceableStack.Create(NullPlaceable.Instance, 0);
            }

            return instance;
        }
    }
}
