/*! Reliak Timer
Copyright (C) 2013  (see AUTHORS file)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
!*/
using System;
using System.Linq;
using System.Reflection;

namespace ReliakTimer
{
    public static class SimpleMapper
    {
        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination = null) where TDestination : class, new()
        {
            destination = destination != null ? destination : new TDestination();

            CopyMatchingProperties(source, destination);

            return destination;
        }

        private static void CopyMatchingProperties<TSource, TDestination>(TSource source, TDestination dest, bool ignoreCase = true)
        {
            foreach (var destProp in typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                var prop = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.Name.Equals(destProp.Name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture) && p.PropertyType == destProp.PropertyType).FirstOrDefault();

                if (prop != null && prop.GetIndexParameters().Length == 0)
                    destProp.SetValue(dest, prop.GetValue(source, null), null);
            }
        }
    }
}
