using System;

namespace KitchenSpeedrunLayouts.Injections
{
    internal interface IInitArgs { }
    internal abstract class BaseModContainer<TArgs> where TArgs : IInitArgs, new()
    {
        public abstract string DependencyName { get; }

        public BaseModContainer(TArgs args)
        {
            try
            {
                Init(args);
            }
            catch (Exception ex)
            {
                Main.LogError($"Failed to initialize {DependencyName}: {ex.Message}\n{ex.StackTrace}");
            }
        }
        protected abstract void Init(TArgs args);
    }
}
