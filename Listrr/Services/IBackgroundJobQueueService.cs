using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Listrr.Data;
using Listrr.Data.Trakt;

namespace Listrr.Services
{
    public interface IBackgroundJobQueueService
    {

        void Queue(TraktList list);

    }
}