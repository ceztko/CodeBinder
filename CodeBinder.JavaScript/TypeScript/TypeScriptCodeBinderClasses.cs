// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

static class TypeScriptCodeBinderClasses
{
    public static string[] Classes
    {
        get
        {
            return new string[] {
                nameof(NativeHandle),
                nameof(BinderUtils),
                nameof(ObjectTS),
                nameof(Exception),
                nameof(HandleRef),
                nameof(BooleanRefBox),
                nameof(NumberRefBox),
                nameof(BigIntRefBox),
                nameof(StringRefBox),
                nameof(IObjectFinalizer),
                nameof(HandledObjectFinalizer),
                nameof(FinalizableObject),
                nameof(HandledObjectBase),
                nameof(HandledObject),
                nameof(IDisposable),
                nameof(IReadOnlyList),
                nameof(KeyValuePair),
                nameof(IEqualityComparer),
                nameof(NotImplementedException),
                nameof(BooleanArray),
            };
        }
    }

    public static string[] ClassesSource
    {
        get
        {
            return new string[] {
                NativeHandle,
                BinderUtils,
                ObjectTS,
                Exception,
                HandleRef,
                BooleanRefBox,
                NumberRefBox,
                BigIntRefBox,
                StringRefBox,
                IObjectFinalizer,
                HandledObjectFinalizer,
                FinalizableObject,
                HandledObjectBase,
                HandledObject,
                IDisposable,
                IReadOnlyList,
                KeyValuePair,
                IEqualityComparer,
                NotImplementedException,
                BooleanArray,
            };
        }
    }

    const string NativeHandle =
"""
export class NativeHandle
{
    #address: number;

    constructor(address: number)
    {
        this.#address = address;
    }

    get address(): number
    {
        return this.#address;
    }

    get target(): object
    {
        return napi.NativeHandleGetTarget(this.#address);
    }
}
""";

    const string BinderUtils =
"""
export class BinderUtils
{
    private static _registry : FinalizationRegistry<IObjectFinalizer>;

    private static _exception : Error | null;

    private constructor() { }

    static
    {
        BinderUtils._registry = new FinalizationRegistry<IObjectFinalizer>((finalizer: IObjectFinalizer) => {
          finalizer.finalize();
        });
    }

    static as<T>(n: any, type: any): T
    {
        if (n instanceof type)
            return n as T;
        else
            return null as any;
    }

    static cast<T>(n: any, type: any): T
    {
        if (n instanceof type)
            return n as T;
        else
            throw new Error(`Could not cast input to type ${typeof type}`);
    }

    static checkException(): void
    {
        if (BinderUtils._exception != null)
        {
            let exception = BinderUtils._exception;
            BinderUtils._exception = null;
            throw exception;
        }
    }

    static setException(exception: Error): void
    {
        BinderUtils._exception = exception;
    }

    static createNativeHandle(obj: object): NativeHandle
    {
        return new NativeHandle(napi.CreateNativeHandle(obj));
    }

    static createWeakNativeHandle(obj: object): NativeHandle
    {
        return new NativeHandle(napi.CreateWeakNativeHandle(obj));
    }

    static freeNativeHandle(nativeHandle: NativeHandle): void
    {
        napi.FreeNativeHandle(nativeHandle.address);
    }

    static keepAlive(obj: object): void
    {
        // Do nothing
    }

    // FIXME: This is garbage, this should replaced by proper AST manipulation
    static clear(arr: { set length(count: number); }): { (): void; }
    {
        arr.length = 0;
        // Returns a callable do nothing function, so it will be
        // compatible with Clear() signature
        return function (): void { };
    }

    /** @internal */
    static registerForFinalization(obj: object, finalizer: IObjectFinalizer): void
    {
        BinderUtils._registry.register(obj, finalizer);
    }
}
""";

    const string ObjectTS =
"""
// This is just for compatibility with .NET System.Object
export class ObjectTS
{
    equals(lhs: object| null): boolean
    {
        throw new Error(`Not implemented`);
    }

    getHashCode(): number
    {
        throw new Error(`Not implemented`);
    }
}
""";

    const string Exception = """
// This is just for compatibility with .NET System.Exception
export class Exception extends Error
{
    constructor(
        message?: string
    )
    {
        super(message!);
    }
}
""";

    const string HandleRef =
"""
// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
export class HandleRef extends ObjectTS
{
    wrapper: object | null;
    handle: number;

    constructor(wrapper?: object | null, handle?: number)
    {
        super();
        this.wrapper = wrapper ?? null;
        this.handle = handle ?? 0;
    }
}
""";

