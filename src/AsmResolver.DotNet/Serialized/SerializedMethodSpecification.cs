﻿using System;
using System.Collections.Generic;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Metadata;
using AsmResolver.PE.DotNet.Metadata.Blob;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace AsmResolver.DotNet.Serialized
{
    /// <summary>
    /// Represents a lazily initialized implementation of <see cref="MethodSpecification"/>  that is read from a
    /// .NET metadata image. 
    /// </summary>
    public class SerializedMethodSpecification : MethodSpecification
    {
        private readonly SerializedModuleDefinition _parentModule;
        private readonly MethodSpecificationRow _row;
        
        /// <summary>
        /// Creates a method specification from a method specification metadata row.
        /// </summary>
        /// <param name="parentModule"></param>
        /// <param name="token">The token to initialize the method specification for.</param>
        /// <param name="row">The metadata table row to base the method specification on.</param>
        public SerializedMethodSpecification(SerializedModuleDefinition parentModule, MetadataToken token, MethodSpecificationRow row)
            : base(token)
        {
            _parentModule = parentModule ?? throw new ArgumentNullException(nameof(parentModule));
            _row = row;
        }

        /// <inheritdoc />
        protected override IMethodDefOrRef GetMethod()
        {
            var encoder = _parentModule.DotNetDirectory.Metadata
                .GetStream<TablesStream>()
                .GetIndexEncoder(CodedIndex.MethodDefOrRef);
            
            var methodToken = encoder.DecodeIndex(_row.Method);
            return _parentModule.TryLookupMember(methodToken, out var member)
                ? member as IMethodDefOrRef
                : null;
        }

        /// <inheritdoc />
        protected override GenericInstanceMethodSignature GetSignature()
        {
            var reader = _parentModule.DotNetDirectory.Metadata
                .GetStream<BlobStream>()
                .GetBlobReaderByIndex(_row.Instantiation);
            
            return GenericInstanceMethodSignature.FromReader(_parentModule, reader);
        }

        /// <inheritdoc />
        protected override IList<CustomAttribute> GetCustomAttributes() => 
            _parentModule.GetCustomAttributeCollection(this);
    }
}