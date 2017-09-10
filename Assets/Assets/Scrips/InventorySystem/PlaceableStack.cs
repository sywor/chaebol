using UnityEngine;

public class PlaceableStack : ScriptableObject
{
    private const int DEFAULT_MAX_COUNT = 100;
    
    public Placeable PlaceableType { get; private set; }
    public int MaxCount { get; private set; }
    public int CurrentCount { get; private set; }

    public static PlaceableStack Create(Placeable _placeableType, int _maxCount = DEFAULT_MAX_COUNT, int _currentCount = 0)
    {
        var placeableStack = CreateInstance<PlaceableStack>();
        
        placeableStack.PlaceableType = _placeableType ?? NullPlaceable.Instance;
        placeableStack.MaxCount = _maxCount;
        placeableStack.CurrentCount = _currentCount;

        return placeableStack;
    }

    public PlaceableStack()
    {
        PlaceableType =  NullPlaceable.Instance;
        MaxCount = DEFAULT_MAX_COUNT;
        CurrentCount = 0;
    }

    public bool GetPlaceable(out Placeable _placeable)
    {
        if (CurrentCount > 0 && PlaceableType.GetType() != NullPlaceable.Instance.GetType())
        {
            _placeable = Instantiate(PlaceableType);
            CurrentCount--;
            return true;
        }
        
        _placeable = NullPlaceable.Instance;
        return false;
    }

    public bool AddPlaceable(Placeable _placeable)
    {
        if (CurrentCount < MaxCount)
        {
            if (_placeable.GetType() == PlaceableType.GetType())
            {
                CurrentCount++;
                return true;
            }
            
            if (PlaceableType.GetType() == NullPlaceable.Instance.GetType())
            {
                PlaceableType = _placeable;
                CurrentCount++;
                return true;
            }
        }

        return false;
    }
}
