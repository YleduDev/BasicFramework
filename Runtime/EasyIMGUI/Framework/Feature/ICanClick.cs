using System;

// ReSharper disable once CheckNamespace
namespace Assets.Scripts
{
    public interface ICanClick<T>
    {
        T OnClick(Action action);
    }
}