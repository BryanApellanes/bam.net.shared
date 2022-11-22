namespace Bam.Net
{
    public class BsonTransformer<TData> : ValueTransformer<TData, byte[]>
    {
        public override TData ReverseTransform(byte[] output)
        {
            return GetReverseTransformer().ReverseTransform(output);
        }

        public override byte[] Transform(TData input)
        {
            return input.ToBson();
        }

        public override IValueReverseTransformer<byte[], TData> GetReverseTransformer()
        {
            return new BsonReverseTransformer<TData>();
        }
    }
}
