public class NullPlaceable : Placeable
{
    private static NullPlaceable instance;
    public static NullPlaceable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NullPlaceable>();
            }

            if (instance == null)
            {
                instance = CreateInstance<NullPlaceable>();
            }

            return instance;
        }
    }
    
    public override void Update()
    {
        //Do nothing
    }
}
