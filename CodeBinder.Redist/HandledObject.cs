using System;
using System.Runtime.InteropServices;

namespace CodeBinder
{
    public class HandledObject<BaseT> : HandledObject
        where BaseT : HandledObject<BaseT>
    {
        protected HandledObject() { }

        protected HandledObject(IntPtr handle)
            : base(handle) { }

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

    public class HandledObject
    {
        IntPtr _handle;

        protected HandledObject()
        {
            _handle = IntPtr.Zero;
        }

        protected HandledObject(IntPtr handle)
        {
            _handle = handle;
        }

        ~HandledObject()
        {
            if (Managed)
                FreeHandle(_handle);
        }

        protected void SetHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new Exception("Handle must be non null");

            if (_handle != IntPtr.Zero)
                throw new Exception("Handle is already set");

            _handle = handle;
        }

        protected virtual void FreeHandle(IntPtr handle)
        {
            throw new NotImplementedException();
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

        public virtual bool Managed
        {
            get { return true; }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HandledObject);
        }

        public bool Equals(HandledObject? obj)
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
}
