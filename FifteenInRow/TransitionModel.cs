namespace FifteenInRow
{
    public sealed class TransitionModel
    {
        public TransitionModel(int fromIndex, int toIndex, int value)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
            Value = value;
        }

        public int FromIndex { get; }
        public int ToIndex { get; }
        public int Value { get; set; }
    }
}
