namespace  Framework
{
    using System;

    public class DisposableObject : IDisposable
    {
        private Boolean mDisposed = false;

        ~DisposableObject()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Overrides it, to dispose managed resources.
        protected virtual void DisposeGC()
        {
        }

        // Overrides it, to dispose unmanaged resources
        protected virtual void DisposeNGC()
        {
        }

        private void Dispose(bool disposing)
        {
            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeGC();
            }

            DisposeNGC();

            mDisposed = true;
        }
    }
}