    const string BooleanRefBox =
"""
export class BooleanRefBox extends ObjectTS
{
    value: boolean;

    constructor(value?: boolean)
    {
        super();
        this.value = value ?? false;
    }
}
""";

    const string NumberRefBox =
"""
export class NumberRefBox extends ObjectTS
{
    value: number;

    constructor(value?: number)
    {
        super();
        this.value = value ?? 0;
    }
}
""";

    const string BigIntRefBox =
"""
export class BigIntRefBox extends ObjectTS
{
    value: bigint;

    constructor(value?: bigint)
    {
        super();
        this.value = value ?? 0n;
    }
}
""";

    const string StringRefBox =
"""
export class StringRefBox extends ObjectTS
{
    value: string | null;

    constructor(value?: string)
    {
        super();
        this.value = value ?? null;
    }
}
""";

    const string IObjectFinalizer = """
export interface IObjectFinalizer
{
    finalize(): void;
}
""";

    const string HandledObjectFinalizer = """
 export abstract class HandledObjectFinalizer extends ObjectTS implements IObjectFinalizer
 {
     handle: number = 0;

     finalize(): void
     {
         this.freeHandle(this.handle);
     }

     abstract freeHandle(handle: number): void;
 }
 """;

    const string FinalizableObject =
"""
export class FinalizableObject extends ObjectTS
{
    protected constructor()
    {
        super();
    }

    protected registerFinalizer(finalizer: IObjectFinalizer): void
    {
        BinderUtils.registerForFinalization(this, finalizer);
    }
}

""";

    const string HandledObjectBase =
"""
export class HandledObjectBase extends FinalizableObject
{
    #_handle : number;

    protected constructor(handle: number, handled: boolean)
    {
        super();
        this.#_handle = handle;
        if (handled)
        {
            let finalizer = this.createFinalizer();
            finalizer!.handle = handle;
            super.registerFinalizer(finalizer!);
        }
    }

    protected createFinalizer(): HandledObjectFinalizer | null
    {
        throw new Error(`Not implemented`);
    }

    get unsafeHandle(): number
    {
        return this.#_handle;
    }

    get handle(): HandleRef
    {
        return new HandleRef(this, this.#_handle);
    }

    get managed(): boolean
    {
        return true;
    }

    override equals(obj: object| null): boolean
    {
        if (obj === null)
            return false;

        var other: HandledObjectBase = BinderUtils.as<HandledObjectBase>(obj, HandledObjectBase);
        return this.referenceHandle === other.referenceHandle;
    }

    override getHashCode(): number
    {
        return this.referenceHandle;
    }

    protected get referenceHandle(): number
    {
        return this.unsafeHandle;
    }
}
""";

    const string HandledObject =
"""
export class HandledObject <BaseT extends HandledObject<BaseT>> extends HandledObjectBase
{
    protected constructor(handle: number, handled: boolean)
    {
        super(handle, handled);
    }

    equals(other: BaseT): boolean
    {
        return super.equals(other);
    }
}
""";

    const string IDisposable =
"""
export interface IDisposable {
    dispose(): void;
}
""";

    const string KeyValuePair =
"""
export class KeyValuePair<TKey, TValue> extends ObjectTS
{
    key:TKey;
    value:TValue;

    constructor(key?: TKey, value?: TValue)
    {
        super();
        this.key = key ?? null!;
        this.value = value ?? null!;
    }
}
""";

    const string IReadOnlyList =
"""
export interface IReadOnlyList<T> extends Iterable<T>
{
    count:number;
    getAt(index: number): T | null;
}
""";

    const string IEqualityComparer =
"""
export interface IEqualityComparer <T> 
{
}

""";

    const string NotImplementedException =
"""
export class NotImplementedException extends Error
{

}
""";

    const string BooleanArray =
"""
export class BooleanArray
{
    #actualArray: Uint8ClampedArray;

    [index: number]: boolean;

    constructor(length? : number)
    {
        this.#actualArray = new Uint8ClampedArray(length ?? 0);
        return new Proxy(this, BooleanArray.indexedHandler);
    }

    get length(): number
    {
        return this.#actualArray.length;
    }

    private static indexedHandler: ProxyHandler<BooleanArray> =
    {
        get(target, prop)
        {
            switch (prop)
            {
                case 'length':
                    return target.length;
                default:
                    return target.#actualArray[Number(prop)] !== 0;
            }
        },
        set(target, index, value): boolean
        {
            target.#actualArray[Number(index)] = value ? 1 : 0;
            return true;
        }
    }
}
""";
}
