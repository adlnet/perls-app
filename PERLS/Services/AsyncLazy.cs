using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PERLS.Services
{
    /// <summary>
    /// A class that provides a simple, lightweight implementation of lazy initialization, using a method source as a value factory.
    /// </summary>
    /// <typeparam name="T">The type of object to lazily return.</typeparam>
    class AsyncLazy<T> : Lazy<Task<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy{T}"/> class.
        /// </summary>
        /// <param name="valueFactory">A function to generate values.</param>
        public AsyncLazy(Func<T> valueFactory) : base(() => Task.Factory.StartNew(valueFactory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy{T}"/> class.
        /// </summary>
        /// <param name="taskFactory">An async function to generate values.</param>
        public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
        {
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="AsyncLazy{T}"/>.
        /// </summary>
        /// <returns>An awaiter instance.</returns>
        public TaskAwaiter<T> GetAwaiter()
        {
            return Value.GetAwaiter();
        }
    }
}
