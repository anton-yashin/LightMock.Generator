﻿using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IIndexer<T>
    {
        T this[int index] { get; set; }
    }
}
