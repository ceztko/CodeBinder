using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    static class ObjCClasses
    {
        public const string CBException_h = @"#ifndef CB_EXCPETION
#define CB_EXCPETION

#import <Foundation/Foundation.h>

// Substitute for .NET Excpetion
@interface CBException : NSException
- (id)init;
- (id)init:(NSString *)message;
@end

#endif // CB_EXCPETION
";

        public const string CBException_mm = @"#import ""CBException.h""

@implementation CBException

- (id)init
{
    self = [super init];
    if (self == nil)
        return nil;

    return self;
}
- (id)init:(NSString *)message
{
    self = [super initWithName:@""Exception"" reason:message userInfo:nil];
    if (self == nil)
        return nil;

    return self;
}
@end";

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
@"#import ""CBKeyValuePair.h""

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
@"#import ""CBHandleRef.h""

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

        public const string CBBinderUtils_h =
@"#ifndef CB_BINDERUTILS
#define CB_BINDERUTILS

#import <Foundation/Foundation.h>

inline BOOL CBBinderEquals(NSString *lhs, NSString *rhs)
{
    if (lhs == nil)
    {
        if (rhs == nil)
            return YES;
        else
            return NO;
    }
    else
    {
        return [lhs isEqualToString:rhs];
    }
}


template<typename T>
T * CBBinderAsOperator(NSObject *obj)
{
    if (obj == nil)
        return nil;
	
	if ([obj isKindOfClass:[T class]])
		return (T *)obj;
	else
		return nil;
}

template<typename T>
T * CBBinderCastOperator(NSObject *obj)
{
    if (obj == nil)
        return nil;
	
	if ([obj isKindOfClass:[T class]])
		return (T *)obj;
	else
		@throw [NSException exceptionWithName:@""InvalidCastException"" reason:nil userInfo:nil];
}

#endif // CB_BINDERUTILS
";
    }
}
