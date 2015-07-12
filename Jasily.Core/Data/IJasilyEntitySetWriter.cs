﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Data
{
    public interface IJasilyEntitySetWriter<TEntity, TKey>
        where TEntity : class, IJasilyEntity<TKey>
    {
        /// <summary>
        /// insert one entity, return if succeed.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(TEntity entity);

        /// <summary>
        /// insert some entity, return if all succeed.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(IEnumerable<TEntity> items);

        /// <summary>
        /// update entity by Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TEntity entity);

        /// <summary>
        /// remove entity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(TKey id);
    }
}