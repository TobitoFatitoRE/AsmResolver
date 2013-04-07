﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TUP.AsmResolver.NET.Specialized
{
    public interface ISpecification
    {
        MemberReference TransformWith(IGenericParametersProvider paramProvider, IGenericArgumentsProvider argProvider);
    }
}
