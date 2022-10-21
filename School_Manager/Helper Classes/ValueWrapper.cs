namespace School_Manager
{
    //A helper class to use VALUE types as REFRENCE types
    public class ValueWrapper<T>
    {
        public T Value { get; set; }

        public ValueWrapper() { }

        public ValueWrapper(T value)
        {
            Value = value;
        }

        public static implicit operator T(ValueWrapper<T> wrapper)
        {
            if (wrapper == null)
            {
                return default(T);
            }
            return wrapper.Value;
        }
    }
}
