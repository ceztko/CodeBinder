namespace CodeBinder.Apple
{
    public struct NSFastEnumerationState
    {

    }

    // Fake (id) struct that allows setting like a buffer, see
    // https://developer.apple.com/documentation/foundation/nsfastenumeration/1412867-countbyenumeratingwithstate?language=objc
    public struct id
    {
        public object this[int index]
        {
            set { }
        }
    }

    public struct NSInteger
    {
        public static implicit operator long(NSInteger d) => 0;
        public static implicit operator NSInteger(long b) => new NSInteger();
        public static implicit operator int(NSInteger d) => 0;
        public static implicit operator NSInteger(int b) => new NSInteger();
    }

    public struct NSUInteger
    {
        public static implicit operator ulong(NSUInteger d) => 0;
        public static implicit operator NSUInteger(ulong b) => new NSUInteger();
        public static implicit operator uint(NSUInteger d) => 0;
        public static implicit operator NSUInteger(uint b) => new NSUInteger();
    }
}
