namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

    using SparkTech.SDK.Executors;

    /// <summary>
    /// The tick utility
    /// </summary>
    [Trigger]
    public class TickOperation : Executable
    {
        /// <summary>
        /// The list containing the operations
        /// </summary>
        private static readonly List<TickOperation> Operations = new List<TickOperation>();

        /// <summary>
        /// The current time
        /// </summary>
        private static int currentTime;

        /// <summary>
        /// Initializes static members of the <see cref="TickOperation"/> class
        /// </summary>
        static TickOperation()
        {
            Game.OnUpdate += delegate
                {
                    currentTime = Game.Time.ToTicks();

                    foreach (var operation in Operations.Where(o => o.Active && o.lastTick + o.TickDelay > currentTime))
                    {
                        operation.lastTick = currentTime;

                        operation.Execute();
                    }
                };
        }

        /// <summary>
        /// Executes an action on next tick
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public static void ExecuteOnNextTick(Action action)
        {
            GameUpdate @delegate = null;

            @delegate = delegate
                {
                    Game.OnUpdate -= @delegate;

                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                };

            Game.OnUpdate += @delegate;
        }

        /// <summary>
        /// The action to be executed
        /// </summary>
        private readonly Action Action;

        /// <summary>
        /// Determines whether the <see cref="TickOperation"/> will be executing
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// The tick delay
        /// </summary>
        public int TickDelay;

        /// <summary>
        /// The last tick time
        /// </summary>
        private int lastTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickOperation"/> class
        /// </summary>
        /// <param name="action"></param>
        public TickOperation(Action action)
        {
            this.Action = action;

            this.lastTick = currentTime;

            Operations.Add(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="managed">Determines whether managed sources should be cleaned</param>
        protected override void Dispose(bool managed)
        {
            if (managed)
            {
                this.Active = false;
                this.TickDelay = 0;
                this.lastTick = 0;
            }

            Operations.Remove(this);
        }

        /// <summary>
        /// Handles an action
        /// </summary>
        private void Execute()
        {
            try
            {
                this.Action();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }
    }
}