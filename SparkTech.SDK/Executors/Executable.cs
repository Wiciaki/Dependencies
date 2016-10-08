namespace SparkTech.SDK.Executors
{
    using System;
    using System.Security;

    using NLog;

    /// <summary>
    /// Represents a disposable class
    /// </summary>
    public abstract class Executable : IDisposable
    {
        /// <summary>
        /// Determines whether this instance has already been disposed
        /// </summary>
        private bool toBeDisposed;

        /// <summary>
        /// The logger for the current instance
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Executable"/> class
        /// </summary>
        protected Executable()
        {
            this.toBeDisposed = true;

            this.logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Executable"/> class
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Executable()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Determines whether the current instance is not awaiting a disposal
        /// </summary>
        public bool IsDisposed => !this.toBeDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.IsDisposed)
                return;

            this.toBeDisposed = false;
            GC.SuppressFinalize(this);

            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="managed">Determines whether managed sources should be cleaned</param>
        protected abstract void Dispose(bool managed);

        /// <summary>
        /// Logs the exception
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="message">The message</param>
        [SecuritySafeCritical]
        protected void Log(Exception ex, string message)
        {
            this.logger.Error(ex, message);
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="ex">The exception object</param>
        [SecuritySafeCritical]
        protected void Log<TException>(TException ex) where TException : Exception
        {
            this.logger.Error(ex);
        }
    }
}