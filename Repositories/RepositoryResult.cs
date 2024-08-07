﻿namespace Onyx.Repositories
{
    public class RepositoryResult<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
