using System;
using System.Runtime.InteropServices;

namespace CodeBinder
{
    public class HandledObject<BaseT> : HandledObjectBase
        where BaseT : HandledObject<BaseT>
    {
        protected HandledObject(IntPtr handle, bool handled)
            : base(handle, handled) { }

        protected static bool Equals(BaseT? lhs, BaseT? rhs)
        {
            if (lhs == null)
            {
                if (rhs == null)
                    return true;
                else
                    return false;
            }
            else if (rhs == null)
            {
                return false;
            }

            return lhs.ReferenceHandle == rhs.ReferenceHandle;
        }

        protected static bool NotEquals(BaseT? lhs, BaseT? rhs)
        {
            if (lhs == null)
            {
                if (rhs == null)
                    return false;
                else
                    return true;
            }
            else if (rhs == null)
            {
                return true;
            }

            return lhs.ReferenceHandle != rhs.ReferenceHandle;
        }

        public bool Equals(BaseT? other)
        {
            return base.Equals(other);
        }
    }

    public class HandledObjectBase : FinalizableObject
    {
        IntPtr _handle;

        protected HandledObjectBase(IntPtr handle, bool handled)
        {
            _handle = handle;

            if (handled)
            {
                var finalizer = CreateFinalizer();
                finalizer.Handle = handle;
                RegisterFinalizer(finalizer);
            }
        }

        protected virtual HandledObjectFinalizer CreateFinalizer()
        {
            throw new NotImplementedException("The finalizer must be supplied");
        }

        /// <summary>
        /// Returns an handle that can be overridden to
        /// specify a different one for equality check
        /// </summary>
        protected virtual IntPtr ReferenceHandle
        {
            get { return UnsafeHandle; }
        }

        public HandleRef Handle
        {
            get { return new HandleRef(this, _handle); }
        }

        public IntPtr UnsafeHandle
        {
            get { return _handle; }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HandledObjectBase);
        }

        public bool Equals(HandledObjectBase? obj)
        {
            if (obj == null)
                return false;

            return ReferenceHandle == obj.ReferenceHandle;
        }

        public override int GetHashCode()
        {
            return ReferenceHandle.GetHashCode();
        }
    }

    public abstract class FinalizableObject
    {
        protected FinalizableObject() { }

        protected void RegisterFinalizer(IObjectFinalizer finalizer)
        {
            BinderUtils.RegisterForFinalization(this, finalizer);
        }
    }

    public abstract class HandledObjectFinalizer : IObjectFinalizer
    {
        internal IntPtr Handle { get; set; }

        ~HandledObjectFinalizer()
        {
            FreeHandle(Handle);
        }

        public abstract void FreeHandle(IntPtr handle);
    }

    public interface IObjectFinalizer
    {

    }
}
