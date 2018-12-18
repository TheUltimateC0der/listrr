﻿using System.Threading.Tasks;

namespace Listrr.BackgroundJob
{
    public interface IRecurringJob
    {
        Task Execute();
    }


    public interface IRecurringJob<T>
    {
        Task Execute(T param);
    }
}