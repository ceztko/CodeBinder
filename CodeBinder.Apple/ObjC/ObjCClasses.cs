using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    static class ObjCClasses
    {
        public const string CBIReadOnlyList_h = @"#ifndef CB_IREADONLYLIST
#define CB_IREADONLYLIST

// Substitute for .NET IReadOnlyList
@protocol CBIReadOnlyList
@end

#endif // CB_IREADONLYLIST
";

        public const string CBIEqualityCompararer_h = @"#ifndef CB_IEQUALITYCOMPARARER
#define CB_IEQUALITYCOMPARARER

// Substitute for .NET IReadOnlyList
@protocol CBIEqualityCompararer
@end

#endif // CB_IEQUALITYCOMPARARER
";

        public const string CBIDisposable_h = @"#ifndef CB_IDISPOSABLE
#define CB_IDISPOSABLE

// Substitute for .NET IDisposable
@protocol CBIDisposable
- (void)dispose;
@end

#endif // CB_IDISPOSABLE
";

        public const string CBKeyValuePair_h =
@"#ifndef CB_KEYVALUEPAIR
#define CB_KEYVALUEPAIR

#import <Foundation/Foundation.h>

// Substitute for .NET KeyValuePair
@interface CBKeyValuePair<__covariant KeyType, __covariant ValueType> : NSObject
{
@private
    KeyType _key;
    ValueType _value;
}

@property (nonatomic,readonly) KeyType key;
@property (nonatomic,readonly) ValueType value;

- (id)init:(KeyType)key :(ValueType)value;
- (KeyType)key;
- (ValueType)value;
@end

#endif // CB_KEYVALUEPAIR
";

        public const string CBKeyValuePair_mm =
@"#include ""CBKeyValuePair.h""

@implementation CBKeyValuePair

- (id)init:(id)key :(id)value
{
    self = [super init];
    if (self == nil)
        return nil;

    _key = key;
    _value = value;
    return self;
}

- (id)key
{
    return _key;
}

- (id)value
{
    return _value;
}
@end
";

        public const string CBHandleRef_h =
@"#ifndef CB_HANDLEREF
#define CB_HANDLEREF

#import <Foundation/Foundation.h>

// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
@interface CBHandleRef : NSObject
{
    @private
    NSObject * _wrapper;
    void * _handle;
}

@property (nonatomic,readonly) NSObject * wrapper;
@property (nonatomic,readonly) void * handle;

- (id)init;
- (id)init:(NSObject *)wrapper :(void *)handle;
- (NSObject *)wrapper;
- (void *)handle;
@end

#endif // CB_HANDLEREF
";

        public const string CBHandleRef_mm =
@"#include ""CBHandleRef.h""

@implementation CBHandleRef

- (id)init
{
    self = [super init];
    if (self == nil)
        return nil;

    _wrapper = nil;
    _handle = 0;
    return self;
}

- (id)init:(NSObject *)wrapper :(void *)handle;
{
    self = [super init];
    if (self == nil)
        return nil;

    _wrapper = wrapper;
    _handle = handle;
    return self;
}

- (NSObject *)wrapper
{
    return _wrapper;
}

- (void *)handle
{
    return _handle;
}

@end
";

        public const string BinderUtils =
@"public class BinderUtils
{
    // Simulates as operator https://stackoverflow.com/a/148949/213871
    public static <T> T as(Object obj, Class<T> clazz)
    {
        if (clazz.isInstance(obj))
            return clazz.cast(obj);

        return null;
    }

    public static boolean equals(String lhs, String rhs)
    {
        if (lhs == null)
        {
            if (rhs == null)
                return true;
            else
                return false;
        }
        else
        {
            return lhs.equals(rhs);
        }
    }

    // TODO: Consider moving this methods to generation of exising .NET class BinderUtils.
    // See CodeBinder.Redist
    public static native long newGlobalRef(Object obj);
    public static native void deleteGlobalRef(long globalref);
    public static native long newGlobalWeakRef(Object obj);
    public static native void deleteGlobalWeakRef(long globalref);
}";
    }
}
