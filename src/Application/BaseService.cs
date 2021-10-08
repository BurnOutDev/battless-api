using System;
using System.Collections.Generic;

namespace Application
{
    public interface BaseService<TCreateRequest, TUpdateRequest, TResponse>
    {
        IEnumerable<TResponse> GetAll();
        TResponse GetById(Guid id);
        TResponse Create(TCreateRequest model);
        TResponse Update(Guid id, TUpdateRequest model);
        void Delete(Guid id);
    }
}
