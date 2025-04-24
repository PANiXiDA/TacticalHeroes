using System;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Domain.Entities.Models;
using Assets.Scripts.Repositories.Interfaces.Core;

namespace Assets.Scripts.Repositories.Interfaces
{
    public interface IBuildsRepository : IBaseRepository<Build, Guid, BuildsSearchParams, BuildsConvertParams>
    {
    }
}
