using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace MyAbpDemo.Infrastructure
{
    public static class AutoMapExtensions
    {
        public static List<TDestination> MapToList<TDestination>(this object source)
        {
            return Mapper.Map<List<TDestination>>(source);
        }
    }
}
