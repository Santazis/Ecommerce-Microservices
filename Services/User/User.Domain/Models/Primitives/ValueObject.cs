namespace User.Domain.Models.Primitives;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if(obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        var valueObject = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(0, (hash, obj) => HashCode.Combine(hash, obj.GetHashCode()));
    }
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return left is not null && right is not null && left.Equals(right);
    }
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}