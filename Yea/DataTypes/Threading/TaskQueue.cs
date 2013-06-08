#region Usings

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes.Threading
{
    /// <summary>
    ///     Class that helps with running tasks in parallel
    ///     on a set of objects (that will come in on an ongoing basis, think producer/consumer situations)
    /// </summary>
    /// <typeparam name="T">Object type to process</typeparam>
    public class TaskQueue<T> : BlockingCollection<T>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="capacity">Number of items that are allowed to be processed in the queue at one time</param>
        /// <param name="processItem">Action that is used to process each item</param>
        /// <param name="handleError">Handles an exception if it occurs (defaults to eating the error)</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public TaskQueue(int capacity, Action<T> processItem, Action<Exception> handleError = null)
            : base(new ConcurrentQueue<T>())
        {
            ProcessItem = processItem;
            HandleError = handleError.NullCheck(x => { });
            CancellationToken = new CancellationTokenSource();
            Tasks = new Task[capacity];
            capacity.Times(x => Tasks[x] = Task.Factory.StartNew(Process));
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Token used to signal cancellation
        /// </summary>
        private CancellationTokenSource CancellationToken { get; set; }

        /// <summary>
        ///     Group of tasks that the queue uses
        /// </summary>
        private Task[] Tasks { get; set; }

        /// <summary>
        ///     Action used to process an individual item in the queue
        /// </summary>
        private Action<T> ProcessItem { get; set; }

        /// <summary>
        ///     Called when an exception occurs when processing the queue
        /// </summary>
        private Action<Exception> HandleError { get; set; }

        /// <summary>
        ///     Determines if it has been cancelled
        /// </summary>
        public bool IsCanceled
        {
            get { return CancellationToken.IsCancellationRequested; }
        }

        /// <summary>
        ///     Determines if it has completed all tasks
        /// </summary>
        public bool IsComplete
        {
            get { return Tasks.TrueForAll(x => x.IsCompleted); }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Adds the item to the queue to be processed
        /// </summary>
        /// <param name="item">Item to process</param>
        public void Enqueue(T item)
        {
            if (IsCompleted || IsCanceled)
                throw new InvalidOperationException("TaskQueue has been stopped");
            Add(item);
        }

        /// <summary>
        ///     Cancels the queue from processing
        /// </summary>
        /// <param name="wait">Determines if the function should wait for the tasks to complete before returning</param>
        public void Cancel(bool wait = false)
        {
            if (IsCompleted || IsCanceled)
                return;
            CancellationToken.Cancel(false);
            if (wait)
                Task.WaitAll(Tasks);
        }

        /// <summary>
        ///     Processes the queue
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void Process()
        {
            while (true)
            {
                try
                {
                    ProcessItem(Take(CancellationToken.Token));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }

        /// <summary>
        ///     Disposes of the objects
        /// </summary>
        /// <param name="disposing">True to dispose of all resources, false only disposes of native resources</param>
        protected override void Dispose(bool disposing)
        {
            if (Tasks != null)
            {
                Cancel(true);
                foreach (var task in Tasks)
                {
                    task.Dispose();
                }
                Tasks = null;
            }
            if (CancellationToken != null)
            {
                CancellationToken.Dispose();
                CancellationToken = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}